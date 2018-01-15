<%@ Page Title="Movimentações Financeiras" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="MovimentacoesFinanceiras.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Administrativos.MovimentacoesFinanceiras" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt()
        {
            var dataIni = FindControl("ctrlDataIni_txtData", "input").value;
            var dataFim = FindControl("ctrlDataFim_txtData", "input");
            dataFim = dataFim != null ? dataFim.value : dataIni;
            var detalhado = FindControl("chkDetalhado", "input").checked;
            
            openWindow(600, 800, "RelBase.aspx?rel=MovimentacoesFinanceiras&dataIni=" + dataIni + "&dataFim=" + dataFim +
                "&detalhado=" + detalhado);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblData" runat="server" ForeColor="#0066FF" Text="Período"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlDataIni" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td id="dataFim" runat="server">
                            <uc1:ctrlData ID="ctrlDataFim" runat="server" ReadOnly="ReadWrite" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkDetalhado" runat="server" Text="Relatório detalhado" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView GridLines="None" ID="grdMovFinanc" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsMovFinanc" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AllowPaging="True" AllowSorting="True" PageSize="20">
                    <Columns>
                        <asp:BoundField DataField="NomeMov" HeaderText="Movimentação" SortExpression="NomeMov" />
                        <asp:BoundField DataField="SaldoAnteriorDia" HeaderText="Saldo Anterior" SortExpression="SaldoAnteriorDia"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="EntradasDia" HeaderText="Entradas" SortExpression="EntradasDia"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="SaidasDia" HeaderText="Saídas" SortExpression="SaidasDia"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="SaldoDia" DataFormatString="{0:c}" HeaderText="Saldo"
                            SortExpression="SaldoDia" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <asp:GridView GridLines="None" ID="grdMovFinancDet" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsMovFinanc" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AllowPaging="True" AllowSorting="True" PageSize="20"
                    Visible="False">
                    <Columns>
                        <asp:BoundField DataField="NomeMov" HeaderText="Movimentação" SortExpression="NomeMov" />
                        <asp:TemplateField HeaderText="Referente a" SortExpression="DescrCategoriaConta, DescrGrupoConta, DescrPlanoConta">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("DescrPlanoConta") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("DescrCategoriaConta") + " - " + Eval("DescrGrupoConta") + " - " + Eval("DescrPlanoConta") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="EntradasDia" HeaderText="Entradas" SortExpression="EntradasDia"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="SaidasDia" HeaderText="Saídas" SortExpression="SaidasDia"
                            DataFormatString="{0:c}" />
                        <asp:BoundField DataField="MovimentoDia" DataFormatString="{0:c}" HeaderText="Saldo"
                            SortExpression="MovimentoDia" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMovFinanc" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.RelDAL.MovimentacoesFinanceirasDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlDataIni" Name="dataIni" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="ctrlDataFim" Name="dataFim" PropertyName="DataString" Type="String" />
                        <asp:ControlParameter ControlID="chkDetalhado" Name="detalhado" PropertyName="Checked"
                            Type="Boolean" />
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
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false"> <img src="../../Images/Printer.png" border="0" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
