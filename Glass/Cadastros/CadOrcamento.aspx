<%@ Page Title="Cadastro de Orçamento" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadOrcamento.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadOrcamento" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc2" %>
<%@ Register src="../Controls/ctrlBenef.ascx" tagname="ctrlBenef" tagprefix="uc3" %>
<%@ Register src="../Controls/ctrlDescontoQtde.ascx" tagname="ctrlDescontoQtde" tagprefix="uc4" %>
<%@ Register src="../Controls/ctrlImagemPopup.ascx" tagname="ctrlImagemPopup" tagprefix="uc5" %>
<%@ Register Src="../Controls/ctrlParcelasSelecionar.ascx" TagName="ctrlParcelasSelecionar" TagPrefix="uc6" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc7" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc8" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc9" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style type="text/css">
        .cabecalho
        {
            font-weight: bold;
            padding-left: 4px;
        }
    </style>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/RecalcularOrcamento.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">

        var alterarLojaOrcamento = "<%= AlterarLojaOrcamento() %>";
        var inserting = false;
        var lnkGerarPedido = null;
        var hdfIdCliente;
        var tipoEntrega;
        var idCliente;
        var numProd = <%= GetNumeroProdutos() %>;
        var usarTabelaDescontoAcrescimoPedidoAVista = <%=(Glass.Configuracoes.PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista).ToString().ToLower() %>;

        function mensagemProdutoComDesconto(editar)
        {
            alert("Não é possível " + (editar ? "editar" : "remover") + " esse produto porque o orçamento possui desconto.\n" +
                "Aplique o desconto apenas ao terminar o cadastro dos produtos.\n" +
                "Para continuar, remova o desconto do orçamento.");
        }

        function mensagemProdutoComPedido()
        {
            alert("Não é possível editar esse produto porque o mesmo já foi negociado e adicionado em um pedido\n" +
                "Para editar esse produto, cancele o pedido associado.\n");
        }

        function getProduto()
        {
            openWindow(450, 700, '../Utils/SelProd.aspx');
        }

        // Calcula em tempo real o valor total do produto
        function calcTotalProd() {
            try {
                var valorIns = FindControl("txtValorIns_txtNumber", "input").value;

                if (valorIns == "")
                    return;

                var totM2 = 0; //FindControl("lblTotM2Ins", "span").innerHTML;
                var totM2Calc = 0; //new Number(FindControl("hdfTotM2Calc", "input").value.replace(',', '.')).toFixed(2);
                var total = new Number(valorIns.replace(',', '.')).toFixed(2);
                var qtde = new Number(FindControl("txtQtde", "input").value.replace(',', '.'));
                var altura = 0; //new Number(FindControl("txtAlturaIns", "input").value.replace(',', '.'));
                var largura = 0; //new Number(FindControl("txtLarguraIns", "input").value.replace(',', '.'));
                var tipoCalc = FindControl("hdfTipoCalc", "input").value;
                var m2Minimo = 0; //FindControl("hdfM2Minimo", "input").value;
                var alturaBenef = FindControl("drpAltBenef", "select");
                alturaBenef = alturaBenef != null ? alturaBenef.value : "0";
                var larguraBenef = FindControl("drpLargBenef", "select");
                larguraBenef = larguraBenef != null ? larguraBenef.value : "0";

                var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
                controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

            var percDesconto = controleDescQtde.PercDesconto();
            var percDescontoAtual = controleDescQtde.PercDescontoAtual();

            // Valida o valor unitário preenchido
            var valorMinimo = new Number(FindControl("hdfValMin", "input").value.replace(',', '.'));
            if (!FindControl("txtValorIns_txtNumber", "input").disabled && new Number(valorIns.replace(',', '.')) < valorMinimo) {
                alert("Valor especificado abaixo do valor mínimo (R$ " + valorMinimo.toFixed(2).replace(".", ",") + ")");
                FindControl("txtValorIns_txtNumber", "input").value = "";
                return false;
            }

            var retorno = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, FindControl("txtAlturaIns", "input"), largura, true, tipoCalc, alturaBenef, larguraBenef, percDescontoAtual, percDesconto);
            if (retorno != "")
                FindControl("lblTotalProd", "span").innerHTML = retorno;
            }
            catch (err) {

            }
        }

        function atualizaValMin()
        {
            var codInterno = FindControl("txtCodProdIns", "input");
            codInterno = codInterno != null ? codInterno.value : FindControl("lblCodProdIns", "span").innerHTML;

            var idOrcamento = <%= Request["idOrca"] != null ? Request["idOrca"] : "0" %>;
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            var cliRevenda = FindControl("hdfRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var altura = FindControl("hdfAltura", "input").value
            var idProdOrca = FindControl("hdfProdOrca", "input");
            idProdOrca = idProdOrca != null ? idProdOrca.value : "";

            var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

            var percDescontoQtde = controleDescQtde.PercDesconto();

            var retorno = CadOrcamento.GetValorMinimo(codInterno, tipoEntrega, idCliente, cliRevenda, idProdOrca, percDescontoQtde, idOrcamento, altura);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return;
            }
            else if(retorno == null){
                alert("Erro na recuperação do valor de tabela do produto.");
                return;
            }

            FindControl("hdfValMin", "input").value = retorno.value;
        }

        // Função chamada após selecionar produto pelo popup
        function setProduto(codInterno) {
            try {
                FindControl("txtCodProd", "input").value = codInterno;
                loadProduto(codInterno);
            }
            catch (err) {

            }
        }

        // Carrega dados do produto com base no código do produto passado
        function loadProduto(codInterno) {
            if (codInterno == "")
                return false;

            try {
                var idOrcamento = <%= Request["idOrca"] != null ? Request["idOrca"] : "0" %>;
                var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
                var cliRevenda = FindControl("hdfRevenda", "input").value;
                var idCliente = FindControl("hdfIdCliente", "input").value;
                var percComissao = getPercComissao();
                percComissao = percComissao == null ? 0 : percComissao.toString().replace('.', ',');

                var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
                controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

                var percDescontoQtde = controleDescQtde.PercDesconto();

                var retorno = CadOrcamento.GetProduto(codInterno, tipoEntrega, cliRevenda, idCliente,
                    percComissao, percDescontoQtde, FindControl("hdfIdLoja", "input").value, idOrcamento).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodProd", "input").value = "";
                    return false;
                }

                if (retorno[0] == "Prod") {
                    FindControl("hdfIdProd", "input").value = retorno[1];
                    FindControl("txtValorIns", "input").value = retorno[3]; // Exibe no cadastro o valor mínimo do produto
                    FindControl("hdfIsVidro", "input").value = retorno[4]; // Informa se o produto é vidro
                    FindControl("hdfM2Minimo", "input").value = retorno[5]; // Informa se o produto possui m² mínimo
                    FindControl("hdfTipoCalc", "input").value = retorno[7]; // Verifica como deve ser calculado o produto

                    atualizaValMin();

                    qtdEstoque = retorno[6]; // Pega a quantidade disponível em estoque deste produto
                    var tipoCalc = retorno[7];

                    // Se o produto não for vidro, desabilita os textboxes largura e altura,
                    // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
                    // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
                    /*
                    var cAltura = FindControl("txtAlturaIns", "input");
                    var cLargura = FindControl("txtLarguraIns", "input");
                    cAltura.disabled = maoDeObra || CalcProd_DesabilitarAltura(tipoCalc);
                    cLargura.disabled = maoDeObra || CalcProd_DesabilitarLargura(tipoCalc);
                    cAltura.value = !maoDeObra ? "" : (tipoCalc != 1 && tipoCalc != 5 ? FindControl("hdfAlturaAmbiente", "input").value : "");
                    cLargura.value = !maoDeObra ? "" : (tipoCalc != 1 && tipoCalc != 4 && tipoCalc != 5 && tipoCalc != 6 && tipoCalc != 7 && tipoCalc != 8 ?
                        FindControl("hdfLarguraAmbiente", "input").value : "");
                    */

                    // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                    if (retorno[4] == "true" && retorno[7] == "2" && FindControl("lnkBenef", "a") != null) {
                        FindControl("lnkBenef", "a").style.display = "inline";
                        FindControl("txtEspessura", "input").value = retorno[8];
                        FindControl("txtEspessura", "input").disabled = retorno[8] != "" && retorno[8] != "0";
                    }
                    else if (FindControl("lnkBenef", "a") != null)
                        FindControl("lnkBenef", "a").style.display = "none";

                    FindControl("hdfAliquotaIcmsProd", "input").value = retorno[9];

                    /*
                    //if (FindControl("hdfPedidoProducao", "input").value == "true")
                    {
                        FindControl("txtAltura", "input").value = retorno[10];
                        FindControl("txtLargura", "input").value = retorno[11];
                    }
                    */
                }

                FindControl("lblDescrProd", "span").innerHTML = codInterno.toString().toUpperCase() + " - " + retorno[2];
                calcTotalProd();
            }
            catch (err) {
                alert(err);
            }
        }

        function recalcular(idOrcamento, perguntar, tipoEntregaNovo, idClienteNovo)
        {
            var nomeControleBenef = "<%= ctrlBenef1.ClientID %>";
            var campoAltura = "<%= hdfBenefAltura.ClientID %>";
            var campoEspessura = "<%= hdfBenefEspessura.ClientID %>";
            var campoLargura = "<%= hdfBenefLargura.ClientID %>";
            var campoIdProd = "<%= hdfBenefIdProd.ClientID %>";
            var campoQtde = "<%= hdfBenefQtde.ClientID %>";
            var campoTotM = "<%= hdfBenefTotM.ClientID %>";
            var campoValorUnit = "<%= hdfBenefValorUnit.ClientID %>";

            recalcularOrcamento(idOrcamento, perguntar, "loading", nomeControleBenef, campoAltura, campoEspessura, campoLargura,
                campoIdProd, campoQtde, campoTotM, campoValorUnit, tipoEntregaNovo, idClienteNovo)
                .then(executado => {
                    if (executado && lnkGerarPedido == null)
                    {
                        alert("Orçamento recalculado com sucesso!");
                        redirectUrl(window.location.href);
                    }
                });
        }

        function limparComissionado()
        {
            FindControl("hdfIdComissionado", "input").value = "";
            FindControl("lblComissionado", "span").innerHTML = "";
            FindControl("txtPercentual", "input").value = "0";
            FindControl("txtValorComissao", "input").value = "R$ 0,00";
        }

        function geraPedido(link, hiddenIdCliente, perguntar)
        {
            if (perguntar && !confirm("Tem certeza que deseja gerar um pedido para este orçamento?"))
                return false;

            lnkGerarPedido = link;
            hdfIdCliente = document.getElementById(hiddenIdCliente);

            if (hdfIdCliente.value == "")
            {
                alert("Você deve selecionar o cliente antes de continuar.");
                openWindow(500, 750, "../Utils/SelCliente.aspx");
                return false;
            }

            return true;
        }

        function setCliente(idCli, nome)
        {
            hdfIdCliente.value = idCli;
            recalcular(<%= Request["idOrca"] != null ? Request["idOrca"] : "0" %>, false, "", idCli);

            eval(lnkGerarPedido.href.replace(/%20/g, " "));
        }

        var botaoAtualizarClicado = false;

        function onSaveOrca() {

            if (botaoAtualizarClicado)
                return false;

            botaoAtualizarClicado = true;

            var txtPercentual = FindControl("txtPercentual", "input");
            var hdfIdComissionado = FindControl("hdfIdComissionado", "input");

            if(usarTabelaDescontoAcrescimoPedidoAVista && FindControl("drpTipoVenda", "select").value == "")
            {
                alert("Selecione o tipo de Venda.");
                botaoAtualizarClicado = false;
                return false;
            }
            // Se o percentual de comissão a ser cobrado for > 0, verifica se o comissionado foi informado
            if (txtPercentual != null && hdfIdComissionado != null && txtPercentual.value != "" &&
                parseFloat(txtPercentual.value.replace(',', '.')) > 0 && hdfIdComissionado.value == "") {
                alert("Informe o comissionado.");
                botaoAtualizarClicado = false;
                return false;
            }

            if (<%= Glass.Configuracoes.OrcamentoConfig.TelaCadastro.PermitirInserirSemTipoOrcamento.ToString().ToLower() %> == false &&
                FindControl("drpTipoOrcamento", "select") != null && FindControl("drpTipoOrcamento", "select").value == "")
            {
                alert("Selecione o tipo do orçamento.");
                botaoAtualizarClicado = false;
                return false;
            }

            FindControl("drpFuncionario", "select").disabled = false;

            if (FindControl("drpLoja", "select"))
            {
                FindControl("drpLoja", "select").disabled = false;
            }

            return true;
        }

        function onInsert()
        {
            if (inserting)
                return false;

            if (FindControl("txtNomeCliente", "input").value == "")
            {
                alert("Digite o nome do cliente ou escolha-o na lista para continuar.");
                return false;
            }

            if (FindControl("ddlTipoEntrega", "select").value == "")
            {
                alert("Selecione o tipo de entrega.");
                return false;
            }

            if(usarTabelaDescontoAcrescimoPedidoAVista && FindControl("DropDownList1", "select").value == "")
            {
                alert("Selecione o tipo de Venda.");
                return false;
            }

            if (!onSaveOrca())
                return false;

            document.getElementById("load").style.display = "";

            FindControl("drpFuncionario", "select").disabled = false;

            inserting = true;

            if (FindControl("drpLoja", "select"))
            {
                FindControl("drpLoja", "select").disabled = false;
            }
            return true;
        }

        function onUpdateProduto()
        {
            var txtAmbiente = FindControl("txtAmbienteProd", "input");

            if (txtAmbiente != null)
            {
                if (txtAmbiente.parentNode.style.display != "none")
                {
                    if (txtAmbiente.value == "")
                    {
                        alert("Informe o ambiente.");
                        return false;
                }
            }
        }

        // Atualiza o valor mínimo do produto
        atualizaValMin();

        // Valida o valor unitário preenchido
        var valorIns = FindControl("txtValorIns_txtNumber", "input").value;
        var valorMinimo = new Number(FindControl("hdfValMin", "input").value.replace(',', '.'));
        if (!FindControl("txtValorIns_txtNumber", "input").disabled && new Number(valorIns.replace(',', '.')) < valorMinimo) {
            alert("Valor especificado abaixo do valor mínimo (R$ " + valorMinimo.toFixed(2).replace(".", ",") + ")");
            FindControl("txtValorIns_txtNumber", "input").value = "";
            return false;
        }

        return true;
    }

        function addAmbiente(add)
        {
            FindControl("imbNovo", "input").style.display = add ? "none" : "";
            FindControl("txtAmbienteIns", "input").style.display = add ? "" : "none";
            FindControl("txtDescricaoIns", "textarea").style.display = add ? "" : "none";
            FindControl("imbInserir", "input").style.display = add ? "" : "none";
        }

        function calcularDesconto(tipoCalculo)
        {
            var controle = FindControl("txtDesconto", "input");
            if (controle.value == "0")
                return;

            var tipo = FindControl("drpTipoDesconto", "select").value;
            var desconto = parseFloat(controle.value.replace(',', '.'));
            if (isNaN(desconto))
                desconto = 0;

            var tipoAtual = FindControl("hdfTipoDesconto", "input").value;
            var descontoAtual = parseFloat(FindControl("hdfDesconto", "input").value.replace(',', '.'));
            if (isNaN(descontoAtual))
                descontoAtual = 0;

            var idOrcamento = <%= !String.IsNullOrEmpty(Request["idOrca"]) ? Request["idOrca"] : "0" %>;
            var idFuncAtual = <%= Glass.Data.Helper.UserInfo.GetUserInfo.CodUser %>;
            var alterou = tipo != tipoAtual || desconto != descontoAtual;
            var descontoMaximo = CadOrcamento.PercDesconto(idOrcamento, idFuncAtual, alterou).value;

            var total = parseFloat(FindControl("hdfTotalSemDesconto", "input").value.replace(/\./g, "").replace(',', '.'));
            var totalProduto = tipoCalculo == 2 ? parseFloat(FindControl("lblTotalProd", "span").innerHTML.replace("R$", "").replace(" ", "").replace(/\./g, "").replace(',', '.')) : 0;
            var valorDescontoMaximo = total * (descontoMaximo / 100);

            var valorDescontoProdutos = <%= GetDescontoProdutos() %> - (tipoCalculo == 2 ? parseFloat(FindControl("hdfValorDescontoAtual", "input").value.replace(',', '.')) : 0);
            var valorDescontoOrcamento = tipoCalculo == 2 ? <%= GetDescontoOrcamento() %> : 0;
            var descontoProdutos = parseFloat(((valorDescontoProdutos / (total > 0 ? total : 1)) * 100).toFixed(2));
            var descontoOrcamento = parseFloat(((valorDescontoOrcamento / (total > 0 ? total : 1)) * 100).toFixed(2));

            var descontoSomar = descontoProdutos + (tipoCalculo == 2 ? descontoOrcamento : 0);
            var valorDescontoSomar = valorDescontoProdutos + (tipoCalculo == 2 ? valorDescontoOrcamento : 0);

            if (tipo == 2)
                desconto = (desconto / total) * 100;

            if (parseFloat((desconto + descontoSomar).toFixed(2)) > parseFloat(descontoMaximo))
            {
                var mensagem = "O desconto máximo permitido é de " + (tipo == 1 ? descontoMaximo + "%" : "R$ " + valorDescontoMaximo.toFixed(2).replace('.', ',')) + ".";
                if (descontoProdutos > 0)
                    mensagem += "\nO desconto já aplicado aos produtos é de " + (tipo == 1 ? descontoProdutos + "%" : "R$ " + valorDescontoProdutos.toFixed(2).replace('.', ',')) + ".";

                if (descontoOrcamento > 0)
                    mensagem += "\nO desconto já aplicado ao orçamento é de " + (tipo == 1 ? descontoOrcamento + "%" : "R$ " + valorDescontoOrcamento.toFixed(2).replace('.', ',')) + ".";

                alert(mensagem);
                controle.value = tipo == 1 ? descontoMaximo - descontoSomar : (valorDescontoMaximo - valorDescontoSomar).toFixed(2).replace('.', ',') ;

                if (parseFloat(controle.value.replace(',', '.')) < 0)
                    controle.value = "0";

                return false;
            }

            return true;
        }

        function setComissionado(id, nome, percentual) {
            FindControl("lblComissionado", "span").innerHTML = nome;
            FindControl("hdfIdComissionado", "input").value = id;
            FindControl("txtPercentual", "input").value = percentual;
        }

        function iniciaPesquisaCep(cep)
        {
            var logradouro = FindControl("txtEndereco", "input");
            var bairro = FindControl("txtBairro", "input");
            var cidade = FindControl("txtCidade", "input");
            pesquisarCep(cep, null, logradouro, bairro, cidade, null);
        }

        function iniciaPesquisaCepObra(cep)
        {
            var logradouro = FindControl("txtEnderecoObra", "input");
            var bairro = FindControl("txtBairroObra", "input");
            var cidade = FindControl("txtCidadeObra", "input");
            pesquisarCep(cep, null, logradouro, bairro, cidade, null);
        }

        function openRpt(idOrca)
        {
            openWindow(600, 800, "../Relatorios/RelOrcamento.aspx?idOrca=" + idOrca);
            return false;
        }

        function openRptMemoria(idOrca)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=MemoriaCalculoOrcamento&idOrca=" + idOrca);
            return false;
        }

        function getCli(idCli)
        {
            var usarComissionado = <%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente.ToString().ToLower() %>;

            var dados = CadOrcamento.GetCli(idCli.value).value;
            if (dados == null || dados == "" || dados.split('|')[0] == "Erro")
            {
                idCli.value == "";
                FindControl("txtNomeCliente", "input").value = "";
                FindControl("hdfIdCliente", "input").value = "";
                FindControl("txtIdCliente", "input").value = "";

                if (usarComissionado)
                    limparComissionado();

                if (dados.split('|')[0] == "Erro")
                    alert(dados.split('|')[1]);

                return;
            }

            dados = dados.split("|");
            setDadosCliente(dados[0], dados[1], dados[2], dados[3], dados[4], dados[5], dados[6], dados[7], idCli.value, dados[8], dados[9], dados[12]);

            if (usarComissionado)
            {
                var comissionado = MetodosAjax.GetComissionado("", idCli.value).value.split(';');
                setComissionado(comissionado[0], comissionado[1], comissionado[2]);
            }
        }

        function setDadosCliente(nome, telRes, telCel, email, endereco, bairro, cidade, cep, idCliente, compl, idFunc, obs) {
            FindControl("txtNomeCliente", "input").value = nome;
            FindControl("txtTelRes", "input").value = telRes;
            FindControl("txtTelCel", "input").value = telCel;
            FindControl("txtEmail", "input").value = email;
            FindControl("txtEndereco", "input").value = endereco + (compl != "" && compl != null ? " (" + compl + ")" : "");
            FindControl("txtBairro", "input").value = bairro;
            FindControl("txtCidade", "input").value = cidade;
            FindControl("txtCep", "input").value = cep;
            FindControl("lblObsCliente", "span").innerHTML = obs;

            FindControl("txtIdCliente", "input").value = idCliente;
            FindControl("hdfIdCliente", "input").value = idCliente;

            var drpFuncionario = FindControl("drpFuncionario", "select");
            if (drpFuncionario != null && idFunc != "0")
                drpFuncionario.value = dados[9];
        }

        function openProdutos(idProd, editar)
        {
            var tipoEntrega = FindControl("ddlTipoEntrega", "select");
            if (tipoEntrega != null)
                tipoEntrega = tipoEntrega.value;
            else
                tipoEntrega = FindControl("hdfTipoEntrega", "input").value;

            if (tipoEntrega == "")
            {
                alert("Selecione o tipo de entrega antes de inserir um produto.");
                return false;
            }

            var ambiente = FindControl("hdfIdAmbienteOrca", "input") != null ? FindControl("hdfIdAmbienteOrca", "input").value : "";
            if (ambiente != "") ambiente = "&idAmbiente=" + ambiente;

            var idCliente = FindControl("hdfIdCliente", "input").value;

            openWindow(screen.height, screen.width, 'CadProdutoOrcamento.aspx?IdOrca=<%= Request["IdOrca"] %>&IdProd=' + idProd +
                "&TipoEntrega=" + tipoEntrega + ambiente + (editar ? "&editar=true" : "") +
                "&idCliente=" + idCliente + "&orcamentoRapido=false");

            return false;
        }

        function openProjeto(idProd)
        {
            var tipoEntrega = FindControl("ddlTipoEntrega", "select");
            if (tipoEntrega != null)
                tipoEntrega = tipoEntrega.value;
            else
                tipoEntrega = FindControl("hdfTipoEntrega", "input").value;

            if (tipoEntrega == "")
            {
                alert("Selecione o tipo de entrega antes de inserir um projeto.");
                return false;
            }

            var idCliente = FindControl("hdfIdCliente", "input").value;

            var ambiente = FindControl("hdfIdAmbienteOrca", "input") != null ? FindControl("hdfIdAmbienteOrca", "input").value : "";
            if (ambiente != "")
                ambiente = "&idAmbienteOrca=" + ambiente;

            openWindow(screen.height, screen.width, '../Cadastros/Projeto/CadProjetoAvulso.aspx?IdOrcamento=<%= Request["IdOrca"] %>' +
                "&IdProdOrca=" + idProd + "&idCliente=" + idCliente + "&TipoEntrega=" + tipoEntrega + ambiente);

            return false;
        }

        function retornaPagina() {
            window.history.go(-1);
        }

        function refreshPage() {
            atualizarPagina();
        }

        function chamarRecalcular()
        {
            var idCliente = FindControl("hdfIdCliente", "input").value;

            recalcular(<%= Request["idOrca"] != null ? Request["idOrca"] : "0" %>, false, "", idCliente);
        }

        function callbackSetParcelas()
        {
            setParcelas(true);
            if (typeof <%= dtvOrcamento.ClientID %>_ctrlParcelas1 != "undefined")
            Parc_visibilidadeParcelas("<%= dtvOrcamento.ClientID %>_ctrlParcelas1");

                // Verifica se a empresa permite desconto para pedidos à vista com uma parcela
        if (<%= (Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoUmaParcela && Glass.Configuracoes.PedidoConfig.Desconto.DescontoPedidoApenasAVista).ToString().ToLower() %>)
                    showHideDesconto(FindControl("hdfNumParcelas", "input").value == "1" || FindControl("drpTipoVenda", "select").value == "1");
            }

    function setParcelas(calcParcelas)
    {
        var nomeControleParcelas = "<%= dtvOrcamento.ClientID %>_ctrlParcelas1";
        if (document.getElementById(nomeControleParcelas + "_tblParcelas") == null)
            return;

        var drpTipoVenda = FindControl("drpTipoVenda", "select");

        if (drpTipoVenda == null)
            return;

        if (FindControl("hdfExibirParcela", "input") != null)
            FindControl("hdfExibirParcela", "input").value = drpTipoVenda.value == 2;

        FindControl("hdfCalcularParcela", "input").value = (calcParcelas == false ? false : true).toString();
    }

        // Evento acionado ao trocar o tipo de venda (à vista/à prazo)
    function tipoVendaChange(control, calcParcelas) {
        if (control == null)
            return;

        if (document.getElementById("divNumParc") != null)
            document.getElementById("divNumParc").style.display = parseInt(control.value) == 2 ? "" : "none";

        setParcelas(calcParcelas);
        if (typeof <%= dtvOrcamento.ClientID %>_ctrlParcelas1 != "undefined")
            Parc_visibilidadeParcelas("<%= dtvOrcamento.ClientID %>_ctrlParcelas1");

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

        if(lblDescontoVista != null)
        {
          lblDescontoVista.style.display = !exibirDesconto ? "" : "none";
        }

        txtDesconto.onchange();
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvOrcamento" runat="server" AutoGenerateRows="False" DataSourceID="odsOrcamento"
                    DefaultMode="Insert" GridLines="None" DataKeyNames="IdOrcamento">
                    <Fields>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <table cellpadding="2" cellspacing="2">
                                    <tr>
                                        <td align="left" class="cabecalho">
                                            Orçamento
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label7" runat="server" Text='<%# Eval("IdOrcamento") %>' Font-Size="Medium"></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Cliente
                                        </td>
                                        <td align="left" nowrap="nowrap" colspan="3">
                                            <asp:Label ID="Label10" runat="server" Text='<%# Eval("NomeClienteLista") %>'></asp:Label>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td align="left" class="cabecalho">
                                            <asp:Label ID="lblProjetoEdit" runat="server" onload="ctrlProjeto_Load"
                                                Text="Projeto"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label8" runat="server" Text='<%# Eval("IdProjeto") %>'
                                                Font-Size="Medium" onload="ctrlProjeto_Load"></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Data
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label27" runat="server" Text='<%# Eval("DataCad") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Situação
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label11" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <%--tr>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Contato
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label13" runat="server" Text='<%# Eval("Contato") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Tel Res.
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label14" runat="server" Text='<%# Eval("TelCliente") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Celular
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label15" runat="server" Text='<%# Eval("CelCliente") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Email
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label16" runat="server" Text='<%# Eval("Email") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Bairro
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label17" runat="server" Text='<%# Eval("Bairro") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            CEP
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label21" runat="server" Text='<%# Eval("Cep") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Endereço
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label18" runat="server" Text='<%# Eval("Endereco") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Cidade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label19" runat="server" Text='<%# Eval("Cidade") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            A/C
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label12" runat="server" Text='<%# Eval("AosCuidados") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Prazo Entrega
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label22" runat="server" Text='<%# Eval("PrazoEntrega") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Validade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label23" runat="server" Text='<%# Eval("Validade") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Forma pagto.
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label24" runat="server" Text='<%# Eval("FormaPagto") %>'></asp:Label>
                                        </td>
                                    </tr--%>
                                    <tr>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            <asp:Label ID="lblProjetoEdit0" runat="server" onload="ctrlMedicao_Load"
                                                Text="Medição"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label26" runat="server" Text='<%# Eval("IdsMedicao") %>'
                                                Font-Size="Medium" onload="ctrlMedicao_Load"></asp:Label>
                                            <asp:Label ID="Label13" runat="server" Font-Size="Medium"
                                                Text='<%# Eval("IdMedicaoDefinitiva") != null ? "Definitiva: " + Eval("IdMedicaoDefinitiva") : "" %>'
                                                onload="ctrlMedicao_Load"></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Tipo Entrega
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label30" runat="server" Text='<%# Eval("DescrTipoEntrega") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Vendedor
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label25" runat="server" Text='<%# Eval("NomeFuncionario") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Tipo
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label14" runat="server" Text='<%# Eval("DescrTipoOrcamento") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            Desconto
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label28" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho">
                                            Acréscimo
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label29" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                                        </td>
                                        <td class="cabecalho" nowrap="nowrap" style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "display: none": "" %>">
                                            Valor ICMS
                                        </td>
                                        <td align="left" nowrap="nowrap" style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "display: none": "" %>">
                                            <asp:Label ID="Label9" runat="server" Text='<%# Eval("ValorIcms", "{0:C}") %>' ForeColor="Red"></asp:Label>
                                        </td>
                                        <td class="cabecalho" nowrap="nowrap" style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIpiPedido ? "display: none": "" %>">
                                            Valor IPI
                                        </td>
                                        <td align="left" nowrap="nowrap" style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIpiPedido ? "display: none": "" %>">
                                            <asp:Label ID="Label12" runat="server" Text='<%# Eval("ValorIpi", "{0:C}") %>' ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            <asp:Label ID="lblValorFrete" runat="server" OnLoad="txtValorFrete_Load" Text="Valor do Frete"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label17" runat="server" Text='<%# Eval("ValorEntrega", "{0:C}") %>' OnLoad="txtValorFrete_Load"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="8" align="center" style="padding-left: 4px; white-space: nowrap">
                                            <table>
                                                <tr>
                                                    <td class="cabecalho">
                                                        <asp:Label ID="lblTitleTotal" runat="server" onload="lblTotalGeral_Load"
                                                            Text="Total"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblTotal" runat="server" onload="lblTotalGeral_Load"
                                                            Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td class="cabecalho" nowrap="nowrap">
                                                        <asp:Label ID="lblTitleTotalBruto" runat="server"
                                                            onload="lblTotalBrutoLiquido_Load" Text="Total Bruto"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblTotalBruto" runat="server" onload="lblTotalBrutoLiquido_Load"
                                                            Text='<%# Eval("TotalBruto", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td class="cabecalho" nowrap="nowrap">
                                                        <asp:Label ID="lblTitleTotalLiquido" runat="server"
                                                            onload="lblTotalBrutoLiquido_Load" Text="Total Líquido"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblTotalLiquido" runat="server"
                                                            onload="lblTotalBrutoLiquido_Load" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="8" style="padding-left: 4px; white-space: nowrap">
                                            <asp:Label ID="Label38" runat="server" ForeColor="Green"
                                                Style="white-space: nowrap" Text='<%# Eval("DescrMostrarTotal") %>'></asp:Label>
                                            <asp:Label ID="Label39" runat="server" ForeColor="Green"
                                                Style="white-space: nowrap"
                                                Text='<%# (Eval("DescrMostrarTotal").ToString().Length > 0 && Eval("DescrMostrarTotalProd").ToString().Length > 0 ? "&nbsp;&nbsp;" : "") + Eval("DescrMostrarTotalProd").ToString() %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">Obs. do Cliente
                                        </td>
                                        <td align="left" nowrap="nowrap" colspan="5">
                                            <asp:Label ID="lblObsCliente" runat="server" OnLoad="lblObsCliente_Load" Text='<%# Eval("ObsCliente") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                <asp:HiddenField ID="hdfIdCliente" runat="server"
                                    Value='<%# Eval("IdCliente") %>' />
                                <asp:HiddenField ID="hdfPercComissao" runat="server"
                                    Value='<%# Eval("PercComissao") %>' />
                                <asp:HiddenField ID="hdfRevenda" runat="server" onload="hdfRevenda_Load" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:HiddenField ID="hdfRevenda" runat="server" onload="hdfRevenda_Load" />
                                <table cellpadding="2" cellspacing="0">
                                    <tr>
                                        <td align="left" class="dtvHeader">
                                            Orçamento
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label7" runat="server" Font-Size="Medium"
                                                Text='<%# Eval("IdOrcamento") %>'></asp:Label>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblProjetoEdit" runat="server" onload="ctrlProjeto_Load"
                                                Text="Projeto"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label8" runat="server" Font-Size="Medium"
                                                            onload="ctrlProjeto_Load" Text='<%# Eval("IdProjeto") %>'></asp:Label>
                                                        &nbsp;</td>
                                                    <td class="dtvHeader">
                                                        <asp:Label ID="lblMedicaoEdit" runat="server" onload="ctrlMedicao_Load"
                                                            Text="Medição"></asp:Label>
                                                        &nbsp;&nbsp;</td>
                                                    <td>
                                                        <asp:Label ID="Label26" runat="server" Text='<%# Eval("IdsMedicao") %>'
                                                            Font-Size="Medium" onload="ctrlMedicao_Load"></asp:Label>
                                                        <asp:Label ID="Label13" runat="server"
                                                            Text='<%# Eval("IdMedicaoDefinitiva") != null ? "Def.: " + Eval("IdMedicaoDefinitiva") : "" %>'
                                                            onload="ctrlMedicao_Load"></asp:Label>
                                                        <asp:HiddenField ID="hdfIdMedicaoDef" runat="server"
                                                            Value='<%# Eval("IdMedicaoDefinitiva") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label14" runat="server" Text="Loja"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <uc8:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="false" MostrarTodas="false"
                                                            SelectedValue='<%# Bind("IdLoja") %>' OnLoad="Loja_Load" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">&nbsp;
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Cliente
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <span style="white-space: nowrap">
                                            <asp:TextBox ID="txtIdCliente" runat="server" Text='<%# Eval("IdCliente") %>' onkeypress="return soNumeros(event, true, true)"
                                                onblur="getCli(this);" Width="50px"></asp:TextBox>
                                            <asp:TextBox ID="txtNomeCliente" runat="server" Text='<%# Bind("NomeCliente") %>'
                                                Width="280px" MaxLength="50"></asp:TextBox>
                                            <asp:ImageButton ID="imgGetCliente" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="openWindow(500, 700, '../Utils/SelCliente.aspx?dadosCliente=1&tipo=orcamento'); return false;" />
                                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                                            </span>
                                            <br />
                                            <asp:Label ID="lblObsCliente" runat="server" ForeColor="<%# GetCorObsCliente() %>"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Situação
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpSituacao" runat="server"
                                                            SelectedValue='<%# Bind("Situacao") %>' DataSourceID="odsSituacao"
                                                            DataTextField="Descr" DataValueField="Id">
                                                        </asp:DropDownList>
                                                        &nbsp;
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap" style="padding: 2px">
                                                        Tipo
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoOrcamento" runat="server"
                                                            SelectedValue='<%# Bind("TipoOrcamento") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem Value="1">Venda</asp:ListItem>
                                                            <asp:ListItem Value="2">Revenda</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            A/C
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtAC" runat="server" MaxLength="50" Text='<%# Bind("AosCuidados") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Contato
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox5" runat="server" MaxLength="20" Text='<%# Bind("Contato") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Tel Res.
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTelRes" runat="server" MaxLength="15" onkeypress="return soTelefone(event)"
                                                onkeydown="return maskTelefone(event, this);" Text='<%# Bind("TelCliente") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Celular
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTelCel" runat="server" MaxLength="20" onkeypress="return soTelefone(event)"
                                                onkeydown="return maskTelefone(event, this);" Text='<%# Bind("CelCliente") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Email
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="60" Text='<%# Bind("Email") %>'
                                                Width="250px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Bairro
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="50" Text='<%# Bind("Bairro") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Endereço
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtEndereco" runat="server" MaxLength="70" Text='<%# Bind("Endereco") %>'
                                                Width="250px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Cidade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Text='<%# Bind("Cidade") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Vendedor
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                                DataTextField="Nome" DataValueField="IdFunc" SelectedValue='<%# Bind("IdFuncionario") %>'
                                                AppendDataBoundItems="True" ondatabound="drpFuncionario_DataBound">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            CEP
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCep" runat="server" MaxLength="9" Text='<%# Bind("Cep") %>' onkeypress="return soCep(event)"
                                                onkeyup="return maskCep(event, this);"></asp:TextBox>
                                            <asp:ImageButton ID="imgPesquisarCep" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Prazo Entrega
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox8" runat="server" MaxLength="300" Text='<%# Bind("PrazoEntrega") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Validade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox7" runat="server" MaxLength="30" Text='<%# Bind("Validade") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Forma pagto.
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtFormaPagto" runat="server" MaxLength="200" Text='<%# Bind("FormaPagto") %>'
                                                Width="300px" TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Obra
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox6" runat="server" MaxLength="30" Text='<%# Bind("Obra") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="tdTipoVenda1" align="left" class="dtvHeader" nowrap="nowrap">Tipo Venda
                                        </td>
                                        <td id="tdTipoVenda2" align="left" nowrap="nowrap" valign="middle">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoVenda" runat="server" SelectedValue='<%# Bind("TipoVenda") %>'
                                                            onchange="tipoVendaChange(this, true);"
                                                            DataSourceID="odsTipoVenda" DataTextField="Descr" DataValueField="Id">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <div id="divNumParc">
                                                            <table>
                                                                <tr>
                                                                    <td nowrap="nowrap" style="font-weight: bold">Num Parc.:
                                                                    </td>
                                                                    <td nowrap="nowrap">
                                                                        <uc6:ctrlParcelasSelecionar ID="ctrlParcelasSelecionar1" runat="server" ParcelaPadrao='<%# Bind("IdParcela") %>'
                                                                            NumeroParcelas='<%# Bind("NumParc") %>' OnLoad="ctrlParcelasSelecionar1_Load"
                                                                            CallbackSelecaoParcelas="callbackSetParcelas" />
                                                                        <asp:HiddenField ID="hdfDataBase" runat="server" OnLoad="hdfDataBase_Load" />
                                                                        <asp:HiddenField ID="hdfExibirParcela" runat="server" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTipoVendaAtual" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Tipo Entrega</td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:DropDownList ID="ddlTipoEntrega" runat="server"
                                                AppendDataBoundItems="True" DataSourceID="odsTipoEntrega" DataTextField="Descr"
                                                DataValueField="Id" SelectedValue='<%# Bind("TipoEntrega") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoEntrega" runat="server"
                                                SelectMethod="GetTipoEntrega" TypeName="Glass.Data.Helper.DataSources">
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Data
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc7:ctrlData ID="ctrlDataOrca" runat="server" ReadOnly="ReadWrite"
                                                DataString='<%# Bind("DataCad") %>' ExibirHoras="False" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Data Entrega
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc7:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadWrite"
                                                DataString='<%# Bind("DataEntrega") %>' ExibirHoras="False" />
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Total
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTotal" runat="server" ReadOnly="True" Text='<%# Eval("Total", "{0:C}") %>'></asp:TextBox>
                                            <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            &nbsp;Desconto
                                        </td>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoDesconto" runat="server" onchange="calcularDesconto(1)"
                                                            SelectedValue='<%# Bind("TipoDesconto") %>'>
                                                            <asp:ListItem Value="2">R$</asp:ListItem>
                                                            <asp:ListItem Value="1">%</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="txtDesconto" runat="server"
                                                            onchange="calcularDesconto(1)"
                                                            onkeypress="return soNumeros(event, false, true);"
                                                            Text='<%# Bind("Desconto") %>' Width="70px"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfTipoDesconto" runat="server"
                                                            Value='<%# Eval("TipoDesconto") %>' />
                                                        <asp:HiddenField ID="hdfDesconto" runat="server"
                                                            Value='<%# Eval("Desconto") %>' />
                                                        <script type="text/javascript">
                                                            calcularDesconto(1);
                                                        </script>
                                                        &nbsp;&nbsp;
                                                    </td>
                                                    <td class="dtvHeader">
                                                        &nbsp;Acréscimo&nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoAcrescimo" runat="server"
                                                            SelectedValue='<%# Bind("TipoAcrescimo") %>'>
                                                            <asp:ListItem Value="2">R$</asp:ListItem>
                                                            <asp:ListItem Value="1">%</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="txtAcrescimo" runat="server"
                                                            onkeypress="return soNumeros(event, false, true);"
                                                            Text='<%# Bind("Acrescimo") %>' Width="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="dtvHeader" nowrap="nowrap" style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "display: none": "" %>">
                                            Valor ICMS
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label9" runat="server" Text='<%# Eval("ValorIcms", "{0:C}") %>' Visible="<%# Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido %>"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="lblValorFrete" runat="server" OnLoad="txtValorFrete_Load" Text="Valor do Frete"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtValorFrete" runat="server" onkeypress="return soNumeros(event, false, true);" OnLoad="txtValorFrete_Load" Text='<%# Bind("ValorEntrega") %>' Width="80px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4" align="center" style="padding: 4px">
                                            <asp:CheckBox ID="chkMostrarTotal" runat="server" Checked='<%# Bind("MostrarTotal") %>'
                                                Text="Mostrar total na impressão" />
                                            &nbsp;&nbsp;
                                            <asp:CheckBox ID="chkMostrarTotalProd" runat="server" Checked='<%# Bind("MostrarTotalProd") %>'
                                                Text="Mostrar total por produto" />
                                            &nbsp;&nbsp;
                                            <asp:CheckBox ID="chkImprimirProdutos" runat="server"
                                                Checked='<%# Bind("ImprimirProdutosOrcamento") %>'
                                                Text="Exibir produtos no lugar dos itens no relatório" />
                                        </td>
                                    </tr>
                                </table>
                                <table style="width: 100%">
                                    <tr>
                                        <td class="dtvHeader">
                                            Local da Obra
                                        </td>
                                        <td>
                                            Endereço
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEnderecoObra" runat="server" MaxLength="70" Text='<%# Bind("EnderecoObra") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                        <td>
                                            Bairro
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtBairroObra" runat="server" MaxLength="50" Text='<%# Bind("BairroObra") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                        <td>
                                            Cidade
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCidadeObra" runat="server" MaxLength="50" Text='<%# Bind("CidadeObra") %>'
                                                Width="120px"></asp:TextBox>
                                        </td>
                                        <td>
                                            Cep
                                        </td>
                                        <td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCepObra" runat="server" MaxLength="9" Text='<%# Bind("CepObra") %>' onkeypress="return soCep(event)"
                                                onkeyup="return maskCep(event, this);"></asp:TextBox>
                                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="iniciaPesquisaCepObra(FindControl('txtCepObra', 'input').value); return false" />
                                        </td>
                                        </td>
                                    </tr>
                                </table>
                                <table id="tbComissionado" style="width: 100%" class="dtvHeader" cellpadding="3"
                                    cellspacing="0">
                                    <tr>
                                        <td align="left" style="padding-left: 0px">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        &nbsp;Comissionado:
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:Label ID="lblComissionado" runat="server" Text='<%# Eval("NomeComissionado") %>'></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:LinkButton ID="lnkSelComissionado" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelComissionado.aspx'); return false;"
                                                            Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>">
                                                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                        <asp:ImageButton ID="imbLimparComissionado" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                            OnClientClick="limparComissionado(); return false;" ToolTip="Limpar comissionado"
                                                            Visible='<%# Eval("ExibirLimparComissionado") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                <tr>
                                                    <td>
                                                        Percentual:
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                            Text='<%# Bind("PercComissao") %>' Width="50px" Enabled='<%# Glass.Configuracoes.PedidoConfig.Comissao.AlterarPercComissionado %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        Valor Comissão:
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:TextBox ID="txtValorComissao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                            ReadOnly="True" Text='<%# Eval("ValorComissao", "{0:C}") %>' Width="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                <table style="width: 100%" cellpadding="3" cellspacing="0">
                                    <tr>
                                        <td class="dtvHeader" colspan="2">
                                            Observação
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:TextBox ID="TextBox3" runat="server" MaxLength="1000" Text='<%# Bind("Obs") %>'
                                                TextMode="MultiLine" Width="600px"></asp:TextBox>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader" colspan="2">
                                            Observação Interna
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:TextBox ID="TextBox9" runat="server" MaxLength="1000" Text='<%# Bind("ObsInterna") %>'
                                                TextMode="MultiLine" Width="600px"></asp:TextBox>
                                            <asp:HiddenField ID="hdfIdComissionado" runat="server" Value='<%# Bind("IdComissionado") %>' />
                                            <asp:HiddenField ID="hdfDataSituacao" runat="server" Value='<%# Bind("DataSituacao") %>' />
                                            <asp:HiddenField ID="hdfIdProjeto" runat="server" Value='<%# Bind("IdProjeto") %>' />
                                            <asp:HiddenField ID="hdfIdPedidoGerado" runat="server" Value='<%# Bind("IdPedidoGerado") %>' />
                                            <asp:HiddenField ID="hdfEditVisible" runat="server" Value='<%# Eval("EditVisible") %>' />
                                            <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                            <asp:HiddenField ID="hdfValorComissao" runat="server" Value='<%# Bind("ValorComissao") %>' />
                                            <asp:HiddenField ID="hdfDataRecalcular" runat="server" Value='<%# Bind("DataRecalcular") %>' />
                                            <asp:HiddenField ID="hdfPercComissao" runat="server" Value='<%# Eval("PercComissao") %>' />
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <table cellpadding="2" cellspacing="0">
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Cliente
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <span style="white-space: nowrap">
                                                <asp:TextBox ID="txtIdCliente" runat="server" Text='<%# Eval("IdCliente") %>' onkeypress="return soNumeros(event, true, true)"
                                                    onblur="getCli(this)" Width="50px"></asp:TextBox>
                                                <asp:TextBox ID="txtNomeCliente" runat="server" MaxLength="50" Text='<%# Bind("NomeCliente") %>'
                                                    Width="280px"></asp:TextBox>
                                                &nbsp;<asp:ImageButton ID="imgGetCliente" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                    OnClientClick="openWindow(500, 700, '../Utils/SelCliente.aspx?dadosCliente=1&tipo=orcamento'); return false;" />
                                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                                            </span>
                                            <br />
                                            <asp:Label ID="lblObsCliente" runat="server" ForeColor="<%# GetCorObsCliente() %>"></asp:Label>
                                        </td>

                                        <td align="left" class="dtvHeader" nowrap="nowrap">Situação
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpSituacaoOrcamento" runat="server"
                                                            SelectedValue='<%# Bind("Situacao") %>'>
                                                            <asp:ListItem Value="1">Em Aberto</asp:ListItem>
                                                            <asp:ListItem Value="2">Negociado</asp:ListItem>
                                                            <asp:ListItem Value="3">Não Negociado</asp:ListItem>
                                                        </asp:DropDownList>
                                                        &nbsp;
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap" style="padding: 2px">
                                                        Tipo
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoOrcamento" runat="server"
                                                            SelectedValue='<%# Bind("TipoOrcamento") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem Value="1">Venda</asp:ListItem>
                                                            <asp:ListItem Value="2">Revenda</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label14" runat="server" Text="Loja"></asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <uc8:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="false" MostrarTodas="false"
                                                            SelectedValue='<%# Bind("IdLoja") %>' OnLoad="Loja_Load" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">&nbsp;
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            A/C
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtAC" runat="server" MaxLength="50" Text='<%# Bind("AosCuidados") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Celular
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTelCel" runat="server" MaxLength="20" onkeypress="return soTelefone(event)"
                                                onkeydown="return maskTelefone(event, this);" Text='<%# Bind("CelCliente") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Tel Res.
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTelRes" runat="server" MaxLength="15" onkeypress="return soTelefone(event)"
                                                onkeydown="return maskTelefone(event, this);" Text='<%# Bind("TelCliente") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Contato
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtContato" runat="server" MaxLength="20" Text='<%# Bind("Contato") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Email
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="60" Text='<%# Bind("Email") %>'
                                                Width="244px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Bairro
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="50" Text='<%# Bind("Bairro") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Endereço
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtEndereco" runat="server" MaxLength="70" Text='<%# Bind("Endereco") %>'
                                                Width="244px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            CEP
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCep" runat="server" MaxLength="9" onkeypress="return soCep(event)"
                                                onkeyup="return maskCep(event, this);" Text='<%# Bind("Cep") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesquisarCep" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Cidade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Text='<%# Bind("Cidade") %>'
                                                Width="244px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Validade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtValidadeIns" runat="server" MaxLength="30" Text='<%# Bind("Validade") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Vendedor
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                                DataTextField="Nome" DataValueField="IdFunc" SelectedValue='<%# Bind("IdFuncionario") %>'
                                                AppendDataBoundItems="True" ondatabound="drpFuncionario_DataBound">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Obra
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox6" runat="server" MaxLength="30" Text='<%# Bind("Obra") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="tdTipoVenda1" align="left" class="dtvHeader" nowrap="nowrap">Tipo Venda
                                        </td>
                                        <td id="tdTipoVenda2" align="left" nowrap="nowrap" valign="middle">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoVenda" runat="server" SelectedValue='<%# Bind("TipoVenda") %>'
                                                            onchange="tipoVendaChange(this, true);"
                                                            DataSourceID="odsTipoVenda" DataTextField="Descr" DataValueField="Id">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <div id="divNumParc">
                                                            <table>
                                                                <tr>
                                                                    <td nowrap="nowrap" style="font-weight: bold">Num Parc.:
                                                                    </td>
                                                                    <td nowrap="nowrap">
                                                                        <uc6:ctrlParcelasSelecionar ID="ctrlParcelasSelecionar1" runat="server" ParcelaPadrao='<%# Bind("IdParcela") %>'
                                                                            NumeroParcelas='<%# Bind("NumParc") %>' OnLoad="ctrlParcelasSelecionar1_Load"
                                                                            CallbackSelecaoParcelas="callbackSetParcelas" />
                                                                        <asp:HiddenField ID="hdfDataBase" runat="server" OnLoad="hdfDataBase_Load" />
                                                                        <asp:HiddenField ID="hdfExibirParcela" runat="server" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTipoVendaAtual" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Prazo Entrega
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:TextBox ID="txtPrazoEntregaIns" runat="server" MaxLength="300" Text='<%# Bind("PrazoEntrega") %>'
                                                Width="250px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Tipo Entrega
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:DropDownList ID="ddlTipoEntrega" runat="server" DataSourceID="odsTipoEntrega"
                                                DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoEntrega") %>'
                                                AppendDataBoundItems="True">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoEntrega" runat="server" SelectMethod="GetTipoEntrega"
                                                TypeName="Glass.Data.Helper.DataSources"></colo:VirtualObjectDataSource>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Forma pagto.
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:TextBox ID="txtFormaPagtoIns" runat="server" MaxLength="200" Text='<%# Bind("FormaPagto") %>'
                                                Width="300px" TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            Data Entrega
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc7:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadWrite"
                                                DataString='<%# Bind("DataEntrega") %>' ExibirHoras="False" />
                                        </td>
                                    </tr>

                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="lblValorFrete" runat="server" OnLoad="txtValorFrete_Load" Text="Valor do Frete"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtValorFrete" runat="server" onkeypress="return soNumeros(event, false, true);" OnLoad="txtValorFrete_Load" Text='<%# Bind("ValorEntrega") %>' Width="80px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <table style="width: 100%">
                                    <tr>
                                        <td class="dtvHeader">
                                            Local da Obra
                                        </td>
                                        <td>
                                            Endereço
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEnderecoObra" runat="server" MaxLength="70" Text='<%# Bind("EnderecoObra") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                        <td>
                                            Bairro
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtBairroObra" runat="server" MaxLength="50" Text='<%# Bind("BairroObra") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                        <td>
                                            Cidade
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCidadeObra" runat="server" MaxLength="50" Text='<%# Bind("CidadeObra") %>'
                                                Width="120px"></asp:TextBox>
                                        </td>
                                        <td>
                                            Cep
                                        </td>
                                        <td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCepObra" runat="server" MaxLength="9" Text='<%# Bind("CepObra") %>' onkeypress="return soCep(event)"
                                                onkeyup="return maskCep(event, this);"></asp:TextBox>
                                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="iniciaPesquisaCepObra(FindControl('txtCepObra', 'input').value); return false" />
                                        </td>
                                        </td>
                                    </tr>
                                </table>
                                <table id="tbComissionado" style="width: 100%" class="dtvHeader" cellpadding="0"
                                    cellspacing="0">
                                    <tr>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        &nbsp;Comissionado:
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:Label ID="lblComissionado" runat="server" Text='<%# Eval("NomeComissionado") %>'></asp:Label>
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:LinkButton ID="lnkSelComissionado" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelComissionado.aspx'); return false;"
                                                            Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>">
                                                            <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                        <asp:ImageButton ID="imbLimparComissionado" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                            OnClientClick="limparComissionado(); return false;" ToolTip="Limpar comissionado" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                <tr>
                                                    <td>
                                                        Percentual:
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                            Text='<%# Bind("PercComissao") %>' Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        Valor Comissão:
                                                    </td>
                                                    <td>
                                                        &nbsp;<asp:TextBox ID="txtValorComissao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                            ReadOnly="True" Text='<%# Eval("ValorComissao", "{0:C}") %>' Width="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                <table style="width: 100%">
                                    <tr>
                                        <td class="dtvHeader" colspan="2">
                                            Observação
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:TextBox ID="TextBox3" runat="server" MaxLength="1000" Text='<%# Bind("Obs") %>'
                                                TextMode="MultiLine" Width="600px"></asp:TextBox>
                                        </td>
                                                    <td>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td class="dtvHeader" colspan="2">
                                                        Observação Interna
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:TextBox ID="TextBox4" runat="server" MaxLength="1000" Text='<%# Bind("ObsInterna") %>'
                                                            TextMode="MultiLine" Width="600px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfIdComissionado" runat="server" Value='<%# Bind("IdComissionado") %>' />
                                        </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="return onSaveOrca();" />
                                <img id="loading" src="../Images/load.gif" style="display: none" />
                                <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" CommandName="Cancel"
                                    Text="Cancelar" OnClick="btnCancelarEditar_Click" />
                                <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdOrca"
                                    Text='<%# Bind("IdOrcamento") %>' />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="if (!onInsert()) return false;" />
                                <img id="load" alt="" src="../Images/load.gif" style="display: none" />
                                <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                    Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" OnClick="btnEditar_Click"
                                    Visible='<%# Eval("EditEnabled") %>' />
                                <asp:Button ID="btnVoltar" runat="server" OnClick="btnCancelar_Click" Text="Voltar" />
                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible="true">
                                        <a href="#" onclick='openWindow(500, 700, "../Utils/SelTextoOrca.aspx?idOrca="+&#039;<%# Eval("IdOrcamento").ToString() %>&#039;); return false;'>
                                                        <img border="0" src="../Images/note_add.gif" title="Textos Orçamento" /></a>
                                                </asp:PlaceHolder>
                                            <asp:ImageButton ID="imgRelatorio" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                                OnClientClick='<%# "openRpt(\"" + Eval("IdOrcamento") + "\"); return false" %>' Visible='<%# Eval("ExibirImpressao") %>' />
                                            <asp:ImageButton ID="imgMemoriaCalculo" runat="server" ImageUrl="~/Images/calculator.gif"
                                                ToolTip="Memória de cálculo" OnClientClick='<%# "openRptMemoria(\"" + Eval("IdOrcamento") + "\"); return false" %>'
                                                Visible='<%# Eval("ExibirRelatorioCalculo") %>' />
                                <asp:LinkButton ID="lnkRecalcular" runat="server" OnClientClick='<%# "recalcular(" + Eval("IdOrcamento") + ", true " +
                                        (Eval("TipoEntrega") == null || string.IsNullOrEmpty(Eval("TipoEntrega").ToString()) ? string.Empty : "," + Eval("TipoEntrega")) +
                                        (Eval("IdCliente") == null || string.IsNullOrEmpty(Eval("IdCliente").ToString()) ? string.Empty : "," + Eval("IdCliente")) + "); return false" %>'>
                                    <img src="../Images/dinheiro.gif" border="0" />&nbsp;Recalcular orçamento</asp:LinkButton>
                                <div style="margin-top: 5px">
                                    <asp:LinkButton ID="lnkGerarPedido" runat="server" CommandName="GerarPedido" OnClientClick="return geraPedido(this, '{0}', true)"
                                        OnClick="lnkGerarPedido_Click" OnLoad="lnkGerarPedido_Load" Visible='<%# Eval("GerarPedidoVisible") %>'>
                                        <img border="0" src="../Images/cart_add.gif">&nbsp;Gerar Pedido</img></asp:LinkButton>
                                    <br />
                                    <asp:LinkButton ID="lnkGerarPedidoAgrupado" runat="server" CommandName="GerarPedidoAgrupado" OnClientClick="return geraPedido(this, '{0}', true)"
                                        OnClick="lnkGerarPedidoAgrupado_Click" OnLoad="lnkGerarPedidoAgrupado_Load" Visible='<%# Eval("GerarPedidoVisible") %>'>
                                        <img border="0" src="../Images/cart_add.gif">&nbsp;Gerar Pedido Agrupado</img></asp:LinkButton>

                                    <asp:LinkButton ID="lnkMedicaoDef" runat="server" Visible='<%# Eval("ExibirMedicaoDefinitiva") %>'
                                        OnClick="lnkMedicaoDef_Click" OnClientClick="if (!confirm(&quot;Deseja gerar a medição definitiva para esse orçamento?&quot;)) return false">Gerar Medição Definitiva</asp:LinkButton>
                                </div>
                                <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                                <img id="loading" src="../Images/load.gif" style="display: none; margin-top: 8px" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr runat="server" id="linhaAmbiente">
            <td align="center">
                <asp:GridView GridLines="None" ID="grdAmbiente" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsAmbiente" DataKeyNames="IdAmbienteOrca" OnRowCommand="grdAmbiente_RowCommand"
                    ShowFooter="True" AllowPaging="True" AllowSorting="True" OnRowDataBound="grdAmbiente_RowDataBound"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" OnRowDeleted="grdAmbiente_RowDeleted">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfIdOrcamento" runat="server" Value='<%# Bind("IdOrcamento") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton ID="imbNovo" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="addAmbiente(true); return false;" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Deseja excluir este ambiente?&quot;)" />
                                <asp:LinkButton ID="lnkProdutos" runat="server" CommandArgument='<%# Eval("IdAmbienteOrca") %>'
                                    CommandName="Produtos">Produtos</asp:LinkButton>
                                <asp:HiddenField ID="hdfIdAmbiente" runat="server" Value='<%# Eval("IdAmbienteOrca") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" MaxLength="50" Text='<%# Bind("Ambiente") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvAmbiente" runat="server" ControlToValidate="TextBox2"
                                    Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAmbienteIns" runat="server" MaxLength="50" Style="display: none"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvAmbiente" runat="server" ControlToValidate="txtAmbienteIns"
                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="inserirAmbiente"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Ambiente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Columns="50" MaxLength="1000" Rows="5"
                                    Text='<%# Bind("Descricao") %>' TextMode="MultiLine"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricaoIns" runat="server" Columns="50" MaxLength="1000" Rows="5"
                                    TextMode="MultiLine" Style="display: none"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Produtos" SortExpression="ValorProdutos">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("ValorProdutos", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("ValorProdutos", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imbInserir" runat="server" ImageUrl="~/Images/ok.gif" OnClick="imbInserir_Click"
                                    Style="display: none" ValidationGroup="inserirAmbiente" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label ID="lblAmbiente" runat="server" CssClass="subtitle1"></asp:Label>
                <br />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAmbiente" runat="server" SelectMethod="GetList" TypeName="Glass.Data.DAL.AmbienteOrcamentoDAO"
                    DataObjectTypeName="Glass.Data.Model.AmbienteOrcamento" DeleteMethod="Delete"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetListCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" UpdateMethod="Update">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idOrca" QueryStringField="IdOrca" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVendaOrcamento"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>

                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedido"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>

                <asp:HiddenField ID="hdfIdAmbienteOrca" runat="server" />
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkProduto" OnClientClick="return openProdutos('', false);" runat="server"> Incluir Produto</asp:LinkButton>&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkProjeto" OnClientClick="return openProjeto('', false);" runat="server"> Incluir Projeto</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXOrc" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    DataKeyNames="IdProd" EmptyDataText="Não há produtos associados ao orçamento"
                    OnRowDeleted="grdProdutos_RowDeleted" ShowFooter="True" OnRowCommand="grdProdutos_RowCommand"
                    OnDataBound="grdProdutos_DataBound" OnRowUpdating="grdProdutos_RowUpdating">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onUpdateProduto();" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfIdOrca" Value='<%# Eval("IdOrcamento") %>' runat="server" />
                                <asp:HiddenField ID="hdfIdItemProjeto" runat="server" Value='<%# Eval("IdItemProjeto") %>' />
                                <asp:HiddenField ID="hdfValorProd" runat="server" Value='<%# Eval("ValorProd") %>' />
                                <asp:HiddenField ID="hdfAltura" runat="server" Value='<%# Eval("Altura") %>' />
                                <asp:HiddenField ID="hdfLargura" runat="server" Value='<%# Eval("Largura") %>' />
                                <asp:HiddenField ID="hdfRedondo" runat="server" Value='<%# Eval("Redondo") %>' />
                                <asp:HiddenField ID="hdfTotM2" runat="server" Value='<%# Eval("TotM") %>' />
                                <asp:HiddenField ID="hdfValorBenef" runat="server" Value='<%# Eval("ValorBenef") %>' />
                                <asp:HiddenField ID="hdfImagemModPath" runat="server" Value='<%# Eval("ImagemProjModPath") %>' />
                                <asp:HiddenField ID="hdfNumItem" runat="server" Value='<%# Eval("NumItem") %>' />
                                <asp:HiddenField ID="hdfEspessura" runat="server" Value='<%# Bind("Espessura") %>' />
                                <asp:HiddenField ID="hdfCusto" runat="server" Value='<%# Bind("Custo") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif"
                                    ToolTip="Editar" CausesValidation="False" Visible='<%# Eval("IdProdPed") == null %>'
                                    OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>' />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" Visible='<%# Eval("IdProdPed") == null %>' OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(false); return false" : "if (!confirm(\"Deseja excluir esse produto do orçamento?\")) return false" %>' />
                                <asp:ImageButton ID="imgIncluirItem" runat="server" ToolTip="Inserir produtos nesse ambiente"
                                    ImageUrl="~/Images/Insert.gif" OnDataBinding="ImageButton1_DataBinding" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" OnLoad="Label6_Load" Text='<%# Bind("Ambiente") %>'></asp:Label>
                                <asp:TextBox ID="txtAmbienteProd" runat="server" Text='<%# Bind("Ambiente") %>' Width="100px"
                                    OnLoad="txtAmbiente_Load"></asp:TextBox>
                                <asp:HiddenField ID="hdfProdOrca" runat="server" Value='<%# Eval("IdProd") %>' />
                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                <asp:HiddenField ID="hdfIsVidro" runat="server" />
                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Bind("TipoCalculoUsado") %>' />
                                <asp:HiddenField ID="hdfIsAluminio" runat="server" />
                                <asp:HiddenField ID="hdfM2Minimo" runat="server" />
                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" />
                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAmbienteProd" runat="server" Width="100px"></asp:TextBox>
                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                <asp:HiddenField ID="hdfIsVidro" runat="server" />
                                <asp:HiddenField ID="hdfTipoCalc" runat="server" />
                                <asp:HiddenField ID="hdfIsAluminio" runat="server" />
                                <asp:HiddenField ID="hdfM2Minimo" runat="server" />
                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" />
                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" />
                                <asp:HiddenField ID="hdfIdProd" runat="server" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Ambiente") %>' Visible='<%# (!(bool)Eval("TemItensProduto") && Eval("IdItemProjeto") == null) || !lnkProduto.Visible %>'></asp:Label>
                                <asp:LinkButton ID="lkbAmbiente" runat="server" ToolTip="Editar os produtos desse ambiente" Enabled='<%# Eval("IdProdPed") == null %>'
                                    OnClientClick='<%# Eval("IdProdPed") == null ? (!(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "return openProdutos(\"" + Eval("IdProd") + "\", true);") : "mensagemProdutoComPedido(true); return false" %>'
                                    Text='<%# Eval("Ambiente") %>' Visible='<%# (bool)Eval("TemItensProduto") && lnkProduto.Visible %>'></asp:LinkButton>
                                <asp:LinkButton ID="lkbProjeto" runat="server" ToolTip="Editar projeto" OnClientClick='<%# Eval("IdProdPed") == null ? (!(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "return openProjeto(\"" + Eval("IdProd") + "\");") : "mensagemProdutoComPedido(true); return false" %>'
                                    Text='<%# Eval("Ambiente") %>' Visible='<%# Eval("IdItemProjeto") != null && lnkProduto.Visible %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" Width="95%" MaxLength="1500" runat="server" Text='<%# Bind("Descricao") %>'
                                    Rows="3" TextMode="MultiLine" Visible='<%# Eval("IdProduto") == null %>' Style='min-width: 350px'></asp:TextBox>
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("IdProduto") != null %>'>
                                    <asp:TextBox ID="txtCodProdIns" runat="server" Text='<%# Eval("CodInterno") %>' onblur="loadProduto(this.value);"
                                        onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                        Width="50px"></asp:TextBox>
                                    <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                    <a href="#" onclick="getProduto(); return false;">
                                        <img border="0" src="../Images/Pesquisar.gif" /></a> </asp:PlaceHolder>
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProduto") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodProdIns" runat="server" onblur="loadProduto(this.value);"
                                    onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                    Width="50px"></asp:TextBox>
                                <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
                                <a href="#" onclick="getProduto(); return false;">
                                    <img border="0" src="../Images/Pesquisar.gif" /></a>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                <asp:Label ID="Label42" runat="server" ForeColor="Red" Text='<%# Eval("DescrObsProj") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                            <EditItemTemplate>
                                <asp:Label ID="lblQtde" runat="server" OnDataBinding="EditarValorQtde_DataBinding"
                                    Text='<%# Bind("Qtde") %>' Visible="False"></asp:Label>
                                <asp:TextBox ID="txtQtde" Width="40px" runat="server" Text='<%# Bind("Qtde") %>'
                                    onblur="calcTotalProd()" OnDataBinding="EditarValorQtde_DataBinding"></asp:TextBox>
                                <uc4:ctrlDescontoQtde ID="ctrlDescontoQtde1" runat="server" OnLoad="ctrlDescontoQtde1_Load"
                                    Callback="calcTotalProd" CallbackValorUnit="calcTotalProd" ValorDescontoQtde='<%# Bind("ValorDescontoQtde") %>'
                                    PercDescontoQtde='<%# Bind("PercDescontoQtde") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" onblur="calcTotalProd()" OnDataBinding="EditarValorQtde_DataBinding"
                                    Text='<%# Bind("Qtde") %>' Width="40px"></asp:TextBox>
                                <uc4:ctrlDescontoQtde ID="ctrlDescontoQtde1" runat="server" Callback="calcTotalProd"
                                    CallbackValorUnit="calcTotalProd" OnLoad="ctrlDescontoQtde1_Load" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="ValorProdAmbiente">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" OnDataBinding="EditarValorQtde_DataBinding"
                                    Text='<%# Bind("ValorProd", "{0:C}") %>' Visible="False"></asp:Label>
                                <uc1:ctrlTextBoxFloat ID="txtValorIns" runat="server" Value='<%# Bind("ValorProd") %>'
                                    onblur="calcTotalProd()" OnDataBinding="EditarValorQtde_DataBinding" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlTextBoxFloat ID="txtValorIns" runat="server" onblur="calcTotalProd()" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("ValorProdAmbiente", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="false" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                            <EditItemTemplate>
                                <asp:Label ID="lblTotalProd" runat="server" Text='<%# Eval("TotalAmbiente", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotalProd" runat="server" Text='<%# Eval("TotalAmbiente", "{0:C}") %>'></asp:Label>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("TotalAmbiente", "{0:C}") %>'></asp:Label>
                                <asp:Label ID="Label43" runat="server" Text='<%# "(Desconto de " + Eval("PercDescontoQtde") + "%)" %>'
                                    Visible='<%# (float)Eval("PercDescontoQtde") > 0 %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="false" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Negociar?" SortExpression="Negociar">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkNegociar" runat="server" AutoPostBack="True" Checked='<%# Bind("Negociar") %>'
                                    Enabled='<%# Eval("IdProdPed") == null %>' OnCheckedChanged="chkNegociar_CheckedChanged" />
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Eval("IdProd") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblNegociar" runat="server" Text='<%# (bool)Eval("Negociar") ? "Sim" : "Não" %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Acréscimo">
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="drpTipoAcrescimo" runat="server" SelectedValue='<%# Bind("TipoAcrescimo") %>'
                                                Enabled='<%# Eval("DescontoAcrescimoPermitido") %>'>
                                                <asp:ListItem Value="1">%</asp:ListItem>
                                                <asp:ListItem Value="2">R$</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtAcrescimo" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"
                                                Text='<%# Bind("Acrescimo") %>' Enabled='<%# Eval("DescontoAcrescimoPermitido") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Label ID="Label15" runat="server" Style="display: block" Text="Acréscimo não permitido para produtos que não sejam projeto"
                                    Visible='<%# !(bool)Eval("DescontoAcrescimoPermitido") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label40" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Desconto">
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="drpTipoDesconto" runat="server" SelectedValue='<%# Bind("TipoDesconto") %>'
                                                onclick="calcularDesconto(2)" Enabled='<%# Eval("DescontoAcrescimoPermitido") %>'>
                                                <asp:ListItem Value="1">%</asp:ListItem>
                                                <asp:ListItem Value="2">R$</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDesconto" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"
                                                onchange="calcularDesconto(2)" Text='<%# Bind("Desconto") %>' Enabled='<%# Eval("DescontoAcrescimoPermitido") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Label ID="Label16" runat="server" Style="display: block" Text="Desconto não permitido para produtos que não sejam projeto"
                                    Visible='<%# !(bool)Eval("DescontoAcrescimoPermitido") %>'></asp:Label>
                                <asp:HiddenField ID="hdfValorDescontoAtual" runat="server" Value='<%# Eval("ValorDescontoAtual") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label41" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc5:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemUrl") %>'
                                    Visible="<%# Glass.Configuracoes.OrcamentoConfig.UploadImagensOrcamento %>" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <div runat="server" id="imagemProdutoOrca" onload="imagemProdutoOrca_Load">
                                    <uc5:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemUrl") %>' />
                                    <asp:ImageButton ID="imbExcluirImagem" runat="server" OnClientClick='if (!confirm("Deseja excluir a imagem desse produto?")) return false;'
                                        ImageUrl="~/Images/ExcluirGrid.gif" Visible='<%# Eval("TemImagem") %>' CommandArgument='<%# Eval("IdProd") %>'
                                        OnClick="imbExcluirImagem_Click" />
                                    <asp:FileUpload ID="fluImagem" runat="server" />
                                </div>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imgAdd_Click" />
                            </FooterTemplate>
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
                <asp:HiddenField ID="hdfIdOrca" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedoresOrca"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idOrcamento" QueryStringField="idOrca" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsOrcamento" runat="server" DataObjectTypeName="Glass.Data.Model.Orcamento"
                    InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.OrcamentoDAO"
                    UpdateMethod="UpdateComTransacao" OnInserted="odsOrcamento_Inserted" OnUpdated="odsOrcamento_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idOrca" QueryStringField="idorca" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdXOrc" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosOrcamento"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    OnDeleted="odsProdXOrc_Deleted" SelectCountMethod="GetCount" SelectMethod="GetList"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosOrcamentoDAO"
                    InsertMethod="Insert" UpdateMethod="UpdateComTransacao" OnUpdated="odsProdXOrc_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idOrca" QueryStringField="idOrca" Type="UInt32" />
                        <asp:ControlParameter ControlID="hdfIdAmbienteOrca" Name="idAmbiente" PropertyName="Value"
                            Type="UInt32" />
                        <asp:Parameter DefaultValue="false" Name="showChildren" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfComissaoVisible" runat="server" />
                <asp:HiddenField ID="hdfNaoVendeVidro" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoOrcamento"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <div style="display: none">
                    <uc3:ctrlBenef ID="ctrlBenef1" runat="server" OnLoad="ctrlBenef1_Load" />
                    <asp:HiddenField ID="hdfBenefAltura" runat="server" />
                    <asp:HiddenField ID="hdfBenefEspessura" runat="server" />
                    <asp:HiddenField ID="hdfBenefLargura" runat="server" />
                    <asp:HiddenField ID="hdfBenefIdProd" runat="server" />
                    <asp:HiddenField ID="hdfBenefQtde" runat="server" />
                    <asp:HiddenField ID="hdfBenefTotM" runat="server" />
                    <asp:HiddenField ID="hdfBenefValorUnit" runat="server" />
                </div>
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        // Esconde tabela de comissionado
        var hdfComissaoVisible = FindControl("hdfComissaoVisible", "input");
        var tbComissionado = FindControl("tbComissionado", "table");
        var loading = true;
        if (hdfComissaoVisible != null && tbComissionado != null && hdfComissaoVisible.value == "false")
            tbComissionado.style.display = "none";

        // Esconde colunas do ambiente
        var grdAmbiente = document.getElementById("<%= grdAmbiente.ClientID %>");
        if (grdAmbiente != null)
        {
            for (i = 0; i < grdAmbiente.rows.length; i++)
            {
                if (i == (grdAmbiente.rows.length - 1) && grdAmbiente.rows[i].cells.length == 1)
                    continue;

                grdAmbiente.rows[i].cells[2].style.display = "none";
            }
        }

        var tbProd = FindControl("grdProdutos", "table");
        if (FindControl("hdfNaoVendeVidro", "input").value == "true" && tbProd != null)
        {
            for (k = 0; k < tbProd.rows.length; k++)
                tbProd.rows[k].cells[1].style.display = "none";
        }

        tipoEntrega = FindControl("ddlTipoEntrega", "select");
        tipoEntrega = tipoEntrega != null ? tipoEntrega.value : "";

        idCliente = FindControl("txtIdCliente", "input");
        idCliente = idCliente != null ? idCliente.value : "";

        var idCli = FindControl("txtIdCliente", "input");
        if (idCli != null && idCli.value != "")
        {
            var dados = CadOrcamento.GetCli(idCli.value).value;

            if (dados != null || dados != "" || dados.split('|')[0] != "Erro")
            {
                dados = dados.split("|");
                FindControl("lblObsCliente", "span").innerHTML = dados[12];
            }
        }

        if (FindControl("drpLoja", "select") != null)
        {
            // Desabilita o drop da loja caso a config "Alterar loja no orçamento" esteja desabilitada
            if(alterarLojaOrcamento == "false" || !alterarLojaOrcamento)
                FindControl("drpLoja", "select").disabled = true;
            else
                FindControl("drpLoja", "select").disabled = false;
        }

        var drpTipoVenda = FindControl("drpTipoVenda", "select");
        if (drpTipoVenda != null)
        {
            tipoVendaChange(drpTipoVenda, false);

            if (FindControl("hdfExibirParcela", "input") != null)
                FindControl("hdfExibirParcela", "input").value = drpTipoVenda.value == 2;

            if (FindControl("hdfCalcularParcela", "input") != null)
                FindControl("hdfCalcularParcela", "input").value = false;
        }

        loading = false;

    </script>

</asp:Content>
