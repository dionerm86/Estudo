<%@ Page Title="Fabricante de Ferragem" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstFabricanteFerragem.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.LstFabricanteFerragem" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        function onSave() {
            var nome = FindControl("txtNome", "input").value;
            var sitio = FindControl("txtSitio", "input").value;

            if (nome == "") {
                alert("Informe o nome.");
                return false;
            }

            if (sitio == "") {
                alert("Informe o site.");
                return false;
            }
        }

    </script>

    <table style="width: 100%">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label9" runat="server" Text="Nome" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFiltroNome" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq0" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Site" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtFiltroSitio" runat="server" Width="200px" onkeydown="if (isEnter(event)) cOnClick('imgPesq', null);"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq1" runat="server" ImageUrl="~/Images/Pesquisar.gif" ToolTip="Pesquisar"
                                OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFabricanteFerragem" runat="server" AllowPaging="true" AllowSorting="true"
                    CssClass="gridStyle" AutoGenerateColumns="false" GridLines="None" ShowFooter="true"
                    DataSourceID="odsFabricanteFerragem" DataKeyNames="IdFabricanteFerragem">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/Edit.gif" ToolTip="Editar" />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm('Tem certeza que deseja excluir este Fabricante?');" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" 
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onSave();" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Nome" SortExpression="Nome">
                            <ItemTemplate>
                                <asp:Label ID="lblNome" runat="server" Text='<%# Bind("Nome") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtNome" runat="server" MaxLength="60" Text='<%# Bind("Nome") %>' Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtNome" runat="server" MaxLength="60" Width="200px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Site" SortExpression="Sitio">
                            <ItemTemplate>
                                <asp:Label ID="lblSitio" runat="server" Text='<%# Bind("Sitio") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSitio" runat="server" MaxLength="60" Text='<%# Bind("Sitio") %>' Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtSitio" runat="server" MaxLength="60" Width="200px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton id="imbInserir" runat="server" ImageUrl="~/Images/insert.gif" OnClick="imbInserir_Click"
                                    OnClientClick="return onSave();" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                    <EditRowStyle CssClass="edit" />
                    <PagerStyle CssClass="pgr" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsFabricanteFerragem" runat="server" Culture="pt-BR"
                    EnablePaging="true" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression"
                    TypeName="Glass.Projeto.Negocios.IFerragemFluxo" DataObjectTypeName="Glass.Projeto.Negocios.Entidades.FabricanteFerragem"
                    SelectMethod="PesquisarFabricanteFerragem" SelectByKeysMethod="ObterFabricanteFerragem"
                    UpdateStrategy="GetAndUpdate" UpdateMethod="SalvarFabricanteFerragem"
                    DeleteStrategy="GetAndDelete" DeleteMethod="ApagarFabricanteFerragem">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtFiltroNome" Name="nome" PropertyName="Text" />
                        <asp:ControlParameter ControlID="txtFiltroSitio" Name="sitio" PropertyName="Text" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
