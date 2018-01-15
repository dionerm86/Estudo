var countCNI = 1;
var totalCNI = 1;

//-------------------------------------------------------------------
//Limpa os dados da tabela
//-------------------------------------------------------------------
function limparTabelaCNI(tabela) {
    tabela.innerHTML = "";
    tabela.setAttribute("totalCNI", null);
    tabela.setAttribute("countCNI", null);
}

//-------------------------------------------------------------------
//obtem o total do cni para o campo
//-------------------------------------------------------------------
function getTotalCNICampo(tabela, nomeCampo) {
    var total = tabela.getAttribute("totalCNI");
    if (total === undefined || total == null)
        total = 0;

    total = parseFloat(total);
    if (isNaN(total))
        total = 0;

    return total;
}

// Função utilizada para retornar lista de ids dos CNI Cadastrados
function getCNI(nomeTabelaCNI) {
    var tb = document.getElementById(nomeTabelaCNI);

    var CNIs = "";

    // Para cada linha da tabela de CNI, menos o cabeçalho
    for (i = 1; i < tb.rows.length; i++) {
        // Se a linha não estiver "Excluída", adiciona CNI cadastrado à variável de cheques próprios
        if (!tb.rows[i].getAttribute("Excluida")) {           
            CNIs += tb.rows[i].getAttribute("idCartaoNaoIdentificado");
        }
        if (i < tb.rows.length)
            CNIs += ";";
    }
    CNIs.trim
    return CNIs;
}

// Função utilizada para cadastrar CNI utilizados no pagamento
function setCartaoNaoIdentificado(nomeTabelaCNI, selCNIWin, idCartaoNaoIdentificado, contaBancaria, valor,
    tipoCartao, observacao, nomeControleFormaPagto) {
    // Monta tabela dinamicamente
    var tabela = document.getElementById(nomeTabelaCNI);

    tabela.className = "gridStyle";
    tabela.cellPadding = "0";
    tabela.cellSpacing = "0";
    tabela.style.borderCollapse = "collapse";

    // Verifica se o CNI existe na tabela (apenas para CNI selecionados)
    if (idCartaoNaoIdentificado != "" && idCartaoNaoIdentificado != null && typeof idCartaoNaoIdentificado != "undefined") {
        for (i = 0; i < tabela.rows.length; i++) {
            if (tabela.rows[i].style.display == "none")
                continue;

            if (tabela.rows[i].getAttribute("idCartaoNaoIdentificado") == idCartaoNaoIdentificado) {
                if (typeof selCNIWin == "object")
                    selCNIWin.alert("CNI já incluído no pagamento.")

                return false;
            }
        }
    }

    countCNI = tabela.getAttribute("countCNI");
    if (countCNI === undefined || countCNI == null || countCNI == "" || isNaN(countCNI))
        countCNI = 1;

    totalCNI = tabela.getAttribute("totalCNI");
    if (totalCNI === undefined || totalCNI == null || totalCNI == "" || isNaN(totalCNI))
        totalCNI = 0;

    var estiloComum = "padding-left: 3px; padding-right: 3px";

    // Cria títulos para a tabela
    if (countCNI == 1) {
        tabela.innerHTML = "<tr align=\"left\">" +
            "<th style=\"font-weight: bold; " + estiloComum + "\"></th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Cod. </th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Banco/Agência/Conta</th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Valor</th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Tipo Cartão</th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Obs.</th></tr>";
    }

    // Cria uma nova linha com os dados passados
    row = tabela.insertRow(countCNI);
    row.id = nomeTabelaCNI + "_rowCNI" + row.rowIndex;

    // Adapta o campo valor
    valor = valor.toString().replace("R$", "").replace(" ", "").replace(".", ",");

    // Se for pagto. com cheque proprio, guarda a conta banco, mas se for pagto. com cheques de terceiros, guarda o idCheque
    row.setAttribute("idCartaoNaoIdentificado", idCartaoNaoIdentificado);
    row.setAttribute("contaBancaria", contaBancaria);
    row.setAttribute("valor", valor);
    row.setAttribute("tipoCartao", tipoCartao);
    row.setAttribute("observacao", observacao);

    row.innerHTML =
       "<td style=\"white-space: nowrap;" + estiloComum + "\">" +
       "<a href=\"#\" onclick=\"excluirItemCNI('" + nomeTabelaCNI + "', " + row.rowIndex + ", '" + nomeControleFormaPagto + "'); return false\">" +
            "<img src=\"../Images/ExcluirGrid.gif\" border=\"0\" title=\"Excluir\" /></a></td>" +
       "<td style=\"" + estiloComum + "\">" + idCartaoNaoIdentificado + "</td>" +
       "<td style=\"" + estiloComum + "\">" + contaBancaria + "</td>" +
       "<td style=\"" + estiloComum + "\" id=\"" + nomeTabelaCNI + "_totalCNI" + row.rowIndex + "\">" + parseFloat(valor.replace(',', '.')).toFixed(2).replace('.', ',') + "</td>" +
       "<td style=\"" + estiloComum + "\">" + tipoCartao + "</td>" +
       "<td style=\"" + estiloComum + "\">" + observacao + "</td>";

    countCNI++;

    // Incrementa o valor total dos CNI
    totalCNI = parseFloat(totalCNI) + parseFloat(valor.replace(".", "").replace("R$", "").replace(" ", "").replace(",", "."));

    tabela.setAttribute("countCNI", countCNI);
    tabela.setAttribute("totalCNI", totalCNI);

    try {
        getPagto(nomeControleFormaPagto).value = getTotalCNICampo(tabela, nomeControleFormaPagto).toFixed(2).replace(".", ",");
        var funcao = getPagto(nomeControleFormaPagto).getAttribute("onchange");
        if (typeof funcao == "string")
            eval(funcao);
    }
    catch (err) { }   

    if (selCNIWin != null) {
        if (FindControl("drpFormaPagto", "select").value == 2 && linha == "")
            selCNIWin.alert("CNI incluído ao pagamento.");

        if (typeof selCNIWin.limpar == "function")
            selCNIWin.limpar();
    }

    return false;
}

function excluirItemCNI(nomeTabelaCNI, linha, nomeCampo) {

    var tabela = document.getElementById(nomeTabelaCNI);

    // Exclui o produto da tabela
    document.getElementById(nomeTabelaCNI + "_rowCNI" + linha).style.display = "none";
    document.getElementById(nomeTabelaCNI + "_rowCNI" + linha).setAttribute("excluida", 1);

    // Recupera o total da linha antes de ser excluída (escondida)
    var totalLinha = new Number(document.getElementById(nomeTabelaCNI + "_totalCNI" + linha).innerHTML.replace("R$", "").replace(" ", "").replace(".", "").replace(",", ".")).toFixed(2);

    // Recalcula o valor total
    totalCNI -= totalLinha;
    tabela.setAttribute("totalCNI", totalCNI);

    try {
        getPagto(nomeCampo).value = getTotalCNICampo(tabela, nomeCampo).toFixed(2).replace(".", ",");
        var funcao = getPagto(nomeCampo).getAttribute("onchange");
        if (typeof funcao == "string")
            eval(funcao);
    }
    catch (err) { }
    
    return false;
}