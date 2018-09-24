<%@ Page Language="C#" MasterPageFile="~/WebGlassParceiros/PainelParceiros.master" AutoEventWireup="true" CodeBehind="CadPedido.aspx.cs"
         Inherits="Glass.UI.Web.WebGlassParceiros.CadPedido" Title="Cadastrar Pedido" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register src="../Controls/ctrlDescontoQtde.ascx" tagname="ctrlDescontoQtde" tagprefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

    // Guarda a quantidade disponível em estoque do produto buscado
    var qtdEstoque = 0;

    var codCartao = CadPedido.GetCartaoCod().value;
    var dataEntregaAntiga = "<%= GetDataEntrega() %>";
    var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;
    var inserting = false;
    var produtoAmbiente = false;
    var aplAmbiente = false;
    var procAmbiente = false;

    function atualizaValMin()
    {
        if (FindControl("hdfTamanhoMaximoObra", "input") != null && parseFloat(FindControl("hdfTamanhoMaximoObra", "input").value.replace(",", ".")) == 0)
        {
            var codInterno = FindControl("txtCodProdIns", "input");
            codInterno = codInterno != null ? codInterno.value : FindControl("lblCodProdIns", "span").innerHTML;

            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;

            var idProdPed = FindControl("hdfProdPed", "input");
            idProdPed = idProdPed != null ? idProdPed.value : "";

            var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

            var percDescontoQtde = controleDescQtde.PercDesconto();

            var reposicao = FindControl("hdfIsReposicao", "input").value;

            FindControl("hdfValMin", "input").value = CadPedido.GetValorMinimo(codInterno, tipoEntrega, idCliente, cliRevenda, reposicao, idProdPed, percDescontoQtde, idPedido).value;
        }
        else if (FindControl("hdfValMin", "input") != null && FindControl("txtValorIns", "input") != null)
        {
            FindControl("hdfValMin", "input").value = FindControl("txtValorIns", "input").value;
        }
    }

    function obrigarProcApl()
    {
        var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;

        var nomeControle = "<%= NomeControleBenef() %>";
        nomeControle = FindControl(nomeControle + "_tblBenef", "table");

        if (nomeControle == null)
            return true;

        nomeControle = nomeControle.id;
        nomeControle = nomeControle.substr(0, nomeControle.lastIndexOf("_"));

        var isVidroBenef = exibirControleBenef(nomeControle);
        var tipoCalculo = FindControl("hdfTipoCalc", "input") != null && FindControl("hdfTipoCalc", "input") != undefined && FindControl("hdfTipoCalc", "input").value != undefined ? FindControl("hdfTipoCalc", "input").value : "";

        /* Chamado 63268. */
        if ((tipoCalculo != "" && (tipoCalculo == "2" || tipoCalculo == "10")) && isObrigarProcApl && isVidroBenef)
        {
            if (FindControl("txtAplIns", "input") != null && FindControl("txtAplIns", "input").value == "")
            {
                alert("Informe a aplicação.");
                return false;
            }

            if (FindControl("txtProcIns", "input") != null && FindControl("txtProcIns", "input").value == "")
            {
                alert("Informe o processo.");
                return false;
            }
        }

        return true;
    }

    function calculaTamanhoMaximo()
    {
        if (FindControl("lblCodProdIns", "span") == null || FindControl("lblTotM2Ins", "span") == null)
        {
            return true;
        }

        var codInterno = FindControl("lblCodProdIns", "span").innerHTML;
        var totM2 = FindControl("lblTotM2Ins", "span").innerHTML;

        var tamanhoMaximo = CadPedido.GetTamanhoMaximoProduto(idPedido, codInterno, totM2).value.split(";");
        tamanhoMaximo = tamanhoMaximo[0] == "Ok" ? parseFloat(tamanhoMaximo[1].replace(",", ".")) : 0;

        FindControl("hdfTamanhoMaximoObra", "input").value = tamanhoMaximo;
    }

    function validaTamanhoMax()
    {
        if (FindControl("hdfTamanhoMaximoObra", "input") == null)
        {
            return true;
        }

        var tamanhoMaximo = parseFloat(FindControl("hdfTamanhoMaximoObra", "input").value.replace(",", "."));
        if (tamanhoMaximo > 0)
        {
            var totM2 = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(",", "."));
            if (totM2 > tamanhoMaximo)
            {
                alert("O total de m² da peça ultrapassa o máximo definido no pagamento antecipado. Tamanho máximo: " + tamanhoMaximo.toString().replace(".", ",") + " m²");
                return false;
            }
        }

        return true;
    }

    function exibirBenef(botao)
    {
        for (iTip = 0; iTip < 2; iTip++)
        {
            TagToTip('tbConfigVidro', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true,
                CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9-getTableWidth('tbConfigVidro'), -41-getTableHeight('tbConfigVidro')]);
        }
    }

    function calcularDesconto(controle, tipoCalculo)
    {
        if (FindControl("drpTipoDesconto", "select") == null || FindControl("hdfTotalSemDesconto", "input") == null || FindControl("lblTotalProd", "span") == null ||
            FindControl("hdfValorDescontoAtual", "input") == null)
        {
            return true;
        }

        var parcela = FindControl("drpParcelas", "select");
        var tipoVenda = FindControl("hdfTipoVendaAtual", "input").value;
        var descontoMaximo = MetodosAjax.GetDescMaxPedido(<%= Glass.Data.Helper.UserInfo.GetUserInfo.CodUser %>, tipoVenda, parcela.value).value;
        var tipo = FindControl("drpTipoDesconto", "select").value;
        var total = parseFloat(FindControl("hdfTotalSemDesconto", "input").value.replace(/\./g, "").replace(',', '.'));
        var totalProduto = tipoCalculo == 2 ? parseFloat(FindControl("lblTotalProd", "span").innerHTML.replace("R$", "").replace(" ", "").replace(/\./g, "").replace(',', '.')) : 0;
        var valorDescontoMaximo = total * (descontoMaximo / 100);

        var valorDescontoProdutos = <%= GetDescontoProdutos() %> - (tipoCalculo == 2 ? parseFloat(FindControl("hdfValorDescontoAtual", "input").value.replace(',', '.')) : 0);
        var valorDescontoPedido = tipoCalculo == 2 ? <%= GetDescontoPedido() %> : 0;
        var descontoProdutos = parseFloat(((valorDescontoProdutos / (total > 0 ? total : 1)) * 100).toFixed(2));
        var descontoPedido = parseFloat(((valorDescontoPedido / (total > 0 ? total : 1)) * 100).toFixed(2));

        var descontoSomar = descontoProdutos + (tipoCalculo == 2 ? descontoPedido : 0);
        var valorDescontoSomar = valorDescontoProdutos + (tipoCalculo == 2 ? valorDescontoPedido : 0);

        var desconto = parseFloat(controle.value.replace(',', '.'));
        if (isNaN(desconto))
            desconto = 0;

        if (tipo == 2)
            desconto = (desconto / total) * 100;

        if (desconto + descontoSomar > descontoMaximo)
        {
            var mensagem = "O desconto máximo permitido é de " + (tipo == 1 ? descontoMaximo + "%" : "R$ " + valorDescontoMaximo.toFixed(2).replace('.', ',')) + ".";
            if (descontoProdutos > 0)
                mensagem += "\nO desconto já aplicado aos produtos é de " + (tipo == 1 ? descontoProdutos + "%" : "R$ " + valorDescontoProdutos.toFixed(2).replace('.', ',')) + ".";

            if (descontoPedido > 0)
                mensagem += "\nO desconto já aplicado ao pedido é de " + (tipo == 1 ? descontoOrcamento + "%" : "R$ " + valorDescontoPedido.toFixed(2).replace('.', ',')) + ".";

            alert(mensagem);
            controle.value = tipo == 1 ? descontoMaximo - descontoSomar : (valorDescontoMaximo - valorDescontoSomar).toFixed(2).replace('.', ',') ;
        }
    }

    function alteraFastDelivery(isFastDelivery)
    {
        if (FindControl("txtDataEntrega", "input") == null)
        {
            return true;
        }

        var tf = FindControl("chkTemperaFora", "input");
        if (tf != null)
        {
            tf.checked = false;
        }

        var alterar = <%= (Glass.Configuracoes.PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido > 0).ToString().ToLower() %>;
        if (!alterar)
        {
            return;
        }

        var novaData = isFastDelivery && FindControl("hdfDataEntregaFD", "input") != null ? FindControl("hdfDataEntregaFD", "input").value :
            FindControl("hdfDataEntregaNormal", "input") != null ? FindControl("hdfDataEntregaNormal", "input").value : FindControl("txtDataEntrega", "input").value;

        FindControl("txtDataEntrega", "input").value = novaData;
    }

    function alteraTemperaFora(isTemperaFora)
    {
        var fd = FindControl("chkFastDelivery", "input");
        if (fd != null)
        {
            fd.checked = false;
        }
    }

    function limparComissionado()
    {
        if (FindControl("hdfIdComissionado", "input") == null || FindControl("lblComissionado", "span") == null || FindControl("txtPercentual", "input") == null ||
            FindControl("txtValorComissao", "input") == null)
        {
            return true;
        }

        FindControl("hdfIdComissionado", "input").value = "";
        FindControl("lblComissionado", "span").innerHTML = "";
        FindControl("txtPercentual", "input").value = "0";
        FindControl("txtValorComissao", "input").value = "R$ 0,00";
    }

    function getProduto()
    {
        openWindow(450, 700, '../Utils/SelProd.aspx?IdPedido=<%= Request["IdPedido"] %>' + (produtoAmbiente ? "&ambiente=true" : "") + "&Parceiro=1");
    }

    function verificaDataEntrega(controle)
    {
        if (<%= (Glass.Configuracoes.PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido == 0).ToString().ToLower() %>)
        {
            return true;
        }

        if (FindControl("hdfDataEntregaNormal", "input") == null)
        {
            return true;
        }

        var textoDataMinima = FindControl("hdfDataEntregaNormal", "input").value;
        var dataControle = textoDataMinima.split("/");
        var dataMinima = new Date(dataControle[2], parseInt(dataControle[1], 10) - 1, dataControle[0]);
        var isDataMinima = <%= GetBloquearDataEntrega().ToString().ToLower() %>;

        dataControle = controle.value.split("/");
        var dataAtual = new Date(dataControle[2], parseInt(dataControle[1], 10) - 1, dataControle[0]);

        if (isDataMinima && dataAtual < dataMinima)
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

    function setValorTotal(valor, custo)
    {
        if (FindControl("hdfIsVidro", "input") != null && FindControl("hdfIsVidro", "input").value == "true")
        {
            var lblValorBenef = FindControl("lblValorBenef", "span");
            lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
        }
    }

    function setObra(idCliente, idObra, descrObra, saldo)
    {
        if (FindControl("txtNumCli", "input") == null || FindControl("hdfIdObra", "input") == null || FindControl("txtObra", "input") == null || FindControl("lblSaldoObra", "span") == null)
        {
            return true;
        }

        FindControl("txtNumCli", "input").value = idCliente;
        FindControl("hdfIdObra", "input").value = idObra;
        FindControl("txtObra", "input").value = descrObra;
        FindControl("lblSaldoObra", "span").innerHTML = saldo;

        getCli(FindControl("txtNumCli", "input"));
    }

    // Função chamada após selecionar produto pelo popup
    function setProduto(codInterno) {
        try {
            if (!produtoAmbiente)
                FindControl("txtCodProd", "input").value = codInterno;
            else
                FindControl("txtCodAmb", "input").value = codInterno;

            loadProduto(codInterno);
        }
        catch (err) {

        }
    }

    // Retorna o percentual de comissão
    function getPercComissao()
    {
        var percComissao = 0;

        if (FindControl("txtPercentual", "input") != null)
            percComissao = parseFloat(FindControl("txtPercentual", "input").value.replace(',', '.'));
        else if (FindControl("hdfPercComissao", "input") != null)
            percComissao = parseFloat(FindControl("hdfPercComissao", "input").value.replace(',', '.'));

        return percComissao != null ? percComissao : 0;
    }

    // Carrega dados do produto com base no código do produto passado
    function loadProduto(codInterno) {
        if (codInterno == "")
            return false;

        var verificaProduto = CadPedido.IsProdutoObra(idPedido, codInterno, false).value.split(";");
        if (verificaProduto[0] == "Erro")
        {
            if (FindControl("txtCodProd", "input") != null)
            {
                FindControl("txtCodProd", "input").value = "";
            }

            alert("Esse produto não pode ser usado no pedido. " + verificaProduto[1]);
            return false;
        }
        else if (verificaProduto[1] > 0)
        {
            if (FindControl("txtValorIns", "input") != null)
            {
                FindControl("txtValorIns", "input").disabled = true;
            }

            if (FindControl("hdfTamanhoMaximoObra", "input") != null)
            {
                FindControl("hdfTamanhoMaximoObra", "input").value = verificaProduto[2];
            }
        }
        else
        {
            if (FindControl("txtValorIns", "input") != null)
            {
                FindControl("txtValorIns", "input").disabled = verificaProduto[3];
            }

            if (FindControl("hdfTamanhoMaximoObra", "input") != null)
            {
                FindControl("hdfTamanhoMaximoObra", "input").value = "0";
            }
        }

        try {
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var pedidoMaoDeObra = FindControl("hdfPedidoMaoDeObra", "input").value;
            var pedidoProducao = FindControl("hdfPedidoProducao", "input").value;
            var percComissao = getPercComissao();
            percComissao = percComissao == null ? 0 : percComissao.toString().replace('.', ',');

            var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

            var percDescontoQtde = controleDescQtde.PercDesconto();

            var reposicao = FindControl("hdfIsReposicao", "input").value;

            var retorno = CadPedido.GetProduto(codInterno, tipoEntrega, cliRevenda, reposicao, idCliente,
                percComissao, pedidoMaoDeObra, pedidoProducao, produtoAmbiente, percDescontoQtde, FindControl("hdfLoja", "input").value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                if (!produtoAmbiente)
                    FindControl("txtCodProd", "input").value = "";
                else
                    FindControl("txtCodAmb", "input").value = "";

                return false;
            }

            else if (!produtoAmbiente)
            {
                if (retorno[0] == "Prod") {

                    valorTabelaProduto = verificaProduto[1] != "0" ? verificaProduto[1] : retorno[3];

                    FindControl("hdfIdProd", "input").value = retorno[1];
                    FindControl("txtValorIns", "input").value = verificaProduto[1] != "0" ? verificaProduto[1] : retorno[3]; // Exibe no cadastro o valor mínimo do produto
                    FindControl("hdfIsVidro", "input").value = retorno[4]; // Informa se o produto é vidro
                    FindControl("hdfM2Minimo", "input").value = retorno[5]; // Informa se o produto possui m² mínimo
                    FindControl("hdfTipoCalc", "input").value = retorno[7]; // Verifica como deve ser calculado o produto

                    atualizaValMin();

                    qtdEstoque = retorno[6]; // Pega a quantidade disponível em estoque deste produto
                    var tipoCalc = retorno[7];

                    // Se o produto não for vidro, desabilita os textboxes largura e altura,
                    // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
                    // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
                    var cAltura = FindControl("txtAlturaIns", "input");
                    var cLargura = FindControl("txtLarguraIns", "input");
                    var maoDeObra = FindControl("hdfPedidoMaoDeObra", "input").value == "true";
                    cAltura.disabled = maoDeObra || CalcProd_DesabilitarAltura(tipoCalc);
                    cLargura.disabled = maoDeObra || CalcProd_DesabilitarLargura(tipoCalc);
                    cAltura.value = !maoDeObra ? "" : (tipoCalc != 1 && tipoCalc != 5 ? FindControl("hdfAlturaAmbiente", "input").value : "");
                    cLargura.value = !maoDeObra ? "" : (tipoCalc != 1 && tipoCalc != 4 && tipoCalc != 5 && tipoCalc != 6 && tipoCalc != 7 && tipoCalc != 8 ?
                        FindControl("hdfLarguraAmbiente", "input").value : "");

                    // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                    if (retorno[4] == "true" && retorno[7] == "2" && FindControl("lnkBenef", "a") != null) {
                        FindControl("lnkBenef", "a").style.display = "inline";
                        FindControl("txtEspessura", "input").value = retorno[8];
                        FindControl("txtEspessura", "input").disabled = retorno[8] != "" && retorno[8] != "0";
                    }
                    else if (FindControl("lnkBenef", "a") != null)
                        FindControl("lnkBenef", "a").style.display = "none";

                    FindControl("hdfAliquotaIcmsProd", "input").value = retorno[9];

                    //if (FindControl("hdfPedidoProducao", "input").value == "true")
                    {
                        FindControl("txtAltura", "input").value = retorno[10];
                        FindControl("txtLargura", "input").value = retorno[11];
                    }

                    FindControl("hdfCustoProd", "input").value = retorno[14];
                }

                FindControl("lblDescrProd", "span").innerHTML = retorno[2];
            }
            else
            {
                FindControl("hdfAmbIdProd", "input").value = retorno[1];
                FindControl("lblDescrAmb", "span").innerHTML = retorno[2];
                FindControl("hdfDescrAmbiente", "input").value = retorno[2];
            }
        }
        catch (err) {
            alert(err);
        }

        produtoAmbiente = false;
    }

    // Se o produto sendo adicionado for ferragem e se a empresa for charneca, informa se qtd vendida
    // do produto existe no estoque
    function verificaEstoque() {
        if (FindControl("txtQtdeIns", "input") == null)
        {
            return true;
        }

        var txtQtd = FindControl("txtQtdeIns", "input").value;

        if (txtQtd != "" && parseInt(txtQtd) > parseInt(qtdEstoque))
        {
            if (qtdEstoque == 0)
                alert("Não há nenhuma peça deste produto no estoque.");
            else
                alert("Há apenas " + qtdEstoque + " peça(s) deste produto no estoque.");

            FindControl("txtQtdeIns", "input").value = "";
            return false;
        }
    }

    // Função chamada pelo popup de escolha da Aplicação do produto
    function setApl(idAplicacao, codInterno) {
        if (!aplAmbiente && FindControl("txtAplIns", "input") != null && FindControl("hdfIdAplicacao", "input") != null)
        {
            FindControl("txtAplIns", "input").value = codInterno;
            FindControl("hdfIdAplicacao", "input").value = idAplicacao;
        }
        else if (aplAmbiente && FindControl("txtAmbAplIns", "input") != null && FindControl("hdfAmbIdAplicacao", "input") != null)
        {
            FindControl("txtAmbAplIns", "input").value = codInterno;
            FindControl("hdfAmbIdAplicacao", "input").value = idAplicacao;
        }

        aplAmbiente = false;
    }

    function loadApl(codInterno) {
        if (codInterno == undefined || codInterno == "") {
            setApl("", "");
            return false;
        }

        try {
            var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Aplicação. Ajax Error.");
                setApl("", "");
                return false
            }

            response = response.split("\t");

            if (response[0] == "Erro") {
                alert(response[1]);
                setApl("", "");
                return false;
            }

            setApl(response[1], response[2]);
        }
        catch (err) {
            alert(err);
        }
    }

    // Função chamada pelo popup de escolha do Processo do produto
    function setProc(idProcesso, codInterno, codAplicacao) {
        if (!procAmbiente && FindControl("txtProcIns", "input") != null && FindControl("hdfIdProcesso", "input") != null)
        {
            FindControl("txtProcIns", "input").value = codInterno;
            FindControl("hdfIdProcesso", "input").value = idProcesso;
        }
        else if (procAmbiente && FindControl("txtAmbProcIns", "input") != null && FindControl("hdfAmbIdProcesso", "input") != null)
        {
            FindControl("txtAmbProcIns", "input").value = codInterno;
            FindControl("hdfAmbIdProcesso", "input").value = idProcesso;
        }

        if (codAplicacao != "")
        {
            aplAmbiente = procAmbiente;
            loadApl(codAplicacao);
        }

        procAmbiente = false;
    }

    function loadProc(codInterno) {
        if (codInterno == "") {
            setProc("", "", "");
            return false;
        }

        try {
            var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Processo. Ajax Error.");
                setProc("", "");
                return false
            }

            response = response.split("\t");

            if (response[0] == "Erro") {
                alert(response[1]);
                setProc("", "", "");
                return false;
            }

            setProc(response[1], response[2], response[3]);
        }
        catch (err) {
            alert(err);
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
        {
            return true;
        }

        formaPagtoVisibility();

        // Ao alterar o tipo de venda, as formas de pagamento devem ser recarregadas para que o controle de desconto por forma de pagamento e dados do produto funcione corretamente.
        if (<%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>)
        {
            atualizaFormasPagtoCli();
        }

        formaPagtoChanged();

        if (document.getElementById("divObra") != null)
        {
            document.getElementById("divObra").style.display = parseInt(control.value) == 5 ? "" : "none";
        }

        if (document.getElementById("funcionarioComprador") != null)
        {
            document.getElementById("funcionarioComprador").style.display = parseInt(control.value) == 6 ? "" : "none";
        }

        var valorEntrada = document.getElementById("tdValorEntrada2") != null && document.getElementById("tdValorEntrada2").getElementsByTagName("input") != null &&
            document.getElementById("tdValorEntrada2").getElementsByTagName("input")[0] != null ? document.getElementById("tdValorEntrada2").getElementsByTagName("input")[0] : null;

        if (valorEntrada != null)
        {
            valorEntrada.style.display = control.value == 2 ? "" : "none";
            valorEntrada.value = control.value == 2 ? valorEntrada.value : "";
        }

        if (parseInt(control.value) != 6 && FindControl("drpFuncVenda", "select") != null)
            FindControl("drpFuncVenda", "select").value = "";

        if (document.getElementById("divNumParc") != null)
            document.getElementById("divNumParc").style.display = parseInt(control.value) == 2 ? "" : "none";

        setParcelas(calcParcelas);
        if (typeof <%= dtvPedido.ClientID %>_ctrlParcelas1 != "undefined")
            Parc_visibilidadeParcelas("<%= dtvPedido.ClientID %>_ctrlParcelas1");
    }

    function setParcelas(calcParcelas)
    {
        var nomeControleParcelas = "<%= dtvPedido.ClientID %>_ctrlParcelas1";
        if (document.getElementById(nomeControleParcelas + "_tblParcelas") == null)
            return;

        var drpTipoVenda = FindControl("drpTipoVenda", "select");

        if (drpTipoVenda == null)
            return;

        if (FindControl("hdfExibirParcela", "input") != null && FindControl("hdfCalcularParcela", "input") != null)
        {
            FindControl("hdfExibirParcela", "input").value = drpTipoVenda.value == 2;
            FindControl("hdfCalcularParcela", "input").value = calcParcelas;
        }
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

    function onInsert() {
        if (inserting)
            return false;

        // Verifica se o cliente foi selecionado
        if (FindControl("hdfCliente", "input") == null || FindControl("hdfCliente", "input").value == "" || FindControl("hdfCliente", "input").value == null) {
            alert("Informe o cliente.");
            return false;
        }

        // Verifica se o tipo de entrega foi selecionado
        if (FindControl("ddlTipoEntrega", "select") == null || FindControl("ddlTipoEntrega", "select").value == "") {
            alert("Selecione o tipo de entrega.");
            return false;
        }

        if (FindControl("ddlTipoEntrega", "select") == null ||
            FindControl("ddlTipoEntrega", "select").value == 2 ||
            FindControl("ddlTipoEntrega", "select").value == 3 ||
            FindControl("ddlTipoEntrega", "select").value == 5 ||
            FindControl("ddlTipoEntrega", "select").value == 6) {
            if (FindControl("txtEnderecoObra", "input").value == "") {
                alert("Informe o endereço do local da obra.");
                return false;
            }

            if (FindControl("txtBairroObra", "input") == null ||
                FindControl("txtBairroObra", "input").value == "") {
                alert("Informe o bairro do local da obra.");
                return false;
            }

            if (FindControl("txtCidadeObra", "input") == null ||
                FindControl("txtCidadeObra", "input").value == "") {
                alert("Informe a cidade do local da obra.");
                return false;
            }
        }

        // Verifica se a data de entrega é menor que a data atual
        var dataEntrega = FindControl("txtDataEntrega", "input") != null ? FindControl("txtDataEntrega", "input").value : "";

        if (dataEntrega == "") {
            alert("Informe a data de entrega do pedido.");
            return false;
        }

        if (!dateGreaterThenNow(dataEntrega)) {
            alert("A data da entrega não pode ser menor que a data do pedido.");
            return false;
        }

        var drpTipoVenda = FindControl("drpTipoVenda", "select");

        if (drpTipoVenda != null) {
            // Se o cliente for consumidor final, o pedido não pode ser à prazo
            var nomeCliente = FindControl("txtNomeCliente", "input") != null ? FindControl("txtNomeCliente", "input").value.toString().toLowerCase() : "";
            if ((nomeCliente == "consumidor final" || nomeCliente == "20298 - consumidor final") &&
                drpTipoVenda.value == 2) {
                alert("Consumidor final não pode realizar pedido à prazo.");
                return false;
            }

            // Se o tipo venda não for a vista, obra ou funcionário, obriga a selecionar forma de pagto.
            var tipoVenda = parseInt(drpTipoVenda.value);
            var usarControleDescontoFormaPagamentoDadosProduto = <%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>;
            if (FindControl("drpFormaPagto", "select") == null || FindControl("drpFormaPagto", "select").value == "")
            {
                // Caso o controle de desconto por forma de pagamento e dados do produto esteja habilitado e o tipo de venda do pedido seja à vista, obriga o usuário a informar a forma de pagamento.
                if (usarControleDescontoFormaPagamentoDadosProduto && tipoVenda == 1)
                {
                    alert("Selecione a forma de pagamento.");
                    controle.disabled = false;
                    return false;
                }
                else if (tipoVenda != 1 && tipoVenda != 5 && tipoVenda != 6)
                {
                    alert("Selecione a forma de pagamento.");
                    controle.disabled = false;
                    return false;
                }
            }

            // Se a forma de pagamento for cartão à prazo, obriga a informar o tipo de cartão
            if (FindControl("drpFormaPagto", "select") != null && FindControl("drpFormaPagto", "select").value == codCartao && FindControl("drpTipoCartao", "select").value == "" &&
                (tipoVenda == 2 || (usarControleDescontoFormaPagamentoDadosProduto && tipoVenda == 1))) {
                alert("Informe o tipo de cartão.");
                return false;
            }

            if (tipoVenda == 6 && FindControl("drpFuncVenda", "select") != null && FindControl("drpFuncVenda", "select").value == "")
            {
                alert("Selecione o funcionário comprador.");
                return false;
            }
        }

        // Verifica se a obra pertence ao cliente
        var hdfIdObra = FindControl("hdfIdObra", "input");
        if (hdfIdObra != null && hdfIdObra.value != null && hdfIdObra.value != "" && FindControl("txtNumCli", "input") != null) {
            var obraCliente = CadPedido.IsObraCliente(hdfIdObra.value, FindControl("txtNumCli", "input").value).value;
            if (obraCliente != null && obraCliente.toLower() == "false")
            {
                alert("A obra selecionada não pertence ao cliente selecionado.");
                return false;
            }
        }

        if (document.getElementById("load") != null)
        {
            document.getElementById("load").style.display = "";
        }

        inserting = true;
        return true;
    }

    // Acionado quando o pedido está para ser salvo
    function onSave() {
        // Verifica se o cliente foi selecionado
        if (FindControl("hdfCliente", "input") == null || FindControl("hdfCliente", "input").value == "" || FindControl("hdfCliente", "input").value == null)
        {
            alert("Informe o cliente.");
            return false;
        }

        var tipoVendaAtual = FindControl("hdfTipoVendaAtual", "input");
        var drpTipoVenda = FindControl("drpTipoVenda", "select");

        if (drpTipoVenda != null)
        {
            // Se o cliente for consumidor final, o pedido não pode ser à prazo
            var nomeCliente = FindControl("txtNomeCliente", "input") != null ? FindControl("txtNomeCliente", "input").value.toString().toLowerCase() : "";
            if ((nomeCliente == "consumidor final" || nomeCliente == "20298 - consumidor final") &&
                FindControl("drpTipoVenda", "select").value == 2) {
                alert("Consumidor final não pode realizar pedido à prazo.");
                return false;
            }

            // Se o tipo venda não for a vista, obra ou funcionário, obriga a selecionar forma de pagto.
            var tipoVenda = parseInt(drpTipoVenda.value);
            var usarControleDescontoFormaPagamentoDadosProduto = <%= Glass.Configuracoes.FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto.ToString().ToLower() %>;
            if (FindControl("drpFormaPagto", "select") == null || FindControl("drpFormaPagto", "select").value == "")
            {
                // Caso o controle de desconto por forma de pagamento e dados do produto esteja habilitado e o tipo de venda do pedido seja à vista, obriga o usuário a informar a forma de pagamento.
                if (usarControleDescontoFormaPagamentoDadosProduto && tipoVenda == 1)
                {
                    alert("Selecione a forma de pagamento.");
                    controle.disabled = false;
                    return false;
                }
                else if (tipoVenda != 1 && tipoVenda != 5 && tipoVenda != 6)
                {
                    alert("Selecione a forma de pagamento.");
                    controle.disabled = false;
                    return false;
                }
            }

            // Se a forma de pagamento for cartão à prazo, obriga a informar o tipo de cartão
            if (FindControl("drpFormaPagto", "select") != null && FindControl("drpFormaPagto", "select").value == codCartao && FindControl("drpTipoCartao", "select") != null &&
                FindControl("drpTipoCartao", "select").value == "" && (tipoVenda == 2 || (usarControleDescontoFormaPagamentoDadosProduto && tipoVenda == 1))) {
                alert("Informe o tipo de cartão.");
                return false;
            }

            if (tipoVenda == 6 && FindControl("drpFuncVenda", "select") != null && FindControl("drpFuncVenda", "select").value == "")
            {
                alert("Selecione o funcionário comprador.");
                return false;
            }

            var numeroProdutos = <%= GetNumeroProdutosPedido() %>;
            if (tipoVendaAtual.value != 5 && tipoVenda == 5 && numeroProdutos > 0)
            {
                alert("Não é possível escolher obra como forma de pagamento se o pedido tiver algum produto cadastrado.");
                return false;
            }
            else if (tipoVendaAtual.value == 5 && tipoVenda != 5 && numeroProdutos > 0)
            {
                alert("Não é possível que a forma de pagamento do pedido não seja mais obra se houver algum produto cadastrado.");
                return false;
            }
        }

        var dataEntrega = FindControl("txtDataEntrega", "input");
        var dataPedido = FindControl("txtDataPed", "input");

        // Verifica se a data de entrega foi preenchida
        if (dataEntrega.value == "") {
            alert("Informe a data de entrega.");
            return false;
        }

        if (FindControl("ddlTipoEntrega", "select") == null ||
            FindControl("ddlTipoEntrega", "select").value == 2 ||
            FindControl("ddlTipoEntrega", "select").value == 3 ||
            FindControl("ddlTipoEntrega", "select").value == 5 ||
            FindControl("ddlTipoEntrega", "select").value == 6) {

            if (FindControl("txtEnderecoObra", "input") == null || FindControl("txtEnderecoObra", "input").value == "") {
                alert("Informe o endereço do local da obra.");
                return false;
            }

            if (FindControl("txtBairroObra", "input") == null || FindControl("txtBairroObra", "input").value == "") {
                alert("Informe o bairro do local da obra.");
                return false;
            }

            if (FindControl("txtCidadeObra", "input") == null || FindControl("txtCidadeObra", "input").value == "") {
                alert("Informe a cidade do local da obra.");
                return false;
            }
        }
        else if (FindControl("ddlTipoEntrega", "select") == null || FindControl("ddlTipoEntrega", "select").value == 4) {
            if (FindControl("txtEnderecoObra", "input") == null || FindControl("txtEnderecoObra", "input").value == "") {
                alert("Informe o endereço da entrega.");
                return false;
            }

            if (FindControl("txtBairroObra", "input") == null || FindControl("txtBairroObra", "input").value == "") {
                alert("Informe o bairro da entrega.");
                return false;
            }

            if (FindControl("txtCidadeObra", "input") == null || FindControl("txtCidadeObra", "input").value == "") {
                alert("Informe a cidade da entrega.");
                return false;
            }
        }

        // Verifica se a data de entrega é menor que a data do pedido
        if (dataEntrega != null && dataEntrega.value != "" && firstGreaterThenSec(dataPedido.value, dataEntrega.value)) {
            alert("A data da entrega não pode ser menor que a data do pedido.");
            return false;
        }

        // Verifica se a obra pertence ao cliente
        var hdfIdObra = FindControl("hdfIdObra", "input");
        if (hdfIdObra != null && hdfIdObra.value != null && hdfIdObra.value != "" && FindControl("txtNumCli", "input") != null) {
            var obraCliente = CadPedido.IsObraCliente(hdfIdObra.value, FindControl("txtNumCli", "input").value).value;
            if (obraCliente != null && obraCliente.toLowerCase() == "false")
            {
                alert("A obra selecionada não pertence ao cliente selecionado.");
                return false;
            }
        }

        // Verifica se o cliente foi alterado
        var clienteAtual = FindControl("hdfClienteAtual", "input") != null ? parseInt(FindControl("hdfClienteAtual", "input").value, 10) : 0;
        var clienteNovo = FindControl("txtNumCli", "input") != null ? parseInt(FindControl("txtNumCli", "input").value, 10) : 0;
        var alterar = clienteAtual != clienteNovo ? confirm("O cliente foi alterado no pedido. Deseja atualizar o projeto?") : false;

        if (FindControl("hdfAlterarProjeto", "input") != null)
        {
            FindControl("hdfAlterarProjeto", "input").value = alterar;
        }

        // Verifica o prazo e a urgência do pedido
        return verificarDatas();
    }

    // Chamado quando um produto está para ser inserido no pedido
    function onSaveProd() {
        if (!validate("produto"))
        {
            return false;
        }

        atualizaValMin();

        var codProd = FindControl("txtCodProdIns", "input") != null ? FindControl("txtCodProdIns", "input").value : "";
        var valor = FindControl("txtValorIns", "input") != null ? FindControl("txtValorIns", "input").value : "";
        var qtde = FindControl("txtQtdeIns", "input") != null ? FindControl("txtQtdeIns", "input").value : "";
        var altura = FindControl("txtAlturaIns", "input") != null ? FindControl("txtAlturaIns", "input").value : "";
        var largura = FindControl("txtLarguraIns", "input") != null ? FindControl("txtLarguraIns", "input").value : "";
        var valMin = FindControl("hdfValMin", "input") != null ? FindControl("hdfValMin", "input").value : "";

        if (codProd == "") {
            alert("Informe o código do produto.");
            return false;
        }

        if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
            alert("Informe o valor vendido.");
            return false;
        }

        if (qtde == "0" || qtde == "") {
            alert("Informe a quantidade.");
            return false;
        }

        valMin = new Number(valMin.replace(',', '.'));
        if (new Number(valor.replace(',', '.')) < valMin) {
            alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
            return false;
        }

        if (FindControl("txtAlturaIns", "input") != null && FindControl("txtAlturaIns", "input").disabled == false) {
            if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                alert("Informe a altura.");
                return false;
            }
        }

        // Se o textbox da largura estiver habilitado, deverá ser informada
        if (FindControl("txtLarguraIns", "input") != null && FindControl("txtLarguraIns", "input").disabled == false && largura == "") {
            alert("Informe a largura.");
            return false;
        }

        if (!obrigarProcApl())
        {
            return false;
        }

        if (!validaTamanhoMax())
        {
            return false;
        }

        // Calcula o ICMS do produto
        var aliquota = FindControl("hdfAliquotaIcmsProd", "input");
        var icms = FindControl("hdfValorIcmsProd", "input");

        if (icms != null)
        {
            icms.value = aliquota != null && parseFloat(aliquota.value) > 0 ? parseFloat(valor) * (parseFloat(aliquota.value) / 100) : 0;
        }

        // Faz verificações do beneficiamento
        //if (!checkBenef(FindControl("txtEspessura", "input").value))
        //    return false;

        // Verifica o prazo e a urgência do pedido
        if (FindControl("hdfIsVidro", "input") != null && FindControl("hdfIsVidro", "input").value == "true" && !verificarDatas())
        {
            return false;
        }

        if (FindControl("txtEspessura", "input") != null)
        {
            FindControl("txtEspessura", "input").disabled = false;
        }

        if (FindControl("txtAlturaIns", "input") != null)
        {
            FindControl("txtAlturaIns", "input").disabled = false;
        }

        if (FindControl("txtLarguraIns", "input") != null)
        {
            FindControl("txtLarguraIns", "input").disabled = false;
        }

        if (FindControl("txtValorIns", "input") != null)
        {
            FindControl("txtValorIns", "input").disabled = false;
        }

        // Verifica o prazo e a urgência do pedido
        if (FindControl("hdfIsVidro", "input") != null && FindControl("hdfIsVidro", "input").value == "true")
        {
            return verificarDatas();
        }
        else
        {
            return true;
        }
    }

    // Função chamada quando o produto está para ser atualizado
    function onUpdateProd() {
        if (!validate("produto"))
        {
            return false;
        }

        atualizaValMin();

        var valor = FindControl("txtValorIns", "input") != null ? FindControl("txtValorIns", "input").value : "";
        var qtde = FindControl("txtQtdeIns", "input") != null ? FindControl("txtQtdeIns", "input").value : "";
        var altura = FindControl("txtAlturaIns", "input") != null ? FindControl("txtAlturaIns", "input").value : "";
        var idProd = FindControl("hdfIdProd", "input") != null ? FindControl("hdfIdProd", "input").value : "";
        var codInterno = FindControl("hdfCodInterno", "input") != null ? FindControl("hdfCodInterno", "input").value : "";
        var valMin = FindControl("hdfValMin", "input") != null ? FindControl("hdfValMin", "input").value : "";

        valMin = new Number(valMin.replace(',', '.'));
        if (new Number(valor.replace(',', '.')) < valMin) {
            alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
            return false;
        }

        if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
            alert("Informe o valor vendido.");
            return false;
        }
        else if (qtde == "0" || qtde == "") {
            alert("Informe a quantidade.");
            return false;
        }
        else if (FindControl("txtAlturaIns", "input") != null && FindControl("txtAlturaIns", "input").disabled == false) {
            if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                alert("Informe a altura.");
                return false;
            }
        }

        if (!obrigarProcApl())
        {
            return false;
        }

        if (!validaTamanhoMax())
        {
            return false;
        }

        // Calcula o ICMS do produto
        var aliquota = FindControl("hdfAliquotaIcmsProd", "input");
        var icms = FindControl("hdfValorIcmsProd", "input");

        if (icms != null)
        {
            icms.value = aliquota != null && parseFloat(aliquota.value) > 0 ? parseFloat(valor) * (parseFloat(aliquota.value) / 100) : 0;
        }

        // Faz verificações do beneficiamento
        //if (!checkBenef(FindControl("txtEspessura", "input").value))
        //    return false;

        if (FindControl("txtEspessura", "input") != null)
        {
            FindControl("txtEspessura", "input").disabled = false;
        }

        if (FindControl("txtAlturaIns", "input") != null)
        {
            FindControl("txtAlturaIns", "input").disabled = false;
        }

        if (FindControl("txtLarguraIns", "input") != null)
        {
            FindControl("txtLarguraIns", "input").disabled = false;
        }

        if (FindControl("txtValorIns", "input") != null)
        {
            FindControl("txtValorIns", "input").disabled = false;
        }

        // Verifica o prazo e a urgência do pedido
        if (FindControl("hdfIsVidro", "input") != null && FindControl("hdfIsVidro", "input").value == "true")
        {
            return verificarDatas();
        }
        else
        {
            return true;
        }
    }

    var valorTabelaProduto = null;

    function GetAdicionalAlturaChapa(){
        var idProd = FindControl("hdfIdProd", "input").value;
        var altura = FindControl("txtAlturaIns", "input").value;
        var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
        var idCliente = FindControl("hdfIdCliente", "input").value;
        var cliRevenda = FindControl("hdfCliRevenda", "input").value;
        var reposicao = FindControl("hdfIsReposicao", "input").value;
        var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
        controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
        var percDescontoQtde = controleDescQtde.PercDesconto();

        FindControl("txtValorIns", "input").value = MetodosAjax.GetValorTabelaProduto(idProd, tipoEntrega, idCliente, cliRevenda,
                    reposicao, percDescontoQtde, Conversoes.StrParaInt(Request["idPedido"]), "", "").value.replace(".", ",");
    }

    // Função chamada ao clicar no botão Em Conferência
    function emConferencia() {
        if (confirm("Mudar pedido para em conferência?") == false)
        {
            return false;
        }

        var entrada = FindControl("ctrValEntrada_txtNumber", "input") != null ? FindControl("ctrValEntrada_txtNumber", "input").value : 0;
        var totalPedido = FindControl("hdfTotal", "input") != null ? FindControl("hdfTotal", "input").value : 0;

        if (!confirm("O sinal não foi inserido, clique em 'Cancelar' para inserir o sinal do pedido ou em 'OK' para continuar."))
        {
            return false;
        }

        if (totalPedido == 0 || totalPedido == "" || totalPedido == "0")
        {
            alert("O pedido não possui valor total, insira um produto 'Conferência' com o valor total do Pedido.");
            return false;
        }

        return false;
    }

    // Calcula em tempo real a metragem quadrada do produto
    function calcM2Prod() {
        try {
            var idProd = FindControl("hdfIdProd", "input").value;
            var altura = FindControl("txtAlturaIns", "input").value;
            var largura = FindControl("txtLarguraIns", "input").value;

            var qtde = FindControl("txtQtdeIns", "input").value;
            var qtdeAmb = new Number(parseInt(FindControl("hdfQtdeAmbiente", "input").value, 10) > 0 ? FindControl("hdfQtdeAmbiente", "input").value : "1");
            var isVidro = FindControl("hdfIsVidro", "input").value == "true";
            var tipoCalc = FindControl("hdfTipoCalc", "input").value;
            var esp = FindControl("txtEspessura", "input") != null ? FindControl("txtEspessura", "input").value : 0;
            // Calcula metro quadrado
            var idCliente = FindControl("hdfIdCliente", "input").value;

            var redondo = (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked) ||
                (FindControl("hdfRedondoAmbiente", "input").value.toLowerCase() == "true");

            if (altura != "" && largura != "" &&
                parseInt(altura) > 0 && parseInt(largura) > 0 &&
                parseInt(altura) != parseInt(largura) && redondo) {
                alert('O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.');

                if (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked)
                    FindControl("Redondo_chkSelecao", "input").checked = false;

                FindControl("hdfRedondoAmbiente", "input").value = false;

                return false;
            }

            var numBenef = "";

            if (FindControl("Redondo_chkSelecao", "input") != null) {
                numBenef = FindControl("Redondo_chkSelecao", "input").id;
                numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
                numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
                numBenef = eval(numBenef).NumeroBeneficiamentos();
            }

            if (altura == "" || largura == "" || qtde == "" || altura == "0" || (tipoCalc != 2 && tipoCalc != 10)) {
                if (altura > 0 && largura > 0 && qtde > 0 && isVidro) {
                    FindControl("lblTotM2Ins", "span").innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, "false").value;
                    FindControl("hdfTotM2Calc", "input").value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde * qtdeAmb, idProd, redondo, esp, numBenef, "false").value;
                    FindControl("hdfTotM2CalcSemChapa", "input").value = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
                    FindControl("lblTotM2Calc", "span").innerHTML = FindControl("hdfTotM2Calc", "input").value.replace('.', ',');
                }

                if (qtde != "" && qtde != "0")
                    calcTotalProd();

                return false;
            }

            FindControl("lblTotM2Ins", "span").innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, "false").value;
            FindControl("hdfTotM2Calc", "input").value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde * qtdeAmb, idProd, redondo, esp, numBenef, "false").value;
            FindControl("hdfTotM2CalcSemChapa", "input").value = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
            FindControl("lblTotM2Calc", "span").innerHTML = FindControl("hdfTotM2Calc", "input").value.replace('.', ',');

            calcTotalProd();
        }
        catch (err) {
            alert("Não foi possível calcular o total de M² do produto. Erro: " + err);
            return false;
        }
    }

    // Calcula em tempo real o valor total do produto
    function calcTotalProd() {
        try {
            var valorIns = FindControl("txtValorIns", "input") != null ? FindControl("txtValorIns", "input").value : "";

            if (valorIns == "")
            {
                return true;
            }

            if (FindControl("lblTotM2Ins", "span") == null || FindControl("hdfTotM2Calc", "input") == null || FindControl("txtQtdeIns", "input") == null ||
                FindControl("hdfQtdeAmbiente", "input") == null || FindControl("txtAlturaIns", "input") == null || FindControl("txtLarguraIns", "input") == null ||
                FindControl("hdfTipoCalc", "input") == null || FindControl("hdfM2Minimo", "input") == null)
            {
                return true;
            }


            var totM2 = FindControl("lblTotM2Ins", "span").innerHTML;
            var totM2Calc = new Number(FindControl("hdfTotM2Calc", "input").value.replace(',', '.')).toFixed(2);
            var total = new Number(valorIns.replace(',', '.')).toFixed(2);
            var qtde = new Number(FindControl("txtQtdeIns", "input").value.replace(',', '.'));
            qtde = qtde * new Number(parseInt(FindControl("hdfQtdeAmbiente", "input").value, 10) > 0 ? FindControl("hdfQtdeAmbiente", "input").value : "1");
            var altura = new Number(FindControl("txtAlturaIns", "input").value.replace(',', '.'));
            var largura = new Number(FindControl("txtLarguraIns", "input").value.replace(',', '.'));
            var tipoCalc = FindControl("hdfTipoCalc", "input").value;
            var m2Minimo = FindControl("hdfM2Minimo", "input").value;

            var alturaBenef = FindControl("drpAltBenef", "select");
            alturaBenef = alturaBenef != null ? alturaBenef.value : "0";

            var larguraBenef = FindControl("drpLargBenef", "select");
            larguraBenef = larguraBenef != null ? larguraBenef.value : "0";

            var controleDescQtde = FindControl("_divDescontoQtde", "div") != null ? FindControl("_divDescontoQtde", "div").id : null;

            if (controleDescQtde != null)
            {
                controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
            }

            var percDesconto = controleDescQtde != null ? controleDescQtde.PercDesconto() : 0;
            var percDescontoAtual = controleDescQtde != null ? controleDescQtde.PercDescontoAtual() : 0;

            var retorno = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, FindControl("txtAlturaIns", "input"), largura, true, tipoCalc, alturaBenef, larguraBenef, percDescontoAtual, percDesconto);

            if (retorno != "")
            {
                FindControl("lblTotalIns", "span").innerHTML = retorno;
            }
        }
        catch (err) {
            alert("Não foi possível calcular o valor total do produto. Erro: " + err);
            return false;
        }
    }

    function getCli(idCli)
    {
        var usarComissionado = <%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente.ToString().ToLower() %>;

        var retorno = CadPedido.GetCli(idCli.value).value.split(';');
        if (retorno[0] == "Erro")
        {
            alert(retorno[1]);
            idCli.value = "";

            if (FindControl("txtNomeCliente", "input") != null)
            {
                FindControl("txtNomeCliente", "input").value = "";
            }

            if (FindControl("hdfCliente", "input") != null)
            {
                FindControl("hdfCliente", "input").value = "";
            }

            if (usarComissionado)
            {
                limparComissionado();
            }

            return false;
        }

        if (FindControl("txtNomeCliente", "input") != null)
        {
            FindControl("txtNomeCliente", "input").value = retorno[1];
        }

        if (FindControl("hdfCliente", "input") != null)
        {
            FindControl("hdfCliente", "input").value = idCli.value;
        }

        if (FindControl("lblObsCliente", "span") != null)
        {
            FindControl("lblObsCliente", "span").innerHTML = retorno[3];
        }

        if (usarComissionado)
        {
            var comissionado = MetodosAjax.GetComissionado("", idCli.value).value.split(';');
            setComissionado(comissionado[0], comissionado[1], comissionado[2]);
        }
    }

    // Habilita/Desabilita campos referente ao local da obra
    function setLocalObra() {
        var cTipoEntrega = FindControl("ddlTipoEntrega", "select");

        if (cTipoEntrega == null || !cTipoEntrega)
        {
            return false;
        }

        var disable = cTipoEntrega.value != 2 && cTipoEntrega.value != 3 && cTipoEntrega.value != 4 && cTipoEntrega.value != 5 && cTipoEntrega.value != 6;

        var cEndereco = FindControl("txtEnderecoObra", "input");
        var cBairro = FindControl("txtBairroObra", "input");
        var cCidade = FindControl("txtCidadeObra", "input");

        if (cEndereco == null || cBairro == null || cCidade == null)
        {
            return false;
        }

        // Se os campos estiverem sendo desabilitados, apaga seus valores
        if (disable) {
            cEndereco.value = "";
            cBairro.value = "";
            cCidade.value = "";
        }

        // Habilita ou desabilita os campos
        cEndereco.disabled = disable;
        cBairro.disabled = disable;
        cCidade.disabled = disable;
    }

    // Busca o endereço do cliente
    function getEnderecoCli() {
        if (FindControl("txtEnderecoObra", "input") != null && FindControl("txtEnderecoObra", "input").disabled)
        {
            return false;
        }

        var idCli = FindControl("hdfCliente", "input") != null ? FindControl("hdfCliente", "input").value : "";

        if (idCli == "")
        {
            alert("Selecione um cliente primeiro.");
            return false;
        }

        var retorno = MetodosAjax.GetEnderecoCli(idCli).value;

        if (retorno != null && retorno != "" && FindControl("txtEnderecoObra", "input") != null && FindControl("txtBairroObra", "input") != null &&
            FindControl("txtCidadeObra", "input") != null)
        {
            retorno = retorno.split('|');
            FindControl("txtEnderecoObra", "input").value = retorno[0];
            FindControl("txtBairroObra", "input").value = retorno[1];
            FindControl("txtCidadeObra", "input").value = retorno[2];
        }
    }

    function setComissionado(id, nome, percentual) {
        if (FindControl("lblComissionado", "span") != null && FindControl("hdfIdComissionado", "input") != null && FindControl("txtPercentual", "input") != null)
        {
            FindControl("lblComissionado", "span").innerHTML = nome;
            FindControl("hdfIdComissionado", "input").value = id;
            FindControl("txtPercentual", "input").value = percentual;
        }
    }

    // Função chamada para mostrar/esconder controles para inserção de novo ambiente
    function addAmbiente(value) {
        var ambiente = FindControl("txtAmbiente", "input");
        if (ambiente == null && FindControl("ambMaoObra", "div") != null)
        {
            ambiente = FindControl("ambMaoObra", "div");
        }

        var descricao = FindControl("txtDescricao", "textarea");

        if (ambiente == null && descricao == null)
        {
            return true;
        }

        if (descricao != null)
        {
            descricao.style.display = value ? "" : "none";
        }

        if (ambiente != null)
        {
            ambiente.style.display = value ? "" : "none";
        }

        var qtde = FindControl("txtQtdeAmbiente", "input");
        var altura = FindControl("txtAlturaAmbiente", "input");
        var largura = FindControl("txtLarguraAmbiente", "input");
        var redondo = FindControl("chkRedondoAmbiente", "input");

        var apl = FindControl("txtAmbAplIns", "input");
        apl = apl != null && apl.parentNode != null && apl.parentNode.parentNode != null ? apl.parentNode.parentNode.parentNode : null;

        var proc = FindControl("txtAmbProcIns", "input");
        proc = proc != null && proc.parentNode != null && proc.parentNode.parentNode != null ? proc.parentNode.parentNode.parentNode : null;

        if (qtde != null)
        {
            qtde.style.display = value ? "" : "none";
        }

        if (altura != null)
        {
            altura.style.display = value ? "" : "none";
        }

        if (largura != null)
        {
            largura.style.display = value ? "" : "none";
        }

        if (redondo != null)
        {
            if (value)
            {
                redondo.style.display = "";

                if (altura.value != "" && largura != "" && altura.value != largura.value && redondo.checked)
                {
                    alert('O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.');
                    redondo.checked = false;
                }
            }
            else
            {
                redondo.style.display = "none";
            }
        }

        if (apl != null)
        {
            apl.style.display = value ? "" : "none";
        }

        if (proc != null)
        {
            proc.style.display = value ? "" : "none";
        }

        if (FindControl("lnkInsAmbiente", "a") != null)
        {
            FindControl("lnkInsAmbiente", "a").style.display = value ? "" : "none";
        }
    }

    // Função chamada ao finalizar o pedido
    function finalizarPedido()
    {
        if (confirm("Finalizar pedido?"))
        {
            return verificarDatas();
        }

        return false;
    }

    // Função chamada para verificar o prazo de entrega e a urgência do pedido
    function verificarDatas()
    {
        // Verifica a data de entrega
        var dataEntrega = FindControl("txtDataEntrega", "input");
        if (dataEntrega != null && !verificaDataEntrega(dataEntrega))
            return false;

        // Variáveis de verificação da necessidade do método
        var isFastDelivery = <%= IsFastDelivery() %>;
        var isTemperaFora = <%= Glass.Configuracoes.PedidoConfig.TamanhoVidro.UsarTamanhoMaximoVidro.ToString().ToLower() %>;

        var pedidoFastDelivery = null;
        var pedidoTemperaFora = null;

        // Verifica se o pedido é Fast Delivery
        if (isFastDelivery)
        {
            pedidoFastDelivery = FindControl("hdfFastDelivery", "input");
            if (pedidoFastDelivery != null)
            {
                pedidoFastDelivery = pedidoFastDelivery.value.toLowerCase() == "true";
            }
            else
            {
                pedidoFastDelivery = FindControl("chkFastDelivery", "input");
                if (pedidoFastDelivery != null)
                {
                    pedidoFastDelivery = pedidoFastDelivery.checked;
                }
                else
                {
                    pedidoFastDelivery = false;
                }
            }
        }
        else
            pedidoFastDelivery = false;

        // Verifica se o pedido pode ser têmpera fora
        if (isTemperaFora)
        {
            // Verifica se o pedido é têmpera fora
            var temperaFora = FindControl("hdfTemperaFora", "input");
            if (temperaFora == null)
            {
                var temperaFora = FindControl("chkTemperaFora", "input");
                if (temperaFora != null)
                {
                    editPedido = true;
                    pedidoTemperaFora = temperaFora.checked;
                }
            }
            else
                pedidoTemperaFora = temperaFora.value.toLowerCase() == "true";
        }
        else
            pedidoTemperaFora = false;

        // Valida a Têmpera fora
        if (isTemperaFora && !checkTemperaFora(pedidoFastDelivery))
            return false;

        // Só testa o Fast Delivery e o Máximo de Vendas se o pedido não for Têmpera fora
        if (!pedidoTemperaFora)
        {
            // Valida o Fast Delivery
            if (pedidoFastDelivery)
            {
                if (isFastDelivery && !checkFastDelivery())
                    return false;
            }
        }

        return true;
    }

    // Função chamada para verificar se há Fast Delivery.
    function checkFastDelivery()
    {
        var editPedido = false;

        var fastDelivery = FindControl("hdfFastDelivery", "input");
        if (fastDelivery == null || fastDelivery.value.toLowerCase() == "false")
        {
            var fastDelivery = FindControl("chkFastDelivery", "input");
            if (fastDelivery == null || !fastDelivery.checked)
                return true;
            else
                editPedido = true;
        }

        var totM2 = parseFloat("<%= GetTotalM2Pedido() %>");
        var dataPedido = "<%= GetDataPedido() %>";
        var diferencaM2 = 0;

        if (!editPedido)
        {
            if (FindControl("drpFooterVisible", "select") != null)
            {
                if (FindControl("lblTotM2Ins", "span") != null)
                {
                    diferencaM2 = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.'));
                }
            }
            else
            {
                var totM2Produto = FindControl("hdfTotM", "input") != null ? parseFloat(FindControl("hdfTotM", "input").value.replace(',', '.')) : 0;
                var novoTotM2Produto = FindControl("lblTotM2Ins", "span") != null ? parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.')) : 0;
                diferencaM2 = novoTotM2Produto - totM2Produto;
            }
        }

        var codInternoProd = !editPedido ? FindControl("txtCodProdIns", "input") : null;
        if (codInternoProd == null)
        {
            codInternoProd = !editPedido ? FindControl("lblCodProdIns", "span") : null;
        }

        if (codInternoProd != null)
        {
            codInternoProd = codInternoProd.nodeName.toLowerCase() == "input" ? codInternoProd.value : codInternoProd.innerHTML;
        }
        else
        {
            codInternoProd = "";
        }

        if (isNaN(diferencaM2) || (!editPedido && !CadPedido.UsarDiferencaM2Prod(codInternoProd).value))
        {
            diferencaM2 = 0;
        }

        var dataEntrega = editPedido && FindControl("txtDataEntrega", "input") != null ? FindControl("txtDataEntrega", "input").value :
            FindControl("lblDataEntrega", "span") != null ? FindControl("lblDataEntrega", "span").innerHTML : "";
        var resposta = CadPedido.CheckFastDelivery(idPedido, dataEntrega, diferencaM2).value;
        var dadosResposta = resposta.split("|");

        if (dadosResposta[0] == "Erro")
        {
            alert(dadosResposta[1]);
            return false;
        }

        if (dadosResposta[1] != "true")
        {
            var diasEntrega = <%= GetPrazoEntregaFastDelivery() %>;
            var textoPergunta = "A data de entrega não pode ser " + dataEntrega + " porque para esse dia já há " +
                dadosResposta[2] + "m² para serem entregues e o pedido possui " + dadosResposta[3] + "m².\nO próximo dia disponível para entrega é " +
                dadosResposta[4] + ".\n\nDeseja alterar a data de entrega para esse dia?";

            if (!confirm(textoPergunta))
                return false;

            if (!editPedido)
            {
                var resposta = CadPedido.AtualizarFastDelivery('<%= Request["IdPedido"] %>', dadosResposta[4]).value;
                var dadosResposta = resposta.split("|");

                if (dadosResposta[0] == "Erro")
                {
                    alert(dadosResposta[1]);
                    return false;
                }
            }
            else
            {
                var txtDataEntrega = FindControl("txtDataEntrega", "input");

                if (txtDataEntrega != null)
                {
                    txtDataEntrega.value = dadosResposta[4];
                }
            }
        }

        return true;
    }

    // Função chamada para verificar se o produto é para têmpera fora
    function checkTemperaFora(isFastDelivery)
    {
        var editPedido = false;
        var isTemperaFora = null;

        var temperaFora = FindControl("hdfTemperaFora", "input");
        if (temperaFora == null)
        {
            var temperaFora = FindControl("chkTemperaFora", "input");
            if (temperaFora != null)
            {
                editPedido = true;
                isTemperaFora = temperaFora.checked;
            }
        }
        else
            isTemperaFora = temperaFora.value.toLowerCase() == "true";

        if (isTemperaFora == null)
            return true;

        if (isTemperaFora)
        {
            if (isFastDelivery)
            {
                alert("Para marcar um pedido para fazer têmpera fora da empresa não é possível marcá-lo como Fast Delivery.");
                return false;
            }
        }
        else if (!editPedido && FindControl("hdfIsVidro", "input") != null && FindControl("hdfIsVidro", "input").value == "true")
        {
            var alturaMaxima = <%= Glass.Configuracoes.PedidoConfig.TamanhoVidro.AlturaMaximaVidro.ToString().Replace(',', '.') %>;
            var larguraMaxima = <%= Glass.Configuracoes.PedidoConfig.TamanhoVidro.LarguraMaximaVidro.ToString().Replace(',', '.') %>;

            var altura = FindControl("txtAlturaIns", "input") != null ? parseFloat(FindControl("txtAlturaIns", "input").value.replace(',', '.')) : 0;
            var largura = FindControl("txtLarguraIns", "input") != null ? parseFloat(FindControl("txtLarguraIns", "input").value.replace(',', '.')) : 0;

            if (altura > alturaMaxima)
            {
                if (altura > larguraMaxima || largura > alturaMaxima)
                {
                    alert("Altura não pode ser maior que " + alturaMaxima + ".");
                    return false;
                }
            }

            if (largura > larguraMaxima)
            {
                if (altura > larguraMaxima || largura > alturaMaxima)
                {
                    alert("Largura não pode ser maior que " + larguraMaxima + ".");
                    return false;
                }
            }
        }

        return true;
    }

    // Função utilizada após selecionar medidor no popup, para preencher o id e o nome do mesmo
    // Nas respectivas textboxes deste form
    function setMedidor(id, nome) {
        if (FindControl("hdfIdMedidor", "input") != null && FindControl("lblMedidor", "span") != null)
        {
            FindControl("hdfIdMedidor", "input").value = id;
            FindControl("lblMedidor", "span").innerHTML = nome;
        }

        return false;
    }

    function openProjeto(idAmbiente)
    {
        var tipoEntrega = FindControl("ddlTipoEntrega", "select");
        if (tipoEntrega != null)
        {
            tipoEntrega = tipoEntrega.value;
        }
        else
        {
            tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
        }

        if (tipoEntrega == "")
        {
            alert("Selecione o tipo de entrega antes de inserir um projeto.");
            return false;
        }

        var idCliente = FindControl("hdfIdCliente", "input") != null ? FindControl("hdfIdCliente", "input").value : 0;

        openWindow(screen.height, screen.width, '../Cadastros/Projeto/CadProjetoAvulso.aspx?IdPedido=<%= Request["IdPedido"] %>' +
            "&IdAmbientePedido=" + idAmbiente + "&idCliente=" + idCliente + "&TipoEntrega=" + tipoEntrega + "&Parceiro=true");

        return false;
    }

    function refreshPage() {
        redirectUrl(window.location.href + "&ref" + Math.random() + "=1");
    }

    </script>

    <table>
        <tr>
            <td>
                <table>
                    <tr>
                        <td align="center">
                            <asp:DetailsView ID="dtvPedido" runat="server" AutoGenerateRows="False" DataSourceID="odsPedido"
                                DefaultMode="Insert" GridLines="None" Height="50px" Width="125px">
                                <Fields>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                            <table cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cliente
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeydown="if (isEnter(event)) getCli(this);"
                                                            onkeypress="return soNumeros(event, true, true);" onblur="getCli(this);" ReadOnly='<%# !(bool)Eval("ClienteEnabled") %>'
                                                            Text='<%# Eval("IdCli") %>'></asp:TextBox>
                                                        <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Text='<%# Eval("NomeCli") %>'
                                                            Width="250px"></asp:TextBox>
                                                        <asp:LinkButton ID="lnkSelCliente" runat="server" Visible='<%# Eval("ClienteEnabled") %>'
                                                            OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=pedido'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                        <br />
                                                        <asp:Label ID="lblObsCliente" runat="server" Text=""></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Ped.&nbsp;
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataPed" runat="server" ReadOnly="True" Text='<%# Eval("DataPedidoString") %>'
                                                            Width="70px"></asp:TextBox>
                                                        <asp:CheckBox ID="chkFastDelivery" runat="server" Checked='<%# Bind("FastDelivery") %>'
                                                            OnLoad="FastDelivery_Load" Text="Fast delivery" onclick="alteraFastDelivery(this.checked)" />
                                                        &nbsp;<asp:CheckBox ID="chkTemperaFora" runat="server" OnLoad="TemperaFora_Load"
                                                            Text="Têmpera fora" onclick="alteraTemperaFora(this.checked)" Checked='<%# Bind("TemperaFora") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cód. Ped. Cli.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtCodPedCli" runat="server" MaxLength="30" Text='<%# Bind("CodCliente") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Orcamento
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIdOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("IdOrcamento") %>' Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdTipoVenda1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Venda
                                                    </td>
                                                    <td id="tdTipoVenda2" align="left" nowrap="nowrap" valign="middle">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoVenda" runat="server" SelectedValue='<%# Bind("TipoVenda") %>'
                                                                        onchange="tipoVendaChange(this, true);" Enabled='<%# !(bool)Eval("RecebeuSinal") %>'
                                                                        DataSourceID="odsTipoVenda" DataTextField="Descr" DataValueField="Id">
                                                                    </asp:DropDownList>
                                                                    <div id="divObra" style="display: none">
                                                                        <asp:TextBox ID="txtObra" runat="server" Enabled="false" Width="200px" Text='<%# Eval("DescrObra") %>'></asp:TextBox>
                                                                        <asp:ImageButton ID="imbObra" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(560, 650, '../Utils/SelObra.aspx?Situacao=4&IdCliente=' + FindControl('txtNumCli', 'input').value); return false;" />
                                                                        <br />
                                                                        Saldo da obra:
                                                                        <asp:Label ID="lblSaldoObra" runat="server" Text='<%# Eval("SaldoObra", "{0:C}") %>'></asp:Label>
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
                                                                                    <asp:DropDownList ID="drpNumParc" runat="server" SelectedValue='<%# Bind("NumParc") %>'
                                                                                        onchange="setParcelas(true);" DataSourceID="odsNumParc" DataTextField="Descr"
                                                                                        DataValueField="Id">
                                                                                        <asp:ListItem></asp:ListItem>
                                                                                        <asp:ListItem>1</asp:ListItem>
                                                                                        <asp:ListItem>2</asp:ListItem>
                                                                                        <asp:ListItem>3</asp:ListItem>
                                                                                        <asp:ListItem>4</asp:ListItem>
                                                                                    </asp:DropDownList>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table cellpadding="0" cellspacing="0" id="funcionarioComprador" style='<%# ((bool)Eval("VendidoFuncionario")) ? "" : "display: none; " %>padding-top: 2px'>
                                                            <tr>
                                                                <td>
                                                                    Funcionário comp.:&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFuncVenda" runat="server" DataSourceID="odsFuncVenda"
                                                                        DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True"
                                                                        SelectedValue='<%# Bind("IdFuncVenda") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <asp:HiddenField ID="hdfTipoVendaAtual" runat="server"
                                                            Value='<%# Eval("TipoVenda") %>' />
                                                    </td>
                                                    <td id="tdTipoEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Entrega
                                                    </td>
                                                    <td id="tdTipoEntrega2" align="left" nowrap="nowrap">
                                                        <asp:DropDownList ID="ddlTipoEntrega" runat="server" SelectedValue='<%# Bind("TipoEntrega") %>'
                                                            onchange="setLocalObra();" DataSourceID="odsTipoEntrega" DataTextField="Descr"
                                                            DataValueField="Id">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdFormaPagto1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Forma Pagto.
                                                    </td>
                                                    <td id="tdFormaPagto2" align="left" nowrap="nowrap" class="dtvAlternatingRow">
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
                                                                        DataTextField="Descricao" DataValueField="IdTipoCartao" SelectedValue='<%# Bind("IdTipoCartao") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdDataEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Entrega
                                                    </td>
                                                    <td id="tdDataEntrega2" align="left" nowrap="nowrap" class="dtvAlternatingRow">
                                                        <asp:TextBox ID="txtDataEntrega" runat="server" onkeypress="return false;" Text='<%# Bind("DataEntregaString") %>'
                                                            onchange="verificaDataEntrega(this)" OnLoad="txtDataEntrega_Load"></asp:TextBox>
                                                        <asp:ImageButton ID="imgDataEntrega" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                                            OnClientClick="return SelecionaData('txtDataEntrega', this)" ToolTip="Alterar" />
                                                        <asp:HiddenField ID="hdfDataEntregaNormal" runat="server" />
                                                        <asp:HiddenField ID="hdfDataEntregaFD" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdValorEntrada1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Valor Entrada
                                                    </td>
                                                    <td id="tdValorEntrada2" align="left" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValEntrada" runat="server" Value='<%# Bind("ValorEntrada") %>'
                                                            Visible='<%# !(bool)Eval("RecebeuSinal") %>' />
                                                        <asp:Label ID="lblValor" runat="server" Text='<%# Eval("ConfirmouRecebeuSinal") %>'
                                                            Visible='<%# Eval("RecebeuSinal") %>'></asp:Label>
                                                        <asp:HiddenField ID="hdfValorEntrada" runat="server" Value='<%# Eval("ValorEntrada") %>'
                                                            Visible='<%# Eval("RecebeuSinal") %>' />
                                                    </td>
                                                    <td id="tdDesconto1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Desconto
                                                    </td>
                                                    <td id="tdDesconto2" align="left" nowrap="nowrap">
                                                        <asp:DropDownList ID="drpTipoDesconto" runat="server" SelectedValue='<%# Bind("TipoDesconto") %>'
                                                            Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>'>
                                                            <asp:ListItem Value="2">R$</asp:ListItem>
                                                            <asp:ListItem Value="1">%</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:TextBox ID="txtDesconto" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                            Enabled='<%# Eval("DescontoEnabled") %>' onchange="calcularDesconto(this, 1)"
                                                            Text='<%# Bind("DescontoString") %>' Width="70px"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfDesconto" runat="server" Value='<%# Eval("Desconto") %>' />
                                                        <asp:HiddenField ID="hdfTipoDesconto" runat="server" Value='<%# Eval("TipoDesconto") %>' />
                                                        <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdTotal1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Total
                                                    </td>
                                                    <td id="tdTotal2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtTotal" runat="server" ReadOnly="True" Text='<%# Eval("Total", "{0:C}") %>'></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    &nbsp;Acréscimo&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoAcrescimo" runat="server" SelectedValue='<%# Bind("TipoAcrescimo") %>'
                                                                        Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>'>
                                                                        <asp:ListItem Value="2">R$</asp:ListItem>
                                                                        <asp:ListItem Value="1">%</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <asp:TextBox ID="txtAcrescimo" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Text='<%# Bind("AcrescimoString") %>' Width="70px" Enabled='<%# !(bool)Eval("BloquearDescontoAcrescimoAtualizar") %>'></asp:TextBox>
                                                                    <asp:HiddenField ID="hdfAcrescimo" runat="server" Value='<%# Eval("Acrescimo") %>' />
                                                                    <asp:HiddenField ID="hdfTipoAcrescimo" runat="server" Value='<%# Eval("TipoAcrescimo") %>' />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdFuncionario1" align="left" nowrap="nowrap" class="dtvHeader">
                                                        Funcionário
                                                    </td>
                                                    <td id="tdFuncionario2" align="left" nowrap="nowrap" class="dtvAlternatingRow">
                                                        <asp:DropDownList ID="drpVendedorEdit" runat="server" DataSourceID="odsFuncionario"
                                                            DataTextField="Nome" DataValueField="IdFunc" Enabled='<%# Eval("SelVendEnabled") %>'
                                                            SelectedValue='<%# Bind("IdFunc") %>'>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" class="dtvHeader">

                                                    </td>
                                                    <td align="left" colspan="3">

                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdParcela" align="left" class="dtvHeader" colspan="4" nowrap="nowrap">
                                                        <uc3:ctrlParcelas ID="ctrlParcelas1" runat="server" NumParcelas="4" NumParcelasLinha="6"
                                                            Datas='<%# Bind("DatasParcelas") %>' Valores='<%# Bind("ValoresParcelas") %>'
                                                            OnDataBinding="ctrlParcelas1_DataBinding" />
                                                        <asp:HiddenField ID="hdfExibirParcela" runat="server" />
                                                        <asp:HiddenField ID="hdfCalcularParcela" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr class="dtvHeader">
                                                    <td nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    Local da Obra&nbsp;
                                                                </td>
                                                                <td>
                                                                    <a href="#" onclick="getEnderecoCli()">
                                                                        <img src="../Images/home.gif" title="Buscar endereço do cliente" border="0"></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        Endereço
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtEnderecoObra" runat="server" MaxLength="100" disabled="true"
                                                            Text='<%# Bind("EnderecoObra") %>' Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Bairro
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtBairroObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("BairroObra") %>'
                                                            Width="130px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Cidade
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCidadeObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("CidadeObra") %>'
                                                            Width="130px"></asp:TextBox>
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
                                                                        OnClientClick="limparComissionado(); return false;" ToolTip="Limpar comissionado"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>" />
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
                                                                    &nbsp;
                                                                    <asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Text='<%# Bind("PercComissaoString") %>' Width="50px" OnLoad="txtPercentual_Load"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
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
                                            <table id="tbMedidor" style="width: 100%" class="dtvHeader" cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    &nbsp;Medidor:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblMedidor" runat="server" Text='<%# Eval("NomeMedidor") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelMedidor" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelMedidor.aspx'); return false;">
                                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr>
                                                    <td class="dtvHeader" align="center">
                                                        Observação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:TextBox ID="TextBox3" runat="server" MaxLength="1000" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfCliente" runat="server" Value='<%# Bind("IdCli") %>' />
                                                        <asp:HiddenField ID="hdfIdComissionado" runat="server" Value='<%# Bind("IdComissionado") %>' />
                                                        <asp:HiddenField ID="hdfIdMedidor" runat="server" Value='<%# Bind("IdMedidor") %>' />
                                                        <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                                        <asp:HiddenField ID="hdfValorComissao" runat="server" Value='<%# Bind("ValorComissaoString") %>' />
                                                        <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                                                        <asp:HiddenField ID="hdfDataPedido" runat="server" Value='<%# Bind("DataPedidoString") %>' />
                                                        <asp:HiddenField ID="hdfAliquotaIcms" runat="server" Value='<%# Bind("AliquotaIcmsString") %>' />
                                                        <asp:HiddenField ID="hdfRecebeuSinal" runat="server" Value='<%# Bind("RecebeuSinal") %>' />
                                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNumParc" runat="server" SelectMethod="GetNumParc" TypeName="Glass.Data.Helper.DataSources">
                                                        </colo:VirtualObjectDataSource>
                                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                                                            TypeName="Glass.Data.Helper.DataSources"></colo:VirtualObjectDataSource>
                                                        <asp:HiddenField ID="hdfMaoDeObra" runat="server" Value='<%# Bind("MaoDeObra") %>' />
                                                        <asp:HiddenField ID="hdfProducao" runat="server" Value='<%# Bind("Producao") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <table cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cliente
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                            onkeydown="if (isEnter(event)) getCli(this);" onblur="getCli(this);" />
                                                        <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Text='<%# Eval("NomeCliente") %>'
                                                            Width="250px"></asp:TextBox>
                                                        <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=pedido'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                        <br />
                                                        <asp:Label ID="lblObsCliente" runat="server" Text=""></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Ped.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataPed" runat="server" ReadOnly="True" Width="70px" Text='<%# Eval("DataPedidoString") %>'></asp:TextBox>
                                                        <asp:CheckBox ID="chkFastDelivery" runat="server" Checked='<%# Bind("FastDelivery") %>'
                                                            OnLoad="FastDelivery_Load" Text="Fast delivery" onclick="alteraFastDelivery(this.checked)" />
                                                        &nbsp;<asp:CheckBox ID="chkTemperaFora" runat="server" OnLoad="TemperaFora_Load"
                                                            Text="Têmpera fora" onclick="alteraTemperaFora(this.checked)" Checked='<%# Bind("TemperaFora") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cód. Ped. Cli.
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtCodPedCli" runat="server" MaxLength="20" Text='<%# Bind("CodCliente") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Orcamento
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIdOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("IdOrcamento") %>' Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdTipoVenda1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Venda
                                                    </td>
                                                    <td id="tdTipoVenda2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoVenda" runat="server" onchange="tipoVendaChange(this, true);"
                                                                        SelectedValue='<%# Bind("TipoVenda") %>' DataSourceID="odsTipoVenda" DataTextField="Descr"
                                                                        DataValueField="Id">
                                                                    </asp:DropDownList>
                                                                    <div id="divObra" style="display: none">
                                                                        <asp:TextBox ID="txtObra" runat="server" Enabled="false" Width="200px" Text='<%# Eval("DescrObra") %>'></asp:TextBox>
                                                                        <asp:ImageButton ID="imbObra" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(560, 650, '../Utils/SelObra.aspx?Situacao=4&IdCliente=' + FindControl('txtNumCli', 'input').value); return false;" />
                                                                        <br />
                                                                        Saldo da obra:
                                                                        <asp:Label ID="lblSaldoObra" runat="server" Text='<%# Eval("SaldoObra", "{0:C}") %>'></asp:Label>
                                                                        <asp:HiddenField ID="hdfIdObra" runat="server" Value='<%# Bind("IdObra") %>' />
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table cellpadding="0" cellspacing="0" id="funcionarioComprador" style='display: none; padding-top: 2px'>
                                                            <tr>
                                                                <td>
                                                                    Funcionário comp.:&nbsp;
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFuncVenda" runat="server" DataSourceID="odsFuncVenda"
                                                                        DataTextField="Nome" DataValueField="IdFunc" AppendDataBoundItems="True"
                                                                        SelectedValue='<%# Bind("IdFuncVenda") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdTipoEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Tipo Entrega
                                                    </td>
                                                    <td id="tdTipoEntrega2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:DropDownList ID="ddlTipoEntrega" runat="server" SelectedValue='<%# Bind("TipoEntrega") %>'
                                                            onchange="setLocalObra();" AppendDataBoundItems="True" DataSourceID="odsTipoEntrega"
                                                            DataTextField="Descr" DataValueField="Id" OnLoad="ddlTipoEntrega_Load">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdFormaPagto1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Forma Pagto.
                                                    </td>
                                                    <td id="tdFormaPagto2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFormaPagto" runat="server" AppendDataBoundItems="True" DataSourceID="odsFormaPagto"
                                                                        DataTextField="Descricao" onchange="formaPagtoChanged();" DataValueField="IdFormaPagto"
                                                                        SelectedValue='<%# Bind("IdFormaPagto") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoCartao" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoCartao"
                                                                        DataTextField="Descricao" DataValueField="IdTipoCartao" SelectedValue='<%# Bind("IdTipoCartao") %>'>
                                                                        <asp:ListItem></asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td id="tdDataEntrega1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data Entrega
                                                    </td>
                                                    <td id="tdDataEntrega2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataEntrega" runat="server" onkeypress="return false;" Text='<%# Bind("DataEntregaString") %>'
                                                            onchange="verificaDataEntrega(this)" OnLoad="txtDataEntrega_Load"></asp:TextBox>
                                                        <asp:ImageButton ID="imgDataEntrega" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif"
                                                            OnClientClick="return SelecionaData('txtDataEntrega', this)" ToolTip="Alterar" />
                                                        <asp:HiddenField ID="hdfDataEntregaNormal" runat="server" />
                                                        <asp:HiddenField ID="hdfDataEntregaFD" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="tdValorEntrada1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Valor Entrada
                                                    </td>
                                                    <td id="tdValorEntrada2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValEntradaIns" runat="server" Value='<%# Bind("ValorEntrada") %>' />
                                                    </td>
                                                    <td id="tdFuncionario1" align="left" class="dtvHeader" nowrap="nowrap">
                                                        Funcionário
                                                    </td>
                                                    <td id="tdFuncionario2" align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:DropDownList ID="drpVendedorIns" runat="server" DataSourceID="odsFuncionario"
                                                            DataTextField="Nome" DataValueField="IdFunc" Enabled="<%# Glass.Data.Helper.UserInfo.GetUserInfo.TipoUsuario!=2 %>"
                                                            SelectedValue='<%# Bind("IdFunc") %>'>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" class="dtvHeader">

                                                    </td>
                                                    <td align="left" colspan="3" class="dtvAlternatingRow">

                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr class="dtvHeader">
                                                    <td nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    Local da Obra&nbsp;
                                                                </td>
                                                                <td>
                                                                    <a href="#" onclick="getEnderecoCli()">
                                                                        <img src="../Images/home.gif" title="Buscar endereço do cliente" border="0"></a>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        Endereço
                                                    </td>
                                                    <td nowrap="nowrap">
                                                        <asp:TextBox ID="txtEnderecoObra" runat="server" MaxLength="100" disabled="true"
                                                            Text='<%# Bind("EnderecoObra") %>' Width="200px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Bairro
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtBairroObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("BairroObra") %>'
                                                            Width="130px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        Cidade
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCidadeObra" runat="server" MaxLength="50" disabled="true" Text='<%# Bind("CidadeObra") %>'
                                                            Width="130px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tbComissionado" cellpadding="0" cellspacing="0" style="width: 100%">
                                                <tr class="dtvHeader">
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td class="dtvHeader">
                                                                    Comissionado:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblComissionado" runat="server"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelComissionado" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelComissionado.aspx'); return false;"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>">
                                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                    <asp:ImageButton ID="imbLimparComissionado" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                                                        OnClientClick="limparComissionado(); return false;" ToolTip="Limpar comissionado"
                                                                        Visible="<%# !Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente %>" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                            <tr>
                                                                <td class="dtvHeader">
                                                                    Percentual:
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Text='<%# Bind("PercComissaoString") %>' Width="50px" OnLoad="txtPercentual_Load"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        <table cellpadding="0" cellspacing="0" style='<%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente ? "display: none": "" %>'>
                                                            <tr>
                                                                <td class="dtvHeader">
                                                                    Valor Comissão:
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtValorComissao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        ReadOnly="True" Text='<%# Eval("ValorComissao", "{0:C}") %>' Width="70px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tbMedidor0" cellpadding="0" cellspacing="0" class="dtvHeader" style="width: 100%">
                                                <tr>
                                                    <td align="left">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    &nbsp;Medidor:
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:Label ID="lblMedidor" runat="server" Text='<%# Eval("NomeMedidor") %>'></asp:Label>
                                                                </td>
                                                                <td>
                                                                    &nbsp;<asp:LinkButton ID="lnkSelMedidor" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelMedidor.aspx'); return false;">
                                                                        <img border="0"
                            src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%" cellpadding="4" cellspacing="0">
                                                <tr>
                                                    <td class="dtvHeader" align="center">
                                                        Observação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:TextBox ID="TextBox3" runat="server" MaxLength="1000" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="650px"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfCliente" runat="server" Value='<%# Bind("IdCli") %>' />
                                                        <asp:HiddenField ID="hdfIdComissionado" runat="server" Value='<%# Bind("IdComissionado") %>' />
                                                        <asp:HiddenField ID="hdfIdMedidor" runat="server" Value='<%# Bind("IdMedidor") %>' />
                                                        <asp:HiddenField ID="hdfAliquotaIcms" runat="server" Value='<%# Eval("AliquotaIcmsString") %>' />
                                                        <asp:HiddenField ID="hdfDataPedido" runat="server" Value='<%# Bind("DataPedidoString") %>' />
                                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoVenda" runat="server" SelectMethod="GetTipoVenda"
                                                            TypeName="Glass.Data.Helper.DataSources"></colo:VirtualObjectDataSource>
                                                        <asp:HiddenField ID="hdfMaoDeObra" runat="server" OnLoad="hdfMaoDeObra_Load" Value='<%# Bind("MaoDeObra") %>' />
                                                        <asp:HiddenField ID="hdfProducao" runat="server" OnLoad="hdfProducao_Load" Value='<%# Bind("Producao") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <table cellpadding="2" cellspacing="2">
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Num. Pedido
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNumPedido" runat="server" Text='<%# Eval("IdPedido") %>' Font-Size="Medium"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:Label ID="lblNomeCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Funcionário
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeFunc" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Venda
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoVenda" runat="server" Text='<%# Eval("DescrTipoVenda") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Loja
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeLoja" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Valor Entrada
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblValorEnt" runat="server"
                                                            Text='<%# Eval("ValorEntrada", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Entrega
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoEntrega" runat="server" Text='<%# Eval("DescrTipoEntrega") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data Entrega
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDataEntrega" runat="server" Text='<%# Eval("DataEntregaString") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Situação
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("DescrSituacaoPedido") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data Ped.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataPedidoString", "{0:d}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        &nbsp;</td>
                                                    <td align="left" nowrap="nowrap">
                                                        &nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Desconto
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDesconto" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label13" runat="server" Text="Comissão" Visible='<%# Eval("ComissaoVisible") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblComissao" runat="server" Text='<%# Eval("ValorComissao", "{0:C}") %>'
                                                            Visible='<%# Eval("ComissaoVisible") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTituloIcms" runat="server" Font-Bold="True" Text="Valor ICMS" OnLoad="Icms_Load"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblValorIcms" runat="server" OnLoad="Icms_Load" Text='<%# Eval("ValorIcms", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="6" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td class="cabecalho">
                                                                    <asp:Label ID="lblTitleTotal" runat="server" Font-Bold="True"
                                                                        onload="lblTotalGeral_Load" Text="Total"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotal" runat="server" ForeColor="#0000CC"
                                                                        onload="lblTotalGeral_Load" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                                <td class="cabecalho" nowrap="nowrap">
                                                                    <asp:Label ID="lblTitleTotalBruto" runat="server" Font-Bold="True"
                                                                        onload="lblTotalBrutoLiquido_Load" Text="Total Bruto"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotalBruto" runat="server" onload="lblTotalBrutoLiquido_Load"
                                                                        Text='<%# Eval("TotalBruto", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                                <td class="cabecalho" nowrap="nowrap">
                                                                    <asp:Label ID="lblTitleTotalLiquido" runat="server" Font-Bold="True"
                                                                        onload="lblTotalBrutoLiquido_Load" Text="Total Líquido"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotalLiquido" runat="server" ForeColor="#0000CC"
                                                                        onload="lblTotalBrutoLiquido_Load" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Forma Pagto.
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <asp:Label ID="lblFormaPagto" runat="server" Text='<%# Eval("PagtoParcela") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="lblTituloFastDelivery" runat="server" Text="Fast delivery" OnLoad="FastDelivery_Load"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblFastDelivery" runat="server" Text='<%# Eval("FastDeliveryString") %>'
                                                            OnLoad="FastDelivery_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">

                                                    </td>
                                                    <td colspan="3" align="left">

                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label15" runat="server" Text="Têmpera fora" OnLoad="TemperaFora_Load"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label ID="lblTemperaFora" runat="server" Text='<%# Eval("TemperaForaString") %>'
                                                            OnLoad="TemperaFora_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label16" runat="server" Text="Funcionário comp."></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="5">
                                                        <asp:Label ID="lblFuncVenda" runat="server" Text='<%# Eval("NomeFuncVenda") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label20" runat="server" Text="Transportador"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsTransportador" DataTextField="Name" DataValueField="Id"
                                                            SelectedValue='<%# Bind("IdTransportador") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr valign="top">
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold; padding-top: 6px">
                                                        <asp:Label ID="Label18" runat="server" Text="Seu cód. Pedido"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtCodPedCli" runat="server" MaxLength="20"
                                                            Text='<%# Bind("CodCliente") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold; padding-top: 6px">
                                                        <asp:Label ID="Label14" runat="server" Text="Observação"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:TextBox ID="txtObs" runat="server" MaxLength="1000"
                                                            Text='<%# Bind("Obs") %>' TextMode="MultiLine" Width="100%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold; padding-top: 6px">
                                                        <asp:Label ID="Label19" runat="server" Text="Observação Liberação/Faturamento/Entrega"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:TextBox ID="txtObsLib" runat="server" MaxLength="1000"
                                                            Text='<%# Bind("ObsLiberacao") %>' TextMode="MultiLine" Width="100%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                            <asp:HiddenField ID="hdfCliRevenda" runat="server" Value='<%# Eval("CliRevenda") %>' />
                                            <asp:HiddenField ID="hdfTipoVenda" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                            <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                            <asp:HiddenField ID="hdfPercComissao" runat="server" Value='<%# Eval("PercComissao") %>'
                                                Visible='<%# Eval("ComissaoVisible") %>' />
                                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCli") %>' />
                                            <asp:HiddenField ID="hdfFastDelivery" runat="server" OnPreRender="FastDelivery_Load"
                                                Value='<%# Eval("FastDelivery") %>' />
                                            <asp:HiddenField ID="hdfTemperaFora" runat="server" OnLoad="TemperaFora_Load" Value='<%# Eval("TemperaFora") %>' />
                                            <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                            <asp:HiddenField ID="hdfIsReposicao" runat="server"
                                                Value='<%# IsReposicao(Eval("TipoVenda")) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                                OnClientClick="if (!onSave()) return false;" />
                                            <asp:Button ID="btnCancelarEdit" CausesValidation="false" runat="server" OnClick="btnCancelarEdit_Click"
                                                Text="Cancelar" />
                                            <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdPedido"
                                                Text='<%# Bind("IdPedido") %>' />
                                            <asp:HiddenField ID="hdfLoja" runat="server" Value='<%# Bind("IdLoja") %>' />
                                            <asp:HiddenField ID="hdfClienteAtual" runat="server" Value='<%# Eval("IdCli") %>' />
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="if (!onInsert()) return false;"
                                                Text="Inserir" />
                                            <img id="load" alt="" src="../Images/load.gif" style="display: none" />
                                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                                Text="Cancelar" />
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar"
                                                OnClick="btnEditar_Click" Visible="False" />
                                            <asp:Button ID="btnSalvar" runat="server" onclick="btnSalvar_Click"
                                                Text="Salvar dados" />
                                            <asp:Button ID="btnFinalizar" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                                Text="Finalizar" OnClientClick="if (!finalizarPedido()) return false;" OnClick="btnFinalizar_Click" />
                                            <asp:Button ID="btnVoltar" runat="server" OnClick="btnCancelar_Click" Visible="false"
                                                Text="Voltar" />
                                            <asp:HiddenField ID="hdfLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                            <asp:HiddenField ID="hdfAlterarProjeto" runat="server" Value="false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkProjeto" runat="server" OnClientClick="return openProjeto('', false);">Incluir Projeto</asp:LinkButton>
                <div id="divProduto" runat="server">
                    <table>
                        <tr runat="server" id="inserirMaoObra" visible="false">
                            <td align="center">
                                <asp:LinkButton ID="lbkInserirMaoObra" runat="server">Inserir várias peças de vidro com a mesma mão de obra</asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdAmbiente" runat="server" AllowPaging="True"
                                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdAmbientePedido"
                                    DataSourceID="odsAmbiente" OnRowCommand="grdAmbiente_RowCommand" ShowFooter="True"
                                    OnPreRender="grdAmbiente_PreRender" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnRowDeleted="grdAmbiente_RowDeleted">
                                    <Columns>
                                        <asp:TemplateField>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="lnkAddAmbiente" runat="server" OnClientClick="addAmbiente(true); return false;"
                                                    ImageUrl="~/Images/Insert.gif" CausesValidation="False" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CausesValidation="False">
                                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Excluir este ambiente fará com que todos os produtos do mesmo sejam excluídos também, confirma exclusão?&quot;)"
                                                    CausesValidation="False" />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" ValidationGroup="ambiente" />
                                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Cancelar" CausesValidation="False" />
                                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                                            </EditItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtAmbiente" runat="server" Text='<%# Eval("Ambiente") %>' MaxLength="50"
                                                    Width="150px" OnLoad="txtAmbiente_Load" onchange="FindControl('hdfDescrAmbiente', 'input').value = this.value"></asp:TextBox>
                                                <div runat="server" id="EditAmbMaoObra" onload="ambMaoObra_Load">
                                                    <asp:TextBox ID="txtCodAmb" runat="server" onblur="produtoAmbiente=true; loadProduto(this.value);"
                                                        onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                                        Text='<%# Eval("CodInterno") %>' Width="50px"></asp:TextBox>
                                                    <asp:Label ID="lblDescrAmb" runat="server" Text='<%# Eval("Ambiente") %>'></asp:Label>
                                                    <a href="#" onclick="produtoAmbiente=true; getProduto(); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                    <asp:HiddenField ID="hdfAmbIdProd" Value='<%# Bind("IdProd") %>' runat="server" />
                                                </div>
                                                <asp:HiddenField ID="hdfDescrAmbiente" Value='<%# Bind("Ambiente") %>' runat="server" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtAmbiente" runat="server" MaxLength="50" Width="150px" OnLoad="txtAmbiente_Load"
                                                    onchange="FindControl('hdfDescrAmbiente', 'input').value = this.value"></asp:TextBox>
                                                <div runat="server" id="ambMaoObra" onload="ambMaoObra_Load">
                                                    <asp:TextBox ID="txtCodAmb" runat="server" onblur="produtoAmbiente=true; loadProduto(this.value);"
                                                        onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                                        Width="50px"></asp:TextBox>
                                                    <asp:Label ID="lblDescrAmb" runat="server"></asp:Label>
                                                    <a href="#" onclick="produtoAmbiente=true; getProduto(); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                    <asp:HiddenField ID="hdfAmbIdProd" runat="server" />
                                                </div>
                                                <asp:HiddenField ID="hdfDescrAmbiente" runat="server" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkViewProd" runat="server" CausesValidation="False" CommandArgument='<%# Eval("IdAmbientePedido") %>'
                                                    CommandName="ViewProd" Text='<%# Eval("Ambiente") %>' Visible='<%# !(bool)Eval("ProjetoVisible") %>'></asp:LinkButton>
                                                <asp:PlaceHolder ID="PlaceHolder1" Visible='<%# Eval("ProjetoVisible") %>' runat="server">
                                                    <a href="#" onclick='return openProjeto(<%# Eval("IdAmbientePedido") %>)'>
                                                        <%# Eval("Ambiente") %></a> </asp:PlaceHolder>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditDescricao" runat="server" Text='<%# Bind("Descricao") %>'
                                                    MaxLength="1000" Rows="2" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="1000" Rows="2" TextMode="MultiLine"
                                                    Width="300px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                                <asp:Label ID="Label17" runat="server" ForeColor="Red"
                                                    Text='<%# Eval("DescrObsProj") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditQtdeAmbiente" runat="server" Text='<%# Bind("Qtde") %>' onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ControlToValidate="txtEditQtdeAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtQtdeAmbiente" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ControlToValidate="txtQtdeAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditAlturaAmbiente" runat="server" Text='<%# Bind("Altura") %>'
                                                    onkeypress="return soNumeros(event, true, true)" Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvAltura" runat="server" ControlToValidate="txtEditAlturaAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtAlturaAmbiente" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvAltura" runat="server" ControlToValidate="txtAlturaAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtEditLarguraAmbiente" runat="server" Text='<%# Bind("Largura") %>'
                                                    onkeypress="return soNumeros(event, true, true)" Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvLargura" runat="server" ControlToValidate="txtEditLarguraAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtLarguraAmbiente" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                    Width="50px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvLargura" runat="server" ControlToValidate="txtLarguraAmbiente"
                                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao"
                                            Visible="False">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbAplIns" runat="server" onblur="aplAmbiente=true; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { aplAmbiente=true; loadApl(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Text='<%# Eval("CodAplicacao") %>'
                                                                Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#"
                                                                onclick="aplAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdAplicacao" runat="server"
                                                    Value='<%# Bind("IdAplicacao") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbAplIns" runat="server" onblur="aplAmbiente=true; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { aplAmbiente=true; loadApl(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Text='<%# Eval("CodAplicacao") %>'
                                                                Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#"
                                                                onclick="aplAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdAplicacao" runat="server"
                                                    Value='<%# Bind("IdAplicacao") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso"
                                            Visible="False">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbProcIns" runat="server" onblur="procAmbiente=true; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { procAmbiente=true; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#"
                                                                onclick="procAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdProcesso" runat="server"
                                                    Value='<%# Bind("IdProcesso") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAmbProcIns" runat="server" onblur="procAmbiente=true; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { procAmbiente=true; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#"
                                                                onclick="procAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfAmbIdProcesso" runat="server"
                                                    Value='<%# Bind("IdProcesso") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label9" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Redondo" SortExpression="Redondo" Visible="False">
                                            <EditItemTemplate>
                                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("Redondo") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:CheckBox ID="chkRedondoAmbiente" runat="server" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("Redondo") %>' Enabled="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor produtos" SortExpression="TotalProdutos">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotalProd" runat="server" Text='<%# Eval("TotalProdutos", "{0:c}") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("TotalProdutos", "{0:c}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <FooterTemplate>
                                                <asp:LinkButton ID="lnkInsAmbiente" runat="server" OnClick="lnkInsAmbiente_Click"
                                                    ValidationGroup="ambiente">
                                            <img border="0" src="../Images/ok.gif" /></asp:LinkButton>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle CssClass="edit"></EditRowStyle>
                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                </asp:GridView>
                                <asp:Label ID="lblAmbiente" runat="server" CssClass="subtitle1" Font-Bold="False"></asp:Label>
                                <asp:HiddenField ID="hdfAlturaAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfLarguraAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfQtdeAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfRedondoAmbiente" runat="server" />
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAmbiente" runat="server" DataObjectTypeName="Glass.Data.Model.AmbientePedido"
                                    DeleteMethod="DeleteComTransacao" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.AmbientePedidoDAO"
                                    UpdateMethod="Update" OnDeleted="odsAmbiente_Deleted" OnUpdated="odsAmbiente_Updated">
                                    <SelectParameters>
                                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                                <asp:HiddenField ID="hdfIdAmbiente" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%# Eval("Ambiente") %>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXPed" CssClass="gridStyle"
                                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                    DataKeyNames="IdProdPed" OnRowDeleted="grdProdutos_RowDeleted" ShowFooter="True"
                                    OnRowCommand="grdProdutos_RowCommand" OnPreRender="grdProdutos_PreRender" PageSize="12"
                                    OnRowUpdated="grdProdutos_RowUpdated" Visible="false">
                                    <FooterStyle Wrap="True" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <FooterTemplate>
                                                <select id="drpFooterVisible" style="display: none">
                                                </select>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Excluir" Visible='<%# Eval("DeleteVisible") %>' OnClientClick="if (!confirm(&quot;Deseja excluir esse produto do pedido?&quot;)) return false;" />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="if (!onUpdateProd()) return false;" />
                                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Cancelar" />
                                                <asp:HiddenField ID="hdfProdPed" runat="server"
                                                    Value='<%# Eval("IdProdPed") %>' />
                                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                                <asp:HiddenField ID="hdfCodInterno" runat="server" Value='<%# Eval("CodInterno") %>' />
                                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                                <asp:HiddenField ID="hdfIsAluminio" runat="server" Value='<%# Eval("IsAluminio") %>' />
                                                <asp:HiddenField ID="hdfM2Minimo" runat="server" Value='<%# Eval("M2Minimo") %>' />
                                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                                <asp:HiddenField ID="hdfIdItemProjeto" runat="server" Value='<%# Bind("IdItemProjeto") %>' />
                                                <asp:HiddenField ID="hdfIdMaterItemProj" runat="server" Value='<%# Bind("IdMaterItemProj") %>' />
                                                <asp:HiddenField ID="hdfIdAmbientePedido" runat="server" Value='<%# Bind("IdAmbientePedido") %>' />
                                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" Value='<%# Bind("AliqIcms") %>' />
                                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" Value='<%# Bind("ValorIcms") %>' />
                                                <asp:HiddenField ID="hdfValorTabelaOrcamento" runat="server" Value='<%# Bind("ValorTabelaOrcamento") %>' />
                                                <asp:HiddenField ID="hdfValorTabelaPedido" runat="server" Value='<%# Bind("ValorTabelaPedido") %>' />
                                            </EditItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblCodProdIns" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfCustoProd" runat="server"
                                                    Value='<%# Eval("CustoCompraProduto") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtCodProdIns" runat="server" onblur="loadProduto(this.value);"
                                                    onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                                    Width="50px"></asp:TextBox>
                                                <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
                                                <a href="#" onclick="getProduto(); return false;">
                                                    <img src="../Images/Pesquisar.gif" border="0" /></a>
                                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" />
                                                <asp:HiddenField ID="hdfTipoCalc" runat="server" />
                                                <asp:HiddenField ID="hdfIsAluminio" runat="server" />
                                                <asp:HiddenField ID="hdfM2Minimo" runat="server" />
                                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" />
                                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" />
                                                <asp:HiddenField ID="hdfCustoProd" runat="server" />
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtQtdeIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    Text='<%# Bind("Qtde") %>' Width="50px" onchange="getDescontoQtde()"></asp:TextBox>
                                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                                <uc5:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" Callback="calcTotalProd" CallbackValorUnit="calcTotalProd"
                                                    ValidationGroup="produto" PercDescontoQtde='<%# Bind("PercDescontoQtde") %>' ValorDescontoQtde='<%# Bind("ValorDescontoQtde") %>'
                                                    OnLoad="ctrlDescontoQtde_Load" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtQtdeIns" runat="server" onkeydown="if (isEnter(event)) calcM2Prod();"
                                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);" onblur="calcM2Prod(); return verificaEstoque();"
                                                    Width="50px" onchange="getDescontoQtde()"></asp:TextBox>
                                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                                <uc5:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" Callback="calcTotalProd" ValidationGroup="produto"
                                                    CallbackValorUnit="calcTotalProd" OnLoad="ctrlDescontoQtde_Load" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtLarguraIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, true, true);"
                                                    Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="50px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtLarguraIns" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                    onblur="calcM2Prod();" Width="50px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtAlturaIns" runat="server" onblur="GetAdicionalAlturaChapa(); calcM2Prod();" Text='<%# Bind("Altura") %>'
                                                    onchange="FindControl('hdfAlturaReal', 'input').value = this.value" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"></asp:TextBox>
                                                <asp:HiddenField ID="hdfAlturaReal" runat="server" Value='<%# Bind("AlturaReal") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtAlturaIns" runat="server" onblur=" GetAdicionalAlturaChapa(); calcM2Prod();" Width="50px"
                                                    onchange="FindControl('hdfAlturaRealIns', 'input').value = this.value" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"></asp:TextBox>
                                                <asp:HiddenField ID="hdfAlturaRealIns" runat="server" />
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotM2Ins" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfTotM" runat="server" Value='<%# Eval("TotM") %>' />
                                                <asp:HiddenField ID="hdfTamanhoMaximoObra" runat="server" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotM2Ins" runat="server"></asp:Label>
                                                <asp:HiddenField ID="hdfTamanhoMaximoObra" runat="server" />
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotM2Calc">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotM2Calc" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                                                <asp:HiddenField ID="hdfTotM2Calc" runat="server" Value='<%# Eval("TotM2Calc") %>' />
                                                <asp:HiddenField ID="hdfTotM2CalcSemChapa" runat="server" Value='<%# Eval("TotalM2CalcSemChapaString") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotM2Calc" runat="server"></asp:Label>
                                                <asp:HiddenField ID="hdfTotM2CalcIns" runat="server" />
                                                <asp:HiddenField ID="hdfTotM2CalcSemChapaIns" runat="server" />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label12" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="True" />
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("ValorVendido", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtValorIns" runat="server" onblur="calcTotalProd();" onkeypress="return soNumeros(event, false, true);"
                                                    Text='<%# Bind("ValorVendido") %>' Width="50px" OnLoad="txtValorIns_Load"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtValorIns" runat="server" onkeydown="if (isEnter(event)) calcTotalProd();"
                                                    onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProd();"
                                                    Width="50px" OnLoad="txtValorIns_Load"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="aplAmbiente=false; loadApl(this.value);" onkeydown="if (isEnter(event)) { aplAmbiente=false; loadApl(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="aplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="aplAmbiente=false; loadApl(this.value);" onkeydown="if (isEnter(event)) { aplAmbiente=false; loadApl(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="aplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="procAmbiente=false; loadProc(this.value);" onkeydown="if (isEnter(event)) { procAmbiente=false; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick='procAmbiente=false; openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="procAmbiente=false; loadProc(this.value);" onkeydown="if (isEnter(event)) { procAmbiente=false; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick='procAmbiente=false; openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ped. Cli." SortExpression="PedCli">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtPedCli" runat="server" MaxLength="50"
                                                    Text='<%# Bind("PedCli") %>' Width="50px"></asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="txtPedCli" runat="server" MaxLength="50"
                                                    Width="50px"></asp:TextBox>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label13" runat="server" Text='<%# Bind("PedCli") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblTotalIns" runat="server" Text='<%# Bind("Total") %>' Style="padding-top: 4px"></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblTotalIns" runat="server"></asp:Label>
                                            </FooterTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
                                            <EditItemTemplate>
                                                <asp:Label ID="lblValorBenef" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblValorBenef" runat="server"></asp:Label>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label11" runat="server" Text='<%# Bind("ValorBenef", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <EditItemTemplate>
                                                <div id="benefMaoObra" style='<%# !IsPedidoMaoDeObra() ? "display: none;": "" %> white-space: nowrap'>
                                                    <asp:DropDownList ID="drpLargBenef" runat="server" onchange="calcTotalProd()" SelectedValue='<%# Bind("LarguraBenef") %>'>
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:DropDownList ID="drpAltBenef" runat="server" onchange="calcTotalProd()" SelectedValue='<%# Bind("AlturaBenef") %>'>
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    Esp.:
                                                    <asp:TextBox ID="txtEspBenef" Width="30px" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                        Text='<%# Bind("EspessuraBenef") %>'></asp:TextBox>
                                                </div>
                                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(this); return false;"
                                                    Visible='<%# Eval("BenefVisible") %>'>
                                                    <img border="0" src="../Images/gear_add.gif" />
                                                </asp:LinkButton>
                                                <table id="tbConfigVidro" cellspacing="0" style="display: none;">
                                                    <tr align="left">
                                                        <td align="center">
                                                            <table>
                                                                <tr>
                                                                    <td class="dtvFieldBold">
                                                                        Espessura
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtEspessura" runat="server" OnDataBinding="txtEspessura_DataBinding"
                                                                            onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Bind("Espessura") %>'></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc4:ctrlBenef ID="ctrlBenefEditar" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>'
                                                                ValidationGroup="produto" OnLoad="ctrlBenef_Load" Redondo='<%# Bind("Redondo") %>'
                                                                CallbackCalculoValorTotal="setValorTotal" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                        </td>
                                                    </tr>
                                                </table>
                                                <script type="text/javascript">
                                                    calculaTamanhoMaximo();
                                                </script>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <div id="benefMaoObra" style='<%# !IsPedidoMaoDeObra() ? "display: none;": "" %> white-space: nowrap'>
                                                    <asp:DropDownList ID="drpLargBenef" runat="server" onchange="calcTotalProd()">
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:DropDownList ID="drpAltBenef" runat="server" onchange="calcTotalProd()">
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    Esp.:
                                                    <asp:TextBox ID="txtEspBenef" Width="30px" runat="server" onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                                </div>
                                                <asp:LinkButton ID="lnkBenef" runat="server" Style="display: none;" OnClientClick="exibirBenef(this); return false;">
                                                    <img border="0" src="../Images/gear_add.gif" />
                                                </asp:LinkButton>
                                                <table id="tbConfigVidro" cellspacing="0" style="display: none;">
                                                    <tr align="left">
                                                        <td align="center">
                                                            <table>
                                                                <tr>
                                                                    <td class="dtvFieldBold">
                                                                        Espessura
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtEspessura" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                            Width="30px"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc4:ctrlBenef ID="ctrlBenefInserir" runat="server" OnLoad="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotal"
                                                                ValidationGroup="produto" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                        </td>
                                                    </tr>
                                                </table>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <EditItemTemplate>
                                                </td> </tr>
                                                <tr style='<%= !IsPedidoMaoDeObra() ? "display: none": "" %>'>
                                                    <td colspan="14" style="text-align: right">
                                                        <span style="position: relative; top: -6px">campos usados para definir altura, largura
                                                            e
                                                            <br />
                                                            espessura da lapidação e bisotê </span>
                                                    </td>
                                                </tr>
                                                <tr style='<%= !IsPedidoProducao() ? "display: none": "" %>'>
                                                    <td colspan="4">
                                                    </td>
                                                    <td colspan="10" style="text-align: left">
                                                        <span style="position: relative; top: -6px">altura e largura definidas no produto
                                                            <br />
                                                            e recuperadas automaticamente </span>
                                                    </td>
                                                </tr>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <asp:ImageButton ID="lnkInsProd" runat="server" OnClick="lnkInsProd_Click" ImageUrl="../Images/ok.gif"
                                                    OnClientClick="if (!onSaveProd()) return false;" />
                                                </td> </tr>
                                                <tr style='<%= !IsPedidoMaoDeObra() ? "display: none": "" %>'>
                                                    <td colspan="14" style="text-align: right">
                                                        <span style="position: relative; top: -6px">campos usados para definir altura, largura
                                                            e
                                                            <br />
                                                            espessura da lapidação e bisotê </span>
                                                    </td>
                                                </tr>
                                                <tr style='<%= !IsPedidoProducao() ? "display: none": "" %>'>
                                                    <td colspan="4">
                                                    </td>
                                                    <td colspan="10" style="text-align: left">
                                                        <span style="position: relative; top: -6px">altura e largura definidas no produto
                                                            <br />
                                                            e recuperadas automaticamente </span>
                                                    </td>
                                                </tr>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle CssClass="edit"></EditRowStyle>
                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdXPed" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedido"
            DeleteMethod="DeleteEAtualizaDataEntrega" EnablePaging="True" MaximumRowsParameterName="pageSize"
            OnDeleted="odsProdXPed_Deleted" SelectCountMethod="GetCount" SelectMethod="GetList"
            SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
            UpdateMethod="UpdateEAtualizaDataEntrega" OnUpdated="odsProdXPed_Updated">
            <SelectParameters>
                <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                <asp:ControlParameter ControlID="hdfIdAmbiente" Name="idAmbientePedido" PropertyName="Value"
                    Type="UInt32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <asp:HiddenField ID="hdfPedidoMaoDeObra" runat="server" />
        <asp:HiddenField ID="hdfPedidoProducao" runat="server" />
        <asp:HiddenField ID="hdfBloquearMaoDeObra" runat="server" />
        <asp:HiddenField ID="hdfIdPedido" runat="server" />
        <asp:HiddenField ID="hdfIdProd" runat="server" />
        <asp:HiddenField ID="hdfComissaoVisible" runat="server" />
        <asp:HiddenField ID="hdfMedidorVisible" runat="server" />
        <asp:HiddenField ID="hdfCurrPage" runat="server" Value="0" />
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCartao" runat="server" SelectMethod="GetCredito"
            TypeName="Glass.Data.DAL.TipoCartaoCreditoDAO"></colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoEntrega" runat="server" SelectMethod="GetTipoEntrega"
            TypeName="Glass.Data.Helper.DataSources"></colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
            InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.PedidoDAO"
            UpdateMethod="Update" OnInserted="odsPedido_Inserted" OnUpdated="odsPedido_Updated">
            <SelectParameters>
                <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedores"
            TypeName="Glass.Data.DAL.FuncionarioDAO"></colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoJato" runat="server" SelectMethod="GetTipoJato"
            TypeName="Glass.Data.Helper.DataSources"></colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCanto" runat="server" SelectMethod="GetTipoCanto"
            TypeName="Glass.Data.Helper.DataSources"></colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForPedido"
            TypeName="Glass.Data.DAL.FormaPagtoDAO"></colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncVenda" runat="server" SelectMethod="GetOrdered"
            TypeName="Glass.Data.DAL.FuncionarioDAO"></colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTransportador" runat="server"
            SelectMethod="ObtemDescritoresTransportadores" TypeName="Glass.Global.Negocios.ITransportadorFluxo">
        </colo:VirtualObjectDataSource>

                                <asp:HiddenField ID="hdfNaoVendeVidro" runat="server" />

    </table>

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

    // Habilita/Desabilita campos do local da obra
    setLocalObra();

    var cCodProd = FindControl("txtCodProdIns", "input");
    if (cCodProd != null)
        cCodProd.focus();

    // Esconde tabela de comissionado
    var hdfComissaoVisible = FindControl("hdfComissaoVisible", "input");
    var tbComissionado = FindControl("tbComissionado", "table");
    if (hdfComissaoVisible != null && tbComissionado != null && hdfComissaoVisible.value == "false")
        tbComissionado.style.display = "none";

    // Esconde tabela de medidor
    var hdfMedidorVisible = FindControl("hdfMedidorVisible", "input");
    var tbMedidor = FindControl("tbMedidor", "table");
    if (hdfMedidorVisible != null && tbMedidor != null && hdfMedidorVisible.value == "false")
        tbMedidor.style.display = "none";

    // Esconde controles de inserção de ambiente
    if (FindControl("lnkAddAmbiente", "input") != null)
        addAmbiente(false);

    // Se a empressa não vende vidros, esconde campos
    if (FindControl("hdfNaoVendeVidro", "input").value == "true" && FindControl("grdProdutos", "table") != null)
    {
        var tbProd = FindControl("grdProdutos", "table");
        var rows = tbProd.children[0].children;

        var colsTitle = rows[0].getElementsByTagName("th");
        colsTitle[4].style.display = "none";
        colsTitle[5].style.display = "none";
        colsTitle[6].style.display = "none";
        colsTitle[7].style.display = "none";

        var k=0;
        for (k=1; k<rows.length; k++) {
            if (rows[k].cells[4] == null)
                break;

            rows[k].cells[4].style.display = "none";
            rows[k].cells[5].style.display = "none";
            rows[k].cells[6].style.display = "none";
            rows[k].cells[7].style.display = "none";
        }
    }
    else {
        // loadConfig();
        posValor = <%= GetPosValor() %>;

        var usarAltLarg = '<%= Glass.Configuracoes.PedidoConfig.EmpresaTrabalhaAlturaLargura %>'.toLowerCase();

        // Troca a posição da altura com a largura
        if (usarAltLarg == "true" && FindControl("grdProdutos", "table") != null) {
            var tbProd = FindControl("grdProdutos", "table");
            var rows = tbProd.children[0].children;

            // Troca a label de título altura-largura
            var colsTitle = rows[0].getElementsByTagName("th");
            var colAltInnerHtml = colsTitle[4].innerHTML;
            colsTitle[4].innerHTML = colsTitle[5].innerHTML;
            colsTitle[5].innerHTML = colAltInnerHtml;

            var j=0;
            for (j=1; j<rows.length; j++) {
                try
                {
                    var cols = rows[j].getElementsByTagName("td");
                    var colTemp = rows[j].cells[4].innerHTML;
                    rows[j].cells[4].innerHTML = rows[j].cells[5].innerHTML;
                    rows[j].cells[5].innerHTML = colTemp;
                }
                catch (err)
                { }
            }
        }
    }

    </script>

</asp:Content>
