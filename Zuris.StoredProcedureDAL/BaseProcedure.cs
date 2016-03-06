using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Zuris.SPDAL
{
    public abstract class BaseProcedure
    {
        private ICommandDataProvider _cdp;

        public BaseProcedure(ICommandDataProvider cdp)
        {
            _cdp = cdp;
        }

        public abstract string CommandText { get; }

        public virtual string Name { get { return CommandText; } }

        public virtual CommandType CommandType { get { return CommandType.StoredProcedure; } }

        protected abstract IParameterGroup QueryParameters { get; }

        public string DatabaseName { get { return ParseEntityIdentifier(Name).Item2; } }

        public string SchemaName { get { return ParseEntityIdentifier(Name).Item3; } }

        public string ProcedureName { get { return ParseEntityIdentifier(Name).Item4; } }

        public ICommandDataProvider CommandDataProvider { get { return _cdp; } }

        protected virtual void AddParameters(IDbCommand command)
        {
        }

        protected virtual void UpdateOutputParameters(IDbCommand command)
        {
        }

        protected virtual Dictionary<string, object> BuildParameterDictionary()
        {
            return null;
        }

        public virtual bool ValidateWithDatabase(bool requireMatchingParameterCount = true)
        {
            return _cdp.ValidateCommand(SchemaName, ProcedureName, QueryParameters, requireMatchingParameterCount);
        }

        public virtual int ExecuteNonQuery()
        {
            return _cdp.ExecuteNonQuery(CommandType.StoredProcedure, Name, QueryParameters);
        }

        public virtual ST ExecuteScalar<ST>()
        {
            ST val = default(ST);
            val = (ST)ConvertToType(typeof(ST), _cdp.ExecuteScalar(CommandType.StoredProcedure, Name, QueryParameters));
            return val;
        }

        public virtual ST ExecuteScalarFromResultSet<ST>()
        {
            ST val = default(ST);
            var dataSet = ExecuteIntoDataSet();
            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0 && dataSet.Tables[0].Columns.Count > 0)
            {
                val = (ST)ConvertToType(typeof(ST), dataSet.Tables[0].Rows[0][0]);
            }
            return val;
        }

        public virtual DataTable ExecuteIntoDataTable()
        {
            var dataSet = ExecuteIntoDataSet();
            return (dataSet.Tables.Count > 0) ? dataSet.Tables[0] : new DataTable();
        }

        public virtual DataSet ExecuteIntoDataSet()
        {
            return _cdp.ExecuteIntoDataSet(CommandType.StoredProcedure, Name, QueryParameters);
        }

        protected virtual void Execute<T>(Func<T, bool> onRecordReadContinue, Action<T, IRecordDataExtractor> bindObject) where T : new()
        {
            _cdp.Execute<T>(CommandType.StoredProcedure, Name, QueryParameters, onRecordReadContinue, bindObject);
        }

        protected virtual void ExecuteMultiRecordSet(Action execute)
        {
            _cdp.ExecuteMultiRecordSet(CommandType.StoredProcedure, Name, QueryParameters, execute);
        }

        protected virtual void ExecuteRecordSetInGroup<T>(Func<T, bool> onRecordReadContinue, Action<T, IRecordDataExtractor> bindObject) where T : new()
        {
            _cdp.ExecuteRecordSetInGroup(onRecordReadContinue, bindObject);
        }

        protected virtual void MoveToNextRecordSetInGroup()
        {
            _cdp.MoveToNextRecordSetInGroup();
        }

        protected static object ConvertToType(Type type, object o)
        {
            object data = type.IsValueType ? Activator.CreateInstance(type) : null;
            if (data != null && !Convert.IsDBNull(data))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = Nullable.GetUnderlyingType(type);
                }

                // add special cases here (int to boolean, etc)

                data = Convert.ChangeType(o, type);
            }

            return data;
        }

        /// <summary>
        /// Parses the procedure identifier into a tuple (Server,Database,Schema,Object)
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A tuple representing Server.Database.Schema.Object</returns>
        public static Tuple<string, string, string, string> ParseEntityIdentifier(string name, bool useDoubleQuotes = false)
        {
            var tkArr = new string[4];
            var tokens = new List<string>();
            if (!string.IsNullOrWhiteSpace(name))
            {
                bool inEscapedIdentifier = false;
                var token = new StringBuilder();
                foreach (var c in name)
                {
                    if (!inEscapedIdentifier)
                    {
                        if (token.Length == 0 && c == (useDoubleQuotes ? '"' : '[')) inEscapedIdentifier = true;
                        else if (c == '.' || c == ' ')
                        {
                            if (token.Length > 0)
                            {
                                tokens.Add(token.ToString());
                                token.Clear();
                            }
                        }
                        else if (!(token.Length == 0 && char.IsWhiteSpace(c)))
                        {
                            token.Append(c);
                        }
                    }
                    else
                    {
                        if (c == (useDoubleQuotes ? '"' : ']')) inEscapedIdentifier = false;
                        else { token.Append(c); }
                    }
                }
                if (token.Length > 0) tokens.Add(token.ToString());

                int j = 3;
                for (int i = tokens.Count - 1; i >= 0; i--)
                {
                    tkArr[j--] = tokens[i];
                }
            }
            return new Tuple<string, string, string, string>(tkArr[0], tkArr[1], tkArr[2], tkArr[3]);
        }
    }
}