<%@ Page Title="Departamentos" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstDepartamento.aspx.cs" Inherits="Glass.UI.Web.Listas.LstDepartamento" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">

    <script type="text/javascript">
        function openFuncionarios(idDepartamento)
        {
            openWindow(400, 600, "LstFuncDepartamento.aspx?idDepartamento=" + idDepartamento);
        }
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Código" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtCod" runat="server" Width="50px" onkeydown="if (isEnter(event)) cOnClick('imgPesq')"
                                onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNome" runat="server" onkeydown="if (isEnter(event)) cOnClick('imgPesq')"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/Pesquisar.gif"
                                OnClick="imgPesq_Click" />
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
                <asp:GridView ID="grdDepartamento" runat="server" SkinID="gridViewEditable"
                    DataKeyNames="IdDepartamento" DataSourceID="odsDepartamento" EnableViewState="false">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse departamento?&quot;)) return false" />
                                <asp:ImageButton ID="imgFunc" runat="server" ImageUrl="~/Images/user_comment.gif"
                                    OnClientClick='<%# "openFuncionarios(" + Eval("IdDepartamento") + "); return false" %>'
                                    ToolTip="Funcionários do departamento" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgSalvar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="IdDepartamento">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdDepartamento") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("IdDepartamento") %>'></asp:Label>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nome" SortExpression="Nome">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Nome") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" MaxLength="20" Text='<%# Bind("Nome") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNome" runat="server" MaxLength="20" Text='<%# Bind("Nome") %>'></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                    ControlToValidate="txtNome" Display="Dynamic" ErrorMessage="Informe o nome" 
                                    SetFocusOnError="True" ToolTip="Informe o nome" ValidationGroup="c">*</asp:RequiredFieldValidator>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                                    DisplayMode="List" ShowMessageBox="True" ShowSummary="False" 
                                    ValidationGroup="c" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" MaxLength="100" Text='<%# Bind("Descricao") %>'
                                    Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" Text='<%# Bind("Descricao") %>'
                                    Width="200px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" 
                                    OnClick="imgAdd_Click" ValidationGroup="c" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDepartamento" runat="server" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.Departamento"
                    DeleteMethod="ApagarDepartamento" 
                    DeleteStrategy="GetAndDelete"
                    MaximumRowsParameterName="pageSize"
                    SelectMethod="PesquisarDepartamentos" 
                    SelectByKeysMethod="ObtemDepartamento"
                    SortParameterName="sortExpression"
                    TypeName="Glass.Global.Negocios.IFuncionarioFluxo" 
                    UpdateMethod="SalvarDepartamento"
                    UpdateStrategy="GetAndUpdate"
                    EnablePaging="True">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtCod" Name="idDepartamento" PropertyName="Text"
                            Type="UInt32" />
                        <asp:ControlParameter ControlID="txtNome" Name="nome" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
