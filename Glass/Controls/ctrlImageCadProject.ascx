<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlImageCadProject.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlImageCadProject" %>
<asp:ImageButton ID="imgIcone" runat="server" ImageUrl="~/Images/imagem.gif" />
<div runat="server" id="divImagem" style="padding: 4px; min-width:250px">
    <div runat="server" id="divSvg">
    </div>
    <asp:Label ID="lblLegenda" runat="server" Font-Bold="true" Style="position: relative; top: 2px"></asp:Label>
</div>
