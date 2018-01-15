using System;
using Glass.Data.DAL;
using GDA;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancArquivoCartaoNaoIdentificado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            var idArquivoCartaoNaoIdentificado = Conversoes.StrParaInt(Request["IdCartaoNaoIdentificado"]);

            var fluxoACNI = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Financeiro.Negocios.IArquivoCartaoNaoIdentificadoFluxo>();

            var resultado = fluxoACNI
                .CancelarArquivoCartaoNaoIdentificado(fluxoACNI.ObterArquivoCartaoNaoIdentificado(idArquivoCartaoNaoIdentificado), txtMotivo.Text);

            if (!resultado)
                MensagemAlerta.ErrorMsg("Erro ao cancelar Arquivo", resultado);
            else
                MensagemAlerta.ShowMsg("Arquivo cancelado.", this);

            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }

    }
}
