<%@ Page Title="Relação de Box para Produção" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="RelacaoBoxProducao.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Producao.RelacaoBoxProducao" %>

<%@ Register Src="../../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openRpt()
        {
            var data = FindControl("ctrlData_txtData", "input").value;
            openWindow(600, 800, "RelBase.aspx?rel=RelacaoBoxProducao&data=" + data);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Data"></asp:Label>
                        </td>
                        <td>
                            <uc1:ctrlData ID="ctrlData" runat="server" ReadOnly="ReadWrite" ExibirHoras="False" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
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
                <asp:GridView GridLines="None" ID="grdRelacaoBox" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsRelacaoBox" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" AllowPaging="True" AllowSorting="True" EmptyDataText="Não foram encontrados box produzidos nessa data."
                    PageSize="15">
                    <Columns>
                        <asp:BoundField DataField="Modelo" HeaderText="Modelo" SortExpression="Modelo" />
                        <asp:BoundField DataField="Altura" HeaderText="Altura" SortExpression="Altura" />
                        <asp:BoundField DataField="Largura" HeaderText="Largura" SortExpression="Largura" />
                        <asp:BoundField DataField="TotM" HeaderText="Área" ReadOnly="True" SortExpression="TotM" />
                        <asp:BoundField DataField="DescrCorVidro" HeaderText="Cor" SortExpression="DescrCorVidro" />
                        <asp:BoundField DataField="QtdeAnterior" HeaderText="Dia anterior" SortExpression="QtdeAnterior" />
                        <asp:BoundField DataField="Qtde" HeaderText="Dia atual" SortExpression="Qtde" />
                        <asp:BoundField DataField="QtdeProduzir" HeaderText="A produzir" SortExpression="QtdeProduzir" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsRelacaoBox" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.RelDAL.RelacaoBoxProducaoDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ctrlData" Name="data" PropertyName="DataString" Type="String" />
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
                <asp:LinkButton ID="lnkImprimir" runat="server" OnClientClick="openRpt(); return false;"><img border="0" 
                    src="../../Images/Printer.png" /> Imprimir</asp:LinkButton>
            </td>
        </tr>
    </table>
</asp:Content>
