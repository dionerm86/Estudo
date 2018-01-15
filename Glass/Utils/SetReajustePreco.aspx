<%@ Page Title="Reajuste de Preços dos Produtos" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="SetReajustePreco.aspx.cs" Inherits="Glass.UI.Web.Utils.SetReajustePreco" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        
        var clicked = false;

        function valida() {
            if (clicked)
                return false;

            clicked = true;

            var custoFab = FindControl("chkCustoFabBase", "input").checked;
            var custoCompra = FindControl("chkCustoCompra", "input").checked;
            var balcao = FindControl("chkBalcao", "input").checked;
            var obra = FindControl("chkObra", "input").checked;
            var atacado = FindControl("chkAtacado", "input").checked;
            var reposicao = FindControl("chkReposicao", "input").checked;
            var fiscal = FindControl("chkFiscal", "input").checked;

            if (!confirm("Tem certeza que deseja aplicar o reajuste informado nos produtos?")) {
                clicked = false;
                return false;
            }

            if (FindControl("txtReajuste", "input").value == "") {
                alert("Informe o reajuste que será aplicado.");
                clicked = false;
                return false;
            }

            if (!custoFab && !custoCompra && !balcao && !obra && !atacado && !reposicao && !fiscal) {
                alert("Selecione um preço.");
                clicked = false;
                return false;
            }

            clicked = false;
        }

        // Carrega dados do produto com base no código do produto passado
        function setProduto() {
            var codInterno = FindControl("txtCodProd", "input").value;

            if (codInterno == "")
                return false;

            var retorno = MetodosAjax.GetProd(codInterno).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                FindControl("txtCodProd", "input").value = "";
                return false;
            }

            FindControl("txtDescr", "input").value = retorno[2];
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="Label3" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpGrupo" runat="server" AutoPostBack="True" DataSourceID="odsGrupo"
                                DataTextField="Descricao" DataValueField="IdGrupoProd" OnSelectedIndexChanged="drpGrupo_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label1" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSubgrupo" runat="server" AutoPostBack="True" DataSourceID="odsSubgrupo"
                                DataTextField="Descricao" DataValueField="IdSubgrupoProd" OnSelectedIndexChanged="drpSubgrupo_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="lnkPesq" runat="server" OnClick="lnkPesq_Click"><img border="0\" 
                                src="../images/pesquisar.gif"></img></asp:LinkButton>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label2" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server" AutoPostBack="True">
                                <asp:ListItem Value="0">Todas</asp:ListItem>
                                <asp:ListItem Value="1">Ativo</asp:ListItem>
                                <asp:ListItem Value="2">Inativo</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="lnkPesq_Click"><img border="0\" 
                                src="../images/pesquisar.gif"></img></asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Reajuste" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtReajuste" runat="server" Width="50px" onkeypress="return soNumeros(event, false, false);"
                                MaxLength="6"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RadioButton ID="radPercent" runat="server" GroupName="1" Text="%" Checked="True" />
                        </td>
                        <td>
                            <asp:RadioButton ID="radReal" runat="server" GroupName="1" Text="R$" />
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Aplicar reajuste aos preços de:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkCustoFabBase" runat="server" Text="Custo Forn." />
                            <asp:CheckBox ID="chkCustoCompra" runat="server" Text="Custo Imp." />
                            <asp:CheckBox ID="chkBalcao" runat="server" Text="Balcão" />
                            <asp:CheckBox ID="chkObra" runat="server" Text="Obra" />
                            <asp:CheckBox ID="chkAtacado" runat="server" Text="Atacado" />
                            <asp:CheckBox ID="chkReposicao" runat="server" Text="Reposição" />
                            <asp:CheckBox ID="chkFiscal" runat="server" Text="Físcal" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Cód." ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="setProduto();" onkeydown="if (isEnter(event)) cOnClick('lnkPesqProd', 'a');"></asp:TextBox>
                        </td>
                        <td>
                            &nbsp;<asp:Label ID="Label9" runat="server" Text="Descrição" ForeColor="#0066FF"></asp:Label>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('lnkPesqProd', 'a');"
                                Width="200px"></asp:TextBox>
                            <asp:LinkButton ID="lnkPesqProd" runat="server" OnClientClick="setProduto();" OnClick="lnkPesq_Click"> <img border="0" src="../Images/Pesquisar.gif" /></asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
                <asp:Label ID="Label10" runat="server" Text="Obs.: Utilize números negativos no campo reajuste para abaixar o valor."></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td valign="top">
                            <table style="width: 100%">
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label7" runat="server" Text="Produtos que serão reajustados" ForeColor="#009933"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:GridView GridLines="None" ID="grdProdutosReajustados" runat="server" AutoGenerateColumns="False"
                                            CellPadding="4" DataSourceID="odsAjustado" ForeColor="#333333" AllowPaging="True"
                                            PageSize="20" OnRowCommand="grdProdutosReajustados_RowCommand" AllowSorting="True">
                                            <PagerSettings PageButtonCount="30" />
                                            <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                            <RowStyle BackColor="#E3EAEB" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btnRem" runat="server" CommandArgument='<%# Eval("IdProd") %>'
                                                            CommandName="RemReajuste" ImageUrl="~/Images/ExcluirGrid.gif" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                                                <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                                                <asp:BoundField DataField="Custofabbase" DataFormatString="{0:c}" HeaderText="Custo Forn."
                                                    SortExpression="Custofabbase" />
                                                <asp:BoundField DataField="CustoCompra" DataFormatString="{0:C}" HeaderText="Custo Imp."
                                                    SortExpression="CustoCompra" />
                                                <asp:BoundField DataField="ValorAtacado" HeaderText="Atacado" SortExpression="ValorAtacado"
                                                    DataFormatString="{0:C}" />
                                                <asp:BoundField DataField="ValorBalcao" HeaderText="Balcão" SortExpression="ValorBalcao"
                                                    DataFormatString="{0:C}" />
                                                <asp:BoundField DataField="ValorObra" HeaderText="Obra" SortExpression="ValorObra"
                                                    DataFormatString="{0:C}" />
                                                <asp:BoundField DataField="ValorReposicao" HeaderText="Reposição" SortExpression="ValorReposicao"
                                                    DataFormatString="{0:C}" />
                                                <asp:BoundField DataField="ValorFiscal" HeaderText="Físcal" SortExpression="ValorFiscal"
                                                    DataFormatString="{0:C}" />
                                            </Columns>
                                            <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                                            <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                            <EditRowStyle BackColor="#7C6F57" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td valign="top">
                            <table style="width: 100%">
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label6" runat="server" Text="Produtos que não serão reajustados" ForeColor="Red"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:GridView GridLines="None" ID="grdProdutosNaoReajustados" runat="server" AutoGenerateColumns="False"
                                            CellPadding="4" DataSourceID="odsNaoReajustado" ForeColor="#333333" AllowPaging="True"
                                            PageSize="20" DataKeyNames="IdProd" AllowSorting="True" OnRowCommand="grdProdutosNaoReajustados_RowCommand">
                                            <PagerSettings PageButtonCount="30" />
                                            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btnRem0" runat="server" CommandArgument='<%# Eval("IdProd") %>'
                                                            CommandName="AddReajuste" ImageUrl="~/Images/ExcluirGrid.gif" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                                            </Columns>
                                            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                                            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnReajustar" runat="server" OnClientClick="return valida();" Text="Reajustar"
                    OnClick="btnReajustar_Click" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HiddenField ID="hdfNaoReajustados" runat="server" />
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsAjustado" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetReajustadoCount" SelectMethod="GetListReajustado"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutoDAO" SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descricao" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="drpGrupo" Name="idGrupo" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="drpSubgrupo" Name="idSubgrupo" PropertyName="SelectedValue"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="hdfNaoReajustados" Name="idsProd" PropertyName="Value"
                            Type="String" />
                            <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsNaoReajustado" runat="server"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectCountMethod="GetNaoReajustadoCount"
                    SelectMethod="GetListNaoReajustado" SortParameterName="sortExpression" StartRowIndexParameterName="startRow"
                    TypeName="Glass.Data.DAL.ProdutoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfNaoReajustados" Name="idsProd" PropertyName="Value"
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupo" runat="server" SelectMethod="GetForFilter"
                    TypeName="Glass.Data.DAL.GrupoProdDAO">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSubgrupo" runat="server" SelectMethod="GetForFilter"
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
