<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetMotivoInutCTe.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetMotivoInutCTe" Title="Inutiliza��o de CTe" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function validate()
        {

            if (!confirm("Tem certeza que deseja inutilizar este CTe?"))
                return false;

            var motivo = FindControl("txtMotivo", "textarea").value;

            if (motivo == "")
            {
                alert("Informe o motivo da inutiliza��o.");
                return false;
            }

            if (motivo.length < 20)
            {
                alert("O motivo da inutiliza��o deve ter no m�nimo 20 caracteres.");
                return false;
            }

            if (motivo.length > 250)
            {
                alert("O motivo da inutiliza��o deve ter no m�ximo 250 caracteres.");
                return false;
            }

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
    <asp:Button ID="btnCancelar" runat="server" Text="Voltar" OnClientClick="window.close();"
        Style="margin: 4px" />
</asp:Content>
