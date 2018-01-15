using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstAjusteContribuicao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void grdAjusteCont_DataBound(object sender, EventArgs e)
        {
            if (grdAjusteCont.Rows.Count == 1)
            {
                grdAjusteCont.Rows[0].Visible = AjusteContribuicaoDAO.Instance.GetCountReal(0, null, null, 0, 0) > 0;
            }
        }
    
        protected void grdAjusteCont_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdAjusteCont.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void imgInserir_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                GridViewRow r = grdAjusteCont.FooterRow;
    
                AjusteContribuicao novo = new AjusteContribuicao();
                novo.FonteAjuste = Conversoes.ConverteValor<int>(((DropDownList)r.FindControl("drpFonteAjuste")).SelectedValue);
                novo.CodCredCont = Conversoes.ConverteValor<int>(((Glass.UI.Web.Controls.ctrlSelPopup)r.FindControl("selCodCredCont")).Valor);
                novo.TipoImposto = Conversoes.ConverteValor<int>(((DropDownList)r.FindControl("drpTipoImposto")).SelectedValue);
                novo.TipoAjuste = Conversoes.ConverteValor<int>(((DropDownList)r.FindControl("drpTipoAjuste")).SelectedValue);
                novo.DataAjuste = ((Glass.UI.Web.Controls.ctrlData)r.FindControl("ctrlDataAjuste")).Data;
                novo.CodigoAjuste = Conversoes.ConverteValor<int>(((Glass.UI.Web.Controls.ctrlSelPopup)r.FindControl("ctrlSelCodigoAjuste")).Valor);
                novo.ValorAjuste = Glass.Conversoes.StrParaDecimal(((TextBox)r.FindControl("txtValorAjuste")).Text);
                novo.NumeroDocumento = ((TextBox)r.FindControl("txtNumDocumento")).Text;
                novo.Descricao = ((TextBox)r.FindControl("txtDescricao")).Text;
    
                AjusteContribuicaoDAO.Instance.Insert(novo);
                grdAjusteCont.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir ajuste.", ex, Page);
            }
        }
    }
}
