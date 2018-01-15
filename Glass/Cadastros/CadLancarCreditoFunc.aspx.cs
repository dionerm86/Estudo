using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadLancarCreditoFunc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                uint idFunc = Glass.Conversoes.StrParaUint(drpFunc.SelectedValue);
                uint idConta = Glass.Conversoes.StrParaUint(drpPlanoConta.SelectedValue);
                decimal valor = decimal.Parse(txtValor.Text);
    
                MovFuncDAO.Instance.Movimentar(idFunc, idConta, 1, valor, txtObs.Text);
                Glass.MensagemAlerta.ShowMsg("Crédito lançado.", Page);
    
                drpFunc.SelectedIndex = 0;
                drpPlanoConta.SelectedIndex = 0;
                txtValor.Text = "";
                txtObs.Text = "";
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao lançar crédito.", ex, Page);
            }
        }
    }
}
