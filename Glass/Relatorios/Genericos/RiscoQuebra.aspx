<%@ Page Title="Impressão de Risco de Quebra" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="RiscoQuebra.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Genericos.RiscoQuebra" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script>
    function openRpt() {
        var idPedido = FindControl("txtNumPedido", "input").value;
        var texto = FindControl("txtTexto", "textarea").value;

        if (idPedido == "") {
            alert("Informe o número do pedido que será emitido o recibo.");
            return false;
        }

        var validaDados = RiscoQuebra.ValidaDados(idPedido);
        if (validaDados.value != '') {
            alert(validaDados.value);
            return false;
        }

        openWindow(600, 800, "RelBase.aspx?rel=riscoquebra&idPedido=" + idPedido + "&texto=" + texto);

        return false;
    }
</script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);" Width="60px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label4" runat="server" Text="Texto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtTexto" runat="server" Rows="2" TextMode="MultiLine" 
                                Width="500px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;</td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();">
                    <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
                </td>
        </tr>
        </table>
</asp:Content>

