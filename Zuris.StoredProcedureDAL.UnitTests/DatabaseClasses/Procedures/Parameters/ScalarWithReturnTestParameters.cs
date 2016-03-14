using System.Data;

namespace Zuris.SPDAL.UnitTests
{
    public class ScalarWithReturnTestParameters : BaseParameterGroup
    {
        private QueryParam<int?> _id = new QueryParam<int?> { Name = "@id", DbType = DbType.Int32 };
        private QueryParam<int?> _returnValue = new QueryParam<int?> { Name = "@RETURN_VALUE", DbType = DbType.Int32, Direction = ParameterDirection.ReturnValue };

        public IQueryParam<int?> Id { get { return _id; } }
        public IQueryParam<int?> ReturnValue { get { return _returnValue; } }
    }
}