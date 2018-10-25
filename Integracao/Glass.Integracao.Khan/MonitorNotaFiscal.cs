// <copyright file="MonitorNotaFiscal.cs" company="Sync Softwares">
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
    /// Representa o monitor das notas fiscais.
    /// </summary>
    internal class MonitorNotaFiscal : MonitorEventos
    {
        private readonly ConfiguracaoKhan configuracao;
        private readonly string serviceUid = Guid.NewGuid().ToString();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorNotaFiscal"/>.
        /// </summary>
        /// <param name="domainEvents">Eventos de domínio.</param>
        /// <param name="configuracao">Configuração.</param>
        public MonitorNotaFiscal(Colosoft.Domain.IDomainEvents domainEvents, ConfiguracaoKhan configuracao)
            : base(domainEvents)
        {
            this.configuracao = configuracao;
            this.AdicionarToken<Data.Domain.NotaFiscalGerada>(
                domainEvents.GetEvent<Data.Domain.NotaFiscalGerada>().Subscribe(this.NotaFiscalGerada));

            Colosoft.Net.ServiceClientsManager.Current.Register(this.serviceUid, this.CriarCliente);
        }

        private KhanPedidoServiceReference.PedidoServiceClient Client =>
           Colosoft.Net.ServiceClientsManager.Current.Get<KhanPedidoServiceReference.PedidoServiceClient>(this.serviceUid);

        private System.ServiceModel.ICommunicationObject CriarCliente()
        {
            var serviceAddress = Colosoft.Net.ServicesConfiguration.Current[IntegradorKhan.NomeProdutosService];
            var client = new KhanPedidoServiceReference.PedidoServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
            client.Endpoint.EndpointBehaviors.Add(new Seguranca.KhanEndpointBehavior(this.configuracao));

            return client;
        }

        private void NotaFiscalGerada(Data.Domain.NotaFiscalEventoArgs e)
        {
            // Deve ser feita a chamada do WebService para salvar os dados do pedido
            System.Diagnostics.Debug.WriteLine($"Integrando pedido");
        }

        /// <summary>
        /// Libera a instância.
        /// </summary>
        /// <param name="disposing">Identifica se a instância está sendo liberada.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Colosoft.Net.ServiceClientsManager.Current.Remove(this.serviceUid);
        }
    }
}
