using System.Data;

namespace Zuris.SPDAL
{
    public interface IObjectQueryParam
    {
        string Name { get; set; }

        object ObjectValue { get; set; }

        bool HasValue { get; }

        bool IsActivated { get; }

        ParameterDirection Direction { get; }

        bool IsRequired { get; }

        DbType? DbType { get; }

        int Size { get; }

        int Scale { get; }

        int Precision { get; }
    }
}