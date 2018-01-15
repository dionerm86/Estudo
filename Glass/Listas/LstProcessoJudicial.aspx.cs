using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstProcessoJudicial : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            string numeroProcesso = ((TextBox)grdProcJud.FooterRow.FindControl("txtNumeroProcesso")).Text;
            string secaoJudiciaria = ((TextBox)grdProcJud.FooterRow.FindControl("txtSecaoJudiciaria")).Text;
            string vara = ((TextBox)grdProcJud.FooterRow.FindControl("txtVara")).Text;
            string natureza = ((DropDownList)grdProcJud.FooterRow.FindControl("ddlNatureza")).SelectedValue;
            string descricao = ((TextBox)grdProcJud.FooterRow.FindControl("txtDescricao")).Text;
            string dataDecisao = ((TextBox)((Glass.UI.Web.Controls.ctrlData)grdProcJud.FooterRow.FindControl("ctrlDataDecisao")).FindControl("txtData")).Text;
    
            ProcessoJudicial novo = new ProcessoJudicial();
            novo.Natureza = Glass.Conversoes.StrParaInt(natureza);
            novo.NumeroProcesso = numeroProcesso;
            novo.SecaoJudiciaria = secaoJudiciaria;
            novo.Vara = vara;
            novo.DataDecisao = DateTime.Parse(dataDecisao);
            novo.Descricao = descricao;
    
            ProcessoJudicialDAO.Instance.Insert(novo);
            grdProcJud.DataBind();
        }
    
        protected void grdProcJud_DataBound(object sender, EventArgs e)
        {
            if (grdProcJud.Rows.Count == 1 && ProcessoJudicialDAO.Instance.GetCountReal(null, 0, null, null, null, null, null) == 0)
                grdProcJud.Rows[0].Visible = false;
    
        }
        protected void grdProcJud_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            e.NewValues[5] = DateTime.Parse(e.NewValues[5].ToString());
        }

        protected void grdProcJud_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProcJud.ShowFooter = e.CommandName != "Edit";
        }
    }
}
