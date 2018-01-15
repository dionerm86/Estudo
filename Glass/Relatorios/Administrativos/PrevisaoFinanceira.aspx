<%@ Page Title="Previsão Financeira" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="PrevisaoFinanceira.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.PrevisaoFinanceira" %>

<%@ Register Src="../../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style type="text/css">
        .titulo
        {
            padding: 4px;
            white-space: nowrap;
            font-weight: bold;
        }
        .valor
        {
            padding-left: 8px;
            padding-right: 8px;
            text-align: center;
        }
        .borda
        {
            border: solid 1px Black;
        }
    </style>

    <script type="text/javascript">
        function alteraTipo(tipo)
        {
            var tituloReceber = document.getElementById("tituloContasReceber");
            var receber = document.getElementById("contasReceber");
            var tituloPagar = document.getElementById("tituloContasPagar");
            var pagar = document.getElementById("contasPagar");
            var separador = document.getElementById("separador");

            tituloReceber.style.display = tipo == 0 || tipo == 1 ? "" : "none";
            receber.style.display = tipo == 0 || tipo == 1 ? "" : "none";
            tituloPagar.style.display = tipo == 0 || tipo == 2 ? "" : "none";
            pagar.style.display = tipo == 0 || tipo == 2 ? "" : "none";
            separador.style.display = tipo == 0 ? "" : "none";
        }

        function openRpt(exportarExcel)
        {
            var data = FindControl("hdfData", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var tipo = FindControl("drpTipo", "select").value;
            var previsaoCustoFixo = FindControl("chkPrevisaoCustoFixo", "input").checked;
            var prevPedidos = FindControl("chkPrevisaoPedido", "input") != null ? FindControl("chkPrevisaoPedido", "input").checked : false;

            openWindow(600, 800, "RelBase.aspx?rel=PrevisaoFinanceira&data=" + data + "&idLoja=" + idLoja + "&tipo=" + tipo +
                "&prevPedidos=" + prevPedidos + "&previsaoCustoFixo=" + previsaoCustoFixo + "&exportarExcel=" + exportarExcel);
        }

        function openRptDetalhes(urlInicial, campoInicio, campoTermino, diasInicio, diasTermino, previsaoCustoFixo)
        {
            if (diasInicio != null)
            {
                var dataInicio = new Date();
                dataInicio.setDate(dataInicio.getDate() + diasInicio);
                dataInicio = dataInicio.getDate() + "/" + (dataInicio.getMonth() + 1) + "/" + dataInicio.getFullYear();
            }
            else
                var dataInicio = "";

            if (diasTermino != null)
            {
                var dataTermino = new Date();
                dataTermino.setDate(dataTermino.getDate() + diasTermino);
                dataTermino = dataTermino.getDate() + "/" + (dataTermino.getMonth() + 1) + "/" + dataTermino.getFullYear();
            }
            else
                var dataTermino = "";

            if (previsaoCustoFixo != null)
                var custoFixo = "&previsaoCustoFixo=" + previsaoCustoFixo + "&exibirSoPrevisaoCustoFixo=" + previsaoCustoFixo;
            else
                var custoFixo = "";

            var idLoja = FindControl("drpLoja", "select").value;

            openWindow(600, 800, urlInicial + "&idLoja=" + idLoja + "&" + campoInicio + "=" + dataInicio + "&" + campoTermino + "=" + dataTermino + custoFixo);
        }

        function openRptDetalhesReceb(diasInicio, diasTermino)
        {
            openRptDetalhes("../RelBase.aspx?rel=ContasReceber&idPedido=0&idLiberarPedido=0&idCli=0&tipoEntrega=0&sort=1" +
                "&renegociadas=false&idAcerto=0&idAcertoParcial=0&numeroNFe=0&agrupar=1&incluirCnab=true&refObra=true", "dtIni", "dtFim", diasInicio, diasTermino);
        }

        function openRptDetalhesPagar(diasInicio, diasTermino)
        {
            openRptDetalhes("../RelBase.aspx?rel=ContasPagar&idCompra=0&idFornec=0&cheques=0&agrupar=1", "dtIni", "dtFim", diasInicio, diasTermino);
        }

        function openRptDetalhesCheques(diasInicio, diasTermino, tipoCheque)
        {
            openRptDetalhes("../RelBase.aspx?Rel=ListaCheque&idPedido=0&idLiberarPedido=0&idAcerto=0&numCheque=0&situacao=10" +
                "&titular=&agencia=&conta=&idCli=&nomeCli=&idFornec=&nomeFornec=&ordenacao=0&reapresentado=true&tipo=" + tipoCheque +
                "&agrupar=true", "dataIni", "dataFim", diasInicio, diasTermino);
        }

        function openRptDetalhesPrevisaoCustoFixo(diasInicio, diasTermino)
        {
            openRptDetalhes("../RelBase.aspx?rel=ContasPagar&idCompra=0&idFornec=0&cheques=0&agrupar=1", "dtIni", "dtFim", diasInicio, diasTermino, true);
        }

        function openRptDetalhesPedidos(diasInicio, diasTermino)
        {
            openRptDetalhes("RelBase.aspx?rel=PrevFinanRecebPedido", "dataIni", "dataFim", diasInicio, diasTermino);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Tipos de contas" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" onchange="alteraTipo(this.value)">
                                <asp:ListItem Value="0">Receber/Pagar</asp:ListItem>
                                <asp:ListItem Value="1">Receber</asp:ListItem>
                                <asp:ListItem Value="2">Pagar</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            &nbsp;
                            <asp:CheckBox ID="chkPrevisaoCustoFixo" runat="server" Text="Exibir previsão de custo fixo"
                                OnCheckedChanged="PrevisaoCustoFixo_CheckChanged" AutoPostBack="true" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkPrevisaoPedido" Checked="true" runat="server" Text="Exibir previsão de pedidos em produção"
                                OnCheckedChanged="PrevisaoPedido_CheckChanged" AutoPostBack="true" />
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
                <span id="tituloContasReceber" class="subtitle1">Receber
                    <br />
                    <br />
                </span>
                <table id="contasReceber" cellpadding="3" cellspacing="0" visible="True" style="border-collapse: collapse">
                    <tr>
                        <td>
                        </td>
                        <td colspan="4" align="center" class="borda titulo">
                            Vencidas
                        </td>
                        <td rowspan="2" align="center" class="borda titulo">
                            Hoje
                        </td>
                        <td colspan="4" align="center" class="borda titulo">
                            A vencer
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="center" class="borda titulo">
                            Mais de 90 dias
                        </td>
                        <td align="center" class="borda titulo">
                            90 dias
                        </td>
                        <td align="center" class="borda titulo">
                            60 dias
                        </td>
                        <td align="center" class="borda titulo">
                            30 dias
                        </td>
                        <td align="center" class="borda titulo">
                            30 dias
                        </td>
                        <td align="center" class="borda titulo">
                            60 dias
                        </td>
                        <td align="center" class="borda titulo">
                            90 dias
                        </td>
                        <td align="center" class="borda titulo">
                            Mais de 90 dias
                        </td>
                    </tr>
                    <tr>
                        <td class="titulo borda">
                            Contas a receber
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblVencidasMais90DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton7" runat="server" OnClientClick="openRptDetalhesReceb(null, -91); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblVencidas90DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton44" runat="server" OnClientClick="openRptDetalhesReceb(-90, -61); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencidas60DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton8" runat="server" OnClientClick="openRptDetalhesReceb(-60, -31); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencidas30DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton9" runat="server" OnClientClick="openRptDetalhesReceb(-30, -1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblVencimentoHojeReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton10" runat="server" OnClientClick="openRptDetalhesReceb(0, 0); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencer30DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton11" runat="server" OnClientClick="openRptDetalhesReceb(1, 30); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencer60DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton12" runat="server" OnClientClick="openRptDetalhesReceb(31, 60); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencer90DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton38" runat="server" OnClientClick="openRptDetalhesReceb(61, 90); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencerMais90DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton13" runat="server" OnClientClick="openRptDetalhesReceb(91, null); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                    </tr>
                    <tr>
                        <td class="titulo borda">
                            Cheques terceiros
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblChequesVencidosMais90DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton15" runat="server" OnClientClick="openRptDetalhesCheques(null, -91, 2); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblChequesVencidos90DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton45" runat="server" OnClientClick="openRptDetalhesCheques(-90, -61, 2); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblChequesVencidos60DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton16" runat="server" OnClientClick="openRptDetalhesCheques(-60, -31, 2); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblChequesVencidos30DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton17" runat="server" OnClientClick="openRptDetalhesCheques(-30, -1, 2); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblChequesVencimentoHojeReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton18" runat="server" OnClientClick="openRptDetalhesCheques(0, 0, 2); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblChequesVencer30DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton19" runat="server" OnClientClick="openRptDetalhesCheques(1, 30, 2); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblChequesVencer60DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton20" runat="server" OnClientClick="openRptDetalhesCheques(31, 60, 2); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblChequesVencer90DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton39" runat="server" OnClientClick="openRptDetalhesCheques(61, 90, 2); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblChequesVencerMais90DiasReceb" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton21" runat="server" OnClientClick="openRptDetalhesCheques(91, null, 2); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                    </tr>
                    <tr id="trPrevisaoPedidos" runat="server">
                        <td class="titulo borda">
                            Pedidos em produção
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblPedidosVencidosMais90Dias" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton29" runat="server" OnClientClick="openRptDetalhesPedidos(null, -91); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblPedidosVencidos90Dias" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton30" runat="server" OnClientClick="openRptDetalhesPedidos(-90, -61); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblPedidosVencidos60Dias" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton46" runat="server" OnClientClick="openRptDetalhesPedidos(-60, -31); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblPedidosVencidos30Dias" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton31" runat="server" OnClientClick="openRptDetalhesPedidos(-30, -1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblPedidosVencimentoHoje" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton32" runat="server" OnClientClick="openRptDetalhesPedidos(0, 0); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblPedidosVencer30Dias" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton35" runat="server" OnClientClick="openRptDetalhesPedidos(1, 30); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblPedidosVencer60Dias" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton36" runat="server" OnClientClick="openRptDetalhesPedidos(31, 60); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblPedidosVencer90Dias" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton40" runat="server" OnClientClick="openRptDetalhesPedidos(61, 90); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblPedidosVencerMais90Dias" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton37" runat="server" OnClientClick="openRptDetalhesPedidos(91, null); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                    </tr>
                    <tr>
                        <td class="titulo borda">
                            Total
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblTotalVencMais90DiasReceb" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblTotalVenc90DiasReceb" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblTotalVenc60DiasReceb" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblTotalVenc30DiasReceb" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblTotalVencHojeReceb" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblTotalVencer30DiasReceb" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblTotalVencer60DiasReceb" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblTotalVencer90DiasReceb" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblTotalVencerMais90DiasReceb" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
                <span id="separador">
                    <br />
                    <br />
                    <br />
                </span><span id="tituloContasPagar" class="subtitle1">Pagar
                    <br />
                    <br />
                </span>
                <table id="contasPagar" cellpadding="3" cellspacing="0" visible="True" style="border-collapse: collapse">
                    <tr>
                        <td>
                        </td>
                        <td colspan="4" align="center" class="borda titulo">
                            Vencidas
                        </td>
                        <td rowspan="2" align="center" class="borda titulo">
                            Hoje
                        </td>
                        <td colspan="4" align="center" class="borda titulo">
                            A vencer
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="center" class="borda titulo">
                            Mais de 90 dias
                        </td>
                        <td align="center" class="borda titulo">
                            90 dias
                        </td>
                        <td align="center" class="borda titulo">
                            60 dias
                        </td>
                        <td align="center" class="borda titulo">
                            30 dias
                        </td>
                        <td align="center" class="borda titulo">
                            30 dias
                        </td>
                        <td align="center" class="borda titulo">
                            60 dias
                        </td>
                        <td align="center" class="borda titulo">
                            90 dias
                        </td>
                        <td align="center" class="borda titulo">
                            Mais de 90 dias
                        </td>
                    </tr>
                    <tr>
                        <td class="titulo borda">
                            Contas a pagar
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblVencidasMais90DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton1" runat="server" OnClientClick="openRptDetalhesPagar(null, -91); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblVencidas90DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton47" runat="server" OnClientClick="openRptDetalhesPagar(-91, -61); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblVencidas60DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton2" runat="server" OnClientClick="openRptDetalhesPagar(-60, -31); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencidas30DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton3" runat="server" OnClientClick="openRptDetalhesPagar(-30, -1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencimentoHojePagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton4" runat="server" OnClientClick="openRptDetalhesPagar(0, 0); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencer30DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton5" runat="server" OnClientClick="openRptDetalhesPagar(1, 30); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencer60DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton6" runat="server" OnClientClick="openRptDetalhesPagar(31, 60); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda" nowrap="nowrap">
                            <asp:Label ID="lblVencer90DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton41" runat="server" OnClientClick="openRptDetalhesPagar(61, 90); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblVencerMais90DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton14" runat="server" OnClientClick="openRptDetalhesPagar(91, null); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                    </tr>
                    <tr>
                        <td class="titulo borda">
                            Cheques próprios
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblChequesVencidosMais90DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton22" runat="server" OnClientClick="openRptDetalhesCheques(null, -91, 1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblChequesVencidos90DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton23" runat="server" OnClientClick="openRptDetalhesCheques(-90, -61, 1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblChequesVencidos60DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton48" runat="server" OnClientClick="openRptDetalhesCheques(-60, -31, 1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblChequesVencidos30DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton24" runat="server" OnClientClick="openRptDetalhesCheques(-30, -1, 1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblChequesVencimentoHojePagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton25" runat="server" OnClientClick="openRptDetalhesCheques(0, 0, 1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblChequesVencer30DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton26" runat="server" OnClientClick="openRptDetalhesCheques(1, 30, 1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblChequesVencer60DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton27" runat="server" OnClientClick="openRptDetalhesCheques(31, 60, 1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblChequesVencer90DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton42" runat="server" OnClientClick="openRptDetalhesCheques(61, 90, 1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblChequesVencerMais90DiasPagar" runat="server" Text=""></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton28" runat="server" OnClientClick="openRptDetalhesCheques(91, null, 1); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                    </tr>
                    <tr id="trPrevisaoCustoFixo" runat="server">
                        <td class="titulo borda">
                            Previsão de Custo Fixo
                        </td>
                        <td class="valor borda">
                            &nbsp;
                        </td>
                        <td class="valor borda">
                            &nbsp;
                        </td>
                        <td class="valor borda">
                            &nbsp;
                        </td>
                        <td class="valor borda">
                            &nbsp;
                        </td>
                        <td class="valor borda">
                            &nbsp;
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblPrevisaoCustoFixoVencer30DiasPagar" runat="server" ForeColor="#0066CC"></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton33" runat="server" OnClientClick="openRptDetalhesPrevisaoCustoFixo(1, 30); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblPrevisaoCustoFixoVencer60DiasPagar" runat="server" ForeColor="#0066CC"></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton34" runat="server" OnClientClick="openRptDetalhesPrevisaoCustoFixo(31, 60); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblPrevisaoCustoFixoVencer90DiasPagar" runat="server" ForeColor="#0066CC"></asp:Label>
                            &nbsp;<asp:ImageButton ID="ImageButton43" runat="server" OnClientClick="openRptDetalhesPrevisaoCustoFixo(61, 90); return false;"
                                ImageUrl="~/Images/Relatorio.gif" ToolTip="Detalhes" />
                        </td>
                        <td class="valor borda">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="titulo borda">
                            Total
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblTotalVencMais90DiasPagar" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblTotalVenc90DiasPagar" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblTotalVenc60DiasPagar" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblTotalVenc30DiasPagar" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblTotalVencHojePagar" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblTotalVencer30DiasPagar" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblTotalVencer60DiasPagar" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblTotalVencer90DiasPagar" runat="server"></asp:Label>
                        </td>
                        <td class="valor borda">
                            <asp:Label ID="lblTotalVencerMais90DiasPagar" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPrevFinancReceb" runat="server" SelectMethod="GetReceber"
                    TypeName="Glass.Data.RelDAL.PrevisaoFinanceiraDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="hdfData" DefaultValue="" Name="data" PropertyName="Value"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkPrevisaoPedido" PropertyName="Checked" Name="previsaoPedidos"
                            Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPrevFinancPagar" runat="server" SelectMethod="GetPagar"
                    TypeName="Glass.Data.RelDAL.PrevisaoFinanceiraDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="hdfData" DefaultValue="" Name="data" PropertyName="Value"
                            Type="String" />
                        <asp:ControlParameter ControlID="chkPrevisaoCustoFixo" Name="previsaoCustoFixo" PropertyName="Checked"
                            Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfData" runat="server" />
                <br />
                <br />
                <asp:LinkButton ID="lkbImprimir" runat="server" OnClientClick="openRpt(); return false;"><img border="0" 
                    src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
