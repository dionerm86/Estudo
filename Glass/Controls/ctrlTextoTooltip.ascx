<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlTextoTooltip.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlTextoTooltip" %>
<asp:ImageButton ID="imgIcone" runat="server" ImageUrl="~/Images/blocodenotas.png" />
<div id="divTooltip" runat="server" style="display: none">
    <asp:Label ID="lblTooltip" runat="server" Text="Label"></asp:Label>
</div>
