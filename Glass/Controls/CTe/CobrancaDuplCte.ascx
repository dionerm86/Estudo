<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CobrancaDuplCte.ascx.cs"
    Inherits="Glass.UI.Web.Controls.CTe.CobrancaDuplCte" %>
<%@ Register Src="~/Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<table runat="server" id="tabela" class="table-linha" cellpadding="1" cellspacing="1">
    <tr>
        <td class="dtvHeader" nowrap="nowrap">
            Número Duplicata:
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtnumDupl" runat="server" MaxLength="60" Width="150px"></asp:TextBox>
        </td>
        <td class="dtvHeader" nowrap="nowrap">
            Valor Duplicata:
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtValorDupl" runat="server" CssClass="mascara-valor" MaxLength="22"
                Width="100px"></asp:TextBox>
        </td>
        <td class="dtvHeader" nowrap="nowrap">
            Data Vencimento:
        </td>
        <td class="dtvAlternatingRow">
            <asp:TextBox ID="txtData0" runat="server" Width="70px" MaxLength="10" />
            <asp:ImageButton ID="imgData" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                ToolTip="Alterar" />
            <asp:ImageButton CssClass="img-linha" ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton CssClass="img-linha" ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfNumDupl" runat="server" />
<asp:HiddenField ID="hdfDataVenc" runat="server" />
<asp:HiddenField ID="hdfValorDupl" runat="server" />
