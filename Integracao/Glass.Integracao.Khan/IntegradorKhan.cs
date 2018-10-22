// <copyright file="IntegradorKhan.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa o integrador da Khan.
    /// </summary>
    public sealed class IntegradorKhan : IIntegrador
    {
        /// <summary>
        /// Nome do serviço de produtos.
        /// </summary>
        internal const string NomeProdutosService = "KhanProdutosService";

        /// <summary>
        /// Nome do serviço de indicadores financeiros.
        /// </summary>
        internal const string NomeIndicadoresFinanceirosService = "KhanIndicadoresFinanceirosService";

        /// <summary>
        /// Nome do serviço dos pedidos.
        /// </summary>
        internal const string NomePedidosService = "KhanPedidosService";

        private readonly Colosoft.Domain.IDomainEvents domainEvents;
        private readonly List<IDisposable> monitores = new List<IDisposable>();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="IntegradorKhan"/>.
        /// </summary>
        /// <param name="domainEvents">Relação dos eventos de domínio.</param>
        /// <param name="rentabilidadeFluxo">Fluxo de negócio da rentabilidade.</param>
        public IntegradorKhan(
            Colosoft.Domain.IDomainEvents domainEvents,
            Glass.Rentabilidade.Negocios.IRentabilidadeFluxo rentabilidadeFluxo)
        {
            this.Logger = new Colosoft.Logging.DebugLogger();
            this.domainEvents = domainEvents;
            this.MonitorIndicadoresFinanceiros = new MonitorIndicadoresFinanceiros(this.Configuracao, this.Logger, rentabilidadeFluxo);
        }

        /// <summary>
        /// Finaliza uma instância da classe <see cref="IntegradorKhan"/>.
        /// </summary>
        ~IntegradorKhan()
        {
            this.Dispose();
        }

        /// <summary>
        /// Obtém o nome do integrador.
        /// </summary>
        public string Nome => "Khan";

        /// <summary>
        /// Obtém a configuração do integrador.
        /// </summary>
        internal ConfiguracaoKhan Configuracao { get; } = new ConfiguracaoKhan();

        /// <summary>
        /// Obtém a configuração do integrador.
        /// </summary>
        ConfiguracaoIntegrador IIntegrador.Configuracao => this.Configuracao;

        /// <summary>
        /// Obtém um valor que indica se o integrador está ativo.
        /// </summary>
        public bool Ativo
        {
            get
            {
                var config = System.Configuration.ConfigurationManager.AppSettings["Khan:Integracao"];
                var resultado = false;
                if (bool.TryParse(config, out resultado))
                {
                    return resultado;
                }

                return false;
            }
        }

        /// <summary>
        /// Obtém o monitor dos indicadores financeiros.
        /// </summary>
        internal MonitorIndicadoresFinanceiros MonitorIndicadoresFinanceiros { get; }

        /// <summary>
        /// Obtém o logger do integrador.
        /// </summary>
        public Colosoft.Logging.ILogger Logger { get; }

        private void ConfigurarMonitor<T>()
            where T : MonitorEventos
        {
            var monitor = (T)Activator.CreateInstance(typeof(T), this.domainEvents, this.Configuracao);
            this.monitores.Add(monitor);
        }

        private Colosoft.Net.ServiceAddress ObterEnderecoServico(string nome, string endereco)
        {
            var bindingParameters = new Colosoft.Net.ServiceAddressParameterCollection();
            bindingParameters.Add("name", $"WSHttpBinding_I{nome}");
            bindingParameters.Add("closeTimeout", "00:00:08");
            bindingParameters.Add("openTimeout", "00:00:08");
            bindingParameters.Add("receiveTimeout", "00:00:20");
            bindingParameters.Add("sendTimeout", "00:00:08");
            bindingParameters.Add("bypassProxyOnLocal", "false");
            bindingParameters.Add("transactionFlow", "false");
            bindingParameters.Add("hostNameComparisonMode", "StrongWildcard");
            bindingParameters.Add("maxBufferPoolSize", "524288");
            bindingParameters.Add("maxReceivedMessageSize", "65536");
            bindingParameters.Add("messageEncoding", "Text");
            bindingParameters.Add("textEncoding", "utf-8");
            bindingParameters.Add("useDefaultWebProxy", "true");
            bindingParameters.Add("allowCookies", "false");

            var readerQuotasParameters = new Colosoft.Net.ServiceAddressParameterCollection();
            readerQuotasParameters.Add("maxDepth", "32");
            readerQuotasParameters.Add("maxStringContentLength", "8192");
            readerQuotasParameters.Add("maxArrayLength", "16384");
            readerQuotasParameters.Add("maxBytesPerRead", "4096");
            readerQuotasParameters.Add("maxNameTableCharCount", "16384");

            var readerQuotaNode = new Colosoft.Net.ServiceAddressNode("readerQuotas", readerQuotasParameters);

            var reliableSessionParameters = new Colosoft.Net.ServiceAddressParameterCollection();
            reliableSessionParameters.Add("ordered", "true");
            reliableSessionParameters.Add("inactivityTimeout", "00:10:00");
            reliableSessionParameters.Add("enabled", "false");

            var reliableSessionNode = new Colosoft.Net.ServiceAddressNode("reliableSession", reliableSessionParameters);

            var securityParameters = new Colosoft.Net.ServiceAddressParameterCollection();
            securityParameters.Add("mode", "None");

            var transportParameters = new Colosoft.Net.ServiceAddressParameterCollection();
            transportParameters.Add("clientCredentialType", "Certificate");
            transportParameters.Add("proxyCredentialType", "None");
            transportParameters.Add("realm", string.Empty);

            var transportNode = new Colosoft.Net.ServiceAddressNode("transport", transportParameters);

            var messageParameters = new Colosoft.Net.ServiceAddressParameterCollection();
            messageParameters.Add("clientCredentialType", "UserName");
            messageParameters.Add("negotiateServiceCredential", "true");
            messageParameters.Add("establishSecurityContext", "true");
            messageParameters.Add("algorithmSuite", "Default");

            var messageNode = new Colosoft.Net.ServiceAddressNode("message", messageParameters);
            var securityNode = new Colosoft.Net.ServiceAddressNode("security", securityParameters, transportNode, messageNode);

            var bindingNode = new Colosoft.Net.ServiceAddressNode("binding", bindingParameters, readerQuotaNode, reliableSessionNode, securityNode);

            var customParameters = new Colosoft.Net.ServiceAddressParameterCollection();
            customParameters.Add("userName", string.Empty);
            customParameters.Add("password", string.Empty);
            customParameters.Add("enabled", "false");

            var customNode = new Colosoft.Net.ServiceAddressNode("appDownloader", customParameters);

            return new Colosoft.Net.ServiceAddress(
                        nome,
                        endereco,
                        "khanBasicHttpBinding",
                        $"ServerHost.AppDownloaderService",
                        bindingNode,
                        null,
                        new[] { customNode });
        }

        private void ConfigurarWebServices()
        {
            Colosoft.Net.ServiceAddress.ConfigureBinding(
                "khanBasicHttpBinding",
                () => new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None)
                {
                    MaxReceivedMessageSize = int.MaxValue,
                });

            var serviceConfiguration = Colosoft.Net.ServicesConfiguration.Current;

            serviceConfiguration.Add(
                this.ObterEnderecoServico(
                    NomeProdutosService,
                    $"{this.Configuracao.EnderecoBase}ProdutosService.svc"));

            serviceConfiguration.Add(
                this.ObterEnderecoServico(
                    NomeIndicadoresFinanceirosService,
                    $"{this.Configuracao.EnderecoBase}IndicadoresService.svc"));

            serviceConfiguration.Add(
                this.ObterEnderecoServico(
                    NomePedidosService,
                    $"{this.Configuracao.EnderecoBase}PedidoService.svc"));
        }

        /// <summary>
        /// Realiza o setup do integrador.
        /// </summary>
        /// <returns>Tarefa.</returns>
        public Task Setup()
        {
            this.ConfigurarWebServices();
            this.ConfigurarMonitor<MonitorProdutos>();
            this.ConfigurarMonitor<MonitorNotaFiscal>();

            var schedulerRegistry = new IntegradorScheculerRegistry(this);
            FluentScheduler.JobManager.Initialize(schedulerRegistry);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Libera a instância.
        /// </summary>
        public void Dispose()
        {
            foreach (var monitor in this.monitores)
            {
                monitor.Dispose();
            }

            this.monitores.Clear();

            this.MonitorIndicadoresFinanceiros.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
