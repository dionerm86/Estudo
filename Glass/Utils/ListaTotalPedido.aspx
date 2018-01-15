<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaTotalPedido.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ListaTotalPedido" Title="Valores Totais dos Pedidos" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <style type="text/css">
        #form1
        {
            text-align: center;
        }
        .style1
        {
            text-align: right;
        }
        .style2
        {
            text-align: left;
        }
        .style3
        {
            height: 16px;
        }
    </style>
    <table align="center">
        <tr>
            <td class="style1">
                <asp:Label ID="Label7" runat="server" Text="&lt;b/&gt;Quantidade de pedidos:&lt;/b&gt;&amp;nbsp;"></asp:Label>
            </td>
            <td class="style2">
                <asp:Label ID="lblQuantidadePedidos" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label4" runat="server" Text="&lt;b/&gt;Total em R$ dos pedidos:&lt;/b&gt;&amp;nbsp;"></asp:Label>
            </td>
            <td class="style2">
                <asp:Label ID="lblTotalPedidos" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label5" runat="server" Text="&lt;b/&gt;Total em m² dos pedidos:&lt;/b&gt;&amp;nbsp;"></asp:Label>
            </td>
            <td class="style2">
                <asp:Label ID="lblM2Pedidos" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label6" runat="server" Text="&lt;b/&gt;Peso total dos pedidos:&lt;/b&gt;&amp;nbsp;"></asp:Label>
            </td>
            <td class="style2">
                <asp:Label ID="lblPesoPedidos" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
