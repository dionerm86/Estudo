using System;
using System.Web.UI;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstPecasSemSaida : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Label10.Text = "Período " + (PedidoConfig.LiberarPedido ? "Lib." : "Conf.");
    
            /* filtros.Visible = txtNumCli.Text != "" || txtNome.Text != "";
            separador.Visible = filtros.Visible;
            lista.Visible = filtros.Visible; */
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProdutosPedido.PageIndex = 0;
        }
    
        protected bool EsconderColunas()
        {
            return Glass.Configuracoes.Geral.NaoVendeVidro();
        }
    }
}
