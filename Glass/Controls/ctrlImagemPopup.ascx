<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlImagemPopup.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlImagemPopup" %>
<asp:ImageButton ID="imgIcone" runat="server" ImageUrl="~/Images/imagem.gif" />
<div runat="server" id="divImagem" style="display: none; padding: 4px">
    <asp:Image ID="imgImagem" runat="server" EnableViewState="False" />
    <center>
        <asp:Label ID="lblLegenda" runat="server" Font-Bold="true"
            Style="position: relative; top: 2px"></asp:Label>
    </center>
</div>