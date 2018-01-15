using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstPosicaoMateriaPrima : System.Web.UI.Page
    {
        private decimal _totalM2Chapa;
        private double _qtdeChapa;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIniEnt.FindControl("txtData")).Text = DateTime.Now.AddDays(-15).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFimEnt.FindControl("txtData")).Text = DateTime.Now.AddDays(15).ToString("dd/MM/yyyy");
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        private bool corAlternada = true;
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }

        protected void grdChapas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var row = (Glass.Data.RelModel.PosicaoMateriaPrimaChapa)e.Row.DataItem;

                _totalM2Chapa += row.TotalM2Chapa;
                _qtdeChapa += row.QtdeChapa;

            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[5].Text = Math.Round(_qtdeChapa, 2).ToString();
                e.Row.Cells[6].Text = Math.Round(_totalM2Chapa, 2).ToString();

                _totalM2Chapa = 0;
                _qtdeChapa = 0;
            }
        }
    }
}
