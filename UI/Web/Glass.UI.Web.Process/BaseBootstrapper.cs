using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Glass.UI.Web.Process
{
    /// <summary>
    /// Bootstrapper do portal.
    /// </summary>
    public abstract class BaseBootstrapper : Colosoft.Mef.MefBootstrapper
    {
        #region Nested Types

        /// <summary>
        /// Implementação para a data do servidor.
        /// </summary>
        class ServerData : Colosoft.IServerData
        {
            public DateTimeOffset GateDateTimeOffSet()
            {
                return DateTimeOffset.Now;
            }

            public DateTime GetDateTime()
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Implementação fake do downloader da entradas do cache.
        /// </summary>
        class DataEntryDownloader : Colosoft.Data.Caching.IDataEntryDownloader
        {
            #region Eventos

            public event Colosoft.Net.DownloadCompletedEventHandler DownloadCompleted;
            public event Colosoft.Net.DownloadProgressEventHandler ProgressChanged;

            #endregion

            #region Propriedades

            public bool IsBusy
            {
                get { return false; }
            }

            #endregion

            #region Métodos Públicos

            public void Add(Colosoft.Data.Caching.DataEntryVersion version)
            {
            }

            public void AddRange(IEnumerable<Colosoft.Data.Caching.DataEntryVersion> versions)
            {
            }

            public void Clear()
            {
            }

            public void CancelAsync()
            {
            }

            public void RunAsync(object userState)
            {
            }

            public void Dispose()
            {
            }

            #endregion
        }

        #endregion

        #region Métodos Protegidos

        /// <summary>
        /// Configura o catalogo que será usado no sistema.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            
            AggregateCatalog.Catalogs.Add(new MefContrib.Hosting.Conventions.ConventionCatalog(
                new Colosoft.Mef.PartConventionBuilder()

                    .Add<Colosoft.Business.EntityEventManager, Colosoft.Business.IEntityEventManager>()
                    .Add<Colosoft.Logging.DebugLogger, Colosoft.Logging.ILogger>()
                    .Add<Colosoft.Web.Security.UserContextConfigurator, Colosoft.Security.IUserContextConfigurator>()
                    .AddImportingConstructor<Colosoft.Business.BusinessValidationManager, Colosoft.Validation.IValidationManager>()

                    .Add<Colosoft.Business.BusinessEntityTypeLoader, Colosoft.Business.IBusinessEntityTypeLoader>()
                    .AddImportingConstructor<Colosoft.Business.BusinessEntityTypeManager,
                        Colosoft.Business.IEntityTypeManager>()

                    .AddImportingConstructor<Colosoft.Business.EntityManager,
                            Colosoft.Business.IEntityManager>()

                    .Add<Colosoft.Data.Database.Generic.PersistenceTransactionExecuter, Colosoft.Data.IPersistenceTransactionExecuter>(true)
                    .AddImportingConstructor<Colosoft.Query.Database.Generic.ProviderLocator, Colosoft.Query.IProviderLocator>()
                    .AddImportingConstructor<Colosoft.Query.Database.SqlSourceContext, Colosoft.Query.ISourceContext>()

                    .Add<Colosoft.Data.Caching.Local.DataEntriesRepository, Colosoft.Data.Caching.IDataEntriesRepository>()

                    .AddImportingConstructor<Colosoft.Data.Schema.RecordKeyFactory, Colosoft.Query.IRecordKeyFactory>()

                    .Add<Colosoft.Security.Captcha.CaptchaImpl, Colosoft.Security.CaptchaSupport.ICaptcha>()
                ));

            AggregateCatalog.Catalogs.Add(new Colosoft.Mef.InstanceCatalog()
               

               .Add<Colosoft.Data.Caching.IDataCacheManager>(new Lazy<Colosoft.Data.Caching.IDataCacheManager>(() =>
               {
                   var manager = new Colosoft.Data.Caching.DataCacheManager
                       (Container.GetExportedValue<Colosoft.Query.ISourceContext>(),
                        Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>(),
                        new DataEntryDownloader(),
                        Container.GetExportedValue<Colosoft.Data.Caching.IDataEntriesRepository>(),
                        Container.GetExportedValue<Colosoft.Data.Caching.ICacheLoaderObserver>(),
                        Container.GetExportedValue<Colosoft.Logging.ILogger>());

                   return manager;

               }))

               .Add<Colosoft.Data.IPersistenceContext>(new Lazy<Colosoft.Data.IPersistenceContext>(() =>
               {
                   var typeSchema = Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>();
                   /*var databaseContext = CreateDatabasePersistenceContext(typeSchema);
                   var recordKeyFactory = Container.GetExportedValue<Colosoft.Query.IRecordKeyFactory>();

                   var cacheContext = new Colosoft.Caching.CachePersistenceContext(
                       new Lazy<Colosoft.Caching.ICacheProvider>(() =>
                   {
                       return Container.GetExportedValue<Colosoft.Data.Caching.IDataCacheManager>();
                   }), new Lazy<Colosoft.Data.Schema.ITypeSchema>(() => typeSchema),
                       new Lazy<Colosoft.Query.IRecordKeyFactory>(() => recordKeyFactory));

                   return new Colosoft.Data.Caching.Dymanic.DynamicPersistenceContext
                       (databaseContext, cacheContext, typeSchema, recordKeyFactory);*/
                   return CreateDatabasePersistenceContext(typeSchema);
               }))

               .Add<Colosoft.Caching.CacheDataSource>(new Lazy<Colosoft.Caching.CacheDataSource>(() =>
               {
                   var cacheManager = Container.GetExportedValue<Colosoft.Data.Caching.IDataCacheManager>();
                   var typeSchema = Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>();
                   return new Colosoft.Caching.CacheDataSource(cacheManager, typeSchema);
               }))
           );
        }

        /// <summary>
        /// Cria o contexto de persistencia do banco de dados.
        /// </summary>
        /// <param name="typeSchema"></param>
        /// <returns></returns>
        protected abstract Colosoft.Data.IPersistenceContext CreateDatabasePersistenceContext(Colosoft.Data.Schema.ITypeSchema typeSchema);

        /// <summary>
        /// Configura o container.
        /// </summary>
        protected override void ConfigureContainer()
        {
            Container.ComposeExportedValue<Colosoft.IServerData>(new ServerData());

            Colosoft.Business.EntityManager.SetEntityManager
                (new Lazy<Colosoft.Business.IEntityManager>(() => Container.GetExportedValue<Colosoft.Business.IEntityManager>()));

            var aggregateCacheLoaderObserver = new Colosoft.Data.Caching.AggregateCacheLoaderObserver();

            Container.ComposeExportedValue<Colosoft.Data.Caching.AggregateCacheLoaderObserver>(aggregateCacheLoaderObserver);
            Container.ComposeExportedValue<Colosoft.Data.Caching.ICacheLoaderObserver>(aggregateCacheLoaderObserver);

            Container.ComposeExportedValue<Colosoft.DataAccess.IQueryDataSourceSelector>(
                new Colosoft.DataAccess.QueryDataSourceSelector(
                    new Lazy<Colosoft.Data.Caching.IDataCacheManager>(() => Container.GetExportedValue<Colosoft.Data.Caching.IDataCacheManager>()),
                    new Lazy<Colosoft.Query.IQueryDataSource>(() => Container.GetExportedValue<Colosoft.Caching.CacheDataSource>()),
                    new Lazy<Colosoft.Query.IQueryDataSource>(() => Container.GetExportedValue<Colosoft.Query.IQueryDataSource>()),
                    new Lazy<Colosoft.Data.Schema.ITypeSchema>(() => Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>()),
                    new Lazy<Colosoft.Query.IRecordKeyFactory>(() => Container.GetExportedValue<Colosoft.Query.IRecordKeyFactory>()),
                    Container.GetExportedValue<Colosoft.Logging.ILogger>()
                ));

            base.ConfigureContainer();

            Colosoft.Query.RecordKeyFactory.Instance = Container.GetExportedValue<Colosoft.Query.IRecordKeyFactory>();
        }

        /// <summary>
        /// Método acionado para configurar os listeners do GDA.
        /// </summary>
        protected virtual void ConfigureGDAListeners()
        {
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Inicializa o cache.
        /// </summary>
        private void InitializeCache()
        {
            // Recupera a instancia do gerenciador do cache de dados
            var dataCacheManager = Container.GetExportedValue<Colosoft.Data.Caching.IDataCacheManager>();

            // Recupera o esquema de tipos do sistema
            var typeSchema = Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>();

            // Registra os tipos que deverão estar armazenados no cache
            foreach (var t in typeSchema.GetTypeMetadatas().Where(f => f.IsCache))
                dataCacheManager.Register(new Colosoft.Reflection.TypeName(string.Format("{0}.{1}, {2}", t.Namespace, t.Name, t.Assembly)));

            var loadProcessingErrors = new Queue<Colosoft.Caching.CacheErrorEventArgs>();
            Exception loadException = null;

            System.Threading.ManualResetEvent resetEvent = null;
            var controlHandler = new EventHandler((sender, e) =>
            {
                if (resetEvent != null) resetEvent.Set();
            });

            var loadErrorHandler = new Colosoft.Caching.CacheErrorEventHandler((sender, e) =>
            {
                // Recupera o erro ocorrido na carga
                loadException = e.Error;
                if (resetEvent != null) resetEvent.Set();
            });

            // Cria o método anônimo que serã usado para captar os erros ocorridos na carga do cache
            var loadProcessingErrorHandler = new Colosoft.Caching.CacheErrorEventHandler(
                (sender, e) =>
                {
                    // Enfileira o erro ocorrido no processamento
                    loadProcessingErrors.Enqueue(e);
                });

            // Registra os evento para monitor a o cache
            dataCacheManager.Loaded += controlHandler;
            dataCacheManager.LoadError += loadErrorHandler;
            dataCacheManager.LoadProcessingError += loadProcessingErrorHandler;

            // Configura o cache do servidor
            dataCacheManager.ConfigureServerCache();

            try
            {
                // Verifica se o cache ainda não foi carregado
                if (!dataCacheManager.Cache.IsLoaded)
                {
                    //resetEvent = new System.Threading.ManualResetEvent(false);
                    // Espera o cache se carregado
                    //while (!dataCacheManager.Cache.IsLoaded && !resetEvent.WaitOne(50)) ;
                }
            }
            finally
            {
                dataCacheManager.Loaded -= controlHandler;
                dataCacheManager.LoadError -= loadErrorHandler;
                dataCacheManager.LoadProcessingError += loadProcessingErrorHandler;
            }

            if (loadException != null)
                throw new Exception("Ocorreu um erro ao carregar o cache.", loadException);
        
            // Verifica se existe erros no processamento do cache
            if (loadProcessingErrors.Count > 0)
            {
                throw new AggregateException("Ocorreram erro no processamento da carga do cache", loadProcessingErrors.Select(f => f.Error));
            }

            // Verifica se nada foi carregado 
            if (dataCacheManager.Cache.Count == 0)
            {
                //"Nada foi carregado para o cache!!!"
            }

            var dataSourceSelector = Container.GetExportedValue<Colosoft.DataAccess.IQueryDataSourceSelector>();
            dataSourceSelector.Reset();
        }

        /// <summary>
        /// Método acionado quando for emitida alguma mensagem de Debug do GDA.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void GDAOperations_DebugTrace(object sender, string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Executa o bootstrapper.
        /// </summary>
        public override void Run()
        {
            base.Run();

            Glass.Armazenamento.ArmazenamentoIsolado.Configure(HttpContext.Current.Server.MapPath("~"));

            // Marca que é para ignorar a execução da sessões vazias
            Colosoft.Data.PersistenceSession.IgnoreAllEmptyActions = true;

            Colosoft.Security.Tokens.TokenGetters.Add(new Colosoft.Web.Security.AccessControl.AccessControlTokenGetter());

            // Configura o local de armazenamento os values
            Colosoft.Runtime.RuntimeValueStorage.Instance = new Colosoft.Web.HttpSessionValueStorage();

            Glass.Negocios.ProvedorControleAlteracao.Configurar();

            GDA.GDASettings.LoadConfiguration();
            GDA.GDASession.DefaultCommandTimeout = 60;
            GDA.GDAOperations.DebugTrace += GDAOperations_DebugTrace;
            ConfigureGDAListeners();

            //InitializeCache();

            Glass.Relatorios.UI.Web.GlassReportViewer
                .AddFullTrustModuleInSandboxAppDomain(typeof(Glass.Data.Helper.Config).Assembly);

            Colosoft.WebControls.VirtualObjectDataSource.ConfigureDefaultFactoryType
                ("Glass.UI.Web.Process.ServiceLocatorAccessor", "GetInstance", null);

            Container.GetExportedValue<Global.UI.Web.Process.Mensagens.LeituraMensagens>();
            Container.GetExportedValue<Global.Negocios.IMensagemFluxo>();
            Container.GetExportedValue<Global.Negocios.IMenuFluxo>();
            Container.GetExportedValue<Colosoft.Data.Caching.IDataCacheManager>();
            Container.GetExportedValue<Colosoft.Query.IQueryDataSource>();
            Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>();
            Container.GetExportedValue<Colosoft.Query.IRecordKeyFactory>();
            Container.GetExportedValue<Colosoft.Logging.ILogger>();
            Container.GetExportedValue<Colosoft.DataAccess.IQueryDataSourceSelector>();
        }

        #endregion
    }
}
