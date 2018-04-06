<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DescontoPedido.aspx.cs" 
    Inherits="Glass.UI.Web.Utils.DescontoPedido"
    Title="Alterações no Pedido" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlParcelasSelecionar.ascx" TagName="ctrlParcelasSelecionar"
    TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlDadosDesconto.ascx" TagName="ctrlDadosDesconto"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc6" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
    
    var codCartao = <%= (int)Glass.Data.Model.Pagto.FormaPagto.Cartao %>;
    
    function alteraFastDelivery(isFastDelivery)
    {
        var alterar = <%= (Glass.Configuracoes.PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido > 0).ToString().ToLower() %>;
        if (!alterar)
            return;
        
        var novaData = isFastDelivery ? FindControl("hdfDataEntregaFD", "input").value : FindControl("hdfDataEntregaNormal", "input").value;
        FindControl("ctrlDataEntrega_txtData", "input").value = novaData;
    }
    
    function loadAjax(tipo)
    {
        var bloquearDadosClientePedido = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.BloquearDadosClientePedido.ToString().ToLower() %>;
        var usarControleDescontoFormaPagamentoDadosProduto = <%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>;

        if (!bloquearDadosClientePedido && !usarControleDescontoFormaPagamentoDadosProduto)
        {
            return null;
        }
        
        // O cliente não deve ser informado ao método caso a configuração de bloqueio de dados do cliente no pedido esteja desabilitada.
        var idCli = bloquearDadosClientePedido && FindControl("hdfIdCliente", "input") != null ? FindControl("hdfIdCliente", "input").value : "";
        // O tipo de venda do pedido não deve ser informado caso o controle de desconto por forma de pagamento e dados do produto esteja desabilitado.
        var tipoVenda = usarControleDescontoFormaPagamentoDadosProduto && FindControl("drpTipoVenda", "select") != null ? FindControl("drpTipoVenda", "select").value : "";

        var retorno = DescontoPedido.LoadAjax(tipo, idCli, tipoVenda);
        
        if (retorno.error != null)
        {
            alert(retorno.error.description);
            return null;
        }
        else if (retorno.value == null)
        {
            alert("Falha de Ajax ao carregar tipo '" + tipo + "'.");
        }
        
        return retorno.value;
    }
    
    function atualizaTipoVendaCli()
    {
        var ajax = loadAjax("tipoVenda");

        if (ajax == null || FindControl("drpTipoVenda", "select") == null)
        {
            return true;
        }
        
        var drpTipoVenda = FindControl("drpTipoVenda", "select");
        // Salva o valor selecionado antes de preencher novamente a drop por ajax
        var tipoVenda = drpTipoVenda.value;
        // Carrega os valores possíveis para o tipo venda
        drpTipoVenda.innerHTML = ajax;
        // Volta o tipo de venda que estava selecionado
        drpTipoVenda.value = tipoVenda;
        drpTipoVenda.onchange();
        
        // As formas de pagamento do cliente devem ser carregadas após recuperar o tipo de venda, para que as formas de pagamento corretas sejam recuperadas.
        // OBS.: o tipo de venda À Vista não deve recuperar a forma de pagamento Prazo. o tipo de venda À Prazo não deve recuperar a forma de pagamento Dinheiro.
        atualizaFormasPagtoCli();
    }
    
    // IMPORTANTE: ao alterar esse método, altere as telas DescontoPedido.aspx, CadDescontoFormaPagtoDadosProduto.aspx e CadPedido.aspx.
    function atualizaFormasPagtoCli()
    {
        var drpFormaPagto = FindControl("drpFormaPagto", "select");
        
        // Verifica se o controle de forma de pagamento existe na tela.
        if (drpFormaPagto == null)
        {
            return true;
        }

        // Salva em uma variável a forma de pagamento selecionada, antes do recarregamento das opções da Drop Down List.
        var idFormaPagtoAtual = drpFormaPagto.value;
        // Recupera as opções de forma de pagamento disponíveis.
        var ajax = loadAjax("formaPagto");
        
        // Verifica se ocorreu algum erro na chamada do Ajax.
        if (ajax.error != null)
        {
            alert(ajax.error.description);
            return false;
        }
        else if (ajax == null)
        {
            return false;
        }
        
        // Atualiza a Drop Down List com as formas de pagamento disponíveis.
        drpFormaPagto.innerHTML = ajax;

        // Variável criada para informar se a forma de pagamento pré-selecionada existe nas opções atuais da Drop Down List de forma de pagamento.
        var formaPagtoEncontrada = false;

        // Percorre cada forma de pagamento atual e verifica se a opção pré-selecionada existe entre elas.
        for (var i = 0; i < drpFormaPagto.options.length; i++)
        {
            if (drpFormaPagto.options[i].value == idFormaPagtoAtual)
            {
                formaPagtoEncontrada = true;
                break;
            }
        }
         
        // Caso a forma de pagamento exista nas opções atuais, seleciona ela na Drop.
        if (formaPagtoEncontrada)
        {
            drpFormaPagto.value = idFormaPagtoAtual;
        }

        drpFormaPagto.onchange();
    }
    
    function calcularDesconto(controle, tipoCalculo)
    {
        if (controle.value == "0")
            return;
        
        var idPedido = <%= !String.IsNullOrEmpty(Request["idPedido"]) ? Request["idPedido"] : "0" %>;
                
        var tipo = FindControl("drpTipoDesconto", "select").value;
        var desconto = parseFloat(controle.value.replace(',', '.'));
        if (isNaN(desconto))
            desconto = 0;
            
        var tipoAtual = FindControl("hdfTipoDesconto", "input").value;
        var descontoAtual = parseFloat(FindControl("hdfDesconto", "input").value.replace(',', '.'));
        if (isNaN(descontoAtual))
            descontoAtual = 0;
        
        var idFuncAtual = <%= Glass.Data.Helper.UserInfo.GetUserInfo.CodUser %>;
        var alterou = tipo != tipoAtual || desconto != descontoAtual;
        
        var descontoMaximo = FindControl('lblDescMaximo', 'span');
        if (descontoMaximo != null && FindControl("hdfUsarDescontoMax", "input").value != "true")
            descontoMaximo = DescontoPedido.PercDesconto(idPedido, idFuncAtual, alterou).value.replace(',', '.');
        else
            descontoMaximo = parseFloat(descontoMaximo.innerHTML.replace("%", "").replace(",", "."));
        
        var total = parseFloat(FindControl("hdfTotalSemDesconto", "input").value.replace(/\./g, "").replace(',', '.'));
        var totalProduto = tipoCalculo == 2 ? parseFloat(FindControl("lblTotalProd", "span").innerHTML.replace("R$", "").replace(" ", "").replace(/\./g, "").replace(',', '.')) : 0;
        var valorDescontoMaximo = total * (descontoMaximo / 100);

        //Busca o Desconto por parcela ou por Forma de pagamento e dados do produto
        var retDesconto = 0;
        var usarDescontoEmParcela = <%= Glass.Configuracoes.FinanceiroConfig.UsarDescontoEmParcela.ToString().ToLower() %>;
        var usarControleDescontoFormaPagamentoDadosProduto = <%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>;

        if (usarDescontoEmParcela && FindControl("drpParcelas","select") != null)
        {
            retDesconto = DescontoPedido.VerificaDescontoParcela(FindControl("drpParcelas","select").value, idPedido);
        }
        else if (usarControleDescontoFormaPagamentoDadosProduto)
        {
            var tipoVenda = FindControl("drpTipoVenda", "select").value;
            var idFormaPagto = FindControl("drpFormaPagto", "select").value;
            var idTipoCartao = FindControl("drpTipoCartao", "select").value;
            var idParcela = FindControl("drpParcelas", "select").value;

            retDesconto = DescontoPedido.VerificaDescontoFormaPagtoDadosProduto(idPedido, tipoVenda, idFormaPagto, idTipoCartao, idParcela);
        }

        if (retDesconto.error != null)
        {
            alert(retDesconto.error.description);
            return false;
        }   
        
        var valorDescontoProdutos = <%= GetDescontoProdutos() %> - (tipoCalculo == 2 ? parseFloat(FindControl("hdfValorDescontoAtual", "input").value.replace(',', '.')) : 0);
        var valorDescontoPedido = tipoCalculo == 2 ? <%= GetDescontoPedido() %> : 0;
        var descontoProdutos = parseFloat(((valorDescontoProdutos / (total > 0 ? total : 1)) * 100).toFixed(2));
        var descontoPedido = parseFloat(((valorDescontoPedido / (total > 0 ? total : 1)) * 100).toFixed(2));
        var descontoAdministrador = <%= Glass.Configuracoes.Geral.ManterDescontoAdministrador.ToString().ToLower() %>;
        var descontoSomar = descontoProdutos + (tipoCalculo == 2 ? descontoPedido : 0);
        var valorDescontoSomar = valorDescontoProdutos + (tipoCalculo == 2 ? valorDescontoPedido : 0);
        
        if (tipo == 2)
            desconto = (desconto / total) * 100;   
        
        //Se tiver desconto de parcela e o desconto da parcela for maior que o desconto maximo, não deve bloquear
        if (retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "" && retDesconto.value != undefined && parseFloat(retDesconto.value.replace(",", ".")) == parseFloat((desconto + descontoSomar).toFixed(2)))
        {
            return true;
        }
        
        // Chamado 12073. É necessário verificar se o desconto foi alterado, pois, caso o administrador tenha
        // aplicado o desconto, ou outro funcionárioa com permissão, então o desconto não deve ser validado,
        // a menos que o funcionário atual o altere.
        if(alterou)
        {
            if ((((desconto + descontoSomar) * total) / 100) != (<%= GetDescontoProdutos() %> + <%= GetDescontoPedido() %>)) {
                if (desconto + descontoSomar > descontoMaximo)
                {
                    var mensagem = "O desconto máximo permitido é de " + (tipo == 1 ? descontoMaximo + "%" : "R$ " + valorDescontoMaximo.toFixed(2).replace('.', ',')) + ".";
                    if (descontoProdutos > 0)
                        mensagem += "\nO desconto já aplicado aos produtos é de " + (tipo == 1 ? descontoProdutos + "%" : "R$ " + valorDescontoProdutos.toFixed(2).replace('.', ',')) + ".";
                
                    if (descontoPedido > 0)
                        mensagem += "\nO desconto já aplicado ao pedido é de " + (tipo == 1 ? descontoOrcamento + "%" : "R$ " + valorDescontoPedido.toFixed(2).replace('.', ',')) + ".";
                
                    alert(mensagem);
                    controle.value = tipo == 1 ? descontoMaximo - descontoSomar : (valorDescontoMaximo - valorDescontoSomar).toFixed(2).replace('.', ',') ;
                
                    if (parseFloat(controle.value.replace(',', '.')) < 0)
                        controle.value = "0";
                }
            }
        }
    }

    // Controla a visibilidade da forma de pagto, escondendo quando
    // o pedido for a vista e exibindo quando o pedido for a prazo
    function formaPagtoVisibility() {
        var control = FindControl("drpTipoVenda", "select");
        var formaPagto = FindControl("drpFormaPagto", "select");
        var parcela = FindControl("drpParcelas", "select");

        if (control == null || formaPagto == null)
        {
            return;
        }
            
        var usarControleDescontoFormaOagamentoDadosProduto = <%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>;

        // Se for à vista e o controle de desconto por forma de pagamento estiver habilitado, esconde somente a parcela.
        if (usarControleDescontoFormaOagamentoDadosProduto && control.value == 1)
        {
            formaPagto.style.display = "";

            if (parcela != null)
            {
                parcela.selectedIndex = 0;
                parcela.style.display = "none";
            }
        }
        // Se for obra, à vista, funcionário ou se estiver vazio, esconde a forma de pagamento e a parcela.
        else if (control.value == 0 || control.value == 1 || control.value == 5 || control.value == 6)
        {
            formaPagto.selectedIndex = 0;
            formaPagto.style.display = "none";

            if (parcela != null)
            {
                parcela.selectedIndex = 0;
                parcela.style.display = "none";
            }
        }
        else
        {
            formaPagto.style.display = "";

            if (parcela != null)
            {
                parcela.style.display = "";
            }
        }
    }

    // Evento acionado ao trocar o tipo de venda (à vista/à prazo)
    function tipoVendaChange(control, calcParcelas) {
        if (control == null)
            return;

        formaPagtoVisibility();

        // Ao alterar o tipo de venda, as formas de pagamento devem ser recarregadas para que o controle de desconto por forma de pagamento e dados do produto funcione corretamente.
        if (<%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>)
        {
            atualizaFormasPagtoCli();
        }

        formaPagtoChanged();
        
        document.getElementById("divObra").style.display = parseInt(control.value) == 5 ? "" : "none";
        document.getElementById("funcionarioComprador").style.display = parseInt(control.value) == 6 ? "" : "none";
        
        var valorEntrada = document.getElementById("tdValorEntrada2").getElementsByTagName("input")[0];        

        if(control.value == 2 || (control.value == 1 && <%= Glass.Configuracoes.PedidoConfig.LiberarPedido.ToString().ToLower() %>))
        {
            valorEntrada.style.display = "";
            valorEntrada.value = valorEntrada.value;
        }            
        else
        {
            valorEntrada.style.display = "none"; 
            valorEntrada.value = "";
        }
        
        if (parseInt(control.value) != 6)
            FindControl("drpFuncVenda", "select").value = "";
        
        if (document.getElementById("divNumParc") != null)
            document.getElementById("divNumParc").style.display = parseInt(control.value) == 2 ? "" : "none";
        
        setParcelas(calcParcelas);
        if (typeof <%= dtvPedido.ClientID %>_ctrlParcelas1 != "undefined")
            Parc_visibilidadeParcelas("<%= dtvPedido.ClientID %>_ctrlParcelas1");
        
        var descontoApenasAVista = <%= Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoApenasAVista.ToString().ToLower() %>;
        var exibirDesconto = !descontoApenasAVista || control.value == 1;
        
        showHideDesconto(exibirDesconto);
    }
    
    function showHideDesconto(exibirDesconto)
    {
        var drpTipoDesconto = FindControl("drpTipoDesconto", "select");
        if (drpTipoDesconto == null)
            return;
        
        var txtDesconto = FindControl("txtDesconto", "input");
        var lblDescontoVista = FindControl("lblDescontoVista", "span");
        
        drpTipoDesconto.style.display = exibirDesconto ? "" : "none";
        txtDesconto.style.display = exibirDesconto ? "" : "none";
        lblDescontoVista.style.display = !exibirDesconto ? "" : "none";
        
        txtDesconto.onchange();    
    }
    
    function callbackSetParcelas()
    {
        setParcelas(true);
        if (typeof <%= dtvPedido.ClientID %>_ctrlParcelas1 != "undefined")
            Parc_visibilidadeParcelas("<%= dtvPedido.ClientID %>_ctrlParcelas1");
            
        var descontoApenasAVista = <%= Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoApenasAVista.ToString().ToLower() %>;
        var exibirDesconto = !descontoApenasAVista || FindControl("drpTipoVenda", "select").value == 1;
            
        // Verifica se a empresa permite desconto para pedidos à vista com uma parcela
        if (<%= Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoUmaParcela.ToString().ToLower() %>)
            showHideDesconto(FindControl("hdfNumParcelas", "input").value == "1" || exibirDesconto);
    }
    
    function setParcelas(calcParcelas)
    {        
        var nomeControleParcelas = "<%= dtvPedido.ClientID %>_ctrlParcelas1";
        if (document.getElementById(nomeControleParcelas + "_tblParcelas") == null)
            return;
        
        var drpTipoVenda = FindControl("drpTipoVenda", "select");
        
        if (drpTipoVenda == null)
            return;
            
        FindControl("hdfExibirParcela", "input").value = drpTipoVenda.value == 2;
        FindControl("hdfCalcularParcela", "input").value = (calcParcelas == false ? false : true).toString();
    }

    // Evento acionado quando a forma de pagamento é alterada
    function formaPagtoChanged()
    {
        var formaPagto = FindControl("drpFormaPagto", "select");
        var tipoCartao = FindControl("drpTipoCartao", "select");

        if (formaPagto == null)
        {
            return true;
        }

        if (tipoCartao != null)
        {
            // Caso a forma de pagamento atual não seja Cartão, esconde o controle de tipo de cartão e desmarca a opção selecionada.
            if (formaPagto.value != codCartao)
            {
                tipoCartao.style.display = "none";
                tipoCartao.selectedIndex = 0;
            }
            else
            {
                tipoCartao.style.display = "";
            }
        }
    }
    
    function setObra(idCliente, idObra, descrObra, saldo)
    {
        FindControl("hdfIdObra", "input").value = idObra;
        FindControl("txtObra", "input").value = descrObra;
        FindControl("lblSaldoObra", "span").innerHTML = saldo.replace(/\n/g, "<br />");
        
        if (idCliente > 0)
        {
            FindControl("txtNumCli", "input").value = idCliente;
            getCli(FindControl("txtNumCli", "input"));
        }
    }
    
    function verificaDataEntrega(controle)
    {
        if (<%= (Glass.Configuracoes.PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido == 0).ToString().ToLower() %>)
            return true;
            
        var textoDataMinima = FindControl("hdfDataEntregaNormal", "input").value;
        var dataControle = textoDataMinima.split("/");
        var dataMinima = new Date(dataControle[2], parseInt(dataControle[1], 10) - 1, dataControle[0]);
        var isDataMinima = <%= GetBloquearDataEntrega().ToString().ToLower() %>;
        
        dataControle = controle.value.split("/");
        var dataAtual = new Date(dataControle[2], parseInt(dataControle[1], 10) - 1, dataControle[0]);
        
        var fastDelivery = FindControl("chkFastDelivery", "input");
        fastDelivery = fastDelivery != null ? fastDelivery.checked : false;

        if (isDataMinima && !fastDelivery && dataAtual < dataMinima)
        {
            alert("Não é possível escolher uma data anterior a " + textoDataMinima + ".");
            controle.value = textoDataMinima;
            dataEntregaAntiga = textoDataMinima;
            return false;
        }
        else if (MetodosAjax.IsDiaUtil(dataAtual).value == "false")
        {
            alert("Não é possível escolher sábado, domingo ou feriado como dia de entrega.");
            controle.value = dataEntregaAntiga;
            return false;
        }
        else
            dataEntregaAntiga = controle.value;
        
        return true;
    }

        function exibirComposicao(botao, idProdPed) {

            var linha = document.getElementById("pp_" + idProdPed);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " Composição";
        }
    
    </script>

    <table id="principal" runat="server">
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfUsarDescontoMax" runat="server" Value="false" />
                <asp:DetailsView ID="dtvPedido" runat="server" AutoGenerateRows="False" DataSourceID="odsPedido"
                    DefaultMode="Edit" GridLines="None" DataKeyNames="IdPedido">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <span id="dadosPedido">
                                    <table>
                                        <tr>
                                            <td style="font-weight: bold">
                                                Pedido
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblIdPedido" runat="server" Text='<%# Eval("IdPedido") %>' Font-Size="Medium"></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Desconto Atual
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblDescontoAtual" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Acréscimo Atual
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblAcrescimoAtual" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Total
                                            </td>
                                            <td>
                                                <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("Total", "{0:c}") %>' ForeColor="Blue"></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Total Conf.
                                            </td>
                                            <td>
                                                <asp:Label ID="lblTotalEspelho" runat="server" Text='<%# Eval("TotalEspelho", "{0:c}") %>'
                                                    ForeColor="Blue"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                </span>
                                <table id="alterarPedido">
                                    <tr>
                                        <td style="font-weight: bold">Cód. Ped. Cli. </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtCodPedCli" runat="server" MaxLength="20" Text='<%#  Bind("CodCliente") %>'
                                                ReadOnly='<%# Importado() %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Funcionário
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpFunc" runat="server" DataSourceID="odsFuncionario" DataTextField="Nome"
                                                DataValueField="IdFunc" SelectedValue='<%# Bind("IdFunc") %>' AppendDataBoundItems="True"
                                                OnDataBinding="drpFunc_DataBinding">
                                            </asp:DropDownList>
                                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedores"
                                                TypeName="Glass.Data.DAL.FuncionarioDAO">
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Tipo Venda
                                        </td>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoVenda" runat="server" SelectedValue='<%# Bind("TipoVenda") %>'
                                                            onchange="tipoVendaChange(this, true);" Enabled='<%# !(bool)Eval("RecebeuSinal") || Glass.Configuracoes.PedidoConfig.LiberarPedido %>'
                                                            DataSourceID="odsTipoVenda" DataTextField="Descr" DataValueField="Id" 
                                                            >
                                                        </asp:DropDownList>
                                                        <div id="divObra" style="display: none">
                                                            <asp:TextBox ID="txtObra" runat="server" Enabled="false" Width="200px" Text='<%# Eval("DescrObra") %>'></asp:TextBox>
                                                            <asp:ImageButton ID="imbObra" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick='<%# "openWindow(560, 650, \"SelObra.aspx?situacao=4&tipo=1&idsPedidosIgnorar=" + Eval("IdPedido") + "&idCliente=" + Eval("IdCli") + "\"); return false;" %>' />
                                                            <br />
                                                            <asp:Label ID="lblSaldoObra" runat="server" Text='<%# Eval("DescrSaldoObraPedidos") %>'></asp:Label>
                                                            <asp:HiddenField ID="hdfIdObra" runat="server" Value='<%# Bind("IdObra") %>' />
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <div id="divNumParc">
                                                            <table>
                                                                <tr>
                                                                    <td nowrap="nowrap" style="font-weight: bold">
                                                                        Num Parc.:
                                                                    </td>
                                                                    <td nowrap="nowrap">
                                                                        <uc1:ctrlParcelasSelecionar ID="ctrlParcelasSelecionar1" runat="server" ParcelaPadrao='<%# Bind("IdParcela") %>'
                                                                            NumeroParcelas='<%# Bind("NumParc") %>' OnLoad="ctrlParcelasSelecionar1_Load"
                                                                            CallbackSelecaoParcelas="callbackSetParcelas"/>
                                                                        <asp:HiddenField ID="hdfDataBase" runat="server" OnLoad="hdfDataBase_Load" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table cellpadding="0" cellspacing="0" id="funcionarioComprador" style='<%# ((bool)Eval("VendidoFuncionario")) ? "": "display: none; " %>padding-top: 2px'>
                                                <tr>
                                                    <td>
                                                        Funcionário comp.:&nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpFuncVenda" runat="server" DataSourceID="odsFuncVenda" DataTextField="Nome"
                                                            DataValueField="IdFunc" AppendDataBoundItems="True" SelectedValue='<%# Bind("IdFuncVenda") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Forma Pagto
                                        </td>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="true" DataSourceID="odsFormaPagto"
                                                            DataTextField="Descricao" DataValueField="IdFormaPagto" SelectedValue='<%# Bind("IdFormaPagto") %>'
                                                            onchange="formaPagtoChanged();">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoCartao" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCartao"
                                                            DataTextField="Descricao" DataValueField="IdTipoCartao" 
                                                            SelectedValue='<%# Bind("IdTipoCartao") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Valor Entrada
                                        </td>
                                        <td align="left" id="tdValorEntrada2">
                                            <uc4:ctrlTextBoxFloat ID="ctrValEntrada" runat="server" 
                                                Value='<%# Bind("ValorEntrada") %>' Visible='<%# !(bool)Eval("RecebeuSinal") %>' />
                                            <asp:Label ID="lblValor" runat="server" Text='<%# Eval("ConfirmouRecebeuSinal") %>'
                                                Visible='<%# Eval("RecebeuSinal") %>'></asp:Label>
                                            <asp:HiddenField Value='<%# Eval("ValorEntrada") %>' ID="hdfValorEntrada" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto Máx. Produto
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescProd" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto Cliente
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescCli" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto Máximo
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescMaximo" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold" nowrap="nowrap">
                                            Novo desconto
                                        </td>
                                        <td align="left">
                                            <table class="pos" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoDesconto" runat="server" SelectedValue='<%# Bind("TipoDesconto") %>'
                                                            onchange="calcularDesconto(FindControl('txtDesconto', 'input'), 1)">
                                                            <asp:ListItem Value="2">R$</asp:ListItem>
                                                            <asp:ListItem Value="1">%</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="txtDesconto" runat="server" Text='<%# Bind("Desconto") %>'
                                                            onchange="calcularDesconto(this, 1)" Width="70px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                                        <asp:Label ID="lblDescontoVista" runat="server" ForeColor="Blue" Text="Desconto só pode ser dado em pedidos à vista"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                        <uc2:ctrlDadosDesconto ID="ctrlDadosDesconto" runat="server" IsPedidoFastDelivery='<%# Eval("FastDelivery") %>'
                                                            TaxaFastDelivery='<%# Glass.Configuracoes.PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery %>' OnLoad="ctrlDadosDesconto_Load" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold" nowrap="nowrap">
                                            Novo acréscimo
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpTipoAcrescimo" runat="server" SelectedValue='<%# Bind("TipoAcrescimo") %>'>
                                                <asp:ListItem Value="2">R$</asp:ListItem>
                                                <asp:ListItem Value="1">%</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:TextBox ID="txtAcrescimo" runat="server" Text='<%# Bind("Acrescimo") %>'
                                                Width="70px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Observação
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Obs") %>' Width="300px" TextMode="MultiLine"
                                                Rows="4"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            <asp:Label ID="lblDataEntrega" runat="server" OnLoad="DataEntrega_Load" Text="Data Entrega"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <uc5:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataEntregaString") %>'
                                                ExibirHoras="False" OnLoad="DataEntrega_Load" onchange="verificaDataEntrega(this)" />
                                            <br />
                                            <asp:Label ID="Label1" runat="server" OnLoad="DataEntrega_Load" ForeColor="Blue"
                                                Text="Alterando esta data de entrega, a data de entrega e data de entrega da fábrica na conferência também serão alteradas."
                                                Width="350px"></asp:Label>
                                            <asp:HiddenField ID="hdfDataEntregaNormal" runat="server" />
                                            <asp:HiddenField ID="hdfDataEntregaFD" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Tipo de Entrega
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpTipoEntrega" runat="server" DataSourceID="odsTipoEntrega"
                                                DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoEntrega") %>'
                                                OnPreRender="drpTipoEntrega_PreRender">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Fast Delivery
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkFastDelivery" runat="server" Checked='<%# Bind("FastDelivery") %>'
                                                OnLoad="FastDelivery_Load" onclick="alteraFastDelivery(this.checked)" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            <asp:Label ID="lblOrdemCargaParcial" runat="server" Text="Ordem de Carga Parcial" 
                                                OnLoad="chkOrdemCargaParcial_Load"></asp:Label>
                                            
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkOrdemCargaParcial" runat="server" Checked='<%# Bind("OrdemCargaParcial") %>' 
                                                OnLoad="chkOrdemCargaParcial_Load" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            <asp:Label ID="lblValorFrete" runat="server" Text="Valor do Frete" OnLoad="txtValorFrete_Load"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorFrete" runat="server" Text='<%# Bind("ValorEntrega") %>' OnLoad="txtValorFrete_Load"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            
                                            <asp:Label ID="Label7" runat="server" Text="Deve Transferir?" 
                                                onload="chkDeveTransferir_Load"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chkDeveTransferir" runat="server" 
                                                Checked='<%# Bind("DeveTransferir") %>' onload="chkDeveTransferir_Load" />
                                        </td>
                                    </tr>
                                    <tr style='<%= Glass.Configuracoes.PedidoConfig.LiberarPedido ? "": "display: none" %>'>
                                        <td style="font-weight: bold">
                                            <asp:Label ID="lblObsLiberacao" runat="server" OnLoad="lblObsLiberacao_Load" Text="Obs. Liberação "></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtObsLiberacao" runat="server" MaxLength="300" Rows="4" TextMode="MultiLine"
                                                Width="300px" Text='<%# Bind("ObsLiberacao") %>' OnLoad="txtObsLiberacao_Load"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style='<%= Glass.Configuracoes.PedidoConfig.Comissao.PerComissaoPedido ? "": "display: none" %>'>
                                        <td style="font-weight: bold">
                                            Percentual de Comissão por Pedido
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtPercComissaoPedido" runat="server" TextMode="SingleLine" Width="70px"
                                                onkeypress="return soNumeros(event, false, true);" Text='<%# Bind("PercentualComissao") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">Loja
                                        </td>
                                        <td align="left">
                                            <uc6:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" MostrarTodas="false" MostrarVazia="false" SelectedValue='<%# Bind("IdLoja") %>' 
                                                OnLoad="drpLoja_Load"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">Transportador</td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                                DataSourceID="odsTransportador" DataTextField="Name" DataValueField="Id"
                                                SelectedValue='<%# Bind("IdTransportador") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <uc3:ctrlParcelas ID="ctrlParcelas1" runat="server" Datas='<%# Bind("DatasParcelas") %>'
                                    NumParcelas="4" NumParcelasLinha="4" OnDataBinding="ctrlParcelas1_DataBinding"
                                    OnLoad="ctrlParcelas1_Load" Valores='<%# Bind("ValoresParcelas") %>' />
                                <asp:HiddenField ID="hdfDesconto" runat="server" Value='<%# Eval("Desconto") %>' />
                                <asp:HiddenField ID="hdfTipoDesconto" runat="server" Value='<%# Eval("TipoDesconto") %>' />
                                <asp:HiddenField ID="hdfAcrescimo" runat="server" Value='<%# Eval("Acrescimo") %>' />
                                <asp:HiddenField ID="hdfTipoAcrescimo" runat="server" Value='<%# Eval("TipoAcrescimo") %>' />
                                <asp:HiddenField ID="hdfExibirParcela" runat="server" />
                                <asp:HiddenField ID="hdfCalcularParcela" runat="server" />
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCli") %>' />
                                <asp:HiddenField ID="hdfFastDelivery" runat="server" Value='<%# Eval("FastDelivery") %>' />

                                <script type="text/javascript">
                                    var descontoApenasAVista = <%= Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoApenasAVista.ToString().ToLower() %>;
                                    var exibirDesconto = !descontoApenasAVista || '<%# Eval("TipoVenda") %>' == '1';
        
                                    showHideDesconto(exibirDesconto);
                                </script>

                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" OnClientClick="return confirm(&quot;Deseja atualizar os dados do pedido?&quot;)"
                                    Text="Atualizar" ValidationGroup="pedido" />
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <asp:DetailsView ID="dtvPedidoConf" runat="server" AutoGenerateRows="False" DataSourceID="odsPedido"
                    DefaultMode="Edit" GridLines="None" DataKeyNames="IdPedido" Visible="False">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <span id="dadosPedido">
                                    <table>
                                        <tr>
                                            <td style="font-weight: bold">
                                                Pedido
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblIdPedido" runat="server" Text='<%# Eval("IdPedido") %>' Font-Size="Medium"></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Desconto Atual
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblDescontoAtual" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Acréscimo Atual
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblAcrescimoAtual" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Total
                                            </td>
                                            <td>
                                                <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("Total", "{0:c}") %>' ForeColor="Blue"></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Total Conf.
                                            </td>
                                            <td>
                                                <asp:Label ID="lblTotalEspelho" runat="server" Text='<%# Eval("TotalEspelho", "{0:c}") %>'
                                                    ForeColor="Blue"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                </span>
                                <table id="alterarPedido">
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto Máx. Produto
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescProd" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto Cliente
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescCli" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto Máximo
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescMaximo" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto
                                        </td>
                                        <td align="left">
                                            <table class="pos" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoDesconto" runat="server" SelectedValue='<%# Bind("TipoDesconto") %>'
                                                            Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>' onchange="calcularDesconto(FindControl('txtDesconto', 'input'), 1)">
                                                            <asp:ListItem Value="2">R$</asp:ListItem>
                                                            <asp:ListItem Value="1">%</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="txtDesconto" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                            Enabled='<%# Eval("DescontoEnabled") %>' onchange="calcularDesconto(this, 1)"
                                                            Text='<%# Bind("Desconto") %>' Width="70px"></asp:TextBox>
                                                        <asp:Label ID="lblDescontoVista" runat="server" ForeColor="Blue" Text="Desconto só pode ser dado em pedidos à vista"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                        <uc2:ctrlDadosDesconto ID="ctrlDadosDesconto" runat="server" TaxaFastDelivery='<%# Glass.Configuracoes.PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery %>'
                                                            OnLoad="ctrlDadosDesconto_Load" IsPedidoFastDelivery='<%# Eval("FastDelivery") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfDesconto" runat="server" Value='<%# Eval("Desconto") %>' />
                                            <asp:HiddenField ID="hdfTipoDesconto" runat="server" Value='<%# Eval("TipoDesconto") %>' />
                                            <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                            <asp:HiddenField ID="hdfFastDelivery" runat="server" Value='<%# Eval("FastDelivery") %>' />
                                            <asp:HiddenField ID="hdfIdFunc" runat="server" Value='<%# Bind("IdFunc") %>' />
                                            <asp:HiddenField ID="hdfValorEntrada" runat="server" Value='<%# Bind("ValorEntrada") %>' />
                                        </td>
                                    </tr>
                                    <tr style='<%= Glass.Configuracoes.PedidoConfig.Comissao.PerComissaoPedido ? "": "display: none" %>'>
                                        <td style="font-weight: bold">
                                            Percentual de Comissão por Pedido
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtPercComissaoPedido" runat="server" TextMode="SingleLine" Width="70px"
                                                onkeypress="return soNumeros(event, false, true);" Text='<%# Bind("PercentualComissao") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            <asp:Label ID="Label2" runat="server" Text="Observação"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtObsConf" runat="server" Rows="4" TextMode="MultiLine" Width="400px"
                                                Text='<%# Bind("Obs") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            <asp:Label ID="lblObsLiberacao" runat="server" OnLoad="lblObsLiberacao_Load" Text="Obs. Liberação "></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtObsLiberacao" runat="server" MaxLength="300" OnLoad="txtObsLiberacao_Load"
                                                Rows="4" Text='<%# Bind("ObsLiberacao") %>' TextMode="MultiLine" Width="400px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>

                                <script type="text/javascript">
                                    var descontoApenasAVista = <%= Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoApenasAVista.ToString().ToLower() %>;
                                    var exibirDesconto = !descontoApenasAVista || '<%# Eval("TipoVenda") %>' == '1';
        
                                    showHideDesconto(exibirDesconto);
                                </script>

                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" OnClientClick="return confirm(&quot;Deseja atualizar os dados do pedido?&quot;)"
                                    Text="Atualizar" ValidationGroup="pedido" />
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
                <asp:DetailsView ID="dtvPedidoDescVendedor" runat="server" AutoGenerateRows="False"
                    DataSourceID="odsPedido" DefaultMode="Edit" GridLines="None" DataKeyNames="IdPedido"
                    Visible="False">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <span id="dadosPedido">
                                    <table>
                                        <tr>
                                            <td style="font-weight: bold">
                                                Pedido
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblIdPedido0" runat="server" Text='<%# Eval("IdPedido") %>' Font-Size="Medium"></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Desconto Atual
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblDescontoAtual" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Acréscimo Atual
                                            </td>
                                            <td align="left">
                                                <asp:Label ID="lblAcrescimoAtual" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Total
                                            </td>
                                            <td>
                                                <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("Total", "{0:c}") %>' ForeColor="Blue"></asp:Label>
                                                &nbsp;&nbsp;
                                            </td>
                                            <td style="font-weight: bold">
                                                Total Conf.
                                            </td>
                                            <td>
                                                <asp:Label ID="lblTotalEspelho" runat="server" Text='<%# Eval("TotalEspelho", "{0:c}") %>'
                                                    ForeColor="Blue"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                </span>
                                <table id="alterarPedido">
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto Máx. Produto
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescProd" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto Cliente
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescCli" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto Máximo
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescMaximo" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            Desconto
                                        </td>
                                        <td align="left">
                                            <table class="pos" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoDesconto" runat="server" SelectedValue='<%# Bind("TipoDesconto") %>'
                                                            Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>' onchange="calcularDesconto(FindControl('txtDesconto', 'input'), 1)">
                                                            <asp:ListItem Value="2">R$</asp:ListItem>
                                                            <asp:ListItem Value="1">%</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="txtDesconto" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                            Enabled='<%# Eval("DescontoEnabled") %>' onchange="calcularDesconto(this, 1)"
                                                            Text='<%# Bind("Desconto") %>' Width="70px"></asp:TextBox>
                                                        <asp:Label ID="lblDescontoVista" runat="server" ForeColor="Blue" Text="Desconto só pode ser dado em pedidos à vista"></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                        <uc2:ctrlDadosDesconto ID="ctrlDadosDesconto" runat="server" TaxaFastDelivery='<%# Glass.Configuracoes.PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery %>'
                                                            OnLoad="ctrlDadosDesconto_Load" IsPedidoFastDelivery='<%# Eval("FastDelivery") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfDesconto" runat="server" Value='<%# Eval("Desconto") %>' />
                                            <asp:HiddenField ID="hdfTipoDesconto" runat="server" Value='<%# Eval("TipoDesconto") %>' />
                                            <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                            <asp:HiddenField ID="hdfFastDelivery" runat="server" Value='<%# Eval("FastDelivery") %>' />
                                            <asp:HiddenField ID="hdfIdFunc" runat="server" Value='<%# Bind("IdFunc") %>' />
                                            <asp:HiddenField ID="hdfValorEntrada" runat="server" Value='<%# Bind("ValorEntrada") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            <asp:Label ID="Label5" runat="server" Text="Observação"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtObsConf0" runat="server" Rows="4" TextMode="MultiLine" Width="400px"
                                                Text='<%# Bind("Obs") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold">
                                            <asp:Label ID="lblObsLiberacao" runat="server" OnLoad="lblObsLiberacao_Load" Text="Obs. Liberação "></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtObsLiberacao" runat="server" MaxLength="300" OnLoad="txtObsLiberacao_Load"
                                                Rows="4" Text='<%# Bind("ObsLiberacao") %>' TextMode="MultiLine" Width="400px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>

                                <script type="text/javascript">
                                    var descontoApenasAVista = <%= Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoApenasAVista.ToString().ToLower() %>;
                                    var exibirDesconto = !descontoApenasAVista || '<%# Eval("TipoVenda") %>' == '1';
        
                                    showHideDesconto(exibirDesconto);
                                </script>

                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <br />
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" OnClientClick="return confirm(&quot;Deseja atualizar os dados do pedido?&quot;)"
                                    Text="Atualizar" ValidationGroup="pedido" />
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
        <tr runat="server" id="produtosPedido">
            <td align="center">
                <div id="divAmbiente" runat="server">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label3" runat="server" Text="Ambiente" ForeColor="#0066FF"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="drpAmbiente" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                    DataSourceID="odsAmbientePedido" DataTextField="PecaVidro" DataValueField="IdAmbientePedido">
                                    <asp:ListItem Value="0">Todos</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
                <br />
                <span class="subtitle1">Produtos do pedido </span>
                <asp:GridView ID="grdProdutos" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedido" GridLines="None" EmptyDataText="Não há produtos para esse pedido."
                    OnRowCommand="grdProdutos_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirComposicao(this, " + Eval("IdProdPed") + "); return false" %>'
                                    Width="10px" ToolTip="Exibir Composicao" Visible='<%# Eval("ExibirFilhosDescontoPedido") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgRemover" runat="server" CommandArgument='<%# Eval("IdProdPed") %>'
                                    CommandName="Remover" ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick='<%# "if (!validate(\"" + Eval("IdProdPed") + "rem\") || !confirm(\"Deseja remover esse produto do pedido?\")) return false" %>'
                                    ToolTip="Remover" ValidationGroup='<%# Eval("IdProdPed") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Ambiente" HeaderText="Ambiente" SortExpression="Ambiente" />
                        <asp:BoundField DataField="DescricaoProdutoComBenef" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="AlturaLista" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                        <asp:TemplateField HeaderText="Qtde Remover">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" Width="50px" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input', this.parent).value), true)"></asp:TextBox>
                                <asp:RangeValidator ID="rgvQtde" runat="server" ControlToValidate="txtQtde" Display="Dynamic"
                                    ErrorMessage='<%# "Mínimo: 1  Máximo: " + ((float)Eval("Qtde") - (float)Eval("QtdSaida")) %>'
                                    MaximumValue='<%# (float)Eval("Qtde") - (float)Eval("QtdSaida") %>' MinimumValue="0,0000001"
                                    Type="Double" ValidationGroup='<%# Eval("IdProdPed") + "rem" %>' Style="white-space: nowrap"></asp:RangeValidator>
                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ValidationGroup='<%# Eval("IdProdPed") + "rem" %>'
                                    ControlToValidate="txtQtde" Display="Dynamic" Style="white-space: nowrap" ErrorMessage="Digite o valor que será removido"></asp:RequiredFieldValidator>
                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tot. M²" SortExpression="TotM">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("TotM") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ValorVendido" HeaderText="Valor Vend." SortExpression="ValorVendido"
                            DataFormatString="{0:c}" />
                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("TotalProdTelaDesconto", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Benef." SortExpression="ValorBenef">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("ValorBenefProdTelaDesconto", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("ValorBenef") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                </td> </tr><asp:HiddenField ID="hdfIdProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                <tr id="pp_<%# Eval("IdProdPed") %>" style="display: none;">
                                    <td colspan="13">
                                        <asp:GridView ID="grdProdutosComposicao" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                                            DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedidoComposicao" GridLines="None" EmptyDataText="Não há produtos para esse pedido."
                                            OnRowCommand="grdProdutos_RowCommand">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/mais.gif" OnClientClick='<%# "exibirComposicao(this, " + Eval("IdProdPed") + "); return false" %>'
                                                            Width="10px" ToolTip="Exibir Composicao" Visible='<%# Eval("ExibirFilhosDescontoPedido") %>'/>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgRemover" runat="server" CommandArgument='<%# Eval("IdProdPed") %>'
                                                            CommandName="Remover" ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick='<%# "if (!validate(\"" + Eval("IdProdPed") + "rem\") || !confirm(\"Deseja remover esse produto do pedido?\")) return false" %>'
                                                            ToolTip="Remover" ValidationGroup='<%# Eval("IdProdPed") %>'/>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Ambiente" HeaderText="Ambiente" SortExpression="Ambiente" />
                                                <asp:BoundField DataField="DescricaoProdutoComBenef" HeaderText="Produto" SortExpression="DescrProduto" />
                                                <asp:BoundField DataField="AlturaLista" HeaderText="Altura" SortExpression="Altura" />
                                                <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                                                <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                                                <asp:TemplateField HeaderText="Qtde Remover">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtQtde" runat="server" Width="50px" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input', this.parent).value), true)"></asp:TextBox>
                                                        <asp:RangeValidator ID="rgvQtde" runat="server" ControlToValidate="txtQtde" Display="Dynamic"
                                                            ErrorMessage='<%# "Mínimo: 1  Máximo: " + ((float)Eval("Qtde") - (float)Eval("QtdSaida")) %>'
                                                            MaximumValue='<%# (float)Eval("Qtde") - (float)Eval("QtdSaida") %>' MinimumValue="0,0000001"
                                                            Type="Double" ValidationGroup='<%# Eval("IdProdPed") + "rem" %>' Style="white-space: nowrap"></asp:RangeValidator>
                                                        <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ValidationGroup='<%# Eval("IdProdPed") + "rem" %>'
                                                            ControlToValidate="txtQtde" Display="Dynamic" Style="white-space: nowrap" ErrorMessage="Digite o valor que será removido"></asp:RequiredFieldValidator>
                                                        <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tot. M²" SortExpression="TotM">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("TotM") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="ValorVendido" HeaderText="Valor Vend." SortExpression="ValorVendido"
                                                    DataFormatString="{0:c}" />
                                                <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label4" runat="server" Text='<%# Eval("TotalProdTelaDesconto", "{0:C}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Valor Benef." SortExpression="ValorBenef">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("ValorBenefProdTelaDesconto", "{0:C}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("ValorBenef") %>'></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        </td> </tr><asp:HiddenField ID="hdfIdProdPedComposicao" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                                        <tr id="pp_<%# Eval("IdProdPed") %>" style="display: none;">
                                                            <td colspan="13">
                                                                <asp:GridView ID="grdProdutosComposicaoChild" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                                                                    DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedidoComposicaoChild" GridLines="None" EmptyDataText="Não há produtos para esse pedido."
                                                                    OnRowCommand="grdProdutos_RowCommand">
                                                                    <Columns>
                                                                        <asp:TemplateField>
                                                                            <ItemTemplate>
                                                                                <asp:ImageButton ID="imgRemover" runat="server" CommandArgument='<%# Eval("IdProdPed") %>'
                                                                                    CommandName="Remover" ImageUrl="~/Images/ExcluirGrid.gif" OnClientClick='<%# "if (!validate(\"" + Eval("IdProdPed") + "rem\") || !confirm(\"Deseja remover esse produto do pedido?\")) return false" %>'
                                                                                    ToolTip="Remover" ValidationGroup='<%# Eval("IdProdPed") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="Ambiente" HeaderText="Ambiente" SortExpression="Ambiente" />
                                                                        <asp:BoundField DataField="DescricaoProdutoComBenef" HeaderText="Produto" SortExpression="DescrProduto" />
                                                                        <asp:BoundField DataField="AlturaLista" HeaderText="Altura" SortExpression="Altura" />
                                                                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                                                                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                                                                        <asp:TemplateField HeaderText="Qtde Remover">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox ID="txtQtde" runat="server" Width="50px" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input', this.parent).value), true)"></asp:TextBox>
                                                                                <asp:RangeValidator ID="rgvQtde" runat="server" ControlToValidate="txtQtde" Display="Dynamic"
                                                                                    ErrorMessage='<%# "Mínimo: 1  Máximo: " + ((float)Eval("Qtde") - (float)Eval("QtdSaida")) %>'
                                                                                    MaximumValue='<%# (float)Eval("Qtde") - (float)Eval("QtdSaida") %>' MinimumValue="0,0000001"
                                                                                    Type="Double" ValidationGroup='<%# Eval("IdProdPed") + "rem" %>' Style="white-space: nowrap"></asp:RangeValidator>
                                                                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ValidationGroup='<%# Eval("IdProdPed") + "rem" %>'
                                                                                    ControlToValidate="txtQtde" Display="Dynamic" Style="white-space: nowrap" ErrorMessage="Digite o valor que será removido"></asp:RequiredFieldValidator>
                                                                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Tot. M²" SortExpression="TotM">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("TotM") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="ValorVendido" HeaderText="Valor Vend." SortExpression="ValorVendido"
                                                                            DataFormatString="{0:c}" />
                                                                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("TotalProdTelaDesconto", "{0:C}") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Valor Benef." SortExpression="ValorBenef">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("ValorBenefProdTelaDesconto", "{0:C}") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("ValorBenef") %>'></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                    <PagerStyle CssClass="pgr" />
                                                                    <AlternatingRowStyle CssClass="alt" />
                                                                </asp:GridView>
                                                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosPedidoComposicaoChild" runat="server" EnablePaging="True"
                                                                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetDescontoAdminCount"
                                                                    SelectMethod="GetDescontoAdminList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                                                                    TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
                                                                    <SelectParameters>
                                                                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                                                                        <asp:ControlParameter ControlID="drpAmbiente" Name="idAmbiente" PropertyName="SelectedValue"
                                                                            Type="UInt32" />
                                                                        <asp:Parameter DefaultValue="true" Name="visiveis" Type="Boolean" />
                                                                        <asp:Parameter DefaultValue="false" Name="ignorarFiltroProdComp" Type="Boolean" />
                                                                        <asp:Parameter DefaultValue="true" Name="prodComp" Type="Boolean" />
                                                                        <asp:ControlParameter ControlID="hdfIdProdPedComposicao" PropertyName="Value" Name="idProdPedParent" Type="UInt32" />
                                                                    </SelectParameters>
                                                                </colo:VirtualObjectDataSource>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                            <PagerStyle CssClass="pgr" />
                                            <AlternatingRowStyle CssClass="alt" />
                                        </asp:GridView>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosPedidoComposicao" runat="server" EnablePaging="True"
                                            MaximumRowsParameterName="pageSize" SelectCountMethod="GetDescontoAdminCount"
                                            SelectMethod="GetDescontoAdminList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                                            TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
                                            <SelectParameters>
                                                <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                                                <asp:ControlParameter ControlID="drpAmbiente" Name="idAmbiente" PropertyName="SelectedValue"
                                                    Type="UInt32" />
                                                <asp:Parameter DefaultValue="true" Name="visiveis" Type="Boolean" />
                                                <asp:Parameter DefaultValue="false" Name="ignorarFiltroProdComp" Type="Boolean" />
                                                <asp:Parameter DefaultValue="true" Name="prodComp" Type="Boolean" />
                                                <asp:ControlParameter ControlID="hdfIdProdPed" PropertyName="Value" Name="idProdPedParent" Type="UInt32" />
                                            </SelectParameters>
                                        </colo:VirtualObjectDataSource>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <br />
                <span class="subtitle1">Produtos removidos do pedido </span>
                <asp:GridView ID="grdProdutosRem" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="IdProdPed" DataSourceID="odsProdutosPedidoRem" GridLines="None"
                    EmptyDataText="Não há produtos removidos para esse pedido." OnRowCommand="grdProdutosRem_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgRestaurar" runat="server" CommandArgument='<%# Eval("IdProdPed") %>'
                                    CommandName="Restaurar" ImageUrl="~/Images/Insert.gif" OnClientClick='<%# "if (!validate(\"" + Eval("IdProdPed") + "rest\") || !confirm(\"Deseja restaurar esse produto ao pedido?\")) return false" %>'
                                    ToolTip="Restaurar" ValidationGroup='<%# Eval("IdProdPed") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Ambiente" HeaderText="Ambiente" SortExpression="Ambiente" />
                        <asp:BoundField DataField="DescricaoProdutoComBenef" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="AlturaLista" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="QtdeInvisivel" HeaderText="Qtde" SortExpression="QtdeInvisivel" />
                        <asp:TemplateField HeaderText="Qtde Restaurar">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" Width="50px" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input', this.parent).value), true)"></asp:TextBox>
                                <asp:RangeValidator ID="rgvQtde" runat="server" ControlToValidate="txtQtde" Display="Dynamic"
                                    ErrorMessage='<%# "Mínimo: 1  Máximo: " + Eval("QtdeInvisivel") %>' MaximumValue='<%# Eval("QtdeInvisivel") %>'
                                    MinimumValue="0,0000001" Type="Double" ValidationGroup='<%# Eval("IdProdPed") + "rest" %>'
                                    Style="white-space: nowrap"></asp:RangeValidator>
                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ValidationGroup='<%# Eval("IdProdPed") + "rest" %>'
                                    ControlToValidate="txtQtde" Display="Dynamic" Style="white-space: nowrap" ErrorMessage="Digite o valor que será removido"></asp:RequiredFieldValidator>
                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tot. M²" SortExpression="TotM">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TotM") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("TotMTelaDesconto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ValorVendido" HeaderText="Valor Vend." SortExpression="ValorVendido"
                            DataFormatString="{0:c}" />
                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Total") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("TotalProdRemTelaDesconto", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Benef." SortExpression="ValorBenef">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("ValorBenefProdRemTelaDesconto", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("ValorBenef") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
    </table>

    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
        SelectMethod="GetElement" TypeName="Glass.Data.DAL.PedidoDAO" UpdateMethod="UpdateDescontoComTransacao"
        OnUpdated="odsPedido_Updated">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
        TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosPedido" runat="server" EnablePaging="True"
        MaximumRowsParameterName="pageSize" SelectCountMethod="GetDescontoAdminCount"
        SelectMethod="GetDescontoAdminList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
        TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
            <asp:ControlParameter ControlID="drpAmbiente" Name="idAmbiente" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:Parameter DefaultValue="true" Name="visiveis" Type="Boolean" />
            <asp:Parameter DefaultValue="false" Name="ignorarFiltroProdComp" Type="Boolean" />
            <asp:Parameter DefaultValue="false" Name="prodComp" Type="Boolean" />
            <asp:Parameter DefaultValue="0" Name="idProdPedParent" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosPedidoRem" runat="server" EnablePaging="True"
        MaximumRowsParameterName="pageSize" SelectCountMethod="GetDescontoAdminCount"
        SelectMethod="GetDescontoAdminList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
        TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
            <asp:ControlParameter ControlID="drpAmbiente" Name="idAmbiente" PropertyName="SelectedValue"
                Type="UInt32" />
            <asp:Parameter DefaultValue="false" Name="visiveis" Type="Boolean" />
            <asp:Parameter DefaultValue="false" Name="ignorarFiltroProdComp" Type="Boolean" />
            <asp:Parameter DefaultValue="false" Name="prodComp" Type="Boolean" />
            <asp:Parameter DefaultValue="0" Name="idProdPedParent" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForPedido"
        TypeName="Glass.Data.DAL.FormaPagtoDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoCartao" runat="server" SelectMethod="ObtemListaPorTipo"
        TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO">
        <SelectParameters>
            <asp:Parameter Name="tipo" Type="Int32" DefaultValue="0" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncVenda" runat="server" SelectMethod="GetOrdered"
        TypeName="Glass.Data.DAL.FuncionarioDAO">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAmbientePedido" runat="server" SelectMethod="GetForDescontoPedido"
        TypeName="Glass.Data.DAL.AmbientePedidoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoEntrega" runat="server" SelectMethod="GetTipoEntrega"
        TypeName="Glass.Data.Helper.DataSources">
    </colo:VirtualObjectDataSource>   
     <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTransportador" runat="server"
        SelectMethod="ObtemDescritoresTransportadores" TypeName="Glass.Global.Negocios.ITransportadorFluxo">
    </colo:VirtualObjectDataSource>

    <script type="text/javascript">
        var drpTipoVenda = FindControl("drpTipoVenda", "select");
        if (drpTipoVenda != null)
        {
            tipoVendaChange(drpTipoVenda, false);

            if (FindControl("hdfExibirParcela", "input") != null)
                FindControl("hdfExibirParcela", "input").value = drpTipoVenda.value == 2;

            if (FindControl("hdfCalcularParcela", "input") != null)
                FindControl("hdfCalcularParcela", "input").value = false;
        }

        var numCli = FindControl("hdfIdCliente", "input");
        if (numCli != null && numCli.value != "")
        {
            var tipoVenda = FindControl("drpTipoVenda", "select");
            var formaPagto = FindControl("drpFormaPagto", "select");

            var tva = tipoVenda != null ? tipoVenda.value : "";
            var fpa = formaPagto != null ? formaPagto.value : "";
            
            // É muito importante que o método atualizaTipoVendaCli seja chamado antes do método atualizaFormasPagtoCli, pois as formas de pagamento são recuperadas com base no tipo de venda do pedido.
            // OBS.: o método atualizaTipoVendaCli está sendo chamado dentro do método atualizaTipoVendaCli.
            atualizaTipoVendaCli();

            if (tipoVenda != null)
            {
                tipoVenda.value = tva;
            }

            if (formaPagto != null)
            {
                formaPagto.value = fpa;
            }
            
            if (tipoVenda != null)
            {
                tipoVenda.onchange();
            }
            
            if (formaPagto != null)
            {
                formaPagto.onchange();
            }
        }

        var idPedido = <%= !string.IsNullOrEmpty(Request["idPedido"]) ? Request["idPedido"] : "0" %>;
        var tipoVenda = FindControl("drpTipoVenda", "select");
        var formaPagto = FindControl("drpFormaPagto", "select");
        var tipoCartao = FindControl("drpTipoCartao", "select");
        var parcelas = FindControl("drpParcelas", "select");

        if (tipoVenda != null)
        {
            tipoVenda.onblur = function()
            {
                var retDesconto = DescontoPedido.VerificaDescontoFormaPagtoDadosProduto(idPedido, tipoVenda != null ? tipoVenda.value : "", formaPagto != null ? formaPagto.value : "",
                    tipoCartao != null ? tipoCartao.value : "", parcelas != null ? parcelas.value : "");

                if (retDesconto.error != null)
                {
                    alert(retDesconto.error.description);
                    return false;
                }
                else if (retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "")
                {
                    var txtDesconto = FindControl("txtDesconto","input");
                    var txtTipoDesconto = FindControl("drpTipoDesconto","select");

                    if (txtTipoDesconto != null)
                    {
                        txtTipoDesconto.value = 1;
                    }

                    if (txtDesconto != null)
                    {
                        txtDesconto.value = retDesconto.value.replace(".", ",");
                        txtDesconto.onchange();
                        txtDesconto.onblur();
                    }
                }
            }
        }

        if (formaPagto != null)
        {
            formaPagto.onblur = function()
            {
                var retDesconto = DescontoPedido.VerificaDescontoFormaPagtoDadosProduto(idPedido, tipoVenda != null ? tipoVenda.value : "", formaPagto != null ? formaPagto.value : "",
                    tipoCartao != null ? tipoCartao.value : "", parcelas != null ? parcelas.value : "");

                if (retDesconto.error != null)
                {
                    alert(retDesconto.error.description);
                    return false;
                }
                else if (retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "")
                {
                    var txtDesconto = FindControl("txtDesconto","input");
                    var txtTipoDesconto = FindControl("drpTipoDesconto","select");

                    if (txtTipoDesconto != null)
                    {
                        txtTipoDesconto.value = 1;
                    }

                    if (txtDesconto != null)
                    {
                        txtDesconto.value = retDesconto.value.replace(".", ",");
                        txtDesconto.onchange();
                        txtDesconto.onblur();
                    }
                }
            }
        }

        if (tipoCartao != null)
        {
            tipoCartao.onblur = function()
            {
                var retDesconto = DescontoPedido.VerificaDescontoFormaPagtoDadosProduto(idPedido, tipoVenda != null ? tipoVenda.value : "", formaPagto != null ? formaPagto.value : "",
                    tipoCartao != null ? tipoCartao.value : "", parcelas != null ? parcelas.value : "");

                if (retDesconto.error != null)
                {
                    alert(retDesconto.error.description);
                    return false;
                }
                else if (retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "")
                {
                    var txtDesconto = FindControl("txtDesconto","input");
                    var txtTipoDesconto = FindControl("drpTipoDesconto","select");

                    if (txtTipoDesconto != null)
                    {
                        txtTipoDesconto.value = 1;
                    }

                    if (txtDesconto != null)
                    {
                        txtDesconto.value = retDesconto.value.replace(".", ",");
                        txtDesconto.onchange();
                        txtDesconto.onblur();
                    }
                }
            }
        }

        if (parcelas != null)
        {
            parcelas.onblur = function()
            {            
                //Busca o Desconto por parcela ou por Forma de pagamento e dados do produto
                var retDesconto = null;
                var usarDescontoEmParcela = <%= Glass.Configuracoes.FinanceiroConfig.UsarDescontoEmParcela.ToString().ToLower() %>;
                var usarControleDescontoFormaPagamentoDadosProduto = <%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>;

                if (usarDescontoEmParcela && parcelas != null)
                {
                    retDesconto = DescontoPedido.VerificaDescontoParcela(parcelas.value, idPedido);
                }
                else if (usarControleDescontoFormaPagamentoDadosProduto)
                {
                    retDesconto = DescontoPedido.VerificaDescontoFormaPagtoDadosProduto(idPedido, tipoVenda != null ? tipoVenda.value : "", formaPagto != null ? formaPagto.value : "",
                        tipoCartao != null ? tipoCartao.value : "", parcelas != null ? parcelas.value : "");
                }

                if (retDesconto.error != null)
                {
                    alert(retDesconto.error.description);
                    return false;
                }
                else if (retDesconto != null && retDesconto != undefined && retDesconto.value != undefined && retDesconto.value != "")
                {
                    var txtDesconto = FindControl("txtDesconto","input");
                    var txtTipoDesconto = FindControl("drpTipoDesconto","select");

                    if (txtTipoDesconto != null)
                    {
                        txtTipoDesconto.value = 1;
                    }

                    if (txtDesconto != null)
                    {
                        txtDesconto.value = retDesconto.value.replace(".", ",");
                        txtDesconto.onchange();
                        txtDesconto.onblur();
                    }
                }
            }
        }
        
    </script>

</asp:Content>
