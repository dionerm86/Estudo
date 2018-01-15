<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowLogAlteracao.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ShowLogAlteracao" Title="Log de Alterações: " MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <style type="text/css">
        .pre
        {
            white-space: pre-wrap;
        }
    </style>
    <table>
        <tr>
            <td align="center" class="subtitle1">
                Referência:
                <asp:Label ID="lblSubtitle" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr runat="server" id="filtroCampo">
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" ForeColor="#0066FF" Text="Campo"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpCampo" runat="server" AppendDataBoundItems="True" AutoPostBack="True"
                                DataSourceID="odsCampos" DataTextField="Value" DataValueField="Key">
                                <asp:ListItem Value="">Todos</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdLog" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsLog" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" PageSize="15"
                    OnRowDataBound="grdLog_RowDataBound" EmptyDataText="Não há itens para esse filtro.">
                    <Columns>
                        <asp:BoundField DataField="DataAlt" HeaderText="Data" SortExpression="DataAlt" />
                        <asp:BoundField DataField="NomeFuncAlt" HeaderText="Funcionário" SortExpression="NomeFuncAlt" />
                        <asp:BoundField DataField="Campo" HeaderText="Campo" SortExpression="Campo" />
                        <asp:BoundField DataField="ValorAnterior" HeaderText="Anterior" SortExpression="ValorAnterior">
                            <ItemStyle CssClass="pre" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ValorAtual" HeaderText="Atual" SortExpression="ValorAtual">
                            <ItemStyle CssClass="pre" />
                        </asp:BoundField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLog" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.LogAlteracaoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="tabela" QueryStringField="tabela" Type="Int32" />
            <asp:QueryStringParameter Name="idRegistroAlt" QueryStringField="id" Type="UInt32" />
            <asp:ControlParameter ControlID="hdfExibirAdmin" Name="exibirAdmin" PropertyName="Value"
                Type="Boolean" />
            <asp:ControlParameter ControlID="drpCampo" Name="campo" PropertyName="SelectedValue"
                Type="String" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCampos" runat="server" SelectMethod="GetCampos" TypeName="Glass.Data.DAL.LogAlteracaoDAO"
        OnSelected="odsCampos_Selected">
        <SelectParameters>
            <asp:QueryStringParameter Name="tabela" QueryStringField="tabela" Type="Int32" />
            <asp:QueryStringParameter Name="idRegistroAlt" QueryStringField="id" Type="UInt32" />
            <asp:QueryStringParameter Name="campo" QueryStringField="campo" Type="String" />
            <asp:ControlParameter ControlID="hdfExibirAdmin" Name="exibirAdmin" PropertyName="Value"
                Type="Boolean" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
    <asp:HiddenField ID="hdfExibirAdmin" runat="server" Value="False" />
</asp:Content>
