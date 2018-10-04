<%@ Page Title="Consulta Produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.LstProducao" %>

<%@ Register Src="../../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlBenefSetor.ascx" TagName="ctrlBenefSetor" TagPrefix="uc2" %>
<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="../../Controls/ctrlLogPopup.ascx" TagName="ctrllogpopup" TagPrefix="uc4" %>
<%@ Register Src="../../Controls/ctrlImageCadProject.ascx" TagName="ctrlImageCadProject" TagPrefix="uc5" %>
<%@ Register Src="../../Controls/ctrlLstProdProducao.ascx" TagName="ctrlLstProdProducao" TagPrefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function exibirBenef(botao) {
            for (iTip = 0; iTip < 2; iTip++) {
                TagToTip('benef', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamentos', CLOSEBTN, true,
                CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 0, 0]);
            }
        }

        function abrirDetalhesReposicao(idProdPedProducao) {
            openWindow(600, 800, "../../Utils/DetalhesReposicaoPeca.aspx?idProdPedProducao=" + idProdPedProducao);
        }

        function openRpt(exportarExcel, setorFiltrado, roteiro, pedidos) {
            var idCarregamento = FindControl("txtIdCarregamento", "input").value;
            var idLiberarPedido = FindControl("txtNumLiberarPedido", "input").value;
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idPedidoImportado = FindControl("txtNumPedidoImportado", "input").value;
            var idImpressao = FindControl("txtNumImpressao", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCliente = FindControl("txtNome", "input").value;
            var codPedCli = FindControl("txtCodPedCli", "input").value;
            var codRota = FindControl("drpRota", "select").itens();
            var numEtiqueta = FindControl("txtNumEtiqueta", "input").value;
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var horaIni = FindControl("ctrlDataIni_txtHora", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var horaFim = FindControl("ctrlDataFim_txtHora", "input").value;
            var dataIniEnt = FindControl("ctrlDataIniEnt_txtData", "input").value;
            var dataFimEnt = FindControl("ctrlDataFimEnt_txtData", "input").value;
            var dataIniFabr = FindControl("ctrlDataIniFabr_txtData", "input").value;
            var dataFimFabr = FindControl("ctrlDataFimFabr_txtData", "input").value;
            var dataIniConfPed = FindControl("ctrlDataIniConfPed_txtData", "input").value;
            var dataFimConfPed = FindControl("ctrlDataFimConfPed_txtData", "input").value;
            var situacao = FindControl("drpSituacao", "select").itens();
            var situacaoPedido = FindControl("drpSituacaoPedido", "select").value;
            var idSetor = FindControl("drpSetor", "select").value;
            var tiposSituacoes = FindControl("drpTipoSituacoes", "select").value;
            var idsSubgrupos = FindControl("cbdSubgrupo", "select").itens();
            var tipoEntrega = FindControl("drpTipoEntrega", "select").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var pecasProdCanc = FindControl("cbdExibirPecas", "select").itens();
            var idFunc = FindControl("drpFuncionario", "select").value;
            var tipoPedido = FindControl("drpTipoPedido", "select").itens();
            var aguardExpedicao = FindControl("chkAguardExpedicao", "input").checked;
            var aguardEntrEstoque = FindControl("chkAguardEntrEstoque", "input").checked;
            var pecaParadaProducao = FindControl("chkPecaParadaProducao", "input").checked;
            var pecasRepostas = FindControl("chkPecasRepostas", "input").checked;
            var idCorVidro = FindControl("drpCorVidro", "select").value;
            var altura = FindControl("txtAltura", "input").value;
            var largura = FindControl("txtLargura", "input").value;
            var idsProc = FindControl("cbdProcesso", "select").itens();
            var idsApl = FindControl("cbdAplicacao", "select").itens();
            var idsBenef = FindControl("hdfBenef", "input").value;
            var espessura = FindControl("txtEspessura", "input").value;
            var planoCorte = FindControl("txtPlanoCorte", "input").value;
            var etiquetaChapa = FindControl("txtNumEtiquetaChapa", "input").value;
            var filtroSetor = setorFiltrado == true ? "&setorFiltrado=true" : "";
            var fastDelivery = FindControl("ddlFastDelivery", "select").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var produtoComposicao = FindControl("drpProdutoComposicao", "select").value;

            if (horaIni != "") dataIni = dataIni + " " + horaIni;
            if (horaFim != "") dataFim = dataFim + " " + horaFim;
            if (idImpressao == "") idImpressao = 0;
            if (altura == "") altura = 0;
            if (largura == "") largura = 0;

            var relatorio = roteiro ? "RelBase.aspx?rel=ProducaoPassou" : pedidos ? "RelBase.aspx?rel=ProducaoPedidos" :
                agrupar == "3" ? "Producao/RelBase.aspx?rel=ProducaoContagem" : "RelBase.aspx?rel=Producao";

            openWindow(600, 800, "../../Relatorios/" + relatorio + "&idCarregamento=" + idCarregamento + "&idLiberarPedido=" + idLiberarPedido + "&idPedido=" + idPedido +
                "&idPedidoImportado=" + idPedidoImportado + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCliente + "&dataIni=" + dataIni +
                "&dataFim=" + dataFim + "&dataIniEnt=" + dataIniEnt + "&espessura=" + espessura + "&dataIniFabr=" + dataIniFabr +
                "&dataFimFabr=" + dataFimFabr + "&dataFimEnt=" + dataFimEnt + "&dataIniConfPed=" + dataIniConfPed + "&dataFimConfPed=" + dataFimConfPed +
                "&situacao=" + situacao + "&situacaoPedido=" + situacaoPedido + "&idSetor=" + idSetor + "&tiposSituacoes=" + tiposSituacoes +
                "&idFunc=" + idFunc + "&tipoPedido=" + tipoPedido + "&idsSubgrupos=" + idsSubgrupos +
                "&tipoEntrega=" + tipoEntrega + "&idImpressao=" + idImpressao + "&codCliente=" + codPedCli + "&agrupar=" + agrupar +
                "&pecasProdCanc=" + pecasProdCanc + "&aguardExpedicao=" + aguardExpedicao + "&numEtiqueta=" + numEtiqueta + "&codRota=" + codRota +
                "&aguardEntrEstoque=" + aguardEntrEstoque + "&idCorVidro=" + idCorVidro + "&altura=" + altura + "&largura=" + largura +
                "&idsProc=" + idsProc + "&idsApl=" + idsApl + "&exportarExcel=" + exportarExcel + "&idsBenef=" + idsBenef +
                "&planoCorte=" + planoCorte + "&numEtiquetaChapa=" + etiquetaChapa + "&fastDelivery=" + fastDelivery +
                "&pecaParadaProducao=" + pecaParadaProducao + "&pecasRepostas=" + pecasRepostas + "&idLoja=" + idLoja + "&produtoComposicao=" + produtoComposicao + filtroSetor);
        }

        function openRptPed(idPedido, isReposicao, tipo, original) {
            var url = original ? "../../Relatorios/RelPedido.aspx?idPedido=" + idPedido :
                "../../Relatorios/RelBase.aspx?rel=PedidoPcp&idPedido=" + idPedido;

            openWindow(600, 800, url);
            return false;
        }

        function getCli(idCli) {
            var retorno = MetodosAjax.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function leuEtiqueta(txtNumEtiqueta) {
            if (txtNumEtiqueta == null || txtNumEtiqueta == undefined)
                return;

            txtNumEtiqueta.value = corrigeLeituraEtiqueta(txtNumEtiqueta.value);
        }

        var idCliente = '<%= Request["cliente"]%>';

        function openSetMotivPararPecaProducao(idProdPedProducao, pecaParadaProducao) {

            if (idProdPedProducao == null || idProdPedProducao == 0) {
                alert("Nenhuma peça foi informada.")
                return false;
            }

            var prod = LstProducao.GetDescProd(idProdPedProducao);

            if (prod.error != null) {
                alert(prod.error.description);
                return false;
            }

            if (!confirm("Tem certeza que deseja" + (pecaParadaProducao ? " retornar esta peça para " : " parar esta peça na ") + "produção?\n" + prod.value))
                return false;

            openWindow(200, 405, "../../Utils/SetMotivoPararPecaProducao.aspx?popup=true&idProdPedProducao=" + idProdPedProducao, null, true, false);

            return false;
        }

        function openLogEstornoCarregamento(idProdPedProducao)
        {
            openWindow(600, 800, '../../Utils/ShowEstornoCarregamento.aspx?idProdPedProducao=' + idProdPedProducao);
        }

        function exibirProdsComposicao(botao, idProdPedProducao) {

            var linha = document.getElementById("ppp_" + idProdPedProducao);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " Produtos da Composição";
        }

        var voltarPecaClicado = false;

        function voltarPeca(idProdPedProducao) {
            if (!voltarPecaClicado)
                voltarPecaClicado = true;
            else
                return false;

            if (!confirm('Confirma remoção desta peça desta situação?'))
            {
                voltarPecaClicado = false;
                return false;
            }

            var retornoVoltarPeca = LstProducao.VoltarPeca(idProdPedProducao);

            if (retornoVoltarPeca.error != null) {
                alert(retornoVoltarPeca.error.description);
                voltarPecaClicado = false;
                return false;
            }

            if (retornoVoltarPeca.value.split('|')[0] == "Erro")
            {
                alert(retornoVoltarPeca.value.split('|')[1]);
                voltarPecaClicado = false;
                return false;
            }

            cOnClick("imgPesq", null);
        }

        function limparFiltros() {
            document.location = 'LstProducao.aspx';
            return false;
        }

    </script>

    <style>
        .tbPesquisa
        {
            vertical-align: middle;
        }
        .tbPesquisa td
        {
            /*background-color: #FAFAFA;
            border: solid 1px #F0F0F0;*/
            display: table-cell;
            vertical-align: middle;
            white-space: nowrap;
            width: auto;
            margin: 0;
            padding: 0;
        }
        .tituloCampos
        {
            text-align: left;
        }
    </style>
    <asp:HiddenField ID="hdfRefresh" runat="server" />
    <%=
        Glass.UI.Web.IncluirTemplateTela.Script(
            "~/Vue/Producao/Templates/LstProducao.Filtros.html")
    %>
    <div id="app">
        <producao-filtros :filtro="filtro" :configuracoes="configuracoes"></producao-filtros>
    </div>
    <br /><br /><br /><br />


    <table width="100%">
        <tr runat="server" id="trFiltros">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="45px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" MaxLength="6" QueryString="idPedido"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td style='<%= IsLiberacao() ? "": "display: none" %>'>
                            <asp:Label ID="Label19" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td style='<%= IsLiberacao() ? "": "display: none" %>'>
                            <asp:TextBox ID="txtNumLiberarPedido" runat="server" Width="45px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" MaxLength="6" QueryString="idLiberarPedido"></asp:TextBox>
                        </td>
                        <td style='<%= IsLiberacao() ? "": "display: none" %>'>
                            <asp:ImageButton ID="ImageButton14" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td style='<%= EmpresaTrabalhaComOrdemCarga() ? "": "display: none" %>'>
                            <asp:Label ID="Label43" runat="server" Text="Carregamento" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td style='<%= EmpresaTrabalhaComOrdemCarga() ? "": "display: none" %>'>
                            <asp:TextBox ID="txtIdCarregamento" runat="server" Width="45px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" MaxLength="6" QueryString="idCarregamento"></asp:TextBox>
                        </td>
                        <td style='<%= EmpresaTrabalhaComOrdemCarga() ? "": "display: none" %>'>
                            <asp:ImageButton ID="ImageButton21" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label34" runat="server" Text="Ped. importado" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedidoImportado" runat="server" Width="45px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" MaxLength="6" QueryString="idPedidoImportado"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton15" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Pedido Cli./Ambiente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodPedCli" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" QueryString="codPedCli"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label25" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpRota" runat="server" DataSourceID="odsRota" DataTextField="CodInterno"
                                DataValueField="IdRota" Title="Selecione a rota" QueryString="codRota"></sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq7" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);" QueryString="idCliente"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="150px" QueryString="nomeCliente"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label173" runat="server" Text="Num. Impressão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumImpressao" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" QueryString="idImpressao"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label26" runat="server" ForeColor="#0066FF" Text="Num. Etiqueta"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumEtiqueta" runat="server" Width="100px" onkeypress="if (isEnter(event)) return leuEtiqueta(this);" QueryString="numEtiqueta"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" ForeColor="#0066FF" Text="Situação"></asp:Label>
                        </td>
                        <td>
                            <%--<asp:DropDownList ID="drpSituacao" runat="server" OnSelectedIndexChanged="drpSetor_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="3">Pendente</asp:ListItem>
                                <asp:ListItem Value="4">Pronta</asp:ListItem>
                                <asp:ListItem Value="5">Entregue</asp:ListItem>
                                <asp:ListItem Value="2">Perda</asp:ListItem>
                            </asp:DropDownList>--%>
                            <sync:CheckBoxListDropDown ID="drpSituacao" runat="server" CheckAll="False" QueryString="situacao">
                                <asp:ListItem Value="3">Pendente</asp:ListItem>
                                <asp:ListItem Value="4">Pronta</asp:ListItem>
                                <asp:ListItem Value="5">Entregue</asp:ListItem>
                                <asp:ListItem Value="2">Perda</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" ForeColor="#0066FF" Text="Setor"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSetor" runat="server" AppendDataBoundItems="True" DataSourceID="odsSetor"
                                DataTextField="Descricao" DataValueField="IdSetor" OnSelectedIndexChanged="drpSetor_SelectedIndexChanged"
                                 QueryString="idSetor">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="-1">Etiqueta não impressa</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblPeriodoSetor" runat="server" ForeColor="#0066FF" Text="Período (Setor)"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="True" QueryString="dataIni"/>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="True" QueryString="dataFim"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label36" runat="server" ForeColor="#0066FF" Text="Loja"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" QueryString="idLoja">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton19" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblTipoSituacoes" runat="server" ForeColor="#0066FF" Text="Produtos por etapa"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoSituacoes" runat="server" QueryString="tipoSituacoes">
                                <asp:ListItem Value="0">Apenas os produtos do setor atual</asp:ListItem>
                                <asp:ListItem Value="1">Apenas produtos que ainda não passaram por este setor</asp:ListItem>
                                <asp:ListItem Value="2">Incluir produtos que já passaram por este setor</asp:ListItem>
                                <asp:ListItem Value="3">Apenas produtos disponíveis para leitura neste setor no momento</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label29" runat="server" Text="Situação Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacaoPedido" runat="server" AppendDataBoundItems="True"
                                DataSourceID="odsSituacao" DataTextField="Descr" DataValueField="Id" QueryString="situacaoPedido">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq14" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Subgrupo"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSubgrupo" runat="server" DataSourceID="odsSubgrupo"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd" QueryString="idsSubgrupos">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq15" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="left" nowrap="nowrap">
                            <asp:Label ID="lblBenef" runat="server" ForeColor="#0066FF" Text="Beneficiamentos"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <img id="botaoExibirBenef" src="../../Images/gear_add.gif" border="0" style="cursor: pointer"
                                onclick="exibirBenef(this)" />
                            <div id="benef" style="display: none">
                                <uc2:ctrlBenefSetor ID="ctrlBenefSetor1" runat="server" FuncaoExibir="exibirBenef(document.getElementById('botaoExibirBenef'))" />
                            </div>
                            <input type="hidden" id="hdfBenef" runat="server" QueryString="idsBenef"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Vendedor"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True" QueryString="idFunc">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td align="left">
                            <asp:Label ID="Label5" runat="server" ForeColor="#0066FF" Text="Tipo Entrega"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpTipoEntrega" runat="server" DataSourceID="odsTipoEntrega"
                                DataTextField="Descr" DataValueField="Id" AppendDataBoundItems="True" QueryString="tipoEntrega">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:Label ID="lblPeriodoEntrega" runat="server" ForeColor="#0066FF" Text="Período (Entrega)"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataIniEnt" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" QueryString="dataIniEnt" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc3:ctrlData ID="ctrlDataFimEnt" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" QueryString="dataFimEnt" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label28" runat="server" ForeColor="#0066FF" Text="Largura"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLargura" runat="server" Width="36px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" MaxLength="5" QueryString="largura"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq12" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label27" runat="server" ForeColor="#0066FF" Text="Altura"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAltura" runat="server" Width="36px" onkeypress="return soNumeros(event, true, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" MaxLength="5" QueryString="altura"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq13" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" ForeColor="#0066FF" Text="Tipo Pedido"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="drpTipoPedido" runat="server" QueryString="tipoPedido" OnLoad="drpTipoPedido_Load">
                                <asp:ListItem Value="1">Venda</asp:ListItem>
                                <asp:ListItem Value="2">Produção</asp:ListItem>
                                <asp:ListItem Value="3">Mão-de-obra</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Exibir peças" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdExibirPecas" runat="server" CheckAll="False" Title="Tipo de peça"
                                Width="180px" QueryString="pecasProdCanc">
                                <asp:ListItem Value="0" Selected="True">Em produção</asp:ListItem>
                                <asp:ListItem Value="1">Canceladas (mão-de-obra)</asp:ListItem>
                                <asp:ListItem Value="2">Canceladas (venda)</asp:ListItem>
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblPeriodoFabrica" runat="server" ForeColor="#0066FF" Text="Período (Fábrica)"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataIniFabr" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" QueryString="dataIniFabr" />
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataFimFabr" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" QueryString="dataFimFabr"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq17" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label33" runat="server" ForeColor="#0066FF" Text="Espessura"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEspessura" runat="server" Width="35px" MaxLength="5" onkeypress="return soNumeros(event, false, true);"
                                onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" QueryString="espessura"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq18" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label31" runat="server" ForeColor="#0066FF" Text="Cor"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpCorVidro" runat="server" DataSourceID="odsCorVidro" DataTextField="Descricao"
                                DataValueField="IdCorVidro" AppendDataBoundItems="True" QueryString="idCorVidro">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq16" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label32" runat="server" ForeColor="#0066FF" Text="Processo"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdProcesso" runat="server" CheckAll="False" DataSourceID="odsProcesso"
                                DataTextField="CodInterno" DataValueField="IdProcesso" Title="Proc." Width="75px" QueryString="idsProc">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" ForeColor="#0066FF" Text="Aplicação"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdAplicacao" runat="server" CheckAll="False" DataSourceID="odsAplicacao"
                                DataTextField="CodInterno" DataValueField="IdAplicacao" Title="Apl." Width="80px" QueryString="idsApl">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Plano de Corte"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPlanoCorte" runat="server" Width="80px" QueryString="planoCorte"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton12" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" ForeColor="#0066FF" Text="Etiqueta da Chapa"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumEtiquetaChapa" runat="server" Width="80px" onkeypress="if (isEnter(event)) return leuEtiqueta(this);" QueryString="numEtiquetaChapa"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton13" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Produto composto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpProdutoComposicao" runat="server" QueryString="produtoComposicao">
                                <asp:ListItem Value="0">Incluir produtos de composição</asp:ListItem>
                                <asp:ListItem Value="1">Somente produtos de composição</asp:ListItem>
                                <asp:ListItem Value="2" Selected="True">Não incluir produtos de composição</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton22" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAguardExpedicao" runat="server" Text="Peças aguardando expedição" QueryString="aguardExpedicao"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAguardEntrEstoque" runat="server" Text="Peças aguardando entrada no estoque" QueryString="aguardEntrEstoque"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton17" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkPecaParadaProducao" runat="server" Text="Peças com produção parada" QueryString="pecaParadaProducao"/>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td style='<%= ExibirPecasRepostas() %>'>
                            <asp:CheckBox ID="chkPecasRepostas" runat="server" Text="Peças que foram repostas" QueryString="pecasRepostas"/>
                        </td>
                        <td style='<%= ExibirPecasRepostas() %>'>
                            <asp:ImageButton ID="ImageButton18" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td style='<%= IsLiberacao() ? "": "display: none" %>'>
                            <asp:Label ID="Label42" runat="server" ForeColor="#0066FF" Text="Período (Conf. Ped.)"></asp:Label>
                        </td>
                        <td style='<%= IsLiberacao() ? "": "display: none" %>'>
                            <uc3:ctrlData ID="ctrlDataIniConfPed" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" QueryString="dataIniConfPed"/>
                        </td>
                        <td style='<%= IsLiberacao() ? "": "display: none" %>'>
                            <uc3:ctrlData ID="ctrlDataFimConfPed" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" QueryString="dataFimConfPed"/>
                        </td>
                        <td style='<%= IsLiberacao() ? "": "display: none" %>'>
                            <asp:ImageButton ID="ImageButton20" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" OnClick="imgPesq_Click" CausesValidation="False" />
                        </td>
                        <td>
                            <asp:Label ID="Label35" runat="server" ForeColor="#0066FF" Text="Fast Delivery"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlFastDelivery" runat="server" QueryString="fastDelivery">
                                <asp:ListItem Selected="True" Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Com Fast Delivery</asp:ListItem>
                                <asp:ListItem Value="2">Sem Fast Delivery</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton16" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label30" runat="server" Text="Agrupar impressão" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAgrupar" runat="server">
                                <asp:ListItem Text="" Value="0"></asp:ListItem>
                                <asp:ListItem Value="1">Cliente</asp:ListItem>
                                <asp:ListItem Value="2">Pedido</asp:ListItem>
                                <asp:ListItem Value="3">Número de peças</asp:ListItem>
                                <asp:ListItem Value="4">Prev. entrega</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <a href="javascript:limparFiltros();" class="button"><img border="0"
                                src="../../Images/ExcluirGrid.gif" /> Limpar filtros</a>
                            <input type="hidden" runat="server" ID="hdfPageIndex" QueryString="pageIndex" />
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
                <asp:GridView GridLines="None" ID="grdPecas" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataKeyNames="IdProdPedProducao" DataSourceID="odsPecas" EmptyDataText="Nenhuma peça encontrada."
                    OnDataBound="grdPecas_DataBound" OnLoad="grdPecas_Load" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnPageIndexChanging="grdPecas_PageIndexChanging"
                    AllowSorting="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/mais.gif" ToolTip="Exibir Produtos da Composição"
                                    Visible='<%# Eval("IsProdutoLaminadoComposicao") %>' OnClientClick='<%# "exibirProdsComposicao(this, " + Eval("IdProdPedProducao") + "); return false"%>' />
                            </ItemTemplate>
                            <EditItemTemplate></EditItemTemplate>
                            <FooterTemplate></FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" ImageUrl="~/Images/arrow_undo.gif" ToolTip="Remover peça desta situação" Visible='<%# Eval("RemoverSituacaoVisible") %>'
                                    OnClientClick='<%# "voltarPeca(" + Eval("IdProdPedProducao") + "); return false;"%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# (bool)Eval("ExibirRelatorioPedido") && Request["Producao"] != "1" %>'>
                                    <a href="#" onclick="openRptPed('<%# Eval("IdPedido") %>', false, 0, true); return false">
                                        <img border="0" src="../../Images/Relatorio.gif" /></a>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchPedidoProducao" runat="server">
                                    <a href="#" onclick="openRptPed('<%# Eval("IdPedido") %>', false, 0, false); return false">
                                        <img border="0" src="../../Images/script_go.gif" /></a>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../CadFotos.aspx?id=<%# Eval("IdPedido") %>&amp;tipo=pedido&#039;); return false;'>
                                    <img border="0px" src="../../Images/Clipe.gif" /></a></asp:PlaceHolder>
                                <uc1:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemPecaUrl") != null ? Eval("ImagemPecaUrl").ToString().Replace("../", "~/") : null %>'
                                    Visible='<%# !(bool)Eval("TemSvgAssociado") %>' />
                                <uc5:ctrlImageCadProject ID="ctrlImageCadProject" DiminuirMedidasPopUp="false" runat="server" IdProdPedEsp='<%# Glass.Conversoes.StrParaIntNullable(Eval("IdProdPed").ToString()).GetValueOrDefault(0) %>'
                                    Visible='<%# Eval("TemSvgAssociado") %>'/>
                                <asp:ImageButton ID="imgPararPecaProducao" runat="server" ImageUrl='<%# (bool)Eval("PecaParadaProducao") ? "~/Images/stop_red.png" : "~/Images/stop_blue.png" %>'
                                    OnClientClick='<%# "openSetMotivPararPecaProducao(" + Eval("IdProdPedProducao") + ", " + Eval("PecaParadaProducao").ToString().ToLower() + "); return false" %>'
                                    Visible='<%# Eval("ExibirPararPecaProducao") %>' Width="16" Height="16" ToolTip=<%# (bool)Eval("PecaParadaProducao") ? "Retornar peça para produção?" : "Parar peça na produção?" %>/>
                                <uc4:ctrllogpopup ID="ctrlLogPopup1" runat="server" Tabela="ProdPedProducao" IdRegistro='<%# Eval("idProdPedProducao") %>' />
                                <asp:HiddenField ID="hdfIdSetor" runat="server" Value='<%# Eval("IdSetor") %>' />
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Eval("Situacao") %>' />
                                <asp:HiddenField ID="hdfPedidoCancelado" runat="server" Value='<%# Eval("PedidoCancelado") %>' />
                                <asp:HiddenField ID="hdfCorLinha" runat="server" Value='<%# Eval("CorLinha") %>' />
                                <asp:ImageButton runat="server" ID="imgPopup" ImageUrl="~/Images/Nota.gif" Visible='<%# Eval("PecaReposta") %>'
                                    OnClientClick='<%# Eval("IdProdPedProducao", "abrirDetalhesReposicao({0}); return false") %>'
                                    ToolTip="Detalhes Reposição Peça" />
                                <asp:ImageButton ID="imgLogEstornoCarregamento" runat="server" OnClientClick='<%# "openLogEstornoCarregamento(" + Eval("IdProdPedProducao") + "); return false" %>'
                                    ImageUrl="~/Images/log_delete.jpg" ToolTip="Exibir log de estorno de carregamento" Visible='<%# Eval("EstornoCarregamentoVisible") %>' />
                                <asp:ImageButton id="imbTemLeituraSetorOculto" runat="server" ImageUrl="~/Images/exclamation.gif" OnClientClick="return false;"
                                    Visible='<%# Eval("TemLeituraSetorOculto") %>' ToolTip="Essa peça tem leitura em ao menos um setor oculto (Não exibido na consulta)." />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pedido">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdPedidoExibir") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("IdPedidoExibir") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="False"></asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo Ped.">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("SiglaTipoPedido") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("SiglaTipoPedido") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pedido Cli.">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodCliente") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("CodCliente") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cliente">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("IdNomeCliente") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("NomeCliente") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DescrProdLargAlt") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label21" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                <asp:Label ID="Label22" runat="server" Text='<%# Eval("DescrBeneficiamentos") %>'></asp:Label>
                                <asp:Label ID="Label23" runat="server" Font-Bold="True" Text='<%# Eval("LarguraAltura") %>'></asp:Label>
                                <br />
                                <asp:Label ID="Label24" runat="server" Font-Size="90%" Text='<%# Eval("DescrTipoPerdaLista") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Apl.">
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Proc.">
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox7" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr><asp:HiddenField ID="hdfPpp" runat="server" Value='<%# Eval("IdProdPedProducao") %>' />
                                <tr id="ppp_<%# Eval("IdProdPedProducao") %>" style="display: none;">
                                    <td colspan="37" align="center">
                                        <br />
                                        <uc5:CtrlLstProdProducao runat="server" ID="ctrlProdComposicao"  IdProdPedProducao='<%# Eval("IdProdPedProducao") %>' />
                                         <br />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPecas" runat="server" SelectMethod="GetListConsulta" OnSelected="odsPecas_Selected"
                    TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountConsulta" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:QueryStringParameter QueryStringField="pageIndex" Name="pageIndex"  />
                        <asp:QueryStringParameter QueryStringField="idLiberarPedido" Name="idLiberarPedido"  />
                        <asp:QueryStringParameter QueryStringField="idPedido" Name="idPedido" />
                        <asp:QueryStringParameter QueryStringField="idCarregamento" Name="idCarregamento" />
                        <asp:QueryStringParameter QueryStringField="idPedidoImportado" Name="idPedidoImportado" />
                        <asp:QueryStringParameter QueryStringField="idImpressao" Name="idImpressao" />
                        <asp:QueryStringParameter QueryStringField="codPedCli" Name="codPedCli" />
                        <asp:QueryStringParameter QueryStringField="codRota" Name="codRota"  />
                        <asp:QueryStringParameter QueryStringField="idCliente" Name="idCliente" />
                        <asp:QueryStringParameter QueryStringField="nomeCliente" Name="nomeCliente" />
                        <asp:QueryStringParameter QueryStringField="numEtiqueta" Name="numEtiqueta" />
                        <asp:QueryStringParameter QueryStringField="dataIni" Name="dataIni"  />
                        <asp:QueryStringParameter QueryStringField="dataFim" Name="dataFim"  />
                        <asp:QueryStringParameter QueryStringField="dataIniEnt" Name="dataIniEnt" />
                        <asp:QueryStringParameter QueryStringField="dataFimEnt" Name="dataFimEnt" />
                        <asp:QueryStringParameter QueryStringField="dataIniFabr" Name="dataIniFabr" />
                        <asp:QueryStringParameter QueryStringField="dataFimFabr" Name="dataFimFabr" />
                        <asp:QueryStringParameter QueryStringField="dataIniConfPed" Name="dataIniConfPed" />
                        <asp:QueryStringParameter QueryStringField="dataFimConfPed" Name="dataFimConfPed" />
                        <asp:QueryStringParameter QueryStringField="idSetor" Name="idSetor" />
                        <asp:QueryStringParameter QueryStringField="situacao" Name="situacao" />
                        <asp:QueryStringParameter QueryStringField="situacaoPedido" Name="situacaoPedido" />
                        <asp:QueryStringParameter QueryStringField="tipoSituacoes" Name="tipoSituacoes" />
                        <asp:QueryStringParameter QueryStringField="idsSubgrupos" Name="idsSubgrupos" />
                        <asp:QueryStringParameter QueryStringField="tipoEntrega" Name="tipoEntrega" />
                        <asp:QueryStringParameter QueryStringField="pecasProdCanc" Name="pecasProdCanc"  />
                        <asp:QueryStringParameter QueryStringField="idFunc" Name="idFunc" />
                        <asp:QueryStringParameter QueryStringField="tipoPedido" Name="tipoPedido" />
                        <asp:QueryStringParameter QueryStringField="idCorVidro" Name="idCorVidro" />
                        <asp:QueryStringParameter QueryStringField="altura" Name="altura" />
                        <asp:QueryStringParameter QueryStringField="largura" Name="largura" />
                        <asp:QueryStringParameter QueryStringField="espessura" Name="espessura" />
                        <asp:QueryStringParameter QueryStringField="idsProc" Name="idsProc" />
                        <asp:QueryStringParameter QueryStringField="idsApl" Name="idsApl" />
                        <asp:QueryStringParameter QueryStringField="aguardExpedicao" Name="aguardExpedicao" />
                        <asp:QueryStringParameter QueryStringField="fastDelivery" Name="fastDelivery"  />
                        <asp:QueryStringParameter QueryStringField="aguardEntrEstoque" Name="aguardEntrEstoque" />
                        <asp:QueryStringParameter QueryStringField="pecaParadaProducao" Name="pecaParadaProducao"  />
                        <asp:QueryStringParameter QueryStringField="idsBenef" Name="idsBenef" />
                        <asp:QueryStringParameter QueryStringField="planoCorte" Name="planoCorte" />
                        <asp:QueryStringParameter QueryStringField="numEtiquetaChapa" Name="numEtiquetaChapa" />
                        <asp:QueryStringParameter QueryStringField="pecasRepostas" Name="pecasRepostas"  />
                        <asp:QueryStringParameter QueryStringField="idLoja" Name="idLoja" />
                        <asp:QueryStringParameter QueryStringField="produtoComposicao" Name="produtoComposicao" />
                        <asp:Parameter Name="idProdPedProducaoParent" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSetor" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.SetorDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoPedido"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="idGrupo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEntrega" runat="server"
                    SelectMethod="GetTipoEntrega" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorVidro" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.CorVidroDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProcesso" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.EtiquetaProcessoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAplicacao" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.EtiquetaAplicacaoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource ID="odsRota" runat="server"
                    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges"
                    Culture="" MaximumRowsParameterName="" SelectMethod="GetRptRota" SkinID=""
                    StartRowIndexParameterName="" TypeName="Glass.Data.DAL.RotaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource runat="server" ID="odsLoja"
                    CacheExpirationPolicy="Absolute" ConflictDetection="OverwriteChanges"
                    Culture="" MaximumRowsParameterName="" SelectMethod="GetAll" SkinID=""
                    StartRowIndexParameterName="" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvContagemPecas" runat="server" DataSourceID="odsContagemPecas"
                    AutoGenerateRows="False" GridLines="None">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label13" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Blue"
                                                Text="Peças Prontas:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPecasProntas" runat="server" Font-Size="Medium" Text='<%# Eval("Prontas") %>'></asp:Label>
                                            <asp:Label ID="Label37" runat="server" Font-Size="Medium" Text='<%# string.Format("({0} / {1} m²)", Eval("TotMProntas"), Eval("TotMProntasCalc")) %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red"
                                                Text="Peças Pendentes:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPecasPendentes" runat="server" Font-Size="Medium" Text='<%# Eval("Pendentes") %>'></asp:Label>
                                            <asp:Label ID="Label38" runat="server" Font-Size="Medium" Text='<%# string.Format("({0} / {1} m²)", Eval("TotMPendentes"), Eval("TotMPendentesCalc")) %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="Label16" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="#009933"
                                                Text="Peças Entregues:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPecasEntregues" runat="server" Font-Size="Medium" Text='<%# Eval("Entregues") %>'></asp:Label>
                                            <asp:Label ID="Label39" runat="server" Font-Size="Medium" Text='<%# string.Format("({0} / {1} m²)", Eval("TotMEntregues"), Eval("TotMEntreguesCalc")) %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Gray"
                                                Text="Peças Perdidas:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPecasPerdidas" runat="server" Font-Size="Medium" Text='<%# Eval("Perdidas") %>'></asp:Label>
                                            <asp:Label ID="Label40" runat="server" Font-Size="Medium" Text='<%# string.Format("({0} / {1} m²)", Eval("TotMPerdidas"), Eval("TotMPerdidasCalc")) %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;&nbsp;
                                        </td>
                                        <td>
                                            <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Black"
                                                Text="Peças Canceladas:"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPecasCanceladas" runat="server" Font-Size="Medium" Text='<%# Eval("Canceladas") %>'></asp:Label>
                                            <asp:Label ID="Label41" runat="server" Font-Size="Medium" Text='<%# string.Format("({0} / {1} m²)", Eval("TotMCanceladas"), Eval("TotMCanceladasCalc")) %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsContagemPecas" runat="server"
                    SelectMethod="GetCountBySituacao" TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter QueryStringField="idLiberarPedido" Name="idLiberarPedido" />
                        <asp:QueryStringParameter QueryStringField="idPedido" Name="idPedido" />
                        <asp:QueryStringParameter QueryStringField="idCarregamento" Name="idCarregamento" />
                        <asp:QueryStringParameter QueryStringField="idPedidoImportado" Name="idPedidoImportado" />
                        <asp:QueryStringParameter QueryStringField="idImpressao" Name="idImpressao" />
                        <asp:QueryStringParameter QueryStringField="codRota" Name="codRota" />
                        <asp:QueryStringParameter QueryStringField="codPedCli" Name="codPedCli"  />
                        <asp:QueryStringParameter QueryStringField="idCliente" Name="idCliente" />
                        <asp:QueryStringParameter QueryStringField="nomeCliente" Name="nomeCliente" />
                        <asp:QueryStringParameter QueryStringField="numEtiqueta" Name="numEtiqueta" />
                        <asp:QueryStringParameter QueryStringField="dataIni" Name="dataIni" />
                        <asp:QueryStringParameter QueryStringField="dataFim" Name="dataFim" />
                        <asp:QueryStringParameter QueryStringField="dataIniEnt" Name="dataIniEnt"  />
                        <asp:QueryStringParameter QueryStringField="dataFimEnt" Name="dataFimEnt" />
                        <asp:QueryStringParameter QueryStringField="dataIniFabr" Name="dataIniFabr"  />
                        <asp:QueryStringParameter QueryStringField="dataFimFabr" Name="dataFimFabr" />
                        <asp:QueryStringParameter QueryStringField="dataIniConfPed" Name="dataIniConfPed" />
                        <asp:QueryStringParameter QueryStringField="dataFimConfPed" Name="dataFimConfPed" />
                        <asp:QueryStringParameter QueryStringField="idSetor" Name="idSetor"  />
                        <asp:QueryStringParameter QueryStringField="situacao" Name="situacao"  />
                        <asp:QueryStringParameter QueryStringField="situacaoPedido" Name="situacaoPedido" />
                        <asp:QueryStringParameter QueryStringField="tipoSituacoes" Name="tipoSituacoes" />
                        <asp:QueryStringParameter QueryStringField="idsSubgrupos" Name="idsSubgrupos" />
                        <asp:QueryStringParameter QueryStringField="tipoEntrega" Name="tipoEntrega" />
                        <asp:QueryStringParameter QueryStringField="pecasProdCanc" Name="pecasProdCanc" />
                        <asp:QueryStringParameter QueryStringField="idFunc" Name="idFunc" />
                        <asp:QueryStringParameter QueryStringField="tipoPedido" Name="tipoPedido" />
                        <asp:QueryStringParameter QueryStringField="idCorVidro" Name="idCorVidro" />
                        <asp:QueryStringParameter QueryStringField="altura" Name="altura" />
                        <asp:QueryStringParameter QueryStringField="largura" Name="largura"  />
                        <asp:QueryStringParameter QueryStringField="espessura" Name="espessura" />
                        <asp:QueryStringParameter QueryStringField="idsProc" Name="idsProc" />
                        <asp:QueryStringParameter QueryStringField="idsApl" Name="idsApl" />
                        <asp:QueryStringParameter QueryStringField="aguardExpedicao" Name="aguardExpedicao" />
                        <asp:QueryStringParameter QueryStringField="fastDelivery" Name="fastDelivery"  />
                        <asp:QueryStringParameter QueryStringField="aguardEntrEstoque" Name="aguardEntrEstoque" />
                        <asp:QueryStringParameter QueryStringField="idsBenef" Name="idsBenef" />
                        <asp:QueryStringParameter QueryStringField="planoCorte" Name="planoCorte" />
                        <asp:QueryStringParameter QueryStringField="numEtiquetaChapa" Name="numEtiquetaChapa" />
                        <asp:QueryStringParameter QueryStringField="pecaParadaProducao" Name="pecaParadaProducao" />
                        <asp:QueryStringParameter QueryStringField="pecasRepostas" Name="pecasRepostas" />
                        <asp:QueryStringParameter QueryStringField="idLoja" Name="idLoja" />
                        <asp:QueryStringParameter QueryStringField="produtoComposicao" Name="produtoComposicao" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblPecasEstoque" runat="server" Font-Bold="True" Font-Size="Medium"
                    ForeColor="Red"></asp:Label>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td align="right">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(false,false,false,false); return false;"> <img border="0" src="../../Images/Printer.png" /> Imprimir</asp:LinkButton>
            </td>
            <td>
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true,false,false,false); return false;"><img border="0"
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr id="trImpresaoSetorFiltrado" runat="server" visible="false">
            <td align="right">
                <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openRpt(false,true,false,false); return false;"> <img border="0" src="../../Images/Printer.png" /> Imprimir (Setor Selecionado)</asp:LinkButton>
            </td>
            <td>
                <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="openRpt(true,true,false); return false;"><img border="0"
                    src="../../Images/Excel.gif" /> Exportar para o Excel (Setor Selecionado)</asp:LinkButton>
            </td>
        </tr>
        <tr id="trImpressaoRoteiro" runat="server" visible="false">
            <td align="right">
                <asp:LinkButton ID="LinkButton3" runat="server" OnClientClick="openRpt(false,false,true,false); return false;"> <img border="0" src="../../Images/Printer.png" /> Imprimir (Roteiro)</asp:LinkButton>
            </td>
            <td>
                <asp:LinkButton ID="LinkButton4" runat="server" OnClientClick="openRpt(true,false,true,false); return false;"><img border="0"
                    src="../../Images/Excel.gif" /> Exportar para o Excel (Roteiro)</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:LinkButton ID="LinkButton5" runat="server" OnClientClick="openRpt(false,false,false,true); return false;"> <img border="0" src="../../Images/Printer.png" /> Imprimir (Pedidos)</asp:LinkButton>
            </td>
            <td>
                <asp:LinkButton ID="LinkButton6" runat="server" OnClientClick="openRpt(true,false,false,true); return false;"><img border="0"
                    src="../../Images/Excel.gif" /> Exportar para o Excel (Pedidos)</asp:LinkButton>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        leuEtiqueta(FindControl("txtNumEtiqueta", "input"));

        $(document.body).on("keydown", this,
     function (event) {
         if (event.keyCode == 116) {
             FindControl("hdfRefresh", "input").value = "0";
         }
     });

    </script>

    <asp:ScriptManager runat="server" LoadScriptsBeforeUI="False">
        <Scripts>
            <asp:ScriptReference Path="~/Vue/Producao/Componentes/LstProducao.Filtros.js" />
            <asp:ScriptReference Path="~/Vue/Producao/Componentes/LstProducao.js" />
        </Scripts>
    </asp:ScriptManager>
</asp:Content>
