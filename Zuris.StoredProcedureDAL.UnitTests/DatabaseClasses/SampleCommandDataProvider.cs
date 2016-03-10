using Zuris.SPDAL.SqlServer;

namespace Zuris.SPDAL.UnitTests
{
    public class SampleCommandDataProvider : SqlServerCommandDataProvider
    {
        public SampleCommandDataProvider(SampleDataManager dataManager)
            : base(dataManager)
        {
        }
    }
}