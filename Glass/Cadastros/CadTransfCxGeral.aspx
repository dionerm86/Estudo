<%@ Page Title="Transferência Cx. Diário para Cx. Geral" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadTransfCxGeral.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTransfCxGeral" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

<script type="text/javascript">

    var clicked = false;

    function onSave(btn) {
        if (clicked)
            return false;

        clicked = true;
    
        var valor = FindControl("txtValor", "input").value;
        var formaSaida = FindControl("drpFormaSaida", "select").value;
        var obs = FindControl("txtObs", "textarea").value;

        if (formaSaida == "" || formaSaida == "0") {
            alert("Informe a forma de saída.");
            clicked = false;
            return false;
        }

        if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
            alert("Informe o valor da transferência");
            clicked = false;
            return false;
        }

        btn.disabled = true;

        var response = CadTransfCxGeral.Transferir(valor, formaSaida, obs).value;

        btn.disabled = false;

        if (response == null) {
            alert("Falha na requisição, recarregue a página e tente novamente.");
            clicked = false;
            return false;
        }

        response = response.split('\t');

        if (response[0] == "Erro") {
            alert(response[1]);
            clicked = false;
            return false;
        }

        alert(response[1]);

        FindControl("drpFormaSaida", "select").selectedIndex = 0;
        FindControl("txtValor", "input").value = "";
        FindControl("txtObs", "textarea").value = "";

        clicked = false;

        return false;
    }

</script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Text="Valor:" Font-Bold="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtValor" runat="server" Width="100px"
                                            onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td>
                            <asp:Label ID="Label6" runat="server" Text="Forma saída:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpFormaSaida" runat="server">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Value="1">Dinheiro</asp:ListItem>
                                            <asp:ListItem Value="2">Cheque</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Observação" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="3" 
                                TextMode="MultiLine" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnTransferir" runat="server" 
                    Text="Transferir" OnClientClick="return onSave(this);" />
            </td>
        </tr>
        </table>
</asp:Content>

