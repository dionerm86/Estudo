<%@ Page Title="Grupo de Medida de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadGrupoMedidaProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadGrupoMedidaProjeto" %>

<%@ Register src="../../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">
    <script type="text/javascript">

        function onSave(insert) {
            var descricao = FindControl(insert ? "txtDescricaoIn" : "txtDescricaoEd", "input").value;

            if (descricao == "") {
                alert("Informe a descrição.");
                return false;
            }
        }

    </script>
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdGrupoMedidaProjeto" runat="server" AllowPaging="true" AllowSorting="true" AutoGenerateColumns="false" GridLines="None"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" PageSize="15" ShowFooter="true"
                    DataSourceID="odsGrupoMedidaProjeto" DataKeyNames="IdGrupoMedProj">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEdit" runat="server" CommandName="Edit"
                                    ImageUrl="~/Images/Edit.gif" ToolTip="Editar" Visible='<%# Eval("PodeEditarExcluir") %>' />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" Visible='<%# Eval("PodeEditarExcluir") %>'
                                    OnClientClick="return confirm('Tem certeza que deseja excluir este Grupo?');" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update" 
                                    Height="16px" ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onSave(false);" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                            </EditItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="lblDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricaoEd" runat="server" MaxLength="60" 
                                    Text='<%# Bind("Descricao") %>' Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricaoIn" runat="server" MaxLength="60" 
                                    Text='<%# Bind("Descricao") %>' Width="300px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# Eval("IdGrupoMedProj") %>' 
                                    Tabela="GrupoMedidaProjeto"/>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton ID="imbInserir" runat="server" OnClientClick="return onSave(true);"
                                    ImageUrl="~/Images/insert.gif" ToolTip="Inserir" onclick="imbInserir_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsGrupoMedidaProjeto" runat="server" Culture="pt-BR" EnablePaging="true"
                    MaximumRowsParameterName="pageSize" StartRowIndexParameterName="startRow" SortParameterName="sortExpression"
                    TypeName="Glass.Data.DAL.GrupoMedidaProjetoDAO" DataObjectTypeName="Glass.Data.Model.GrupoMedidaProjeto"
                    SelectMethod="GetList" SelectCountMethod="GetCount"
                    UpdateMethod="Update" DeleteMethod="Delete"
                    OnUpdated="odsGrupoMedidaProjeto_Updated" OnDeleted="odsGrupoMedidaProjeto_Deleted">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
