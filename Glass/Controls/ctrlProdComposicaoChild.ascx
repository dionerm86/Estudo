<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlProdComposicaoChild.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlProdComposicaoChild" %>

<%@ Register Src="ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc5" %>


<script type="text/javascript">

    // Guarda a quantidade disponível em estoque do produto buscado
    var qtdEstoqueComposicaoChild = 0;
    var exibirMensagemEstoqueComposicaoChild = false;
    var qtdEstoqueMensagemComposicaoChild = 0;
    
    var insertingComposicaoChild = false;
    var produtoAmbienteComposicaoChild = false;
    var aplAmbienteComposicaoChild = false;
    var procAmbienteComposicaoChild = false;
    var loadingComposicaoChild = true;

    function buscaTableChild(control) {
        var tr = control;
        while (tr.id == "" || (tr.id.indexOf("prodPedChild_") == -1 && tr.nodeName.toLowerCase() != "tr")) {
            tr = tr.parentElement;
        }

        return tr;
    }

    function getNomeControleBenefComposicaoChild(control)
    {
        var nomeControle = "<%= NomeControleBenefComposicao() %>";
        nomeControle = FindControl(nomeControle + "_tblBenef", "table", control);

        if (nomeControle == null)
            return null;

        nomeControle = nomeControle.id;
        return nomeControle.substr(0, nomeControle.lastIndexOf("_"));
    }

    // Carrega dados do produto com base no código do produto passado
    function loadProdutoComposicaoChild(codInterno, control, alterarValor) {
        if (control == null || codInterno == "")
            return false;

        var table = buscaTableChild(control);
        
        var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;        
        var txtValor = FindControl("txtChild_ValorComposicaoIns", "input", table);
        
        var verificaProduto = CadPedido.IsProdutoObra(idPedido, codInterno).value.split(";");        
        if (verificaProduto[0] == "Erro")
        {
            if (FindControl("txtChild_CodProdComposicao", "input", table) != null)
                FindControl("txtChild_CodProdComposicao", "input", table).value = "";
                    
            alert("Esse produto não pode ser usado no pedido. " + verificaProduto[1]);
            return false;
        }
        else if (parseFloat(verificaProduto[1].replace(",", ".")) > 0)
        {
            if (txtValor != null)
                txtValor.disabled = true;
            
            // Se for edição de produto, chamad o método padrão de cálculo da metragem máxima permitida
            if (FindControl("hdfChild_ProdPedComposicao", "input", table) != null)
                calculaTamanhoMaximoComposicaoChild(table);
            else if (FindControl("hdfChild_TamanhoMaximoObraComposicao", "input", table) != null)    
                FindControl("hdfChild_TamanhoMaximoObraComposicao", "input", table).value = verificaProduto[2];
        }
        else
        {
            if (txtValor != null)
                txtValor.disabled = verificaProduto[3] == "false";
            
            if (FindControl("hdfChild_TamanhoMaximoObraComposicao", "input", table) != null)    
                FindControl("hdfChild_TamanhoMaximoObraComposicao", "input", table).value = "0";
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
                percComissao, tipoPedido, tipoVenda, produtoAmbienteComposicaoChild, percDescontoQtde, FindControl("hdfLoja", "input").value, true).value.split(';');
            
            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                if (!produtoAmbienteComposicaoChild)
                    FindControl("txtChild_CodProdComposicao", "input", table).value = "";
                else
                    FindControl("txtChild_CodAmbComposicao", "input", table).value = "";
                
                return false;
            }
            
            else if (!produtoAmbienteComposicaoChild)
            {
                if (retorno[0] == "Prod") {
                    FindControl("hdfChild_IdProdComposicao", "input", table).value = retorno[1];

                    var subgrupoProdComposto = CadPedido.SubgrupoProdComposto(retorno[1]).value;
                    var tipoPedido = FindControl("hdfTipoPedido", "input").value;

                    alterarValor = alterarValor === false ? false : true;
                                    
                    if (verificaProduto[1] != "0") // Exibe no cadastro o valor mínimo do produto
                        txtValor.value = alterarValor ? verificaProduto[1] : txtValor.value;
                        // O valor do produto deve ser atualizado sempre, para que caso seja buscado um produto, preenchendo automaticamente
                        // o valor unitário e o usuário resolva buscar outro produto sem ter inserido o primeiro, garanta que será buscado o valor deste
                    else 
                        txtValor.value = alterarValor ? retorno[3] : txtValor.value;
                    
                    FindControl("hdfChild_IsVidroComposicao", "input", table).value = retorno[4]; // Informa se o produto é vidro
                    FindControl("hdfChild_M2MinimoComposicao", "input", table).value = retorno[5]; // Informa se o produto possui m² mínimo
                    FindControl("hdfChild_TipoCalcComposicao", "input", table).value = retorno[7]; // Verifica como deve ser calculado o produto
                    
                    // Se o campo do valor estiver desativado não precisa calcular o valor mínimo, tendo em vista que o usuário não poderá alterar.
                    if (!txtValor.disabled)
                        atualizaValMinComposicaoChild(table);
                    
                    qtdEstoqueComposicaoChild = retorno[6]; // Pega a quantidade disponível em estoque deste produto
                    exibirMensagemEstoqueComposicaoChild = retorno[14] == "true";
                    qtdEstoqueMensagemComposicaoChild = retorno[15];
                    
                    var tipoCalc = retorno[7];

                    // Se o produto não for vidro, desabilita os textboxes largura e altura,
                    // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
                    // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
                    var cAltura = FindControl("txtChild_AlturaComposicaoIns", "input", table);
                    var cLargura = FindControl("txtChild_LarguraComposicaoIns", "input", table);
                    // var maoDeObra = FindControl("hdfPedidoMaoDeObra", "input").value == "true";
                    //var alturaAmbiente = FindControl("hdf_AlturaAmbienteComposicao", "input", table).value;
                    // var larguraAmbiente = FindControl("hdf_LarguraAmbienteComposicao", "input", table).value;
                    cAltura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
                    cLargura.disabled = CalcProd_DesabilitarLargura(tipoCalc);
                    
                    var nomeControle = getNomeControleBenefComposicaoChild(table);

                    // Zera o campo qtd para evitar que produtos calculados por mҠfiquem com quantidade decimal por exemplo (chamado 11010)
                    var txtQtdProd = FindControl("txtChild_QtdeComposicaoIns", "input", table);
                    if (txtQtdProd != null && !loadingComposicaoChild)
                        txtQtdProd.value = "";
                    
                    if (nomeControle != null && nomeControle != undefined) {
                        // Se produto for do grupo vidro, habilita campos de beneficiamento e mostra a espessura
                        if (retorno[4] == "true" && exibirControleBenef(nomeControle) && FindControl("lnkChild_BenefComposicao", "input", table) != null) {
                            FindControl("txtChild_EspessuraComposicao", "input", table).value = retorno[8];
                            FindControl("txtChild_EspessuraComposicao", "input", table).disabled = retorno[8] != "" && retorno[8] != "0";
                        }
                    
                        if (FindControl("lnkChild_BenefComposicao", "input", table) != null && nomeControle != null && nomeControle.indexOf("Inserir") > -1)
                            FindControl("lnkChild_BenefComposicao", "input", table).style.display = exibirControleBenef(nomeControle) ? "" : "none";
                    }
                        
                    FindControl("hdfChild_AliquotaIcmsProdComposicao", "input", table).value = retorno[9].replace('.', ',');
                    
                    // O campo altura e largura devem sempre ser atribuídos pois caso seja selecionado um box e logo após seja selecionado um kit 
                    // por exemplo, ao inserí-lo ele estava ficando com o campo altura, largura e m² preenchidos apesar de ser calculado por qtd
                    if (retorno[10] != "" || retorno[4] == "false") {
                        FindControl("txtChild_AlturaComposicao", "input", table).value = retorno[10];
                        FindControl("hdfChild_AlturaRealComposicao", "input", table).value = retorno[10];
                    }
                    if (retorno[11] != "" || retorno[4] == "false") FindControl("txt_LarguraComposicao", "input", table).value = retorno[11];
                        
                    if (cAltura.disabled && FindControl("hdfChild_AlturaRealComposicao", "input", table) != null)
                        FindControl("hdfChild_AlturaRealComposicao", "input", table).value = cAltura.value;

                    var idProdPed = FindControl("hdfChild_IdProdPed", "input", table).value;

                    if (retorno[16] != "")
                        setAplComposicaoChild(retorno[16], retorno[17], idProdPed);
                    
                    if (retorno[18] != "")
                        setProcComposicaoChild(retorno[18], retorno[19], null, idProdPed);
                    
                    FindControl("hdfChild_CustoProdComposicao", "input", table).value = retorno[20];

                    var cPodeEditarComposicao = FindControl("hdfChild_PodeEditarComposicao","input", table);
                    if(cPodeEditarComposicao != null)
                    {
                        var podeEditarComposicao = cPodeEditarComposicao.value;
                        var cQtdeComposicaoIns = FindControl("txtChild_QtdeComposicaoIns", "input", table);
                        cQtdeComposicaoIns.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cQtdeComposicaoIns.disabled;
                        cLargura.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cLargura.disabled;
                        cAltura.disabled = podeEditarComposicao.toLowerCase() == "false" ? true : cAltura.disabled;
                    }
                }

                FindControl("lblChild_DescrProdComposicao", "span", table).innerHTML = retorno[2];

                if (retorno.length >= 22)
                    FindControl("lblChild_DescrProdComposicao", "span", table).innerHTML += " (Valor m²: " + retorno[21] + ")";
            }
            else
            {
                FindControl("hdfChild_AmbIdProdComposicao", "input", table).value = retorno[1];
                FindControl("lblChild_DescrAmbComposicao", "span", table).innerHTML = retorno[2];
                FindControl("hdfChild_DescrAmbienteComposicao", "input", table).value = retorno[2];
            }
        }
        catch (err) {
            alert(err);
        }
        
        produtoAmbienteComposicaoChild = false;
    }

    function calculaTamanhoMaximoComposicaoChild(control)
    {
        if (FindControl("lblChild_CodProdComposicaoIns", "span") == null)
            return;

        var table = buscaTableChild(control);
            
        var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;
        var codInterno = FindControl("lblChild_CodProdComposicaoIns", "span", table).innerHTML;
        var totM2 = FindControl("llbChild_TotM2ComposicaoIns", "span", table).innerHTML;
        var idProdPed = FindControl("hdfChild_ProdPedComposicao", "input", table) != null ? FindControl("hdfChild_ProdPedComposicao", "input", table).value : 0;
        
        var tamanhoMaximo = CadPedido.GetTamanhoMaximoProduto(idPedido, codInterno, totM2, idProdPed).value.split(";");
        tamanhoMaximo = tamanhoMaximo[0] == "Ok" ? parseFloat(tamanhoMaximo[1].replace(",", ".")) : 0;
        
        FindControl("hdfChild_TamanhoMaximoObraComposicao", "input", table).value = tamanhoMaximo;
    }

    function atualizaValMinComposicaoChild(control)
    {
        var table = buscaTableChild(control);

        if (parseFloat(FindControl("hdfChild_TamanhoMaximoObraComposicao", "input", table).value.replace(",", ".")) == 0)
        {
            var codInterno = FindControl("txtChild_CodProdComposicaoIns", "input", table);
            codInterno = codInterno != null ? codInterno.value : FindControl("lblChild_CodProdComposicaoIns", "span", table).innerHTML;
            
            var idPedido = '<%= Request["idPedido"] %>';
            var tipoPedido = FindControl("hdfTipoPedido", "input").value;
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;       
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var tipoVenda = FindControl("hdfTipoVenda", "input").value;
            
            var idProdPed = FindControl("hdfChild_ProdPedComposicao", "input", table);
            idProdPed = idProdPed != null ? idProdPed.value : "";
            
            var controleDescQtde = FindControl("_divDescontoQtde", "div", table).id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
            
            var percDescontoQtde = controleDescQtde.PercDesconto();
            
            FindControl("hdfChild_ValMinComposicao", "input", table).value = CadPedido.GetValorMinimo(codInterno, tipoPedido, tipoEntrega, tipoVenda, 
                idCliente, cliRevenda, idProdPed, percDescontoQtde, idPedido).value;
        }
        else
            FindControl("hdfChild_ValMinComposicao", "input", table).value = FindControl("txtChild_ValorComposicaoIns", "input", table).value;
    }

    // Função chamada pelo popup de escolha da Aplicação do produto
    function setAplComposicaoChild(idAplicacao, codInterno, idProdPed) {

        var tr = FindControl("prodPedChild_" + idProdPed, "tr");

        if (!aplAmbienteComposicaoChild)
        {
            FindControl("txtChild_AplComposicaoIns", "input", tr).value = codInterno;
            FindControl("hdfChild_IdAplicacaoComposicao", "input", tr).value = idAplicacao;
        }
        else
        {
            FindControl("txtChild_AmbAplComposicaoIns", "input", tr).value = codInterno;
            FindControl("hdf_AmbIdAplicacaoComposicao", "input", tr).value = idAplicacao;
        }
        
        aplAmbienteComposicaoChild = false;
    }

    function loadAplComposicaoChild(control, codInterno) {

        var tr = buscaTableChild(control);

        var idProdPed = FindControl("hdfChild_IdProdPed", "input", tr).value;

        if (codInterno == "") {
            setAplComposicaoChild("", "", idProdPed);
            return false;
        }
    
        try {
            var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Aplicação. Ajax Error.");
                setAplComposicaoChild("", "", idProdPed);
                return false
            }

            response = response.split("\t");
            
            if (response[0] == "Erro") {
                alert(response[1]);
                setAplComposicaoChild("", "", idProdPed);
                return false;
            }

            setAplComposicaoChild(response[1], response[2], idProdPed);
        }
        catch (err) {
            alert(err);
        }
    }

    // Função chamada pelo popup de escolha do Processo do produto
    function setProcComposicaoChild(idProcesso, codInterno, codAplicacao, idProdPed) {
        var codInternoProd = "";
        var codAplicacaoAtual = "";

        var tr = FindControl("prodPedChild_" + idProdPed, "tr");
        
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfChild_IdProdComposicao", "input", tr).value);
        var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

        if(idSubgrupo.value != "" && retornoValidacao.value == "False" && (FindControl("txtChild_ProcComposicaoIns", "input", tr) != null && FindControl("txtChild_ProcComposicaoIns", "input", tr).value != ""))
        {
            FindControl("txtChild_ProcComposicaoIns", "input", tr).value = "";
            alert("Este processo não pode ser selecionado para este produto.")
            return false;
        }

        var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, FindControl("hdfIdPedido", "input").value);
        
        if(verificaEtiquetaProc.error != null){

            if (!procAmbienteComposicao && FindControl("txtChild_ProcComposicaoIns", "input", tr) != null)
            {
                FindControl("txtChild_ProcComposicaoIns", "input", tr).value = "";
                FindControl("hdfChild_IdProcessoComposicao", "input", tr).value = "";
            }
            else
            {
                FindControl("txtChild_AmbProcComposicaoIns", "input", tr).value = "";
                FindControl("hdfChild_AmbIdProcessoComposicao", "input", tr).value = "";
            }

            setAplComposicaoChild("", "", idProdPed);

            alert(verificaEtiquetaProc.error.description);
            return false;
        }
        

        if (!procAmbienteComposicaoChild)
        {
            FindControl("txtChild_ProcComposicaoIns", "input", tr).value = codInterno;
            FindControl("hdfChild_IdProcessoComposicao", "input", tr).value = idProcesso;
            
            if (FindControl("txtChild_CodProdComposicaoIns", "input", tr) != null)
                codInternoProd = FindControl("txtChild_CodProdComposicaoIns", "input", tr).value;
            else
                codInternoProd = FindControl("lblChild_CodProdComposicaoIns", "span", tr).innerHTML;
                
            codAplicacaoAtual = FindControl("txtChild_AplComposicaoIns", "input", tr).value;
        }
        else
        {
            FindControl("txtChild_AmbProcComposicaoIns", "input", tr).value = codInterno;
            FindControl("hdfChild_AmbIdProcessoComposicao", "input", tr).value = idProcesso;
            
            codInternoProd = FindControl("txtChild_CodAmbComposicao", "input", tr).value;
            codAplicacaoAtual = FindControl("txtChild_AmbAplComposicaoIns", "input", tr).value;
        }
        
        if (((codAplicacao && codAplicacao != "") ||
            (codInternoProd != "" && CadPedido.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) &&
            (codAplicacaoAtual == null || codAplicacaoAtual == ""))
        {
            aplAmbienteComposicaoChild = procAmbienteComposicaoChild;
            loadAplComposicaoChild(tr, codAplicacao);
        }
        
        procAmbienteComposicaoChild = false;
    }

    function loadProcComposicaoChild(control, codInterno) {

        var tr = buscaTableChild(control);

        var idProdPed = FindControl("hdfChild_IdProdPed", "input", tr).value;

        if (codInterno == "") {
            setProcComposicaoChild("", "", "", idProdPed);
            return false;
        }

        try {
            var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Processo. Ajax Error.");
                setProcComposicaoChild("", "", "", idProdPed);
                return false
            }

            response = response.split("\t");
            
            if (response[0] == "Erro") {
                alert(response[1]);
                setProcComposicaoChild("", "", "", idProdPed);
                return false;
            }

            setProcComposicaoChild(response[1], response[2], response[3], idProdPed);
        }
        catch (err) {
            alert(err);
        }
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

            var idProd = FindControl("hdfChild_IdProdComposicao", "input", table).value;
            var altura = FindControl("txtChild_AlturaComposicaoIns", "input", table).value;
            var largura = FindControl("txtChild_LarguraComposicaoIns", "input", table).value;
            
            var qtde = FindControl("txtChild_QtdeComposicaoIns", "input", table).value;
            var isVidro = FindControl("hdfChild_IsVidroComposicao", "input", table).value == "true";
            var tipoCalc = FindControl("hdfChild_TipoCalcComposicao", "input", table).value;
            
            if (altura == "" || largura == "" || qtde == "" || altura == "0" || (tipoCalc != 2 && tipoCalc != 10 && !usarBenefTodosGrupos)) {
                if (qtde != "" && qtde != "0")
                    calcTotalProdComposicaoChild(table);

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

            var esp = FindControl("txtChild_EspessuraComposicao", "input", table) != null ? FindControl("txtChild_EspessuraComposicao", "input", table).value : 0;
            
            // Calcula metro quadrado
            var idCliente = FindControl("hdfIdCliente", "input").value;
            
            if ((idProd != dadoscalcM2ProdComposicaoChildChild.IdProd && idProd > 0) || (altura != dadoscalcM2ProdComposicaoChildChild.Altura && altura > 0) ||
                (largura != dadoscalcM2ProdComposicaoChildChild.Largura) || (qtde != dadoscalcM2ProdComposicaoChildChild.Qtde && qtde > 0) ||
                (tipoCalc != dadoscalcM2ProdComposicaoChildChild.TipoCalc && tipoCalc > 0) || (idCliente != dadoscalcM2ProdComposicaoChildChild.Cliente) || (redondo != dadoscalcM2ProdComposicaoChildChild.Redondo) ||
                (numBenef != dadoscalcM2ProdComposicaoChildChild.NumBenef))
            {
                FindControl("llbChild_TotM2ComposicaoIns", "span", table).innerHTML = MetodosAjax.CalcM2(tipoCalc, altura, largura, qtde, idProd, redondo, esp, "false").value;
                FindControl("hdfChild_TotM2CalcComposicao", "input", table).value = MetodosAjax.CalcM2Calculo(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
                FindControl("hdf_TotM2CalcSemChapaComposicao", "input", table).value = MetodosAjax.CalcM2CalculoSemChapa(idCliente, tipoCalc, altura, largura, qtde, idProd, redondo, esp, numBenef, "false").value;
                FindControl("lbl_TotM2CalcComposicao", "span", table).innerHTML = FindControl("hdfChild_TotM2CalcComposicao", "input", table).value.replace('.', ',');
                
                if (FindControl("hdfChild_TotM2ComposicaoIns", "input", table) != null)
                    FindControl("hdfChild_TotM2ComposicaoIns", "input", table).value = FindControl("llbChild_TotM2ComposicaoIns", "span", table).innerHTML.replace(',', '.');
                else if (FindControl("hdfChild_TotMComposicao", "input", table) != null)
                    FindControl("hdfChild_TotMComposicao", "input", table).value = FindControl("llbChild_TotM2ComposicaoIns", "span", table).innerHTML.replace(',', '.');
                
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
        catch (err) {
            alert(err);
        }
    }

    // Calcula em tempo real o valor total do produto
    function calcTotalProdComposicaoChild(control) {
        try {
            var table = buscaTableChild(control);

            var valorIns = FindControl("txtChild_ValorComposicaoIns", "input", table).value;

            if (valorIns == "")
                return;

            var totM2 = FindControl("llbChild_TotM2ComposicaoIns", "span", table).innerHTML;
            var totM2Calc = new Number(FindControl("hdfChild_TotM2CalcComposicao", "input", table).value.replace(',', '.')).toFixed(2);
            var total = new Number(valorIns.replace(',', '.')).toFixed(2);
            var qtde = new Number(FindControl("txtChild_QtdeComposicaoIns", "input", table).value.replace(',', '.'));
            var altura = new Number(FindControl("txtChild_AlturaComposicaoIns", "input", table).value.replace(',', '.'));
            var largura = new Number(FindControl("txtChild_LarguraComposicaoIns", "input", table).value.replace(',', '.'));
            var tipoCalc = FindControl("hdfChild_TipoCalcComposicao", "input", table).value;
            var m2Minimo = FindControl("hdfChild_M2MinimoComposicao", "input", table).value;
            
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
    function verificaEstoqueComposicaoChild(control) {

        var table = buscaTableChild(control);

        var txtQtd = FindControl("txtChild_QtdeComposicaoIns", "input", table).value;
        var txtAltura = FindControl("txtChild_AlturaComposicaoIns", "input", table).value;
        var tipoCalc = FindControl("hdfChild_TipoCalcComposicao", "input", table).value;
        var idPedido = FindControl("hdfIdPedido", "input").value;
        var totM2 = FindControl("llbChild_TotM2ComposicaoIns", "span", table).innerHTML;
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
    
        var estoqueMenor = txtQtd != "" && parseInt(txtQtd) > parseInt(qtdEstoqueComposicaoChild);
        if (estoqueMenor)
        {
            if (qtdEstoqueComposicaoChild == 0)
                alert("Não há nenhuma peça deste produto no estoque.");
            else
                alert("Há apenas " + qtdEstoqueComposicaoChild + " " + (isCalcM2 ? "m²" : isCalcAluminio ? "ml (" + parseFloat(qtdEstoqueComposicaoChild / 6).toFixed(2) + " barras)" : "peça(s)") + " deste produto no estoque.");
            
                FindControl("txtChild_QtdeComposicaoIns", "input", table).value = "";
        }
        
        var exibirPopup = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ExibePopupVidrosEstoque.ToString().ToLower() %>;
        if (exibirPopup && exibirMensagemEstoqueComposicaoChild && (qtdEstoqueMensagemComposicaoChild <= 0 || estoqueMenor))
            openWindow(400, 600, "../Utils/DadosEstoque.aspx?idProd=" + FindControl("hdfChild_IdProdComposicao", "input", table).value + "&idPedido=" + idPedido);
    }

    function exibirBenefComposicaoChild(botao, id) {
        for (iTip = 0; iTip < 2; iTip++) {
            TagToTip('tb_ConfigVidroComposicao_' + id, FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true,
                CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                FIX, [botao, 9 - getTableWidth('tb_ConfigVidroComposicao_' + id), -41 - getTableHeight('tb_ConfigVidroComposicao_' + id)]);
        }
    }

    function setValorTotalComposicaoChild(valor, custo, idProdPed) {

        if (getNomeControleBenefComposicaoChild() != null) {
            if (exibirControleBenef(getNomeControleBenefComposicaoChild())) {
                var tr = FindControl("prodPedChild_" + idProdPed, "tr");
                var lblValorBenef = FindControl("lbl_ValorBenefComposicao", "span", tr);
                lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
            }
        }
    }

    function selProcComposicaoChild(control) {

        var tr = buscaTableChild(control);

        var idProdPed = FindControl("hdfChild_IdProdPed", "input", tr).value;
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfChild_IdProdComposicao", "input", tr).value);

        openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx?idProdPed=' + idProdPed+'&idSubgrupo=' + idSubgrupo.value);

        return false;
    }

    function selAplComposicaoChild(control) {

        var tr = buscaTableChild(control);

        var idProdPed = FindControl("hdfChild_IdProdPed", "input", tr).value;

        openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx?idProdPed=' + idProdPed);

        return false;
    }

    function getProdutoComposicaoChild(idPedido, control) {

        var tr = buscaTableChild(control);

        var idProdPed = FindControl("hdfChild_IdProdPed", "input", tr).value;
        openWindow(450, 700, '../Utils/SelProd.aspx?idPedido=' + idPedido + '&idProdPed=' + idProdPed + '&callback=prodComposicao');
    }

    function setProdutoComposicaoChild(codInterno, idPedido, idProdPed) {

        var tr = FindControl("prodPedChild_" + idProdPed, "tr");

        FindControl("txtChild_CodProdComposicaoIns", "input", tr).value = codInterno;
        loadProdutoComposicaoChild(codInterno, tr);
    }

    function obrigarProcAplComposicaoChild(control)
    {
        var table = buscaTableChild(control);

        var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
        var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
        var isVidroRoteiro = dadosProduto.Grupo == 1 && <%= UtilizarRoteiroProducao().ToString().ToLower() %>;
        
        if (dadosProduto.IsChapaVidro)
            return true;

        if (isVidroRoteiro || (isObrigarProcApl && isVidroBenef))
        {
            if (FindControl("txtChild_AplComposicaoIns", "input", table) != null && FindControl("txtChild_AplComposicaoIns", "input", table).value == "")
            {
                if (isVidroRoteiro && !isObrigarProcApl) {
                    alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                    return false;
                }

                alert("Informe a aplicação.");
                return false;
            }
            
            if (FindControl("txtChild_ProcComposicaoIns", "input", table) != null && FindControl("txtChild_ProcComposicaoIns", "input", table).value == "")
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

    function validaTamanhoMaxComposicaoChild(control)
    {
        var table = buscaTableChild(control);

        var tamanhoMaximo = parseFloat(FindControl("hdfChild_TamanhoMaximoObraComposicao", "input", table).value.replace(",", "."));
        if (tamanhoMaximo > 0)
        {        
            var totM2 = parseFloat(FindControl("llbChild_TotM2ComposicaoIns", "span", table).innerHTML.replace(",", "."));
            if (totM2 > tamanhoMaximo)
            {
                alert("O total de m² da peça ultrapassa o máximo definido no pagamento antecipado. Tamanho máximo restante: " + tamanhoMaximo.toString().replace(".", ",") + " m²");
                return false;
            }
        }
        
        return true;
    }

    var saveProdComposicaoClickedChild = false;

    // Chamado quando um produto está para ser inserido no pedido
    function onSaveProdComposicaoChild(control, idTbConfigVidro) {
        if (!validate("produtoComposicaoChild"))
            return false;
            
        if (saveProdComposicaoClickedChild == true)
            return false;
            
        saveProdComposicaoClickedChild = true;
        
        var tr = buscaTableChild(control);

        atualizaValMinComposicaoChild(tr);
    
        var codProd = FindControl("txtChild_CodProdComposicaoIns", "input", tr).value;
        var idProd = FindControl("hdfChild_IdProdComposicao", "input", tr).value;
        var valor = FindControl("txtChild_ValorComposicaoIns", "input", tr).value;
        var qtde = FindControl("txtChild_QtdeComposicaoIns", "input", tr).value;
        var altura = FindControl("txtChild_AlturaComposicaoIns", "input", tr).value;
        var largura = FindControl("txtChild_LarguraComposicaoIns", "input", tr).value;
        var valMin = FindControl("hdfChild_ValMinComposicao", "input", tr).value;
        var tipoVenda = FindControl("hdfTipoVenda", "input");
        tipoVenda = tipoVenda != null ? tipoVenda.value : 0;

        if (codProd == "") {
            alert("Informe o código do produto.");
            saveProdComposicaoClickedChild = false;
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
            saveProdComposicaoClickedChild = false;
            return false;
        }
        
        if (qtde == "0" || qtde == "") {
            alert("Informe a quantidade.");
            saveProdComposicaoClickedChild = false;
            return false;
        }
        
        valMin = new Number(valMin.replace(',', '.'));
        if (!FindControl("txtChild_ValorComposicaoIns", "input", tr).disabled && new Number(valor.replace(',', '.')) < valMin) {
            alert("Valor especificado abaixo do valor mínimo (R$ " + valMin.toFixed(2).replace(".", ",") + ")");
            saveProdComposicaoClickedChild = false;
            return false;
        }
        
        if (FindControl("txtChild_AlturaComposicaoIns", "input", tr).disabled == false) {
            if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                alert("Informe a altura.");
                saveProdComposicaoClickedChild = false;
                return false;
            }

            if (FindControl("hdfChild_IsAluminioComposicao", "input", tr).value == "true" && altura > parseInt(comprimentoMaxAluminio)) {
                alert("A altura deve ser no máximo " + comprimentoMaxAluminio + "ml.");
                saveProdComposicaoClickedChild = false;
                return false;
            }            
        }
        
        // Se o textbox da largura estiver habilitado, deverá ser informada
        if (FindControl("txtChild_LarguraComposicaoIns", "input", tr).disabled == false && largura == "") {
            alert("Informe a largura.");
            saveProdComposicaoClickedChild = false;
            return false;
        }
        
        if (!obrigarProcAplComposicaoChild(tr))
        {
            saveProdComposicaoClickedChild = false;
            return false;
        }
        
        if (!validaTamanhoMaxComposicaoChild(tr))
        {
            saveProdComposicaoClickedChild = false;
            return false;
        }
        
        // Calcula o ICMS do produto
        var aliquota = FindControl("hdfChild_AliquotaIcmsProdComposicao", "input", tr);
        var icms = FindControl("hdfChild_ValorIcmsProdComposicao", "input", tr);
        icms.value = aliquota.value > 0 ? parseFloat(valor) * (parseFloat(aliquota.value) / 100) : 0;
        icms.value = icms.value.toString().replace('.', ',');
        
        if (FindControl("txtChild_EspessuraComposicao", "input", tr) != null)
            FindControl("txtChild_EspessuraComposicao", "input", tr).disabled = false;
        
        FindControl("txtChild_AlturaComposicaoIns", "input", tr).disabled = false;
        FindControl("txtChild_LarguraComposicaoIns", "input", tr).disabled = false;
        FindControl("txtChild_ValorComposicaoIns", "input", tr).disabled = false;
        FindControl("txtChild_QtdeComposicaoIns", "input", tr).disabled = false;
        
        return true;
    }

    // Função chamada quando o produto está para ser atualizado
    function onUpdateProdComposicaoChild(control, idTbConfigVidro) {
        if (!validate("produtoComposicaoChild"))
            return false;
        
        var table = buscaTableChild(control);

        atualizaValMinComposicaoChild(table);
    
        var valor = FindControl("txtChild_ValorComposicaoIns", "input", table).value;
        var qtde = FindControl("txtChild_QtdeComposicaoIns", "input", table).value;
        var altura = FindControl("txtChild_AlturaComposicaoIns", "input", table).value;
        var idProd = FindControl("hdfChild_IdProdComposicao", "input", table).value;
        var codInterno = FindControl("hdfChild_CodInternoComposicao", "input", table).value;
        var valMin = FindControl("hdfChild_ValMinComposicao", "input", table).value;
        var tipoVenda = FindControl("hdfTipoVenda", "input");
        tipoVenda = tipoVenda != null ? tipoVenda.value : 0;

        valMin = new Number(valMin.replace(',', '.'));
        if (!FindControl("txtChild_ValorComposicaoIns", "input", table).disabled && new Number(valor.replace(',', '.')) < valMin) {
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
        else if (FindControl("txtChild_AlturaComposicaoIns", "input", table).disabled == false) {
            if (altura == "" || parseFloat(altura.replace(",", ".")) == 0) {
                alert("Informe a altura.");
                return false;
            }

            if (FindControl("hdfChild_IsAluminioComposicao", "input", table).value == "true" && altura > parseInt(comprimentoMaxAluminio)) {
                alert("A altura deve ser no máximo " + comprimentoMaxAluminio + "ml.");
                return false;
            }
        }
        
        if (!obrigarProcAplComposicaoChild(table))
            return false;
        
        if (!validaTamanhoMaxComposicaoChild(table))
            return false;
        
        // Calcula o ICMS do produto
        var aliquota = FindControl("hdfChild_AliquotaIcmsProdComposicao", "input", table);
        var icms = FindControl("hdfChild_ValorIcmsProdComposicao", "input", table);
        icms.value = parseFloat(valor) * (parseFloat(aliquota.value) / 100);
        icms.value = icms.value.toString().replace('.', ',');

        if (FindControl("txtChild_EspessuraComposicao", "input", table) != null)
            FindControl("txtChild_EspessuraComposicao", "input", table).disabled = false;
        
        FindControl("txtChild_AlturaComposicaoIns", "input", table).disabled = false;
        FindControl("txtChild_LarguraComposicaoIns", "input", table).disabled = false;
        FindControl("txtChild_ValorComposicaoIns", "input", table).disabled = false;
        FindControl("txtChild_QtdeComposicaoIns", "input", table).disabled = false;
            
        return true;
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
                <asp:ImageButton ID="lnk_EditComposicao" runat="server" CommandName="Edit" OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>'
                    ImageUrl="~/Images/Edit.gif" />
                <asp:ImageButton ID="imb_ExcluirComposicao" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" Visible='<%# Eval("DeleteVisible") %>'
                    OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(false); return false" : "if (!confirm(\"Deseja remover esse produto do pedido?\")) return false" %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <asp:ImageButton ID="imb_AtualizarComposicao" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar"
                    OnClientClick='<%# "if (!onUpdateProdComposicaoChild(this, &#39;" + IdProdPed + "_" + Eval("IdProdPed") + "&#39;)) return false;" %>' />
                <asp:ImageButton ID="imb_CancelarComposicao" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />

                <asp:HiddenField ID="hdfChild_ProdPedComposicao" runat="server" Value='<%# Eval("IdProdPed") %>' />
                <asp:HiddenField ID="hdfChild_IdPedidoComposicao" runat="server" Value='<%# Bind("IdPedido") %>' />
                <asp:HiddenField ID="hdfChild_IdProdComposicao" runat="server" Value='<%# Bind("IdProd") %>' />
                <asp:HiddenField ID="hdfChild_CodInternoComposicao" runat="server" Value='<%# Eval("CodInterno") %>' />
                <asp:HiddenField ID="hdfChild_ValMinComposicao" runat="server" />
                <asp:HiddenField ID="hdfChild_IsVidroComposicao" runat="server" Value='<%# Eval("IsVidro") %>' />
                <asp:HiddenField ID="hdfChild_IsAluminioComposicao" runat="server" Value='<%# Eval("IsAluminio") %>' />
                <asp:HiddenField ID="hdfChild_M2MinimoComposicao" runat="server" Value='<%# Eval("M2Minimo") %>' />
                <asp:HiddenField ID="hdfChild_TipoCalcComposicao" runat="server" Value='<%# Eval("TipoCalc") %>' />
                <asp:HiddenField ID="hdfChild_IdItemProjetoComposicao" runat="server" Value='<%# Bind("IdItemProjeto") %>' />
                <asp:HiddenField ID="hdfChild_IdMaterItemProjComposicao" runat="server" Value='<%# Bind("IdMaterItemProj") %>' />
                <asp:HiddenField ID="hdfChild_IdAmbientePedidoComposicao" runat="server" Value='<%# Bind("IdAmbientePedido") %>' />
                <asp:HiddenField ID="hdfChild_AliquotaIcmsProdComposicao" runat="server" Value='<%# Bind("AliqIcms") %>' />
                <asp:HiddenField ID="hdfChild_ValorIcmsProdComposicao" runat="server" Value='<%# Bind("ValorIcms") %>' />
                <asp:HiddenField ID="hdfChild_ValorTabelaOrcamentoComposicao" runat="server" Value='<%# Bind("ValorTabelaOrcamento") %>' />
                <asp:HiddenField ID="hdfChild_ValorTabelaPedidoComposicao" runat="server" Value='<%# Bind("ValorTabelaPedido") %>' />
                <asp:HiddenField ID="hdfChild_IdProdPedParent" runat="server" Value='<%# Bind("IdProdPedParent") %>' />
                <asp:HiddenField ID="hdfChild_IdProdBaixaEst" runat="server" Value='<%# Bind("IdProdBaixaEst") %>' />
                <asp:HiddenField ID="hdfChild_PodeEditarComposicao" runat="server" Value='<%# Eval("PodeEditarComposicao") %>' />
            </EditItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
            <ItemTemplate>
                <asp:Label ID="lbl_CodProdComposicao" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Label ID="lblChild_CodProdComposicaoIns" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
            </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
            <ItemTemplate>
                <asp:Label ID="lbl_ProdutoComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Label ID="lblChild_DescrProdComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") %>'></asp:Label>
                <asp:HiddenField ID="hdfChild_CustoProdComposicao" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txtChild_CodProdComposicaoIns" runat="server" onblur="loadProdutoComposicaoChild(this.value, this);"
                    onkeydown="if (isEnter(event)) loadProdutoComposicaoChild(this.value, this);"
                    onkeypress="return !(isEnter(event));" Width="50px"></asp:TextBox>

                <asp:Label ID="lblChild_DescrProdComposicao" runat="server"></asp:Label>

                <input id="img_PesqProd_Composicao" type="image" onclick='<%# "getProdutoComposicaoChild("  + (Request["idPedido"] != null ? Request["idPedido"] : "0") + ", this); return false;" %>' src="../Images/Pesquisar.gif" />

                <asp:HiddenField ID="hdfChild_ValMinComposicao" runat="server" />
                <asp:HiddenField ID="hdfChild_IsVidroComposicao" runat="server" />
                <asp:HiddenField ID="hdfChild_TipoCalcComposicao" runat="server" />
                <asp:HiddenField ID="hdfChild_IsAluminioComposicao" runat="server" />
                <asp:HiddenField ID="hdfChild_M2MinimoComposicao" runat="server" />
                <asp:HiddenField ID="hdfChild_AliquotaIcmsProdComposicao" runat="server" />
                <asp:HiddenField ID="hdfChild_ValorIcmsProdComposicao" runat="server" />
                <asp:HiddenField ID="hdfChild_CustoProdComposicao" runat="server" Value='<%# Eval("CustoCompraProduto") %>' />
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
            <ItemTemplate>
                <asp:Label ID="lbl_QtdeComposicao" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                <asp:Label ID="lbl_QtdeAmbienteComposicao" runat="server"></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtChild_QtdeComposicaoIns" runat="server" onblur="calcM2ProdComposicaoChild(this); return verificaEstoqueComposicaoChild(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfChild_TipoCalcComposicao', 'input').value), true);"
                    Text='<%# Bind("Qtde") %>' Width="50px"></asp:TextBox>
                <asp:Label ID="lbl_QtdeAmbienteComposicao" runat="server"></asp:Label>
                <uc5:ctrlDescontoQtde ID="ctrl_DescontoQtdeComposicao" runat="server" Callback="calcTotalProdComposicaoChild"
                    CallbackValorUnit="calcTotalProdComposicaoChild" ValidationGroup="produtoComposicaoChild" PercDescontoQtde='<%# Bind("PercDescontoQtde") %>'
                    ValorDescontoQtde='<%# Bind("ValorDescontoQtde") %>' OnLoad="ctrl_DescontoQtdeComposicao_Load" />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txtChild_QtdeComposicaoIns" runat="server" onkeydown="if (isEnter(event)) calcM2ProdComposicaoChild(this);"
                    onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfChild_TipoCalcComposicao', 'input').value), true);"
                    onblur="calcM2ProdComposicaoChild(this); return verificaEstoqueComposicaoChild(this);" Width="50px"></asp:TextBox>
                <asp:Label ID="lbl_QtdeAmbienteComposicao" runat="server"></asp:Label>
                <uc5:ctrlDescontoQtde ID="ctrl_DescontoQtdeComposicao" runat="server" Callback="calcTotalProdComposicaoChild"
                    ValidationGroup="produtoComposicaoChild" CallbackValorUnit="calcTotalProdComposicaoChild" OnLoad="ctrl_DescontoQtdeComposicao_Load" />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
            <ItemTemplate>
                <asp:Label ID="lbl_larguraComposicao" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtChild_LarguraComposicaoIns" runat="server" onblur="calcM2ProdComposicaoChild(this);" onkeypress="return soNumeros(event, true, true);"
                    Text='<%# Bind("Largura") %>' Enabled='<%# Eval("LarguraEnabled") %>' Width="50px"></asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txtChild_LarguraComposicaoIns" runat="server" onkeypress="return soNumeros(event, true, true);"
                    onblur="calcM2ProdComposicaoChild(this);" Width="50px"></asp:TextBox>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
            <ItemTemplate>
                <asp:Label ID="lbl_alturaComposicao" runat="server" Text='<%# Bind("AlturaLista") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:TextBox ID="txtChild_AlturaComposicaoIns" runat="server" onblur="calcM2ProdComposicaoChild(this); return verificaEstoqueComposicaoChild(this);"
                    Text='<%# Bind("Altura") %>' onchange="FindControl('hdfChild_AlturaRealComposicao', 'input').value = this.value"
                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfChild_TipoCalcComposicao', 'input').value), true);"
                    Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"></asp:TextBox>
                <asp:HiddenField ID="hdfChild_AlturaRealComposicao" runat="server" Value='<%# Bind("AlturaReal") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txtChild_AlturaComposicaoIns" runat="server" onblur="calcM2ProdComposicaoChild(this); return verificaEstoqueComposicaoChild(this);"
                    Width="50px" onchange="FindControl('hdfChild_AlturaRealComposicao', 'input').value = this.value"
                    onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfChild_TipoCalcComposicao', 'input').value), true);"></asp:TextBox>
                <asp:HiddenField ID="hdfChild_AlturaRealComposicaoIns" runat="server" />
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
            <ItemTemplate>
                <asp:Label ID="lbl_totMComposicao" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <asp:Label ID="llbChild_TotM2ComposicaoIns" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                <asp:HiddenField ID="hdfChild_TotMComposicao" runat="server" Value='<%# Eval("TotM") %>' />
                <asp:HiddenField ID="hdfChild_TamanhoMaximoObraComposicao" runat="server" />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:Label ID="llbChild_TotM2ComposicaoIns" runat="server"></asp:Label>
                <asp:HiddenField ID="hdfChild_TamanhoMaximoObraComposicao" runat="server" />
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotM2Calc">
            <EditItemTemplate>
                <asp:Label ID="lbl_TotM2CalcComposicao" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                <asp:HiddenField ID="hdfChild_TotM2CalcComposicao" runat="server" Value='<%# Eval("TotM2Calc") %>' />
                <asp:HiddenField ID="hdf_TotM2CalcSemChapaComposicao" runat="server" Value='<%# Eval("TotalM2CalcSemChapaString") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <asp:Label ID="lbl_TotM2CalcComposicaoIns" runat="server"></asp:Label>
                <asp:HiddenField ID="hdfChild_TotM2ComposicaoIns" runat="server" />
                <asp:HiddenField ID="hdfChild_TotM2CalcComposicaoIns" runat="server" />
                <asp:HiddenField ID="hdfChild_TotM2CalcSemChapaComposicaoIns" runat="server" />
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
                <asp:TextBox ID="txtChild_ValorComposicaoIns" runat="server" onblur="calcTotalProdComposicaoChild(this);" onkeypress="return soNumeros(event, false, true);"
                    Text='<%# Bind("ValorVendido") %>' Width="50px" OnLoad="txt_ValorInsComposicao_Load"></asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txtChild_ValorComposicaoIns" runat="server" onkeydown="if (isEnter(event)) calcTotalProdComposicaoChild(this);"
                    onkeypress="return soNumeros(event, false, true);" onblur="calcTotalProdComposicaoChild(this);"
                    Width="50px" OnLoad="txt_ValorInsComposicao_Load"></asp:TextBox>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
            <EditItemTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtChild_ProcComposicaoIns" runat="server" onblur="procAmbienteComposicaoChild=false; loadProcComposicaoChild(this, this.value);"
                                onkeydown="if (isEnter(event)) { procAmbienteComposicaoChild=false; loadProcComposicaoChild(this, this.value); }"
                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="procAmbienteComposicaoChild=false; return selProcComposicaoChild(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfChild_IdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtChild_ProcComposicaoIns" runat="server" onblur="procAmbienteComposicaoChild=false; loadProcComposicaoChild(this, this.value);"
                                onkeydown="if (isEnter(event)) { procAmbienteComposicaoChild=false; loadProcComposicaoChild(this, this.value); }"
                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="procAmbienteComposicaoChild=false; return selProcComposicaoChild(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfChild_IdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
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
                            <asp:TextBox ID="txtChild_AplComposicaoIns" runat="server" onblur="aplAmbienteComposicaoChild=false; loadAplComposicaoChild(this, this.value);"
                                onkeydown="if (isEnter(event)) { aplAmbienteComposicaoChild=false; loadAplComposicaoChild(this, this.value); }" onkeypress="return !(isEnter(event));"
                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="aplAmbienteComposicaoChild=false; return selAplComposicaoChild(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfChild_IdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
            </EditItemTemplate>
            <FooterTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtChild_AplComposicaoIns" runat="server" onblur="aplAmbienteComposicaoChild=false; loadAplComposicaoChild(this, this.value);"
                                onkeydown="if (isEnter(event)) { aplAmbienteComposicaoChild=false; loadAplComposicaoChild(this, this.value); }" onkeypress="return !(isEnter(event));"
                                Width="30px"></asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="aplAmbienteComposicaoChild=false; return selAplComposicaoChild(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfChild_IdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
            </FooterTemplate>
            <ItemTemplate>
                <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ped. Cli." SortExpression="PedCli">
            <EditItemTemplate>
                <asp:TextBox ID="txtChild_PedCliComposicao" runat="server" MaxLength="50" Text='<%# Bind("PedCli") %>'
                    Width="50px"></asp:TextBox>
            </EditItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txtChild_PedCliComposicao" runat="server" MaxLength="50" Width="50px"></asp:TextBox>
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
                <div id='<%# "imgProdsComposto_" + Eval("IdProdPed") %>'>
                     <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/imagem.gif"
                        OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=pedido&idPedido=" + Eval("IdPedido") +"&idProdPed=" +  Eval("IdProdPed") +"&pecaAvulsa=" +  ((bool)Eval("IsProdLamComposicao") == false) + "\"); return false" %>'
                        ToolTip="Exibir imagem das peças" Visible='<%# (Eval("IsVidro").ToString() == "true")%>' />
                </div>
            </ItemTemplate>
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
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField>
            <EditItemTemplate>

                <asp:ImageButton ID="lnkChild_BenefComposicao" runat="server" OnClientClick='<%# "exibirBenefComposicaoChild(this, &#39;" + Eval("IdProdPedParent") + "_" + Eval("IdProdPed") + "&#39;); return false;" %>'
                    Visible='<%# Eval("BenefVisible") %>' ImageUrl="~/Images/gear_add.gif" />
                <table id="tb_ConfigVidroComposicao_<%# Eval("IdProdPedParent") + "_" + Eval("IdProdPed") %>" cellspacing="0" style="display: none;">
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtChild_EspessuraComposicao" runat="server" OnDataBinding="txtChild_EspessuraComposicao_DataBinding"
                                            onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Bind("Espessura") %>'></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrlChild_BenefEditarComposicao" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>'
                                ValidationGroup="produtoComposicaoChild" OnInit="ctrl_Benef_Load" Redondo='<%# Bind("Redondo") %>'
                                CallbackCalculoValorTotal="setValorTotalComposicaoChild" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left"></td>
                    </tr>
                </table>

                <script type="text/javascript">
                    <%# "calculaTamanhoMaximoComposicaoChild(" + Request["idPedido"] != null ? Request["idPedido"] : "0" + ");"  %>
                </script>

            </EditItemTemplate>
            <FooterTemplate>

                <asp:ImageButton ID="lnkChild_BenefComposicao" runat="server" Style="display: none;" OnClientClick='<%# "exibirBenefComposicaoChild(this, &#39;" + IdProdPed + "_0&#39;); return false;" %>'
                    ImageUrl="~/Images/gear_add.gif" />
                <table id="tb_ConfigVidroComposicao_<%# IdProdPed + "_0" %>" cellspacing="0" style="display: none;">
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtChild_EspessuraComposicao" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="30px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrlChild_BenefInserirComposicao" runat="server" OnInit="ctrl_Benef_Load" CallbackCalculoValorTotal="setValorTotalComposicaoChild"
                                ValidationGroup="produtoComposicaoChild" />
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
                <asp:ImageButton ID="lnk_InsProdComposicao" runat="server" OnClick="lnk_InsProdComposicao_Click" ImageUrl="../Images/ok.gif" OnClientClick='<%# "if (!onSaveProdComposicaoChild(this, &#39;" + IdProdPed + "_0&#39;)) return false;" %>' />
            </FooterTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerStyle CssClass="pgr"></PagerStyle>
    <EditRowStyle CssClass="edit"></EditRowStyle>
    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
</asp:GridView>

<asp:HiddenField runat="server" ID="hdfChild_IdProdPed" />
<asp:HiddenField ID="hdfChild_IdProdComposicao" runat="server" />

<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdXPed" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedido"
    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
    OnDeleted="odsProdXPed_Deleted" SelectCountMethod="GetCount" SelectMethod="GetList"
    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
    InsertMethod="Insert" UpdateMethod="UpdateComTransacao" OnUpdated="odsProdXPed_Updated">
    <SelectParameters>
        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        <asp:ControlParameter ControlID="hdfIdAmbiente" Name="idAmbientePedido" PropertyName="Value" Type="UInt32" />
        <asp:Parameter Name="prodComposicao" DefaultValue="true" />
        <asp:ControlParameter Name="idProdPedParent" ControlID="hdfChild_IdProdPed" PropertyName="Value" />
    </SelectParameters>
</colo:VirtualObjectDataSource>

<script type="text/javascript">
    
    $(document).ready(function(){
        if (FindControl("imb_AtualizarComposicao", "input") != null && FindControl("lblChild_CodProdComposicaoIns", "span") != null)
            loadProdutoComposicaoChild(FindControl("lblChild_CodProdComposicaoIns", "span").innerHTML, FindControl("imb_AtualizarComposicao", "input"), false);

    })

</script>
