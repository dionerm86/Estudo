using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaPlanoConta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.AddDays(-DateTime.Now.Day).ToString("dd/MM/yyyy");
            }
    
            if (chkDetalhes.Checked)
                chkAjustado.Checked = false;
    
            // Esconde coluna Cliente/Fornecedor e Data se não for plano de conta detalhado
            grdPlanoContas.Columns[3].Visible = chkDetalhes.Checked;
            grdPlanoContas.Columns[4].Visible = chkDetalhes.Checked;
    
            chkMes.Enabled = !chkDetalhes.Checked;
            chkAjustado.Enabled = !chkDetalhes.Checked;
            chkAgruparDetalhes.Enabled = chkDetalhes.Checked;

            if (chkDetalhes.Checked)
                tbOrdenar.Style.Remove("display");
            else
            {
                tbOrdenar.Style.Add("display", "none");
                drpOrdenar.SelectedIndex = 0;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPlanoContas.DataBind();
            grdPlanoContas.PageIndex = 0;
        }
    
        protected void drp_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdPlanoContas.DataBind();
            grdPlanoContas.PageIndex = 0;
        }
    
        protected void drpCategoriaConta_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpGrupoConta.Items.Clear();
            drpGrupoConta.DataBind();
    
            drpGrupoConta.Items.Insert(0, new ListItem("Todos", "0"));
    
            grdPlanoContas.PageIndex = 0;
        }
    
        protected void odsListPlanoContas_Selecting(object sender, Colosoft.WebControls.VirtualObjectDataSourceSelectingEventArgs e)
        {
            odsListPlanoContas.SelectMethod = !chkDetalhes.Checked ? "GetList" : "GetListDetalhes";
            odsListPlanoContas.SelectCountMethod = !chkDetalhes.Checked ? "GetCount" : "GetDetalhesCount";
        }
    
        protected void grdPlanoContas_Load(object sender, EventArgs e)
        {
            // Esconde colunas de ajuste se a opção "Ajustado" não estiver selecionada
            for (int i = 6; i <= 8; i++)
                grdPlanoContas.Columns[i].Visible = chkAjustado.Checked;
        }    
    }
}
