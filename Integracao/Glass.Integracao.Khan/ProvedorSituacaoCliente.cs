// <copyright file="ProvedorSituacaoCliente.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa o provedor de situação do cliente que acessa
    /// a base da Khan.
    /// </summary>
    public class ProvedorSituacaoCliente : Data.IProvedorSituacaoCliente
    {
        private readonly IIntegradorKhan integrador;
        private readonly string serviceUid = Guid.NewGuid().ToString();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ProvedorSituacaoCliente"/>.
        /// </summary>
        /// <param name="integrador">Instância do integrador que será usada.</param>
        public ProvedorSituacaoCliente(IIntegradorKhan integrador)
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
        public bool VerificarBloqueio(GDA.GDASession sessao, Data.Model.Cliente cliente, out IEnumerable<string> motivos)
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

            var parceiro = parceiros.FirstOrDefault();

            if (parceiro != null && parceiro.BLOQUEAR)
            {
                if (parceiro.AVISAR)
                {
                    motivos = new string[] { parceiro.SITMEN };
                }
                else
                {
                    motivos = new string[] { parceiro.Nomsit };
                }

                return true;
            }

            motivos = new string[0];
            return false;
        }
    }
}
