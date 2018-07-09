// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.ProdutosPedido.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um produto de pedido.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="produtoPedido">A model de produto de pedido.</param>
        internal ListaDto(Data.Model.ProdutosPedido produtoPedido)
        {
            this.Id = (int)produtoPedido.IdProdPed;
            this.IdMaterialProjeto = (int?)produtoPedido.IdMaterItemProj;
            this.Produto = new ProdutoDto
            {
                Id = (int)produtoPedido.IdProd,
                Codigo = produtoPedido.CodInterno,
                Descricao = produtoPedido.DescrProduto,
                DescricaoComBeneficiamentos = produtoPedido.DescricaoProdutoComBenef,
                Espessura = produtoPedido.Espessura,
            };

            this.Quantidade = produtoPedido.Qtde;
            this.QuantidadeAmbiente = produtoPedido.QtdeAmbiente;
            this.DescontoPorQuantidade = new DescontoQuantidadeDto
            {
                Percentual = produtoPedido.PercDescontoQtde,
                Valor = produtoPedido.ValorDescontoQtde,
            };

            this.Largura = produtoPedido.Largura;
            this.Altura = new AlturaDto
            {
                Real = produtoPedido.AlturaReal,
                ParaCalculo = produtoPedido.Altura,
                ParaExibirNaLista = produtoPedido.AlturaLista,
            };

            this.AreaEmM2 = new AreaDto
            {
                Real = produtoPedido.TotM,
                ParaCalculo = produtoPedido.TotM2Calc,
                ParaCalculoSemChapa = produtoPedido.TotalM2CalcSemChapaString,
            };

            this.ValorUnitario = produtoPedido.ValorVendido;
            this.Processo = this.ObterIdCodigo((int?)produtoPedido.IdProcesso, produtoPedido.CodProcesso);
            this.Aplicacao = this.ObterIdCodigo((int?)produtoPedido.IdAplicacao, produtoPedido.CodAplicacao);
            this.CodigoPedidoCliente = produtoPedido.PedCli;
            this.Total = produtoPedido.Total;
            this.Beneficiamentos = new BeneficiamentosDto
            {
                Valor = produtoPedido.ValorBenef,
                Altura = produtoPedido.AlturaBenef,
                Largura = produtoPedido.LarguraBenef,
                Redondo = produtoPedido.Redondo,
                Itens = produtoPedido.Beneficiamentos?.ObterListaBeneficiamentos(),
            };

            this.Observacao = produtoPedido.Obs;
            this.PercentualComissao = (double)produtoPedido.PercComissao;
            this.Permissoes = new PermissoesDto
            {
                Editar = produtoPedido.PodeEditar,
                Excluir = produtoPedido.DeleteVisible,
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do material de projeto que gerou o produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idMaterialProjeto")]
        public int? IdMaterialProjeto { get; set; }

        /// <summary>
        /// Obtém ou define os dados de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public ProdutoDto Produto { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de produtos do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public double Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade do ambiente para o produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeAmbiente")]
        public int QuantidadeAmbiente { get; set; }

        /// <summary>
        /// Obtém ou define os dados de desconto por quantidade do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("descontoPorQuantidade")]
        public DescontoQuantidadeDto DescontoPorQuantidade { get; set; }

        /// <summary>
        /// Obtém ou define a largura pra o produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int Largura { get; set; }

        /// <summary>
        /// Obtém ou define os dados de altura para o produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public AlturaDto Altura { get; set; }

        /// <summary>
        /// Obtém ou define os dados de área para o produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("areaEmM2")]
        public AreaDto AreaEmM2 { get; set; }

        /// <summary>
        /// Obtém ou define o valor unitário para o produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("valorUnitario")]
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos para o processo do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("processo")]
        public IdCodigoDto Processo { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos para a aplicação do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("aplicacao")]
        public IdCodigoDto Aplicacao { get; set; }

        /// <summary>
        /// Obtém ou define o código do pedido do cliente para o produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoPedidoCliente")]
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o valor total do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define os beneficiamentos do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("beneficiamentos")]
        public BeneficiamentosDto Beneficiamentos { get; set; }

        /// <summary>
        /// Obtém ou define a observação do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de comissão do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualComissao")]
        public double PercentualComissao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private IdCodigoDto ObterIdCodigo(int? id, string codigo)
        {
            return !id.HasValue || string.IsNullOrWhiteSpace(codigo)
                ? null
                : new IdCodigoDto
                {
                    Id = id.Value,
                    Codigo = codigo,
                };
        }
    }
}
