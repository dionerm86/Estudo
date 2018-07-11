using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;

namespace Glass.UI.Web.Utils
{
    public partial class SelPedidoEspelhoCompraProdBenef : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SelPedidoEspelhoCompraProdBenef));
        }
    
        #region M�todos Ajax
    
        /// <summary>
        /// Gera a compra dos produtos dos beneficiamentos dos pedidos selecionados.
        /// </summary>
        /// <param name="idsPedido"></param>
        /// <returns>Retorna os pedidos que geraram e os pedidos que n�o geraram compra.</returns>
        [Ajax.AjaxMethod()]
        public string GerarCompraProdBenef(string idsPedido, string idFornecedor)
        {
            // M�todo que gera a compra dos produtos associados aos itens de beneficiamento.
            return Glass.Data.DAL.CompraDAO.Instance.GerarCompraProdBenef(idsPedido, Glass.Conversoes.StrParaUint(idFornecedor));
        }
        
        /// <summary>
        /// M�todo ajax que valida se foram selecionados pedidos de lojas diferentes.
        /// </summary>
        /// <param name="idsPedido">Ids dos pedidos separados por v�rgula.</param>
        /// <returns>Retorna true caso todos os pedidos sejam da mesma loja ou false
        /// caso existam pedidos selecionados que s�o de lojas diferentes.</returns>
        [Ajax.AjaxMethod()]
        public string ValidaLojaPedidos(string idsPedido)
        {
            // Caso somente um pedido tenha sido selecionado ent�o n�o � necess�rio fazer a compara��o de lojas.
            if (idsPedido.Split(',').Length == 1)
                return "Ok;";
    
            var idLoja = (uint)0;
    
            // Verifica o id da loja de cada pedido.
            foreach (var id in idsPedido.Split(','))
            {
                // Caso o id da loja esteja zerado significa que � a primeira vez que a estrutura de repeti��o est� sendo executada
                // ent�o o id da loja deve ser salvo para ser feita a compara��o.
                if (idLoja == 0)
                    idLoja = Glass.Data.DAL.PedidoDAO.Instance.ObtemIdLoja(null, Glass.Conversoes.StrParaUint(id));
                // Caso os pedidos possuam lojas diferentes ent�o � retornada uma mensagem de erro.
                else if (idLoja != Glass.Data.DAL.PedidoDAO.Instance.ObtemIdLoja(null, Glass.Conversoes.StrParaUint(id)))
                    return "Erro;Selecione somente pedidos da mesma loja.";
            }
    
            return "Ok;";
        }
    
        #endregion
    
        protected void chkImprimir_DataBinding(object sender, EventArgs e)
        {
            var linha = ((CheckBox)sender).Parent.Parent as GridViewRow;
            if (linha == null)
                return;
    
            var pe = linha.DataItem as PedidoEspelho;
            if (pe == null)
                return;
    
            ((CheckBox)sender).Attributes.Add("IdPedido", pe.IdPedido.ToString());
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    }
}
