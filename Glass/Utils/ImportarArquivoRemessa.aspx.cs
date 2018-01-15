using System;
using System.Web.UI;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class ImportarArquivoRemessa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnImportarArquivo_Click(object sender, EventArgs e)
        {
            try
            {
                int tipoCnab = Convert.ToInt32(ddlTipoCnab.SelectedValue);
                uint idContaBanco = Convert.ToUInt32(drpContaBanco.SelectedValue);
    
                var retorno = ArquivoRemessaDAO.Instance.ImportarRetorno(fluArquivo.FileBytes, tipoCnab, idContaBanco,
                    Request["cxDiario"] == "1");
    
                string script;
    
                if (!string.IsNullOrEmpty(retorno))
                {
                    var fileName = retorno.Split(';')[0];
                    var filePath = retorno.Split(';')[1];
    
                    filePath = filePath.Replace("\\", "\\\\");
    
                    script = "alert('Arquivo importado com sucesso!\\nAlgumas contas não foram recebidas.');" +
                        "window.location.href = window.location.href;" +
                        "redirectUrl('../Handlers/Download.ashx?filePath=" + filePath + "&fileName=" + fileName + "');";
    
                }
                else
                {
                    script = "alert('Arquivo importado com sucesso!');window.location.href = window.location.href;";
                }
    
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "retornoImportacao", script, true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao importar arquivo de remessa.", ex, Page);
            }
        }
    }
}
