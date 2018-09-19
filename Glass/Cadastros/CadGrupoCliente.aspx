<%@ Page Title="Grupos de Cliente" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadGrupoCliente.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadGrupoCliente" %>

<%@ Register Src="../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdGrupoCliente" runat="server"
                    AllowPaging="True" AutoGenerateColumns="False"
                    DataKeyNames="IdGrupoCliente" DataSourceID="odsGrupoCliente"
                    CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" PageSize="15"
                    AllowSorting="True" ShowFooter="True"
                    OnRowCommand="grdGrupoCliente_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" CommandName="Update"
                                    ImageUrl="~/Images/Ok.gif" />
                                <asp:ImageButton ID="imbExcluir" runat="server" CausesValidation="False"
                                    CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" CommandName="Edit"
                                    ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Grupo de Cliente?&quot;);" />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="35"
                                    Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="35"
                                    Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" OnClick="lnkInserir_Click">
                                    <img border="0" src="../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server"
                                    IdRegistro='<%# (uint)(int)Eval("IdGrupoCliente") %>' Tabela="GrupoCliente" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <PagerStyle CssClass="pgr"></PagerStyle>

                    <EditRowStyle CssClass="edit"></EditRowStyle>

                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource Culture="pt-BR" ID="odsGrupoCliente" runat="server"
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.GrupoCliente" DeleteMethod="ApagarGrupoCliente"
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectMethod="PesquisarGruposCliente"
                    SelectByKeysMethod="ObterGrupoCliente"
                    SortParameterName="sortExpression" DeleteStrategy="GetAndDelete" UpdateStrategy="GetAndUpdate"
                    TypeName="Glass.Global.Negocios.IClienteFluxo" UpdateMethod="SalvarGrupoCliente">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
