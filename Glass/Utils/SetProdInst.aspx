<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetProdInst.aspx.cs" Inherits="Glass.UI.Web.Utils.SetProdInst"
    Title="Produtos Instalados" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <table id="tbProd" runat="server">
                    <tr>
                        <td align="center">
                            <asp:GridView GridLines="None" ID="grdProduto" runat="server" AutoGenerateColumns="False"
                                DataKeyNames="IdProdPed" DataSourceID="odsProdutosInst" CssClass="gridStyle"
                                PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                                EmptyDataText="Nenhum produto encontrado.">
                                <Columns>
                                    <asp:TemplateField HeaderText="Qtd. instalada">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQtdInst" runat="server" Width="50" MaxLength="4" Text='<%# Glass.Conversoes.StrParaInt(Eval("Qtde").ToString()) - Glass.Conversoes.StrParaInt(Eval("QtdeInstalada").ToString()) %>'
                                                onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                            <asp:HiddenField ID="hdfIdProdPed" runat="server" Value='<%# Eval("IdProdPed") %>' />
                                            <asp:HiddenField ID="hdfQtdeInst" runat="server" Value='<%# Eval("QtdeInstalada") %>' />
                                            <asp:HiddenField ID="hdfQtdeTotal" runat="server" Value='<%# Eval("Qtde") %>' />
                                            <asp:HiddenField ID="hdfDescrProduto" runat="server" Value='<%# Eval("DescrProduto") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CodInterno" HeaderText="Código" SortExpression="CodInterno" />
                                    <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                                    <asp:BoundField DataField="Qtde" HeaderText="Qtde total" SortExpression="Qtde">
                                        <ItemStyle Font-Bold="True" ForeColor="#00CC00" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="QtdeInstalada" HeaderText="Qtd. já instalada" SortExpression="QtdeInstalada">
                                        <ItemStyle Font-Bold="True" ForeColor="Red" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                                    <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="TotM" HeaderText="Total m²" SortExpression="TotM"></asp:BoundField>
                                    <asp:BoundField DataField="Total" DataFormatString="{0:C}" HeaderText="Total" SortExpression="Total">
                                    </asp:BoundField>
                                </Columns>
                                <PagerStyle />
                                <EditRowStyle />
                                <AlternatingRowStyle />
                            </asp:GridView>
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
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Observação" ForeColor="#0066FF"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtObs" runat="server" Rows="3" TextMode="MultiLine" Width="300px"></asp:TextBox>
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
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClick="btnConfirmar_Click" />
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutosInst" runat="server" MaximumRowsParameterName=""
        SelectMethod="GetListInst" StartRowIndexParameterName="" TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="idInstalacao" QueryStringField="idInstalacao" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
