<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlNaturezaOperacao.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlNaturezaOperacao" %>
<%@ Register src="ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc1" %>
<uc1:ctrlSelPopup ID="selNaturezaOperacao" runat="server" 
    DataSourceID="odsNaturezaOperacao" DataTextField="CodigoControleUsar" DataValueField="Codigo"
    ExibirIdPopup="false" TamanhoTela="Tamanho1000x600" TextWidth="75px" 
    ColunasExibirPopup="Codigo|CodigoInternoCfop|CodigoInterno|DescricaoCfop|CstIcms|CalcularIcms|CalcularIcmsSt|CalcularIpi|CalcularPis|CalcularCofins|AlterarEstoqueFiscal"
    TitulosColunas="Código|CFOP|Cód.|Descrição|CST ICMS|Calcula ICMS|Calcula ICMS-ST|Calcula IPI|Calcula PIS|Calcula Cofins|Alterar Estoque Fiscal" 
    TituloTela="Selecione a Natureza da Operação" UrlPopup="~/Utils/SelNaturezaOperacao.aspx" />
<colo:VirtualObjectDataSource ID="odsNaturezaOperacao" runat="server" 
    SelectMethod="ObtemParaRelatorio" Culture="pt-BR"
    TypeName="WebGlass.Business.NaturezaOperacao.Fluxo.BuscarEValidar">
</colo:VirtualObjectDataSource>
