<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlFoto.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlFoto" %>
<table cellspacing="0">
    <tr>
        <td align="center" colspan="2">
            <asp:ImageButton ID="imgImagem" runat="server" />
        </td>
    </tr>
    <tr>
        <td align="left">
            <asp:LinkButton ID="lnkRemover" CausesValidation="false" runat="server" CommandName="Delete" OnClientClick="return confirm('Deseja mesmo remover esta foto?')" OnClick="lnkRemover_Click"><img src="../Images/ExcluirGrid.gif" border="0" /> Remover</asp:LinkButton>&nbsp;</td>
        <td align="right">
            <asp:LinkButton ID="lnkEditar" runat="server" OnClientClick="return alterarLegenda('panEditar', this);"><img src="../Images/EditarGrid.gif" border="0" /> Editar</asp:LinkButton></td>
    </tr>
    <tr>
        <td align="center" colspan="2">
            <asp:LinkButton ID="lnkCalc" runat="server" onclick="lnkCalc_Click"><img src="../Images/calculator.gif" border="0" /> Cálculo de Área</asp:LinkButton></td>
    </tr>
    <tr>
        <td align="center" colspan="2">
            <asp:Label ID="lblLegenda" runat="server"></asp:Label><br />
            <asp:Panel ID="panEditar" runat="server" Style="display: none; margin-top: -16px" CssClass="larguraTela">
                <table cellspacing="0" class="larguraTela">
                    <tr>
                        <td style="padding-right: 8px">
                            <asp:TextBox ID="txtEditar" runat="server" MaxLength="100" Rows="3" TextMode="MultiLine"></asp:TextBox></td>
                        <td align="right" style="white-space: nowrap">
                            <asp:ImageButton ID="imgEditar" runat="server" ImageUrl="~/Images/ok.gif"
                                OnClick="imgEditar_Click" ToolTip="Atualizar" CausesValidation="False" />
                            <asp:ImageButton ID="imgExcluir" runat="server" ImageUrl="~/Images/ExcluirGrid.gif"
                                OnClientClick="return cancelarEdicao('panEditar', this);" ToolTip="Cancelar" /></td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
