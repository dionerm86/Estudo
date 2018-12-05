using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class PlanilhaMetragem : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!this.IsPostBack)
            {
                this.drpSetor.SelectedIndex = this.drpSetor.Items.IndexOf(this.drpSetor.Items.FindByValue("5"));
                this.drpSetor_SelectedIndexChanged(sender, e);
            }

            this.ctrlDataIniEnt.Data = DateTime.Now.AddMonths(-1);
            this.ctrlDataFimEnt.Data = DateTime.Now.AddMonths(1);
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            this.grdMetragem.PageIndex = 0;
        }

        protected void drpSetor_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool periodoSetorVisible = this.drpSetor.SelectedValue != "0";
            this.lblPeriodoSit.Visible = periodoSetorVisible;
            ((TextBox)this.ctrlDataIni.FindControl("txtData")).Style.Add("display", periodoSetorVisible ? string.Empty : "none");
            ((ImageButton)this.ctrlDataIni.FindControl("imgData")).Style.Add("display", periodoSetorVisible ? string.Empty : "none");
            ((TextBox)this.ctrlDataFim.FindControl("txtData")).Style.Add("display", periodoSetorVisible ? string.Empty : "none");
            ((ImageButton)this.ctrlDataFim.FindControl("imgData")).Style.Add("display", periodoSetorVisible ? string.Empty : "none");
            this.imgPesq2.Visible = periodoSetorVisible;

            if (periodoSetorVisible)
            {
                this.lblPeriodoSit.Text = "Período (" + this.drpSetor.SelectedItem.Text + ")";
            }
            else
            {
                ((TextBox)this.ctrlDataIni.FindControl("txtData")).Text = string.Empty;
                ((TextBox)this.ctrlDataFim.FindControl("txtData")).Text = string.Empty;
            }

            while (this.drpFuncionario.Items.Count > 1)
            {
                this.drpFuncionario.Items.RemoveAt(1);
            }

            this.drpFuncionario.DataBind();
        }
    }
}
