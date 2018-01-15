<%@ Page Title="Relação de Vendas" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="RelacaoVendas.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.RelacaoVendas" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        
        function openRpt(exportarExcel)
        {
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var agrupar = FindControl("drpAgrupar", "select").value;
            var ordenar = FindControl("drpOrdenar", "select").value;

            openWindow(600, 800, "RelBase.aspx?rel=RelacaoVendas&dataIni=" + dataIni + "&dataFim=" + dataFim + 
                "&situacao=" + situacao + "&agrupar=" + agrupar + "&exportarExcel=" + exportarExcel);
        }
        
        function openRptPedidos(idFunc, agrupar, exportarExcel)
        {
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
            var situacao = FindControl("drpSituacao", "select").value;
            var ordenar = FindControl("drpOrdenar", "select").value;
            
            openWindow(600, 800, "../RelBase.aspx?rel=semLucr&idLoja=0&idVend=" + idFunc + "&dtIni=" + dataIni + "&dtFim=" + dataFim + 
                "&agruparFunc=" + agrupar + "&situacao=" + situacao + "&ordenar=" + ordenar + "&exportarExcel=" + exportarExcel + "&tipoVenda=0&orderBy=0");
        }
        
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="right">
                            <asp:Label ID="lblPeriodo" runat="server" Text="Período Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="lblSituacao" runat="server" Text="Situação" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpSituacao" runat="server">
                                <asp:ListItem Value="5">Liberado</asp:ListItem>
                                <asp:ListItem Value="7">Confirmado</asp:ListItem>
                                <asp:ListItem Value="99">Confirmado/Liberado</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Agrupar Pedidos por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpAgrupar" runat="server">
                                <asp:ListItem Value="0">Vendedor (Assoc. Pedido)</asp:ListItem>
                                <asp:ListItem Value="1">Vendedor (Assoc. Cliente)</asp:ListItem>
                                <asp:ListItem Value="2">Comissionado</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Ordenar Pedidos por" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrdenar" runat="server">
                                <asp:ListItem Value="0">Vendedor (asc.)</asp:ListItem>
                                <asp:ListItem Value="1">Vendedor (desc.)</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                ToolTip="Pesquisar" />
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
                <asp:GridView GridLines="None" ID="grdRelacaoVendas" runat="server"
                    AutoGenerateColumns="False" DataSourceID="odsRelacaoVendas" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" 
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbRelatorio" runat="server" ImageUrl="~/Images/Relatorio.gif"
                                    ToolTip="Pedidos" OnClientClick='<%# "openRptPedidos(" + Eval("IdFunc") + ", " + Eval("Agrupar") + ", false); return false;" %>' />
                                <asp:LinkButton ID="lnkExportarExcel" runat="server" ToolTip="Pedidos" OnClientClick='<%# "openRptPedidos(" + Eval("IdFunc") + ", " + Eval("Agrupar") + ", true); return false;" %>'><img border="0" 
                                    src="../../Images/Excel.gif" /></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Vendedor" HeaderText="Vendedor" SortExpression="Vendedor" />
                        <asp:BoundField DataField="NumPedidos" HeaderText="Qtd. Pedidos" SortExpression="NumPedidos" />
                        <asp:BoundField DataField="TotM2" HeaderText="Total m²" SortExpression="TotM2" />
                        <asp:BoundField DataField="ValorTotal" DataFormatString="{0:c}" HeaderText="Valor da Venda"
                            SortExpression="ValorTotal" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRelacaoVendas" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.RelDAL.RelacaoVendasDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="drpSituacao" Name="situacao" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpAgrupar" Name="agruparFunc" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="drpOrdenar" Name="ordenar" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lkbImprimir" runat="server" OnClientClick="openRpt(false); return false;">
                    <img src="../../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" OnClientClick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
