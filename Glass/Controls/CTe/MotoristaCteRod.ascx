<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MotoristaCteRod.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.MotoristaCteRod" %>
<table runat="server" id="tabela" class="table-linha" cellpadding="1" cellspacing="1">
    <tr>
        <td class="dtvHeader">
            Motorista
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpMotorista" runat="server" DataSourceID="odsMotorista" DataTextField="Nome"
                DataValueField="IdFunc" Height="20px" Width="125px" AppendDataBoundItems="True"
                Enabled="true" Visible="true">
                <asp:ListItem Value="selecione" Text="Selecione"></asp:ListItem>
            </asp:DropDownList>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfIdFunc" runat="server" />
<colo:VirtualObjectDataSource culture="pt-BR" ID="odsMotorista" runat="server" SelectMethod="GetMotoristas"
    TypeName="Glass.Data.DAL.FuncionarioDAO">
    <SelectParameters>
        <asp:Parameter Name="nome" Type="String" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
