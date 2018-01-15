<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Glass.UI.Web.WebGlassParceiros.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>WebGlass Parceiros</title>
    <link rel="stylesheet" type="text/css" href="../Style/Geral.css"/>
    <link runat="server" rel="shortcut icon" href="~/Images/favicon.ico" type="image/x-icon"/>
    <meta name="robots" content="noindex"/>
    <meta name="googlebot" content="noindex"/>
    <style type="text/css">
                .style5
        {
            width: 50%;
        }
        .style7
        {
            width: 1040px;
        }
        .style8
        {
            width: 1035px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <table border="0" cellspacing="0" cellpadding="0" height="100%">
        <tr>
            <td class="style5">
            </td>
            <td class="style8">
                <table border="0" cellspacing="0" cellpadding="0" class="loginParceiros">
                    <tr valign="top">
                        <td style="vertical-align: middle;" align="center">
                                        <asp:Image ID="imgBanner" runat="server" 
                                            ImageUrl="~/Images/banner_parceiros.jpg" />
                        </td>
                    </tr>
                    <tr valign="top" align="center">
                        <td height="400" align="center" valign="middle" class="style7">
                            <table align="center">
                                <tr>
                                    <td class="subtitle" align="center">
                                        <asp:Image ID="imgLogo" runat="server" />
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
                                                <td align="right" style="width: 70px;">
                                                    Usuário&nbsp;&nbsp;&nbsp;
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtUsuario" runat="server" MaxLength="20" Width="155px"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td align="right" style="width: 70px;">
                                                    Senha&nbsp;&nbsp;&nbsp;</td>
                                                <td>
                                                    <asp:TextBox ID="txtSenha" runat="server" MaxLength="20" TextMode="Password" 
                                                        Width="155px"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <asp:Button ID="btnLogin" runat="server" Text="ENTRAR" OnClick="btnLogin_Click" 
                                            BackColor="#003366" BorderColor="#336699" ForeColor="White" Width="147px" />
                                    </td>
                                </tr>
                            </table>
                    </tr>
                    <tr>
                        <td class="style7">
                            <asp:Label ID="lblRodape" runat="server" Font-Bold="True" Font-Size="Small" 
                                ForeColor="Silver" Text="Sync Softwares - (31) 3063 5551"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td width="50%">
            </td>
        </tr>
    </table>
    </form>
</body>
