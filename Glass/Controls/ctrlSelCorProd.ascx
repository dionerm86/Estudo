<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlSelCorProd.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlSelCorProd" %>

<asp:DropDownList ID="drpTipoCor" runat="server"
    onchange="alteraTipoCor(this.value)" onprerender="drpTipoCor_PreRender">
    <asp:ListItem Value="0">Sem cor</asp:ListItem>
    <asp:ListItem Value="1">Cor do vidro</asp:ListItem>
    <asp:ListItem Value="2">Cor da ferragem</asp:ListItem>
    <asp:ListItem Value="3">Cor do alumínio</asp:ListItem>
</asp:DropDownList>
<asp:DropDownList ID="drpCorVidro" runat="server" AppendDataBoundItems="True" DataSourceID="odsCorVidro"
    DataTextField="Descricao" DataValueField="IdCorVidro">
    <asp:ListItem></asp:ListItem>
</asp:DropDownList>
<asp:DropDownList ID="drpCorFerragem" runat="server" AppendDataBoundItems="True"
    DataSourceID="odsCorFerragem" DataTextField="Descricao" DataValueField="IdCorFerragem">
    <asp:ListItem></asp:ListItem>
</asp:DropDownList>
<asp:DropDownList ID="drpCorAluminio" runat="server" AppendDataBoundItems="True"
    DataSourceID="odsCorAluminio" DataTextField="Descricao" DataValueField="IdCorAluminio">
    <asp:ListItem></asp:ListItem>
</asp:DropDownList>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorVidro" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorVidroDAO">
</colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorFerragem" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorFerragemDAO">
</colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorAluminio" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.CorAluminioDAO">
</colo:VirtualObjectDataSource>
