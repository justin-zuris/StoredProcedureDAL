using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zuris.SPDAL.SqlServer;

namespace Zuris.SPDAL.UnitTests
{
    public class SecondResultSetRecord
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public Guid TheGuid { get; set; }
        public DateTime TheDateTime { get; set; }
    }
}
