<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaTotalLiberacao.aspx.cs" Inherits="Glass.UI.Web.Utils.ListaTotalLiberacao" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Valores Totais das Liberações</title>
    
    <link type="text/css" rel="Stylesheet" href="<%= ResolveUrl("~/Style/StyleProd.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>"/>

    <style type="text/css">
        #form1
        {
            text-align: center;
        }
        .style1
        {
            text-align: right;
        }
        .style2
        {
            text-align: left;
        }
        .style3
        {
            height: 16px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <script type="text/javascript">
        function fechar() {
            window.close();
        }
    </script>
    
    <div>
        <table align="center">
            <tr>
                <td align="center" class="subtitle" colspan="2">
                    Valores Totais das Liberações
                </td> 
            </tr>
            <tr><td class="style1">
                <asp:Label ID="Label4" runat="server" 
                    Text="&lt;b/&gt;Total em R$ das liberações:&lt;/b&gt;&amp;nbsp;"></asp:Label>
                </td>
            <td class="style2">
            
                <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            
            </td></tr>
            <tr><td class="style1">
                <asp:Label ID="Label5" runat="server" 
                    Text="&lt;b/&gt;Total em m² das liberações:&lt;/b&gt;&amp;nbsp;"></asp:Label>
                </td>
            <td class="style2">
            
                <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
            
            </td></tr>
            <tr><td class="style1">
                <asp:Label ID="Label6" runat="server" 
                    Text="&lt;b/&gt;Peso total das liberações:&lt;/b&gt;&amp;nbsp;"></asp:Label>
                </td>
            <td class="style2">
            
                <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
            
            </td></tr>
            <tr>
            <td colspan="2" class="style3">
                &nbsp;</td>
            </tr>
            
            <tr><td colspan="2">
                <asp:Button ID="Button1" runat="server" onclientclick="fechar(); return false;" 
                    Text="Fechar" />
                </td></tr>
        </table>
        
    </div>
    </form>
</body>
</html>
