using System;
using System.Collections.Generic;


namespace RS.UtilityLib.RetryLib
{
    public class RetryResult<TResult> : RetryResult
    {
        public RetryResult(RetryContext context, TResult value) : base(context)
        {
            Value = value;
        }

        public TResult Value { get; private set; }
    }
}
