<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlLoja.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlLoja" %>

<asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLojaAtiva" DataTextField="Name"
    DataValueField="Id" AppendDataBoundItems="True" EnableViewState="false">
</asp:DropDownList>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" EnableViewState="false"
    SelectMethod="ObtemLojas" TypeName="Glass.Global.Negocios.ILojaFluxo">
</colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsLojaAtiva" runat="server" EnableViewState="false" 
    SelectMethod="ObtemLojasAtivas" TypeName="Glass.Global.Negocios.ILojaFluxo">
</colo:VirtualObjectDataSource>