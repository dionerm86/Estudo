using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using System.Drawing;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadVolume : System.Web.UI.Page
    {
        #region Variaveis locais
    
        private bool corAlternada = true;
        private double _totM = 0, _pesoTotal = 0, _qtdeTotal = 0;
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadVolume));

            if(!IsPostBack)
            {
                ((TextBox)ctrlDataEntIni.FindControl("txtData")).Text = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataEntFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                lblPeriodoLiberacaoPedido.Visible = false;
                imbPeriodoLiberacaoPedido.Visible = false;
                ctrlDataLibIni.Visible = false;
                ctrlDataLibFim.Visible = false;

                tbClienteExterno.Visible = OrdemCargaConfig.ControlarPedidosImportados;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedidos.DataBind();
        }
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    
        protected void grdVolume_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
    
                var vol = (Volume)e.Row.DataItem;
    
                _totM += vol.TotM;
                _pesoTotal += vol.PesoTotal;
                _qtdeTotal += vol.QtdeItens;
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[2].Text = Math.Round(_qtdeTotal, 2).ToString();
                e.Row.Cells[3].Text = Math.Round(_pesoTotal, 2).ToString();
                e.Row.Cells[4].Text = Math.Round(_totM, 2).ToString();
    
                _totM = 0;
                _pesoTotal = 0;
                _qtdeTotal = 0;
            }
        }
    
        protected void odsVolume_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir volume.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                grdPedidos.DataBind();
            }
        }
    
        protected void grdPedidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var situacao = ((Glass.Data.Model.Pedido)e.Row.DataItem).SituacaoVolume;
    
                if (situacao == Glass.Data.Model.Pedido.SituacaoVolumeEnum.SemVolume)
                    foreach (TableCell cell in e.Row.Cells)
                        cell.ForeColor = Color.Red;
                else if(situacao == Glass.Data.Model.Pedido.SituacaoVolumeEnum.Pendente)
                    foreach (TableCell cell in e.Row.Cells)
                        cell.ForeColor = Color.Blue;
            }
        }
    
    }
}
