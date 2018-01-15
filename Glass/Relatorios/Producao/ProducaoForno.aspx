<%@ Page Title="Produção do Forno" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="ProducaoForno.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.ProducaoForno" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <script type="text/javascript">
    
    function openRpt(exportarExcel)
    {
        var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
        var dataFim = FindControl("ctrlDataFim_txtData", "input").value;
        
        openWindow(600, 800, "RelBase.aspx?rel=ProducaoForno&dataIni=" + dataIni + "&dataFim=" + dataFim + "&exportarExcel=" + exportarExcel);
    }
    
    </script>
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblPeriodo" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td align="left" nowrap="nowrap">
                            <asp:ImageButton ID="imgPesq2" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" CausesValidation="False" />
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
                <asp:GridView GridLines="None" ID="grdProducaoForno" runat="server" AutoGenerateColumns="False" DataSourceID="odsProducaoForno" 
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AllowPaging="True" PageSize="15">
                    <Columns>
                        <asp:BoundField DataField="Data" HeaderText="Data" SortExpression="Data" 
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="TotM2PedidoVenda" HeaderText="Pedido" 
                            SortExpression="TotM2PedidoVenda" />
                        <asp:BoundField DataField="TotM2PedidoProducao" 
                            HeaderText="Estoque" SortExpression="TotM2PedidoProducao" />
                        <asp:TemplateField SortExpression="TotM2PrimSetor">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("TotM2PrimSetor") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TotM2Corte") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <HeaderTemplate>
                                <asp:Label ID="lblNomePrimSetor" runat="server" 
                                    onprerender="lblNomePrimSetor_PreRender"></asp:Label>
                            </HeaderTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="TotM2FornoProducao" HeaderText="Produção" 
                            SortExpression="TotM2FornoProducao" />
                        <asp:BoundField DataField="TotM2FornoPerda" HeaderText="Perda" 
                            SortExpression="TotM2FornoPerda" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsProducaoForno" runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" 
                    SelectMethod="GetList" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.RelDAL.ProducaoFornoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" 
                            Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" 
                            Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkImprimir" runat="server" 
                    onclientclick="openRpt(false); return false;"><img border="0" 
                    src="../../Images/Printer.png" /> Imprimir</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lnkExportarExcel" runat="server" 
                    onclientclick="openRpt(true); return false;"><img border="0" 
                    src="../../Images/Excel.gif" /> Exportar para o Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>

