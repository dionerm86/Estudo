using System;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Listas
{
    public partial class LstBenef : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdBenef.Register();
            odsBenefConfig.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadBenef.aspx");
        }
    
        protected void grdBenef_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Up" || e.CommandName == "Down")
            {
                try
                {
                    var fluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IBeneficiamentoFluxo>();

                    var resultado = fluxo.AlterarPosicaoBenefConfig(e.CommandArgument.ToString().StrParaInt(), e.CommandName == "Up");

                    if (!resultado)
                        Glass.MensagemAlerta.ErrorMsg("Falha ao mudar posição do beneficiamento.", resultado); 
                    else
                        grdBenef.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao mudar posição do beneficiamento.", ex, Page);
                }
            }   
        }
    }
}
