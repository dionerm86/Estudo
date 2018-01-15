using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancCompra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            Compra comp = CompraDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["IdCompra"]));
    
            // Concatena a observação da compra já existente com o motivo do cancelamento
            string motivo = "Motivo do Cancelamento: " + txtMotivo.Text;
            comp.Obs = !String.IsNullOrEmpty(comp.Obs) ? comp.Obs + " " + motivo : motivo;
    
            // Se o tamanho do campo Obs exceder 300 caracteres, salva apenas os 300 primeiros, descartando o restante
            comp.Obs = comp.Obs.Length > 300 ? comp.Obs.Substring(0, 300) : comp.Obs;
    
            try
            {
                CompraDAO.Instance.CancelarCompra(comp.IdCompra, comp.Obs);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }
    }
}
