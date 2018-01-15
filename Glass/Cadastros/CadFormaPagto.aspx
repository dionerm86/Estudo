<%@ Page Title="Formas de Pagamento" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="CadFormaPagto.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadFormaPagto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvFormaPagto" runat="server" AutoGenerateRows="False" CellPadding="4"
                    DataSourceID="odsCadFormaPagto" DefaultMode="Insert" ForeColor="#333333" GridLines="None"
                    Height="50px" Width="125px" DataKeyNames="IdFormaPagto">
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#D1DDF1" Font-Bold="True" />
                    <RowStyle BackColor="#EFF3FB" />
                    <FieldHeaderStyle BackColor="#DEE8F5" Font-Bold="True" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Descrição" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Descricao") %>' Width="300px"></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Descricao") %>' Width="300px"></asp:TextBox>
                                <asp:Button ID="Button2" runat="server" CommandName="Insert" Text="Inserir" />
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Descricao") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <InsertRowStyle HorizontalAlign="Left" />
                    <EditRowStyle BackColor="#2461BF" HorizontalAlign="Left" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsCadFormaPagto" runat="server" SelectMethod="GetElementByPrimaryKey"
                    TypeName="Glass.Data.DAL.FormaPagtoDAO" OnInserted="odsCadFormaPagto_Inserted"
                    DataObjectTypeName="Glass.Data.Model.FormaPagto" InsertMethod="Insert">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="key" QueryStringField="idFormaPagto" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdFormaPagto" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                    DataKeyNames="IdFormaPagto" DataSourceID="odsFormaPagto" PageSize="15" Width="434px"
                    AllowSorting="True" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta Forma de Pagamento?&quot;);"
                                    ToolTip="Excluir" Visible='<%# Eval("DeleteVisible") %>' />
                            </ItemTemplate>
                            <ItemStyle Wrap="False" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="IdFormaPagto" HeaderText="Cod." SortExpression="IdFormaPagto">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao">
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:BoundField>
                    </Columns>
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFormaPagto" runat="server" DataObjectTypeName="Glass.Data.Model.FormaPagto"
                    DeleteMethod="Delete" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SelectCountMethod="GetAllForListCount" SelectMethod="GetAllForList" SortParameterName="sortExpression"
                    StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.FormaPagtoDAO"
                    OnDeleted="odsFormaPagto_Deleted" InsertMethod="Insert"></colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
