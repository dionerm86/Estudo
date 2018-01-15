using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancPagtoSinalCompra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ctrlDataEstorno.Data = DateTime.Now;
                estornoBanco.Visible = ExibirEstornoBanco();
            }
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                Glass.FilaOperacoes.RecebimentosGerais.AguardarVez();
                DateTime? dataEstorno = chkEstornar.Checked ? ctrlDataEstorno.DataNullable : null;
                SinalCompraDAO.Instance.CancelarSinal(Glass.Conversoes.StrParaUint(Request["IdSinalCompra"]),
                    txtMotivo.Text, dataEstorno);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
            finally
            {
                Glass.FilaOperacoes.RecebimentosGerais.ProximoFila();
            }

            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }
    
        protected bool ExibirEstornoBanco()
        {
            return false;
        }
    }
}
