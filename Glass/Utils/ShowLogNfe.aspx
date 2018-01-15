<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowLogNfe.aspx.cs" Inherits="Glass.UI.Web.Utils.ShowLogNfe"
    Title="Log da NF-e n.º " MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdLog" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdLogNf" DataSourceID="odsLog" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhum evento registrado para esta NFe." PageSize="15">
                    <Columns>
                        <asp:BoundField DataField="Codigo" HeaderText="Código" SortExpression="Codigo" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="DataHora" HeaderText="Data" SortExpression="DataHora"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DataHora" HeaderText="Hora" SortExpression="DataHora"
                            DataFormatString="{0:t}"></asp:BoundField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblTituloMotivo" runat="server" Font-Bold="True"></asp:Label>
                &nbsp;<asp:Label ID="lblTextoMotivo" runat="server" Font-Bold="False"></asp:Label>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td align="center">
                <asp:Label ID="lblTituloSeparacaoValores" runat="server" CssClass="subtitle"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdLogSeparacaoValores" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdLogNf" DataSourceID="odsLogSeparacaoValores" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhum evento registrado para esta NFe." PageSize="15">
                    <Columns>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="NomeFuncionario" HeaderText="Funcionário" SortExpression="NomeFuncionario" />
                        <asp:BoundField DataField="DataHora" HeaderText="Data" SortExpression="DataHora"
                            DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DataHora" HeaderText="Hora" SortExpression="DataHora"
                            DataFormatString="{0:t}"></asp:BoundField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLog" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.LogNfDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
            <asp:Parameter Name="separacaoValores" Type="Boolean" DefaultValue="False" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLogSeparacaoValores" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.LogNfDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
            <asp:Parameter Name="separacaoValores" Type="Boolean" DefaultValue="True" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
