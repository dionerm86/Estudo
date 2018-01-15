<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CadProducaoMobile.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.Producao.CadProducaoMobile" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Controle de Produção</title>
    <link href="../../Style/Producao.css" type="text/css" rel="Stylesheet" />
    <link href="../../Style/dhtmlgoodies_calendar.css" type="text/css" rel="Stylesheet" />
    <link href="../../Style/gridView.css" type="text/css" rel="Stylesheet" />
    <link href="../../Style/m2br.dialog.producao.css" rel="stylesheet" type="text/css" />

    <script src="../../Scripts/jquery-1.4.4.js" type="text/javascript"></script>

    <script src="../../Scripts/m2br.dialog.js" type="text/javascript"></script>

    <script src="../../Scripts/dhtmlgoodies_calendar.js" type="text/javascript"></script>

    <script src="../../Scripts/Utils.js" type="text/javascript"></script>

</head>
<body>
    <style title="text/css">
        .aba {
            position: relative;
            padding-bottom: 6px;
        }

            .aba span {
                padding: 6px;
                margin-right: 3px;
                cursor: pointer;
                background-color: White;
            }

        .painel {
            border: 1px solid gray;
            vertical-align: top;
            overflow: auto;
        }

        .main {
            width: 100%;
            background-color: #FFFFFF;
            vertical-align: top;
        }
    </style>

    <script type="text/javascript">

        function alteraClasseEstilo(tagName, estiloAtual, estiloNovo) {
            var nos = document.getElementsByTagName(tagName);
            for (i = 0; i < nos.length; i++)
                if (nos[i].className == estiloAtual)
                    nos[i].className = estiloNovo;
        }

        function alteraCorTela(corNova, corAtual) {
            alteraClasseEstilo("td", "tdTitulo" + corAtual, "tdTitulo" + corNova);
            alteraClasseEstilo("td", "tdConfirmacao" + corAtual, "tdConfirmacao" + corNova);
            alteraClasseEstilo("td", "title" + corAtual, "title" + corNova);
            alteraClasseEstilo("td", "subtitle" + corAtual, "subtitle" + corNova);
            alteraClasseEstilo("td", "subtitle1" + corAtual, "subtitle1" + corNova);
        }

        function keyPressPedidoNovo(e) {
            if (!isEnter(e))
                return soNumeros(e, true, true);
            else {
                FindControl("txtCodEtiqueta", "input").focus();
                return false;
            }
        }

        function atualizaSituacao() {

            var txtCodChapa = FindControl("txtCodChapa", "input");
            var txtCodEtiqueta = FindControl("txtCodEtiqueta", "input");
            var txtCodCavalete = FindControl("txtCodCavalete", "input");

            if (txtCodChapa != null && txtCodChapa.value == "") {
                txtCodChapa.focus();
                return false;
            }

            if (txtCodCavalete != null && txtCodCavalete.value == "") {
                txtCodCavalete.focus();
                return false;
            }

            if (txtCodEtiqueta.value == "") {
                txtCodEtiqueta.focus();
                return false;
            }

            cOnClick('btnMarcarPeca', 'input');
        }

    </script>

    <form id="form1" runat="server" defaultbutton="block">

        <div style="display: none;">
            <div id="usrMsg">
            </div>
        </div>

        <asp:Button ID="block" runat="server" Text="Button" Style="display: none" OnClientClick="return false;" />
        <asp:HiddenField ID="hdfDescrEtiqueta" runat="server" />

        <table class="main" id="tbMain" align="center" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <table style="width: 100%;" cellpadding="0" cellspacing="2">
                        <tr>
                            <td id="titulo" class="tdTituloAzul">
                                <table align="center" style="width: 100%;">
                                    <tr>
                                        <td>
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td style="width:20%;">&nbsp;</td>
                                                    <td class="titleAzul" style="width:60%;">
                                                        <asp:Label ID="lblTitulo" runat="server"></asp:Label>
                                                        <asp:HiddenField ID="hdfTitulo" runat="server" />
                                                    </td>
                                                    <td style="width:20%; text-align:right;">
                                                        <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click" CausesValidation="False">
                                                            <img border="0" src="../../Images/Logout.png" /></asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table cellpadding="0" cellspacing="0" style="width:100%; padding-right: 4px; margin-left: 2px">
                        <tr>
                            <td class="tdConfirmacaoAzul" align="left">
                                <asp:Table ID="tbMenu" runat="server">
                                </asp:Table>
                            </td>
                        </tr>
                    </table>
                    <table id="tbControleProducao" class="divisor" cellpadding="0" cellspacing="2" style="width:100%;">
                        <tr>
                            <td class="tdConfirmacaoAzul">
                                <table style="margin: 0 auto;">
                                    <tr>
                                        <td>
                                            <table id="tbLeitura">
                                                <tr>
                                                    <td align="center">
                                                        <table>
                                                            <tr>
                                                                <td class="subtitleAzul">
                                                                    <asp:Label ID="lblEtiqueta" runat="server" Text="Código Etiqueta"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <table>
                                                            <tr id="tbRota" style="display: none;" runat="server">
                                                                <td align="center">
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="Label3" runat="server" Text="Rota" Font-Size="Small"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:DropDownList ID="drpRota" runat="server" AppendDataBoundItems="True" DataSourceID="odsRota"
                                                                                    DataTextField="CodInterno" DataValueField="IdRota" Font-Size="Small">
                                                                                    <asp:ListItem></asp:ListItem>
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2" align="center">
                                                                                <asp:CustomValidator ID="ctvRota" runat="server" ErrorMessage="Informe a Rota" ClientValidationFunction="validaRota"
                                                                                    ControlToValidate="drpRota" Display="Dynamic" Font-Size="Small" OnServerValidate="ctvRota_ServerValidate"
                                                                                    ValidateEmptyText="True"></asp:CustomValidator>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr id="codChapa" runat="server" visible="false">
                                                                <td align="center">
                                                                    <asp:Label ID="Label2" runat="server" Text="Matéria-prima" Font-Size="Small"></asp:Label>
                                                                    <br />
                                                                    <asp:TextBox ID="txtCodChapa" runat="server" Font-Size="XX-Large" Width="230px" ForeColor="Green"
                                                                        onkeypress="if (isEnter(event)) { return atualizaSituacao(); }"></asp:TextBox>
                                                                    <br />
                                                                    <asp:RequiredFieldValidator ID="rfvChapa" runat="server" ErrorMessage="Informe a matéria-prima"
                                                                        ControlToValidate="txtCodChapa" Display="Dynamic" Font-Size="Small"></asp:RequiredFieldValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="Label1" runat="server" Text="Etiqueta" Font-Size="Small"></asp:Label>
                                                                    <br />
                                                                    <asp:TextBox ID="txtCodEtiqueta" runat="server" Font-Size="XX-Large" Width="230px"
                                                                        onkeypress="if (isEnter(event)) { this.value = corrigeLeituraEtiqueta(this.value); return atualizaSituacao(); }"></asp:TextBox>
                                                                    <br />
                                                                    <asp:RequiredFieldValidator ID="rfvEtiqueta" runat="server" ErrorMessage="Informe a etiqueta"
                                                                        ControlToValidate="txtCodEtiqueta" Display="Dynamic" Font-Size="Small"></asp:RequiredFieldValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center" id="tdCodCavalete" runat="server">
                                                                    <asp:Label ID="Label4" runat="server" Text="Cavalete" Font-Size="Small"></asp:Label>
                                                                    <br />
                                                                    <asp:TextBox ID="txtCodCavalete" runat="server" Font-Size="XX-Large" Width="230px"
                                                                        onkeypress="if (isEnter(event)) { return atualizaSituacao(); }"></asp:TextBox>
                                                                    <br />
                                                                    <asp:RequiredFieldValidator ID="rfvCavalete" runat="server" ErrorMessage="Informe o cavalete"
                                                                        ControlToValidate="txtCodCavalete" Display="Dynamic" Font-Size="Small"></asp:RequiredFieldValidator>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Label ID="lblDescrEtiqueta" runat="server" Font-Size="Small"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <br />
                                                                    <asp:Button ID="btnMarcarPeca" runat="server" Text="Marcar etiqueta" Font-Size="Small" CausesValidation="false"
                                                                        OnClick="btnMarcarPeca_Click" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">&nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <table>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:CheckBox ID="chkPerda" runat="server" Text="Marcar perda?" Font-Size="Small"
                                                                        OnCheckedChanged="chkPerda_CheckedChanged" AutoPostBack="True" />
                                                                    <span id="pedidoNovo" runat="server" visible="false">
                                                                        <br />
                                                                        <asp:CheckBox ID="chkPedidoNovo" runat="server" Text="Alterar pedido novo?" Font-Size="Small"
                                                                            OnCheckedChanged="chkPedidoNovo_CheckedChanged" AutoPostBack="True" />
                                                                    </span><span id="entradaEstoque" runat="server" visible="false">
                                                                        <br />
                                                                        <asp:CheckBox ID="chkEntradaEstoque" runat="server" Text="Marcar entrada no estoque"
                                                                            Font-Size="Small" AutoPostBack="True" OnCheckedChanged="chkEntradaEstoque_CheckedChanged" />
                                                                    </span>
                                                                    <br />
                                                                    <br />
                                                                </td>
                                                            </tr>
                                                            <tr id="dadosPerda" runat="server" visible="false">
                                                                <td align="center">
                                                                    <table>
                                                                        <tr align="left">
                                                                            <td style="font-size: small">Motivo
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="font-size: small">
                                                                                <asp:DropDownList ID="drpTipoPerda" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoPerda"
                                                                                    DataTextField="Descricao" DataValueField="IdTipoPerda" ValidationGroup="perda"
                                                                                    Font-Size="Small" AutoPostBack="True"
                                                                                    OnSelectedIndexChanged="drpTipoPerda_SelectedIndexChanged">
                                                                                    <asp:ListItem></asp:ListItem>
                                                                                </asp:DropDownList>
                                                                                <asp:Label ID="lblSubtipo" runat="server" Text="Subtipo:"></asp:Label>
                                                                                <asp:DropDownList ID="drpSubtipoPerda" runat="server"
                                                                                    AppendDataBoundItems="True" DataSourceID="odsSubtipoPerda"
                                                                                    DataTextField="Descricao" DataValueField="IdSubtipoPerda" Font-Size="Small">
                                                                                </asp:DropDownList>
                                                                                <br />
                                                                                <asp:CustomValidator ID="ctvTipoPerda" runat="server" ErrorMessage="Selecione o motivo da perda"
                                                                                    Font-Size="Small" ClientValidationFunction="validaTipoPerda" ControlToValidate="drpTipoPerda"
                                                                                    Display="Dynamic" ValidateEmptyText="True" ValidationGroup="perda" OnServerValidate="ctvTipoPerda_ServerValidate"></asp:CustomValidator>
                                                                                <div runat="server" id="perdaDefinitiva">
                                                                                    <asp:CheckBox ID="chkPerdaDefinitiva" runat="server" Text="Perda definitiva?" Font-Size="Small" />
                                                                                </div>
                                                                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPerda" runat="server" SelectMethod="GetBySetor"
                                                                                    TypeName="Glass.Data.DAL.TipoPerdaDAO">
                                                                                    <SelectParameters>
                                                                                        <asp:ControlParameter ControlID="hdfSetor" Name="idSetor" PropertyName="Value" Type="UInt32" />
                                                                                    </SelectParameters>
                                                                                </colo:VirtualObjectDataSource>
                                                                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubtipoPerda" runat="server"
                                                                                    SelectMethod="GetByTipoPerda" TypeName="Glass.Data.DAL.SubtipoPerdaDAO">
                                                                                    <SelectParameters>
                                                                                        <asp:ControlParameter ControlID="drpTipoPerda" Name="idTipoPerda"
                                                                                            PropertyName="SelectedValue" Type="UInt32" />
                                                                                    </SelectParameters>
                                                                                </colo:VirtualObjectDataSource>
                                                                            </td>
                                                                        </tr>
                                                                        <tr align="left">
                                                                            <td style="font-size: small">
                                                                                <asp:Label ID="lblObs" runat="server" Text="Observações"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" Width="250px"
                                                                                    Font-Size="Small" ValidationGroup="perda"></asp:TextBox>
                                                                                <br />
                                                                                <asp:CustomValidator ID="ctvObs" runat="server" ClientValidationFunction="validaObs"
                                                                                    Font-Size="Small" ControlToValidate="txtObs" Display="Dynamic" ErrorMessage="Digite a observação"
                                                                                    ValidateEmptyText="True" ValidationGroup="perda" OnServerValidate="ctvObs_ServerValidate"></asp:CustomValidator>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr id="dadosPedidoNovo" runat="server" visible="false">
                                                                <td align="center">
                                                                    <table>
                                                                        <tr>
                                                                            <td align="left" style="font-size: small">Núm. Pedido Novo
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:TextBox ID="txtPedidoNovo" runat="server" onchange="produtosPedido(this.value)"
                                                                                    Font-Size="Medium" onkeypress="if (isEnter(event)) produtosPedido(this.value); return keyPressPedidoNovo(event)"
                                                                                    Width="75px"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2" align="center">
                                                                                <asp:Label ID="lblProdutosPedido" runat="server" Font-Size="Small"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:HiddenField ID="hdfFunc" runat="server" />
                                            <asp:HiddenField ID="hdfSetor" runat="server" />
                                            <asp:HiddenField ID="hdfTempoLogin" runat="server" />
                                            <asp:HiddenField ID="hdfSituacao" runat="server" />
                                            <asp:HiddenField ID="hdfNumEtiqueta" runat="server" />
                                            <asp:HiddenField ID="hdfCorTela" runat="server" />
                                            <asp:HiddenField ID="hdfInformarRota" runat="server" />
                                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetRptRota" TypeName="Glass.Data.DAL.RotaDAO">
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

    </form>
</body>

<script type="text/javascript">

    // Conta os minutos que o usuário está logado
    var countMin = 0;

    function manterLogado() {
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

    // Altera a cor da tela
    alteraCorTela(FindControl("hdfCorTela", "input").value, "Azul");

    var chkPedidoNovo = document.getElementById("<%= chkPedidoNovo.ClientID %>");
    var chkEntradaEstoque = document.getElementById("<%= chkEntradaEstoque.ClientID %>");

    if (chkPedidoNovo != null && chkPedidoNovo.checked) {
        alterarPedidoNovo(chkPedidoNovo);
        produtosPedido(document.getElementById("<%= txtPedidoNovo.ClientID %>").value);
    }
    else if (chkEntradaEstoque != null && chkEntradaEstoque.checked)
        alterarEntradaEstoque(chkEntradaEstoque);

    var txtCodChapa = FindControl("txtCodChapa", "input");

    if (txtCodChapa != null)
        txtCodChapa.focus();
    else
        FindControl("txtCodEtiqueta", "input").focus();

    // Se o tempo de login for igual a 0, não precisa conta quanto tempo o usuário está logado.
    if (FindControl("hdfTempoLogin", "input").value == 0)
        setInterval("MetodosAjax.ManterLogado()", 600000);
    else
        setInterval("manterLogado()", 120000);

</script>

</html>
