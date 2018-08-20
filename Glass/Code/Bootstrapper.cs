using System;
using System.Data;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Glass.UI.Web
{
    /// <summary>
    /// Summary description for Bootstrapper
    /// </summary>
    public class Bootstrapper : Glass.UI.Web.Process.BaseBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.ComposeExportedValue<Glass.Otimizacao.IRepositorioSolucaoOtimizacao>(
                new Glass.Otimizacao.RepositorioSolucaoOtimizacao(HttpContext.Current.Server.MapPath("~/Upload/Otimizacoes")));   
        }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

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
                .Where(f => f.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) &&
                            (f.IndexOf("Relatorios", StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                             f.IndexOf("UI", StringComparison.InvariantCultureIgnoreCase) >= 0)))
            {
                var assembly = System.Reflection.Assembly.Load(System.IO.Path.GetFileNameWithoutExtension(i));

                if (assembly != null)
                    this.AggregateCatalog.Catalogs.Add(new System.ComponentModel.Composition.Hosting.AssemblyCatalog(assembly));
            }

            AggregateCatalog.Catalogs.Add(new MefContrib.Hosting.Conventions.ConventionCatalog(
                new Colosoft.Mef.PartConventionBuilder()
                    .Add<Glass.Negocios.Componentes.Seguranca.ProvedorToken, Colosoft.Security.ITokenProvider>()
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

        protected override Colosoft.Data.IPersistenceContext CreateDatabasePersistenceContext
            (Colosoft.Data.Schema.ITypeSchema typeSchema)
        {
            return new Colosoft.Data.Database.MySql.MySqlPersistenceContext
                (Microsoft.Practices.ServiceLocation.ServiceLocator.Current, typeSchema);
        }

        protected override void ConfigureGDAListeners()
        {
            GDA.GDAConnectionManager.Listeners.Add(new Glass.Dados.MySql.MySqlConnectionListener());
        }
    }
}
