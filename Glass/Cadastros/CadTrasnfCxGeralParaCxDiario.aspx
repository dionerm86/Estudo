<%@ Page Title="Transfer�ncia Cx. Geral para Cx. Di�rio" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadTrasnfCxGeralParaCxDiario.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTrasnfCxGeralParaCxDiario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

<script type="text/javascript">

    var clicked = false;

    function onSave(btn) {
        if (clicked)
            return false;

        clicked = true;
        
        var idLoja = FindControl("drpLoja", "select").value;
        var valor = FindControl("txtValor", "input").value;
        var obs = FindControl("txtObs", "textarea").value;

        if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
            alert("Informe o valor do cr�dito");
            clicked = false;
            return false;
        }

        if (obs == "") {
            alert("Informe o motivo da transfer�ncia.");
            clicked = false;
            return false;
        }

        btn.disabled = true;

        var response = CadTrasnfCxGeralParaCxDiario.Transferir(idLoja, valor, obs).value;

        btn.disabled = false;

        if (response == null) {
            alert("Falha na requisi��o, recarregue a p�gina e tente novamente.");
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
                            <asp:Label ID="Label6" runat="server" Text="Cx Di�rio:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                            DataTextField="NomeFantasia" DataValueField="IdLoja">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Observa��o" Font-Bold="False"></asp:Label>
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" 
                    TypeName="Glass.Data.DAL.LojaDAO"></colo:VirtualObjectDataSource>
            </td>
        </tr>
        </table>
</asp:Content>