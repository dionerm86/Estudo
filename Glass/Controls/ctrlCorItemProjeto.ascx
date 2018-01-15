<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlCorItemProjeto.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlCorItemProjeto" %>

<asp:ImageButton ID="imgExibir" runat="server" ImageUrl="../Images/Cor.gif" 
    onload="imgExibir_Load" ToolTip="Alterar cor" />
<span style="display: none">
    <asp:Button ID="btnAplicar" runat="server" Text="Alterar" 
        onclick="btnAplicar_Click" />
    <asp:HiddenField ID="hdfExibirTooltip" runat="server" Value="true" />
    <asp:HiddenField ID="hdfCorIdItemProjeto" runat="server" />
    <asp:HiddenField ID="hdfCorIdProjeto" runat="server" />
    <asp:HiddenField ID="hdfCorIdOrcamento" runat="server" />
    <asp:HiddenField ID="hdfCorIdPedido" runat="server" />
    <asp:HiddenField ID="hdfCorIdPedidoEspelho" runat="server" />
    <asp:HiddenField ID="hdfCorVidro" runat="server" />
    <asp:HiddenField ID="hdfCorAluminio" runat="server" />
    <asp:HiddenField ID="hdfCorFerragem" runat="server" />
    <asp:HiddenField ID="hdfTitulo" runat="server" Value="Alterar cores do item" />
</span>
<table id="CorItemProjeto" runat="server">
    <tr id="tituloCorItemProjeto" runat="server">
        <th colspan="2" nowrap="nowrap" align="center">
            <asp:Label ID="lblTitulo" runat="server"></asp:Label>
&nbsp;</th>
    </tr>
    <tr>
        <td nowrap="nowrap">
            Cor dos vidros
        </td>
        <td nowrap="nowrap">
            <asp:DropDownList ID="drpCorVidro" runat="server"
                DataTextField="Descricao" DataValueField="IdCorVidro" AppendDataBoundItems="True">
                <asp:ListItem Value="0">Não alterar</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td nowrap="nowrap">
            Cor dos alumínios
        </td>
        <td nowrap="nowrap">
            <asp:DropDownList ID="drpCorAluminio" runat="server" 
                DataSourceID="odsCorAluminio" DataTextField="Descricao" 
                DataValueField="IdCorAluminio" AppendDataBoundItems="True">
                <asp:ListItem Value="">Não alterar</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td nowrap="nowrap">
            Cor das ferragens
        </td>
        <td nowrap="nowrap">
            <asp:DropDownList ID="drpCorFerragem" runat="server" 
                DataSourceID="odsCorFerragem" DataTextField="Descricao" 
                DataValueField="IdCorFerragem" AppendDataBoundItems="True">
                <asp:ListItem Value="">Não alterar</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="center">
            <asp:Button ID="btnAlterar" runat="server" Text="Alterar" />
        </td>
    </tr>
</table>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorAluminio" runat="server" SelectMethod="GetAll" 
    TypeName="Glass.Data.DAL.CorAluminioDAO"></colo:VirtualObjectDataSource>
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsCorFerragem" runat="server" SelectMethod="GetAll" 
    TypeName="Glass.Data.DAL.CorFerragemDAO"></colo:VirtualObjectDataSource>
