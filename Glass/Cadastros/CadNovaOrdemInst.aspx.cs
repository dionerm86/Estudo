using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadNovaOrdemInst : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadNovaOrdemInst));
        }
    
        [Ajax.AjaxMethod()]
        public string GetInstByPedido(string idPedidoStr, string noCache)
        {
            return WebGlass.Business.Instalacao.Fluxo.BuscarEValidar.Ajax.GetInstByPedido(idPedidoStr, noCache);
        }
    
        [Ajax.AjaxMethod()]
        public string Confirmar(string idsInstalacao, string idsEquipe, string tipoInstalacao, string dataInstalacao, 
            string produtos, string obs, string noCache)
        {
            return WebGlass.Business.Instalacao.Fluxo.OrdemInstalacao.Ajax.Nova(idsInstalacao, idsEquipe, tipoInstalacao,
                dataInstalacao, produtos, obs, noCache);
        }
    
        [Ajax.AjaxMethod]
        public string GetAmbientes(string idPedido, string linhaAmbiente)
        {
            return WebGlass.Business.AmbientePedidoEspelho.Fluxo.BuscarEValidar.Ajax.GetAmbientesOrdemInst(idPedido, linhaAmbiente);
        }
    }
}
