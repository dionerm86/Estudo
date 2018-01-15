<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetMotivoCancAntecipContaRec.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetMotivoCancAntecipContaRec" Title="Cancelamento de Antecipação de Boleto"
    MasterPageFile="~/Layout.master" %>

<%@ Register Src="../Controls/ctrlData.ascx" TagName="ctrlData" TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function cancelar()
        {

            if (FindControl("txtMotivo", "textarea").value == "")
            {
                alert("Informe o motivo do cancelamento.");
                return false;
            }

            if (!confirm("Tem certeza que deseja cancelar este Pedido?"))
                return false;
        }
   
    </script>

    <table cellpadding="0" cellspacing="0" align="center">
        <tr>
            <td align="center">
                <table width="100%" cellpadding="4" cellspacing="0">
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
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:CheckBox ID="chkGerarEstorno" runat="server" Text="Gerar Estorno Bancário" OnCheckedChanged="chkGerarEstorno_CheckedChanged"
                                            AutoPostBack="true" />
                                    </td>
                                </tr>
                            </table>
                            <table runat="server" id="tbEstornoBanco" visible="false">
                                <tr>
                                    <td align="center">
                                        Data do Estorno Bancário:
                                    </td>
                                    <td>
                                        <uc1:ctrlData ID="ctrlDataEstorno" runat="server" ReadOnly="ReadWrite" ValidateEmptyText="True" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClientClick="return cancelar();"
                    Style="margin: 4px" OnClick="btnConfirmar_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
