<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetLojaEmail.aspx.cs" Inherits="Glass.UI.Web.Utils.SetLojaEmail"
    Title="Dados de Contato via e-Mail" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <style type="text/css">
        .style1
        {
            text-align: right;
        }
    </style>

    <script type="text/javascript">
        function fechar()
        {
            closeWindow();
        }
    </script>

    <table runat="server">
        <tr>
            <td align="center" colspan="2" bgcolor="#DDDDDD">
                <asp:Label ID="Label4" runat="server" Text="e-Mail Fiscal"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label13" runat="server" Text="Email: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxEmailFiscal" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label1" runat="server" Text="Login: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxLoginFiscal" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label2" runat="server" Text="Senha: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxSenhaFiscal" runat="server" Width="150px" TextMode="Password"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label3" runat="server" Text="Servidor: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxServidorFiscal" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2" bgcolor="#DDDDDD">
                <asp:Label ID="Label5" runat="server" Text="e-Mail Comercial"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label14" runat="server" Text="Email: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxEmailComercial" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label6" runat="server" Text="Login: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxLoginComercial" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label7" runat="server" Text="Senha: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxSenhaComercial" runat="server" Width="150px" TextMode="Password"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label8" runat="server" Text="Servidor: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxServidorComercial" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2" bgcolor="#DDDDDD">
                <asp:Label ID="Label9" runat="server" Text="e-Mail de Contato"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label15" runat="server" Text="Email: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxEmailContato" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label10" runat="server" Text="Login: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxLoginContato" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label11" runat="server" Text="Senha: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxSenhaContato" runat="server" Width="150px" TextMode="Password"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                <asp:Label ID="Label12" runat="server" Text="Servidor: "></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbxServidorContato" runat="server" Width="150px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblupdate" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" Width="78px" OnClick="btnConfirmar_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
