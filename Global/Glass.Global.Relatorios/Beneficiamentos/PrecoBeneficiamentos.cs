using Colosoft.Reports;
using System.ComponentModel.Composition;
using System.Linq;

namespace Glass.Global.Relatorios.Beneficiamentos
{
    /// <summary>
    /// Representa o relatório da lista de comissionados.
    /// </summary>
    [ExportaRelatorio("PrecoBeneficiamentos")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class PrecoBeneficiamentos : ReportDocument
    {
        #region Variáveis Locais

        private Negocios.IBeneficiamentoFluxo _beneficiamentoFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Descrição.
        /// </summary>
        [ReportDataSourceParameter("descricao")]
        public string Descricao { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="beneficiamentoFluxo"></param>
        [ImportingConstructor]
        public PrecoBeneficiamentos(Negocios.IBeneficiamentoFluxo beneficiamentoFluxo)
            : base("PrecoBeneficiamentos")
        {
            _beneficiamentoFluxo = beneficiamentoFluxo;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera a definição do relatório
        /// </summary>
        /// <returns></returns>
        public override System.IO.Stream GetDefinition()
        {
            return typeof(PrecoBeneficiamentos).Assembly
                .GetManifestResourceStream("Glass.Global.Relatorios.Beneficiamentos.rptPrecoBenefConfig.rdlc");
        }

        /// <summary>
        /// Atualiza os DataSources.
        /// </summary>
        public override void RefreshDataSources()
        {
            var precos = _beneficiamentoFluxo.PesquisarResumoPrecosPadraoBeneficiamentos(Descricao);

            // Recupera o critério da pesquisa
            DataSources.Add(new ReportDataSource("BenefConfigPreco", precos.ToArray()));
        }

        #endregion
    }
}
