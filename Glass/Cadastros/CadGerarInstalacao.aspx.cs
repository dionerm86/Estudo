using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadGerarInstalacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadGerarInstalacao));
        }
    
        protected void btnGerar_Click(object sender, EventArgs e)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(txtPedido.Text);
                int tipoInstalacao = Glass.Conversoes.StrParaInt(drpTipoInstalacao.SelectedValue);
    
                WebGlass.Business.Instalacao.Fluxo.Gerar.Instance.GerarInstalacao(idPedido, tipoInstalacao);
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "novaInstalacao", "alert('Instalação gerada com sucesso!'); " +
                    "redirectUrl('../Listas/LstInstalacao.aspx?idPedido=" + idPedido + "&situacao=1');\n", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar instalação.", ex, Page);
            }
        }
    
        [Ajax.AjaxMethod]
        public string ValidarPedido(string idPedidoStr)
        {
            return WebGlass.Business.Instalacao.Fluxo.BuscarEValidar.Ajax.ValidarPedido(idPedidoStr);
        }
    
        [Ajax.AjaxMethod]
        public string VerificarInstalacao(string idPedidoStr, string tipoInstalacaoStr)
        {
            return WebGlass.Business.Instalacao.Fluxo.BuscarEValidar.Ajax.VerificarInstalacao(idPedidoStr, tipoInstalacaoStr);
        }

        [Ajax.AjaxMethod]
        public string VerificarPedidoJaInstalado(string idPedidoStr)
        {
            return WebGlass.Business.Instalacao.Fluxo.BuscarEValidar.Ajax.VerificarPedidoJaInstalado(idPedidoStr);
        }
    }
}
