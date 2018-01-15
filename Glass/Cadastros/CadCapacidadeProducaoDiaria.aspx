<%@ Page Title="Capacidade de Produção Diária" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" 
    CodeBehind="CadCapacidadeProducaoDiaria.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCapacidadeProducaoDiaria" %>

<%@ Register src="../Controls/ctrlData.ascx" tagname="ctrlData" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlCapacidadeProducaoDiaria.ascx" tagname="ctrlCapacidadeProducaoDiaria" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <style type="text/css">
        .hoje {
            background-color: #ddd;
        }
    </style>
    <div class="filtro">
        <div>
            <span>
                <asp:Label ID="Label1" runat="server" Text="Período" AssociatedControlID="ctrlDataIni"></asp:Label>
                <uc1:ctrlData ID="ctrlDataIni" runat="server" />
                a
                <uc1:ctrlData ID="ctrlDataFim" runat="server" />
                <asp:ImageButton ID="imgPesq" runat="server" 
                ImageUrl="~/Images/Pesquisar.gif" CssClass="botaoPesquisar" 
                onclick="imgPesq_Click" />
            </span>
        </div>
    </div>
    <asp:Table ID="tblCalendario" runat="server" CssClass="gridStyle">
        <asp:TableHeaderRow runat="server" TableSection="TableHeader">
            <asp:TableHeaderCell ID="Domingo" runat="server">Domingo</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="Segunda" runat="server">Segunda-feira</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="Terca" runat="server">Terça-feira</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="Quarta" runat="server">Quarta-feira</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="Quinta" runat="server">Quinta-feira</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="Sexta" runat="server">Sexta-feira</asp:TableHeaderCell>
            <asp:TableHeaderCell ID="Sabado" runat="server">Sábado</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>
</asp:Content>

