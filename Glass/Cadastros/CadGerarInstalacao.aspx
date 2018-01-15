<%@ Page Title="Gerar Instalação" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadGerarInstalacao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadGerarInstalacao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function loadPedido()
        {
            openWindow(600, 800, "../Utils/SelPedido.aspx?tipo=4");
        }

        function setPedido(idPedido)
        {
            FindControl("txtPedido", "input").value = idPedido;
        }

        function validar()
        {
            var idPedido = FindControl("txtPedido", "input").value;
            var resposta = CadGerarInstalacao.ValidarPedido(idPedido).value.split(";");

            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return false;
            }

            var tipoInstalacao = FindControl("drpTipoInstalacao", "select").value;
            if (tipoInstalacao == "")
            {
                alert("Selecione o tipo de instalação.");
                return false;
            }

            if (!confirm('Gerar instalação para esse pedido?'))
                return false;

            var jaInstalado = CadGerarInstalacao.VerificarPedidoJaInstalado(idPedido).value;
            if (jaInstalado == "true") {
                alert("Todos os produtos do pedido " + idPedido + " já foram instalados, verifique as instalações desse padido!");
                return false;
            }

            var existe = CadGerarInstalacao.VerificarInstalacao(idPedido, tipoInstalacao).value;
            if (existe == "true" && !confirm("Já existe uma instalação aberta para esse pedido e com esse tipo. Deseja continuar?"))
                return false;

            return true;
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPedido" runat="server" onkeypress="return soNumeros(event, true, true)"
                                            Width="80px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imgPesq" runat="server" OnClientClick="loadPedido(); return false"
                                            ImageUrl="~/Images/Pesquisar.gif" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="left" nowrap="nowrap">
                                        Tipo de instalação
                                    </td>
                                    <td align="left" nowrap="nowrap">
                                        <asp:DropDownList ID="drpTipoInstalacao" runat="server" AppendDataBoundItems="True"
                                            DataSourceID="odsTipoInstalacao" DataTextField="Descr" DataValueField="Id">
                                            <asp:ListItem></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnGerar" runat="server" Text="Gerar instalação" OnClientClick="if (!validar()) return false;"
                                OnClick="btnGerar_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoInstalacao" runat="server" SelectMethod="GetTipoInstalacao"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
