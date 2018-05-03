<%@ Page Title="Cadastro de Matéria Prima"  Language="C#" AutoEventWireup="true" CodeBehind="SelProdMateriaPrima.aspx.cs"
     Inherits="Glass.UI.Web.Utils.SelProdMateriaPrima" MasterPageFile="~/Layout.master" %>

<%@ Register src="../Controls/ctrlSelProduto.ascx" tagname="ctrlSelProduto" tagprefix="uc1" %>
<%@ Register Src="../Controls/ctrlBenef.ascx" TagName="ctrlBenef" TagPrefix="uc2" %>
<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc4" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Pagina" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        // Função chamada pelo popup de escolha da Aplicação do produto
        function setApl(idAplicacao, codInterno, idControle) {

            if (idControle != undefined && idControle != "") {
                ctrlProdutoBaixaEst.setApl(idAplicacao, codInterno, idControle);
                return;
            }

            if (!aplAmbiente) {
                FindControl("txtAplIns", "input").value = codInterno;
                FindControl("hdfIdAplicacao", "input").value = idAplicacao;
            }
            else {
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
        function setProc(idProcesso, codInterno, codAplicacao, idControle) {

            if (idControle != undefined && idControle != "") {
                ctrlProdutoBaixaEst.setProc(idProcesso, codInterno, codAplicacao, idControle);
                return;
            }

            if (!procAmbiente) {
                FindControl("txtProcIns", "input").value = codInterno;
                FindControl("hdfIdProcesso", "input").value = idProcesso;
            }
            else {
                FindControl("txtAmbProcIns", "input").value = codInterno;
                FindControl("hdfAmbIdProcesso", "input").value = idProcesso;
            }

            if (codAplicacao != "") {
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

        function exibirBenef(botao) {
            for (iTip = 0; iTip < 2; iTip++) {
                // O controle de beneficiamentos deve ser exibido abaixo das peças, pois se for exibido acima e for muito grande,
                // esconde o botão aplicar
                TagToTip('tbConfigVidro', FADEIN, 300, COPYCONTENT, false, TITLE, 'Beneficiamento', CLOSEBTN, true,
                    CLOSEBTNTEXT, 'Aplicar', CLOSEBTNCOLORS, ['#cc0000', '#ffffff', '#D3E3F6', '#0000cc'], STICKY, true,
                    FIX, [botao, 9 - getTableWidth('tbConfigVidro'), 17]);
            }
        }

        function atualizarBeneficiamento(controle)
        {
            var hdfBenef = document.getElementById("<%= hdfBenef.ClientID %>");

            var nomeControleBenef = FindControl("ctrlBenef1", "table", controle.parentElement.parentElement).id;

            nomeControleBenef = nomeControleBenef.substr(0, nomeControleBenef.length-9)

            var benef = eval(nomeControleBenef).Servicos().Info;

            hdfBenef.value = benef;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdProdMateriaPrima" runat="server" SkinID="gridViewEditable" OnRowEditing="grdProdMateriaPrima_RowEditing"
                    DataSourceID="odsProdMateriaPrima" DataKeyNames="IdProdBaixaEst" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server"  OnClick="imbAtualizar_Click" OnClientClick="atualizarBeneficiamento(this);" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód. Produto" SortExpression="IdProd">
                            <EditItemTemplate>
                               <uc1:ctrlSelProduto runat="server" ID="ctrlSelProd" IdProdInt32='<%# Bind("IdProdBaixa") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlSelProduto runat="server" ID="ctrlSelProd" IdProdInt32='<%# Bind("IdProdBaixa") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("CodInternoProduto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde" SortExpression="Qtde">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" MaxLength="5" Text='<%# Bind("Qtde") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" MaxLength="5" Text='<%# Bind("Qtde") %>'
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblQtde" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Altura" SortExpression="Altura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" MaxLength="5" Text='<%# Bind("Altura") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAltura" runat="server" MaxLength="5" Text='<%# Bind("Altura") %>'
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblAltura" runat="server" Text='<%# Bind("Altura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Largura" SortExpression="Largura">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" MaxLength="5" Text='<%# Bind("Largura") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtLargura" runat="server" MaxLength="5" Text='<%# Bind("Largura") %>'
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblLargura" runat="server" Text='<%# Bind("Largura") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Forma" SortExpression="Forma">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtForma" runat="server" MaxLength="5" Text='<%# Bind("Forma") %>'
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtForma" runat="server" MaxLength="5" Text='<%# Bind("Forma") %>'
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblForma" runat="server" Text='<%# Bind("Forma") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>                        
                        <asp:TemplateField HeaderText="Proc." SortExpression="IdProcesso">
                            <EditItemTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="procAmbiente=false; loadProc(this.value);"
                                                onkeydown="if (isEnter(event)) { procAmbiente=false; loadProc(this.value); }"
                                                onkeypress="return !(isEnter(event));" Width="30px" 
                                                Text='<%# Eval("CodProcesso")%>'></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick='procAmbiente=false; openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                <asp:HiddenField ID="hdfIdProdBaixaEst" runat="server" Value='<%# Bind("IdProdBaixaEst") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtProcIns" runat="server" onblur="procAmbiente=false; loadProc(this.value);"
                                                onkeydown="if (isEnter(event)) { procAmbiente=false; loadProc(this.value); }"
                                                onkeypress="return !(isEnter(event));" Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick='openWindow(450, 700, &#039;../Utils/SelEtiquetaProcesso.aspx&#039;); return false;'>
                                                <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdProcesso" runat="server" Value='<%# Bind("IdProcesso") %>' />
                                <asp:HiddenField ID="hdfIdProdBaixaEst" runat="server" Value='<%# Bind("IdProdBaixaEst") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label10" runat="server" Text='<%# Eval("CodProcesso")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>                       
                        <asp:TemplateField HeaderText="Apl." SortExpression="IdAplicacao">
                            <EditItemTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="loadApl(this.value);"
                                                onkeydown="if (isEnter(event)) { aplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                Text='<%# Eval("CodAplicacao") %>' Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="aplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <table class="pos">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAplIns" runat="server" onblur="aplAmbiente=false; loadApl(this.value);"
                                                onkeydown="if (isEnter(event)) { aplAmbiente=false; loadApl(this.value); }" onkeypress="return !(isEnter(event));"
                                                Width="30px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a href="#" onclick="aplAmbiente=false; openWindow(450, 700, '../Utils/SelEtiquetaAplicacao.aspx'); return false;">
                                                <img border="0" src="../Images/Pesquisar.gif" /></a>
                                        </td>
                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdfIdAplicacao" runat="server" Value='<%# Bind("IdAplicacao") %>' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label9" runat="server" Text='<%# Eval("CodAplicacao")%>'></asp:Label>
                                <asp:HiddenField ID="hdfIdProdBaixaEst" runat="server" Value='<%# Bind("IdProdBaixaEst") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(this); return false;">
                                    <img border="0" src="../../Images/gear_add.gif" />
                                </asp:LinkButton>
                                <table id="tbConfigVidro" cellspacing="0" style="display: none;">
                                    <tr>
                                        <td>
                                            <uc2:ctrlBenef ID="ctrlBenef1" runat="server" OnLoad="ctrlBenef1_Load" Redondo="false"
                                                ValidationGroup="produto" 
                                                Beneficiamentos2='<%# Bind("ProdutoBaixaEstBeneficiamentos") %>' 
                                                TipoBeneficiamento="Glass.Global.Negocios.Entidades.ProdutoBaixaEstoqueBenef"
                                                CarregarBenefPadrao="false"
                                                CalcularBeneficiamentoPadrao="true" />
                                        </td>
                                    </tr>
                                </table>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkBenef" runat="server" OnClientClick="exibirBenef(this); return false;">
                                    <img border="0" src="../../Images/gear_add.gif" />
                                </asp:LinkButton>
                                <table id="tbConfigVidro" cellspacing="0" style="display: none;">
                                    <tr>
                                        <td>
                                            <uc2:ctrlBenef ID="ctrlBenef1" runat="server" OnLoad="ctrlBenef1_Load" Redondo="false"
                                                ValidationGroup="produto" 
                                                Beneficiamentos2='<%# Bind("ProdutoBaixaEstBeneficiamentos") %>' 
                                                TipoBeneficiamento="Glass.Global.Negocios.Entidades.ProdutoBaixaEstoqueBenef"
                                                CarregarBenefPadrao="false"
                                                CalcularBeneficiamentoPadrao="true" />
                                        </td>
                                    </tr>
                                </table>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Imagem" SortExpression="Imagem">
                            <FooterTemplate>
                                <asp:FileUpload ID="filImagem" runat="server"  accept="image/*"/>
                            </FooterTemplate>
                            <EditItemTemplate>
                                <asp:FileUpload ID="filImagem" runat="server" accept="image/*"/>
                                <uc4:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" ImageUrl='<%# Glass.Global.UI.Web.Process.ProdutoBaixaEstoqueRepositorioImagens.Instance.ObtemUrl((int)Eval("IdProdBaixaEst")) %>' />
                                <asp:ImageButton ID="imbExcluirImagem" runat="server" OnClick="imbExcluirImagem_Click" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir Imagem" Visible='<%# Eval("PossuiImagem") %>' />
                            </EditItemTemplate>
                            <ItemTemplate >
                                <uc4:ctrlImagemPopup ID="ctrlImagemPopup2" runat="server" ImageUrl='<%# Glass.Global.UI.Web.Process.ProdutoBaixaEstoqueRepositorioImagens.Instance.ObtemUrl((int)Eval("IdProdBaixaEst")) %>' />
                                <asp:ImageButton ID="ImageButton1" runat="server" OnClick="imbExcluirImagem_Click" ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir Imagem" Visible='<%# Eval("PossuiImagem") %>' />
                            </ItemTemplate>  
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click" OnClientClick="atualizarBeneficiamento(this);">
                                    <img border="0" src="../Images/insert.gif" alt="Inserir"/>
                                </asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfIdProd" runat="server" />
                <asp:HiddenField ID="hdfIdProdBaixaEst" runat="server"/>
                <asp:HiddenField ID="hdfBenef" runat="server"/>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdMateriaPrima" runat="server" 
                    EnablePaging="True"
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="ObterProdBaixaEst"
                    CreateDataObjectMethod="CriarProdutoBaixaEstoque"
                    SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.IProdutoFluxo" 
                    DataObjectTypeName="Glass.Estoque.Negocios.Entidades.ProdutoBaixaEstoque"
                    SelectByKeysMethod="ObterProdutoBaixaEstoque"
                    DeleteMethod="ApagarProdutoBaixaEstoque" DeleteStrategy="GetAndDelete">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idProd" QueryStringField="idProd" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
