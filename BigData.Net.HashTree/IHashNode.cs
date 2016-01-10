using System;


namespace BigData.Net
{
    public interface IHashNode<U,T>
    {
        U Key { get; }
        T Value { get; set; }
        IHashNode<U,T> Add(U node, T value);
        IHashNode<U,T> this[U node] { get; }
        void Save();
        
    }
}
