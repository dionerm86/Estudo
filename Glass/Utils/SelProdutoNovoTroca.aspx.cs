using System;
using System.Web.UI;
using Glass.Data.Model;
using System.Collections.Generic;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SelProdutoNovoTroca : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                grdProdutos.Columns[1].Visible = PedidoConfig.DadosPedido.AmbientePedido;
                lblAmbiente.Visible = PedidoConfig.DadosPedido.AmbientePedido;
                txtAmbiente.Visible = PedidoConfig.DadosPedido.AmbientePedido;
                imgPesq2.Visible = PedidoConfig.DadosPedido.AmbientePedido;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void lnkAddAll_Click(object sender, EventArgs e)
        {
            // Busca as contas a receber que estiverem na tela
            var lstProdutosPedido = odsProdutoPedido.Select() as List<ProdutosPedido>;
    
            // Gera o script para adicionar todas elas na tela
            string script = String.Empty;
    
            foreach (ProdutosPedido prodPed in lstProdutosPedido)
                script += "window.opener.setProdutoTroca(" + prodPed.IdProdPed + ",'" + prodPed.Qtde + "');";
    
            script += "closeWindow()";
    
            ClientScript.RegisterStartupScript(typeof(string), "addAll", script, true);
        }
    }
}
