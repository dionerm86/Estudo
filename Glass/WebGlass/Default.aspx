<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Glass.UI.Web.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>WebGlass</title>

    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/Geral.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>

    <link runat="server" rel="shortcut icon" href="~/Images/favicon.ico" type="image/x-icon"/>

    <meta name="robots" content="noindex"/>
    <meta name="googlebot" content="noindex"/>

</head>
<body>
    <form id="form1" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" width="100%" height="100%">
        <tr>
            <td width="50%">
            </td>
            <td>
                <table border="0" cellspacing="0" cellpadding="0" width="780" class="loginCabecalho">
                    <tr>
                        <td align="left" width="50%" valign="top">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <asp:Image ID="imgLogoWebGlass" runat="server" ImageUrl="~/Images/webglass.png" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td>
                                        &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
                                    </td>
                                    <td>
                                        <table>
                                            <tr>
                                                <td class="textoLogo">
                                                    <asp:Label ID="lblTelSuporte" runat="server" Text="Suporte: (31) 2571-7070"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="textoLogo">
                                                    Email: <a id="emailSuporte" runat="server" href="mailto:suporte@syncsoftwares.com.br">suporte@syncsoftwares.com.br</a>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td align="right" valign="top" style="height: 85px;">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:Image ID="imgLogoSync" runat="server" ImageUrl="~/Images/syncsoftwares.png"
                                            Height="75px" Width="141px" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table border="0" cellspacing="0" cellpadding="0" width="780" class="login">
                    <tr valign="top">
                        <td style="vertical-align: middle; background-color: White; height: 152px;" align="center">
                            <table align="center">
                                <tr>
                                    <td align="left">
                                        &nbsp;
                                    </td>
                                    <td>
                                        <asp:Image ID="imgLogo" runat="server" />
                                    </td>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr valign="top" align="center">
                        <td align="center" valign="middle" style="height: 350px; background-color: White;">
                            <table align="center">
                                <tr>
                                    <td class="subtitle" align="center">
                                        Login
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td align="left" style="width: 70px;">
                                                    Usuário:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtUsuario" runat="server" MaxLength="20"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td align="left" style="width: 70px;">
                                                    Senha:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtSenha" runat="server" MaxLength="20" TextMode="Password"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
                                    </td>
                                </tr>
                            </table>
                    </tr>
                </table>
            </td>
            <td width="50%">
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
