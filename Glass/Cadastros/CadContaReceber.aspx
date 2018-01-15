<%@ Page Title="Contas a Receber" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadContaReceber.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadContaReceber" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlLogCancPopup.ascx" TagName="ctrlLogCancPopup" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc6" %>
<%@ Register Src="../Controls/ctrlBoleto.ascx" TagName="ctrlBoleto" TagPrefix="uc7" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc8" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        var totalASerPago = 0;

        function openRptPedido(url)
        {    
            openWindow(600, 800, url);
        }

        function openRpt(exportarExcel, total)
        {
            var idContaR = FindControl("txtIdContaR", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idLiberarPedido = FindControl("txtNumLiberarPedido", "input").value;
            var idAcerto = FindControl("txtAcerto", "input").value;
            var idTrocaDev = FindControl("txtTrocaDev", "input").value;
            var idCli = FindControl("txtNumCli", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var lojaCliente = FindControl("chkLojaCliente", "input").checked;
            var idFunc = FindControl("drpFuncionario", "select").value;
            var tipoEntrega = FindControl("drpTipoEntrega", "select").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var dtIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dtFim = FindControl("ctrlDataFim_txtData", "input").value;
            var sort = FindControl("drpOrdenar", "select").value;
            var precoInicial = FindControl("txtPrecoInicial", "input").value;
            var precoFinal = FindControl("txtPrecoFinal", "input").value;
            var dtIniLib = FindControl("ctrlDataIniLib_txtData", "input").value;
            var dtFimLib = FindControl("ctrlDataFimLib_txtData", "input").value;
            var agrupar = FindControl("cbdAgrupar", "select").itens();
            var formaPagto = FindControl("drpFormaPagto", "select").value;
            var numeroNFe = FindControl("txtNFe", "input").value;
            var situacaoPedido = FindControl("drpSituacaoPedido", "select").value;
            var incluirParcCartao = FindControl("chkExibirParcCartao", "input").checked;
            var dataCadIni = FindControl("ctrlDataCadIni_txtData", "input").value;
            var dataCadFim = FindControl("ctrlDataCadFim_txtData", "input").value;
            var contasRenegociadas = FindControl("drpContasRenegociadas", "select").value;
            var apenasNfe = FindControl("chkApenasNfe", "input").checked;
            var contasCnab = FindControl("drpArquivoRemessa", "select").value;
            var idsRotas = FindControl("cblRota", "select").itens();
            var apenasContasAntecipadas = FindControl("drpFiltroContasAntecipadas", "select").value;
            var Obs = FindControl("txtSrcObs", "input").value;
            var tipoContasBuscar = FindControl("cblBuscarContas", "select").itens();
            var tipoContas = FindControl("drpTipoConta", "select").itens();
            var numArqRemessa = FindControl("txtNumArqRemessa", "input").value
            var refObra = FindControl("chkRefObra", "input").checked;
            var protestadas = FindControl("drpProtestadas", "select").value;
            var idContaBanco = FindControl("drpContaBanco", "select").value;
            var exibirContasVinculadas = FindControl("chkExibirContasVinculadas", "input").checked;
            var numCte = FindControl("txtNumCte", "input").value;
    
            var queryString = idPedido == "" ? "&idPedido=0" : "&idPedido=" + idPedido;
            queryString += idLiberarPedido == "" ? "&idLiberarPedido=0" : "&idLiberarPedido=" + idLiberarPedido;
            queryString += "&idTrocaDev=" + idTrocaDev;
            queryString += idCli == "" ? "&idCli=0" : "&idCli=" + idCli;
            queryString += numeroNFe == "" ? "&numeroNFe=0" : "&numeroNFe=" + numeroNFe;
            queryString += "&agrupar=" + (agrupar == "" ? "0" : agrupar) + "&dtIniLib=" + dtIniLib + "&dtFimLib=" + dtFimLib;
            queryString += "&apenasContasAntecipadas=" + apenasContasAntecipadas;
            queryString += "&Obs=" + Obs;
            queryString += "&tipoContasBuscar=" + tipoContasBuscar;
            queryString += "&tipoConta=" + tipoContas;
            queryString += "&numArqRemessa=" + numArqRemessa;
            queryString += "&refObra=" + refObra;
            queryString += "&exibirContasVinculadas=" + exibirContasVinculadas;
            queryString += "&idContaR=" + idContaR;
            queryString += "&protestadas=" + protestadas;
            queryString += "&idContaBanco=" + idContaBanco;
            queryString += "&numCte=" + numCte;
    
    
            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=ContasReceber" + (total ? "Total" : "") + 
                "&nomeCli=" + nomeCli + "&idLoja=" + idLoja + "&lojaCliente=" + lojaCliente + "&dtIni=" + dtIni + 
                "&renegociadas=" + "&dtFim=" + dtFim + "&tipoEntrega=" + tipoEntrega + 
                "&precoInicial=" + precoInicial + "&precoFinal=" + precoFinal + "&sort=" + sort + 
                "&formaPagto=" + formaPagto + "&idFunc=" + idFunc + queryString + "&exportarExcel=" + exportarExcel + 
                "&situacaoPedido=" + situacaoPedido + "&incluirParcCartao=" + incluirParcCartao + 
                "&dataCadIni=" + dataCadIni + "&dataCadFim=" + dataCadFim + "&contasRenegociadas=" + contasRenegociadas + 
                "&idAcerto=" + idAcerto + "&idsRotas=" + idsRotas + "&apenasNfe=" + apenasNfe + 
                "&contasCnab=" + contasCnab);
    
            return false;
        }

        function setContaReceber(idContaR, idPedido, idLiberarPedido, cliente, valor, dataVenc, idCli, juros, multa, obs, control){

            // Limpa o que estiver na tela, pois caso cadastre o cheque antes, o mesmo não terá referência do pedido
            limpar(); 
            
            FindControl("hdfIdContaR", "input").value = idContaR;
            FindControl("hdfIdPedido", "input").value = idPedido;
            FindControl("hdfIdLiberarPedido", "input").value = idLiberarPedido;
            FindControl("hdfIdCliente", "input").value = idCli;
            FindControl("lblCliente", "span").innerHTML = cliente;
            FindControl("lblValor", "span").innerHTML = valor;
            FindControl("lblDataVenc", "span").innerHTML = dataVenc;
            FindControl("hdfValorCredito", "input").value = MetodosAjax.GetClienteCredito(idCli).value;
    
            <%= ctrlFormaPagto1.ClientID %>.LimparIDs();
            <%= ctrlFormaPagto1.ClientID %>.AdicionarID(idContaR);
            <%= ctrlFormaPagto1.ClientID %>.AlterarJurosMinimos(parseFloat(juros.replace(',', '.')) + parseFloat(multa.replace(',', '.')));
            usarCredito('<%= ctrlFormaPagto1.ClientID %>', "callbackUsarCredito");
    
            chkRenegociarChecked(FindControl("chkRenegociar", "input"));

            var row = $(control).parent().parent();
            row.css("background-color","rgb(191, 239, 255)");
        }

// Validações realizadas ao receber conta
function onReceber() {
    if (!validate())
        return false;
    
    var control = FindControl("btnReceber", "input");
    //control.disabled = true;
    
    var controle = <%= ctrlFormaPagto1.ClientID %>;
    var idPedido = FindControl("hdfIdPedido", "input").value;
    var idConta = FindControl("hdfIdContaR", "input").value;
    var formasPagto = controle.FormasPagamento();
    var tiposCartao = controle.TiposCartao();
    var tiposBoleto = controle.TiposBoleto();
    var taxasAntecipacao = controle.TaxasAntecipacao();
    var parcelasCredito = controle.ParcelasCartao();

    // Se a conta a receber não tiver sido recebida
    if (idConta == "" || idConta == null || idConta == "0")
    {
        alert("Busque uma conta a receber primeiro");
        control.disabled = false;
        return false;
    }

    bloquearPagina();
    //FindControl("loadGif", "img").style.visibility = "visible";
    
    // Guarda os cheques proprios ou de terceiros, de acordo com a forma de pagamento, cadastrados/selecionados, separados por |
    var chequesPagto = controle.Cheques();
    var CNI = controle.CartoesNaoIdentificados();
    var cxDiario = FindControl("hdfCxDiario", "input").value;
    var dataRecebido = controle.DataRecebimento();
    var valores = controle.Valores();
    var parcial = controle.RecebimentoParcial();
    var contas = controle.ContasBanco();
    var valorConta = FindControl("lblValor", "span").innerHTML.replace("R$", "").replace(" ", "").replace('.', '').replace(',', '.');
    var juros = controle.Juros();
    var creditoUtilizado = controle.CreditoUtilizado();
    var isGerarCredito = controle.GerarCredito();
    var numAut = controle.NumeroConstrucard();
    var isDescontarComissao = controle.DescontarComissao();
    var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
    var numAutCartao = controle.NumeroAutCartao();

    retorno = CadContaReceber.Receber(idPedido, idConta, dataRecebido, formasPagto, valores, contas,
        tiposCartao, tiposBoleto, taxasAntecipacao, juros, parcial, isGerarCredito, creditoUtilizado, cxDiario,
         numAut, parcelasCredito, chequesPagto, isDescontarComissao, depositoNaoIdentificado, CNI, numAutCartao).value.split('\t');

    desbloquearPagina(true);
    
    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        //FindControl("loadGif", "img").style.visibility = "hidden";
        //control.disabled = false;
        return false;
    }
    else {
        alert(retorno[1]);
        limpar();
        //FindControl("loadGif", "img").style.visibility = "hidden";
        
        // Atualiza página
        cOnClick('imgPesq', null);

    }
}

function getCli(idCli)
{
    if (idCli.value == "")
        return;

    var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');
    
    if (retorno[0] == "Erro")
    {
        alert(retorno[1]);
        idCli.value = "";
        FindControl("txtNome", "input").value = "";
        return false;
    }
    
    FindControl("txtNome", "input").value = retorno[1];
}

// Abre popup para cadastrar cheques
function callbackCheques() {
    var idPedido = FindControl("hdfIdPedido", "input").value;
    var idLiberarPedido = FindControl("hdfIdLiberarPedido", "input").value;
    //alert(idPedido);
    return (idPedido != "" ? "?IdPedido=" + idPedido : "?IdLiberarPedido=" + idLiberarPedido) + 
        "&IdContaR=" + FindControl("hdfIdContaR", "input").value + "&origem=3";
}

// Mostra para o usuário o restante que falta ser pago e a data
function setRestante(dados) {
    dados.Data = FindControl("lblDataVenc", "span").innerHTML;
    dados.ExibirData = dados.Data != "";
}

function chkRenegociarChecked(chk)
{
    document.getElementById("tbPagto").style.display = chk.checked ? "none" : "";
    document.getElementById("tbRenegociar").style.display = !chk.checked ? "none" : "";
    
    if (chk.checked) setParcelas();
}

function setParcelas() 
{
    var nomeControleParcelas = "<%= ctrlParcelas.ClientID %>";
    Parc_visibilidadeParcelas(nomeControleParcelas);
}

function callbackUsarCredito(controle, checked)
{
    var valor = FindControl("lblValor", "span").innerHTML;
    totalASerPago = valor.replace("R$", "").replace(" ", "").replace('.', '').replace(',', '.');
    
    // Busca referência ao hiddenfield que guarda quanto de crédito este cliente possui
    var creditoCli = checked ? FindControl("hdfValorCredito", "input").value : 0;
        
    // se o cliente possuir crédito
    if (creditoCli > 0) {
        // Se o crédito do cliente for superior ao valor da conta
        if (parseFloat(creditoCli) > parseFloat(totalASerPago))
            totalASerPago = 0;
        else
            totalASerPago = (totalASerPago - creditoCli).toFixed(2);
    }
}

function renegociar(control)
{
    try {
        if (!validate())
            return false;
    }
    catch (err) { alert(err); }
    
    var numParc = parseInt(FindControl("drpNumParc", "select").value);
    var parcelas = "";

    // Salva os valores das parcelas
    for (i=0; i<numParc; i++) 
        parcelas += FindControl("ctrlParcelas_Parc" + (i + 1) + "_txtValor", "input").value + ";" +
            FindControl("ctrlParcelas_Parc" + (i + 1) + "_txtData", "input").value + ";" +
            FindControl("ctrlParcelas_Parc" + (i + 1) + "_txtJuros", "input").value+ "|";

    var idPedido = FindControl("hdfIdPedido", "input").value;
    var idConta = FindControl("hdfIdContaR", "input").value;
    var idFormaPagto = FindControl("drpPagtoReneg", "select").value;
        
    var multa = FindControl("txtMultaReneg", "input").value;

    bloquearPagina();
    //control.disabled = true;
    
    var retornoValidaCnab = CadContaReceber.TemCnabGerado(idConta);
    
    if(retornoValidaCnab.error!=null){
        alert(retornoValidaCnab.error.description);
        desbloquearPagina(true);
        return false;
    }
    
    if(retornoValidaCnab.value.toLowerCase() == "true" && !confirm('A conta a receber possui arquivo remessa gerado. Deseja continuar?')){
        desbloquearPagina(true);
        return false;
    }

    var retorno = CadContaReceber.Renegociar(idPedido, idConta, idFormaPagto, 
        numParc, parcelas, multa).value.split('\t');

    desbloquearPagina(true);
    
    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        //control.disabled = false;
        return false;
    }
    else {
        alert(retorno[1]);
        limpar();
    }
}

function limpar() {
    var btnRenegociar = FindControl("btnRenegociar", "input");
    var chkRenegociar = FindControl("chkRenegociar", "input");

    if (btnRenegociar != null) btnRenegociar.disabled = false;
    if (chkRenegociar != null) chkRenegociar.checked = false;
    FindControl("btnReceber", "input").disabled = false;
    FindControl("hdfIdContaR", "input").value = "";
    FindControl("hdfIdPedido", "input").value = "";
    FindControl("hdfIdLiberarPedido", "input").value = "";
    FindControl("hdfIdCliente", "input").value = "";
    FindControl("hdfValorCredito", "input").value = "";
    FindControl("txtNumPedido", "input").value = "";
    
    <%= ctrlFormaPagto1.ClientID %>.Limpar();

    var grdContasRows = $('#<%= grdConta.ClientID %> tr');

    for (var i = 0; i < grdContasRows.length; i++) {
        if($(grdContasRows[i]).css("background-color") != "rgb(191, 239, 255)")
            continue;
        $(grdContasRows[i]).css("background-color", "");
        break;
    }

}
         
function openRptProm(idContaR)
{
    openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=NotaPromissoria&idContaR=" + idContaR);
}

    function marcarJuridicoCartorio(idContaR, marcar){

        var msg = 'Deseja ' + (marcar ? 'marcar': 'desmarcar') + ' essa conta como jurídico/cartório';
        if(!confirm(msg))
            return false;

        var result = CadContaReceber.MarcarJuridicoCartorio(idContaR, marcar);

        if(result.error != null){
            alert(result.error.description);
            return false;
        }

        cOnClick('imgPesq', null);
    }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="lblIdContaR" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtIdContaR" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label7" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:Label ID="Label11" runat="server" Text="Liberação Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:TextBox ID="txtNumLiberarPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label10" runat="server" Text="Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc5:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap">
                            <uc5:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="lblLoja" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc6:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True" />
                            <asp:CheckBox ID="chkLojaCliente" runat="server" Text="Loja do Cliente?" AutoPostBack="true" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label21" runat="server" Text="Acerto/Acerto Parcial" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtAcerto" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:Label ID="Label26" runat="server" Text="Troca/Dev." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:TextBox ID="txtTrocaDev" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label22" runat="server" Text="Num. NF" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNFe" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="Label35" runat="server" Text="Num. CTe" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumCte" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click"
                                CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" Text="Tipo Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoEntrega" runat="server" AppendDataBoundItems="true"
                                AutoPostBack="true" DataSourceID="odsTipoEntrega" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Text="Todas" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Valor Venc." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecoInicial" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>até
                        </td>
                        <td>
                            <asp:TextBox ID="txtPrecoFinal" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label24" runat="server" Text="Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq6" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Forma Pagto." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFormaPagto" runat="server" AutoPostBack="True" DataSourceID="odsFormaPagto"
                                DataTextField="Descricao" DataValueField="IdFormaPagto">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Ordenar por:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="1">Vencimento</asp:ListItem>
                                <asp:ListItem Value="2">Cliente</asp:ListItem>
                                <asp:ListItem Value="3">Valor</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label33" runat="server" Text="Contas Antecipadas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpFiltroContasAntecipadas" runat="server" AutoPostBack="True"
                                OnSelectedIndexChanged="drpFiltroContasAntecipadas_SelectedIndexChanged">
                                <asp:ListItem Value="0">Apenas Contas Não Antecipadas</asp:ListItem>
                                <asp:ListItem Value="1">Incluir Contas Antecipadas</asp:ListItem>
                                <asp:ListItem Value="2">Apenas Contas Antecipadas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td nowrap="nowrap" style='<%= !Glass.Configuracoes.InstalacaoConfig.UsarControleEntregaInstalacao ? "display: none": "" %>'>
                            <asp:Label ID="Label23" runat="server" Text="Situação Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td style='<%= !Glass.Configuracoes.InstalacaoConfig.UsarControleEntregaInstalacao ? "display: none": "" %>'>
                            <asp:DropDownList ID="drpSituacaoPedido" runat="server" AutoPostBack="True" AppendDataBoundItems="True"
                                DataSourceID="odsSituacaoPedido" DataTextField="Descr" DataValueField="Id">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:Label ID="Label13" runat="server" Text="Período de liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <uc5:ctrlData ID="ctrlDataIniLib" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <uc5:ctrlData ID="ctrlDataFimLib" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td nowrap="nowrap" style='<%= !Glass.Configuracoes.PedidoConfig.LiberarPedido ? "display: none": "" %>'>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label28" runat="server" Text="Agrupar impressão por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdAgrupar" runat="server" OnLoad="cbdAgrupar_Load"
                                Title="Agrupar por" Width="110px">
                                <asp:ListItem Selected="True" Value="1">Cliente</asp:ListItem>
                                <asp:ListItem Value="2">Data Venc.</asp:ListItem>
                                <asp:ListItem Value="3">Data Cad.</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblRota" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblRota" runat="server" Width="110px" CheckAll="False"
                                Title="Selecione a rota" DataSourceID="odsRota" DataTextField="Descricao" DataValueField="IdRota"
                                ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="VirtualObjectDataSource1" runat="server" SelectMethod="GetAll"
                                TypeName="Glass.Data.DAL.RotaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqRota" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label27" runat="server" ForeColor="#0066FF" Text="Período Cad."></asp:Label>
                        </td>
                        <td>
                            <uc5:ctrlData ID="ctrlDataCadIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <uc5:ctrlData ID="ctrlDataCadFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="false" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getCli(FindControl('txtNumCli', 'input'));" OnClick="imgPesq_Click"
                                CausesValidation="False" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkExibirParcCartao" runat="server" Text="Exibir Parcelas Cartão"
                                ForeColor="Green" AutoPostBack="True" />
                        </td>
                    </tr>
                </table>
                <table runat="server" id="dadosCnab">
                    <tr>
                        <td align="right" nowrap="nowrap">
                            <asp:Label ID="lblArquivoRemessa2" runat="server" Text="Núm. Remessa " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="right" nowrap="nowrap">
                            <asp:TextBox ID="txtNumArqRemessa" runat="server" Width="80px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="lblArquivoRemessa" runat="server" Text="Arquivo Remessa " ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpArquivoRemessa" runat="server" OnLoad="drpArquivoRemessa_Load">
                                <asp:ListItem Value="1">Não incluir contas de arquivo de remessa</asp:ListItem>
                                <asp:ListItem Value="2">Incluir contas de arquivo de remessa</asp:ListItem>
                                <asp:ListItem Value="3">Somente contas de arquivo de remessa</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label34" runat="server" Text="Banco" ForeColor="#0066FF"></asp:Label>
                        </td>
                         <td>
                             <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                DataTextField="Nome" DataValueField="IdContaBanco" AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label37" runat="server" Text="Jurídico/Cartório" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpProtestadas" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0" Selected="True">Incluir contas em jurídico/cartório</asp:ListItem>
                                <asp:ListItem Value="1">Somente contas em jurídico/cartório</asp:ListItem>
                                <asp:ListItem Value="2">Não incluir contas em jurídico/cartório</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" /></td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label36" runat="server" Text="Contas Renegociadas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContasRenegociadas" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="1" Selected="True">Incluir contas renegociadas</asp:ListItem>
                                <asp:ListItem Value="2">Apenas contas renegociadas</asp:ListItem>
                                <asp:ListItem Value="3">Não incluir contas renegociadas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label31" runat="server" Text="Tipo Conta:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown runat="server" ID="drpTipoConta" DataSourceID="odsTiposContas"
                                DataValueField="Id" DataTextField="Descr" Title="Selecione o tipo de conta">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label29" runat="server" Text="Obs.:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtSrcObs" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label30" runat="server" Text="Buscar contas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td >
                            <sync:CheckBoxListDropDown ID="cblBuscarContas" runat="server" CheckAll="True" Title="Selecione o tipo das contas"
                                Width="200px">
                                <asp:ListItem Value="1">Contas com NF-e geradas</asp:ListItem>
                                <asp:ListItem Value="2" Style="color: red">Contas sem NF-e geradas</asp:ListItem>
                                <asp:ListItem Value="3">Outas contas</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClientClick="getCli(FindControl('txtNumCli', 'input'));"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkApenasNfe" runat="server" Text="Apenas contas com núm. NF" AutoPostBack="True" />&nbsp;
                        </td>
                        <td>
                            <asp:CheckBox ID="chkRefObra" runat="server" Text="Incluir contas referência Obra" Checked="true" AutoPostBack="True" />&nbsp;
                        </td>
                        <td>
                            <asp:CheckBox ID="chkExibirContasVinculadas" runat="server" Text="Incluir contas vinculadas" AutoPostBack="True" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdConta" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdContaR" DataSourceID="odsContasReceber" EnableViewState="false"
                    EmptyDataText="Nenhuma conta a receber encontrada." CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnRowDataBound="grdConta_RowDataBound">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" CausesValidation="false" />
                                <asp:HiddenField ID="hdfIdContaRec" runat="server" Value='<%# Eval("IdAntecipContaRec") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CausesValidation="False"
                                    Visible='<%# !(bool)Eval("IsParcelaCartao") %>'>
                         <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:HiddenField runat="server" ID="hdfIdContaR" Value='<%# Eval("IdContaR") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" CausesValidation="False" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# !(bool)Eval("IsParcelaCartao") && Eval("IdAntecipContaRec") == null %>'>
                                    <a href="#" onclick="setContaReceber('<%# Eval("IdContaR") %>', '<%# Eval("IdPedido") %>', '<%# Eval("IdLiberarPedido") %>', '<%# Eval("NomeCli").ToString().Replace("'", "") %>', '<%# Eval("ValorVec", "{0:C}") %>', '<%# Eval("DataVec", "{0:d}") %>', '<%# Eval("IdCliente") %>', '<%# Eval("Juros") %>', '<%# Eval("Multa") %>', '<%# Eval("ObsScript") %>', this);">
                                        <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                                </asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbRelatorio" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    OnClientClick='<%# "openRptPedido(\"" + Eval("RelatorioPedido") + "\"); return false" %>'
                                    Visible='<%# Eval("RelatorioPedido").ToString() != "" %>' />
                                 <asp:ImageButton ID="imgJuridicoCartorio" runat="server" ImageUrl="~/Images/hammer.png" ToolTip="Marcar conta como jurídico/cartório" OnClientClick='<%# "marcarJuridicoCartorio(" + Eval("IdContaR") + ", true); return false" %>'
                                     Visible='<%# Glass.Data.Helper.Config.PossuiPermissao(Glass.Data.Helper.Config.FuncaoMenuFinanceiro.MarcarContaJuridicoCartorio) && ((bool)Eval("Protestado")) == false %>' />
                                <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/hammerCancel.png" ToolTip="Desmarcar conta como jurídico/cartório" OnClientClick='<%# "marcarJuridicoCartorio(" + Eval("IdContaR") + ", false); return false" %>'
                                     Visible='<%# Glass.Data.Helper.Config.PossuiPermissao(Glass.Data.Helper.Config.FuncaoMenuFinanceiro.MarcarContaJuridicoCartorio) && ((bool)Eval("Juridico")) == true %>' />
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Nota.gif" OnClientClick='<%# "openRptProm(" + Eval("IdContaR") + "); return false" %>'
                                    ToolTip="Nota promissória" Visible='<%# Eval("ExibirNotaPromissoria") %>' />
                                <uc7:ctrlBoleto ID="ctrlBoleto1" runat="server" CodigoContaReceber='<%# Eval("IdContaR") != null ? Glass.Conversoes.StrParaInt(Eval("IdContaR").ToString()) : (int?)null %>'
                                    Visible='<%# Eval("BoletoVisivel") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="idContaR">
                            <EditItemTemplate>
                                <asp:Label ID="Label55" runat="server" Text='<%# Eval("idContaR") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label56" runat="server" Text='<%# Bind("idContaR") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referência" SortExpression="Referencia">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Referencia") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Referencia") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Cad." SortExpression="datacad">
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("DataCad", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Parc." SortExpression="NumParc">
                            <EditItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("NumParcString") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("NumParcString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente" SortExpression="NomeCli">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("IdNomeCli") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("IdNomeCli") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Referente a" SortExpression="DescrPlanoConta">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("DescrPlanoConta") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("DescrPlanoConta") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorVec">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("ValorVec", "{0:C}") %>'></asp:Label>
                                <asp:Label ID="Label25" runat="server" Text='<%# Eval("TextoJuros") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotal" runat="server"></asp:Label>
                                <asp:Label ID="Label25" runat="server" Text='<%# Eval("TextoJuros") %>'></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ValorVec", "{0:C}") %>'></asp:Label>
                                <asp:Label ID="Label25" runat="server" Text='<%# Eval("TextoJuros") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vencimento" SortExpression="DataVec">
                            <EditItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("DataVencPrimNeg") %>'></asp:Label>
                                <asp:Panel ID="panEditarDataVenc" runat="server" Wrap="False">
                                    <asp:TextBox ID="txtDataVenc" runat="server" onkeypress="return false;" Width="70px"
                                        Text='<%# Bind("DataVecString") %>'></asp:TextBox>
                                    <asp:ImageButton ID="imgDataVenc" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                        OnClientClick="return SelecionaData('txtDataVenc', this)" ToolTip="Alterar" />
                                </asp:Panel>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("DataVencPrimNeg") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação Pedido" SortExpression="DescrSituacaoProdPedido">
                            <EditItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("DescrSituacaoProdPedido") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("DescrSituacaoProdPedido") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num. NF" SortExpression="NumeroNFe">
                            <EditItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("NumeroNFe") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Num. Rem" SortExpression="NumeroArquivoRemessaCnab">
                            <EditItemTemplate>
                                <asp:Label ID="Label77" runat="server" Text='<%# Bind("NumeroArquivoRemessaCnab") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label76" runat="server" Text='<%# Bind("NumeroArquivoRemessaCnab") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nosso Número" SortExpression="NossoNumero">
                            <EditItemTemplate>
                                <asp:Label ID="Label78" runat="server" Text='<%# Eval("NossoNumero") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label79" runat="server" Text='<%# Eval("NossoNumero") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Banco" SortExpression="Banco">
                            <EditItemTemplate>
                                <asp:Label ID="Label80" runat="server" Text='<%# Eval("Banco") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label81" runat="server" Text='<%# Eval("Banco") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Obs" SortExpression="Obs">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                    Width="200px"></asp:TextBox>
                                <asp:HiddenField ID="hdfIdContaR" runat="server" Value='<%# Bind("IdContaR") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                                <asp:Label ID="Label32" runat="server" Text='<%# Bind("ObsDescAcresc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescricaoContaContabil">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("DescricaoContaContabil") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("DescricaoContaContabil") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc4:ctrlLogCancPopup ID="ctrlLogCancPopup2" runat="server" Tabela="ContasReceber"
                                    IdRegistro='<%# Eval("IdContaR") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc8:ctrlLogPopup ID="ctrlLogContasReceber" runat="server" Tabela="ContasReceber" IdRegistro='<%# Eval("IdContaR") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false, false); return false;"
                    CausesValidation="False"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true, false); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkTotal" runat="server" OnClientClick="openRpt(true, true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Relatório de totais</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="divReceber" runat="server">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label15" runat="server" Text="Cliente:" Font-Bold="True"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblCliente" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="Label2" runat="server" Text="Valor:" Font-Bold="True"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblValor" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="Label3" runat="server" Text="Data Venc.:" Font-Bold="True"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblDataVenc" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:CheckBox ID="chkRenegociar" runat="server" Text="Renegociar" onclick="chkRenegociarChecked(this);" />
                            </td>
                        </tr>
                    </table>
                    <table id="tbRenegociar" style="display: none;">
                        <tr>
                            <td>
                                <table align="center">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label19" runat="server" Text="Número de Parcelas:" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpNumParc" runat="server">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label20" runat="server" Text="Forma Pagto.:" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpPagtoReneg" runat="server" DataSourceID="odsFormaPagtoReneg"
                                                DataTextField="Descricao" DataValueField="IdFormaPagto">
                                            </asp:DropDownList>
                                        </td>
                                        <%--<td>
                                            Juros:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtJurosReneg" runat="server" Width="60px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                        </td>--%>
                                        <td>Multa:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtMultaReneg" runat="server" Width="60px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <uc2:ctrlParcelas ID="ctrlParcelas" runat="server" NumParcelas="5" ParentID="tbRenegociar"
                                    OnLoad="ctrlParcelas_Load" ExibirJurosPorParcela="true" /> 
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;
                                <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="true" />
                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagtoReneg" runat="server"
                                    SelectMethod="GetForRenegociacao" TypeName="Glass.Data.DAL.FormaPagtoDAO">
                                </colo:VirtualObjectDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnRenegociar" runat="server" Text="Renegociar" OnClientClick="return renegociar(this);" />
                            </td>
                        </tr>
                    </table>
                    <table cellpadding="2" cellspacing="1" border="0px">
                        <tr>
                            <td>
                                <table id="tbPagto">
                                    <tr>
                                        <td>
                                            <uc3:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" FuncaoQueryStringCheques="callbackCheques"
                                                ParentID="tbPagto" TipoModel="ContasReceber" OnLoad="ctrlFormaPagto1_Load" FuncaoDadosRecebParcial="setRestante"
                                                CallbackUsarCredito="callbackUsarCredito" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="10">
                                            <asp:Button ID="btnReceber" runat="server" OnClientClick="return onReceber();" Text="Receber" />
                                            <img id="loadGif" src="../Images/load.gif" border="0px" title="Aguarde..." width="20px"
                                                height="20px" style="visibility: hidden;" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hdfIdCliente" runat="server" />
                <asp:HiddenField ID="hdfIdFormaPagto" runat="server" />
                <asp:HiddenField ID="hdfValorCredito" runat="server" />
                <asp:HiddenField ID="hdfIdContaR" runat="server" />
                <asp:HiddenField ID="hdfIdPedido" runat="server" />
                <asp:HiddenField ID="hdfIdLiberarPedido" runat="server" />
                <asp:HiddenField ID="hdfCxDiario" runat="server" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContasReceber" runat="server" 
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetNaoRecebidasCount"
                    SelectMethod="GetNaoRecebidas" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ContasReceberDAO" DataObjectTypeName="Glass.Data.Model.ContasReceber"
                    UpdateMethod="AtualizaObsDataVec" ConflictDetection="OverwriteChanges" EnableViewState="false"
                    SkinID="">
                    <SelectParameters>
                    <asp:ControlParameter ControlID="txtIdContaR" Name="idContaR" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumLiberarPedido" Name="idLiberarPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtAcerto" Name="idAcerto" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtTrocaDev" Name="idTrocaDevolucao" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNFe" Name="numeroNFe" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpFuncionario" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipoEntrega" Name="tipoEntrega" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dtIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dtFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniLib" Name="dtIniLib" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimLib" Name="dtFimLib" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadIni" Name="dataCadIni" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataCadFim" Name="dataCadFim" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtPrecoInicial" Name="precoInicial" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="txtPrecoFinal" Name="precoFinal" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="drpFormaPagto" Name="idFormaPagto" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacaoPedido" Name="situacaoPedido" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkExibirParcCartao" Name="incluirParcCartao" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpContasRenegociadas" Name="contasRenegociadas" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="chkApenasNfe" Name="apenasNf" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="drpFiltroContasAntecipadas" Name="filtroContasAntecipadas"
                            PropertyName="SelectedValue" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="sort" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpArquivoRemessa" Name="contasCnab" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="cblRota" Name="idsRotas" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtSrcObs" Name="obs" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="cblBuscarContas" Name="tipoContasBuscar" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpTipoConta" Name="tipoContaContabil" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkLojaCliente" Name="lojaCliente" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="txtNumArqRemessa" Name="numArqRemessa" PropertyName="text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="chkRefObra" Name="refObra" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="drpProtestadas" Name="protestadas" PropertyName="SelectedValue"/>
                        <asp:ControlParameter ControlID="drpContaBanco" Name="idContaBanco" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="chkExibirContasVinculadas" Name="exibirContasVinculadas"
                            PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="txtNumCte" Name="numCte" PropertyName="Text"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEntrega" runat="server"
                    SelectMethod="GetTipoEntrega" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForConsultaContasReceber"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacaoPedido" runat="server"
                    SelectMethod="GetSituacaoProducao" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.RotaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsTiposContas" runat="server" TypeName="Glass.Data.DAL.ContasReceberDAO"
                    SelectMethod="ObtemTiposContas">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="ObterBancoAgrupado"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        var isGerarCredito = FindControl("chkGerarCredito", "input");
        if (isGerarCredito != null)
            isGerarCredito.checked = false;

        var chkParcial = FindControl("chkParcial", "input");
        if (chkParcial != null)
            chkParcial.checked = false;
    </script>

</asp:Content>
