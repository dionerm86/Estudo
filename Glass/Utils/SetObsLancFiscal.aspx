<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetObsLancFiscal.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetObsLancFiscal" Title="Observações do Lançamento Fiscal" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <span class="subtitle1">Observações Disponíveis </span>
                <asp:GridView ID="grdObsLancFiscal" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdObsLancFiscal"
                    DataSourceID="odsObsLancFiscal" EmptyDataText="Não há observações a adicionar."
                    GridLines="None" OnRowCommand="grdObsLancFiscal_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgAdicionar" runat="server" CommandArgument='<%# Eval("IdObsLancFiscal") %>'
                                    CommandName="Adicionar" ImageUrl="~/Images/Insert.gif" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsObsLancFiscal" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCountReal" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ObsLancFiscalDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idNfAdd" QueryStringField="idNf" Type="UInt32" />
                        <asp:QueryStringParameter Name="idCteAdd" QueryStringField="idCte" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr runat="server" id="nf">
            <td align="center">
                <span class="subtitle1">Observações na Nota Fiscal</span>
                <asp:GridView ID="grdObsLancFiscalNf" runat="server" AllowPaging="True" AllowSorting="True"
                    CssClass="gridStyle" EmptyDataText="Ainda não há observações fiscais na nota fiscal."
                    GridLines="None" AutoGenerateColumns="False" DataSourceID="odsObsLancFiscalNf"
                    DataKeyNames="IdNf,IdObsLancFiscal">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsObsLancFiscalNf" runat="server" DataObjectTypeName="Glass.Data.Model.ObsLancFiscalNf"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ObsLancFiscalNfDAO"
                    OnDeleted="odsObsLancFiscalNf_Deleted">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idNf" QueryStringField="idNf" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr runat="server" id="ct">
            <td align="center">
                <span class="subtitle1">Observações no Conhecimento de Transporte</span>
                <asp:GridView ID="grdObsLancFiscalCte" runat="server" AllowPaging="True" AllowSorting="True"
                    CssClass="gridStyle" EmptyDataText="Ainda não há observações fiscais no conhecimento de transporte."
                    GridLines="None" AutoGenerateColumns="False" DataSourceID="odsObsLancFiscalCte"
                    DataKeyNames="IdCte,IdObsLancFiscal">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsObsLancFiscalCte" runat="server" DataObjectTypeName="Glass.Data.Model.ObsLancFiscalCte"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.ObsLancFiscalCteDAO"
                    OnDeleted="odsObsLancFiscalCte_Deleted">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idCte" QueryStringField="idCte" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
