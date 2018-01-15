using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstMovChapa : System.Web.UI.Page
    {
        #region Variaveis locais

        private bool corAlternada = true;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ctrlDataIni.Data = DateTime.Now;
                ctrlDataFim.Data = DateTime.Now;
            }
        }

        #region Métodos da Pagina

        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }

        #endregion

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void grdMovChapaDetalhe_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var chapa = (Glass.Estoque.Negocios.Entidades.MovChapaDetalhe)e.Row.DataItem;

                if (chapa.TemOutrasLeituras)
                    foreach (TableCell cell in e.Row.Cells)
                        cell.ForeColor = System.Drawing.Color.Red;

                if (chapa.SaidaRevenda)
                    foreach (TableCell cell in e.Row.Cells)
                        cell.ForeColor = System.Drawing.Color.Blue;
            }
        }
    }
}