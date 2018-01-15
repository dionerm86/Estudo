<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelArquivoOtimizado.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelArquivoOtimizado" Title="Importar Arquivo de Otimização" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <asp:FileUpload ID="fluArquivoOtimizado" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnImportarArquivo" runat="server" OnClick="btnImportarArquivo_Click"
                    Text="Importar Arquivo" />
            </td>
        </tr>
    </table>
</asp:Content>
