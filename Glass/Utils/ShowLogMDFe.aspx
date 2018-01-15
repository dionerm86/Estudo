<%@ Page Title="Log do MDFe n.º " Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true"
    CodeBehind="ShowLogMDFe.aspx.cs" Inherits="Glass.UI.Web.Utils.ShowLogMDFe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="javaScript" runat="server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="Menu" runat="server">
</asp:Content>
<asp:Content ID="pagina" ContentPlaceHolderID="Pagina" runat="server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdLog" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataKeyNames="IdLogMDFe" DataSourceID="odsLog" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhum evento registrado para este MDFe." PageSize="15">
                    <Columns>
                        <asp:BoundField DataField="Codigo" HeaderText="Código" SortExpression="Codigo" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                        <asp:BoundField DataField="DataHora" HeaderText="Data" SortExpression="DataHora" DataFormatString="{0:d}" />
                        <asp:BoundField DataField="DataHora" HeaderText="Hora" SortExpression="DataHora" DataFormatString="{0:t}"></asp:BoundField>
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
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLog" runat="server" EnablePaging="True"
        SortParameterName="sortExpression" StartRowIndexParameterName="startRow" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCount" SelectMethod="GetList"
        TypeName="Glass.Data.DAL.LogMDFeDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="idManifestoEletronico" QueryStringField="IdManifestoEletronico" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
