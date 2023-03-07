using System;
using System.Collections.Generic;

namespace RS.UtilityLib.RetryLib
{
    public class RetryResult
    {
        public RetryResult(RetryContext context)
        {
            Context = context;
        }

        public RetryContext Context { get; private set; }

        public RetryResult<TResult> WithValue<TResult>(TResult value)
        {
            var result = new RetryResult<TResult>(Context, value);
            return result;
        }
    }
}
