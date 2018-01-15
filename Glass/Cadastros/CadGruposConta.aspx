<%@ Page Title="Grupos do Plano de Contas" Language="C#" MasterPageFile="~/Painel.master"
    AutoEventWireup="true" CodeBehind="CadGruposConta.aspx.cs" Inherits="Glass.UI.Web.Cadastros.CadGruposConta" %>

<%@ Register Src="~/Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">

        function onSave() {
            if (FindControl("txtDescricao", "input").value == "") {
                alert("Informe a descrição");
                return false;
            }
        }

        function SetPontoEquilibrio(idGrupo, controle) {
            
            var retorno = CadGruposConta.SetPontoEquilibrio(idGrupo, controle.checked);

            if (retorno.error != null) {
                alert(retorno.error.description);
                return false;
            }
            else if (retorno.value.split('|')[0] == "Erro"){
                alert(retorno.value.split('|')[1]);
                controle.checked = !controle.checked;
                return false;
            }
            
            alert(retorno.value.split('|')[0]);
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:GridView ID="grdGruposConta" runat="server" DataKeyNames="IdGrupo" DataSourceID="odsGruposConta"
                              SkinID="gridViewEditable"
                              onrowcommand="grdGruposConta_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../Images/Edit.gif" alt="Editar" /></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Grupo?&quot;);"
                                    ToolTip="Excluir" Visible='<%# Eval("GrupoContaSistema") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" Height="16px"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Categoria" SortExpression="Categoria">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("Categoria") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:HiddenField ID="hdfIdCategoria" runat="server" Value='<%# Eval("IdCategoriaConta") %>' />
                                <asp:DropDownList ID="drpCategoria" runat="server" AppendDataBoundItems="True" DataSourceID="odsCategoriaConta" OnDataBinding="drpCategoria_DataBinding"
                                    DataTextField="Name" DataValueField="Id" SelectedValue='<%# Bind("IdCategoriaConta") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpCategoria" runat="server" AppendDataBoundItems="True" DataSourceID="odsCategoriaConta"
                                    DataTextField="Name" DataValueField="Id">
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo" SortExpression="IdGrupo">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("IdGrupo") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("IdGrupo") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" Width="200px"></asp:TextBox>
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
                        <asp:TemplateField HeaderText="Ponto de Equilíbrio" SortExpression="PontoEquilibrio">
                            <ItemTemplate>
                                <asp:CheckBox ID="ckbPontoEquilibrio" runat="server" Checked='<%# Bind("PontoEquilibrio") %>'
                                    onclick='<%# "SetPontoEquilibrio(" + Eval("IdGrupo") + ", this);" %>' />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="GrupoConta" IdRegistro='<%# (int)Eval("IdGrupo") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgUp" runat="server" CommandArgument='<%# Eval("IdGrupo") %>'
                                    CommandName="Up" ImageUrl="~/Images/up.gif" Visible='<%# ((int)Eval("NumeroSequencia")) > 1 %>' />
                                <asp:ImageButton ID="imgDown" runat="server" CommandArgument='<%# Eval("IdGrupo") %>'
                                    CommandName="Down" ImageUrl="~/Images/down.gif" />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click" OnClientClick="return onSave();">
                                    <img border="0" src="../Images/ok.gif" alt="Inserir" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGruposConta" runat="server" 
                    DataObjectTypeName="Glass.Financeiro.Negocios.Entidades.GrupoConta"
                    TypeName="Glass.Financeiro.Negocios.IPlanoContasFluxo"
                    DeleteMethod="ApagarGrupoConta" EnablePaging="True" 
                    DeleteStrategy="GetAndDelete"
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarGruposConta" 
                    SelectByKeysMethod="ObtemGrupoConta"
                    UpdateStrategy="GetAndUpdate"
                    SortParameterName="sortExpression"
                    UpdateMethod="SalvarGrupoConta"> 
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCategoriaConta" 
                    runat="server" SelectMethod="ObtemCategoriasConta"
                    TypeName="Glass.Financeiro.Negocios.IPlanoContasFluxo">
                    <SelectParameters>
                        <asp:Parameter Name="ativas" DefaultValue="true" Type="Boolean"/>
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
