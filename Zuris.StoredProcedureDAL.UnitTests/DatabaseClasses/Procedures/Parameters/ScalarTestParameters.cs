using System.Data;

namespace Zuris.SPDAL.UnitTests
{
    public class ScalarTestParameters : BaseParameterGroup
    {
        private QueryParam<int?> _id = new QueryParam<int?> { Name = "@id", DbType = DbType.Int32 };
        
        public IQueryParam<int?> Id { get { return _id; } }
    }
}