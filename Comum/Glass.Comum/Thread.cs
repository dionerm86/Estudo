using System;
using System.Threading;
using System.Globalization;
using System.Security.Principal;
using System.Web;

namespace Glass
{
    public sealed class Thread
    {
        private System.Threading.Thread _thread;
        private DadosThread _dadosThread;

        #region Classe privada

        private class DadosThread
        {
            public ThreadStart threadStart;
            public ParameterizedThreadStart parameterizedThreadStart;
            public CultureInfo cultura;
            public object parametro;

            public IPrincipal principal;
            public ExecutionContext executionContext;
            public HttpContext httpContext;

            private DadosThread()
            {
                httpContext = HttpContext.Current;
                principal = httpContext.User;
                executionContext = ExecutionContext.Capture();
                cultura = System.Threading.Thread.CurrentThread.CurrentCulture;
            }

            public DadosThread(ThreadStart threadStart)
                : this()
            {
                this.threadStart = threadStart;
            }

            public DadosThread(ParameterizedThreadStart parameterizedThreadStart)
                : this()
            {
                this.parameterizedThreadStart = parameterizedThreadStart;
            }
        }

        #endregion

        #region Construtores

        private void Init()
        {
            _thread = new System.Threading.Thread(Executar);
        }

        public Thread(ParameterizedThreadStart start)
        {
            _dadosThread = new DadosThread(start);
            Init();
        }

        public Thread(ThreadStart start)
        {
            _dadosThread = new DadosThread(start);
            Init();
        }

        #endregion

        #region Propriedades

        public ExecutionContext ExecutionContext
        {
            get { return _thread.ExecutionContext; }
        }

        public bool IsAlive
        {
            get { return _thread.IsAlive; }
        }

        #endregion

        #region Métodos públicos

        public void Abort()
        {
            _thread.Abort();
        }

        public void Abort(object stateInfo)
        {
            _thread.Abort(stateInfo);
        }

        public void Interrupt()
        {
            _thread.Interrupt();
        }

        public void Join()
        {
            _thread.Join();
        }

        public bool Join(int millisecondsTimeout)
        {
            return _thread.Join(millisecondsTimeout);
        }

        public bool Join(TimeSpan timeout)
        {
            return _thread.Join(timeout);
        }

        public void Resume()
        {
            _thread.Resume();
        }

        public void Start()
        {
            _thread.Start(_dadosThread);
        }

        public void Start(object parameter)
        {
            _dadosThread.parametro = parameter;
            _thread.Start(_dadosThread);
        }

        public void Suspend()
        {
            _thread.Suspend();
        }

        #endregion

        #region Métodos estáticos públicos

        public static void Sleep(int millisecondsTimeout)
        {
            System.Threading.Thread.Sleep(millisecondsTimeout);
        }

        public static void Sleep(TimeSpan timeout)
        {
            System.Threading.Thread.Sleep(timeout);
        }

        #endregion

        #region Métodos de execução

        private void Executar(object obj)
        {
            var dadosThread = obj as DadosThread;

            HttpContext.Current = dadosThread.httpContext;
            System.Threading.Thread.CurrentThread.CurrentCulture = dadosThread.cultura;
            System.Threading.Thread.CurrentThread.CurrentUICulture = dadosThread.cultura;

            ExecutionContext.Run(dadosThread.executionContext, state =>
            {
                var dt = state as DadosThread;

                HttpContext.Current = dt.httpContext;
                System.Threading.Thread.CurrentThread.CurrentCulture = dt.cultura;
                System.Threading.Thread.CurrentThread.CurrentUICulture = dt.cultura;

                if (dt.threadStart != null)
                    dt.threadStart();
                else
                    dt.parameterizedThreadStart(dt.parametro);

            }, dadosThread);
        }

        #endregion
    }
}
