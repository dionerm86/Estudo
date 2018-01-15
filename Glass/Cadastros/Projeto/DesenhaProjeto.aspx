<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DesenhaProjeto.aspx.cs" Inherits="Glass.UI.Web.Cadastros.Projeto.DesenhaProjeto"
    MasterPageFile="~/Layout.master" Title="Edição da Imagem do Projeto" %>

<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">

    <script type='text/javascript' src='<%= ResolveUrl("~/Scripts/wz_jsgraphics.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

    <style type="text/css">
        .toolNormal
        {
            border: solid 1px #ddd;
        }
        .toolOver
        {
            border: solid 1px black;
        }
        .toolClicked
        {
            border: solid 1px red;
        }
        .toolGroup
        {
            text-align: center;
            background-color: #eee;
        }
    </style>

    <script type="text/javascript">
        var jg = null; // Variável de gráficos
        var vetFigura = new Array(); // Armazena as imagens e as posições que as mesmas foram inseridas
        var currTool = null; // Controle de figura sendo utilizado
        var currToolImage = null; // Url da figura sendo utilizada
        var currIdFiguraProjeto = null; // IdFiguraProjeto da figura sendo utilizada

        // Variáveis utilizadas para redesenhar figuras, ao reabrir tela
        var vetIdFiguraProjetoReload = null;
        var vetCoordReload = null;

        // Template para trabalhar com pontos
        function point(x, y)
        {
            this.x = x;
            this.y = y;
        }

        // Template para salvar as figuras
        function figura(idFiguraProjeto, point)
        {
            this.idFiguraProjeto = idFiguraProjeto;
            this.point = point;
        }

        // Retorna a coordenada capturada do mouse
        function getMouseCoord(e)
        {
            if (e.layerX)
                return new point(e.layerX, e.layerY);
            else if (e.offsetX)
                return new point(e.offsetX, e.offsetY);
            else if (e.x)
                return new point(e.x, e.y);
        }

        function onMouseClick(e)
        {
            // Pega o ponto onde a div foi clicada
            var p = getMouseCoord(e);

            if (p == undefined || p.x == 0 || p.y == 0)
                return;

            p.x--;
            p.y--;

            // Salva coordenadas da figura
            vetFigura.push(new figura(currIdFiguraProjeto, p));

            // Desenha figura na tela
            jg.drawImage(currToolImage, p.x, p.y, currTool.width, currTool.height);
            jg.paint();
        }

        function desfazer()
        {
            // Salva coordenadas da figura
            if (vetFigura == null)
                return false;

            var imgRem = vetFigura.pop();

            // Recarrega imagem
            jg.clear();

            for (var j = 0; j < vetFigura.length; j++)
            {
                currTool = FindControl("tool_" + vetFigura[j].idFiguraProjeto + "_", "input");
                jg.drawImage(currTool.src, vetFigura[j].point.x, vetFigura[j].point.y, currTool.height, currTool.width);
            }

            jg.paint();

            return false;
        }

        // Função responsável por redesenhar imagens, em caso de edição
        function carregaFigurasDesenhadas()
        {
            if (vetIdFiguraProjetoReload == null || vetCoordReload == null)
                return;

            var vetIdFiguraProjeto = vetIdFiguraProjetoReload.split(';');
            var vetCoord = vetCoordReload.split('|');

            for (var j = 0; j < vetIdFiguraProjeto.length; j++)
            {
                var coord = vetCoord[j].split(';');
            vetFigura.push(new figura(vetIdFiguraProjeto[j], new point(coord[0], coord[1])));

            currTool = FindControl("tool_" + vetIdFiguraProjeto[j] + "_", "input");
            jg.drawImage(currTool.src, coord[0], coord[1]);
        }

        // Desenha figuras
            jg.paint();
        }

        function tool_OnMouseOver(tool)
        {
            if (tool.getAttribute("class") == "toolNormal")
                tool.setAttribute("class", "toolOver");
        }

        function tool_OnMouseOut(tool)
        {
            if (tool.getAttribute("class") == "toolOver")
                tool.setAttribute("class", "toolNormal");
        }

        // Seta qual das imagens será usada
        function setTool(tool, idFiguraProjeto, toolUrl)
        {
            document.getElementById("ctl00_Pagina_imgFigura").style.cursor = "url(" + toolUrl + "),pointer";

            // Volta a última figura usada para a classe padrão
            if (currTool != null)
                currTool.setAttribute("class", "toolNormal");

            // Altera a borda da figura selecionada para indicar que está sendo usada
            tool.setAttribute("class", "toolClicked");

            // Salva na variável o idFiguraProjeto, a imagem da figura sendo usada e a própria figura
            currIdFiguraProjeto = idFiguraProjeto;
            currToolImage = toolUrl;
            currTool = tool;
        }

        // Salva os pontos
        function salvar()
        {
            var vetIdFiguraProjeto = "";
            var vetCoord = "";

            var i = 0;
            for (i = 0; i < vetFigura.length; i++)
            {
                vetIdFiguraProjeto += vetFigura[i].idFiguraProjeto + ";";
                vetCoord += (vetFigura[i].point.x != "" ? vetFigura[i].point.x : 0) + ";" +
                (vetFigura[i].point.y != "" ? vetFigura[i].point.y : 0) + "|";
            }

            var response = DesenhaProjeto.Salvar('<%= Request["IdItemProjeto"] %>', '<%= Request["IdPecaItemProj"] %>',
            '<%= Request["Item"] %>', vetIdFiguraProjeto, vetCoord).value;

            if (response == null)
            {
                alert("Falha ao salvar posições. Ajax Error.");
                return false;
            }

            response = response.split('\t');

            if (response[0] == "Erro")
            {
                alert(response[1])
                return false;
            }

            alert("Posições salvas.");

            // Em algumas situações este comando estava redirecionando para a página errada quando
            // window.opener.redirectUrl(window.opener.location.href);

            return false;
        }

        function exibeEscondeGrupo(idTr)
        {
            var trGrupo = FindControl(idTr, "tr");
            trGrupo.style.display = trGrupo.style.display == "none" ? "inherit" : "none";
        }

        function limpar()
        {
            vetFigura = new Array();
            jg.clear();

            return false;
        }
         
    </script>

    <table>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td valign="top">
                                        <asp:PlaceHolder ID="pchTool" runat="server"></asp:PlaceHolder>
                                    </td>
                                    <td>
                                        &nbsp;&nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <div id="divCanvas" onclick="onMouseClick(event);" style="position: relative; border: 1px solid gray;"
                                align="center">
                                <asp:Image ID="imgFigura" runat="server" />
                            </div>
                        </td>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClientClick="return salvar();" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnDesfazer" runat="server" Text="Desfazer" OnClientClick="return desfazer();" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnLimpar" runat="server" Text="Limpar" OnClientClick="return limpar();" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnFechar" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
                <colo:VirtualObjectDataSource culture="pt-BR" ID="odsGrupoFigura" runat="server" SelectMethod="GetOrdered"
                    TypeName="Glass.Data.DAL.GrupoFiguraProjetoDAO">
                </colo:VirtualObjectDataSource>
            </td>
        </tr>
    </table>

    <script type="text/javascript">
        // Inicializa a biblioteca de gráficos
        jg = new jsGraphics(find("divCanvas"));

        // Carrega figuras já inseridas nesta image de projeto
        carregaFigurasDesenhadas();
    </script>
</asp:Content>
