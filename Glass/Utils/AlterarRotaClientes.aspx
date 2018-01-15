<%@ Page Title="Alterar Rota" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true"
    CodeBehind="AlterarRotaClientes.aspx.cs" Inherits="Glass.UI.Web.Utils.AlterarRotaClientes" %>
<asp:Content ID="menu" ContentPlaceHolderID="Menu" runat="server">
</asp:Content>
<asp:Content ID="pagina" ContentPlaceHolderID="Pagina" runat="server">

    <script type="text/javascript">

        window.onload = function () {
            var idsClienteAlt = FindControl("hdfIdsCliente", "input");

            // Pega os idsCliente da tela de clientes e carrega no hidden field desta tela
            if (idsClienteAlt.value == "") {
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
                            <asp:Label ID="lblRotaNova" runat="server" Text="Nova Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpRota" runat="server" AppendDataBoundItems="True" DataSourceID="odsRota"
                                DataTextField="CodInterno" DataValueField="IdRota">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <br/>
                <asp:Button ID="btnAlterarRota" runat="server" Text="Alterar" OnClick="btnAlterarRota_Click"
                    OnClientClick="if (!confirm(&quot;Deseja alterar a rota dos clientes filtrados?&quot;)) return false;" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hdfIdsCliente" runat="server" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.RotaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
