<%@ Page Title="Informação sobre Valores Agregados" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstInfoValorAgregado.aspx.cs" Inherits="Glass.UI.Web.Listas.LstInfoValorAgregado" %>

<%@ Register src="../Controls/ctrlSelProduto.ascx" tagname="ctrlSelProduto" tagprefix="uc1" %>
<%@ Register src="../Controls/ctrlSelPopup.ascx" tagname="ctrlSelPopup" tagprefix="uc2" %>
<%@ Register src="../Controls/ctrlData.ascx" tagname="ctrlData" tagprefix="uc3" %>
<%@ Register src="../Controls/ctrlSelCidade.ascx" tagname="ctrlSelCidade" tagprefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdInfoValorAgregado" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdInfoValorAgregado" DataSourceID="odsInfoValorAgregado" 
                    GridLines="None" ShowFooter="True" 
                    ondatabound="grdInfoValorAgregado_DataBound" 
                    onrowcommand="grdInfoValorAgregado_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" 
                                    ImageUrl="~/Images/Ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" 
                                    CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CausesValidation="False" 
                                    CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CausesValidation="False" 
                                    CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif" 
                                    onclientclick="if (!confirm(&quot;Deseja excluir essa informação sobre valor agregado?&quot;)) return false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Produto" SortExpression="IdProd">
                            <EditItemTemplate>
                                <uc1:ctrlSelProduto ID="ctrlSelProduto" runat="server" 
                                    IdProd='<%# Bind("IdProd") %>' PermitirVazio="False" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc1:ctrlSelProduto ID="ctrlSelProduto" runat="server" PermitirVazio="False" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                                -
                                <asp:Label ID="Label5" runat="server" Text='<%# Eval("DescrProduto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cidade" SortExpression="IdCidade">
                            <EditItemTemplate>
                                <uc4:ctrlSelCidade ID="ctrlSelCidade1" runat="server" 
                                    IdCidade='<%# Bind("IdCidade") %>' PermitirVazio="False" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc4:ctrlSelCidade ID="ctrlSelCidade" runat="server" PermitirVazio="False" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("NomeCidade") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="Data">
                            <EditItemTemplate>
                                <uc3:ctrlData ID="ctrlData1" runat="server" Data='<%# Bind("Data") %>' 
                                    ValidateEmptyText="True" />
                            </EditItemTemplate>
                            <FooterTemplate>
                                <uc3:ctrlData ID="ctrlData" runat="server" ValidateEmptyText="True" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Data", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor" SortExpression="Valor">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" Text='<%# Bind("ValorString") %>'
                                    onkeypress="return soNumeros(event, false, true)" Width="70px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValor" runat="server" 
                                    ControlToValidate="txtValor" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValor" runat="server" 
                                    onkeypress="return soNumeros(event, false, true)" Width="70px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvValor" runat="server" 
                                    ControlToValidate="txtValor" Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Valor", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgInserir" runat="server" ImageUrl="~/Images/Insert.gif" 
                                    onclick="imgInserir_Click" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsInfoValorAgregado" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.InfoValorAgregado" DeleteMethod="Delete" 
                    EnablePaging="True" InsertMethod="Insert" MaximumRowsParameterName="pageSize" 
                    SelectCountMethod="GetCount" SelectMethod="GetList" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.InfoValorAgregadoDAO" UpdateMethod="Update" 
                    >
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

