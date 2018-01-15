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

// -----------------------------------
// Indica se o pedido é Fast Delivery.
// -----------------------------------
function getFastDelivery(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoFastDelivery);
        return getValorCampo(campo, "bool");
    }
    catch (err)
    {
        return false;
    }
}

// -----------------------------------------
// Retorna o total do pedido (sem desconto).
// -----------------------------------------
function getTotalSemDesconto(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoTotalSemDesconto);
        return getValorCampo(campo, "float") / (1 + (getVar(nomeControle).IsPedidoFastDelivery ? getVar(nomeControle).TaxaFastDelivery / 100 : 0));
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------
// Retorna o tipo de desconto.
// ---------------------------
function getTipoDesconto(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoTipoDesconto);
        return getValorCampo(campo, "int");
    }
    catch (err)
    {
        return 2;
    }
}

// ----------------------------
// Retorna o desconto aplicado.
// ----------------------------
function getDesconto(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoDesconto);
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// ----------------------------
// Retorna o desconto esperado.
// ----------------------------
function getDescontoEsperado(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoDesconto);
        return getValorCampo(campo, "float") / (1 + (getVar(nomeControle).TaxaFastDelivery / 100));
    }
    catch (err)
    {
        return 0;
    }
}

// -------------------------------------
// Retorna o valor do desconto aplicado.
// -------------------------------------
function getValorDesconto(nomeControle)
{
    try
    {
        return getDesconto(nomeControle) * (getTipoDesconto(nomeControle) == 2 ? 1 : 
            getTotalSemDesconto(nomeControle));
    }
    catch (err)
    {
        return 0;
    }
}

// -------------------------------------
// Retorna o valor do desconto esperado.
// -------------------------------------
function getValorDescontoEsperado(nomeControle)
{
    try
    {
        return getDescontoEsperado(nomeControle) * (getTipoDesconto(nomeControle) == 2 ? 1 : 
            getTotalSemDesconto(nomeControle));
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------------------
// Calcula os dados de desconto do pedido.
// ---------------------------------------
function calculaDadosDesconto(nomeControle)
{
    var exibir = getFastDelivery(nomeControle);
    var span = document.getElementById(nomeControle);
    
    span.style.display = exibir && getTipoDesconto(nomeControle) != 1 ? "" : "none";
    if (!exibir) return;
    
    var lblValorFinal = document.getElementById(nomeControle + "_lblValorFinal");
    var lblDescontoEsperado = document.getElementById(nomeControle + "_lblDescontoEsperado");
    var lblValorFinalEsperado = document.getElementById(nomeControle + "_lblValorFinalEsperado");
    
    lblValorFinal.innerHTML = "R$ " + ((getTotalSemDesconto(nomeControle) - getValorDesconto(nomeControle)) * 
        (1 + (getVar(nomeControle).TaxaFastDelivery / 100))).toFixed(2).replace(".", ",");
    
    lblDescontoEsperado.innerHTML = (getTipoDesconto(nomeControle) == 2 ? "R$ " : "") +
        getDescontoEsperado(nomeControle).toFixed(2).replace(".", ",") + 
        (getTipoDesconto(nomeControle) == 1 ? "%" : "");
        
    lblValorFinalEsperado.innerHTML = "R$ " + ((getTotalSemDesconto(nomeControle) - getValorDescontoEsperado(nomeControle)) * 
        (1 + (getVar(nomeControle).TaxaFastDelivery / 100))).toFixed(2).replace(".", ",");
}