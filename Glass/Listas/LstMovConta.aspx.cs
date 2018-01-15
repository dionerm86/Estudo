using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstMovConta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                drpContaBanco.SelectedIndex = 1;
            }
    
            lnkImprimir.Visible = true;
            chkApenasDinheiro.Visible = false;
        }
    
        protected string GetCodigoTabela()
        {
            return ((int)LogCancelamento.TabelaCancelamento.MovimentacaoBancaria).ToString();
        }
    
        protected void grdMovBanco_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grdMovBanco.Rows)
            {
                if (String.IsNullOrEmpty(((Label)row.Cells[0].FindControl("lblCodMov")).Text))
                {
                    row.ForeColor = System.Drawing.Color.Black;
                    row.Font.Bold = true;
                }
    
                if (((HiddenField)row.Cells[0].FindControl("hdfTipoMov")).Value == "2")
                    foreach (TableCell c in row.Cells)
                        c.ForeColor = System.Drawing.Color.Red;
            }
        }
    
        protected void grdMovBanco_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Up")
                    MovBancoDAO.Instance.TrocaMovimentacao(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()), 2);
                else if (e.CommandName == "Down")
                    MovBancoDAO.Instance.TrocaMovimentacao(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()), 1);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao mudar posição da movimentação.", ex, Page);
            }

            grdMovBanco.DataBind();
        }
    
        protected void drpContaBanco_DataBound(object sender, EventArgs e)
        {
            // Caso não esteja filtrando por conta bancária, mostra a coluna com a conta bancária e esconde a coluna de saldo
            grdMovBanco.Columns[4].Visible = drpContaBanco.SelectedIndex == 0; // Conta bancária
            grdMovBanco.Columns[9].Visible = !grdMovBanco.Columns[4].Visible; // Saldo
        }

        protected void odsMovBanco_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados da movimentação.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
