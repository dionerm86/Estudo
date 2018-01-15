<%@ Page Title="Exportar/importar Preço de Produto" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true" 
    CodeBehind="ExportarPrecoProduto.aspx.cs" Inherits="Glass.UI.Web.Utils.ExportarPrecoProduto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Menu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Pagina" Runat="Server">
    <script type="text/javascript">
        function importar()
        {
            if (!validate("importar"))
                return false;
            
            bloquearPagina();
            desbloquearPagina(false);
            return true;
        }
    </script>
    <fieldset style="float: left; display: table">
        <legend>Exportar</legend>
        <div style="display: table-cell; height: 80px; vertical-align: middle">
            Gerar o arquivo com os filtros da lista de produtos
            <br />
            <asp:Button ID="btnExportar" runat="server" Text="Exportar" 
                OnClientClick="window.opener.setTimeout(window.opener.exportarPrecos, 100); closeWindow()" 
                ValidationGroup="exportar" />
        </div>
    </fieldset>
    <fieldset style="display: table">
        <legend>Importar</legend>
        <div style="display: table-cell; height: 80px; vertical-align: middle">
            Arquivo com os preços
            <br />
            <asp:FileUpload ID="filArquivo" runat="server" />
            <asp:RequiredFieldValidator ID="rfvArquivo" runat="server" 
                ControlToValidate="filArquivo" Display="Dynamic" ErrorMessage="<br />Arquivo não pode ser vazio" 
                ValidationGroup="importar"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="revArquivo" runat="server" 
                ErrorMessage="<br />Apenas arquivos de Excel" ControlToValidate="filArquivo" 
                Display="Dynamic" ValidationExpression="^.*\.(xls|xlsx|csv)$" 
                ValidationGroup="importar"></asp:RegularExpressionValidator>
            <br />
            <asp:Button ID="btnImportar" runat="server" Text="Importar" 
                onclick="btnImportar_Click" ValidationGroup="importar" OnClientClick="if (!importar()) return false" />
        </div>
    </fieldset>
</asp:Content>

