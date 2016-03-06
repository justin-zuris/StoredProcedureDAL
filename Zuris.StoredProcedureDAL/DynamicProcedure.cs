using System.Data;

namespace Zuris.SPDAL
{
    public class DynamicProcedure : Procedure<DynamicParameterList>
    {
        private string _name;
        private string _commandText;
        private CommandType _commandType;

        public DynamicProcedure(ICommandDataProvider cdp, CommandType commandType, string commandText)
            : base(cdp)
        {
            _name = commandText;
            _commandText = commandText;
            _commandType = commandType;
        }

        public override string Name
        {
            get { return CommandText; }
        }

        public override string CommandText
        {
            get { return _commandText; }
        }

        public override CommandType CommandType
        {
            get { return _commandType; }
        }
    }
}