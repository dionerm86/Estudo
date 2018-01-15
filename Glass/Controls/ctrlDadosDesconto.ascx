<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlDadosDesconto.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlDadosDesconto" %>
<span id='<%= this.ClientID %>' style="display: none">
    <table class="pos" cellpadding="0" cellspacing="0" style="display: inline-table">
        <tr>
            <td style="color: Blue">
                Taxa Fast Delivery
                &nbsp;
            </td>
            <td style="color: Blue">
                <asp:Label ID="lblTaxaFastDelivery" runat="server" style="font-weight: 700"></asp:Label>
            </td>
            <td style="color: Blue">
                &nbsp;&nbsp;
                Valor Desconto Esperado
                &nbsp;
            </td>
            <td style="color: Blue">
                <asp:Label ID="lblDescontoEsperado" runat="server" style="font-weight: 700"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="color: Blue">
                Valor Final com Desconto
                &nbsp;
            </td>
            <td style="color: Blue">
                <asp:Label ID="lblValorFinal" runat="server" style="font-weight: 700"></asp:Label>
            </td>
            <td style="color: Blue">
                &nbsp;&nbsp;
                Valor Final Esperado
                &nbsp;
            </td>
            <td style="color: Blue">
                <asp:Label ID="lblValorFinalEsperado" runat="server" style="font-weight: 700"></asp:Label>
            </td>
        </tr>
    </table>
</span>