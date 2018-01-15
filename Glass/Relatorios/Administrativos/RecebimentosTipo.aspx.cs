using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.RelModel;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class RecebimentosTipo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = FuncoesData.ObtemDataPrimeiroDiaUltimoMes();
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = FuncoesData.ObtemDataUltimoDiaUltimoMes();
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void grdRecebimentoTipo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            if (((Recebimento)e.Row.DataItem).IsTotal)
                e.Row.Font.Bold = true;
        }
    }
}
