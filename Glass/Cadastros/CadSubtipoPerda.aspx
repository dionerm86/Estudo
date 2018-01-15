<%@ Page Title="Subtipos de Perda" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadSubtipoPerda.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadSubtipoPerda" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center" class="subtitle1">
                Tipo de perda:
                <colo:ItemDetailsView runat="server" DataSourceID="odsTipoPerda" EnableViewState="false">
                    <ItemTemplate>
                        <%# Eval("Descricao") %>
                    </ItemTemplate>
                </colo:ItemDetailsView>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:HyperLink ID="lnkVoltar" runat="server" NavigateUrl="~/Cadastros/CadTipoPerda.aspx">Voltar</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdSubtipoPerda" runat="server" SkinID="gridViewEditable"
                    DataSourceID="odsSubtipoPerda" DataKeyNames="IdSubtipoPerda" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse subtipo de perda?&quot;)) return false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" ValidationGroup="c1" CommandName="Update"
                                    ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" Text='<%# Bind("Descricao") %>'
                                    Width="200px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescricao"
                                    Display="Dynamic" ErrorMessage="Informe uma descrição" SetFocusOnError="True"
                                    ToolTip="Informe uma descrição" ValidationGroup="c1">*</asp:RequiredFieldValidator>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                    ShowMessageBox="True" ShowSummary="False" ValidationGroup="c1" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" Text='<%# Bind("Descricao") %>'
                                    Width="200px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDescricao"
                                    Display="Dynamic" ErrorMessage="Informe uma descrição" SetFocusOnError="True"
                                    ToolTip="Informe uma descrição" ValidationGroup="c">*</asp:RequiredFieldValidator>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                    ShowMessageBox="True" ShowSummary="False" ValidationGroup="c" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="SubtipoPerda" IdRegistro='<%# (uint)(int)Eval("IdSubtipoPerda") %>' />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ValidationGroup="c" ImageUrl="~/Images/Insert.gif"
                                    OnClick="imgAdd_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsSubtipoPerda" runat="server" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarSubtiposPerda" SortParameterName="sortExpression"
                    SelectByKeysMethod="ObtemSubtipoPerda"
                    TypeName="Glass.PCP.Negocios.IPerdaFluxo"
                    DataObjectTypeName="Glass.PCP.Negocios.Entidades.SubtipoPerda"
                    DeleteMethod="ApagarSubtipoPerda" 
                    DeleteStrategy="GetAndDelete"
                    UpdateMethod="SalvarSubtipoPerda"
                    UpdateStrategy="GetAndUpdate" >
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idTipoPerda" QueryStringField="idTipoPerda" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsTipoPerda" runat="server" EnablePaging="true"
                    SelectMethod="ObtemTipoPerda" TypeName="Glass.PCP.Negocios.IPerdaFluxo">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idTipoPerda" QueryStringField="idTipoPerda" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
