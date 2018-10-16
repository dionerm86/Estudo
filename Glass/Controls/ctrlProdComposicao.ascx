<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlProdComposicao.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlProdComposicao" %>

<%@ Register Src="ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlProdComposicaoChild.ascx" TagName="ctrlProdComposicaoChild" TagPrefix="uc1"%>


<script type="text/javascript">

    // Guarda a quantidade disponível em estoque do produto buscado
    var qtdEstoqueComposicao = 0;
    var exibirMensagemEstoqueComposicao = false;
    var qtdEstoqueMensagemComposicao = 0;
    var pedidoReposicao = <%= VerificaPedidoReposicao() %>;
    var insertingComposicao = false;
    var produtoAmbienteComposicao = false;
    var aplAmbienteComposicao = false;
    var procAmbienteComposicao = false;
    var loadingComposicao = true;

    function buscaTable(control) {
        var tr = control;
        while (tr.id == "" || (tr.id.indexOf("prodPed_") == -1 && tr.nodeName.toLowerCase() != "tr")) {
            tr = tr.parentElement;
        }

        return tr;
    }

    function getNomeControleBenefComposicao(control)
    {
        var nomeControle = "<%= NomeControleBenefComposicao() %>";
        nomeControle = FindControl(nomeControle + "_tblBenef", "table", control);

        if (nomeControle == null)
            return null;

        nomeControle = nomeControle.id;
        return nomeControle.substr(0, nomeControle.lastIndexOf("_"));
    }

    // Carrega dados do produto com base no código do produto passado
    function loadProdutoComposicao(codInterno, control, alterarValor) {
        if (control == null || codInterno == "")
            return false;

        var table = buscaTable(control);

        var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;
        var txtValor = FindControl("txt_ValorComposicaoIns", "input", table);

        var verificaProduto = CadPedido.IsProdutoObra(idPedido, codInterno, true).value.split(";");
        if (verificaProduto[0] == "Erro")
        {
            if (FindControl("txt_CodProdComposicao", "input", table) != null)
                FindControl("txt_CodProdComposicao", "input", table).value = "";

            alert("Esse produto não pode ser usado no pedido. " + verificaProduto[1]);
            return false;
        }
        else if (parseFloat(verificaProduto[1].replace(",", ".")) > 0)
        {
            if (txtValor != null)
                txtValor.disabled = true;

            // Se for edição de produto, chamad o método padrão de cálculo da metragem máxima permitida
            if (FindControl("hdf_ProdPedComposicao", "input", table) != null)
                calculaTamanhoMaximoComposicao(table);
            else if (FindControl("hdf_TamanhoMaximoObraComposicao", "input", table) != null)
                FindControl("hdf_TamanhoMaximoObraComposicao", "input", table).value = verificaProduto[2];
        }
        else
        {
            if (txtValor != null)
                txtValor.disabled = verificaProduto[3] == "false";

            if (FindControl("hdf_TamanhoMaximoObraComposicao", "input", table) != null)
                FindControl("hdf_TamanhoMaximoObraComposicao", "input", table).value = "0";
        }

        try {
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var percComissao = getPercComissao();
            var tipoPedido = FindControl("hdfTipoPedido", "input").value;
            var tipoVenda = FindControl("hdfTipoVenda", "input").value;
            percComissao = percComissao == null ? 0 : percComissao.toString().replace('.', ',');

            var controleDescQtde = null;
            var percDescontoQtde = 0;

            if (FindControl("_divDescontoQtde", "div", table) != null)
            {
                controleDescQtde = FindControl("_divDescontoQtde", "div", table).id;
                controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
                if (controleDescQtde != null)
                    percDescontoQtde = controleDescQtde.PercDesconto();
            }

            var retorno = CadPedido.GetProduto(idPedido, codInterno, tipoEntrega, cliRevenda, idCliente,
                percComissao, tipoPedido, tipoVenda, produtoAmbienteComposicao, percDescontoQtde, FindControl("hdfLoja", "input").value, true).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                if (!produtoAmbienteComposicao)
                    FindControl("txt_CodProdComposicao", "input", table).value = "";
                else
                    FindControl("txt_CodAmbComposicao", "input", table).value = "";

                return false;
            }

            else if (!produtoAmbienteComposicao)
            {
                if (retorno[0] == "Prod") {
                    FindControl("hdf_IdProdComposicao", "input", table).value = retorno[1];

                    var subgrupoProdComposto = CadPedido.SubgrupoProdComposto(retorno[1]).value;
                    var tipoPedido = FindControl("hdfTipoPedido", "input").value;

                    alterarValor = alterarValor === false ? false : true;

                    if (parseFloat(verificaProduto[1].replace(",", ".")) > 0) // Exibe no cadastro o valor mínimo do produto
                        txtValor.value = alterarValor ? verificaProduto[1] : txtValor.value;
                        // O valor do produto deve ser atualizado sempre, para que caso seja buscado um produto, preenchendo automaticamente
                        // o valor unitário e o usuário resolva buscar outro produto sem ter inserido o primeiro, garanta que será buscado o valor deste
                    else
                        txtValor.value = alterarValor ? retorno[3] : txtValor.value;

                    FindControl("hdf_IsVidroComposicao", "input", table).value = retorno[4]; // Informa se o produto é vidro
                    FindControl("hdf_M2MinimoComposicao", "input", table).value = retorno[5]; // Informa se o produto possui m² mínimo
                    FindControl("hdf_TipoCalcComposicao", "input", table).value = retorno[7]; // Verifica como deve ser calculado o produto

                    // Se o campo do valor estiver desativado não precisa calcular o valor mínimo, tendo em vista que o usuário não poderá alterar.
                    if (!txtValor.disabled)
                        atualizaValMinComposicao(table);

                    qtdEstoqueComposicao = retorno[6]; // Pega a quantidade disponível em estoque deste produto
                    exibirMensagemEstoqueComposicao = retorno[14] == "true";
                    qtdEstoqueMensagemComposicao = retorno[15];

                    if(FindControl("txt_AlturaComposicaoIns", "input", table) != null && FindControl("txt_AlturaComposicaoIns", "input", table).value != ""){
                        GetAdicionalAlturaChapa();
                    }

                    var tipoCalc = retorno[7];

                    // Se o produto não for vidro, desabilita os textboxes largura e altura,
                    // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
                    // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
                    var cAltura = FindControl("txt_AlturaComposicaoIns", "input", table);
                    var cLargura = FindControl("txt_LarguraComposicaoIns", "input", table);
                    // var maoDeObra = FindControl("hdfPedidoMaoDeObra", "input").value == "true";
                    //var alturaAmbiente = FindControl("hdf_AlturaAmbienteComposicao", "input", table).value;
                    // var larguraAmbiente = FindControl("hdf_LarguraAmbienteComposicao", "input", table).value;
                    cAltura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
                    cLargura.disabled = CalcProd_DesabilitarLargura(tipoCalc);

                    var nomeControle = getNomeControleBenefComposicao(table);

                    // Zera o campo qtd para evitar que produtos calculados por mҠfiquem com quantidade decimal por exemplo (chamado 11010)
                    var txtQtdProd = FindControl("txt_QtdeComposicaoIns", "input", table);
                    if (txtQtdProd != null && !loadingComposicao)
                        txtQtdProd.value = "";

                    if (nomeControle != null && nomeControle != undefined) {
                        // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                        if (retorno[4] == "true" && exibirControleBenef(nomeControle) && FindControl("lnk_BenefComposicao", "input", table) != null) {
                            FindControl("txt_EspessuraComposicao", "input", table).value = retorno[8];
                            FindControl("txt_EspessuraComposicao", "input", table).disabled = retorno[8] != "" && retorno[8] != "0";
                        }

                        if (FindControl("lnk_BenefComposicao", "input", table) != null && nomeControle != null && nomeControle.indexOf("Inserir") > -1)
                            FindControl("lnk_BenefComposicao", "input", table).style.display = exibirControleBenef(nomeControle) ? "" : "none";
                    }

                    FindControl("hdf_AliquotaIcmsProdComposicao", "input", table).value = retorno[9].replace('.', ',');

                    // O campo altura e largura devem sempre ser atribuídos pois caso seja selecionado um box e logo após seja selecionado um kit
                    // por exemplo, ao inserí-lo ele estava ficando com o campo altura, largura e m² preenchidos apesar de ser calculado por qtd
                    if (retorno[10] != "" || retorno[4] == "false") {
                        FindControl("txt_AlturaComposicao", "input", table).value = retorno[10];
                        FindControl("hdf_AlturaRealComposicao", "input", table).value = retorno[10];
                    }
                    if (retorno[11] != "" || retorno[4] == "false") FindControl("txt_LarguraComposicao", "input", table).value = retorno[11];

                    if (cAltura.disabled && FindControl("hdf_AlturaRealComposicao", "input", table) != null)
                        FindControl("hdf_AlturaRealComposicao", "input", table).value = cAltura.value;

                    var idProdPed = FindControl("hdf_IdProdPed", "input", table).value;

                    if (retorno[16] != "")
                        setAplComposicao(retorno[16], retorno[17], idProdPed);

                    if (retorno[18] != "")
                        setProcComposicao(retorno[18], retorno[19], null, idProdPed);

                    FindControl("hdf_CustoProdComposicao", "input", table).value = retorno[20];

                    var cPodeEditarComposicao = FindControl("hdf_PodeEditarComposicao","input", table);
                    if(cPodeEditarComposicao != null)
                    {
                        var podeEditarComposicao = cPodeEditarComposicao.value;
                        var cQtdeComposicaoIns = FindControl("txt_QtdeComposicaoIns", "input", table);
                        cQtdeComposicaoIns.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cQtdeComposicaoIns.disabled;
                        cLargura.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cLargura.disabled;
                        cAltura.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cAltura.disabled;
                    }
                }

                FindControl("lbl_DescrProdComposicao", "span", table).innerHTML = retorno[2];

                if (retorno.length >= 22)
                    FindControl("lbl_DescrProdComposicao", "span", table).innerHTML += " (Valor m²: " + retorno[21] + ")";
            }
            else
            {
                FindControl("hdf_AmbIdProdComposicao", "input", table).value = retorno[1];
                FindControl("lbl_DescrAmbComposicao", "span", table).innerHTML = retorno[2];
                FindControl("hdf_DescrAmbienteComposicao", "input", table).value = retorno[2];
            }
        }
        catch (err) {
            alert(err);
        }

        produtoAmbienteComposicao = false;
    }

    function calculaTamanhoMaximoComposicao(control)
    {
        if (FindControl("lbl_CodProdComposicaoIns", "span") == null)
            return;

        var table = buscaTable(control);

        var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;
        var codInterno = FindControl("lbl_CodProdComposicaoIns", "span", table).innerHTML;
        var totM2 = FindControl("lbl_TotM2ComposicaoIns", "span", table).innerHTML;
        var idProdPed = FindControl("hdf_ProdPedComposicao", "input", table) != null ? FindControl("hdf_ProdPedComposicao", "input", table).value : 0;

        var tamanhoMaximo = CadPedido.GetTamanhoMaximoProduto(idPedido, codInterno, totM2, idProdPed).value.split(";");
        tamanhoMaximo = tamanhoMaximo[0] == "Ok" ? parseFloat(tamanhoMaximo[1].replace(",", ".")) : 0;

        FindControl("hdf_TamanhoMaximoObraComposicao", "input", table).value = tamanhoMaximo;
    }

    function atualizaValMinComposicao(control)
    {
        var table = buscaTable(control);

        if (parseFloat(FindControl("hdf_TamanhoMaximoObraComposicao", "input", table).value.replace(",", ".")) == 0)
        {
            var codInterno = FindControl("txt_CodProdComposicaoIns", "input", table);
            codInterno = codInterno != null ? codInterno.value : FindControl("lbl_CodProdComposicaoIns", "span", table).innerHTML;

            var idPedido = '<%= Request["idPedido"] %>';
            var tipoPedido = FindControl("hdfTipoPedido", "input").value;
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var tipoVenda = FindControl("hdfTipoVenda", "input").value;
            var altura = FindControl("txt_AlturaComposicaoIns", "input", table).value;
            var idProdPed = FindControl("hdf_ProdPedComposicao", "input", table);
            idProdPed = idProdPed != null ? idProdPed.value : "";

            var controleDescQtde = FindControl("_divDescontoQtde", "div", table).id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

            var percDescontoQtde = controleDescQtde.PercDesconto();

            FindControl("hdf_ValMinComposicao", "input", table).value = CadPedido.GetValorMinimo(codInterno, tipoPedido, tipoEntrega, tipoVenda,
                idCliente, cliRevenda, idProdPed, percDescontoQtde, idPedido, altura).value;
        }
        else
            FindControl("hdf_ValMinComposicao", "input", table).value = FindControl("txt_ValorComposicaoIns", "input", table).value;
    }

    // Função chamada pelo popup de escolha da Aplicação do produto
    function setAplComposicao(idAplicacao, codInterno, idProdPed) {

        var tr = FindControl("prodPed_" + idProdPed, "tr");

        if (tr == null || tr == undefined)
            setAplComposicaoChild(idAplicacao, codInterno, idProdPed);
        else
        {
            if (!aplAmbienteComposicao)
            {
                FindControl("txt_AplComposicaoIns", "input", tr).value = codInterno;
                FindControl("hdf_IdAplicacaoComposicao", "input", tr).value = idAplicacao;
            }
            else
            {
                FindControl("txt_AmbAplComposicaoIns", "input", tr).value = codInterno;
                FindControl("hdf_AmbIdAplicacaoComposicao", "input", tr).value = idAplicacao;
            }

            aplAmbienteComposicao = false;
        }
    }

    function loadAplComposicao(control, codInterno) {

        var tr = buscaTable(control);

        var idProdPed = FindControl("hdf_IdProdPed", "input", tr).value;

        if (codInterno == "") {
            setAplComposicao("", "", idProdPed);
            return false;
        }

        try {
            var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Aplicação. Ajax Error.");
                setAplComposicao("", "", idProdPed);
                return false
            }

            response = response.split("\t");

            if (response[0] == "Erro") {
                alert(response[1]);
                setAplComposicao("", "", idProdPed);
                return false;
            }

            setAplComposicao(response[1], response[2], idProdPed);
        }
        catch (err) {
            alert(err);
        }
    }

    // Função chamada pelo popup de escolha do Processo do produto
    function setProcComposicao(idProcesso, codInterno, codAplicacao, idProdPed) {
        var codInternoProd = "";
        var codAplicacaoAtual = "";

        var tr = FindControl("prodPed_" + idProdPed, "tr");

        if (tr == null || tr == undefined)
            setProcComposicaoChild(idProcesso, codInterno, codAplicacao, idProdPed);
        else
        {
            var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdf_IdProdComposicao", "input", tr).value);
            var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

            if(idSubgrupo.value != "" && retornoValidacao.value == "False" && (FindControl("txt_ProcComposicaoIns", "input", tr) != null && FindControl("txt_ProcComposicaoIns", "input", tr).value != ""))
            {
                FindControl("txt_ProcComposicaoIns", "input", tr).value = "";
                alert("Este processo não pode ser selecionado para este produto.")
                return false;
            }

            var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, FindControl("hdfIdPedido", "input").value);

            if(verificaEtiquetaProc.error != null){

                if (!procAmbienteComposicao && FindControl("txt_ProcComposicaoIns", "input", tr) != null)
                {
                    FindControl("txt_ProcComposicaoIns", "input", tr).value = "";
                    FindControl("hdf_IdProcessoComposicao", "input", tr).value = "";
                }
                else
                {
                    FindControl("txt_AmbProcComposicaoIns", "input", tr).value = "";
                    FindControl("hdf_AmbIdProcessoComposicao", "input", tr).value = "";
                }

                setAplComposicao("", "", idProdPed);

                alert(verificaEtiquetaProc.error.description);
                return false;
            }

            if (!procAmbienteComposicao)
            {
                FindControl("txt_ProcComposicaoIns", "input", tr).value = codInterno;
                FindControl("hdf_IdProcessoComposicao", "input", tr).value = idProcesso;

                if (FindControl("txt_CodProdComposicaoIns", "input", tr) != null)
                    codInternoProd = FindControl("txt_CodProdComposicaoIns", "input", tr).value;
                else
                    codInternoProd = FindControl("lbl_CodProdComposicaoIns", "span", tr).innerHTML;

                codAplicacaoAtual = FindControl("txt_AplComposicaoIns", "input", tr).value;
            }
            else
            {
                FindControl("txt_AmbProcComposicaoIns", "input", tr).value = codInterno;
                FindControl("hdf_AmbIdProcessoComposicao", "input", tr).value = idProcesso;

                codInternoProd = FindControl("txt_CodAmbComposicao", "input", tr).value;
                codAplicacaoAtual = FindControl("txt_AmbAplComposicaoIns", "input", tr).value;
            }

            if (((codAplicacao && codAplicacao != "") ||
                (codInternoProd != "" && CadPedido.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) &&
                (codAplicacaoAtual == null || codAplicacaoAtual == ""))
            {
                aplAmbienteComposicao = procAmbienteComposicao;
                loadAplComposicao(tr, codAplicacao);
            }

            procAmbienteComposicao = false;
        }
    }

    function loadProcComposicao(control, codInterno) {

        var tr = buscaTable(control);

        var idProdPed = FindControl("hdf_IdProdPed", "input", tr).value;

        if (codInterno == "") {
            setProcComposicao("", "", "", idProdPed);
            return false;
        }

        try {
            var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Processo. Ajax Error.");
                setProcComposicao("", "", "", idProdPed);
                return false
            }

            response = response.split("\t");

            if (response[0] == "Erro") {
                alert(response[1]);
                setProcComposicao("", "", "", idProdPed);
                return false;
            }

            setProcComposicao(response[1], response[2], response[3], idProdPed);
        }
        catch (err) {
            alert(err);
        }
    }

    var dadosCalcM2ProdComposicao = {
        IdProd: 0,
        Altura: 0,
        Largura: 0,
        Qtde: 0,
        QtdeAmbiente: 0,
        TipoCalc: 0,
        Cliente: 0,
        Redondo: false,
        NumBenef: 0
    };

    // Calcula em tempo real a metragem quadrada do produto
    function calcM2ProdComposicao(control) {
        try {

            var table = buscaTable(control);

            var idProd = FindControl("hdf_IdProdComposicao", "input", table).value;
            var altura = FindControl("txt_AlturaComposicaoIns", "input", table).value;
            var largura = FindControl("txt_LarguraComposicaoIns", "input", table).value;

            var qtde = FindControl("txt_QtdeComposicaoIns", "input", table).value;
            var isVidro = FindControl("hdf_IsVidroComposicao", "input", table).value == "true";
            var tipoCalc = FindControl("hdf_TipoCalcComposicao", "input", table).value;

            if (altura == "" || largura == "" || qtde == "" || altura == "0" || (tipoCalc != 2 && tipoCalc != 10 && !usarBenefTodosGrupos)) {
                if (qtde != "" && qtde != "0")
                    calcTotalProdComposicao(table);

                return false;
            }

            var redondo = FindControl("Redondo_chkSelecao", "input", table) != null && FindControl("Redondo_chkSelecao", "input", table).checked;
            var numBenef = "";

            if (FindControl("Redondo_chkSelecao", "input", table) != null)
            {
                numBenef = FindControl("Redondo_chkSelecao", "input", table).id
                numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
                numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
                numBenef = eval(numBenef).NumeroBeneficiamentos();
            }

            var esp = FindControl("txt_EspessuraComposicao", "input", table) != null ? FindControl("txt_EspessuraComposicao", "input", table).value : 0;

            // Calcula metro quadrado
            var idCliente = FindControl("hdfIdCliente", "input").value;

            if ((idProd != dadosCalcM2ProdComposicao.IdProd && idProd > 0) || (altura != dadosCalcM2ProdComposicao.Altura && altura > 0) ||
                (largura != dadosCalcM2ProdComposicao.Largura) || (qtde != dadosCalcM2ProdComposicao.Qtde && qtde > 0) ||
                (tipoCalc != dadosCalcM2ProdComposicao.TipoCalc && tipoCalc > 0) || (idCliente != dadosCalcM2ProdComposicao.Cliente) || (redondo != dadosCalcM2ProdComposicao.Redondo) ||
                (numBenef != dadosCalcM2ProdComposicao.NumBenef))
            {
                FindControl("lbl_TotM2ComposicaoIns", "span", table).innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, "false").value;
                FindControl("hdf_TotM2CalcComposicao", "input", table).value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
                FindControl("hdf_TotM2CalcSemChapaComposicao", "input", table).value = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
                FindControl("lbl_TotM2CalcComposicao", "span", table).innerHTML = FindControl("hdf_TotM2CalcComposicao", "input", table).value.replace('.', ',');

                if (FindControl("hdf_TotM2ComposicaoIns", "input", table) != null)
                    FindControl("hdf_TotM2ComposicaoIns", "input", table).value = FindControl("lbl_TotM2ComposicaoIns", "span", table).innerHTML.replace(',', '.');
                else if (FindControl("hdf_TotMComposicao", "input", table) != null)
                    FindControl("hdf_TotMComposicao", "input", table).value = FindControl("lbl_TotM2ComposicaoIns", "span", table).innerHTML.replace(',', '.');

                dadosCalcM2ProdComposicao = {
                    IdProd: idProd,
                    Altura: altura,
                    Largura: largura,
                    Qtde: qtde,
                    TipoCalc: tipoCalc,
                    Cliente: idCliente,
                    Redondo: redondo,
                    NumBenef: numBenef
                };
            }

            calcTotalProdComposicao(table);
        }
        catch (err) {
            alert(err);
        }
    }

    function GetAdicionalAlturaChapa(){
        var idPedido = FindControl("hdfIdPedido", "input").value;
        var idProd = FindControl("hdf_IdProdComposicao", "input", table).value;
        var altura = FindControl("txt_AlturaComposicaoIns", "input", table).value;
        var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
        var idCliente = FindControl("hdfIdCliente", "input").value;
        var cliRevenda = FindControl("hdfCliRevenda", "input").value;
        var controleDescQtde = FindControl("_divDescontoQtde", "div", table).id;
        controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
        var percDescontoQtde = controleDescQtde.PercDesconto();

        var retorno = MetodosAjax.GetValorTabelaProduto(idProd, tipoEntrega, idCliente,
            cliRevenda, pedidoReposicao, percDescontoQtde, idPedido, "", "", altura);

        if (retorno.error != null) {
                alert(retorno.error.description);
                return;
        }
        else if(retorno == null){
            alert("Erro na recuperação do valor de tabela do produto.");
            return;
        }

        var valorIns = FindControl("txt_ValorComposicaoIns", "input");

        if(valorIns != null){
            valorIns.value = retorno.value.replace(".", ",");
        }
        else{
            alert("Não foi possível encontrar o controle 'txt_ValorComposicaoIns'");
            return false;
        }
    }

    // Calcula em tempo real o valor total do produto
    function calcTotalProdComposicao(control) {
        try {
            var table = buscaTable(control);

            var valorIns = FindControl("txt_ValorComposicaoIns", "input", table).value;

            if (valorIns == "")
                return;

            var totM2 = FindControl("lbl_TotM2ComposicaoIns", "span", table).innerHTML;
            var totM2Calc = new Number(FindControl("hdf_TotM2CalcComposicao", "input", table).value.replace(',', '.')).toFixed(2);
            var total = new Number(valorIns.replace(',', '.')).toFixed(2);
            var qtde = new Number(FindControl("txt_QtdeComposicaoIns", "input", table).value.replace(',', '.'));
            var altura = new Number(FindControl("txt_AlturaComposicaoIns", "input", table).value.replace(',', '.'));
            var largura = new Number(FindControl("txt_LarguraComposicaoIns", "input", table).value.replace(',', '.'));
            var tipoCalc = FindControl("hdf_TipoCalcComposicao", "input", table).value;
            var m2Minimo = FindControl("hdf_M2MinimoComposicao", "input", table).value;

            var controleDescQtde = FindControl("_divDescontoQtde", "div", table).id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

            var percDesconto = controleDescQtde.PercDesconto();
            var percDescontoAtual = controleDescQtde.PercDescontoAtual();

            var retorno = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, FindControl("txtAlturaIns", "input"), largura, true, tipoCalc, 0, 0, percDescontoAtual, percDesconto);
            if (retorno != "")
                FindControl("lbl_TotalComposicaoIns", "span", table).innerHTML = retorno;
        }
        catch (err) {
            alert(err);
        }
    }

    // Se o produto sendo adicionado for ferragem e se a empresa for charneca, informa se qtd vendida
    // do produto existe no estoque
    function verificaEstoqueComposicao(control) {

        var table = buscaTable(control);

        var txtQtd = FindControl("txt_QtdeComposicaoIns", "input", table).value;
        var txtAltura = FindControl("txt_AlturaComposicaoIns", "input", table).value;
        var tipoCalc = FindControl("hdf_TipoCalcComposicao", "input", table).value;
        var idPedido = FindControl("hdfIdPedido", "input").value;
        var totM2 = FindControl("lbl_TotM2ComposicaoIns", "span", table).innerHTML;
        var isCalcAluminio = tipoCalc == 4 || tipoCalc == 6 || tipoCalc == 7 || tipoCalc == 9;
        var isCalcM2 = tipoCalc == 2 || tipoCalc == 10;

        // Se for cálculo por barra de 6m, multiplica a qtd pela altura
        if (isCalcAluminio)
            txtQtd = parseInt(txtQtd) * parseFloat(txtAltura.toString().replace(',', '.'));
        else if (isCalcM2)
        {
            if (totM2 == "")
                return;

            txtQtd = totM2;
        }

        var estoqueMenor = txtQtd != "" && parseInt(txtQtd) > parseInt(qtdEstoqueComposicao);
        if (estoqueMenor)
        {
            if (qtdEstoqueComposicao == 0)
                alert("Não há nenhuma peça deste produto no estoque.");
            else
                alert("Há apenas " + qtdEstoqueComposicao + " " + (isCalcM2 ? "m²" : isCalcAluminio ? "ml (" + parseFloat(qtdEstoqueComposicao / 6).toFixed(2) + " barras)" : "peça(s)") + " deste produto no estoque.");

                FindControl("txt_QtdeComposicaoIns", "input", table).value = "";
        }

        var exibirPopup = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ExibePopupVidrosEstoque.ToString().ToLower() %>;
        if (exibirPopup && exibirMensagemEstoqueComposicao && (qtdEstoqueMensagemComposicao <= 0 || estoqueMenor))
            openWindow(400, 600, "../Utils/DadosEstoque.aspx?idProd=" + FindControl("hdf_IdProdComposicao", "input", table).value + "&idPedido=" + idPedido);
    }

    function exibirBenefComposicao(botao, id) {
        for (iTip = 0; iTip < 2; iTip++) {
            TagToTip('tb_ConfigVidroComposicao_' + id, FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true,
                CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9 - getTableWidth('tb_ConfigVidroComposicao_' + id), -41 - getTableHeight('tb_ConfigVidroComposicao_' + id)]);
        }
    }

    function setValorTotalComposicao(valor, custo, idProdPed) {

        if (getNomeControleBenefComposicao() != null) {
            if (exibirControleBenef(getNomeControleBenefComposicao())) {
                var tr = FindControl("prodPed_" + idProdPed, "tr");
                var lblValorBenef = FindControl("lbl_ValorBenefComposicao", "span", tr);
                lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
            }
        }
    }

    function selProcComposicao(control) {

        var tr = buscaTable(control);

        var idProdPed = FindControl("hdf_IdProdPed", "input", tr).value;
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdf_IdProdComposicao", "input", tr).value);

        openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx?idProdPed=' + idProdPed+'&idSubgrupo=' + idSubgrupo.value);

        return false;
    }

    function selAplComposicao(control) {

        var tr = buscaTable(control);

        var idProdPed = FindControl("hdf_IdProdPed", "input", tr).value;

        openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx?idProdPed=' + idProdPed);

        return false;
    }

    function getProdutoComposicao(idPedido, control) {

        var tr = buscaTable(control);

        var idProdPed = FindControl("hdf_IdProdPed", "input", tr).value;
        openWindow(450, 700, '../Utils/SelProd.aspx?idPedido=' + idPedido + '&idProdPed=' + idProdPed + '&callback=prodComposicao');
    }

    function setProdutoComposicao(codInterno, idPedido, idProdPed) {

        var tr = FindControl("prodPed_" + idProdPed, "tr");

        FindControl("txt_CodProdComposicaoIns", "input", tr).value = codInterno;
        loadProdutoComposicao(codInterno, tr);
    }

    function obrigarProcAplComposicao(control)
    {
        var table = buscaTable(control);

        var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
        var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
        var isVidroRoteiro = dadosProduto.Grupo == 1 && <%= UtilizarRoteiroProducao().ToString().ToLower() %>;

        if (dadosProduto.IsChapaVidro)
            return true;

        if (isVidroRoteiro || (isObrigarProcApl && isVidroBenef))
        {
            if (FindControl("txt_AplComposicaoIns", "input", table) != null && FindControl("txt_AplComposicaoIns", "input", table).value == "")
            {
                if (isVidroRoteiro && !isObrigarProcApl) {
                    alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                    return false;
                }

                alert("Informe a aplicação.");
                return false;
            }

            if (FindControl("txt_ProcComposicaoIns", "input", table) != null && FindControl("txt_ProcComposicaoIns", "input", table).value == "")
            {
                if (isVidroRoteiro && !isObrigarProcApl) {
                    alert("É obrigatório informar o processo caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                    return false;
                }

                alert("Informe o processo.");
                return false;
            }
        }

        return true;
    }

    function validaTamanhoMaxComposicao(control)
    {
        var table = buscaTable(control);

        var tamanhoMaximo = parseFloat(FindControl("hdf_TamanhoMaximoObraComposicao", "input", table).value.replace(",", "."));
        if (tamanhoMaximo > 0)
        {
            var totM2 = parseFloat(FindControl("lbl_TotM2ComposicaoIns", "span", table).innerHTML.replace(",", "."));
            if (totM2 > tamanhoMaximo)
            {
                alert("O total de m² da peça ultrapassa o máximo definido no pagamento antecipado. Tamanho máximo restante: " + tamanhoMaximo.toString().replace(".", ",") + " m²");
                return false;
            }
        }

        return true;
    }

    var saveProdComposicaoClicked = false;

    // Chamado quando um produto está para ser inserido no pedido
    function onSaveProdComposicao(control, idTbConfigVidro) {
        if (!validate("produtoComposicao"))
            return false;

        if (saveProdComposicaoClicked == true)
            return false;

        saveProdComposicaoClicked = true;

        var tr = buscaTable(control);

        atualizaValMinComposicao(tr);

        var codProd = FindControl("txt_CodProdComposicaoIns", "input", tr).value;
        var idProd = FindControl("hdf_IdProdComposicao", "input", tr).value;
        var valor = FindControl("txt_ValorComposicaoIns", "input", tr).value;
        var qtde = FindControl("txt_QtdeComposicaoIns", "input", tr).value;
        var altura = FindControl("txt_AlturaComposicaoIns", "input", tr).value;
        var largura = FindControl("txt_LarguraComposicaoIns", "input", tr).value;
        var valMin = FindControl("hdf_ValMinComposicao", "input", tr).value;
        var tipoVenda = FindControl("hdfTipoVenda", "input");
        tipoVenda = tipoVenda != null ? tipoVenda.value : 0;

        if (codProd == "") {
            alert("Informe o código do produto.");
            saveProdComposicaoClicked = false;
            return false;
        }

        // Verifica se foi clicado no aplicar na telinha de beneficiamentos
        if (FindControl("tb_ConfigVidroComposicao_" + idTbConfigVidro, "table").style.display == "block")
        {
            alert("Aplique as alterações no beneficiamento antes de salvar o item.");
            return false;
        }

        if ( tipoVenda != 3 && tipoVenda != 4 && (valor == "" || parseFloat(valor.replace(",", ".")) == 0)) {
            alert("Informe o valor vendido.");
            saveProdComposicaoClicked = false;
            return false;
        }

        if (qtde == "0" || qtde == "") {
            alert("Informe a quantidade.");
            saveProdComposicaoClicked = false;
            return false;
        }

        valMin = new Number(valMin.replace(',', '.'));
        if (!FindControl("txt_ValorComposicaoIns", "input", tr).disabled && new Number(valor.replace(',', '.')) < valMin) {
            alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
            saveProdComposicaoClicked = false;
            return false;
        }

        if (FindControl("txt_AlturaComposicaoIns", "input", tr).disabled == false) {
            if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                alert("Informe a altura.");
                saveProdComposicaoClicked = false;
                return false;
            }

            if (FindControl("hdf_IsAluminioComposicao", "input", tr).value == "true" && altura > parseInt(comprimentoMaxAluminio)) {
                alert("A altura deve ser no máximo " + comprimentoMaxAluminio + "ml.");
                saveProdComposicaoClicked = false;
                return false;
            }
        }

        // Se o textbox da largura estiver habilitado, deverá ser informada
        if (FindControl("txt_LarguraComposicaoIns", "input", tr).disabled == false && largura == "") {
            alert("Informe a largura.");
            saveProdComposicaoClicked = false;
            return false;
        }

        if (!obrigarProcAplComposicao(tr))
        {
            saveProdComposicaoClicked = false;
            return false;
        }

        if (!validaTamanhoMaxComposicao(tr))
        {
            saveProdComposicaoClicked = false;
            return false;
        }

        // Calcula o ICMS do produto
        var aliquota = FindControl("hdf_AliquotaIcmsProdComposicao", "input", tr);
        var icms = FindControl("hdf_ValorIcmsProdComposicao", "input", tr);
        icms.value = aliquota.value > 0 ? parseFloat(valor) * (parseFloat(aliquota.value) / 100) : 0;
        icms.value = icms.value.toString().replace('.', ',');

        if (FindControl("txt_EspessuraComposicao", "input", tr) != null)
            FindControl("txt_EspessuraComposicao", "input", tr).disabled = false;

        FindControl("txt_AlturaComposicaoIns", "input", tr).disabled = false;
        FindControl("txt_LarguraComposicaoIns", "input", tr).disabled = false;
        FindControl("txt_ValorComposicaoIns", "input", tr).disabled = false;
        FindControl("txt_QtdeComposicaoIns", "input", tr).disabled = false;

        return true;
    }

    // Função chamada quando o produto está para ser atualizado
    function onUpdateProdComposicao(control, idTbConfigVidro) {
        if (!validate("produtoComposicao"))
            return false;

        var table = buscaTable(control);

        atualizaValMinComposicao(table);

        var valor = FindControl("txt_ValorComposicaoIns", "input", table).value;
        var qtde = FindControl("txt_QtdeComposicaoIns", "input", table).value;
        var altura = FindControl("txt_AlturaComposicaoIns", "input", table).value;
        var idProd = FindControl("hdf_IdProdComposicao", "input", table).value;
        var codInterno = FindControl("hdf_CodInternoComposicao", "input", table).value;
        var valMin = FindControl("hdf_ValMinComposicao", "input", table).value;
        var tipoVenda = FindControl("hdfTipoVenda", "input");
        tipoVenda = tipoVenda != null ? tipoVenda.value : 0;

        valMin = new Number(valMin.replace(',', '.'));
        if (!FindControl("txt_ValorComposicaoIns", "input", table).disabled && new Number(valor.replace(',', '.')) < valMin) {
            alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
            return false;
        }

        // Verifica se foi clicado no aplicar na telinha de beneficiamentos
        if (FindControl("tb_ConfigVidroComposicao_" + idTbConfigVidro, "table").style.display == "block")
        {
            alert("Aplique as alterações no beneficiamento antes de salvar o item.");
            return false;
        }

        var tipoPedido = FindControl("hdfTipoPedido", "input").value;
        var subgrupoProdComposto = CadPedido.SubgrupoProdComposto(idProd).value;

        if (tipoVenda != 3 && tipoVenda != 4 && (valor == "" || parseFloat(valor.replace(",", ".")) == 0) && !(tipoPedido == 1 && subgrupoProdComposto))
        {
            alert("Informe o valor vendido.");
            return false;
        }
        else if (qtde == "0" || qtde == "") {
            alert("Informe a quantidade.");
            return false;
        }
        else if (FindControl("txt_AlturaComposicaoIns", "input", table).disabled == false) {
            if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                alert("Informe a altura.");
                return false;
            }

            if (FindControl("hdf_IsAluminioComposicao", "input", table).value == "true" && altura > parseInt(comprimentoMaxAluminio)) {
                alert("A altura deve ser no máximo " + comprimentoMaxAluminio + "ml.");
                return false;
            }
        }

        if (!obrigarProcAplComposicao(table))
            return false;

        if (!validaTamanhoMaxComposicao(table))
            return false;

        // Calcula o ICMS do produto
        var aliquota = FindControl("hdf_AliquotaIcmsProdComposicao", "input", table);
        var icms = FindControl("hdf_ValorIcmsProdComposicao", "input", table);
        icms.value = parseFloat(valor) * (parseFloat(aliquota.value) / 100);
        icms.value = icms.value.toString().replace('.', ',');

        if (FindControl("txt_EspessuraComposicao", "input", table) != null)
            FindControl("txt_EspessuraComposicao", "input", table).disabled = false;

        FindControl("txt_AlturaComposicaoIns", "input", table).disabled = false;
        FindControl("txt_LarguraComposicaoIns", "input", table).disabled = false;
        FindControl("txt_ValorComposicaoIns", "input", table).disabled = false;
        FindControl("txt_QtdeComposicaoIns", "input", table).disabled = false;

        return true;
    }

    function exibirProdsComposicaoChild(botao, idProdPed) {

        var grdProds = FindControl("grdProdutosComposicao", "table");

        if(grdProds == null)
            return;

        var linha = document.getElementById("prodPedChild_" + idProdPed);
        var exibir = linha.style.display == "none";
        linha.style.display = exibir ? "" : "none";
        botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
        botao.title = (exibir ? "Esconder" : "Exibir") + " Produtos da Composição";
    }

</script>

<asp:GridView GridLines="None" ID="grdProdutosComposicao" runat="server" AllowPaging="True"
    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXPed" CssClass="gridStyle"
    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
    DataKeyNames="IdProdPed" OnRowDeleted="grdProdutos_RowDeleted" ShowFooter="True"
    OnRowCommand="grdProdutos_RowCommand" OnPreRender="grdProdutos_PreRender" PageSize="12"
    OnRowUpdated="grdProdutos_RowUpdated">
    <FooterStyle Wrap="True" />
    <Columns>
        <asp:TemplateField>
            <FooterTemplate>
                <select id="drpFooterVisible" style="display: none"></select>
            </FooterTemplate>
            <ItemTemplate>
                <asp:ImageButton ID="lnk_EditComposicao" runat="server" CommandName="Edit" OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>' ImageUrl="~/Images/Edit.gif" />
                <asp:ImageButton ID="imb_ExcluirComposicao" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" Visible='<%# Eval("DeleteVisible") %>'
                    OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(false); return false" : "if (!confirm(\"Deseja remover esse produto do pedido?\")) return false" %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:ImageButton ID="imb_AtualizarComposicao" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar"
                    OnClientClick='<%# "if (!onUpdateProdComposicao(this, &#39;" + IdProdPed + "_" + Eval("IdProdPed") + "&#39;)) return false;" %>' />
                <asp:ImageButton ID="imb_CancelarComposicao" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />

                <asp:HiddenField ID="hdf_ProdPedComposicao" runat="server" Value='<%# Eval("IdProdPed") %>' />
                <asp:HiddenField ID="hdf_IdPedidoComposicao" runat="server" Value='<%# Bind("IdPedido") %>' />
                <asp:HiddenField ID="hdf_IdProdComposicao" runat="server" Value='<%# Bind("IdProd") %>' />
                <asp:HiddenField ID="hdf_CodInternoComposicao" runat="server" Value='<%# Eval("CodInterno") %>' />
                <asp:HiddenField ID="hdf_ValMinComposicao" runat="server" />
                <asp:HiddenField ID="hdf_IsVidroComposicao" runat="server" Value='<%# Eval("IsVidro") %>' />
                <asp:HiddenField ID="hdf_IsAluminioComposicao" runat="server" Value='<%# Eval("IsAluminio") %>' />
                <asp:HiddenField ID="hdf_M2MinimoComposicao" runat="server" Value='<%# Eval("M2Minimo") %>' />
                <asp:HiddenField ID="hdf_TipoCalcComposicao" runat="server" Value='<%# Eval("TipoCalc") %>' />
                <asp:HiddenField ID="hdf_IdItemProjetoComposicao" runat="server" Value='<%# Bind("IdItemProjeto") %>' />
                <asp:HiddenField ID="hdf_IdMaterItemProjComposicao" runat="server" Value='<%# Bind("IdMaterItemProj") %>' />
                <asp:HiddenField ID="hdf_IdAmbientePedidoComposicao" runat="server" Value='<%# Bind("IdAmbientePedido") %>' />
                <asp:HiddenField ID="hdf_AliquotaIcmsProdComposicao" runat="server" Value='<%# Bind("AliqIcms") %>' />
                <asp:HiddenField ID="hdf_ValorIcmsProdComposicao" runat="server" Value='<%# Bind("ValorIcms") %>' />
                <asp:HiddenField ID="hdf_ValorTabelaOrcamentoComposicao" runat="server" Value='<%# Bind("ValorTabelaOrcamento") %>' />
                <asp:HiddenField ID="hdf_ValorTabelaPedidoComposicao" runat="server" Value='<%# Bind("ValorTabelaPedido") %>' />
                <asp:HiddenField ID="hdf_IdProdPedParent" runat="server" Value='<%# Bind("IdProdPedParent") %>' />
                <asp:HiddenField ID="hdf_PodeEditarComposicao" runat="server" Value='<%# Eval("PodeEditarComposicao") %>' />
                <asp:HiddenField ID="hdf_IdProdBaixaEst" runat="server" Value='<%# Bind("IdProdBaixaEst") %>' />
            </EditItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
            <ItemTemplate>
                <asp:Label ID="lbl_CodProdComposicao" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Label ID="lbl_CodProdComposicaoIns" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
            <ItemTemplate>
                <asp:Label ID="lbl_ProdutoComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Label ID="lbl_DescrProdComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                <asp:HiddenField ID="hdf_CustoProdComposicao" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txt_CodProdComposicaoIns" runat="server" onblur="loadProdutoComposicao(this.value, this);"
                    onkeydown="if (isEnter(event)) loadProdutoComposicao(this.value, this);"
                    onkeypress="return !(isEnter(event));" Width="50px"></asp:TextBox>

                <asp:Label ID="lbl_DescrProdComposicao" runat="server"></asp:Label>

                <input id="img_PesqProd_Composicao" type="image" onclick='<%# "getProdutoComposicao("  + (Request["idPedido"] != null ? Request["idPedido"] : "0") + ", this); return false;" %>' src="../Images/Pesquisar.gif" />

                <asp:HiddenField ID="hdf_ValMinComposicao" runat="server" />
                <asp:HiddenField ID="hdf_IsVidroComposicao" runat="server" />
                <asp:HiddenField ID="hdf_TipoCalcComposicao" runat="server" />
                <asp:HiddenField ID="hdf_IsAluminioComposicao" runat="server" />
                <asp:HiddenField ID="hdf_M2MinimoComposicao" runat="server" />
                <asp:HiddenField ID="hdf_AliquotaIcmsProdComposicao" runat="server" />
                <asp:HiddenField ID="hdf_ValorIcmsProdComposicao" runat="server" />
                <asp:HiddenField ID="hdf_CustoProdComposicao" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
            <ItemTemplate>
                <asp:Label ID="lbl_QtdeComposicao" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                <asp:Label ID="lbl_QtdeAmbienteComposicao" runat="server" ></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txt_QtdeComposicaoIns" runat="server" onblur="calcM2ProdComposicao(this); return verificaEstoqueComposicao(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdf_TipoCalcComposicao', 'input').value), true);"
                    Text='<%# Bind("Qtde") %>' Width="50px"></asp:TextBox>
                <asp:Label ID="lbl_QtdeAmbienteComposicao" runat="server" ></asp:Label>
                <uc5:ctrlDescontoQtde ID="ctrl_DescontoQtdeComposicao" runat="server" Callback="calcTotalProdComposicao"
                    CallbackValorUnit="calcTotalProdComposicao" ValidationGroup="produtoComposicao" PercDescontoQtde='<%# Bind("PercDescontoQtde") %>'
                    ValorDescontoQtde='<%# Bind("ValorDescontoQtde") %>' OnLoad="ctrl_DescontoQtdeComposicao_Load" />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txt_QtdeComposicaoIns" runat="server" onkeydown="if (isEnter(event)) calcM2ProdComposicao(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdf_TipoCalcComposicao', 'input').value), true);"
                    onblur="calcM2ProdComposicao(this); return verificaEstoqueComposicao(this);" Width="50px"></asp:TextBox>
                <asp:Label ID="lbl_QtdeAmbienteComposicao" runat="server" ></asp:Label>
                <uc5:ctrlDescontoQtde ID="ctrl_DescontoQtdeComposicao" runat="server" Callback="calcTotalProdComposicao"
                    ValidationGroup="produtoComposicao" CallbackValorUnit="calcTotalProdComposicao" OnLoad="ctrl_DescontoQtdeComposicao_Load" />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
            <ItemTemplate>
                <asp:Label ID="lbl_larguraComposicao" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txt_LarguraComposicaoIns" runat="server" onblur="calcM2ProdComposicao(this);" onkeypress="return soNumeros(event, true, true);"
                    Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="50px"></asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txt_LarguraComposicaoIns" runat="server" onkeypress="return soNumeros(event, true, true);"
                    onblur="calcM2ProdComposicao(this);" Width="50px"></asp:TextBox>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
            <ItemTemplate>
                <asp:Label ID="lbl_alturaComposicao" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txt_AlturaComposicaoIns" runat="server" onblur="GetAdicionalAlturaChapa(); calcM2ProdComposicao(this); return verificaEstoqueComposicao(this);"
                    Text='<%# Bind("Altura") %>' onchange="FindControl('hdf_AlturaRealComposicao', 'input').value = this.value"
                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdf_TipoCalcComposicao', 'input').value), true);"
                    Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"></asp:TextBox>
                <asp:HiddenField ID="hdf_AlturaRealComposicao" runat="server" Value='<%# Bind("AlturaReal") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txt_AlturaComposicaoIns" runat="server" onblur="GetAdicionalAlturaChapa(); calcM2ProdComposicao(this); return verificaEstoqueComposicao(this);"
                    Width="50px" onchange="FindControl('hdf_AlturaRealComposicao', 'input').value = this.value"
                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdf_TipoCalcComposicao', 'input').value), true);"></asp:TextBox>
                <asp:HiddenField ID="hdf_AlturaRealComposicaoIns" runat="server" />
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
            <ItemTemplate>
                <asp:Label ID="lbl_totMComposicao" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Label ID="lbl_TotM2ComposicaoIns" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                <asp:HiddenField ID="hdf_TotMComposicao" runat="server" Value='<%# Eval("TotM") %>' />
                <asp:HiddenField ID="hdf_TamanhoMaximoObraComposicao" runat="server" />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lbl_TotM2ComposicaoIns" runat="server"></asp:Label>
                <asp:HiddenField ID="hdf_TamanhoMaximoObraComposicao" runat="server" />
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotM2Calc">
            <EditItemTemplate>
                <asp:Label ID="lbl_TotM2CalcComposicao" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                <asp:HiddenField ID="hdf_TotM2CalcComposicao" runat="server" Value='<%# Eval("TotM2Calc") %>' />
                <asp:HiddenField ID="hdf_TotM2CalcSemChapaComposicao" runat="server" Value='<%# Eval("TotalM2CalcSemChapaString") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lbl_TotM2CalcComposicaoIns" runat="server"></asp:Label>
                <asp:HiddenField ID="hdf_TotM2ComposicaoIns" runat="server" />
                <asp:HiddenField ID="hdf_TotM2CalcComposicaoIns" runat="server" />
                <asp:HiddenField ID="hdf_TotM2CalcSemChapaComposicaoIns" runat="server" />
            </FooterTemplate>
            <ItemTemplate>
                <asp:Label ID="lbl_Totm2CalcComposicao" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Wrap="True" />
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
            <ItemTemplate>
                <asp:Label ID="lbl_valorVendidoComposicao" runat="server" Text='<%# Bind("ValorVendido", "{0:C}") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txt_ValorComposicaoIns" runat="server" onblur="calcTotalProdComposicao(this);" onkeypress="return soNumeros(event, false, true);"
                    Text='<%# Bind("ValorVendido") %>' Width="50px" OnLoad="txt_ValorInsComposicao_Load"></asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txt_ValorComposicaoIns" runat="server" onkeydown="if (isEnter(event)) calcTotalProdComposicao(this);"
                    onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProdComposicao(this);"
                    Width="50px" OnLoad="txt_ValorInsComposicao_Load"></asp:TextBox>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
            <EditItemTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txt_ProcComposicaoIns" runat="server" onblur="procAmbienteComposicao=false; loadProcComposicao(this, this.value);"
                                onkeydown="if (isEnter(event)) { procAmbienteComposicao=false; loadProcComposicao(this, this.value); }"
                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="procAmbienteComposicao=false; return selProcComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdf_IdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txt_ProcComposicaoIns" runat="server" onblur="procAmbienteComposicao=false; loadProcComposicao(this, this.value);"
                                onkeydown="if (isEnter(event)) { procAmbienteComposicao=false; loadProcComposicao(this, this.value); }"
                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="procAmbienteComposicao=false; return selProcComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdf_IdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
            </FooterTemplate>
            <ItemTemplate>
                <asp:Label ID="lbl_CodProcessoComposicao" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
            <EditItemTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txt_AplComposicaoIns" runat="server" onblur="aplAmbienteComposicao=false; loadAplComposicao(this, this.value);"
                                onkeydown="if (isEnter(event)) { aplAmbienteComposicao=false; loadAplComposicao(this, this.value); }" onkeypress="return !(isEnter(event));"
                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="aplAmbienteComposicao=false; return selAplComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdf_IdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txt_AplComposicaoIns" runat="server" onblur="aplAmbienteComposicao=false; loadAplComposicao(this, this.value);"
                                onkeydown="if (isEnter(event)) { aplAmbienteComposicao=false; loadAplComposicao(this, this.value); }" onkeypress="return !(isEnter(event));"
                                Width="30px"></asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="aplAmbienteComposicao=false; return selAplComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdf_IdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
            </FooterTemplate>
            <ItemTemplate>
                <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ped. Cli." SortExpression="PedCli">
            <EditItemTemplate>
                <asp:TextBox ID="txt_PedCliComposicao" runat="server" MaxLength="50" Text='<%# Bind("PedCli") %>'
                    Width="50px"></asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txt_PedCliComposicao" runat="server" MaxLength="50" Width="50px"></asp:TextBox>
            </FooterTemplate>
            <ItemTemplate>
                <asp:Label ID="Label13" runat="server" Text='<%# Bind("PedCli") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Total" SortExpression="Total">
            <ItemTemplate>
                <asp:Label ID="Label7" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label>
                <asp:Label ID="Label43" runat="server" Text='<%# "(Desconto de " + Eval("PercDescontoQtde") + "%)" %>'
                    Visible='<%# (float)Eval("PercDescontoQtde") > 0 %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Label ID="lbl_TotalComposicaoIns" runat="server" Text='<%# Bind("Total") %>' Style="padding-top: 4px"></asp:Label>
            </EditItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lbl_TotalComposicaoIns" runat="server"></asp:Label>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
            <EditItemTemplate>
                <asp:Label ID="lbl_ValorBenefComposicao" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label>
            </EditItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lbl_ValorBenefComposicao" runat="server"></asp:Label>
            </FooterTemplate>
            <ItemTemplate>
                <asp:Label ID="Label11" runat="server" Text='<%# Bind("ValorBenef", "{0:C}") %>'></asp:Label>
            </ItemTemplate>
            <ItemTemplate>
            </ItemTemplate>
            <EditItemTemplate></EditItemTemplate>
            <FooterTemplate></FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField>
            <EditItemTemplate>

                <asp:ImageButton ID="lnk_BenefComposicao" runat="server" OnClientClick='<%# "exibirBenefComposicao(this, &#39;" + Eval("IdProdPedParent") + "_" + Eval("IdProdPed") + "&#39;); return false;" %>'
                    Visible='<%# Eval("BenefVisible") %>' ImageUrl="~/Images/gear_add.gif" />
                <table id="tb_ConfigVidroComposicao_<%# Eval("IdProdPedParent") + "_" + Eval("IdProdPed") %>" cellspacing="0" style="display: none;">
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txt_EspessuraComposicao" runat="server" OnDataBinding="txt_EspessuraComposicao_DataBinding"
                                            onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Bind("Espessura") %>'></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrl_BenefEditarComposicao" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>'
                                ValidationGroup="produtoComposicao" OnInit="ctrl_Benef_Load" Redondo='<%# Bind("Redondo") %>'
                                CallbackCalculoValorTotal="setValorTotalComposicao" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left"></td>
                    </tr>
                </table>

                <script type="text/javascript">
                    <%# "calculaTamanhoMaximoComposicao(" + Request["idPedido"] != null ? Request["idPedido"] : "0" + ");"  %>
                </script>

            </EditItemTemplate>
            <FooterTemplate>

                <asp:ImageButton ID="lnk_BenefComposicao" runat="server" Style="display: none;" OnClientClick='<%# "exibirBenefComposicao(this, &#39;" + IdProdPed + "_0&#39;); return false;" %>'
                    ImageUrl="~/Images/gear_add.gif" />
                <table id="tb_ConfigVidroComposicao_<%# IdProdPed + "_0" %>" cellspacing="0" style="display: none;">
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txt_EspessuraComposicao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrl_BenefInserirComposicao" runat="server" OnInit="ctrl_Benef_Load" CallbackCalculoValorTotal="setValorTotalComposicao"
                                ValidationGroup="produtoComposicao" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left"></td>
                    </tr>
                </table>
            </FooterTemplate>
            <ItemTemplate>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <FooterTemplate>
                <asp:ImageButton ID="lnk_InsProdComposicao" runat="server" OnClick="lnk_InsProdComposicao_Click" ImageUrl="../Images/ok.gif" OnClientClick='<%# "if (!onSaveProdComposicao(this, &#39;" + IdProdPed + "_0&#39;)) return false;" %>' />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <div id='<%# "imgProdsComposto_" + Eval("IdProdPed") %>'>
                    <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/box.png" ToolTip="Exibir Produtos da Composição"
                        Visible='<%# Eval("IsProdLamComposicao") %>' OnClientClick='<%# "exibirProdsComposicaoChild(this, " + Eval("IdProdPed") + "); return false"%>' />
                     <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/imagem.gif"
                        OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=pedido&idPedido=" + Eval("IdPedido") +"&idProdPed=" +  Eval("IdProdPed") +"&pecaAvulsa=" +  ((bool)Eval("IsProdLamComposicao") == false) + "\"); return false" %>'
                        ToolTip="Exibir imagem das peças" Visible='<%# (Eval("IsVidro").ToString() == "true")%>' />
                </div>
            </ItemTemplate>
            <EditItemTemplate></EditItemTemplate>
            <FooterTemplate></FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <a href="#" id="lnkObsCalc" onclick="exibirObs(<%# Eval("IdProdPed") %>, this); return false;" visible='<%# (Eval("IsVidro").ToString() == "true")%>'>
                    <img border="0" src="../../Images/blocodenotas.png" title="Observação da peça" /></a>
                <table id='tbObsCalc_<%# Eval("IdProdPed") %>' cellspacing="0" style="display: none;">
                    <tr>
                        <td align="center">
                            <asp:TextBox ID="txtObsCalc" runat="server" Width="320" Rows="4" MaxLength="500"
                                TextMode="MultiLine" Text='<%# Eval("Obs") %>'></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <input id="btnSalvarObs" onclick='setCalcObs(<%# Eval("IdProdPed") %>, this); return false;'
                                type="button" value="Salvar" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
            <EditItemTemplate></EditItemTemplate>
            <FooterTemplate></FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <tr id="prodPedChild_<%# Eval("IdProdPed") %>" style="display: none" align="center">
                    <td colspan="17">
                        <br />
                        <uc1:ctrlProdComposicaoChild runat="server" id="ctrlProdCompChild" visible='<%# Eval("IsProdLamComposicao") %>'
                            idprodped='<%# Glass.Conversoes.StrParaUint(Eval("IdProdPed").ToString()) %>' />
                        <br />
                    </td>
                </tr>
            </ItemTemplate>
            <EditItemTemplate></EditItemTemplate>
            <FooterTemplate></FooterTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerStyle CssClass="pgr"></PagerStyle>
    <EditRowStyle CssClass="edit"></EditRowStyle>
    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
</asp:GridView>

<asp:HiddenField runat="server" ID="hdf_IdProdPed" />
<asp:HiddenField ID="hdf_IdProdComposicao" runat="server" />

<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdXPed" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedido"
    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
    OnDeleted="odsProdXPed_Deleted" SelectCountMethod="GetCount" SelectMethod="GetList"
    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
    InsertMethod="Insert" UpdateMethod="UpdateComTransacao" OnUpdated="odsProdXPed_Updated">
    <SelectParameters>
        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        <asp:ControlParameter ControlID="hdfIdAmbiente" Name="idAmbientePedido" PropertyName="Value" Type="UInt32" />
        <asp:Parameter Name="prodComposicao" DefaultValue="true" />
        <asp:ControlParameter Name="idProdPedParent" ControlID="hdf_IdProdPed" PropertyName="Value" />
    </SelectParameters>
</colo:VirtualObjectDataSource>

<script type="text/javascript">

    $(document).ready(function(){
        if (FindControl("imb_AtualizarComposicao", "input") != null && FindControl("lbl_CodProdComposicaoIns", "span") != null)
            loadProdutoComposicao(FindControl("lbl_CodProdComposicaoIns", "span").innerHTML, FindControl("imb_AtualizarComposicao", "input"), false);

    })

</script>
