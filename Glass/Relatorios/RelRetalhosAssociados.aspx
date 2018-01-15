<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelRetalhosAssociados.aspx.cs" Inherits="Glass.UI.Web.Relatorios.RelRetalhosAssociados" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Retalhos Associados</title>
    <link href='~/Style/Geral.css' type="text/css" rel="Stylesheet" />
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/jquery/jquery-1.8.2.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
        function load()
        {
            if (document.getElementById("hdfTabelaProdutos").value == "")
            {
                var tbProd = window.opener.document.getElementById("lstRetalhos");
                var rows = tbProd.getElementsByTagName('tr');

                var hidden = document.getElementById("hdfTabelaProdutos");

                for (i = 1; i < rows.length; i++)
                {
                    if (rows[i].style.display == "none" || rows[i].id.indexOf("lstRetalhos") == -1)
                        continue;

                    var texto = "";
                    var cells = rows[i].getElementsByTagName('td');

                    for (j = 1; j < 9; j++)
                        texto += cells[j].innerHTML + "\t";

                    var inputs = cells[9].getElementsByTagName("input");

                    var dados = [];
                    for (j = 0; j < inputs.length; j++)
                    {
                        if (inputs[j].type.toLowerCase() == "checkbox" && inputs[j].checked)
                            dados.push(inputs[j].nextElementSibling.innerHTML.replace("<b>", "").replace("</b> - ", "\t"));
                    }

                    if (dados.length > 0)
                        hidden.value += texto + dados.join(", ") + "|";
                }

                if (hidden.value == "")
                {
                    alert("Inclua pelo menos um produto na lista de produtos para exibir o relatório.");
                    closeWindow();
                }

                document.getElementById("form1").submit();
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hdfTabelaProdutos" runat="server" />
        <asp:PlaceHolder ID="pchTabela" runat="server"></asp:PlaceHolder>
    </form>
    <script type="text/javascript">
        load();
    </script>
</body>
</html>
