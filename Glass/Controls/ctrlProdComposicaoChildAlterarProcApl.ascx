<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlProdComposicaoChildAlterarProcApl.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlProdComposicaoChildAlterarProcApl" %>

<%@ Register Src="ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc4" %>

<script type="text/javascript">
        
    var aplAmbienteComposicaoChild = false;
    var procAmbienteComposicaoChild = false;

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

    function calculaTamanhoMaximoComposicaoChild(control)
    {
        if (FindControl("hdf_CodProdComposicaoIns", "input") == null)
            return;

        var table = buscaTableChild(control);
            
        var idPedido = <%= Request["idPedido"] != null ? Request["idPedido"] : "0" %>;
        var codInterno = FindControl("hdf_CodProdComposicaoIns", "input", table).innerHTML;
        var totM2 = FindControl("llbChild_TotM2ComposicaoIns", "span", table).innerHTML;
        var idProdPed = FindControl("hdfChild_ProdPedComposicao", "input", table) != null ? FindControl("hdfChild_ProdPedComposicao", "input", table).value : 0;
        
        var tamanhoMaximo = CadPedido.GetTamanhoMaximoProduto(idPedido, codInterno, totM2, idProdPed).value.split(";");
        tamanhoMaximo = tamanhoMaximo[0] == "Ok" ? parseFloat(tamanhoMaximo[1].replace(",", ".")) : 0;
        
        FindControl("hdfChild_TamanhoMaximoObraComposicao", "input", table).value = tamanhoMaximo;
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

        if(idSubgrupo.value != "" && retornoValidacao.value == "false" && (FindControl("txtChild_ProcComposicaoIns", "input", tr) != null && FindControl("txtChild_ProcComposicaoIns", "input", tr).value != ""))
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
                codInternoProd = FindControl("hdf_CodProdComposicaoIns", "input", tr).innerHTML;
                
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

    // Função chamada quando o produto está para ser atualizado
    function onUpdateProdComposicaoChild(control, idTbConfigVidro) {
                
        if (!obrigarProcAplComposicaoChild(table))
            return false;
            
        return true;
    }

</script>

<asp:GridView GridLines="None" ID="grdProdutosComposicao" runat="server" AllowPaging="True"
    AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsProdXPed" CssClass="gridStyle"
    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
    DataKeyNames="IdProdPed" 
    OnRowCommand="grdProdutos_RowCommand" OnPreRender="grdProdutos_PreRender" PageSize="12"
    OnRowUpdated="grdProdutos_RowUpdated">    
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:ImageButton ID="lnk_EditComposicao" runat="server" CommandName="Edit" Visible='<%# Eval("AlterarProcessoAplicacaoVisible") %>'
                     OnClientClick='<%# !(bool)Eval("PodeEditar") ? "mensagemProdutoComDesconto(true); return false" : "" %>'
                    ImageUrl="~/Images/Edit.gif" />                
            </ItemTemplate>
            <EditItemTemplate>
                <asp:ImageButton ID="imb_AtualizarComposicao" runat="server" CommandName="Update" Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar"
                    OnClientClick='<%# "if (!onUpdateProdComposicaoChild(this, &#39;" + IdProdPed + "_" + Eval("IdProdPed") + "&#39;)) return false;" %>' />
                <asp:ImageButton ID="imb_CancelarComposicao" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />

                <asp:HiddenField ID="hdfChild_ProdPedComposicao" runat="server" Value='<%# Bind("IdProdPed") %>' />
                <asp:HiddenField ID="hdfChild_IdProdComposicao" runat="server" Value='<%# Eval("IdProd") %>' />
                <asp:HiddenField ID="hdf_CodProdComposicaoIns" runat="server" Value='<%# Eval("CodInterno") %>'></asp:HiddenField>
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
                <asp:Label ID="lbl_QtdeAmbienteComposicao" runat="server"></asp:Label>
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
            <ItemTemplate>
                <asp:Label ID="lbl_CodProcessoComposicao" runat="server" Text='<%# Eval("CodProcesso") %>'></asp:Label>
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
                <table id="tb_ConfigVidroComposicao_<%# Eval("IdProdPedParent") + "_" + Eval("IdProdPed") %>" cellspacing="0" style="display: none;">
                    <tr align="left">
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="dtvFieldBold">Espessura
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtChild_EspessuraComposicao" runat="server" OnDataBinding="txtChild_EspessuraComposicao_DataBinding"
                                            onkeypress="return soNumeros(event, false, true);" Width="30px" Text='<%# Eval("Espessura") %>'></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <uc4:ctrlBenef ID="ctrlChild_BenefEditarComposicao" runat="server" Beneficiamentos='<%# Eval("Beneficiamentos") %>'
                                ValidationGroup="produtoComposicaoChild" OnInit="ctrl_Benef_Load" Redondo='<%# Eval("Redondo") %>'
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
            <ItemTemplate>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <PagerStyle CssClass="pgr"></PagerStyle>
    <EditRowStyle CssClass="edit"></EditRowStyle>
    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
</asp:GridView>

<asp:HiddenField runat="server" ID="hdfChild_IdProdPed" />
<asp:HiddenField ID="hdfChild_IdProdComposicao" runat="server" />

<colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdXPed" runat="server" DataObjectTypeName="Glass.Data.Model.ProdutosPedido"
    EnablePaging="True" MaximumRowsParameterName="pageSize"
    SelectCountMethod="GetCount" SelectMethod="GetList"
    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
    UpdateMethod="UpdateProcessoAplicacao" OnUpdated="odsProdXPed_Updated">
    <SelectParameters>
        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
        <asp:ControlParameter ControlID="hdfIdAmbiente" Name="idAmbientePedido" PropertyName="Value" Type="UInt32" />
        <asp:Parameter Name="prodComposicao" DefaultValue="true" />
        <asp:ControlParameter Name="idProdPedParent" ControlID="hdfChild_IdProdPed" PropertyName="Value" />
    </SelectParameters>
</colo:VirtualObjectDataSource>