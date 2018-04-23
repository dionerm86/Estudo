<%@ Page Title="Liberar Pedidos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeBehind="CadLiberarPedido.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadLiberarPedido" %>

<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlParcelasSelecionar.ascx" TagName="ctrlParcelasSelecionar" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src="https://s3.amazonaws.com/cappta.api/js/cappta-checkout.js"></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/cappta-tef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
    
    var chamarCallback = true;
    var buscandoCliente = false;
    var descontoLiberacao = <%= Glass.Configuracoes.Liberacao.DadosLiberacao.DescontoLiberarPedido.ToString().ToLower() %>;
    
    function verificaAlteracaoPedidos()
    {        
        var idsPedidos = new Array();
        var idsSinais = new Array();
        var idsPagtoAntecip = new Array();
        var tabela = FindControl("grdPedido", "table");
        
        for (var i = 0; i < tabela.rows.length; i++)
        {
            var idPedido = FindControl("hdfIdPedido", "input", tabela.rows[i].cells[0]);
            if (idPedido != null)
                idsPedidos.push(idPedido.value);
            
            var idSinal = FindControl("hdfIdSinal", "input", tabela.rows[i].cells[0]);
            if (idSinal != null && idSinal.value != "")
                idsSinais.push(idSinal.value);
                
            var idPagtoAntecip = FindControl("hdfIdPagtoAntecip", "input", tabela.rows[i].cells[0]);
            if (idPagtoAntecip != null && idPagtoAntecip.value != "")
                idsPagtoAntecip.push(idPagtoAntecip.value);   
        }
        
        if (idsPedidos.length == 0)
            return true;
        
        var dataTela = FindControl("hdfDataTela", "input").value;
        
        var recalcular = CadLiberarPedido.IsPedidosAlterados(idsPedidos.join(','), idsSinais.join(','), idsPagtoAntecip.join(','), dataTela);
        if (recalcular.value.split('|')[0] == "true")
        {
            FindControl("lblMensagemRecalcular", "span").innerHTML = recalcular.value.split('|')[1] + "<br /><br />";
            FindControl("hdfRecarregarTabelaPedido", "input").value = "true";
            alterouProduto();
            
            return false;
        }
        
        return true;
    }
    
    function alterouProduto(iniciando)
    {
        iniciando = iniciando == true;
        document.getElementById("pagamento").style.display = iniciando ? "" : "none";
        document.getElementById("recalcular").style.display = iniciando ? "none" : "";
    }
    
    function alteraQtdeLib(tabela, idProdPed)
    {
        var qtde = document.getElementById("qtde_" + idProdPed);
        var qtdeMax = document.getElementById("qtdeMax_" + idProdPed);
        
        if (qtde.value == "")
            qtde.value = 0;
        else if (parseInt(qtde.value, 10) > parseInt(qtdeMax.value, 10))
            qtde.value = qtdeMax.value;
        
        marcaProd(tabela, idProdPed, false);
        marcaProd(tabela, idProdPed, true, parseInt(qtde.value, 10));
        
        document.getElementById("chkProdPed_" + idProdPed).checked = qtde.value == qtdeMax.value;
    }
    
    function escondeDescontoAcrescimo()
    {
        try
        {
            document.getElementById("dadosDesconto").style.display = "none";
            document.getElementById("dadosAcrescimo").style.display = "none";
        }
        catch (err) { }
    }
    
    function exibeProd(tabela, idProdPed, botaoExibir)
    {
        var exibir = botaoExibir.src.indexOf("mais") > -1;
        botaoExibir.src = botaoExibir.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
        botaoExibir.title = (exibir ? "Esconder" : "Exibir") + " produtos";
        
        tabela = document.getElementById(tabela);
        for (i = 1; i < tabela.rows.length; i++)
        {
            var campoIdProdPed = FindControl("hdfIdProdPed", "input", tabela.rows[i]);
            if (campoIdProdPed == null || campoIdProdPed.value != idProdPed)
                continue;
            
            tabela.rows[i].style.display = exibir ? "" : "none";
        }
    }
    
    function marcaProd(tabela, idProdPed, marcar, qtdeMarcar)
    {
        qtdeMarcar = typeof qtdeMarcar == 'number' ? qtdeMarcar : 
            parseInt(document.getElementById("qtdeMax_" + idProdPed).value, 10);
        
        var qtdeMarcados = 0;
        tabela = document.getElementById(tabela);
        for (i = 1; i < tabela.rows.length; i++)
        {
            if (qtdeMarcados >= qtdeMarcar)
                break;
            
            var campoIdProdPed = FindControl("hdfIdProdPed", "input", tabela.rows[i]);
            if (campoIdProdPed == null || campoIdProdPed.value != idProdPed || FindControl("chkSelProdPed", "input", tabela.rows[i]).disabled)
                continue;
            
            FindControl("chkSelProdPed", "input", tabela.rows[i]).checked = marcar;
            qtdeMarcados++;
        }
        
        if (qtdeMarcados > 0)
            alterouProduto();
    }
    
    function getBotao(idPedidoBuscar)
    {
        var tabela = document.getElementById("<%= grdPedido.ClientID %>");
        for (i = 1; i < tabela.rows.length; i += 2)
        {
            var idPedido = tabela.rows[i].cells[1].innerHTML;
            if (idPedido != idPedidoBuscar)
                continue;
            
            var inputs = tabela.rows[i].cells[0].getElementsByTagName("input");
            for (j = 0; j < inputs.length; j++)
                if (inputs[j].id.indexOf("ImageButton1") > -1)
                    return inputs[j];
        }
        
        return null;
    }
    
    function validaFormaPagtoPrazo(val, args)
    {
        var totalPrazo = parseFloat(document.getElementById("<%= ctrlParcelas1.ClientID %>_txtValorParcelas").value);
        if (isNaN(totalPrazo))
            totalPrazo = 0;
    
        args.IsValid = document.getElementById("tbAPrazo").style.display == "none" ||
            args.Value != "" || totalPrazo == 0;
    }
    
    function getPedidosAbertos()
    {
        var pedidosAbertos = new Array();
        var tabela = document.getElementById("<%= grdPedido.ClientID %>");
        
        for (i = 1; i < tabela.rows.length; i += 2)
        {
            var idPedido = tabela.rows[i].cells[1].innerHTML;
            var aberto = getBotao(idPedido);
            aberto = aberto != null ? aberto.src.toLowerCase().indexOf("menos.gif") > -1 : false;
            
            if (aberto)
                pedidosAbertos.push(idPedido);
        }
        
        FindControl("hdfPedidosAbertos", "input").value = pedidosAbertos.join(",");
    }
    
     function addOC(){
    
        var idOC = FindControl("txtNumOC", "input").value;
        if (Trim(idOC) == "")
        {
            alert("Selecione uma OC para continuar.");
            FindControl("txtNumOC", "input").value = "";
            FindControl("txtNumOC", "input").focus();
            return;
        }
        
        var idsPedidosNovos = CadLiberarPedido.GetIdsPedidosByOCForLiberacao(idOC).value.split(';');
        if (idsPedidosNovos[0] == "Erro")
        {
            alert(idsPedidosNovos[1]);
            FindControl("txtNumOC", "input").value = "";
            FindControl("txtNumOC", "input").focus();
            return;
        }
        
        var tipoVenda = FindControl("hdfBloqueioTipoVenda", "input").value;
        var idFormaPagto = FindControl("hdfBloqueioIdFormaPagto", "input").value;
        var cxDiario = FindControl("hdfCxDiario", "input").value;
        
        var idsNovos = idsPedidosNovos[1].split(','); 
        var idsAntigos = FindControl("hdfBuscarIdsPedidos", "input").value.split(',');
        var retorno = new Array();
        
        for (var i = 0; i < idsAntigos.length; i++){
            if(idsAntigos[i].length > 0 && idsAntigos[i] != " ")
                retorno.push(idsAntigos[i]);
        }
        
        for (var i = 0; i < idsNovos.length; i++){
        
            var validaPedido = CadLiberarPedido.ValidaPedido(idsNovos[i], tipoVenda, idFormaPagto, cxDiario, "").value.split('|');
        
            if (validaPedido[0] == "false")
            {
                alert(validaPedido[1]);
                FindControl("txtNumPedido", "input").value = "";
                FindControl("txtNumPedido", "input").focus();
                return;
            }
            
            for (var j = 0; j < retorno.length; j++){
                if (retorno[j] == idsNovos[i] && retorno[j].length > 0)
                    continue;
            }
            
            retorno.push(idsNovos[i]);
        }
                
        FindControl("hdfBuscarIdsPedidos", "input").value = retorno.join(',');
        FindControl("txtNumPedido", "input").value = "";
        cOnClick("btnBuscarPedidos", null);
    }
    
    function addPedido()
    {
        if (buscandoCliente)
            return;
           
        var idPedido = FindControl("txtNumPedido", "input").value;
                
        if (Trim(idPedido) == "")
        {
            alert("Selecione um pedido para continuar.");
            FindControl("txtNumPedido", "input").value = "";
            FindControl("txtNumPedido", "input").focus();
            return;
        }
        
        var tipoVenda = FindControl("hdfBloqueioTipoVenda", "input").value;
        var idFormaPagto = FindControl("hdfBloqueioIdFormaPagto", "input").value;
        var cxDiario = FindControl("hdfCxDiario", "input").value;        
        var idsPedidos = FindControl("hdfBuscarIdsPedidos", "input").value;
        
        var validaPedido = CadLiberarPedido.ValidaPedido(idPedido, tipoVenda, idFormaPagto, cxDiario, (idsPedidos == "" || idsPedidos == null ? "" : idsPedidos + ",") + idPedido).value.split('|');
            
        if (validaPedido[0] == "false")
        {
            alert(validaPedido[1]);
            FindControl("txtNumPedido", "input").value = "";
            FindControl("txtNumPedido", "input").focus();
            return;
        }

        FindControl("hdfIdCliente", "input").value = validaPedido[1];
        
        idsPedidos = idsPedidos.split(',');
        
        var novosIds = new Array();
        
        novosIds.push(idPedido);
        for (i = 0; i < idsPedidos.length; i++)
            if (idsPedidos[i] != idPedido && idsPedidos[i].length > 0)
                novosIds.push(idsPedidos[i]);

        var validaPedidosMesmaLoja = CadLiberarPedido.VerificarPedidosMesmaLoja(novosIds).value.split('|');

        if (validaPedidosMesmaLoja[0] == "false")
        {
            alert(validaPedidosMesmaLoja[1]);
            return;
        }
        
        FindControl("hdfBuscarIdsPedidos", "input").value = novosIds.join(',');
        FindControl("txtNumPedido", "input").value = "";
        cOnClick("btnBuscarPedidos", null);
    }
    
    function removePedido(idPedido)
    {
        var idsPedidos = FindControl("hdfBuscarIdsPedidos", "input").value.split(',');
        var novosIds = new Array();
        
        for (i = 0; i < idsPedidos.length; i++)
            if (idsPedidos[i] != idPedido && idsPedidos[i].length > 0)
                novosIds.push(idsPedidos[i]);
        
        FindControl("hdfIdsPedidosRem", "input").value += idPedido + ",";
        
        FindControl("hdfBuscarIdsPedidos", "input").value = novosIds.join(',');
        cOnClick("btnBuscarPedidos", null);
    }
    
    function exibirProdutos(botao, idPedido)
    {
        var liberarProdutos = <%= Glass.Configuracoes.Liberacao.DadosLiberacao.LiberarPedidoProdutos.ToString().ToLower() %>;
        var exibirProdutosPedidoAoLiberar = <%= Glass.Configuracoes.PedidoConfig.ExibirProdutosPedidoAoLiberar.ToString().ToLower() %>;
        
        if (!liberarProdutos && !exibirProdutosPedidoAoLiberar)
            return;
        
        var linha = document.getElementById("produtos_" + idPedido);
        
        if (linha == null)
            return;
        
        var exibir = linha.style.display == "none";
        linha.style.display = exibir ? "" : "none";
        botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
        botao.title = (exibir ? "Esconder" : "Exibir") + " produtos";
    }
    
    function buscarPedidos()
    {
        var idCliente = FindControl("txtNumCli", "input").value;
        var nomeCliente = FindControl("txtNomeCliente", "input").value;
        if (idCliente == "" && nomeCliente == "")
            return;
        
        buscandoCliente = true;
        
        if (idCliente == "")
            idCliente = "0";
            
        var idsPedidosRem = FindControl("hdfIdsPedidosRem", "input").value;
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        var situacaoProd = FindControl("drpSituacaoProd", "select").value;
        var tiposPedidos = FindControl("cbdTipoPedido", "select").itens();
        var idLoja = FindControl("drpLoja", "select").value;
        
        FindControl("hdfBuscarIdsPedidos", "input").value = CadLiberarPedido.GetPedidosByCliente(
            idCliente, nomeCliente, idsPedidosRem, dataIni, dataFim, situacaoProd, tiposPedidos, idLoja).value;
    }
    
    function selecionaTodosProdutos(check)
    {
        var tabela = check;
        while (tabela.nodeName.toLowerCase() != "table")
            tabela = tabela.parentNode;
        
        var checkBoxProdutos = tabela.getElementsByTagName("input");
        
        var i = 0;
        for (i = 0; i < checkBoxProdutos.length; i++)
        {
            if (checkBoxProdutos[i].id.indexOf("chkTodos") > -1 || checkBoxProdutos[i].disabled)
                continue;
            
            checkBoxProdutos[i].checked = check.checked;
        }
        
        alterouProduto();
    }

    function getCli(idCli) {
        if (idCli.value == "")
            return false;

        var idCliente = idCli.value;

        var retorno = MetodosAjax.GetCli(idCliente).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idCli.value = "";
            FindControl("txtNomeCliente", "input").value = "";
            return false;
        }
        
        limpar();

        FindControl("txtNomeCliente", "input").value = retorno[1];
        FindControl("txtNumCli", "input").value = idCliente;
        FindControl("hdfIdCliente", "input").value = idCliente;

        return false;
    }

    // Abre popup para cadastrar cheques
    function queryStringCheques() {
        return "?LiberarPedido=1&origem=2";
    }

    function tipoPagtoChanged() {
        var ctrTipoPagto = FindControl("drpTipoPagto", "select");
        if (ctrTipoPagto == null) return;

        // Mostra/Esconde campos de pagamento à vista/à prazo dependendo do tipo pagto
        FindControl("msgErroTipoPagto", "div").style.display = ctrTipoPagto.value == "" ? "" : "none";
        FindControl("tbAVista", "table").style.display = ctrTipoPagto.value == 1 ? "inline" : "none";
        FindControl("tbAPrazo", "table").style.display = ctrTipoPagto.value == 2 ? "inline" : "none";
        FindControl("btnConfirmar", "input").style.display = ctrTipoPagto.value != "" ? "inline" : "none";
        
        //alteraFormaPagtoPrazo(FindControl("drpFormaPagtoPrazo", "select"));
        var chkReceberEntrada = FindControl("chkReceberEntrada", "input");
        receberEntrada(chkReceberEntrada ? chkReceberEntrada.checked : false);
        
        // Atualiza os dados da forma de pagamento selecionada
        <%= ctrlFormaPagto1.ClientID %>.Calcular();
        <%= ctrlFormaPagto2.ClientID %>.Calcular();
        <%= ctrlParcelas1.ClientID %>.Calcular();
        
        // Esconde o campo de desconto se for liberação à prazo e se a empresa não permitir
        var descontoPrazo = true;
        if (descontoLiberacao && !descontoPrazo)
        {
            var campoDesconto = FindControl("txtDesconto", "input");
            document.getElementById("dadosDesconto").style.display = ctrTipoPagto.value == 1 ? "" : "none";
            
            if (ctrTipoPagto.value == 2 && campoDesconto != null)
                campoDesconto.value = "";
        }
        
        if (campoDesconto != null)
            campoDesconto.onblur();
        
        //alteraFormaPagtoPrazo(FindControl("drpFormaPagtoPrazo", "select"));
    }

        function confirmar(control) {
            if (!validate())
                return false;

            if (confirm(control.value + '?') == false)
                return false;

            if (FindControl("hdfLibParc", "input").value == "true" && 
                !confirm("Um ou mais pedidos desta liberação estão sendo liberados PARCIALMENTE, deseja continuar?"))
                return false;

            try {
                var idCliente = FindControl("hdfIdCliente", "input").value;            
                var idsPedido = FindControl("hdfIdsPedido", "input").value;
                var idsProdutosPedido = FindControl("hdfIdsProdutosPedido", "input").value;
                var idsProdutosProducao = FindControl("hdfIdsProdutoPedidoProducao", "input").value;
                var qtdeProdutosLiberar = FindControl("hdfQtdeProdutosLiberar", "input").value;
                var totalASerPago = FindControl("hdfTotalASerPago", "input").value;
            
                // Verifica se algum dos pedidos foi pago antecipado ou recebeu sinal ou teve algum cancelamento do momento que foi 
                // adicionado na tela até agora.
                if (!verificaAlteracaoPedidos())
                    return false;

                // Se for garantia/reposição
                if (FindControl("hdfIsGarantiaReposicao", "input").value == "true")
                {
                    retorno = CadLiberarPedido.ConfirmarGarantiaReposicao(idCliente, idsPedido, idsProdutosPedido, 
                        idsProdutosProducao, qtdeProdutosLiberar).value;
                }
                else if (FindControl("hdfIsPedidoFuncionario", "input").value == "true")
                {
                    retorno = CadLiberarPedido.ConfirmarPedidoFuncionario(idCliente, idsPedido, idsProdutosPedido, 
                        idsProdutosProducao, qtdeProdutosLiberar).value;            
                }
                else
                {
                    var isAVista = FindControl("drpTipoPagto", "select").value == 1;
                    var cxDiario = FindControl("hdfCxDiario", "input").value;
                
                    var controle = isAVista ? <%= ctrlFormaPagto1.ClientID %> : <%= ctrlFormaPagto2.ClientID %>;
                    var formasPagto = controle.FormasPagamento();
                    var tiposCartao = controle.TiposCartao();
                    var valores = controle.Valores();
                    var contas = controle.ContasBanco();
                    var parcelasCartao = controle.ParcelasCartao();
                    var creditoUtilizado = controle.CreditoUtilizado();
                    var isGerarCredito = controle.GerarCredito();
                    var utilizarCredito = controle.UsarCredito();
                    var numAut = controle.NumeroConstrucard();
                    var isDescontarComissao = controle.DescontarComissao();
                    var cheques = controle.Cheques();
                    var tipoDesconto = FindControl("drpTipoDesconto", "select").value;
                    var desconto = FindControl("txtDesconto", "input").value;
                    var tipoDesconto = FindControl("drpTipoDesconto", "select").value;
                    var acrescimo = FindControl("txtAcrescimo", "input").value;
                    var tipoAcrescimo = FindControl("drpTipoAcrescimo", "select").value;
                    var valorUtilizadoObra = controle.ValorObra();
                    var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
                    var numAutCartao = controle.NumeroAutCartao();
                    var CNI = controle.CartoesNaoIdentificados();

                    var idFormaPgtoCartao = <%= (int)Glass.Data.Model.Pagto.FormaPagto.Cartao %>;
                    var utilizarTefCappta = <%= Glass.Configuracoes.FinanceiroConfig.UtilizarTefCappta.ToString().ToLower() %>;
                    var tipoCartaoCredito = <%= (int)Glass.Data.Model.TipoCartaoEnum.Credito %>;
                    var tipoRecebimento = <%= (int)Glass.Data.Helper.UtilsFinanceiro.TipoReceb.LiberacaoAVista %>;
                    var receberCappta = isAVista && utilizarTefCappta && formasPagto.split(';').indexOf(idFormaPgtoCartao.toString()) > -1;

                    // Se o tipo de pagamento for à vista
                    if (isAVista) {
                        retorno = CadLiberarPedido.ConfirmarAVista(idCliente, idsPedido, idsProdutosPedido, idsProdutosProducao, qtdeProdutosLiberar, formasPagto, tiposCartao,
                            totalASerPago, valores, contas, depositoNaoIdentificado, CNI, isGerarCredito, utilizarCredito, creditoUtilizado, numAut, cxDiario, parcelasCartao, isDescontarComissao, cheques,
                            tipoDesconto, desconto, tipoAcrescimo, acrescimo, valorUtilizadoObra, numAutCartao, receberCappta.toString().toLowerCase()).value;
                    }
                    else {
                        var drpParcelas = FindControl("drpParcelas", "select");
                        var numParcelas = FindControl("ctrlParcelasSelecionar1_hdfNumParcelas", "input").value;
                        var receberEntrada = FindControl("chkReceberEntrada", "input");
                        receberEntrada = receberEntrada ? receberEntrada.checked : false;
                        var diasParcelas = FindControl("ctrlParcelasSelecionar1_hdfDiasParcelas", "input").value;
                        var drpFormaPagtoPrazo = FindControl("drpFormaPagtoPrazo", "select");
                        var valoresParcelas = <%= ctrlParcelas1.ClientID %>.Valores();                        
                        var idParcela = drpParcelas != null && drpParcelas.value > 0 ? drpParcelas.value : "";
                    
                        retorno = CadLiberarPedido.ConfirmarAPrazo(idCliente, idsPedido, idsProdutosPedido, idsProdutosProducao, qtdeProdutosLiberar, totalASerPago, numParcelas, diasParcelas, 
                            idParcela, valoresParcelas, receberEntrada, formasPagto, tiposCartao, valores, contas, depositoNaoIdentificado, CNI, utilizarCredito, creditoUtilizado, numAut, cxDiario, parcelasCartao, isDescontarComissao,
                            tipoDesconto, desconto, tipoAcrescimo, acrescimo, drpFormaPagtoPrazo.value, valorUtilizadoObra, cheques, numAutCartao).value;
                    }
                }

                if(retorno.error != null){
                    desbloquearPagina(true);
                    alert(retorno.error.description);
                    return false;
                } else {
                    retorno = retorno.split('\t');
                }

                if (retorno[0] == "Erro") {
                    desbloquearPagina(true);
                    alert(retorno[1]);
                    return false;
                }

                // Limpa hidden com pedidos eliminados
                FindControl("hdfIdsPedidosRem", "input").value = "";

                if(receberCappta) {

                    var idLiberarPedido = retorno[3];

                    //Busca os dados para autenticar na cappta
                    var dadosAutenticacaoCappta = MetodosAjax.ObterDadosAutenticacaoCappta();

                    if(dadosAutenticacaoCappta.error) {
                        desbloquearPagina(true);
                        alert(dadosAutenticacaoCappta.error.description);
                        limpar();
                        cOnClick("btnBuscarPedidos", null);
                        return true;
                    }

                    //Instancia do canal de recebimento
                    CapptaTef.init(dadosAutenticacaoCappta.value, (sucesso, msg, codigosAdministrativos) => callbackCappta(sucesso, msg, codigosAdministrativos, retorno));

                    //Inicia o recebimento
                    CapptaTef.efetuarRecebimento(idLiberarPedido, tipoRecebimento, idFormaPgtoCartao, tipoCartaoCredito, formasPagto, tiposCartao, valores, parcelasCartao);

                    return false;

                } else {
            
                    openWindow(600, 800, "../Relatorios/RelLiberacao.aspx?idLiberarPedido=" + retorno[3]);

                    if (retorno[2] == "true")
                        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=NotaPromissoria&idLiberarPedido=" + retorno[3]);
                }

                desbloquearPagina(true);
                alert(retorno[1]);  
                limpar();
                cOnClick("btnBuscarPedidos", null);
                return true;
            }
            catch (err) {
                alert(err);
                return false;
            }
        }

        function callbackCappta(sucesso, msg, codigosAdministrativos, retorno) {

            desbloquearPagina(true);

            if(sucesso) {
                openWindow(600, 800, "../Relatorios/RelLiberacao.aspx?idLiberarPedido=" + retorno[3]);
                openWindow(600, 800, "../Relatorios/Relbase.aspx?rel=ComprovanteTef&codControle=" + codigosAdministrativos.join(';'));

                if (retorno[2] == "true")
                    openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=NotaPromissoria&idLiberarPedido=" + retorno[3]);

                var retEmitirNfce = CadLiberarPedido.EmitirNFCe(retorno[3]);

                if(retEmitirNfce.error) {
                    alert(retEmitirNfce.error.description);
                } else if(retEmitirNfce.value != "") {
                    openWindow(600, 800, "../Relatorios/NFe/RelBase.aspx?rel=Danfe&idNf=" + retEmitirNfce.value);
                }
            }
       
            alert(sucesso ? retorno[1] : msg);  
            limpar();
            cOnClick("btnBuscarPedidos", null);
            return true;
        }

    function limpar() {
        FindControl("hdfValorCredito", "input").value = "";
        FindControl("hdfIdCliente", "input").value = "";
        FindControl("hdfIdsPedido", "input").value = "";
        FindControl("hdfTotalASerPago", "input").value = "";

        try {
            FindControl("hdfIdsProdutosPedido", "input").value = "";
            FindControl("hdfIdsProdutoPedidoProducao", "input").value = "";
            FindControl("lblCredito", "span").innerHTML = "";
            FindControl("lblValorASerPago", "span").innerHTML = "";
            FindControl("txtNumCli", "input").value = "";
            FindControl("txtNomeCliente", "input").value = "";
                
            if (FindControl("txtDesconto", "input") != null)
                FindControl("txtDesconto", "input").value = "";
                
            if (FindControl("lblTotalIcms", "span") != null)
                FindControl("lblTotalIcms", "span").innerHTML = "";

            if (FindControl("lblTotalIpi", "span") != null)
                FindControl("lblTotalIpi", "span").innerHTML = "";
                
            <%= ctrlFormaPagto1.ClientID %>.Limpar();
            <%= ctrlFormaPagto2.ClientID %>.Limpar();
            
        } 
        catch (err) {}
    }
    
    function parcelasChanged()
    {
        Parc_visibilidadeParcelas("<%= ctrlParcelas1.ClientID %>", "atualizarValorTotalPrazo");
    }
    
    function atualizarValorTotalPrazo()
    {
        //var totalEntrada = parseFloat(document.getElementById("<%= ctrlFormaPagto2.ClientID %>_txtValorPago").value);
        //if (isNaN(totalEntrada))
            totalEntrada = 0;
            
        var totalPrazo = parseFloat(document.getElementById("<%= ctrlParcelas1.ClientID %>_txtValorParcelas").value);
        if (isNaN(totalPrazo))
            totalPrazo = 0;
            
        if (totalPrazo == 0)
            totalPrazo = parseFloat(FindControl("hdfValorASerPagoPrazo", "input").value.replace(",", "."));
        
        var totalObra = parseFloat(FindControl("hdfValorObra", "input").value.replace(",", "."));
        if (isNaN(totalObra))
            totalObra = 0;
            
        var totalCredito = parseFloat(FindControl("hdfValorCredito", "input").value.replace(",", "."));
        if (isNaN(totalCredito))
            totalCredito = 0;
        
        document.getElementById("valorTotalPrazo").innerHTML = "Valor a ser Pago: R$ " + (totalEntrada + totalPrazo).toFixed(2).replace('.', ',');
        
        if (document.getElementById("<%= creditoClientePrazo.ClientID %>") != null)
            document.getElementById("<%= creditoClientePrazo.ClientID %>").innerHTML = "O cliente possui R$ " + totalCredito.toFixed(2).replace(".", ",") + " de Crédito.";
        
        if (document.getElementById("<%= valorObraPrazo.ClientID %>") != null)
        {
            document.getElementById("<%= valorObraPrazo.ClientID %>").innerHTML = totalObra == 0 ? "" : 
                "Valor utilizado da(s) obra(s): R$ " + totalObra.toFixed(2).replace(".", ",");
        }
    }
    
    function receberEntrada(checked)
    {
        document.getElementById('receberEntrada').style.display = checked ? FindControl("tbAPrazo", "table").style.display : 'none';
        if (checked)
            usarCredito("<%= ctrlFormaPagto2.ClientID %>", "", "");
        else
        {
            document.getElementById("<%= ctrlFormaPagto2.ClientID %>_txtValorPago").value = 0;
            parcelasChanged();
        }
    }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imbAdd', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addPedido(); return false;" />
                        </td>
                        <td>
                            <asp:Label ID="lblOC" runat="server" Text="Num. OC" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumOC" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imbAddOC', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAddOC" runat="server" ImageUrl="~/Images/Insert.gif"
                                OnClientClick="addOC(); return false;" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="60px" onkeydown="if (isEnter(event)) getCli(this);"
                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeCliente" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('btnBuscarPedidos', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx'); return false;"> <img border="0" src="../Images/Pesquisar.gif" /> </asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc5:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Data de Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <uc4:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <uc4:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <asp:Label ID="lblSituacaoProd" runat="server" Text="Situação Prod." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacaoProd" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsSituacaoProd" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="Tipo de Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdTipoPedido" runat="server" CheckAll="False" DataSourceID="odsTipoPedido"
                                DataTextField="Descr" DataValueField="Id" ImageURL="~/Images/DropDown.png" OpenOnStart="False"
                                Title="Tipo de pedido" Width="160px">
                            </sync:CheckBoxListDropDown>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnBuscarPedidos" runat="server" Text="Buscar Pedidos" OnClick="btnBuscarPedidos_Click"
                    OnClientClick="buscarPedidos()" CausesValidation="False" />
                <asp:HiddenField ID="hdfBuscarIdsPedidos" runat="server" />
                <asp:HiddenField ID="hdfPedidosAbertos" runat="server" />
                <asp:HiddenField ID="hdfDataTela" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblObsCliente" runat="server" Style="display: inline-block; padding: 10px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsPedidos" DataKeyNames="IdPedido" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum pedido confirmado encontrado."
                    OnDataBound="grdPedido_DataBound" OnRowDataBound="grdPedido_RowDataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "removePedido(" + Eval("IdPedido") + "); return false;" %>'
                                    ToolTip="Remover pedido" />
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirProdutos(this, " + Eval("IdPedido") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir produtos" 
                                    Visible="<%# Glass.Configuracoes.Liberacao.DadosLiberacao.LiberarPedidoProdutos || Glass.Configuracoes.PedidoConfig.ExibirProdutosPedidoAoLiberar %>" />
                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCli") %>' />
                                <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Eval("TotalParaLiberacao") %>' />
                                <asp:HiddenField ID="hdfTotalEspelho" runat="server" Value='<%# Eval("TotalEspelho") %>' />
                                <asp:HiddenField ID="hdfTipoVenda" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                <asp:HiddenField ID="hdfIdFormaPagto" runat="server" Value='<%# Eval("IdFormaPagto") %>' />
                                <asp:HiddenField ID="hdfIdSinal" runat="server" Value='<%# Eval("IdSinal") %>' />
                                <asp:HiddenField ID="hdfIdPagtoAntecip" runat="server" Value='<%# Eval("IdPagamentoAntecipado") %>' />
                                <asp:HiddenField ID="hdfPedidoMaoObra" runat="server" Value='<%# Eval("MaoDeObra") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="CodCliente" HeaderText="Cód. Pedido Cli." SortExpression="CodCliente" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="ValorEntrada" DataFormatString="{0:c}" HeaderText="Valor Entrada"
                            SortExpression="ValorEntrada" />
                        <asp:BoundField DataField="ValorPagamentoAntecipado" DataFormatString="{0:c}" HeaderText="Pagto. Antecipado"
                            SortExpression="PagamentoAntecipado">
                            <ItemStyle ForeColor="#009900" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ValorIcms" DataFormatString="{0:c}" HeaderText="Valor ICMS"
                            SortExpression="ValorIcms" />
                        <asp:BoundField DataField="TextoDescontoTotal" HeaderText="Desconto" SortExpression="TextoDescontoTotal" />
                        <asp:BoundField DataField="TotalParaLiberacao" HeaderText="Total" SortExpression="TotalParaLiberacao"
                            DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Total a ser pago" SortExpression="TotalParaLiberacao" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Math.Round(((decimal)Eval("ValorASerPagoLiberacao")), 2) %>'></asp:Label>
                                <br/>
                                <asp:ImageButton ID="imbAviso" runat="server" ImageUrl="~/Images/help.gif" Visible='<%# Math.Round(((decimal)Eval("ValorASerPagoLiberacao")), 2) < 0 %>' 
                                    ToolTip='<%# Eval("ValorNegativoLiberar") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pagto" SortExpression="DescrTipoVenda">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("DescrTipoVenda").ToString() + Eval("DescricaoParcelas").ToString() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrTipoVenda") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataEntrega" DataFormatString="{0:d}" HeaderText="Data Entrega"
                            SortExpression="DataEntrega" />
                        <asp:TemplateField HeaderText="Fast Delivery" SortExpression="FastDelivery">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("FastDelivery") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" Style="white-space: nowrap" Checked='<%# Eval("FastDelivery") %>'
                                    Enabled="False" Text='<%# ((decimal)Eval("ValorFastDeliveryLiberacao")).ToString("C") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação Produção" SortExpression="DescrSituacaoProducao">
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DescrSituacaoProducao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("DescrSituacaoProducao") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs Lib." SortExpression="ObsLiberacao">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ObsLiberacao") %>'></asp:Label>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("ObsLiberacaoCliente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <img src="../Images/carregamento.png" alt='<%# "Ordem de Carga: " + Eval("IdsOCs") %>' title='<%# "Ordem de Carga: " + Eval("IdsOCs") %>'
                                    style='<%# string.IsNullOrEmpty((string)Eval("IdsOCs")) ? "display:none;" : "" %>' Width="16" Height="16" />
                            </ItemTemplate>
                        </asp:TemplateField>  
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr>
                                <tr id="produtos_<%# Eval("IdPedido") %>" style="display: none" class="<%= GetAlternateClass() %>">
                                    <td>
                                    </td>
                                    <td colspan="17" style="padding: 0px">
                                        <asp:GridView ID="grdProdutosPedido" runat="server" AutoGenerateColumns="False" CellPadding="3"
                                            DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedido" GridLines="None" Width="100%"
                                            OnRowDataBound="grdProdutosPedido_RowDataBound" ShowFooter="True" OnDataBound="grdProdutosPedido_DataBound">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <FooterTemplate>
                                                        Total
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkTodos" runat="server" onclick="selecionaTodosProdutos(this)"
                                                            Checked="True" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:PlaceHolder ID="pchInicio" runat="server"></asp:PlaceHolder>
                                                        <asp:CheckBox ID="chkSelProdPed" runat="server" Checked="True" onclick="alterouProduto()"
                                                            Style="margin-left: -1px; margin-right: 1px" />
                                                        <asp:HiddenField ID="hdfIdProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                                        <asp:HiddenField ID="hdfIdProdPedProducao" runat="server" Value='<%# Eval("IdProdPedProducaoConsulta") %>' />
                                                        <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# ((System.Drawing.Color)Eval("CorLinha")).Name %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="1%" />
                                                    <HeaderStyle Width="1%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Cod." SortExpression="CodInterno">
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox10" runat="server" Text='<%# Bind("CodInterno") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCodInterno" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDescricao" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                                                        <asp:Label ID="lblBenef" runat="server" Text='<%# Eval("DescrBeneficiamentos") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Altura" SortExpression="AlturaLista">
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAltura" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox8" runat="server" Text='<%# Bind("LarguraLista") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLargura" runat="server" Text='<%# Bind("LarguraLista") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotM" runat="server" Text='<%# Bind("TotM2Liberacao") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Valor ICMS" SortExpression="ValorIcms">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblValorIcms" runat="server" Text='<%# Bind("ValorIcms", "{0:c}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("ValorIcms") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total" SortExpression="TotalCalc">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotal" runat="server" Text='<%# Bind("TotalCalc", "{0:C4}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("TotalCalc") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Qtde disp." SortExpression="QtdeDisponivelLiberacao">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQtdeDisp" runat="server" Text='<%# Eval("QtdeDisponivelLiberacao") %>'></asp:Label>
                                                        <asp:Label ID="lblQtdePV" runat="server" Text='<%# Eval("QtdePecasVidroMaoDeObra") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("QtdeDisponivelLiberacao") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Qtde liberar" SortExpression="Qtde">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtQtde" runat="server" Text='<%# Eval("QtdeDisponivelLiberacao") %>'
                                                            onkeypress='<%# "return soNumeros(event, CalcProd_IsQtdeInteira(" + Eval("TipoCalc") + "), true)" %>'
                                                            Width="50px" onchange="alterouProduto()"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("QtdeDisponivelLiberacao") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Etiqueta" SortExpression="NumEtiquetaConsulta">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblNumEtiqueta" runat="server" Text='<%# Bind("NumEtiquetaConsulta") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox9" runat="server" Text='<%# Bind("NumEtiquetaConsulta") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle Font-Bold="true" />
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:GridView>
                                        <asp:HiddenField ID="hdfIdPedidoProdutos" runat="server" Value='<%# Eval("IdPedido") %>' />
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosPedido" runat="server"
                                            SelectMethod="GetForLiberacao" TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdPedidoProdutos" Name="idPedido" PropertyName="Value"
                                                    Type="UInt32" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <asp:Label ID="lblPedidosRem" runat="server"></asp:Label>
                <asp:ImageButton ID="imbLimparRemovidos" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                    OnClick="imbLimparRemovidos_Click" Visible="False" />
                <br />
                <asp:Label ID="lblTotalIcms" runat="server" Font-Size="Medium"></asp:Label>
                <br />
                <asp:Label ID="lblTotalIpi" runat="server" Font-Size="Medium"></asp:Label>
                <br />
                <asp:Label ID="lblDescrTotalM2" runat="server" Text="Total M2 selecionado: " 
                    Font-Size="12pt" ForeColor="Blue" Visible="false"></asp:Label>
                <asp:Label ID="lblTotalM2" runat="server" Font-Bold="True" Font-Size="12pt" 
                    ForeColor="Blue" Visible="false">0.00</asp:Label>
                &nbsp;&nbsp;
                <asp:Label ID="lblDescrTotalPeso" runat="server" Text="Total peso selecionado: " 
                    Font-Size="12pt" ForeColor="Blue" Visible="false"></asp:Label>
                <asp:Label ID="lblTotalPeso" runat="server" Font-Bold="True" Font-Size="12pt" 
                    ForeColor="Blue" Visible="false">0.00</asp:Label>
                <br />
                <br />
                <asp:LinkButton ID="lnkReceberSinal" runat="server" OnClientClick="openWindow(700, 850, 'CadReceberSinal.aspx?popup=1'); return false;">
                    Receber Sinal</asp:LinkButton>
                &nbsp;&nbsp;
                <asp:LinkButton ID="lnkPagtoAntecip" runat="server" OnClientClick="openWindow(700, 850, 'CadReceberSinal.aspx?antecipado=1&popup=1'); return false;">
                    Efetuar Pagto. Antecipado</asp:LinkButton>
                &nbsp;&nbsp;
                <asp:LinkButton ID="lnkConfirmarPedido" runat="server" OnClientClick="openWindow(700, 1050, 'CadConfirmarPedidoLiberacao.aspx?liberarPedido=true&revenda=true&popup=1'); return false;">
                    Confirmar Pedido</asp:LinkButton>
                <div runat="server" id="legenda" visible="false">
                    <br />
                    <span style="color: Red">Produtos em vermelho: Pendentes</span>
                    <br />
                    <span style="color: Blue">Produtos em azul: Prontos</span>
                    <br />
                    <span style="color: Green">Produtos em verde: Expedidos</span>
                    <br />
                    <span style="color: Gray">Produtos em cinza: Perda</span>
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
    <table id="recalcular">
        <tr>
            <td align="center">
                <asp:Label ID="lblMensagemRecalcular" runat="server" ForeColor="Red" Font-Size="Medium"></asp:Label>
                <asp:Button ID="btnRecalcular" runat="server" Text="Recalcular liberação" OnClick="btnRecalcular_Click"
                    CausesValidation="False" />
            </td>
        </tr>
    </table>
    <table id="pagamento">
        <tr>
            <td align="center">
                <asp:CheckBox ID="chkTaxaPrazo" runat="server" Text="Calcular a taxa de juros de venda à prazo para liberação à vista"
                    Visible="False" onclick="alterouProduto()" />
                <span runat="server" id="mensagemErro" visible="false">
                    <br />
                    <br />
                    <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                </span>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="tbPagto" runat="server" visible="false">
                    <tr>
                        <td align="center">
                            <div id="msgErroTipoPagto" style="display: none; color: Red; font-size: 120%">
                                Selecione uma parcela disponível para o cliente
                                <br />
                                (ou habilite a permissão para o cliente pagar à vista se só tiver parcelas à prazo)
                                <br />
                                <br />
                                <br />
                            </div>
                            <table>
                                <tr>
                                    <td>
                                        Pagto.
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpTipoPagto" runat="server" onchange="tipoPagtoChanged();"
                                            OnLoad="drpTipoPagto_Load">
                                            <asp:ListItem Value="1">À Vista</asp:ListItem>
                                            <asp:ListItem Value="2">À Prazo</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr id="dadosDesconto">
                                    <td>
                                        <asp:Label ID="lblDesconto" runat="server" Text="Desconto"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpTipoDesconto" runat="server">
                                            <asp:ListItem Value="1">%</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">R$</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:TextBox ID="txtDesconto" runat="server" onkeypress="return soNumeros(event, false, true)"
                                            Width="70px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr id="dadosAcrescimo">
                                    <td>
                                        <asp:Label ID="lblAcrescimo" runat="server" Text="Acréscimo"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpTipoAcrescimo" runat="server">
                                            <asp:ListItem Value="1">%</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">R$</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:TextBox ID="txtAcrescimo" runat="server" onkeypress="return soNumeros(event, false, true)"
                                            Width="70px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table id="tbAVista">
                                <tr>
                                    <td>
                                        <uc1:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" CalcularTroco="False" ParentID="tbAVista"
                                            TipoModel="Pedido" FuncaoQueryStringCheques="queryStringCheques"
                                            OnLoad="ctrlFormasPagto_Load" ExibirDataRecebimento="False"
                                            ExibirJuros="False" CallbackGerarCredito="callbackGerarCredito" ExibirRecebParcial="True"
                                            PermitirValorPagarNegativo="true" />

                                        <script type="text/javascript">
                                            document.getElementById("<%= ctrlFormaPagto1.ClientID %>_chkRecebimentoParcial").checked = true;
                                            document.getElementById("<%= ctrlFormaPagto1.ClientID %>_chkRecebimentoParcial").parentNode.style.display = "none";
                                        </script>

                                    </td>
                                </tr>
                            </table>
                            <table id="tbAPrazo">
                                <tr>
                                    <td align="center">
                                        <div>
                                            <span id="valorTotalPrazo" style="font-size: large;">Valor a ser Pago: R$ 0,00 </span>
                                        </div>
                                        <div id="creditoClientePrazo" style="font-size: 10pt; padding-top: 4px" runat="server">
                                            O cliente possui R$ 0,00 de crédito.
                                        </div>
                                        <div id="valorObraPrazo" style="font-size: 10pt; padding-top: 4px" runat="server">
                                            Valor utilizado da(s) obra(s): R$ 0,00
                                        </div>
                                        <br />
                                        <asp:CheckBox ID="chkReceberEntrada" runat="server" Text="Receber entrada?" onclick="receberEntrada(this.checked)" />
                                        <div id="receberEntrada" style="display: none">
                                            <br />
                                            <uc1:ctrlFormaPagto ID="ctrlFormaPagto2" runat="server" CalcularTroco="False" ParentID="receberEntrada"
                                                ExibirValorAPagar="false" TipoModel="Pedido" FuncaoQueryStringCheques="queryStringCheques"
                                                OnLoad="ctrlFormasPagto_Load" ExibirDataRecebimento="False"
                                                ExibirJuros="False" ExibirGerarCredito="false" CallbackGerarCredito="callbackGerarCredito"
                                                ExibirRecebParcial="False" PermitirValorPagarNegativo="true" />

                                            <script type="text/javascript">
                                                var spanRestante = document.getElementById("<%= ctrlFormaPagto2.ClientID %>_lblRestante");
                                                spanRestante.parentNode.style.display = "none";
                                            </script>

                                            <br />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <table>
                                            <tr>
                                                <td align="center" width="50%" nowrap="nowrap">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                Dias das parcelas:
                                                            </td>
                                                            <td>
                                                                <uc3:ctrlParcelasSelecionar ID="ctrlParcelasSelecionar1" runat="server" OnLoad="ctrlParcelasSelecionar1_Load"
                                                                    CallbackSelecaoParcelas="parcelasChanged" ExibirParcConfiguravel="True" SempreExibirDatasParcelas="True"
                                                                    ParentID="tbAPrazo" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                Forma de pagamento:
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="drpFormaPagtoPrazo" runat="server" DataSourceID="odsFormaPagto"
                                                                    AppendDataBoundItems="true" DataTextField="Descricao" 
                                                                    DataValueField="IdFormaPagto" ondatabound="drpFormaPagtoPrazo_DataBound">
                                                                    <asp:ListItem></asp:ListItem>
                                                                </asp:DropDownList>
                                                                
                                                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForPedido"
                                                                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                                                                    <SelectParameters>
                                                                        <asp:ControlParameter ControlID="hdfIdCliente" PropertyName="Value" Name="idCliente" Type="Int32" />
                                                                    </SelectParameters>
                                                                </colo:VirtualObjectDataSource>

                                                                <asp:CustomValidator ID="ctvPrazo" runat="server" ClientValidationFunction="validaFormaPagtoPrazo"
                                                                    ControlToValidate="drpFormaPagtoPrazo" Display="Dynamic" ErrorMessage="Selecione uma forma de pagamento"
                                                                    ValidateEmptyText="True"></asp:CustomValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="center">
                                                    <br />
                                                    <uc2:ctrlParcelas ID="ctrlParcelas1" runat="server" OnLoad="ctrlParcelas1_Load" CallbackTotal="atualizarValorTotalPrazo"
                                                        NumParcelas="8" ReadOnly="False" ParentID="tbAPrazo" />
                                                    <asp:HiddenField ID="hdfTextoParcelas" runat="server" />
                                                    <asp:HiddenField ID="hdfDataBase" runat="server" />
                                                    <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="true" />
                                                    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPagto" runat="server" SelectMethod="GetByCliente"
                                                        TypeName="Glass.Data.DAL.ParcelasDAO">
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="hdfIdCliente" Name="idCliente" PropertyName="Value"
                                                                Type="UInt32" />
                                                            <asp:Parameter DefaultValue="2" Name="tipo" Type="Object" />
                                                        </SelectParameters>
                                                    </colo:VirtualObjectDataSource>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" style="height: 23px">
                <asp:Button ID="btnConfirmar" runat="server" Text="Liberar Pedidos" OnClientClick="confirmar(this); return false;"
                    Visible="False" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfRecarregarTabelaPedido" runat="server" />
                <asp:HiddenField ID="hdfValorCredito" runat="server" />
                <asp:HiddenField ID="hdfIdsPedido" runat="server" />
                <asp:HiddenField ID="hdfIdsProdutosPedido" runat="server" />
                <asp:HiddenField ID="hdfIdsProdutoPedidoProducao" runat="server" />
                <asp:HiddenField ID="hdfQtdeProdutosLiberar" runat="server" />
                <asp:HiddenField ID="hdfIdCliente" runat="server" />
                <asp:HiddenField ID="hdfTotalASerPago" runat="server" />
                <asp:HiddenField ID="hdfValorASerPagoPrazo" runat="server" />
                <asp:HiddenField ID="hdfValorObra" runat="server" />
                <asp:HiddenField ID="hdfCxDiario" runat="server" />
                <asp:HiddenField ID="hdfValorEntrada" runat="server" />
                <asp:HiddenField ID="hdfIdsPedidosRem" runat="server" />
                <asp:HiddenField ID="hdfIsGarantiaReposicao" runat="server" />
                <asp:HiddenField ID="hdfIsPedidoFuncionario" runat="server" />
                <asp:HiddenField ID="hdfLibParc" runat="server" />
                <asp:HiddenField ID="hdfBloqueioTipoVenda" runat="server" />
                <asp:HiddenField ID="hdfBloqueioIdFormaPagto" runat="server" />
                <asp:HiddenField ID="hdfExibirParcelas" runat="server" Value="true" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedidos" runat="server" SelectMethod="GetForLiberacao"
                    TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfBuscarIdsPedidos" Name="idsPedidos" PropertyName="Value"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacaoProd" runat="server"
                    SelectMethod="GetSituacaoProducao" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedidoFilter"
                    TypeName="Glass.Data.Helper.DataSources">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="false" Name="incluirProducao" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        tipoPagtoChanged();

        if (document.getElementById("<%= ctrlParcelasSelecionar1.ClientID %>_drpParcelas") != null)
            parcelasChanged();

        FindControl("txtNumPedido", "input").focus();

        if (!descontoLiberacao) {
            var dadosDesconto = document.getElementById("dadosDesconto");
            if (dadosDesconto != null) dadosDesconto.style.display = "none";
        }

        alterouProduto(true);

    </script>

</asp:Content>
