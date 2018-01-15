using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Utils
{
    public partial class DocRefNotaFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.DocRefNotaFiscal));
    
            docArrec.Visible = Request["idNf"] != null;
            docFisc.Visible = Request["idNf"] != null;
        }
    
        protected uint? GetIdNf()
        {
            return Glass.Conversoes.StrParaUintNullable(Request["idNf"]);
        }
    
        protected uint? GetIdCte()
        {
            return Glass.Conversoes.StrParaUintNullable(Request["idCte"]);
        }
    
        #region Processo referenciado
    
        protected void grdProcRef_DataBound(object sender, EventArgs e)
        {
            if (grdProcRef.Rows.Count == 1)
                grdProcRef.Rows[0].Visible = ProcessoReferenciadoDAO.Instance.GetCountReal(GetIdNf().GetValueOrDefault(), 
                    GetIdCte().GetValueOrDefault()) > 0;
            else
                grdProcRef.Rows[0].Visible = true;
        }
    
        protected void grdProcRef_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdProcRef.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void imgAddProc_Click(object sender, ImageClickEventArgs e)
        {
            ProcessoReferenciado novo = new ProcessoReferenciado();
            novo.IdNf = GetIdNf();
            novo.IdCte = GetIdCte();
            novo.Origem = Glass.Conversoes.StrParaInt(((DropDownList)grdProcRef.FooterRow.FindControl("drpOrigem")).SelectedValue);
            novo.Numero = ((TextBox)grdProcRef.FooterRow.FindControl("txtNumero")).Text;
    
            uint idProcRef = ProcessoReferenciadoDAO.Instance.Insert(novo);
            grdProcRef.DataBind();
            grdProcRef_RowCommand(sender, new GridViewCommandEventArgs(sender, new CommandEventArgs("Documentos", idProcRef)));
        }
    
        #endregion
    
        #region Documento arrecadação
    
        protected void grdDocArrec_DataBound(object sender, EventArgs e)
        {
            if (grdDocArrec.Rows.Count == 1)
                grdDocArrec.Rows[0].Visible = DocumentoArrecadacaoDAO.Instance.GetCountReal(GetIdNf().GetValueOrDefault()) > 0;
            else
                grdDocArrec.Rows[0].Visible = true;
        }
    
        protected void grdDocArrec_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdDocArrec.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void imgAddDocArrec_Click(object sender, ImageClickEventArgs e)
        {
            DocumentoArrecadacao novo = new DocumentoArrecadacao();
            novo.IdNf = GetIdNf().GetValueOrDefault();
            novo.CodAutBanco = ((TextBox)grdDocArrec.FooterRow.FindControl("txtCodAutBanco")).Text;
            novo.CodTipo = Glass.Conversoes.StrParaInt(((DropDownList)grdDocArrec.FooterRow.FindControl("drpTipo")).SelectedValue);
            novo.DataPagto = ((Controls.ctrlData)grdDocArrec.FooterRow.FindControl("ctrlDataPagto")).Data;
            novo.DataVenc = ((Controls.ctrlData)grdDocArrec.FooterRow.FindControl("ctrlDataVenc")).Data;
            novo.Numero = ((TextBox)grdDocArrec.FooterRow.FindControl("txtNumero")).Text;
            novo.Uf = ((DropDownList)grdDocArrec.FooterRow.FindControl("drpUf")).SelectedValue;
            novo.Valor = Glass.Conversoes.StrParaDecimal(((TextBox)grdDocArrec.FooterRow.FindControl("txtValor")).Text);
    
            DocumentoArrecadacaoDAO.Instance.Insert(novo);
            grdDocArrec.DataBind();
        }
    
        #endregion
    
        #region Documento fiscal
    
        protected void grdDocFiscal_DataBound(object sender, EventArgs e)
        {
            if (grdDocFiscal.Rows.Count == 1)
                grdDocFiscal.Rows[0].Visible = DocumentoFiscalDAO.Instance.GetCountReal(GetIdNf().GetValueOrDefault()) > 0;
            else
                grdDocFiscal.Rows[0].Visible = true;
        }
    
        protected void grdDocFiscal_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            grdDocFiscal.ShowFooter = e.CommandName != "Edit";
        }
    
        protected void imgAddDocFiscal_Click(object sender, ImageClickEventArgs e)
        {
            Controls.ctrlSelParticipante selPart = (Controls.ctrlSelParticipante)grdDocFiscal.FooterRow.FindControl("ctrlSelParticipante1");
    
            DocumentoFiscal novo = new DocumentoFiscal();
            novo.IdNf = GetIdNf().GetValueOrDefault();
            novo.IdCliente = selPart.IdCliente;
            novo.IdFornec = selPart.IdFornec;
            novo.IdLoja = selPart.IdLoja;
            novo.IdTransportador = selPart.IdTransportador;
            novo.DataEmissao = ((Controls.ctrlData)grdDocFiscal.FooterRow.FindControl("ctrlDataEmissao")).Data;
            novo.Emitente = Glass.Conversoes.StrParaInt(((DropDownList)grdDocFiscal.FooterRow.FindControl("drpEmitente")).SelectedValue);
            novo.Modelo = ((TextBox)grdDocFiscal.FooterRow.FindControl("txtModelo")).Text;
            novo.Numero = ((TextBox)grdDocFiscal.FooterRow.FindControl("txtNumero")).Text;
            novo.Serie = ((TextBox)grdDocFiscal.FooterRow.FindControl("txtSerie")).Text;
            novo.SubSerie = ((TextBox)grdDocFiscal.FooterRow.FindControl("txtSubserie")).Text;
            novo.Tipo = Glass.Conversoes.StrParaInt(((DropDownList)grdDocFiscal.FooterRow.FindControl("drpTipo")).SelectedValue);
    
            DocumentoFiscalDAO.Instance.Insert(novo);
            grdDocFiscal.DataBind();
        }
    
        #endregion
    }
}
