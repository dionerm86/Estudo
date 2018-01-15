<%@ Page Title="Observação Liberação/Faturamento/Entrega" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true" 
    CodeBehind="AlterarObsLiberacaoPedido.aspx.cs" Inherits="Glass.UI.Web.WebGlassParceiros.AlterarObsLiberacaoPedido" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

<script type="text/javascript"> 


</script>
    <table>
        <tr>
            <td align="center">
                <table>                   
                    <tr>
                        <td align="left" class="dtvHeader" nowrap="nowrap">
                            <asp:Label ID="Label32" runat="server" Text="Observação Liberação/Faturamento/Entrega"></asp:Label>
                        </td>
                        <td align="left" class="dtvAlternatingRow">
                            <asp:TextBox ID="txtObs" runat="server" MaxLength="500" Width="300px" 
                                Rows="5" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnSalvar" runat="server" Text="Salvar"  OnClick="btnSalvar_Click" />
                        </td>
                    </tr>
                    </table>
            </td>
        </tr>        
    </table>
</asp:Content>

