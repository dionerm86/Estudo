<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaProdutos.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaProdutos" Title="Produtos" %>

<%@ Register Src="../Controls/ctrlImagemPopup.ascx" TagName="ctrlImagemPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function openRpt(exportarExcel) {
            var idFornec = FindControl("txtNumFornec", "input").value;
            idFornec = idFornec != "" ? idFornec : "0";
            var nomeFornec = FindControl("txtNomeFornec", "input").value;
            var idGrupo = FindControl("drpGrupo", "select").itens();
            var idSubgrupo = FindControl("drpSubgrupo", "select").itens();
            var codInterno = FindControl("txtCodInterno", "input").value;
            var descricao = FindControl("txtDescricao", "input").value;
            var ordenar = FindControl("drpOrdenar", "select").value;
            var agrupar = FindControl("chkAgrupar", "input").checked ? "true" : "false";
            var tipoProd = FindControl("drpTipoProd", "select").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var apenasProdutosEstoqueBaixa = FindControl("chkProdutosBaixa", "input").checked;
            var colunas = FindControl("cbdColunas", "select").itens();

            openWindow(600, 800, "RelBase.aspx?Rel=Produtos&idFornec=" + idFornec + "&nomeFornec=" + nomeFornec + "&idGrupo=" + idGrupo +
                "&tipoProd=" + tipoProd + "&idSubgrupo=" + idSubgrupo +  "&codInterno=" + codInterno + "&descr=" + descricao +
                "&exportarExcel=" + exportarExcel + "&apenasProdutosEstoqueBaixa=" + apenasProdutosEstoqueBaixa + "&situacao=" + situacao +
                "&orderBy=" + ordenar + "&agrupar=" + agrupar + "&colunas=" + colunas);

            return false;
        }

        function getFornec(idFornec) {
            if (idFornec.value == "")
                return;

            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtNomeFornec", "input").value = "";
                return false;
            }

            FindControl("txtNomeFornec", "input").value = retorno[1];
        }

        // Carrega dados do produto com base no código do produto passado
        function setProduto() {
            var codInterno = FindControl("txtCodInterno", "input").value;

            if (codInterno == "")
                return false;

            try {
                var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    FindControl("txtCodInterno", "input").value = "";
                    return false;
                }

                FindControl("txtDescricao", "input").value = retorno[2];
            }
            catch (err) {
                alert(err.value);
            }
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label2" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                             <sync:CheckBoxListDropDown runat="server" ID="drpGrupo" DataSourceID="odsGrupo"
                                DataValueField="Id" DataTextField="Name" Title="Selecione o grupo" AutoPostBack="true">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label1" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                             <sync:CheckBoxListDropDown runat="server" ID="drpSubgrupo" DataSourceID="odsSubgrupo"
                                DataValueField="Id" DataTextField="Name" Title="Selecione o subgrupo" AutoPostBack="true">
                            </sync:CheckBoxListDropDown>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq" runat="server" OnClick="lnkPesq_Click">
                                <img border="0" src="../images/pesquisar.gif" alt="Pesquisar" /></asp:LinkButton>
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodInterno" runat="server" Width="60px" onblur="setProduto();" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq0" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click">
                            <img border="0" src="../Images/Pesquisar.gif" alt="Pesquisar" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label4" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumFornec" runat="server" Width="60px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNomeFornec" runat="server" Width="170px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClientClick="getFornec(FindControl('txtNumFornec', 'input'));" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Tipo Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpTipoProd" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="Venda">Venda</asp:ListItem>
                                <asp:ListItem Value="Compra">Compra</asp:ListItem>
                                <asp:ListItem Value="CompraVenda">Compra/Venda</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label6" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="">Todas</asp:ListItem>
                                <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Imprimir" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <sync:CheckBoxListDropDown ID="cbdColunas" runat="server" CheckAll="true"
                                Title="Selecione os dados">
                            </sync:CheckBoxListDropDown>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Ordenar por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="Descricao">Descrição</asp:ListItem>
                                <asp:ListItem Value="CodInterno">Código</asp:ListItem>
                                <asp:ListItem Value="IdProd">Cadastro</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAgrupar" runat="server" Text="Agrupar impressão por grupo e sub-grupo" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkProdutosBaixa" runat="server" 
                                Text="Apenas produtos que indicam produto para baixa" AutoPostBack="True" />
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
                <asp:GridView ID="grdProdutos" runat="server"
                    DataSourceID="odsProdutos" SkinID="defaultGridView" PageSize="20">
                    <PagerSettings PageButtonCount="30" />
                    <EmptyDataTemplate>
                        Nenhum produto encontrado.
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="Custofabbase" DataFormatString="{0:c}" HeaderText="Custo Forn."
                            SortExpression="Custofabbase" />
                        <asp:BoundField DataField="CustoCompra" DataFormatString="{0:C}" 
                            HeaderText="Custo Imp." SortExpression="CustoCompra" />
                        <asp:BoundField DataField="ValorAtacado" HeaderText="Atacado" SortExpression="ValorAtacado"
                            DataFormatString="{0:C}" />
                        <asp:BoundField DataField="ValorBalcao" HeaderText="Balcão" SortExpression="ValorBalcao"
                            DataFormatString="{0:C}" />
                        <asp:BoundField DataField="ValorObra" HeaderText="Obra" SortExpression="ValorObra"
                            DataFormatString="{0:C}" />
                        <asp:BoundField DataField="ValorReposicao" DataFormatString="{0:C}" HeaderText="Reposição"
                            SortExpression="ValorReposicao" />
                        <asp:BoundField DataField="ValorMinimo" HeaderText="Mínimo" SortExpression="ValorMinimo"
                            DataFormatString="{0:C}" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlImagemPopup ID="ctrlImagemPopup1" runat="server" 
                                    ImageUrl='<%# Glass.Global.UI.Web.Process.ProdutoRepositorioImagens.Instance.ObtemUrl((int)Eval("IdProd")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="return openRpt(false);"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"> <img border="0" src="../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutos" runat="server" 
                    TypeName="Glass.Global.Negocios.IProdutoFluxo"
                    EnablePaging="True"
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarListagemProdutos" SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodInterno" Name="codInterno" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtDescricao" Name="descricao" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="txtNumFornec" Name="idFornec" PropertyName="Text" Type="Int32" />
                        <asp:ControlParameter ControlID="txtNomeFornec" Name="nomeFornecedor" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpGrupo" Name="idsGrupos" PropertyName="SelectedValue" Type="string" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idsSubgrupos" PropertyName="SelectedValue" Type="string" />
                        <asp:ControlParameter ControlID="drpTipoProd" Name="tipoNegociacao" PropertyName="SelectedValue" />
                        <asp:ControlParameter ControlID="chkProdutosBaixa" DefaultValue=""  Name="apenasProdutosEstoqueBaixa" PropertyName="Checked" Type="Boolean" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="ordenacao" PropertyName="SelectedValue" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupo" runat="server" 
                    SelectMethod="ObtemGruposProduto" TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubgrupo" runat="server" 
                    SelectMethod="ObtemSubgruposProduto" TypeName="Glass.Global.Negocios.IGrupoProdutoFluxo">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpGrupo" Name="idsGrupoProds" PropertyName="SelectedValue" Type="string" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
