using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadLancarDebitoFunc : System.Web.UI.Page
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
                decimal valor = Glass.Conversoes.StrParaDecimal(txtValor.Text);
    
                MovFuncDAO.Instance.Movimentar(idFunc, idConta, 2, valor, txtObs.Text);
                Glass.MensagemAlerta.ShowMsg("D�bito lan�ado.", Page);
    
                drpFunc.SelectedIndex = 0;
                drpPlanoConta.SelectedIndex = 0;
                txtValor.Text = "";
                txtObs.Text = "";
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao lan�ar d�bito.", ex, Page);
            }
        }
    }
}
