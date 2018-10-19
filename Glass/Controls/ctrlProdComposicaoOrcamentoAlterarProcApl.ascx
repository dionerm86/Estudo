<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlProdComposicaoOrcamentoAlterarProcApl.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlProdComposicaoOrcamentoAlterarProcApl" %>

<%@ Register Src="ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlProdComposicaoOrcamentoChildAlterarProcApl.ascx" TagName="ctrlProdComposicaoOrcamentoChild" TagPrefix="uc1"%>

<script type="text/javascript">

// Guarda a quantidade disponível em estoque do produto buscado
var qtdEstoqueComposicao = 0;
var exibirMensagemEstoqueComposicao = false;
var qtdEstoqueMensagemComposicao = 0;    
var insertingComposicao = false;
var produtoAmbienteComposicao = false;
var loadingComposicao = true;
var idOrcamento = <%= Request["idOrca"] != null ? Request["idOrca"] : "0" %>;
var nomeControleBenefComposicao = "<%= NomeControleBenefComposicao() %>";
var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
var utilizarRoteiroProducao = <%= UtilizarRoteiroProducao().ToString().ToLower() %>;

function buscaTable(control) {
    var tr = control;

    while (tr.id == "" || (tr.id.indexOf("produtoOrcamento_") == -1 && tr.nodeName.toLowerCase() != "tr")) {
        tr = tr.parentElement;
    }

    return tr;
}

function getNomeControleBenefComposicao(control) {
    nomeControleBenefComposicao = FindControl(nomeControleBenefComposicao + "_tblBenef", "table", control);

    if (nomeControleBenefComposicao == null) {
        return null;
    }

    nomeControleBenefComposicao = nomeControleBenefComposicao.id;
    return nomeControleBenefComposicao.substr(0, nomeControleBenefComposicao.lastIndexOf("_"));
}

// Função chamada pelo popup de escolha da Aplicação do produto
function setAplComposicao(idAplicacao, codInterno, idProdOrcamento) {
    var tr = FindControl("produtoOrcamento_" + idProdOrcamento, "tr");

    if (tr == null || tr == undefined) {
        setAplComposicaoChild(idAplicacao, codInterno, idProdOrcamento);
    } else {
        FindControl("txtAplComposicaoIns", "input", tr).value = codInterno;
        FindControl("hdfIdAplicacaoComposicao", "input", tr).value = idAplicacao;
    }
}

function loadAplComposicao(control, codInterno) {
    var tr = buscaTable(control);
    var idProdOrcamento = FindControl("hdfIdProdOrcamento", "input", tr).value;

    if (codInterno == "") {
        setAplComposicao("", "", idProdOrcamento);
        return false;
    }
    
    try {
        var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

        if (response == null || response == "") {
            alert("Falha ao buscar Aplicação. Ajax Error.");
            setAplComposicao("", "", idProdOrcamento);
            return false
        }

        response = response.split("\t");
            
        if (response[0] == "Erro") {
            alert(response[1]);
            setAplComposicao("", "", idProdOrcamento);
            return false;
        }

        setAplComposicao(response[1], response[2], idProdOrcamento);
    }
    catch (err) { alert(err); }
}

// Função chamada pelo popup de escolha do Processo do produto
function setProcComposicao(idProcesso, codInterno, codAplicacao, idProdOrcamento) {
    var codInternoProd = "";
    var codAplicacaoAtual = "";
    var tr = FindControl("produtoOrcamento_" + idProdOrcamento, "tr");

    if (tr == null || tr == undefined) {
        setProcComposicaoChild(idProcesso, codInterno, codAplicacao, idProdOrcamento);
    } else {
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProdutoComposicao", "input", tr).value);
        var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

        if (idSubgrupo.value != "" && retornoValidacao.value == "false" && (FindControl("txtProcComposicaoIns", "input", tr) != null && FindControl("txtProcComposicaoIns", "input", tr).value != "")) {
            FindControl("txtProcComposicaoIns", "input", tr).value = "";
            alert("Este processo não pode ser selecionado para este produto.");
            return false;
        }

        var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, idOrcamento);
        
        if (verificaEtiquetaProc.error != null) {
            FindControl("txtProcComposicaoIns", "input", tr).value = "";
            FindControl("hdfIdProcessoComposicao", "input", tr).value = "";

            setAplComposicao("", "", idProdOrcamento);
            alert(verificaEtiquetaProc.error.description);
            return false;
        }

        FindControl("txtProcComposicaoIns", "input", tr).value = codInterno;
        FindControl("hdfIdProcessoComposicao", "input", tr).value = idProcesso;
            
        if (FindControl("txtCodProdComposicaoIns", "input", tr) != null) {
            codInternoProd = FindControl("txtCodProdComposicaoIns", "input", tr).value;
        } else {
            codInternoProd = FindControl("hdfCodProdComposicaoIns", "input", tr).value;
        }
                
        codAplicacaoAtual = FindControl("txtAplComposicaoIns", "input", tr).value;
        
        if (((codAplicacao && codAplicacao != "") || (codInternoProd != "" && CadOrcamento.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) && (codAplicacaoAtual == null || codAplicacaoAtual == "")) {
            loadAplComposicao(tr, codAplicacao);
        }
    }
}

function loadProcComposicao(control, codInterno) {
    var tr = buscaTable(control);
    var idProdOrcamento = FindControl("hdfIdProdOrcamento", "input", tr).value;

    if (codInterno == "") {
        setProcComposicao("", "", "", idProdOrcamento);
        return false;
    }

    try {
        var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

        if (response == null || response == "") {
            alert("Falha ao buscar Processo. Ajax Error.");
            setProcComposicao("", "", "", idProdOrcamento);
            return false
        }

        response = response.split("\t");
            
        if (response[0] == "Erro") {
            alert(response[1]);
            setProcComposicao("", "", "", idProdOrcamento);
            return false;
        }

        setProcComposicao(response[1], response[2], response[3], idProdOrcamento);
    }
    catch (err) { alert(err); }
}

function setValorTotalComposicao(valor, custo, idProdOrcamento) {
    if (getNomeControleBenefComposicao() != null) {
        if (exibirControleBenef(getNomeControleBenefComposicao())) {
            var tr = FindControl("produtoOrcamento_" + idProdOrcamento, "tr");
            var lblValorBenef = FindControl("lblValorBenefComposicao", "span", tr);

            lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
        }
    }
}

function selProcComposicao(control) {
    var tr = buscaTable(control);
    var idProdOrcamento = FindControl("hdfIdProdOrcamento", "input", tr).value;
    var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfIdProdutoComposicao", "input", tr).value);

    openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx?idProdOrcamento=' + idProdOrcamento + '&idSubgrupo=' + idSubgrupo.value);

    return false;
}

function selAplComposicao(control) {
    var tr = buscaTable(control);
    var idProdOrcamento = FindControl("hdfIdProdOrcamento", "input", tr).value;

    openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx?idProdOrcamento=' + idProdOrcamento);
    return false;
}

function obrigarProcAplComposicao(control) {
    var table = buscaTable(control);
    var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
    var isVidroRoteiro = dadosProduto.Grupo == 1 && utilizarRoteiroProducao;

    if (dadosProduto.IsChapaVidro) {
        return true;
    }

    if (isVidroRoteiro || (isObrigarProcApl && isVidroBenef)) {
        if (FindControl("txtAplComposicaoIns", "input", table) != null && FindControl("txtAplComposicaoIns", "input", table).value == "") {
            if (isVidroRoteiro && !isObrigarProcApl) {
                alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                return false;
            }

            alert("Informe a aplicação.");
            return false;
        }
            
        if (FindControl("txtProcComposicaoIns", "input", table) != null && FindControl("txtProcComposicaoIns", "input", table).value == "") {
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

// Função chamada quando o produto está para ser atualizado
function onUpdateProdComposicao(control, idTbConfigVidro) {        
    if (!obrigarProcAplComposicao(table)) {
        return false;
    }
                    
    return true;
}

function exibirProdsComposicaoChild(botao, idProdOrcamento) {
    var grdProds = FindControl("grdProdutosOrcamento", "table");

    if (grdProds == null) {
        return;
    }

    var linha = document.getElementById("prodOrcamentoChild_" + idProdOrcamento);
    var exibir = linha.style.display == "none";
    linha.style.display = exibir ? "" : "none";
    botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
    botao.title = (exibir ? "Esconder" : "Exibir") + " Produtos da Composição";
}

</script>

<asp:GridView GridLines="None" ID="grdProdutosOrcamentoComposicao" runat="server" DataSourceID="odsProdutosOrcamentoComposicao" DataKeyNames="IdProd"
    PageSize="12" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
    OnRowCommand="grdProdutosOrcamentoComposicao_RowCommand" OnRowUpdated="grdProdutosOrcamentoComposicao_RowUpdated">
    <FooterStyle Wrap="True" />
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 0 -->
                <!-- EDITAR -->
                <asp:ImageButton ID="lnkEditComposicao" runat="server" CommandName="Edit" ImageUrl="~/Images/Edit.gif" Visible='<%# Eval("AlterarProcessoAplicacaoVisible") %>'
                     OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <!-- ATUALIZAR -->
                <asp:ImageButton ID="imbAtualizarComposicao" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" 
                    OnClientClick='<%# "if (!onUpdateProdComposicao(this, &#39;" + IdProdOrcamento + "_" + Eval("IdProd") + "&#39;)) return false;" %>' />
                <!-- CANCELAR -->
                <asp:ImageButton ID="imbCancelarComposicao" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                
                <asp:HiddenField ID="hdfCodProdComposicaoIns" runat="server" Value='<%# Eval("CodInterno") %>' />
                <asp:HiddenField ID="hdfIdProdOrcamentoComposicao" runat="server" Value='<%# Eval("IdProd") %>' />
                <asp:HiddenField ID="hdfIdProdutoComposicao" runat="server" Value='<%# Bind("IdProduto") %>' />
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
            <ItemTemplate>
                <!-- 1 -->
                <!-- CODIGO -->
                <asp:Label ID="lblCodProdComposicao" runat="server" Text='<%# Eval("CodInterno") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
            <ItemTemplate>
                <!-- 2 -->
                <!-- DESCRICAO -->
                <asp:Label ID="lblProdutoComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") + (!string.IsNullOrEmpty(Eval("DescrBeneficiamentos").ToString()) ? " " + Eval("DescrBeneficiamentos") : "") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
            <ItemTemplate>
                <!-- 3 -->
                <!-- QUANTIDADE -->
                <asp:Label ID="lblQtdeComposicao" runat="server" Text='<%# Eval("Qtde") %>'>
                </asp:Label>
                <!-- QUANTIDADE AMBIENTE -->
                <asp:Label ID="lblQtdeAmbienteComposicao" runat="server" >
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
            <ItemTemplate>
                <!-- 4 -->
                <!-- LARGURA -->
                <asp:Label ID="lblLarguraComposicao" runat="server" Text='<%# Eval("Largura") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
            <ItemTemplate>
                <!-- 5 -->
                <!-- ALTURA -->
                <asp:Label ID="lblAlturaComposicao" runat="server" Text='<%# Eval("Altura") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
            <ItemTemplate>
                <!-- 6 -->
                <!-- TOTAL M2 -->
                <asp:Label ID="lblTotMComposicao" runat="server" Text='<%# Eval("TotM") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotMCalc">
            <ItemTemplate>
                <!-- 7 -->
                <!-- TOTAL M2 CALCULADO -->
                <asp:Label ID="lblTotM2CalcComposicao" runat="server" Text='<%# Eval("TotMCalc") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <HeaderStyle Wrap="True" />
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
            <ItemTemplate>
                <!-- 8 -->
                <!-- VALOR VENDIDO -->
                <asp:Label ID="lblValorVendidoComposicao" runat="server" Text='<%# Eval("ValorProd", "{0:C}") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
            <ItemTemplate>
                <!-- 9 -->
                <!-- CODIGO PROCESSO -->
                <asp:Label ID="lblCodProcessoComposicao" runat="server" Text='<%# Eval("CodProcesso") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtProcComposicaoIns" runat="server" Width="30px" Text='<%# Eval("CodProcesso") %>' onkeypress="return !(isEnter(event));"
                                onblur="loadProcComposicao(this, this.value);" onkeydown="if (isEnter(event)) { loadProcComposicao(this, this.value); }">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selProcComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
            <ItemTemplate>
                <!-- 10 -->
                <!-- CODIGO APLICACAO -->
                <asp:Label ID="lblCodAplicacaoComposicao" runat="server" Text='<%# Eval("CodAplicacao") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtAplComposicaoIns" runat="server" Text='<%# Eval("CodAplicacao") %>' Width="30px" onkeypress="return !(isEnter(event));"
                                onblur="loadAplComposicao(this, this.value);" onkeydown="if (isEnter(event)) { loadAplComposicao(this, this.value); }">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selAplComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfIdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Total" SortExpression="Total">
            <ItemTemplate>
                <!-- 11 -->
                <!-- TOTAL -->
                <asp:Label ID="lblTotalComposicao" runat="server" Text='<%# Eval("Total", "{0:C}") %>'></asp:Label>
                <!-- PERCENTUAL DESCONTO QUANTIDADE -->
                <asp:Label ID="lblPercDescontoQtdeComposicao" runat="server" Text='<%# "(Desconto de " + Eval("PercDescontoQtde") + "%)" %>' Visible='<%# (float)Eval("PercDescontoQtde") > 0 %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="V. Benef." SortExpression="ValorBenef">
            <ItemTemplate>
                <!-- 12 -->
                <!-- VALOR BENEFICIAMENTO -->
                <asp:Label ID="lblValorBenefComposicao" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 13 -->
            </ItemTemplate>
            <EditItemTemplate>
                <!-- CONTROLE BENEFICIAMENTO -->
                <table id="tb_ConfigVidroComposicao_<%# Eval("IdProdOrcamentoParent") + "_" + Eval("IdProd") %>" cellspacing="0" style="display: none;">
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtEspessuraComposicao" runat="server" Width="30px" Text='<%# Eval("Espessura") %>'
                                            OnDataBinding="txtEspessuraComposicao_DataBinding" onkeypress="return soNumeros(event, false, true);">
                                        </asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrlBenefEditarComposicao" runat="server" Beneficiamentos='<%# Eval("Beneficiamentos") %>' Redondo='<%# Eval("Redondo") %>'
                                ValidationGroup="produtoComposicao" OnInit="ctrlBenef_Load" CallbackCalculoValorTotal="setValorTotalComposicao" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left"></td>
                    </tr>
                </table>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 14 -->
                <!-- EXIBIR CONTROLE PRODUTO COMPOSICAO -->
                <div id='<%# "imgProdsComposto_" + Eval("IdProd") %>'>
                    <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/box.png" ToolTip="Exibir Produtos da Composição"
                        Visible='<%# Eval("IsProdLamComposicao") %>' OnClientClick='<%# "exibirProdsComposicaoChild(this, " + Eval("IdProd") + "); return false"%>' />                     
                </div>
            </ItemTemplate>
            <EditItemTemplate></EditItemTemplate>            
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 15 -->
                <!-- CONTROLE PRODUTO COMPOSICAO -->
                <tr id="prodOrcamentoChild_<%# Eval("IdProd") %>" style="display: none" align="center">
                    <td colspan="17">
                        <br />
                        <uc1:ctrlProdComposicaoOrcamentoChild runat="server" id="ctrlProdCompChild" visible='<%# Eval("IsProdLamComposicao") %>' IdProdOrcamento='<%# Glass.Conversoes.StrParaUint(Eval("IdProd").ToString()) %>' />
                        <br />
                    </td>
                </tr>
            </ItemTemplate>
            <EditItemTemplate></EditItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerStyle CssClass="pgr"></PagerStyle>
    <EditRowStyle CssClass="edit"></EditRowStyle>
    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
</asp:GridView>
<asp:HiddenField runat="server" ID="hdfIdProdOrcamento" />
<asp:HiddenField runat="server" ID="hdfIdProdutoComposicao" />
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosOrcamentoComposicao" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosOrcamento" TypeName="Glass.Data.DAL.ProdutosOrcamentoDAO"
    EnablePaging="True" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
    SelectCountMethod="PesquisarProdutosComposicaoCount" SelectMethod="PesquisarProdutosComposicao" UpdateMethod="UpdateProcessoAplicacao"
    OnUpdated="odsProdutosOrcamentoComposicao_Updated">
    <SelectParameters>
        <asp:QueryStringParameter QueryStringField="idOrcamento" Name="idOrcamento" Type="Int32" />
        <asp:ControlParameter ControlID="hdfIdProdAmbienteOrcamento" Name="idProdAmbienteOrcamento" PropertyName="Value" Type="Int32" />
        <asp:ControlParameter ControlID="hdfIdProdOrcamento" Name="idProdOrcamentoParent" PropertyName="Value" Type="Int32" />
    </SelectParameters>
</colo:VirtualObjectDataSource>
