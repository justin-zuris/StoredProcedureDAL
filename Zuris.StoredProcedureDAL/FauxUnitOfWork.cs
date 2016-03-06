using System.Data;

namespace Zuris.SPDAL
{
    public class FauxUnitOfWork : IUnitOfWork
    {
        private int _transactionCount = 0;

        private bool InTransaction { get { return _transactionCount == 0; } }

        public void CommitTransaction()
        {
            if (_transactionCount > 0) _transactionCount--;
        }

        public void RollbackTransaction()
        {
            if (_transactionCount > 0) _transactionCount--;
        }

        public void BeginTransaction()
        {
            _transactionCount++;
        }

        public void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            _transactionCount++;
        }

        public void ResetTransaction()
        {
            _transactionCount = 0;
        }

        public bool IsInActiveTransaction { get { return InTransaction; } }

        public IGenericTransaction BeginScopeTransaction(IsolationLevel isolationLevel)
        {
            _transactionCount++;
            return new GenericTransaction((tx) => { CommitTransaction(); }, (tx) => { RollbackTransaction(); });
        }

        public IGenericTransaction BeginScopeTransaction()
        {
            _transactionCount++;
            return BeginScopeTransaction(IsolationLevel.ReadCommitted);
        }

        public void Dispose()
        {
        }
    }
}