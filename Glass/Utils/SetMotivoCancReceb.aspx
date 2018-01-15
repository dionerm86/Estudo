<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetMotivoCancReceb.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetMotivoCancReceb" Title="Cancelamento de Recebimento" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        var clicked = false;
        function validar()
        {
            if (!validate())
                return false;

            var motivo = FindControl("txtMotivo", "textarea");
            var estornar = FindControl("chkEstornar", "input");

            if (motivo == null || motivo.value == "")
            {
                alert("Informe o motivo do cancelamento.");
                return false;
            }

            if (estornar != null && estornar.checked)
                if (FindControl("ctrlDataEstorno_txtData", "input").value == "")
            {
                alert("A data do estorno n�o pode ser vazia.");
                return false;
            }

            if (!confirm("Tem certeza que deseja cancelar este Recebimento?"))
                return false;

            if (clicked)
                return false;

            clicked = true;

            return true;
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
                                        <asp:Label ID="Label1" runat="server" Text="Motivo:"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtMotivo" runat="server" MaxLength="250" TextMode="MultiLine" Width="329px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                            <table runat="server" id="estornoBanco">
                                <tr>
                                    <td>
                                        Data do Estorno Banc�rio
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataEstorno" runat="server" ReadOnly="ReadWrite" ValidateEmptyText="True" />

                                        <script type="text/javascript">
                                            FindControl("chkEstornar", "input").onclick();
                                        </script>

                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClick="btnConfirmar_Click"
                    OnClientClick="return validar();" Style="margin: 4px" />
            </td>
        </tr>
    </table>
</asp:Content>
