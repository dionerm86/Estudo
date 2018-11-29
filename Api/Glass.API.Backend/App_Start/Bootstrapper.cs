// <copyright file="Bootstrapper.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Glass.API.Backend
{
    /// <summary>
    /// Bootstrapper da API.
    /// </summary>
    public sealed class Bootstrapper : Colosoft.Mef.MefBootstrapper, IDisposable, System.Web.Hosting.IRegisteredObject
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Bootstrapper"/>.
        /// </summary>
        public Bootstrapper()
        {
            System.Web.Hosting.HostingEnvironment.RegisterObject(this);
        }

        /// <summary>
        /// Finaliza uma instância da classe <see cref="Bootstrapper"/>.
        /// </summary>
        ~Bootstrapper()
        {
            this.Dispose();
        }

        /// <summary>
        /// Obtém o domínio de eventos.
        /// </summary>
        public override Colosoft.Domain.IDomainEvents DomainEvents => Colosoft.Domain.DomainEvents.Instance;

        /// <summary>
        /// Obtém o logger padrão.
        /// </summary>
        private Colosoft.Logging.ILogger Logger { get; } = new Colosoft.Logging.DebugLogger();

        private Integracao.GerenciadorIntegradores GerenciadorIntegradores { get; set; }

        private Integracao.GerenciadorIntegradores CarregarGerenciadorIntegradores()
        {
            if (this.GerenciadorIntegradores == null)
            {
                var provedorIntegradores = this.Container.GetExportedValue<Integracao.IProvedorIntegradores>();
                this.GerenciadorIntegradores = new Integracao.GerenciadorIntegradores(provedorIntegradores);
            }

            return this.GerenciadorIntegradores;
        }

        /// <summary>
        /// Configura o catalogo que será usado no sistema.
        /// </summary>
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            this.AggregateCatalog.Catalogs.Add(new MefContrib.Hosting.Conventions.ConventionCatalog(
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

                    .AddImportingConstructor<Integracao.ProvedorIntegradores, Integracao.IProvedorIntegradores>()));

            this.AggregateCatalog.Catalogs.Add(new Colosoft.Mef.InstanceCatalog()
               .Add<Colosoft.Data.Caching.IDataCacheManager>(new Lazy<Colosoft.Data.Caching.IDataCacheManager>(() =>
               {
                   var manager = new Colosoft.Data.Caching.DataCacheManager(
                        this.Container.GetExportedValue<Colosoft.Query.ISourceContext>(),
                        this.Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>(),
                        new DataEntryDownloader(),
                        this.Container.GetExportedValue<Colosoft.Data.Caching.IDataEntriesRepository>(),
                        this.Container.GetExportedValue<Colosoft.Data.Caching.ICacheLoaderObserver>(),
                        this.Container.GetExportedValue<Colosoft.Logging.ILogger>());

                   return manager;
               }))

               .Add<Integracao.GerenciadorIntegradores>(new Lazy<Integracao.GerenciadorIntegradores>(this.CarregarGerenciadorIntegradores))

               .Add<Colosoft.Data.IPersistenceContext>(new Lazy<Colosoft.Data.IPersistenceContext>(() =>
               {
                   var typeSchema = this.Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>();
                   return this.CreateDatabasePersistenceContext(typeSchema);
               }))

               .Add<Colosoft.Caching.CacheDataSource>(new Lazy<Colosoft.Caching.CacheDataSource>(() =>
               {
                   var cacheManager = this.Container.GetExportedValue<Colosoft.Data.Caching.IDataCacheManager>();
                   var typeSchema = this.Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>();
                   return new Colosoft.Caching.CacheDataSource(cacheManager, typeSchema);
               })));

            using (var textReader = new System.IO.StreamReader(HttpContext.Current.Server.MapPath("~/Mef.config")))
            {
                var reader = System.Xml.XmlReader.Create(textReader, new System.Xml.XmlReaderSettings
                {
                    IgnoreWhitespace = true,
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
                {
                    this.AggregateCatalog.Catalogs.Add(new System.ComponentModel.Composition.Hosting.AssemblyCatalog(assembly));
                }
            }

            this.AggregateCatalog.Catalogs.Add(new MefContrib.Hosting.Conventions.ConventionCatalog(
                new Colosoft.Mef.PartConventionBuilder()
                    .AddImportingConstructor<Colosoft.Query.Database.MySql.MySqlDataSource, Colosoft.Query.Database.SqlQueryDataSource>()
                    .AddImportingConstructor<Colosoft.Query.Database.MySql.MySqlDataSource, Colosoft.Query.IQueryDataSource>()
                    .Add<Colosoft.Data.Database.MySql.MySqlPrimaryKeyRepository, Colosoft.Data.Database.MySql.IMySqlPrimaryKeyRepository>()));

            this.AggregateCatalog.Catalogs.Add(new Colosoft.Mef.InstanceCatalog()

               .Add<Colosoft.Data.Schema.ITypeSchema>(new Lazy<Colosoft.Data.Schema.ITypeSchema>(() =>
               {
                   // Carrega o esquema dos metadados do GDA
                   return Colosoft.Data.Schema.GDA.Metadata.MetadataTypeSchema.Load(typeof(Glass.Data.Model.Pedido).Assembly);
               })));
        }

        /// <summary>
        /// Cria o contexto de persistencia do banco de dados.
        /// </summary>
        /// <param name="typeSchema">Esquema de tipos.</param>
        /// <returns>Contexto de persistencia.</returns>
        private Colosoft.Data.IPersistenceContext CreateDatabasePersistenceContext(Colosoft.Data.Schema.ITypeSchema typeSchema)
        {
            return new Colosoft.Data.Database.MySql.MySqlPersistenceContext(
                Microsoft.Practices.ServiceLocation.ServiceLocator.Current,
                typeSchema);
        }

        /// <summary>
        /// Configura o container.
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            this.Container.ComposeExportedValue<Colosoft.IServerData>(new ServerData());

            Colosoft.Business.EntityManager.SetEntityManager(
                new Lazy<Colosoft.Business.IEntityManager>(
                    () => this.Container.GetExportedValue<Colosoft.Business.IEntityManager>()));

            var aggregateCacheLoaderObserver = new Colosoft.Data.Caching.AggregateCacheLoaderObserver();

            this.Container.ComposeExportedValue<Colosoft.Domain.IDomainEvents>(this.DomainEvents);
            this.Container.ComposeExportedValue<Colosoft.Data.Caching.AggregateCacheLoaderObserver>(aggregateCacheLoaderObserver);
            this.Container.ComposeExportedValue<Colosoft.Data.Caching.ICacheLoaderObserver>(aggregateCacheLoaderObserver);

            this.Container.ComposeExportedValue<Colosoft.DataAccess.IQueryDataSourceSelector>(
                new Colosoft.DataAccess.QueryDataSourceSelector(
                    new Lazy<Colosoft.Data.Caching.IDataCacheManager>(() => this.Container.GetExportedValue<Colosoft.Data.Caching.IDataCacheManager>()),
                    new Lazy<Colosoft.Query.IQueryDataSource>(() => this.Container.GetExportedValue<Colosoft.Caching.CacheDataSource>()),
                    new Lazy<Colosoft.Query.IQueryDataSource>(() => this.Container.GetExportedValue<Colosoft.Query.IQueryDataSource>()),
                    new Lazy<Colosoft.Data.Schema.ITypeSchema>(() => this.Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>()),
                    new Lazy<Colosoft.Query.IRecordKeyFactory>(() => this.Container.GetExportedValue<Colosoft.Query.IRecordKeyFactory>()),
                    this.Container.GetExportedValue<Colosoft.Logging.ILogger>()));

            Colosoft.Query.RecordKeyFactory.Instance = this.Container.GetExportedValue<Colosoft.Query.IRecordKeyFactory>();
        }

        /// <summary>
        /// Método acionado para configurar os listeners do GDA.
        /// </summary>
        private void ConfigureGDAListeners()
        {
            GDA.GDAConnectionManager.Listeners.Add(new Glass.Dados.MySql.MySqlConnectionListener());
        }

        private void GDAOperations_DebugTrace(object sender, string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private string ObterDiretorioSistema()
        {
            var diretorioSistema = ConfigurationManager.AppSettings["diretorioSistema"];
            var diretorioAtual = HttpContext.Current.Server.MapPath("~");

            return Path.Combine(diretorioAtual, diretorioSistema);
        }

        private void InicializarIntegradores()
        {
            var task = Task.Run(async () => await this.InicializarIntegradoresAsync());

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any())
                {
                    throw ex.InnerExceptions.FirstOrDefault();
                }

                throw;
            }
        }

        private async Task InicializarIntegradoresAsync()
        {
            var inicializador = this.Container.GetExportedValue<Integracao.GerenciadorIntegradores>();

            await inicializador.Inicializar(this.Logger);
        }

        /// <summary>
        /// Executa o bootstrapper.
        /// </summary>
        public override void Run()
        {
            base.Run();

            Glass.Armazenamento.ArmazenamentoIsolado.Configure(this.ObterDiretorioSistema());

            // Marca que é para ignorar a execução da sessões vazias
            Colosoft.Data.PersistenceSession.IgnoreAllEmptyActions = true;

            Colosoft.Security.Tokens.TokenGetters.Add(new Colosoft.Web.Security.AccessControl.AccessControlTokenGetter());

            // Configura o local de armazenamento os values
            Colosoft.Runtime.RuntimeValueStorage.Instance = new Colosoft.Web.HttpSessionValueStorage();

            Glass.Negocios.ProvedorControleAlteracao.Configurar();

            GDA.GDASettings.LoadConfiguration();
            GDA.GDASession.DefaultCommandTimeout = 60;
            GDA.GDAOperations.DebugTrace += this.GDAOperations_DebugTrace;
            this.ConfigureGDAListeners();

            this.Container.GetExportedValue<Global.UI.Web.Process.Mensagens.LeituraMensagens>();
            this.Container.GetExportedValue<Global.Negocios.IMensagemFluxo>();
            this.Container.GetExportedValue<Global.Negocios.IMenuFluxo>();
            this.Container.GetExportedValue<Colosoft.Data.Caching.IDataCacheManager>();
            this.Container.GetExportedValue<Colosoft.Query.IQueryDataSource>();
            this.Container.GetExportedValue<Colosoft.Data.Schema.ITypeSchema>();
            this.Container.GetExportedValue<Colosoft.Query.IRecordKeyFactory>();
            this.Container.GetExportedValue<Colosoft.Logging.ILogger>();
            this.Container.GetExportedValue<Colosoft.DataAccess.IQueryDataSourceSelector>();

            this.InicializarIntegradores();

            Data.Helper.ConfiguracaoBiesse.Instancia.Inicializar(
                HttpContext.Current?.Server?.MapPath("~") ?? System.IO.Path.GetDirectoryName(typeof(Bootstrapper).Assembly.Location));
        }

        /// <inheritdoc />
        public void Stop(bool immediate)
        {
            System.Web.Hosting.HostingEnvironment.UnregisterObject(this);
            this.Dispose();
        }

        /// <summary>
        /// Libera a instância.
        /// </summary>
        public void Dispose()
        {
            this.GerenciadorIntegradores?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementação para a data do servidor.
        /// </summary>
        private class ServerData : Colosoft.IServerData
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
        private sealed class DataEntryDownloader : Colosoft.Data.Caching.IDataEntryDownloader
        {
            event Colosoft.Net.DownloadCompletedEventHandler Colosoft.Net.IDownloader.DownloadCompleted
            {
                add
                {
                    // Não faz nada
                }

                remove
                {
                    // Não faz nada
                }
            }

            event Colosoft.Net.DownloadProgressEventHandler Colosoft.Net.IDownloader.ProgressChanged
            {
                add
                {
                    // Não faz nada
                }

                remove
                {
                    // Não faz nada
                }
            }

            public bool IsBusy
            {
                get { return false; }
            }

            public void Add(Colosoft.Data.Caching.DataEntryVersion version)
            {
                // Ignora
            }

            public void AddRange(IEnumerable<Colosoft.Data.Caching.DataEntryVersion> versions)
            {
                // Ignora
            }

            public void Clear()
            {
                // Ignora
            }

            public void CancelAsync()
            {
                // Ignora
            }

            public void RunAsync(object userState)
            {
                // Ignora
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }
        }
    }
}
