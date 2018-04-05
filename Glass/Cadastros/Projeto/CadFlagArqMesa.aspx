<%@ Page Title="Flag Arquivo de Mesa" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadFlagArqMesa.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadFlagArqMesa" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

     <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFlag" runat="server" SkinID="gridViewEditable"
                    DataSourceID="odsFlag" DataKeyNames="IdFlagArqMesa" EnableViewState="False">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                <img border="0" src="../../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Flag?&quot;);"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif"
                                    OnClientClick="return onSave(false);" ToolTip="Atualizar" ValidationGroup="c1" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód" SortExpression="IdFlagArqMesa">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdFlagArqMesa") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descricao" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                                 <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescricao"
                                    Display="Dynamic" ErrorMessage="Informe uma descrição" SetFocusOnError="True"
                                    ToolTip="Informe uma descrição" ValidationGroup="c1">*</asp:RequiredFieldValidator>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                    ShowMessageBox="True" ShowSummary="False" ValidationGroup="c1" />--%>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="35" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescricao"
                                    Display="Dynamic" ErrorMessage="Informe uma descrição" SetFocusOnError="True"
                                    ToolTip="Informe uma descrição" ValidationGroup="c">*</asp:RequiredFieldValidator>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                    ShowMessageBox="True" ShowSummary="False" ValidationGroup="c" />--%>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Flag Padrão" SortExpression="Padrao">
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkPadrao" runat="server" Checked='<%# Bind("Padrao") %>'></asp:CheckBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblPadrao" runat="server" Text='<%# ((bool)Eval("Padrao") ? "Sim" : "Não") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkPadrao" runat="server" Checked='<%# Bind("Padrao") %>'></asp:CheckBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo de Arquivo" SortExpression="TipoArquivo">
                            <EditItemTemplate>
                                 <sync:CheckBoxListDropDown runat="server" ID="drpTipoArquivo" DataSourceID="odsTipoArquivoMesaCorte"
                                    DataValueField="Value" DataTextField="Translation" Title="Selecione o tipo de arquivo"
                                     SelectedValues='<%# Bind("TipoArquivoArr") %>'>
                                </sync:CheckBoxListDropDown>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblTipoArquivo" runat="server" Text='<%# Eval("TipoArquivoDescr") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <sync:CheckBoxListDropDown runat="server" ID="drpTipoArquivo" DataSourceID="odsTipoArquivoMesaCorte"
                                    DataValueField="Value" DataTextField="Translation" Title="Selecione o tipo de arquivo"
                                    SelectedValues='<%# Bind("TipoArquivoArr") %>'>
                                </sync:CheckBoxListDropDown>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="lblSituacao" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>'></asp:Label>
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
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click" OnClientClick="return onSave(true);"
                                    ValidationGroup="c"><img border="0" src="../../Images/insert.gif" alt="Inserir" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFlag" runat="server" 
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression"
                    SelectMethod="PesquisarFlag"
                    SelectByKeysMethod="ObtemFlag"
                    TypeName="Glass.Projeto.Negocios.IFlagArqMesaFluxo"
                    DataObjectTypeName="Glass.Projeto.Negocios.Entidades.FlagArqMesa"
                    DeleteMethod="ApagarFlagArqMesa" 
                    DeleteStrategy="GetAndDelete"
                    UpdateMethod="SalvarFlagArqMesa"
                    UpdateStrategy="GetAndUpdate">
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource runat="server" ID="odsTipoArquivoMesaCorte" SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Data.Model.TipoArquivoMesaCorte, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsSituacao" runat="server"
                    SelectMethod="GetTranslatesFromTypeName" TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" DefaultValue="Glass.Situacao, Glass.Comum" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
