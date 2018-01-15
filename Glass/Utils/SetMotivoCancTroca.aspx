<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetMotivoCancTroca.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetMotivoCancTroca" Title="Cancelamento de Troca/Devolução" MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">
        function validate()
        {
            if (!confirm("Tem certeza que deseja cancelar?"))
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
                                        Data do Estorno Bancário
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
                    OnClientClick="return validate();" Style="margin: 4px" />
            </td>
        </tr>
    </table>
</asp:Content>
