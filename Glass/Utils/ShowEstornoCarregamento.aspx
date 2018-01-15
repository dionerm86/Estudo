<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowEstornoCarregamento.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ShowEstornoCarregamento" Title="Estorno de carregamento " MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdLog" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsEstornoCarregamento" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Não há itens a serem exibidos."
                    PageSize="15">
                    <Columns>
                        <asp:BoundField DataField="CodInternoDescrPeca" HeaderText="Peça" SortExpression="CodInternoDescrPeca" />
                        <asp:BoundField DataField="Motivo" HeaderText="Motivo" SortExpression="Motivo" />
                        <asp:BoundField DataField="NomeFuncionario" HeaderText="Usuário" SortExpression="NomeFuncionario" />
                        <asp:BoundField DataField="DataCad" HeaderText="Data Estorno" SortExpression="DataCad" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsEstornoCarregamento" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="PesquisarPeloIdProdPedProducaoCount" SelectMethod="PesquisarPeloIdProdPedProducao" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.EstornoItemCarregamentoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="idProdPedProducao" QueryStringField="idProdPedProducao" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
