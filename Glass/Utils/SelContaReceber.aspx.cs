using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System.Collections.Generic;
using System.Drawing;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SelContaReceber : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["IdCli"] != null)
            {
                if (!IsPostBack)
                {
                    txtNumCli.Text = Request["IdCli"];
                    txtNome.Text = Request["NomeCli"];

                    /* Chamado 11247. */
                    if (!IsPostBack && FinanceiroConfig.FiltroContasVinculadasMarcadoPorPadrao &&
                        chkContasVinculadas != null)
                        chkContasVinculadas.Checked = true;

                    // Verifica se o cliente possui algum cheque devolvido
                    if (!String.IsNullOrEmpty(txtNumCli.Text))
                    {
                        uint idCliente = Glass.Conversoes.StrParaUint(txtNumCli.Text);
                        int qtdDev = ChequesDAO.Instance.ObtemQtdChequeDevolvido(idCliente);
                        int qtdProt = ChequesDAO.Instance.ObtemQtdChequeProtestado(idCliente);
    
                        if (qtdDev > 0)
                            lblMsg.Text = "Este cliente possui " + qtdDev + " cheque(s) devolvido(s)";
    
                        if (qtdProt > 0)
                        {
                            if (qtdDev > 0)
                                lblMsg.Text += " e " + qtdProt + " cheque(s) protestado(s)";
                            else
                                lblMsg.Text = "Este cliente possui " + qtdProt + " cheque(s) protestado(s)";
                        }
    
                        if (lblMsg.Text != "")
                            lblMsg.Text += ".";
    
                        mensagem.Visible = lblMsg.Text != "";
                    }
    
                    if (FinanceiroConfig.FinanceiroRec.ImpedirRecebimentoPorLoja && UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.Administrador)
                    {
                        drpLoja.SelectedValue = UserInfo.GetUserInfo.IdLoja.ToString();
                        lblLoja.Style.Add("display", "none");
                        drpLoja.Style.Add("display", "none");
                    }
                }
    
                LoginUsuario login = UserInfo.GetUserInfo;
    
                txtNumCli.Enabled = false;
                txtNome.Enabled = txtNumCli.Enabled;            
            }
    
            if (!IsPostBack)
            {
                odsContasReceber.SelectParameters["buscarContasValorZerado"].DefaultValue = (Request["desconto"] == "1").ToString();
    
                drpTipo.Items.Add(new ListItem(FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil, ((int)ContasReceber.TipoContaEnum.Contabil).ToString()));
                drpTipo.Items.Add(new ListItem(FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil, ((int)ContasReceber.TipoContaEnum.NaoContabil).ToString()));
    
                if (Request["desconto"] == "1")
                    lnkAddAll.Visible = false;
    
                if (!FinanceiroConfig.FinanceiroRec.ExibirCnab)
                    lblCNAB.Visible = false;
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SelContaReceber));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), "desabilitaUnload", "window.onunload = null;\n");
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
    
        #endregion
    
        protected void lnkAddAll_Click(object sender, EventArgs e)
        {
            // Busca as contas a receber que estiverem na tela
            List<ContasReceber> lstContaRec = new List<ContasReceber>((List<ContasReceber>)odsContasReceber.Select());
            
            // Gera o script para adicionar todas elas na tela
            string script = String.Empty;

            if (lstContaRec.Count > 0)
            {
                foreach (ContasReceber conta in lstContaRec)
                {
                    script += "window.opener.setContaReceber(" + conta.IdContaR + ",'" + conta.IdPedido + "','" + conta.PedidosLiberacao + "','" +
                        conta.NomeCli.ToString().Replace("'", "") + "','" + conta.ValorVec.ToString("C") + "','" + conta.DataVec.ToString("d") + "', '" +
                        conta.Juros + "', '" + conta.Multa + "', '" + conta.ObsScript + "', " + (Request["acerto"] != "1" ? "" :
                        "'" + conta.DescricaoContaContabil + "', ") + "window); ";
                }

                if (Request["encontroContas"] == "1")
                    script += "window.opener.redirectUrl(window.opener.location.href); ";
    
                script += "closeWindow()";
            }
            else
                script += "alert('Não há conta a receber para ser adicionada.');";

            ClientScript.RegisterStartupScript(typeof(string), "addAll", script, true);
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void PlaceHolder1_Load(object sender, EventArgs e)
        {
            ((PlaceHolder)sender).Visible = Request["encontroContas"] == "1" || Request["desconto"] != "1";
        }
    
        protected void ImageButton3_Load(object sender, EventArgs e)
        {
            ((ImageButton)sender).Visible = Request["desconto"] == "1";
        }
    
        protected void grdProduto_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //var conta = e.Row.DataItemIndex as ContasReceber;
            var contaR = e.Row.DataItem as ContasReceber;
            if(contaR != null)
                foreach (var item in hdfIdsEsconder.Value.Split(','))
                {
                   if (contaR.IdContaR == item.StrParaUint())
                        e.Row.Visible = false;
                }

            // Verifica se a linha é de dados
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            // Preenche de Azul as linhas de contas com CNAB gerado.
            if (FinanceiroConfig.FinanceiroRec.ExibirCnab)
            {
                if(contaR.GerouCNAB)
                    foreach (TableCell cell in e.Row.Cells)
                        cell.ForeColor = Color.Blue;
            }

            // Preenche de Amarelo as linhas de contas com CNAB gerado.
            if (FinanceiroConfig.ContasReceber.UtilizarControleContaReceberJuridico && contaR.Juridico)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.FromArgb(225, 200, 0);
        }
    }
}
