using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Listas
{
    public partial class LstNaturezaOperacao : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdNaturezaOperacao.Register(true, true);
            odsNaturezaOperacao.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title += CfopDAO.Instance.ObtemCodInterno(Glass.Conversoes.StrParaUint(Request["idCfop"]));
        }
    
        protected void imgInserir_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                var nova = new Glass.Fiscal.Negocios.Entidades.NaturezaOperacao()
                {
                    IdCfop = Glass.Conversoes.StrParaInt(Request["idCfop"]),
                    CodInterno = (grdNaturezaOperacao.FooterRow.FindControl("txtCodigoInterno") as TextBox).Text,
                    Mensagem = (grdNaturezaOperacao.FooterRow.FindControl("txtMensagem") as TextBox).Text,
                    CstIcms = (grdNaturezaOperacao.FooterRow.FindControl("drpCstIcms") as DropDownList).SelectedValue,
                    PercReducaoBcIcms = Glass.Conversoes.StrParaFloat((grdNaturezaOperacao.FooterRow.FindControl("txtPercReducaoBcIcms") as TextBox).Text),
                    Csosn = (grdNaturezaOperacao.FooterRow.FindControl("drpCsosn") as DropDownList).SelectedValue,
                    CstIpi = (Glass.Data.Model.ProdutoCstIpi?)Glass.Conversoes.StrParaIntNullable((grdNaturezaOperacao.FooterRow.FindControl("drpCstIpi") as DropDownList).SelectedValue),
                    CstPisCofins = Glass.Conversoes.StrParaIntNullable((grdNaturezaOperacao.FooterRow.FindControl("drpCstPisCofins") as DropDownList).SelectedValue),
                    CalcIcms = (grdNaturezaOperacao.FooterRow.FindControl("chkCalcularIcms") as CheckBox).Checked,
                    CalcIcmsSt = (grdNaturezaOperacao.FooterRow.FindControl("chkCalcularIcmsSt") as CheckBox).Checked,
                    CalcIpi = (grdNaturezaOperacao.FooterRow.FindControl("chkCalcularIpi") as CheckBox).Checked,
                    CalcPis = (grdNaturezaOperacao.FooterRow.FindControl("chkCalcularPis") as CheckBox).Checked,
                    CalcCofins = (grdNaturezaOperacao.FooterRow.FindControl("chkCalcularCofins") as CheckBox).Checked,
                    IpiIntegraBcIcms = (grdNaturezaOperacao.FooterRow.FindControl("chkIpiIntegraBaseCalculoIcms") as CheckBox).Checked,
                    FreteIntegraBcIpi = (grdNaturezaOperacao.FooterRow.FindControl("chkFreteIntegraBaseCalculoIpi") as CheckBox).Checked,
                    AlterarEstoqueFiscal = (grdNaturezaOperacao.FooterRow.FindControl("chkAlterarEstoqueFiscal") as CheckBox).Checked,
                    CalcularDifal = (grdNaturezaOperacao.FooterRow.FindControl("chkCalcularDifal") as CheckBox).Checked,
                    CalcEnergiaEletrica = (grdNaturezaOperacao.FooterRow.FindControl("chkCalcEnergiaEletrica") as CheckBox).Checked,
                    CodEnqIpi = (grdNaturezaOperacao.FooterRow.FindControl("txtCodEnqIpi") as TextBox).Text,
                };

                var fluxo = ServiceLocator.Current.GetInstance<Glass.Fiscal.Negocios.ICfopFluxo>();

                var resultado = fluxo.SalvarNaturezaOperacao(nova);
                if (resultado)
                    grdNaturezaOperacao.DataBind();
                else
                    Glass.MensagemAlerta.ErrorMsg("Falha ao inserir natureza de operação.", resultado);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir natureza de operação.", ex, Page);
            }
        }
    
        protected void grdNaturezaOperacao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdNaturezaOperacao.ShowFooter = e.CommandName != "Edit";
        }
    }
}
