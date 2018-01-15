using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaVendasSemImposto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (PedidoConfig.LiberarPedido)
                {
                    ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.ToString("01/MM/yyyy");
                    ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                    drpOrdenar.Items[0].Text = "Data Lib.";
                }
    
                grdVendasLucr.Columns[5].Visible = !PedidoConfig.LiberarPedido;
                grdVendasLucr.Columns[6].Visible = PedidoConfig.LiberarPedido;
    
                LoginUsuario login = UserInfo.GetUserInfo;
    
                if (login.TipoUsuario != (int)Data.Helper.Utils.TipoFuncionario.Administrador && login.TipoUsuario != (int)Data.Helper.Utils.TipoFuncionario.Gerente)
                {
                    drpLoja.DataBind();
                    drpLoja.SelectedValue = login.IdLoja.ToString();
                    drpLoja.Enabled = false;
    
                    drpVendedor.DataBind();
                    drpVendedor.SelectedValue = login.CodUser.ToString();
                    drpVendedor.Enabled = false;
                }
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdVendasLucr.PageIndex = 0;
        }
    }
}
