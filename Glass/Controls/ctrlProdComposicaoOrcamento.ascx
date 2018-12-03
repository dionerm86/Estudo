<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlProdComposicaoOrcamento.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlProdComposicaoOrcamento" %>

<%@ Register Src="ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlProdComposicaoOrcamentoChild.ascx" TagName="ctrlProdComposicaoOrcamentoChild" TagPrefix="uc1" %>
    
<script type="text/javascript">

// Guarda a quantidade disponível em estoque do produto buscado
var exibirMensagemEstoqueComposicao = false;
var qtdEstoqueMensagemComposicao = 0;    
var insertingComposicao = false;
var produtoAmbienteComposicao = false;
var loadingComposicao = true;
var nomeControleBenefComposicao = "<%= NomeControleBenefComposicao() %>";
var idOrcamento = <%= Request["idOrca"] != null ? Request["idOrca"] : "0" %>;
var vendedorAlteraValorUnitarioProduto = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.AlterarValorUnitarioProduto.ToString().ToLower() %>;
var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
var utilizarRoteiroProducao = <%= UtilizarRoteiroProducao().ToString().ToLower() %>;
var exibirPopup = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ExibePopupVidrosEstoque.ToString().ToLower() %>;

function buscaTable(control) {
    var tr = control;

    while (tr.id == "" || (tr.id.indexOf("produtoOrcamento_") == -1 && tr.nodeName.toLowerCase() != "tr")) {
        tr = tr.parentElement;
    }

    return tr;
}

function getNomeControleBenefComposicao(control) {
    nomeControleBenefComposicao = FindControl(nomeControleBenefComposicao + "_tblBenef", "table", control);

    if (nomeControleBenefComposicao == null) {
        return null;
    }

    nomeControleBenefComposicao = nomeControleBenefComposicao.id;
    return nomeControleBenefComposicao.substr(0, nomeControleBenefComposicao.lastIndexOf("_"));
}

// Carrega dados do produto com base no código do produto passado
function loadProdutoComposicao(codInterno, control) {
    if (control == null || codInterno == "") {
        return false;
    }

    var table = buscaTable(control);
    var txtValor = FindControl("txtValorComposicaoIns", "input", table);

    if (txtValor != null) {
        txtValor.disabled = vendedorAlteraValorUnitarioProduto;
    }

    try {
        var idLoja = FindControl("hdfIdLoja", "input").value;
        var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;       
        var cliRevenda = FindControl("hdfCliRevenda", "input").value;
        var idCliente = FindControl("hdfIdCliente", "input").value;
        var percComissao = getPercComissao();
        percComissao = percComissao == null ? 0 : percComissao.toString().replace('.', ',');   
        var tipoOrcamento = FindControl("hdfTipoOrcamento", "input").value;        
        var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
        controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));            
        var percDescontoQtde = controleDescQtde.PercDesconto();
        
        if (FindControl("_divDescontoQtde", "div") != null) {
            controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

            if (controleDescQtde != null) {
                percDescontoQtde = controleDescQtde.PercDesconto();
            }
        }

        var retorno = CadOrcamento.GetProduto(codInterno, tipoEntrega, cliRevenda, idCliente, percComissao, percDescontoQtde, idLoja);
            
        if (retorno.error != null) {
            if (FindControl("txtCodProdComposicao", "input", table) != null) {
                FindControl("txtCodProdComposicao", "input", table).value = "";
            }

            alert(retorno.error.description);
            return false;
        }
        
        retorno = retorno.value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);

            if (!produtoAmbienteComposicao) {
                FindControl("txtCodProdComposicao", "input", table).value = "";
            } else {
                FindControl("txtCodAmbComposicao", "input", table).value = "";
            }
                
            return false;
        }

        if (retorno[0] == "Prod") {
            FindControl("hdfIdProdutoComposicao", "input", table).value = retorno[1];

            var subgrupoProdComposto = CadOrcamento.SubgrupoProdComposto(retorno[1]).value;
            var tipoOrcamento = FindControl("hdfTipoOrcamento", "input").value;
            var alterarValor = !(tipoOrcamento == 1 && subgrupoProdComposto);
            txtValor.value = alterarValor ? retorno[3] : txtValor.value;

            FindControl("hdfIsVidroComposicao", "input", table).value = retorno[4]; // Informa se o produto é vidro
            FindControl("hdfM2MinimoComposicao", "input", table).value = retorno[5]; // Informa se o produto possui m² mínimo
            FindControl("hdfTipoCalcComposicao", "input", table).value = retorno[6]; // Verifica como deve ser calculado o produto

            // Se o campo do valor estiver desativado não precisa calcular o valor mínimo, tendo em vista que o usuário não poderá alterar.
            if (!txtValor.disabled) {
                atualizaValMinComposicao(table);
            }
                
            var tipoCalc = retorno[6];

            // Se o produto não for vidro, desabilita os textboxes largura e altura,
            // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
            // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
            var cAltura = FindControl("txtAlturaComposicaoIns", "input", table);
            var cLargura = FindControl("txtLarguraComposicaoIns", "input", table);
            cAltura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
            cLargura.disabled = CalcProd_DesabilitarLargura(tipoCalc);

            var nomeControle = getNomeControleBenefComposicao(table);

            // Zera o campo qtd para evitar que produtos calculados por mҠfiquem com quantidade decimal por exemplo (chamado 11010)
            var txtQtdProd = FindControl("txtQtdeComposicaoIns", "input", table);
                
            if (txtQtdProd != null && !loadingComposicao) {
                txtQtdProd.value = "";
            }

            if (nomeControle != null && nomeControle != undefined) {
                // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                if (retorno[4] == "true" && exibirControleBenef(nomeControle) && FindControl("lnkBenefComposicao", "input", table) != null) {
                    FindControl("txtEspessuraComposicao", "input", table).value = retorno[7];
                    FindControl("txtEspessuraComposicao", "input", table).disabled = retorno[7] != "" && retorno[7] != "0";
                }
                    
                if (FindControl("lnkBenefComposicao", "input", table) != null && nomeControle != null && nomeControle.indexOf("Inserir") > -1) {
                    FindControl("lnkBenefComposicao", "input", table).style.display = exibirControleBenef(nomeControle) ? "" : "none";
                }
            }

            FindControl("hdfAliquotaIcmsProdComposicao", "input", table).value = retorno[8].replace('.', ',');

            // O campo altura e largura devem sempre ser atribuídos pois caso seja selecionado um box e logo após seja selecionado um kit 
            // por exemplo, ao inserí-lo ele estava ficando com o campo altura, largura e m² preenchidos apesar de ser calculado por qtd
            if (retorno[9] != "" || retorno[4] == "false") {
                FindControl("txtAlturaComposicaoIns", "input", table).value = retorno[9];
                FindControl("hdfAlturaRealComposicao", "input", table).value = retorno[9];
            }
                
            if (retorno[10] != "" || retorno[4] == "false") {
                FindControl("txtLarguraComposicaoIns", "input", table).value = retorno[10];
            }
                 
            if (cAltura.disabled && FindControl("hdfAlturaRealComposicao", "input", table) != null) {
                FindControl("hdfAlturaRealComposicao", "input", table).value = cAltura.value;
            }

            var idProdOrcamento = FindControl("hdfIdProdOrcamento", "input", table).value;

            if (retorno[14] != "") {
                setAplComposicao(retorno[14], retorno[15], idProdOrcamento);
            }
                    
            if (retorno[16] != "") {
                setProcComposicao(retorno[16], retorno[17], null, idProdOrcamento);
            }
                    
            FindControl("hdfCustoProdComposicao", "input", table).value = retorno[18];

            var cPodeEditarComposicao = FindControl("hdfPodeEditarComposicao","input", table);

            if (cPodeEditarComposicao != null) {
                var podeEditarComposicao = cPodeEditarComposicao.value;
                var cQtdeComposicaoIns = FindControl("txtQtdeComposicaoIns", "input", table);

                cQtdeComposicaoIns.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cQtdeComposicaoIns.disabled;
                cLargura.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cLargura.disabled;
                cAltura.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cAltura.disabled;
            }
        }

        FindControl("lblDescrProdComposicao", "span", table).innerHTML = retorno[2];
    }
    catch (err) { alert(err); }
}

function atualizaValMinComposicao(control) {
    var idProdOrcamento = FindControl("hdfIdProdOrcamentoComposicao", "input", table);
    idProdOrcamento = idProdOrcamento > 0 ? idProdOrcamento : 0;
    var table = buscaTable(control);
    var codInterno = FindControl("txtCodProdComposicaoIns", "input", table);
    codInterno = codInterno != null ? codInterno.value : FindControl("lblCodProdComposicaoIns", "span", table).innerHTML;
    var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
    var cliRevenda = FindControl("hdfCliRevenda", "input").value;
    var idCliente = FindControl("hdfIdCliente", "input").value;
    var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
    controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));        
    var percDescontoQtde = controleDescQtde.PercDesconto();

    FindControl("hdfValMin", "input").value = CadOrcamento.GetValorMinimo(codInterno, tipoEntrega, idCliente, cliRevenda, idProdOrcamento, percDescontoQtde).value;
}

// Função chamada pelo popup de escolha da Aplicação do produto
function setAplComposicao(idAplicacao, codInterno, idProdOrcamento) {
    var tr = FindControl("produtoOrcamento_" + idProdOrcamento, "tr");

    if (tr == null || tr == undefined) {
        setAplComposicaoChild(idAplicacao, codInterno, idProdOrcamento);
    } else {
        FindControl("txtAplComposicaoIns", "input", tr).value = codInterno;
        FindControl("hdfIdAplicacaoComposicao", "input", tr).value = idAplicacao;
    }
}

function loadAplComposicao(control, codInterno) {
    var tr = buscaTable(control);
    var idProdOrcamento = FindControl("hdfIdProdOrcamento", "input", tr).value;

    if (codInterno == "") {
        setAplComposicao("", "", idProdOrcamento);
        return false;
    }
    
    try {
        var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

        if (response == null || response == "") {
            alert("Falha ao buscar Aplicação. Ajax Error.");
            setAplComposicao("", "", idProdOrcamento);
            return false
        }

        response = response.split("\t");
            
        if (response[0] == "Erro") {
            alert(response[1]);
            setAplComposicao("", "", idProdOrcamento);
            return false;
        }

        setAplComposicao(response[1], response[2], idProdOrcamento);
    }
    catch (err) { alert(err); }
}

// Função chamada pelo popup de escolha do Processo do produto
function setProcComposicao(idProcesso, codInterno, codAplicacao, idProdOrcamento) {
    var codInternoProd = "";
    var codAplicacaoAtual = "";
    var tr = FindControl("produtoOrcamento_" + idProdOrcamento, "tr");

    if (tr == null || tr == undefined) {
        setProcComposicaoChild(idProcesso, codInterno, codAplicacao, idProdOrcamento);
    } else {
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProdutoComposicao", "input", tr).value);
        var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

        if (idSubgrupo.value != "" && retornoValidacao.value == "false" && (FindControl("txtProcComposicaoIns", "input", tr) != null && FindControl("txtProcComposicaoIns", "input", tr).value != "")) {
            FindControl("txtProcComposicaoIns", "input", tr).value = "";
            alert("Este processo não pode ser selecionado para este produto.")
            return false;
        }

        var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, FindControl("hdfIdOrcamento", "input").value);
        
        if (verificaEtiquetaProc.error != null) {
            FindControl("txtProcComposicaoIns", "input", tr).value = "";
            FindControl("hdfIdProcessoComposicao", "input", tr).value = "";

            setAplComposicao("", "", idProdOrcamento);
            alert(verificaEtiquetaProc.error.description);
            return false;
        }

        FindControl("txtProcComposicaoIns", "input", tr).value = codInterno;
        FindControl("hdfIdProcessoComposicao", "input", tr).value = idProcesso;
            
        if (FindControl("txtCodProdComposicaoIns", "input", tr) != null) {
            codInternoProd = FindControl("txtCodProdComposicaoIns", "input", tr).value;
        } else {
            codInternoProd = FindControl("lblCodProdComposicaoIns", "span", tr).innerHTML;
        }
                
        codAplicacaoAtual = FindControl("txtAplComposicaoIns", "input", tr).value;
        
        if (((codAplicacao && codAplicacao != "") || (codInternoProd != "" && CadOrcamento.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) && (codAplicacaoAtual == null || codAplicacaoAtual == "")) {
            loadAplComposicao(tr, codAplicacao);
        }
    }
}

function loadProcComposicao(control, codInterno) {
    var tr = buscaTable(control);
    var idProdOrcamento = FindControl("hdfIdProdOrcamento", "input", tr).value;

    if (codInterno == "") {
        setProcComposicao("", "", "", idProdOrcamento);
        return false;
    }

    try {
        var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

        if (response == null || response == "") {
            alert("Falha ao buscar Processo. Ajax Error.");
            setProcComposicao("", "", "", idProdOrcamento);
            return false
        }

        response = response.split("\t");
            
        if (response[0] == "Erro") {
            alert(response[1]);
            setProcComposicao("", "", "", idProdOrcamento);
            return false;
        }

        setProcComposicao(response[1], response[2], response[3], idProdOrcamento);
    }
    catch (err) { alert(err); }
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
        var idProd = FindControl("hdfIdProdutoComposicao", "input", table).value;
        var altura = FindControl("txtAlturaComposicaoIns", "input", table).value;
        var largura = FindControl("txtLarguraComposicaoIns", "input", table).value;            
        var qtde = FindControl("txtQtdeComposicaoIns", "input", table).value;
        var isVidro = FindControl("hdfIsVidroComposicao", "input", table).value == "true";
        var tipoCalc = FindControl("hdfTipoCalcComposicao", "input", table).value;
            
        if (altura == "" || largura == "" || qtde == "" || altura == "0" || (tipoCalc != 2 && tipoCalc != 10 && !usarBenefTodosGrupos)) {
            if (qtde != "" && qtde != "0") {
                calcTotalProdComposicao(table);
            }

            return false;
        }

        var redondo = FindControl("Redondo_chkSelecao", "input", table) != null && FindControl("Redondo_chkSelecao", "input", table).checked;                          
        var numBenef = "";
            
        if (FindControl("Redondo_chkSelecao", "input", table) != null) {
            numBenef = FindControl("Redondo_chkSelecao", "input", table).id
            numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
            numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
            numBenef = eval(numBenef).NumeroBeneficiamentos();
        }

        var esp = FindControl("txtEspessuraComposicao", "input", table) != null ? FindControl("txtEspessuraComposicao", "input", table).value : 0;            
        // Calcula metro quadrado
        var idCliente = FindControl("hdfIdCliente", "input").value;
            
        if ((idProd != dadosCalcM2ProdComposicao.IdProd && idProd > 0) || (altura != dadosCalcM2ProdComposicao.Altura && altura > 0) ||
            (largura != dadosCalcM2ProdComposicao.Largura) || (qtde != dadosCalcM2ProdComposicao.Qtde && qtde > 0) ||
            (tipoCalc != dadosCalcM2ProdComposicao.TipoCalc && tipoCalc > 0) || (idCliente != dadosCalcM2ProdComposicao.Cliente) || (redondo != dadosCalcM2ProdComposicao.Redondo) ||
            (numBenef != dadosCalcM2ProdComposicao.NumBenef)) {
            FindControl("lblTotM2ComposicaoIns", "span", table).innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, "false").value;
            FindControl("hdfTotM2CalcComposicao", "input", table).value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
            FindControl("hdfTotM2CalcSemChapaComposicao", "input", table).value = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
            FindControl("lblTotM2CalcComposicao", "span", table).innerHTML = FindControl("hdfTotM2CalcComposicao", "input", table).value.replace('.', ',');
                
            if (FindControl("hdfTotM2ComposicaoIns", "input", table) != null) {
                FindControl("hdfTotM2ComposicaoIns", "input", table).value = FindControl("lblTotM2ComposicaoIns", "span", table).innerHTML.replace(',', '.');
            } else if (FindControl("hdfTotMComposicao", "input", table) != null) {
                FindControl("hdfTotMComposicao", "input", table).value = FindControl("lblTotM2ComposicaoIns", "span", table).innerHTML.replace(',', '.');
            }
                
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
    catch (err) { alert(err); }
}

// Calcula em tempo real o valor total do produto
function calcTotalProdComposicao(control) {
    try {
        var table = buscaTable(control);
        var valorIns = FindControl("txtValorComposicaoIns", "input", table).value;

        if (valorIns == "") {
            return;
        }

        var totM2 = FindControl("lblTotM2ComposicaoIns", "span", table).innerHTML;
        var totM2Calc = new Number(FindControl("hdfTotM2CalcComposicao", "input", table).value.replace(',', '.')).toFixed(2);
        var total = new Number(valorIns.replace(',', '.')).toFixed(2);
        var qtde = new Number(FindControl("txtQtdeComposicaoIns", "input", table).value.replace(',', '.'));
        var altura = new Number(FindControl("txtAlturaComposicaoIns", "input", table).value.replace(',', '.'));
        var largura = new Number(FindControl("txtLarguraComposicaoIns", "input", table).value.replace(',', '.'));
        var tipoCalc = FindControl("hdfTipoCalcComposicao", "input", table).value;
        var m2Minimo = FindControl("hdfM2MinimoComposicao", "input", table).value;            
        var controleDescQtde = FindControl("_divDescontoQtde", "div", table).id;
        controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));            
        var percDesconto = controleDescQtde.PercDesconto();
        var percDescontoAtual = controleDescQtde.PercDescontoAtual();            
        var retorno = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, FindControl("txtAlturaIns", "input"), largura, true, tipoCalc, 0, 0, percDescontoAtual, percDesconto);
            
        if (retorno != "") {
            FindControl("lblTotalComposicaoIns", "span", table).innerHTML = retorno;
        }
    }
    catch (err) { alert(err); }
}

function exibirBenefComposicao(botao, id) {
    for (iTip = 0; iTip < 2; iTip++) {
        TagToTip('tb_ConfigVidroComposicao_' + id, FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true,
            CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
            FIX, [botao, 9 - getTableWidth('tb_ConfigVidroComposicao_' + id), -41 - getTableHeight('tb_ConfigVidroComposicao_' + id)]);
    }
}

function setValorTotalComposicao(valor, custo, idProdOrcamento) {
    if (getNomeControleBenefComposicao() != null) {
        if (exibirControleBenef(getNomeControleBenefComposicao())) {
            var tr = FindControl("produtoOrcamento_" + idProdOrcamento, "tr");
            var lblValorBenef = FindControl("lblValorBenefComposicao", "span", tr);

            lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
        }
    }
}

function selProcComposicao(control) {
    var tr = buscaTable(control);
    var idProdOrcamento = FindControl("hdfIdProdOrcamento", "input", tr).value;
    var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProdutoComposicao", "input", tr).value);

    openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx?idProdOrcamento=' + idProdOrcamento + '&idSubgrupo=' + idSubgrupo.value);
    return false;
}

function selAplComposicao(control) {
    var tr = buscaTable(control);
    var idProdOrcamento = FindControl("hdfIdProdOrcamento", "input", tr).value;

    openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx?idProdOrcamento=' + idProdOrcamento);
    return false;
}

function getProdutoComposicao(idOrcamento, control) {
    var tr = buscaTable(control);
    var idProdOrcamento = FindControl("hdfIdProdOrcamento", "input", tr).value;

    openWindow(450, 700, '../Utils/SelProd.aspx?idOrcamento=' + idOrcamento + '&idProdOrcamento=' + idProdOrcamento + '&callback=prodComposicao');
}

function setProdutoComposicao(codInterno, idOrcamento, idProdOrcamento) {
    var tr = FindControl("produtoOrcamento_" + idProdOrcamento, "tr");
    FindControl("txtCodProdComposicaoIns", "input", tr).value = codInterno;

    loadProdutoComposicao(codInterno, tr);
}

function obrigarProcAplComposicao(control) {
    var table = buscaTable(control);
    var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
    var isVidroRoteiro = dadosProduto.Grupo == 1 && utilizarRoteiroProducao;

    if (dadosProduto.IsChapaVidro) {
        return true;
    }

    if (isVidroRoteiro || (isObrigarProcApl && isVidroBenef)) {
        if (FindControl("txtAplComposicaoIns", "input", table) != null && FindControl("txtAplComposicaoIns", "input", table).value == "") {
            if (isVidroRoteiro && !isObrigarProcApl) {
                alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                return false;
            }

            alert("Informe a aplicação.");
            return false;
        }
            
        if (FindControl("txtProcComposicaoIns", "input", table) != null && FindControl("txtProcComposicaoIns", "input", table).value == "") {
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

var saveProdComposicaoClicked = false;

// Chamado quando um produto está para ser inserido no orçamento
function onSaveProdComposicao(control, idTbConfigVidro) {
    if (!validate("produtoComposicao")) {
        return false;
    }
            
    if (saveProdComposicaoClicked == true) {
        return false;
    }
            
    saveProdComposicaoClicked = true;
        
    var tr = buscaTable(control);

    atualizaValMinComposicao(tr);
    
    var codProd = FindControl("txtCodProdComposicaoIns", "input", tr).value;
    var idProd = FindControl("hdfIdProdutoComposicao", "input", tr).value;
    var valor = FindControl("txtValorComposicaoIns", "input", tr).value;
    var qtde = FindControl("txtQtdeComposicaoIns", "input", tr).value;
    var altura = FindControl("txtAlturaComposicaoIns", "input", tr).value;
    var largura = FindControl("txtLarguraComposicaoIns", "input", tr).value;
    var valMin = FindControl("hdfValMinComposicao", "input", tr).value;
    var tipoVenda = FindControl("hdfTipoVenda", "input");
    tipoVenda = tipoVenda != null ? tipoVenda.value : 0;

    if (codProd == "") {
        alert("Informe o código do produto.");
        saveProdComposicaoClicked = false;
        return false;
    }
        
    // Verifica se foi clicado no aplicar na telinha de beneficiamentos
    if (FindControl("tb_ConfigVidroComposicao_" + idTbConfigVidro, "table").style.display == "block") {
        alert("Aplique as alterações no beneficiamento antes de salvar o item.");
        return false;
    }
        
    if (tipoVenda != 3 && tipoVenda != 4 && (valor == "" || parseFloat(valor.replace(",", ".")) == 0)) {
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

    if (!FindControl("txtValorComposicaoIns", "input", tr).disabled && new Number(valor.replace(',', '.')) < valMin) {
        alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
        saveProdComposicaoClicked = false;
        return false;
    }
        
    if (FindControl("txtAlturaComposicaoIns", "input", tr).disabled == false) {
        if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
            alert("Informe a altura.");
            saveProdComposicaoClicked = false;
            return false;
        }

        if (FindControl("hdfIsAluminioComposicao", "input", tr).value == "true" && altura > parseInt(comprimentoMaxAluminio)) {
            alert("A altura deve ser no máximo " + comprimentoMaxAluminio + "ml.");
            saveProdComposicaoClicked = false;
            return false;
        }            
    }
        
    // Se o textbox da largura estiver habilitado, deverá ser informada
    if (FindControl("txtLarguraComposicaoIns", "input", tr).disabled == false && largura == "") {
        alert("Informe a largura.");
        saveProdComposicaoClicked = false;
        return false;
    }
        
    if (!obrigarProcAplComposicao(tr)) {
        saveProdComposicaoClicked = false;
        return false;
    }
        
    // Calcula o ICMS do produto
    var aliquota = FindControl("hdfAliquotaIcmsProdComposicao", "input", tr);
    var icms = FindControl("hdfValorIcmsProdComposicao", "input", tr);
    icms.value = aliquota.value > 0 ? parseFloat(valor) * (parseFloat(aliquota.value) / 100) : 0;
    icms.value = icms.value.toString().replace('.', ',');
        
    if (FindControl("txtEspessuraComposicao", "input", tr) != null) {
        FindControl("txtEspessuraComposicao", "input", tr).disabled = false;
    }
        
    FindControl("txtAlturaComposicaoIns", "input", tr).disabled = false;
    FindControl("txtLarguraComposicaoIns", "input", tr).disabled = false;
    FindControl("txtValorComposicaoIns", "input", tr).disabled = false;
    FindControl("txtQtdeComposicaoIns", "input", tr).disabled = false;
        
    return true;
}

// Função chamada quando o produto está para ser atualizado
function onUpdateProdComposicao(control, idTbConfigVidro) {
    if (!validate("produtoComposicao")) {
        return false;
    }
        
    var table = buscaTable(control);

    atualizaValMinComposicao(table);
    
    var valor = FindControl("txtValorComposicaoIns", "input", table).value;
    var qtde = FindControl("txtQtdeComposicaoIns", "input", table).value;
    var altura = FindControl("txtAlturaComposicaoIns", "input", table).value;
    var idProd = FindControl("hdfIdProdutoComposicao", "input", table).value;
    var codInterno = FindControl("hdfCodInternoComposicao", "input", table).value;
    var valMin = FindControl("hdfValMinComposicao", "input", table).value;
    var tipoVenda = FindControl("hdfTipoVenda", "input");
    tipoVenda = tipoVenda != null ? tipoVenda.value : 0;
    valMin = new Number(valMin.replace(',', '.'));

    if (!FindControl("txtValorComposicaoIns", "input", table).disabled && new Number(valor.replace(',', '.')) < valMin) {
        alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
        return false;
    }

    // Verifica se foi clicado no aplicar na telinha de beneficiamentos
    if (FindControl("tb_ConfigVidroComposicao_" + idTbConfigVidro, "table").style.display == "block") {
        alert("Aplique as alterações no beneficiamento antes de salvar o item.");
        return false;
    }

    var tipoOrcamento = FindControl("hdfTipoOrcamento", "input").value;
    var subgrupoProdComposto = CadOrcamento.SubgrupoProdComposto(idProd).value;
        
    if (tipoVenda != 3 && tipoVenda != 4 && (valor == "" || parseFloat(valor.replace(",", ".")) == 0) && !(tipoOrcamento == 1 && subgrupoProdComposto)) {
        alert("Informe o valor vendido.");
        return false;
    } else if (qtde == "0" || qtde == "") {
        alert("Informe a quantidade.");
        return false;
    } else if (FindControl("txtAlturaComposicaoIns", "input", table).disabled == false) {
        if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
            alert("Informe a altura.");
            return false;
        }

        if (FindControl("hdfIsAluminioComposicao", "input", table).value == "true" && altura > parseInt(comprimentoMaxAluminio)) {
            alert("A altura deve ser no máximo " + comprimentoMaxAluminio + "ml.");
            return false;
        }
    }
        
    if (!obrigarProcAplComposicao(table)) {
        return false;
    }
        
    // Calcula o ICMS do produto
    var aliquota = FindControl("hdfAliquotaIcmsProdComposicao", "input", table);
    var icms = FindControl("hdfValorIcmsProdComposicao", "input", table);
    icms.value = parseFloat(valor) * (parseFloat(aliquota.value) / 100);
    icms.value = icms.value.toString().replace('.', ',');

    if (FindControl("txtEspessuraComposicao", "input", table) != null) {
        FindControl("txtEspessuraComposicao", "input", table).disabled = false;
    }
        
    FindControl("txtAlturaComposicaoIns", "input", table).disabled = false;
    FindControl("txtLarguraComposicaoIns", "input", table).disabled = false;
    FindControl("txtValorComposicaoIns", "input", table).disabled = false;
    FindControl("txtQtdeComposicaoIns", "input", table).disabled = false;
            
    return true;
}

function exibirProdsComposicaoChild(botao, idProdOrcamento) {
    var grdProds = FindControl("grdProdutosComposicao", "table");

    if (grdProds == null) {
        return;
    }

    var linha = document.getElementById("prodOrcamentoChild_" + idProdOrcamento);
    var exibir = linha.style.display == "none";
    linha.style.display = exibir ? "" : "none";
    botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
    botao.title = (exibir ? "Esconder" : "Exibir") + " Produtos da Composição";
}

</script>

<asp:GridView GridLines="None" ID="grdProdutosOrcamentoComposicao" runat="server" DataSourceID="odsProdutosOrcamentoComposicao" DataKeyNames="IdProd"
    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" ShowFooter="True" PageSize="12" CssClass="gridStyle"
    OnPreRender="grdProdutosOrcamentoComposicao_PreRender" OnRowCommand="grdProdutosOrcamentoComposicao_RowCommand" OnRowUpdated="grdProdutosOrcamentoComposicao_RowUpdated"
    OnRowDeleted="grdProdutosOrcamentoComposicao_RowDeleted">
    <FooterStyle Wrap="True" />
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 0 -->
                <!-- EDITAR -->
                <asp:ImageButton ID="lnkEditComposicao" runat="server" CommandName="Edit" ImageUrl="~/Images/Edit.gif"
                    OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>'/>
                <!-- EXCLUIR -->
                <asp:ImageButton ID="imbExcluirComposicao" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" Visible='<%# Eval("DeleteVisible") %>'
                    OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(false); return false" : "if (!confirm(\"Deseja remover esse produto do orçamento?\")) return false" %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <!-- ATUALIZAR -->
                <asp:ImageButton ID="imbAtualizarComposicao" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar"
                    OnClientClick='<%# "if (!onUpdateProdComposicao(this, &#39;" + IdProdOrcamento + "_" + Eval("IdProd") + "&#39;)) return false;" %>' />                
                <!-- CANCELAR -->
                <asp:ImageButton ID="imbCancelarComposicao" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />

                <asp:HiddenField ID="hdfIdProdOrcamentoComposicao" runat="server" Value='<%# Eval("IdProd") %>' />
                <asp:HiddenField ID="hdfIdOrcamentoComposicao" runat="server" Value='<%# Bind("IdOrcamento") %>' />
                <asp:HiddenField ID="hdfIdProdutoComposicao" runat="server" Value='<%# Bind("IdProduto") %>' />
                <asp:HiddenField ID="hdfCodInternoComposicao" runat="server" Value='<%# Eval("CodInterno") %>' />
                <asp:HiddenField ID="hdfValMinComposicao" runat="server" />
                <asp:HiddenField ID="hdfIsVidroComposicao" runat="server" Value='<%# Eval("IsVidro") %>' />
                <asp:HiddenField ID="hdfIsAluminioComposicao" runat="server" Value='<%# Eval("IsAluminio") %>' />
                <asp:HiddenField ID="hdfM2MinimoComposicao" runat="server" Value='<%# Eval("M2Minimo") %>' />
                <asp:HiddenField ID="hdfTipoCalcComposicao" runat="server" Value='<%# Eval("TipoCalc") %>' />
                <asp:HiddenField ID="hdfIdItemProjetoComposicao" runat="server" Value='<%# Bind("IdItemProjeto") %>' />
                <asp:HiddenField ID="hdfIdMaterItemProjComposicao" runat="server" Value='<%# Bind("IdMaterItemProj") %>' />
                <asp:HiddenField ID="hdfIdAmbienteOrcamentoComposicao" runat="server" Value='<%# Bind("IdProdParent") %>' />
                <asp:HiddenField ID="hdfAliquotaIcmsProdComposicao" runat="server" Value='<%# Bind("AliquotaIcms") %>' />
                <asp:HiddenField ID="hdfValorIcmsProdComposicao" runat="server" Value='<%# Bind("ValorIcms") %>' />
                <asp:HiddenField ID="hdfIdProdOrcamentoParent" runat="server" Value='<%# Bind("IdProdOrcamentoParent") %>' />
                <asp:HiddenField ID="hdfPodeEditarComposicao" runat="server" Value='<%# Eval("PodeEditarComposicao") %>' />
                <asp:HiddenField ID="hdfIdProdBaixaEst" runat="server" Value='<%# Bind("IdProdBaixaEst") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <select id="drpFooterVisible" style="display: none">
                </select>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
            <ItemTemplate>
                <!-- 1 -->
                <!-- CODIGO -->
                <asp:Label ID="lblCodProdComposicao" runat="server" Text='<%# Bind("CodInterno") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- CODIGO -->
                <asp:Label ID="lblCodProdComposicaoIns" runat="server" Text='<%# Eval("CodInterno") %>'>
                </asp:Label>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
            <ItemTemplate>
                <!-- 2 -->
                <!-- DESCRICAO -->
                <asp:Label ID="lblDescricaoProdutoComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- DESCRICAO -->
                <asp:Label ID="lblDescrProdComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                <!-- CUSTO COMPRA -->
                <asp:HiddenField ID="hdfCustoProdComposicao" runat="server" Value='<%# Eval("Custo") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- CODIGO -->
                <asp:TextBox ID="txtCodProdComposicaoIns" runat="server" Width="50px"
                    onblur="loadProdutoComposicao(this.value, this);" onkeydown="if (isEnter(event)) loadProdutoComposicao(this.value, this);" onkeypress="return !(isEnter(event));">
                </asp:TextBox>
                <!-- DESCRICAO -->
                <asp:Label ID="lblDescrProdComposicao" runat="server">
                </asp:Label>
                <input id="imgPesqProdComposicao" type="image" src="../Images/Pesquisar.gif"
                    onclick='<%# "getProdutoComposicao("  + (Request["idOrca"] != null ? Request["idOrca"] : "0") + ", this); return false;" %>'/>

                <asp:HiddenField ID="hdfValMinComposicao" runat="server" />
                <asp:HiddenField ID="hdfIsVidroComposicao" runat="server" />
                <asp:HiddenField ID="hdfTipoCalcComposicao" runat="server" />
                <asp:HiddenField ID="hdfIsAluminioComposicao" runat="server" />
                <asp:HiddenField ID="hdfM2MinimoComposicao" runat="server" />
                <asp:HiddenField ID="hdfAliquotaIcmsProdComposicao" runat="server" />
                <asp:HiddenField ID="hdfValorIcmsProdComposicao" runat="server" />
                <asp:HiddenField ID="hdfCustoProdComposicao" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
            <ItemTemplate>
                <!-- 3 -->
                <!-- QUANTIDADE -->
                <asp:Label ID="lblQtdeComposicao" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- QUANTIDADE -->
                <asp:TextBox ID="txtQtdeComposicaoIns" runat="server" Text='<%# Bind("Qtde") %>' Width="50px"
                    onblur="calcM2ProdComposicao(this);" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalcComposicao', 'input').value), true);" >
                </asp:TextBox>
                <!-- DESCONTO POR QUANTIDADE -->
                <uc5:ctrlDescontoQtde ID="ctrlDescontoQtdeComposicao" runat="server" Callback="calcTotalProdComposicao" CallbackValorUnit="calcTotalProdComposicao" ValidationGroup="produtoComposicao"
                    PercDescontoQtde='<%# Bind("PercDescontoQtde") %>' ValorDescontoQtde='<%# Bind("ValorDescontoQtde") %>' OnLoad="ctrlDescontoQtdeComposicao_Load" />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- QUANTIDADE -->
                <asp:TextBox ID="txtQtdeComposicaoIns" runat="server" Width="50px" onkeydown="if (isEnter(event)) calcM2ProdComposicao(this);" onblur="calcM2ProdComposicao(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalcComposicao', 'input').value), true);">
                </asp:TextBox>
                <!-- DESCONTO POR QUANTIDADE -->
                <uc5:ctrlDescontoQtde ID="ctrlDescontoQtdeComposicao" runat="server" Callback="calcTotalProdComposicao" ValidationGroup="produtoComposicao"
                    CallbackValorUnit="calcTotalProdComposicao" OnLoad="ctrlDescontoQtdeComposicao_Load" />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
            <ItemTemplate>
                <!-- 4 -->
                <!-- LARGURA -->
                <asp:Label ID="lblLarguraComposicao" runat="server" Text='<%# Bind("Largura") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- LARGURA -->
                <asp:TextBox ID="txtLarguraComposicaoIns" runat="server" Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="50px"
                    onblur="calcM2ProdComposicao(this);" onkeypress="return soNumeros(event, true, true);">
                </asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- LARGURA -->
                <asp:TextBox ID="txtLarguraComposicaoIns" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);" onblur="calcM2ProdComposicao(this);">
                </asp:TextBox>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
            <ItemTemplate>
                <!-- 5 -->
                <!-- ALTURA -->
                <asp:Label ID="lblAlturaComposicao" runat="server" Text='<%# Bind("AlturaLista") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- ALTURA -->
                <asp:TextBox ID="txtAlturaComposicaoIns" runat="server" Text='<%# Bind("Altura") %>' Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"
                    onblur="calcM2ProdComposicao(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalcComposicao', 'input').value), true);"
                    onchange="FindControl('hdfAlturaRealComposicaoIns', 'input').value = this.value">
                </asp:TextBox>
                <asp:HiddenField ID="hdfAlturaRealComposicaoIns" runat="server" Value='<%# Bind("AlturaCalc") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- ALTURA -->
                <asp:TextBox ID="txtAlturaComposicaoIns" runat="server" Width="50px" onblur="calcM2ProdComposicao(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalcComposicao', 'input').value), true);"
                    onchange="FindControl('hdfAlturaRealComposicaoIns', 'input').value = this.value">
                </asp:TextBox>
                <asp:HiddenField ID="hdfAlturaRealComposicaoIns" runat="server" />
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
            <ItemTemplate>
                <!-- 6 -->
                <!-- TOTAL M2 -->
                <asp:Label ID="lblTotMComposicao" runat="server" Text='<%# Bind("TotM") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- TOTAL M2 -->
                <asp:Label ID="lblTotM2ComposicaoIns" runat="server" Text='<%# Bind("TotM") %>'>
                </asp:Label>
                <asp:HiddenField ID="hdfTotMComposicao" runat="server" Value='<%# Eval("TotM") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- TOTAL M2 -->
                <asp:Label ID="lblTotM2ComposicaoIns" runat="server"></asp:Label>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotM2Calc">
            <ItemTemplate>
                <!-- 7 -->
                <!-- TOTAL M2 CALCULADO -->
                <asp:Label ID="lblTotM2CalcComposicao" runat="server" Text='<%# Eval("TotMCalc") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- TOTAL M2 CALCULADO -->
                <asp:Label ID="lblTotM2CalcComposicao" runat="server" Text='<%# Eval("TotMCalc") %>'>
                </asp:Label>
                <asp:HiddenField ID="hdfTotM2CalcComposicao" runat="server" Value='<%# Eval("TotMCalc") %>' />
                <asp:HiddenField ID="hdfTotM2CalcSemChapaComposicao" runat="server" Value='<%# Eval("TotM2SemChapa") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- TOTAL M2 CALCULADO -->
                <asp:Label ID="lblTotM2CalcComposicaoIns" runat="server">
                </asp:Label>
                <asp:HiddenField ID="hdfTotM2ComposicaoIns" runat="server" />
                <asp:HiddenField ID="hdfTotM2CalcComposicaoIns" runat="server" />
                <asp:HiddenField ID="hdfTotM2CalcSemChapaComposicaoIns" runat="server" />
            </FooterTemplate>
            <HeaderStyle Wrap="True" />
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
            <ItemTemplate>
                <!-- 8 -->
                <!-- VALOR VENDIDO -->
                <asp:Label ID="lblValorVendidoComposicao" runat="server" Text='<%# Bind("ValorProd", "{0:C}") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- VALOR VENDIDO -->
                <asp:TextBox ID="txtValorComposicaoIns" runat="server" Text='<%# Bind("ValorProd") %>' Width="50px" OnLoad="txtValorInsComposicao_Load"
                    onblur="calcTotalProdComposicao(this);" onkeypress="return soNumeros(event, false, true);">
                </asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- VALOR VENDIDO -->
                <asp:TextBox ID="txtValorComposicaoIns" runat="server" Width="50px" OnLoad="txtValorInsComposicao_Load"
                    onkeydown="if (isEnter(event)) calcTotalProdComposicao(this);" onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProdComposicao(this);">
                </asp:TextBox>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
            <ItemTemplate>
                <!-- 9 -->
                <!-- CODIGO PROCESSO -->
                <asp:Label ID="lblCodProcessoComposicao" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- CODIGO PROCESSO -->
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtProcComposicaoIns" runat="server" Width="30px" Text='<%# Eval("CodProcesso") %>' onkeypress="return !(isEnter(event));"
                                onblur="loadProcComposicao(this, this.value);" onkeydown="if (isEnter(event)) { loadProcComposicao(this, this.value); }">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selProcComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- CODIGO PROCESSO -->
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtProcComposicaoIns" runat="server" onkeypress="return !(isEnter(event));" Width="30px"
                                onblur="loadProcComposicao(this, this.value);" onkeydown="if (isEnter(event)) { loadProcComposicao(this, this.value); }">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selProcComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
            <ItemTemplate>
                <!-- 10 -->
                <!-- CODIGO APLICACAO -->
                <asp:Label ID="lblCodAplicacaoComposicao" runat="server" Text='<%# Eval("CodAplicacao") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- CODIGO APLICACAO -->
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtAplComposicaoIns" runat="server" Text='<%# Eval("CodAplicacao") %>' Width="30px" onkeypress="return !(isEnter(event));"
                                onblur="loadAplComposicao(this, this.value);" onkeydown="if (isEnter(event)) { loadAplComposicao(this, this.value); }">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selAplComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- CODIGO APLICACAO -->
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtAplComposicaoIns" runat="server" Width="30px" onkeypress="return !(isEnter(event));"
                                onblur="loadAplComposicao(this, this.value);" onkeydown="if (isEnter(event)) { loadAplComposicao(this, this.value); }">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selAplComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Total" SortExpression="Total">
            <ItemTemplate>
                <!-- 11 -->
                <!-- TOTAL -->
                <asp:Label ID="lblTotalComposicao" runat="server" Text='<%# Bind("Total", "{0:C}") %>'>
                </asp:Label>
                <!-- PERCENTUAL DESCONTO QUANTIDADE -->
                <asp:Label ID="lblPercDescontoQtdeComposicao" runat="server" Text='<%# "(Desconto de " + Eval("PercDescontoQtde") + "%)" %>' Visible='<%# (float)Eval("PercDescontoQtde") > 0 %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- TOTAL -->
                <asp:Label ID="lblTotalComposicaoIns" runat="server" Text='<%# Bind("Total") %>' Style="padding-top: 4px">
                </asp:Label>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- TOTAL -->
                <asp:Label ID="lblTotalComposicaoIns" runat="server">
                </asp:Label>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
            <ItemTemplate>
                <!-- 12 -->
                <!-- VALOR BENEFICIAMENTO -->
                <asp:Label ID="lblValorBenefComposicao" runat="server" Text='<%# Bind("ValorBenef", "{0:C}") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- VALOR BENEFICIAMENTO -->
                <asp:Label ID="lblValorBenefComposicao" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'>
                </asp:Label>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- VALOR BENEFICIAMENTO -->
                <asp:Label ID="lblValorBenefComposicao" runat="server">
                </asp:Label>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 13 -->
            </ItemTemplate>
            <EditItemTemplate>
                <!-- EXIBIR CONTROLE BENEFICIAMENTO -->
                <asp:ImageButton ID="lnkBenefComposicao" runat="server" Visible='<%# Eval("BenefVisible") %>' ImageUrl="~/Images/gear_add.gif"
                    OnClientClick='<%# "exibirBenefComposicao(this, &#39;" + Eval("IdProdOrcamentoParent") + "_" + Eval("IdProd") + "&#39;); return false;" %>' />
                <table id="tb_ConfigVidroComposicao_<%# Eval("IdProdOrcamentoParent") + "_" + Eval("IdProd") %>" cellspacing="0" style="display: none;">
                    <!-- ESPESSURA -->
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEspessuraComposicao" runat="server" Width="30px" Text='<%# Bind("Espessura") %>'
                                            OnDataBinding="txtEspessuraComposicao_DataBinding" onkeypress="return soNumeros(event, false, true);">
                                        </asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <!-- CONTROLE BENEFICIAMENTO -->
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrlBenefEditarComposicao" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>' Redondo='<%# Bind("Redondo") %>'
                                ValidationGroup="produtoComposicao" OnInit="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotalComposicao" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left"></td>
                    </tr>
                </table>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- EXIBIR CONTROLE BENEFICIAMENTO -->
                <asp:ImageButton ID="lnkBenefComposicao" runat="server" ImageUrl="~/Images/gear_add.gif" Style="display: none;"
                    OnClientClick='<%# "exibirBenefComposicao(this, &#39;" + IdProdOrcamento + "_0&#39;); return false;" %>' />
                <table id="tb_ConfigVidroComposicao_<%# IdProdOrcamento + "_0" %>" cellspacing="0" style="display: none;">
                    <!-- ESPESSURA -->
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEspessuraComposicao" runat="server" onkeypress="return soNumeros(event, false, true);" Width="30px">
                                        </asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <!-- CONTROLE BENEFICIAMENTO -->
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrlBenefInserirComposicao" runat="server" OnInit="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotalComposicao" ValidationGroup="produtoComposicao" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                        </td>
                    </tr>
                </table>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 14 -->
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- INSERIR PRODUTO COMPOSICAO -->
                <asp:ImageButton ID="lnkInsProdComposicao" runat="server" OnClick="lnkInsProdComposicao_Click" ImageUrl="../Images/ok.gif"
                    OnClientClick='<%# "if (!onSaveProdComposicao(this, &#39;" + IdProdOrcamento + "_0&#39;)) return false;" %>' />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <div id='<%# "imgProdsComposto_" + Eval("IdProd") %>'>
                    <!-- EXIBIR PRODUTO COMPOSICAO -->
                    <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/box.png" ToolTip="Exibir Produtos da Composição" Visible='<%# Eval("IsProdLamComposicao") %>'
                        OnClientClick='<%# "exibirProdsComposicaoChild(this, " + Eval("IdProd") + "); return false"%>' />
                    <!-- EXIBIR IMAGEM DAS PEÇAS-->
                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/imagem.gif" ToolTip="Exibir imagem das peças" Visible='<%# (Eval("IsVidro").ToString() == "true")%>'
                        OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=orcamento&idOrcamento=" + Eval("IdOrcamento") +"&IdProdOrcamento=" +  Eval("IdProd") +"&pecaAvulsa=" +  ((bool)Eval("IsProdLamComposicao") == false) + "\"); return false" %>' />
                </div>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 15 -->
                <!-- CONTROLE PRODUTO COMPOSICAO -->
                <tr id="prodOrcamentoChild_<%# Eval("IdProd") %>" style="display: none" align="center">
                    <td colspan="17">
                        <br />
                        <uc1:ctrlProdComposicaoOrcamentoChild runat="server" ID="ctrlProdCompChild" Visible='<%# Eval("IsProdLamComposicao") %>' IdProdOrcamento='<%# Glass.Conversoes.StrParaInt(Eval("IdProd").ToString()) %>' />
                        <br />
                    </td>
                </tr>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerStyle CssClass="pgr"></PagerStyle>
    <EditRowStyle CssClass="edit"></EditRowStyle>
    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
</asp:GridView>
<asp:HiddenField runat="server" ID="hdfIdProdOrcamento" />
<asp:HiddenField ID="hdfIdProdutoComposicao" runat="server" />
<asp:HiddenField ID="hdfIdProdOrcamentoComposicao" runat="server" />
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosOrcamentoComposicao" runat="server" TypeName="Glass.Data.DAL.ProdutosOrcamentoDAO" DataObjectTypeName="Glass.Data.Model.ProdutosOrcamento"
    EnablePaging="True" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
    SelectMethod="PesquisarProdutosComposicao" SelectCountMethod="PesquisarProdutosComposicaoCountGrid" InsertMethod="Insert" UpdateMethod="UpdateComTransacao" DeleteMethod="Delete"
    OnUpdated="odsProdutosOrcamentoComposicao_Updated" OnDeleted="odsProdutosOrcamentoComposicao_Deleted">
    <SelectParameters>
        <asp:QueryStringParameter QueryStringField="idOrcamento" Name="idOrcamento" Type="Int32" />
        <asp:ControlParameter ControlID="hdfIdProdAmbienteOrcamento" Name="idProdAmbienteOrcamento" PropertyName="Value" Type="Int32" />
        <asp:ControlParameter ControlID="hdfIdProdOrcamento" Name="idProdOrcamentoParent" PropertyName="Value" Type="Int32" />
    </SelectParameters>
</colo:VirtualObjectDataSource>

<script type="text/javascript">
    
    $(document).ready(function(){
        if (FindControl("imbAtualizarComposicao", "input") != null && FindControl("lblCodProdComposicaoIns", "span") != null) {
            loadProdutoComposicao(FindControl("lblCodProdComposicaoIns", "span").innerHTML, FindControl("imbAtualizarComposicao", "input"));
        }
    });

    var codigoProduto = FindControl('txtCodProdComposicaoIns', 'input');

    if (codigoProduto != null && codigoProduto.value != "") {
        codigoProduto.onblur();
    }

</script>
