using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuris.SPDAL.SqlServer
{
    public class SqlServerCommandDataProvider : DbCommandDataProvider
    {
        public SqlServerCommandDataProvider(IDataManager dataManager) : base(dataManager) { }

        protected override void CustomizeDatabaseSpecificParameter(IDbCommand command, IObjectQueryParam queryParam, IDbDataParameter p)
        {
            var sqlParam = p as SqlParameter;
            if (queryParam.HasValue && queryParam.ObjectValue is DataTable) sqlParam.SqlDbType = SqlDbType.Structured;
        }
    }
}
