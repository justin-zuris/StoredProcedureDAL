namespace Zuris.SPDAL
{
    public class ParameterGroup<T> : BaseParameterGroup, IParameterGroup where T : class
    {
        public virtual TI GetCopy<TI>() where TI : T, new()
        {
            // could create a dynamic proxy at some point?
            var newObject = new TI();
            CopyInto(newObject);
            return newObject;
        }

        public virtual void PopulateFrom(T parameters)
        {
            PopulateFrom(parameters as object);
        }

        public virtual void PopulateFrom(object parameters)
        {
            this.CopyPropertiesFrom<T>(parameters);
        }

        public virtual void CopyInto(T parameters)
        {
            CopyInto(parameters as object);
        }

        public virtual void CopyInto(object parameters)
        {
            parameters.CopyPropertiesFrom<T>(this);
        }
    }
}