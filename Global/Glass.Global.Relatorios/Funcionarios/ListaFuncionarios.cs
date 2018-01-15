using Colosoft.Reports;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using Colosoft;

namespace Glass.Global.Relatorios.Funcionarios
{
    /// <summary>
    /// Representa o relatório da lista de comissionados.
    /// </summary>
    [ExportaRelatorio("ListaFuncionarios")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class ListaFuncionarios : ReportDocument
    {
        #region Variáveis Locais

        private Glass.Global.Negocios.IFuncionarioFluxo _funcionarioFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador da loja.
        /// </summary>
        [ReportDataSourceParameter("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Nome do funcionário.
        /// </summary>
        [ReportDataSourceParameter("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Situação.
        /// </summary>
        [ReportDataSourceParameter("situacao")]
        public Glass.Situacao? Situacao { get; set; }

        /// <summary>
        /// Identifica se é para recupera apenas registrados.
        /// </summary>
        [ReportDataSourceParameter("regitrado")]
        public bool ApenasRegistrados { get; set; }

        /// <summary>
        /// Identificador do tipo de funcionário.
        /// </summary>
        [ReportDataSourceParameter("idTipoFunc")]
        public int? IdTipoFuncionario { get; set; }

        /// <summary>
        /// Identificador do setor.
        /// </summary>
        [ReportDataSourceParameter("idSetorFunc")]
        public int? IdSetor { get; set; }

        /// <summary>
        /// Inicio da data de nascimento.
        /// </summary>
        [ReportDataSourceParameter("dtNascIni")]
        public DateTime? DataNascInicio { get; set; }

        /// <summary>
        /// Fim da data de nascimento
        /// </summary>
        [ReportDataSourceParameter("dtNascFim")]
        public DateTime? DataNascFim { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="funcionarioFluxo"></param>
        [ImportingConstructor]
        public ListaFuncionarios(Glass.Global.Negocios.IFuncionarioFluxo funcionarioFluxo)
            : base("ListaFuncionarios")
        {
            _funcionarioFluxo = funcionarioFluxo;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera a definição do relatório
        /// </summary>
        /// <returns></returns>
        public override System.IO.Stream GetDefinition()
        {
            return typeof(ListaFuncionarios).Assembly
                .GetManifestResourceStream("Glass.Global.Relatorios.Funcionarios.rptListaFuncionarios.rdlc");
        }

        /// <summary>
        /// Atualiza os DataSources.
        /// </summary>
        public override void RefreshDataSources()
        {
            var funcionarios = _funcionarioFluxo
                .PesquisarFuncionarios(IdLoja, Nome, Situacao, ApenasRegistrados, IdTipoFuncionario, IdSetor, DataNascInicio, DataNascFim);

            this.Parameters.Add("Criterio", funcionarios.GetSearchParameterDescriptions().Join(" ").Format() ?? "");
            // Recupera o critério da pesquisa
            DataSources.Add(new ReportDataSource("Funcionario", funcionarios.ToArray()));
        }

        #endregion
    }
}
