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

// ------------------------------------
// Função que recupera o ID do cliente.
// ------------------------------------
function getClienteID(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoClienteID);
        return getValorCampo(campo, "int");
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------------------
// Função que recupera o ID do fornecedor.
// ---------------------------------------
function getFornecedorID(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoFornecedorID);
        return getValorCampo(campo, "int");
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------------------
// Função que recupera os IDs dos pedidos.
// ---------------------------------------
function getPedidosIDs(nomeControle)
{
    try
    {
        var campo = document.getElementById(getVar(nomeControle).CampoPedidosIDs);
        return getValorCampo(campo, "");
    }
    catch (err)
    {
        return "";
    }
}

// -------------------------------------------------------------
// Função que recupera o tipo das parcelas que serão retornadas.
// -------------------------------------------------------------
function getTipoConsultaParcelas(nomeControle)
{
    try
    {
        var campo = document.getElementById(nomeControle + "_hdfTipoConsulta");
        return getValorCampo(campo, "int");
    }
    catch (err)
    {
        return 0;
    }
}

// -----------------------------------------------------------------------------
// Função que gerencia o controle de parcelas dependendo da parcela selecionada.
// -----------------------------------------------------------------------------
function alteraParcela(nomeControle, callback)
{
    var drpParcelas = document.getElementById(nomeControle + "_drpParcelas");
    if (drpParcelas == null)
        return;
    
    var numeroParcelas = document.getElementById(nomeControle + "_numeroParcelas")
    numeroParcelas.style.display = drpParcelas.value == -1 ? "" : "none";

    if (getVar(nomeControle).DesabilitarControleParcelas && getVar(nomeControle).ControleParcelas != "")
    {
        var campoParcelas = eval(getVar(nomeControle).ControleParcelas);
        campoParcelas.Habilitar(drpParcelas.value == -1);
    }

    if (drpParcelas.value > -1)
    {
        for (i = 0; i < getVar(nomeControle).Parcelas.length; i++)
            if (getVar(nomeControle).Parcelas[i].ID == drpParcelas.value)
            {
                alteraDiasParcelas(nomeControle, getVar(nomeControle).Parcelas[i].NumParcelas,
                    getVar(nomeControle).Parcelas[i].Dias, callback);

                break;
            }
    }
    else
    {
        var campoParcelas = eval(getVar(nomeControle).ControleParcelas);
        var numeroParcelas = document.getElementById(nomeControle + "_drpNumParcCustom");
        numeroParcelas.value = campoParcelas.NumeroParcelas();
        
        alteraNumParcCustom(nomeControle, callback);
    }
}

// -----------------------------------------------------------------------------------
// Função que altera o número de parcelas exibidas de acordo com o número customizado.
// -----------------------------------------------------------------------------------
function alteraNumParcCustom(nomeControle, callback)
{
    var numeroParcelas = document.getElementById(nomeControle + "_drpNumParcCustom");
    numeroParcelas = numeroParcelas.value;

    alteraDiasParcelas(nomeControle, numeroParcelas, "", callback);

    var campoParcelas = eval(getVar(nomeControle).ControleParcelas);
    var diasParcelas = campoParcelas != null ? campoParcelas.DiasParcelas(true) : "";
    diasParcelas = diasParcelas.replace(/;/g, ",");
    
    alteraDiasParcelas(nomeControle, numeroParcelas, diasParcelas, callback);
}

// -----------------------------------------------------------------------
// Função que salva nos campos apropriados os dias e o número de parcelas.
// -----------------------------------------------------------------------
function alteraDiasParcelas(nomeControle, numeroParcelas, diasParcelas, callback)
{
    var hdfNumParcelas = document.getElementById(nomeControle + "_hdfNumParcelas");
    var hdfDiasParcelas = document.getElementById(nomeControle + "_hdfDiasParcelas");

    hdfNumParcelas.value = numeroParcelas;
    hdfDiasParcelas.value = diasParcelas;

    if (typeof callback == "string" && !!callback)
    {
        var drpParcelas = document.getElementById(nomeControle + "_drpParcelas");
        eval(callback + "('" + getVar(nomeControle).ControleParcelas + "', drpParcelas)");
    }
}

// ------------------------------------------------
// Função que indica se as parcelas calculam valor.
// ------------------------------------------------
function isParcelasComValor(nomeControle)
{
    var controleParcelas = eval(getVar(nomeControle).ControleParcelas);
    if (controleParcelas == null)
        return true;
    
    var valores = controleParcelas.Valores(false);
    for (iParc = 0; iParc < valores.length; iParc++)
        if (valores[iParc] > 0)
            return true;
    
    return false;
}

// -----------------------------------------------------------------
// Função que carrega as parcelas do cliente/fornecedor selecionado.
// -----------------------------------------------------------------
function atualizaParcCliFornec(nomeControle)
{
    var drpParcelas = document.getElementById(nomeControle + "_drpParcelas");
    if (drpParcelas == null)
        return;

    var resposta = ctrlParcelasSelecionar.GetParcelasCliFornec(getClienteID(nomeControle),
        getFornecedorID(nomeControle), getPedidosIDs(nomeControle), getVar(nomeControle).ExibirParcelasConfiguraveis,
        getVar(nomeControle).SempreExibirDatasParcelas, getTipoConsultaParcelas(nomeControle)).value.split("#");

    if (resposta[0] == "Ok")
    {
        if (resposta[2] == "") resposta[2] = "[]";

        drpParcelas.innerHTML = resposta[1];

        eval(nomeControle + ".Parcelas = " + resposta[2]);
        
        for (var i = 0; i < drpParcelas.options.length; i++)
        {
            if (!getVar(nomeControle).ExibirParcelasConfiguraveis || drpParcelas.options[i].value != -1)
            {
                drpParcelas.options[i].style.display = "none";
                
                for (var j = 0; j < getVar(nomeControle).Parcelas.length; j++)
                    if (drpParcelas.options[i].value == getVar(nomeControle).Parcelas[j].ID)
                    {
                        drpParcelas.options[i].style.display = "";
                        
                        // Verifica se o controle está sendo carregado através do cadastro de pedido.
                        // O método "getPedidosIDs" retorna o id de cada pedido incluído na tela do cadastro de liberação,
                        // ou seja, caso esteja vazio, significa que o controle não está sendo carregado através da tela de liberação.
                        // O método "getFornecedorID(nomeControle)" recupera o id do fornecedor, ou seja, caso esteja vazio,
                        // significa que o controle não está sendo carregado através do financeiro pagamento.
                        if (getPedidosIDs(nomeControle) == "" && getFornecedorID(nomeControle) == "") {
                            // Se o controle estiver sendo carregado através da tela de cadastro do pedido e a parcela corrente
                            // seja a parcela padrão, definida no cadastro do cliente, então ela é selecionada.
                            if (getVar(nomeControle).Parcelas[j].Padrao) {
                                drpParcelas.value = getVar(nomeControle).Parcelas[j].ID;
                                var hdfIdParcela = document.getElementById(nomeControle + "_hdfIdParcela");
                                if (hdfIdParcela != null)
                                    hdfIdParcela.value = drpParcelas.value;
                            }
                        }
                        // Caso o controle esteja sendo carregado através do cadastro de liberação, verifica a parcela padrão do cliente,
                        // verifica a configuração "UsarMenorPrazoLiberarPedido" e verifica se já existe alguma parcela selecionada, pois,
                        // a configuração "UsarMenorPrazoLiberarPedido" deve buscar sempre o menor prazo, e como as parcelas
                        // são ordenadas crescentemente, sempre deve ser marcada a primeira parcela recuperada.
                        else if (getPedidosIDs(nomeControle) != "") {
                            if (getVar(nomeControle).Parcelas[j].Padrao || (getVar(nomeControle).UsarMenorPrazoLiberarPedido &&
                                (drpParcelas.value == "" || drpParcelas.value == "1")))
                                drpParcelas.value = getVar(nomeControle).Parcelas[j].ID;
                        }
                        // Caso o controle esteja sendo carregado através do financeiro pagamento, verifica se a parcela
                        // é a parcela padrão, caso seja, marca-a.
                        else if (getFornecedorID(nomeControle) == "") {
                            if (getVar(nomeControle).Parcelas[j].Padrao)
                                drpParcelas.value = getVar(nomeControle).Parcelas[j].ID;
                        }

                        break;
                    }
            }
        }

        // Remove os controles que estiverem com display none, para que não seja possível selecioná-los com as setas do teclado
        for (var i = 0; i < drpParcelas.options.length; i++)
        {
            if (drpParcelas.options[i].style.display == "none") {
                drpParcelas.remove(i);
                i--;
            }
        }

        getVar(nomeControle).Calcular();
        
        // Criado para a tela de liberação, a posição 3 é a mensagem de erro mostrada caso nenhuma parcela seja recuperada.
        if (resposta[3] != null && resposta[3] != "")
            alert(resposta[3]);
    }
    else if (resposta[0] == "Erro")
    {
        alert(resposta[1]);
    }
}

// ------------------------------------------------------------
// Função que retorna o nome do controle a partir do validador.
// ------------------------------------------------------------
function getNomeControleFromValPS(val)
{
    return val.id.substr(0, val.id.indexOf("_ctv"));
}

// --------------------------------------
// Indica se o controle pai está visível.
// --------------------------------------
function parentVisivelPS(val)
{
    var nomeControle = getNomeControleFromValPS(val);
    
    var retorno = getVar(nomeControle).ParentID == "" || document.getElementById(getVar(nomeControle).ParentID).style.display != "none";
    
    return retorno;
}

// ----------------------------------------
// Valida a seleção de parcela do controle.
// ----------------------------------------
function validaParcelaSel(val, args) {
    if (val.style.visibility != "") {

        if (!parentVisivelPS(val)) {
            args.IsValid = true;
            return;
        }

        args.IsValid = args.Value != "";
    }
}
