<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadConfirmarPedido.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadConfirmarPedido" Title="Confirmar Pedido" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrllinkquerystring"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

function setPedido(idPedido) {
    FindControl("txtNumPedido", "input").value = idPedido;
}

// Abre popup para cadastrar cheques
function queryStringCheques() {
    var idPedido = FindControl('txtNumPedido', 'input').value;
    return "?IdPedido=" + idPedido + "&origem=2";
}

function onConfirmVista(control) {
    if (!validate())
        return false;
        
    if (confirm(control.value + '?') == false)
        return false;

    if(!verificaAlteracaoPedidos())
        return false;

    control.disabled = true;
    
    var controle = <%= ctrlFormaPagto1.ClientID %>;
    var idPedido = FindControl("txtNumPedido", "input").value;
    var formasPagto = controle.FormasPagamento();
    var tiposCartao = controle.TiposCartao();
    var parcelasCredito = controle.ParcelasCartao();
    
    // Verifica se o pedido foi buscado
    if (idPedido == "") {
        alert("Busque um pedido primeiro.");
        control.disabled = false;
        return false;
    }
    
    bloquearPagina();
    
    var valores = controle.Valores();
    var contasBanco = controle.ContasBanco();
    var creditoUtilizado = controle.CreditoUtilizado();
    var isGerarCredito = controle.GerarCredito();
    var numAut = controle.NumeroConstrucard();
    var isDescontarComissao = controle.DescontarComissao();
    var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
    var numAutCartao = controle.NumeroAutCartao();
    
    // Guarda os cheques proprios ou de terceiros, de acordo com a forma de pagamento, cadastrados/selecionados, separados por |
    var chequesPagto = controle.Cheques();

    var retorno = CadConfirmarPedido.Confirmar(idPedido, formasPagto, tiposCartao, valores, contasBanco, depositoNaoIdentificado,
        isGerarCredito, creditoUtilizado, numAut, parcelasCredito, chequesPagto, isDescontarComissao, "0", numAutCartao).value;        
        
    desbloquearPagina(true);
        
    if (retorno != null)
        retorno = retorno.split('\t');
    else {
        alert('Falha ao confirmar pedido. AJAX Erro.');
        control.disabled = false;
        return false;
    }

    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        control.disabled = false;
        return false;
    }
    else {
        alert(retorno[1]);
        control.disabled = true;

        try {
            controle.Limpar();
        }
        catch (err) {
            alert(err);
        }

        redirectUrl(window.location.href);
        
        return true;
    }

    return true;
}

function onConfirmPrazo(control) {
    if (confirm(control.value + '?') == false)
        return false;

    if(!verificaAlteracaoPedidos())
        return false;

    control.disabled = true;

    var idPedido = FindControl("txtNumPedido", "input").value;

    // Verifica se o pedido foi buscado
    if (idPedido == "") {
        alert("Busque um pedido primeiro.");
        control.disabled = false;
        return false;
    }
    
    bloquearPagina();

    var verificarParcelas = FindControl("chkVerificarParcelas", "input");
    verificarParcelas = verificarParcelas != null ? verificarParcelas.checked : true;
    retorno = CadConfirmarPedido.ConfirmarPrazo(idPedido, "0", verificarParcelas).value.split('\t');
    
    desbloquearPagina(true);

    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        //control.disabled = false;
        return false;
    }
    else {
        alert(retorno[1]);
        control.disabled = true;
    }
        
    redirectUrl(window.location.href);
        
    return true;
}

function onConfirmObra(control)
{
    if (!validate())
        return false;
        
    if (confirm(control.value + '?') == false)
        return false;

    if(!verificaAlteracaoPedidos())
        return false;
    
    //control.disabled = true;
    
    var idPedido = FindControl("txtNumPedido", "input").value;
    
    // Verifica se o pedido foi buscado
    if (idPedido == "") {
        alert("Busque um pedido primeiro.");
        //control.disabled = false;
        return false;
    }
    
    bloquearPagina();
    
    if (document.getElementById("<%= pagtoObra.ClientID %>") != null)
    {
        var controle = <%= ctrlFormaPagto2.ClientID %>;
        var formasPagto = controle.FormasPagamento();
        var tiposCartao = controle.TiposCartao();
        var parcelasCredito = controle.ParcelasCartao();
        var valores = controle.Valores();
        var contasBanco = controle.ContasBanco();
        var creditoUtilizado = controle.CreditoUtilizado();
        var isGerarCredito = controle.GerarCredito();
        var numAut = controle.NumeroConstrucard();
        var isDescontarComissao = controle.DescontarComissao();
        var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
        var numAutCartao = controle.NumeroAutCartao();
        
        // Guarda os cheques proprios ou de terceiros, de acordo com a forma de pagamento, cadastrados/selecionados, separados por |
        var chequesPagto = controle.Cheques();
        
        var tipoVendaObra = FindControl("drpTipoVendaObra", "select");
        tipoVendaObra = tipoVendaObra != null ? tipoVendaObra.value : "0";
        
        var formaPagto = FindControl("drpFormaPagtoObra", "select");
        formaPagto = formaPagto != null ? formaPagto.value : "0";
        
        var tipoCartao = FindControl("drpTipoCartaoObra", "select");
        tipoCartao = tipoCartao != null ? tipoCartao.value : "0";
        
        var controle = <%= ctrlParcelas1.ClientID %>;
        var valoresParc = controle.Valores();
        var datasParc = controle.Datas();
        
        retorno = CadConfirmarPedido.ConfirmarObra(idPedido, formasPagto, tiposCartao, valores, contasBanco, depositoNaoIdentificado, 
            isGerarCredito, creditoUtilizado, numAut, parcelasCredito, chequesPagto, isDescontarComissao, formaPagto, tipoCartao, valoresParc,
            datasParc, tipoVendaObra, numAutCartao).value.split('\t');
    }
    else
        retorno = CadConfirmarPedido.ConfirmarObra(idPedido, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "0", "").value.split('\t');
    
    desbloquearPagina(true);
    
    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        //control.disabled = false;
        return false;
    }
    else {
        alert(retorno[1]);
        //control.disabled = true;
    }

    redirectUrl(window.location.href);
        
    return true;
}

function onConfirmFunc(control)
{
    if (!validate())
        return false;
        
    if (confirm(control.value + '?') == false)
        return false;

    if(!verificaAlteracaoPedidos())
        return false;
    
    //control.disabled = true;
    
    var idPedido = FindControl("txtNumPedido", "input").value;
    
    // Verifica se o pedido foi buscado
    if (idPedido == "") {
        alert("Busque um pedido primeiro.");
        //control.disabled = false;
        return false;
    }
    
    bloquearPagina();
    retorno = CadConfirmarPedido.ConfirmarFunc(idPedido).value.split('\t');
    desbloquearPagina(true);
    
    if (retorno[0] == "Erro") {
        alert(retorno[1]);
        //control.disabled = false;
        return false;
    }
    else {
        alert(retorno[1]);
        //control.disabled = true;
    }

    redirectUrl(window.location.href);
        
    return true;
}

function tipoVendaObraChange(valor)
{
    document.getElementById("pagtoObraVista").style.display = valor == 1 ? "" : "none";
    document.getElementById("pagtoObraPrazo").style.display = valor == 2 ? "" : "none";
}

function formaPagtoChange(valor)
{
    var codCartao = <%= GetCartaoCod() %>;
    FindControl("drpTipoCartaoObra", "select").style.display = valor == codCartao ? "" : "none";
    if (valor != codCartao)
        FindControl("drpTipoCartaoObra", "select").selectedIndex = 0;
}

        function openRpt() {
            openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + FindControl("txtNumPedido", "input").value + "&tipo=0");
                
            return false;
        }

        function verificaAlteracaoPedidos()
        {        
            var dataTela = FindControl("hdfDataTela", "input").value;
        
            var recalcular = CadConfirmarPedido.IsPedidosAlterados(FindControl("txtNumPedido", "input").value, dataTela);
            if (recalcular.value == "true")
            {
                FindControl("lblMensagemRecalcular", "span").innerHTML = "É necessário atualizar a tela.<br />O pedido sofreu alguma alteração após ser inserido na tela.";
                window.location.href = window.location.href;
            
                return false;
            }
        
            return true;
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Número do Pedido:&nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" onkeypress="return soNumeros(event, true, true);"
                                runat="server" onkeydown="if (isEnter(event)) cOnClick('btnBuscarPedido', null);"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNumPedido" runat="server" ControlToValidate="txtNumPedido"
                                Display="Dynamic" ErrorMessage="*" ValidationGroup="numPedido"></asp:RequiredFieldValidator>
                            <asp:ImageButton ID="imbPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelPedido.aspx?tipo=2'); return false;" />
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnBuscarPedido" runat="server" Text="Buscar Pedido" OnClick="btnBuscarPedido_Click"
                    ValidationGroup="numPedido" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvPedido" runat="server" AutoGenerateRows="False" DataSourceID="odsPedido"
                    GridLines="None" Height="50px" Width="125px">
                    <Fields>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <table cellpadding="2" cellspacing="2">
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Num. Pedido
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblNumPedido" runat="server" Text='<%# Eval("IdPedido") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Cliente
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblNomeCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Pagto. Antecipado
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblValorPagamentoAntecipado" runat="server" Text='<%# Eval("ValorPagamentoAntecipado", "{0:C}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Funcionário
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblNomeFunc" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Loja
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblNomeLoja" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Situação
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("DescrSituacaoPedido") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Tipo Venda
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblTipoVenda" runat="server" Text='<%# Eval("DescrTipoVenda") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Tipo Entrega
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblTipoEntrega" runat="server" Text='<%# Eval("DescrTipoEntrega") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Data Entrega
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblDataEntrega" runat="server" Text='<%# Eval("DataEntregaString") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Data Ped.
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Desconto
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblDesconto" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Total
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Forma Pagto.
                                        </td>
                                        <td align="left" nowrap="nowrap" colspan="5">
                                            <asp:Label ID="lblFormaPagto" runat="server" Text='<%# Eval("PagtoParcela") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" SelectMethod="GetForConfirmation"
                    TypeName="Glass.Data.DAL.PedidoDAO" OnSelected="odsPedido_Selected">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfDataTela" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXPed" DataKeyNames="IdProdPed"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Não há produtos associados à este pedido.">
                    <Columns>
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                        <asp:BoundField DataField="DescricaoProdutoComBenef" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="AlturaLista" HeaderText="Altura" SortExpression="AlturaLista" />
                        <asp:BoundField DataField="TotM" HeaderText="Tot. m²" SortExpression="TotM" />
                        <asp:BoundField DataField="TotM2Calc" HeaderText="Tot. m² calc." SortExpression="TotM2Calc" />
                        <asp:BoundField DataField="ValorVendido" HeaderText="Valor Vendido" SortExpression="ValorVendido"
                            DataFormatString="{0:C}" />
                        <asp:BoundField DataField="IdAplicacao" HeaderText="Apl." SortExpression="IdAplicacao" />
                        <asp:BoundField DataField="IdProcesso" HeaderText="Proc." SortExpression="IdProcesso" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="ValorBenef" DataFormatString="{0:C}" HeaderText="Valor benef."
                            SortExpression="ValorBenef" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="tbObra" runat="server" visible="false">
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="Obra:"></asp:Label>
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblDescrObra" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Saldo:"></asp:Label>
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblSaldoObra" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Font-Bold="True" Text="Valor do pedido:"></asp:Label>
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblValorPedido" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="hdfValorObra" runat="server" />
                            <br />
                        </td>
                    </tr>
                    <tr runat="server" id="pagtoObra" visible="false">
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="right">
                                        Tipo de venda:
                                    </td>
                                    <td align="left">
                                        <asp:DropDownList ID="drpTipoVendaObra" runat="server" onchange="tipoVendaObraChange(this.value)">
                                            <asp:ListItem Value="1">À vista</asp:ListItem>
                                            <asp:ListItem Value="2">À prazo</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <div id="pagtoObraVista">
                                <uc3:ctrlFormaPagto ID="ctrlFormaPagto2" runat="server" OnLoad="ctrlFormaPagto2_Load"
                                    TipoModel="Pedido" ExibirDataRecebimento='<%# Glass.Configuracoes.Geral.SistemaLite %>' ExibirJuros="false" ExibirRecebParcial="false"
                                    ParentID="pagtoObraVista" CalcularTroco="true" FuncaoQueryStringCheques="queryStringCheques" />
                            </div>
                            <div id="pagtoObraPrazo">
                                <table>
                                    <tr>
                                        <td>
                                            Forma de pagamento
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpFormaPagtoObra" runat="server" onchange="formaPagtoChange(this.value)"
                                                DataSourceID="odsFormaPagtoPedido" DataTextField="Descricao" DataValueField="IdFormaPagto">
                                            </asp:DropDownList>
                                            <asp:DropDownList ID="drpTipoCartaoObra" runat="server" DataSourceID="odsTipoCartaoPedido"
                                                DataTextField="Descricao" DataValueField="IdTipoCartao">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <table>
                                    <tr>
                                        <td>
                                            Número de parcelas
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpNumParcelas" runat="server" DataSourceID="odsParcelas" DataTextField="Descr"
                                                DataValueField="Id">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsParcelas" runat="server" SelectMethod="GetNumParc"
                                    TypeName="Glass.Data.Helper.DataSources">
                                </colo:VirtualObjectDataSource>
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagtoPedido" runat="server" SelectMethod="GetForPedido"
                                    TypeName="Glass.Data.DAL.FormaPagtoDAO">
                                </colo:VirtualObjectDataSource>
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCartaoPedido" runat="server" SelectMethod="GetCredito"
                                    TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
                                </colo:VirtualObjectDataSource>
                                <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="true" />
                                <br />
                                <uc4:ctrlParcelas ID="ctrlParcelas1" runat="server" NumParcelas="4" ParentID="pagtoObraPrazo"
                                    OnLoad="ctrlParcelas1_Load" />
                            </div>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnConfirmarObra" runat="server" OnClientClick="return onConfirmObra(this);"
                                Text="Confirmar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="divConfirmar" runat="server" align="center">
                    <div id="divAVista" runat="server" visible="False">
                        <table>
                            <tr>
                                <td>
                                    <uc3:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" OnLoad="ctrlFormaPagto1_Load"
                                        TipoModel="Pedido" ExibirDataRecebimento='<%# Glass.Configuracoes.Geral.SistemaLite %>' ExibirJuros="false" ExibirRecebParcial="false"
                                        CalcularTroco="true" FuncaoQueryStringCheques="queryStringCheques" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar Pedido" OnClientClick="return onConfirmVista(this);"
                                        Width="120px" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="divFunc" runat="server" visible="false">
                        <asp:Label ID="Label4" runat="server" Text="Pedido vendido para funcionário. Não será gerada movimentação no caixa.&lt;br /&gt;Confira o nome do funcionário comprador."
                            ForeColor="Blue" Font-Size="Small"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="lblNomeFuncVenda" runat="server" ForeColor="Green" Font-Size="Small"></asp:Label>
                        <br />
                        <br />
                        <asp:Button ID="btnConfirmarFunc" runat="server" Text="Confirmar Pedido" OnClientClick="return onConfirmFunc(this);"
                            Width="120px" Style="margin-top: 8px" />
                    </div>
                    <table cellpadding="0" cellspacing="0" style="width: 100%;">
                        <tr>
                            <td nowrap="nowrap" align="center">
                                <asp:Label ID="lblViewSinal" runat="server"></asp:Label>
                                <br />
                                <br />
                                <asp:Label ID="lblViewConfirm" runat="server"></asp:Label>
                                &nbsp; <a id="lnkImprimir" href="#" onclick="openRpt(); return false;">
                                    <asp:Image ID="imgImprimir" ImageUrl="~/Images/Relatorio.gif" Visible="false" runat="server" /></a>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap" align="center">
                                <br />
                                <asp:CheckBox ID="chkVerificarParcelas" runat="server" Checked="True" Text="Verificar datas das parcelas?"
                                    Visible="False" />
                                <br />
                                <asp:Button ID="btnConfirmarPrazo" runat="server" Text="Confirmar Pedido" OnClientClick="return onConfirmPrazo(this);"
                                    Width="120px" Visible="False" Style="margin-top: 8px" />
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap" align="center">
                                <asp:HiddenField ID="hdfValorCredito" runat="server" />
                                <asp:HiddenField ID="hdfIdCliente" runat="server" />
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdXPed" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                    SelectCountMethod="GetForConfirmationCount" SelectMethod="GetForConfirmation"
                                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
                                    OnSelected="odsProdXPed_Selected">
                                    <SelectParameters>
                                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        if (<%= (!String.IsNullOrEmpty(Request["IdPedido"]) && divAVista.Visible).ToString().ToLower() %>)
            <%= ctrlFormaPagto1.ClientID %>.AdicionarID(<%= Request["IdPedido"] %>);
        
        if (<%= (pagtoObra.Visible).ToString().ToLower() %>)
        {
            tipoVendaObraChange(FindControl("drpTipoVendaObra", "select").value);
            formaPagtoChange(FindControl("drpFormaPagtoObra", "select").value);
        }
        
        if (FindControl("txtNumPedido", "input").value == "")
            FindControl("txtNumPedido", "input").focus();
    </script>

</asp:Content>
