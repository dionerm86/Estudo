using System;
using System.Web.UI;
using Glass.Data.DAL;

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
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }
    }
}
