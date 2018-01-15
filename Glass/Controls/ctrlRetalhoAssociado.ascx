<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlRetalhoAssociado.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlRetalhoAssociado" %>

<div id="div_<%= this.ClientID %>" style="display: none; width: 200px">
    <table class="pos">
        <tr>
            <td width="100%">
                <asp:Label ID="lblDescricao" runat="server" style="display: none"></asp:Label>
            </td>
            <td>
                <asp:ImageButton ID="imgSelecionar" runat="server" AlternateText="Retalhos"
                    ImageUrl="~/Images/Pesquisar.gif" style="display: none" />
            </td>
        </tr>
    </table>
    <table id="table_<%= this.ClientID %>" style="display: none"></table>
    <asp:HiddenField ID="hdfIdsRetalhosDisponiveis" runat="server" />
    <asp:HiddenField ID="hdfIdsRetalhosAssociados" runat="server" />
</div>
<script type="text/javascript">
    var qtdImprimir = <%= this.ClientID %>.CampoQtdeImprimir;
    qtdImprimir = document.getElementById(qtdImprimir);
    
    if (!!qtdImprimir)
    {
        var change = qtdImprimir.getAttribute("onChange");
        change = "var input = document.getElementById('table_<%= this.ClientID %>').getElementsByTagName('input'); if (input.length > 0) validarRetalhos(input[0]); " + change;
        qtdImprimir.setAttribute("onChange", change);
    }
    
    var imgSelecionar = document.getElementById("<%= imgSelecionar.ClientID %>");
    imgSelecionar.setAttribute("onclick", "<%= this.ClientID %>.ExibirRetalhos(this); return false");
</script>
