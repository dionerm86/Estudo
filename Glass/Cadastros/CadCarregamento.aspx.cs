using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCarregamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadCarregamento));
    
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request["idsRotas"]) && !string.IsNullOrEmpty(Request["dtEntPedidoIni"]) &&
                    !string.IsNullOrEmpty(Request["dtEntPedidoFin"]) && !string.IsNullOrEmpty(Request["idLoja"]))
                {
                    hdfIdsOCs.Value = WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance
                        .GetIdsOCsForGerarCarregamento(Request["idsRotas"], Request["idLoja"], Request["dtEntPedIni"], Request["dtEntPedFin"]);
                    grdOC.DataBind();
                }
            }
        }
    
        protected void btnBuscarOCs_Click(object sender, EventArgs e)
        {
            grdOC.DataBind();
        }
    
        #region Métodos AJAX
    
        [Ajax.AjaxMethod]
        public string ValidaOC(string idOC)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.ValidaOCForCarregamento(idOC);
        }
    
        [Ajax.AjaxMethod]
        public string GetIdsOCsParaCarregar(string idCliente, string nomeCliente, string idLoja, string idRota, string idsOCs)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.GetIdsOCsParaCarregar(idCliente, nomeCliente, idLoja, idRota, idsOCs);
        }
    
        #endregion
    
        #region Variaveis Locais
    
        private bool corAlternada = true;
        private double _peso = 0;
    
        #endregion
    
        #region Métodos usados na página
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    
        #endregion
    
        protected void btnFinalizarCarregamento_Click(object sender, EventArgs e)
        {
            List<uint> idsOCs = new List<uint>();
            
    
            foreach (GridViewRow row in grdOC.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    var grdOCs = row.Cells[4].FindControl("grdOCs");
    
                    if (grdOCs == null)
                        continue;
    
                    foreach (GridViewRow ocRow in ((GridView)grdOCs).Rows)
                        if (((CheckBox)ocRow.Cells[0].FindControl("chkSelOC")).Checked)
                            idsOCs.Add(Glass.Conversoes.StrParaUint(((HiddenField)ocRow.Cells[0].FindControl("hdfIdOC")).Value));
                }
            }
    
            if (idsOCs.Count == 0)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhuma OC foi selecionada.", Page);
                return;
            }
    
            var ids = string.Join(",", idsOCs.Select(i => i.ToString()).ToArray());
    
            Page.ClientScript.RegisterStartupScript(this.GetType(), "finalizarCarregamento",
                "finalizarCarregamento('" + drpLoja.SelectedValue + "','" + ids + "');", true);
        }
    
        protected void grdOC_DataBound(object sender, EventArgs e)
        {
            btnFinalizarCarregamento.Visible = grdOC.Rows.Count > 0;
            tdBoxFlutuante.Visible = grdOC.Rows.Count > 0;
    
            CalculaPesoTotal();
        }
    
        protected void chkSelOC_CheckedChanged(object sender, EventArgs e)
        {
            CalculaPesoTotal();
        }
    
        private void CalculaPesoTotal()
        {
            _peso = 0;
            var idsOCs = new List<uint>();
    
            foreach (GridViewRow row in grdOC.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    var grdOCs = row.Cells[4].FindControl("grdOCs");
    
                    if (grdOCs == null)
                        continue;
    
                    foreach (GridViewRow ocRow in ((GridView)grdOCs).Rows)
                        if (((CheckBox)ocRow.Cells[0].FindControl("chkSelOC")).Checked)
                        {
                            _peso += Glass.Conversoes.StrParaDouble(((HiddenField)ocRow.Cells[0].FindControl("hdfPeso")).Value);
                            idsOCs.Add(Glass.Conversoes.StrParaUint(((HiddenField)ocRow.Cells[0].FindControl("hdfIdOC")).Value));
                        }
                }
            }
    
            lblPeso.Text = Math.Round(WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.CalcPesoCarregamento(idsOCs),2).ToString(); //Math.Round(_peso, 2).ToString();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdOC.DataBind();
        }
    }
}
