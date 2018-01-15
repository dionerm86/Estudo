<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowPrecoAnterior.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ShowPrecoAnterior" Title="Histórico de Preço" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblProduto" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label4" runat="server" Text="Fab. Base:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblFabBase" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label5" runat="server" Text="Custo Compra:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblCustoCompra" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label6" runat="server" Text="Atacado:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblAtacado" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label7" runat="server" Text="Balcão:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblBalcao" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label8" runat="server" Text="Obra:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblObra" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
