<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ValePedagioCteRod.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.ValePedagioCteRod" %>
<table runat="server" id="tabelaValePedagio" class="table-linha" cellpadding="1"
    cellspacing="1">
    <tr style="margin-bottom: 15px">
        <td class="dtvHeader">
            Fornecedor
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpFornecedor" runat="server" AppendDataBoundItems="True" DataSourceID="odsFornecedor"
                DataTextField="Nomefantasia" DataValueField="IdFornec">
                <asp:ListItem Value="selecione" Text="Selecione"></asp:ListItem>
            </asp:DropDownList>
            <%--<asp:CompareValidator ID="cmpFornecedor" runat="server" ClientValidationFunction="validaCampoVazioPedagio"
                ControlToValidate="drpFornecedor" ErrorMessage="Selecione o fornecedor de pedágio"
                ValidateEmptyText="true" ValidationGroup="c" Operator="NotEqual" ValueToCompare="selecione">*</asp:CompareValidator>--%>
        </td>
        <td class="dtvHeader">
            Número Compra
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox runat="server" ID="txtNumeroCompraPedagio" MaxLength="20" Width="210px"></asp:TextBox>
            <%--<asp:CustomValidator ID="ctvNumeroCompraPedagio" runat="server" ClientValidationFunction="validaCampoVazioPedagio"
                ControlToValidate="txtNumeroCompraPedagio" ErrorMessage="Indique o número de compra de pedágio"
                ValidateEmptyText="true" ValidationGroup="c">*</asp:CustomValidator>--%>
        </td>
        <td class="dtvHeader">
            CNPJ Comprador
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox runat="server" ID="txtCnpjComprador" Width="100px"></asp:TextBox>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfIdFornecedor" runat="server" />
<asp:HiddenField ID="hdfNumeroCompra" runat="server" />
<asp:HiddenField ID="hdfCnpjComprador" runat="server" />
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFornecedor" runat="server" SelectMethod="GetOrdered"
    TypeName="Glass.Data.DAL.FornecedorDAO" />
