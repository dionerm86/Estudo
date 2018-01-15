<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlFormaPagtoNotaFiscal.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlFormaPagtoNotaFiscal" %>

<table runat="server" id="tabela" class="pos" cellpadding="5" cellspacing="1">
    <tr>
        <td>
            <asp:Label runat="server" ID="lblValorReceb" Text="Valor Receb.:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtValorReceb" runat="server" Width="90px" onkeypress="return soNumeros(event, false, true);" OnChange='<%# this.ID +".pagamentoCallback();" %>'></asp:TextBox>
        </td>
        <td>
            <asp:Label runat="server" ID="lblFormaPagto" Text="Forma de Pagto.:"></asp:Label>
        </td>
        <td>
            <asp:DropDownList ID="drpFormaPagto" runat="server"  Width="150px" OnChange='<%# this.ID +".pagamentoCallback();drpFormaPagtoChanged(this); return false;" %>'>
                <asp:ListItem Text="" Value="" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Dinheiro" Value="1"></asp:ListItem>
                <asp:ListItem Text="Cheque" Value="2"></asp:ListItem>
                <asp:ListItem Text="Cartão de Crédito" Value="3"></asp:ListItem>
                <asp:ListItem Text="Cartão de Débito" Value="4"></asp:ListItem>
                <asp:ListItem Text="Crédito Loja" Value="5"></asp:ListItem>
                <asp:ListItem Text="Vale Alimentação" Value="10"></asp:ListItem>
                <asp:ListItem Text="Vale Refeição" Value="11"></asp:ListItem>
                <asp:ListItem Text="Vale Presente" Value="12"></asp:ListItem>
                <asp:ListItem Text="Vale Combustível" Value="13"></asp:ListItem>
                <asp:ListItem Text="Outros" Value="99"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td colspan="2">
            <asp:ImageButton ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
        </td>
    </tr>
    <tr id="dadosCartao" style="display:none;">
        <td>
            <asp:Label runat="server" ID="Label1" Text="CNPJ Credenciadora:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtCnpjCredenciadora" runat="server" Width="130px" OnChange='<%# this.ID +".pagamentoCallback();" %>'></asp:TextBox>
        </td>
        <td>
            <asp:Label runat="server" ID="lblBandeira" Text="Bandeira:"></asp:Label>

        </td>
        <td>
            <asp:DropDownList ID="drpBandeira" runat="server" Width="150px" OnChange='<%# this.ID +".pagamentoCallback();" %>'>
                <asp:ListItem Text="" Value="" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Visa" Value="1"></asp:ListItem>
                <asp:ListItem Text="Mastercard" Value="2"></asp:ListItem>
                <asp:ListItem Text="American Express" Value="3"></asp:ListItem>
                <asp:ListItem Text="Sorocred" Value="4"></asp:ListItem>
                <asp:ListItem Text="Outros" Value="99"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td>
            <asp:Label runat="server" ID="Label2" Text="Núm. Aut.:"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtNumAut" runat="server" Width="40px" OnChange='<%# this.ID +".pagamentoCallback();" %>'></asp:TextBox>
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfValoreReceb" runat="server" />
<asp:HiddenField ID="hdfFormaPagto" runat="server" />
<asp:HiddenField ID="hdfCnpj" runat="server" />
<asp:HiddenField ID="hdfBandeira" runat="server" />
<asp:HiddenField ID="hdfNumAut" runat="server" />