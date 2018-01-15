using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Sync.Controls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaPedidos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            grdPedido.Columns[12].Visible = PCPConfig.ControlarProducao ||
                Geral.ControleInstalacao;

            if (Request["prod"] == "1")
            {
                lblTipoPedido.Visible = false;
                cblTipoPedido.Style.Add("display", "none");
                grdPedido.Columns[17].Visible = false;
                Page.Title = "Pedidos em Produção";
            }

            if (!PedidoConfig.Pedido_FastDelivery.FastDelivery)
            {
                lblFastDelivery.Visible = false;
                drpFastDelivery.Visible = false;
                grdPedido.Columns[18].Visible = false;
            }

            Page.ClientScript.RegisterOnSubmitStatement(GetType(), "submit", "FindControl('hdfBenef', 'input').value = " + ctrlBenefSetor1.ClientID + ".Selecionados();");

            LoginUsuario login = UserInfo.GetUserInfo;

            uint tipoUsuario = UserInfo.GetUserInfo.TipoUsuario;
            lnkImprimir.Visible = Config.PossuiPermissao(Config.FuncaoMenuRelatorios.ImprimirRelatorioPedidos);
            lnkExportarExcel.Visible = lnkImprimir.Visible;
            lblAgrupar.Visible = lnkImprimir.Visible;
            cbdAgrupar.Visible = lnkImprimir.Visible;

            if (!String.IsNullOrEmpty(hdfCidade.Value))
                txtCidade.Text = CidadeDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(hdfCidade.Value));

            if (!IsPostBack)
            {
                grdPedido.Columns[9].Visible = false;

                if (Request["prod"] == "1")
                {
                    chkFiltroPronto.Style.Add("display", "none");
                    ImageButton3.Visible = false;
                }

                if (!Glass.Configuracoes.PedidoConfig.TelaListagemRelatorio.ExibirCampoProdutosRelatorio)
                    chkExibirProdutos.Style.Add("display", "none");
            }

            if (!PCPConfig.ControlarProducao)
            {
                lnkExportarExcelSimples.Visible = false;
                lnkImprimirSimples.Visible = false;
            }

            var pedidoProntoNaoEntregue = Request["pedidosProntosNaoEntregues"];

            if (!IsPostBack && pedidoProntoNaoEntregue != null && pedidoProntoNaoEntregue.ToLower() == "true")
            {
                chkFiltroPronto.Checked = true;
                grdPedido.Columns[11].Visible = filtroPronto.Visible;
                grdPedido.Columns[12].Visible = filtroPronto.Visible;
                ((TextBox)ctrlDataProntoFim.FindControl("txtData")).Text = DateTime.Now.AddDays(-30).ToShortDateString();
                cbdSituacaoProd.SelectedValue = "3";
            }

            filtroPronto.Visible = chkFiltroPronto.Checked;
            grdPedido.Columns[11].Visible = filtroPronto.Visible;
            grdPedido.Columns[12].Visible = filtroPronto.Visible;
            grdPedido.Columns[8].Visible = PCPConfig.ExibirCustoVendaRelatoriosProducao;
        }

        protected void lblSitProd_Load(object sender, EventArgs e)
        {
            // Mostra o link para visualizar as peças deste pedido na produção se a situação não for "-"
            // e se a empresa controla produção
            // TODO: O texto está vindo vazio
            if (((WebControl)sender).ID == "lnkSitProd")
                ((WebControl)sender).Visible = ((LinkButton)sender).Text != "-" && PCPConfig.ControlarProducao;
            else
                ((WebControl)sender).Visible = ((Label)sender).Text == "-" || !PCPConfig.ControlarProducao;
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
            dtvTotaisPedidos.PageIndex = 0;
        }

        protected void lnkLimparFiltros_Click(object sender, EventArgs e)
        {
            Response.Redirect(this.Request.Url.AbsoluteUri);
        }

        protected void grdPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Producao")
            {
                Response.Redirect("~/Cadastros/Producao/LstProducao.aspx?idPedido=" + e.CommandArgument);
            }
        }

        protected string ExibirInstalacao()
        {
            return Geral.ControleInstalacao ? "" : "display: none";
        }

        protected void cblTipoVenda_DataBound(object sender, EventArgs e)
        {
            // Define como filtro padrão pedidos À Vista e À Prazo
            foreach (ListItem li in ((CheckBoxListDropDown)sender).Items)
            {
                switch (Glass.Conversoes.StrParaUint(li.Value))
                {
                    case (uint)Glass.Data.Model.Pedido.TipoVendaPedido.AVista:
                    case (uint)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo:
                    case (uint)Glass.Data.Model.Pedido.TipoVendaPedido.Obra:
                        li.Selected = true;
                        break;
                }
            }
        }

        protected void cblTipoPedido_DataBound(object sender, EventArgs e)
        {
            // Define como filtro padrão pedidos de Venda/Revenda e Mão de Obra
            foreach (ListItem li in ((CheckBoxListDropDown)sender).Items)
            {
                switch (Glass.Conversoes.StrParaUint(li.Value))
                {
                    case (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Venda:
                    case (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.Revenda:
                    case (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObra:
                    case (uint)Glass.Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial:
                        li.Selected = true;
                        break;
                }
            }
        }

        protected void drpSubgrupo_DataBound(object sender, EventArgs e)
        {
        }

        protected void drpSubgrupo0_DataBound(object sender, EventArgs e)
        {
            if (cbdGrupo.SelectedValue.IndexOf(',') > -1)
            {
                drpSubgrupo.SelectedValue = "0";
                drpSubgrupo.Enabled = false;
            }
            else
                drpSubgrupo.Enabled = true;
        }

        protected void chkMostrarDescontoTotal_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkMostrarDescontoTotal = (CheckBox)sender;

            grdPedido.Columns[9].Visible = chkMostrarDescontoTotal.Checked;
        }
    }
}
