using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstCompras : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(LstCompras));
    
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
                lnkInserir.Visible = false;
    
            chkEmAtraso.Visible = FinanceiroConfig.Compra.UsarControleFinalizacaoCompra;
            chkCentroCustoDivergente.Visible = FiscalConfig.UsarControleCentroCusto && CentroCustoDAO.Instance.GetCountReal() > 0;
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadCompra.aspx");
        }
    
        protected void odsCompra_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void grdCompra_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reabrir")
            {
                try
                {
                    CompraDAO.Instance.ReabrirCompra(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
                    grdCompra.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ShowMsg(ex.Message, Page);
                    return;
                }
            }
            else if (e.CommandName == "Finalizar")
            {
                 uint idCompra = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                 CompraDAO.Instance.FinalizarCompraComTransacao(idCompra);
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCompra.PageIndex = 0;
        }
    
        protected void drpSituacao_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                drpSituacao.Items[2].Enabled =
                    FinanceiroConfig.Compra.UsarControleCompraContabilNF ||
                    FinanceiroConfig.Compra.UsarControleFinalizacaoCompra ||
                    CompraDAO.Instance.TemCompraEmAndamentoAguardandoEntrega();

                drpSituacao.Items[3].Enabled = drpSituacao.Items[2].Enabled;
            }
        }
    
        protected string GetEntregaPedido()
        {
            bool exibir = Glass.Configuracoes.CompraConfig.TelaListagem.ExibirDadosAdicionaisPedido;
            grdCompra.Columns[9].Visible = exibir;
            grdCompra.Columns[10].Visible = exibir;
    
            return exibir ? "" : "display: none";
        }

        protected void drpSubgrupo_DataBound(object sender, EventArgs e)
        {
            if (cbdGrupo.SelectedValue.IndexOf(',') > -1)
            {
                drpSubgrupo.SelectedValue = "0";
                drpSubgrupo.Enabled = false;
            }
            else
                drpSubgrupo.Enabled = true;
        }

        [Ajax.AjaxMethod]
        public string ObtemIdLoja(string idCompra)
        {
            return CompraDAO.Instance.ObtemIdLoja(idCompra.StrParaUint()).ToString();
        }
    }
}
