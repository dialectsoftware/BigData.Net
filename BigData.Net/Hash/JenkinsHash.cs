using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigData.Net
{
    public class JenkinsHash : HashFunction 
    {
        public override ulong hash(object key)
        {
            Int64 hash, i;
            string value = key.ToString();
            for (hash = i = 0; i < value.Length; ++i)
            {
                hash += value[(int)i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return (ulong)hash;
        }
    }
}
