<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowMsg.aspx.cs" Inherits="Glass.UI.Web.Utils.ShowMsg"
    Title="" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <asp:TextBox ID="txtMsg" runat="server" BorderStyle="None" Rows="20" TextMode="MultiLine"
                    Width="500px"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
