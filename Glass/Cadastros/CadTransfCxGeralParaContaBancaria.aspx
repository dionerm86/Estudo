<%@ Page Title="Transferência Cx. Geral para Conta Bancária" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadTransfCxGeralParaContaBancaria.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTransfCxGeralParaContaBancaria" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    var clicked = false;

    function onSave(btn) {
        if (clicked)
            return false;

        clicked = true;
    
        var idContaBanco = FindControl("drpContaBanco", "select").value;
        var valor = FindControl("txtValor", "input").value;
        var data = FindControl("ctrlData_txtData", "input").value;
        var obs = FindControl("txtObs", "textarea").value;

        if (idContaBanco == "" || idContaBanco == "0") {
            alert("Informe a conta bancária.");
            clicked = false;
            return false;
        }

        if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
            alert("Informe o valor.");
            clicked = false;
            return false;
        }

        btn.disabled = true;

        var response = CadTransfCxGeralParaContaBancaria.Transferir(idContaBanco, valor, data, obs).value;

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

        FindControl("txtValor", "input").value = "";
        FindControl("txtObs", "textarea").value = "";
        FindControl("drpContaBanco", "select").value = "";

        clicked = false;

        return false;
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Conta Bancária" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                DataTextField="DescricaoSaldo" DataValueField="IdContaBanco" AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Valor:" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtValor" runat="server" Width="100px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Data:" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlData" runat="server" ReadOnly="ReadWrite" ExibirHoras="false"
                                ValidateEmptyText="true" DataString='<%# Bind("DataEntregaString") %>' />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Observação" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <asp:Button ID="btnTransferir" runat="server" Text="Transferir" OnClientClick="return onSave(this);" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
