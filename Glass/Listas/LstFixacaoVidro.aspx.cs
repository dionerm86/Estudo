using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstFixacaoVidro : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdFixacaoVidro.Register(true, true);
            odsFixacaoVidro.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            var fixacaoVidro = new Instalacao.Negocios.Entidades.FixacaoVidro();
            fixacaoVidro.Descricao = ((TextBox)grdFixacaoVidro.FooterRow.FindControl("txtDescricaoIns")).Text;
            fixacaoVidro.Sigla = ((TextBox)grdFixacaoVidro.FooterRow.FindControl("txtSiglaIns")).Text;

            var instalcaoFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Instalacao.Negocios.IInstalacaoFluxo>();

            var resultado = instalcaoFluxo.SalvarFixacaoVidro(fixacaoVidro);
            
            if (resultado)
                grdFixacaoVidro.DataBind();
            else
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir Fixação.", resultado);
        }
    }
}
