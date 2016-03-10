using System.Data;

namespace Zuris.SPDAL.UnitTests
{
    public class TableAndXmlParameters : BaseParameterGroup
    {
        private QueryParam<DataTable> _countries = new QueryParam<DataTable> { Name = "@countries", DbType = DbType.Object };
        private QueryParam<string> _xml = new QueryParam<string> { Name = "@xml", DbType = DbType.Xml };
        
        public IQueryParam<DataTable> Countries { get { return _countries; } }
        public IQueryParam<string> Xml { get { return _xml; } }
    }
}