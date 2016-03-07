namespace Zuris.SPDAL.UnitTests
{
    public class MultiResultSetTest : RecordSetProcedure<ResultSetTestParameters, FirstResultSetRecord, SecondResultSetRecord>
    {
        protected const string PROCEDURE_NAME = "[dbo].[__MultiResultSetTest]";

        public MultiResultSetTest(SampleCommandDataProvider cdp)
            : base(cdp)
        {
        }

        public override string CommandText { get { return PROCEDURE_NAME; } }

        protected override void BindRecord(FirstResultSetRecord record, IRecordDataExtractor rde)
        {
            record.Id = rde.GetInt32("Id");
            record.Name = rde.GetString("Name");
            record.Enabled = rde.GetBoolean("Enabled");
            record.Cost = rde.GetDoubleNullable("Cost");
            record.UniqueId = rde.GetGuid("UniqueId");
            record.Checksum = rde.GetInt32("Checksum");
        }

        protected override void BindRecord(SecondResultSetRecord record, IRecordDataExtractor rde)
        {
            record.Id = rde.GetInt32("IdColumn");
            record.Amount = rde.GetDecimal("AmountColumn");
            record.TheGuid = rde.GetGuid("GuidColumn");
            record.TheDateTime = rde.GetDateTime("DateTimeColumn");
        }
    }
}