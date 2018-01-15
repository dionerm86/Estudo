<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelContaBanco.aspx.cs" Inherits="Glass.UI.Web.Utils.SelContaBanco"
    Title="Selecione a conta bancária" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center" class="subtitle1">
                <asp:Label ID="lblSubtitulo" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView GridLines="None" ID="grdContaBanco" runat="server" AutoGenerateColumns="False"
                    DataSourceID="odsContaBanco" CssClass="gridStyle" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EditRowStyle-CssClass="edit" DataKeyNames="IdContaBanco" OnRowCommand="grdContaBanco_RowCommand">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/ok.gif" OnClientClick="if (!confirm('Deseja atribuir essa conta bancária ao cheque?')) return false"
                                    CommandName="Selecionar" CommandArgument='<%# Eval("IdContaBanco") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="Agencia" HeaderText="Agência" SortExpression="Agencia" />
                        <asp:BoundField DataField="Conta" HeaderText="Conta" SortExpression="Conta" />
                    </Columns>
                    <PagerStyle />
                    <EditRowStyle />
                    <AlternatingRowStyle />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsContaBanco" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.ContaBancoDAO">
                </colo:VirtualObjectDataSource>
                <asp:HiddenField ID="hdfNfe" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
