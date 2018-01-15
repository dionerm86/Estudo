using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstPedidoInterno : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var isProducao = Request["producao"];

                if (isProducao == "1")
                {
                    lkbInserir.OnClientClick = "redirectUrl('../Cadastros/CadPedidoInterno.aspx?producao=1&popup=true'); return false";
                    lnkAutorizar.Visible = false;
                    lnkImprimir.Visible = false;
                    lnkExportarExcel.Visible = false;
                    ckbAgruparImpressao.Visible = false;
                }
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedidos.DataBind();
        }
    
        protected void grdPedidos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reabrir")
            {
                try
                {
                    uint idPedidoInterno = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    PedidoInternoDAO.Instance.Reabrir(idPedidoInterno);
                    Glass.MensagemAlerta.ShowMsg("Pedido reaberto!", Page);
                    grdPedidos.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao reabrir o pedido interno.", ex, Page);
                }
            }
        }
    
        protected bool ExibirAutorizar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuEstoque.AutorizarPedidoInterno);
        }
    }
}
