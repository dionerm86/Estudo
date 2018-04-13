using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SelContaPagar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            // Se o id do fornecedor tiver sido passado, permite pesquisar contas apenas dele
            if (Request["Num"] != null)
            {
                txtFornecedor.Text = Request["Num"];
                txtNome.Text = Request["Nome"];
                txtFornecedor.Enabled = false;
                txtNome.Enabled = false;
            }
    
            if (!IsPostBack)
            {    
                drpTipo.Items.Add(new ListItem(FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil, FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil));
                drpTipo.Items.Add(new ListItem(FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil, FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil));
            }
    
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), "desabilitaUnload", "window.onunload = null;\n");
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdConta.PageIndex = 0;
        }
    
        protected void chkCustoFixo_CheckedChanged(object sender, EventArgs e)
        {
            grdConta.PageIndex = 0;
        }
    
        protected void lnkAddAll_Click(object sender, EventArgs e)
        {
            // Busca as contas a pagar que estiverem na tela
            List<ContasPagar> lstContaPag = new List<ContasPagar>((List<ContasPagar>)odsContasPagar.Select());
    
            // Gera o script para adicionar todas elas na tela
            string script = String.Empty;

            foreach (ContasPagar conta in lstContaPag)
                script += "window.opener.setContaPagar(" +
                    conta.IdContaPg + ",'" +
                    conta.IdCompra + "','" +
                    conta.IdCustoFixo + "','" +
                    conta.IdImpostoServ + "','" +
                    conta.IdFornec + "','" +
                    (!String.IsNullOrEmpty(conta.NomeFornec) ? conta.NomeFornec.ToString().Replace("'", "") : "") + "','" +
                    conta.ValorVenc.ToString("C") + "','" +
                    conta.DataVenc.ToString("d") + "', '" +
                    (conta.DescrPlanoConta != null ? conta.DescrPlanoConta.ToString().Replace("'", "") : String.Empty) + "', window);";
    
            if (Request["encontroContas"] == "1")
                script += "window.opener.redirectUrl(window.opener.location.href); ";
    
            script += "closeWindow()";
    
            ClientScript.RegisterStartupScript(typeof(string), "addAll", script, true);
        }

        protected void chkContasVinculadas_CheckedChanged(object sender, EventArgs e)
        {
            if (txtNome != null && string.IsNullOrEmpty(txtNome.Text))
                ((CheckBox)sender).Checked = false;
        }
    }
}
