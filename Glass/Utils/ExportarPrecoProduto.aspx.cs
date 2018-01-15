using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class ExportarPrecoProduto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnImportar_Click(object sender, EventArgs e)
        {
            try
            {
                WebGlass.Business.Produto.Fluxo.ImportarPrecos.Instance.Importar(filArquivo.FileBytes);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok", @"alert('Arquivo importado com sucesso!');
                    window.opener.atualizarPagina(); closeWindow();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao importar arquivo de preços.", ex, Page);
            }
        }
    }
}
