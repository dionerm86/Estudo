<%@ Page Title="Antecipação de Boletos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadAntecipContaRec.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadAntecipContaRec" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" >

var countContas = 1; // Conta a quantidade de contas adicionados ao form
var totalContas = 0; // Calcula o total de todas as contas
var creditoCliente = 0; // Guarda quanto de crédito o cliente possui
var selContasWin;

function setContaReceber(idContaR, idPedido, pedidosLiberados, cliente, valor, vencimento, juros, multa, obs, selContasWin) {

    // Verifica se conta já foi adicionada
    var contas = FindControl("hdfIdContas", "input").value.split(',');
    for (i = 0; i < contas.length; i++) {
        if (idContaR == contas[i]) {
            selContasWin.alert("Conta já adicionada.");
            return false;
        }
    }
    
    // Adiciona item à tabela
    addItem(new Array(idPedido == "" ? pedidosLiberados : idPedido, cliente, valor, vencimento),
        new Array('Num. Pedido', 'Cliente', 'Valor', 'Vencimento'), 
        'lstContas', idContaR, 'hdfIdContas', valor, 'lblTotalContas');

    return false;
}

// Busca contas a receber de um pedido
function getContaRecFromPed() {
    var idPedido = FindControl("txtNumPedido", "input").value;

    if (idPedido == "") {
        alert("Informe o número do pedido.");
        return false;
    }


    var retorno = CadAntecipContaRec.GetContasRecFromPedido(idPedido).value;

    if (retorno == null) {
        alert("Erro de Ajax.");
        return false;
    }

    retorno = retorno.split('|');

    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        return false;
    }

    // Inclui as contas a receber do pedido informado
    var i = 1;
    for (i = 1; i < retorno.length; i++) {
        var conta = retorno[i].split(';');
        setContaReceber(conta[0], conta[1], null, conta[2], conta[3], conta[4]);
    }

    FindControl("txtNumPedido", "input").value = "";
}

function antecipar() {
    var idsContaRec = FindControl("hdfIdContas", "input").value;
    var idContaBanco = FindControl("drpContaBanco", "select").value;
    var dataRec = FindControl("ctrlDataRecebimento_txtData", "input").value;
    var taxa = FindControl("txtTaxa", "input").value;
    var juros = FindControl("txtJuros", "input").value;
    var iof = FindControl("txtIof", "input").value;
    var total = FindControl("lblTotalContas", "span").innerHTML.replace("R$", "").replace(" ", "");
    var obs = FindControl("txtObs", "textarea").value;

    taxa = taxa == "" ? 0 : taxa;

    if (idsContaRec == "") {
        alert("Inclua pelo menos uma conta a receber a ser antecipada.");
        return false;
    }

    if (idContaBanco == "") {
        alert("Informe a conta bancária na qual será feita a antecipação.");
        return false;
    }

    if (taxa == "") {
        alert("Informe a taxa da antecipação.");
        return false;
    }

    if (juros == "") {
        alert("Informe os juros da antecipação.");
        return false;
    }

    if (iof == "") {
        alert("Informe o IOF da antecipação.");
        return false;
    }

    var retorno = CadAntecipContaRec.Antecipar(idsContaRec, idContaBanco, total, taxa, juros, iof, dataRec, obs).value;

    if (retorno == null) {
        alert("Falha ao efetuar antecipação. Recarregue a página e efetue novamente a antecipação.");
        return false;
    }

    retorno = retorno.split('|');

    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        return false;
    }

    alert("Antecipação efetuada. Núm. Antecip: " + retorno[1]);
    FindControl("btnReceber", "input").disabled = true;

    return false;
}

// Limpa os dados
function limpaCampos() {
    countContas = 1;
    countItem = 1;
    totalContas = 0;
    creditoCliente = 0;
    selContasWin = null;
    
    FindControl("btnReceber", "input").disabled = false;
    FindControl("lstContas", "table").innerHTML = "";
    FindControl("hdfIdContas", "input").value = "";
    FindControl("lblTotalContas", "span").innerHTML = "0";
    FindControl("txtTaxa", "input").value = "";
    FindControl("txtJuros", "input").value = "";
    FindControl("txtIof", "input").value = "";
    FindControl("txtObs", "textarea").value = "";

    return false;
}

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <table id="tbGetFromPedido">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label9" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) getContaRecFromPed();"
                                            onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td align="center">
                                        <asp:ImageButton ID="imgGetContaRec" runat="server" ImageUrl="~/Images/Insert.gif"
                                            Width="16px" OnClientClick="getContaRecFromPed(); return false;" ToolTip="Adicionar Contas a Receber" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar Contas" OnClientClick="openWindow(600, 800, '../Utils/SelContaReceber.aspx'); return false;" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="lstContas" align="left" cellpadding="4" cellspacing="0" width="100%">
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table align="center">
                    <tr>
                        <td style="font-size: large">
                            Total das Contas:
                            <asp:Label ID="lblTotalContas" runat="server">0,00</asp:Label>
                        </td>
                        <td>
                            Taxa:
                        </td>
                        <td>
                            <asp:TextBox ID="txtTaxa" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            IOF
                        </td>
                        <td>
                            <asp:TextBox ID="txtIof" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                        <td>
                            Juros
                        </td>
                        <td>
                            <asp:TextBox ID="txtJuros" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Conta Bancária
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                DataTextField="Descricao" DataValueField="IdContaBanco">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            Data Receb.
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataRecebimento" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" ValidateEmptyText="true"/>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            Observação
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnReceber" runat="server" Text="Receber" OnClientClick="return antecipar();" />
                &nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnNovaAntecip" runat="server" OnClientClick="return limpaCampos();"
                    Text="Nova Antecipação" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdContas" runat="server" />
            </td>
        </tr>
    </table>

    <script>

    // Esconde opção de buscar contas pelo idPedido se empresa trabalha com liberação
    if ("<%= Glass.Configuracoes.PedidoConfig.LiberarPedido.ToString().ToLower() %>" == "true")
        document.getElementById("tbGetFromPedido").style.display = "none";
    
    </script>

</asp:Content>
