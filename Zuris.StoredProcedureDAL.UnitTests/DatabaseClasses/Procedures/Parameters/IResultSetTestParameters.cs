using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zuris.SPDAL.SqlServer;

namespace Zuris.SPDAL.UnitTests
{
    public interface IResultSetTestParameters
    {
        int? Id { get; set; }
        string Name { get; set; }
        bool? Enabled { get; set; }
        double? Cost { get; set; }
    }
}
