using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstLimiteChequeCpfCnpj : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!FinanceiroConfig.LimitarChequesPorCpfOuCnpj)
                Response.Redirect("~/WebGlass/Main.aspx", true);
    
            grdLimiteCheque.Columns[0].Visible =
                Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) ||
                /* Chamado 15907.
                 * Foi solicitado que usuários do tipo Caixa Diário pudessem alterar o limite de cheque por CPF/CNPJ. */
                UserInfo.GetUserInfo.IsCaixaDiario;
        }
    
        protected void Label4_PreRender(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            decimal valor = Glass.Conversoes.StrParaDecimal(lbl.Text.Replace(".", ""));
    
            if (valor < 0)
                lbl.ForeColor = System.Drawing.Color.Red;
            else if (valor > 0)
                lbl.ForeColor = System.Drawing.Color.Green;
        }
    }
}
