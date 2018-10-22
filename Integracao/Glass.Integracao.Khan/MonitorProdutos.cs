﻿// <copyright file="MonitorProdutos.cs" company="Sync Softwares">
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
    /// Classe responsável por monitora as alterações dos produtos.
    /// </summary>
    internal sealed class MonitorProdutos : MonitorEventosEntitidade<Global.Negocios.Entidades.Produto>
    {
        private readonly ConfiguracaoKhan configuracao;
        private readonly string serviceUid = Guid.NewGuid().ToString();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorProdutos"/>.
        /// </summary>
        /// <param name="domainEvents">Eventos de domínio.</param>
        /// <param name="configuracao">Configuração.</param>
        public MonitorProdutos(Colosoft.Domain.IDomainEvents domainEvents, ConfiguracaoKhan configuracao)
            : base(domainEvents)
        {
            this.configuracao = configuracao;
            Colosoft.Net.ServiceClientsManager.Current.Register(this.serviceUid, this.CriarCliente);
        }

        private KhanProdutosServiceReference.ProdutosServiceClient Client =>
            Colosoft.Net.ServiceClientsManager.Current.Get<KhanProdutosServiceReference.ProdutosServiceClient>(this.serviceUid);

        /// <summary>
        /// Converte os dados da entidade do produto para o produto da khan.
        /// </summary>
        /// <param name="produto">Produto que será convertido.</param>
        /// <returns>Produto convertido.</returns>
        private static KhanProdutosServiceReference.Produtos Converter(Global.Negocios.Entidades.Produto produto)
        {
            return new KhanProdutosServiceReference.Produtos
            {
                CodPRO = produto.CodInterno,
                NomPRO = produto.Descricao,
                UniPRO = produto.UnidadeMedida?.Codigo,
                CodFAM = produto.GrupoProd?.Descricao,
                Flagman = false,
                PrCust = produto.CustoCompra,
                Lucro1 = 0,
                Lucro2 = 0,
                PrVend1 = produto.ValorBalcao,
                PrVend2 = produto.ValorAtacado,
                PPerda = 0,
                PrContab = produto.ValorFiscal,
                EstoqMAX = 0,
                EstoqMIN = 0,
                CodCAD = produto.IdProd,
                CodFAB = produto.Fornecedor?.CpfCnpj,
                Peso = (decimal)produto.Peso,
                PICMS = 0,
                PREDICMS = 0,
                PIPI = (decimal)produto.AliqIPI,
                LETCLASS = string.Empty,
                CODCLASS = string.Empty,
                SITTRIB = string.Empty,
                ESPECIF = produto.Descricao,
                CODCATEG = string.Empty,
                CODMOE = "REAL",
                NOCPRO = string.Empty,
                PPIS = 0,
                PIRRF = 0,
                PSEGS = 0,
                CODEMPRESA = null,
                TIPOPROD = produto.Subgrupo?.Descricao,
                CODTIPPRO = produto.IdSubgrupoProd.ToString(),
                PCUSTOFIXO = 0,
                PRCUSTVAR = 0,
                CODBAR = null,
                MARCA = null,
                PRPROMO = 0,
                LUCROPROMO = 0,
                PRCUSTREFER = 0,
                PRREFER = 0,
                PDESCCOM = 0,
            };
        }

        private System.ServiceModel.ICommunicationObject CriarCliente()
        {
            var serviceAddress = Colosoft.Net.ServicesConfiguration.Current[IntegradorKhan.NomeProdutosService];
            var client = new KhanProdutosServiceReference.ProdutosServiceClient(serviceAddress.GetBinding(), serviceAddress.GetEndpointAddress());
            client.Endpoint.EndpointBehaviors.Add(new Seguranca.KhanEndpointBehavior(this.configuracao));

            return client;
        }

        /// <summary>
        /// Método acionado quando um produto for atualizado.
        /// </summary>
        /// <param name="entidade">Produto atualizado.</param>
        protected override void EntidadeAtualizada(Global.Negocios.Entidades.Produto entidade)
        {
            // Não faz nada
            var produto = Converter(entidade);
            this.Client.SalvarProdutos(produto);
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