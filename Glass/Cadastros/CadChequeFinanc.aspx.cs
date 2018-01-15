using System;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadChequeFinanc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["IdCheque"] != null)
                dtvCheque.ChangeMode(DetailsViewMode.Edit);
    
            if (!IsPostBack)
                ((TextBox)dtvCheque.Rows[0].Cells[0].FindControl("txtTitular")).Focus();
    
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstChequeFinanc.aspx");
        }
    
        protected void odsCheque_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir cheque.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                var chkMovCaixaGeral = dtvCheque.FindControl("chkMovCaixaGeral") as CheckBox;
                var chkMovBanco = dtvCheque.FindControl("chkMovBanco") as CheckBox;
                var drpPlanoConta = dtvCheque.FindControl("drpPlanoConta") as DropDownList;
                var drpContaBanco = dtvCheque.FindControl("drpContaBanco") as DropDownList;
    
                var idCheque = Glass.Conversoes.StrParaUint(e.ReturnValue.ToString());
                var movCaixaGeral = chkMovCaixaGeral != null && chkMovCaixaGeral.Checked;
                var movContaBanco = chkMovBanco != null && chkMovBanco.Checked;
                var idConta = drpPlanoConta != null && !string.IsNullOrEmpty(drpPlanoConta.SelectedValue) ? Glass.Conversoes.StrParaUint(drpPlanoConta.SelectedValue) : 0;
                uint? idContaBanco = drpContaBanco != null && !string.IsNullOrEmpty(drpContaBanco.SelectedValue) ? (uint?)Glass.Conversoes.StrParaUint(drpContaBanco.SelectedValue) : null;
    
                WebGlass.Business.Cheque.Fluxo.Cheque.Instance.ChequeInserted(idCheque, movCaixaGeral, 
                    movContaBanco, idConta, idContaBanco);
    
                Response.Redirect("../Listas/LstChequeFinanc.aspx");
            }
        }
    
        protected void odsCheque_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do cheque.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("../Listas/LstChequeFinanc.aspx");
        }
    
        protected void ctrValor_PreRender(object sender, EventArgs e)
        {
            HiddenField hdfMovCaixaFinanceiro = dtvCheque.FindControl("hdfMovCaixaFinanceiro") as HiddenField;
            bool isMovCaixa = hdfMovCaixaFinanceiro != null && bool.Parse(hdfMovCaixaFinanceiro.Value);
    
            HiddenField hdfMovBanco = dtvCheque.FindControl("hdfMovBanco") as HiddenField;
            bool isMovBanco = hdfMovBanco != null && bool.Parse(hdfMovBanco.Value);
    
            Glass.UI.Web.Controls.ctrlTextBoxFloat ctrValor = sender as Glass.UI.Web.Controls.ctrlTextBoxFloat;
            
            TextBox texto = ctrValor.FindControl("txtNumber") as TextBox;
            if (texto != null)
                texto.Enabled = !isMovCaixa && !isMovBanco;
        }
    
        protected void txtDigitoNum_Load(object sender, EventArgs e)
        {
            // Esconde campo do dígito verificador
            if (!FinanceiroConfig.FormaPagamento.BloquearChequesDigitoVerificador)
                ((TextBox)sender).Style.Add("display", "none");
        }
    
        protected void rqdDigitoNum_Load(object sender, EventArgs e)
        {
            // Desabilita a validação do campo dígito verificador
            if (!FinanceiroConfig.FormaPagamento.BloquearChequesDigitoVerificador)
                ((RequiredFieldValidator)sender).Enabled = false;
        }
    
        protected string ValorTipoPessoa(object cpfCnpj)
        {
            string c = cpfCnpj != null ? cpfCnpj.ToString() : "";
            return c.Length > 11 ? "J" : "F";
        }
    }
}
