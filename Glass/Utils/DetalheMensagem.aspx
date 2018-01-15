<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetalheMensagem.aspx.cs"
    Inherits="Glass.UI.Web.Utils.DetalheMensagem" Title="Mensagem" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <asp:DetailsView ID="dtvMensagem" runat="server" AutoGenerateRows="False" CellPadding="4"
                    DataSourceID="odsMensagem" ForeColor="#333333" GridLines="None" Height="50px"
                    Width="125px">
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Left" />
                    <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" HorizontalAlign="Left" />
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <Fields>
                        <asp:TemplateField HeaderText="Remetente" SortExpression="NomeRemetente">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Remetente") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Remetente") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Remetente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Destinatarios" SortExpression="Destinatarios">
                            <EditItemTemplate>
                                <asp:TextBox ID="destinatariosTextBox" runat="server" Text='<%# Bind("Destinatarios") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="destinatariosTextBox" runat="server" Text='<%# Bind("Destinatarios") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <%# string.Join(", ", ((IEnumerable<Glass.Global.Negocios.Entidades.MensagemDetalhes.Destinatario>)Eval("Destinatarios")).Select(f => f.Nome).ToArray()) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="DataCad">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("DataCad") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("DataCad") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("DataCad") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Assunto" SortExpression="Assunto">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Assunto") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Assunto") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("Assunto") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Mensagem" SortExpression="Descricao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <InsertItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("Descricao") %>'></asp:TextBox>
                            </InsertItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Descricao") != null ? Eval("Descricao").ToString().Replace("\n", "<br/>") : String.Empty %>'
                                    Width="300px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Fields>
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#999999" HorizontalAlign="Left" />
                    <AlternatingRowStyle BackColor="White" />
                </asp:DetailsView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMensagem" runat="server" 
                    SelectMethod="ObtemDetalhesMensagem" TypeName="Glass.Global.Negocios.IMensagemFluxo">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idMensagem" QueryStringField="idMsg" Type="Int32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnResponder" runat="server" Text="Responder" OnClick="btnResponder_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="Button1" runat="server" OnClientClick="closeWindow(); return false;"
                    Text="Fechar" />
            </td>
        </tr>
    </table>
</asp:Content>
