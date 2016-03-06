using System;
using System.Collections.Generic;

namespace Zuris.SPDAL
{
    public interface IEvaluateRetryable
    {
        bool IsRetryable(Exception ex, out Dictionary<int, string> sourceErrorCodes);
    }
}