<%@ Page Title="Pedidos em Conferência" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstPedidosEspelho.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPedidosEspelho" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(idPedido, isReposicao, tipo)
        {
            if (!isReposicao)
                openWindow(600, 800, "../Relatorios/RelPedido.aspx?idPedido=" + idPedido + "&tipo=" + tipo);
            else
                openWindow(600, 800, "../Relatorios/RelPedidoRepos.aspx?idPedido=" + idPedido + "&tipo=" + tipo);

            return false;
        }

        function openRptComprar(idPedido)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ProdutosComprar&idPedido=" + idPedido);
        }

        function openRptProj(idPedido)
        {
            openWindow(600, 800, "../Cadastros/Projeto/ImprimirProjeto.aspx?idPedido=" + idPedido + "&pcp=1");
            return false;
        }

        function getRptQueryString()
        {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var idFunc = FindControl("drpVendedor", "select").value;
            var idFuncionarioConferente = FindControl("drpConferente", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var situacaoPedOri = FindControl("cbdSituacaoPedOri", "select").itens();
            var idsProcesso = FindControl("cbdProcesso", "select").itens();
            var dataIniEnt = FindControl("ctrlDataIniEnt_txtData", "input").value;
            var dataFimEnt = FindControl("ctrlDataFimEnt_txtData", "input").value;
            var dataIniFab = FindControl("ctrlDataIniFab_txtData", "input").value;
            var dataFimFab = FindControl("ctrlDataFimFab_txtData", "input").value;
            var dataIniFin = FindControl("ctrlDataIniFin_txtData", "input").value;
            var dataFimFin = FindControl("ctrlDataFimFin_txtData", "input").value;
            var dataIniConf = FindControl("ctrlDataIniConf_txtData", "input").value;
            var dataFimConf = FindControl("ctrlDataFimConf_txtData", "input").value;
            var dataIniEmis = FindControl("ctrlDataIniEmis_txtData", "input").value;
            var dataFimEmis = FindControl("ctrlDataFimEmis_txtData", "input").value;
            var pedidosSemAnexos = FindControl("chkPedidosSemAnexos", "input").checked;
            var pedidosAComprar = FindControl("chkPedidosAComprar", "input").checked;
            var situacaoCnc = FindControl("cblSituacaoCnc", "select").itens();
            var dataIniSituacaoCnc = FindControl("ctrlDataSitCncIni_txtData", "input").value;
            var dataFimSituacaoCnc = FindControl("ctrlDataSitCncFim_txtData", "input").value;
            var tipoPedido = FindControl("cbdTipoPedido", "select").itens();
            var idsRotas = FindControl("cblRota", "select").itens();
            var origemPedido = FindControl("drpOrigemPedido", "select").value;
            var pedidosConferidos = FindControl("drpPedConferido", "select").value;

            idPedido = idPedido == "" ? 0 : idPedido;
            idCliente = idCliente == "" ? 0 : idCliente;

            return "idPedido=" + idPedido + "&idCliente=" + idCliente + "&nomeCliente=" + nomeCli + "&idLoja=" + idLoja + "&idFunc=" + idFunc +
                "&idFuncionarioConferente=" + idFuncionarioConferente + "&situacao=" + situacao +
                "&situacaoPedOri=" + situacaoPedOri + "&idsProcesso=" + idsProcesso + "&dataIniEnt=" + dataIniEnt + "&dataFimEnt=" + dataFimEnt  + "&dataIniFab=" + dataIniFab +
                "&dataFimFab=" + dataFimFab + "&dataIniFin=" + dataIniFin + "&dataFimFin=" + dataFimFin + "&pedidosSemAnexos=" + pedidosSemAnexos + "&dataIniConf=" + dataIniConf +
                "&dataFimConf=" + dataFimConf + "&dataIniEmis=" + dataIniEmis + "&dataFimEmis=" + dataFimEmis + "&pedidosAComprar=" + pedidosAComprar + "&situacaoCnc=" + situacaoCnc +
                "&dataIniSituacaoCnc=" + dataIniSituacaoCnc + "&dataFimSituacaoCnc=" + dataFimSituacaoCnc + "&tipoPedido=" + tipoPedido+
                "&idsRotas=" + idsRotas + "&origemPedido=" + origemPedido + "&pedidosConferidos=" + pedidosConferidos;
        }

        function openRptLista()
        {
            if (validaFiltro())
            {
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=PedidoEspelho&" + getRptQueryString());
                return false;
            }
        }

        function openRptListaSel()
        {
            if (validaFiltro())
            {
                openWindow(600, 800, "../Utils/SelPedidoEspelhoImprimir.aspx?" + getRptQueryString());
                return false;
            }
        }

        function openRptPedidosAComprar()
        {
            if (validaFiltro())
                openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ProdutosComprar&buscarPedidos=true&" + getRptQueryString());
        }

        function getCli(idCli)
        {
            validaNumero(idCli);

            var retorno = LstPedidosEspelho.GetCli(idCli.value).value.split(';');

            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNome", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];
        }

        function openMotivoCanc(idPedido)
        {
            openWindow(150, 400, "../Utils/SetMotivoCanc.aspx?idPedido=" + idPedido);
            return false;
        }

        function validaFiltro()
        {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var idCliente = FindControl("txtNumCli", "input").value;
            var nomeCli = FindControl("txtNome", "input").value;
            var idFunc = FindControl("drpVendedor", "select").value;
            var idFuncionarioConferente = FindControl("drpConferente", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var situacaoPedOri = FindControl("cbdSituacaoPedOri", "select").itens();
            var idsProcesso = FindControl("cbdProcesso", "select").itens();
            var dataIniEnt = FindControl("ctrlDataIniEnt_txtData", "input").value;
            var dataFimEnt = FindControl("ctrlDataFimEnt_txtData", "input").value;            
            var dataIniFab = FindControl("ctrlDataIniFab_txtData", "input").value;
            var dataFimFab = FindControl("ctrlDataFimFab_txtData", "input").value;
            var dataIniFin = FindControl("ctrlDataIniFin_txtData", "input").value;
            var dataFimFin = FindControl("ctrlDataFimFin_txtData", "input").value;
            var dataIniConf = FindControl("ctrlDataIniConf_txtData", "input").value;
            var dataFimConf = FindControl("ctrlDataFimConf_txtData", "input").value;
            var dataIniEmis = FindControl("ctrlDataIniEmis_txtData", "input").value;
            var dataFimEmis = FindControl("ctrlDataFimEmis_txtData", "input").value;
            var pedidosSemAnexos = FindControl("chkPedidosSemAnexos", "input").checked;
            var pedidosAComprar = FindControl("chkPedidosAComprar", "input").checked;
            var situacaoCnc = FindControl("cblSituacaoCnc", "select").itens();
            var dataIniSituacaoCnc = FindControl("ctrlDataSitCncIni_txtData", "input").value;
            var dataFimSituacaoCnc = FindControl("ctrlDataSitCncFim_txtData", "input").value;
            var tipoPedido = FindControl("cbdTipoPedido", "select").itens();
            var idsRotas = FindControl("cblRota", "select").itens();
            var pedConf = FindControl("drpPedConferido", "select").value;

            if (idPedido == "" && idCliente == "" && nomeCli == "" && situacao == 0 && situacaoPedOri == 0 && idsProcesso == 0 &&
                (idFunc == "0" || idFunc == "") && (idFuncionarioConferente == "0" || idFuncionarioConferente == "") && dataIniEnt == "" &&
                dataFimEnt == "" && dataIniFab == "" && dataFimFab == "" && dataIniFin == "" &&
                dataFimFin == "" && dataIniConf == "" && dataFimConf == "" && dataIniEmis == "" && dataFimEmis == "" && !pedidosSemAnexos &&
                !pedidosAComprar && situacaoCnc == "" && dataIniSituacaoCnc == "" && dataFimSituacaoCnc == "" && tipoPedido == "" && idsRotas == "" && pedConf == 0) {
                if (!confirm("É recomendável aplicar um filtro! Deseja realmente prosseguir?")) return false;
                else return true;
            }
            else return true;
        }
         
        function validaFiltroTipo(tipo)
        {
            var idPedido = FindControl("txtNumPedido", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            
            if ((situacao == "0" || situacao == "1") && idPedido == "") {
                alert("O arquivo " + tipo + " pode ser gerado apenas para pedidos Finalizados ou Impressos, filtre por alguma dessas situações e tente novamente.");
                return false;
            }
            return true;
        }

        function validaPodeGerarArquivo()
        {
            var idPedido = FindControl("txtNumPedido", "input").value;

            if (LstPedidosEspelho.PodeImprimirPedidoImportado(idPedido).value.toLowerCase() == "false") {
                alert("O pedido importado ainda não foi conferido, confira o mesmo antes de gerar arquivo");
                return false;
            }

            return true;
        }

        function gerarArquivoCnc() {

            if (!validaPodeGerarArquivo())
                return false;

            if (!validaFiltro())
                return false;

            window.open("../Handlers/ArquivoCnc.ashx?" + getRptQueryString());
        }
 
        function gerarArquivoDxf() {

            if (!validaPodeGerarArquivo())
                return false;

            if (validaFiltroTipo("DXF"))
                window.open("../Handlers/ArquivoDxf.ashx?" + getRptQueryString());
        }

        function gerarArquivoFml() {

            if (!validaPodeGerarArquivo())
                return false;

            if (validaFiltroTipo("FML"))
                window.open("../Handlers/ArquivoFml.ashx?" + getRptQueryString());
        }

        function gerarArquivoSglass() {

            if (!validaPodeGerarArquivo())
                return false;

            if (validaFiltroTipo("SGLASS"))
                window.open("../Handlers/ArquivoSglass.ashx?" + getRptQueryString());
        }

        function gerarArquivoIntermac() {

            if (!validaPodeGerarArquivo())
                return false;

            if (validaFiltroTipo("Intermac"))
                window.open("../Handlers/ArquivoIntermac.ashx?" + getRptQueryString());
        }

        function openListaTotalMarcacao() {
            if (validaFiltro()) {
                openWindow(140, 320, "../Listas/ListaTotalMarcacao.aspx?" + getRptQueryString());
            }

            return false;
        }

        function validaNumero(controle) {
            if (isNaN(controle.value))
                controle.value = "";

            return false;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" onkeypress="return soNumeros(event, true, true);"
                                Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                onblur="validaNumero(this);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getCli(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                           <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                DataValueField="IdLoja" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdSituacaoPedOri" runat="server"
                                DataTextField="Descr" CheckAll="False" DataSourceID="odsSituacao" 
                                DataValueField="Id" Title="Selecione a Situação">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label28" runat="server" ForeColor="#0066FF" Text="Origem Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrigemPedido" runat="server" AutoPostBack="true">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Normal</asp:ListItem>
                                <asp:ListItem Value="2">Ecommerce</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label17" runat="server" Text="Tipo Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdTipoPedido" runat="server" Title="Selecione o tipo de pedido"
                                DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton10" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Situação PCP" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Aberto</asp:ListItem>
                                <asp:ListItem Value="2">Finalizado</asp:ListItem>
                                <asp:ListItem Value="3">Impresso</asp:ListItem>
                                <asp:ListItem Value="4">Impresso Comum</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Período Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniEnt" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimEnt" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Período Fábrica" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniFab" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimFab" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton6" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblRota" runat="server" Width="110px" CheckAll="False"
                                Title="Selecione a rota" DataSourceID="odsRota" DataTextField="Descricao" DataValueField="IdRota"
                                ImageURL="~/Images/DropDown.png" JQueryURL="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"
                                OpenOnStart="False">
                            </sync:CheckBoxListDropDown>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll"
                                TypeName="Glass.Data.DAL.RotaDAO">
                            </colo:VirtualObjectDataSource>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton11" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label12" runat="server" Text="Período Emissão Ped." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniEmis" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimEmis" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq6" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label11" runat="server" Text="Período Conf." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniConf" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimConf" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Período Finalização" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataIniFin" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataFimFin" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq5" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label6" runat="server" ForeColor="#0066FF" Text="Vendedor (Assoc. Pedido)"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpVendedor" runat="server" DataSourceID="odsFuncionario" DataTextField="Nome"
                                DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq4" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Conferente"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpConferente" runat="server" DataSourceID="odsConferente" DataTextField="Nome"
                                DataValueField="IdFunc" AppendDataBoundItems="True">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton7" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>                        
                        <td>
                            <asp:CheckBox ID="chkPedidosSemAnexos" runat="server" Text="Pedidos sem anexos" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkPedidosAComprar" runat="server" Text="Pedidos com produtos a comprar" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq3" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Processo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdProcesso" runat="server"
                                DataTextField="CodInterno" CheckAll="False" DataSourceID="odsProcesso" 
                                DataValueField="IdProcesso" ImageURL="~/Images/DropDown.png" OpenOnStart="False" 
                                Title="Selecione o Processo">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton9" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblPedConferido" runat="server" Text="Pedidos Importados" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpPedConferido" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Conferido</asp:ListItem>
                                <asp:ListItem Value="2">Não Conferido</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table runat="server" id="tbSituacaoCnc">
                    <tr>
                        
                        <td>
                            <asp:Label ID="lblSitProjCnc" runat="server" Text="Situação Proj. CNC" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cblSituacaoCnc" runat="server" ImageURL="~/Images/DropDown.png"
                                OpenOnStart="False" Title="Selecione a Situação">
                                <asp:ListItem Text="Sem necessidade (Não conferido)" Value="1" />
                                <asp:ListItem Text="Sem necessidade (Conferido)" Value="4" />
                                <asp:ListItem Text="Não projetado" Value="2" />
                                <asp:ListItem Text="Projetado" Value="3" />
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesqSitProjCnc" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label15" runat="server" Text="Período situação Proj. CNC" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataSitCncIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc2:ctrlData ID="ctrlDataSitCncFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton8" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataSourceID="odsPedidoEspelho" DataKeyNames="IdPedido" OnRowDataBound="grdPedido_RowDataBound"
                    EmptyDataText="Nenhum pedido em produção encontrado." OnRowCommand="grdPedido_RowCommand">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" Visible='<%# Eval("EditVisible") %>'
                                    NavigateUrl='<%# "../Cadastros/CadPedidoEspelho.aspx?idPedido=" + Eval("IdPedido") %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                                <asp:LinkButton ID="lnkCancelar" runat="server" CommandName="CancelarEspelho" CommandArgument='<%# Eval("IdPedido") %>'
                                    Visible='<%# Eval("CancelarVisible") %>' OnClientClick="return confirm('Tem certeza que deseja excluir esta conferência?');">
                                    <img src="../Images/ExcluirGrid.gif" border="0" /></asp:LinkButton>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# (int)Eval("Situacao") != (int)Glass.Data.Model.PedidoEspelho.SituacaoPedido.Processando %>'>
                                    <a href="#" onclick="openRpt('<%# Eval("IdPedido") %>', <%# Eval("UsarControleReposicao").ToString().ToLower() %>, 2);">
                                        <img border="0" src="../Images/page_gear.png" title="Pedido PCP" /></a>
                                    <a href="#" onclick="openRpt('<%# Eval("IdPedido") %>', <%# Eval("UsarControleReposicao").ToString().ToLower() %>, 0);">
                                        <img border="0" src="../Images/Relatorio.gif" title="Pedido" /></a>
                                </asp:PlaceHolder>
                                <asp:ImageButton ID="imbMemoriaCalculo" runat="server" ImageUrl="~/Images/calculator.gif"
                                    OnClientClick='<%# "openRpt(" + Eval("IdPedido") + ", false, 3); return false" %>'
                                    ToolTip="Memória de cálculo" Visible='<%# Eval("ExibirRelatorioCalculo") %>' />
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# (int)Eval("Situacao") != (int)Glass.Data.Model.PedidoEspelho.SituacaoPedido.Processando %>'>
                                    <asp:PlaceHolder ID="pchAnexos" runat="server"><a href="#" onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdPedido") %>&tipo=pedido&#039;); return false;'>
                                        <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                                    <asp:PlaceHolder ID="pchImprProj" runat="server" Visible='<%# Eval("ExibirImpressaoProjeto") %>'>
                                        <a href="#" onclick="openRptProj('<%# Eval("IdPedido") %>');">
                                            <img border="0" src="../Images/clipboard.gif" title="Projeto" /></a> </asp:PlaceHolder>
                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/imagem.gif"
                                        OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=pcp&idPedido=" + Eval("IdPedido") + "\"); return false" %>'
                                        ToolTip="Associar imagem à peça" />
                                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/basket_go.gif"
                                        ToolTip="Produtos que ainda não foram comprados" Visible='<%# Eval("ComprarVisible") %>'
                                        OnClientClick='<%# "openRptComprar(" + Eval("IdPedido") + "); return false" %>' />
                                </asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedidoExibir" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeInicialCli" HeaderText="Cliente" SortExpression="NomeInicialCli" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:BoundField DataField="ResponsavelConferecia" HeaderText="Conferente" SortExpression="Conferente" />
                        <asp:BoundField DataField="TotalPedido" HeaderText="Total Pedido" SortExpression="TotalPedido"
                            DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Total" HeaderText="Total Conf." SortExpression="Total"
                            DataFormatString="{0:C}"></asp:BoundField>
                        <asp:BoundField DataField="DataEspelho" DataFormatString="{0:d}" HeaderText="Data Conf."
                            SortExpression="DataEspelho" />
                        <asp:BoundField DataField="DataConf" DataFormatString="{0:d}" HeaderText="Finalização"
                            SortExpression="DataConf" />
                        <asp:TemplateField HeaderText="Total m² / Qtde." SortExpression="TotM">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("TotM") + " (" + Eval("QtdePecas") + " pç.)" %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Peso" HeaderText="Peso" SortExpression="Peso" />
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacao") %>' Style='<%# (bool)Eval("ExibirReabrir") ? "position: relative; bottom: 5px": "" %>'>
                                </asp:Label>
                                <asp:ImageButton ID="imbReabrir" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                    CommandName="Reabrir" ImageUrl="~/Images/cadeado.gif" ToolTip="Reabrir pedido"
                                    Visible='<%# Eval("ExibirReabrir") %>'
                                    OnClientClick='<%# (bool)Eval("Importado") ?
                                        "return confirm(&#39;Este pedido foi importado, ao reabri-lo as marcações e etiquetas importadas serão PERDIDAS. Deseja reabrir este pedido?&#39;)" : "return confirm(&#39;Deseja reabrir este pedido?&#39;)" %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemStyle VerticalAlign="Middle" Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Entrega" SortExpression="DataEntrega">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("DataEntrega") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("DataEntregaExibicao", "{0:d}") + ((bool)Eval("FastDelivery") ? " - Fast Del." : "") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Fábrica" SortExpression="DataFabrica">
                            <ItemTemplate>
                                <asp:Label ID="Label45" runat="server" Text='<%# Bind("DataFabrica", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox41" runat="server" Text='<%# Bind("DataFabrica") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação Proj. CNC" SortExpression="SituacaoCnc">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0" class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label44" runat="server" Text='<%# Bind("DescrSituacaoCnc") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imbSituacaoCnc" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                                CommandName="SituacaoCnc" ImageUrl='<%# "~/Images/" + ((int)Eval("SituacaoCnc") == (int)Glass.Data.Model.PedidoEspelho.SituacaoCncEnum.Projetado ? "projeto_deletar.png" : "projeto.png")  %>'
                                                ToolTip='<%# (int)Eval("SituacaoCnc") == (int)Glass.Data.Model.PedidoEspelho.SituacaoCncEnum.Projetado ? "Marcar pedido como não projetado?" : "Marcar pedido como projetado?" %>'
                                                Visible='<%# Eval("ExibirSituacaoCnc") %>'
                                                 OnClientClick='<%# "return confirm(&#39;Deseja marcar este pedido como" + ((int)Eval("SituacaoCnc") == (int)Glass.Data.Model.PedidoEspelho.SituacaoCncEnum.Projetado ? " não projetado" : " projetado") + "?&#39;)" %>' />

                                            <asp:ImageButton ID="ImageButton12" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                                CommandName="SituacaoCncConferencia" ImageUrl='<%# "~/Images/" + ((int)Eval("SituacaoCnc") == (int)Glass.Data.Model.PedidoEspelho.SituacaoCncEnum.SemNecessidadeNaoConferido ? "ok.gif" : "Inativar.gif")  %>'
                                                ToolTip='<%# (int)Eval("SituacaoCnc") == (int)Glass.Data.Model.PedidoEspelho.SituacaoCncEnum.SemNecessidadeNaoConferido ? "Marcar pedido como sem necessidade (Conferido)?" : "Marcar pedido como sem necessidade (Não conferido)?" %>'
                                                Visible='<%# Eval("ExibirSituacaoCncConferencia") %>'
                                                 OnClientClick='<%# "return confirm(&#39;Deseja marcar a situação proj. CNC deste pedido como" + ((int)Eval("SituacaoCnc") == (int)Glass.Data.Model.PedidoEspelho.SituacaoCncEnum.SemNecessidadeNaoConferido ? " sem necessidade (Conferido)" : " sem necessidade (Não conferido)") + "?&#39;)" %>' />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pedido Conferido ?">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0" class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblPedidoConferido" runat="server" Text='<%# (bool)Eval("PedidoConferido") == true ? "Conferido" : "Não Conferido" %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imgBtnPedidoConferido" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                                CommandName="PedidoImportadoConferido" ImageUrl='<%# "~/Images/" + ((bool)Eval("PedidoConferido") == false ? "ok.gif" : "Inativar.gif")  %>'
                                                ToolTip='<%# (bool)Eval("PedidoConferido") == true ? "Marcar pedido importado como não conferido?" : "Marcar pedido importado como Conferido?" %>'
                                                Visible='<%# Eval("ConferirPedidoVisible") %>'
                                                 OnClientClick='<%# "return confirm(&#39;Deseja marcar o pedido importado como" + ((bool)Eval("PedidoConferido") == true ? " Não Conferido" : " Conferido") + "?&#39;)" %>' />
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>                            
                                <asp:Image ID="imgCompraGerada" runat="server" 
                                    ImageUrl="../Images/basket_go.gif"
                                    ToolTip='<%# "Compra gerada: " + Eval("CompraGerada") %>'
                                    Visible='<%# Eval("CompraGerada") != null && Eval("CompraGerada") != "" %>' />
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# Eval("IdPedido") %>'
                                    Tabela="PedidoEspelho" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedidoEspelho" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.PedidoEspelhoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumCli" Name="idCli" PropertyName="Text" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeCli" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpVendedor" Name="idFunc" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpConferente" Name="idFuncionarioConferente" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="cbdSituacaoPedOri" Name="situacaoPedOri" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdProcesso" Name="idsProcesso" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniEnt" Name="dataIniEnt" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimEnt" Name="dataFimEnt" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniFab" Name="dataIniFab" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimFab" Name="dataFimFab" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniFin" Name="dataIniFin" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimFin" Name="dataFimFin" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniConf" Name="dataIniConf" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimConf" Name="dataFimConf" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataIniEmis" Name="dataIniEmis" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFimEmis" Name="dataFimEmis" PropertyName="DataString"
                            Type="String" />
                        <asp:Parameter DefaultValue="false" Name="soFinalizados" Type="Boolean" />
                        <asp:ControlParameter ControlID="chkPedidosSemAnexos" Name="pedidosSemAnexo" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="chkPedidosAComprar" Name="pedidosAComprar" PropertyName="Checked"
                            Type="Boolean" />
                        <asp:ControlParameter ControlID="cblSituacaoCnc" Name="situacaoCnc" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataSitCncIni" Name="dataIniSituacaoCnc" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataSitCncFim" Name="dataFimSituacaoCnc" PropertyName="DataString"
                            Type="String" />
                        <asp:ControlParameter ControlID="cbdTipoPedido" Name="tipoPedido" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="cblRota" Name="idsRotas" PropertyName="SelectedValue"
                            Type="String" />
                        <asp:ControlParameter ControlID="drpOrigemPedido" Name="origemPedido" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpPedConferido" Name="pedidosConferidos" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetVendedores" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsConferente" runat="server"
                    SelectMethod="GetConferentesPCP" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoPedidoPCP"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProcesso" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.EtiquetaProcessoDAO" CacheExpirationPolicy="Absolute"
                    ConflictDetection="OverwriteChanges" MaximumRowsParameterName="" OldValuesParameterFormatString="original_{0}"
                    SkinID="" StartRowIndexParameterName="">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedido"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkTotaisMarcacoes" runat="server" OnClientClick="return openListaTotalMarcacao();"
                                ToolTip="Exibe os valores de preço, peso e m² totais dos pedidos listados."> <img 
                                alt="" border="0" src="../Images/detalhes.gif" /> Total marcação</asp:LinkButton>
                        </td>
                        <td align="center">
                            <asp:Image ID="Image1" runat="server" ImageUrl="../Images/Clipe.gif" />
                            <asp:LinkButton ID="lnkAnexos" runat="server" OnClientClick="openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=0&tipo=pedido&#039;); return false;"
                                ToolTip="Anexar arquivos à vários pedidos">Anexar arquivos à vários pedidos</asp:LinkButton>
                        </td>
                        <td></td>
                        <td align="center">
                            <asp:Image ID="imbArquivoCnc" runat="server" ImageUrl="../Images/blocodenotas.png" />
                        </td>
                        <td align="center">
                            <asp:LinkButton ID="lnkArquivoCnc" runat="server" OnClientClick="gerarArquivoCnc();"
                                ToolTip="Gerar arquivo CNC">Gerar arquivo CNC</asp:LinkButton>
                        </td>
                        <td></td>
                        <td align="center">
                            <asp:Image ID="imbArquivoDxf" runat="server" ImageUrl="../Images/blocodenotas.png" />
                        </td>
                        <td align="center">
                            <asp:LinkButton ID="lnkArquivoDxf" runat="server" OnClientClick="gerarArquivoDxf();"
                                ToolTip="Gerar arquivo DXF">Gerar arquivo DXF</asp:LinkButton>
                        </td>
                        <td></td>
                        <td align="center">
                            <asp:Image ID="imbArquivoFml" runat="server" ImageUrl="../Images/blocodenotas.png" />
                        </td>
                        <td align="center">
                            <asp:LinkButton ID="lnkArquivoFml" runat="server" OnClientClick="gerarArquivoFml();"
                                ToolTip="Gerar arquivo FML">Gerar arquivo FML</asp:LinkButton>
                        </td>
                        <td></td>
                        <td align="center">
                            <asp:Image ID="imbArquivoSglass" runat="server" ImageUrl="../Images/blocodenotas.png" />
                        </td>
                        <td align="center">
                            <asp:LinkButton ID="lnkArquivoSglass" runat="server" OnClientClick="gerarArquivoSglass();"
                                ToolTip="Gerar arquivo SGLASS">Gerar arquivo SGLASS</asp:LinkButton>
                        </td>
                        <td></td>
                        <td align="center">
                            <asp:Image ID="imgArquivoIntermac" runat="server" ImageUrl="../Images/blocodenotas.png" />
                        </td>
                        <td align="center">
                            <asp:LinkButton ID="lnkArquivoIntermac" runat="server" OnClientClick="gerarArquivoIntermac();"
                                ToolTip="Gerar arquivo Intermac">Gerar arquivo Intermac</asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRptLista(); return false">
                                <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                        </td>
                        <td></td>
                        <td align="center">
                            <asp:LinkButton ID="lnkImprimirSel" runat="server" OnClientClick="openRptListaSel(); return false">
                                <img alt="" border="0" src="../Images/printer.png" /> Impressão seletiva</asp:LinkButton>
                        </td>
                        <td></td>
                        <td align="center">
                            <asp:LinkButton ID="lnkImprimirProdutosAComprar" runat="server" OnClientClick="openRptPedidosAComprar(); return false">
                                <img alt="" border="0" src="../Images/basket_go.gif" /> Produtos a comprar</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
