// Variáveis de controle
var dadosCliente = {
    ID: 0,
    Nome: '',
    IsConsumidorFinal: false
}

// --------------------------------------
// Função usada para pesquisa de cliente.
// --------------------------------------
function getCliFormaPagto(nomeControle, idCli, nomeCli)
{
    if (idCli.value == "")
        atualizaFormasPagto(nomeControle, false);

    var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
    
    if (retorno[0] == "Erro")
    {
        alert(retorno[1]);
        setCliFormaPagto(nomeControle, "", "", "0");
        
        return false;
    }

    setCliFormaPagto(nomeControle, idCli.value, retorno[1], retorno[3]);
}

// ------------------------------------
// Função usada para alterar o cliente.
// ------------------------------------
function setCliFormaPagto(nomeControle, idCli, nomeCli, credito)
{
    var campoIdCli = document.getElementById(nomeControle + "_txtNumCli");
    var campoNomeCli = document.getElementById(nomeControle + "_txtNomeCliente");
    var campoCredito = document.getElementById(nomeControle + "_hdfFormaPagtoCreditoCliente");
    
    campoIdCli.value = idCli;
    campoNomeCli.value = nomeCli;
    campoCredito.value = credito;

    if (getVar(nomeControle).CallbackSelCliente != '' && eval("window." + getVar(nomeControle).CallbackSelCliente) != undefined)
        eval("window." + getVar(nomeControle).CallbackSelCliente + "(" + idCli + ", " + retorno[1] + ")");

    usarCredito(nomeControle, null, null);
}

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

// ------------------------------------------------
// Função que recupera o valor da conta a ser paga.
// ------------------------------------------------
function getValorConta(nomeControle)
{
    try
    {
        // Recupera o valor do campo do valor da conta
        var campo = document.getElementById(getVar(nomeControle).CampoValorConta);
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// --------------------------------------------------
// Função que recupera o valor do crédito do cliente.
// --------------------------------------------------
function getValorCredito(nomeControle)
{
    var retorno;

    if (!dadosCliente.IsConsumidorFinal)
    {
        try
        {
            // Recupera o valor do campo de valor do crédito
            var campo = document.getElementById(getVar(nomeControle).CampoValorCredito);
            retorno = getValorCampo(campo, "float");
        }
        catch (err)
        {
            retorno = 0;
        }
    }
    else
        retorno = 0;
    
    // Atualiza o campo com o valor utilizado, se o valor no campo for maior que o valor do crédito
    var campoUtilizado = document.getElementById(nomeControle + "_txtCreditoUtilizado");
    var valorUtilizado;
    
    try
    {
        valorUtilizado = getValorCampo(campoUtilizado, "float");
    }
    catch (err)
    {
        valorUtilizado = 0;
    }
    
    var valorAlterado = getValorPagar(nomeControle, false) != getVar(nomeControle).ValorPagarAtual;
    if (valorAlterado)
        getVar(nomeControle).ValorPagarAtual = getValorPagar(nomeControle, false);
    
    var creditoMaximo = retorno;
    
    if (getVar(nomeControle).CampoValorConta != "" && retorno > getValorPagar(nomeControle, false))
        creditoMaximo = getValorPagar(nomeControle, false);
    
    if (valorUtilizado == 0 || valorUtilizado > creditoMaximo || (valorUtilizado != creditoMaximo && valorAlterado))
        campoUtilizado.value = creditoMaximo.toFixed(2).replace(".", ",");

    return retorno;
}

// ----------------------------------------
// Função que recupera o valor do desconto.
// ----------------------------------------
function getValorDesconto(nomeControle)
{
    try
    {
        // Recupera o valor do campo de valor do desconto
        var campo = document.getElementById(getVar(nomeControle).CampoValorDesconto);
        var valor = getValorCampo(campo, "float");
        
        // Calcula o valor do desconto pelo tipo selecionado
        var tipo = document.getElementById(getVar(nomeControle).CampoTipoDesconto);
        if (tipo != null)
        {
            tipo = tipo.tagName.toLowerCase() == "select" ? tipo.options[tipo.selectedIndex].text : tipo.value;
            return tipo == "%" || tipo == "1" ? getValorConta(nomeControle) * valor / 100 : valor;
        }
        else
            return valor;
    }
    catch (err)
    {
        return 0;
    }
}

// -----------------------------------------
// Função que recupera o valor do acréscimo.
// -----------------------------------------
function getValorAcrescimo(nomeControle)
{
    try
    {
        // Recupera o valor do campo de valor do acréscimo
        var campo = document.getElementById(getVar(nomeControle).CampoValorAcrescimo);
        var valor = getValorCampo(campo, "float");
        
        // Calcula o valor do acréscimo pelo tipo selecionado
        var tipo = document.getElementById(getVar(nomeControle).CampoTipoAcrescimo);
        if (tipo != null)
        {
            tipo = tipo.tagName.toLowerCase() == "select" ? tipo.options[tipo.selectedIndex].text : tipo.value;
            return tipo == "%" || tipo == "1" ? getValorConta(nomeControle) * valor / 100 : valor;
        }
        else
            return valor;
    }
    catch (err)
    {
        return 0;
    }
}

// ----------------------------------------
// Função que recupera o valor da comissão.
// ----------------------------------------
function getValorComissao(nomeControle)
{
    try
    {
        // Verifica se o valor da comissão deve ser retornado
        var check = document.getElementById(nomeControle + "_chkComissaoComissionado");
        if (check == null || !check.checked)
            throw new Error();
        
        // Recupera o valor do campo de valor da comissão
        var campo = document.getElementById(nomeControle + "_txtValorComissao");
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// --------------------------------------------------
// Função que calcula o troco que deve ser devolvido.
// --------------------------------------------------
function calcularTroco(nomeControle)
{
    // Recupera o Label lblTroco
    var lblTroco = document.getElementById(nomeControle + "_lblTroco");
    
    // Verifica se o Label foi encontrado
    if (lblTroco == null)
        return;
    
    // Calcula o troco
    var valorTroco = getValorPago(nomeControle) - getValorPagar(nomeControle);
    
    // Altera o Label lblTroco
    lblTroco.innerHTML = "Troco: R$ " + valorTroco.toFixed(2).replace('.', ',');
    lblTroco.parentNode.style.display = valorTroco > 0 ? "" : "none";
}

// -----------------------------------
// Função que calcula o valor a pagar.
// -----------------------------------
function getValorPagar(nomeControle, calcularCredito)
{
    var valorPagar = getValorPagarReal(nomeControle, calcularCredito);

    // Retorna o valor negativo apenas se o controle indicar (caso da liberação de pedido, por exemplo)
    return valorPagar >= 0 || getVar(nomeControle).PermitirValorPagarNegativo ? valorPagar : 0;
}

// -----------------------------------
// Função que calcula o valor a pagar.
// -----------------------------------
function getValorPagarReal(nomeControle, calcularCredito)
{
    // Padroniza o valor de calcularCredito
    calcularCredito = calcularCredito == false ? false : true;
    
    if (calcularCredito)
    {
        // Recupera o CheckBox chkUsarCredito
        var chkUsarCredito = document.getElementById(nomeControle + "_chkUsarCredito");
        if (chkUsarCredito == null || !chkUsarCredito.checked)
            valorCredito = 0;
        else
            valorCredito = getCreditoUsado(nomeControle);
    }
    else
        var valorCredito = 0;

    // Calcula o valor a pagar
    var valorPagar = getValorConta(nomeControle) - getValorDesconto(nomeControle) - valorCredito + getJuros(nomeControle) - getValorComissao(nomeControle) + getValorAcrescimo(nomeControle) - getValorObra(nomeControle);
    
    // Retorna o valor
    return valorPagar;
}

// --------------------------------
// Função que retorna o valor pago.
// --------------------------------
function getValorPago(nomeControle)
{
    // Recupera o valor pago
    var valorPago = 0;
    var valores = getValores(nomeControle, false);
    for (iPago = 0; iPago < valores.length; iPago++)
        valorPago += getValorPagoParcelaCartao(nomeControle, iPago);

    // Retorna o valor pago
    return valorPago;
}

// -----------------------------------------
// Função que retorna o ID do cliente usado.
// -----------------------------------------
function getIdCliente(nomeControle)
{
    try
    {
        var campo = getVar(nomeControle).ExibirCliente ? nomeControle + "_txtNumCli" : getVar(nomeControle).CampoClienteID;
        campo = document.getElementById(campo);
        var retorno = getValorCampo(campo, "int");

        if (typeof dadosCliente == "undefined" || dadosCliente.ID != retorno)
            eval("dadosCliente = " + ctrlFormaPagto.GetDadosCliente(retorno).value);

        return retorno;
    }
    catch (err)
    {
        dadosCliente = {
            ID: 0,
            Nome: '',
            IsConsumidorFinal: false
        }
        
        getVar(nomeControle).ClienteID_Atual = 0;
        return 0;
    }
}

// -----------------------------------------------
// Função que retorna o valor utilizado pela obra.
// -----------------------------------------------
function getValorObra(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoValorObra);
        var retorno = getValorCampo(campo, "float");
        document.getElementById(nomeControle + "_lblValorObra").innerHTML = retorno == 0 ? "" : 
            "Valor utilizado da(s) obra(s): R$ " + retorno.toFixed(2).replace(".", ",");
        return retorno;
    }
    catch (err)
    {
        document.getElementById(nomeControle + "_lblValorObra").innerHTML = "";
        return 0;
    }
}

// ------------------------------------------------------------
// Função que salva o número dos pagamentos que são por cheque.
// ------------------------------------------------------------
function alteraCheques(nomeControle, numPagto, adicionar)
{
    var temp = new Array();
    if (adicionar)
        temp.push(numPagto);

    for (i = 0; i < getVar(nomeControle).PagamentoCheque.length; i++)
    {
        var item = getVar(nomeControle).PagamentoCheque[i];
        if (item == numPagto)
            continue;

        for (j = 0; j < temp.length; j++)
        {
            if (temp[j] == item)
            {
                item = null;
                break;
            }
        }
        
        if (item != null)
            temp.push(item);
    }
    
    getVar(nomeControle).PagamentoCheque = temp;
}

// ---------------------------------------------------------
// Função que altera a visibilidade das células do controle.
// ---------------------------------------------------------
function alteraVisibilidade(nomeTabela, numPagto, formaPagto, atualizarOpcoesSelecionadas, desabilitarOpcoes)
{
    // Recupera o nome do controle
    var nomeControle = nomeTabela.substr(0, nomeTabela.indexOf("_tbl"));
    
    // Recupera o prefixo do controle
    var prefixo = nomeTabela + "_Pagto" + numPagto + "_";
    
    // Recupera as células que serão exibidas ou escondidas
    var cartao_titulo = document.getElementById(prefixo + "Cartao_Titulo");
    var cartao_controles = document.getElementById(prefixo + "Cartao_Controles");
    var boletoTipo_titulo = document.getElementById(prefixo + "BoletoTipo_Titulo");
    var boletoTipo_controles = document.getElementById(prefixo + "BoletoTipo_Controles");
    var cheque = document.getElementById(prefixo + "Cheque");
    var cni = document.getElementById(prefixo + "CartaoNaoIdentificado");
    var conta_titulo = document.getElementById(prefixo + "Conta_Titulo");
    var conta_controles = document.getElementById(prefixo + "Conta_Controles");
    var boletoTaxa_titulo = document.getElementById(prefixo + "BoletoTaxa_Titulo");
    var boletoTaxa_controles = document.getElementById(prefixo + "BoletoTaxa_Controles");
    var numAut_titulo = document.getElementById(prefixo + "NumAut_Titulo");
    var numAut_controles = document.getElementById(prefixo + "NumAut_Controles");
    var boletoNumero_titulo = document.getElementById(prefixo + "BoletoNumero_Titulo");
    var boletoNumero_controles = document.getElementById(prefixo + "BoletoNumero_Controles");
    var depositoNaoIdentificado_titulo = document.getElementById(prefixo + "DepositoNaoIdentificado_Titulo");
    var depositoNaoIdentificado_controles = document.getElementById(prefixo + "DepositoNaoIdentificado_Controles");
    var antecipacaoFornecedor_titulo = document.getElementById(prefixo + "AntecipacaoFornecedor_Titulo");
    var antecipacaoFornecedor_controles = document.getElementById(prefixo + "AntecipacaoFornecedor_Controles");
    var dataRecebimento = document.getElementById(nomeControle + "_txtDataRecebimento");
    var imgDataRecebimento = document.getElementById(nomeControle + "_imgDataRecebido");
    
    // Atualiza as opções selecionadas, se for o caso
    if (atualizarOpcoesSelecionadas)
        atualizarOpcoes(nomeTabela, numPagto);
    
    // Recupera a forma de pagamento
    formaPagto = getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1];
    if (formaPagto == null)
        formaPagto = "";
    
    // Variáveis de controle da exibição
    var exibirCartao = formaPagto == "cartao";
    var exibirCheque = formaPagto == "cheque";
    var exibirCartaoNaoIdentificado = formaPagto == "cartao nao identificado";
    var exibirBoleto = !getVar(nomeControle).IsWebglassLite && formaPagto == "boleto";
    var exibirConta = !getVar(nomeControle).IsWebglassLite && (formaPagto == "deposito" || formaPagto == "cartao" || formaPagto == "boleto" || formaPagto == "construcard");
    var exibirNumAut = formaPagto == "construcard";
    var exibirDepositoNaoIdentificado = !getVar(nomeControle).IsWebglassLite && formaPagto == "deposito nao identificado";
    var exibirAntecipacaoFornecedor = !getVar(nomeControle).IsWebglassLite && formaPagto == "antecipacao de fornecedor";
    
    // Atualiza os controles do cheque
    var txtValor = document.getElementById(prefixo + "txtValor");
    txtValor.readOnly = exibirCheque || exibirDepositoNaoIdentificado || exibirAntecipacaoFornecedor || exibirCartaoNaoIdentificado;

    if (exibirCheque)
    {
        alteraCheques(nomeControle, numPagto, true);
        setValorCampo(txtValor, getTotalChequesCampo(document.getElementById(nomeControle + "_TabelaCheques"), txtValor.id).toFixed(2).replace('.', ','), true);
    }
    else
    {
        alteraCheques(nomeControle, numPagto, false);
        
        if (exibirDepositoNaoIdentificado)
            atualizaTotalDepositoNaoIdentificado(nomeControle, numPagto);

        if (exibirAntecipacaoFornecedor)
            atualizaTotalAntecipacaoFornecedor(nomeControle, numPagto);
    }
        
    // Desabilita os itens nas outras DropDownList de forma de pagamento
    desabilitarOpcoes = desabilitarOpcoes == false ? false : true;
    if (desabilitarOpcoes)
        desabilitaOpcoes(nomeTabela);
    
    // Exibe as células de acordo com a forma de pagamento selecionada
    cartao_titulo.style.display = exibirCartao ? "" : "none";
    cartao_controles.style.display = exibirCartao ? "" : "none";
    boletoTipo_titulo.style.display = exibirBoleto && boletoTipo_titulo.innerHTML != "" ? "" : "none";
    boletoTipo_controles.style.display = exibirBoleto && boletoTipo_titulo.innerHTML != "" ? "" : "none";
    cheque.style.display = exibirCheque ? "" : "none";
    cni.style.display = exibirCartaoNaoIdentificado ? "" : "none";
    conta_titulo.style.display = exibirConta ? "" : "none";
    conta_controles.style.display = exibirConta ? "" : "none";
    boletoTaxa_titulo.style.display = exibirBoleto && boletoTaxa_titulo.innerHTML != "" ? "" : "none";
    boletoTaxa_controles.style.display = exibirBoleto && boletoTaxa_titulo.innerHTML != "" ? "" : "none";
    boletoNumero_titulo.style.display = exibirBoleto && boletoNumero_titulo.innerHTML != "" ? "" : "none";
    boletoNumero_controles.style.display = exibirBoleto && boletoNumero_titulo.innerHTML != "" ? "" : "none";
    numAut_titulo.style.display = exibirNumAut ? "" : "none";
    numAut_controles.style.display = exibirNumAut ? "" : "none";

    if (getVar(nomeControle).IsRecebimento)
    {
        depositoNaoIdentificado_titulo.style.display = exibirDepositoNaoIdentificado ? "" : "none";
        depositoNaoIdentificado_controles.style.display = exibirDepositoNaoIdentificado ? "" : "none";
    }
    else
    {
        antecipacaoFornecedor_titulo.style.display = exibirAntecipacaoFornecedor ? "" : "none";
        antecipacaoFornecedor_controles.style.display = exibirAntecipacaoFornecedor ? "" : "none";
    }

    //Verifica se deve deixa o campo data recebimento habilitado
    var habilitarDataRec = true;

    var formaPagtoSelecionada = false;

    // Percorre todas as formas de pagamento
    for (iPagto = 0; iPagto <= getVar(nomeControle).NumeroPagamentos; iPagto++) {

        // Recupera a forma de pagamento
        var fp = getVar(nomeControle).DescricaoFormasPagamento[iPagto];
        if (fp == null)
            fp = "";

        if (fp != "")
            formaPagtoSelecionada = true;

        if (fp != "boleto" && fp != "deposito" && fp != "")
            habilitarDataRec = false;
    }

    if (!formaPagtoSelecionada)
        habilitarDataRec = false;

    if (dataRecebimento != null) {
        dataRecebimento.readOnly = !habilitarDataRec;

        if(!habilitarDataRec) {
            var date = new Date();
            dataRecebimento.value = (date.getDate() < 10 ? "0" : "") + date.getDate() + "/" + (date.getMonth() + 1 < 10 ? "0" : "") + (date.getMonth() + 1) + "/" + date.getFullYear();
            }
    }
    if (imgDataRecebimento != null)
        imgDataRecebimento.disabled = !habilitarDataRec;

    // Exibe ou esconde a linha de baixo referente ao pagamento, dependendo os controles exibidos
    var linhaBaixo = document.getElementById(prefixo + "LinhaBaixo");
    linhaBaixo.style.display = "";
    if (linhaBaixo.clientHeight <= 8) linhaBaixo.style.display = "none";
    
    var controleContaBanco = document.getElementById(prefixo + "drpContaBanco");
    for (i = 0; i < controleContaBanco.options.length; i++)
        controleContaBanco.options[i].disabled = false;

    var tipoCartao = document.getElementById(prefixo + "drpTipoCartao");

    // Exibe as parcelas do cartão de crédito de acordo com o tipo do cartão e do parâmetro
    if (getVar(nomeControle).ExibirParcelasCartao)
    {
        var isCredito = tipoCartao.options[tipoCartao.selectedIndex].text.toLowerCase().indexOf("crédito") > -1;
        
        var parcCartao = document.getElementById(prefixo + "drpParcCredito");
        parcCartao.style.display = isCredito ? "" : "none";
        
        if (isCredito)
            getNumeroParcelas(nomeControle, prefixo.substr(nomeControle.length + 1), tipoCartao.value);
    }

    if (cartao_titulo.style.display != "none") {
        // Recupera a conta bancária padrão (se houver)
        for (i = 0; i < assoc_conta_banco.length; i++)
            if (assoc_conta_banco[i].TipoCartao == tipoCartao.value && assoc_conta_banco[i].ContaBanco != null) {
                controleContaBanco.value = assoc_conta_banco[i].ContaBanco;

                if (assoc_conta_banco[i].Bloquear) {
                    for (j = 0; j < controleContaBanco.options.length; j++)
                        controleContaBanco.options[j].disabled = controleContaBanco.options[j].value != assoc_conta_banco[i].ContaBanco;
                }

                break;
            }
    }

    if (boletoTipo_titulo.style.display != "none")
    {
        var tipoBoleto = document.getElementById(prefixo + "drpTipoBoleto");
        
        // Recupera a conta bancária padrão (se houver)
        for (i = 0; i < assoc_conta_banco.length; i++)
            if (assoc_conta_banco[i].TipoBoleto == tipoBoleto.value && assoc_conta_banco[i].ContaBanco != null)
            {
                controleContaBanco.value = assoc_conta_banco[i].ContaBanco;
                
                if (assoc_conta_banco[i].Bloquear)
                {
                    for (j = 0; j < controleContaBanco.options.length; j++)
                        controleContaBanco.options[j].disabled = controleContaBanco.options[j].value != assoc_conta_banco[i].ContaBanco;
                }
                
                break;
            }
    }
}

// ----------------------------------
// Abre popup para cadastrar cheques.
// ----------------------------------
function openWindowCheques(nomeControle, nomeTabela, numPagto, selecionar, cadastrarCheque, queryString, pagina, paginaPadrao,
    callbackIncluirCheque, callbackExcluirCheque, formaPagto)
{
    if (!selecionar && pagina == paginaPadrao && queryString == false)
        return false;
    
    // Verifica se o cheque deve ser cadastrado
    cadastrarCheque = !cadastrarCheque && !selecionar ? "&cadastrar=false" : "";
    
    // Formata o restante da QueryString
    if (queryString.length > 0 && queryString[0] != "&")
    {
        if (queryString[0] == "?")
            queryString = queryString.substr(1);
            
        queryString = "&" + queryString;
    }

    // Recupera o nome do controle que será colocado o valor dos cheques
    var controlPagto = nomeTabela + "_Pagto" + numPagto + "_txtValor";
    
    // Recupera o nome do controle que será colocado o id da forma de pagamento dos cheques
    var controlForma = nomeTabela + "_Pagto" + numPagto + "_drpFormaPagto";
    
    // Tabela que conterá os cheques
    var tabelaCheques = nomeControle + "_TabelaCheques";

    // Busca o id do Cliente para preencher o campo titular, usado na tela de liberação
    var ctrCliente = FindControl("hdfIdCliente", "input");
    if (ctrCliente != null)
        queryString += queryString.length > 0 ? "&idCliente=" + ctrCliente.value : "?idCliente=" + ctrCliente.value;

    // Abre a janela
    openWindow(600, 850, pagina + "?nomeControleFormaPagto='" + nomeControle + "'&controlForma=" + formaPagto +  "&controlPagto='" + controlPagto  
        + "'&tabelaCheque='" + tabelaCheques + "'" + cadastrarCheque + "&callbackIncluir='" + callbackIncluirCheque + "'&callbackExcluir='" 
        + callbackExcluirCheque + "'" + queryString);
    
    return false;
}

//-------------------------------------------------------------------
//Abre a popup para inserção de CNI
//-------------------------------------------------------------------
function openWindowCNI(nometabela, nomeTabelaFormaPagto, numPagto)
{
    // Recupera o nome do controle que será colocado o valor dos DNI
    var controleValor = nomeTabelaFormaPagto + "_Pagto" + numPagto + "_txtValor";

    openWindow(600, 850, "../Utils/SelCartaoNaoIdentificado.aspx?tabelaCNI='" + nometabela + "'" + "&nomeControleFormaPagto='" + controleValor + "'");
    return false;
}

// ------------------------------------------------------------------
// Atualiza as opções selecionadas de forma de pagamento do controle.
// ------------------------------------------------------------------
function atualizarOpcoes(nomeTabela, numPagto)
{
    // Recupera o nome do controle
    var nomeControle = nomeTabela.substr(0, nomeTabela.indexOf("_tbl"));
    
    // Verifica se há um índice definido
    if (typeof numPagto == 'undefined')
    {
        // Variáveis de suporte
        var numPagtos = getVar(nomeControle).NumeroPagamentos;
        var descrFormasPagto = "";
        
        // Recupera a lista de opções selecionadas
        for (i = 1; i <= numPagtos; i++)
            descrFormasPagto += "|" + document.getElementById(nomeTabela + "_Pagto" + i + "_drpFormaPagto").value;
        
        // Recupera a lista de formas de pagamento
        descrFormasPagto = numPagtos >= 1 ? ctrlFormaPagto.GetFormasPagto(descrFormasPagto.substr(1)).value.split('|') : "";
        
        // Atualiza a lista na variável do controle
        for (i = 0; i < numPagtos; i++)
            getVar(nomeControle).DescricaoFormasPagamento[i] = descrFormasPagto[i];
    }
    else if (document.getElementById(nomeTabela + "_Pagto" + numPagto + "_drpFormaPagto").value != "")
        getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] = ctrlFormaPagto.GetFormaPagto(document.getElementById(nomeTabela + "_Pagto" + numPagto + "_drpFormaPagto").value).value;
    else
        getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] = "";
}

// ------------------------------------------------------------------------
// Desabilita as opções nas DropDownLists de seleção de forma de pagamento.
// ------------------------------------------------------------------------
function desabilitaOpcoes(nomeTabela)
{
    // Impede que o usuário selecione cheque, dinheiro, construcard ou cartão não identificado em mais de uma DropDownList
    var valores = new Array("cheque", "dinheiro", "construcard", "cartao nao identificado");
    
    // Recupera o nome do controle
    var nomeControle = nomeTabela.substr(0, nomeTabela.indexOf("_tbl"));
    
    // Variáveis de suporte da função
    var numPagtos = getVar(nomeControle).NumeroPagamentos;
    var formasPagto = new Array(numPagtos);
    
    // Cria as variáveis de suporte
    for (i = 1; i <= numPagtos; i++)
    {
        var formaPagto = document.getElementById(nomeTabela + "_Pagto" + i + "_drpFormaPagto");
        formasPagto[i - 1] = formaPagto;
    }
    
    // Sai da função se o controle deve ficar desabilitado
    if (!getVar(nomeControle).PermitirTodasGerarCredito && getGerarCredito(nomeControle))
        return;
    
    // Habilita todas as opções de todas as DropDownLists
    for (i = 0; i < formasPagto.length; i++)
        for (j = 0; j < formasPagto[i].options.length; j++)
            formasPagto[i].options[j].disabled = false;
    
    // Percorre todos os valores
    for (l = 0; l < valores.length; l++)
    {
        // Recupera os valores selecionados de todas as DropDownLists
        var valorItens = new Array(formasPagto.length);
        for (i = 0; i < formasPagto.length; i++)
            valorItens[i] = getVar(nomeControle).DescricaoFormasPagamento[i] == valores[l] ? formasPagto[i].value : "-1";
        
        // Percorre os valores
        for (i = 0; i < valorItens.length; i++)
        {
            // Se o valor não tiver que ser bloqueado, prossegue para o próximo
            if (valorItens[i] == "-1")
                continue;
            
            // Percorre todas as DropDownLists
            for (j = 0; j < formasPagto.length; j++)
            {
                // Se a DropDownList atual for a que contém o valor, prossegue para a próxima
                if (i == j)
                    continue;
                
                // Desabilita a opção da forma de pagamento selecionada
                for (k = 0; k < formasPagto[j].options.length; k++)
                    if (formasPagto[j].options[k].value == valorItens[i])
                    {
                        formasPagto[j].options[k].disabled = true;
                        break;
                    }
            }
        }
    }
}

// --------------------------------------------------
// Calcula o valor a ser pago se o crédito for usado.
// --------------------------------------------------
function usarCredito(nomeControle, callbackTotal, callback)
{
    if (getVar(nomeControle).ParentID != "" && document.getElementById(getVar(nomeControle).ParentID).style.display == "none")
        return;

    // Habilita/desabilita os controles de acordo com o valor da conta
    habilitarControleFP(nomeControle, getValorPagarReal(nomeControle, false) > 0, false);

    // Atualiza as formas de pagamento por cliente
    atualizaFormasPagto(nomeControle, true);

    // Recupera os controles usados na função
    var chkUsarCredito = document.getElementById(nomeControle + "_chkUsarCredito");
    var lblValorASerPago = document.getElementById(nomeControle + "_lblValorASerPago");
    var lblCredito = document.getElementById(nomeControle + "_lblCredito");
    var exibirCredito = document.getElementById(nomeControle + "_hdfExibirCredito");

    // Recupera o valor da conta e o valor do crédito
    getIdCliente(nomeControle);
    var valorCredito = getValorCredito(nomeControle);
    
    // Atualiza o Label do crédito
    lblCredito.innerHTML = "R$ " + valorCredito.toFixed(2).replace('.', ',');
    
    var valorPagar = getValorPagar(nomeControle, false);
    
    if (chkUsarCredito != null)
    {
        var exibirUsarCredito = valorCredito > 0 && (valorPagar > 0 || getVar(nomeControle).CampoValorConta == "") &&
            exibirCredito.value.toLowerCase() != "false";
            
        // Não permite usar o crédito se o controle não exibe a linha com o crédito do cliente
        if (exibirCredito.value.toLowerCase() == "false")
            chkUsarCredito.checked = false;
        
        // Exibe o CheckBox se o valor do crédito for maior que 0 e se o CheckBox existir na tela
        chkUsarCredito.parentNode.style.display = exibirUsarCredito ? "" : "none";

        // Exibe o campo para digitar o valor utilizado se o valor do crédito for maior que 0 e se o CheckBox estiver marcado
        document.getElementById(nomeControle + "_usarCredito").style.display = chkUsarCredito.checked && exibirUsarCredito ? "" : "none";
    }
    
    // Atualiza o Label do valor a pagar
    if (lblValorASerPago != null)
        lblValorASerPago.innerHTML = "R$ " + getValorPagar(nomeControle).toFixed(2).replace('.', ',');
    
    // Invoca a função de callback
    if (typeof callback == "string")
        eval(callback + "(chkUsarCredito, chkUsarCredito.checked)");
    
    // Calcula o troco
    calcularTroco(nomeControle);
    
    // Chama a função de recebimento parcial
    var primeiroCampoValor = document.getElementById(nomeControle + "_tblFormaPagto_Pagto1_txtValor");
    if (primeiroCampoValor != null)
        eval(primeiroCampoValor.getAttribute("onchange"));
    
    // Atualiza o valor pago
    callbackTotal = typeof callbackTotal != 'undefined' && callbackTotal != null ? callbackTotal : "";
    atualizaValorPago(nomeControle, callbackTotal);

    // Marca o checkbox de geração de crédito se o valor da conta for negativo (e se o checkbox existir)
    var chkGerarCredito = document.getElementById(nomeControle + "_chkGerarCredito");
    if (chkGerarCredito != null)
    {
        var isGerarCredito = !dadosCliente.IsConsumidorFinal && getValorPagarReal(nomeControle, false) < 0;
        chkGerarCredito.checked = isGerarCredito || getVar(nomeControle).SituacaoGerarCredito;
        chkGerarCredito.disabled = dadosCliente.IsConsumidorFinal;// || isGerarCredito;
        chkGerarCredito.onclick();
    }
}

// ----------------------------------------------------
// Função que gerencia a exibição do Label lblRestante.
// ----------------------------------------------------
function recebimentoParcial(nomeControle, funcaoDados, callback)
{
    // Recupera o Label que contém o texto
    var lblRestante = document.getElementById(nomeControle + "_lblRestante");
    if (lblRestante == null)
        return;
    
    var receber = true;
    
    // Verifica se o valor deve ser calculado
    if (receber)
    {
        // Variável de suporte
        var dados = {
            Valor: getValorPagar(nomeControle, true) - getValorPago(nomeControle),
            Data: "01/01/0001",
            ExibirData: false
        };
        
        // Chama a função de dados
        if (funcaoDados != "")
            eval(funcaoDados + "(dados)");
        
        // Altera o formato dos dados
        dados.Valor = parseFloat(dados.Valor.toString().replace(',', '.'));
        
        var valorPositivo = dados.Valor >= 0 || document.getElementById(nomeControle + "_chkGerarCredito") == null;
        dados.Valor = Math.abs(dados.Valor).toFixed(2).replace('.', ',');

        // Altera o texto do Label
        lblRestante.innerHTML = (valorPositivo ? "Valor restante: R$ " : "Valor do crédito Gerado: R$ ") + dados.Valor + 
            (valorPositivo && dados.ExibirData ? " a ser pago em " + dados.Data : "");
    }
    
    // Chama o callback
    if (callback != "")
        eval(callback + "()");
    
    // Calcula o troco
    calcularTroco(nomeControle);
}

// ------------------------------------------------
// Função que altera o controle para gerar crédito.
// ------------------------------------------------
function gerarCredito(nomeControle, callback, gerar)
{    
    // Indica se o crédito será gerado
    gerar = typeof gerar != "boolean" ? getGerarCredito(nomeControle) : gerar;
    if (dadosCliente.IsConsumidorFinal) gerar = false;
    getVar(nomeControle).SituacaoGerarCredito = gerar;
    
    // Recupera o CheckBox chkRecebimentoParcial
    var chkRecebimentoParcial = document.getElementById(nomeControle + "_chkRecebimentoParcial");
    if (chkRecebimentoParcial != null)
    {
        // Desmarca e desabilita o CheckBox chkRecebimentoParcial se o crédito for gerado
        if (gerar) chkRecebimentoParcial.checked = false;
        chkRecebimentoParcial.disabled = gerar;
    }
    
    // Verifica se todas as opções podem gerar crédito
    if (!getVar(nomeControle).PermitirTodasGerarCredito)
    {
        // Esconde/exibe todas as linhas com excessão da primeira
        for (g = 2; g <= getVar(nomeControle).NumeroPagamentos; g++)
        {
            var prefixo = nomeControle + "_tblFormaPagto_Pagto" + g + "_";
            document.getElementById(prefixo + "LinhaCima").style.display = gerar ? "none" : "";
            document.getElementById(prefixo + "LinhaBaixo").style.display = gerar ? "none" : "";
            if (gerar)
            {
                document.getElementById(prefixo + "txtValor").value = "";
                eval(document.getElementById(prefixo + "txtValor").getAttribute("onchange"));
                document.getElementById(prefixo + "drpFormaPagto").selectedIndex = 0;
            }
            alteraVisibilidade(nomeControle + "_tblFormaPagto", g, "", false, g == getVar(nomeControle).NumeroPagamentos);
        }
        
        // Seleciona a opção 'Cheque' e desabilita a primeira forma de pagamento
        var formaPagto1 = document.getElementById(nomeControle + "_tblFormaPagto_Pagto1_drpFormaPagto");
        if (gerar) formaPagto1.value = 2;
        formaPagto1.disabled = gerar || document.getElementById(nomeControle + "_tblFormaPagto_Pagto1_txtValor").disabled;

        // Altera a visibilidade dos controles da primeira forma de pagamento
        alteraVisibilidade(nomeControle + "_tblFormaPagto", 1, "2", true, true);
    }   
}

// -----------------
// Limpa o controle.
// -----------------
function limparControle(nomeControle, chamarUsarCredito, callbackUsarCredito)
{
    // Recupera o número de pagamentos e o nome da tabela
    var numPagtos = getVar(nomeControle).NumeroPagamentos;
    var nomeTabela = nomeControle + "_tblFormaPagto";
    
    // Percorre toda a tabela
    for (limp = 0; limp < numPagtos; limp++)
    {
        // Volta os campos para os valores padrão
        var prefixo = nomeTabela + "_Pagto" + (limp + 1) + "_";
        document.getElementById(prefixo + "txtValor").value = "";
        document.getElementById(prefixo + "drpFormaPagto").selectedIndex = 0;
        document.getElementById(prefixo + "drpContaBanco").selectedIndex = 0;
        document.getElementById(prefixo + "txtNumAutConstrucard").value = "";
        if (document.getElementById(prefixo + "drpTipoCartao") != null) document.getElementById(prefixo + "drpTipoCartao").selectedIndex = 0;
        if (document.getElementById(prefixo + "drpParcCredito") != null) document.getElementById(prefixo + "drpParcCredito").selectedIndex = 0;
        if (document.getElementById(prefixo + "drpTipoBoleto") != null) document.getElementById(prefixo + "drpTipoBoleto").selectedIndex = 0;
        if (document.getElementById(prefixo + "txtTaxaAntecipacao") != null) document.getElementById(prefixo + "txtTaxaAntecipacao").value = "";
        if (document.getElementById(prefixo + "txtNumeroBoleto") != null) document.getElementById(prefixo + "txtNumeroBoleto").value = "";
        
        // Altera a visibilidade dos controles da forma de pagamento
        alteraVisibilidade(nomeTabela, limp + 1, "", true, limp == (numPagtos - 1));
    }

    // Limpa os demais controles
    var txtNumCli = document.getElementById(nomeControle + "_txtNumCli");
    var txtNomeCliente = document.getElementById(nomeControle + "_txtNomeCliente");
    var txtJuros = document.getElementById(nomeControle + "_txtJuros");
    
    if (txtJuros != null)
        txtJuros.value = "";

    if (txtNumCli != null)
        txtNumCli.value = "";

    if (txtNomeCliente != null)
        txtNomeCliente.value = "";

    // Padroniza o valor do parâmetro
    chamarUsarCredito = chamarUsarCredito != false;

    // Atualiza os valores da conta e crédito
    if (chamarUsarCredito)
        usarCredito(nomeControle, callbackUsarCredito);
    
    // Limpa os IDs de controle da comissão
    limparIdsComissao(nomeControle);
    
    // Limpa a tabela de cheques
    limparTabelaCheques(document.getElementById(nomeControle + "_TabelaCheques"));

    // Lima a tabela de CNI
    limparTabelaCNI(document.getElementById(nomeControle + "_TabelaCNI"));
}

// -----------------------------------------------------------
// Função para habilitar/desabilitar os campos de um controle.
// -----------------------------------------------------------
function habilitarControleFP(nomeControle, habilitar, chamarUsarCredito)
{
    // Limpa o controle se for desabilitar os campos
    if (!habilitar)
    {
        if (!getVar(nomeControle).BloquearCamposContaVazia)
            return;
        
        limparControle(nomeControle, chamarUsarCredito, null);
    }

    // Recupera o número de pagamentos e o nome da tabela
    var numPagtos = getVar(nomeControle).NumeroPagamentos;
    var nomeTabela = nomeControle + "_tblFormaPagto";

    // Percorre toda a tabela
    for (hab = 0; hab < numPagtos; hab++)
    {
        // Habilita os campos
        var prefixo = nomeTabela + "_Pagto" + (hab + 1) + "_";
        document.getElementById(prefixo + "txtValor").disabled = !habilitar;
        document.getElementById(prefixo + "drpFormaPagto").disabled = !habilitar;
        document.getElementById(prefixo + "drpContaBanco").disabled = !habilitar;
        document.getElementById(prefixo + "txtNumAutConstrucard").disabled = !habilitar;
        if (document.getElementById(prefixo + "drpTipoCartao") != null) document.getElementById(prefixo + "drpTipoCartao").disabled = !habilitar;
        if (document.getElementById(prefixo + "drpParcCredito") != null) document.getElementById(prefixo + "drpParcCredito").disabled = !habilitar;
        if (document.getElementById(prefixo + "drpTipoBoleto") != null) document.getElementById(prefixo + "drpTipoBoleto").disabled = !habilitar;
        if (document.getElementById(prefixo + "txtTaxaAntecipacao") != null) document.getElementById(prefixo + "txtTaxaAntecipacao").disabled = !habilitar;
        if (document.getElementById(prefixo + "txtNumeroBoleto") != null) document.getElementById(prefixo + "txtNumeroBoleto").disabled = !habilitar;
    }

    // Habilita os demais controles
    var txtNumCli = document.getElementById(nomeControle + "_txtNumCli");
    var txtNomeCliente = document.getElementById(nomeControle + "_txtNomeCliente");
    var txtJuros = document.getElementById(nomeControle + "_txtJuros");

    if (txtJuros != null)
        txtJuros.disabled = !habilitar;

    if (txtNumCli != null)
        txtNumCli.disabled = !habilitar;

    if (txtNomeCliente != null)
        txtNomeCliente.disabled = !habilitar;
}

// ------------------------------------------------------------------
// Função usada ao fazer Submit no form.
//   Coloca uma string com os dados do cheque no controle de suporte.
//   Usada para recuperar os cheques no ASP.net.
// ------------------------------------------------------------------
function atualizarChequesECNI(nomeControle)
{
    var hdfCheques = document.getElementById(nomeControle + "_hdfCheques");
    hdfCheques.value = getVar(nomeControle).Cheques();

    var hdfCNI = document.getElementById(nomeControle + "_hdfCNI");
    hdfCNI.value = getVar(nomeControle).CartoesNaoIdentificados();
}

// ---------------------------------------------------------
// Função que retorna o valor do crédito usado pelo cliente.
// ---------------------------------------------------------
function getCreditoUsado(nomeControle)
{
    try
    {
        if (!getUsarCredito(nomeControle))
            return 0;
        
        // Retorna o valor do crédito utilizado
        var campo = document.getElementById(nomeControle + "_txtCreditoUtilizado");
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// --------------------------------------------------------------
// Função usada para recuperar os valores dos campos do controle.
// --------------------------------------------------------------
function getValoresFromControle(nomeControle, nomeCampo, parseTo, defaultValue, asString)
{
    // Cria o vetor de retorno
    var retorno = new Array(getVar(nomeControle).NumeroPagamentos);
    
    // Percorre todas as linhas da tabela
    for (i = 0; i < retorno.length; i++)
    {
        try
        {
            // Recupera o valor do campo
            var prefixo = nomeControle + "_tblFormaPagto_Pagto" + (i + 1) + "_";
            var campo = document.getElementById(prefixo + nomeCampo);
            retorno[i] = getValorCampo(campo, parseTo);
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
// Função que retorna as taxas de antecipação.
// -------------------------------------------
function getTaxasAntecipacao(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "txtTaxaAntecipacao", "float", 0, asString);
}

// --------------------------------------
// Função que retorna os tipos de boleto.
// --------------------------------------
function getTiposBoleto(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "drpTipoBoleto", "int", 0, asString);
}

// --------------------------------------
// Função que retorna os tipos de boleto.
// --------------------------------------
function getNumerosBoleto(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "txtNumeroBoleto", "string", 0, asString);
}

// ------------------------------------------
// Função que retorna as formas de pagamento.
// ------------------------------------------
function getFormasPagto(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "drpFormaPagto", "int", 0, asString);
}

// ---------------------------------------------
// Função que retorna os valores dos pagamentos.
// ---------------------------------------------
function getValores(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "txtValor", "float", 0, asString);
}

// --------------------------------------
// Função que retorna os tipos de cartão.
// --------------------------------------
function getTiposCartao(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "drpTipoCartao", "int", 0, asString);
}

// ----------------------------------------------
// Função que retorna as parcelas de cada cartão.
// ----------------------------------------------
function getParcelasCartao(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "drpParcCredito", "int", 0, asString);
}

// ----------------------------------------------------------------------
// Função que retorna as datas selecionadas para cada forma de pagamento.
// ----------------------------------------------------------------------
function getDatasFormaPagto(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "txtData", "", "", asString);
}

// -------------------------------------------------------------------------------
// Função que retorna os depósitos não identificados para cada forma de pagamento.
// -------------------------------------------------------------------------------
function getDepositosNaoIdentificados(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "selDni_hdfValor", "int", 0, asString);
}

// --------------------------------------------------------------------------------
// Função que retorna as antecipações de fornecedores para cada forma de pagamento.
// --------------------------------------------------------------------------------
function getAntecipacoesFornecedores(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "selAntecipFornec_hdfValor", "int", 0, asString);
}

// -----------------------------------------------------------
// Função que retorna o valor pago para uma posição do cartão.
// -----------------------------------------------------------
function getValorPagoParcelaCartao(nomeControle, posicaoPagto)
{
    // Valor pago para a posição desejada
    var valor = getValores(nomeControle, false)[posicaoPagto];
    
    // Verifica se os juros do cartão serão cobrados do cliente
    if (getVar(nomeControle).ExibirParcelasCartao && getVar(nomeControle).CobrarJurosCartaoCliente &&
        getVar(nomeControle).DescricaoFormasPagamento[posicaoPagto] == "cartao")
    {
        var jurosCartao = getJurosCartao(nomeControle, posicaoPagto);
        valor = valor - jurosCartao;
    }

    return valor;
}

// --------------------------------------
// Função que retorna os juros do cartão.
// --------------------------------------
function getJurosCartao(nomeControle, posicaoPagto)
{
    // Verifica se os juros do cartão serão cobrados do cliente
    if (getVar(nomeControle).ExibirParcelasCartao && getVar(nomeControle).CobrarJurosCartaoCliente && 
        getVar(nomeControle).DescricaoFormasPagamento[posicaoPagto] == "cartao")
    {
        // Recupera os dados do cartão verificado
        var valor = getValores(nomeControle, false);
        var tipoCartao = getTiposCartao(nomeControle, false)[posicaoPagto];
        var numeroParcelas = getParcelasCartao(nomeControle, false)[posicaoPagto];

        // Variáveis de suporte
        var valorParcela = 0;
        var jurosCartao = 0;
        var valorEsperado = getValorPagar(nomeControle, true);
        
        // Desconta os valores já pagos do valor esperado
        for (iJuros = 0; iJuros < valor.length; iJuros++)
            valorEsperado -= iJuros != posicaoPagto ? valor[iJuros] : 0;
        
        // Recupera apenas o valor da posição considerada
        valor = valor[posicaoPagto];
        
        // Calcula os juros do cartão utilizado
        for (iJuros = 0; iJuros < juros_parc_cartao.length; iJuros++)
            if (juros_parc_cartao[iJuros].TipoCartao == tipoCartao && juros_parc_cartao[iJuros].Parcelas == numeroParcelas)
            {
                var taxaJuros = 1 + (juros_parc_cartao[iJuros].Juros / 100);
                valorParcela = parseFloat((valor * (1 / taxaJuros)).toFixed(2));
                jurosCartao = parseFloat((valor - valorParcela).toFixed(2));
                
                valorEsperado *= taxaJuros;
                break;
            }
        
        // Atualiza os labels
        var lblValorCartao = document.getElementById(nomeControle + "_tblFormaPagto_Pagto" + (posicaoPagto + 1) + "_lblDadosPagtoCartao_lblValorCartao");
        var lblJurosCartao = document.getElementById(nomeControle + "_tblFormaPagto_Pagto" + (posicaoPagto + 1) + "_lblDadosPagtoCartao_lblJurosCartao");
        var lblValorEsperadoCartao = document.getElementById(nomeControle + "_tblFormaPagto_Pagto" + (posicaoPagto + 1) + "_lblDadosPagtoCartao_lblValorEsperadoCartao");

        if (lblValorCartao != null)
            lblValorCartao.innerHTML = "Valor pago: R$ " + valorParcela.toFixed(2).replace(".", ",");

        if (lblJurosCartao != null)
            lblJurosCartao.innerHTML = "Juros: R$ " + jurosCartao.toFixed(2).replace(".", ",");
            
        if (lblValorEsperadoCartao != null)
            lblValorEsperadoCartao.innerHTML = "Valor esperado: R$ " + valorEsperado.toFixed(2).replace(".", ",");

        return jurosCartao;
    }
    
    // Retorna se não calcular os juros
    return 0;
}

// ----------------------------
// Função que retorna os juros.
// ----------------------------
function getJuros(nomeControle)
{
    try
    {
        // Recupera o valor do campo txtJuros
        var campo = document.getElementById(nomeControle + "_txtJuros");
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------
// Função que altera os juros.
// ---------------------------
function setJuros(nomeControle, valor)
{
    try
    {
        var campo = document.getElementById(nomeControle + "_txtJuros");
        return setValorCampo(campo, valor, true);
    }
    catch (err)
    {
    }
}

// ------------------------------------
// Função que retorna os juros mínimos.
// ------------------------------------
function getJurosMin(nomeControle)
{
    try
    {
        // Recupera o valor do campo hdfJurosMin
        var campo = document.getElementById(nomeControle + "_hdfJurosMin");
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// -----------------------------------
// Função que altera os juros mínimos.
// -----------------------------------
function setJurosMin(nomeControle, valor)
{
    try
    {
        var campo = document.getElementById(nomeControle + "_hdfJurosMin");
        setValorCampo(campo, valor, false);
        alteraJuros(nomeControle, false);
    }
    catch (err)
    {
    }
}

// -----------------------------------------
// Função que retorna a data de recebimento.
// -----------------------------------------
function getDataRecebimento(nomeControle)
{
    try
    {
        // Recupera o valor do campo txtDataRecebimento
        var campo = document.getElementById(nomeControle + "_txtDataRecebimento");
        return getValorCampo(campo, "");
    }
    catch (err)
    {
        return "";
    }
}

// ----------------------------------------------------
// Função que retorna as contas bancárias do pagamento.
// ----------------------------------------------------
function getContasBanco(nomeControle, asString)
{
    return getValoresFromControle(nomeControle, "drpContaBanco", "int", 0, asString);
}

// --------------------------------------------------------
// Função que retorna o número do Construcard do pagamento.
// --------------------------------------------------------
function getNumConstrucard(nomeControle)
{
    // Verifica se há algum controle com o número do Construcard
    var numeros = getValoresFromControle(nomeControle, "txtNumAutConstrucard", "");
    for (i = 0; i < numeros.length; i++)
        if (numeros[i] != "" && getVar(nomeControle).DescricaoFormasPagamento[i] == "construcard")
            return numeros[i];
            
    return "";
}

// --------------------------------------------------------
// Função que retorna o número de autorização do cartão do pagamento.
// --------------------------------------------------------
function getNumAutCartao(nomeControle, asString) {
    return getValoresFromControle(nomeControle, "txtNumAutCartao", "", 0, asString);
}

// -------------------------------------------------------------------
// Função que indica se o crédito será usado para efetuar o pagamento.
// -------------------------------------------------------------------
function getUsarCredito(nomeControle)
{
    try
    {
        // Recupera o valor do campo chkUsarCredito
        var campo = document.getElementById(nomeControle + "_chkUsarCredito");
        return getValorCampo(campo, "bool");
    }
    catch (err)
    {
        return true;
    }
}

// -----------------------------------------------------
// Função que indica se será gerado o crédito excedente.
// -----------------------------------------------------
function getGerarCredito(nomeControle)
{
    try
    {
        // Recupera o valor do campo chkGerarCredito
        var campo = document.getElementById(nomeControle + "_chkGerarCredito");
        return getValorCampo(campo, "bool");
    }
    catch (err)
    {
        return false;
    }
}

// ------------------------------------------------
// Função que indica se o recebimento será parcial.
// ------------------------------------------------
function getRecebimentoParcial(nomeControle)
{
    try
    {
        // Recupera o valor do campo chkRecebimentoParcial
        var campo = document.getElementById(nomeControle + "_chkRecebimentoParcial");
        return getValorCampo(campo, "bool");
    }
    catch (err)
    {
        return false;
    }
}

// ----------------------------------------------------------------
// Função que indica se a comissão do comissionado será descontada.
// ----------------------------------------------------------------
function getDescontarComissao(nomeControle)
{
    try
    {
        // Recupera o valor do campo chkComissaoComissionado
        var campo = document.getElementById(nomeControle + "_chkComissaoComissionado");
        return getValorCampo(campo, "bool");
    }
    catch (err)
    {
        return false;
    }
}

// -------------------------------------------------------------
// Adiciona uma lista de IDs à lista de verificação da comissão.
// -------------------------------------------------------------
function adicionarIdsComissao(nomeControle, ids)
{
    ids = ids.split(',');
    for (i = 0; i < ids.length; i++)
        adicionarIdComissao(nomeControle, ids[i], true);
    
    getVar(nomeControle).AtualizarComissao = true;
    atualizarValorComissao(nomeControle);
}

// --------------------------------------------------
// Adiciona um ID à lista de verificação da comissão.
// --------------------------------------------------
function adicionarIdComissao(nomeControle, id, varios)
{
    for (i = 0; i < getVar(nomeControle).IDs.length; i++)
        if (getVar(nomeControle).IDs[i] == id)
            return;
    
    getVar(nomeControle).IDs.push(id);
    
    if (!varios)
    {
        getVar(nomeControle).AtualizarComissao = true;
        atualizarValorComissao(nomeControle);
    }
}

// -------------------------------------------------
// Remove um ID da lista de verificação de comissão.
// -------------------------------------------------
function removerIdComissao(nomeControle, id)
{
    var temp = new Array();
    var alterou = false;
    
    for (i = 0; i < getVar(nomeControle).IDs.length; i++)
        if (getVar(nomeControle).IDs[i] != id)
            temp.push(getVar(nomeControle).IDs[i]);
        else
            alterou = true;
        
    if (!alterou)
        return;
    
    getVar(nomeControle).IDs = temp;
    getVar(nomeControle).AtualizarComissao = true;
    atualizarValorComissao(nomeControle);
}

// --------------------------------------------------------
// Remove todos os IDs da lista de verificação de comissão.
// --------------------------------------------------------
function limparIdsComissao(nomeControle)
{
    if (getVar(nomeControle).IDs.length > 0)
    {
        getVar(nomeControle).IDs = new Array();
        getVar(nomeControle).AtualizarComissao = true;
        atualizarValorComissao(nomeControle);
    }
}

// -------------------------------------------------------
// Atualiza o valor da comissão através dos IDs indicados.
// -------------------------------------------------------
function atualizarValorComissao(nomeControle)
{
    // Só atualiza o valor da comissão se a lista mudar
    if (!getVar(nomeControle).AtualizarComissao)
        return;
    
    // Recupera o campo com o valor da comissão
    var txtValorComissao = document.getElementById(nomeControle + "_txtValorComissao");
    if (txtValorComissao == null)
        return;
    
    // Recupera o valor da comissão para os IDs selecionados
    if (getVar(nomeControle).IDs.length > 0)
        var valorComissao = ctrlFormaPagto.GetValorComissao(getVar(nomeControle).IDs.toString(), getVar(nomeControle).TipoModel).value;
    else
        var valorComissao = 0;

    valorComissao = parseFloat(valorComissao.toString().replace(',', '.'));
    
    // Atualiza o campo com o valor da comissão
    txtValorComissao.value = valorComissao.toFixed(2).replace('.', ',');
    
    // Variável que indica se o CheckBox deve ser exibido
    var exibirCheck = valorComissao > 0 && valorComissao <= getValorConta(nomeControle);
    
    // Recupera o CheckBox e altera suas propriedades
    var chkComissaoComissionado = document.getElementById(nomeControle + "_chkComissaoComissionado");
    chkComissaoComissionado.parentNode.parentNode.style.display = exibirCheck ? "" : "none";
    chkComissaoComissionado.checked = exibirCheck ? chkComissaoComissionado.checked : false;
}

// ---------------------------------------------------------
// Função que atualiza o valor que está sendo pago no campo.
// ---------------------------------------------------------
function atualizaValorPago(nomeControle, callback)
{
    var valorAtual = document.getElementById(nomeControle + "_txtValorPago").value;
    var novoValor = getValorPago(nomeControle) + getCreditoUsado(nomeControle);
    
    if (valorAtual != novoValor)
    {
        document.getElementById(nomeControle + "_txtValorPago").value = novoValor;
        eval(document.getElementById(nomeControle + "_txtValorPago").getAttribute("onchange"));
        eval(document.getElementById(nomeControle + "_txtValorPago").getAttribute("onblur"));
        
        if (callback != "")
            eval(callback + "()");

        usarCredito(nomeControle, null, null);
    }
}

// ---------------------------------
// Verifica se os juros são válidos.
// ---------------------------------
function alteraJuros(nomeControle, exibirMensagem, callbackTotal, callbackUsarCredito)
{
    // Verifica se o valor dos juros é menor que o mínimo
    if (getJurosMin(nomeControle) > 0 && parseFloat(getJurosMin(nomeControle).toFixed(2)) > parseFloat(getJuros(nomeControle).toFixed(2)))
    {
        // Exibe a mensagem de erro
        if (exibirMensagem)
            alert("Valor mínimo de juros: R$ " + getJurosMin(nomeControle).toFixed(2).replace(".", ","));

        // Altera o campo de juros
        setJuros(nomeControle, getJurosMin(nomeControle).toFixed(2).replace('.', ','));
    }
    
    // Atualiza o valor a pagar
    usarCredito(nomeControle, callbackTotal, callbackUsarCredito);
}

// ----------------------------------------------------------------
// Função que atualiza o número de parcelas para um tipo de cartão.
// ----------------------------------------------------------------
function getNumeroParcelas(nomeControle, prefixo, tipoCartao)
{
    // Recupera o número de parcelas do cartão
    var numeroParcelas = parseInt(ctrlFormaPagto.GetNumeroParcelas(tipoCartao).value, 10);

    // Recupera o controle do número de parcelas
    var drpNumParc = document.getElementById(nomeControle + "_" + prefixo + "drpParcCredito");
    if (drpNumParc == null)
        return;

    // Salva o valor atual
    var opcaoAtual = parseInt(drpNumParc.value, 10);
    
    // Altera o número de parcelas no controle
    for (i = 0; i < drpNumParc.options.length; i++)
        drpNumParc.options[i].style.display = i < numeroParcelas ? "" : "none";

    // Tenta restaurar a opção atual
    drpNumParc.value = opcaoAtual <= numeroParcelas ? opcaoAtual : 1;
}

// --------------------------------------------
// Atualiza as formas de pagamento do controle.
// --------------------------------------------
function atualizaFormasPagto(nomeControle, usarCliente)
{
    var idCliente = "";
    
    if (usarCliente) {
        // Recupera o cliente
        idCliente = getIdCliente(nomeControle);
        
        // Verifica se o cliente foi alterado
        if (getVar(nomeControle).ClienteID_Atual == idCliente)
            return;
            
        // Salva o cliente atual
        getVar(nomeControle).ClienteID_Atual = idCliente;
    }
    
    // Recupera as formas de pagamento
    var resposta = ctrlFormaPagto.GetFormasPagtoCliente(idCliente, getVar(nomeControle).FormasPagamentoDisponiveis).value.split("|");

    // Percorre todas as formas de pagamento
    for (iPagto = 1; iPagto <= getVar(nomeControle).NumeroPagamentos; iPagto++)
    {
        // Recupera o campo que receberá as formas de pagamento
        var campo = document.getElementById(nomeControle + "_tblFormaPagto_Pagto" + iPagto + "_drpFormaPagto");
        
        // Salva a forma de pagamento atual
        var fpAtual = campo.value;
        
        // Limpa as opções
        while (campo.options.length > 1)
            campo.remove(1);

        // Cria as opções
        for (iFp = 0; iFp < resposta.length; iFp++)
        {
            // Verifica se há opção para exibir
            if (resposta[iFp].length == 0)
                continue;
                
            var dados = resposta[iFp].split(";");
            var opcao = document.createElement("option");
            opcao.value = dados[0];
            opcao.text = dados[1];
            
            try
            {
                campo.add(opcao, campo.options[null]);
            }
            catch (err)
            {
                campo.add(opcao, null);
            }
        }

        // Tenta colocar a forma de pagamento salva
        campo.value = fpAtual;
    }

    // Atualiza as permissões de pagamento
    desabilitaOpcoes(nomeControle + "_tblFormaPagto");
}

// ---------------------------------------------------------
// Função que passa a exibir apenas os controles de crédito.
// ---------------------------------------------------------
function exibirApenasCredito(nomeControle, exibir)
{
    var cabecalho = document.getElementById(nomeControle + "_tblCabecalho");
    var formaPagto = document.getElementById(nomeControle + "_tblFormaPagto");
    var troco = document.getElementById(nomeControle + "_Troco");
    var restante = document.getElementById(nomeControle + "_Restante");
    
    cabecalho.rows[0].style.display = exibir || document.getElementById(nomeControle + "_hdfExibirCliente").value == "False" ? "none" : ""; 
    cabecalho.rows[1].style.display = document.getElementById(nomeControle + "_hdfExibirCredito").value == "False" ? "none" : ""; 
    for (i = 2; i <= 4; i++)
        cabecalho.rows[i].style.display = exibir ? "none" : "";
    
    atualizarValorComissao(nomeControle);
    
    formaPagto.style.display = exibir ? "none" : "";
    troco.style.display = exibir ? "none" : "";
    restante.style.display = exibir ? "none" : "";
    
    getVar(nomeControle).ExibindoApenasCredito = exibir;
    
    if (exibir)
        limparControle(nomeControle, true, null);
    else
    {
        try { document.getElementById(nomeControle + "_tblFormaPagto_Pagto1_txtValor").onchange(); }
        catch (err) { }
    }
}

// -----------------------------------------------------------
// Função de callback de seleção de depósito não identificado.
// -----------------------------------------------------------
function selecaoDni(controle, id)
{
    var nomeControle = controle.substr(0, controle.indexOf("_tbl"));
    var numPagto = controle.substr(controle.indexOf("_Pagto") + 6).split("_")[0];
    
    atualizaTotalDepositoNaoIdentificado(nomeControle, numPagto);
}

// ------------------------------------------------------------------------------------------
// Função que atualiza o valor do depósito não identificado no campo do valor correspondente.
// ------------------------------------------------------------------------------------------
function atualizaTotalDepositoNaoIdentificado(nomeControle, numPagto)
{
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";

    var txtValor = document.getElementById(prefixo + "txtValor");
    var celula = document.getElementById(prefixo + "DepositoNaoIdentificado_Controles");

    var valor = document.getElementById(prefixo + "selDni_txtDescr");
    
    if (!!valor && valor.value != "")
    {
        valor = valor.value;

        if (valor.indexOf(", Valor: R$ ") >= 0)
            valor = valor.substr(valor.indexOf(", Valor: R$ ") + 12).split(",");
        else
            valor = valor.substr(valor.indexOf(", Valor: R$") + 11).split(",");

        valor = valor[0].replace(".","") + "," + valor[1];
    }
    else
        valor = "0,00";

    setValorCampo(txtValor, valor, true);
}

// -----------------------------------------------------------
// Função de callback de seleção de antecipação de fornecedor.
// -----------------------------------------------------------
function selecaoAntecipFornec(controle, id)
{
    var nomeControle = controle.substr(0, controle.indexOf("_tbl"));
    var numPagto = controle.substr(controle.indexOf("_Pagto") + 6).split("_")[0];

    atualizaTotalAntecipacaoFornecedor(nomeControle, numPagto);
}

// ------------------------------------------------------------------------------------------
// Função que atualiza o valor da antecipação de fornecedor no campo do valor correspondente.
// ------------------------------------------------------------------------------------------
function atualizaTotalAntecipacaoFornecedor(nomeControle, numPagto)
{
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";

    var txtValor = document.getElementById(prefixo + "txtValor");
    var celula = document.getElementById(prefixo + "AntecipacaoFornecedor_Controles");

    var valor = document.getElementById(prefixo + "selAntecipFornec_txtDescr");

    if (!!valor && valor.value != "")
    {
        valor = valor.value;
        valor = valor.substr(valor.indexOf(", Saldo: R$ ") + 12).split(",");
        valor = valor[0].replace(".", "") + "," + valor[1];
    }
    else
        valor = "0,00";

    setValorCampo(txtValor, valor, true);
}


// ------------------------------------------------------------
// Função que retorna o nome do controle a partir do validador.
// ------------------------------------------------------------
function getNomeControleFromValFP(val)
{
    // Recupera o nome do controle removendo o final do nome do validador
    if (val.id.indexOf("_tbl") > -1)
        return val.id.substr(0, val.id.indexOf("_tbl"));
    else
        return val.id.substr(0, val.id.indexOf("_ctv"));
}

// -------------------------------------------------------------------
// Função que retorna o número da parcela atual a partir do validador.
// -------------------------------------------------------------------
function getNumPagtoFromValFP(val)
{
    var numPagto = val.id.indexOf("_Pagto") + 6;
    numPagto = val.id.substr(numPagto, val.id.indexOf("_", numPagto) - numPagto)
    return parseInt(numPagto);
}

// -----------------------------------------------------
// Função que habilita ou desabilita o sumário de erros.
// -----------------------------------------------------
function habilitarSumarioFP(nomeControle, habilitar)
{
    var textoEval = nomeControle + "_vsuSumario.showmessagebox = \"";
    textoEval += habilitar ? "True" : "False";
    eval(textoEval + "\";");
}

// --------------------------------------
// Indica se o controle pai está visível.
// --------------------------------------
function parentVisivelFP(val)
{
    var nomeControle = getNomeControleFromValFP(val);
    
    // Verifica se o controle é apenas para usar crédito ou se o Parent do controle está visível
    var retorno = !getVar(nomeControle).ExibindoApenasCredito && (getVar(nomeControle).ParentID == "" || 
        document.getElementById(getVar(nomeControle).ParentID).style.display != "none");
    
    habilitarSumarioFP(nomeControle, retorno);
    return retorno;
}

// -------------------------------------------------
// Função que valida o valor e a forma de pagamento.
// -------------------------------------------------
function validaValorFormaPagto(val, args)
{
    if (!parentVisivelFP(val))
    {
        args.IsValid = true;
        return;
    }

    var nomeControle = getNomeControleFromValFP(val);
    var numPagto = getNumPagtoFromValFP(val);
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";
    
    var valor = document.getElementById(prefixo + "txtValor");
    var formaPagto = document.getElementById(prefixo + "drpFormaPagto");
    
    args.IsValid = (valor.value != "" && formaPagto.value != "") || (valor.value == "" && formaPagto.value == "");
    args.IsValid = !args.IsValid ? args.Value != "" : true;
}

// -----------------------------------
// Função que valida o tipo de cartão.
// -----------------------------------
function validaCartao(val, args)
{
    if (!parentVisivelFP(val))
    {
        args.IsValid = true;
        return;
    }

    var nomeControle = getNomeControleFromValFP(val);
    var numPagto = getNumPagtoFromValFP(val);
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";
    
    if (getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "cartao")
    {
        args.IsValid = true;
        return;
    }
    
    args.IsValid = args.Value != "";
}

// -----------------------------
// Função que valida os cheques.
// -----------------------------
function validaCheque(val, args)
{
    if (!parentVisivelFP(val))
    {
        args.IsValid = true;
        return;
    }

    var nomeControle = getNomeControleFromValFP(val);
    var numPagto = getNumPagtoFromValFP(val);
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";
    
    if (getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "cheque")
    {
        args.IsValid = true;
        return;
    }
    
    var chequeValido = false;
    var tabelaCheques = document.getElementById(nomeControle + "_TabelaCheques");
    for (i = 1; i < tabelaCheques.rows.length; i++)
        if (!tabelaCheques.rows[i].getAttribute("Excluida"))
        {
            chequeValido = true;
            break;
        }
    
    args.IsValid = chequeValido;
}

// -------------------------------------------------------------------------
// Função que valida o prazo dos cheques.
// Não permite gerar crédito para cheques com prazo maior que o configurado.
// -------------------------------------------------------------------------
function validaDiasCheque(val, args)
{
    var nomeControle = getNomeControleFromValFP(val);
    
    if (!parentVisivelFP(val) || getVar(nomeControle).NumeroDiasImpedirGerarCreditoCheque == 0 || !getGerarCredito(nomeControle))
    {
        args.IsValid = true;
        return;
    }
    
    var numPagto = getNumPagtoFromValFP(val);
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";
    
    if (getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "cheque")
    {
        args.IsValid = true;
        return;
    }
    
    var temCheques = false;
    var tabelaCheques = document.getElementById(nomeControle + "_TabelaCheques");
    for (i = 1; i < tabelaCheques.rows.length; i++)
        if (!tabelaCheques.rows[i].getAttribute("Excluida"))
        {
            temCheques = true;
            break;
        }
    
    args.IsValid = temCheques ? getMaiorPrazoCheques(tabelaCheques.id) <= 
        getVar(nomeControle).NumeroDiasImpedirGerarCreditoCheque : true;
}

// -----------------------------------
// Função que valida a conta bancária.
// -----------------------------------
function validaConta(val, args)
{
    if (!parentVisivelFP(val))
    {
        args.IsValid = true;
        return;
    }

    var nomeControle = getNomeControleFromValFP(val);
    var numPagto = getNumPagtoFromValFP(val);
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";
    
    if ((getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "deposito" && getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "cartao" &&
        getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "boleto") || getVar(nomeControle).IsWebglassLite)
    {
        args.IsValid = true;
        return;
    }
    
    args.IsValid = args.Value != "";
}

// ----------------------------------------------------
// Função que valida o número de autorização do cartão.
// ----------------------------------------------------
function validaNumAutCartao(val, args) {
    if (!parentVisivelFP(val)) {
        args.IsValid = true;
        return;
    }
    
    var nomeControle = getNomeControleFromValFP(val);
    var numPagto = getNumPagtoFromValFP(val);
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";
    
    if (getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "cartao") {
        args.IsValid = true;
        return;
    }
    
    args.IsValid = args.Value != "";
}

// ------------------------------------------
// Função que valida o número do Construcard.
// ------------------------------------------
function validaNumAut(val, args)
{
    if (!parentVisivelFP(val))
    {
        args.IsValid = true;
        return;
    }

    var nomeControle = getNomeControleFromValFP(val);
    var numPagto = getNumPagtoFromValFP(val);
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";
    
    if (getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "construcard")
    {
        args.IsValid = true;
        return;
    }
    
    args.IsValid = args.Value != "";
}

// -----------------------------------
// Função que valida o tipo de boleto.
// -----------------------------------
function validaBoleto(val, args)
{
    if (!parentVisivelFP(val))
    {
        args.IsValid = true;
        return;
    }

    var nomeControle = getNomeControleFromValFP(val);
    var numPagto = getNumPagtoFromValFP(val);
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";

    if (getVar(nomeControle).IsWebglassLite || getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "boleto")
    {
        args.IsValid = true;
        return;
    }
    
    args.IsValid = args.Value != "";
}

// ---------------------------------------------------
// Função que valida a seleção de formas de pagamento.
// ---------------------------------------------------
function validaFormasPagto(val, args)
{
    var nomeControle = getNomeControleFromValFP(val);
    
    if (!parentVisivelFP(val) || (!getVar(nomeControle).ExibirValorAPagar && getValorPagar(nomeControle) != 0))
    {
        args.IsValid = true;
        return;
    }

    // A comparação abaixo foi alterada, de "getValorPagar(nomeControle, true) == 0" passou a ser "getValorPagar(nomeControle, true) <= 0"
    // para que pudesse confirmar uma liberação com valor negativo sem dar a mensagem "selecione pelo menos uma forma de pagamento"
    if (getValorPagar(nomeControle, true) <= 0 || getUsarCredito(nomeControle) || getValorObra(nomeControle) > 0)
    {
        args.IsValid = true;
        return;
    }
    
    for (i = 0; i < getVar(nomeControle).NumeroPagamentos; i++)
        if (typeof getVar(nomeControle).DescricaoFormasPagamento != 'undefined' && getVar(nomeControle).DescricaoFormasPagamento[i] != null && getVar(nomeControle).DescricaoFormasPagamento[i] != "")
        {
            args.IsValid = true;
            return;
        }
    
    args.IsValid = false;
}

// -----------------------------------------------
// Função que valida o cliente para gerar crédito.
// -----------------------------------------------
function validaCliente(val, args)
{
    var nomeControle = getNomeControleFromValFP(val);
    
    if (!parentVisivelFP(val) || (!getVar(nomeControle).ExibirValorAPagar && getValorPagar(nomeControle) != 0) || !getVar(nomeControle).ExibirCliente)
    {
        args.IsValid = true;
        return;
    }
    
    if ((getValorCredito(nomeControle) > 0 && getUsarCredito(nomeControle)) || getGerarCredito(nomeControle))
        args.IsValid = args.Value != "";
    else
        args.IsValid = true;
}

// ----------------------------------------------
// Função que valida o depósito não identificado.
// ----------------------------------------------
function validaDepositoNaoIdentificado(val, args)
{
    if (!parentVisivelFP(val))
    {
        args.IsValid = true;
        return;
    }

    var nomeControle = getNomeControleFromValFP(val);
    var numPagto = getNumPagtoFromValFP(val);
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";

    if (getVar(nomeControle).IsWebglassLite || getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "deposito nao identificado")
    {
        args.IsValid = true;
        return;
    }

    val.errormessage = numPagto + "ª forma de pagamento: Selecione o depósito não identificado.";
    args.IsValid = args.Value != "";

    if (args.IsValid)
    {
        var numFormasPgto = getVar(nomeControle).NumeroPagamentos;
        var prefixoVerificar = prefixo.substr(0, prefixo.length - numPagto.toString().length - 1);

        for (var i = 0; i < numFormasPgto; i++)
            if (getVar(nomeControle).DescricaoFormasPagamento[i] == "deposito nao identificado" && (i + 1) != numPagto)
            {
                var valor = document.getElementById(prefixoVerificar + (i + 1) + "_selDni_txtDescr");

                if (valor && valor.value != "" && args.Value.split(" ")[0] == valor.value.split(" ")[0])
                {   
                    val.errormessage = numPagto + "ª forma de pagamento: Depósito não identificado já selecionado.";
                    args.IsValid = false;
                    break;
                }
            }
    }
}

// ----------------------------------------------
// Função que valida a antecipação de fornecedor.
// ----------------------------------------------
function validaAntecipacaoFornecedor(val, args)
{
    if (!parentVisivelFP(val))
    {
        args.IsValid = true;
        return;
    }

    var nomeControle = getNomeControleFromValFP(val);
    var numPagto = getNumPagtoFromValFP(val);
    var prefixo = nomeControle + "_tblFormaPagto_Pagto" + numPagto + "_";

    if (getVar(nomeControle).IsWebglassLite || getVar(nomeControle).DescricaoFormasPagamento[numPagto - 1] != "antecipacao de fornecedor")
    {
        args.IsValid = true;
        return;
    }

    val.errormessage = numPagto + "ª forma de pagamento: Selecione a antecipação de pagamento.";
    args.IsValid = args.Value != "";

    if (args.IsValid)
    {
        var numFormasPgto = getVar(nomeControle).NumeroPagamentos;
        var prefixoVerificar = prefixo.substr(0, prefixo.length - numPagto.toString().length - 1);

        for (var i = 0; i < numFormasPgto; i++)
            if (getVar(nomeControle).DescricaoFormasPagamento[i] == "antecipacao de fornecedor" && (i + 1) != numPagto)
        {
            var valor = document.getElementById(prefixoVerificar + (i + 1) + "_selAntecipFornec_txtDescr");

            if (valor && valor.value != "" && args.Value.split(" ")[0] == valor.value.split(" ")[0])
            {
                val.errormessage = numPagto + "ª forma de pagamento: Antecipação de pagamento já selecionada.";
                args.IsValid = false;
                break;
            }
        }
    }
}