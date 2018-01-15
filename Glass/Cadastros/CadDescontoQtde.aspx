<%@ Page Title="Desconto por Quantidade" Language="C#" AutoEventWireup="true" CodeBehind="CadDescontoQtde.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.CadDescontoQtde" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function validar()
        {
            var qtde = FindControl("txtQtde", "input").value;
            if (qtde == "")
            {
                alert("Digite a quantidade.");
                return false;
            }

            var percDesconto = FindControl("txtPercDesconto", "input").value;
            if (percDesconto == "")
            {
                alert("Digite o percentual máximo de desconto.");
                return false;
            }

            return true;
        }
    </script>

    <table align="center">
        <tr>
            <td class="subtitle1">
                Produto:
                <asp:Label ID="lblProd" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdDescontoQtde" runat="server" GridLines="None" ShowFooter="True"
                    AutoGenerateColumns="False" CssClass="gridStyle" DataSourceID="odsDescontoQtde"
                    DataKeyNames="IdDescontoQtde" OnPreRender="grdDescontoQtde_PreRender">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEditar" runat="server" CommandName="Edit" ImageUrl="~/Images/EditarGrid.gif" />
                                <asp:ImageButton ID="imgExcluir" runat="server" CommandName="Delete" ImageUrl="~/Images/ExcluirGrid.gif"
                                    OnClientClick="if (!confirm(&quot;Deseja excluir esse desconto?&quot;)) return false;" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:ImageButton ID="imgAtualizar" runat="server" CommandName="Update" ImageUrl="~/Images/ok.gif"
                                    OnClientClick="if (!validar()) return false;" />
                                <asp:ImageButton ID="imgCancelar" runat="server" CommandName="Cancel" ImageUrl="~/Images/ExcluirGrid.gif" />
                                <asp:HiddenField ID="hdfIdProd" runat="server" Value='<%# Bind("IdProd") %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qtde Mín. p/ Desconto" SortExpression="Qtde">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("Qtde") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" Text='<%# Bind("Qtde") %>' onkeypress="return soNumeros(event, true, true)"
                                    Width="50px"></asp:TextBox>
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtQtde" runat="server" onkeypress="return soNumeros(event, true, true)"
                                    Width="50px"></asp:TextBox>
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Perc. Máx. Desconto" SortExpression="PercDescontoMax">
                            <ItemTemplate>
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("PercDescontoMax") %>'></asp:Label>
                                %
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPercDesconto" runat="server" Text='<%# Bind("PercDescontoMax") %>'
                                    onkeypress="return soNumeros(event, false, true)" Width="50px"></asp:TextBox>
                                %
                            </EditItemTemplate>
                            <FooterTemplate>
                                <asp:TextBox ID="txtPercDesconto" runat="server" onkeypress="return soNumeros(event, false, true)"
                                    Width="50px"></asp:TextBox>
                                %
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <FooterTemplate>
                                <asp:ImageButton ID="imbAdd" runat="server" ImageUrl="~/Images/Insert.gif" OnClick="imbAdd_Click"
                                    OnClientClick="if (!validar()) return false;" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EditRowStyle CssClass="edit" />
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsDescontoQtde" runat="server" SelectMethod="GetByProd"
                    TypeName="Glass.Data.DAL.DescontoQtdeDAO" DataObjectTypeName="Glass.Data.Model.DescontoQtde"
                    DeleteMethod="Delete" UpdateMethod="Update">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="idProd" QueryStringField="idProd" Type="UInt32" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
