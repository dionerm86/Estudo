<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelBase.aspx.cs" Inherits="Glass.UI.Web.Relatorios.NFe.RelBase" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href='~/Style/Geral.css' type="text/css" rel="Stylesheet">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.8.2.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:PlaceHolder ID="pchTabela" runat="server"></asp:PlaceHolder>
    </form>
</body>
</html>
