using Glass.PCP.Negocios.Entidades;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadSubtipoPerda : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdSubtipoPerda.Register(true, true);
            odsSubtipoPerda.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var novo = new SubtipoPerda();
                novo.IdTipoPerda = Request["idTipoPerda"].StrParaInt();
                novo.Descricao = (grdSubtipoPerda.FooterRow.FindControl("txtDescricao") as TextBox).Text;

                var perdaFluxo = ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IPerdaFluxo>();
                var resultado = perdaFluxo.SalvarSubtipoPerda(novo);

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir subtipo de perda.", resultado);
                else
                    grdSubtipoPerda.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir subtipo de perda.", ex, Page);
            }
        }
    }
}
