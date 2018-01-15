using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class lstOrdemCarga : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.lstOrdemCarga));

            if (!IsPostBack)
            {
                if (!OrdemCargaConfig.ControlarPedidosImportados)
                    tbClienteExterno.Style.Add("Display", "none");
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdOrdemCarga.DataBind();
        }
    
        protected void odsOrdemCarga_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir OC.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                grdOrdemCarga.DataBind();
            }
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/CadOrdemCarga.aspx");
        }
    
        #region Variaveis locais
    
        private bool corAlternada = true;
    
        #endregion
    
        #region Métodos AJAX
    
        [Ajax.AjaxMethod()]
        public string BuscarDadosOC(string idOC)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Ajax.BuscarDadosOC(idOC);
        }
    
        [Ajax.AjaxMethod()]
        public string PodeAdicionarPedidoOC(string idOC)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Ajax.PodeAdicionarPedidoOC(idOC);
        }
    
        #endregion
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    
        protected void grdPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var pedido = (Glass.Data.Model.Pedido)e.Row.DataItem;
    
                if (!string.IsNullOrEmpty(pedido.ObsLiberacao) || pedido.QtdePecaPendenteProducao > 0 || !pedido.GerouTodosVolumes)
                    foreach (TableCell cell in e.Row.Cells)
                        cell.ForeColor = System.Drawing.Color.Red;
            }
        }
    
        protected void imgExcluir_Command(object sender, CommandEventArgs e)
        {
            var dados = e.CommandArgument.ToString().Split(',').Select(x => Glass.Conversoes.StrParaUint(x)).ToList();
    
            try
            {
                WebGlass.Business.OrdemCarga.Fluxo.PedidosOCFluxo.Instance.RemoverPedido(dados[0], dados[1]);
                grdOrdemCarga.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao remover Pedido.", ex, Page);
            }
            
        }
    
        protected void imgExcluir_PreRender(object sender, EventArgs e)
        {
            var hdfPedido = (sender as WebControl).Parent.FindControl("hdfIdPedido") as HiddenField;
            var hdfIdOc = (sender as WebControl).Parent.Parent.Parent.Parent.Parent.FindControl("hdfIdOC") as HiddenField;
    
            ((ImageButton)sender).CommandArgument = hdfIdOc.Value + "," + hdfPedido.Value;
        }

        protected void grdOrdemCarga_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var idCliente = ((HiddenField)e.Row.Cells[3].FindControl("hdfIdCliente")).Value.StrParaUint();
                var importacao = ClienteDAO.Instance.ClienteImportacao(idCliente);

                if (importacao)
                    return;

                var grdP = (GridView)e.Row.Cells[17].FindControl("grdPedidos");

                grdP.Columns[2].Visible = false;
                grdP.Columns[3].Visible = false;
            }
        }
    }
}
