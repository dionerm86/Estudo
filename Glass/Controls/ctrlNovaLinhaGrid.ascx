<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctrlNovaLinhaGrid.ascx.cs" Inherits="Glass.UI.Web.Controls.ctrlNovaLinhaGrid" %>

<div id="<%= this.ClientID %>" style="position: absolute; margin-top: 4px">
    <%= GetHtmlExibir() %>
</div>
<script type="text/javascript">
    $(document).ready(function()
    {
        var controle = document.getElementById("<%= this.ClientID %>")

        var celula = controle;
        while (celula.nodeName.toLowerCase() != "td")
            celula = celula.parentNode;

        var linha = celula.parentNode;
        
        var paddingTop = <%= this.PaddingTop %>;
        if (paddingTop > 0)
            controle.style.paddingTop = paddingTop + "px";
        
        var heightDiv = controle.offsetHeight;
        celula.style.height = (celula.offsetHeight + heightDiv) + "px";

        var celulasLinha = linha.cells;

        for (i = 0; i < celulasLinha.length; i++)
        {
            var controles = celulasLinha[i].children;

            for (j = 0; j < controles.length; j++)
            {
                if (controles[j].id != controle.id)
                {
                    controles[j].style.position = "relative";
                    controles[j].style.bottom = (heightDiv / 2) + "px";
                }
                else
                {
                    var marginTop = parseInt(controle.style.marginTop.replace("px", ""), 10);
                    var padding = (celula.clientHeight - parseInt(celula.style.height.replace("px", ""), 10)) / 2;
                    
                    controle.style.marginTop = marginTop - (heightDiv / 2) + "px";
                    controle.style.width = (linha.offsetWidth - (padding * 2)) + "px";

                    controle.style.marginLeft = -padding + "px";
                    controle.style.padding = padding + "px";
                    
                    if (paddingTop > 0)
                        controle.style.paddingTop = paddingTop + "px";
                }
            }
        }
    });
</script>