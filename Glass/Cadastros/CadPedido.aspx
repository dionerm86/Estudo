<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadPedido.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadPedido" Title="Cadastrar Pedido" EnableEventValidation="false" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlParcelasSelecionar.ascx" TagName="ctrlParcelasSelecionar" TagPrefix="uc6" %>
<%@ Register Src="../Controls/ctrlDadosDesconto.ascx" TagName="ctrlDadosDesconto" TagPrefix="uc7" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc8" %>
<%@ Register Src="../Controls/ctrlConsultaCadCliSintegra.ascx" TagName="ctrlConsultaCadCliSintegra" TagPrefix="uc9" %>
<%@ Register Src="../Controls/ctrlLimiteTexto.ascx" TagName="ctrlLimiteTexto" TagPrefix="uc10" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc11" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc12" %>
<%@ Register Src="../Controls/ctrlProdComposicao.ascx" TagName="ctrlProdComposicao" TagPrefix="uc13" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        
    var config_UsarBenefTodosGrupos = <%= Glass.Configuracoes.Geral.UsarBeneficiamentosTodosOsGrupos.ToString().ToLower() %>;
    var config_FastDelivery = <%= Glass.Configuracoes.PedidoConfig.Pedido_FastDelivery.FastDelivery.ToString().ToLower() %>;
    var config_GerarPedidoProducaoCorte = <%= Glass.Configuracoes.PedidoConfig.GerarPedidoProducaoCorte.ToString().ToLower() %>;
    var config_BloquearDadosClientePedido = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.BloquearDadosClientePedido.ToString().ToLower() %>;
    var config_UsarControleDescontoFormaPagamentoDadosProduto = <%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>;
    var config_ObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
    var config_UtilizarRoteiroProducao = <%= UtilizarRoteiroProducao().ToString().ToLower() %>;
    var config_UsarDescontoEmParcela = <%= Glass.Configuracoes.FinanceiroConfig.UsarDescontoEmParcela.ToString().ToLower() %>;
    var config_NumeroDiasUteisDataEntregaPedido = <%= Glass.Configuracoes.PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido %>;
    var config_ExibirPopupFaltaEstoque = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ExibePopupVidrosEstoque.ToString().ToLower() %>;
    var config_LiberarPedido = <%= Glass.Configuracoes.PedidoConfig.LiberarPedido.ToString().ToLower() %>;
    var config_DescontoApenasAVista = <%= Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoApenasAVista.ToString().ToLower() %>;
    var config_NumeroDiasPedidoProntoAtrasado = <%= Glass.Configuracoes.PedidoConfig.NumeroDiasPedidoProntoAtrasado %>;
    var config_UsarControleObraComProduto = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.UsarControleNovoObra.ToString().ToLower() %>;
    var config_UsarComissaoPorPedido = <%= Glass.Configuracoes.PedidoConfig.Comissao.PerComissaoPedido.ToString().ToLower() %>;
    var config_UsarComissionado = <%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente.ToString().ToLower() %>;
    var config_BuscarEnderecoClienteSeEstiverVazio = <%= Glass.Configuracoes.PedidoConfig.TelaCadastro.BuscarEnderecoClienteSeEstiverVazio.ToString().ToLower() %>;
    var config_PermitirDescontoAVistaComUmaParcela = <%= (Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoUmaParcela && Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoApenasAVista).ToString().ToLower() %>;
    var config_BloqEmisPedidoPorPosicaoMateriaPrima = <%= (Glass.Configuracoes.PedidoConfig.BloqEmisPedidoPorPosicaoMateriaPrima != Glass.Data.Helper.DataSources.BloqEmisPedidoPorPosicaoMateriaPrima.Bloquear).ToString().ToLower() %>
    var config_AlterarLojaPedido = <%= Glass.Configuracoes.PedidoConfig.AlterarLojaPedido.ToString().ToLower() %>;
    var config_UsarAltLarg = <%= Glass.Configuracoes.PedidoConfig.EmpresaTrabalhaAlturaLargura.ToString().ToLower() %>;
        
    var var_IdPedido = '<%= Request["idPedido"] %>';
    var var_CodCartao = CadPedido.GetCartaoCod().value;
    var var_DataEntregaAntiga = "<%= GetDataEntrega() %>";
    var var_IgnorarBloqueioDataEntrega = "<%= IgnorarBloqueioDataEntrega() %>";
    var var_NomeControleParcelas = "<%= dtvPedido.ClientID %>_ctrlParcelas1";
    var var_PedidoMaoDeObra = '<%= Request["maoObra"] %>' == 1;
    var var_BloquearDataEntrega = <%= GetBloquearDataEntrega().ToString().ToLower() %>;
    var var_ValorDescontoTotalProdutos = <%= GetDescontoProdutos() %>;
    var var_ValorDescontoTotalPedido = <%= GetDescontoPedido() %>;
    var var_QtdProdutosPedido = <%= GetNumeroProdutosPedido() %>;
    var var_TipoEntregaBalcao = <%= GetTipoEntregaBalcao() %>;
    var var_TipoEntregaEntrega = <%= GetTipoEntrega() %>;
    var var_TotalM2Pedido = "<%= GetTotalM2Pedido() %>";
    var var_DataPedido = "<%= GetDataPedido() %>";
    var var_QtdEstoque = 0;
    var var_QtdEstoqueMensagem = 0;
    var var_ExibirMensagemEstoque = false;
    var var_Inserting = false;
    var var_ProdutoAmbiente = false;
    var var_AplAmbiente = false;
    var var_ProcAmbiente = false;
    var var_Loading = true;
    
    function podeSelecionarObra()
    {
        if (FindControl("hdfCliente", "input").value == "")
        {
            alert("Selecione um cliente antes de selecionar a obra.");
            return false;
        }
        else
            return true;
    }
    
    function mensagemProdutoComDesconto(editar)
    {
        alert("Não é possível " + (editar ? "editar" : "remover") + " esse produto porque o pedido possui desconto.\n" +
            "Aplique o desconto apenas ao terminar o cadastro dos produtos.\n" +
            "Para continuar, remova o desconto do pedido.");
    }
    
    function alteraDataPedidoFunc(idFunc)
    {
        var txt = FindControl("txtDataPed", "input");
        var hdf = FindControl("hdfDataPedido", "input");
        
        var data = CadPedido.GetAtrasoFuncionario(idFunc).value;
        txt.value = data;
        hdf.value = data;
        
        alteraDataEntrega(false);
    }
    
    function alteraDataEntrega(forcarAlteracao)
    {
        var idCli = FindControl("txtNumCli", "input").value;
        var tipoEntrega = FindControl("ddlTipoEntrega", "select").value;
        var isFastDelivery = FindControl("chkFastDelivery", "input");
        var dataBase = var_IdPedido != "" ? "" : FindControl("txtDataPed", "input").value;
        isFastDelivery = isFastDelivery != null ? isFastDelivery.checked : "false";
        var campoDataEntrega = FindControl("ctrlDataEntrega_txtData", "input");
        var tipoPedido = FindControl("hdfTipoPedido", "input") != null ? FindControl("hdfTipoPedido", "input").value : FindControl("drpTipoPedido", "select").value;

        var dataEntrega = CadPedido.GetDataEntrega(idCli, var_IdPedido, tipoPedido, tipoEntrega, dataBase, isFastDelivery).value.split(';');
        if (dataEntrega[0] != "")
        {
            // Altera a data de entrega somente se o campo data entrega estiver vazio, ou se a data preenchida atualmente for menor do que a 
            // data mínima permitida ou se o método tiver sido chamado forçando a alteração porém sem estar no load da página.
            if (campoDataEntrega.value == "" || firstGreaterThenSec(dataEntrega[0], campoDataEntrega.value) || (forcarAlteracao && !var_Loading))
            {
                // Altera a data de entrega somente se o usuário não tiver permissão de ignorar bloqueio na data de entrega ou se for inserção
                // ou se o campo estiver vazio
                if (var_IgnorarBloqueioDataEntrega == "false" || campoDataEntrega.value == "" || var_IdPedido == '') {
                    campoDataEntrega.value = dataEntrega[0];
                    FindControl("hdfDataEntregaNormal", "input").value = dataEntrega[0];
                }

                // Chamado 14801: Caso esteja sendo usado apenas o controle de dias mínimos do subgrupo é necessário usar esta condição
                if ((firstGreaterThenSec(dataEntrega[0], campoDataEntrega.value) || (forcarAlteracao && !var_Loading))) {
                    campoDataEntrega.value = dataEntrega[0];
                    FindControl("hdfDataEntregaNormal", "input").value = dataEntrega[0];
                }
            }

            var desabilitar = dataEntrega[1] == "true";
            FindControl("imgData", "input").disabled = desabilitar;
        }

        controlarVisibilidadeProducaoCorte();
    }

    function controlarVisibilidadeProducaoCorte()
    {
        if (FindControl("drpTipoPedido", "select") == null)
            return;

        var tipoPedido = FindControl("drpTipoPedido", "select").value;

        var chkGerarPedidoProducaoCorte = FindControl("chkGerarPedidoProducaoCorte", "input");
        var divGerarPedidoProducaoCorte = FindControl("divGerarPedidoProducaoCorte", "div");

        if(chkGerarPedidoProducaoCorte != null && (tipoPedido != '2' || !config_GerarPedidoProducaoCorte)){
            chkGerarPedidoProducaoCorte.checked = false;
            divGerarPedidoProducaoCorte.style.display = 'none';
            chkGerarPedidoProducaoCorte.parentNode.style.display = 'none';
        }
        else
        {
            divGerarPedidoProducaoCorte.style.display = '';
            chkGerarPedidoProducaoCorte.parentNode.style.display = '';
        }
    }
    
    function loadAjax(tipo)
    {
        if (!config_BloquearDadosClientePedido && !config_UsarControleDescontoFormaPagamentoDadosProduto)
        {
            return null;
        }
        
        // O cliente não deve ser informado ao método caso a configuração de bloqueio de dados do cliente no pedido esteja desabilitada.
        var idCli = config_BloquearDadosClientePedido && FindControl("txtNumCli", "input") != null ? FindControl("txtNumCli", "input").value : "";
        // O tipo de venda do pedido não deve ser informado caso o controle de desconto por forma de pagamento e dados do produto esteja desabilitado.
        var tipoVenda = config_UsarControleDescontoFormaPagamentoDadosProduto && FindControl("drpTipoVenda", "select") != null ? FindControl("drpTipoVenda", "select").value : "";

        var retorno = CadPedido.LoadAjax(tipo, idCli, tipoVenda);
        
        if (retorno.error != null)
        {
            alert(retorno.error.description);
            return null;
        }
        else if (retorno.value == null)
        {
            alert("Falha de Ajax ao carregar tipo '" + tipo + "'.");
        }
        
        return retorno.value;
    }
    
    function atualizaTipoVendaCli()
    {
        var ajax = loadAjax("tipoVenda");
        
        if (ajax == null || FindControl("drpTipoVenda", "select") == null)
        {
            return true;
        }
        
        var drpTipoVenda = FindControl("drpTipoVenda", "select");  
        // Salva o valor selecionado antes de preencher novamente a drop por ajax
        var tipoVenda = drpTipoVenda.value;
        // Carrega os valores possíveis para o tipo venda
        drpTipoVenda.innerHTML = ajax;
        // Volta o tipo de venda que estava selecionado
        drpTipoVenda.value = tipoVenda;
        drpTipoVenda.onchange();

        // As formas de pagamento do cliente devem ser carregadas após recuperar o tipo de venda, para que as formas de pagamento corretas sejam recuperadas.
        // OBS.: o tipo de venda À Vista não deve recuperar a forma de pagamento Prazo. o tipo de venda À Prazo não deve recuperar a forma de pagamento Dinheiro.
        atualizaFormasPagtoCli();
    }

    // IMPORTANTE: ao alterar esse método, altere as telas DescontoPedido.aspx, CadDescontoFormaPagtoDadosProduto.aspx e CadPedido.aspx.
    function atualizaFormasPagtoCli()
    {
        var drpFormaPagto = FindControl("drpFormaPagto", "select");
        
        // Verifica se o controle de forma de pagamento existe na tela.
        if (drpFormaPagto == null)
        {
            return true;
        }

        // Salva em uma variável a forma de pagamento selecionada, antes do recarregamento das opções da Drop Down List.
        var idFormaPagtoAtual = drpFormaPagto.value;
        // Recupera as opções de forma de pagamento disponíveis.
        var ajax = loadAjax("formaPagto");
        
        // Verifica se ocorreu algum erro na chamada do Ajax.
        if (ajax.error != null)
        {
            alert(ajax.error.description);
            return false;
        }
        else if (ajax == null)
        {
            return false;
        }
        
        // Atualiza a Drop Down List com as formas de pagamento disponíveis.
        drpFormaPagto.innerHTML = ajax;

        // Variável criada para informar se a forma de pagamento pré-selecionada existe nas opções atuais da Drop Down List de forma de pagamento.
        var formaPagtoEncontrada = false;

        // Percorre cada forma de pagamento atual e verifica se a opção pré-selecionada existe entre elas.
        for (var i = 0; i < drpFormaPagto.options.length; i++)
        {
            if (drpFormaPagto.options[i].value == idFormaPagtoAtual)
            {
                formaPagtoEncontrada = true;
                break;
            }
        }
         
        // Caso a forma de pagamento exista nas opções atuais, seleciona ela na Drop.
        if (formaPagtoEncontrada)
        {
            drpFormaPagto.value = idFormaPagtoAtual;
        }

        drpFormaPagto.onchange();
    }
    
    function atualizaValMin()
    {
        if (parseFloat(FindControl("hdfTamanhoMaximoObra", "input").value.replace(",", ".")) == 0)
        {
            var codInterno = FindControl("txtCodProdIns", "input");
            codInterno = codInterno != null ? codInterno.value : FindControl("lblCodProdIns", "span").innerHTML;
            
            var tipoPedido = FindControl("hdfTipoPedido", "input").value;
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;       
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var tipoVenda = FindControl("hdfTipoVenda", "input").value;
            
            var idProdPed = FindControl("hdfProdPed", "input");
            idProdPed = idProdPed != null ? idProdPed.value : "";
            
            var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
            
            var percDescontoQtde = controleDescQtde.PercDesconto();
            
            FindControl("hdfValMin", "input").value = CadPedido.GetValorMinimo(codInterno, tipoPedido, tipoEntrega, tipoVenda, 
                idCliente, cliRevenda, idProdPed, percDescontoQtde, var_IdPedido).value;
        }
        else
            FindControl("hdfValMin", "input").value = FindControl("txtValorIns", "input").value;
    }
    
    function obrigarProcApl()
    {
        var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
        var isVidroRoteiro = dadosProduto.Grupo == 1 && config_UtilizarRoteiroProducao;
        var tipoCalculo = FindControl("hdfTipoCalc", "input") != null && FindControl("hdfTipoCalc", "input") != undefined && FindControl("hdfTipoCalc", "input").value != undefined ? FindControl("hdfTipoCalc", "input").value : "";
        
        if (dadosProduto.IsChapaVidro)
            return true;

        /* Chamado 63268. */
        if ((tipoCalculo != "" && (tipoCalculo == "2" || tipoCalculo == "10")) && (isVidroRoteiro || (config_ObrigarProcApl && isVidroBenef)))
        {
            if (FindControl("txtAplIns", "input") != null && FindControl("txtAplIns", "input").value == "")
            {
                if (isVidroRoteiro && !config_ObrigarProcApl) {
                    alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                    return false;
                }

                alert("Informe a aplicação.");
                return false;
            }
            
            if (FindControl("txtProcIns", "input") != null && FindControl("txtProcIns", "input").value == "")
            {
                if (isVidroRoteiro && !config_ObrigarProcApl) {
                    alert("É obrigatório informar o processo caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                    return false;
                }

                alert("Informe o processo.");
                return false;
            }
        }
        
        return true;
    }
    
    function calculaTamanhoMaximo()
    {
        if (FindControl("lblCodProdIns", "span") == null)
            return;
            
        var codInterno = FindControl("lblCodProdIns", "span").innerHTML;
        var totM2 = FindControl("lblTotM2Ins", "span").innerHTML;
        var idProdPed = FindControl("hdfProdPed", "input") != null ? FindControl("hdfProdPed", "input").value : 0;
        
        var tamanhoMaximo = CadPedido.GetTamanhoMaximoProduto(var_IdPedido, codInterno, totM2, idProdPed).value.split(";");
        tamanhoMaximo = tamanhoMaximo[0] == "Ok" ? parseFloat(tamanhoMaximo[1].replace(",", ".")) : 0;
        
        FindControl("hdfTamanhoMaximoObra", "input").value = tamanhoMaximo;
    }
    
    function validaTamanhoMax()
    {
        var tamanhoMaximo = parseFloat(FindControl("hdfTamanhoMaximoObra", "input").value.replace(",", "."));
        if (tamanhoMaximo > 0)
        {        
            var totM2 = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(",", "."));
            if (totM2 > tamanhoMaximo)
            {
                alert("O total de m² da peça ultrapassa o máximo definido no pagamento antecipado. Tamanho máximo restante: " + tamanhoMaximo.toString().replace(".", ",") + " m²");
                return false;
            }
        }
        
        return true;
    }
    
    function exibirBenef(botao, idProdPed)
    {
        for (iTip = 0; iTip < 2; iTip++)
        {
            TagToTip('tbConfigVidro_' + idProdPed, FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true, 
                CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true, 
                FIX, [botao, 9-getTableWidth('tbConfigVidro_' + idProdPed), -41-getTableHeight('tbConfigVidro_' + idProdPed)]);
        }
    }
    
    function calcularDesconto(tipoCalculo)
    {
        var controle = FindControl("txtDesconto", "input");
        if (controle.value == "0")
            return;

        var tipo = FindControl("drpTipoDesconto", "select").value;
        var desconto = parseFloat(controle.value.replace(',', '.'));
        if (isNaN(desconto))
            desconto = 0;
        
        var tipoAtual = FindControl("hdfTipoDesconto", "input").value;
        var descontoAtual = parseFloat(FindControl("hdfDesconto", "input").value.replace(',', '.'));
        if (isNaN(descontoAtual))
            descontoAtual = 0;
        
        var alterou = tipo != tipoAtual || desconto != descontoAtual;
        var descontoMaximo = CadPedido.PercDesconto(var_IdPedido, alterou).value.replace(',', '.');

        //Busca o Desconto por parcela ou por Forma de pagamento e dados do produto
        var retDesconto = 0;

        if (config_UsarDescontoEmParcela && FindControl("drpParcelas","select") != null)
        {
            retDesconto = CadPedido.VerificaDescontoParcela(FindControl("drpParcelas","select").value, var_IdPedido);
        }
        else if (config_UsarControleDescontoFormaPagamentoDadosProduto)
        {
            var tipoVenda = FindControl("drpTipoVenda", "select") != null ? FindControl("drpTipoVenda", "select").value : "";
            var idFormaPagto = FindControl("drpFormaPagto", "select") != null ? FindControl("drpFormaPagto", "select").value : "";
            var idTipoCartao = FindControl("drpTipoCartao", "select") != null ? FindControl("drpTipoCartao", "select").value : "";
            var idParcela = FindControl("drpParcelas", "select") != null ? FindControl("drpParcelas", "select").value : "";
            
            retDesconto = CadPedido.VerificaDescontoFormaPagtoDadosProduto(var_IdPedido, tipoVenda, idFormaPagto, idTipoCartao, idParcela);
        }

        if (retDesconto.error != null)
        {
            alert(retDesconto.error.description);
            return false;
        }

        if (descontoMaximo == 0)
            return true;

        var total = parseFloat(FindControl("hdfTotalSemDesconto", "input").value.replace(/\./g, "").replace(',', '.'));
        var totalProduto = tipoCalculo == 2 ? parseFloat(FindControl("lblTotalProd", "span").innerHTML.replace("R$", "").replace(" ", "").replace(/\./g, "").replace(',', '.')) : 0;
        var valorDescontoMaximo = total * (descontoMaximo / 100);
        
        var valorDescontoProdutos = var_ValorDescontoTotalProdutos - (tipoCalculo == 2 ? parseFloat(FindControl("hdfValorDescontoAtual", "input").value.replace(',', '.')) : 0);
        var valorDescontoPedido = tipoCalculo == 2 ? var_ValorDescontoTotalPedido : 0;
        var descontoProdutos = parseFloat(((valorDescontoProdutos / (total > 0 ? total : 1)) * 100).toFixed(2));
        var descontoPedido = parseFloat(((valorDescontoPedido / (total > 0 ? total : 1)) * 100).toFixed(2));
        
        var descontoSomar = descontoProdutos + (tipoCalculo == 2 ? descontoPedido : 0);
        var valorDescontoSomar = valorDescontoProdutos + (tipoCalculo == 2 ? valorDescontoPedido : 0);
        
        if (tipo == 2 && desconto > 0 && total > 0)
            desconto = (desconto / total) * 100;

        //Se tiver desconto de parcela e o desconto da parcela for maior que o desconto maximo, não deve bloquear
        if (retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "" && retDesconto.value != undefined && parseFloat(retDesconto.value.replace(",", ".")) == parseFloat((desconto + descontoSomar).toFixed(2)))
        {
            return true;
        }
        
        if (parseFloat((desconto + descontoSomar).toFixed(2)) > parseFloat(descontoMaximo) && !var_Loading)
        {
            var mensagem = "O desconto máximo permitido é de " + (tipo == 1 ? descontoMaximo + "%" : "R$ " + valorDescontoMaximo.toFixed(2).replace('.', ',')) + ".";
            if (descontoProdutos > 0)
                mensagem += "\nO desconto já aplicado aos produtos é de " + (tipo == 1 ? descontoProdutos + "%" : "R$ " + valorDescontoProdutos.toFixed(2).replace('.', ',')) + ".";
            
            if (descontoPedido > 0)
                mensagem += "\nO desconto já aplicado ao pedido é de " + (tipo == 1 ? descontoOrcamento + "%" : "R$ " + valorDescontoPedido.toFixed(2).replace('.', ',')) + ".";
            
            alert(mensagem);
            controle.value = tipo == 1 ? (descontoMaximo - descontoSomar).toFixed(2).replace('.', ',') : (valorDescontoMaximo - valorDescontoSomar).toFixed(2).replace('.', ',') ;
            
            if (parseFloat(controle.value.replace(',', '.')) < 0)
                controle.value = "0";
                
            return false;
        }
        
        return true;
    }
    
    function alteraFastDelivery(isFastDelivery)
    {            
        if (isFastDelivery) {
                        
            var retorno = CadPedido.PodeMarcarFastDelivery(var_IdPedido).value;

            var resultado = retorno.split('|');
            if (resultado[0] == "Erro") {
                FindControl("chkFastDelivery", "input").checked = false;
                return alert(resultado[1]);
            }
        }

        var alterar = config_NumeroDiasUteisDataEntregaPedido > 0;
        if (!alterar && !isFastDelivery)
            return;
        
        var novaData = isFastDelivery ? FindControl("hdfDataEntregaFD", "input").value : FindControl("hdfDataEntregaNormal", "input").value;
        FindControl("ctrlDataEntrega_txtData", "input").value = novaData;
    }
    
    function limparComissionado()
    {
        FindControl("hdfIdComissionado", "input").value = "";
        FindControl("lblComissionado", "span").innerHTML = "";
        FindControl("txtPercentual", "input").value = "0";
        FindControl("txtValorComissao", "input").value = "R$ 0,00";
    }
    
    function getProduto()
    {
        openWindow(450, 700, '../Utils/SelProd.aspx?IdPedido=' + var_IdPedido + (var_ProdutoAmbiente ? "&ambiente=true" : ""));
    }
    
    function verificaDataEntrega(controle)
    {
        if (config_NumeroDiasUteisDataEntregaPedido == 0)
            return true;
        
        if (var_PedidoMaoDeObra || FindControl("hdfDataEntregaNormal", "input") == null)
            return true;
            
        var textoDataMinima = FindControl("hdfDataEntregaNormal", "input").value;
        var dataControle = textoDataMinima.split("/");
        var dataMinima = new Date(dataControle[2], parseInt(dataControle[1], 10) - 1, dataControle[0]);
        var isDataMinima = var_BloquearDataEntrega;
        
        dataControle = controle.value.split("/");
        var dataAtual = new Date(dataControle[2], parseInt(dataControle[1], 10) - 1, dataControle[0]);
        
        var fastDelivery = FindControl("chkFastDelivery", "input");
        fastDelivery = fastDelivery != null ? fastDelivery.checked : false;

        if (isDataMinima && !fastDelivery && dataAtual < dataMinima)
        {
            alert("Não é possível escolher uma data anterior a " + textoDataMinima + ".");
            controle.value = textoDataMinima;
            var_DataEntregaAntiga = textoDataMinima;
            return false;
        }
        else if (MetodosAjax.IsDiaUtil(dataAtual).value == "false")
        {
            alert("Não é possível escolher sábado, domingo ou feriado como dia de entrega.");
            controle.value = var_DataEntregaAntiga;
            return false;
        }
        else
            var_DataEntregaAntiga = controle.value;
        
        return true;
    }
    
    function getNomeControleBenef()
    {
        var nomeControle = FindControl("ctrlBenefEditar", "input") != null ? "ctrlBenefEditar" : "ctrlBenefInserir";
        nomeControle = FindControl(nomeControle + "_tblBenef", "table");
        
        if (nomeControle == null)
            return null;
        
        nomeControle = nomeControle.id;
        return nomeControle.substr(0, nomeControle.lastIndexOf("_"));
    }
    
    function setValorTotal(valor, custo)
    {
        if (getNomeControleBenef() != null) {
            if (exibirControleBenef(getNomeControleBenef()))
            {
                var lblValorBenef = FindControl("lblValorBenef", "span");
                lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
            }
        }
    }
    
    function setObra(idCliente, idObra, descrObra, saldo)
    {
        FindControl("hdfIdObra", "input").value = idObra;
        FindControl("txtObra", "input").value = descrObra;
        FindControl("lblSaldoObra", "span").innerHTML = saldo.replace(/\n/g, "<br />");
        
        if (idCliente > 0)
        {
            FindControl("txtNumCli", "input").value = idCliente;
            getCli(FindControl("txtNumCli", "input").value);
        }
    }
    
    // Função chamada após selecionar produto pelo popup
    function setProduto(codInterno) {
        try {
            if (!var_ProdutoAmbiente)
                FindControl("txtCodProd", "input").value = codInterno;
            else
                FindControl("txtCodAmb", "input").value = codInterno;
            
            loadProduto(codInterno, 0);
        }
        catch (err) {

        }
    }
    
    var comissaoAlteraValor = null;
    
    // Retorna o percentual de comissão
    function getPercComissao()
    {
        var percComissao = 0;
        var txtComissao = FindControl("txtPercentual", "input");
        var hdfPercComissao = FindControl("hdfPercComissao", "input");
        var hdfIdPedido = FindControl("hdfIdPedido", "input");
    
        if (comissaoAlteraValor == null)
            comissaoAlteraValor = MetodosAjax.ComissaoAlteraValor(hdfIdPedido.value).value;
    
        if (hdfIdPedido != null && comissaoAlteraValor == "false")
            return 0;
    
        if (txtComissao != null && txtComissao.value != "")
            percComissao = parseFloat(txtComissao.value.replace(',', '.'));
        else if (hdfPercComissao != null && hdfPercComissao.value != "")
            percComissao = parseFloat(hdfPercComissao.value.replace(',', '.'));
            
        return percComissao != null ? percComissao : 0;
    }

    // Carrega dados do produto com base no código do produto passado
    function loadProduto(codInterno, idProdPed, manterProcessoAplicacao) {
        if (codInterno == "")
            return false;
        
        var txtValor = FindControl("txtValorIns", "input");

        try {
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;       
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var percComissao = getPercComissao();
            var tipoPedido = FindControl("hdfTipoPedido", "input").value;
            var tipoVenda = FindControl("hdfTipoVenda", "input").value;
            percComissao = percComissao == null ? 0 : percComissao.toString().replace('.', ',');
            
            var controleDescQtde = null; 
            var percDescontoQtde = 0;
            
            if (FindControl("_divDescontoQtde", "div") != null)
            {
                controleDescQtde = FindControl("_divDescontoQtde", "div").id;
                controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
                if (controleDescQtde != null) 
                    percDescontoQtde = controleDescQtde.PercDesconto();
            }
            
            var retorno = CadPedido.GetProduto(var_IdPedido, codInterno, tipoEntrega, cliRevenda, idCliente, 
                percComissao, tipoPedido, tipoVenda, var_ProdutoAmbiente, percDescontoQtde, FindControl("hdfLoja", "input").value, false).value.split(';');

            if (!manterProcessoAplicacao && FindControl("txtProcIns", "input") != null)
                FindControl("txtProcIns", "input").value = "";

            if (tipoPedido == 2 && CadPedido.GerarPedidoProducaoCorte(var_IdPedido).value == "true")
            {
                var tipoSubGrupo = CadPedido.ObterSubgrupoProd(codInterno); 
                if(tipoSubGrupo.value != "1")
                {
                    alert('Esse produto não pode ser utilizado, pois não pertence ao Sub-Grupo Chapas de Vidro.');
                    return false;
                }
            }

            var verificaProduto = CadPedido.IsProdutoObra(var_IdPedido, codInterno, false).value.split(";");        
            if (verificaProduto[0] == "Erro")
            {
                if (FindControl("txtCodProd", "input") != null)
                    FindControl("txtCodProd", "input").value = "";
                    
                alert("Esse produto não pode ser usado no pedido. " + verificaProduto[1]);
                return false;
            }
            else if (parseFloat(verificaProduto[1].replace(",", ".")) > 0)
            {
                if (txtValor != null)
                    txtValor.disabled = true;
            
                // Se for edição de produto, chamad o método padrão de cálculo da metragem máxima permitida
                if (FindControl("hdfProdPed", "input") != null)
                    calculaTamanhoMaximo();
                else if (FindControl("hdfTamanhoMaximoObra", "input") != null)    
                    FindControl("hdfTamanhoMaximoObra", "input").value = verificaProduto[2];
            }
            else
            {
                if (txtValor != null)
                    txtValor.disabled = verificaProduto[3] == "false";
            
                if (FindControl("hdfTamanhoMaximoObra", "input") != null)    
                    FindControl("hdfTamanhoMaximoObra", "input").value = "0";
            }

            var idLojaSubgrupo = CadPedido.ObterLojaSubgrupoProd(codInterno);    
            var idLoja = FindControl("hdfLoja", "input").value;

            if(idLojaSubgrupo.error!=null){

                if (FindControl("txtCodProd", "input") != null)
                    FindControl("txtCodProd", "input").value = "";

                alert(idLojaSubgrupo.error.description);
                return false;
            }
       
            if(idLojaSubgrupo.value != "0" && idLojaSubgrupo.value != idLoja){

                if (FindControl("txtCodProd", "input") != null)
                    FindControl("txtCodProd", "input").value = "";

                alert('Esse produto não pode ser utilizado, pois a loja do seu subgrupo é diferente da loja do pedido.');
                return false;
            }

            var validaClienteSubgrupo = MetodosAjax.ValidaClienteSubgrupo(FindControl("hdfIdCliente", "input").value, codInterno);    
            if(validaClienteSubgrupo.error!=null){

                if (FindControl("txtCodProd", "input") != null)
                    FindControl("txtCodProd", "input").value = "";

                alert(validaClienteSubgrupo.error.description);
                return false;
            }
            
            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                if (!var_ProdutoAmbiente)
                    FindControl("txtCodProd", "input").value = "";
                else
                    FindControl("txtCodAmb", "input").value = "";
                
                return false;
            }
            
            else if (!var_ProdutoAmbiente)
            {
                if (retorno[0] == "Prod") {
                    FindControl("hdfIdProd", "input").value = retorno[1];

                    // Caso o vendedor não possa alterar o valor vendido do produto OU o valor vendido do produto seja zero ou o valor vendido do produto seja menor que o valor de tabela,
                    // atualiza o valor da obra ou de tabela do produto.
                    if (verificaProduto[3] == "false" || txtValor.value == "" || parseFloat(txtValor.value.toString().replace(",", ".")) == 0 || parseFloat(txtValor.value.toString().replace(",", ".")) < parseFloat(retorno[3].toString().replace(",", ".")))
                    {
                        if (verificaProduto[1] != "0") // Exibe no cadastro o valor mínimo do produto
                            txtValor.value = verificaProduto[1];
                            // O valor do produto deve ser atualizado sempre, para que caso seja buscado um produto, preenchendo automaticamente
                            // o valor unitário e o usuário resolva buscar outro produto sem ter inserido o primeiro, garanta que será buscado o valor deste
                        else 
                            txtValor.value = retorno[3];
                    }
                    
                    FindControl("hdfIsVidro", "input").value = retorno[4]; // Informa se o produto é vidro
                    FindControl("hdfM2Minimo", "input").value = retorno[5]; // Informa se o produto possui m² mínimo
                    FindControl("hdfTipoCalc", "input").value = retorno[7]; // Verifica como deve ser calculado o produto
                    
                    // Se o campo do valor estiver desativado não precisa calcular o valor mínimo, tendo em vista que o usuário não poderá alterar.
                    if (!txtValor.disabled)
                        atualizaValMin();
                    
                    var_QtdEstoque = retorno[6]; // Pega a quantidade disponível em estoque deste produto
                    var_ExibirMensagemEstoque = retorno[14] == "true";
                    var_QtdEstoqueMensagem = retorno[15];
                    
                    var tipoCalc = retorno[7];

                    // Se o produto não for vidro, desabilita os textboxes largura e altura,
                    // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
                    // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
                    var cAltura = FindControl("txtAlturaIns", "input");
                    var cLargura = FindControl("txtLarguraIns", "input");
                    var maoDeObra = FindControl("hdfPedidoMaoDeObra", "input").value == "true";
                    var alturaAmbiente = FindControl("hdfAlturaAmbiente", "input").value;
                    var larguraAmbiente = FindControl("hdfLarguraAmbiente", "input").value;
                    cAltura.disabled = maoDeObra || CalcProd_DesabilitarAltura(tipoCalc);
                    cLargura.disabled = maoDeObra || CalcProd_DesabilitarLargura(tipoCalc);
                    
                    if (maoDeObra && alturaAmbiente > 0) {
                        cAltura.value = tipoCalc != 1 && tipoCalc != 5 ? alturaAmbiente : "";
                        FindControl("hdfAlturaReal", "input").value = cAltura.value;
                    }
                        
                    if (maoDeObra && larguraAmbiente > 0) 
                        cLargura.value = tipoCalc != 1 && tipoCalc != 4 && tipoCalc != 5 && tipoCalc != 6 && tipoCalc != 7 && tipoCalc != 8 ? larguraAmbiente : "";
                    
                    var nomeControle = getNomeControleBenef();
                    var tbConfigVidro = FindControl("tbConfigVidro_" + idProdPed, "table");

                    // Zera o campo qtd para evitar que produtos calculados por mҠfiquem com quantidade decimal por exemplo (chamado 11010)
                    var txtQtdProd = FindControl("txtQtdeIns", "input");
                    if (txtQtdProd != null && !var_Loading)
                        txtQtdProd.value = "";
                    
                    if (tbConfigVidro != null && tbConfigVidro != undefined && nomeControle != null && nomeControle != undefined) {
                        // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                        if (retorno[4] == "true" && exibirControleBenef(nomeControle) && FindControl("lnkBenef", "a") != null) {
                            FindControl("txtEspessura", "input", tbConfigVidro).value = retorno[8];
                            FindControl("txtEspessura", "input", tbConfigVidro).disabled = retorno[8] != "" && retorno[8] != "0";
                        }
                    
                        if (FindControl("lnkBenef", "a") != null && nomeControle != null && nomeControle.indexOf("Inserir") > -1)
                            FindControl("lnkBenef", "a").style.display = exibirControleBenef(nomeControle) ? "" : "none";
                    }
                        
                    FindControl("hdfAliquotaIcmsProd", "input").value = retorno[9].replace('.', ',');
                    
                    // O campo altura e largura devem sempre ser atribuídos pois caso seja selecionado um box e logo após seja selecionado um kit 
                    // por exemplo, ao inserí-lo ele estava ficando com o campo altura, largura e m² preenchidos apesar de ser calculado por qtd
                    if (retorno[10] != "" || retorno[4] == "false") {
                        FindControl("txtAltura", "input").value = retorno[10];
                        FindControl("hdfAlturaReal", "input").value = retorno[10];
                    }
                    if (retorno[11] != "" || retorno[4] == "false") FindControl("txtLargura", "input").value = retorno[11];
                        
                    if (cAltura.disabled && FindControl("hdfAlturaReal", "input") != null)
                        FindControl("hdfAlturaReal", "input").value = cAltura.value;

                    if (!manterProcessoAplicacao && retorno[16] != "")
                        setApl(retorno[16], retorno[17]);
                    
                    if (!manterProcessoAplicacao && retorno[18] != "")
                        setProc(retorno[18], retorno[19]);
                    
                    FindControl("hdfCustoProd", "input").value = retorno[20];
                }

                FindControl("lblDescrProd", "span").innerHTML = retorno[2];

                if (retorno.length >= 22)
                    FindControl("lblDescrProd", "span").innerHTML += " (Valor m²: " + retorno[21] + ")";
            }
            else
            {
                FindControl("hdfAmbIdProd", "input").value = retorno[1];
                FindControl("lblDescrAmb", "span").innerHTML = retorno[2];
                FindControl("hdfDescrAmbiente", "input").value = retorno[2];
            }
        }
        catch (err) {
            alert(err);
        }
        
        var_ProdutoAmbiente = false;
    }

    // Se o produto sendo adicionado for ferragem e se a empresa for charneca, informa se qtd vendida
    // do produto existe no estoque
    function verificaEstoque() {
        var txtQtd = FindControl("txtQtdeIns", "input").value;
        var txtAltura = FindControl("txtAlturaIns", "input").value;
        var tipoCalc = FindControl("hdfTipoCalc", "input").value;
        var totM2 = FindControl("lblTotM2Ins", "span").innerHTML;
        var isCalcAluminio = tipoCalc == 4 || tipoCalc == 6 || tipoCalc == 7 || tipoCalc == 9;
        var isCalcM2 = tipoCalc == 2 || tipoCalc == 10;
    
        // Se for cálculo por barra de 6m, multiplica a qtd pela altura
        if (isCalcAluminio)
            txtQtd = parseInt(txtQtd) * parseFloat(txtAltura.toString().replace(',', '.'));
        else if (isCalcM2)
        {
            if (totM2 == "") 
                return;
            
            txtQtd = totM2;
        }
    
        var estoqueMenor = txtQtd != "" && parseInt(txtQtd) > parseInt(var_QtdEstoque);
        if (estoqueMenor)
        {
            if (var_QtdEstoque == 0)
                alert("Não há nenhuma peça deste produto no estoque.");
            else
                alert("Há apenas " + var_QtdEstoque + " " + (isCalcM2 ? "m²" : isCalcAluminio ? "ml (" + parseFloat(var_QtdEstoque / 6).toFixed(2) + " barras)" : "peça(s)") + " deste produto no estoque.");
                
            FindControl("txtQtdeIns", "input").value = "";
        }
        
        if (config_ExibirPopupFaltaEstoque && var_ExibirMensagemEstoque && (var_QtdEstoqueMensagem <= 0 || estoqueMenor))
            openWindow(400, 600, "../Utils/DadosEstoque.aspx?idProd=" + FindControl("hdfIdProd", "input").value + "&idPedido=" + var_IdPedido);
    }

    // Função chamada pelo popup de escolha da Aplicação do produto
    function setApl(idAplicacao, codInterno) {
        var verificaEtiquetaApl = MetodosAjax.VerificaEtiquetaAplicacao(idAplicacao, FindControl("hdfIdPedido", "input").value);
        if(verificaEtiquetaApl.error != null){

            if (!var_AplAmbiente)
            {
                FindControl("txtAplIns", "input").value = "";
                FindControl("hdfIdAplicacao", "input").value = "";
            }
            else
            {
                FindControl("txtAmbAplIns", "input").value = "";
                FindControl("hdfAmbIdAplicacao", "input").value = "";
            }

            alert(verificaEtiquetaApl.error.description);
            return false;
        }

        if (!var_AplAmbiente && FindControl("txtAplIns", "input") != null)
        {
            FindControl("txtAplIns", "input").value = codInterno;
            FindControl("hdfIdAplicacao", "input").value = idAplicacao;
        }
        else if (FindControl("txtAmbAplIns", "input") != null && FindControl("hdfAmbIdAplicacao", "input") != null)
        {
            FindControl("txtAmbAplIns", "input").value = codInterno;
            FindControl("hdfAmbIdAplicacao", "input").value = idAplicacao;
        }
        
        var_AplAmbiente = false;
    }

    function loadApl(codInterno) {
        if (codInterno == undefined || codInterno == "") {
            setApl("", "");
            return false;
        }
    
        try {
            var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Aplicação. Ajax Error.");
                setApl("", "");
                return false
            }

            response = response.split("\t");
            
            if (response[0] == "Erro") {
                alert(response[1]);
                setApl("", "");
                return false;
            }

            setApl(response[1], response[2]);
        }
        catch (err) {
            alert(err);
        }
    }

    // Função chamada pelo popup de escolha do Processo do produto
    function setProc(idProcesso, codInterno, codAplicacao) {
        var codInternoProd = "";
        var codAplicacaoAtual = "";
        
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProd", "input").value);
        var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

        if(idSubgrupo.value != "" && retornoValidacao.value == "False" && (FindControl("txtProcIns", "input") != null && FindControl("txtProcIns", "input").value != ""))
        {
            FindControl("txtProcIns", "input").value = "";
            alert("Este processo não pode ser selecionado para este produto.")
            return false;
        }

        var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, FindControl("hdfIdPedido", "input").value);
        if(verificaEtiquetaProc.error != null){

            if (!var_ProcAmbiente && FindControl("txtProcIns", "input") != null)
            {
                FindControl("txtProcIns", "input").value = "";
                FindControl("hdfIdProcesso", "input").value = "";
            }
            else if (FindControl("txtAmbProcIns", "input") != null && FindControl("hdfAmbIdProcesso", "input") != null)
            {
                FindControl("txtAmbProcIns", "input").value = "";
                FindControl("hdfAmbIdProcesso", "input").value = "";
            }

            setApl("", "");

            alert(verificaEtiquetaProc.error.description);
            return false;
        }

        if (!var_ProcAmbiente && FindControl("txtProcIns", "input") != null)
        {
            FindControl("txtProcIns", "input").value = codInterno;
            FindControl("hdfIdProcesso", "input").value = idProcesso;
            
            if (FindControl("txtCodProdIns", "input") != null)
                codInternoProd = FindControl("txtCodProdIns", "input").value;
            else
                codInternoProd = FindControl("lblCodProdIns", "span").innerHTML;
                
            codAplicacaoAtual = FindControl("txtAplIns", "input").value;
        }
        else if (FindControl("txtAmbProcIns", "input") != null && FindControl("hdfAmbIdProcesso", "input") != null)
        {
            FindControl("txtAmbProcIns", "input").value = codInterno;
            FindControl("hdfAmbIdProcesso", "input").value = idProcesso;
            
            codInternoProd = FindControl("txtCodAmb", "input").value;
            codAplicacaoAtual = FindControl("txtAmbAplIns", "input").value;
        }
        
        if (((codAplicacao && codAplicacao != "") ||
            (codInternoProd != "" && CadPedido.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) &&
            (codAplicacaoAtual == null || codAplicacaoAtual == ""))
        {
            var_AplAmbiente = var_ProcAmbiente;
            loadApl(codAplicacao);
        }
        
        var_ProcAmbiente = false;
    }

    function loadProc(codInterno) {
        if (codInterno == "") {
            setProc("", "", "");
            return false;
        }

        try {
            var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Processo. Ajax Error.");
                setProc("", "");
                return false
            }

            response = response.split("\t");
            
            if (response[0] == "Erro") {
                alert(response[1]);
                setProc("", "", "");
                return false;
            }

            setProc(response[1], response[2], response[3]);
        }
        catch (err) {
            alert(err);
        }
    }

    // Controla a visibilidade da forma de pagto, escondendo quando
    // o pedido for a vista e exibindo quando o pedido for a prazo
    function formaPagtoVisibility() {
        var control = FindControl("drpTipoVenda", "select");
        var formaPagto = FindControl("drpFormaPagto", "select");
        var parcela = FindControl("drpParcelas", "select");

        if (control == null || formaPagto == null)
        {
            return;
        }
                    
        // Se for à vista e o controle de desconto por forma de pagamento estiver habilitado, esconde somente a parcela.
        if (config_UsarControleDescontoFormaPagamentoDadosProduto && control.value == 1)
        {
            formaPagto.style.display = "";

            if (parcela != null)
            {
                parcela.selectedIndex = 0;
                parcela.style.display = "none";
            }
        }
        // Se for obra, à vista, funcionário ou se estiver vazio, esconde a forma de pagamento e a parcela.
        else if (control.value == 0 || control.value == 1 || control.value == 5 || control.value == 6)
        {
            formaPagto.selectedIndex = 0;
            formaPagto.style.display = "none";

            if (parcela != null)
            {
                parcela.selectedIndex = 0;
                parcela.style.display = "none";
            }
        }
        else
        {
            formaPagto.style.display = "";

            if (parcela != null)
            {
                parcela.style.display = "";
            }
        }
    }
    
    function exibirEntrada(tipoVenda)
    {
        return tipoVenda == "" || tipoVenda == 2 || (tipoVenda == 1 && config_LiberarPedido);
    }

    // Evento acionado ao trocar o tipo de venda (à vista/à prazo)
    function tipoVendaChange(control, calcParcelas) {
        if (control == null)
        {
            return;
        }

        formaPagtoVisibility();

        // Ao alterar o tipo de venda, as formas de pagamento devem ser recarregadas para que o controle de desconto por forma de pagamento e dados do produto funcione corretamente.
        if (config_UsarControleDescontoFormaPagamentoDadosProduto)
        {
            atualizaFormasPagtoCli();
        }

        formaPagtoChanged();
        
        document.getElementById("divObra").style.display = parseInt(control.value) == 5 ? "" : "none";
        document.getElementById("funcionarioComprador").style.display = parseInt(control.value) == 6 ? "" : "none";
        
        var valorEntrada = document.getElementById("tdValorEntrada2").getElementsByTagName("input")[0];
        
        if (!exibirEntrada(control.value)){
            valorEntrada.style.display = "none";
            
            if (FindControl("ctrValEntrada_txtNumber", "input") != null)
                FindControl("ctrValEntrada_txtNumber", "input").value = "";
        }
        else
            valorEntrada.style.display = "";            
        
        if (parseInt(control.value) != 6)
            FindControl("drpFuncVenda", "select").value = "";
        
        if (document.getElementById("divNumParc") != null)
            document.getElementById("divNumParc").style.display = parseInt(control.value) == 2 ? "" : "none";
            
        setParcelas(!var_Loading && calcParcelas);
        if (document.getElementById(var_NomeControleParcelas + "_tblParcelas") != null)
            Parc_visibilidadeParcelas(var_NomeControleParcelas);
        
        var exibirDesconto = !config_DescontoApenasAVista || control.value == 1;
        
        showHideDesconto(exibirDesconto);
    }

    function verificarDescontoFormaPagtoDadosProduto()
    {
        var tipoVenda = FindControl("drpTipoVenda", "select");
        var formaPagto = FindControl("drpFormaPagto", "select");
        var tipoCartao = FindControl("drpTipoCartao", "select");
        var parcelas = FindControl("drpParcelas", "select");

        var retDesconto = CadPedido.VerificaDescontoFormaPagtoDadosProduto(var_IdPedido, tipoVenda != null ? tipoVenda.value : "", formaPagto != null ? formaPagto.value : "",
            tipoCartao != null ? tipoCartao.value : "", parcelas != null ? parcelas.value : "");

        if (retDesconto.error != null)
        {
            alert(retDesconto.error.description);
            return false;
        }
        else if (retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "")
        {
            var txtDesconto = FindControl("txtDesconto","input");
            var txtTipoDesconto = FindControl("drpTipoDesconto","select");

            if (txtTipoDesconto != null)
            {
                txtTipoDesconto.value = 1;
            }

            if (txtDesconto != null)
            {
                txtDesconto.value = retDesconto.value.replace(".", ",");
                txtDesconto.onchange();
                txtDesconto.onblur();
            }
        }
    }
    
    function showHideDesconto(exibirDesconto)
    {
        var drpTipoDesconto = FindControl("drpTipoDesconto", "select");
        if (drpTipoDesconto == null)
            return;
        
        var txtDesconto = FindControl("txtDesconto", "input");
        var lblDescontoVista = FindControl("lblDescontoVista", "span");
        
        drpTipoDesconto.style.display = exibirDesconto ? "" : "none";
        txtDesconto.style.display = exibirDesconto ? "" : "none";
        lblDescontoVista.style.display = !exibirDesconto ? "" : "none";
        
        txtDesconto.onchange();    
    }
    
    function callbackSetParcelas()
    {
        setParcelas(true);
        if (document.getElementById(var_NomeControleParcelas + "_tblParcelas") != null)
            Parc_visibilidadeParcelas(var_NomeControleParcelas);
            
        // Verifica se a empresa permite desconto para pedidos à vista com uma parcela
        if (config_PermitirDescontoAVistaComUmaParcela)
            showHideDesconto(FindControl("hdfNumParcelas", "input").value == "1" || FindControl("drpTipoVenda", "select").value == "1");
    }
    
    function setParcelas(calcParcelas)
    {        
        if (document.getElementById(var_NomeControleParcelas + "_tblParcelas") == null)
            return;
        
        var drpTipoVenda = FindControl("drpTipoVenda", "select");
        
        if (drpTipoVenda == null)
            return;
        
        if (FindControl("hdfExibirParcela", "input") != null)
            FindControl("hdfExibirParcela", "input").value = drpTipoVenda.value == 2;
            
        FindControl("hdfCalcularParcela", "input").value = (calcParcelas == false ? false : true).toString();
    }

    // Evento acionado quando a forma de pagamento é alterada
    function formaPagtoChanged()
    {
        var formaPagto = FindControl("drpFormaPagto", "select");
        var tipoCartao = FindControl("drpTipoCartao", "select");

        if (formaPagto == null)
        {
            return true;
        }

        if (tipoCartao != null)
        {
            // Caso a forma de pagamento atual não seja Cartão, esconde o controle de tipo de cartão e desmarca a opção selecionada.
            if (formaPagto.value != var_CodCartao)
            {
                tipoCartao.style.display = "none";
                tipoCartao.selectedIndex = 0;
            }
            else
            {
                tipoCartao.style.display = "";
            }
        }
    }

    /*
    *   Função chamada ao inserir e atualizar pedido
    */
    function validarPedido(controle)
    {
        var tipoPedido = FindControl("drpTipoPedido", "select").value;
        var tipoEntrega = FindControl("ddlTipoEntrega", "select").value;
        var hdfIdObra = FindControl("hdfIdObra", "input");
        var drpTipoVenda = FindControl("drpTipoVenda", "select");
        var dataPedido = FindControl("txtDataPed", "input").value;
        var dataEntrega = FindControl("ctrlDataEntrega_txtData", "input").value;

        // Verifica se o cliente foi selecionado
        if (FindControl("hdfCliente", "input").value == "" ||
            FindControl("hdfCliente", "input").value == null) {
            alert("Informe o cliente.");
            controle.disabled = false;
            return false;
        }
        
        // Verifica se o tipo do pedido foi selecionado
        if (tipoPedido == "" || tipoPedido == "0")
        {
            alert("Selecione o tipo do pedido.");
            controle.disabled = false;
            return false;
        }

        if (drpTipoVenda != null) 
        {
            // Se o tipo venda não for a vista, obra ou funcionário, obriga a selecionar forma de pagto.
            var tipoVenda = parseInt(drpTipoVenda.value);

            if (FindControl("drpFormaPagto", "select") == null || FindControl("drpFormaPagto", "select").value == "")
            {
                // Caso o controle de desconto por forma de pagamento e dados do produto esteja habilitado e o tipo de venda do pedido seja à vista, obriga o usuário a informar a forma de pagamento.
                if (config_UsarControleDescontoFormaPagamentoDadosProduto && tipoVenda == 1)
                {
                    alert("Selecione a forma de pagamento.");
                    controle.disabled = false;
                    return false;
                }
                else if (tipoVenda != 1 && tipoVenda != 5 && tipoVenda != 6)
                {
                    alert("Selecione a forma de pagamento.");
                    controle.disabled = false;
                    return false;
                }
            }

            if (config_UsarControleObraComProduto)
            {
                var tipoVendaAtual = FindControl("hdfTipoVendaAtual", "input");

                if (tipoVendaAtual != null && tipoVendaAtual.value != 5 && tipoVenda == 5 && var_QtdProdutosPedido > 0)
                {
                    alert("Não é possível escolher obra como forma de pagamento se o pedido tiver algum produto cadastrado.");
                    controle.disabled = false;
                    return false;
                }
                else if (tipoVendaAtual != null && tipoVendaAtual.value == 5 && tipoVenda != 5 && var_QtdProdutosPedido > 0)
                {
                    alert("Não é possível que a forma de pagamento do pedido não seja mais obra se houver algum produto cadastrado.");
                    controle.disabled = false;
                    return false;
                }
            }

            if (tipoVenda == 6 && FindControl("drpFuncVenda", "select").value == "")
            {
                alert("Selecione o funcionário comprador.");
                controle.disabled = false;
                return false;
            }

            // Chamado 13192. Um pedido ficou com a forma de pagamento Obra porém não foi selecionada a obra que deveria ser associada ao pedido.
            // Criamos este bloqueio para evitar que isto ocorra novamente.
            if (tipoVenda == 5 && (hdfIdObra == null || hdfIdObra.value == null || hdfIdObra.value == "")) {
                alert('Informe a obra associada ao pedido ou altere o tipo de venda.');
                controle.disabled = false;
                return false;
            }
        }

        // Verifica se a data de entrega foi preenchida
        if (dataEntrega == "") {
            alert("Informe a data de entrega.");
            controle.disabled = false;
            return false;
        }

        // Verifica se a data de entrega é menor que a data do pedido
        if (dataEntrega != "" && firstGreaterThenSec(dataPedido, dataEntrega)) {
            alert("A data da entrega não pode ser menor que a data do pedido.");
            controle.disabled = false;
            return false;
        }

        // Verifica se o tipo de entrega foi selecionado
        if (tipoEntrega == "") 
        {
            alert("Selecione o tipo de entrega.");
            controle.disabled = false;
            return false;
        }
        else if (tipoEntrega != 1) 
        {
            if (FindControl("txtEnderecoObra", "input").value == "") {
                alert("Informe o endereço " + (tipoEntrega == 4 ? "da entrega" : "do local da obra."));
                controle.disabled = false;
                return false;
            }

            if (FindControl("txtBairroObra", "input").value == "") {
                alert("Informe o bairro " + (tipoEntrega == 4 ? "da entrega" : "do local da obra."));
                controle.disabled = false;
                return false;
            }

            if (FindControl("txtCidadeObra", "input").value == "") {
                alert("Informe a cidade " + (tipoEntrega == 4 ? "da entrega" : "do local da obra."));
                controle.disabled = false;
                return false;
            }
        }
        
        // Verifica se a obra pertence ao cliente
        if (hdfIdObra != null && hdfIdObra.value != null && hdfIdObra.value != "") { 
            var obraCliente = CadPedido.IsObraCliente(hdfIdObra.value, FindControl("txtNumCli", "input").value).value;
            if (obraCliente != null && obraCliente.toLowerCase() == "false")
            {
                alert("A obra selecionada não pertence ao cliente selecionado.");
                controle.disabled = false;
                return false;
            }
        }

        return true;
    }

    /*
    *   Habilita campos antes de inserir/atualizar o pedido, para que ao fazer postback os valores dos mesmos sejam enviados para o backend
    */
    function habilitarCamposAposInsercaoAtualizacaoPedido(controle)
    {
        controle.disabled = false;

        if (FindControl("drpLoja", "select"))
            FindControl("drpLoja", "select").disabled = false;

        if (FindControl("drpVendedorIns", "select"))
            FindControl("drpVendedorIns", "select").disabled = false;

        if (FindControl("drpVendedorEdit", "select"))
            FindControl("drpVendedorEdit", "select").disabled = false;

        var cEndereco = FindControl("txtEnderecoObra", "input");
        var cBairro = FindControl("txtBairroObra", "input");
        var cCidade = FindControl("txtCidadeObra", "input");
        var cCep = FindControl("txtCepObra", "input");
            
        if (cEndereco != null)
            cEndereco.disabled = false;
            
        if (cBairro != null)
            cBairro.disabled = false;
            
        if (cCidade != null)
            cCidade.disabled = false;
            
        if (cCep != null)
            cCep.disabled = false;
    }

    function onInsert(controle) {
        controle.disabled = true;

        if (var_Inserting)
        {
            controle.disabled = false;
            return false;
        }

        if (!validarPedido(controle))
            return false;

        var podeInserir = CadPedido.PodeInserir(FindControl("hdfCliente", "input").value).value.split(';');
        if (parseInt(podeInserir[0], 10) > 0)
        {
            var dias = " há pelo menos " + config_NumeroDiasPedidoProntoAtrasado + " dias ";
            var inicio = parseInt(podeInserir[0], 10) > 1 ? "Os pedidos " : "O pedido ";
            var fim = parseInt(podeInserir[0], 10) > 1 ? " estão prontos" + dias + "e ainda não foram liberados" : " está pronto" + dias + "e ainda não foi liberado";
            alert("Não é possível emitir esse pedido. " + inicio + podeInserir[1] + fim + " para o cliente.");
            controle.disabled = false;
            return false;
        }
        
        bloquearPagina();
        desbloquearPagina(false);

        var_Inserting = true;

        habilitarCamposAposInsercaoAtualizacaoPedido(controle);

        return true;
    }

    // Acionado quando o pedido está para ser salvo
    function onUpdate(controle) {
        if (!validarPedido(controle))
            return false;
        
        var drpTipoVenda = FindControl("drpTipoVenda", "select");
        var valorEntrada = document.getElementById("tdValorEntrada2").getElementsByTagName("input")[0];

        if (drpTipoVenda != null) {
            var tipoVenda = parseInt(drpTipoVenda.value);
            
            // Se a forma de pagamento for cartão à prazo, obriga a informar o tipo de cartão
            if (FindControl("drpFormaPagto", "select") != null && FindControl("drpFormaPagto", "select").value == var_CodCartao && FindControl("drpTipoCartao", "select").value == "" &&
                (tipoVenda == 2 || (config_UsarControleDescontoFormaPagamentoDadosProduto && tipoVenda == 1))) {
                alert("Informe o tipo de cartão.");
                return false;
            }
            
            if (!exibirEntrada(tipoVenda))
                valorEntrada.value = "";
        }
        
        // Verifica se o cliente foi alterado
        if (FindControl("hdfClienteAtual", "input") != null)
        {
            var clienteAtual = FindControl("hdfClienteAtual", "input").value;
            var clienteNovo = FindControl("txtNumCli", "input").value;
            var alterar = clienteAtual != clienteNovo ? confirm("O cliente foi alterado no pedido. Deseja atualizar o projeto?") : false;
            FindControl("hdfAlterarProjeto", "input").value = alterar;
        }

        try
        {
            // Verifica o prazo e a urgência do pedido
            if (!verificarDatas())
                return false;
        }
        catch(err)
        {
            alert("Falha ao verificar datas. " + err);
            return false;
        }

        // Verifica forma de pagamento cartão, se não for seta tipo cartao nulo
        var formaPagto = FindControl("drpFormaPagto", "select");
        if (formaPagto && formaPagto.value != var_CodCartao)
            FindControl("drpTipoCartao","select").value = "";

        habilitarCamposAposInsercaoAtualizacaoPedido(controle);

        return true;
    }

    /*
    *   Função chamada ao inserir e atualizar produto no pedido
    */
    function validarProduto()
    {
        if (!validate("produto"))
            return false;

        atualizaValMin();

        var tbConfigVidro = FindControl("tbConfigVidro_", "table");
        var valor = FindControl("txtValorIns", "input").value;
        var qtde = FindControl("txtQtdeIns", "input").value;
        var altura = FindControl("txtAlturaIns", "input").value;
        var idProd = FindControl("hdfIdProd", "input").value;
        var largura = FindControl("txtLarguraIns", "input").value;
        var valMin = FindControl("hdfValMin", "input").value;
        var tipoVenda = FindControl("hdfTipoVenda", "input");
        tipoVenda = tipoVenda != null ? tipoVenda.value : 0;

        var tipoPedido = FindControl("hdfTipoPedido", "input").value;
        var pedidoProducao = FindControl("hdfPedidoProducao", "input").value == "true";
        var subgrupoProdComposto = CadPedido.SubgrupoProdComposto(idProd).value;
        
        if (!pedidoProducao && tipoVenda != 3 && tipoVenda != 4 && 
            (valor == "" || parseFloat(valor.replace(",", ".")) == 0) && !(tipoPedido == 1 && subgrupoProdComposto)) {
            alert("Informe o valor vendido.");
            saveProdClicked = false;
            return false;
        }
        
        if (qtde == "0" || qtde == "") {
            alert("Informe a quantidade.");
            saveProdClicked = false;
            return false;
        }
        
        if (FindControl("txtAlturaIns", "input").disabled == false && (altura == "" || parseFloat(altura.replace(",", ".")) == 0)) {
            alert("Informe a altura.");
            saveProdClicked = false;
            return false;
        }
        
        if (FindControl("txtLarguraIns", "input").disabled == false && largura == "") {
            alert("Informe a largura.");
            saveProdClicked = false;
            return false;
        }

        valMin = new Number(valMin.replace(',', '.'));
        if (!FindControl("txtValorIns", "input").disabled && new Number(valor.replace(',', '.')) < valMin) {
            alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
            saveProdClicked = false;
            return false;
        }

        // Verifica se foi clicado no aplicar na telinha de beneficiamentos
        if (tbConfigVidro != null && FindControl("tbConfigVidro", "table").style.display == "block")
        {
            alert("Aplique as alterações no beneficiamento antes de salvar o item.");
            saveProdClicked = false;
            return false;
        }
        
        if (!obrigarProcApl())
        {
            saveProdClicked = false;
            return false;
        }
        
        if (!validaTamanhoMax())
        {
            saveProdClicked = false;
            return false;
        }
        
        // Calcula o ICMS do produto
        var aliquota = FindControl("hdfAliquotaIcmsProd", "input");
        var icms = FindControl("hdfValorIcmsProd", "input");
        icms.value = aliquota.value > 0 ? parseFloat(valor) * (parseFloat(aliquota.value) / 100) : 0;
        icms.value = icms.value.toString().replace('.', ',');

        // Habilita campos para que seus valores sejam enviados para o backend
        FindControl("txtAlturaIns", "input").disabled = false;
        FindControl("txtLarguraIns", "input").disabled = false;
        FindControl("txtValorIns", "input").disabled = false;

        if (tbConfigVidro != null && FindControl("txtEspessura", "input", tbConfigVidro) != null)
            FindControl("txtEspessura", "input", tbConfigVidro).disabled = false;
                
        var nomeControle = getNomeControleBenef();        

        if(exibirControleBenef(nomeControle))
        {
            var resultadoVerificacaoObrigatoriedade = verificarObrigatoriedadeBeneficiamentos(dadosProduto.ID);
            saveProdClicked = resultadoVerificacaoObrigatoriedade;
            return resultadoVerificacaoObrigatoriedade;
        }
    }

    var saveProdClicked = false;

    // Chamado quando um produto está para ser inserido no pedido
    function onInsertProd() {            
        if (saveProdClicked == true)
            return false;
            
        saveProdClicked = true;

        if (FindControl("txtCodProdIns", "input").value == "") {
            alert("Informe o código do produto.");
            saveProdClicked = false;
            return false;
        }

        if (!validarProduto())
            return false;

        return true;
    }

    // Função chamada quando o produto está para ser atualizado
    function onUpdateProd(idProdPed) {
        if (!validarProduto())
            return false;

        return true;
    }

    // Função chamada ao clicar no botão Em Conferência
    function emConferencia() {
        if (confirm("Mudar pedido para em conferência?") == false)
            return false;

        var entrada = FindControl("ctrValEntrada_txtNumber", "input").value;
        var totalPedido = FindControl("hdfTotal", "input").value;

        if (entrada == 0 || entrada == "" || entrada == "0");
        if (!confirm("O sinal não foi inserido, clique em 'Cancelar' para inserir o sinal do pedido ou em 'OK' para continuar."))
            return false

        if (totalPedido == 0 || totalPedido == "" || totalPedido == "0") {
            alert("O pedido não possui valor total, insira um produto 'Conferência' com o valor total do Pedido.");
            return false;
        }

        return false;
    }
    
    var dadosCalcM2Prod = {
        IdProd: 0,
        Altura: 0,
        Largura: 0,
        Qtde: 0,
        QtdeAmbiente: 0,
        TipoCalc: 0,
        Cliente: 0,
        Redondo: false,
        NumBenef: 0
    };

    // Calcula em tempo real a metragem quadrada do produto
    function calcM2Prod() {
        try {
            var idProd = FindControl("hdfIdProd", "input").value;
            var altura = FindControl("txtAlturaIns", "input").value;
            var largura = FindControl("txtLarguraIns", "input").value;
            
            var qtde = FindControl("txtQtdeIns", "input").value;
            var qtdeAmb = parseInt(FindControl("hdfQtdeAmbiente", "input").value, 10) > 0 ? FindControl("hdfQtdeAmbiente", "input").value : "1";
            var isVidro = FindControl("hdfIsVidro", "input").value == "true";
            var tipoCalc = FindControl("hdfTipoCalc", "input").value;
            
            if (altura == "" || largura == "" || qtde == "" || altura == "0" || (tipoCalc != 2 && tipoCalc != 10 && !config_UsarBenefTodosGrupos)) {
                if (qtde != "" && qtde != "0")
                    calcTotalProd();

                return false;
            }

            var redondo = (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked) || 
                (FindControl("hdfRedondoAmbiente", "input").value.toLowerCase() == "true");  
                 
            if (altura != "" && largura != "" &&
                parseInt(altura) > 0 && parseInt(largura) > 0 &&
                parseInt(altura) != parseInt(largura) && redondo) {
                    alert('O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.');
                                
                    if (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked)
                        FindControl("Redondo_chkSelecao", "input").checked = false;
                
                    FindControl("hdfRedondoAmbiente", "input").value = false;
                                
                    return false;
                }
            
            var numBenef = "";
            
            if (FindControl("Redondo_chkSelecao", "input") != null) {
                numBenef = FindControl("Redondo_chkSelecao", "input").id
                numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
                numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
                numBenef = eval(numBenef).NumeroBeneficiamentos();
            }

            var esp = FindControl("txtEspessura", "input") != null ? FindControl("txtEspessura", "input").value : 0;
            
            // Calcula metro quadrado
            var idCliente = FindControl("hdfIdCliente", "input").value;
            
            if ((idProd != dadosCalcM2Prod.IdProd && idProd > 0) || (altura != dadosCalcM2Prod.Altura && altura > 0) ||
                (largura != dadosCalcM2Prod.Largura) || (qtde != dadosCalcM2Prod.Qtde && qtde > 0) || (qtdeAmb != dadosCalcM2Prod.QtdeAmbiente) ||
                (tipoCalc != dadosCalcM2Prod.TipoCalc && tipoCalc > 0) || (idCliente != dadosCalcM2Prod.Cliente) || (redondo != dadosCalcM2Prod.Redondo) ||
                (numBenef != dadosCalcM2Prod.NumBenef))
            {
                var isPedProducaoCorte = CadPedido.IsPedidoProducaoCorte(var_IdPedido);
                if(isPedProducaoCorte.error != null){
                    alert(isPedProducaoCorte.error.description);
                    return false;
                }

                FindControl("lblTotM2Ins", "span").innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, isPedProducaoCorte.value).value;
                FindControl("hdfTotM2Calc", "input").value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde * qtdeAmb, idProd, redondo, esp, numBenef, isPedProducaoCorte.value).value;
                FindControl("hdfTotM2CalcSemChapa", "input").value = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, isPedProducaoCorte.value).value;
                FindControl("lblTotM2Calc", "span").innerHTML = FindControl("hdfTotM2Calc", "input").value.replace('.', ',');
                
                if (FindControl("hdfTotM2Ins", "input") != null)
                    FindControl("hdfTotM2Ins", "input").value = FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.');
                else if (FindControl("hdfTotM", "input") != null)
                    FindControl("hdfTotM", "input").value = FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.');
                
                dadosCalcM2Prod = {
                    IdProd: idProd,
                    Altura: altura,
                    Largura: largura,
                    Qtde: qtde,
                    QtdeAmbiente: qtdeAmb,
                    TipoCalc: tipoCalc,
                    Cliente: idCliente,
                    Redondo: redondo,
                    NumBenef: numBenef
                };
            }
            
            calcTotalProd();
        }
        catch (err) {
            alert(err);
        }
    }

    // Calcula em tempo real o valor total do produto
    function calcTotalProd() {
        try {

            var valorIns = FindControl("txtValorIns", "input").value;

            if (valorIns == "")
                return;

            var totM2 = FindControl("lblTotM2Ins", "span").innerHTML;
            var totM2Calc = new Number(FindControl("hdfTotM2Calc", "input").value.replace(',', '.')).toFixed(2);
            var total = new Number(valorIns.replace(',', '.')).toFixed(2);
            var qtde = new Number(FindControl("txtQtdeIns", "input").value.replace(',', '.'));
            qtde = qtde * new Number(parseInt(FindControl("hdfQtdeAmbiente", "input").value, 10) > 0 ? FindControl("hdfQtdeAmbiente", "input").value : "1");
            var altura = new Number(FindControl("txtAlturaIns", "input").value.replace(',', '.'));
            var largura = new Number(FindControl("txtLarguraIns", "input").value.replace(',', '.'));
            var tipoCalc = FindControl("hdfTipoCalc", "input").value;
            var m2Minimo = FindControl("hdfM2Minimo", "input").value;
            var alturaBenef = FindControl("drpAltBenef", "select");
            alturaBenef = alturaBenef != null ? alturaBenef.value : "0";
            var larguraBenef = FindControl("drpLargBenef", "select");
            larguraBenef = larguraBenef != null ? larguraBenef.value : "0";
            
            var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
            
            var percDesconto = controleDescQtde.PercDesconto();
            var percDescontoAtual = controleDescQtde.PercDescontoAtual();
            
            var retorno = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, FindControl("txtAlturaIns", "input"), largura, true, tipoCalc, alturaBenef, larguraBenef, percDescontoAtual, percDesconto);
            if (retorno != "")
                FindControl("lblTotalIns", "span").innerHTML = retorno;
        }
        catch (err) {

        }
    }

    function getCli(idCliente)
    {        
        if (idCliente == undefined || idCliente == null || idCliente == "")
            return false;

        FindControl("txtNumCli", "input").value = idCliente;
        
        var retorno = CadPedido.GetCli(idCliente).value.split(';');
        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            FindControl("txtNomeCliente", "input").value = "";
            FindControl("hdfCliente", "input").value = "";
            txtIdCliente.value = "";
            
            if (config_UsarComissionado)
                limparComissionado();
            
            return false;
        }
                
        if (FindControl("hdfCliente", "input").value != idCliente && FindControl("txtDesconto", "input") != null)
            FindControl("txtDesconto", "input").value = "";

        FindControl("txtNomeCliente", "input").value = retorno[1];
        FindControl("hdfCliente", "input").value = idCliente;
        FindControl("lblObsCliente", "span").innerHTML = retorno[3];
        
        var entregaBalcao = CadPedido.RotaBalcao(idCliente).value == "true";

        if (!var_Loading && entregaBalcao && var_TipoEntregaBalcao != null)
            FindControl("ddlTipoEntrega", "select").selectedIndex = var_TipoEntregaBalcao;
                
        PodeConsSitCadContr();
        
        // Limpa endereço de entrega
        if (!var_Loading)
        {
            FindControl("txtEnderecoObra", "input").value = "";
            FindControl("txtBairroObra", "input").value = "";
            FindControl("txtCidadeObra", "input").value = "";
        }
        
        if (!var_Loading)
        {
            if (retorno[5] == "true" && !entregaBalcao)
            {
                if (var_TipoEntregaEntrega != null)
                    FindControl("ddlTipoEntrega", "select").value = var_TipoEntregaEntrega;
                    
                setLocalObra(false);
                getEnderecoCli();
            }
        }
        
        if (config_UsarComissionado)
        {
            var comissionado = MetodosAjax.GetComissionado("", idCliente).value.split(';');
            setComissionado(comissionado[0], comissionado[1], comissionado[2], undefined, true);
        }
        
        if (FindControl("hdfPercSinalMin", "input") != null)
        {
            if (FindControl("hdfCliPagaAntecipado", "input") != null)
                FindControl("hdfCliPagaAntecipado", "input").value = retorno[6];
                
            FindControl("hdfPercSinalMin", "input").value = retorno[7];
        }
        
        if (!var_Loading)
            FindControl("drpVendedor", "select").value = retorno[8];
        
        if (config_UsarComissionado && retorno[9] != "")
            setComissionado(retorno[9], retorno[10], retorno[11]);
         
        if (config_UsarComissaoPorPedido && retorno[12] != "")
            FindControl("hdfPercentualComissao", "input").value = retorno[12];
        else
            FindControl("hdfPercentualComissao", "input").value = "0";
        
        if (FindControl("hdfClienteAtual", "input") != null)
        {
            var clienteAtual = parseInt(FindControl("hdfClienteAtual", "input").value, 10);
            var clienteNovo = parseInt(FindControl("txtNumCli", "input").value, 10);
            if (retorno[14] != "" && clienteAtual != clienteNovo)
                FindControl("drpTransportador", "select").value = retorno[14]; 
        }

        if (!var_Loading)
        {
            if (retorno.length > 13 && retorno[13] != "")
            {
                FindControl("drpLoja", "select").value = retorno[13];
                FindControl("drpLoja", "select").disabled = !config_AlterarLojaPedido;

                if (FindControl("chkDeveTransferir", "input") != null)
                {
                    FindControl("chkDeveTransferir", "input").checked = true;                    
                    FindControl("chkDeveTransferir", "input").disabled = true;
                }
            }
            else if (FindControl("drpLoja", "select") != null)
            {                
                if (FindControl("chkDeveTransferir", "input") != null)
                {
                    FindControl("drpLoja", "select").disabled = false;
                    FindControl("chkDeveTransferir", "input").checked = false;
                    FindControl("chkDeveTransferir", "input").disabled = false;
                }
            }
        }
        
        // É muito importante que o método atualizaTipoVendaCli seja chamado antes do método atualizaFormasPagtoCli, pois as formas de pagamento são recuperadas com base no tipo de venda do pedido.
        // OBS.: o método atualizaTipoVendaCli está sendo chamado dentro do método atualizaTipoVendaCli.
        atualizaTipoVendaCli();
        alteraDataEntrega(true);
    }

    // Habilita/Desabilita campos referente ao local da obra
    function setLocalObra(forcarAlteracaoDataEntrega) {
        var cTipoEntrega = FindControl("ddlTipoEntrega", "select");

        if (!cTipoEntrega)
            return false;

        var disable = cTipoEntrega.value != 2 && cTipoEntrega.value != 3 && cTipoEntrega.value != 4 && cTipoEntrega.value != 5 && cTipoEntrega.value != 6;
        
        var cEndereco = FindControl("txtEnderecoObra", "input");
        var cBairro = FindControl("txtBairroObra", "input");
        var cCidade = FindControl("txtCidadeObra", "input");
        var cCep = FindControl("txtCepObra", "input");

        // Se os campos estiverem sendo desabilitados, apaga seus valores
        if (disable) {
            cEndereco.value = "";
            cBairro.value = "";
            cCidade.value = "";
            if (cCep != null) cCep.value = "";
        }
        
        // Habilita ou desabilita os campos
        cEndereco.disabled = disable;
        cBairro.disabled = disable;
        cCidade.disabled = disable;
        if (cCep != null) cCep.disabled = disable;
        
        // Se os campos estiverem habilitados, busca o endereço do cliente como endereço de entrega
        if (config_BuscarEnderecoClienteSeEstiverVazio && cEndereco.value == "" && cBairro.value == "" && cCidade.value == "") 
            getEnderecoCli();
        
        alteraDataEntrega(forcarAlteracaoDataEntrega);
    }

    // Busca o endereço do cliente
    function getEnderecoCli() {
        if (FindControl("txtEnderecoObra", "input").disabled)
            return false;

        var idCli = FindControl("hdfCliente", "input").value;

        if (idCli == "") {
            if (!var_Loading)
                alert("Selecione um cliente primeiro.");
            return false;
        }

        var retorno = MetodosAjax.GetEnderecoCli(idCli).value;

        if (retorno != null && retorno != "") {
            retorno = retorno.split('|');
            FindControl("txtEnderecoObra", "input").value = retorno[0];
            FindControl("txtBairroObra", "input").value = retorno[1];
            FindControl("txtCidadeObra", "input").value = retorno[2];
            if (FindControl("txtCepObra", "input") != null) 
                FindControl("txtCepObra", "input").value = retorno[3];
        }
    }

    function setComissionado(id, nome, percentual, edicaoComissionado, forcarCarregamentoComissionado) {
        forcarCarregamentoComissionado = forcarCarregamentoComissionado != undefined && forcarCarregamentoComissionado != null && forcarCarregamentoComissionado != "" ? forcarCarregamentoComissionado : false;
        var campoPercentual = FindControl("txtPercentual", "input").value;
        var idComissinado = FindControl("hdfIdComissionado", "input").value;        
        var possuiComissionado = CadPedido.IdComissionadoPedido(var_IdPedido).value;       
       
        if (forcarCarregamentoComissionado || (possuiComissionado == "true" && edicaoComissionado != undefined))
        {            
            FindControl("lblComissionado", "span").innerHTML = nome;
            FindControl("hdfIdComissionado", "input").value = id;
            FindControl("txtPercentual", "input").value = percentual;            
        }
        else if (var_IdPedido == "" || edicaoComissionado != undefined)
        {
            FindControl("lblComissionado", "span").innerHTML = nome;
            FindControl("hdfIdComissionado", "input").value = id;  
            FindControl("txtPercentual", "input").value = percentual;
        }        

        if (!forcarCarregamentoComissionado && campoPercentual != percentual && var_IdPedido != "" && edicaoComissionado == undefined)
            FindControl("txtPercentual", "input").value = campoPercentual;
        else
            FindControl("txtPercentual", "input").value = percentual;     
    }

    // Função chamada para mostrar/esconder controles para inserção de novo ambiente
    function addAmbiente(value) {
        var ambiente = FindControl("txtAmbiente", "input");
        if (ambiente == null)
            ambiente = FindControl("ambMaoObra", "div");
        
        var descricao = FindControl("txtDescricao", "textarea");
        if (ambiente == null && descricao == null)
            return;
    
        if (descricao != null)
            descricao.style.display = value ? "" : "none";
        
        if (ambiente != null)
            ambiente.style.display = value ? "" : "none";
        
        var qtde = FindControl("txtQtdeAmbiente", "input");
        var altura = FindControl("txtAlturaAmbiente", "input");
        var largura = FindControl("txtLarguraAmbiente", "input");
        var redondo = FindControl("chkRedondoAmbiente", "input");
        var apl = FindControl("txtAmbAplIns", "input");
        apl = apl != null ? apl.parentNode.parentNode.parentNode : null;
        var proc = FindControl("txtAmbProcIns", "input");
        proc = proc != null ? proc.parentNode.parentNode.parentNode : null;
        
        if (qtde != null)
            qtde.style.display = value ? "" : "none";
            
        if (altura != null)
            altura.style.display = value ? "" : "none";
            
        if (largura != null)
            largura.style.display = value ? "" : "none";
        
        if (redondo != null) {
            if (value) {
                redondo.style.display = "";
                                
                if (altura.value != "" && largura != "" &&
                    altura.value != largura.value &&
                    redondo.checked) {
                        alert('O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.');
                        redondo.checked = false;
                    }
            }
            else
                redondo.style.display = "none";
        }
        
        if (apl != null)
            apl.style.display = value ? "" : "none";
        
        if (proc != null)
            proc.style.display = value ? "" : "none";
        
        FindControl("lnkInsAmbiente", "a").style.display = value ? "" : "none";
    }
    
    // Função chamada ao finalizar o pedido
    function finalizarPedido()
    {
        if (confirm("Finalizar pedido?"))
            return verificarDatas();
        
        return false;
    }
    
    // Função chamada para verificar o prazo de entrega e a urgência do pedido
    function verificarDatas()
    {
        // Verifica a data de entrega
        var dataEntrega = FindControl("ctrlDataEntrega_txtData", "input");

        if (FindControl("lblDataEntrega", "span") != null && FindControl("lblDataEntrega", "span").innerHTML == "")
        {
            alert("Informe a data de entrega do pedido");
            return false;
        }

        if (!verificaDataEntrega(dataEntrega))
            return false;
        
        var pedidoFastDelivery = null;
        
        // Verifica se o pedido é Fast Delivery
        if (config_FastDelivery)
        {
            pedidoFastDelivery = FindControl("hdfFastDelivery", "input");
            if (pedidoFastDelivery != null)
                pedidoFastDelivery = pedidoFastDelivery.value.toLowerCase() == "true";
            else
            {
                pedidoFastDelivery = FindControl("chkFastDelivery", "input");
                if (pedidoFastDelivery != null)
                    pedidoFastDelivery = pedidoFastDelivery.checked;
                else
                    pedidoFastDelivery = false;
            }
        }
        else
            pedidoFastDelivery = false;
        
        // Só testa o Fast Delivery e o Máximo de Vendas se o pedido não for Têmpera fora
        if (pedidoFastDelivery && config_FastDelivery && !checkFastDelivery())
            return false;

        if (!checkPosMateriaPrima())
            return false;
            
        return checkCapacidadeProducaoSetor();
    }

    function checkPosMateriaPrima()
    {
        var result = CadPedido.VerificaPosMateriaPrima(var_IdPedido);

        if(result.error != null){
            alert(result.error.description);
            return true;
        }

        if(result.value.split(';')[0] == "erro"){
            alert(result.value.split(';')[1]);
            return config_BloqEmisPedidoPorPosicaoMateriaPrima;
        }

        return true;
    }
    
    function checkCapacidadeProducaoSetor()
    {
        var editPedido = FindControl("grdProdutos", "table") == null;
        
        var totM2 = parseFloat(var_TotalM2Pedido);
        var dataEntrega = editPedido ? 
            (FindControl("ctrlDataEntrega_txtData", "input") == null ? FindControl("lblDataEntrega", "span").innerHTML : FindControl("ctrlDataEntrega_txtData", "input").value) :
            (FindControl("lblDataEntrega", "span") == null ? FindControl("ctrlDataEntrega_txtData", "input").value : FindControl("lblDataEntrega", "span").innerHTML);
        dataEntrega = dataEntrega.toString().split(' ')[0];
        var idProcesso = FindControl("hdfIdProcesso", "input") != null ? FindControl("hdfIdProcesso", "input").value : 0;
        
        if (!editPedido)
        {
            if (FindControl("drpFooterVisible", "select") != null)
                var diferencaM2 = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.'));
            else
            {
                var totM2Produto = FindControl("hdfTotM", "input") != null ? parseFloat(FindControl("hdfTotM", "input").value.replace(',', '.')) : 0;
                var novoTotM2Produto = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.'));
                var diferencaM2 = novoTotM2Produto - totM2Produto;
            }
        }
        else
            var diferencaM2 = 0;
        
        var codInternoProd = !editPedido ? FindControl("txtCodProdIns", "input") : null;
        if (codInternoProd == null)
            codInternoProd = !editPedido ? FindControl("lblCodProdIns", "span") : null;
        
        if (codInternoProd != null)
            codInternoProd = codInternoProd.nodeName.toLowerCase() == "input" ? codInternoProd.value : codInternoProd.innerHTML;
        else
            codInternoProd = "";
        
        if (isNaN(diferencaM2) || (!editPedido && !CadPedido.UsarDiferencaM2Prod(codInternoProd).value))
            diferencaM2 = 0;
        
        var resposta = CadPedido.VerificarProducaoSetor(var_IdPedido, dataEntrega, diferencaM2, idProcesso).value;
        var dadosResposta = resposta.split("|");
        
        if (dadosResposta[0] == "Erro")
        {
            alert(dadosResposta[1]);
            return false;
        }
        
        return true;
    }
    
    // Função chamada para verificar se há Fast Delivery.
    function checkFastDelivery()
    {        
        var editPedido = false;
        
        var fastDelivery = FindControl("hdfFastDelivery", "input");
        if (fastDelivery == null || fastDelivery.value.toLowerCase() == "false")
        {
            var fastDelivery = FindControl("chkFastDelivery", "input");
            if (fastDelivery == null || !fastDelivery.checked)
                return true;
            else    
                editPedido = true;
        }
            
        var totM2 = parseFloat(var_TotalM2Pedido);
        var dataPedido = var_DataPedido;

        if (!editPedido)
        {
            if (FindControl("drpFooterVisible", "select") != null)
                diferencaM2 = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.'));
            else
            {
                var totM2Produto = FindControl("hdfTotM", "input") != null ? parseFloat(FindControl("hdfTotM", "input").value.replace(',', '.')) : 0;
                var novoTotM2Produto = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.'));
                diferencaM2 = novoTotM2Produto - totM2Produto;
            }
        }
            
        var codInternoProd = !editPedido ? FindControl("txtCodProdIns", "input") : null;
        if (codInternoProd == null)
            codInternoProd = !editPedido ? FindControl("lblCodProdIns", "span") : null;
        
        if (codInternoProd != null)
            codInternoProd = codInternoProd.nodeName.toLowerCase() == "input" ? codInternoProd.value : codInternoProd.innerHTML;
        else
            codInternoProd = "";
        
        return true;
    }
    
    // Função utilizada após selecionar medidor no popup, para preencher o id e o nome do mesmo
    // Nas respectivas textboxes deste form
    function setMedidor(id, nome) {
        FindControl("hdfIdMedidor", "input").value = id;
        FindControl("lblMedidor", "span").innerHTML = nome;
        return false;
    }
    
    function openProjeto(idAmbiente)
    {
        var tipoEntrega = FindControl("ddlTipoEntrega", "select");
        if (tipoEntrega != null)
            tipoEntrega = tipoEntrega.value;
        else
            tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
        
        if (tipoEntrega == "")
        {
            alert("Selecione o tipo de entrega antes de inserir um projeto.");
            return false;
        }
        
        var idCliente = FindControl("hdfIdCliente", "input").value;
        
        openWindow(screen.height, screen.width, '../Cadastros/Projeto/CadProjetoAvulso.aspx?IdPedido=' + var_IdPedido +
            "&IdAmbientePedido=" + idAmbiente + "&idCliente=" + idCliente + "&TipoEntrega=" + tipoEntrega);
            
        return false;
    }
    
    function refreshPage() {
        atualizarPagina();
    }
    
    function PodeConsSitCadContr(){
         var idCli = FindControl("hdfCliente", "input").value;

        if (idCli == "" || CadPedido.PodeConsultarCadastro(idCli).value == "False")
            FindControl("ConsultaCadCliSintegra", "div").style.display = 'none';
        else
            FindControl("ConsultaCadCliSintegra", "div").style.display = 'inline';
    }
    
    function AlterouLoja(){
        var idLoja = CadPedido.GetLojaFuncionario().value;
        
        if (FindControl("chkDeveTransferir", "input") != null)
        {
            if(FindControl("drpLoja", "select").value != idLoja)
                FindControl("chkDeveTransferir","input").checked = true;
            else
                FindControl("chkDeveTransferir","input").checked = false;
        }
    }

    function buscarProcessos(){
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProd", "input").value);
        openWindow(450, 700, "../Utils/SelEtiquetaProcesso.aspx?idSubgrupo=" + idSubgrupo.value);
    }

    function exibirProdsComposicao(botao, idProdPed) {

        var grdProds = FindControl("grdProdutos", "table");

        if(grdProds == null)
            return;

        for (var i = 0; i < grdProds.rows.length; i++) {

            var row = grdProds.rows[i];
            if(row.id.indexOf("prodPed_") != -1 && row.id.split('_')[1] != idProdPed){
                row.style.display = "none";
            }
        }

        var linha = document.getElementById("prodPed_" + idProdPed);
        var exibir = linha.style.display == "none";
        linha.style.display = exibir ? "" : "none";
        botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
        botao.title = (exibir ? "Esconder" : "Exibir") + " Produtos da Composição";

        if(FindControl("txtCodProdIns","input") != null)
            FindControl("txtCodProdIns","input").parentElement.parentElement.style.display = !exibir ? "" : "none";

        FindControl("hdfProdPedComposicaoSelecionado", "input").value = exibir? idProdPed : 0;
    }

    function exibirObs(num, botao) {
        for (iTip = 0; iTip < 2; iTip++) {
            TagToTip('tbObsCalc_' + num, FADEIN, 300, COPYCONTENT, false, TITLE, 'Observação', CLOSEBTN, true,
                CLOSEBTNTEXT, 'Fechar (Não salva as alterações)', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, false,
                FIX, [botao, 9 - getTableWidth('tbObsCalc_' + num), 7]);
        }
    }

    function setCalcObs(idItemProjeto, button) {
        var obs = button.parentNode.parentNode.parentNode.getElementsByTagName('textarea')[0].value;

        var retorno = CadPedido.SalvaObsProdutoPedido(idItemProjeto, obs).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            return false;
        }
        else {
            alert("Observação salva.");
            window.opener.refreshPage();
        }
    }

    function iniciaPesquisaCepObra(cep)
    {
        var logradouro = FindControl("txtEnderecoObra", "input");
        var bairro = FindControl("txtBairroObra", "input");
        var cidade = FindControl("txtCidadeObra", "input");
        pesquisarCep(cep, null, logradouro, bairro, cidade, null);
    }

    function modificarLayoutGridProdutos()
    {
        // Se a empressa não vende vidros, esconde campos
        if (FindControl("hdfNaoVendeVidro", "input").value == "true" && FindControl("grdProdutos", "table") != null)
        {
            var tbProd = FindControl("grdProdutos", "table");
            var rows = tbProd.rows;
        
            var colsTitle = rows[0].getElementsByTagName("th");
            colsTitle[4].style.display = "none";
            colsTitle[5].style.display = "none";
            colsTitle[6].style.display = "none";
            colsTitle[7].style.display = "none";
        
            var k=0;
            for (k=1; k<rows.length; k++) {
                if (rows[k].cells.length <= 2)
                    continue;
                
                if (rows[k].cells[4] == null)
                    break;
                
                rows[k].cells[4].style.display = "none";
                rows[k].cells[5].style.display = "none";
                rows[k].cells[6].style.display = "none";
                rows[k].cells[7].style.display = "none";
            }
        }
        else {
            // Troca a posição da altura com a largura
            if (config_UsarAltLarg && FindControl("grdProdutos", "table") != null) {
                var tbProd = FindControl("grdProdutos", "table");
                var rows = tbProd.children[0].children;
            
                // Troca a label de título altura-largura
                var colsTitle = rows[0].getElementsByTagName("th");
                var colAltInnerHtml = colsTitle[4].innerHTML;
                colsTitle[4].innerHTML = colsTitle[5].innerHTML;
                colsTitle[5].innerHTML = colAltInnerHtml;
            
                var j=0;
                for (j=1; j<rows.length; j++) {
                    try
                    {
                        var cols = rows[j].getElementsByTagName("td");
                        var colTemp = rows[j].cells[4].innerHTML;
                        rows[j].cells[4].innerHTML = rows[j].cells[5].innerHTML;
                        rows[j].cells[5].innerHTML = colTemp;
                    }
                    catch (err)
                    { }
                }
            }
        }
    }

    </script>

    <table id="mainTable" runat="server" clientidmode="Static" style="width: 100%">
        <tr>
            <td>
                <table style="width: 100%">
                    <tr>
                        <td align="center">
                            <asp:DetailsView ID="dtvPedido" runat="server" AutoGenerateRows="False" DataSourceID="odsPedido"
                                DefaultMode="Insert" GridLines="None" Height="50px" Width="125px">
                                <Fields>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <table cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cliente
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow">
                                                        <span style="white-space: nowrap">
                                                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeydown="if (isEnter(event)) getCli(this);"
                                                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this.value);" ReadOnly='<%# !(bool)Eval("ClienteEnabled") %>'
                                                                Text='<%# Eval("IdCli") %>'></asp:TextBox>
                                                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Text='<%# Eval("NomeCli") %>'
                                                                Width="250px"></asp:TextBox>
                                                            <asp:LinkButton ID="lnkSelCliente" runat="server" Visible='<%# Eval("ClienteEnabled") %>'
                                                                OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=pedido'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>&nbsp;&nbsp;
                                                            <div id="ConsultaCadCliSintegra" style="display: none">
                                                                <uc9:ctrlConsultaCadCliSintegra runat="server" ID="ctrlConsultaCadCliSintegra1" OnLoad="ctrlConsultaCadCliSintegra1_Load" />
                                                            </div>
                                                            <asp:HiddenField ID="hdfPercentualComissao" runat="server" Value='<%# Bind("PercentualComissao") %>' />
                                                        </span>
                                                        <br />
                                                        <asp:Label ID="lblObsCliente" runat="server" ForeColor="<%# GetCorObsCliente() %>"></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Ped.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataPed" runat="server" ReadOnly="True" Text='<%# Eval("DataPedidoString") %>'
                                                            Width="70px"></asp:TextBox>
                                                        <asp:CheckBox ID="chkFastDelivery" runat="server" Checked='<%# Bind("FastDelivery") %>'
                                                            OnLoad="FastDelivery_Load" Text="Fast delivery" onclick="alteraFastDelivery(this.checked)" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cód. Ped. Cli.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtCodPedCli" runat="server" MaxLength="30" Text='<%# Bind("CodCliente") %>'
                                                            onchange="verificaPedCli();" ReadOnly='<%# Importado() %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Orcamento
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIdOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("IdOrcamento") %>' Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label14" runat="server" Text="Loja" OnLoad="Loja_Load"></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <uc11:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="false" MostrarTodas="false"
                                                                        SelectedValue='<%# Bind("IdLoja") %>' OnLoad="Loja_Load" OnChange="AlterouLoja();" />
                                                                </td>
                                                                <td>
                                                                    <asp:CheckBox ID="chkDeveTransferir" runat="server" Text="Deve Transferir?" Checked='<%# Bind("DeveTransferir") %>'
                                                                        OnLoad="Loja_Load" ForeColor="Red" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        &nbsp;
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdTipoVenda1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Venda
                                                    </td>
                                                    <td id="tdTipoVenda2" align="left" nowrap="nowrap" valign="middle">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoVenda" runat="server" SelectedValue='<%# Bind("TipoVenda") %>'
                                                                        onchange="tipoVendaChange(this, true);" onblur="verificarDescontoFormaPagtoDadosProduto();" Enabled='<%# !(bool)Eval("RecebeuSinal") || (bool)Glass.Configuracoes.PedidoConfig.LiberarPedido %>'
                                                                        DataSourceID="odsTipoVenda" DataTextField="Descr" DataValueField="Id">
                                                                    </asp:DropDownList>
                                                                    <div id="divObra" style="display: none">
                                                                        <asp:TextBox ID="txtObra" runat="server" Enabled="false" Width="200px" Text='<%# Eval("DescrObra") %>'></asp:TextBox>
                                                                        <asp:ImageButton ID="imbObra" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick='<%# "if (podeSelecionarObra()) openWindow(560, 650, \"../Utils/SelObra.aspx?situacao=4&tipo=1&idsPedidosIgnorar=" + Request["idPedido"] + "&idCliente=\" + FindControl(\"txtNumCli\", \"input\").value); return false;" %>' />
                                                                        <br />
                                                                        <asp:Label ID="lblSaldoObra" runat="server" Text='<%# Eval("DescrSaldoObraPedidos") %>'></asp:Label>
                                                                        <asp:HiddenField ID="hdfIdObra" runat="server" Value='<%# Bind("IdObra") %>' />
                                                                    </div>
                                                                </td>
                                                                <td>
                                                                    <div id="divNumParc">
                                                                        <table>
                                                                            <tr>
                                                                                <td nowrap="nowrap" style="font-weight: bold">
                                                                                    Num Parc.:
                                                                                </td>
                                                                                <td nowrap="nowrap">
                                                                                    <uc6:ctrlParcelasSelecionar ID="ctrlParcelasSelecionar1" runat="server" ParcelaPadrao='<%# Bind("IdParcela") %>'
                                                                                        NumeroParcelas='<%# Bind("NumParc") %>' OnLoad="ctrlParcelasSelecionar1_Load"
                                                                                        CallbackSelecaoParcelas="callbackSetParcelas"/>
                                                                                    <asp:HiddenField ID="hdfDataBase" runat="server" OnLoad="hdfDataBase_Load" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table cellpadding="0" cellspacing="0" id="funcionarioComprador" style='<%# ((bool)Eval("VendidoFuncionario")) ? "": "display: none; " %>padding-top: 2px'>
                                                            <tr>
                                                                <td>
                                                                    Funcionário comp.:&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFuncVenda" runat="server" DataSourceID="odsFuncVenda" DataTextField="Nome"
                                                                        DataValueField="IdFunc" AppendDataBoundItems="True" SelectedValue='<%# Bind("IdFuncVenda") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:HiddenField ID="hdfTipoVendaAtual" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                                    </td>
                                                    <td id="tdTipoEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Entrega
                                                    </td>
                                                    <td id="tdTipoEntrega2" align="left" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                <asp:DropDownList ID="ddlTipoEntrega" runat="server" SelectedValue='<%# Bind("TipoEntrega") %>'
                                                                        onchange="setLocalObra(true);" AppendDataBoundItems="True" DataSourceID="odsTipoEntrega"
                                                                        DataTextField="Descr" DataValueField="Id" OnLoad="ddlTipoEntrega_Load" OnDataBound="ddlTipoEntrega_DataBound">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                
                                                                    
                                                                    &nbsp;
                                                                </td>
                                                                <td class="dtvHeader" nowrap="nowrap">
                                                                    Tipo Pedido
                                                                </td>
                                                                <td>
                                                                    <asp:HiddenField ID="hdfPedidoRevenda" runat="server" Value='<%# Bind("IdPedidoRevenda") %>' />
                                                                    <asp:DropDownList ID="drpTipoPedido" runat="server" onchange="alteraDataEntrega(false)" 
                                                                        DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" OnDataBound="drpTipoPedido_DataBound"
                                                                        SelectedValue='<%# Bind("TipoPedido") %>' Enabled='<%# Eval("TipoPedidoEnabled") %>'>
                                                                    </asp:DropDownList>
                                                                    <div id="divGerarPedidoProducaoCorte">
                                                                        <asp:CheckBox ID="chkGerarPedidoProducaoCorte" runat="server"
                                                                           Text="Gerar Pedido de Produção para Corte" Checked='<%# Bind("GerarPedidoProducaoCorte") %>'/>
                                                                    </>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdFormaPagto1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Forma Pagto.
                                                    </td>
                                                    <td id="tdFormaPagto2" align="left" nowrap="nowrap" class="dtvAlternatingRow">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="true" DataSourceID="odsFormaPagto"
                                                                        DataTextField="Descricao" DataValueField="IdFormaPagto" SelectedValue='<%# Bind("IdFormaPagto") %>'
                                                                        onchange="formaPagtoChanged();" onblur="verificarDescontoFormaPagtoDadosProduto();">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoCartao" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCartao"
                                                                        DataTextField="Descricao" DataValueField="IdTipoCartao" SelectedValue='<%# Bind("IdTipoCartao") %>' onblur="verificarDescontoFormaPagtoDadosProduto();">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdDataEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Entrega
                                                    </td>
                                                    <td id="tdDataEntrega2" align="left" nowrap="nowrap" class="dtvAlternatingRow">
                                                         <table>
                                                            <tr>
                                                                <td>
                                                                    <uc8:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadOnly" DataString='<%# Bind("DataEntregaString") %>'
                                                                        ExibirHoras="False" onchange="verificaDataEntrega(this)" OnLoad="ctrlDataEntrega_Load" />
                                                                    <asp:HiddenField ID="hdfDataEntregaNormal" runat="server" />
                                                                    <asp:HiddenField ID="hdfDataEntregaFD" runat="server" />
                                                                </td>
                                                                <td align="left" class="dtvHeader" nowrap="nowrap">
                                                                    <asp:Label runat="server" ID="lblValorFrete" OnLoad="txtValorFrete_Load" Text="Valor do Frete"></asp:Label> 
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox runat="server" ID="txtValorFrete" onkeypress="return soNumeros(event, false, true);" Width="80px" Text='<%# Bind("ValorEntrega") %>'
                                                                        OnLoad="txtValorFrete_Load"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdValorEntrada1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Valor Entrada
                                                    </td>
                                                    <td id="tdValorEntrada2" align="left" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValEntrada" runat="server" Value='<%# Bind("ValorEntrada") %>'
                                                            Visible='<%# !(bool)Eval("RecebeuSinal") %>' />
                                                        <asp:Label ID="lblValor" runat="server" Text='<%# Eval("ConfirmouRecebeuSinal") %>'
                                                            Visible='<%# Eval("RecebeuSinal") %>'></asp:Label>
                                                        <asp:HiddenField ID="hdfValorEntrada" runat="server" Value='<%# Eval("ValorEntrada") %>'
                                                            Visible='<%# Eval("RecebeuSinal") %>' />
                                                    </td>
                                                    <td id="tdDesconto1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Desconto
                                                    </td>
                                                    <td id="tdDesconto2" align="left" nowrap="nowrap" valign="middle">
                                                        <table class="pos" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoDesconto" runat="server" SelectedValue='<%# Bind("TipoDesconto") %>'
                                                                        Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>' onclick="calcularDesconto(1)">
                                                                        <asp:ListItem Value="2">R$</asp:ListItem>
                                                                        <asp:ListItem Value="1">%</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:TextBox ID="txtDesconto" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Enabled='<%# Eval("DescontoEnabled") %>' onchange="calcularDesconto(1)" Text='<%# Bind("Desconto") %>'
                                                                        Width="70px"></asp:TextBox>
                                                                    <asp:Label ID="lblDescontoVista" runat="server" ForeColor="Blue" Text="Desconto só pode ser dado em pedidos à vista"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;
                                                                    <uc7:ctrlDadosDesconto ID="ctrlDadosDesconto" runat="server" TaxaFastDelivery='<%# Glass.Configuracoes.PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery %>'
                                                                        OnLoad="ctrlDadosDesconto_Load" IsPedidoFastDelivery='<%# Eval("FastDelivery") %>' />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:HiddenField ID="hdfDesconto" runat="server" Value='<%# Eval("Desconto") %>' />
                                                        <asp:HiddenField ID="hdfTipoDesconto" runat="server" Value='<%# Eval("TipoDesconto") %>' />
                                                        <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdTotal1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Total
                                                    </td>
                                                    <td id="tdTotal2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtTotal" runat="server" ReadOnly="True" Text='<%# Eval("Total", "{0:C}") %>'></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    &nbsp;Acréscimo&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoAcrescimo" runat="server" SelectedValue='<%# Bind("TipoAcrescimo") %>'
                                                                        Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>'>
                                                                        <asp:ListItem Value="2">R$</asp:ListItem>
                                                                        <asp:ListItem Value="1">%</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:TextBox ID="txtAcrescimo" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Text='<%# Bind("Acrescimo") %>' Width="70px" Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>'></asp:TextBox>
                                                                    <asp:HiddenField ID="hdfAcrescimo" runat="server" Value='<%# Eval("Acrescimo") %>' />
                                                                    <asp:HiddenField ID="hdfTipoAcrescimo" runat="server" Value='<%# Eval("TipoAcrescimo") %>' />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdFuncionario1" align="left" nowrap="nowrap" class="dtvHeader">
                                                        Funcionário
                                                    </td>
                                                    <td id="tdFuncionario2" align="left" nowrap="nowrap" class="dtvAlternatingRow">
                                                        <asp:DropDownList ID="drpVendedorEdit" runat="server" DataSourceID="odsFuncionario" AppendDataBoundItems="true"
                                                            DataTextField="Nome" DataValueField="IdFunc" Enabled='<%# Eval("SelVendEnabled") %>'
                                                            SelectedValue='<%# Bind("IdFunc") %>' OnDataBinding="drpVendedorEdit_DataBinding">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdParcela" align="left" class="dtvHeader" colspan="4" nowrap="nowrap">
                                                        <uc3:ctrlParcelas ID="ctrlParcelas1" runat="server" NumParcelas="4" NumParcelasLinha="6"
                                                            Datas='<%# Bind("DatasParcelas") %>' Valores='<%# Bind("ValoresParcelas") %>'
                                                            OnLoad="ctrlParcelas1_Load" OnDataBinding="ctrlParcelas1_DataBinding" />
                                                        <asp:HiddenField ID="hdfExibirParcela" runat="server" />
                                                        <asp:HiddenField ID="hdfCalcularParcela" runat="server" />
                                                        <asp:HiddenField ID="hdfCliPagaAntecipado" runat="server" Value='<%# Eval("ClientePagaAntecipado") %>' />
                                                        <asp:HiddenField ID="hdfPercSinalMin" runat="server" Value='<%# Eval("PercSinalMinCliente") %>' />
                                                        <asp:HiddenField ID="hdfIdSinal" runat="server" Value='<%# Bind("IdSinal") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr class="dtvHeader">
                                                    <td>
                                                        Transportador
                                                        <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsTransportador" DataTextField="Name" DataValueField="Id"
                                                            SelectedValue='<%# Bind("IdTransportador") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr class="dtvHeader">
                                                    <td nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    Local da Obra&nbsp;
                                                                </td>
                                                                <td>
                                                                    <a href="javascript:getEnderecoCli();">
                                                                        <img src="../Images/home.gif" title="Buscar endereço do cliente" border="0"></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        Endereço
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtEnderecoObra" runat="server" MaxLength="100" disabled="true" onkeydown="if (isEnter(event)) return false;"
                                                            Text='<%# Bind("EnderecoObra") %>' Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Bairro
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtBairroObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("BairroObra") %>' onkeydown="if (isEnter(event)) return false;"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Cidade
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCidadeObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("CidadeObra") %>' onkeydown="if (isEnter(event)) return false;"
                                                            Width="100px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        CEP
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtCepObra" runat="server" MaxLength="9" Text='<%# Bind("CepObra") %>' onkeypress="return soCep(event)"
                                                            onkeyup="return maskCep(event, this);"></asp:TextBox>
                                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="iniciaPesquisaCepObra(FindControl('txtCepObra', 'input').value); return false" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tbComissionado" style="width: 100%" class="dtvHeader" cellpadding="0"
                                                cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    &nbsp;Comissionado:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblComissionado" runat="server" Text='<%# Eval("NomeComissionado") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelComissionado" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelComissionado.aspx'); return false;"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>">
                                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                    <asp:ImageButton ID="imbLimparComissionado" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                                        OnClientClick="limparComissionado(); return false;" ToolTip="Limpar comissionado"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                            <tr>
                                                                <td>
                                                                    Percentual:
                                                                </td>
                                                                <td>
                                                                    &nbsp;
                                                                    <asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Text='<%# Bind("PercComissao") %>' Width="50px" OnLoad="txtPercentual_Load"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                            <tr>
                                                                <td>
                                                                    Valor Comissão:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:TextBox ID="txtValorComissao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        ReadOnly="True" Text='<%# Eval("ValorComissao", "{0:C}") %>' Width="70px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tbMedidor" style="width: 100%" class="dtvHeader" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    &nbsp;Medidor:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblMedidor" runat="server" Text='<%# Eval("NomeMedidor") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelMedidor" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelMedidor.aspx'); return false;">
                                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr>
                                                    <td class="dtvHeader" align="center" colspan="2">
                                                        Observação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:TextBox ID="txtObs" runat="server" MaxLength="1000" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <uc10:ctrlLimiteTexto ID="lmtTxtObs" runat="server" IdControlToValidate="txtObs" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="dtvHeader" align="center" colspan="2" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                        Observação Liberação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                        <asp:TextBox ID="txtObsLiberacao" runat="server" MaxLength="1000" Text='<%# Bind("ObsLiberacao") %>'
                                                            TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfCliente" runat="server" Value='<%# Bind("IdCli") %>' />
                                                        <asp:HiddenField ID="hdfIdComissionado" runat="server" Value='<%# Bind("IdComissionado") %>' />
                                                        <asp:HiddenField ID="hdfIdMedidor" runat="server" Value='<%# Bind("IdMedidor") %>' />
                                                        <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                                        <asp:HiddenField ID="hdfValorComissao" runat="server" Value='<%# Bind("ValorComissao") %>' />
                                                        <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                                                        <asp:HiddenField ID="hdfDataPedido" runat="server" Value='<%# Bind("DataPedidoString") %>' />
                                                        <asp:HiddenField ID="hdfAliquotaIcms" runat="server" Value='<%# Bind("AliquotaIcms") %>' />
                                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsNumParc" runat="server" SelectMethod="GetNumParc"
                                                            TypeName="Glass.Data.Helper.DataSources">
                                                        </colo:VirtualObjectDataSource>
                                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                                                            TypeName="Glass.Data.Helper.DataSources">
                                                        </colo:VirtualObjectDataSource>
                                                    </td>
                                                    <td style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                        <uc10:ctrlLimiteTexto ID="lmtTxtObsLiberacao" runat="server" IdControlToValidate="txtObsLiberacao" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <table cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cliente
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow">
                                                        <span style="white-space: nowrap">
                                                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                                onkeydown="if (isEnter(event)) getCli(this.value);" onblur="getCli(this.value);" />
                                                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Text='<%# Eval("NomeCliente") %>'
                                                                Width="250px"></asp:TextBox>
                                                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=pedido'); return false;">
                                                            <img alt="" border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                            <div id="ConsultaCadCliSintegra" style="display: none">
                                                                <uc9:ctrlConsultaCadCliSintegra runat="server" ID="ctrlConsultaCadCliSintegra1" OnLoad="ctrlConsultaCadCliSintegra1_Load" />
                                                            </div>
                                                        </span>
                                                        <br />
                                                        <asp:Label ID="lblObsCliente" runat="server" Text="" ForeColor="<%# GetCorObsCliente() %>"></asp:Label>
                                                        <asp:HiddenField ID="hdfPercentualComissao" runat="server" Value='<%# Bind("PercentualComissao") %>' />
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Ped.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataPed" runat="server" ReadOnly="True" Width="70px" Text='<%# Eval("DataPedidoString") %>'></asp:TextBox>
                                                        <asp:CheckBox ID="chkFastDelivery" runat="server" Checked='<%# Bind("FastDelivery") %>'
                                                            OnLoad="FastDelivery_Load" Text="Fast delivery" onclick="alteraFastDelivery(this.checked)" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cód. Ped. Cli.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtCodPedCli" runat="server" MaxLength="20" Text='<%# Bind("CodCliente") %>'
                                                            ReadOnly='<%# Importado() %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Orcamento
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIdOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("IdOrcamento") %>' Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label14" runat="server" Text="Loja" OnLoad="Loja_Load"></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <uc11:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="false" OnLoad="Loja_Load"
                                                                        MostrarTodas="false" SelectedValue='<%# Bind("IdLoja") %>' OnChange="AlterouLoja();"/>
                                                                </td>
                                                                <td>
                                                                    <asp:CheckBox ID="chkDeveTransferir" runat="server" Text="Deve Transferir?" Checked='<%# Bind("DeveTransferir") %>'
                                                                        OnLoad="Loja_Load" ForeColor="Red" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdTipoVenda1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Venda
                                                    </td>
                                                    <td id="tdTipoVenda2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoVenda" runat="server" onchange="tipoVendaChange(this, true);" onblur="verificarDescontoFormaPagtoDadosProduto();"
                                                                        SelectedValue='<%# Bind("TipoVenda") %>' DataSourceID="odsTipoVenda" DataTextField="Descr"
                                                                        DataValueField="Id">
                                                                    </asp:DropDownList>
                                                                    <div id="divObra" style="display: none">
                                                                        <asp:TextBox ID="txtObra" runat="server" Enabled="false" Width="200px" Text='<%# Eval("DescrObra") %>'></asp:TextBox>
                                                                        <asp:ImageButton ID="imbObra" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick='<%# "if (podeSelecionarObra()) openWindow(560, 650, \"../Utils/SelObra.aspx?Situacao=4&idsPedidosIgnorar=" + Request["idPedido"] + "&idCliente=\" + FindControl(\"txtNumCli\", \"input\").value); return false;" %>' />
                                                                        <br />
                                                                        <asp:Label ID="lblSaldoObra" runat="server" Text='<%# Eval("SaldoObra", "{0:C}") %>'></asp:Label>
                                                                        <asp:HiddenField ID="hdfIdObra" runat="server" Value='<%# Bind("IdObra") %>' />
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table cellpadding="0" cellspacing="0" id="funcionarioComprador" style='display: none;
                                                            padding-top: 2px'>
                                                            <tr>
                                                                <td>
                                                                    Funcionário comp.:&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFuncVenda" runat="server" DataSourceID="odsFuncVenda" DataTextField="Nome"
                                                                        DataValueField="IdFunc" AppendDataBoundItems="True" SelectedValue='<%# Bind("IdFuncVenda") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdTipoEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Entrega
                                                    </td>
                                                    <td id="tdTipoEntrega2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="ddlTipoEntrega" runat="server" SelectedValue='<%# Bind("TipoEntrega") %>'
                                                                        onchange="setLocalObra(true);" AppendDataBoundItems="True" DataSourceID="odsTipoEntrega"
                                                                        DataTextField="Descr" DataValueField="Id" OnLoad="ddlTipoEntrega_Load" OnDataBound="ddlTipoEntrega_DataBound">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    &nbsp;
                                                                </td>
                                                                <td class="dtvHeader" nowrap="nowrap">
                                                                    Tipo Pedido
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoPedido" runat="server" onchange="alteraDataEntrega(false)"
                                                                        DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" OnDataBound="drpTipoPedido_DataBound"
                                                                        SelectedValue='<%# Bind("TipoPedido") %>' AppendDataBoundItems="True">
                                                                    </asp:DropDownList>
                                                                    <div id="divGerarPedidoProducaoCorte">
                                                                        <asp:CheckBox ID="chkGerarPedidoProducaoCorte" runat="server" 
                                                                            Text="Gerar Pedido de Produção para Corte" Checked='<%# Bind("GerarPedidoProducaoCorte") %>' />
                                                                    </div>
                                                                    <asp:HiddenField ID="hdfPedidoRevenda" runat="server" Value='<%# Bind("IdPedidoRevenda") %>' />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdFormaPagto1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Forma Pagto.
                                                    </td>
                                                    <td id="tdFormaPagto2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="True" DataSourceID="odsFormaPagto"
                                                                        DataTextField="Descricao" onchange="formaPagtoChanged();" onblur="verificarDescontoFormaPagtoDadosProduto();" DataValueField="IdFormaPagto"
                                                                        SelectedValue='<%# Bind("IdFormaPagto") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoCartao" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCartao"
                                                                        DataTextField="Descricao" DataValueField="IdTipoCartao" SelectedValue='<%# Bind("IdTipoCartao") %>' onblur="verificarDescontoFormaPagtoDadosProduto();">
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdDataEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Entrega
                                                    </td>
                                                    <td id="tdDataEntrega2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td>
                                                                    <uc8:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadOnly" DataString='<%# Bind("DataEntregaString") %>'
                                                                        ExibirHoras="False" onchange="verificaDataEntrega(this)" OnLoad="ctrlDataEntrega_Load" />
                                                                    <asp:HiddenField ID="hdfDataEntregaNormal" runat="server" />
                                                                    <asp:HiddenField ID="hdfDataEntregaFD" runat="server" />
                                                                </td>
                                                                <td align="left" class="dtvHeader" nowrap="nowrap">
                                                                    <asp:Label runat="server" ID="lblValorFrete" OnLoad="txtValorFrete_Load" Text="Valor do Frete"></asp:Label> 
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox runat="server" ID="txtValorFrete" onkeypress="return soNumeros(event, false, true);" Width="80px" Text='<%# Bind("ValorEntrega") %>'
                                                                        OnLoad="txtValorFrete_Load"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdValorEntrada1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Valor Entrada
                                                    </td>
                                                    <td id="tdValorEntrada2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValEntradaIns" runat="server" Value='<%# Bind("ValorEntrada") %>' />
                                                    </td>
                                                    <td id="tdFuncionario1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Funcionário
                                                    </td>
                                                    <td id="tdFuncionario2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:DropDownList ID="drpVendedorIns" runat="server" DataSourceID="odsFuncionario"
                                                            DataTextField="Nome" DataValueField="IdFunc" Enabled="<%# Glass.Data.Helper.Config.PossuiPermissao(Glass.Data.Helper.Config.FuncaoMenuPedido.AlterarVendedorPedido) %>"
                                                            SelectedValue='<%# Bind("IdFunc") %>' onchange="alteraDataPedidoFunc(this.value)">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr class="dtvHeader">
                                                    <td>
                                                        Transportador
                                                        <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsTransportador" DataTextField="Name" DataValueField="Id"
                                                            SelectedValue='<%# Bind("IdTransportador") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr class="dtvHeader">
                                                    <td nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    Local da Obra&nbsp;
                                                                </td>
                                                                <td>
                                                                    <a href="javascript:getEnderecoCli();">
                                                                        <img src="../Images/home.gif" title="Buscar endereço do cliente" border="0"></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        Endereço
                                                    </td>
                                                    <td nowrap="nowrap">
                                                        <asp:TextBox ID="txtEnderecoObra" runat="server" MaxLength="100" disabled="true"
                                                            Text='<%# Bind("EnderecoObra") %>' Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Bairro
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtBairroObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("BairroObra") %>'
                                                            Width="130px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Cidade
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCidadeObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("CidadeObra") %>'
                                                            Width="130px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Cep
                                                    </td>
                                                    <td>
                                                        <td align="left" nowrap="nowrap">
                                                            <asp:TextBox ID="txtCepObra" runat="server" MaxLength="9" Text='<%# Bind("CepObra") %>' onkeypress="return soCep(event)"
                                                                onkeyup="return maskCep(event, this);"></asp:TextBox>
                                                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                                OnClientClick="iniciaPesquisaCepObra(FindControl('txtCepObra', 'input').value); return false" />
                                                        </td>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tbComissionado" cellpadding="0" cellspacing="0" style="width: 100%">
                                                <tr class="dtvHeader">
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td class="dtvHeader">
                                                                    Comissionado:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblComissionado" runat="server"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelComissionado" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelComissionado.aspx'); return false;"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>">
                                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                    <asp:ImageButton ID="imbLimparComissionado" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                                        OnClientClick="limparComissionado(); return false;" ToolTip="Limpar comissionado"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                            <tr>
                                                                <td class="dtvHeader">
                                                                    Percentual:
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Text='<%# Bind("PercComissao") %>' Width="50px" OnLoad="txtPercentual_Load"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                            <tr>
                                                                <td class="dtvHeader">
                                                                    Valor Comissão:
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtValorComissao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        ReadOnly="True" Text='<%# Eval("ValorComissao", "{0:C}") %>' Width="70px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tbMedidor0" cellpadding="0" cellspacing="0" class="dtvHeader" style="width: 100%">
                                                <tr>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    &nbsp;Medidor:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblMedidor" runat="server" Text='<%# Eval("NomeMedidor") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelMedidor" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelMedidor.aspx'); return false;">
                                                                        <img border="0" 
                            src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr>
                                                    <td class="dtvHeader" align="center" colspan="2">
                                                        Observação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:TextBox ID="txtObs" runat="server" MaxLength="1000" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                        <td>
                                                            <uc10:ctrlLimiteTexto ID="lmtTxtObs" runat="server" IdControlToValidate="txtObs" />
                                                        </td>
                                                        <tr>
                                                            <td class="dtvHeader" align="center" colspan="2" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                                Observação Liberação
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                                <asp:TextBox ID="txtObsLiberacao" runat="server" MaxLength="1000" Text='<%# Bind("ObsLiberacao") %>'
                                                                    TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                                <asp:HiddenField ID="hdfCliente" runat="server" Value='<%# Bind("IdCli") %>' />
                                                                <asp:HiddenField ID="hdfIdComissionado" runat="server" Value='<%# Bind("IdComissionado") %>' />
                                                                <asp:HiddenField ID="hdfIdMedidor" runat="server" Value='<%# Bind("IdMedidor") %>' />
                                                                <asp:HiddenField ID="hdfAliquotaIcms" runat="server" Value='<%# Eval("AliquotaIcms") %>' />
                                                                <asp:HiddenField ID="hdfDataPedido" runat="server" Value='<%# Bind("DataPedidoString") %>' />
                                                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                                                                    TypeName="Glass.Data.Helper.DataSources">
                                                                </colo:VirtualObjectDataSource>
                                                            </td>
                                                            <td style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                                                                <uc10:ctrlLimiteTexto ID="lmtTxtObsLiberacao" runat="server" IdControlToValidate="txtObsLiberacao" />
                                                            </td>
                                                        </tr>
                                            </table>
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <table cellpadding="2" cellspacing="2">
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Num. Pedido
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNumPedido" runat="server" Text='<%# Eval("IdPedido") %>' Font-Size="Medium"></asp:Label>
                                                        <asp:Label ID="lblDescrTipoPedido" runat="server" Text='<%# "(" + Eval("DescricaoTipoPedido") + ")" %>'
                                                            ForeColor="Green"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:Label ID="lblNomeCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Funcionário
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeFunc" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tel. Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTelCliente" runat="server" Text='<%# Eval("RptTelContCli") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Loja
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeLoja" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Endereço Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="5">
                                                        <asp:Label ID="lblEndereco" runat="server" Text='<%# Eval("EnderecoCompletoCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Endereço Obra</td>
                                                    <td align="left" colspan="5" nowrap="nowrap">
                                                        <asp:Label ID="lblLocalObra" runat="server" Text='<%# Eval("LocalizacaoObra") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Valor Entrada
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblValorEnt" runat="server" Text='<%# Eval("ValorEntrada", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Venda
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoVenda" runat="server" Text='<%# Eval("DescrTipoVenda") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Entrega
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoEntrega" runat="server" Text='<%# Eval("DescrTipoEntrega") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Situação
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("DescrSituacaoPedido") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data Ped.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataPedidoString", "{0:d}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data Entrega
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDataEntrega" runat="server" Text='<%# Eval("DataEntregaString") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label runat="server" ID="lblValorFrete" OnLoad="txtValorFrete_Load" Text="Valor do Frete"></asp:Label> 
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="Label18" runat="server" Text='<%# Eval("ValorEntrega", "{0:C}") %>' OnLoad="txtValorFrete_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Desconto
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDesconto" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold" runat="server" id="tituloComissao"
                                                        visible='<%# Eval("ComissaoVisible") %>'>
                                                        <asp:Label ID="Label13" runat="server" Text="Comissão"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="comissao" visible='<%# Eval("ComissaoVisible") %>'>
                                                        <asp:Label ID="lblComissao" runat="server" Text='<%# Eval("ValorComissao", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" runat="server" id="tituloIcms" onload="Icms_Load">
                                                        <asp:Label ID="lblTituloIcms" runat="server" Font-Bold="True" Text="Valor ICMS"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="valorIcms" onload="Icms_Load">
                                                        <asp:Label ID="lblValorIcms" runat="server" Text='<%# Eval("ValorIcms", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="tituloIpi" onload="Ipi_Load">
                                                        <asp:Label ID="lblTituloIpi" runat="server" Font-Bold="True" Text="Valor IPI"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="valorIpi" onload="Ipi_Load">
                                                        <asp:Label ID="lblValorIpi" runat="server" Text='<%# Eval("ValorIpi", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="6" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td class="cabecalho">
                                                                    <asp:Label ID="lblTitleTotal" runat="server" Font-Bold="True" OnLoad="lblTotalGeral_Load"
                                                                        Text="Total"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotal" runat="server" ForeColor="#0000CC" OnLoad="lblTotalGeral_Load"
                                                                        Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                                <td class="cabecalho" nowrap="nowrap">
                                                                    <asp:Label ID="lblTitleTotalBruto" runat="server" Font-Bold="True" OnLoad="lblTotalBrutoLiquido_Load"
                                                                        Text="Total Bruto"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotalBruto" runat="server" OnLoad="lblTotalBrutoLiquido_Load" Text='<%# Eval("TotalBruto", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                                <td class="cabecalho" nowrap="nowrap">
                                                                    <asp:Label ID="lblTitleTotalLiquido" runat="server" Font-Bold="True" OnLoad="lblTotalBrutoLiquido_Load"
                                                                        Text="Total Líquido"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotalLiquido" runat="server" ForeColor="#0000CC" OnLoad="lblTotalBrutoLiquido_Load"
                                                                        Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Forma Pagto.
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <asp:Label ID="lblFormaPagto" runat="server" Text='<%# Eval("PagtoParcela") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="lblTituloFastDelivery" runat="server" Text="Fast delivery" OnLoad="FastDelivery_Load"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblFastDelivery" runat="server" Text='<%# Eval("FastDelivery") %>'
                                                            OnLoad="FastDelivery_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        
                                                    </td>
                                                    <td colspan="3" align="left">
                                                        
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        
                                                    </td>
                                                    <td align="left">
                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label16" runat="server" Text="Funcionário comp."></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:Label ID="lblFuncVenda" runat="server" Text='<%# Eval("NomeFuncVenda") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="lblDeveTransferirTexto" runat="server" Text="Deve Transferir?" OnLoad="Loja_Load"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label ID="lblDeveTransferirValor" runat="server" Text='<%# Eval("DeveTransferirStr") %>' OnLoad="Loja_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Transportador
                                                    </td>
                                                    <td align="left" colspan="5">
                                                        <asp:Label ID="Label19" runat="server" Text='<%# Eval("NomeTransportador") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Observação
                                                    </td>
                                                    <td align="left" colspan="5">
                                                        <asp:Label ID="lblObs" runat="server" Text='<%# Eval("Obs") %>' ForeColor="Blue"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Obs. do Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="5">
                                                        <asp:Label ID="lblObsCliente" runat="server" OnLoad="lblObsCliente_Load" Text='<%# Eval("ObsCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                            <asp:HiddenField ID="hdfCliRevenda" runat="server" Value='<%# Eval("CliRevenda") %>' />
                                            <asp:HiddenField ID="hdfTipoVenda" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                            <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                            <asp:HiddenField ID="hdfPercComissao" runat="server" Value='<%# Eval("PercComissao") %>' />
                                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCli") %>' />
                                            <asp:HiddenField ID="hdfFastDelivery" runat="server" OnPreRender="FastDelivery_Load"
                                                Value='<%# Eval("FastDelivery") %>' />
                                            <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                            <asp:HiddenField ID="hdfTipoPedido" runat="server" Value='<%# Eval("TipoPedido") %>' />
                                            <asp:HiddenField ID="hdfIsReposicao" runat="server" Value='<%# IsReposicao(Eval("TipoVenda")) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                                OnClientClick="if (!onUpdate(this)) return false;" />
                                            <asp:Button ID="btnCancelarEdit" CausesValidation="false" runat="server" OnClick="btnCancelarEdit_Click"
                                                Text="Cancelar" />
                                            <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdPedido"
                                                Text='<%# Bind("IdPedido") %>' />
                                            <asp:HiddenField ID="hdfLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                                            <asp:HiddenField ID="hdfClienteAtual" runat="server" Value='<%# Eval("IdCli") %>' />
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="if (!onInsert(this)) return false;"
                                                Text="Inserir" />
                                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                                Text="Cancelar" />
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="true"><a href="#" onclick='openWindow(500, 700, "../Utils/SelTextoPedido.aspx?idPedido="+&#039;<%# Eval("IdPedido").ToString() %>&#039;); return false;'>
                                                <img border="0" src="../Images/note_add.gif" title="Textos Pedido" /></a> </asp:PlaceHolder>
                                            <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" OnClick="btnEditar_Click" />
                                            <asp:Button ID="btnFinalizar" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                                Text="Finalizar" OnClientClick="if (!finalizarPedido()) return false;" OnClick="btnFinalizar_Click" />
                                            <asp:Button ID="btnEmConferencia" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                                OnClick="btnEmConferencia_Click" OnClientClick="if (!emConferencia()) return false;"
                                                Text="Em Conferência" Visible='<%# Eval("ConferenciaVisible") %>' Width="110px" />
                                            <asp:Button ID="btnGerarConfEdit" runat="server" OnClick="btnGerarConfEdit_Click"
                                                OnLoad="btnGerarConfEdit_Load" Text="Confirmar editando Conferência " />
                                            <asp:Button ID="btnGerarConfFin" runat="server" OnClick="btnGerarConfFin_Click" OnLoad="btnGerarConfFin_Load"
                                                Text="Confirmar com Conferência Finalizada" />
                                            <asp:Button ID="btnVoltar" runat="server" OnClick="btnCancelar_Click" Visible="false"
                                                Text="Voltar" />
                                            <asp:HiddenField ID="hdfLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                            <asp:HiddenField ID="hdfAlterarProjeto" runat="server" Value="false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkProjeto" runat="server" OnClientClick="return openProjeto('', false);">Incluir Projeto</asp:LinkButton>
                <div id="divProduto" runat="server">
                    <table>
                        <tr runat="server" id="inserirMaoObra" visible="false">
                            <td align="center">
                                <asp:LinkButton ID="lbkInserirMaoObra" runat="server">Inserir várias peças de vidro com a mesma mão de obra</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdAmbiente" runat="server" AllowPaging="True"
                                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdAmbientePedido"
                                    DataSourceID="odsAmbiente" OnRowCommand="grdAmbiente_RowCommand" ShowFooter="True"
                                    OnPreRender="grdAmbiente_PreRender" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnRowDeleted="grdAmbiente_RowDeleted"
                                    OnRowUpdated="grdAmbiente_RowUpdated">
                                    <Columns>
                                        <asp:TemplateField>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="lnkAddAmbiente" runat="server" OnClientClick="addAmbiente(true); return false;"
                                                    ImageUrl="~/Images/Insert.gif" CausesValidation="False" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CausesValidation="False">
                                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Excluir este ambiente fará com que todos os produtos do mesmo sejam excluídos também, confirma exclusão?&quot;)"
                                                    CausesValidation="False" />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" ValidationGroup="ambiente" />
                                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Cancelar" CausesValidation="False" />
                                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                                            </EditItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtAmbiente" runat="server" Text='<%# Eval("Ambiente") %>' MaxLength="50"
                                                    Width="150px" OnLoad="txtAmbiente_Load" onchange="FindControl('hdfDescrAmbiente', 'input').value = this.value"></asp:TextBox>
                                                <div runat="server" id="EditAmbMaoObra" onload="ambMaoObra_Load">
                                                    <asp:TextBox ID="txtCodAmb" runat="server" onblur='<%# "var_ProdutoAmbiente=true; loadProduto(this.value, 0);" %>'
                                                        onkeydown='<%# "if (isEnter(event)) loadProduto(this.value, 0);" %>' onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodInterno") %>' Width="50px"></asp:TextBox>
                                                    <asp:Label ID="lblDescrAmb" runat="server" Text='<%# Eval("Ambiente") %>'></asp:Label>
                                                    <a href="#" onclick="var_ProdutoAmbiente=true; getProduto(); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                    <asp:HiddenField ID="hdfAmbIdProd" Value='<%# Bind("IdProd") %>' runat="server" />
                                                </div>
                                                <asp:HiddenField ID="hdfDescrAmbiente" Value='<%# Bind("Ambiente") %>' runat="server" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtAmbiente" runat="server" MaxLength="50" Width="150px" OnLoad="txtAmbiente_Load"
                                                    onchange="FindControl('hdfDescrAmbiente', 'input').value = this.value"></asp:TextBox>
                                                <div runat="server" id="ambMaoObra" onload="ambMaoObra_Load">
                                                    <asp:TextBox ID="txtCodAmb" runat="server" onblur='<%# "var_ProdutoAmbiente=true; loadProduto(this.value, 0);" %>'
                                                        onkeydown='<%# "if (isEnter(event)) loadProduto(this.value, 0" %>' onkeypress="return !(isEnter(event));"
                                                        Width="50px"></asp:TextBox>
                                                    <asp:Label ID="lblDescrAmb" runat="server"></asp:Label>
                                                    <a href="#" onclick="var_ProdutoAmbiente=true; getProduto(); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                    <asp:HiddenField ID="hdfAmbIdProd" runat="server" />
                                                </div>
                                                <asp:HiddenField ID="hdfDescrAmbiente" runat="server" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkViewProd" runat="server" CausesValidation="False" CommandArgument='<%# Eval("IdAmbientePedido") %>'
                                                    CommandName="ViewProd" Text='<%# Eval("Ambiente") %>' Visible='<%# !(bool)Eval("ProjetoVisible") %>'></asp:LinkButton>
                                                <asp:PlaceHolder ID="PlaceHolder1" Visible='<%# Eval("ProjetoVisible") %>' runat="server">
                                                    <a href="#" onclick='return openProjeto(<%# Eval("IdAmbientePedido") %>)'>
                                                        <%# Eval("Ambiente") %></a> </asp:PlaceHolder>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditDescricao" runat="server" Text='<%# Bind("Descricao") %>'
                                                    MaxLength="1000" Rows="2" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="1000" Rows="2" TextMode="MultiLine"
                                                    Width="300px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                                <asp:Label ID="Label17" runat="server" ForeColor="Red" Text='<%# Eval("DescrObsProj") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditQtdeAmbiente" runat="server" Text='<%# Bind("Qtde") %>' onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ControlToValidate="txtEditQtdeAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtQtdeAmbiente" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ControlToValidate="txtQtdeAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </FooterTemplate>
                                        </asp:TemplateField>                                        
                                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditLarguraAmbiente" runat="server" Text='<%# Bind("Largura") %>'
                                                    onkeypress="return soNumeros(event, true, true)" Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvLargura" runat="server" ControlToValidate="txtEditLarguraAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtLarguraAmbiente" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvLargura" runat="server" ControlToValidate="txtLarguraAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditAlturaAmbiente" runat="server" Text='<%# Bind("Altura") %>'
                                                    onkeypress="return soNumeros(event, true, true)" Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvAltura" runat="server" ControlToValidate="txtEditAlturaAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtAlturaAmbiente" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvAltura" runat="server" ControlToValidate="txtAlturaAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </FooterTemplate>
                                        </asp:TemplateField>                                        
                                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso" Visible="False">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbProcIns" runat="server" onblur="var_ProcAmbiente=true; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_ProcAmbiente=true; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_ProcAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbProcIns" runat="server" onblur="var_ProcAmbiente=true; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_ProcAmbiente=true; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_ProcAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao" Visible="False">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbAplIns" runat="server" onblur="var_AplAmbiente=true; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_AplAmbiente=true; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_AplAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbAplIns" runat="server" onblur="var_AplAmbiente=true; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_AplAmbiente=true; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_AplAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Redondo" SortExpression="Redondo" Visible="False">
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("Redondo") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:CheckBox ID="chkRedondoAmbiente" runat="server" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("Redondo") %>' Enabled="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor produtos" SortExpression="TotalProdutos">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotalProd" runat="server" Text='<%# Eval("TotalProdutos", "{0:c}") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("TotalProdutos", "{0:c}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Acréscimo" SortExpression="Acrescimo">
                                            <EditItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList ID="drpTipoAcrescimo" runat="server" SelectedValue='<%# Bind("TipoAcrescimo") %>'>
                                                                <asp:ListItem Value="1">%</asp:ListItem>
                                                                <asp:ListItem Selected="True" Value="2">R$</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtAcrescimo" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                                Text='<%# Bind("Acrescimo") %>' Width="50px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Desconto" SortExpression="Desconto">
                                            <EditItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList ID="drpTipoDesconto" runat="server" SelectedValue='<%# Bind("TipoDesconto") %>'
                                                                onclick="calcularDesconto(2)">
                                                                <asp:ListItem Value="1">%</asp:ListItem>
                                                                <asp:ListItem Value="2">R$</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtDesconto" runat="server" onchange="calcularDesconto(2)" onkeypress="return soNumeros(event, false, true)"
                                                                Text='<%# Bind("Desconto") %>' Width="50px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfValorDescontoAtual" runat="server" Value='<%# Eval("ValorDescontoAtual") %>' />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <FooterTemplate>
                                                <asp:LinkButton ID="lnkInsAmbiente" runat="server" OnClick="lnkInsAmbiente_Click"
                                                    ValidationGroup="ambiente">
                                            <img border="0" src="../Images/ok.gif" /></asp:LinkButton>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                             <ItemTemplate>
                                                 <uc12:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="AmbientePedido" IdRegistro='<%# Eval("IdAmbientePedido") %>' />
                                             </ItemTemplate>
                                         </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle CssClass="edit"></EditRowStyle>
                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                </asp:GridView>
                                <asp:Label ID="lblAmbiente" runat="server" CssClass="subtitle1" Font-Bold="False"></asp:Label>
                                <asp:HiddenField ID="hdfAlturaAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfLarguraAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfQtdeAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfRedondoAmbiente" runat="server" />
                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAmbiente" runat="server" DataObjectTypeName="Glass.Data.Model.AmbientePedido"
                                    DeleteMethod="DeleteComTransacao" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.AmbientePedidoDAO"
                                    UpdateMethod="Update" OnDeleted="odsAmbiente_Deleted" OnUpdating="odsAmbiente_Updating"
                                    OnDeleting="odsAmbiente_Deleting" OnInserting="odsAmbiente_Inserting" >
                                    <SelectParameters>
                                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                                <asp:HiddenField ID="hdfIdAmbiente" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%# Eval("Ambiente") %>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXPed" CssClass="gridStyle"
                                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                    DataKeyNames="IdProdPed" OnRowDeleted="grdProdutos_RowDeleted" ShowFooter="True"
                                    OnRowCommand="grdProdutos_RowCommand" OnPreRender="grdProdutos_PreRender" PageSize="12"
                                    OnRowUpdated="grdProdutos_RowUpdated" OnRowCreated="grdProdutos_RowCreated">
                                    <FooterStyle Wrap="True" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <FooterTemplate>
                                                <select id="drpFooterVisible" style="display: none">
                                                </select>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>'>
                                                    <img border="0" src="../Images/Edit.gif" ></img></asp:LinkButton>
                                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Excluir" Visible='<%# Eval("DeleteVisible") %>' OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(false); return false" : "if (!confirm(\"Deseja remover esse produto do pedido?\")) return false" %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick='<%# "if(!onUpdateProd(" + Eval("IdProdPed") + ")) return false;"%>' />
                                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Cancelar" />
                                                <asp:HiddenField ID="hdfProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                                <asp:HiddenField ID="hdfCodInterno" runat="server" Value='<%# Eval("CodInterno") %>' />
                                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                                <asp:HiddenField ID="hdfIsAluminio" runat="server" Value='<%# Eval("IsAluminio") %>' />
                                                <asp:HiddenField ID="hdfM2Minimo" runat="server" Value='<%# Eval("M2Minimo") %>' />
                                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                                <asp:HiddenField ID="hdfIdItemProjeto" runat="server" Value='<%# Bind("IdItemProjeto") %>' />
                                                <asp:HiddenField ID="hdfIdMaterItemProj" runat="server" Value='<%# Bind("IdMaterItemProj") %>' />
                                                <asp:HiddenField ID="hdfIdAmbientePedido" runat="server" Value='<%# Bind("IdAmbientePedido") %>' />
                                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" Value='<%# Bind("AliqIcms") %>' />
                                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" Value='<%# Bind("ValorIcms") %>' />
                                                <asp:HiddenField ID="hdfValorTabelaOrcamento" runat="server" Value='<%# Bind("ValorTabelaOrcamento") %>' />
                                                <asp:HiddenField ID="hdfValorTabelaPedido" runat="server" Value='<%# Bind("ValorTabelaPedido") %>' />
                                            </EditItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblCodProdIns" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfCustoProd" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtCodProdIns" runat="server" onblur='<%# "loadProduto(this.value, 0);" %>'
                                                    onkeydown='<%# "if (isEnter(event)) loadProduto(this.value, 0);" %>' onkeypress="return !(isEnter(event));"
                                                    Width="50px"></asp:TextBox>
                                                <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
                                                <a href="#" onclick="getProduto(); return false;">
                                                    <img src="../Images/Pesquisar.gif" border="0" /></a>
                                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" />
                                                <asp:HiddenField ID="hdfTipoCalc" runat="server" />
                                                <asp:HiddenField ID="hdfIsAluminio" runat="server" />
                                                <asp:HiddenField ID="hdfM2Minimo" runat="server" />
                                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" />
                                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" />
                                                <asp:HiddenField ID="hdfCustoProd" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtQtdeIns" runat="server" onblur="calcM2Prod(); return verificaEstoque();"
                                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    Text='<%# Bind("Qtde") %>' Width="50px"></asp:TextBox>
                                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                                <uc5:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" Callback="calcTotalProd"
                                                    CallbackValorUnit="calcTotalProd" ValidationGroup="produto" PercDescontoQtde='<%# Bind("PercDescontoQtde") %>'
                                                    ValorDescontoQtde='<%# Bind("ValorDescontoQtde") %>' OnLoad="ctrlDescontoQtde_Load" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtQtdeIns" runat="server" onkeydown="if (isEnter(event)) calcM2Prod();"
                                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    onblur="calcM2Prod(); return verificaEstoque();" Width="50px"></asp:TextBox>
                                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                                <uc5:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" Callback="calcTotalProd"
                                                    ValidationGroup="produto" CallbackValorUnit="calcTotalProd" OnLoad="ctrlDescontoQtde_Load" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtLarguraIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, true, true);"
                                                    Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="50px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtLarguraIns" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                    onblur="calcM2Prod();" Width="50px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtAlturaIns" runat="server" onblur="calcM2Prod(); return verificaEstoque();"
                                                    Text='<%# Bind("Altura") %>' onchange="FindControl('hdfAlturaReal', 'input').value = this.value"
                                                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"></asp:TextBox>
                                                <asp:HiddenField ID="hdfAlturaReal" runat="server" Value='<%# Bind("AlturaReal") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtAlturaIns" runat="server" onblur="calcM2Prod(); return verificaEstoque();"
                                                    Width="50px" onchange="FindControl('hdfAlturaReal', 'input').value = this.value"
                                                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"></asp:TextBox>
                                                <asp:HiddenField ID="hdfAlturaRealIns" runat="server" />
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotM2Ins" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfTotM" runat="server" Value='<%# Eval("TotM") %>' />
                                                <asp:HiddenField ID="hdfTamanhoMaximoObra" runat="server" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotM2Ins" runat="server"></asp:Label>
                                                <asp:HiddenField ID="hdfTamanhoMaximoObra" runat="server" />
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotM2Calc">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotM2Calc" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfTotM2Calc" runat="server" Value='<%# Eval("TotM2Calc") %>' />
                                                <asp:HiddenField ID="hdfTotM2CalcSemChapa" runat="server" Value='<%# Eval("TotalM2CalcSemChapaString") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotM2CalcIns" runat="server"></asp:Label>
                                                <asp:HiddenField ID="hdfTotM2Ins" runat="server" />
                                                <asp:HiddenField ID="hdfTotM2CalcIns" runat="server" />
                                                <asp:HiddenField ID="hdfTotM2CalcSemChapaIns" runat="server" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label12" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="True" />
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("ValorVendido", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtValorIns" runat="server" onblur="calcTotalProd();" onkeypress="return soNumeros(event, false, true);"
                                                    Text='<%# Bind("ValorVendido") %>' Width="50px" OnLoad="txtValorIns_Load"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtValorIns" runat="server" onkeydown="if (isEnter(event)) calcTotalProd();"
                                                    onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProd();"
                                                    Width="50px" OnLoad="txtValorIns_Load"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="var_ProcAmbiente=false; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_ProcAmbiente=false; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a id="lnkProcesso" href="#" onclick='var_ProcAmbiente=false; buscarProcessos(); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="var_ProcAmbiente=false; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_ProcAmbiente=false; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a id="lnkProcesso" href="#" onclick='var_ProcAmbiente=false; buscarProcessos(); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="var_AplAmbiente=false; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_AplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_AplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="var_AplAmbiente=false; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { var_AplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                                Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="var_AplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ped. Cli." SortExpression="PedCli">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtPedCli" runat="server" MaxLength="50" Text='<%# Bind("PedCli") %>'
                                                    Width="50px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtPedCli" runat="server" MaxLength="50" Width="50px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("PedCli") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label>
                                                <asp:Label ID="Label43" runat="server" Text='<%# "(Desconto de " + Eval("PercDescontoQtde") + "%)" %>'
                                                    Visible='<%# (float)Eval("PercDescontoQtde") > 0 %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotalIns" runat="server" Text='<%# Bind("Total") %>' Style="padding-top: 4px"></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotalIns" runat="server"></asp:Label>
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblValorBenef" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblValorBenef" runat="server"></asp:Label>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("ValorBenef", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <EditItemTemplate>
                                                <div id="benefMaoObra" style='<%# !IsPedidoMaoDeObra() ? "display: none;": "" %> white-space: nowrap'>                                                    
                                                    <asp:DropDownList ID="drpLargBenef" runat="server" onchange="calcTotalProd()" SelectedValue='<%# Bind("LarguraBenef") %>'>
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:DropDownList ID="drpAltBenef" runat="server" onchange="calcTotalProd()" SelectedValue='<%# Bind("AlturaBenef") %>'>
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    Esp.:
                                                    <asp:TextBox ID="txtEspBenef" Width="30px" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                        Text='<%# Bind("EspessuraBenef") %>'></asp:TextBox>
                                                </div>
                                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick='<%# "exibirBenef(this, " + Eval("IdProdPed") + "); return false;" %>'
                                                    Visible='<%# Eval("BenefVisible") %>'>
                                                    <img border="0" src="../Images/gear_add.gif" />
                                                </asp:LinkButton>
                                                <table id='<%# "tbConfigVidro_" + Eval("IdProdPed") %>' cellspacing="0" style="display: none;">
                                                    <tr align="left">
                                                        <td align="center">
                                                            <table>
                                                                <tr>
                                                                    <td class="dtvFieldBold">
                                                                        Espessura
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtEspessura" runat="server" OnDataBinding="txtEspessura_DataBinding"
                                                                            onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Bind("Espessura") %>'></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc4:ctrlBenef ID="ctrlBenefEditar" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>'
                                                                ValidationGroup="produto" OnInit="ctrlBenef_Load" Redondo='<%# Bind("Redondo") %>'
                                                                CallbackCalculoValorTotal="setValorTotal" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                        </td>
                                                    </tr>
                                                </table>
                                                <script type="text/javascript">
                                                    calculaTamanhoMaximo();
                                                </script>

                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <div id="benefMaoObra" style='<%# !IsPedidoMaoDeObra() ? "display: none;": "" %> white-space: nowrap'>
                                                    <asp:DropDownList ID="drpLargBenef" runat="server" onchange="calcTotalProd()">
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:DropDownList ID="drpAltBenef" runat="server" onchange="calcTotalProd()">
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>                                                    
                                                    Esp.:
                                                    <asp:TextBox ID="txtEspBenef" Width="30px" runat="server" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                                </div>
                                                <asp:LinkButton ID="lnkBenef" runat="server" Style="display: none;" OnClientClick="exibirBenef(this, 0); return false;">
                                                    <img border="0" src="../Images/gear_add.gif" />
                                                </asp:LinkButton>
                                                <table id="tbConfigVidro_0" cellspacing="0" style="display: none;">
                                                    <tr align="left">
                                                        <td align="center">
                                                            <table>
                                                                <tr>
                                                                    <td class="dtvFieldBold">
                                                                        Espessura
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtEspessura" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                            Width="30px"></asp:TextBox>
                                                                        <asp:HiddenField ID="xsds" runat="server" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc4:ctrlBenef ID="ctrlBenefInserir" runat="server" OnInit="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotal"
                                                                ValidationGroup="produto" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                        </td>
                                                    </tr>
                                                </table>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <div id='<%# "imgProdsComposto_" + Eval("IdProdPed") %>'>
                                                    <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/box.png" ToolTip="Exibir Produtos da Composição"
                                                        Visible='<%# Eval("IsProdLamComposicao") %>' OnClientClick='<%# "exibirProdsComposicao(this, " + Eval("IdProdPed") + "); return false"%>' />
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/imagem.gif"
                                                        OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=pedido&idPedido=" + Eval("IdPedido") +"&idProdPed=" +  Eval("IdProdPed") +
                                                            "&pecaAvulsa=" +  ((bool)Eval("IsProdLamComposicao") == false) + "\"); return false" %>'
                                                        ToolTip="Exibir imagem das peças"  Visible='<%# (Eval("IsVidro").ToString() == "true")%>'/>
                                                </div>
                                            </ItemTemplate>
                                            <EditItemTemplate></EditItemTemplate>
                                            <FooterTemplate></FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <a href="#" id="lnkObsCalc" onclick="exibirObs(<%# Eval("IdProdPed") %>, this); return false;" visible='<%# (Eval("IsVidro").ToString() == "true")%>'>
                                                    <img border="0" src="../../Images/blocodenotas.png" title="Observação da peça" /></a>
                                                <table id='tbObsCalc_<%# Eval("IdProdPed") %>' cellspacing="0" style="display: none;">
                                                    <tr>
                                                        <td align="center">
                                                            <asp:TextBox ID="txtObsCalc" runat="server" Width="320" Rows="4" MaxLength="500"
                                                                TextMode="MultiLine" Text='<%# Eval("Obs") %>'></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <input id="btnSalvarObs" onclick='setCalcObs(<%# Eval("IdProdPed") %>, this); return false;'
                                                                type="button" value="Salvar" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                            <EditItemTemplate></EditItemTemplate>
                                            <FooterTemplate></FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                             <ItemTemplate>
                                                </td> </tr>

                                                <tr id="prodPed_<%# Eval("IdProdPed") %>" style="display: none" align="center">
                                                    <td colspan="17">
                                                        <br />
                                                        <uc13:ctrlProdComposicao runat="server" ID="ctrlProdComp" Visible='<%# Eval("IsProdLamComposicao") %>' 
                                                            IdProdPed='<%# Glass.Conversoes.StrParaUint(Eval("IdProdPed").ToString()) %>'/>
                                                        <br />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                </td> </tr>
                                                <tr style='<%= !IsPedidoMaoDeObra() ? "display: none": "" %>'>
                                                    <td colspan="17" style="text-align: right">
                                                        <span style="position: relative; top: -6px">campos usados para definir altura, largura
                                                            e
                                                            <br />
                                                            espessura da lapidação e bisotê </span>
                                                    </td>
                                                </tr>
                                                <tr style='<%= !IsPedidoProducao() ? "display: none": "" %>'>
                                                    <td colspan="4">
                                                    </td>
                                                    <td colspan="13" style="text-align: left">
                                                        <span style="position: relative; top: -6px">altura e largura definidas no produto
                                                            <br />
                                                            e recuperadas automaticamente </span>
                                                    </td>
                                                </tr>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="lnkInsProd" runat="server" OnClick="lnkInsProd_Click" ImageUrl="../Images/ok.gif"
                                                    OnClientClick="if (!onInsertProd()) return false;" />
                                                </td> </tr>
                                                <tr style='<%= !IsPedidoMaoDeObra() ? "display: none": "" %>'>
                                                    <td colspan="15" style="text-align: right">
                                                        <span style="position: relative; top: -6px">campos usados para definir altura, largura
                                                            e
                                                            <br />
                                                            espessura da lapidação e bisotê </span>
                                                    </td>
                                                </tr>
                                                <tr style='<%= !IsPedidoProducao() ? "display: none": "" %>'>
                                                    <td colspan="4">
                                                    </td>
                                                    <td colspan="13" style="text-align: left">
                                                        <span style="position: relative; top: -6px">altura e largura definidas no produto
                                                            <br />
                                                            e recuperadas automaticamente </span>
                                                    </td>
                                                </tr>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle CssClass="edit"></EditRowStyle>
                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdXPed" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedido"
        DeleteMethod="DeleteEAtualizaDataEntrega" EnablePaging="True" MaximumRowsParameterName="pageSize"
        OnDeleted="odsProdXPed_Deleted" SelectCountMethod="GetCount" SelectMethod="GetList"
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
        UpdateMethod="UpdateEAtualizaDataEntrega" OnUpdating="odsProdXPed_Updating" 
        OnDeleting="odsProdXPed_Deleting" OnInserting="odsProdXPed_Inserting" OnUpdated="odsProdXPed_Updated">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
            <asp:ControlParameter ControlID="hdfIdAmbiente" Name="idAmbientePedido" PropertyName="Value"
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfPedidoMaoDeObra" runat="server" />
    <asp:HiddenField ID="hdfPedidoProducao" runat="server" />
    <asp:HiddenField ID="hdfBloquearMaoDeObra" runat="server" />
    <asp:HiddenField ID="hdfIdPedido" runat="server" />
    <asp:HiddenField ID="hdfIdProd" runat="server" />
    <asp:HiddenField ID="hdfComissaoVisible" runat="server" />
    <asp:HiddenField ID="hdfMedidorVisible" runat="server" />
    <asp:HiddenField ID="hdfCurrPage" runat="server" Value="0" />
    <asp:HiddenField ID="hdfNaoVendeVidro" runat="server" />
    <asp:HiddenField ID="hdfProdPedComposicaoSelecionado" runat="server" Value="0" />

    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCartao" runat="server" SelectMethod="ObtemListaPorTipo"
        TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
        <SelectParameters>
            <asp:Parameter Name="tipo" Type="Int32" DefaultValue="0" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEntrega" runat="server"
        SelectMethod="GetTipoEntrega" TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedido" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
        InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.PedidoDAO"
        UpdateMethod="Update" OnInserted="odsPedido_Inserted" OnUpdating="odsPedido_Updating" OnUpdated="odsPedido_Updated">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
        SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoJato" runat="server" SelectMethod="GetTipoJato"
        TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCanto" runat="server" SelectMethod="GetTipoCanto"
        TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
        TypeName="Glass.Data.DAL.LojaDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForPedido"
        TypeName="Glass.Data.DAL.FormaPagtoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncVenda" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.FuncionarioDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedido"
        TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTransportador" runat="server"
        SelectMethod="ObtemDescritoresTransportadores" TypeName="Glass.Global.Negocios.ITransportadorFluxo">
    </colo:VirtualObjectDataSource>
    <script type="text/javascript">
        
    controlarVisibilidadeProducaoCorte();

    if (FindControl("imbAtualizar", "input") != null && 
        FindControl("lblCodProdIns", "span") != null && 
        FindControl("hdfProdPed", "input") != null)
        loadProduto(FindControl("lblCodProdIns", "span").innerHTML, FindControl("hdfProdPed", "input").value, true);

    var drpTipoVenda = FindControl("drpTipoVenda", "select");
    if (drpTipoVenda != null)
    {
        tipoVendaChange(drpTipoVenda, false);
        
        if (FindControl("hdfExibirParcela", "input") != null)
            FindControl("hdfExibirParcela", "input").value = drpTipoVenda.value == 2;
        
        if (FindControl("hdfCalcularParcela", "input") != null)
            FindControl("hdfCalcularParcela", "input").value = false;
    }
    
    var cCodProd = FindControl("txtCodProdIns", "input");
    if (cCodProd != null)
        cCodProd.focus();

    try
    {
        if (ultimoCodProd)
            cCodProd.value = ultimoCodProd;
    }
    catch (err) {}

    // Esconde tabela de comissionado
    var hdfComissaoVisible = FindControl("hdfComissaoVisible", "input");
    var tbComissionado = FindControl("tbComissionado", "table");
    if (hdfComissaoVisible != null && tbComissionado != null && hdfComissaoVisible.value == "false")
        tbComissionado.style.display = "none";
        
    // Esconde tabela de medidor
    var hdfMedidorVisible = FindControl("hdfMedidorVisible", "input");
    var tbMedidor = FindControl("tbMedidor", "table");
    if (hdfMedidorVisible != null && tbMedidor != null && hdfMedidorVisible.value == "false")
        tbMedidor.style.display = "none";

    // Esconde controles de inserção de ambiente
    if (FindControl("lnkAddAmbiente", "input") != null)
        addAmbiente(false);

    modificarLayoutGridProdutos();
    
    var numCli = FindControl("txtNumCli", "input");
    if (numCli != null && numCli.value != "")
    {
        var tipoVenda = FindControl("drpTipoVenda", "select");
        var formaPagto = FindControl("drpFormaPagto", "select");
        var tipoCartaoCredito = FindControl("drpTipoCartao", "select");
        
        var tva = tipoVenda.value;
        var fpa = formaPagto != null ? formaPagto.value : null;
        var tcc = tipoCartaoCredito != null ? tipoCartaoCredito.value : null;
        
        getCli(numCli.value);
        
        tipoVenda.value = tva;
        if (formaPagto != null) formaPagto.value = fpa;
        
        tipoVenda.onchange();

        if (formaPagto != null)
        {
            formaPagto.onchange();

            if (tcc > 0 && formaPagto.value == var_CodCartao)
                tipoCartaoCredito.value = tcc;
        }
    }
    
    // Habilita/Desabilita campos do local da obra
    setLocalObra(false);
    
    if (FindControl("drpVendedorIns", "select") != null)
        FindControl("drpVendedorIns", "select").onchange();
    else if (FindControl("drpVendedorEdit", "select") != null)
        alteraDataEntrega(false);

    if (FindControl("drpLoja", "select") && !config_AlterarLojaPedido)
        FindControl("drpLoja", "select").disabled = true;
    
    var parcelas = FindControl("drpParcelas", "select");

    if (parcelas != null)
    {
        parcelas.onblur = function()
        {            
            //Busca o Desconto por parcela ou por Forma de pagamento e dados do produto
            var retDesconto = null;

            if (config_UsarDescontoEmParcela && parcelas != null)
            {
                retDesconto = CadPedido.VerificaDescontoParcela(parcelas.value, var_IdPedido);

                if (retDesconto.error != null)
                {
                    alert(retDesconto.error.description);
                    return false;
                }
                else if (retDesconto != null && retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "")
                {
                    var txtDesconto = FindControl("txtDesconto","input");
                    var txtTipoDesconto = FindControl("drpTipoDesconto","select");

                    if (txtTipoDesconto != null)
                    {
                        txtTipoDesconto.value = 1;
                    }

                    if (txtDesconto != null)
                    {
                        txtDesconto.value = retDesconto.value.replace(".", ",");
                        txtDesconto.onchange();
                        txtDesconto.onblur();
                    }
                }
            }
            else if (config_UsarControleDescontoFormaPagamentoDadosProduto)
            {
                verificarDescontoFormaPagtoDadosProduto();
            }
        }
    }

    $(document).ready(function(){

        var hdfProdPedComposicaoSelecionado = FindControl("hdfProdPedComposicaoSelecionado", "input");

        if(hdfProdPedComposicaoSelecionado.value > 0){
            var div = FindControl("imgProdsComposto_" + hdfProdPedComposicaoSelecionado.value, "div");

            if(div == null) return;

            var botao = FindControl("imgProdsComposto", "input", div);
            exibirProdsComposicao(botao, hdfProdPedComposicaoSelecionado.value);
        }
    });
    
    var_Loading = false;
    
    </script>

</asp:Content>
