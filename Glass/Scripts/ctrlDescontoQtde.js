var controlesDescontoQtde = new Array();

// ------------------------------------------
// Função que retorna a variável do controle.
// ------------------------------------------
function getVar(idControle)
{
    // Retorna a variável com os dados do controle
    return eval(idControle);
}

// ------------------------------------------------------------------------
// Função que indica se o controle de desconto por quantidade está visível.
// ------------------------------------------------------------------------
function descQtde_getControleVisible(nomeControle)
{
    try
    {
        var div = document.getElementById(nomeControle + "_divDescontoQtde");
        return div.style.display != "none";
    }
    catch (err)
    {
        return false;
    }
}

// ---------------------------------------
// Função que retorna o valor de um campo.
// ---------------------------------------
function descQtde_getValorCampo(campo, parseTo)
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
function descQtde_setValorCampo(campo, valor, executarFuncao)
{
    // Verifica se o campo é válido
    if (campo == null)
        throw new Error();

    // Altera o valor do campo de acordo com o tipo
    switch (campo.tagName.toLowerCase())
    {
        case "input":
            campo.value = valor;
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

// -----------------------------
// Recupera o código do produto.
// -----------------------------
function descQtde_getIdProduto(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).ProdutoID);
        return descQtde_getValorCampo(campo, "int");
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------------
// Recupera a quantidade do produto.
// ---------------------------------
function descQtde_getQuantidade(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).Quantidade);
        return descQtde_getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// -----------------------------
// Recupera o código do cliente.
// -----------------------------
function descQtde_getIdCliente(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).ClienteID);
        return descQtde_getValorCampo(campo, "int");
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------
// Recupera o tipo de entrega.
// ---------------------------
function descQtde_getTipoEntrega(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).TipoEntrega);
        return descQtde_getValorCampo(campo, "int");
    }
    catch (err)
    {
        return 0;
    }
}

// --------------------------------
// Recupera se o cliente é revenda.
// --------------------------------
function descQtde_getRevenda(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).Revenda);
        return descQtde_getValorCampo(campo, "bool");
    }
    catch (err)
    {
        return false;
    }
}

// --------------------------------------------------------
// Recupera se o tipo de venda (apenas pedido) é reposição.
// --------------------------------------------------------
function descQtde_getReposicao(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).Reposicao);
        return descQtde_getValorCampo(campo, "bool");
    }
    catch (err)
    {
        return false;
    }
}

// ------------------------
// Altera o valor unitário.
// ------------------------
function descQtde_setValorUnitario(nomeControle, valor)
{
    var campo = document.getElementById(getVar(nomeControle).ValorUnitario);
    descQtde_setValorCampo(campo, valor, getVar(nomeControle).ExecutarOnChangeValorUnitario);
}

// ----------------------------
// Recupera o total do produto.
// ----------------------------
function descQtde_getTotal(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).Total);
        return descQtde_getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------
// Altera o valor total.
// ---------------------
function descQtde_setTotal(nomeControle, valor)
{
    var campo = document.getElementById(getVar(nomeControle).Total);
    descQtde_setValorCampo(campo, valor, false);
}

// ----------------------------------------
// Retorna o percentual máximo de desconto.
// ----------------------------------------
function descQtde_getPercDescontoQtdeMax(nomeControle)
{
    try
    {
        var campo = document.getElementById(nomeControle + "_lblPercDescQtde");
        return descQtde_getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// -------------------------------------------
// Retorna o percentual de desconto de tabela.
// -------------------------------------------
function descQtde_getPercDescontoTabela(nomeControle) {
    try {
        var campo = document.getElementById(nomeControle + "_lblDescTabela");
        return descQtde_getValorCampo(campo, "float");
    }
    catch (err) {
        return 0;
    }
}

// ---------------------------------------
// Retorna o percentual do desconto usado.
// ---------------------------------------
function descQtde_getPercDescontoQtdeAtual(nomeControle)
{
    try
    {
        return getVar(nomeControle).RetornarDescontoAtual ? getVar(nomeControle).PercDescontoAtualInterno :
            descQtde_getPercDescontoQtde(nomeControle);
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------------------
// Retorna o percentual do desconto usado.
// ---------------------------------------
function descQtde_getPercDescontoQtde(nomeControle)
{
    try
    {
        if (!descQtde_getControleVisible(nomeControle))
            throw new Error();
        
        var campo = document.getElementById(nomeControle + "_txtPercDescQtde");
        return descQtde_getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// ------------------------------------------------------------------
// Função que verifica se há desconto por quantidade para um produto.
// ------------------------------------------------------------------
function descQtde_getDescontoQtde(nomeControle, usarDescontoQtde, atualizarValorUnit, forcarEsconderControle, callback, callbackValorUnit)
{
    if (document.getElementById(nomeControle + "_divDescontoQtde") == null)
        return;

    if (usarDescontoQtde) 
    {
        // Exibe na tela o percentual de desconto que o cliente possui
        var descTabela = ctrlDescontoQtde.GetDescontoTabela(descQtde_getIdProduto(nomeControle), descQtde_getIdCliente(nomeControle)).value;
        if (parseFloat(descTabela.replace(",", ".")) > 0) {
            FindControl("lblDescTabela", "span").innerHTML = descTabela;
            FindControl("trDescontoTabela", "tr").style.display = "";
        }
        else
            FindControl("trDescontoTabela", "tr").style.display = "none";
    
        var percDesconto = 0;
        if (descQtde_getIdProduto(nomeControle) > 0)
        {
            var resposta = ctrlDescontoQtde.GetPercDescontoQtde(descQtde_getIdProduto(nomeControle), descQtde_getQuantidade(nomeControle)).value.split(';');

            if (resposta[0] == "Erro")
                alert(resposta[1]);
            else
                percDesconto = parseFloat(resposta[1].replace(",", "."));

            document.getElementById(nomeControle + "_lblPercDescQtde").innerHTML = resposta[1];
        }

        document.getElementById(nomeControle + "_divDescontoQtde").style.display = percDesconto > 0 && !forcarEsconderControle ? "" : "none";
        var campoDesc = document.getElementById(nomeControle + "_txtPercDescQtde");

        if (percDesconto == 0 || (campoDesc.value != "" && percDesconto < parseFloat(campoDesc.value.replace(",", "."))))
            campoDesc.value = "";

        if (callback != "" && callback != callbackValorUnit)
            eval(callback + "()");
    }

    if (atualizarValorUnit)
        descQtde_atualizaValorUnit(nomeControle, callbackValorUnit);
}

// ------------------------------------------------
// Função que atualiza o valor unitário do produto.
// ------------------------------------------------
function descQtde_atualizaValorUnit(nomeControle, callbackValorUnit)
{
    if (getVar(nomeControle).ValorUnitario == "")
        return;
    
    var valorUnit = ctrlDescontoQtde.GetValorTabela(descQtde_getIdProduto(nomeControle), descQtde_getTipoEntrega(nomeControle),
        descQtde_getIdCliente(nomeControle), descQtde_getRevenda(nomeControle), descQtde_getReposicao(nomeControle), 
        descQtde_getPercDescontoQtde(nomeControle)).value;

    descQtde_setValorUnitario(nomeControle, valorUnit);

    getVar(nomeControle).RetornarDescontoAtual = true;
    
    if (callbackValorUnit != "")
        eval(callbackValorUnit + "()");

    getVar(nomeControle).RetornarDescontoAtual = false;
}

// ------------------------------------------------------------
// Função que retorna o nome do controle a partir do validador.
// ------------------------------------------------------------
function getNomeControleFromValDQ(val)
{
    // Recupera o nome do controle removendo o final do nome do validador
    if (val.id.indexOf("_div") > -1)
        return val.id.substr(0, val.id.indexOf("_div"));
    else
        return val.id.substr(0, val.id.indexOf("_ctv"));
}

// -----------------------------------------------------
// Função que valida o percentual de desconto escolhido.
// -----------------------------------------------------
function validaPercDesconto(val, args)
{
    var nomeControle = getNomeControleFromValDQ(val);

    if (!descQtde_getControleVisible(nomeControle)) {
        var campoDesc = document.getElementById(nomeControle + "_txtPercDescQtde");
        setValorCampo(campoDesc, "", false);
        return;
    }

    var percDescontoQtdeMax = descQtde_getPercDescontoQtdeMax(nomeControle);

    // Verifica se o desconto por cliente será considerado no desconto máximo que pode ser lançado
    if (ctrlDescontoQtde.ImpedirDescontoSomativo().value == "true")
        percDescontoQtdeMax -= descQtde_getPercDescontoTabela(nomeControle);

    var descAplicado = descQtde_getPercDescontoQtde(nomeControle);

    if (descAplicado > percDescontoQtdeMax && parseFloat(descAplicado.toString().replace(',', '.')) > 0) {
        alert("Percentual máximo de desconto deve ser de " + percDescontoQtdeMax.toString().replace(".", ",") + "%.");
        document.getElementById(nomeControle + "_txtPercDescQtde").value = "";
        args.IsValid = false;
    }
}

// ---------------------------------------------------------------------------
// Função executada no submit da página.
// Atualiza o valor total do produto, removendo o valor descontado atualmente.
// ---------------------------------------------------------------------------
function atualizaTotalDescontoQtde()
{
    for (i = 0; i < controlesDescontoQtde.length; i++)
    {
        try
        {
            var valorAtual = descQtde_getTotal(controlesDescontoQtde[i]);
            var valorDescontoQtde = getVar(controlesDescontoQtde[i]).ValorDescontoAtual;
            descQtde_setTotal(controlesDescontoQtde[i], "R$ " + (valorAtual - valorDescontoQtde).toFixed(2).replace(".", ","));
        }
        catch (err) { }
    }
}