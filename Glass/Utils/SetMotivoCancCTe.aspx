<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetMotivoCancCTe.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetMotivoCancCTe" Title="Cancelamento de CTe" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function validate()
        {

            if (!confirm("Tem certeza que deseja cancelar este CTe?"))
                return false;

            var motivo = FindControl("txtMotivo", "textarea").value;

            if (motivo == "")
            {
                alert("Informe o motivo do cancelamento.");
                return false;
            }

            if (motivo.length < 20)
            {
                alert("O motivo do cancelamento deve ter no m�nimo 20 caracteres.");
                return false;
            }

            if (motivo.length > 250)
            {
                alert("O motivo do cancelamento deve ter no m�ximo 250 caracteres.");
                return false;
            }

            //FindControl("btnConfirmar", "input").disabled = true;
            return true;
        }
    </script>

    <table cellpadding="4" cellspacing="0">
        <tr>
            <td align="center">
                <asp:Label ID="Label1" runat="server" Text="Motivo:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtMotivo" runat="server" MaxLength="250" TextMode="MultiLine" Width="329px"></asp:TextBox>
            </td>
        </tr>
    </table>
    <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClick="btnConfirmar_Click"
        OnClientClick="return validate();" Style="margin: 4px" />
</asp:Content>
