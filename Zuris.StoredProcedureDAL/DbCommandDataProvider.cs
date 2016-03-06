using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Zuris.SPDAL
{
    public class DbCommandDataProvider : ICommandDataProvider
    {
        public static event DataAccessEventOccurred LogEvent;

        protected IDataManager _dataManager;
        protected IDataReader _currentReader;

        public DbCommandDataProvider(IDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public void ExecuteMultiRecordSet(
            CommandType commandType,
            string commandText,
            IParameterGroup parameters,
            Action executeRecordSets)
        {
            using (var command = _dataManager.CreateCommand())
            {
                command.CommandType = commandType;
                command.CommandText = commandText;

                AddParameters(command, parameters);
                if (CommandLoggingEnabled)
                {
                    OnLogEvent("ExecuteMultiRecordSet", _dataManager.CommandLogHelper.GetSqlWithParameterDeclaration(command));
                }

                try
                {
                    using (_currentReader = command.ExecuteReader())
                    {
                        executeRecordSets();
                    }
                }
                finally
                {
                    _currentReader = null;
                }

                UpdateOutputParameters(command, parameters);
            }
        }

        public void ExecuteRecordSetInGroup<T>(
            Func<T, bool> onRecordReadContinue,
            Action<T, IRecordDataExtractor> bindObject) where T : new()
        {
            if (_currentReader != null)
            {
                while (_currentReader.Read())
                {
                    var record = new T();
                    bindObject(record, this);
                    if (!onRecordReadContinue(record)) break;
                }
            }
        }

        public bool MoveToNextRecordSetInGroup()
        {
            if (_currentReader != null)
            {
                return _currentReader.NextResult();
            }
            else return false;
        }

        public void Execute<T>(
            CommandType commandType,
            string commandText,
            IParameterGroup parameters,
            Func<T, bool> onRecordReadContinue,
            Action<T, IRecordDataExtractor> bindObject) where T : new()
        {
            using (var command = _dataManager.CreateCommand())
            {
                command.CommandType = commandType;
                command.CommandText = commandText;

                AddParameters(command, parameters);
                if (CommandLoggingEnabled)
                {
                    OnLogEvent("Execute", _dataManager.CommandLogHelper.GetSqlWithParameterDeclaration(command));
                }

                using (_currentReader = _dataManager.ExecuteReader(command))
                {
                    while (_currentReader.Read())
                    {
                        var record = new T();
                        bindObject(record, this);
                        if (!onRecordReadContinue(record)) break;
                    }
                }
                _currentReader = null;

                UpdateOutputParameters(command, parameters);
            }
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText, IParameterGroup parameters)
        {
            int val;
            using (var command = _dataManager.CreateCommand())
            {
                command.CommandType = commandType;
                command.CommandText = commandText;

                AddParameters(command, parameters);
                if (CommandLoggingEnabled)
                {
                    OnLogEvent("ExecuteNonQuery", _dataManager.CommandLogHelper.GetSqlWithParameterDeclaration(command));
                }

                val = command.ExecuteNonQuery();

                UpdateOutputParameters(command, parameters);
            }
            return val;
        }

        public object ExecuteScalar(CommandType commandType, string commandText, IParameterGroup parameters)
        {
            object val;
            using (var command = _dataManager.CreateCommand())
            {
                command.CommandType = commandType;
                command.CommandText = commandText;

                AddParameters(command, parameters);
                if (CommandLoggingEnabled)
                {
                    OnLogEvent("ExecuteScalar", _dataManager.CommandLogHelper.GetSqlWithParameterDeclaration(command));
                }

                val = command.ExecuteScalar();

                UpdateOutputParameters(command, parameters);
            }
            return val;
        }

        public DataSet ExecuteIntoDataSet(CommandType commandType, string commandText, IParameterGroup parameters)
        {
            var dataSet = new DataSet();

            using (var command = _dataManager.CreateCommand())
            {
                command.CommandType = commandType;
                command.CommandText = commandText;

                AddParameters(command, parameters);
                if (CommandLoggingEnabled)
                {
                    OnLogEvent("ExecuteNonQuery", _dataManager.CommandLogHelper.GetSqlWithParameterDeclaration(command));
                }

                IDbDataAdapter dataAdapter = _dataManager.CreateDataAdapter();
                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(dataSet);

                UpdateOutputParameters(command, parameters);
            }
            return dataSet;
        }

        protected bool CommandLoggingEnabled { get { return LogEvent != null; } }

        protected void OnLogEvent(string message, string command = null)
        {
            if (LogEvent != null) LogEvent(message, command);
            else System.Diagnostics.Debug.WriteLine(message + Environment.NewLine + command);
        }

        protected virtual void AddParameters(IDbCommand command, IParameterGroup parameters)
        {
            if (parameters != null)
            {
                foreach (var queryParamPInfo in parameters.QueryParameters) AddParameter(command, queryParamPInfo);
            }
        }

        protected virtual IDbDataParameter AddParameter(IDbCommand command, IObjectQueryParam param)
        {
            IDbDataParameter p = null;
            if (param.IsActivated || param.Direction == ParameterDirection.Output)
            {
                if (!param.IsActivated && param.Direction == ParameterDirection.Output)
                    param.ObjectValue = null;

                object value = param.ObjectValue ?? DBNull.Value;
                p = command.CreateParameter();
                p.Direction = param.Direction;
                if (param.DbType.HasValue) p.DbType = param.DbType.Value;
                p.ParameterName = param.Name;
                p.Value = value;
                if (param.Size > 0) p.Size = param.Size;
                if (param.Precision > 0) p.Precision = Convert.ToByte(param.Precision);
                if (param.Scale > 0) p.Scale = Convert.ToByte(param.Scale);
                command.Parameters.Add(p);
            }
            return p;
        }

        protected virtual void UpdateOutputParameters(IDbCommand command, IParameterGroup parameters)
        {
            if (parameters != null)
            {
                var outParams = parameters.OutputQueryParameters;
                foreach (IDbDataParameter dbParam in command.Parameters)
                {
                    var queryParam = outParams.Where(qp => dbParam.ParameterName == qp.Name).FirstOrDefault();
                    if (queryParam != null)
                        queryParam.ObjectValue = Convert.IsDBNull(dbParam.Value) ? null : dbParam.Value;
                }
            }
        }

        public bool ValidateCommand(string schemaName, string procedureName, IParameterGroup parameters, bool requireMatchingParameterCount = true)
        {
            var isValid = true;
            var metaData = _dataManager.CreateMetaDataManager();
            var procInfo = metaData.GetProcedureInfo(schemaName, procedureName);
            if (procInfo == null)
            {
                isValid = false;
                throw new ZurisException<ZurisFrameworkErrorCode>(ZurisFrameworkErrorCode.ProcedureDoesNotExist, "A procedure with the name " + procedureName + " does not exist in the database.", procInfo);
            }
            else if (parameters != null)
            {
                var commandParameters = parameters.QueryParameters;
                var missingParameters = new List<string>();
                foreach (var cp in commandParameters)
                {
                    var exists = procInfo.Parameters.Any(pip => pip.ParameterName == cp.Name);
                    if (!exists) missingParameters.Add(cp.Name);
                }

                if (missingParameters.Count > 0)
                {
                    isValid = false;
                    throw new ZurisException<ZurisFrameworkErrorCode>(ZurisFrameworkErrorCode.ProcedureParameterMismatch, "A procedure with the name " + procedureName + " does not have these parameters: " + string.Join(", ", missingParameters), procInfo);
                }

                if (requireMatchingParameterCount && (procInfo.Parameters.Count != commandParameters.Count))
                {
                    isValid = false;
                    throw new ZurisException<ZurisFrameworkErrorCode>(ZurisFrameworkErrorCode.ProcedureParameterMismatch, "A procedure with the name " + procedureName + " does not have the same number of parameters as the database.");
                }
            }
            return isValid;
        }

        #region Strongly typed _currentReader accessors

        public string GetString(string name)
        {
            var i = _currentReader.GetOrdinal(name);
            return _currentReader.IsDBNull(i) ? null : _currentReader.GetString(i);
        }

        public DateTime GetDateTime(string name)
        {
            var i = _currentReader.GetOrdinal(name);
            return _currentReader.IsDBNull(i) ? default(DateTime) : _currentReader.GetDateTime(i);
        }

        public DateTime? GetDateTimeNullable(string name)
        {
            var i = _currentReader.GetOrdinal(name);
            return _currentReader.IsDBNull(i) ? (DateTime?)null : _currentReader.GetDateTime(i);
        }

        public bool GetBoolean(string name)
        {
            var i = _currentReader.GetOrdinal(name);
            if (_currentReader.IsDBNull(i)) 
                return default(bool);
            else
            {
                object o = _currentReader[i];
                return (o is bool && (bool)o == true) || (!(o is bool) && o.ToString() != "0");
            }
        }

        public bool? GetBooleanNullable(string name)
        {
            var i = _currentReader.GetOrdinal(name);
            if (_currentReader.IsDBNull(i)) return (bool?)null;
            else
            {
                object o = _currentReader[i];
                if (o is bool) return (bool)o;
                else return (o.ToString() != "0");
            }
        }

        public int GetInt32(string name)
        {
            var i = _currentReader.GetOrdinal(name);
            if (_currentReader.IsDBNull(i))
                return default(int);
            else if (_currentReader.GetFieldType(i) == typeof(Boolean))
                return _currentReader.GetBoolean(i) ? 1 : 0;
            else
                return _currentReader.GetInt32(i);
        }

        public int? GetInt32Nullable(string name)
        {
            return _currentReader.IsDBNull(_currentReader.GetOrdinal(name)) ? (int?)null : GetInt32(name);
        }

        public long GetInt64(string name)
        {
            var i = _currentReader.GetOrdinal(name);
            if (_currentReader.IsDBNull(i))
                return default(long);
            else if (_currentReader.GetFieldType(i) == typeof(Boolean))
                return _currentReader.GetBoolean(i) ? 1 : 0;
            else
                return _currentReader.GetInt64(i);
        }

        public long? GetInt64Nullable(string name)
        {
            return _currentReader.IsDBNull(_currentReader.GetOrdinal(name)) ? (long?)null : GetInt64(name);
        }

        public byte GetByte(string name)
        {
            var i = _currentReader.GetOrdinal(name);
            if (_currentReader.IsDBNull(i))
                return default(byte);
            else if (_currentReader.GetFieldType(i) == typeof(Boolean))
                return _currentReader.GetBoolean(i) ? (byte)1 : (byte)0;
            else
                return _currentReader.GetByte(i);
        }

        public byte? GetByteNullable(string name)
        {
            return _currentReader.IsDBNull(_currentReader.GetOrdinal(name)) ? (byte?)null : GetByte(name);
        }

        public short GetInt16(string name)
        {
            var i = _currentReader.GetOrdinal(name);
            if (_currentReader.IsDBNull(i))
                return default(int);
            else if (_currentReader.GetFieldType(i) == typeof(Boolean))
                return _currentReader.GetBoolean(i) ? (short)1 : (short)0;
            else
                return _currentReader.GetInt16(i);
        }

        public short? GetInt16Nullable(string name)
        {
            return _currentReader.IsDBNull(_currentReader.GetOrdinal(name)) ? (short?)null : GetInt16(name);
        }

        public decimal GetDecimal(string name)
        {
            var i = _currentReader.GetOrdinal(name);
            return _currentReader.IsDBNull(i) ? default(decimal) : _currentReader.GetDecimal(i);
        }

        public decimal? GetDecimalNullable(string name)
        {
            return _currentReader.IsDBNull(_currentReader.GetOrdinal(name)) ? (decimal?)null : GetDecimal(name);
        }

        #endregion Strongly typed _currentReader accessors
    }
}