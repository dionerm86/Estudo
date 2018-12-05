using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaDreCompetencia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.AddDays(-DateTime.Now.Day).ToString("dd/MM/yyyy");
            }

            // Esconde coluna Cliente/Fornecedor e Data se não for plano de conta detalhado
            grdPlanoContas.Columns[2].Visible = chkDetalhes.Checked;
            grdPlanoContas.Columns[3].Visible = chkDetalhes.Checked;
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
    }}
