<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Erro.aspx.cs" Inherits="Glass.UI.Web.Utils.Erro"
    Title="Erro" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table align="center">
        <tr id="url" runat="server">
            <td style="font-weight: bold; padding-bottom: 6px">
                URL:
                <asp:Label ID="lblURL" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblErro" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <a id="fechar" href="#" onclick="closeWindow();">Fechar</a>
                <a id="voltar" href="#" onclick="<%= BackUrl %>">Voltar</a>
                
                <script type="text/javascript">
                    var isPopup = window.top != window;
                    document.getElementById("fechar").style.display = isPopup ? "" : "none";
                    document.getElementById("voltar").style.display = isPopup ? "none" : "";
                </script>
            </td>
        </tr>
    </table>
</asp:Content>
