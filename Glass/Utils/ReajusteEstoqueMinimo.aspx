<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReajusteEstoqueMinimo.aspx.cs"
    Inherits="Glass.UI.Web.Utils.ReajusteEstoqueMinimo" Title="Reajuste de Estoque Mínimo" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function validate()
        {

            var valor = FindControl("txtReajuste", "input").value;

            if (valor == "")
            {
                alert("Informe um valor de reajuste.");
                return false;
            }
            else
            {
                return true;
            }
        }
    </script>

    <table cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <asp:Label ID="lblFiltros" runat="server"></asp:Label>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:TextBox ID="txtResultados" runat="server" ForeColor="Black" Height="139px" ReadOnly="True"
                    TextMode="MultiLine" Visible="False" Width="450px"></asp:TextBox>
                <br />
                <br />
                <asp:Label ID="lblSucesso" runat="server"></asp:Label>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td align="center" style="vertical-align: middle;">
                <asp:Label ID="Label1" runat="server" Text="Reajuste: " Font-Size="Small"></asp:Label>
                <asp:TextBox ID="txtReajuste" runat="server" Width="50px" onkeypress="return soNumeros(event, false, false);"
                    MaxLength="6"></asp:TextBox>
                &nbsp;<asp:RadioButton ID="rdbPorcentagem" runat="server" Checked="True" Font-Bold="True"
                    Font-Size="Medium" GroupName="tipoReajuste" Text="%" />
                &nbsp;
                <asp:RadioButton ID="rdbQuantidade" runat="server" Font-Bold="True" Font-Size="Medium"
                    GroupName="tipoReajuste" Text="Qtd" />
            </td>
        </tr>
        <tr>
            <td align="center" class="style1">
                <br />
                <asp:Label ID="Label3" runat="server" Text="Utilize números negativos para diminuir o estoque mínimo."
                    Font-Size="Small"></asp:Label><br/>
                
                <asp:Label ID="Label2" runat="server" Text="Produtos com resultados de reajuste negativos, não serão alterados." ForeColor="Red"
                    Font-Size="Small"></asp:Label><br/>
                
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClientClick="return validate();"
                    OnClick="btnConfirmar_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
