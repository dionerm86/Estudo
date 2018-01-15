using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadOrdemCarga : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadOrdemCarga));
    
            if (!IsPostBack)
            {
                txtDataIni.DataString = DateTime.Now.ToShortDateString();
                txtDataFin.DataString = DateTime.Now.AddDays(1).ToShortDateString();
                grdOrdensCarga.Columns[12].Visible = false;

                if (!OrdemCargaConfig.ControlarPedidosImportados)
                    tbClienteExterno.Style.Add("Display", "none");
            }
        }
    
        #region Variaveis locais
    
        private bool corAlternada = true;
    
        private double peso = 0, pesoPendente = 0, totM = 0, itens = 0,
            totMPendente = 0, itensPendentes = 0, volume = 0;
    
        private double pesoTotal = 0, pesoGerado = 0;
    
        #endregion
    
        #region Métodos da pagina
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    
        #endregion
    
        #region Métodos AJAX
    
        /// <summary>
        /// Gera as ocs automaticamente
        /// </summary>
        /// <param name="idsRotas"></param>
        /// <param name="idCli"></param>
        /// <param name="nomeCli"></param>
        /// <param name="dtEntPedIni"></param>
        /// <param name="dtEntPedFin"></param>
        /// <param name="idLoja"></param>
        /// <param name="tipoOC"></param>
        /// <param name="cliIgnorar"></param>
        /// <param name="pedidosObs"></param>
        /// <param name="idPedido"></param>
        /// <param name="codRotasExternas"></param>
        /// <param name="idCliExterno"></param>
        /// <param name="nomeCliExterno"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string GerarOCs(string idsRotas, string idCli, string nomeCli, string dtEntPedIni, string dtEntPedFin, string idLoja,
            string tipoOC, string cliIgnorar, string pedidosObs, string idPedido,
            string codRotasExternas, string idCliExterno, string nomeCliExterno, string fastDelivery)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Ajax.GerarOCs(idsRotas, idCli, nomeCli, dtEntPedIni, dtEntPedFin, idLoja,
                tipoOC, cliIgnorar, pedidosObs, idPedido, codRotasExternas, idCliExterno, nomeCliExterno, fastDelivery);
        }
    
        #endregion
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdOrdensCarga.DataBind();
        }
    
        protected void odsOrdemCarga_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir ordem de carga.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                grdOrdensCarga.DataBind();
            }
        }
    
        protected void grdOrdensCarga_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var row = (WebGlass.Business.OrdemCarga.Entidade.ListagemOrdemCarga)e.Row.DataItem;
    
                var cor = row.CorLinha;
    
                foreach (TableCell cell in e.Row.Cells)
                    cell.ForeColor = cor;
    
                pesoTotal += row.Peso;
            }
        }
    
        protected void grdOrdensCarga_DataBound(object sender, EventArgs e)
        {
            btnGerarCarregamento.Visible = grdOrdensCarga.Rows.Count > 0;
            btnGerarOCs.Visible = grdOrdensCarga.Rows.Count > 0;
            boxFloat.Visible = grdOrdensCarga.Rows.Count > 0;
    
            lblPesoTotal.Text = pesoTotal.ToString();
            lblPesoGerado.Text = pesoGerado.ToString();
            lblPendenteGerar.Text = Math.Round((pesoTotal - pesoGerado), 2).ToString();
    
            pesoTotal = 0;
        }
    
        protected void grdOrdemCarga_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var row = (Glass.Data.Model.OrdemCarga)e.Row.DataItem;
    
                peso += row.Peso;
                pesoPendente += row.PesoPendenteProducao;
                totM += row.TotalM2;
                totMPendente += row.TotalM2PendenteProducao;
                itens += row.QtdePecasVidro;
                itensPendentes += row.QtdePecaPendenteProducao;
                volume += row.QtdeVolumes;
    
                pesoGerado += row.Peso;
    
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[3].Text = Math.Round(peso, 2).ToString();
                e.Row.Cells[4].Text = Math.Round(pesoPendente, 2).ToString();
                e.Row.Cells[5].Text = Math.Round(totM, 2).ToString();
                e.Row.Cells[6].Text = Math.Round(itens, 2).ToString();
                e.Row.Cells[7].Text = Math.Round(totMPendente, 2).ToString();
                e.Row.Cells[8].Text = Math.Round(itensPendentes, 2).ToString();
                e.Row.Cells[9].Text = Math.Round(volume, 2).ToString();
    
                peso = 0;
                pesoPendente = 0;
                totM = 0;
                totMPendente = 0;
                itens = 0;
                itensPendentes = 0;
                volume = 0;
            }
        }
    
        protected void drpTipoOC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (grdOrdensCarga.Rows.Count == 0)
                return;
    
            grdOrdensCarga.Columns[12].Visible = Glass.Conversoes.StrParaInt(((DropDownList)sender).SelectedValue) == (int)Data.Model.OrdemCarga.TipoOCEnum.Transferencia;
        }
    }
}
