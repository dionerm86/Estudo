<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TrocarSenha.aspx.cs" Inherits="Glass.UI.Web.Utils.TrocarSenha"
    Title="Trocar senha" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function confirmPass(src, args)
        {
            args.IsValid = (FindControl("txtNova","input").value == FindControl("txtConfirmar","input").value);
        }

    </script>

    <table cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <table cellpadding="4" cellspacing="0">
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" CssClass="dtvHeader" Text="Nova senha:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtNova" runat="server" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valNova" runat="server" ControlToValidate="txtNova"
                                ErrorMessage="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" CssClass="dtvHeader" Text="Confirmar nova senha:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtConfirmar" runat="server" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valConfirm" runat="server" ControlToValidate="txtConfirmar"
                                ErrorMessage="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClick="btnConfirmar_Click"
                    Style="margin: 4px" />
                <br />
                <asp:Label ID="lblOk" runat="server" Font-Bold="False" ForeColor="#009933" Text="Senha alterada com sucesso!"
                    Visible="False"></asp:Label>
                <asp:CustomValidator ID="CustomValidator1" runat="server" ClientValidationFunction="confirmPass"
                    ControlToValidate="txtConfirmar" ErrorMessage="Confirmação de senha não confere."
                    Font-Bold="False" Display="Dynamic"></asp:CustomValidator>
                <asp:HiddenField ID="hdnSenha" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
