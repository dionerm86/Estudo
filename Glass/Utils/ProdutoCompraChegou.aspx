<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProdutoCompraChegou.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ProdutoCompraChegou" Title="Finalizar Compra Gerando Nota Fiscal"
    MasterPageFile="~/Layout.master" %>

<%@ Register src="../Controls/ctrlNaturezaOperacao.ascx" tagname="ctrlNaturezaOperacao" tagprefix="uc1" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function validar()
        {

        }
    </script>

    <table align="center">
        <tr>
            <td class="subtitle1">
                Compra:
                <asp:Label ID="lblIdCompra" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdProdutosCompra" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="IdProdCompra" DataSourceID="odsProdutosCompra" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="CodInterno" HeaderText="Cód." SortExpression="CodInterno" />
                        <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                        <asp:BoundField DataField="Qtde" HeaderText="Qtde" SortExpression="Qtde" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="TotM" HeaderText="Tot. M2" SortExpression="TotM" />
                        <asp:BoundField DataField="Total" DataFormatString="{0:c}" HeaderText="Total" SortExpression="Total" />
                        <asp:TemplateField HeaderText="Qtde prod. contábil">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQtdeContabil" runat="server" Width="50px" Text="0" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                                <asp:RangeValidator ID="rgvQtdeContabil" runat="server" ControlToValidate="txtQtdeContabil"
                                    MaximumValue='<%# Eval("Qtde") %>' MinimumValue="0" Type="Double" Display="Dynamic"
                                    ErrorMessage='<%# "Valor entre 0 e " + Eval("Qtde") %>'></asp:RangeValidator>
                                <asp:HiddenField ID="hdfIdProdCompra" runat="server" Value='<%# Eval("IdProdCompra") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Fiscal" SortExpression="ValorFiscal">
                            <ItemTemplate>
                                <asp:TextBox ID="txtValorFiscal" runat="server" Text='<%# Bind("ValorFiscalString") %>'
                                    Width="70px"></asp:TextBox>
                                <asp:HiddenField ID="hdfValorFiscal" runat="server" Value='<%# Bind("ValorFiscalString") %>' />
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                Para gerar uma nota fiscal com produtos em separado da compra,
                <br />
                selecione a quantidade de cada produto que sairá da compra e irá
                <br />
                para a nota fiscal na tabela acima. Depois selecione a natureza da operação
                <br />
                que será usada pela nota fiscal.
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            Natureza da Operação:
                        </td>
                        <td>
                            <uc1:ctrlNaturezaOperacao ID="ctrlNaturezaOperacao" runat="server"
                                PermitirVazio="false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnFinalizar" runat="server" Text="Finalizar" OnClick="btnFinalizar_Click" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosCompra" runat="server" SelectMethod="GetByCompra"
                    TypeName="Glass.Data.DAL.ProdutosCompraDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idCompra" QueryStringField="idCompra" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
