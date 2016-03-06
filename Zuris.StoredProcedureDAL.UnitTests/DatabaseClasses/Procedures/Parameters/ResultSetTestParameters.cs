using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zuris.SPDAL.SqlServer;

namespace Zuris.SPDAL.UnitTests
{
    public class ResultSetTestParameters : ParameterGroup<IResultSetTestParameters>, IResultSetTestParameters
    {
        private QueryParam<int?> _id = new QueryParam<int?> { Name = "@id", DbType = DbType.Int32 };
        private QueryParam<string> _name = new QueryParam<string> { Name = "@name", DbType = DbType.String };
        private QueryParam<bool?> _enabled = new QueryParam<bool?> { Name = "@enabled", DbType = DbType.Boolean };
        private QueryParam<double?> _cost = new QueryParam<double?> { Name = "@cost", DbType = DbType.Double };

        public IQueryParam<int?> Id { get { return _id; } }
        public IQueryParam<string> Name { get { return _name; } }
        public IQueryParam<bool?> Enabled { get { return _enabled; } }
        public IQueryParam<double?> Cost { get { return _cost; } }

        int? IResultSetTestParameters.Id { get { return Id.Value; } set { Id.Value = value; } }
        string IResultSetTestParameters.Name { get { return Name.Value; } set { Name.Value = value; } }
        bool? IResultSetTestParameters.Enabled { get { return Enabled.Value; } set { Enabled.Value = value; } }
        double? IResultSetTestParameters.Cost { get { return Cost.Value; } set { Cost.Value = value; } }
    }
}
