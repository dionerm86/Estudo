<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelTextoOrca.aspx.cs" Inherits="Glass.UI.Web.Utils.SelTextoOrca"
    Title="Textos para o Orçamento n.º" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <asp:Label ID="Label2" runat="server" CssClass="subtitle1" Text="Textos Disponíveis"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdTextoImprOrca" runat="server" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataSourceID="odsTextoImprOrca"
                    EmptyDataText="Nenhum texto de orçamento cadastrado." DataKeyNames="IdTextoImprOrca"
                    PageSize="5" OnRowCommand="grdTextoImprOrca_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkAdd" runat="server" CommandArgument='<%# Eval("IdTextoImprOrca") %>'
                                    CommandName="IncluiTexto">
                                     <img src="../Images/Insert.gif" border="0"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Título" SortExpression="Titulo">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTitulo" runat="server" MaxLength="50" Text='<%# Bind("Titulo") %>'
                                    Width="150px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Titulo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="1000" Rows="5" Text='<%# Bind("Descricao") %>'
                                    TextMode="MultiLine" Width="600px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTextoImprOrca" runat="server" DataObjectTypeName="Glass.Data.Model.TextoImprOrca"
                    DeleteMethod="Delete" SelectMethod="GetList" TypeName="Glass.Data.DAL.TextoImprOrcaDAO"
                    UpdateMethod="Update" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetCount" SortParameterName="sortExpression" StartRowIndexParameterName="startRow">
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
                <asp:Label ID="Label1" runat="server" CssClass="subtitle1" Text="Textos Selecionados"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdTextoOrca" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdTextoOrcamento" DataSourceID="odsTextoOrcamento" EmptyDataText="Nenhum texto selecionado."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Titulo" HeaderText="Título" SortExpression="Titulo" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao" />
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTextoOrcamento" runat="server" DataObjectTypeName="Glass.Data.Model.TextoOrcamento"
                    DeleteMethod="Delete" SelectMethod="GetByOrcamento" TypeName="Glass.Data.DAL.TextoOrcamentoDAO">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idOrcamento" QueryStringField="idOrca" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
