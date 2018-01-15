<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComponenteCte.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.ComponenteCte" %>
<table runat="server" id="tabela" class="table-linha" cellpadding="1" cellspacing="1">
    <tr>
        <td class="dtvHeader">
            Nome Componente
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtNomeComponente" runat="server" MaxLength="15" ></asp:TextBox>
        </td>
        <td class="dtvHeader">
            Valor Componente
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtValorComponente" runat="server" Width="140" MaxLength="20" onclick="mascaraValor(this, 2); return false;" ></asp:TextBox>
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfNomeComponente" runat="server" />
<asp:HiddenField ID="hdfValorComponente" runat="server" />
