<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetMotivoCancEtiqueta.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetMotivoCancEtiqueta" Title="Cancelamento de Etiqueta" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        var click = false;

        function confirmarCancelamento()
        {
            if (click)
                return false;

            var idImpressao = '<%= Request["IdImpressao"] %>';
            var tipoImpressao = '<%= Request["tipo"] %>';
            var idPedido = FindControl("txtNumeroPedido", "input") == null ? "" : FindControl("txtNumeroPedido", "input").value;
            var numeroNFe = FindControl("txtNumeroNFe", "input") == null ? "" : FindControl("txtNumeroNFe", "input").value;
            var planoCorte = FindControl("txtPlanoCorte", "input") == null ? "" : FindControl("txtPlanoCorte", "input").value;
            var motivo = FindControl("txtMotivo", "textarea").value;

            if (!confirm("Tem certeza que deseja cancelar esta impress�o?"))
                return false;

            if (idPedido != null)
            {
                var respLiberacao = SetMotivoCancEtiqueta.VerificaLiberacao(idImpressao, idPedido).value.split("|");

                if (respLiberacao[0] == "Erro")
                {
                    alert(respLiberacao[1]);
                    return false;
                }

                var respPecaProducao = SetMotivoCancEtiqueta.VerificaImpressaoEmProducao(idImpressao, idPedido).value.split("|");
                if (respPecaProducao[0] == "Erro" && !confirm(respPecaProducao[1]))
                    return false;

                //var respPecaReposta = SetMotivoCancEtiqueta.VerificaPecaResposta(idImpressao, idPedido).value.split("|");
                //if (respPecaReposta[0] == "Erro" && !confirm(respPecaReposta[1]))
                //    return false;

                // Obriga informar o c�digo do pedido ou o plano de corte caso a impress�o seja de pedido.
                if (((idPedido == "" || isNaN(parseInt(idPedido, 10))) && planoCorte == "") && tipoImpressao == "1")
                {
                    if (empresaUsaPlanoCorte)
                        alert("Informe o n�mero do pedido que ser� cancelado desta impress�o, digite '0' para cancelar toda a impress�o ou informe o plano de corte que ser� cancelado.");
                    else
                        alert("Informe o n�mero do pedido que ser� cancelado desta impress�o ou digite '0' para cancelar toda a impress�o");

                    return false;
                }
            }

            if (tipoImpressao == "2") {
                // Obriga informar o n�mero da NFe
                if (numeroNFe == "" || isNaN(parseInt(numeroNFe, 10))) {
                    alert("Informe o n�mero da NFe que ser� cancelada desta impress�o ou digite '0' para cancelar toda a impress�o");
                    return false;
                }
            
                var respChapaProducao = SetMotivoCancEtiqueta.VerificaImpressaoEmProducaoNF(idImpressao, numeroNFe).value.split("|");
                if (respChapaProducao[0] == "Erro") {
                    alert(respChapaProducao[1]);
                    return false;
                }
            }

            if (motivo == "") {
                alert("Informe o motivo do cancelamento.");
                return false;
            }

            click = true;

            bloquearPagina();
            desbloquearPagina(false);

            var resposta = SetMotivoCancEtiqueta.CancelarImpressao(idImpressao, planoCorte, idPedido, numeroNFe, motivo).value.split("|");

            if (resposta[0] == "Erro")
            {
                desbloquearPagina(true);
                alert(resposta[1]);
                click = false;
            }
            else
            {
                desbloquearPagina(true);
                window.opener.redirectUrl(window.opener.location.href);
                alert("Impress�o cancelada.");

                window.onbeforeunload = null;
                closeWindow();
            }
        }

        function naoDeixaSair()
        {
            return click ? "AGUARDE O T�RMINO DO CANCELAMENTO." : "Tem certeza que deseja sair?";
        }

        //Essa fun��o deve ser acionado assim que a p�gina terminar de carregar. 
        window.onbeforeunload = naoDeixaSair;
    
    </script>

    <table cellpadding="0" cellspacing="0" align="center">
        <tr>
            <td align="center">
                <table cellpadding="4" cellspacing="0">
                    <tr>
                        <td align="center">
                            <table runat="server" id="dadosCanc">
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label2" runat="server" Text="* N�m. do Pedido:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumeroPedido" ToolTip="Informe o n�mero do pedido que ser� cancelado desta impress�o ou digite 0 para cancelar toda a impress�o."
                                            runat="server" Width="329px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label4" runat="server" Text="* N�m. NFe:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumeroNFe" ToolTip="Informe o n�mero da NFe que ser� cancelada desta impress�o ou digite 0 para cancelar toda a impress�o."
                                            runat="server" Width="329px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label3" runat="server" Text="Plano de corte:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPlanoCorte" ToolTip="Informe o plano de corte que ser� cancelado desta impress�o."
                                            runat="server" Width="329px"></asp:TextBox>
                                    </td>
                                </tr>
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
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClientClick="return confirmarCancelamento();"
                    Style="margin: 4px" />
                <asp:Button ID="btnCancelar" runat="server" Text="Voltar" OnClientClick="closeWindow();"
                    Style="margin: 4px" />
                <br />
                <br />
                <asp:Label ID="lblInfoCanc" runat="server" Text="* Informe o n�mero do pedido que ser� cancelado desta<br />impress�o ou digite &quot;0&quot; para cancelar toda a impress�o."></asp:Label>
                <asp:Label ID="lblInfoCancNFe" runat="server" Text="* Informe o n�mero da NFe que ser� cancelada desta<br />impress�o ou digite &quot;0&quot; para cancelar toda a impress�o." Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
