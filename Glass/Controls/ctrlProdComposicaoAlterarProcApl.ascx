<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlProdComposicaoAlterarProcApl.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlProdComposicaoAlterarProcApl" %>

<%@ Register Src="ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>
<%@ Register Src="../Controls/ctrlDescontoQtde.ascx" TagName="ctrlDescontoQtde" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ctrlProdComposicaoChildAlterarProcApl.ascx" TagName="ctrlProdComposicaoChild" TagPrefix="uc1"%>


<script type="text/javascript">

    // Guarda a quantidade disponível em estoque do produto buscado
    var qtdEstoqueComposicao = 0;
    var exibirMensagemEstoqueComposicao = false;
    var qtdEstoqueMensagemComposicao = 0;
    
    var insertingComposicao = false;
    var produtoAmbienteComposicao = false;
    var aplAmbienteComposicao = false;
    var procAmbienteComposicao = false;
    var loadingComposicao = true;

    function buscaTable(control) {
        var tr = control;
        while (tr.id == "" || (tr.id.indexOf("prodPed_") == -1 && tr.nodeName.toLowerCase() != "tr")) {
            tr = tr.parentElement;
        }

        return tr;
    }

    function getNomeControleBenefComposicao(control)
    {
        var nomeControle = "<%= NomeControleBenefComposicao() %>";
        nomeControle = FindControl(nomeControle + "_tblBenef", "table", control);

        if (nomeControle == null)
            return null;

        nomeControle = nomeControle.id;
        return nomeControle.substr(0, nomeControle.lastIndexOf("_"));
    }

    function calculaTamanhoMaximoComposicao(control)
    {
        if (FindControl("hdf_CodProdComposicaoIns", "input") == null)
            return;

        var table = buscaTable(control);
            
        var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;
        var codInterno = FindControl("hdf_CodProdComposicaoIns", "input", table).value;
        var totM2 = FindControl("lbl_TotM2ComposicaoIns", "span", table).innerHTML;
        var idProdPed = FindControl("hdf_ProdPedComposicao", "input", table) != null ? FindControl("hdf_ProdPedComposicao", "input", table).value : 0;
        
        var tamanhoMaximo = CadPedido.GetTamanhoMaximoProduto(idPedido, codInterno, totM2, idProdPed).value.split(";");
        tamanhoMaximo = tamanhoMaximo[0] == "Ok" ? parseFloat(tamanhoMaximo[1].replace(",", ".")) : 0;
        
        FindControl("hdf_TamanhoMaximoObraComposicao", "input", table).value = tamanhoMaximo;
    }

    // Função chamada pelo popup de escolha da Aplicação do produto
    function setAplComposicao(idAplicacao, codInterno, idProdPed) {

        var tr = FindControl("prodPed_" + idProdPed, "tr");

        if (tr == null || tr == undefined)
            setAplComposicaoChild(idAplicacao, codInterno, idProdPed);
        else
        {
            if (!aplAmbienteComposicao)
            {
                FindControl("txt_AplComposicaoIns", "input", tr).value = codInterno;
                FindControl("hdf_IdAplicacaoComposicao", "input", tr).value = idAplicacao;
            }
            else
            {
                FindControl("txt_AmbAplComposicaoIns", "input", tr).value = codInterno;
                FindControl("hdf_AmbIdAplicacaoComposicao", "input", tr).value = idAplicacao;
            }
        
            aplAmbienteComposicao = false;
        }
    }

    function loadAplComposicao(control, codInterno) {

        var tr = buscaTable(control);

        var idProdPed = FindControl("hdf_IdProdPed", "input", tr).value;

        if (codInterno == "") {
            setAplComposicao("", "", idProdPed);
            return false;
        }
    
        try {
            var response = MetodosAjax.GetEtiqAplicacao(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Aplicação. Ajax Error.");
                setAplComposicao("", "", idProdPed);
                return false
            }

            response = response.split("\t");
            
            if (response[0] == "Erro") {
                alert(response[1]);
                setAplComposicao("", "", idProdPed);
                return false;
            }

            setAplComposicao(response[1], response[2], idProdPed);
        }
        catch (err) {
            alert(err);
        }
    }

    // Função chamada pelo popup de escolha do Processo do produto
    function setProcComposicao(idProcesso, codInterno, codAplicacao, idProdPed) {
        var codInternoProd = "";
        var codAplicacaoAtual = "";

        var tr = FindControl("prodPed_" + idProdPed, "tr");

        if (tr == null || tr == undefined)
            setProcComposicaoChild(idProcesso, codInterno, codAplicacao, idProdPed);
        else
        {
            var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdf_IdProdComposicao", "input", tr).value);
            var retornoValidacao = MetodosAjax.ValidarProcesso(idSubgrupo.value, idProcesso);

            if(idSubgrupo.value != "" && retornoValidacao.value == "False" && (FindControl("txt_ProcComposicaoIns", "input", tr) != null && FindControl("txt_ProcComposicaoIns", "input", tr).value != ""))
            {
                FindControl("txt_ProcComposicaoIns", "input", tr).value = "";
                alert("Este processo não pode ser selecionado para este produto.")
                return false;
            }

            var verificaEtiquetaProc = MetodosAjax.VerificaEtiquetaProcesso(idProcesso, FindControl("hdfIdPedido", "input").value);
        
            if(verificaEtiquetaProc.error != null){

                if (!procAmbienteComposicao && FindControl("txt_ProcComposicaoIns", "input", tr) != null)
                {
                    FindControl("txt_ProcComposicaoIns", "input", tr).value = "";
                    FindControl("hdf_IdProcessoComposicao", "input", tr).value = "";
                }
                else
                {
                    FindControl("txt_AmbProcComposicaoIns", "input", tr).value = "";
                    FindControl("hdf_AmbIdProcessoComposicao", "input", tr).value = "";
                }

                setAplComposicao("", "", idProdPed);

                alert(verificaEtiquetaProc.error.description);
                return false;
            }

            if (!procAmbienteComposicao)
            {
                FindControl("txt_ProcComposicaoIns", "input", tr).value = codInterno;
                FindControl("hdf_IdProcessoComposicao", "input", tr).value = idProcesso;
            
                if (FindControl("txt_CodProdComposicaoIns", "input", tr) != null)
                    codInternoProd = FindControl("txt_CodProdComposicaoIns", "input", tr).value;
                else
                    codInternoProd = FindControl("hdf_CodProdComposicaoIns", "input", tr).value;
                
                codAplicacaoAtual = FindControl("txt_AplComposicaoIns", "input", tr).value;
            }
            else
            {
                FindControl("txt_AmbProcComposicaoIns", "input", tr).value = codInterno;
                FindControl("hdf_AmbIdProcessoComposicao", "input", tr).value = idProcesso;
            
                codInternoProd = FindControl("txt_CodAmbComposicao", "input", tr).value;
                codAplicacaoAtual = FindControl("txt_AmbAplComposicaoIns", "input", tr).value;
            }
        
            if (((codAplicacao && codAplicacao != "") ||
                (codInternoProd != "" && CadPedido.ProdutoPossuiAplPadrao(codInternoProd).value == "false")) &&
                (codAplicacaoAtual == null || codAplicacaoAtual == ""))
            {
                aplAmbienteComposicao = procAmbienteComposicao;
                loadAplComposicao(tr, codAplicacao);
            }
        
            procAmbienteComposicao = false;
        }
    }

    function loadProcComposicao(control, codInterno) {

        var tr = buscaTable(control);

        var idProdPed = FindControl("hdf_IdProdPed", "input", tr).value;

        if (codInterno == "") {
            setProcComposicao("", "", "", idProdPed);
            return false;
        }

        try {
            var response = MetodosAjax.GetEtiqProcesso(codInterno).value;

            if (response == null || response == "") {
                alert("Falha ao buscar Processo. Ajax Error.");
                setProcComposicao("", "", "", idProdPed);
                return false
            }

            response = response.split("\t");
            
            if (response[0] == "Erro") {
                alert(response[1]);
                setProcComposicao("", "", "", idProdPed);
                return false;
            }

            setProcComposicao(response[1], response[2], response[3], idProdPed);
        }
        catch (err) {
            alert(err);
        }
    }

    function setValorTotalComposicao(valor, custo, idProdPed) {

        if (getNomeControleBenefComposicao() != null) {
            if (exibirControleBenef(getNomeControleBenefComposicao())) {
                var tr = FindControl("prodPed_" + idProdPed, "tr");
                var lblValorBenef = FindControl("lbl_ValorBenefComposicao", "span", tr);
                lblValorBenef.innerHTML = "R$ " + valor.toFixed(2).replace('.', ',');
            }
        }
    }

    function selProcComposicao(control) {

        var tr = buscaTable(control);

        var idProdPed = FindControl("hdf_IdProdPed", "input", tr).value;
        var idSubgrupo = MetodosAjax.GetSubgrupoProdByProd(FindControl("hdf_IdProdComposicao", "input", tr).value);

        openWindow(450, 700, '../Utils/SelEtiquetaProcesso.aspx?idProdPed=' + idProdPed+'&idSubgrupo=' + idSubgrupo.value);

        return false;
    }

    function selAplComposicao(control) {

        var tr = buscaTable(control);

        var idProdPed = FindControl("hdf_IdProdPed", "input", tr).value;

        openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx?idProdPed=' + idProdPed);

        return false;
    }

    function obrigarProcAplComposicao(control)
    {
        var table = buscaTable(control);

        var isObrigarProcApl = <%= Glass.Configuracoes.PedidoConfig.DadosPedido.ObrigarProcAplVidros.ToString().ToLower() %>;
        var isVidroBenef = getNomeControleBenef() != null ? exibirControleBenef(getNomeControleBenef()) && dadosProduto.Grupo == 1 : false;
        var isVidroRoteiro = dadosProduto.Grupo == 1 && <%= UtilizarRoteiroProducao().ToString().ToLower() %>;

            debugger;
        if (dadosProduto.IsChapaVidro)
            return true;

        if (isVidroRoteiro || (isObrigarProcApl && isVidroBenef))
        {
            if (FindControl("txt_AplComposicaoIns", "input", table) != null && FindControl("txt_AplComposicaoIns", "input", table).value == "")
            {
                if (isVidroRoteiro && !isObrigarProcApl) {
                    alert("É obrigatório informar a aplicação caso algum setor seja to tipo 'Por Roteiro' ou 'Por Benef.'.");
                    return false;
                }

                alert("Informe a aplicação.");
                return false;
            }
            
            if (FindControl("txt_ProcComposicaoIns", "input", table) != null && FindControl("txt_ProcComposicaoIns", "input", table).value == "")
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

    // Função chamada quando o produto está para ser atualizado
    function onUpdateProdComposicao(control, idTbConfigVidro) {
        
        if (!obrigarProcAplComposicao(table))
            return false;
                    
        return true;
    }

    function exibirProdsComposicaoChild(botao, idProdPed) {

        var grdProds = FindControl("grdProdutosComposicao", "table");

        if(grdProds == null)
            return;

        var linha = document.getElementById("prodPedChild_" + idProdPed);
        var exibir = linha.style.display == "none";
        linha.style.display = exibir ? "" : "none";
        botao.src = botao.src.replace(exibir ? "mais" : "menos", exibir ? "menos" : "mais");
        botao.title = (exibir ? "Esconder" : "Exibir") + " Produtos da Composição";
    }

</script>

<asp:GridView GridLines="None" ID="grdProdutosComposicao" runat="server" AllowPaging="True"
    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXPed" CssClass="gridStyle"
    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
    DataKeyNames="IdProdPed" 
    OnRowCommand="grdProdutos_RowCommand" OnPreRender="grdProdutos_PreRender" PageSize="12"
    OnRowUpdated="grdProdutos_RowUpdated">
    <FooterStyle Wrap="True" />
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:ImageButton ID="lnk_EditComposicao" runat="server" CommandName="Edit" Visible='<%# Eval("AlterarProcessoAplicacaoVisible") %>'
                     OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>' ImageUrl="~/Images/Edit.gif" />                
            </ItemTemplate>
            <EditItemTemplate>
                <asp:HiddenField ID="hdf_CodProdComposicaoIns" runat="server" Value='<%# Eval("CodInterno") %>'></asp:HiddenField>
                <asp:ImageButton ID="imb_AtualizarComposicao" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" 
                    OnClientClick='<%# "if (!onUpdateProdComposicao(this, &#39;" + IdProdPed + "_" + Eval("IdProdPed") + "&#39;)) return false;" %>' />
                <asp:ImageButton ID="imb_CancelarComposicao" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />

                <asp:HiddenField ID="hdf_ProdPedComposicao" runat="server" Value='<%# Eval("IdProdPed") %>' />
                <asp:HiddenField ID="hdf_IdProdComposicao" runat="server" Value='<%# Bind("IdProd") %>' />

            </EditItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
            <ItemTemplate>
                <asp:Label ID="lbl_CodProdComposicao" runat="server" Text='<%# Eval("CodInterno") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Produto" SortExpression="DescrProduto">
            <ItemTemplate>
                <asp:Label ID="lbl_ProdutoComposicao" runat="server" Text='<%# Eval("DescricaoProdutoComBenef") + (!string.IsNullOrEmpty(Eval("DescrBeneficiamentos").ToString()) ? " " + Eval("DescrBeneficiamentos") : "") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
            <ItemTemplate>
                <asp:Label ID="lbl_QtdeComposicao" runat="server" Text='<%# Eval("Qtde") %>'></asp:Label>
                <asp:Label ID="lbl_QtdeAmbienteComposicao" runat="server" ></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
            <ItemTemplate>
                <asp:Label ID="lbl_larguraComposicao" runat="server" Text='<%# Eval("Largura") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
            <ItemTemplate>
                <asp:Label ID="lbl_alturaComposicao" runat="server" Text='<%# Eval("AlturaLista") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m²" SortExpression="TotM">
            <ItemTemplate>
                <asp:Label ID="lbl_totMComposicao" runat="server" Text='<%# Eval("TotM") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tot. m² calc." SortExpression="TotM2Calc">
            <ItemTemplate>
                <asp:Label ID="lbl_Totm2CalcComposicao" runat="server" Text='<%# Eval("TotM2Calc") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Wrap="True" />
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Valor Vendido" SortExpression="ValorVendido">
            <ItemTemplate>
                <asp:Label ID="lbl_valorVendidoComposicao" runat="server" Text='<%# Eval("ValorVendido", "{0:C}") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
            <EditItemTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txt_ProcComposicaoIns" runat="server" onblur="procAmbienteComposicao=false; loadProcComposicao(this, this.value);"
                                onkeydown="if (isEnter(event)) { procAmbienteComposicao=false; loadProcComposicao(this, this.value); }"
                                onkeypress="return !(isEnter(event));" Width="30px" Text='<%# Eval("CodProcesso") %>'></asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="procAmbienteComposicao=false; return selProcComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdf_IdProcessoComposicao" runat="server" Value='<%# Bind("IdProcesso") %>' />
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lbl_CodProcessoComposicao" runat="server" Text='<%# Eval("CodProcesso") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
            <EditItemTemplate>
                <table class="pos">
                    <tr>
                        <td>
                            <asp:TextBox ID="txt_AplComposicaoIns" runat="server" onblur="aplAmbienteComposicao=false; loadAplComposicao(this, this.value);"
                                onkeydown="if (isEnter(event)) { aplAmbienteComposicao=false; loadAplComposicao(this, this.value); }" onkeypress="return !(isEnter(event));"
                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                        </td>
                        <td>
                            <input type="image" onclick="aplAmbienteComposicao=false; return selAplComposicao(this);" src="../Images/Pesquisar.gif" />
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdf_IdAplicacaoComposicao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
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
            <ItemTemplate>
            </ItemTemplate>
            <EditItemTemplate></EditItemTemplate>
            <FooterTemplate></FooterTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:TemplateField>
            <EditItemTemplate>
                <table id="tb_ConfigVidroComposicao_<%# Eval("IdProdPedParent") + "_" + Eval("IdProdPed") %>" cellspacing="0" style="display: none;">
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txt_EspessuraComposicao" runat="server" OnDataBinding="txt_EspessuraComposicao_DataBinding"
                                            onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Eval("Espessura") %>'></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrl_BenefEditarComposicao" runat="server" Beneficiamentos='<%# Eval("Beneficiamentos") %>'
                                ValidationGroup="produtoComposicao" OnInit="ctrl_Benef_Load" Redondo='<%# Eval("Redondo") %>'
                                CallbackCalculoValorTotal="setValorTotalComposicao" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left"></td>
                    </tr>
                </table>

                <script type="text/javascript">
                    <%# "calculaTamanhoMaximoComposicao(" + Request["idPedido"] != null ? Request["idPedido"] : "0" + ");"  %>
                </script>

            </EditItemTemplate>            
            <ItemTemplate>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <div id='<%# "imgProdsComposto_" + Eval("IdProdPed") %>'>
                    <asp:ImageButton ID="imgProdsComposto" runat="server" ImageUrl="~/Images/box.png" ToolTip="Exibir Produtos da Composição"
                        Visible='<%# Eval("IsProdLamComposicao") %>' OnClientClick='<%# "exibirProdsComposicaoChild(this, " + Eval("IdProdPed") + "); return false"%>' />                     
                </div>
            </ItemTemplate>
            <EditItemTemplate></EditItemTemplate>            
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <tr id="prodPedChild_<%# Eval("IdProdPed") %>" style="display: none" align="center">
                    <td colspan="17">
                        <br />
                        <uc1:ctrlProdComposicaoChild runat="server" id="ctrlProdCompChild" visible='<%# Eval("IsProdLamComposicao") %>'
                            idprodped='<%# Glass.Conversoes.StrParaUint(Eval("IdProdPed").ToString()) %>' />
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

<asp:HiddenField runat="server" ID="hdf_IdProdPed" />
<asp:HiddenField ID="hdf_IdProdComposicao" runat="server" />

<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdXPed" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedido"
    EnablePaging="True" MaximumRowsParameterName="pageSize"
    SelectCountMethod="GetCount" SelectMethod="GetList"
    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
    UpdateMethod="UpdateProcessoAplicacao" OnUpdated="odsProdXPed_Updated">
    <SelectParameters>
        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        <asp:ControlParameter ControlID="hdfIdAmbiente" Name="idAmbientePedido" PropertyName="Value" Type="UInt32" />
        <asp:Parameter Name="prodComposicao" DefaultValue="true" />
        <asp:ControlParameter Name="idProdPedParent" ControlID="hdf_IdProdPed" PropertyName="Value" />
    </SelectParameters>
</colo:VirtualObjectDataSource>

</script>
