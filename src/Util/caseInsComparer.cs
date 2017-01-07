using System;
using System.Collections.Generic;

namespace Util
{
    public struct CaseInsComparer : IEqualityComparer<KeyValuePair<byte,string>>
    {
        public int GetHashCode(KeyValuePair<byte,string> obj)
        {
            return obj.Value.GetHashCode();
        }

        public bool Equals(KeyValuePair<byte,string> a, KeyValuePair<byte,string> b)
        {
            bool status = false;
            if (String.Compare(a.Value, b.Value, true) == 0)
            {
                status = true;
            }
            return status;
        }
    };
}
