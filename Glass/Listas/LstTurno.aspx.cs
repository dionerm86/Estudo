using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstTurno : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdTurno.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkImprimir.Visible = false;// grdTurno.Rows.Count > 0;
            lnkExportarExcel.Visible = false;//grdTurno.Rows.Count > 0;
        }
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadTurno.aspx");
        }
    }
}
