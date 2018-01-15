using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class LucroAproximado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (((TextBox)ctrlDataIni.FindControl("txtData")).Text == String.Empty)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Parse("01/" + DateTime.Now.AddMonths(-1).Month + "/" + DateTime.Now.AddMonths(-1).Year).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = FuncoesData.ObtemDataUltimoDiaUltimoMes();
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void grdLucro_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            if (((Data.RelModel.LucroAproximado)e.Row.DataItem).IsTotal)
                e.Row.Font.Bold = true;
        }
    }
}
