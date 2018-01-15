using System;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaComprasProd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            //if (!IsPostBack)
            //{
            //    ((TextBox)ctrlDataIniSit.FindControl("txtData")).Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("dd/MM/yyyy");
            //    ((TextBox)ctrlDataFimSit.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            //}
    
            chkExibirDetalhes.Enabled = !chkAgruparFornec.Checked;
            if (chkAgruparFornec.Checked)
                chkExibirDetalhes.Checked = false;
    
            grdComprasProd.Columns[2].Visible = chkAgruparFornec.Checked;

            if (!IsPostBack)
            {
                ctrlDataIniSit.Data = DateTime.Now.AddMonths(-1);
                ctrlDataFimSit.Data = DateTime.Now;
            }
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdComprasProd.PageIndex = 0;
        }
    }
}
