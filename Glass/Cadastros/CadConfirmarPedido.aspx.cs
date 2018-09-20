using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadConfirmarPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadConfirmarPedido));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            hdfDataTela.Value = DateTime.Now.ToString();

            if (!IsPostBack && Request["idPedido"] != null)
            {
                txtNumPedido.Text = Request["idPedido"];
                uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);

                // Se o pedido não existir
                if (!PedidoDAO.Instance.PedidoExists(idPedido))
                    grdProdutos.Visible = false;
                else
                {
                    // Busca o pedido
                    Glass.Data.Model.Pedido pedido = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido);

                    // Se o pedido já tiver sido cancelado, esconde os produtos
                    grdProdutos.Visible = pedido.Situacao != Glass.Data.Model.Pedido.SituacaoPedido.Cancelado;

                    if (pedido.Situacao == Glass.Data.Model.Pedido.SituacaoPedido.Ativo)
                    {
                        lblViewConfirm.Text += "Pedido está ativo. Só é possível confirmar pedidos conferidos.";
                        imgImprimir.Visible = false;
                    }

                    // Se o pedido estiver confirmado, mostra quem confirmou e quando
                    if (pedido.Situacao == Glass.Data.Model.Pedido.SituacaoPedido.Confirmado)
                    {
                        lblViewConfirm.Text += "Pedido confirmado";

                        if (pedido.UsuConf != null)
                            lblViewConfirm.Text += " por " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome((uint)pedido.UsuConf.Value));

                        lblViewConfirm.Text += pedido.DataConf != null ? " no dia " + pedido.DataConf.Value.ToString("dd/MM/yy") + ". " : ". ";
                        imgImprimir.Visible = true;
                    }

                    // Vendido para funcionário
                    if (pedido.VendidoFuncionario)
                    {
                        divFunc.Visible = true;
                        divAVista.Visible = false;
                        chkVerificarParcelas.Visible = false;
                        chkVerificarParcelas.Checked = false;
                        btnConfirmarPrazo.Visible = false;
                        tbObra.Visible = false;

                        lblNomeFuncVenda.Text += "Funcionário comprador: " + PedidoDAO.Instance.ObtemNomeFuncVenda(idPedido);
                    }
                    // À Prazo
                    else if (pedido.TipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo)
                    {
                        // Se tiver sido recebido o sinal, mostra quem recebeu
                        if (pedido.RecebeuSinal)
                        {
                            CaixaDiario caixa = CaixaDiarioDAO.Instance.GetPedidoSinal(pedido.IdPedido);
                            lblViewSinal.Text = "O sinal deste pedido no valor de " + pedido.ValorEntrada.ToString("F2") + " foi recebido por " + caixa.DescrUsuCad + " em " + caixa.DataCad.ToString("dd/MM/yy") + ".";
                        }

                        divAVista.Visible = false;
                        chkVerificarParcelas.Checked = true;
                        btnConfirmarPrazo.Visible = true;
                        divFunc.Visible = false;
                        tbObra.Visible = false;
                    }
                    // À Vista
                    else if (pedido.TipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista)
                    {
                        decimal totalASerPago = pedido.Total;

                        #region Crédito cliente

                        decimal valorCredito = 0;
                        decimal credito = ClienteDAO.Instance.GetCredito(pedido.IdCli);

                        valorCredito = credito;
                        hdfValorCredito.Value = credito.ToString().Replace(',', '.');
                        hdfIdCliente.Value = pedido.IdCli.ToString();

                        #endregion

                        divAVista.Visible = true;
                        chkVerificarParcelas.Visible = false;
                        btnConfirmarPrazo.Visible = false;
                        divFunc.Visible = false;
                        tbObra.Visible = false;
                    }
                    else if (pedido.TipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Obra)
                    {
                        if (pedido.IdObra != null)
                        {
                            Obra obra = ObraDAO.Instance.GetElementByPrimaryKey(pedido.IdObra.Value);
                            lblDescrObra.Text = obra.Descricao;
                            lblSaldoObra.Text = obra.Saldo.ToString("C");
                            lblValorPedido.Text = pedido.Total.ToString("C");
                            hdfValorObra.Value = (pedido.Total - obra.Saldo).ToString();
                            pagtoObra.Visible = (pedido.Total - obra.Saldo) > 0;
                            hdfIdCliente.Value = obra.IdCliente.ToString();
                        }

                        divAVista.Visible = false;
                        chkVerificarParcelas.Visible = false;
                        btnConfirmarPrazo.Visible = false;
                        divFunc.Visible = false;
                        tbObra.Visible = true;
                    }

                    if (pedido.Situacao == Glass.Data.Model.Pedido.SituacaoPedido.Confirmado || pedido.Situacao == Glass.Data.Model.Pedido.SituacaoPedido.Ativo)
                    {
                        divAVista.Visible = false;
                        grdProdutos.Visible = true;
                        btnConfirmarPrazo.Visible = false;
                        chkVerificarParcelas.Visible = false;
                        divFunc.Visible = false;
                        tbObra.Visible = false;
                    }
                }
            }
            else if (!IsPostBack)
                grdProdutos.Visible = false;
        }

        #region Eventos Datasource

        protected void odsPedido_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
                divConfirmar.Visible = false;
            }
            else
                btnConfirmar.Visible = Request["IdPedido"] != null;
        }

        protected void odsProdXPed_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }

        #endregion

        [Ajax.AjaxMethod()]
        public string Confirmar(string idPedido, string fPagtos, string tpCartoes, string valores,
            string contasBanco, string depositoNaoIdentificado, string gerarCredito, string creditoUtilizado, string numAutConstrucard,
            string parcCreditos, string chequesPagto, string descontarComissao, string tipoVendaObraStr, string numAutCartao)
        {
            return WebGlass.Business.Pedido.Fluxo.ConfirmarPedido.Ajax.ConfirmarPedido(idPedido, fPagtos, tpCartoes,
                valores, contasBanco, depositoNaoIdentificado, gerarCredito, creditoUtilizado, numAutConstrucard, parcCreditos,
                chequesPagto, descontarComissao, tipoVendaObraStr, numAutCartao);
        }

        [Ajax.AjaxMethod()]
        public string ConfirmarPrazo(string idPedidoStr, string tipoVendaObraStr, string verificarParcelas)
        {
            return WebGlass.Business.Pedido.Fluxo.ConfirmarPedido.Ajax.ConfirmarPrazo(idPedidoStr,
                tipoVendaObraStr, verificarParcelas);
        }

        [Ajax.AjaxMethod]
        public string ConfirmarObra(string idPedidoStr, string fPagtos, string tpCartoes, string valores,
            string contasBanco, string depositoNaoIdentificado, string gerarCredito, string creditoUtilizado, string numAutConstrucard,
            string parcCreditos, string chequesPagto, string descontarComissao, string fPagto, string tipoCartao,
            string valoresParcelas, string datasParcelas, string tipoVendaObraStr, string numAutCartao)
        {
            return WebGlass.Business.Pedido.Fluxo.ConfirmarPedido.Ajax.ConfirmarObra(idPedidoStr, fPagtos,
                tpCartoes, valores, contasBanco, depositoNaoIdentificado, gerarCredito, creditoUtilizado, numAutConstrucard, parcCreditos,
                chequesPagto, descontarComissao, fPagto, tipoCartao, valoresParcelas, datasParcelas, tipoVendaObraStr, numAutCartao);
        }

        [Ajax.AjaxMethod]
        public string ConfirmarFunc(string idPedidoStr)
        {
            return WebGlass.Business.Pedido.Fluxo.ConfirmarPedido.Ajax.ConfirmarFunc(idPedidoStr);
        }

        [Ajax.AjaxMethod]
        public string IsPedidosAlterados(string idPedido, string dataTela)
        {
            var idsSinais = PedidoDAO.Instance.ObtemIdSinal(null, idPedido.StrParaUint());
            var idsPagtoAntecip = PedidoDAO.Instance.ObtemIdPagamentoAntecipado(null, idPedido.StrParaUint());
            return WebGlass.Business.Pedido.Fluxo.BuscarEValidar.Ajax.IsPedidosAlterados(idPedido, idsSinais.GetValueOrDefault(0).ToString(), idsPagtoAntecip.GetValueOrDefault(0).ToString(), dataTela);
        }

        protected void drpParcCredito_Load(object sender, EventArgs e)
        {
            ((DropDownList)sender).Visible = FinanceiroConfig.Cartao.PedidoJurosCartao;
        }

        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            ctrlFormaPagto1.CampoCredito = hdfValorCredito;
            ctrlFormaPagto1.CampoValorConta = (Label)dtvPedido.FindControl("lblTotal");
            ctrlFormaPagto1.ParentID = divAVista.ClientID;
            ctrlFormaPagto1.CampoClienteID = hdfIdCliente;

            if (ctrlFormaPagto1.DataRecebimento == null)
                ctrlFormaPagto1.DataRecebimento = DateTime.Now;
        }

        protected void ctrlFormaPagto2_Load(object sender, EventArgs e)
        {
            ctrlFormaPagto2.CampoCredito = hdfValorCredito;
            ctrlFormaPagto2.CampoValorConta = hdfValorObra;
            ctrlFormaPagto1.CampoClienteID = hdfIdCliente;
        }

        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            ctrlParcelas1.CampoValorTotal = hdfValorObra;
            ctrlParcelas1.CampoParcelasVisiveis = drpNumParcelas;
            ctrlParcelas1.CampoCalcularParcelas = hdfCalcularParcelas;
        }

        protected void btnBuscarPedido_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadConfirmarPedido.aspx?IdPedido=" + txtNumPedido.Text);
        }

        protected int GetCartaoCod()
        {
            return (int)Glass.Data.Model.Pagto.FormaPagto.Cartao;
        }

        protected void drpTipoVendaObra_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
