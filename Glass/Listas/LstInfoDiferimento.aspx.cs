using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstInfoDiferimento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void grdInfoDifer_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdInfoDifer.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void grdInfoDifer_DataBound(object sender, EventArgs e)
        {
            if (grdInfoDifer.Rows.Count > 0)
                grdInfoDifer.Rows[0].Visible = grdInfoDifer.Rows.Count > 1 || InfoDiferimentoDAO.Instance.GetCountReal() > 0;
        }
    
        protected void imgInserir_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                InfoDiferimento novo = new InfoDiferimento();
                novo.AliqImposto = Glass.Conversoes.StrParaFloat(((TextBox)grdInfoDifer.FooterRow.FindControl("txtAliqImposto")).Text);
                novo.Cnpj = ((TextBox)grdInfoDifer.FooterRow.FindControl("txtCnpj")).Text;
                novo.CodCont = Glass.Conversoes.StrParaInt(((Glass.UI.Web.Controls.ctrlSelPopup)grdInfoDifer.FooterRow.FindControl("ctrlSelCodCont")).Valor);
                novo.CodCred = Glass.Conversoes.StrParaInt(((Glass.UI.Web.Controls.ctrlSelPopup)grdInfoDifer.FooterRow.FindControl("ctrlSelCodCred")).Valor);
                novo.Data = ((Glass.UI.Web.Controls.ctrlData)grdInfoDifer.FooterRow.FindControl("ctrlData")).Data;
                novo.TipoImposto = Glass.Conversoes.StrParaInt(((DropDownList)grdInfoDifer.FooterRow.FindControl("drpTipoImposto")).SelectedValue);
                novo.ValorContribuicao = Glass.Conversoes.StrParaDecimal(((TextBox)grdInfoDifer.FooterRow.FindControl("txtValorCont")).Text);
                novo.ValorCredito = Glass.Conversoes.StrParaDecimal(((TextBox)grdInfoDifer.FooterRow.FindControl("txtValorCredito")).Text);
                novo.ValorNaoRecebido = Glass.Conversoes.StrParaDecimal(((TextBox)grdInfoDifer.FooterRow.FindControl("txtValorNaoRecebido")).Text);
                novo.ValorRecebido = Glass.Conversoes.StrParaDecimal(((TextBox)grdInfoDifer.FooterRow.FindControl("txtValorRecebido")).Text);
    
                InfoDiferimentoDAO.Instance.Insert(novo);
                grdInfoDifer.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir informação de diferimento.", ex, Page);
            }
        }
    }
}
