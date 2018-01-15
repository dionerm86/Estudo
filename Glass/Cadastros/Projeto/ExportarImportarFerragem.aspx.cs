using System;
using System.Web.UI;
using System.Linq;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class ExportarImportarFerragem : System.Web.UI.Page
    {
        #region Carregamento da tela

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(ExportarImportarFerragem));
    
            if (!IsPostBack)
            {
                lblTamanhoMaximo.Text += string.Format("{0} MB", FuncoesGerais.GetTamanhoMaximoUpload());

                // Verifica se existem ids de ferragem na URL atual.
                if (Request["exportar"] != null)
                    // Registra um script que irá chamar o método de exportação de ferragens, para todos os ids de ferragem informados na URL atual.
                    Page.ClientScript.RegisterStartupScript(GetType(), "exportar", string.Format("exportar('{0}');", Request["exportar"]), true);
            }
        }

        #endregion

        #region Importação

        /// <summary>
        /// Método chamado ao clicar no botão Importar.
        /// </summary>
        protected void btnImportar_Click(object sender, EventArgs e)
        {
            try
            {
                // Recupera o fluxo de exportação de ferragem.
                var exportacaoImportacaoFerragemFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Projeto.Negocios.IExportacaoImportacaoFerragem>();
                // Importa o arquivo carregado pelo usuário.
                var retorno = exportacaoImportacaoFerragemFluxo.Importar(fluArquivo.FileBytes, chkSubstituirFerragemExistente.Checked);

                MensagemAlerta.ShowMsg(retorno.Message.ToString(), Page);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg(ex.InnerException != null ? string.Format("{0}\n", ex.Message) : string.Empty, ex, Page);
            }
        }

        #endregion

        #region Exportação

        /// <summary>
        /// Método chamado pelo botão Exportar.
        /// </summary>
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            // Recupera os ids das ferragens setadas na tela.
            var idsFerragem = hdfIdsFerragem.Value.TrimEnd(' ', ',');
            // Limpa o hidden field.
            hdfIdsFerragem.Value = string.Empty;

            // Envia os ids das ferragens, que devem ser exportadas, para a tela atual. Dessa forma, será identificado que existem ids de ferragem e a exportação será continuada pelo javascript.
            Response.Redirect(string.Format("~/Cadastros/Projeto/ExportarImportarFerragem.aspx?exportar={0}", idsFerragem));
        }

        #endregion

        #region Métodos ajax

        /// <summary>
        /// Método utilizado para recuperar uma ferragem pelo nome e retornar os dados dela para a tela.
        /// </summary>
        [Ajax.AjaxMethod]
        public string ObterDadosFerragem(string nomeFerragem)
        {
            try
            {
                // Recupera o fluxo de ferragem.
                var ferragemFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Projeto.Negocios.IFerragemFluxo>();
                // Obtém a ferragem pelo nome.
                var ferragem = ferragemFluxo.ObterFerragem(nomeFerragem);

                // Verifica se a ferragem foi encontrada.
                if (ferragem == null || ferragem.IdFerragem <= 0)
                    throw new Exception("Erro;Ferragem não encontrada.");

                return string.Format("Ok;{0};{1};{2};{3};{4}", ferragem.IdFerragem, ferragem.Nome, ferragem.Fabricante.Nome, ferragem.Situacao, ferragem.DataAlteracao);
            }
            catch (Exception ex)
            {
                return string.Format("Erro;{0}", MensagemAlerta.FormatErrorMsg("Falha ao recuperar os dados da ferragem.", ex));
            }
        }

        #endregion
    }
}
