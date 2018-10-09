using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstInfoPedidosPeriodo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = String.IsNullOrEmpty(Request["dataIni"]) ? DateTime.Now.ToString("dd/MM/yyyy") : Request["dataIni"];
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = String.IsNullOrEmpty(Request["dataFim"]) ? DateTime.Now.AddDays(15).ToString("dd/MM/yyyy") : Request["dataFim"];
            }
        }
    
        private bool corAlternada = true;
    
        protected string GetBackgroundColor()
        {
            corAlternada = !corAlternada;
            return corAlternada ? GetColorName(grdInfoPedidos.AlternatingRowStyle.BackColor) : GetColorName(grdInfoPedidos.RowStyle.BackColor);
        }
    
        private string GetColorName(System.Drawing.Color cor)
        {
            return cor.IsNamedColor ? cor.Name : "#" + cor.Name.Substring(2);
        }
    
        protected void grdInfoPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView grdPedidos = e.Row.FindControl("grdPedidos") as GridView;
            if (grdPedidos != null)
                grdPedidos.DataBind();
        }
    }
}
