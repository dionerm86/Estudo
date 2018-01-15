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
    public partial class ListaCredito : System.Web.UI.Page
    {
        private List<uint> planosContaEstorno = UtilsPlanoConta.GetLstCredito(1);
        private List<uint> planosContaGeracao = UtilsPlanoConta.GetLstCredito(2);
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Relatorios.ListaCredito));
    
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    
        #region Métodos AJAX
    
        /// <summary>
        /// Busca o cliente em tempo real
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GetCli(string idCli)
        {
            if (String.IsNullOrEmpty(idCli) || !ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }

        /// <summary>
        /// Busca o IdAcertoParcial.
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string ObterIdAcertoParcial(string idContaR)
        {
            var idAcertoParcial = ContasReceberDAO.Instance.ObterIdAcertoParcial(idContaR.StrParaInt());
            return idAcertoParcial.GetValueOrDefault() == 0 ? string.Empty : idAcertoParcial.Value.ToString();
        }

        /// <summary>
        /// Busca o IdObra.
        /// </summary>
        [Ajax.AjaxMethod()]
        public string ObterIdObra(string idContaR)
        {
            var idObra = ContasReceberDAO.Instance.ObterIdObra(null, idContaR.StrParaInt());
            return idObra.GetValueOrDefault() == 0 ? string.Empty : idObra.Value.ToString();
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
    
            switch (((HiddenField)e.Row.FindControl("hdfTipoMov")).Value)
            {
                case "1":
                case "2":
                    ((Label)e.Row.FindControl("lblCredito")).Visible = true;
                    ((Label)e.Row.FindControl("lblDebito")).Visible = false;
                    break;
    
                case "3":
                case "4":
                    ((Label)e.Row.FindControl("lblCredito")).Visible = false;
                    ((Label)e.Row.FindControl("lblDebito")).Visible = true;
                    break;
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
    
            if (!String.IsNullOrEmpty(txtNumCli.Text))
            {
                CreditoDAO.TotaisCredito totais = CreditoDAO.Instance.GetTotaisCredito(Glass.Conversoes.StrParaUint(txtNumCli.Text),
                    Glass.Conversoes.StrParaDate(((TextBox)ctrlDataIni.FindControl("txtData")).Text).Value,
                    Glass.Conversoes.StrParaDate(((TextBox)ctrlDataFim.FindControl("txtData")).Text).Value, ddoTipoMov.SelectedValue);
    
                lblCreditoGerado.Text = totais.Gerado.ToString("C");
                lblCreditoUtilizado.Text = totais.Utilizado.ToString("C");
                lblCreditoAtual.Text = ClienteDAO.Instance.GetCredito(Glass.Conversoes.StrParaUint(txtNumCli.Text)).ToString("C");
            }
        }
    }
}
