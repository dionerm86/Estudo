<%@ Page Title="Emissão de Nota Fiscal" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadNotaFiscal.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadNotaFiscal" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlTextoTooltip.ascx" TagName="ctrlTextoTooltip" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlSelPopup.ascx" TagName="ctrlSelPopup" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc6" %>
<%@ Register src="../Controls/ctrlNaturezaOperacao.ascx" tagname="ctrlNaturezaOperacao" tagprefix="uc7" %>
<%@ Register src="../Controls/ctrlSelProduto.ascx" tagname="ctrlSelProduto" tagprefix="uc8" %>
<%@ Register Src="../Controls/ctrlConsultaCadCliSintegra.ascx" TagName="ctrlConsultaCadCliSintegra"TagPrefix="uc9" %>
<%@ Register Src="../Controls/ctrlFormaPagtoNotaFiscal.ascx" TagName="ctrlFormaPagtoNotaFiscal" TagPrefix="uc10" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

    var manual = <%= (Request["manual"] == "1").ToString().ToLower() %>;
    var inserindo = <%= (Request["idNf"] == null).ToString().ToLower() %>;
    var isNfEntradaTerceiros = <%= IsNfEntradaTerceiros().ToString().ToLower() %>;
    var serieNf = <%= Glass.Configuracoes.FiscalConfig.NotaFiscalConfig
            .SeriePadraoNFe(null, null, Glass.Conversoes.StrParaIntNullable(Request["finalidade"]).GetValueOrDefault(0) == (int)Glass.Data.Model.NotaFiscal.FinalidadeEmissaoEnum.Ajuste).ToString() %>;

    function atualizaTotalParcelas() {
        var nota = FindControl("txtTotalNota", "input");
        var manual = FindControl("txtTotalNotaManual", "input");
        var total = FindControl("hdfTotalParcelas", "input");

        total.value = manual && manual.value && parseFloat(manual.value.replace(",", ".")) > 0 ? manual.value :
            nota ? nota.value : "0";

        exibeParcelas();
    }

    function exibirInfoAdicProdNf(botao, complTabela)
    {
        for (iTip = 0; iTip < 2; iTip++)
        {
            TagToTip('tbInfoAdicProdNf' + complTabela, FADEIN, 300, COPYCONTENT, false, TITLE, 'Informações Adicionais', CLOSEBTN, true,
                CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9-getTableWidth('tbInfoAdicProdNf' + complTabela), -41-getTableHeight('tbInfoAdicProdNf' + complTabela)]);
        }
    }

    function calcValorPis(atualizaBc)
    {
        if (!isNfEntradaTerceiros) return;
        atualizaBc = atualizaBc == true ? true : false;

        var aliq = FindControl("txtAliqPis", "input").value;
        aliq = aliq.replace(",", ".");
        aliq = parseFloat(aliq);

        var total = FindControl("txtBcPis", "input");
        if (atualizaBc)
        {
            var totalProd = FindControl("lblTotalIns", "span").innerHTML.replace("R$", "").replace(" ", "");
            totalProd = totalProd.replace(/\./g, "").replace(",", ".");
            totalProd = parseFloat(totalProd);
            if (isNaN(totalProd)) totalProd = 0;

            var valorIpi = FindControl("selCstIpi_hdfValor", "input").value == "0" ? "0" :
                FindControl("txtValorIpi", "input") ? FindControl("txtValorIpi", "input").value :
                FindControl("lblValorIpiIns", "span").innerHTML.replace("R$", "").replace(" ", "");
            valorIpi = valorIpi.replace(/\./g, "").replace(",", ".");
            valorIpi = parseFloat(valorIpi);
            if (isNaN(valorIpi)) valorIpi = 0;

            total.value = (totalProd + valorIpi).toString().replace(".", ",");
        }
        total = total.value.replace(/\./g, "").replace(",", ".");
        total = parseFloat(total);

        FindControl("txtValorPis", "input").value = (total * aliq / 100).toFixed(4).replace(".", ",");
    }

    function calcValorCofins(atualizaBc)
    {
        if (!isNfEntradaTerceiros) return;
        atualizaBc = atualizaBc == true ? true : false;

        var aliq = FindControl("txtAliqCofins", "input").value;
        aliq = aliq.replace(",", ".");
        aliq = parseFloat(aliq);

        //var total = FindControl("txtBcCofins", "input");
        var total = FindControl("txtBcPis", "input");
        if (atualizaBc) total.value = FindControl("txtBcPis", "input").value.replace("R$", "").replace(" ", "");
        total = total.value.replace(/\./g, "").replace(",", ".");
        total = parseFloat(total);

        FindControl("txtValorCofins", "input").value = (total * aliq / 100).toFixed(4).replace(".", ",");
    }

    function openInfoAdic(idProdNf)
    {
        openWindow(600, 800, "../Utils/InfoAdicProdNf.aspx?idProdNf=" + idProdNf);
    }

    function infComplProd(idProdNf) {
        openWindow(200, 400, "../Utils/SetInfComplProdNf.aspx?idProdNf=" + idProdNf);
    }

    function validarCfop(codCfop)
    {
        if (codCfop == "" || codCfop == null)
            return;

        //Pega o primeiro numero do CFOP
        var comecoCfop = codCfop.substring(0,1);

        //Se o tipo da nota for saida verifica se o CFOP se inicia com 5,6 ou 7
        if (GetQueryString("tipo") == "2") {
            if (comecoCfop == "5" || comecoCfop == "6" || comecoCfop == "7") {
                return "true"
            }

            return "false";
        }
        ///Se o tipo da nota for entrada ou entrada terceiros verifica se o cfop se inicia com 1,2 ou 3
        if (GetQueryString("tipo") == "1" || GetQueryString("tipo") == "3") {
            if (comecoCfop == 1 || comecoCfop == "2" || comecoCfop == "3") {
                return "true"
            }
            return "false";
        }

        return "true";
    }

    var cfopDevolucao = null;

    function atualizaCfopDevolucao(controle, id) {
        controle = eval(controle);

        var retorno = controle ? controle.IsCfopDevolucao() : false;
        if (cfopDevolucao != null && (!!cfopDevolucao) != retorno) {
            limparDestinatario();
            limparRemetente();
        }

        var idNf = FindControl("hdfIdNf", "input") != null ? FindControl("hdfIdNf", "input").value : 0;
        var idCfop = FindControl("ctl00_ctl00_Pagina_Conteudo_dtvNf_ctrlNaturezaOperacaoNf_selNaturezaOperacao_txtDescr", "input").value;

        var validar = validarCfop(idCfop);

        if (validar == "false") {
            alert("A Natureza da Operação selecionada não pode ser usada nesse tipo de nota")
            FindControl("ctl00_ctl00_Pagina_Conteudo_dtvNf_ctrlNaturezaOperacaoNf_selNaturezaOperacao_txtDescr", "input").value = "";
        }

        if(CadNotaFiscal.habilitarReferenciaNFe(idNf, idCfop, GetQueryString("tipo")).value == "false")
        {
            if (FindControl("txtNfReferenciada", "input") != null)
            {
                FindControl("hdfNfReferenciada", "input").value = "";
                FindControl("txtNfReferenciada", "input").value = "";
                FindControl("txtNfReferenciada", "input").style.display = "none";
                FindControl("lblNfReferenciada", "span").style.display = "none";
                FindControl("lblNotaFiscalConsumidorReferenciada", "span").style.display = "none";
                FindControl("imbOpenNfReferenciada", "input").style.display = "none";
            }
        }
        else if(idCfop == "5929")
        {
            if (FindControl("txtNfReferenciada", "input") != null)
            {
                FindControl("txtNfReferenciada", "input").style.display = "";
                FindControl("lblNfReferenciada", "span").style.display = "none";
                FindControl("lblNotaFiscalConsumidorReferenciada", "span").style.display = "";
                FindControl("imbOpenNfReferenciada", "input").style.display = "";
            }
        }
        else
        {
            if (FindControl("txtNfReferenciada", "input") != null)
            {
                FindControl("txtNfReferenciada", "input").style.display = "";
                FindControl("lblNfReferenciada", "span").style.display = "";
                FindControl("imbOpenNfReferenciada", "input").style.display = "";
            }
        }

        cfopDevolucao = retorno;
    }

    function isCfopDevolucao() {
        return cfopDevolucao;
    }

    function selLojaDest(tipoDoc) {
        return tipoDoc == 3;
    }

    function selClienteDest(tipoDoc) {
        return (tipoDoc == 2 && !isCfopDevolucao()) || (tipoDoc == 1 && isCfopDevolucao()) || tipoDoc == 4;
    }

    function selFornecDest(tipoDoc) {
        return (tipoDoc == 1 && !isCfopDevolucao()) || (tipoDoc == 2 && isCfopDevolucao());
    }

    function limparDestinatario() {
        var tipoDoc = FindControl("drpTipoDocumento", "select").value;

        FindControl("txtRazaoDest", "input").value = "";
        FindControl("txtCnpjDest", "input").value = "";
        FindControl("txtCodDest", "input").value = "";

        if (selClienteDest(tipoDoc)) // Saída/NFe cliente - Seleciona cliente
            FindControl("hdfIdCliente", "input").value = "";
        else if (selFornecDest(tipoDoc)) // Entrada (ou devolução) - Seleciona fornecedor
            FindControl("hdfIdFornec", "input").value = "";
        else if (selLojaDest(tipoDoc))
            FindControl("hdfIdLoja", "input").value = "";

        PodeConsSitCadContr();
    }

    function selLojaEmit(tipoDoc) {
        return tipoDoc != 3 && tipoDoc != 4;
    }

    function selClienteEmit(tipoDoc) {
        return isCfopDevolucao() && tipoDoc == 3;
    }

    function selFornecEmit(tipoDoc) {
        return !isCfopDevolucao() || tipoDoc == 4;
    }

    function limparRemetente() {
        var tipoDoc = FindControl("drpTipoDocumento", "select").value;

        // Não pode modificar o remetente se for nota de saída, pois o remetente será sempre a loja
        if (tipoDoc != 2)
        {
            FindControl("txtRazaoEmit", "input").value = "";
            FindControl("txtCnpjEmit", "input").value = "";
            FindControl("txtCodEmit", "input").value = "";

            if (selLojaEmit(tipoDoc))
                FindControl("hdfIdLoja", "input").value = "";
            else if (selFornecEmit(tipoDoc))
                FindControl("hdfIdFornec", "input").value = "";
            else if (selClienteEmit(tipoDoc))
                FindControl("hdfIdCliente", "input").value = "";
        }
    }

    function openDestinatario() {
        var tipoDoc = FindControl("drpTipoDocumento", "select").value;

        if (selClienteDest(tipoDoc)) // Saída/NFe cliente - Seleciona cliente
            openWindow(500, 700, '../Utils/SelCliente.aspx?Nfe=1');
        else if (selFornecDest(tipoDoc)) // Entrada (ou devolução) - Seleciona fornecedor
            openWindow(500, 700, '../Utils/SelFornec.aspx?Nfe=1');
        else if (tipoDoc == 3)
            openWindow(500, 700, '../Utils/SelLoja.aspx?Nfe=2');
    }

    function abrirEmitente() {
        var tipoDoc = FindControl("drpTipoDocumento", "select").value;

        if (selLojaEmit(tipoDoc))
            openWindow(600, 700, '../Utils/SelLoja.aspx?Nfe=1');
        else if (selFornecEmit(tipoDoc))
            openWindow(600, 1050, '../Utils/SelFornec.aspx?Nfe=2');
        else if (selClienteEmit(tipoDoc))
            openWindow(600, 1050, '../Utils/SelCliente.aspx?Nfe=2');
    }

    function getDestinatario(control) {
        var cod = control.value;
        var tipoDoc = FindControl("drpTipoDocumento", "select").value;

        if (selClienteDest(tipoDoc)) {
            var dadosCli = MetodosAjax.GetDadosCli(cod);

            if (dadosCli.error != null) {
                alert(dadosCli.error.description);
                return;
            }

            var dados = dadosCli.value.split('|');

            setClienteNfe(cod, dados[0], dados[10], '', dados[11]);
        }
        else if (selFornecDest(tipoDoc)) {
            var dadosFornec = MetodosAjax.GetDadosFornec(cod);

            if (dadosFornec.error != null) {
                alert(dadosFornec.error.description);
                return;
            }

            var dados = dadosFornec.value.split('|');

            setFornecNfe(cod, dados[0], dados[1]);
        }
        else if (selLojaDest(tipoDoc)) {
            var dadosLoja = MetodosAjax.GetDadosLoja(cod);

            if (dadosLoja.error != null) {
                alert(dadosLoja.error.description);
                return;
            }

            var dados = dadosLoja.value.split('|');

            setLojaDest(cod, dados[0], dados[1]);
        }
    }

    function getRemetente(control) {

        var cod = control.value;
        var tipoDoc = FindControl("drpTipoDocumento", "select").value;

        if (selLojaEmit(tipoDoc)) {

            var dadosLoja = MetodosAjax.GetDadosLoja(cod);

            if (dadosLoja.error != null) {
                alert(dadosLoja.error.description);
                return;
            }

            var dados = dadosLoja.value.split('|');

            setLojaNfe(cod, dados[0], dados[1]);

        }
        else if (selFornecEmit(tipoDoc)) {
            var dadosFornec = MetodosAjax.GetDadosFornec(cod);

            if (dadosFornec.error != null) {
                alert(dadosFornec.error.description);
                return;
            }

            var dados = dadosFornec.value.split('|');

            setFornecEmit(cod, dados[0], dados[1]);
        }
        else if (selClienteEmit(tipoDoc)) {
            var dadosCli = MetodosAjax.GetDadosCli(cod);

            if (dadosCli.error != null) {
                alert(dadosCli.error.description);
                return;
            }

            var dados = dadosCli.value.split('|');

            setClienteEmit(cod, dados[0], dados[10], '', dados[11]);
        }
    }

    // Seta informações da Cidade selecionada no popup
    function setCidade(idCidade, nomeCidade) {
        FindControl('hdfCidade', 'input').value = idCidade;
        FindControl('txtCidade', 'input').value = nomeCidade;
    }

    // Seta informações da Loja selecionada no popup
    function setLojaNfe(idLoja, razaoSocial, cnpj) {
        FindControl("txtRazaoEmit", "input").value = razaoSocial;
        FindControl("txtCnpjEmit", "input").value = cnpj;
        FindControl("hdfIdLoja", "input").value = idLoja;
    }

    // Seta informações do Emitente selecionado no popup
    function setFornecEmit(idFornec, razaoSocial, cnpj, idConta) {

        var retorno = CadNotaFiscal.VigenciaPrecoFornec(idFornec).value.split(';');

        if(retorno[0]=="Erro"){
            alert(retorno[1]);
            return false;
        }

        FindControl("txtRazaoEmit", "input").value = razaoSocial;
        FindControl("txtCnpjEmit", "input").value = cnpj;
        FindControl("hdfIdFornec", "input").value = idFornec;
        if (idConta > 0 && FindControl("drpPlanoContas", "select")) FindControl("drpPlanoContas", "select").value = idConta;
        PodeConsSitCadContr();
    }

    // Seta informações do Emitente selecionado no popup
    function setClienteEmit(idCliente, razaoSocial, cnpj) {
        FindControl("txtRazaoEmit", "input").value = razaoSocial;
        FindControl("txtCnpjEmit", "input").value = cnpj;
        FindControl("hdfIdCliente", "input").value = idCliente;
        PodeConsSitCadContr();
    }

    // Seta informações do Cliente selecionado no popup
    function setClienteNfe(idCliente, razaoSocial, cpfCnpj, suframa, obsNfe) {
        FindControl("txtRazaoDest", "input").value = razaoSocial;
        FindControl("txtCnpjDest", "input").value = cpfCnpj;
        FindControl("hdfIdCliente", "input").value = idCliente;

        var tipo = GetQueryString("tipo");
        var mod = GetQueryString("mod");
        var idNf = GetQueryString("idNf");

        if(idNf == undefined)
            idNf = GetQueryString("IdNf");

        tipo = tipo == undefined ? "" : tipo;
        mod = mod == undefined ? "" : mod;
        idNf = idNf == undefined ? "" : idNf;

        var habilitarCpfCnpj = CadNotaFiscal.HabilitarCpfCnpj(idCliente, tipo, mod, idNf);

        if(habilitarCpfCnpj.error != null){
            alert(habilitarCpfCnpj.erro.description);
            return false;
        }

        FindControl("txtCnpjDest", "input").disabled = !habilitarCpfCnpj.value;

        var suframaDisplay = suframa != null && suframa != "" ? "inline" : "none";
        FindControl("lblSuframa", "span").style.display = suframaDisplay;
        FindControl("txtSuframa", "input").style.display = suframaDisplay;
        FindControl("txtSuframa", "input").value = suframa;

        if (obsNfe)
        {
            if (FindControl("txtInfCmpl", "textarea").value.indexOf(obsNfe) == -1)
                FindControl("txtInfCmpl", "textarea").value += obsNfe;
        }

        var temTransportador = CadNotaFiscal.ClienteTemTransportador(idCliente);

        if(temTransportador.error != null){
            alert(temTransportador.erro.description);
            return false;
        }

        if(temTransportador.value == "true"){
            FindControl("drpFrete", "select").value = "2";
        }


        PodeConsSitCadContr();
    }

    // Seta informações do Fornecedor selecionado no popup
    function setFornecNfe(idFornec, razaoSocial, cpfCnpj) {
        FindControl("txtRazaoDest", "input").value = razaoSocial;
        FindControl("txtCnpjDest", "input").value = cpfCnpj;
        FindControl("hdfIdFornec", "input").value = idFornec;
        PodeConsSitCadContr();
    }

    // Atribui as informações da loja ao destinatário
    function setLojaDest(idLoja, razaoSocial, cpfCnpj) {
        FindControl("txtRazaoDest", "input").value = razaoSocial;
        FindControl("txtCnpjDest", "input").value = cpfCnpj;
        FindControl("hdfIdLoja", "input").value = idLoja;
    }

    function tipoPagtoChange(control) {
        if (control == null)
            return;

        exibeParcelas();
    }

    function exibeParcelas() {
        var drpTipoPagto = FindControl("drpFormaPagto", "select");
        var txtNumParc = FindControl("txtNumParc", "input");
        var lblNumParc = FindControl("lblNumParc", "span");
        var maxNumParc = FindControl("hdfMaxNumParc", "input");

        if (drpTipoPagto == null)
            return;

        // Esconde o controle de pagamento se o tipo de pagamento for à vista, antecipação ou se o número de parcelas for superior
        var exibirParcelas = drpTipoPagto.value >= 2 && drpTipoPagto.value < 12 && parseInt(txtNumParc.value, 10) <= parseInt(maxNumParc.value, 10);

        if (lblNumParc != null) {
            FindControl("hdfExibirParcelas", "input").value = exibirParcelas;
            lblNumParc.parentNode.style.display = drpTipoPagto.value >= 2 && drpTipoPagto.value < 12 ? "inline" : "none";
            txtNumParc.parentNode.style.display = drpTipoPagto.value >= 2 && drpTipoPagto.value < 12 ? "inline" : "none";
            Parc_visibilidadeParcelas("<%= dtvNf.ClientID %>_ctrlParcelas1");
        }

        // Exibe campos de parcelas automáticas se quantidade de parcelas for alto
        if (FindControl("tbParcAut", "table") != null)
            FindControl("tbParcAut", "table").style.display = (drpTipoPagto.value >= 2 && exibirParcelas) || drpTipoPagto.value == 12 || drpTipoPagto.value == 1 ? "none" : "";

        // Exibe o campo de fatura, se existir
        var fatura = FindControl("_fatura", "span");
        if (fatura != null)
        {
            fatura.style.display = drpTipoPagto.value >= 2 && drpTipoPagto.value < 12 ? "" : "none";

            var tipoFatura = FindControl("drpFatura", "select");
            if (drpTipoPagto.value == 1)
                tipoFatura.value = "";

            alteraFatura(tipoFatura.value);
        }
    }

    function alteraFatura(tipoFatura)
    {
        var outros = <%= ((int)Glass.Data.Model.NotaFiscal.TipoFaturaEnum.Outros).ToString() %>;
        var descr = FindControl("txtDescrFatura", "input");
        FindControl("lblDescrFatura", "span").style.display = tipoFatura == outros ? "" : "none";
        descr.style.display = tipoFatura == outros ? "" : "none";
        descr.value = tipoFatura != outros ? "" : descr.value;
    }

    var clicked = false;

    function onInsertUpdate() {

        if (clicked)
            return false;

        clicked = true;

        return true;
    }

    function validarDataHora()
    {
        //pega as datas de saida/entrada e emissao
        var dataSaida = FindControl("ctrlDataSaida", "input").value;
        var dataEmissao = FindControl("ctrlDataEmissao", "input").value;

        //Separa as datas em um array
        var dataSaidaSeparada = dataSaida != "" && dataSaida != null ? dataSaida.split('/') : null;
        var dataEmissaoSeparada = dataEmissao != "" && dataEmissao != null ? dataEmissao.split('/') : null;

        //pega as horas de saida/entrada e emissão
        var horaSaida = FindControl("ctrlDataSaida_txtHora", "input").value;
        var horaEmissao = FindControl("ctrlDataEmissao_txtHora", "input").value;

        //Separa as horas em um array
        var horaSaidaSeparada = horaSaida != "" && horaSaida != null ? horaSaida.split(':') : null;
        var horaEmissaoSeparada = horaEmissao != "" && horaEmissao != null ? horaEmissao.split(':') : null;

        //Valida se as datas tem valor
        if (dataSaidaSeparada != null && dataEmissaoSeparada != null) {
            //Converte a data de um array de string para uma Data
            var dataDeSaida = new Date(dataSaidaSeparada[2], dataSaidaSeparada[1] - 1, dataSaidaSeparada[0], horaSaidaSeparada != null ? horaSaidaSeparada[0] : 0, horaSaidaSeparada != null ? horaSaidaSeparada[1] : 0);
            var dataDeEmissao = new Date(dataEmissaoSeparada[2], dataEmissaoSeparada[1] - 1, dataEmissaoSeparada[0], horaEmissaoSeparada != null ? horaEmissaoSeparada[0] : 0, horaEmissaoSeparada != null? horaEmissaoSeparada[1] : 0);

            if (dataDeSaida < dataDeEmissao) {
                alert("Data de saída/entrada não pode ser inferior à data de emissão");
                return false;
            }
        }
        return true;
    }

    // Validações realizadas ao inserir NF
    function onSave()
    {
        // try...catch() necessário para caso ocorra algum erro de javascript durante a validação,
        // o usuário não consiga salvar as alterações feitas na nota
        try
        {
            if (!validarDataHora()) {
                clicked = false;
                return false;
            }

            if (FindControl("txtInfCmpl", "textarea") != null && FindControl("txtInfCmpl", "textarea").value != "")
                FindControl("txtInfCmpl", "textarea").value = removeCaractereEspecial(FindControl("txtInfCmpl", "textarea").value);

            // Verifica se a chave de acesso está correta
            if (FindControl("valChaveAcesso", "span") != null && FindControl("valChaveAcesso", "span").style.visibility == "visible") {
                alert("Chave de acesso inválida.");
                clicked = false;
                return false;
            }

            var tipoDoc = FindControl("drpTipoDocumento", "select").value;

            // Verifica se o modelo da nota foi selecionado
            if (tipoDoc == 3 && FindControl("txtModelo", "input").value == "") {
                alert("Informe o modelo desta nota.");
                clicked = false;
                return false;
            }

            // Verifica se a série foi informada
            if (((tipoDoc == 3 && FindControl("txtModelo", "input").value != "22") || tipoDoc != 3) && FindControl("txtSerie", "input").value == "") {
                alert("Informe a série da nota fiscal.");
                clicked = false;
                return false;
            }

            // Verifica se a forma de pagamento foi informada
            if (FindControl("drpFormaPagto", "select").value == "") {

                alert("Informe a forma de pagamento.");
                clicked = false;
                return false;
            }
            else
            {
                if(FindControl("drpFormaPagto", "select").value == "12" &&
                    FindControl("hdfIdAntecipFornec", "input").value == ""){
                    alert("Informe a antecipação.");
                    clicked = false;
                    return false;
                }

                var valoresRecebidos = FindControl("ctrlFormaPagto_hdfValoreReceb", "input").value.split(';');
                if(FindControl("drpFormaPagto", "select").value != "12"){
                    for(var i=0; i < valoresRecebidos.length; i++){
                        if(valoresRecebidos[i] == ""){
                            alert("Informe os valores da forma de pagamento.");
                            clicked = false;
                            return false;
                        }
                    }
                }

                var formasPagamento = FindControl("ctrlFormaPagto_hdfFormaPagto", "input").value.split(';');
                if(FindControl("drpFormaPagto", "select").value == "1"){
                    for(var i=0; i < valoresRecebidos.length; i++){
                        if(formasPagamento[i] == 14 || formasPagamento[i] == 15){
                            alert("Não é possível selecionar as formas de pagamento 'Boleto' ou 'Duplicata' à vista.");
                            clicked = false;
                            return false;
                        }
                    }
                }
            }

            // Verifica se o emitente foi selecionado
            if (FindControl("hdfIdLoja", "input").value == "" && FindControl("hdfIdFornec", "input").value == "") {
                alert("Informe o emitente.");
                clicked = false;
                return false;
            }

            // Verifica se o tipo do documento foi informado
            if (tipoDoc == "") {
                alert("Informe o tipo do documento");
                clicked = false;
                return false;
            }

            // Verifica se o cliente/fornecedor foi informado, deve verificar os dois ao mesmo tempo, pois caso a nota seja de saída,
            // pode ser necessário informar o cliente ou fornecedor, caso seja devolução por exemplo
            if (FindControl("hdfIdCliente", "input").value == "" && FindControl("hdfIdFornec", "input").value == "") {
                alert("Informe o destinatário/remetente.");
                clicked = false;
                return false;
            }

            // Verifica se o frete foi informado
            if (FindControl("drpFrete", "select").value == "") {
                alert("Informe a modalidade do frete.");
                clicked = false;
                return false;
            }

            var idCfop = FindControl("ctl00_ctl00_Pagina_Conteudo_dtvNf_ctrlNaturezaOperacaoNf_selNaturezaOperacao_txtDescr", "input").value;

            var validar = validarCfop(idCfop);

            if (validar == "false") {
                alert("A Natureza da Operação selecionada não pode ser usada nesse tipo de nota")
                FindControl("ctl00_ctl00_Pagina_Conteudo_dtvNf_ctrlNaturezaOperacaoNf_selNaturezaOperacao_txtDescr", "input").value = "";
                return false;
            }
        }
        catch(err)
        {
            alert("Falha ao validar nota fiscal. " + err.message);
            clicked = false;
            return false;
        }
    }

    //Remove os caracteres especiais da string
    function removeCaractereEspecial(texto){
        while(texto.indexOf("&#39;") != -1){
            texto = texto.replace("&#39;", "");
        }

        return texto;
    }

    // Carrega dados do produto com base no código do produto passado
    function selProduto(nomeControle, idProd) {
        if (idProd == "")
            return false;

        try {
            var idNf = FindControl("hdfIdNf", "input").value;

            var retorno = CadNotaFiscal.GetProduto(idProd, FindControl("hdfTipoEntrega", "input").value,
                FindControl("hdfIdCliente", "input").value, FindControl("hdfIdFornec", "input").value, idNf).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }

            var finalidade = FindControl("hdfFinalidade", "input");
            if (finalidade != null) finalidade = finalidade.value;

            FindControl("hdfIdProd", "input").value = idProd;
            FindControl("txtValorIns", "input").value = finalidade == 1 || finalidade == null ? retorno[1] : 0;
            FindControl("hdfValMin", "input").value = finalidade == 1 || finalidade == null ? retorno[1] : 0; // Armazena o valor mínimo
            FindControl("hdfIsVidro", "input").value = retorno[2]; // Informa se o produto é vidro
            FindControl("hdfIsAluminio", "input").value = retorno[3]; // Informa se o produto é vidro
            FindControl("hdfM2Minimo", "input").value = retorno[4]; // Informa se o produto possui m² mínimo

            if(FindControl("txtAlturaIns", "input") != null && FindControl("txtAlturaIns", "input").value != "") {
                GetAdicionalAlturaChapa();
            }

            var tipoCalc = eval(nomeControle).DadosProduto().TipoCalculo;

            if (retorno[5] != "" && FindControl("drpCst", "select") != null)
            {
                FindControl("drpCst", "select").value = retorno[5];
                obterCodValorFiscalPorCst(FindControl("drpCst", "select"));
            }

            if (retorno[19] != "" && FindControl("drpOrigCst", "select") != null)
                FindControl("drpOrigCst", "select").value = retorno[19];

            if (FindControl("drpCsosn", "select") != null)
            {
                if (retorno[6] != "")
                    FindControl("drpCsosn", "select").value = retorno[6];

                if (FindControl("ddlCodValorFiscal", "select") != null)
                    FindControl("ddlCodValorFiscal", "select").value = "3";
            }

            FindControl("txtAliqIcms", "input").value = retorno[7];
            FindControl("txtAliqIpi", "input").value = retorno[9];

            if (FindControl("txtAliqIcmsSt", "input") != null)
                FindControl("txtAliqIcmsSt", "input").value = retorno[15];

            // FCP
            if(FindControl("txtAliqFcp", "input") != null)
                FindControl("txtAliqFcp", "input").value = retorno[8];

            if(FindControl("txtAliqFcpSt", "input") != null)
                FindControl("txtAliqFcpSt", "input").value = retorno[20];

            var controle = FindControl("ctrlNaturezaOperacaoProd_selNaturezaOperacao_txtDescr", "input");
            if (controle != null)
            {
                controle.value = retorno[10];
                controle.onblur();
            }

            if (FindControl("txtNcm", "input") != null)
                FindControl("txtNcm", "input").value = retorno[11];

            if (FindControl("txtMva", "input") != null)
                FindControl("txtMva", "input").value = retorno[12];

            if (retorno[16] != "" && FindControl("selCstIpi_hdfValor", "input") != null)
            {
                FindControl("selCstIpi_txtDescr", "input").value = retorno[16];
                FindControl("selCstIpi_txtDescr", "input").onblur();
            }

            if (retorno[17] != "" && FindControl("drpContaContabil", "select") != null)
                FindControl("drpContaContabil", "select").value = retorno[17];

            // Define se a opção de mostrar a qtd tributária será mostrada
            var mostrarQtdeTrib = retorno[18] == "true";
            FindControl("txtQtdeTrib", "input").style.display = mostrarQtdeTrib ? "inline" : "none";
            FindControl("lblQtdeTrib", "span").style.display = mostrarQtdeTrib ? "inline" : "none";
            if (!mostrarQtdeTrib) FindControl("txtQtdeTrib", "input").value = "";

            // Se o produto não for vidro, desabilita os textboxes largura e altura,
            // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
            // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
            var cAltura = FindControl("txtAlturaIns", "input");
            var cLargura = FindControl("txtLarguraIns", "input");
            cAltura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
            cLargura.disabled = CalcProd_DesabilitarLargura(tipoCalc);
            cAltura.value = "";
            cLargura.value = "";
            FindControl("txtTotM2", "input").disabled = tipoCalc != 2 && tipoCalc != 3 && tipoCalc != 10 && retorno[2] != "true";

            var funcaoNumeros = "return soNumeros(event, " + (tipoCalc != 5).toString().toLowerCase() + ", true);";
            FindControl("txtQtde", "input").setAttribute("OnKeyPress", funcaoNumeros);

            if (retorno[13] > 0 && retorno[14] > 0)
            {
                cAltura.value = retorno[13];
                cLargura.value = retorno[14];
            }

            // Limpa os controles abaixo somente se o FCI estiver habilitado
            if (FindControl("txtParcelaImportada", "input") != null)
            {
                FindControl("txtParcelaImportada", "input").value = "";
                FindControl("txtSaidaInterestadual", "input").value = "";
                FindControl("txtConteudoImportacao", "input").value = "";
                FindControl("txtNumControleFci", "input").value = "";
            }
        }
        catch (err) {
            alert(err);
        }
    }

    function callbackNatOp(nomeControle, id) {
        if (FindControl("drpCst", "select") != null)
            obterCodValorFiscalPorCst(FindControl("drpCst", "select"));

        var idNf = FindControl("hdfIdNf", "input") != null ? FindControl("hdfIdNf", "input").value : 0;
        var idCfop = FindControl(nomeControle +"_selNaturezaOperacao_txtDescr", "input").value;

        var validar = validarCfop(idCfop);

        if (validar == "false") {
            alert("A Natureza da Operação selecionada não pode ser usada nesse tipo de nota")
            FindControl(nomeControle +"_selNaturezaOperacao_txtDescr", "input").value = "";
        }

        mensagemAlertaNaturezaOperacao();
    }

    var botaoInserirProdutoClicado = false;

    // Chamado quando um produto está para ser inserido no pedido
    function onSaveProd() {
        if (!botaoInserirProdutoClicado)
            botaoInserirProdutoClicado = true;
        else
            return false;

        var codProd = FindControl("ctrlSelProd_ctrlSelProdBuscar_txtDescr", "input").value;
        var valor = FindControl("txtValorIns", "input").value;
        var qtde = FindControl("txtQtdeIns", "input").value;
        var altura = FindControl("txtAlturaIns", "input").value;
        var largura = FindControl("txtLarguraIns", "input").value;
        var valMin = FindControl("hdfValMin", "input").value;
        var finalidade = FindControl("hdfFinalidade", "input").value;

        var codValorFiscal = FindControl("ddlCodValorFiscal", "select").value;

        if (codProd == "") {
            alert("Informe o código do produto.");
            botaoInserirProdutoClicado = false;
            return false;
        }
        else if ((valor == "0" || valor == "") && finalidade == 1) {
            alert("Informe o valor vendido.");
            botaoInserirProdutoClicado = false;
            return false;
        }
        else if ((qtde == "0" || qtde == "") && finalidade == 1) {
            alert("Informe a quantidade.");
            botaoInserirProdutoClicado = false;
            return false;
        }
        else if (FindControl("txtAlturaIns", "input").disabled == false) {
            if (altura == "") {
                alert("Informe a altura.");
                botaoInserirProdutoClicado = false;
                return false;
            }
        }
        // Se o textbox da largura estiver habilitado, deverá ser informada
        else if (FindControl("txtLarguraIns", "input").disabled == false && largura == "") {
            alert("Informe a largura.");
            botaoInserirProdutoClicado = false;
            return false;
        }

        var usarFci = <%= UtilizaFCI().ToString().ToLower() %>;

        if(usarFci){
            var numControleFci = FindControl("txtNumControleFci", "input");
            if(!validaFci(numControleFci)){
                botaoInserirProdutoClicado = false;
                return false;
            }
        }

        codValorFiscalSimples();

        // Habilita os campos altura e largura para que caso o produto inserido seja um box, os campos altura e largura
        // devem estar desbloqueados no momento do postback para que os valores sejam enviados.
        FindControl("txtAlturaIns", "input").disabled = false;
        FindControl("txtLarguraIns", "input").disabled = false;

        return true;
    }

    // Função chamada quando o produto está para ser atualizado
    function onUpdateProd() {
        var valor = FindControl("txtValorIns", "input").value;
        var qtde = FindControl("txtQtdeIns", "input").value;
        var altura = FindControl("txtAlturaIns", "input").value;
        var idProd = FindControl("hdfIdProd", "input").value;
        var valMin = FindControl("hdfValMin", "input").value;
        var idNf = FindControl("hdfIdNf", "input").value;
        var finalidade = FindControl("hdfFinalidade", "input").value;
        var codValorFiscal = FindControl("ddlCodValorFiscal", "select").value;

        var retorno = CadNotaFiscal.GetProduto(idProd, FindControl("hdfTipoEntrega", "input").value, FindControl("hdfIdCliente", "input").value, FindControl("hdfIdFornec", "input").value, idNf).value.split(';');

        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            return false;
        }

        if ((valor == "0" || valor == "") && finalidade == 1) {
            alert("Informe o valor vendido.");
            return false;
        }
        else if ((qtde == "0" || qtde == "") && finalidade == 1) {
            alert("Informe a quantidade.");
            return false;
        }
        else if (FindControl("txtAlturaIns", "input").disabled == false) {
            if (altura == "") {
                alert("Informe a altura.");
                return false;
            }
        }

        var usarFci = <%= UtilizaFCI().ToString().ToLower() %>;

        if(usarFci){
            var numControleFci = FindControl("txtNumControleFci", "input");
            if(!validaFci(numControleFci))
                return false;
        }

        codValorFiscalSimples();

        // Habilita os campos altura e largura para que caso o produto inserido seja um box, os campos altura e largura
        // devem estar desbloqueados no momento do postback para que os valores sejam enviados.
        FindControl("txtAlturaIns", "input").disabled = false;
        FindControl("txtLarguraIns", "input").disabled = false;

        return true;
    }

    function GetAdicionalAlturaChapa(){
        var idProd = FindControl("hdfIdProd", "input").value;
        var altura = FindControl("txtAlturaIns", "input").value;
        var idCliente = FindControl("hdfIdFornec", "input").value;
        var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;

        var retorno = MetodosAjax.GetValorTabelaProduto(idProd, tipoEntrega, idCliente, false, false, 0, "", "", "", altura);

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
            valMin.value = retorno.value.replace(".", ",");
        }
        else{
            alert("Não foi possível encontrar o controle 'hdfValMin'");
            return false;
        }

        var valorIns = FindControl("txtValorIns", "input");

        if(valorIns != null){
            valorIns.value = retorno.value.replace(".", ",");
        }
        else{
            alert("Não foi possível encontrar o controle 'txtValorIns'");
            return false;
        }
    }

    // Calcula em tempo real a metragem quadrada do produto
    function calcM2Prod() {
        try {
            var altura = FindControl("txtAlturaIns", "input").value;
            var largura = FindControl("txtLarguraIns", "input").value;
            var qtde = FindControl("txtQtdeIns", "input").value;
            var isVidro = FindControl("hdfIsVidro", "input").value == "true";
            var tipoCalc = FindControl("hdfTipoCalc", "input").value;

            if (altura == "" || largura == "" || qtde == "" || altura == "0" || largura == "0") {
                if (qtde != "" && qtde != "0")
                    calcTotalProd();

                return false;
            }

            FindControl("txtTotM2", "input").value = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, 0, false, 0, false).value;

            calcTotalProd();
        }
        catch (err) {

        }
    }

    // Calcula em tempo real o valor total do produto
    function calcTotalProd() {
        try {
            var valorIns = FindControl("txtValorIns", "input").value;
            var totM2 = new Number(FindControl("txtTotM2", "input").value.replace(',', '.')).toFixed(6);
            var total = new Number(valorIns.replace(',', '.')).toFixed(6);
            var qtde = new Number(FindControl("txtQtdeIns", "input").value.replace(',', '.'));
            var altura = new Number(FindControl("txtAlturaIns", "input").value.replace(',', '.'));
            var largura = new Number(FindControl("txtLarguraIns", "input").value.replace(',', '.'));
            var tipoCalc = FindControl("hdfTipoCalc", "input").value;
            var m2Minimo = FindControl("hdfM2Minimo", "input").value;
            var totM2Calc = totM2;

            var calculo = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, FindControl("txtAlturaIns", "input"),
                largura, 2, tipoCalc, 2, 2);

            if (!manual)
                FindControl("lblTotalIns", "span").innerHTML = calculo;
            else
                FindControl("txtTotalIns", "input").value = calculo.replace("R$", "").replace(" ", "");

            calcValorPis(true);
            calcValorCofins(true);

            FindControl("hdfTotalIns", "input").value = calculo.replace("R$", "").replace(" ", "");
        }
        catch (err) {

        }
    }

    // Se a nota for série U ou a finalidade for NF-e Complementar, abre campos de impostos e do total da nota para edição,
    // e esconde os produtos
    function habilitaTxtImpostos(alterarSerie) {
        if (FindControl("txtSerie", "input") == null)
            return;

        var lblBcIcmsSt = FindControl("lblBcIcmsSt", "span");
        var lblValorIcmsSt = FindControl("lblValorIcmsSt", "span");
        var lblValorIpi = FindControl("lblValorIpi", "span");
        var lblTotalProd = FindControl("lblTotalProd", "span");
        var txtBCIcmsSt = FindControl("txtBCIcmsSt", "input");
        var txtValorIcmsSt = FindControl("txtValorIcmsSt", "input");
        var txtValorIpi = FindControl("txtValorIpi", "input");
        var txtTotalProd = FindControl("txtTotalProd", "input");
        var txtTotalNota = FindControl("txtTotalNota", "input");
        var txtBCIcms = FindControl("txtBCIcms", "input");
        var txtValorIcms = FindControl("txtValorIcms", "input");
        var txtValorPis = FindControl("txtValorPis", "input");
        var txtValorCofins = FindControl("txtValorCofins", "input");

        // Se for nota de série "U"
        if (FindControl("txtSerie", "input").value == "U") {
            if (lblBcIcmsSt != null) {
                //lblBcIcmsSt.style.display = "none";
                //lblValorIcmsSt.style.display = "none";
                lblValorIpi.style.display = "none";
                //txtBCIcmsSt.style.display = "none";
                //txtValorIcmsSt.style.display = "none";
                txtValorIpi.style.display = "none";
                if (lblTotalProd != null) lblTotalProd.style.display = "none";
                if (txtTotalProd != null) txtTotalProd.style.display = "none";
            }

            txtTotalNota.disabled = false;
            txtBCIcms.disabled = false;
            txtValorIcms.disabled = false;
            txtBCIcmsSt.disabled = false;
            txtValorIcmsSt.disabled = false;
            txtValorPis.disabled = false;
            txtValorCofins.disabled = false;
        }
        else {
            if (alterarSerie && inserindo)
                FindControl("txtSerie", "input").value = serieNf;

            if (lblBcIcmsSt != null) {
                lblBcIcmsSt.style.display = "inline";
                lblValorIcmsSt.style.display = "inline";
                lblValorIpi.style.display = "inline";
                txtBCIcmsSt.style.display = "inline";
                txtValorIcmsSt.style.display = "inline";
                txtValorIpi.style.display = "inline";
                if (lblTotalProd != null) lblTotalProd.style.display = "inline";
                if (txtTotalProd != null) txtTotalProd.style.display = "inline";
            }

            txtTotalNota.disabled = true;
            txtBCIcms.disabled = true;
            txtValorIcms.disabled = true;
            txtBCIcmsSt.disabled = true;
            txtValorIcmsSt.disabled = true;
            txtValorPis.disabled = !isNfEntradaTerceiros;
            txtValorCofins.disabled = !isNfEntradaTerceiros;
        }

        // Libera campos se for NF-e complementar/ajuste ou se for ajuste manual
        var finalidade = FindControl("hdfFinalidade", "input");
        var chkCompl = FindControl("chkCompl", "input");
        if ((finalidade != null && (finalidade.value == 2 || finalidade.value == 3)) || (chkCompl != null && chkCompl.checked) || manual) {
            if (FindControl("txtSerie", "input").value == "")
                FindControl("txtSerie", "input").value = serieNf;

            if (lblBcIcmsSt != null) {
                lblBcIcmsSt.style.display = "inline";
                lblValorIcmsSt.style.display = "inline";
                lblValorIpi.style.display = "inline";
                txtBCIcmsSt.style.display = "inline";
                txtValorIcmsSt.style.display = "inline";
                txtValorIpi.style.display = "inline";
                if (lblTotalProd != null) lblTotalProd.style.display = "inline";
                if (txtTotalProd != null) txtTotalProd.style.display = "inline";
            }

            txtTotalNota.disabled = false;
            txtBCIcms.disabled = false;
            txtValorIcms.disabled = false;
            txtBCIcmsSt.disabled = false;
            txtValorIcmsSt.disabled = false;
            txtValorIpi.disabled = false;
            txtValorPis.disabled = false;
            txtValorCofins.disabled = false;
            if (txtTotalProd != null && (manual || finalidade.value == 2 || finalidade.value == 3))
                txtTotalProd.disabled = false;
        }
    }

    // Função chamada ao clicar na check de nf complementar
    function chkComplClick(chkCompl, alterarOutros) {
        if (chkCompl == null)
            return false;

        if (alterarOutros)
        {
            var chkServico = FindControl("chkServico", "input");
            chkServico.checked = false;
            chkServicoClick(chkServico, false);
        }

        habilitaTxtImpostos(false);
    }

    function chkServicoClick(chkServico, alterarOutros)
    {
        if (chkServico == null)
            return false;

        if (alterarOutros)
        {
            var chkCompl = FindControl("chkCompl", "input");
            chkCompl.checked = false;
            chkComplClick(chkCompl, false);
        }

        FindControl("lblValorISS", "span").style.display = chkServico.checked ? "" : "none";
        FindControl("txtValorISS", "input").style.display = chkServico.checked ? "" : "none";
    }

    function serieOnChange(serie) {
        if (manual) return;
        if (serie == "U")
            habilitaTxtImpostos(true);
    }

    // Se cst escolhido for 20, mostra campo para informar o percentual de redução da base de cálculo
    function drpCst_Changed(atualizarOutros) {
        var drpCst = FindControl("drpCst", "select");

        if (drpCst == null)
            return;

        atualizarOutros = atualizarOutros == false ? false : true;
        if (atualizarOutros)
            obterCodValorFiscalPorCst(drpCst);

        var codValorFiscal = FindControl("ddlCodValorFiscal", "select").value;

        // Define se será exibido a table com informações adicionais de CST (Redução na BC, Desoneração de ICMS e Perc. de diferimento)
        var displayInfoCST = drpCst.value == "20" || drpCst.value == "30" || drpCst.value == "40" || drpCst.value == "41" || drpCst.value == "50" || drpCst.value == "51" || drpCst.value == "70" || drpCst.value == "90" ? "inline" : "none";
        document.getElementById("percRedIcms").style.display = displayInfoCST;

        // Define se será exibido o percentual de redução da BC
        var displayRedBC = drpCst.value == "20" || drpCst.value == "51" || (drpCst.value == "70" && codValorFiscal == "1") ? "inline" : "none";
        FindControl("txtPercRedBcIcms", "input").style.display = displayRedBC;
        FindControl("lblPercRedBcIcms", "span").style.display = displayRedBC;

        // Define se serão exibidos campos de icms desonerado
        var displayDeson = drpCst.value == "20" || drpCst.value == "30" || drpCst.value == "40" || drpCst.value == "41" || drpCst.value == "50" || drpCst.value == "70" || drpCst.value == "90" ? "inline" : "none";
        FindControl("txtValorIcmsDeson", "input").style.display = displayDeson;
        FindControl("lblValorIcmsDeson", "span").style.display = displayDeson;
        FindControl("lblMotivoIcmsDeson", "span").style.display = displayDeson;

        // Define se será exibido o motivo da desoneração do ICMS
        var displayMotivoDeson = drpCst.value == "20" || drpCst.value == "30" || drpCst.value == "40" || drpCst.value == "41" || drpCst.value == "50" || drpCst.value == "70" || drpCst.value == "90" ? "inline" : "none";
        FindControl("drpMotivoIcmsDeson", "select").style.display = displayMotivoDeson;

        // Define se serão exibidos campos de percentual de diferimento de ICMS
        var displayPercDiferimento = drpCst.value == "51" ? "inline" : "none";
        FindControl("lblPercDiferimento", "span").style.display = displayPercDiferimento;
        FindControl("txtPercDiferimento", "input").style.display = displayPercDiferimento;

        if (atualizarOutros)
            ddlCodValorFiscal_change(FindControl("ddlCodValorFiscal", "select"), false);
    }

    function infComplProd(idProdNf) {
        openWindow(200, 400, "../Utils/SetInfComplProdNf.aspx?idProdNf=" + idProdNf);
    }

    // Se CSOSN escolhido for o 900, mostra campo para informar o percentual de redução da base de cálculo.
    function drpCsosn_Changed() {
        var drpCsosn = FindControl("drpCsosn", "select");

        if (drpCsosn == null) {
            return;
        }
        var display = drpCsosn.value == "900" ? "inline" : "none";

        document.getElementById("csosnPercRedIcms").style.display = display;
        FindControl("txtCsosnPercRedBcIcms", "input").style.display = display;
        FindControl("lblCsosnPercRedBcIcms", "span").style.display = display;

        FindControl("txtCsosnPercRedBcIcmsSt", "input").style.display = display;
        FindControl("lblCsosnPercRedBcIcmsSt", "span").style.display = display;
    }

    function obterCodValorFiscalPorCst(drpCst){
        var codValorFiscal = FindControl("ddlCodValorFiscal", "select")
        var simplesNacional = FindControl("hdfSimplesNacional", "input").value;
        var tipoDocumento = FindControl("hdfTipoDocumento", "input").value;

        if(simplesNacional == "False")
        {
             if(drpCst.value == "00")
                codValorFiscal.value = "1";
             if(drpCst.value == "10")
                codValorFiscal.value = tipoDocumento != 2 ? "3" : "1";
             if(drpCst.value == "20")
                codValorFiscal.value = "1";
             if(drpCst.value == "30")
                codValorFiscal.value = "3";
             if(drpCst.value == "40")
                codValorFiscal.value = "2";
             if(drpCst.value == "41")
                codValorFiscal.value = "2";
             if(drpCst.value == "50")
                codValorFiscal.value = "3";
             if(drpCst.value == "51")
                codValorFiscal.value = "3";
             if(drpCst.value == "60")
                codValorFiscal.value = "3";
             if(drpCst.value == "70")
                codValorFiscal.value = tipoDocumento != 2 ? "3" : "1";
             if(drpCst.value == "90")
                codValorFiscal.value = tipoDocumento != 2 ? "3" : "1";
        }
        else {
            codValorFiscal.value = "3";
        }
    }

    function codValorFiscalSimples(){
        var codValorFiscal = FindControl("ddlCodValorFiscal", "select");
        var simplesNacional = FindControl("hdfSimplesNacional", "input").value == "True";
        var tipoDocumento = FindControl("hdfTipoDocumento", "input").value;

        if(simplesNacional && codValorFiscal.value != 3)
        {
            var msg = tipoDocumento == 3 ? "Esse fornecedor está dentro do Simples Nacional, portanto, o código do valor fiscal deverá ser 3.\nDeseja alterar o código ?" :
                "Essa loja está dentro do Simples Nacional, portanto, o código do valor fiscal deverá ser 3.\nDeseja alterar o código ?";
            if(confirm(msg))
            {
                codValorFiscal.value = "3";
            }
        }
    }

    function ddlCodValorFiscal_change(sender, atualizarOutros) {

        var tipoDocumento = FindControl("hdfTipoDocumento", "input").value;
        var drpCst = FindControl("drpCst", "select");
        var display = sender.value == "1" && drpCst.value == "70" ? "inline" : "none";

        FindControl("txtPercRedBcIcmsSt", "input").style.display = display;
        FindControl("lblPercRedBcIcmsSt", "span").style.display = display;

        atualizarOutros = atualizarOutros == false ? false : true;
        if (atualizarOutros)
            drpCst_Changed(false);
    }

    function PodeConsSitCadContr(){

        var idCli = FindControl("hdfIdCliente", "input").value;
        var idFornec = FindControl("hdfIdFornec", "input").value;

        if(idCli == "" && idFornec == ""){
            FindControl("ConsultaCadCliSintegra", "div").style.display = 'none';
            return false;
        }

        var podeConsultar;

        if(idCli != "")
            podeConsultar = CadNotaFiscal.PodeConsultarCadastro(idCli, "cliente").value;
        else
            podeConsultar = CadNotaFiscal.PodeConsultarCadastro(idFornec, "fornec").value;

        if (podeConsultar == "True")
            FindControl("ConsultaCadCliSintegra", "div").style.display = 'inline';
        else
            FindControl("ConsultaCadCliSintegra", "div").style.display = 'none';

        return false;


    }

    function formaPagtoChanged(formaPagto){

        var txtAntecip = FindControl("txtAntecip", "input");
        var imbBuscaAntecip = FindControl("imbBuscaAntecip", "input");

        if(formaPagto == 12)
        {
            txtAntecip.style.display = "";
            imbBuscaAntecip.style.display = "";

            var idAntecip = FindControl("hdfIdAntecipFornec", "input").value;
            var descrAntecip = CadNotaFiscal.GetDescrAntecipFornec(idAntecip).value;
            FindControl("txtAntecip", "input").value = descrAntecip;
        }
        else
        {
            FindControl("txtAntecip", "input").value = "";
            FindControl("hdfIdAntecipFornec", "input").value = "";

            imbBuscaAntecip.style.display = "none";
            txtAntecip.style.display = "none";
        }
    }

    function openSelAntecipFornec(){

        var idFornec = FindControl("hdfIdFornec","input").value;

        if (idFornec == "")
        {
            alert("Selecione um fornecedor antes de selecionar a antecipação.");
            return false;
        }

        openWindow(560, 650, "../Utils/SelAntecipFornec.aspx?situacao=4&idFornec=" + idFornec);
    }

    function setAntecipFornec(idAntecipFornec, descrAntecip){

        FindControl("hdfIdAntecipFornec", "input").value = idAntecipFornec;
        FindControl("txtAntecip", "input").value = descrAntecip;
    }

    function openSelNfReferenciada(numeros){

        var tipo = GetQueryString("tipo");
        var idCfop = FindControl("ctl00_ctl00_Pagina_Conteudo_dtvNf_ctrlNaturezaOperacaoNf_selNaturezaOperacao_txtDescr", "input").value;

        if(tipo != 1 && tipo != 2)
            return false;
        if (idCfop == "5929") {
            if(numeros != '')
                openWindow(560, 1000, "../Utils/SelNotaFiscalConsumidorReferenciada.aspx?numeros=" + numeros);
            else
                openWindow(560, 1000, "../Utils/SelNotaFiscalConsumidorReferenciada.aspx?");
        }
        else{
            if(numeros != '')
                openWindow(560, 1000, "../Utils/SelNotaFiscalReferenciada.aspx?numeros=" + numeros);
            else
                openWindow(560, 1000, "../Utils/SelNotaFiscalReferenciada.aspx?");
        }
    }

    function setNfReferenciada(idNf, numNf){
        var idCfop = FindControl("ctl00_ctl00_Pagina_Conteudo_dtvNf_ctrlNaturezaOperacaoNf_selNaturezaOperacao_txtDescr", "input").value;

        if (FindControl("txtNfReferenciada", "input") != null)
        {
            if(CadNotaFiscal.habilitarReferenciaNFe(idNf, idCfop, GetQueryString("tipo")).value == "true")
            {
                FindControl("hdfNfReferenciada", "input").value = idNf;
                FindControl("txtNfReferenciada", "input").value = numNf;
            }
            else
            {
                FindControl("hdfNfReferenciada", "input").value = "";
                FindControl("txtNfReferenciada", "input").value = "";
            }
        }
    }

    function validaFci(controle){

        var origCst = FindControl("drpOrigCst", "select");

        if(origCst == null || origCst.value == null){
            alert("Falha ao validar número de controle da FCI. A origem da mercadoria não foi encontada.");
            controle.value = "";
            return false;
        }

        if(origCst.value == 3 || origCst.value == 5 || origCst.value == 8){

//            if(<%= (TipoDocumentoNF() != 2).ToString().ToLower() %>){
//                if(controle == null || controle.value == ""){
//                    alert("O número de controle da FCI não foi informado.");
//                    controle.value = "";
//                    return false;
//                }
//            }

            if(controle != null && controle.value != "" && !isGUID(controle.value)){
                alert("O número de controle da FCI informado não é valido.");
                controle.value = "";
                return false;
            }

        }
        else if(controle != null && controle.value != ""){
            alert("O número de controle da FCI deve ser informado apenas quando a origem da mercadoria for 3, 5 ou 8.");
            controle.value = "";
            return false;
        }

        return true;
    }

    function buscarDadosFci(){

     if(<%= (TipoDocumentoNF() != 2 || !UtilizaFCI()).ToString().ToLower() %>)
        return false;

        var origCst = FindControl("drpOrigCst", "select");
        var txtParcelaImportada = FindControl("txtParcelaImportada", "input");
        var txtSaidaInterestadual = FindControl("txtSaidaInterestadual", "input");
        var txtConteudoImportacao = FindControl("txtConteudoImportacao", "input");
        var txtNumControleFci = FindControl("txtNumControleFci", "input");

        if(origCst == null || origCst.value == "" ||
            (origCst.value != 3 && origCst.value != 5 && origCst.value != 8)){

            txtParcelaImportada.value = "";
            txtSaidaInterestadual.value = "";
            txtConteudoImportacao.value = "";
            txtNumControleFci.value = "";

            return false;
        }

        var idProd = FindControl("hdfIdProd", "input");

        if(idProd == null || parseInt(idProd.value) == 0 || idProd.value == "")
            return false;

        var dadosFci = CadNotaFiscal.BuscaDadosFci(idProd.value, origCst.value);

        if(dadosFci.error != null){
            alert(dadosFci.error.description);
            return false;
        }

        if(!confirm('Buscar núm. de controle da FCI?'))
            return false;

        txtNumControleFci.value = dadosFci.value.split(';')[0];
        txtParcelaImportada.value = dadosFci.value.split(';')[1];
        txtSaidaInterestadual.value = dadosFci.value.split(';')[2];
        txtConteudoImportacao.value = dadosFci.value.split(';')[3];
    }

    function validaFciFinalizarEmitir(){

        if(<%= (!UtilizaFCI()).ToString().ToLower() %>)
            return true;

        var retorno = CadNotaFiscal.validaFciFinalizarEmitir(<%= Request["idNf"] %>);

        if(retorno.error != null){
            alert(retorno.error.description);
            return false;
        }

        if(retorno.value == "")
            return true;

        var dados = retorno.value.split(';');

        var msg = "É necessário informar FCI para o(s) produto(s):\n\n";

        for(var i = 0; i < dados.length ; i++)
            msg += dados[i] + "\n";

        msg += "\nDeseja prosseguir assim mesmo?";

        return confirm(msg);
    }

        function confirmarNaturezaOperacaoAlteraEstoqueFiscal(){
            var retornoValidacao = CadNotaFiscal.ValidaNaturezaOperacao(<%= Request["idNf"] %>);

            if(retornoValidacao.error != null){
                alert(retornoValidacao.error.description);
                return false;
            }

            if(retornoValidacao.value == "nao") {
                alert("Informe a natureza de operação da nota fiscal.");
                return false;
            }

            var retorno = CadNotaFiscal.VerificarNaturezaOperacaoAlteraEstoqueFiscal(<%= Request["idNf"] %>);

            if(retorno.error != null){
                alert(retorno.error.description);
                return false;
            }

            if(retorno.value == "nao")
                return confirm("A natureza de operação selecionada na nota fiscal não altera o estoque fiscal, deseja finalizá-la assim mesmo?");

            return true;
        }

        function setPlanoConta(idConta, descricao) {
            var planoConta = FindControl("drpPlanoContas", "select");

            if (planoConta == null)
                return false;

            planoConta.value = idConta;
        }

        function mensagemAlertaCST(){
            var drpCst = FindControl("drpCst", "select");
            var drpOrigCst = FindControl("drpOrigCst", "select");
            var imbAlerta = FindControl("imbAlerta", "img");

            if (drpCst == null || drpCst == undefined || drpCst.value == null || drpCst.value == "" || drpCst.value == undefined ||
                drpOrigCst == null || drpOrigCst == undefined || drpOrigCst.value == null || drpOrigCst.value == "" || drpOrigCst.value == undefined ||
                imbAlerta == null || imbAlerta == undefined) {
                if (imbAlerta == null || imbAlerta == undefined)
                    return false;

                imbAlerta.title = "";
                imbAlerta.hidden = true;
                return false;
            }

            var origCst = drpOrigCst.value;
            var cst = drpCst.value;

            if(cst != "00" && origCst + cst != "010" && origCst + cst != "060"){
                imbAlerta.title = "";
                imbAlerta.hidden = true;
                return false;
            }

            if (cst == "00"){
                imbAlerta.title = "Para o CST 00 deverá ser informado a alíquota de ICMS e alíquota de ICMS ST.";
                imbAlerta.hidden = false;
                return false;
            }
            else if (origCst + cst == "010"){
                imbAlerta.title = "Para o CST 010 deverá ser informado a alíquota de ICMS.";
                imbAlerta.hidden = false;
                return false;
            }
            else if (origCst + cst == "060"){
                imbAlerta.title = "Para o CST 060 não deverá ser informado a alíquota de ICMS.";
                imbAlerta.hidden = false;
                return false;
            }
        }

        function mensagemAlertaNaturezaOperacao(){
            var imbAlerta = FindControl("imbAlerta", "img");
            var codProd = FindControl("hdfIdProd", "input").value;

            // Recupera quais impostos a natureza de operação deve calcular
            var idNatureza = FindControl("ctrlNaturezaOperacaoProd_selNaturezaOperacao_hdfValor", "input").value;
            var retornoNat = CadNotaFiscal.ObterCalcularIcmsIpi(idNatureza).value.split(';');

            if (retornoNat[0] == "Erro") {
                alert(retornoNat[1]);
                return false;
            }

            // Recupera os dados do produto para verificar se os impostos necessários estão informados em seu cadastro.
            var idNf = FindControl("hdfIdNf", "input").value;
            var idProd = FindControl("hdfIdProd", "input").value;
            var retornoProd = CadNotaFiscal.GetProduto(idProd, FindControl("hdfTipoEntrega", "input").value,
                FindControl("hdfIdCliente", "input").value, FindControl("hdfIdFornec", "input").value, idNf).value.split(';');

            if (retornoProd[0] == "Erro") {
                alert(retornoProd[1]);
                return false;
            }
            // Recupera a mensagem de alerta do CST
            imbAlerta.title = "";
            mensagemAlertaCST();
            var mensagem = "";
            var hidden = imbAlerta.hidden;

            // Verifica se deve exibir mensagem de CFOP
            if(retornoNat[1] == "true")
                if(retornoProd[7] == "0"){
                    mensagem += "O produto " + codProd + " Não tem alíquota de ICMS informada em seu cadastro. ";
                    hidden = false;
                }

            if(retornoNat[2] == "true")
                if(retornoProd[14] == "0"){
                    mensagem += "O produto " + codProd + " Não tem alíquota de ICMSST informada em seu cadastro. ";
                    hidden = false;
                }

            if(retornoNat[3] == "true")
                if(retornoProd[8] == "0"){
                    mensagem += "O produto " + codProd + " Não tem alíquota de IPI informada em seu cadastro. ";
                    hidden = false;
                }

            // Exibe a mensagem caso necessário
            if(mensagem != ""){
                imbAlerta.title += mensagem;
                imbAlerta.hidden = hidden;
                return false;
            }
        }

        var emitirNFeClicked = false;

        function onEmitirNFe() {

            if (emitirNFeClicked)
                return false;

            emitirNFeClicked = true;

            if(!confirm('Tem certeza que deseja emitir esta Nota Fiscal?') || !validaFciFinalizarEmitir()){
                emitirNFeClicked = false;
                return false;
            }

            return true;
        }

        function alertaGerarEtiquetaEstoque(){
            var gerarEtiqueta = FindControl("lblGerarEtiqueta","span");
            var gerarEstoque = FindControl("lblGerarEstoqueReal","span");

            var msg = "";

            if(gerarEtiqueta != null && gerarEtiqueta.innerHTML == "" && gerarEstoque != null && gerarEstoque.innerHTML == "")
                msg = "As opções Gerar estoque real e Gerar etiqueta de nota fiscal não estão marcadas, deseja continuar?"
            else if(gerarEtiqueta != null && gerarEtiqueta.innerHTML == "")
                msg = "A opção Gerar etiqueta de nota fiscal não esta marcada, deseja continuar?";
            else if(gerarEstoque != null && gerarEstoque.innerHTML == "")
                msg = "A opção Gerar estoque real não esta marcada, deseja continuar?";

            if(msg != "" && !confirm(msg))
                return false;

            return true;
        }
    </script>

    <table style="width: 100%">
        <tr>
            <td class="subtitle1">
                <asp:Label ID="lblSubtitle" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvNf" runat="server" AutoGenerateRows="False" DataSourceID="odsNf"
                    DefaultMode="Insert" GridLines="None" DataKeyNames="IdNf" CssClass="gridStyle detailsViewStyle"
                    OnDataBound="dtvNf_DataBound">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Dados da Nota Fiscal
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label53" runat="server" Text="Série"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtSerie" runat="server" Enabled='<%# Request["tipo"] != ((int)Glass.Data.Model.NotaFiscal.TipoDoc.Saída).ToString() %>'
                                                MaxLength="3" Text='<%# Bind("Serie") %>' Width="40px" onchange="return serieOnChange(this.value);"></asp:TextBox>
                                        </td>
                                        <td runat="server" id="titulo_subserie" onload="EntradaTerceiros_Load">
                                            Subsérie
                                        </td>
                                        <td runat="server" id="subserie" onload="EntradaTerceiros_Load">
                                            <asp:TextBox ID="txtSubserie" runat="server" MaxLength="3" Text='<%# Bind("Subserie") %>'
                                                Width="40px"></asp:TextBox>
                                        </td>
                                        <td style="padding-top: 2px">
                                            <asp:CheckBox ID="chkCompl" runat="server" Checked='<%# Bind("Complementar") %>'
                                                onClick="chkComplClick(this, true);" OnLoad="EntradaTerceiros_Load" Text="Compl." />
                                        </td>
                                        <td style="padding-top: 2px">
                                            <asp:CheckBox ID="chkServico" runat="server" Checked='<%# Bind("Servico") %>' onClick="chkServicoClick(this, true)"
                                                Text="Serviço" OnLoad="EntradaTerceiros_Load" />
                                        </td>
                                        <td>
                                            <asp:Label ID="Label328" runat="server" Text="Modelo"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtModelo" runat="server" MaxLength="3" Text='<%# Bind("Modelo") %>'
                                                Width="40px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label54" runat="server" Text="Número NF"></asp:Label>&nbsp;
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNumNfe" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                Text='<%# Bind("NumeroNFe") %>' MaxLength="9" Width="80px" OnLoad="txtNumNfe_Load"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label39" runat="server" Text="Tipo Documento"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpTipoDocumento" runat="server" Enabled="False" SelectedValue='<%# Eval("TipoDocumento") %>'>
                                                <asp:ListItem Value="1">Entrada</asp:ListItem>
                                                <asp:ListItem Value="2">Saída</asp:ListItem>
                                                <asp:ListItem Value="3">Entrada (terceiros)</asp:ListItem>
                                                <asp:ListItem Value="4">Nota Fiscal de Cliente</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblNfReferenciada" runat="server" Text="NF Referenciada" Visible="false" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblNotaFiscalConsumidorReferenciada" runat="server" Text="NFC-e Referenciada" Visible="true" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNfReferenciada" runat="server" Enabled="false" Visible="false" />
                                            <asp:HiddenField ID="hdfNfReferenciada" runat="server" Value='<%# Bind("IdsNfRef") %>' />
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imbOpenNfReferenciada" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick='<%# String.Format("openSelNfReferenciada(\"{0}\"); return false;", Eval("IdsNfRef")) %>' Visible="false" />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label57" runat="server" Text="Natureza da Operação"></asp:Label>
                                        </td>
                                        <td>
                                            <uc7:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoNf" runat="server"
                                                CodigoNaturezaOperacao='<%# Bind("IdNaturezaOperacao") %>' PermitirVazio="False"
                                                Callback="atualizaCfopDevolucao" />
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label59" runat="server" Text="Município de ocorrência"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtCidade" runat="server" Width="200px" Enabled="False" Text='<%# Eval("MunicOcor") %>'></asp:TextBox>
                                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
                                        </td>
                                        <td>
                                            <asp:Label ID="Label55" runat="server" Text="Data Emissão"></asp:Label>
                                        </td>
                                        <td>
                                            <uc6:ctrlData ID="ctrlDataEmissao" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataEmissao") %>'
                                                ExibirHoras="true" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblDataSaida" runat="server" Text="Data Saída/Entrada"></asp:Label>
                                        </td>
                                        <td>
                                            <uc6:ctrlData ID="ctrlDataSaida" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataSaidaEnt") %>'
                                                ExibirHoras="True" />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label22" runat="server" Text="Período Apuração IPI"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpPeriodoIpi" runat="server" DataSourceID="odsPeriodoIpi"
                                                DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("PeriodoApuracaoIpi") %>'>
                                            </asp:DropDownList>
                                        </td>
                                        <td nowrap="nowrap" runat="server" id="fp1" onload="AlteracaoManual_Load">
                                            <asp:Label ID="Label320" runat="server" Text="Forma Pagto."></asp:Label>
                                        </td>
                                        <td nowrap="nowrap" align="left" runat="server" id="fp2" onload="AlteracaoManual_Load">
                                            <asp:DropDownList ID="drpFormaPagto" runat="server" SelectedValue='<%# Bind("FormaPagto") %>'
                                                onclick="exibeParcelas()" OnLoad="drpFormaPagto_Load">
                                                <asp:ListItem Value="0" Text=""></asp:ListItem>
                                                <asp:ListItem Value="1">À Vista</asp:ListItem>
                                                <asp:ListItem Value="2">À Prazo</asp:ListItem>
                                                <asp:ListItem Value="3">Outros</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:TextBox ID="txtAntecip" Enabled="false" runat="server" Width="250px" />
                                            <asp:ImageButton ID="imbBuscaAntecip" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="openSelAntecipFornec(); return false;" />
                                            <asp:HiddenField ID="hdfIdAntecipFornec" runat="server" Value='<%# Bind("IdAntecipFornec") %>' />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblNumParc" runat="server" Text="Parc."></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNumParc" runat="server" Width="40px" Text='<%# Bind("NumParc") %>'
                                                onkeypress="exibeParcelas(); return soNumeros(event, true, true);" onblur="exibeParcelas();"></asp:TextBox>
                                        </td>
                                        <td style="<%= ExibirLojaEstoque() %>">
                                            <asp:Label ID="Label52" runat="server" Text="Loja (estoque)"></asp:Label>
                                        </td>
                                        <td style="<%= ExibirLojaEstoque() %>">
                                            <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="true"
                                                SelectedValue='<%# Eval("IdLoja") %>' onchange="FindControl('hdfIdLoja', 'input').value = this.value"
                                                DataSourceID="odsLoja" DataTextField="NomeFantasia" DataValueField="IdLoja">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table class="pos" runat="server" id="tbFormaPagto">
                                    <tr>
                                        <td>
                                            <uc10:ctrlFormaPagtoNotaFiscal ID="ctrlFormaPagto" runat="server" PagtoNotaFiscal='<%# Bind("PagamentoNfce") %>'
                                                EnableViewState="true" />
                                        </td>
                                    </tr>
                                </table>
                                <span runat="server" id="fatura" onload="EntradaTerceiros_Load">
                                    <table class="pos">
                                        <tr>
                                            <td>
                                                Fatura
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpFatura" runat="server" AppendDataBoundItems="True" DataSourceID="odsTipoFatura"
                                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("TipoFatura") %>'
                                                    onchange="alteraFatura(this.value)">
                                                    <asp:ListItem></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                Núm.
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtNumFatura" runat="server" MaxLength="30" Text='<%# Bind("NumFatura") %>'
                                                    Width="70px"></asp:TextBox>
                                                <asp:Label ID="lblDescrFatura" runat="server" Text="Descr."></asp:Label>
                                                <asp:TextBox ID="txtDescrFatura" runat="server" Text='<%# Bind("DescrFatura") %>'
                                                    MaxLength="100"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </span>
                                <table class="pos" id="tbParcAut">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblNumParc0" runat="server" Text="Data Base Venc."></asp:Label>
                                        </td>
                                        <td>
                                            <uc6:ctrlData ID="ctrlDataBaseVenc" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataBaseVenc") %>'
                                                ExibirHoras="False" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblNumParc1" runat="server" Text="Valor das Parcelas"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtValorParc" runat="server" Width="70px" onkeypress="return soNumeros(event, false, true)"
                                                Text='<%# Bind("ValorParc") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <span runat="server" id="br1" onload="EntradaTerceiros_Load"></span>
                                <table class="pos" id="tbChaveAcesso">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label43" runat="server" Text="Chave de Acesso" OnLoad="EntradaTerceiros_Load"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtChaveAcesso" runat="server" OnLoad="EntradaTerceiros_Load" onkeypress="return soNumeros(event, true, true)"
                                                MaxLength="44" Text='<%# Bind("ChaveAcesso") %>' Width="310px"></asp:TextBox>
                                            <asp:CustomValidator ID="valChaveAcesso" runat="server" ClientValidationFunction="validarChaveAcesso"
                                                ControlToValidate="txtChaveAcesso" ErrorMessage="Chave de acesso inválida" OnLoad="EntradaTerceiros_Load"></asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label325" runat="server" Text="Plano de contas" OnLoad="EntradaTerceiros_Load"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpPlanoContas" runat="server" AppendDataBoundItems="True"
                                                DataSourceID="odsPlanoContas" DataTextField="DescrPlanoGrupo" DataValueField="IdConta"
                                                OnLoad="EntradaTerceiros_Load" SelectedValue='<%# Bind("IdConta") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:LinkButton ID="lnkSelPlanoConta" runat="server" OnClientClick="openWindow(600, 700, '../Utils/SelPlanoConta.aspx'); return false;" OnLoad="EntradaTerceiros_Load">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                                <span runat="server" id="br2" onload="EntradaTerceiros_Load">
                                    <br />
                                </span>
                                <table class="pos" id="tbGerarEtq">
                                    <tr>
                                        <td align="center">
                                            <span runat="server" id="gerar" onload="AlteracaoManual_Load">
                                                <asp:CheckBox ID="chkGerarContasPagar" runat="server" Checked='<%# Bind("GerarContasPagar") %>'
                                                    Style="padding-right: 8px" OnLoad="GerarContasPagar_Load" Text="Gerar contas a pagar?" />
                                                <asp:CheckBox ID="chkGerarEstoqueReal" runat="server" Checked='<%# Bind("GerarEstoqueReal") %>'
                                                    Style="padding-right: 8px" OnLoad="GerarEstoqueReal_Load" Text="Gerar estoque real?" />
                                            </span>
                                             <asp:CheckBox ID="chkGerarEtiqueta" runat="server" Checked='<%# Bind("GerarEtiqueta") %>' OnLoad="chkGerarEtiqueta_Load"
                                                Text="Gerar etiqueta de nota fiscal?" />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table class="pos" id="tbParc2">
                                    <tr>
                                        <td>
                                            <asp:Label Text="Caso haja pagamento antecipado ou sinal em algum dos pedidos da nota fiscal,
                                                o valor da primeira parcela deve ser igual a soma dos pagamentos antecipados e sinais,
                                                pois a mesma será desconsiderada ao efetuar a separação de valores."
                                                ForeColor="Red" Font-Size="Small" Width="500px" runat="server" Visible="<%# Glass.Configuracoes.FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber %>" />
                                            <td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label Text="Valor total dos pagamentos antecipados:" runat="server" />
                                            <asp:TextBox runat="server" Text='<%# Eval("ValoresPagosAntecipadamente") %>' Enabled="false" Width="80px"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td runat="server" id="parc2" onload="AlteracaoManual_Load">
                                            <uc2:ctrlParcelas ID="ctrlParcelas1" runat="server" Adicionais='<%# Bind("BoletosParcelas") %>'
                                                Datas='<%# Bind("DatasParcelas") %>' ExibirCampoAdicional="True" NumParcelas="10"
                                                NumParcelasLinha="5" OnLoad="ctrlParcelas1_Load" TituloCampoAdicional="Boleto:"
                                                Valores='<%# Bind("ValoresParcelas") %>' />
                                            <asp:HiddenField runat="server" ID="hdfTotalParcelas" Value='<%# (decimal)Eval("TotalManual") > 0 ? Eval("TotalManual") : Eval("TotalNota") %>' />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Emitente
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label64" runat="server" Text="Cód."></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCodEmit" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                onblur="getRemetente(this);"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label60" runat="server" Text="Razão Social"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtRazaoEmit" runat="server" Width="300px" Enabled="False" Text='<%# Eval("NomeEmitente") %>'></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label61" runat="server" Text="CNPJ"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCnpjEmit" runat="server" Width="150px" Enabled="False" Text='<%# Eval("CnpjEmitente") %>'></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imgGetEmitente" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="abrirEmitente(); return false" Visible='<%# Eval("EditEmitente") %>' />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfExibirParcelas" runat="server" Value="true" />
                                <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="false" />
                                <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                                <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Bind("IdLoja") %>' />
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                                <asp:HiddenField ID="hdfIdFornec" runat="server" Value='<%# Bind("IdFornec") %>' />
                                <asp:HiddenField ID="hdfFinalidade" runat="server" Value='<%# Eval("FinalidadeEmissao") %>' />
                                <asp:HiddenField ID="hdfConsumidor" runat="server" Value='<%# Eval("Consumidor") %>' />
                                <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Eval("TotalNota") %>' />
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Destinatário/Remetente
                                </div>
                                <table class="pos">
                                    <tr>
                                    <td>
                                            <asp:Label ID="Label65" runat="server" Text="Cód."></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCodDest" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                onblur="getDestinatario(this);"></asp:TextBox>
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label1" runat="server" Text="Razão Social"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtRazaoDest" runat="server" ReadOnly="True" Text='<%# Eval("NomeDestRem") %>'
                                                Width="300px" Enabled="False"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label62" runat="server" Text="CPF/CNPJ"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCnpjDest" runat="server" Width="150px" Enabled="False" Text='<%# Bind("CpfCnpjDestRem") %>'
                                                 onkeypress="maskCPF(event, this);"></asp:TextBox>
                                            <asp:CustomValidator ID="valCpfCnpj" runat="server" ClientValidationFunction="validarCpfCnpj"
                                                ControlToValidate="txtCnpjDest" ErrorMessage="CPF Inválido"></asp:CustomValidator>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblSuframa" runat="server"
                                                Text="Suframa"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtSuframa" runat="server" Enabled="False"
                                                Text='<%# Eval("SuframaCliente") %>' Width="150px"
                                                ondatabinding="txtSuframa_DataBinding"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imgGetDest" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="openDestinatario(); return false;" />
                                        </td>
                                        <td>
                                            <div id="ConsultaCadCliSintegra" style="display: none">
                                                <uc9:ctrlConsultaCadCliSintegra runat="server" ID="ctrlConsultaCadCliSintegra1" OnLoad="ctrlConsultaCadCliSintegra1_Load" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Cálculo Impostos
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label45" runat="server" Text="BC do ICMS"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtBCIcms" runat="server" Width="80px" onkeypress="return soNumeros(event, false, true);"
                                                Enabled="False" Text='<%# Bind("BcIcms") %>'></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label46" runat="server" Text="Valor do ICMS"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorIcms" runat="server" Width="80px" onkeypress="return soNumeros(event, false, true);"
                                                Enabled="False" Text='<%# Bind("ValorIcms") %>'></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblValorFcp" runat="server" Text="Valor do FCP"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorFcp" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                    Text='<%# Bind("ValorFcp") %>' Width="80px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lblBcIcmsSt" runat="server" Text="BC do ICMS ST"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtBCIcmsSt" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("BcIcmsSt") %>' Width="80px" Enabled='<%# Eval("TipoDocumento").ToString() == "3" %>'></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblValorIcmsSt" runat="server" Text="Valor do ICMS ST"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorIcmsSt" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorIcmsSt") %>' Width="80px"></asp:TextBox>
                                        </td>
                                            <td align="left" nowrap="nowrap">
                                                <asp:Label ID="lblValorFcpSt" runat="server" Text="Valor do FCP ST"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtValorFcpSt" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                    Text='<%# Bind("ValorFcpSt") %>' Width="80px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" nowrap="nowrap">
                                                <asp:Label ID="lblTotalProd" runat="server" Text="Total dos Produtos"></asp:Label>
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtTotalProd" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                    Text='<%# Bind("TotalProd") %>' Width="80px"></asp:TextBox>
                                            </td>
                                        <td align="left">
                                            <asp:Label ID="lblValorIpi" runat="server" Text="Valor do IPI"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorIpi" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorIpi") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblValorIpiDevolvido" runat="server" Text="Valor do IPI Devolvido"
                                                Visible='<%# (int)Eval("FinalidadeEmissao") == (int)Glass.Data.Model.NotaFiscal.FinalidadeEmissaoEnum.Devolucao %>'></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorIpiDevolvido" runat="server" Width="80px" Enabled="False"
                                                Visible='<%# (int)Eval("FinalidadeEmissao") == (int)Glass.Data.Model.NotaFiscal.FinalidadeEmissaoEnum.Devolucao %>'
                                                onkeypress="return soNumeros(event, false, true);" Text='<%# Bind("ValorIpiDevolvido") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label48" runat="server" Text="Valor Frete"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorFrete" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorFrete") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label49" runat="server" Text="Valor do Seguro"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorSeguro" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorSeguro") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label50" runat="server" Text="Outras Despesas"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtOutrasDespesas" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("OutrasDespesas") %>' Width="80px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label321" runat="server" Text="Desconto"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtDesconto" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("Desconto") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label312" runat="server" Text="Valor PIS"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorPis" runat="server" Enabled="false" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorPis") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label329" runat="server" Text="Valor Cofins"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorCofins" runat="server" Enabled="false" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorCofins") %>' Width="80px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblValorISS" runat="server" Text="Valor ISS" Style='<%# (bool)Eval("Servico") ? "": "display: none" %>'></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorISS" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorISS") %>' Width="80px" Style='<%# (bool)Eval("Servico") ? "": "display: none" %>'></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label330" runat="server" Text="Valor da Nota"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtTotalNota" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("TotalNota") %>' Width="80px" onchange="atualizaTotalParcelas()"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label331" runat="server" Text="Valor da Nota (Manual)"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtTotalNotaManual" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("TotalManual") %>' Width="80px" Wrap="False" onchange="atualizaTotalParcelas()"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                 <div style="<%= !IsNfExportacao() ? "display: none": "" %>">

                                    <div align="center" class="dtvTitle" style="padding: 3px">
                                        Exportação
                                    </div>
                                     <br />
                                    <table class="pos">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label69" runat="server" Font-Bold="True" Text="UF de Embarque"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpUfEmbarque" runat="server" SelectedValue='<%# Bind("UfEmbarque") %>'>
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem>AC</asp:ListItem>
                                                <asp:ListItem>AL</asp:ListItem>
                                                <asp:ListItem>AM</asp:ListItem>
                                                <asp:ListItem>AP</asp:ListItem>
                                                <asp:ListItem>BA</asp:ListItem>
                                                <asp:ListItem>CE</asp:ListItem>
                                                <asp:ListItem>DF</asp:ListItem>
                                                <asp:ListItem>ES</asp:ListItem>
                                                <asp:ListItem>GO</asp:ListItem>
                                                <asp:ListItem>MA</asp:ListItem>
                                                <asp:ListItem>MG</asp:ListItem>
                                                <asp:ListItem>MS</asp:ListItem>
                                                <asp:ListItem>MT</asp:ListItem>
                                                <asp:ListItem>PB</asp:ListItem>
                                                <asp:ListItem>PA</asp:ListItem>
                                                <asp:ListItem>PE</asp:ListItem>
                                                <asp:ListItem>PI</asp:ListItem>
                                                <asp:ListItem>PR</asp:ListItem>
                                                <asp:ListItem>RJ</asp:ListItem>
                                                <asp:ListItem>RN</asp:ListItem>
                                                <asp:ListItem>RO</asp:ListItem>
                                                <asp:ListItem>RR</asp:ListItem>
                                                <asp:ListItem>RS</asp:ListItem>
                                                <asp:ListItem>SC</asp:ListItem>
                                                <asp:ListItem>SP</asp:ListItem>
                                                <asp:ListItem>SE</asp:ListItem>
                                                <asp:ListItem>TO</asp:ListItem>
                                            </asp:DropDownList>
                                            </td>

                                            <td>
                                                <asp:Label ID="Label70" runat="server" Font-Bold="True" Text="Local de Embarque"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("LocalEmbarque") %>' Width="200px" MaxLength="60"></asp:TextBox>
                                            </td>

                                            <td>
                                                <asp:Label ID="Label71" runat="server" Font-Bold="True" Text="Local de despacho"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("LocalDespacho") %>' Width="200px" MaxLength="60"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <br />

                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Transportador / Volumes transportados
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label313" runat="server" Text="Modalidade do Frete"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpFrete" runat="server" DataSourceID="odsModalidadeFrete"
                                                DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Bind("ModalidadeFrete") %>'>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label314" runat="server" Text="Transportador"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                                DataSourceID="odsTransportador" DataTextField="Nome" DataValueField="IdTransportador"
                                                SelectedValue='<%# Bind("IdTransportador") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label326" runat="server" Text="Qtd. Volumes"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtQtdVol" runat="server" Text='<%# Bind("QtdVol") %>' MaxLength="10"
                                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label327" runat="server" Text="Espécie"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtEspecie" runat="server" Text='<%# Bind("Especie") %>' MaxLength="60"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label332" runat="server" Text="Marca"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtMarca" runat="server" MaxLength="60" Text='<%# Bind("MarcaVol") %>'></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label333" runat="server" Text="Numeração"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumeracao" runat="server" MaxLength="60" Text='<%# Bind("NumeracaoVol") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label318" runat="server" Text="Peso Contêiner (kg)" ToolTip="Peso do contêiner utilizado para transportar a carga"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtPesoConteiner" runat="server" Text='<%# Bind("PesoConteiner") %>'></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label319" runat="server" Text="Peso Líquido (kg)" ToolTip="Peso dos produtos da nota fiscal"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtPesoLiquido" runat="server" Text='<%# Bind("PesoLiq") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lblTotBruto" runat="server" Text="Peso Bruto (kg)" ToolTip="Peso líquido somado com o peso do container"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtPesoBruto" runat="server" Text='<%# Bind("PesoBruto") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px; background: none">
                                    Dados do veículo
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label315" runat="server" Text="Placa"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtVeicPlaca" runat="server" Text='<%# Bind("VeicPlaca") %>' MaxLength="8"
                                                Width="70px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label316" runat="server" Text="RNTC "></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtVeicRntc" runat="server" MaxLength="20" Text='<%# Bind("VeicRntc") %>'></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label317" runat="server" Text="UF do veículo"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpVeicUf" runat="server" SelectedValue='<%# Bind("VeicUf") %>'>
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem>AC</asp:ListItem>
                                                <asp:ListItem>AL</asp:ListItem>
                                                <asp:ListItem>AM</asp:ListItem>
                                                <asp:ListItem>AP</asp:ListItem>
                                                <asp:ListItem>BA</asp:ListItem>
                                                <asp:ListItem>CE</asp:ListItem>
                                                <asp:ListItem>DF</asp:ListItem>
                                                <asp:ListItem>ES</asp:ListItem>
                                                <asp:ListItem>GO</asp:ListItem>
                                                <asp:ListItem>MA</asp:ListItem>
                                                <asp:ListItem>MG</asp:ListItem>
                                                <asp:ListItem>MS</asp:ListItem>
                                                <asp:ListItem>MT</asp:ListItem>
                                                <asp:ListItem>PB</asp:ListItem>
                                                <asp:ListItem>PA</asp:ListItem>
                                                <asp:ListItem>PE</asp:ListItem>
                                                <asp:ListItem>PI</asp:ListItem>
                                                <asp:ListItem>PR</asp:ListItem>
                                                <asp:ListItem>RJ</asp:ListItem>
                                                <asp:ListItem>RN</asp:ListItem>
                                                <asp:ListItem>RO</asp:ListItem>
                                                <asp:ListItem>RR</asp:ListItem>
                                                <asp:ListItem>RS</asp:ListItem>
                                                <asp:ListItem>SC</asp:ListItem>
                                                <asp:ListItem>SP</asp:ListItem>
                                                <asp:ListItem>SE</asp:ListItem>
                                                <asp:ListItem>TO</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Informações Complementares
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtInfCmpl" runat="server" MaxLength="500" Rows="2" Text='<%# Bind("InfCompl") %>'
                                                TextMode="MultiLine" Width="608px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>

                                <div style="<%= !IsContingenciaFsda() ? "display: none": "" %>">
                                    <br />
                                    <br />
                                    <div align="center" class="dtvTitle" style="padding: 3px">
                                        Dados sobre o formulário FS-DA
                                    </div>
                                    <table class="pos">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label26" runat="server" Font-Bold="True" Text="Número do documento FS-DA"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtNumDocFsda" runat="server" onkeypress="return soNumeros(event, false, false)"
                                                    Text='<%# Bind("NumeroDocumentoFsda") %>' Width="200px" MaxLength="12"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Dados da Nota Fiscal
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label53" runat="server" Text="Série"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtSerie" runat="server" Enabled='<%# Request["tipo"] == ((int)Glass.Data.Model.NotaFiscal.TipoDoc.EntradaTerceiros).ToString() || Request["tipo"] == ((int)Glass.Data.Model.NotaFiscal.TipoDoc.Entrada).ToString() %>'
                                                MaxLength="3" onchange="return serieOnChange(this.value);" OnLoad="txtSerie_Load"
                                                Text='<%# Bind("Serie") %>' Width="40px"></asp:TextBox>
                                        </td>
                                        <td runat="server" id="titulo_subserie" onload="EntradaTerceiros_Load">
                                            Subsérie
                                        </td>
                                        <td runat="server" id="subserie" onload="EntradaTerceiros_Load">
                                            <asp:TextBox ID="txtSubserie" runat="server" MaxLength="3" Text='<%# Bind("Subserie") %>'
                                                Width="40px"></asp:TextBox>
                                        </td>
                                        <td style="padding-top: 2px">
                                            <asp:CheckBox ID="chkCompl" runat="server" Checked='<%# Bind("Complementar") %>'
                                                onClick="chkComplClick(this, true);" OnLoad="EntradaTerceiros_Load" Text="Compl." />
                                        </td>
                                        <td style="padding-top: 2px">
                                            <asp:CheckBox ID="chkServico" runat="server" Checked='<%# Bind("Servico") %>' onClick="chkServicoClick(this, true)"
                                                Text="Serviço" OnLoad="EntradaTerceiros_Load" />
                                        </td>
                                        <td>
                                            <asp:Label ID="Label328" runat="server" Text="Modelo"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtModelo" runat="server" MaxLength="3" Text='<%# Bind("Modelo") %>'
                                                Width="40px"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblNumeroNF" runat="server" Text="Número NF" OnLoad="txtNumNfe_Load"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumeroNF" runat="server" Width="80px" onkeypress="return soNumeros(event, true, true)"
                                                Text='<%# Bind("NumeroNFe") %>' MaxLength="9" OnLoad="txtNumNfe_Load"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label39" runat="server" Text="Tipo Documento"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpTipoDocumento" runat="server" Enabled="false">
                                                <asp:ListItem Value="1">Entrada</asp:ListItem>
                                                <asp:ListItem Value="2">Saída</asp:ListItem>
                                                <asp:ListItem Value="3">Entrada (terceiros)</asp:ListItem>
                                                <asp:ListItem Value="4">Nota Fiscal de Cliente</asp:ListItem>
                                                <asp:ListItem Value="4">Saída NFC-e</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblNfReferenciada" runat="server" Text="NF Referenciada" Visible="false"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblNotaFiscalConsumidorReferenciada" runat="server" Text="NFC-e Referenciada" Visible="true" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtNfReferenciada" runat="server" Enabled="false" Visible="false"></asp:TextBox>
                                            <asp:HiddenField ID="hdfNfReferenciada" runat="server" Value='<%# Bind("IdsNfRef") %>' />
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imbOpenNfReferenciada" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick='<%# String.Format("openSelNfReferenciada(\"{0}\"); return false;", Eval("IdsNfRef")) %>' Visible="false" />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label57" runat="server" Text="Natureza da Operação"></asp:Label>
                                        </td>
                                        <td>
                                            <uc7:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoNf" runat="server"
                                                CodigoNaturezaOperacao='<%# Bind("IdNaturezaOperacao") %>' PermitirVazio="False"
                                                Callback="atualizaCfopDevolucao" />
                                        </td>
                                        <td>
                                            <asp:Label ID="Label59" runat="server" Text="Município de ocorrência"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCidade" runat="server" Width="200px" Enabled="False"></asp:TextBox>
                                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClientClick="openWindow(500, 700, '../Utils/SelCidade.aspx'); return false;" />
                                            <asp:HiddenField ID="hdfCidade" runat="server" Value='<%# Bind("IdCidade") %>' />
                                        </td>
                                        <td>
                                            <asp:Label ID="Label55" runat="server" Text="Data Emissão"></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <uc6:ctrlData ID="ctrlDataEmissao" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataEmissao") %>'
                                                ExibirHoras="true" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblDataSaida" runat="server" Text="Data Saída/Entrada"></asp:Label>
                                        </td>
                                        <td>
                                            <uc6:ctrlData ID="ctrlDataSaida" runat="server" ReadOnly="ReadWrite" DataString='<%# Bind("DataSaidaEnt") %>'
                                                ExibirHoras="True" />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label22" runat="server" Text="Período Apuração IPI"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpPeriodoIpi" runat="server" DataSourceID="odsPeriodoIpi"
                                                DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("PeriodoApuracaoIpi") %>'
                                                OnDataBound="drpPeriodoIpi_DataBound">
                                            </asp:DropDownList>
                                        </td>
                                        <td id="fp1">
                                            <asp:Label ID="Label320" runat="server" Text="Forma Pagto."></asp:Label>
                                        </td>
                                        <td id="fp2">
                                            <asp:DropDownList ID="drpFormaPagto" runat="server" SelectedValue='<%# Bind("FormaPagto") %>'
                                                OnLoad="drpFormaPagto_Load">
                                                <asp:ListItem Value="1">À Vista</asp:ListItem>
                                                <asp:ListItem Value="2">À Prazo</asp:ListItem>
                                                <asp:ListItem Value="3">Outros</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtAntecip" Enabled="false" runat="server" Width="250px" />
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imbBuscaAntecip" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="openSelAntecipFornec(); return false;" />
                                            <asp:HiddenField ID="hdfIdAntecipFornec" runat="server" Value='<%# Bind("IdAntecipFornec") %>' />
                                        </td>
                                        <td style="<%= ExibirLojaEstoque() %>">
                                            <asp:Label ID="Label52" runat="server" Text="Loja (estoque)"></asp:Label>
                                        </td>
                                        <td style="<%= ExibirLojaEstoque() %>">
                                            <asp:DropDownList ID="drpLoja" runat="server" AppendDataBoundItems="true"
                                                SelectedValue='<%# Eval("IdLoja") %>' onchange="FindControl('hdfIdLoja', 'input').value = this.value"
                                                DataSourceID="odsLoja" DataTextField="NomeFantasia" DataValueField="IdLoja">
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <table class="pos" runat="server" id="tbFormaPagto">
                                    <tr>
                                        <td>
                                            <uc10:ctrlFormaPagtoNotaFiscal ID="ctrlFormaPagto" runat="server" PagtoNotaFiscal='<%# Bind("PagamentoNfce") %>'
                                                EnableViewState="true" />
                                        </td>
                                    </tr>
                                </table>
                                <span runat="server" id="br1" onload="EntradaTerceiros_Load">
                                    <br />
                                </span>
                                <table class="pos" id="tbChaveAcesso">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label43" runat="server" OnLoad="EntradaTerceiros_Load" Text="Chave de Acesso"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtChaveAcesso" runat="server" MaxLength="44" OnLoad="EntradaTerceiros_Load"
                                                onkeypress="return soNumeros(event, true, true)" Text='<%# Bind("ChaveAcesso") %>'
                                                Width="310px"></asp:TextBox>
                                            <asp:CustomValidator ID="valChaveAcesso" runat="server" ClientValidationFunction="validarChaveAcesso"
                                                ControlToValidate="txtChaveAcesso" ErrorMessage="Chave de acesso inválida"></asp:CustomValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label325" runat="server" OnLoad="EntradaTerceiros_Load" Text="Plano de contas"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpPlanoContas" runat="server" AppendDataBoundItems="True"
                                                DataSourceID="odsPlanoContas" DataTextField="DescrPlanoGrupo" DataValueField="IdConta"
                                                OnLoad="EntradaTerceiros_Load" SelectedValue='<%# Bind("IdConta") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:LinkButton ID="lnkSelPlanoConta" runat="server" OnClientClick="openWindow(600, 700, '../Utils/SelPlanoConta.aspx'); return false;" OnLoad="EntradaTerceiros_Load">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                                <span runat="server" id="br2" onload="EntradaTerceiros_Load">
                                    <br />
                                </span>
                                <table class="pos" id="tbGerarEtq">
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkGerarContasPagar" runat="server" OnLoad="GerarContasPagar_Load"
                                                Checked='<%# Bind("GerarContasPagar") %>' Text="Gerar contas a pagar?" Style="padding-right: 8px" />
                                            <asp:CheckBox ID="chkGerarEstoqueReal" runat="server" Checked='<%# Bind("GerarEstoqueReal") %>'
                                                OnLoad="GerarEstoqueReal_Load" Text="Gerar estoque real?" Style="padding-right: 8px" />
                                            <asp:CheckBox ID="chkGerarEtiqueta" runat="server" Checked='<%# Bind("GerarEtiqueta") %>' OnLoad="chkGerarEtiqueta_Load"
                                                Text="Gerar etiqueta de nota fiscal?" />
                                        </td>
                                    </tr>
                                </table>
                                <uc1:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="tipo"
                                    Text='<%# Bind("TipoDocumento") %>' />
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Emitente
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label64" runat="server" Text="Cód."></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCodEmit" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                onblur="getRemetente(this);"></asp:TextBox>
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label60" runat="server" Text="Razão Social"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtRazaoEmit" runat="server" Width="300px" Enabled="False"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label61" runat="server" Text="CNPJ"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCnpjEmit" runat="server" Width="150px" Enabled="False"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imgGetEmitente" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="abrirEmitente(); return false" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Bind("IdLoja") %>' />
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Bind("IdCliente") %>' />
                                <asp:HiddenField ID="hdfIdFornec" runat="server" Value='<%# Bind("IdFornec") %>' />
                                <asp:HiddenField ID="hdfFinalidade" runat="server" Value='<%# Bind("FinalidadeEmissao") %>' />
                                <asp:HiddenField ID="hdfConsumidor" runat="server" Value='<%# Bind("Consumidor") %>' />
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Destinatário/Remetente
                                </div>
                                <table>
                                    <tr>
                                    <td>
                                            <asp:Label ID="Label66" runat="server" Text="Cód."></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCodDest" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                onblur="getDestinatario(this);"></asp:TextBox>
                                        </td>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label1" runat="server" Text="Razão Social"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtRazaoDest" runat="server" ReadOnly="True" Width="300px" Enabled="False"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label62" runat="server" Text="CPF/CNPJ"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCnpjDest" runat="server" Width="130px" Enabled="False" Text='<%# Bind("CpfCnpjDestRem") %>' onblur="validaCPF(this);"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblSuframa" runat="server" Text="Suframa" style="display: none"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtSuframa" runat="server" Enabled="False" Width="130px" style="display: none"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="imgGetDest" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                OnClientClick="openDestinatario(); return false;" />
                                        </td>
                                        <td>
                                            <div id="ConsultaCadCliSintegra" style="display: none">
                                                <uc9:ctrlConsultaCadCliSintegra runat="server" ID="ctrlConsultaCadCliSintegra1" OnLoad="ctrlConsultaCadCliSintegra1_Load" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Cálculo Impostos
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label45" runat="server" Text="BC do ICMS"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtBCIcms" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("BcIcms") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label46" runat="server" Text="Valor do ICMS"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorIcms" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorIcms") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblValorFcp" runat="server" Text="Valor do FCP"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorFcp" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorFcp") %>' Width="80px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lblBcIcmsSt" runat="server" Text="BC do ICMS ST"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtBCIcmsSt" runat="server" Enabled="false" OnKeyPress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("BcIcmsSt") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblValorIcmsSt" runat="server" Text="Valor do ICMS ST"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorIcmsSt" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorIcmsSt") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="lblValorFcpSt" runat="server" Text="Valor do FCP ST"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorFcpSt" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorFcpSt") %>' Width="80px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">&nbsp;
                                        </td>
                                        <td align="left">&nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblValorIpi" runat="server" Text="Valor do IPI"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorIpi" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Eval("ValorIpi") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblValorIpiDevolvido" runat="server" Text="Valor do IPI Devolvido" Visible="false"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorIpiDevolvido" runat="server" Width="80px" onkeypress="return soNumeros(event, false, true);"
                                                Visible="false" Text='<%# Bind("ValorIpiDevolvido") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label48" runat="server" Text="Valor Frete"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorFrete" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorFrete") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label49" runat="server" Text="Valor do Seguro"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorSeguro" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorSeguro") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label50" runat="server" Text="Outras Despesas"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtOutrasDespesas" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("OutrasDespesas") %>' Width="80px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label321" runat="server" Text="Desconto"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtDesconto" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("Desconto") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label329" runat="server" Text="Valor PIS"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorPis" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorPis") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label330" runat="server" Text="Valor Cofins"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorCofins" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorCofins") %>' Width="80px"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lblValorISS" runat="server" Text="Valor ISS" Style="display: none"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtValorISS" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorISS") %>' Width="80px" Style="display: none"></asp:TextBox>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label312" runat="server" Text="Valor da Nota"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtTotalNota" runat="server" Enabled="False" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("TotalNota") %>' Width="80px"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Transportador / Volumes transportados
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label313" runat="server" Text="Modalidade do Frete"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpFrete" runat="server" DataSourceID="odsModalidadeFrete"
                                                DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Bind("ModalidadeFrete") %>'>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label314" runat="server" Text="Transportador"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpTransportador" runat="server" AppendDataBoundItems="True"
                                                DataSourceID="odsTransportador" DataTextField="Nome" DataValueField="IdTransportador"
                                                SelectedValue='<%# Bind("IdTransportador") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label326" runat="server" Text="Qtd. Volumes"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtQtdVol" runat="server" Text='<%# Bind("QtdVol") %>' MaxLength="10"
                                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label327" runat="server" Text="Espécie"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtEspecie" runat="server" Text='<%# Bind("Especie") %>' MaxLength="60"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label332" runat="server" Text="Marca"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtMarca" runat="server" MaxLength="60" Text='<%# Bind("MarcaVol") %>'></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label333" runat="server" Text="Numeração"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtNumeracao" runat="server" MaxLength="60" Text='<%# Bind("NumeracaoVol") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label318" runat="server" Text="Peso Bruto (kg)"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtPesoBruto" runat="server" MaxLength="10" Text='<%# Bind("PesoBruto") %>'></asp:TextBox>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label319" runat="server" Text="Peso Líquido (kg)"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtPesoLiquido" runat="server" MaxLength="10" Text='<%# Bind("PesoLiq") %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px; background: none">
                                    Dados do veículo
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label315" runat="server" Text="Placa"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtVeicPlaca" runat="server" Text='<%# Bind("VeicPlaca") %>' MaxLength="8"
                                                Width="70px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label316" runat="server" Text="RNTC "></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtVeicRntc" runat="server" Text='<%# Bind("VeicRntc") %>' MaxLength="20"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label317" runat="server" Text="UF do veículo"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="drpVeicUf" runat="server" SelectedValue='<%# Bind("VeicUf") %>'>
                                                <asp:ListItem></asp:ListItem>
                                                <asp:ListItem>AC</asp:ListItem>
                                                <asp:ListItem>AL</asp:ListItem>
                                                <asp:ListItem>AM</asp:ListItem>
                                                <asp:ListItem>AP</asp:ListItem>
                                                <asp:ListItem>BA</asp:ListItem>
                                                <asp:ListItem>CE</asp:ListItem>
                                                <asp:ListItem>DF</asp:ListItem>
                                                <asp:ListItem>ES</asp:ListItem>
                                                <asp:ListItem>GO</asp:ListItem>
                                                <asp:ListItem>MA</asp:ListItem>
                                                <asp:ListItem>MG</asp:ListItem>
                                                <asp:ListItem>MS</asp:ListItem>
                                                <asp:ListItem>MT</asp:ListItem>
                                                <asp:ListItem>PB</asp:ListItem>
                                                <asp:ListItem>PA</asp:ListItem>
                                                <asp:ListItem>PE</asp:ListItem>
                                                <asp:ListItem>PI</asp:ListItem>
                                                <asp:ListItem>PR</asp:ListItem>
                                                <asp:ListItem>RJ</asp:ListItem>
                                                <asp:ListItem>RN</asp:ListItem>
                                                <asp:ListItem>RO</asp:ListItem>
                                                <asp:ListItem>RR</asp:ListItem>
                                                <asp:ListItem>RS</asp:ListItem>
                                                <asp:ListItem>SC</asp:ListItem>
                                                <asp:ListItem>SP</asp:ListItem>
                                                <asp:ListItem>SE</asp:ListItem>
                                                <asp:ListItem>TO</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Informações Complementares
                                </div>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtInfCmpl" runat="server" MaxLength="500" Rows="2" Text='<%# Bind("InfCompl") %>'
                                                TextMode="MultiLine" Width="608px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>

                                <div style="<%= !IsContingenciaFsda() ? "display: none": "" %>">
                                    <br />
                                    <br />
                                    <div align="center" class="dtvTitle" style="padding: 3px">
                                        Dados sobre o formulário FS-DA
                                    </div>
                                    <table class="pos">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label26" runat="server" Font-Bold="True" Text="Número do documento FS-DA"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtNumDocFsda" runat="server" onkeypress="return soNumeros(event, false, false)"
                                                    Text='<%# Bind("NumeroDocumentoFsda") %>' Width="200px" MaxLength="12"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Dados da Nota Fiscal
                                </div>
                                <table class="pos espaco">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label53" runat="server" Text="Série" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label320" runat="server" Text='<%# Eval("Serie") %>'></asp:Label>
                                            <asp:Label ID="Label362" runat="server" Text='<%# !String.IsNullOrEmpty(Eval("Subserie") as string) ? "&nbsp;&nbsp;Sub.: " + Eval("Subserie") : "" %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label54" runat="server" Text="Número NF" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label322" runat="server" Text='<%# Eval("NumeroNFe") %>'></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label55" runat="server" Text="Data de Emissão" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label325" runat="server" Text='<%# Eval("DataEmissao", "{0:d}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label57" runat="server" Text="Natureza da Operação"
                                                Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label321" runat="server"
                                                Text='<%# Eval("CodNaturezaOperacao") %>'></asp:Label>
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label39" runat="server" Text="Tipo Documento" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label323" runat="server" Text='<%# Eval("TipoDocumentoString") %>'></asp:Label>
                                        </td>
                                        <td nowrap="nowrap" align="left">
                                            <asp:Label ID="Label56" runat="server" Text="Data Saída/Entrada" Font-Bold="True" OnLoad="Nfce_Load"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label324" runat="server" Text='<%# Eval("DataSaidaEnt", "{0:g}") %>' OnLoad="Nfce_Load"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label59" runat="server" Text="Município de ocorrência" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label326" runat="server" Text='<%# Eval("MunicOcor") %>'></asp:Label>
                                        </td>
                                        <td nowrap="nowrap" align="left">
                                            <asp:Label ID="Label23" runat="server" Font-Bold="True" Text="Período Apuração IPI"></asp:Label>
                                        </td>
                                        <td nowrap="nowrap" align="left">
                                            <asp:Label ID="Label24" runat="server" Text='<%# Eval("DescrPeriodoApuracaoIpi") %>'></asp:Label>
                                        </td>
                                        <td nowrap="nowrap" align="left">
                                            <asp:Label ID="Label345" runat="server" Font-Bold="True" Text="Forma Pagto"></asp:Label>
                                        </td>
                                        <td nowrap="nowrap" align="left">
                                            <asp:Label ID="Label346" runat="server" Text='<%# Eval("FormaPagtoString") %>'></asp:Label>
                                            <asp:Label ID="Label21" runat="server" Text='<%# !String.IsNullOrEmpty(Eval("DescrTipoFatura") as string) ? "(Fat.: " + Eval("DescrTipoFatura") + " " + Eval("NumFatura") + ")" : "" %>' OnLoad="Nfce_Load"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td nowrap="nowrap" align="left">
                                            <asp:Label ID="Label15" runat="server" Font-Bold="True" Text="Plano de contas" OnLoad="EntradaTerceiros_Load"></asp:Label>
                                        </td>
                                        <td nowrap="nowrap" align="left" colspan="3">
                                            <asp:Label ID="Label17" runat="server" Text='<%# Eval("DescrPlanoContas") %>' OnLoad="EntradaTerceiros_Load"></asp:Label>
                                        </td>
                                        <td nowrap="nowrap" align="left" style="<%= ExibirLojaEstoque() %>">
                                            <asp:Label ID="Label58" runat="server" Font-Bold="True" Text="Loja (estoque)"></asp:Label>
                                        </td>
                                        <td nowrap="nowrap" align="left" style="<%= ExibirLojaEstoque() %>">
                                            <asp:Label ID="Label63" runat="server" Text='<%# ObtemNomeLoja(Eval("IdLoja")) %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="6" nowrap="nowrap">
                                            <table>
                                                <tr>
                                                    <td><asp:Label ID="Label367" runat="server" Style="padding-right: 8px" ForeColor="Green"
                                                Text='<%# (bool)Eval("Transporte") ? "Transporte" : (bool)Eval("Complementar") ? "Complementar" : "Serviço" %>'
                                                Visible='<%# (bool)Eval("Transporte") || (bool)Eval("Complementar") || (bool)Eval("Servico") %>'></asp:Label></td>
                                                    <td><asp:Label ID="Label16" runat="server" Font-Bold="False" ForeColor="Blue" Style="padding-right: 8px"
                                                OnLoad="EntradaTerceiros_Load" Text='<%# Eval("DescrGerarContasPagar") %>'></asp:Label></td>
                                                    <td></td>
                                                    <td><asp:Label ID="lblGerarEtiqueta" runat="server" Font-Bold="False" ForeColor="Blue" OnLoad="EntradaTerceiros_Load"
                                                Text='<%# (bool)Eval("GerarEtiqueta") ? "Gerar etiqueta de nota fiscal" : "" %>'></asp:Label></td>
                                                    <td><asp:Label ID="lblGerarEstoqueReal" runat="server" Font-Bold="False" ForeColor="Blue" OnLoad="EntradaTerceiros_Load"
                                                Text='<%# (bool)Eval("GerarEstoqueReal") ? "Gerar estoque real" : "" %>'></asp:Label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Emitente
                                </div>
                                <table class="pos espaco">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label60" runat="server" Text="Razão Social" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label327" runat="server" Text='<%# Eval("NomeEmitente") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label61" runat="server" Text="CNPJ" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label328" runat="server" Text='<%# Eval("CnpjEmitente") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Destinatário/Remetente
                                </div>
                                <table class="pos espaco">
                                    <tr>
                                        <td nowrap="nowrap">
                                            <asp:Label ID="Label1" runat="server" Text="Razão Social" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label329" runat="server" Text='<%# Eval("NomeDestRem") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label62" runat="server" Text="CPF/CNPJ" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label330" runat="server" Text='<%# Eval("CpfCnpjDestRem") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:LinkButton ID="btnConsSitContr" ToolTip="Consulta Situação do Contribuinte no Sintegra"
                                                Style="display: none;" runat="server" OnClientClick="ConsSitCadContr(); return false;"><img alt="" src="../Images/ConsSitNFe.gif" /></asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Cálculo Impostos
                                </div>
                                <table class="pos espaco">
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label45" runat="server" Text="BC do ICMS" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label331" runat="server" Text='<%# Eval("BcIcms", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label46" runat="server" Text="Valor do ICMS" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label332" runat="server" Text='<%# Eval("Valoricms", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label30" runat="server" Text="Valor do FCP" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="lblValorFcp" runat="server" Text='<%# Eval("ValorFcp", "{0:C}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label352" runat="server" Text="BC do ICMS ST" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label354" runat="server" Text='<%# Eval("BcIcmsSt", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label353" runat="server" Font-Bold="True" Text="Valor do ICMS ST"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label355" runat="server" Text='<%# Eval("ValorIcmsSt", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label68" runat="server" Text="Valor do FCP ST" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="lblValorFcpSt" runat="server" Text='<%# Eval("ValorFcpSt", "{0:C}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label311" runat="server" Text="Total dos Produtos" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label333" runat="server" Text='<%# Eval("TotalProd", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label347" runat="server" Font-Bold="True" Text="Valor do IPI"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label348" runat="server" Text='<%# Eval("ValorIpi", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblValorIpiDevolvido" runat="server" Font-Bold="True" Text="Valor do IPI Devolvido"
                                                Visible='<%# (int)Eval("FinalidadeEmissao") == (int)Glass.Data.Model.NotaFiscal.FinalidadeEmissaoEnum.Devolucao %>'></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="lblValorIpiDevolvido1" runat="server" Text='<%# Eval("ValorIpiDevolvido", "{0:C}") %>'
                                                Visible='<%# (int)Eval("FinalidadeEmissao") == (int)Glass.Data.Model.NotaFiscal.FinalidadeEmissaoEnum.Devolucao %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label48" runat="server" Font-Bold="True" Text="Valor Frete"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label334" runat="server" Text='<%# Eval("ValorFrete", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label49" runat="server" Font-Bold="True" Text="Valor do Seguro"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label335" runat="server" Text='<%# Eval("ValorSeguro", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label50" runat="server" Font-Bold="True" Text="Outras Despesas"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label336" runat="server" Text='<%# Eval("OutrasDespesas", "{0:C}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label350" runat="server" Font-Bold="True" Text="Desconto"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label351" runat="server" Text='<%# Eval("Desconto", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label368" runat="server" Font-Bold="True" Text="Valor PIS"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label369" runat="server" Text='<%# Eval("ValorPis", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label370" runat="server" Font-Bold="True" Text="Valor Cofins"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label371" runat="server" Text='<%# Eval("ValorCofins", "{0:C}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" nowrap="nowrap">
                                            <asp:Label ID="Label365" runat="server" Font-Bold="True" Text="Valor ISS" Visible='<%# ((int)Eval("TipoDocumento") == (int)Glass.Data.Model.NotaFiscal.TipoDoc.EntradaTerceiros) && (bool)Eval("Servico") %>'></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label366" runat="server" Text='<%# Eval("ValorISS", "{0:C}") %>' Visible='<%# ((int)Eval("TipoDocumento") == (int)Glass.Data.Model.NotaFiscal.TipoDoc.EntradaTerceiros) && (bool)Eval("Servico") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label312" runat="server" Font-Bold="True" Text="Valor da Nota"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label337" runat="server" Text='<%# Eval("TotalNota", "{0:C}") %>'></asp:Label>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label363" runat="server" Font-Bold="True" Text="Valor da Nota (Manual)"></asp:Label>
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="Label364" runat="server" Text='<%# Eval("TotalManual", "{0:C}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <div style="<%= !IsNfExportacao() ? "display: none": "" %>">
                                    <div align="center" class="dtvTitle" style="padding: 3px">
                                        Exportação
                                    </div>
                                    <table class="pos espaco">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label72" runat="server" Text="UF de Embarque " Font-Bold="True"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label73" runat="server" Text='<%# Eval("UfEmbarque") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label74" runat="server" Text="Local de embarque " Font-Bold="True"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label75" runat="server" Text='<%# Eval("LocalEmbarque") %>'></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label76" runat="server" Text="Local de despacho" Font-Bold="True"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label77" runat="server" Text='<%# Eval("LocalDespacho") %>'></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Transportador / Volumes transportados
                                </div>
                                <table class="pos espaco">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label313" runat="server" Text="Modalidade do Frete" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label338" runat="server" Text='<%# Eval("ModalidadeFreteString") %>'></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label314" runat="server" Text="Transportador" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label339" runat="server" Text='<%# Eval("NomeTransportador") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label357" runat="server" Font-Bold="True" Text="Qtd. Volume"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label358" runat="server" Text='<%# Eval("QtdVol") %>'></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label360" runat="server" Font-Bold="True" Text="Espécie"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label359" runat="server" Text='<%# Eval("Especie") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label372" runat="server" Font-Bold="True" Text="Marca"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label374" runat="server" Text='<%# Eval("MarcaVol") %>'></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label373" runat="server" Font-Bold="True" Text="Numeração"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label375" runat="server" Text='<%# Eval("NumeracaoVol") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="Label318" runat="server" Font-Bold="True" Text="Peso Bruto (kg)"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label340" runat="server" Text='<%# Eval("PesoBruto") %>'></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label319" runat="server" Font-Bold="True" Text="Peso Líquido (kg)"></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label341" runat="server" Text='<%# Eval("PesoLiq") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px; background: none">
                                    Dados do veículo
                                </div>
                                <table class="pos espaco">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label315" runat="server" Text="Placa" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label342" runat="server" Text='<%# Eval("VeicPlaca") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label316" runat="server" Text="RNTC " Font-Bold="True"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label343" runat="server" Text='<%# Eval("VeicRntc") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label317" runat="server" Text="UF do veículo" Font-Bold="True"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label344" runat="server" Text='<%# Eval("VeicUf") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCliente") %>' />
                                <asp:HiddenField ID="hdfIdFornec" runat="server" Value='<%# Eval("IdFornec") %>' />
                                <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Eval("TotalNota") %>' />
                                <asp:HiddenField ID="hdfFinalidade" runat="server" Value='<%# Eval("FinalidadeEmissao") %>' />
                                <br />
                                <br />
                                <div align="center" class="dtvTitle" style="padding: 3px">
                                    Informações Complementares
                                </div>
                                <table class="pos espaco">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label349" runat="server" Text='<%# Eval("InfCompl") %>'></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label51" runat="server"
                                                Text='<%# Eval("MensagemNaturezasOperacao") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <div style="<%= !IsContingenciaFsda() ? "display: none": "" %>">
                                    <div align="center" class="dtvTitle" style="padding: 3px">
                                        Dados sobre o formulário FS-DA
                                    </div>
                                    <table class="pos espaco">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label26" runat="server" Font-Bold="True" Text="Número do documento FS-DA"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="Label27" runat="server" Text='<%# Eval("NumeroDocumentoFsda") %>'></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                                <div align="center" class="dtvTitle" style="padding: 3px"
                                     runat="server" visible='<%# Glass.Configuracoes.RentabilidadeConfig.ExibirRentabilidadeNotaFiscal %>'>
                                    Rentabilidade
                                </div>
                                <table class="pos espaco"
                                       runat="server" visible='<%# Glass.Configuracoes.RentabilidadeConfig.ExibirRentabilidadeNotaFiscal %>'>
                                    <tr >
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
                                            <a href="#" onclick='openWindow(500, 700, "../Relatorios/Rentabilidade/VisualizacaoItemRentabilidade.aspx?tipo=notafiscal&id=<%# Eval("IdNf") %>"); return false;'>
                                            <img border="0" src="../Images/cash_red.png" title="Rentabilidade" /></a>
                                        </td>
                                        <td align="left" nowrap="nowrap" colspan="4"></td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="if (!onInsertUpdate()) return false; return onSave();"
                                    Text="Inserir" />
                                <asp:Button ID="btnCancelar" runat="server" CausesValidation="false" OnClick="btnCancelar_Click"
                                    Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" CausesValidation="false" Text="Editar" OnClick="btnEditar_Click" />
                                <asp:Button ID="btnEmitir" runat="server"  CausesValidation="false" ForeColor="Red" OnClientClick="return onEmitirNFe();"
                                    Text="Emitir Nota Fiscal" Width="125px" OnClick="btnEmitir_Click" OnLoad="btnEmitirFinalizar_Load" />
                                <asp:Button ID="btnFinalizar" runat="server" ForeColor="Red" OnClick="btnFinalizar_Click"  CausesValidation="false"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja finalizar essa nota fiscal?&quot;) && validaFciFinalizarEmitir() && confirmarNaturezaOperacaoAlteraEstoqueFiscal() && alertaGerarEtiquetaEstoque();"
                                    OnLoad="btnEmitirFinalizar_Load" Text="Finalizar" />
                                <asp:Button ID="btnFinalizarFs" runat="server" ForeColor="Red" OnClick="btnFinalizarFs_Click"
                                    OnLoad="btnEmitirFinalizar_Load" Text="Finalizar"  CausesValidation="false" />
                                <asp:Button ID="btnPreVisualizar" runat="server" CausesValidation="false" OnClick="btnPreVisualizar_Click"
                                    OnLoad="btnPreVisualizar_Load" Text="Pré-visualizar" ToolTip="Exibe uma prévia de como ficará o DANFE após emissão da nota." />
                                <asp:Button ID="btnVoltar" runat="server" CausesValidation="false" OnClick="btnCancelar_Click"
                                    Text="Voltar" />
                                <asp:ImageButton ID="imgObsLancFiscal" runat="server" ImageUrl="~/Images/Nota.gif"
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/SetObsLancFiscal.aspx?idNf=" + Eval("IdNf") + "\"); return false" %>'
                                    ToolTip="Observações do Lançamento Fiscal" />
                                <asp:ImageButton ID="imgAguaGasEnergia" runat="server" ImageUrl="~/Images/page_gear.png"
                                    OnClientClick='<%# "openWindow(600, 800, \"../Utils/InfoAdicNotaFiscal.aspx?idNf=" + Eval("IdNf") + "\"); return false" %>'
                                    ToolTip="Informações adicionais" Visible='<%# Eval("ExibirLinkInfoAdic") %>' />
                                <asp:ImageButton ID="imgAjustes" runat="server" ImageUrl="~/Images/dinheiro.gif"
                                    onclientclick='<%# Eval("IdNf", "openWindow(600, 950, \"../Listas/LstAjusteDocumentoFiscal.aspx?idNf={0}\"); return false;") %>'
                                    ToolTip="Ajustes do Documento Fiscal" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="return onSave();" />
                                <asp:Button ID="btnCancelar" runat="server" OnClick="btnCancelarEdit_Click" Text="Cancelar"
                                    CausesValidation="false" />
                                <script type="text/javascript">
                                    $(document).ready(function() {
                                        atualizaCfopDevolucao("ctl00_ctl00_Pagina_Conteudo_dtvNf_ctrlNaturezaOperacaoNf", "");
                                    });
                                </script>
                            </EditItemTemplate>
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
        <tr>
            <td align="center">
                <div id="lnkProduto" runat="server">
                    <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                        AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdutos" CssClass="gridStyle"
                        PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                        DataKeyNames="IdProdNf" OnRowDeleted="grdProdutos_RowDeleted" ShowFooter="True"
                        OnRowCommand="grdProdutos_RowCommand"
                        OnRowDataBound="grdProdutos_RowDataBound">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CausesValidation="false">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                    <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" CausesValidation="false"
                                        ImageUrl="~/Images/ExcluirGrid.gif"
                                        ToolTip="Excluir" OnClientClick="if (!confirm('Deseja excluir esse produto?')) return false"
                                        Visible='<%# ExibirRemoverProduto(Eval("IdNf")) && !(bool)Eval("TemMovimentacaoBemAtivoImob") %>' />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Image ID="imbAlerta" runat="server" ImageUrl="~/Images/alerta.png" />
                                    <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" CausesValidation="false"
                                        ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onUpdateProd();" />
                                    <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                        CausesValidation="false" ToolTip="Cancelar" />
                                    <asp:HiddenField ID="hdfIdNf" runat="server" Value='<%# Bind("IdNf") %>' />
                                </EditItemTemplate>
                                <ItemStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                                <ItemTemplate>
                                    <asp:Label ID="Label8" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                <ItemTemplate>
                                    <asp:Label ID="Label320" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                    <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                    <asp:HiddenField ID="hdfValMin" runat="server" />
                                    <asp:HiddenField ID="hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                    <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                    <asp:HiddenField ID="hdfIsAluminio" runat="server" Value='<%# Eval("IsAluminio") %>' />
                                    <asp:HiddenField ID="hdfM2Minimo" runat="server" Value='<%# Eval("M2Minimo") %>' />
                                    <asp:HiddenField ID="hdfDescrItemGenerico" runat="server"
                                        Value='<%# Bind("DescricaoItemGenerico") %>' />
                                    <asp:HiddenField ID="hdfPercComissao" runat="server" Value='<%# Bind("PercComissao") %>' />
                                    <asp:HiddenField ID="hdfAlturaBenef" runat="server" Value='<%# Bind("AlturaBenef") %>' />
                                    <asp:HiddenField ID="hdfLarguraBenef" runat="server" Value='<%# Bind("LarguraBenef") %>' />
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <uc8:ctrlSelProduto ID="ctrlSelProd" runat="server" OnLoad="ctrlSelProd_Load"
                                        Callback="selProduto" Nf="true" />
                                    <asp:HiddenField ID="hdfValMin" runat="server" />
                                    <asp:HiddenField ID="hdfIsVidro" runat="server" />
                                    <asp:HiddenField ID="hdfIsAluminio" runat="server" />
                                    <asp:HiddenField ID="hdfM2Minimo" runat="server" />
                                </FooterTemplate>
                                <ItemStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="NCM" SortExpression="Ncm">
                                <ItemTemplate>
                                    <asp:Label ID="Label15" runat="server" Text='<%# Bind("Ncm") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtNcm" runat="server" MaxLength="8" Text='<%# Bind("Ncm") %>' Width="70px"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtNcm" runat="server" MaxLength="8" Width="70px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Eval("DescrQtde") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <table cellpadding="0" cellspacing="0" class="pos">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtQtdeIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    Text='<%# Bind("Qtde") %>' Width="30px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblQtdTrib" runat="server" Text=" Qtd. Trib: "></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQtdeTrib" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    Text='<%# Bind("QtdeTrib") %>' Width="30px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <table cellpadding="0" cellspacing="0" class="pos">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtQtdeIns" runat="server" onblur="calcM2Prod();" onkeydown="if (isEnter(event)) calcM2Prod();"
                                                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    Width="30px"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblQtdeTrib" runat="server" Text="&nbsp;Qtd. Trib: " Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQtdeTrib" runat="server" onblur="calcM2Prod();" onkeydown="if (isEnter(event)) calcM2Prod();"
                                                    Style="display: none" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                    Width="30px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                <ItemTemplate>
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAlturaIns" runat="server" onblur="GetAdicionalAlturaChapa(); calcM2Prod();" Text='<%# Bind("Altura") %>'
                                        onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                        Enabled='<%# Eval("AlturaEnabled") %>' Width="35px"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtAlturaIns" runat="server" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                        onblur="GetAdicionalAlturaChapa(); calcM2Prod();" Width="35px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                <ItemTemplate>
                                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtLarguraIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, true, true);"
                                        Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="35px"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtLarguraIns" runat="server" onkeypress="return soNumeros(event, true, true);"
                                        onblur="calcM2Prod();" Width="35px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="M²" SortExpression="TotM">
                                <ItemTemplate>
                                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtTotM2" runat="server" Text='<%# Bind("TotM") %>' Width="50px"
                                        onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtTotM2Ins" runat="server" onkeypress="return soNumeros(event, false, true)"
                                        Width="50px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Vl. Unit." SortExpression="ValorVendido">
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("ValorUnitario") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtValorIns" runat="server" onblur="calcTotalProd();" onkeypress="return soNumeros(event, false, true);"
                                        Text='<%# Bind("ValorUnitario") %>' Width="50px"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtValorIns" runat="server" onkeydown="if (isEnter(event)) calcTotalProd();"
                                        onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProd();"
                                        Width="50px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="MVA" SortExpression="Mva">
                                <ItemTemplate>
                                    <asp:Label ID="Label16" runat="server" Text='<%# Bind("Mva") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtMva" runat="server" MaxLength="5" Width="50px" Text='<%# Bind("Mva") %>'
                                        onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtMva" runat="server" MaxLength="5" Width="50px" onkeypress="return soNumeros(event, false, true);"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="drpOrigCst" runat="server" DataSourceID="odsCstOrig" DataTextField="Descr"
                                        DataValueField="Id" SelectedValue='<%# Bind("CstOrig") %>' onchange="buscarDadosFci();mensagemAlertaCST();return false;">
                                        <asp:ListItem>0</asp:ListItem>
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="drpOrigCst" runat="server" DataSourceID="odsCstOrig" DataTextField="Descr"
                                        DataValueField="Id" onchange="buscarDadosFci();mensagemAlertaCST();return false;">
                                        <asp:ListItem>0</asp:ListItem>
                                        <asp:ListItem>1</asp:ListItem>
                                        <asp:ListItem>2</asp:ListItem>
                                    </asp:DropDownList>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CST" SortExpression="Cst">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("CstCompleto") %>' Style="display: block"></asp:Label>
                                    <asp:Label ID="Label361" runat="server" Text='<%# Eval("DescrPercRedBcIcms") %>'
                                        Style="display: block"></asp:Label>
                                    <asp:Label ID="Label44" runat="server" Text='<%# Eval("DescrPercRedBcIcmsSt") %>'
                                        Style="display: block"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="drpCst" runat="server" onchange="drpCst_Changed();mensagemAlertaCST();" SelectedValue='<%# Bind("Cst") %>'>
                                        <asp:ListItem></asp:ListItem>
                                        <asp:ListItem>00</asp:ListItem>
                                        <asp:ListItem>10</asp:ListItem>
                                        <asp:ListItem>20</asp:ListItem>
                                        <asp:ListItem>30</asp:ListItem>
                                        <asp:ListItem>40</asp:ListItem>
                                        <asp:ListItem>41</asp:ListItem>
                                        <asp:ListItem>50</asp:ListItem>
                                        <asp:ListItem>51</asp:ListItem>
                                        <asp:ListItem>60</asp:ListItem>
                                        <asp:ListItem>70</asp:ListItem>
                                        <asp:ListItem>90</asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <table id="percRedIcms" class="pos" style="position: absolute; border: 1px solid silver;
                                        background-color: White">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPercRedBcIcms" runat="server" Text="Perc. Red. BC ICMS" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPercRedBcIcms" runat="server" Style="display: none" Text='<%# Bind("PercRedBcIcms") %>'
                                                    Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPercRedBcIcmsSt" runat="server" Text="Perc. Red. BC ICMS ST" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPercRedBcIcmsSt" runat="server" Style="display: none" Text='<%# Bind("PercRedBcIcmsSt") %>'
                                                    Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPercDiferimento" runat="server" Text="Perc. Diferimento" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPercDiferimento" runat="server" Style="display: none" Text='<%# Bind("PercDiferimento") %>'
                                                    Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblValorIcmsDeson" runat="server" Text="Valor do ICMS Desonerado" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValorIcmsDeson" runat="server" Style="display: none" Text='<%# Bind("ValorIcmsDesonerado") %>'
                                                    Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblMotivoIcmsDeson" runat="server" Text="Motivo Desoneração" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpMotivoIcmsDeson" runat="server" Style="display: none" SelectedValue='<%# Bind("MotivoDesoneracao") %>'
                                                    Width="200px">
                                                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Táxi"></asp:ListItem>
                                                    <asp:ListItem Value="3" Text="Produtor Agropecuário"></asp:ListItem>
                                                    <asp:ListItem Value="4" Text="Frotista/Locadora"></asp:ListItem>
                                                    <asp:ListItem Value="5" Text="Diplomático/Consular"></asp:ListItem>
                                                    <asp:ListItem Value="6" Text="Utilitários e Motocicletas da Amazônia Ocidental e
                                                           Áreas de Livre Comércio (Resolução 714/88 e 790/94 – CONTRAN e suas alterações)"></asp:ListItem>
                                                    <asp:ListItem Value="7" Text="SUFRAMA"></asp:ListItem>
                                                    <asp:ListItem Value="8" Text="Venda a Órgão Público"></asp:ListItem>
                                                    <asp:ListItem Value="9" Text="Outros. (NT 2011/004)"></asp:ListItem>
                                                    <asp:ListItem Value="10" Text="Deficiente Condutor (Convênio ICMS 38/12)"></asp:ListItem>
                                                    <asp:ListItem Value="11" Text="Deficiente Não Condutor (Convênio ICMS 38/12)"></asp:ListItem>
                                                    <asp:ListItem Value="16" Text="Olimpíadas Rio 2016 (NT 2015.002)"></asp:ListItem>
                                                    <asp:ListItem Value="90" Text="Solicitado pelo Fisco"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="drpCst" runat="server" onchange="drpCst_Changed();mensagemAlertaCST();">
                                        <asp:ListItem>00</asp:ListItem>
                                        <asp:ListItem>10</asp:ListItem>
                                        <asp:ListItem>20</asp:ListItem>
                                        <asp:ListItem>30</asp:ListItem>
                                        <asp:ListItem>40</asp:ListItem>
                                        <asp:ListItem>41</asp:ListItem>
                                        <asp:ListItem>50</asp:ListItem>
                                        <asp:ListItem>51</asp:ListItem>
                                        <asp:ListItem>60</asp:ListItem>
                                        <asp:ListItem>70</asp:ListItem>
                                        <asp:ListItem>90</asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <table id="percRedIcms" class="pos" style="position: absolute; border: 1px solid silver;
                                        background-color: White">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPercRedBcIcms" runat="server" Text="Perc. Red. BC ICMS" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPercRedBcIcms" runat="server" Width="50px" Style="display: none"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPercRedBcIcmsSt" runat="server" Text="Perc. Red. BC ICMS ST" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPercRedBcIcmsSt" runat="server" Style="display: none" Text='<%# Bind("PercRedBcIcmsSt") %>'
                                                    Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPercDiferimento" runat="server" Text="Perc. Diferimento" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPercDiferimento" runat="server" Style="display: none" Text='<%# Bind("PercDiferimento") %>'
                                                    Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblValorIcmsDeson" runat="server" Text="Valor do ICMS Desonerado" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValorIcmsDeson" runat="server" Style="display: none" Text='<%# Bind("ValorIcmsDesonerado") %>'
                                                    Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblMotivoIcmsDeson" runat="server" Text="Motivo Desoneração" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="drpMotivoIcmsDeson" runat="server" Style="display: none" SelectedValue='<%# Bind("MotivoDesoneracao") %>'
                                                    Width="200px">
                                                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="3" Text="Uso na agropecuária"></asp:ListItem>
                                                    <asp:ListItem Value="7" Text="SUFRAMA"></asp:ListItem>
                                                    <asp:ListItem Value="9" Text="Outros"></asp:ListItem>
                                                    <asp:ListItem Value="12" Text="Órgão de fomento e desenv. agropecuário"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </FooterTemplate>
                                <FooterStyle Wrap="False" />
                                <ItemStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CSOSN" SortExpression="Csosn">
                                <ItemTemplate>
                                    <asp:Label ID="lblCsosnCompleto" runat="server" Text='<%# Eval("CsosnCompleto") %>' Style="display: block; white-space: nowrap;"></asp:Label>
                                    <asp:Label ID="lblCsosnDescrPercRedBcIcms" runat="server" Text='<%# Eval("DescrPercRedBcIcms") %>' Style="display: block; white-space: nowrap;"></asp:Label>
                                    <asp:Label ID="lblCsosnDescrPercRedBcIcmsSt" runat="server" Text='<%# Eval("DescrPercRedBcIcmsSt") %>' Style="display: block; white-space: nowrap;"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="drpCsosn" runat="server" DataSourceID="odsCsosn" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("Csosn") %>' Height="16px"
                                        AppendDataBoundItems="True" onchange="drpCsosn_Changed();">
                                        <asp:ListItem></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <table id="csosnPercRedIcms" class="pos" style="position: absolute; border: 1px solid silver;
                                        background-color: White">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCsosnPercRedBcIcms" runat="server" Text="Perc. Red. BC ICMS" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCsosnPercRedBcIcms" runat="server" Style="display: none" Text='<%# Bind("PercRedBcIcms") %>' Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCsosnPercRedBcIcmsSt" runat="server" Text="Perc. Red. BC ICMS ST" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCsosnPercRedBcIcmsSt" runat="server" Style="display: none" Text='<%# Bind("PercRedBcIcmsSt") %>' Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="drpCsosnIns" runat="server" DataSourceID="odsCsosn" DataTextField="Descr" DataValueField="Id" AppendDataBoundItems="True"
                                        onchange="drpCsosn_Changed();">
                                    </asp:DropDownList>
                                    <br />
                                    <table id="csosnPercRedIcms" class="pos" style="position: absolute; border: 1px solid silver; background-color: White">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCsosnPercRedBcIcms" runat="server" Text="Perc. Red. BC ICMS" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCsosnPercRedBcIcms" runat="server" Width="50px" Style="display: none"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblCsosnPercRedBcIcmsSt" runat="server" Text="Perc. Red. BC ICMS ST" Style="display: none"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCsosnPercRedBcIcmsSt" runat="server" Style="display: none" Text='<%# Bind("PercRedBcIcmsSt") %>' Width="50px"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Nat. Op." SortExpression="IdNaturezaOperacao">
                                <EditItemTemplate>
                                    <uc7:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoProd" runat="server"
                                        CodigoNaturezaOperacao='<%# Bind("IdNaturezaOperacao") %>' PermitirVazio="True"
                                        OnLoad="ctrlNaturezaOperacaoProd_Load" Callback="callbackNatOp" />
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <uc7:ctrlNaturezaOperacao ID="ctrlNaturezaOperacaoProd" runat="server"
                                        CodigoNaturezaOperacao='<%# Bind("IdNaturezaOperacao") %>' PermitirVazio="True"
                                        OnLoad="ctrlNaturezaOperacaoProd_Load" Callback="callbackNatOp" />
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label14" runat="server"
                                        Text='<%# Eval("CodNaturezaOperacao") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                <EditItemTemplate>
                                    <asp:Label ID="lblTotalIns" runat="server" Text='<%# Eval("Total") %>' OnLoad="CamposEditarManual_Load"></asp:Label>
                                    <asp:TextBox ID="txtTotalIns" runat="server" OnLoad="CamposEditarManual_Load" onkeypress="return soNumeros(event, false, true)"
                                        Text='<%# Eval("Total") %>' Width="60px" onchange="FindControl('hdfTotalIns', 'input').value = this.value"></asp:TextBox>
                                    <asp:HiddenField ID="hdfTotalIns" runat="server" Value='<%# Bind("Total") %>' />
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotalIns" runat="server"></asp:Label>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label7" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Base Calc. ICMS" SortExpression="BcIcms">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtBcIcmsIns" runat="server" onkeypress="return soNumeros(event, false, true)"
                                        Text='<%# Bind("BcIcms") %>' Width="40px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label17" runat="server" Text='<%# Bind("BcIcms") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Aliq. ICMS" SortExpression="AliqIcms">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAliqIcmsIns" runat="server" Width="40px" onkeypress="return soNumeros(event, false, true);"
                                        Text='<%# Bind("AliqIcms") %>' OnLoad="txtAliquota_Load"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtAliqIcmsIns" runat="server" onkeypress="return soNumeros(event, false, true);"
                                        Width="40px" OnLoad="txtAliquota_Load"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label10" runat="server" Text='<%# Bind("AliqIcms") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor ICMS" SortExpression="ValorIcms">
                                <ItemTemplate>
                                    <asp:Label ID="Label9" runat="server" Text='<%# Bind("ValorIcms") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblValorIcmsIns" runat="server" Text='<%# Eval("ValorIcms") %>' OnLoad="CamposEditarManual_Load"></asp:Label>
                                    <asp:TextBox ID="txtValorIcmsIns" runat="server" onkeypress="return soNumeros(event, false, true)"
                                        OnLoad="CamposEditarManual_Load" Text='<%# Bind("ValorIcms") %>' Width="40px"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="Label9" runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Base Calc. ICMS ST" SortExpression="BcIcmsSt">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtBcIcmsStIns" runat="server" onkeypress="return soNumeros(event, false, true)"
                                        Text='<%# Bind("BcIcmsSt") %>' Width="40px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label19" runat="server" Text='<%# Bind("BcIcmsSt") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Aliq. ICMS ST" SortExpression="AliqIcmsSt">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAliqIcmsStIns" runat="server" Width="40px" onkeypress="return soNumeros(event, false, true);"
                                        Text='<%# Bind("AliqIcmsSt") %>' OnLoad="txtAliquota_Load"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtAliqIcmsStIns" runat="server" onkeypress="return soNumeros(event, false, true);"
                                        Width="40px" OnLoad="txtAliquota_Load"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label18" runat="server" Text='<%# Bind("AliqIcmsSt") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Aliq. FCP" SortExpression="AliqFcp">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAliqFcp" runat="server" Width="40px" onkeypress="return soNumeros(event, false, true);"
                                        Text='<%# Bind("AliqFcp") %>' OnLoad="txtAliquota_Load"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtAliqFcp" runat="server" onkeypress="return soNumeros(event, false, true);"
                                        Width="40px" OnLoad="txtAliquota_Load"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblAliqFcp" runat="server" Text='<%# Bind("AliqFcp") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Aliq. FCP ST" SortExpression="AliqFcpSt">
                                <ItemTemplate>
                                    <asp:Label ID="lblAliqFcpSt" runat="server" Text='<%# Bind("AliqFcpSt") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAliqFcpSt" runat="server" Width="40px" onkeypress="return soNumeros(event, false, true);"
                                        Text='<%# Bind("AliqFcpSt") %>' OnLoad="txtAliquota_Load"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtAliqFcpSt" runat="server" onkeypress="return soNumeros(event, false, true);"
                                        Width="40px" OnLoad="txtAliquota_Load"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor ICMS ST" SortExpression="ValorIcmsSt">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtValorIcmsStIns" runat="server" onkeypress="return soNumeros(event, false, true)"
                                        Text='<%# Bind("ValorIcmsSt") %>' Width="40px"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label20" runat="server" Text='<%# Bind("ValorIcmsSt") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Aliq. IPI" SortExpression="AliqIpi">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAliqIpiIns" runat="server" onkeypress="return soNumeros(event, false, true);"
                                        Text='<%# Bind("AliqIpi") %>' Width="40px" OnLoad="txtAliquota_Load"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtAliqIpiIns" runat="server" onkeypress="return soNumeros(event, false, true);"
                                        Width="40px" OnLoad="txtAliquota_Load"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label11" runat="server" Text='<%# Bind("AliqIpi") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor IPI" SortExpression="ValorIpi">
                                <FooterTemplate>
                                    <asp:Label ID="lblValorIpiIns" runat="server" OnLoad="CamposEditarManual_Load"
                                        Text='<%# Eval("ValorIpi") %>'></asp:Label>
                                    <asp:TextBox ID="txtValorIpiIns" runat="server"
                                        onkeypress="return soNumeros(event, false, true)"
                                        OnLoad="CamposEditarManual_Load" Text='<%# Bind("ValorIpi") %>' Width="40px"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label12" runat="server" Text='<%# Bind("ValorIpi") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblValorIpiIns" runat="server" Text='<%# Eval("ValorIpi") %>' OnLoad="CamposEditarManual_Load"></asp:Label>
                                    <asp:TextBox ID="txtValorIpiIns" runat="server" onkeypress="return soNumeros(event, false, true)"
                                        OnLoad="CamposEditarManual_Load" Text='<%# Bind("ValorIpi") %>' Width="40px"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Lote" SortExpression="Lote">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtLote" runat="server" Text='<%# Bind("Lote") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtLote" runat="server" Text='<%# Bind("Lote") %>'></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label25" runat="server" Text='<%# Bind("Lote") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Parcela Importada">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtParcelaImportada" runat="server" Text='<%# Bind("ParcelaImportada") %>'
                                    Width="40px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtParcelaImportada" runat="server" Text='<%# Bind("ParcelaImportada") %>'
                                    Width="40px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblParcelaImportada" runat="server" Text='<%# Bind("ParcelaImportada") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Saída Interestadual">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSaidaInterestadual" runat="server" Text='<%# Bind("SaidaInterestadual") %>'
                                    Width="40px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtSaidaInterestadual" runat="server" Text='<%# Bind("SaidaInterestadual") %>'
                                    Width="40px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblSaidaInterestadual" runat="server" Text='<%# Bind("SaidaInterestadual") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Conteudo Importação">
                                <ItemTemplate>
                                    <asp:Label ID="lblCounteudoImportacao" runat="server" Text='<%# Bind("ConteudoImportacao") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                   <asp:TextBox ID="txtConteudoImportacao" runat="server" Text='<%# Bind("ConteudoImportacao") %>'
                                   Width="40px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtConteudoImportacao" runat="server" Text='<%# Bind("ConteudoImportacao") %>'
                                    Width="40px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="FCI">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtNumControleFci" runat="server" onblur="return validaFci(this);" Text='<%# Bind("NumControleFciStr") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtNumControleFci" runat="server" onblur="return validaFci(this);" Text='<%# Bind("NumControleFciStr") %>'></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblNumControleFci" runat="server" Text='<%# Bind("NumControleFciStr") %>'></asp:Label>
                                </ItemTemplate>
                                </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor IPI Devolvido" SortExpression="ValorIpiDevolv">
                                <FooterTemplate>
                                    <asp:TextBox ID="txtValorIpiDevolvidoProd" runat="server" Font-Bold="True" Visible='<%# IsNfFinalidadeDevolucao() %>'></asp:TextBox>
                                </FooterTemplate>
                                <ItemTemplate>
                                   <asp:Label ID="lblValorIpiDevolvidoProd" runat="server" Text='<%# Eval("ValorIpiDevolvido", "{0:C}") %>' Visible='<%# IsNfFinalidadeDevolucao() %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtValorIpiDevolvidoProd" runat="server" Text='<%# Bind("ValorIpiDevolvido") %>' Visible='<%# IsNfFinalidadeDevolucao() %>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <EditItemTemplate>
                                    <asp:LinkButton ID="lnkEfd" runat="server" OnClientClick="exibirInfoAdicProdNf(this, ''); return false;"
                                        Visible="<%# Glass.Configuracoes.FiscalConfig.NotaFiscalConfig.GerarEFD %>"><img border="0" src="../Images/gear_add.gif" />
                                    </asp:LinkButton>
                                    <table id="tbInfoAdicProdNf" cellspacing="0" style="display: none;">
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Total dos tributos
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtTotalTrib" runat="server" MaxLength="20" onkeypress="return soNumeros(event, false, true)"
                                                    Text='<%# Bind("ValorTotalTrib") %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Cód. Valor Fiscal ICMS
                                            </td>
                                            <td align="left" style="padding-left: 4px">
                                                <asp:DropDownList ID="ddlCodValorFiscal" runat="server" AppendDataBoundItems="True"
                                                    DataSourceID="odsCodValorFiscal" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("CodValorFiscal") %>'
                                                    onchange="ddlCodValorFiscal_change(this)" Width="200px">
                                                    <asp:ListItem Text="Selecione um código" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                CST IPI
                                            </td>
                                            <td align="left" style="padding-left: 4px">
                                                <uc5:ctrlSelPopup ID="selCstIpi" runat="server" DataSourceID="odsCstIpi" DataTextField="Descr"
                                                    DataValueField="Id" Descricao='<%# Eval("DescrCstIpi") %>' ExibirIdPopup="True"
                                                    FazerPostBackBotaoPesquisar="False" TextWidth="200px" TituloTela="Selecione o CST do IPI"
                                                    Valor='<%# Bind("CstIpi") %>'  />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold" style="padding-left: 1px">
                                                Plano Conta Contábil
                                            </td>
                                            <td align="left">
                                                <asp:DropDownList ID="drpContaContabil" runat="server" AppendDataBoundItems="True"
                                                    DataSourceID="odsPlanoContaContabil" DataTextField="Descricao" DataValueField="IdContaContabil"
                                                    SelectedValue='<%# Bind("IdContaContabil") %>'>
                                                    <asp:ListItem></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Natureza BC do Crédito
                                            </td>
                                            <td align="left" style="padding-left: 4px">
                                                <uc5:ctrlSelPopup ID="selNatBcCred" runat="server" DataSourceID="odsNaturezaBcCredito"
                                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("DescrNaturezaBcCred") %>'
                                                    FazerPostBackBotaoPesquisar="False" TextWidth="200px" TituloTela="Selecione a Natureza da Base de Cálculo do Crédito"
                                                    Valor='<%# Bind("NaturezaBcCred") %>' />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Indicador Natureza Frete
                                            </td>
                                            <td align="left" style="padding-left: 4px">
                                                <uc5:ctrlSelPopup ID="selIndNatFrete" runat="server" DataSourceID="odsIndNaturezaFrete"
                                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("DescrIndNaturezaFrete") %>'
                                                    FazerPostBackBotaoPesquisar="False" TextWidth="200px" TituloTela="Selecione o Indicador da Natureza do Frete"
                                                    Valor='<%# Bind("IndNaturezaFrete") %>' />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Tipo de Contribuição Social
                                            </td>
                                            <td align="left" style="padding-left: 4px">
                                                <uc5:ctrlSelPopup ID="selCodCont" runat="server" DataSourceID="odsCodCont" DataTextField="Descr"
                                                    DataValueField="Id" Descricao='<%# Eval("DescrCodCont") %>' FazerPostBackBotaoPesquisar="False"
                                                    TextWidth="200px" TituloTela="Selecione o Tipo de Contribuição Social" Valor='<%# Bind("CodCont") %>' />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Tipo de Crédito
                                            </td>
                                            <td align="left" style="padding-left: 4px">
                                                <uc5:ctrlSelPopup ID="selCodCred" runat="server" DataSourceID="odsCodCred" DataTextField="Descr"
                                                    DataValueField="Id" Descricao='<%# Eval("DescrCodCred") %>' FazerPostBackBotaoPesquisar="False"
                                                    TextWidth="200px" TituloTela="Selecione o Tipo de Crédito" Valor='<%# Bind("CodCred") %>' />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                CST PIS/Cofins
                                            </td>
                                            <td align="left" style="padding-left: 4px">
                                                <uc5:ctrlSelPopup ID="selCstPis" runat="server" DataSourceID="odsCstPisCofins" DataTextField="Descr"
                                                    DataValueField="Id" Descricao='<%# Eval("DescrCstPis") %>' ExibirIdPopup="True"
                                                    FazerPostBackBotaoPesquisar="False" TextWidth="200px" TituloTela="Selecione o CST do PIS"
                                                    Valor='<%# Bind("CstPis") %>' />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                BC do PIS/Cofins
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtBcPis" runat="server" onchange="calcValorPis()" onkeypress="return soNumeros(event, false, true)"
                                                    MaxLength="20" Text='<%# Bind("BcPis") %>' Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Alíquota PIS
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtAliqPis" runat="server" MaxLength="20" onchange="calcValorPis()"
                                                    onkeypress="return soNumeros(event, false, true)" Text='<%# Bind("AliqPis") %>'
                                                    Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Valor PIS
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtValorPis" runat="server" MaxLength="20" onkeypress="return soNumeros(event, false, true)"
                                                    Text='<%# Bind("ValorPis") %>' Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <%--
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                CST Cofins
                                            </td>
                                            <td align="left" style="padding-left: 4px">
                                                <uc5:ctrlSelPopup ID="selCstCofins" runat="server" DataSourceID="odsCstPisCofins"
                                                    DataTextField="Descr" DataValueField="Id" Descricao='<%# Eval("DescrCstCofins") %>'
                                                    ExibirIdPopup="True" FazerPostBackBotaoPesquisar="False" TextWidth="200px" TituloTela="Selecione o CST do Cofins"
                                                    Valor='<%# Bind("CstCofins") %>' />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                BC do Cofins
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtBcCofins" runat="server" MaxLength="20" onchange="calcValorCofins()"
                                                    onkeypress="return soNumeros(event, false, true)" Text='<%# Bind("BcCofins") %>'
                                                    Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        --%>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Alíquota Cofins
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtAliqCofins" runat="server" MaxLength="20" onchange="calcValorCofins()"
                                                    onkeypress="return soNumeros(event, false, true)" Text='<%# Bind("AliqCofins") %>'
                                                    Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Valor Cofins
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtValorCofins" runat="server" MaxLength="20" onkeypress="return soNumeros(event, false, true)"
                                                    Text='<%# Bind("ValorCofins") %>' Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style='<%= !IsNfImportacao() ? "display: none": "" %>'>
                                            <td align="left" style="font-weight: bold">
                                                Número Ato Concessionário Drawback
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtNumACDrawback" runat="server" Text='<%# Bind("NumACDrawback") %>'
                                                    Width="150px"></asp:TextBox>
                                            </td>
                                        </tr>
                                         <tr style='<%= !IsNfExportacao() ? "display: none": "" %>;'>
                                            <td colspan="2">
                                                <br />
                                                <table width="100%">
                                                    <tr align="center">
                                                        <td colspan="2"><b>Exportação</b></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Núm. Ato Concessionário Drawback</b></td>
                                                        <td align="left">
                                                            <asp:TextBox ID="NumACDrawbackExp" runat="server" Text='<%# Bind("NumACDrawback") %>'></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Núm. do Reg. de exportação.</b></td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtNumRegExportacao" runat="server" Text='<%# Bind("NumRegExportacao") %>'></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Chave acesso de exportação</b></td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtChaveAcessoExportacao" runat="server" Text='<%# Bind("ChaveAcessoExportacao") %>'></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Qtde. Exportada</b></td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtQtdeExportada" runat="server" Text='<%# Bind("QtdeExportada") %>'></asp:TextBox></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:LinkButton ID="lnkEfd" runat="server" OnClientClick="exibirInfoAdicProdNf(this, ''); return false;"
                                        Visible="<%# Glass.Configuracoes.FiscalConfig.NotaFiscalConfig.GerarEFD %>">
                                        <img border="0" src="../Images/gear_add.gif" />
                                    </asp:LinkButton>
                                    <table id="tbInfoAdicProdNf" cellspacing="0" style="display: none;">
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Total dos tributos
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtTotalTrib" runat="server" MaxLength="20" onkeypress="return soNumeros(event, false, true)"
                                                    Text='<%# Bind("ValorTotalTrib") %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Cód. Valor Fiscal ICMS
                                            </td>
                                            <td align="left" style="padding-left: 0px">
                                                <asp:DropDownList ID="ddlCodValorFiscal" runat="server" AppendDataBoundItems="True"
                                                    DataSourceID="odsCodValorFiscal" DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("CodValorFiscal") %>'
                                                    onchange="ddlCodValorFiscal_change(this)" Width="200px">
                                                    <asp:ListItem Text="Selecione um código" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                CST IPI
                                            </td>
                                            <td align="left">
                                                <uc5:ctrlSelPopup ID="selCstIpi" runat="server" DataSourceID="odsCstIpi" DataTextField="Descr"
                                                    DataValueField="Id" OnLoad="selCstIpi_Load" ExibirIdPopup="True" FazerPostBackBotaoPesquisar="False"
                                                    TextWidth="200px" TituloTela="Selecione o CST do IPI" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Plano Conta Contábil
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:DropDownList ID="drpContaContabil" runat="server" AppendDataBoundItems="True"
                                                    DataSourceID="odsPlanoContaContabil" DataTextField="Descricao" DataValueField="IdContaContabil"
                                                    SelectedValue='<%# Bind("IdContaContabil") %>'>
                                                    <asp:ListItem></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Natureza BC do Crédito
                                            </td>
                                            <td align="left">
                                                <uc5:ctrlSelPopup ID="selNatBcCred" runat="server" DataSourceID="odsNaturezaBcCredito"
                                                    DataTextField="Descr" DataValueField="Id" FazerPostBackBotaoPesquisar="False"
                                                    TextWidth="200px" TituloTela="Selecione a Natureza da Base de Cálculo do Crédito" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Indicador Natureza Frete
                                            </td>
                                            <td align="left">
                                                <uc5:ctrlSelPopup ID="selIndNatFrete" runat="server" DataSourceID="odsIndNaturezaFrete"
                                                    DataTextField="Descr" DataValueField="Id" FazerPostBackBotaoPesquisar="False"
                                                    TextWidth="200px" TituloTela="Selecione o Indicador da Natureza do Frete" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Tipo de Contribuição Social
                                            </td>
                                            <td align="left">
                                                <uc5:ctrlSelPopup ID="selCodCont" runat="server" DataSourceID="odsCodCont" DataTextField="Descr"
                                                    DataValueField="Id" OnLoad="selCodCont_Load" FazerPostBackBotaoPesquisar="False"
                                                    TextWidth="200px" TituloTela="Selecione o Tipo de Contribuição Social" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Tipo de Crédito
                                            </td>
                                            <td align="left">
                                                <uc5:ctrlSelPopup ID="selCodCred" runat="server" DataSourceID="odsCodCred" DataTextField="Descr"
                                                    DataValueField="Id" OnLoad="selCodCred_Load" FazerPostBackBotaoPesquisar="False"
                                                    TextWidth="200px" TituloTela="Selecione o Tipo de Crédito" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                CST PIS/Cofins
                                            </td>
                                            <td align="left">
                                                <uc5:ctrlSelPopup ID="selCstPis" runat="server" DataSourceID="odsCstPisCofins" DataTextField="Descr"
                                                    DataValueField="Id" OnLoad="selCstPisCofins_Load" ExibirIdPopup="True" FazerPostBackBotaoPesquisar="False"
                                                    TextWidth="200px" TituloTela="Selecione o CST do PIS" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                BC do PIS/Cofins
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtBcPis" runat="server" onchange="calcValorPis()" onkeypress="return soNumeros(event, false, true)"
                                                    MaxLength="20" Text='<%# Bind("BcPis") %>' Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Alíquota PIS
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtAliqPis" runat="server" MaxLength="20" onchange="calcValorPis()"
                                                    onkeypress="return soNumeros(event, false, true)" OnLoad="txtAliqPis_Load" Text='<%# Bind("AliqPis") %>'
                                                    Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Valor PIS
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtValorPis" runat="server" MaxLength="20" onkeypress="return soNumeros(event, false, true)"
                                                    Text='<%# Bind("ValorPis") %>' Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <%--
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                CST Cofins
                                            </td>
                                            <td align="left">
                                                <uc5:ctrlSelPopup ID="selCstCofins" runat="server" OnLoad="selCstPisCofins_Load"
                                                    DataSourceID="odsCstPisCofins" DataTextField="Descr" DataValueField="Id" ExibirIdPopup="True"
                                                    FazerPostBackBotaoPesquisar="False" TextWidth="200px" TituloTela="Selecione o CST do Cofins" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                BC do Cofins
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtBcCofins" runat="server" MaxLength="20" onchange="calcValorCofins()"
                                                    onkeypress="return soNumeros(event, false, true)" Text='<%# Bind("BcCofins") %>'
                                                    Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        --%>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Alíquota Cofins
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtAliqCofins" runat="server" MaxLength="20" onchange="calcValorCofins()"
                                                    onkeypress="return soNumeros(event, false, true)" OnLoad="txtAliqCofins_Load"
                                                    Text='<%# Bind("AliqCofins") %>' Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Valor Cofins
                                            </td>
                                            <td align="left" style="padding-left: 2px">
                                                <asp:TextBox ID="txtValorCofins" runat="server" MaxLength="20" onkeypress="return soNumeros(event, false, true)"
                                                    Text='<%# Bind("ValorCofins") %>' Enabled='<%# HabilitarPisCofins() %>'></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style='<%= !IsNfImportacao() ? "display: none": "" %>'>
                                            <td align="left" style="font-weight: bold">
                                                Número Ato Concessionário Drawback
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:TextBox ID="txtNumACDrawback" runat="server" Text='<%# Bind("NumACDrawback") %>'
                                                    Width="150px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr style='<%= !IsNfExportacao() ? "display: none": "" %>;'>
                                            <td colspan="2">
                                                <br />
                                                <table width="100%">
                                                    <tr align="center">
                                                        <td colspan="2"><b>Exportação</b></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Núm. Ato Concessionário Drawback</b></td>
                                                        <td align="left">
                                                            <asp:TextBox ID="NumACDrawbackExp" runat="server" Text='<%# Bind("NumACDrawback") %>'></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Núm. do Reg. de exportação.</b></td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtNumRegExportacao" runat="server" Text='<%# Bind("NumRegExportacao") %>'></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Chave acesso de exportação</b></td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtChaveAcessoExportacao" runat="server" Text='<%# Bind("ChaveAcessoExportacao") %>'></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Qtde. Exportada</b></td>
                                                        <td align="left">
                                                            <asp:TextBox ID="txtQtdeExportada" runat="server" Text='<%# Bind("QtdeExportada") %>'></asp:TextBox></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEfd" runat="server" OnClientClick='<%# "exibirInfoAdicProdNf(this, \"Exibir_" + Eval("IdProdNf") + "\"); return false;" %>'
                                        Visible="<%# Glass.Configuracoes.FiscalConfig.NotaFiscalConfig.GerarEFD %>">
                                        <img border="0" src="../Images/gear_add.gif" />
                                    </asp:LinkButton>
                                    <a href="#" id="lnkInfCompl" onclick="infComplProd(<%# Eval("IdProdNf") %>); return false;">
                                        <img src="../Images/Nota.gif" border="0" title="Informações adicionais"></a>
                                    <asp:ImageButton ID="imgInfoAdic" runat="server" ImageUrl="~/Images/book_go.png"
                                        OnClientClick='<%# "openInfoAdic(" + Eval("IdProdNf") + "); return false" %>'
                                        ToolTip="Dados adicionais" Visible='<%# Eval("IsNfImportacao") %>' />
                                    <table id='tbInfoAdicProdNfExibir_<%# Eval("IdProdNf") %>' cellpadding="2" cellspacing="2"
                                        style="display: none;">
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Total de tributos
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label47" runat="server" Text='<%# Eval("ValorTotalTrib") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Cód. Valor Fiscal ICMS
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label21" runat="server" Text='<%# Eval("CodValorFiscalString") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                CST IPI
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label28" runat="server" Text='<%# Bind("DescrCstIpi") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Plano Conta Contábil
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label29" runat="server" Text='<%# Bind("DescrPlanoContaContabil") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Natureza BC do Crédito
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label31" runat="server" Text='<%# Bind("DescrNaturezaBcCred") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Indicador Natureza Frete
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label32" runat="server" Text='<%# Bind("DescrIndNaturezaFrete") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Tipo de Contribuição Social
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label41" runat="server" Text='<%# Bind("DescrCodCont") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Tipo de Crédito
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label42" runat="server" Text='<%# Bind("DescrCodCred") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                CST PIS
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label33" runat="server" Text='<%# Bind("DescrCstPis") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                BC do PIS
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label34" runat="server" Text='<%# Bind("BcPis", "{0:c}") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Valor PIS
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label35" runat="server" Text='<%# Bind("ValorPis", "{0:c}") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                CST Cofins
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label36" runat="server" Text='<%# Bind("DescrCstCofins") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                BC do Cofins
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label37" runat="server" Text='<%# Bind("BcCofins", "{0:c}") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="font-weight: bold">
                                                Valor Cofins
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label38" runat="server" Text='<%# Bind("ValorCofins", "{0:c}") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr style='<%= !IsNfImportacao() ? "display: none": "" %>'>
                                            <td align="left" style="font-weight: bold">
                                                Número Ato Concessionário Drawback
                                            </td>
                                            <td align="left" style="padding-left: 3px">
                                                <asp:Label ID="Label40" runat="server" Text='<%# Bind("NumACDrawback") %>'></asp:Label>
                                            </td>
                                        </tr>
                                        <tr style='<%= !IsNfExportacao() ? "display: none": "" %>;'>
                                            <td colspan="2">
                                                <br />
                                                <table width="100%">
                                                    <tr align="center">
                                                        <td colspan="2"><b>Exportação</b></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Núm. Ato Concessionário Drawback</b></td>
                                                        <td align="left">
                                                            <asp:Label ID="Label78" runat="server" Text='<%# Eval("NumACDrawback") %>'></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Núm. do Reg. de exportação.</b></td>
                                                        <td align="left">
                                                            <asp:Label ID="Label79" runat="server" Text='<%# Eval("NumRegExportacao") %>'></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Chave acesso de exportação</b></td>
                                                        <td align="left">
                                                            <asp:Label ID="Label80" runat="server" Text='<%# Eval("ChaveAcessoExportacao") %>'></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td><b>Qtde. Exportada</b></td>
                                                        <td align="left">
                                                            <asp:Label ID="Label81" runat="server" Text='<%# Eval("QtdeExportada") %>'></asp:Label></td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                                <ItemStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <FooterTemplate>
                                    <asp:LinkButton ID="lnkInsProd" runat="server" CommandName="Insert" OnClick="lnkInsProd_Click"
                                        OnClientClick="return onSaveProd();" CausesValidation="false">
                                        <img border="0" src="../Images/ok.gif" />
                                    </asp:LinkButton>
                                    <asp:Image ID="imbAlerta" runat="server" ImageUrl="~/Images/alerta.png" />
                                </FooterTemplate>
                                <ItemTemplate>
                                    <uc3:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="ProdutoNotaFiscal" IdRegistro='<%# Eval("IdProdNf") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pgr"></PagerStyle>
                        <EditRowStyle CssClass="edit"></EditRowStyle>
                        <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    </asp:GridView>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hdfIdNf" runat="server" />
                <asp:HiddenField ID="hdfIdProd" runat="server" />
                <asp:HiddenField ID="hdfMaxNumParc" runat="server" />
                <asp:HiddenField ID="hdfTipoDocumento" runat="server" />
                <asp:HiddenField ID="hdfSimplesNacional" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNf" runat="server" DataObjectTypeName="Glass.Data.Model.NotaFiscal"
                    InsertMethod="InsertComTransacao" SelectMethod="GetElement" TypeName="Glass.Data.DAL.NotaFiscalDAO"
                    UpdateMethod="UpdateComTransacao" OnInserted="odsNf_Inserted" OnUpdated="odsNf_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTransportador" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.TransportadorDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCsosn" runat="server" SelectMethod="GetCSOSN" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCstOrig" runat="server" SelectMethod="GetOrigCST" TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutos" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosNf"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" OnDeleted="odsProdutos_Deleted"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosNfDAO"
                    InsertMethod="InsertComTransacao" UpdateMethod="UpdateComTransacao" DeleteMethod="DeleteComTransacao" OnUpdated="odsProdutos_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContas" runat="server" SelectMethod="GetPlanoContasNf"
                    TypeName="Glass.Data.DAL.PlanoContasDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfNaoVendeVidro" runat="server" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoFatura" runat="server" SelectMethod="GetTipoFaturaNF"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPeriodoIpi" runat="server" SelectMethod="GetPeriodoApuracaoIpiNF"
                    TypeName="Glass.Data.Helper.DataSources">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCstIpi" runat="server" SelectMethod="GetCstIpi" TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContaContabil" runat="server" SelectMethod="GetSorted"
                    TypeName="Glass.Data.DAL.PlanoContaContabilDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="natureza" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAjBenInc" runat="server" SelectMethod="GetForSelPopup"
                    TypeName="Glass.Data.DAL.AjusteBeneficioIncentivoDAO" >
                    <SelectParameters>
                        <asp:Parameter DefaultValue="1" Name="tipoImposto" Type="Int32" />
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNaturezaBcCredito" runat="server" SelectMethod="GetNaturezaBcCredito"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsIndNaturezaFrete" runat="server" SelectMethod="GetIndNaturezaFrete"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodCont" runat="server" SelectMethod="GetCodCont" TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodCred" runat="server" SelectMethod="GetCodCred" TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCstPisCofins" runat="server" SelectMethod="GetCstPisCofins"
                    TypeName="Glass.Data.EFD.DataSourcesEFD">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCodValorFiscal" runat="server" SelectMethod="GetCodValorFiscal"
                    TypeName="Glass.Data.Helper.DataSources" >
                    <SelectParameters>
                        <asp:QueryStringParameter Name="tipoDocumento" QueryStringField="tipo" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll"
                    TypeName="Glass.Data.DAL.LojaDAO" CacheExpirationPolicy="Absolute"
                    ConflictDetection="OverwriteChanges" MaximumRowsParameterName="" SkinID=""
                    StartRowIndexParameterName="" >
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsModalidadeFrete" runat="server"
                    TypeName="Colosoft.Translator" SelectMethod="GetTranslatesFromTypeName">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.ModalidadeFrete, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">

        // Se a empressa não vende vidros, esconde campos
        if (FindControl("hdfNaoVendeVidro", "input").value == "true" && FindControl("grdProdutos", "table") != null) {
            var tbProd = FindControl("grdProdutos", "table");
            var rows = tbProd.children[0].children;

            var colsTitle = rows[0].getElementsByTagName("th");
            colsTitle[5].style.display = "none";
            colsTitle[6].style.display = "none";
            colsTitle[7].style.display = "none";

            var k = 0;
            for (k = 1; k < rows.length; k++) {
                if (rows[k].cells[5] == null)
                    break;

                rows[k].cells[5].style.display = "none";
                rows[k].cells[6].style.display = "none";
                rows[k].cells[7].style.display = "none";
            }
        }

        if (FindControl("ctrlDataEmissao_txtData", "input") != null && FindControl("ctrlDataEmissao_txtData", "input").value == "") {
            FindControl("ctrlDataEmissao_txtData", "input").value = '<%= DateTime.Now.ToString("dd/MM/yyyy") %>';
            FindControl("ctrlDataEmissao_txtHora", "input").value = '<%= DateTime.Now.ToString("HH:mm") %>';
        }

        exibeParcelas();

        var cCodProd = FindControl("ctrlSelProd_ctrlSelProdBuscar_txtDescr", "input");
        if (cCodProd != null) cCodProd.focus();

        var drpTipoDoc = FindControl("drpTipoDocumento", "select");
        var consumidor = FindControl("hdfConsumidor", "input") == null ? "false" : FindControl("hdfConsumidor", "input").value.toLowerCase() == "true";

        if (drpTipoDoc != null && drpTipoDoc.value != "3" && drpTipoDoc.value != "4")
        {
            var txtModelo = FindControl("txtModelo", "input");

            txtModelo.value = consumidor? "65" : "55";
            txtModelo.disabled = true;
        }

        drpCst_Changed();
        drpCsosn_Changed();

        // Mostra/Esconde campos de inserção de impostos
        habilitaTxtImpostos(true);

        if (manual) {
            FindControl("lblSubtitle", "span").innerHTML = "Alteração manual de valores";
            FindControl("lblSubtitle", "span").style.color = "Red";

            if (FindControl("txtBcPis", "input") != null) {
                FindControl("txtBcPis", "input").disabled = false;
                FindControl("txtValorPis", "input").disabled = false;
                FindControl("txtValorCofins", "input").disabled = false;
                FindControl("txtAliqPis", "input").disabled = false;
                FindControl("txtAliqCofins", "input").disabled = false;
            }
        }

        if (!!FindControl("drpFormaPagto", "select"))
            formaPagtoChanged(FindControl("drpFormaPagto", "select").value);

        if ('<%= Request["tipo"]%>' != "" && FindControl("drpTipoDocumento", "select") != null)
            FindControl("drpTipoDocumento", "select").value = '<%= Request["tipo"]%>';

        if(consumidor){

            if(FindControl("lblDataSaida", "span") != null){

                FindControl("lblDataSaida", "span").style.display = "none";
                FindControl("ctrlDataSaida", "input").style.display = "none";
                FindControl("ctrlDataSaida_txtHora", "input").style.display = "none";
                FindControl("ctrlDataSaida_imgData", "input").style.display = "none";
                FindControl("fp1", "td").style.display = "none";
                FindControl("fp2", "td").style.display = "none";

                FindControl("tbGerarEtq", "table").style.display = "none";
                FindControl("tbChaveAcesso", "table").style.display = "none";

                if(FindControl("tbParc2", "table") != null){

                    FindControl("tbParc2", "table").style.display = "none";
                    FindControl("tbParcAut", "table").style.display = "none";
                }
            }
        }

        mensagemAlertaCST();

    </script>

</asp:Content>
