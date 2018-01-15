<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VeiculoCte.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.VeiculoCte" %>
<table runat="server" id="tabela" class="table-linha" cellpadding="1" cellspacing="1">
    <tr>
        <td class="dtvHeader">
            Placa
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpPlaca" runat="server" Height="20px" Width="100px" AppendDataBoundItems="True"
                Enabled="true" Visible="true" 
                DataSourceID="odsVeiculo" DataTextField="PLACA" DataValueField="PLACA">
                <asp:ListItem Value="selecione" Text="Selecione"></asp:ListItem>
            </asp:DropDownList>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
        </td>
        <%--<td class="dtvHeader">
            &nbsp; Valor Frete
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtValorFrete" runat="server" CssClass="mascara-valor" MaxLength="50"
                Width="100px"></asp:TextBox>
            
        </td>--%>
    </tr>
</table>
<asp:HiddenField ID="hdfPlaca" runat="server" />
<%--<asp:HiddenField ID="hdfValorFrete" runat="server" />--%>

<colo:VirtualObjectDataSource culture="pt-BR" ID="odsVeiculo" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.VeiculoDAO">
</colo:VirtualObjectDataSource>
