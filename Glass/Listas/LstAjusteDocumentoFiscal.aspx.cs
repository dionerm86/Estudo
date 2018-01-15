using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstAjusteDocumentoFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstAjusteDocumentoFiscal));
    
            if (Request["idCte"] != null)
            {
                odsObsLancFiscal.TypeName = "Glass.Data.DAL.ObsLancFiscalCteDAO";
                odsObsLancFiscal.SelectMethod = "GetByCte";
                odsObsLancFiscal.SelectParameters[0].Name = "idCte";
                (odsObsLancFiscal.SelectParameters[0] as QueryStringParameter).QueryStringField = "idCte";
                
                odsAjusteDocumentoFiscal.SelectMethod = "ObtemPorCte";
                odsAjusteDocumentoFiscal.SelectParameters[0].Name = "idCte";
                (odsAjusteDocumentoFiscal.SelectParameters[0] as QueryStringParameter).QueryStringField = "idCte";
            }
        }
    
        protected void imgAdicionar_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                AjusteDocumentoFiscal item = new AjusteDocumentoFiscal()
                {
                    IdObsLancFiscal = Glass.Conversoes.StrParaUint((grdAjusteDocumentoFiscal.FooterRow.FindControl("drpObsLancFiscal") as DropDownList).SelectedValue),
                    AliquotaImposto = Glass.Conversoes.StrParaFloatNullable((grdAjusteDocumentoFiscal.FooterRow.FindControl("txtAliquotaImposto") as TextBox).Text),
                    IdAjBenInc = Glass.Conversoes.StrParaUint((grdAjusteDocumentoFiscal.FooterRow.FindControl("selAjuste") as Glass.UI.Web.Controls.ctrlSelPopup).Valor),
                    IdNf = Glass.Conversoes.StrParaUintNullable(Request["idNf"]),
                    IdCte = Glass.Conversoes.StrParaUintNullable(Request["idCte"]),
                    IdProd = (grdAjusteDocumentoFiscal.FooterRow.FindControl("selProduto") as Glass.UI.Web.Controls.ctrlSelProduto).IdProd,
                    Obs = (grdAjusteDocumentoFiscal.FooterRow.FindControl("txtObservacao") as TextBox).Text,
                    OutrosValores = Glass.Conversoes.StrParaDecimalNullable((grdAjusteDocumentoFiscal.FooterRow.FindControl("txtOutrosValores") as TextBox).Text),
                    ValorBaseCalculoImposto = Glass.Conversoes.StrParaDecimalNullable((grdAjusteDocumentoFiscal.FooterRow.FindControl("txtValorBaseCalculoImposto") as TextBox).Text),
                    ValorImposto = Glass.Conversoes.StrParaDecimalNullable((grdAjusteDocumentoFiscal.FooterRow.FindControl("txtValorImposto") as TextBox).Text)
                };
    
                if (item.ValorImposto.GetValueOrDefault() == 0 && item.OutrosValores.GetValueOrDefault() == 0)
                    throw new Exception("Preencha o valor do ajuste (ou como \"valor do imposto\" ou como \"outros valores\").");
    
                AjusteDocumentoFiscalDAO.Instance.Insert(item);
    
                grdAjusteDocumentoFiscal.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inserir ajuste do documento.", ex, Page);
            }
        }
    
        protected void grdAjusteDocumentoFiscal_DataBound(object sender, EventArgs e)
        {
            grdAjusteDocumentoFiscal.Rows[0].Visible = grdAjusteDocumentoFiscal.Rows.Count > 1 ||
                (grdAjusteDocumentoFiscal.Rows[0].FindControl("chkExibirLinha") as CheckBox).Checked;
        }
    
        protected void grdAjusteDocumentoFiscal_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdAjusteDocumentoFiscal.ShowFooter = e.CommandName != "Edit";
        }
    
        [Ajax.AjaxMethod]
        public string VerificaProdutoNota(string produto, string idNfStr)
        {
            uint idNf = Glass.Conversoes.StrParaUint(idNfStr);
    
            if (!ProdutoDAO.Instance.ProdutoEstaNaNotaFiscal(produto, idNf))
                throw new Exception("Produto não está na nota fiscal.");
    
            return null;
        }
    }
}
