using System;
using System.Data;

namespace Zuris.SPDAL
{
    public interface ICommandDataProvider : IRecordDataExtractor
    {
        bool ValidateCommand(string schemaName, string procedureName, IParameterGroup parameters, bool requireMatchingParameterCount = true);

        int ExecuteNonQuery(CommandType commandType, string commandText, IParameterGroup parameters);

        object ExecuteScalar(CommandType commandType, string commandText, IParameterGroup parameters);

        DataSet ExecuteIntoDataSet(CommandType commandType, string commandText, IParameterGroup parameters);

        void Execute<T>(
            CommandType commandType,
            string commandText,
            IParameterGroup parameters,
            Func<T, bool> onRecordReadContinue,
            Action<T, IRecordDataExtractor> bindObject) where T : new();

        void ExecuteMultiRecordSet(
            CommandType commandType,
            string commandText,
            IParameterGroup parameters,
            Action executeRecordSets);

        void ExecuteRecordSetInGroup<T>(
            Func<T, bool> onRecordReadContinue,
            Action<T, IRecordDataExtractor> bindObject) where T : new();

        bool MoveToNextRecordSetInGroup();
    }
}