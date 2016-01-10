using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigData.Net
{
    public abstract class HashFunction
    {
        public abstract ulong hash(Object key);
    }
}
