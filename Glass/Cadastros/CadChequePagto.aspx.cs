using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadChequePagto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadChequePagto));
    
            txtBanco.Enabled = false;
            txtConta.Enabled = false;
            txtAgencia.Enabled = false;
        }
    
        /// <summary>
        /// Verifica se o cheque já existe ou se deve ser bloqueado pelo dígito verificador
        /// </summary>
        [Ajax.AjaxMethod()]
        public string ValidaCheque(string banco, string agencia, string conta, string numero)
        {
            if (ChequesDAO.Instance.ExisteCheque(0, banco, agencia, conta, Glass.Conversoes.StrParaInt(numero)))
                return "false|Já foi cadastrado um cheque com os dados informados.";
    
            return "true";
        }
    
        protected void drpContaBanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(drpContaBanco.SelectedValue))
            {
                ContaBanco conta = ContaBancoDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(drpContaBanco.SelectedValue));
                txtBanco.Text = conta.Nome;
                txtAgencia.Text = conta.Agencia;
                txtConta.Text = conta.Conta;
                txtTitular.Text = conta.Titular != null && conta.Titular.Length > 45 ? conta.Titular.Substring(0, 45) : conta.Titular;
            }
            else
            {
                txtBanco.Text = String.Empty;
                txtAgencia.Text = String.Empty;
                txtConta.Text = String.Empty;
                txtTitular.Text = String.Empty;
            }
        }
    }
}
