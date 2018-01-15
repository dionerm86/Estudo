using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstAjusteApuracaoIPI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdAjuste.PageIndex = 0;
        }
    
        protected void grdAjuste_DataBound(object sender, EventArgs e)
        {
            if (grdAjuste.Rows.Count == 1)
            {
                int tipo = 3; // !string.IsNullOrEmpty(drpTipoImposto0.SelectedValue) ? Glass.Conversoes.StrParaInt(drpTipoImposto0.SelectedValue) : 0;
    
                grdAjuste.Rows[0].Visible = AjusteApuracaoIPIDAO.Instance.GetCount((Glass.Data.EFD.ConfigEFD.TipoImpostoEnum)tipo, txtDataInicio.DataString, txtDataFim.DataString) > 0;
            }
        }
    
        protected void grdAjuste_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdAjuste.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void grdAjuste_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            e.NewValues["Valor"] = Convert.ToDecimal(e.NewValues["Valor"]);
            e.NewValues["CodAjuste"] = (grdAjuste.Rows[e.RowIndex].FindControl("selCodAjuste") as Glass.UI.Web.Controls.ctrlSelPopup).Valor;
        }
    }
}
