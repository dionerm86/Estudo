using System.Linq;
using System.ComponentModel.Composition;
using Colosoft.Reports;
using Glass.Global.Negocios;

namespace Glass.Global.Relatorios.Rotas
{
    /// <summary>
    /// Representa o relatório dos dados da rota.
    /// </summary>
    [ExportaRelatorio("DadosRota")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class DadosRota : ReportDocument
    {
        #region Variáveis Locais

        private IRotaFluxo _rotaFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da rota.
        /// </summary>
        [ReportDataSourceParameter("idRota")]
        public int IdRota { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="rotaFluxo"></param>
        [ImportingConstructor]
        public DadosRota(IRotaFluxo rotaFluxo)
            : base("DadosRota")
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
                .GetManifestResourceStream("Glass.Global.Relatorios.Rotas.rptDadosRota.rdlc");
        }

        /// <summary>
        /// Atualiza os DataSources.
        /// </summary>
        public override void RefreshDataSources()
        {
            var rota = _rotaFluxo.ObtemRota(IdRota);
            var rotaClientes = _rotaFluxo.PesquisarClientesRota(IdRota);

            DataSources.Add(new ReportDataSource("Rota", new Global.Negocios.Entidades.Rota[] { rota }));
            DataSources.Add(new ReportDataSource("RotaCliente", rotaClientes.ToArray()));
        }

        #endregion
    }
}
