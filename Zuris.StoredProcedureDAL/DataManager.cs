using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;

namespace Zuris.SPDAL
{
    //we need to be able to hit multiple databases...

    public delegate void DataAccessErrorOccurred(Exception ex, string message, string command);

    public delegate void DataAccessEventOccurred(string message, string command);

    /// <summary>
    /// The DataManager is the core object for managing database interactivity. It contains the active
    /// connection for the current execution thread, facilitates transaction management, and provides
    /// access to the entity manager.
    /// </summary>
    public abstract class DataManager : IDataManager
    {
        public const int RetrySleepMilliseconds = 10;
        public const int NumberConnectionRetrys = 3;

        private static DataManagerSettings _settings = new DataManagerSettings();

        private readonly Stack<GenericTransaction> _transactionStack = new Stack<GenericTransaction>();
        private IDbConnection _primaryConnection;
        private IDbTransaction _currentTransaction;
        private bool _isDoomed;
        private Dictionary<Type, object> _entityManagers = new Dictionary<Type, object>();

        /// <summary>
        /// Gets the settings object.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public static DataManagerSettings Settings
        {
            get { return _settings; }
        }

        public static event DataAccessEventOccurred LogEvent;

        public static event DataAccessErrorOccurred LogError;

        protected void OnLogEvent(string message, string command = null)
        {
            if (LogEvent != null) LogEvent(message, command);
        }

        protected void OnLogError(Exception ex, string message = null, string command = null)
        {
            if (LogError != null) LogError(ex, message, command);
        }

        /// <summary>
        /// Provides the connection string for this data manager to use when creating connections.
        /// </summary>
        protected abstract string ConnectionString { get; }

        /// <summary>
        /// Provides the database provider string. For example, "System.Data.SqlClient" would be used for Microsoft SQL Server.
        /// </summary>
        protected abstract string ProviderString { get; }

        /// <summary>
        /// Gets the retryable exception policy.
        /// </summary>
        /// <value>
        /// The retryable exception policy.
        /// </value>
        protected abstract IEvaluateRetryable RetryableExceptionPolicy { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract IDbCommandLogHelper CommandLogHelper { get; }

        /// <summary>
        /// Gets the provider factory for this database type (based on the provider string).
        /// </summary>
        protected virtual DbProviderFactory ProviderFactory
        {
            get
            {
                DbProviderFactory factory = DbProviderFactories.GetFactory(ProviderString);

                if (factory == null)
                {
                    throw new ZurisException<ZurisFrameworkErrorCode>(ZurisFrameworkErrorCode.AdoNetProviderNotFound, "The configured ADO.Net provider does not exist: " + ProviderString);
                }
                return factory;
            }
        }

        /// <summary>
        /// Creates a meta data manager that can get routine information from the database.
        /// </summary>
        /// <returns></returns>
        public abstract IMetaDataManager CreateMetaDataManager();

        /// <summary>
        /// Gets the primary connection for the current thread.
        /// </summary>
        public IDbConnection PrimaryConnection
        {
            get
            {
                if (_primaryConnection != null && _primaryConnection.State == ConnectionState.Closed)
                {
                    _primaryConnection = null;
                }

                if (_primaryConnection == null)
                {
                    _primaryConnection = CreateUnmanagedConnection();
                    _primaryConnection.Open();
                    if (Settings.CommandLoggingEnabled) OnLogEvent("Connection Opened: " + _primaryConnection.ConnectionString);
                }
                return _primaryConnection;
            }
        }

        /// <summary>
        /// Creates an connection that will not be managed by the DataManager (for transactions, entities, etc).
        /// </summary>
        /// <returns>
        /// A connection to the database.
        /// </returns>
        public IDbConnection CreateUnmanagedConnection()
        {
            var conn = ProviderFactory.CreateConnection();
            conn.ConnectionString = ConnectionString;
            return conn;
        }

        /// <summary>
        /// Creates a database command using the current connection.
        /// </summary>
        /// <returns>
        /// A new Command object.
        /// </returns>
        public IDbCommand CreateCommand()
        {
            var cmd = RunWithRetry(() => { return PrimaryConnection.CreateCommand(); });
            if (this._currentTransaction != null) cmd.Transaction = this._currentTransaction;
            return cmd;
        }

        /// <summary>
        /// Creates a data adapter of this provider has one.
        /// </summary>
        /// <returns>
        /// A new Command object.
        /// </returns>
        public IDbDataAdapter CreateDataAdapter()
        {
            return ProviderFactory.CreateDataAdapter();
        }

        /// <summary>
        /// Enrolls the specified commandinto the current transaction.
        /// </summary>
        /// <param name="command">The command to enroll.</param>
        public void Enroll(IDbCommand command)
        {
            if (command.Connection == null || command.Connection != PrimaryConnection)
                command.Connection = PrimaryConnection;

            if (this._currentTransaction != null)
                command.Transaction = this._currentTransaction;
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public void CommitTransaction()
        {
            if (_transactionStack.Count > 0)
            {
                _transactionStack.Peek().Commit();
            }
        }

        /// <summary>
        /// Rollbacks the current transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            if (_transactionStack.Count > 0)
            {
                _transactionStack.Peek().Rollback();
            }
        }

        /// <summary>
        /// Returns true if the unit of work is currently in an active transaction.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in active transaction; otherwise, <c>false</c>.
        /// </value>
        public bool IsInActiveTransaction
        {
            get { return _transactionStack.Count > 0; }
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns></returns>
        public IGenericTransaction BeginScopeTransaction(IsolationLevel isolationLevel)
        {
            return BeginTransaction(isolationLevel, true);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        public IGenericTransaction BeginScopeTransaction()
        {
            return BeginTransaction(IsolationLevel.ReadCommitted, true);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        public void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted, false);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            BeginTransaction(IsolationLevel.ReadCommitted, false);
        }

        /// <summary>
        /// Resets the transaction.
        /// </summary>
        public void ResetTransaction()
        {
            TransactionalFlush();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CleanUpResources();
            if (Settings.CommandLoggingEnabled) OnLogEvent("Disposed the Data Manager");
        }

        /// <summary>
        /// Cleans up resources.
        /// </summary>
        public void CleanUpResources()
        {
            if (_primaryConnection != null)
            {
                TransactionalFlush();
                if (_primaryConnection.State != ConnectionState.Closed)
                {
                    _primaryConnection.Close();
                    if (Settings.CommandLoggingEnabled) OnLogEvent("Closed the Connection");
                }
                _primaryConnection = null;
            }
        }

        /// <summary>
        /// Commits the scope transaction.
        /// </summary>
        /// <param name="tx">The tx.</param>
        private void CommitScopeTransaction(GenericTransaction tx)
        {
            if (_currentTransaction == null)
            {
                throw new DataException("You cannot commit a transaction when one has not been started.");
            }

            GenericTransaction stx = (_transactionStack.Count > 0) ? _transactionStack.Peek() : null;
            if (stx != tx)
            {
                _isDoomed = true;
                TransactionalFlush();
                throw new DataException("The current stack transaction does not match the scoped transaction that is being affected.");
            }
            else
            {
                stx = _transactionStack.Pop();
                stx.IsDead = true;
                if (_transactionStack.Count == 0)
                {
                    _currentTransaction.Commit();
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
            if (Settings.CommandLoggingEnabled) OnLogEvent("Committed Scope Transaction");
        }

        /// <summary>
        /// Rollbacks the scope transaction.
        /// </summary>
        /// <param name="tx">The tx.</param>
        private void RollbackScopeTransaction(GenericTransaction tx)
        {
            if (!_isDoomed)
            {
                if (_currentTransaction == null)
                {
                    throw new DataException("You cannot rollback a transaction when one has not been started.");
                }

                GenericTransaction stx = (_transactionStack.Count > 0) ? _transactionStack.Peek() : null;
                if (stx != tx)
                {
                    _isDoomed = true;
                    TransactionalFlush();
                    throw new DataException("The current stack transaction does not match the scoped transaction that is being affected.");
                }
                else
                {
                    _isDoomed = true;
                    TransactionalFlush();
                }
                if (Settings.CommandLoggingEnabled) OnLogEvent("Rolled Back Scope Transaction");
            }
        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="usingScope">if set to <c>true</c> [using scope].</param>
        /// <returns></returns>
        private IGenericTransaction BeginTransaction(IsolationLevel isolationLevel, bool usingScope)
        {
            if (_isDoomed) this.TransactionalFlush();

            if (!IsInActiveTransaction)
            {
                _isDoomed = false;
                _currentTransaction = RunWithRetry(() => { return PrimaryConnection.BeginTransaction(isolationLevel); });
            }

            _transactionStack.Push(new GenericTransaction(CommitScopeTransaction, RollbackScopeTransaction));

            if (Settings.CommandLoggingEnabled) OnLogEvent("Begin Transaction - Level " + _transactionStack.Count);

            return _transactionStack.Peek();
        }

        /// <summary>
        /// Flushes the transaction stack.
        /// </summary>
        private void TransactionalFlush()
        {
            if (IsInActiveTransaction || (_currentTransaction != null))
            {
                try
                {
                    if (_isDoomed) _currentTransaction.Rollback();
                    else _currentTransaction.Commit();

                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
                catch (Exception ex)
                {
                    if (_isDoomed)
                        OnLogError(ex, "Exception occurred when rolling back a doomed transaction during a TransactionalFlush.");
                    else
                        OnLogError(ex, "Exception occurred when committing the current transaction during a TransactionalFlush.");
                }

                while (_transactionStack.Count > 0)
                {
                    _transactionStack.Pop().IsDead = true;
                }

                if (Settings.CommandLoggingEnabled) OnLogEvent("Flush Transaction");
            }
        }

        /// <summary>
        /// Executes a datareader, allowing for failures to be captured and retried!
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(IDbCommand command)
        {
            return ExecuteWithRetry<IDataReader>(command, () => { return command.ExecuteReader(); });
        }

        /// <summary>
        /// Executes into a scalar object, allowing for failures to be captured and retried!
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public object ExecuteScalar(IDbCommand command)
        {
            return ExecuteWithRetry<object>(command, () => { return command.ExecuteScalar(); });
        }

        /// <summary>
        /// Executes non query, allowing for failures to be captured and retried!
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(IDbCommand command)
        {
            return ExecuteWithRetry<int>(command, () => { return command.ExecuteNonQuery(); });
        }

        /// <summary>
        /// Determines whether the specified exception is retryable.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="sourceErrors">The list of source errors that identify this exception as retryable.</param>
        /// <returns></returns>
        protected bool IsRetryable(Exception ex, out Dictionary<int, string> sourceErrors)
        {
            var result = false;
            if (RetryableExceptionPolicy != null)
            {
                result = RetryableExceptionPolicy.IsRetryable(ex, out sourceErrors);
            }
            else
            {
                sourceErrors = new Dictionary<int, string>();
            }
            return result;
        }

        /// <summary>
        /// Runs with retry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="runMe">The run me.</param>
        /// <returns></returns>
        private T RunWithRetry<T>(Func<T> runMe)
        {
            if (this._currentTransaction != null) return runMe();

            int retryCount = NumberConnectionRetrys;
            T result = default(T);
            while (retryCount-- >= 0)
            {
                try
                {
                    result = runMe();
                    break;
                }
                catch (Exception ex)
                {
                    Dictionary<int, string> errors = null;
                    if (IsRetryable(ex, out errors))
                    {
                        if (retryCount < 0) throw ex;
                        else
                        {
                            var msg = new StringBuilder();
                            msg.Append("Retryable error detected. (Retry #").Append(NumberConnectionRetrys - retryCount).Append(")").AppendLine();
                            foreach (var errorNumber in errors.Keys)
                            {
                                msg.Append("Error ").Append(errorNumber).Append(": ").Append(errors[errorNumber]).AppendLine();
                            }
                            if (Settings.CommandLoggingEnabled) OnLogEvent(msg.ToString());
                            Thread.Sleep(RetrySleepMilliseconds);
                            if (_primaryConnection != null)
                                ResetConnection(this._primaryConnection);
                        }
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Executes the with retry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The command execution function.</param>
        /// <returns></returns>
        private T ExecuteWithRetry<T>(IDbCommand command, Func<T> executeCommand)
        {
            if (command.Transaction != null) return executeCommand();

            int retryCount = NumberConnectionRetrys;
            T result = default(T);
            while (retryCount-- >= 0)
            {
                try
                {
                    if (command.Connection.State == ConnectionState.Closed)
                        ResetConnection(command.Connection);

                    result = executeCommand();
                    break;
                }
                catch (Exception ex)
                {
                    Dictionary<int, string> errors = null;
                    if (IsRetryable(ex, out errors))
                    {
                        if (retryCount < 0) throw ex;
                        else
                        {
                            var msg = new StringBuilder();
                            msg.Append("Retryable error detected. (Retry #").Append(NumberConnectionRetrys - retryCount).Append(")").AppendLine();
                            foreach (var errorNumber in errors.Keys)
                            {
                                msg.Append("Error ").Append(errorNumber).Append(": ").Append(errors[errorNumber]).AppendLine();
                            }
                            if (Settings.CommandLoggingEnabled) OnLogEvent(msg.ToString());
                            Thread.Sleep(RetrySleepMilliseconds);
                            ResetConnection(command.Connection);
                        }
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Resets the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private void ResetConnection(IDbConnection connection)
        {
            if (connection != null)
            {
                try { connection.Close(); }
                catch { }
                connection.Open();
            }
        }

        protected void ResetPrimaryConnection()
        {
            ResetConnection(_primaryConnection);
        }
        
    }
}