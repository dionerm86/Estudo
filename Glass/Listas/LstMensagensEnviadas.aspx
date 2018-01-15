<%@ Page Title="Mensagens Enviadas" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="LstMensagensEnviadas.aspx.cs" Inherits="Glass.UI.Web.Listas.LstMensagensEnviadas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <script type="text/javascript">
        function exibirMensagem(id)
        {
            openWindow(400, 550, '../Utils/DetalheMensagem.aspx?idMsg=' + id);
            return false;
        }
    </script>
    <table>
        <tr>
            <td align="center">
                <asp:GridView ID="grdMensagem" runat="server"
                    DataKeyNames="IdMensagem" DataSourceID="odsMensagem"  SkinID="defaultGridView"
                    EmptyDataText="Nenhuma mensagem encontrada."
                    PageSize="15">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="return exibirMensagem(<%# Eval("IdMensagem") %>);">
                                    <img src="../Images/detalhes.gif" border="0" alt="Detalhes"></a>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta Mensagem? Os destinatários não terão acesso à mesma.&quot;);" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Remetente" SortExpression="Remetente">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Remetente") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Remetente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Destinatarios" HeaderText="Destinatários" />
                        <asp:TemplateField HeaderText="Assunto" SortExpression="Assunto">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Assunto") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <a href="#" onclick="return exibirMensagem(<%# Eval("IdMensagem") %>);">
                                    <asp:Label ID="lblAssunto" runat="server" Text='<%# Bind("Assunto") %>'></asp:Label>
                                </a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="DataCad">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("DataCad") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DataCad", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Hora" SortExpression="DataCad">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("DataCad") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("DataCad", "{0:t}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMensagem" runat="server" 
                    DeleteMethod="ApagarMensagemEnviada" 
                    SelectMethod="PesquisarMensagensEnviadas"
                    TypeName="Glass.Global.UI.Web.Process.Mensagens.LeituraMensagens" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
