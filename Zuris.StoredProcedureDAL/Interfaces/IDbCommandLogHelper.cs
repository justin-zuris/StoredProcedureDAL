using System;
using System.Collections;
using System.Data;

namespace Zuris.SPDAL
{
    public interface IDbCommandLogHelper
    {
        string GetSqlWithParameterDeclaration(IDbCommand command);
        string GetSqlWithParameterDeclaration(CommandType commandType, string sql, IEnumerable parameters);
        string GetSqlDataType(SqlDbType sqlDbType, int size, byte scale, byte precision);
    }
}