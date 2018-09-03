// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um pedido para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Pedido")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="pedido">A model de pedidos.</param>
        internal ListaDto(Data.Model.Pedido pedido)
        {
            this.Id = (int)pedido.IdPedido;
            this.IdProjeto = (int?)pedido.IdProjeto;
            this.IdOrcamento = (int?)pedido.IdOrcamento;
            this.CodigoPedidoCliente = pedido.CodCliente;
            this.Cliente = new IdNomeDto
            {
                Id = (int)pedido.IdCli,
                Nome = pedido.NomeCli,
            };

            this.Loja = new IdNomeDto
            {
                Id = (int)pedido.IdLoja,
                Nome = pedido.NomeLoja,
            };

            this.Vendedor = new IdNomeDto
            {
                Id = (int)pedido.IdFunc,
                Nome = pedido.NomeFunc,
            };

            this.DataEntrega = new DatasEntregaDto
            {
                Atual = pedido.DataEntrega,
                Original = pedido.DataEntregaOriginal,
            };

            this.Total = pedido.ExibirTotalEspelho
                ? pedido.TotalEspelho
                : pedido.Total;

            this.TipoVenda = pedido.DescrTipoVenda;
            this.DataPedido = pedido.DataPedido;
            this.Finalizacao = this.DataEFuncionarioOperacao(pedido.DataFin, pedido.NomeUsuFin);
            this.Confirmacao = this.DataEFuncionarioOperacao(pedido.DataConf, pedido.NomeUsuConf);
            this.DataPronto = pedido.DataPronto;
            this.Liberacao = !pedido.IdLiberarPedido.HasValue || !pedido.DataLiberacao.HasValue
                ? null
                : new LiberacaoDto
                {
                    Id = (int)pedido.IdLiberarPedido.Value,
                    Observacao = pedido.ObsLiberacao,
                    Data = pedido.DataLiberacao.Value,
                    Funcionario = pedido.NomeUsuLib,
                };

            this.Situacao = pedido.DescrSituacaoPedido;
            this.Producao = new ProducaoDto
            {
                Situacao = pedido.DescrSituacaoProducao,
                Pronto = pedido.SituacaoProducao == (int)Data.Model.Pedido.SituacaoProducaoEnum.Pronto,
                Pendente = pedido.SituacaoProducao == (int)Data.Model.Pedido.SituacaoProducaoEnum.Pendente,
            };

            this.Tipo = pedido.DescricaoTipoPedido;
            this.LiberadoFinanceiro = pedido.LiberadoFinanc;
            this.UsarControleReposicao = pedido.UsarControleReposicao;
            this.Permissoes = new PermissoesDto
            {
                Editar = pedido.EditVisible,
                ImprimirPcp = pedido.ExibirImpressaoPcp,
                Imprimir = pedido.ExibirRelatorio,
                ImprimirMemoriaCalculo = pedido.ExibirRelatorioCalculo,
                ImprimirNotaPromissoria = pedido.ExibirNotaPromissoria,
                ImprimirProjeto = pedido.ExibirImpressaoProjeto,
                TemAlteracaoPcp = pedido.TemAlteracaoPcp,
                Cancelar = pedido.CancelarVisible,
                Desconto = pedido.DescontoVisible,
                ImagemPeca = pedido.ExibirImagemPeca,
                ImpressaoItensLiberar = pedido.ExibirImpressaoItensLiberar,
                AlterarProcessoEAplicacao = pedido.AlterarProcessoAplicacaoVisible,
                Reabrir = pedido.ExibirReabrir,
                AnexosLiberacao = pedido.Situacao == Data.Model.Pedido.SituacaoPedido.Confirmado && PedidoConfig.LiberarPedido,
                FinalizacoesFinanceiro = pedido.ExibirFinalizacoesFinanceiro,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.Pedido, pedido.IdPedido, null),
                AlterarObservacoes = pedido.TipoVenda == 3 || pedido.TipoVenda == 4,
            };

            this.SinalEPagamentoAntecipado = new SinalEPagamentoAntecipadoDto
            {
                IdSinal = (int?)pedido.IdSinal,
                TemPagamentoAntecipado = pedido.PagamentoAntecipado,
                ValorSinal = pedido.ValorEntrada,
                ValorPagamentoAntecipado = pedido.ValorPagamentoAntecipado,
            };

            this.IdsOrdensDeCarga = pedido.IdsOCs == null
                ? new int[0]
                : pedido.IdsOCs.Split(',')
                    .Select(oc => oc.Trim().StrParaInt())
                    .Where(oc => oc > 0);

            this.Obs = pedido.Obs;
            this.ObsLiberacao = pedido.ObsLiberacao;
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do projeto, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("idProjeto")]
        public int? IdProjeto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do orçamento, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("idOrcamento")]
        public int? IdOrcamento { get; set; }

        /// <summary>
        /// Obtém ou define o código de pedido do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoPedidoCliente")]
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos da loja.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public IdNomeDto Loja { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos do vendedor.
        /// </summary>
        [DataMember]
        [JsonProperty("vendedor")]
        public IdNomeDto Vendedor { get; set; }

        /// <summary>
        /// Obtém ou define o total do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de venda do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVenda")]
        public string TipoVenda { get; set; }

        /// <summary>
        /// Obtém ou define a data de emissão do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataPedido")]
        public DateTime DataPedido { get; set; }

        /// <summary>
        /// Obtém ou define os dados de finalização do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("finalizacao")]
        public DataFuncionarioDto Finalizacao { get; set; }

        /// <summary>
        /// Obtém ou define os dados de confirmação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("confirmacao")]
        public DataFuncionarioDto Confirmacao { get; set; }

        /// <summary>
        /// Obtém ou define a data que o pedido ficou pronto.
        /// </summary>
        [DataMember]
        [JsonProperty("dataPronto")]
        public DateTime? DataPronto { get; set; }

        /// <summary>
        /// Obtém ou define as datas de entrega do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntrega")]
        public DatasEntregaDto DataEntrega { get; set; }

        /// <summary>
        /// Obtém ou define os dados de liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("liberacao")]
        public LiberacaoDto Liberacao { get; set; }

        /// <summary>
        /// Obtém ou define a situação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define os dados de produção do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("producao")]
        public ProducaoDto Producao { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido foi liberado pelo financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("liberadoFinanceiro")]
        public bool LiberadoFinanceiro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido usa o controle de reposição.
        /// </summary>
        [DataMember]
        [JsonProperty("usarControleReposicao")]
        public bool UsarControleReposicao { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas ao pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        /// <summary>
        /// Obtém ou define os dados de sinal e pagamento antecipado do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("sinalEPagamentoAntecipado")]
        public SinalEPagamentoAntecipadoDto SinalEPagamentoAntecipado { get; set; }

        /// <summary>
        /// Obtém ou define a lista de ordens de carga do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idsOrdensDeCarga")]
        public IEnumerable<int> IdsOrdensDeCarga { get; set; }

        /// <summary>
        /// Obtém ou define a observação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("obs")]
        public string Obs { get; set; }

        /// <summary>
        /// Obtém ou define a observação de Liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("obsLiberacao")]
        public string ObsLiberacao { get; set; }

        private DataFuncionarioDto DataEFuncionarioOperacao(DateTime? data, string funcionario)
        {
            return !data.HasValue && string.IsNullOrWhiteSpace(funcionario)
                ? null
                : new DataFuncionarioDto
                {
                    Data = data.Value,
                    Funcionario = funcionario,
                };
        }
    }
}
