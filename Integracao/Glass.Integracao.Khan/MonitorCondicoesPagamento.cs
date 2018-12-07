// <copyright file="MonitorCondicoesPagamento.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using System;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa o monitor das formas de pagamento.
    /// </summary>
    internal sealed class MonitorCondicoesPagamento : IDisposable
    {
        private readonly ConfiguracaoKhan configuracao;
        private readonly Colosoft.Logging.ILogger logger;
        private readonly string serviceUid = Guid.NewGuid().ToString();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorCondicoesPagamento"/>.
        /// </summary>
        /// <param name="configuracao">Configuração que será usada.</param>
        /// <param name="logger">Logger.</param>
        public MonitorCondicoesPagamento(
            ConfiguracaoKhan configuracao,
            Colosoft.Logging.ILogger logger)
        {
            this.configuracao = configuracao;
            this.logger = logger;
            Colosoft.Net.ServiceClientsManager.Current.Register(this.serviceUid, this.CriarCliente);
        }

        /// <summary>
        /// Finaliza uma instância da classe <see cref="MonitorCondicoesPagamento"/>.
        /// </summary>
        ~MonitorCondicoesPagamento()
        {
            this.Dispose();
        }

        private KhanPedidoServiceReference.PedidoServiceClient Client =>
            Colosoft.Net.ServiceClientsManager.Current.Get<KhanPedidoServiceReference.PedidoServiceClient>(this.serviceUid);

        private System.ServiceModel.ICommunicationObject CriarCliente()
        {
            var serviceAddress = Colosoft.Net.ServicesConfiguration.Current[IntegradorKhan.NomePedidosService];
            var client = new KhanPedidoServiceReference.PedidoServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
            client.Endpoint.EndpointBehaviors.Add(new Seguranca.KhanEndpointBehavior(this.configuracao));
            return client;
        }

        /// <summary>
        /// Importa as formas de pagamento.
        /// </summary>
        public void ImportarFormasPagamento()
        {
            this.logger.Info("Executando integração das formas de pagamento...".GetFormatter());

            if (this.Client != null)
            {
                // Ainda falta verificar com serão recuperadas os dados
                var condicoesPagamento = this.Client.ListarCondicoesPagto();

                foreach (var condicao in condicoesPagamento)
                {
                    System.Diagnostics.Debug.WriteLine($"{condicao.CODCPGTO}:{condicao.NOMCPGTO}");
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Colosoft.Net.ServiceClientsManager.Current.Remove(this.serviceUid);
            GC.SuppressFinalize(this);
        }
    }
}
