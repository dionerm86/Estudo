<%@ Page Title="Grupos de Figuras de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadGrupoFiguraProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadGrupoFiguraProjeto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

<script type="text/javascript">

    function onSave(insert) {
        var descricao = FindControl(insert ? "txtDescricao" : "txtDescricao", "input").value;

        if (descricao == "") {
            alert("Informe a descrição.");
            return false;
        }
    }

</script>
    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdGrupoFiguraProjeto" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    DataSourceID="odsGrupoFiguraProjeto" 
                    ShowFooter="True" DataKeyNames="IdGrupoFigProj" PageSize="15"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" 
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit">
                                    <img border="0" src="../../Images/Edit.gif"></img></asp:LinkButton>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" 
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
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="60" 
                                    Text='<%# Bind("Descricao") %>' Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="60" 
                                    Text='<%# Bind("Descricao") %>' Width="200px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="DescrSituacao">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrSituacao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server" 
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="2">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server">
                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="2">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);" 
                                    onclick="lnkInserir_Click"><img border="0" src="../../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                    <EditRowStyle CssClass="edit"></EditRowStyle>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoFiguraProjeto" runat="server" 
                    DeleteMethod="Delete" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" ondeleted="odsGrupoFiguraProjeto_Deleted" 
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.GrupoFiguraProjetoDAO" DataObjectTypeName="Glass.Data.Model.GrupoFiguraProjeto" 
                    UpdateMethod="Update" onupdated="odsGrupoFiguraProjeto_Updated">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        </table>
</asp:Content>

