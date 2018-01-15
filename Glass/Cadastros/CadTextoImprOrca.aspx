<%@ Page Title="Cadastro de Textos de Orçamento" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadTextoImprOrca.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTextoImprOrca" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left" class="dtvHeader">
                            <asp:Label ID="Label1" runat="server" Text="Título"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtTitulo" runat="server" MaxLength="50" Width="150px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="dtvHeader" align="left">
                            <asp:Label ID="Label2" runat="server" Text="Descrição"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="1500" Rows="5" TextMode="MultiLine"
                                Width="600px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="dtvHeader" align="center">
                            <asp:Label ID="Label3" runat="server" Text="Buscar Sempre"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:CheckBox ID="chkBuscarSempre" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="btnInserir" runat="server" Text="Inserir" OnClick="btnInserir_Click" />
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
                <asp:GridView GridLines="None" ID="grdTextoImprOrca" runat="server" AllowPaging="True" AllowSorting="True"
                    AutoGenerateColumns="False" DataSourceID="odsTextoImprOrca" EmptyDataText="Nenhum texto de orçamento cadastrado."
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdTextoImprOrca">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif"
                                    ToolTip="Editar" CausesValidation="False" />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" OnClientClick="return confirm('Tem certeza que deseja excluir este texto?');"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Título" SortExpression="Titulo">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtTitulo" runat="server" MaxLength="50" Text='<%# Bind("Titulo") %>'
                                    Width="150px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Titulo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="1000" Rows="5" Text='<%# Bind("Descricao") %>'
                                    TextMode="MultiLine" Width="600px"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Buscar Sempre" SortExpression="BuscarSempre">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" 
                                    Checked='<%# Bind("BuscarSempre") %>' Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" 
                                    Checked='<%# Bind("BuscarSempre") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>

<PagerStyle CssClass="pgr"></PagerStyle>

<EditRowStyle CssClass="edit"></EditRowStyle>

<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTextoImprOrca" runat="server" DataObjectTypeName="Glass.Data.Model.TextoImprOrca"
                    DeleteMethod="Delete" SelectMethod="GetList" TypeName="Glass.Data.DAL.TextoImprOrcaDAO"
                    UpdateMethod="Update" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    OnDeleted="odsTextoImprOrca_Deleted" OnUpdated="odsTextoImprOrca_Updated" SelectCountMethod="GetCount"
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow"></colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
