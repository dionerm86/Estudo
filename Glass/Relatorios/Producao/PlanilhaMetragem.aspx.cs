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
    
            if (!IsPostBack)
            {
                drpSetor.SelectedIndex = drpSetor.Items.IndexOf(drpSetor.Items.FindByValue("5"));
                drpSetor_SelectedIndexChanged(sender, e);
            }
    
            /*
            chkSetoresPosteriores.Visible = Glass.Conversoes.StrParaInt(drpSetor.SelectedValue) > 0;
            if (!chkSetoresPosteriores.Visible)
                chkSetoresPosteriores.Checked = false;
            */
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdMetragem.PageIndex = 0;
        }
    
        protected void drpSetor_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool periodoSetorVisible = drpSetor.SelectedValue != "0";
            lblPeriodoSit.Visible = periodoSetorVisible;
            ((TextBox)ctrlDataIni.FindControl("txtData")).Style.Add("display", (periodoSetorVisible ? "" : "none"));
            ((ImageButton)ctrlDataIni.FindControl("imgData")).Style.Add("display", (periodoSetorVisible ? "" : "none"));
            ((TextBox)ctrlDataFim.FindControl("txtData")).Style.Add("display", (periodoSetorVisible ? "" : "none"));
            ((ImageButton)ctrlDataFim.FindControl("imgData")).Style.Add("display", (periodoSetorVisible ? "" : "none"));
            imgPesq2.Visible = periodoSetorVisible;
    
            if (periodoSetorVisible)
                lblPeriodoSit.Text = "Período (" + drpSetor.SelectedItem.Text + ")";
            else
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = "";
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = "";
            }
    
            while (drpFuncionario.Items.Count > 1)
                drpFuncionario.Items.RemoveAt(1);
    
            drpFuncionario.DataBind();
        }
    }
}
