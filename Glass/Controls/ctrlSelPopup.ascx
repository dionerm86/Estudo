<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlSelPopup.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlSelPopup" %>
<table id="<%= this.ClientID %>" class="pos" cellpadding="0" cellspacing="0" style="margin-left: -2px; margin-right: -2px; display: inline-table">
    <tr>
        <td>
            <asp:TextBox ID="txtDescr" runat="server" onkeypress="if (isEnter(event)) this.onblur();"></asp:TextBox>
        </td>
        <td style="padding-left: 2px">
            <asp:ImageButton ID="imgPesq" runat="server" 
                ImageUrl="~/Images/Pesquisar.gif" onclick="imgPesq_Click" />
        </td>
        <td style='<%= PermitirVazio ? "display: none" : "padding-left: 2px" %>'>
            <asp:CustomValidator ID="ctvSelPopup" runat="server" ControlToValidate="txtDescr" 
                Display="Dynamic" Text="*" ValidateEmptyText="True" Enabled="False"></asp:CustomValidator>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfValor" runat="server" />