<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlSelProduto.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlSelProduto" %>
    
<%@ Register Src="ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc1" %>

<span style="display: inline-table">
    <span style="display: table-cell; padding-right: 8px">
        <uc1:ctrlSelPopup ID="ctrlSelProdBuscar" runat="server" TamanhoTela="Tamanho700x525"
            ColunasExibirPopup="IdProd|CodInterno|Descricao" DataSourceID="odsProduto" DataTextField="CodInterno"
            DataValueField="IdProd" ExibirIdPopup="False" FazerPostBackBotaoPesquisar="False"
            TextWidth="70px" TitulosColunas="Id|Cód.|Descrição"
            TituloTela="Selecione o produto" UrlPopup='<%# Nf != null && (bool)Nf ? "~/Utils/SelProd.aspx?notaFiscal=1" : "~/Utils/SelProd.aspx" %>' />
    </span>
    <span style="display: table-cell; vertical-align: middle">
        <span id="<%= this.ClientID %>_containerItemGenerico" style="display: none">
            Descr. Item Genérico:
            <asp:TextBox ID="txtDescricaoItemGenerico" runat="server"></asp:TextBox>
            <br />
        </span>
        <asp:Label ID="lblDescricaoProd" runat="server"></asp:Label>
    </span>
</span>
<asp:HiddenField ID="hdfTipoCalculo" runat="server" />
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" SelectMethod="GetList"
    TypeName="Glass.Data.DAL.ProdutoDAO">
</colo:VirtualObjectDataSource>
