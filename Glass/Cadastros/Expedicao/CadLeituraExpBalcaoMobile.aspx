<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CadLeituraExpBalcaoMobile.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.Expedicao.CadLeituraExpBalcaoMobile" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Exp. Balcão</title>

    <link href="../../Style/CarregamentoMobile.css" rel="stylesheet" />
    <link href="../../Style/gridView.css" type="text/css" rel="Stylesheet" />

    <script src="../../Scripts/Utils.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.4.4.js" type="text/javascript"></script>
    <script src="../../Scripts/ExpBalcao.js" type="text/javascript"></script>

    <style type="text/css">
        .auto-style1 {
            width: 17px;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <table>
                <tr>
                    <td>
                        <table class="boxAzul">
                            <tr>
                                <td>
                                    <asp:Label ID="Label3" runat="server" Text="Liberação" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtCodLiberacao" runat="server" Width="80px"
                                        onkeypress="if (isEnter(event)) return carregaLiberacao();"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <table class="boxAzul">
                            <tr>
                                <td>
                                    <asp:Label ID="Label1" runat="server" Text="Etiqueta" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtCodEtiqueta" runat="server" Width="80px"
                                        onkeypress="if (isEnter(event)) return efetuaLeitura();"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <table style="width: 95px; text-align: center;">
                            <tr>
                                <td>
                                    <asp:LinkButton ID="lnkLgout" runat="server" OnClick="lnkLgout_Click" Font-Bold="True" ForeColor="White">Sair</asp:LinkButton>

                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <table style="background-color: #74C0E6; margin-left: 3px;">
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="Label17" runat="server" Text="Cód Pedido " ForeColor="#0066FF"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtIdPedido" runat="server" Width="185px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                </td>
                                <td class="auto-style1">
                                    <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                        OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <asp:DetailsView runat="server" ID="dtvExpBalcao" DataSourceID="odsExpBalcao"
                AutoGenerateRows="False" GridLines="None" HorizontalAlign="Center" Width="100%"
                CellPadding="0" CellSpacing="0">
                <Fields>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <table>
                                <tr>
                                    <td>
                                        <table class="boxAzul">
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="Label2" runat="server" Text="Expedido" Font-Bold="True"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label4" runat="server" Text="Vidros: "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label5" runat="server" Text='<%# Eval("TotalPecasExpedidas") %>' Font-Bold="True" ForeColor="Green"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label6" runat="server" Text="Volumes: "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label7" runat="server" Text='<%# Eval("TotalVolumesExpedidos") %>' Font-Bold="True" ForeColor="Green"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label8" runat="server" Text="Peso: "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label9" runat="server" Text='<%# Eval("PesoTotalExpedido", "{0:N2}") %>' Font-Bold="True" ForeColor="Green"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <table class="boxAzul">
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="Label10" runat="server" Text="Pendente" Font-Bold="True"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label11" runat="server" Text="Vidros: "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label12" runat="server" Text='<%# Eval("TotalPecasPendentes") %>' Font-Bold="True" ForeColor="Red"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label13" runat="server" Text="Volumes: "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label14" runat="server" Text='<%# Eval("TotalVolumesPendentes") %>' Font-Bold="True" ForeColor="Red"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label15" runat="server" Text="Peso: "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label16" runat="server" Text='<%# Eval("PesoTotalPendente", "{0:N2}") %>' Font-Bold="True" ForeColor="Red"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        <table class="boxAzul">
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="Label39" runat="server" Text="Total" Font-Bold="True"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label40" runat="server" Text="Vidros: "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label41" runat="server" Text='<%# Eval("TotalPecas") %>' Font-Bold="True"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label42" runat="server" Text="Volumes: "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label43" runat="server" Text='<%# Eval("TotalVolumes") %>' Font-Bold="True"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label44" runat="server" Text="Peso: "></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="Label45" runat="server" Text='<%# Eval("PesoTotal", "{0:N2}") %>' Font-Bold="True"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>

        </div>

        <div style="display: none;">
            <asp:HiddenField ID="hdfTempoLogin" runat="server" />
            <asp:HiddenField ID="hdfFunc" runat="server" />

            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsExpBalcao" runat="server"
                TypeName="Glass.PCP.Negocios.IExpedicaoFluxo" SelectMethod="BuscaParaExpBalcao">
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtCodLiberacao" PropertyName="Text" Name="idLiberarPedido" Type="Int32" />
                    <asp:ControlParameter ControlID="txtIdPedido" PropertyName="Text" Name="idPedido" Type="Int32" />
                    <asp:Parameter Name="visualizar" />
                </SelectParameters>
            </colo:VirtualObjectDataSource>

            <audio id="sndOk" src="../../Images/ok.wav"></audio>
            <audio id="sndError" src="../../Images/error.wav"></audio>

        </div>

    </form>
    <script type="text/javascript">

        function setFocus() {
            if ($('#txtCodLiberacao').val() == "")
                $('#txtCodLiberacao').focus();
            else
                $('#txtCodEtiqueta').focus();
        }


        setFocus();

        var alertaPadrao = function (titulo, msg, tipo, altura, largura) {
            $('body').append('<a href="#" id="alerta-padrao"></a>');
            $('#alerta-padrao').m2brDialog({
                draggable: true,
                texto: msg,
                tipo: tipo,
                titulo: titulo,
                altura: altura,
                largura: largura,
                botoes: {
                    1: {
                        label: 'Fechar',
                        tipo: 'fechar'
                    }

                },
                unloadCallback: function () {
                    setFocus();
                }
            });
            $('#alerta-padrao')
            .click()
            .remove();
        };

        // Conta os minutos que o usuário está logado
        var countMin = 0;

        function manterLogado() {
            try {
                countMin += 2;

                var tempoLogin = FindControl("hdfTempoLogin", "input").value;

                // Verifica se o tempo de login está dentro do tempo máximo permitido para este setor
                if (tempoLogin >= countMin)
                    MetodosAjax.ManterLogado();
                else {
                    MetodosAjax.Logout();
                    redirectUrl(window.location.href);
                }
            }
            catch (err) {
                alert(err);
            }
        }


        try {
            // Se o tempo de login for igual a 0, não precisa contar quanto tempo o usuário está logado.
            if (FindControl("hdfTempoLogin", "input").value == 0)
                setInterval("MetodosAjax.ManterLogado()", 600000);
            else
                setInterval("manterLogado()", 120000);

        }
        catch (err) {
            alert(err);
        }

    </script>
</body>
</html>