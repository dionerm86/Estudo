<%@ Page Title="Gerar Conferência de Pedido" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadPedidoEspelhoGerar.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadPedidoEspelhoGerar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            Número do Pedido:&nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" onkeypress="return soNumeros(event, true, true);"
                                runat="server" onkeydown="if (isEnter(event)) cOnClick('btnGerarEspelho', null);"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNumPedido" runat="server" ControlToValidate="txtNumPedido"
                                ErrorMessage="*"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <asp:Button ID="btnGerarEspelho" runat="server" OnClick="btnGerarEspelho_Click" Text="Gerar Conferência"
                                OnClientClick="bloquearPagina(); desbloquearPagina(false)" />
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
    </table>

    <script>
        FindControl("txtNumPedido", "input").focus();
    </script>

</asp:Content>
