using System.Data;

namespace Zuris.SPDAL
{
    public interface IProcedure
    {
        string Name { get; }

        string CommandText { get; }

        CommandType CommandType { get; }

        DataTable ExecuteIntoDataTable();

        DataSet ExecuteIntoDataSet();

        ST ExecuteScalar<ST>();

        ST ExecuteScalarFromResultSet<ST>();

        int ExecuteNonQuery();
    }

    public interface IProcedure<P> : IProcedure
        where P : new()
    {
        P Parameters { get; }
    }
}