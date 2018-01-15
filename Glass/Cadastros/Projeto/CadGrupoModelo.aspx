<%@ Page Title="Grupos de Projeto" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadGrupoModelo.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.CadGrupoModelo" %>

<%@ Register src="../../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

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
                <asp:GridView GridLines="None" ID="grdGrupoModelo" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" 
                    DataSourceID="odsGrupoModelo" 
                    ShowFooter="True" DataKeyNames="IdGrupoModelo" PageSize="15"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit">
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
                                    Text='<%# Bind("Descricao") %>' Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="60" 
                                    Text='<%# Bind("Descricao") %>' Width="300px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
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
                        <asp:TemplateField HeaderText="Box Padrão?" SortExpression="BoxPadrao">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("BoxPadrao") %>' 
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" 
                                    Checked='<%# Bind("BoxPadrao") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkBoxPadrao" runat="server" 
                                    Checked='<%# Bind("BoxPadrao") %>' />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Esquadria?" SortExpression="Esquadria">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkEsquadria" runat="server" Checked='<%# Bind("Esquadria") %>' 
                                    Enabled="False" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkEsquadria" runat="server" 
                                    Checked='<%# Bind("Esquadria") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkEsquadria" runat="server" 
                                    Checked='<%# Bind("Esquadria") %>' />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClientClick="return onSave(true);" 
                                    onclick="lnkInserir_Click"><img border="0" src="../../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# Eval("IdGrupoModelo") %>' 
                                    Tabela="GrupoModelo" />
                            </ItemTemplate>
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
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoModelo" runat="server" 
                    DeleteMethod="Delete" EnablePaging="True" 
                    MaximumRowsParameterName="pageSize" ondeleted="odsGrupoModelo_Deleted" 
                    SelectCountMethod="GetCount" SelectMethod="GetList" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.GrupoModeloDAO" DataObjectTypeName="Glass.Data.Model.GrupoModelo" 
                    UpdateMethod="Update" onupdated="odsGrupoModelo_Updated">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        </table>
</asp:Content>

