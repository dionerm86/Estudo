using System;
using System.Collections.Generic;
using System.Reflection;

namespace Glass.Pool
{
    static class Pool
    {
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