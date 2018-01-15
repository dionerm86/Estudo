<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlVeiculoRodoviarioMDFe.ascx.cs" Inherits="Glass.UI.Web.Controls.MDFe.ctrlVeiculoRodoviarioMDFe" %>

<table id="tabela" runat="server" class="table-linha" cellpadding="0" cellspacing="0">
    <tr class="dtvRow">
        <td class="dtvHeader">
            Veiculo Reboque
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpVeiculoReboque" runat="server" Width="250px" AppendDataBoundItems="true"
                DataSourceID="odsVeiculoReboque" DataTextField="Placa" DataValueField="Placa">
                <asp:ListItem Value="" Text="Selecione"></asp:ListItem>
            </asp:DropDownList>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfVeiculosReboque" runat="server" />

<asp:ObjectDataSource ID="odsVeiculoReboque" runat="server"
    TypeName="Glass.Data.DAL.VeiculoDAO" DataObjectTypeName="Glass.Data.Model.Veiculo"
    SelectMethod="ObterVeiculoPorTipo">
    <SelectParameters>
        <asp:Parameter Name="tipoVeiculo" DefaultValue="1" />
    </SelectParameters>
</asp:ObjectDataSource>