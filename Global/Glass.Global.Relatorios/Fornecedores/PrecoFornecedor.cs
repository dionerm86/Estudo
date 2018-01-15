using Colosoft.Reports;
using System.ComponentModel.Composition;
using System.Linq;
using Colosoft;

namespace Glass.Global.Relatorios.Fornecedores
{
    /// <summary>
    /// Representa o relatório dos preços do fornecedor.
    /// </summary>
    [ExportaRelatorio("PrecoFornecedor")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class PrecoFornecedor : ReportDocument
    {
        #region Variáveis Locais

        private Negocios.IFornecedorFluxo _fornecedorFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        [ReportDataSourceParameter("idProd")]
        public int? IdProduto { get; set; }

        /// <summary>
        /// Identificador do fornecedor.
        /// </summary>
        [ReportDataSourceParameter("idFornec")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Código do produto.
        /// </summary>
        [ReportDataSourceParameter("codProd")]
        public string CodigoProduto { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        [ReportDataSourceParameter("descrProd")]
        public string DescricaoProduto { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="fornecedorFluxo"></param>
        [ImportingConstructor]
        public PrecoFornecedor(Negocios.IFornecedorFluxo fornecedorFluxo)
            : base("PrecoFornecedor")
        {
            _fornecedorFluxo = fornecedorFluxo;
        }

        #endregion

        #region Método Públicos

        /// <summary>
        /// Recupera a defineção do relatório.
        /// </summary>
        /// <returns></returns>
        public override System.IO.Stream GetDefinition()
        {
            return typeof(PrecoFornecedor).Assembly
                .GetManifestResourceStream("Glass.Global.Relatorios.Fornecedores.rptPrecoFornecedor.rdlc");
        }

        /// <summary>
        /// Atualiza as fontes de dados.
        /// </summary>
        public override void RefreshDataSources()
        {
            var produtosFornecedor = _fornecedorFluxo.PesquisarProdutosFornecedor(IdFornecedor, IdProduto, null, CodigoProduto, DescricaoProduto);

            // Recupera o critério da pesquisa
            Parameters.Add("Criterio", produtosFornecedor.GetSearchParameterDescriptions().Join(" ").Format() ?? "");
            Parameters.Add("AgruparProduto", (produtosFornecedor.Select(x => x.IdProd).Distinct().Count() == 1).ToString());
            Parameters.Add("AgruparFornecedor", (produtosFornecedor.Select(x => x.IdFornec).Distinct().Count() == 1).ToString());
            DataSources.Add(new ReportDataSource("ProdutoFornecedor", produtosFornecedor));
        }

        #endregion
    }
}
