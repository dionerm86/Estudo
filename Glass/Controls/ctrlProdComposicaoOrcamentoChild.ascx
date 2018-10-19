<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlProdComposicaoOrcamentoChild.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlProdComposicaoOrcamentoChild" %>

<%@ Register Src="ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc5" %>

<script type="text/html">

// Guarda a quantidade disponível em estoque do produto buscado
var qtdEstoqueComposicaoChild = 0;
var exibirMensagemEstoqueComposicaoChild = false;
var qtdEstoqueMensagemComposicaoChild = 0;    
var insertingComposicaoChild = false;
var produtoAmbienteComposicaoChild = false;
var loadingComposicaoChild = true;
var idOrcamento = <%= Request["idOrca"] != null ? Request["idOrca"] : "0" %>;
var vendedorAlteraValorUnitarioProduto = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.AlterarValorUnitarioProduto.ToString().ToLower() %>;
var exibirPopup = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ExibePopupVidrosEstoque.ToString().ToLower() %>;
var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
var utilizarRoteiroProducao = <%= UtilizarRoteiroProducao().ToString().ToLower() %>;

function buscaTableChild(control) {
    var tr = control;

    while (tr.id == "" || (tr.id.indexOf("prodOrcamentoChild_") == -1 && tr.nodeName.toLowerCase() != "tr")) {
        tr = tr.parentElement;
    }

    return tr;
}

function getNomeControleBenefComposicaoChild(control) {
    var nomeControle = "<%= NomeControleBenefComposicao() %>";
    nomeControle = FindControl(nomeControle + "_tblBenef", "table", control);

    if (nomeControle == null) {
        return null;
    }

    nomeControle = nomeControle.id;
    return nomeControle.substr(0, nomeControle.lastIndexOf("_"));
}

// Carrega dados do produto com base no código do produto passado
function loadProdutoComposicaoChild(codInterno, control) {
    if (control == null || codInterno == "") {
        return false;
    }

    var table = buscaTableChild(control);
    var txtValor = FindControl("txtChildValorComposicaoIns", "input", table);
        
    if (txtValor != null) {
        txtValor.disabled = vendedorAlteraValorUnitarioProduto == false;
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
            
        if (retorno[0] == "Erro") {
            alert(retorno[1]);

            if (!produtoAmbienteComposicaoChild) {
                FindControl("txtChildCodProdComposicao", "input", table).value = "";
            } else {
                FindControl("txtChildCodAmbComposicao", "input", table).value = "";
            }
                
            return false;
        }

        if (retorno[0] == "Prod") {
            FindControl("hdfChildIdProdComposicao", "input", table).value = retorno[1];
            var subgrupoProdComposto = CadOrcamento.SubgrupoProdComposto(retorno[1]).value;
            var tipoOrcamento = FindControl("hdfTipoOrcamento", "input").value;
            var alterarValor = !(tipoOrcamento == 1 && subgrupoProdComposto);
            txtValor.value = alterarValor ? retorno[3] : txtValor.value;
                    
            FindControl("hdfChildIsVidroComposicao", "input", table).value = retorno[4]; // Informa se o produto é vidro
            FindControl("hdfChildM2MinimoComposicao", "input", table).value = retorno[5]; // Informa se o produto possui m² mínimo
            FindControl("hdfChildTipoCalcComposicao", "input", table).value = retorno[6]; // Verifica como deve ser calculado o produto
                    
            // Se o campo do valor estiver desativado não precisa calcular o valor mínimo, tendo em vista que o usuário não poderá alterar.
            if (!txtValor.disabled) {
                atualizaValMinComposicaoChild(table);
            }
                      
            var tipoCalc = retorno[6];
            // Se o produto não for vidro, desabilita os textboxes largura e altura,
            // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
            // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
            var cAltura = FindControl("txtChildAlturaComposicaoIns", "input", table);
            var cLargura = FindControl("txtChildLarguraComposicaoIns", "input", table);
            cAltura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
            cLargura.disabled = CalcProd_DesabilitarLargura(tipoCalc);                    
            var nomeControle = getNomeControleBenefComposicaoChild(table);
            // Zera o campo qtd para evitar que produtos calculados por mҠfiquem com quantidade decimal por exemplo (chamado 11010)
            var txtQtdProd = FindControl("txtChildQtdeComposicaoIns", "input", table);

            if (txtQtdProd != null && !loadingComposicaoChild) {
                txtQtdProd.value = "";
            }
                    
            if (nomeControle != null && nomeControle != undefined) {
                // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                if (retorno[4] == "true" && exibirControleBenef(nomeControle) && FindControl("lnkChildBenefComposicao", "input", table) != null) {
                    FindControl("txtChildEspessuraComposicao", "input", table).value = retorno[7];
                    FindControl("txtChildEspessuraComposicao", "input", table).disabled = retorno[7] != "" && retorno[7] != "0";
                }
                    
                if (FindControl("lnkChildBenefComposicao", "input", table) != null && nomeControle != null && nomeControle.indexOf("Inserir") > -1) {
                    FindControl("lnkChildBenefComposicao", "input", table).style.display = exibirControleBenef(nomeControle) ? "" : "none";
                }
            }
                        
            FindControl("hdfChildAliquotaIcmsProdComposicao", "input", table).value = retorno[8].replace('.', ',');
                    
            // O campo altura e largura devem sempre ser atribuídos pois caso seja selecionado um box e logo após seja selecionado um kit 
            // por exemplo, ao inserí-lo ele estava ficando com o campo altura, largura e m² preenchidos apesar de ser calculado por qtd
            if (retorno[9] != "" || retorno[4] == "false") {
                FindControl("txtChildAlturaComposicao", "input", table).value = retorno[9];
                FindControl("hdfChildAlturaRealComposicao", "input", table).value = retorno[9];
            }

            if (retorno[10] != "" || retorno[4] == "false") {
                FindControl("txtChildLarguraComposicao", "input", table).value = retorno[10];
            }
                        
            if (cAltura.disabled && FindControl("hdfChildAlturaRealComposicao", "input", table) != null) {
                FindControl("hdfChildAlturaRealComposicao", "input", table).value = cAltura.value;
            }

            var idProdOrcamento = FindControl("hdfChildIdProdOrcamento", "input", table).value;

            if (retorno[14] != "") {
                setAplComposicaoChild(retorno[14], retorno[15], idProdOrcamento);
            }
                    
            if (retorno[16] != "") {
                setProcComposicaoChild(retorno[16], retorno[17], null, idProdOrcamento);
            }
                    
            FindControl("hdfChildCustoProdComposicao", "input", table).value = retorno[18];
            var cPodeEditarComposicao = FindControl("hdfChildPodeEditarComposicao","input", table);

            if (cPodeEditarComposicao != null) {
                var podeEditarComposicao = cPodeEditarComposicao.value;
                var cQtdeComposicaoIns = FindControl("txtChildQtdeComposicaoIns", "input", table);
                cQtdeComposicaoIns.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cQtdeComposicaoIns.disabled;
                cLargura.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cLargura.disabled;
                cAltura.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cAltura.disabled;
            }
        }

        FindControl("lblChildDescrProdComposicao", "span", table).innerHTML = retorno[2];
    }
    catch (err) { alert(err); }
}

function atualizaValMinComposicaoChild(control) {
    var table = buscaTableChild(control);
    var codInterno = FindControl("txtChildCodProdComposicaoIns", "input", table);
    codInterno = codInterno != null ? codInterno.value : FindControl("lblChildCodProdComposicaoIns", "span", table).innerHTML;
    var tipoOrcamento = FindControl("hdfTipoOrcamento", "input").value;
    var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;       
    var cliRevenda = FindControl("hdfCliRevenda", "input").value;
    var idCliente = FindControl("hdfIdCliente", "input").value;
    var tipoVenda = FindControl("hdfTipoVenda", "input").value;            
    var idProdOrcamento = FindControl("hdfChildProdOrcamentoComposicao", "input", table);
    idProdOrcamento = idProdOrcamento != null ? idProdOrcamento.value : "";            
    var controleDescQtde = FindControl("_divDescontoQtde", "div", table).id;
    controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));            
    var percDescontoQtde = controleDescQtde.PercDesconto();
            
    FindControl("hdfChildValMinComposicao", "input", table).value = CadOrcamento.GetValorMinimo(codInterno, tipoOrcamento, tipoEntrega, tipoVenda, idCliente, cliRevenda, idProdOrcamento, percDescontoQtde, idOrcamento).value;
}

// Função chamada pelo popup de escolha da Aplicação do produto
function setAplComposicaoChild(idAplicacao, codInterno, idProdOrcamento) {
    var tr = FindControl("prodOrcamentoChild_" + idProdOrcamento, "tr");

    FindControl("txtChildAplComposicaoIns", "input", tr).value = codInterno;
    FindControl("hdfChildIdAplicacaoComposicao", "input", tr).value = idAplicacao;
}

function loadAplComposicaoChild(control, codInterno) {
    var tr = buscaTableChild(control);
    var idProdOrcamento = FindControl("hdfChildIdProdOrcamento", "input", tr).value;

    if (codInterno == "") {
        setAplComposicaoChild("", "", idProdOrcamento);
        return false;
    }
    
    try {
        var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

        if (response == null || response == "") {
            alert("Falha ao buscar Aplicação. Ajax Error.");
            setAplComposicaoChild("", "", idProdOrcamento);
            return false
        }

        response = response.split("\t");
            
        if (response[0] == "Erro") {
            alert(response[1]);
            setAplComposicaoChild("", "", idProdOrcamento);
            return false;
        }

        setAplComposicaoChild(response[1], response[2], idProdOrcamento);
    }
    catch (err) { alert(err); }
}

// Função chamada pelo popup de escolha do Processo do produto
function setProcComposicaoChild(idProcesso, codInterno, codAplicacao, idProdOrcamento) {
    var codInternoProd = "";
    var codAplicacaoAtual = "";
    var tr = FindControl("prodOrcamentoChild_" + idProdOrcamento, "tr");        
    var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfChildIdProdComposicao", "input", tr).value);
    var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

    if (idSubgrupo.value != "" && retornoValidacao.value == "false" && (FindControl("txtChildProcComposicaoIns", "input", tr) != null && FindControl("txtChildProcComposicaoIns", "input", tr).value != ""))
    {
        FindControl("txtChildProcComposicaoIns", "input", tr).value = "";
        alert("Este processo não pode ser selecionado para este produto.")
        return false;
    }

    var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, FindControl("hdfIdOrcamento", "input").value);
        
    if (verificaEtiquetaProc.error != null) {
        FindControl("txtChildProcComposicaoIns", "input", tr).value = "";
        FindControl("hdfChildIdProcessoComposicao", "input", tr).value = "";

        setAplComposicaoChild("", "", idProdOrcamento);
        alert(verificaEtiquetaProc.error.description);
        return false;
    }
    
    FindControl("txtChildProcComposicaoIns", "input", tr).value = codInterno;
    FindControl("hdfChildIdProcessoComposicao", "input", tr).value = idProcesso;
            
    if (FindControl("txtChildCodProdComposicaoIns", "input", tr) != null) {
        codInternoProd = FindControl("txtChildCodProdComposicaoIns", "input", tr).value;
    } else {
        codInternoProd = FindControl("lblChildCodProdComposicaoIns", "span", tr).innerHTML;
    }
                
    codAplicacaoAtual = FindControl("txtChildAplComposicaoIns", "input", tr).value;
        
    if (((codAplicacao && codAplicacao != "") || (codInternoProd != "" && CadOrcamento.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) && (codAplicacaoAtual == null || codAplicacaoAtual == "")) {
        loadAplComposicaoChild(tr, codAplicacao);
    }
}

function loadProcComposicaoChild(control, codInterno) {
    var tr = buscaTableChild(control);
    var idProdOrcamento = FindControl("hdfChildIdProdOrcamento", "input", tr).value;

    if (codInterno == "") {
        setProcComposicaoChild("", "", "", idProdOrcamento);
        return false;
    }

    try {
        var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

        if (response == null || response == "") {
            alert("Falha ao buscar Processo. Ajax Error.");
            setProcComposicaoChild("", "", "", idProdOrcamento);
            return false
        }

        response = response.split("\t");
            
        if (response[0] == "Erro") {
            alert(response[1]);
            setProcComposicaoChild("", "", "", idProdOrcamento);
            return false;
        }

        setProcComposicaoChild(response[1], response[2], response[3], idProdOrcamento);
    }
    catch (err) { alert(err); }
}

var dadoscalcM2ProdComposicaoChildChild = {
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
function calcM2ProdComposicaoChild(control) {
    try {
        var table = buscaTableChild(control);
        var idProd = FindControl("hdfChildIdProdComposicao", "input", table).value;
        var altura = FindControl("txtChildAlturaComposicaoIns", "input", table).value;
        var largura = FindControl("txtChildLarguraComposicaoIns", "input", table).value;            
        var qtde = FindControl("txtChildQtdeComposicaoIns", "input", table).value;
        var isVidro = FindControl("hdfChildIsVidroComposicao", "input", table).value == "true";
        var tipoCalc = FindControl("hdfChildTipoCalcComposicao", "input", table).value;
            
        if (altura == "" || largura == "" || qtde == "" || altura == "0" || (tipoCalc != 2 && tipoCalc != 10 && !usarBenefTodosGrupos)) {
            if (qtde != "" && qtde != "0") {
                calcTotalProdComposicaoChild(table);
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

        var esp = FindControl("txtChildEspessuraComposicao", "input", table) != null ? FindControl("txtChildEspessuraComposicao", "input", table).value : 0;
            
        // Calcula metro quadrado
        var idCliente = FindControl("hdfIdCliente", "input").value;
            
        if ((idProd != dadoscalcM2ProdComposicaoChildChild.IdProd && idProd > 0) || (altura != dadoscalcM2ProdComposicaoChildChild.Altura && altura > 0) ||
            (largura != dadoscalcM2ProdComposicaoChildChild.Largura) || (qtde != dadoscalcM2ProdComposicaoChildChild.Qtde && qtde > 0) ||
            (tipoCalc != dadoscalcM2ProdComposicaoChildChild.TipoCalc && tipoCalc > 0) || (idCliente != dadoscalcM2ProdComposicaoChildChild.Cliente) || (redondo != dadoscalcM2ProdComposicaoChildChild.Redondo) ||
            (numBenef != dadoscalcM2ProdComposicaoChildChild.NumBenef)) {
            FindControl("lblChildTotM2ComposicaoIns", "span", table).innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, "false").value;
            FindControl("hdfChildTotM2CalcComposicao", "input", table).value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
            FindControl("hdfChildTotM2CalcSemChapaComposicao", "input", table).value = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
            FindControl("lblChildTotM2CalcComposicao", "span", table).innerHTML = FindControl("hdfChildTotM2CalcComposicao", "input", table).value.replace('.', ',');
                
            if (FindControl("hdfChildTotM2ComposicaoIns", "input", table) != null) {
                FindControl("hdfChildTotM2ComposicaoIns", "input", table).value = FindControl("lblChildTotM2ComposicaoIns", "span", table).innerHTML.replace(',', '.');
            } else if (FindControl("hdfChildTotMComposicao", "input", table) != null) {
                FindControl("hdfChildTotMComposicao", "input", table).value = FindControl("lblChildTotM2ComposicaoIns", "span", table).innerHTML.replace(',', '.');
            }
                
            dadoscalcM2ProdComposicaoChildChild = {
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
            
        calcTotalProdComposicaoChild(table);
    }
    catch (err) { alert(err); }
}

// Calcula em tempo real o valor total do produto
function calcTotalProdComposicaoChild(control) {
    try {
        var table = buscaTableChild(control);
        var valorIns = FindControl("txtChildValorComposicaoIns", "input", table).value;

        if (valorIns == "") {
            return;
        }

        var totM2 = FindControl("lblChildTotM2ComposicaoIns", "span", table).innerHTML;
        var totM2Calc = new Number(FindControl("hdfChildTotM2CalcComposicao", "input", table).value.replace(',', '.')).toFixed(2);
        var total = new Number(valorIns.replace(',', '.')).toFixed(2);
        var qtde = new Number(FindControl("txtChildQtdeComposicaoIns", "input", table).value.replace(',', '.'));
        var altura = new Number(FindControl("txtChildAlturaComposicaoIns", "input", table).value.replace(',', '.'));
        var largura = new Number(FindControl("txtChildLarguraComposicaoIns", "input", table).value.replace(',', '.'));
        var tipoCalc = FindControl("hdfChildTipoCalcComposicao", "input", table).value;
        var m2Minimo = FindControl("hdfChildM2MinimoComposicao", "input", table).value;            
        var controleDescQtde = FindControl("_divDescontoQtde", "div", table).id;
        controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
        var percDesconto = controleDescQtde.PercDesconto();
        var percDescontoAtual = controleDescQtde.PercDescontoAtual();
            
        var retorno = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, FindControl("txtAlturaIns", "input"), largura, true, tipoCalc, 0, 0, percDescontoAtual, percDesconto);

        if (retorno != "") {
            FindControl("lblChildTotalComposicaoIns", "span", table).innerHTML = retorno;
        }
    }
    catch (err) { alert(err); }
}

function exibirBenefComposicaoChild(botao, id) {
    for (iTip = 0; iTip < 2; iTip++) {
        TagToTip('tb_ConfigVidroComposicao_' + id, FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true,
            CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
            FIX, [botao, 9 - getTableWidth('tb_ConfigVidroComposicao_' + id), -41 - getTableHeight('tb_ConfigVidroComposicao_' + id)]);
    }
}

function setValorTotalComposicaoChild(valor, custo, idProdOrcamento) {
    if (getNomeControleBenefComposicaoChild() != null) {
        if (exibirControleBenef(getNomeControleBenefComposicaoChild())) {
            var tr = FindControl("prodOrcamentoChild_" + idProdOrcamento, "tr");
            var lblValorBenef = FindControl("lblChildValorBenefComposicao", "span", tr);

            lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
        }
    }
}

function selProcComposicaoChild(control) {
    var tr = buscaTableChild(control);
    var idProdOrcamento = FindControl("hdfChildIdProdOrcamento", "input", tr).value;
    var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfChildIdProdComposicao", "input", tr).value);

    openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx?idProdOrcamento=' + idProdOrcamento + '&idSubgrupo=' + idSubgrupo.value);
    return false;
}

function selAplComposicaoChild(control) {
    var tr = buscaTableChild(control);
    var idProdOrcamento = FindControl("hdfChildIdProdOrcamento", "input", tr).value;

    openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx?idProdOrcamento=' + idProdOrcamento);
    return false;
}

function getProdutoComposicaoChild(control) {
    var tr = buscaTableChild(control);
    var idProdOrcamento = FindControl("hdfChildIdProdOrcamento", "input", tr).value;

    openWindow(450, 700, '../Utils/SelProd.aspx?idOrcamento=' + idOrcamento + '&idProdOrcamento=' + idProdOrcamento + '&callback=prodComposicao');
}

function setProdutoComposicaoChild(codInterno, idProdOrcamento) {
    var tr = FindControl("prodOrcamentoChild_" + idProdOrcamento, "tr");
    FindControl("txtChildCodProdComposicaoIns", "input", tr).value = codInterno;

    loadProdutoComposicaoChild(codInterno, tr);
}

function obrigarProcAplComposicaoChild(control) {
    var table = buscaTableChild(control);
    var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
    var isVidroRoteiro = dadosProduto.Grupo == 1 && utilizarRoteiroProducao;
        
    if (dadosProduto.IsChapaVidro) {
        return true;
    }

    if (isVidroRoteiro || (isObrigarProcApl && isVidroBenef)) {
        if (FindControl("txtChildAplComposicaoIns", "input", table) != null && FindControl("txtChildAplComposicaoIns", "input", table).value == "") {
            if (isVidroRoteiro && !isObrigarProcApl) {
                alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                return false;
            }

            alert("Informe a aplicação.");
            return false;
        }
            
        if (FindControl("txtChildProcComposicaoIns", "input", table) != null && FindControl("txtChildProcComposicaoIns", "input", table).value == "") {
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

var saveProdComposicaoClickedChild = false;

// Chamado quando um produto está para ser inserido no orçamento
function onSaveProdComposicaoChild(control, idTbConfigVidro) {
    if (!validate("produtoComposicaoChild")) {
        return false;
    }
            
    if (saveProdComposicaoClickedChild == true) {
        return false;
    }
            
    saveProdComposicaoClickedChild = true;
        
    var tr = buscaTableChild(control);

    atualizaValMinComposicaoChild(tr);
    
    var codProd = FindControl("txtChildCodProdComposicaoIns", "input", tr).value;
    var idProd = FindControl("hdfChildIdProdutoComposicao", "input", tr).value;
    var valor = FindControl("txtChildValorComposicaoIns", "input", tr).value;
    var qtde = FindControl("txtChildQtdeComposicaoIns", "input", tr).value;
    var altura = FindControl("txtChildAlturaComposicaoIns", "input", tr).value;
    var largura = FindControl("txtChildLarguraComposicaoIns", "input", tr).value;
    var valMin = FindControl("hdfChildValMinComposicao", "input", tr).value;
    var tipoVenda = FindControl("hdfTipoVenda", "input");
    tipoVenda = tipoVenda != null ? tipoVenda.value : 0;

    if (codProd == "") {
        alert("Informe o código do produto.");
        saveProdComposicaoClickedChild = false;
        return false;
    }
        
    // Verifica se foi clicado no aplicar na telinha de beneficiamentos
    if (FindControl("tb_ConfigVidroComposicao_" + idTbConfigVidro, "table").style.display == "block") {
        alert("Aplique as alterações no beneficiamento antes de salvar o item.");
        return false;
    }
        
    if ( tipoVenda != 3 && tipoVenda != 4 && (valor == "" || parseFloat(valor.replace(",", ".")) == 0)) {
        alert("Informe o valor vendido.");
        saveProdComposicaoClickedChild = false;
        return false;
    }
        
    if (qtde == "0" || qtde == "") {
        alert("Informe a quantidade.");
        saveProdComposicaoClickedChild = false;
        return false;
    }
        
    valMin = new Number(valMin.replace(',', '.'));

    if (!FindControl("txtChildValorComposicaoIns", "input", tr).disabled && new Number(valor.replace(',', '.')) < valMin) {
        alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
        saveProdComposicaoClickedChild = false;
        return false;
    }
        
    if (FindControl("txtChildAlturaComposicaoIns", "input", tr).disabled == false) {
        if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
            alert("Informe a altura.");
            saveProdComposicaoClickedChild = false;
            return false;
        }

        if (FindControl("hdfChildIsAluminioComposicao", "input", tr).value == "true" && altura > parseInt(comprimentoMaxAluminio)) {
            alert("A altura deve ser no máximo " + comprimentoMaxAluminio + "ml.");
            saveProdComposicaoClickedChild = false;
            return false;
        }            
    }
        
    // Se o textbox da largura estiver habilitado, deverá ser informada
    if (FindControl("txtChildLarguraComposicaoIns", "input", tr).disabled == false && largura == "") {
        alert("Informe a largura.");
        saveProdComposicaoClickedChild = false;
        return false;
    }
        
    if (!obrigarProcAplComposicaoChild(tr)) {
        saveProdComposicaoClickedChild = false;
        return false;
    }
        
    // Calcula o ICMS do produto
    var aliquota = FindControl("hdfChildAliquotaIcmsProdComposicao", "input", tr);
    var icms = FindControl("hdfChildValorIcmsProdComposicao", "input", tr);
    icms.value = aliquota.value > 0 ? parseFloat(valor) * (parseFloat(aliquota.value) / 100) : 0;
    icms.value = icms.value.toString().replace('.', ',');
        
    if (FindControl("txtChildEspessuraComposicao", "input", tr) != null) {
        FindControl("txtChildEspessuraComposicao", "input", tr).disabled = false;
    }
        
    FindControl("txtChildAlturaComposicaoIns", "input", tr).disabled = false;
    FindControl("txtChildLarguraComposicaoIns", "input", tr).disabled = false;
    FindControl("txtChildValorComposicaoIns", "input", tr).disabled = false;
    FindControl("txtChildQtdeComposicaoIns", "input", tr).disabled = false;
        
    return true;
}

// Função chamada quando o produto está para ser atualizado
function onUpdateProdComposicaoChild(control, idTbConfigVidro) {
    if (!validate("produtoComposicaoChild")) {
        return false;
    }
        
    var table = buscaTableChild(control);

    atualizaValMinComposicaoChild(table);
    
    var valor = FindControl("txtChildValorComposicaoIns", "input", table).value;
    var qtde = FindControl("txtChildQtdeComposicaoIns", "input", table).value;
    var altura = FindControl("txtChildAlturaComposicaoIns", "input", table).value;
    var idProd = FindControl("hdfChildIdProdComposicao", "input", table).value;
    var codInterno = FindControl("hdfChildCodInternoComposicao", "input", table).value;
    var valMin = FindControl("hdfChildValMinComposicao", "input", table).value;
    var tipoVenda = FindControl("hdfTipoVenda", "input");
    tipoVenda = tipoVenda != null ? tipoVenda.value : 0;
    valMin = new Number(valMin.replace(',', '.'));

    if (!FindControl("txtChildValorComposicaoIns", "input", table).disabled && new Number(valor.replace(',', '.')) < valMin) {
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
    } else if (FindControl("txtChildAlturaComposicaoIns", "input", table).disabled == false) {
        if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
            alert("Informe a altura.");
            return false;
        }

        if (FindControl("hdfChildIsAluminioComposicao", "input", table).value == "true" && altura > parseInt(comprimentoMaxAluminio)) {
            alert("A altura deve ser no máximo " + comprimentoMaxAluminio + "ml.");
            return false;
        }
    }
        
    if (!obrigarProcAplComposicaoChild(table)) {
        return false;
    }
        
    // Calcula o ICMS do produto
    var aliquota = FindControl("hdfChildAliquotaIcmsProdComposicao", "input", table);
    var icms = FindControl("hdfChildValorIcmsProdComposicao", "input", table);
    icms.value = parseFloat(valor) * (parseFloat(aliquota.value) / 100);
    icms.value = icms.value.toString().replace('.', ',');

    if (FindControl("txtChildEspessuraComposicao", "input", table) != null) {
        FindControl("txtChildEspessuraComposicao", "input", table).disabled = false;
    }
        
    FindControl("txtChildAlturaComposicaoIns", "input", table).disabled = false;
    FindControl("txtChildLarguraComposicaoIns", "input", table).disabled = false;
    FindControl("txtChildValorComposicaoIns", "input", table).disabled = false;
    FindControl("txtChildQtdeComposicaoIns", "input", table).disabled = false;
            
    return true;
}
        
</script>

<asp:GridView GridLines="None" ID="grdProdutosOrcamentoComposicao" runat="server" DataSourceID="odsProdutosOrcamentoComposicao" DataKeyNames="IdProd"
    PageSize="12" AllowPaging="True" ShowFooter="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
    OnPreRender="grdProdutosOrcamentoComposicao_PreRender" OnRowUpdated="grdProdutosOrcamentoComposicao_RowUpdated" OnRowDeleted="grdProdutosOrcamentoComposicao_RowDeleted" OnRowCommand="grdProdutosOrcamentoComposicao_RowCommand" >
    <FooterStyle Wrap="True" />
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 0 -->
                <!-- EDITAR -->
                <asp:ImageButton ID="lnkChildEditComposicao" runat="server" CommandName="Edit" ImageUrl="~/Images/Edit.gif"
                    OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>' />
                <!-- EXCLUIR -->
                <asp:ImageButton ID="imbChildExcluirComposicao" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" Visible='<%# Eval("DeleteVisible") %>'
                    OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(false); return false" : "if (!confirm(\"Deseja remover esse produto do orçamento?\")) return false" %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <!-- ATUALIZAR -->
                <asp:ImageButton ID="imbChildAtualizarComposicao" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar"
                    OnClientClick='<%# "if (!onUpdateProdComposicaoChild(this, &#39;" + IdProdOrcamento + "_" + Eval("IdProd") + "&#39;)) return false;" %>' />
                <!-- CANCELAR -->
                <asp:ImageButton ID="imbChildCancelarComposicao" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />

                <asp:HiddenField ID="hdfChildProdOrcamentoComposicao" runat="server" Value='<%# Eval("IdProd") %>' />
                <asp:HiddenField ID="hdfChildIdOrcamentoComposicao" runat="server" Value='<%# Bind("IdOrcamento") %>' />
                <asp:HiddenField ID="hdfChildIdProdutoComposicao" runat="server" Value='<%# Bind("IdProduto") %>' />
                <asp:HiddenField ID="hdfChildCodInternoComposicao" runat="server" Value='<%# Eval("CodInterno") %>' />
                <asp:HiddenField ID="hdfChildValMinComposicao" runat="server" />
                <asp:HiddenField ID="hdfChildIsVidroComposicao" runat="server" Value='<%# Eval("IsVidro") %>' />
                <asp:HiddenField ID="hdfChildIsAluminioComposicao" runat="server" Value='<%# Eval("IsAluminio") %>' />
                <asp:HiddenField ID="hdfChildM2MinimoComposicao" runat="server" Value='<%# Eval("M2Minimo") %>' />
                <asp:HiddenField ID="hdfChildTipoCalcComposicao" runat="server" Value='<%# Eval("TipoCalc") %>' />
                <asp:HiddenField ID="hdfChildIdItemProjetoComposicao" runat="server" Value='<%# Bind("IdItemProjeto") %>' />
                <asp:HiddenField ID="hdfChildIdMaterItemProjComposicao" runat="server" Value='<%# Bind("IdMaterItemProj") %>' />
                <asp:HiddenField ID="hdfChildIdProdParentComposicao" runat="server" Value='<%# Bind("IdProdParent") %>' />
                <asp:HiddenField ID="hdfChildAliquotaIcmsProdComposicao" runat="server" Value='<%# Bind("AliqIcms") %>' />
                <asp:HiddenField ID="hdfChildValorIcmsProdComposicao" runat="server" Value='<%# Bind("ValorIcms") %>' />
                <asp:HiddenField ID="hdfChildValorTabelaOrcamentoComposicao" runat="server" Value='<%# Bind("ValorTabelaOrcamento") %>' />
                <asp:HiddenField ID="hdfChildIdProdOrcamentoParent" runat="server" Value='<%# Bind("IdProdOrcamentoParent") %>' />
                <asp:HiddenField ID="hdfChildIdProdBaixaEst" runat="server" Value='<%# Bind("IdProdBaixaEst") %>' />
                <asp:HiddenField ID="hdfChildPodeEditarComposicao" runat="server" Value='<%# Eval("PodeEditarComposicao") %>' />
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
                <asp:Label ID="lblChildCodProdComposicao" runat="server" Text='<%# Bind("CodInterno") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- CODIGO -->
                <asp:Label ID="lblChildCodProdComposicaoIns" runat="server" Text='<%# Eval("CodInterno") %>'>
                </asp:Label>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
            <ItemTemplate>
                <!-- 2 -->
                <!-- DESCRICAO -->
                <asp:Label ID="lblChildProdutoComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- DESCRICAO -->
                <asp:Label ID="lblChildDescrProdComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'>
                </asp:Label>

                <asp:HiddenField ID="hdfChildCustoProdComposicao" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- CODIGO -->
                <asp:TextBox ID="txtChildCodProdComposicaoIns" runat="server" Width="50px" onkeypress="return !(isEnter(event));"
                    onblur="loadProdutoComposicaoChild(this.value, this);" onkeydown="if (isEnter(event)) loadProdutoComposicaoChild(this.value, this);">
                </asp:TextBox>
                <!-- DESCRICAO -->
                <asp:Label ID="lblChildDescrProdComposicao" runat="server"></asp:Label>
                <input id="imgChildPesqProdComposicao" type="image" onclick='<%# "getProdutoComposicaoChild(" + (Request["idOrca"] != null ? Request["idOrca"] : "0") + ", this); return false;" %>' src="../Images/Pesquisar.gif" />

                <asp:HiddenField ID="hdfChildValMinComposicao" runat="server" />
                <asp:HiddenField ID="hdfChildIsVidroComposicao" runat="server" />
                <asp:HiddenField ID="hdfChildTipoCalcComposicao" runat="server" />
                <asp:HiddenField ID="hdfChildIsAluminioComposicao" runat="server" />
                <asp:HiddenField ID="hdfChildM2MinimoComposicao" runat="server" />
                <asp:HiddenField ID="hdfChildAliquotaIcmsProdComposicao" runat="server" />
                <asp:HiddenField ID="hdfChildValorIcmsProdComposicao" runat="server" />
                <asp:HiddenField ID="hdfChildCustoProdComposicao" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
            <ItemTemplate>
                <!-- 3 -->
                <!-- QUANTIDADE -->
                <asp:Label ID="lblChildQtdeComposicao" runat="server" Text='<%# Bind("Qtde") %>'>
                </asp:Label>
                <!-- QUANTIDADE AMBIENTE -->
                <asp:Label ID="lblChildQtdeAmbienteComposicao" runat="server">
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- QUANTIDADE -->
                <asp:TextBox ID="txtChildQtdeComposicaoIns" runat="server" Text='<%# Bind("Qtde") %>' Width="50px" onblur="calcM2ProdComposicaoChild(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfChildTipoCalcComposicao', 'input').value), true);">
                </asp:TextBox>
                <!-- QUANTIDADE AMBIENTE -->
                <asp:Label ID="lblChildQtdeAmbienteComposicao" runat="server">
                </asp:Label>
                <!-- DESCONTO QUANTIDADE -->
                <uc5:ctrlDescontoQtde ID="ctrlDescontoQtdeComposicao" runat="server" PercDescontoQtde='<%# Bind("PercDescontoQtde") %>' ValorDescontoQtde='<%# Bind("ValorDescontoQtde") %>'
                    OnLoad="ctrlDescontoQtdeComposicao_Load" Callback="calcTotalProdComposicaoChild" CallbackValorUnit="calcTotalProdComposicaoChild" ValidationGroup="produtoComposicaoChild" />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- QUANTIDADE -->
                <asp:TextBox ID="txtChildQtdeComposicaoIns" runat="server" Width="50px" onkeydown="if (isEnter(event)) calcM2ProdComposicaoChild(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfChildTipoCalcComposicao', 'input').value), true);"
                    onblur="calcM2ProdComposicaoChild(this);">
                </asp:TextBox>
                <!-- QUANTIDADE AMBIENTE -->
                <asp:Label ID="lblChildQtdeAmbienteComposicao" runat="server">
                </asp:Label>
                <!-- DESCONTO QUANTIDADE -->
                <uc5:ctrlDescontoQtde ID="ctrlDescontoQtdeComposicao" runat="server" OnLoad="ctrlDescontoQtdeComposicao_Load" Callback="calcTotalProdComposicaoChild" ValidationGroup="produtoComposicaoChild"
                    CallbackValorUnit="calcTotalProdComposicaoChild" />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
            <ItemTemplate>
                <!-- 4 -->
                <!-- LARGURA -->
                <asp:Label ID="lblChildLarguraComposicao" runat="server" Text='<%# Bind("Largura") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- LARGURA -->
                <asp:TextBox ID="txtChildLarguraComposicaoIns" runat="server" Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="50px"
                    onblur="calcM2ProdComposicaoChild(this);" onkeypress="return soNumeros(event, true, true);">
                </asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- LARGURA -->
                <asp:TextBox ID="txtChildLarguraComposicaoIns" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);" onblur="calcM2ProdComposicaoChild(this);">
                </asp:TextBox>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
            <ItemTemplate>
                <!-- 5 -->
                <!-- ALTURA -->
                <asp:Label ID="lblChildAlturaComposicao" runat="server" Text='<%# Bind("Altura") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- ALTURA -->
                <asp:TextBox ID="txtChildAlturaComposicaoIns" runat="server" Text='<%# Bind("Altura") %>' Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"
                    onblur="calcM2ProdComposicaoChild(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfChildTipoCalcComposicao', 'input').value), true);">
                </asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- ALTURA -->
                <asp:TextBox ID="txtChildAlturaComposicaoIns" runat="server" Width="50px" onblur="calcM2ProdComposicaoChild(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfChildTipoCalcComposicao', 'input').value), true);">
                </asp:TextBox>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
            <ItemTemplate>
                <!-- 6 -->
                <!-- TOTAL M2 -->
                <asp:Label ID="lblChildTotMComposicao" runat="server" Text='<%# Bind("TotM") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- TOTAL M2 -->
                <asp:Label ID="lblChildTotM2ComposicaoIns" runat="server" Text='<%# Bind("TotM") %>'>
                </asp:Label>

                <asp:HiddenField ID="hdfChildTotMComposicao" runat="server" Value='<%# Eval("TotM") %>' />
                <asp:HiddenField ID="hdfChildTamanhoMaximoObraComposicao" runat="server" />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- TOTAL M2 -->
                <asp:Label ID="lblChildTotM2ComposicaoIns" runat="server">
                </asp:Label>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotMCalc">
            <ItemTemplate>
                <!-- 7 -->
                <!-- TOTAL M2 CALCULADO -->
                <asp:Label ID="lblChildTotm2CalcComposicao" runat="server" Text='<%# Eval("TotMCalc") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- TOTAL M2 CALCULADO -->
                <asp:Label ID="lblChildTotM2CalcComposicao" runat="server" Text='<%# Eval("TotMCalc") %>'>
                </asp:Label>

                <asp:HiddenField ID="hdfChildTotM2CalcComposicao" runat="server" Value='<%# Eval("TotMCalc") %>' />
                <asp:HiddenField ID="hdfChildTotM2CalcSemChapaComposicao" runat="server" Value='<%# Eval("TotalM2CalcSemChapa") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- TOTAL M2 CALCULADO -->
                <asp:Label ID="lblChildTotM2CalcComposicaoIns" runat="server">
                </asp:Label>

                <asp:HiddenField ID="hdfChildTotM2ComposicaoIns" runat="server" />
                <asp:HiddenField ID="hdfChildTotM2CalcComposicaoIns" runat="server" />
                <asp:HiddenField ID="hdfChildTotM2CalcSemChapaComposicaoIns" runat="server" />
            </FooterTemplate>
            <HeaderStyle Wrap="True" />
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorProd">
            <ItemTemplate>
                <!-- 8 -->
                <!-- VALOR VENDIDO -->
                <asp:Label ID="lblChildValorVendidoComposicao" runat="server" Text='<%# Bind("ValorProd", "{0:C}") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- VALOR VENDIDO -->
                <asp:TextBox ID="txtChildValorComposicaoIns" runat="server" Text='<%# Bind("ValorProd") %>' Width="50px" OnLoad="txtChildValorInsComposicao_Load"
                    onblur="calcTotalProdComposicaoChild(this);" onkeypress="return soNumeros(event, false, true);">
                </asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- VALOR VENDIDO -->
                <asp:TextBox ID="txtChildValorComposicaoIns" runat="server" Width="50px" OnLoad="txtChildValorInsComposicao_Load"
                    onkeydown="if (isEnter(event)) calcTotalProdComposicaoChild(this);" onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProdComposicaoChild(this);">
                </asp:TextBox>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
            <ItemTemplate>
                <!-- 9 -->
                <!-- CODIGO PROCESSO -->
                <asp:Label ID="lblChildCodProcessoComposicao" runat="server" Text='<%# Bind("CodProcesso") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- CODIGO PROCESSO -->
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtChildProcComposicaoIns" runat="server" Text='<%# Eval("CodProcesso") %>' Width="30px" onkeypress="return !(isEnter(event));"
                                onblur="loadProcComposicaoChild(this, this.value);" onkeydown="if (isEnter(event)) { loadProcComposicaoChild(this, this.value); }">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selProcComposicaoChild(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfChildIdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- CODIGO PROCESSO -->
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtChildProcComposicaoIns" runat="server" Width="30px" onblur="loadProcComposicaoChild(this, this.value);"
                                onkeydown="if (isEnter(event)) { loadProcComposicaoChild(this, this.value); }" onkeypress="return !(isEnter(event));">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selProcComposicaoChild(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfChildIdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
            <ItemTemplate>
                <!-- 10 -->
                <!-- CODIGO APLICACAO -->
                <asp:Label ID="lblChildCodAplicacaoComposicao" runat="server" Text='<%# Eval("CodAplicacao") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- CODIGO APLICACAO -->
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtChildAplComposicaoIns" runat="server" Text='<%# Eval("CodAplicacao") %>' Width="30px" onblur="loadAplComposicaoChild(this, this.value);"
                                onkeydown="if (isEnter(event)) { loadAplComposicaoChild(this, this.value); }" onkeypress="return !(isEnter(event));">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selAplComposicaoChild(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfChildIdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <!-- CODIGO APLICACAO -->
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtChildAplComposicaoIns" runat="server" Width="30px" onblur="loadAplComposicaoChild(this, this.value);"
                                onkeydown="if (isEnter(event)) { loadAplComposicaoChild(this, this.value); }" onkeypress="return !(isEnter(event));">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selAplComposicaoChild(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfChildIdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Total" SortExpression="Total">
            <ItemTemplate>
                <!-- 11 -->
                <!-- TOTAL -->
                <asp:Label ID="lblChildTotalComposicao" runat="server" Text='<%# Bind("Total", "{0:C}") %>'>
                </asp:Label>
                <!-- PERCENTUAL DESCONTO QUANTIDADE -->
                <asp:Label ID="lblChildPercDescontoQtdeComposicao" runat="server" Text='<%# "(Desconto de " + Eval("PercDescontoQtde") + "%)" %>' Visible='<%# (float)Eval("PercDescontoQtde") > 0 %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- TOTAL -->
                <asp:Label ID="lblChildTotalComposicaoIns" runat="server" Text='<%# Bind("Total") %>' Style="padding-top: 4px">
                </asp:Label>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
            <ItemTemplate>
                <!-- 12 -->
                <!-- VALOR BENEFICIAMENTO -->
                <asp:Label ID="lblChildValorBenefComposicao" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- VALOR BENEFICIAMENTO -->
                <asp:Label ID="lblChildValorBenefComposicao" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'>
                </asp:Label>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 13 -->
                <!-- IMAGEM PEÇAS -->
                <div id='<%# "imgProdsComposto_" + Eval("IdProd") %>'>
                     <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/imagem.gif"
                        OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=orcamento&idOrcamento=" + Eval("IdOrcamento") +"&idProdOrcamento=" + Eval("IdProd") +"&pecaAvulsa=" + ((bool)Eval("IsProdLamComposicao") == false) + "\"); return false" %>'
                        ToolTip="Exibir imagem das peças" Visible='<%# (Eval("IsVidro").ToString() == "true")%>' />
                </div>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- 14 -->
                <!-- CONTROLE BENEFICIAMENTO -->
                <asp:ImageButton ID="lnkChildBenefComposicao" runat="server" Visible='<%# Eval("BenefVisible") %>' ImageUrl="~/Images/gear_add.gif"
                    OnClientClick='<%# "exibirBenefComposicaoChild(this, &#39;" + Eval("IdProdOrcamentoParent") + "_" + Eval("IdProd") + "&#39;); return false;" %>' />
                <table id="tb_ConfigVidroComposicao_<%# Eval("IdProdOrcamentoParent") + "_" + Eval("IdProd") %>" cellspacing="0" style="display: none;">
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtChildEspessuraComposicao" runat="server" Text='<%# Bind("Espessura") %>' Width="30px"
                                            OnDataBinding="txtChildEspessuraComposicao_DataBinding" onkeypress="return soNumeros(event, false, true);">
                                        </asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrlChildBenefEditarComposicao" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>' Redondo='<%# Bind("Redondo") %>'
                                OnInit="ctrlBenef_Load" ValidationGroup="produtoComposicaoChild" CallbackCalculoValorTotal="setValorTotalComposicaoChild" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left"></td>
                    </tr>
                </table>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- CONTROLE BENEFICIAMENTO -->
                <asp:ImageButton ID="lnkChildBenefComposicao" runat="server" Style="display: none;" ImageUrl="~/Images/gear_add.gif"
                    OnClientClick='<%# "exibirBenefComposicaoChild(this, &#39;" + IdProdOrcamento + "_0&#39;); return false;" %>' />
                <table id="tb_ConfigVidroComposicao_<%# IdProdOrcamento + "_0" %>" cellspacing="0" style="display: none;">
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtChildEspessuraComposicao" runat="server" Width="30px" onkeypress="return soNumeros(event, false, true);">
                                        </asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrlChildBenefInserirComposicao" runat="server" OnInit="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotalComposicaoChild" ValidationGroup="produtoComposicaoChild" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left"></td>
                    </tr>
                </table>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 15 -->
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
                <!-- INSERIR -->
                <asp:ImageButton ID="lnkChildInsProdComposicao" runat="server" OnClick="lnkChildInsProdComposicao_Click" ImageUrl="../Images/ok.gif"
                    OnClientClick='<%# "if (!onSaveProdComposicaoChild(this, &#39;" + IdProdOrcamento + "_0&#39;)) return false;" %>' />
            </FooterTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerStyle CssClass="pgr"></PagerStyle>
    <EditRowStyle CssClass="edit"></EditRowStyle>
    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
</asp:GridView>
<asp:HiddenField ID="hdfChildIdProdOrcamento" runat="server" />
<asp:HiddenField ID="hdfChildIdProdutoComposicao" runat="server" />
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosOrcamentoComposicao" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosOrcamento" TypeName="Glass.Data.DAL.ProdutosOrcamentoDAO"
    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" EnablePaging="True" MaximumRowsParameterName="pageSize"
    SelectMethod="PesquisarProdutosComposicao" SelectCountMethod="PesquisarProdutosComposicaoCountGrid" InsertMethod="Insert" UpdateMethod="UpdateComTransacao" DeleteMethod="Delete"
    OnUpdated="odsProdutosOrcamentoComposicao_Updated" OnDeleted="odsProdutosOrcamentoComposicao_Deleted">
    <SelectParameters>
        <asp:QueryStringParameter QueryStringField="idOrcamento" Name="idOrcamento" Type="Int32" />
        <asp:ControlParameter ControlID="hdfIdProdAmbienteOrcamento" Name="idProdAmbienteOrcamento" PropertyName="Value" Type="Int32" />
        <asp:ControlParameter ControlID="hdfChildIdProdOrcamento" Name="idProdOrcamentoParent" PropertyName="Value" Type="Int32" />
    </SelectParameters>
</colo:VirtualObjectDataSource>

<script type="text/javascript">
    
    $(document).ready(function(){
        if (FindControl("imbChildAtualizarComposicao", "input") != null && FindControl("lblChildCodProdComposicaoIns", "span") != null) {
            loadProdutoComposicaoChild(FindControl("lblChildCodProdComposicaoIns", "span").innerHTML, FindControl("imbChildAtualizarComposicao", "input"));
        }
    })

    var codigoProduto = FindControl('txtChildCodProdComposicaoIns', 'input');

    if (codigoProduto != null && codigoProduto.value != "") {
        codigoProduto.onblur();
    }

</script>
