using Colosoft.Reports;
using System.Linq;
using Colosoft;
using System.ComponentModel.Composition;

namespace Glass.Global.Relatorios.Fornecedores
{
    /// <summary>
    /// Classe que representa o relatório da ficha do fornecedor.
    /// </summary>
    [ExportaRelatorio("FichaFornecedor")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FichaFornecedor : ReportDocument
    {
        #region Local Variables

        private Negocios.IFornecedorFluxo _fornecedorFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do fornecedor.
        /// </summary>
        [Colosoft.Reports.ReportDataSourceParameter("idFornecedor")]
        public int IdFornecedor { get; set; }

        /// <summary>
        /// Nome do fornecedor.
        /// </summary>
        [Colosoft.Reports.ReportDataSourceParameter("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Situação do fornecedor.
        /// </summary>
        [Colosoft.Reports.ReportDataSourceParameter("situacao")]
        public Data.Model.SituacaoFornecedor? Situacao { get; set; }

        /// <summary>
        /// Cnpj.
        /// </summary>
        [Colosoft.Reports.ReportDataSourceParameter("cnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Identifica se é crédito.
        /// </summary>
        [Colosoft.Reports.ReportDataSourceParameter("credito")]
        public bool ComCredito { get; set; }

        [ReportDataSourceParameter("idConta")]
        public int? IdConta { get; set; }

        [ReportDataSourceParameter("tipoPagto")]
        public int? TipoPagto { get; set; }

        [ReportDataSourceParameter("endereco")]
        public string Endereco { get; set; }

        [ReportDataSourceParameter("vendedor")]
        public string Vendedor { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="fornecedorFluxo"></param>
        [System.ComponentModel.Composition.ImportingConstructor]
        public FichaFornecedor(Negocios.IFornecedorFluxo fornecedorFluxo)
            : base("FichaFornecedor")
        {
            _fornecedorFluxo = fornecedorFluxo;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Atualiza as origens de dados.
        /// </summary>
        public override void RefreshDataSources()
        {
            if (IdFornecedor > 0)
            {
                var fornecedor = _fornecedorFluxo.ObtemDetalhesFornecedor(IdFornecedor);

                this.Parameters.Add("Criterio",
                    string.Format("Cód.: {0}    {1} {2}", 
                        fornecedor.IdFornec, 
                        fornecedor.TipoPessoa == "J" ? "Razão Social: " : "Nome: ", 
                        fornecedor.Nome));

                this.DataSources.Add(new ReportDataSource("Fornecedor", 
                    new [] { fornecedor }));
            }
            else
            {
                var fornecedores = _fornecedorFluxo.PesquisarFornecedores(IdFornecedor,  Nome, Situacao, CpfCnpj, ComCredito, null, IdConta.GetValueOrDefault(0), TipoPagto.GetValueOrDefault(0),
                    Endereco, Vendedor);

                this.Parameters.Add("Criterio", fornecedores.GetSearchParameterDescriptions().Join(" ").Format() ?? "");
                this.DataSources.Add(new ReportDataSource("Fornecedor", fornecedores.ToArray()));
            }
        }

        /// <summary>
        /// Recupera a definição do relatório.
        /// </summary>
        /// <returns></returns>
        public override System.IO.Stream GetDefinition()
        {
            return typeof(FichaFornecedor).Assembly
                .GetManifestResourceStream("Glass.Global.Relatorios.Fornecedores.rptFichaFornecedor.rdlc");
        }

        #endregion
    }
}
