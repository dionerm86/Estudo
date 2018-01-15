using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstCorFerragem : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdCorFerragem.Register(true, true);
            odsCorFerragem.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var corFerragem = new Glass.Global.Negocios.Entidades.CorFerragem();
                corFerragem.Descricao = ((TextBox)grdCorFerragem.FooterRow.FindControl("txtDescricaoIns")).Text;
                corFerragem.Sigla = ((TextBox)grdCorFerragem.FooterRow.FindControl("txtSiglaIns")).Text;

                var coresFluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.ICoresFluxo>();

                var resultado = coresFluxo.SalvarCorFerragem(corFerragem);
                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Cor de Ferragem.", resultado);
                else
                    grdCorFerragem.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Cor de Ferragem.", ex, Page);
            }
        }
    }
}
