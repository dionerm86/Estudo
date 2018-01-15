using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstCarregamentos : System.Web.UI.Page
    {
        #region Variaveis locais
    
        private bool corAlternada = true;
    
        private double peso = 0, pesoPendente = 0, totM = 0, itens = 0,
            totMPendente = 0, itensPendentes = 0, volume = 0;

        private double pesoGerado = 0;
    
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(LstCarregamentos));

            /* Chamado 63569. */
            if (!IsPostBack && !string.IsNullOrWhiteSpace(Request["idCarregamento"]))
            {
                txtCodCarregamento.Text = Request["idCarregamento"];
                grdCarregamento.DataBind();
            }

            grdCarregamento.Visible = true;            
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCarregamento.DataBind();
        }
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }

        [Ajax.AjaxMethod]
        public string Faturar(string strCarregamento)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.EfetuarFaturamento(strCarregamento.StrParaInt());
        }

        [Ajax.AjaxMethod]
        public string BuscarErrosFaturamentoCarregamento(string strCarregamento)
        {
            var idCarregamento = strCarregamento.StrParaUint();      
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.BuscarPendenciasFaturamento(idCarregamento);
        }

        [Ajax.AjaxMethod]
        public string BuscarFaturamento(string strCarregamento)
        {
            var idCarregamento = strCarregamento.StrParaUint();
            try
            {
                return "ok||" + WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Instance.BuscarFaturamento(idCarregamento);
            }
            catch (Exception x)
            {
                return "Erro||" + x.Message.ToString();             
            }
        }

        protected void odsCarregamento_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                MensagemAlerta.ErrorMsg("Falha ao excluir carregamento.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
            {
                grdCarregamento.DataBind();
            }
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/CadCarregamento.aspx");
        }
    
        protected void imgExcluir_Command(object sender, CommandEventArgs e)
        {
            var dados = e.CommandArgument.ToString().Split(',').Select(x => Glass.Conversoes.StrParaUint(x)).ToList();
    
            try
            {
                WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Instance.RetiraOcCarregamento(dados[0], dados[1]);
                grdCarregamento.DataBind();
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao remover OC.", ex, Page);
            }
    
        }
    
        protected void imgExcluir_PreRender(object sender, EventArgs e)
        {
            var hdfIdOC = (sender as WebControl).Parent.FindControl("hdfIdOC") as HiddenField;
            var hdfIdCarregamento = (sender as WebControl).Parent.Parent.Parent.Parent.Parent.FindControl("hdfIdCarregamento") as HiddenField;
    
            ((ImageButton)sender).CommandArgument = hdfIdCarregamento.Value + "," + hdfIdOC.Value;
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
    
        protected void odsCarregamento_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados do carregamento.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    }
}
