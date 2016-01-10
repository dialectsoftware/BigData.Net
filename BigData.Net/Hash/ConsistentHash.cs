using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;

namespace BigData.Net
{
    public class ConsistentHash<U,T>
    {
        private List<ulong> keys;
        private readonly HashFunction hashFunction;
        private readonly SortedDictionary<ulong, T> circle = new SortedDictionary<ulong, T>();
        private readonly AutoResetEvent syncHandleA = new AutoResetEvent(true);

        public ConsistentHash():this(new FNVHash())
        { 
        
        }

        public ConsistentHash(HashFunction hashFunction )
        {
            
            this.hashFunction = hashFunction;
        }

        public virtual void Add(U key, T node)
        {
            if (Monitor.TryEnter(syncHandleA.SafeWaitHandle))
            {
                circle.Add(hashFunction.hash(key), node);
                Monitor.Exit(syncHandleA.SafeWaitHandle);
            }
        }

        public virtual void Remove(U key)
        {
            if (Monitor.TryEnter(syncHandleA.SafeWaitHandle))
            {
                circle.Remove(hashFunction.hash(key));
                Monitor.Exit(syncHandleA.SafeWaitHandle);
            }
         
        }

        public virtual void Commit()
        { 
            if (Monitor.TryEnter(syncHandleA.SafeWaitHandle))
            {
                keys = circle.Keys.ToList();
                Monitor.Exit(syncHandleA.SafeWaitHandle);
            }
        
        }

        public virtual T this[Object key]
        {
            get
            {
                if (0 == circle.Count())
                {
                    return default(T);
                }
                ulong hash = hashFunction.hash(key);
                hash = keys.BinarySearch<ulong, ulong>((l) => l, hash);
                T value = default(T);
                circle.TryGetValue(hash, out value);
                return value;
            }

        }
    }
}

