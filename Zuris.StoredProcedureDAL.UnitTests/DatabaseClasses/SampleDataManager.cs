using Zuris.SPDAL.SqlServer;

namespace Zuris.SPDAL.UnitTests
{
    public class SampleDataManager : SqlServerDataManager
    {
        private string _cs;

        public SampleDataManager(string connectionString)
        {
            _cs = connectionString;
        }

        protected override string ConnectionString
        {
            get { return _cs; }
        }

        protected override string ProviderString
        {
            get { return "System.Data.SqlClient"; }
        }
    }
}