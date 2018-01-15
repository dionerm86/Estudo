<%@ Page Title="Mensagens" Language="C#" MasterPageFile="~/WebGlassParceiros/PainelParceiros.master"
    AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="Glass.UI.Web.WebGlassParceiros.Main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" runat="Server">
    <table style="width: 100%">
        <tr>
            <td align="center">
                <asp:LinkButton ID="lnkNovaMensagem" runat="server" OnClick="lnkNovaMensagem_Click"
                    Visible="False">Nova mensagem</asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdMensagem0" runat="server" SkinID="defaultGridView"
                    DataKeyNames="IdMensagem" DataSourceID="odsMensagemParceiro"
                    EmptyDataText="Nenhuma mensagem encontrada." PageSize="15">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href="#" onclick="openWindow(400, 550, 'DetalheMensagemParceiro.aspx?idMsg=<%# Eval("IdMensagem") %>')">
                                    <img src="../Images/detalhes.gif" border="0p" alt="Visualizar"></a>
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
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsMensagemParceiro" runat="server" 
                    SelectMethod="PesquisarMensagensParceirosRecebidasCliente"
                    TypeName="Glass.Global.UI.Web.Process.Mensagens.LeituraMensagens" EnablePaging="True" MaximumRowsParameterName="pageSize"
                    SortParameterName="sortExpression" DeleteMethod="ApagarMensagemParceiroRecebidaCliente">
                    <DeleteParameters>
                        <asp:Parameter Name="idMensagem" Type="Int32" />
                    </DeleteParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
