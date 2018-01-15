

<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadTrocaDev.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadTrocaDev" Title="Cadastro de Troca/Devolução" %>

<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlbenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlFormaPagto.ascx" TagName="ctrlFormaPagto" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlTipoPerda.ascx" TagName="ctrlTipoPerda" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        function alteraValorInsTroca(valor)
        {
            var campo = FindControl('Troca_hdfValorIns', 'input');
            if (campo != null)
                campo.value = valor;
        }
        
        function marcarReposicao()
        {
            var reposicao = FindControl("chkUsarPedidoReposicao", "input");
            if (!reposicao)
                return;
            
            var tipo = FindControl("drpTipoTroca", "select");
            if (!tipo)
                return;
            
            if (reposicao.checked)
            {
                tipo.disabled = true;
                tipo.value = "1";
            }
            else
                tipo.disabled = false;
        }

        var clicouInserir = false;
        
        function onInsert()
        {
            if(clicouInserir)
                return false;

            clicouInserir = true;

            if (FindControl("ctrlTipoPerda1_drpTipoPerda", "select").value == "")
            {
                alert("Informe o tipo de perda.");
                clicouInserir = false;
                return false;
            }

            if (FindControl("txtIdPedido", "input").value == "")
            {
                alert("Informe o pedido.");
                clicouInserir = false;
                return false;
            }
            
            var obs = Trim(FindControl("txtDescricao", "textarea").value);
            if (obs == "")
            {
                alert("Digite a observação.");
                clicouInserir = false;
                return false;
            }
            
            var tipo = FindControl("drpTipoTroca", "select");
            if (tipo) tipo.disabled = false;
            
            return true;
        }
    
        function onUpdate()
        {
            if (FindControl("ctrlTipoPerda1_drpTipoPerda", "select").value == "")
            {
                alert("Informe o tipo de perda.");
                return false;
            }

            if (FindControl("txtIdPedido", "input").value == "")
            {
                alert("Informe o pedido.");
                return false;
            }
            
            var obs = Trim(FindControl("txtDescricao", "textarea").value);
            if (obs == "")
            {
                alert("Digite a observação.");
                return false;
            }
            
            return true;
        }
        
        function atualizaTipo(tipo)
        {
            if (typeof tipo == "number")
                return tipo == 1 ? "Troca_" : "Novo_";
            else if (typeof tipo == "string")
                return tipo;
            else
                return "Novo_";
        }
        
        function getControleDescQtde(tipo)
        {
            tipo = atualizaTipo(tipo);
            
            var controleDescQtde = FindControl(tipo + "ctrlDescontoQtde", "div").id;
            controleDescQtde = controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_"));
                
            return eval(controleDescQtde);
        }
        
        function atualizaValMin(tipo)
        {
            tipo = atualizaTipo(tipo);
        
            var codInterno = FindControl(tipo + "txtCodProdIns", "input");
            codInterno = codInterno != null ? codInterno.value : FindControl(tipo + "lblCodProdIns", "span").innerHTML;
            
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;       
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            
            var id = tipo != "Troca_" ? FindControl("hdfIdProdTrocaDev", "input") : FindControl("hdfIdProdTrocado", "input");
            id = id != null ? id.value : "";
            
            var controleDescQtde = getControleDescQtde(tipo);
            var percDescontoQtde = controleDescQtde.PercDesconto();
            
            FindControl(tipo + "hdfValMin", "input").value = CadTrocaDev.GetValorMinimo(codInterno, tipoEntrega, idCliente, cliRevenda, id, percDescontoQtde, tipo, FindControl("lblIdPedido", "span").innerHTML).value;
        }
        
        function getCli(idCli)
        {
            var retorno = MetodosAjax.GetCli(idCli).value.split(';');
            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                FindControl("txtNumCli", "input").value = "";
                FindControl("txtNomeCliente", "input").value = "";
                return false;
            }
            
            FindControl("txtNumCli", "input").value = idCli;
            FindControl("txtNomeCliente", "input").value = retorno[1];
        }
    
        function exibirBenef(botao, tipo)
        {
            tipo = atualizaTipo(tipo);
            
            for (iTip = 0; iTip < 2; iTip++)
            {
                TagToTip(tipo + 'tbConfigVidro', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true, 
                    CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true, 
                    FIX, [botao, 9-getTableWidth(tipo + 'tbConfigVidro'), -41-getTableHeight(tipo + 'tbConfigVidro')]);
            }
        }
        
        function getPedido()
        {        
            openWindow(500, 700, "../Utils/SelPedido.aspx");
        }
        
        function setPedido(idPedido)
        {
            var pedidoReposicao = FindControl("chkUsarPedidoReposicao", "input").parentNode;
            
            FindControl("txtIdPedido", "input").value = idPedido;
            if (idPedido == "")
            {
                pedidoReposicao.style.display = "none";
                return;
            }

            var idTrocaDev = '<%= Request["idTrocaDev"] %>';
            var dadosPedido = CadTrocaDev.GetDadosPedido(idPedido, idTrocaDev).value.split(";");
            
            if (dadosPedido[0] == "Erro")
            {
                FindControl("txtIdPedido", "input").value = "";
                alert(dadosPedido[1]);
                return;
            }
            
            if (dadosPedido[2] != "")
                alert("Atenção: Já foram geradas as seguintes trocas/devoluções para este pedido: " + dadosPedido[2]);
                
            pedidoReposicao.style.display = dadosPedido[3] == "true" ? "" : "none";
            
            getCli(dadosPedido[1]);
        }
    
        function selProdutoTroca()
        {
            var idPedido = FindControl("lblIdPedido", "span").innerHTML;
            openWindow(600, 800, "../Utils/SelProdutoTroca.aspx?idPedido=" + idPedido);
        }
        
        function setProdutoTroca(idProdPed, qtde, etiquetas)
        {
            var idTrocaDev = FindControl("lblIdTrocaDev", "span").innerHTML;
            var resposta = CadTrocaDev.AddProdutoTroca(idTrocaDev, idProdPed, qtde, etiquetas).value.split(';');
            
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return false;
            }
            
            redirectUrl(window.location.href);
        }
    
        function selProdutoNovo()
        {
            var idPedido = FindControl("lblIdPedido", "span").innerHTML;
            openWindow(600, 800, "../Utils/SelProdutoNovoTroca.aspx?idPedido=" + idPedido);
        }
        
        function setProdutoNovo(idProdPed, qtde)
        {
            var idTrocaDev = FindControl("lblIdTrocaDev", "span").innerHTML;
            var resposta = CadTrocaDev.AddProdutoNovo(idTrocaDev, idProdPed, qtde).value.split(';');
            
            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                return false;
            }
            
            redirectUrl(window.location.href);
        }
        
        function setValorTotalTroca(valor, custo)
        {
            setValorTotal(valor, custo, 1);
        }
        
        function getNomeControleBenef(tipo)
        {
            tipo = atualizaTipo(tipo);
            
            var nomeControle = tipo == "Troca_" ? "<%= NomeControleBenefTrocado() %>" : "<%= NomeControleBenefNovo() %>";
            nomeControle = FindControl(nomeControle + "_tblBenef", "table");
            
            if (nomeControle == null)
                return null;
            
            nomeControle = nomeControle.id;
            return nomeControle.substr(0, nomeControle.lastIndexOf("_"));
        }
        
        function setValorTotal(valor, custo, tipo)
        {
            tipo = atualizaTipo(tipo);
            
            if (exibirControleBenef(getNomeControleBenef(tipo)))
            {
                var lblValorBenef = FindControl(tipo + "lblValorBenef", "span");
                lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
            }
        }
        
        var tipoProd = "";
        
        function getProduto(tipo)
        {
            tipoProd = atualizaTipo(tipo);
            openWindow(450, 700, '../Utils/SelProd.aspx');
        }
        
        // Função chamada após selecionar produto pelo popup
        function setProduto(codInterno) {
            try {
                FindControl(tipoProd + "txtCodProd", "input").value = codInterno;
                loadProduto(codInterno, tipoProd);
                tipoProd = "";
            }
            catch (err) {

            }
        }
        
        // Carrega dados do produto com base no código do produto passado
        function loadProduto(codInterno, tipo) {
            debugger;
            if (codInterno == "")
                return false;
                
            tipo = atualizaTipo(tipo);
            
            try {
                var controleDescQtde = getControleDescQtde(tipo);
                var percDescontoQtde = controleDescQtde.PercDesconto();
                var retorno = CadTrocaDev.GetProduto(codInterno, FindControl("hdfTipoEntrega", "input").value, FindControl("hdfCliRevenda", "input").value, FindControl("hdfIdCliente", "input").value,
                    percDescontoQtde, FindControl("hdfIdLoja", "input").value, FindControl("lblIdPedido", "span").innerHTML).value.split(';');
                
                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl(tipo + "txtCodProd", "input").value = "";
                    return false;
                }
                else if (retorno[0] == "Prod") {
                    FindControl(tipo + "hdfIdProd", "input").value = retorno[1];
                    FindControl(tipo + "txtValorIns", "input").value = retorno[3]; // Exibe no cadastro o valor mínimo do produto
                    FindControl(tipo + "hdfIsVidro", "input").value = retorno[4]; // Informa se o produto é vidro
                    FindControl(tipo + "hdfM2Minimo", "input").value = retorno[5]; // Informa se o produto possui m² mínimo
                    FindControl(tipo + "hdfTipoCalc", "input").value = retorno[7]; // Verifica como deve ser calculado o produto
                    FindControl(tipo + "hdfCustoProd", "input").value = retorno[13];
                    
                    atualizaValMin(tipo);
                    
                    qtdEstoque = retorno[6]; // Pega a quantidade disponível em estoque deste produto
                    var tipoCalc = retorno[7];

                    // Se o produto não for vidro, desabilita os textboxes largura e altura,
                    // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
                    // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
                    var cAltura = FindControl(tipo + "txtAlturaIns", "input");
                    var cLargura = FindControl(tipo + "txtLarguraIns", "input");
                    cAltura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
                    cLargura.disabled = CalcProd_DesabilitarLargura(tipoCalc);
                    cAltura.value = "";
                    cLargura.value = "";
                        
                    // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                    if (retorno[4] == "true" && FindControl(tipo + "txtEspessura", "input") != null) {
                        FindControl(tipo + "txtEspessura", "input").value = retorno[8];
                        FindControl(tipo + "txtEspessura", "input").disabled = retorno[8] != "" && retorno[8] != "0";
                    }
                    
                    var nomeControle = getNomeControleBenef(tipo);
                    
                    if (FindControl(tipo + "lnkBenef", "a") != null && nomeControle != null && nomeControle.indexOf("Inserir") > -1)
                        FindControl(tipo + "lnkBenef", "a").style.display = exibirControleBenef(nomeControle) ? "" : "none";
                        
                    FindControl(tipo + "hdfAliquotaIcmsProd", "input").value = retorno[9];
                    
                    //if (FindControl(tipo + "hdfPedidoProducao", "input").value == "true")
                    {
                        FindControl(tipo + "txtAltura", "input").value = retorno[10];
                        FindControl(tipo + "txtLargura", "input").value = retorno[11];
                    }
                }

                FindControl(tipo + "lblDescrProd", "span").innerHTML = retorno[2];
            }
            catch (err) {
                alert(err);
            }
        }

        // Se o produto sendo adicionado for ferragem e se a empresa for charneca, informa se qtd vendida
        // do produto existe no estoque
        function verificaEstoque(tipo) {
            tipo = atualizaTipo(tipo);
            
            var txtQtd = FindControl(tipo + "txtQtdeIns", "input").value;
        
            if (txtQtd != "" && parseInt(txtQtd) > parseInt(qtdEstoque))
            {
                if (qtdEstoque == 0)
                    alert("Não há nenhuma peça deste produto no estoque.");
                else
                    alert("Há apenas " + qtdEstoque + " peça(s) deste produto no estoque.");
                
                FindControl(tipo + "txtQtdeIns", "input").value = "";
                return false;
            }
        }
        
        var tipoApl = "";

        // Função chamada pelo popup de escolha da Aplicação do produto
        function setApl(idAplicacao, codInterno) {
            tipoApl = atualizaTipo(tipoApl);

            var idPedido = FindControl("lblIdPedido", "span").innerHTML;
            var verificaEtiquetaApl = MetodosAjax.VerificaEtiquetaAplicacao(idAplicacao, idPedido);
            if(verificaEtiquetaApl.error != null){

                FindControl(tipoApl + "txtAplIns", "input").value = "";
                FindControl(tipoApl + "hdfIdAplicacao", "input").value = "";

                alert(verificaEtiquetaApl.error.description);
                return false;
            }

            FindControl(tipoApl + "txtAplIns", "input").value = codInterno;
            FindControl(tipoApl + "hdfIdAplicacao", "input").value = idAplicacao;
            
            tipoApl = "";
        }

        function loadApl(codInterno, tipo) {
            tipoApl = tipo;
            
            if (codInterno == "") {
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
        
        var tipoProc = "";

        // Função chamada pelo popup de escolha do Processo do produto
        function setProc(idProcesso, codInterno, codAplicacao) {
            tipoProc = atualizaTipo(tipoProc);

            var idPedido = FindControl("lblIdPedido", "span").innerHTML;
            var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, idPedido);
            if(verificaEtiquetaProc.error != null){

                FindControl(tipoProc + "txtProcIns", "input").value = "";
                FindControl(tipoProc + "hdfIdProcesso", "input").value = "";

                setApl("", "");

                alert(verificaEtiquetaProc.error.description);
                return false;
            }

            FindControl(tipoProc + "txtProcIns", "input").value = codInterno;
            FindControl(tipoProc + "hdfIdProcesso", "input").value = idProcesso;
            
            if (codAplicacao != "")
                loadApl(codAplicacao, tipoProc);
            
            tipoProc = "";
        }

        function loadProc(codInterno, tipo) {
            tipoProc = tipo;
            
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
        
        // Chamado quando um produto está para ser inserido no pedido
        function onSaveProd(tipo) {
            if (!validate("produto"))
                return false;
            
            tipo = atualizaTipo(tipo);
            atualizaValMin(tipo);
            
            var codProd = FindControl(tipo + "txtCodProdIns", "input").value;
            var valor = FindControl(tipo + "txtValorIns", "input").value;
            var qtde = FindControl(tipo + "txtQtdeIns", "input").value;
            var altura = FindControl(tipo + "txtAlturaIns", "input").value;
            var largura = FindControl(tipo + "txtLarguraIns", "input").value;
            var valMin = FindControl(tipo + "hdfValMin", "input").value;

            valMin = new Number(valMin.replace(',', '.'));
            if (codProd == "") {
                alert("Informe o código do produto.");
                return false;
            }
            else if (valor == "0" || valor == "") {
                alert("Informe o valor vendido.");
                return false;
            }
            else if (qtde == "0" || qtde == "") {
                alert("Informe a quantidade.");
                return false;
            }
            else if (new Number(valor.replace(',', '.')) < valMin && CadTrocaDev.BloquearValorMin(tipo) == "true") {
                alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
                return false;
            }
            else if (FindControl(tipo + "txtAlturaIns", "input").disabled == false) {
                if (altura == "") {
                    alert("Informe a altura.");
                    return false;
                }    
            }
            // Se o textbox da largura estiver habilitado, deverá ser informada
            else if (FindControl(tipo + "txtLarguraIns", "input").disabled == false && largura == "") {
                alert("Informe a largura.");
                return false;
            }
            
            /*
            // Calcula o ICMS do produto
            var aliquota = FindControl(tipo + "hdfAliquotaIcmsProd", "input");
            var icms = FindControl(tipo + "hdfValorIcmsProd", "input");
            icms.value = aliquota.value > 0 ? parseFloat(valor) * (parseFloat(aliquota.value) / 100) : 0;
            */

            if (FindControl(tipo + "txtEspessura", "input") != null)
                FindControl(tipo + "txtEspessura", "input").disabled = false;
            
            FindControl(tipo + "txtAlturaIns", "input").disabled = false;
            FindControl(tipo + "txtLarguraIns", "input").disabled = false;
            
            return true;
        }

        // Função chamada quando o produto está para ser atualizado
        function onUpdateProd(tipo) {
            if (!validate("produto"))
                return false;
            
            tipo = atualizaTipo(tipo);
            atualizaValMin(tipo);
            
            if (FindControl(tipo + "txtValorIns", "input") == null)
                return true;
            
            return true;
        }
        
        // Calcula em tempo real a metragem quadrada do produto
        function calcM2Prod(tipo) {
            tipo = atualizaTipo(tipo);
            
            try {
                var idProd = FindControl(tipo + "hdfIdProd", "input").value;
                var altura = FindControl(tipo + "txtAlturaIns", "input") != null ? FindControl(tipo + "txtAlturaIns", "input").value : FindControl(tipo + "lblAlturaIns", "span").innerHTML;
                var largura = FindControl(tipo + "txtLarguraIns", "input") != null ? FindControl(tipo + "txtLarguraIns", "input").value : FindControl(tipo + "lblLarguraIns", "span").innerHTML;
                var qtde = FindControl(tipo + "txtQtdeIns", "input").value;
                var isVidro = FindControl(tipo + "hdfIsVidro", "input").value == "true";
                var tipoCalc = FindControl(tipo + "hdfTipoCalc", "input").value;
                
                if (altura == "" || largura == "" || qtde == "" || altura == "0" || (tipoCalc != 2 && tipoCalc != 10)) {
                    if (qtde != "" && qtde != "0")
                        calcTotalProd(tipo);

                    return false;
                }

                var redondo = (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked) ||
                    (FindControl("hdfRedondoAmbiente", "input") != null ? FindControl("hdfRedondoAmbiente", "input").value.toLowerCase() == "true" : false);

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

                var esp = FindControl(tipo + "txtEspessura", "input") != null ? FindControl("txtEspessura", "input").value : 0;
                
                // Calcula metro quadrado
                var idCliente = FindControl("hdfIdCliente", "input").value;
                FindControl(tipo + "lblTotM2Ins", "span").innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, "false").value;
                FindControl(tipo + "hdfTotM2Calc", "input").value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
                FindControl(tipo + "hdfTotM2CalcSemChapa", "input").value = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
                FindControl(tipo + "lblTotM2CalcIns", "span").innerHTML = FindControl(tipo + "hdfTotM2Calc", "input").value.replace('.', ',');
                
                calcTotalProd(tipo);
            }
            catch (err) {
                alert(err);
            }
        }
        
        function calcTotalProdTroca() {
            calcTotalProd(1);
        }

        // Calcula em tempo real o valor total do produto
        function calcTotalProd(tipo) {
            tipo = atualizaTipo(tipo);
            
            try {
                var valorIns = FindControl(tipo + "txtValorIns", "input");
                if (valorIns == null)
                {
                    valorIns = FindControl(tipo + "lblValorIns", "span");
                    if (valorIns == null)
                        return;
                    else
                        valorIns = valorIns.innerHTML;
                }
                else
                    valorIns = valorIns.value;
                    
                if (valorIns == "")
                    return;

                var totM2 = FindControl(tipo + "lblTotM2Ins", "span").innerHTML;
                var totM2Calc = new Number(FindControl(tipo + "hdfTotM2Calc", "input").value.replace(',', '.')).toFixed(2);
                var total = new Number(valorIns.replace(',', '.')).toFixed(2);
                var qtde = new Number(FindControl(tipo + "txtQtdeIns", "input").value.replace(',', '.'));
                var tipoCalc = FindControl(tipo + "hdfTipoCalc", "input").value;
                var m2Minimo = FindControl(tipo + "hdfM2Minimo", "input").value;
                var controleDescQtde = getControleDescQtde(tipo);
                var percDesconto = controleDescQtde.PercDesconto();
                var percDescontoAtual = controleDescQtde.PercDescontoAtual();
                
                var altura = FindControl(tipo + "txtAlturaIns", "input");
                if (altura != null)
                {
                    altura = new Number(altura.value.replace(',', '.'));
                    var largura = new Number(FindControl(tipo + "txtLarguraIns", "input").value.replace(',', '.'));
                }
                else
                {
                    altura = 0;
                    var largura = 0;
                }
                
                var retorno = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, m2Minimo, total, qtde, altura, FindControl(tipo + "txtAlturaIns", "input"), largura, true, tipoCalc, 2, 2, percDescontoAtual, percDesconto);
                if (retorno != "")
                    FindControl(tipo + "lblTotalIns", "span").innerHTML = retorno;
            }
            catch (err) {

            }
        }
        
        function receber(control)
        {
            if (!validate())
                return false;
            
            var btnCancelarReceb = FindControl("btnCancelarReceb", "input");
            control.disabled = true;
            btnCancelarReceb.disabled = true;
            
            document.getElementById("load").style.visibility = "visible";
            
            var controle = <%= ctrlFormaPagto1.ClientID %>;
            var idTrocaDev = <%= Request["idTrocaDev"] != null ? Request["idTrocaDev"] : "0" %>;
            var valoresReceb = controle.Valores();
            var formasPagto = controle.FormasPagamento();
            var contasBanco = controle.ContasBanco();
            var tiposCartao = controle.TiposCartao();
            var tiposBoleto = controle.TiposBoleto();
            var txAntecip = controle.TaxasAntecipacao();
            var juros = controle.Juros();
            var recebParcial = controle.RecebimentoParcial();
            var gerarCreditoControle = controle.GerarCredito();
            var creditoUtilizadoControle = controle.CreditoUtilizado();
            var numAutConstrucard = controle.NumeroConstrucard();
            var numParcCartoes = controle.ParcelasCartao();
            var chequesPagto = controle.Cheques();
            var depositoNaoIdentificado = controle.DepositosNaoIdentificados();
            var numAutCartao = controle.NumeroAutCartao();
            var CNI = controle.CartoesNaoIdentificados();
            
            var resposta = CadTrocaDev.Finalizar(idTrocaDev, valoresReceb, formasPagto, contasBanco, depositoNaoIdentificado, CNI, tiposCartao,
                tiposBoleto, txAntecip, juros, recebParcial, gerarCreditoControle, creditoUtilizadoControle,
                numAutConstrucard, numParcCartoes, chequesPagto, numAutCartao).value.split(";");

            if (resposta[0] == "Erro")
            {
                alert(resposta[1]);
                control.disabled = false;
                btnCancelarReceb.disabled = false;
                document.getElementById("load").style.visibility = "hidden";
                return false;
            }
            
            alert("Troca/devolução finalizada! " + resposta[1]);
            redirectUrl(resposta[2]);
            return false;
            //voltaLstTrocaDev();
        }
        
        function voltaLstTrocaDev()
        {
            redirectUrl(FindControl("hdfUrlRetorno", "input").value);
            return false;
        }
    </script>

    <table>
        <tr>
            <td align="center" colspan="2">
                <asp:DetailsView ID="dtvTroca" runat="server" AutoGenerateRows="False" DataSourceID="odsTroca"
                    GridLines="None" OnItemCommand="dtvTroca_ItemCommand" DataKeyNames="IdTrocaDevolucao">
                    <Fields>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <table cellpadding="2" cellspacing="0">
                                    <tr>
                                        <td class="dtvHeader">
                                            Cód.
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblIdTrocaDev" runat="server" Text='<%# Eval("IdTrocaDevolucao") %>'
                                                Font-Size="Medium"></asp:Label>
                                        </td>
                                        <td class="dtvHeader">
                                            Pedido
                                        </td>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="1">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtIdPedido" runat="server" Text='<%# Bind("IdPedido") %>' Width="70px"
                                                            onkeypress="return soNumeros(event, true, true)" onchange="setPedido(this.value)"
                                                            Enabled='<%# Eval("EditarPedido") %>'></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:ImageButton ID="imbPedido" runat="server" ImageAlign="AbsBottom" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="getPedido(); return false;" />
                                                    </td>
                                                    <td nowrap="nowrap">
                                                        &nbsp;
                                                        <asp:CheckBox ID="chkUsarPedidoReposicao" runat="server" Text="Usar pedidos de reposição"
                                                            Style='<%# (bool)Eval("ExibirUsarPedidoReposicao") ? "": "display: none" %>'
                                                            Checked='<%# Bind("UsarPedidoReposicao") %>' onclick="marcarReposicao()" Enabled='<%# Eval("EditarUsarPedidoReposicao") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Cliente
                                        </td>
                                        <td colspan="3" align="left">
                                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeydown="if (isEnter(event)) getCli(this.value);"
                                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this.value);"
                                                Text='<%# Bind("IdCliente") %>'></asp:TextBox>
                                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Text='<%# Eval("NomeCliente") %>'
                                                Width="250px"></asp:TextBox>
                                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=pedido'); return false;">
                                                <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Funcionário
                                        </td>
                                        <td colspan="3" align="left">
                                            <asp:DropDownList ID="drpFunc" runat="server" SelectedValue='<%# Bind("IdFunc") %>'
                                                DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc" OnDataBound="drpFunc_DataBound">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td class="dtvHeader">Setor
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpSetor" runat="server" AppendDataBoundItems="True" DataSourceID="odsSetor"
                                                DataTextField="Descricao" DataValueField="IdSetor" SelectedValue='<%# Bind("IdSetor") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Tipo
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpTipoTroca" runat="server" SelectedValue='<%# Bind("Tipo") %>'>
                                                <asp:ListItem Value="1">Troca</asp:ListItem>
                                                <asp:ListItem Value="2">Devolução</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="dtvHeader" align="left">
                                            Tipo perda
                                        </td>
                                        <td align="left">
                                            <uc3:ctrlTipoPerda ID="ctrlTipoPerda1" runat="server" ExibirItemVazio="true" IdTipoPerda='<%# Bind("IdTipoPerda") %>'
                                                IdSubtipoPerda='<%# Bind("IdSubtipoPerda") %>' PermitirVazio="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Data
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDataTroca" runat="server" Text='<%# Bind("DataTroca", "{0:d}") %>'></asp:Label>
                                        </td>
                                        <td class="dtvHeader">
                                            <asp:Label ID="lblDescrValor" runat="server" Text='<%# Eval("DescrValor") %>'></asp:Label>
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblValor" runat="server" ForeColor='<%# Eval("CorValor") %>' Text='<%# Eval("Valor") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr style='<%# (bool)Eval("PermitirAlterarCreditoGerado") ? "": "display: none" %>'>
                                        <td class="dtvHeader">
                                            Crédito Gerado
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtCreditoGerado" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                Text='<%# Bind("CreditoGerado") %>' Width="60px"></asp:TextBox>
                                        </td>
                                        <td class="dtvHeader">
                                            Data Erro
                                        </td>
                                        <td align="left">
                                            <uc4:ctrlData ID="ctrlDataErro" runat="server" ReadOnly="ReadOnly" ExibirHoras="false" DataNullable='<%# Bind("DataErro") %>'/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            <asp:Label ID="Label25" runat="server" Text='Origem da Troca / Devolução' 
                                                onload="ddlOrigem_Load"></asp:Label>
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:DropDownList ID="ddlOrigem" runat="server" DataSourceID="odsOrigem" 
                                                DataTextField="Descricao" DataValueField="idOrigemTrocaDesconto" 
                                                onload="ddlOrigem_Load" SelectedValue='<%# Bind("IdOrigemTrocaDevolucao") %>' AppendDataBoundItems="true">
                                                 <asp:ListItem Selected="True"></asp:ListItem>
                                            </asp:DropDownList>
                                            <colo:VirtualObjectDataSource ID="odsOrigem" runat="server" Culture="pt-BR" 
                                                SelectMethod="GetList" TypeName="Glass.Data.DAL.OrigemTrocaDescontoDAO">
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                        <tr>
                                            <td class="dtvHeader">
                                                Observação
                                            </td>
                                            <td align="left" colspan="3">
                                                <asp:TextBox ID="txtDescricao" runat="server" Columns="50" Rows="3" 
                                                    Text='<%# Bind("Descricao") %>' TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                                <asp:HiddenField ID="hdfCreditoGeradoMax" runat="server" Value='<%# Eval("CreditoGeradoMax") %>' />

                                <script type="text/javascript">
                                    marcarReposicao();
                                </script>

                            </EditItemTemplate>
                            <ItemTemplate>
                                <table cellpadding="4" cellspacing="0">
                                    <tr>
                                        <td class="dtvHeader">
                                            Cód.
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblIdTrocaDev" runat="server" Text='<%# Eval("IdTrocaDevolucao") %>'
                                                Font-Size="Medium"></asp:Label>
                                        </td>
                                        <td class="dtvHeader">
                                            Pedido
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblIdPedido" runat="server" Text='<%# Eval("IdPedido") %>' Font-Size="Medium"></asp:Label>
                                            <asp:Label ID="Label29" runat="server" ForeColor="Blue" Style="white-space: nowrap"
                                                Text="Usar pedidos de reposição" Visible='<%# Eval("UsarPedidoReposicao") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Cliente
                                        </td>
                                        <td colspan="3" align="left">
                                            <asp:Label ID="lblNomeCliente" runat="server" Text='<%# Eval("IdCliente") + " - " + Eval("NomeCliente") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Funcionário
                                        </td>
                                        <td colspan="3" align="left">
                                            <asp:Label ID="lblNomeFunc" runat="server" Text='<%# Eval("NomeFunc") %>'></asp:Label>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td class="dtvHeader">Setor
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label26" runat="server" Text='<%# Eval("Setor") %>'></asp:Label>
                                            </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Tipo
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescrTipo" runat="server" Text='<%# Eval("DescrTipo") %>'></asp:Label>
                                        </td>
                                        <td class="dtvHeader">
                                            Situação
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDescrSituacao" runat="server" Text='<%# Eval("DescrSituacao") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader" style="height: 20px">
                                            Data
                                        </td>
                                        <td align="left" style="height: 20px">
                                            <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataTroca", "{0:d}") %>'></asp:Label>
                                        </td>
                                        <td class="dtvHeader" style="height: 20px">
                                            <asp:Label ID="lblDescrValor" runat="server" Text='<%# Eval("DescrValor") %>'></asp:Label>
                                        </td>
                                        <td align="left" style="height: 20px">
                                            <asp:Label ID="lblValor" runat="server" Text='<%# Eval("Valor", "{0:c}") %>' ForeColor='<%# Eval("CorValor") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader" style="height: 20px">Data Erro
                                        </td>
                                        <td align="left" style="height: 20px">
                                            <asp:Label ID="Label30" runat="server" Text='<%# Eval("DataErro", "{0:d}") %>'></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            <asp:Label ID="Label25" runat="server" Text='Origem da Troca / Devolução' 
                                                onload="ddlOrigem_Load"></asp:Label>
                                        </td>
                                        <td align="left" colspan="3">
                                        <asp:Label ID="Label8" runat="server" 
                                                Text='<%# Eval("DescrOrigemTrocaDevolucao") %>' onload="ddlOrigem_Load"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Observação
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:TextBox ID="txtDescricao" runat="server" Columns="100" Rows="5" 
                                                    Text='<%# Bind("Descricao") %>' TextMode="MultiLine" ReadOnly="true"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCliente") %>' />
                                <asp:HiddenField ID="hdfCliRevenda" runat="server" Value='<%# Eval("CliRevenda") %>' />
                                <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntregaPedido") %>' />
                                <asp:HiddenField ID="hdfCreditoCliente" runat="server" Value='<%# Eval("CreditoCliente") %>' />
                                <asp:HiddenField ID="hdfValorExcedente" runat="server" Value='<%# Eval("ValorExcedente") %>' />
                                <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                            </ItemTemplate>
                            <InsertItemTemplate>
                                <table cellpadding="2" cellspacing="0">
                                    <tr>
                                        <td class="dtvHeader">
                                            Pedido
                                        </td>
                                        <td align="left">
                                            <table cellpadding="0" cellspacing="1">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtIdPedido" runat="server" Text='<%# Bind("IdPedido") %>' Width="70px"
                                                            onkeypress="return soNumeros(event, true, true)" onchange="setPedido(this.value)"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:ImageButton ID="imbPedido" runat="server" ImageAlign="AbsBottom" ImageUrl="~/Images/Pesquisar.gif"
                                                            OnClientClick="getPedido(); return false;" />
                                                    </td>
                                                    <td nowrap="nowrap">
                                                        &nbsp;
                                                        <asp:CheckBox ID="chkUsarPedidoReposicao" runat="server" Text="Usar pedidos de reposição"
                                                            Style="display: none" Checked='<%# Bind("UsarPedidoReposicao") %>' onclick="marcarReposicao()" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Cliente
                                        </td>
                                        <td colspan="3" align="left">
                                            <asp:TextBox ID="txtNumCli" runat="server" Width="50px" onkeydown="if (isEnter(event)) getCli(this.value);"
                                                onkeypress="return soNumeros(event, true, true);" onblur="getCli(this.value);"
                                                Text='<%# Bind("IdCliente") %>'></asp:TextBox>
                                            <asp:TextBox ID="txtNomeCliente" runat="server" ReadOnly="True" Text='<%# Eval("NomeCliente") %>'
                                                Width="250px"></asp:TextBox>
                                            <asp:LinkButton ID="lnkSelCliente" runat="server" OnClientClick="openWindow(590, 760, '../Utils/SelCliente.aspx?tipo=pedido'); return false;">
                                                <img src="../Images/Pesquisar.gif" border="0" /></asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Funcionário
                                        </td>
                                        <td colspan="3" align="left">
                                            <asp:DropDownList ID="drpFunc" runat="server" SelectedValue='<%# Bind("IdFunc") %>'
                                                DataSourceID="odsFuncionario" DataTextField="Nome" DataValueField="IdFunc" OnDataBound="drpFunc_DataBound">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td class="dtvHeader">Setor
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpSetor" runat="server" AppendDataBoundItems="True" DataSourceID="odsSetor"
                                                DataTextField="Descricao" DataValueField="IdSetor" SelectedValue='<%# Bind("IdSetor") %>'>
                                                <asp:ListItem></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Tipo
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpTipoTroca" runat="server" SelectedValue='<%# Bind("Tipo") %>'>
                                                <asp:ListItem Value="2">Devolução</asp:ListItem>
                                                <asp:ListItem Value="1">Troca</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="dtvHeader">
                                            Tipo Perda
                                        </td>
                                        <td align="left">
                                            <uc3:ctrlTipoPerda ID="ctrlTipoPerda1" runat="server" ExibirItemVazio="true" IdSubtipoPerda='<%# Bind("IdSubtipoPerda") %>'
                                                IdTipoPerda='<%# Bind("IdTipoPerda") %>' PermitirVazio="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                            Data
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lblDataTroca" runat="server" Text='<%# Bind("DataTroca") %>'></asp:Label>
                                        </td>
                                        <td class="dtvHeader">
                                            Data Erro
                                        </td>
                                        <td align="left">
                                            <uc4:ctrlData ID="ctrlDataErro" runat="server" ReadOnly="ReadOnly" ExibirHoras="false" DataNullable='<%# Bind("DataErro") %>'/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="dtvHeader">
                                        <asp:Label ID="Label24" runat="server" Text='Origem da Troca / Devolução' 
                                                onload="ddlOrigem_Load"></asp:Label>
                                            
                                        </td>
                                        <td align="left" colspan="3">
                                            <asp:DropDownList ID="ddlOrigem" runat="server" DataSourceID="odsOrigem" 
                                                DataTextField="Descricao" DataValueField="idOrigemTrocaDesconto" 
                                                onload="ddlOrigem_Load" SelectedValue='<%# Bind("IdOrigemTrocaDevolucao") %>' AppendDataBoundItems="true">
                                                 <asp:ListItem Selected="True"></asp:ListItem>
                                            </asp:DropDownList>
                                            <colo:VirtualObjectDataSource ID="odsOrigem" runat="server" Culture="pt-BR" 
                                                SelectMethod="GetList" TypeName="Glass.Data.DAL.OrigemTrocaDescontoDAO">
                                            </colo:VirtualObjectDataSource>
                                        </td>
                                        <tr>
                                            <td class="dtvHeader">
                                                Observação
                                            </td>
                                            <td align="left" colspan="3">
                                                <asp:TextBox ID="txtDescricao" runat="server" Columns="50" Rows="3" 
                                                    Text='<%# Bind("Descricao") %>' TextMode="MultiLine"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </tr>
                                </table>
                            </InsertItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <EditItemTemplate>
                                <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                    OnClientClick="return onUpdate();" />
                                <asp:Button ID="btnCancelar" runat="server" CommandName="Cancel" Text="Cancelar" />
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:Button ID="btnInserir" runat="server" CommandName="Insert" Text="Inserir" OnClientClick="return onInsert();" />
                                <asp:Button ID="btnCancelar" runat="server" OnClientClick="return voltaLstTrocaDev();"
                                    Text="Cancelar" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Button ID="btnEditar" runat="server" CommandName="Edit" Text="Editar" />
                                <asp:Button ID="btnFinalizar" runat="server" CommandArgument='<%# Eval("IdTrocaDevolucao") %>'
                                    CommandName="Finalizar" OnClientClick="if (!confirm(&quot;Deseja finalizar essa troca/devolução?&quot;)) return false;"
                                    Text="Finalizar" />
                                <asp:Button ID="btnVoltar" runat="server" OnClientClick="return voltaLstTrocaDev(); "
                                    Text="Voltar" />
                            </ItemTemplate>
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
        <tr id="produtos" runat="server">
            <td align="center">
                <table>
                    <tr>
                        <td align="center" valign="top">
                            <asp:LinkButton ID="lkbInserir" runat="server" OnClientClick="selProdutoTroca(); return false;">Inserir novo produto <%= GetSubtitulo().ToLower().TrimEnd('s') %></asp:LinkButton>
                            <div class="subtitle1" style="margin-top: 8px; margin-bottom: 4px">
                                Produtos
                                <%= GetSubtitulo() %>
                            </div>
                            <asp:GridView GridLines="None" ID="grdProdutosTrocados" runat="server" AllowPaging="True"
                                AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdutosTrocados"
                                DataKeyNames="IdProdTrocado" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" EmptyDataText="Ainda não há produtos para serem trocados/devolvidos."
                                ShowFooter="True" OnRowCommand="grdProdutosTrocados_RowCommand">
                                <FooterStyle Wrap="True" />
                                <Columns>
                                    <asp:TemplateField>
                                        <FooterTemplate>
                                            <select id="drpFooterVisible" style="display: none">
                                            </select></FooterTemplate>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                                <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                            <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                ToolTip="Excluir" OnClientClick="if (!confirm(&quot;Deseja excluir esse produto da troca/devolução?&quot;)) return false;" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                                ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="if (!onUpdateProd(1)) return false;" />
                                            <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                ToolTip="Cancelar" />
                                            <asp:HiddenField ID="hdfIdProdTrocado" runat="server" Value='<%# Eval("IdProdTrocado") %>' />
                                            <asp:HiddenField ID="hdfIdProdPed" runat="server" Value='<%# Bind("IdProdPed") %>' />
                                            <asp:HiddenField ID="Troca_hdfIdTrocaDevolucao" runat="server" Value='<%# Bind("IdTrocaDevolucao") %>' />
                                            <asp:HiddenField ID="Troca_hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                            <asp:HiddenField ID="Troca_hdfIsAluminio" runat="server" Value='<%# Eval("IsAluminio") %>' />
                                            <asp:HiddenField ID="Troca_hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                            <asp:HiddenField ID="Troca_hdfValMin" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfM2Minimo" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfAliquotaIcmsProd" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfValorIcmsProd" runat="server" />
                                            <asp:HiddenField ID="hdfEtiquetas" runat="server" Value='<%# Bind("Etiquetas") %>' />
                                        </EditItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                        <ItemTemplate>
                                            <asp:Label ID="Label13" runat="server" Text='<%# Eval("CodInterno") + " - " + Eval("DescrProduto") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="Troca_txtCodProdIns" runat="server" onblur="loadProduto(this.value, 1);"
                                                onkeydown="if (isEnter(event)) loadProduto(this.value, 1);" onkeypress="return !(isEnter(event));"
                                                Text='<%# Eval("CodInterno") %>' Width="50px" Visible='<%# Eval("EditarVisible") %>'></asp:TextBox>
                                            <asp:Label ID="Troca_lblCodProdIns" runat="server" Visible='<%# !(bool)Eval("EditarVisible") %>'
                                                Text='<%# Eval("CodInterno") %>'></asp:Label>
                                            <asp:Label ID="Troca_lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("EditarVisible") %>'>
                                                <a href="#" onclick="getProduto(1); return false;">
                                                    <img border="0" src="../Images/Pesquisar.gif" /></a></asp:PlaceHolder>
                                            <asp:HiddenField ID="Troca_hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                            <asp:HiddenField ID="Troca_hdfCustoProd" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="Troca_txtCodProdIns" runat="server" onblur="loadProduto(this.value, 1);"
                                                onkeydown="if (isEnter(event)) loadProduto(this.value, 1);" onkeypress="return !(isEnter(event));"
                                                Width="50px"></asp:TextBox><asp:Label ID="Troca_lblDescrProd" runat="server"></asp:Label><a
                                                    href="#" onclick="getProduto(1); return false;"><img src="../Images/Pesquisar.gif"
                                                        border="0" /></a>
                                            <asp:HiddenField ID="Troca_hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                            <asp:HiddenField ID="Troca_hdfValMin" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfIsVidro" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfTipoCalc" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfIsAluminio" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfM2Minimo" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfAliquotaIcmsProd" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfValorIcmsProd" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfCustoProd" runat="server" />
                                        </FooterTemplate>
                                        <ItemStyle Wrap="True" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                        <ItemTemplate>
                                            <asp:Label ID="Label14" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="Troca_txtQtdeIns" runat="server" onblur="calcM2Prod(1);" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('Troca_hdfTipoCalc', 'input').value), true);"
                                                Text='<%# Bind("Qtde") %>' Width="50px"></asp:TextBox>
                                            <asp:RangeValidator ID="rgvQtde" runat="server" ControlToValidate="Troca_txtQtdeIns"
                                                Display="Dynamic" ErrorMessage='<%# "Valor entre 1 e " + Eval("QtdeTrocar") %>'
                                                MaximumValue='<%# Eval("QtdeTrocar") %>' MinimumValue="1" Type="Double" ValidationGroup="produto"
                                                Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:RangeValidator>
                                            <uc2:ctrlDescontoQtde ID="Troca_ctrlDescontoQtde" runat="server" OnLoad="ctrlDescontoQtde_Load"
                                                Callback="calcTotalProdTroca" CallbackValorUnit="calcTotalProdTroca" PercDescontoQtde='<%# Bind("PercDescontoQtde")%>'
                                                ValidationGroup="produto" ValorDescontoQtde='<%# Bind("ValorDescontoQtde")%>' BloquearAlteracaoDesconto="true"
                                                ExecutarOnChangeValorUnitario="True" />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="Troca_txtQtdeIns" runat="server" onkeydown="if (isEnter(event)) calcM2Prod(1);"
                                                onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('Troca_hdfTipoCalc', 'input').value), true);"
                                                onblur="calcM2Prod(1); return verificaEstoque(1);" Width="50px"></asp:TextBox>
                                            <uc2:ctrlDescontoQtde ID="Troca_ctrlDescontoQtde" runat="server" ValidationGroup="produto"
                                                Callback="calcTotalProdTroca" CallbackValorUnit="calcTotalProdTroca" OnLoad="ctrlDescontoQtde_Load"
                                                ExecutarOnChangeValorUnitario="True" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                        <ItemTemplate>
                                            <asp:Label ID="Label15" runat="server" Text='<%# Bind("Largura") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Troca_lblLarguraIns" runat="server" Text='<%# Bind("Largura") %>'
                                                Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label>
                                            <asp:TextBox ID="Troca_txtLarguraIns" runat="server" onblur="calcM2Prod(1);" onkeypress="return soNumeros(event, true, true);"
                                                Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="50px"
                                                Visible='<%# Eval("EditarVisible") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="Troca_txtLarguraIns" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                onblur="calcM2Prod(1);" Width="50px"></asp:TextBox></FooterTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                        <ItemTemplate>
                                            <asp:Label ID="Label16" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Troca_lblAlturaIns" runat="server" Text='<%# Bind("Altura") %>' Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label>
                                            <asp:TextBox ID="Troca_txtAlturaIns" runat="server" onblur="calcM2Prod(1);" Text='<%# Bind("Altura") %>'
                                                onchange="FindControl('Troca_hdfAlturaReal', 'input').value = this.value" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('Troca_hdfTipoCalc', 'input').value), true);"
                                                Enabled='<%# Eval("AlturaEnabled") %>' Width="50px" Visible='<%# Eval("EditarVisible") %>'></asp:TextBox><asp:HiddenField
                                                    ID="Troca_hdfAlturaReal" runat="server" Value='<%# Bind("AlturaReal") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="Troca_txtAlturaIns" runat="server" onblur="calcM2Prod(1);" Width="50px"
                                                onchange="FindControl('Troca_hdfAlturaRealIns', 'input').value = this.value"
                                                onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('Troca_hdfTipoCalc', 'input').value), true);"></asp:TextBox><asp:HiddenField
                                                    ID="Troca_hdfAlturaRealIns" runat="server" />
                                        </FooterTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                        <ItemTemplate>
                                            <asp:Label ID="Label17" runat="server" Text='<%# Bind("TotM") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Troca_lblTotM2Ins" runat="server" Text='<%# Eval("TotM") %>'></asp:Label><asp:HiddenField
                                                ID="Troca_hdfTotM" runat="server" Value='<%# Eval("TotM") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="Troca_lblTotM2Ins" runat="server"></asp:Label></FooterTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotM2Calc">
                                        <EditItemTemplate>
                                            <asp:Label ID="Troca_lblTotM2CalcIns" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label><asp:HiddenField
                                                ID="Troca_hdfTotM2Calc" runat="server" Value='<%# Eval("TotM2Calc") %>' />
                                            <asp:HiddenField ID="Troca_hdfTotM2CalcSemChapa" runat="server" Value='<%# Eval("TotalM2CalcSemChapa") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="Troca_lblTotM2CalcIns" runat="server"></asp:Label><asp:HiddenField
                                                ID="Troca_hdfTotM2CalcIns" runat="server" />
                                            <asp:HiddenField ID="Troca_hdfTotM2CalcSemChapaIns" runat="server" />
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label18" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label></ItemTemplate>
                                        <HeaderStyle Wrap="True" />
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
                                        <ItemTemplate>
                                            <asp:Label ID="Label19" runat="server" Text='<%# Eval("ValorVendidoLista") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Troca_lblValorIns" runat="server" Text='<%# Eval("ValorVendido") %>'
                                                onchange="alteraValorInsTroca(this.innerHTML)" Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label>
                                            <asp:TextBox ID="Troca_txtValorIns" runat="server" onblur="calcTotalProd(1);" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Eval("ValorVendido") %>' Width="50px" onchange="alteraValorInsTroca(this.value)"
                                                Visible='<%# Eval("EditarVisible") %>'></asp:TextBox>
                                            <asp:HiddenField ID="Troca_hdfValorIns" runat="server" Value='<%# Bind("ValorVendido") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="Troca_txtValorIns" runat="server" onkeydown="if (isEnter(event)) calcTotalProd(1);"
                                                onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProd(1);"
                                                Width="50px"></asp:TextBox></FooterTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                                        <EditItemTemplate>
                                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("EditarVisible") %>'>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="Troca_txtAplIns" runat="server" onblur="loadApl(this.value, 1);"
                                                                onkeydown="if (isEnter(event)) loadApl(this.value, 1);" onkeypress="return !(isEnter(event));"
                                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="tipoApl=1; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:PlaceHolder>
                                            <asp:Label ID="Label27" runat="server" Text='<%# Eval("CodAplicacao") %>' Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label><asp:HiddenField
                                                ID="Troca_hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <table class="pos">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="Troca_txtAplIns" runat="server" onblur="loadApl(this.value, 1);"
                                                            onkeydown="if (isEnter(event)) loadApl(this.value, 1);" onkeypress="return !(isEnter(event));"
                                                            Width="30px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <a href="#" onclick="tipoApl=1; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="Troca_hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label20" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                                        <EditItemTemplate>
                                            <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%# Eval("EditarVisible") %>'>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="Troca_txtProcIns" runat="server" onblur="loadProc(this.value, 1);"
                                                                onkeydown="if (isEnter(event)) loadProc(this.value, 1);" onkeypress="return !(isEnter(event));"
                                                                Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick='tipoProc=1; openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:PlaceHolder>
                                            <asp:Label ID="Label28" runat="server" Text='<%# Eval("CodProcesso") %>' Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label><asp:HiddenField
                                                ID="Troca_hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <table class="pos">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="Troca_txtProcIns" runat="server" onblur="loadProc(this.value, 1);"
                                                            onkeydown="if (isEnter(event)) loadProc(this.value, 1);" onkeypress="return !(isEnter(event));"
                                                            Width="30px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <a href="#" onclick='tipoProc=1; openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="Troca_hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label21" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                        <ItemTemplate>
                                            <asp:Label ID="Label22" runat="server" Text='<%# Eval("TotalLista") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Troca_lblTotalIns" runat="server" Text='<%# Bind("Total") %>' Style="padding-top: 4px"></asp:Label></EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="Troca_lblTotalIns" runat="server"></asp:Label></FooterTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
                                        <EditItemTemplate>
                                            <asp:Label ID="Troca_lblValorBenef" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label></EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="Troca_lblValorBenef" runat="server"></asp:Label></FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label23" runat="server" Text='<%# Eval("ValorBenefLista") %>'></asp:Label></ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                            <asp:HiddenField ID="hdfRecalcBenef" runat="server" Value='<%# Eval("BenefVisible") %>' />
                                            <asp:LinkButton ID="Troca_lnkBenef" runat="server" OnClientClick="exibirBenef(this, 1); return false;"
                                                Visible='<%# Eval("BenefVisible") %>'>
                                                <img border="0" src="../Images/gear_add.gif" />
                                            </asp:LinkButton>
                                            <table id="Troca_tbConfigVidro" cellspacing="0" style="display: none;">
                                                <tr align="left">
                                                    <td align="center">
                                                        <table>
                                                            <tr>
                                                                <td class="dtvFieldBold">
                                                                    Espessura
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="Troca_txtEspessura" runat="server" OnDataBinding="txtEspessura_DataBinding"
                                                                        onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Bind("Espessura") %>'></asp:TextBox>
                                                                </td>
                                                                <td class="dtvFieldBold">
                                                                    Ped. Cli
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="Troca_txtPedCli" runat="server" MaxLength="50" Width="50px" Text='<%# Bind("PedCli") %>'></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <uc4:ctrlbenef ID="Troca_ctrlBenefEditar" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>'
                                                            ValidationGroup="produto" OnLoad="ctrlBenef_Load" Redondo='<%# Bind("Redondo") %>'
                                                            CallbackCalculoValorTotal="setValorTotalTroca" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:LinkButton ID="Troca_lnkBenef" runat="server" Style="display: none;" OnClientClick="exibirBenef(this, 1); return false;">
                                                <img border="0" src="../Images/gear_add.gif" />
                                            </asp:LinkButton>
                                            <table id="Troca_tbConfigVidro" cellspacing="0" style="display: none;">
                                                <tr align="left">
                                                    <td align="center">
                                                        <table>
                                                            <tr>
                                                                <td class="dtvFieldBold">
                                                                    Espessura
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="Troca_txtEspessura" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Width="30px"></asp:TextBox>
                                                                </td>
                                                                <td class="dtvFieldBold">
                                                                    Ped. Cli
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="Troca_txtPedCli" runat="server" MaxLength="50" Width="50px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <uc4:ctrlbenef ID="Troca_ctrlBenefInserir" runat="server" OnLoad="ctrlBenef_Load"
                                                            CallbackCalculoValorTotal="setValorTotalTroca" ValidationGroup="produto" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                    </td>
                                                </tr>
                                            </table>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Alterar Estoque?" SortExpression="AlterarEstoque">
                                        <EditItemTemplate>
                                            <asp:CheckBox ID="CheckBox3" runat="server" Checked='<%# Bind("AlterarEstoque") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:CheckBox ID="Troca_chkAlterarEstoque" runat="server" Checked='True' />
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="CheckBox4" runat="server" Checked='<%# Eval("AlterarEstoque") %>'
                                                Enabled="False" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Com Defeito?" SortExpression="ComDefeito">
                                        <EditItemTemplate>
                                            <asp:CheckBox ID="CheckBox5" runat="server" Checked='<%# Bind("ComDefeito") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:CheckBox ID="Troca_chkComDefeito" runat="server" Checked="True" />
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="CheckBox5" runat="server" Checked='<%# Bind("ComDefeito") %>' Enabled="False" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <FooterTemplate>
                                            <asp:ImageButton ID="Troca_imgAdd" runat="server" OnClientClick="if (!onSaveProd(1)) return false;"
                                                ImageUrl="~/Images/Insert.gif" OnClick="imgAdd_Click" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pgr"></PagerStyle>
                                <EditRowStyle CssClass="edit"></EditRowStyle>
                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr id="separadorProd" runat="server">
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center" valign="top" id="produtosNovos" runat="server">
                            <asp:LinkButton ID="lkbInserirNovo" runat="server" OnClientClick="selProdutoNovo(); return false;">Inserir produto novo</asp:LinkButton>
                            <div class="subtitle1" style="margin-top: 8px; margin-bottom: 4px">
                                Produtos Novos</div>
                            <asp:GridView GridLines="None" ID="grdProdutosNovos" runat="server" AllowPaging="True"
                                AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdTroca" DataKeyNames="IdProdTrocaDev"
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" EmptyDataText="Ainda não há produtos na troca/devolução."
                                ShowFooter="True" OnRowCommand="grdProdutos_RowCommand">
                                <FooterStyle Wrap="True" />
                                <Columns>
                                    <asp:TemplateField>
                                        <FooterTemplate>
                                            <select id="drpFooterVisible" style="display: none">
                                            </select></FooterTemplate>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Visible='<%# Eval("IsTroca") %>'>
                                                <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                            <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                                ToolTip="Excluir" OnClientClick="if (!confirm(&quot;Deseja excluir esse produto da troca/devolução?&quot;)) return false;" />
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                                ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="if (!onUpdateProd()) return false;" />
                                            <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                ToolTip="Cancelar" />
                                            <asp:HiddenField ID="hdfIdProdTrocaDev" runat="server" Value='<%# Eval("IdProdTrocaDev") %>' />
                                            <asp:HiddenField ID="hdfIdTrocaDevolucao" runat="server" Value='<%# Bind("IdTrocaDevolucao") %>' />
                                            <asp:HiddenField ID="Novo_hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                            <asp:HiddenField ID="Novo_hdfIsAluminio" runat="server" Value='<%# Eval("IsAluminio") %>' />
                                            <asp:HiddenField ID="Novo_hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                            <asp:HiddenField ID="Novo_hdfValMin" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfM2Minimo" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfAliquotaIcmsProd" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfValorIcmsProd" runat="server" />
                                        </EditItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("CodInterno") + " - " + Eval("DescrProduto") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="Novo_txtCodProdIns" runat="server" onblur="loadProduto(this.value);"
                                                onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                                Text='<%# Eval("CodInterno") %>' Width="50px" Enabled='<%# Eval("IsTroca") %>'
                                                Visible='<%# Eval("EditarVisible") %>'></asp:TextBox>
                                            <asp:Label ID="Novo_lblCodProdIns" runat="server" Text='<%# Eval("CodInterno") %>'
                                                Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label>
                                            <asp:Label ID="Novo_lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                            <asp:HiddenField ID="Novo_hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                            <asp:HiddenField ID="Novo_hdfCustoProd" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="Novo_txtCodProdIns" runat="server" onblur="loadProduto(this.value);"
                                                onkeydown="if (isEnter(event)) loadProduto(this.value);" onkeypress="return !(isEnter(event));"
                                                Width="50px"></asp:TextBox><asp:Label ID="Novo_lblDescrProd" runat="server"></asp:Label><a
                                                    href="#" onclick="getProduto(); return false;"><img src="../Images/Pesquisar.gif"
                                                        border="0" /></a>
                                            <asp:HiddenField ID="Novo_hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                                            <asp:HiddenField ID="Novo_hdfValMin" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfIsVidro" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfTipoCalc" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfIsAluminio" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfM2Minimo" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfAliquotaIcmsProd" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfValorIcmsProd" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfCustoProd" runat="server" />
                                        </FooterTemplate>
                                        <ItemStyle Wrap="True" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Novo_lblQtdeIns" runat="server" Text='<%# Bind("Qtde") %>' Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label>
                                            <asp:TextBox ID="Novo_txtQtdeIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                Text='<%# Bind("Qtde") %>' Visible='<%# Eval("EditarVisible") %>' Width="50px"
                                                Enabled='<%# Eval("IsTroca") %>'></asp:TextBox>
                                            <uc2:ctrlDescontoQtde ID="Novo_ctrlDescontoQtde" runat="server" OnLoad="ctrlDescontoQtde_Load"
                                                Callback="calcTotalProd" CallbackValorUnit="calcTotalProd" PercDescontoQtde='<%# Bind("PercDescontoQtde")%>'
                                                ValidationGroup="produto" ValorDescontoQtde='<%# Bind("ValorDescontoQtde")%>'
                                                Visible='<%# Eval("EditarVisible") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="Novo_txtQtdeIns" runat="server" onkeydown="if (isEnter(event)) calcM2Prod();"
                                                onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                                onblur="calcM2Prod(); return verificaEstoque();" Width="50px"></asp:TextBox>
                                            <uc2:ctrlDescontoQtde ID="Novo_ctrlDescontoQtde" runat="server" ValidationGroup="produto"
                                                Callback="calcTotalProd" CallbackValorUnit="calcTotalProd" OnLoad="ctrlDescontoQtde_Load" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("Largura") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Novo_lblLarguraIns" runat="server" Text='<%# Bind("Largura") %>' Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label>
                                            <asp:TextBox ID="Novo_txtLarguraIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, true, true);"
                                                Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="50px"
                                                Visible='<%# Eval("EditarVisible") %>'></asp:TextBox></EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="Novo_txtLarguraIns" runat="server" onkeypress="return soNumeros(event, true, true);"
                                                onblur="calcM2Prod();" Width="50px"></asp:TextBox></FooterTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Novo_lblAlturaIns" runat="server" Text='<%# Bind("Altura") %>' Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label>
                                            <asp:TextBox ID="Novo_txtAlturaIns" runat="server" onblur="calcM2Prod();" Text='<%# Bind("Altura") %>'
                                                onchange="FindControl('Novo_hdfAlturaReal', 'input').value = this.value" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('Novo_hdfTipoCalc', 'input').value), true);"
                                                Enabled='<%# Eval("AlturaEnabled") %>' Width="50px" Visible='<%# Eval("EditarVisible") %>'></asp:TextBox><asp:HiddenField
                                                    ID="Novo_hdfAlturaReal" runat="server" Value='<%# Bind("AlturaReal") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="Novo_txtAlturaIns" runat="server" onblur="calcM2Prod();" Width="50px"
                                                onchange="FindControl('Novo_hdfAlturaRealIns', 'input').value = this.value" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('Novo_hdfTipoCalc', 'input').value), true);"></asp:TextBox><asp:HiddenField
                                                    ID="Novo_hdfAlturaRealIns" runat="server" />
                                        </FooterTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                        <ItemTemplate>
                                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("TotM") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Novo_lblTotM2Ins" runat="server" Text='<%# Eval("TotM") %>'></asp:Label><asp:HiddenField
                                                ID="Novo_hdfTotM" runat="server" Value='<%# Eval("TotM") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="Novo_lblTotM2Ins" runat="server"></asp:Label></FooterTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotM2Calc">
                                        <EditItemTemplate>
                                            <asp:Label ID="Novo_lblTotM2CalcIns" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label><asp:HiddenField
                                                ID="Novo_hdfTotM2Calc" runat="server" Value='<%# Eval("TotM2Calc") %>' />
                                            <asp:HiddenField ID="Novo_hdfTotM2CalcSemChapa" runat="server" Value='<%# Eval("TotalM2CalcSemChapa") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="Novo_lblTotM2CalcIns" runat="server"></asp:Label><asp:HiddenField
                                                ID="Novo_hdfTotM2CalcIns" runat="server" />
                                            <asp:HiddenField ID="Novo_hdfTotM2CalcSemChapaIns" runat="server" />
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label12" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label></ItemTemplate>
                                        <HeaderStyle Wrap="True" />
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("ValorVendido", "{0:C}") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Novo_lblValorIns" runat="server" onchange="alteraValorInsTroca(this.innerHTML)"
                                                Text='<%# Eval("ValorVendido") %>' Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label>
                                            <asp:TextBox ID="Novo_txtValorIns" runat="server" onblur="calcTotalProd();" onkeypress="return soNumeros(event, false, true);"
                                                Text='<%# Bind("ValorVendido") %>' Visible='<%# Eval("EditarVisible") %>' Width="50px"
                                                Enabled='<%# Eval("IsTroca") %>'></asp:TextBox></EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="Novo_txtValorIns" runat="server" onkeydown="if (isEnter(event)) calcTotalProd();"
                                                onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProd();"
                                                Width="50px"></asp:TextBox></FooterTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                                        <EditItemTemplate>
                                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%# Eval("EditarVisible") %>'>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="Novo_txtAplIns" runat="server" onblur="loadApl(this.value);" onkeydown="if (isEnter(event)) loadApl(this.value);"
                                                                onkeypress="return !(isEnter(event));" Text='<%# Eval("CodAplicacao") %>' Width="30px"
                                                                Enabled='<%# Eval("IsTroca") %>'></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:PlaceHolder>
                                            <asp:Label ID="Label27" runat="server" Text='<%# Eval("CodAplicacao") %>' Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label>
                                            <asp:HiddenField ID="Novo_hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <table class="pos">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="Novo_txtAplIns" runat="server" onblur="loadApl(this.value);" onkeydown="if (isEnter(event)) loadApl(this.value);"
                                                            onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <a href="#" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="Novo_hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                                        <EditItemTemplate>
                                            <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%# Eval("EditarVisible") %>'>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="Novo_txtProcIns" runat="server" onblur="loadProc(this.value);" onkeydown="if (isEnter(event)) loadProc(this.value);"
                                                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'
                                                                Enabled='<%# Eval("IsTroca") %>'></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a href="#" onclick='openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:PlaceHolder>
                                            <asp:Label ID="Label28" runat="server" Text='<%# Eval("CodProcesso") %>' Visible='<%# !(bool)Eval("EditarVisible") %>'></asp:Label>
                                            <asp:HiddenField ID="Novo_hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <table class="pos">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="Novo_txtProcIns" runat="server" onblur="loadProc(this.value);" onkeydown="if (isEnter(event)) loadProc(this.value);"
                                                            onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <a href="#" onclick='openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="Novo_hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label10" runat="server" Text='<%# Bind("CodProcesso") %>'></asp:Label></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                        <ItemTemplate>
                                            <asp:Label ID="Label7" runat="server" Text='<%# Bind("Total", "{0:C}") %>'></asp:Label></ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:Label ID="Novo_lblTotalIns" runat="server" Text='<%# Bind("Total") %>' Style="padding-top: 4px"></asp:Label></EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="Novo_lblTotalIns" runat="server"></asp:Label></FooterTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
                                        <EditItemTemplate>
                                            <asp:Label ID="Novo_lblValorBenef" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label></EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:Label ID="Novo_lblValorBenef" runat="server"></asp:Label></FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label11" runat="server" Text='<%# Bind("ValorBenef", "{0:C}") %>'></asp:Label></ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                            <asp:LinkButton ID="Novo_lnkBenef" runat="server" OnClientClick="exibirBenef(this); return false;"
                                                Visible='<%# Eval("BenefVisible") %>'>
                                                <img border="0" src="../Images/gear_add.gif" />
                                            </asp:LinkButton>
                                            <table id="Novo_tbConfigVidro" cellspacing="0" style="display: none;">
                                                <tr align="left">
                                                    <td align="center">
                                                        <table>
                                                            <tr>
                                                                <td class="dtvFieldBold">
                                                                    Espessura
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="Novo_txtEspessura" runat="server" OnDataBinding="txtEspessura_DataBinding"
                                                                        onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Bind("Espessura") %>'></asp:TextBox>
                                                                </td>
                                                                <td class="dtvFieldBold">
                                                                    Ped. Cli
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="Novo_txtPedCli" runat="server" MaxLength="50" Width="50px" Text='<%# Bind("PedCli") %>'></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <uc4:ctrlbenef ID="Novo_ctrlBenefEditar" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>'
                                                            ValidationGroup="produto" OnLoad="ctrlBenef_Load" Redondo='<%# Bind("Redondo") %>'
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
                                            <asp:LinkButton ID="Novo_lnkBenef" runat="server" Style="display: none;" OnClientClick="exibirBenef(this); return false;">
                                                <img border="0" src="../Images/gear_add.gif" />
                                            </asp:LinkButton>
                                            <table id="Novo_tbConfigVidro" cellspacing="0" style="display: none;">
                                                <tr align="left">
                                                    <td align="center">
                                                        <table>
                                                            <tr>
                                                                <td class="dtvFieldBold">
                                                                    Espessura
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="Novo_txtEspessura" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                        Width="30px"></asp:TextBox>
                                                                </td>
                                                                <td class="dtvFieldBold">
                                                                    Ped. Cli
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="Novo_txtPedCli" runat="server" MaxLength="50" Width="50px"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <uc4:ctrlbenef ID="Novo_ctrlBenefInserir" runat="server" OnLoad="ctrlBenef_Load"
                                                            CallbackCalculoValorTotal="setValorTotal" ValidationGroup="produto" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                    </td>
                                                </tr>
                                            </table>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Alterar Estoque?" SortExpression="AlterarEstoque">
                                        <EditItemTemplate>
                                            <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("AlterarEstoque") %>' />
                                        </EditItemTemplate>
                                        <FooterTemplate>
                                            <asp:CheckBox ID="Novo_chkAlterarEstoque" runat="server" Checked='True' />
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Eval("AlterarEstoque") %>'
                                                Enabled="False" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <FooterTemplate>
                                            <asp:ImageButton ID="Novo_imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClientClick="if (!onSaveProd()) return false;"
                                                OnClick="imgAdd_Click" />
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
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTroca" runat="server" SelectMethod="GetElement"
                    TypeName="Glass.Data.DAL.TrocaDevolucaoDAO" DataObjectTypeName="Glass.Data.Model.TrocaDevolucao"
                    InsertMethod="InsertComTransacao" UpdateMethod="UpdateComTransacao" OnInserted="odsTroca_Inserted" OnUpdated="odsTroca_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idTrocaDevolucao" QueryStringField="idTrocaDev" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdTroca" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutoTrocaDevolucao"
                    DeleteMethod="Delete" EnablePaging="True" InsertMethod="InsertComTransacao" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoTrocaDevolucaoDAO"
                    UpdateMethod="UpdateComTransacao" OnInserted="odsProdTroca_Inserted" OnUpdated="odsProdTroca_Updated"
                    OnDeleted="odsProdTroca_Deleted">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idTrocaDev" QueryStringField="idTrocaDev" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoDePerda" runat="server"
                    SelectMethod="GetBySetor" TypeName="Glass.Data.DAL.TipoPerdaDAO">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="0" Name="idSetor" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server"
                    SelectMethod="GetOrdered" TypeName="Glass.Data.DAL.FuncionarioDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosTrocados" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount"
                    SelectMethod="GetList" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ProdutoTrocadoDAO" DataObjectTypeName="Glass.Data.Model.ProdutoTrocado"
                    DeleteMethod="Delete" OnDeleted="odsProdutosTrocados_Deleted" UpdateMethod="ExcluirInserir"
                    OnUpdated="odsProdutosTrocados_Updated" OnInserted="odsProdutosTrocados_Inserted">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idTrocaDevolucao" QueryStringField="idTrocaDev" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSetor" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.SetorDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfUrlRetorno" runat="server" />
            </td>
        </tr>
        <tr id="pagamento" runat="server" visible="false">
            <td align="center">
                <uc1:ctrlFormaPagto ID="ctrlFormaPagto1" runat="server" ExibirComissaoComissionado="False"
                    OnLoad="ctrlFormaPagto_Load" ExibirRecebParcial="false" ExibirDataRecebimento="False" ExibirValorAPagar="True" />
                <br />
                <asp:Button ID="btnReceber" runat="server" Text="Receber" OnClientClick="receber(this); return false;" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnGerarContaRec" runat="server" Text="Gerar Conta a Receber" OnClick="btnGerarContaRec_Click"
                    CausesValidation="False" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancelarReceb" runat="server" Text="Cancelar Recebimento" OnClientClick="redirectUrl(window.location.href); return false;" />
                <br />
                <img id="load" src="../Images/load.gif" style="visibility: hidden" />
                <br />
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        
        // Se a empressa não vende vidros, esconde campos
        if (<%= Glass.Configuracoes.Geral.NaoVendeVidro().ToString().ToLower() %>)
        {
            var tbProdTrocados = FindControl("grdProdutosTrocados", "table");
            var tbProdNovo = FindControl("grdProdutosNovos", "table");
            
            if (tbProdTrocados != null)
            {
                var rows = tbProdTrocados.children[0].children;
                
                if (rows.length > 1)
                {
                    var colsTitle = rows[0].getElementsByTagName("th");                
                    colsTitle[3].style.display = "none";
                    colsTitle[4].style.display = "none";
                    colsTitle[5].style.display = "none";
                    colsTitle[6].style.display = "none";
                    colsTitle[8].style.display = "none";
                    colsTitle[9].style.display = "none";
                    colsTitle[11].style.display = "none";
                    
                    var k=0;
                    for (k=1; k<rows.length; k++) {
                        if (rows[k].cells[4] == null)
                            break;
                            
                        rows[k].cells[3].style.display = "none";
                        rows[k].cells[4].style.display = "none";
                        rows[k].cells[5].style.display = "none";
                        rows[k].cells[6].style.display = "none";
                        rows[k].cells[8].style.display = "none";
                        rows[k].cells[9].style.display = "none";
                        rows[k].cells[11].style.display = "none";
                    }
                }
            }
            
            if (tbProdNovo != null)
            {
                var rows = tbProdNovo.children[0].children;
                
                if (rows.length > 1)
                {
                    var colsTitle = rows[0].getElementsByTagName("th");
                    colsTitle[3].style.display = "none";
                    colsTitle[4].style.display = "none";
                    colsTitle[5].style.display = "none";
                    colsTitle[6].style.display = "none";
                    colsTitle[8].style.display = "none";
                    colsTitle[9].style.display = "none";
                    colsTitle[11].style.display = "none";
                    
                    var k=0;
                    for (k=1; k<rows.length; k++) {
                        if (rows[k].cells[4] == null)
                            break;
                            
                        rows[k].cells[3].style.display = "none";
                        rows[k].cells[4].style.display = "none";
                        rows[k].cells[5].style.display = "none";
                        rows[k].cells[6].style.display = "none";
                        rows[k].cells[8].style.display = "none";
                        rows[k].cells[9].style.display = "none";
                        rows[k].cells[11].style.display = "none";
                    }
                }
            }
        }
        
    </script>

</asp:Content>
