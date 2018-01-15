using System;
using System.Linq;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCavalete : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdCavalete.Register(true, true);
            odsCavalete.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var cavalete = new PCP.Negocios.Entidades.Cavalete()
                {
                    CodInterno = ((TextBox)grdCavalete.FooterRow.FindControl("txtCodInterno")).Text,
                    Localizacao = ((TextBox)grdCavalete.FooterRow.FindControl("txtLocalizacao")).Text
                };

                var resultado = ServiceLocator.Current.GetInstance<PCP.Negocios.ICavaleteFluxo>()
                    .SalvarCavalete(cavalete);

                if (resultado)
                    grdCavalete.DataBind();
                else
                    MensagemAlerta.ErrorMsg("Falha ao inserir Cavalete.", resultado);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao inserir Cavalete.", ex, Page);
            }
        }
    }
}
