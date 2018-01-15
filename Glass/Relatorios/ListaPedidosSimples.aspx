<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="ListaPedidosSimples.aspx.cs" Inherits="Glass.UI.Web.Relatorios.ListaPedidosSimples" Title="Pedidos por M² / Peso" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <script type="text/javascript">

        function openRptBase(exportarExcel, nomeRel) {
            var idRota = FindControl("drpRota", "select").value;
            var dtIniEnt = FindControl("txtDataIni", "input").value;
            var dtFimEnt = FindControl("txtDataFim", "input").value;
    
            var queryString = "&idsRota=" + idRota + "&dtIniEnt=" + dtIniEnt + "&dtFimEnt=" + dtFimEnt + "&simples=true&exportarExcel=" + exportarExcel;

            openWindow(600, 800, 'RelBase.aspx?rel=' + nomeRel + queryString);
            return false;
        }

        function openRpt(exportarExcel) {
            return openRptBase(exportarExcel, 'ListaPedidos<%= Request["prod"] == "1" ? "Prod" : "" %>');
        }

        function openRptSimples(exportarExcel) {
            return openRptBase(exportarExcel, 'ListaPedidosSimples');
        }       

        function openRptUnico(idPedido) {
            openWindow(600, 800, "RelPedido.aspx?idPedido=" + idPedido);
            return false;
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Rota" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpRota" runat="server" DataSourceID="odsRota" DataTextField="Descricao"
                                DataValueField="IdRota" AppendDataBoundItems="True">
                                <asp:ListItem Value="">Selecione uma Rota</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Data de Entrega" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="txtDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="txtDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" ToolTip="Pesquisar" Style="height: 16px" />
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
                <asp:GridView GridLines="None" ID="grdPedido" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsPedido" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdPedido" EmptyDataText="Nenhum pedido encontrado."
                    AllowPaging="True" OnRowCommand="grdPedido_RowCommand">
                    <PagerSettings PageButtonCount="20" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# Eval("ExibirRelatorio") %>'>
                                    <a href="#" onclick="openRptUnico('<%# Eval("IdPedido") %>');">
                                        <img border="0" src="../Images/Relatorio.gif" /></a> </asp:PlaceHolder>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdPedido" HeaderText="Num" SortExpression="IdPedido" />
                        <asp:BoundField DataField="NfeAssociada" HeaderText="Num. NFe" 
                            SortExpression="NfeAssociada" Visible="False" />
                        <asp:BoundField DataField="CodCliente" HeaderText="Pedido Cli." SortExpression="CodCliente" />
                        <asp:BoundField DataField="NomeCliente" HeaderText="Cliente" SortExpression="NomeCliente" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:TemplateField HeaderText="Cidade" SortExpression="RptCidade" 
                            Visible="False">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("RptCidade") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("RptCidade") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NomeFunc" HeaderText="Funcionário" SortExpression="NomeFunc" />
                        <asp:BoundField DataField="TotalComDescontoConcatenado" HeaderText="Total" SortExpression="TotalComDescontoConcatenado" />
                        <asp:BoundField DataField="DescrTipoVenda" HeaderText="Pagto" SortExpression="DescrTipoVenda">
                            <ItemStyle Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DataPedido" HeaderText="Data" SortExpression="DataPedido" />
                        <asp:BoundField DataField="DataEntregaExibicao" HeaderText="Entrega" SortExpression="DataEntrega" />
                        <asp:BoundField DataField="DataConfLib" HeaderText="Conf. / Lib." 
                            SortExpression="DataConfLib" Visible="False" />
                        <asp:BoundField DataField="DataPronto" HeaderText="Pronto" 
                            SortExpression="DataPronto" Visible="False" />
                        <asp:BoundField DataField="DescrSituacaoPedido" HeaderText="Situação" SortExpression="DescrSituacaoPedido" />
                        <asp:TemplateField HeaderText="Situação Prod." SortExpression="DescrSituacaoProducao">
                            <ItemTemplate>
                                <asp:Label ID="lblSitProd" runat="server" OnLoad="lblSitProd_Load" Text='<%# Eval("DescrSituacaoProducao") %>'></asp:Label>
                                <asp:LinkButton ID="lnkSitProd" runat="server" CommandArgument='<%# Eval("IdPedido") %>'
                                    CommandName="Producao" OnLoad="lblSitProd_Load" Text='<%# Eval("DescrSituacaoProducao") %>'></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DescrSituacaoProducao") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DescricaoTipoPedido" HeaderText="Tipo" SortExpression="DescricaoTipoPedido" />
                        <asp:BoundField DataField="FastDeliveryString" HeaderText="Fast Delivery?" 
                            SortExpression="FastDeliveryString" Visible="False" />
                        <asp:BoundField DataField="TotM" HeaderText="Total m²" SortExpression="TotM" />
                        <asp:BoundField DataField="Peso" HeaderText="Peso total" SortExpression="Peso" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
           <td align="center">
                <asp:LinkButton ID="lnkImprimirSimples" runat="server" OnClientClick="return openRptSimples();"> <img alt="" border="0" src="../Images/printer.png" /> Imprimir (Peso e Tot. m²)</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcelSimples" runat="server" OnClientClick="openRptSimples(true); return false;"><img border="0" 
                    src="../Images/Excel.gif" /> Exportar para o Excel (Peso e Tot. m²)</asp:LinkButton>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPedido" runat="server" StartRowIndexParameterName="startRow"
        SelectMethod="PesquisarListaVendasPedidosSimples" TypeName="Glass.Data.DAL.PedidoDAO" MaximumRowsParameterName="pageSize"
        EnablePaging="True" SelectCountMethod="PesquisarListaVendasPedidosSimplesCount" SortParameterName="sortExpression">
        <SelectParameters>
            <asp:ControlParameter ControlID="drpRota" Name="idsRota" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="txtDataIni" Name="dataInicioEntrega" PropertyName="DataString" Type="String" />
            <asp:ControlParameter ControlID="txtDataFim" Name="dataFimEntrega" PropertyName="DataString" Type="String" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRota" runat="server" SelectMethod="GetAll" TypeName="Glass.Data.DAL.RotaDAO">
    </colo:VirtualObjectDataSource>
</asp:Content>
