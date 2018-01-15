<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="BackupBD.aspx.cs" Inherits="Glass.UI.Web.Utils.BackupBD" Title="Backup BD" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <table>
        <tr>
            <td align="center">
                <asp:Label ID="Label2" runat="server" Text="Selecione um arquivo para ser enviado para o FTP:"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:RadioButtonList ID="rblUltimosBkps" runat="server">
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td align="center">
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" Text="Tipo de Backup (Se gerar novo):"></asp:Label>
&nbsp;<asp:DropDownList ID="drpTipoBackup" runat="server">
                    <asp:ListItem Value="1">Otimizado</asp:ListItem>
                    <asp:ListItem Value="2">Completo</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="text-align: center">
                <asp:Button ID="btnProcessarEnviarFTP" runat="server" OnClick="btnProcessarEnviarFTP_Click" Text="Processar e enviar para FTP" />
            </td>
        </tr>
    </table>
</asp:Content>
