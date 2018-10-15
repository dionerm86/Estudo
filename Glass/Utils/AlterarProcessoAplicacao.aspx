<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AlterarProcessoAplicacao.aspx.cs"
    Inherits="Glass.UI.Web.Utils.AlterarProcessoAplicacao" MasterPageFile="~/Painel.master" Title="Alterar Processo/Aplicação" %>

<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlProdComposicaoAlterarProcApl.ascx" TagName="ctrlProdComposicao" TagPrefix="uc13" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

    // Guarda a quantidade disponível em estoque do produto buscado
    var qtdEstoque = 0;
    var exibirMensagemEstoque = false;
    var qtdEstoqueMensagem = 0;
        
    
    var produtoAmbiente = false;
    var aplAmbiente = false;
    var procAmbiente = false;
    var loading = true;    
    
    function mensagemProdutoComDesconto(editar)
    {
        alert("Não é possível " + (editar ? "editar" : "remover") + " esse produto porque o pedido possui desconto.\n" +
            "Aplique o desconto apenas ao terminar o cadastro dos produtos.\n" +
            "Para continuar, remova o desconto do pedido.");
    }   
    
    
    function atualizaValMin()
    {
        if (parseFloat(FindControl("hdfTamanhoMaximoObra", "input").value.replace(",", ".")) == 0)
        {
            var codInterno = FindControl("txtCodProdIns", "input");
            codInterno = codInterno != null ? codInterno.value : FindControl("lblCodProdIns", "span").innerHTML;
            
            var tipoPedido = FindControl("hdfTipoPedido", "input").value;
            var tipoEntrega = FindControl("hdfTipoEntrega", "input").value;       
            var cliRevenda = FindControl("hdfCliRevenda", "input").value;
            var idCliente = FindControl("hdfIdCliente", "input").value;
            var tipoVenda = FindControl("hdfTipoVenda", "input").value;
            
            var idProdPed = FindControl("hdfProdPed", "input");
            idProdPed = idProdPed != null ? idProdPed.value : "";
            
            var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
            
            var percDescontoQtde = controleDescQtde.PercDesconto();
            
            FindControl("hdfValMin", "input").value = AlterarProcessoAplicacao.GetValorMinimo(codInterno, tipoPedido, tipoEntrega, tipoVenda, 
                idCliente, cliRevenda, idProdPed, percDescontoQtde).value;
        }
        else
            FindControl("hdfValMin", "input").value = FindControl("txtValorIns", "input").value;
    }
    
    function obrigarProcApl()
    {
        var isVidro = FindControl("hdfIsVidro", "input").value == "true";

        var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
        var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
        var isVidroRoteiro = isVidro && <%= UtilizarRoteiroProducao().ToString().ToLower() %>;
        
        if (dadosProduto.IsChapaVidro)
            return true;

        if (isVidroRoteiro || (isObrigarProcApl && isVidroBenef))
        {
            if (FindControl("txtAplIns", "input") != null && FindControl("txtAplIns", "input").value == "")
            {
                if (isVidroRoteiro && !isObrigarProcApl) {
                    alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                    return false;
                }

                alert("Informe a aplicação.");
                return false;
            }
            
            if (FindControl("txtProcIns", "input") != null && FindControl("txtProcIns", "input").value == "")
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
    
    function calculaTamanhoMaximo()
    {
        if (FindControl("lblCodProdIns", "span") == null)
            return;
            
        var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;
        var codInterno = FindControl("lblCodProdIns", "span").innerHTML;
        var totM2 = FindControl("lblTotM2Ins", "span").innerHTML;
        var idProdPed = FindControl("hdfProdPed", "input") != null ? FindControl("hdfProdPed", "input").value : 0;
        
        var tamanhoMaximo = AlterarProcessoAplicacao.GetTamanhoMaximoProduto(idPedido, codInterno, totM2, idProdPed).value.split(";");
        tamanhoMaximo = tamanhoMaximo[0] == "Ok" ? parseFloat(tamanhoMaximo[1].replace(",", ".")) : 0;
        
        FindControl("hdfTamanhoMaximoObra", "input").value = tamanhoMaximo;
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
    
    var comissaoAlteraValor = null;
    
    // Retorna o percentual de comissão
    function getPercComissao()
    {
        var percComissao = 0;
        var txtComissao = FindControl("txtPercentual", "input");
        var hdfPercComissao = FindControl("hdfPercComissao", "input");
        var hdfIdPedido = FindControl("hdfIdPedido", "input");
    
        if (comissaoAlteraValor == null)
            comissaoAlteraValor = MetodosAjax.ComissaoAlteraValor(hdfIdPedido.value).value;
    
        if (hdfIdPedido != null && comissaoAlteraValor == "false")
            return 0;
    
        if (txtComissao != null && txtComissao.value != "")
            percComissao = parseFloat(txtComissao.value.replace(',', '.'));
        else if (hdfPercComissao != null && hdfPercComissao.value != "")
            percComissao = parseFloat(hdfPercComissao.value.replace(',', '.'));
            
        return percComissao != null ? percComissao : 0;
    }    

    // Função chamada pelo popup de escolha da Aplicação do produto
    function setApl(idAplicacao, codInterno) {

            var verificaEtiquetaApl = MetodosAjax.VerificaEtiquetaAplicacao(idAplicacao, FindControl("hdfIdPedido", "input").value);
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

            if (!aplAmbiente && FindControl("txtAplIns", "input") != null)
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
        var codInternoProd = "";
        var codAplicacaoAtual = "";
        
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProd", "input").value);
        var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

        if(idSubgrupo.value != "" && retornoValidacao.value == "false" && (FindControl("txtProcIns", "input") != null && FindControl("txtProcIns", "input").value != ""))
        {
            FindControl("txtProcIns", "input").value = "";
            alert("Este processo não pode ser selecionado para este produto.")
            return false;
        }

        var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, FindControl("hdfIdPedido", "input").value);
        if(verificaEtiquetaProc.error != null){

            setApl("", "");

            alert(verificaEtiquetaProc.error.description);
            return false;
        }

        if (!procAmbiente && FindControl("txtProcIns", "input") != null)
        {
            FindControl("txtProcIns", "input").value = codInterno;
            FindControl("hdfIdProcesso", "input").value = idProcesso;            
                
            codAplicacaoAtual = FindControl("txtAplIns", "input").value;
        }
               
        if (((codAplicacao && codAplicacao != "") ||
            (codInternoProd != "" && AlterarProcessoAplicacao.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) &&
            (codAplicacaoAtual == null || codAplicacaoAtual == ""))
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

    var saveProdClicked = false;    

    // Função chamada quando o produto está para ser atualizado
    function onUpdateProd(idProdPed) {     
        
        if (!obrigarProcApl())
            return false;
        
        return true;
    }

    function getCli(idCliente)
    {
        if (idCliente == undefined || idCliente == null || idCliente == "")
            return false;

        var usarComissionado = <%= Glass.Configuracoes.PedidoConfig.Comissao.UsarComissionadoCliente.ToString().ToLower() %>;
                
        var retorno = AlterarProcessoAplicacao.GetCli(idCliente).value.split(';');
        if (retorno[0] == "Erro") {
            alert(retorno[1]);
            
            return false;
        }

        FindControl("lblObsCliente", "span").innerHTML = retorno[3];
    } 

    function buscarProcessos(){
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProd", "input").value);
        openWindow(450, 700, "../Utils/SelEtiquetaProcesso.aspx?idSubgrupo=" + idSubgrupo.value);
    }

    function exibirProdsComposicao(botao, idProdPed) {

        var grdProds = FindControl("grdProdutos", "table");

        if(grdProds == null)
            return;

        for (var i = 0; i < grdProds.rows.length; i++) {

            var row = grdProds.rows[i];
            if(row.id.indexOf("prodPed_") != -1 && row.id.split('_')[1] != idProdPed){
                row.style.display = "none";
            }
        }

        var linha = document.getElementById("prodPed_" + idProdPed);
        var exibir = linha.style.display == "none";
        linha.style.display = exibir ? "" : "none";
        botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
        botao.title = (exibir ? "Esconder" : "Exibir") + " Produtos da Composição";

        if(FindControl("txtCodProdIns","input") != null)
            FindControl("txtCodProdIns","input").parentElement.parentElement.style.display = !exibir ? "" : "none";

        FindControl("hdfProdPedComposicaoSelecionado", "input").value = exibir? idProdPed : 0;
    }

    </script>

    <table id="mainTable" runat="server" clientidmode="Static" style="width: 100%">
        <tr>
            <td>
                <table style="width: 100%">
                    <tr>
                        <td align="center">
                            <asp:DetailsView ID="dtvPedido" runat="server" AutoGenerateRows="False" DataSourceID="odsPedido"
                                DefaultMode="Insert" GridLines="None" Height="50px" Width="125px">
                                <Fields>
                                    <asp:TemplateField ShowHeader="False">
                                        
                                        <ItemTemplate>
                                            <table cellpadding="2" cellspacing="2">
                                                <asp:HiddenField ID="hdfIdPedido" runat="server" Value='<%# Eval("IdPedido") %>' />
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Num. Pedido
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblNumPedido" runat="server" Text='<%# Eval("IdPedido") %>' Font-Size="Medium"></asp:Label>
                                                        <asp:Label ID="lblDescrTipoPedido" runat="server" Text='<%# "(" + Eval("DescricaoTipoPedido") + ")" %>'
                                                            ForeColor="Green"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:Label ID="lblNomeCliente" runat="server" Text='<%# Eval("NomeCliente") %>'></asp:Label>
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
                                                        Tel. Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTelCliente" runat="server" Text='<%# Eval("RptTelContCli") %>'></asp:Label>
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
                                                        Endereço Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="5">
                                                        <asp:Label ID="lblEndereco" runat="server" Text='<%# Eval("EnderecoCompletoCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">Endereço Obra</td>
                                                    <td align="left" colspan="5" nowrap="nowrap">
                                                        <asp:Label ID="lblLocalObra" runat="server" Text='<%# Eval("LocalizacaoObra") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Valor Entrada
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblValorEnt" runat="server" Text='<%# Eval("ValorEntrada", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Venda
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoVenda" runat="server" Text='<%# Eval("DescrTipoVenda") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Tipo Entrega
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblTipoEntrega" runat="server" Text='<%# Eval("DescrTipoEntrega") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Situação
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("DescrSituacaoPedido") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data Ped.
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblData" runat="server" Text='<%# Eval("DataPedidoString", "{0:d}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Data Entrega
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDataEntrega" runat="server" Text='<%# Eval("DataEntregaString") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label runat="server" ID="lblValorFrete" OnLoad="txtValorFrete_Load" Text="Valor do Frete"></asp:Label> 
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="Label18" runat="server" Text='<%# Eval("ValorEntrega", "{0:C}") %>' OnLoad="txtValorFrete_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Desconto
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblDesconto" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold" runat="server" id="tituloComissao"
                                                        visible='<%# Eval("ComissaoVisible") %>'>
                                                        <asp:Label ID="Label13" runat="server" Text="Comissão"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="comissao" visible='<%# Eval("ComissaoVisible") %>'>
                                                        <asp:Label ID="lblComissao" runat="server" Text='<%# Eval("ValorComissao", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" runat="server" id="tituloIcms" onload="Icms_Load">
                                                        <asp:Label ID="lblTituloIcms" runat="server" Font-Bold="True" Text="Valor ICMS"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="valorIcms" onload="Icms_Load">
                                                        <asp:Label ID="lblValorIcms" runat="server" Text='<%# Eval("ValorIcms", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="tituloIpi" onload="Ipi_Load">
                                                        <asp:Label ID="lblTituloIpi" runat="server" Font-Bold="True" Text="Valor IPI"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" runat="server" id="valorIpi" onload="Ipi_Load">
                                                        <asp:Label ID="lblValorIpi" runat="server" Text='<%# Eval("ValorIpi", "{0:C}") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="6" nowrap="nowrap">
                                                        <table>
                                                            <tr>
                                                                <td class="cabecalho">
                                                                    <asp:Label ID="lblTitleTotal" runat="server" Font-Bold="True" OnLoad="lblTotalGeral_Load"
                                                                        Text="Total"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotal" runat="server" ForeColor="#0000CC" OnLoad="lblTotalGeral_Load"
                                                                        Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                                <td class="cabecalho" nowrap="nowrap">
                                                                    <asp:Label ID="lblTitleTotalBruto" runat="server" Font-Bold="True" OnLoad="lblTotalBrutoLiquido_Load"
                                                                        Text="Total Bruto"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotalBruto" runat="server" OnLoad="lblTotalBrutoLiquido_Load" Text='<%# Eval("TotalBruto", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                                <td class="cabecalho" nowrap="nowrap">
                                                                    <asp:Label ID="lblTitleTotalLiquido" runat="server" Font-Bold="True" OnLoad="lblTotalBrutoLiquido_Load"
                                                                        Text="Total Líquido"></asp:Label>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblTotalLiquido" runat="server" ForeColor="#0000CC" OnLoad="lblTotalBrutoLiquido_Load"
                                                                        Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Forma Pagto.
                                                    </td>
                                                    <td align="left" colspan="3">
                                                        <asp:Label ID="lblFormaPagto" runat="server" Text='<%# Eval("PagtoParcela") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="lblTituloFastDelivery" runat="server" Text="Fast delivery" OnLoad="FastDelivery_Load"></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap">
                                                        <asp:Label ID="lblFastDelivery" runat="server" Text='<%# Eval("FastDelivery") %>'
                                                            OnLoad="FastDelivery_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        
                                                    </td>
                                                    <td colspan="3" align="left">
                                                        
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label15" runat="server" Text="Têmpera fora" OnLoad="TemperaFora_Load"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label ID="lblTemperaFora" runat="server" Text='<%# Eval("TemperaFora") %>' OnLoad="TemperaFora_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="Label16" runat="server" Text="Funcionário comp."></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="3">
                                                        <asp:Label ID="lblFuncVenda" runat="server" Text='<%# Eval("NomeFuncVenda") %>'></asp:Label>
                                                    </td>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        <asp:Label ID="lblDeveTransferirTexto" runat="server" Text="Deve Transferir?" OnLoad="Loja_Load"></asp:Label>
                                                    </td>
                                                    <td align="left">
                                                        <asp:Label ID="lblDeveTransferirValor" runat="server" Text='<%# Eval("DeveTransferirStr") %>' OnLoad="Loja_Load"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Observação
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="5">
                                                        <asp:Label ID="lblObs" runat="server" Text='<%# Eval("Obs") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" nowrap="nowrap" style="font-weight: bold">
                                                        Obs. do Cliente
                                                    </td>
                                                    <td align="left" nowrap="nowrap" colspan="5">
                                                        <asp:Label ID="lblObsCliente" runat="server" OnLoad="lblObsCliente_Load" Text='<%# Eval("ObsCliente") %>'></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:HiddenField ID="hdfTipoEntrega" runat="server" Value='<%# Eval("TipoEntrega") %>' />
                                            <asp:HiddenField ID="hdfCliRevenda" runat="server" Value='<%# Eval("CliRevenda") %>' />
                                            <asp:HiddenField ID="hdfTipoVenda" runat="server" Value='<%# Eval("TipoVenda") %>' />
                                            <asp:HiddenField ID="hdfTotal" runat="server" Value='<%# Eval("Total") %>' />
                                            <asp:HiddenField ID="hdfPercComissao" runat="server" Value='<%# Eval("PercComissao") %>' />
                                            <asp:HiddenField ID="hdfIdCliente" runat="server" Value='<%# Eval("IdCli") %>' />
                                            <asp:HiddenField ID="hdfFastDelivery" runat="server" OnPreRender="FastDelivery_Load"
                                                Value='<%# Eval("FastDelivery") %>' />
                                            <asp:HiddenField ID="hdfTemperaFora" runat="server" OnLoad="TemperaFora_Load" Value='<%# Eval("TemperaFora") %>' />
                                            <asp:HiddenField ID="hdfTotalSemDesconto" runat="server" Value='<%# Eval("TotalSemDesconto") %>' />
                                            <asp:HiddenField ID="hdfTipoPedido" runat="server" Value='<%# Eval("TipoPedido") %>' />
                                            <asp:HiddenField ID="hdfIsReposicao" runat="server" Value='<%# IsReposicao(Eval("TipoVenda")) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="False">                                        
                                        <ItemTemplate>                                            
                                            <asp:HiddenField ID="hdfLoja" runat="server" Value='<%# Eval("IdLoja") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Fields>
                            </asp:DetailsView>
                            <asp:HiddenField ID="hdfAlterarProjeto" runat="server" Value="false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td align="center">                
                <div id="divProduto" runat="server">
                    <table>                        
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdAmbiente" runat="server" AllowPaging="True"
                                    AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdAmbientePedido"
                                    DataSourceID="odsAmbiente" OnRowCommand="grdAmbiente_RowCommand" ShowFooter="false"
                                    OnPreRender="grdAmbiente_PreRender" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" 
                                    OnRowUpdated="grdAmbiente_RowUpdated">
                                    <Columns>                                        
                                        <asp:TemplateField HeaderText="Ambiente" SortExpression="Ambiente">                                                                            
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkViewProd" runat="server" CausesValidation="False" CommandArgument='<%# Eval("IdAmbientePedido") %>'
                                                    CommandName="ViewProd" Text='<%# Eval("Ambiente") %>'></asp:LinkButton>                                                
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">                                            
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                                <asp:Label ID="Label17" runat="server" ForeColor="Red" Text='<%# Eval("DescrObsProj") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Qtde") %>'></asp:Label>
                                            </ItemTemplate>                                            
                                        </asp:TemplateField>                                        
                                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("Largura") %>'></asp:Label>
                                            </ItemTemplate>                                            
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura" Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("Altura") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                        
                                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso" Visible="False">                                            
                                            <ItemTemplate>
                                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodProcesso") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao" Visible="False">                                            
                                            <ItemTemplate>
                                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Redondo" SortExpression="Redondo" Visible="False">                                            
                                            <ItemTemplate>
                                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Eval("Redondo") %>' Enabled="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor produtos" SortExpression="TotalProdutos">                                            
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("TotalProdutos", "{0:c}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Acréscimo" SortExpression="Acrescimo">                                            
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("TextoAcrescimo") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Desconto" SortExpression="Desconto">                                            
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("TextoDesconto") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle CssClass="edit"></EditRowStyle>
                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                </asp:GridView>
                                <asp:Label ID="lblAmbiente" runat="server" CssClass="subtitle1" Font-Bold="False"></asp:Label>
                                <asp:HiddenField ID="hdfAlturaAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfLarguraAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfQtdeAmbiente" runat="server" />
                                <asp:HiddenField ID="hdfRedondoAmbiente" runat="server" />
                                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAmbiente" runat="server" DataObjectTypeName="Glass.Data.Model.AmbientePedido"
                                    DeleteMethod="DeleteComTransacao" EnablePaging="True" MaximumRowsParameterName="pageSize"
                                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.AmbientePedidoDAO"
                                    UpdateMethod="Update" >
                                    <SelectParameters>
                                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                                <asp:HiddenField ID="hdfIdAmbiente" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%# Eval("Ambiente") %>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                                    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXPed" CssClass="gridStyle"
                                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                    DataKeyNames="IdProdPed" ShowFooter="false"
                                    OnRowCommand="grdProdutos_RowCommand" OnPreRender="grdProdutos_PreRender" PageSize="12"
                                    OnRowUpdated="grdProdutos_RowUpdated" >
                                    <FooterStyle Wrap="True" />
                                    <Columns>
                                        <asp:TemplateField>                                            
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Visible='<%# Eval("AlterarProcessoAplicacaoVisible") %>'
                                                     OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>'>
                                                    <img border="0" src="../Images/Edit.gif" ></img></asp:LinkButton>                                                
                                            </ItemTemplate>       
                                            <EditItemTemplate>
                                                <asp:HiddenField ID="hdfIsVidro" runat="server" Value='<%# Eval("IsVidro") %>' />
                                                <asp:HiddenField runat="server" id="hdfIdProdPed" Value='<%# Bind("IdProdPed") %>' />
                                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick='<%# "if(!onUpdateProd(" + Eval("IdProdPed") + ")) return false;"%>' />
                                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                                    ToolTip="Cancelar" />                                                
                                            </EditItemTemplate>                                     
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                                            <ItemTemplate>
                                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>                                       
                                        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") + (!String.IsNullOrEmpty(Eval("DescrBeneficiamentos").ToString()) ? " " + Eval("DescrBeneficiamentos") : "") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                                            <ItemTemplate>
                                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Qtde") %>'></asp:Label>
                                                <asp:Label ID="lblQtdeAmbiente" runat="server" OnPreRender="lblQtdeAmbiente_PreRender"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                                            <ItemTemplate>
                                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("Largura") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                                            <ItemTemplate>
                                                <asp:Label ID="Label4" runat="server" Text='<%# Eval("AlturaLista") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
                                            <ItemTemplate>
                                                <asp:Label ID="Label6" runat="server" Text='<%# Eval("TotM") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotM2Calc">                                            
                                            <ItemTemplate>
                                                <asp:Label ID="Label12" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="True" />
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
                                            <ItemTemplate>
                                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("ValorVendido", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                                            <EditItemTemplate>
                                                <table class="pos">
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="procAmbiente=false; loadProc(this.value);"
                                                                onkeydown="if (isEnter(event)) { procAmbiente=false; loadProc(this.value); }"
                                                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <a id="lnkProcesso" href="#" onclick='procAmbiente=false; buscarProcessos(); return false;'>
                                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                            </EditItemTemplate>                                            
                                            <ItemTemplate>
                                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("CodProcesso") %>'></asp:Label>
                                            </ItemTemplate>
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
                                            <ItemTemplate>
                                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodAplicacao") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Ped. Cli." SortExpression="PedCli">
                                            <ItemTemplate>
                                                <asp:Label ID="Label13" runat="server" Text='<%# Eval("PedCli") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>                           
                                        <asp:TemplateField HeaderText="Total" SortExpression="Total">
                                            <ItemTemplate>
                                                <asp:Label ID="Label7" runat="server" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                                                <asp:Label ID="Label43" runat="server" Text='<%# "(Desconto de " + Eval("PercDescontoQtde") + "%)" %>'
                                                    Visible='<%# (float)Eval("PercDescontoQtde") > 0 %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">                                            
                                            <ItemTemplate>
                                                <asp:Label ID="Label11" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <EditItemTemplate>
                                                <div id="benefMaoObra" style='<%# !IsPedidoMaoDeObra() ? "display: none;": "" %> white-space: nowrap'>
                                                    <asp:DropDownList ID="drpAltBenef" runat="server" onchange="calcTotalProd()" SelectedValue='<%# Eval("AlturaBenef") %>'>
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:DropDownList ID="drpLargBenef" runat="server" onchange="calcTotalProd()" SelectedValue='<%# Eval("LarguraBenef") %>'>
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem>0</asp:ListItem>
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList>
                                                    Esp.:
                                                    <asp:TextBox ID="txtEspBenef" Width="30px" runat="server" onkeypress="return soNumeros(event, true, true)"
                                                        Text='<%# Eval("EspessuraBenef") %>'></asp:TextBox>
                                                </div>                                                
                                                <table id='<%# "tbConfigVidro_" + Eval("IdProdPed") %>' cellspacing="0" style="display: none;">
                                                    <tr align="left">
                                                        <td align="center">
                                                            <table>
                                                                <tr>
                                                                    <td class="dtvFieldBold">
                                                                        Espessura
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtEspessura" runat="server" OnDataBinding="txtEspessura_DataBinding"
                                                                            onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Eval("Espessura") %>'></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc4:ctrlBenef ID="ctrlBenefEditar" runat="server" Beneficiamentos='<%# Eval("Beneficiamentos") %>'
                                                                ValidationGroup="produto" OnInit="ctrlBenef_Load" Redondo='<%# Eval("Redondo") %>'
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
                                                    <asp:TextBox ID="txtEspBenef" Width="30px" runat="server" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                                </div>
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
                                                                        <asp:TextBox ID="txtEspessura" runat="server" onkeypress="return soNumeros(event, false, true);"
                                                                            Width="30px"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <uc4:ctrlBenef ID="ctrlBenefInserir" runat="server" OnInit="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotal"
                                                                ValidationGroup="produto" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="left">
                                                        </td>
                                                    </tr>
                                                </table>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <div id='<%# "imgProdsComposto_" + Eval("IdProdPed") %>'>
                                                    <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/box.png" ToolTip="Exibir Produtos da Composição"
                                                        Visible='<%# Eval("IsProdLamComposicao") %>' OnClientClick='<%# "exibirProdsComposicao(this, " + Eval("IdProdPed") + "); return false"%>' />
                                                    <%--<asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/imagem.gif"
                                                        OnClientClick='<%# "openWindow(600, 800, \"../Utils/SelImagemPeca.aspx?tipo=pedido&idPedido=" + Eval("IdPedido") +"&idProdPed=" +  Eval("IdProdPed") +"&pecaAvulsa=" +  ((bool)Eval("IsProdLamComposicao") == false) + "\"); return false" %>'
                                                        ToolTip="Exibir imagem das peças"  Visible='<%# (Eval("IsVidro").ToString() == "true")%>'/>--%>
                                                </div>
                                            </ItemTemplate>
                                            <EditItemTemplate></EditItemTemplate>
                                            <FooterTemplate></FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                </td> </tr>

                                                <tr id="prodPed_<%# Eval("IdProdPed") %>" style="display: none" align="center">
                                                    <td colspan="17">
                                                        <br />
                                                        <uc13:ctrlProdComposicao runat="server" ID="ctrlProdComp" Visible='<%# Eval("IsProdLamComposicao") %>' 
                                                            IdProdPed='<%# Glass.Conversoes.StrParaUint(Eval("IdProdPed").ToString()) %>'/>
                                                        <br />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                </td> </tr>
                                                <tr style='<%= !IsPedidoMaoDeObra() ? "display: none": "" %>'>
                                                    <td colspan="17" style="text-align: right">
                                                        <span style="position: relative; top: -6px">campos usados para definir altura, largura
                                                            e
                                                            <br />
                                                            espessura da lapidação e bisotê </span>
                                                    </td>
                                                </tr>
                                                <tr style='<%= !IsPedidoProducao() ? "display: none": "" %>'>
                                                    <td colspan="4">
                                                    </td>
                                                    <td colspan="13" style="text-align: left">
                                                        <span style="position: relative; top: -6px">altura e largura definidas no produto
                                                            <br />
                                                            e recuperadas automaticamente </span>
                                                    </td>
                                                </tr>
                                            </EditItemTemplate>
                                            
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pgr"></PagerStyle>
                                    <EditRowStyle CssClass="edit"></EditRowStyle>
                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdXPed" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedido"
        DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCount" SelectMethod="GetList"
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
        InsertMethod="Insert" UpdateMethod="UpdateProcessoAplicacao"  
        OnUpdated="odsProdXPed_Updated">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
            <asp:ControlParameter ControlID="hdfIdAmbiente" Name="idAmbientePedido" PropertyName="Value" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfIdPedido" runat="server" />
    <asp:HiddenField ID="hdfIdProd" runat="server" />
    <asp:HiddenField ID="hdfNaoVendeVidro" runat="server" />
    <asp:HiddenField ID="hdfProdPedComposicaoSelecionado" runat="server" Value="0" />

    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsPedido" runat="server" DataObjectTypeName="Glass.Data.Model.Pedido"
        SelectMethod="GetElement" TypeName="Glass.Data.DAL.PedidoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>

    <script type="text/javascript">    
    
                  

    // Se a empressa não vende vidros, esconde campos
    if (FindControl("hdfNaoVendeVidro", "input").value == "true" && FindControl("grdProdutos", "table") != null)
    {
        var tbProd = FindControl("grdProdutos", "table");
        var rows = tbProd.rows;
        
        var colsTitle = rows[0].getElementsByTagName("th");
        colsTitle[4].style.display = "none";
        colsTitle[5].style.display = "none";
        colsTitle[6].style.display = "none";
        colsTitle[7].style.display = "none";
        
        var k=0;
        for (k=1; k<rows.length; k++) {
            if (rows[k].cells.length <= 2)
                continue;
                
            if (rows[k].cells[4] == null)
                break;
                
            rows[k].cells[4].style.display = "none";
            rows[k].cells[5].style.display = "none";
            rows[k].cells[6].style.display = "none";
            rows[k].cells[7].style.display = "none";
        }
    }
    else {
        // loadConfig();
        posValor = <%= GetPosValor() %>;
        
        var usarAltLarg = '<%= Glass.Configuracoes.PedidoConfig.EmpresaTrabalhaAlturaLargura %>'.toLowerCase();

        // Troca a posição da altura com a largura
        if (usarAltLarg == "true" && FindControl("grdProdutos", "table") != null) {
            var tbProd = FindControl("grdProdutos", "table");
            var rows = tbProd.children[0].children;
            
            // Troca a label de título altura-largura
            var colsTitle = rows[0].getElementsByTagName("th");
            var colAltInnerHtml = colsTitle[4].innerHTML;
            colsTitle[4].innerHTML = colsTitle[5].innerHTML;
            colsTitle[5].innerHTML = colAltInnerHtml;
            
            var j=0;
            for (j=1; j<rows.length; j++) {
                try
                {
                    var cols = rows[j].getElementsByTagName("td");
                    var colTemp = rows[j].cells[4].innerHTML;
                    rows[j].cells[4].innerHTML = rows[j].cells[5].innerHTML;
                    rows[j].cells[5].innerHTML = colTemp;
                }
                catch (err)
                { }
            }
        }
    }
    
    var numCli = FindControl("hdfIdCliente", "input");
    if (numCli != null && numCli.value != "")
    {        
        getCli(numCli.value);
    }
    
    var idPedido = <%= !string.IsNullOrEmpty(Request["idPedido"]) ? Request["idPedido"] : "0" %>;
    
        $(document).ready(function(){

            var hdfProdPedComposicaoSelecionado = FindControl("hdfProdPedComposicaoSelecionado", "input");

            if(hdfProdPedComposicaoSelecionado.value > 0){
                var div = FindControl("imgProdsComposto_" + hdfProdPedComposicaoSelecionado.value, "div");

                if(div == null) return;

                var botao = FindControl("imgProdsComposto", "input", div);
                exibirProdsComposicao(botao, hdfProdPedComposicaoSelecionado.value);
            }
        });
    
        loading = false;
    
    </script>

</asp:Content>