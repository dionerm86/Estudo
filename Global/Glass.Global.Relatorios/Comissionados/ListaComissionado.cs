using System.Linq;
using System.ComponentModel.Composition;
using Colosoft.Reports;

namespace Glass.Global.Relatorios.Comissionados
{
    /// <summary>
    /// Representa o relatório da lista de comissionados.
    /// </summary>
    [ExportaRelatorio("ListaComissionado")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class ListaComissionado : ReportDocument
    {
        #region Variáveis Locais

        private Negocios.IComissionadoFluxo _comissionadoFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Nome do comissionado.
        /// </summary>
        [ReportDataSourceParameter("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Situação.
        /// </summary>
        [ReportDataSourceParameter("situacao")]
        public Glass.Situacao? Situacao { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="comissionadoFluxo"></param>
        [ImportingConstructor]
        public ListaComissionado(Negocios.IComissionadoFluxo comissionadoFluxo)
            : base("ListaComissionado")
        {
            _comissionadoFluxo = comissionadoFluxo;
        }

        #endregion

        #region Métodos Protegidos

        /// <summary>
        /// Recupera a definição do relatório
        /// </summary>
        /// <returns></returns>
        public override System.IO.Stream GetDefinition()
        {
            return typeof(ListaComissionado).Assembly
                .GetManifestResourceStream("Glass.Global.Relatorios.Comissionados.rptListaComissionado.rdlc");
        }

        /// <summary>
        /// Atualiza os DataSources.
        /// </summary>
        public override void RefreshDataSources()
        {
            var comissionados = _comissionadoFluxo.PesquisarComissionados(Nome, Situacao);

            // Recupera o critério da pesquisa
            //Parameters.Add("Criterio", comissionados.GetSearchParameterDescriptions().Join(" ").Format() ?? "");
            DataSources.Add(new ReportDataSource("Comissionado", comissionados.ToArray()));
        }

        #endregion
    }
}
