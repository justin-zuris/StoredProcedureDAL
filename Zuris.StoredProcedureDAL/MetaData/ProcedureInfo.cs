using System.Collections.Generic;

namespace Zuris.SPDAL
{
    public enum RoutineTypeEnum
    {
        Procedure = 0,
        Function
    }

    public enum RoutineDataAccessTypeEnum
    {
        None = 0,
        Contains,
        Reads,
        Modifies
    }

    public class ProcedureInfo
    {
        public ProcedureInfo()
        {
            Parameters = new List<ParameterInfo>();
        }

        public string Schema { get; set; }

        public string Name { get; set; }

        public RoutineTypeEnum RoutineType { get; set; }

        public RoutineDataAccessTypeEnum DataAccessType { get; set; }

        public List<ParameterInfo> Parameters { get; set; }
    }
}