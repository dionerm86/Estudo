<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlAverbacaoSeguroMDFe.ascx.cs" Inherits="Glass.UI.Web.Controls.MDFe.ctrlAverbacaoSeguroMDFe" %>

<table id="tabela" runat="server" class="table-linha" cellpadding="0" cellspacing="0">
    <tr class="dtvRow">
        <td class="dtvHeader">
            Número da Averbação
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtNumeroAverbacao" runat="server" MaxLength="40" Width="300px"></asp:TextBox>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfNumerosAverbacao" runat="server" />