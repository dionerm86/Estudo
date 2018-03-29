using System.Collections.Generic;

namespace Glass.Pool
{
    class QueueStore<T> : Queue<T>, IItemStore<T>
    {
        public QueueStore(int capacity)
            : base(capacity)
        {
        }

        public T Fetch()
        {
            return Dequeue();
        }

        public void Store(T item)
        {
            Enqueue(item);
        }
    }
}
