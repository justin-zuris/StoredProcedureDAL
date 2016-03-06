using System;

namespace Zuris.SPDAL.SqlServer
{
    public abstract class SqlServerDataManager : DataManager
    {
        private Lazy<SqlServerRetryManager> _retryManager = new Lazy<SqlServerRetryManager>();

        public override IMetaDataManager CreateMetaDataManager()
        {
            return new SqlServerMetaDataManager(this);
        }

        public override IDbCommandLogHelper CommandLogHelper
        {
            get { return SqlServerCommandLogHelper.Instance; }
        }

        protected override IEvaluateRetryable RetryableExceptionPolicy
        {
            get { return _retryManager.Value; }
        }
    }
}