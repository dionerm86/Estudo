using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstCorAluminio : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdCorAluminio.Register(true, true);
            odsCorAluminio.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            try
            {
                var corAluminio = new Glass.Global.Negocios.Entidades.CorAluminio();
                corAluminio.Descricao = ((TextBox)grdCorAluminio.FooterRow.FindControl("txtDescricaoIns")).Text;
                corAluminio.Sigla = ((TextBox)grdCorAluminio.FooterRow.FindControl("txtSiglaIns")).Text;

                var corFluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.ICoresFluxo>();

                var resultado = corFluxo.SalvarCorAluminio(corAluminio);

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Cor de Alumínio.", resultado);
                else
                    grdCorAluminio.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Cor de Alumínio.", ex, Page);
            }
        }
       
    }
}
