// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.DAL;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um produto para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="produto">A model de produtos.</param>
        internal ListaDto(ProdutoPesquisa produto)
        {
            this.Id = produto.IdProd;
            this.Codigo = produto.CodInterno;
            this.Descricao = produto.DescricaoProdutoBeneficiamento;
            this.DescricaoBeneficiamentos = produto.DescricaoBeneficiamentos;
            this.DescricaoGrupo = produto.Grupo;
            this.DescricaoSubgrupo = produto.Subgrupo;
            this.Altura = produto.Altura;
            this.Largura = produto.Largura;
            this.CustoFornecedor = produto.Custofabbase;
            this.CustoComImpostos = produto.CustoCompra;
            this.ValorAtacado = produto.ValorAtacado;
            this.ValorBalcao = produto.ValorBalcao;
            this.ValorObra = produto.ValorObra;
            this.ValorReposicao = produto.ValorReposicao;
            this.QuantidadeReserva = (decimal)produto.Reserva;
            this.QuantidadeLiberacao = (decimal)produto.Liberacao;
            this.Estoque = produto.Estoque;
            this.EstoqueDisponivel = produto.EstoqueDisponivel;
            this.UrlImagemProduto = Global.UI.Web.Process.ProdutoRepositorioImagens.Instance.ObtemUrl(produto.IdProd);
            this.Permissoes = new PermissoesDto
            {
                ExibirLinkReserva = produto.TipoCalculo != Data.Model.TipoCalculoGrupoProd.M2 && produto.TipoCalculo != Data.Model.TipoCalculoGrupoProd.M2Direto,
                ExibirLinkLiberacao = produto.TipoCalculo != Data.Model.TipoCalculoGrupoProd.M2 && produto.TipoCalculo != Data.Model.TipoCalculoGrupoProd.M2Direto,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Data.Model.LogAlteracao.TabelaAlteracao.Produto, (uint)produto.IdProd, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a descrição dos beneficiamentos do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoBeneficiamentos")]
        public string DescricaoBeneficiamentos { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do grupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoGrupo")]
        public string DescricaoGrupo { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do subgrupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoSubgrupo")]
        public string DescricaoSubgrupo { get; set; }

        /// <summary>
        /// Obtém ou define a altura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public int? Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int? Largura { get; set; }

        /// <summary>
        /// Obtém ou define o custo de fornecedor do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("custoFornecedor")]
        public decimal CustoFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o custo com impostos do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("custoComImpostos")]
        public decimal CustoComImpostos { get; set; }

        /// <summary>
        /// Obtém ou define o valor de atacado do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("valorAtacado")]
        public decimal ValorAtacado { get; set; }

        /// <summary>
        /// Obtém ou define o valor de balcão do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("valorBalcao")]
        public decimal ValorBalcao { get; set; }

        /// <summary>
        /// Obtém ou define o valor de obra do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("valorObra")]
        public decimal ValorObra { get; set; }

        /// <summary>
        /// Obtém ou define o valor de reposição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("valorReposicao")]
        public decimal ValorReposicao { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de estoque em "reserva" do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeReserva")]
        public decimal QuantidadeReserva { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de estoque em "liberação" do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeLiberacao")]
        public decimal QuantidadeLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define o estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("estoque")]
        public string Estoque { get; set; }

        /// <summary>
        /// Obtém ou define o estoque disponível do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("estoqueDisponivel")]
        public string EstoqueDisponivel { get; set; }

        /// <summary>
        /// Obtém ou define a url da imagem do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("urlImagemProduto")]
        public string UrlImagemProduto { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas ao usuário na lista de produtos.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
