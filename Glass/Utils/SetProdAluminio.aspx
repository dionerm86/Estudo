<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetProdAluminio.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetProdAluminio" Title="Inserir Produtos Alumínio" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function validate()
        {

            if (!confirm("Inserir produtos?"))
                return false;
        }
    </script>

    <table cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <table cellpadding="4" cellspacing="0">
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label2" runat="server" Text="Produto:" Font-Bold="True"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblProd" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" Text="Qtd.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtd1" runat="server" MaxLength="3" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label13" runat="server" Text="ml.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMl1" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label4" runat="server" Text="Qtd.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtd2" runat="server" MaxLength="3" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label14" runat="server" Text="ml.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMl2" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" Text="Qtd.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtd3" runat="server" MaxLength="3" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label15" runat="server" Text="ml.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMl3" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label6" runat="server" Text="Qtd.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtd4" runat="server" MaxLength="3" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label16" runat="server" Text="ml.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMl4" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label7" runat="server" Text="Qtd.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtd5" runat="server" MaxLength="3" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label17" runat="server" Text="ml.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMl5" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label8" runat="server" Text="Qtd.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtd6" runat="server" MaxLength="3" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label18" runat="server" Text="ml.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMl6" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label9" runat="server" Text="Qtd.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtd7" runat="server" MaxLength="3" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label19" runat="server" Text="ml.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMl7" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label10" runat="server" Text="Qtd.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtd8" runat="server" MaxLength="3" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label20" runat="server" Text="ml.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMl8" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label11" runat="server" Text="Qtd.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtd9" runat="server" MaxLength="3" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label21" runat="server" Text="ml.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMl9" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label12" runat="server" Text="Qtd.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtQtd10" runat="server" MaxLength="3" Width="40px" onkeypress="return soNumeros(event, true, true);"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label22" runat="server" Text="ml.:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMl10" runat="server" onkeypress="return soNumeros(event, false, true);"
                                            Width="50px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClick="btnConfirmar_Click"
                    OnClientClick="return validate();" Style="margin: 4px" />
            </td>
        </tr>
    </table>
</asp:Content>
