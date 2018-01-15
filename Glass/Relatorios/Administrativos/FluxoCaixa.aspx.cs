using System;
using System.Web.UI.WebControls;
using Glass.Data.RelModel;
using System.Drawing;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class FluxoCaixa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Parse(DateTime.Parse("01/" + DateTime.Now.Month + "/" + 
                    DateTime.Now.Year).AddMonths(1).AddDays(-1).Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year).ToString("dd/MM/yyyy");
            }
    
            grdFluxoCaixa.Visible = !chkResumido.Checked;
            grdFluxoCaixaSint.Visible = chkResumido.Checked;
        }
    
        protected void grdFluxoCaixa_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            var f = (Data.RelModel.FluxoCaixa)e.Row.DataItem;
    
            if (f.IsTotal)
            {
                e.Row.Cells[0].Text = "";
                e.Row.Cells[1].ColumnSpan = 2;
                e.Row.Cells.RemoveAt(2);
                e.Row.Font.Bold = true;
    
                if (f.Descricao.Contains("SALDO DO DIA: "))
                    e.Row.Cells[e.Row.Cells.Count - 1].Text = "";
            }
    
            if (f.PrevCustoFixo)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Blue;
    
            if (f.SaldoGeral < 0)
                e.Row.Cells[e.Row.Cells.Count - 1].ForeColor = Color.Red;
        }
    
        protected void grdFluxoCaixaSint_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            FluxoCaixaSintetico f = (FluxoCaixaSintetico)e.Row.DataItem;
    
            if (f.IsTotal)
            {
                e.Row.Cells[0].Text = f.Descricao;
                e.Row.Font.Bold = true;
            }
    
            if (f.SaldoGeral < 0)
                e.Row.Cells[e.Row.Cells.Count - 1].ForeColor = Color.Red;
        }
    
        protected void imgPesq_Click(object sender, EventArgs e)
        {
    
        }
    }
}
