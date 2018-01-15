using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstAjusteApuracaoValoresDeclaratorios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void grdAjuste_DataBound(object sender, EventArgs e)
        {
            if (grdAjuste.Rows.Count == 0)
                return;
    
            grdAjuste.Rows[0].Visible = grdAjuste.Rows.Count > 1 || AjusteApuracaoValorDeclaratorioDAO.Instance.GetCount(txtDataInicio.DataString, txtDataFim.DataString) > 0;
        }
    
        protected void grdAjuste_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdAjuste.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void grdAjuste_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            e.NewValues["Valor"] = Convert.ToDecimal(e.NewValues["Valor"]);
            e.NewValues["IdAjBenInc"] = ((HiddenField)grdAjuste.Rows[e.RowIndex].FindControl("hdfCodigo")).Value;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdAjuste.PageIndex = 0;
        }
    }
}
