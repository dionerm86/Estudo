<%@ Page Title="Alterar Grupo/Subgrupo" Language="C#" AutoEventWireup="true" CodeBehind="CadAlterarAliqIcmsSn.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadAlterarAliqIcmsSn" %>

<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
<link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/Geral.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>

<form id="form1" runat="server">
<div class="h2">
    <asp:label id="Label1" runat="server" text="Alterar Alíq. ICMS (Simples Nacional)"></asp:label>
</div>
<br />
<div align="center">
    <asp:label id="Label3" runat="server" text="Alíquota ICMS SN: "></asp:label>
    <asp:textbox id="txtAliquotaIcmsSn" maxlength="7" width="40px" runat="server" onkeypress="return soNumeros(event, false, true)"></asp:textbox>
</div>
<div align="center">
    <asp:button id="btnSalvar" runat="server" text="Salvar" onclick="btnSalvar_Click"></asp:button>
</div>
</form>
