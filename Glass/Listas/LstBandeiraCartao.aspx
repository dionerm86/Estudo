<%@ Page Title="Bandeira de Cartão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstBandeiraCartao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstBandeiraCartao" %>

<%@ Register Src="~/Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="server">

    <script type="text/javascript">

        function onSave() {
            var descricao = FindControl("txtDescricao", "input").value;

            if (descricao == "") {
                alert("Informe a descrição.");
                return false;
            }
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdBandeiraCartao" runat="server" AllowPaging="true" AllowSorting="true"
                    AutoGenerateColumns="false" CssClass="gridStyle" ShowFooter="true" GridLines="None"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    DataSourceID="odsBandeiraCartao" DataKeyNames="IdBandeiraCartao">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbEditar" runat="server" CommandName="Edit"
                                    ImageUrl="~/Images/edit.gif" ToolTip="Editar" />
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" Visible='<%# Eval("PodeExcluir") %>'
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Excluir" />
                                <asp:HiddenField ID="hdfBandeiraCartao" runat="server" Value='<%# Eval("IdBandeiraCartao") %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imbAtualizar" runat="server" CommandName="Update"
                                    ImageUrl="~/Images/ok.gif" ToolTip="Atualizar" OnClientClick="return onSave();" />
                                <asp:ImageButton ID="imbCancelar" runat="server" CommandName="Cancel" Visible="true"
                                    ImageUrl="~/Images/ExcluirGrid.gif" ToolTip="Cancelar" />
                                <asp:HiddenField ID="hdfBandeiraCartao" runat="server" Value='<%# Eval("IdBandeiraCartao") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição">
                            <ItemTemplate>
                                <asp:Label ID="lblDescricao" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpSituacao" runat="server"
                                    SelectedValue='<%# Bind("Situacao") %>'>
                                    <asp:ListItem Value="Ativo">Ativo</asp:ListItem>
                                    <asp:ListItem Value="Inativo">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Colosoft.Translator.Translate(Eval("Situacao")).Format() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup" runat="server" Tabela="BandeiraCartao" IdRegistro='<%# Eval("IdBandeiraCartao") %>' />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton ID="imbInserir" runat="server" ToolTip="Inserir" ImageUrl="~/Images/insert.gif"
                                    OnClick="imbInserir_Click" OnClientClick="return onSave();" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                <colo:VirtualObjectDataSource ID="odsBandeiraCartao" runat="server" Culture="pt-BR" EnablePaging="true"
                    TypeName="Glass.Data.DAL.BandeiraCartaoDAO" DataObjectTypeName="Glass.Data.Model.BandeiraCartao"
                    StartRowIndexParameterName="startRow" MaximumRowsParameterName="pageSize" SortParameterName="sortExpression"
                    UpdateMethod="Update" DeleteMethod="Delete" SelectMethod="GetList" SelectCountMethod="GetCount">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

</asp:Content>
