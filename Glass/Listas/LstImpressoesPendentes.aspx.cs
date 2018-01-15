using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstImpressoesPendentes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Configuracoes.PCPConfig.BuscarProdutoPedidoAssociadoAoIdLojaFuncionarioAoBuscarProdutos && !Data.Helper.UserInfo.GetUserInfo.IsAdministrador)
            {
                drpLoja.Enabled = false;
                drpLoja.SelectedValue = Data.Helper.UserInfo.GetUserInfo.IdLoja.ToString();
            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    }
}
