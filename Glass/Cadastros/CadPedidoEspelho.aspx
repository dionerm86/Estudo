<%@ Page Title="Pedido em Conferência" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadPedidoEspelho.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadPedidoEspelho" %>

<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlDadosDesconto.ascx" TagName="ctrlDadosDesconto"
    TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        // Verifica se o produto que está sendo inserido é um alumínio
        var prodInsAluminio = false;

        var produtoAmbiente = false;
        var aplAmbiente = false;
        var procAmbiente = false;
        var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;
        var pedidoReposicao = <%= VerificaPedidoReposicao() %>;
        // Guarda a quantidade disponível em estoque do produto buscado
        var qtdEstoque = 0;
        var exibirMensagemEstoque = false;
        var qtdEstoqueMensagem = 0;

        function limparComissionado()
        {
            FindControl("hdfIdComissionado", "input").value = "";
            FindControl("lblComissionado", "span").innerHTML = "";
            FindControl("txtPercentual", "input").value = "0";
            FindControl("txtValorComissao", "input").value = "R$ 0,00";
        }

        function setComissionado(id, nome, percentual) {
            FindControl("lblComissionado", "span").innerHTML = nome;
            FindControl("hdfIdComissionado", "input").value = id;
            FindControl("txtPercentual", "input").value = percentual;
        }

        function finalizar()
        {
            if (!confirm("Finalizar pedido?"))
                return false;

            return checkCapacidadeProducaoSetor();
        }

        function atualizaValMin()
        {
            if (parseFloat(FindControl("hdfTamanhoMaximoObra", "input").value.replace(",", ".")) == 0)
            {
                var codInterno = FindControl("txtCodProdIns", "input");
                codInterno = codInterno != null ? codInterno.value : FindControl("lblCodProdIns", "span").innerHTML;

                var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;
                var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
                var tipoPedido = FindControl("hdfTipoPedido", "input").value;
                var cliRevenda = FindControl("hdfCliRevenda", "input").value;
                var idCliente = FindControl("hdfIdCliente", "input").value;
                var altura = FindControl("txtAlturaIns", "input").value;

                var idProdPed = FindControl("hdfProdPed", "input");
                idProdPed = idProdPed != null ? idProdPed.value : "";

                var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
                controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

                var percDescontoQtde = controleDescQtde.PercDesconto();
                var reposicao = FindControl("hdfIsReposicao", "input").value;

                var retorno = CadPedidoEspelho.GetValorMinimo(codInterno, tipoPedido, tipoEntrega,
                    idCliente, cliRevenda, reposicao, idProdPed, percDescontoQtde, idPedido, altura);

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
            else
                FindControl("hdfValMin", "input").value = FindControl("lblValorIns", "input") != null ? FindControl("lblValorIns", "input").value : "";
        }

        function calculaTamanhoMaximo()
        {
            var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;
            var codInterno = FindControl("lblCodProdIns", "span").innerHTML;
            var totM2 = FindControl("lblTotM2Ins", "span").innerHTML;
            var idProdPed = FindControl("hdfProdPed", "input").value;

            var tamanhoMaximo = CadPedidoEspelho.GetTamanhoMaximoProduto(idPedido, codInterno, totM2, idProdPed).value.split(";");
            tamanhoMaximo = tamanhoMaximo[0] == "Ok" ? parseFloat(tamanhoMaximo[1].replace(",", ".")) : 0;

            FindControl("hdfTamanhoMaximoObra", "input").value = tamanhoMaximo;
        }

        function validaTamanhoMax()
        {
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

        function getNomeControleBenef()
        {
            var nomeControle = "<%= NomeControleBenef() %>";
            nomeControle = FindControl(nomeControle + "_tblBenef", "table");

            if (nomeControle == null)
                return null;

            nomeControle = nomeControle.id;
            return nomeControle.substr(0, nomeControle.lastIndexOf("_"));
        }

        function obrigarProcApl()
        {
            var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
            var isVidroBenef = exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1;
            var isVidroRoteiro = dadosProduto.Grupo == 1 && <%= UtilizarRoteiroProducao().ToString().ToLower() %>;
            var tipoCalculo = FindControl("hdfTipoCalc", "input") != null && FindControl("hdfTipoCalc", "input") != undefined && FindControl("hdfTipoCalc", "input").value != undefined ? FindControl("hdfTipoCalc", "input").value : "";

            /* Chamado 63268. */
            if ((tipoCalculo != "" && (tipoCalculo == "2" || tipoCalculo == "10")) && (isVidroRoteiro || (isObrigarProcApl && isVidroBenef)))
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

        function desmembrar(idProdPed, qtde)
        {
            var valor = null;

            do {
                valor = prompt("Quantidade do produto: " + qtde + "\nDeseja separar quantos desses produtos?", "");
                if (valor == null)
                    return;

                valor = parseInt(valor, 10);
                if (isNaN(valor) || valor == 0 || valor > (qtde - 1))
                    alert("Número inválido. Digite um número entre 1 e " + (qtde - 1) + ".");
                else
                    break;
            }
            while (true);

            var resposta = CadPedidoEspelho.Desmembrar(idProdPed, valor).value.split(';');
            alert(resposta[1]);

            if (resposta[0] == "Ok")
                redirectUrl(window.location.href);
        }

        // Se o produto sendo adicionado for ferragem e se a empresa for charneca, informa se qtd vendida
        // do produto existe no estoque
        function verificaEstoque() {
            var txtQtd = FindControl("txtQtdeIns", "input").value;

            var estoqueMenor = txtQtd != "" && parseInt(txtQtd) > parseInt(qtdEstoque);
            if (estoqueMenor)
            {
                if (qtdEstoque == 0)
                    alert("Não há nenhuma peça deste produto no estoque.");
                else
                    alert("Há apenas " + qtdEstoque + " peça(s) deste produto no estoque.");

                FindControl("txtQtdeIns", "input").value = "";
            }

            var exibirPopup = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ExibePopupVidrosEstoque.ToString().ToLower() %>;
            if (exibirPopup && exibirMensagemEstoque && (qtdEstoqueMensagem <= 0 || estoqueMenor))
                openWindow(400, 600, "../Utils/DadosEstoque.aspx?idProd=" + FindControl("hdfIdProd", "input").value);
        }

        function exibirBenef(botao) {
            for (iTip = 0; iTip < 2; iTip++) {
                TagToTip('tbConfigVidro', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true,
                    CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                    FIX, [botao, 9 - getTableWidth('tbConfigVidro'), -41 - getTableHeight('tbConfigVidro')]);
            }
        }

        function getProduto() {
            openWindow(450, 700, '../Utils/SelProd.aspx?IdPedido=<%= Request["IdPedido"] %>' + (produtoAmbiente ? "&ambiente=true" : ""));
        }

        // Função chamada para mostrar/esconder controles para inserção de novo ambiente
        function exibirEsconderAmbiente(value) {
            var ambiente = FindControl("txtAmbiente", "input");
            if (ambiente == null)
                ambiente = FindControl("ambMaoObra", "div");

            var descricao = FindControl("txtDescricao", "textarea");
            if (ambiente == null && descricao == null)
                return;

            if (descricao != null)
                descricao.style.display = value ? "" : "none";

            if (ambiente != null)
                ambiente.style.display = value ? "" : "none";

            var qtde = FindControl("txtQtdeAmbiente", "input");
            var altura = FindControl("txtAlturaAmbiente", "input");
            var largura = FindControl("txtLarguraAmbiente", "input");
            var redondo = FindControl("chkRedondoAmbiente", "input");
            var apl = FindControl("txtAmbAplIns", "input");
            apl = apl != null ? apl.parentNode.parentNode.parentNode : null;
            var proc = FindControl("txtAmbProcIns", "input");
            proc = proc != null ? proc.parentNode.parentNode.parentNode : null;

            if (qtde != null)
                qtde.style.display = value ? "" : "none";

            if (altura != null)
                altura.style.display = value ? "" : "none";

            if (largura != null)
                largura.style.display = value ? "" : "none";

            if (redondo != null) {
                if (value) {
                    redondo.style.display = "";

                    if (altura.value != "" && largura != "" &&
                        altura.value != largura.value &&
                        redondo.checked) {
                        alert('O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.');
                        redondo.checked = false;
                    }
                }
                else
                    redondo.style.display = "none";
            }

            if (apl != null)
                apl.style.display = value ? "" : "none";

            if (proc != null)
                proc.style.display = value ? "" : "none";

            FindControl("lnkInsAmbiente", "a").style.display = value ? "" : "none";
        }

        function setValorTotal(valor, custo) {
            if (exibirControleBenef(getNomeControleBenef())) {
                var lblValorBenef = FindControl("lblValorBenef", "span");
                lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
            }
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

        // Carrega dados do produto com base no código do produto passado
        function loadProduto(codInterno) {
            // Esconde link para inserir vários alumínios
            if (FindControl("lnkAddAluminio", "a") != null)
                FindControl("lnkAddAluminio", "a").style.visibility = "hidden";

            if (codInterno == "")
                return false;

            if(FindControl("txtProcIns", "input") != null)
                FindControl("txtProcIns", "input").value = "";

            var verificaProduto = CadPedidoEspelho.IsProdutoObra(idPedido, codInterno, false).value.split(";");
            if (verificaProduto[0] == "Erro")
            {
                FindControl("txtCodProd", "input").value = "";
                alert("Esse produto não pode ser usado no pedido. " + verificaProduto[1]);
                return false;
            }
            else if (parseFloat(verificaProduto[1].replace(",", ".")) > 0)
            {
                if (FindControl("txtValorIns", "input") != null)
                    FindControl("txtValorIns", "input").disabled = true;

                // Se for edição de produto, chama o método padrão de cálculo da metragem máxima permitida
                if (FindControl("hdfProdPed", "input") != null)
                    calculaTamanhoMaximo();
                else if (FindControl("hdfTamanhoMaximoObra", "input") != null)
                    FindControl("hdfTamanhoMaximoObra", "input").value = verificaProduto[2];
            }
            else
            {
                if (FindControl("txtValorIns", "input") != null)
                    FindControl("txtValorIns", "input").disabled = verificaProduto[3] == "false";

                FindControl("hdfTamanhoMaximoObra", "input").value = "0";
            }

            var idLojaSubgrupo = CadPedidoEspelho.ObterLojaSubgrupoProd(codInterno);
            var idLoja = FindControl("hdfIdLoja", "input").value;

            if(idLojaSubgrupo.error!=null){

                if (FindControl("txtCodProd", "input") != null)
                    FindControl("txtCodProd", "input").value = "";

                alert(idLojaSubgrupo.error.description);
                return false;
            }

            if(idLojaSubgrupo.value != "" && !idLojaSubgrupo.value.includes(idLoja)){

                if (FindControl("txtCodProd", "input") != null)
                    FindControl("txtCodProd", "input").value = "";

                alert('Esse produto não pode ser utilizado, pois a loja do seu subgrupo é diferente da loja do pedido.');
                return false;
            }

            var validaClienteSubgrupo = MetodosAjax.ValidaClienteSubgrupo(FindControl("hdfIdCliente", "input").value, codInterno);
            if(validaClienteSubgrupo.error!=null){

                if (FindControl("txtCodProd", "input") != null)
                    FindControl("txtCodProd", "input").value = "";

                alert(validaClienteSubgrupo.error.description);
                return false;
            }

            try {
                var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
                controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

                var percDescontoQtde = controleDescQtde.PercDesconto();
                var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
                var cliRevenda = FindControl("hdfCliRevenda", "input").value;
                var idCliente = FindControl("hdfIdCliente", "input").value;
                var tipoPedido = FindControl("hdfTipoPedido", "input").value;

                var retorno = CadPedidoEspelho.GetProduto(idPedido, codInterno, tipoEntrega, cliRevenda, idCliente, tipoPedido,
                    produtoAmbiente, percDescontoQtde, FindControl("hdfIdLoja", "input").value).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    if (!produtoAmbiente)
                        FindControl("txtCodProd", "input").value = "";
                    else
                        FindControl("txtCodAmb", "input").value = "";

                    return false;
                }

                else if (!produtoAmbiente) {
                    if (retorno[0] == "Prod") {
                        FindControl("hdfIdProd", "input").value = retorno[1];
                        FindControl("lblValorIns", "span").innerHTML = verificaProduto[1] != "0" ? verificaProduto[1] : retorno[3]; // Exibe no cadastro o valor mínimo do produto
                        FindControl("hdfValorIns", "input").value = verificaProduto[1] != "0" ? verificaProduto[1] : retorno[3];;
                        FindControl("hdfIsVidro", "input").value = retorno[4]; // Informa se o produto é vidro
                        FindControl("hdfIsAluminio", "input").value = retorno[5]; // Informa se o produto é alumínio
                        FindControl("hdfM2Minimo", "input").value = retorno[6]; // Informa se o produto possui m² mínimo
                        FindControl("hdfTipoCalc", "input").value = retorno[7]; // Verifica como deve ser calculado o produto
                        var tipoCalc = retorno[7];

                        if(FindControl("txtAlturaIns", "input") != null && FindControl("txtAlturaIns", "input").value != ""){
                            GetAdicionalAlturaChapa();
                        }

                        qtdEstoque = retorno[13]; // Pega a quantidade disponível em estoque deste produto
                        exibirMensagemEstoque = retorno[14] == "true";
                        qtdEstoqueMensagem = retorno[15];

                        atualizaValMin();

                        // Se o produto for alumínio, mostra link para inserir vários alumínios
                        if (retorno[5] == "true")
                        {
                            if (FindControl("lnkAddAluminio", "a") != null)
                                FindControl("lnkAddAluminio", "a").style.visibility = "visible";
                        }
                        // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                        else if (retorno[4] == "true" && (retorno[7] == "2" || retorno[7] == "10") && FindControl("lnkBenef", "a") != null) {
                            FindControl("txtEspessura", "input").value = retorno[8];
                            FindControl("txtEspessura", "input").disabled = retorno[8] != "" && retorno[8] != "0";
                        }

                        var nomeControle = getNomeControleBenef();

                        if (FindControl("lnkBenef", "a") != null && nomeControle != null && nomeControle.indexOf("Inserir") > -1)
                            FindControl("lnkBenef", "a").style.display = exibirControleBenef(nomeControle) ? "" : "none";

                        // Se o produto não for vidro, desabilita os textboxes largura e altura,
                        // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
                        // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
                        var cAltura = FindControl("txtAlturaIns", "input");
                        var cLargura = FindControl("txtLarguraIns", "input");
                        var maoDeObra = FindControl("hdfPedidoMaoDeObra", "input").value == "true";
                        var alturaAmbiente = FindControl("hdfAlturaAmbiente", "input").value;
                        var larguraAmbiente = FindControl("hdfLarguraAmbiente", "input").value;
                        cAltura.disabled = maoDeObra || CalcProd_DesabilitarAltura(tipoCalc);
                        cLargura.disabled = maoDeObra || CalcProd_DesabilitarLargura(tipoCalc);

                        if (maoDeObra && alturaAmbiente > 0) {
                            cAltura.value = tipoCalc != 1 && tipoCalc != 5 ? alturaAmbiente : "";
                            FindControl("hdfAlturaCalcIns", "input").value = cAltura.value;
                        }

                        if (maoDeObra && larguraAmbiente > 0)
                            cLargura.value = tipoCalc != 1 && tipoCalc != 4 && tipoCalc != 5 && tipoCalc != 6 && tipoCalc != 7 && tipoCalc != 8 ? larguraAmbiente : "";

                        FindControl("hdfAliquotaIcmsProd", "input").value = retorno[9].replace(".", ",");

                        // Se o campo altura e largura forem maior que 0, exibe-os
                        if (retorno[11] > 0) {
                            FindControl("txtAlturaIns", "input").value = retorno[11];
                            FindControl("hdfAlturaCalcIns", "input").value = retorno[11];
                        }
                        if (retorno[12] > 0) {
                            FindControl("txtLarguraIns", "input").value = retorno[12];
                            FindControl("hdfLarguraCalc", "input").value = retorno[12];
                        }

                        if (cAltura.disabled && FindControl("hdfAlturaCalcIns", "input") != null)
                            FindControl("hdfAlturaCalcIns", "input").value = cAltura.value;

                        if (retorno[16] != "")
                            setApl(retorno[16], retorno[17]);

                        if (retorno[18] != "")
                            setProc(retorno[18], retorno[19], "");
                    }

                    FindControl("lblDescrProd", "span").innerHTML = retorno[2];
                }
                else {
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

        // Função chamada pelo popup de escolha da Aplicação do produto
        function setApl(idAplicacao, codInterno) {

            var idPedido = '<%= Request["idPedido"] %>';

            var verificaEtiquetaApl = MetodosAjax.VerificaEtiquetaAplicacao(idAplicacao, idPedido);
            if(verificaEtiquetaApl.error != null){

                if (!aplAmbiente)
                {
                    FindControl("txtAplIns", "input").value = "";
                    FindControl("hdfIdAplicacao", "input").value = "";
                }
                else
                {
                    FindControl("txtAmbAplIns", "input").value = "";
                    FindControl("hdfAmbIdAplicacao", "input").value = "";
                }

                alert(verificaEtiquetaApl.error.description);
                return false;
            }

            if (!aplAmbiente)
            {
                FindControl("txtAplIns", "input").value = codInterno;
                FindControl("hdfIdAplicacao", "input").value = idAplicacao;
            }
            else
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

            var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProd", "input").value);
            var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

            if(idSubgrupo.value != "" && retornoValidacao.value == "false" && FindControl("txtProcIns", "input").value != "")
            {
                FindControl("txtProcIns", "input").value = "";
                alert("Este processo não pode ser selecionado para este produto.")
                return false;
            }

            var idPedido = '<%= Request["idPedido"] %>';

            var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, idPedido);
            if(verificaEtiquetaProc.error != null){

                if (!procAmbiente)
                {
                    FindControl("txtProcIns", "input").value = "";
                    FindControl("hdfIdProcesso", "input").value = "";
                }
                else
                {
                    FindControl("txtAmbProcIns", "input").value = "";
                    FindControl("hdfAmbIdProcesso", "input").value = "";
                }

                setApl("", "");

                alert(verificaEtiquetaProc.error.description);
                return false;
            }

            if (!procAmbiente)
            {
                FindControl("txtProcIns", "input").value = codInterno;
                FindControl("hdfIdProcesso", "input").value = idProcesso;
            }
            else
            {
                FindControl("txtAmbProcIns", "input").value = codInterno;
                FindControl("hdfAmbIdProcesso", "input").value = idProcesso;
            }

            if (codAplicacao && codAplicacao != "")
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
                    setProc("", "", "");
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

        // Chamado quando um produto está para ser inserido no pedido
        function onSaveProd() {
            if (!validate("produto"))
                return false;

            atualizaValMin();

            var codProd = FindControl("txtCodProdIns", "input").value;
            var valor = FindControl("lblValorIns", "span").innerHTML;
            var qtde = FindControl("txtQtdeIns", "input").value;
            var altura = FindControl("txtAlturaIns", "input").value;
            var largura = FindControl("txtLarguraIns", "input").value;
            var valMin = FindControl("hdfValMin", "input").value;

            var tipoPedido = FindControl("hdfTipoPedido", "input").value;
            var pedidoProducao = FindControl("hdfPedidoProducao", "input").value == "true";
            var pedidoMaoObraEspecial = tipoPedido == "<%= CodigoTipoPedidoMaoObraEspecial() %>";

            // Verifica se foi clicado no aplicar na telinha de beneficiamentos
            if (FindControl("tbConfigVidro", "table").style.display == "block")
            {
                alert("Aplique as alterações no beneficiamento antes de salvar o item.");
                return false;
            }

            valMin = new Number(valMin.replace(',', '.'));
            if (codProd == "") {
                alert("Informe o código do produto.");
                return false;
            }
            else if (!pedidoProducao && (valor == "" || parseFloat(valor.replace(",", ".")) == 0)) {
                alert("Informe o valor vendido.");
                return false;
            }
            else if (qtde == "0" || qtde == "") {
                alert("Informe a quantidade.");
                return false;
            }
            else if (new Number(valor.replace(',', '.')) < valMin) {
                alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
                return false;
            }
            else if (FindControl("txtAlturaIns", "input").disabled == false) {
            if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                    alert("Informe a altura.");
                    return false;
                }

                if (FindControl("hdfIsAluminio", "input").value == "true" && altura > 6) {
                    alert("A altura deve ser no máximo 6ml.");
                    return false;
                }
            }
            // Se o textbox da largura estiver habilitado, deverá ser informada
            else if (FindControl("txtLarguraIns", "input").disabled == false && largura == "") {
                alert("Informe a largura.");
                return false;
            }

            if (!obrigarProcApl())
                return false;

            if (!validaTamanhoMax())
                return false;

            // Faz verificações do beneficiamento
            //if (!checkBenef(FindControl("txtEspessura", "input").value))
            //    return false;

            if (FindControl("txtEspessura", "input") != null)
                FindControl("txtEspessura", "input").disabled = false;

            // Calcula o ICMS do produto
            var aliquota = FindControl("hdfAliquotaIcmsProd", "input");
            var icms = FindControl("hdfValorIcmsProd", "input");
            icms.value = aliquota.value > 0 ? parseFloat(valor.replace(",", ".")) * (parseFloat(aliquota.value.replace(",", ".")) / 100) : 0;

            if (icms.value != null && icms.value != "")
                icms.value = icms.value.replace(".", ",");

            FindControl("txtAlturaIns", "input").disabled = false;
            FindControl("txtLarguraIns", "input").disabled = false;

            var nomeControle = getNomeControleBenef();

            if(exibirControleBenef(nomeControle))
            {
                var resultadoVerificacaoObrigatoriedade = verificarObrigatoriedadeBeneficiamentos(dadosProduto.ID);
                return resultadoVerificacaoObrigatoriedade;
            }

            return true;
        }

        // Função chamada quando o produto está para ser atualizado
        function onUpdateProd() {
            if (!validate("produto"))
                return false;

            atualizaValMin();

            var valor = FindControl("lblValorIns", "span").innerHTML;
            var qtde = FindControl("txtQtdeIns", "input").value;
            var altura = FindControl("txtAlturaIns", "input").value;
            var idProd = FindControl("hdfIdProd", "input").value;
            var codInterno = FindControl("hdfCodInterno", "input").value;
            var valMin = FindControl("hdfValMin", "input").value;
            valMin = new Number(valMin.replace(',', '.'));

            var tipoPedido = FindControl("hdfTipoPedido", "input").value;
            var pedidoProducao = FindControl("hdfPedidoProducao", "input").value == "true";
            var pedidoMaoObraEspecial = tipoPedido == "<%= CodigoTipoPedidoMaoObraEspecial() %>";

            // Verifica se foi clicado no aplicar na telinha de beneficiamentos
            if (FindControl("tbConfigVidro", "table").style.display == "block")
            {
                alert("Aplique as alterações no beneficiamento antes de salvar o item.");
                return false;
            }

            if (!pedidoProducao && (valor == "" || parseFloat(valor.replace(",", ".")) == 0)) {
                alert("Informe o valor vendido.");
                return false;
            }
            else if (qtde == "0" || qtde == "") {
                alert("Informe a quantidade.");
                return false;
            }
            else if (new Number(valor.replace(',', '.')) < valMin) {
                alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
                return false;
            }
            else if (FindControl("txtAlturaIns", "input").disabled == false) {
                if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                    alert("Informe a altura.");
                    return false;
                }

                if (FindControl("hdfIsAluminio", "input").value == "true" && altura > 6) {
                    alert("A altura deve ser no máximo 6ml.");
                    return false;
                }
            }

            if (!obrigarProcApl())
                return false;

            if (!validaTamanhoMax())
                return false;

            // Faz verificações do beneficiamento
            //if (!checkBenef(FindControl("txtEspessura", "input").value))
            //    return false;

            if (FindControl("txtEspessura", "input") != null)
                FindControl("txtEspessura", "input").disabled = false;

            // Calcula o ICMS do produto
            var aliquota = FindControl("hdfAliquotaIcmsProd", "input");
            var icms = FindControl("hdfValorIcmsProd", "input");
            icms.value = aliquota.value > 0 ? parseFloat(valor.replace(",", ".")) * (parseFloat(aliquota.value.replace(",", ".")) / 100) : 0;

            if (icms.value != null && icms.value != "")
                icms.value = icms.value.replace(".", ",");

            var nomeControle = getNomeControleBenef();

            if(exibirControleBenef(nomeControle))
            {
                var resultadoVerificacaoObrigatoriedade = verificarObrigatoriedadeBeneficiamentos(dadosProduto.ID);
                return resultadoVerificacaoObrigatoriedade;
            }

            return true;
        }

        function checkCapacidadeProducaoSetor()
        {
            var editPedido = FindControl("grdProdutos", "table") == null;
            var idPedido = '<%= Request["idPedido"] %>';
            var idProcesso = FindControl("hdfIdProcesso", "input").value;

            if (!editPedido)
            {
                if (FindControl("drpFooterVisible", "select") != null)
                    var diferencaM2 = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.'));
                else
                {
                    var totM2Produto = FindControl("hdfTotM", "input") != null ? parseFloat(FindControl("hdfTotM", "input").value.replace(',', '.')) : 0;
                    var novoTotM2Produto = parseFloat(FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.'));
                    var diferencaM2 = novoTotM2Produto - totM2Produto;
                }
            }
            else
                var diferencaM2 = 0;

            if (isNaN(diferencaM2))
                diferencaM2 = 0;

            var resposta = CadPedidoEspelho.VerificarProducaoSetor(idPedido, diferencaM2, idProcesso).value;
            var dadosResposta = resposta.split("|");

            if (dadosResposta[0] == "Erro")
            {
                alert(dadosResposta[1]);
                return false;
            }

            return true;
        }

        // Calcula em tempo real a metragem quadrada do produto
        function calcM2Prod() {
            try {
                var idProd = FindControl("hdfIdProd", "input").value
                FindControl("hdfAlturaCalcIns", "input").value = FindControl("txtAlturaIns", "input").value;
                FindControl("hdfLarguraCalc", "input").value = FindControl("txtLarguraIns", "input").value;
                var altura = FindControl("hdfAlturaCalcIns", "input").value;
                var largura = FindControl("txtLarguraIns", "input").value;
                var qtde = FindControl("txtQtdeIns", "input").value;
                var qtdeAmb = new Number(parseInt(FindControl("hdfQtdeAmbiente", "input").value, 10) > 0 ? FindControl("hdfQtdeAmbiente", "input").value : "1");
                var isVidro = FindControl("hdfIsVidro", "input").value == "true";
                var tipoCalc = FindControl("hdfTipoCalc", "input").value;

                if (altura == "" || largura == "" || qtde == "" || altura == "0" || (tipoCalc != 2 && tipoCalc != 10)) {
                    if (qtde != "" && qtde != "0")
                        calcTotalProd();

                    return false;
                }

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

                var esp = FindControl("txtEspessura", "input") != null ? FindControl("txtEspessura", "input").value : 0;
                var idCliente = FindControl("hdfIdCliente", "input").value;

                var idPedido = '<%= Request["idPedido"] %>';
                var isPedProducaoCorte = CadPedidoEspelho.IsPedidoProducaoCorte(idPedido);
                if(isPedProducaoCorte.error != null){
                    alert(isPedProducaoCorte.error.description);
                    return false;
                }

                FindControl("lblTotM2Ins", "span").innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, isPedProducaoCorte.value).value;
                FindControl("hdfTotM2Calc", "input").value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde * qtdeAmb, idProd, redondo, esp, numBenef, isPedProducaoCorte.value).value;
                FindControl("hdfTotM2CalcSemChapa", "input").value = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, isPedProducaoCorte.value).value;
                FindControl("lblTotM2Calc", "span").innerHTML = FindControl("hdfTotM2Calc", "input").value.replace('.', ',');

                if (FindControl("hdfTotM2Ins", "input") != null)
                    FindControl("hdfTotM2Ins", "input").value = FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.');
                else if (FindControl("hdfTotM", "input") != null)
                    FindControl("hdfTotM", "input").value = FindControl("lblTotM2Ins", "span").innerHTML.replace(',', '.');

                calcTotalProd();
            }
            catch (err) {
                alert(err);
            }
        }

        function GetAdicionalAlturaChapa(){

            var idProd = FindControl("hdfIdProd", "input").value;
            var altura = FindControl("txtAlturaIns", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
            var percDescontoQtde = controleDescQtde.PercDesconto();

            var retorno = MetodosAjax.GetValorTabelaProduto(idProd, tipoEntrega, idCliente, cliRevenda == "True",
                pedidoReposicao, percDescontoQtde, idPedido, "", "", altura);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return;
            }
            else if(retorno == null){
                alert("Erro na recuperação do valor de tabela do produto.");
                return;
            }
            
            var hdfValorIns = FindControl('hdfValorIns', 'input');

            if(hdfValorIns != null) {
                hdfValorIns.value = retorno.value.replace(".", ",");
            } 
            else {
                alert("Não foi possível encontrar o controle 'hdfValorIns'");
                return false;
            }
            
            var valorIns = FindControl('lblValorIns', 'span');

            if(valorIns != null) {
                valorIns.innerHTML = retorno.value.replace(".", ",");
            }
            else{
                alert("Não foi possível encontrar o controle 'lblValorIns'");
                return false;
            }
        }

        // Calcula em tempo real o valor total do produto
        function calcTotalProd() {
            try {
                var valorIns = FindControl("lblValorIns", "span").innerHTML;

                if (valorIns == "")
                    return;

                var totM2 = FindControl("lblTotM2Ins", "span").innerHTML;
                var totM2Calc = new Number(FindControl("hdfTotM2Calc", "input").value.replace(',', '.')).toFixed(2);
                var total = new Number(valorIns.replace(',', '.')).toFixed(2);
                var qtde = new Number(FindControl("txtQtdeIns", "input").value.replace(',', '.'));
                qtde = qtde * new Number(parseInt(FindControl("hdfQtdeAmbiente", "input").value, 10) > 0 ? FindControl("hdfQtdeAmbiente", "input").value : "1");
                var altura = new Number(FindControl("hdfAlturaCalcIns", "input").value.replace(',', '.'));
                var largura = new Number(FindControl("txtLarguraIns", "input").value.replace(',', '.'));
                var tipoCalc = FindControl("hdfTipoCalc", "input").value;
                var m2Minimo = FindControl("hdfM2Minimo", "input").value;
                var alturaBenef = FindControl("drpAltBenef", "select").value;
                var larguraBenef = FindControl("drpLargBenef", "select").value;

                var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
                controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));

                var percDesconto = controleDescQtde.PercDesconto();
                var percDescontoAtual = controleDescQtde.PercDescontoAtual();

                var retorno = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, FindControl("hdfAlturaCalcIns", "input"), largura, true, tipoCalc, alturaBenef, larguraBenef, percDescontoAtual, percDesconto);
                if (retorno != "")
                    FindControl("lblTotalIns", "span").innerHTML = retorno;
            }
            catch (err) {

            }
        }

        // Abre tela para inserir vários alumínios de uma só vez
        function openProdAluminio(idPedido) {
            var idProd = FindControl("txtCodProdIns", "input").value;
            var valor = FindControl("hdfValorIns", "input").value;

            if (idPedido == "" || idProd == "")
                return false;

            openWindow(400, 500, '../Utils/SetProdAluminio.aspx?IdPedido=' + idPedido + '&idProd=' + idProd + '&val=' + valor + '&idAmbiente=' + FindControl('hdfIdAmbiente', 'input').value);
        }

        function openProjeto(idAmbientePedidoEspelho) {

            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;

            openWindow(screen.height, screen.width, '../Cadastros/Projeto/CadProjetoAvulso.aspx?IdPedidoEspelho=<%= Request["IdPedido"] %>' +
                "&IdAmbientePedidoEspelho=" + idAmbientePedidoEspelho + "&idCliente=" + idCliente + "&TipoEntrega=" + tipoEntrega + "&pcp=1");

            return false;
        }

        function buscarProcessos(){
            var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProd", "input").value);
            openWindow(450, 700, "../Utils/SelEtiquetaProcesso.aspx?idSubgrupo=" + idSubgrupo.value);
        }

        function recarregar() {
            atualizarPagina();
        }

        function refreshPage() {
            atualizarPagina();
        }

        function exibirObs(num, botao) {
            for (iTip = 0; iTip < 2; iTip++) {
                TagToTip('tbObsCalc_' + num, FADEIN, 300, COPYCONTENT, false, TITLE, 'Observação', CLOSEBTN, true,
                    CLOSEBTNTEXT, 'Fechar (Não salva as alterações)', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, false,
                    FIX, [botao, 9 - getTableWidth('tbObsCalc_' + num), 7]);
            }
        }

        function exibirInfoAdicProd(num, botao) {
            for (iTip = 0; iTip < 2; iTip++) {
                TagToTip('tbInfoAdicProd_' + num, FADEIN, 300, COPYCONTENT, false, TITLE, 'Informações Adicionais', CLOSEBTN, true,
                    CLOSEBTNTEXT, 'Fechar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, false,
                    FIX, [botao, 9 - getTableWidth('tbInfoAdicProd_' + num), 7]);
            }
        }

        function setCalcObs(idItemProjeto, button) {
            var obs = button.parentNode.parentNode.parentNode.getElementsByTagName('textarea')[0].value;

            var retorno = CadPedidoEspelho.SalvaObsProdutoPedido(idItemProjeto, obs).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }
            else {
                alert("Observação salva.");
                window.opener.refreshPage();
            }
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvPedido" runat="server" AutoGenerateRows="False" DataSourceID="odsPedido"
                    GridLines="None" DataKeyNames="IdPedido" OnItemCommand="dtvPedido_ItemCommand">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <table cellpadding="2" cellspacing="2" align="center">
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Num. Pedido
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblNumPedido" runat="server" Text='<%# Eval("IdPedido") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Cliente
                                        </td>
                                        <td align="left" nowrap="nowrap" colspan="3">
                                            <asp:Label ID="lblNomeCliente" runat="server" Text='<%# Eval("NomeCli") %>'></asp:Label>
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
                                            <asp:Label ID="lblValorEnt" runat="server" Text='<%# Eval("ValorEntrada") %>'></asp:Label>
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
                                            <asp:Label ID="lblDataEntrega" runat="server" Text='<%# Eval("DataEntrega", "{0:d}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Situação
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Data Espelho
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataEspelho", "{0:d}") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            <asp:Label ID="lblDescrValorFrete" runat="server" Text='Valor do Frete' OnLoad="LblValorFrete_Load"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblValorFrete" runat="server" Text='<%# Eval("ValorEntrega", "{0:C}") %>' OnLoad="LblValorFrete_Load"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Total Pedido
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblTotalPed" runat="server" Text='<%# Eval("TotalPedido", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Total Espelho
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblTotalEsp" runat="server" Text='<%# Eval("Total", "{0:C}") %>' ForeColor="Blue"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            Diferença
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label11" runat="server" Text='<%# String.Format("{0:C}", Math.Abs((decimal)Eval("Diferenca"))) %>'
                                                ForeColor='<%# Single.Parse(Eval("Diferenca").ToString()) < 0 ? System.Drawing.Color.Green : System.Drawing.Color.Red %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style="<%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido && !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIpiPedido ? "display: none": "" %>">
                                        <td align="left" nowrap="nowrap" runat="server" id="tituloIcms" onload="Icms_Load">
                                            <asp:Label ID="lblTituloIcms" runat="server" Text="Valor ICMS" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" runat="server" id="valorIcms" onload="Icms_Load">
                                            <asp:Label ID="lblValorIcms" runat="server" Text='<%# Eval("ValorIcms", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" runat="server" id="tituloIpi" onload="Ipi_Load">
                                            <asp:Label ID="lblTituloIpi" runat="server" Text="Valor IPI" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" runat="server" id="valorIpi" onload="Ipi_Load">
                                            <asp:Label ID="lblValorIpi" runat="server" Text='<%# Eval("ValorIpi", "{0:C}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr runat="server" visible='<%# Glass.Configuracoes.RentabilidadeConfig.ExibirRentabilidadePedidoEspelho %>'>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            <asp:Label runat="server" Text="Rentabilidade"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label runat="server" Text='<%# Eval("PercentualRentabilidade", "{0:#0.00}") + "%" %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap" style="font-weight: bold">
                                            <asp:Label runat="server" Text="Rent. Financeira"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label runat="server" Text='<%# Eval("RentabilidadeFinanceira", "{0:C}") %>'></asp:Label>
                                            <a href="#" onclick='openWindow(500, 700, "../Relatorios/Rentabilidade/VisualizacaoItemRentabilidade.aspx?tipo=pedidoespelho&id=<%# Eval("IdPedido") %>"); return false;'>
                                            <img border="0" src="../Images/cash_red.png" title="Rentabilidade" /></a>
                                        </td>
                                        <td align="left" nowrap="nowrap" colspan="4"></td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                <asp:HiddenField ID="hdfCliRevenda" runat="server" Value='<%# Eval("CliRevenda") %>' />
                                <asp:HiddenField ID="hdfTipoVenda" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Eval("Total") %>' />
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCli") %>' />
                                <asp:HiddenField ID="hdfIsReposicao" runat="server" Value='<%# IsReposicao(Eval("TipoVenda")) %>' />
                                <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            Data de entrega da fábrica
                                        </td>
                                        <td>
                                            <uc3:ctrlData ID="ctrlDataFabrica" runat="server" ReadOnly="ReadWrite" DataString='<%# Eval("DataFabrica", "{0:d}") %>'
                                                ExibirHoras="False" />
                                        </td>
                                    </tr>
                                </table>
                                <table>
                                    <tr>
                                        <td>
                                            Obs.
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtObs" runat="server" MaxLength="500" Rows="3" Text='<%# Eval("Obs") %>'
                                                TextMode="MultiLine" Width="400px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdComissionado" runat="server" Value='<%# Bind("IdComissionado") %>' />
                                <asp:HiddenField ID="hdfPercComissao" runat="server" Value='<%# Eval("PercComissao") %>' />
                                <asp:HiddenField ID="hdfValorComissao" runat="server" Value='<%# Bind("ValorComissao") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" Text="Atualizar/Recalcular" CommandName="Atualizar" />
                                <asp:Button ID="btnFinalizar" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                    Text="Finalizar" OnClientClick="if (!finalizar()) return false" OnClick="btnFinalizar_Click" />
                                <asp:Button ID="btnExcedente" runat="server" OnClick="btnExcedente_Click" OnClientClick="return confirm('Tem certeza que deseja gerar uma conta a receber com o valor excedente da Conferência?');"
                                    Text="Gerar Valor Excedente" OnLoad="btnExcedente_Load" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Fields>
                </asp:DetailsView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkProjeto" runat="server" OnClientClick="return openProjeto('', false);">Incluir Projeto</asp:LinkButton>
            </td>
        </tr>
        <tr runat="server" id="inserirMaoObra" visible="false">
            <td align="center">
                <asp:LinkButton ID="lbkInserirMaoObra" runat="server">Inserir várias peças de vidro com a mesma mão de obra</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdAmbiente" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataKeyNames="IdAmbientePedido"
                    DataSourceID="odsAmbiente" OnRowCommand="grdAmbiente_RowCommand" ShowFooter="True"
                    OnPreRender="grdAmbiente_PreRender" OnRowDeleted="grdAmbiente_RowDeleted">
                    <Columns>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="lnkAddAmbiente" runat="server" OnClientClick="exibirEsconderAmbiente(true); return false;"
                                    ImageUrl="~/Images/Insert.gif" CausesValidation="False" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit0" runat="server" CommandName="Edit" CausesValidation="False"
                                    Visible='<%# Eval("EditDeleteVisible") %>'>
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Excluir este ambiente fará com que todos os produtos do mesmo sejam excluídos também, confirma exclusão?&quot;)"
                                    CausesValidation="False" Visible='<%# Eval("EditDeleteVisible") %>' />
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
                                <asp:TextBox ID="txtEditAmbiente" runat="server" Text='<%# Eval("Ambiente") %>' MaxLength="50"
                                    Width="150px" OnLoad="txtAmbiente_Load" onchange="FindControl('hdfDescrAmbiente', 'input').value = this.value"></asp:TextBox>
                                <div runat="server" id="EditAmbMaoObra" onload="ambMaoObra_Load">
                                    <asp:TextBox ID="txtCodAmb" runat="server" onblur="produtoAmbiente=true; loadProduto(this.value);"
                                        onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                        Text='<%# Eval("CodInterno") %>' Width="50px"></asp:TextBox>
                                    <asp:Label ID="lblDescrAmb" runat="server" Text='<%# Eval("Ambiente") %>'></asp:Label>
                                    <a href="#" onclick="produtoAmbiente=true; getProduto(); return false;">
                                        <img border="0" src="../Images/Pesquisar.gif" /></a>
                                    <asp:HiddenField ID="hdfAmbIdProd" Value='<%# Bind("IdProd") %>' runat="server" />
                                    <asp:HiddenField ID="hdfIdItemProjeto" Value='<%# Bind("IdItemProjeto") %>' runat="server" />
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
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("ProjetoVisible") %>'>
                                    <asp:PlaceHolder ID="pchEditProjeto" runat="server" Visible='<%# Eval("EditDeleteVisible") %>'>
                                        <a href="#" onclick='return openProjeto(<%# Eval("IdAmbientePedido") %>)'>
                                            <%# Eval("Ambiente") %></a> </asp:PlaceHolder>
                                    <asp:Label ID="lblAmbiente" runat="server" Text='<%# Eval("Ambiente") %>' Visible='<%# !(bool)Eval("EditDeleteVisible") %>'></asp:Label>
                                </asp:PlaceHolder>
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
                                <asp:Label ID="Label13" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                <asp:Label ID="Label17" runat="server" ForeColor="Red" Text='<%# Eval("DescrObsProj") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde" Visible="False">
                            <ItemTemplate>
                                <asp:Label ID="Label14" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEditQtdeAmbiente" runat="server" Text='<%# Bind("Qtde") %>' onkeypress="return soNumeros(event, true, true)"
                                    Width="50px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQtde" runat="server" ControlToValidate="txtEditQtdeAmbiente"
                                    Display="Dynamic" ErrorMessage="*" ValidationGroup="ambiente"></asp:RequiredFieldValidator>
                                <asp:HiddenField ID="hdfQtdeImpresso" runat="server" Value='<%# Bind("QtdeImpresso") %>' />
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
                                <asp:Label ID="Label15" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
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
                                <asp:Label ID="Label16" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
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
                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso" Visible="False">
                            <EditItemTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAmbProcIns" runat="server" onblur="procAmbiente=true; loadProc(this.value);"
                                                onkeydown="if (isEnter(event)) { procAmbiente=true; loadProc(this.value); }"
                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="procAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfAmbIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
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
                                            <a href="#" onclick="procAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfAmbIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao" Visible="False">
                            <EditItemTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAmbAplIns" runat="server" onblur="aplAmbiente=true; loadApl(this.value);"
                                                onkeydown="if (isEnter(event)) { aplAmbiente=true; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="aplAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfAmbIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAmbAplIns" runat="server" onblur="aplAmbiente=true; loadApl(this.value);"
                                                onkeydown="if (isEnter(event)) { aplAmbiente=true; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="aplAmbiente=true; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfAmbIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodAplicacao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Redondo" SortExpression="Redondo" Visible="False">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("Redondo") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkRedondoAmbiente" runat="server" Checked="False" />
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
                        <asp:TemplateField HeaderText="Acréscimo" SortExpression="Acrescimo">
                            <EditItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Desconto" SortExpression="Desconto">
                            <EditItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                <asp:HiddenField ID="hdfValorDescontoAtual" runat="server" Value='<%# Eval("ValorDescontoAtual") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
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
                <br />
                <asp:Label ID="lblAmbiente" runat="server" CssClass="subtitle1" Font-Bold="False"></asp:Label>
                <asp:HiddenField ID="hdfAlturaAmbiente" runat="server" />
                <asp:HiddenField ID="hdfLarguraAmbiente" runat="server" />
                <asp:HiddenField ID="hdfQtdeAmbiente" runat="server" />
                <asp:HiddenField ID="hdfRedondoAmbiente" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAmbiente" runat="server" SelectMethod="GetList" TypeName="Glass.Data.DAL.AmbientePedidoEspelhoDAO"
                    OnDeleted="odsAmbiente_Deleted" OnUpdated="odsAmbiente_Updated" DataObjectTypeName="Glass.Data.Model.AmbientePedidoEspelho"
                    DeleteMethod="Delete" UpdateMethod="Update" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    EnablePaging="True" OnUpdating="odsAmbiente_Updating" OnInserting="odsAmbiente_Inserting" OnDeleting="odsAmbiente_Deleting">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdAmbiente" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXPed" DataKeyNames="IdProdPed"
                    OnRowDeleted="grdProdutos_RowDeleted" ShowFooter="True" OnRowCommand="grdProdutos_RowCommand"
                    OnPreRender="grdProdutos_PreRender" CssClass="gridStyle" OnRowUpdated="grdProdutos_RowUpdated">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Visible='<%# Eval("EditDeleteVisible") %>'>
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" Visible='<%# Eval("EditDeleteVisible") %>'
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" />
                                <asp:ImageButton ID="imbDesmembrar" runat="server" ImageUrl="../Images/Split.png"
                                    OnClientClick='<%# "desmembrar(" + Eval("IdProdPed") + ", " + Eval("Qtde") + "); return false;" %>'
                                    ToolTip="Dividir produtos" Visible='<%# Eval("DesmembrarVisible") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onUpdateProd();" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                <asp:HiddenField ID="hdfIdMaterItemProj" runat="server" Value='<%# Bind("IdMaterItemProj") %>' />
                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Bind("IdPedido") %>' />
                                <asp:HiddenField ID="hdfIdAmbiente" runat="server" Value='<%# Bind("IdAmbientePedido") %>' />
                                <asp:HiddenField ID="hdfValorAcrescimo" runat="server" Value='<%# Bind("ValorAcrescimo") %>' />
                                <asp:HiddenField ID="hdfValorDesconto" runat="server" Value='<%# Bind("ValorDesconto") %>' />
                                <asp:HiddenField ID="hdfValorAcrescimoProd" runat="server" Value='<%# Bind("ValorAcrescimoProd") %>' />
                                <asp:HiddenField ID="hdfValorDescontoProd" runat="server" Value='<%# Bind("ValorDescontoProd") %>' />
                                <asp:HiddenField ID="hdfPercComissao" runat="server" Value='<%# Bind("PercComissao") %>' />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                            <EditItemTemplate>
                                <asp:Label ID="lblCodProdIns" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                <asp:HiddenField ID="hdfCodInterno" runat="server" Value='<%# Eval("CodInterno") %>' />
                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                <asp:HiddenField ID="hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                <asp:HiddenField ID="hdfIsAluminio" runat="server" Value='<%# Eval("IsAluminio") %>' />
                                <asp:HiddenField ID="hdfM2Minimo" runat="server" Value='<%# Eval("M2Minimo") %>' />
                                <asp:HiddenField ID="hdfAliquotaIcmsProd" runat="server" Value='<%# Bind("AliqIcms") %>' />
                                <asp:HiddenField ID="hdfValorIcmsProd" runat="server" Value='<%# Bind("ValorIcms") %>' />
                                <asp:HiddenField ID="hdfCustoProd" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodProdIns" runat="server" Width="50px" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                    onkeypress="return !(isEnter(event));" onblur="loadProduto(this.value);"></asp:TextBox>
                                <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
                                <a href="#" onclick="getProduto(); return false;">
                                    <img src="../Images/Pesquisar.gif" border="0" /></a> <a id="lnkAddAluminio" href="#"
                                        onclick="openProdAluminio('<%= Request["IdPedido"] %>'); return false;" style="visibility: hidden;"
                                        title="Adicionar Vários">
                                        <img src="../Images/addMany.gif" border="0" /></a>
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
                                <asp:TextBox ID="txtQtdeIns" runat="server" onblur="calcM2Prod(); return verificaEstoque();"
                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                    Text='<%# Bind("Qtde") %>' Width="50px"></asp:TextBox>
                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                <uc2:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" Callback="calcTotalProd"
                                    CallbackValorUnit="calcTotalProd" PercDescontoQtde='<%# Bind("PercDescontoQtde") %>'
                                    ValorDescontoQtde='<%# Bind("ValorDescontoQtde") %>' ValidationGroup="produto" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtQtdeIns" runat="server" onkeydown="if (isEnter(event)) calcM2Prod();"
                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                    onblur="calcM2Prod(); return verificaEstoque();" Width="50px"></asp:TextBox>
                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                <uc2:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" CallbackValorUnit="calcTotalProd"
                                    Callback="calcTotalProd" ValidationGroup="produto" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLarguraIns" runat="server" onblur="calcM2Prod();" Enabled='<%# Eval("LarguraEnabled") %>'
                                    onkeypress="return soNumeros(event, true, true);" Text='<%# Bind("LarguraReal") %>'
                                    Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfLarguraCalc" runat="server" Value='<%# Bind("Largura") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLarguraIns" runat="server" onkeypress="return soNumeros(event, true, true);"
                                    onblur="calcM2Prod();" Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfLarguraCalc" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAlturaIns" runat="server" onblur="GetAdicionalAlturaChapa(); calcM2Prod();" Text='<%# Bind("AlturaReal") %>'
                                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                    Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfAlturaCalcIns" runat="server" Value='<%# Bind("Altura") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAlturaIns" runat="server" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                    onblur="GetAdicionalAlturaChapa(); calcM2Prod();" Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfAlturaCalcIns" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total M2" SortExpression="TotM">
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
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total M2 calc." SortExpression="TotM2Calc">
                            <EditItemTemplate>
                                <asp:Label ID="lblTotM2Calc" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                                <asp:HiddenField ID="hdfTotM2Calc" runat="server" Value='<%# Eval("TotM2Calc") %>' />
                                <asp:HiddenField ID="hdfTotM2CalcSemChapa" runat="server" Value='<%# Eval("TotM2CalcSemChapaString") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotM2CalcIns" runat="server"></asp:Label>
                                <asp:HiddenField ID="hdfTotM2Ins" runat="server" />
                                <asp:HiddenField ID="hdfTotM2CalcIns" runat="server" />
                                <asp:HiddenField ID="hdfTotM2CalcSemChapaIns" runat="server" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LabelM2Calc" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("ValorVendido", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblValorIns" runat="server" Text='<%# Eval("ValorVendido") %>'></asp:Label>
                                <asp:HiddenField ID="hdfValorIns" runat="server" Value='<%# Bind("ValorVendido") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblValorIns" runat="server"></asp:Label>
                                <asp:HiddenField ID="hdfValorIns" runat="server" Value='<%# Bind("ValorVendido") %>' />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                            <EditItemTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="aplAmbiente=false; loadApl(this.value);"
                                                onkeydown="if (isEnter(event)) { aplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
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
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="aplAmbiente=false; loadApl(this.value);"
                                                onkeydown="if (isEnter(event)) { aplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                Width="30px"></asp:TextBox>
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
                        <asp:TemplateField HeaderText="Proc." SortExpression="CodProcesso">
                            <EditItemTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="procAmbiente=false; loadProc(this.value);"
                                                onkeydown="if (isEnter(event)) { procAmbiente=false; loadProc(this.value); }"
                                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="procAmbiente=false; buscarProcessos(); return false;">
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
                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="procAmbiente=false; loadProc(this.value);"
                                                onkeydown="if (isEnter(event)) { procAmbiente=false; loadProc(this.value); }"
                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="procAmbiente=false; buscarProcessos(); return false;">
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
                                <asp:TextBox ID="txtPedCliIns" runat="server" MaxLength="50" Text='<%# Bind("PedCli") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtPedCliIns" runat="server" MaxLength="50" Width="50px"></asp:TextBox>
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
                                <asp:Label ID="lblTotalIns" runat="server" Text='<%# Bind("Total") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotalIns" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Benef." SortExpression="ValorBenef">
                            <ItemTemplate>
                                <asp:Label ID="Label11" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblValorBenef" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblValorBenef" runat="server"></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
<%--                        <asp:TemplateField HeaderText="Obs." SortExpression="ObsGrid">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtObsIns" runat="server" MaxLength="100" Text='<%# Bind("ObsGrid") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtObsIns" runat="server" MaxLength="100"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label12" runat="server" Text='<%# Bind("ObsGrid") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbImagemPeca" runat="server" ImageUrl="~/Images/imagem.gif"
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=pcp&idPedido=" + Eval("IdPedido") +"&idProdPed=" +  Eval("IdProdPed") + "&pecaAvulsa=True" + "\"); return false" %>'
                                    ToolTip="Exibir imagem da peça"  Visible='<%# (Eval("IsVidro").ToString() == "true")%>'/>
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
                                <a href="#" id="lnkInfoAdicProd" onclick="exibirInfoAdicProd(<%# Eval("IdProdPed") %>, this); return false;">
                                    <img border="0" src="../../Images/tax.png" title="Informações Adicionais" width="16px"/></a>
                                <table id='tbInfoAdicProd_<%# Eval("IdProdPed") %>' cellspacing="0" style="display: none;">
                                     <tr>
                                        <td align="left" style="font-weight: bold">Natureza de Operação
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label ID="lblNatOp" runat="server" Text='<%# Eval("CodNaturezaOperacao") %>'></asp:label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="font-weight: bold">Aliq. IPI
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("AliqIpi") %>'></asp:label>
                                        </td>
                                        <td align="left" style="font-weight: bold">Valor IPI
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("ValorIpi") %>'></asp:label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="font-weight: bold">Aliq. ICMS
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("AliqIcms") %>'></asp:label>
                                        </td>
                                        <td align="left" style="font-weight: bold">Bc. ICMS
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("BcIcms") %>'></asp:label>
                                        </td>
                                        <td align="left" style="font-weight: bold">Valor ICMS
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("ValorIcms") %>'></asp:label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="font-weight: bold">Aliq. ICMS ST
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("AliqIcmsSt") %>'></asp:label>
                                        </td>
                                            <td align="left" style="font-weight: bold">Bc. ICMS ST
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("BcIcmsSt") %>'></asp:label>
                                        </td>
                                        <td align="left" style="font-weight: bold">Valor ICMS ST
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("ValorIcmsSt") %>'></asp:label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="font-weight: bold">Aliq. COFINS
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("AliqCofins") %>'></asp:label>
                                        </td>
                                            <td align="left" style="font-weight: bold">Bc. COFINS
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("BcCofins") %>'></asp:label>
                                        </td>
                                        <td align="left" style="font-weight: bold">Valor COFINS
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("ValorCofins") %>'></asp:label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="font-weight: bold">Aliq. PIS
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("AliqPis") %>'></asp:label>
                                        </td>
                                            <td align="left" style="font-weight: bold">Bc. PIS
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("BcPis") %>'></asp:label>
                                        </td>
                                        <td align="left" style="font-weight: bold">Valor PIS
                                        </td>
                                        <td align="left" style="padding-left: 2px">
                                            <asp:label runat="server" Text='<%# Eval("ValorPis") %>'></asp:label>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                            <EditItemTemplate></EditItemTemplate>
                            <FooterTemplate></FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <div id="benefMaoObra" style='<%# !IsPedidoMaoDeObra() ? "display: none;": "" %> white-space: nowrap'>
                                    <asp:DropDownList ID="drpAltBenef" runat="server" onchange="calcTotalProd()" SelectedValue='<%# Bind("AlturaBenef") %>'>
                                        <asp:ListItem></asp:ListItem>
                                        <asp:ListItem>0</asp:ListItem>
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:DropDownList ID="drpLargBenef" runat="server" onchange="calcTotalProd()" SelectedValue='<%# Bind("LarguraBenef") %>'>
                                        <asp:ListItem></asp:ListItem>
                                        <asp:ListItem>0</asp:ListItem>
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                    </asp:DropDownList>
                                    Esp.:
                                    <asp:TextBox ID="txtEspBenef" runat="server" onkeypress="return soNumeros(event, true, true)"
                                        Text='<%# Bind("EspessuraBenef") %>' Width="30px"></asp:TextBox>
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
                                            <uc1:ctrlBenef ID="ctrlBenefEditar" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>'
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
                                    <asp:DropDownList ID="drpAltBenef" runat="server" onchange="calcTotalProd()">
                                        <asp:ListItem>0</asp:ListItem>
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:DropDownList ID="drpLargBenef" runat="server" onchange="calcTotalProd()">
                                        <asp:ListItem>0</asp:ListItem>
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                    </asp:DropDownList>
                                    Esp.:
                                    <asp:TextBox ID="txtEspBenef" runat="server" onkeypress="return soNumeros(event, true, true)"
                                        Width="30px"></asp:TextBox>
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
                                                    <td class="dtvFieldBold">
                                                        Ped. Cli
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPedCli" runat="server" Width="50px" MaxLength="50"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc1:ctrlBenef ID="ctrlBenefInserir" runat="server" OnLoad="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotal"
                                                ValidationGroup="produto" />
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
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInsProd" runat="server" CommandName="Insert" OnClick="lnkInsProd_Click"
                                    OnClientClick="return onSaveProd();">
                                <img border="0" src="../Images/ok.gif" /></asp:LinkButton>
                                </td> </tr>
                                <tr style='<%= !IsPedidoMaoDeObra() ? "display: none": "" %>'>
                                    <td colspan="14" style="text-align: right">
                                        <span style="position: relative; top: -6px">campos usados para definir altura, largura
                                            e
                                            <br />
                                            espessura da lapidação e bisotê </span>
                                    </td>
                                </tr>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfTipoPedido" runat="server" />
    <asp:HiddenField ID="hdfPedidoMaoDeObra" runat="server" />
    <asp:HiddenField ID="hdfPedidoProducao" runat="server" />
    <asp:HiddenField ID="hdfBloquearMaoDeObra" runat="server" />
    <asp:HiddenField ID="hdfIdPedido" runat="server" />
    <asp:HiddenField ID="hdfIdProd" runat="server" />
    <asp:HiddenField ID="hdfCurrPage" runat="server" Value="0" />
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" SelectMethod="GetElement" TypeName="Glass.Data.DAL.PedidoEspelhoDAO"
        DataObjectTypeName="Glass.Data.Model.PedidoEspelho" UpdateMethod="UpdateDados" OnUpdating="odsPedido_Updating" >
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdXPed" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedidoEspelho"
        DeleteMethod="DeleteComTransacao" EnablePaging="True" MaximumRowsParameterName="pageSize"
        OnDeleted="odsProdXPed_Deleted" SelectCountMethod="GetCount" SelectMethod="GetList"
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoEspelhoDAO"
        UpdateMethod="UpdateComTransacao" OnUpdated="odsProdXPed_Updated" InsertMethod="InsertComTransacao"
        OnInserting="odsProdXPed_Inserting" OnUpdating="odsProdXPed_Updating" OnDeleting="odsProdXPed_Deleting" >
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
            <asp:ControlParameter ControlID="hdfIdAmbiente" Name="idAmbientePedido" PropertyName="Value"
                Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>

    <script type="text/javascript">
        if (FindControl("imbAtualizar", "input") != null && FindControl("lblCodProdIns", "span") != null)
            loadProduto(FindControl("lblCodProdIns", "span").innerHTML);

        // Esconde controles de inserção de ambiente
        if (FindControl("lnkAddAmbiente", "input") != null)
            exibirEsconderAmbiente(false);
    </script>

</asp:Content>
