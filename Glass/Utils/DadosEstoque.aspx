<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DadosEstoque.aspx.cs" Inherits="Glass.UI.Web.Utils.DadosEstoque"
    Title="Estoque de Produtos" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function getProduto()
        {

            if (FindControl("txtCodProd", "input").value == "")
            {
                openWindow(500, 650, '../Utils/SelProd.aspx'); return false;
            }
        }
        // Função chamada após selecionar produto pelo popup
        function setProduto(codInterno)
        {
            try
            {
                FindControl("txtCodProd", "input").value = codInterno;
                FindControl("txtCodProd", "input").focus();
            }
            catch (err)
            {

            }
        }
    </script>

    <table>
        <tr runat="server" id="nomeProduto">
            <td align="center" class="subtitle1">
                Produto:
                <asp:Label ID="lblProduto" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr runat="server" id="separador">
            <td>
                &nbsp;
            </td>
        </tr>
        <tr runat="server" id="buscaProduto">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Produto:" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq')"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                OnClientClick="getProduto()" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblDescrProd" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr id="dadosReserva" runat="server">
            <td align="center">
                <span style="font-weight: bold">Produtos em Reserva
                    <asp:Label ID="lblLiberacao" runat="server" Text="ou Liberação"></asp:Label>
                </span>
                <asp:GridView ID="grdEstoqueProdutos" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" DataSourceID="odsEstoqueProdutos" GridLines="None" EmptyDataText="Não há produtos em reserva"
                    DataKeyNames="IdPedido">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja cancelar esse pedido?&quot;)) return false"
                                    OnDataBinding="imgCancelar_DataBinding" ToolTip="Cancelar pedido" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="QtdeReserva" HeaderText="Qtde Reserva" SortExpression="QtdeReserva" />
                        <asp:BoundField DataField="QtdeLiberacao" HeaderText="Qtde Liberação" SortExpression="QtdeLiberacao" />
                        <asp:BoundField DataField="DataEntrega" DataFormatString="{0:d}" HeaderText="Data Entrega"
                            SortExpression="DataEntrega" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <asp:Label ID="Label2" runat="server" Font-Italic="True" ForeColor="Red" Text="Pedidos com data de entrega vencida há mais de 5 dias poderão ser cancelados para disponibilizar estoque."></asp:Label>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEstoqueProdutos" runat="server" SelectMethod="GetByProd"
                    TypeName="Glass.Data.RelDAL.EstoqueProdutosDAO" DeleteMethod="CancelarPedido">
                    <DeleteParameters>
                        <asp:Parameter Name="idPedido" Type="UInt32" />
                    </DeleteParameters>
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdProd" Name="idProd" PropertyName="Value" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr runat="server" id="dadosProducao">
            <td align="center">
                <br />
                <span style="font-weight: bold">Produtos em Produção </span>
                <asp:GridView ID="grdProdutosPedido" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataSourceID="odsProdutosPedido" GridLines="None" EmptyDataText="Não há produtos em produção">
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="Ambiente" HeaderText="Ambiente" SortExpression="Ambiente" />
                        <asp:BoundField DataField="DataEntregaPedido" HeaderText="Prev. Entrega" SortExpression="DataEntregaPedido"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="Produzindo" HeaderText="Qtde" SortExpression="Produzindo" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosPedido" runat="server" SelectMethod="GetForEstoqueVidrosP"
                    TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdProd" Name="idProd" PropertyName="Value" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr runat="server" id="dadosCompra">
            <td align="center">
                <br />
                <span style="font-weight: bold">Produtos sendo comprados </span>
                <asp:GridView ID="grdProdutosComprando" runat="server" AutoGenerateColumns="False"
                    CssClass="gridStyle" DataSourceID="odsProdutosComprando" GridLines="None" EmptyDataText="Não há produtos em produção">
                    <Columns>
                        <asp:BoundField DataField="IdCompra" HeaderText="Compra" SortExpression="IdCompra" />
                        <asp:BoundField DataField="DescricaoComprando" HeaderText="Qtde" SortExpression="DescricaoComprando" />
                        <asp:BoundField DataField="DataFabrica" DataFormatString="{0:d}" HeaderText="Prev. Entrega"
                            SortExpression="DataFabrica" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosComprando" runat="server" SelectMethod="GetProdutosComprando"
                    TypeName="Glass.Data.DAL.ProdutosCompraDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="hdfIdProd" Name="idProd" PropertyName="Value" Type="UInt32" />
                        <asp:QueryStringParameter Name="idLoja" QueryStringField="idLoja" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfIdProd" runat="server" />
</asp:Content>
