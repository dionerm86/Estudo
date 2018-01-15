<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelTextoPedido.aspx.cs" Inherits="Glass.UI.Web.Utils.SelTextoPedido"
    Title="Textos para o Pedido n.º" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    
    <script type="text/javascript">

        function openLog()
        {
            openWindow(600, 800, '../Utils/ShowLogCancelamento.aspx?tabela=<%= GetCodigoTabela() %>');
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Label ID="Label2" runat="server" CssClass="subtitle1" Text="Textos Disponíveis"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdTextoImprPedido" runat="server" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsTextoImprPedido"
                    EmptyDataText="Nenhum texto de pedido cadastrado." DataKeyNames="IdTextoImprPedido"
                    PageSize="5" OnRowCommand="grdTextoImprPedido_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkAdd" runat="server" CommandArgument='<%# Eval("IdTextoImprPedido") %>'
                                    CommandName="IncluiTexto">
                                     <img src="../Images/Insert.gif" border="0"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Título" SortExpression="Titulo">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTitulo" runat="server" MaxLength="50" Text='<%# Bind("Titulo") %>'
                                    Width="150px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Titulo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="1000" Rows="5" Text='<%# Bind("Descricao") %>'
                                    TextMode="MultiLine" Width="600px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTextoImprPedido" runat="server" DataObjectTypeName="Glass.Data.Model.TextoImprPedido"
                    DeleteMethod="Delete" SelectMethod="GetList" TypeName="Glass.Data.DAL.TextoImprPedidoDAO"
                    UpdateMethod="Update" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="Label1" runat="server" CssClass="subtitle1" Text="Textos Selecionados"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdTextoPedido" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdTextoPedido" DataSourceID="odsTextoPedido" EmptyDataText="Nenhum texto selecionado."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Titulo" HeaderText="Título" SortExpression="Titulo" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTextoPedido" runat="server" DataObjectTypeName="Glass.Data.Model.TextoPedido"
                    DeleteMethod="Delete" SelectMethod="GetByPedido" TypeName="Glass.Data.DAL.TextoPedidoDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                
                <asp:LinkButton ID="lnkCanceladas" runat="server" OnClientClick="openLog(); return false">
                    <img alt="" border="0" src="../Images/ExcluirGrid.gif" /> Textos Excluídos</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
