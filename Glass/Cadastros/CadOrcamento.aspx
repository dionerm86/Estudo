
<%@ Page Title="Cadastro de Orçamento" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadOrcamento.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadOrcamento" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc7" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc8" %>
<%@ Register Src="../Controls/ctrlProdComposicaoOrcamento.ascx" TagName="ctrlProdComposicaoOrcamento" TagPrefix="uc9" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style type="text/css">
        .cabecalho {
            font-weight: bold;
            padding-left: 4px;
        }
    </style>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/RecalcularOrcamento.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        var inserting = false;
        var lnkGerarPedido = null;
        var idOrcamento = <%= !string.IsNullOrEmpty(Request["idOrca"]) ? Request["idOrca"] : "0" %>;
        var idCliente;
        var hdfIdCliente;
        var idFuncAtual = <%= Glass.Data.Helper.UserInfo.GetUserInfo.CodUser %>;
        var tipoEntrega;
        var descontoOrcamento = <%= GetDescontoOrcamento() %>;
        var descontoProdutos = <%= GetDescontoProdutos() %>;
        var usarBenefTodosGrupos = <%= Glass.Configuracoes.Geral.UsarBeneficiamentosTodosOsGrupos.ToString().ToLower() %>;
        var permitirInserirSemTipoOrcamento = <%= Glass.Configuracoes.OrcamentoConfig.TelaCadastro.PermitirInserirSemTipoOrcamento.ToString().ToLower() %>;
        var usarComissionadoCliente = <%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente.ToString().ToLower() %>;
        var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
        var utilizarRoteiroProducao = <%= UtilizarRoteiroProducao().ToString().ToLower() %>;
        var procFilha = false;
        var aplFilha = false;
        var loading = true;

        function obterTbConfigVidro(id) {
            id = id > 0 ? id : 0;

            if (FindControl("tbConfigVidro_" + id, "table")) {
                return FindControl("tbConfigVidro_" + id, "table");
            } else if (FindControl("tbConfigVidro_", "table") != null) {
                return FindControl("tbConfigVidro_", "table");
            } else if (FindControl("tbConfigVidro", "table") != null) {
                return FindControl("tbConfigVidro_", "table");
            }
        }

        function setValorTotal(valor, custo) {
            if (getNomeControleBenef() != null) {
                if (exibirControleBenef(getNomeControleBenef())) {
                    var lblValorBenef = FindControl("lblValorBenef", "span");
                    lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
                }
            }
        }

        function getNomeControleBenef() {
            var nomeControle = "<%= NomeControleBenef() %>";
            nomeControle = FindControl(nomeControle + "_tblBenef", "table");

            if (nomeControle == null) {
                return null;
            }

            nomeControle = nomeControle.id;
            return nomeControle.substr(0, nomeControle.lastIndexOf("_"));
        }

        function exibirBenef(botao, idProd) {
            for (iTip = 0; iTip < 2; iTip++) {
                TagToTip('tbConfigVidro_' + idProd, FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true, CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'],
                    STICKY, true, FIX, [botao, 9-getTableWidth('tbConfigVidro_' + idProd), -41-getTableHeight('tbConfigVidro_' + idProd)]);
            }
        }

        function mensagemProdutoComDesconto(editar) {
            alert("Não é possível " + (editar ? "editar" : "remover") + " esse produto porque o orçamento possui desconto.\n" +
                "Aplique o desconto apenas ao terminar o cadastro dos produtos.\n" +
                "Para continuar, remova o desconto do orçamento.");
        }

        function getProduto() {
            openWindow(450, 700, '../Utils/SelProd.aspx');
        }

        // Calcula em tempo real o valor total do produto
        function calcTotalProd() {
            try {
                var valorIns = FindControl("txtValorIns", "input").value;

                if (valorIns == "") {
                    return;
                }

                var totM2 = FindControl("lblTotMIns", "span").innerHTML;
                var totM2Calc = new Number(FindControl("hdfTotMCalcIns", "input").value.replace(',', '.')).toFixed(2);
                var total = new Number(valorIns.replace(',', '.')).toFixed(2);
                var qtde = new Number(FindControl("txtQtdeIns", "input").value.replace(',', '.'));
                var campoAltura = FindControl("txtAlturaIns", "input");
                var altura = new Number(campoAltura.value.replace(',', '.'));
                var largura = new Number(FindControl("txtLarguraIns", "input").value.replace(',', '.'));
                var tipoCalc = FindControl("hdfTipoCalc", "input").value;
                var m2Minimo = FindControl("hdfM2Minimo", "input").value;
                var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
                controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
                var percDesconto = controleDescQtde.PercDesconto();
                var percDescontoAtual = controleDescQtde.PercDescontoAtual();

                var retorno = CalcProd_CalcTotalProd(valorIns, tipoCalc != 1 ? totM2 : 0, totM2Calc, m2Minimo, total, qtde, altura, campoAltura, largura, true, tipoCalc, 2, 2, percDescontoAtual, percDesconto);

                if (retorno != "") {
                    FindControl("lblTotalIns", "span").innerHTML = retorno;
                }
            }
            catch (err) { }
        }

        function atualizaValMin(idProdOrcamento) {
            idProdOrcamento = idProdOrcamento > 0 ? idProdOrcamento : 0;
            var codInterno = FindControl("txtCodProdIns", "input");
            codInterno = codInterno != null ? codInterno.value : FindControl("lblCodProdIns", "span").innerHTML;
            var altura = FindControl("txtAlturaIns", "input").value;
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
            var percDescontoQtde = controleDescQtde.PercDesconto();

            FindControl("hdfValMin", "input").value = CadOrcamento.GetValorMinimo(codInterno, tipoEntrega, idCliente, cliRevenda, idProdOrcamento, percDescontoQtde, altura).value;
        }

        // Função chamada após selecionar produto pelo popup
        function setProduto(codInterno) {
            try {
                FindControl("txtCodProdIns", "input").value = codInterno;
                loadProduto(codInterno, 0);
            }
            catch (err) { }
        }

        function obrigarProcApl() {
            var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
            var isVidroRoteiro = (utilizarRoteiroProducao && dadosProduto.Grupo == 1);

            if (dadosProduto.IsChapaVidro) {
                return true;
            }

            if (isVidroRoteiro || (isObrigarProcApl && isVidroBenef)) {
                if (FindControl("txtAplIns", "input") != null && FindControl("txtAplIns", "input").value == "") {
                    if (isVidroRoteiro && !isObrigarProcApl) {
                        alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                        return false;
                    }

                    alert("Informe a aplicação.");
                    return false;
                }

                if (FindControl("txtProcIns", "input") != null && FindControl("txtProcIns", "input").value == "") {
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
            catch (err) { alert(err); }
        }


        // Função chamada pelo popup de escolha da Aplicação do produto
        function setApl(idAplicacao, codInterno) {
            if (!aplFilha) {
                if (FindControl("txtAplIns", "input") != null) {
                    FindControl("txtAplIns", "input").value = codInterno;
                }

                if (FindControl("hdfIdAplicacao", "input") != null) {
                    FindControl("hdfIdAplicacao", "input").value = idAplicacao;
                }
            } else if (aplFilha) {
                if (FindControl("txtAplInsFilhos", "input") != null) {
                    FindControl("txtAplInsFilhos", "input").value = codInterno;
                }

                if (FindControl("hdfIdAplicacaoFilhos", "input") != null) {
                    FindControl("hdfIdAplicacaoFilhos", "input").value = idAplicacao;
                }
            }

            aplFilha = false;
        }

        // Função chamada pelo popup de escolha do Processo do produto
        function setProc(idProcesso, codInterno, codAplicacao) {
            var codInternoProd = "";
            var codAplicacaoAtual = "";

            if (procFilha) {
                var idSubgrupo = MetodosAjax.GetSubgrupoProdByProdFilhas(FindControl("hdfIdProd", "input").value);
                var retornoValidacao = MetodosAjax.ValidarProcessoFilhas(idSubgrupo.value, idProcesso);

                if (retornoValidacao.error != null) {
                    if (FindControl("txtProcInsFilhos", "input") != null) {
                        FindControl("txtProcInsFilhos", "input").value = "";
                    }

                    alert(retornoValidacao.error.description);
                    return false;
                }

                if (idSubgrupo.value != "" &&
                    retornoValidacao.value == "false" &&
                    FindControl("txtProcInsFilhos", "input") != null &&
                    FindControl("txtProcInsFilhos", "input").value != "")
                {
                    FindControl("txtProcInsFilhos", "input").value = "";
                    alert("Este processo não pode ser selecionado para este produto.")
                    return false;
                }
            } else {
                var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProduto", "input").value);
                var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

                if (idSubgrupo.value != "" &&
                    retornoValidacao.value == "false" &&
                    (FindControl("txtProcIns", "input") != null &&
                    FindControl("txtProcIns", "input").value != "")) {
                    FindControl("txtProcIns", "input").value = "";
                    alert("Este processo não pode ser selecionado para este produto.")
                    return false;
                }
            }

            if (!procFilha) {
                if (FindControl("txtProcIns", "input") != null) {
                    FindControl("txtProcIns", "input").value = codInterno;
                }

                if (FindControl("hdfIdProcesso", "input") != null) {
                    FindControl("hdfIdProcesso", "input").value = idProcesso;
                }

                if (FindControl("txtCodProdIns", "input") != null) {
                    codInternoProd = FindControl("txtCodProdIns", "input").value;
                } else {
                    codInternoProd = FindControl("lblCodProdIns", "span").innerHTML;
                }

                if (FindControl("txtAplIns", "input") != null) {
                    codAplicacaoAtual = FindControl("txtAplIns", "input").value;
                }
            } else if (procFilha) {
                if (FindControl("txtProcInsFilhos", "input") != null) {
                    FindControl("txtProcInsFilhos", "input").value = codInterno;
                }

                if (FindControl("hdfIdProcessoFilhos", "input") != null) {
                    FindControl("hdfIdProcessoFilhos", "input").value = idProcesso;
                }
            }


            if (((codAplicacao && codAplicacao != "") ||
                (codInternoProd != "" && CadPedido.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) &&
                (codAplicacaoAtual == null || codAplicacaoAtual == ""))
            {
                aplFilha = procFilha;
                loadApl(codAplicacao);
            }

            procFilha = false;
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
            catch (err) { alert(err); }
        }

        function buscarProcessos(desconsiderarSubgrupo) {
            var idSubgrupo = "";

            if (desconsiderarSubgrupo) {
                openWindow(450, 700, "../Utils/SelEtiquetaProcesso.aspx?idSubgrupo=");
            } else {
                idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProduto", "input").value);
                openWindow(450, 700, "../Utils/SelEtiquetaProcesso.aspx?idSubgrupo=" + idSubgrupo.value);
            }
        }

        // Carrega dados do produto com base no código do produto passado
        function loadProduto(codInterno, idProdOrcamento, manterProcessoAplicacao) {
            if (codInterno == undefined || codInterno == null || codInterno == "") {
                return false;
            }

            var txtValor = FindControl("txtValorIns", "input");

            try {
                var idLoja = FindControl("hdfIdLoja", "input").value;
                var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
                var cliRevenda = FindControl("hdfCliRevenda", "input").value;
                var idCliente = FindControl("hdfIdCliente", "input").value;
                var percComissao = getPercComissao();
                percComissao = percComissao == null ? 0 : percComissao.toString().replace('.', ',');
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
                    if (FindControl("txtCodProdIns", "input") != null) {
                        FindControl("txtCodProdIns", "input").value = "";
                    }

                    alert(retorno.error.description);
                    return false;
                }

                retorno = retorno.value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodProdIns", "input").value = "";
                    return false;
                }

                if (!manterProcessoAplicacao && FindControl("txtProcIns", "input") != null) {
                    FindControl("txtProcIns", "input").value = "";
                }

                var idLojaSubgrupo = CadOrcamento.ObterLojaSubgrupoProd(codInterno);

                if (idLojaSubgrupo.error != null) {
                    if (FindControl("txtCodProdIns", "input") != null) {
                        FindControl("txtCodProdIns", "input").value = "";
                    }

                    alert(idLojaSubgrupo.error.description);
                    return false;
                }

                if (idLojaSubgrupo.value != "0" && idLojaSubgrupo.value != idLoja) {
                    if (FindControl("txtCodProdIns", "input") != null) {
                        FindControl("txtCodProdIns", "input").value = "";
                    }

                    alert('Esse produto não pode ser utilizado, pois a loja do seu subgrupo é diferente da loja do pedido.');
                    return false;
                }

                var validaClienteSubgrupo = MetodosAjax.ValidaClienteSubgrupo(FindControl("hdfIdCliente", "input").value, codInterno);

                if (validaClienteSubgrupo.error != null) {
                    if (FindControl("txtCodProdIns", "input") != null) {
                        FindControl("txtCodProdIns", "input").value = "";
                    }

                    alert(validaClienteSubgrupo.error.description);
                    return false;
                }

                FindControl("lblDescrProd", "span").innerHTML = retorno[2];

                if (retorno[0] == "Prod") {
                    FindControl("hdfIdProduto", "input").value = retorno[1];
                    FindControl("txtValorIns", "input").value = retorno[3]; // Exibe no cadastro o valor mínimo do produto
                    FindControl("hdfIsVidro", "input").value = retorno[4]; // Informa se o produto é vidro
                    FindControl("hdfM2Minimo", "input").value = retorno[5]; // Informa se o produto possui m² mínimo
                    FindControl("hdfTipoCalc", "input").value = retorno[6]; // Verifica como deve ser calculado o produto

                    // Caso o vendedor não possa alterar o valor vendido do produto OU o valor vendido do produto seja zero ou o valor vendido do produto seja menor que o valor de tabela,
                    // atualiza o valor da obra ou de tabela do produto.
                    if (txtValor.value == "" || parseFloat(txtValor.value.toString().replace(",", ".")) == 0 || parseFloat(txtValor.value.toString().replace(",", ".")) < parseFloat(retorno[3].toString().replace(",", "."))) {
                        txtValor.value = retorno[3];
                    }

                    // Se o campo do valor estiver desativado não precisa calcular o valor mínimo, tendo em vista que o usuário não poderá alterar.
                    if (!txtValor.disabled) {
                        atualizaValMin();
                    }

                    var tipoCalc = FindControl("hdfTipoCalc", "input").value;
                    // Se o produto não for vidro, desabilita os textboxes largura e altura,
                    // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
                    // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
                    var cAltura = FindControl("txtAlturaIns", "input");
                    var cLargura = FindControl("txtLarguraIns", "input");
                    cAltura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
                    cLargura.disabled = CalcProd_DesabilitarLargura(tipoCalc);
                    var nomeControle = getNomeControleBenef();
                    var tbConfigVidro = obterTbConfigVidro();

                    // Zera o campo qtd para evitar que produtos calculados por mҠfiquem com quantidade decimal por exemplo (chamado 11010)
                    var txtQtdProd = FindControl("txtQtdeIns", "input");

                    if (txtQtdProd != null && !loading) {
                        txtQtdProd.value = "";
                    }

                    if (nomeControle != null && nomeControle != undefined) {
                        // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                        if (retorno[4] == "true" && exibirControleBenef(nomeControle) && FindControl("lnkBenef", "a") != null) {
                            if (tbConfigVidro != null && FindControl("txtEspessura", "input", tbConfigVidro) != null) {
                                FindControl("txtEspessura", "input", tbConfigVidro).value = retorno[7];
                                FindControl("txtEspessura", "input", tbConfigVidro).disabled = retorno[7] != "" && retorno[7] != "0";
                            } else if (FindControl("txtEspessura", "input") != null) {
                                FindControl("txtEspessura", "input").value = retorno[7];
                                FindControl("txtEspessura", "input").disabled = retorno[7] != "" && retorno[7] != "0";
                            }
                        }

                        if (FindControl("lnkBenef", "a") != null && nomeControle != null && nomeControle.indexOf("Inserir") > -1) {
                            FindControl("lnkBenef", "a").style.display = exibirControleBenef(nomeControle) ? "" : "none";
                        }
                    }

                    if (FindControl("hdfAliquotaIcmsProd", "input") != null) {
                        FindControl("hdfAliquotaIcmsProd", "input").value = retorno[8].replace('.', ',');
                    }

                    // O campo altura e largura devem sempre ser atribuídos pois caso seja selecionado um box e logo após seja selecionado um kit
                    // por exemplo, ao inserí-lo ele estava ficando com o campo altura, largura e m² preenchidos apesar de ser calculado por qtd
                    if (retorno[9] != "" || retorno[4] == "false") {
                        if (FindControl("txtAlturaIns", "input") != null) {
                            FindControl("txtAlturaIns", "input").value = retorno[9];
                        }

                        if (FindControl("hdfAlturaCalcIns", "input") != null) {
                            FindControl("hdfAlturaCalcIns", "input").value = retorno[9];
                        }
                    }

                    if (retorno[10] != "" || retorno[4] == "false") {
                        if (FindControl("txtLarguraIns", "input") != null) {
                            FindControl("txtLarguraIns", "input").value = retorno[10];
                        }
                    }

                    if (cAltura.disabled) {
                        if (FindControl("hdfAlturaCalcIns", "input") != null != null) {
                            FindControl("hdfAlturaCalcIns", "input").value = cAltura.value;
                        }
                    }

                    if (!manterProcessoAplicacao && retorno[14] != "") {
                        setApl(retorno[14], retorno[15]);
                    }

                    if (!manterProcessoAplicacao && retorno[16] != "") {
                        setProc(retorno[16], retorno[17]);
                    }

                    if (FindControl("hdfCustoProd", "input") != null) {
                        FindControl("hdfCustoProd", "input").value = retorno[18];
                    }

                    if (FindControl("chkAplicarBenefFilhos", "input") != null) {
                        FindControl("chkAplicarBenefFilhos", "input").style.display = retorno[19] == "true" ? "" : "none";
                    }
                }

                if (retorno[19].value == "true") {
                    if (document.getElementById("tblProcessoFilhas") != null) {
                        document.getElementById("tblProcessoFilhas").style.display = "";
                    }

                    if (document.getElementById("tblAplicacaoFilhos") != null) {
                        document.getElementById("tblAplicacaoFilhos").style.display = "";
                    }
                } else {
                    if (document.getElementById("tblProcessoFilhas") != null) {
                        document.getElementById("tblProcessoFilhas").style.display = "none";
                    }

                    if (document.getElementById("tblAplicacaoFilhos") != null) {
                        document.getElementById("tblAplicacaoFilhos").style.display = "none";
                    }
                }
            }
            catch (err) { alert(err); }
        }

        function recalcular(idOrcamento, perguntar, tipoEntregaNovo, idClienteNovo) {
            if (recalcularOrcamento(idOrcamento, perguntar, "loading", tipoEntregaNovo, idClienteNovo)) {
                if (lnkGerarPedido == null) {
                    alert("Orçamento recalculado com sucesso!");
                    redirectUrl(window.location.href);
                }
            }
        }

        function limparComissionado() {
            FindControl("hdfIdComissionado", "input").value = "";
            FindControl("lblComissionado", "span").innerHTML = "";
            FindControl("txtPercentual", "input").value = "0";
            FindControl("txtValorComissao", "input").value = "R$ 0,00";
        }

        function geraPedido(link, hiddenIdCliente, perguntar) {
            if (perguntar && !confirm("Tem certeza que deseja gerar um pedido para este orçamento?")) {
                return false;
            }

            lnkGerarPedido = link;
            hdfIdCliente = document.getElementById(hiddenIdCliente);

            if (hdfIdCliente.value == "") {
                alert("Você deve selecionar o cliente antes de continuar.");
                openWindow(500, 750, "../Utils/SelCliente.aspx");
                return false;
            }

            return true;
        }

        function setCliente(idCli, nome) {
            hdfIdCliente.value = idCli;
            recalcular(idOrcamento, false, "", idCli);
            eval(lnkGerarPedido.href.replace(/%20/g, " "));
        }

        var botaoAtualizarClicado = false;

        function onSaveOrca() {
            if (botaoAtualizarClicado) {
                return false;
            }

            botaoAtualizarClicado = true;

            var txtPercentual = FindControl("txtPercentual", "input");
            var hdfIdComissionado = FindControl("hdfIdComissionado", "input");

            // Se o percentual de comissão a ser cobrado for > 0, verifica se o comissionado foi informado
            if (txtPercentual != null && hdfIdComissionado != null && txtPercentual.value != "" && parseFloat(txtPercentual.value.replace(',', '.')) > 0 && hdfIdComissionado.value == "") {
                alert("Informe o comissionado.");
                botaoAtualizarClicado = false;
                return false;
            }

            var tipoEntregaAtual = FindControl("ddlTipoEntrega", "select");
            var idClienteAtual = FindControl("txtIdCliente", "input");

            if (!permitirInserirSemTipoOrcamento && FindControl("drpTipoOrcamento", "select") != null && FindControl("drpTipoOrcamento", "select").value == "") {
                alert("Selecione o tipo do orçamento.");
                botaoAtualizarClicado = false;
                return false;
            }

            FindControl("drpFuncionario", "select").disabled = false;

            return true;
        }

        function onInsert() {
            if (inserting) {
                return false;
            }

            if (FindControl("txtNomeCliente", "input").value == "") {
                alert("Digite o nome do cliente ou escolha-o na lista para continuar.");
                return false;
            }

            if (FindControl("ddlTipoEntrega", "select").value == "") {
                alert("Selecione o tipo de entrega.");
                return false;
            }

            if (!onSaveOrca()) {
                return false;
            }

            document.getElementById("load").style.display = "";
            FindControl("drpFuncionario", "select").disabled = false;
            inserting = true;

            if (FindControl("drpLoja", "select")) {
                FindControl("drpLoja", "select").disabled = false;
            }

            return true;
        }

        // Função chamada para mostrar/esconder controles para inserção de novo ambiente
        function addAmbiente(value) {
            var ambiente = FindControl("txtAmbienteIns", "input");
            var descricao = FindControl("txtDescricaoAmbienteIns", "textarea");
            var qtde = FindControl("txtQtdeAmbiente", "input");

            if (ambiente == null && descricao == null) {
                return;
            }

            if (descricao != null) {
                descricao.style.display = value ? "" : "none";
            }

            if (ambiente != null) {
                ambiente.style.display = value ? "" : "none";
            }

            if (qtde != null) {
                qtde.style.display = value ? "" : "none";
            }

            FindControl("lnkInsAmbiente", "a").style.display = value ? "" : "none";
        }

        function calcularDesconto(tipoCalculo) {
            var controle = FindControl("txtDesconto", "input");

            if (controle.value == "0") {
                return;
            }

            var tipo = FindControl("drpTipoDesconto", "select").value;
            var desconto = parseFloat(controle.value.replace(',', '.'));

            if (isNaN(desconto)) {
                desconto = 0;
            }

            var tipoAtual = FindControl("hdfTipoDesconto", "input").value;
            var descontoAtual = parseFloat(FindControl("hdfDesconto", "input").value.replace(',', '.'));

            if (isNaN(descontoAtual)) {
                descontoAtual = 0;
            }

            var alterou = tipo != tipoAtual || desconto != descontoAtual;
            var descontoMaximo = CadOrcamento.PercDesconto(idOrcamento, idFuncAtual, alterou).value;
            var total = parseFloat(FindControl("hdfTotalSemDesconto", "input").value.replace(/\./g, "").replace(',', '.'));
            var totalProduto = tipoCalculo == 2 ? parseFloat(FindControl("lblTotalIns", "span").innerHTML.replace("R$", "").replace(" ", "").replace(/\./g, "").replace(',', '.')) : 0;
            var valorDescontoMaximo = total * (descontoMaximo / 100);
            var valorDescontoProdutos = descontoProdutos - (tipoCalculo == 2 ? parseFloat(FindControl("hdfValorDescontoAtual", "input").value.replace(',', '.')) : 0);
            var valorDescontoOrcamento = tipoCalculo == 2 ? descontoOrcamento : 0;
            var descontoProdutos = parseFloat(((valorDescontoProdutos / (total > 0 ? total : 1)) * 100).toFixed(2));
            var descontoOrcamento = parseFloat(((valorDescontoOrcamento / (total > 0 ? total : 1)) * 100).toFixed(2));
            var descontoSomar = descontoProdutos + (tipoCalculo == 2 ? descontoOrcamento : 0);
            var valorDescontoSomar = valorDescontoProdutos + (tipoCalculo == 2 ? valorDescontoOrcamento : 0);

            if (tipo == 2) {
                desconto = (desconto / total) * 100;
            }

            if (parseFloat((desconto + descontoSomar).toFixed(2)) > parseFloat(descontoMaximo)) {
                var mensagem = "O desconto máximo permitido é de " + (tipo == 1 ? descontoMaximo + "%" : "R$ " + valorDescontoMaximo.toFixed(2).replace('.', ',')) + ".";

                if (descontoProdutos > 0) {
                    mensagem += "\nO desconto já aplicado aos produtos é de " + (tipo == 1 ? descontoProdutos + "%" : "R$ " + valorDescontoProdutos.toFixed(2).replace('.', ',')) + ".";
                }

                if (descontoOrcamento > 0) {
                    mensagem += "\nO desconto já aplicado ao orçamento é de " + (tipo == 1 ? descontoOrcamento + "%" : "R$ " + valorDescontoOrcamento.toFixed(2).replace('.', ',')) + ".";
                }

                alert(mensagem);
                controle.value = tipo == 1 ? descontoMaximo - descontoSomar : (valorDescontoMaximo - valorDescontoSomar).toFixed(2).replace('.', ',');

                if (parseFloat(controle.value.replace(',', '.')) < 0) {
                    controle.value = "0";
                }

                return false;
            }

            return true;
        }

        function setComissionado(id, nome, percentual) {
            FindControl("lblComissionado", "span").innerHTML = nome;
            FindControl("hdfIdComissionado", "input").value = id;
            FindControl("txtPercentual", "input").value = percentual;
        }

        function iniciaPesquisaCep(cep) {
            var logradouro = FindControl("txtEndereco", "input");
            var bairro = FindControl("txtBairro", "input");
            var cidade = FindControl("txtCidade", "input");

            pesquisarCep(cep, null, logradouro, bairro, cidade, null);
        }

        function openRpt(idOrca) {
            openWindow(600, 800, "../Relatorios/RelOrcamento.aspx?idOrca=" + idOrca);
            return false;
        }

        function openRptMemoria(idOrca) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=MemoriaCalculoOrcamento&idOrca=" + idOrca);
            return false;
        }

        function getCli(idCli) {
            var usarComissionado = usarComissionadoCliente;
            var dados = CadOrcamento.GetCli(idCli.value).value;

            if (dados == null || dados == "" || dados.split('|')[0] == "Erro") {
                idCli.value == "";
                FindControl("txtNomeCliente", "input").value = "";
                FindControl("hdfIdCliente", "input").value = "";
                FindControl("txtIdCliente", "input").value = "";

                if (usarComissionado) {
                    limparComissionado();
                }

                if (dados.split('|')[0] == "Erro") {
                    alert(dados.split('|')[1]);
                }

                return;
            }

            dados = dados.split("|");
            setDadosCliente(dados[0], dados[1], dados[2], dados[3], dados[4], dados[5], dados[6], dados[7], idCli.value, dados[8], dados[12]);

            var drpFuncionario = FindControl("drpFuncionario", "select");

            if (drpFuncionario != null && dados[9] != "0") {
                drpFuncionario.value = dados[9];
            }

            if (usarComissionado) {
                var comissionado = MetodosAjax.GetComissionado("", idCli.value).value.split(';');
                setComissionado(comissionado[0], comissionado[1], comissionado[2]);
            }
        }

        function setDadosCliente(nome, telRes, telCel, email, endereco, bairro, cidade, cep, idCliente, compl, obs) {
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
        }

        function openProdutos(idProd, editar) {
            var tipoEntrega = FindControl("ddlTipoEntrega", "select");

            if (tipoEntrega != null) {
                tipoEntrega = tipoEntrega.value;
            } else {
                tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            }

            if (tipoEntrega == "") {
                alert("Selecione o tipo de entrega antes de inserir um produto.");
                return false;
            }

            var liberarOrcamento = FindControl("hdfLiberarOrcamento", "input").value; // Define se poderá ser colocado valor abaixo do mínimo no orçamento
            var idCliente = FindControl("hdfIdCliente", "input").value;

            return false;
        }

        function openProjeto(idProd) {
            var tipoEntrega = FindControl("ddlTipoEntrega", "select");

            if (tipoEntrega != null) {
                tipoEntrega = tipoEntrega.value;
            } else {
                tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            }

            if (tipoEntrega == "") {
                alert("Selecione o tipo de entrega antes de inserir um projeto.");
                return false;
            }

            var idCliente = FindControl("hdfIdCliente", "input").value;

            openWindow(screen.height, screen.width, '../Cadastros/Projeto/CadProjetoAvulso.aspx?IdOrcamento=<%= Request["IdOrca"] %>' + "&IdProdOrca=" + idProd + "&idCliente=" + idCliente + "&TipoEntrega=" + tipoEntrega);

            return false;
        }

        function retornaPagina() {
            window.history.go(-1);
        }

        function refreshPage() {
            atualizarPagina();
        }

        var dadosCalcM2Prod = {
            IdProd: 0,
            Altura: 0,
            Largura: 0,
            Qtde: 0,
            QtdeAmbiente: 1,
            TipoCalc: 0,
            Cliente: 0,
            Redondo: false,
            NumBenef: 0
        };

        // Calcula em tempo real a metragem quadrada do produto
        function calcM2Prod() {
            try {
                var idProd = FindControl("hdfIdProduto", "input").value;
                var altura = FindControl("txtAlturaIns", "input").value;
                var largura = FindControl("txtLarguraIns", "input").value;
                var qtde = FindControl("txtQtdeIns", "input").value;
                var isVidro = FindControl("hdfIsVidro", "input").value == "true";
                var tipoCalc = FindControl("hdfTipoCalc", "input").value;

                if (altura == "" || largura == "" || qtde == "" || altura == "0" || (tipoCalc != 2 && tipoCalc != 10 && !usarBenefTodosGrupos)) {
                    if (qtde != "" && qtde != "0") {
                        calcTotalProd();
                    }

                    return false;
                }

                var redondo = (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked);

                if (altura != "" && largura != "" && parseInt(altura) > 0 && parseInt(largura) > 0 && parseInt(altura) != parseInt(largura) && redondo) {
                    alert('O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.');

                    if (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked) {
                        FindControl("Redondo_chkSelecao", "input").checked = false;
                    }

                    return false;
                }

                var numBenef = "";

                if (FindControl("Redondo_chkSelecao", "input") != null) {
                    numBenef = FindControl("Redondo_chkSelecao", "input").id
                    numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
                    numBenef = numBenef.substr(0, numBenef.lastIndexOf("_"));
                    numBenef = eval(numBenef).NumeroBeneficiamentos();
                }

                var esp = FindControl("txtEspessura", "input") != null ? FindControl("txtEspessura", "input").value : 0;
                // Calcula metro quadrado
                var idCliente = FindControl("hdfIdCliente", "input").value;

                if ((idProd != dadosCalcM2Prod.IdProd && idProd > 0) || (altura != dadosCalcM2Prod.Altura && altura > 0) || (largura != dadosCalcM2Prod.Largura) || (qtde != dadosCalcM2Prod.Qtde && qtde > 0) ||
                    (tipoCalc != dadosCalcM2Prod.TipoCalc && tipoCalc > 0) || (idCliente != dadosCalcM2Prod.Cliente) || (redondo != dadosCalcM2Prod.Redondo) ||
                    (numBenef != dadosCalcM2Prod.NumBenef)) {
                    FindControl("lblTotMIns", "span").innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, false).value;
                    FindControl("hdfTotMCalcIns", "input").value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, false).value;
                    FindControl("lblTotMCalcIns", "span").innerHTML = FindControl("hdfTotMCalcIns", "input").value.replace('.', ',');

                    if (FindControl("hdfTotMIns", "input") != null) {
                        FindControl("hdfTotMIns", "input").value = FindControl("lblTotMIns", "span").innerHTML.replace(',', '.');
                    } else if (FindControl("hdfTotM", "input") != null) {
                        FindControl("hdfTotM", "input").value = FindControl("lblTotMIns", "span").innerHTML.replace(',', '.');
                    }

                    dadosCalcM2Prod = {
                        IdProd: idProd,
                        Altura: altura,
                        Largura: largura,
                        Qtde: qtde,
                        QtdeAmbiente: 1,
                        TipoCalc: tipoCalc,
                        Cliente: idCliente,
                        Redondo: redondo,
                        NumBenef: numBenef
                    };
                }

                calcTotalProd();
            }
            catch (err) { alert(err); }
        }

        var saveProdClicked = false;

        // Chamado quando um produto está para ser inserido no orçamento.
        function onSaveProd() {
            if (!validate("produto") || saveProdClicked) {
                return false;
            }

            saveProdClicked = true;

            atualizaValMin();

            var codProd = FindControl("txtCodProdIns", "input").value;
            var idProduto = FindControl("hdfIdProduto", "input").value;
            var valor = FindControl("txtValorIns", "input").value;
            var qtde = FindControl("txtQtdeIns", "input").value;
            var altura = FindControl("txtAlturaIns", "input").value;
            var largura = FindControl("txtLarguraIns", "input").value;
            var valMin = FindControl("hdfValMin", "input").value;
            var tipoVenda = FindControl("hdfTipoVenda", "input");
            tipoVenda = tipoVenda != null ? tipoVenda.value : 0;
            var tbConfigVidro = obterTbConfigVidro();

            if (codProd == "") {
                alert("Informe o código do produto.");
                saveProdClicked = false;
                return false;
            }

            // Verifica se foi clicado no aplicar na telinha de beneficiamentos
            if (tbConfigVidro != null && tbConfigVidro.style.display == "block") {
                alert("Aplique as alterações no beneficiamento antes de salvar o item.");
                return false;
            }

            var tipoOrcamento = FindControl("hdfTipoOrcamento", "input").value;
            var subgrupoProdComposto = CadOrcamento.SubgrupoProdComposto(idProduto).value;

            if ((valor == "" || parseFloat(valor.replace(",", ".")) == 0) && !(tipoOrcamento == 1 && subgrupoProdComposto)) {
                alert("Informe o valor vendido.");
                saveProdClicked = false;
                return false;
            }

            if (qtde == "0" || qtde == "") {
                alert("Informe a quantidade.");
                saveProdClicked = false;
                return false;
            }

            valMin = new Number(valMin.replace(',', '.'));

            if (!FindControl("txtValorIns", "input").disabled && new Number(valor.replace(',', '.')) < valMin) {
                alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
                saveProdClicked = false;
                return false;
            }

            if (FindControl("txtAlturaIns", "input").disabled == false) {
                if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                    alert("Informe a altura.");
                    saveProdClicked = false;
                    return false;
                }
            }

            // Se o textbox da largura estiver habilitado, deverá ser informada
            if (FindControl("txtLarguraIns", "input").disabled == false && largura == "") {
                alert("Informe a largura.");
                saveProdClicked = false;
                return false;
            }

            if (!obrigarProcApl()) {
                saveProdClicked = false;
                return false;
            }

            // Calcula o ICMS do produto
            var aliquota = FindControl("hdfAliquotaIcmsProd", "input");
            var icms = FindControl("hdfValorIcmsProd", "input");
            icms.value = aliquota.value > 0 ? parseFloat(valor) * (parseFloat(aliquota.value) / 100) : 0;
            icms.value = icms.value.toString().replace('.', ',');

            if (FindControl("txtEspessura", "input") != null) {
                FindControl("txtEspessura", "input").disabled = false;
            }

            FindControl("txtAlturaIns", "input").disabled = false;
            FindControl("txtLarguraIns", "input").disabled = false;
            FindControl("txtValorIns", "input").disabled = false;

            var nomeControle = getNomeControleBenef();

            if (exibirControleBenef(nomeControle)) {
                var resultadoVerificacaoObrigatoriedade = verificarObrigatoriedadeBeneficiamentos();
                saveProdClicked = resultadoVerificacaoObrigatoriedade;
                return resultadoVerificacaoObrigatoriedade;
            }

            return true;
        }

        // Função chamada quando o produto está para ser atualizado
        function onUpdateProd(idProdOrcamento) {
            if (!validate("produto")) {
                return false;
            }

            atualizaValMin(idProdOrcamento);

            var valor = FindControl("txtValorIns", "input").value;
            var qtde = FindControl("txtQtdeIns", "input").value;
            var altura = FindControl("txtAlturaIns", "input").value;
            var idProduto = FindControl("hdfIdProduto", "input").value;
            var codInterno = FindControl("hdfCodInterno", "input").value;
            var valMin = FindControl("hdfValMin", "input").value;
            var tipoVenda = FindControl("hdfTipoVenda", "input");
            tipoVenda = tipoVenda != null ? tipoVenda.value : 0;
            valMin = new Number(valMin.replace(',', '.'));
            var tbConfigVidro = obterTbConfigVidro(idProdOrcamento);

            if (!FindControl("txtValorIns", "input").disabled && new Number(valor.replace(',', '.')) < valMin) {
                alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
                return false;
            }

            // Verifica se foi clicado no aplicar na telinha de beneficiamentos
            if (tbConfigVidro != null && tbConfigVidro.style.display == "block")
            {
                alert("Aplique as alterações no beneficiamento antes de salvar o item.");
                return false;
            }

            var tipoOrcamento = FindControl("hdfTipoOrcamento", "input").value;
            var subgrupoProdComposto = CadOrcamento.SubgrupoProdComposto(idProduto).value;

            if ((valor == "" || parseFloat(valor.replace(",", ".")) == 0) && !(tipoOrcamento == 1 && subgrupoProdComposto)) {
                alert("Informe o valor vendido.");
                return false;
            } else if (qtde == "0" || qtde == "") {
                alert("Informe a quantidade.");
                return false;
            } else if (FindControl("txtAlturaIns", "input").disabled == false) {
                if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                    alert("Informe a altura.");
                    return false;
                }
            }

            if (!obrigarProcApl()) {
                return false;
            }

            // Calcula o ICMS do produto
            var aliquota = FindControl("hdfAliquotaIcmsProd", "input");
            var icms = FindControl("hdfValorIcmsProd", "input");
            icms.value = parseFloat(valor) * (parseFloat(aliquota.value) / 100);
            icms.value = icms.value.toString().replace('.', ',');

            if (tbConfigVidro != null && FindControl("txtEspessura", "input", tbConfigVidro) != null) {
                FindControl("txtEspessura", "input", tbConfigVidro).disabled = false;
            } else if (FindControl("txtEspessura", "input") != null) {
                FindControl("txtEspessura", "input").disabled = false;
            }

            FindControl("txtAlturaIns", "input").disabled = false;
            FindControl("txtLarguraIns", "input").disabled = false;
            FindControl("txtValorIns", "input").disabled = false;

            var nomeControle = getNomeControleBenef();

            if (exibirControleBenef(nomeControle)) {
                var resultadoVerificacaoObrigatoriedade = verificarObrigatoriedadeBeneficiamentos();
                saveProdClicked = resultadoVerificacaoObrigatoriedade;
                return resultadoVerificacaoObrigatoriedade;
            }

            return true;
        }

        function exibirProdsComposicao(botao, idProd) {
            var grdProds = FindControl("grdProdutosOrcamento", "table");

            if (grdProds == null) {
                return;
            }

            for (var i = 0; i < grdProds.rows.length; i++) {
                var row = grdProds.rows[i];

                if (row.id.indexOf("produtoOrcamento_") != -1 && row.id.split('_')[1] != idProd) {
                    row.style.display = "none";
                }
            }

            var linha = document.getElementById("produtoOrcamento_" + idProd);
            var exibir = linha.style.display == "none";
            linha.style.display = exibir ? "" : "none";
            botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
            botao.title = (exibir ? "Esconder" : "Exibir") + " Produtos da Composição";

            if (FindControl("txtCodProdIns","input") != null) {
                FindControl("txtCodProdIns","input").parentElement.parentElement.style.display = !exibir ? "" : "none";
            }

            FindControl("hdfProdOrcamentoComposicaoSelecionado", "input").value = exibir? idProd : 0;
        }

        function exibirInfoAdicProd(num, botao) {
            for (iTip = 0; iTip < 2; iTip++) {
                TagToTip('tbInfoAdicProd_' + num, FADEIN, 300, COPYCONTENT, false, TITLE, 'Informações Adicionais', CLOSEBTN, true,
                    CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, false,
                    FIX, [botao, 9 - getTableWidth('tbInfoAdicProd_' + num), 7]);
            }
        }

    </script>

    <table id="mainTable" runat="server" clientidmode="Static" style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvOrcamento" runat="server" AutoGenerateRows="False" DataSourceID="odsOrcamento"
                    DefaultMode="Insert" GridLines="None" DataKeyNames="IdOrcamento">
                    <Fields>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <table cellpadding="2" cellspacing="2">
                                    <tr>
                                        <td align="left" class="cabecalho">Orçamento
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label7" runat="server" Text='<%# Eval("IdOrcamento") %>' Font-Size="Medium">
                                            </asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">Cliente
                                        </td>
                                        <td align="left" nowrap="nowrap" colspan="3">
                                            <asp:Label ID="Label10" runat="server" Text='<%# Eval("NomeClienteLista") %>'>
                                            </asp:Label>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td align="left" class="cabecalho">
                                            <asp:Label ID="lblProjetoEdit" runat="server" Text="Projeto">
                                            </asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label8" runat="server" Text='<%# Eval("IdProjeto") %>' Font-Size="Medium">
                                            </asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">Data
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label27" runat="server" Text='<%# Eval("DataCad") %>'>
                                            </asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">Situação
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label11" runat="server" Text='<%# Eval("DescrSituacao") %>'>
                                            </asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            <asp:Label ID="lblProjetoEdit0" runat="server" OnLoad="ctrlMedicao_Load" Text="Medição">
                                            </asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label26" runat="server" Text='<%# Eval("IdMedicao") %>' Font-Size="Medium" OnLoad="ctrlMedicao_Load">
                                            </asp:Label>
                                            <asp:Label ID="Label13" runat="server" Font-Size="Medium" OnLoad="ctrlMedicao_Load"
                                                Text='<%# Eval("IdMedicaoDefinitiva") != null ? "Definitiva: " + Eval("IdMedicaoDefinitiva") : "" %>'>
                                            </asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">Tipo Entrega
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label30" runat="server" Text='<%# Eval("DescrTipoEntrega") %>'>
                                            </asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">Vendedor
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label25" runat="server" Text='<%# Eval("NomeFuncionario") %>'>
                                            </asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho" nowrap="nowrap">Tipo
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label14" runat="server" Text='<%# Eval("DescrTipoOrcamento") %>'>
                                            </asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="cabecalho" nowrap="nowrap">Desconto
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label28" runat="server" Text='<%# Eval("TextoDesconto") %>'>
                                            </asp:Label>
                                        </td>
                                        <td align="left" class="cabecalho">Acréscimo
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label29" runat="server" Text='<%# Eval("TextoAcrescimo") %>'>
                                            </asp:Label>
                                        </td>
                                        <td class="cabecalho" nowrap="nowrap" style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "display: none": "" %>">Valor ICMS
                                        </td>
                                        <td align="left" nowrap="nowrap" style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "display: none": "" %>">
                                            <asp:Label ID="Label9" runat="server" Text='<%# Eval("ValorIcms", "{0:C}") %>' ForeColor="Red">
                                            </asp:Label>
                                        </td>
                                        <td class="cabecalho" nowrap="nowrap" style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIpiPedido ? "display: none": "" %>">Valor IPI
                                        </td>
                                        <td align="left" nowrap="nowrap" style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIpiPedido ? "display: none": "" %>">
                                            <asp:Label ID="Label12" runat="server" Text='<%# Eval("ValorIpi", "{0:C}") %>' ForeColor="Red">
                                            </asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="cabecalho" nowrap="nowrap">
                                            <asp:Label ID="lblValorFrete" runat="server" OnLoad="txtValorFrete_Load" Text="Valor do Frete">
                                            </asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label17" runat="server" Text='<%# Eval("ValorEntrega", "{0:C}") %>' OnLoad="txtValorFrete_Load">
                                            </asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="8" align="center" style="padding-left: 4px; white-space: nowrap">
                                            <table>
                                                <tr>
                                                    <td class="cabecalho">
                                                        <asp:Label ID="lblTitleTotal" runat="server" OnLoad="lblTotalGeral_Load" Text="Total">
                                                        </asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblTotal" runat="server" OnLoad="lblTotalGeral_Load" Text='<%# Eval("Total", "{0:C}") %>'>
                                                        </asp:Label>
                                                    </td>
                                                    <td class="cabecalho" nowrap="nowrap">
                                                        <asp:Label ID="lblTitleTotalBruto" runat="server" OnLoad="lblTotalBrutoLiquido_Load" Text="Total Bruto">
                                                        </asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblTotalBruto" runat="server" OnLoad="lblTotalBrutoLiquido_Load" Text='<%# Eval("TotalBruto", "{0:C}") %>'>
                                                        </asp:Label>
                                                    </td>
                                                    <td class="cabecalho" nowrap="nowrap">
                                                        <asp:Label ID="lblTitleTotalLiquido" runat="server" OnLoad="lblTotalBrutoLiquido_Load" Text="Total Líquido">
                                                        </asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblTotalLiquido" runat="server" OnLoad="lblTotalBrutoLiquido_Load" Text='<%# Eval("Total", "{0:C}") %>'>
                                                        </asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="8" style="padding-left: 4px; white-space: nowrap">
                                            <asp:Label ID="Label38" runat="server" ForeColor="Green" Style="white-space: nowrap" Text='<%# Eval("DescrMostrarTotal") %>'>
                                            </asp:Label>
                                            <asp:Label ID="Label39" runat="server" ForeColor="Green" Style="white-space: nowrap"
                                                Text='<%# (Eval("DescrMostrarTotal").ToString().Length > 0 && Eval("DescrMostrarTotalProd").ToString().Length > 0 ? "&nbsp;&nbsp;" : "") + Eval("DescrMostrarTotalProd").ToString() %>'>
                                            </asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">Obs. do Cliente
                                        </td>
                                        <td align="left" nowrap="nowrap" colspan="5">
                                            <asp:Label ID="lblObsCliente" runat="server" OnLoad="lblObsCliente_Load" Text='<%# Eval("ObsCliente") %>'>
                                            </asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                <asp:HiddenField ID="hdfTipoOrcamento" runat="server" Value='<%# Eval("TipoOrcamento") %>' />
                                <asp:HiddenField ID="hdfLiberarOrcamento" runat="server" Value='<%# Eval("LiberarOrcamento") %>' />
                                <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCliente") %>' />
                                <asp:HiddenField ID="hdfPercComissao" runat="server" Value='<%# Eval("PercComissao") %>' />
                                <asp:HiddenField ID="hdfCliRevenda" runat="server" OnLoad="hdfCliRevenda_Load" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:HiddenField ID="hdfCliRevenda" runat="server" OnLoad="hdfCliRevenda_Load" />
                                <table cellpadding="2" cellspacing="0">
                                    <tr>
                                        <td align="left" class="dtvHeader">Orçamento
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label7" runat="server" Font-Size="Medium" Text='<%# Eval("IdOrcamento") %>'>
                                            </asp:Label>
                                        </td>
                                        <td align="left" class="dtvHeader">
                                            <asp:Label ID="lblProjetoEdit" runat="server" Text="Projeto">
                                            </asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label8" runat="server" Font-Size="Medium" Text='<%# Eval("IdProjeto") %>'>
                                                        </asp:Label>
                                                        &nbsp;
                                                    </td>
                                                    <td class="dtvHeader">
                                                        <asp:Label ID="lblMedicaoEdit" runat="server" OnLoad="ctrlMedicao_Load" Text="Medição">
                                                        </asp:Label>
                                                        &nbsp;&nbsp;
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label26" runat="server" Text='<%# Bind("IdMedicao") %>' Font-Size="Medium" OnLoad="ctrlMedicao_Load">
                                                        </asp:Label>
                                                        <asp:Label ID="Label13" runat="server" Text='<%# Eval("IdMedicaoDefinitiva") != null ? "Def.: " + Eval("IdMedicaoDefinitiva") : "" %>' OnLoad="ctrlMedicao_Load">
                                                        </asp:Label>
                                                        <asp:HiddenField ID="hdfIdMedicaoDef" runat="server" Value='<%# Bind("IdMedicaoDefinitiva") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">
                                            <asp:Label ID="Label14" runat="server" Text="Loja" OnLoad="Loja_Load">
                                            </asp:Label>
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <uc8:ctrlLoja runat="server" ID="drpLoja" AutoPostBack="false" MostrarTodas="false" SelectedValue='<%# Bind("IdLoja") %>' OnLoad="Loja_Load" />
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
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Cliente
                                        </td>
                                        <td align="left" class="dtvAlternatingRow">
                                            <span style="white-space: nowrap">
                                                <asp:TextBox ID="txtIdCliente" runat="server" Text='<%# Eval("IdCliente") %>' onkeypress="return soNumeros(event, true, true)" onblur="getCli(this);" Width="50px">
                                                </asp:TextBox>
                                                <asp:TextBox ID="txtNomeCliente" runat="server" Text='<%# Bind("NomeCliente") %>' Width="280px" MaxLength="50">
                                                </asp:TextBox>
                                                <asp:ImageButton ID="imgGetCliente" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                    OnClientClick="openWindow(500, 700, '../Utils/SelCliente.aspx?dadosCliente=1&tipo=orcamento'); return false;" />
                                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                                            </span>
                                            <br />
                                            <asp:Label ID="lblObsCliente" runat="server" ForeColor="<%# GetCorObsCliente() %>">
                                            </asp:Label>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Situação
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>' DataSourceID="odsSituacao" DataTextField="Descr" DataValueField="Id">
                                                        </asp:DropDownList>
                                                        &nbsp;
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap" style="padding: 2px">Tipo
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpTipoOrcamento" runat="server" SelectedValue='<%# Bind("TipoOrcamento") %>'>
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
                                        <td align="left" class="dtvHeader" nowrap="nowrap">A/C
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtAC" runat="server" MaxLength="50" Text='<%# Bind("AosCuidados") %>'>
                                            </asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Contato
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox5" runat="server" MaxLength="20" Text='<%# Bind("Contato") %>' Width="200px">
                                            </asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Tel Res.
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTelRes" runat="server" MaxLength="15" onkeypress="return soTelefone(event)" onkeydown="return maskTelefone(event, this);" Text='<%# Bind("TelCliente") %>'>
                                            </asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Celular
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTelCel" runat="server" MaxLength="20" onkeypress="return soTelefone(event)" onkeydown="return maskTelefone(event, this);" Text='<%# Bind("CelCliente") %>' Width="200px">
                                            </asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Email
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="60" Text='<%# Bind("Email") %>' Width="250px">
                                            </asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Bairro
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="50" Text='<%# Bind("Bairro") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Endereço
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtEndereco" runat="server" MaxLength="70" Text='<%# Bind("Endereco") %>'
                                                Width="250px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Cidade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Text='<%# Bind("Cidade") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Vendedor
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                                DataTextField="Nome" DataValueField="IdFunc" SelectedValue='<%# Bind("IdFuncionario") %>'
                                                AppendDataBoundItems="True" OnDataBound="drpFuncionario_DataBound">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">CEP
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCep" runat="server" MaxLength="9" Text='<%# Bind("Cep") %>' onkeypress="return soCep(event)"
                                                onkeyup="return maskCep(event, this);"></asp:TextBox>
                                            <asp:ImageButton ID="imgPesquisarCep" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Prazo Entrega
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox8" runat="server" MaxLength="300" Text='<%# Bind("PrazoEntrega") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Validade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox7" runat="server" MaxLength="30" Text='<%# Bind("Validade") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Forma pagto.
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtFormaPagto" runat="server" MaxLength="200" Text='<%# Bind("FormaPagto") %>'
                                                Width="300px" TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Obra
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox6" runat="server" MaxLength="30" Text='<%# Bind("Obra") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Tipo Entrega</td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:DropDownList ID="ddlTipoEntrega" runat="server"
                                                AppendDataBoundItems="True" DataSourceID="odsTipoEntrega" DataTextField="Descr"
                                                DataValueField="Id" SelectedValue='<%# Bind("TipoEntrega") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEntrega" runat="server"
                                                SelectMethod="GetTipoEntrega" TypeName="Glass.Data.Helper.DataSources">
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Data
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc7:ctrlData ID="ctrlDataOrca" runat="server" ReadOnly="ReadWrite"
                                                DataString='<%# Bind("DataCad") %>' ExibirHoras="False" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Data Entrega
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc7:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadWrite"
                                                DataString='<%# Bind("DataEntrega") %>' ExibirHoras="False" />
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Total
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTotal" runat="server" ReadOnly="True" Text='<%# Eval("Total", "{0:C}") %>'></asp:TextBox>
                                            <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">&nbsp;Desconto
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
                                                    <td class="dtvHeader">&nbsp;Acréscimo&nbsp;
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
                                        <td class="dtvHeader" nowrap="nowrap" style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "display: none": "" %>">Valor ICMS
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
                                        <td class="dtvHeader">Local da Obra
                                        </td>
                                        <td>Endereço
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEnderecoObra" runat="server" MaxLength="70" Text='<%# Bind("EnderecoObra") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                        <td>Bairro
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtBairroObra" runat="server" MaxLength="50" Text='<%# Bind("BairroObra") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                        <td>Cidade
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCidadeObra" runat="server" MaxLength="50" Text='<%# Bind("CidadeObra") %>'
                                                Width="120px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <table id="tbComissionado" style="width: 100%" class="dtvHeader" cellpadding="3"
                                    cellspacing="0">
                                    <tr>
                                        <td align="left" style="padding-left: 0px">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>&nbsp;Comissionado:
                                                    </td>
                                                    <td>&nbsp;<asp:Label ID="lblComissionado" runat="server" Text='<%# Eval("NomeComissionado") %>'></asp:Label>
                                                    </td>
                                                    <td>&nbsp;<asp:LinkButton ID="lnkSelComissionado" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelComissionado.aspx'); return false;"
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
                                                    <td>Percentual:
                                                    </td>
                                                    <td>&nbsp;<asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                        Text='<%# Bind("PercComissao") %>' Width="50px" Enabled='<%# Glass.Configuracoes.PedidoConfig.Comissao.AlterarPercComissionado %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>Valor Comissão:
                                                    </td>
                                                    <td>&nbsp;<asp:TextBox ID="txtValorComissao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                        ReadOnly="True" Text='<%# Eval("ValorComissao", "{0:C}") %>' Width="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                                <table style="width: 100%" cellpadding="3" cellspacing="0">
                                    <tr>
                                        <td class="dtvHeader" colspan="2">Observação
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:TextBox ID="TextBox3" runat="server" MaxLength="1000" Text='<%# Bind("Obs") %>'
                                                TextMode="MultiLine" Width="600px"></asp:TextBox>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader" colspan="2">Observação Interna
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
                                            <asp:HiddenField ID="hfdTipoOrcamento" runat="server" Value='<%# Eval("TipoOrcamento") %>' />
                                            <asp:HiddenField ID="hdfValorComissao" runat="server" Value='<%# Bind("ValorComissao") %>' />
                                            <asp:HiddenField ID="hdfDataRecalcular" runat="server" Value='<%# Bind("DataRecalcular") %>' />
                                            <asp:HiddenField ID="hdfPercComissao" runat="server" Value='<%# Eval("PercComissao") %>' />
                                        </td>
                                        <td></td>
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
                                                        <asp:DropDownList ID="drpTipoVenda" runat="server"
                                                            SelectedValue='<%# Bind("Situacao") %>'>
                                                            <asp:ListItem Value="1">Em Aberto</asp:ListItem>
                                                            <asp:ListItem Value="2">Negociado</asp:ListItem>
                                                            <asp:ListItem Value="3">Não Negociado</asp:ListItem>
                                                        </asp:DropDownList>
                                                        &nbsp;
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap" style="padding: 2px">Tipo
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
                                            <asp:Label ID="Label14" runat="server" Text="Loja" OnLoad="Loja_Load"></asp:Label>
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
                                        <td align="left" class="dtvHeader" nowrap="nowrap">A/C
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtAC" runat="server" MaxLength="50" Text='<%# Bind("AosCuidados") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Celular
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTelCel" runat="server" MaxLength="20" onkeypress="return soTelefone(event)"
                                                onkeydown="return maskTelefone(event, this);" Text='<%# Bind("CelCliente") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Tel Res.
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtTelRes" runat="server" MaxLength="15" onkeypress="return soTelefone(event)"
                                                onkeydown="return maskTelefone(event, this);" Text='<%# Bind("TelCliente") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Contato
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtContato" runat="server" MaxLength="20" Text='<%# Bind("Contato") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Email
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="60" Text='<%# Bind("Email") %>'
                                                Width="244px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Bairro
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="50" Text='<%# Bind("Bairro") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Endereço
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtEndereco" runat="server" MaxLength="70" Text='<%# Bind("Endereco") %>'
                                                Width="244px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">CEP
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCep" runat="server" MaxLength="9" onkeypress="return soCep(event)"
                                                onkeyup="return maskCep(event, this);" Text='<%# Bind("Cep") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesquisarCep" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="iniciaPesquisaCep(FindControl('txtCep', 'input').value); return false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Cidade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="50" Text='<%# Bind("Cidade") %>'
                                                Width="244px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Validade
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="txtValidadeIns" runat="server" MaxLength="30" Text='<%# Bind("Validade") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Vendedor
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:DropDownList ID="drpFuncionario" runat="server" DataSourceID="odsFuncionario"
                                                DataTextField="Nome" DataValueField="IdFunc" SelectedValue='<%# Bind("IdFuncionario") %>'
                                                AppendDataBoundItems="True" OnDataBound="drpFuncionario_DataBound">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Obra
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:TextBox ID="TextBox6" runat="server" MaxLength="30" Text='<%# Bind("Obra") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Prazo Entrega
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:TextBox ID="txtPrazoEntregaIns" runat="server" MaxLength="300" Text='<%# Bind("PrazoEntrega") %>'
                                                Width="250px"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Tipo Entrega
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:DropDownList ID="ddlTipoEntrega" runat="server" DataSourceID="odsTipoEntrega"
                                                DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoEntrega") %>'
                                                AppendDataBoundItems="True">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoEntrega" runat="server" SelectMethod="GetTipoEntrega"
                                                TypeName="Glass.Data.Helper.DataSources">
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Forma pagto.
                                        </td>
                                        <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                            <asp:TextBox ID="txtFormaPagtoIns" runat="server" MaxLength="200" Text='<%# Bind("FormaPagto") %>'
                                                Width="300px" TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                        <td align="left" class="dtvHeader" nowrap="nowrap">Data Entrega
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
                                        <td class="dtvHeader">Local da Obra
                                        </td>
                                        <td>Endereço
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEnderecoObra" runat="server" MaxLength="70" Text='<%# Bind("EnderecoObra") %>'
                                                Width="200px"></asp:TextBox>
                                        </td>
                                        <td>Bairro
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtBairroObra" runat="server" MaxLength="50" Text='<%# Bind("BairroObra") %>'
                                                Width="150px"></asp:TextBox>
                                        </td>
                                        <td>Cidade
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCidadeObra" runat="server" MaxLength="50" Text='<%# Bind("CidadeObra") %>'
                                                Width="120px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <table id="tbComissionado" style="width: 100%" class="dtvHeader" cellpadding="0"
                                    cellspacing="0">
                                    <tr>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>&nbsp;Comissionado:
                                                    </td>
                                                    <td>&nbsp;<asp:Label ID="lblComissionado" runat="server" Text='<%# Eval("NomeComissionado") %>'></asp:Label>
                                                    </td>
                                                    <td>&nbsp;<asp:LinkButton ID="lnkSelComissionado" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelComissionado.aspx'); return false;"
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
                                                    <td>Percentual:
                                                    </td>
                                                    <td>&nbsp;<asp:TextBox ID="txtPercentual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                        Text='<%# Bind("PercComissao") %>' Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>Valor Comissão:
                                                    </td>
                                                    <td>&nbsp;<asp:TextBox ID="txtValorComissao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                        ReadOnly="True" Text='<%# Eval("ValorComissao", "{0:C}") %>' Width="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                                <table style="width: 100%">
                                    <tr>
                                        <td class="dtvHeader" colspan="2">Observação
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:TextBox ID="TextBox3" runat="server" MaxLength="1000" Text='<%# Bind("Obs") %>'
                                                TextMode="MultiLine" Width="600px"></asp:TextBox>
                                        </td>
                                        <td></td>
                                    </tr>

                                    <tr>
                                        <td class="dtvHeader" colspan="2">Observação Interna
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <asp:TextBox ID="TextBox4" runat="server" MaxLength="1000" Text='<%# Bind("ObsInterna") %>'
                                                TextMode="MultiLine" Width="600px"></asp:TextBox>
                                        </td>
                                        <td></td>
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
                                        <img border="0" src="../Images/cart_add.gif">&nbsp;Gerar Pedido Agrupado</img></asp:LinkButton>                                    <asp:LinkButton ID="lnkMedicaoDef" runat="server" Visible='<%# Eval("ExibirMedicaoDefinitiva") %>'
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
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkProjeto" OnClientClick="return openProjeto('', false);" runat="server"> Incluir Projeto</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="divProduto" runat="server">
                    <table>
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdProdutosAmbienteOrcamento" runat="server" DataSourceID="odsProdutosAmbienteOrcamento" DataKeyNames="IdProd"
                                    AllowPaging="True" ShowFooter="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                    OnPreRender="grdProdutosAmbienteOrcamento_PreRender" OnRowCommand="grdProdutosAmbienteOrcamento_RowCommand"
                                    OnRowUpdated="grdProdutosAmbienteOrcamento_RowUpdated" OnRowDeleted="grdProdutosAmbienteOrcamento_RowDeleted">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <!-- 0 -->
                                                <!-- EDITAR -->
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CausesValidation="False">
                                                    <img border="0" src="../Images/Edit.gif">
                                                    </img>
                                                </asp:LinkButton>
                                                <!-- REMOVER -->
                                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" CausesValidation="False"
                                                    OnClientClick="return confirm(&quot;Excluir este ambiente fará com que todos os produtos do mesmo sejam excluídos também, confirma exclusão?&quot;)" />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- ATUALIZAR -->
                                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" ValidationGroup="ambiente" />
                                                <!-- CANCELAR -->
                                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" CausesValidation="False" />
                                                <asp:HiddenField ID="hdfIdOrcamento" runat="server" Value='<%# Bind("IdOrcamento") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- INSERIR -->
                                                <asp:ImageButton ID="lnkAddProdutoAmbiente" runat="server" OnClientClick="addAmbiente(true); return false;" ImageUrl="~/Images/Insert.gif" CausesValidation="False" />
                                            </FooterTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">
                                            <ItemTemplate>
                                                <!-- 1 -->
                                                <!-- EXIBIR PRODUTOS -->
                                                <asp:LinkButton ID="lnkViewProd" runat="server" CausesValidation="False" CommandArgument='<%# Eval("IdProd") %>' CommandName="ViewProd" Text='<%# Eval("Ambiente") %>'
                                                    Visible='<%# !(bool)Eval("ProjetoVisible") %>'>
                                                </asp:LinkButton>
                                                <!-- EXIBIR PROJETO -->
                                                <asp:PlaceHolder ID="plhViewProjeto" Visible='<%# Eval("ProjetoVisible") %>' runat="server">
                                                    <a href="#" onclick='return openProjeto(<%# Eval("IdProd") %>)'>
                                                        <%# Eval("Ambiente") %>
                                                    </a>
                                                </asp:PlaceHolder>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- AMBIENTE -->
                                                <asp:TextBox ID="txtAmbienteIns" runat="server" Text='<%# Eval("Ambiente") %>' MaxLength="50" Width="150px"
                                                    onchange="FindControl('hdfDescrAmbienteIns', 'input').value = this.value">
                                                </asp:TextBox>
                                                <asp:HiddenField ID="hdfDescrAmbienteIns" Value='<%# Bind("Ambiente") %>' runat="server" />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- AMBIENTE -->
                                                <asp:TextBox ID="txtAmbienteIns" runat="server" MaxLength="50" Width="150px" onchange="FindControl('hdfDescrAmbienteIns', 'input').value = this.value">
                                                </asp:TextBox>
                                                <asp:HiddenField ID="hdfDescrAmbienteIns" runat="server" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                                            <ItemTemplate>
                                                <!-- 2 -->
                                                <!-- DESCRIÇÃO AMBIENTE -->
                                                <asp:Label ID="lblDescricaoAmbiente" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                                <asp:Label ID="lblDescricaoObsProj" runat="server" ForeColor="Red" Text='<%# Eval("DescrObsProj") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- DESCRIÇÃO AMBIENTE -->
                                                <asp:TextBox ID="txtDescricaoAmbienteIns" runat="server" Text='<%# Bind("Descricao") %>' Rows="2" TextMode="MultiLine" MaxLength="1000" Width="300px">
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- DESCRIÇÃO AMBIENTE -->
                                                <asp:TextBox ID="txtDescricaoAmbienteIns" runat="server" MaxLength="1000" Rows="2" TextMode="MultiLine" Width="300px">
                                                </asp:TextBox>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor produtos" SortExpression="TotalProdutos">
                                            <ItemTemplate>
                                                <!-- 3 -->
                                                <!-- TOTAL DOS PRODUTOS -->
                                                <asp:Label ID="lblTotalProd" runat="server" Text='<%# Bind("TotalProdutos", "{0:c}") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- TOTAL DOS PRODUTOS -->
                                                <asp:Label ID="lblTotalProd" runat="server" Text='<%# Eval("TotalProdutos", "{0:c}") %>'>
                                                </asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Acréscimo" SortExpression="Acrescimo">
                                            <ItemTemplate>
                                                <!-- 4 -->
                                                <!-- ACRÉSCIMO -->
                                                <asp:Label ID="lblAcrescimoAmbiente" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <!-- TIPO ACRÉSCIMO -->
                                                            <asp:DropDownList ID="drpTipoAcrescimo" runat="server" SelectedValue='<%# Bind("TipoAcrescimo") %>'>
                                                                <asp:ListItem Value="1">%</asp:ListItem>
                                                                <asp:ListItem Selected="True" Value="2">R$</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <!-- ACRÉSCIMO -->
                                                            <asp:TextBox ID="txtAcrescimo" runat="server" onkeypress="return soNumeros(event, false, true)" Text='<%# Bind("Acrescimo") %>' Width="50px">
                                                            </asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Desconto" SortExpression="Desconto">
                                            <ItemTemplate>
                                                <!-- 5 -->
                                                <!-- DESCONTO -->
                                                <asp:Label ID="lblDescontoAmbiente" runat="server" Text='<%# Eval("TextoDesconto") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <!-- TIPO DESCONTO -->
                                                            <asp:DropDownList ID="drpTipoDesconto" runat="server" SelectedValue='<%# Bind("TipoDesconto") %>'
                                                                onclick="calcularDesconto(2)">
                                                                <asp:ListItem Value="1">%</asp:ListItem>
                                                                <asp:ListItem Value="2">R$</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <!-- DESCONTO -->
                                                            <asp:TextBox ID="txtDesconto" runat="server" onchange="calcularDesconto(2)" onkeypress="return soNumeros(event, false, true)" Text='<%# Bind("Desconto") %>' Width="50px">
                                                            </asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfValorDescontoAtual" runat="server" Value='<%# Eval("ValorDescontoAtualAmbiente") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Negociar?" SortExpression="Negociar">
                                            <ItemTemplate>
                                                <!-- 6 -->
                                                <!-- NEGOCIAR? -->
                                                <asp:CheckBox ID="chkNegociar" runat="server" AutoPostBack="True" Checked='<%# Bind("Negociar") %>' OnCheckedChanged="chkNegociar_CheckedChanged"
                                                    Enabled='<%# Eval("IdAmbientePedido") == null || string.IsNullOrWhiteSpace(Eval("IdAmbientePedido").ToString()) || Eval("IdAmbientePedido").ToString() == "0" %>' />
                                                <asp:HiddenField ID="hdfIdProdOrcamento" runat="server" Value='<%# Eval("IdProd") %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lblNegociar" runat="server" Text='<%# (bool)Eval("Negociar") ? "Sim" : "Não" %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <!-- 7 -->
                                                <!-- IMAGEM -->
                                                <uc5:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemUrl") %>' Visible="<%# Glass.Configuracoes.OrcamentoConfig.UploadImagensOrcamento %>" />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- IMAGEM -->
                                                <div runat="server" id="imagemProdutoOrca" onload="imagemProdutoOrca_Load">
                                                    <uc5:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Eval("ImagemUrl") %>' />
                                                    <asp:ImageButton ID="imbExcluirImagem" runat="server" OnClientClick='if (!confirm("Deseja excluir a imagem desse produto?")) return false;'
                                                        ImageUrl="~/Images/ExcluirGrid.gif" Visible='<%# Eval("TemImagem") %>' CommandArgument='<%# Eval("IdProd") %>' OnClick="imbExcluirImagem_Click" />
                                                    <asp:FileUpload ID="fluImagem" runat="server" />
                                                </div>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- INSERIR -->
                                                <asp:LinkButton ID="lnkInsAmbiente" runat="server" OnClick="lnkInsAmbiente_Click" ValidationGroup="ambiente">
                                                <img border="0" src="../Images/ok.gif" />
                                                </asp:LinkButton>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle CssClass="edit"></EditRowStyle>
                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                </asp:GridView>
                                <asp:Label ID="lblAmbiente" runat="server" CssClass="subtitle1" Font-Bold="False"></asp:Label>
                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosAmbienteOrcamento" runat="server"
                                    DataObjectTypeName="Glass.Data.Model.ProdutosOrcamento" TypeName="Glass.Data.DAL.ProdutosOrcamentoDAO"
                                    EnablePaging="True" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                                    SelectMethod="PesquisarProdutosAmbienteOrcamento" SelectCountMethod="PesquisarProdutosAmbienteOrcamentoCountGrid"
                                    OnInserting="odsProdutosAmbienteOrcamento_Inserting" OnUpdating="odsProdutosAmbienteOrcamento_Updating" OnDeleting="odsProdutosAmbienteOrcamento_Deleting"
                                    OnUpdated="odsProdutosAmbienteOrcamento_Updated" OnDeleted="odsProdutosAmbienteOrcamento_Deleted"
                                    InsertMethod="InsertProdutoAmbienteComTransacao" UpdateMethod="UpdateProdutoAmbienteComTransacao" DeleteMethod="DeleteProdutoAmbienteComTransacao">
                                    <SelectParameters>
                                        <asp:Parameter Name="session" />
                                        <asp:QueryStringParameter Name="idOrcamento" QueryStringField="idOrca" Type="Int32" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                                <asp:HiddenField ID="hdfIdProdAmbienteOrcamento" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%# Eval("Ambiente") %>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdProdutosOrcamento" runat="server" DataSourceID="odsProdutosOrcamento" DataKeyNames="IdProd"
                                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" PageSize="12" ShowFooter="True" EditRowStyle-CssClass="edit"
                                    OnPreRender="grdProdutosOrcamento_PreRender" OnRowUpdating="grdProdutosOrcamento_RowUpdating" OnRowUpdated="grdProdutosOrcamento_RowUpdated" OnRowDeleted="grdProdutosOrcamento_RowDeleted">
                                    <FooterStyle Wrap="True" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <!-- 0 -->
                                                <!-- EDITAR -->
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>'>
                                                    <img border="0" src="../Images/Edit.gif" >
                                                    </img>
                                                </asp:LinkButton>
                                                <!-- REMOVER -->
                                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" Visible='<%# Eval("DeleteVisible") %>'
                                                    OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(false); return false" : "if (!confirm(\"Deseja remover esse produto do orçamento?\")) return false" %>' />
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- ATUALIZAR -->
                                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar"
                                                    OnClientClick='<%# "if(!onUpdateProd(" + Eval("IdProd") + ")) return false;"%>' />
                                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                                                <asp:HiddenField ID="hdfIdProdParent" runat="server" Value='<%# Eval("IdProdParent") %>' />
                                                <asp:HiddenField ID="hdfIdOrcamento" runat="server" Value='<%# Bind("IdOrcamento") %>' />
                                                <asp:HiddenField ID="hdfIdProduto" runat="server" Value='<%# Bind("IdProduto") %>' />
                                                <asp:HiddenField ID="hdfCodInterno" runat="server" Value='<%# Eval("CodInterno") %>' />
                                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                                <asp:HiddenField ID="hdfIsAluminio" runat="server" Value='<%# Eval("IsAluminio") %>' />
                                                <asp:HiddenField ID="hdfM2Minimo" runat="server" Value='<%# Eval("M2Minimo") %>' />
                                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                                <asp:HiddenField ID="hdfIdItemProjeto" runat="server" Value='<%# Bind("IdItemProjeto") %>' />
                                                <asp:HiddenField ID="hdfIdMaterItemProj" runat="server" Value='<%# Bind("IdMaterItemProj") %>' />
                                                <asp:HiddenField ID="hdfIdAmbientePedido" runat="server" Value='<%# Bind("IdAmbientePedido") %>' />
                                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" Value='<%# Bind("AliquotaIcms") %>' />
                                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" Value='<%# Bind("ValorIcms") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <select id="drpFooterVisible" style="display: none">
                                                </select>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                                            <ItemTemplate>
                                                <!-- 1 -->
                                                <!-- CODIGO -->
                                                <asp:Label ID="lblCodInterno" runat="server" Text='<%# Bind("CodInterno") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- CODIGO -->
                                                <asp:Label ID="lblCodProdIns" runat="server" Text='<%# Eval("CodInterno") %>'>
                                                </asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                            <ItemTemplate>
                                                <!-- 2 -->
                                                <!-- DESCRICAO COM BENEF -->
                                                <asp:Label ID="lblDescricaoProdutoComBenef" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- DESCRICAO COM BENEF -->
                                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'>
                                                </asp:Label>
                                                <!-- CUSTO -->
                                                <asp:HiddenField ID="hdfCustoProd" runat="server" Value='<%# Eval("Custo") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- CODIGO -->
                                                <asp:TextBox ID="txtCodProdIns" runat="server" onblur='<%# "loadProduto(this.value, 0);" %>' Width="50px"
                                                    onkeydown='<%# "if (isEnter(event)) loadProduto(this.value, 0);" %>' onkeypress="return !(isEnter(event));">
                                                </asp:TextBox>
                                                <!-- DESCRICAO -->
                                                <asp:Label ID="lblDescrProd" runat="server">
                                                </asp:Label>
                                                <a href="#" onclick="getProduto(); return false;">
                                                    <img src="../Images/Pesquisar.gif" border="0" />
                                                </a>
                                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" />
                                                <asp:HiddenField ID="hdfTipoCalc" runat="server" />
                                                <asp:HiddenField ID="hdfIsAluminio" runat="server" />
                                                <asp:HiddenField ID="hdfM2Minimo" runat="server" />
                                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" />
                                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" />
                                                <asp:HiddenField ID="hdfCustoProd" runat="server" Value='<%# Eval("Custo") %>' />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                            <ItemTemplate>
                                                <!-- 3 -->
                                                <!-- QUANTIDADE -->
                                                <asp:Label ID="lblQtde" runat="server" Text='<%# Bind("Qtde") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- QUANTIDADE -->
                                                <asp:TextBox ID="txtQtdeIns" runat="server" onblur="calcM2Prod();" Text='<%# Bind("Qtde") %>' Width="50px"
                                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);">
                                                </asp:TextBox>
                                                <!-- DESCONTO POR QUANTIDADE -->
                                                <uc4:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" Callback="calcTotalProd" OnLoad="ctrlDescontoQtde_Load" ValorDescontoQtde='<%# Bind("ValorDescontoQtde") %>'
                                                    CallbackValorUnit="calcTotalProd" ValidationGroup="produto" PercDescontoQtde='<%# Bind("PercDescontoQtde") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- QUANTIDADE -->
                                                <asp:TextBox ID="txtQtdeIns" runat="server" onkeydown="if (isEnter(event)) calcM2Prod();" Width="50px" onblur="calcM2Prod();"
                                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);">
                                                </asp:TextBox>
                                                <!-- DESCONTO POR QUANTIDADE -->
                                                <uc4:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" Callback="calcTotalProd" ValidationGroup="produto" CallbackValorUnit="calcTotalProd" OnLoad="ctrlDescontoQtde_Load" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                            <ItemTemplate>
                                                <!-- 4 -->
                                                <!-- LARGURA -->
                                                <asp:Label ID="lblLargura" runat="server" Text='<%# Bind("Largura") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- LARGURA -->
                                                <asp:TextBox ID="txtLarguraIns" runat="server" Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="50px"
                                                    onblur="calcM2Prod();" onkeypress="return soNumeros(event, true, true);">
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- LARGURA -->
                                                <asp:TextBox ID="txtLarguraIns" runat="server" onkeypress="return soNumeros(event, true, true);" onblur="calcM2Prod();" Width="50px">
                                                </asp:TextBox>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                            <ItemTemplate>
                                                <!-- 5 -->
                                                <!-- ALTURA -->
                                                <asp:Label ID="lblAltura" runat="server" Text='<%# Bind("Altura") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- ALTURA -->
                                                <asp:TextBox ID="txtAlturaIns" runat="server" onblur="calcM2Prod();" Text='<%# Bind("Altura") %>' Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"
                                                    onchange="FindControl('hdfAlturaCalcIns', 'input').value = this.value" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);">
                                                </asp:TextBox>
                                                <asp:HiddenField ID="hdfAlturaCalcIns" runat="server" Value='<%# Bind("AlturaCalc") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- ALTURA -->
                                                <asp:TextBox ID="txtAlturaIns" runat="server" Width="50px" onblur="calcM2Prod();" onchange="FindControl('hdfAlturaCalcIns', 'input').value = this.value"
                                                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);">
                                                </asp:TextBox>
                                                <asp:HiddenField ID="hdfAlturaCalcIns" runat="server" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                            <ItemTemplate>
                                                <!-- 6 -->
                                                <!-- TOTAL M2 -->
                                                <asp:Label ID="lblTotM" runat="server" Text='<%# Bind("TotM") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- TOTAL M2 -->
                                                <asp:Label ID="lblTotMIns" runat="server" Text='<%# Bind("TotM") %>'>
                                                </asp:Label>
                                                <asp:HiddenField ID="hdfTotM" runat="server" Value='<%# Eval("TotM") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- TOTAL M2 -->
                                                <asp:Label ID="lblTotMIns" runat="server">
                                                </asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotMCalc">
                                            <ItemTemplate>
                                                <!-- 7 -->
                                                <!-- TOTAL M2 CALCULADO -->
                                                <asp:Label ID="lblTotMCalc" runat="server" Text='<%# Eval("TotMCalc") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- TOTAL M2 CALCULADO -->
                                                <asp:Label ID="lblTotMCalcIns" runat="server" Text='<%# Eval("TotMCalc") %>'>
                                                </asp:Label>
                                                <asp:HiddenField ID="hdfTotMCalcIns" runat="server" Value='<%# Eval("TotMCalc") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- TOTAL M2 CALCULADO -->
                                                <asp:Label ID="lblTotMCalcIns" runat="server">
                                                </asp:Label>
                                                <asp:HiddenField ID="hdfTotMIns" runat="server" />
                                                <asp:HiddenField ID="hdfTotMCalcIns" runat="server" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorProd">
                                            <ItemTemplate>
                                                <!-- 8 -->
                                                <!-- VALOR VENDIDO -->
                                                <asp:Label ID="lblValor" runat="server" Text='<%# Bind("ValorProd", "{0:C}") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- VALOR VENDIDO -->
                                                <asp:TextBox ID="txtValorIns" runat="server" Text='<%# Bind("ValorProd") %>' Width="50px" OnLoad="txtValorIns_Load"
                                                    onblur="calcTotalProd();" onkeypress="return soNumeros(event, false, true);">
                                                </asp:TextBox>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- VALOR VENDIDO -->
                                                <asp:TextBox ID="txtValorIns" runat="server" Width="50px" OnLoad="txtValorIns_Load"
                                                    onkeydown="if (isEnter(event)) calcTotalProd();" onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProd();">
                                                </asp:TextBox>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                                            <ItemTemplate>
                                                <!-- 9 -->
                                                <!-- CODIGO PROCESSO -->
                                                <asp:Label ID="lblCodProcesso" runat="server" Text='<%# Bind("CodProcesso") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- CODIGO PROCESSO -->
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcIns" runat="server" Width="30px" Text='<%# Eval("CodProcesso") %>' onkeypress="return !(isEnter(event));"
                                                                onblur="loadProc(this.value);" onkeydown="if (isEnter(event)) { loadProc(this.value); }">
                                                            </asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a id="lnkProcesso" href="#" onclick='buscarProcessos(false); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" />
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- CODIGO PROCESSO -->
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcIns" runat="server" Width="30px" onblur="loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { loadProc(this.value); }" onkeypress="return !(isEnter(event));">
                                                            </asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a id="lnkProcesso" href="#" onclick='buscarProcessos(false); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" />
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                                            <ItemTemplate>
                                                <!-- 10 -->
                                                <!-- CODIGO APLICACAO -->
                                                <asp:Label ID="lblCodAplicacao" runat="server" Text='<%# Eval("CodAplicacao") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- CODIGO APLICACAO -->
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAplIns" runat="server" Text='<%# Eval("CodAplicacao") %>' Width="30px" onblur="loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { loadApl(this.value); }" onkeypress="return !(isEnter(event));">
                                                            </asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" />
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- CODIGO APLICACAO -->
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAplIns" runat="server" Width="30px" onblur="loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { loadApl(this.value); }" onkeypress="return !(isEnter(event));">
                                                            </asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" />
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Proc. Filhas" >
                                            <ItemTemplate>
                                                <!-- 11 -->
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- PROCESSO PEÇAS FILHAS -->
                                                <table class="pos" id="tblProcessoFilhas" style="display:none">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcInsFilhos" runat="server" onblur="procFilha=true; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { procFilha=true; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px">
                                                            </asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a id="lnkProcessoFilhos" href="#" onclick='procFilha=true; buscarProcessos(true); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" />
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcessoFilhos" runat="server" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Apl. Filhas">
                                            <ItemTemplate>
                                                <!-- 12 -->
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- APLICAÇÃO PEÇAS FILHAS -->
                                                <table class="pos" id="tblAplicacaoFilhos" style="display: none ">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtAplInsFilhos" runat="server" Width="30px" onblur="aplFilha=true; loadApl(this.value);"
                                                                onkeydown="if (isEnter(event)) { aplFilha=true; loadApl(this.value); }" onkeypress="return !(isEnter(event));">
                                                            </asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a id="lnkAplFilhos" href="#" onclick="aplFilha=true; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" />
                                                            </a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdAplicacaoFilhos" runat="server"/>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                            <ItemTemplate>
                                                <!-- 13 -->
                                                <!-- TOTAL -->
                                                <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("Total", "{0:C}") %>'>
                                                </asp:Label>
                                                <!-- PERCENTUAL DESCONTO QUANTIDADE -->
                                                <asp:Label ID="lblPercDescontoQtde" runat="server" Text='<%# "(Desconto de " + Eval("PercDescontoQtde") + "%)" %>' Visible='<%# (float)Eval("PercDescontoQtde") > 0 %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- TOTAL -->
                                                <asp:Label ID="lblTotalIns" runat="server" Text='<%# Bind("Total") %>' Style="padding-top: 4px">
                                                </asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- TOTAL -->
                                                <asp:Label ID="lblTotalIns" runat="server">
                                                </asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
                                            <ItemTemplate>
                                                <!-- 14 -->
                                                <!-- VALOR BENEFICIAMENTO -->
                                                <asp:Label ID="lblValorBenef" runat="server" Text='<%# Bind("ValorBenef", "{0:C}") %>'>
                                                </asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- VALOR BENEFICIAMENTO -->
                                                <asp:Label ID="lblValorBenef" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'>
                                                </asp:Label>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- VALOR BENEFICIAMENTO -->
                                                <asp:Label ID="lblValorBenef" runat="server">
                                                </asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <!-- 15 -->
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <!-- CONTROLE BENEFICIAMENTO -->
                                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick='<%# "exibirBenef(this, " + Eval("IdProd") + "); return false;" %>' Visible='<%# Eval("BenefVisible") %>'>
                                                    <img border="0" src="../Images/gear_add.gif" />
                                                </asp:LinkButton>
                                                <table id='<%# "tbConfigVidro_" + Eval("IdProd") %>' cellspacing="0" style="display: none;">
                                                    <tr align="left">
                                                        <td align="center">
                                                            <table>
                                                                <tr>
                                                                    <td class="dtvFieldBold">
                                                                        Espessura
                                                                        <asp:TextBox ID="txtEspessura" runat="server" OnDataBinding="txtEspessura_DataBinding"
                                                                            onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Bind("Espessura") %>'></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="dtvFieldBold" Visible='<%# Eval("IsProdLamComposicao") %>' >
                                                                        Aplicar Beneficiamentos Composição
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="chkAplicarBenefFilhos" runat="server" Visible='<%# Eval("IsProdLamComposicao") %>' Checked='<%# Bind("AplicarBenefComposicao") %>' />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc3:ctrlBenef ID="ctrlBenefEditar" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>' ValidationGroup="produto" OnInit="ctrlBenef_Load" Redondo='<%# Bind("Redondo") %>'
                                                                CallbackCalculoValorTotal="setValorTotal" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                        </td>
                                                    </tr>
                                                </table>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- CONTROLE BENEFICIAMENTO -->
                                                <asp:LinkButton ID="lnkBenef" runat="server" Style="display: none;" OnClientClick="exibirBenef(this, 0); return false;">
                                                    <img border="0" src="../Images/gear_add.gif" />
                                                </asp:LinkButton>
                                                <table id="tbConfigVidro_0" cellspacing="0" style="display: none;">
                                                    <tr align="left">
                                                        <td align="center">
                                                            <table>
                                                                <tr>
                                                                    <td class="dtvFieldBold">
                                                                        Espessura
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtEspessura" runat="server" onkeypress="return soNumeros(event, false, true);" Width="30px">
                                                                        </asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="dtvFieldBold" Visible='<%# Eval("IsProdLamComposicao") %>' >
                                                                        Aplicar Beneficiamentos Composição
                                                                    </td>
                                                                    <td>
                                                                        <asp:CheckBox ID="chkAplicarBenefFilhos" runat="server" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc3:ctrlBenef ID="ctrlBenefInserir" runat="server" OnInit="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotal" ValidationGroup="produto" />
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
                                            <ItemTemplate >
                                                <!-- 16 -->
                                                <!-- EXIBIÇÃO/IMAGENS PRODUTO COMPOSICAO -->
                                                <div id='<%# "imgProdsComposto_" + Eval("IdProd") %>'>
                                                    <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/box.png" ToolTip="Exibir Produtos da Composição"
                                                        Visible='<%# Eval("IsProdLamComposicao") %>' OnClientClick='<%# "exibirProdsComposicao(this, " + Eval("IdProd") + "); return false"%>' />
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/imagem.gif" ToolTip="Exibir imagem das peças"  Visible='<%# (Eval("IsVidro").ToString() == "true")%>'
                                                        OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=orcamento&idOrcamento=" + Eval("IdOrcamento") +"&idProd=" +  Eval("IdProd") +"&pecaAvulsa=" + ((bool)Eval("IsProdLamComposicao") == false) + "\"); return false" %>' />
                                                </div>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <!-- 17 -->
                                                <!-- CONTROLE IMPOSTO -->
                                                <a href="#" id="lnkInfoAdicProd" onclick="exibirInfoAdicProd(<%# Eval("IdProd") %>, this); return false;">
                                                    <img border="0" src="../../Images/tax.png" title="Informações Adicionais" width="16px"/></a>
                                                <table id='tbInfoAdicProd_<%# Eval("IdProd") %>' cellspacing="0" style="display: none;">
                                                     <tr>
                                                        <td align="left" style="font-weight: bold">Natureza de Operação
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label ID="lblNatOp" runat="server" Text='<%# Eval("CodNaturezaOperacao") %>'>
                                                            </asp:label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <!-- ALIQUOTA IPI -->
                                                        <td align="left" style="font-weight: bold">Aliq. IPI
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("AliquotaIpi") %>'>
                                                            </asp:label>
                                                        </td>
                                                        <!-- VALOR IPI -->
                                                        <td align="left" style="font-weight: bold">Valor IPI
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("ValorIpi") %>'>
                                                            </asp:label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <!-- ALIQUOTA ICMS -->
                                                        <td align="left" style="font-weight: bold">Aliq. ICMS
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("AliquotaIcms") %>'>
                                                            </asp:label>
                                                        </td>
                                                        <!-- BC ICMS -->
                                                        <td align="left" style="font-weight: bold">Bc. ICMS
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("BcIcms") %>'>
                                                            </asp:label>
                                                        </td>
                                                        <!-- VALOR ICMS -->
                                                        <td align="left" style="font-weight: bold">Valor ICMS
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("ValorIcms") %>'>
                                                            </asp:label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <!-- ALIQUOTA ICMS ST -->
                                                        <td align="left" style="font-weight: bold">Aliq. ICMS ST
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("AliqIcmsSt") %>'>
                                                            </asp:label>
                                                        </td>
                                                        <!-- BC ICMS ST -->
                                                        <td align="left" style="font-weight: bold">Bc. ICMS ST
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("BcIcmsSt") %>'>
                                                            </asp:label>
                                                        </td>
                                                        <!-- VALOR ICMS ST -->
                                                        <td align="left" style="font-weight: bold">Valor ICMS ST
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("ValorIcmsSt") %>'>
                                                            </asp:label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <!-- ALIQUOTA COFINS -->
                                                        <td align="left" style="font-weight: bold">Aliq. COFINS
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("AliqCofins") %>'>
                                                            </asp:label>
                                                        </td>
                                                        <!-- BC COFINS -->
                                                        <td align="left" style="font-weight: bold">Bc. COFINS
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("BcCofins") %>'>
                                                            </asp:label>
                                                        </td>
                                                        <!-- VALOR COFINS -->
                                                        <td align="left" style="font-weight: bold">Valor COFINS
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("ValorCofins") %>'>
                                                            </asp:label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <!-- ALIQUOTA PIS -->
                                                        <td align="left" style="font-weight: bold">Aliq. PIS
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("AliqPis") %>'>
                                                            </asp:label>
                                                        <!-- BC PIS -->
                                                        </td>
                                                            <td align="left" style="font-weight: bold">Bc. PIS
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("BcPis") %>'>
                                                            </asp:label>
                                                        </td>
                                                        <!-- VALOR PIS -->
                                                        <td align="left" style="font-weight: bold">Valor PIS
                                                        </td>
                                                        <td align="left" style="padding-left: 2px">
                                                            <asp:label runat="server" Text='<%# Eval("ValorPis") %>'>
                                                            </asp:label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                            </EditItemTemplate>
                                            <FooterTemplate>
                                                <!-- INSERIR -->
                                                <asp:ImageButton ID="lnkInsProd" runat="server" OnClick="lnkInsProd_Click" ImageUrl="../Images/ok.gif" OnClientClick="if (!onSaveProd()) return false;" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <!-- 18 -->
                                                <!-- CONTROLE PRODUTO COMPOSICAO -->
                                                <tr id="produtoOrcamento_<%# Eval("IdProd") %>" style="display: none" align="center">
                                                    <td colspan="19">
                                                        <br />
                                                        <uc9:ctrlProdComposicaoOrcamento runat="server" ID="ctrlProdComp" Visible='<%# Eval("IsProdLamComposicao") %>' IdProdOrcamento='<%# Glass.Conversoes.StrParaUint(Eval("IdProd").ToString()) %>'/>
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
                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosOrcamento" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosOrcamento" TypeName="Glass.Data.DAL.ProdutosOrcamentoDAO"
                                    EnablePaging="True" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                                    SelectMethod="PesquisarProdutosOrcamento" SelectCountMethod="PesquisarProdutosOrcamentoCountGrid" InsertMethod="Insert" UpdateMethod="UpdateComTransacao" DeleteMethod="Delete"
                                    OnInserting="odsProdutosOrcamento_Inserting" OnUpdating="odsProdutosOrcamento_Updating" OnDeleting="odsProdutosOrcamento_Deleting"
                                    OnUpdated="odsProdutosOrcamento_Updated" OnDeleted="odsProdutosOrcamento_Deleted">
                                    <SelectParameters>
                                        <asp:Parameter Name="session" />
                                        <asp:Parameter Name="idOrcamento" />
                                        <asp:ControlParameter ControlID="hdfIdProdAmbienteOrcamento" Name="idProdAmbienteOrcamento" PropertyName="Value" Type="Int32" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hdfIdProduto" runat="server" />
                <asp:HiddenField ID="hdfIdOrcamento" runat="server" />
                <asp:HiddenField ID="hdfComissaoVisible" runat="server" />
                <asp:HiddenField ID="hdfProdOrcamentoComposicaoSelecionado" runat="server" Value="0" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedoresOrca" TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idOrcamento" QueryStringField="idOrca" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsOrcamento" runat="server" DataObjectTypeName="Glass.Data.Model.Orcamento"
                    InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.OrcamentoDAO"
                    UpdateMethod="UpdateComTransacao" OnInserted="odsOrcamento_Inserted" OnUpdated="odsOrcamento_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idOrca" QueryStringField="idorca" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server" SelectMethod="GetSituacaoOrcamento" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        // Esconde controles de inserção de ambiente
        if (FindControl("lnkAddProdutoAmbiente", "input") != null) {
            addAmbiente(false);
        }

        // Esconde tabela de comissionado
        var hdfComissaoVisible = FindControl("hdfComissaoVisible", "input");
        var tbComissionado = FindControl("tbComissionado", "table");

        if (hdfComissaoVisible != null && tbComissionado != null && hdfComissaoVisible.value == "false") {
            tbComissionado.style.display = "none";
        }

        tipoEntrega = FindControl("ddlTipoEntrega", "select");
        tipoEntrega = tipoEntrega != null ? tipoEntrega.value : "";

        idCliente = FindControl("txtIdCliente", "input");
        idCliente = idCliente != null ? idCliente.value : "";

        var idCli = FindControl("txtIdCliente", "input");

        if (idCli != null && idCli.value != "") {
            var dados = CadOrcamento.GetCli(idCli.value).value;

            if (dados != null || dados != "" || dados.split('|')[0] != "Erro")
            {
                dados = dados.split("|");
                FindControl("lblObsCliente", "span").innerHTML = dados[12];
            }
        }

        $(document).ready(function () {
            var hdfProdOrcamentoComposicaoSelecionado = FindControl("hdfProdOrcamentoComposicaoSelecionado", "input");

            if (hdfProdOrcamentoComposicaoSelecionado.value > 0) {
                var div = FindControl("imgProdsComposto_" + hdfProdOrcamentoComposicaoSelecionado.value, "div");

                if (div == null) {
                    return;
                }

                var botao = FindControl("imgProdsComposto", "input", div);
                exibirProdsComposicao(botao, hdfProdOrcamentoComposicaoSelecionado.value);
            }
        });

        var codigoProduto = FindControl('txtCodProdIns', 'input');

        if (codigoProduto != null && codigoProduto.value != "") {
            codigoProduto.onblur();
        }

        loading = false;

    </script>

</asp:Content>
