<%@ Page Title="Compra de Mercadoria" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstCompraPcp.aspx.cs"
    Inherits="Glass.UI.Web.Listas.LstCompraPcp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function cancelar(idCompra) {
            openWindow(150, 420, "../Utils/SetMotivoCancCompra.aspx?idCompra=" + idCompra);
        }

        function openRpt(idCompra) {
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=Compra&idCompra=" + idCompra);
            return false;
        }

        function novaCompra(idCompra) {
            alert("Compra inserida com sucesso! Num. compra: " + idCompra);
            window.location = "LstCompraPcp.aspx";
        }

        function getFornec(idFornec) {
            var retorno = MetodosAjax.GetFornecConsulta(idFornec.value).value.split(';');

            if (retorno[0] == "Erro") {
                alert(retorno[1]);
                idFornec.value = "";
                FindControl("txtFornecedor", "input").value = "";
                return false;
            }

            FindControl("txtNome", "input").value = retorno[1];

        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Num. Compra" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumCompra" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />&nbsp;
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Num. Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNumPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFornecedor" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"
                                onblur="getFornec(this);"></asp:TextBox>
                            <asp:TextBox ID="txtNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">Nova Compra</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
            <asp:GridView GridLines="None" ID="grdCompraPcp" runat="server" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" DataSourceID="odsCompraPcp"
                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                EditRowStyle-CssClass="edit" DataKeyNames="IdCompra" EmptyDataText="Não há compras cadastradas."
                OnRowCommand="grdCompraPcp_RowCommand">
                <PagerSettings PageButtonCount="30" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkEditar" runat="server" ToolTip="Editar" NavigateUrl='<%# "../Cadastros/CadCompra.aspx?idCompra=" + Eval("IdCompra") %>'
                                Visible='<%# Eval("EditVisible") %>'><img border="0" src="../Images/EditarGrid.gif" /></asp:HyperLink>
                            <a href="#" onclick="return openRpt(<%# Eval("IdCompra") %>);">
                                <img src="../Images/relatorio.gif" border="0" title="Visualizar dados da Compra"></a>
                            <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Cancelar" ImageUrl="~/Images/ExcluirGrid.gif"
                                Visible='<%# Eval("CancelVisible") %>' OnClientClick='<%# "cancelar(" + Eval("IdCompra") + "); return false;" %>'
                                ToolTip="Cancelar" />
                            <asp:ImageButton ID="imbExcluirPagto" runat="server" CommandName="CancelarPagto"
                                ImageUrl="~/Images/CancelarPagto.gif" Visible='<%# Eval("CancelPagtoVisible") %>'
                                OnClientClick="return confirm(&quot;Tem certeza que deseja cancelar o pagamento desta compra?&quot;);"
                                ToolTip="Cancelar Pagto." CommandArgument='<%# Eval("IdCompra") %>' />
                            <asp:PlaceHolder ID="pchFotos" runat="server" 
                                Visible='<%# Eval("FotosVisible") %>'><a href="#" 
                                    onclick='openWindow(600, 700, &#039;../Cadastros/CadFotos.aspx?id=<%# Eval("IdCompra") %>&tipo=compra&#039;); return false;'>
                                <img border="0px" src="../Images/Clipe.gif"></img></a></asp:PlaceHolder>
                            <asp:ImageButton ID="imgNfe" runat="server" 
                                CommandArgument='<%# Eval("IdCompra") %>' CommandName="GerarNFe" 
                                ImageUrl="~/Images/script_go.gif" 
                                onclientclick="if (!confirm(&quot;Deseja gerar a nota fiscal para essa compra?&quot;)) return false" 
                                ToolTip="Gerar NF de entrada" Visible='<%# Eval("GerarNFeVisible") %>' />
                        </ItemTemplate>
                        <ItemStyle Wrap="False" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="IdCompra" HeaderText="Num" SortExpression="IdCompra" />
                    <asp:BoundField DataField="IdPedidoEspelho" HeaderText="Pedido" 
                        SortExpression="IdPedidoEspelho" />
                    <asp:BoundField DataField="NomeFornec" HeaderText="Fornecedor" SortExpression="NomeFornec" />
                    <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                    <asp:BoundField DataField="DescrUsuCad" HeaderText="Funcionário" SortExpression="DescrUsuCad" />
                    <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" DataFormatString="{0:C}">
                        <ItemStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DescrTipoCompra" HeaderText="Pagto" SortExpression="DescrTipoCompra">
                        <ItemStyle Wrap="False" />
                    </asp:BoundField>
                    <asp:BoundField DataField="DataCad" DataFormatString="{0:d}" HeaderText="Data" SortExpression="DataCad" />
                    <asp:BoundField DataField="DescrSituacao" HeaderText="Situação" SortExpression="DescrSituacao" />
                    <asp:CheckBoxField DataField="Contabil" HeaderText="Contábil" 
                        SortExpression="Contabil" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ID="imgReabrir" runat="server" CommandArgument='<%# Eval("IdCompra") %>'
                                CommandName="Reabrir" ImageUrl="~/Images/cadeado.gif" OnClientClick="if (!confirm(&quot;Deseja reabrir essa compra?&quot;)) return false;"
                                Visible='<%# Eval("ReabrirVisible") %>' />
                            <asp:Image ID="imgEstoque" runat="server" ImageUrl="~/Images/basket_add.gif" ToolTip="Estoque Creditado"
                                Visible='<%# Eval("EstoqueBaixado") %>' />
                            <asp:LinkButton ID="lnkProdutoChegou" runat="server" 
                                onclientclick='<%# "produtoChegou(" + Eval("IdCompra") + "); return false" %>' 
                                Visible='<%# Eval("ProdutoChegouVisible") %>'>Produto chegou</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle />
                <EditRowStyle />
                <AlternatingRowStyle />
            </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCompraPcp" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountPcp" SelectMethod="GetListPcp" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.CompraDAO" OnDeleted="odsCompraPcp_Deleted"
                    DataObjectTypeName="Glass.Data.Model.Compra" DeleteMethod="Delete">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNumCompra" Name="idCompra" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNumPedido" Name="idPedido" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtFornecedor" Name="idFornec" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nomeFornec" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
