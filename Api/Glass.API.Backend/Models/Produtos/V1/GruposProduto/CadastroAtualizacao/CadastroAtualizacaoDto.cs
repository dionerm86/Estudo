// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.GruposProduto.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um grupo de produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do grupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo do grupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public TipoGrupoProd Tipo
        {
            get { return this.ObterValor(c => c.Tipo); }
            set { this.AdicionarValor(c => c.Tipo, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo de cálculo no pedido do grupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCalculoPedido")]
        public TipoCalculoGrupoProd? TipoCalculoPedido
        {
            get { return this.ObterValor(c => c.TipoCalculoPedido); }
            set { this.AdicionarValor(c => c.TipoCalculoPedido, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo de cálculo na nota fiscal do grupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoCalculoNotaFiscal")]
        public TipoCalculoGrupoProd? TipoCalculoNotaFiscal
        {
            get { return this.ObterValor(c => c.TipoCalculoNotaFiscal); }
            set { this.AdicionarValor(c => c.TipoCalculoNotaFiscal, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo de produto bloqueia venda se não tiver estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("bloquearEstoque")]
        public bool BloquearEstoque
        {
            get { return this.ObterValor(c => c.BloquearEstoque); }
            set { this.AdicionarValor(c => c.BloquearEstoque, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo de produto altera estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoque")]
        public bool AlterarEstoque
        {
            get { return this.ObterValor(c => c.AlterarEstoque); }
            set { this.AdicionarValor(c => c.AlterarEstoque, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo de produto altera estoque fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueFiscal")]
        public bool AlterarEstoqueFiscal
        {
            get { return this.ObterValor(c => c.AlterarEstoqueFiscal); }
            set { this.AdicionarValor(c => c.AlterarEstoqueFiscal, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibida mensagem de estoque caso o produto deste grupo não tenha estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirMensagemEstoque")]
        public bool ExibirMensagemEstoque
        {
            get { return this.ObterValor(c => c.ExibirMensagemEstoque); }
            set { this.AdicionarValor(c => c.ExibirMensagemEstoque, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo de produto gera volume.
        /// </summary>
        [DataMember]
        [JsonProperty("geraVolume")]
        public bool GeraVolume
        {
            get { return this.ObterValor(c => c.GeraVolume); }
            set { this.AdicionarValor(c => c.GeraVolume, value); }
        }
    }
}
