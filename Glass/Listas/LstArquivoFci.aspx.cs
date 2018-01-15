using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstArquivoFci : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/CadArquivoFCI.aspx");
        }
    
        protected void grdArquivoFci_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ImportarArqRetorno")
            {
                var row = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                var idArquivoFci = Glass.Conversoes.StrParaUintNullable(((HiddenField)row.FindControl("hdfIdArquivoFci")).Value);
                var arqRetorno = ((FileUpload)row.FindControl("fupArqRetorno")).FileBytes;
    
                if (idArquivoFci == null || idArquivoFci == 0)
                {
                    Glass.MensagemAlerta.ShowMsg("Informe o FCI.", Page);
                    return;
                }
    
                if (arqRetorno == null || arqRetorno.Length == 0)
                {
                    Glass.MensagemAlerta.ShowMsg("Informe o arquivo de retorno.", Page);
                    return;
                }
    
                try
                {
                    WebGlass.Business.FCI.Fluxo.ArquivoFCIFluxo.Instance.ImportaArqRetorno(idArquivoFci.Value, arqRetorno);
                    Page.ClientScript.RegisterStartupScript(typeof(string), Guid.NewGuid().ToString(),
                    "atualiza('Arquivo de retorno importado.');", true);
                }
                catch (Exception ex)
                {
                    Page.ClientScript.RegisterStartupScript(typeof(string), Guid.NewGuid().ToString(),
                    "atualiza('Falha ao importar arquivo de retorno FCI. " + ex.Message + "');", true);
                }
            }
        }
    
        #region Métodos da pagina
    
        private bool corAlternada = true;
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    
        protected void odsArquivoFci_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir arquivo FCI.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                grdArquivoFci.DataBind();
            }
        }
    
        #endregion
    }
}
