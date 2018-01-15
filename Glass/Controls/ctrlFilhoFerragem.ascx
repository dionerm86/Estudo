<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlFilhoFerragem.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlFilhoFerragem" %>

<table runat="server" id="tabela" class="pos" cellpadding="1" cellspacing="1">
    <tr>
        <td>
            <asp:HiddenField ID="hdfIdFilhoFerragem" runat="server" />
        </td>
        <td>
            <asp:Label ID="lblDescricao" runat="server"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="txtDescricao" runat="server" onchange="selFilhoCallback(this.id)" Enabled='<%#this.PodeAlterar %>' ></asp:TextBox>
        </td>
        <td class='<%# "tdValor_"+ this.ID %>'>
            <asp:Label ID="lblValor" runat="server"></asp:Label>
        </td>
        <td class='<%# "tdValor_"+ this.ID %>'>
            <asp:TextBox ID="txtValor" runat="server" onkeypress="return soNumeros(event, false, true)"
                onchange="selFilhoCallback(this.id)"></asp:TextBox>
        </td>
        <td>
            <asp:ImageButton ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif" />
            <asp:ImageButton ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif" style="display: none" />
        </td>
    </tr>
</table>

<asp:HiddenField ID="hdfIdFerragem" runat="server" />
<asp:HiddenField ID="hdfIdsfilhosFerragem" runat="server" />
<asp:HiddenField ID="hdfDescricoes" runat="server" />
<asp:HiddenField ID="hdfValores" runat="server" />

<script>
    $(".tdValor_ctrlCodigoFerragem").css("display", "none");
    //$(".tdValor_ctrlConstanteFerragem").css("display", "none");
</script>