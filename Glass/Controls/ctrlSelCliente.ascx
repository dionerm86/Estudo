<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlSelCliente.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlSelCliente" %>

<%@ Register src="ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc1" %>

<span style="display: inline-table">
    <span style="display: table-cell; padding-right: 8px">
        <uc1:ctrlSelPopup ID="ctrlSelClienteBuscar" runat="server" 
            DataSourceID="odsCliente" DataTextField="IdCli" DataValueField="IdCli" 
            ExibirIdPopup="True" TitulosColunas="Cód.|Nome" 
            TituloTela="Selecione o cliente" UrlPopup="~/Utils/SelCliente.aspx" 
            CallbackSelecao="getDescricaoSelCliente" TamanhoTela="Tamanho700x525" 
            TextWidth="50px" />
    </span>
    <span style="display: table-cell; vertical-align: middle">
        <asp:Label ID="lblNomeCliente" runat="server"></asp:Label>
    </span>
</span>
<asp:HiddenField ID="hdfCallback" runat="server" />
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsCliente" runat="server" 
    SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.ClienteDAO">
</colo:VirtualObjectDataSource>
