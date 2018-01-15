<%@ Page Title="Anexar XML NFe Entrada de Terceiros" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true"
    CodeBehind="AnexarXMLNFeEntradaTerceiros.aspx.cs" Inherits="Glass.UI.Web.Utils.AnexarXMLNFeEntradaTerceiros" %>
<asp:Content ID="menu" ContentPlaceHolderID="Menu" runat="server">
</asp:Content>
<asp:Content ID="pagina" ContentPlaceHolderID="Pagina" runat="server">

    <table>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblXML" runat="server" Text="Selecione o XML da Nota Fiscal de Entrada de Terceiros"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:FileUpload ID="fupXMLNFeTer" runat="server" Height="24px" />
                &nbsp;<asp:Button ID="btnUpload" runat="server" OnClick="btnUpload_Click" Text="Enviar" Width="100px" Height="24px" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblInfoNFE" runat="server"></asp:Label>
            </td>
        </tr>
    </table>

</asp:Content>
