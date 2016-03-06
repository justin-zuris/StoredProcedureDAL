using System.Data;

namespace Zuris.SPDAL
{
    /// <summary>
    /// The DataManager is the core object for managing database interactivity. It contains the active
    /// connection for the current execution thread, facilitates transaction management, and provides
    /// access to the entity manager.
    /// </summary>
    public interface IDataManager : IUnitOfWork
    {
        /// <summary>
        /// Gets the primary connection for the current thread.
        /// </summary>
        IDbConnection PrimaryConnection { get; }

        /// <summary>
        /// Creates an connection that will not be managed by the DataManager (for transactions, entities, etc).
        /// </summary>
        /// <returns>A connection to the database.</returns>
        IDbConnection CreateUnmanagedConnection();

        /// <summary>
        /// Creates a database command using the current connection.
        /// </summary>
        /// <returns>A new Command object.</returns>
        IDbCommand CreateCommand();

        /// <summary>
        /// Creates a database command using the current connection.
        /// </summary>
        /// <returns>A new Command object.</returns>
        IDbDataAdapter CreateDataAdapter();

        /// <summary>
        /// Enrolls the specified commandinto the current transaction.
        /// </summary>
        /// <param name="command">The command to enroll.</param>
        void Enroll(IDbCommand command);

        /// <summary>
        /// Executes a datareader, allowing for failures to be captured and retried!
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        IDataReader ExecuteReader(IDbCommand command);

        /// <summary>
        /// Executes into a scalar object, allowing for failures to be captured and retried!
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        object ExecuteScalar(IDbCommand command);

        /// <summary>
        /// Executes non query, allowing for failures to be captured and retried!
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        int ExecuteNonQuery(IDbCommand command);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IMetaDataManager CreateMetaDataManager();

        /// <summary>
        ///
        /// </summary>
        IDbCommandLogHelper CommandLogHelper { get; }

        /// <summary>
        /// Cleans up resources.
        /// </summary>
        void CleanUpResources();
    }
}