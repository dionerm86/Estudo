using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadItensCarregamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadItensCarregamento));
        }
    
        protected void btnAdicionarOCs_Click(object sender, EventArgs e)
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
    
            try
            {
                WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.AdicionaOCsCarregamento(Glass.Conversoes.StrParaUint(Request["IdCarregamento"]), ids);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Erro ao adicionar OC's", ex, this);
                return;
            }
    
            Page.ClientScript.RegisterStartupScript(this.GetType(), "finalizar", "fechaTela()", true);
        }
    
        protected void btnBuscarOCs_Click(object sender, EventArgs e)
        {
            grdOC.DataBind();
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdOC.DataBind();
        }
    
        protected void chkSelOC_CheckedChanged(object sender, EventArgs e)
        {
            CalculaPesoTotal();
        }
    
        protected void grdOC_DataBound(object sender, EventArgs e)
        {
            btnAdicionarOCs.Visible = grdOC.Rows.Count > 0;
            tdBoxFlutuante.Visible = grdOC.Rows.Count > 0;
    
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
    
            lblPeso.Text = Math.Round(WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.CalcPesoCarregamento(idsOCs), 2).ToString(); //Math.Round(_peso, 2).ToString();
        }
    
        #region Métodos AJAX
    
        [Ajax.AjaxMethod]
        public string ValidaOC(string idOC)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.ValidaOCForCarregamento(idOC);
        }
    
        [Ajax.AjaxMethod]
        public string GetIdsOCsParaCarregar(string idCliente, string nomeCliente, string idsLoja, string idRota, string idsOCs)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.GetIdsOCsParaCarregar(idCliente, nomeCliente, idsLoja, idRota, idsOCs);
        }

        [Ajax.AjaxMethod()]
        public string ValidaCarregamentoAcimaCapacidadeVeiculo(string idCarregamento, string oCsSelecionadas)
        {
            //var idsOCsSelecionada = ((HiddenField)FindControl("hdfIdsOCs")).Value;
            var listOCsCarregamento = OrdemCargaDAO.Instance.GetListForCarregamento(Glass.Conversoes.StrParaUint(idCarregamento));
            var idsOcsCarregamento = string.Join(",", listOCsCarregamento.Select(i => i.IdOrdemCarga.ToString()).ToArray());
            //var idsOCs = idsOcsCarregamento + "," + idsOCsSelecionada;

            var idsOCs = (string.IsNullOrEmpty(oCsSelecionadas) ? string.Empty : oCsSelecionadas) + idsOcsCarregamento;

            var veiculo = CarregamentoDAO.Instance.ObtemPlaca(Glass.Conversoes.StrParaUint(idCarregamento));

            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax
                .ValidaCarregamentoAcimaCapacidadeVeiculo(veiculo, idsOCs);
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
    }
}
