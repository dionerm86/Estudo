<%@ Page Title="Tipos de Cliente" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadTipoCliente.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadTipoCliente" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdTipoCliente" runat="server" 
                    AllowPaging="True" AutoGenerateColumns="False"
                    DataKeyNames="IdTipoCliente" DataSourceID="odsTipoCliente" 
                    CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" PageSize="15"
                    AllowSorting="True" ShowFooter="True" 
                    onrowcommand="grdTipoCliente_RowCommand">
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
                                    
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir este Tipo de Cliente?&quot;);"  />
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
                        <asp:TemplateField HeaderText="Cobrar Área Mínima" 
                            SortExpression="CobrarAreaMinima">
                            <EditItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" 
                                    Checked='<%# Bind("CobrarAreaMinima") %>' />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:CheckBox ID="chkCobrarAreaMinima" runat="server" 
                                    Checked='True' />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" 
                                    Checked='<%# Bind("CobrarAreaMinima") %>' Enabled="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:LinkButton ID="lnkInserir" runat="server" onclick="lnkInserir_Click">
                                    <img border="0" src="../Images/insert.gif" /></asp:LinkButton>
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" 
                                    IdRegistro='<%# (uint)(int)Eval("IdTipoCliente") %>' Tabela="TipoCliente" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

<PagerStyle CssClass="pgr"></PagerStyle>

<EditRowStyle CssClass="edit"></EditRowStyle>

<AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsTipoCliente" runat="server" 
                    DataObjectTypeName="Glass.Global.Negocios.Entidades.TipoCliente" DeleteMethod="ApagarTipoCliente" 
                    EnablePaging="True" MaximumRowsParameterName="pageSize" SelectMethod="PesquisaTiposCliente" 
                    SelectByKeysMethod="ObtemTipoCliente"
                    SortParameterName="sortExpression" DeleteStrategy="GetAndDelete" UpdateStrategy="GetAndUpdate"
                    TypeName="Glass.Global.Negocios.IClienteFluxo" UpdateMethod="SalvarTipoCliente" >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
