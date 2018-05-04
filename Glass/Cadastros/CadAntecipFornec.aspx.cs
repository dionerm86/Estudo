using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadAntecipFornec : System.Web.UI.Page
    {
        #region Loads
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack && Request["idAntecipFornec"] != null)
                dtvAntecipFornec.ChangeMode(DetailsViewMode.ReadOnly);
    
            if (dtvAntecipFornec.CurrentMode == DetailsViewMode.Insert)
            {
                ((HiddenField)dtvAntecipFornec.FindControl("hdfIdFunc")).Value = UserInfo.GetUserInfo.CodUser.ToString();
                ((HiddenField)dtvAntecipFornec.FindControl("hdfSituacao")).Value = "1";
            }
        }
    
        protected void drpParcCredito_Load(object sender, EventArgs e)
        {
            ((DropDownList)sender).Visible = FinanceiroConfig.Cartao.PedidoJurosCartao;
        }
    
        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlParcelas ctrlParcelas = (Glass.UI.Web.Controls.ctrlParcelas)sender;
    
            ctrlParcelas.CampoCalcularParcelas = (HiddenField)dtvAntecipFornec.FindControl("hdfCalcularParcelas");
            ctrlParcelas.CampoParcelasVisiveis = (DropDownList)dtvAntecipFornec.FindControl("drpNumParcelas");
            ctrlParcelas.CampoValorTotal = dtvAntecipFornec.FindControl("hdfTotalAntecipFornec");
        }
    
        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlFormaPagto ctrlFormaPagto = (Glass.UI.Web.Controls.ctrlFormaPagto)sender;
    
            ctrlFormaPagto.CampoClienteID = dtvAntecipFornec.FindControl("lblIdFornec");
            ctrlFormaPagto.CampoCredito = dtvAntecipFornec.FindControl("hdfCreditoFornec");
            ctrlFormaPagto.CampoValorConta = dtvAntecipFornec.FindControl("hdfTotalAntecipFornec");
            ctrlFormaPagto.ExibirDataRecebimento = false;
            ctrlFormaPagto.ExibirGerarCredito = false;
            ctrlFormaPagto.ExibirUsarCredito = false;
        }
    
        #endregion
    
        #region Clicks
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstAntecipFornec.aspx");
        }
    
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (UserInfo.GetUserInfo.IsFinanceiroPagto)
                ExibirPagamento(true);
        }
    
        protected void btnCancelarPagamento_Click(object sender, EventArgs e)
        {
            ExibirPagamento(false);
        }
    
        protected void btnPagar_Click(object sender, EventArgs e)
        {
            try
            {
                var drpTipoPagto = dtvAntecipFornec.FindControl("drpTipoPagto") as DropDownList;
                var antecip = AntecipacaoFornecedorDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idAntecipFornec"]));
                var retorno = "";
    
                if (drpTipoPagto.SelectedValue == "1")
                {
                    // À vista
                    var ctrlFormaPagto1 = dtvAntecipFornec.FindControl("ctrlFormaPagto1") as Glass.UI.Web.Controls.ctrlFormaPagto;
                    antecip.ValoresPagto = ctrlFormaPagto1.Valores;
                    antecip.FormasPagto = ctrlFormaPagto1.FormasPagto;
                    antecip.TiposCartaoPagto = ctrlFormaPagto1.TiposCartao;
                    antecip.ParcelasCartaoPagto = ctrlFormaPagto1.ParcelasCartao;
                    antecip.ContasBancoPagto = ctrlFormaPagto1.ContasBanco;
                    antecip.ChequesPagto = ctrlFormaPagto1.ChequesString;
                    antecip.CreditoUtilizado = ctrlFormaPagto1.CreditoUtilizado;
                    antecip.DataRecebimento = ctrlFormaPagto1.DataRecebimento;
                    antecip.NumAutCartao = ctrlFormaPagto1.NumAutCartao;
    
                    retorno = AntecipacaoFornecedorDAO.Instance.PagamentoVista(antecip);
                }
                else
                {
                    // À prazo
                    var drpNumParcelas = dtvAntecipFornec.FindControl("drpNumParcelas") as DropDownList;
                    antecip.NumParcelas = Glass.Conversoes.StrParaInt(drpNumParcelas.SelectedValue);
    
                    var ctrlParcelas1 = dtvAntecipFornec.FindControl("ctrlParcelas1") as Glass.UI.Web.Controls.ctrlParcelas;
                    antecip.DatasParcelas = ctrlParcelas1.Datas;
                    antecip.ValoresParcelas = ctrlParcelas1.Valores;
    
                    retorno = AntecipacaoFornecedorDAO.Instance.PagamentoPrazo(antecip);
                }
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "pago", "alert('" + retorno + "'); redirectUrl('../Listas/LstAntecipFornec.aspx');\n", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao efetuar antecipação de pagto. fornecedor.", ex, Page);
            }
        }
    
        #endregion
    
        protected void odsAntecipFornec_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                Response.Redirect("~/Cadastros/CadAntecipFornec.aspx?idAntecipFornec=" + e.ReturnValue);
            }
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Erro ao inserir pagamento antecipado.", e.Exception.InnerException, this);
                e.ExceptionHandled = true;
            }
        }
    
        protected void odsAntecipFornec_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                Response.Redirect(Request.Url.ToString());
            }
            else
            {
                Glass.MensagemAlerta.ErrorMsg("Erro ao atualizar pagamento antecipado.", e.Exception.InnerException, this);
                e.ExceptionHandled = true;
            }
        }
    
        private void ExibirPagamento(bool exibir)
        {
            var valorObra = AntecipacaoFornecedorDAO.Instance.GetValorAntecip(Glass.Conversoes.StrParaUint(Request["idAntecipFornec"]));
            if (exibir && valorObra == 0)
            {
                Glass.MensagemAlerta.ShowMsg("O valor do pagamento antecipado não pode ser zero.", Page);
                return;
            }
    
            var btnEditar = dtvAntecipFornec.FindControl("btnEditar") as Button;
            var btnFinalizar = dtvAntecipFornec.FindControl("btnFinalizar") as Button;
            var btnVoltar = dtvAntecipFornec.FindControl("btnVoltar") as Button;
            var btnPagar = dtvAntecipFornec.FindControl("btnPagar") as Button;
            var btnCancelar = dtvAntecipFornec.FindControl("btnCancelar") as Button;
            var panPagar = dtvAntecipFornec.FindControl("panPagar") as Panel;
    
            btnEditar.Visible = !exibir;
            btnFinalizar.Visible = !exibir;
            btnVoltar.Visible = !exibir;
            btnPagar.Visible = exibir;
            btnCancelar.Visible = exibir;
            panPagar.Visible = exibir;
    
            if (exibir)
                ((HiddenField)dtvAntecipFornec.FindControl("hdfTotalAntecipFornec")).Value = valorObra.ToString();
        }
    }
}
