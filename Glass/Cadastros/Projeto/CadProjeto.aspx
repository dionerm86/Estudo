﻿<%@ Page Title="Cadastro de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadProjeto" Strict="true" Explicit="true" EnableEventValidation="false" %>

<%@ Register Src="../../Controls/ctrlLinkQueryString.ascx" TagName="ctrllinkquerystring"
    TagPrefix="uc2" %>
<%@ Register Src="../../Controls/ctrlTextBoxFloat.ascx" TagName="ctrltextboxfloat"
    TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register src="../../Controls/ctrlCorItemProjeto.ascx" tagname="ctrlCorItemProjeto" tagprefix="uc3" %>
<%@ Register src="../../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Grid.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        var idProjeto = <%= Request["idProjeto"] != null ? Request["idProjeto"] : "0" %>;

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

        function exibirObs(num, botao)
        {
            for (iTip = 0; iTip < 2; iTip++)
            {
                TagToTip('tbObsCalc_' + num, FADEIN, 300, COPYCONTENT, false, TITLE, 'Observação', CLOSEBTN, true,
                    CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, false,
                    FIX, [botao, 9 - getTableWidth('tbObsCalc_' + num), 7]);
            }
        }

        function exibirBenef(botao)
        {
            for (iTip = 0; iTip < 2; iTip++)
            {
                TagToTip('tbConfigVidro', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true,
                    CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                    FIX, [botao, 9 - getTableWidth('tbConfigVidro'), -41 - getTableHeight('tbConfigVidro')])
            }
        }

        function setValorTotal(valor, custo) {
            if (exibirControleBenef(getNomeControleBenef())) {
                var lblValorBenef = FindControl("lblValorBenef", "span");
                lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
            }
        }

        function duplicar()
        {
            if (!confirm("Deseja duplicar o projeto atual?"))
                return;

            setModelo(FindControl("hdfDuplicarCodigo", "input").value, FindControl("hdfDuplicarEspessura", "input").value,
                FindControl("hdfDuplicarCorVidro", "input").value, FindControl("hdfDuplicarCorAluminio", "input").value,
                FindControl("hdfDuplicarCorFerragem", "input").value, FindControl("hdfDuplicarApenasVidros", "input").value,
                FindControl("hdfDuplicarMedidaExata", "input").value);
        }

        // Insere novo modelo na tela
        function setModelo(idProjetoModelo, espessuraVidro, idCorVidro, idCorAluminio, idCorFerragem, apenasVidros, medidaExata) {
            var idProjeto = FindControl("hdfIdProjeto", "input").value;

            var retorno = CadProjeto.NovoItemProjeto(idProjeto, idProjetoModelo, espessuraVidro, idCorVidro, idCorAluminio,
                idCorFerragem, apenasVidros, medidaExata).value;

            if (retorno == null) {
                alert("Falha na requisição AJAX.");
                return false;
            }

            retorno = retorno.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }
            else if (retorno[0] == "ok") {
                FindControl("hdfIdItemProjeto", "input").value = retorno[1];

                // Limpa a tabela de medidas da instalação e de peças para não trazerem os mesmos valores
                var tbMedInst = FindControl("tbMedInst", "table");
                var tbPecaModelo = FindControl("tbPecaModelo", "table");
                if (tbMedInst != null) tbMedInst.innerHTML = "";
                if (tbPecaModelo != null) tbPecaModelo.innerHTML = "";

                // Faz um submit no form, para recarregar a página e montar o modelo escolhido
                getForm().submit();
            }
        }

        // Verificaçãoes ao inserir projeto
        function onInsert() {
            // Verifica se o cliente foi selecionado
            if ((FindControl("hdfCliente", "input").value == "" || FindControl("hdfCliente", "input").value == null) &&
                FindControl("txtNomeCliente", "input").value == "") {
                alert("Informe o cliente.");
                return false;
            }

            // Se for versão WebGlass Projetos, informa o primeiro valor no tipo de entrega
            if (FindControl("ddlTipoEntrega", "select").style.display == "none") {
                FindControl("ddlTipoEntrega", "select").selectedIndex = 1;
            }

            // Verifica se o tipo de entrega foi selecionado
            if (FindControl("ddlTipoEntrega", "select").value == "") {
                alert("Selecione o tipo de entrega.");
                return false;
            }

            // Se o idOrcamento tiver sido informado, verifica se o mesmo existe e está em aberto
            if (FindControl("txtIdOrcamento", "input").value != "") {
                if (CadProjeto.ExistsOrcamentoEmAberto(FindControl("txtIdOrcamento", "input").value).value == "false") {
                    alert("O orçamento informado não existe ou não está em aberto.");
                    return false;
                }
            }
        }

        // Verificaçãoes ao atualizar projeto
        function onUpdate() {
            // Verifica se o cliente foi selecionado
            if ((FindControl("hdfCliente", "input").value == "" || FindControl("hdfCliente", "input").value == null) &&
                FindControl("txtNomeCliente", "input").value == "") {
                alert("Informe o cliente.");
                return false;
            }

            // Se o idOrcamento tiver sido informado, verifica se o mesmo existe e está em aberto
            if (FindControl("txtIdOrcamento", "input").value != "") {
                if (CadProjeto.ExistsOrcamentoEmAberto(FindControl("txtIdOrcamento", "input").value).value == "false") {
                    alert("O orçamento informado não existe ou não está em aberto.");
                    return false;
                }
            }
        }

        function getCli(idCliente) {
            var idCli = idCliente.value != undefined && idCliente.value != null ? idCliente.value : idCliente;
            var retorno = CadProjeto.GetCli(idCli).value.split(';');
            var isAtualRevenda = FindControl("hdfCliRevenda", "input");
            isAtualRevenda = isAtualRevenda != null ? isAtualRevenda.value.toLowerCase() : null;

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idCli.value = "";
                FindControl("txtNomeCliente", "input").value = "";
                FindControl("hdfCliente", "input").value = "";
                return false;
            }
            else if (isAtualRevenda != null && retorno[2].toLowerCase() != isAtualRevenda)
            {
                var atualRevenda = isAtualRevenda == "true" ? "" : "não ";
                var novoRevenda = isAtualRevenda == "true" ? "não " : "";

                alert("O cliente atual " + atualRevenda + "é revenda e o cliente buscado " + novoRevenda +
                    "é. Essa alteração não é possível. Para alterar, selecione um novo cliente que " + atualRevenda + "seja revenda.");

                idCliente.value = FindControl("hdfCliente", "input").value;
                return false;
            }

            FindControl("txtNomeCliente", "input").value = retorno[1];
            FindControl("hdfCliente", "input").value = idCli;
            FindControl("txtNumCli", "input").value = idCli;
            if (isAtualRevenda != null)
                FindControl("hdfCliRevenda", "input").value = retorno[2];
        }

        var hdfVidro;
        var txtVidro;

        function setVidro(codInterno) {
            loadVidro(codInterno, hdfVidro, txtVidro);
        }

        function loadVidro(codInterno, hdf, txt) {
            if (codInterno == "")
                return false;

            var idProjeto = <%= Request["idProjeto"] != null ? Request["idProjeto"] : "0" %>;

            var retorno = CadProjeto.GetVidro(idProjeto, codInterno).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                hdf.value = "";
                txt.value = "";
                return false;
            }
            else if (retorno[0] == "Prod") {
                hdf.value = retorno[1];
                txt.value = retorno[2];
            }

            FindControl("hdfPecasAlteradas", "input").value = "true";
        }

        var insKit = false;
        var insTubo = false;

        // Função chamada após selecionar produto pelo popup
        function setProduto(codInterno) {
            try {
                if (insKit)
                    FindControl("txtCodKit", "input").value = codInterno;
                else if (insTubo)
                    FindControl("txtCodTubo", "input").value = codInterno;
                else
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
                var idProjeto = <%= Request["idProjeto"] != null ? Request["idProjeto"] : "0" %>;

                var retorno = CadProjeto.GetProduto(idProjeto, codInterno, FindControl("hdfTipoEntrega", "input").value, FindControl("hdfCliRevenda", "input").value, FindControl("hdfIdCliente", "input").value).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);

                    if (insKit)
                        FindControl("txtCodKit", "input").value = "";
                    else if (insTubo)
                        FindControl("txtCodTubo", "input").value = "";
                    else
                        FindControl("txtCodProd", "input").value = "";

                    insKit = false;
                    insTubo = false;

                    return false;
                }
                else if (insKit) {
                    FindControl("hdfIdKit", "input").value = retorno[1];
                    FindControl("lblDescrKit", "span").innerHTML = retorno[2];
                    FindControl("txtValorKit", "input").value = retorno[3];
                    FindControl("hdfKitValMin", "input").value = retorno[3]; // Armazena o valor mínimo
                }
                else if (insTubo) {
                    FindControl("hdfIdTubo", "input").value = retorno[1];
                    FindControl("lblDescrTubo", "span").innerHTML = retorno[2];
                    FindControl("txtValorTubo", "input").value = retorno[3];
                    FindControl("hdfTuboValMin", "input").value = retorno[3]; // Armazena o valor mínimo
                }
                else if (retorno[0] == "Prod") {
                    FindControl("hdfIdProdMater", "input").value = retorno[1];
                    FindControl("txtValorIns", "input").value = retorno[3]; // Exibe no cadastro o valor mínimo do produto
                    FindControl("hdfValMin", "input").value = retorno[3]; // Armazena o valor mínimo
                    FindControl("hdfIsVidro", "input").value = retorno[4]; // Informa se o produto é vidro
                    FindControl("hdfIsAluminio", "input").value = retorno[5]; // Informa se o produto é vidro
                    FindControl("hdfM2Minimo", "input").value = retorno[6]; // Informa se o produto possui m² mínimo
                    FindControl("hdfTipoCalc", "input").value = retorno[7]; // Verifica como produto é calculado
                    var tipoCalc = retorno[7];

                    if(FindControl("txtAlturaIns", "input") != null && FindControl("txtAlturaIns", "input").value != "") {
                        GetAdicionalAlturaChapa();
                    }

                    var nomeControle = getNomeControleBenef();

                    // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                    if (retorno[4] == "true" && getNomeControleBenef() && FindControl("lnkBenef", "a") != null) {
                        FindControl("txtEspessura", "input").value = retorno[8];
                        FindControl("txtEspessura", "input").disabled = retorno[8] != "" && retorno[8] != "0";
                    }

                    if (FindControl("lnkBenef", "a") != null && nomeControle != null && nomeControle.indexOf("Inserir") > -1)
                        FindControl("lnkBenef", "a").style.display = exibirControleBenef(nomeControle) ? "" : "none";

                    // Se o produto não for vidro, desabilita os textboxes largura e altura,
                    // mas se o produto for alumínio e a empresa trabalhar com venda de alumínio
                    // no metro linear, deixa o campo altura habilitado
                    var cAltura = FindControl("txtAlturaIns", "input");
                    var cLargura = FindControl("txtLarguraIns", "input");

                    if(retorno[11] != "" && retorno[11] != "0")
                    {
                        cAltura.value = retorno[11];
                    }

                    if(retorno[12] != "" && retorno[12] != "0")
                    {
                        cLargura.value = retorno[12];
                    }

                    if(retorno[13] != "" && retorno[13] != "0")
                    {
                        loadProc(retorno[13], false);
                    }

                    if(retorno[14] != "" && retorno[14] != "0")
                    {
                        loadApl(retorno[14], false);
                    }

                    cAltura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
                    cLargura.disabled = CalcProd_DesabilitarLargura(tipoCalc);

                    FindControl("lblDescrProd", "span").innerHTML = retorno[2];
                }

                insKit = false;
                insTubo = false;
            }
            catch (err) {
                alert(err);

                insKit = false;
                insTubo = false;
            }
        }

        // Chamado quando um produto está para ser inserido no item_projeto
        function onSaveProd() {
            if (!validate("produto"))
                return false;

            var codProd = FindControl("txtCodProdIns", "input").value;
            var valor = FindControl("txtValorIns", "input").value;
            var qtde = FindControl("txtQtdeIns", "input").value;
            var altura = FindControl("txtAlturaIns", "input").value;
            var largura = FindControl("txtLarguraIns", "input").value;
            var valMin = FindControl("hdfValMin", "input").value;

            valMin = new Number(valMin.replace(',', '.'));
            if (codProd == "") {
                alert("Informe o código do produto.");
                return false;
            }
            else if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
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
            }
            // Se o textbox da largura estiver habilitado, deverá ser informada
            else if (FindControl("txtLarguraIns", "input").disabled == false && largura == "") {
                alert("Informe a largura.");
                return false;
            }

            if (!obrigarProcApl())
                return false;

            FindControl("txtValorIns", "input").disabled = false;
            FindControl("txtAlturaIns", "input").disabled = false;
            FindControl("txtLarguraIns", "input").disabled = false;
        }

        // Função chamada quando o produto está para ser atualizado
        function onUpdateProd() {
            if (!validate("produto"))
                return false;

            var valor = FindControl("txtValorIns", "input").value;
            var qtde = FindControl("txtQtdeIns", "input") != null ? FindControl("txtQtdeIns", "input").value :
                FindControl("lblQtde", "span") != null ? FindControl("lblQtde", "span").innerHTML : "";
            var altura = FindControl("txtAlturaIns", "input").value;
            var idProd = FindControl("hdfIdProdMater", "input").value;
            var codInterno = FindControl("hdfCodInterno", "input").value;
            var valMin = FindControl("hdfValMin", "input").value;



            var retorno = CadProjeto.GetProduto(idProjeto, codInterno, FindControl("hdfTipoEntrega", "input").value, FindControl("hdfCliRevenda", "input").value, FindControl("hdfIdCliente", "input").value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }
            else if (retorno[0] == "Prod") {
                retorno[3] = new Number(retorno[3].replace(',', '.'));
                if (new Number(valor.replace(',', '.')) < retorno[3]) {
                    alert("Valor especificado abaixo do valor mínimo (R$ " + retorno[3].toFixed(2).replace(".", ",") + ")");
                    return false;
                }
            }

            if (valor == "" || parseFloat(valor.replace(",", ".")) == 0) {
                alert("Informe o valor vendido.");
                return false;
            }
            else if (qtde == "0" || qtde == "") {
                alert("Informe a quantidade.");
                return false;
            }
            else if (FindControl("txtAlturaIns", "input").disabled == false) {
                if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                    alert("Informe a altura.");
                    return false;
                }
            }

            if (!obrigarProcApl())
                return false;
        }

        function GetAdicionalAlturaChapa(){
            var idProd = FindControl("hdfIdProdMater", "input").value;
            var altura = FindControl("txtAlturaIns", "input").value;
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;

            var retorno = MetodosAjax.GetValorTabelaProduto(idProd, tipoEntrega, idCli, false, false, 0, "", idProjeto, "", altura);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return;
            }
            else if(retorno == null){
                alert("Erro na recuperação do valor de tabela do produto.");
                return;
            }

            var valMin = FindControl("hdfValMin", "input");

            if(valMin != null) {
                valMin.value = retorno.replace(".", ",");
            }
            else{
                alert("Não foi possível encontrar o controle 'hdfValMin'");
                return false;
            }

            var valorIns = FindControl("txtValorIns", "input");

            if(valorIns != null){
                valorIns.value = retorno.replace(".", ",");
            }
            else{
                alert("Não foi possível encontrar o controle 'txtValorIns'");
                return false;
            }

        }

        // Calcula em tempo real a metragem quadrada do produto
        function calcM2Prod() {
            try {
                var idProd = FindControl("hdfIdProdMater", "input").value;
                var altura = FindControl("txtAlturaIns", "input").value;
                var largura = FindControl("txtLarguraIns", "input").value;
                var qtde = FindControl("txtQtdeIns", "input").value;
                var isVidro = FindControl("hdfIsVidro", "input").value == "true";
                var tipoCalc = FindControl("hdfTipoCalc", "input").value;
                var esp = FindControl("txtEspessura", "input") != null ? FindControl("txtEspessura", "input").value : 0;
                var idCliente = FindControl("hdfIdCliente", "input").value;

                var redondo = FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked;

                if (altura != "" && largura != "" &&
                    parseInt(altura) > 0 && parseInt(largura) > 0 &&
                    parseInt(altura) != parseInt(largura) && redondo) {
                    alert('O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.');

                    if (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked)
                        FindControl("Redondo_chkSelecao", "input").checked = false;

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
                        FindControl("hdfTotM2Calc", "input").value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
                        FindControl("lblTotM2Calc", "span").innerHTML = FindControl("hdfTotM2Calc", "input").value.replace('.', ',');
                    }

                    if (qtde != "" && qtde != "0")
                        calcTotalProd();

                    return false;
                }

                var adicVidroRedondoAte12mm = '<%= Glass.Configuracoes.Geral.AdicionalVidroRedondoAte12mm %>';
                var adicVidroRedondoAcima12mm = '<%= Glass.Configuracoes.Geral.AdicionalVidroRedondoAcima12mm %>';

                FindControl("lblTotM2Ins", "span").innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, "false").value;
                FindControl("hdfTotM2Calc", "input").value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
                FindControl("lblTotM2Calc", "span").innerHTML = FindControl("hdfTotM2Calc", "input").value.replace('.', ',');

                calcTotalProd();
            }
            catch (err) {

            }
        }

        // Calcula em tempo real o valor total do produto
        function calcTotalProd() {
            try {
                var valorIns = FindControl("txtValorIns", "input").value;

                if (valorIns == "")
                    return;

                var totM2 = FindControl("lblTotM2Ins", "span").innerHTML;
                var totM2Calc = new Number(FindControl("lblTotM2Calc", "span").innerHTML.replace(',', '.')).toFixed(2);
                var total = new Number(valorIns.replace(',', '.')).toFixed(2);
                var qtde = new Number(FindControl("txtQtdeIns", "input").value.replace(',', '.'));
                var altura = new Number(FindControl("txtAlturaIns", "input").value.replace(',', '.'));
                var largura = new Number(FindControl("txtLarguraIns", "input").value.replace(',', '.'));
                var tipoCalc = FindControl("hdfTipoCalc", "input").value;
                var m2Minimo = FindControl("hdfM2Minimo", "input").value;

                var retorno = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, FindControl("hdfAlturaCalc", "input"), largura, false, tipoCalc);
                if (retorno != "")
                    FindControl("lblTotalIns", "span").innerHTML = retorno;
            }
            catch (err) {

            }
        }

        // Função chamada pelo popup de escolha da Aplicação do produto
        function setApl(idAplicacao, codInterno, aplBenef) {
            aplBenef = aplBenef == true ? true : false;
            var campo = !aplBenef ? "txtAplIns" : "txtAplicacaoIns";

            FindControl(campo, "input").value = codInterno;
            FindControl("hdfIdAplicacao", "input").value = idAplicacao;
        }

        function loadApl(codInterno, aplBenef) {
            if (codInterno == "") {
                setApl("", "", aplBenef);
                return false;
            }

            aplBenef = aplBenef == true ? true : false;

            try {
                var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

                if (response == null || response == "") {
                    alert("Falha ao buscar Aplicação. Ajax Error.");
                    setApl("", "", aplBenef);
                    return false
                }

                response = response.split("\t");

                if (response[0] == "Erro") {
                    alert(response[1]);
                    setApl("", "", aplBenef);
                    return false;
                }

                setApl(response[1], response[2], aplBenef);
            }
            catch (err) {
                alert(err);
            }
        }

        // Função chamada pelo popup de escolha do Processo do produto
        function setProc(idProcesso, codInterno, codAplicacao, procBenef) {
            procBenef = procBenef == true ? true : false;
            var campo = !procBenef ? "txtProcIns" : "txtProcessoIns";

            FindControl(campo, "input").value = codInterno;
            FindControl("hdfIdProcesso", "input").value = idProcesso;

            var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProdMater", "input").value);
            var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

            if(idSubgrupo.value != "" && retornoValidacao.value == "false" && FindControl("txtProcIns", "input").value != "")
            {
                FindControl("txtProcIns", "input").value = "";
                alert("Este processo não pode ser selecionado para este produto.")
                return false;
            }

            if (codAplicacao != "")
                loadApl(codAplicacao, procBenef);
        }

        function buscarProcessos(){
            var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProdMater", "input").value);
            openWindow(450, 700, "../../Utils/SelEtiquetaProcesso.aspx?idSubgrupo=" + idSubgrupo.value);
        }

        function loadProc(codInterno, procBenef) {
            if (codInterno == "") {
                setProc("", "", "", procBenef);
                return false;
            }

            procBenef = procBenef == true ? true : false;

            try {
                var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

                if (response == null || response == "") {
                    alert("Falha ao buscar Processo. Ajax Error.");
                    setProc("", "", "", procBenef);
                    return false
                }

                response = response.split("\t");

                if (response[0] == "Erro") {
                    alert(response[1]);
                    setProc("", "", "", procBenef);
                    return false;
                }

                setProc(response[1], response[2], response[3], procBenef);
            }
            catch (err) {
                alert(err);
            }
        }

        function setOrcamento(idOrcamento) {
            FindControl("txtIdOrcamento", "input").value = idOrcamento;
        }

        function openRptTotais() {
            var idProjeto = '<%= Request["IdProjeto"] %>';

            openWindow(600, 800, "../../Relatorios/Projeto/RelBase.aspx?rel=totaisProjeto&idProjeto=" + idProjeto);

            return false;
        }

        function setCalcObs(idItemProjeto, button) {
            var obs = button.parentNode.parentNode.parentNode.getElementsByTagName('textarea')[0].value;

            var retorno = CadProjeto.SalvaObsItemProjeto(idItemProjeto, obs).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }
            else
                alert("Observação salva.");
        }

        function abrirCADProject(codModelo, idPecaItemProj){
            var estaConferido = CadProjeto.EstaConferido(FindControl('hdfIdItemProjeto', 'input').value);

            if (estaConferido != null && estaConferido.value == 'false') {
                alert('Confirme o projeto antes de editar as imagens.');
                return false;
            }

            var url = removeParam("idPecaItemProj", document.location.href);
            url = removeParam("cancel", url);
            var projetoCadProject = CadProjeto.CriarProjetoCADProject(codModelo, idPecaItemProj, url, GetQueryString("pcp") == "1");

            if(projetoCadProject.error != null){
                alert(projetoCadProject.error.description);
                return false;
            }

            var w = screen.width;
            var h = screen.height;

            setTimeout(function(){}, 100);

            openWindow(h, w, projetoCadProject.value);

            setTimeout(function(){}, 100);

            window.close();

            return false;
        }

        function removeParam(key, sourceURL) {
            var rtn = sourceURL.split("?")[0],
                param,
                params_arr = [],
                queryString = (sourceURL.indexOf("?") !== -1) ? sourceURL.split("?")[1] : "";
            if (queryString !== "") {
                params_arr = queryString.split("&");
                for (var i = params_arr.length - 1; i >= 0; i -= 1) {
                    param = params_arr[i].split("=")[0];
                    if (param === key) {
                        params_arr.splice(i, 1);
                    }
                }
                rtn = rtn + "?" + params_arr.join("&");
            }
            return rtn;
        }

        function novoCalculo() {
            window.location = window.location.href;

            openWindow(600, 750, 'SelModelo.aspx?idProjeto=' + 
                (FindControl('hdfIdProjeto', 'input') != null ? FindControl('hdfIdProjeto', 'input').value : '0') + 
                '&apenasVidro=' + (FindControl('hdfApenasVidros', 'input') != null ? FindControl('hdfApenasVidros', 'input').value : 'false') + 
                '&Parceiro=true');
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table width="100%">
                    <tr>
                        <td width="80%" valign="top" align="center">
                            <asp:DetailsView ID="dtvProjeto" runat="server" AutoGenerateRows="False" DataSourceID="odsProjeto"
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
                                                        <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                            onblur="getCli(this);" Text='<%# Eval("IdCliente") %>'
                                                            onkeydown="if (isEnter(event)) getCli(this);"></asp:TextBox>
                                                        <asp:TextBox ID="txtNomeCliente" runat="server" Text='<%# Bind("NomeCliente") %>'
                                                            Width="250px"></asp:TextBox>
                                                        <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../../Utils/SelCliente.aspx?tipo=projeto'); return false;">
                                                                    <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataCad" runat="server" ReadOnly="True" Width="70px" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label14" runat="server"
                                                            Text="Tipo Entrega "></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" valign="middle" class="dtvAlternatingRow">
                                                        <asp:DropDownList ID="ddlTipoEntrega" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsTipoEntrega" DataTextField="Descr" DataValueField="Id"
                                                            SelectedValue='<%# Bind("TipoEntrega") %>' >
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label15" runat="server"
                                                            Text="Orçamento"></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIdOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("IdOrcamento") %>' Width="70px" ></asp:TextBox>
                                                        <asp:LinkButton ID="lnkSelOrcamento" runat="server"
                                                            OnClientClick="openWindow(590, 760, '../../Utils/SelOrcamento.aspx'); return false;"
                                                            >
                                                                     <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap" colspan="2">
                                                        <asp:CheckBox ID="chkApenasVidro" runat="server" Checked='<%# Bind("ApenasVidro") %>'
                                                            Text="Calcular apenas vidros" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label16" runat="server"  Text="Total"></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtTotal" runat="server" ReadOnly="True" Text='<%# Eval("Total", "{0:C}") %>'
                                                            Width="70px" ></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td class="dtvHeader">
                                                        Observação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:TextBox ID="TextBox3" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="504px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                                            <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                            <asp:HiddenField ID="hdfCliRevenda" runat="server"
                                                Value='<%# Eval("CliRevenda") %>' />
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <table cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Cliente
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtNumCli" runat="server" onblur="getCli(this);" Width="50px" onkeydown="if (isEnter(event)) getCli(this);"
                                                            onkeypress="return soNumeros(event, true, true);" ></asp:TextBox>
                                                        <asp:TextBox ID="txtNomeCliente" runat="server" Width="250px" Text='<%# Bind("NomeCliente") %>'></asp:TextBox>
                                                        <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../../Utils/SelCliente.aspx?tipo=projeto'); return false;">
                                                                    <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        Data
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDataCad" runat="server" ReadOnly="True" Width="70px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label14" runat="server"
                                                            Text="Tipo Entrega "></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" valign="middle">
                                                        <asp:DropDownList ID="ddlTipoEntrega" runat="server" AppendDataBoundItems="True"
                                                            DataSourceID="odsTipoEntrega" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoEntrega") %>'>
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="Label15" runat="server"
                                                            Text="Orçamento"></asp:Label>
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIdOrcamento" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("IdOrcamento") %>' Width="70px" ></asp:TextBox>
                                                        <asp:LinkButton ID="lnkSelOrcamento" runat="server"
                                                            OnClientClick="openWindow(590, 760, '../../Utils/SelOrcamento.aspx'); return false;"
                                                            >
                                                                     <img border="0" src="../../Images/Pesquisar.gif" /></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" colspan="2" nowrap="nowrap">
                                                        <asp:CheckBox ID="chkApenasVidro" runat="server" Checked='<%# Bind("ApenasVidro") %>'
                                                            Text="Calcular apenas vidros" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        &nbsp;
                                                    </td>
                                                    <td align="left" class="dtvAlternatingRow" nowrap="nowrap">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td class="dtvHeader">
                                                        Observação
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:TextBox ID="TextBox4" runat="server" MaxLength="300" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="504px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <table cellpadding="2" cellspacing="2">
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Num. Projeto
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNumProj" runat="server" Text='<%# Eval("IdProjeto") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:Label ID="lblCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold; height: 25px;">
                                                        <asp:Label ID="Label17" runat="server"
                                                            Text="Funcionário"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="height: 25px" colspan="3">
                                                        <asp:Label ID="lblNomeFunc" runat="server"
                                                            Text='<%# Eval("NomeFunc") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold; height: 25px;">
                                                        <asp:Label ID="Label21" runat="server"  Text="Total"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="height: 25px">
                                                        <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("Total", "{0:C}") %>'
                                                            ></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label19" runat="server"  Text="Loja"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeLoja" runat="server" Text='<%# Eval("NomeLoja") %>'
                                                            ></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label18" runat="server"
                                                            Text="Tipo Entrega"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoEntrega" runat="server"
                                                            Text='<%# Eval("DescrTipoEntrega") %>' ></asp:Label>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td align="left" nowrap="nowrap" colspan="6">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr style='font-weight: bold; height: 25px; <%= !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "display: none": "" %>'>
                                                                <td>
                                                                    <asp:Label ID="Label20" runat="server"  Text="Valor ICMS"></asp:Label>
                                                                    </td>
                                                                <td>
                                                                    <asp:Label ID="lblValorIcms" runat="server" ForeColor="Red"
                                                                         Text='<%# Eval("ValorIcms", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                            <asp:HiddenField ID="hdfCliRevenda" runat="server" Value='<%# Eval("CliRevenda") %>' />
                                            <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCliente") %>' />
                                            <asp:HiddenField ID="hdfApenasVidros" runat="server" Value='<%# Eval("ApenasVidro").ToString().ToLower() %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                                OnClientClick="return onUpdate();" />
                                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                                Text="Cancelar" />
                                            <uc2:ctrllinkquerystring ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdProjeto"
                                                Text='<%# Bind("IdProjeto") %>' />
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="return onInsert();"
                                                Text="Inserir" />
                                            <asp:Button ID="btnCancelar0" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                                Text="Cancelar" />
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                            <asp:Button ID="btnVoltar" runat="server" OnClick="btnCancelar_Click" Text="Voltar" />
                                            <asp:Button ID="btnGerarOrcamento" runat="server" OnClick="btnGerarOrcamento_Click"
                                                Text="Gerar Orçamento"
                                                OnClientClick="return confirm('Gerar orçamento para este projeto?');"
                                                 />
                                            <asp:Button ID="btnGerarPedido" runat="server" OnClick="btnGerarPedido_Click" OnClientClick="return confirm('Ao gerar um pedido, este projeto será finalizado e não poderá mais ser alterado, deseja continuar?');"
                                                Text="Gerar Pedido"  />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                        </td>
                        <td align="center">
                            <uc3:ctrlCorItemProjeto ID="ctrlCorItemProjeto2" runat="server" ExibirTooltip="false" Titulo="Alterar cores dos materiais de todos os cálculos"
                                OnLoad="ctrlCorItemProjeto2_Load" OnCorAlterada="ctrlCorItemProjeto_CorAlterada"></uc3:ctrlCorItemProjeto>
                        </td>
                    </tr>
                </table>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" SelectMethod="GetElement" TypeName="Glass.Data.DAL.PedidoEspelhoDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProjeto" runat="server" DataObjectTypeName="Glass.Data.Model.Projeto"
                    InsertMethod="Insert" SelectMethod="GetElement" TypeName="Glass.Data.DAL.ProjetoDAO"
                    UpdateMethod="Update" OnInserted="odsProjeto_Inserted" OnUpdated="odsProjeto_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idProjeto" QueryStringField="idProjeto" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdProjeto" runat="server" />
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table style="width: 100%">
                    <tr>
                        <td class="subtitle1">
                            <asp:Label ID="lblCalculoProjeto" runat="server" Text="Cálculo de Projeto"></asp:Label>
                        </td>
                        <td class="subtitle1" align="center">
                            <asp:Label ID="lblCalculosEfetuados" runat="server" Text="Cálculos Efetuados"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td width="80%" valign="top">
                            <table style="width: 100%">
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btnNovoCalculo" runat="server" Text="Novo Cálculo" OnClientClick="novoCalculo();" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="lblImagem" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" valign="top">
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:DetailsView ID="dtvImagem" runat="server" AutoGenerateRows="False"
                                                        DataSourceID="odsItemProjetoImagem" GridLines="None">
                                                        <Fields>
                                                            <asp:TemplateField ShowHeader="False">
                                                                <ItemTemplate>
                                                                    <asp:Image ID="imgImagemProjeto" runat="server"
                                                                        ImageUrl='<%# Eval("ImagemUrl") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ShowHeader="False">
                                                                <ItemTemplate>
                                                                    <asp:Button ID="btnImprimir" runat="server" Text="Imprimir"
                                                                        onprerender="btnImprimir_PreRender" />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" />
                                                            </asp:TemplateField>
                                                        </Fields>
                                                    </asp:DetailsView>
                                                    <script type="text/javascript">
                                                        var imagem = FindControl("dtvImagem", "table").parentNode;
                                                        if (imagem.offsetWidth >= 250)
                                                        {
                                                            var imagemGrande = FindControl("lblImagem", "span");
                                                            imagemGrande.innerHTML = imagem.innerHTML;
                                                            imagem.innerHTML = "";
                                                        }
                                                    </script>
                                                </td>
                                                <td width="100%">
                                                    <table style="width: 100%">
                                                        <tr>
                                                            <td align="center">
                                                                <asp:Label ID="lblMedidas" runat="server" Text="Medidas das Peças" Font-Bold="True"
                                                                    Visible="False"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <asp:Table ID="tbPecaModelo" runat="server">
                                                                </asp:Table>
                                                                <br />
                                                                <table id="tbInsKit" runat="server" visible="false">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblKit" runat="server" Text="Kit"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtCodKit" runat="server" Width="50px" onkeydown="if (isEnter(event)) { insKit=true; loadProduto(this.value);}"
                                                                                onkeypress="return !(isEnter(event));" onblur="insKit=true; loadProduto(this.value);"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <a href="#" onclick="insKit=true; openWindow(450, 700, &quot;../../Utils/SelProd.aspx<%= this.BuscarKitQueryString() %>&quot;); return false;">
                                                                                <img border="0" src="../../Images/Pesquisar.gif" /></a>
                                                                        </td>
                                                                        <td nowrap="nowrap">
                                                                            <asp:Label ID="lblDescrKit" runat="server"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblQtdKit" runat="server" Text="Qtd."></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtQtdeKit" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                                                Width="50px"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblValorKit" runat="server" Text="Valor"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtValorKit" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                                Width="50px"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <table id="tbInsTubo" runat="server" visible="false">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblTubo" runat="server" Text="Tubo"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtCodTubo" runat="server" Width="50px" onkeydown="if (isEnter(event)) { insTubo=true; loadProduto(this.value); }"
                                                                                onkeypress="return !(isEnter(event));" onblur="insTubo=true; loadProduto(this.value);"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <a href="#" onclick="insTubo=true; openWindow(450, 700, '../../Utils/SelProd.aspx?descricao=Tubo'); return false;">
                                                                                <img border="0" src="../../Images/Pesquisar.gif" /></a>
                                                                        </td>
                                                                        <td nowrap="nowrap">
                                                                            <asp:Label ID="lblDescrTubo" runat="server"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblQtdTubo" runat="server" Text="Qtd."></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtQtdeTubo" runat="server" onkeydown="if (isEnter(event)) return false;"
                                                                                onkeypress="return soNumeros(event, true, true);" Width="50px"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblComprTubo" runat="server" Text="Compr."></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtComprTubo" runat="server" onkeydown="if (isEnter(event)) return false;"
                                                                                onkeypress="return soNumeros(event, false, true);" Width="50px"></asp:TextBox>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblValorTubo" runat="server" Text="Valor"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtValorTubo" runat="server" onkeydown="if (isEnter(event)) return false;"
                                                                                onkeypress="return soNumeros(event, false, true);" Width="50px"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                                <asp:HiddenField ID="hdfIdKit" runat="server" />
                                                                <asp:HiddenField ID="hdfIdTubo" runat="server" />
                                                                <asp:HiddenField ID="hdfKitValMin" runat="server" />
                                                                <asp:HiddenField ID="hdfTuboValMin" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                &nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <table runat="server" visible="false" id="tbAmbiente">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="Label11" runat="server" Font-Bold="True" Text="Ambiente:"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:TextBox ID="txtAmbiente" runat="server" MaxLength="30"></asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2" align="center">
                                                                            <asp:Button ID="btnConfAmbiente" runat="server" Text="Confirmar" OnClick="btnConfAmbiente_Click"
                                                                                Visible="False" />
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
                                                                <asp:Label ID="lblMedidasInst" runat="server" Text="Medidas da área Instalação" Font-Bold="True"
                                                                    Visible="False"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="center">
                                                                <asp:PlaceHolder ID="pchTbTipoInst" runat="server" Visible="False">
                                                                    <table>
                                                                        <tr>
                                                                            <td align="center">
                                                                                <asp:Table ID="tbMedInst" runat="server">
                                                                                </asp:Table>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="4" align="center">
                                                                                <asp:Button ID="btnCalcMed" runat="server" OnClick="btnCalcMed_Click" Text="Calcular Medidas" />
                                                                                <asp:Button ID="btnConfCalc" runat="server" OnClick="btnConfCalc_Click" Text="Confirmar" />
                                                                                <asp:Button ID="btnCancCalc" runat="server" OnClick="btnCancCalc_Click" Text="Limpar" />
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </asp:PlaceHolder>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="right">
                                                                <table id="tbSubtotal" runat="server" visible="false">
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblDescrM2Vao" runat="server" Text="Área do Vão:" Font-Bold="True"
                                                                                Font-Size="Small"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblM2Vao" runat="server" Font-Bold="False" Font-Size="Small"></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblDescrSubtotal" runat="server" Text="Subtotal:" Font-Bold="True"
                                                                                Font-Size="Small" ></asp:Label>
                                                                        </td>
                                                                        <td>
                                                                            <asp:Label ID="lblSubtotal" runat="server" Font-Bold="False" Font-Size="Small"
                                                                                ></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="center" valign="top">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td height="180" valign="top">
                                        <asp:GridView GridLines="None" ID="grdItemProjeto" runat="server" AllowPaging="True"
                                            AllowSorting="True" AutoGenerateColumns="False"
                                            DataKeyNames="IdItemProjeto" DataSourceID="odsItemProjeto"
                                            OnRowCommand="grdItemProjeto_RowCommand"
                                            CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkEditItem" runat="server" CommandArgument='<%# Eval("IdItemProjeto") %>'
                                                            CommandName="EditarItem">
                                                    <img src="../../Images/edit.gif" border="0"></asp:LinkButton>
                                                        <asp:LinkButton ID="lnkExcluirItem" runat="server" CommandName="Delete" OnClientClick="return confirm('Tem certeza que deseja excluir este cálculo?');"
                                                            Visible='<%# Eval("EditDeleteVisible") %>'>
                                                    <img src="../../Images/ExcluirGrid.gif" border="0"></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Wrap="False" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="CodigoModelo" HeaderText="Cód." SortExpression="CodigoModelo">
                                                    <HeaderStyle Wrap="False" />
                                                    <ItemStyle Wrap="False" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Ambiente" HeaderText="Ambiente" SortExpression="Ambiente" />
                                                <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total" SortExpression="Total">
                                                    <ItemStyle Wrap="False" />
                                                </asp:BoundField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <a href="#" id="lnkObsCalc" onclick="exibirObs(<%# Eval("IdItemProjeto") %>, this); return false;">
                                                            <img border="0" src="../../Images/blocodenotas.png" /></a>
                                                        <table id='tbObsCalc_<%# Eval("IdItemProjeto") %>' cellspacing="0" style="display: none;">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:TextBox ID="txtObsCalc" runat="server" Width="320" Rows="4" MaxLength="500"
                                                                        TextMode="MultiLine" Text='<%# Eval("Obs") %>'></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <input id="btnSalvarObs" onclick='setCalcObs(<%# Eval("IdItemProjeto") %>, this); return false;'
                                                                        type="button" value="Salvar" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <uc3:ctrlCorItemProjeto ID="ctrlCorItemProjeto1" runat="server" IdItemProjeto='<%# Eval("IdItemProjeto") %>'
                                                            OnCorAlterada="ctrlCorItemProjeto_CorAlterada" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>

<PagerStyle CssClass="pgr"></PagerStyle>

<EditRowStyle CssClass="edit"></EditRowStyle>

<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                        </asp:GridView>
                                        <br />
                                        <asp:Button ID="btnNovoCalculoDupl" runat="server" Text="Novo Cálculo ()"
                                            OnClientClick="duplicar(); return false"
                                            Visible="False" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 19px">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblDescrTotal" runat="server" Text="Total Projeto:" Font-Bold="True"
                                                        Visible="False" Font-Size="Small" ></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:Label ID="lblTotalProj" runat="server" Font-Bold="False" Visible="False"
                                                        Font-Size="Small" ></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblObsTotal" runat="server" Font-Size="Smaller" Text="Considerando otimização das barras de alumínio"
                                                        Visible="False" ></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="center">
                                                    <asp:LinkButton ID="lnkRelatorio" runat="server"
                                                        OnClientClick="openRptTotais(); return false;" >Visualizar totais</asp:LinkButton>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:DetailsView ID="dtvImagemMini" runat="server" AutoGenerateRows="False"
                                            DataSourceID="odsItemProjetoImagem" GridLines="None">
                                            <Fields>
                                                <asp:TemplateField ShowHeader="False">
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgImagemProjeto0" runat="server"
                                                            ImageUrl='<%# Eval("ImagemUrlMini") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                            </Fields>
                                        </asp:DetailsView>
                                        <asp:HiddenField ID="hdfDuplicarCodigo" runat="server" />
                                        <asp:HiddenField ID="hdfDuplicarCorVidro" runat="server" />
                                        <asp:HiddenField ID="hdfDuplicarCorAluminio" runat="server" />
                                        <asp:HiddenField ID="hdfDuplicarCorFerragem" runat="server" />
                                        <asp:HiddenField ID="hdfDuplicarEspessura" runat="server" />
                                        <asp:HiddenField ID="hdfDuplicarApenasVidros" runat="server" />
                                        <asp:HiddenField ID="hdfDuplicarMedidaExata" runat="server" />
                                        <colo:VirtualObjectDataSource culture="pt-BR" ID="odsItemProjetoImagem" runat="server"
                                            SelectMethod="GetElementForProjetoAvulso"
                                            TypeName="Glass.Data.DAL.ItemProjetoDAO">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="hdfIdItemProjeto" Name="idItemProjeto"
                                                    PropertyName="Value" Type="UInt32" />
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
        <tr>
            <td align="center" class="subtitle1">
                <asp:Label ID="lblMateriais" runat="server" Text="Materiais Utilizados" Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdMaterialProjeto" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsMaterialItemProjeto"
                    DataKeyNames="IdMaterItemProj" ShowFooter="True"
                    OnPreRender="grdMaterialProjeto_PreRender" OnRowCommand="grdMaterialProjeto_RowCommand"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Visible='<%# Eval("EditVisible") %>'>
                                    <img border="0" src="../../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" Visible='<%# Eval("DeleteVisible") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onUpdateProd();" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfIdItemProjetoMater" runat="server" Value='<%# Bind("IdItemProjeto") %>' />
                                <asp:HiddenField ID="hdfIdPecaItemProj" runat="server"
                                    Value='<%# Bind("IdPecaItemProj") %>' />
                                <asp:HiddenField ID="hdfValorAcrescimo" runat="server"
                                    Value='<%# Bind("ValorAcrescimo") %>' />
                                <asp:HiddenField ID="hdfValorDesconto" runat="server"
                                    Value='<%# Bind("ValorDesconto") %>' />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>&nbsp;
                                <asp:HiddenField ID="hdfIdProdMater" runat="server" Value='<%# Bind("IdProd") %>' />
                                <asp:HiddenField ID="hdfCodInterno" runat="server" Value='<%# Eval("CodInterno") %>' />
                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                <asp:HiddenField ID="hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                <asp:HiddenField ID="hdfIsAluminio" runat="server" Value='<%# Eval("IsAluminio") %>' />
                                <asp:HiddenField ID="hdfM2Minimo" runat="server" Value='<%# Eval("M2Minimo") %>' />
                                <asp:HiddenField ID="hdfIdMaterProjMod" runat="server" />
                                <asp:HiddenField ID="hdfCustoProd" runat="server"
                                    Value='<%# Eval("CustoCompraProduto") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodProdIns" runat="server" Width="50px" onkeydown="if (isEnter(event)) loadProduto(this.value);"
                                    onkeypress="return !(isEnter(event));" onblur="loadProduto(this.value);"></asp:TextBox>
                                <asp:Label ID="lblDescrProd0" runat="server"></asp:Label>
                                <a href="#" onclick="openWindow(450, 700, '../../Utils/SelProd.aspx'); return false;">
                                    <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                <asp:HiddenField ID="hdfValMin" runat="server" />
                                <asp:HiddenField ID="hdfIsVidro" runat="server" />
                                <asp:HiddenField ID="hdfTipoCalc" runat="server" />
                                <asp:HiddenField ID="hdfIsAluminio" runat="server" />
                                <asp:HiddenField ID="hdfM2Minimo" runat="server" />
                                <asp:HiddenField ID="hdfCustoProd" runat="server" />
                            </FooterTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtdeIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                    Text='<%# Bind("Qtde") %>' Width="50px"
                                    Visible='<%# Eval("IdPecaItemProj") == null %>'></asp:TextBox>
                                <asp:Label ID="lblQtde" runat="server" Text='<%# Eval("Qtde") %>'
                                    Visible='<%# Eval("IdPecaItemProj") != null %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtQtdeIns" runat="server" onkeydown="if (isEnter(event)) calcM2Prod();"
                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);" onblur="calcM2Prod();" Width="50px"></asp:TextBox>
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
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("AlturaLista") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAlturaIns" runat="server" onblur="GetAdicionalAlturaChapa(); calcM2Prod();" Text='<%# Bind("Altura") %>'
                                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                    onchange="FindControl('hdfAlturaCalc', 'input').value = this.value; arredondaAltura(FindControl('hdfAlturaCalc', 'input'));"
                                    Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdfAlturaCalc" runat="server" Value='<%# Bind("AlturaCalc") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAlturaIns" runat="server" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                    onchange="FindControl('hdfAlturaCalcIns', 'input').value = this.value; arredondaAltura(FindControl('hdfAlturaCalcIns', 'input'));"
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
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotM2Ins" runat="server"></asp:Label>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total M2 Calc." SortExpression="TotM2Calc">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("TotM2Calc") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblTotM2Calc" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                                <asp:HiddenField ID="hdfTotM2Calc" runat="server" Value='<%# Eval("TotM2Calc") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotM2Calc" runat="server"></asp:Label>
                                <asp:HiddenField ID="hdfTotM2Calc" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Valor", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorIns" runat="server" onblur="calcTotalProd();" OnLoad="txtValorIns_Load"  onkeypress="return soNumeros(event, false, true);"
                                    Text='<%# Bind("Valor") %>' Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorIns" runat="server" OnLoad="txtValorIns_Load" onkeydown="if (isEnter(event)) calcTotalProd();"
                                    onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProd();"
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                            <EditItemTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="loadApl(this.value);" onkeydown="if (isEnter(event)) loadApl(this.value);"
                                                onkeypress="return !(isEnter(event));" Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="openWindow(450, 700, '../../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="loadApl(this.value);" onkeydown="if (isEnter(event)) loadApl(this.value);"
                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="openWindow(450, 700, '../../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../../Images/Pesquisar.gif" /></a>
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
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="loadProc(this.value);" onkeydown="if (isEnter(event)) loadProc(this.value);"
                                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick='buscarProcessos(); return false;'>
                                                <img border="0" src="../../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="loadProc(this.value);" onkeydown="if (isEnter(event)) loadProc(this.value);"
                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick='buscarProcessos(); return false;'>
                                                <img border="0" src="../../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label>
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
                                <asp:Label ID="Label13" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="lblValorBenef" runat="server"></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblValorBenef" runat="server"></asp:Label>
                            </FooterTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(this); return false;"
                                    Visible='<%# Eval("BenefVisible") %>'>
                                    <img border="0" src="../../Images/gear_add.gif" />
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
                                                    <td class="dtvFieldBold">
                                                        Ped. Cli
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtPedCli" runat="server" MaxLength="30" Width="50px" Text='<%# Bind("PedCli") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc4:ctrlBenef ID="ctrlBenefEditar" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>' ValidationGroup="produto"
                                                OnLoad="ctrlBenef_Load" Redondo='<%# Bind("Redondo") %>' CallbackCalculoValorTotal="setValorTotal" OnPreRender="ctrlBenef_PreRender" />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(this); return false;"
                                    Style="display: none">
                                    <img border="0" src="../../Images/gear_add.gif" />
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
                                                        <asp:TextBox ID="txtPedCli" runat="server" MaxLength="50" Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <uc4:ctrlBenef ID="ctrlBenefInserir" runat="server" OnLoad="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotal" ValidationGroup="produto" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:HiddenField ID="hdfIdAplic" runat="server" />
                                            <asp:HiddenField ID="hdfIdProc" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInsProd" runat="server" CommandName="Insert" OnClick="lnkInsProd_Click"
                                    OnClientClick="return onSaveProd();">
                                    <img border="0" src="../../Images/ok.gif" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMaterialItemProjeto" runat="server" DataObjectTypeName="Glass.Data.Model.MaterialItemProjeto"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.MaterialItemProjetoDAO"
                    UpdateMethod="Update" OnDeleted="odsMaterialItemProjeto_Deleted" OnDeleting="odsMaterialItemProjeto_Deleting"
                    OnUpdated="odsMaterialItemProjeto_Updated" OnUpdating="odsMaterialItemProjeto_Updating">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdItemProjeto" Name="idItemProjeto" PropertyName="Value"
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsItemProjeto" runat="server" DataObjectTypeName="Glass.Data.Model.ItemProjeto"
                    DeleteMethod="Delete" SelectMethod="GetList" TypeName="Glass.Data.DAL.ItemProjetoDAO"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" OnDeleted="odsItemProjeto_Deleted">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idProjeto" QueryStringField="idProjeto" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfIdProdMater" runat="server" />
                <asp:HiddenField ID="hdfIdItemProjeto" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoEntrega" runat="server" SelectMethod="GetTipoEntregaForPojeto"
                    TypeName="Glass.Data.Helper.DataSources"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
