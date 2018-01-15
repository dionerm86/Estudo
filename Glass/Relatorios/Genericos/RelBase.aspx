<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelBase.aspx.cs" Inherits="Glass.UI.Web.Relatorios.Genericos.RelBase" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href='~/Style/Geral.css' type="text/css" rel="Stylesheet">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.8.2.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
</head>
<script type="text/javascript">
    function load() {
        var loaded = document.getElementById("<%= hdfLoad.ClientID %>").value == "true";
        if (loaded)
            return;
        
        var itens = document.getElementById("hdfItensRecebidos");
        var parcelas = document.getElementById("hdfParcelas");

        if (itens.value == "")
        {
            var tbProd = window.opener.document.getElementById("lstProd");
            
            if (tbProd)
            {
                var rows = tbProd.getElementsByTagName('tr');

                for (i = 1; i < rows.length; i++)
                {
                    if (rows[i].style.display == "none")
                        continue;

                    var cells = rows[i].getElementsByTagName('td');
                    itens.value += "- " + cells[1].innerHTML + "\r\n";
                }

                if (itens.value == "")
                    itens.value = ".";
            }
        }

        if (parcelas.value == "")
        {
            var tbParc = window.opener.document.getElementById("parcelas");

            if (tbParc)
            {
                var inputs = tbParc.getElementsByTagName("input");
                var itens = new Array();
                for (i = 0; i < inputs.length; i++)
                {
                    if (inputs[i].type.toLowerCase() != "checkbox")
                        continue;

                    if (inputs[i].checked)
                        itens.push(inputs[i].value);
                }

                parcelas.value = itens.length > 0 ? itens.join(",") : "0";
            }
        }

        document.getElementById("<%= hdfLoad.ClientID %>").value = "true";
        document.getElementById("form1").submit();
    }
</script>
<body onload="load()">
    <form id="form1" runat="server">
        <asp:HiddenField ID="hdfLoad" runat="server" />
        <asp:HiddenField ID="hdfItensRecebidos" runat="server" />
        <asp:HiddenField ID="hdfParcelas" runat="server" />
        <asp:PlaceHolder ID="pchTabela" runat="server"></asp:PlaceHolder>
    </form>
</body>
</html>
