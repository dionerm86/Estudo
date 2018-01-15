<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetMotivoCanc.aspx.cs" Inherits="Glass.UI.Web.Utils.SetMotivoCanc"
    Title="Cancelamento de Pedido" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        var click = false;

        function validate()
        {
            if (click) return false;

            if (!confirm("Tem certeza que deseja cancelar este Pedido?"))
                return false;

            if (FindControl("txtMotivo", "textarea").value == "")
            {
                alert("Informe o motivo do cancelamento.");
                return false;
            }

            if (FindControl("txtMotivo", "textarea").value.length < 15 || FindControl("txtMotivo", "textarea").value.length > 200)
            {
                alert("O campo deve ter entre 15 e 200 caracteres.");
                return false;
            }

            var msgValidacao = SetMotivoCanc.ValidaPedido('<%= Request["IdPedido"] %>').value;

            if (msgValidacao != "")
            {
                if (!confirm(msgValidacao))
                    return false;
            }

            click = true;
            return true;
        }

        function confirmarCancelamento(idPedido, gerarCredito, motivo, data)
        {
            var resposta = SetMotivoCanc.Confirmar(idPedido, gerarCredito, motivo, data).value.split(";");

            if (resposta[0] == "Erro")
                alert(resposta[1]);
            else
                window.opener.redirectUrl(window.opener.location.href);

            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table width="100%" cellpadding="4" cellspacing="0">
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label1" runat="server" Text="Motivo:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMotivo" runat="server" MaxLength="250" TextMode="MultiLine" Width="329px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr id="gerarCredito" runat="server">
                                    <td align="center" colspan="2">
                                        <asp:CheckBox ID="chkGerarCredito" runat="server" Text="Gerar crédito para o cliente (não estorna os valores já pagos)" />
                                    </td>
                                </tr>
                            </table>
                            <table runat="server" id="estornoBanco">
                                <tr>
                                    <td>
                                        Data do Estorno Bancário
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataEstorno" runat="server" ReadOnly="ReadWrite" ValidateEmptyText="True" />

                                        <script type="text/javascript">
                                            FindControl("chkEstornar", "input").onclick();
                                        </script>

                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClick="btnConfirmar_Click"
                    OnClientClick="return validate();" Style="margin: 4px" />
            </td>
        </tr>
    </table>
</asp:Content>
