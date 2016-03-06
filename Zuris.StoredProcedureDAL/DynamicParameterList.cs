using System.Collections.Generic;
using System.Data;

namespace Zuris.SPDAL
{
    public class DynamicParameterList : BaseParameterGroup
    {
        public DynamicParameterList()
            : base()
        {
            _queryParameters = new List<IObjectQueryParam>();
        }

        public void AddParameter<T>(IObjectQueryParam parameter)
        {
            this._queryParameters.Add(parameter);
        }

        public void Add<T>(IQueryParam<T> parameter)
        {
            this._queryParameters.Add(parameter);
        }

        public IQueryParam<T> Add<T>(string parameterName, T value,
            DbType type = DbType.Object, ParameterDirection direction = ParameterDirection.Input, int? size = null, int? precision = null, int? scale = null)
        {
            var p = new QueryParam<T>
            {
                Name = parameterName,
                Value = value,
                DbType = type,
                Direction = direction,
                IsRequired = true
            };

            if (size.HasValue) p.Size = size.Value;
            if (precision.HasValue) p.Precision = precision.Value;
            if (scale.HasValue) p.Scale = scale.Value;

            this._queryParameters.Add(p);
            return p;
        }
    }
}