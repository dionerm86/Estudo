using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Glass.Api.Host
{
    /// <summary>
    /// Bootstrapper da API
    /// </summary>
    public class Bootstrapper : Colosoft.Mef.MefBootstrapper
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
                    .AddImportingConstructor<Colosoft.Business.BusinessEntityTypeManager, Colosoft.Business.IEntityTypeManager>()
                    .AddImportingConstructor<Colosoft.Business.EntityManager, Colosoft.Business.IEntityManager>()
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
                        Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>(), new DataEntryDownloader(),
                        Container.GetExportedValue<Colosoft.Data.Caching.IDataEntriesRepository>(),
                        Container.GetExportedValue<Colosoft.Data.Caching.ICacheLoaderObserver>(),
                        Container.GetExportedValue<Colosoft.Logging.ILogger>());

                   return manager;

               }))

               .Add<Colosoft.Data.IPersistenceContext>(new Lazy<Colosoft.Data.IPersistenceContext>(() =>
               {
                   var typeSchema = Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>();
                   return CreateDatabasePersistenceContext(typeSchema);
               }))

               .Add<Colosoft.Caching.CacheDataSource>(new Lazy<Colosoft.Caching.CacheDataSource>(() =>
               {
                   var cacheManager = Container.GetExportedValue<Colosoft.Data.Caching.IDataCacheManager>();
                   var typeSchema = Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>();
                   return new Colosoft.Caching.CacheDataSource(cacheManager, typeSchema);
               }))
           );

            using (var textReader = new System.IO.StreamReader(HttpContext.Current.Server.MapPath("~/Mef.config")))
            {
                var reader = System.Xml.XmlReader.Create(textReader, new System.Xml.XmlReaderSettings
                {
                    IgnoreWhitespace = true
                });
                reader.Read();
                var configurableCatalog =
                    new MefContrib.Models.Provider.DefinitionProviderPartCatalog
                        <MefContrib.Models.Provider.Definitions.Configurable.ConfigurableDefinitionProvider>(reader);

                this.AggregateCatalog.Catalogs.Add(configurableCatalog);
            }

            // Recupera o diretório onde o site está em execução
            var directory = HttpContext.Current.Server.MapPath("~/bin");

            // Localiza os assemblies
            foreach (var i in System.IO.Directory.GetFiles(directory)
                .Where(f => f.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)))
            {
                var assembly = System.Reflection.Assembly.Load(System.IO.Path.GetFileNameWithoutExtension(i));

                if (assembly != null)
                    this.AggregateCatalog.Catalogs.Add(new System.ComponentModel.Composition.Hosting.AssemblyCatalog(assembly));
            }

            AggregateCatalog.Catalogs.Add(new MefContrib.Hosting.Conventions.ConventionCatalog(
                new Colosoft.Mef.PartConventionBuilder()
                    .AddImportingConstructor<Colosoft.Query.Database.MySql.MySqlDataSource, Colosoft.Query.Database.SqlQueryDataSource>()
                    .AddImportingConstructor<Colosoft.Query.Database.MySql.MySqlDataSource, Colosoft.Query.IQueryDataSource>()
                    .Add<Colosoft.Data.Database.MySql.MySqlPrimaryKeyRepository, Colosoft.Data.Database.MySql.IMySqlPrimaryKeyRepository>()
                ));

            AggregateCatalog.Catalogs.Add(new Colosoft.Mef.InstanceCatalog()

               .Add<Colosoft.Data.Schema.ITypeSchema>(new Lazy<Colosoft.Data.Schema.ITypeSchema>(() =>
               {
                   // Carrega o esquema dos metadados do GDA
                   return Colosoft.Data.Schema.GDA.Metadata.MetadataTypeSchema.Load(typeof(Glass.Data.Model.Pedido).Assembly);
               })));
        }

        /// <summary>
        /// Cria o contexto de persistencia do banco de dados.
        /// </summary>
        /// <param name="typeSchema"></param>
        /// <returns></returns>
        protected Colosoft.Data.IPersistenceContext CreateDatabasePersistenceContext(Colosoft.Data.Schema.ITypeSchema typeSchema)
        {
            return new Colosoft.Data.Database.MySql.MySqlPersistenceContext
                (Microsoft.Practices.ServiceLocation.ServiceLocator.Current, typeSchema);
        }

        /// <summary>
        /// Configura o container.
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

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

            Colosoft.Query.RecordKeyFactory.Instance = Container.GetExportedValue<Colosoft.Query.IRecordKeyFactory>();
        }

        /// <summary>
        /// Método acionado para configurar os listeners do GDA.
        /// </summary>
        protected virtual void ConfigureGDAListeners()
        {
            GDA.GDAConnectionManager.Listeners.Add(new Glass.Dados.MySql.MySqlConnectionListener());
        }

        #endregion

        #region Métodos Privados

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

            // Marca que é para ignorar a execução da sessões vazias
            Colosoft.Data.PersistenceSession.IgnoreAllEmptyActions = true;

            Colosoft.Security.Tokens.TokenGetters.Add(new Colosoft.Web.Security.AccessControl.AccessControlTokenGetter());

            // Configura o local de armazenamento os values
            Colosoft.Runtime.RuntimeValueStorage.Instance = new Colosoft.Web.HttpSessionValueStorage();

            GDA.GDASettings.LoadConfiguration();
            GDA.GDASession.DefaultCommandTimeout = 60;
            GDA.GDAOperations.DebugTrace += GDAOperations_DebugTrace;
            ConfigureGDAListeners();

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