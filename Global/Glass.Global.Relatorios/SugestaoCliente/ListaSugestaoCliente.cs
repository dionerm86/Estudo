using System;
using System.Linq;
using System.ComponentModel.Composition;
using Colosoft;
using Colosoft.Reports;

namespace Glass.Global.Relatorios.SugestaoCliente
{
    /// <summary>
    /// Representa o relatório da lista de sugestões dos clientes.
    /// </summary>
    [ExportaRelatorio("ListaSugestaoCliente")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class ListaSugestaoCliente : ReportDocument
    {
        #region Variáveis Locais

        private Negocios.ISugestaoFluxo _sugestaoFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da sugestão.
        /// </summary>
        [ReportDataSourceParameter("idSug")]
        public int? IdSugestao { get; set; }

        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        [ReportDataSourceParameter("idCli")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Identificador do funcionário.
        /// </summary>
        [ReportDataSourceParameter("idFunc")]
        public int? IdFunc { get; set; }

        /// <summary>
        /// Nome do cliente.
        /// </summary>
        [ReportDataSourceParameter("nomeCli")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Data de início.
        /// </summary>
        [ReportDataSourceParameter("dataIni")]
        public DateTime? DataInicio { get; set; }

        /// <summary>
        /// Data de fim.
        /// </summary>
        [ReportDataSourceParameter("dataFim")]
        public DateTime? DataFim { get; set; }

        /// <summary>
        /// Tipo de sugestão.
        /// </summary>
        [ReportDataSourceParameter("tipo")]
        public Data.Model.TipoSugestao? Tipo { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        [ReportDataSourceParameter("desc")]
        public string Descricao { get; set; }

        /// <summary>
        /// Situação.
        /// </summary>
        [ReportDataSourceParameter("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Id Rota.
        /// </summary>
        [ReportDataSourceParameter("idRota")]
        public int? IdRota { get; set; }

        /// <summary>
        /// Id do pedido
        /// </summary>
        [ReportDataSourceParameter("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// IdOrcamento
        /// </summary>
        [ReportDataSourceParameter("idOrcamento")]
        public uint? IdOrcamento { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="sugestaoFluxo"></param>
        [ImportingConstructor]
        public ListaSugestaoCliente(Negocios.ISugestaoFluxo sugestaoFluxo)
            : base("ListaSugestaoCliente")
        {
            _sugestaoFluxo = sugestaoFluxo;
        }

        #endregion

        #region Métodos Protegidos

        /// <summary>
        /// Recupera a definição do relatório
        /// </summary>
        /// <returns></returns>
        public override System.IO.Stream GetDefinition()
        {
            return typeof(ListaSugestaoCliente).Assembly
                .GetManifestResourceStream("Glass.Global.Relatorios.SugestaoCliente.rptListaSugestaoCliente.rdlc");
        }

        /// <summary>
        /// Atualiza os DataSources.
        /// </summary>
        public override void RefreshDataSources()
        {
            var situacoes2 = new Situacao[0];

            if (!string.IsNullOrEmpty(Situacao))
                situacoes2 = Situacao.Split(',').Select(f => (Situacao)int.Parse(f)).ToArray();

            var sugestoes = _sugestaoFluxo.PesquisarSugestoes(
                IdSugestao, IdCliente, IdFunc, null, NomeCliente, DataInicio, DataFim, Tipo, Descricao, situacoes2, IdRota, IdPedido, IdOrcamento);

            // Recupera o critério da pesquisa
            Parameters.Add("Criterio", sugestoes.GetSearchParameterDescriptions().Join(" ").Format() ?? "");
            DataSources.Add(new ReportDataSource("SugestaoCliente", sugestoes.ToArray()));
        }

        #endregion
    }
}
