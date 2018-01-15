<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetCertificado.aspx.cs" Inherits="Glass.UI.Web.Utils.SetCertificado"
    Title="Cadastrar Certificado Digital" MasterPageFile="~/Layout.master" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type="text/javascript">

        function confirmaSenha()
        {
            if (FindControl("txtSenhaCert", "input").value != FindControl("txtConfSenha", "input").value)
            {
                alert("A confirmação da senha está diferente da senha informada.");
                return false;
            }

            return true;
        }

    </script>

    <table>
        <tr>
            <td align="center">
                <asp:Label ID="lblSituacao" runat="server" Font-Bold="True" Font-Italic="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td class="dtvHeader">
                            <asp:Label ID="Label4" runat="server" Text="Certificado Digital"></asp:Label>
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:FileUpload ID="fluFoto" runat="server" />
                                    </td>
                                    <td>
                                        &nbsp;<asp:RequiredFieldValidator ID="rqdArquivo" runat="server" ControlToValidate="fluFoto"
                                            ErrorMessage="Busque o arquivo do certificado." Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader">
                            <asp:Label ID="Label5" runat="server" Text="Senha"></asp:Label>
                        </td>
                        <td align="left">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtSenhaCert" runat="server" MaxLength="30" TextMode="Password"
                                            Width="120px"></asp:TextBox>
                                    </td>
                                    <td>
                                        &nbsp;<asp:RequiredFieldValidator ID="rqdArquivo0" runat="server" ControlToValidate="txtSenhaCert"
                                            ErrorMessage="Informe a senha do certificado." Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="dtvHeader">
                            <asp:Label ID="Label6" runat="server" Text="Confirmar Senha"></asp:Label>
                        </td>
                        <td align="left">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtConfSenha" runat="server" MaxLength="30" TextMode="Password"
                                            Width="120px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="rqdArquivo1" runat="server" ControlToValidate="txtConfSenha"
                                            ErrorMessage="Confirme a senha do certificado." Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClick="btnConfirmar_Click"
                                OnClientClick="if(!confirmaSenha()) return false;" />
                        </td>
                        <td>
                            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CausesValidation="False"
                                OnClientClick="closeWindow();" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
