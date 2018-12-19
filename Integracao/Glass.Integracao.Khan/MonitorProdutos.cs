// <copyright file="MonitorProdutos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using Glass.Integracao.Historico;
using System;
using System.Linq;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Classe responsável por monitora as alterações dos produtos.
    /// </summary>
    internal sealed class MonitorProdutos : MonitorEventosEntitidade<Global.Negocios.Entidades.Produto>
    {
        private readonly Colosoft.Logging.ILogger logger;
        private readonly ConfiguracaoKhan configuracao;
        private readonly string serviceUid = Guid.NewGuid().ToString();
        private readonly IProvedorHistorico provedorHistorico;
        private readonly Global.Negocios.IProdutoFluxo produtoFluxo;
        private bool sincronizandoTodosProdutos;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorProdutos"/>.
        /// </summary>
        /// <param name="domainEvents">Eventos de domínio.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="configuracao">Configuração.</param>
        /// <param name="produtoFluxo">Fluxo de negócio dos produtos.</param>
        /// <param name="provedorHistorico">Provedor dos históricos.</param>
        public MonitorProdutos(
            Colosoft.Domain.IDomainEvents domainEvents,
            Colosoft.Logging.ILogger logger,
            ConfiguracaoKhan configuracao,
            Global.Negocios.IProdutoFluxo produtoFluxo,
            Historico.IProvedorHistorico provedorHistorico)
            : base(domainEvents)
        {
            this.logger = logger;
            this.configuracao = configuracao;
            this.produtoFluxo = produtoFluxo;
            this.provedorHistorico = provedorHistorico;
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
            if (!this.configuracao.Ativo)
            {
                return;
            }

            var produto = Converter(entidade);

            this.logger.Info($"Salvando produto '{entidade.CodInterno}' na Khan...".GetFormatter());

            try
            {
                this.Client.SalvarProdutos(produto);
            }
            catch (Exception ex)
            {
                var mensagem = $"Não foi possível salvar os dados do produto '{entidade.CodInterno}' na Khan.";
                this.logger.Error(mensagem.GetFormatter(), ex);
                this.provedorHistorico.RegistrarFalha(HistoricoKhan.Produtos, entidade, mensagem, ex);
                throw;
            }

            this.provedorHistorico.NotificarIntegrado(HistoricoKhan.Produtos, entidade);
        }

        /// <summary>
        /// Configura as operações no gerenciador informado.
        /// </summary>
        /// <param name="gerenciadorOperacoes">Gerenciador onde as operações serão configuradas.</param>
        internal void ConfigurarOperacoes(GerenciadorOperacaoIntegracao gerenciadorOperacoes)
        {
            gerenciadorOperacoes.Adicionar(
                new OperacaoIntegracao(
                    "SincronizarProduto",
                    "Sincroniza os dados do produto com o ERP da KHAN",
                    new ParametroOperacaoIntegracao(
                        "idProd",
                        typeof(int),
                        "Identificador do produto",
                        0)),
                new Action<int>(this.SincronizarProduto));
        }

        /// <summary>
        /// Atualiza o dados do produto no ERP da Khan.
        /// </summary>
        /// <param name="idProd">Identificador do produto que será sincronizado.</param>
        internal void SincronizarProduto(int idProd)
        {
            var produto = this.produtoFluxo.ObtemProduto(idProd);

            if (produto == null)
            {
                throw new IntegracaoException($"Não foi encontrado nenhum produto com o identificador {idProd}.");
            }

            this.EntidadeAtualizada(produto);
        }

        /// <summary>
        /// Sincroniza todos os produtos do sistema.
        /// </summary>
        internal void SincronizarTodosProdutos()
        {
            if (!this.sincronizandoTodosProdutos)
            {
                this.sincronizandoTodosProdutos = true;

                try
                {
                    var total = this.produtoFluxo.ObterQuantidadeTodosProduto();
                    var processado = 0;
                    var progresso = 0;
                    var produtos = this.produtoFluxo.ObterTodosProdutos();

                    foreach (var produto in produtos)
                    {
                        var integrado = this.provedorHistorico.VerificarItensIntegrados(HistoricoKhan.Produtos, new[] { new object[] { produto.IdProd } }).First();

                        if (!integrado)
                        {
                            this.EntidadeAtualizada(produto);
                        }

                        processado++;
                        var progresso1 = (int)((processado / (double)total) * 100);

                        if (progresso != progresso1 || (processado % 10) == 0)
                        {
                            progresso = progresso1;
                            this.logger.Info($"######## Sincronização produtos ({processado}/{total}) {progresso}%... ########".GetFormatter());
                        }
                    }
                }
                finally
                {
                    this.sincronizandoTodosProdutos = false;
                }
            }
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
