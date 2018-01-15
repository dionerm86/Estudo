using Glass.Data.DAL;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRetificarAntecipContaRec : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadRetificarAntecipContaRec));
        }

        protected void btnRetificarAntecipacao_Click(object sender, EventArgs e)
        {
            string idsContaR = String.Empty;

            // Pega o id das contas que serão removidas da comissão já paga
            foreach (GridViewRow r in grdContasRec.Rows)
                if (!((CheckBox)r.FindControl("chkSel")).Checked)
                    idsContaR += ((HiddenField)r.FindControl("hdfIdContaR")).Value + ",";

            try
            {
                var idAntecip = ((HiddenField)dtvAntecipacao.FindControl("hdfIdAntecip")).Value.StrParaUint();
                var taxa = ((TextBox)dtvAntecipacao.FindControl("txtTaxa")).Text.StrParaDecimal();
                var iof = ((TextBox)dtvAntecipacao.FindControl("txtIof")).Text.StrParaDecimal();
                var juros = ((TextBox)dtvAntecipacao.FindControl("txtJuros")).Text.StrParaDecimal();
                var idContaBanco = ((DropDownList)dtvAntecipacao.FindControl("drpContaBanco")).SelectedValue.StrParaUint();
                var obs = ((TextBox)dtvAntecipacao.FindControl("txtObs")).Text;
                var dataRec = ((Glass.UI.Web.Controls.ctrlData)dtvAntecipacao.FindControl("ctrlDataRecebimento")).Data;
                var total = hdfValor.Value.StrParaDecimal();
                var estornar = chkEstornar.Checked;
                var dataEstorno = ctrlDataEstorno.DataNullable;

                AntecipContaRecDAO.Instance.Retificar(idAntecip, idsContaR, idContaBanco, total, taxa, juros, iof, dataRec, obs, estornar, dataEstorno);

                Glass.MensagemAlerta.ShowMsg("Antecipação retificada!", Page);

                txtNumAntecip.Text = "";

                grdContasRec.DataBind();
                dtvAntecipacao.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao retificar antecipação.", ex, Page);
            }
        }

        protected void grdContasRec_DataBound(object sender, EventArgs e)
        {
            tbRetificarAntecipacao.Visible = grdContasRec.Rows.Count > 0;

            if (grdContasRec.Rows.Count == 0)
                grdContasRec.EmptyDataText = "Não há contas à receber para o filtro especificado.";
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdContasRec.PageIndex = 0;
            grdContasRec.DataBind();
        }

        protected void chkSel_DataBinding(object sender, EventArgs e)
        {
            GridViewRow linha = ((CheckBox)sender).Parent.Parent as GridViewRow;
            var cr = linha != null ? linha.DataItem as Glass.Data.Model.ContasReceber : null;

            if (cr == null)
                return;

            ((CheckBox)sender).Attributes.Add("Valor", cr.ValorVec.ToString().Replace(",", "."));
        }

        #region Métodos AJAX

        [Ajax.AjaxMethod()]
        public string VerificaPodeRetificarAntecipacao(string idAntecip)
        {
            return ContasReceberDAO.Instance.PodeRetificarAntecipacao(null, idAntecip.StrParaUint()).ToString().ToLower();
        }

        #endregion
    }
}