using System;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadClienteMulti : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadClienteMulti));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCli.PageIndex = 0;
        }
    
        [Ajax.AjaxMethod]
        public string IgnorarBloqueioPedPronto(string chaves, string ignorar)
        {
            try
            {
                ClienteDAO.Instance.IgnorarBloqueioPedidoPronto(chaves,bool.Parse(ignorar));
                return "Operação efetuada com sucesso";
            }
            catch(Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }
    
         [Ajax.AjaxMethod]
        public string BloquearPedidoContaVencida(string chaves, string bloquear)
        {
            try
            {
                ClienteDAO.Instance.BloquearPedidoContaVencida(chaves,bool.Parse(bloquear));
                return "Operação efetuada com sucesso";
            }
            catch(Exception ex)
            {
                return "Erro: " + ex.Message;
            }
        }
    
         [Ajax.AjaxMethod]
         public string GetData(string rota, string revenda)
         {
             try
             {
                 Cliente[] clientes = ClienteDAO.Instance.ObterClientesRotaRevenda(rota, revenda);
    
                 string valor = "";
    
                 foreach (Cliente c in clientes)
                 {
                     valor += c.IdCli + ",";
                 }
    
                 return valor.Remove(valor.LastIndexOf(','));
             }
             catch (Exception ex)
             {
                 return ex.Message;
             }
         }
    }
}
