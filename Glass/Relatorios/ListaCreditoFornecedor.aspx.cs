using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System.Collections.Generic;
using Glass.Data.RelDAL;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaCreditoFornecedor : System.Web.UI.Page
    {
        private List<uint> planosContaEstorno = UtilsPlanoConta.GetLstCredito(1);
        private List<uint> planosContaGeracao = UtilsPlanoConta.GetLstCredito(2);
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.ListaCreditoFornecedor));
    
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Busca o fornecedor em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetFornecedor(string idFornec)
        {
            if (String.IsNullOrEmpty(idFornec) || !FornecedorDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idFornec)))
                return "Erro;Fornecedor não encontrado.";
            else
                return "Ok;" + FornecedorDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idFornec));
        }
    
        #endregion
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCredito.DataBind();
            grdCredito.PageIndex = 0;
            lnkImprimir.Visible = grdCredito.Rows.Count > 0;
        }
    
        protected void grdCredito_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.FindControl("hdfIdConta") == null)
                return;
    
            uint idConta = Glass.Conversoes.StrParaUint(((HiddenField)e.Row.FindControl("hdfIdConta")).Value);
            
            if (planosContaEstorno.Contains(idConta))
            {
                foreach (TableCell celula in e.Row.Cells)
                    celula.ForeColor = System.Drawing.Color.Red;
            }
            else if (planosContaGeracao.Contains(idConta))
            {
                foreach (TableCell celula in e.Row.Cells)
                    celula.ForeColor = System.Drawing.Color.Blue;
            }
        }
    
        protected void grdCredito_Sorting(object sender, GridViewSortEventArgs e)
        {
            hdfSort.Value = e.SortExpression;
            if (e.SortDirection == SortDirection.Descending)
                hdfSort.Value += " DESC";
        }
    
        protected void odsCredito_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (!(e.ReturnValue is IEnumerable))
                return;
    
            if (!String.IsNullOrEmpty(txtNumFornecedor.Text))
            {
                CreditoDAO.TotaisCredito totais = CreditoDAO.Instance.GetTotaisCreditoFornecedor(Glass.Conversoes.StrParaUint(txtNumFornecedor.Text), 
                    Glass.Conversoes.StrParaDate(((TextBox)ctrlDataIni.FindControl("txtData")).Text).Value, 
                    Glass.Conversoes.StrParaDate(((TextBox)ctrlDataFim.FindControl("txtData")).Text).Value, ddoTipoMov.SelectedValue);
    
                lblCreditoGerado.Text = totais.Gerado.ToString("C");
                lblCreditoUtilizado.Text = totais.Utilizado.ToString("C");
                lblCreditoAtual.Text = FornecedorDAO.Instance.GetCredito(Glass.Conversoes.StrParaUint(txtNumFornecedor.Text)).ToString("C");
            }
        }
    }
}
