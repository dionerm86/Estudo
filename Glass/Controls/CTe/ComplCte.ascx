<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComplCte.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.ComplCte" %>
<%@ Register Src="~/Controls/CTe/ComplPassagemCte.ascx" TagName="ctrlComplPassagemCte"
    TagPrefix="uc1" %>
<uc1:ctrlComplPassagemCte ID="ctrlComplPassagemCte" runat="server" />
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="lblRota" runat="server" Text="Rota" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:DropDownList ID="drpRota" runat="server" DataSourceID="odsRota" DataTextField="Descricao"
            DataValueField="IdRota" AppendDataBoundItems="True">
            <asp:ListItem Value="0" Text="Selecione"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="dtvHeader">
        <asp:Label ID="lblCaractTransporte" runat="server" Text="Caract. Transporte" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox runat="server" ID="txtCaractTransporte"></asp:TextBox>
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="lblCaractServico" runat="server" Text="Caract. Serviço" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox runat="server" ID="txtCaractServico"></asp:TextBox>
    </div>
    <div class="dtvHeader">
        <asp:Label ID="lblSiglaOrigem" runat="server" Text="Sigla Origem" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox runat="server" ID="txtSiglaOrigem"></asp:TextBox>
    </div>
</div>
<div class="dtvRow">
    <div class="dtvHeader">
        <asp:Label ID="lblSiglaDestino" runat="server" Text="Sigla Destino" Font-Bold="True"></asp:Label>
    </div>
    <div class="dtvAlternatingRow">
        <asp:TextBox runat="server" ID="txtSiglaDestino"></asp:TextBox>
    </div>
</div>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.RotaDAO">
</colo:VirtualObjectDataSource>
