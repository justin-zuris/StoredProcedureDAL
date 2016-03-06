using System;

namespace Zuris.SPDAL
{
    public interface IRecordDataExtractor
    {
        string GetString(string name);

        DateTime GetDateTime(string name);

        DateTime? GetDateTimeNullable(string name);

        bool GetBoolean(string name);

        bool? GetBooleanNullable(string name);

        int GetInt32(string name);

        int? GetInt32Nullable(string name);

        long GetInt64(string name);

        long? GetInt64Nullable(string name);

        byte GetByte(string name);

        byte? GetByteNullable(string name);

        short GetInt16(string name);

        short? GetInt16Nullable(string name);

        decimal GetDecimal(string name);

        decimal? GetDecimalNullable(string name);

        Guid GetGuid(string name);

        Guid? GetGuidNullable(string name);

        double GetDouble(string name);

        double? GetDoubleNullable(string name);
    }
}