<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LacreCteRod.ascx.cs" Inherits="Glass.UI.Web.Controls.CTe.LacreCteRod" %>

<table runat="server" id="tabela_lacre" class="pos" cellpadding="1" cellspacing="1">
    <tr>
        <td class="dtvHeader">
            Número Lacre
        </td>
        <td class="dtvAlternatingRow" valign="top" style="padding: 1px">
            <asp:TextBox ID="txtNumLacre" runat="server" MaxLength="20" Width="210px"></asp:TextBox>
            <%--<asp:RequiredFieldValidator ID="rfvValidaQtde" runat="server" ErrorMessage="Preencha o número do lacre" 
                Display="Dynamic" ControlToValidate="txtNumLacre" ValidationGroup="c">*</asp:RequiredFieldValidator>--%>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfNumLacre" runat="server" />
