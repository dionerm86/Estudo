<%@ Page Title="Informações sobre Pedidos por Período" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="LstInfoPedidosPeriodo.aspx.cs" Inherits="Glass.UI.Web.Listas.LstInfoPedidosPeriodo" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function exibirDetalhes(data)
        {
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            redirectUrl("LstInfoPedidos.aspx?data=" + data + "&voltar=1&dataIni=" + dataIni + "&dataFim=" + dataFim);
        }
        
        function openRpt()
        {
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            openWindow(600, 800, "../Relatorios/RelBase.aspx?rel=InfoPedidosPeriodo&dataIni=" + dataIni + "&dataFim=" + dataFim);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Panel ID="panFiltro" runat="server">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label19" runat="server" ForeColor="#0066FF" Text="Período de consulta"></asp:Label>
                            </td>
                            <td>
                                <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                            </td>
                            <td>
                                <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                            </td>
                            <td>
                                <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                            </td>
                        </tr>
                    </table>
                    <br />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdInfoPedidos" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsInfoPedidos" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" OnRowDataBound="grdInfoPedidos_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Data" DataFormatString="{0:d}" HeaderText="Data" SortExpression="Data" />
                        <asp:BoundField DataField="TotalFastDelivery" HeaderText="m² Fast Delivery" ReadOnly="True"
                            SortExpression="TotalFastDelivery" />
                        <asp:BoundField DataField="TotalProducaoGeral" HeaderText="m² Produção" SortExpression="TotalProducaoGeral" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExibirPedidos" runat="server" ImageUrl="~/Images/book_go.png"
                                    ToolTip="Detalhes" OnClientClick='<%# "exibirDetalhes(\"" + Eval("Data", "{0:d}") + "\"); return false;" %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInfoPedidos" runat="server" SelectMethod="GetInfoPedidos"
                    TypeName="Glass.Data.RelDAL.InfoPedidosDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <br />
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false"><img border="0" 
                    src="../Images/Printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
