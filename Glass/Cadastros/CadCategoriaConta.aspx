<%@ Page Title="Categorias do Plano de Contas" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadCategoriaConta.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadCategoriaConta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onSave() {
            if (FindControl("txtDescricao", "input").value == "") {
                alert("Informe a descrição");
                return false;
            }
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:GridView SkinID="gridViewEditable" ID="grdCategoriaConta" runat="server" 
                    DataKeyNames="IdCategoriaConta" DataSourceID="odsCategoriaConta" 
                    EnableViewState="false" 
                    OnRowCommand="grdCategoriaConta_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Grupo?&quot;);"
                                    ToolTip="Excluir" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfNumSeq" runat="server" Value='<%# Bind("NumeroSequencia") %>' />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" Text='<%# Bind("Descricao") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" Width="200px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tipo" SortExpression="Tipo">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Tipo")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" 
                                                  SelectedValue='<%# Bind("Tipo") %>'
                                                  DataSourceID="odsTipoCategoria" 
                                                  DataValueField="Key" DataTextField="Translation"
                                                  AppendDataBoundItems="true">
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpTipo" runat="server" 
                                                  DataSourceID="odsTipoCategoria" 
                                                  DataValueField="Key" DataTextField="Translation"
                                                  AppendDataBoundItems="true">                                   
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server">
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgUp" runat="server" CommandArgument='<%# Eval("IdCategoriaConta") %>'
                                    CommandName="Up" ImageUrl="~/Images/up.gif" Visible='<%# Glass.Conversoes.StrParaInt(Eval("NumeroSequencia").ToString()) > 1 %>' />
                                <asp:ImageButton ID="imgDown" runat="server" CommandArgument='<%# Eval("IdCategoriaConta") %>'
                                    CommandName="Down" ImageUrl="~/Images/down.gif" />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click" OnClientClick="return onSave();">
                                    <img border="0" src="../Images/ok.gif" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource runat="server" ID="odsTipoCategoria" 
                    SelectMethod="GetTranslatesFromTypeName" EnableViewState="false"
                    TypeName="Colosoft.Translator">
                    <SelectParameters>
                        <asp:Parameter Name="typeName" Type="String"
                             DefaultValue="Glass.Data.Model.TipoCategoriaConta, Glass.Data" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCategoriaConta" runat="server" 
                    SelectMethod="PesquisarCategoriasConta" EnableViewState="false" 
                    SelectByKeysMethod="ObtemCategoriaConta"
                    DeleteMethod="ApagarCategoriaConta"
                    DeleteStrategy="GetAndDelete"
                    TypeName="Glass.Financeiro.Negocios.IPlanoContasFluxo"
                    DataObjectTypeName="Glass.Financeiro.Negocios.Entidades.CategoriaConta"
                    EnablePaging="True" 
                    MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression"
                    UpdateMethod="SalvarCategoriaConta"
                    UpdateStrategy="GetAndUpdate">
                    <DeleteParameters>
                        <asp:Parameter Name="IdCategoriaConta" Type="Int32" />
                    </DeleteParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
