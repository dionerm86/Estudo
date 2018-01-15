using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Listas
{
    public partial class LstUnidadeMedida : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdUnidadeMedida.Register(true, true);
            odsUnidadeMedida.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var unidadeMedida = new Glass.Global.Negocios.Entidades.UnidadeMedida();
                unidadeMedida.Descricao = ((TextBox)grdUnidadeMedida.FooterRow.FindControl("txtDescricaoIns")).Text;
                unidadeMedida.Codigo = ((TextBox)grdUnidadeMedida.FooterRow.FindControl("txtCodigoIns")).Text;

                var fluxo = ServiceLocator.Current
                    .GetInstance<Glass.Global.Negocios.IUnidadesFluxo>();

                var resultado = fluxo.SalvarUnidadeMedida(unidadeMedida);

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir unidade de medida.", resultado);
                else
                    grdUnidadeMedida.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir unidade de medida.", ex, Page);
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdUnidadeMedida.PageIndex = 0;
        }
    }
}
