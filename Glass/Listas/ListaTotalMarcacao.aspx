<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaTotalMarcacao.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ListaTotalMarcacao" Title="Totais de peças com/sem marcação" MasterPageFile="~/Layout.master" %>

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
                <asp:Label ID="Label7" runat="server" Text="&lt;b/&gt;Peças com marcação:&lt;/b&gt;&amp;nbsp;"></asp:Label>
            </td>
            <td class="style2">
                <asp:Label ID="lblQuantidadeComMarcacao" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label4" runat="server" Text="&lt;b/&gt;Peças sem marcação:&lt;/b&gt;&amp;nbsp;"></asp:Label>
            </td>
            <td class="style2">
                <asp:Label ID="lblQuantidadeSemMarcacao" runat="server" Text="Label"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
