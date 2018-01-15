<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstOrcamentoRapido.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstOrcamentoRapido" Title="Orçamento Rápido" %>

<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc3" %>
<%@ Register src="../Controls/ctrlSelProcesso.ascx" tagname="ctrlSelProcesso" tagprefix="uc4" %>
<%@ Register src="../Controls/ctrlSelAplicacao.ascx" tagname="ctrlSelAplicacao" tagprefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Listas/LstOrcamentoRapido.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script> 

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcAluminio.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CalcProd.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
    var isPopup = <%= IsPopup().ToString().ToLower() %>;
    var callbackIncluir = '<%= Request["callbackIncluir"] %>';
    var callbackExcluir = '<%= Request["callbackExcluir"] %>';
    var liberarOrcamento = '<%= Request["LiberarOrcamento"] %>';
    var adicVidroRedondoAte12mm = '<%= Glass.Configuracoes.Geral.AdicionalVidroRedondoAte12mm %>';
    var adicVidroRedondoAcima12mm = '<%= Glass.Configuracoes.Geral.AdicionalVidroRedondoAcima12mm %>';
    var usarAltLarg = '<%= Glass.Configuracoes.PedidoConfig.EmpresaTrabalhaAlturaLargura %>'.toLowerCase() == "true";
    var percDescontoAtual = 0;
    var numCasasDecTotM = <%= Glass.Configuracoes.Geral.NumeroCasasDecimaisTotM %>;
    var verificarEstoqueAoInserirProduto = <%= VerificarEstoqueAoInserirProduto().ToString().ToLower() %>;
    
    function hideThisPopup()
    {
        var itens = new Array("ctl00_ctl00_titulo", "separaTotal", "geracao", "entrega", "textoTaxaJuros");
        for (i = 0; i < itens.length; i++)
            document.getElementById(itens[i]).style.display = "none";
        
        document.getElementById("tabelaPrincipal").style.height = "";
    }
    
    function obrigarProcApl()
    {
        var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
        var isVidroBenef = exibirControleBenef(nomeControleBenef) && dadosProduto.Grupo == 1;
        var isVidroRoteiro = dadosProduto.Grupo == 1 && <%= UtilizarRoteiroProducao().ToString().ToLower() %>;
        
        if (isVidroRoteiro || (isObrigarProcApl && isVidroBenef))
        {
            if (FindControl("selProc_selProcesso_txtDescr", "input") != null && FindControl("selProc_selProcesso_txtDescr", "input").value == "")
            {
                alert("Informe o processo.");
                return false;
            }
            
            if (FindControl("selApl_selAplicacao_txtDescr", "input") != null && FindControl("selApl_selAplicacao_txtDescr", "input").value == "")
            {
                alert("Informe a aplicação.");
                return false;
            }
        }
        
        return true;
    }
    
    function atualizaTotalDesc()
    {
        try
        {
            var valor = parseFloat(FindControl("lblTotal", "span").innerHTML.replace("R$", "").replace(" ", "").replace(",", "."));
            
            var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
            controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
            
            var percDesconto = controleDescQtde.PercDesconto();
            if (percDescontoAtual != percDesconto)
            {
                valor = valor * (1 / (1 - (percDescontoAtual / 100)));
                valor = parseFloat(valor.toFixed(2));
            }
            
            callbackSetTotal(valor, 0);
        }
        catch (err) { }
    }
    
    function callbackSetTotal(valor, custo)
    {
        var valorItem = FindControl("txtValor", "input").value;
        valorItem = valorItem != "" ? parseFloat(valorItem.replace(',', '.')) : 0;
        
        var totM2 = FindControl("hdfTotM2", "input").value;
        totM2 = totM2 != "" ? parseFloat(totM2.replace(',', '.')) : 0;
        
        var percComissao = FindControl("hdfPercComissao", "input").value;
        percComissao = percComissao != "" ? parseFloat(percComissao.replace(',', '.')) : 0;
        
        var valorProd = 0;
        
        if (dadosProduto.TipoCalculo == 2 || dadosProduto.TipoCalculo == 10)
            valorProd = (valorItem * totM2 / ((100 - percComissao) / 100)) + valor;
        else
            valorProd = valor;
            
        var controleDescQtde = FindControl("_divDescontoQtde", "div").id;
        controleDescQtde = eval(controleDescQtde.substr(0, controleDescQtde.lastIndexOf("_")));
        
        var percDesconto = controleDescQtde.PercDesconto();
        if (percDesconto > 0)
            valorProd = valorProd * (1 - (percDesconto / 100));
        
        percDescontoAtual = percDesconto;
        
        FindControl("lblTotal", "span").innerHTML = "R$ " + valorProd.toFixed(2).replace('.', ',');
        
        var aliqIcms = parseFloat(FindControl("hdfAliqIcms", "input").value);
        var valorIcms = parseFloat(FindControl("lblTotal", "span").innerHTML.replace("R$", "").replace(" ", "").replace(",", "."));
        valorIcms = valorIcms * (aliqIcms / 100);
        if (isNaN(valorIcms))
            valorIcms = 0;
        
        FindControl("lblValorIcms", "span").innerHTML = "R$ " + valorIcms.toFixed(2).replace(".", ",");
        FindControl("lblTotalProd", "span").innerHTML = "R$ " + (parseFloat(valorProd.toFixed(2)) + parseFloat(valorIcms.toFixed(2))).toFixed(2).replace(".", ",");
    }
    
    function validaValorMinimo()
    {
        var valor = FindControl("txtValor", "input").value != "" ? parseFloat(FindControl("txtValor", "input").value.replace(',', '.')) : 0;
        var valorMinimo = FindControl("hdfValMin", "input").value != "" ? parseFloat(FindControl("hdfValMin", "input").value.replace(',', '.')) : 0;
        
        if (valor < valorMinimo && liberarOrcamento != "True")
        {
            alert("O valor digitado é menor que o valor mínimo do produto (R$ " + valorMinimo.toFixed(2).replace('.', ',') + ").");
            FindControl("txtValor", "input").value = valorMinimo.toFixed(2).replace('.', ',');
        }
    }

    function buscarProcessos(){
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProd", "input").value);
        openWindow(450, 700, "../Utils/SelEtiquetaProcesso.aspx?idSubgrupo=" + idSubgrupo.value);
    }
    
    // Função chamada pelo popup de escolha da Aplicação do produto
    function setApl(idAplicacao, codInterno) {    
        FindControl("txtAplIns", "input").value = codInterno;
        FindControl("hdfIdAplicacao", "input").value = idAplicacao;
    }

    function loadApl(codInterno) {
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

    // Função chamada pelo popup de escolha do Processo do produto
    function setProc(idProcesso, codInterno, codAplicacao) {

        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProd", "input").value);
        var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

        if(idSubgrupo.value != "" && retornoValidacao.value == "False" && FindControl("txtProcIns", "input").value != "")
        {
            FindControl("txtProcIns", "input").value = "";
            alert("Este processo não pode ser selecionado para este produto.")
            return false;
        }
        
        FindControl("txtProcIns", "input").value = codInterno;
        FindControl("hdfIdProcesso", "input").value = idProcesso;       

        if (codAplicacao && codAplicacao != "")
        {
            loadApl(codAplicacao);
        }           
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

    </script>

    <table>
        <tr>
            <td align="center">
                <table id="entrega">
                    <tr>
                        <td style="font-weight: bold;">
                            <asp:Label ID="Label1" runat="server" Text="Tipo Entrega"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="drpTipoEntrega" runat="server" onchange="loadProduto();calcTotal();"
                                DataSourceID="odsTipoEntrega" DataTextField="Descr" DataValueField="Id">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkRevenda" Text="Revenda" runat="server" onclick="revendaClick(this);loadProduto();calcTotal();" />
                            &nbsp;
                        </td>
                        <td style="font-weight: bold;">
                            <asp:Label ID="Label4" runat="server" Text="Data Entrega"></asp:Label>
                        </td>
                        <td>
                            <uc3:ctrlData ID="ctrlDataEntrega" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td style="font-weight: bold;">
                            <asp:Label ID="Label25" runat="server" Text="Tipo Pedido"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoPedido" runat="server" 
                                DataSourceID="odsTipoPedido" DataTextField="Descr" DataValueField="Id" OnDataBound="drpTipoPedido_DataBound">
                            </asp:DropDownList>
                        </td>
                        <td style="font-weight: bold; display: none">
                            <asp:Label ID="lblNumParcelas" runat="server" Text="Número parcelas"></asp:Label>
                        </td>
                        <td style="display: none">
                            <asp:DropDownList ID="drpNumParcelas" runat="server" DataSourceID="odsParcelas" DataTextField="Descr"
                                DataValueField="Id" onchange="atualizaTotalOrca(totalOrca)">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td style="font-weight: bold;" align="left">
                            <asp:Label ID="Label2" runat="server" Text="Produto"></asp:Label>
                        </td>
                        <td align="left">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtCodProd" runat="server" Width="50px" onblur="limpaCampos();loadProduto();"
                                            onkeydown="if (isEnter(event)) FindControl('txtQtde', 'input').select();" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblDescrProd" Style="margin-left: 3px; margin-right: 5px" runat="server" />
                                    </td>
                                    <td>
                                        <a href="#" onclick="openWindow(500, 650, '../Utils/SelProd.aspx'); return false;"
                                            tabindex="-1">
                                            <img src="../Images/Pesquisar.gif" border="0" /></a>&nbsp;
                                    </td>
                                    <td style="font-weight: bold;">
                                        <asp:Label ID="Label24" runat="server" Text="Valor"></asp:Label>
                                        &nbsp;
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtValor" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px" onchange="validaValorMinimo()" onblur="calcM2()" TabIndex="-1" OnLoad="txtValor_Load"></asp:TextBox>
                                        &nbsp;
                                    </td>
                                    <td style="font-weight: bold;">
                                        <asp:Label ID="Label7" runat="server" Text="Processo"></asp:Label>
                                        &nbsp;
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtProcIns" runat="server" onblur="loadProc(this.value);"
                                            onkeydown="if (isEnter(event)) { loadProc(this.value); }"
                                            onkeypress="return !(isEnter(event));" Width="30px" ></asp:TextBox>
                                    </td>
                                    <td>
                                        <a href="#" onclick="buscarProcessos(); return false;">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                    </td>                                    
                                    <td style="font-weight: bold;">
                                        <asp:Label ID="Label9" runat="server" Text="Aplicação"></asp:Label>
                                        &nbsp;
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAplIns" runat="server" onblur="loadApl(this.value);"
                                            onkeydown="if (isEnter(event)) { loadApl(this.value); }" 
                                            onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <a href="#" onclick="openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                            <img border="0" src="../Images/Pesquisar.gif" /></a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td style="font-weight: bold">
                            <asp:Label ID="Label23" runat="server" Text="Qtde"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtQtde" runat="server" Width="50px" onkeydown="if (event.keyCode==13) { calcTotal(); }"
                                onblur="calcM2(); return verificaEstoque()" onkeypress="return soNumeros(event, CalcProd_IsQtdeInteira(FindControl('hdfTipoCalc', 'input').value), true);"></asp:TextBox>
                            <uc2:ctrlDescontoQtde ID="ctrlDescontoQtde" runat="server" OnLoad="ctrlDescontoQtde_Load"
                                Callback="atualizaTotalDesc" />
                        </td>
                        <td style="font-weight: bold">
                            <asp:Label ID="lblLarguraAltura" runat="server" Text="Largura x Altura"></asp:Label>
                        </td>
                        <td>
                            <table id="tbLarguraAltura" cellpadding="0" cellspacing="0" runat="server">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtLargura" runat="server" Width="50px" onblur="calcM2();" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtAltura" runat="server" Width="50px" onblur="calcM2();" onkeypress="return soNumeros(event, CalcProd_IsAlturaInteira(FindControl('hdfTipoCalc', 'input').value), true);"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="font-weight: bold">
                            <asp:Label ID="lblTitleEspessura" runat="server" Text="Espessura"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtEspessura" onkeypress="return soNumeros(event, false, true);"
                                runat="server" MaxLength="2" Width="20px">0</asp:TextBox>
                            <asp:Label ID="lblMm" runat="server" Text="mm"></asp:Label>
                            &nbsp;
                        </td>
                        <td style="font-weight: bold">
                            <asp:Label ID="lblTitleTotM2" runat="server" Text="Tot. M2:"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTotM2" runat="server"></asp:Label>
                            <asp:Label ID="lblTotM2Calc" runat="server"></asp:Label>
                            <asp:HiddenField ID="hdfTotM2" runat="server" />
                            <asp:HiddenField ID="hdfTotM2SemChapa" runat="server" />
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <br />
                <br />
                <br />
                <table id="tbConfigVidro" cellspacing="0">
                    <tr>
                        <td>
                            <uc1:ctrlBenef ID="ctrlBenef1" runat="server" OnLoad="ctrlBenef_Load" EnableViewState="false"
                                CallbackCalculoValorTotal="callbackSetTotal" CssClassCabecalho="dtvHeader" SomarTotalValorBenef="True" />
                        </td>
                    </tr>
                </table>
                <table cellpadding="0" cellspacing="0" width="700px">
                    <tr>
                        <td align="center" style="font-size: medium" colspan="2">
                            <asp:HiddenField ID="hdfExibePopupEstoque" runat="server" />
                            <asp:HiddenField ID="hdfAlturaCalc" runat="server" />
                            <asp:HiddenField ID="hdfPercComissao" runat="server" />
                            <asp:HiddenField ID="hdfIdProd" runat="server" />
                            <asp:HiddenField ID="hdfValMin" runat="server" />
                            <asp:HiddenField ID="hdfIsVidro" runat="server" />
                            <asp:HiddenField ID="hdfTipoCalc" runat="server" />
                            <asp:HiddenField ID="hdfIsAluminio" runat="server" />
                            <asp:HiddenField ID="hdfM2Minimo" runat="server" />
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoEntrega" runat="server" SelectMethod="GetTipoEntrega"
                                TypeName="Glass.Data.Helper.DataSources">
                            </colo:VirtualObjectDataSource>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoPedido" runat="server" SelectMethod="GetTipoPedido" 
                                TypeName="Glass.Data.Helper.DataSources">
                            </colo:VirtualObjectDataSource>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoJato" runat="server" SelectMethod="GetTipoJato"
                                TypeName="Glass.Data.Helper.DataSources">
                            </colo:VirtualObjectDataSource>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCanto" runat="server" SelectMethod="GetTipoCanto"
                                TypeName="Glass.Data.Helper.DataSources">
                            </colo:VirtualObjectDataSource>
                            <colo:VirtualObjectDataSource culture="pt-BR" ID="odsParcelas" runat="server" SelectMethod="GetNumeroParcelas"
                                TypeName="Glass.Data.DAL.ParcelasDAO">
                            </colo:VirtualObjectDataSource>
                            <asp:HiddenField ID="hdfCustoProd" runat="server" />
                            <asp:HiddenField ID="hdfAliqIcms" runat="server" />
                            <asp:HiddenField ID="hdfIdCliente" runat="server" />
                            <asp:HiddenField ID="hdfIdOrca" runat="server" />
                            <asp:HiddenField ID="hdfNaoVendeVidro" runat="server" />
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr align="left">
                        <td align="center" style='font-size: <%= Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "10pt" : "medium" %>'>
                            <asp:Label ID="Label15" runat="server" Text="Subtotal:" Font-Bold="True"></asp:Label>
                            &nbsp;<asp:Label ID="lblTotal" runat="server"></asp:Label>
                            <span id="dadosIcms" style='<%= Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "": "display: none" %>'>
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label ID="Label3" runat="server" Text="Valor ICMS:" Font-Bold="True"></asp:Label>
                                &nbsp;<asp:Label ID="lblValorIcms" runat="server"></asp:Label>
                                &nbsp;&nbsp;&nbsp;
                                <asp:Label ID="Label6" runat="server" Text="Total Prod.:" Font-Bold="True" Font-Size="Large"></asp:Label>
                                &nbsp;<asp:Label ID="lblTotalProd" runat="server" Font-Size="Large"></asp:Label>
                            </span>
                            <br />
                            <span style='font-size: x-small; <%= String.IsNullOrEmpty(Request["PercComissao"]) || float.Parse(Request["PercComissao"]) <= 0 ? "display: none": "" %>'>
                                (valor considerando o percentual de comissão de
                                <%= Request["PercComissao"]%>%) </span>
                        </td>
                        <td nowrap="nowrap" width="50px">
                            <asp:Button ID="btnIncluir" runat="server" Text="Incluir Item" OnLoad="btnIncluir_Load" />
                            &nbsp;
                        </td>
                    </tr>
                </table>
                <br />
                <table id="lstProd" align="left" cellpadding="0" cellspacing="0" style="width: 100%;
                    caption-side: bottom">
                    <tr>
                        <td class="dtvHeader" height="20px" width="20px">
                        </td>
                        <td class="dtvHeader">
                            DESCRIÇÃO&nbsp;&nbsp;
                        </td>
                        <td class="dtvHeader">
                            QTDE&nbsp;&nbsp;
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="lblLargLst" runat="server">LARGURA</asp:Label>&nbsp;&nbsp;
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="lblAltLst" runat="server">ALTURA</asp:Label>&nbsp;&nbsp;
                        </td>
                        <td class="dtvHeader" nowrap="nowrap">
                            <asp:Label ID="lblLstTotM2" runat="server">TOT. M2</asp:Label>&nbsp;&nbsp;
                        </td>
                        <td class="dtvHeader">
                            <asp:Label ID="lblLstServicos" runat="server">SERVIÇOS</asp:Label>&nbsp;&nbsp;
                        </td>
                        <td class="dtvHeader" nowrap="nowrap">
                            VALOR UNIT.&nbsp;&nbsp;
                        </td>
                        <td class="dtvHeader" nowrap="nowrap">
                            TOTAL&nbsp;&nbsp;
                        </td>
                        <td class="dtvHeader" nowrap="nowrap">
                            PROCESSO / APLICAÇÃO&nbsp;&nbsp;
                        </td>
                    </tr>
                    <caption>
                        <td colspan="2" style="font-weight: bold; color: Blue; padding: 2px; padding-left: 0px"
                            align="center">
                            Totais
                        </td>
                        <td style="font-weight: bold; color: Blue; padding: 2px; padding-left: 0px">
                            <span id="totalQtde"></span>
                        </td>
                        <td colspan="2" style="font-weight: bold; color: Blue; padding: 2px; padding-left: 0px">
                        </td>
                        <td style="font-weight: bold; color: Blue; padding: 2px; padding-left: 0px">
                            <span id="totalM2"></span>
                        </td>
                        <td colspan="4" style="font-weight: bold; color: Blue; padding: 2px; padding-left: 0px">
                        </td>
                    </caption>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" style='font-size: <%= Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "10pt" : "large" %>'>
                <br />
                <asp:Label ID="lblTitleTotalOrca" runat="server" Text="Total do Orçamento:" Font-Bold="True"></asp:Label>&nbsp;
                <asp:Label ID="lblTotalOrca" runat="server" Text="R$ 0,00"></asp:Label>
                <span id="totalIcms" style='<%= Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido ? "": "display: none" %>'>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Label ID="Label5" runat="server" Text="Valor ICMS:" Font-Bold="True"></asp:Label>
                    &nbsp;<asp:Label ID="lblSomaIcms" runat="server" Text="R$ 0,00"></asp:Label>
                    &nbsp;&nbsp;&nbsp;
                    <asp:Label ID="Label8" runat="server" Text="Total com ICMS:" Font-Bold="True" Font-Size="Large"></asp:Label>
                    &nbsp;<asp:Label ID="lblTotalIcms" runat="server" Font-Size="Large" Text="R$ 0,00"></asp:Label>
                </span>
                <br />
                <span id="textoTaxaJuros" style='font-size: x-small; display: none'>
                    Taxa de juros:
                    0% por parcela (já incluso
                    no total) </span>
            </td>
        </tr>
        <tr id="separaTotal">
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table id="geracao">
                    <tr>
                        <td>
                            <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;">
                                <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkGerarPedido" runat="server" OnClientClick="gerarPedido(); return false;">
                                <img alt="" border="0" src="../Images/cart.png" /> Gerar Pedido</asp:LinkButton>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkGerarOrcamento" runat="server" OnClientClick="gerarOrcamento(); return false;">
                            <img alt="" border="0" src="../Images/book_go.png" /> Gerar Orçamento</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfIdProcesso" runat="server" />
    <asp:HiddenField ID="hdfIdAplicacao" runat="server" />

    <script type="text/javascript">
        if (FindControl("hdfNaoVendeVidro", "input").value == "true")
            FindControl("tblBenef", "table").style.display = "none";
    </script>

</asp:Content>
