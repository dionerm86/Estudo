<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetNaturezaOperacaoProdutoGerarNf.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetNaturezaOperacaoProdutoGerarNf" Title="Selecionar Natureza de Operação por Produto" MasterPageFile="~/Layout.master" %>

<%@ Register src="../Controls/ctrlNaturezaOperacao.ascx" tagname="ctrlNaturezaOperacao" tagprefix="uc1" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function gerarNf(tipo)
        {
            var tabela = document.getElementById(tipo == "ped" ? "<%= grdProdutos.ClientID %>" : "<%= grdProdutosCompra.ClientID %>");
            var dadosNaturezasOperacao = [];

            for (i = 1; i < tabela.rows.length; i++)
            {
                var idProd = FindControl("hdfIdProd", "input", tabela.rows[i]).value;
                var idNaturezaOperacao = FindControl("ctrlNaturezaOperacao_selNaturezaOperacao_hdfValor", "input", tabela.rows[i]).value;
                 
                dadosNaturezasOperacao.push(idProd + "," + idNaturezaOperacao);
            }

            var botaoGerarOpener = FindControl("btnGerarNf", "input", window.opener.document);
            window.opener.gerarNf(botaoGerarOpener, dadosNaturezasOperacao.join("-"), <%= Request["nfce"] %>);
            closeWindow();
        }

        function fechouJanela()
        {
            window.opener.FalhaGerarNf("", true);
        }

        window.addEventListener("unload", fechouJanela, false);
    
    </script>

    <table>
        <tr runat="server" id="pedidos">
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" DataSourceID="odsProdutos"
                    DataKeyNames="idProd" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum produto encontrado.">
                    <Columns>
                        <asp:BoundField HeaderText="Cód." DataField="codInterno" />
                        <asp:BoundField HeaderText="Produto" DataField="DescrProduto" />
                        <asp:TemplateField HeaderText="Natureza de Operação">
                            <ItemTemplate>
                                <uc1:ctrlNaturezaOperacao ID="ctrlNaturezaOperacao" runat="server" OnPreRender="ctrlNaturezaOperacao_PreRender" />
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Eval("IdProdUsar") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource ID="odsProdutos" runat="server" SelectMethod="GetByVariosPedidos"
                    TypeName="Glass.Data.DAL.ProdutosPedidoDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter QueryStringField="idsPedidos" Type="String" Name="idsPedido" />
                        <asp:QueryStringParameter QueryStringField="idsLiberarPedidos" Type="String" Name="idsLiberarPedido"
                            ConvertEmptyStringToNull="false" DefaultValue="" />
                        <asp:QueryStringParameter QueryStringField="agruparProdutos" Type="Boolean" Name="agruparProdutos" />
                        <asp:Parameter DefaultValue="true" Name="agruparSomentePorProduto" Type="Boolean" />
                        <asp:Parameter DefaultValue="true" Name="agruparProjetosAoAgruparProdutos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:Button ID="btnConfirmar" runat="server" Text="Gerar NF" OnClientClick="gerarNf('ped');"
                    Style="margin: 4px" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClientClick="closeWindow();"
                    Style="margin: 4px" />
            </td>
        </tr>
        <tr runat="server" id="compras">
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutosCompra" runat="server" DataSourceID="odsProdutosCompra"
                    DataKeyNames="idProd" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum produto encontrado.">
                    <Columns>
                        <asp:BoundField HeaderText="Cód." DataField="codInterno" />
                        <asp:BoundField HeaderText="Produto" DataField="DescrProduto" />
                        <asp:TemplateField HeaderText="Natureza de Operação">
                            <ItemTemplate>
                                <uc1:ctrlNaturezaOperacao ID="ctrlNaturezaOperacao" runat="server" OnPreRender="ctrlNaturezaOperacao_PreRender" />
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Eval("IdProd") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource ID="odsProdutosCompra" runat="server" SelectMethod="GetByVariasCompras"
                    TypeName="Glass.Data.DAL.ProdutosCompraDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter QueryStringField="idsCompras" Type="String" Name="idsCompras" />
                        <asp:QueryStringParameter QueryStringField="agruparProdutos" Type="Boolean" Name="agruparProdutos" />
                        <asp:Parameter DefaultValue="true" Name="agruparSomentePorProduto" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <asp:Button ID="btnConfirmarCompra" runat="server" Text="Gerar NF" OnClientClick="gerarNf('compra');"
                    Style="margin: 4px" />
                <asp:Button ID="btnCancelarCompra" runat="server" Text="Cancelar" OnClientClick="closeWindow();"
                    Style="margin: 4px" />
            </td>
        </tr>
    </table>
</asp:Content>
