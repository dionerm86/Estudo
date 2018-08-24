<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CadProducao.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Producao.CadProducao"ValidateRequest="false" %>

<%@ Register Src="../../Controls/ctrlTipoPerda.ascx" TagName="ctrlTipoPerda" TagPrefix="uc1" %>
<%@ Register Src="../../Controls/ctrlRetalhoProducao.ascx" TagName="ctrlRetalhoProducao" TagPrefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Controle de Produção</title>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/Producao.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/dhtmlgoodies_calendar.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/gridView.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/m2br.dialog.producao.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>
    
    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.8.2.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jlinq/jlinq.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/jquery/jquery.utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/m2br.dialog.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/dhtmlgoodies_calendar.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/svg-pan-zoom.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

</head>
<body>
    <style title="text/css">
        .aba, .painel
        {
        }
        .aba
        {
            position: relative;
            padding-bottom: 6px;
        }
        .aba span
        {
            padding: 6px;
            margin-right: 3px;
            cursor: pointer;
            background-color: White;
        }
        .painel
        {
            border: 1px solid gray;
            vertical-align: top;
            overflow: auto;
        }
        @-webkit-keyframes@-webkit-keyframes@-webkit-keyframesblinker{from{opacity:1.0;}
            to
            {
                opacity: 0.0;
            }
        }
        .blinkText
        {
            -webkit-animation-name: blinker;
            -webkit-animation-iteration-count: infinite;
            -webkit-animation-timing-function: cubic-bezier(1.0,0,0,1.0);
            -webkit-animation-duration: 1s;
        }
        .Wrapper {
            position:absolute;
            width: 100%;
            height: 80%;

        }

        .Wrapper iframe {
            position: absolute;
            width: 99%;
            height: 80%;
        }
    </style>

    <script type="text/javascript">
        // variável criada para salvar o id do timeout da função de redirecionamento para o painel de produção.
        var idTimeoutRedirecionarPainelProducao;
        
        // Médoto que seta o timeout da função de redirecionamento para o painel de produção.
        function redirecionaPainelProducao () {
            idTimeoutRedirecionarPainelProducao = setTimeout(function() {
                redirectUrl('<%=ResolveUrl("~/Relatorios/Producao/PainelProducao.aspx?redirecionamentoProducao=true")%>'); }, (1000 * 60) * 10 );
        }
        
        // Método para zerar o timeout da função de redirecionamento, para o painel de produção, e para iniciar o contador novamente.
        function zeraTimoutRedirecionamentoPainelProducao () {
            // Zera a função que redireciona o usuário para a tela do Painel de Produção.
            clearTimeout(idTimeoutRedirecionarPainelProducao);
            // Inicia o contador para que caso o usuário fique um determinado tempo inativo o mesmo
            // seja redirecionado para a tela do Painel de Produção.
            redirecionaPainelProducao();
        }

        function alteraClasseEstilo(tagName, estiloAtual, estiloNovo) {
            var nos = document.getElementsByTagName(tagName);
            for (i = 0; i < nos.length; i++)
                if (nos[i].className == estiloAtual)
                nos[i].className = estiloNovo;
        }

        function alteraCorTela(corNova, corAtual) {
            alteraClasseEstilo("td", "tdTitulo" + corAtual, "tdTitulo" + corNova);
            alteraClasseEstilo("td", "tdConfirmacao" + corAtual, "tdConfirmacao" + corNova);
            alteraClasseEstilo("td", "title" + corAtual, "title" + corNova);
            alteraClasseEstilo("td", "subtitle" + corAtual, "subtitle" + corNova);
            alteraClasseEstilo("td", "subtitle1" + corAtual, "subtitle1" + corNova);
        }

        function keyPressPedidoNovo(e) {
            if (!isEnter(e))
                return soNumeros(e, true, true);
            else {
                FindControl("txtCodEtiqueta", "input").focus();
                return false;
            }
        }
        
        var imprimindoEtiqueta = false;
        
        // Altera situação da peça
        function atualizaSituacao(planoCorteVerificado, chamadaEtiqueta) {
            if (!validate("perda"))
                return false;

            var txtCodChapa = FindControl("txtCodChapa", "input");
            var txtNumEtiqueta = FindControl("txtCodEtiqueta", "input");
            var chkPerda = document.getElementById("chkPerda");
            
            //Se for etiqueta de abertura de fornada
            if(txtNumEtiqueta.value.toUpperCase() == "AF"){
                novaFornada();
                cOnClick("imgPesq", null);
                return;
            }

            if (txtCodChapa != null) txtCodChapa.value = corrigeLeituraEtiqueta(txtCodChapa.value);
            txtNumEtiqueta.value = corrigeLeituraEtiqueta(txtNumEtiqueta.value);
            
            if (txtCodChapa != null && txtCodChapa.value == "" && (chkPerda == null || !chkPerda.checked))
            {
                txtCodChapa.focus();
                return;
            }
            
            if (txtNumEtiqueta != null && txtNumEtiqueta.value == "")
            {
                txtNumEtiqueta.focus();
                return;
            }  
            
            //Verifica se a peça esta parada na produção
            var responsePecaParada = CadProducao.PecaParadaProducao(txtNumEtiqueta.value);
            
            if(responsePecaParada.error != null){
                alertaPadrao("Falha na leitura.", responsePecaParada.error.description, 'erro', 280, 600);
                return false;
            }     
            
            if(responsePecaParada.value.split(';')[0] == "true"){
                alertaPadrao("Falha na leitura.", "Esta peça esta com a produção parada.\n\nMotivo: "
                + responsePecaParada.value.split(';')[1], 'erro', 280, 600);
                return false;
            }
            
            if (txtCodChapa != null)
            {
                planoCorteVerificado = planoCorteVerificado == true;
                if (!planoCorteVerificado)
                {
                    //Verifica se a chapa deu saida em um pedido de revenda
                    var retornoSaidaRevenda = CadProducao.VerificaSeChapaDeuSaidaEmPedidoRevenda(txtCodChapa.value).value;

                    if (retornoSaidaRevenda == "true")
                    {
                        alertaPadrao("Plano de Corte", "Falha na leitura. A matéria-prima informada já deu saída do sistema por meio de um pedido.", 'erro', 280, 600);
                        return false;
                    }

                    var retorno = CadProducao.VerificaPlanoDeCorteDaChapa(txtCodChapa.value).value.split("\t");
                
                    if (retorno[0] == "true")
                    {
                        if (retorno[1] == "true")
                            alertaPadrao("Plano de Corte", "Falha na leitura. A matéria-prima informada já possui um plano de corte vinculado.", 'erro', 280, 600);
                        else
                            confirmPadrao("Plano de Corte", "A matéria-prima informada já possui um plano de corte vinculado.\n Continuar assim mesmo?", 'pergunta', 280, 600);
                            
                        return false;
                    }

                    if(txtNumEtiqueta.value[0].toUpperCase() == 'C'){
                        var retChapaLeitura = CadProducao.VerificaLeituraChapa(txtCodChapa.value).value.split("\t");

                        if (retChapaLeitura[0] == "true")
                        {
                            if (retChapaLeitura[1] == "true")
                                alertaPadrao("Falha na Leitura", "A matéria-prima informada já possui uma ou mais etiquetas vinculadas.", 'erro', 280, 600);
                            else
                                confirmPadrao("Falha na Leitura", "A matéria-prima informada já possui uma ou mais etiquetas vinculadas\n Continuar assim mesmo?", 'pergunta', 280, 600);
                            
                            return false;
                        }
                    }

                    if(txtNumEtiqueta.value[0].toUpperCase() != 'C' && txtNumEtiqueta.value[0].toUpperCase() != 'P')
                    {
                        var retLeituraDiasDiferentes = CadProducao.VerificaLeituraChapaDiasDiferentes(txtCodChapa.value);

                        if(retLeituraDiasDiferentes.value == "false"){
                            alertaPadrao("Falha na Leitura", "A matéria-prima informada já possui uma ou mais etiquetas vinculadas.", 'erro', 280, 600);
                            return false;
                        }

                    }
                }   
                
                var perdaChapa = chkPerda != null && chkPerda.checked && CadProducao.IsCorte(idSetor).value == "True";
                if (txtCodChapa.value != "" && perdaChapa && !confirm("A perda será marcada na matéria-prima.\nContinuar assim mesmo?"))
                    return false;
            }
            
            var idRota = FindControl("hdfIdRota", "input").value;

            // Verifica se esta peça está sendo lida em um setor que exige que seja informada uma rota
            if (FindControl("hdfInformarRota", "input").value == "true" && 
                (idRota == "" || FindControl("txtCodRota", "input").value == "")) {
                
                alertaPadrao('Falha ao Marcar Peça', "Informe a rota antes de marcar esta peça neste setor.", 'erro', 280, 600);
                
                // focaliza o botão fechar
                setTimeout(function() {
                    if (FindControl("m2brDialog-botao-1", "a") != null)
                        FindControl("m2brDialog-botao-1", "a").focus();
                }, 1000);
                
                return false;
            }

            var txtCodCavalete = FindControl("txtCodCavalete", "input");
            if(txtCodCavalete != null && txtCodCavalete.value == ""){
                txtCodCavalete.focus();
                return false;
            }

            var isLaminado = CadProducao.IsLaminado(FindControl("hdfSetor", "input").value).value;
            var isProducao = CadProducao.IsProducao(txtNumEtiqueta.value).value;
            var isProdLamComposicao = CadProducao.IsProdLamComposicao(txtNumEtiqueta.value).value;
            var isFilhoProdLamComposicao = CadProducao.IsFilhoProdLamComposicao(txtNumEtiqueta.value).value;
            var etiquetasMateriaPrima = "";
            
            if(isLaminado.toUpperCase() == "TRUE" && (isProducao.toUpperCase() == "TRUE" || isProdLamComposicao.toUpperCase() == "TRUE" || isFilhoProdLamComposicao.toUpperCase() == "TRUE")){
            
                var _verMateriaPrimaPreenchida = verificaMateriaPrimaPreenchida();
                if(!_verMateriaPrimaPreenchida.result)
                    return false;
                else
                    etiquetasMateriaPrima = _verMateriaPrimaPreenchida.etqs;

                var hdfCodEtiqueta = FindControl("hdfCodEtiqueta", "input").value;

                if(chamadaEtiqueta && txtNumEtiqueta.value != hdfCodEtiqueta){

                    var trMateriaPrima = FindControl("trMateriaPrima", "tr");
                    if(trMateriaPrima != null){
                        trMateriaPrima.remove();
                    }

                    if (imprimindoEtiqueta)
                    {
                        txtNumEtiqueta.value = "";
                        txtNumEtiqueta.focus = "";
                        FindControl("lnkImprimir", "input").style.display = "none";
                        return false;
                    }

                    var camposMateriaPrima = CadProducao.CamposMateriaPrima(txtNumEtiqueta.value);
                    
                    if(camposMateriaPrima.error != null){
                        alertaPadrao("Etiqueta", camposMateriaPrima.error.description, 'erro', 280, 600);
                        return false;
                    }

                    var tbDadosEtiqueta = FindControl("tbDadosEtiqueta", "table");
                    var trCodEtiqueta = FindControl("trCodEtiqueta", "tr");
                    var row = tbDadosEtiqueta.insertRow(trCodEtiqueta.rowIndex + 1);
                    row.setAttribute("id", "trMateriaPrima");
                    var cell = row.insertCell(0);
                    cell.innerHTML = camposMateriaPrima.value.split("$$")[0];
                    
                    setFocus("txtMateriaPrima_1");

                    if(isFilhoProdLamComposicao.toUpperCase() == "TRUE")
                    {
                        txtNumEtiqueta.value = camposMateriaPrima.value.split("$$")[1];
                        
                        var inputs = document.getElementsByTagName('input');

                        for(var i = 0; i < inputs.length; i++) {
                            if(inputs[i] != null && inputs[i].type.toLowerCase() == 'text' && inputs[i].value=="" && inputs[i].id.includes("txtMateriaPrima")) {
                                setFocus(inputs[i].id);
                            }
                        }
                    }

                    FindControl("hdfCodEtiqueta", "input").value = txtNumEtiqueta.value;

                    if(isProdLamComposicao.toUpperCase() == "TRUE" || isFilhoProdLamComposicao.toUpperCase() == "TRUE"){

                        var retPodeImprimir = CadProducao.PodeImprimir(FindControl("txtCodEtiqueta","input").value, etiquetasMateriaPrima);

                        if(retPodeImprimir.error != null){
                            alertaPadrao("Etiqueta", retPodeImprimir.error.description, 'erro', 280, 600);
                            return false;
                        }

                        if (retPodeImprimir.value.split('|')[0] == "Erro" && retPodeImprimir.value.split('|')[1] != ""){
                            if(trMateriaPrima != null){
                                trMateriaPrima.remove();
                            }
                        
                            cell.innerHTML = "";
                            txtNumEtiqueta.value = "";
                            txtNumEtiqueta.focus = "";
                            FindControl("lnkImprimir", "input").style.display = "none";

                            alert(retPodeImprimir.value.split('|')[1]);
                            return false;
                        }
                    
                        FindControl("lnkImprimir", "input").style.display = retPodeImprimir.value.split('|')[0] == "Ok" ? "block" : "none";
                    }
                }

                var _verMateriaPrimaPreenchida = verificaMateriaPrimaPreenchida();
                if(!_verMateriaPrimaPreenchida.result)
                    return false;
                else
                    etiquetasMateriaPrima = _verMateriaPrimaPreenchida.etqs;

                if (FindControl("lnkImprimir", "input").style.display != "none")
                {
                    var retPodeImprimir = CadProducao.PodeImprimir(FindControl("txtCodEtiqueta","input").value, etiquetasMateriaPrima);

                    if(retPodeImprimir.error != null){
                        alertaPadrao("Etiqueta", retPodeImprimir.error.description, 'erro', 280, 600);
                        return false;
                    }

                    if (retPodeImprimir.value.split('|')[0] == "Erro" && retPodeImprimir.value.split('|')[1] != ""){
                        alertaPadrao("Etiqueta",retPodeImprimir.value.split('|')[1],'erro',280,600);
                        return false;
                    }

                    if(retPodeImprimir.value.split('|')[0] == "Ok")
                    {
                        if (!imprimindoEtiqueta) {
                            imprimir();
                            imprimindoEtiqueta = true;
                        }
                    
                        setTimeout(function(){ atualizaSituacao(planoCorteVerificado, chamadaEtiqueta); }, 1000);
                        return false;
                    }
                }
            }

            try {
                var chkPerda = document.getElementById("chkPerda");
                var chkPerdaDefinitiva = FindControl("chkPerdaDefinitiva", "input");
                var drpTipoPerda = document.getElementById("<%= ctrlTipoPerda1.ClientID %>_drpTipoPerda");
                var drpSubtipoPerda = document.getElementById("<%= ctrlTipoPerda1.ClientID %>_drpSubtipoPerda");
                var txtObs = document.getElementById("txtObs");
                var txtPedidoNovo = document.getElementById("txtPedidoNovo");
                var hdfSetor = document.getElementById("hdfSetor");
                var hdfFunc = document.getElementById("hdfFunc");
                var temRetalho = FindControl("ctrlRetalhoProducao1", "input") != null;
                var altura = temRetalho ? FindControl("hdfAltura", "input").value : "";
                var largura = temRetalho ? FindControl("hdfLargura", "input").value : "";
                var quantidade = temRetalho ? FindControl("hdfQtde", "input").value : "";
                var observacao = temRetalho ? FindControl("hdfObservacao", "input").value : "";
                var numChapa = txtCodChapa != null ? txtCodChapa.value : "";
                var numCavalete = txtCodCavalete != null ? txtCodCavalete.value : "";
                var txtIdFornada = FindControl("txtCodFornada", "input");
                var idFornada = txtIdFornada != null ? txtIdFornada.value : "";

                var retorno = CadProducao.AtualizaSituacao(hdfFunc.value, numChapa, txtNumEtiqueta.value, hdfSetor.value, chkPerda.checked,
                    drpTipoPerda.value, drpSubtipoPerda.value, txtObs.value, txtPedidoNovo.value, idRota != "" ? idRota : "0", "false", 
                    chkPerdaDefinitiva != null ? chkPerdaDefinitiva.checked : false, altura, largura, quantidade, observacao,
                    etiquetasMateriaPrima, numCavalete, idFornada);
                                
                if (retorno.error != null){
                    alertaPadrao("Produção", retPodeImprimir.error.description, 'erro', 280, 600);
                    return false;
                }

                if (retorno.value == undefined || retorno.value == null || retorno.value == "")
                {
                    alert("Não foi possível efetuar a leitura da peça. Efetue logout do sistema, logue e tente novamente. Caso a mensagem persista, entre em contato com o suporte do software WebGlass.");
                    return false;
                }

                retorno = retorno.value.split('|');
                
                if (temRetalho)
                    limpaDadosRetalho();

                FindControl("txtCodPeca", "input").value = txtNumEtiqueta.value;

                if (retorno[0] == "Ok") {
                    FindControl("hdfDescrEtiqueta", "input").value = "Última inclusão: " + retorno[1] +
                    (document.getElementById("chkPerda").checked ? " - PERDA" : "");

                    document.getElementById("chkPerda").checked = false;

                    try
                    {
                        document.getElementById("sndOk").play();
                    }
                    catch(err) { }

                    if (FindControl("txtCodCavalete", "input") != null) 
                        FindControl("txtCodCavalete", "input").value = "";

                    cOnClick('imgPesq', null);
                }
                else {
                    alertaPadrao('Falha ao Marcar Peça', retorno[1], 'erro', 280, 600);

                    // focaliza o botão fechar
                    setTimeout(function() {
                        if (FindControl("m2brDialog-botao-1", "a") != null)
                            FindControl("m2brDialog-botao-1", "a").focus();
                    }, 1000);
                    
                    FindControl("lblDescrEtiqueta", "span").innerHTML = "";
                    FindControl("hdfDescrEtiqueta", "input").value = "";

                    if (FindControl("txtCodChapa", "input") != null) 
                        FindControl("txtCodChapa", "input").value = "";

                    if (FindControl("txtCodCavalete", "input") != null) 
                        FindControl("txtCodCavalete", "input").value = "";

                    FindControl("txtCodEtiqueta", "input").value = "";
                    FindControl("txtCodPeca", "input").value = "";

                    $('#txtCodPeca').focus();

                    return false;
                }
            }
            catch (err) {
                alert("Falha ao alterar situação da peça." + err);
            }      
        }

        function verificaMateriaPrimaPreenchida()
        {
            var _etqs = "";
            var trMateriaPrima = FindControl("trMateriaPrima", "tr");

            if(trMateriaPrima != null)
            {
                var inputs = trMateriaPrima.getElementsByTagName("input");
                        
                if (inputs != null)
                {
                    for (i = 0; i < inputs.length; i++)
                    {
                        if(inputs[i] != null && (inputs[i].value == null || inputs[i].value == ""))
                        {
                            inputs[i].focus();
                            return { result: false, etqs: _etqs };
                        }

                        _etqs += inputs[i].value + ",";
                    }
                }
            }

            return { result: true, etqs: _etqs };
        }

        function getTdFromControl(controle, classe) {
            var retorno = controle;
            while (retorno.nodeName.toLowerCase() != "td" || retorno.className != classe)
                retorno = retorno.parentNode;

            return retorno;
        }

        function alterarPerda(check, alterarOutros) {
            // Se a empresa não utiliza o redirecionamento da tela de produção para o painel de produção então o timeout não será zerado.
            if (CadProducao.RedirecionarPainelProducao().value == "true") {
                zeraTimoutRedirecionamentoPainelProducao();
            }
            
            alterarOutros = alterarOutros == false ? false : true;
            
            var trMateriaPrima = FindControl("trMateriaPrima", "tr");
                if(trMateriaPrima != null)
                    trMateriaPrima.remove();

            if (alterarOutros) {
                var chkPedidoNovo = document.getElementById("<%= chkPedidoNovo.ClientID %>");
                if (chkPedidoNovo != null) {
                    chkPedidoNovo.checked = false;
                    alterarPedidoNovo(chkPedidoNovo, false);
                }
            }

            if (document.getElementById("dadosPerda") != null)
                document.getElementById("dadosPerda").style.display = check.checked ? "" : "none";
            
            var idSetor = FindControl("hdfSetor", "input").value;
            var perdaChapa = document.getElementById("txtCodChapa"); 
            
            if (perdaChapa != null)
                perdaChapa = perdaChapa.value != "" && CadProducao.IsCorte(idSetor).value == "True";
            else
                perdaChapa = false;
            
            var corFundo = !check.checked ? "White" : "#FF5050";
            var corTexto = !check.checked ? perdaChapa ? "Green" : "Black" : "White";
            
            var texto = perdaChapa ? document.getElementById("txtCodChapa") : document.getElementById("txtCodEtiqueta");
            texto.style.backgroundColor = corFundo;
            texto.style.color = corTexto;
        }

        function alterarPedidoNovo(check, alterarOutros) {
            // Se a empresa não utiliza o redirecionamento da tela de produção para o painel de produção então o timeout não será zerado.
            if (CadProducao.RedirecionarPainelProducao().value == "true") {
                zeraTimoutRedirecionamentoPainelProducao();
            }
            
            alterarOutros = alterarOutros == false ? false : true;

            if (alterarOutros) {
                var chkPerda = document.getElementById("<%= chkPerda.ClientID %>");
                chkPerda.checked = false;
                alterarPerda(chkPerda, false);
            }

            document.getElementById("dadosPedidoNovo").style.display = check.checked ? "" : "none";

            if (check.checked)
                document.getElementById("<%= txtPedidoNovo.ClientID %>").focus();
            else {
                document.getElementById("<%= txtPedidoNovo.ClientID %>").value = "";
                document.getElementById("<%= lblProdutosPedido.ClientID %>").innerHTML = "";
            }
        }

        function produtosPedido(idPedido) {
            if (idPedido != "")
                var produtos = CadProducao.GetProdutosPedido(idPedido).value;
            else
                var produtos = "";

            document.getElementById("<%= lblProdutosPedido.ClientID %>").innerHTML = produtos;
        }

        function validaTipoPerda(val, args) {
            var isPerda = document.getElementById("chkPerda").checked;
            args.IsValid = isPerda ? args.Value != "" : true;
        }

        function validaObs(val, args) {
            var obrigarObs = <%= Glass.Configuracoes.ProducaoConfig.ObrigarMotivoPerda.ToString().ToLower() %>;
            var isPerda = document.getElementById("chkPerda").checked;
            var outros = <%= (int)Glass.Data.DAL.TipoPerdaDAO.Instance.GetIDByNomeExato("Outros") %>;
            var isOutros = FindControl("drpTipoPerda", "select").value == outros;
            args.IsValid = !obrigarObs || (isPerda && isOutros ? args.Value != "" : true);
        }

        function consultaProducao(repeating) {
            // Se a empresa não utiliza o redirecionamento da tela de produção para o painel de produção então o timeout não será zerado.
            if (CadProducao.RedirecionarPainelProducao().value == "true") {
                zeraTimoutRedirecionamentoPainelProducao();
            }
            
            var iFrame = document.getElementById("frameModuloSistema");

            if(iFrame != null){
                $(iFrame).height($(window).height() - 100);
                $(iFrame).width($("#tbMain").width());
            }

            if (!repeating) {
                document.getElementById("tbModuloSistema").style.display = "inline";
                document.getElementById("tbControleProducao").style.display = "none";

                iFrame.style.display = "inline";

                iFrame.contentWindow.location = "LstProducao.aspx?producao=1&popup=true";

                // Remove o z-index da divWrapper para que seja possível clicar nos campos de filtro do consulta produção
                document.getElementById("divWrapper").style.zIndex = "";
            }

            if (iFrame.contentDocument.body == null || iFrame.contentDocument.body.scrollHeight <= 150)
                setTimeout(function() { consultaProducao(true); }, 500);

            return false;
        }

        function painelGeral() {
            window.open('<%=ResolveUrl("~/Relatorios/Producao/PainelProducao.aspx?tipo=Geral")%>');
            return false;
        }

        function pedidosPendentesLeitura(){
            var idSetor = FindControl("hdfSetor", "input").value;
            openWindow(600, 800, "../../Relatorios/RelBase.aspx?rel=PedidosPendentesLeitura&idSetor=" + idSetor);
            return false;
        }

        function painelProducao() {
            window.open('<%=ResolveUrl("~/Relatorios/Producao/PainelProducao.aspx")%>');
            return false;
        }
 
        function painelProducaoSetores() {
            window.open('<%=ResolveUrl("~/Relatorios/Producao/PainelProducaoSetores.aspx")%>');
            return false;
        }

        function pedidoInterno(repeating){
            // Se a empresa não utiliza o redirecionamento da tela de produção para o painel de produção então o timeout não será zerado.
            if (CadProducao.RedirecionarPainelProducao().value == "true") {
                zeraTimoutRedirecionamentoPainelProducao();
            }
            
            var iFrame = document.getElementById("frameModuloSistema");

            if (!repeating) {
                document.getElementById("tbModuloSistema").style.display = "inline";
                document.getElementById("tbControleProducao").style.display = "none";

                iFrame.style.display = "inline";

                iFrame.contentWindow.location = "../../Listas/LstPedidoInterno.aspx?producao=1&popup=true";

            }

            if (iFrame.contentDocument.body == null || iFrame.contentDocument.body.scrollHeight <= 150)
                setTimeout(function() { pedidoInterno(true); }, 500);

            return false;
        }

        function setRota(codInterno) {
            FindControl("txtCodRota", "input").value = codInterno;
            loadRota();
        }

        function loadRota() {
            var retorno = CadProducao.GetRota(FindControl("txtCodRota", "input").value).value;

            if (retorno == null) {
                alert("É necessário recarregar a página para continuar a leitura das peças.");
                return true;
            }

            retorno = retorno.split("|");

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }
            else {
                FindControl("hdfIdRota", "input").value = retorno[1];
                FindControl("lblDescrRota", "span").innerHTML = retorno[2];
            }
        }
        
        function permiteMudarMenu()
        {
            for (var val in Page_Validators)
                Page_Validators[val].enabled = false;
        }
        
        function validaMateriaPrimaLaminado(controle){
        
            if(controle.value == null || controle.value == "")
                return true;
        
            var codEtiqueta = FindControl("txtCodEtiqueta", "input").value;
            var isLaminado = CadProducao.IsLaminado(FindControl("hdfSetor", "input").value).value;
            var isProducao = CadProducao.IsProducao(codEtiqueta).value;
            var isProdLamComposicao = CadProducao.IsProdLamComposicao(codEtiqueta).value;
            var trMateriaPrima = FindControl("trMateriaPrima", "tr");

            if(isLaminado.toUpperCase() == "TRUE" && (isProducao.toUpperCase() == "TRUE" || isProdLamComposicao.toUpperCase() == "TRUE") && trMateriaPrima != null){
                var inputs = trMateriaPrima.getElementsByTagName("input");
                for (i = 0; i < inputs.length; i++){
                    if(inputs[i].value != null && inputs[i].value != "" && inputs[i].id != controle.id && inputs[i].value == controle.value){
                        alertaPadrao('Falha ao Marcar Peça', 'A matéria-prima: ' + controle.value + ' já foi adicionada.', 'erro', 280, 600);
                        controle.value = "";
                        controle.focus();
                        return false;
                    }
                }
                
                controle.value = corrigeLeituraEtiqueta(controle.value);

                var response = CadProducao.ValidaMateriaPrimaLaminado(controle.value);
                
                if(response.error != null){
                    alertaPadrao('Falha ao Marcar Peça', response.error.description, 'erro', 280, 600);
                    return false;
                }
            }
            
            return true;
        }

        function imprimir(){
            // Abre tela de impressão de etiquetas
            openWindow(500, 700, '../../Relatorios/RelEtiquetas.aspx?apenasPlano=false&producao=true&etq=' + FindControl("txtCodEtiqueta","input").value);
        }

        var voltarPecaClicado = false;

        function voltarPeca(idProdPedProducao) {
            if (!voltarPecaClicado)
                voltarPecaClicado = true;
            else
                return false;

            if (!confirm('Confirma remoção desta peça desta situação?'))
            {
                voltarPecaClicado = false;
                return false;
            }

            var retornoVoltarPeca = CadProducao.VoltarPeca(idProdPedProducao);

            if (retornoVoltarPeca.error != null) {
                alert(retornoVoltarPeca.error.description);
                voltarPecaClicado = false;
                return false;
            }

            if (retornoVoltarPeca.value.split('|')[0] == "Erro")
            {
                alert(retornoVoltarPeca.value.split('|')[1]);
                voltarPecaClicado = false;
                return false;
            }

            cOnClick("imgPesq", null);
        }

        function novaFornada(){

            var hdfSetor = document.getElementById("hdfSetor");
            var hdfFunc = document.getElementById("hdfFunc");

            var retornoNovaFornada = CadProducao.NovaFornada(hdfSetor.value, hdfFunc.value);

            if (retornoNovaFornada.error != null) {
                alertaPadrao('Falha ao abrir fornada', retornoNovaFornada.error.description, 'erro', 280, 600);
                return false;
            }
        }
        
    </script>

    <audio src="../../Images/ok.wav" preload="auto" id="sndOk"></audio>
    <audio src="../../Images/error.wav" preload="auto" id="sndError"></audio>
    
    <form id="form1" runat="server" defaultbutton="block">
    <div style="display: none;">
        <div id="usrMsg">
        </div>
    </div>
    <asp:Button ID="block" runat="server" Text="Button" Style="display: none" OnClientClick="return false;" />
    <asp:HiddenField ID="hdfDescrEtiqueta" runat="server" />
    <div style="width:100%">
        <table class="main" id="tbMain" align="center" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <table width="100%" cellpadding="0" cellspacing="2">
                        <tr>
                            <td id="titulo" class="tdTituloAzul">
                                <table style="float: left; width: 100%">
                                    <tr>
                                        <td style="width: 300px; color: #0A0; font-family: Arial, Helvetica, sans-serif; font-size: 15px;">
                                            <colo:ItemDetailsView runat="server" DataSourceID="odsTotaisSetor" ID="dtvTotaisSetor" Visible="false">
                                                <ItemTemplate>
                                                    <div>
                                                        <label>Total de peças: </label>
                                                        <%# Eval("TotalPecas") %> (<%# Eval("TotalPecasM2", "{0:#0.00}") %>m²)
                                                    </div>
                                                    <div>
                                                        <label>Total de peças no momento: </label>
                                                        <%# Eval("TotalPecasMomento") %> (<%# Eval("TotalPecasMomentoM2", "{0:#0.00}") %>m²)
                                                    </div>
                                                </ItemTemplate>
                                            </colo:ItemDetailsView>
                                            <colo:VirtualObjectDataSource runat="server" ID="odsTotaisSetor"
                                                SelectMethod="ObtemTotaisSetor" culture="pt-BR" 
                                                TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO">
                                                <SelectParameters>
                                                    <asp:ControlParameter ControlID="hdfSetor" PropertyName="Value" Name="idSetor" />
                                                </SelectParameters>
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                        <td class="titleAzul">
                                            <asp:Label ID="lblTitulo" runat="server"></asp:Label>
                                            <asp:HiddenField ID="hdfTitulo" runat="server" />
                                        </td>
                                        <td align="right">
                                            <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLgout_Click" CausesValidation="False">
                                             <img border="0" src="../../Images/Logout.png" /></asp:LinkButton>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                             </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; width: 100%;">
                    <table cellpadding="0" cellspacing="0" style="padding-right: 4px; margin-left: 2px;"
                        width="100%">
                        <tr>
                            <td class="tdConfirmacaoAzul" align="left">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Table ID="tbMenu" runat="server">
                                            </asp:Table>
                                        </td>
                                        <td>
                                            <asp:LinkButton ID="lnkMensagensNaoLidas" runat="server" ToolTip="Mensagens Recebidas"
                                                Visible="false" OnClientClick="openWindow(600, 800, '../../WebGlass/Main.aspx?popup=true')"> 
                                            <img src='<%= ResolveUrl("~/Images/mail_received.png") %>' border="0" /></asp:LinkButton>
                                            <asp:LinkButton ID="lnkMensagens" runat="server" ToolTip="Mensagens Recebidas" Visible="False"
                                                OnClientClick="openWindow(600, 800, '../../WebGlass/Main.aspx?popup=true')"> 
                                            <img src='<%= ResolveUrl("~/Images/mail.png") %>' border="0" /></asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <div id="divWrapper" class="Wrapper" style="z-index: -1">
                    <table id="tbModuloSistema" width="100%" cellpadding="0" cellspacing="2" style="display: none">
                        <tr>
                            <td>
                                <iframe runat="server" id="frameModuloSistema" frameborder="0" style="display: none">
                                </iframe>
                            </td>
                        </tr>
                    </table>
                        </div>
                    <table id="tbControleProducao" class="divisor" cellpadding="0" cellspacing="2">
                        <tr>
                            <td class="tdConfirmacaoAzul" style="vertical-align: middle;" height="150">
                                <table>
                                    <tr>
                                        <td>
                                            <table id="tbLeitura">
                                                <tr>
                                                    <td align="center">
                                                        <table>
                                                            <tr>
                                                                <td class="subtitleAzul">
                                                                    <asp:Label ID="lblEtiqueta" runat="server" Text="Código Etiqueta"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <table id="tbDadosEtiqueta">
                                                            <tr>
                                                                <td colspan="2" align="center">
                                                                    <table id="tbRota" style="display: none;" runat="server">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="Label3" runat="server" Text="Rota"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtCodRota" onblur="loadRota();" runat="server" Width="70px"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Label ID="lblDescrRota" runat="server"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:LinkButton ID="lnkRota" runat="server" OnClientClick="openWindow(500, 700, '../../Utils/SelRota.aspx'); return false;">
                                                                        <img src="../../Images/Pesquisar.gif" border="0" />
                                                                                </asp:LinkButton>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="font-size: small" id="codChapa" runat="server">
                                                                    Matéria-prima
                                                                    <br />
                                                                    <asp:TextBox ID="txtCodChapa" runat="server" onkeypress="if (isEnter(event)) return atualizaSituacao(null, null);"
                                                                        Font-Size="XX-Large" Width="230px" ForeColor="Green"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr id="trCodEtiqueta">
                                                                <td style="font-size: small">
                                                                    Etiqueta
                                                                    <br />
                                                                    <asp:TextBox ID="txtCodEtiqueta" runat="server" onkeypress="if (isEnter(event)) return atualizaSituacao(null, true);"
                                                                        Font-Size="XX-Large" Width="230px"></asp:TextBox>
                                                                    <img runat="server" src="~/Images/Help.gif" title="Para leituras de faixas de etiquteta utilizar após a barra da etiqueta (número inicial da faixa = número final da faixa).
Exemplo: Etiquetas do intervalo 1111-1.1/10 até 1111-1.10/10 podem ser lidas em faixas, digitando no campo da etiqueta 1111-1.1/2=6
as etiquetas referentes à posição 1 serão lidas do item 2 até o item 6, utilizando 1111-1.1/7=9 serão lidas dos itens 7 até 9." />
                                                                    <asp:HiddenField runat="server" ID="hdfCodEtiqueta" />
                                                                </td>
                                                            </tr>
                                                             <tr>
                                                                <td style="font-size: small" id="tdCodCavalete" runat="server">
                                                                    Cavalete
                                                                    <br />
                                                                    <asp:TextBox ID="txtCodCavalete" runat="server" onkeypress="if (isEnter(event)) return atualizaSituacao(null, null);"
                                                                        Font-Size="XX-Large" Width="230px" ForeColor="Green"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="font-size: small" id="tdFornada" runat="server">
                                                                    Fornada
                                                                    <br />
                                                                    <asp:TextBox ID="txtCodFornada" runat="server"
                                                                        Font-Size="XX-Large" Width="230px" ForeColor="Green" Enabled="false"></asp:TextBox>
                                                                    <br />
                                                                    <asp:Button runat="server" ID="btnNovaFornada" Width="234px" Height="30px" Text="Nova fornada"
                                                                        style="margin-top:5px;" OnClientClick="return novaFornada();"/>
                                                                    <br /><br />
                                                                    <div style="text-align:center; color:#ed0000; font-weight:bold; font-size:20px;">
                                                                        <asp:Label runat="server" ID="lblDadosFornada"></asp:Label>
                                                                    </div>
                                                                    <br />
                                                                    <asp:GridView GridLines="None" ID="grdFornada" runat="server" SkinID="gridViewEditable"
                                                                         AutoGenerateColumns="False" DataSourceID="odsPecasFornada" DataKeyNames="IdProdPedProducao"
                                                                        EmptyDataText="Nenhuma peça encontrada." AllowPaging="True" AllowSorting="True">
                                                                        <Columns>
                                                                            <asp:TemplateField>
                                                                                <ItemTemplate>
                                                                                    <asp:ImageButton ID="imgExcluir" runat="server" ImageUrl="~/Images/arrow_undo.gif" ToolTip="Remover peça desta situação" Visible='<%# Eval("RemoverSituacaoVisible") %>'
                                                                                        OnClientClick='<%# "voltarPeca(" + Eval("IdProdPedProducao") + "); return false;"%>' />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Etiqueta">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="Label7" runat="server" Text='<%# Bind("NumEtiqueta") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Font-Size="10px" HorizontalAlign="Center"/>
                                                                               <ItemStyle Font-Size="10px" HorizontalAlign="Center" Width="68px" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Prod.">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="Label78" runat="server" Text='<%# Bind("DescrProdLargAlt") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Font-Size="10px" HorizontalAlign="Center"/>
                                                                               <ItemStyle Font-Size="10px" HorizontalAlign="Center" />
                                                                            </asp:TemplateField>
                                                                           
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPecasFornada" runat="server" SelectMethod="ObterPecasFornada"
                                                                        TypeName="Glass.Data.DAL.ProdutoPedidoProducaoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                                                        SelectCountMethod="ObterPecasFornadaCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                                                                        <SelectParameters>
                                                                            <asp:ControlParameter ControlID="txtCodFornada" PropertyName="Text" name="idFornada" />
                                                                        </SelectParameters>
                                                                    </colo:VirtualObjectDataSource>
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
                                                       <asp:ImageButton ID="lnkImprimir" runat="server" ImageUrl="~/Images/EtiquetaImprimir.png" Width="32" Height="32"
                                                           ToolTip="Imprimir Etiqueta" ClientIDMode="Static" style="display:none;" OnClientClick="atualizaSituacao(null, true);" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <table>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:CheckBox ID="chkPerda" runat="server" Text="Marcar perda?" Font-Size="Small"
                                                                        onclick="alterarPerda(this)" />&nbsp;&nbsp; <span id="pedidoNovo" runat="server"
                                                                            visible="false">
                                                                            <asp:CheckBox ID="chkPedidoNovo" runat="server" Text="Alterar pedido novo?" Font-Size="Small"
                                                                                onclick="alterarPedidoNovo(this)" />
                                                                        </span>
                                                                    <br />
                                                                </td>
                                                            </tr>
                                                            <tr id="dadosPerda" style="display: none">
                                                                <td align="center">
                                                                    <table>
                                                                        <tr>
                                                                            <td style="font-size: small">
                                                                                Motivo
                                                                            </td>
                                                                            <td style="font-size: small">
                                                                                <uc1:ctrlTipoPerda ID="ctrlTipoPerda1" runat="server" ExibirItemVazio="True" IdSetor='<%# Glass.Conversoes.StrParaIntNullable(hdfSetor.Value) %>' />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="font-size: small">
                                                                                <asp:Label ID="lblObs" runat="server" Text="Observações"></asp:Label>
                                                                                &nbsp;
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" Width="250px"
                                                                                    Font-Size="Small" ValidationGroup="perda"></asp:TextBox>
                                                                                <asp:CustomValidator ID="ctvObs" runat="server" ClientValidationFunction="validaObs"
                                                                                    Font-Size="Small" ControlToValidate="txtObs" Display="Dynamic" ErrorMessage="Digite a observação"
                                                                                    ValidateEmptyText="True" ValidationGroup="perda"></asp:CustomValidator>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2">
                                                                                <uc2:ctrlRetalhoProducao ID="ctrlRetalhoProducao1" runat="server" />
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2" align="center">
                                                                                <br />
                                                                                <div runat="server" id="perdaDefinitiva">
                                                                                    <asp:CheckBox ID="chkPerdaDefinitiva" runat="server" Text="Perda definitiva?" Font-Size="Small" />
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr id="dadosPedidoNovo" style="display: none">
                                                                <td align="center">
                                                                    <table>
                                                                        <tr>
                                                                            <td align="right" style="font-size: small">
                                                                                Núm. Pedido Novo
                                                                            </td>
                                                                            <td align="left">
                                                                                <asp:TextBox ID="txtPedidoNovo" runat="server" onchange="produtosPedido(this.value)"
                                                                                    Font-Size="Medium" onkeypress="if (isEnter(event)) produtosPedido(this.value); return keyPressPedidoNovo(event)"
                                                                                    Width="75px"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td colspan="2" align="center">
                                                                                <asp:Label ID="lblProdutosPedido" runat="server" Font-Size="Small"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
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
                                                        <asp:Label ID="lblDescrEtiqueta" runat="server" Font-Size="Small"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <asp:Button ID="imgPesq" runat="server" Text="submit" OnClientClick="permiteMudarMenu();"
                                                            OnClick="imgPesq_Click" Style="display: none;" />
                                                        <asp:HiddenField ID="hdfFunc" runat="server" />
                                                        <asp:HiddenField ID="hdfSetor" runat="server" />
                                                        <asp:HiddenField ID="hdfTempoLogin" runat="server" />
                                                        <asp:HiddenField ID="hdfSituacao" runat="server" />
                                                        <asp:HiddenField ID="hdfNumEtiqueta" runat="server" />
                                                        <asp:HiddenField ID="hdfCorTela" runat="server" />
                                                        <asp:HiddenField ID="hdfInformarRota" runat="server" />
                                                        <asp:HiddenField ID="hdfIdRota" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <table>
                                                            <tr>
                                                                <td class="subtitleAzul">
                                                                    Informações da Peça
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td nowrap="nowrap" align="center">
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                Consultar Peça
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtCodPeca" runat="server" Width="80px" onkeypress="if (isEnter(event)) {this.value = corrigeLeituraEtiqueta(this.value); cOnClick('imgPesq');}"></asp:TextBox>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top">
                                                                    <table height="200px">
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTitleCliente" runat="server" Font-Bold="True"></asp:Label>
                                                                                <asp:Label ID="lblCliente" runat="server"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTitleVendedor" runat="server" Font-Bold="True"></asp:Label>
                                                                                <asp:Label ID="lblVendedor" runat="server"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTitlePeca" runat="server" Font-Bold="True"></asp:Label>
                                                                                <asp:Label ID="lblPeca" runat="server"></asp:Label>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left">
                                                                                <asp:Label ID="lblTitleApl" runat="server" Font-Bold="True"></asp:Label>
                                                                                <asp:Label ID="lblApl" runat="server"></asp:Label>&nbsp;
                                                                                <asp:Label ID="lblTitleProc" runat="server" Font-Bold="True"></asp:Label>
                                                                                <asp:Label ID="lblProc" runat="server"></asp:Label>&nbsp;
                                                                                <asp:LinkButton ID="lnkAnexo" runat="server">
                                                            <img border="0px" src="../../Images/Clipe.gif" /></asp:LinkButton>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left">
                                                                                <table id="tbSetoresPeca" runat="server">
                                                                                    <tr>
                                                                                        <td nowrap="nowrap" style="color: #0A0; font-family: Arial, Helvetica, sans-serif;
                                                                                            font-size: 15px;" align="center">
                                                                                            Setores Passados
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td nowrap="nowrap" align="center" style="vertical-align: top;">
                                                                                            <asp:GridView ID="grdSetoresLidos" runat="server" DataSourceID="odsSetoresLidos"
                                                                                                AutoGenerateColumns="False" EmptyDataText="Aguardando leitura da peça." CellPadding="4"
                                                                                                PageSize="20" ShowHeader="False" GridLines="None">
                                                                                                <Columns>
                                                                                                    <asp:BoundField DataField="Descricao" HeaderText="Setor" SortExpression="Descricao">
                                                                                                        <ItemStyle Font-Bold="True" />
                                                                                                    </asp:BoundField>
                                                                                                    <asp:BoundField DataField="DataLeitura" HeaderText="Data" SortExpression="DataLeitura" />
                                                                                                    <asp:BoundField DataField="FuncLeitura" HeaderText="Func." SortExpression="FuncLeitura" />
                                                                                                </Columns>
                                                                                            </asp:GridView>
                                                                                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetoresLidos" runat="server" SelectMethod="ObtemSetoresLidos"
                                                                                                TypeName="Glass.Data.DAL.SetorDAO">
                                                                                                <SelectParameters>
                                                                                                    <asp:ControlParameter ControlID="txtCodPeca" Name="numEtiqueta" PropertyName="Text"
                                                                                                        Type="String" />
                                                                                                </SelectParameters>
                                                                                            </colo:VirtualObjectDataSource>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td nowrap="nowrap" style="color: #F00; font-family: Arial, Helvetica, sans-serif;
                                                                                            font-size: 15px;" align="center">
                                                                                            Setores Restantes
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td nowrap="nowrap" align="center" style="vertical-align: top;">
                                                                                            <asp:GridView ID="grdSetoresRestantes" runat="server" AutoGenerateColumns="False"
                                                                                                DataSourceID="odsSetoresRestantes" EmptyDataText="Aguardando leitura da peça."
                                                                                                GridLines="None" ShowHeader="False" CellPadding="4" PageSize="20">
                                                                                                <Columns>
                                                                                                    <asp:TemplateField HeaderText="Setor" SortExpression="Descricao">
                                                                                                        <EditItemTemplate>
                                                                                                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                                                                                                        </EditItemTemplate>
                                                                                                        <ItemTemplate>
                                                                                                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                                                                                                            <asp:Label ID="Label2" runat="server" Font-Bold="False" Text='<%# Eval("DescrBenef") %>'></asp:Label>
                                                                                                        </ItemTemplate>
                                                                                                        <ItemStyle Font-Bold="True" />
                                                                                                    </asp:TemplateField>
                                                                                                </Columns>
                                                                                            </asp:GridView>
                                                                                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetoresRestantes" runat="server" SelectMethod="ObtemSetoresRestantes"
                                                                                                TypeName="Glass.Data.DAL.SetorDAO">
                                                                                                <SelectParameters>
                                                                                                    <asp:ControlParameter ControlID="txtCodPeca" Name="numEtiqueta" PropertyName="Text"
                                                                                                        Type="String" />
                                                                                                </SelectParameters>
                                                                                            </colo:VirtualObjectDataSource>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td align="center" width="100%">
                                                                                            <asp:ImageButton ID="imgLogoCliente" runat="server" />
                                                                                        </td>                                        
                                                                                    </tr>
                                                                                    <%-- <tr>
                                                                                        <td nowrap="nowrap" align="center">
                                                                                            <asp:Label ID="lblTituloPrevistoSetorPeriodo" runat="server" Font-Bold="True" Text="Previsto ()"></asp:Label>
                                                                                            <asp:Label ID="lblPrevistoSetorPeriodo" runat="server"></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td nowrap="nowrap" align="center">
                                                                                            <asp:Label ID="lblTituloRealizadoSetorPeriodo" runat="server" Font-Bold="True" Text="Realizado ()"></asp:Label>
                                                                                            <asp:Label ID="lblRealizadoSetorPeriodo" runat="server"></asp:Label>
                                                                                        </td>
                                                                                    </tr> --%>
                                                                                </table>
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="tbImagemCompleta" runat="server">
                                                                                                <tr>
                                                                                                    <td>
                                                                                                        <asp:ImageButton ID="imgProjeto" runat="server" />
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                        <td>
                                                                                            <table id="tbImagemModelo" runat="server">
                                                                                                <tr>
                                                                                                    <td>
                                                                                                        <asp:ImageButton ID="imgModelo" runat="server" />
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
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td class="tdConfirmacaoAzul" style="vertical-align: top; width: 100%;" height="150">
                                <table width="100%">
                                    <tr>
                                        <td align="center" width="100%">
                                            <asp:Label ID="lblNumEtiquetaAcima" runat="server" Font-Bold="True" Font-Size="Medium"
                                                ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" width="100%">
                                            <asp:Label ID="lblObsProjAcima" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red"
                                                CssClass="blinkText"></asp:Label>
                                        </td>
                                    </tr>                                    
                                    <tr>
                                        <td align="center" width="100%">
                                            <asp:ImageButton ID="imgPeca" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" width="100%">
                                            <asp:Label ID="lblNumEtiquetaAbaixo" runat="server" Font-Bold="True" Font-Size="Medium"
                                                ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" width="100%">
                                            <asp:Label ID="lblObsProj" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="Red"
                                                CssClass="blinkText"></asp:Label>
                                            <br />
                                            <asp:HyperLink ID="lnkArquivoIso" runat="server" Visible="False">Arquivo ISO</asp:HyperLink>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfTeste" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>

<script type="text/javascript">

    var alertaPadrao = function(titulo, msg, tipo, altura, largura) {
        $('body').append('<a href="#" id="alerta-padrao"></a>');
        $('#alerta-padrao').m2brDialog({
            draggable: true,
            texto: msg,
            tipo: tipo,
            titulo: titulo,
            altura: altura,
            largura: largura,
            botoes: {
                1: {
                    label: 'Fechar',
                    tipo: 'fechar'
                }
            },
            loadCallback: carregouAlerta,
            unloadCallback: fechaErro
        });
        $('#alerta-padrao')
            .click()
            .remove();
    };


    var confirmPadrao = function(titulo, msg, tipo, altura, largura) {
        $('body').append('<a href="#" id="confirm-padrao"></a>');
        $('#confirm-padrao').m2brDialog({
            draggable: true,
            texto: msg,
            tipo: tipo,
            titulo: titulo,
            altura: altura,
            largura: largura,
            botoes: {
                1: {
                    label: 'Sim',
                    tipo: 'script',
                    funcao: function() { setTimeout(function() { atualizaSituacao(true) }, 10); }
                },
                2: {
                    label: 'Não',
                    tipo: 'script',
                    funcao: function() {
                        FindControl("txtCodChapa", "input").value = "";
                        FindControl("txtCodEtiqueta", "input").value = "";
                    }
                }
            },
            unloadCallback: fechaErro
        });
        $('#confirm-padrao')
            .click()
            .remove();
    };
    
    function carregouAlerta(opcoes) {
        if (opcoes.tipo == "erro") {
            try {
                document.getElementById("sndError").play();
            }
            catch (err) { }
        }
    }

    function fechaErro(opcoes) {
        $('#txtCodEtiqueta').focus();
    }

    // Conta os minutos que o usuário está logado
    var countMin = 0;

    function manterLogado() {
        try {
            countMin += 2;

            var tempoLogin = FindControl("hdfTempoLogin", "input").value;

            // Verifica se o tempo de login está dentro do tempo máximo permitido para este setor
            if (tempoLogin >= countMin)
                MetodosAjax.ManterLogado();
            else {
                MetodosAjax.Logout();
                redirectUrl(window.location.href);
            }
        }
        catch (err) {
            alert(err);
        }
    }

    // Coloca a etiqueta buscada na tela
    if (FindControl("txtCodEtiqueta", "input").value != "") {
        try {
            FindControl("lblDescrEtiqueta", "span").innerHTML = FindControl("hdfDescrEtiqueta", "input").value;
            if (FindControl("txtCodChapa", "input") != null) FindControl("txtCodChapa", "input").value = "";
            FindControl("txtCodEtiqueta", "input").value = "";

            if (CadProducao.ConsultaAntes(FindControl("hdfSetor", "input").value).value == "True") {
                FindControl("txtCodPeca", "input").value = "";
                FindControl("txtCodEtiqueta", "input").focus();
            }
        }
        catch (err) {
            alert(err);
        }
    }

    try {
        // Altera a cor da tela
        alteraCorTela(FindControl("hdfCorTela", "input").value, "Azul");

        var chkPedidoNovo = document.getElementById("<%= chkPedidoNovo.ClientID %>");

        if (chkPedidoNovo != null && chkPedidoNovo.checked) {
            alterarPedidoNovo(chkPedidoNovo);
            produtosPedido(document.getElementById("<%= txtPedidoNovo.ClientID %>").value);
        }

        //FindControl("txtCodEtiqueta", "input").focus();

        // Se o tempo de login for igual a 0, não precisa contar quanto tempo o usuário está logado.
        if (FindControl("hdfTempoLogin", "input").value == 0)
            setInterval("MetodosAjax.ManterLogado()", 600000);
        else
            setInterval("manterLogado()", 120000);

        if (FindControl("grdSetoresRestantes", "table") != null)
            FindControl("grdSetoresRestantes", "table").innerHTML = FindControl("grdSetoresRestantes", "table").innerHTML.replace("Aguardando leitura da peça.", "Nenhum setor restante.");

        // Se for reposição de peça, 
        if (FindControl("hdfSetor", "input").value == "1000") {
            alterarPerda(FindControl('chkPerda', 'input'));
            FindControl('btnMarcar', 'input').style.display = "none";
        }

        if (navigator.userAgent != null && navigator.userAgent.toString().indexOf("Firefox") == -1)
            FindControl("imgPeca", "input").style = "zoom: 1.5; -moz-transform: scale(1.5);";
    }
    catch (err) {
        alert(err);
    }

    var idSetor = FindControl("hdfSetor", "input").value;
    if (CadProducao.ConsultaAntes(idSetor).value == "True") {
        if ($('#txtCodPeca').val() == "")
            $('#txtCodPeca').focus();
        else if (CadProducao.IsCorte(idSetor).value == "True")
            $('#txtCodChapa').focus();
        else
            $('#txtCodEtiqueta').focus();
    }
    else if (CadProducao.IsCorte(idSetor).value == "True")
        $('#txtCodChapa').focus();
    else
        $('#txtCodEtiqueta').focus();

    function setFocus(controlName) {
        if (FindControl(controlName, "input") != null)
            FindControl(controlName, "input").focus();
    }
    
    // Caso a empresa utilize a função de redirecionar a tela de produção para o painel de produção então o timeout é iniciado.
    if (CadProducao.RedirecionarPainelProducao().value == "true") {
        // Inicia o contador para que após um determinado tempo a tela do marcador de produção seja redirecionada para a tela do Painel de Produção.
        if (idTimeoutRedirecionarPainelProducao != null && idTimeoutRedirecionarPainelProducao != "" &&
            idTimeoutRedirecionarPainelProducao > 0) {
            zeraTimoutRedirecionamentoPainelProducao();
        }
        else
            redirecionaPainelProducao();
            
    }      
       
</script>

</html>
