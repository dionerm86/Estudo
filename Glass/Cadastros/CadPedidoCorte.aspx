<%@ Page Title="Controle de Produção" Language="C#" AutoEventWireup="true" CodeBehind="CadPedidoCorte.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadPedidoCorte" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/Corte.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/dhtmlgoodies_calendar.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/dhtmlgoodies_calendar.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
</head>
<body>
<script type="text/javascript">

function openRpt()
{
    var idPedido = FindControl("txtNumPedido", "input").value;
    var dtIni = FindControl("txtDataIni", "input").value;
    var dtFim = FindControl("txtDataFim", "input").value;
    var situacao = <%= Request["Sit"] %>;
        
    if (idPedido == "")
        idPedido = 0;
        
    var queryString = "&IdPedido=" + idPedido + "&dtIni=" + dtIni + "&dtFim=" + dtFim + "&situacao=" + situacao;
    
    openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ListaProducao" + queryString);
    return false;
}

// Busca pedido para confirmação ao dar enter na textBox do numero do pedido
function pesqPed(evento){
    if (evento.keyCode == 13)
        WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("lnkPesqPed", "", true, "", "", false, true));
}

</script>

    <form id="form1" runat="server">
    <div>    
        <table class="main" align="center" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <table width="100%" cellpadding="0" cellspacing="2">
                        <tr>
                            <td class="tdTitulo">
                                <table align="center">
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td class="title">
                                            Controle de Produção</td>
                                        <td align="right" width="10%">
                                            <asp:LinkButton ID="lnkLogout" runat="server" onclick="lnkLgout_Click" 
                                                CausesValidation="False">
                                             <img border="0" src="../Images/Logout.gif" /></asp:LinkButton>
                                        &nbsp;</td>
                                    </tr>
                                    
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; width: 100%;">
                    <table class="divisor" cellpadding="0" cellspacing="2">
                        <tr>
                            <td class="tdConfirmacao">
                                <table id="tbSituacoes" runat="server" class="situacoes" align="center">
                                    <tr>
                                        <td align="right">
                                            <asp:Button ID="btnProducao" runat="server" class="botao" 
                                                Text="Pedidos Produção" onclick="btnProducao_Click" />
                                            <br />
                                            <br />
                                            <asp:LinkButton ID="lnkConsProducao" runat="server" 
                                                onclick="lnkConsProducao_Click">Consultar Pedidos em Produção</asp:LinkButton>
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Button ID="btnPronto" runat="server" class="botao" 
                                                Text="Pedidos Prontos" onclick="btnPronto_Click" />
                                            <br />
                                            <br />
                                            <asp:LinkButton ID="lnkConsProntos" runat="server" 
                                                onclick="lnkConsProntos_Click">Consultar Pedidos Prontos</asp:LinkButton>
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            <asp:Button ID="btnEntregue" runat="server" class="botao" 
                                                Text="Pedidos Entregues" onclick="btnEntregue_Click" />
                                            <br />
                                            <br />
                                            <asp:LinkButton ID="lnkConsEntregue" runat="server" 
                                                onclick="lnkConsEntregue_Click">Consultar Pedidos Entregues</asp:LinkButton>
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                    </table>

    <table id="tbConfirmar" runat="server" style="width: 100%;" visible="false">
    <tr>
        <td class="subtitle">
            <asp:Label ID="lblTitulo" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;</td>
    </tr>
    <tr>
        <td align="center">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Número do Pedido:" 
                            Font-Size="Medium"></asp:Label>&nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="txtNumPedidoConf" 
                            onkeypress="return soNumeros(event, true, true);" 
                            onkeydown="pesqPed(event);" runat="server" 
                            Font-Size="Medium" Width="80px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="valNumPedido" runat="server" 
                            ControlToValidate="txtNumPedidoConf" ErrorMessage="*"></asp:RequiredFieldValidator>&nbsp;
                        <asp:LinkButton ID="lnkPesqPed" runat="server" onclick="lnkPesqPed_Click">
                            <img src="../Images/Pesquisar.gif" border="0"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;</td>
    </tr>
    <tr>
        <td align="center" style="height: 100%">
                            <asp:DetailsView ID="dtvPedido" runat="server" AutoGenerateRows="False" 
                                DataSourceID="odsPedido" GridLines="None" 
                                Height="50px" Width="125px">
                                <Fields>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <table cellpadding="2" cellspacing="2">
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Num. Pedido</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNumPedido" runat="server" Text='<%# Eval("IdPedido") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Cliente</td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:Label ID="lblNomeCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Funcionário</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeFunc" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Loja</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeLoja" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Situação</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblSituacao" runat="server" 
                                                            Text='<%# Eval("DescrSituacaoPedido") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Venda</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoVenda" runat="server" 
                                                            Text='<%# Eval("DescrTipoVenda") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Entrega</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoEntrega" runat="server" 
                                                            Text='<%# Eval("DescrTipoEntrega") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data Entrega</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDataEntrega" runat="server" 
                                                            Text='<%# Eval("DataEntregaString") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data Ped.</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Desconto</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDesconto" runat="server" 
                                                            Text='<%# Eval("Desconto", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Total</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Forma Pagto.</td>
                                                    <td align="left" nowrap="nowrap" colspan="5">
                                                        <asp:Label ID="lblFormaPagto" runat="server" Text='<%# Eval("PagtoParcela") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" 
            SelectMethod="GetForCorte" 
            TypeName="Glass.Data.DAL.PedidoDAO" onselected="odsPedido_Selected">
            <SelectParameters>
                <asp:ControlParameter ControlID="hdfIdPedido" Name="idPedido" 
                    PropertyName="Value" Type="UInt32" />
                <asp:ControlParameter ControlID="hdfSitConf" Name="situacao" 
                    PropertyName="Value" Type="Int32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
                </td>
    </tr>
    <tr>
        <td>
            &nbsp;</td>
    </tr>
    
    <tr>
        <td align="center" style="height: 100%">
                <asp:GridView ID="grdProdutos" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" 
                    DataSourceID="odsProdXPed" ForeColor="#333333" GridLines="None" 
                    DataKeyNames="IdProdPed">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                    <Columns>
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" 
                            SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura" 
                            SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" 
                            SortExpression="Largura" />
                        <asp:BoundField DataField="TotM" HeaderText="Total M2" SortExpression="TotM" />
                        <asp:BoundField DataField="ValorVendido" HeaderText="Valor Vendido" 
                            SortExpression="ValorVendido" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" 
                            DataFormatString="{0:C}" />
                    </Columns>
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#999999" />
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdXPed" runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetForCorteCount" 
                    SelectMethod="GetForCorte" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.ProdutosPedidoDAO" 
                    onselected="odsProdXPed_Selected">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdPedido" Name="idPedido" 
                            PropertyName="Value" Type="UInt32" />
                        <asp:ControlParameter ControlID="hdfSitConf" Name="situacao" 
                            PropertyName="Value" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfSitConf" runat="server" />
                <asp:HiddenField ID="hdfIdPedido" runat="server" />
            </td>
    </tr>
    
    <tr>
        <td align="center">
            </td>
    </tr>
    
    <tr>
        <td align="center" style="vertical-align: bottom;">            
            <table cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr>
                    <td nowrap="nowrap" align="center">
                        <br />
                        <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" Width="120px" 
                            Visible="False" onclick="btnConfirmar_Click" Font-Size="Medium" />&nbsp;<asp:Button 
                            ID="btnCancelar" runat="server" Text="Cancelar" Width="120px" 
                            onclick="btnCancelar_Click" CausesValidation="False" Font-Size="Medium" /></td>
                </tr>
            </table>  
        </td>
    </tr>
</table>
                            </td>
                            <td class="tdFiltro">
                                <table>
                                    <tr>
                                        <td class="subtitle">                                            
                                            <asp:Label ID="lblPedidos" runat="server"></asp:Label>                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="Label10" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>&nbsp;</td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataIni" runat="server" onkeypress="return false;" 
                                                            Width="80px"></asp:TextBox>
                                                        <asp:ImageButton ID="imgDataRecebido0" runat="server" ImageAlign="AbsMiddle" 
                                                            ImageUrl="~/Images/calendario.gif" ToolTip="Alterar"
                                                            OnClientClick="return SelecionaData('txtDataIni', this);" /></td>
                                                    <td align="left" nowrap="nowrap">
                                                        &nbsp;<asp:TextBox ID="txtDataFim" runat="server" onkeypress="return false;" 
                                                            Width="80px"></asp:TextBox>
                                                        <asp:ImageButton ID="imgDataRecebido1" runat="server" ImageAlign="AbsMiddle" 
                                                            ImageUrl="~/Images/calendario.gif" ToolTip="Alterar"
                                                            OnClientClick="return SelecionaData('txtDataFim', this);" /></td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                                            ToolTip="Pesquisar" onclick="imgPesq_Click" CausesValidation="False" /></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>&nbsp; </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtNumPedido" runat="server" Width="60px"></asp:TextBox>
                                                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" 
                                                                OnClick="imgPesq_Click" ToolTip="Pesquisar" CausesValidation="False" /></td>
                                                    <td nowrap="nowrap">
                                                        &nbsp;</td>
                                                    <td>
                                                        <asp:LinkButton ID="lnkRemFiltro" runat="server" onclick="lnkRemFiltro_Click" 
                                                            Font-Size="8pt">Remover Filtros</asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                <asp:GridView ID="grdCorte" runat="server" CellPadding="4" ForeColor="#333333" 
                                                GridLines="None" AllowPaging="True" 
                                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdPedido" 
                                    DataSourceID="odsCorte" EmptyDataText="Nenhum pedido encontrado.">
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkExcluir" runat="server" CommandName="Delete"
                                                    onclientclick="return confirm('Confirma remoção deste Pedido da situação \'' + FindControl('lblPedidos', 'span').innerHTML + '\'?');">
                                                    <img src="../Images/ExcluirGrid.gif" border="0"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" 
                                            SortExpression="IdPedido" />
                                        <asp:BoundField DataField="NomeInicialCli" HeaderText="Cliente" ReadOnly="True" 
                                            SortExpression="NomeInicialCli" />
                                        <asp:BoundField DataField="DataEntrega" DataFormatString="{0:d}" 
                                            HeaderText="Entrega" SortExpression="DataEntrega" />
                                    </Columns>
                                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <EditRowStyle BackColor="#999999" />
                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                </asp:GridView>
                                            <br />
                                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();">
                                            <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorte" runat="server" 
                                                DataObjectTypeName="Glass.Data.Model.Pedido" DeleteMethod="VoltaSituacao" 
                                                SelectMethod="GetForCorte" TypeName="Glass.Data.DAL.PedidoDAO" 
                                                EnablePaging="True" MaximumRowsParameterName="pageSize" 
                                                SelectCountMethod="GetCountCorte" SortParameterName="sortExpression" 
                                                StartRowIndexParameterName="startRow">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" 
                                                        PropertyName="Text" Type="UInt32" />
                                                    <asp:ControlParameter ControlID="txtDataIni" Name="dataIni" PropertyName="Text" 
                                                        Type="String" />
                                                    <asp:ControlParameter ControlID="txtDataFim" Name="dataFim" PropertyName="Text" 
                                                        Type="String" />
                                                    <asp:QueryStringParameter Name="situacao" QueryStringField="sit" Type="Int32" />
                                                </SelectParameters>
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
<script type="text/javascript">

    setInterval("MetodosAjax.ManterLogado()", 600000);

</script>
</html>
