<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlProdComposicaoOrcamentoChildAlterarProcApl.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlProdComposicaoOrcamentoChildAlterarProcApl" %>

<%@ Register Src="ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>

<script type="text/javascript">

var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
var utilizarRoteiroProducao = <%= UtilizarRoteiroProducao().ToString().ToLower() %>;
var nomeControleBenefComposicao = "<%= NomeControleBenefComposicao() %>";

function buscaTableChild(control) {
    var tr = control;

    while (tr.id == "" || (tr.id.indexOf("prodOrcamentoChild_") == -1 && tr.nodeName.toLowerCase() != "tr")) {
        tr = tr.parentElement;
    }

    return tr;
}

function getNomeControleBenefComposicaoChild(control) {        
    nomeControleBenefComposicao = FindControl(nomeControleBenefComposicao + "_tblBenef", "table", control);

    if (nomeControleBenefComposicao == null) {
        return null;
    }

    nomeControleBenefComposicao = nomeControleBenefComposicao.id;
    return nomeControleBenefComposicao.substr(0, nomeControleBenefComposicao.lastIndexOf("_"));
}

// Função chamada pelo popup de escolha da Aplicação do produto
function setAplComposicaoChild(idAplicacao, codInterno, idProdOrcamento) {
    var tr = FindControl("prodOrcamentoChild_" + idProdOrcamento, "tr");

    if (FindControl("txtChildAplComposicaoIns", "input", tr) != null) {
        FindControl("txtChildAplComposicaoIns", "input", tr).value = codInterno;
        FindControl("hdfChildIdAplicacaoComposicao", "input", tr).value = idAplicacao;
    }
        
    aplAmbienteComposicaoChild = false;
}

function loadAplComposicaoChild(control, codInterno) {
    var tr = buscaTableChild(control);
    var idProdOrcamento = FindControl("hdfChildIdProdOrcamento", "input", tr).value;

    if (codInterno == "") {
        setAplComposicaoChild("", "", idProdOrcamento);
        return false;
    }
    
    try {
        var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

        if (response == null || response == "") {
            alert("Falha ao buscar Aplicação. Ajax Error.");
            setAplComposicaoChild("", "", idProdOrcamento);
            return false
        }

        response = response.split("\t");
            
        if (response[0] == "Erro") {
            alert(response[1]);
            setAplComposicaoChild("", "", idProdOrcamento);
            return false;
        }

        setAplComposicaoChild(response[1], response[2], idProdOrcamento);
    }
    catch (err) { alert(err); }
}

// Função chamada pelo popup de escolha do Processo do produto
function setProcComposicaoChild(idProcesso, codInterno, codAplicacao, idProdOrcamento) {
    var codInternoProd = "";
    var codAplicacaoAtual = "";
    var tr = FindControl("prodOrcamentoChild_" + idProdOrcamento, "tr");        
    var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfChildIdProdutoComposicao", "input", tr).value);
    var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

    if (idSubgrupo.value != "" && retornoValidacao.value == "false" && (FindControl("txtChildProcComposicaoIns", "input", tr) != null && FindControl("txtChildProcComposicaoIns", "input", tr).value != "")) {
        FindControl("txtChildProcComposicaoIns", "input", tr).value = "";
        alert("Este processo não pode ser selecionado para este produto.")
        return false;
    }

    var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, FindControl("hdfIdOrcamento", "input").value);
        
    if (verificaEtiquetaProc.error != null) {
        if (FindControl("txtChildProcComposicaoIns", "input", tr) != null) {
            FindControl("txtChildProcComposicaoIns", "input", tr).value = "";
            FindControl("hdfChildIdProcessoComposicao", "input", tr).value = "";
        }

        setAplComposicaoChild("", "", idProdOrcamento);
        alert(verificaEtiquetaProc.error.description);

        return false;
    }

    FindControl("txtChildProcComposicaoIns", "input", tr).value = codInterno;
    FindControl("hdfChildIdProcessoComposicao", "input", tr).value = idProcesso;
            
    if (FindControl("txtChildCodProdComposicaoIns", "input", tr) != null) {
        codInternoProd = FindControl("txtChild_CodProdComposicaoIns", "input", tr).value;
    } else {
        codInternoProd = FindControl("hdfCodProdComposicaoIns", "input", tr).innerHTML;
    }
                
    codAplicacaoAtual = FindControl("txtChildAplComposicaoIns", "input", tr).value;
        
    if (((codAplicacao && codAplicacao != "") || (codInternoProd != "" && CadOrcamento.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) && (codAplicacaoAtual == null || codAplicacaoAtual == "")) {
        loadAplComposicaoChild(tr, codAplicacao);
    }
}

function loadProcComposicaoChild(control, codInterno) {
    var tr = buscaTableChild(control);
    var idProdOrcamento = FindControl("hdfChildIdProdOrcamento", "input", tr).value;

    if (codInterno == "") {
        setProcComposicaoChild("", "", "", idProdOrcamento);
        return false;
    }

    try {
        var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

        if (response == null || response == "") {
            alert("Falha ao buscar Processo. Ajax Error.");
            setProcComposicaoChild("", "", "", idProdOrcamento);
            return false
        }

        response = response.split("\t");
            
        if (response[0] == "Erro") {
            alert(response[1]);
            setProcComposicaoChild("", "", "", idProdOrcamento);
            return false;
        }

        setProcComposicaoChild(response[1], response[2], response[3], idProdOrcamento);
    }
    catch (err) { alert(err); }
}

function setValorTotalComposicaoChild(valor, custo, idProdOrcamento) {
    if (getNomeControleBenefComposicaoChild() != null) {
        if (exibirControleBenef(getNomeControleBenefComposicaoChild())) {
            var tr = FindControl("prodOrcamentoChild_" + idProdOrcamento, "tr");
            var lblValorBenef = FindControl("lblValorBenefComposicao", "span", tr);

            lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
        }
    }
}

function selProcComposicaoChild(control) {
    var tr = buscaTableChild(control);
    var idProdOrcamento = FindControl("hdfChildIdProdOrcamento", "input", tr).value;
    var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdfChildIdProdutoComposicao", "input", tr).value);

    openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx?idProdOrcamento=' + idProdOrcamento + '&idSubgrupo=' + idSubgrupo.value);

    return false;
}

function selAplComposicaoChild(control) {
    var tr = buscaTableChild(control);
    var idProdOrcamento = FindControl("hdfChildIdProdOrcamento", "input", tr).value;

    openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx?idProdOrcamento=' + idProdOrcamento);
    return false;
}

function obrigarProcAplComposicaoChild(control) {
    var table = buscaTableChild(control);
    var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
    var isVidroRoteiro = dadosProduto.Grupo == 1 && utilizarRoteiroProducao;
        
    if (dadosProduto.IsChapaVidro) {
        return true;
    }

    if (isVidroRoteiro || (isObrigarProcApl && isVidroBenef)) {
        if (FindControl("txtChildAplComposicaoIns", "input", table) != null && FindControl("txtChildAplComposicaoIns", "input", table).value == "") {
            if (isVidroRoteiro && !isObrigarProcApl) {
                alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                return false;
            }

            alert("Informe a aplicação.");
            return false;
        }
            
        if (FindControl("txtChildProcComposicaoIns", "input", table) != null && FindControl("txtChildProcComposicaoIns", "input", table).value == "") {
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
function onUpdateProdComposicaoChild(control, idTbConfigVidro) {                
    if (!obrigarProcAplComposicaoChild(table)) {
        return false;
    }
            
    return true;
}

</script>

<asp:GridView GridLines="None" ID="grdProdutosOrcamentoComposicao" runat="server" DataSourceID="odsProdutosOrcamentoComposicao" DataKeyNames="IdProd"
    PageSize="12" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
    OnRowCommand="grdProdutosOrcamentoComposicao_RowCommand" OnRowUpdated="grdProdutosOrcamentoComposicao_RowUpdated">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <!-- 0 -->
                <!-- EDITAR -->
                <asp:ImageButton ID="lnkChildEditComposicao" runat="server" CommandName="Edit" ImageUrl="~/Images/Edit.gif" Visible='<%# Eval("AlterarProcessoAplicacaoVisible") %>'
                     OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>' />
            </ItemTemplate>
            <EditItemTemplate>
                <!-- ATUALIZAR -->
                <asp:ImageButton ID="imbChildAtualizarComposicao" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar"
                    OnClientClick='<%# "if (!onUpdateProdComposicaoChild(this, &#39;" + IdProdOrcamento + "_" + Eval("IdProd") + "&#39;)) return false;" %>' />
                <!-- CANCELAR -->
                <asp:ImageButton ID="imbChildCancelarComposicao" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />

                <asp:HiddenField ID="hdfChildProdOrcamentoComposicao" runat="server" Value='<%# Bind("IdProd") %>' />
                <asp:HiddenField ID="hdfChildIdProdutoComposicao" runat="server" Value='<%# Eval("IdProduto") %>' />
                <asp:HiddenField ID="hdfChildCodProdComposicaoIns" runat="server" Value='<%# Eval("CodInterno") %>' />
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
            <ItemTemplate>
                <!-- 1 -->
                <!-- CODIGO -->
                <asp:Label ID="lblChildCodProdComposicao" runat="server" Text='<%# Eval("CodInterno") %>'>
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
                <asp:Label ID="lblChildProdutoComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") + (!string.IsNullOrEmpty(Eval("DescrBeneficiamentos").ToString()) ? " " + Eval("DescrBeneficiamentos") : "") %>'>
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
                <asp:Label ID="lblChildQtdeComposicao" runat="server" Text='<%# Eval("Qtde") %>'>
                </asp:Label>
                <!-- QUANTIDADE AMBIENTE -->
                <asp:Label ID="lblChildQtdeAmbienteComposicao" runat="server">
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
                <asp:Label ID="lblChildLarguraComposicao" runat="server" Text='<%# Eval("Largura") %>'>
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
                <asp:Label ID="lblChildAlturaComposicao" runat="server" Text='<%# Eval("Altura") %>'>
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
                <asp:Label ID="lblChildTotMComposicao" runat="server" Text='<%# Eval("TotM") %>'>
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
                <asp:Label ID="lblChildTotm2CalcComposicao" runat="server" Text='<%# Eval("TotMCalc") %>'>
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
                <asp:Label ID="lblChildValorVendidoComposicao" runat="server" Text='<%# Eval("ValorVendido", "{0:C}") %>'>
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
                <asp:Label ID="lblChildCodProcessoComposicao" runat="server" Text='<%# Eval("CodProcesso") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- CODIGO PROCESSO -->
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtChildProcComposicaoIns" runat="server" Text='<%# Eval("CodProcesso") %>' Width="30px" onkeypress="return !(isEnter(event));"
                                onblur="loadProcComposicaoChild(this, this.value);" onkeydown="if (isEnter(event)) { loadProcComposicaoChild(this, this.value); }">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selProcComposicaoChild(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfChildIdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
            <ItemTemplate>
                <!-- 10 -->
                <!-- CODIGO APLICACAO -->
                <asp:Label ID="lblChildCodAplicacaoComposicao" runat="server" Text='<%# Eval("CodAplicacao") %>'>
                </asp:Label>
            </ItemTemplate>
            <EditItemTemplate>
                <!-- CODIGO APLICACAO -->
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txtChildAplComposicaoIns" runat="server" Text='<%# Eval("CodAplicacao") %>' Width="30px" onkeypress="return !(isEnter(event));"
                                onblur="loadAplComposicaoChild(this, this.value);" onkeydown="if (isEnter(event)) { loadAplComposicaoChild(this, this.value); }">
                            </asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="return selAplComposicaoChild(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdfChildIdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Total" SortExpression="Total">
            <ItemTemplate>
                <!-- 11 -->
                <!-- TOTAL -->
                <asp:Label ID="lblChildTotalComposicao" runat="server" Text='<%# Eval("Total", "{0:C}") %>'>
                </asp:Label>
                <!-- PERCENTUAL DESCONTO QUANTIDADE -->
                <asp:Label ID="lblChildPercDescontoQtdeComposicao" runat="server" Text='<%# "(Desconto de " + Eval("PercDescontoQtde") + "%)" %>' Visible='<%# (float)Eval("PercDescontoQtde") > 0 %>'>
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
                <asp:Label ID="lblChildValorBenefComposicao" runat="server" Text='<%# Eval("ValorBenef", "{0:C}") %>'>
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
            </ItemTemplate>
            <EditItemTemplate>
                <!-- 13 -->
                <!-- CONTROLE BENEFICIAMENTO -->
                <table id="tb_ConfigVidroComposicao_<%# Eval("IdProdOrcamentoParent") + "_" + Eval("IdProd") %>" cellspacing="0" style="display: none;">
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <!-- ESPESSURA -->
                                        <asp:TextBox ID="txtChildEspessuraComposicao" runat="server" OnDataBinding="txtChildEspessuraComposicao_DataBinding"
                                            onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Eval("Espessura") %>'>
                                        </asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrlChildBenefEditarComposicao" runat="server" Beneficiamentos='<%# Eval("Beneficiamentos") %>'
                                ValidationGroup="produtoComposicaoChild" OnInit="ctrlChildBenef_Load" Redondo='<%# Eval("Redondo") %>' CallbackCalculoValorTotal="setValorTotalComposicaoChild" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                        </td>
                    </tr>
                </table>
            </EditItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerStyle CssClass="pgr"></PagerStyle>
    <EditRowStyle CssClass="edit"></EditRowStyle>
    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
</asp:GridView>
<asp:HiddenField runat="server" ID="hdfChildIdProdOrcamento" />
<asp:HiddenField runat="server" ID="hdfChildIdProdutoComposicao" />
<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutosOrcamentoComposicao" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosOrcamento" TypeName="Glass.Data.DAL.ProdutosOrcamentoDAO"
    EnablePaging="True" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"    
    SelectMethod="PesquisarProdutosComposicao" SelectCountMethod="PesquisarProdutosComposicaoCount" UpdateMethod="UpdateProcessoAplicacao"
    OnUpdated="odsProdutosOrcamentoComposicao_Updated">
    <SelectParameters>
        <asp:QueryStringParameter QueryStringField="idOrcamento" Name="idOrcamento" Type="Int32" />
        <asp:ControlParameter ControlID="hdfIdProdAmbienteOrcamento" Name="idProdAmbienteOrcamento" PropertyName="Value" Type="Int32" />
        <asp:ControlParameter ControlID="hdfChildIdProdOrcamento" Name="idProdOrcamentoParent" PropertyName="Value" Type="Int32" />
    </SelectParameters>
</colo:VirtualObjectDataSource>