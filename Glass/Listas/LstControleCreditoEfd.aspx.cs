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
                var codCred = (grdControleCreditos.FooterRow.FindControl("selCodCred") as Controls.ctrlSelPopup) != null ?
                    (grdControleCreditos.FooterRow.FindControl("selCodCred") as Controls.ctrlSelPopup).Valor.StrParaIntNullable() : null;
                var periodoGeracao = (grdControleCreditos.FooterRow.FindControl("txtPeriodo") as TextBox) != null ? (grdControleCreditos.FooterRow.FindControl("txtPeriodo") as TextBox).Text : string.Empty;
                var tipoImposto = (grdControleCreditos.FooterRow.FindControl("drpTipoImposto") as DropDownList) != null ?
                    (grdControleCreditos.FooterRow.FindControl("drpTipoImposto") as DropDownList).SelectedValue.StrParaInt() : 0;
                var valorGerado = (grdControleCreditos.FooterRow.FindControl("txtValorGerado") as TextBox) != null ?
                    (grdControleCreditos.FooterRow.FindControl("txtValorGerado") as TextBox).Text.StrParaDecimal() : 0;
                var idLoja = (grdControleCreditos.FooterRow.FindControl("drpLoja") as DropDownList) != null ?
                    (grdControleCreditos.FooterRow.FindControl("drpLoja") as DropDownList).SelectedValue.StrParaUint() : 0;

                ControleCreditoEfd item = new ControleCreditoEfd()
                {
                    CodCred = codCred,
                    PeriodoGeracao = periodoGeracao,
                    TipoImposto = tipoImposto,
                    ValorGerado = valorGerado,
                    IdLoja = idLoja
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
