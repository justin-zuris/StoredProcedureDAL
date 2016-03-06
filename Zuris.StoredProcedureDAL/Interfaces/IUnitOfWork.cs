using System;
using System.Data;

namespace Zuris.SPDAL
{
    /// <summary>
    /// The DataManager is the core object for managing database interactivity. It contains the active
    /// connection for the current execution thread, facilitates transaction management, and provides
    /// access to the entity manager.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rollbacks the current transaction.
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Gets a value indicating whether this instance is in an active transaction.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in active transaction; otherwise, <c>false</c>.
        /// </value>
        bool IsInActiveTransaction { get; }

        /// <summary>
        /// Begins a scope based transaction (that will dispose itself, including rollback if not committed, at the end of a using statement).
        /// </summary>
        /// <param name="isolationLevel">The isolation level of the transaction.</param>
        /// <returns>A generic transaction object for this new transaction.</returns>
        IGenericTransaction BeginScopeTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Begins a scope based transaction (that will dispose itself, including rollback if not committed, at the end of a using statement).
        /// </summary>
        /// <returns>A generic transaction object for this new transaction.</returns>
        IGenericTransaction BeginScopeTransaction();

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level for this new transaction.</param>
        void BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Resets the current transaction (disposes of the transaction and rolls back if needed).
        /// </summary>
        void ResetTransaction();
    }
}