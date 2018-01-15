<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InfoCargaCte.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.InfoCargaCte" %>
<table runat="server" id="tabelaInfoCargaCte" class="table-linha" cellpadding="1"
    cellspacing="1">
    <tr>
        <td class="dtvHeader" nowrap="nowrap">
            Tipo Unidade
            <%= ObtemTextoCampoObrigatorio(cvdrpTipoUnidade) %>
        </td>
        <td class="dtvAlternatingRow" valign="top" style="padding: 1px">            
            <asp:DropDownList ID="drpTipoUnidade" runat="server" Height="20px" Width="100px"
                AppendDataBoundItems="True" Enabled="true" Visible="true">
                <asp:ListItem Value="-1" Text="Selecione"></asp:ListItem>
                <asp:ListItem Value="0" Text="M3"></asp:ListItem>
                <asp:ListItem Value="1" Text="KG"></asp:ListItem>
                <asp:ListItem Value="2" Text="TON"></asp:ListItem>
                <asp:ListItem Value="3" Text="UNIDADE"></asp:ListItem>
                <asp:ListItem Value="4" Text="LITROS"></asp:ListItem>
                <asp:ListItem Value="5" Text="MMBTU"></asp:ListItem>
            </asp:DropDownList>
            <asp:CompareValidator ID="cvdrpTipoUnidade" ControlToValidate="drpTipoUnidade" runat="server"
                ErrorMessage="Selecionar tipo unidade" ValueToCompare="-1" ValidationGroup="c"
                Display="Dynamic" Operator="NotEqual">*</asp:CompareValidator>
        </td>
        <td class="dtvHeader" nowrap="nowrap">
            Tipo Medida
            <%= ObtemTextoCampoObrigatorio(rfvtxtTipoMedida) %>
        </td>
        <td class="dtvAlternatingRow" valign="top" style="padding-top: 3px">
            <asp:TextBox ID="txtTipoMedida" runat="server" Width="180px" MaxLength="20"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvtxtTipoMedida" runat="server" ControlToValidate="txtTipoMedida"
                ErrorMessage="Campo tipo medida não pode ser vazio" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
        </td>
        <td class="dtvHeader" nowrap="nowrap">
            Quantidade
            <%= ObtemTextoCampoObrigatorio(rfvtxtQuantidade) %>
        </td>
        <td class="dtvAlternatingRow" valign="top" style="padding-top: 3px">
            <asp:TextBox ID="txtQuantidade" runat="server" Width="140" onclick="mascaraValor(this, 4); return false;"
                MaxLength="19"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvtxtQuantidade" runat="server" ControlToValidate="txtQuantidade"
                ErrorMessage="Campo quantidade não pode ser vazio" ValidationGroup="c" Display="Dynamic">*</asp:RequiredFieldValidator>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
            <asp:HiddenField runat="server" ID="hdfIdInfoCarga" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfTipoUnidade" runat="server" />
<asp:HiddenField ID="hdfTipoMedida" runat="server" />
<asp:HiddenField ID="hdfQuantidade" runat="server" />
<asp:HiddenField runat="server" ID="hdfIdInfoCarga1" />
