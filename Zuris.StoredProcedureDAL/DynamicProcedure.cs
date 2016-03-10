using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

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
        
        public T ExecuteIntoRecord<T>() where T : new()
        {
            T o = default(T);
            Execute<T>((record) => { o = record; return false; }, (record, rde) => { AutoBindRecord(record, rde); });
            return o;
        }

        public List<T> ExecuteIntoList<T>() where T : new()
        {
            var l = new List<T>();
            Execute<T>((record) => { l.Add(record); return true; }, (record, rde) => { AutoBindRecord(record, rde); });
            return l;
        }

        protected virtual void AutoBindRecord<T>(T record, IRecordDataExtractor rde)
        {
            var type = typeof(T);
            foreach (var prop in type.GetProperties().Where(p => p.CanWrite && p.CanRead))
            {
                var objValue = ConvertToType(prop.PropertyType, rde.GetObject(prop.Name));
                prop.SetValue(record, objValue, new object[] { });
            }
        }
    }
}