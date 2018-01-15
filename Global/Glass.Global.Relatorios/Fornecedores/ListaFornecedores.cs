using Colosoft.Reports;
using System.ComponentModel.Composition;
using System.Linq;
using Colosoft;

namespace Glass.Global.Relatorios.Fornecedores
{
    /// <summary>
    /// Representa o relatório da lista de fornecedores.
    /// </summary>
    [ExportaRelatorio("ListaFornecedores")]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class ListaFornecedores : ReportDocument
    {
        #region Variáveis Locais

        private Global.Negocios.IFornecedorFluxo _fornecedorFluxo;

        #endregion

        #region Propriedades

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
        /// Situação.
        /// </summary>
        [ReportDataSourceParameter("situacao")]
        public Glass.Data.Model.SituacaoFornecedor? Situacao { get; set; }

        /// <summary>
        /// Cnpj
        /// </summary>
        [ReportDataSourceParameter("cnpj")]
        public string Cnpj { get; set; }

        /// <summary>
        /// Com crédito.
        /// </summary>
        [ReportDataSourceParameter("comCredito")]
        public bool ComCredito { get; set; }

        /// <summary>
        /// Tipo de Pessoa.
        /// </summary>
        [ReportDataSourceParameter("tipoPessoa")]
        public int? TipoPessoa { get; set; }

        [ReportDataSourceParameter("idConta")]
        public int? IdConta { get; set; }

        [ReportDataSourceParameter("tipoPagto")]
        public int? TipoPagto { get; set; }

        [ReportDataSourceParameter("endereco")]
        public string Endereco { get; set; }

        [ReportDataSourceParameter("vendedor")]
        public string Vendedor { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="fornecedorFluxo"></param>
        [ImportingConstructor]
        public ListaFornecedores(Negocios.IFornecedorFluxo fornecedorFluxo)
            : base("ListaFornecedores")
        {
            _fornecedorFluxo = fornecedorFluxo;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera a definição do relatório
        /// </summary>
        /// <returns></returns>
        public override System.IO.Stream GetDefinition()
        {
            return typeof(ListaFornecedores).Assembly
                .GetManifestResourceStream("Glass.Global.Relatorios.Fornecedores.rptListaFornecedores.rdlc");
        }

        /// <summary>
        /// Atualiza os DataSources.
        /// </summary>
        public override void RefreshDataSources()
        {
            var tipoPessoa = !TipoPessoa.HasValue ? (Data.Model.TipoPessoa?)null :
                TipoPessoa.Value == 1 ? Data.Model.TipoPessoa.Fisica :
                Data.Model.TipoPessoa.Juridica;

            var fornecedores = _fornecedorFluxo.PesquisarFornecedores
                (IdFornecedor, NomeFornecedor, Situacao, Cnpj, ComCredito, tipoPessoa, IdConta.GetValueOrDefault(0), TipoPagto.GetValueOrDefault(0),
                Endereco, Vendedor);

            // Recupera o critério da pesquisa
            Parameters.Add("Criterio", fornecedores.GetSearchParameterDescriptions().Join(" ").Format() ?? "");
            DataSources.Add(new ReportDataSource("Fornecedor", fornecedores.ToArray()));
        }

        #endregion
    }
}
