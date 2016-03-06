using System;

namespace Zuris.SPDAL
{
    /// <summary>
    /// A generic abstraction of a database transaction.
    /// </summary>
    public class GenericTransaction : IGenericTransaction
    {
        private readonly Action<GenericTransaction> _commitAction;
        private readonly Action<GenericTransaction> _rollbackAction;
        private bool _isDead;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericTransaction"/> class.
        /// </summary>
        public GenericTransaction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericTransaction"/> class.
        /// </summary>
        /// <param name="commitAction">The commit action.</param>
        /// <param name="rollbackAction">The rollback action.</param>
        public GenericTransaction(Action<GenericTransaction> commitAction, Action<GenericTransaction> rollbackAction)
        {
            _commitAction = commitAction;
            _rollbackAction = rollbackAction;
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit()
        {
            if (!_isDead)
            {
                if (_commitAction != null) _commitAction(this);
                _isDead = true;
            }
        }

        /// <summary>
        /// Rolls back the transaction.
        /// </summary>
        public void Rollback()
        {
            if (!_isDead)
            {
                if (_rollbackAction != null) _rollbackAction(this);
                _isDead = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dead.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dead; otherwise, <c>false</c>.
        /// </value>
        internal bool IsDead
        {
            get;
            set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!_isDead) Rollback();
        }
    }
}