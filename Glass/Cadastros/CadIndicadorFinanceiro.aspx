<%@ Page Title="Indicadores Financeiros" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadIndicadorFinanceiro.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadIndicadorFinanceiro" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/wz_tooltip.js?v=" + Glass.Configuracoes.Geral.ObtemVersao()) %>'></script>

    <div class="tabela-responsiva">
        <table>
            <tr>
                <td align="center">
                    <asp:GridView ID="grdIndicadoresFinanceiros" runat="server" SkinID="gridViewEditable"
                        DataSourceID="odsIndicadoresFinanceiros" DataKeyNames="IdIndicadorFinanceiro" PageSize="15">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                    <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                        ToolTip="Excluir" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este indicador financeiro?&quot;);" />
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
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtNome1" runat="server" Text='<%# Bind("Nome") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblNome" runat="server" Text='<%# Bind("Nome") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtNome" runat="server" MaxLength="150" Width="150px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtDescricao" runat="server" MaxLength="150" Width="150px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtValor1" runat="server" Text='<%# Bind("Valor") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblValor" runat="server" Text='<%# Bind("Valor") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtValor" runat="server" MaxLength="150" Width="150px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Formatação" SortExpression="Formatacao">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtFormatacao" runat="server" Text='<%# Bind("Formatacao") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblFormatacao" runat="server" Text='<%# Bind("Formatacao") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtFormatacao" runat="server" MaxLength="150" Width="150px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                           
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="IndicadorFinanceiro" IdRegistro='<%# (int)Eval("IdIndicadorFinanceiro") %>' />
                                </ItemTemplate>
                                <FooterTemplate>
                                            <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">
                                            <img border="0" src="../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                                        </td> 
                                    </tr>
                                </FooterTemplate>
                                <FooterStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsIndicadoresFinanceiros" runat="server"
                        DataObjectTypeName="Glass.Rentabilidade.Negocios.Entidades.IndicadorFinanceiro"
                        DeleteMethod="ApagarIndicadorFinanceiro" EnablePaging="True"
                        DeleteStrategy="GetAndDelete"
                        MaximumRowsParameterName="pageSize"
                        SelectMethod="PesquisaIndicadoresFinanceiros" SortParameterName="sortExpression"
                        SelectByKeysMethod="ObterIndicadorFinanceiro"
                        TypeName="Glass.Rentabilidade.Negocios.IRentabilidadeFluxo"
                        UpdateMethod="SalvarIndicadorFinanceiro"
                        UpdateStrategy="GetAndUpdate">
                        <SelectParameters>
                            <asp:Parameter Name="nome" DefaultValue="" />
                        </SelectParameters>
                    </colo:VirtualObjectDataSource>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
