// Variáveis que contém dados para os beneficiamentos
var redondo = false;
var alterouProduto = false;
var alterouPecaItemProj = false;
var dadosProduto = {
    ID: 0,
    Grupo: 0,
    Subgrupo: null,
    Custo: 0,
    TipoCalculo: 0,
    CodInterno: '',
    DescontoAcrescimo: 0,
    UsarDescontoAcrescimo: false,
    Cor: null,
    IsSubgrupoEstoque: false,
    IsChapaVidro : false
};
var dadosPecaItemProjeto = {
    ID: 0
};
var chamarCallbackSelecaoItem = true;
var executarBenefAssoc = true;
var camposNaoCobrar = new Array();
var adicionandoBenef = false;
var removendoBenef = false;
var calcularTotal = true;
var carregandoPadrao = false;
var iniciando = true;
var calculandoTotal = false;

// --------------------------------------------------------------------------------------------------------
// Função de suporte para indicar se o controle de beneficiamento deve ser exibido de acordo com o produto.
// --------------------------------------------------------------------------------------------------------
function exibirControleBenef(idControle)
{
    if (getVar(idControle).BeneficiamentosApenasVidros)
    {
        getProdutoID(idControle);
        return dadosProduto.Grupo == 1 && (getVar(idControle).CalcularBeneficiamentoPadrao || 
            dadosProduto.TipoCalculo == 2 || dadosProduto.TipoCalculo == 10);
    }
    else
        return true;
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
            if (campo.type.toLowerCase() != "checkbox") {
                var valor = campo.value != null ? campo.value.replace("(", "").replace(")", "") : campo.value;

                // O parênteses não pode ser removido pois produtos que possuam "(" ou ")" no código interno dariam problema ao carregar
                if (valor.indexOf("(") == 0 || parseTo != "")
                    valor = valor.replace("(", "").replace(")", "")
            }
            else
                var valor = campo.checked;
            break;
            
        case "select":
            var valor = campo.value;
            break;
            
        case "span":
            var valor = campo.innerHTML != null ? campo.innerHTML : campo.innerHTML;

            // O parênteses não pode ser removido pois produtos que possuam "(" ou ")" no código interno dariam problema ao carregar
            if (valor.indexOf("(") == 0 || parseTo != "")
                valor = valor.replace("(", "").replace(")", "")
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

// -------------------------------------------------------------------------------
// Função que atualiza as variáveis que controlam os cálculos dos beneficiamentos.
// -------------------------------------------------------------------------------
function atualizaVariaveis(nomeControle)
{
    getProdutoID(nomeControle);
    getPecaItemProjetoID(nomeControle);
    
    if (alterouProduto || alterouPecaItemProj)
    {
        alterouProduto = false;
        alterouPecaItemProj = false;
        loadBenefPadrao(nomeControle);
    }
}

// ----------------------------------------------------------------------------------------
// Função que altera o processo e aplicação do vidro, se o beneficiamento tiver seu padrão.
// ----------------------------------------------------------------------------------------
function atualizaProcessoAplicacao(nomeControle, beneficiamento)
{
    var campoAplicacaoID = document.getElementById(getVar(nomeControle).AplicacaoID);
    var campoProcessoID = document.getElementById(getVar(nomeControle).ProcessoID);
    var campoAplicacao = document.getElementById(getVar(nomeControle).Aplicacao);
    var campoProcesso = document.getElementById(getVar(nomeControle).Processo);

    if (beneficiamento.AplicacaoID != null && campoAplicacaoID != null)
        campoAplicacaoID.value = beneficiamento.AplicacaoID;

    if (beneficiamento.Aplicacao != null && beneficiamento.Aplicacao != "" && campoAplicacao != null)
        campoAplicacao.value = beneficiamento.Aplicacao;

    if (beneficiamento.ProcessoID != null && campoProcessoID != null)
        campoProcessoID.value = beneficiamento.ProcessoID;

    if (beneficiamento.Processo != null && beneficiamento.Processo != "" && campoProcesso != null)
        campoProcesso.value = beneficiamento.Processo;
}

// -----------------------------------
// Função que retorna o ID do cliente.
// -----------------------------------
function getClienteID(idControle)
{
    try
    {
        var campo = document.getElementById(getVar(idControle).ClienteID);
        return getValorCampo(campo, "");
    }
    catch (err)
    {
        return "";
    }
}

// -----------------------------------
// Função que retorna o ID do produto.
// -----------------------------------
function getProdutoID(idControle)
{
    try
    {
        // Recupera o campo do ID do produto
        var campo = document.getElementById(getVar(idControle).ProdutoID);
        
        // Recupera o ID
        var retorno = getValorCampo(campo, "");

        // Se o ID do produto foi alterado atualiza os dados dele
        if (retorno.toString().toLowerCase() != dadosProduto.CodInterno.toLowerCase())
        {
            var dados = ctrlBenef.GetDadosProduto(getClienteID(idControle), retorno).value;
            if (dados == null)
                throw new Error();
            
            eval("dadosProduto = " + dados);
            alterouProduto = true;

            if (dadosProduto.ID > 0)
                campo.value = dadosProduto.CodInterno;
        }
        
        // Retorna o ID do produto
        return retorno;
    }
    catch (err)
    {
        dadosProduto = {
            ID: 0,
            Grupo: 0,
            Subgrupo: null,
            Custo: 0,
            TipoCalculo: 0,
            CodInterno: '',
            DescontoAcrescimo: 0,
            UsarDescontoAcrescimo: false,
            IsSubgrupoEstoque: false,
            IsChapaVidro: false
        };
        return 0;
    }
}

// ---------------------------------------------
// Função que retorna o ID da peça item projeto.
// ---------------------------------------------
function getPecaItemProjetoID(idControle)
{
    try
    {
        // Recupera o campo do ID do produto
        var campo = document.getElementById(getVar(idControle).PecaItemProjetoID);
        
        // Recupera o ID
        var retorno = getValorCampo(campo, "");

        // Se o ID da peça foi alterado recupera seus dados
        if (retorno != dadosPecaItemProjeto.ID)
        {
            var dados = ctrlBenef.GetDadosPecaItemProjeto(retorno).value;
            if (dados == null)
                throw new Error();
            
            eval("dadosPecaItemProjeto = " + dados);
            alterouPecaItemProj = true;
        }
        
        // Retorna o ID do produto
        return retorno;
    }
    catch (err)
    {
        dadosPecaItemProjeto = {
            ID: 0
        };
        return 0;
    }
}

// ----------------------------------------
// Função que retorna a espessura do vidro.
// ----------------------------------------
function getEspessura(idControle)
{
    try
    {
        // Recupera o campo da espessura e retorna o valor
        var campo = document.getElementById(getVar(idControle).Espessura);
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// -------------------------------------
// Função que retorna o tipo de entrega.
// -------------------------------------
function getTipoEntrega(idControle)
{
    try
    {
        // Recupera o campo do tipo da venda e retorna o valor
        var campo = document.getElementById(getVar(idControle).TipoEntrega);
        return getValorCampo(campo, "int");
    }
    catch (err)
    {
        return 1;
    }
}

// ------------------------------------------------------------
// Função que retorna o percentual de comissão do comissionado.
// ------------------------------------------------------------
function getPercComissao(idControle)
{
    try
    {
        // Recupera o campo de percentual de comissão e retorna o valor
        var campo = document.getElementById(getVar(idControle).PercComissao);
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// -------------------------------------
// Função que retorna a altura do vidro.
// -------------------------------------
function getAltura(idControle)
{
    try
    {
        // Recupera o campo de altura e retorna o valor
        var campo = document.getElementById(getVar(idControle).Altura);
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// --------------------------------------
// Função que modifica a altura do vidro.
// --------------------------------------
function setAltura(idControle, altura, executarFuncao)
{
    try
    {
        // Recupera o campo de altura e altera o valor
        var campo = document.getElementById(getVar(idControle).Altura);
        setValorCampo(campo, altura, executarFuncao);
    }
    catch (err)
    { }
}

// --------------------------------------
// Função que retorna a largura do vidro.
// --------------------------------------
function getLargura(idControle)
{
    try
    {
        // Recupera o campo de largura e retorna o valor
        var campo = document.getElementById(getVar(idControle).Largura);
        return getValorCampo(campo, "int");
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------------------
// Função que modifica a largura do vidro.
// ---------------------------------------
function setLargura(idControle, largura, executarFuncao)
{
    try
    {
        // Recupera o campo de largura e altera o valor
        var campo = document.getElementById(getVar(idControle).Largura);
        setValorCampo(campo, largura, executarFuncao);
    }
    catch (err)
    { }
}

// -----------------------------------
// Função que retorna a área do vidro.
// -----------------------------------
function getTotalM2(idControle)
{
    try
    {
        /* Chamado 64466. */
        if (dadosProduto.TipoCalculo != 2 && dadosProduto.TipoCalculo != 10)
            return 1;

        // Recupera o campo de área e retorna o valor
        var campo = document.getElementById(getVar(idControle).TotalM2);
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------------------------
// Função que retorna o valor unitário do vidro.
// ---------------------------------------------
function getValorUnitario(idControle)
{
    try
    {
        // Recupera o campo de valor unitário e retorna o valor
        var campo = document.getElementById(getVar(idControle).ValorUnitario);
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// ------------------------------------
// Função que retorna o custo do vidro.
// ------------------------------------
function getCusto(idControle)
{
    try
    {
        // Recupera o campo de valor unitário e retorna o valor
        var campo = document.getElementById(getVar(idControle).Custo);
        return getValorCampo(campo, "float");
    }
    catch (err)
    {
        return 0;
    }
}

// ------------------------------------------
// Função que retorna a quantidade de vidros.
// ------------------------------------------
function getQuantidade(idControle)
{
    try
    {
        // Recupera o campo de quantidade e retorna o valor
        var campo = document.getElementById(getVar(idControle).Quantidade);
        var qtdProd = getValorCampo(campo, "float");

        // Multiplica a qtd do produto pela qtd de ambiente
        var campoQtdAmb = document.getElementById(getVar(idControle).QuantidadeAmbiente);
        if (campoQtdAmb != null && campoQtdAmb != undefined) {
            var qtdAmbiente = getValorCampo(campoQtdAmb, "float");

            if (qtdAmbiente > 0)
                qtdProd *= qtdAmbiente;
        }

        return qtdProd;
    }
    catch (err)
    {
        return 0;
    }
}

// ---------------------------------------------
// Função que retorna se o cliente é revendedor.
// ---------------------------------------------
function getRevenda(idControle)
{
    try
    {
        // Recupra o campo de revenda e retorna o valor
        var campo = document.getElementById(getVar(idControle).Revenda);
        return getValorCampo(campo, "bool");
    }
    catch (err)
    {
        return false;
    }
}

// -------------------------------------------------------
// Função que retorna o valor adicional do beneficiamento.
// -------------------------------------------------------
function getValorAdicional(funcaoValorAdicional, beneficiamento, controle, beneficiamentosControle, prefixoControle)
{
    // Recupera o valor adicional do beneficiamento
    var valorAdicional = 0;
    if (typeof funcaoValorAdicional != "undefined" && funcaoValorAdicional != null && funcaoValorAdicional != "")
        eval("valorAdicional = " + funcaoValorAdicional + "(beneficiamento, controle, beneficiamentosControle, prefixoControle)");
    
    return valorAdicional;
}

// ---------------------------------------------------------------
// Função que recupera o ID do beneficiamento através do controle.
// ---------------------------------------------------------------
function getBenefConfigIdFromControl(controle)
{
    // Garante que haja um controle
    if (controle == null)
        return null;
    
    // Recupera o identificador do beneficiamento do controle
    var id = controle.getAttribute("idBeneficiamento");
    
    // Se o identificador não for encontrado, tenta com o controle pai
    if (id != null)
        return id;
    else
        return getBenefConfigIdFromControl(controle.parentNode);
}

// -------------------------------------------------------------------------
// Função que retorna os dados do beneficiamento aplicado através do seu ID.
// -------------------------------------------------------------------------
function getBenefConfigFromId(id)
{
    // Cria a variável de retorno
    var retorno = new Array();
    
    // Percorre todos os beneficiamentos
    for (i = 0; i < benefConfig.length; i++)
    {
        // Verifica se o beneficiamento foi encontrado e se ele é usado nos cálculos
        if (benefConfig[i].ID == id && benefConfig[i].Calcular)
        {
            // Verifica se o subgrupo do produto é levado em consideração
            if (benefConfig[i].Subgrupo == dadosProduto.Subgrupo && benefConfig[i].Cor == dadosProduto.Cor)
            {
                while (retorno.length > 0)
                    retorno.pop();
                
                retorno.push(benefConfig[i]);
                break;
            }
            else
                retorno.push(benefConfig[i]);
        }
    }
    
    // Retorna os beneficiamentos, se encontrados
    return retorno;
}

// --------------------------------------------------------------------------------------------
// Função que retorna os dados dos possíveis beneficiamentos aplicados através do seu ParentID.
// --------------------------------------------------------------------------------------------
function getBenefConfigFromParentId(id)
{
    // Variável de controle dos identificadores
    var ids = new Array(id.toString());
    
    // Variável de retorno
    var retorno = new Array();
    
    // Executa enquanto houver algum identificador a ser verificado
    while (ids.length > 0)
    {
        // Recupera um identificador da lista
        var idAtual = ids.pop();
        
        // Percorre todos os beneficiamentos
        for (i = 0; i < benefConfig.length; i++)
        {
            // Verifica se o identificador do pai é um identificador desejado
            if (benefConfig[i].ParentID == idAtual)
            {
                // Se ele não for calculado, adiciona o seu identificador à lista de verificação
                if (benefConfig[i].Calcular)
                    retorno.push(benefConfig[i]);
                else
                    ids.push(benefConfig[i].ID);
            }
        }
    }
    
    // Retorna a lista
    return retorno;
}

// ------------------------------------------------------------------------
// Função usada para recuperar os beneficiamentos do subgrupo dos produtos.
// ------------------------------------------------------------------------
function getBenefConfigFromSubgrupoId(beneficiamentos)
{
    // Variável de retorno
    var retorno = new Array();
    for (i = 0; i < beneficiamentos.length; i++)
        if (beneficiamentos[i].SubgrupoID == dadosProduto.Subgrupo)
            retorno.push(beneficiamentos[i]);
    
    // Percorre os beneficiamentos para adicionar os beneficiamentos sem identificador de subgrupo que ainda não foram adicionados
    for (i = 0; i < beneficiamentos.length; i++)
    {
        // Só insere os beneficiamentos sem subgrupo
        if (beneficiamentos[i].SubgrupoID != null)
            continue;
        
        // Variável de controle do retorno
        var inserir = true;
        
        if (retorno.length > 0)
        {
            // Verifica se o identificador do beneficiamento é um 'sibling' ou se o subgrupo é diferente do subgrupo de retorno para esse beneficiamento
            for (j = 0; j < retorno.length; j++)
                if (beneficiamentos[i].ID == retorno[j].ID && beneficiamentos[i].Espessura == retorno[j].Espessura && beneficiamentos[i].Cor == retorno[j].Cor)
                {
                    inserir = false;
                    break;
                }
        }
        
        // Insere na lista de retorno
        if (inserir)    
            retorno.push(beneficiamentos[i]);
    }
    
    // Retorna a lista
    return retorno;
}

// ------------------------------------------------------
// Função usada para recuperar o beneficiamento pela cor.
// ------------------------------------------------------
function getBenefConfigByCor(cor, beneficiamentos)
{
    // Garante que há beneficiamentos para verificar
    if (beneficiamentos.length == 0)
        return beneficiamentos;
    
    // Variável de retorno
    var retorno = new Array();
    
    // Percorre os beneficiamentos
    for (i = 0; i < beneficiamentos.length; i++)
    {
        // Verifica se a cor é a cor escolhida
        if (beneficiamentos[i].Cor != cor)
            continue;
        
        // Adiciona o beneficiamento com a cor escolhida ao retorno
        retorno.push(beneficiamentos[i]);
    }
    
    // Adiciona os beneficiamentos com a cor padrão (para beneficiamentos que não foram encontrados para a cor)
    for (i = 0; i < beneficiamentos.length; i++)
    {
        // Verifica se a cor é a padrão
        if (beneficiamentos[i].Cor != null)
            continue;
        
        // Variável de controle
        var inserir = true;
        
        // Verifica se o beneficiamento já foi incluído ao retorno
        for (j = 0; j < retorno.length; j++)
            if (beneficiamentos[i].ID == retorno[j].ID && beneficiamentos[i].Espessura == retorno[j].Espessura)
            {
                inserir = false;
                break;
            }
        
        // Verifica se o beneficiamento será inserido
        if (!inserir)
            continue;
        
        // Inclui o beneficiamento ao retorno
        retorno.push(beneficiamentos[i]);
    }
    
    // Retorna a lista
    return retorno;
}

// ------------------------------------------------------------
// Função usada para recuperar o beneficiamento pela espessura.
// ------------------------------------------------------------
function getBenefConfigByEspessura(espessura, beneficiamentos)
{
    // Garante que há beneficiamentos para verificar
    if (beneficiamentos.length == 0 || beneficiamentos[0].TipoEspessura == 0)
        return beneficiamentos;
    
    // Ordena a lista de beneficiamentos pela espessura
    for (i = 0; i < beneficiamentos.length - 1; i++)
        for (j = i + 1; j < beneficiamentos.length; j++)
            if (beneficiamentos[j].Espessura < beneficiamentos[i].Espessura)
            {
                var temp = beneficiamentos[i];
                beneficiamentos[i] = beneficiamentos[j];
                beneficiamentos[j] = temp;
            }
    
    // Variável de retorno
    var retorno = new Array();
    
    // Percorre os beneficiamentos
    for (i = 0; i < beneficiamentos.length; i++)
    {
        // Verifica se a espessura é válida. Se não for válida, mas for o último beneficiamento considera esse
        if (beneficiamentos[i].Espessura < espessura && i < (beneficiamentos.length - 1))
            continue;
        
        // Variável de controle
        var inserir = true;
        
        // Verifica se o beneficiamento já foi cadastrado
        for (j = 0; j < retorno.length; j++)
            if (beneficiamentos[i].ID == retorno[j].ID)
            {
                inserir = false;
                break;
            }
        
        // Verifica se o beneficiamento será inserido ao retorno
        if (!inserir)
            continue;
        
        // Adiciona o beneficiamento à lista de retorno
        retorno.push(beneficiamentos[i]);
    }
    
    // Retorna a lista
    return retorno;
}

// -----------------------------------------------
// Função que filtra os beneficiamentos pelo tipo.
// -----------------------------------------------
function getBenefConfigByTipo(prefixo, beneficiamentos)
{
    // Garante que há beneficiamentos para verificar
    if (beneficiamentos.length == 0)
        return beneficiamentos;
    
    // Verifica se é necessário filtrar os beneficiamentos pelo tipo
    var tipo = document.getElementById(prefixo + "drpTipo");
    if (tipo == null)
        return beneficiamentos;
    
    // Recupera o tipo selecionado
    var tipo = parseInt(tipo.value, 10);
    
    // Variável de retorno
    var retorno = new Array();
    
    // Adiciona à lista de retorno apenas os itens do tipo selecionado
    for (i = 0; i < beneficiamentos.length; i++)
        if (beneficiamentos[i].ID == tipo || beneficiamentos[i].ParentID == tipo)
            retorno.push(beneficiamentos[i]);
    
    // Retorna a lista
    return retorno;
}

// ---------------------------------------------------------
// Função usada para retornar os beneficiamentos associados.
// ---------------------------------------------------------
function getBenefConfigAssocById(id)
{
    // Variável de retorno
    var retorno = new Array();
    
    // Percorre a lista de beneficiamentos associados
    for (i = 0; i < benefConfigAssoc.length; i++)
        if (benefConfigAssoc[i].ID == id)
            retorno.push(benefConfigAssoc[i]);
    
    // Retorna a lista
    return retorno;
}

// -------------------------------------------
// Função usada para retornar os itens vazios.
// -------------------------------------------
function getItemVazio(nomeControle, controle)
{
    // Recupera a lista de itens vazios
    var itensVazios = getVar(nomeControle).ItensVazios;
    
    // Retorna o item correspondente ao item selecionado como um item de um vetor
    for (i = 0; i < itensVazios.length; i++)
        if (controle.value == itensVazios[i].ID)
            return new Array(itensVazios[i]);
    
    // Se não achar, retorna null
    return null;
}

// -----------------------------------------
// Função usada pelo controle de quantidade.
// -----------------------------------------
function numUpDown(idCampoValor, tipo)
{
    // Recupera o campo do valor
    var controle = document.getElementById(idCampoValor);
    
    // Recupera o valor atual do campo
    var valor = controle.value != "" ? parseInt(controle.value, 10) : 0;
    
    // Altera o valor do campo pelo tipo
    if (tipo == "up")
        controle.value = valor + 1;
    else if (valor > 0)
        controle.value = valor - 1;
}

// --------------------------------------------------------
// Função que garante que apenas 1 controle esteja marcado.
// --------------------------------------------------------
function selecaoUnica(prefixo, controleMarcado)
{
    // Variável de controle
    var i = 1;
    
    // Percorre todos os controles de seleção do beneficiamento
    while (document.getElementById(prefixo + "chkSelecao" + i) != null)
    {
        // Desmarca o controle do beneficiamento, se não for o controle marcado
        if (controleMarcado.id != prefixo + "chkSelecao" + i)
            document.getElementById(prefixo + "chkSelecao" + i).checked = false;
        
        // Incrementa a variável de controle
        i++;
    }
}

// ------------------------------------------------------------------------
// Função responsável pelo disparo das funções de cálculo para cada célula.
// ------------------------------------------------------------------------
function efetuaCalculoBenef(cabecalho, controles)
{
    // Variável com o nome do controle
    var nomeControle = "";
    
    // Recupera todos os inputs da célula
    var inputs = controles.getElementsByTagName("input");

    var NUMERO_INPUTS_MINIMO = 8;

    // Verifica se há inputs além dos (NUMERO_INPUTS_MINIMO) HiddenField de controle do beneficiamento
    if (inputs.length > NUMERO_INPUTS_MINIMO)
    {
        // Verifica se o controle é válido para ser usado no cálculo
        // O controle é válido se for um checkbox marcado ou outro tipo de input
        for (numInput = 0; numInput < (inputs.length - NUMERO_INPUTS_MINIMO); numInput++)
        {
            if (inputs[numInput].type.toLowerCase() != "checkbox" && inputs[numInput].type.toLowerCase() != "hidden")
                break;
            else if (inputs[numInput].checked)
                break;
        }
        
        // Se não foi encontrado um input válido, usa o primeiro
        if (numInput >= (inputs.length - NUMERO_INPUTS_MINIMO))
            numInput = 0;
        
        // Recupera a função que será executada
        var funcao = inputs[numInput].getAttribute("onclick");
        if (funcao == null)
            funcao = inputs[numInput].getAttribute("onchange");
        if (funcao == null)
            funcao = inputs[numInput].getAttribute("onblur");
        
        // Salva o nome do controle
        nomeControle = inputs[numInput].id;
    }
    else
    {
        // Recupera todos os selects da célula
        var selects = controles.getElementsByTagName("select");
        if (selects.length == 0)
            return;
        
        // Recupera a função que será executada
        var funcao = selects[0].getAttribute("onchange");
        
        // Salva o nome do controle
        nomeControle = selects[0].id;
    }
    
    // Altera a referência do controle de 'this' para 'document.getElementById'
    // ('this' referencia o objeto 'window')
    while (funcao.indexOf("this") > -1)
        funcao = funcao.replace("this", "document.getElementById('" + nomeControle + "')");
    
    // Executa a função
    eval(funcao);
}

// ---------------------------------------------------------
// Função que recalcula o valor de todos os beneficiamentos.
// ---------------------------------------------------------
function calculaTodos(prefixoControle)
{
    if (calculandoTotal)
        return;

    calculandoTotal = true;

    try
    {
        // Recupera a tabela do controle
        var tblBenef = document.getElementById(prefixoControle + "tblBenef");
        if (tblBenef == null)
            return;

        // Verifica se deve calcular mesmo, na tela de troca/devolução por exemplo, terá situações que não poderá recalcular
        var hdfRecalcBenef = FindControl("hdfRecalcBenef", "input");
        if (hdfRecalcBenef != null && hdfRecalcBenef.value.toString().toLowerCase() == "false")
            return;

        var numLinha = new Number();

        // Percorre todas as linhas da tabela
        for (numLinha = 0; numLinha < tblBenef.rows.length; numLinha++) {
            try {
                // Só calcula o total para a última linha
                calcularTotal = (numLinha == tblBenef.rows.length - 1);

                // Executa o cálculo para o primeiro beneficiamento
                efetuaCalculoBenef(tblBenef.rows[numLinha].cells[0], tblBenef.rows[numLinha].cells[1]);

                // Verifica se há o segundo beneficiamento
                if (tblBenef.rows[numLinha].cells[2].innerHTML == "")
                    continue;

                // Executa o cálculo para o segundo beneficiamento
                efetuaCalculoBenef(tblBenef.rows[numLinha].cells[2], tblBenef.rows[numLinha].cells[3]);
            }
            catch (err)
            { }
        }
    }
    finally
    {
        calculandoTotal = false;
    }
}

// --------------------------------------
// Função que calcula o valor do produto.
// --------------------------------------
function calculaValor(prefixoControle, prefixoItem, controle, funcaoValorAdicional, valorAdicionalCalculoBenef, callbackItem, callbackTotal, callbackSelecaoItem)
{
    // Garante que haja um controle
    if (controle == null || adicionandoBenef || removendoBenef)
        return;
    
    // Recupera o nome do controle de beneficiamentos
    var nomeControle = prefixoControle.substr(0, prefixoControle.length - 1);
    
    // Recupera as variáveis que controlam o fluxo
    atualizaVariaveis(nomeControle);
    
    // Calcula o valor do produto de acordo com o seu grupo
    if (exibirControleBenef(nomeControle))
        var dadosBenef = calculaValorVidro(nomeControle, prefixoControle, prefixoItem, controle, funcaoValorAdicional, valorAdicionalCalculoBenef, callbackItem, callbackTotal, callbackSelecaoItem);
    else
        var dadosBenef = null;
    
    // Chama o callback de cálculo do valor do item
    if (callbackItem != null && callbackItem != "" && dadosBenef != null)
        eval(callbackItem + "(" + dadosBenef.Valor + ", " + dadosBenef.Custo + ")");
    
    // Calcula os totais do beneficiamento
    if (calcularTotal)
        calculaTotal(nomeControle, prefixoControle, callbackTotal);
}

// ------------------------------------------------------------
// Função que calcula o valor do beneficiamento para os vidros.
// ------------------------------------------------------------
function calculaValorVidro(nomeControle, prefixoControle, prefixoItem, controle, funcaoValorAdicional, valorAdicionalCalculoBenef, callbackItem, callbackTotal, callbackSelecaoItem)
{
    // Recupera o ID do beneficiamento do controle
    var id = getBenefConfigIdFromControl(controle);
    
    // Tenta recuperar o beneficiamento através do ID
    var beneficiamento = getBenefConfigFromId(id);
    
    // Verifica se o beneficiamento foi encontrado
    if (beneficiamento.length != 1)
    {
        // Se o beneficiamento não foi encontrado
        if (beneficiamento.length == 0)
        {
            // Recupera os beneficiamentos filhos do ID do controle
            beneficiamento = getBenefConfigFromParentId(id);
        }
            
        // Depois filtra só beneficiamentos os do subgrupo usado
        beneficiamento = getBenefConfigFromSubgrupoId(beneficiamento);
        
        // Então só os beneficiamentos do tipo selecionado na lista
        beneficiamento = getBenefConfigByTipo(prefixoItem, beneficiamento);
        
        // Em seguida só os beneficiamentos para a cor escolhida
        beneficiamento = getBenefConfigByCor(dadosProduto.Cor, beneficiamento);
        
        // Finalmente só os beneneficiamentos para a espessura escolhida
        beneficiamento = getBenefConfigByEspessura(getEspessura(nomeControle), beneficiamento);
        
        // Se não foi encontrado beneficiamento
        if (beneficiamento.length == 0)
        {
            // Se não há um controle de seleção de tipo, sai da função
            if (controle.id.indexOf("drpTipo") == -1)
                return;
            else
            {
                // Verifica se o item selecionado na lista de tipos é um item vazio
                beneficiamento = getItemVazio(nomeControle, controle);
                if (beneficiamento == null)
                    return;
            }
        }
        
        // Recupera o beneficiamento usado pelo tipo se não for espessura
        if (beneficiamento[0].TipoEspessura == 0 && document.getElementById(prefixoItem + "drpTipo") != null)
        {
            // Recupera o controle do tipo
            var tipo = document.getElementById(prefixoItem + "drpTipo");
            
            // Seleciona apenas o beneficiamento do tipo
            for (i = 0; i < beneficiamento.length; i++)
                if (beneficiamento[i].ID == tipo.value)
                {
                    beneficiamento = beneficiamento[i];
                    break;
                }
        }
        
        // Seleciona o primeiro beneficiamento
        else
            beneficiamento = beneficiamento[0];
        
        // Se o beneficiamento não foi selecionado corretamente sai do método
        if (typeof beneficiamento.length != 'undefined')
            return;
    }
    else
        beneficiamento = beneficiamento[0];

    // Variável que indica se o cálculo será feito
    var deveCalcular = true;
    
    // Chama o callback de seleção do item
    if (chamarCallbackSelecaoItem && callbackSelecaoItem != null && callbackSelecaoItem != "")
    {
        // Invoca o callback de seleção do item
        var retornoCallback = eval(callbackSelecaoItem + "(beneficiamento, controle, getVar(nomeControle).Beneficiamentos, prefixoControle)");
        
        // Indica se o cálculo deve ser feito de acordo com o retorno do callback
        retornoCallback = retornoCallback == false ? false : true;
        if (!retornoCallback)
            deveCalcular = false;
    }

    if (calcularTotal && !getVar(nomeControle).Compra)
    {
        // Verifica se o beneficiamento deve ser calculado
        if (!deveCalcular || !calcular(prefixoItem, beneficiamento, controle) && getNumeroBeneficiamentos(nomeControle) == 0)
        {
            try
            {
                removendoBenef = true;
                setLargura(nomeControle, getLargura(nomeControle), true);
            }
            catch (err) { }
        }
        
        // Recalcula o m² (efeito apenas para o primeiro beneficiamento)
        if (!removendoBenef && !adicionandoBenef && getNumeroBeneficiamentos(nomeControle) == 1)
        {
            try
            {
                adicionandoBenef = true;
                setLargura(nomeControle, getLargura(nomeControle), true);
            }
            catch (err) { }
            finally
            {
                adicionandoBenef = false;
            }
        }
    }
    
    // Padroniza o valor do parâmetro
    valorAdicionalCalculoBenef = valorAdicionalCalculoBenef == true ? true : false;

    // Verifica se o beneficiamento escolhido é "Redondo"
    var beneficiamentoRedondo = beneficiamento.ID > 0 && beneficiamento.Descricao.toLowerCase() == "redondo";
    if (beneficiamentoRedondo)
        calculoRedondo(nomeControle, controle);
    
    // Efetua o cálculo do beneficiamento de acordo com o tipo do cálculo
    switch (beneficiamento.TipoCalculo)
    {
        case 1:
            var dadosBenef = calculoTipo1(nomeControle, prefixoItem, beneficiamento, controle, deveCalcular, 
                prefixoControle, funcaoValorAdicional, valorAdicionalCalculoBenef);
            break;
            
        case 2:
            var dadosBenef = calculoTipo2(nomeControle, prefixoItem, beneficiamento, controle, deveCalcular, 
                prefixoControle, funcaoValorAdicional, valorAdicionalCalculoBenef);
            break;
            
        case 3:
            var dadosBenef = calculoTipo3(nomeControle, prefixoItem, beneficiamento, controle, deveCalcular, 
                prefixoControle, funcaoValorAdicional, valorAdicionalCalculoBenef);
            break;
            
        case 4:
            var dadosBenef = calculoTipo4(nomeControle, prefixoItem, beneficiamento, controle, deveCalcular, 
                prefixoControle, funcaoValorAdicional, valorAdicionalCalculoBenef);
            break;
            
        default:
            var dadosBenef = {
                Valor: 0,
                ValorUnit: 0,
                Custo: 0
            };
            
            break;
    }

    // Atualiza o processo e aplicação do vidro
    var celulasBenef = recuperaCelulasBenef(nomeControle, beneficiamento.ParentID != null ? beneficiamento.ParentID : beneficiamento.ID);
    if (celulasBenef != null && celulasBenef.Controles != null && isBenefAplicado(celulasBenef.Controles))
        atualizaProcessoAplicacao(nomeControle, beneficiamento);
    
    removendoBenef = false;

    if (getVar(nomeControle).AlterarBenefAssoc && executarBenefAssoc && (dadosProduto.TipoCalculo == 2 || dadosProduto.TipoCalculo == 10))
    {
        // Desabilita a execução dessa parte novamente
        executarBenefAssoc = false;
        
        // Habilita os controles que estão bloqueados, desconsiderando os beneficiamentos padrão
        if (!carregandoPadrao && !iniciando)
            habilitaCampos(nomeControle, false);
        
        // Habilita a cobrança dos beneficiamentos associados
        var campoNaoCobrar = camposNaoCobrar.pop();
        while (campoNaoCobrar != null)
        {
            campoNaoCobrar.Campo.value = "false";
            efetuaCalculoBenef(campoNaoCobrar.Cabecalho, campoNaoCobrar.Controles);
            campoNaoCobrar = camposNaoCobrar.pop();
        }
        
        // Marca os beneficiamentos associados
        var benefAssoc = getBenefConfigAssocById(beneficiamento.ID);
        for (iAssoc = 0; iAssoc < benefAssoc.length; iAssoc++)
        {
            // Variável com o prefixo dos controles do beneficiamento associado
            var prefixoItemAssoc = nomeControle + "_" + benefAssoc[iAssoc].PrefixoBenefAssoc;
            
            // Altera o beneficiamento pelo tipo de cálculo
            switch (benefAssoc[iAssoc].TipoControleAssoc)
            {
                case 1: // Lapidacao
                case 2: // Bisote
                case 6: // ListaSelecao
                case 8: // ListaSelecaoQtd
                    
                    // Busca o campo do tipo
                    var drpTipo = document.getElementById(prefixoItemAssoc + "drpTipo");
                    if (drpTipo.disabled)
                        continue;
                    
                    // Altera o beneficiamento
                    drpTipo.value = benefAssoc[iAssoc].AssocID;
                    if (drpTipo.value != benefAssoc[iAssoc].AssocID)
                        continue;
                    
                    // Verifica se é lapidação ou bisotê
                    if (benefAssoc[iAssoc].TipoControleAssoc == 1 || benefAssoc[iAssoc].TipoControleAssoc == 2)
                    {
                        // Recupera os campos de altura e largura
                        var drpAltura = document.getElementById(prefixoItemAssoc + "drpAltura");
                        var drpLargura = document.getElementById(prefixoItemAssoc + "drpLargura");
                        
                        // Altera os valores
                        drpAltura.value = benefAssoc[iAssoc].AlturaBenef;
                        drpLargura.value = benefAssoc[iAssoc].LarguraBenef;
                        
                        // Verifica se o campo espessura existe
                        if (benefAssoc.TipoControleAssoc == 2)
                        {
                            var txtEspessura = document.getElementById(prefixoItemAssoc + "txtEspessura");
                            txtEspessura.value = benefAssoc[iAssoc].EspessuraBenef;
                        }
                    }
                    
                    // Verifica se é lista de seleção com quantidade
                    else if (benefAssoc[iAssoc].TipoControleAssoc == 8)
                    {
                        // Recupera o campo de quantidade
                        var txtQtd = document.getElementById(prefixoItemAssoc + "tblQtd_txtQtd");
                        if (txtQtd.disabled)
                            continue;
                        
                        txtQtd.value = benefAssoc[iAssoc].QtdeBenef;
                    }
                    
                    break;
                
                case 3: // SelecaoSimples
                    
                    // Recupera o checkbox
                    var chkSelecao = document.getElementById(prefixoItemAssoc + "chkSelecao");
                    if (chkSelecao.disabled)
                        continue;
                    
                    chkSelecao.checked = true;
                    
                    break;
                
                case 4: // SelecaoMultiplaInclusiva
                case 5: // SelecaoMultiplaExclusiva
                    
                    // Variável de controle
                    var jAssoc = 1;
                    
                    // Recupera o checkbox
                    var chkSelecao = document.getElementById(prefixoItemAssoc + "chkSelecao" + jAssoc);
                    if (chkSelecao.disabled)
                        continue;
                    
                    while (chkSelecao != null)
                    {
                        // Recupera o id do beneficiamento do controle
                        var benefSelecao = chkSelecao.parentNode.getAttribute("idBeneficiamento");
                        
                        // Verifica se o controle é o procurado
                        if (benefSelecao == benefAssoc[iAssoc].AssocID)
                        {
                            chkSelecao.checked = true;
                            break;
                        }
                        
                        // Recupera o próximo checkbox
                        jAssoc++;
                        chkSelecao = document.getElementById(prefixoItemAssoc + "chkSelecao" + jAssoc);
                    }
                    
                    break;
                
                case 7: // Quantidade
                    
                    // Recupera o campo de quantidade
                    var txtQtd = document.getElementById(prefixoItemAssoc + "tblQtd_txtQtd");
                    if (txtQtd.disabled)
                        continue;
                    
                    txtQtd.value = benefAssoc[iAssoc].QtdeBenef;
                    
                    break;
            }
            
            // Recupera as células do beneficiamento
            var celulas = recuperaCelulasBenef(nomeControle, benefAssoc[iAssoc].AssocParentID);
            
            // Define se o beneficiamento será cobrado
            if (!benefAssoc[iAssoc].CobrarAssoc)
            {
                document.getElementById(prefixoItemAssoc + "hdfNaoCobrarBenef").value = "true";
                
                var novo = {
                    Campo: document.getElementById(prefixoItemAssoc + "hdfNaoCobrarBenef"),
                    Cabecalho: celulas.Cabecalho,
                    Controles: celulas.Controles
                }
                
                camposNaoCobrar.push(novo);
            }
            
            // Desabilita os campos (bloqueia) se necessário
            if (benefAssoc[iAssoc].BloquearAssoc)
            {
                // Recupera os controles
                var inputs = celulas.Controles.getElementsByTagName("input");
                var selects = celulas.Controles.getElementsByTagName("select");
                
                // Habilita os controles
                for (iHab = 0; iHab < inputs.length; iHab++)
                    inputs[iHab].disabled = true;
                
                for (iHab = 0; iHab < selects.length; iHab++)
                    selects[iHab].disabled = true;
            }
            
            // Calcula o beneficiamento novamente
            efetuaCalculoBenef(celulas.Cabecalho, celulas.Controles);
        }
        
        // Reabilita a execução dessa parte
        executarBenefAssoc = true;
    }
    
    // Retorna os dados do beneficiamento
    return dadosBenef;
}

// -----------------------------------------------------
// Função que calcula o valor total dos beneficiamentos.
// -----------------------------------------------------
function calculaTotal(nomeControle, prefixoControle, callbackTotal)
{
    // Variável de retorno
    var total = {
        ValorUnit: 0,
        Valor: 0,
        Custo: 0
    };

    // Calcula o valor do produto se for alumínio ou se o tipo de cálculo for 4, 6, 7 ou 9 (ML AL)
    if (dadosProduto.TipoCalculo == 4 || dadosProduto.TipoCalculo == 6 || dadosProduto.TipoCalculo == 7 || dadosProduto.TipoCalculo == 9)
    {
        var campoAltura = document.getElementById(getVar(nomeControle).Altura);
        total.Valor = valorAluminio(campoAltura, getValorUnitario(nomeControle).toString(), getQuantidade(nomeControle), dadosProduto.TipoCalculo != 9 && !getVar(nomeControle).Compra);
        total.Valor = getValorPercComissao(nomeControle, total.Valor);
    }

    // Calcula o valor do produto se o tipo de cálculo for 3 (Perímetro)
    else if (dadosProduto.TipoCalculo == 3)
    {
        var ml = (getAltura(nomeControle) + getLargura(nomeControle)) * 2;
        total.Valor = getValorUnitario(nomeControle) * (ml / 1000) * getQuantidade(nomeControle);
        total.Valor = getValorPercComissao(nomeControle, total.Valor);
    }

    // Calcula o valor do produto se o tipo de cálculo for 8 (ML)
    else if (dadosProduto.TipoCalculo == 8)
    {
        total.Valor = getValorUnitario(nomeControle) * getAltura(nomeControle) * getQuantidade(nomeControle);
        total.Valor = getValorPercComissao(nomeControle, total.Valor);
    }

    // Calcula o valor para os outros casos
    else if (!(dadosProduto.TipoCalculo == 2 || dadosProduto.TipoCalculo == 10))
    {
        total.Valor = getValorUnitario(nomeControle) * getQuantidade(nomeControle);
        total.Valor = getValorPercComissao(nomeControle, total.Valor);
    }
    
    // Ignora o valor calculado, se o controle não for somar o total do produto no resultado
    if (!getVar(nomeControle).BeneficiamentosApenasVidros && !getVar(nomeControle).SomarTotalValorBenef)
        total = {
            ValorUnit: 0,
            Valor: 0,
            Custo: 0
        };

    // Calcula o valor dos beneficiamentos
    if (exibirControleBenef(nomeControle))
    {
        // Recupera a tabela dos beneficiamentos
        var tblBenef = document.getElementById(prefixoControle + "tblBenef");

        // Percorre todas as linhas da tabela
        for (i = 0; i < tblBenef.rows.length; i++)
        {
            // Percorre as células dos controles do beneficiamento
            for (j = 0; j < tblBenef.rows[i].cells.length / 2; j++)
            {
                // Recupera os inputs da célula dos controles
                var inputs = tblBenef.rows[i].cells[(j * 2) + 1].getElementsByTagName("input");
                if (inputs.length == 0)
                    continue;

                // Variáveis que contém os campos de valor e custo do beneficiamento
                var hdfValorUnit = new Array();
                var hdfValor = new Array();
                var hdfCusto = new Array();

                // Cria as variáveis de valor e custo
                for (k = 0; k < inputs.length; k++)
                {
                    if (inputs[k].id.indexOf("hdfValorUnit") > -1)
                        hdfValorUnit.push(inputs[k]);
                    else if (inputs[k].id.indexOf("hdfValor") > -1)
                        hdfValor.push(inputs[k]);
                    else if (inputs[k].id.indexOf("hdfCusto") > -1)
                        hdfCusto.push(inputs[k]);
                }

                // Soma os valores unitários do beneficiamento ao total
                for (k = 0; k < hdfValorUnit.length; k++)
                    total.ValorUnit += hdfValorUnit[k].value != "" ? parseFloat(hdfValorUnit[k].value) : 0;

                // Soma os valores do beneficiamento ao total
                for (k = 0; k < hdfValor.length; k++)
                    total.Valor += hdfValor[k].value != "" ? parseFloat(hdfValor[k].value) : 0;

                // Soma os custos do beneficiamento ao total
                for (k = 0; k < hdfCusto.length; k++)
                    total.Custo += hdfCusto[k].value != "" ? parseFloat(hdfCusto[k].value) : 0;
            }
        }
    }

    // Arredonda o total e o custo para 2 casas decimais
    total.ValorUnit = parseFloat(total.ValorUnit.toFixed(2));
    total.Valor = parseFloat(total.Valor.toFixed(2));
    total.Custo = parseFloat(total.Custo.toFixed(2));

    // Recupera os controles de total do beneficiamento
    var hdfValorUnitTotal = document.getElementById(prefixoControle + "hdfValorUnitTotal");
    var hdfValorTotal = document.getElementById(prefixoControle + "hdfValorTotal");
    var hdfCustoTotal = document.getElementById(prefixoControle + "hdfCustoTotal");
    
    // Salva o valor e o custo total nos controles
    hdfValorUnitTotal.value = total.ValorUnit;
    hdfValorTotal.value = total.Valor;
    hdfCustoTotal.value = total.Custo;

    //Recupera o IdProdPed para setar o valor total do benefeciamento de um produto composto
    var idProdPed = document.getElementById(prefixoControle + "hdf_benef_IdProdPed").value;

    if (idProdPed == "")
        idProdPed = 0;
    
    // Chama o callback do total
    if (callbackTotal != null && callbackTotal != "")
        eval(callbackTotal + "(" + total.Valor + ", " + total.Custo + ", " + idProdPed + ")");
}

// -----------------------------------------------------------------------------
// Função que retorna o valor do beneficiamento de acordo com o tipo de entrega.
// -----------------------------------------------------------------------------
function getValorBeneficiamento(nomeControle, controle, beneficiamento)
{
    // Recupera a célula do controle
    var celula = controle;
    while (celula.nodeName.toLowerCase() != "td" || celula.getAttribute("idBeneficiamento") == null)
        celula = celula.parentNode;
    
    // Verifica se o campo com o valor do beneficiamento existe
    var txtValorBenef = null;
    var inputs = celula.getElementsByTagName("input");
    for (i = 0; i < inputs.length; i++)
        if (inputs[i].id.indexOf("txtValorBenef") > -1)
        {
            txtValorBenef = inputs[i];
            break;
        }
    
    // Verifica o desconto/acréscimo que será aplicado ao beneficiamento
    var descontoAcrescimo = dadosProduto.DescontoAcrescimo;
    if (!dadosProduto.UsarDescontoAcrescimo || (beneficiamento.TipoControle != 1 && beneficiamento.TipoControle != 2))
        descontoAcrescimo = 1;
    
    // Retorna o valor do campo, se houver
    if (txtValorBenef != null)
    {
        try
        {
            var retorno = getValorCampo(txtValorBenef, "float");
//            if (beneficiamento.TipoCalculo == 4)
//            {
//                var divisor = getValorUnitario(nomeControle) * getTotalM2(nomeControle);
//                retorno = divisor > 0 ? retorno / divisor * 100 : 0;
//            }
            
            return retorno * descontoAcrescimo;
        }
        catch (e)
        {
            return 0;
        }
    }
    else
    {
        // Se for compra retorna o custo do beneficiamento
        if (getVar(nomeControle).Compra)
            return beneficiamento.Custo * descontoAcrescimo;
        
        // Se o cliente for revendedor retorna o valor de atacado
        else if (getRevenda(nomeControle))
            return beneficiamento.ValorAtacado * descontoAcrescimo;
        
        else
        {
            // Retorna o valor do beneficiamento de acordo com o tipo de entrega
            switch (getTipoEntrega(nomeControle))
            {
                case 1:
                case 4:
                    return beneficiamento.ValorBalcao * descontoAcrescimo;

                default:
                    return beneficiamento.ValorObra * descontoAcrescimo;
            }
        }
    }
}

// ----------------------------------------------------------
// Função que calcula o percentual de comissão sobre o valor.
// ----------------------------------------------------------
function getValorPercComissao(nomeControle, valor)
{
    // Adiciona o percentual de comissão ao valor
    return valor / ((100 - getPercComissao(nomeControle)) / 100);
}

// -------------------------------------------------
// Função que determina se o cálculo deve ser feito.
// -------------------------------------------------
function calcular(prefixo, beneficiamento, controle)
{
    // Recupera o controle do ID do beneficiamento aplicado
    var hdfIdBenefAplicado = document.getElementById(prefixo + "hdfIdBenefAplicado");
    
    // Se o controle marcado não for o de cobrança opcional
    if (controle.id.indexOf("chkOpcional") == -1)
    {
        // Verifica se o cálculo será feito de acordo com o tipo de controle
        switch (beneficiamento.TipoControle)
        {
            case 1:
                var calcular = calcularTipo1(prefixo, beneficiamento, controle);
                break;
                
            case 2:
                var calcular = calcularTipo2(prefixo, beneficiamento, controle);
                break;
                
            case 3:
                var calcular = calcularTipo3(prefixo, beneficiamento, controle);
                break;
                
            case 4:
                var calcular = calcularTipo4(prefixo, beneficiamento, controle);
                break;
                
            case 5:
                var calcular = calcularTipo5(prefixo, beneficiamento, controle);
                break;
                
            case 6:
                var calcular = calcularTipo6(prefixo, beneficiamento, controle);
                break;
                
            case 7:
                var calcular = calcularTipo7(prefixo, beneficiamento, controle);
                break;
                
            case 8:
                var calcular = calcularTipo8(prefixo, beneficiamento, controle);
                break;
        }
    }
    
    // O cálculo será feito de acordo com o campo de cobrança opcional estar marcado ou não
    else
        var calcular = true;
    
    // Se o cálculo for feito salva o ID do beneficiamento, senão salva 0
    if (hdfIdBenefAplicado != null)
        hdfIdBenefAplicado.value = calcular ? beneficiamento.ID : "0";

    // Retorna indicando se o cálculo será feito
    return calcular;
}

// -------------------------------------------------------------------------------
// Função que define se o beneficiamento de tipo de controle 1 deve ser calculado.
// -------------------------------------------------------------------------------
function calcularTipo1(prefixo, beneficiamento, controle)
{
    // O cálculo será feito se a validação do tipo 6 for confirmada e se a altura ou a largura for diferente de 0
    var drpAltura = document.getElementById(prefixo + "drpAltura");
    var drpLargura = document.getElementById(prefixo + "drpLargura");
    return calcularTipo6(prefixo, beneficiamento, controle) && (drpAltura.value != "0" || drpLargura.value != "0");
}

// -------------------------------------------------------------------------------
// Função que define se o beneficiamento de tipo de controle 2 deve ser calculado.
// -------------------------------------------------------------------------------
function calcularTipo2(prefixo, beneficiamento, controle)
{
    // O cálculo será feito do mesmo modo do tipo de controle 1
    return calcularTipo1(prefixo, beneficiamento, controle);
}

// -------------------------------------------------------------------------------
// Função que define se o beneficiamento de tipo de controle 3 deve ser calculado.
// -------------------------------------------------------------------------------
function calcularTipo3(prefixo, beneficiamento, controle)
{
    // O cálculo será feito se o controle estiver marcado
    var chkSelecao = document.getElementById(prefixo + "chkSelecao");
    return chkSelecao.checked;
}

// -------------------------------------------------------------------------------
// Função que define se o beneficiamento de tipo de controle 4 deve ser calculado.
// -------------------------------------------------------------------------------
function calcularTipo4(prefixo, beneficiamento, controle)
{
    // Variável de controle
    var i = 1;
    
    // O cálculo será feito se houver algum controle marcado
    while (document.getElementById(prefixo + "chkSelecao" + i.toString()) != null)
    {
        var chkSelecao = document.getElementById(prefixo + "chkSelecao" + i.toString());
        if (chkSelecao.checked)
            return true;
            
        i++;
    }
    
    return false;
}

// -------------------------------------------------------------------------------
// Função que define se o beneficiamento de tipo de controle 5 deve ser calculado.
// -------------------------------------------------------------------------------
function calcularTipo5(prefixo, beneficiamento, controle)
{
    // O cálculo será feito do mesmo modo do tipo de controle 4
    return calcularTipo4(prefixo, beneficiamento, controle);
}

// -------------------------------------------------------------------------------
// Função que define se o beneficiamento de tipo de controle 6 deve ser calculado.
// -------------------------------------------------------------------------------
function calcularTipo6(prefixo, beneficiamento, controle)
{
    // O cálculo será feito se o tipo estiver selecionado
    var drpTipo = document.getElementById(prefixo + "drpTipo");
    return drpTipo.value != "" && parseInt(drpTipo.value, 10) > 0;
}

// -------------------------------------------------------------------------------
// Função que define se o beneficiamento de tipo de controle 7 deve ser calculado.
// -------------------------------------------------------------------------------
function calcularTipo7(prefixo, beneficiamento, controle)
{
    // O cálculo será feito se a quantidade for maior que 0
    var txtQtd = document.getElementById(prefixo + "tblQtd_txtQtd");
    return txtQtd.value != "" && parseInt(txtQtd.value, 10) > 0;
}

// -------------------------------------------------------------------------------
// Função que define se o beneficiamento de tipo de controle 8 deve ser calculado.
// -------------------------------------------------------------------------------
function calcularTipo8(prefixo, beneficiamento, controle)
{
    // O cálculo será feito se as validações do tipo 6 e 7 forem confirmadas
    return calcularTipo6(prefixo, beneficiamento, controle) && calcularTipo7(prefixo, beneficiamento, controle);
}

// ------------------------------------------------------------------
// Verifica se a cobrança é opcional e, caso seja, se ela será feita.
// ------------------------------------------------------------------
function cobrancaOpcionalCobrar(prefixo)
{
    // Recupera o controle e verifica se ele existe e se está desmarcado
    var cobrar = document.getElementById(prefixo + "chkOpcional");
    if (cobrar != null && !cobrar.checked)
        return false;
    
    // Se o controle não for encontrado ou se ele estiver marcado
    return true;
}

// ---------------------------------------------------------
// Função que atualiza o valor e o custo nos campos ocultos.
// ---------------------------------------------------------
function atualizaValorCusto(nomeControle, prefixo, beneficiamento, controle, dadosBenef)
{
    // Variável com o número do campo de valor e custo que será usado
    var numeroCampo = "1";
    
    // Se for um campo de seleção múltipla inclusiva, recupera o número do campo
    if (beneficiamento.TipoControle == 4)
        numeroCampo = controle.id.substr(controle.id.lastIndexOf("_chkSelecao") + 11);

    // Atualiza o valor e o custo
    dadosBenef.ValorUnit = getValorPercComissao(nomeControle, dadosBenef.ValorUnit);
    dadosBenef.ValorUnit = parseFloat(dadosBenef.ValorUnit.toFixed(2));
    
    dadosBenef.Valor = getValorPercComissao(nomeControle, dadosBenef.Valor);
    dadosBenef.Valor = parseFloat(dadosBenef.Valor.toFixed(2));
    
    dadosBenef.Custo = parseFloat(dadosBenef.Custo.toFixed(2));
    
    // Recupera o controle que indica se o beneficiamento é padrão
    var benefPadrao = document.getElementById(prefixo + "hdfBenefPadrao").value == "true";
    
    // Recupera o controle que indica se o beneficiamento será cobrado
    var cobrarBenef = document.getElementById(prefixo + "hdfNaoCobrarBenef").value == "false";
        
    // Recupera os controles que conterão o valor e o custo
    var hdfValor = document.getElementById(prefixo + "hdfValor" + numeroCampo);
    var hdfValorUnit = document.getElementById(prefixo + "hdfValorUnit" + numeroCampo);
    var hdfCusto = document.getElementById(prefixo + "hdfCusto" + numeroCampo);
    
    // Salva o valor e o custo
    hdfValor.value = (!benefPadrao) && cobrarBenef ? dadosBenef.Valor : 0;
    hdfValorUnit.value = (!benefPadrao) && cobrarBenef ? dadosBenef.ValorUnit : 0;
    hdfCusto.value = (!benefPadrao) && cobrarBenef ? dadosBenef.Custo : 0;
}

// 
// Função que atualiza o valor do beneficiamento dentro da caixa que salva o valor, existente somene na compra
//
function atualizaTxtValorBenef(controle)
{
    // Se o próprio txtValorBenef estiver sendo alterado, não modifica o seu valor buscando o que está no banco de dados
    if (controle.type == "text")
        return 0;

    // recupera a célula na qual se encontra a txtValorBenef para em seguida recuperá-la
    var celula = controle.parentNode.parentNode;
    var txtValor = FindControl("txtValorBenef", "input", celula);
    
    // Se o txtValorBenef não estiver na tela, então não faz nada
    if (txtValor == null)
        return 0;
        
    var idBenef = getBenefConfigIdFromControl(controle);
    
    txtValor.value = ctrlBenef.GetCustoBenef(idBenef).value.replace('.', ',');
}

// ------------------------------------------------------------
// Função que calcula o valor do beneficiamento de tipo 1 (m²).
// ------------------------------------------------------------
function calculoTipo1(nomeControle, prefixo, beneficiamento, controle, deveCalcular, prefixoControle, funcaoValorAdicional, valorAdicionalCalculoBenef)
{
    // Variável de retorno
    var retorno = {
        Valor: 0,
        ValorUnit: 0,
        Custo: 0
    };
    
    // Verifica se não é o controle de cobrança opcional
    if (controle.id.indexOf("chkOpcional") == -1)
    {
        // Recupera os campos de descrição e informação dos serviços
        var hdfDescricao = document.getElementById(prefixo + "hdfDescricao");
        var hdfInfo = document.getElementById(prefixo + "hdfInfo");
        
        // Verifica se o cálculo será feito
        if (deveCalcular && calcular(prefixo, beneficiamento, controle))
        {
            var complementoInfo = "";
            
            // Verifica se o controle é de lapidação ou bisotê
            if (beneficiamento.TipoControle == 1 || beneficiamento.TipoControle == 2)
            {
                // Recupera a altura e a largura dos controles
                var drpAltura = document.getElementById(prefixo + "drpAltura");
                var drpLargura = document.getElementById(prefixo + "drpLargura");
                
                alturas = parseInt(drpAltura.value, 10);
                larguras = parseInt(drpLargura.value, 10);
                
                // Salva a informação adicional à variável
                complementoInfo = ";" + alturas + ";" + larguras;
                
                // Se o controle for de bisotê recupera a espessura selecionada
                if (beneficiamento.TipoControle == 2)
                {
                    var txtEspessura = document.getElementById(prefixo + "txtEspessura");
                    complementoInfo += ";" + (txtEspessura.value != "" ? txtEspessura.value : "0");
                }
            }
            
            // Atualiza o campo txtValorBenef no cadastro de compras.
            atualizaTxtValorBenef(controle);
            
            // Calcula o valor e o custo
            var valorAdicional = getValorAdicional(funcaoValorAdicional, beneficiamento, controle, getVar(nomeControle).Beneficiamentos, prefixoControle);
            var baseCalc = getTotalM2(nomeControle);
            retorno.ValorUnit = getValorBeneficiamento(nomeControle, controle, beneficiamento);
            retorno.Valor = (baseCalc + (valorAdicionalCalculoBenef ? valorAdicional : 0)) * retorno.ValorUnit;
            retorno.Custo = baseCalc * beneficiamento.Custo;
                
            // Calcula o valor adicional, se o cálculo for feito fora do cálculo do beneficiamento
            if (!valorAdicionalCalculoBenef)
                retorno.Valor += valorAdicional;
            
            // Verifica se a cobrança opcional será feita
            if (!cobrancaOpcionalCobrar(prefixo))
                retorno.Valor = 0;
            
            // Atualiza o valor e o custo
            atualizaValorCusto(nomeControle, prefixo, beneficiamento, controle, retorno);
            
            // Salva a descrição e os dados do serviço
            hdfDescricao.value = beneficiamento.DescricaoParent + beneficiamento.Descricao;
            hdfInfo.value = beneficiamento.ID + ";0;" + retorno.ValorUnit + ";" + retorno.Valor + ";" + retorno.Custo + ";" + getPercComissao(nomeControle) + complementoInfo;
        }
        else
        {
            // Atualiza o valor e o custo
            atualizaValorCusto(nomeControle, prefixo, beneficiamento, controle, retorno);
            
            // Limpa os campos de descrição e dados do serviço
            hdfDescricao.value = "";
            hdfInfo.value = "";
        }
    }
    else
    {
        // Recupera a célula e a linha que contém o controle atual
        var cabecalho = null;
        var linha = controle;
        while (linha.tagName.toLowerCase() != "tr")
        {
            cabecalho = linha;
            linha = linha.parentNode;
        }
        
        // Recupera a descrição do beneficiamento
        var descricao = beneficiamento.DescricaoParent != "" ? Trim(beneficiamento.DescricaoParent) : beneficiamento.Descricao;
        
        // Recupera a célula dos controles do beneficiamento e efetua o cálculo
        var controles = linha.cells[0].innerHTML.indexOf(descricao) > -1 ? linha.cells[1] : linha.cells[3];
        efetuaCalculoBenef(cabecalho, controles);
    }
    
    // Retorna o valor e o custo
    return retorno;
}

// ------------------------------------------------------------
// Função que calcula o valor do beneficiamento de tipo 2 (ml).
// ------------------------------------------------------------
function calculoTipo2(nomeControle, prefixo, beneficiamento, controle, deveCalcular, prefixoControle, funcaoValorAdicional, valorAdicionalCalculoBenef)
{
    // Variável de retorno
    var retorno = {
        Valor: 0,
        ValorUnit: 0,
        Custo: 0
    };
    
    // Verifica se não é o controle de cobrança opcional
    if (controle.id.indexOf("chkOpcional") == -1)
    {
        // Recupera os campos de descrição e informação dos serviços
        var hdfDescricao = document.getElementById(prefixo + "hdfDescricao");
        var hdfInfo = document.getElementById(prefixo + "hdfInfo");
        
        // Verifica se o cálculo será feito
        if (deveCalcular && calcular(prefixo, beneficiamento, controle))
        {
            // Variáveis de controle do cálculo
            var alturas = 2;
            var larguras = 2;
            var complementoInfo = "";
            
            // Verifica se o controle é de lapidação ou bisotê
            if (beneficiamento.TipoControle == 1 || beneficiamento.TipoControle == 2)
            {
                // Recupera a altura e a largura dos controles
                var drpAltura = document.getElementById(prefixo + "drpAltura");
                var drpLargura = document.getElementById(prefixo + "drpLargura");
                
                alturas = parseInt(drpAltura.value, 10);
                larguras = parseInt(drpLargura.value, 10);
                
                // Salva a informação adicional à variável
                complementoInfo = ";" + alturas + ";" + larguras;
                
                // Se o controle for de bisotê recupera a espessura selecionada
                if (beneficiamento.TipoControle == 2)
                {
                    var txtEspessura = document.getElementById(prefixo + "txtEspessura");
                    complementoInfo += ";" + (txtEspessura.value != "" ? txtEspessura.value : "0");
                }
            }
            
            // Calcula o valor e o custo
            var valorAdicional = getValorAdicional(funcaoValorAdicional, beneficiamento, controle, getVar(nomeControle).Beneficiamentos, prefixoControle);
            var baseCalc = getQuantidade(nomeControle) * ((getAltura(nomeControle) * alturas) + (getLargura(nomeControle) * larguras)) / 1000;
            retorno.ValorUnit = getValorBeneficiamento(nomeControle, controle, beneficiamento);
            retorno.Valor = (baseCalc + (valorAdicionalCalculoBenef ? valorAdicional : 0)) * retorno.ValorUnit;
            retorno.Custo = baseCalc * beneficiamento.Custo;
                
            // Calcula o valor adicional, se o cálculo for feito fora do cálculo do beneficiamento
            if (!valorAdicionalCalculoBenef)
                retorno.Valor += valorAdicional;
            
            // Verifica se a cobrança opcional será feita
            if (!cobrancaOpcionalCobrar(prefixo))
                retorno.Valor = 0;
            
            // Atualiza o valor e o custo
            atualizaValorCusto(nomeControle, prefixo, beneficiamento, controle, retorno);
            
            // Salva a descrição e os dados do serviço
            hdfDescricao.value = beneficiamento.DescricaoParent + beneficiamento.Descricao;
            hdfInfo.value = beneficiamento.ID + ";0;" + retorno.ValorUnit + ";" + retorno.Valor + ";" + retorno.Custo + ";" + getPercComissao(nomeControle) + complementoInfo;
        }
        else
        {
            // Atualiza o valor e o custo
            atualizaValorCusto(nomeControle, prefixo, beneficiamento, controle, retorno);
            
            // Limpa os campos de descrição e dados do serviço
            hdfDescricao.value = "";
            hdfInfo.value = "";
        }
    }
    else
    {
        // Recupera a célula e a linha que contém o controle atual
        var cabecalho = null;
        var linha = controle;
        while (linha.tagName.toLowerCase() != "tr")
        {
            cabecalho = linha;
            linha = linha.parentNode;
        }
        
        // Recupera a descrição do beneficiamento
        var descricao = beneficiamento.DescricaoParent != "" ? Trim(beneficiamento.DescricaoParent) : beneficiamento.Descricao;
        
        // Recupera a célula dos controles do beneficiamento e efetua o cálculo
        var controles = linha.cells[0].innerHTML.indexOf(descricao) > -1 ? linha.cells[1] : linha.cells[3];
        efetuaCalculoBenef(cabecalho, controles);
    }
    
    return retorno;
}

// -------------------------------------------------------------
// Função que calcula o valor do beneficiamento de tipo 3 (Qtd).
// -------------------------------------------------------------
function calculoTipo3(nomeControle, prefixo, beneficiamento, controle, deveCalcular, prefixoControle, funcaoValorAdicional, valorAdicionalCalculoBenef)
{
    // Variável de retorno
    var retorno = {
        Valor: 0,
        ValorUnit: 0,
        Custo: 0
    };
    
    // Verifica se não é o controle de cobrança opcional
    if (controle.id.indexOf("chkOpcional") == -1)
    {
        // Recupera os campos de descrição e informação dos serviços
        var hdfDescricao = document.getElementById(prefixo + "hdfDescricao");
        var hdfInfo = document.getElementById(prefixo + "hdfInfo");
        
        // Verifica se o cálculo será feito
        if (deveCalcular && calcular(prefixo, beneficiamento, controle))
        {
            var complementoInfo = "";
            
            // Verifica se o controle é de lapidação ou bisotê
            if (beneficiamento.TipoControle == 1 || beneficiamento.TipoControle == 2)
            {
                // Recupera a altura e a largura dos controles
                var drpAltura = document.getElementById(prefixo + "drpAltura");
                var drpLargura = document.getElementById(prefixo + "drpLargura");
                
                alturas = parseInt(drpAltura.value, 10);
                larguras = parseInt(drpLargura.value, 10);
                
                // Salva a informação adicional à variável
                complementoInfo = ";" + alturas + ";" + larguras;
                
                // Se o controle for de bisotê recupera a espessura selecionada
                if (beneficiamento.TipoControle == 2)
                {
                    var txtEspessura = document.getElementById(prefixo + "txtEspessura");
                    complementoInfo += ";" + (txtEspessura.value != "" ? txtEspessura.value : "0");
                }
            }
            
            // Recupera a quantidade selecionada no controle
            var qtd = 0;
            if (beneficiamento.TipoControle == 7 || beneficiamento.TipoControle == 8)
            {
                var txtQtd = document.getElementById(prefixo + "tblQtd_txtQtd");
                qtd = txtQtd.value != "" ? parseInt(txtQtd.value, 10) : 0;
            }
            /* Chamado 23568. */
            else if (beneficiamento.TipoControle == 6)
                qtd = 1;
            
            // Calcula o valor e o custo
            var valorAdicional = getValorAdicional(funcaoValorAdicional, beneficiamento, controle, getVar(nomeControle).Beneficiamentos, prefixoControle);
            var baseCalc = getQuantidade(nomeControle) * qtd;
            retorno.ValorUnit = getValorBeneficiamento(nomeControle, controle, beneficiamento);
            retorno.Valor = (baseCalc + (valorAdicionalCalculoBenef ? valorAdicional : 0)) * retorno.ValorUnit;
            retorno.Custo = baseCalc * beneficiamento.Custo;
                
            // Calcula o valor adicional, se o cálculo for feito fora do cálculo do beneficiamento
            if (!valorAdicionalCalculoBenef)
                retorno.Valor += valorAdicional;
            
            // Verifica se a cobrança opcional será feita
            if (!cobrancaOpcionalCobrar(prefixo))
                retorno.Valor = 0;
            
            // Atualiza o valor e o custo
            atualizaValorCusto(nomeControle, prefixo, beneficiamento, controle, retorno);
            
            // Salva a descrição e os dados do serviço
            hdfDescricao.value = qtd + " " + beneficiamento.Descricao;
            hdfInfo.value = beneficiamento.ID + ";" + qtd + ";" + retorno.ValorUnit + ";" + retorno.Valor + ";" + retorno.Custo + ";" + getPercComissao(nomeControle) + complementoInfo;
        }
        else
        {
            // Atualiza o valor e o custo
            atualizaValorCusto(nomeControle, prefixo, beneficiamento, controle, retorno);
            
            // Limpa os campos de descrição e dados do serviço
            hdfDescricao.value = "";
            hdfInfo.value = "";
        }
    }
    else
    {
        // Recupera a célula e a linha que contém o controle atual
        var cabecalho = null;
        var linha = controle;
        while (linha.tagName.toLowerCase() != "tr")
        {
            cabecalho = linha;
            linha = linha.parentNode;
        }
        
        // Recupera a descrição do beneficiamento
        var descricao = beneficiamento.DescricaoParent != "" ? Trim(beneficiamento.DescricaoParent) : beneficiamento.Descricao;
        
        // Recupera a célula dos controles do beneficiamento e efetua o cálculo
        var controles = linha.cells[0].innerHTML.indexOf(descricao) > -1 ? linha.cells[1] : linha.cells[3];
        efetuaCalculoBenef(cabecalho, controles);
    }
    
    return retorno;
}

// -----------------------------------------------------------
// Função que calcula o valor do beneficiamento de tipo 4 (%).
// -----------------------------------------------------------
function calculoTipo4(nomeControle, prefixo, beneficiamento, controle, deveCalcular, prefixoControle, funcaoValorAdicional, valorAdicionalCalculoBenef)
{
    // Variável de retorno
    var retorno = {
        Valor: 0,
        ValorUnit: 0,
        Custo: 0
    };
    
    // Verifica se não é o controle de cobrança opcional
    if (controle.id.indexOf("chkOpcional") == -1)
    {
        // Recupera os campos de descrição e informação dos serviços
        var hdfDescricao = document.getElementById(prefixo + "hdfDescricao");
        var hdfInfo = document.getElementById(prefixo + "hdfInfo");
        
        // Verifica se o cálculo será feito
        if (deveCalcular && calcular(prefixo, beneficiamento, controle))
        {
            var complementoInfo = "";
            // Criado para recuperar a quantidade de beneficiamento, usado somente para porcentagem.
            var quantidade = 1;
            
            // Verifica se o controle é de lapidação ou bisotê
            if (beneficiamento.TipoControle == 1 || beneficiamento.TipoControle == 2)
            {
                // Recupera a altura e a largura dos controles
                var drpAltura = document.getElementById(prefixo + "drpAltura");
                var drpLargura = document.getElementById(prefixo + "drpLargura");
                
                alturas = parseInt(drpAltura.value, 10);
                larguras = parseInt(drpLargura.value, 10);
                
                // Salva a informação adicional à variável
                complementoInfo = ";" + alturas + ";" + larguras;
                
                // Se o controle for de bisotê recupera a espessura selecionada
                if (beneficiamento.TipoControle == 2)
                {
                    var txtEspessura = document.getElementById(prefixo + "txtEspessura");
                    complementoInfo += ";" + (txtEspessura.value != "" ? txtEspessura.value : "0");
                }
            }
            // Caso o controle seja calculado por quantidade e % então a porcentagem de cada quantidade será cobrada.
            else if (beneficiamento.TipoControle == 7) {
                // Se o valor for igual a zero ou se o controle não existir salva a quantidade 1, caso contrário recupera o valor do controle.
                quantidade = document.getElementById(prefixo + "tblQtd_txtQtd");
                quantidade = quantidade != null ? quantidade.value : quantidade.value == 0 ? 1 : quantidade.value;
            }
            
            // Calcula o valor e o custo
            var valorAdicional = getValorAdicional(funcaoValorAdicional, beneficiamento, controle, getVar(nomeControle).Beneficiamentos, prefixoControle);
            var baseCalcValor = getValorUnitario(nomeControle) / 100 * getTotalM2(nomeControle);
            var baseCalcCusto = (getCusto(nomeControle) > 0 ? getCusto(nomeControle) : getValorUnitario(nomeControle)) / 100 * getTotalM2(nomeControle);
            var valorBenef = getValorBeneficiamento(nomeControle, controle, beneficiamento)

            // O valor unitário deve salvar o valor do beneficimento mesmo, caso contrário, ocorreria erro ao inserir o mesmo em uma compra,
            // o valor estava sempre sendo alterado a cada vez que o produto era editado e atualizado
            retorno.ValorUnit = valorBenef;  //valorBenef * getValorUnitario(nomeControle) / 100;
            retorno.Valor = ((baseCalcValor + (valorAdicionalCalculoBenef ? valorAdicional : 0)) * valorBenef) * quantidade;
            retorno.Custo = baseCalcCusto * beneficiamento.Custo;
                
            // Calcula o valor adicional, se o cálculo for feito fora do cálculo do beneficiamento
            if (!valorAdicionalCalculoBenef)
                retorno.Valor += valorAdicional;
            
            // Verifica se a cobrança opcional será feita
            if (!cobrancaOpcionalCobrar(prefixo))
                retorno.Valor = 0;
            
            // Atualiza o valor e o custo
            atualizaValorCusto(nomeControle, prefixo, beneficiamento, controle, retorno);
            
            // Salva a descrição e os dados do serviço
            hdfDescricao.value = beneficiamento.DescricaoParent + beneficiamento.Descricao;
            hdfInfo.value = beneficiamento.ID + ";0;" + retorno.ValorUnit + ";" + retorno.Valor + ";" + retorno.Custo + ";" + getPercComissao(nomeControle) + complementoInfo;
        }
        else
        {
            // Atualiza o valor e o custo
            atualizaValorCusto(nomeControle, prefixo, beneficiamento, controle, retorno);
            
            // Limpa os campos de descrição e dados do serviço
            hdfDescricao.value = "";
            hdfInfo.value = "";
        }
    }
    else
    {
        // Recupera a célula e a linha que contém o controle atual
        var cabecalho = null;
        var linha = controle;
        while (linha.tagName.toLowerCase() != "tr")
        {
            cabecalho = linha;
            linha = linha.parentNode;
        }
        
        // Recupera a descrição do beneficiamento
        var descricao = beneficiamento.DescricaoParent != "" ? Trim(beneficiamento.DescricaoParent) : beneficiamento.Descricao;
        
        // Recupera a célula dos controles do beneficiamento e efetua o cálculo
        var controles = linha.cells[0].innerHTML.indexOf(descricao) > -1 ? linha.cells[1] : linha.cells[3];
        efetuaCalculoBenef(cabecalho, controles);
    }
    
    return retorno;
}

// -------------------------------------------------
// Função que efetua as mudanças para vidro redondo.
// -------------------------------------------------
function calculoRedondo(nomeControle, controle)
{
    // Se o controle não mudou o valor sai do método
    if (controle.checked == redondo)
        return;
    
    // Salva o novo valor
    redondo = controle.checked;
    
    // Indica ao campo de largura para chamar o callback
    setLargura(nomeControle, getLargura(nomeControle), true);
}

// --------------------------------------------------------------------------
// Função que retorna os serviços formatados para a tela de orçamento rápido.
// --------------------------------------------------------------------------
function getServicos(nomeControle)
{
    // Variável de retorno
    var retorno = {
        Descricao: "",
        Info: ""
    }
    
    // Recupera a tabela dos beneficiamentos do controle
    var tblBenef = document.getElementById(nomeControle + "_tblBenef");
    
    // Percorre todas as linhas da tabela
    for (i = 0; i < tblBenef.rows.length; i++)
    {
        // Percorre as células dos controles
        for (j = 0; j < tblBenef.rows[i].cells.length / 2; j++)
        {
            // Recupera todos os inputs da célula
            var inputs = tblBenef.rows[i].cells[(j * 2) + 1].getElementsByTagName("input");
            if (inputs.length == 0)
                continue;
            
            // Variáveis que são usadas para verificar os dados do beneficiamento
            var hdfIdBenefAplicado = null;
            var hdfDescricao = null;
            var hdfInfo = null;
            
            // Percorre todos os inputs selecionados
            for (k = 0; k < inputs.length; k++)
            {
                // Verifica se o input é o que contém o ID do beneficiamento
                if (inputs[k].id.indexOf("hdfIdBenefAplicado") > -1)
                {
                    hdfIdBenefAplicado = inputs[k];
                    if (hdfDescricao != null && hdfInfo != null)
                        break;
                }
                
                // Verifica se o input é o que contém a descrição do beneficiamento
                else if (inputs[k].id.indexOf("hdfDescricao") > -1)
                {
                    hdfDescricao = inputs[k];
                    if (hdfIdBenefAplicado != null && hdfInfo != null)
                        break;
                }
                
                // Verifica se o input é o que contém os dados do serviço
                else if (inputs[k].id.indexOf("hdfInfo") > -1)
                {
                    hdfInfo = inputs[k];
                    if (hdfIdBenefAplicado != null && hdfDescricao != null)
                        break;
                }
            }
            
            // Se o ID do beneficiamento for 0 significa que não há beneficiamento
            if (hdfIdBenefAplicado == null || hdfIdBenefAplicado.value == "0")
                continue;
            
            // Concatena a descrição e os dados do serviço à variável de retorno
            retorno.Descricao += ", " + hdfDescricao.value;
            retorno.Info += "|" + hdfInfo.value;
        }
    }
    
    // Remove os caracteres do início dos dados do retorno
    retorno.Descricao = retorno.Descricao.substr(2);
    retorno.Info = retorno.Info.substr(1);
    
    // Retorna os dados dos beneficiamentos
    return retorno;
}

// -----------------------------------------------------------
// Função que altera o tipo de cálculo para um beneficiamento.
// -----------------------------------------------------------
function alteraTipoCalculo(prefixoControle, idBeneficiamentoPai, tipoCalculoNovo)
{
    // Variável de controle dos identificadores
    var ids = new Array(idBeneficiamentoPai.toString());
    
    // Executa enquanto houver algum identificador a ser verificado
    while (ids.length > 0)
    {
        // Recupera um identificador da lista
        var idAtual = ids.pop();
        
        // Percorre todos os beneficiamentos
        for (i = 0; i < benefConfig.length; i++)
        {
            // Verifica se o identificador do pai é um identificador desejado
            if (benefConfig[i].ParentID == idAtual)
            {
                // Se ele não for calculado, adiciona o seu identificador à lista de verificação
                if (benefConfig[i].Calcular)
                {
                    // Verifica se há alteração a ser feita
                    if (benefConfig[i].TipoCalculo != tipoCalculoNovo)
                        benefConfig[i].TipoCalculo = tipoCalculoNovo;
                    else
                        return;
                }
                else
                    ids.push(benefConfig[i].ID);
            }
        }
    }
    
    // Refaz os cálculos do beneficiamento
    var celulas = recuperaCelulasBenef(prefixoControle, idBeneficiamentoPai);
    if (celulas.Cabecalho != null && celulas.Controles != null)
        efetuaCalculoBenef(celulas.Cabecalho, celulas.Controles);
}

// -------------------------------------------------------
// Função que recupera o valor de um beneficiamento feito.
// -------------------------------------------------------
function recuperaValorBenef(prefixoControle, idBeneficiamentoPai)
{
    // Variável de retorno
    var valor = 0;
    
    // Recupera as células do beneficiamento
    var celulas = recuperaCelulasBenef(prefixoControle, idBeneficiamentoPai);
    
    // Verifica se a célula dos controles foi encontrada
    if (celulas.Controles != null)
    {
        // Variável que guarda os valores encontrados
        var hdfValor = new Array();
        
        // Recupera os campos da célula
        var inputs = celulas.Controles.getElementsByTagName("input");
        
        // Salva todos os campos de valor na lista
        for (i = 0; i < inputs.length; i++)
        {
            if (inputs[i].id.indexOf("hdfValor") > -1)
                hdfValor.push(inputs[i]);
        }
        
        // Soma os valores do beneficiamento
        for (i = 0; i < hdfValor.length; i++)
        {
            var tempValor = parseFloat(hdfValor[i].value.replace(',', '.'));
            if (isNaN(tempValor))
                tempValor = 0;
            valor += tempValor;
        }
    }
    
    // Retorna o valor do beneficiamento
    return valor;
}

// --------------------------------------
// Recupera as células do beneficiamento.
// --------------------------------------
function recuperaCelulasBenef(nomeControle, idBeneficiamentoPai)
{
    // Variável de retorno
    var retorno = {
        Cabecalho: null,
        Controles: null
    };
    
    // Verifica se há necessidade de se colocar um underline do final do nome do controle
    if (nomeControle[nomeControle.length - 1] != "_")
        nomeControle += "_";
    
    // Recupera a tabela dos beneficiamentos do controle
    var tblBenef = document.getElementById(nomeControle + "tblBenef");
    
    // Percorre todas as linhas da tabela
    for (i = 0; i < tblBenef.rows.length; i++)
    {
        // Percorre as células dos controles
        for (j = 0; j < tblBenef.rows[i].cells.length / 2; j++)
        {
            // Recupera o ID do beneficiamento da célula
            var idBeneficiamento = tblBenef.rows[i].cells[(j * 2) + 1].getAttribute("IdBeneficiamento");
            
            // Verifica se a célula atual é a célula procurada
            if (idBeneficiamento == idBeneficiamentoPai)
            {
                // Recupera e retorna as células
                retorno.Cabecalho = tblBenef.rows[i].cells[(j * 2)];
                retorno.Controles = tblBenef.rows[i].cells[(j * 2) + 1];
                return retorno;
            }
        }
    }
    
    // Retorno em caso do beneficiamento não ser encontrado
    return retorno;
}

// ------------------------------------------------
// Função que habilita todos os campos do controle.
// ------------------------------------------------
function habilitaCampos(nomeControle, incluirPadrao)
{
    // Recupera a tabela
    var tabela = document.getElementById(nomeControle + "_tblBenef");
    if (tabela == null)
        return;
    
    // Recupera os controles
    var inputs = tabela.getElementsByTagName("input");
    var selects = tabela.getElementsByTagName("select");
    
    // Habilita os controles
    for (iHab = 0; iHab < inputs.length; iHab++)
    {
        if (inputs[iHab].type == "hidden")
            continue;
        
        if (!incluirPadrao)
        {
            var celula = inputs[iHab];
            while (celula.parentNode != null && celula.nodeName.toLowerCase() != "td" && celula.getAttribute("idBeneficiamento") == null)
                celula = celula.parentNode;
            
            if (celula.parentNode == null || celula.nodeName.toLowerCase() != "td" || isBenefPadrao(celula))
                continue;
        }
        
        inputs[iHab].disabled = false;
    }
    
    for (iHab = 0; iHab < selects.length; iHab++)
    {
        if (!incluirPadrao)
        {
            var celula = selects[iHab];
            while (celula.parentNode != null && celula.nodeName.toLowerCase() != "td" && celula.getAttribute("idBeneficiamento") == null)
                celula = celula.parentNode;
            
            if (celula.parentNode == null || celula.nodeName.toLowerCase() != "td" || isBenefPadrao(celula))
                continue;
        }
        
        selects[iHab].disabled = false;
    }
}

// -------------------------------------------------------------------
// Função responsável por carregar o beneficiamento padrão do produto.
// -------------------------------------------------------------------
function loadBenef(nomeControle, benef, redondo, isPadrao)
{
    // Sai do método se os beneficiamentos não forem recuperados
    if (benef == null || typeof benef != "object")
    {
        calculaTodos(nomeControle + "_");
        return;
    }
    
    // Habilita os campos do controle
    if (dadosProduto.TipoCalculo == 2 || dadosProduto.TipoCalculo == 10)
        habilitaCampos(nomeControle, true);
    
    // Indica o carregamento dos beneficiamentos padrão
    carregandoPadrao = true;
    
    // Percorre todos os beneficiamentos do produto
    for (iLoadBenef = 0; iLoadBenef < benef.length; iLoadBenef++)
    {
        // Prefixo dos controles que serão alterados
        var prefixoControle = nomeControle + "_" + benef[iLoadBenef].Prefixo;
        
        // Verifica o tipo de controle
        switch (benef[iLoadBenef].TipoControle)
        {
            case 1:
            case 2:
                document.getElementById(prefixoControle + "drpTipo").value = benef[iLoadBenef].ID;
                document.getElementById(prefixoControle + "drpTipo").disabled = true;
                document.getElementById(prefixoControle + "drpAltura").value = benef[iLoadBenef].Altura;
                document.getElementById(prefixoControle + "drpAltura").disabled = true;
                document.getElementById(prefixoControle + "drpLargura").value = benef[iLoadBenef].Largura;
                document.getElementById(prefixoControle + "drpLargura").disabled = true;
                
                if (benef[iLoadBenef].TipoControle == 2)
                {
                    document.getElementById(prefixoControle + "txtEspessura").value = benef[iLoadBenef].Espessura;
                    document.getElementById(prefixoControle + "txtEspessura").disabled = isPadrao;
                }
                
                break;
            
            case 3:
                document.getElementById(prefixoControle + "chkSelecao").checked = true;
                document.getElementById(prefixoControle + "chkSelecao").disabled = isPadrao;
                break;
            
            case 4:
            case 5:
                var j = 1;
                while (document.getElementById(prefixoControle + "chkSelecao" + j) != null)
                {
                    if (document.getElementById(prefixoControle + "chkSelecao" + j).parentNode.getAttribute("idBeneficiamento") == benef[iLoadBenef].ID)
                    {
                        document.getElementById(prefixoControle + "chkSelecao" + j).checked = true;
                        document.getElementById(prefixoControle + "chkSelecao" + j).disabled = isPadrao;
                        
                        if (benef[iLoadBenef].TipoControle == 5)
                            break;
                    }
                    
                    j++;
                }
                break;
            
            case 6:
                document.getElementById(prefixoControle + "drpTipo").value = benef[iLoadBenef].ID;
                document.getElementById(prefixoControle + "drpTipo").disabled = isPadrao;
                break;
            
            case 7:
            case 8:
                if (benef[iLoadBenef].TipoControle == 8)
                {
                    document.getElementById(prefixoControle + "drpTipo").value = benef[iLoadBenef].ID;
                    document.getElementById(prefixoControle + "drpTipo").disabled = isPadrao;
                }
                
                document.getElementById(prefixoControle + "tblQtd_txtQtd").value = benef[iLoadBenef].Qtde;
                document.getElementById(prefixoControle + "tblQtd_txtQtd").disabled = isPadrao;
                document.getElementById(prefixoControle + "tblQtd_imbUp").disabled = isPadrao;
                document.getElementById(prefixoControle + "tblQtd_imbDown").disabled = isPadrao;

                break;
        }
        
        // Indica se o beneficiamento é padrão do produto
        document.getElementById(prefixoControle + "hdfBenefPadrao").value = isPadrao;
        
        // Recupera as células do beneficiamento
        var celulasBenef = recuperaCelulasBenef(nomeControle, benef[iLoadBenef].ParentID != null ? benef[iLoadBenef].ParentID : benef[iLoadBenef].ID);
        efetuaCalculoBenef(celulasBenef.Cabecalho, celulasBenef.Controles);
    }
    
    // Verifica se o checkbox 'Redondo' deve ser marcado
    if (redondo)
    {
        // Recupera o id do beneficiamento
        var idBenefRedondo = document.getElementById(nomeControle + "_Redondo_chkSelecao").parentNode.parentNode.getAttribute("idBeneficiamento");
        
        // Recupera as células do beneficiamento
        var celulasBenef = recuperaCelulasBenef(nomeControle, idBenefRedondo);

        document.getElementById(nomeControle + "_Redondo_chkSelecao").checked = true;
        
        // Esta linha foi comentada para que ao fazer postback o sistema consiga identificar se o checkbox redondo está marcado, quando for padrão
        //document.getElementById(nomeControle + "_Redondo_chkSelecao").disabled = isPadrao;
        efetuaCalculoBenef(celulasBenef.Cabecalho, celulasBenef.Controles);
    }
    
    // Recalcula os beneficiamentos
    calculaTodos(nomeControle + "_");
    
    // Indica que não está mais carregando os beneficiamentos padrão
    carregandoPadrao = false;
}

// -------------------------------------------------------------------
// Função responsável por carregar o beneficiamento padrão do produto.
// -------------------------------------------------------------------
function loadBenefPadrao(nomeControle)
{
    // Só carrega os beneficiamentos padrão se o controle tiver essa propriedade
    if (!eval(nomeControle).CarregarBenefPadrao)
        return;
    
    if (dadosProduto.ID > 0)
        carregarBeneficiamentos(nomeControle, dadosProduto.ID, "produto", true);
        
    if (dadosPecaItemProjeto.ID > 0)
        carregarBeneficiamentos(nomeControle, dadosPecaItemProjeto.ID, "projeto", true);
}

// ---------------------------------------------------------------------------
// Função que carrega os beneficiamentos de um produto de orçamento, pedido...
// ---------------------------------------------------------------------------
function carregarBeneficiamentos(nomeControle, id, tipo, padrao)
{
    // Variável de controle
    var benef = new Array ({
        ID: 0,
        ParentID: null,
        Altura: 0,
        Largura: 0,
        Espessura: 0,
        Qtde: 0,
        TipoControle: 0,
        Prefixo: ''
    });
    
    // Recupera os beneficiamentos do produto
    eval("benef = " + ctrlBenef.GetBenefByProd(id, tipo).value);
    
    if (benef != null && benef.length > 0)
    {
        var redondo = ctrlBenef.IsRedondo(id, tipo).value == "true";

        // Padroniza o valor do parâmetro
        padrao = padrao == true ? true : false;

        // Atualiza as variáveis
        atualizaVariaveis(nomeControle);

        // Carrega os beneficiamentos
        loadBenef(nomeControle, benef, redondo, padrao);
    }
}

// ----------------------------------------------------
// Função executada quando é feito um submit na página.
// ----------------------------------------------------
function benefSubmit()
{
    for (i = 0; i < benef_habilitar.length; i++)
        habilitaCampos(benef_habilitar[i], true);
}

// ------------------------------------------------------------------------
// Função usada para verificar se um beneficiamento foi aplicado na célula.
// ------------------------------------------------------------------------
function isBenefAplicado(celula)
{
    // Recupera os inputs da célula
    var inputs = celula.getElementsByTagName("input");

    // Recupera o id do beneficiamento
    var id = "";
    for (k = 0; k < inputs.length; k++)
        if (inputs[k].id.indexOf("hdfIdBenefAplicado") > -1)
        {
            id = parseInt(inputs[k].value, 10);
            if (isNaN(id)) id = 0;
            break;
        }
    
    // Indica se o beneficiamento foi aplicado
    return id > 0;
}

// -----------------------------------------------------------------------------
// Função usada para verificar se um beneficiamento aplicado na célula é padrão.
// -----------------------------------------------------------------------------
function isBenefPadrao(celula)
{
    // Recupera os inputs da célula
    var inputs = celula.getElementsByTagName("input");

    // Recupera o id do beneficiamento
    var retorno = false;
    for (k = 0; k < inputs.length; k++)
        if (inputs[k].id.indexOf("hdfBenefPadrao") > -1)
        {
            retorno = inputs[k].value.toLowerCase() == "true";
            break;
        }
    
    // Indica se o beneficiamento é padrão
    return retorno;
}

// ---------------------------------------------------------------------
// Função executada para retornar o número de beneficiamentos aplicados.
// ---------------------------------------------------------------------
function getNumeroBeneficiamentos(nomeControle)
{    
    // Variável com o número de beneficiamentos aplicados
    var retorno = 0;
    
    // Recupera a tabela dos beneficiamentos do controle
    var tblBenef = document.getElementById(nomeControle + "_tblBenef");
    
    // Percorre todas as linhas da tabela
    for (i = 0; i < tblBenef.rows.length; i++)
    {
        // Percorre as células dos controles
        for (j = 0; j < tblBenef.rows[i].cells.length / 2; j++)
        {
            // Conta o beneficiamento, se ele foi aplicado
            if (isBenefAplicado(tblBenef.rows[i].cells[(j * 2) + 1]))
            {
                retorno++;

                var idBeneficiamentoPai = tblBenef.rows[i].cells[(j * 2) + 1].getAttribute("idBeneficiamento");
                for (k = 0; k < eval(nomeControle).Beneficiamentos.length; k++)
                    if (eval(nomeControle).Beneficiamentos[k].ID == idBeneficiamentoPai)
                    {
                        if (!eval(nomeControle).Beneficiamentos[k].CobrarAreaMinima)
                            retorno--;
                            
                        break;
                    }
            }
        }
    }
    
    // Retorna o número de beneficiamentos
    return retorno;
}

// ------------------------------------------------
// Função que limpa os beneficiamentos do controle.
// ------------------------------------------------
function limparBenef(prefixoControle, callbackTotal)
{
    // Recupera o nome do controle de beneficiamentos
    var nomeControle = prefixoControle.substr(0, prefixoControle.length - 1);
    
    // Recupera a tabela dos beneficiamentos do controle
    var tblBenef = document.getElementById(prefixoControle + "tblBenef");
    
    // Percorre todas as linhas da tabela
    for (i = 0; i < tblBenef.rows.length; i++)
    {
        // Percorre as células dos controles
        for (j = 0; j < tblBenef.rows[i].cells.length / 2; j++)
        {
            // Se o beneficiamento não foi aplicado nesta célula passa para a próxima
            if (!isBenefAplicado(tblBenef.rows[i].cells[(j * 2) + 1]))
                continue;
            
            // Recupera os controles da célula
            var inputs = tblBenef.rows[i].cells[(j * 2) + 1].getElementsByTagName("input");
            var selects = tblBenef.rows[i].cells[(j * 2) + 1].getElementsByTagName("select");
            
            // Limpa os controles
            for (k = 0; k < inputs.length; k++)
            {
                inputs[k].value = inputs[k].id.indexOf("hdfNaoCobrarBenef") == -1 ? "" : "false";
                inputs[k].checked = false;
            }
            
            for (k = 0; k < selects.length; k++)
                selects[k].selectedIndex = 0;
        }
    }
    
    // Recalcula os beneficiamentos para zerar o total
    calculaTotal(nomeControle, prefixoControle, callbackTotal);
}

// ------------------------------------------------------------
// Função que retorna o nome do controle a partir do validador.
// ------------------------------------------------------------
function getNomeControleFromValB(val)
{
    // Recupera o nome do controle removendo o final do nome do validador
    if (val.id.indexOf("_tbl") > -1)
        return val.id.substr(0, val.id.indexOf("_tbl"));
    else
        return val.id.substr(0, val.id.indexOf("_ctv"));
}

// -----------------------------------------------------
// Função que habilita ou desabilita o sumário de erros.
// -----------------------------------------------------
function habilitarSumarioB(nomeControle, habilitar)
{
    var textoEval = nomeControle + "_vsuSumario.showmessagebox = \"";
    textoEval += habilitar ? "True" : "False";
    eval(textoEval + "\";");
}

// --------------------------------------
// Indica se o controle pai está visível.
// --------------------------------------
function parentVisivelB(val)
{
    var nomeControle = getNomeControleFromValB(val);
    
    // Verifica se o Parent do controle está visível
    var retorno = getVar(nomeControle).ParentID == "" || document.getElementById(getVar(nomeControle).ParentID).style.display != "none";
    
    habilitarSumarioB(nomeControle, retorno);
    return retorno;
}

// ------------------------------------------------------
// Função de validação da espessura dos campos de bisotê.
// ------------------------------------------------------
function validaEspessuraBisote(val, args)
{
    var celula = val;
    while (celula.nodeName.toLowerCase() != "td")
        celula = celula.parentNode;
    
    args.IsValid = !isBenefAplicado(celula) || args.Value != "";
}

// ------------------------------------------
// Função de validação da espessura do vidro.
// ------------------------------------------
function validaEspessuraVidro(val, args)
{
    var nomeControle = getNomeControleFromValB(val);
    var lnkBenef = FindControl("lnkBenef", "a"); // André: Feito para corrigir problema ao inserir vidro na compra.
    
    if (!exibirControleBenef(nomeControle) || dadosProduto.Grupo != 1 || getVar(nomeControle).Espessura == "" ||
        (lnkBenef != null && lnkBenef.style.display == "none"))
    {
        args.IsValid = true;
        return;
    }
    
    var espessura = getEspessura(nomeControle);
    args.IsValid = espessura > 0;
}