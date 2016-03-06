using System.Collections.Generic;

namespace Zuris.SPDAL
{
    public interface IRecordSetProcedure<P, T> : IProcedure<P>
        where P : new()
        where T : new()
    {
        T ExecuteIntoRecord();

        List<T> ExecuteIntoList();
    }

    public interface IRecordSetProcedure<P, T1, T2> : IProcedure<P>
        where P : new()
        where T1 : new()
        where T2 : new()
    {
        void ExecuteIntoLists(out List<T1> list1, out List<T2> list2);
    }

    public interface IRecordSetProcedure<P, T1, T2, T3> : IProcedure<P>
        where P : new()
        where T1 : new()
        where T2 : new()
        where T3 : new()
    {
        void ExecuteIntoLists(out List<T1> list1, out List<T2> list2, out List<T3> list3);
    }

    public interface IRecordSetProcedure<P, T1, T2, T3, T4> : IProcedure<P>
        where P : new()
        where T1 : new()
        where T2 : new()
        where T3 : new()
        where T4 : new()
    {
        void ExecuteIntoLists(
            out List<T1> list1, out List<T2> list2, out List<T3> list3, out List<T4> list4);
    }

    public interface IRecordSetProcedure<P, T1, T2, T3, T4, T5, T6> : IProcedure<P>
        where P : new()
        where T1 : new()
        where T2 : new()
        where T3 : new()
        where T4 : new()
        where T5 : new()
        where T6 : new()
    {
        void ExecuteIntoLists(
            out List<T1> list1, out List<T2> list2, out List<T3> list3, out List<T4> list4, out List<T5> list5, out List<T6> list6);
    }

    public interface IRecordSetProcedure<P, T1, T2, T3, T4, T5, T6, T7, T8, T9> : IProcedure<P>
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
        void ExecuteIntoLists(
            out List<T1> list1, out List<T2> list2, out List<T3> list3,
            out List<T4> list4, out List<T5> list5, out List<T6> list6,
            out List<T7> list7, out List<T8> list8, out List<T9> list9);
    }
}