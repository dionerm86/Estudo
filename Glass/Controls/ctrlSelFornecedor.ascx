<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlSelFornecedor.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlSelFornecedor" %>

<%@ Register src="ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc1" %>

<span style="display: inline-table">
    <span style="display: table-cell; padding-right: 8px">
        <uc1:ctrlSelPopup ID="ctrlSelFornecBuscar" runat="server" 
            DataSourceID="odsFornecedor" DataTextField="IdFornec" DataValueField="IdFornec" 
            ExibirIdPopup="True" TitulosColunas="Cód.|Nome" 
            TituloTela="Selecione o fornecedor" UrlPopup="~/Utils/SelFornec.aspx" 
            CallbackSelecao="getDescricaoSelFornecedor" TamanhoTela="Tamanho700x525" 
            TextWidth="50px" />
    </span>
    <span style="display: table-cell; vertical-align: middle">
        <asp:Label ID="lblNomeFornec" runat="server"></asp:Label>
    </span>
</span>
<asp:HiddenField ID="hdfCallback" runat="server" />
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsFornecedor" runat="server" 
    SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.FornecedorDAO">
</colo:VirtualObjectDataSource>
