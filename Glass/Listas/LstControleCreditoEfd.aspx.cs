using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Colosoft.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstControleCreditoEfd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                drpMes.DataBind();
                drpMes.SelectedValue = DateTime.Now.Month.ToString();
                txtAno.Text = DateTime.Now.Year.ToString();
            }
    
            hdfPeriodo.Value = drpMes.SelectedValue + "/" + txtAno.Text;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void imgAdd_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                ControleCreditoEfd item = new ControleCreditoEfd()
                {
                    CodCred = Glass.Conversoes.StrParaIntNullable((grdControleCreditos.FooterRow.FindControl("selCodCred") as Glass.UI.Web.Controls.ctrlSelPopup).Valor),
                    PeriodoGeracao = (grdControleCreditos.FooterRow.FindControl("txtPeriodo") as TextBox).Text,
                    TipoImposto = Glass.Conversoes.StrParaInt((grdControleCreditos.FooterRow.FindControl("drpTipoImposto") as DropDownList).SelectedValue),
                    ValorGerado = Glass.Conversoes.StrParaDecimal((grdControleCreditos.FooterRow.FindControl("txtValorGerado") as TextBox).Text),
                    IdLoja = Glass.Conversoes.StrParaUint((grdControleCreditos.FooterRow.FindControl("drpLoja") as DropDownList).SelectedValue)
                };
    
                ControleCreditoEfdDAO.Instance.Insert(item);
    
                grdControleCreditos.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir crédito.", ex, Page);
            }
        }
    
        protected void grdControleCreditos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdControleCreditos.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void odsControleCreditos_Updated(object sender, VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar crédito.", e.Exception, Page);
            }
        }
    
        protected void odsControleCreditos_Deleted(object sender, VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir crédito.", e.Exception, Page);
            }
        }
    }
}
