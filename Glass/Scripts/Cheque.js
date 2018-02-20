var countCheques = 1; // Conta a quantidade de cheques adicionados ao form
var totalCheques = 0; // Calcula o total de todos os cheques

function getMaiorPrazoCheques(nomeTabelaCheques)
{
    // Coloca um nome padrão na tabela
    if (nomeTabelaCheques === undefined || nomeTabelaCheques == null || nomeTabelaCheques == "")
        nomeTabelaCheques = "tbChequePagto";
        
    var maiorData = null;
    var cheques = getChequesString(nomeTabelaCheques, false).split("|");
    for (i = 0; i < cheques.length; i++)
    {
        if (cheques[i] == "")
            continue;
        
        var dadosCheque = cheques[i].split("\t");
        if (dadosCheque.length < 7 || dadosCheque[6] == "")
            continue;
        
        var dataCheque = toDate(dadosCheque[6]);
        if (maiorData == null || dataCheque.getTime() > maiorData.getTime())
            maiorData = dataCheque;
    }
    
    return diferencaDatas(maiorData, new Date());
}

// Recupera o total de cheques
function getTotalCheques(nomeTabelaCheques)
{
    // Coloca um nome padrão na tabela
    if (nomeTabelaCheques === undefined || nomeTabelaCheques == null || nomeTabelaCheques == "")
        nomeTabelaCheques = "tbChequePagto";
    
    // Recupera tabela
    var tabela = document.getElementById(nomeTabelaCheques);
    
    countCheques = tabela.getAttribute("countCheques");
    if (countCheques === undefined || countCheques == null || countCheques == "" || isNaN(countCheques))
        countCheques = 1;
        
    totalCheques = tabela.getAttribute("totalCheques");
    if (totalCheques === undefined || totalCheques == null || totalCheques == "" || isNaN(totalCheques))
        totalCheques = 0;
    
    return totalCheques;
}

// Função que invoca o método de atualização dos dados do controle de forma de pagamento
function callbackControleFormaPagto(nomeControleFormaPagto)
{
    /*
    if (typeof eval(nomeControleFormaPagto) == "object" && typeof eval(nomeControleFormaPagto).Calcular == "function")
        eval(nomeControleFormaPagto).Calcular();
    */
}

// Função utilizada para alterar o valor de um atributo em toda a tabela
function alteraAtributos(nomeTabelaCheques, nomeAtributo, valor)
{
    // Coloca um nome padrão na tabela
    if (nomeTabelaCheques === undefined || nomeTabelaCheques == null || nomeTabelaCheques == "")
        nomeTabelaCheques = "tbChequePagto";
    
    // Altera o atributo de todas as linhas da tabela
    var tb = document.getElementById(nomeTabelaCheques);
    for (i = 1; i < tb.rows.length; i++)
        tb.rows[i].setAttribute(nomeAtributo, valor);
}

// Função utilizada para retornar uma string com os dados dos cheques
function getChequesString(nomeTabelaCheques, selecionarCheque)
{
    // Coloca um nome padrão na tabela
    if (nomeTabelaCheques === undefined || nomeTabelaCheques == null || nomeTabelaCheques == "")
        nomeTabelaCheques = "tbChequePagto";
    
    // Guarda os cheques proprios ou de terceiros, de acordo com a forma de pagamento, cadastrados/selecionados, separados por |
    var chequesPagto = "";
    var tb = document.getElementById(nomeTabelaCheques);

    // Para cada linha da tabela de cheques, menos o cabeçalho
    for (i = 1; i < tb.rows.length; i++) {
        // Se a linha não estiver "Excluída", adiciona cheque cadastrado à variável de cheques próprios
        if (!tb.rows[i].getAttribute("Excluida"))
        {
            if (!selecionarCheque)
                chequesPagto += tb.rows[i].getAttribute("tipo") + "\t" + tb.rows[i].getAttribute("contaBancoIdCheque") + "\t" + 
                    tb.rows[i].getAttribute("numCheque") + "\t" + tb.rows[i].getAttribute("digitoNum") + "\t" +
                    tb.rows[i].getAttribute("titular") + "\t" + tb.rows[i].getAttribute("valor") + "\t" + tb.rows[i].getAttribute("dataVenc") + "\t" +
                    tb.rows[i].getAttribute("situacao") + "\t" + tb.rows[i].getAttribute("origem") + "\t" + tb.rows[i].getAttribute("idAcertoCheque") + "\t" +
                    tb.rows[i].getAttribute("idContaR") + "\t" + tb.rows[i].getAttribute("idPedido") + "\t" + tb.rows[i].getAttribute("idAcerto") + "\t" +
                    tb.rows[i].getAttribute("idLiberarPedido") + "\t" + tb.rows[i].getAttribute("idTrocaDevolucao") + "\t" + tb.rows[i].getAttribute("banco") + "\t" +
                    tb.rows[i].getAttribute("agencia") + "\t" + tb.rows[i].getAttribute("conta") + "\t" + tb.rows[i].getAttribute("idCheque") + "\t" +
                    tb.rows[i].getAttribute("obs") + "\t" + tb.rows[i].getAttribute("idSinal") + "\t" + tb.rows[i].getAttribute("cpfCnpj") + "\t" +
                    tb.rows[i].getAttribute("idLoja") + "|";
            else
                chequesPagto += tb.rows[i].getAttribute("idCheque") + ",";
        }
    }
    
    return chequesPagto;
}

// Função utilizada para cadastrar cheques utilizados no pagamento
function setCheque(nomeTabelaCheques, idCheque, contaBancoIdCheque, numCheque, digitoNum, titular, valor, dataVenc, banco, agencia, conta,
    situacao, obs, selChequesWin, tipo, origem, idAcertoCheque, idContaR, idPedido, idSinal, idAcerto, idLiberarPedido, idTrocaDevolucao,
    cpfCnpj, idLoja, nomeLoja, nomeCampo, linha, callbackIncluir, callbackExcluir, nomeControleFormaPagto, exibirCpfCnpj)
{
    // Monta tabela dinamicamente
    var tabela = document.getElementById(nomeTabelaCheques);
    
    tabela.className = "gridStyle";
    tabela.cellPadding = "0";
    tabela.cellSpacing = "0";
    tabela.style.borderCollapse = "collapse";

    // Verifica se o cheque existe na tabela (apenas para cheques selecionados)
    if (idCheque != "" && idCheque != null && typeof idCheque != "undefined")
    {
        for (i = 0; i < tabela.rows.length; i++)
        {
            if (tabela.rows[i].style.display == "none")
                continue;

            if (tabela.rows[i].getAttribute("idCheque") == idCheque)
            {
                if (typeof selChequesWin == "object")
                    selChequesWin.alert("Cheque já incluído no pagamento.")

                return false;
            }
        }
    }

    callbackIncluir = typeof callbackIncluir == "string" ? callbackIncluir : "";
    callbackExcluir = typeof callbackExcluir == "string" ? callbackExcluir : "";
    nomeCampo = nomeCampo.replace(/\'/g, "");
    
    // Remove uma linha, se necessário
    if (linha != "")
        excluirItemCheque(nomeTabelaCheques, linha, nomeCampo, callbackExcluir, nomeControleFormaPagto);
    
    countCheques = tabela.getAttribute("countCheques");
    if (countCheques === undefined || countCheques == null || countCheques == "" || isNaN(countCheques))
        countCheques = 1;
        
    totalCheques = tabela.getAttribute("totalCheques");
    if (totalCheques === undefined || totalCheques == null || totalCheques == "" || isNaN(totalCheques))
        totalCheques = 0;

    var estiloComum = "padding-left: 3px; padding-right: 3px";
    
    // Cria títulos para a tabela
    if (countCheques == 1) {
        tabela.innerHTML = "<tr align=\"left\">" +
            "<th style=\"font-weight: bold; " + estiloComum + "\"></th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Loja</th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Num. Cheque</th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Titular</th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Valor</th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Data Venc.</th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Banco/Agência/Conta</th>" +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Situação</th>" +
            (exibirCpfCnpj ? "<th style=\"font-weight: bold; " + estiloComum + "\">CPF/CNPJ</th>" : "") +
            "<th style=\"font-weight: bold; " + estiloComum + "\">Obs.</th></tr>";
    }

    // Cria uma nova linha com os dados passados
    row = tabela.insertRow(countCheques);
    row.id = nomeTabelaCheques + "_rowCheque" + row.rowIndex;

    // Adapta o campo valor
    valor = valor.toString().replace("R$", "").replace(" ", "").replace(".", "");

    // Adapta o dígito do cheque
    digitoNum = digitoNum != null && digitoNum != undefined ? digitoNum : "";
    
    // Adapta o campo do cpf/cnpj
    cpfCnpj = cpfCnpj != null && cpfCnpj != undefined ? cpfCnpj : "";

    // Se for pagto. com cheque proprio, guarda a conta banco, mas se for pagto. com cheques de terceiros, guarda o idCheque
    row.setAttribute("idCheque", idCheque);
    row.setAttribute("contaBancoIdCheque", contaBancoIdCheque);
    row.setAttribute("numCheque", numCheque);
    row.setAttribute("digitoNum", digitoNum);
    row.setAttribute("titular", titular != null ? titular.replace("\t", "") : titular);
    row.setAttribute("valor", valor);
    row.setAttribute("dataVenc", dataVenc);
    row.setAttribute("situacao", situacao);
    row.setAttribute("tipo", tipo);
    row.setAttribute("origem", origem);
    row.setAttribute("idAcertoCheque", idAcertoCheque);
    row.setAttribute("idContaR", idContaR);
    row.setAttribute("idPedido", idPedido);
    row.setAttribute("idAcerto", idAcerto);
    row.setAttribute("idLiberarPedido", idLiberarPedido);
    row.setAttribute("idTrocaDevolucao", idTrocaDevolucao);
    row.setAttribute("banco", banco);
    row.setAttribute("agencia", agencia);
    row.setAttribute("conta", conta);
    row.setAttribute("obs", obs);
    row.setAttribute("idSinal", idSinal);
    row.setAttribute("cpfCnpj", cpfCnpj);
    row.setAttribute("idLoja", idLoja);

    var descrSituacao = situacao == 1 ? "Aberto" : "Compensado";
    var queryString = "cadastrar=false&controlPagto='" + nomeCampo + "'&tabelaCheque='" + nomeTabelaCheques + "'&origem=" + origem +
        "&idAcertoCheque=" + idAcertoCheque + "&idContaR=" + idContaR + "&idPedido=" + idPedido + "&idAcerto=" + idAcerto + 
        "&idLiberarPedido=" + idLiberarPedido + "&callbackIncluir='" + callbackIncluir + "'&callbackExcluir='" + callbackExcluir + "'";
    queryString = queryString.replace(/\'/g, "\\'");

    var urlEditar = selChequesWin != null ? selChequesWin.location.pathname : "../Cadastros/CadCheque.aspx";
    
    row.innerHTML =
        "<td style=\"white-space: nowrap;" + estiloComum + "\">" + (idCheque == null ? "<a href=\"#\" onclick=\"editarItemCheque('" + nomeTabelaCheques + "', " + row.rowIndex + ", '" + nomeCampo + "', '" + urlEditar + "', '" + queryString + "'); return false\">" +
        "<img src=\"../Images/EditarGrid.gif\" border=\"0\" title=\"Editar\" /></a>&nbsp;" : "") +
        "<a href=\"#\" onclick=\"excluirItemCheque('" + nomeTabelaCheques + "', " + row.rowIndex + ", '" + nomeCampo + "', '" + callbackExcluir + "', '" + nomeControleFormaPagto + "'); return false\">" +
        "<img src=\"../Images/ExcluirGrid.gif\" border=\"0\" title=\"Excluir\" /></a></td>" +
        "<td style=\"" + estiloComum + "\">" + nomeLoja + "</td><td style=\"" + estiloComum + "\">" + numCheque + (digitoNum != "" ? "-" + digitoNum : "") + "</td><td style=\"" + estiloComum + "\">" + titular + "</td>" +
        "<td style=\"" + estiloComum + "\" id=\"" + nomeTabelaCheques + "_totalCheque" + row.rowIndex + "\">" + parseFloat(valor.replace(',', '.')).toFixed(2).replace('.', ',') + "</td>" +
        "<td style=\"" + estiloComum + "\">" + dataVenc + "</td><td style=\"" + estiloComum + "\">" + banco + "/" + agencia + "/" + conta +
        "</td><td style=\"" + estiloComum + "\">" + descrSituacao + "</td>" + (exibirCpfCnpj ? "<td style=\"" + estiloComum + "\">" + cpfCnpj + "</td>" : "") +
        "<td style=\"" + estiloComum + "\">" + obs + "</td>";

    countCheques++;

    // Incrementa o valor total dos cheques
    totalCheques = parseFloat(totalCheques) + parseFloat(valor.replace(".", "").replace("R$", "").replace(" ", "").replace(",", "."));
    
    tabela.setAttribute("countCheques", countCheques);
    tabela.setAttribute("totalCheques", totalCheques);

    // Exibe o valor total pago até então
    try
    {
        getPagto(nomeCampo).value = getTotalChequesCampo(tabela, nomeCampo).toFixed(2).replace(".", ",");
        var funcao = getPagto(nomeCampo).getAttribute("onchange");
        if (typeof funcao == "string")
            eval(funcao);
    }
    catch (err) { }

    if (selChequesWin != null)
    {
        if (FindControl("drpFormaPagto", "select").value == 2 && linha == "")
            selChequesWin.alert("Cheque incluído ao pagamento.");

        if (typeof selChequesWin.limpar == "function")
            selChequesWin.limpar();
    }

    if (callbackIncluir != "")
        eval(callbackIncluir + "()");

    callbackControleFormaPagto(nomeControleFormaPagto);
    return false;
}

function editarItemCheque(nomeTabelaCheques, linha, nomeCampo, url, queryString)
{
    // Verifica se a função foi chamada a partir de uma página que não é popup
    if (FindControl("hdfPopupCheque", "input") == null)
    {
        openWindow(500, 650, url + "?editar=" + linha + "&" + queryString);
        return;
    }
    else
        nomeTabelaCheques = "tbChequePagto";
    
    var row = document.getElementById(nomeTabelaCheques).rows[linha];
    
    // Indica que é uma edição
    FindControl("hdfLinha", "input").value = linha;
    FindControl("btnInserir", "input").value = "Atualizar";
    row.style.backgroundColor = "#EEEEEE";
    var botoes = row.cells[0].getElementsByTagName("a");
    for (var b = 0; b < botoes.length; b++)
        botoes[b].style.visibility = "hidden";

    // Recupera para os controles os dados do cheque
    FindControl("txtNumero", "input").value = row.getAttribute("numCheque");
    FindControl("txtDigitoNum", "input").value = row.getAttribute("digitoNum");
    FindControl("txtTitular", "input").value = row.getAttribute("titular");
    var txtValor = FindControl("ctrValor_txtNumber", "input");
    txtValor = txtValor != null ? txtValor : FindControl("txtValor", "input");
    txtValor.value = row.getAttribute("valor");
    FindControl("ctrlData_txtData", "input").value = row.getAttribute("dataVenc");
    FindControl("txtBanco", "input").value = row.getAttribute("banco");
    FindControl("txtAgencia", "input").value = row.getAttribute("agencia");
    FindControl("txtConta", "input").value = row.getAttribute("conta");
    FindControl("txtObs", "textarea").value = row.getAttribute("obs");

    if (FindControl("drpTipoPessoa", "select"))
        FindControl("drpTipoPessoa", "select").value = row.getAttribute("cpfCnpj").length == 14 ? "F" : "J";

    if (window["alteraTipoPessoa"])
        alteraTipoPessoa();

    if (FindControl("txtCpfCnpj", "input"))
        FindControl("txtCpfCnpj", "input").value = row.getAttribute("cpfCnpj");

    try
    {
        FindControl("drpLoja", "select").value = row.getAttribute("idLoja");
        FindControl("drpContaBanco", "select").value = row.getAttribute("contaBancoIdCheque");
        FindControl("drpSituacao", "select").value = row.getAttribute("situacao");
    }
    catch (err) { }
}

function getTotalChequesCampo(tabela, nomeCampo)
{
    var total = tabela.getAttribute("totalCheques");
    if (total === undefined || total == null)
        total = 0;

    total = parseFloat(total);
    if (isNaN(total))
        total = 0;
    
    if (nomeCampo == null || nomeCampo === undefined)
        return total;
        
    for (iTotal = 1; iTotal < tabela.rows.length; iTotal++)
    {
        if (tabela.rows[iTotal].style.display == "none")
            continue;

        var botaoEditar = tabela.rows[iTotal].cells[0].getElementsByTagName("a")[0];
        if (botaoEditar.onclick.toString().indexOf(nomeCampo) == -1)
        {
            // tabela.rows[iTotal].cells[4] é onde se encontra o valor do cheque.
            var valor = parseFloat(tabela.rows[iTotal].cells[4].innerHTML.replace("R$", "").replace(" ", "").replace(".", "").replace(",", "."));
            total -= !isNaN(valor) ? valor : 0;
        }
    }

    return total;
}

function excluirItemCheque(nomeTabelaCheques, linha, nomeCampo, callbackExcluir, nomeControleFormaPagto)
{
    // Verifica se a função foi chamada a partir de um popup
    if (FindControl("hdfPopupCheque", "input") != null)
    {
        window.opener.excluirItemCheque(nomeTabelaCheques, linha, nomeCampo);
        
        var tabela = document.getElementById("tbChequePagto");
        var tabelaOpener = window.opener.document.getElementById(nomeTabelaCheques);
        duplicarTabela(tabela, tabelaOpener);
        atualizaTotal();
        
        return;
    }

    var tabela = document.getElementById(nomeTabelaCheques);

    // Exclui o produto da tabela
    document.getElementById(nomeTabelaCheques + "_rowCheque" + linha).style.display = "none";
    document.getElementById(nomeTabelaCheques + "_rowCheque" + linha).setAttribute("excluida", 1);

    // Recupera o total da linha antes de ser excluída (escondida)
    var totalLinha = new Number(document.getElementById(nomeTabelaCheques + "_totalCheque" + linha).innerHTML.replace("R$", "").replace(" ", "").replace(".", "").replace(",", ".")).toFixed(2);
    
    // Recalcula o valor total dos cheques
    totalCheques -= totalLinha;
    tabela.setAttribute("totalCheques", totalCheques);
    
    try
    {
        getPagto(nomeCampo).value = getTotalChequesCampo(tabela, nomeCampo).toFixed(2).replace(".", ",");
        var funcao = getPagto(nomeCampo).getAttribute("onchange");
        if (typeof funcao == "string")
            eval(funcao);
    }
    catch (err) { }

    if (callbackExcluir != "" && callbackExcluir != undefined)
        eval(callbackExcluir + "()");

    callbackControleFormaPagto(nomeControleFormaPagto);
    return false;
}

function getPagto(nomeCampo)
{
    if (typeof nomeCampo === undefined || nomeCampo == null || nomeCampo == "")
    {
        // Procura o campo na tela
        var valor = FindControl("txtPagto", "input");
        if (valor == null)
            valor = FindControl("txtValor", "input");
        if (valor == null)
            valor = FindControl("txtSinal", "input");
            
        return valor;
    }
    else
        return document.getElementById(nomeCampo);
}

function duplicarTabela(novaTabela, tabela)
{
    var prop = ["innerHTML", "className", "cellPadding", "cellSpacing", "style.borderCollapse"];

    for (var p = 0; p < prop.length; p++)
        eval("novaTabela." + prop[p] + " = tabela." + prop[p]);
}

function limparTabelaCheques(tabela)
{
    tabela.innerHTML = "";
    tabela.setAttribute("totalCheques", null);
    tabela.setAttribute("countCheques", null);
}