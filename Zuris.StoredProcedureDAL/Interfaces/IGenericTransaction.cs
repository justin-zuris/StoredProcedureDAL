using System;

namespace Zuris.SPDAL
{
    /// <summary>
    /// A generic abstraction of a database transaction.
    /// </summary>
    public interface IGenericTransaction : IDisposable
    {
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rolls back the transaction.
        /// </summary>
        void Rollback();
    }
}