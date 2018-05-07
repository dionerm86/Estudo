using System.Collections.Generic;

namespace Glass.Pool
{
    class StackStore<T> : Stack<T>, IItemStore<T>
    {
        public StackStore(int capacity)
            : base(capacity)
        {
        }

        public T Fetch()
        {
            return Pop();
        }

        public void Store(T item)
        {
            Push(item);
        }
    }
}
