<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LancarEstoqueMinimo.aspx.cs"
    Inherits="Glass.UI.Web.Utils.LancarEstoqueMinimo" Title="Lançar Estoque Mínimo" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function validar()
        {
            var valMin = FindControl('txtEstoqueMinimo', 'input').value;
            if (valMin == '')
            {
                alert('Digite o valor mínimo para o estoque.');
                return false;
            }

            return confirm("Deseja atualizar o estoque mínimo de todos os " +
                "produtos desse grupo/subgrupo para " + valMin + "?");
        }
    </script>

    <table align="center">
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label3" runat="server" Text="Loja" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblLoja" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label1" runat="server" Text="Grupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblGrupo" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="Label2" runat="server" Text="Subgrupo" ForeColor="#0066FF"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblSubgrupo" runat="server"></asp:Label>
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
                <table>
                    <tr>
                        <td>
                            Estoque Mìnimo:
                        </td>
                        <td>
                            <asp:TextBox ID="txtEstoqueMinimo" runat="server" Width="50px" onkeypress="return soNumeros(event, false, true)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="btnAtualizar" runat="server" Text="Atualizar" OnClick="btnAtualizar_Click"
                                OnClientClick="if (!validar()) return false" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
