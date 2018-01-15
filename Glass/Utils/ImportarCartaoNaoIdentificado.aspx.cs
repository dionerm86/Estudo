using System;
using System.Web.UI;
using Glass.Data.DAL;
using System.IO;

namespace Glass.UI.Web.Utils
{
    public partial class ImportarCartaoNaoIdentificado : System.Web.UI.Page
    {

        private Financeiro.UI.Web.Process.CartaoNaoIdentificado.CadastroCartaoNaoIdentificadoFluxo _cadastroCNI;

        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnImportarArquivo_Click(object sender, EventArgs e)
        {
            try
            {
                var stream = new MemoryStream(fluArquivo.FileBytes);

                var extensao = Path.GetExtension(fluArquivo.FileName);                        

                _cadastroCNI = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<Financeiro.UI.Web.Process.CartaoNaoIdentificado.CadastroCartaoNaoIdentificadoFluxo>();

                var retorno = _cadastroCNI.Importar(stream, extensao, Request["cxDiario"] == "1");

                var script = string.Empty;

                if (!retorno)
                    MensagemAlerta.ErrorMsg("Falha ao importar arquivo", retorno);
                else
                    MensagemAlerta.ShowMsg("Arquivo importado com sucesso!", this);               
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao importar arquivo de cartões não identificados.", ex, Page);
            }
        }

        protected void Voltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstCartaoNaoIdentificado.aspx");
        }
    }
}
