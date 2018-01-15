<%@ Page Title="Pagamentos de Administradoras de Cartão" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstPagtoAdminCartao.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPagtoAdminCartao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">
    <table>
        <tr>
            <td>
                <asp:GridView ID="grdPagtoAdminCartao" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdPagtoAdminCartao" DataSourceID="odsPagtoAdminCartao" GridLines="None" 
                    ondatabound="grdPagtoAdminCartao_DataBound" ShowFooter="True" 
                    onrowcommand="grdPagtoAdminCartao_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" 
                                    ImageUrl="~/Images/Ok.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CausesValidation="False" 
                                    CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfIdAdminCartao" runat="server" 
                                    Value='<%# Bind("IdAdminCartao") %>' />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" 
                                    ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" 
                                    onclientclick="if (!confirm(&quot;Deseja excluir esse pagamento de cartão?&quot;)) return false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loja" SortExpression="NomeLoja">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                    DataTextField="NomeFantasia" DataValueField="IdLoja" 
                                    SelectedValue='<%# Bind("IdLoja") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpLoja" runat="server" DataSourceID="odsLoja" 
                                    DataTextField="NomeFantasia" DataValueField="IdLoja" 
                                    SelectedValue='<%# Bind("IdLoja") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("NomeLoja") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Mês" SortExpression="Mes">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpMes" runat="server" DataSourceID="odsMeses" 
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("Mes") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpMes" runat="server" DataSourceID="odsMeses" 
                                    DataTextField="Descr" DataValueField="Id" SelectedValue='<%# Bind("Mes") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrMes") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ano" SortExpression="Ano">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtAno" runat="server" Text='<%# Bind("Ano") %>' Width="50px"
                                    onkeypress="return soNumeros(event, true, true)"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtAno" runat="server" 
                                    onkeypress="return soNumeros(event, true, true)" Text='<%# Bind("Ano") %>' 
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Ano") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Cartão Crédito" 
                            SortExpression="ValorCredito">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorCredito" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Text='<%# Bind("ValorCredito") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorCredito" runat="server" 
                                    onkeypress="return soNumeros(event, false, true)" 
                                    Text='<%# Bind("ValorCredito") %>'></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" 
                                    Text='<%# Bind("ValorCredito", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor Cartão Débito" 
                            SortExpression="ValorDebito">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtValorDebito" runat="server" 
                                    onkeypress="return soNumeros(event, false, true)" 
                                    Text='<%# Bind("ValorDebito") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtValorDebito" runat="server" 
                                    onkeypress="return soNumeros(event, false, true)" 
                                    Text='<%# Bind("ValorDebito") %>'></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" 
                                    Text='<%# Bind("ValorDebito", "{0:c}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgInsert" runat="server" ImageUrl="~/Images/Insert.gif" 
                                    onclick="imgInsert_Click" style="width: 16px" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPagtoAdminCartao" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.PagtoAdministradoraCartao" 
                    DeleteMethod="Delete" EnablePaging="True" InsertMethod="Insert" 
                    MaximumRowsParameterName="pageSize" SelectCountMethod="GetCount" 
                    SelectMethod="GetList" SortParameterName="sortExpression" 
                    StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.PagtoAdministradoraCartaoDAO" 
                    UpdateMethod="Update" onupdated="odsPagtoAdminCartao_Updated">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idAdminCartao" QueryStringField="idAdminCartao" 
                            Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsLoja" runat="server" SelectMethod="GetAll" 
                    TypeName="Glass.Data.DAL.LojaDAO"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMeses" runat="server" SelectMethod="GetMeses" 
                    TypeName="Glass.Data.Helper.DataSources"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

