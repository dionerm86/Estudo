<%@ Page Title="Integradores" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstIntegradores.aspx.cs" Inherits="Glass.UI.Web.Listas.LstIntegradores" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <link href="<%= ResolveClientUrl("~/Vue/Integracao/Integradores/Assets/LstIntegradores.css") %>" rel="stylesheet"/></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Integracao/Integradores/Templates/LstIntegradores.Integrador.html",
            "~/Vue/Integracao/Integradores/Templates/LstIntegradores.ParametroOperacaoIntegracao.html",
            "~/Vue/Integracao/Integradores/Templates/LstIntegradores.OperacaoIntegracao.html",
            "~/Vue/Integracao/Integradores/Templates/LstIntegradores.JobIntegracao.html",
            "~/Vue/Integracao/Integradores/Templates/LstIntegradores.ItemLogger.html",
            "~/Vue/Integracao/Integradores/Templates/LstIntegradores.Logger.html",
            "~/Vue/Integracao/Integradores/Templates/LstIntegradores.ItemEsquemaHistorico.html",
            "~/Vue/Integracao/Integradores/Templates/LstIntegradores.ItemHistorico.html",
            "~/Vue/Integracao/Integradores/Templates/LstIntegradores.Configuracao.html")
    %>
    <div id="app">
        <section class="lstintegradores">
            <span v-for="integrador in integradores">
                <integradores-integrador :integrador="integrador"></integradores-integrador>
            </span>
        </section>
    </div>
    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Integracao/Integradores/Componentes/LstIntegradores.Integrador.js" />
            <asp:ScriptReference Path="~/Vue/Integracao/Integradores/Componentes/LstIntegradores.ParametroOperacaoIntegracao.js" />
            <asp:ScriptReference Path="~/Vue/Integracao/Integradores/Componentes/LstIntegradores.OperacaoIntegracao.js" />
            <asp:ScriptReference Path="~/Vue/Integracao/Integradores/Componentes/LstIntegradores.JobIntegracao.js" />
            <asp:ScriptReference Path="~/Vue/Integracao/Integradores/Componentes/LstIntegradores.ItemEsquemaHistorico.js" />
            <asp:ScriptReference Path="~/Vue/Integracao/Integradores/Componentes/LstIntegradores.ItemHistorico.js" />
            <asp:ScriptReference Path="~/Vue/Integracao/Integradores/Componentes/LstIntegradores.ItemLogger.js" />
            <asp:ScriptReference Path="~/Vue/Integracao/Integradores/Componentes/LstIntegradores.Logger.js" />
            <asp:ScriptReference Path="~/Vue/Integracao/Integradores/Componentes/LstIntegradores.Configuracao.js" />
            <asp:ScriptReference Path="~/Vue/Integracao/Integradores/Componentes/LstIntegradores.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>

