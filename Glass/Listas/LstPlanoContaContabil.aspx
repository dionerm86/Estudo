<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="LstPlanoContaContabil.aspx.cs" Inherits="Glass.UI.Web.Listas.LstPlanoContaContabil" Title="Planos de Conta Contábeis" %>

<%@ Register src="../Controls/ctrlLogPopup.ascx" tagname="ctrlLogPopup" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript">

        function openPlanoConta(idContaContabil) {
            openWindow(600, 800, '../Cadastros/CadVincularPlanoConta.aspx?idContaContabil=' + idContaContabil);
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdPlanoContaCont" runat="server" AllowPaging="True" 
                    AllowSorting="True" AutoGenerateColumns="False" CssClass="gridStyle" 
                    DataKeyNames="IdContaContabil" DataSourceID="odsPlanoContaCont" 
                    GridLines="None" ondatabound="grdPlanoContaCont_DataBound" 
                    ShowFooter="True" onrowcommand="grdPlanoContaCont_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgEdit" runat="server" CommandName="Update" 
                                    ImageUrl="~/Images/ok.gif" />
                                <asp:ImageButton ID="imgDelete" runat="server" CommandName="Cancel" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEdit" runat="server" CommandName="Edit" 
                                    ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgDelete" runat="server" CommandName="Delete" 
                                    ImageUrl="~/Images/ExcluirGrid.gif" 
                                    onclientclick="if (!confirm(&quot;Deseja excluir esse plano de conta contábil?&quot;)) return false;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cód." SortExpression="CodInterno">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCodInterno" runat="server" MaxLength="60" 
                                    Text='<%# Bind("CodInterno") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtCodInterno" runat="server" MaxLength="60" 
                                    Text='<%# Bind("CodInterno") %>'></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("CodInterno") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" Text='<%# Bind("Descricao") %>' 
                                    MaxLength="60" Width="200px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtDescricao" runat="server" MaxLength="60" 
                                    Text='<%# Bind("Descricao") %>' Width="200px"></asp:TextBox>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Natureza" SortExpression="DescrNatureza">
                            <EditItemTemplate>
                                <asp:DropDownList ID="drpNatureza" runat="server" DataSourceID="odsNatureza" 
                                    DataTextField="Descr" DataValueField="Id" 
                                    SelectedValue='<%# Bind("Natureza") %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:DropDownList ID="drpNatureza" runat="server" DataSourceID="odsNatureza" 
                                    DataTextField="Descr" DataValueField="Id" 
                                    SelectedValue='<%# Bind("Natureza") %>'>
                                </asp:DropDownList>
                            </FooterTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("DescrNatureza") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Cadastro" SortExpression="DataCad">
                            <EditItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Eval("DataCad", "{0:d}") %>'></asp:Label>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DataCad", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Plano de Contas">
                            <ItemTemplate>
                                <asp:ImageButton ID="imgAddPlanoConta" runat="server" ImageUrl="~/Images/addMany.gif" 
                                    OnClientClick='<%# "openPlanoConta(" + Eval("IdContaContabil") + "); return false" %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imgAdd" runat="server" ImageUrl="~/Images/Insert.gif" 
                                    onclick="imgAdd_Click" />
                            </FooterTemplate>
                            <ItemTemplate>
                                <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" Tabela="PlanoContaContabil"
                                    IdRegistro='<%# (int)Eval("IdContaContabil") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pgr" />
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsPlanoContaCont" runat="server" 
                    DataObjectTypeName="Glass.Data.Model.PlanoContaContabil" DeleteMethod="Delete" 
                    EnablePaging="True" InsertMethod="Insert" MaximumRowsParameterName="pageSize" 
                    SelectCountMethod="GetCount" SelectMethod="GetList" 
                    SortParameterName="sortExpression" StartRowIndexParameterName="startRow" 
                    TypeName="Glass.Data.DAL.PlanoContaContabilDAO" UpdateMethod="Update" 
                    ondeleted="odsPlanoContaCont_Deleted"
                    onupdated="odsPlanoContaCont_Updated"></colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsNatureza" runat="server" 
                    SelectMethod="GetNaturezas" TypeName="Glass.Data.DAL.PlanoContaContabilDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>

