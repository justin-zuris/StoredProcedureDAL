using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Zuris.SPDAL.SqlServer
{

    public class SqlServerCommandLogHelper: IDbCommandLogHelper
    {
        private static SqlServerCommandLogHelper _instance;
        public static IDbCommandLogHelper Instance { get { _instance = _instance ?? new SqlServerCommandLogHelper(); return _instance; } }

        public string GetSqlWithParameterDeclaration(IDbCommand command)
        {
            return GetSqlWithParameterDeclaration(command.CommandType, command.CommandText, command.Parameters);
        }

        /// <summary>
        /// Gets the SQL with parameter declaration.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public string GetSqlWithParameterDeclaration(CommandType commandType, string sql, IEnumerable parameters)
        {
            var scb = new SqlCommandBuilder();
            var sb = new StringBuilder();
            var parameterArguments = new StringBuilder();
            if (parameters != null)
            {
                foreach (IDbDataParameter dbp in parameters)
                {
                    if (dbp is SqlParameter)
                    {
                        var sqlp = dbp as SqlParameter;
                        if (parameterArguments.Length > 0) parameterArguments.Append(", ");
                        parameterArguments.Append(sqlp.ParameterName).Append(" = ").Append(sqlp.ParameterName);

                        sb.Append("DECLARE ").Append(sqlp.ParameterName).Append(" ").Append(GetSqlDataType(sqlp.SqlDbType, sqlp.Size, sqlp.Scale, sqlp.Precision)).Append(" = ");
                        if (sqlp.Value is String)
                        {
                            sb.Append("'")
                                .Append(new string((sqlp.Value as String).Where(c => (!char.IsControl(c) || c == '\r' || c == '\f' || c == '\n' || c == '\t')).ToArray()).Replace("'", "''"))
                                .Append("'");
                        }
                        else if (sqlp.Value is DateTime)
                        {
                            sb.Append("{ts '").Append(((DateTime)sqlp.Value).ToString("yyyy-MM-dd HH:mm:ss")).Append("'}");
                        }
                        else if (sqlp.Value is Boolean)
                        {
                            sb.Append(((Boolean)sqlp.Value) ? "1" : "0");
                        }
                        else if (sqlp.Value == null || Convert.IsDBNull(sqlp.Value))
                        {
                            sb.Append("null");
                        }
                        else
                        {
                            sb.Append(sqlp.Value.ToString());
                        }
                        sb.AppendLine(";");
                    }
                }
            }
            if (commandType == CommandType.StoredProcedure)
            {
                sb.Append("exec ").Append(sql).Append(" ").Append(parameterArguments);
            }
            else if (commandType == CommandType.TableDirect)
                sb.Append("select * from ").Append(sql);
            else
                sb.Append(sql);

            return sb.ToString().Trim();
        }

        public string GetSqlDataType(SqlDbType sqlDbType, int size, byte scale, byte precision)
        {
            var sb = new StringBuilder();
            sb.Append(sqlDbType.ToString().ToLower());
            switch (sqlDbType)
            {
                case SqlDbType.Decimal:
                    sb.Append("(").Append(scale).Append(",").Append(precision).Append(")");
                    break;

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.Binary:
                case SqlDbType.VarBinary:
                    sb.Append("(").Append(size <= 0 ? "max" : size.ToString()).Append(")");
                    break;
            }

            return sb.ToString();
        }
    }
}