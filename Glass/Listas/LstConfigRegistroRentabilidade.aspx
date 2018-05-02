<%@ Page Title="Configuração Relatório Rentabilidade" Language="C#" AutoEventWireup="true" MasterPageFile="~/Painel.master"
    CodeBehind="LstConfigRegistroRentabilidade.aspx.cs" Inherits="Glass.UI.Web.Listas.LstConfigRegistroRentabilidade" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()) %>'></script>

    <div class="tabela-responsiva">
        <table>
            <tr>
                <td align="center">
                    <asp:GridView ID="grdConfigRegistroRentabilidade" runat="server" SkinID="defaultGridView"
                        DataSourceID="odsConfigRegistroRentabilidade" DataKeyNames="Tipo, IdRegistro"
                        OnRowCommand="grdConfigRegistroRentabilidade_RowCommand">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" Height="16px"
                                        ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                    <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                        ToolTip="Cancelar" />
                                </EditItemTemplate>
                                <ItemStyle Wrap="False" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Nome" SortExpression="Nome">
                                <ItemTemplate>
                                    <asp:Label ID="lblNome" runat="server" Text='<%# Eval("Nome") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                                <ItemTemplate>
                                    <asp:Label ID="lblDescricao" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Tipo">
                                <ItemTemplate>
                                    <asp:Label ID="lblTipo" runat="server" Text='<%# Eval("DescricaoTipo") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Exibir Relatório" SortExpression="">
                                <EditItemTemplate>
                                    <asp:CheckBox ID="chkSomaFormulaRentabilidade" runat="server" Checked='<%# Bind("ExibirRelatorio") %>' />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="CheckBox2" runat="server" Checked='<%# Bind("ExibirRelatorio") %>'
                                        Enabled="False" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>

                            <asp:TemplateField>
                                <EditItemTemplate>

                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton ID="imgUp" runat="server" CommandArgument='<%# Eval("Tipo") + "x" + Eval("IdRegistro")  %>'
                                        CommandName="Up" ImageUrl="~/Images/up.gif" Visible='<%# Eval("PodeMoverParaCima") %>' />
                                    &nbsp;<asp:ImageButton ID="imgDown" runat="server" CommandArgument='<%# Eval("Tipo") + "x" + Eval("IdRegistro") %>'
                                        CommandName="Down" ImageUrl="~/Images/down.gif"  Visible='<%# Eval("PodeMoverParaBaixo") %>' />
                                </ItemTemplate>
                                <ItemStyle Wrap="False" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsConfigRegistroRentabilidade" runat="server"
                        DataObjectTypeName="Glass.Rentabilidade.Negocios.Entidades.ConfigRegistroRentabilidade"
                        EnablePaging="True"
                        MaximumRowsParameterName="pageSize"
                        SelectMethod="ObterConfigsRegistroRentabilidade" SortParameterName="sortExpression"
                        SelectByKeysMethod="ObterConfigRegistroRentabilidade"
                        TypeName="Glass.Rentabilidade.Negocios.IRentabilidadeFluxo"
                        UpdateMethod="SalvarConfigRegistroRentabilidade"
                        UpdateStrategy="GetAndUpdate">
                    </colo:VirtualObjectDataSource>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
