using System;
using System.Web.UI;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaEstoqueProdutos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            grdProduto.Columns[4].Visible = PedidoConfig.LiberarPedido;
    
            if (PedidoConfig.LiberarPedido)
                Page.Title = "Produtos em Reserva/Liberação";
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void drpGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void drpSubgrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    }
}
