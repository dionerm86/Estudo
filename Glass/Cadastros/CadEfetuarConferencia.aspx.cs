using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadEfetuarConferencia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadEfetuarConferencia));
        }
    
        [Ajax.AjaxMethod()]
        public string Confirmar(string idConferente, string idsPedido, string dataEfetuar, string retificar, string idsRetificar)
        {
            return WebGlass.Business.PedidoConferencia.Fluxo.Confirmar.Ajax.ConfirmarPedido(idConferente, idsPedido,
                dataEfetuar, retificar, idsRetificar);
        }
    
        [Ajax.AjaxMethod()]
        public string GetPedido(string idPedido)
        {
            return WebGlass.Business.Pedido.Fluxo.BuscarEValidar.Ajax.GetPedidoParaConferencia(idPedido);
        }
    }
}
