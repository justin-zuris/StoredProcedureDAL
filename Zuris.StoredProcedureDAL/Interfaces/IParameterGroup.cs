using System.Collections.Generic;

namespace Zuris.SPDAL
{
    public interface IParameterGroup
    {
        List<IObjectQueryParam> QueryParameters { get; }

        List<IObjectQueryParam> OutputQueryParameters { get; }
    }
}