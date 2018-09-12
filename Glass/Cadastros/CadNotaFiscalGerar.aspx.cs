using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections;
using System.Drawing;
using Glass.Data.NFeUtils;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadNotaFiscalGerar : System.Web.UI.Page
    {
        private bool dataBoundPedido = false;
        private bool dataBoundOC = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof (MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof (Glass.UI.Web.Cadastros.CadNotaFiscalGerar));

            Label2.Visible = PedidoConfig.LiberarPedido;
            txtLiberacao.Visible = PedidoConfig.LiberarPedido;
            imbAddLib.Visible = PedidoConfig.LiberarPedido;

            txtNumPedido.Enabled = hdfBuscarIdsLiberacoes.Value.Length == 0;
            txtLiberacao.Enabled = hdfBuscarIdsPedidos.Value.Length > 0 ? hdfBuscarIdsLiberacoes.Value.Length > 0 : true;

            imbAddPed.Enabled = txtNumPedido.Enabled;
            imbAddLib.Enabled = txtLiberacao.Enabled;

            // Permite gerar nota apenas de liberação
            /* Chamado 16662.
             * Os campos do pedido devem ser escondidos somente se a empresa trabalhar com liberação de pedido. */
            //if (FinanceiroConfig.FinanceiroRec.GerarNotaApenasDeLiberacao || FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            if ((FinanceiroConfig.FinanceiroRec.GerarNotaApenasDeLiberacao ||
                 FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber) &&
                Configuracoes.PedidoConfig.LiberarPedido)
            {
                pedido_titulo.Visible = false;
                pedido_campo.Visible = false;
                pedido_buscar.Visible = false;
                pedido_separa.Visible = false;
                cliente.Visible = false;
                ctrlDataIni.Visible = false;
                ctrlDataFim.Visible = false;
            }

            txtIdCli.Text = "";
            txtNomeCli.Text = "";

            if (!IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request["idLiberarPedido"]))
                {
                    txtLiberacao.Text = Request["idLiberarPedido"];
                    ClientScript.RegisterStartupScript(GetType(), "adlib", "addLiberacao();", true);
                }

                if (drpTipoNota.SelectedValue == "1" &&
                    Configuracoes.FiscalConfig.NotaFiscalConfig.ExportarNotaFiscalOutroBD)
                    chkTransferirNf.Visible = true;

                btnGerarNfc.Visible = FiscalConfig.UtilizaNFCe;
            }

            if (!OrdemCargaConfig.UsarControleOrdemCarga || !PedidoConfig.ExibirOpcaoDeveTransferir)
            {
                lblTipoNota.Style.Add("display", "none");
                drpTipoNota.Style.Add("display", "none");
            }
        }

        protected void imgPesq_Click(object sender, EventArgs e)
        {
            grdPedidos.DataBind();
        }

        protected void odsLoja_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (!(e.ReturnValue is IEnumerable))
                return;

            drpLoja.SelectedIndex =
                drpLoja.Items.IndexOf(drpLoja.Items.FindByValue(UserInfo.GetUserInfo.IdLoja.ToString()));
        }

        protected void btnBuscarPedidos_Click(object sender, EventArgs e)
        {
            grdPedidos.DataBind();
            BuscaClientesVinculados(hdfBuscarIdsPedidos.Value.Split(',')[0]);
        }

        protected void odsPedidos_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue is IEnumerable)
            {
                Glass.Data.Model.Pedido[] pedidos = (Glass.Data.Model.Pedido[]) e.ReturnValue;
                lblMensagem.Text = String.Empty;
                btnGerarNf.Enabled = false;
                gerar.Visible = pedidos.Length > 0;

                var lojas = pedidos.Select(x => x.IdLoja).Distinct().ToList();
                if (lojas.Count == 1)
                {
                    if (drpLoja.Items.Count == 0) drpLoja.DataBind();
                    drpLoja.SelectedValue = lojas[0].ToString();
                }

                if (pedidos.Length == 0)
                    lblMensagem.Text = "Selecione pelo menos um pedido para gerar a nota.";
                else
                {
                    drpTipoNota.Enabled = false;

                    List<uint> clientes = new List<uint>();
                    foreach (Glass.Data.Model.Pedido p in pedidos)
                        if (!clientes.Contains(p.IdCli))
                            clientes.Add(p.IdCli);

                    if (drpTipoNota.SelectedValue == "1" && clientes.Count > 1)
                    {
                        lblMensagem.Text = "Há pedidos selecionados de mais de um cliente.";
                        return;
                    }

                    btnGerarNf.Enabled = true;

                    if (drpTipoNota.SelectedValue == "2")
                    {
                        txtNumCarregamento.Enabled = false;
                        imbAddCarregamento.Enabled = false;
                    }
                    else
                        ConfiguraPercReducao(clientes[0]);
                }
            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            hdfBuscarIdsPedidos.Value = "";
            hdfBuscarIdsLiberacoes.Value = "";
        }

        protected void grdPedidos_DataBound(object sender, EventArgs e)
        {
            dataBoundPedido = true;

            float total = 0;

            for (int i = 0; i < grdPedidos.Rows.Count; i++)
            {
                uint idPedido =
                    Glass.Conversoes.StrParaUint(
                        ((HiddenField) grdPedidos.Rows[i].Cells[0].FindControl("hdfIdPedido")).Value);
                string notasGeradas = PedidosNotaFiscalDAO.Instance.NotasFiscaisGeradas(null, idPedido);

                hdfIdCliente.Value = PedidoDAO.Instance.ObtemIdCliente(null, idPedido).ToString();

                HiddenField hdfNotasGeradas = (HiddenField) grdPedidos.Rows[i].Cells[0].FindControl("hdfNotasGeradas");
                hdfNotasGeradas.Value = notasGeradas;
                Color corLinha = !String.IsNullOrEmpty(notasGeradas) ? Color.Red : Color.Black;

                for (int j = 0; j < grdPedidos.Rows[i].Cells.Count; j++)
                    grdPedidos.Rows[i].Cells[j].ForeColor = corLinha;

                if (txtLiberacao.Enabled && !string.IsNullOrEmpty(hdfIdsLiberacaoPedidos.Value))
                {
                    float valorLiberado = 0;

                    foreach (string liberacaoPedidos in hdfIdsLiberacaoPedidos.Value.Split(';'))
                    {
                        List<string> pedidos = new List<string>(liberacaoPedidos.Split('&')[1].Replace(" ", "").Split(','));

                        if (!string.IsNullOrEmpty(pedidos.Find(delegate(string s) { return s == idPedido.ToString(); })))
                        {
                            // Esta configuração define se a empresa trabalha com liberação parcial ou não.
                            if (Liberacao.DadosLiberacao.LiberarPedidoProdutos)
                                // Este valor ficará incorreto em caso de cobrança da taxa de fast delivery.
                                valorLiberado += PedidoDAO.Instance
                                    .GetTotalLiberado(idPedido, liberacaoPedidos.Split('&')[0]);
                            /* Chamado 14959.
                             * Caso a empresa não trabalhe com liberação parcial de pedido então o valor total do pedido deve ser considerado. */
                            else
                            {
                                if (PCPConfig.UsarConferenciaFluxo)
                                    valorLiberado =
                                        Conversoes.StrParaFloat(PedidoEspelhoDAO.Instance.GetTotal(idPedido).ToString());
                                else
                                    valorLiberado =
                                        Conversoes.StrParaFloat(PedidoDAO.Instance.GetTotal(null, idPedido).ToString());
                            }
                        }
                    }

                    if (valorLiberado > 0)
                        if (((Label) grdPedidos.Rows[i].FindControl("lblTotalPedido")).Visible)
                            ((Label) grdPedidos.Rows[i].FindControl("lblTotalPedido")).Text = valorLiberado.ToString("C");
                        else
                            ((Label) grdPedidos.Rows[i].FindControl("lblTotalPedidoEsp")).Text =
                                valorLiberado.ToString("C");
                }

                Label totalPedido = (Label) grdPedidos.Rows[i].FindControl("lblTotalPedido");
                Label totalPedidoEsp = (Label) grdPedidos.Rows[i].FindControl("lblTotalPedidoEsp");

                string totalStr = totalPedido.Visible ? totalPedido.Text : totalPedidoEsp.Text;

                float totalLinha = Glass.Conversoes.StrParaFloat(totalStr.Replace("R$", "").Replace(" ", "").Replace(".", ""));
                total += totalLinha;
            }

            lblTotal.Text = total.ToString("C");

            if (!dataBoundOC)
                grdOcs_DataBound(null, null);
        }

        protected void ddlClienteVinculado_DataBound(object sender, EventArgs e)
        {
            if (ddlClienteVinculado.Items.Count == 0 || drpTipoNota.SelectedValue == "2")
            {
                tbCliVinculado.Visible = false;
            }
            else
            {
                tbCliVinculado.Visible = true;
                ddlClienteVinculado.Items.Insert(0, new ListItem()
                {
                    Selected = true,
                    Text = "Usar cliente original"
                });
            }
        }

        private void BuscaClientesVinculados(string idPedido)
        {
            string idCli = PedidoDAO.Instance.GetIdCliente(null, Glass.Conversoes.StrParaUint(idPedido)).ToString();
            odsClienteVinculado.SelectParameters[0].DefaultValue = idCli;
            ddlClienteVinculado.DataBind();
        }

        protected void drpLojaDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idCliente = Glass.Conversoes.StrParaUint(drpLojaDestino.SelectedValue);

            if (idCliente == 0)
            {
                percReducao.Style.Add("display", "none");
                return;
            }

            ConfiguraPercReducao(idCliente);
        }

        #region Métodos AJAX

        [Ajax.AjaxMethod]
        public string GerarNf(string idsPedidos, string idsLiberarPedidos, string idNaturezaOperacao, string idLoja,
            string percReducao, string percReducaoRevenda, string dadosNaturezasOperacao, string idCliente,
            string transferencia, string idCarregamento,
            string transferirNf, string nfce, string manterAgrupamentoDeProdutos)
        {
            return WebGlass.Business.NotaFiscal.Fluxo.Gerar.Ajax.GerarNf(idsPedidos, idsLiberarPedidos,
                idNaturezaOperacao, idLoja, percReducao, percReducaoRevenda, dadosNaturezasOperacao, idCliente,
                transferencia, idCarregamento, transferirNf, nfce, manterAgrupamentoDeProdutos);
        }

        [Ajax.AjaxMethod]
        public string GetAgruparProdutoNf()
        {
            return FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe.ToString();
        }

        [Ajax.AjaxMethod]
        public string GetPedidosByCliente(string idCliente, string nomeCliente)
        {
            return WebGlass.Business.Pedido.Fluxo.BuscarEValidar.Ajax.GetPedidosByCliente(idCliente, nomeCliente);
        }

        [Ajax.AjaxMethod]
        public string PedidoExiste(string idPedido)
        {
            return WebGlass.Business.Pedido.Fluxo.BuscarEValidar.Ajax.PedidoExiste(idPedido);
        }

        [Ajax.AjaxMethod]
        public string IsPedidoConfirmadoLiberado(string idPedido)
        {
            return WebGlass.Business.Pedido.Fluxo.BuscarEValidar.Ajax.IsPedidoConfirmadoLiberado(idPedido, "true");
        }

        [Ajax.AjaxMethod]
        public string LiberacaoExiste(string idLiberacao)
        {
            return WebGlass.Business.LiberarPedido.Fluxo.BuscarEValidar.Ajax.LiberacaoExiste(idLiberacao);
        }

        [Ajax.AjaxMethod]
        public string IsLiberacaoAberta(string idLiberacao)
        {
            return WebGlass.Business.LiberarPedido.Fluxo.BuscarEValidar.Ajax.IsLiberacaoAberta(idLiberacao);
        }

        [Ajax.AjaxMethod]
        public string IdsPedidosLiberacoes(string idsLiberacoes)
        {
            return WebGlass.Business.LiberarPedido.Fluxo.BuscarEValidar.Ajax.IdsPedidosLiberacoes(idsLiberacoes);
        }

        [Ajax.AjaxMethod]
        public string LiberacoesPedidos(string idsLiberacoes, string idsPedidos)
        {
            return WebGlass.Business.LiberarPedido.Fluxo.BuscarEValidar.Ajax.LiberacoesPedidos(idsLiberacoes, idsPedidos);
        }

        /// <summary>
        /// Verifica se a empresa calcula icms no pedido
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string CalculaIcmsPedido(string idsPedidos, string idsLiberarPedidos)
        {
            var idsLojas = string.Empty;
            
            if(!string.IsNullOrWhiteSpace(idsPedidos))
                idsLojas = PedidoDAO.Instance.ObtemIdsLojas(idsPedidos);
            if (!string.IsNullOrWhiteSpace(idsLiberarPedidos))
                idsLojas = LiberarPedidoDAO.Instance.ObtemIdsLojas(idsLiberarPedidos);

            var lojas = LojaDAO.Instance.GetByString(idsLojas);

            if (lojas.Any(f => !f.CalcularIcmsPedido))
                return "false";
            else
                return "true";
            //return PedidoConfig.Impostos.CalcularIcmsPedido.ToString().ToLower();
        }

        /// <summary>
        /// Verifica se os pedidos passados calculam ST
        /// </summary>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string PedidosPossuemSt(string idsPedido)
        {
            return WebGlass.Business.Pedido.Fluxo.BuscarEValidar.Ajax.PedidosPossuemSt(idsPedido);
        }

        [Ajax.AjaxMethod()]
        public string CalcSt(string idNaturezaOperacao)
        {
            return
                NaturezaOperacaoDAO.Instance.CalculaIcmsSt(null, Glass.Conversoes.StrParaUint(idNaturezaOperacao))
                    .ToString()
                    .ToLower();
        }

        [Ajax.AjaxMethod()]
        public string GetIdsPedidosByCarregamento(string idCarregamento)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.CarregamentoFluxo.Ajax.GetIdsPedidosByCarregamento(idCarregamento);
        }

        [Ajax.AjaxMethod()]
        public bool ExibirMensagem(string idNf)
        {
            return LogNfDAO.Instance.ExibirMensagemDiferençaValores(idNf.StrParaInt());
        }

        #endregion

        #region Consulta Situação do Cadastro Contribuinte

        /// <summary>
        /// Verifica se e possivel consultar a situação do cliente em questão
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string PodeConsultarCadastro(string idCliente)
        {
            uint idCli = Glass.Conversoes.StrParaUint(idCliente);

            if (idCli < 1)
                return "False";

            return ConsultaSituacao.HabilitadoConsultaCadastro(ClienteDAO.Instance.ObtemUf(idCli)).ToString();
        }

        protected void ctrlConsultaCadCliSintegra1_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlConsultaCadCliSintegra ConsCad =
                (Glass.UI.Web.Controls.ctrlConsultaCadCliSintegra) sender;

            ConsCad.CtrlCliente = txtIdCli;
        }

        #endregion

        protected void grdOcs_DataBound(object sender, EventArgs e)
        {
            if (!dataBoundPedido)
                return;

            dataBoundOC = true;

            foreach (GridViewRow item in grdOcs.Rows)
            {
                var idsPedidos = ((HiddenField) item.FindControl("idsPedidosOC")).Value.Split(',');

                for (int i = 0; i < grdPedidos.Rows.Count; i++)
                {
                    uint idPedido =
                        Glass.Conversoes.StrParaUint(
                            ((HiddenField) grdPedidos.Rows[i].Cells[0].FindControl("hdfIdPedido")).Value);
                    if (idsPedidos.Count(x => x.Trim() == idPedido.ToString()) == 0)
                        continue;

                    if (grdPedidos.Rows[i].Cells[0].ForeColor == Color.Red)
                    {
                        foreach (TableCell c in item.Cells)
                            c.ForeColor = Color.Red;

                        break;
                    }
                }
            }
        }

        private void ConfiguraPercReducao(uint idCliente)
        {
            float percReducaoCli = ClienteDAO.Instance.GetPercReducaoNFe(idCliente);
            rgvPercReducao.MaximumValue = percReducaoCli.ToString();
            rgvPercReducao.ErrorMessage = "Valor entre 0% e " + percReducaoCli + "%";
            rgvPercReducao.Enabled = percReducaoCli > 0;
            txtPercReducao.Text = percReducaoCli.ToString();

            float percReducaoCliRevenda = ClienteDAO.Instance.GetPercReducaoNFeRevenda(idCliente);
            rgvPercReducaoRev.MaximumValue = percReducaoCliRevenda.ToString();
            rgvPercReducaoRev.ErrorMessage = "Valor entre 0% e " + percReducaoCliRevenda + "%";
            rgvPercReducaoRev.Enabled = percReducaoCli > 0;
            txtPercReducaoRev.Text = percReducaoCliRevenda.ToString();

            percReducao.Style.Add("display", "none");
        }

        protected string NaoPermitirMaisDeUmaNfeParaUmPedido()
        {
            return FiscalConfig.NotaFiscalConfig.NaoPermitirMaisDeUmaNfeParaUmPedido.ToString().ToLower();
        }

        protected void drpTipoNota_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpTipoNota.SelectedValue == "1" && FiscalConfig.NotaFiscalConfig.ExportarNotaFiscalOutroBD)
                chkTransferirNf.Visible = true;
        }
        
        protected void ddlClienteVinculado_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idCliente = 0;

            if (!int.TryParse(((DropDownList)sender).SelectedValue, out idCliente))
                idCliente = hdfIdCliente.Value.StrParaIntNullable().GetValueOrDefault();

            ConfiguraPercReducao((uint)idCliente);
        }

        protected void chkAguparProdutos_Load(object sender, EventArgs e)
        {
            if (!FiscalConfig.ExibirCheckGerarProdutoConjunto)
                ((CheckBox)sender).Visible = false;
        }
    }
}