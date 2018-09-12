using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstNotaFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["erroNf"] != null)
            {
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "erroNf", "alert('O arquivo dessa nota fiscal não foi encontrado.')", true);
            }

            if (this.Request["autorizada"] != null)
            {
                if (this.Request["autorizada"] == "true")
                {
                    MensagemAlerta.ShowMsg("Nota autorizada.", this.Page);
                }
                else
                {
                    MensagemAlerta.ShowMsg(this.Request["autorizada"], this.Page);
                }
            }
        }
    }
}
