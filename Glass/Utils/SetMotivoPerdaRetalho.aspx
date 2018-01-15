<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetMotivoPerdaRetalho.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetMotivoPerdaRetalho" Title="Perda de Retalho de Produção"
    MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlTipoPerda.ascx" TagName="ctrlTipoPerda" TagPrefix="uc1" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function validate() {

            if (!confirm("Tem certeza que deseja marcar perda nesse retalho?"))
                return false;

            if (FindControl("txtMotivo", "textarea").value == "") {
                alert("Informe o motivo.");
                return false;
            }

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
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="Tipo de Perda:"></asp:Label></td>
                                    <td>
                                        <uc1:ctrlTipoPerda ID="ctrlTipoPerda1" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
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
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClick="btnConfirmar_Click"
                    OnClientClick="return validate();" Style="margin: 4px" />
            </td>
        </tr>
    </table>
</asp:Content>
