<%@ Page Title="Entrada de Estoque" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadEntradaEstoque.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadEntradaEstoque" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            Número da
                            <asp:DropDownList ID="drpTipo" runat="server">
                                <asp:ListItem Value="0">Compra</asp:ListItem>
                                <asp:ListItem Value="1">NFe</asp:ListItem>
                            </asp:DropDownList>&nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNum" onkeypress="return soNumeros(event, true, true);"
                                runat="server" onkeydown="if (isEnter(event)) cOnClick('imbPesq', null);" Width="70px"
                                TabIndex="1"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNumPedido" runat="server" ControlToValidate="txtNum"
                                ErrorMessage="*"></asp:RequiredFieldValidator>&nbsp;<asp:ImageButton ID="imbPesq"
                                runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imbPesq_Click" TabIndex="2" />
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
    </table>
    <table id="tbEntradaCompra" runat="server" style="width: 100%" visible="false">
        <tr>
            <td class="subtitle1">
                <asp:Label ID="lblSubtituloCompra" runat="server" Text="Produtos da Compra"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdCompra" runat="server" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsProdCompra" 
                    CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdProdCompra"
                    EmptyDataText="Nenhum produto encontrado." PageSize="30">
                    <Columns>
                        <asp:TemplateField HeaderText="Qtde. Entrada">
                            <HeaderTemplate>
                                <asp:Label ID="Label3" runat="server" Text="Qtde. Entrada"></asp:Label></td>
                                &nbsp;<br />
                                <asp:LinkButton ID="lnkTodos" runat="server" ForeColor="blue"
                                    OnClientClick="return marcarEntrada();" onclick="lnkTodosCompra_Click">(Marcar todos)</asp:LinkButton></td>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtdEntrada" runat="server" onkeypress="return soNumeros(event, true, true);"
                                    Width="50px" TabIndex="3" Text='<%# Eval("QtdeEntradaEstoque") %>'></asp:TextBox>
                                <asp:HiddenField ID="hdfIdProdCompra" runat="server" 
                                    Value='<%# Eval("IdProdCompra") %>' />
                                <asp:HiddenField ID="hdfDescr" runat="server" Value='<%# Eval("DescrProduto") %>' />
                                <asp:HiddenField ID="hdfQtde" runat="server" Value='<%# Eval("Qtde") %>' />
                                <asp:HiddenField ID="hdfQtdEntrada" runat="server" 
                                    Value='<%# Eval("QtdeEntrada") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor Vendido" SortExpression="Valor"
                            DataFormatString="{0:C}" />
                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Qtde") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="#33CC33" Text='<%# Eval("Qtde") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Entrada" SortExpression="QtdEntrada">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("QtdSaida") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Font-Bold="True" ForeColor="Red" 
                                    Text='<%# Eval("QtdeEntrada") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="TotM" HeaderText="Tot. m²" SortExpression="TotM" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}" />
                    </Columns>

<PagerStyle CssClass="pgr"></PagerStyle>

<EditRowStyle CssClass="edit"></EditRowStyle>

<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Marcar entrada na loja:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLojaCompra" runat="server" 
                                DataSourceID="odsLojaCompra" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" TabIndex="4">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnMarcarEntradaCompra" runat="server" Text="Marcar Entrada" OnClick="btnMarcarEntradaCompra_Click"
                    OnClientClick="return confirm('Marcar entrada dos produtos nas quantidades informadas?');"
                    TabIndex="5" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdCompra" runat="server" 
                    MaximumRowsParameterName="pageSize" SelectMethod="GetForEntradaEstoque" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.ProdutosCompraDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNum" Name="idCompra" PropertyName="Text"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLojaCompra" runat="server" SelectMethod="GetAll" 
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <table id="tbEntradaNFe" runat="server" style="width: 100%" visible="false">
        <tr>
            <td class="subtitle1">
                <asp:Label ID="lblSubtitloNFe" runat="server" Text="Produtos da NFe"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdNf" runat="server" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsProdNf" 
                    CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdProdNf"
                    EmptyDataText="Nenhum produto encontrado." PageSize="30">
                    <Columns>
                        <asp:TemplateField HeaderText="Qtde. Entrada">
                            <HeaderTemplate>
                                <asp:Label ID="Label6" runat="server" Text="Qtde. Entrada"></asp:Label></td>
                                &nbsp;<br />
                                <asp:LinkButton ID="lnkTodos" runat="server" ForeColor="blue"
                                    OnClientClick="return marcarEntrada();" onclick="lnkTodosNf_Click">(Marcar todos)</asp:LinkButton></td>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtdEntrada" runat="server" onkeypress="return soNumeros(event, true, true);"
                                    Width="50px" TabIndex="3" Text='<%# Eval("QtdeEntradaEstoque") %>'></asp:TextBox>
                                <asp:HiddenField ID="hdfIdProdNf" runat="server" 
                                    Value='<%# Eval("IdProdNf") %>' />
                                <asp:HiddenField ID="hdfDescr" runat="server" Value='<%# Eval("DescrProduto") %>' />
                                <asp:HiddenField ID="hdfQtde" runat="server" Value='<%# Eval("Qtde") %>' />
                                <asp:HiddenField ID="hdfQtdEntrada" runat="server" 
                                    Value='<%# Eval("QtdeEntrada") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="ValorUnitario" HeaderText="Valor Vendido" SortExpression="ValorUnitario"
                            DataFormatString="{0:C}" />
                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Qtde") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Font-Bold="True" ForeColor="#33CC33" 
                                    Text='<%# Eval("Qtde") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Entrada" SortExpression="QtdEntrada">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("QtdSaida") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Font-Bold="True" ForeColor="Red" 
                                    Text='<%# Eval("QtdeEntrada") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="TotM" HeaderText="Tot. m²" SortExpression="TotM" />
                        <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}" />
                    </Columns>

<PagerStyle CssClass="pgr"></PagerStyle>

<EditRowStyle CssClass="edit"></EditRowStyle>

<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Marcar entrada na loja:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLojaNf" runat="server" DataSourceID="odsLojaNf" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" TabIndex="4">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnMarcarEntradaNF" runat="server" Text="Marcar Entrada" OnClick="btnMarcarEntradaNF_Click"
                    OnClientClick="return confirm('Marcar entrada dos produtos nas quantidades informadas?');"
                    TabIndex="5" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdNf" runat="server" 
                    MaximumRowsParameterName="pageSize" SelectMethod="GetForEntradaEstoque" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.ProdutosNfDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNum" Name="numeroNFe" PropertyName="Text"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLojaNf" runat="server" SelectMethod="GetAll" 
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
