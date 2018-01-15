using System;
using System.Reflection;

namespace Glass.Pool
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private class _Inner
        {
            public static T _instance;

            static _Inner()
            {
                var c = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                    null, new Type[] { }, null);

                _instance = (T)c.Invoke(new object[] { });
            }
        }

        public static T Instance
        {
            get { return _Inner._instance; }
        }
    }
}
