// <copyright file="ProvedorLimiteCredito.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa o provedor de limite de crédito que acessa a
    /// base da Khan.
    /// </summary>
    public class ProvedorLimiteCredito : Data.IProvedorLimiteCredito
    {
        private readonly IIntegradorKhan integrador;
        private readonly string serviceUid = Guid.NewGuid().ToString();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ProvedorLimiteCredito"/>.
        /// </summary>
        /// <param name="integrador">Instância do integrado que será usada.</param>
        public ProvedorLimiteCredito(IIntegradorKhan integrador)
        {
            this.integrador = integrador;

            Colosoft.Net.ServiceClientsManager.Current.Register(this.serviceUid, this.CriarCliente);
        }

        private ConfiguracaoKhan Configuracao => (ConfiguracaoKhan)this.integrador.Configuracao;

        private KhanParceirosServiceReference.ParceirosServiceClient ParceirosCliente =>
            Colosoft.Net.ServiceClientsManager.Current.Get<KhanParceirosServiceReference.ParceirosServiceClient>(this.serviceUid);

        /// <inheritdoc />
        public bool Ativo => this.integrador.Ativo;

        private System.ServiceModel.ICommunicationObject CriarCliente()
        {
            var serviceAddress = Colosoft.Net.ServicesConfiguration.Current[IntegradorKhan.NomeParceirosService];
            var client = new KhanParceirosServiceReference.ParceirosServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
            client.Endpoint.EndpointBehaviors.Add(new Seguranca.KhanEndpointBehavior(this.Configuracao));

            return client;
        }

        /// <inheritdoc />
        public decimal ObterLimite(GDASession sessao, int idCliente)
        {
            var cliente = Data.DAL.ClienteDAO.Instance.GetElement(sessao, (uint)idCliente);
            return this.ObterLimite(sessao, cliente);
        }

        /// <inheritdoc />
        public decimal ObterLimite(GDASession sessao, Cliente cliente)
        {
            var args = new List<string> { cliente.CpfCnpj };
            IEnumerable<KhanParceirosServiceReference.Parceiros> parceiros;

            try
            {
                parceiros = this.ParceirosCliente.ListaParceiros(args);
            }
            catch (Exception ex)
            {
                throw new KhanException("Não foi possível consultar o limite de crédito do cliente na Khan.", ex);
            }

            return parceiros.FirstOrDefault()?.LimCred ?? 0m;
        }
    }
}
