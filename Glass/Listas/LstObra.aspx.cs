using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstObra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Listas.LstObra));

            /* Chamado 45420. */
            if (!GerarCreditoObra())
            {
                drpAgrupar.Visible = false;
                lblAgrupar.Visible = false;
                imbAgrupar.Visible = false;
            }

            if (GerarCreditoObra())
            {
                if (Request["cxDiario"] == "1")
                    lbkInserir.Visible = Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.GerarCreditoAvulsoCliente);
                else
                    lbkInserir.Visible = Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.GerarCreditoAvulsoCliente);

                grdObra.Columns[9].Visible = false;
                drpSituacao.Items[3].Enabled = false;
                Page.Title = "Gerar crédito cliente";
                lbkInserir.Text = "Cadastrar crédito";
                lbkInserir.PostBackUrl = "~/Cadastros/CadObra.aspx?gerarCredito=1";
                grdObra.EmptyDataText = "Ainda não foi gerado crédito para clientes utilizando esse cadastro.";
                grdObra.Columns[1].HeaderText = "Num.";
    
                lnkExportarExcelGerarCred.Visible = true;
                lnkImprimirGerarCred.Visible = true;
    
                lnkExportarExcelPagtoAnt.Visible = false;
                lnkImprimirPagtoAnt.Visible = false;
    
                lblNumPagto.Text = "Num.";
                tbNumPedido.Style.Add("display", "none");
            }
            else if (UsarControleNovoObra())
                lbkInserir.PostBackUrl = "~/Cadastros/CadObraNovo.aspx";
    
            if (Request["cxDiario"] == "1")
                lbkInserir.PostBackUrl += (GerarCreditoObra() ? "&" : "?") + "cxDiario=1";
    
            hdfGerarCredito.Value = GerarCreditoObra().ToString();
        }
    
        protected bool GerarCreditoObra()
        {
            return Request["gerarCredito"] == "1";
        }
    
        protected bool UsarControleNovoObra()
        {
            return PedidoConfig.DadosPedido.UsarControleNovoObra;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdObra.PageIndex = 0;
        }
    
        protected void odsObra_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar Obra.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Glass.MensagemAlerta.ShowMsg("Obra cancelada.", Page);
        }
    
        protected void grdObra_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Finalizar")
            {
                try
                {
                    uint idObra = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    decimal creditoGerado;
                    ObraDAO.Instance.FinalizarComTransacao(idObra, Request["cxDiario"] == "1", out creditoGerado);
    
                    string msg = "Obra finalizada.";
                    if (creditoGerado > 0)
                        msg += " Foi gerado um crédito de " + creditoGerado.ToString("C") + " para o cliente " + ObraDAO.Instance.GetNomeCliente(idObra, true) + ".";
    
                    Glass.MensagemAlerta.ShowMsg(msg, Page);
                    grdObra.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar Obra.", ex, Page);
                }
            }
            else if (e.CommandName == "Reabrir")
            {
                try
                {
                    uint idObra = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    ObraDAO.Instance.Reabrir(idObra);
    
                    Glass.MensagemAlerta.ShowMsg("Obra reaberta!", Page);
                    grdObra.DataBind();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar Obra.", ex, Page);
                }
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
            if (!ClienteDAO.Instance.Exists(Glass.Conversoes.StrParaUint(idCli)))
                return "Erro;Cliente não encontrado.";
            else
                return "Ok;" + ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCli));
        }
        #endregion
    }
}
