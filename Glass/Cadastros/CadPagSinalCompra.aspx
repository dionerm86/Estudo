<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadPagSinalCompra.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadPagSinalCompra" Title="Pagamento de Sinal"%>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrllinkquerystring"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlLimiteTexto.ascx" TagName="ctrlLimiteTexto" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc6" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc7" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
    
     var buscandoFornec = false;
    
    function addCompra()
    {
        if (buscandoFornec)
            return;
        
        var idCompra = FindControl("txtNumCompra", "input").value;
        if (Trim(idCompra) == "")
        {
            alert("Selecione uma compra para continuar.");
            FindControl("txtNumCompra", "input").value = "";
            FindControl("txtNumCompra", "input").focus();
            return;
        }
        
        var idFornec = FindControl("hdfIdFornec", "input").value;
        var validaCompra = CadPagSinalCompra.ValidaCompra(idCompra, idFornec).value.split('|');
        
        if (validaCompra[0] == "false")
        {
            alert(validaCompra[1]);
            FindControl("txtNumCompra", "input").value = "";
            FindControl("txtNumCompra", "input").focus();
            return;
        }
        
        var idsCompras = FindControl("hdfBuscarIdsCompras", "input").value.split(',');
        var novosIds = new Array();
        
        novosIds.push(idCompra);
        for (i = 0; i < idsCompras.length; i++)
            if (idsCompras[i] != idCompra && idsCompras[i].length > 0)
                novosIds.push(idsCompras[i]);
        
        FindControl("hdfBuscarIdsCompras", "input").value = novosIds.join(',');
        FindControl("txtNumCompra", "input").value = "";
        cOnClick("btnBuscarCompras", null);
    }
    
    function removeCompra(idCompra)
    {
        var idsCompras = FindControl("hdfBuscarIdsCompras", "input").value.split(',');
        var novosIds = new Array();
        
        for (i = 0; i < idsCompras.length; i++)
            if (idsCompras[i] != idCompra && idsCompras[i].length > 0)
                novosIds.push(idsCompras[i]);
        
        FindControl("hdfIdsComprasRem", "input").value = idCompra + ",";
        
        FindControl("hdfBuscarIdsCompras", "input").value = novosIds.join(',');
        cOnClick("btnBuscarCompras", null);
    }
    
       function exibirProdutos(botao, idCompra)
    {
        var linha = document.getElementById("produtos_" + idCompra);
        var exibir = linha.style.display == "none";
        linha.style.display = exibir ? "" : "none";
        botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
        botao.title = (exibir ? "Esconder" : "Exibir") + " produtos";
    }
    
     function getFornec(idFornec) {
        if (idFornec.value == "")
            return false;

        var idFornecedor = idFornec.value;

        var retorno = MetodosAjax.GetFornec(idFornecedor).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            idFornec.value = "";
            FindControl("txtNomeFornec", "input").value = "";
            return false;
        }
        
        FindControl("txtNomeFornec", "input").value = retorno[1];
        FindControl("txtNumFornec", "input").value = idFornecedor;
        FindControl("hdfIdFornec", "input").value = idFornecedor;

        return false;
    }
    
     function buscarCompras()
    {
        var idFornec = FindControl("txtNumFornec", "input").value;
        var nomeFornec = FindControl("txtNomeFornec", "input").value;
        if (idFornec == "" && nomeFornec == "")
            return;
        
        buscandoFornec = true;
        
        if (idFornec == "")
            idFornec = "0";
            
        var idsComprasRem = FindControl("hdfIdsComprasRem", "input").value;
        
     
        FindControl("hdfBuscarIdsCompras", "input").value = CadPagSinalCompra.GetComprasByFornec(idFornec, nomeFornec, idsComprasRem).value;
    }
    
    function onPagar(control) {
        if (!validate())
            return false;
        
        if (!confirm('Pagar o sinal da compra?'))
            return false;

        control.disabled = true;
        
        var idsCompras = FindControl("hdfBuscarIdsCompras", "input").value;
        
        // Verifica se as compras foram buscadas
        if (idsCompras == "") {
            alert("Busque uma compra primeiro.");
            control.disabled = false;
            return false;
        }
        
        var controle = <%= ctrlFormaPagto1.ClientID %>;
        
        var isGerarCredito = controle.GerarCredito();
        var creditoUtilizado = controle.CreditoUtilizado();
        
        var totalCompras = parseFloat(CadPagSinalCompra.GetTotalCompras(idsCompras).value.replace(",", "."));
        
        var totalPago = creditoUtilizado;
            var pagtos = controle.Valores(false);
            for (i = 0; i < pagtos.length; i++)
                totalPago += pagtos[i];
        
        if (!isGerarCredito && totalPago > totalCompras)
        {
            alert("O valor pago é maior que o valor a pagar!")
            control.disabled = false;
            return false;
        }
        
        if(totalPago < totalCompras)
        {
            alert("O valor pago é menor que o valor a pagar!")
            control.disabled = false;
            return false;
        }
        
        var boletos = controle.NumerosBoleto();
        var formasPagto = controle.FormasPagamento();
        var tiposCartao = controle.TiposCartao();
        var parcCartao = controle.ParcelasCartao();
        var dataReceb = controle.DataRecebimento();
        var cxDiario = FindControl("hdfCxDiario", "input").value;
        var valores = controle.Valores();
        var contasBanco = controle.ContasBanco();
        var datasPagto = controle.DatasFormasPagamento()
        var obs = FindControl("txtObs", "textarea").value;
        
        // Guarda os cheques proprios ou de terceiros, de acordo com a forma de pagamento, cadastrados/selecionados, separados por |
        var chequesPagto = controle.Cheques();
          
        var idFornec = FindControl("hdfIdFornec", "input").value;
        
        //pagar-------------
        var retorno = CadPagSinalCompra.PagarSinalCompra(idsCompras, valores, formasPagto, contasBanco, tiposCartao,
            parcCartao, datasPagto, obs, boletos, isGerarCredito, creditoUtilizado, chequesPagto, idFornec).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            control.disabled = false;
        }
        else {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=SinalCompra&idSinalCompra=" + retorno[1]);
            window.location = "../Listas/LstCompras.aspx";
        }
        
        return false;
    }
    
    function getUrlCheques(tipoPagto, urlPadrao)
    {
        return tipoPagto == 2 ? "CadChequePagto.aspx" : "CadChequePagtoTerc.aspx";
    }
    


function isPagtoChequeProprio()
{    
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

// Se empresa trabalha com crédito de fornecedor, mostra dados referente ao crédito do mesmo
function utilizaCredito(nomeFornec, idFornec) {
    var hdfIdFornec = FindControl("hdfIdFornec", "input");
    
    if (hdfIdFornec.value == "") 
        hdfIdFornec.value = idFornec;
    
    // Se a empresa trabalha com crédito de fornecedor
    if (FindControl("hdfCredito", "input").value == "true") {
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
    spanGerarCredito.style.display = hdfIdFornec.value == idFornec ? "" : "none";
    if (spanGerarCredito.style.display == "none")
        chkGerarCredito.checked = false;
    
    <%= ctrlFormaPagto1.ClientID %>.Calcular();
}
    
    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Num. Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCompra" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) cOnClick('imbAdd', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imbAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addCompra(); return false;" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="60px" onkeydown="if (isEnter(event)) getFornec(this);"
                                onkeypress="return soNumeros(event, true, true);" onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeFornecedor" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('btnBuscarCompras', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelFornec.aspx'); return false;"> <img border="0" src="../Images/Pesquisar.gif" /> </asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnBuscarCompras" runat="server" Text="Buscar Compras" OnClientClick="buscarCompras()"
                    CausesValidation="False" OnClick="btnBuscarCompras_Click" />
                <asp:HiddenField ID="hdfBuscarIdsCompras" runat="server" />
                <asp:HiddenField ID="hdfComprasAbertas" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdCompra" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsCompras" DataKeyNames="IdCompra" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhuma compra encontrada."
                    OnDataBound="grdCompra_DataBound">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick='<%# "removeCompra(" + Eval("IdCompra") + "); return false;" %>'
                                    ToolTip="Remover compra" />
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirProdutos(this, " + Eval("IdCompra") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir compras" />
                                <asp:HiddenField ID="hdfIdCompra" runat="server" Value='<%# Eval("IdCompra") %>' />
                                <asp:HiddenField ID="hdfIdFornecedor" runat="server" Value='<%# Eval("IdFornec") %>' />
                                <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Eval("Total") %>' />
                                <asp:HiddenField ID="hdfValorEntrada" runat="server" Value='<%# Eval("ValorEntrada") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdCompra" HeaderText="Num" SortExpression="IdCompra" />
                        <asp:BoundField DataField="IdNomeFornec" HeaderText="Fornecedor" SortExpression="IdNomeFornec" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="FuncionÃ¡rio" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="ValorEntrada" DataFormatString="{0:c}" HeaderText="Valor Entrada"
                            SortExpression="ValorEntrada" />
                        <asp:BoundField DataField="Desconto" HeaderText="Desconto" SortExpression="Desconto" />
                        <asp:BoundField DataField="Icms" DataFormatString="{0:c}" HeaderText="Valor ICMS"
                            SortExpression="Icms" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Pagto" SortExpression="DescrTipoCompra">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("DescrTipoCompra").ToString() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrTipoCompra") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data Compra"
                            SortExpression="DataCad" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr>
                                <tr id="produtos_<%# Eval("IdCompra") %>" style="display: none" class="<%= GetAlternateClass() %>">
                                    <td>
                                    </td>
                                    <td colspan="11" style="padding: 0px">
                                        <asp:HiddenField ID="hdfIdCompraProdutos" runat="server" Value='<%# Eval("IdCompra") %>' />
                                        <asp:GridView ID="grdProdutosCompra" runat="server" AutoGenerateColumns="False" CellPadding="3"
                                            DataKeyNames="IdProdCompra" DataSourceID="odsProdutosCompra" GridLines="None"
                                            Width="100%">
                                            <Columns>
                                                <asp:BoundField DataField="CodInterno" HeaderText="Cod." SortExpression="CodInterno">
                                                    <ItemStyle Wrap="False" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                                                        <asp:Label ID="Label11" runat="server" Text='<%# Eval("DescrBeneficiamentos") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura"></asp:BoundField>
                                                <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura">
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label14" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label5" runat="server" Text='<%# Eval("Qtde") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("Qtde") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle Font-Bold="true" />
                                            <HeaderStyle HorizontalAlign="Left" />
                                        </asp:GridView>
                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosCompra" runat="server" SelectMethod="GetByCompra"
                                            TypeName="Glass.Data.DAL.ProdutosCompraDAO">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdCompraProdutos" Name="idCompra" PropertyName="Value"
                                                    Type="UInt32" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
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
                <asp:Label ID="lblComprasRem" runat="server"></asp:Label>
                <asp:ImageButton ID="imgLimparRemovidos" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                    OnClick="imgLimparRemovidos_Click" Visible="False" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfValorCredito" runat="server" />
                <asp:HiddenField ID="hdfIdFornec" runat="server" />
                <asp:HiddenField ID="hdfTotalASerPago" runat="server" />
                <asp:HiddenField ID="hdfCxDiario" runat="server" />
                <asp:HiddenField ID="hdfIdsComprasRem" runat="server" />
                <asp:HiddenField ID="hdfAcrescimo" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCompras" runat="server" SelectMethod="GetForPagarSinal"
                    TypeName="Glass.Data.DAL.CompraDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfBuscarIdsCompras" Name="idsCompras" PropertyName="Value"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="receber" runat="server" visible="false">
                    <uc6:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" OnLoad="ctrlFormaPagto1_Load" 
                    TextoValorReceb="Valor Pagto." OnPreRender="ctrlFormaPagto1_PreRender" ExibirJuros="false"
                    ExibirRecebParcial="false" MetodoFormasPagto="GetForPagto" FuncaoUrlCheques="getUrlCheques"
                    IsRecebimento="false"/>
                    <br />
                    <table>
                        <tr>
                            <td align="left">
                                <asp:Label ID="lblObs" runat="server" Text="Obs."></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtObs" runat="server" MaxLength="500" Rows="2" TextMode="MultiLine"
                                    Width="300px"></asp:TextBox>
                            </td>
                            <td>
                                <uc5:ctrlLimiteTexto ID="lmtTxtObservacao" runat="server" IdControlToValidate="txtObs" />
                            </td>
                        </tr>
                    </table>
                    <span runat="server" id="gerarCreditoCheque" visible="false" style="display: none">
                        <br />
                        <asp:CheckBox ID="chkGerarCreditoCheque" runat="server" Text="Gerar crÃ©dito para os fornecedores das contas removidas" />
                        <asp:HiddenField ID="hdfIdsContasRemovidas" runat="server" />
                        <br />
                    </span>
                    <br />
                    <asp:Button ID="btnPagar" runat="server" OnClientClick="return onPagar(this);" Text="Pagar" />
                    <img id="loadGif" border="0px" height="20px" src="../Images/load.gif" style="visibility: hidden;"
                        title="Aguarde..." width="20px" />
                    <asp:Button ID="btnLimpar" runat="server" OnClientClick="limpar(); return false;"
                        Text="Limpar" />
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
