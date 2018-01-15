<%@ Page Title="Tipos de Funcionário" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadTipoFuncionario.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadTipoFuncionario" %>
<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdTipoFunc" runat="server" SkinID="gridViewEditable"
                              DataKeyNames="IdTipoFuncionario" DataSourceID="odsTipoFunc"
                              EnableViewState="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Tipo de Funcionário?&quot;);"
                                    ToolTip="Excluir" Visible='<%# Eval("TipoSistema") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cod." SortExpression="IdTipoFuncionario">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("IdTipoFuncionario") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("IdTipoFuncionario") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="35" ClientIDMode="Static"
                                    Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click"
                                    OnClientClick="return onSave(true);">
                                    <img border="0" src="../Images/insert.gif" />
                                </asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="TipoFuncionario" IdRegistro='<%# (uint)(int)Eval("IdTipoFuncionario") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoFunc" runat="server"
                    DeleteMethod="ApagarTipoFuncionario"
                    SelectMethod="PesquisarTiposFuncionario"
                    SelectByKeysMethod="ObtemTipoFuncionario"
                    UpdateStrategy="GetAndUpdate"
                    EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.IFuncionarioFluxo">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
