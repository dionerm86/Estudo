using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRetificarArquivoRemessa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grdContasReceber_DataBound(object sender, EventArgs e)
        {
            var possuiItens = ((GridView)sender).Rows.Count > 0;

            btnRetificarArquivoRemessa.Visible = possuiItens;
            lblContas.Visible = possuiItens;
            lblNaoRemover.Visible = possuiItens;
        }

        protected void grdContasReceber_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (((Data.Model.ContasReceber)e.Row.DataItem).Recebida)
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                        e.Row.Cells[i].ForeColor = System.Drawing.Color.Red;
            }

        }

        protected void btnRetificarArquivoRemessa_Click(object sender, EventArgs e)
        {
            try
            {
                var idsContaR = new List<int>();

                // Pega o id das contas que serão removidas da comissão já paga
                foreach (GridViewRow r in grdContasReceber.Rows)
                    if (!((CheckBox)r.FindControl("chkSel")).Checked)
                        idsContaR.Add(((HiddenField)r.FindControl("hdfIdContaR")).Value.StrParaInt());

                if (Request["id"].StrParaInt() == 0)
                    throw new Exception("Arquivo remessa não encontrado.");

                if (idsContaR == null || idsContaR.Count == 0)
                    throw new Exception("Nenhuma conta foi removida");

                Data.DAL.ArquivoRemessaDAO.Instance.RetificarArquivoRemessa(idsContaR, Request["id"].StrParaInt());

                MensagemAlerta.ShowMsg("Arquivo remessa retificado", Page);

                grdContasReceber.DataBind();
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao retificar", ex, Page);
            }
        }
    }
}