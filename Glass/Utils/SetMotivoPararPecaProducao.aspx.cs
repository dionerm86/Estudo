using Glass.Data.DAL;
using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoPararPecaProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var idProdPedProducao = Glass.Conversoes.StrParaUint(Request["idProdPedProducao"]);
            if (idProdPedProducao == 0)
                return;

            using (var dao = ProdutoPedidoProducaoDAO.Instance)
            {
                string etiqueta = dao.ObtemEtiqueta(idProdPedProducao);
                var parada = dao.VerificaPecaProducaoParada(etiqueta).Split(';');

                if (bool.Parse(parada[0]))
                    Page.Title = "Retornar peça para produção";
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                var idProdPedProducao = Glass.Conversoes.StrParaUint(Request["idProdPedProducao"]);
                ProdutoPedidoProducaoDAO.Instance.PararRetornarPecaProducao(idProdPedProducao, txtMotivo.Text);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }

            if (Request["vue"] == "true")
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.app.__vue__.atualizarPecas(); closeWindow();", true);
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
            }
        }
    }
}
