using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Utils
{
    public partial class DescontoPedido : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["idPedido"] == null || !PedidoDAO.Instance.GetElement(Request["idPedido"].StrParaUint()).DescontoVisible)
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", "closeWindow();\n", true);
                principal.Visible = false;
                return;
            }

            if (PedidoDAO.Instance.IsPedidoReposicao(Request["idPedido"]) &&
                !Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoReposicao))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", "alert('Você não tem permissão para editar pedidos de Reposição');closeWindow();\n", true);
                principal.Visible = false;
                return;
            }

            if (PedidoDAO.Instance.IsPedidoGarantia(Request["idPedido"]) &&
                !Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantia))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "fechar", "alert('Você não tem permissão para editar pedidos de Garantia');closeWindow();\n", true);
                principal.Visible = false;
                return;
            }

            if (!IsPostBack && !PedidoConfig.LiberarPedido)
            {
                dtvPedido.Visible = false;
                dtvPedidoConf.Visible = true;
                Page.Title = "Alteração dos produtos do pedido";
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(DescontoPedido));
    
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), "bloquear", "bloquearPagina(); desbloquearPagina(false);");
    
            if (!IsPostBack)
                divAmbiente.Visible = PedidoConfig.DadosPedido.AmbientePedido || PedidoDAO.Instance.IsMaoDeObra(null, Request["idPedido"].StrParaUint());
    
            Page.PreRender += delegate
            {
                var script = ScriptEsconderDadosPedido();
                if (!string.IsNullOrEmpty(script))
                    Page.ClientScript.RegisterStartupScript(GetType(), "esconderDados", script, true);
            };
        }
    
        protected void drpTipoVenda_DataBound(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var tipoVenda = PedidoDAO.Instance.ObtemTipoVenda(null, Request["idPedido"].StrParaUint());
                if (tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Obra)
                    ((DropDownList)dtvPedido.FindControl("drpTipoVenda")).Enabled = false;
                else
                    ((DropDownList)dtvPedido.FindControl("drpTipoVenda")).Items[5].Enabled = true;
            }
        }
    
        protected void odsPedido_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                AtualizarOpener();
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "atualizado", @"
                    alert('Pedido atualizado!');
                    closeWindow();", true);
            }
            else
            {
                MensagemAlerta.ErrorMsg("Falha ao atualizar dados do pedido.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
    
        protected void DataEntrega_Load(object sender, EventArgs e)
        {
            // Se as etiquetas do pedido já estiverem impressas, não permite alterar a data de entrega
            //((WebControl)sender).Visible = !PedidoEspelhoDAO.Instance.IsPedidoImpresso(Glass.Conversoes.StrParaUint(Request["IdPedido"]));

            var pedidoTemOC = PedidoOrdemCargaDAO.Instance.PedidoTemOC(null, Request["IdPedido"].StrParaUint());

            //Se o pedido ja tiver OC gerada não permite alterar a data de entrega
            if (sender is Controls.ctrlData)
                ((Controls.ctrlData)sender).Enabled = !pedidoTemOC;
            else
                ((WebControl)sender).Enabled = !pedidoTemOC;

            if (!pedidoTemOC)
            {
                var idPedido = !string.IsNullOrEmpty(Request["idPedido"])
                    ? (uint?) Request["idPedido"].StrParaUint()
                    : null;
                var idCli = idPedido > 0 ? PedidoDAO.Instance.GetIdCliente(null, idPedido.Value) : 0;
                var dataBase = idPedido > 0
                    ? PedidoDAO.Instance.ObtemDataPedido(null, idPedido.Value)
                    : FuncionarioDAO.Instance.ObtemDataAtraso(UserInfo.GetUserInfo.CodUser);

                DateTime dataMinima, dataFastDelivery;
                bool desabilitarCampo;

                if (!IsPostBack &&
                    PedidoDAO.Instance.GetDataEntregaMinima(null, idCli, idPedido, null, null, dataBase, out dataMinima, out dataFastDelivery, out desabilitarCampo))
                {
                    if (dataFastDelivery.Date <= DateTime.Now.Date)
                        dataFastDelivery = DateTime.Now;
                    if (dataMinima.Date <= DateTime.Now.Date)
                        dataMinima = DateTime.Now;

                    ((HiddenField) dtvPedido.FindControl("hdfDataEntregaFD"))
                        .Value = dataFastDelivery.ToString("dd/MM/yyyy");
                    ((HiddenField) dtvPedido.FindControl("hdfDataEntregaNormal"))
                        .Value = dataMinima.ToString("dd/MM/yyyy");
                }
            }
        }

        protected bool GetBloquearDataEntrega()
        {
            return PedidoDAO.Instance.BloquearDataEntregaMinima(null, Request["idPedido"].StrParaUintNullable());
        }

        protected void FastDelivery_Load(object sender, EventArgs e)
        {
            sender.GetType().GetProperty("Visible").SetValue(sender, PedidoConfig.Pedido_FastDelivery.FastDelivery, null);

            if (sender is CheckBox && ((CheckBox)sender).ID == "chkFastDelivery")
            {
                bool exibir = PedidoConfig.Pedido_FastDelivery.FastDelivery && Config.PossuiPermissao(Config.FuncaoMenuPedido.PermitirMarcarFastDelivery);
                ((CheckBox)sender).Style.Value = exibir ? "" : "display: none";
            }
        }

        protected void grdProdutos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Remover")
            {
                try
                {
                    var idProdPed = e.CommandArgument.ToString().StrParaUint();
                    var qtdeRemover = ((TextBox)((ImageButton)e.CommandSource).Parent.Parent.FindControl("txtQtde")).Text.StrParaFloat();
                    ProdutosPedidoDAO.Instance.RemoverProdutoDescontoAdmin(ref idProdPed, qtdeRemover);
    
                    if (PedidoConfig.LiberarPedido)
                    {
                        var idPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(idProdPed);
                        if (PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido))
                        {
                            var idProdPedEsp = ProdutosPedidoDAO.Instance.ObterIdProdPedEsp(idProdPed);
                            if (idProdPedEsp.GetValueOrDefault(0) == 0)
                                MensagemAlerta.ShowMsg("Não foi possível remover o produto do PCP. Produto não encontrado.", Page);
                        }
    
                        dtvPedido.DataBind();
                    }
    
                    grdProdutos.DataBind();
                    grdProdutosRem.DataBind();
                    AtualizarOpener();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao remover produto do pedido.", ex, Page);
                }
            }
        }
    
        protected void grdProdutosRem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Restaurar")
            {
                try
                {
                    var idProdPed = e.CommandArgument.ToString().StrParaUint();
                    var qtdeRestaurar = ((TextBox)((ImageButton)e.CommandSource).Parent.Parent.FindControl("txtQtde")).Text.StrParaFloat();
                    ProdutosPedidoDAO.Instance.RestaurarProdutoDescontoAdmin(ref idProdPed, qtdeRestaurar);
    
                    if (PedidoConfig.LiberarPedido)
                    {
                        uint idPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(idProdPed);
                        if (PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido))
                        {
                            uint? idProdPedEsp = ProdutosPedidoDAO.Instance.ObterIdProdPedEsp(idProdPed);
                            if (idProdPedEsp.GetValueOrDefault(0) == 0)
                                Glass.MensagemAlerta.ShowMsg("Não foi possível restaurar o produto no PCP. Produto não encontrado.", Page);
                        }
    
                        dtvPedido.DataBind();
                    }
    
                    grdProdutos.DataBind();
                    grdProdutosRem.DataBind();
                    AtualizarOpener();
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao restaurar produto ao pedido.", ex, Page);
                }
            }
        }
    
        private void AtualizarOpener()
        {
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "atualizar_opener", @"
                window.opener.FindControl('txtEndereco', 'input').value = window.opener.FindControl('txtEndereco', 'input').value == ' ' ? '' : ' ';
                window.opener.atualizarPagina();", true);
        }
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string LoadAjax(string tipo, string idClienteStr, string tipoVendaStr)
        {
            var idCliente = idClienteStr.StrParaUint();
            var tipoVenda = tipoVendaStr.StrParaInt();
            var formato = "<option value='{0}'{2}>{1}</option>";
            var retorno = string.Empty;
    
            switch (tipo)
            {
                case "tipoVenda":
                    var tipoPagto = ClienteDAO.Instance.ObtemTipoPagto(idCliente);
                    var parcela = tipoPagto.HasValue ? ParcelasDAO.Instance.GetElementByPrimaryKey(tipoPagto.Value) : null;
                    var isTipoPagtoAVista = tipoPagto > 0 && parcela != null ? parcela.NumParcelas == 0 : true;
    
                    foreach (GenericModel g in DataSources.Instance.GetTipoVenda())
                    {
                        bool adicionar = g.Id != (uint)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo && g.Id != (uint)Glass.Data.Model.Pedido.TipoVendaPedido.AVista;
                        adicionar = adicionar || g.Id == (uint)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo && ParcelasDAO.Instance.GetCountByCliente(idCliente, ParcelasDAO.TipoConsulta.Prazo) > 0;
                        adicionar = adicionar || g.Id == (uint)Glass.Data.Model.Pedido.TipoVendaPedido.AVista && ParcelasDAO.Instance.GetCountByCliente(idCliente, ParcelasDAO.TipoConsulta.Vista) > 0;
    
                        if (adicionar)
                        {
                            retorno += string.Format(formato, g.Id, g.Descr, tipoPagto > 0 && (isTipoPagtoAVista && g.Id == (uint)Glass.Data.Model.Pedido.TipoVendaPedido.AVista ||
                                !isTipoPagtoAVista && g.Id == (uint)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo) ? " selected='selected'" : string.Empty);
                        }
                    }
                    break;
    
                case "formaPagto":
                    var idFormaPagto = ClienteDAO.Instance.ObtemIdFormaPagto(idCliente);
                    retorno = string.Format(formato, string.Empty, string.Empty, string.Empty);

                    foreach (var f in FormaPagtoDAO.Instance.GetForPedido(null, (int)idCliente, tipoVenda))
                    {
                        retorno += string.Format(formato, f.IdFormaPagto, f.Descricao, idFormaPagto > 0 && f.IdFormaPagto == idFormaPagto ? " selected='selected'" : string.Empty);
                    }

                    break;
            }
    
            return retorno;
        }

        [Ajax.AjaxMethod()]
        public string VerificaDescontoParcela(string idParcela, string idPedido)
        {
            if (!FinanceiroConfig.UsarDescontoEmParcela)
                return "";

            var parcela = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Financeiro.Negocios.IParcelasFluxo>()
                .ObtemParcela(idParcela.StrParaInt());

            if (parcela == null || parcela.Desconto == 0)
                return "";

            return parcela.Desconto.ToString(CultureInfo.InvariantCulture);
        }

        [Ajax.AjaxMethod]
        public string VerificarDescontoPermitido(string idPedido, string tipoVenda, string tipoEntrega, string idParcela)
        {
            var isAdministrador = UserInfo.GetUserInfo.IsAdministrador;
            var idFuncDesc = Geral.ManterDescontoAdministrador ? PedidoDAO.Instance.ObtemIdFuncDesc(null, idPedido.StrParaUint()).GetValueOrDefault() : 0;

            var pedido = new Glass.Data.Model.Pedido();
            pedido = PedidoDAO.Instance.GetElementByPrimaryKey(idPedido.StrParaUint());
            pedido.TipoVenda = tipoVenda.StrParaInt();
            pedido.TipoEntrega = idPedido.StrParaInt();
            pedido.IdParcela = idPedido.StrParaUint();

            return isAdministrador && !PedidoDAO.Instance.DescontoPermitido(null, pedido) ?
                $"1|O funcionário {FuncionarioDAO.Instance.GetNome(idFuncDesc)} aplicou um desconto maior do que o desconto máximo configurado para esta alteração." +
                " Tem certeza que deseja alterar o pedido?" : "0|Deseja atualizar os dados do pedido?";
        }

        [Ajax.AjaxMethod]
        public string PercDesconto(string idPedidoStr, string idFuncAtualStr, string alterouDesconto, string idParcela)
        {
            var idPedido = idPedidoStr.StrParaUint();
            var idFuncAtual = idFuncAtualStr.StrParaUint();
            var idFuncDesc = Geral.ManterDescontoAdministrador ? PedidoDAO.Instance.ObtemIdFuncDesc(null, idPedido).GetValueOrDefault() : 0;

            return (idFuncDesc == 0 || UserInfo.IsAdministrador(idFuncAtual) || alterouDesconto.ToLower() == "true" ?
                PedidoConfig.Desconto.GetDescontoMaximoPedido(idFuncAtual, (int)PedidoDAO.Instance.ObtemTipoVenda(null, idPedido), idParcela.StrParaInt()) :
                PedidoConfig.Desconto.GetDescontoMaximoPedido(idFuncDesc, (int)PedidoDAO.Instance.ObtemTipoVenda(null, idPedido), idParcela.StrParaInt())).ToString().Replace(",", ".");
        }

        [Ajax.AjaxMethod()]
        public string VerificaDescontoFormaPagtoDadosProduto(string idPedido, string tipoVenda, string idFormaPagto, string idTipoCartao, string idParcela)
        {
            if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
            {
                uint? _idGrupoProd = null;
                uint? _idSubgrupoProd = null;
                var _idPedido = Conversoes.StrParaUint(idPedido);
                var _tipoVenda = Conversoes.StrParaUint(tipoVenda);
                var _idFormaPagto = Conversoes.StrParaUintNullable(idFormaPagto);
                var _idTipoCartao = Conversoes.StrParaUintNullable(idTipoCartao);
                var _idParcela = Conversoes.StrParaUintNullable(idParcela);

                bool isPcp = PedidoEspelhoDAO.Instance.ExisteEspelho(_idPedido);
                if (isPcp)
                {
                    var produtosPedido = ProdutosPedidoEspelhoDAO.Instance.GetByPedido(_idPedido, false);
                    if (produtosPedido != null && produtosPedido.Count > 0)
                    {
                        _idGrupoProd = produtosPedido[0].IdGrupoProd;
                        _idSubgrupoProd = produtosPedido[0].IdSubgrupoProd;
                    }
                }
                else
                {
                    var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(_idPedido);
                    if (produtosPedido != null && produtosPedido.Count > 0)
                    {
                        _idGrupoProd = produtosPedido[0].IdGrupoProd;
                        _idSubgrupoProd = produtosPedido[0].IdSubgrupoProd;
                    }
                }

                var desconto = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObterDesconto(_tipoVenda, _idFormaPagto, _idTipoCartao, _idParcela, _idGrupoProd, _idSubgrupoProd);

                return desconto.ToString();
            }
            else
                return string.Empty;
        }

        #endregion

        #region Parcelas

        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            Controls.ctrlParcelas ctrlParcelas = (Controls.ctrlParcelas)sender;
            Controls.ctrlParcelasSelecionar parcSel = (Controls.ctrlParcelasSelecionar)dtvPedido.FindControl("ctrlParcelasSelecionar1");
    
            ctrlParcelas.AlterarData = !parcSel.ExibirDias();
        }
    
        protected void ctrlParcelas1_DataBinding(object sender, EventArgs e)
        {
            var ped = dtvPedido.DataItem as Glass.Data.Model.Pedido;

            var usarTotalEspelho = ((Label)dtvPedido.FindControl("lblTotalEspelho")).Text.Replace("R$", "").Replace(" ", "").StrParaDecimal() > 0;

            var ctrlParcelas = (Controls.ctrlParcelas)sender;
            var hdfCalcularParcela = (HiddenField)dtvPedido.FindControl("hdfCalcularParcela");
            var hdfExibirParcela = (HiddenField)dtvPedido.FindControl("hdfExibirParcela");
            var ctrValEntrada = (Controls.ctrlTextBoxFloat)dtvPedido.FindControl("ctrValEntrada");
            var txtEntrada = (TextBox)ctrValEntrada.FindControl("txtNumber");
            var hdfEntrada = (HiddenField)dtvPedido.FindControl("hdfValorEntrada");
            var lblTotal = (Label)dtvPedido.FindControl(!usarTotalEspelho ? "lblTotal" : "lblTotalEspelho");
            var txtDesconto = (TextBox)dtvPedido.FindControl("txtDesconto");
            var drpTipoDesconto = (DropDownList)dtvPedido.FindControl("drpTipoDesconto");
            var hdfDesconto = (HiddenField)dtvPedido.FindControl("hdfDesconto");
            var hdfTipoDesconto = (HiddenField)dtvPedido.FindControl("hdfTipoDesconto");
            var txtAcrescimo = (TextBox)dtvPedido.FindControl("txtAcrescimo");
            var drpTipoAcrescimo = (DropDownList)dtvPedido.FindControl("drpTipoAcrescimo");
            var hdfAcrescimo = (HiddenField)dtvPedido.FindControl("hdfAcrescimo");
            var hdfTipoAcrescimo = (HiddenField)dtvPedido.FindControl("hdfTipoAcrescimo");
            var hdfDataBase = (HiddenField)dtvPedido.FindControl("hdfDataBase");
    
            ctrlParcelas.CampoCalcularParcelas = hdfCalcularParcela;
            ctrlParcelas.CampoExibirParcelas = hdfExibirParcela;
            ctrlParcelas.CampoValorEntrada = ped != null && ped.RecebeuSinal ? (Control)hdfEntrada : (Control)txtEntrada;
            ctrlParcelas.CampoValorTotal = lblTotal;
            ctrlParcelas.CampoValorDescontoAtual = txtDesconto;
            ctrlParcelas.CampoTipoDescontoAtual = drpTipoDesconto;
            ctrlParcelas.CampoValorDescontoAnterior = hdfDesconto;
            ctrlParcelas.CampoTipoDescontoAnterior = hdfTipoDesconto;
            ctrlParcelas.CampoValorAcrescimoAtual = txtAcrescimo;
            ctrlParcelas.CampoTipoAcrescimoAtual = drpTipoAcrescimo;
            ctrlParcelas.CampoValorAcrescimoAnterior = hdfAcrescimo;
            ctrlParcelas.CampoTipoAcrescimoAnterior = hdfTipoAcrescimo;
            ctrlParcelas.CampoDataBase = hdfDataBase;
        }
    
        protected void ctrlParcelasSelecionar1_Load(object sender, EventArgs e)
        {
            Controls.ctrlParcelasSelecionar parcSel = (Controls.ctrlParcelasSelecionar)sender;
            parcSel.ControleParcelas = dtvPedido.FindControl("ctrlParcelas1") as Controls.ctrlParcelas;
            parcSel.CampoClienteID = dtvPedido.FindControl("hdfIdCliente");
        }
    
        protected void hdfDataBase_Load(object sender, EventArgs e)
        {
            ((HiddenField)sender).Value = DateTime.Now.ToString("dd/MM/yyyy");
        }
    
        #endregion
    
        protected string GetDescontoProdutos()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request["idPedido"]))
                    return PedidoDAO.Instance.GetDescontoProdutos(null, Request["idPedido"].StrParaUint()).ToString().Replace(",", ".");
                
                return "0";
            }
            catch
            {
                return "0";
            }
        }
    
        protected string GetDescontoPedido()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Request["idPedido"]))
                {
                    decimal descontoProdutos, descontoPedido;
                    var idPedido = Request["idPedido"].StrParaUint();

                    descontoProdutos = PedidoDAO.Instance.GetDescontoProdutos(null, idPedido);
                    descontoPedido = PedidoDAO.Instance.GetDescontoPedido(null, idPedido, descontoProdutos);

                    return (descontoProdutos + descontoPedido).ToString().Replace(",", ".");
                }
                
                return "0";
            }
            catch
            {
                return "0";
            }
        }
    
        protected void ctrlDadosDesconto_Load(object sender, EventArgs e)
        {
            Controls.ctrlDadosDesconto d = (Controls.ctrlDadosDesconto)sender;
            d.CampoDesconto = dtvPedido.FindControl("txtDesconto");
            
            d.CampoFastDelivery = dtvPedido.FindControl("chkFastDelivery");
            if (d.CampoFastDelivery != null)
                d.CampoFastDelivery = dtvPedido.FindControl("hdfFastDelivery");
    
            d.CampoTipoDesconto = dtvPedido.FindControl("drpTipoDesconto");
            d.CampoTotalSemDesconto = dtvPedido.FindControl("hdfTotalSemDesconto");
        }
    
        /// <summary>
        /// Gera o script para esconder os campos desnecessários para a tela,
        /// de acordo com o pedido que está sendo editado.
        /// </summary>
        /// <returns></returns>
        protected string ScriptEsconderDadosPedido()
        {
            var script = string.Empty;
    
            // Bsuca o desconto máximo do pedido de acordo com o vendedor
            var desconto = ProdutosPedidoDAO.Instance.
                GetDescontoMaximoPedidoVendedor(Request["idPedido"].StrParaUint());
    
            // Se a empresa for confirmação, será usado o DetailsView dtvPedidoConf
            if (!PedidoConfig.LiberarPedido)
            {
                // Exibe os dados do desconto caso a empresa trabalhe com o desconto
                // do pedido quando houver apenas 1 produto
                if (PedidoConfig.DescontoPedidoVendedorUmProduto)
                {
                    // Exibe os dados do desconto para 1 produto
                    var descProd = dtvPedidoConf.FindControl("lblDescProd") as Label;
                    if (descProd != null)
                        descProd.Text = desconto.DescontoProd.ToString("0.##") + "%";
                    var descCli = dtvPedidoConf.FindControl("lblDescCli") as Label;
                    if (descCli != null)
                        descCli.Text = desconto.DescontoCliente.ToString("0.##") + "%";
                    var descMaximo = dtvPedidoConf.FindControl("lblDescMaximo") as Label;
                    if (descMaximo != null)
                        descMaximo.Text = desconto.DescontoMax.ToString("0.##") + "%";

                    hdfUsarDescontoMax.Value = "true";
                }
                else
                {
                    // Define as linhas que continuarão a ser exibidas
                    var linhasExibir = new List<int>(new [] { 4, 5 });
    
                    // Esconde todas as outras linhas
                    script = "document.getElementById('dadosPedido').style.display='none';\n";
                    for (int i = 0; i < 6; i++)
                        if (!linhasExibir.Contains(i))
                            script += "document.getElementById('alterarPedido').rows[" + i + "].style.display='none';\n";
                }
            }
    
            // Caso a empresa seja de liberação, mas o funcionário não seja administrador ou não tenha
            // permissão para alteração dos dados do pedido (ou seja, só tem permissão para alteração
            // do desconto se a empresa trabalhar com o desconto para 1 produto apenas)
            else if (!UserInfo.GetUserInfo.IsAdministrador && !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoConfirmadoListaPedidos))
            {
                // Esconde o controle de remoção/restauração dos produtos do pedido
                produtosPedido.Visible = false;
    
                // Permite que apenas a data de entrega seja modificada
                if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor) &&
                    Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarDataEntregaPedidoListaPedidos))
                {
                    // Define as linhas que serão exibidas (Data de entrega)
                    var linhasExibir = new List<int>(new [] { 11 });
    
                    // Esconde todas as outras linhas
                    script = "document.getElementById('dadosPedido').style.display='none';\n";
                    for (int i = 0; i < 16; i++)
                        if (!linhasExibir.Contains(i))
                            script += "document.getElementById('alterarPedido').rows[" + i + "].style.display='none';\n";
                }
                // Este if está de acordo com o código de atualização de valores ns PedidoDAO, método UpdateDesconto(),
                // caso este if seja alterado será necessário alterar o código neste método para definir quais campos serão atualizados
                else if (UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.Vendedor ||
                    Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor))
                {
                    // Define as linhas que serão exibidas
                    var linhasExibir = new List<int>(new [] { 2, 3, 4, 10, 11, 12, 14 });

                    // Se a config "Alterar Dados Básicos do Pedido na Lista de Pedidos Após Confirmado" estiver habilitada, exibe a Obs da Liberação.
                    if (Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor))
                        linhasExibir.Add(16);
    
                    // Também exibe as linhas de desconto para 1 produto se o vendedor tiver permissão
                    // para alteração do desconto e se o pedido permitir
                    if (PedidoConfig.DescontoPedidoVendedorUmProduto && UserInfo.GetUserInfo.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Vendedor &&
                        Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor) &&
                        ProdutosPedidoDAO.Instance.PodeAplicarDescontoVendedor(Request["idPedido"].StrParaUint()))
                    {
                        linhasExibir.AddRange(new [] { 5, 6, 7, 8 });
                        hdfUsarDescontoMax.Value = "true";
                    }
    
                    // Esconde todas as outras linhas
                    script = "document.getElementById('dadosPedido').style.display='none';\n";
                    for (int i = 0; i < 17; i++)
                        if (!linhasExibir.Contains(i))
                            script += "document.getElementById('alterarPedido').rows[" + i + "].style.display='none';\n";
    
                    // Exibe os dados do desconto para 1 produto
                    var descProd = dtvPedido.FindControl("lblDescProd") as Label;
                    if (descProd != null)
                        descProd.Text = desconto.DescontoProd.ToString("0.##") + "%";
                    var descCli = dtvPedido.FindControl("lblDescCli") as Label;
                    if (descCli != null)
                        descCli.Text = desconto.DescontoCliente.ToString("0.##") + "%";
                    var descMaximo = dtvPedido.FindControl("lblDescMaximo") as Label;
                    if (descMaximo != null)
                        descMaximo.Text = desconto.DescontoMax.ToString("0.##") + "%";
                }
                else
                {
                    // Exibe apenas o DetailsView de desconto do vendedor
                    dtvPedido.Visible = false;
                    dtvPedidoConf.Visible = false;
                    dtvPedidoDescVendedor.Visible = true;
    
                    // Exibe os dados do desconto para 1 produto
                    var descProd = dtvPedidoDescVendedor.FindControl("lblDescProd") as Label;
                    if (descProd != null)
                        descProd.Text = desconto.DescontoProd.ToString("0.##") + "%";
                    var descCli = dtvPedidoDescVendedor.FindControl("lblDescCli") as Label;
                    if (descCli != null)
                        descCli.Text = desconto.DescontoCliente.ToString("0.##") + "%";
                    var descMaximo = dtvPedidoDescVendedor.FindControl("lblDescMaximo") as Label;
                    if (descMaximo != null)
                        descMaximo.Text = desconto.DescontoMax.ToString("0.##") + "%";

                    hdfUsarDescontoMax.Value = "true";
                }
            }
            else if (Config.PossuiPermissao(Config.FuncaoMenuPedido.PermitirExcluirPecaPedidoConfirmado))
            {                
                // Esconde as linhas de desconto para 1 produto
                for (int i = 5; i < 8; i++)
                    script += "document.getElementById('alterarPedido').rows[" + i + "].style.display='none';\n";
            }
            else
            {
                // Esconde o controle de remoção/restauração dos produtos do pedido
                produtosPedido.Visible = false;

                // Esconde as linhas de desconto para 1 produto
                for (int i = 5; i < 8; i++)
                    script += "document.getElementById('alterarPedido').rows[" + i + "].style.display='none';\n";
            }
            
            // Retorna o script montado
            return script;
        }
    
        protected void drpFunc_DataBinding(object sender, EventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaUint();
            var idFunc = PedidoDAO.Instance.ObtemIdFunc(null, idPedido);
    
            // Se o funcionário deste pedido estiver inativo, inclui o mesmo na listagem para não ocorrer erro
            if (FuncionarioDAO.Instance.GetVendedores().All(f => f.IdFunc != idFunc))
            {
                string nomeFunc = FuncionarioDAO.Instance.GetNome(idFunc);
                ((DropDownList)sender).Items.Add(new ListItem(nomeFunc, idFunc.ToString()));
            }
        }
    
        protected void lblObsLiberacao_Load(object sender, EventArgs e)
        {
            // A observação de liberação só ficará visível se a empresa trabalhar com liberação de pedidos
            ((WebControl)sender).Visible = PedidoConfig.LiberarPedido;
        }
    
        protected void txtObsLiberacao_Load(object sender, EventArgs e)
        {
            // A observação de liberação só ficará visível se a empresa trabalhar com liberação de pedidos
            ((WebControl)sender).Visible = PedidoConfig.LiberarPedido;
        }
    
        protected void drpTipoEntrega_PreRender(object sender, EventArgs e)
        {
            var idPedido = Request["idPedido"].StrParaUint();
    
            if (OrdemCargaConfig.UsarControleOrdemCarga)
            {
                // Bloqueia o tipo de entrega se houver OC ou volume gerados para o pedido
                if (((DropDownList)sender).SelectedValue == (DataSources.Instance.GetTipoEntregaEntrega()).ToString())
                    ((WebControl)sender).Enabled = !PedidoDAO.Instance.TemVolume(null, idPedido) && !PedidoOrdemCargaDAO.Instance.PedidoTemOC(null, idPedido);
            }

            var isPedidoProducao = PedidoDAO.Instance.IsProducao(null, idPedido);

            var situacaoPedidoRevenda = PedidoDAO.Instance.ObtemSituacao(null, (uint)(PedidoDAO.Instance.ObterIdPedidoRevenda(null, (int)idPedido)).Value);

            //se o pedido for de produção e o pedido de revenda estiver liberado, bloqueia o drop de alterar tipo entrega
            if (isPedidoProducao && (situacaoPedidoRevenda == Data.Model.Pedido.SituacaoPedido.Confirmado || situacaoPedidoRevenda == Data.Model.Pedido.SituacaoPedido.LiberadoParcialmente))
            {
                ((WebControl)sender).Enabled = false;
            }
        }
    
        protected void chkDeveTransferir_Load(object sender, EventArgs e)
        {
            if (!OrdemCargaConfig.UsarControleOrdemCarga || !PedidoConfig.ExibirOpcaoDeveTransferir || PedidoOrdemCargaDAO.Instance.PedidoTemOC(null, Request["idPedido"].StrParaUint()))
                ((WebControl)sender).Visible = false;
        }

        protected void drpLoja_Load(object sender, EventArgs e)
        {
            //Se o pedido ja tiver OC gerada não permite alterar a data de entrega
            if (sender is Controls.ctrlLoja)
                ((Controls.ctrlLoja)sender).Enabled = !PedidoOrdemCargaDAO.Instance.PedidoTemOC(null, Request["IdPedido"].StrParaUint());
        }

        protected bool PedCliApenasLeitura()
        {
            string idPedido = Request["idPedido"];
            if (idPedido != null)
            {
                var p = PedidoDAO.Instance.GetElementByPrimaryKey(Conversoes.StrParaUint(idPedido));

                return p.Importado || p.SituacaoProducao > 1;
            }
            return false;
        }

        protected void txtValorFrete_Load(object sender, EventArgs e)
        {
            if (!PedidoConfig.ExibirValorFretePedido)
                ((WebControl)sender).Style.Add("Display", "none");
        }

        protected void chkOrdemCargaParcial_Load(object sender, EventArgs e)
        {
            if (!OrdemCargaConfig.UsarOrdemCargaParcial)
            {
                ((WebControl)sender).Visible = false;
            }
            else
            {
                var idPedido = Request["idPedido"].StrParaUint();
                var tipoEntregaEntrega = PedidoDAO.Instance.ObtemTipoEntrega(null, idPedido) == DataSources.Instance.GetTipoEntregaEntrega().GetValueOrDefault(0);
                var deveTransferir = PedidoDAO.Instance.ObtemDeveTransferir(null, idPedido);

                ((WebControl)sender).Enabled = tipoEntregaEntrega && !deveTransferir;
            }
        }

        protected void drpFormaPagto_DataBinding(object sender, EventArgs e)
        {
            uint idPedido = Conversoes.StrParaUint(Request["idPedido"]);
            var idCliente = PedidoDAO.Instance.GetIdCliente(null, idPedido);
            var idFormaPagto = PedidoDAO.Instance.GetFormaPagto(null, idPedido);

            if (idFormaPagto > 0)
            {
                var formaPagto = FormaPagtoDAO.Instance.GetElement(idFormaPagto.Value);

                var formas = FormaPagtoDAO.Instance.GetForPedido((int)idCliente);

                if (!formas.Contains(formaPagto))
                    ((DropDownList)sender).Items.Add(new ListItem(formaPagto.Descricao, formaPagto.IdFormaPagto.ToString()));
            }            
        }

        protected void drpFormaPagto_Load(object sender, EventArgs e)
        {
            if (!PedidoConfig.LiberarPedido)
            {
                var idFormaPagto = PedidoDAO.Instance.GetFormaPagto(null, Request["idPedido"].StrParaUint());
                var formaPagto = FormaPagtoDAO.Instance.GetElement(idFormaPagto.GetValueOrDefault());

                var item = new ListItem(formaPagto?.Descricao, idFormaPagto.ToString());

                if (!((DropDownList)sender).Items.Contains(item))
                    ((DropDownList)sender).Items.Add(item);
            }
        }
    }
}
