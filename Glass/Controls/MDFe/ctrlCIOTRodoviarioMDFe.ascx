<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlCIOTRodoviarioMDFe.ascx.cs"
    Inherits="Glass.UI.Web.Controls.MDFe.ctrlCIOTRodoviarioMDFe" %>

<table id="tabela" runat="server" class="table-linha" cellpadding="0" cellspacing="0">
    <tr class="dtvRow">
        <td class="dtvHeader">
            CIOT
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtCIOT" runat="server" MaxLength="12" Width="150px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
        </td>
        <td class="dtvHeader">
            CPF CNPJ Responsável
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtCPFCNPJ" runat="server" MaxLength="14" Width="150px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfCIOTs" runat="server" />
<asp:HiddenField ID="hdfCPFCNPJs" runat="server" />