// ------------------------------------------
// Função que retorna a variável do controle.
// ------------------------------------------
function getVar(idControle)
{
    // Retorna a variável com os dados do controle
    return eval(idControle);
}

// ---------------------------------------
// Função que retorna o valor de um campo.
// ---------------------------------------
function getValorCampo(campo, parseTo)
{
    // Verifica se o campo é válido
    if (campo == null)
        throw new Error();
    
    // Recupera o valor de acordo com o tipo de campo
    switch (campo.tagName.toLowerCase())
    {
        case "input":
            if (campo.type.toLowerCase() != "checkbox")
                var valor = campo.value;
            else
                var valor = campo.checked;
            break;
            
        case "select":
            var valor = campo.value;
            break;
            
        case "span":
            var valor = campo.innerHTML;
            break;
            
        default:
            throw new Error();
    }
    
    // Converte o valor para int ou float
    parseTo = parseTo != null && parseTo !== undefined ? parseTo.toLowerCase() : "";
    switch (parseTo)
    {
        case "int":
            var retorno = parseInt(valor, 10);
            if (isNaN(retorno))
                throw new Error();
            break;

        case "float":
            if (valor.toString().indexOf('$') > -1)
                valor = valor.replace("R$", "").replace(" ", "").replace(/\./g, "");
            var retorno = parseFloat(valor.toString().replace(",", "."));
            if (isNaN(retorno))
                throw new Error();
            break;
            
        case "bool":
            var retorno = valor.toString().toLowerCase() == "true";
            break;
        
        case "date":
            var temp = valor.toString().split("/");
            var retorno = new Date(temp[2], parseInt(temp[1], 10) - 1, temp[0]);
            break;
            
        default:
            var retorno = valor;
            break;
    }
    
    // Retorna o valor
    return retorno;
}

// --------------------------------------
// Função que altera o valor de um campo.
// --------------------------------------
function setValorCampo(campo, valor, executarFuncao)
{
    // Verifica se o campo é válido
    if (campo == null)
        throw new Error();
    
    // Altera o valor do campo de acordo com o tipo
    switch (campo.tagName.toLowerCase())
    {
        case "input":
            campo.value = valor > 0 ? valor : "";
            break;
            
        case "select":
            campo.value = valor;
            break;
            
        case "span":
            campo.innerHTML = valor;
            break;
            
        default:
            throw new Error();
    }
    
    // Verifica se a função OnChange ou OnBlur deve ser executada
    executarFuncao = executarFuncao == true ? true : false;
    if (!executarFuncao)
        return;
    
    // Recupera a função que será executada
    var funcao = campo.getAttribute("onchange");
    if (funcao == null)
        funcao = campo.getAttribute("onblur");
    
    // Garante que a função foi recuperada
    if (funcao == null)
        return;
    
    // Altera a referência do controle de 'this' para 'document.getElementById'
    // ('this' referencia o objeto 'window')
    while (funcao.indexOf("this") > -1)
        funcao = funcao.replace("this", "document.getElementById('" + campo.id + "')");
    
    // Executa a função
    eval(funcao);
}

// ----------------------------------------------------------------
// Função que recupera os beneficiamentos selecionados no controle.
// ----------------------------------------------------------------
function getBeneficiamentos(nomeControle)
{
    // Variável de retorno
    var ids = new Array()
    
    // Recupera a tabela do beneficiamento
    var tabela = document.getElementById(nomeControle + "_tblBenef");
    if (tabela == null)
        return;
    
    // Percorre a tabela
    for (r = 0; r < tabela.rows.length; r++)
        for (c = 0; c < tabela.rows[r].cells.length; c++)
        {
            // Recupera o controles pai
            var pai = tabela.rows[r].cells[c].getElementsByTagName("input")[0];
            if (pai == null || !pai.checked)
                continue;
            
            // Recupera os controles filhos
            var filhos = tabela.rows[r].cells[c].getElementsByTagName("table");
            filhos = filhos.length > 0 ? filhos[0] : null;
            
            // Verifica se há filhos para esse beneficiamento
            if (filhos != null)
            {
                // Percorre a tabela dos filhos
                for (fr = 0; fr < filhos.rows.length; fr++)
                    for (fc = 0; fc < filhos.rows[fr].cells.length; fc++)
                    {
                        // Verifica se o filho está marcado
                        var filho = filhos.rows[fr].cells[fc].getElementsByTagName("input")[0];
                        if (filho != null && filho.checked)
                            ids.push(filho.parentNode.getAttribute("idBeneficiamento"));
                    }
            }
            else
                ids.push(tabela.rows[r].cells[c].getAttribute("idBeneficiamento"));
        }
    
    // Retorna os beneficiamentos selecionados
    return ids.join(",");
}

// ------------------------------------------------------
// Função que exibe a tabela de filhos do beneficiamento.
// ------------------------------------------------------
function exibirFilhos(nomeControle, pai)
{
    if (pai == null)
        return;
    
    var prefixoBenef = pai.parentNode.getAttribute("prefixoBenef");
    var filhos = document.getElementById(nomeControle + "_" + prefixoBenef + "tblFilhos");
    if (filhos != null)
        filhos.style.display = pai.checked ? "" : "none";
}

// ----------------------------------------------------------------
// Função que exibe todas as tabelas de filhos dos beneficiamentos.
// ----------------------------------------------------------------
function exibirTodosOsFilhos(nomeControle)
{
    var tabela = document.getElementById(nomeControle + "_tblBenef");
    if (tabela == null)
        return;

    var inputs = tabela.getElementsByTagName("input");
    for (i = 0; i < inputs.length; i++)
    {
        if (inputs[i].type != "checkbox" || inputs[i].id.indexOf("chkPai") == -1)
            continue;
        
        exibirFilhos(nomeControle, inputs[i]);
    }
}