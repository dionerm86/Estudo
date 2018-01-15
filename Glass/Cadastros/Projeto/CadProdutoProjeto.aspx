<%@ Page Title="Produtos de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadProdutoProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadProdutoProjeto" %>

<%@ Register src="../../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">
        var idProdProj = 0;
        var txtIdProdCurrent;

        function setProduto(codInterno) {
            if (idProdProj > 0) {
                var response = CadProdutoProjeto.Associar(idProdProj, codInterno).value;

                if (response == null) {
                    alert("Falha ao associar produto. AJAX Error.");
                    return false;
                }

                response = response.split('\t');
                alert(response[1]);

                if (response[0] != "Erro")
                    redirectUrl("../Cadastros/Projeto/CadProdutoProjeto.aspx");
            }

            txtIdProdCurrent.value = codInterno;
            loadProduto(txtIdProdCurrent);
        }

        // Carrega dados do produto com base no código do produto passado
        function loadProduto(txtIdProd) {
            if (txtIdProd.value == "")
                return false;

            try {
                var retorno = CadProdutoProjeto.GetProduto(txtIdProd.value).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    txtIdProd.value = "";
                    return false;
                }

                // Pega a terminação do id do txt que possui o codInterno do produto, para referenciar o hdf e o label
                var terminacaoTxtIdProd = txtIdProd.id.toString().substring(txtIdProd.id.toString().indexOf('_'));

                FindControl("hdfIdProd" + terminacaoTxtIdProd, "input").value = retorno[1];
                FindControl("lblDescrProd" + terminacaoTxtIdProd, "span").innerHTML = retorno[2];

                if (botaoTooltip != null)
                    eval(botaoTooltip.getAttribute("onclick").replace("this", "botaoTooltip").replace(";", ";//"));
            }
            catch (err) {
                alert(err);
            }
        }

        // Aplica configurações das cores com os produtos
        function aplicarConfig(btnAplicar) {
            if (!confirm("Aplicar configurações?"))
                return false;

            // Pega o id do produtoProjeto sendo configurado
            var idProdProj = btnAplicar.id.toString().substring(btnAplicar.id.toString().indexOf('_') + 1);

            // Nome inicial dos hiddenfiels utilizados na configuração deste produto de projeto
            var nomeInicial = "hdfIdProd_" + idProdProj + "_";

            var listaIdCor = ""; // Utilizada para salvar os ids das cores sendo configuradas
            var listaIdProd = ""; // Utilizada para salvar os ids do produtos configurados nas cores

            // Busca os hiddenfiels utilizados na configuração deste produto de projeto
            var listaControles = document.getElementsByTagName("input");
            for (var i = 0; i < listaControles.length; i++)
                if (listaControles[i].id.indexOf(nomeInicial) != -1) {
                listaIdCor += listaControles[i].id.substring(listaControles[i].id.lastIndexOf('_') + 1) + ",";
                listaIdProd += listaControles[i].value + ",";
            }

            var retorno = CadProdutoProjeto.AplicarConfig(idProdProj, listaIdCor, listaIdProd).value.split('\t');

            alert(retorno[1]);
            if (retorno[0] == "Erro")
                return false;
            
            // Fecha o tooltip de associação de produtos
            tt_HideInit();

            return false;
        }

        function openRpt() {
            var codInterno = FindControl("txtCodProd", "input").value;
            var descricao = FindControl("txtDescr", "input").value;
            var codInternoAssoc = FindControl("txtCodInternoAssoc", "input").value;
            var descricaoAssoc = FindControl("txtDescricaoAssoc", "input").value;
            var tipo = FindControl("drpTipo", "select").value;

            openWindow(600, 800, "../../Relatorios/RelBase.aspx?Rel=ProdutoProjeto&codInterno=" + codInterno + "&descricao=" + descricao + 
                "&codInternoAssoc=" + codInternoAssoc + "&descricaoAssoc=" + descricaoAssoc + "&tipo=" + tipo);

            return false;
        }

        var botaoTooltip = null;

        function openTooltip(id, titulo, botao) {
            botaoTooltip = botao;
            TagToTip("tbConfigProd_" + id, FADEIN, 300, COPYCONTENT, false, TITLE, titulo, CLOSEBTN, true, CLOSEBTNTEXT, "Fechar", CLOSEBTNCOLORS, ["#cc0000", "#ffffff", "#D3E3F6", "#0000cc"], STICKY, true, FIX, [botao, 10, 0]);
        }

        function desassociarProduto(idProdProjConfig) {
            if (idProdProjConfig == 0) {
                alert("Essa cor não tem produto associado!");
                return false;
            }

            if (!confirm('Tem certeza que deseja desvincular o produto dessa cor?'))
                return false;

            var retorno = CadProdutoProjeto.DesassociarProduto(idProdProjConfig).value.split(';');
            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                return false;
            }

            alert(retorno[1]);
            redirectUrl('<%= ResolveUrl("~/Cadastros/Projeto/CadProdutoProjeto.aspx") %>');
        }

    </script>

    <table align="center" style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;<asp:Label ID="Label4" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Tipo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipo" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpTipo_SelectedIndexChanged">
                                <asp:ListItem Value="0">Todos</asp:ListItem>
                                <asp:ListItem Value="1">Alumínio</asp:ListItem>
                                <asp:ListItem Value="2">Ferragem</asp:ListItem>
                                <asp:ListItem Value="4">Vidro</asp:ListItem>
                                <asp:ListItem Value="3">Outros</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>                    
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Cód. Prod. Associado" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodInternoAssoc" runat="server" Width="60px" onblur="setProduto();" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;<asp:Label ID="Label6" runat="server" Text="Descr. Prod. Associado" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricaoAssoc" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdProj" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdProdProj" DataSourceID="odsProdProj"
                    EmptyDataText="Nenhum produto encontrado." PageSize="22" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    OnDataBound="grdProdProj_DataBound" OnRowCommand="grdProdProj_RowCommand" ShowFooter="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Visible='<%# !(bool)Eval("ProdutoPadrao") %>'>
                                    <img border="0" src="../../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" Visible='<%# !(bool)Eval("ProdutoPadrao") %>' />
                                <asp:PlaceHolder ID="pchConfig" runat="server">
                                    <a href="#" onclick='openTooltip(<%# Eval("IdProdProj") %>, "<%# Eval("CodInterno") + " " + Eval("Descricao").ToString().Replace("\"", "") %>", this); return false;'>
                                        <img src="../../Images/gear.gif" border="0" title="Configurar Produtos" /></a>
                                </asp:PlaceHolder>
                                <table id='tbConfigProd_<%# Eval("IdProdProj") %>' style="display: none;">
                                    <tr align="left">
                                        <td align="center">
                                            <asp:GridView ID="grdConfigProd" runat="server" AutoGenerateColumns="False" DataSourceID="odsConfigProd"
                                                CellPadding="4" ForeColor="#333333" GridLines="None">
                                                <RowStyle BackColor="#EFF3FB" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Cor">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCorProd" runat="server" Text='<%# Eval("DescrCorProduto") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemStyle Wrap="false" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Produto">
                                                        <ItemTemplate>
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <input id='txtIdProd_<%# Eval("IdProdProj") %>_<%# Eval("IdCorProduto") %>' onblur="loadProduto(this);"
                                                                            style="width: 50px" onkeydown="if (isEnter(event)) loadProduto(this);" onkeypress="return !(isEnter(event));"
                                                                            type="text" />
                                                                        <input id='hdfIdProd_<%# Eval("IdProdProj") %>_<%# Eval("IdCorProduto") %>' type="hidden" />
                                                                    </td>
                                                                    <td>
                                                                        <a href="#" onclick='txtIdProdCurrent=FindControl("txtIdProd_<%# Eval("IdProdProj") %>_<%# Eval("IdCorProduto") %>", "input"); idProdProj=0; 
                                                                            openWindow(450, 700, &#039;../../Utils/SelProd.aspx&#039;); return false;'>
                                                                            <img border="0" src="../../Images/Pesquisar.gif" /></a>
                                                                    </td>
                                                                    <td>
                                                                        <asp:ImageButton ID="imbDesassociarProduto" runat="server" ToolTip="Desassociar Produto" ImageUrl="~/Images/gear_delete.png"
                                                                            OnClientClick='<%# "return desassociarProduto(" + Eval("IdProdProjConfig") + ")" %>' />
                                                                    </td>
                                                                    <td nowrap="nowrap">
                                                                        <span id="lblDescrProd_<%# Eval("IdProdProj") %>_<%# Eval("IdCorProduto") %>">
                                                                            <%# Eval("CodDescrProd") %></span>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                        <ItemTemplate>
                                                            <uc1:ctrlLogPopup ID="ctrlLogPopup2" runat="server" 
                                                                IdRegistro='<%# Eval("IdProdProjConfig") %>' 
                                                                Tabela="ProdutoProjetoConfig" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                                <EditRowStyle BackColor="#2461BF" />
                                                <AlternatingRowStyle BackColor="White" />
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <input id='btnAplicar_<%# Eval("IdProdProj") %>' value="Aplicar" type="submit" onclick="aplicarConfig(this);" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:ImageButton ID="imbDesvincular" runat="server" CommandName="Desvincular" CommandArgument='<%# Eval("IdProdProj") %>'
                                    ImageUrl="~/Images/gear_delete.png" ToolTip="Desvincular Produtos" OnClientClick="return confirm('Tem certeza que deseja desvincular os itens associados à este produto?');" />
                                <asp:HiddenField ID="hdfIdProdProj" runat="server" Value='<%# Eval("IdProdProj") %>' />
                                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsConfigProd" runat="server" SelectMethod="GetByProdProj"
                                    TypeName="Glass.Data.DAL.ProdutoProjetoConfigDAO">
                                    <SelectParameters>
                                        <asp:Parameter Name="idProdProj" Type="UInt32" />
                                    </SelectParameters>
                                </colo:VirtualObjectDataSource>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" SortExpression="CodInterno">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodInterno" runat="server" MaxLength="15" Text='<%# Bind("CodInterno") %>'
                                    Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodInterno" runat="server" MaxLength="15" Width="70px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="50" Text='<%# Bind("Descricao") %>'
                                    Width="250px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="50" Width="250px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="Tipo">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" SelectedValue='<%# Bind("Tipo") %>'>
                                    <asp:ListItem Value="1">Alumínio</asp:ListItem>
                                    <asp:ListItem Value="2">Ferragem</asp:ListItem>
                                    <asp:ListItem Value="4">Vidro</asp:ListItem>
                                    <asp:ListItem Value="3">Outros</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server">
                                    <asp:ListItem Value="1">Alumínio</asp:ListItem>
                                    <asp:ListItem Value="2">Ferragem</asp:ListItem>
                                    <asp:ListItem Value="4">Vidro</asp:ListItem>
                                    <asp:ListItem Value="3">Outros</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescrTipo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="lnkInsProd" runat="server" ImageUrl="~/Images/insert.gif" OnClick="lnkInsProd_Click"
                                    Style="width: 16px" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# Eval("IdProdProj") %>' Tabela="ProdutoProjeto" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdProj" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoProjetoDAO"
                    DataObjectTypeName="Glass.Data.Model.ProdutoProjeto" DeleteMethod="Delete" OnDeleted="odsProdProj_Deleted"
                    OnUpdated="odsProdProj_Updated" UpdateMethod="Update">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descricao" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtCodInternoAssoc" Name="codInternoAssoc" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDescricaoAssoc" Name="descricaoAssoc" 
                            PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpTipo" Name="tipo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();"> <img alt="" border="0" src="../../Images/printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
