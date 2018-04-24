using System;
using System.Web.UI;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SelInstalacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            Ajax.Utility.RegisterTypeForAjax(typeof(SelInstalacao));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdInstalacao.PageIndex = 0;
        }

        [Ajax.AjaxMethod()]
        public string VerificarPedidoJaFinalizadoPCP(string idPedidoStr)
        {
            return WebGlass.Business.Instalacao.Fluxo.BuscarEValidar.Ajax.VerificarPedidoJaFinalizadoPCP(idPedidoStr);
        }
    }
}
