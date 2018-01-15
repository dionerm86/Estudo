using System;
using System.Web.UI;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class ExportarImportar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Projeto.ExportarImportar));
    
            if (!IsPostBack)
            {
                lblTamanhoMaximo.Text += FuncoesGerais.GetTamanhoMaximoUpload() + " MB";
                if (Request["exportar"] != null)
                    Page.ClientScript.RegisterStartupScript(GetType(),
                        "exportar", "exportar('" + Request["exportar"] + "'" + "," + Request["semFolgas"] + ");", true);
            }
        }
    
        [Ajax.AjaxMethod]
        public string GetDadosProjetoModelo(string codigo)
        {
            try
            {
                ProjetoModelo projeto = ProjetoModeloDAO.Instance.GetByCodigo(codigo);
                if (projeto == null)
                    throw new Exception("Modelo de projeto não encontrado.");
    
                return "Ok;" + projeto.IdProjetoModelo + ";" + projeto.DescrGrupo + ";" + projeto.Descricao + ";" + projeto.Espessura;
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao recuperar dados do modelo de projeto.", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public string PodeAdicionar(string idProjetoModelo)
        {
            return ProjetoModeloDAO.Instance.IsConfiguravel(Glass.Conversoes.StrParaUint(idProjetoModelo)).ToString().ToLower();
        }
    
        protected void btnImportar_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = UtilsExportacaoProjeto.Importar(fluArquivo.FileBytes, cbxArquivoMesa.Checked, cbxFlag.Checked, cbxRegraValidacao.Checked, cbxFormulaExpressaoCalculo.Checked,
                    cbxSubstituirProjetoModeloExistente.Checked);                    
                Glass.MensagemAlerta.ShowMsg(msg, Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(ex.InnerException != null ? ex.Message + "\n" : "", ex, Page);
            }
        }
    
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            string ids = hdfIdsProjetosModelosExp.Value.TrimEnd(' ', ',');
            hdfIdsProjetosModelosExp.Value = "";
            Response.Redirect("~/Cadastros/Projeto/ExportarImportar.aspx?semFolgas=" + chkExportarSemFolgas.Checked.ToString().ToLower() +
                "&exportar=" + ids);
        }
    
        protected void btnDuplicar_Click(object sender, EventArgs e)
        {
            uint idGrupoModelo = Glass.Conversoes.StrParaUint(drpGrupoModelo.SelectedValue);
            string ids = hdfIdsProjetosModelosDup.Value.TrimEnd(' ', ',');
            hdfIdsProjetosModelosDup.Value = "";
            string msg = UtilsExportacaoProjeto.Duplicar(idGrupoModelo, txtFinalCodigo.Text, ids);
            Glass.MensagemAlerta.ShowMsg(msg, Page);
        }
    }
}
