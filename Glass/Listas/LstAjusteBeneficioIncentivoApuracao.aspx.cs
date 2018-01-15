using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstAjusteBeneficioIncentivoApuracao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                grdAjuste.DataBind();
            }
    
            grdAjuste.DataBound += new EventHandler(grdAjuste_DataBound);
            grdAjuste.RowCommand += new GridViewCommandEventHandler(grdAjuste_RowCommand);
    
        }
        protected void imgInserir_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                GridViewRow r = grdAjuste.FooterRow;
    
                AjusteBeneficioIncentivoApuracao novo = new AjusteBeneficioIncentivoApuracao();
                novo.Data = ((Glass.UI.Web.Controls.ctrlData)r.FindControl("txtData")).Data;
                novo.IdAjBenInc = Conversoes.ConverteValor<uint>(((DropDownList)r.FindControl("drpCodigo")).SelectedValue);
                novo.Observacao = ((TextBox)r.FindControl("txtObs")).Text;
                novo.Valor = Conversoes.ConverteValor<decimal>(((TextBox)r.FindControl("txtValor")).Text);
    
                AjusteBeneficioIncentivoApuracaoDAO.Instance.Insert(novo);
                grdAjuste.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir ajuste.", ex, Page);
            }
        }
    
        protected void grdAjuste_DataBound(object sender, EventArgs e)
        {
            if (grdAjuste.Rows.Count == 1)
            {
                grdAjuste.Rows[0].Visible = AjusteBeneficioIncentivoApuracaoDAO.Instance.GetCount((Glass.Data.EFD.ConfigEFD.TipoImpostoEnum)Glass.Conversoes.StrParaInt(drpTipoImposto0.SelectedValue), txtDataInicio.DataString, txtDataFim.DataString) > 0;
            }
        }
    
        protected void grdAjuste_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdAjuste.ShowFooter = e.CommandName != "Edit";
    
            if(e.CommandName.Equals("Info"))
            {
                string[] dados = e.CommandArgument.ToString().Split(';');
    
                uint id = Convert.ToUInt32(dados[0]);
                int imposto = Convert.ToInt32(dados[1]);
    
                Response.Redirect("LstAjusteApuracaoInfo.aspx?id=" + id + "&imposto=" + imposto);
            }
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
