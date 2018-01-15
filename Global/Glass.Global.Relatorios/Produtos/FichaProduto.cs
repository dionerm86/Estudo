using Colosoft.Reports;
using Glass.Global.Negocios;
using System.ComponentModel.Composition;
using System.Linq;
using Colosoft;

namespace Glass.Global.Relatorios.Produtos
{
    /// <summary>
    /// Classe que representa o relatório da ficha do produto.
    /// </summary>
    [ExportaRelatorio("FichaProdutos")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class FichaProduto : ReportDocument
    {
        #region Variáveis Locais

        private IProdutoFluxo _produtoFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        [ReportDataSourceParameter("idProduto")]
        public int? IdProduto { get; set; }

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
        public int? IdGrupoProd { get; set; }

        /// <summary>
        /// Identificador do subgrupo de produtos.
        /// </summary>
        [ReportDataSourceParameter("idSubgrupo")]
        public int? IdSubgrupoProd { get; set; }

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
        /// Tipo do produto.
        /// </summary>
        [ReportDataSourceParameter("tipoProd")]
        public TipoNegociacaoProduto TipoNegociacao { get; set; }

        /// <summary>
        /// Situação.
        /// </summary>
        [ReportDataSourceParameter("situacao")]
        public Glass.Situacao? Situacao { get; set; }

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

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="produtoFluxo"></param>
        [ImportingConstructor]
        public FichaProduto(IProdutoFluxo produtoFluxo)
            : base("FichaProdutos")
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
            if (IdProduto.HasValue && IdProduto.Value > 0)
            {
                // Recupera os dados do produto
                var produto = _produtoFluxo.ObtemFichaProduto(IdProduto.Value);

                this.DataSources.Add(new ReportDataSource("Produtos",
                    new Global.Negocios.Entidades.FichaProduto[]
                    {
                        produto
                    }));

                Parameters.Add("Criterio", string.Format("Cód.: {0}", produto.CodInterno));
            }
            else
            {
                var produtos = _produtoFluxo.ObtemFichasProdutos(
                    IdFornecedor, NomeFornecedor, IdGrupoProd, IdSubgrupoProd, CodInterno,
                    Descricao, TipoNegociacao, Situacao, ApenasProdutosEstoqueBaixa,
                    AlturaInicio.GetValueOrDefault(), AlturaFim.GetValueOrDefault(),
                    LarguraInicio.GetValueOrDefault(), LarguraFim.GetValueOrDefault(), Ordenacao);

                this.Parameters.Add("Criterio", produtos.GetSearchParameterDescriptions().Join(" ").Format() ?? "");
                this.DataSources.Add(new ReportDataSource("Produtos", produtos.ToArray()));
            }


            Parameters.Add("ApenasProdutosEstoqueBaixa", IdProduto.HasValue ? 
                (!ApenasProdutosEstoqueBaixa).ToString() : ApenasProdutosEstoqueBaixa.ToString());

            Parameters.Add("Colunas", Colunas);
            Parameters.Add("Agrupar", Agrupar);
        }

        /// <summary>
        /// Recupera a definição do relatório.
        /// </summary>
        /// <returns></returns>
        public override System.IO.Stream GetDefinition()
        {
            return typeof(FichaProduto).Assembly
                .GetManifestResourceStream("Glass.Global.Relatorios.Produtos.rptFichaProduto.rdlc");
        }

        #endregion
    }
}
