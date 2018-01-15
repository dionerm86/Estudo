<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LocalizacaoCheque.aspx.cs"
    Inherits="Glass.UI.Web.Utils.LocalizacaoCheque" Title="Localização de Cheque" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <asp:Label ID="lblLocalizacao" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
