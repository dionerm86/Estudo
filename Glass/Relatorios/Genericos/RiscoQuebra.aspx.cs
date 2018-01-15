using Glass.Data.DAL;
using System;

namespace Glass.UI.Web.Relatorios.Genericos
{
    public partial class RiscoQuebra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.Genericos.RiscoQuebra));
        }

        [Ajax.AjaxMethod()]
        public string ValidaDados(string idPed)
        {
            uint idPedido = idPed != "0" && idPed != "" ? Glass.Conversoes.StrParaUint(idPed) : 0;

            // Verifica se pedido passado existe
            if (!PedidoDAO.Instance.PedidoExists(idPedido))
            {
                return "O pedido informado não existe.";
            }
            return string.Empty;
        }
    }
}
