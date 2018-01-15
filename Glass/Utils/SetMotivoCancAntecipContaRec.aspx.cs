using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancAntecipContaRec : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ctrlDataEstorno.Data = DateTime.Now;
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                uint idAntecipContaRec = Glass.Conversoes.StrParaUint(Request["idAntecipContaRec"]);
                
                if (idAntecipContaRec <= 0)
                    throw new Exception("Nenhum boleto encontrado");
    
                // Cancela a antecipação
                AntecipContaRecDAO.Instance.Cancelar(idAntecipContaRec, chkGerarEstorno.Checked, txtMotivo.Text, ctrlDataEstorno.DataNullable);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.location.href=window.opener.location.href;closeWindow();", true);
        }
    
        protected void chkGerarEstorno_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGerarEstorno.Checked)
                tbEstornoBanco.Visible = true;
            else
                tbEstornoBanco.Visible = false;
        }
    }
}
