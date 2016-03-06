using System;

namespace Zuris
{
    public class ZurisException : Exception
    {
        protected int _errorCode = 0;
        protected object _contextualData;

        public ZurisException()
            : base()
        {
        }

        public ZurisException(string message, object contextualData = null)
            : base(message)
        {
            _contextualData = contextualData;
        }

        public ZurisException(string message, int errorCode, object contextualData = null)
            : base(message)
        {
            _errorCode = errorCode;
            _contextualData = contextualData;
        }

        public virtual object ContextualData { get { return _contextualData; } protected set { _contextualData = value; } }

        public virtual int ErrorCode { get { return _errorCode; } protected set { _errorCode = value; } }

        public virtual string ErrorTypeString { get { return _errorCode.ToString(); } }
    }

    public class ZurisException<T> : ZurisException where T : struct, IConvertible
    {
        protected T _errorType = default(T);

        public ZurisException()
            : base()
        {
        }

        public ZurisException(string message, object contextualData = null)
            : base(message, contextualData)
        {
        }

        public ZurisException(T errorType, string message = null, object contextualData = null)
            : base(message, contextualData)
        {
            ErrorType = errorType;
        }

        public virtual T ErrorType
        {
            get { return _errorType; }
            protected set
            {
                _errorType = value;
                try { ErrorCode = Convert.ToInt32(_errorType); }
                catch { }
            }
        }

        public override string ErrorTypeString { get { return _errorType.ToString(); } }
    }
}