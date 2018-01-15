<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlLacreRodoviarioMDFe.ascx.cs" Inherits="Glass.UI.Web.Controls.MDFe.ctrlLacreRodoviarioMDFe" %>

<table id="tabela" runat="server" class="table-linha" cellpadding="0" cellspacing="0">
    <tr class="dtvRow">
        <td class="dtvHeader">
            Lacre
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtLacre" runat="server" MaxLength="20" Width="150px"></asp:TextBox>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfLacres" runat="server" />