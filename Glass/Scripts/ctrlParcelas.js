var atualizandoTotal = false;

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
function Parc_getValorCampo(campo, parseTo)
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
function Parc_setValorCampo(campo, valor, executarFuncao)
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

// ---------------------------------
// Função que retorna o valor total.
// ---------------------------------
function Parc_getValorTotal(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoValorTotal);
        return Parc_getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// --------------------------------------
// Função que retorna o valor da entrada.
// --------------------------------------
function Parc_getValorEntrada(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoValorEntrada);
        return Parc_getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------------------------
// Função que retorna o valor do desconto atual.
// ---------------------------------------------
function Parc_getValorDescontoAtual(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoValorDescontoAtual);
        var valor = Parc_getValorCampo(campo, "float");
        
        // Calcula o valor do desconto pelo tipo selecionado
        var tipo = document.getElementById(getVar(nomeControle).CampoTipoDescontoAtual);
        if (tipo != null)
        {
            tipo = tipo.tagName.toLowerCase() == "select" ? tipo.options[tipo.selectedIndex].text : tipo.value;
            return tipo == "%" || tipo == "1" ? Parc_getValorTotal(nomeControle) * valor / 100 : valor;
        }
        else
            return valor;
    }
    catch (err)
    {
        return 0;
    }
}

// ------------------------------------------------
// Função que retorna o valor do desconto anterior.
// ------------------------------------------------
function Parc_getValorDescontoAnterior(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoValorDescontoAnterior);
        var valor = Parc_getValorCampo(campo, "float");
        
        // Calcula o valor do desconto pelo tipo selecionado
        var tipo = document.getElementById(getVar(nomeControle).CampoTipoDescontoAnterior);
        if (tipo != null)
        {
            tipo = tipo.tagName.toLowerCase() == "select" ? tipo.options[tipo.selectedIndex].text : tipo.value;
            return tipo == "%" || tipo == "1" ? Parc_getValorTotal(nomeControle) * valor / 100 : valor;
        }
        else
            return valor;
    }
    catch (err)
    {
        return 0;
    }
}

// ----------------------------------------------
// Função que retorna o valor do acréscimo atual.
// ----------------------------------------------
function Parc_getValorAcrescimoAtual(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoValorAcrescimoAtual);
        var valor = Parc_getValorCampo(campo, "float");

        // Calcula o valor do acréscimo pelo tipo selecionado
        var tipo = document.getElementById(getVar(nomeControle).CampoTipoAcrescimoAtual);
        if (tipo != null)
        {
            tipo = tipo.tagName.toLowerCase() == "select" ? tipo.options[tipo.selectedIndex].text : tipo.value;
            return tipo == "%" || tipo == "1" ? Parc_getValorTotal(nomeControle) * valor / 100 : valor;
        }
        else
            return valor;
    }
    catch (err)
    {
        return 0;
    }
}

// -------------------------------------------------
// Função que retorna o valor do acréscimo anterior.
// -------------------------------------------------
function Parc_getValorAcrescimoAnterior(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoValorAcrescimoAnterior);
        var valor = Parc_getValorCampo(campo, "float");
        
        // Calcula o valor do acréscimo pelo tipo selecionado
        var tipo = document.getElementById(getVar(nomeControle).CampoTipoAcrescimoAnterior);
        if (tipo != null)
        {
            tipo = tipo.tagName.toLowerCase() == "select" ? tipo.options[tipo.selectedIndex].text : tipo.value;
            return tipo == "%" || tipo == "1" ? Parc_getValorTotal(nomeControle) * valor / 100 : valor;
        }
        else
            return valor;
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------------------
// Função que retorna o valor do desconto.
// ---------------------------------------
function Parc_getValorDesconto(nomeControle)
{
    return Parc_getValorDescontoAtual(nomeControle) - Parc_getValorDescontoAnterior(nomeControle);
}

// ----------------------------------------
// Função que retorna o valor do acréscimo.
// ----------------------------------------
function Parc_getValorAcrescimo(nomeControle)
{
    return Parc_getValorAcrescimoAtual(nomeControle) - Parc_getValorAcrescimoAnterior(nomeControle);
}

// -------------------------------------------------
// Função que retorna o número de parcelas visíveis.
// -------------------------------------------------
function Parc_getParcelasVisiveis(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoParcelasVisiveis);
        var retorno = Parc_getExibirParcelas(nomeControle) ? Parc_getValorCampo(campo, "int") : 0;
        return retorno < getVar(nomeControle).TotalParcelas ? retorno : getVar(nomeControle).TotalParcelas;
    }
    catch (err)
    {
        return 0;
    }
}

// -----------------------------------------------------------------------------
// Função que indica se as datas serão calculadas automaticamente pelo controle.
// -----------------------------------------------------------------------------
function Parc_getCalcularData(nomeControle)
{
    try
    {
        var campo = getVar(nomeControle).CalcularData;
        return campo;
    }
    catch (err)
    {
        return 0;
    }
}

// ------------------------------------------------
// Função que indica se as parcelas serão exibidas.
// ------------------------------------------------
function Parc_getExibirParcelas(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoExibirParcelas);
        return Parc_getValorCampo(campo, "bool");
    }
    catch (err)
    {
        return false;
    }
}

// --------------------------------------------------------------------
// Função que indica se o campo de valor da parcela está sendo exibido.
// --------------------------------------------------------------------
function Parc_exibirValor(nomeControle)
{
    try
    {
        var campo = document.getElementById(nomeControle + "_Parc1_txtValor");
        return campo != null;
    }
    catch (err)
    {
        return false;
    }
}

// --------------------------------------------------
// Função que indica se as parcelas serão calculadas.
// --------------------------------------------------
function Parc_getCalcularParcelas(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoCalcularParcelas);
        return Parc_getValorCampo(campo, "bool");
    }
    catch (err)
    {
        return false;
    }
}

// ---------------------------------------------
// Função que retorna o valor utilizado da obra.
// ---------------------------------------------
function Parc_getValorObra(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoValorObra);
        return Parc_getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// ----------------------------------------------
// Função que retorna o valor total das parcelas.
// ----------------------------------------------
function Parc_getValorParcelas(nomeControle)
{
    var retorno = Parc_getValorTotal(nomeControle) - Parc_getValorEntrada(nomeControle) - Parc_getValorObra(nomeControle);
    if (!getVar(nomeControle).AplicarJuros)
        retorno += Parc_getValorAcrescimo(nomeControle) - Parc_getValorDesconto(nomeControle);
    else if (getVar(nomeControle).JurosCompostos)
    {
        var juros = getJurosParcelas(nomeControle, false);
        
        if (Parc_getParcelasVisiveis(nomeControle) > 1)
        {
            var aplicarJuros = false;
            for (n = 0; n < juros.length; n++)
                if (juros[n])
                {
                    aplicarJuros = true;
                    break;
                }
            
            var valorTotal = retorno * (1 + ((aplicarJuros ? Parc_getTaxaJurosParcela(nomeControle) : 0) / 100));
            var valorBaseParcela = (valorTotal / Parc_getParcelasVisiveis(nomeControle));
            
            retorno = aplicarJuros ? valorBaseParcela : 0;
            for (n = (aplicarJuros ? 1 : 0); n < Parc_getParcelasVisiveis(nomeControle); n++)
            {
                var valorParcela = valorBaseParcela * (1 + ((juros[n] ? Parc_getTaxaJurosParcela(nomeControle) : 0) / 100));
                retorno += valorParcela;
            }
        }
        else
            retorno = retorno * (1 + ((juros[0] ? Parc_getTaxaJurosParcela(nomeControle) : 0) / 100));
        
        retorno += Parc_getValorAcrescimo(nomeControle) - Parc_getValorDesconto(nomeControle);
    }
    
    return retorno;
}

// -----------------------------------------------------------------
// Função que retorna o texto que representa os prazos das parcelas.
// -----------------------------------------------------------------
function Parc_getTextoParcelas(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoTextoParcelas);
        return Parc_getValorCampo(campo, "");
    }
    catch (err)
    {
        return "";
    }
}

// --------------------------------------------------------------------
// Função que retorna a data base para o cálculo do prazo das parcelas.
// --------------------------------------------------------------------
function Parc_getDataBase(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoDataBase);

        if (campo.value == "")
            return new Date().format("dd/mm/yyyy");
        
        return Parc_getValorCampo(campo, "date");
    }
    catch (err)
    {
        return null;
    }
}

// -----------------------------------------------
// Função que retorna a taxa de juros por parcela.
// -----------------------------------------------
function Parc_getTaxaJurosParcela(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoTaxaJurosParcela);
        return Parc_getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// ----------------------------------------------------------------
// Função que atualiza nas parcelas a forma de pagamento principal.
// ----------------------------------------------------------------
function Parc_atualizaFormasPagto(nomeControle)
{
    var campo = document.getElementById(getVar(nomeControle).CampoFormaPagto);
    if (!getVar(nomeControle).IsCompra || campo == null)
        return;

    var formaPagto = 0;

    // Recupera a forma de pagamento principal
    try { formaPagto = Parc_getValorCampo(campo, "int"); }
    catch (err) { return; }

    // Atribui a forma de pagamento aos campos
    var nomeParcelas = nomeControle + "_Parc";
    for (fp = 0; fp < getVar(nomeControle).TotalParcelas; fp++)
    {
        var nomeCelula = nomeParcelas + (fp + 1).toString();
        document.getElementById(nomeCelula + "_drpFormaPagto").value = formaPagto;
    }
}

// ----------------------------------------------
// Função que altera a visibilidade das parcelas.
// ----------------------------------------------
function Parc_visibilidadeParcelas(nomeControle, callbackTotal, splitChar)
{
    if (getVar(nomeControle).ParentID != "" && document.getElementById(getVar(nomeControle).ParentID).style.display == "none")
        return;

    var nomeParcelas = nomeControle + "_Parc";
    var numParcVisiveis = Parc_getParcelasVisiveis(nomeControle);
    
    for (n = 0; n < getVar(nomeControle).TotalParcelas; n++)
    {
        var nomeCelula = nomeParcelas + (n + 1).toString();
        document.getElementById(nomeCelula).style.display = n < numParcVisiveis ? "" : "none";
    }
    
    var linhas = document.getElementById(nomeControle + "_tblParcelas").rows;
    for (n = 0; n < linhas.length; n++)
    {
        var exibir = false;
        for (m = 0; m < linhas[n].cells.length; m++)
            if (linhas[n].cells[m].style.display != "none")
            {
                exibir = true;
                break;
            }
        
        linhas[n].style.display = exibir ? "" : "none";
    }

    if (Parc_getCalcularParcelas(nomeControle) && Parc_getExibirParcelas(nomeControle))
        Parc_calcularValorParcela(nomeControle, 0, splitChar);
    
    callbackTotal = typeof callbackTotal != 'undefined' && callbackTotal != null ? callbackTotal : "";
    Parc_atualizarValorTotal(nomeControle, callbackTotal);

    //Parc_atualizaDiasParcelas(nomeControle);
    Parc_habilitar(nomeControle, getVar(nomeControle).Habilitado, false);
}

// ----------------------------------------
// Função que calcula o valor das parcelas.
// ----------------------------------------
function Parc_calcularValorParcela(nomeControle, inicio, splitChar)
{
    var nomeParcelas = nomeControle + "_Parc";
    var valorParcelasIgnorar = 0;
    for (n = 0; n < inicio; n++)
    {
        try
        {
            var valor = nomeParcelas + (n + 1).toString() + "_txtValor";
            valor = Parc_getValorCampo(document.getElementById(valor), "float");
        }
        catch (err)
        {
            valor = 0;
        }
        
        valorParcelasIgnorar += valor;
    }
    
    if (inicio > 0 && valorParcelasIgnorar <= 0)
    {
        Parc_calcularValorParcela(nomeControle, 0);
        return;
    }
    
    var valorParcela = (Parc_getValorParcelas(nomeControle) - valorParcelasIgnorar) / (Parc_getParcelasVisiveis(nomeControle) - inicio);
    var prazos = Parc_getTextoParcelas(nomeControle);
    if (prazos.length > 0 && Parc_getDataBase(nomeControle) != null)
    {
        splitChar = typeof splitChar == "undefined" || splitChar == null || splitChar.length == 0 ? "," : splitChar;
        prazos = prazos.split(splitChar);
    }
    else
    {
        prazos = "";
        dataBase = null;
    }
    
    inicio = typeof inicio == "undefined" || typeof inicio != "number" ? 0 : inicio;
    for (n = inicio; n < getVar(nomeControle).TotalParcelas; n++)
    {
        var valor = nomeParcelas + (n + 1).toString() + "_txtValor";
        var data = nomeParcelas + (n + 1).toString() + "_txtData";
        var calcularJuros = nomeParcelas + (n + 1).toString() + "_chkJuros";

        if (getVar(nomeControle).ExibirValores)
        {
            var valorAplicar = valorParcela;
            if (!getVar(nomeControle).JurosCompostos)
            {
                if (document.getElementById(calcularJuros) != null && document.getElementById(calcularJuros).checked)
                    valorAplicar = valorAplicar * (1 + (Parc_getTaxaJurosParcela(nomeControle) * (getVar(nomeControle).JurosCompostos ? (n + 1) : 1) / 100));

                if (getVar(nomeControle).AplicarJuros)
                    valorAplicar += (Parc_getValorAcrescimo(nomeControle) - Parc_getValorDesconto(nomeControle)) / Parc_getParcelasVisiveis(nomeControle);
            }

            document.getElementById(valor).value = n < Parc_getParcelasVisiveis(nomeControle) ? valorAplicar.toFixed(2).replace('.', ',') : "0";
        }
        
        if (document.getElementById(data) != null && (document.getElementById(data).disabled || Parc_getCalcularData(nomeControle)))
        {
            if (n < Parc_getParcelasVisiveis(nomeControle) && prazos.length > n && Parc_getDataBase(nomeControle) != null)
            {
                var dataParcela = Parc_getDataBase(nomeControle);

                var diaBase = dataParcela.getDate();
                var mesBase = dataParcela.getMonth() + 1;
                
                dataParcela.setDate(dataParcela.getDate() + parseInt(prazos[n], 10));

                // Foi necessário colocar a condição "prazos[0] == 30" para que as parcelas sejam recalculadas corretamente
                // na liberação, no caso do dia atual ser menor que dia 15 e escolher parcelas 15/30 dias por exemplo

                // Foi comentada a parte abaixo para que a situação acima funcionasse corretamente, tanto na liberação quanto no pedido
                // tanto em parcelas de 30/60 e parcelas de 15/30
                var dia = /*Parc_getCalcularData(nomeControle) && n == 0 && prazos[0] == 30 ? diaBase :*/dataParcela.getDate();
                dia = dia < 10 ? "0" + dia : dia;
                
                var mes = dataParcela.getMonth() + 1;
                mes = mes < 10 ? "0" + mes : mes;
                
                var ano = dataParcela.getFullYear();

                document.getElementById(data).value = dia + "/" + mes + "/" + ano;
            }
            // Comentado pois estava zerando a data das parcelas do pedido.
            //else if (getVar(nomeControle).LiberarPedido)
                //document.getElementById(data).value = "";
        }
    }

    if (getVar(nomeControle).ExibirValores)
        Parc_ajustarValorUltimaParcela(nomeControle, inicio);
}

// ---------------------------------------------
// Função que corrige o valor da última parcela.
// ---------------------------------------------
function Parc_ajustarValorUltimaParcela(nomeControle, inicio)
{
    if (Parc_getParcelasVisiveis(nomeControle) == 0 || !getVar(nomeControle).ExibirValores)
        return;

    var nomeParcelas = nomeControle + "_Parc";
    var valorParcelasIgnorar = 0;
    for (n = 0; n < inicio; n++)
    {
        var valor = nomeParcelas + (n + 1).toString() + "_txtValor";
        valorParcelasIgnorar += parseFloat(document.getElementById(valor).value.replace(/,/g, "."));
    }

    var valorParcela = parseFloat(((Parc_getValorParcelas(nomeControle) - valorParcelasIgnorar) / (Parc_getParcelasVisiveis(nomeControle) - inicio)).toFixed(2));
    var valor = nomeParcelas + Parc_getParcelasVisiveis(nomeControle) + "_txtValor";
    var calcularJuros = nomeParcelas + Parc_getParcelasVisiveis(nomeControle) + "_chkJuros";
    
    var valorAplicar = valorParcela + parseFloat(Parc_getValorParcelas(nomeControle).toFixed(2)) - valorParcelasIgnorar - (valorParcela * (Parc_getParcelasVisiveis(nomeControle) - inicio));
    if (!getVar(nomeControle).JurosCompostos)
    {
        if (document.getElementById(calcularJuros) != null && document.getElementById(calcularJuros).checked)
            valorAplicar = valorAplicar * (1 + (Parc_getTaxaJurosParcela(nomeControle) / 100));
        
        if (getVar(nomeControle).AplicarJuros)
            valorAplicar += (Parc_getValorAcrescimo(nomeControle) - Parc_getValorDesconto(nomeControle)) / Parc_getParcelasVisiveis(nomeControle);
    }
    
    document.getElementById(valor).value = valorAplicar.toFixed(2).replace('.', ',');
}

// -------------------------------------------
// Função que atualiza o valor total no campo.
// -------------------------------------------
function Parc_atualizarValorTotal(nomeControle, callback)
{
    if (atualizandoTotal || !getVar(nomeControle).ExibirValores)
        return;
    
    atualizandoTotal = true;
    var nomeParcelas = nomeControle + "_Parc";
    var valorParcelas = 0;
    
    for (n = 0; n < getVar(nomeControle).TotalParcelas; n++)
    {
        var nomeCelula = nomeParcelas + (n + 1).toString();
        var valor = document.getElementById(nomeCelula + "_txtValor");
        
        valor = valor.value;
        valor = valor == "" ? 0 : parseFloat(valor.replace(',', '.'));
        valorParcelas += valor;
    }
    
    var campoTotal = document.getElementById(nomeControle + "_txtValorParcelas");
    if (campoTotal.value != valorParcelas.toFixed(2))
        Parc_setValorCampo(campoTotal, parseFloat(valorParcelas.toFixed(2)), true);
    
    if (callback != "")
        eval(callback + "()");
    
    atualizandoTotal = false;
}

// ------------------------------------------------------------------
// Função que calcula o valor das parcelas e depois chama o callback.
// ------------------------------------------------------------------
function Parc_calculaJuros(nomeControle, callbackTotal, callback)
{
    Parc_calcularValorParcela(nomeControle, 0);
    Parc_atualizarValorTotal(nomeControle, callbackTotal);
    if (callback != "")
        eval(callback + "()");
}

// --------------------------------------------------------
// Função que calcula o valor das parcelas após a primeira.
// --------------------------------------------------------
function Parc_calculaDemaisParcelas(nomeControle)
{
    if (Parc_getParcelasVisiveis(nomeControle) > 1)
        Parc_calcularValorParcela(nomeControle, 1);
}

// --------------------------------------------------------------
// Função usada para recuperar os valores dos campos do controle.
// --------------------------------------------------------------
function getValoresFromControleParc(nomeControle, nomeCampo, parseTo, defaultValue, asString)
{
    // Cria o vetor de retorno
    var retorno = new Array(Parc_getParcelasVisiveis(nomeControle));
    
    // Percorre todas as linhas da tabela
    for (i = 0; i < retorno.length; i++)
    {
        try
        {
            // Recupera o valor do campo
            var prefixo = nomeControle + "_Parc" + (i + 1) + "_";
            var campo = document.getElementById(prefixo + nomeCampo);
            retorno[i] = Parc_getValorCampo(campo, parseTo);
        }
        catch (err)
        {
            // Em caso de erro indica que o resultado foi null
            retorno[i] = defaultValue;
        }
    }
    
    // Padroniza o valor do parâmetro asString
    asString = asString == false ? false : true;
    
    // Verifica se o retorno deve ser como string
    if (asString)
    {
        var temp = "";
        for (i = 0; i < retorno.length; i++)
            temp += ";" + retorno[i];
        
        retorno = temp.length > 0 ? temp.substr(1) : "";
    }
    
    // Retorna os valores dos campos
    return retorno;
}

// -------------------------------------------
// Função que retorna os valores das parcelas.
// -------------------------------------------
function getValoresParcelas(nomeControle, asString)
{
    return getValoresFromControleParc(nomeControle, "txtValor", "float", 0, asString);
}

// -----------------------------------------
// Função que retorna as datas das parcelas.
// -----------------------------------------
function getDatasParcelas(nomeControle, asString)
{
    return getValoresFromControleParc(nomeControle, "txtData", "", "", asString);
}

// ----------------------------------------------------
// Função que retorna os dados adicionais das parcelas.
// ----------------------------------------------------
function getAdicionaisParcelas(nomeControle, asString)
{
    return getValoresFromControleParc(nomeControle, "txtAdicional", "", "", asString);
}

// ---------------------------------------------------------
// Função que retorna se as parcelas estão calculando juros.
// ---------------------------------------------------------
function getJurosParcelas(nomeControle, asString)
{
    return getValoresFromControleParc(nomeControle, "chkJuros", "bool", false, asString);
}

// ------------------------------------------------------------
// Função que retorna as formas de pagamento para cada parcela.
// ------------------------------------------------------------
function getFormasPagamento(nomeControle, asString)
{
    return getValoresFromControleParc(nomeControle, "drpFormaPagto", "int", 0, asString);
}

// -------------------------------------------------
// Função que retorna o número de dias das parcelas.
// -------------------------------------------------
function getNumeroDiasParcelas(nomeControle, asString)
{
    var agora = new Date().getTime();    
    var datas = getDatasParcelas(nomeControle, false);
    var retorno = new Array(datas.length);

    for (iDias = 0; iDias < datas.length; iDias++)
    {
        var dataCompString = datas[iDias].split("/");
        var dataComp = new Date(parseInt(dataCompString[2], 10), parseInt(dataCompString[1], 10) - 1, parseInt(dataCompString[0], 10), 0, 0, 0, 0);

        retorno[iDias] = new Date(dataComp.getTime() - agora).getTime();
        /* Chamado 56911.
         * Foi necessário alterar o round para ceil, pois, a data da parcela da liberação estava sendo calculada sempre com um dia a menos. */
        retorno[iDias] = Math.ceil(retorno[iDias] / (1000 * 60 * 60 * 24));

        if (isNaN(retorno[iDias]))
            retorno[iDias] = iDias > 0 ? retorno[iDias - 1] + getVar(nomeControle).DiasSomarDataVazia : getVar(nomeControle).DiasSomarDataVazia;
    }

    // Padroniza o valor do parâmetro asString
    asString = asString == false ? false : true;

    // Verifica se o retorno deve ser como string
    if (asString)
    {
        var temp = "";
        for (i = 0; i < retorno.length; i++)
            temp += ";" + retorno[i];

        retorno = temp.length > 0 ? temp.substr(1) : "";
    }
    
    return retorno;
}

// ---------------------------------------------------------------------
// Função que atualiza o número de dias das parcelas no campo associado.
// ---------------------------------------------------------------------
function Parc_atualizaDiasParcelas(nomeControle)
{
    // Recupera o campo que será alterado
    var campo = getVar(nomeControle).CampoTextoParcelas;
    if (campo == "")
        return;

    campo = document.getElementById(campo);
    if (campo == null)
        return;

    // Atualiza o valor do campo
    campo.value = getNumeroDiasParcelas(nomeControle, true).replace(/;/g, ",");
}

// ------------------------------------------
// Função que habilita os campos do controle.
// ------------------------------------------
function Parc_habilitar(nomeControle, habilitar, isSubmit)
{
    // Percorre todas as linhas da tabela
    for (i = 0; i < Parc_getParcelasVisiveis(nomeControle); i++)
    {
        // Define o prefixo dos campos
        var prefixo = nomeControle + "_Parc" + (i + 1) + "_";

        // Recupera os campos
        var campoValor = document.getElementById(prefixo + "txtValor");
        var campoData = document.getElementById(prefixo + "txtData");
        var imagemData = document.getElementById(prefixo + "imgData");

        // Habilita/desabilita os campos
        if (campoValor != null)
            campoValor.disabled = !habilitar;
        
        if (campoData != null && (getVar(nomeControle).AlterarData || isSubmit))
        {
            campoData.disabled = !habilitar;
            if (!isSubmit)
                imagemData.style.display = habilitar ? "" : "none";
        }
    }

    // Define a situação do controle
    getVar(nomeControle).Habilitado = habilitar;
}

// ------------------------------------------------------------
// Função que retorna o nome do controle a partir do validador.
// ------------------------------------------------------------
function getNomeControleFromValP(val)
{
    if (val.id.indexOf("_Parc") > -1)
        return val.id.substr(0, val.id.indexOf("_Parc"));
    else
        return val.id.substr(0, val.id.indexOf("_ctv"));
}

// -------------------------------------------------------------------
// Função que retorna o número da parcela atual a partir do validador.
// -------------------------------------------------------------------
function getNumParcelaFromValP(val)
{
    return parseInt(val.id.substr(val.id.indexOf("_Parc") + 5, val.id.indexOf("_", val.id.indexOf("_Parc") + 5) - val.id.indexOf("_Parc") + 5));
}

// -----------------------------------------------------
// Função que habilita ou desabilita o sumário de erros.
// -----------------------------------------------------
function habilitarSumarioP(nomeControle, habilitar)
{
    var textoEval = nomeControle + "_vsuValidacao.showmessagebox = \"";
    textoEval += habilitar ? "True" : "False";
    eval(textoEval + "\";");
}

// --------------------------------------
// Indica se o controle pai está visível.
// --------------------------------------
function parentVisivelP(val)
{
    var nomeControle = getNomeControleFromValP(val);
    
    var retorno = !getVar(nomeControle).Habilitado ? false : 
        getVar(nomeControle).ParentID == "" || document.getElementById(getVar(nomeControle).ParentID).style.display != "none";
    
    habilitarSumarioP(nomeControle, retorno);
    return retorno;
}

// -------------------------
// Função que valida a data.
// -------------------------
function validaDataP(val, args)
{
    if (!parentVisivelP(val))
    {
        args.IsValid = true;
        return;
    }

    var nomeControle = getNomeControleFromValP(val);
    var numParcela = getNumParcelaFromValP(val);
    var nomeParcelas = nomeControle + "_Parc";
    var nomeCelula = nomeParcelas + numParcela.toString();
    
    if (numParcela > Parc_getParcelasVisiveis(nomeControle))
    {
        args.IsValid = true;
        return;
    }
    
    var dataValida = true;
    var data = document.getElementById(nomeCelula + "_txtData").value;
    
    if (data == "")
        dataValida = false;
    
    if (dataValida && numParcela > 1)
    {
        var nomeCelulaAnterior = nomeParcelas + (numParcela - 1).toString();
        var dataAnterior = document.getElementById(nomeCelulaAnterior + "_txtData").value;
        
        try
        {
            if (firstEqualOrGreaterThenSec(dataAnterior, data) && dataAnterior != data)
                dataValida = false;
        }
        catch (err)
        {
            dataValida = false;
        }
    }
    
    args.IsValid = dataValida && args.Value != "";
}

// --------------------------
// Função que valida o valor.
// --------------------------
function validaValor(val, args)
{
    if (!parentVisivelP(val))
    {
        args.IsValid = true;
        return;
    }
    
    var nomeControle = getNomeControleFromValP(val);
    var numParcela = getNumParcelaFromValP(val);
    
    if (numParcela <= Parc_getParcelasVisiveis(nomeControle))
        args.IsValid = args.Value != "";
    else
        args.IsValid = true;
}

// --------------------------------
// Função que valida o valor total.
// --------------------------------
function validaValorTotal(val, args)
{
    var nomeControle = getNomeControleFromValP(val);
    
    if (!parentVisivelP(val) || !Parc_getExibirParcelas(nomeControle) || !Parc_exibirValor(nomeControle))
    {
        args.IsValid = true;
        return;
    }

    var nomeParcelas = nomeControle + "_Parc";
    var valorParcelas = 0;
    
    for (n = 0; n < getVar(nomeControle).TotalParcelas; n++)
    {
        var nomeCelula = nomeParcelas + (n + 1).toString();
        var valor = document.getElementById(nomeCelula + "_txtValor");
        if (valor.disabled)
            return;
        
        valor = valor.value;
        valor = valor == "" ? 0 : parseFloat(valor.replace(',', '.'));
        valorParcelas += valor;
    }

    valorParcelas = parseFloat(valorParcelas.toFixed(2));
    /* Chamado 56167. */
    var valorParcelasComparar = Parc_getValorParcelas(nomeControle) + (getVar(nomeControle).AplicarJuros && !getVar(nomeControle).JurosCompostos ? Parc_getValorAcrescimo(nomeControle) - Parc_getValorDesconto(nomeControle) : 0);

    args.IsValid = valorParcelas >= 0 && valorParcelas == valorParcelasComparar.toFixed(2);
        
    if (valorParcelas >= 0)
        eval(val.id).errormessage = "Valor da soma das parcelas (R$ " + valorParcelas.toFixed(2).replace(".", ",") + 
            ") difere do total a pagar (R$ " + valorParcelasComparar.toFixed(2).replace(".", ",") + ").";
    else
        eval(val.id).errormessage = "Valor da soma das parcelas (R$ " + valorParcelas.toFixed(2).replace(".", ",") + 
            ") não pode ser negativo.";
}