<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" Title="Estoque Mínimo"
    CodeBehind="ListaEstoqueMinimo.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaEstoqueMinimo" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlSelCorProd.ascx" tagname="ctrlSelCorProd" tagprefix="uc2" %>
<%@ Register src="../Controls/ctrlTextoTooltip.ascx" tagname="ctrlTextoTooltip" tagprefix="uc3" %>
<%@ Register Src="../Controls/ctrlLoja.ascx" TagName="ctrlLoja" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

    function openRptEstoque(idProduto, tipo)
    {
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=EstoqueProdutos&TipoColunas=" + tipo + "&idProd=" + idProduto + "&agrupar=false");
    }

    function abrirReserva(idProduto)
    {
        openRptEstoque(idProduto, 1);
    }

    function abrirLiberacao(idProduto)
    {
        openRptEstoque(idProduto, 2);
    }

    function openRpt() {
        var cod = FindControl("txtCodProd", "input").value;
        var descr = FindControl("txtDescr", "input").value;
        var idLoja = FindControl("drpLoja", "select").value;
        var idGrupo = FindControl("drpGrupo", "select").value;
        var idSubgrupo = FindControl("drpSubgrupo", "select").value;
        var abaixoEstMin = FindControl("chkAbaixoMinimo", "input").checked;
        var controleCor = <%= ctrlSelCorProd1.ClientID %>;
        var tipoBox = FindControl("drpTipoBox", "select");
        tipoBox = tipoBox != null ? tipoBox.value : "";
        
        openWindow(600, 800, "RelBase.aspx?Rel=EstoqueMinimo&codInterno=" + cod + "&descr=" + descr + "&idGrupo=" + idGrupo +
            "&idLoja=" + idLoja + "&idSubgrupo=" + idSubgrupo + "&abaixoEstMin=" + abaixoEstMin + "&idCorVidro=" + controleCor.idCorVidro() +
            "&idCorFerragem=" + controleCor.idCorFerragem() + "&idCorAluminio=" + controleCor.idCorAluminio() + "&tipoBox=" + tipoBox);

        return false;
    }
    
    function lancarEstMin()
    {
        var idLoja = FindControl("drpLoja", "select").value;
        var idGrupoProd = FindControl("drpGrupo", "select").value;
        var idSubgrupoProd = FindControl("drpSubgrupo", "select").value;
        
        openWindow(200, 500, "../Utils/LancarEstoqueMinimo.aspx?idLoja=" + idLoja + "&idGrupoProd=" + idGrupoProd +
            (idSubgrupoProd != "0" ? "&idSubgrupoProd=" + idSubgrupoProd : ""));
    }

    // Carrega dados do produto com base no código do produto passado
    function setProduto() {
        var codInterno = FindControl("txtCodProd", "input").value;

        if (codInterno == "")
            return false;

        try {
            var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                FindControl("txtCodProd", "input").value = "";
                return false;
            }
        }
        catch (err) {
            alert(err.value);
        }
    }
    
    function reajuste() {
    
        var cod = FindControl("txtCodProd", "input").value;
        var descricao = FindControl("txtDescr", "input").value;
        var idLoja = FindControl("drpLoja", "select").value;
        var idGrupo = FindControl("drpGrupo", "select").value;
        var idSubgrupo = FindControl("drpSubgrupo", "select").value;
        var abaixoEstMin = FindControl("chkAbaixoMinimo", "input").checked;
        var controleCor = <%= ctrlSelCorProd1.ClientID %>;
        var tipoBox = FindControl("drpTipoBox", "select");
        tipoBox = tipoBox != null ? tipoBox.value : "";

        openWindow(500, 500, "../Utils/ReajusteEstoqueMinimo.aspx?codInterno=" + cod + "&descricao=" + descricao + "&idGrupo=" + idGrupo +
            "&idLoja=" + idLoja + "&idSubgrupo=" + idSubgrupo + "&abaixoEstMin=" + abaixoEstMin + "&idCorVidro=" + controleCor.idCorVidro() +
            "&idCorFerragem=" + controleCor.idCorFerragem() + "&idCorAluminio=" + controleCor.idCorAluminio() + "&tipoBox=" + tipoBox);
            
        //qdo ficar pronto, return true pra atualizar o grid.
        return false;
    }
    
    </script>
    

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc4:ctrlLoja runat="server" ID="drpLoja" SomenteAtivas="true" AutoPostBack="True" MostrarTodas="false"/>
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                Style="height: 16px" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label2" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" AutoPostBack="True" DataSourceID="odsGrupo"
                                DataTextField="Descricao" DataValueField="IdGrupoProd">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label1" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd">
                            </asp:DropDownList>
                            <asp:DropDownList ID="drpTipoBox" runat="server" AutoPostBack="True">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem>Fixo</asp:ListItem>
                                <asp:ListItem>Móvel</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq" runat="server" OnClick="lnkPesq_Click"><img border="0\" 
                                src="../images/pesquisar.gif"></img></asp:LinkButton>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label9" runat="server" Text="Cor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <uc2:ctrlSelCorProd ID="ctrlSelCorProd1" runat="server" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkPesq_Click"><img border="0\" 
                                src="../images/pesquisar.gif"></img></asp:LinkButton>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAbaixoMinimo" runat="server" Checked="True" 
                                Text="Apenas produtos abaixo do estoque mínimo" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center" style="height: 29px">
                <table>
                <tr>
                    <td></td>
                </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsProdutos" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" AllowPaging="True"
                    PageSize="20" DataKeyNames="IdLoja,IdProd" AllowSorting="True">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" CommandName="Edit" 
                                    ImageUrl="~/Images/EditarGrid.gif" Visible='<%# Eval("EditVisible") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInternoProd">
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("CodInternoProd") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("CodInternoProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="DescrProduto">
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescrProduto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="DescrGrupoSubgrupoProd">
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("DescrGrupoSubgrupoProd") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescrGrupoSubgrupoProd") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Estoque Mínimo" SortExpression="EstoqueMinimo">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEstMinimo" runat="server" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Bind("EstMinimo") %>' Width="70px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("EstoqueMinimoString") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtd. Estoque" SortExpression="DescrEstoque">
                            <EditItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# (Eval("QtdEstoque") ?? "").ToString() + (Eval("DescrEstoque") ?? "").ToString() %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# (Eval("QtdEstoqueStringLabel") ?? "").ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Reserva" SortExpression="Reserva">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkReserva" runat="server" 
                                    OnClientClick='<%# "abrirReserva(" + Eval("IdProd") + "); return false" %>' 
                                    Text='<%# Eval("ReservaString") %>'></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label61" runat="server" Text='<%# Bind("Reserva") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Liberação" SortExpression="Liberacao">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkLiberacao" runat="server" 
                                    OnClientClick='<%# "abrirLiberacao(" + Eval("IdProd") + "); return false" %>' 
                                    Text='<%# Eval("LiberacaoString") %>'></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label71" runat="server" Text='<%# Bind("Liberacao") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Disponível" SortExpression="EstoqueDisponivel">
                            <ItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Bind("EstoqueDisponivel") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label8" runat="server" Text='<%# Eval("EstoqueDisponivel") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Comprando / Produzindo" 
                            SortExpression="DescricaoComprando, DescricaoProduzindo">
                            <EditItemTemplate>
                                <asp:Label ID="Label73" runat="server" Text='<%# Eval("DescricaoComprando") %>'></asp:Label>
                                &nbsp;/
                                <asp:Label ID="Label80" runat="server" 
                                    Text='<%# Eval("DescricaoProduzindo") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton3" runat="server" 
                                    onclientclick='<%# "openWindow(400, 600, \"../Utils/DadosEstoque.aspx?compra=1&idProd=" + Eval("IdProd") + "&idLoja=" + Eval("IdLoja") + "\"); return false" %>' 
                                    Text='<%# Eval("DescricaoComprando") %>'></asp:LinkButton>
                                &nbsp;/
                                <asp:LinkButton ID="LinkButton2" runat="server" 
                                    onclientclick='<%# "openWindow(400, 600, \"../Utils/DadosEstoque.aspx?idProd=" + Eval("IdProd") + "\"); return false" %>' 
                                    Text='<%# Eval("DescricaoProduzindo") %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pendente para Compra / Produção" 
                            SortExpression="DescricaoPendenteCompra, DescricaoPendenteProducao">
                            <EditItemTemplate>
                                <asp:Label ID="Label79" runat="server" 
                                    Text='<%# Eval("DescricaoPendenteCompra") %>'></asp:Label>
                                &nbsp;/
                                <asp:Label ID="Label81" runat="server" 
                                    Text='<%# Eval("DescricaoPendenteProducao") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label79" runat="server" 
                                    Text='<%# Eval("DescricaoPendenteCompra") %>'></asp:Label>
                                &nbsp;/
                                <asp:Label ID="Label81" runat="server" 
                                    Text='<%# Eval("DescricaoPendenteProducao") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderTemplate>
                                <table class="pos" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            Pendente para Compra / Produção
                                        </td>
                                        <td>
                                            &nbsp;
                                            <uc3:ctrlTextoTooltip ID="ctrlTextoTooltip1" runat="server" 
                                                Texto="Compra: Estoque mínimo - Disponível - Comprando<br />Produção: Estoque mínimo - Disponível - Produzindo" />
                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# Eval("IdLog") %>' Tabela="ProdutoLoja" />
                            </ItemTemplate>
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
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt();"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:HyperLink ID="lnkSugestaoCompra" runat="server" 
                    NavigateUrl="~/Cadastros/CadSugestaoCompra.aspx" 
                    onprerender="lnkSugestaoCompra_PreRender">
                    <img src="../Images/Cart.png" border="0" />  Sugestão de compra</asp:HyperLink>
                &nbsp;&nbsp;&nbsp;
                <asp:HyperLink ID="lnkSugestaoProducao" runat="server" 
                    NavigateUrl="~/Cadastros/CadSugestaoCompra.aspx?producao=1" 
                    onprerender="lnkSugestaoCompra_PreRender">
                    <img src="../Images/Gear.gif" border="0" />  Sugestão de produção</asp:HyperLink>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkLancarEstoqueMin" runat="server" 
                    onclientclick="lancarEstMin(); return false">Lançar Estoque Mínimo</asp:LinkButton>
                    &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkReajuste" runat="server" 
                            onclientclick="return reajuste();">Reajuste de Estoque Mínimo</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutos" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetForEstoqueMinCount" SelectMethod="GetForEstoqueMin" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ProdutoLojaDAO" SortParameterName="sortExpression" DataObjectTypeName="Glass.Data.Model.ProdutoLoja"
                    UpdateMethod="AtualizaEstoqueMinimo">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpLoja" Name="idLoja" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInternoProd" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descricao" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupoProd" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupoProd" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="chkAbaixoMinimo" Name="abaixoEstMin" 
                            PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorVidro" 
                            PropertyName="IdCorVidro" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorFerragem" 
                            PropertyName="IdCorFerragem" Type="UInt32" />
                        <asp:ControlParameter ControlID="ctrlSelCorProd1" Name="idCorAluminio" 
                            PropertyName="IdCorAluminio" Type="UInt32" />
                        <asp:ControlParameter ControlID="drpTipoBox" Name="tipoBox" 
                            PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.LojaDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter" TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.SubgrupoProdDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
