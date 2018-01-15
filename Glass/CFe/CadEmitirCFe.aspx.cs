using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Configuration;
using Glass.Configuracoes;

namespace Glass.UI.Web.CFe
{
    public partial class CadEmitirCFe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(CadEmitirCFe));
    
            this.MontarComboFormaPagamento();
            this.MontarComboCartao();
    
            //txtNumPedido.Enabled = hdfBuscarIdsLiberacoes.Value.Length == 0;
            imbAddPed.Enabled = txtNumPedido.Enabled;
    
            // Permite gerar nota apenas de liberação
            if (!FinanceiroConfig.FinanceiroRec.ExibirTodasNfeContasReceberLiberacao)
            {
                lblPedido.Style.Add("display", "none");
                txtNumPedido.Style.Add("display", "none");
                imbAddPed.Style.Add("display", "none");
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
    
            drpLoja.SelectedIndex = drpLoja.Items.IndexOf(drpLoja.Items.FindByValue(UserInfo.GetUserInfo.IdLoja.ToString()));
        }
    
        private void MontarComboFormaPagamento()
        {
            this.cbFormaPagamento1.DataSource = Enum.GetNames(typeof(Glass.Data.CFeUtils.PagamentoCFe.FormaPagamentoCFe));
            this.cbFormaPagamento2.DataSource = Enum.GetNames(typeof(Glass.Data.CFeUtils.PagamentoCFe.FormaPagamentoCFe));
            this.cbFormaPagamento3.DataSource = Enum.GetNames(typeof(Glass.Data.CFeUtils.PagamentoCFe.FormaPagamentoCFe));
            this.cbFormaPagamento1.DataBind();
            this.cbFormaPagamento2.DataBind();
            this.cbFormaPagamento3.DataBind();
        }
    
        private void MontarComboCartao()
        {
            this.cbOperadoraCartao1.DataSource = Enum.GetNames(typeof(Glass.Data.CFeUtils.PagamentoCFe.OperadorasCartaoCreditoCFe));
            this.cbOperadoraCartao2.DataSource = Enum.GetNames(typeof(Glass.Data.CFeUtils.PagamentoCFe.OperadorasCartaoCreditoCFe));
            this.cbOperadoraCartao3.DataSource = Enum.GetNames(typeof(Glass.Data.CFeUtils.PagamentoCFe.OperadorasCartaoCreditoCFe));
            this.cbOperadoraCartao1.DataBind();
            this.cbOperadoraCartao2.DataBind();
            this.cbOperadoraCartao3.DataBind();
        }
    
        [Ajax.AjaxMethod]
        public string SalvarCupom(int numeroSessao, string chaveSAT, int idCliente, int idLoja, string idsPedidos, string totalCupom, string totalPago)
        {
            try
            {
                uint idCupom = 0;
    
                // Insere o Cupom Fiscal
                CupomFiscal objCupom = new CupomFiscal();
    
                objCupom.Cancelado = "N";
                objCupom.ChaveCupomSat = chaveSAT;
                objCupom.DataCad = DateTime.Now;
                objCupom.DataEmissao = DateTime.Now;
                objCupom.IdCliente = Convert.ToUInt32(idCliente);
                objCupom.IdLoja = Convert.ToUInt32(idLoja);
                //objCupom.IdPedido = Convert.ToUInt32(strPedido);
                objCupom.NumeroSessao = Convert.ToUInt32(numeroSessao);
                objCupom.TotalCupom = float.Parse(totalCupom);
    
                foreach (string valorPago in totalPago.Split('|'))
                {
                    if (String.IsNullOrEmpty(valorPago))
                        continue;
    
                    objCupom.ValorPago += float.Parse(valorPago);
                }
    
                objCupom.Usucad = UserInfo.GetUserInfo.CodUser;
    
                idCupom = CupomFiscalDAO.Instance.Insert(objCupom);
    
                foreach (string strPedido in idsPedidos.Split(','))
                {
                    if (String.IsNullOrEmpty(strPedido))
                        continue;
    
                    PedidosCupomFiscal objPedCupom = new PedidosCupomFiscal();
                    objPedCupom.IdCupomFiscal = idCupom;
                    objPedCupom.IdPedido = Convert.ToUInt32(strPedido);
                    PedidosCupomFiscalDAO.Instance.Insert(objPedCupom);
                }
    
                return "Ok;" + idCupom.ToString();
            }
            catch (Exception e)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("", e);
            }
        }
    
        [Ajax.AjaxMethod]
        public string ObterNumeroSessao()
        {
            string numSessao = CupomFiscalDAO.Instance.GerarNumeroSessao().ToString();
            return numSessao;
        }
    
        [Ajax.AjaxMethod]
        public string ObterCodigoAtivacao()
        {
            return ConfigurationManager.AppSettings.Get("CodigoAtivacaoSAT");
        }
    
        [Ajax.AjaxMethod]
        public void SalvarArquivoCupom(string strCupom, string chaveCFe, bool cupomCancelado)
        {
            string path;
            FileStream reader;
                    
            try
            {
                if(cupomCancelado)
                    path = "C:\\sync\\webGlass\\" + chaveCFe + "Cancel.xml";
                else
                    path = "C:\\sync\\webGlass\\" + chaveCFe + ".xml";
                
                reader = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                reader.Write(Convert.FromBase64String(strCupom), 0, Convert.FromBase64String(strCupom).Length);
                reader.Close();
                reader.Dispose();
            }
            catch
            {
            }
        }
    
        [Ajax.AjaxMethod]
        public string ObterDadosEmitirCupom(string idsPedidos, bool prestacaoServico,
            bool entregarPedido, string idNaturezaOperacao, string idLoja, string valorPago, string formaPag, string opCartao, string totalCupom)
        {
            try
            {
                uint idCliente = 0;
    
                // Monta a lista de formas de pagamento
                List<Glass.Data.CFeUtils.PagamentoCFe> lstPag = new List<Glass.Data.CFeUtils.PagamentoCFe>();
                Glass.Data.CFeUtils.PagamentoCFe objFormaPag;
                string[] lstFormPag = formaPag.Split('|');
                string[] lstOpCartao = opCartao.Split('|');
                int countPag = 0;
    
                foreach (string strValor in valorPago.Split('|'))
                {
                    if (String.IsNullOrEmpty(strValor))
                    {
                        countPag++;
                        continue;
                    }
    
                    objFormaPag = new Glass.Data.CFeUtils.PagamentoCFe();
                    objFormaPag.FormaPagamento = (Glass.Data.CFeUtils.PagamentoCFe.FormaPagamentoCFe)Enum.Parse(typeof(Glass.Data.CFeUtils.PagamentoCFe.FormaPagamentoCFe), lstFormPag[countPag]);
                    objFormaPag.ValorPagamento = float.Parse(strValor);
                    objFormaPag.OperadoraCartao = (Glass.Data.CFeUtils.PagamentoCFe.OperadorasCartaoCreditoCFe)Enum.Parse(typeof(Glass.Data.CFeUtils.PagamentoCFe.OperadorasCartaoCreditoCFe), lstOpCartao[countPag]);
    
                    lstPag.Add(objFormaPag);
    
                    countPag++;
                }
    
                ProdutosNf[] lstProdutos = CupomFiscalDAO.Instance.obterProdutosPedido(idsPedidos, Glass.Conversoes.StrParaUint(idLoja), Glass.Conversoes.StrParaUint(idNaturezaOperacao), out idCliente);
    
                Cliente objCliente = ClienteDAO.Instance.GetElement(idCliente);
                Loja objLoja = LojaDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(idLoja));
    
                string dadosVenda = Glass.Data.CFeUtils.FormataXML.MontarXmlVenda(prestacaoServico, false, objCliente, entregarPedido, lstProdutos,
                    Glass.Conversoes.StrParaUint(idNaturezaOperacao), objLoja, lstPag.ToArray(), "");
                
                return "Ok;" + idCliente.ToString() + ";" + dadosVenda;
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }   
     
        [Ajax.AjaxMethod]
        public string ObterDadosCancelarCupom(string idLoja)
        {
            try
            {
                string chaveSat = "";
                uint seqCupom = 0;
                
                // É necessário avaliar com o André como o número do caixa será preenchido. Está fixo como "1"
                string strCupom = CupomFiscalDAO.Instance.ObterDadosCancelarVenda(this.ObterCodigoAtivacao(), "1", Glass.Conversoes.StrParaUint(idLoja), out chaveSat, out seqCupom);
    
                return "Ok;" + chaveSat + ";" + seqCupom.ToString() + ";" + strCupom;
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public string CancelarCupom(string idCupom)
        {
            try
            {
                CupomFiscal objCupom = CupomFiscalDAO.Instance.GetElementByPrimaryKey(Convert.ToUInt32(idCupom));
                objCupom.Cancelado = "S";
                CupomFiscalDAO.Instance.Update(objCupom);
                return "Ok;";
            }
            catch
            {
                return "Erro;";
            }
        }
    
        [Ajax.AjaxMethod]
        public string PedidoExiste(string idPedido)
        {
            try
            {
                return PedidoDAO.Instance.PedidoExists(Glass.Conversoes.StrParaUint(idPedido)).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }
    
        [Ajax.AjaxMethod]
        public string IsPedidoConfirmadoLiberado(string idPedido)
        {
            try
            {
                return PedidoDAO.Instance.IsPedidoConfirmadoLiberado(Glass.Conversoes.StrParaUint(idPedido)).ToString().ToLower();
            }
            catch
            {
                return "false";
            }
        }
    
        /// <summary>
        /// Verifica se a empresa calcula icms no pedido
        /// </summary>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string CalculaIcmsPedido()
        {
            return PedidoConfig.Impostos.CalcularIcmsPedido.ToString().ToLower();
        }
    
        /// <summary>
        /// Verifica se os pedidos passados calculam ST
        /// </summary>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod()]
        public string PedidosPossuemSt(string idsPedido)
        {
            return PedidoDAO.Instance.PedidosPossuemST(idsPedido).ToString().ToLower();
        }
    
        [Ajax.AjaxMethod()]
        public string NaturezaOperacaoCalcSt(string idNaturezaOperacao)
        {
            return NaturezaOperacaoDAO.Instance.CalculaIcmsSt(null, Glass.Conversoes.StrParaUint(idNaturezaOperacao)).ToString().ToLower();
        }
    
        protected void btnBuscarPedidos_Click(object sender, EventArgs e)
        {
            grdPedidos.DataBind();
        }
    
        protected void odsPedidos_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.ReturnValue is IEnumerable)
            {
                Glass.Data.Model.Pedido[] pedidos = (Glass.Data.Model.Pedido[])e.ReturnValue;
                lblMensagem.Text = String.Empty;
                btnGerarNf.Enabled = false;
                gerar.Visible = pedidos.Length > 0;
    
                if (pedidos.Length == 0)
                    lblMensagem.Text = "Selecione pelo menos um pedido para gerar a nota.";
                else
                {
                    List<uint> clientes = new List<uint>();
                    foreach (Glass.Data.Model.Pedido p in pedidos)
                        if (!clientes.Contains(p.IdCli))
                            clientes.Add(p.IdCli);
    
                    if (clientes.Count > 1)
                        lblMensagem.Text = "Há pedidos selecionados de mais de um cliente.";
                    else
                        btnGerarNf.Enabled = true;
                }
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            hdfBuscarIdsPedidos.Value = "";
            //hdfBuscarIdsLiberacoes.Value = "";
        }
    
        protected void grdPedidos_DataBound(object sender, EventArgs e)
        {
            for (int i = 0; i < grdPedidos.Rows.Count; i++)
            {
                uint idPedido = Glass.Conversoes.StrParaUint(((HiddenField)grdPedidos.Rows[i].Cells[0].FindControl("hdfIdPedido")).Value);
                string cuponsGerados = CupomFiscalDAO.Instance.CuponsFiscaisGerados(idPedido);
    
                HiddenField hdfNotasGeradas = (HiddenField)grdPedidos.Rows[i].Cells[0].FindControl("hdfNotasGeradas");
                hdfNotasGeradas.Value = cuponsGerados;
                Color corLinha = !String.IsNullOrEmpty(cuponsGerados) ? Color.Red : Color.Black;
    
                for (int j = 0; j < grdPedidos.Rows[i].Cells.Count; j++)
                    grdPedidos.Rows[i].Cells[j].ForeColor = corLinha;
            }
    
            float total = 0;
            for (int i = 0; i < grdPedidos.Rows.Count; i++)
            {
                Label totalPedido = (Label)grdPedidos.Rows[i].FindControl("lblTotalPedido");
                Label totalPedidoEsp = (Label)grdPedidos.Rows[i].FindControl("lblTotalPedidoEsp");
    
                string totalStr = totalPedido.Visible ? totalPedido.Text : totalPedidoEsp.Text;
    
                float totalLinha = Glass.Conversoes.StrParaFloat(totalStr.Replace("R$", "").Replace(" ", "").Replace(".", ""));
                total += totalLinha;
            }
    
            lblTotal.Text = total.ToString("C");
        }
    }
}
