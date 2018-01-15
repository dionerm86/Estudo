<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlSelAplicacao.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlSelAplicacao" %>
<%@ Register Src="~/Controls/ctrlSelPopup.ascx" TagPrefix="uc1" TagName="ctrlSelPopup" %>
<uc1:ctrlSelPopup ID="selAplicacao" runat="server" DataSourceID="odsAplicacao"
    DataTextField="CodInterno" DataValueField="IdAplicacao" FazerPostBackBotaoPesquisar="false"
    ColunasExibirPopup="IdAplicacao|CodInterno|Descricao" 
    ExibirIdPopup="false" TitulosColunas="Id|Cód.|Descrição" TituloTela="Selecione a aplicação"
    TextWidth="50px" />
<colo:VirtualObjectDataSource ID="odsAplicacao" runat="server" SortParameterName="sortExpression"
    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges" 
    Culture="pt-BR" MaximumRowsParameterName="pageSize" SelectMethod="GetForSel"
    StartRowIndexParameterName="startRow" 
    TypeName="Glass.Data.DAL.EtiquetaAplicacaoDAO" SkinID="" 
    EnablePaging="True" SelectCountMethod="GetForSelCount">
</colo:VirtualObjectDataSource>