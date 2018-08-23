var countItem = new Object();

// Inclui item na tabela dinamicamente
function addItem(items, itemsName, nomeTabela, objId, hdfObjIdName, valor, lblValor, callbackExcluir, inserirTopo, podeExcluir) {

    // Monta tabela dinamicamente
    var tabela = document.getElementById(nomeTabela);
    
    // Altera a formatação da tabela
    tabela.className = "gridStyle";
    tabela.cellPadding = "0";
    tabela.cellSpacing = "0";
    tabela.style.borderCollapse = "collapse";

    // Cria títulos para a tabela
    if (!countItem[nomeTabela] || tabela.rows.length == 0)
    {
        var rowHeader = tabela.insertRow(0);
        rowHeader.align = "left";

        // Adiciona uma celula na posição 0
        var th = document.createElement("th");
        rowHeader.appendChild(th);

        for (i = 0; i < itemsName.length; i++) {
            var th = document.createElement("th");
            th.setAttribute('nowrap', 'nowrap');
            th.innerHTML = itemsName[i];
            rowHeader.appendChild(th);
        }

        countItem[nomeTabela] = 1;
    }

    // Adiciona uma nova linha na tabela
    row = tabela.insertRow(inserirTopo ? 1 : countItem[nomeTabela]);
    row.id = nomeTabela + "_row" + countItem[nomeTabela];
    
    // Se um identificador de objeto tiver sido passado, salva o mesmo no atributo da linha e no hiddenfield
    if (objId != null && objId != "") {
        row.setAttribute("objId", objId);

        // Se houver um hiddenfield para adicionar id's
        if (hdfObjIdName != null) {
            row.setAttribute("hdfObjIdName", hdfObjIdName);

            // Adiciona o id do objeto selecionado ao hiddenfield que guarda todos os ids ja selecionados
            FindControl(hdfObjIdName, "input").value += objId + ",";
        }
    }

    // Se um valor para o objeto tiver sido passado, salva o mesmo no atributo da linha e exibe no label
    if (valor != null && valor != "") {
        row.setAttribute("valor", valor);
        row.setAttribute("lblValor", lblValor);

        // Pega o valor do label que contem o total
        var total = FindControl(lblValor, "span").innerHTML;

        // Incrementa o valor do label passado com o valor passado
        total = parseFloat(valor.replace(".", "").replace("R$", "").replace(" ", "").replace(",", ".")) +
            parseFloat(total.replace(".", "").replace("R$", "").replace(" ", "").replace(",", "."));

        // Exibe o valor total no label
        FindControl(lblValor, "span").innerHTML = "R$ " + total.toFixed(2).toString().replace(".", ",");
    }
    
    // Adiciona botão excluir na coluna 1 da linha que está sendo inserida
    cell = row.insertCell(0);
    callbackExcluir = typeof callbackExcluir != 'undefined' && callbackExcluir != null && callbackExcluir != "" ? callbackExcluir : "";

    if (typeof podeExcluir == 'undefined' || podeExcluir) {
        cell.innerHTML = "<a href=\"#\" onclick=\"return removeItem(" + countItem[nomeTabela] + ",'" + nomeTabela + "', '" + callbackExcluir + "');\">" +
            "<img src=\"../Images/ExcluirGrid.gif\" border=\"0\" title=\"Excluir\"/></a>";
    }

    // Adiciona o item passado na tabela
    for (i = 0; i < items.length; i++) {
        cell = row.insertCell(i + 1);
        cell.innerHTML = items[i];
    }

    // Incrementa a quantidade de itens adicionados na tabela
    countItem[nomeTabela]++;

    drawAlternateLines(nomeTabela);

    return false;
}

// Limpa a tabela
function resetGrid(nomeTabela)
{
    var tabela = document.getElementById(nomeTabela);
    while (tabela.rows.length > 0)
        tabela.deleteRow(0);
}

// Remove o produto da tabela
function removeItem(linha, nomeTabela, callbackExcluir) {

    // Se a linha possuir um identificador de objeto(objId), remove o mesmo do hiddenfield que esta armazenado
    if (document.getElementById(nomeTabela + "_row" + linha).getAttribute("objId") != null) {
    
        // Pega o nome do hidden field que guarda os ids dos objetos
        var hdfObjIdName = document.getElementById(nomeTabela + "_row" + linha).getAttribute("hdfObjIdName");
        
        // Pega o conteúdo do mesmo hiddenfield
        var hdfObjIds = FindControl(hdfObjIdName, "input").value;
        
        var objs = hdfObjIds.substring(0, hdfObjIds.lastIndexOf(',')).split(',');
        var objAExcluir = document.getElementById(nomeTabela + "_row" + linha).getAttribute("objId");
        var newObjs = ""; // Novo vetor de objs

        // Cria um novo vetor de objs, tirando o objId que foi excluido, porém caso o mesmo ocorra duas vezes exclui apenas uma vez
        var excluido = false;
        for (i = 0; i < objs.length; i++) {
            if (objAExcluir == objs[i] && !excluido)
                excluido = true;
            else
                newObjs += objs[i] + ",";
        }

        // Atribui o novo vetor criado ao hidden field que guarda os ids dos objs adicionados
        FindControl(hdfObjIdName, "input").value = newObjs.replace(",,", ",");
    }
    
    // Se a linha possuir um valor para o objeto, decrementa o valor do label que exibe o total
    if (document.getElementById(nomeTabela + "_row" + linha).getAttribute("valor") != null)
    {
        // Recupera o valor do objeto salvo na linha antes de ser excluída
        var valorLinha = document.getElementById(nomeTabela + "_row" + linha).getAttribute("valor");

        // Recupera o label que possui o total
        var labelValor = FindControl(document.getElementById(nomeTabela + "_row" + linha).getAttribute("lblValor"), "span");

        // Recalcula o valor total        
        var total = subtract(labelValor.innerHTML, valorLinha);
        labelValor.innerHTML = "R$ " + new Number(total).toFixed(2).toString().replace(".", ",");
    }

    document.getElementById(nomeTabela + "_row" + linha).style.display = "none";
    
    if (callbackExcluir != "")
        eval(callbackExcluir + "(document.getElementById(nomeTabela + '_row' + linha))");

    drawAlternateLines(nomeTabela); 

    return false;
}

// Colore o fundo de linhas alternadas da grid
function drawAlternateLines(nomeTabela) {
    drawAlternateLinesEx(nomeTabela, 1);
}

// Colore o fundo de linhas alternadas da grid
function drawAlternateLinesEx(nomeTabela, numeroLinhas) {
    numeroLinhas = typeof numeroLinhas != "number" ? 1 : numeroLinhas;
    var tabela = document.getElementById(nomeTabela);
    var preenche = false;
    
    var isGridStyle = tabela.className.indexOf("gridStyle") > -1;

    var suporte = 1;
    for (i = 1; i < tabela.rows.length; i++) {
        if (tabela.rows[i].style.display != "none") {
            if (isGridStyle)
            {
                if (preenche)
                    tabela.rows[i].className = "alt";
                else
                    tabela.rows[i].className = "";
            }
            else if (preenche)
                tabela.rows[i].style.backgroundColor = "#E4EFF1";
            else
                tabela.rows[i].style.backgroundColor = "#FFFFFF";
            
            suporte++;
            if (suporte > numeroLinhas) {
                preenche = !preenche;
                suporte = 1;
            }
        }
    }
}