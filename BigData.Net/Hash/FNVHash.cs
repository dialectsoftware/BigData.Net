using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigData.Net
{
    //http://www.codeproject.com/Articles/28296/C-FNV-Hash
    public class FNVHash : HashFunction
    {
        public override ulong hash(object key)
        {
            const ulong p = 1099511628211; 
            ulong hash = 2166136261;
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(key.ToString());
            foreach (byte b in data)
                hash = (hash ^ b) * p;
            hash += hash << 13;
            hash ^= hash >> 7;
            hash += hash << 3;
            hash ^= hash >> 17;
            hash += hash << 5;
            return hash;
        }
    }
}
