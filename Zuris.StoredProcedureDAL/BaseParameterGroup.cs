using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Zuris.SPDAL
{
    public abstract class BaseParameterGroup : IParameterGroup
    {
        protected List<IObjectQueryParam> _queryParameters;

        public virtual List<IObjectQueryParam> QueryParameters
        {
            get
            {
                if (_queryParameters == null)
                {
                    //FRAMEWORK VERSION CONFLICT: 4.5
                    //_queryParameters = this.GetType().GetProperties()
                    //    .Where(p => p.CanRead && p.PropertyType.GetInterfaces().Contains(typeof(IObjectQueryParam)))
                    //    .Select(p => p.GetValue(this) as IObjectQueryParam).ToList();
                    _queryParameters = this.GetType().GetProperties()
                        .Where(p => p.CanRead && p.PropertyType.GetInterfaces().Contains(typeof(IObjectQueryParam)))
                        .Select(p => p.GetValue(this, new object[] { }) as IObjectQueryParam).ToList();
                }
                return _queryParameters;
            }
        }

        public virtual List<IObjectQueryParam> OutputQueryParameters
        {
            get
            {
                return QueryParameters.Where(qp => qp.Direction == ParameterDirection.Output || qp.Direction == ParameterDirection.InputOutput).ToList();
            }
        }
    }
}