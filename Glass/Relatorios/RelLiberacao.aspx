<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelLiberacao.aspx.cs" Inherits="Glass.UI.Web.Relatorios.RelLiberacao" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href='~/Style/Geral.css' type="text/css" rel="Stylesheet">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.8.2.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
    
        function load()
        {
            var init = <%= !String.IsNullOrEmpty(Request["init"]) ? Request["init"] : "0" %>;
            var loaded = document.getElementById("<%= hdfInit.ClientID %>").value == "true";
            
            if (init == 0 || loaded)
            {
                document.getElementById("<%= hdfLoad.ClientID %>").value = "true";
                return;
            }
            
            switch (init)
            {
                case 1:
                    var obs = window.opener.document.getElementById("ctl00_Conteudo_txtObservacoes");
                    if (obs != null)
                        document.getElementById("<%= txtObs.ClientID %>").value = obs.value;
                    break;
            }
            
            document.getElementById("<%= hdfInit.ClientID %>").value = "true";
            document.getElementById("<%= hdfLoad.ClientID %>").value = "true";
            document.forms[0].submit();
        }
    
    </script>
</head>
<body onload="load()">
    <form id="form1" runat="server">
        <asp:TextBox ID="txtObs" runat="server" style="display: none"></asp:TextBox>
        <asp:HiddenField ID="hdfInit" runat="server" Value="false" />
        <asp:HiddenField ID="hdfLoad" runat="server" Value="false" />
        <asp:PlaceHolder ID="pchTabela" runat="server"></asp:PlaceHolder>
    </form>
</body>
</html>
