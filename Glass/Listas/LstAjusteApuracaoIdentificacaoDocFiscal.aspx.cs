using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstAjusteApuracaoIdentificacaoDocFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void grdAjuste_DataBound(object sender, EventArgs e)
        {
            if (grdAjuste.Rows.Count == 1)
            {
                int tipo = !string.IsNullOrEmpty(drpTipoImposto0.SelectedValue) ? Glass.Conversoes.StrParaInt(drpTipoImposto0.SelectedValue) : 0;
                grdAjuste.Rows[0].Visible = AjusteApuracaoIdentificacaoDocFiscalDAO.Instance.GetCount((Glass.Data.EFD.ConfigEFD.TipoImpostoEnum)tipo, Glass.Conversoes.StrParaUint(Request["idNf"]), 0, 0) > 0;
            }
        }
    
        protected void grdAjuste_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdAjuste.ShowFooter = e.CommandName != "Edit";
        }
    
    
        protected void grdAjuste_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            e.NewValues["ValAjItem"] = Convert.ToDecimal(e.NewValues["ValAjItem"]);
            e.NewValues["IdABIA"] = ((HiddenField)grdAjuste.Rows[e.RowIndex].FindControl("hdfAjuste")).Value;
        }
    }
}
