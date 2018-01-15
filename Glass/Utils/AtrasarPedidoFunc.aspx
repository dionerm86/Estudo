<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AtrasarPedidoFunc.aspx.cs"
    Inherits="Glass.UI.Web.Utils.AtrasarPedidoFunc" MasterPageFile="~/Layout.master" Title="Atrasar Pedidos" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function validar()
        {

        }
    </script>

    <table align="center">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label3" runat="server" Text="Funcionário" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtNomeFunc" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgPesq" runat="server" ImageUrl="~/Images/Pesquisar.gif" OnClick="imgPesq_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="grdFuncionario" runat="server" AutoGenerateColumns="False" CssClass="gridStyle"
                    DataKeyNames="IdFunc" DataSourceID="odsFuncionario" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" />
                        <asp:BoundField DataField="DescrTipoFunc" HeaderText="Tipo" SortExpression="DescrTipoFunc" />
                        <asp:BoundField DataField="NomeLoja" HeaderText="Loja" SortExpression="NomeLoja" />
                        <asp:TemplateField HeaderText="Núm. Dias Atrasar" SortExpression="NumDiasAtrasarPedido">
                            <ItemTemplate>
                                <asp:TextBox ID="txtAtrasar" runat="server" onkeypress="return soNumeros(event, true, true)"
                                    Text='<%# Eval("NumDiasAtrasarPedido") %>' Width="50px"></asp:TextBox>
                                dia(s)
                                <asp:HiddenField ID="hdfIdFunc" runat="server" Value='<%# Eval("IdFunc") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar atrasos" OnClick="btnSalvar_Click" />
            </td>
        </tr>
        <tr>
            <td align="center">
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsFuncionario" runat="server" SelectMethod="GetVendedores"
                    TypeName="Glass.Data.DAL.FuncionarioDAO">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="txtNomeFunc" Name="nome" PropertyName="Text" Type="String" />
                    </SelectParameters>
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>
</asp:Content>
