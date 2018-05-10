function podeSelecionarObra() {
    if (FindControl("hdfCliente", "input").value == "") {
        alert("Selecione um cliente antes de selecionar a obra.");
        return false;
    }
    else
        return true;
}

function mensagemProdutoComDesconto(editar) {
    alert("Não é possível " + (editar ? "editar" : "remover") + " esse produto porque o pedido possui desconto.\n" +
        "Aplique o desconto apenas ao terminar o cadastro dos produtos.\n" +
        "Para continuar, remova o desconto do pedido.");
}

function alteraDataPedidoFunc(controle) {
    if (controle == null)
        return;

    var txt = FindControl("txtDataPed", "input");
    var hdf = FindControl("hdfDataPedido", "input");

    var data = CadPedido.GetAtrasoFuncionario(controle.value).value;
    txt.value = data;
    hdf.value = data;

    alteraDataEntrega(false);
}

function alteraDataEntrega(forcarAlteracao) {
    var idCli = FindControl("txtNumCli", "input").value;
    var tipoEntrega = FindControl("ddlTipoEntrega", "select").value;
    var isFastDelivery = FindControl("chkFastDelivery", "input");
    var dataBase = var_IdPedido != "" ? "" : FindControl("txtDataPed", "input").value;
    isFastDelivery = isFastDelivery != null ? isFastDelivery.checked : "false";
    var campoDataEntrega = FindControl("ctrlDataEntrega_txtData", "input");
    var tipoPedido = FindControl("hdfTipoPedido", "input") != null ? FindControl("hdfTipoPedido", "input").value : FindControl("drpTipoPedido", "select").value;

    var dataEntrega = CadPedido.GetDataEntrega(idCli, var_IdPedido, tipoPedido, tipoEntrega, dataBase, isFastDelivery).value.split(';');
    if (dataEntrega[0] != "") {
        // Altera a data de entrega somente se o campo data entrega estiver vazio, ou se a data preenchida atualmente for menor do que a 
        // data mínima permitida ou se o método tiver sido chamado forçando a alteração porém sem estar no load da página.
        if (campoDataEntrega.value == "" || firstGreaterThenSec(dataEntrega[0], campoDataEntrega.value) || (forcarAlteracao && !var_Loading)) {
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

function controlarVisibilidadeProducaoCorte() {
    if (FindControl("drpTipoPedido", "select") == null)
        return;

    var tipoPedido = FindControl("drpTipoPedido", "select").value;

    var chkGerarPedidoProducaoCorte = FindControl("chkGerarPedidoProducaoCorte", "input");
    var divGerarPedidoProducaoCorte = FindControl("divGerarPedidoProducaoCorte", "div");

    if (chkGerarPedidoProducaoCorte != null && (tipoPedido != '2' || !config_GerarPedidoProducaoCorte)) {
        chkGerarPedidoProducaoCorte.checked = false;
        divGerarPedidoProducaoCorte.style.display = 'none';
        chkGerarPedidoProducaoCorte.parentNode.style.display = 'none';
    }
    else {
        divGerarPedidoProducaoCorte.style.display = '';
        chkGerarPedidoProducaoCorte.parentNode.style.display = '';
    }
}

function loadAjax(tipo) {
    if (!config_BloquearDadosClientePedido && !config_UsarControleDescontoFormaPagamentoDadosProduto) {
        return null;
    }

    // O cliente não deve ser informado ao método caso a configuração de bloqueio de dados do cliente no pedido esteja desabilitada.
    var idCli = config_BloquearDadosClientePedido && FindControl("txtNumCli", "input") != null ? FindControl("txtNumCli", "input").value : "";
    // O tipo de venda do pedido não deve ser informado caso o controle de desconto por forma de pagamento e dados do produto esteja desabilitado.
    var tipoVenda = config_UsarControleDescontoFormaPagamentoDadosProduto && FindControl("drpTipoVenda", "select") != null ? FindControl("drpTipoVenda", "select").value : "";

    var retorno = CadPedido.LoadAjax(tipo, idCli, tipoVenda);

    if (retorno.error != null) {
        alert(retorno.error.description);
        return null;
    }
    else if (retorno.value == null) {
        alert("Falha de Ajax ao carregar tipo '" + tipo + "'.");
    }

    return retorno.value;
}

function atualizaTipoVendaCli() {
    var ajax = loadAjax("tipoVenda");

    if (ajax == null || FindControl("drpTipoVenda", "select") == null) {
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
function atualizaFormasPagtoCli() {
    var drpFormaPagto = FindControl("drpFormaPagto", "select");

    // Verifica se o controle de forma de pagamento existe na tela.
    if (drpFormaPagto == null) {
        return true;
    }

    // Salva em uma variável a forma de pagamento selecionada, antes do recarregamento das opções da Drop Down List.
    var idFormaPagtoAtual = drpFormaPagto.value;
    // Recupera as opções de forma de pagamento disponíveis.
    var ajax = loadAjax("formaPagto");

    // Verifica se ocorreu algum erro na chamada do Ajax.
    if (ajax.error != null) {
        alert(ajax.error.description);
        return false;
    }
    else if (ajax == null) {
        return false;
    }

    // Atualiza a Drop Down List com as formas de pagamento disponíveis.
    drpFormaPagto.innerHTML = ajax;

    // Variável criada para informar se a forma de pagamento pré-selecionada existe nas opções atuais da Drop Down List de forma de pagamento.
    var formaPagtoEncontrada = false;

    // Percorre cada forma de pagamento atual e verifica se a opção pré-selecionada existe entre elas.
    for (var i = 0; i < drpFormaPagto.options.length; i++) {
        if (drpFormaPagto.options[i].value == idFormaPagtoAtual) {
            formaPagtoEncontrada = true;
            break;
        }
    }

    // Caso a forma de pagamento exista nas opções atuais, seleciona ela na Drop.
    if (formaPagtoEncontrada) {
        drpFormaPagto.value = idFormaPagtoAtual;
    }

    drpFormaPagto.onchange();
}

function atualizaValMin() {
    if (parseFloat(FindControl("hdfTamanhoMaximoObra", "input").value.replace(",", ".")) == 0) {
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

function obrigarProcApl() {
    var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
    var isVidroRoteiro = dadosProduto.Grupo == 1 && config_UtilizarRoteiroProducao;
    var tipoCalculo = FindControl("hdfTipoCalc", "input") != null && FindControl("hdfTipoCalc", "input") != undefined && FindControl("hdfTipoCalc", "input").value != undefined ? FindControl("hdfTipoCalc", "input").value : "";

    if (dadosProduto.IsChapaVidro)
        return true;

    /* Chamado 63268. */
    if ((tipoCalculo != "" && (tipoCalculo == "2" || tipoCalculo == "10")) && (isVidroRoteiro || (config_ObrigarProcApl && isVidroBenef))) {
        if (FindControl("txtAplIns", "input") != null && FindControl("txtAplIns", "input").value == "") {
            if (isVidroRoteiro && !config_ObrigarProcApl) {
                alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                return false;
            }

            alert("Informe a aplicação.");
            return false;
        }

        if (FindControl("txtProcIns", "input") != null && FindControl("txtProcIns", "input").value == "") {
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

function calculaTamanhoMaximo() {
    if (FindControl("lblCodProdIns", "span") == null)
        return;

    var codInterno = FindControl("lblCodProdIns", "span").innerHTML;
    var totM2 = FindControl("lblTotM2Ins", "span").innerHTML;
    var idProdPed = FindControl("hdfProdPed", "input") != null ? FindControl("hdfProdPed", "input").value : 0;

    var tamanhoMaximo = CadPedido.GetTamanhoMaximoProduto(var_IdPedido, codInterno, totM2, idProdPed).value.split(";");
    tamanhoMaximo = tamanhoMaximo[0] == "Ok" ? parseFloat(tamanhoMaximo[1].replace(",", ".")) : 0;

    FindControl("hdfTamanhoMaximoObra", "input").value = tamanhoMaximo;
}

function validaTamanhoMax() {
    var tamanhoMaximo = parseFloat(FindControl("hdfTamanhoMaximoObra", "input").value.replace(",", "."));
    if (tamanhoMaximo > 0) {
        var totM2 = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(",", "."));
        if (totM2 > tamanhoMaximo) {
            alert("O total de m² da peça ultrapassa o máximo definido no pagamento antecipado. Tamanho máximo restante: " + tamanhoMaximo.toString().replace(".", ",") + " m²");
            return false;
        }
    }

    return true;
}

function exibirBenef(botao, idProdPed) {
    for (iTip = 0; iTip < 2; iTip++) {
        TagToTip('tbConfigVidro_' + idProdPed, FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true,
            CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
            FIX, [botao, 9 - getTableWidth('tbConfigVidro_' + idProdPed), -41 - getTableHeight('tbConfigVidro_' + idProdPed)]);
    }
}

function calcularDesconto(tipoCalculo) {
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
    var descontoMaximo = CadPedido.PercDesconto(var_IdPedido, alterou, FindControl("drpParcelas", "select").value).value.replace(',', '.');

    //Busca o Desconto por parcela ou por Forma de pagamento e dados do produto
    var retDesconto = 0;

    if (config_UsarDescontoEmParcela && FindControl("drpParcelas", "select") != null) {
        retDesconto = CadPedido.VerificaDescontoParcela(FindControl("drpParcelas", "select").value, var_IdPedido);
    }
    else if (config_UsarControleDescontoFormaPagamentoDadosProduto) {
        var tipoVenda = FindControl("drpTipoVenda", "select") != null ? FindControl("drpTipoVenda", "select").value : "";
        var idFormaPagto = FindControl("drpFormaPagto", "select") != null ? FindControl("drpFormaPagto", "select").value : "";
        var idTipoCartao = FindControl("drpTipoCartao", "select") != null ? FindControl("drpTipoCartao", "select").value : "";
        var idParcela = FindControl("drpParcelas", "select") != null ? FindControl("drpParcelas", "select").value : "";

        retDesconto = CadPedido.VerificaDescontoFormaPagtoDadosProduto(var_IdPedido, tipoVenda, idFormaPagto, idTipoCartao, idParcela);
    }

    if (retDesconto.error != null) {
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
    if (retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "" && retDesconto.value != undefined && parseFloat(retDesconto.value.replace(",", ".")) == parseFloat((desconto + descontoSomar).toFixed(2))) {
        return true;
    }

    if (parseFloat((desconto + descontoSomar).toFixed(2)) > parseFloat(descontoMaximo) && !var_Loading) {
        var mensagem = "O desconto máximo permitido é de " + (tipo == 1 ? descontoMaximo + "%" : "R$ " + valorDescontoMaximo.toFixed(2).replace('.', ',')) + ".";
        if (descontoProdutos > 0)
            mensagem += "\nO desconto já aplicado aos produtos é de " + (tipo == 1 ? descontoProdutos + "%" : "R$ " + valorDescontoProdutos.toFixed(2).replace('.', ',')) + ".";

        if (descontoPedido > 0)
            mensagem += "\nO desconto já aplicado ao pedido é de " + (tipo == 1 ? descontoOrcamento + "%" : "R$ " + valorDescontoPedido.toFixed(2).replace('.', ',')) + ".";

        alert(mensagem);
        controle.value = tipo == 1 ? (descontoMaximo - descontoSomar).toFixed(2).replace('.', ',') : (valorDescontoMaximo - valorDescontoSomar).toFixed(2).replace('.', ',');

        if (parseFloat(controle.value.replace(',', '.')) < 0)
            controle.value = "0";

        return false;
    }

    return true;
}

function alteraFastDelivery(isFastDelivery) {
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

function limparComissionado() {
    FindControl("hdfIdComissionado", "input").value = "";
    FindControl("lblComissionado", "span").innerHTML = "";
    FindControl("txtPercentual", "input").value = "0";
    FindControl("txtValorComissao", "input").value = "R$ 0,00";
}

function getProduto() {
    openWindow(450, 700, '../Utils/SelProd.aspx?IdPedido=' + var_IdPedido + (var_ProdutoAmbiente ? "&ambiente=true" : ""));
}

function verificaDataEntrega(controle) {
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

    if (isDataMinima && !fastDelivery && dataAtual < dataMinima) {
        alert("Não é possível escolher uma data anterior a " + textoDataMinima + ".");
        controle.value = textoDataMinima;
        var_DataEntregaAntiga = textoDataMinima;
        return false;
    }
    else if (MetodosAjax.IsDiaUtil(dataAtual).value == "false") {
        alert("Não é possível escolher sábado, domingo ou feriado como dia de entrega.");
        controle.value = var_DataEntregaAntiga;
        return false;
    }
    else
        var_DataEntregaAntiga = controle.value;

    return true;
}

function getNomeControleBenef() {
    var nomeControle = FindControl("ctrlBenefEditar", "input") != null ? "ctrlBenefEditar" : "ctrlBenefInserir";
    nomeControle = FindControl(nomeControle + "_tblBenef", "table");

    if (nomeControle == null)
        return null;

    nomeControle = nomeControle.id;
    return nomeControle.substr(0, nomeControle.lastIndexOf("_"));
}

function setValorTotal(valor, custo) {
    if (getNomeControleBenef() != null) {
        if (exibirControleBenef(getNomeControleBenef())) {
            var lblValorBenef = FindControl("lblValorBenef", "span");
            lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
        }
    }
}

function setObra(idCliente, idObra, descrObra, saldo) {
    FindControl("hdfIdObra", "input").value = idObra;
    FindControl("txtObra", "input").value = descrObra;
    FindControl("lblSaldoObra", "span").innerHTML = saldo.replace(/\n/g, "<br />");

    if (idCliente > 0) {
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
function getPercComissao() {
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

        if (FindControl("_divDescontoQtde", "div") != null) {
            controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
            if (controleDescQtde != null)
                percDescontoQtde = controleDescQtde.PercDesconto();
        }

        var retorno = CadPedido.GetProduto(var_IdPedido, codInterno, tipoEntrega, cliRevenda, idCliente,
            percComissao, tipoPedido, tipoVenda, var_ProdutoAmbiente, percDescontoQtde, FindControl("hdfLoja", "input").value, false).value.split(';');

        if (!manterProcessoAplicacao && FindControl("txtProcIns", "input") != null)
            FindControl("txtProcIns", "input").value = "";

        if (tipoPedido == 2 && CadPedido.GerarPedidoProducaoCorte(var_IdPedido).value == "true") {
            var tipoSubGrupo = CadPedido.ObterSubgrupoProd(codInterno);
            if (tipoSubGrupo.value != "1") {
                alert('Esse produto não pode ser utilizado, pois não pertence ao Sub-Grupo Chapas de Vidro.');
                return false;
            }
        }

        var verificaProduto = CadPedido.IsProdutoObra(var_IdPedido, codInterno, false).value.split(";");
        if (verificaProduto[0] == "Erro") {
            if (FindControl("txtCodProd", "input") != null)
                FindControl("txtCodProd", "input").value = "";

            alert("Esse produto não pode ser usado no pedido. " + verificaProduto[1]);
            return false;
        }
        else if (parseFloat(verificaProduto[1].replace(",", ".")) > 0) {
            if (txtValor != null)
                txtValor.disabled = true;

            // Se for edição de produto, chamad o método padrão de cálculo da metragem máxima permitida
            if (FindControl("hdfProdPed", "input") != null)
                calculaTamanhoMaximo();
            else if (FindControl("hdfTamanhoMaximoObra", "input") != null)
                FindControl("hdfTamanhoMaximoObra", "input").value = verificaProduto[2];
        }
        else {
            if (txtValor != null)
                txtValor.disabled = verificaProduto[3] == "false";

            if (FindControl("hdfTamanhoMaximoObra", "input") != null)
                FindControl("hdfTamanhoMaximoObra", "input").value = "0";
        }

        var idLojaSubgrupo = CadPedido.ObterLojaSubgrupoProd(codInterno);
        var idLoja = FindControl("hdfLoja", "input").value;

        if (idLojaSubgrupo.error != null) {

            if (FindControl("txtCodProd", "input") != null)
                FindControl("txtCodProd", "input").value = "";

            alert(idLojaSubgrupo.error.description);
            return false;
        }

        if (idLojaSubgrupo.value != "0" && idLojaSubgrupo.value != idLoja) {

            if (FindControl("txtCodProd", "input") != null)
                FindControl("txtCodProd", "input").value = "";

            alert('Esse produto não pode ser utilizado, pois a loja do seu subgrupo é diferente da loja do pedido.');
            return false;
        }

        var validaClienteSubgrupo = MetodosAjax.ValidaClienteSubgrupo(FindControl("hdfIdCliente", "input").value, codInterno);
        if (validaClienteSubgrupo.error != null) {

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

        else if (!var_ProdutoAmbiente) {
            if (retorno[0] == "Prod") {
                FindControl("hdfIdProd", "input").value = retorno[1];

                // Caso o vendedor não possa alterar o valor vendido do produto OU o valor vendido do produto seja zero ou o valor vendido do produto seja menor que o valor de tabela,
                // atualiza o valor da obra ou de tabela do produto.
                if (verificaProduto[3] == "false" || txtValor.value == "" || parseFloat(txtValor.value.toString().replace(",", ".")) == 0 || parseFloat(txtValor.value.toString().replace(",", ".")) < parseFloat(retorno[3].toString().replace(",", "."))) {
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

            if (FindControl("txtComissaoProd", "input") != null)
                FindControl("txtComissaoProd", "input").value = retorno[22] != undefined ? retorno[22] : retorno[21];
        }
        else {
            FindControl("hdfAmbIdProd", "input").value = retorno[1];
            FindControl("lblDescrAmb", "span").innerHTML = retorno[2];
            FindControl("hdfDescrAmbiente", "input").value = retorno[2];
        }
    }
    catch (err) {
        alert(err);
    }

    if (FindControl("hdfIdItemProjeto", "input") != null && FindControl("hdfIdItemProjeto", "input").value > 0) {
        if (FindControl("txtQtdeIns", "input") != null)
            FindControl("txtQtdeIns", "input").disabled = true;
        if (FindControl("txtAlturaIns", "input") != null)
            FindControl("txtAlturaIns", "input").disabled = true;
        if (FindControl("txtLargura", "input") != null)
            FindControl("txtLargura", "input").disabled = true;
        if (FindControl("txtValorIns", "input") != null)
            FindControl("txtValorIns", "input").disabled = true;
        if (FindControl("txtProcIns", "input") != null) {
            FindControl("txtProcIns", "input").disabled = true;
            FindControl("lnkProcesso", "a").style.display = "none";
        }
        if (FindControl("txtAplIns", "input") != null) {
            FindControl("txtAplIns", "input").disabled = true;
            FindControl("lnkAplicacao", "a").style.display = "none";
        }
        if (FindControl("drpLargBenef", "select") != null)
            FindControl("drpLargBenef", "select").disabled = true;
        if (FindControl("lnkBenef", "a") != null)
            FindControl("lnkBenef", "a").style.display = "none";
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
    else if (isCalcM2) {
        if (totM2 == "")
            return;

        txtQtd = totM2;
    }

    var estoqueMenor = txtQtd != "" && parseInt(txtQtd) > parseInt(var_QtdEstoque);
    if (estoqueMenor) {
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
    if (verificaEtiquetaApl.error != null) {

        if (!var_AplAmbiente) {
            FindControl("txtAplIns", "input").value = "";
            FindControl("hdfIdAplicacao", "input").value = "";
        }
        else {
            FindControl("txtAmbAplIns", "input").value = "";
            FindControl("hdfAmbIdAplicacao", "input").value = "";
        }

        alert(verificaEtiquetaApl.error.description);
        return false;
    }

    if (!var_AplAmbiente && FindControl("txtAplIns", "input") != null) {
        FindControl("txtAplIns", "input").value = codInterno;
        FindControl("hdfIdAplicacao", "input").value = idAplicacao;
    }
    else if (FindControl("txtAmbAplIns", "input") != null && FindControl("hdfAmbIdAplicacao", "input") != null) {
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

    if (idSubgrupo.value != "" && retornoValidacao.value == "False" && (FindControl("txtProcIns", "input") != null && FindControl("txtProcIns", "input").value != "")) {
        FindControl("txtProcIns", "input").value = "";
        alert("Este processo não pode ser selecionado para este produto.")
        return false;
    }

    var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, FindControl("hdfIdPedido", "input").value);
    if (verificaEtiquetaProc.error != null) {

        if (!var_ProcAmbiente && FindControl("txtProcIns", "input") != null) {
            FindControl("txtProcIns", "input").value = "";
            FindControl("hdfIdProcesso", "input").value = "";
        }
        else if (FindControl("txtAmbProcIns", "input") != null && FindControl("hdfAmbIdProcesso", "input") != null) {
            FindControl("txtAmbProcIns", "input").value = "";
            FindControl("hdfAmbIdProcesso", "input").value = "";
        }

        setApl("", "");

        alert(verificaEtiquetaProc.error.description);
        return false;
    }

    if (!var_ProcAmbiente && FindControl("txtProcIns", "input") != null) {
        FindControl("txtProcIns", "input").value = codInterno;
        FindControl("hdfIdProcesso", "input").value = idProcesso;

        if (FindControl("txtCodProdIns", "input") != null)
            codInternoProd = FindControl("txtCodProdIns", "input").value;
        else
            codInternoProd = FindControl("lblCodProdIns", "span").innerHTML;

        codAplicacaoAtual = FindControl("txtAplIns", "input").value;
    }
    else if (FindControl("txtAmbProcIns", "input") != null && FindControl("hdfAmbIdProcesso", "input") != null) {
        FindControl("txtAmbProcIns", "input").value = codInterno;
        FindControl("hdfAmbIdProcesso", "input").value = idProcesso;

        codInternoProd = FindControl("txtCodAmb", "input").value;
        codAplicacaoAtual = FindControl("txtAmbAplIns", "input").value;
    }

    if (((codAplicacao && codAplicacao != "") ||
        (codInternoProd != "" && CadPedido.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) &&
        (codAplicacaoAtual == null || codAplicacaoAtual == "")) {
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

    if (control == null || formaPagto == null) {
        return;
    }

    // Se for à vista e o controle de desconto por forma de pagamento estiver habilitado, esconde somente a parcela.
    if (config_UsarControleDescontoFormaPagamentoDadosProduto && control.value == 1) {
        formaPagto.style.display = "";

        if (parcela != null) {
            parcela.selectedIndex = 0;
            parcela.style.display = "none";
        }
    }
        // Se for obra, à vista, funcionário ou se estiver vazio, esconde a forma de pagamento e a parcela.
    else if (control.value == 0 || control.value == 1 || control.value == 5 || control.value == 6) {
        formaPagto.selectedIndex = 0;
        formaPagto.style.display = "none";

        if (parcela != null) {
            parcela.selectedIndex = 0;
            parcela.style.display = "none";
        }
    }
    else {
        formaPagto.style.display = "";

        if (parcela != null) {
            parcela.style.display = "";
        }
    }
}

function exibirEntrada(tipoVenda) {
    return tipoVenda == "" || tipoVenda == 2 || (tipoVenda == 1 && config_LiberarPedido);
}

// Evento acionado ao trocar o tipo de venda (à vista/à prazo)
function tipoVendaChange(control, calcParcelas) {
    if (control == null)
        return;

    formaPagtoVisibility();

    // Ao alterar o tipo de venda, as formas de pagamento devem ser recarregadas para que o controle de desconto por forma de pagamento e dados do produto funcione corretamente.
    if (config_UsarControleDescontoFormaPagamentoDadosProduto) {
        atualizaFormasPagtoCli();
    }

    formaPagtoChanged();

    document.getElementById("divObra").style.display = parseInt(control.value) == 5 ? "" : "none";
    document.getElementById("funcionarioComprador").style.display = parseInt(control.value) == 6 ? "" : "none";

    var valorEntrada = document.getElementById("tdValorEntrada2").getElementsByTagName("input")[0];

    if (!exibirEntrada(control.value)) {
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

function verificarDescontoFormaPagtoDadosProduto() {
    var tipoVenda = FindControl("drpTipoVenda", "select");
    var formaPagto = FindControl("drpFormaPagto", "select");
    var tipoCartao = FindControl("drpTipoCartao", "select");
    var parcelas = FindControl("drpParcelas", "select");

    var retDesconto = CadPedido.VerificaDescontoFormaPagtoDadosProduto(var_IdPedido, tipoVenda != null ? tipoVenda.value : "", formaPagto != null ? formaPagto.value : "",
        tipoCartao != null ? tipoCartao.value : "", parcelas != null ? parcelas.value : "");

    if (retDesconto.error != null) {
        alert(retDesconto.error.description);
        return false;
    }
    else if (retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "") {
        var txtDesconto = FindControl("txtDesconto", "input");
        var txtTipoDesconto = FindControl("drpTipoDesconto", "select");

        if (txtTipoDesconto != null) {
            txtTipoDesconto.value = 1;
        }

        if (txtDesconto != null) {
            txtDesconto.value = retDesconto.value.replace(".", ",");
            txtDesconto.onchange();
            txtDesconto.onblur();
        }
    }
}

function showHideDesconto(exibirDesconto) {
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

function callbackSetParcelas() {
    setParcelas(true);
    if (document.getElementById(var_NomeControleParcelas + "_tblParcelas") != null)
        Parc_visibilidadeParcelas(var_NomeControleParcelas);

    // Verifica se a empresa permite desconto para pedidos à vista com uma parcela
    if (config_PermitirDescontoAVistaComUmaParcela)
        showHideDesconto(FindControl("hdfNumParcelas", "input").value == "1" || FindControl("drpTipoVenda", "select").value == "1");
}

function setParcelas(calcParcelas) {
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
function formaPagtoChanged() {
    var formaPagto = FindControl("drpFormaPagto", "select");
    var tipoCartao = FindControl("drpTipoCartao", "select");

    if (formaPagto == null) {
        return true;
    }

    if (tipoCartao != null) {
        // Caso a forma de pagamento atual não seja Cartão, esconde o controle de tipo de cartão e desmarca a opção selecionada.
        if (formaPagto.value != var_CodCartao) {
            tipoCartao.style.display = "none";
            tipoCartao.selectedIndex = 0;
        }
        else {
            tipoCartao.style.display = "";
        }
    }
}

/*
*   Função chamada ao inserir e atualizar pedido
*/
function validarPedido(controle) {
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
    if (tipoPedido == "" || tipoPedido == "0") {
        alert("Selecione o tipo do pedido.");
        controle.disabled = false;
        return false;
    }

    if (drpTipoVenda != null) {
        // Se o tipo venda não for a vista, obra ou funcionário, obriga a selecionar forma de pagto.
        var tipoVenda = parseInt(drpTipoVenda.value);

        if (FindControl("drpFormaPagto", "select") == null || FindControl("drpFormaPagto", "select").value == "") {
            // Caso o controle de desconto por forma de pagamento e dados do produto esteja habilitado e o tipo de venda do pedido seja à vista, obriga o usuário a informar a forma de pagamento.
            if (config_UsarControleDescontoFormaPagamentoDadosProduto && tipoVenda == 1) {
                alert("Selecione a forma de pagamento.");
                controle.disabled = false;
                return false;
            }
            else if (tipoVenda != 1 && tipoVenda != 5 && tipoVenda != 6) {
                alert("Selecione a forma de pagamento.");
                controle.disabled = false;
                return false;
            }
        }

        if (config_UsarControleObraComProduto) {
            var tipoVendaAtual = FindControl("hdfTipoVendaAtual", "input");

            if (tipoVendaAtual != null && tipoVendaAtual.value != 5 && tipoVenda == 5 && var_QtdProdutosPedido > 0) {
                alert("Não é possível escolher obra como forma de pagamento se o pedido tiver algum produto cadastrado.");
                controle.disabled = false;
                return false;
            }
            else if (tipoVendaAtual != null && tipoVendaAtual.value == 5 && tipoVenda != 5 && var_QtdProdutosPedido > 0) {
                alert("Não é possível que a forma de pagamento do pedido não seja mais obra se houver algum produto cadastrado.");
                controle.disabled = false;
                return false;
            }
        }

        if (tipoVenda == 6 && FindControl("drpFuncVenda", "select").value == "") {
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
    if (tipoEntrega == "") {
        alert("Selecione o tipo de entrega.");
        controle.disabled = false;
        return false;
    }
    else if (tipoEntrega != 1) {
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
        if (obraCliente != null && obraCliente.toLowerCase() == "false") {
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
function habilitarCamposAposInsercaoAtualizacaoPedido(controle) {
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

    if (var_Inserting) {
        controle.disabled = false;
        return false;
    }

    if (!validarPedido(controle))
        return false;

    var podeInserir = CadPedido.PodeInserir(FindControl("hdfCliente", "input").value).value.split(';');
    if (parseInt(podeInserir[0], 10) > 0) {
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
    if (FindControl("hdfClienteAtual", "input") != null) {
        var clienteAtual = FindControl("hdfClienteAtual", "input").value;
        var clienteNovo = FindControl("txtNumCli", "input").value;
        var alterar = clienteAtual != clienteNovo ? confirm("O cliente foi alterado no pedido. Deseja atualizar o projeto?") : false;
        FindControl("hdfAlterarProjeto", "input").value = alterar;
    }

    try {
        // Verifica o prazo e a urgência do pedido
        if (!verificarDatas())
            return false;
    }
    catch (err) {
        alert("Falha ao verificar datas. " + err);
        return false;
    }

    // Verifica forma de pagamento cartão, se não for seta tipo cartao nulo
    var formaPagto = FindControl("drpFormaPagto", "select");
    if (formaPagto && formaPagto.value != var_CodCartao)
        FindControl("drpTipoCartao", "select").value = "";

    habilitarCamposAposInsercaoAtualizacaoPedido(controle);

    return true;
}

/*
*   Função chamada ao inserir e atualizar produto no pedido
*/
function validarProduto() {
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
        var_SaveProdClicked = false;
        return false;
    }

    if (qtde == "0" || qtde == "") {
        alert("Informe a quantidade.");
        var_SaveProdClicked = false;
        return false;
    }

    if (FindControl("txtAlturaIns", "input").disabled == false && (altura == "" || parseFloat(altura.replace(",", ".")) == 0)) {
        alert("Informe a altura.");
        var_SaveProdClicked = false;
        return false;
    }

    if (FindControl("txtLarguraIns", "input").disabled == false && largura == "") {
        alert("Informe a largura.");
        var_SaveProdClicked = false;
        return false;
    }

    valMin = new Number(valMin.replace(',', '.'));
    if (!FindControl("txtValorIns", "input").disabled && new Number(valor.replace(',', '.')) < valMin) {
        alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
        var_SaveProdClicked = false;
        return false;
    }

    // Verifica se foi clicado no aplicar na telinha de beneficiamentos
    if (tbConfigVidro != null && FindControl("tbConfigVidro", "table").style.display == "block") {
        alert("Aplique as alterações no beneficiamento antes de salvar o item.");
        var_SaveProdClicked = false;
        return false;
    }

    if (!obrigarProcApl()) {
        var_SaveProdClicked = false;
        return false;
    }

    if (!validaTamanhoMax()) {
        var_SaveProdClicked = false;
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

    if (exibirControleBenef(nomeControle)) {
        var resultadoVerificacaoObrigatoriedade = verificarObrigatoriedadeBeneficiamentos(dadosProduto.ID);
        var_SaveProdClicked = resultadoVerificacaoObrigatoriedade;
        return resultadoVerificacaoObrigatoriedade;
    }

    return true;
}

// Chamado quando um produto está para ser inserido no pedido
function onInsertProd() {
    if (var_SaveProdClicked == true)
        return false;

    var_SaveProdClicked = true;

    if (FindControl("txtCodProdIns", "input").value == "") {
        alert("Informe o código do produto.");
        var_SaveProdClicked = false;
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
            (numBenef != dadosCalcM2Prod.NumBenef)) {
            var isPedProducaoCorte = CadPedido.IsPedidoProducaoCorte(var_IdPedido);
            if (isPedProducaoCorte.error != null) {
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

function getCli(idCliente) {
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
    if (!var_Loading) {
        FindControl("txtEnderecoObra", "input").value = "";
        FindControl("txtBairroObra", "input").value = "";
        FindControl("txtCidadeObra", "input").value = "";
    }

    if (!var_Loading) {
        if (retorno[5] == "true" && !entregaBalcao) {
            if (var_TipoEntregaEntrega != null)
                FindControl("ddlTipoEntrega", "select").value = var_TipoEntregaEntrega;

            setLocalObra(false);
            getEnderecoCli();
        }
    }

    if (config_UsarComissionado) {
        var comissionado = MetodosAjax.GetComissionado("", idCliente).value.split(';');
        setComissionado(comissionado[0], comissionado[1], comissionado[2], undefined, true);
    }

    if (FindControl("hdfPercSinalMin", "input") != null) {
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

    if (FindControl("hdfClienteAtual", "input") != null) {
        var clienteAtual = parseInt(FindControl("hdfClienteAtual", "input").value, 10);
        var clienteNovo = parseInt(FindControl("txtNumCli", "input").value, 10);
        if (retorno[14] != "" && clienteAtual != clienteNovo)
            FindControl("drpTransportador", "select").value = retorno[14];
    }

    if (!var_Loading) {
        if (retorno.length > 13 && retorno[13] != "") {
            FindControl("drpLoja", "select").value = retorno[13];
            FindControl("drpLoja", "select").disabled = !config_AlterarLojaPedido;

            if (FindControl("chkDeveTransferir", "input") != null) {
                FindControl("chkDeveTransferir", "input").checked = true;
                FindControl("chkDeveTransferir", "input").disabled = true;
            }
        }
        else if (FindControl("drpLoja", "select") != null) {
            if (FindControl("chkDeveTransferir", "input") != null) {
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
    if (cEndereco.value == "" && cBairro.value == "" && cCidade.value == "")
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

    if (forcarCarregamentoComissionado || (possuiComissionado == "true" && edicaoComissionado != undefined)) {
        FindControl("lblComissionado", "span").innerHTML = nome;
        FindControl("hdfIdComissionado", "input").value = id;
        FindControl("txtPercentual", "input").value = percentual;
    }
    else if (var_IdPedido == "" || edicaoComissionado != undefined) {
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
function exibirEsconderAmbiente(value) {
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
function finalizarPedido() {
    if (confirm("Finalizar pedido?"))
        return verificarDatas();

    return false;
}

// Função chamada para verificar o prazo de entrega e a urgência do pedido
function verificarDatas() {
    // Verifica a data de entrega
    var dataEntrega = FindControl("ctrlDataEntrega_txtData", "input");

    if (FindControl("lblDataEntrega", "span") != null && FindControl("lblDataEntrega", "span").innerHTML == "") {
        alert("Informe a data de entrega do pedido");
        return false;
    }

    if (!verificaDataEntrega(dataEntrega))
        return false;

    var pedidoFastDelivery = null;

    // Verifica se o pedido é Fast Delivery
    if (config_FastDelivery) {
        pedidoFastDelivery = FindControl("hdfFastDelivery", "input");
        if (pedidoFastDelivery != null)
            pedidoFastDelivery = pedidoFastDelivery.value.toLowerCase() == "true";
        else {
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

function checkPosMateriaPrima() {
    var result = CadPedido.VerificaPosMateriaPrima(var_IdPedido);

    if (result.error != null) {
        alert(result.error.description);
        return true;
    }

    if (result.value.split(';')[0] == "erro") {
        alert(result.value.split(';')[1]);
        return config_BloqEmisPedidoPorPosicaoMateriaPrima;
    }

    return true;
}

function checkCapacidadeProducaoSetor() {
    var editPedido = FindControl("grdProdutos", "table") == null;

    var totM2 = parseFloat(var_TotalM2Pedido);
    var dataEntrega = editPedido ?
        (FindControl("ctrlDataEntrega_txtData", "input") == null ? FindControl("lblDataEntrega", "span").innerHTML : FindControl("ctrlDataEntrega_txtData", "input").value) :
        (FindControl("lblDataEntrega", "span") == null ? FindControl("ctrlDataEntrega_txtData", "input").value : FindControl("lblDataEntrega", "span").innerHTML);
    dataEntrega = dataEntrega.toString().split(' ')[0];
    var idProcesso = FindControl("hdfIdProcesso", "input") != null ? FindControl("hdfIdProcesso", "input").value : 0;

    if (!editPedido) {
        if (FindControl("drpFooterVisible", "select") != null)
            var diferencaM2 = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.'));
        else {
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

    if (dadosResposta[0] == "Erro") {
        alert(dadosResposta[1]);
        return false;
    }

    return true;
}

/*
*   Função chamada para verificar se há Fast Delivery.
*/
function checkFastDelivery() {
    var editPedido = false;

    var fastDelivery = FindControl("hdfFastDelivery", "input");
    if (fastDelivery == null || fastDelivery.value.toLowerCase() == "false") {
        var fastDelivery = FindControl("chkFastDelivery", "input");
        if (fastDelivery == null || !fastDelivery.checked)
            return true;
        else
            editPedido = true;
    }

    var totM2 = parseFloat(var_TotalM2Pedido);
    var dataPedido = var_DataPedido;

    if (!editPedido) {
        if (FindControl("drpFooterVisible", "select") != null)
            diferencaM2 = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.'));
        else {
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

/* 
*   Função utilizada após selecionar medidor no popup, para preencher o id e o nome do mesmo, nas respectivas textboxes deste form
*/
function setMedidor(id, nome) {
    FindControl("hdfIdMedidor", "input").value = id;
    FindControl("lblMedidor", "span").innerHTML = nome;
    return false;
}

function openProjeto(idAmbiente) {
    var tipoEntrega = FindControl("ddlTipoEntrega", "select");
    if (tipoEntrega != null)
        tipoEntrega = tipoEntrega.value;
    else
        tipoEntrega = FindControl("hdfTipoEntrega", "input").value;

    if (tipoEntrega == "") {
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

function PodeConsSitCadContr() {
    var idCli = FindControl("hdfCliente", "input").value;

    if (idCli == "" || CadPedido.PodeConsultarCadastro(idCli).value == "False")
        FindControl("ConsultaCadCliSintegra", "div").style.display = 'none';
    else
        FindControl("ConsultaCadCliSintegra", "div").style.display = 'inline';
}

function AlterouLoja() {
    var idLoja = CadPedido.GetLojaFuncionario().value;

    if (FindControl("chkDeveTransferir", "input") != null) {
        if (FindControl("drpLoja", "select").value != idLoja)
            FindControl("chkDeveTransferir", "input").checked = true;
        else
            FindControl("chkDeveTransferir", "input").checked = false;
    }
}

function buscarProcessos() {
    var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProd", "input").value);
    openWindow(450, 700, "../Utils/SelEtiquetaProcesso.aspx?idSubgrupo=" + idSubgrupo.value);
}

function exibirProdsComposicao(botao, idProdPed) {

    var grdProds = FindControl("grdProdutos", "table");

    if (grdProds == null)
        return;

    for (var i = 0; i < grdProds.rows.length; i++) {

        var row = grdProds.rows[i];
        if (row.id.indexOf("prodPed_") != -1 && row.id.split('_')[1] != idProdPed) {
            row.style.display = "none";
        }
    }

    var linha = document.getElementById("prodPed_" + idProdPed);
    var exibir = linha.style.display == "none";
    linha.style.display = exibir ? "" : "none";
    botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
    botao.title = (exibir ? "Esconder" : "Exibir") + " Produtos da Composição";

    if (FindControl("txtCodProdIns", "input") != null)
        FindControl("txtCodProdIns", "input").parentElement.parentElement.style.display = !exibir ? "" : "none";

    FindControl("hdfProdPedComposicaoSelecionado", "input").value = exibir ? idProdPed : 0;
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

function iniciaPesquisaCepObra(cep) {
    var logradouro = FindControl("txtEnderecoObra", "input");
    var bairro = FindControl("txtBairroObra", "input");
    var cidade = FindControl("txtCidadeObra", "input");
    pesquisarCep(cep, null, logradouro, bairro, cidade, null);
}

function modificarLayoutGridProdutos() {
    // Se a empressa não vende vidros, esconde campos
    if (FindControl("hdfNaoVendeVidro", "input").value == "true" && FindControl("grdProdutos", "table") != null) {
        var tbProd = FindControl("grdProdutos", "table");
        var rows = tbProd.rows;

        var colsTitle = rows[0].getElementsByTagName("th");
        colsTitle[4].style.display = "none";
        colsTitle[5].style.display = "none";
        colsTitle[6].style.display = "none";
        colsTitle[7].style.display = "none";

        var k = 0;
        for (k = 1; k < rows.length; k++) {
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

            var j = 0;
            for (j = 1; j < rows.length; j++) {
                try {
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

/*
*   Carrega dados financeiros do cliente ao editar o pedido
*/
function carregarDadosFinanceirosCliente() {
    var numCli = FindControl("txtNumCli", "input");
    if (numCli != null && numCli.value != "") {
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

        if (formaPagto != null) {
            formaPagto.onchange();

            if (tcc > 0 && formaPagto.value == var_CodCartao)
                tipoCartaoCredito.value = tcc;
        }
    }
}

/*
*   Inicializa os controle da tela
*/
function inicializarControles() {
    // Se estiver editando um item, carrega dados do mesmo na tela
    if (FindControl("lblCodProdIns", "span") != null && FindControl("hdfProdPed", "input") != null)
        loadProduto(FindControl("lblCodProdIns", "span").innerHTML, FindControl("hdfProdPed", "input").value, true);

    // Se tiver definido o último produto inserido, preenche o campo CodInterno com o mesmo
    if (typeof ultimoCodProd != "undefined" && FindControl("txtCodProdIns", "input") != null)
        FindControl("txtCodProdIns", "input").value = ultimoCodProd;

    controlarVisibilidadeProducaoCorte();
    tipoVendaChange(FindControl("drpTipoVenda", "select"), false);
    exibirEsconderAmbiente(false);
    modificarLayoutGridProdutos();
    calculaTamanhoMaximo();
    carregarDadosFinanceirosCliente();
    setLocalObra(false);
    alteraDataPedidoFunc(FindControl("drpVendedorIns", "select"));

    if (FindControl("drpLoja", "select") && !config_AlterarLojaPedido)
        FindControl("drpLoja", "select").disabled = true;

    var parcelas = FindControl("drpParcelas", "select");

    if (parcelas != null) {
        parcelas.onblur = function () {
            //Busca o Desconto por parcela ou por Forma de pagamento e dados do produto
            var retDesconto = null;

            if (config_UsarDescontoEmParcela && parcelas != null) {
                retDesconto = CadPedido.VerificaDescontoParcela(parcelas.value, var_IdPedido);

                if (retDesconto.error != null) {
                    alert(retDesconto.error.description);
                    return false;
                }
                else if (retDesconto != null && retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "") {
                    var txtDesconto = FindControl("txtDesconto", "input");
                    var txtTipoDesconto = FindControl("drpTipoDesconto", "select");

                    if (txtTipoDesconto != null) {
                        txtTipoDesconto.value = 1;
                    }

                    if (txtDesconto != null) {
                        txtDesconto.value = retDesconto.value.replace(".", ",");
                        txtDesconto.onchange();
                        txtDesconto.onblur();
                    }
                }
            }
            else if (config_UsarControleDescontoFormaPagamentoDadosProduto) {
                verificarDescontoFormaPagtoDadosProduto();
            }
        }
    }

    $(document).ready(function () {

        var hdfProdPedComposicaoSelecionado = FindControl("hdfProdPedComposicaoSelecionado", "input");

        if (hdfProdPedComposicaoSelecionado.value > 0) {
            var div = FindControl("imgProdsComposto_" + hdfProdPedComposicaoSelecionado.value, "div");

            if (div == null) return;

            var botao = FindControl("imgProdsComposto", "input", div);
            exibirProdsComposicao(botao, hdfProdPedComposicaoSelecionado.value);
        }
    });

    var_Loading = false;
}