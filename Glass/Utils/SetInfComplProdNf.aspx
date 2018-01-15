<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetInfComplProdNf.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetInfComplProdNf" Title="Informações adicionais" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table cellpadding="0" cellspacing="0" align="center">
        <tr>
            <td align="center">
                <asp:Label ID="lblProdNf" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table width="100%" cellpadding="4" cellspacing="0">
                    <tr>
                        <td align="center">
                            <asp:Label ID="Label1" runat="server" Text="Info.:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtInfo" runat="server" MaxLength="500" TextMode="MultiLine" Width="329px"
                                Rows="5"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click"
                    Style="margin: 4px" />
            </td>
        </tr>
    </table>
</asp:Content>
