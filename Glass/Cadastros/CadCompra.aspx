<%@ Page Title="Cadastro de Compra" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadCompra.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCompra" %>

<%@ Register Src="../Controls/ctrlTextBoxFloat.ascx" TagName="ctrlTextBoxFloat" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlParcelas.ascx" TagName="ctrlParcelas" TagPrefix="uc3" %>
<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc5" %>

<%@ Register Src="../Controls/ctrlSelProduto.ascx" TagName="ctrlSelProduto" TagPrefix="uc6" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" >

        // Pega o id do forma de pagto boleto
        var codBoleto = MetodosAjax.GetIdFormaPagto("boleto").value;
        var codCheque = MetodosAjax.GetIdFormaPagto("cheque").value;
        var idFornec = <%= GetIdFornec() %>;
var numeroParcelasMax = <%= Glass.Configuracoes.FinanceiroConfig.Compra.NumeroParcelasCompra %>;
        var loading = false;

        function getNomeControleBenef()
        {
            var nomeControle = "<%= NomeControleBenef() %>";
    nomeControle = FindControl(nomeControle + "_tblBenef", "table");
    
    if (nomeControle == null)
        return null;
    
    nomeControle = nomeControle.id;
    return nomeControle.substr(0, nomeControle.lastIndexOf("_"));
}

function exibirBenef(botao)
{
    for (iTip = 0; iTip < 2; iTip++)
    {
        TagToTip('tbConfigVidro', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true, 
            CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true, 
            FIX, [botao, -10 - getTableWidth('tbConfigVidro'), 7]);
    }
}

function setValorTotal(valor, custo)
{
    if (exibirControleBenef(getNomeControleBenef()))
    {        
        var lblValorBenef = FindControl("lblValorBenef", "span");
        lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
    }
}

function calcularValorTotalFinal()
{
    if (FindControl("txtTotalFinal", "input") != null)
    {
        /*var valorEntrada = FindControl("lblValorEntrada", "span") != null ?
        FindControl("lblValorEntrada", "span").innerText.replace("R$", "").replace(" ", "").replace(".","").replace(",",".") :
        FindControl("ctrValEntrada_txtNumber", "input") != null ?
        FindControl("ctrValEntrada_txtNumber", "input").value.replace(".","").replace(",",".") :
        FindControl("ctrValEntradaIns_txtNumber", "input") != null ?
        FindControl("ctrValEntradaIns_txtNumber", "input").value.replace(".","").replace(",",".") : "0";*/
        var subTotal = FindControl("txtTotal", "input") != null ?
        FindControl("txtTotal", "input").value.replace("R$", "").replace(" ", "").replace(".","").replace(",",".") : "0";
        var valorTributado = FindControl("txtValorTributado", "input") != null ?
        FindControl("txtValorTributado", "input").value.replace("R$", "").replace(" ", "").replace(".","").replace(",",".") : "0";
        valorTributado = parseFloat(valorTributado == "" ? "0" : valorTributado);
        FindControl("txtTotalFinal", "input").value = "R$ " + (parseFloat(subTotal) - valorTributado/* - parseFloat(valorEntrada)*/).toFixed(2).toString().replace(".",",");
    }
}

// Função chamada para calcular o acréscimo das parcelas
function calcAcrescimo()
{
    var tipoCompra = FindControl("drpTipoCompra", "select");

    if (tipoCompra == null || tipoCompra.value != 2)
        return;
    
    var outrasDespesas = FindControl("txtOutrasDespesas", "input").value;
    var frete = FindControl("txtFrete", "input").value;
    var icms = FindControl("txtIcms", "input").value;
    var seguro = FindControl("txtSeguro", "input").value;
    var ipi = FindControl("txtIpi", "input").value;
    
    outrasDespesas = parseFloat(outrasDespesas.toString().replace(',', '.'));
    frete = parseFloat(frete.toString().replace(',', '.'));
    icms = parseFloat(icms.toString().replace(',', '.'));
    seguro = parseFloat(seguro.toString().replace(',', '.'));
    ipi = parseFloat(ipi.toString().replace(',', '.'));
    
    FindControl("hdfAcrescimo", "input").value = (outrasDespesas + frete + icms + seguro + ipi).toFixed(2).replace('.', ',');
    Parc_visibilidadeParcelas("<%= dtvCompra.ClientID %>_ctrlParcelas1");
}

// Carrega dados do produto com base no código do produto passado
function selProduto(nomeControle, idProd)
{
    if (idProd == "")
        return false;

    try
    {
        var retorno = CadCompra.GetProduto(FindControl("hdfIdLoja", "input").value, idFornec, idProd).value.split('|');
        
        if (retorno[0] == "Erro")
        {
            alert(retorno[1]);
            FindControl("txtCodProd", "input").value = "";
            return false;
        }
        
        FindControl("hdfCodProd", "input").value = retorno[9]; 
        FindControl("lblInfoEstoque", "span").innerHTML = retorno[1];
        FindControl("hdfIsVidro", "input").value = retorno[2]; // Informa se o produto é vidro
        FindControl("txtValorIns", "input").value = retorno[4]; // Informa se o produto é vidro
        FindControl("hdfCustoProd", "input").value = retorno[5];
        FindControl("txtValorIns", "input").disabled = retorno[6] == "true"; // Desabilita o campo de valor unitário
        FindControl("txtEspessura", "input").value = retorno[3];

        var tipoCalc = eval(nomeControle).DadosProduto().TipoCalculo;        
        var nomeControle = getNomeControleBenef();
                    
        if (FindControl("lnkBenef", "a") != null && nomeControle != null && nomeControle.indexOf("Inserir") > -1)
            FindControl("lnkBenef", "a").style.display = exibirControleBenef(nomeControle) ? "" : "none";

        // Se o produto não for vidro, desabilita os textboxes largura e altura,
        // mas se o produto for tipoCalc=ML AL e a empresa trabalhar com venda de alumínio, deixa o campo altura habilitado
        // no metro linear ou se o produto for tipoCalc=ML, deixa os campos altura e largura habilitados
        var cAltura = FindControl("txtAlturaIns", "input");
        var cLargura = FindControl("txtLarguraIns", "input");
        var cTotM2 = FindControl("txtTotM2", "input");
        cAltura.disabled = CalcProd_DesabilitarAltura(tipoCalc);
        cLargura.disabled = CalcProd_DesabilitarLargura(tipoCalc);
        cTotM2.disabled = cLargura.disabled;
        cAltura.value = "";
        cLargura.value = "";
        
        // O campo altura e largura devem sempre ser atribuídos pois caso seja selecionado um box e logo após seja selecionado um kit 
        // por exemplo, ao inserí-lo ele estava ficando com o campo altura, largura e m² preenchidos apesar de ser calculado por qtd
        if (retorno[7] != "" || retorno[2] == "false") FindControl("txtAltura", "input").value = retorno[7];
        if (retorno[8] != "" || retorno[2] == "false") FindControl("txtLargura", "input").value = retorno[8];

        // Chamado 14457: Sempre que carregar o produto, apaga o campo Qtd para não permitir inserir decimal onde não pode
        FindControl("txtQtdeIns", "input").value = "";
    }
    catch(err)
    {
        alert(err);
    }
}

// Evento acionado ao trocar o tipo de compra (à vista/à prazo)
function tipoCompraChange(control) {
    if (control == null)
        return;

    var cNumParc = FindControl("txtNumParc", "input");

    // Se for à vista, Desabilita o campo número de parcelas se for compra à vista
    FindControl("txtNumParc", "input").disabled = control.value != 2;

    // Desabilita valor da parcela e data base de vencimento se for compra a vista ou 
    // se o numero de parcelas for < 6 && != 0
    FindControl("ctrValorParc_txtNumber", "input").disabled = control.value == 1 || (cNumParc < numeroParcelasMax && cNumParc != 0);

    // Se for à prazo e o número de parcelas estiver 0, muda para 1
    if (control.value == 2 && cNumParc.value == 0)
        cNumParc.value = 1;
    
    if(control.value == <%= (int)Glass.Data.Model.Compra.TipoCompraEnum.AntecipFornec %>)
    {
        FindControl("divAntecipFornec","div").style.display = "";
        FindControl("trFormaPgto","tr").style.display = "none";
        FindControl("drpFormaPagto","select").value = "0";
        
        var idAntecip = FindControl("hdfIdAntecipFornec", "input").value;
        var descrAntecip = CadCompra.GetDescrAntecipFornec(idAntecip).value;
        FindControl("txtAntecip", "input").value = descrAntecip;  
    }    
    else
    {
        FindControl("divAntecipFornec","div").style.display = "none";
        FindControl("trFormaPgto","tr").style.display = "";
        
        FindControl("hdfIdAntecipFornec","input").value = "";
        FindControl("txtAntecip","input").value = "";
    }
    
    exibeParcelas();
}

function exibeParcelas()
{
    var drpTipoCompra = FindControl("drpTipoCompra", "select");
    var txtNumParc = FindControl("txtNumParc", "input");
    var txtValorParc = FindControl("ctrValorParc_txtNumber", "input");
    var txtDataBaseVenc = FindControl("ctrlDataBaseVenc_txtData", "input");
    var imgDataBaseVenc = FindControl("ctrlDataBaseVenc_imgData", "input");
    var hdfPrazos = FindControl("hdfPrazos", "input");
    
    var prazos=new Array();
    for(i = 0; i < parseInt(txtNumParc.value) + 1; i++)
    {
        prazos[i - 1] = i * 30;
    }
    
    hdfPrazos.value = prazos.toString();
    
    var exibirParcelas = drpTipoCompra.value == 2 && parseInt(txtNumParc.value, 10) <= numeroParcelasMax;
    
    if (FindControl("txtTotalFinal", "input").value == "R$ 0,00")
        exibirParcelas = false;
    
    FindControl("hdfExibirParcelas", "input").value = exibirParcelas;    
    txtValorParc.disabled = exibirParcelas;        
    
    Parc_visibilidadeParcelas("<%= dtvCompra.ClientID %>_ctrlParcelas1");
    if (!loading)
        Parc_atualizaFormasPagto("<%= dtvCompra.ClientID %>_ctrlParcelas1");
}

function formaPagtoChange(control) {
    if (control == null)
        return;
    
    // Se a forma de pagamento for cheque/boleto, mostra os campos numBoleto nas parcelas, senão, esconde
    var display = control.value == codBoleto || control.value == codCheque ? "" : "none";
    for (i = 0; i < <%= dtvCompra.FindControl("ctrlParcelas1") != null ? ((Glass.UI.Web.Controls.ctrlParcelas)dtvCompra.FindControl("ctrlParcelas1")).NumParcelas : 0 %>; i++)
            FindControl("ctrlParcelas1_Parc" + (i + 1) + "_Linha4", "tr").style.display = display;
    }

    var inserindo = false;
    function onInsert() {
        debugger;
        /* Chamado 15918.
         * Evita que a compra seja inserida duplicadamente. */
        if (!inserindo)
            inserindo = true;
        else
            return false;

        if (!validate()) {
            inserindo = false;
            return false;
        }
        
        var fornec = FindControl("hdfFornec", "input").value;
        var planoConta = FindControl("ddlPlanoConta", "select").value;

        // Verifica se o fornecedor foi selecionado
        if (fornec == "" || fornec == null) {
            alert("Informe o Fornecedor.");
            inserindo = false;
            return false;
        }
    
        // Verifica se o plano de contas foi selecionado
        if (planoConta == "" || planoConta == null)
        {
            alert("Informe o Plano de Contas.");
            inserindo = false;
            return false;
        }
    
        var prazo = parseInt(FindControl("hdfPrazoFornec", "input").value, 10);
        var numParcelas = parseInt(FindControl("txtNumParc", "input").value, 10);
    
        if (prazo != "" && numParcelas > prazo)
        {
            if (prazo > 0) {
                inserindo = false;
                alert("Esse fornecedor aceita apenas " + prazo + " parcela(s).");
            }
            else {
                inserindo = false;
                alert("Esse fornecedor aceita apenas compras à vista.")
            }
        
            return false;
        }

        return true;
    }

    // Acionado quando a Compra está para ser salva
    function onSave() {
        if (!validate())
            return false;
    
        var fornec = FindControl("hdfFornec", "input").value;
        var planoConta = FindControl("ddlPlanoConta", "select").value;

        // Verifica se o Fornecedor foi selecionado
        if (fornec == "" || fornec == null)
        {
            alert("Informe o Fornecedor.");
            return false;
        }
    
        // Verifica se o plano de contas foi selecionado
        if (planoConta == "" || planoConta == null)
        {
            alert("Informe o Plano de Contas.");
            return false;
        }

        // Se for compra a prazo
        if (FindControl("drpTipoCompra", "select").value == "2")
        {
            var numParc = FindControl("txtNumParc", "input").value;
            var valorParc = FindControl("ctrValorParc_txtNumber", "input").value;
            var dataBaseVenc = FindControl("ctrlDataBaseVenc_txtData", "input").value;

            if (numParc == "0" || numParc == "") {
                alert("Informe o número de parcelas.");
                return false;
            }
            else if (parseInt(numParc) > numeroParcelasMax) {
                if (valorParc == "" || valorParc == "0" || valorParc == 0) {
                    alert("Informe o valor da parcela.");
                    return false;
                }
                else if (dataBaseVenc == "") {
                    alert("Informe a data de vencimento base das parcelas.");
                    return false;
                }
            }
        }
    
        var prazo = parseInt(FindControl("hdfPrazoFornec", "input").value, 10);
        var numParcelas = parseInt(FindControl("txtNumParc", "input").value, 10);
    
        if (prazo != "" && numParcelas > prazo)
        {
            if (prazo > 0)
                alert("Esse fornecedor aceita apenas " + prazo + " parcela(s).");
            else
                alert("Esse fornecedor aceita apenas compras à vista.")
            
            return false;
        }
    
        return true;
    }

    // Chamado quando um produto está para ser inserido na Compra
    function onSaveProd()
    {
        if (!validate("produto"))
            return false;
    
        var codProd = FindControl("ctrlSelProd_ctrlSelProdBuscar_txtDescr", "input").value;
        var valor = FindControl("txtValorIns", "input").value;
        var qtde = FindControl("txtQtdeIns", "input").value;
        
        if (FindControl("tbConfigVidro", "table").style.display == "block")
        {
            alert("Aplique as alterações no beneficiamento antes de salvar o item.");
            return false;
        }
    
        if (codProd == "")
        {
            alert("Informe o código do produto.");
            return false;
        }
        else if (valor == "0" || valor == "")
        {
            alert("Informe o valor vendido.");
            return false;
        }
        else if (qtde == "0" || qtde == "")
        {
            alert("Informe a quantidade.");
            return false;
        }
    
        FindControl("txtAltura", "input").disabled = false;
        FindControl("txtLargura", "input").disabled = false;
        FindControl("txtValorIns", "input").disabled = false;
        return true;
    }

    // Função chamada quando o produto está para ser atualizado
    function onUpdateProd()
    {
        if (!validate("produto"))
            return false;
    
        if (FindControl("tbConfigVidro", "table").style.display == "block")
        {
            alert("Aplique as alterações no beneficiamento antes de salvar o item.");
            return false;
        }
    
        var valor = FindControl("txtValorIns", "input").value;
        var qtde = FindControl("txtQtdeIns", "input").value;
        var idProd = FindControl("hdfIdProduto", "input").value;
        var codInterno = FindControl("hdfCodInterno", "input").value;
    
        var retorno = CadCompra.GetProduto(idFornec, codInterno, idProd).value.split(';');
    
        if (retorno[0] == "Erro")
        {
            alert(retorno[1]);
            return false;
        }    
        else if (valor == "0" || valor == "")
        {
            alert("Informe o valor vendido.");
            return false;
        }
        else if (qtde == "0" || qtde == "")
        {
            alert("Informe a quantidade.");
            return false;
        }

        FindControl("txtValorIns", "input").disabled = false;
    
        return true;
    }

    // Calcula em tempo real a metragem quadrada do produto
    function calcM2Prod()
    {    
        try
        {
            var idProd = FindControl("hdfIdProduto", "input").value;
            var altura = FindControl("txtAlturaIns", "input").value;
            var largura = FindControl("txtLarguraIns", "input").value;
            var qtde = FindControl("txtQtdeIns", "input").value;
            var tipoCalc = FindControl("hdfTipoCalc", "input").value;
        
            if (altura == "" || largura == "" || qtde == "" || altura == "0")
                return false;
        
            var adicVidroRedondoAte12mm = '<%= Glass.Configuracoes.Geral.AdicionalVidroRedondoAte12mm %>';
            var adicVidroRedondoAcima12mm = '<%= Glass.Configuracoes.Geral.AdicionalVidroRedondoAcima12mm %>';

            // Se a opção vidro redondo estiver marcado, adiciona 50mm na largura e na altura,
            // bisote e lapidação não cobram sobre este acréscimo
            if (FindControl("Redondo_chkSelecao", "input") != null && FindControl("Redondo_chkSelecao", "input").checked) {
                if (altura != "" && largura != "" &&
                    parseInt(altura) > 0 && parseInt(largura) > 0 &&
                    parseInt(altura) != parseInt(largura) && FindControl("Redondo_chkSelecao", "input").checked) {
                        alert('O beneficiamento Redondo pode ser marcado somente em peças de medidas iguais.');
                        FindControl("Redondo_chkSelecao", "input").checked = false;
                        return false;
                    }

                var esp = FindControl("txtEspessura", "input").value;
                var addValor = esp < 12 ? adicVidroRedondoAte12mm : adicVidroRedondoAcima12mm;

                altura = parseInt(altura) + parseInt(addValor);
                largura = parseInt(largura) + parseInt(addValor);
            }
            
            if (tipoCalc == 2 || tipoCalc == 10)
                FindControl("txtTotM2", "input").value = CadCompra.CalcM2(idProd, altura, largura, qtde, tipoCalc).value;
        
            calcTotalProd();
        }
        catch(err)
        {
    
        }
    }

        // Calcula em tempo real o valor total do produto
        function calcTotalProd()
        {
            try
            {
                if (FindControl("chkNaoCobrarVidro", "input").checked)
                {
                    FindControl("lblTotalIns", "span").innerHTML = "R$ 0,00";
                    return;
                }
        
                var valorIns = FindControl("txtValorIns", "input").value;
        
                if (valorIns == "")
                    return;
    
                var totM2 = FindControl("txtTotM2", "input").value;
                var totM2Calc = new Number(FindControl("txtTotM2", "input").value.replace(',', '.')).toFixed(3);
                var total = new Number(valorIns.replace(',', '.'));
                var qtde = new Number(FindControl("txtQtdeIns", "input").value.replace(',', '.'));
                var altura = new Number(FindControl("txtAlturaIns", "input").value.replace(',', '.'));
                var largura = new Number(FindControl("txtLarguraIns", "input").value.replace(',', '.'));
                var tipoCalc = FindControl("hdfTipoCalc", "input").value;
        
                var retorno = CalcProd_CalcTotalProd(valorIns, totM2, totM2Calc, 0, total, qtde, altura, FindControl("txtAlturaIns", "input"), largura, false, tipoCalc);
                if (retorno != "")
                    FindControl("lblTotalIns", "span").innerHTML = retorno;
            }
            catch(err)
            {
    
            }
        }

        function arrayContains(array, value)
        {
            for (ac = 0; ac < array.length; ac++)
                if (array[ac] == value)
                    return true;
    
            return false;
        }

        function getFornec(idFornec)
        {
            if (idFornec == null || idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornec(idFornec.value).value.split(';');
            var isFornecProprio = false;
    
            if (retorno[0] == "Erro")
            {
                //if (!loading)
                alert(retorno[1]);
        
                idFornec.value = "";
                FindControl("txtNomeFornec", "input").value = "";
                FindControl("hdfFornec", "input").value = "";
                FindControl("hdfPrazoFornec", "input").value = "";
                return false;
            }
            else
                isFornecProprio = CadCompra.IsFornecedorProprio(idFornec.value).value == "true";
    
            FindControl("txtNomeFornec", "input").value = retorno[1];
            FindControl("hdfFornec", "input").value = idFornec.value;
            FindControl("hdfPrazoFornec", "input").value = retorno[2];
            if (retorno[3] != "" && FindControl("ddlPlanoConta", "select").value == "")
                FindControl("ddlPlanoConta", "select").value = retorno[3];
    
            // Verifica se o fornecedor da compra é a própria empresa, para esconder as opções à vista e à prazo
            if (<%= Glass.Configuracoes.FinanceiroConfig.Compra.CompraSemValores.ToString().ToLower() %>){
                var semValores = new Array(3, 4);
        
                var tipoCompra = FindControl("drpTipoCompra", "select");
                var atual = tipoCompra.value;
                var isAtualSemValor = arrayContains(semValores, atual);
        
                for (i = 0; i < tipoCompra.options.length; i++)
                {
                    var isSemValor = arrayContains(semValores, tipoCompra.options[i].value);
                    var exibir = (isFornecProprio && isSemValor) || (!isFornecProprio && !isSemValor);
            
                    tipoCompra.options[i].style.display = exibir ? "" : "none";
                    tipoCompra.options[i].disabled = !exibir;
            
                    if (atual == tipoCompra.options[i].value)
                        atual = exibir ? atual : isAtualSemValor ? 2 : 3;
                }
        
                tipoCompra.value = atual;
            }
        }

        function openSelAntecipFornec(){
        
            var idFornec = FindControl("txtNumFornec","input").value;
        
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

        function setPlanoConta(idConta, descricao) {
            var planoConta = FindControl("ddlPlanoConta", "select");

            if (planoConta == null)
                return false;
            
            planoConta.value = idConta;
        }
    
        function tamanhoMaximo(control, tamanhoLimite){
            if (control == null || control.value == null) {
                return false;
            }
            debugger;
            if (control.value.toString().length >= tamanhoLimite) {
                control.value = control.value.toString().substring(0, tamanhoLimite - 1)
                return false;
            }
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td>
                <table style="width: 100%">
                    <tr>
                        <td align="center">
                            <asp:DetailsView ID="dtvCompra" runat="server" AutoGenerateRows="False" DataSourceID="odsCompra"
                                DefaultMode="Insert" GridLines="None" OnDataBound="dtvCompra_DataBound" OnItemCommand="dtvCompra_ItemCommand">
                                <Fields>
                                    <asp:TemplateField>
                                        <EditItemTemplate>
                                            <table cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Fornecedor
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                            onblur="getFornec(this);" Text='<%# Eval("IdFornec") %>'></asp:TextBox>
                                                        <asp:TextBox ID="txtNomeFornec" runat="server" ReadOnly="True" Text='<%# Eval("NomeFornec") %>'
                                                            Width="250px"></asp:TextBox>
                                                        <asp:LinkButton ID="lnkSelFornec" runat="server" OnClientClick="openWindow(570, 760, '../Utils/SelFornec.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Valor Entrada
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValEntrada" runat="server" Value='<%# Bind("ValorEntrada") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Plano de Conta
                                                    </td>
                                                    <td align="left" nowrap="nowrap" valign="middle">
                                                        <asp:DropDownList ID="ddlPlanoConta" runat="server" DataSourceID="odsPlanoConta"
                                                            DataTextField="DescrPlanoGrupo" DataValueField="IdConta" SelectedValue='<%# Bind("IdConta") %>'
                                                            AppendDataBoundItems="True">
                                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:LinkButton ID="lnkSelPlanoConta" runat="server" OnClientClick="openWindow(600, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">NF/Pedido
                                                    </td>
                                                    <td align="left" nowrap="nowrap" valign="middle">
                                                        <asp:TextBox ID="txtNf" runat="server" MaxLength="20" Text='<%# Bind("Nf") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Tipo Compra
                                                    </td>
                                                    <td align="left" nowrap="nowrap" valign="middle">
                                                        <table cellpadding="0" cellspacing="0" style="width: 100%">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpTipoCompra" runat="server" onchange="tipoCompraChange(this);"
                                                                        SelectedValue='<%# Bind("TipoCompra") %>' OnLoad="drpTipoCompra_Load">
                                                                        <asp:ListItem Value="2">À Prazo</asp:ListItem>
                                                                        <asp:ListItem Value="1">À Vista</asp:ListItem>
                                                                        <asp:ListItem Value="3">Estoque</asp:ListItem>
                                                                        <asp:ListItem Value="4">Produção</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                    <div id="divAntecipFornec" style="display: none">
                                                                        <asp:TextBox ID="txtAntecip" Enabled="false" runat="server" Width="250px" />
                                                                        <asp:ImageButton ID="imbBuscaAntecip" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                                            OnClientClick="openSelAntecipFornec(); return false;" />
                                                                        <asp:HiddenField ID="hdfIdAntecipFornec" runat="server" Value='<%# Bind("IdAntecipFornec") %>' />
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Num Parc.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtNumParc" runat="server" onblur="exibeParcelas()" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("NumParc") %>' Width="50px"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfPrazos" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr id="trFormaPgto">
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Forma Pagto.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFormaPagto" runat="server" onchange="formaPagtoChange(this);"
                                                                        DataSourceID="odsFormaPagto" DataTextField="Descricao" DataValueField="IdFormaPagto"
                                                                        SelectedValue='<%# Bind("IdFormaPagto") %>'>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>&nbsp;<asp:CheckBox ID="chkBoletoChegou" runat="server" Checked='<%# Bind("BoletoChegou") %>'
                                                                    Text="Boleto Chegou" />
                                                                    &nbsp;
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Valor Parc.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValorParc" runat="server" onblur="calcValParc(true);"
                                                            Value='<%# Bind("ValorParc") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Loja
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                                            DataValueField="IdLoja" SelectedValue='<%# Bind("IdLoja") %>'>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Data Base Venc.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <uc5:ctrlData ID="ctrlDataBaseVenc" runat="server" ReadOnly="ReadWrite" DataNullable='<%# Bind("DataBaseVenc") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Frete
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtFrete" runat="server" MaxLength="10" onkeypress="return soNumeros(event, false, true)"
                                                            onblur="calcAcrescimo()" Text='<%# Bind("Frete") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">ICMS
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIcms" runat="server" MaxLength="10" onkeypress="return soNumeros(event, false, true)"
                                                            onblur="calcAcrescimo()" Text='<%# Bind("Icms") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Seguro
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtSeguro" runat="server" MaxLength="10" onkeypress="return soNumeros(event, false, true)"
                                                            onblur="calcAcrescimo()" Text='<%# Bind("Seguro") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">IPI
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIpi" runat="server" MaxLength="10" onkeypress="return soNumeros(event, false, true)"
                                                            onblur="calcAcrescimo()" Text='<%# Bind("Ipi") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Desconto
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtDesconto" runat="server" onkeypress="return soNumeros(event, false, true)"
                                                            Text='<%# Bind("Desconto") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">SubTotal
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtTotal" runat="server" ReadOnly="True" Text='<%# Eval("Total", "{0:C}") %>'></asp:TextBox>
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Outras Despesas
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtOutrasDespesas" runat="server" Text='<%# Bind("OutrasDespesas") %>'
                                                            onblur="calcAcrescimo()" onKeyPress="return soNumeros(event, false, true);" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="lblValorTributado" runat="server" OnLoad="lblValorTributado_Load">Valor Tributado</asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtValorTributado" runat="server" Text='<%# Bind("ValorTributado") %>' OnLoad="txtValorTributado_Load"
                                                            onKeyPress="return soNumeros(event, false, true);" onBlur="calcularValorTotalFinal(); exibeParcelas();" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Data entr. fábrica
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <uc5:ctrlData ID="ctrlDataFabrica" runat="server" ReadOnly="ReadWrite" ValidateEmptyText="true"
                                                            DataNullable='<%# Bind("DataFabrica") %>' ErrorMessage="Informe a data de entr. na fábrica." />
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Total
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTotalFinal" runat="server" ReadOnly="true" Text='<%# ((decimal)Eval("Total") - (decimal)Eval("ValorTributado")).ToString("C") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:CheckBox ID="chkContabil" runat="server" Checked='<%# Bind("Contabil") %>'
                                                            OnLoad="chkContabil_Load" Text="Contábil" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td id="parcelas" align="left" class="dtvHeader" colspan="4" nowrap="nowrap">
                                                        <uc3:ctrlParcelas ID="ctrlParcelas1" runat="server"
                                                            Adicionais='<%# Bind("BoletosParcelas") %>'
                                                            Datas='<%# Bind("DatasParcelas") %>' ExibirCampoAdicional="true"
                                                            FormasPagamento='<%# Bind("FormasPagtoParcelas") %>' IsCompra="true"
                                                            NumParcelasLinha="3" OnLoad="ctrlParcelas1_Load" ParentID="parcelas"
                                                            TituloCampoAdicional="Cheque/Boleto:"
                                                            Valores='<%# Bind("ValoresParcelas") %>' />
                                                        <asp:HiddenField ID="hdfCalcularParcelas" runat="server" Value="true" />
                                                        <asp:HiddenField ID="hdfDesconto" runat="server"
                                                            Value='<%# Eval("Desconto") %>' />
                                                        <asp:HiddenField ID="hdfExibirParcelas" runat="server" Value="true" />
                                                        <asp:HiddenField ID="hdfAcrescimo" runat="server" />
                                                        <asp:HiddenField ID="hdfAcrescimoAnterior" runat="server"
                                                            Value='<%# (decimal)Eval("OutrasDespesas") + (decimal)Eval("Frete") + (decimal)Eval("Icms") + (decimal)Eval("Seguro") + (decimal)Eval("Ipi") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td class="dtvHeader">Observação
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="100%" 
                                                            onKeyUp="return tamanhoMaximo(this, 350);"
                                                            onKeyDown="return tamanhoMaximo(this, 350);"
                                                            onblur="return tamanhoMaximo(this, 350);"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                                        <asp:HiddenField ID="hdfSituacao" runat="server" Value='<%# Bind("Situacao") %>' />
                                                        <asp:HiddenField ID="hdfFornec" runat="server" Value='<%# Bind("IdFornec") %>' />
                                                        <asp:HiddenField ID="hdfPrazoFornec" runat="server" Value='<%# Eval("TipoPagtoFornec") %>' />
                                                        <asp:HiddenField ID="hdfEstoqueBaixado" runat="server" Value='<%# Bind("EstoqueBaixado") %>' />
                                                    </td>
                                                </tr>
                                            </table>
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <table cellpadding="2" cellspacing="0">
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Fornecedor
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtNumFornec" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                                            onblur="getFornec(this);" Text='<%# Eval("IdFornec") %>' />
                                                        <asp:TextBox ID="txtNomeFornec" runat="server" ReadOnly="True" Text='<%# Eval("NomeFornec") %>'
                                                            Width="250px"></asp:TextBox>
                                                        <asp:LinkButton ID="lnkSelFornec" runat="server" OnClientClick="openWindow(570, 760, '../Utils/SelFornec.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Valor Entrada
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValEntradaIns" runat="server" Value='<%# Bind("ValorEntrada") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Plano de Conta
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:DropDownList ID="ddlPlanoConta" runat="server" DataSourceID="odsPlanoConta"
                                                            DataTextField="DescrPlanoGrupo" DataValueField="IdConta" SelectedValue='<%# Bind("IdConta") %>'
                                                            AppendDataBoundItems="True">
                                                            <asp:ListItem></asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:LinkButton ID="lnkSelPlanoConta" runat="server" OnClientClick="openWindow(600, 700, '../Utils/SelPlanoConta.aspx'); return false;">
                                                        <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">NF/Pedido
                                                    </td>
                                                    <td align="left" nowrap="nowrap" valign="middle">
                                                        <asp:TextBox ID="txtNf" runat="server" MaxLength="20" Text='<%# Bind("Nf") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Tipo Compra
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:DropDownList ID="drpTipoCompra" runat="server" SelectedValue='<%# Bind("TipoCompra") %>'
                                                            onchange="tipoCompraChange(this);" OnLoad="drpTipoCompra_Load">
                                                            <asp:ListItem Value="2">À Prazo</asp:ListItem>
                                                            <asp:ListItem Value="1">À Vista</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <div id="divAntecipFornec" style="display: none">
                                                            <asp:TextBox ID="txtAntecip" Enabled="false" runat="server" Width="250px" />
                                                            <asp:ImageButton ID="imbBuscaAntecip" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                                                OnClientClick="openSelAntecipFornec(); return false;" />
                                                            <asp:HiddenField ID="hdfIdAntecipFornec" runat="server" Value='<%# Bind("IdAntecipFornec") %>' />
                                                        </div>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Num Parc.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtNumParc" runat="server" onblur="calcValParc(true);" onkeypress="return soNumeros(event, true, true);"
                                                            Text='<%# Bind("NumParc") %>' Width="50px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr id="trFormaPgto">
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Forma Pagto.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <table cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:DropDownList ID="drpFormaPagto0" runat="server" DataSourceID="odsFormaPagto"
                                                                        DataTextField="Descricao" DataValueField="IdFormaPagto" onchange="setLnkChequeVisibility(this);"
                                                                        SelectedValue='<%# Bind("IdFormaPagto") %>'>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>&nbsp;&nbsp;<asp:CheckBox ID="chkBoletoChegou" runat="server" Checked='<%# Bind("BoletoChegou") %>'
                                                                    Text="Boleto Chegou" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Valor Parc.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <uc1:ctrlTextBoxFloat ID="ctrValorParc" runat="server" onblur="calcValParc(true);"
                                                            Value='<%# Bind("ValorParc") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Loja
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" DataTextField="NomeFantasia"
                                                            DataValueField="IdLoja" SelectedValue='<%# Bind("IdLoja") %>' OnDataBound="drpLoja_DataBound">
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Data Base Venc.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <uc5:ctrlData ID="ctrlDataBaseVenc" runat="server" ReadOnly="ReadWrite" DataNullable='<%# Bind("DataBaseVenc") %>' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Frete
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtFrete" runat="server" MaxLength="10" onkeypress="return soNumeros(event, false, true)"
                                                            Text='<%# Bind("Frete") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">ICMS
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIcms" runat="server" MaxLength="10" onkeypress="return soNumeros(event, false, true)"
                                                            Text='<%# Bind("Icms") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Seguro
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtSeguro" runat="server" MaxLength="10" onkeypress="return soNumeros(event, false, true)"
                                                            Text='<%# Bind("Seguro") %>'></asp:TextBox>
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">IPI
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtIpi" runat="server" MaxLength="10" onkeypress="return soNumeros(event, false, true)"
                                                            Text='<%# Bind("Ipi") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Outras Despesas
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtOutrasDespesas" runat="server" Text='<%# Bind("OutrasDespesas") %>'
                                                            onKeyPress="return soNumeros(event, false, true);" />
                                                    </td>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">
                                                        <asp:Label ID="lblValorTributado" runat="server" OnLoad="lblValorTributado_Load">Valor Tributado</asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="txtValorTributado" runat="server" Text='<%# Bind("ValorTributado") %>'
                                                            onKeyPress="return soNumeros(event, false, true);" OnLoad="txtValorTributado_Load" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Data entr. fábrica
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <uc5:ctrlData ID="ctrlDataFabrica" runat="server" ReadOnly="ReadWrite" ValidateEmptyText="true"
                                                            DataNullable='<%# Bind("DataFabrica") %>' />
                                                    </td>
                                                    <td colspan="2"></td>
                                                </tr>
                                                <tr>
                                                    <td align="left" class="dtvHeader" nowrap="nowrap">Contábil
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:CheckBox ID="chkContabil" runat="server" Checked='<%# Bind("Contabil") %>' OnLoad="chkContabil_Load" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td class="dtvHeader">Observação
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:TextBox ID="txtObs" runat="server" Text='<%# Bind("Obs") %>'
                                                            TextMode="MultiLine" Width="100%" 
                                                            onKeyUp="return tamanhoMaximo(this, 350);"
                                                            onKeyDown="return tamanhoMaximo(this, 350);"
                                                            onblur="return tamanhoMaximo(this, 350);"></asp:TextBox>
                                                        <asp:HiddenField ID="hdfFornec" runat="server" Value='<%# Bind("IdFornec") %>' />
                                                        <asp:HiddenField ID="hdfPrazoFornec" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <table cellpadding="2" cellspacing="2">
                                                <tr>
                                                    <td class="dtvHeader">Num. Compra
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNumCompra" runat="server" Text='<%# Eval("IdCompra") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">Fornecedor
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeFornecedor" runat="server" Text='<%# Eval("NomeFornec") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">Loja
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeLoja" runat="server" Text='<%# Eval("NomeLoja") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="dtvHeader">Funcionário
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNomeFunc" runat="server" Text='<%# Eval("DescrUsuCad") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">Tipo Compra
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoCompra" runat="server" Text='<%# Eval("DescrTipoCompra") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">Forma Pagto.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblFormaPagto" runat="server" Text='<%# (int)Eval("TipoCompra") != (int)Glass.Data.Model.Compra.TipoCompraEnum.AntecipFornec ? Eval("FormaPagto") : "" %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="dtvHeader">Valor Entrada
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblValorEnt" runat="server" Text='<%# Eval("ValorEntrada", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">Num. Parcela
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="Label9" runat="server" Text='<%# Eval("NumParc") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">Valor Parcela
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="Label10" runat="server" Text='<%# Eval("ValorParc", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="dtvHeader">Nota Fiscal
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNF" runat="server" Text='<%# Eval("Nf") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">Desconto
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDesconto" runat="server" Text='<%# Eval("Desconto", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">Frete
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="Label6" runat="server" Text='<%# Eval("Frete", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="dtvHeader">Seguro
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblSeguro" runat="server" Text='<%# Eval("Seguro", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">ICMS
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblIcms" runat="server" Text='<%# Eval("Icms", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">IPI
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblIpi" runat="server" Text='<%# Eval("Ipi", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="dtvHeader">Data entr. fábrica
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDataFabrica" runat="server" Text='<%# Eval("DataFabrica", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">SubTotal
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTotal" runat="server" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td class="dtvHeader">OutrasDespesas
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblOutrasDespesas" runat="server" Text='<%# Eval("OutrasDespesas", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Bind("Total") %>' />
                                            <asp:HiddenField ID="hdfIdLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                                            <asp:HiddenField ID="hdfTipoCompra" runat="server" Value='<%# Eval("TipoCompra") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">
                                        <EditItemTemplate>
                                            <asp:Button ID="btnAtualizar" runat="server" CommandName="Update" Text="Atualizar"
                                                OnClientClick="if (!onSave()) return false;" />
                                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click1"
                                                Text="Cancelar" />
                                            <uc2:ctrlLinkQueryString ID="ctrlLinkQueryString1" runat="server" NameQueryString="IdCompra"
                                                Text='<%# Bind("IdCompra") %>' />
                                            <asp:HiddenField ID="hdfIdPedidoEspelho" runat="server" Value='<%# Bind("IdPedidoEspelho") %>' />
                                            <asp:HiddenField ID="hdfIdSinalCompra" runat="server" Value='<%# Bind("IdSinalCompra") %>' />
                                        </EditItemTemplate>
                                        <InsertItemTemplate>
                                            <asp:Button ID="btnInserir" runat="server" CommandName="Insert" OnClientClick="if (!onInsert()) return false;"
                                                Text="Inserir" />
                                            <asp:Button ID="btnCancelar" CausesValidation="false" runat="server" OnClick="btnCancelar_Click"
                                                Text="Cancelar" />
                                        </InsertItemTemplate>
                                        <ItemTemplate>
                                            <asp:Button ID="btnEditar" CausesValidation="false" runat="server" CommandName="Edit" Text="Editar" />
                                            <asp:Button ID="btnFinalizar" CausesValidation="false" runat="server" CommandArgument='<%# Eval("IdCompra") %>'
                                                Text="Finalizar" OnClientClick="return confirm(&quot;Finalizar compra?&quot;);"
                                                OnClick="btnFinalizar_Click" Visible='<%# (bool)Eval("FinalizarVisible") %>' />
                                            <asp:Button ID="btnBaixarEstoque" CausesValidation="false" runat="server" CommandArgument='<%# Eval("IdCompra") %>'
                                                OnClick="btnBaixarEstoque_Click" OnClientClick="return confirm(&quot;Creditar estoque desta compra?&quot;);"
                                                Text="Creditar Estoque" Visible='<%# (bool)Eval("BaixarEstoqueVisible") %>' />
                                            <asp:Button ID="btnVoltar" runat="server" OnClick="btnCancelar_Click" Visible="false" CausesValidation="false"
                                                Text="Voltar" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <div id="lnkProduto" runat="server">
                    <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                        AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXCompra"
                        DataKeyNames="IdProdCompra" OnRowDeleted="grdProdutos_RowDeleted" CssClass="gridStyle"
                        PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                        ShowFooter="True" OnRowCommand="grdProdutos_RowCommand">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" CausesValidation="false">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                    <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" OnClientClick="if (!confirm('Tem certeza que deseja excluir esse produto?')) return false;"
                                        ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" CausesValidation="false" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                        ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onUpdateProd();" />
                                    <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                        ToolTip="Cancelar" />
                                    <asp:HiddenField ID="hdfIdCompra" runat="server" Value='<%# Bind("IdCompra") %>' />
                                    <asp:HiddenField ID="hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                    <asp:HiddenField ID="hdfIdProdPed" runat="server" Value='<%# Bind("IdProdPed") %>' />
                                </EditItemTemplate>
                                <ItemStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                                <EditItemTemplate>
                                    <asp:Label ID="lblCodProdIns" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblCodProdIns" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                <ItemTemplate>
                                    <asp:Label ID="Label112" runat="server" Text='<%# Eval("DescrProduto") + (!String.IsNullOrEmpty(Eval("DescrBeneficiamentos").ToString()) ? " " + Eval("DescrBeneficiamentos") : "") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblDescrProd" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                                    &nbsp;<asp:HiddenField ID="hdfIdProduto" runat="server" Value='<%# Bind("IdProd") %>' />
                                    <asp:HiddenField ID="hdfCodInterno" runat="server" Value='<%# Eval("CodInterno") %>' />
                                    <asp:HiddenField ID="hdfTipoCalc" runat="server" Value='<%# Eval("TipoCalc") %>' />
                                    <asp:HiddenField ID="hdfCustoProd" runat="server" />
                                    <asp:HiddenField ID="hdfDescrItemGenerico" runat="server"
                                        Value='<%# Bind("DescricaoItemGenerico") %>' />
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <uc6:ctrlSelProduto ID="ctrlSelProd" runat="server" Compra="True" Callback="selProduto" />
                                    <asp:Label ID="lblInfoEstoque" runat="server" Text=""></asp:Label>
                                    <asp:HiddenField ID="hdfCodProd" runat="server" />
                                    <asp:HiddenField ID="hdfIsVidro" runat="server" />
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
                                        Text='<%# Bind("Qtde") %>' Width="50px"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtQtdeIns" runat="server" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                        onblur="calcM2Prod();" Width="50px"></asp:TextBox>
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
                                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAlturaIns" runat="server" onblur="calcM2Prod();" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                        Text='<%# Bind("Altura") %>' Enabled='<%# Eval("AlturaEnabled") %>' Width="50px"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtAlturaIns" runat="server" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"
                                        onblur="calcM2Prod();" Width="50px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total M2" SortExpression="TotM">
                                <ItemTemplate>
                                    <asp:Label ID="lblTotM2" runat="server" Text='<%# Bind("TotM") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtTotM2" runat="server" MaxLength="10" onkeypress="return soNumeros(event, false, true)"
                                        Text='<%# Bind("TotM") %>' Width="60px"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtTotM2" runat="server" MaxLength="10" onkeypress="return soNumeros(event, false, true)"
                                        Text='<%# Bind("TotM") %>' Width="60px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor Comprado" SortExpression="Valor">
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Valor", "{0:C}") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtValorIns" runat="server" onblur="calcTotalProd();" onkeypress="return soNumeros(event, false, true);"
                                        Text='<%# Bind("Valor") %>'
                                        Width="50px" OnLoad="txtValorIns_Load"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtValorIns" runat="server" onkeypress="return soNumeros(event, false, true);"
                                        onblur="calcTotalProd();" Width="50px" OnLoad="txtValorIns_Load"></asp:TextBox>
                                </FooterTemplate>
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
                            <asp:TemplateField HeaderText="Cobrar só benef.?" SortExpression="NaoCobrarVidro">
                                <EditItemTemplate>
                                    <asp:CheckBox ID="chkNaoCobrarVidro" runat="server" onclick="calcTotalProd()" Checked='<%# Bind("NaoCobrarVidro") %>' />
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:CheckBox ID="chkNaoCobrarVidro" runat="server" onclick="calcTotalProd()" />
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label10" runat="server" Text='<%# (bool)Eval("NaoCobrarVidro") ? "Sim" : "Não" %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
                                <EditItemTemplate>
                                    <asp:Label ID="lblValorBenef" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblValorBenef" runat="server" Text="R$ 0,00"></asp:Label>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label8" runat="server" Text='<%# Bind("ValorBenef", "{0:c}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Obs." SortExpression="Obs">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtObsIns" runat="server" MaxLength="350" Text='<%# Bind("Obs") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label9" runat="server" Text='<%# Bind("Obs") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtObsIns" runat="server" MaxLength="350"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <EditItemTemplate>
                                    <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(this); return false;"
                                        Visible='<%# Eval("BenefVisible") %>'>
                                    <img border="0" src="../Images/gear_add.gif" />
                                    </asp:LinkButton>
                                    <table id="tbConfigVidro" cellspacing="0" style="display: none;">
                                        <tr align="left">
                                            <td align="center">
                                                <table>
                                                    <tr>
                                                        <td class="dtvFieldBold">Espessura
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtEspessura" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                Width="30px" Text='<%# Bind("Espessura") %>'></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <uc4:ctrlBenef ID="ctrlBenefEditar" runat="server" Beneficiamentos='<%# Bind("Beneficiamentos") %>'
                                                    ExibirValorBeneficiamento="true" IsCompra="true" ValidationGroup="produto" OnLoad="ctrlBenef_Load"
                                                    Redondo='<%# Bind("Redondo") %>' CallbackCalculoValorTotal="setValorTotal" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left"></td>
                                        </tr>
                                    </table>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:LinkButton ID="lnkBenef" runat="server" Style="display: none;" OnClientClick="exibirBenef(this); return false;">
                                    <img border="0" src="../Images/gear_add.gif" />
                                    </asp:LinkButton>
                                    <table id="tbConfigVidro" cellspacing="0" style="display: none;">
                                        <tr align="left">
                                            <td align="center">
                                                <table>
                                                    <tr>
                                                        <td class="dtvFieldBold">Espessura
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtEspessura" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                Width="30px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <uc4:ctrlBenef ID="ctrlBenefInserir" runat="server" ExibirValorBeneficiamento="true"
                                                    IsCompra="true" ValidationGroup="produto" OnLoad="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotal" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left"></td>
                                        </tr>
                                    </table>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <FooterTemplate>
                                    <asp:ImageButton ID="lnkInsProd" runat="server" ImageUrl="../Images/ok.gif" OnClick="lnkInsProd_Click"
                                        OnClientClick="return onSaveProd();" />
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pgr"></PagerStyle>
                        <EditRowStyle CssClass="edit"></EditRowStyle>
                        <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    </asp:GridView>
                </div>
            </td>
        </tr>
        <asp:HiddenField ID="hdfIdCompra" runat="server" />
        <asp:HiddenField ID="hdfIdProduto" runat="server" />
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsCompra" runat="server" DataObjectTypeName="Glass.Data.Model.Compra"
            InsertMethod="Insert" SelectMethod="GetCompra" TypeName="Glass.Data.DAL.CompraDAO"
            UpdateMethod="Update" OnInserted="odsCompra_Inserted" OnUpdated="odsCompra_Updated">
            <SelectParameters>
                <asp:QueryStringParameter Name="idCompra" QueryStringField="idCompra" Type="UInt32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedores"
            TypeName="Glass.Data.DAL.FuncionarioDAO">
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPlanoConta" runat="server" SelectMethod="GetPlanoContasCompra"
            TypeName="Glass.Data.DAL.PlanoContasDAO">
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsFormaPagto" runat="server" SelectMethod="GetForCompra"
            TypeName="Glass.Data.DAL.FormaPagtoDAO">
        </colo:VirtualObjectDataSource>
        <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdXCompra"
            runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosCompra"
            DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
            OnDeleted="odsProdXCompra_Deleted" SelectCountMethod="GetCount" SelectMethod="GetList"
            SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosCompraDAO"
            InsertMethod="Insert" UpdateMethod="Update"
            OnUpdated="odsProdXCompra_Updated">
            <SelectParameters>
                <asp:QueryStringParameter Name="idCompra" QueryStringField="idCompra" Type="UInt32" />
            </SelectParameters>
        </colo:VirtualObjectDataSource>
    </table>

    <script type="text/javascript">
        loading = true;
        
        getFornec(FindControl("txtNumFornec", "input"));
        tipoCompraChange(FindControl("drpTipoCompra", "select"));
        formaPagtoChange(FindControl("drpFormaPagto", "select"));

        var cCodProd = FindControl("ctrlSelProd_ctrlSelProdBuscar_txtDescr", "input");
        if (cCodProd != null)
            cCodProd.focus();

        calcAcrescimo();
        
        // Se a empressa não vende vidros, esconde campos
        if (<%= Glass.Configuracoes.Geral.NaoVendeVidro().ToString().ToLower() %>)
        {
            var tbProd = FindControl("grdProdutos", "table");
            
        if (tbProd != null)
        {
            var rows = tbProd.children[0].children;
                
            var colsTitle = rows[0].getElementsByTagName("th");
            colsTitle[4].style.display = "none";
            colsTitle[5].style.display = "none";
            colsTitle[6].style.display = "none";
            colsTitle[9].style.display = "none";
            colsTitle[10].style.display = "none";
                
            var k=0;
            for (k=1; k<rows.length; k++) {
                if (rows[k].cells[4] == null)
                    break;                        
                       
                rows[k].cells[4].style.display = "none";
                rows[k].cells[5].style.display = "none";
                rows[k].cells[6].style.display = "none";
                rows[k].cells[9].style.display = "none";
                rows[k].cells[10].style.display = "none";
            }
        }
        }
        
        loading = false;
    </script>

</asp:Content>
