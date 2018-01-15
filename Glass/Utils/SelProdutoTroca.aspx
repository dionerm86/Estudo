<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelProdutoTroca.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelProdutoTroca" Title="Selecione o produto do pedido" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function setProdutoTroca(idProdPed, qtde, control) {


            var txtEtiquetas = FindControl("txtEtiquetas", "input", control.parentElement.parentElement);

            window.opener.setProdutoTroca(idProdPed, qtde, txtEtiquetas != null ? txtEtiquetas.value : "");
            closeWindow();
        }

        function selEtiquetaTroca(idProdPed) {
            openWindow(600, 800, "../Utils/SelEtiquetaTroca.aspx?idProdPed=" + idProdPed);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Cód. Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCodProd" runat="server" Width="70px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Produto" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescrProd" runat="server" Width="200px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblAmbiente" runat="server" Text="Ambiente" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtAmbiente" runat="server" Width="150px"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Altura" ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtAltura" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox></td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Largura" ForeColor="#0066FF"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtLargura" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox></td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" AllowPaging="True"
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" DataSourceID="odsProdutoPedido"
                    DataKeyNames="IdProd" EmptyDataText="Não há produtos disponíveis para troca.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbOk" runat="server" ImageUrl="~/Images/ok.gif" ToolTip="Selecionar"
                                    OnClientClick='<%# "setProdutoTroca(" + Eval("IdProdPed") + ", " + (Eval("QtdeTroca") != null ? Eval("QtdeTroca").ToString().Replace(",", ".") : "0") + ", this); return false" %>' />
                                <asp:HiddenField runat="server" ID="hdfIdProdPed" value='<%# Eval("IdProdPed") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Ambiente" HeaderText="Ambiente" SortExpression="Ambiente" />
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód. Produto" SortExpression="CodInterno" />
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:TemplateField HeaderText="Qtde" SortExpression="QtdeTroca">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblQtde" Text='<%# Eval("QtdeTroca") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="ValorVendido" DataFormatString="{0:c}" HeaderText="Valor" SortExpression="ValorVendido" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:c}" HeaderText="Total" SortExpression="Total" />
                        <asp:TemplateField HeaderText="Etiquetas">
                            <ItemTemplate>
                                <asp:TextBox runat="server" ID="txtEtiquetas" Visible='<%# (bool)Eval("IsVidroEstoque") %>' Width="170px"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:LinkButton ID="lnkAddAll" runat="server" Font-Size="Small" OnClick="lnkAddAll_Click"><img src="../Images/addMany.gif" border="0"> 
                    Adicionar Todos</asp:LinkButton>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsProdutoPedido" runat="server" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetForTrocaCount" SelectMethod="GetForTroca" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosPedidoDAO"
                    EnablePaging="True">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idPedido" QueryStringField="idPedido" Type="UInt32" />
                        <asp:ControlParameter ControlID="txtCodProd" Name="codInterno" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtDescrProd" Name="descricao" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtAmbiente" Name="ambiente" PropertyName="Text"
                            Type="String" />
                        <asp:ControlParameter ControlID="txtAltura" Name="altura" PropertyName="Text"
                            Type="Single" />
                        <asp:ControlParameter ControlID="txtLargura" Name="largura" PropertyName="Text"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
