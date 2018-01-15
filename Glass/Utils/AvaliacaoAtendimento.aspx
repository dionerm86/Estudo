<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AvaliacaoAtendimento.aspx.cs" Inherits="Glass.UI.Web.Utils.AvaliacaoAtendimento"
    Title="Avaliação de atendimento" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlLinkQueryString.ascx" TagName="ctrlLinkQueryString"
    TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
    </script>

    <table>
        <tr>
            <td align="center">
                <br />
                <br />
                <asp:Label ID="Label1" runat="server" Font-Size="Small" Text="Sua avaliação é muito importante para a melhoria contínua dos nossos serviços."></asp:Label>
                <br />
                <br />
                <asp:Label ID="Label2" runat="server" Font-Size="Small" Text="Clique no ícone “Aprovar” para confirmar que o chamado foi solucionado, caso contrário, clique em “Negar”."></asp:Label>
                <br />
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdAvaliacaoAtendimento" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsAvaliacaoAtendimento" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" EmptyDataText="Nenhum atendimento a ser avaliado!"
                    DataKeyNames="IdAvaliacaoAtendimento" OnRowCommand="grdAvaliacaoAtendimento_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="IdChamado" HeaderText="Chamado" />
                        <asp:BoundField DataField="Analista" HeaderText="Analista" />
                        <asp:BoundField DataField="DataAvaliacao" HeaderText="Cadastro" />
                        <asp:TemplateField HeaderText="Descrição" HeaderStyle-Font-Bold="true">
                            <ItemTemplate>
                                <asp:Label ID="lblDescricao" runat="server" Text='<%# Eval("Descricao") %>'></asp:Label><br />
                                <asp:Label ID="lblResolucao" runat="server" Text='<%# "Resolução: " + (Eval("Resolucao") != null ? Eval("Resolucao") : string.Empty).ToString() %>' Font-Bold="true"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Aprovar" HeaderStyle-Font-Bold="true">
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ID="imgAprovar" CommandName="Aprovar" ImageUrl="~/Images/check.gif" CommandArgument='<%# Eval("IdAvaliacaoAtendimento") %>'
                                    OnClientClick="return confirm('Confirma a APROVAÇÃO da resolução deste chamado?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Negar" HeaderStyle-Font-Bold="true">
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ID="imgNegar" CommandName="Negar" ImageUrl="~/Images/delete.gif" CommandArgument='<%# Eval("IdAvaliacaoAtendimento") %>'
                                    OnClientClick="return confirm('Confirma a NÃO APROVAÇÃO da resolução deste chamado?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsAvaliacaoAtendimento" runat="server" DataObjectTypeName="Glass.Data.Model.AvaliacaoAtendimento"
                    SelectMethod="GetList" TypeName="Glass.Data.DAL.AvaliacaoAtendimentoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="Label3" runat="server" ForeColor="Red" Text="* Nossa equipe entrará em contato em breve, caso um chamado seja negado."></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>
