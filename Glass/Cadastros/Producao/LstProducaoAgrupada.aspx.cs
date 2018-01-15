using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros.Producao
{
    public partial class LstProducaoAgrupada : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                ctrlDataFim.Data = DateTime.Now;
                ctrlDataIni.Data = ctrlDataFim.Data.AddDays(-3);
            }

            grdPecas.Columns[4].Visible = PCPConfig.ExibirCustoVendaRelatoriosProducao;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPecas.PageIndex = 0;
        }
    
        protected void drpSetor_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool periodoSitVisible = drpSetor.SelectedValue != "0";
            lblPeriodoSit.Visible = periodoSitVisible;
            ((TextBox)ctrlDataIni.FindControl("txtData")).Style.Add("display", (periodoSitVisible ? "" : "none"));
            ((ImageButton)ctrlDataIni.FindControl("imgData")).Style.Add("display", (periodoSitVisible ? "" : "none"));
            ((TextBox)ctrlDataFim.FindControl("txtData")).Style.Add("display", (periodoSitVisible ? "" : "none"));
            ((ImageButton)ctrlDataFim.FindControl("imgData")).Style.Add("display", (periodoSitVisible ? "" : "none"));
            imgPesq2.Visible = periodoSitVisible;
    
            drpFuncionario.Items.Clear();
    
            if (periodoSitVisible)
                lblPeriodoSit.Text = "Período (" + drpSetor.SelectedItem.Text + ")";
            else
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = "";
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = "";
            }
        }
    }
}
