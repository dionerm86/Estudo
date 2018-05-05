using System;
using System.Web.UI;
using Glass.Data.DAL;
using System.Text;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class ImportarArquivoRemessa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["FileUploadImportarArquivoRemessa"] = null;
            }
            else
            {
                if (fluArquivo.HasFile)
                    Session["FileUploadImportarArquivoRemessa"] = fluArquivo;
            }
        }
    
        protected void btnImportarArquivo_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fluArquivo.HasFile && Session["FileUploadImportarArquivoRemessa"] == null)
                {
                    MensagemAlerta.ErrorMsg("Selecione o arquivo que será importado.", null, Page);
                    return;                    
                }

                if (!fluArquivo.HasFile && Session["FileUploadImportarArquivoRemessa"] != null)
                    fluArquivo = (FileUpload)Session["FileUploadImportarArquivoRemessa"];

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

                Session["FileUploadImportarArquivoRemessa"] = null;

                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "retornoImportacao", script, true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao importar arquivo de remessa.", ex, Page);
            }
        }
        
        protected void btnVerificarArquivo_Click(object sender, EventArgs e)
        {
            int tipoCnab = Convert.ToInt32(ddlTipoCnab.SelectedValue);
            uint idContaBanco = Convert.ToUInt32(drpContaBanco.SelectedValue);

            var retorno = ArquivoRemessaDAO.Instance.VerificarImportarArquivoRemessa(fluArquivo.FileBytes, tipoCnab, idContaBanco, Request["cxDiario"] == "1");

            grdItensCNAB.Visible = true;
            grdItensCNAB.DataSource = retorno;
            grdItensCNAB.DataBind();
        }

        protected void grdItensCNAB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var item = e.Row.DataItem as Data.RelModel.LinhaRemessaRetorno;
            if (item == null)
                return;

            foreach (TableCell c in e.Row.Cells)
                c.ForeColor = item.Quitada ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }
    }
}
