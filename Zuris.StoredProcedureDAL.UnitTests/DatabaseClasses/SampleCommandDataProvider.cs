using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
