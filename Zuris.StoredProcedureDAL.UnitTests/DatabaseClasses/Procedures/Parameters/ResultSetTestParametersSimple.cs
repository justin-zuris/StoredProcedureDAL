using System.Data;

namespace Zuris.SPDAL.UnitTests
{
    public class ResultSetTestParametersSimple : BaseParameterGroup
    {
        private QueryParam<int?> _id = new QueryParam<int?> { Name = "@id", DbType = DbType.Int32 };
        private QueryParam<string> _name = new QueryParam<string> { Name = "@name", DbType = DbType.String };
        private QueryParam<bool?> _enabled = new QueryParam<bool?> { Name = "@enabled", DbType = DbType.Boolean };
        private QueryParam<double?> _cost = new QueryParam<double?> { Name = "@cost", DbType = DbType.Double };

        public IQueryParam<int?> Id { get { return _id; } }
        public IQueryParam<string> Name { get { return _name; } }
        public IQueryParam<bool?> Enabled { get { return _enabled; } }
        public IQueryParam<double?> Cost { get { return _cost; } }
    }
}