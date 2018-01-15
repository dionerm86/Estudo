<%@ Page Title="Transferência entre Contas Bancárias" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadTransfContaBanco.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTransfContaBanco" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        var clicked = false;

        function onSave(btn) {
            if (!clicked)
                clicked = true;
            else
                return false;

            var idContaBancoOrigem = FindControl("drpContaBancoOrigem", "select").value;
            var idContaBancoDest = FindControl("drpContaBancoDest", "select").value;
            var valor = FindControl("txtValor", "input").value;
            var data = FindControl("ctrlData_txtData", "input").value;
            var obs = FindControl("txtObs", "textarea").value;

            if (idContaBancoOrigem == "" || idContaBancoOrigem == "0") {
                alert("Informe a conta bancária de origem.");
                clicked = false;
                return false;
            }

            if (idContaBancoDest == "" || idContaBancoDest == "0") {
                alert("Informe a conta bancária de destino.");
                clicked = false;
                return false;
            }

            if (idContaBancoOrigem == idContaBancoDest) {
                alert("A conta de origem e de destino devem ser diferentes.");
                clicked = false;
                return false;
            }

            if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
                alert("Informe o valor do crédito");
                clicked = false;
                return false;
            }

            btn.disabled = true;

            var response = CadTransfContaBanco.Transferir(idContaBancoOrigem, idContaBancoDest, valor, data, obs).value;

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

            FindControl("drpContaBancoOrigem", "select").selectedIndex = 0;
            FindControl("drpContaBancoDest", "select").selectedIndex = 0;
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
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Origem:" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBancoOrigem" runat="server" DataSourceID="odsContaBanco"
                                DataTextField="DescricaoSaldo" DataValueField="IdContaBanco" AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Destino:" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBancoDest" runat="server" DataSourceID="odsContaBanco"
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
                            <uc1:ctrlData ID="ctrlData" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataEntregaString") %>'
                                ValidateEmptyText="true" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label10" runat="server" Text="Observação:" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnTransferir" runat="server" Text="Transferir" OnClientClick="return onSave(this)" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
