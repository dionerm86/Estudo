using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Drawing;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaContasRecebidas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (Request["rel"] == "1")
                grdConta.Columns[0].Visible = false;
    
            if (!FinanceiroConfig.FinanceiroRec.ExibirCnab)
            {
                drpArquivoRemessa.Style.Add("display", "none");
                lblArquivoRemessa.Style.Add("display", "none");
                txtNumArqRemessa.Style.Add("display", "none");
                lblArquivoRemessa2.Style.Add("display", "none");
                chkProtestadas.Style.Add("display", "none");
            }
    
            // Exibe a coluna de comiss�o
            grdConta.Columns[16].Visible = FinanceiroConfig.RelatorioContasRecebidas.ExibirComissao;
    
            grdConta.Columns[14].Visible = drpArquivoRemessa.SelectedValue != "1" && FinanceiroConfig.FinanceiroRec.ExibirCnab;

            if (!IsPostBack)
            {
                /* Chamado 11247. */
                if (FinanceiroConfig.FiltroContasVinculadasMarcadoPorPadrao &&
                    chkExibirContasVinculadas != null)
                    chkExibirContasVinculadas.Checked = true;

                lnkExportarGCON.Visible = FinanceiroConfig.GerarArquivoGCon;
                lnkExportarProsoft.Visible = FinanceiroConfig.GerarArquivoProsoft;
                lnkExportarDominio.Visible = FinanceiroConfig.GerarArquivoDominio;

                if (!Glass.Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas)
                {
                    txtIdComissao.Visible = false;
                    imbComissao.Visible = false;
                    lblComissao.Visible = false;
                    grdConta.Columns[3].Visible = false;
                }

            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdConta.PageIndex = 0;
        }
    
        protected void cblFormaPagto_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpTipoBoleto.Visible = cblFormaPagto.SelectedValue == ((uint)Glass.Data.Model.Pagto.FormaPagto.Boleto).ToString();
        }
    
        protected void chkExibirAReceber_CheckedChanged(object sender, EventArgs e)
        {
            bool exibirAReceber = ((CheckBox)sender).Checked;
            hdfExibirAReceber.Value = exibirAReceber ? "" : "true";
        }
    
        protected void grdConta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var conta = e.Row.DataItem as ContasReceber;

            if (!conta.Recebida)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Green;

            if (conta.IdArquivoRemessa.GetValueOrDefault(0) > 0)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = conta.Protestado ? Color.FromArgb(225, 200, 0) : Color.Blue;
            else if (conta.Protestado)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.FromArgb(225, 200, 0);
        }
    
        protected void drpRota_Load(object sender, EventArgs e)
        {
            if (!RotaDAO.Instance.ExisteRota())
            {
                ((DropDownList)sender).Style.Add("display", "none");
                lblRota.Style.Add("display", "none");
                imgPesqRota.Style.Add("display", "none");
            }
        }

        /// <summary>
        /// Esconde o drop de tipo entrega caso a empresa trabalhe com libera��o de pedido
        /// o filtro n�o funciona nesse caso
        /// </summary>
        protected void drpTipoEntrega_Load(object sender, EventArgs e)
        {
            var visibilidade = !PedidoConfig.LiberarPedido;

            drpTipoEntrega.Visible = visibilidade;
            Label14.Visible = visibilidade;
            imgPesq9.Visible = visibilidade;
        }
    }
}
