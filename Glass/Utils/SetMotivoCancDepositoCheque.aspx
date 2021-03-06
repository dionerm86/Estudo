﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Layout.master" AutoEventWireup="true" CodeBehind="SetMotivoCancDepositoCheque.aspx.cs" Inherits="Glass.UI.Web.Utils.SetMotivoCancDepositoCheque" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Menu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Pagina" runat="server">
    <script type="text/javascript">
        function validar() {
            if (!validate())
                return false;

            if (FindControl("txtMotivo", "textarea").value == "") {
                alert("Informe o motivo do cancelamento.");
                return false;
            }

            if (FindControl("chkEstornar", "input").checked)
                if (FindControl("ctrlDataEstorno_txtData", "input").value == "") {
                    alert("A data do estorno não pode ser vazia.");
                    return false;
                }

            if (!confirm("Tem certeza que deseja cancelar este depósito?"))
                return false;

            return true;
        }

        function habilitarData(habilitar) {
            FindControl("ctrlDataEstorno_txtData", "input").disabled = !habilitar;
            FindControl("ctrlDataEstorno_imgData", "input").disabled = !habilitar;

            if (!habilitar)
                FindControl("ctrlDataEstorno_txtData", "input").value = "";
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
                                        <asp:CheckBox ID="chkEstornar" runat="server" Text="Estornar movimentações bancárias"
                                            onclick="habilitarData(this.checked)" />
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataEstorno" runat="server" ReadOnly="ReadWrite" />

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
                    OnClientClick="return validar();" Style="margin: 4px; height: 22px;" />
            </td>
        </tr>
    </table>
</asp:Content>
