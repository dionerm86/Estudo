<%@ Page Title="Pedidos ignorados na comissão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstPedidosIgnoradosComissao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPedidosIgnoradosComissao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtIdPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Motivo" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtMotivo" runat="server" Width="300px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:GridView ID="grdPedidosIgnorados" runat="server" SkinID="gridViewEditable"
                    DataKeyNames="IdPedido" DataSourceID="odsPedidosIgnorados" AutoGenerateColumns="false"
                    AllowPaging="true" AllowSorting="true" ShowFooter="true" PageSize="10"
                    OnDataBound="grdPedidosIgnorados_DataBound" OnRowCommand="grdPedidosIgnorados_RowCommand" Style="min-width: 500px">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" OnClientClick="return confirm('Deseja realmente remover a exclusão do pedido para gerar comissão?');"
                                    CommandName="RemoverIgnorar" CommandArgument='<%# Eval("IdPedido") %>' ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód" SortExpression="IdPedido">
                            <FooterTemplate>
                                <asp:TextBox ID="txtIdPedido" runat="server" onkeypress="return soNumeros(event, true, true);" Text='<%# Bind("IdPedido") %>' Width="70px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblIdPedido" runat="server" Text='<%# Eval("IdPedido") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodCliente" HeaderText="Pedido Cli." SortExpression="CodCliente" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        
                        <asp:TemplateField HeaderText="Motivo" SortExpression="MotivoIgnorarComissao">
                            <FooterTemplate>
                                <asp:TextBox ID="txtMotivo" runat="server" Width="400px" MaxLength="200" Text='<%# Bind("MotivoIgnorarComissao") %>' TextMode="MultiLine"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblMotivo" runat="server" Text='<%# Eval("MotivoIgnorarComissao") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return confirm('Deseja realmente ignorar esse pedido na comissão?');" OnClick="lnkInserir_Click">
                                <img border="0" src="../Images/insert.gif" alt=""/>
                                </asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedidosIgnorados" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="ObterPedidosIgnorarComissaoCount" SelectMethod="ObterPedidosIgnorarComissao" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.PedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtIdPedido" PropertyName="Text" Name="idPedido" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtMotivo" PropertyName="Text" Name="motivo" Type="String"/>
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
