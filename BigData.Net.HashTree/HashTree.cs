using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BigData.Net;

namespace BigData.Net
{
    public class HashTree<U,T>:ConsistentHash<U,HashTree<U,T>>, IHashNode<U,T> 
    {
        T _value;
        U _key;
        HashTree<U,T> _hash;

        public U Key
        {
            get { return _key; }
        }

        public T Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public HashTree():base(new FNVHash())
        {
            _key = default(U);
            _hash = this;
        }

        private HashTree(HashTree<U,T> root, U key, T value)
            : base(new FNVHash())
        {
            _key = key;
            _hash = root;
            _value = value; 
        }

        public IHashNode<U,T> Add(U node, T value)
        { 
            var hash = new HashTree<U,T>(this._hash,node,value);
            this.Add(node,hash);
            return this; 
        }

        public IHashNode<U,T> this[U node]
        {
            get
            {
                IHashNode<U,T> hash = base[node];
                if (hash != null && hash.Key.Equals(node))
                {
                    return hash;
                }
                else
                {
                    if (this.Key != null) 
                    {
                        if (_value != null)
                        {
                            return this; 
                        }
                        else
                        {
                            return _hash[node]; 
                        }
                    }
                    return this; 
                }
            }
        }

        public void Save()
        {
            this.Commit();
        }
    }

}
