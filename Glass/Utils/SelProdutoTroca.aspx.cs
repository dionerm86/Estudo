using System;
using System.Web.UI;
using Glass.Data.Model;
using System.Collections.Generic;
using Glass.Configuracoes;
using Glass.Data.DAL;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class SelProdutoTroca : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                grdProdutos.Columns[1].Visible = PedidoConfig.DadosPedido.AmbientePedido;
                lblAmbiente.Visible = PedidoConfig.DadosPedido.AmbientePedido;
                txtAmbiente.Visible = PedidoConfig.DadosPedido.AmbientePedido;
                imgPesq2.Visible = PedidoConfig.DadosPedido.AmbientePedido;
                grdProdutos.Columns[1].Visible = PedidoDAO.Instance.PossuiVidrosEstoque(null, Request["idPedido"].StrParaUint());
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void lnkAddAll_Click(object sender, EventArgs e)
        {
            // Gera o script para adicionar todas elas na tela
            string script = string.Empty;

            foreach (GridViewRow row in grdProdutos.Rows)
            {
                if(row.RowType == DataControlRowType.DataRow)
                {
                    var idProdPed = ((HiddenField)row.FindControl("hdfIdProdPed")).Value.StrParaInt();
                    var qtde = ((Label)row.FindControl("lblQtde")).Text.StrParaDecimal();
                    var etiquetas = ((TextBox)row.FindControl("txtEtiquetas")).Text;

                    script += "window.opener.setProdutoTroca(" + idProdPed + ",'" + qtde + "','" + etiquetas + "');";
                }
            }

            script += "closeWindow()";
    
            ClientScript.RegisterStartupScript(typeof(string), "addAll", script, true);
        }
    }
}
