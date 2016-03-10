using System.Collections.Generic;
using System.Data;

namespace Zuris.SPDAL
{
    public class QueryParam<T> : IQueryParam<T>
    {
        private T _value;
        private bool _isActivated;
        private bool _hasValue;
        private ParameterDirection _direction = ParameterDirection.Input;

        public QueryParam()
        {
        }

        public QueryParam(T value)
        {
            _value = value;
            _hasValue = (value != null);
            _isActivated = true;
        }

        public ParameterDirection Direction { get { return _direction; } set { _direction = value; } }

        public DbType? DbType { get; set; }

        public bool IsRequired { get; set; }

        public int Size { get; set; }

        public int Scale { get; set; }

        public int Precision { get; set; }

        public string Name { get; set; }

        public bool HasValue { get { return _hasValue; } }

        public bool IsActivated { get { return _isActivated; } }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _hasValue = (value != null);
                _isActivated = true;
            }
        }

        public void ClearValue()
        {
            _hasValue = false; _isActivated = false; _value = default(T);
        }

        public override bool Equals(object other)
        {
            if (!HasValue) return other == null;
            if (other == null) return false;
            return _value.Equals(other);
        }

        public override int GetHashCode()
        {
            return HasValue ? _value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return HasValue ? _value.ToString() : "";
        }

        object IObjectQueryParam.ObjectValue
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (T)value;
            }
        }
    }

    public static class QueryParam
    {
        public static int Compare<T>(QueryParam<T> n1, QueryParam<T> n2)
        {
            if (n1.HasValue)
            {
                if (n2.HasValue) return Comparer<T>.Default.Compare(n1.Value, n2.Value);
                return 1;
            }
            if (n2.HasValue) return -1;
            return 0;
        }

        public static bool Equals<T>(QueryParam<T> n1, QueryParam<T> n2)
        {
            if (n1.HasValue)
            {
                if (n2.HasValue) return EqualityComparer<T>.Default.Equals(n1.Value, n2.Value);
                return false;
            }
            if (n2.HasValue) return false;
            return true;
        }
    }
}