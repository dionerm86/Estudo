using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

namespace Glass.Pool
{
    public static class Pool
    {
        #region Classe Privada

        private class PoolObject<T> : IDisposable
        {
            private delegate T Construtor();

            public enum LoadingMode { Eager, Lazy, LazyExpanding };

            public enum AccessMode { FIFO, LIFO, Circular };

            private bool isDisposed;
            private Construtor factory;
            private LoadingMode loadingMode;
            private IItemStore itemStore;
            private int size;
            private int count;

            public PoolObject(int size, ConstructorInfo factory)
                : this(size, factory, LoadingMode.Lazy, AccessMode.FIFO)
            {
            }

            public PoolObject(int size, ConstructorInfo factory,
                LoadingMode loadingMode, AccessMode accessMode)
            {
                if (size <= 0)
                    throw new ArgumentOutOfRangeException("size", size,
                        "Argument 'size' must be greater than zero.");

                if (factory == null)
                    throw new ArgumentNullException("factory");

                CreateDelegate(factory);
                this.size = size;
                this.loadingMode = loadingMode;
                this.itemStore = CreateItemStore(accessMode, size);

                if (loadingMode == LoadingMode.Eager)
                    PreloadItems();
            }

            private void CreateDelegate(ConstructorInfo ctor)
            {
                var mthd = new DynamicMethod("", typeof(T), new Type[] { typeof(object[]) }, typeof(T));
                var il = mthd.GetILGenerator();
                
                var ctorParams = ctor.GetParameters();
                for (int i = 0; i < ctorParams.Length; i++)
                {
                    il.Emit(OpCodes.Ldarg_0);

                    switch (i)
                    {
                        case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                        case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                        case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                        case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                        case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                        case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                        case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                        case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                        case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                        default: il.Emit(OpCodes.Ldc_I4, i); break;
                    }

                    il.Emit(OpCodes.Ldelem_Ref);
                    Type paramType = ctorParams[i].ParameterType;
                    il.Emit(paramType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, paramType);
                }

                il.Emit(OpCodes.Newobj, ctor);
                il.Emit(OpCodes.Ret);

                factory = mthd.CreateDelegate(typeof(Construtor)) as Construtor;
            }

            public T Acquire()
            {
                switch (loadingMode)
                {
                    case LoadingMode.Eager:
                        return AcquireEager();
                    case LoadingMode.Lazy:
                        return AcquireLazy();
                    default:
                        return AcquireLazyExpanding();
                }
            }

            public bool Release(T item)
            {
                lock (itemStore)
                    return Insert(item);
            }

            private bool Insert(T item)
            {
                if (!isDisposed && itemStore.Count < size)
                {
                    itemStore.Store(item);
                    return true;
                }

                return false;
            }

            public void Dispose()
            {
                if (isDisposed)
                    return;

                isDisposed = true;
                if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
                {
                    lock (itemStore)
                    {
                        while (itemStore.Count > 0)
                        {
                            IDisposable disposable = (IDisposable)itemStore.Fetch();
                            disposable.Dispose();
                        }
                    }
                }
            }

            #region Acquisition

            private T AcquireEager()
            {
                lock (itemStore)
                {
                    if (itemStore.Count == 0)
                        PreloadItems();

                    return itemStore.Fetch();
                }
            }

            private T AcquireLazy()
            {
                lock (itemStore)
                {
                    if (itemStore.Count > 0)
                        return itemStore.Fetch();

                    Interlocked.Increment(ref count);
                    return factory();
                }
            }

            private T AcquireLazyExpanding()
            {
                bool shouldExpand = false;
                if (count < size)
                {
                    int newCount = Interlocked.Increment(ref count);
                    if (newCount <= size)
                        shouldExpand = true;
                    else
                    {
                        // Another thread took the last spot - use the store instead
                        Interlocked.Decrement(ref count);
                    }
                }

                if (shouldExpand)
                    return factory();
                else
                    lock (itemStore)
                        return itemStore.Fetch();
            }

            private void PreloadItems()
            {
                for (int i = 0; i < size; i++)
                    Insert(factory());

                count = size;
            }

            #endregion

            #region Collection Wrappers

            interface IItemStore
            {
                T Fetch();
                void Store(T item);
                int Count { get; }
            }

            private IItemStore CreateItemStore(AccessMode mode, int capacity)
            {
                switch (mode)
                {
                    case AccessMode.FIFO:
                        return new QueueStore(capacity);
                    case AccessMode.LIFO:
                        return new StackStore(capacity);
                    default:
                        return new CircularStore(capacity);
                }
            }

            class QueueStore : Queue<T>, IItemStore
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

            class StackStore : Stack<T>, IItemStore
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

            class CircularStore : IItemStore
            {
                private List<Slot> slots;
                private int freeSlotCount;
                private int position = -1;

                public CircularStore(int capacity)
                {
                    slots = new List<Slot>(capacity);
                }

                public T Fetch()
                {
                    if (Count == 0)
                        throw new InvalidOperationException("The buffer is empty.");

                    int startPosition = position;
                    do
                    {
                        Advance();
                        Slot slot = slots[position];
                        if (!slot.IsInUse)
                        {
                            slot.IsInUse = true;
                            --freeSlotCount;
                            return slot.Item;
                        }
                    } while (startPosition != position);
                    throw new InvalidOperationException("No free slots.");
                }

                public void Store(T item)
                {
                    Slot slot = slots.Find(s => object.Equals(s.Item, item));
                    if (slot == null)
                    {
                        slot = new Slot(item);
                        slots.Add(slot);
                    }
                    slot.IsInUse = false;
                    ++freeSlotCount;
                }

                public int Count
                {
                    get { return freeSlotCount; }
                }

                private void Advance()
                {
                    position = (position + 1) % slots.Count;
                }

                class Slot
                {
                    public Slot(T item)
                    {
                        this.Item = item;
                    }

                    public T Item { get; private set; }
                    public bool IsInUse { get; set; }
                }
            }

            #endregion

            public bool IsDisposed
            {
                get { return isDisposed; }
            }
        }

        #endregion

        #region Campos Privados

        private static object syncRoot = new object();
        private static volatile Dictionary<Type, object> _pool = new Dictionary<Type, object>();

        #endregion

        public static bool IsRegistered<T>()
        {
            return _pool.ContainsKey(typeof(T));
        }

        public static void Register<T>(int size, ConstructorInfo factory)
        {
            lock (syncRoot)
            {
                if (IsRegistered<T>())
                    return;

                _pool.Add(typeof(T), new PoolObject<T>(size, factory));
            }
        }

        public static void Unregister<T>()
        {
            lock (syncRoot)
            {
                if (!IsRegistered<T>())
                    return;

                _pool.Remove(typeof(T));
            }
        }

        public static T Acquire<T>()
        {
            if (!IsRegistered<T>())
                throw new ArgumentException("Tipo não registrado.", "T");

            return (_pool[typeof(T)] as PoolObject<T>).Acquire();
        }

        public static bool Release<T>(T item)
        {
            if (!IsRegistered<T>())
                throw new ArgumentException("Tipo não registrado.", "T");

            return (_pool[typeof(T)] as PoolObject<T>).Release(item);
        }
    }
}