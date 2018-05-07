namespace Glass.Pool
{
    interface IItemStore<T>
    {
        T Fetch();
        void Store(T item);
        int Count { get; }
    }
}
