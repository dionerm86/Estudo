<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlRetalhoProducao.ascx.cs"
    Inherits="Glass.UI.Web.Controls.ctrlRetalhoProducao" %>
    
<%-- <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.js") %>'></script>
<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.min.js") %>'></script>
<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jlinq/jlinq.js") %>'></script>
<script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery.utils.js") %>'></script> --%>
    
<style type="text/css">
    .style1
    {
        width: 53px;
    }
</style>
<div style="margin-bottom:10px;margin-top:20px; text-align:center; font-size:small; font-weight:bold">
    <asp:Label ID="lblRetalho" runat="server" Text="Reaproveitamento de Retalhos"></asp:Label>
</div>
<table runat="server" id="tblRetalhos" class="pos" cellpadding="1" cellspacing="1" style="margin-top: 10px;" align="center">
    <tr>
        <td colspan="2">
            &nbsp;</td>
        <td style="font-size: small;">
            Largura:
        </td>
        <td style="padding-right: 2px">
            <asp:TextBox ID="txtLargura" Width="50px" runat="server" onchange="addDataCallback(this.id, 0)"
                OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
        </td>
        <td style="font-size: small;">
            Altura:
        </td>
        <td style="padding-right: 2px" class="style1">
            <asp:TextBox ID="txtAltura" Width="50px" runat="server" onchange="addDataCallback(this.id, 0)"
                OnKeyPress="return soNumeros(event, false, true)"></asp:TextBox>
        </td>
        <td style="font-size: small;">
            Qtde:
        </td>
        <td style="padding-right: 2px">
            <asp:TextBox ID="txtQuantidade" Width="50px" runat="server" onchange="addDataCallback(this.id, 0)"
                OnKeyPress="return soNumeros(event, true, true)"></asp:TextBox>
        </td>
        <td style="font-size: small;">
            Obs.:
        </td>
        <td style="padding-right: 2px">
            <asp:TextBox ID="txtObservacao" Width="300px" runat="server" onchange="addDataCallback(this.id, 0)"></asp:TextBox>
        </td>
        <td valign="top" style="padding-top: 3px">
            <asp:ImageButton ID="imgAdicionar" runat="server" ImageUrl="~/Images/Insert.gif"
                ToolTip="Adicionar Linha" />
            <asp:ImageButton ID="imgRemover" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                Style="display: none" />
        </td>
    </tr>
</table>
<asp:HiddenField ID="hdfAltura" runat="server" />
<asp:HiddenField ID="hdfLargura" runat="server" />
<asp:HiddenField ID="hdfQtde" runat="server" />
<asp:HiddenField ID="hdfObservacao" runat="server" />
<asp:HiddenField ID="hdfIdProdPed" runat="server" />
<asp:HiddenField ID="hdfRetalhos" runat="server" />
<asp:HiddenField ID="hdfIdProd" runat="server" />
        

<%--<input type="hidden" id="hdfRetalhos" />
--%>
<script type="text/javascript">
    atualizaDados();
</script>
