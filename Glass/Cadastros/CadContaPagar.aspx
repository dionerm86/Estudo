<%@ Page Title="Efetuar Pagamento" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadContaPagar.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadContaPagar" %>

<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc2" %>
<%@ Register src="../Controls/ctrlLimiteTexto.ascx" tagname="ctrlLimiteTexto" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

<script type="text/javascript">

var countContas = 1; // Conta a quantidade de contas adicionados ao form
var totalContas = 0; // Calcula o total de todas as contas

function buscarPagto() {
    openWindow(550, 750, "../Utils/SelPagto.aspx");
    return false;
}

function setPagto(idPagto)
{
    FindControl("txtNumPagto", "input").value = idPagto;
    loadPagto();
}

var pagtoTemCheque = false;

function loadPagto()
{
    var idPagto = FindControl("txtNumPagto", "input").value;
    if (idPagto == "")
    {
        alert("Informe o número do pagamento.");
        FindControl("txtNumPagto", "input").value = FindControl("hdfIdPagto", "input").value;
        return false;
    }
    
    var validar = CadContaPagar.ValidarPagamento(idPagto).value.split("#");
    if (validar[0] == "Erro")
    {
        alert(validar[1]);
        FindControl("txtNumPagto", "input").value = FindControl("hdfIdPagto", "input").value;
        return false;
    }

    limpar();
    
    var dadosPagto = CadContaPagar.GetDadosPagto(idPagto, "<%= ctrlFormaPagto1.ClientID %>", 
        "<%= ctrlParcelas.ClientID %>", <%= ctrlFormaPagto1.CallbackIncluirCheque %>, <%= ctrlFormaPagto1.CallbackExcluirCheque %>).value.split('#');
        
    if (dadosPagto[0] == "Erro")
    {
        alert(dadosPagto[1]);
        return false;
    }
    
    FindControl("hdfIdPagto", "input").value = FindControl("txtNumPagto", "input").value;
    
    // Carrega as contas a pagar
    eval(dadosPagto[1]);
    
    // Carrega os dados do pagamento
    utilizaCredito(dadosPagto[5], dadosPagto[6]);
    FindControl("txtDataRecebimento", "input").value = dadosPagto[7];
    FindControl("txtDesconto", "input").value = dadosPagto[8];
    FindControl("txtObs", "textarea").value = dadosPagto[9];
    FindControl("chkGerarCredito", "input").checked = dadosPagto[10] == "true";
    FindControl("chkRecebimentoParcial", "input").checked = dadosPagto[11] == "true";
    FindControl("chkRenegociar", "input").checked = dadosPagto[12] == "true";
    FindControl("drpNumParc", "select").value = dadosPagto[12] == "true" ? dadosPagto[13] : "1";
    chkRenegociarChecked(dadosPagto[12] == "true");
    
    // Carrega as formas de pagamento/parcelas
    eval(dadosPagto[4]);
    
    // Carrega os cheques
    eval(dadosPagto[2]);
    pagtoTemCheque = dadosPagto[2] != "";
    if (FindControl("chkGerarCreditoCheque", "input") != null)
        FindControl("chkGerarCreditoCheque", "input").checked = false;
    
    // Associa os cheques às contas
    var dadosCheques = dadosPagto[3].split(";");
    for (i = 0; i < dadosCheques.length; i++)
    {
        var assoc = dadosCheques[i].split(",");
        var drpCheque = document.getElementById("drpChequeProprio_" + assoc[0]);
        if (drpCheque != null)
            drpCheque.value = assoc[1];
    }
    
    <%= ctrlFormaPagto1.ClientID %>.AtualizaFormasPagamento();
}

function buscarContas() {
    openWindow(550, 750, "../Utils/SelContaPagar.aspx");
    return false;
}

function callbackUsarCredito()
{
    FindControl("lblValorPagar", "span").innerHTML = "Valor a ser Pago: " + FindControl("lblValorASerPago", "span").innerHTML;
    loadDropCheque();
}

/*
 *    Caso seja necessário alterar esta função, alterar também o método GetDadosPagto() desta página
 *    e a função setContaPagar do popup em Utils/SelContaPagar.aspx
 */
function setContaPagar(idContaPg, idCompra, idCustoFixo, idImpostoServ, idFornec, fornec, valor, dataVenc, referenteA, 
    selContasWin, idFornecFiltro, nomeFornecFiltro, multa, juros) {
    
    // Verifica se conta já foi adicionada
    var contas = FindControl("hdfIdContas", "input").value.split(',');
    for (i = 0; i < contas.length; i++) {
        if (idContaPg == contas[i]) {
            if (selContasWin != null)
                selContasWin.alert("Conta já adicionada.");

            return false;
        }
    }

    // Adiciona idConta selecionada ao hiddenfield que guarda todos os ids ja selecionados
    FindControl("hdfIdContas", "input").value += idContaPg + ",";

    // Monta tabela dinamicamente
    tabela = document.getElementById('lstContas');

    // Cria títulos para a tabela
    if (countContas == 1) {
        tabela.innerHTML = "<tr align=\"left\"><td></td>" +
            "<td style=\"font-weight: bold\">Num. Compra</td>" +
            "<td style=\"font-weight: bold\">Num. Custo Fixo</td>" +
            "<td style=\"font-weight: bold\">Num. Imposto/Serv.</td>" +
            "<td style=\"font-weight: bold\">Fornecedor/Func.</td>" +
            "<td style=\"font-weight: bold\">Referente a</td>" +
            "<td style=\"font-weight: bold\">Valor</td>" +
            "<td style=\"font-weight: bold\">Vencimento</td>" +
            "<td style=\"font-weight: bold\">Multa</td>" +
            "<td style=\"font-weight: bold\">Juros</td>" +
            "<td id=\"tdCheque\" style=\"font-weight: bold\">Cheque</td></tr>";
    }

    row = tabela.insertRow(countContas);
    row.id = "row" + row.rowIndex;
    row.setAttribute("idConta", idContaPg);

    var inputMulta = "<input name='txtMulta_" + idContaPg + "' onblur='calcTotalContas();' type='text' id='txtMulta_" + idContaPg + "' value='" + (typeof multa != "undefined" ? multa : "") 
        + "' style='width: 50px' onkeypress='return soNumeros(event, false, true);' />";

    var inputJuros = "<input name='txtJuros_" + idContaPg + "' onblur='calcTotalContas();' type='text' id='txtJuros_" + idContaPg + "' value='" + (typeof juros != "undefined" ? juros : "") 
        + "' style='width: 50px' onkeypress='return soNumeros(event, false, true);' />";

    var inputCheque = "<select name='drpChequeProprio_" + idContaPg + "' id='drpChequeProprio_" + idContaPg + "'></select>";
    
    row.innerHTML =
        "<td><a href=\"#\" onclick=\"return excluirItem(" + row.rowIndex + ");\">" +
        "<img src=\"../Images/ExcluirGrid.gif\" border=\"0\" title=\"Excluir\"/></a></td>" +
        "<td>" + (idCompra > 0 ? idCompra : "") + "</td><td>" + (idCustoFixo > 0 ? idCustoFixo : "") + 
        "</td><td>" + (idImpostoServ > 0 ? idImpostoServ : "") + "</td><td>" + fornec + "</td><td>" + referenteA + 
        "</td><td id=\"total" + row.rowIndex + "\">" + valor + "</td><td>" + dataVenc + 
        "</td><td id=\"multa\">" + inputMulta + "</td><td id=\"juros\">" + inputJuros + "</td>" + 
        "<td id=\"tdCheque\">" + inputCheque + "</td>";

    countContas++;

    // Incrementa o valor total das contas
    totalContas = parseFloat(totalContas) + parseFloat(valor.replace(".", "").replace(".", "").replace("R$", "").replace(" ", "").replace(",", "."));
    calcTotalContas();

    // Mostra dados de crédito do fornecedor
    var fornecVinculo = CadContaPagar.ObterFornecVinculado(idFornec);

    if(fornecVinculo.error != null){
        alert(fornecVinculo.error.description);
        return false;
    }

    if((nomeFornecFiltro != null && idFornecFiltro != null) && (nomeFornecFiltro != "" && idFornecFiltro != "")) {       
        fornec = nomeFornecFiltro;
        idFornec = idFornecFiltro;
    }

    utilizaCredito(fornec, idFornec);

    // Carrega as drops de cheque, se necessário
    loadDropCheque();

    return false;
}

function excluirItem(linha) {
    // Recupera o total da linha antes de ser excluída
    var totalLinha = new Number(document.getElementById('total' + linha).innerHTML.replace("R$", "").replace(" ", "").replace(".", "").replace(".", "").replace(",", ".")).toFixed(2);
    
    // Exclui conta do vetor de contas
    var contas = FindControl("hdfIdContas", "input").value.split(',');
    var contaAExcluir = document.getElementById("row" + linha).getAttribute("idConta");
    var newContas = ""; // Novo vetor de contas

    // Cria um novo vetor de contas, tirando o id da conta que foi excluido
    for (i = 0; i < contas.length; i++) {
        if (contaAExcluir != contas[i])
            newContas += contas[i] + ",";
    }
    
    // Atribui o novo vetor criado ao hidden field que guarda os ids das contas adicionadas
    FindControl("hdfIdContas", "input").value = newContas.replace(",,", ",");

    // Remove todos os campos desta linha para que os campos multa e juros não sejam buscados, 
    // caso esta conta seja adicionada novamente
    document.getElementById("row" + linha).innerHTML = "";

    // Recalcula o valor total das contas
    totalContas -= totalLinha;
    calcTotalContas();

    // Exclui o produto da tabela
    document.getElementById("row" + linha).style.display = "none";
    
    // Exibe o checkbox de geração de crédito para o fornecedor ao remover a conta
    var gerarCreditoRet = document.getElementById("<%= gerarCreditoCheque.ClientID %>");
    if (gerarCreditoRet != null)
    {
        gerarCreditoRet.style.display = pagtoTemCheque ? "" : "none";
        FindControl("hdfIdsContasRemovidas", "input").value += contaAExcluir + ",";
    }

    return false;
}

// Calcula o total das contas com os juros e as multas
function calcTotalContas() {
    var contas = FindControl("hdfIdContas", "input").value;
    FindControl("hdfTotalASerPago", "input").value = totalContas;
    var totalMultaJuros = 0;
    
    // Pega multa e juros que possam ter sido informados pelo usuário, pelo idContaPg
    var idContasPg = contas.split(',');
    for (var i = 0; i < idContasPg.length; i++) {
        
        if (idContasPg[i] == "")
            continue;
            
        var multa = find("txtMulta_" + idContasPg[i]).value;
        var juros = find("txtJuros_" + idContasPg[i]).value;
        totalMultaJuros += multa == "" ? 0 : parseFloat(multa.replace(',','.'));
        totalMultaJuros += juros == "" ? 0 : parseFloat(juros.replace(',', '.'));
    }
    
    FindControl("hdfAcrescimo", "input").value = totalMultaJuros;
    
    document.getElementById("receber").style.display = "";
    chkRenegociarChecked(FindControl("chkRenegociar", "input").checked);
}

function getContasBase(response, tipo, idCompra, idCustoFixo, idImpostoServ)
{
    if (response == null || response.value == null) {
        alert("Falha ao buscar contas d" + (tipo == "compra" ? "a " : "o ") + tipo + ". AJAX error.");
        return false;
    }
    
    response = response.value.split('#');

    if (response[0] == "Erro") {
        alert(response[1]);
        return false;
    }

    if (response[1] == '') {
        alert("Nenhuma conta a pagar encontrada para est" + (tipo == "compra" ? "a " : "e ") + tipo + ".");
        return false;
    }

    FindControl("txtObs", "textarea").value = response[4];

    var contas = response[1].split('|');
    
    for (var l = 0; l < contas.length; l++) {
        var conta = contas[l].split('\t');
        setContaPagar(conta[0], idCompra, idCustoFixo, idImpostoServ,
            conta[1], conta[2], conta[3], conta[4], conta[5], null, null, null);
    }
}

// Busca todas as contas a pagar da compra informada
function getContasByCompra(idCompra, soAVista) {
    if (idCompra == "") {
        alert("Informe o número da compra.");
        return false;
    }
    
    var response = CadContaPagar.GetContasByCompra(idCompra, soAVista);
    getContasBase(response, "compra", idCompra, 0, 0);
}

// Busca todas as contas a pagar do custo fixo informado
function getContasByCustoFixo(idCustoFixo, soAVista)
{
    if (idCustoFixo == "") {
        alert("Informe o número do custo fixo.");
        return false;
    }
    
    var response = CadContaPagar.GetContasByCustoFixo(idCustoFixo, soAVista);
    getContasBase(response, "custo fixo", 0, idCustoFixo, 0);
}

// Busca todas as contas a pagar do lançamento de imposto/serviço avulso
function getContasByImpostoServ(idImpostoServ, soAVista) {
    if (idImpostoServ == "") {
        alert("Informe o número do lançamento de imposto/serviço.");
        return false;
    }
    
    var response = CadContaPagar.GetContasByImpostoServ(idImpostoServ, soAVista);
    getContasBase(response, "lançamento de imposto/serviço", 0, 0, idImpostoServ);
}

// Se empresa trabalha com crédito de fornecedor, mostra dados referente ao crédito do mesmo
function utilizaCredito(nomeFornec, idFornec) {
    var hdfIdFornec = FindControl("hdfIdFornec", "input");
    
    if (hdfIdFornec.value == "") 
        hdfIdFornec.value = idFornec;
    
    var controlarCredito = FindControl("hdfCredito", "input").value == "true";

    // Se a empresa trabalha com crédito de fornecedor
    if (controlarCredito) {
        // Verifica se o idFornec da última conta é igual a este
        if (hdfIdFornec.value == "null") { } // Estão sendo pagas contas a pagar de fornecedores diferentes
        else if (hdfIdFornec.value != idFornec) {
            // Se entrar aqui, significa que não é para utilizar crédito, 
            // uma vez que foram adicionadas contas a pagar de fornecedores diferentes
            FindControl("hdfCredito", "input").value = "false";
            FindControl("hdfValorCredito", "input").value = "";
            FindControl("lblFornec", "span").innerHTML = "";
            hdfIdFornec.value = "null";
        }
        else
        {
            // Mostra o nome do fornecedor
            FindControl("lblFornec", "span").innerHTML = nomeFornec;

            // Busca o crédito que este fornecedor possui
            var creditoFornec = MetodosAjax.GetFornecedorCredito(idFornec).value;
            if (typeof creditoFornec != "string")
                creditoFornec = "0";

            // Busca referência ao hiddenfield que guarda quanto de crédito este fornecedor possui
            FindControl("hdfValorCredito", "input").value = creditoFornec;
        }
    }
    // Se houver contas de mais de um fornecedor, o pagto não pode ter fornecedor, 
    // independente da empresa ter crédito de fornecedor ou não.
    else if (hdfIdFornec.value != idFornec) 
        hdfIdFornec.value = "null";
    
    var chkGerarCredito = FindControl("chkGerarCredito", "input");
    var spanGerarCredito = chkGerarCredito.parentNode;
    spanGerarCredito.style.display = controlarCredito && hdfIdFornec.value == idFornec ? "" : "none";
    if (spanGerarCredito.style.display == "none")
        chkGerarCredito.checked = false;
    
    <%= ctrlFormaPagto1.ClientID %>.Calcular();
}

function esconderData()
{
    FindControl("imgDataRecebido", "input").style.display = "none";
    var i = 0;
    while (true)
    {
        var imgData = FindControl("Pagto" + (++i) + "_imgData", "input");
        if (imgData == null)
            break;
        
        imgData.style.display = "none";
    }
}

// Validações realizadas ao pagar conta
function onPagar() {
    if (!validate())
        return false;
    
    if (!confirm(FindControl("lblTitulo", "span").innerHTML + "?"))
        return false;

    var btnPagar = FindControl("btnPagar", "input");
    //btnPagar.disabled = true;
    
    var retificar = FindControl("hdfRetificarPagto", "input").value == "true";
    var idPagto = retificar ? FindControl("hdfIdPagto", "input").value : "0";
    if (retificar && idPagto == "")
    {
        alert("Selecione um pagamento para retificar.");
        //btnPagar.disabled = false;
        return false;
    }

    var idContas = FindControl("hdfIdContas", "input").value;
    var controle = <%= ctrlFormaPagto1.ClientID %>;
    var formasPagto = controle.FormasPagamento();
    var idContasBanco = controle.ContasBanco();
    var desconto = FindControl("txtDesconto", "input").value;
    var obs = FindControl("txtObs", "textarea").value;
    var gerarCredito = controle.GerarCredito();
    var pagtoParcial = controle.RecebimentoParcial();
    var creditoUtilizado = controle.CreditoUtilizado();
    var boletos = controle.NumerosBoleto();
    var tiposCartao = controle.TiposCartao();
    var numParcCartao = controle.ParcelasCartao();

    if (desconto == "")
        desconto = 0;

    // Se a conta a pagar não tiver sido escolhida
    if (idContas == "" || idContas == null || idContas == "0") {
        alert("Busque pelo menos uma conta a pagar.");
        //btnPagar.disabled = false;
        return false;
    }

    // Se o desconto tiver sido informado mas o motivo não
    if (desconto != "" && parseFloat(desconto.replace(',', '.')) > 0 && obs == "") {
        alert("Informe o motivo do desconto.");
        //btnPagar.disabled = false;
        return false;
    }
    
    bloquearPagina();
    //FindControl("loadGif", "img").style.visibility = "visible";
    
    // Guarda os cheques proprios ou de terceiros, de acordo com a forma de pagamento, cadastrados/selecionados, separados por |
    var chequesPagto = "";
    
    try {
        chequesPagto = controle.Cheques();
    } 
    catch (err) {
        alert(err);
        //FindControl("loadGif", "img").style.visibility = "hidden";
        //btnPagar.disabled = false;
        desbloquearPagina(true);
        return false;
    }

    var chequesAssoc = "";
    var dataPagto = controle.DataRecebimento();
    var datasFormasPagto = controle.DatasFormasPagamento();
    var valores = controle.Valores();
    var idFornec = FindControl("hdfIdFornec", "input").value;
    var construcard = controle.NumeroConstrucard();
    var antecipFornec = controle.AntecipacoesFornecedores();
    
    // Pega multa e juros que possam ter sido informados pelo usuário, pelo idContaPg
    var idContasPg = idContas.split(',');
    var vetJurosMulta = "";
    for (var i = 0; i < idContasPg.length; i++) {
        if (idContasPg[i] == "") {
            vetJurosMulta += "0;0|";
            continue;
        }
        
        var multa = find("txtMulta_" + idContasPg[i]).value;
        var juros = find("txtJuros_" + idContasPg[i]).value;
        vetJurosMulta += (multa == "" ? 0 : multa) + ";" + (juros == "" ? 0 : juros) + "|";
    }

    // Se for cheque próprio, busca os cheques associados às contas
    var temContaAssociadaCheque = false;
    var pagtoChequeProprio = isPagtoChequeProprio();
    
    if (pagtoChequeProprio > 0) {
        var vetContas = idContas.split(',');
        for (var i = 0; i < vetContas.length; i++) {
            var dadosCheque = FindControl("drpChequeProprio_" + vetContas[i], "select").value;
            if (vetContas[i] == "")
                continue;
            
            if (dadosCheque == "" && pagtoChequeProprio == 1)
            {
                temContaAssociadaCheque = false;
                break;
            }
            else if (dadosCheque != "")
                temContaAssociadaCheque = true;
                
            chequesAssoc += vetContas[i] + ";" + dadosCheque + "|";
        }
        
        if (!temContaAssociadaCheque)
        {
            alert("Associe os cheques às contas a pagar.");
            //FindControl("loadGif", "img").style.visibility = "hidden";
            //btnPagar.disabled = false;
            desbloquearPagina(true);
            return false;
        }
    }

    // Lixo utilizado para dar "refresh no AJAX"
    var myDate = new Date();
    
    var gerarCreditoRet = FindControl("chkGerarCreditoCheque", "input");
    var idsContasRemovidas = "";
    if (gerarCreditoRet != null)
    {
        gerarCreditoRet = gerarCreditoRet.checked;
        idsContasRemovidas = FindControl("hdfIdsContasRemovidas", "input").value;
    }
    else
        gerarCreditoRet = false;

    var retorno = CadContaPagar.Pagar(idPagto, idFornec, idContas, chequesAssoc, vetJurosMulta, dataPagto, datasFormasPagto, valores, formasPagto, tiposCartao, numParcCartao,
        chequesPagto, idContasBanco, boletos, antecipFornec, desconto, obs, gerarCredito, creditoUtilizado, pagtoParcial, construcard, retificar, gerarCreditoRet, idsContasRemovidas, 
        myDate.getMilliseconds()).value;
        
    desbloquearPagina(true);

    if (retorno == null) {
        alert("Falha ao efetuar pagamento. AJAX error.");
        //FindControl("loadGif", "img").style.visibility = "hidden";
        //btnPagar.disabled = false;
        return false;
    }

    retorno = retorno.split('\t');
        
    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        //FindControl("loadGif", "img").style.visibility = "hidden";
        //btnPagar.disabled = false;
        return false;
    }
    else {
        limpar();
        //FindControl("loadGif", "img").style.visibility = "hidden";
        alert(retorno[1]);
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Pagto&idPagto=" + retorno[2]);
        //btnPagar.disabled = false;
    }
}

function getUrlCheques(tipoPagto, urlPadrao)
{
    return tipoPagto == 2 ? "CadChequePagto.aspx" : "CadChequePagtoTerc.aspx";
}

function isPagtoChequeProprio()
{
    if (FindControl("chkRenegociar", "input").checked)
        return 0;
    
    var valores = <%= ctrlFormaPagto1.ClientID %>.Valores(false);
    var formasPagto = <%= ctrlFormaPagto1.ClientID %>.FormasPagamento(false);
    
    var temChequeProprio = false;
    var temOutraFormaPagto = false;
    
    for (i = 0; i < formasPagto.length; i++)
        if (valores[i] > 0)
            if (formasPagto[i] == 2)
            {
                temChequeProprio = true;
                if (temOutraFormaPagto)
                    break;
            }
            else
            {
                temOutraFormaPagto = true;
                if (temChequeProprio)
                    break;
            }
    
    return temChequeProprio ? (temOutraFormaPagto ? 2 : 1) : 0;
}

// Exibe/Esconde e Recarrega as drops com seleção de cheques próprio
function loadDropCheque() {
    var pagtoChequeProprio = isPagtoChequeProprio() > 0;

    // Exibe esconde coluna de associação do cheque à conta
    var rowsContaPg = document.getElementById('lstContas').getElementsByTagName("tr");
    for (var i = 0; i < rowsContaPg.length; i++)
        if (rowsContaPg[i].cells[10] != null)
            rowsContaPg[i].cells[10].style.display = pagtoChequeProprio ? "" : "none";
        
    // Se for cheque próprio, carrega as drops de cheque
    if (pagtoChequeProprio) {

        // Pega os cheques inseridos
        var cheques = "";
        var rowsCheque = document.getElementById('<%= ctrlFormaPagto1.ClientID %>_TabelaCheques').getElementsByTagName("tr");
        for (var r = 1; r < rowsCheque.length; r++) // r=1 para não considerar o título da tabela
            if (rowsCheque[r].style.display != "none" && rowsCheque[r].getAttribute("tipo") != "terceiro")
            cheques += rowsCheque[r].getAttribute("numCheque") + "/" + rowsCheque[r].getAttribute("agencia") + "/" +
                rowsCheque[r].getAttribute("conta") + "|";

        cheques = cheques.split('|');

        // Pega as drops de cheque
        var selects = document.getElementsByTagName("select");

        // Carrega as drops de cheque
        for (i = 0; i < selects.length; i++) {
            if (selects[i].name.indexOf("drpChequeProprio") >= 0) {
                selects[i].innerHTML = "";
                selects[i].options[0] = new Option("", "");

                for (j = 0; j < cheques.length - 1; j++) // -1 para não inserir o espaço em branco
                    selects[i].options[j + 1] = new Option(cheques[j], cheques[j]);
            }
        }
    }
}

function limpar() {
    countContas = 1;
    totalContas = 0;

    FindControl("hdfIdContas", "input").value = "";
    FindControl("hdfTotalASerPago", "input").value = "";
    FindControl("hdfValorCredito", "input").value = "";
    FindControl("hdfIdFornec", "input").value = "";
    FindControl("txtDesconto", "input").value = "";
    FindControl("txtObs", "textarea").value = "";
    FindControl("chkRenegociar", "input").checked = false;
    FindControl("drpNumParc", "select").value = "1";
    
    if (FindControl("hdfIdPagto", "input") != null)
        FindControl("hdfIdPagto", "input").value = "";
        
    // Esconde o checkbox de geração de crédito para o fornecedor
    var gerarCreditoRet = document.getElementById("<%= gerarCreditoCheque.ClientID %>");
    if (gerarCreditoRet != null)
    {
        gerarCreditoRet.style.display = "none";
        FindControl("chkGerarCreditoCheque", "input").checked = false;
    }
    
    <%= ctrlFormaPagto1.ClientID %>.Limpar();
    document.getElementById('lstContas').innerHTML = "";
    chkRenegociarChecked(false);
    callbackUsarCredito();
}

function chkRenegociarChecked(checked)
{
    document.getElementById("receber").style.display = "";
    document.getElementById("renegociar").style.display = checked ? "" : "none";
    
    document.getElementById("descontoTitulo").style.display = checked ? "none" : "";
    document.getElementById("descontoCampo").style.display = checked ? "none" : "";
    
    var desconto = FindControl("txtDesconto", "input");
    var usarCredito = FindControl("chkUsarCredito", "input");
    
    var descAtual = desconto.value;
    var usarCredAtual = usarCredito.checked;
    
    if (checked)
    {
        desconto.value = "";
        usarCredito.checked = false;
    }
    
    <%= ctrlFormaPagto1.ClientID %>.Calcular();
    document.getElementById("receber").style.display = checked ? "none" : "";
    
    desconto.value = descAtual;
    usarCredito.checked = usarCredAtual;
    
    if (checked)
        setParcelas();
}

function setParcelas()
{
    var nomeControleParcelas = "<%= ctrlParcelas.ClientID %>";
    Parc_visibilidadeParcelas(nomeControleParcelas);
}

function renegociar()
{
    if (!validate())
        return false;
    
    if (!confirm(FindControl("lblTitulo", "span").innerHTML + "?"))
        return false;

    var btnRenegociar = FindControl("btnRenegociar", "input");
    //btnRenegociar.disabled = true;
    
    var retificar = FindControl("hdfRetificarPagto", "input").value == "true";
    var idPagto = retificar ? FindControl("hdfIdPagto", "input").value : "0";
    if (retificar && idPagto == "")
    {
        alert("Selecione um pagamento para retificar.");
        //btnRenegociar.disabled = false;
        return false;
    }

    var idFornec = FindControl("hdfIdFornec", "input").value;
    var idContas = FindControl("hdfIdContas", "input").value;
    
    // Se a conta a pagar não tiver sido escolhida
    if (idContas == "" || idContas == null || idContas == "0") {
        alert("Busque pelo menos uma conta a pagar.");
        //btnRenegociar.disabled = false;
        return false;
    }
    
    bloquearPagina();
    //FindControl("loadGifReneg", "img").style.visibility = "visible";
    
    var controle = <%= ctrlParcelas.ClientID %>;
    var numParcelas = FindControl("drpNumParc", "select").value;
    var datas = controle.Datas();
    var valores = controle.Valores();
    
    // Lixo utilizado para dar "refresh no AJAX"
    var myDate = new Date();
    
    // Pega multa e juros que possam ter sido informados pelo usuário, pelo idContaPg
    var idContasPg = idContas.split(',');
    var vetJurosMulta = "";
    for (var i = 0; i < idContasPg.length; i++) {
        if (idContasPg[i] == "") {
            vetJurosMulta += "0;0|";
            continue;
        }
        
        var multa = find("txtMulta_" + idContasPg[i]).value;
        var juros = find("txtJuros_" + idContasPg[i]).value;
        vetJurosMulta += (multa == "" ? 0 : multa) + ";" + (juros == "" ? 0 : juros) + "|";
    }

    var retorno = CadContaPagar.Renegociar(idPagto, idFornec, idContas, numParcelas, datas, valores, vetJurosMulta, retificar, myDate.getMilliseconds()).value;
    
    desbloquearPagina(true);

    if (retorno == null) {
        alert("Falha ao efetuar pagamento. AJAX error.");
        //FindControl("loadGifReneg", "img").style.visibility = "hidden";
        //btnRenegociar.disabled = false;
        return false;
    }

    retorno = retorno.split('\t');
        
    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        //FindControl("loadGifReneg", "img").style.visibility = "hidden";
        //btnRenegociar.disabled = false;
        return false;
    }
    else {
        limpar();
        //FindControl("loadGifReneg", "img").style.visibility = "hidden";
        alert(retorno[1]);
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Pagto&idPagto=" + retorno[2]);
        //btnRenegociar.disabled = false;
    }
}

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center" style="font-family: Verdana, Arial, Helvetica, sans-serif; font-size: 10px;">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td id="buscarContas" runat="server" align="center">
                                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar Contas" OnClientClick="return buscarContas();" />
                                        <table>
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label3" runat="server" Text="Buscar por Compra:" ForeColor="#0066FF"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtNumCompra" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                                                    onkeydown="if (isEnter(event)) cOnClick('imgAddContas', null);"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:ImageButton ID="imgAddContas" runat="server" ImageUrl="~/Images/Insert.gif"
                                                                    OnClientClick="getContasByCompra(FindControl('txtNumCompra', 'input').value, false); return false;"
                                                                    ToolTip="Adicionar Contas" Width="16px" TabIndex="1" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label1" runat="server" Text="Buscar por Custo Fixo:" ForeColor="#0066FF"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtNumCustoFixo" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                                                    onkeydown="if (isEnter(event)) cOnClick('imgAddCustoFixo', null);"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:ImageButton ID="imgAddCustoFixo" runat="server" ImageUrl="~/Images/Insert.gif"
                                                                    OnClientClick="getContasByCustoFixo(FindControl('txtNumCustoFixo', 'input').value, false); return false;"
                                                                    ToolTip="Adicionar Contas" Width="16px" TabIndex="1" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label4" runat="server" Text="Buscar por Imposto/Serviço:" ForeColor="#0066FF"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtNumImpostoServ" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                                                    onkeydown="if (isEnter(event)) cOnClick('imgAddImpostoServ', null);"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:ImageButton ID="imgAddImpostoServ" runat="server" ImageUrl="~/Images/Insert.gif"
                                                                    OnClientClick="getContasByImpostoServ(FindControl('txtNumImpostoServ', 'input').value, false); return false;"
                                                                    ToolTip="Adicionar Contas" Width="16px" TabIndex="1" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="buscarPagamento" runat="server" visible="false">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label2" runat="server" Text="Buscar por Pagamento:" ForeColor="#0066FF"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtNumPagto" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                                        onkeydown="if (isEnter(event)) cOnClick('btnBuscarPagto', null);"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:ImageButton ID="imgBuscarPagto" runat="server" 
                                                        ImageUrl="~/Images/Pesquisar.gif" onclientclick="buscarPagto(); return false" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" align="center">
                                                    <asp:Button ID="btnBuscarPagto" runat="server" Text="Buscar Pagamento" 
                                                        onclientclick="loadPagto(); return false;" />
                                                    <asp:HiddenField ID="hdfIdPagto" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="hdfRetificarPagto" runat="server" Value="false" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="padding: 15px 0">
                            <table id="lstContas" cellpadding="4" cellspacing="0">
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="center">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label18" runat="server" Text="Fornecedor/Func.:" Font-Bold="True"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblFornec" runat="server"></asp:Label>
                                                </td>
                                                <td>
                                                    &nbsp;&nbsp;
                                                    <asp:Label ID="lblValorPagar" runat="server" Text="Valor a ser Pago: R$ 0,00" Font-Size="Large"></asp:Label>
                                                </td>
                                                <td id="descontoTitulo">
                                                    &nbsp;&nbsp;
                                                    <asp:Label ID="Label20" runat="server" Text="Desconto:" Font-Bold="True"></asp:Label>
                                                </td>
                                                <td id="descontoCampo">
                                                    <asp:TextBox ID="txtDesconto" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                        Width="70px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    &nbsp;&nbsp;
                                                    <asp:CheckBox ID="chkRenegociar" runat="server" Text="Renegociar" OnClick="chkRenegociarChecked(this.checked)" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <div id="receber">
                                <uc1:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" OnLoad="ctrlFormaPagto1_Load"
                                    ExibirJuros="False" ExibirNumBoleto="True" IsRecebimento="False" TextoValorReceb="Valor Pagto."
                                    FuncaoUrlCheques="getUrlCheques" CallbackUsarCredito="callbackUsarCredito" CallbackIncluirCheque="loadDropCheque"
                                    CallbackExcluirCheque="loadDropCheque" ParentID="receber" OnPreRender="ctrlFormaPagto1_PreRender"
                                    MetodoFormasPagto="GetForPagto" />
                                <script type="text/javascript">
                                    FindControl("lblTextoValorASerPago", "span").style.display = "none";
                                    FindControl("lblValorASerPago", "span").style.display = "none";
                                </script>
                                <br />
                                <table>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lblObs" runat="server" Text="Obs."></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtObs" runat="server" MaxLength="500" Rows="3" TextMode="MultiLine"
                                                Width="400px"></asp:TextBox>
                                        </td>
                                        <td>                                        
                                            <uc3:ctrlLimiteTexto ID="lmtTxtObservacao" runat="server" IdControlToValidate="txtObs" />                                        
                                        </td>
                                    </tr>
                                </table>
                                <span runat="server" id="gerarCreditoCheque" visible="false" style="display: none">
                                    <br />
                                    <asp:CheckBox ID="chkGerarCreditoCheque" runat="server" Text="Gerar crédito para os fornecedores das contas removidas" />
                                    <asp:HiddenField ID="hdfIdsContasRemovidas" runat="server" />
                                    <br />
                                </span>
                                <br />
                                <asp:Button ID="btnPagar" runat="server" OnClientClick="return onPagar();" Text="Pagar" />
                                <img id="loadGif" border="0px" height="20px" src="../Images/load.gif" style="visibility: hidden;"
                                    title="Aguarde..." width="20px" />
                                <asp:Button ID="btnLimpar" runat="server" OnClientClick="limpar(); return false;"
                                    Text="Limpar" />
                            </div>
                            <div id="renegociar">
                                <table>
                                    <tr>
                                        <td align="center">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label19" runat="server" Text="Número de Parcelas:" Font-Bold="True"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpNumParc" runat="server">
                                                            <asp:ListItem>1</asp:ListItem>
                                                            <asp:ListItem>2</asp:ListItem>
                                                            <asp:ListItem Selected="True">3</asp:ListItem>
                                                            <asp:ListItem>4</asp:ListItem>
                                                            <asp:ListItem Value="5"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <uc2:ctrlParcelas ID="ctrlParcelas" runat="server" NumParcelas="5" ParentID="renegociar"
                                                OnLoad="ctrlParcelas_Load" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                            <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="true" />
                                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagtoReneg" runat="server" SelectMethod="GetForRenegociacao"
                                                TypeName="Glass.Data.DAL.FormaPagtoDAO"></colo:VirtualObjectDataSource>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:Button ID="btnRenegociar" runat="server" Text="Renegociar" OnClientClick="return renegociar();" />
                                            <img id="loadGifReneg" border="0px" height="20px" src="../Images/load.gif" style="visibility: hidden;"
                                                title="Aguarde..." width="20px" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="divPagar" runat="server">
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hdfIdContas" runat="server" />
                <asp:HiddenField ID="hdfTotalASerPago" runat="server" />
                <asp:HiddenField ID="hdfAcrescimo" runat="server" />
                <asp:HiddenField ID="hdfCredito" runat="server" />
                <asp:HiddenField ID="hdfValorCredito" runat="server" />
                <asp:HiddenField ID="hdfIdFornec" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        chkRenegociarChecked(FindControl("chkRenegociar", "input").checked);
    </script>

</asp:Content>
