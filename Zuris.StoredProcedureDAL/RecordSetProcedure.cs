using System.Collections.Generic;

namespace Zuris.SPDAL
{
    public abstract class RecordSetProcedure<P, T> : Procedure<P>, IRecordSetProcedure<P, T>
        where P : new()
        where T : new()
    {
        public RecordSetProcedure(ICommandDataProvider cdp)
            : base(cdp)
        {
        }

        public T ExecuteIntoRecord()
        {
            T o = default(T);
            Execute<T>((record) => { o = record; return false; }, (record, rde) => { BindRecord(record, rde); });
            return o;
        }

        public List<T> ExecuteIntoList()
        {
            var l = new List<T>();
            Execute<T>((record) => { l.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
            return l;
        }

        protected abstract void BindRecord(T record, IRecordDataExtractor rde);
    }
}