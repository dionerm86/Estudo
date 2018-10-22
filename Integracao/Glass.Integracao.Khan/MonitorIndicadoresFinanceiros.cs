// <copyright file="MonitorIndicadoresFinanceiros.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa o monitor dos indicadores financeiros.
    /// </summary>
    internal sealed class MonitorIndicadoresFinanceiros : IDisposable
    {
        private readonly ConfiguracaoKhan configuracao;
        private readonly Colosoft.Logging.ILogger logger;
        private readonly Rentabilidade.Negocios.IRentabilidadeFluxo rentabilidadeFluxo;
        private readonly string serviceUid = Guid.NewGuid().ToString();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorIndicadoresFinanceiros"/>.
        /// </summary>
        /// <param name="configuracao">Configuração.</param>
        /// <param name="logger">Logger que deve ser usado.</param>
        /// <param name="rentabilidadeFluxo">Fluxo de negócio da rentabilidade.</param>
        public MonitorIndicadoresFinanceiros(
            ConfiguracaoKhan configuracao,
            Colosoft.Logging.ILogger logger,
            Rentabilidade.Negocios.IRentabilidadeFluxo rentabilidadeFluxo)
        {
            this.configuracao = configuracao;
            this.logger = logger;
            this.rentabilidadeFluxo = rentabilidadeFluxo;
            Colosoft.Net.ServiceClientsManager.Current.Register(this.serviceUid, this.CriarCliente);
        }

        /// <summary>
        /// Finaliza uma instância da classe <see cref="MonitorIndicadoresFinanceiros"/>.
        /// </summary>
        ~MonitorIndicadoresFinanceiros()
        {
            this.Dispose();
        }

        private KhanIndicadoresServiceReference.IndicadoresServiceClient Client =>
            Colosoft.Net.ServiceClientsManager.Current.Get<KhanIndicadoresServiceReference.IndicadoresServiceClient>(this.serviceUid);

        private System.ServiceModel.ICommunicationObject CriarCliente()
        {
            var serviceAddress = Colosoft.Net.ServicesConfiguration.Current[IntegradorKhan.NomeIndicadoresFinanceirosService];
            var client = new KhanIndicadoresServiceReference.IndicadoresServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
            client.Endpoint.EndpointBehaviors.Add(new Seguranca.KhanEndpointBehavior(this.configuracao));
            return client;
        }

        /// <summary>
        /// Reaaliza a importação dos indicadores financeiros.
        /// </summary>
        public void ImportarIndicadores()
        {
            var indicadores = this.Client.ConsultarIndicadores(null, null);

            var atualizacoes = indicadores.Select(f => new Rentabilidade.Negocios.AtualizacaoIndicadorFinanceiro
            {
                Nome = f.CODMOE,
                Valor = f.VALOR,
                Data = f.DATA,
            });

            this.rentabilidadeFluxo.AtualizarIndicadores(atualizacoes);
        }

        /// <summary>
        /// Libera a instância.
        /// </summary>
        public void Dispose()
        {
            Colosoft.Net.ServiceClientsManager.Current.Remove(this.serviceUid);
            GC.SuppressFinalize(this);
        }
    }
}
