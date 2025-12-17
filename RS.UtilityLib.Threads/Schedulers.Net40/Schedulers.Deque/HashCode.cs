using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schedulers
{
    internal class HashCode
    {
        internal static int Combine(int schedulerId, long jobId, int version)
        {
            //string str = $"{schedulerId},{jobId},{version}";
            //return str.GetHashCode();
            unchecked
            {
                int hash = 17;
                //foreach (var code in hashCodes)
                //{
                hash = hash * 31 + schedulerId;
                hash = (int)(hash * 31 + jobId);
                hash = hash * 31 + version;
                //}
                return hash;
            }
        }

        internal static int Combine(int v1, int v2)
        {
            //string str = $"{v1},{v2}";
            //return str.GetHashCode();
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + v1;
                hash = hash * 31 + v2;
                return hash;
            }
        }
    }
}
