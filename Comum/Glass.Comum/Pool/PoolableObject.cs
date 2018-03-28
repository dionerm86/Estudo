using System;
using System.Reflection;

namespace Glass.Pool
{
    public abstract class PoolableObject<T> : IDisposable
        where T : PoolableObject<T>
    {
        static PoolableObject()
        {
            var c = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                null, new Type[] { }, null);

            Pool.Register<T>(5, c);
        }

        #region Instância

        /// <summary>
        /// Instância do objeto.
        /// </summary>
        public static T Instance
        {
            get { return Pool.Acquire<T>(); }
        }

        #endregion

        #region IDisposable Members

        ~PoolableObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// Remove o objeto da memória.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (!disposing)
            //    GC.ReRegisterForFinalize(this);

            if (Pool.IsRegistered<T>())
                Pool.Release<T>(this as T);
        }

        #endregion
    }
}
