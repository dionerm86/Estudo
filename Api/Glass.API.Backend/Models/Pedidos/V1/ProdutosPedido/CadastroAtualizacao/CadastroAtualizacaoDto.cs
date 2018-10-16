// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Glass.API.Backend.Models.Genericas.V1.Venda;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ProdutosPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados para cadastro ou atualização de produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o identificador do ambiente de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idAmbiente")]
        public int? IdAmbiente
        {
            get { return this.ObterValor(c => c.IdAmbiente); }
            set { this.AdicionarValor(c => c.IdAmbiente, value); }
        }

        /// <summary>
        /// Obtém ou define os dados básicos do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public ProdutoDto Produto
        {
            get { return this.ObterValor(c => c.Produto); }
            set { this.AdicionarValor(c => c.Produto, value); }
        }

        /// <summary>
        /// Obtém ou define a quantidade de produtos do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public decimal Quantidade
        {
            get { return this.ObterValor(c => c.Quantidade); }
            set { this.AdicionarValor(c => c.Quantidade, value); }
        }

        /// <summary>
        /// Obtém ou define os dados do desconto por quantidade do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("descontoPorQuantidade")]
        public PercentualValorDto DescontoPorQuantidade
        {
            get { return this.ObterValor(c => c.DescontoPorQuantidade); }
            set { this.AdicionarValor(c => c.DescontoPorQuantidade, value); }
        }

        /// <summary>
        /// Obtém ou define a largura do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int Largura
        {
            get { return this.ObterValor(c => c.Largura); }
            set { this.AdicionarValor(c => c.Largura, value); }
        }

        /// <summary>
        /// Obtém ou define a altura do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public DadoRealECalculadoDto Altura
        {
            get { return this.ObterValor(c => c.Altura); }
            set { this.AdicionarValor(c => c.Altura, value); }
        }

        /// <summary>
        /// Obtém ou define a área, em m², do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("areaEmM2")]
        public DadoRealECalculadoDto AreaEmM2
        {
            get { return this.ObterValor(c => c.AreaEmM2); }
            set { this.AdicionarValor(c => c.AreaEmM2, value); }
        }

        /// <summary>
        /// Obtém ou define o valor unitário do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("valorUnitario")]
        public decimal ValorUnitario
        {
            get { return this.ObterValor(c => c.ValorUnitario); }
            set { this.AdicionarValor(c => c.ValorUnitario, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do processo do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idProcesso")]
        public int? IdProcesso
        {
            get { return this.ObterValor(c => c.IdProcesso); }
            set { this.AdicionarValor(c => c.IdProcesso, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador da aplicação do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idAplicacao")]
        public int? IdAplicacao
        {
            get { return this.ObterValor(c => c.IdAplicacao); }
            set { this.AdicionarValor(c => c.IdAplicacao, value); }
        }

        /// <summary>
        /// Obtém ou define o código do pedido do cliente para o produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoPedidoCliente")]
        public string CodigoPedidoCliente
        {
            get { return this.ObterValor(c => c.CodigoPedidoCliente); }
            set { this.AdicionarValor(c => c.CodigoPedidoCliente, value); }
        }

        /// <summary>
        /// Obtém ou define o valor total do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total
        {
            get { return this.ObterValor(c => c.Total); }
            set { this.AdicionarValor(c => c.Total, value); }
        }

        /// <summary>
        /// Obtém ou define a observação do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao
        {
            get { return this.ObterValor(c => c.Observacao); }
            set { this.AdicionarValor(c => c.Observacao, value); }
        }

        /// <summary>
        /// Obtém ou define os beneficiamentos do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("beneficiamentos")]
        public BeneficiamentosDto Beneficiamentos
        {
            get { return this.ObterValor(c => c.Beneficiamentos); }
            set { this.AdicionarValor(c => c.Beneficiamentos, value); }
        }

        /// <summary>
        /// Obtém ou define os dados de composição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("composicao")]
        public ComposicaoDto Composicao
        {
            get { return this.ObterValor(c => c.Composicao); }
            set { this.AdicionarValor(c => c.Composicao, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do processo do produto do pedido peças filhas.
        /// </summary>
        [DataMember]
        [JsonProperty("idProcessoFilhas")]
        public int? IdProcessoFilhas
        {
            get { return this.ObterValor(c => c.IdProcessoFilhas); }
            set { this.AdicionarValor(c => c.IdProcessoFilhas, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador da aplicação do produto do pedido peças filhas.
        /// </summary>
        [DataMember]
        [JsonProperty("idAplicacaoFilhas")]
        public int? IdAplicacaoFilhas
        {
            get { return this.ObterValor(c => c.IdAplicacaoFilhas); }
            set { this.AdicionarValor(c => c.IdAplicacaoFilhas, value); }
        }
    }
}
