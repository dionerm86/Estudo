// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.SubgruposProduto.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um subgrupo de produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o identificador do grupo do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupoProduto")]
        public int IdGrupoProduto
        {
            get { return this.ObterValor(c => c.IdGrupoProduto); }
            set { this.AdicionarValor(c => c.IdGrupoProduto, value); }
        }

        /// <summary>
        /// Obtém ou define o nome do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define o cliente do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idCliente")]
        public int? IdCliente
        {
            get { return this.ObterValor(c => c.IdCliente); }
            set { this.AdicionarValor(c => c.IdCliente, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public TipoSubgrupoProd Tipo
        {
            get { return this.ObterValor(c => c.Tipo); }
            set { this.AdicionarValor(c => c.Tipo, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo de cálculo no pedido do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCalculoPedido")]
        public TipoCalculoGrupoProd? TipoCalculoPedido
        {
            get { return this.ObterValor(c => c.TipoCalculoPedido); }
            set { this.AdicionarValor(c => c.TipoCalculoPedido, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo de cálculo na nota fiscal do subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCalculoNotaFiscal")]
        public TipoCalculoGrupoProd? TipoCalculoNotaFiscal
        {
            get { return this.ObterValor(c => c.TipoCalculoNotaFiscal); }
            set { this.AdicionarValor(c => c.TipoCalculoNotaFiscal, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se os produtos deste subgrupo são para estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("produtoParaEstoque")]
        public bool ProdutoParaEstoque
        {
            get { return this.ObterValor(c => c.ProdutoParaEstoque); }
            set { this.AdicionarValor(c => c.ProdutoParaEstoque, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o subgrupo de produto bloqueia venda se não tiver estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("bloquearEstoque")]
        public bool BloquearEstoque
        {
            get { return this.ObterValor(c => c.BloquearEstoque); }
            set { this.AdicionarValor(c => c.BloquearEstoque, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o subgrupo de produto altera estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoque")]
        public bool AlterarEstoque
        {
            get { return this.ObterValor(c => c.AlterarEstoque); }
            set { this.AdicionarValor(c => c.AlterarEstoque, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o subgrupo de produto altera estoque fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueFiscal")]
        public bool AlterarEstoqueFiscal
        {
            get { return this.ObterValor(c => c.AlterarEstoqueFiscal); }
            set { this.AdicionarValor(c => c.AlterarEstoqueFiscal, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibida mensagem de estoque caso o produto deste subgrupo não tenha estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirMensagemEstoque")]
        public bool ExibirMensagemEstoque
        {
            get { return this.ObterValor(c => c.ExibirMensagemEstoque); }
            set { this.AdicionarValor(c => c.ExibirMensagemEstoque, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o subgrupo de produto é de vidros temperados.
        /// </summary>
        [DataMember]
        [JsonProperty("vidroTemperado")]
        public bool VidroTemperado
        {
            get { return this.ObterValor(c => c.VidroTemperado); }
            set { this.AdicionarValor(c => c.VidroTemperado, value); }
        }

        /// <summary>
        /// Obtém ou define a quantidade de dias mínimos para entrega dos produtos deste subgrupo.
        /// </summary>
        [DataMember]
        [JsonProperty("diasMinimoEntrega")]
        public int? DiasMinimoEntrega
        {
            get { return this.ObterValor(c => c.DiasMinimoEntrega); }
            set { this.AdicionarValor(c => c.DiasMinimoEntrega, value); }
        }

        /// <summary>
        /// Obtém ou define o dia da semana para entrega dos produtos deste subgrupo.
        /// </summary>
        [DataMember]
        [JsonProperty("diaSemanaEntrega")]
        public int? DiaSemanaEntrega
        {
            get { return this.ObterValor(c => c.DiaSemanaEntrega); }
            set { this.AdicionarValor(c => c.DiaSemanaEntrega, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se os produtos desse subgrupo podem ser liberados com a produção pendente.
        /// </summary>
        [DataMember]
        [JsonProperty("liberarPendenteProducao")]
        public bool LiberarPendenteProducao
        {
            get { return this.ObterValor(c => c.LiberarPendenteProducao); }
            set { this.AdicionarValor(c => c.LiberarPendenteProducao, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se é permitida a revenda de produtos do tipo venda (solução para inclusão de embalagem no pedido de venda).
        /// </summary>
        [DataMember]
        [JsonProperty("permitirItemRevendaNaVenda")]
        public bool PermitirItemRevendaNaVenda
        {
            get { return this.ObterValor(c => c.PermitirItemRevendaNaVenda); }
            set { this.AdicionarValor(c => c.PermitirItemRevendaNaVenda, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se os produtos deste subgrupo poderão ser usados no ecommerce.
        /// </summary>
        [DataMember]
        [JsonProperty("bloquearVendaECommerce")]
        public bool BloquearVendaECommerce
        {
            get { return this.ObterValor(c => c.BloquearVendaECommerce); }
            set { this.AdicionarValor(c => c.BloquearVendaECommerce, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o subgrupo de produto gera volume.
        /// </summary>
        [DataMember]
        [JsonProperty("geraVolume")]
        public bool GeraVolume
        {
            get { return this.ObterValor(c => c.GeraVolume); }
            set { this.AdicionarValor(c => c.GeraVolume, value); }
        }

        /// <summary>
        /// Obtém ou define as lojas associadas à este subgrupo.
        /// </summary>
        [DataMember]
        [JsonProperty("idsLojasAssociadas")]
        public IEnumerable<int> IdsLojasAssociadas
        {
            get { return this.ObterValor(c => c.IdsLojasAssociadas); }
            set { this.AdicionarValor(c => c.IdsLojasAssociadas, value); }
        }
    }
}
