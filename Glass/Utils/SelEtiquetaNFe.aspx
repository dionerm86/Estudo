<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelEtiquetaNFe.aspx.cs" Inherits="Glass.UI.Web.Utils.SelEtiquetaNFe"
    Title="Impressões Pendentes" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<%@ Register Src="../Controls/ctrlSelFornecedor.ascx" TagName="ctrlSelFornecedor"
    TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setProdEtiqueta(idProdNf, numeroNFe, descrProd, qtd, qtdImpresso, altura, largura, totM, lote)
        {
            totM = new Number((parseFloat(totM.toString().replace(',', '.')) / parseFloat(qtd)) * parseFloat((qtd - qtdImpresso))).toFixed(2).toString().replace('.', ',');

            window.opener.setProdEtiqueta(null, null, null, idProdNf, numeroNFe, descrProd, "", "", qtd, qtdImpresso,
                qtd - qtdImpresso, altura, largura, "", totM, null, window, false, "", null, null, lote);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table id="tbProd" runat="server">
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="Label2" runat="server" Text="NF-e" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:TextBox ID="txtNumeroNFe" runat="server" Width="70px" onkeypress="return soNumeros(event, true, true);"
                                            onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td align="right">
                                        <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="Fornecedor" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <uc1:ctrlSelFornecedor ID="ctrlSelFornecedor" runat="server" />
                                    </td>
                                    <td align="right">
                                        <asp:Label ID="Label3" runat="server" Text="Descrição Prod." ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDescrProduto" runat="server" Width="150px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            OnClick="imgPesq_Click" />
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text="Data de emissão" ForeColor="#0066FF"></asp:Label>
                                    </td>
                                    <td nowrap="nowrap">
                                        <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                            OnClick="imgPesq_Click" />
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
                            <asp:GridView GridLines="None" ID="grdProduto" runat="server" AllowPaging="True"
                                AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="IdProd" DataSourceID="odsProduto"
                                CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                EditRowStyle-CssClass="edit" EmptyDataText="Nenhum produto encontrado.">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <a href="#" onclick="setProdEtiqueta('<%# Eval("IdProdNf") %>', '<%# Eval("NumeroNFe") %>', '<%# Eval("DescrProduto") %>', '<%# Eval("Qtde") %>', '<%# Eval("QtdImpresso") %>', '<%# Eval("Altura") %>', '<%# Eval("Largura") %>', '<%# Eval("TotM") %>', '<%# Eval("Lote") %>');">
                                                <img src="../Images/ok.gif" border="0" title="Selecionar" alt="Selecionar" /></a>
                                        </ItemTemplate>
                                        <ItemStyle Wrap="False" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="NumeroNfe" HeaderText="NF-e" SortExpression="NumeroNfe" />
                                    <asp:BoundField DataField="EmitenteNfe" HeaderText="Fornecedor" SortExpression="EmitenteNfe" />
                                    <asp:BoundField DataField="DescrProduto" HeaderText="Produto" SortExpression="DescrProduto" />
                                    <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                                    <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura">
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Qtde" HeaderText="Qtd." SortExpression="Qtde" />
                                    <asp:BoundField DataField="QtdImpresso" HeaderText="Qtd. Já Impresso" SortExpression="QtdImpresso" />
                                    <asp:BoundField DataField="Lote" HeaderText="Lote" SortExpression="Lote" />
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
                            <asp:LinkButton ID="lnkAddAll" runat="server" Font-Size="Small" OnClick="lnkAddAll_Click"><img src="../Images/addMany.gif" border="0"> Adicionar Todos (Ordenando por Cor/Espessura)</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProduto" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCountImpressaoEtiqueta" SelectMethod="GetListImpressaoEtiqueta"
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ProdutosNfDAO"
        >
        <SelectParameters>
            <asp:ControlParameter ControlID="txtNumeroNFe" Name="numeroNFe" PropertyName="Text"
                Type="UInt32" />
            <asp:ControlParameter ControlID="ctrlSelFornecedor" Name="idFornecedor" PropertyName="IdFornec"
                Type="UInt32" />
            <asp:ControlParameter ControlID="txtDescrProduto" Name="descricaoProd" PropertyName="Text"
                Type="String" />
            <asp:ControlParameter ControlID="ctrlDataIni" Name="dataEmissaoIni" PropertyName="DataString"
                Type="String" />
            <asp:ControlParameter ControlID="ctrlDataFim" Name="dataEmissaoFim" PropertyName="DataString"
                Type="String" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfImpressao" runat="server" />
</asp:Content>
