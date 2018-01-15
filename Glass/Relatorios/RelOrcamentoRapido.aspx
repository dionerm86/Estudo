<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelOrcamentoRapido.aspx.cs" Inherits="Glass.UI.Web.Relatorios.RelOrcamentoRapido" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Orçamento Rápido</title>
    <link href='~/Style/Geral.css' type="text/css" rel="Stylesheet">
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.8.2.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
</head>
<script type="text/javascript">
    function load() {
        try {
            if (document.getElementById("hdfTabelaProdutos").value == "") {
                var tbProd = window.opener.document.getElementById("lstProd");
                var rows = tbProd.getElementsByTagName('tr');

                var hidden = document.getElementById("hdfTabelaProdutos");

                for (i = 1; i < rows.length - 1; i++) {
                    if (rows[i].style.display == "none")
                        continue;

                    var cells = rows[i].getElementsByTagName('td');

                    for (j = 1; j < cells.length; j++)
                        hidden.value += cells[j].innerHTML + "\t";

                    hidden.value += "|";
                }
                
                if (hidden.value == "") {
                    alert("Inclua pelo menos um produto na lista de produtos para exibir o relatório.");
                    closeWindow();
                }

                document.getElementById("form1").submit();
            }
        }
        catch (err) {
            alert(err);
        }
    }
</script>
<body onload="load()">
    <form id="form1" runat="server">
        <asp:HiddenField ID="hdfInit" runat="server" Value="false" />
        <asp:HiddenField ID="hdfTabelaProdutos" runat="server" />
        <asp:PlaceHolder ID="pchTabela" runat="server"></asp:PlaceHolder>
    </form>
</body>
</html>
