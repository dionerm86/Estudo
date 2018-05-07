using Glass.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Glass.Pool
{
    class PoolObject<T> : IDisposable
    {
        private delegate T Construtor();

        public enum LoadingMode { Eager, Lazy, LazyExpanding };

        public enum AccessMode { FIFO, LIFO, Circular };

        private bool isDisposed;
        private Construtor factory;
        private readonly LoadingMode loadingMode;
        private readonly IItemStore<T> itemStore;
        private readonly int size;
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

        #region IDisposable Members

        ~PoolObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;

            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
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

        #endregion

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

        private IItemStore<T> CreateItemStore(AccessMode mode, int capacity)
        {
            switch (mode)
            {
                case AccessMode.FIFO:
                    return new QueueStore<T>(capacity);
                case AccessMode.LIFO:
                    return new StackStore<T>(capacity);
                default:
                    return new CircularStore<T>(capacity);
            }
        }

        public bool IsDisposed
        {
            get { return isDisposed; }
        }
    }
}
