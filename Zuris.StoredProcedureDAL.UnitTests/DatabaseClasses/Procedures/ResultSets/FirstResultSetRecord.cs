using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zuris.SPDAL.SqlServer;

namespace Zuris.SPDAL.UnitTests
{
    public class FirstResultSetRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public double? Cost { get; set; }
        public Guid UniqueId { get; set; }
        public long Checksum { get; set; }
    }
}
