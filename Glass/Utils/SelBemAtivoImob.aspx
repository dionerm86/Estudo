<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelBemAtivoImob.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SelBemAtivoImob" Title="Selecione o Bem Principal" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function setBem(idBemAtivoImobilizado)
        {
            window.opener.setBem(idBemAtivoImobilizado);
            closeWindow();
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <table id="tbProd" runat="server">
                    <tr>
                        <td align="center">
                            <asp:GridView ID="grdBemAtivoImobilizado" runat="server" AllowPaging="True" AllowSorting="True"
                                AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="IdBemAtivoImobilizado"
                                DataSourceID="odsBemAtivoImob" GridLines="None" EmptyDataText="Não há bens que podem ser selecionados.">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imgOk" runat="server" ImageUrl="~/Images/ok.gif" OnClientClick='<%# "return setBem(" + Eval("IdBemAtivoImobilizado") + ");" %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cód." SortExpression="IdBemAtivoImobilizado">
                                        <ItemTemplate>
                                            <asp:Label ID="Label9" runat="server" Text='<%# Bind("IdBemAtivoImobilizado") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Produto" SortExpression="DescrProd">
                                        <ItemTemplate>
                                            <asp:Label ID="Label7" runat="server" Text='<%# Eval("CodInternoProd") %>'></asp:Label>
                                            -
                                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("DescrProd") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Plano de Conta Contábil" SortExpression="DescrPlanoContaContabil">
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("DescrPlanoContaContabil") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Tipo" SortExpression="DescrTipo">
                                        <ItemTemplate>
                                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("DescrTipo") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Bem Principal" SortExpression="IdBemAtivoImobilizadoPrinc">
                                        <ItemTemplate>
                                            <asp:Label ID="Label8" runat="server" Text='<%# Eval("IdBemAtivoImobilizadoPrinc") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Núm. Parcelas" SortExpression="NumParc">
                                        <ItemTemplate>
                                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("NumParc") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Data Cadastro" SortExpression="DataCad">
                                        <ItemTemplate>
                                            <asp:Label ID="Label10" runat="server" Text='<%# Bind("DataCad", "{0:d}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle CssClass="pgr" />
                                <EditRowStyle CssClass="edit" />
                                <AlternatingRowStyle CssClass="alt" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <colo:VirtualObjectDataSource culture="pt-BR" ID="odsBemAtivoImob" runat="server" EnablePaging="True" MaximumRowsParameterName="pageSize"
        SelectCountMethod="GetCountReal" SelectMethod="GetListReal" SortParameterName="sortExpression"
        StartRowIndexParameterName="startRow" TypeName="Glass.Data.DAL.BemAtivoImobilizadoDAO">
        <SelectParameters>
            <asp:QueryStringParameter Name="tipo" QueryStringField="tipo" Type="Int32" />
        </SelectParameters>
    </colo:VirtualObjectDataSource>
</asp:Content>
