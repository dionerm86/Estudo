using Colosoft.Reports;
using System.ComponentModel.Composition;
using System.Linq;
using Colosoft;

namespace Glass.Global.Relatorios.Produtos
{
    /// <summary>
    /// Classe que representa o relatórios de produtos.
    /// </summary>
    [ExportaRelatorio("Produtos")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class Produtos : ReportDocument
    {
        #region Variáveis Locais

        private Glass.Global.Negocios.IProdutoFluxo _produtoFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da loja.
        /// </summary>
        [ReportDataSourceParameter("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Identificador do fornecedor.
        /// </summary>
        [ReportDataSourceParameter("idFornec")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Nome do fornecedor.
        /// </summary>
        [ReportDataSourceParameter("nomeFornec")]
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Identificador do grupo de produtos.
        /// </summary>
        [ReportDataSourceParameter("idGrupo")]
        public string IdGrupoProd { get; set; }

        /// <summary>
        /// Identificador do subgrupo de produtos.
        /// </summary>
        [ReportDataSourceParameter("idSubgrupo")]
        public string IdSubgrupoProd { get; set; }

        /// <summary>
        /// Código interno.
        /// </summary>
        [ReportDataSourceParameter("codInterno")]
        public string CodInterno { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        [ReportDataSourceParameter("descr")]
        public string Descricao { get; set; }

        /// <summary>
        /// Situação.
        /// </summary>
        [ReportDataSourceParameter("situacao")]
        public Glass.Situacao? Situacao { get; set; }

        /// <summary>
        /// Identificador do tipo de produto.
        /// </summary>
        [ReportDataSourceParameter("tipoProd")]
        public Global.Negocios.TipoNegociacaoProduto TipoNegociacaoProduto { get; set; }

        /// <summary>
        /// ApenasProdutosEstoqueBaixa
        /// </summary>
        [ReportDataSourceParameter("apenasProdutosEstoqueBaixa")]
        public bool ApenasProdutosEstoqueBaixa { get; set; }

        /// <summary>
        /// Altura de início.
        /// </summary>
        [ReportDataSourceParameter("alturaInicio")]
        public decimal? AlturaInicio { get; set; }

        /// <summary>
        /// Altura final.
        /// </summary>
        [ReportDataSourceParameter("alturaFim")]
        public decimal? AlturaFim { get; set; }

        /// <summary>
        /// Largura de início.
        /// </summary>
        [ReportDataSourceParameter("larguraInicio")]
        public decimal? LarguraInicio { get; set; }

        /// <summary>
        /// Largura final.
        /// </summary>
        [ReportDataSourceParameter("larguraFim")]
        public decimal? LarguraFim { get; set; }

        /// <summary>
        /// Ordenação.
        /// </summary>
        [ReportDataSourceParameter("orderBy")]
        public string Ordenacao { get; set; }

        /// <summary>
        /// Colunas.
        /// </summary>
        [ReportDataSourceParameter("colunas")]
        public string Colunas { get; set; }

        /// <summary>
        /// Agrupar.
        /// </summary>
        [ReportDataSourceParameter("agrupar")]
        public bool Agrupar { get; set; }

        /// <summary>
        /// Decrição do Atacado/Reposição.
        /// </summary>
        public string DescricaoAtacadoReposicao
        {
            get { return "Atacado"; }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="produtoFluxo"></param>
        [ImportingConstructor]
        public Produtos(Glass.Global.Negocios.IProdutoFluxo produtoFluxo)
            : base("Produtos")
        {
            _produtoFluxo = produtoFluxo;
        }

        #endregion

        #region Métodos Públicos

         /// <summary>
        /// Atualiza as fontes de dados.
        /// </summary>
        public override void RefreshDataSources()
        {
            var produtos = _produtoFluxo.PesquisarProdutos(
                CodInterno, Descricao, Situacao, IdLoja, IdFornecedor, NomeFornecedor, IdGrupoProd, IdSubgrupoProd,
                TipoNegociacaoProduto, ApenasProdutosEstoqueBaixa, Agrupar, AlturaInicio, AlturaFim, LarguraInicio, LarguraFim, Ordenacao);

            Parameters.Add("Criterio", produtos.GetSearchParameterDescriptions().Join(" ").Format() ?? "");
            Parameters.Add("ApenasProdutosEstoqueBaixa", ApenasProdutosEstoqueBaixa.ToString());
            Parameters.Add("Colunas", Colunas);
            Parameters.Add("Agrupar", Agrupar);
            Parameters.Add("DescricaoAtacadoReposicao", DescricaoAtacadoReposicao);


            DataSources.Add(new ReportDataSource("Produto", produtos.ToArray()));
        }


        /// <summary>
        /// Recupera a definição do relatório.
        /// </summary>
        /// <returns></returns>
        public override System.IO.Stream GetDefinition()
        {
            return typeof(FichaProduto).Assembly
                .GetManifestResourceStream("Glass.Global.Relatorios.Produtos.rptProdutos.rdlc");
        }

        #endregion
    }
}
