using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstFeriado : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdFeriado.Register(true, true);
            odsFeriado.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var feriado = new Glass.Global.Negocios.Entidades.Feriado();
    
                feriado.Descricao = ((TextBox)grdFeriado.FooterRow.FindControl("txtDescricaoIns")).Text;
                feriado.Dia = Convert.ToInt32(((TextBox)grdFeriado.FooterRow.FindControl("txtDiaIns")).Text);
                feriado.Mes = Convert.ToInt32(((TextBox)grdFeriado.FooterRow.FindControl("txtMesIns")).Text);

                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Global.Negocios.IDataFluxo>();

                var resultado = fluxo.SalvarFeriado(feriado);

                if (resultado)
                    grdFeriado.DataBind();
                else
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Feriado.", resultado);
                
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Feriado.", ex, Page);
            }
        }
    }
}
