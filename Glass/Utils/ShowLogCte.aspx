<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowLogCte.aspx.cs" Inherits="Glass.UI.Web.Utils.ShowLogCte"
    Title="Log do CTe n.º " MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdLog" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdLogCte" DataSourceID="odsLog" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhum evento registrado para este CTe." PageSize="15">
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
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLog" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="WebGlass.Business.ConhecimentoTransporte.LogCteOds">
        <SelectParameters>
            <asp:QueryStringParameter Name="idCte" QueryStringField="idCte" Type="UInt32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
