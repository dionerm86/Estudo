<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ListaProdutosComprar.aspx.cs"
    Inherits="Glass.UI.Web.Relatorios.ListaProdutosComprar" Title="Produtos que Ainda não Foram Comprados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt() {
            var codInterno = FindControl("txtCodProd", "input").value;
            var descrProd = FindControl("txtDescr", "input").value;
            var idPedido = FindControl("txtPedido", "input").value;

            openWindow(600, 800, "RelBase.aspx?rel=ProdutosComprar&codInterno=" + codInterno + "&descrProd=" + descrProd + "&idPedido=" + idPedido);
        }

        function loadProduto(codInterno) {
            if (codInterno.value == "")
                return false;

            try {
                var retorno = MetodosAjax.GetProd(codInterno.value).value.split(';');

                if (retorno[0] == "Erro") {
                    alert(retorno[1]);
                    codInterno.value = "";
                    return false;
                }

                FindControl("txtDescr", "input").value = retorno[2];
            }
            catch (err) {
                alert(err.value);
            }
        }
    </script>

    <table>
        
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="60px" onblur="loadProduto(this);"
                                onkeydown="if (isEnter(event)) OnClick('imgPesq', null);"></asp:TextBox>
                            <asp:TextBox ID="txtDescr" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"
                                Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Pedido" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPedido" runat="server" Width="60px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
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
                <asp:GridView ID="grdProdutos" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataSourceID="odsProdutos" GridLines="None" AllowPaging="True" 
                    PageSize="20" 
                    EmptyDataText="Informe algum filtro.">
                    <Columns>
                        <asp:BoundField DataField="IdPedido" HeaderText="Pedido" SortExpression="IdPedido" />
                        <asp:BoundField DataField="AmbientePedido" HeaderText="Ambiente" SortExpression="AmbientePedido" />
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="TotM" HeaderText="Tot. M2" SortExpression="TotM" />
                        <asp:BoundField DataField="QtdeComprada" HeaderText="Qtde Comprada" SortExpression="QtdeComprada" />
                        <asp:BoundField DataField="QtdeComprar" HeaderText="Qtde Comprar" SortExpression="QtdeComprar" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutos" runat="server" SelectMethod="GetForCompraList"
                    TypeName="Glass.Data.DAL.ProdutosPedidoEspelhoDAO" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetForCompraCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
                    <SelectParameters>
                        <asp:Parameter Name="idPedidoEspelho" Type="UInt32" />
                        <asp:Parameter Name="idAmbientePedido" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInternoProd" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescr" Name="descrProd" PropertyName="Text" Type="String" />
                        <asp:ControlParameter ControlID="txtPedido" Name="idPedido" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false"><img src="../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
