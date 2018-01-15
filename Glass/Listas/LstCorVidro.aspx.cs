using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstCorVidro : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdCorVidro.Register(true, true);
            odsCorVidro.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var corVidro = new Glass.Global.Negocios.Entidades.CorVidro();
                corVidro.Descricao = ((TextBox)grdCorVidro.FooterRow.FindControl("txtDescricaoIns")).Text;
                corVidro.Sigla = ((TextBox)grdCorVidro.FooterRow.FindControl("txtSiglaIns")).Text;

                var coresFluxos = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.ICoresFluxo>();

                var resultado = coresFluxos.SalvarCorVidro(corVidro);
                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Cor.", resultado);
                else
                    grdCorVidro.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Cor.", ex, Page);
            }
        }
    }
}
