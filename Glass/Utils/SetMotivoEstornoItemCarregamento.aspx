<%@ Page Title="Estorno de Item do Carregamento" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="SetMotivoEstornoItemCarregamento.aspx.cs" Inherits="Glass.UI.Web.Utils.SetMotivoEstornoItemCarregamento" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function validar() {

            var motivo = FindControl("txtMotivo", "textarea").value;
            var ids = GetQueryString("ids") == undefined ? "" : GetQueryString("ids");
            var idCarregamento = GetQueryString("idCarregamento") == undefined ? "" : GetQueryString("idCarregamento");

            if (motivo == null || motivo == "") {
                alert("Informe o motivo.");
                return false;
            }

            if ((ids == null || ids == "") && (idCarregamento == null || idCarregamento == "")) {
                alert("Nenhum item foi informado.");
                return false;
            }

            if (idCarregamento != null && idCarregamento != "" && !confirm('Deseja realmente estornar todos os itens do carregamento: ' + idCarregamento))
                return false;

            bloquearPagina();

            var ret = SetMotivoEstornoItemCarregamento.EstornoCarregamento(ids, idCarregamento, motivo).value.split(';');

            if (ret[0] == "Erro") {
                desbloquearPagina(true);
                alert("Falha ao estornar itens. " + ret[1]);
                return false;
            }

            desbloquearPagina(false);
            window.opener.AtualizaPagina();
            window.close();
        }
    
    </script>

    <table cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <table cellpadding="4" cellspacing="0">
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
        <tr>
            <td align="center">
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClientClick="return validar();" />
            </td>
        </tr>
    </table>
</asp:Content>
