using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Zuris.SPDAL.SqlServer
{
    public class SqlServerMetaDataManager : IMetaDataManager
    {
        private IDataManager _mgr;

        public SqlServerMetaDataManager(IDataManager dataManager)
        {
            _mgr = dataManager;
        }

        public ProcedureInfo GetProcedureInfo(string procedureName)
        {
            return GetProcedureInfo(null, procedureName);
        }

        public ProcedureInfo GetProcedureInfo(string schemaName, string procedureName)
        {
            ProcedureInfo procedure = null;
            GetProcedureInfoWithCallback(schemaName, procedureName, (p) => procedure = p);
            return procedure;
        }

        public List<ProcedureInfo> GetProcedureInfo()
        {
            var procedures = new List<ProcedureInfo>();
            GetProcedureInfoWithCallback(null, null, (p) => procedures.Add(p));
            return procedures;
        }

        public bool ProcedureExists(string procedureName)
        {
            return ProcedureExists(null, procedureName);
        }

        public bool ProcedureExists(string schemaName, string procedureName)
        {
            var sql = new StringBuilder().Append(
@"select count(*)
from INFORMATION_SCHEMA.ROUTINES
");
            if (!string.IsNullOrWhiteSpace(procedureName))
            {
                bool schemaAdded = false;
                if (!string.IsNullOrWhiteSpace(schemaName))
                {
                    schemaAdded = true;
                    sql.Append("where [SPECIFIC_SCHEMA] = @Schema").AppendLine();
                }
                sql.Append(schemaAdded ? "and" : "where").AppendLine(" [SPECIFIC_NAME] = @Procedure").AppendLine();
            }
            return ExecuteIntoInteger(sql.ToString(), schemaName, procedureName) > 0;
        }

        private void GetProcedureInfoWithCallback(string schemaName, string procedureName, Action<ProcedureInfo> procedureRead)
        {
            var sql = new StringBuilder().Append(
@"select
	SPECIFIC_SCHEMA as [Schema],
	SPECIFIC_NAME as [Name],
	ROUTINE_TYPE as [RoutineType],
	SQL_DATA_ACCESS as [DataAccessType]
from INFORMATION_SCHEMA.ROUTINES
");
            if (!string.IsNullOrWhiteSpace(procedureName))
            {
                bool schemaAdded = false;
                if (!string.IsNullOrWhiteSpace(schemaName))
                {
                    schemaAdded = true;
                    sql.Append("where [SPECIFIC_SCHEMA] = @Schema").AppendLine();
                }
                sql.Append(schemaAdded ? "and" : "where").AppendLine(" [SPECIFIC_NAME] = @Procedure").AppendLine();
            }
            sql.Append(
@"order by SPECIFIC_SCHEMA, SPECIFIC_NAME");

            ExecuteWithCallback(sql.ToString(), schemaName, procedureName, procedureRead);
        }

        protected List<ParameterInfo> GetParameterInfoList(string schema, string procedure)
        {
            var parameters = new List<ParameterInfo>();
            var sql = new StringBuilder().Append(
@"select
	SPECIFIC_SCHEMA as [Schema],
	SPECIFIC_NAME as [ProcedureName],
	ORDINAL_POSITION as [Ordinal],
	PARAMETER_NAME as [ParameterName],
	DATA_TYPE as [NativeDataType],
	PARAMETER_MODE as [Direction],
	CHARACTER_MAXIMUM_LENGTH as [SizeInCharacters],
	CHARACTER_OCTET_LENGTH as [SizeInBytes],
	NUMERIC_PRECISION as [Precision],
	NUMERIC_SCALE as [Scale]
from INFORMATION_SCHEMA.PARAMETERS
where IS_RESULT = 'NO'
");
            if (!string.IsNullOrWhiteSpace(procedure))
            {
                if (!string.IsNullOrWhiteSpace(schema))
                {
                    sql.Append("and [SPECIFIC_SCHEMA] = @Schema").AppendLine();
                }
                sql.Append("and [SPECIFIC_NAME] = @Procedure").AppendLine();
            }
            sql.Append(
@"order by SPECIFIC_SCHEMA, SPECIFIC_NAME, ORDINAL_POSITION");
            ExecuteWithCallback(sql.ToString(), schema, procedure, (param) => { parameters.Add(param); });

            return parameters;
        }

        private void PopulateCommandParameters(IDbCommand cmd, string schema, string procedure)
        {
            if (!string.IsNullOrWhiteSpace(procedure))
            {
                IDbDataParameter parameter;
                if (!string.IsNullOrWhiteSpace(schema))
                {
                    parameter = cmd.CreateParameter();
                    parameter.ParameterName = "@Schema";
                    parameter.Size = 512;
                    parameter.DbType = DbType.String;
                    parameter.Value = schema;
                    cmd.Parameters.Add(parameter);
                }
                parameter = cmd.CreateParameter();
                parameter.ParameterName = "@Procedure";
                parameter.Size = 512;
                parameter.DbType = DbType.String;
                parameter.Value = procedure;
                cmd.Parameters.Add(parameter);
            }
        }

        protected virtual int ExecuteIntoInteger(string sql, string schemaName, string procedureName)
        {
            using (var cmd = _mgr.CreateCommand())
            {
                PopulateCommandParameters(cmd, schemaName, procedureName);
                cmd.CommandText = sql;
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        protected virtual void ExecuteWithCallback(string sql, string schemaName, string procedureName, Action<ProcedureInfo> procedureRead)
        {
            using (var cmd = _mgr.CreateCommand())
            {
                PopulateCommandParameters(cmd, schemaName, procedureName);

                cmd.CommandText = sql.ToString();
                var parameters = GetParameterInfoList(schemaName, procedureName);

                int pIdx = 0;
                using (var reader = _mgr.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        var procedureInfo = new ProcedureInfo()
                        {
                            Schema = reader.GetString(reader.GetOrdinal("Schema")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            RoutineType = (RoutineTypeEnum)Enum.Parse(typeof(RoutineTypeEnum), reader.GetString(reader.GetOrdinal("RoutineType")), true),
                            DataAccessType = (RoutineDataAccessTypeEnum)Enum.Parse(typeof(RoutineDataAccessTypeEnum), reader.GetString(reader.GetOrdinal("DataAccessType")), true)
                        };

                        while ((pIdx < parameters.Count) && (parameters[pIdx].ProcedureName == procedureInfo.Name))
                        {
                            procedureInfo.Parameters.Add(parameters[pIdx]);
                            pIdx++;
                        }

                        procedureRead(procedureInfo);
                    }
                }
            }
        }

        protected virtual void ExecuteWithCallback(string sql, string schemaName, string procedureName, Action<ParameterInfo> parameterRead)
        {
            using (var cmd = _mgr.CreateCommand())
            {
                PopulateCommandParameters(cmd, schemaName, procedureName);

                cmd.CommandText = sql.ToString();

                using (var reader = _mgr.ExecuteReader(cmd))
                {
                    while (reader.Read())
                    {
                        var parameter = new ParameterInfo()
                        {
                            Schema = reader.GetString(reader.GetOrdinal("Schema")),
                            ProcedureName = reader.GetString(reader.GetOrdinal("ProcedureName")),
                            Ordinal = Convert.ToInt32(reader["Ordinal"]),
                            ParameterName = reader.GetString(reader.GetOrdinal("ParameterName")),
                            NativeDataType = reader.GetString(reader.GetOrdinal("NativeDataType"))
                        };

                        var mode = reader.GetString(reader.GetOrdinal("Direction"));
                        if (mode == "IN") parameter.Direction = ParameterDirection.Input;
                        else if (mode == "OUT") parameter.Direction = ParameterDirection.Output;
                        else parameter.Direction = ParameterDirection.InputOutput;

                        if (!reader.IsDBNull(reader.GetOrdinal("SizeInCharacters")))
                            parameter.SizeInCharacters = Convert.ToInt32(reader["SizeInCharacters"]);

                        if (!reader.IsDBNull(reader.GetOrdinal("SizeInBytes")))
                            parameter.SizeInBytes = Convert.ToInt32(reader["SizeInBytes"]);

                        if (!reader.IsDBNull(reader.GetOrdinal("Precision")))
                            parameter.Precision = Convert.ToInt32(reader["Precision"]);

                        if (!reader.IsDBNull(reader.GetOrdinal("Scale")))
                            parameter.Scale = Convert.ToInt32(reader["Scale"]);

                        parameterRead(parameter);
                    }
                }
            }
        }
    }
}

/*
 select
	SPECIFIC_SCHEMA,
	SPECIFIC_NAME,
	ROUTINE_TYPE,
	SQL_DATA_ACCESS
from INFORMATION_SCHEMA.ROUTINES
Returns one of the following values:
NONE = Function does not contain SQL.
CONTAINS = Function possibly contains SQL.
READS = Function possibly reads SQL data.
MODIFIES = Function possibly modifies SQL data.
Returns READS for all functions, and MODIFIES for all stored procedures.

select
	SPECIFIC_SCHEMA,
	SPECIFIC_NAME,
	ORDINAL_POSITION,
	IS_RESULT,
	PARAMETER_NAME,
	PARAMETER_MODE,
	DATA_TYPE,
	CHARACTER_MAXIMUM_LENGTH,
	CHARACTER_OCTET_LENGTH,
	NUMERIC_PRECISION,
	NUMERIC_SCALE
from INFORMATION_SCHEMA.PARAMETERS
where IS_RESULT = 'NO'*/