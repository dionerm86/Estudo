<%@ Page Title="Gerar Nota Fiscal" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadNotaFiscalGerar.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadNotaFiscalGerar" %>

<%@ Register Src="../Controls/ctrlConsultaCadCliSintegra.ascx" TagName="ctrlConsultaCadCliSintegra"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlNaturezaOperacao.ascx" TagName="ctrlNaturezaOperacao"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        var clientes = new Array();
        var buscandoCliente = false;

        function addCarregamento() {
            var idCarregamento = FindControl("txtNumCarregamento", "input").value;
            if (Trim(idCarregamento) == "") {
                alert("Selecione um carregamento para continuar.");
                FindControl("txtNumCarregamento", "input").value = "";
                FindControl("txtNumCarregamento", "input").focus();
                return;
            }

            var idsPedidos = CadNotaFiscalGerar.GetIdsPedidosByCarregamento(idCarregamento).value.split(';');

            if (idsPedidos[0] == "Erro") {
                alert(idsPedidos[1]);
                FindControl("txtNumCarregamento", "input").value = "";
                FindControl("txtNumCarregamento", "input").focus();
                return;
            }

            FindControl("hdfBuscarIdsPedidos", "input").value = idsPedidos[1].split(',');
            FindControl("hdfIdCarregamento", "input").value = FindControl("txtNumCarregamento", "input").value;
            cOnClick("btnBuscarPedidos", null);
        }

        function addPedido(idPedido) {
            if (buscandoCliente)
                return;

            if (idPedido == null || idPedido == 0)
                idPedido = FindControl("txtNumPedido", "input").value;

            if (Trim(idPedido) == "") {
                alert("Selecione um pedido para continuar.");
                FindControl("txtNumPedido", "input").value = "";
                FindControl("txtNumPedido", "input").focus();
                return;
            }

            if (CadNotaFiscalGerar.PedidoExiste(idPedido).value == "false") {
                alert("Não existe pedido com esse número.");
                FindControl("txtNumPedido", "input").value = "";
                FindControl("txtNumPedido", "input").focus();
                return;
            }

            var conf = CadNotaFiscalGerar.IsPedidoConfirmadoLiberado(idPedido).value.split('|');
            if (conf[0] == "false") {
                alert("Esse pedido não está " + conf[1] + ".");
                FindControl("txtNumPedido", "input").value = "";
                FindControl("txtNumPedido", "input").focus();
                return;
            }

            var idsPedidos = FindControl("hdfBuscarIdsPedidos", "input").value.split(',');
            var novosIds = new Array();

            novosIds.push(idPedido);
            for (i = 0; i < idsPedidos.length; i++)
                if (idsPedidos[i] != idPedido && idsPedidos[i].length > 0)
                    novosIds.push(idsPedidos[i]);

            FindControl("hdfBuscarIdsPedidos", "input").value = novosIds.join(',');
            FindControl("txtNumPedido", "input").value = "";
            cOnClick("btnBuscarPedidos", null);
        }

        function removePedido(idPedido, atualiza) {
            var idsPedidos = FindControl("hdfBuscarIdsPedidos", "input").value.split(',');
            var novosIds = new Array();

            for (i = 0; i < idsPedidos.length; i++)
                if (idsPedidos[i] != idPedido && idsPedidos[i].length > 0)
                novosIds.push(idsPedidos[i]);

            FindControl("hdfBuscarIdsPedidos", "input").value = novosIds.join(',');
            var liberacoes = FindControl("hdfBuscarIdsLiberacoes", "input");
            liberacoes.value = CadNotaFiscalGerar.LiberacoesPedidos(liberacoes.value, novosIds.join(',')).value;

            var idsLiberacaoPedidos = new Array();
            var arrLiberacoes = liberacoes.value.split(',');

            for (i = 0; i < arrLiberacoes.length; i++)
                idsLiberacaoPedidos.push(arrLiberacoes[i] + "&" + CadNotaFiscalGerar.IdsPedidosLiberacoes(arrLiberacoes[i]).value);

            FindControl("hdfIdsLiberacaoPedidos", "input").value = idsLiberacaoPedidos.join(';');

            if (atualiza)
                cOnClick("btnBuscarPedidos", null);
        }

        function removeOC(idsPedidos) {
            
            var ids = idsPedidos.toString().split(',');

            for (var i = 0; i < ids.length; i++) {

                removePedido(ids[i].trim(), i == ids.length -1);
            }
        }

        function addLiberacao() {
            if (buscandoCliente)
                return;

            var idLiberacao = FindControl("txtLiberacao", "input").value;
            if (Trim(idLiberacao) == "") {
                alert("Selecione uma liberação de pedido para continuar.");
                FindControl("txtLiberacao", "input").value = "";
                FindControl("txtLiberacao", "input").focus();
                return;
            }

            if (CadNotaFiscalGerar.LiberacaoExiste(idLiberacao).value == "false") {
                alert("Não existe liberação de pedido com esse número.");
                FindControl("txtLiberacao", "input").value = "";
                FindControl("txtLiberacao", "input").focus();
                return;
            }

            if (CadNotaFiscalGerar.IsLiberacaoAberta(idLiberacao).value == "false") {
                alert("Essa liberação de pedido está cancelada.");
                FindControl("txtLiberacao", "input").value = "";
                FindControl("txtLiberacao", "input").focus();
                return;
            }

            var idsLiberacoes = FindControl("hdfBuscarIdsLiberacoes", "input").value.split(',');
            var novosIds = new Array();

            novosIds.push(idLiberacao);
            for (i = 0; i < idsLiberacoes.length; i++)
                if (idsLiberacoes[i] != idLiberacao && idsLiberacoes[i].length > 0)
                novosIds.push(idsLiberacoes[i]);

            var idsLiberacaoPedidos = new Array();

            for (i = 0; i < novosIds.length; i++)
                idsLiberacaoPedidos.push(novosIds[i] + "&" + CadNotaFiscalGerar.IdsPedidosLiberacoes(novosIds[i]).value);

            FindControl("hdfIdsLiberacaoPedidos", "input").value = idsLiberacaoPedidos.join(';');
            FindControl("hdfBuscarIdsLiberacoes", "input").value = novosIds.join(',');
            FindControl("hdfBuscarIdsPedido", "input").value = CadNotaFiscalGerar.IdsPedidosLiberacoes(novosIds.join(',')).value;
            FindControl("txtLiberacao", "input").value = "";
            cOnClick("btnBuscarPedidos", null);
        }

        function buscarPedidos() {
            var idCliente = "";
            var nomeCliente = "";
            
            if (FindControl("txtIdCli", "input") != null && FindControl("txtIdCli", "input") != undefined &&
                FindControl("txtNomeCli", "input") != null && FindControl("txtNomeCli", "input") != undefined) {
                if (FindControl("txtIdCli", "input").value != "" && FindControl("txtNomeCli", "input").value != "") {
                    idCliente = FindControl("txtIdCli", "input").value;
                    nomeCliente = FindControl("txtNomeCli", "input").value;
                }
                else
                    return;
            }
            else
                return;

            buscandoCliente = true;

            if (idCliente == "")
                idCliente = "0";

            FindControl("hdfBuscarIdsPedidos", "input").value = CadNotaFiscalGerar.GetPedidosByCliente(idCliente, nomeCliente).value;
        }

        function getCli(idCli) {
            if (idCli.value == "")
                return;

            idCli = idCli.value != undefined ? idCli.value : idCli;

            var retorno = MetodosAjax.GetCli(idCli).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCli", "input").value = "";
                return false;
            }

            FindControl("txtIdCli", "input").value = idCli;
            FindControl("txtNomeCli", "input").value = retorno[1];
            PodeConsSitCadContr();
        }

        function gerarNf(botao, dadosNaturezasOperacao, nfce) {
            if (!validate())
                return false;

            // Informa sobre os pedidos que já contém notas fiscais
            var tabela = document.getElementById("<%= grdPedidos.ClientID %>");
            var pedidosNf = new Array();

            for (i = 1; i < tabela.rows.length; i++) {
                var inputs = tabela.rows[i].cells[0].getElementsByTagName("input");
                var idPedido;
                var notasGeradas;

                for (j = 0; j < inputs.length; j++)
                    if (inputs[j].id.indexOf("hdfIdPedido") > -1)
                    idPedido = inputs[j].value;
                else if (inputs[j].id.indexOf("hdfNotasGeradas") > -1)
                    notasGeradas = inputs[j].value;

                if (notasGeradas.length > 0)
                    pedidosNf.push(new Array(idPedido, notasGeradas));
            }
            
            if(<%= NaoPermitirMaisDeUmaNfeParaUmPedido() %> && pedidosNf.length > 0){
                var pedidos = "";
                for (i = 0; i < pedidosNf.length; i++)
                    pedidos += ", " + pedidosNf[i][0] + " (NF " + pedidosNf[i][1] + ")";
                    
                    alert('Não é possível gerar a NF-e, pois os seguintes pedidos já possuem NF-e gerada:\n' + pedidos.substr(2));
                    return false;
            }

            if (!dadosNaturezasOperacao && pedidosNf.length > 0) {
                var pedidos = "";
                for (i = 0; i < pedidosNf.length; i++)
                    pedidos += ", " + pedidosNf[i][0] + " (NF " + pedidosNf[i][1] + ")";

                if (!confirm("Os seguintes pedidos já possuem notas fiscais geradas para eles:\n" +
                pedidos.substr(2) + ".\n\nDeseja continuar com a geração das notas?"))
                    return false;
            }

            var idCliente = FindControl("hdfIdCliente", "input").value;
            var idsPedidos = FindControl("hdfBuscarIdsPedidos", "input").value;
            var idsLiberarPedidos = FindControl("hdfBuscarIdsLiberacoes", "input").value;
            var idNaturezaOperacao = FindControl("ctrlNaturezaOperacao_selNaturezaOperacao_hdfValor", "input").value;
            var idLoja = FindControl("drpLoja", "select").value;
            var tipoNota = FindControl("drpTipoNota", "select").value;
            var idLojaDestino = FindControl("drpLojaDestino", "select").value;
            var transferirNf = FindControl("chkTransferirNf", "input") != null ? FindControl("chkTransferirNf", "input").checked : false;
            var manterAgrupamentoDeProdutos = FindControl("chkAguparProdutos", "input") != null ? FindControl("chkAguparProdutos", "input").checked : true;

            // Se a empresa calcula icms no pedido, verifica se os pedidos possuem ST e se a natureza de operação calcula ST
            if (!dadosNaturezasOperacao && CadNotaFiscalGerar.CalculaIcmsPedido(idsPedidos, idsLiberarPedidos).value == "true") {
                var pedidosPossuemSt = CadNotaFiscalGerar.PedidosPossuemSt(idsPedidos).value == "true";
                var calcSt = CadNotaFiscalGerar.CalcSt(idNaturezaOperacao).value == "true";

                if (pedidosPossuemSt && !calcSt) {
                    if (!confirm("Em alguns dos pedidos selecionados foram calculados o ICMS ST, porém você selecionou uma natureza de operação que não calcula ICMS ST, haverá diferença de valores, deseja continuar assim mesmo?"))
                        return false;
                }
            }

            if (idLoja == "" || idLoja == "0") {
                alert("Informe a loja.");
                return false;
            }

            botao.disabled = true;
            bloquearPagina();

            var percReducaoCli = FindControl("txtPercReducao", "input");
            var percReducaoCliRevenda = FindControl("txtPercReducaoRev", "input");
            percReducaoCli = percReducaoCli != null ? percReducaoCli.value : "";
            percReducaoCliRevenda = percReducaoCliRevenda != null ? percReducaoCliRevenda.value : "";

            var idCli = FindControl("ddlClienteVinculado", "select") != null ? FindControl("ddlClienteVinculado", "select").value : "";

            if (tipoNota == "2") {
             
                 if (idLojaDestino == "" || idLojaDestino == "0") {
                     alert("Informe a loja de destino.");
                     return false;
                 }
                 
                 idCli = idLojaDestino;
            }

            if (FindControl("chkNaturezaOperacaoPorProduto", "input").checked && !dadosNaturezasOperacao) {
                var agruparProduto = CadNotaFiscalGerar.GetAgruparProdutoNf().value;

                openWindow(500, 750, "../Utils/SetNaturezaOperacaoProdutoGerarNf.aspx?idCliente=" + (idCli != "" ? idCli : idCliente) + "&idsPedidos=" + idsPedidos +
                    "&idsLiberarPedidos=" + idsLiberarPedidos + "&idNaturezaOperacao=" + idNaturezaOperacao + "&idLoja=" + idLoja +
                    "&percReducaoCli=" + percReducaoCli + "&percReducaoCliRevenda=" + percReducaoCliRevenda + "&agruparProdutos=" + agruparProduto +
                    "&nfce=" + (nfce == undefined || nfce == null ? false : nfce));

                return false;
            }

            var transferencia = tipoNota == 2 ? true : false;
            var carregamento = FindControl("txtNumCarregamento", "input");
            var idCarregamento = "";

            if (transferencia && carregamento != null)
                idCarregamento = carregamento.value;

            var retorno = CadNotaFiscalGerar.GerarNf(idsPedidos, idsLiberarPedidos, idNaturezaOperacao,
                idLoja, percReducaoCli, percReducaoCliRevenda, dadosNaturezasOperacao, idCli, transferencia,
                idCarregamento, transferirNf, nfce == undefined || nfce == null ? false : nfce, manterAgrupamentoDeProdutos).value.split(';');

            if (retorno[0] == "Erro")
                FalhaGerarNf(retorno[1]);
            else if(retorno[0] == "Exp")
                NfExportada();
            else
                NfGerada(retorno[1], true, botao);
        }

        function NfGerada(idNf, popup, botao) {
            try
            {
                desbloquearPagina(true);
            }
            catch (err)
            {}
            
            var exibirMensagem = CadNotaFiscalGerar.ExibirMensagem(idNf).value;

            alert("Nota fiscal gerada com sucesso!");
            botao.disabled = true;
            redirectUrl((popup ? "../Cadastros/" : "") + "CadNotaFiscal.aspx?tipo=2&idNf=" + idNf+"&exibirMensagem="+ exibirMensagem);            
        }

        function FalhaGerarNf(erro, fechouJanela) {
            desbloquearPagina(true);
            
            if (!fechouJanela)
                alert(erro);

            FindControl("btnGerarNf", "input").disabled = false;
            return;
        }

        function NfExportada(){
            try
            {
                desbloquearPagina(true);
            }
            catch (err)
            {}
            
            alert("Nota fiscal exportada com sucesso!");
            botao.disabled = true;
            window.location.href = window.location.href; 
        }

        function PodeConsSitCadContr() {
            var idCli = FindControl("txtIdCli", "input").value;

            if (idCli == "" || CadNotaFiscalGerar.PodeConsultarCadastro(idCli).value == "False")
                FindControl("ConsultaCadCliSintegra", "div").style.display = 'none';
            else
                FindControl("ConsultaCadCliSintegra", "div").style.display = 'inline';

        }

        function atualizaTipoNf(limparIds) {

            var tipoNf = FindControl("drpTipoNota", "select").value;

            var drpLojaDestino = FindControl("drpLojaDestino", "select");
            var lblLojaDestino = FindControl("lblLojaDestino", "span");

            if(drpLojaDestino != null){
                drpLojaDestino.style.display = tipoNf == 2 ? "inline" : "none";
            }

            if(lblLojaDestino != null){
                lblLojaDestino.style.display = tipoNf == 2 ? "inline" : "none";
            }

            FindControl("nfTransferencia", "tr").style.display = tipoNf == 2 ? "inline" : "none";
            FindControl("nfSaida", "tr").style.display = tipoNf == 1 ? "inline" : "none";

            var grdPedidos = FindControl("grdPedidos", "table");
            if(grdPedidos != null) 
                grdPedidos.style.display = tipoNf == 1 ? "inline" : "none";
                
            var grdOcs = FindControl("grdOcs", "table");  
            if(grdOcs != null)  
                grdOcs.style.display = tipoNf == 2 ? "inline" : "none";

            if (limparIds)
                FindControl("hdfBuscarIdsPedidos", "input").value = "";
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table runat="server" id="tbTipoNota">
                    <tr>
                        <td>
                            <asp:Label ID="lblTipoNota" runat="server" Text="Tipo de Nota Fiscal" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoNota" runat="server" onchange="atualizaTipoNf(true);" OnSelectedIndexChanged="drpTipoNota_SelectedIndexChanged">
                                <asp:ListItem Selected="True" Value="1">Saída</asp:ListItem>
                                <asp:ListItem Value="2">Tranferência</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr id="nfTransferencia" style="display: none;">
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label1" runat="server" Text="Carregamento" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtNumCarregamento" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                            onkeydown="if (isEnter(event)) cOnClick('imbAddCarregamento', null);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imbAddCarregamento" runat="server" ImageUrl="~/Images/Insert.gif"
                                            OnClientClick="addCarregamento(); return false;" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr id="nfSaida" style="display: inline;">
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="center" id="pedido_titulo" runat="server">
                                        <asp:Label ID="lblPedido" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td id="pedido_campo" runat="server">
                                        <asp:TextBox ID="txtNumPedido" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true)"
                                            onkeydown="if (isEnter(event)) cOnClick('imbAddPed', null);"></asp:TextBox>
                                    </td>
                                    <td id="pedido_buscar" runat="server">
                                        <asp:ImageButton ID="imbAddPed" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addPedido(); return false;" />
                                    </td>
                                    <td id="pedido_separa" runat="server">
                                        &nbsp;
                                    </td>
                                    <td align="center">
                                        <asp:Label ID="Label2" runat="server" Text="Liberação" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLiberacao" onkeypress="return soNumeros(event, true, true);"
                                            runat="server" onkeydown="if (isEnter(event)) cOnClick('imbAddLib', null);" Width="70px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="imbAddLib" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addLiberacao(); return false;" />
                                    </td>
                                </tr>
                            </table>
                            <table id="cliente" runat="server">
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="lblCliente" runat="server" Text="Cliente" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtIdCli" onkeypress="return soNumeros(event, true, true);" runat="server"
                                            Width="50px" onblur="getCli(this)"></asp:TextBox>
                                        <asp:TextBox ID="txtNomeCli" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq1', null);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=gerarNfe'); return false;">
                                <img border="0" src="../Images/Pesquisar.gif" />
                                        </asp:LinkButton>
                                    </td>
                                    <td>
                                        <div id="ConsultaCadCliSintegra" style="display: none">
                                            <uc2:ctrlConsultaCadCliSintegra runat="server" ID="ctrlConsultaCadCliSintegra1" OnLoad="ctrlConsultaCadCliSintegra1_Load" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <asp:Button ID="btnBuscarPedidos" runat="server" Text="Buscar Pedidos" OnClick="btnBuscarPedidos_Click"
                                OnClientClick="buscarPedidos()" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfBuscarIdsPedidos" runat="server" />
                <asp:HiddenField ID="hdfBuscarIdsLiberacoes" runat="server" />
                <asp:HiddenField ID="hdfIdsLiberacaoPedidos" runat="server" />
                <asp:HiddenField ID="hdfIdCliente" runat="server" />
                <asp:HiddenField ID="hdfIdCarregamento" runat="server" />
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkNaturezaOperacaoPorProduto" runat="server" Text="Selecionar natureza de operação por produto" />
                        </td>
                    </tr>
                </table>
                <br />
                <table id="gerar" runat="server" visible="false">
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdOcs" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsOcs" DataKeyNames="IdOrdemCarga" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Não foram encontradas OC's."
                                EnableViewState="False" ondatabound="grdOcs_DataBound">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                        <asp:HiddenField ID="idsPedidosOC" runat="server" Value='<%# Bind("IdsPedidos") %>' />
                                            <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                OnClientClick='<%# "removeOC(&#39;" + Eval("IdsPedidos") + "&#39;); return false;" %>'
                                                ToolTip="Remover OC" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cód. OC">
                                        <ItemTemplate>
                                            <asp:Label ID="Label9" runat="server" Text='<%# Bind("IdOrdemCarga") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Funcionário">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("NomeFunc") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cliente">
                                        <ItemTemplate>
                                            <asp:Label ID="Label121" runat="server" Text='<%# Bind("IdNomeCliente") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Loja">
                                        <ItemTemplate>
                                            <asp:Label ID="Label11" runat="server" Text='<%# Bind("NomeLoja") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("TotalPedido", "{0:c}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsOcs" runat="server" SelectMethod="GetOCsForNfe"
                                TypeName="Glass.Data.DAL.OrdemCargaDAO" >
                                <SelectParameters>
                                     <asp:ControlParameter ControlID="hdfBuscarIdsPedidos" Name="idsPedidos" PropertyName="Value"
                                        Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <asp:GridView GridLines="None" ID="grdPedidos" runat="server" AutoGenerateColumns="False"
                                DataSourceID="odsPedidos" DataKeyNames="IdPedido" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Não foram encontrados pedidos confirmados para esse cliente ou com esse número."
                                EnableViewState="False" OnDataBound="grdPedidos_DataBound">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imbExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                OnClientClick='<%# "removePedido(" + Eval("IdPedido") + ", true); return false;" %>'
                                                ToolTip="Remover pedido" />
                                            <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                                            <asp:HiddenField ID="hdfNotasGeradas" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                                    <asp:BoundField DataField="IdProjeto" HeaderText="Projeto" SortExpression="IdProjeto" />
                                    <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                                    <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                                    <asp:BoundField DataField="CpfCnpjCliente" HeaderText="Cpf/Cnpj" SortExpression="CpfCnpjCliente" />
                                    <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                                    <asp:BoundField DataField="Obs" HeaderText="Obs. Pedido" SortExpression="Obs" />
                                    <asp:BoundField DataField="ObsLiberacao" HeaderText="Obs. Liberação" SortExpression="ObsLiberacao" />
                                    <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                        <EditItemTemplate>
                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblTotalPedido" runat="server" Text='<%# Bind("Total", "{0:C}") %>'
                                                Visible='<%# !(bool)Eval("ExibirTotalEspelhoGerarNfe") %>'></asp:Label>
                                            <asp:Label ID="lblTotalPedidoEsp" runat="server" Text='<%# Bind("TotalEspelho", "{0:C}") %>'
                                                Visible='<%# Eval("ExibirTotalEspelhoGerarNfe") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="DataPedido" DataFormatString="{0:d}" HeaderText="Data Pedido"
                                        SortExpression="DataPedido" />
                                </Columns>
                                <PagerStyle CssClass="pgr"></PagerStyle>
                                <EditRowStyle CssClass="edit"></EditRowStyle>
                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            </asp:GridView>
                            <br />
                            <div style="font-size: medium; text-align: center">
                                Total:
                                <asp:Label ID="lblTotal" runat="server" Text=""></asp:Label>
                            </div>
                            <asp:Label ID="lblInfoBloqueioPedidos" runat="server" Text="<br />Pedidos em Vermelho já possuem notas fiscais geradas."
                                ForeColor="Red"></asp:Label>
                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedidos" runat="server" SelectMethod="GetForNFe"
                                TypeName="Glass.Data.DAL.PedidoDAO" OnSelected="odsPedidos_Selected">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="hdfBuscarIdsPedidos" Name="idsPedidos" PropertyName="Value"
                                        Type="String" />
                                    <asp:ControlParameter ControlID="hdfBuscarIdsLiberacoes" Name="idsLiberarPedidos"
                                        PropertyName="Value" Type="String" />
                                    <asp:ControlParameter ControlID="txtIdCli" Name="idCliente" PropertyName="Text" Type="UInt32" />
                                    <asp:ControlParameter ControlID="txtNomeCli" Name="nomeCliente" PropertyName="Text"
                                        Type="String" />
                                </SelectParameters>
                            </colo:VirtualObjectDataSource>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td>
                                        Natureza de Operação:
                                    </td>
                                    <td>
                                        <uc1:ctrlNaturezaOperacao ID="ctrlNaturezaOperacao" runat="server" 
                                            PermitirVazio="True" />
                                    </td>
                                    <td>
                                        &nbsp;&nbsp;
                                    </td>
                                    <td>
                                        Loja:
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                            DataValueField="IdLoja">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr runat="server" id="tbCliVinculado">
                                    <td>
                                        Cliente Vinculado:
                                    </td>
                                    <td colspan="4">
                                        <asp:DropDownList ID="ddlClienteVinculado" runat="server" DataSourceID="odsClienteVinculado" DataTextField="Nome" DataValueField="IdCli"
                                            OnDataBound="ddlClienteVinculado_DataBound" AutoPostBack="true" OnSelectedIndexChanged="ddlClienteVinculado_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr id="nfTransferencia">
                                    <td>
                                        <asp:Label ID="lblLojaDestino" runat="server" Text="Loja Destino:"></asp:Label>
                                    </td>
                                    <td colspan="4">
                                        <asp:DropDownList ID="drpLojaDestino" runat="server" DataSourceID="odsLojaCliente"
                                            AppendDataBoundItems="true" DataTextField="NomeFantasia" 
                                            DataValueField="idCli" >
                                            <asp:ListItem Selected="True"></asp:ListItem>
                                        </asp:DropDownList>
                                        <colo:VirtualObjectDataSource runat="server" ID="odsLojaCliente" Culture="pt-BR"
                                            SelectMethod="GetClienteLoja" TypeName="Glass.Data.DAL.ClienteDAO">
                                        </colo:VirtualObjectDataSource>
                                    </td>
                                </tr>
                                <tr id="percReducao" runat="server">
                                    <td colspan="5" align="center">
                                        <table class="pos">
                                            <tr>
                                                <td id="percReducaoVendaT" runat="server">
                                                    Perc. Desconto Venda:
                                                </td>
                                                <td id="percReducaoVenda" runat="server">
                                                    <asp:TextBox ID="txtPercReducao" runat="server" Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    %
                                                    <asp:RangeValidator ID="rgvPercReducao" runat="server" ErrorMessage="Valor entre 0 e "
                                                        ControlToValidate="txtPercReducao" Display="Dynamic" MinimumValue="0" Type="Double"></asp:RangeValidator>
                                                </td>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td id="percReducaoRevendaT" runat="server">
                                                    Perc. Desconto Revenda:
                                                </td>
                                                <td id="percReducaoRevenda" runat="server">
                                                    <asp:TextBox ID="txtPercReducaoRev" runat="server" Width="70px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                                    %
                                                    <asp:RangeValidator ID="rgvPercReducaoRev" runat="server" ErrorMessage="Valor entre 0 e "
                                                        ControlToValidate="txtPercReducaoRev" Display="Dynamic" MinimumValue="0" Type="Double"></asp:RangeValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td >
                                        <asp:CheckBox ID="chkAguparProdutos" Font-Bold="true" Text="Gerar conjunto de produto" Checked="false" OnLoad="chkAguparProdutos_Load" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5" align="center">
                                        &nbsp
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="5" align="center">
                                        <div style="color: Blue">
                                            Caso a natureza de operação seja deixada vazia, será utilizada<br />
                                            na NF-e a natureza de operação do primeiro produto
                                        </div>
                                        <br />
                                         <asp:CheckBox ID="chkTransferirNf" runat="server" Text="Transferir NF-e ?" Visible="false"/>
                                        <br />
                                        <br />
                                        <table>
                                            <tr align="center">
                                                <td>
                                                    <asp:Button ID="btnGerarNf" runat="server" OnClientClick="gerarNf(this, '', false); return false;"
                                                        Text="Gerar NF-e" /></td>
                                                <td>
                                                    <asp:Button ID="btnGerarNfc" runat="server" OnClientClick="gerarNf(this, '', true); return false;"
                                                        Text="Gerar NFC-e" /></td>
                                            </tr>
                                        </table>
                                        <br />
                                        <br />
                                        <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" OnSelected="odsLoja_Selected"
                                            SelectMethod="GetListSituacao" TypeName="Glass.Data.DAL.LojaDAO">
                                            <SelectParameters>
                                                <asp:Parameter Name="situacao" DefaultValue="1" Type="Int32" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsClienteVinculado" runat="server"
                                            SelectMethod="GetVinculados" TypeName="Glass.Data.DAL.ClienteDAO">
                                            <SelectParameters>
                                                <asp:Parameter Name="idCli" DefaultValue="0" Type="UInt32" />
                                            </SelectParameters>
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

    <script type="text/javascript">
    
        if (FindControl("txtNumPedido", "input") != null && FindControl("txtNumPedido", "input") != undefined) {
            if (!FindControl("txtNumPedido", "input").disabled)
                FindControl("txtNumPedido", "input").focus();
        }
        else if (FindControl("txtLiberacao", "input") != null && FindControl("txtLiberacao", "input") != undefined) {
            if (!FindControl("txtLiberacao", "input").disabled)
                FindControl("txtLiberacao", "input").focus();
        }

        atualizaTipoNf(false);
        
    </script>

</asp:Content>
