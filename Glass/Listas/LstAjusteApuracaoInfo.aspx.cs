using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstAjusteApuracaoInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["id"] == null)
                Response.Redirect("LstAjusteBeneficioIncentivoApuracao.aspx");
    
        }
    
        protected void imgInserir_Click(object sender, EventArgs e)
        {
            try
            {
                GridViewRow r = grdAjuste.FooterRow;
    
                AjusteApuracaoInfoAdicional novo = new AjusteApuracaoInfoAdicional();
                novo.IdABIA = Glass.Conversoes.StrParaUint(Request["id"]);
                novo.IndProc = Convert.ToInt32(((DropDownList)r.FindControl("drpIndProc")).SelectedValue);
                novo.NumDa = ((TextBox)r.FindControl("txtNumDa")).Text;
                novo.NumProc = ((TextBox)r.FindControl("txtNumProc")).Text;
                novo.Proc = ((TextBox)r.FindControl("txtProc")).Text;
                novo.TxtCompl = ((TextBox)r.FindControl("txtCompl")).Text;
    
                AjusteApuracaoInfoAdicionalDAO.Instance.Insert(novo);
                grdAjuste.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir registro.", ex, Page);
            }
        }
    
        protected void grdAjuste_DataBound(object sender, EventArgs e)
        {
            if (grdAjuste.Rows.Count == 1)
            {
                grdAjuste.Rows[0].Visible = AjusteApuracaoInfoAdicionalDAO.Instance.GetCount((Glass.Data.EFD.ConfigEFD.TipoImpostoEnum)Glass.Conversoes.StrParaInt(Request["imposto"]), Glass.Conversoes.StrParaUint(Request["id"])) > 0;
            }
        }
    
        protected void grdAjuste_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdAjuste.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void lkbVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("LstAjusteBeneficioIncentivoApuracao.aspx");
        }
    }
}
