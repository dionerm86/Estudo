using Glass.PCP.Negocios.Entidades;
using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTipoPerda : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdTipoPerda.Register(true, true);
            odsTipoPerda.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var tipoPerda = new TipoPerda();
                tipoPerda.Descricao = ((TextBox)grdTipoPerda.FooterRow.FindControl("txtDescricao")).Text;

                var perdaFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.PCP.Negocios.IPerdaFluxo>();

                var resultado = perdaFluxo.SalvarTipoPerda(tipoPerda);
                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir tipo de perda.", resultado);
    
                else
                    grdTipoPerda.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir tipo de perda.", ex, Page);
            }
        }
    }
}
