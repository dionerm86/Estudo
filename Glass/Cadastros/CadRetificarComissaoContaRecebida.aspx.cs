using Glass.Data.DAL;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRetificarComissaoContaRecebida : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadRetificarComissaoContaRecebida));

            if (!IsPostBack)
            {
                DateTime final = DateTime.Now.AddDays(-DateTime.Now.Day);
                DateTime inicial = final.AddDays(1 - final.Day);

                ((TextBox)ctrlDataComissao.FindControl("txtData")).Text = DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy");
            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdComissao.PageIndex = 0;
            grdComissao.DataBind();
        }

        protected void grdComissao_DataBound(object sender, EventArgs e)
        {
            gerarComissao.Visible = grdComissao.Rows.Count > 0;

            if (grdComissao.Rows.Count == 0)
                grdComissao.EmptyDataText = "Não há comissões para o filtro especificado.";
        }

        protected void odsComissoesFunc_Selecting(object sender, Colosoft.WebControls.VirtualObjectDataSourceSelectingEventArgs e)
        {
            if (e.ExecutingSelectCount)
                return;

            while (drpIdComissao.Items.Count > 1)
                drpIdComissao.Items.RemoveAt(1);
        }

        protected void chkSel_DataBinding(object sender, EventArgs e)
        {
            GridViewRow linha = ((CheckBox)sender).Parent.Parent as GridViewRow;
            var cr = linha != null ? linha.DataItem as Glass.Data.Model.ContasReceber : null;

            if (cr == null)
                return;

            ((CheckBox)sender).Attributes.Add("ValorComissao", cr.ValorComissao.ToString().Replace(",", "."));
            ((CheckBox)sender).Attributes.Add("ValorRecebido", cr.ValorRec.ToString().Replace(",", "."));
            ((CheckBox)sender).Attributes.Add("IdContaR", cr.IdContaR.ToString().Replace(",", "."));
        }

        protected void btnRetificarComissao_Click(object sender, EventArgs e)
        {
            string idsContaR = String.Empty;

            // Pega o id das contas que serão removidas da comissão já paga
            foreach (GridViewRow r in grdComissao.Rows)
                if (!((CheckBox)r.FindControl("chkSel")).Checked)
                    idsContaR += ((HiddenField)r.FindControl("hdfIdContaR")).Value + ",";

            try
            {
                decimal valorComissao = decimal.Parse(hdfValorComissao.Value);

                ComissaoDAO.Instance.RetificarComissaoContasReceber(drpIdComissao.SelectedValue.StrParaUint(), drpNome.SelectedValue.StrParaUint(),
                    idsContaR.TrimEnd(','), valorComissao, ((TextBox)ctrlDataComissao.FindControl("txtData")).Text);

                Glass.MensagemAlerta.ShowMsg("Comissão retificada!", Page);

                drpIdComissao.DataBind();
                grdComissao.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao retificar comissão.", ex, Page);
            }
        }
    }
}