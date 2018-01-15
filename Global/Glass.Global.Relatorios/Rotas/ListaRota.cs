using System.Linq;
using System.ComponentModel.Composition;
using Colosoft.Reports;
using Glass.Global.Negocios;

namespace Glass.Global.Relatorios.Rotas
{
    /// <summary>
    /// Representa o relatório da lista de rotas.
    /// </summary>
    [ExportaRelatorio("ListaRota")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class ListaRota : ReportDocument
    {
        #region Variáveis Locais

        private IRotaFluxo _rotaFluxo;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="rotaFluxo"></param>
        [ImportingConstructor]
        public ListaRota(IRotaFluxo rotaFluxo)
            : base("ListaRota")
        {
            _rotaFluxo = rotaFluxo;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera a definição do relatório
        /// </summary>
        /// <returns></returns>
        public override System.IO.Stream GetDefinition()
        {
            return typeof(DadosRota).Assembly
                .GetManifestResourceStream("Glass.Global.Relatorios.Rotas.rptRota.rdlc");
        }

        /// <summary>
        /// Atualiza os DataSources.
        /// </summary>
        public override void RefreshDataSources()
        {
            var rotas = _rotaFluxo.PesquisarRotas();

            DataSources.Add(new ReportDataSource("Rota", rotas.ToArray()));
        }

        #endregion
    }
}
