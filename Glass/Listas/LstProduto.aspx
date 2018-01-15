<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstProduto.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstProduto" Title="Produtos" %>

<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

    function precoFornecedor(idProd)
    {
        openWindow(600, 800, "../Utils/PrecoFornecedor.aspx?idProd=" + idProd);
    }

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

    function abrirTelaExportar()
    {
        openWindow(400, 600, "../Utils/ExportarPrecoProduto.aspx");
    }

    function exportarPrecos()
    {
        openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=ProdutosPreco&" + obtemQueryString() + "&exportarExcel=true");
    }

    // Carrega dados do produto com base no código do produto passado
    function setProduto()
    {
        var codInterno = FindControl("txtCodProd", "input").value;

        if (codInterno == "")
            return false;

        try
        {
            var retorno = MetodosAjax.ObtemProdutoParaListagem(codInterno).value.split(';');
            
            if (retorno[0] == "Erro")
            {
                alert(retorno[1]);
                FindControl("txtCodProd", "input").value = "";
                return false;
            }
        }
        catch(err)
        {
            alert(err.value);
        }
    }
    
    function obtemQueryString()
    {
        var codInterno = FindControl("txtCodProd", "input").value;
        var idGrupo = FindControl("drpGrupo", "select").value;
        var idSubgrupo = FindControl("drpSubgrupo", "select").value;
        var situacao = FindControl("drpSituacao", "select").value;
        var descricao = FindControl("txtDescr", "input").value;
        var ordenar = FindControl("drpOrdenar", "select").value;
        var alturaInicio = FindControl("txtAlturaInicio", "input").value;
        var alturaFim = FindControl("txtAlturaFim", "input").value;
        var larguraInicio = FindControl("txtLarguraInicio", "input").value;
        var larguraFim = FindControl("txtLarguraFim", "input").value;
        
        return "codInterno=" + codInterno + "&idProduto=0&idFornec=0&idGrupo=" + idGrupo + 
            "&idSubgrupo=" + idSubgrupo + "&descr=" + descricao + "&situacao=" + situacao +
            "&alturaInicio=" + alturaInicio + "&alturaFim=" + alturaFim +
            "&larguraInicio=" + larguraInicio + "&larguraFim=" + larguraFim +
            "&orderBy=" + ordenar;
    }

    function openRpt(exportarExcel, ficha, id) {
        var colunas = FindControl("cbdColunas", "select").itens();

        if (id == 0)
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=" + (ficha ? "Ficha" : "") + "Produtos&" + obtemQueryString() + 
                "&exportarExcel=" + exportarExcel + "&colunas=" + colunas);
        }
        else
        {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?Rel=FichaProdutos&idProduto=" + id + "&colunas=0");
        }
        return false;
    }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="">Todas</asp:ListItem>
                                <asp:ListItem Value="Ativo" Selected="True">Ativo</asp:ListItem>
                                <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" AutoPostBack="True" DataSourceID="odsGrupo"
                                DataTextField="Name" DataValueField="Id"  AppendDataBoundItems="true"
                                OnSelectedIndexChanged="drpGrupo_SelectedIndexChanged"
                                EnableViewState="false">
                                <asp:ListItem Text="Todos" Value="" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo"
                                DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true"
                                OnSelectedIndexChanged="drpSubgrupo_SelectedIndexChanged"
                                EnableViewState="false">
                                <asp:ListItem Text="Todos" Value="" />
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="CodInterno">Código</asp:ListItem>
                                <asp:ListItem Value="Descricao">Descrição</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Imprimir" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdColunas" runat="server" CheckAll="true" Title="Selecione os dados">
                            </sync:CheckBoxListDropDown>
                        </td>
                    </tr>
                </table>
                <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label7" runat="server" Text="Altura" ForeColor="#0066FF"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAlturaInicio" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);">
                                </asp:TextBox>
                                <asp:Label runat="server" Text="Até"></asp:Label>
                                <asp:TextBox ID="txtAlturaFim" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);">
                                </asp:TextBox>
                                <asp:LinkButton ID="LinkButton4" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> 
                                    <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" />
                                </asp:LinkButton>
                            </td>
                            <td></td>
                            <td>
                                <asp:Label ID="Label10" runat="server" Text="Largura" ForeColor="#0066FF"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtLarguraInicio" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);">
                                </asp:TextBox>
                                <asp:Label runat="server" Text="Até"></asp:Label>
                                <asp:TextBox ID="txtLarguraFim" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);">
                                </asp:TextBox>
                                <asp:LinkButton ID="LinkButton5" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> 
                                    <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" />
                                </asp:LinkButton>
                            </td>
                            <td></td>
                        </tr>
                    </table>
            </td>
        </tr>
        <tr>
            <td align="left">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Inserir Produto</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdProduto" runat="server" SkinID="defaultGridView"
                    DataKeyNames="IdProd" DataSourceID="odsProduto" EnableViewState="false"
                    OnPageIndexChanged="grdProduto_PageIndexChanged">
                    <PagerSettings PageButtonCount="30" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadProduto.aspx?idProd=" + Eval("IdProd") + "&gr=" + drpGrupo.SelectedValue + "&sb=" + drpSubgrupo.SelectedValue %>'>
                                    <img border="0" src="../Images/EditarGrid.gif" alt="Editar" /></asp:HyperLink>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="../Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Produto?&quot;);" />
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/dinheiro.gif"
                                    OnClientClick='<%# "precoFornecedor(" + Eval("IdProd") + "); return false" %>'
                                    ToolTip="Preço por Fornecedor" />
                                <asp:ImageButton ID="imgDescontoQtde" runat="server" ToolTip="Desconto por Quantidade"
                                    Visible='<%# Glass.Configuracoes.PedidoConfig.Desconto.DescontoPorProduto %>' OnClientClick='<%# "openWindow(600, 800, \"../Cadastros/CadDescontoQtde.aspx?idProd=" + Eval("IdProd") + "\"); return false" %>'
                                    ImageUrl="~/Images/money_delete.gif" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                        <asp:BoundField DataField="DescricaoProdutoBeneficiamento" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="TipoProduto" HeaderText="Tipo" SortExpression="TipoProduto" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura"
                            SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura"
                            SortExpression="Largura" />
                        <asp:BoundField DataField="Custofabbase" DataFormatString="{0:C}" HeaderText="Custo Forn."
                            SortExpression="Custofabbase"></asp:BoundField>
                        <asp:BoundField DataField="CustoCompra" DataFormatString="{0:C}" HeaderText="Custo Imp."
                            SortExpression="CustoCompra">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ValorAtacado" HeaderText="Atacado" SortExpression="ValorAtacado"
                            DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ValorBalcao" HeaderText="Balcão" SortExpression="ValorBalcao"
                            DataFormatString="{0:C}">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ValorObra" HeaderText="Obra" SortExpression="ValorObra"
                            DataFormatString="{0:C}"></asp:BoundField>
                        <asp:BoundField DataField="ValorReposicao" DataFormatString="{0:C}" HeaderText="Reposição"
                            SortExpression="ValorReposicao" />
                        <asp:BoundField DataField="ValorMinimo" DataFormatString="{0:c}" HeaderText="Mínimo"
                            SortExpression="ValorMinimo" />
                        <asp:TemplateField HeaderText="Reserva" SortExpression="Reserva">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Reserva") %>'  Visible='<%# Eval("TipoCalculo").ToString() != "M2" && Eval("TipoCalculo").ToString() != "M2Direto" %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkReserva" runat="server" OnClientClick='<%# "abrirReserva(" + Eval("IdProd") + "); return false" %>'
                                    Text='<%# Eval("Reserva") %>' Visible='<%# Eval("TipoCalculo").ToString() != "M2" && Eval("TipoCalculo").ToString() != "M2Direto" %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Liberação" SortExpression="Liberacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Liberacao") %>'  Visible='<%# Eval("TipoCalculo").ToString() != "M2" && Eval("TipoCalculo").ToString() != "M2Direto" %>' ></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkLiberacao" runat="server" OnClientClick='<%# "abrirLiberacao(" + Eval("IdProd") + "); return false" %>'
                                    Text='<%# Eval("Liberacao") %>' Visible='<%# Eval("TipoCalculo").ToString() != "M2" && Eval("TipoCalculo").ToString() != "M2Direto" %>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Estoque" HeaderText="Estoque" SortExpression="QtdeEstoque" />
                        <asp:BoundField DataField="EstoqueDisponivel" HeaderText="Disponível" SortExpression="EstoqueDisponivel" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick='TagToTip("produto_<%# Eval("IdProd") %>", FADEIN, 300, COPYCONTENT, false, TITLE, "Detalhes", CLOSEBTN, true, CLOSEBTNTEXT, "Fechar", CLOSEBTNCOLORS, ["#cc0000", "#ffffff", "#D3E3F6", "#0000cc"], STICKY, true, FIX, [this, 10, 0]); return false;'>
                                    <img src="../Images/user_comment.gif" border="0" alt="Detalhes" /></a>
                                <div id="produto_<%# Eval("IdProd") %>" style="display: none">
                                    <asp:Label ID="Label1" runat="server" Text='<%# "Data de cadastro: " + Eval("DataCad", "{0:d}") %>'></asp:Label><br />
                                    <asp:Label ID="Label2" runat="server" Text='<%# "Usuário que cadastrou: " + Eval("NomeUsuarioCad") %>'></asp:Label><br />
                                    <asp:Label ID="Label6" runat="server" Text='<%# "Data de alteração: " + (Eval("DataAlt") != null && Eval("DataAlt") != "" ? Eval("DataAlt", "{0:d}") : "") %>'></asp:Label><br />
                                    <asp:Label ID="Label7" runat="server" Text='<%# "Usuário que alterou: " + Eval("NomeUsuarioAlt") %>'></asp:Label><br />
                                </div>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# ExibirPrecoAnterior %>'>
                                    <a href="#" onclick="openWindow(300, 400, '../Utils/ShowPrecoAnterior.aspx?idProd=<%# Eval("IdProd") %>');">
                                        <img src="../Images/money_hist.gif" border="0" title="Preço anterior" alt="Preço anterior" />
                                    </a>
                                </asp:PlaceHolder>
                                <uc1:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" 
                                    ImageUrl='<%# Glass.Global.UI.Web.Process.ProdutoRepositorioImagens.Instance.ObtemUrl((int)Eval("IdProd")) %>' />
                                <a href="#" onclick='openRpt(false, true, "<%# Eval("IdProd") %>"); return false;'>
                                    <img src="../Images/printer.png" border="0" alt="Imprimir" /></a> &nbsp;<uc2:ctrlLogPopup ID="ctrlLogPopup1"
                                        runat="server" Tabela="Produto" IdRegistro='<%# (uint)(int)Eval("IdProd") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
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
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false, false, 0);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true, false, 0); return false;"><img border="0" 
                    src="../Images/Excel.gif" alt="Exportar para o Excel" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="return openRpt(false, true, 0);">
                    <img alt="" border="0" src="../Images/printer.png" /> Imprimir ficha</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="LinkButton2" runat="server" OnClientClick="openRpt(true, true, 0); return false;"><img border="0" 
                    src="../Images/Excel.gif" alt="Exportar Ficha para o Excel" /> Exportar ficha para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="LinkButton3" runat="server" OnClientClick="abrirTelaExportar(); return false;">
                    Exportar/importar preços de produtos</asp:LinkButton>
            </td>
        </tr>
    </table>

    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" 
        DataObjectTypeName="Glass.Global.Negocios.Entidades.Produto"
        TypeName="Glass.Global.Negocios.IProdutoFluxo"
        DeleteMethod="ApagarProduto" EnablePaging="True"
        DeleteStrategy="GetAndDelete" 
        MaximumRowsParameterName="pageSize"
        SelectByKeysMethod="ObtemProduto"
        SelectMethod="PesquisarProdutos" SortParameterName="sortExpression">
        <SelectParameters>
            <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtDescr" Name="descricao" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue" />
            <asp:Parameter Name="idLoja" />
            <asp:Parameter Name="idFornec" />
            <asp:Parameter Name="nomeFornecedor" />
            <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupo" PropertyName="SelectedValue" Type="String" />
            <asp:Parameter Name="tipoNegociacao" />
            <asp:Parameter Name="apenasProdutosEstoqueBaixa" DefaultValue="False" />
            <asp:Parameter Name="agruparEstoqueLoja" DefaultValue="False" />
            <asp:ControlParameter ControlID="drpOrdenar" Name="ordenacao" PropertyName="SelectedValue" />
            <asp:ControlParameter ControlID="txtAlturaInicio" Name="alturaInicio" PropertyName="Text" Type="Decimal" />
            <asp:ControlParameter ControlID="txtAlturaFim" Name="alturaFim" PropertyName="Text" Type="Decimal" />
            <asp:ControlParameter ControlID="txtLArguraInicio" Name="larguraInicio" PropertyName="Text" Type="Decimal" />
            <asp:ControlParameter ControlID="txtLArguraFim" Name="larguraFim" PropertyName="Text" Type="Decimal" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupo" runat="server" 
        SelectMethod="ObtemGruposProduto" 
        TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo">
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupo" runat="server" 
        SelectMethod="ObtemSubgruposProduto"
        TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo">
        <SelectParameters>
            <asp:ControlParameter ControlID="drpGrupo" Name="idGrupoProd" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
