using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstProcessoAdministrativo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            string numeroProcesso = ((TextBox)grdProcAdm.FooterRow.FindControl("txtNumeroProcesso")).Text;
            string natureza = ((DropDownList)grdProcAdm.FooterRow.FindControl("ddlNatureza")).SelectedValue;
            string dataDecisao = ((TextBox)((Glass.UI.Web.Controls.ctrlData)grdProcAdm.FooterRow.FindControl("ctrlDataDecisao")).FindControl("txtData")).Text;
    
            ProcessoAdministrativo novo = new ProcessoAdministrativo();
            novo.Natureza = Glass.Conversoes.StrParaInt(natureza);
            novo.NumeroProcesso = numeroProcesso;
            novo.DataDecisao = DateTime.Parse(dataDecisao);
    
            ProcessoAdministrativoDAO.Instance.Insert(novo);
            grdProcAdm.DataBind();
        }
    
        protected void grdProcAdm_DataBound(object sender, EventArgs e)
        {
            if (grdProcAdm.Rows.Count == 1 && ProcessoAdministrativoDAO.Instance.GetCountReal(null, 0, null, null) == 0)
                grdProcAdm.Rows[0].Visible = false;
        }
        protected void grdProcAdm_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            e.NewValues[2] = DateTime.Parse(e.NewValues[2].ToString());
        }

        protected void grdProcAdm_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProcAdm.ShowFooter = e.CommandName != "Edit";
        }
    }
}
