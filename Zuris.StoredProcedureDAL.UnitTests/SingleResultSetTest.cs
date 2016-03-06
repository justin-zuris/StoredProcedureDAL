using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuris.SPDAL.UnitTests
{
    public class SingleResultSetTest : RecordSetProcedure<ResultSetTestParameters, FirstResultSetRecord>
    {
        protected const string PROCEDURE_NAME = "[dbo].[__SingleResultSetTest]";

        public SingleResultSetTest(SampleCommandDataProvider cdp)
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
    }
}