<%@ Page Title="Tipos de Perda" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="CadTipoPerda.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadTipoPerda" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdTipoPerda" runat="server" SkinID="gridViewEditable"
                    DataSourceID="odsTipoPerda" DataKeyNames="IdTipoPerda" EnableViewState="false" OnRowCommand="grdTipoPerda_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                <img border="0" src="../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                <asp:ImageButton ID="imbAtivarInativar" runat="server" CommandName="AtivarInativar" CommandArgument='<%# Eval("IdTipoPerda") %>'
                                    ImageUrl="~/Images/Inativar.gif" ToolTip="Ativar/Inativar" />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Tipo de Perda?&quot;);"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" />
                                <asp:HyperLink IdD="HyperLink1" runat="server" ImageUrl="~/Images/subgrupo.png" NavigateUrl='<%# "CadSubtipoPerda.aspx?idTipoPerda=" + Eval("IdTipoPerda") %>'
                                    ToolTip="Subtipos de Perda"></asp:HyperLink>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif"
                                    OnClientClick="return onSave(false);" ToolTip="Atualizar" ValidationGroup="c1" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descricao" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                                 <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescricao"
                                    Display="Dynamic" ErrorMessage="Informe uma descrição" SetFocusOnError="True"
                                    ToolTip="Informe uma descrição" ValidationGroup="c1">*</asp:RequiredFieldValidator>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                    ShowMessageBox="True" ShowSummary="False" ValidationGroup="c1" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="35" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescricao"
                                    Display="Dynamic" ErrorMessage="Informe uma descrição" SetFocusOnError="True"
                                    ToolTip="Informe uma descrição" ValidationGroup="c">*</asp:RequiredFieldValidator>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                    ShowMessageBox="True" ShowSummary="False" ValidationGroup="c" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Setor" SortExpression="DescrSetor">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSetor" runat="server" AppendDataBoundItems="True" DataSourceID="odsSetor"
                                    DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdSetor") %>'>
                                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpSetor" runat="server" DataSourceID="odsSetor" DataTextField="Name"
                                    DataValueField="Id" SelectedValue='<%# Bind("Id") %>' AppendDataBoundItems="True">
                                    <asp:ListItem Value="">Todos</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Setor") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="lblSituacao" runat="server" Text='<%# Eval("Situacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" DataSourceID="odsSituacao"
                                    DataTextField="Translation" DataValueField="Key" SelectedValue='<%# Bind("Situacao") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" DataSourceID="odsSituacao"
                                    DataTextField="Translation" DataValueField="Key" Enabled="false">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Exibir no Painel de Produção">
                            <ItemTemplate>
                                <asp:CheckBox ID="cbxExibirPainelProducao" runat="server" Checked='<%# Bind("ExibirPainelProducao") %>' Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="cbxExibirPainelProducao" runat="server" Checked='<%# Bind("ExibirPainelProducao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="cbxExibirPainelProducao" runat="server" Checked="true" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" IdRegistro='<%# (uint)(int)Eval("IdTipoPerda") %>'
                                    Tabela="TipoPerda" />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click" OnClientClick="return onSave(true);"
                                    ValidationGroup="c"><img border="0" src="../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoPerda" runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarTiposPerda" SortParameterName="sortExpression"
                    SelectByKeysMethod="ObtemTipoPerda"
                    TypeName="Glass.PCP.Negocios.IPerdaFluxo"
                    DataObjectTypeName="Glass.PCP.Negocios.Entidades.TipoPerda"
                    DeleteMethod="ApagarTipoPerda" 
                    DeleteStrategy="GetAndDelete"
                    UpdateMethod="SalvarTipoPerda"
                    UpdateStrategy="GetAndUpdate">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSetor" runat="server" 
                    SelectMethod="ObtemSetores" TypeName="Glass.PCP.Negocios.ISetorFluxo">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server"
                    SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.SituacaoTipoPerda, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
