using System.Data;

namespace Zuris.SPDAL
{
    public class ParameterInfo
    {
        public string Schema { get; set; }

        public string ProcedureName { get; set; }

        public int Ordinal { get; set; }

        public string ParameterName { get; set; }

        public ParameterDirection Direction { get; set; }

        public string NativeDataType { get; set; }

        public int SizeInCharacters { get; set; }

        public int SizeInBytes { get; set; }

        public int Precision { get; set; }

        public int Scale { get; set; }
    }
}