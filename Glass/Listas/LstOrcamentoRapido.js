
var nomeControleBenef = "ctl00_ctl00_Pagina_Conteudo_ctrlBenef1";

var countProd = 1; // Conta a quantidade de produtos adicionados ao orçamento
var totalOrca = 0; // Calcula o total do orçamento

// Guarda a quantidade disponível em estoque do produto buscado
var qtdEstoque = 0;
var exibirMensagemEstoque = false;
var qtdEstoqueMensagem = 0;

// Função chamada após selecionar produto pelo popup
function setProduto(codInterno) {
    try {
        limpaCampos();
        FindControl("txtCodProd", "input").value = codInterno;
        loadProduto(GetQueryString('orcamentoRapido'));
    }
    catch (err) {

    }
}

function atualizaValMin()
{
    var codInterno = FindControl("txtCodProd", "input");

    var tipoEntrega = FindControl("drpTipoEntrega", "select").value;
    var cliRevenda = FindControl("chkRevenda", "input").checked;
    var idCliente = FindControl("hdfIdCliente", "input") != null && FindControl("hdfIdCliente", "input") != undefined ?
        FindControl("hdfIdCliente", "input").value : "0";

    var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
    controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

    var percDescontoQtde = controleDescQtde.PercDesconto();

    FindControl("hdfValMin", "input").value = LstOrcamentoRapido.GetValorMinimo(codInterno.value, idCliente, tipoEntrega, cliRevenda, percDescontoQtde).value;
}

// Carrega dados do produto com base no código do produto passado
function loadProduto(orcamentoRapido) {
    if (FindControl('txtCodProd', 'input').value == "")
        return false;

    try {

        var validaClienteSubgrupo = MetodosAjax.ValidaClienteSubgrupo(FindControl("hdfIdCliente", "input").value, FindControl('txtCodProd', 'input').value);
        if (validaClienteSubgrupo.error != null) {

            if (FindControl("txtCodProd", "input") != null)
                FindControl("txtCodProd", "input").value = "";

            alert(validaClienteSubgrupo.error.description);
            return false;
        }

        var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
        controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

        var percDescontoQtde = controleDescQtde.PercDesconto();

        var retorno = LstOrcamentoRapido.GetProduto(FindControl('txtCodProd', 'input').value,
            FindControl("hdfIdCliente", "input").value, FindControl("hdfIdOrca", "input").value,
            FindControl("drpTipoEntrega", "select").value, FindControl("chkRevenda", "input").checked ? "true" : "false",
            percDescontoQtde, orcamentoRapido == undefined ? true : orcamentoRapido).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            FindControl("txtCodProd", "input").value = "";
            return false;
        }

        FindControl("hdfIdProd", "input").value = retorno[1];
        FindControl("lblDescrProd", "span").innerHTML = retorno[2];
        FindControl("txtValor", "input").value = retorno[3]; // Exibe o valor do produto
        FindControl("hdfValMin", "input").value = retorno[3]; // Salva o valor mínimo
        posValor = retorno[4]; // Posição que deverá ser utilizada para pegar o valor dos itens de transformação do produto
        FindControl("hdfIsVidro", "input").value = retorno[6]; // Guarda no hiddenField se o produto é vidro ou não
        FindControl("hdfIsAluminio", "input").value = retorno[7]; // Guarda no hiddenField se o produto é alumínio ou não
        FindControl("hdfM2Minimo", "input").value = retorno[8]; // Guarda no hiddenField o valor mínimo em m² para a venda do mesmo
        FindControl("hdfTipoCalc", "input").value = retorno[10]; // Armazena o tipo de cálculo que será feito no produto
        FindControl("hdfCustoProd", "input").value = retorno[11]; // Armazena o custo do produto

        var esp = retorno[5] != "" ? parseFloat(retorno[5].replace(",", ".")) : 0;
        if (FindControl("txtEspessura", "input") != null)
        {
            if (esp > 0)
                FindControl("txtEspessura", "input").value = retorno[5]; // Exibe a espessura do produto

            FindControl("txtEspessura", "input").disabled = esp > 0;
        }

        FindControl("hdfAliqIcms", "input").value = retorno[12];

        qtdEstoque = retorno[13];
        exibirMensagemEstoque = retorno[14] == "true";
        qtdEstoqueMensagem = retorno[15];

        calcM2();

        desabilitaCampos(retorno[6] == "true", retorno[10], esp > 0);

        if (retorno[16] != "" && retorno[17] != "")
        {
            FindControl("txtAltura", "input").value = retorno[16];
            FindControl("hdfAlturaCalc", "input").value = retorno[16];
            FindControl("txtLargura", "input").value = retorno[17];
        }

        FindControl("hdfIdProcesso", "input").value = retorno[18];
        FindControl("txtProcIns", "input").value = retorno[19];
        FindControl("hdfIdAplicacao", "input").value = retorno[20];
        FindControl("txtAplIns", "input").value = retorno[21];

        // se produto for temperado, muda a lapidação para 2x2 e desabilita
        // as drops de lapidacao para não serem alteradas
        prodTemperado(retorno[9] == "true", retorno[7] == "true");
    }
    catch (err) {

    }
}

function desabilitaCampos(isVidro, tipoCalculo, desabilitarEspessuraVidro) {
    var tabela = document.getElementById(nomeControleBenef + "_tblBenef");
    var inputs = tabela.getElementsByTagName("input");
    var selects = tabela.getElementsByTagName("select");

    var habilitar = exibirControleBenef(nomeControleBenef);

    for (i = 0; i < inputs.length; i++)
        inputs[i].disabled = !habilitar;

    for (i = 0; i < selects.length; i++)
        selects[i].disabled = !habilitar;

    FindControl("txtAltura", "input").disabled = CalcProd_DesabilitarAltura(tipoCalculo);
    FindControl("txtLargura", "input").disabled = CalcProd_DesabilitarLargura(tipoCalculo);

    if (FindControl("txtEspessura", "input") != null)
        FindControl("txtEspessura", "input").disabled = !(isVidro && !desabilitarEspessuraVidro);
}

function limpaCampos() {
    FindControl("txtAltura", "input").value = "";
    FindControl("txtLargura", "input").value = "";
    if (FindControl("txtEspessura", "input") != null)
        FindControl("txtEspessura", "input").value = "";
    FindControl("txtValor", "input").value = "";
    FindControl("txtQtde", "input").value = "";
    FindControl("lblTotM2", "span").innerHTML = "";
    FindControl("lblTotM2Calc", "span").innerHTML = "";
    FindControl("hdfTotM2", "input").value = "";
    FindControl("hdfTotM2SemChapa", "input").value = "";
    FindControl("txtProcIns", "input").value = "";
    FindControl("txtAplIns", "input").value = "";
}

// Se o produto for temperado, muda a lapidação para 2x2 e desabilita
// as drops de lapidacao para não serem alteradas
function prodTemperado(temperado, aluminio) {
    if (temperado) {
        FindControl("Lapidacao_drpAltura", "select").value = 2;
        FindControl("Lapidacao_drpLargura", "select").value = 2;
    }

    var desabilitado = temperado;
    if (typeof aluminio != 'undefined' && aluminio != null)
        desabilitado = temperado || aluminio;

    // Desabilita campos se produto for temperado
    FindControl("Lapidacao_drpAltura", "select").disabled = desabilitado;
    FindControl("Lapidacao_drpLargura", "select").disabled = desabilitado;
}

// Calcula em tempo real a metragem quadrada do produto
function calcM2() {
    try {
        var idProd = FindControl("hdfIdProd", "input").value;
        var altura = FindControl("txtAltura", "input").value;
        var largura = FindControl("txtLargura", "input").value;

        var qtde = FindControl("txtQtde", "input").value;
        var isVidro = FindControl("hdfIsVidro", "input").value == "true";
        var tipoCalc = FindControl("hdfTipoCalc", "input").value;
        var esp = FindControl("txtEspessura", "input") != null ? FindControl("txtEspessura", "input").value : 0;
        var idCliente = FindControl("hdfIdCliente", "input").value;

        var redondo = FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked;

        if (altura != "" && largura != "" &&
            parseInt(altura) > 0 && parseInt(largura) > 0 &&
            parseInt(altura) != parseInt(largura) && redondo) {
            alert('O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.');

            if (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked)
                FindControl("Redondo_chkSelecao", "input").checked = false;

            return false;
        }

        var numBenef = "";

        if (FindControl("Redondo_chkSelecao", "input") != null) {
            numBenef = FindControl("Redondo_chkSelecao", "input").id;
            numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
            numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
            numBenef = eval(numBenef).NumeroBeneficiamentos();
        }
        else
            numBenef = eval(nomeControleBenef).NumeroBeneficiamentos();

        if (altura == "" || largura == "" || qtde == "" || altura == "0" || (tipoCalc != 2 && tipoCalc != 10)) {
            if (altura > 0 && largura > 0 && qtde > 0 && isVidro) {
                var totM2 = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, "false").value;
                var totM2Calc = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
                var totM2CalcSemChapa = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;

                FindControl("lblTotM2", "span").innerHTML = totM2;
                FindControl("lblTotM2Calc", "span").innerHTML = " (" + totM2Calc + ")"; // Este campo sempre deve ser preenchido, pois ele é associado ao controle de beneficiamento
                FindControl("hdfTotM2", "input").value = totM2Calc;
                FindControl("hdfTotM2SemChapa", "input").value = totM2CalcSemChapa;
            }

            if (qtde != "" && qtde != "0")
                calcTotal();

            return false;
        }

        var totM2 = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, "false").value;
        var totM2Calc = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
        var totM2CalcSemChapa = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;

        FindControl("lblTotM2", "span").innerHTML = totM2;
        FindControl("lblTotM2Calc", "span").innerHTML = " (" + totM2Calc + ")"; // Este campo sempre deve ser preenchido, pois ele é associado ao controle de beneficiamento
        FindControl("hdfTotM2", "input").value = totM2Calc;
        FindControl("hdfTotM2SemChapa", "input").value = totM2CalcSemChapa;

        calcTotal();
    }
    catch (err) {

    }
}

function GetAdicionalAlturaChapa() {

  var idProd = FindControl("hdfIdProd", "input").value;
  var altura = FindControl("txtAltura", "input").value;
  var idCliente = FindControl("hdfIdCliente", "input").value;
  var idOrca = FindControl("hdfIdOrca", "input").value;
  var tipoEntrega = FindControl("drpTipoEntrega", "select").value;
  var revenda = FindControl("chkRevenda", "input").checked;
  var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
  controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
  var percDescontoQtde = controleDescQtde.PercDesconto();

  FindControl("txtValor", "input").value = MetodosAjax.GetValorTabelaProduto(idProd, tipoEntrega, idCliente, revenda, false, percDescontoQtde, "", "", idOrca, altura).value.replace(".", ",");
}

// Calcula o total mostrando mensagem de erro se houver
function calcTotalMsg() {
    atualizaValMin();

    // Verifica se campos obrigatórios estão preenchidos
    var codProd = FindControl("txtCodProd", "input").value;
    var valor = FindControl("txtValor", "input").value;
    var qtde = FindControl("txtQtde", "input").value;
    var valMin = FindControl("hdfValMin", "input").value;
    var esp = FindControl("txtEspessura", "input") != null ? FindControl("txtEspessura", "input").value : 0;
    var altura = FindControl("txtAltura", "input").value;
    var largura = FindControl("txtLargura", "input").value;

    valMin = new Number(valMin.replace(',', '.'));
    if (codProd == "") {
        alert("Informe o código do produto.");
        return false;
    }
    else if (!FindControl("txtValor", "input").disabled && new Number(valor.replace(',', '.')) < valMin) {
        alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
        return false;
    }

    if (valor == "" || valor == 0) {
        alert("Informe o valor do produto.");
        return false;
    }

    if (qtde == "0" || qtde == "") {
        alert("Informe a quantidade.");
        return false;
    }

    if (FindControl("hdfIsVidro", "input").value == "true") {
        // Verifica se a altura foi especificada
        if ((altura == "" || altura == 0) && !FindControl("txtAltura", "input").disabled) {
            alert("Informe a Altura.");
            return false;
        }

        // Verifica se a largura foi especificada
        if ((largura == "" || largura == 0) && !FindControl("txtLargura", "input").disabled) {
            alert("Informe a Largura.");
            return false;
        }

        // Verifica se a espessura foi informada
        if (esp == "0" || esp == "") {
            alert("Informe a espessura.");
            return false;
        }

        // Verifica se a espessura é válida, se for vidro
//        if (esp == 11 || esp == 13 || esp == 14 || (esp > 15 && esp < 19) || esp > 19) {
//            alert("Espessura inválida.");
//            return false;
//        }
    }

    calcTotal();
}

// Calcula em tempo real o valor total do produto
function calcTotal() {
    try {
        FindControl("hdfAlturaCalc", "input").value = FindControl("txtAltura", "input").value;
        calculaTodos(nomeControleBenef + "_", '', 'callbackSetTotal');
    }
    catch (err) {
        alert(err)
        return false;
    }

    return true;
}

// Inclui item na tabela de produtos dinamicamente
function incluirItem(percComissao) {
    // Calcula o valor total do item
    if (calcTotalMsg() == false)
        return false;

    // Busca valores que serão utilizados na tabela de produtos
    var codProd = FindControl("txtCodProd", "input").value;
    var totM2 = FindControl("lblTotM2", "span").innerHTML.replace(',', '.');
    var descricao = FindControl("lblDescrProd", "span").innerHTML;
    var altura = FindControl("txtAltura", "input").value;
    var alturaCalc = FindControl("hdfAlturaCalc", "input").value;
    var largura = FindControl("txtLargura", "input").value;
    var espessura = FindControl("txtEspessura", "input") != null ? FindControl("txtEspessura", "input").value : 0;
    var qtde = FindControl("txtQtde", "input").value;
    var total = FindControl("lblTotal", "span").innerHTML.replace("R$", "").replace(" ", "");
    var valorPrimario = FindControl("txtValor", "input").value.replace(',', '.');
    var percComissao = FindControl("hdfPercComissao", "input").value.replace(',', '.');
    percComissao = isNaN(parseFloat(percComissao)) ? 0 : percComissao;
    valorPrimario = (parseFloat(valorPrimario) / ((100 - parseFloat(percComissao)) / 100)).toFixed(2);
    var servicos = "";
    var servicosInfo = "";
    var custo = FindControl("hdfCustoProd", "input").value;
    var valorTabela = FindControl("hdfValMin", "input").value;
    var percDescontoQtde = FindControl("txtPercDescQtde", "input").value;
    var idProcesso = FindControl("hdfIdProcesso", "input").value;
    var codProcesso = FindControl("txtProcIns", "input").value;
    var idAplicacao = FindControl("hdfIdAplicacao", "input").value;
    var codAplicacao = FindControl("txtAplIns", "input").value;

    if (valorPrimario == "" || parseFloat(valorPrimario.replace(",", ".")) == 0) {
        alert("Informe o valor unitário do produto.");
        return false;
    }

    if (FindControl("txtAltura", "input").disabled == false &&
        (altura == "" || parseFloat(altura.replace(",", ".")) == 0)) {
        alert("Informe a altura.");
        return false;
    }

    if (!obrigarProcApl())
        return false;

    if (!validaProc(idProcesso))
        return false;

    // Se o item for vidro, verifica quais serviços serão executados no mesmo
    if (exibirControleBenef(nomeControleBenef))
    {
        var temp = getServicos(nomeControleBenef);
        servicos = temp.Descricao;
        servicosInfo = temp.Info;
    }

    // Volta metragem quadrada para o original
    totM2 = FindControl("lblTotM2", "span").innerHTML + FindControl("lblTotM2Calc", "span").innerHTML;

    var retornoValidacao = LstOrcamentoRapido.ValidarTamanhoDosProdutos(FindControl("hdfIdProd", "input").value, altura, largura, servicosInfo)

    if (retornoValidacao != null && retornoValidacao.value != "" && retornoValidacao.value != null) {
        alert(retornoValidacao.value);
        return false;
    }

    // Adiciona a linha à tabela
    var row = criarLinha(FindControl("hdfIdProd", "input").value, codProd, valorPrimario, total, altura, alturaCalc, largura, qtde,
        FindControl("Redondo_chkSelecao", "input") != null ? FindControl("Redondo_chkSelecao", "input").checked : false, totM2, descricao,
        servicos, custo, valorTabela, espessura, servicosInfo, percDescontoQtde, idProcesso, codProcesso, idAplicacao, codAplicacao);

    // Limpa o controle de beneficiamentos
    eval(nomeControleBenef).Limpar();

    if (callbackIncluir != "")
    {
        var dadosProduto = getProdutoRow(row, "~", "");
        if (window.opener != null)
            eval("window.opener." + callbackIncluir + "('" + row.id + "', '" + dadosProduto + "')");
        else
            eval("window.top." + callbackIncluir + "('" + row.id + "', '" + dadosProduto + "')");
    }

    var txtCodProd = FindControl("txtCodProd", "input");
    if (txtCodProd != null)
        txtCodProd.select();

    return false;
}

function criarLinha(idProd, codigoProduto, valorUnit, total, altura, alturaCalc, largura, qtde, redondo, totM2,
    descricao, servicos, custo, valorTabela, espessura, servicosInfo, percDescontoQtde, idProcesso, codProcesso, idAplicacao, codAplicacao)
{
    // Monta tabela dinamicamente
    tabela = document.getElementById('lstProd');
    row = tabela.insertRow(countProd);
    row.id = "row" + row.rowIndex;
    row.setAttribute('idProd', idProd);
    row.setAttribute('valor', valorUnit);
    row.setAttribute('valorTotal', total);
    row.setAttribute('altura', altura);
    row.setAttribute('alturaCalc', alturaCalc);
    row.setAttribute('largura', largura);
    row.setAttribute('qtde', qtde);
    row.setAttribute('redondo', redondo);
    row.setAttribute('totM2', totM2.replace(',', '.'));
    row.setAttribute('descricao', descricao + (servicos.length > 0 ? " (" + servicos + ")" : ""));
    row.setAttribute('servicoInfo', servicosInfo); // [0]Id do Beneficiamento;[1]Qtd;[2]Valor;[3]Total;[4]Altura;[5]Largura;[6]Esp.Bisote|
    row.setAttribute('custoProd', custo);
    row.setAttribute('espessura', espessura);
    row.setAttribute('valorTabela', valorTabela);
    row.setAttribute('percDescontoQtde', percDescontoQtde);
    row.setAttribute('idProcesso', idProcesso);
    row.setAttribute('idAplicacao', idAplicacao);

    var totalIcms = document.getElementById("dadosIcms").style.display != "none" ? parseFloat(FindControl("lblValorIcms", "span").innerHTML.replace("R$", "").replace(" ", "").replace(",", ".")) : 0;
    row.setAttribute('totalIcms', totalIcms);

    // Se o vidro for redondo, zera a largura
    if (redondo)
        largura = 0;

    try {
        row.innerHTML = "<td style='padding-right: 4px'><a href=\"#\" onclick=\"return excluirItem(" + row.rowIndex + ");\">" +
            "<img src=\"../Images/ExcluirGrid.gif\" border=\"0\" title=\"Excluir\"/></a></td>" +
            "<td style='padding-right: 4px'>" + codigoProduto + " - " + descricao + "</td><td style='padding-right: 4px'>" + qtde + "</td><td style='padding-right: 4px'>" + (usarAltLarg ? altura + (altura != alturaCalc ? " (" + alturaCalc + ")" :
            "") : largura) + "</td><td style='padding-right: 4px'>" + (usarAltLarg ? largura : altura + (altura != alturaCalc ? " (" + alturaCalc + ")" : "")) +
            "</td><td style='padding-right: 4px'>" + totM2 + "</td><td style='padding-right: 4px'>" + servicos + "</td><td style='padding-right: 4px'>" + parseFloat(valorUnit.toString().replace(',', '.')).toFixed(2).replace('.', ',') + "</td>" +
            "<td id=\"total" + row.rowIndex + "\" style='padding-right: 4px'>" + parseFloat(total.toString().replace(',', '.')).toFixed(2).replace('.', ',') + "</td>" +
            "<td>" + codProcesso + (codProcesso && codAplicacao ? " / " : "") + codAplicacao + "</td>";
    }
    catch (err) {
        var tdExcluir = row.insertCell(0);
        var tdDescr = row.insertCell(1);
        var tdQtd = row.insertCell(2);
        var tdAlt = row.insertCell(3);
        var tdLarg = row.insertCell(4);
        var tdTotM = row.insertCell(5);
        var tdServ = row.insertCell(6);
        var tdTotal = row.insertCell(7);
        var tdProcApl = row.insertCell(8);

        tdExcluir.innerHTML = "<a href=\"#\" onclick=\"return excluirItem(" + row.rowIndex + ");\"><img src=\"../Images/ExcluirGrid.gif\" border=\"0\" title=\"Excluir\"/></a>";
        tdDescr.innerHTML = descricao;
        tdQtd.innerHTML = qtde;
        tdAlt.innerHTML = altura + (altura != alturaCalc ? " (" + alturaCalc + ")" : "");
        tdLarg.innerHTML = largura;
        tdTotM.innerHTML = totM2;
        tdServ.innerHTML = servicos;
        tdTotal.innerHTML = total;
        tdTotal.setAttribute("id", "total" + row.rowIndex);
        tdProcApl.innerHTML = codProcesso + (codProcesso && codAplicacao ? " / " : "") + codAplicacao;
    }

    countProd++;

    // Incrementa o valor total do orçamento
    totalOrca = parseFloat(totalOrca) + parseFloat(total.replace(".", "").replace("R$", "").replace(" ", "").replace(",", "."));

    atualizaTotalOrca(totalOrca);

    drawAlternateLines();

    desabilitaTipoEntregaRevenda();

    calculaTotais();

    return row;
}

function replace_html(el, html) {
    if (el) {
        var oldEl = (typeof el === "string" ? document.getElementById(el) : el);
        var newEl = document.createElement(oldEl.nodeName);

        // Preserve any properties we care about (id and class in this example)
        newEl.id = oldEl.id;
        newEl.className = oldEl.className;

        //set the new HTML and insert back into the DOM
        newEl.innerHTML = html;
        if (oldEl.parentNode)
            oldEl.parentNode.replaceChild(newEl, oldEl);
        else
            oldEl.innerHTML = html;

        //return a reference to the new element in case we need it
        return newEl;
    }
};

// Desabilita a opção tipo de entrega e revenda se já houver produtos inseridos neste orçamento rápido
function desabilitaTipoEntregaRevenda() {
    var disabled = false;

    FindControl("drpTipoEntrega", "select").disabled = disabled;
    FindControl("chkRevenda", "input").disabled = disabled;
}

function excluirItem(linha) {
    // Recupera o total da linha antes de ser excluída
    var totalLinha = new Number(document.getElementById('total' + linha).innerHTML.replace("R$", "").replace(" ", "").replace(",", ".")).toFixed(2);

    // Recalcula o valor do Orçamento
    totalOrca -= totalLinha;
    atualizaTotalOrca(totalOrca);

    // Exclui o produto da tabela
    var row = document.getElementById("row" + linha);
    row.style.display = "none";

    drawAlternateLines();

    if (callbackExcluir != "")
    {
        var dadosProduto = getProdutoRow(row, "~", "");
        if (window.opener != null)
            eval("window.opener." + callbackExcluir + "('" + row.id + "', '" + dadosProduto + "')");
        else
            eval("window.top." + callbackExcluir + "('" + row.id + "', '" + dadosProduto + "')");
    }

    desabilitaTipoEntregaRevenda();

    calculaTotais();

    return false;
}

function getTotalIcms()
{
    var retorno = 0;

    var tabela = document.getElementById("lstProd");
    for (i = 1; i < tabela.rows.length - 1; i++)
    {
        if (tabela.rows[i].style.display == "none")
            continue;

        var valorIcms = parseFloat(tabela.rows[i].getAttribute("totalIcms"));
        retorno += !isNaN(valorIcms) ? valorIcms : 0;
    }

    return retorno;
}

function atualizaTotalOrca(total)
{
    // Exibe o valor total do orçamento até então
    var totalIcms = getTotalIcms();

    FindControl("lblTotalOrca", "span").innerHTML = "R$ " + total.toFixed(2).toString().replace(".", ",");
    FindControl("lblSomaIcms", "span").innerHTML = "R$ " + totalIcms.toFixed(2).toString().replace(".", ",");
    FindControl("lblTotalIcms", "span").innerHTML = "R$ " + (total + totalIcms).toFixed(2).toString().replace(".", ",");
}

function revendaClick(chkRevenda) {
    var drpTipoEntrega = FindControl("drpTipoEntrega", "select");

    drpTipoEntrega.disabled = chkRevenda.checked;

    if (chkRevenda.checked)
        drpTipoEntrega.selectedIndex = 0;
}

// Colore o fundo de linhas alternadas da grid de produtos
function drawAlternateLines() {
    var tabela = document.getElementById('lstProd');
    var rows = tabela.getElementsByTagName('tr');
    var preenche = false;

    for (i = 1; i < rows.length; i++) {
        if (rows[i].style.display != "none") {
            if (preenche)
                rows[i].style.backgroundColor = "#E4EFF1";
            else
                rows[i].style.backgroundColor = "#FFFFFF";

            preenche = !preenche;
        }
    }
}

// Calcula os totais de quantidade e m²
function calculaTotais()
{
    tabela = document.getElementById('lstProd');
    var totalQtde = document.getElementById("totalQtde");
    var totalM2 = document.getElementById("totalM2");

    var qtde = 0, m2 = 0, m2Calc = 0;
    for (i = 1; i < tabela.rows.length - 1; i++)
        if (tabela.rows[i].style.display != "none")
        {
            qtde += !isNaN(parseFloat(tabela.rows[i].cells[2].innerHTML)) ? parseFloat(tabela.rows[i].cells[2].innerHTML) : 0;

            var m2Temp = tabela.rows[i].cells[5].innerHTML;
            var m2CalcTemp = m2Temp.indexOf(' (') > -1 ? m2Temp.substr(m2Temp.indexOf(' (') + 2) : "0";
            if (m2Temp.indexOf(' (') > -1)
            {
                m2Temp = m2Temp.substr(0, m2Temp.indexOf(' ('));
                m2CalcTemp = m2CalcTemp.substr(0, m2CalcTemp.length - 1);
            }

            var m2Temp = !isNaN(parseFloat(m2Temp.replace(',', '.'))) ? parseFloat(m2Temp.replace(',', '.')) : 0;
            var m2CalcTemp = !isNaN(parseFloat(m2CalcTemp.replace(',', '.'))) ? parseFloat(m2CalcTemp.replace(',', '.')) : 0;

            m2 += m2Temp;
            m2Calc += m2CalcTemp > 0 ? m2CalcTemp : m2Temp;
        }

    totalQtde.innerHTML = qtde.toString().replace('.', ',');
    totalM2.innerHTML = parseFloat(m2).toString().replace('.', ',') +
        (m2Calc > 0 ? " (" + parseFloat(m2Calc).toString().replace('.', ',') + ")" : "");
}

// Abre relatório para imprimir o orçamento rápido
function openRpt() {
    openWindow(600, 800, "../Relatorios/RelOrcamentoRapido.aspx?1=1");
    return false;
}

var isPedido = true;

function gerarPedido() {
    openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=pedido');
    isPedido = true;
    return false;
}

function gerarOrcamento() {
    var nomeCliente = prompt("Digite o nome do cliente", "");
    if (nomeCliente == null)
        return false;

    isPedido = false;
    setCliente(0, nomeCliente);
    return false;
}

function getProdutoRow(row, sepItem, sepLinha)
{
    return row.getAttribute("idProd") + sepItem + row.getAttribute("valor") + sepItem + row.getAttribute("valorTotal") + sepItem +
        row.getAttribute("qtde") + sepItem + row.getAttribute("altura") + sepItem + row.getAttribute("alturaCalc") + sepItem +
        row.getAttribute("largura") + sepItem + row.getAttribute("redondo") + sepItem + row.getAttribute("totM2") + sepItem +
        row.getAttribute("descricao") + sepItem + row.getAttribute("custoProd") + sepItem + row.getAttribute("valorTabela") + sepItem +
        row.getAttribute("espessura") + sepItem + row.getAttribute("percDescontoQtde") + sepItem + row.getAttribute("servicoInfo") + sepItem +
        FindControl("hdfPercComissao", "input").value + sepItem + row.getAttribute("idProcesso") + sepItem + row.getAttribute("idAplicacao") + sepLinha;
}

function setCliente(idCli, Nome, windowSel) {
    var produtos = "";

    try
    {
        var tbProd = document.getElementById("lstProd");
        var rows = tbProd.getElementsByTagName('tr');

        for (i = 1; i < rows.length - 1; i++) {
            if (rows[i].style.display == "none")
                continue;

            produtos += getProdutoRow(rows[i], "\t", "\n");
        }
    }
    catch (err) {
        alert(err);
    }

    /* Chamado 33666. */
    var windowSelExiste = false;

    if (windowSel == null || windowSel == undefined)
        windowSel = window;
    else
        windowSelExiste = true;

    if (produtos == "") {
        if (isPedido)
            windowSel.alert("Inclua pelo menos um produto na lista de produtos para gerar pedido.");
        else
            windowSel.alert("Inclua pelo menos um produto na lista de produtos para gerar orçamento.");
        return false;
    }

    var tipoEntrega = FindControl("drpTipoEntrega", "select").value;
    var tipoPedido = FindControl("drpTipoPedido", "select").value;
    var response;
    var espessura = FindControl("txtEspessura", "input") != null ? FindControl("txtEspessura", "input").value : 0;
    var dataEntrega = FindControl("ctrlDataEntrega_txtData", "input").value;

    if (isPedido)
        response = LstOrcamentoRapido.GerarPedido(idCli, tipoPedido, tipoEntrega, dataEntrega, produtos, espessura).value;
    else
        response = LstOrcamentoRapido.GerarOrcamento(Nome, tipoPedido, tipoEntrega, dataEntrega, produtos, espessura, FindControl("drpNumParcelas", "select").value).value;

    if (response == null) {
        if (isPedido)
            windowSel.alert("Falha ao gerar pedido. AJAX Error.");
        else
            windowSel.alert("Falha ao gerar orçamento. AJAX Error.");
        return false;
    }

    response = response.split('\t');

    // Mostra a mensagem retornada, independente se for de erro ou de operação executada corretamente
    windowSel.alert(response[1]);

    if (response[0] == "Erro")
        return false;
    else if (isPedido) {
        if (windowSelExiste)
            windowSel.close();

        window.location = "../Cadastros/CadPedido.aspx?idPedido=" + response[2] + "&ByVend=";
    }
    else {
        if (windowSelExiste)
            windowSel.close();

        window.location = "../Cadastros/CadOrcamento.aspx?idOrca=" + response[2] + "&ByVend=";
    }
}

function criarTabela(produtos, benefProdutos, nomeAtributoId, idProdutos)
{
    for (prod = 0; prod < produtos.length; prod++)
    {
        var p = produtos[prod];
        var row = criarLinha(p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7], p[8], p[9], p[10], p[11], p[12], p[13], p[14], benefProdutos[prod], 0, p[15], p[16], p[17], p[18]);
        row.setAttribute(nomeAtributoId, idProdutos[prod]);
    }
}
