namespace Zuris.SPDAL.UnitTests
{
    public class TableAndXmlParamTest : RecordSetProcedure<TableAndXmlParameters, CountryAndXml>
    {
        protected const string PROCEDURE_NAME = "[dbo].[__ProcWithTableAndXmlParams]";

        public TableAndXmlParamTest(SampleCommandDataProvider cdp)
            : base(cdp)
        {
        }

        public override string CommandText { get { return PROCEDURE_NAME; } }

        protected override void BindRecord(CountryAndXml record, IRecordDataExtractor rde)
        {
            record.Code = rde.GetString("Code");
            record.Name = rde.GetString("Name");
            record.MyXml = rde.GetString("MyXml");
        }
    }
}