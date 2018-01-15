<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetCFOPProdutoGerarNf.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetCFOPProdutoGerarNf" Title="Selecionar CFOP por Produto" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function gerarNf()
        {
            var tabela = document.getElementById("<%= grdProdutos.ClientID %>");
            var dadosCfops = [];

            for (i = 1; i < tabela.rows.length; i++)
            {
                var idProd = FindControl("hdfIdProd", "input", tabela.rows[i]).value;
                var idCfop = FindControl("ddlCfop", "select", tabela.rows[i]).value;

                dadosCfops.push(idProd + "," + idCfop);
            }

            var botaoGerarOpener = FindControl("btnGerarNf", "input", window.opener.document);
            window.opener.gerarNf(botaoGerarOpener, dadosCfops.join("-"));
            closeWindow();
        }

        function fechouJanela()
        {
            window.opener.FalhaGerarNf("", true);
        }

        window.addEventListener("unload", fechouJanela, false);
    
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdProdutos" runat="server" DataSourceID="odsProdutos"
                    DataKeyNames="idProd" AutoGenerateColumns="False" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhum produto encontrado.">
                    <Columns>
                        <asp:BoundField HeaderText="Cód." DataField="codInterno" />
                        <asp:BoundField HeaderText="Produto" DataField="DescrProduto" />
                        <asp:TemplateField HeaderText="CFOP">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlCfop" runat="server" DataSourceID="odsCfop" DataTextField="CodInternoDescricao"
                                    DataValueField="IdCfop" AppendDataBoundItems="True" OnLoad="odsCfop_Load">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="valCfop" runat="server" ErrorMessage="*" ControlToValidate="ddlCfop"
                                    Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Eval("IdProdUsar") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProdutos" runat="server" SelectMethod="GetByVariosPedidos"
                    TypeName="Glass.Data.DAL.ProdutosPedidoDAO" >
                    <SelectParameters>
                        <asp:QueryStringParameter QueryStringField="idsPedidos" Type="String" Name="idsPedido" />
                        <asp:QueryStringParameter QueryStringField="idsLiberarPedidos" Type="String" Name="idsLiberarPedido"
                            ConvertEmptyStringToNull="false" DefaultValue="" />
                        <asp:QueryStringParameter QueryStringField="agruparProdutos" Type="Boolean" Name="agruparProdutos" />
                        <asp:Parameter DefaultValue="true" Name="agruparSomentePorProduto" Type="Boolean" />
                        <asp:Parameter DefaultValue="true" Name="agruparProjetosAoAgruparProdutos" Type="Boolean" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCfop" runat="server" SelectMethod="GetSortedByCodInterno"
                    TypeName="Glass.Data.DAL.CfopDAO">
                </colo:VirtualObjectDataSource>
                <asp:Button ID="btnConfirmar" runat="server" Text="Gerar NF" OnClientClick="gerarNf();"
                    Style="margin: 4px" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClientClick="closeWindow();"
                    Style="margin: 4px" />
            </td>
        </tr>
    </table>
</asp:Content>
