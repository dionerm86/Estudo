<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetFinalizarFinanceiro.aspx.cs"
    Inherits="Glass.UI.Web.Utils.SetFinalizarFinanceiro" Title="Finalizar/Confirmar Pedido pelo Financeiro"
    MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <th align="left" colspan="2">
                            Observações
                        </th>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:TextBox ID="txtObs" runat="server" Columns="50" Rows="4" TextMode="MultiLine"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvObs" runat="server" ErrorMessage="*" ControlToValidate="txtObs"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th align="left" width="1px" nowrap="nowrap">
                            Aprovado?
                        </th>
                        <td>
                            <asp:DropDownList ID="drpAprovado" runat="server">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Value="True">Sim</asp:ListItem>
                                <asp:ListItem Value="False">Não</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvAprovado" runat="server" ErrorMessage="*" ControlToValidate="drpAprovado"
                                Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:Button ID="btnExecutar" runat="server" Text="Finalizar/Confirmar" OnClick="btnExecutar_Click" />
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow(); return false" />
            </td>
        </tr>
    </table>
</asp:Content>
