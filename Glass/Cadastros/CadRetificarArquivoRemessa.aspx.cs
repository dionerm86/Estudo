using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRetificarArquivoRemessa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!string.IsNullOrWhiteSpace(this.Request["id"]))
            {
                this.txtNumRemessa.Text = this.Request["id"];
                this.txtNumRemessa.Enabled = false;
                grdContasReceber.Columns[1].Visible = false;
            }
        }

        protected void grdContasReceber_DataBound(object sender, EventArgs e)
        {
            var possuiItens = ((GridView)sender).Rows.Count > 0;

            btnRetificarArquivoRemessa.Visible = possuiItens;
            lblContas.Visible = possuiItens;
            lblNaoRemover.Visible = possuiItens;
        }

        protected void btnRetificarArquivoRemessa_Click(object sender, EventArgs e)
        {
            try
            {
                var idsContaRRemessa = new List<Tuple<int, int>>();

                // Pega o id das contas que serão removidas da comissão já paga
                foreach (GridViewRow r in grdContasReceber.Rows)
                {
                    if (!((CheckBox)r.FindControl("chkSel")).Checked)
                    {
                        var idContaR = ((HiddenField)r.FindControl("hdfIdContaR")).Value.StrParaInt();
                        var idArquivoRemessa = ((HiddenField)r.FindControl("hdfIdArquivoRemessa")).Value.StrParaInt();

                        idsContaRRemessa.Add(new Tuple<int, int>(idContaR, idArquivoRemessa));
                    }
                }

                if (idsContaRRemessa.Count == 0)
                {
                    throw new InvalidOperationException("Nenhuma conta foi removida");
                }

                var idsRemessa = idsContaRRemessa.Select(f => f.Item2).Distinct();

                foreach (var idRemessa in idsRemessa)
                {
                    var idsConta = idsContaRRemessa.Where(f => f.Item2 == idRemessa).Select(f => f.Item1);
                    Data.DAL.ArquivoRemessaDAO.Instance.RetificarArquivoRemessa(idsConta, idRemessa);
                }

                MensagemAlerta.ShowMsg("Arquivo remessa retificado", Page);

                grdContasReceber.DataBind();
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao retificar", ex, Page);
            }
        }

        protected void imgPesq_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
        }
    }
}
