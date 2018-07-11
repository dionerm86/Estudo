<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AlterarVendedorCli.aspx.cs"
    Inherits="Glass.UI.Web.Utils.AlterarVendedorCli" MasterPageFile="~/Layout.master" Title="Alterar Vendedor" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        window.onload = function()
        {
            var idsClienteAlt = FindControl("hdfIdsCliente", "input");

            // Pega os idsCliente da tela de clientes e carrega no hidden field desta tela
            if (idsClienteAlt.value == "" && window.opener.FindControl("hdfIdsCliente", "input")) {
                idsClienteAlt.value = window.opener.FindControl("hdfIdsCliente", "input").value;
                document.getElementById("form1").submit();
            }
        }
    </script>

    <table align="center">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label3" runat="server" Text="Novo Vendedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Label ID="Label4" runat="server" ForeColor="Red" 
                    Text="Antes de alterar o vendedor, certifique-se de ter clicado na lupa e filtrado os clientes na tela apÃ³s ter informado os filtros."></asp:Label>
                <br />
                <br />
                <asp:Button ID="btnAlterarVendedor" runat="server" Text="Alterar" OnClick="btnAlterarVendedor_Click"
                    OnClientClick="if (!confirm(&quot;Deseja alterar o vendedor dos clientes filtrados?&quot;)) return false;" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                O vendedor pode ficar em branco para que os clientes não tenham um vendedor associado.
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdsCliente" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
