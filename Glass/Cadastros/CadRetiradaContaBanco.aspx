<%@ Page Title="Retirada Conta Bancária" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadRetiradaContaBanco.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadRetiradaContaBanco" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    function onSave(btn) {
        var idConta = FindControl("drpPlanoConta", "select").value;
        var idContaBanco = FindControl("drpContaBanco", "select").value;
        var valor = FindControl("txtValor", "input").value;
        var data = FindControl("ctrlData_txtData", "input").value;
        var obs = FindControl("txtObs", "textarea").value;

        if (idConta == "" || idConta == "0") {
            alert("Informe o plano de conta.");
            return false;
        }

        if (idContaBanco == "" || idContaBanco == "0") {
            alert("Informe o plano de conta.");
            return false;
        }

        if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
            alert("Informe o valor da retirada.");
            return false;
        }

        btn.disabled = true;

        var response = CadRetiradaContaBanco.MarcarSaida(idConta, idContaBanco, valor, data, obs).value;

        btn.disabled = false;

        if (response == null) {
            alert("Falha na requisição, recarregue a página e tente novamente.");
            return false;
        }

        response = response.split('\t');

        if (response[0] == "Erro") {
            alert(response[1]);
            return false;
        }

        alert(response[1]);

        FindControl("drpPlanoConta", "select").selectedIndex = 0;
        FindControl("txtValor", "input").value = "";
        FindControl("txtObs", "textarea").value = "";

        return false;
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Referente a:" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpPlanoConta" runat="server" DataSourceID="odsPlanoConta"
                                DataTextField="DescrPlanoGrupo" DataValueField="IdConta" AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Conta Bancária:" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpContaBanco" runat="server" DataSourceID="odsContaBanco"
                                DataTextField="Descricao" DataValueField="IdContaBanco" AppendDataBoundItems="True">
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
                <asp:Button ID="btnMarcarSaida" runat="server" OnClientClick="return onSave(this);"
                    Text="Marcar Saída" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" SelectMethod="GetPlanoContas"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="2" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
