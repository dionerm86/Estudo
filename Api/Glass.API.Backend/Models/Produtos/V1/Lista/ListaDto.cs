// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um produto para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="produto">A model de produtos.</param>
        internal ListaDto(ProdutoPesquisa produto)
        {
            this.Id = produto.IdProd;
            this.Nome = produto.Descricao;
            this.Codigo = produto.CodInterno;
            this.DescricaoBeneficiamentos = produto.DescricaoBeneficiamentos;
            this.DescricaoGrupo = produto.Grupo;
            this.DescricaoSubgrupo = produto.Subgrupo;
            this.Altura = produto.Altura;
            this.Largura = produto.Largura;
            this.Custos = new CustosDto()
            {
                Fornecedor = produto.Custofabbase,
                ComImpostos = produto.CustoCompra,
            };

            this.ValoresVenda = new ValoresVendaDto()
            {
                Atacado = produto.ValorAtacado,
                Balcao = produto.ValorBalcao,
                Obra = produto.ValorObra,
                Reposicao = produto.ValorReposicao,
            };

            this.Estoque = new EstoqueDto()
            {
                Reserva = (decimal)produto.Reserva,
                Liberacao = (decimal)produto.Liberacao,
                Real = (decimal)produto.QtdeEstoque,
                Disponivel = (decimal)produto.Disponivel,
                Unidade = Colosoft.Translator.Translate(produto.TipoCalculo, true).Format(),
            };

            this.UrlImagemProduto = Global.UI.Web.Process.ProdutoRepositorioImagens.Instance.ObtemUrl(produto.IdProd);
            this.Permissoes = new PermissoesDto
            {
                ExibirLinkReserva = produto.TipoCalculo != Data.Model.TipoCalculoGrupoProd.M2 && produto.TipoCalculo != Data.Model.TipoCalculoGrupoProd.M2Direto,
                ExibirLinkLiberacao = produto.TipoCalculo != Data.Model.TipoCalculoGrupoProd.M2 && produto.TipoCalculo != Data.Model.TipoCalculoGrupoProd.M2Direto,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Data.Model.LogAlteracao.TabelaAlteracao.Produto, (uint)produto.IdProd, null),
            };
        }

        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

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
        /// Obtém ou define os custos do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("custos")]
        public CustosDto Custos { get; set; }

        /// <summary>
        /// Obtém ou define os valores de venda do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("valoresVenda")]
        public ValoresVendaDto ValoresVenda { get; set; }

        /// <summary>
        /// Obtém ou define dados de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("estoque")]
        public EstoqueDto Estoque { get; set; }

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
