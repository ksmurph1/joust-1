using System;
using System.Collections.Generic;

namespace Util
{
    public struct CaseInsComparer : IEqualityComparer<string>
    {
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }

        public bool Equals(string a, string b)
        {
            bool status = false;
            if (String.Compare(a, b, true) == 0)
            {
                status = true;
            }
            return status;
        }
    };
}
