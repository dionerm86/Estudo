<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlPedagioRodoviarioMDFe.ascx.cs"
    Inherits="Glass.UI.Web.Controls.MDFe.ctrlPedagioRodoviarioMDFe" %>

<table id="tabela" runat="server" class="table-linha" cellpadding="0" cellspacing="0">
    <tr class="dtvRow">
        <td class="dtvHeader">
            Fornecedor Pedágio
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpFornecedor" runat="server" Width="250px" AppendDataBoundItems="true"
                DataSourceID="odsFornecedor" DataValueField="IdFornec" DataTextField="Razaosocial">
                <asp:ListItem Value="" Text="Selecione"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td class="dtvHeader">
            Responsável Pagamento
        </td>
        <td class="dtvAlternatingRow">
            <asp:DropDownList ID="drpResponsavelPedagio" runat="server" Width="250px" AppendDataBoundItems="true"
                DataSourceID="odsResponsavelPedagio" DataTextField="Translation" DataValueField="Key">
                <asp:ListItem Value="" Text="Selecione"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr class="dtvRow">
        <td class="dtvHeader">
            Número Compra
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtNumeroCompra" runat="server" MaxLength="20" Width="150px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
        </td>
        <td class="dtvHeader">
            Valor Vale Pedágio
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtValorPedagio" runat="server" MaxLength="15" onclick="mascaraValor(this, 2); return false;"
                Width="150px"></asp:TextBox>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfFornecedor" runat="server" />
<asp:HiddenField ID="hdfResponsavel" runat="server" />
<asp:HiddenField ID="hdfNumeroCompra" runat="server" />
<asp:HiddenField ID="hdfValorPedagio" runat="server" />

<asp:ObjectDataSource ID="odsFornecedor" runat="server"
    TypeName="Glass.Data.DAL.FornecedorDAO" DataObjectTypeName="Glass.Data.Model.Fornecedor"
    SelectMethod="ObterFornecedoresAtivos"></asp:ObjectDataSource>
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsResponsavelPedagio" runat="server"
    TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
    <SelectParameters>
        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.ResponsavelEnum, Glass.Data" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
