<%@ Page Title="Mensagens" Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true"
    CodeBehind="Main.aspx.cs" Inherits="Glass.UI.Web.Main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table>
        <tr>
            <td class="subtitle">
                <asp:Label ID="lblMensagemFunc" runat="server" Text="Mensagens de Funcionários" Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;<asp:LinkButton ID="lnkNovaMensagem" runat="server" OnClick="lnkNovaMensagem_Click">Nova mensagem</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdMensagem" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdMensagem" DataSourceID="odsMensagem" CssClass="gridStyle" PagerStyle-CssClass="pgr"
                    AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit" EmptyDataText="Nenhuma mensagem encontrada."
                    AllowPaging="True" AllowSorting="True" PageSize="15">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="openWindow(400, 550, '../Utils/DetalheMensagem.aspx?idMsg=<%# Eval("IdMensagem") %>')">
                                    <img src="../Images/detalhes.gif" border="0" alt="Visualizar"></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta Mensagem?&quot;);" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Remetente" SortExpression="NomeRemetente">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Remetente") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label1" Font-Bold='<%# !(bool)Eval("Lida") %>' runat="server" Text='<%# Bind("Remetente") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Assunto" SortExpression="Assunto">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Assunto") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <a href="#" onclick="openWindow(400, 550, '../Utils/DetalheMensagem.aspx?idMsg=<%# Eval("IdMensagem") %>')">
                                    <asp:Label Font-Bold='<%# !(bool)Eval("Lida") %>' ID="lblAssunto" runat="server"
                                        Text='<%# Bind("Assunto") %>'></asp:Label></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data" SortExpression="DataCad">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox5" runat="server" Text='<%# Bind("DataCad") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label4" Font-Bold='<%# !(bool)Eval("Lida") %>' runat="server" Text='<%# Bind("DataCad", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Hora" SortExpression="DataCad">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("DataCad") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label2" Font-Bold='<%# !(bool)Eval("Lida") %>' runat="server" Text='<%# Bind("DataCad", "{0:t}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# (bool)Eval("Lida") ? "Lida" : "Não Lida" %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" Font-Bold='<%# !(bool)Eval("Lida") %>' runat="server" Text='<%# (bool)Eval("Lida") ? "Lida" : "Não Lida" %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <asp:LinkButton ID="lnkEnviadas" runat="server" OnClick="lnkEnviadas_Click">Enviadas</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td class="subtitle1">
                <asp:Label ID="lblMensagemParceiro" runat="server" Text="Mensagens de Parceiros"
                    Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkNovaMensagemParceiro" runat="server" OnClick="lnkNovaMensagemParceiro_Click"
                    Visible="False">Nova mensagem</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdMensagemParceiro" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="IdMensagem" DataSourceID="odsMensagemParceiro" CssClass="gridStyle"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EditRowStyle-CssClass="edit"
                    EmptyDataText="Nenhuma mensagem encontrada." AllowPaging="True" AllowSorting="True"
                    PageSize="15" Visible="False">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="openWindow(400, 550, '../Utils/DetalheMensagemParceiro.aspx?idMsg=<%# Eval("IdMensagem") %>')">
                                    <img src="../Images/detalhes.gif" border="0" alt="Visualizar"></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imbExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    ToolTip="Excluir" OnClientClick="return confirm(&quot;Tem certeza que deseja excluir esta Mensagem?&quot;);" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Remetente" HeaderText="Remetente" SortExpression="Remetente" />
                        <asp:BoundField DataField="Assunto" HeaderText="Assunto" SortExpression="Assunto" />
                        <asp:TemplateField HeaderText="Data" SortExpression="DataCad">
                            <ItemTemplate>
                                <asp:Label ID="Label4" Font-Bold='<%# !(bool)Eval("Lida") %>' runat="server" Text='<%# Bind("DataCad", "{0:d}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Hora" SortExpression="DataCad">
                            <ItemTemplate>
                                <asp:Label ID="Label2" Font-Bold='<%# !(bool)Eval("Lida") %>' runat="server" Text='<%# Bind("DataCad", "{0:t}") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Situação" SortExpression="Situacao">
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox4" runat="server" Text='<%# (bool)Eval("Lida") ? "Lida" : "Não Lida" %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="Label3" Font-Bold='<%# !(bool)Eval("Lida") %>' runat="server" Text='<%# (bool)Eval("Lida") ? "Lida" : "Não Lida" %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMensagem" runat="server"
                    DeleteMethod="ApagarMensagemRecebida"
                    SelectMethod="PesquisarMensagensRecebidas"
                    TypeName="Glass.Global.UI.Web.Process.Mensagens.LeituraMensagens" EnablePaging="True"
                    MaximumRowsParameterName="pageSize" SortParameterName="sortExpression">
                    <DeleteParameters>
                        <asp:Parameter Name="idMensagem" Type="Int32" />
                    </DeleteParameters>
                </colo:VirtualObjectDataSource>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMensagemParceiro" runat="server"
                    SelectMethod="PesquisarMensagensParceirosRecebidasFuncionario"
                    TypeName="Glass.Global.UI.Web.Process.Mensagens.LeituraMensagens" EnablePaging="True"
                    MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression" DeleteMethod="ApagarMensagemParceiroRecebidaFuncionario">
                    <DeleteParameters>
                        <asp:Parameter Name="idMensagem" Type="Int32" />
                    </DeleteParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
