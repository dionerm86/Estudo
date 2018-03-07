using System;
using System.Web.UI;
using System.Text;
using System.Linq;
using Glass.Data.Model;

namespace Glass.UI.Web.Utils
{
    public partial class SelPedidoEspelhoImpressaoEtiqueta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request["idCorVidro"]))
                {
                    drpCorVidro.DataBind();
                    drpCorVidro.SelectedValue = Request["idCorVidro"];
                }
    
                if (!String.IsNullOrEmpty(Request["espessura"]))
                    txtEspessura.Text = Request["espessura"];
    
                if (!String.IsNullOrEmpty(Request["idSubgrupoProd"]))
                {
                    drpSubgrupoProd.DataBind();
                    drpSubgrupoProd.SelectedValue = Request["idSubgrupoProd"];
                }
    
                if (!String.IsNullOrEmpty(Request["alturaMin"]))
                    txtAlturaMin.Text = Request["alturaMin"];
    
                if (!String.IsNullOrEmpty(Request["alturaMax"]))
                    txtAlturaMax.Text = Request["alturaMax"];
    
                if (!String.IsNullOrEmpty(Request["larguraMin"]))
                    txtLarguraMin.Text = Request["larguraMin"];
    
                if (!String.IsNullOrEmpty(Request["larguraMax"]))
                    txtLarguraMax.Text = Request["larguraMax"];
            }

            if (Configuracoes.PCPConfig.BuscarProdutoPedidoAssociadoAoIdLojaFuncionarioAoBuscarProdutos && !Data.Helper.UserInfo.GetUserInfo.IsAdministrador)
            {
                drpLoja.Enabled = false;
                drpLoja.SelectedValue = Data.Helper.UserInfo.GetUserInfo.IdLoja.ToString();
            }
        }
        
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        protected void lnkAddAll_Click(object sender, EventArgs e)
        {
            var itens = odsPedidoEspelho.Select().Cast<PedidoEspelho>();
    
            if (itens.Count() == 0)
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "adicionar",
                    "alert('Não há itens para ser adicionados');", true);
    
                return;
            }
    
            StringBuilder sb = new StringBuilder("bloquearPagina(); ");
    
            foreach (var item in itens)
                sb.AppendFormat("adicionar({0}, false); ", item.IdPedido);
    
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "adicionar", "$(document).ready(function() { " +
                sb.ToString() + "desbloquearPagina(false); closeWindow(); }, 100);", true);
        }
    }
}
