<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetMotivoCancProj.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetMotivoCancProj" Title="Cancelamento de " MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript">
        function validate()
        {
            if (!confirm("Tem certeza que deseja cancelar est" + "<%= GetValidate() %>" + "?"))
                return false;

            if (FindControl("txtMotivo", "textarea").value == "")
            {
                alert("Informe o motivo do cancelamento.");
                return false;
            }

            //FindControl("btnConfirmar", "input").disabled = true;
            return true;
        }
    </script>

    <table cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <table cellpadding="4" cellspacing="0">
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label1" runat="server" Text="Motivo:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMotivo" runat="server" MaxLength="250" TextMode="MultiLine" Width="329px"></asp:TextBox>
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
