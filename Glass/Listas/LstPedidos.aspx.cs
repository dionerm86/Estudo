using Glass.Configuracoes;
using Glass.Data.Helper;
using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstPedidos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstPedidos));

            if (this.Request["maoObra"] == "1")
            {
                this.Page.Title = "Pedidos Mão de Obra";
            }
            else if (this.Request["producao"] == "1")
            {
                this.Page.Title = "Pedidos para Produção";
            }
            else
            {
                this.Page.Title = "Pedidos";
            }

            if (this.Request["byVend"] == "1")
            {
                this.Page.Title = "Meus " + this.Page.Title;
            }

            if (!this.IsPostBack)
            {
                uint tipoUsuario = UserInfo.GetUserInfo.TipoUsuario;

                if (PedidoConfig.DadosPedido.ListaApenasPedidosVendedor
                    && tipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Vendedor
                    && this.Request["byVend"] != "1")
                {
                    var requestQuery = this.Request.Url.ToString().Contains("?")
                        ? this.Request.Url.ToString().Substring(this.Request.Url.ToString().IndexOf('?'))
                        : string.Empty;

                    if (string.IsNullOrEmpty(requestQuery))
                    {
                        requestQuery += "?byVend=1";
                    }
                    else
                    {
                        requestQuery += requestQuery.Contains("byVend")
                            ? requestQuery.Replace("byVend=" + this.Request["byVend"], "byVend=1")
                            : "&byVend=1";
                    }

                    this.Response.Redirect("LstPedidos.aspx" + requestQuery);
                }
            }
        }
    }
}
