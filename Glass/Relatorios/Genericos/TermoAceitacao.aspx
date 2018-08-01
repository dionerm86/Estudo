<%@ Page Title="Impressão de Termo de Aceitação" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="TermoAceitacao.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Genericos.TermoAceitacao" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
<script type="text/javascript">

    var data;

    function openRpt() {
        var idPedido = FindControl("txtNumPedido", "input");

        if (idPedido != null && idPedido.value == "") {
            alert("Informe o número do pedido que será emitido o recibo.");
            return false;
        }

        var rel = openWindow(600, 800, "RelBase.aspx?postData=getPostData()");
        data = new Object();
        data["rel"] = "termoaceitacao";
        data["ped"] = (idPedido != null ? idPedido.value : "0");
        data["infAdic"] = FindControl("txtInfAdic", "textarea").value.replace(/\n/g, "\\n")

        return false;
    }

    function getPostData() {
        return data;
    }
     
</script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <table cellpadding="2" cellspacing="0">
                                <tr>
                                    <td id="labelPedido" runat="server">
                                        <asp:Label ID="Label3" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td id="pedido" runat="server">
                                        <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);" Width="60px"></asp:TextBox>
                                        </td>
                                    <td id="botaoBuscar" runat="server">
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" />
                                    </td>
                                </tr>
                            </table>
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
                <table id="tbInfPedido" runat="server">
                    <tr>
                        <td colspan="4" bgcolor="#DDDDDD">
                            <asp:Label ID="Label4" runat="server" Font-Bold="True" 
                                Text="Informações do Pedido"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label5" runat="server" Text="Pedido:" Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblPed" runat="server"></asp:Label>
                        </td>
                        <td align="right">
                            &nbsp;&nbsp;
                            <asp:Label ID="Label7" runat="server" Text="Vendedor:" Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblVend" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label6" runat="server" Text="Orçamento:" Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblOrca" runat="server"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label8" runat="server" Text="Cliente:" Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblCli" runat="server"></asp:Label>
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
                <table id="tbInfAdicional" runat="server">
                    <tr>
                        <td bgcolor="#DDDDDD">
                            <asp:Label ID="Label9" runat="server" Font-Bold="True" 
                                Text="Informações Adicionais"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtInfAdic" runat="server" Height="119px" TextMode="MultiLine" 
                                Width="522px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfPedido" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" 
                    OnClientClick="return openRpt();" Visible="False">
                    <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>

