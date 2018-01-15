using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Drawing;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadItensOrdemCarga : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadItensOrdemCarga));
    
            Page.Title = "Pedidos de "
                + (Glass.Conversoes.StrParaInt(Request["tipoOC"]) == (int)Glass.Data.Model.OrdemCarga.TipoOCEnum.Venda ? "Venda" : "Transferência")
                + " da Ordem de Carga";
    
            if (Glass.Conversoes.StrParaUint(Request["idOC"]) > 0)
                btnFinalizar.Text = "Adicionar Pedidos";

            if (!IsPostBack)
            {
                if (!Glass.Configuracoes.OrdemCargaConfig.ControlarPedidosImportados || (!ClienteDAO.Instance.ClienteImportacao(Request["IdCliente"].StrParaUint()) && !ClienteDAO.Instance.ClienteImportacao(Request["idCliente"].StrParaUint())))
                {
                    grdPedidos.Columns[4].Visible = false;
                    grdPedidos.Columns[5].Visible = false;
                }
            }
        }
    
        #region Métodos da pagina
    
        private bool corAlternada = true;
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    
        #endregion
    
        protected void btnFinalizar_Click(object sender, EventArgs e)
        {
            try
            {
                uint idOC = Glass.Conversoes.StrParaUint(Request["idOC"]);
                uint idCliente = Glass.Conversoes.StrParaUint(Request["idCliente"]);
                uint idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                var tipoOC = (Glass.Data.Model.OrdemCarga.TipoOCEnum)Glass.Conversoes.StrParaInt(Request["tipoOC"]);
                uint idRota = Glass.Conversoes.StrParaUint(Request["idRota"]);
                string dtEntPedidoIni = Request["dtEntPedidoIni"];
                string dtEntPedidoFin = Request["dtEntPedidoFin"];
    
                List<string> pedidos = new List<string>();
    
                foreach (GridViewRow row in grdPedidos.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        var chkPedido = row.Cells[1].FindControl("chkSelPed");
    
                        if (chkPedido != null && ((CheckBox)chkPedido).Checked)
                            pedidos.Add(((HiddenField)row.Cells[1].FindControl("hdfIdPedido")).Value);
                    }
                }
    
                if (idOC > 0)
                    WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Instance.AdicionarPedidosOC(idOC, string.Join(",", pedidos.ToArray()), tipoOC);
                else
                    WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Instance.FinalizarOC(idCliente, idLoja, tipoOC, idRota,
                        dtEntPedidoIni, dtEntPedidoFin, string.Join(",", pedidos.ToArray()));
    
                Page.ClientScript.RegisterStartupScript(this.GetType(), "finalizar", "finalizarOC()", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao " + (Glass.Conversoes.StrParaUint(Request["idOC"]) > 0 ? "finalizar OC." : "adicionar Pedidos."), ex, Page);
            }
        }
    
        protected void grdPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var pedido = (Glass.Data.Model.Pedido)e.Row.DataItem;
    
                if(!string.IsNullOrEmpty(pedido.ObsLiberacao) || pedido.QtdePecaPendenteProducao > 0 || !pedido.GerouTodosVolumes)
                    foreach (TableCell cell in e.Row.Cells)
                        cell.ForeColor = Color.Red;
            }
        }
    
        protected void chkSelPed_CheckedChanged(object sender, EventArgs e)
        {
            double pesoTotal = 0;
            double totM = 0;
            int qtdeItens = 0;
            int qtdeVolumes = 0;
    
            foreach (GridViewRow row in grdPedidos.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    if (((CheckBox)row.Cells[2].FindControl("chkSelPed")).Checked)
                    {
    
                        pesoTotal += Glass.Conversoes.StrParaDouble(((HiddenField)row.FindControl("hdfPeso")).Value);
                        totM += Glass.Conversoes.StrParaDouble(((HiddenField)row.FindControl("hdfTotM")).Value);
                        qtdeItens += Glass.Conversoes.StrParaInt(((HiddenField)row.FindControl("hdfQtdeItens")).Value);
                        qtdeVolumes += Glass.Conversoes.StrParaInt(((HiddenField)row.FindControl("hdfQtdeVolumes")).Value);
                    }
                }
            }
    
            lblPeso.Text = Math.Round(pesoTotal, 2).ToString();
            lblTotM.Text = Math.Round(totM, 2).ToString();
            lblQtdeItens.Text = qtdeItens.ToString();
            lblQtdeVolumes.Text = qtdeVolumes.ToString();
        }
    }
}
