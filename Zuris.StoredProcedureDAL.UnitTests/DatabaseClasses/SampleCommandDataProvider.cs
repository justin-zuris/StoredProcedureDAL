namespace Zuris.SPDAL.UnitTests
{
    public class SampleCommandDataProvider : DbCommandDataProvider
    {
        public SampleCommandDataProvider(SampleDataManager dataManager)
            : base(dataManager)
        {
        }
    }
}