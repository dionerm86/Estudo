<%@ Page Title="Parar peça na produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="SetMotivoPararPecaProducao.aspx.cs" Inherits="Glass.UI.Web.Utils.SetMotivoPararPecaProducao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function validate() {

            var idProdPedProducao = GetQueryString("idProdPedProducao");

            if (idProdPedProducao == null) {
                alert("Nenhuma peça foi informada");
                return false;
            }

            var motivo = FindControl("txtMotivo", "textarea");

            if (motivo.value == "") {
                alert("Informe o motivo do cancelamento.");
                return false;
            }

            if (motivo.value.length > 500) {
                alert("O motivo deve ter no máximo 500 caracteres.");
                return false;
            }


            return true;
        }
    </script>

    <table cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <table cellpadding="4" cellspacing="0">
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label1" runat="server" Text="Motivo:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMotivo" runat="server" MaxLength="250" TextMode="MultiLine" Width="329px"></asp:TextBox>
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
