<%@ Page Title="Creditar Cx. Diário" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadCreditarCxDiario.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCreditarCxDiario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onSave(btn)
        {
            var idLoja = FindControl("drpLoja", "select").value;
            var idConta = FindControl("drpPlanoConta", "select").value;
            var valor = FindControl("txtValor", "input").value;
            var formaEntrada = FindControl("drpFormaSaida", "select").value;
            var obs = FindControl("txtObs", "textarea").value;

            if (idLoja == "" || idLoja == "0")
            {
                alert("Informe a loja.");
                return false;
            }

            if (idConta == "" || idConta == "0")
            {
                alert("Informe o plano de conta.");
                return false;
            }

            if (formaEntrada == "" || formaEntrada == "0")
            {
                alert("Informe a forma de entrada.");
                return false;
            }

            if (valor == "" || parseFloat(valor.replace(",", ".")) == 0)
            {
                alert("Informe o valor do crédito");
                return false;
            }

            btn.disabled = true;

            var response = CadCreditarCxDiario.Creditar(idLoja, idConta, valor, formaEntrada, obs).value;

            btn.disabled = false;

            if (response == null)
            {
                alert("Falha na requisição, recarregue a página e tente novamente.");
                return false;
            }

            response = response.split('\t');

            if (response[0] == "Erro")
            {
                alert(response[1]);
                return false;
            }

            alert(response[1]);

            FindControl("drpLoja", "select").selectedIndex = 0;
            FindControl("drpPlanoConta", "select").selectedIndex = 0;
            FindControl("txtValor", "input").value = "";
            FindControl("txtObs", "textarea").value = "";

            return false;
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="Plano de Conta:"></asp:Label>
                                    </td>
                                    <td colspan="3">
                                        <asp:DropDownList ID="drpPlanoConta" runat="server" DataSourceID="odsPlanoConta"
                                            DataTextField="DescrPlanoGrupo" DataValueField="IdConta" AppendDataBoundItems="True">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Loja:"></asp:Label>
                                    </td>
                                    <td colspan="3">
                                        <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                            DataValueField="IdLoja" AppendDataBoundItems="True">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Text="Valor:" Font-Bold="False"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtValor" runat="server" Width="100px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label6" runat="server" Text="Forma entrada:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpFormaSaida" runat="server">
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
                        <td align="center">
                            <asp:TextBox ID="txtObs" runat="server" MaxLength="300" Rows="3" TextMode="MultiLine"
                                Width="400px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnCreditar" runat="server" Text="Creditar" OnClientClick="return onSave(this);" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoConta" runat="server" SelectMethod="GetPlanoContas"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
