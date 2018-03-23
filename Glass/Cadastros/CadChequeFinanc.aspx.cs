using System;
using System.Web.UI.WebControls;
using Glass.Configuracoes;
using Glass.Data.DAL;

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
            Ajax.Utility.RegisterTypeForAjax(typeof(CadChequeFinanc));
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

        /// <summary>
        /// Verifica se o cheque já existe ou se deve ser bloqueado pelo dígito verificador
        /// </summary>
        [Ajax.AjaxMethod()]
        public string ValidaCheque(string idCliente, string banco, string agencia, string conta, string numero, string digitoNum)
        {
            if (ChequesDAO.Instance.ExisteCheque(banco, agencia, conta, Conversoes.StrParaInt(numero)))
                return "false|Já foi cadastrado um cheque com os dados informados.";

            if (FinanceiroConfig.FormaPagamento.BloquearChequesDigitoVerificador && !String.IsNullOrEmpty(idCliente) && idCliente != "0" &&
                !String.IsNullOrEmpty(digitoNum) && ChequesDAO.Instance.ExisteChequeDigito(Conversoes.StrParaUint(idCliente), 0, Conversoes.StrParaInt(numero), digitoNum))
                return "false|Já foi cadastrado um cheque com os dados informados.";

            return "true";
        }
    }
}
