using System.Collections.Generic;

namespace Zuris.SPDAL
{
    public abstract class RecordSetProcedure<P, T1, T2> : Procedure<P>, IRecordSetProcedure<P, T1, T2>
        where P : new()
        where T1 : new()
        where T2 : new()
    {
        public RecordSetProcedure(ICommandDataProvider cdp)
            : base(cdp)
        {
        }

        public void ExecuteIntoLists(out List<T1> list1, out List<T2> list2)
        {
            var l1 = new List<T1>();
            var l2 = new List<T2>();

            ExecuteMultiRecordSet(() =>
            {
                ExecuteRecordSetInGroup<T1>((record) => { l1.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T2>((record) => { l2.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
            });

            list1 = l1;
            list2 = l2;
        }

        protected abstract void BindRecord(T1 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T2 record, IRecordDataExtractor rde);
    }

    public abstract class RecordSetProcedure<P, T1, T2, T3> : Procedure<P>, IRecordSetProcedure<P, T1, T2, T3>
        where P : new()
        where T1 : new()
        where T2 : new()
        where T3 : new()
    {
        public RecordSetProcedure(ICommandDataProvider cdp)
            : base(cdp)
        {
        }

        public void ExecuteIntoLists(out List<T1> list1, out List<T2> list2, out List<T3> list3)
        {
            var l1 = new List<T1>();
            var l2 = new List<T2>();
            var l3 = new List<T3>();

            ExecuteMultiRecordSet(() =>
            {
                ExecuteRecordSetInGroup<T1>((record) => { l1.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T2>((record) => { l2.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T3>((record) => { l3.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
            });

            list1 = l1;
            list2 = l2;
            list3 = l3;
        }

        protected abstract void BindRecord(T1 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T2 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T3 record, IRecordDataExtractor rde);
    }

    public abstract class RecordSetProcedure<P, T1, T2, T3, T4> : Procedure<P>, IRecordSetProcedure<P, T1, T2, T3, T4>
        where P : new()
        where T1 : new()
        where T2 : new()
        where T3 : new()
        where T4 : new()
    {
        public RecordSetProcedure(ICommandDataProvider cdp)
            : base(cdp)
        {
        }

        public void ExecuteIntoLists(
            out List<T1> list1, out List<T2> list2, out List<T3> list3, out List<T4> list4)
        {
            var l1 = new List<T1>();
            var l2 = new List<T2>();
            var l3 = new List<T3>();
            var l4 = new List<T4>();

            ExecuteMultiRecordSet(() =>
            {
                ExecuteRecordSetInGroup<T1>((record) => { l1.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T2>((record) => { l2.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T3>((record) => { l3.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T4>((record) => { l4.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
            });

            list1 = l1;
            list2 = l2;
            list3 = l3;
            list4 = l4;
        }

        protected abstract void BindRecord(T1 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T2 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T3 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T4 record, IRecordDataExtractor rde);
    }

    public abstract class RecordSetProcedure<P, T1, T2, T3, T4, T5, T6> : Procedure<P>, IRecordSetProcedure<P, T1, T2, T3, T4, T5, T6>
        where P : new()
        where T1 : new()
        where T2 : new()
        where T3 : new()
        where T4 : new()
        where T5 : new()
        where T6 : new()
    {
        public RecordSetProcedure(ICommandDataProvider cdp)
            : base(cdp)
        {
        }

        public void ExecuteIntoLists(
            out List<T1> list1, out List<T2> list2, out List<T3> list3,
            out List<T4> list4, out List<T5> list5, out List<T6> list6)
        {
            var l1 = new List<T1>();
            var l2 = new List<T2>();
            var l3 = new List<T3>();
            var l4 = new List<T4>();
            var l5 = new List<T5>();
            var l6 = new List<T6>();

            ExecuteMultiRecordSet(() =>
            {
                ExecuteRecordSetInGroup<T1>((record) => { l1.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T2>((record) => { l2.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T3>((record) => { l3.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T4>((record) => { l4.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T5>((record) => { l5.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T6>((record) => { l6.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
            });

            list1 = l1;
            list2 = l2;
            list3 = l3;
            list4 = l4;
            list5 = l5;
            list6 = l6;
        }

        protected abstract void BindRecord(T1 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T2 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T3 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T4 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T5 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T6 record, IRecordDataExtractor rde);
    }

    public abstract class RecordSetProcedure<P, T1, T2, T3, T4, T5, T6, T7, T8, T9> : Procedure<P>, IRecordSetProcedure<P, T1, T2, T3, T4, T5, T6, T7, T8, T9>
        where P : new()
        where T1 : new()
        where T2 : new()
        where T3 : new()
        where T4 : new()
        where T5 : new()
        where T6 : new()
        where T7 : new()
        where T8 : new()
        where T9 : new()
    {
        public RecordSetProcedure(ICommandDataProvider cdp)
            : base(cdp)
        {
        }

        public void ExecuteIntoLists(
            out List<T1> list1, out List<T2> list2, out List<T3> list3,
            out List<T4> list4, out List<T5> list5, out List<T6> list6,
            out List<T7> list7, out List<T8> list8, out List<T9> list9)
        {
            var l1 = new List<T1>();
            var l2 = new List<T2>();
            var l3 = new List<T3>();
            var l4 = new List<T4>();
            var l5 = new List<T5>();
            var l6 = new List<T6>();
            var l7 = new List<T7>();
            var l8 = new List<T8>();
            var l9 = new List<T9>();

            ExecuteMultiRecordSet(() =>
            {
                ExecuteRecordSetInGroup<T1>((record) => { l1.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T2>((record) => { l2.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T3>((record) => { l3.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T4>((record) => { l4.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T5>((record) => { l5.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T6>((record) => { l6.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T7>((record) => { l7.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T8>((record) => { l8.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
                MoveToNextRecordSetInGroup();
                ExecuteRecordSetInGroup<T9>((record) => { l9.Add(record); return true; }, (record, rde) => { BindRecord(record, rde); });
            });

            list1 = l1;
            list2 = l2;
            list3 = l3;
            list4 = l4;
            list5 = l5;
            list6 = l6;
            list7 = l7;
            list8 = l8;
            list9 = l9;
        }

        protected abstract void BindRecord(T1 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T2 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T3 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T4 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T5 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T6 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T7 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T8 record, IRecordDataExtractor rde);

        protected abstract void BindRecord(T9 record, IRecordDataExtractor rde);
    }
}