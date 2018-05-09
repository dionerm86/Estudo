<%@ Page Title="Impressão de Etiquetas" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstEtiquetaImprimir.aspx.cs" Inherits="Glass.UI.Web.Listas.LstEtiquetaImprimir" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlSelFornecedor.ascx" TagName="ctrlSelFornecedor"
    TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlRetalhoAssociado.ascx" TagName="ctrlRetalhoAssociado"
    TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <style type="text/css">
        .tabela 
        {
            padding: 0;
            border-spacing: 0;
        }
        
        .tabela td
        {
            padding: 0 2px;
            margin: 0;
        }
    </style>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery-1.9.0.min.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jlinq/jlinq.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery/jquery.utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"></script>

    <script type="text/javascript">
        var produtoPlanoCorte = new Array();
        var usarPlanoCorte = <%= Glass.Configuracoes.PCPConfig.Etiqueta.UsarPlanoCorte.ToString().ToLower() %>;
        var exportarImportarOptyWay = "<%= IsExportacaoOptyWay().ToString().ToLower() %>";
        var permissaoImprimir = "<%= PermissaoParaImprimir().ToString().ToLower() %>";
        var permissaoImprimirMaoDeObra = "<%= PermissaoParaImprimirMaoDeObra().ToString().ToLower() %>";
        var permissaoImprimirNFe = "<%= PermissaoParaImprimirNFe().ToString().ToLower() %>";
        var produtosJaAdicionados = false;
        var temRetalhos = [];
        var controlesRetalhos = [];

        function isTipoImpressaoEtiquetaPedido(tipo)
        {
            return tipo == "<%= GetTipoImpressaoEtiquetaPedido() %>";
        }
        
        function textoTipoImpressaoEtiqueta(drpTipoEtiqueta) {
            return drpTipoEtiqueta.options[drpTipoEtiqueta.selectedIndex].text;
        }
        
        function buscarPedidos()
        {
            var idCorVidro = FindControl("drpCorVidro", "select").value;
            var espessura = FindControl("txtEspessura", "input").value;
            var idSubgrupoProd = FindControl("drpSubgrupoProd", "select").value;
            var alturaMin = FindControl("txtAlturaMin", "input").value;
            var alturaMax = FindControl("txtAlturaMax", "input").value;
            var larguraMin = FindControl("txtLarguraMin", "input").value;
            var larguraMax = FindControl("txtLarguraMax", "input").value;
            
            openWindow(600, 800, "../Utils/SelPedidoEspelhoImpressaoEtiqueta.aspx" +
                "?idCorVidro=" + idCorVidro + 
                "&espessura=" + espessura + 
                "&idSubgrupoProd=" + idSubgrupoProd + 
                "&alturaMin=" + alturaMin + 
                "&alturaMax=" + alturaMax + 
                "&larguraMin=" + larguraMin +
                "&larguraMax=" + larguraMax);
        }

        function alteraTipoEtiqueta(tipoEtiqueta)
        {
            var isPedido = isTipoImpressaoEtiquetaPedido(tipoEtiqueta.value);
            document.getElementById('fornec').style.display = isPedido ? 'none' : '';
            document.getElementById('loja1').style.display = isPedido ? '' : 'none';
            document.getElementById('loja2').style.display = isPedido ? '' : 'none';
            document.getElementById('buscarPedido').style.display = isPedido ? '' : 'none';
            document.getElementById('importar').style.display = isPedido ? '' : 'none';
            document.getElementById('processo1').style.display = isPedido ? '' : 'none';
            document.getElementById('processo2').style.display = isPedido ? '' : 'none';
            document.getElementById('aplicacao1').style.display = isPedido ? '' : 'none';
            document.getElementById('aplicacao2').style.display = isPedido ? '' : 'none';
            document.getElementById('subgrupo1').style.display = isPedido ? '' : 'none';
            document.getElementById('subgrupo2').style.display = isPedido ? '' : 'none';
            
            var lstProd = document.getElementById("lstProd");
            if (lstProd.rows.length > 0)
                lstProd.rows[0].cells[1].innerHTML = textoTipoImpressaoEtiqueta(tipoEtiqueta);
        }

        function esconderEmpresa() {
            return usarPlanoCorte;
        }

        function esconderBuscaPedido() {
            if (!esconderEmpresa())
                return;

            /*
            for (i = 1; i <= 3; i++)
                document.getElementById("buscar" + i).style.display = "none";
            */
        }

        function getProduto(idsProdPedNf, exibirErro, idCorVidro, espessura, idSubgrupoProd, alturaMin, alturaMax, larguraMin, larguraMax) {
            var numero = FindControl("txtNumero", "input");

            if (numero.value == "")
            {
                alert("Informe o número d" + (isPedido ? "o " : "a ") + textoTipoImpressaoEtiqueta(tipo) + ".");
                numero.focus();
                return false;
            }

            if (LstEtiquetaImprimir.PodeImprimirPedidoImportado(numero.value).value.toLowerCase() == "false") {
                alert("O pedido importado ainda não foi conferido, confira o mesmo antes de imprimir");
                return false;
            }
            
            var tipo = FindControl("drpTipoEtiqueta", "select");
            var isPedido = isTipoImpressaoEtiquetaPedido(tipo.value);
            var loja = FindControl("drpLoja", "select").value;
            var idProcesso = FindControl("drpProcesso", "select").value;
            var idAplicacao = FindControl("drpAplicacao", "select").value;
            
            idCorVidro = !!idCorVidro ? idCorVidro : FindControl("drpCorVidro", "select").value;
            espessura = !!espessura ? espessura : FindControl("txtEspessura", "input").value;
            idSubgrupoProd = !!idSubgrupoProd ? idSubgrupoProd : FindControl("drpSubgrupoProd", "select").value;
            alturaMin = !!alturaMin ? alturaMin : FindControl("txtAlturaMin", "input").value;
            alturaMax = !!alturaMax ? alturaMax : FindControl("txtAlturaMax", "input").value;
            larguraMin = !!larguraMin ? larguraMin : FindControl("txtLarguraMin", "input").value;
            larguraMax = !!larguraMax ? larguraMax : FindControl("txtLarguraMax", "input").value;

            var noCache = new Date();
            var response = null;
            idsProdPedNf = !!idsProdPedNf ? idsProdPedNf : "";

            if (isPedido)
                response = LstEtiquetaImprimir.GetProdByPedido(numero.value, loja, idProcesso, idAplicacao, idsProdPedNf, idCorVidro, espessura, 
                    idSubgrupoProd, alturaMin, alturaMax, larguraMin, larguraMax, noCache.getMilliseconds()).value;
            else
            {
                var idFornec = FindControl("ctrlSelFornecBuscar_hdfValor", "input").value;
                response = LstEtiquetaImprimir.GetProdByNFe(numero.value, idFornec, idsProdPedNf, idCorVidro, espessura, alturaMin, 
                    alturaMax, larguraMin, larguraMax, noCache.getMilliseconds()).value;
            }

            if (response == null) {
                alert("Falha ao buscar Produtos. AJAX Error.");
                return false;
            }

            response = response.split('\t');
            
            exibirErro = exibirErro == false ? false : true;

            if (response[0] == "Erro") {
                if(exibirErro)
                    alert(response[1]);
                return false;
            }

            var produtos = response[1].split('|');

            produtosJaAdicionados = false;

            for (j = 0; j < produtos.length; j++)
            {
                if (produtos[j] == "")
                    continue;
                
                var items = produtos[j].split(';');

                if (isPedido)
                {
                    setProdEtiqueta(items[0], items[1], items[2], "", "", items[3], items[4], items[5], items[6],
                        items[7], items[6] - items[7], items[8], items[9], items[10], items[11], null, null,
                        items[12] != '', items[12], null, items[13], null);
                }
                else
                {
                    setProdEtiqueta("", "", "", items[0], items[1], items[2], "", "", items[3], items[4],
                        items[3] - items[4], items[5], items[6], "", items[7], null, null, false, "", null, null, items[9]);
                }
            }
            
            if (produtosJaAdicionados)
                alert("Alguns produtos já haviam sido adicionados.");
            
            numero.value = "";
            numero.focus();

            return false;
        }
        
        function verificarProdutoPlanoCorte(id, planoCorte) {
            if (planoCorte != null && planoCorte != undefined) {
                for (j = 0; j < produtoPlanoCorte.length; j++)
                    if (produtoPlanoCorte[j][0] == id && produtoPlanoCorte[j][1] == planoCorte)
                        return true;

                return false;
            }
            else
                return true;
        }
        
        function numeroColunaEtiquetasExportadas()
        {
            var linhas = document.getElementById("lstProd").rows;
            if (linhas.length == 0)
                return -1;
            
            return linhas[0].cells.length - 2;
        }
        
        function verificaEtiquetasExportadas(controle, idProdPed, qtdeImprimir)
        {
            LstEtiquetaImprimir.NumeroEtiquetasExportadas(idProdPed, qtdeImprimir, function(response)
            {
                var linha = controle;
                while (linha.nodeName.toLowerCase() != "tr")
                    linha = linha.parentNode;
                
                linha.cells[numeroColunaEtiquetasExportadas()].innerHTML  = response.value;
            });
        }
        
        function formataObjId(etiqRepos, objId) {
            return (etiqRepos + objId).replace(/-/g, "_").replace(/\//g, "_");
        }

        /*
            Função chamada em:
            - Nesta tela na função javascript getProduto();
            - Utils/SelArquivoOtimizado.aspx, no método ASP btnImportarArquivo_Click
            - Utils/SelProdEtiqueta, na função javascript setProdEtiqueta
            - Utils/SelProdEtiqueta, na função ASP lnkAddAll_Click
            - Utils/SelEtiquetaNFe, na função javascript setProdEtiqueta
            - Utils/SelEtiquetaNFe, na função ASP lnkAddAll_Click
            - Utils/SelPedidoEspelhoImpressaoEtiqueta, na função javascript setPedido
            - Utils/SelPedidoEspelhoImpressaoEtiqueta, na funçao ASP lnkAddAll_Click
        */
        function setProdEtiqueta(idProdPed, idAmbiente, idPedido, idProdNf, idNf, descrProd, codProc, codApl, qtd, qtdImpresso, 
            qtdImprimir, altura, largura, obs, totM, planoCorte, selInstWin, arquivoOtimizado, etiquetas, atualizarTotais, totMCalc, lote) {
            
            document.getElementById("tabelaAlteraDataFab").style.display = "";
            
            // Verifica se o produto já foi adicionado
            var produtos = FindControl("hdfIdProdPedNf", "input").value.split(',');

            var tipoEtiqueta = FindControl("drpTipoEtiqueta", "select");
            var tipo = tipoEtiqueta.value;
            var textoEtiqueta = textoTipoImpressaoEtiqueta(tipoEtiqueta);
            var isPedido = isTipoImpressaoEtiquetaPedido(tipo);

            var idUsar = isPedido ? idPedido : idNf;
            var id = !isPedido ? "N" + idProdNf : idProdPed != "" && idProdPed != null ? idProdPed : "A" + idAmbiente;

            if (planoCorte != null && planoCorte != undefined)
                id += "_" + planoCorte;

            // Só faz a verificação de peças já adicionadas se não estiver clicado na opção "Adicionar todas" no popup
            //if (atualizarTotais == undefined || atualizarTotais == true)
            for (i = 0; i < produtos.length; i++) {
                if (id != produtos[i].replace("R", ""))
                    continue;

                // Verifica se a etiqueta já foi adicionada pelo id, caso seja reposição, não deve validar, pois pode acontecer em certos casos
                // do mesmo produto do pedido possuir uma peça de reposição e outras peças normais, o código ".replace("R", "")" foi removido por este
                // motivo, para permitir adicionar peças normais mesmo que o produto_pedido_espelho possua uma peça reposição nesta impressão
                var txtQtdImp = FindControl("txtQtdImp_" + idProdPed, "input");
                txtQtdImp = txtQtdImp == null ? FindControl("txtQtdImp_R" + idProdPed, "input") : txtQtdImp;
 
                if ((id == produtos[i]/*.replace("R", "")*/ && (qtdImprimir > 0 || txtQtdImp == null || txtQtdImp.value == "0")) ||
                // Não deixa adicionar a mesma peça de reposição duas vezes
                (id == produtos[i].replace("R", "") && produtos[i][0] == "R" && qtdImprimir == 0)) {
                    if (selInstWin != null)
                        selInstWin.alert("Produto já adicionado.");
 
                    produtosJaAdicionados = true;
                    return false;
                }
            }

            // Salva o pedido/NF-e da peça inserida para alterar a data de fabricação do mesmo caso necessite.
            var hdfIdsPedidoNFe = FindControl("hdfIdsPedidoNFe", "input");
            if (("," + hdfIdsPedidoNFe.value + ",").indexOf("," + idUsar + ",") == -1)
                hdfIdsPedidoNFe.value += idUsar + ",";
            
            // Se a etiqueta for de reposição, altera o nome dos controles
            var etiqRepos = "";

            if (qtdImprimir == 0 && etiquetas != "") {
                descrProd += " (" + etiquetas.split('_').length + " peça(s) de reposição)";
                etiqRepos += "R";
            }

            if (planoCorte != null && planoCorte != undefined && planoCorte != "") {
                if (planoCorte.toLowerCase().indexOf("arquivo") >= 0) {
                    alert("Plano de corte " + planoCorte + " inválido.");
                    return false;
                }
                produtoPlanoCorte.push(new Array(id, planoCorte));
            }

            arquivoOtimizado = arquivoOtimizado == true ? true : false;

            // txtQtdImprimir (Qtd que o usuário planeja imprimir) e hdfQtdImprimir (Qtd máxima que poderá ser impressa)
            var inputQtdImp = "<input name='txtQtdImp_" + etiqRepos + id + "' type='text' id='txtQtdImp_" + etiqRepos + id + "' " +
                (arquivoOtimizado ? "disabled='true'" : "") + " value='" + qtdImprimir + "' style='width: 30px' onkeypress='return soNumeros(event, true, true)' " +
                "onchange='verificaEtiquetasExportadas(this, \"" + idProdPed + "\", this.value)' />" +
                "<input type='hidden' name='hdfQtdImp_" + etiqRepos + id + "' id='hdfQtdImp_" + etiqRepos + id + "' value='" + qtdImprimir + "' />" +
                "<input type='hidden' name='hdfQtdImpresso_" + etiqRepos + id + "' id='hdfQtdImpresso_" + etiqRepos + id + "' value='" + qtdImpresso + "' />";

            // Observação a ser anexada junto à etiqueta, e Etiquetas retornadas do plano de corte (se houver)
            if (obs == null) obs = "";
            var inputObs = "<input name='txtObs_" + etiqRepos + id + "' type='text' id='txtObs_" + etiqRepos + id +
                "' value='" + obs + "' style='width: 100px' maxlength='200' /><input type='hidden' name='hdfEtiquetas_" +
                etiqRepos + id + "' id='hdfEtiquetas_" + etiqRepos + id + "' value='" + etiquetas + "' />";

            var linkRetalhos = "<input type='hidden' id='hdfIdRetalhosProducao_" + formataObjId(etiqRepos, id) + "' />";
            
            var dadosRetalhos = LstEtiquetaImprimir.GetLinkRetalhos(formataObjId(etiqRepos, id), etiqRepos + idProdPed, "txtQtdImp_" + etiqRepos + id).value.split("|");
            linkRetalhos += dadosRetalhos[0];
            var funcaoExecutar = dadosRetalhos[1];
            
            if (planoCorte != null && planoCorte != undefined) {
                // Adiciona item à tabela com o plano de corte
                addItem(new Array(idUsar, descrProd, largura + " x " + altura, totM, isPedido ? totMCalc : null, codProc, codApl, qtd, qtdImpresso, inputQtdImp, planoCorte, inputObs, '', linkRetalhos),
                    new Array(textoEtiqueta, 'Produto', 'Largura x Altura', 'Tot. M²', isPedido ? 'Tot. M² Calc.' : null, 'Proc.', 'Apl.', 'Qtd.', 'Qtd. já impresso', 'Qtd. a imprimir', 'Plano Corte', 'Obs', '', ''),
                    'lstProd', etiqRepos + id, "hdfIdProdPedNf", null, null, "callbackRemover", true);
            }
            else {
                // Adiciona item à tabela
                addItem(new Array(idUsar, descrProd, largura + " x " + altura, totM, isPedido ? totMCalc : null, codProc, codApl, qtd, qtdImpresso, inputQtdImp, inputObs, isPedido ? null : lote, '', linkRetalhos),
                    new Array(textoEtiqueta, 'Produto', 'Largura x Altura', 'Tot. M²', isPedido ? 'Tot. M² Calc.' : null, 'Proc.', 'Apl.', 'Qtd.', 'Qtd. já impresso', 'Qtd. a imprimir', 'Obs', isPedido ? null : 'Lote', '', ''),
                    'lstProd', etiqRepos + id, "hdfIdProdPedNf", null, null, "callbackRemover", true);
            }
            
            // Verifica as etiquetas exportadas
            FindControl("txtQtdImp_" + etiqRepos + id, "input").onchange();
            
            var linha = document.getElementById("lstProd_row" + (countItem["lstProd"] - 1));
            linha.cells[linha.cells.length - 1].width = "1px";
            
            if (funcaoExecutar)
                eval(funcaoExecutar);
            
            if (dadosRetalhos[2])
                controlesRetalhos.push(dadosRetalhos[2]);
            
            // Devem ser três iguais, para que caso o valor seja undefined não entre neste if
            if (atualizarTotais === false)
                return false;
            
            FindControl("drpTipoEtiqueta", "select").disabled = true;
            FindControl("lnkImprimir", "a").style.visibility = "visible";
            FindControl("lnkImprimirApenasPlanos", "a").style.visibility = "visible";

            if (esconderEmpresa()){
                var lnkVisivel = planoCorte != null && planoCorte != undefined ? "hidden" : "visible";
                FindControl("lnkArqOtimizacao", "a").style.visibility = lnkVisivel;
                if (FindControl("lnkArqOtimizacaoSemSag", "a") != null)
                    FindControl("lnkArqOtimizacaoSemSag", "a").style.visibility = lnkVisivel;
                if (FindControl("lnkArqOtimizacaoSemExportadas", "a") != null)
                    FindControl("lnkArqOtimizacaoSemExportadas", "a").style.visibility = lnkVisivel;
            }
            else{
                FindControl("lnkArqOtimizacao", "a").style.visibility = "visible";
                if (FindControl("lnkArqOtimizacaoSemSag", "a") != null)
                    FindControl("lnkArqOtimizacaoSemSag", "a").style.visibility = "visible";
                if (FindControl("lnkArqOtimizacaoSemExportadas", "a") != null)
                    FindControl("lnkArqOtimizacaoSemExportadas", "a").style.visibility = "visible";
            }

            atualizaTotais();
            
            return false;
        }
        
        function callbackRetalhos(nomeControle)
        {
            var controle = eval(nomeControle);
            temRetalhos[nomeControle] = controle.PossuiRetalhosFolga();
            
            var nova = FindControl("novaImpressao", "span").style.display != "none";
            document.getElementById("selecionarRetalhos").style.display = verificaTemRetalhos() && !nova ? "" : "none";
        }
        
        function callbackSelecaoRetalhos(nomeControle)
        {
            var nova = FindControl("novaImpressao", "span").style.display != "none";
            document.getElementById("imprimirApenasRetalhos").style.display = temRetalhosSelecionados(false, true, true) && !nova ? "" : "none";
        }
        
        function alterarAjaxRetalhos(parar)
        {
            for (var c in controlesRetalhos)
            {
                var controle = window[controlesRetalhos[c]];
                
                if (parar)
                    controle.Parar();
                else
                    controle.Iniciar();
            }
        }

        function callbackRemover(linha)
        {
            var tabela = linha;
            while (tabela.nodeName.toLowerCase() != "table")
                tabela = tabela.parentNode;

            var numeroLinhas = 0;
            for (i = 1; i < tabela.rows.length; i++)
                if (tabela.rows[i].style.display != 'none')
                    numeroLinhas++;
            
            // Remove a referência dos retalhos do controle
            var c = "ctrlRetalhos_" + formataObjId("", linha.getAttribute("objId"));
            
            if (!!window[c])
            {
                c = eval(c);
                c.IdProdPed = "";
                c.Parar();
            }
            
            FindControl("drpTipoEtiqueta", "select").disabled = numeroLinhas > 0;
            atualizaTotais();
        }

        function escondeRemoverUltimaLinha()
        {
            var tabela = document.getElementById("lstProd");
            var linha = tabela.rows.length - 1;
            while (linha >= 0 && tabela.rows[linha].style.display == "none")
                linha--;
            
            if (linha >= 0)
                tabela.rows[linha].cells[0].innerHTML = "";
        }

        function escondeRemoverTodasAsLinhas() {
            var tabela = document.getElementById("lstProd");
            var linha = tabela.rows.length - 1;

            while (linha >= 0)
            {
                tabela.rows[linha].cells[0].innerHTML = "";
                linha--;
            }
        }

        function atualizaTotais() {
            var totM = 0;
            var totMCalc = 0;

            var tipo = FindControl("drpTipoEtiqueta", "select");
            var isPedido = isTipoImpressaoEtiquetaPedido(tipo.value);

            var tabela = document.getElementById("lstProd");
            for (t = 1; t < tabela.rows.length; t++) {
                if (tabela.rows[t].style.display == "none")
                    continue;

                // Soma o total de m²
                var totM_Linha = parseFloat(tabela.rows[t].cells[4].innerHTML.replace(",", "."));
                totM += !isNaN(totM_Linha) ? totM_Linha : 0;

                // Soma o total de m² calc
                var totMCalc_Linha = parseFloat(tabela.rows[t].cells[5].innerHTML.replace(",", "."));
                totMCalc += !isNaN(totMCalc_Linha) ? totMCalc_Linha : 0;
            }

            FindControl("lblTotM", "span").innerHTML = totM.toFixed(2).replace(".", ",") + (isPedido? ' (' + totMCalc.toFixed(2).replace(".", ",") + ')':'');
        }
        
        function verificaTemRetalhos()
        {
            for (var i in temRetalhos)
                if (temRetalhos[i])
                    return true;
            
            return false;
        }
        
        function temRetalhosSelecionados(imprimir, apenasRetalhos, retornarSelecionados)
        {
            if (!verificaTemRetalhos())
                return false;
            
            var selecionados = false;
            var todasSemRetalhos = true;
            var linhas = document.getElementById("lstProd").rows;
            
            for (var i = 1; i < linhas.length; i++)
            {
                if (linhas[i].style.display == "none")
                    continue;
                    
                var c = "ctrlRetalhos_" + formataObjId("", linhas[i].getAttribute("objId"));
            
                if (window[c] == null || window[c] == undefined)
                    return false;
                
                if (document.getElementById(c + "_imgSelecionar").style.display != "none")
                {
                    todasSemRetalhos = false;
                    
                    var retalhosAssociados = eval(c).RetalhosAssociados();
                    selecionados = selecionados || retalhosAssociados != "";
                    
                    document.getElementById("hdfIdRetalhosProducao_" + formataObjId("", linhas[i].getAttribute("objId"))).value = retalhosAssociados;
                }
            }
            
            if (selecionados || todasSemRetalhos)
                return true;
            
            else if (!apenasRetalhos)
                return confirm("Há retalhos disponíveis para seleção.\nDeseja " + 
                    (imprimir ? "imprimir as peças" : "gerar o arquivo de otimização") + " sem selecioná-los?");
            
            else
                return !retornarSelecionados ? true : selecionados;
        }
        
        function temEtiquetasExportadas()
        {
            var linhas = document.getElementById("lstProd").rows;
            for (var i = 1; i < linhas.length; i++)
            {
                if (linhas[i].style.display == "none")
                    continue;
                
                if (linhas[i].cells[numeroColunaEtiquetasExportadas()].innerHTML != "")
                    return true;
            }
            
            return false;
        }

        // Gera PDF das etiquetas
        function imprimir(apenasPlano, apenasRetalho) {
            if (!permissaoImprimir && !permissaoImprimirMaoDeObra && !permissaoImprimirNFe){
                alert('Você não tem permissão para imprimir etiquetas.');
                return false;
            }
        
            if (!confirm('Tem certeza que deseja imprimir etiquetas para os produtos ' + 
                (apenasRetalho ? "com retalhos associados" : "selecionados") + '?'))
                return false;
            
            if (verificaTemRetalhos())
            {
                if (!temRetalhosSelecionados(true, apenasRetalho, false))
                    return false;
                else if (controlesRetalhos.length > 0 && !eval(controlesRetalhos[0]).Validar())
                    return false;
            }
            
            if (temEtiquetasExportadas() && !confirm("Há etiquetas exportadas para otimização nesta impressão.\nTem certeza que deseja imprimi-las?"))
                return false;
            
            FindControl("hdfSomenteRetalhos", "input").value = apenasRetalho ? "1" : "";
            
            // Verifica se existem impressões na situação "Processando"
            var podeImprimir = LstEtiquetaImprimir.PodeImprimir().value.split('|');
            if (podeImprimir[0] != "" && podeImprimir[0] != null) {
                alert("As impressões " + podeImprimir[0] + " estão em estado \"Processando\", cancele-as antes de imprimir outras etiquetas na listagem de impressões.");
                return false;
            }
            // Verifica se existem produtos de impressão sem impressão de etiqueta.
            else if (podeImprimir[1] == "true") {
                alert("Existem produtos de impressão sem impressão associada.");
                return false;
            }

            //Valida os retalhos selecionados
            $lstProd = $("table[id*=lstProd]");
            $rows = $lstProd.children("tbody").children("tr");

            var dadosRetalhos = [];
            $.each($rows, function(index, value)
            {
                if (index > 0 && $(this).css("display") != "none")
                {
                    var idProdPed = value.getAttribute("objId");
                    var idsRetalhos = $("input[id*=hdfIdRetalhosProducao]", value).val();
                    var qtdImpUsuario = FindControl("txtQtdImp_" + idProdPed, "input");
                    qtdImpUsuario = qtdImpUsuario != null && qtdImpUsuario != undefined ? qtdImpUsuario.value : 0;

                    if (idsRetalhos != "")
                        dadosRetalhos.push(idProdPed + "|" + idsRetalhos + "|" + qtdImpUsuario);
                }
            });

            var validaRetalhosResult = LstEtiquetaImprimir.ValidaRetalhos(dadosRetalhos);
            if(validaRetalhosResult.error != null){
                alert(validaRetalhosResult.error.description);
                return false;
            }


            // Desabilita link de impressão
            document.getElementById("lnkImprimir").style.visibility = "hidden";
            FindControl("lnkImprimirApenasPlanos", "a").style.visibility = "hidden";
            document.getElementById("selecionarRetalhos").style.display = "none";
            document.getElementById("imprimirApenasRetalhos").style.display = "none";
            
            FindControl("lnkArqOtimizacao", "a").style.visibility = "hidden";
            if (FindControl("lnkArqOtimizacaoSemSag", "a") != null)
                FindControl("lnkArqOtimizacaoSemSag", "a").style.visibility = "hidden";
            if (FindControl("lnkArqOtimizacaoSemExportadas", "a") != null)
                FindControl("lnkArqOtimizacaoSemExportadas", "a").style.visibility = "hidden";

            FindControl('novaImpressao', 'span').style.display = "";
            FindControl('lnkBuscarProd', 'a').style.visibility = "hidden";
            FindControl('txtNumero', 'input').disabled = true;
            FindControl('imgAddProd', 'input').disabled = true;
       
            // Abre tela de impressão de etiquetas
            openWindow(500, 700, '../Relatorios/RelEtiquetas.aspx?apenasPlano=' + apenasPlano);
            document.getElementById("reimprimir").style.display = verificaTemRetalhos() && apenasRetalho ? "" : "none";
            
            return false;
        }

        function limpar() {
            produtosJaAdicionados = false;
            temRetalhos = [];
            countItem["lstProd"] = 1;
            
            FindControl("lnkImprimir", "a").style.visibility = "hidden";
            FindControl("lnkImprimirApenasPlanos", "a").style.visibility = "hidden";
            document.getElementById("selecionarRetalhos").style.display = "none";
            document.getElementById("imprimirApenasRetalhos").style.display = "none";
            
            FindControl("novaImpressao", "span").style.display = "none";
            FindControl('lnkBuscarProd', 'a').style.visibility = "visible";
            FindControl('txtNumero', 'input').disabled = false;
            FindControl('imgAddProd', 'input').disabled = false;
            FindControl("drpTipoEtiqueta", "select").disabled = false;
            
            FindControl("ctrlSelFornecBuscar_txtDescr", "input").value = "";
            FindControl("ctrlSelFornecBuscar_hdfValor", "input").value = "";
            FindControl("lblNomeFornec", "span").innerHTML = "";

            FindControl("hdfIdProdPedNf", 'input').value = "";
            document.getElementById('lstProd').innerHTML = "";
        }

        function habilitarImpressao()
        {
            FindControl("novaImpressao", "span").style.display = "none";
            FindControl("lnkImprimir", "a").style.visibility = "visible";
            FindControl("lnkImprimirApenasPlanos", "a").style.visibility = "visible";
            document.getElementById("selecionarRetalhos").style.display = verificaTemRetalhos() ? "" : "none";
            document.getElementById("imprimirApenasRetalhos").style.display = verificaTemRetalhos() ? "" : "none";
            FindControl("lnkArqOtimizacao", "a").style.visibility = "hidden";
            if (FindControl("lnkArqOtimizacaoSemSag", "a") != null)
                FindControl("lnkArqOtimizacaoSemSag", "a").style.visibility = "hidden";
            if (FindControl("lnkArqOtimizacaoSemExportadas", "a") != null)
                FindControl("lnkArqOtimizacaoSemExportadas", "a").style.visibility = "hidden";
        }

        function openRpt() {
            var idOrdemInst = FindControl("hdfIdOrdemInst", "input").value;
            var idEquipe = FindControl("drpEquipe", "select").value;
            var dataInst = FindControl("txtDataInst", "input").value;

            var queryString = "?Rel=ListaOrdemInst&IdOrdemInst=" + idOrdemInst + "&idEquipe=" + idEquipe + "&dataInst=" + dataInst;

            openWindow("../Relatorios/RelBase.aspx" + queryString);
            return false;
        }

        function showArqOtimizacao(ignorarExportadas, ignorarSag) {
            if (verificaTemRetalhos())
            {
                if (!temRetalhosSelecionados(false, false, false))
                    return false;
                else if (controlesRetalhos.length > 0 && !eval(controlesRetalhos[0]).Validar())
                    return false;
            }
                    
            var idsProdPed = document.getElementById("<%= hdfIdProdPedNf.ClientID %>").value;

            // Remove a última ','
            idsProdPed = idsProdPed.substring(0, idsProdPed.length - 1);

            // Gera um vetor dos ids
            idsProdPed = idsProdPed.split(',');
                                    
            // Verifica se pedidos já foram exportados anteriormente
            var tipo = FindControl("drpTipoEtiqueta", "select").value;
            if (isTipoImpressaoEtiquetaPedido(tipo) && !ignorarExportadas) {
                var retorno = LstEtiquetaImprimir.ValidarPedidosJaExportados(idsProdPed).value.split('\t');

                if (retorno[0] == "Erro")
                    alert(retorno[1]);
                else if (retorno[0] == "true") {
                    if (!confirm("Os seguintes pedidos já foram exportados: " + retorno[1] + ". Tem certeza que deseja exportá-los novamente?"))
                        return false;
                }
            }
            
            var etiquetas = "";
            
            // Busca as qtde a serem impressas dos produtos
            for (var i = 0; i < idsProdPed.length; i++) {
                
                // Qtd informada para ser impressa
                var qtdImpUsuario = document.getElementById("txtQtdImp_" + idsProdPed[i]).value;

                // Qtd máxima que pode ser impressa
                var qtdImpMaxima = document.getElementById("hdfQtdImp_" + idsProdPed[i]).value;

                // Obs da etiqueta
                var obs = document.getElementById("txtObs_" + idsProdPed[i]).value;

                var retornoIsProdutoLaminadoComposicao = LstEtiquetaImprimir.IsProdutoLaminadoComposicao(idsProdPed[i]);

                if(retornoIsProdutoLaminadoComposicao.error != null)
                {
                    alert(retornoIsProdutoLaminadoComposicao.error.description);
                    return;
                }

                var isProdutoLaminadoComposicao = retornoIsProdutoLaminadoComposicao.value

                // Verifica se a quantidade a ser impressa foi informada
                if (qtdImpUsuario == "" && !isProdutoLaminadoComposicao) {
                    alert("Informe a quantidade a ser impressa de todos os itens.");
                    return false;
                }

                // Verifica se a qtde especificada para ser impressa é maior que a qtde máxima permitida
                if (parseInt(qtdImpUsuario) > parseInt(qtdImpMaxima)) {
                    alert("A quantidade especificada de um dos itens para ser impressa está maior que a quantidade máxima ('Qtd.' - 'Qtd já impresso').");
                    return false;
                }

                // Monta arquivo a ser passado para o handler para gerar a otimização
                etiquetas += idsProdPed[i] + ";" + qtdImpUsuario + ";" + obs;

                var etiqRepos = FindControl("hdfEtiquetas_" + idsProdPed[i], "input");
                if (etiqRepos != null) etiquetas += ";" + etiqRepos.value;

                etiquetas += "|";
            }

            var campoEtiquetas = document.getElementById("campoEtiquetas");

            if (campoEtiquetas == null) {
                campoEtiquetas = document.createElement("input");
                campoEtiquetas.id = "campoEtiquetas";
                campoEtiquetas.name = "etiquetas";
                document.formPost.appendChild(campoEtiquetas);
            }

            campoEtiquetas.value = etiquetas;

            if (exportarImportarOptyWay == "True")
                openWindow(300, 400, "../Utils/SelArquivoOtimizacao.aspx");
            else {
                document.formPost.action = "../Handlers/ArquivoOtimizacao.ashx" + (ignorarExportadas ? "?ignorarExportadas=true" : "") + (ignorarSag ? "?ignorarSag=true" : "");
                document.formPost.submit();
            }
        }

        function mostraData() {
            var controleData = FindControl("controleData", "div");
            controleData.style.display = 'block';
            var alterarReposicao = FindControl("alterarReposicao", "div");
            alterarReposicao.style.display = 'block';
        }

        function salvaData() {
            var ids = FindControl("hdfIdsPedido", "input").value;
            var data = FindControl("ctrlDataFabricacao_txtData", "input").value;
            var alterarReposicao = FindControl("chkAlterarReposicao", "input").checked;
            var response = LstEtiquetaImprimir.AlteraDataFabricacao(ids, data, alterarReposicao).value;
            alert(response);
            var controleData = FindControl("controleData", "div");
            controleData.style.display = 'none';
            var alterarReposicao = FindControl("alterarReposicao", "div");
            alterarReposicao.style.display = 'none';

            criarTabelaProducaoDataSemana();
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td id="buscar1">
                                        <asp:DropDownList ID="drpTipoEtiqueta" runat="server" onchange="alteraTipoEtiqueta(this)"
                                            DataSourceID="odsTipoImpressaoEtiqueta" DataTextField="Descr" DataValueField="Id">
                                        </asp:DropDownList>
                                        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoImpressaoEtiqueta" runat="server"
                                            SelectMethod="GetTipoImpressaoEtiqueta" TypeName="Glass.Data.Helper.DataSources">
                                        </colo:VirtualObjectDataSource>
                                    </td>
                                    <td id="buscar2">
                                        <table class="tabela">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtNumero" runat="server" Width="60px" onkeydown="if (isEnter(event)) return getProduto();"
                                                        onkeypress="if (isEnter(event)) return false;" ToolTip="Campo obrigatório."></asp:TextBox>
                                                </td>
                                                <td id="loja1" style="color: #0066FF">
                                                    Loja
                                                </td>
                                                <td id="loja2">
                                                    <uc5:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="true" VerificarSeControleDeveSerDesabilitado="false" />
                                                </td>
                                                <td id="fornec" style="display: none">
                                                    <table class="tabela">
                                                        <tr>
                                                            <td style="color: #0066FF">
                                                                Fornecedor*
                                                            </td>
                                                            <td>
                                                                <uc3:ctrlSelFornecedor ID="ctrlSelFornecedor" runat="server" />
                                                            </td>
                                                            <td style="color: Blue">
                                                                * O fornecedor pode ser deixado em
                                                                branco para buscar NF-e de Importação
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td id="processo1" style="color: #0066FF">
                                                    Processo
                                                </td>
                                                <td id="processo2">
                                                    <asp:DropDownList ID="drpProcesso" runat="server" AppendDataBoundItems="True" DataSourceID="odsProcesso"
                                                        DataTextField="CodInterno" DataValueField="IdProcesso">
                                                        <asp:ListItem Value="0">Todos</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td id="aplicacao1" style="color: #0066FF">
                                                    Aplicação
                                                </td>
                                                <td id="aplicacao2">
                                                    <asp:DropDownList ID="drpAplicacao" runat="server" AppendDataBoundItems="True" DataSourceID="odsAplicacao"
                                                        DataTextField="CodInterno" DataValueField="IdAplicacao">
                                                        <asp:ListItem Value="0">Todos</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td style="color: #0066FF">
                                        Cor
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpCorVidro" runat="server" AppendDataBoundItems="True" DataSourceID="odsCorVidro"
                                            DataTextField="Descricao" DataValueField="IdCorVidro">
                                            <asp:ListItem Value="0">Todas</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td style="color: #0066FF">
                                        Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEspessura" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                    </td>
                                    <td id="subgrupo1" style="color: #0066FF">
                                        Subgrupo
                                    </td>
                                    <td id="subgrupo2">
                                        <asp:DropDownList ID="drpSubgrupoProd" runat="server" DataSourceID="odsSubgrupo"
                                            DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td style="color: #0066FF">
                                        Altura
                                    </td>
                                    <td>
                                        mín.:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAlturaMin" runat="server" Width="50px"
                                            onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                    </td>
                                    <td>
                                        máx.:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAlturaMax" runat="server" Width="50px"
                                            onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                    </td>
                                    <td style="color: #0066FF">
                                        Largura
                                    </td>
                                    <td>
                                        mín.:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLarguraMin" runat="server" Width="50px"
                                            onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                    </td>
                                    <td>
                                        máx.:
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtLarguraMax" runat="server" Width="50px"
                                            onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="padding-left: 8px">
                            <asp:ImageButton ID="imgAddProd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="getProduto(); return false;"
                                ToolTip="Adicionar Instalação" Width="16px" />
                        </td>
                    </tr>
                </table>
                <table cellspacing="8">
                    <tr>
                        <td id="buscarPedido">
                            <a href="#" id="lnkBuscarPedido" onclick="buscarPedidos(); return false;"
                                style="font-size: small;">Buscar Pedidos</a>
                        </td>
                        <td id="buscar3">
                            <a href="#" id="lnkBuscarProd" 
                                 onclick="<%= "openWindow(500, 750, FindControl('drpTipoEtiqueta', 'select').value == 1 ? '../Utils/SelProdEtiqueta.aspx' : '../Utils/SelEtiquetaNFe.aspx'); return false;" %>"
                                style="font-size: small;">Buscar Produtos</a>
                        </td>
                        <td id="importar">
                            <asp:LinkButton ID="lnkImportarArquivo" runat="server" Style="font-size: small;"
                                OnClientClick="return openWindow(500, 700, '../Utils/SelArquivoOtimizado.aspx');"> Importar Arquivo de Otimização</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div id="producaoDataEntrega">
                </div>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="lstProd" align="left" cellpadding="4" cellspacing="0" width="100%">
                </table>
                <table style="padding-top: 10px">
                    <tr>
                        <td style="font-weight: bold; font-size: 130%">
                            Total M²
                        </td>
                        <td style="font-size: 130%">
                            <asp:Label ID="lblTotM" runat="server" Text="0,00"></asp:Label>
                            m²
                        </td>
                    </tr>
                </table>
                <table style="padding-top: 10px; display: none" id="tabelaAlteraDataFab">
                    <tr>
                        <td>
                            <asp:HyperLink ID="lnkALterarDataFab" NavigateUrl="#" runat="server" onclick="mostraData();">Alterar Data Fabricação</asp:HyperLink>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center; padding: 10px;">
                            <div id="controleData" style="display: none">
                                <uc1:ctrlData ID="ctrlDataFabricacao" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                <asp:HyperLink ID="lnkSalvaData" NavigateUrl="#" runat="server" onclick="salvaData();"
                                    Style="margin-left: 10px;">Salvar</asp:HyperLink>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center; padding: 10px;">
                            <div id="alterarReposicao" style="display: none">
                                <asp:CheckBox ID="chkAlterarReposicao" runat="server" Checked="true" Text="Alterar pedidos com/de reposição" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <a href="#" id="lnkArqOtimizacao" onclick="return showArqOtimizacao(false, false);" style="visibility: hidden">
                    <img alt="" border="0" src="../Images/blocodenotas.png" />
                    Arquivo de Otimização</a> 
                &nbsp;&nbsp;&nbsp;&nbsp;
                <a href="#" id="lnkArqOtimizacaoSemSag" onclick="return showArqOtimizacao(false, true);" style="visibility: hidden"
                    runat="server" clientidmode="Static">
                    <img alt="" border="0" src="../Images/blocodenotas.png" />
                    Arquivo de Otimização (Sem arquivo SAG)</a>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <a href="#" id="lnkArqOtimizacaoSemExportadas" onclick="return showArqOtimizacao(true, false);" style="visibility: hidden"
                    runat="server" clientidmode="Static">
                    <img alt="" border="0" src="../Images/blocodenotas.png" />
                    Arquivo de Otimização (Ignorar etiquetas exportadas anteriormente)</a>
                
                <span id="selecionarRetalhos" style="display: none">&nbsp;&nbsp;&nbsp;&nbsp;
                        <a href="#" onclick="alterarAjaxRetalhos(true); openWindow(600, 800, '../Utils/AtribuirRetalhos.aspx?popup=true'); return false"
                            id="lnkSelRetalhos">Selecionar/otimizar retalhos</a> </span>
                <br />
                <br />
                <a href="#" id="lnkImprimir" onclick="imprimir(false, false); return false" style="visibility: hidden">
                    <img alt="" border="0" src="../Images/printer.png" />
                    Imprimir</a>&nbsp;&nbsp;&nbsp;&nbsp; <a href="#" id="lnkImprimirApenasPlanos" onclick="imprimir(true); return false"
                        style="visibility: hidden">
                        <img alt="" border="0" src="../Images/printer.png" />
                        Imprimir apenas planos de corte</a> <span id="imprimirApenasRetalhos" style="display: none">
                            &nbsp;&nbsp;&nbsp;&nbsp; <a href="#" onclick="alterarAjaxRetalhos(true); imprimir(false, true); return false"
                                id="lnkImprimirRetalhos">
                                <img alt="" border="0" src="../Images/printer.png" />
                                Imprimir (apenas peças com retalhos)</a></span> <span id="novaImpressao" style="display: none">
                                    <br />
                                    <br />
                                    <asp:Button ID="btnNova" runat="server" Text="Nova Impressão" OnClick="btnNova_Click" />
                                    <span id="reimprimir" style="display: none">&nbsp;&nbsp;&nbsp;&nbsp;
                                        <asp:Button ID="btnReimprimir" runat="server" Text="Imprimir peças restantes" OnClientClick="atualizarPagina(); return false" />
                                    </span></span>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdProdPedNf" runat="server" />
                <asp:HiddenField ID="hdfIdsPedidoNFe" runat="server" Value="" />
                <asp:HiddenField ID="hdfSomenteRetalhos" runat="server" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProcesso" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.EtiquetaProcessoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAplicacao" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.EtiquetaAplicacaoDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCorVidro" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.CorVidroDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="idGrupo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <div style="display: none">
                    <!-- Não é usado -->
                    <uc4:ctrlRetalhoAssociado ID="ctrlRetalhoAssociado1" runat="server" />
                </div>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        esconderBuscaPedido();
        alteraTipoEtiqueta(document.getElementById("<%= drpTipoEtiqueta.ClientID %>"));

        // Não permite imprimir por plano de corte se a empresa não utiliza esse controle
        if (!usarPlanoCorte)
            FindControl("lnkImprimirApenasPlanos", "a").style.display = "none";

        $(document).ready(function()
        {
            <% if (ItensOtimizacao != null)
            { %>
            var itensOtimizacao = <%= Newtonsoft.Json.JsonConvert.SerializeObject(ItensOtimizacao) %>;

            for(var i=0; i<itensOtimizacao.length; i++) {
                var item = itensOtimizacao[i];
                setProdEtiqueta(
                    item.IdProdPed, item.IdAmbiente, item.IdPedido, item.IdProdNf, item.IdNf, 
                    item.DescricaoProduto, item.CodProcesso, item.CodAplicacao, item.Qtd,
                    item.QtdImpresso, item.QtdImprimir, item.Altura, item.Largura, 
                    item.Obs, item.TotM2, item.PlanoCorte, null, item.ArquivoOtimizado, 
                    item.Etiquetas, item.AtualizarTotais, item.TotMCalc, item.Lote);
            }

            <%}
            %>

            criarTabelaProducaoDataSemana();
        });

        function criarTabelaProducaoDataSemana()
        {
            $("#result").empty();
            $("#grid").empty();
            $.ajax({
                type: "POST",
                url: "../Service/WebGlassService.asmx/ObterProducaoDataEntregaSemana",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response) {
                    if (response.d.Status == 1) {
                        $message = $("<p style='color: #FF0000; font-size: 16px; padding-left: 20px; background-color: #FFFF00'></p>");
                        $message.text(response.d.Message);
                        $("#result").append($message);
                    }
                    else if (response.d.Status == 0) {
                        var Model = response.d.Object;

                        var producaoDataList = new Array();

                        for (i = 0; i < Model.length; i++) {
                            var producaoData = new Object();

                            var data = new Date(parseInt(Model[i].DataHora.substr(6)));
                            var dia = data.getDate() < 10 ? "0" + data.getDate() : data.getDate();
                            var mes = (data.getMonth() + 1) < 10 ? "0" + (data.getMonth() + 1) : (data.getMonth() + 1);
                            var ano = data.getFullYear();
                            var str_data = dia + '/' + mes + '/' + ano;

                            producaoData.Data = str_data;
                            producaoData.Pendente = Model[i].Pronto == 0 ? Model[i].TotM2 : 0;
                            producaoData.Pronto = Model[i].Pronto == 1 ? Model[i].TotM2 : 0;
                            producaoData.Total = producaoData.Pendente + producaoData.Pronto;
                            producaoData.ProducaoMaximaDia = Model[i].ProducaoMaximaDia;

                            var item = $.grep(producaoDataList, function(e) {
                                return e.Data == str_data;
                            });

                            if (item.length == 0)
                                producaoDataList.push(producaoData);
                            else if (item.length == 1) {
                                if (Model[i].Pronto == 0)
                                    item[0].Pendente = Model[i].TotM2;
                                else if (Model[i].Pronto == 1)
                                    item[0].Pronto = Model[i].TotM2;

                                producaoDataList = $.grep(producaoDataList, function(e) {
                                    return e.Data != str_data;
                                });

                                item[0].Total = item[0].Pendente + item[0].Pronto;

                                producaoDataList.push(item[0]);
                            }
                            else {
                                alert("Erro");
                                false;
                            }
                        }

                        var $grid = $("#grid");
                        $grid.append("<span style='font-weight:bold; margin-bottom:5px'>* Data Fábrica</span>");
                        var $table = $("<table class='gridStyle'>");
                        $table.css("width", "100%");

                        // thread
                        $table.append("<thead>").children('thead').append('<tr />');
                        var $tbody = $table.append('<tbody />').children('tbody').append('<tr/>');

                        for (i = 0; i < producaoDataList.length; i++) {
                            var producaoData = producaoDataList[i];

                            var color = "#000000";

                            if (producaoData.ProducaoMaximaDia > 0) {
                                if (producaoData.Total > producaoData.ProducaoMaximaDia)
                                    color = "Red";
                                else
                                    color = "Green";
                            }

                            $table.children("thead").children("tr").append("<th>" + producaoData.Data + "</th>");
                            var html = "<table style='width:100%; text-align:center'><thead><tr><th>Pendente</th><th>Pronto</th></tr>";
                            html += "<tbody><tr><td>" + producaoData.Pendente.toFixed(2) + "</td><td>" + producaoData.Pronto.toFixed(2) + "</td></tr>";

                            if (producaoData.ProducaoMaximaDia > 0)
                                html += "<tr><td style='font-weight:bold'>Prod. Máx. Dia:</td><td>" + producaoData.ProducaoMaximaDia.toFixed(2) + "</td></tr>";

                            html += "<tr style='background: #F6F6F6;'><td colspan='2' style='font-weight:bold;text-align:center'>Total: <span style='color:" + color + "'>" + producaoData.Total.toFixed(2) + "</span></td></tbody>";
                            $tbody.children('tr:last').append("<td style='text-align:center'>" + html + "</td>");
                        }

                        $grid.append($table);
                    }
                },
                error: function(response) {
                    $message = $("<p style='color: #FF0000; font-size: 16px; padding-left: 20px; background-color: #FFFF00'></p>");
                    if (response.d != undefined)
                        $message.text(response.d.Message);
                    else
                        $message.text(response.responseText);
                    $("#result").append($message);
                }
            });
        }
    </script>

    <div id="grid" style="text-align: left">
    </div>
</asp:Content>
