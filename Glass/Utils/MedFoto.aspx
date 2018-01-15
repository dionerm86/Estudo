<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MedFoto.aspx.cs" Inherits="Glass.UI.Web.Utils.MedFoto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Medição</title>
    <script type="text/javascript" src="../Scripts/wz_jsgraphics.js"></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Utils.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>
    <script type="text/javascript">
        var jg = null; // Variável de gráficos
        var vetX = new Array(); // pontos X capturados
        var vetY = new Array(); // pontos Y capturados
        var escala = false; // Controla se está ou não sendo marcada uma escala
        var p1Escala = null; // Ponto inicial da escala
        var p2Escala = null; // Ponto final da escala
        var corEscala = "#0000FF"; // Cor da linha da escala
        var corArea = "#FF0000"; // Cor das linhas utilizada no cálculo da área
        var pontoCentral = null; // Ponto central do polígono
        var areaPoligono = 0; // Área do polígono desenhado
        var perimetroPoligono = 0; // Perímetro do polígono

        // Retorna a distância, em metros, entre dois pontos
        function distancia(p1, p2) {
            var distPixels = Math.sqrt(Math.pow(p1.x - p2.x, 2) + Math.pow(p1.y - p2.y, 2));
            
            // Pega o tamanho da escala em pixels e em metros
            var escalaPixels = Math.sqrt(Math.pow(p1Escala.x - p2Escala.x, 2) + Math.pow(p1Escala.y - p2Escala.y, 2));
            var escalaMetros = FindControl("txtEscala", "input").value.toString().replace(',', '.') / 100;

            // Aplica regra de três para converter a distância de pixels para metros
            var distMetros = (distPixels * escalaMetros) / escalaPixels;
            
            return distMetros;
        }

        // Calcula o ponto central do polígono
        function getPontoCentral() {
            var somatorioX = 0;
            var somatorioY = 0;

            for (var i = 0; i < vetX.length; i++) {
                somatorioX += vetX[i];
                somatorioY += vetY[i];
            }

            pontoCentral = new point(somatorioX / vetX.length, somatorioY / vetY.length);
        }
        
        // Calcula a área do polígono
        function calcArea() {
            if (vetX.length < 3)
                return;
            
            var qtdeTriang = vetX.length - 2;
            areaPoligono = 0;
            perimetroPoligono = 0;
            
            // Calcula todos os triângulo formados pelo click do mouse
            for (var i = 2; i < vetX.length + 1; i++) {
                var p1 = new point(vetX[i - 2], vetY[i - 2]);
                var p2 = new point(vetX[i - 1], vetY[i - 1]);

                // Obtém os lados do triângulo em metros
                var l1 = distancia(p1, pontoCentral);
                var l2 = distancia(p2, pontoCentral);
                var l3 = distancia(p1, p2);
                //alert(l1 + "/" + l2 + "/" + l3);
                // Obtém o semi-perimetro deste triângulo em metros
                var semiPerim = (l1 + l2 + l3) / 2;
                
                // Aplicação do teorema de Heron para calcular área deste triângulo em m2
                var areaTriangulo = Math.sqrt((semiPerim * (semiPerim - l1) * (semiPerim - l2) * (semiPerim - l3)));
                areaPoligono += areaTriangulo;
                perimetroPoligono += l3;
            }

            if (vetX.length > 3) {
                // Calcula a área do triângulo formado automaticamente (ligação do primeiro ao último ponto)
                var p1 = new point(vetX[0], vetY[0]);
                var p2 = new point(vetX[vetX.length - 1], vetY[vetY.length - 1]);
                var l1 = distancia(p1, pontoCentral);
                var l2 = distancia(p2, pontoCentral);
                var l3 = distancia(p1, p2);
                var semiPerim = (l1 + l2 + l3) / 2;
                var areaTriangulo = Math.sqrt((semiPerim * (semiPerim - l1) * (semiPerim - l2) * (semiPerim - l3)));
                areaPoligono += areaTriangulo;
                perimetroPoligono += l3;
            }

            // Mostra a área calculada
            if (areaPoligono != NaN)
                areaPoligono = areaPoligono.toFixed(3).toString().replace('.', ',');
            FindControl("lblArea", "span").innerHTML = areaPoligono + " m²";

            // Mostra o metro linear calculado
            if (perimetroPoligono != NaN)
                perimetroPoligono = perimetroPoligono.toFixed(3).toString().replace('.', ',');                
            FindControl("lblMetroLinear", "span").innerHTML = perimetroPoligono + " m";
        }
        
        // Template para trabalhar com pontos
        function point(x, y) {
            this.x = x;
            this.y = y;
        }

        // Retorna a coordenada capturada do mouse
        function getMouseCoord(e) {
            if (e.layerX)
                return new point(e.layerX, e.layerY);
            else if (e.offsetX)
                return new point(e.offsetX, e.offsetY);
            else if (e.x)
                return new point(e.x, e.y);
        }

        function onMouseClick(e) {
            // Pega o ponto onde a div foi clicada
            var p = getMouseCoord(e);

            if (p == undefined || p.x == 0 || p.y == 0)
                return;             
                
            // Se estiver pegando escala
            if (escala) {
                marcarEscala(p);
                return;
            }
            // Se não tiver pego escala, manda pegar
            else if (p1Escala == null) {
                alert("Marque a escala da foto.");
                return;
            }

            // Armazena o ponto clicado
            vetX.push(p.x);
            vetY.push(p.y);

            // Desenha área capturada e escala na div
            desenha();
        }

        // Desenha área capturada e escala na div
        function desenha() {
            // Limpa todos os pontos
            jg.clear();
        
            // Se houver mais de um ponto no vetor desenha reta do último ponto até esse que foi clicado
            if (vetX.length > 1) {
                jg.setColor(corArea);
                jg.drawPolyline(vetX, vetY);
            }
            
            // Se houver mais de dois pontos no vetor, liga o primeiro ponto ao último
            if (vetX.length > 2) {
                jg.drawLine(parseInt(vetX[0]), parseInt(vetY[0]), parseInt(vetX[vetX.length-1]), parseInt(vetY[vetY.length-1]));
                getPontoCentral();
                calcArea();
            }

            // Desenha largura e altura de imagens com 4 pontos (retangulo),
            // se a área capturada possuir 5 ou mais arestas, mostra todos os pontos
            if (vetX.length == 4) {
                jg.setFont("arial", "12px", Font.ITALIC_BOLD);
                jg.drawString((distancia(new point(vetX[0], vetY[0]), new point(vetX[1], vetY[1])) * 1000).toFixed(0), (vetX[0] + vetX[1]) / 2, (vetY[0] + vetY[1]) / 2);
                jg.drawString((distancia(new point(vetX[1], vetY[1]), new point(vetX[2], vetY[2])) * 1000).toFixed(0), (vetX[1] + vetX[2]) / 2, (vetY[1] + vetY[2]) / 2);
            }
            else if (vetX.length >= 5) {
                jg.setFont("arial", "12px", Font.ITALIC_BOLD);

                for (var i = 0; i < vetX.length-1; i++)
                    jg.drawString((distancia(new point(vetX[i], vetY[i]), new point(vetX[i + 1], vetY[i + 1])) * 1000).toFixed(0), (vetX[i] + vetX[i + 1]) / 2, (vetY[i] + vetY[i + 1]) / 2);

                // Informa a distância do primeiro pro último ponto
                jg.drawString((distancia(new point(vetX[0], vetY[0]), new point(vetX[i], vetY[i])) * 1000).toFixed(0), (vetX[0] + vetX[i]) / 2, (vetY[0] + vetY[i]) / 2);
            }

            // Se houver escala, desenha
            if (p2Escala != null) {
                jg.setColor(corEscala); // Azul
                jg.drawLine(parseInt(p1Escala.x), parseInt(p1Escala.y), parseInt(p2Escala.x), parseInt(p2Escala.y));
            }

            jg.paint();
        }

        // Desfaz a última linha capturada (referente à área)
        function desfazer() {
            if (vetX.length > 0) {
                // Exclui o último ponto
                vetX.pop();
                vetY.pop();

                // Desenha área capturada e escala na div
                desenha();
            }
        }

        // Muda para o modo de marcar escala
        function chageToEscala() {
            if (!escala) {
                p1Escala = null;
                p2Escala = null;
            }
            
            escala = !escala;
            FindControl("btnMarcarEscala", "input").disabled = escala;
        }

        // Marca escala na tela
        function marcarEscala(p) {
            jg.setColor(corEscala);

            if (p1Escala == null) 
                p1Escala = p;
            else if (p2Escala == null) {
                p2Escala = p;
                desenha();

                // Desmarca a captura de escala
                escala = false;
                FindControl("btnMarcarEscala", "input").disabled = false;
            }
        }

        // Salva os pontos, a área e o metro linear da figura
        function salvarPontos() {
            if (!confirm("Tem certeza que deseja salvar a área obtida?"))
                return false;

            if (vetX.length <= 0) {
                alert("Marque a área a ser calculada.");
                return false;
            }

            var idFoto = FindControl("hdfIdFoto", "input").value;
            var escala = FindControl("txtEscala", "input").value;
            var escalaP1 = p1Escala.x + ";" + p1Escala.y;
            var escalaP2 = p2Escala.x + ";" + p2Escala.y;
            var pontos = "";

            for (var i = 0; i < vetX.length; i++)
                pontos += vetX[i] + ";" + vetY[i] + "|";

            var response = MedFoto.SalvarPontos(idFoto, areaPoligono, perimetroPoligono, escala, escalaP1, escalaP2, pontos).value;

            if (response == null) {
                alert("Falha ao salvar área. Ajax Error.");
                return false;
            }

            response = response.split('\t');

            alert(response[1]);

            return false;
        }

        function limpar() {
            jg.clear();
            areaPoligono = 0;
            perimetroPoligono = 0;
            vetX = new Array();
            vetY = new Array();
            p1Escala = null;
            p2Escala = null;

            FindControl("lblArea", "span").innerHTML = "0,00 m²";
            FindControl("lblMetroLinear", "span").innerHTML = "0,00 m";
            FindControl("txtEscala", "input").value = "100";
        }
             
    </script>  
</head>
<body>
    <form id="form1" runat="server">
        <table style="width: 100%">
            <tr>
                <td align="center">
                    <div id="divCanvas" style="position:relative;height:567px;width:756px; border: 1px solid gray;" 
                        onclick="onMouseClick(event);">
                        <asp:Image ID="imgFoto" runat="server" />
                    </div></td>
            </tr>
            <tr>
                <td align="center">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Escala:"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtEscala" runat="server" onkeypress="return soNumeros(event, true, true);" MaxLength="5" Width="50px">100</asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="Label3" runat="server" Text="cm"></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="btnMarcarEscala" runat="server" Text="Marcar Escala" 
                                    OnClientClick="chageToEscala(); return false;" Width="107px" />
                            </td>
                            <td>
                                <asp:Label ID="Label1" runat="server" Font-Bold="True" Text="Área total:"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblArea" runat="server">0,00 m²</asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="Label4" runat="server" Font-Bold="True" Text="Metro Linear:"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblMetroLinear" runat="server">0,00 m</asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td>
                                <asp:LinkButton ID="lnkLimpar" runat="server" OnClientClick="limpar(); return false;">
                                    <img src="../Images/ExcluirGrid.gif" border="0"> Limpar</asp:LinkButton>&nbsp;&nbsp;
                            </td>
                            <td>
                                <asp:LinkButton ID="lnkDesfazer" runat="server" OnClientClick="desfazer(); return false;">
                                    <img src="../Images/arrow_undo.gif" border="0"> Desfazer</asp:LinkButton>&nbsp;&nbsp;
                            </td>
                            <td>
                                <asp:LinkButton ID="lnkVoltar" runat="server" OnClientClick="window.history.go(-1); return false">
                                    <img src="../Images/arrow_left.gif" border="0"> Voltar</asp:LinkButton>&nbsp;&nbsp;
                            </td>
                            <td>
                                <asp:LinkButton ID="lnkSalvar" runat="server" OnClientClick="salvarPontos(); return false;">
                                    <img src="../Images/disk.gif" border="0"> Salvar</asp:LinkButton>
                            </td>
                        </tr>
                    </table>
                    <asp:HiddenField ID="hdfIdFoto" runat="server" />
                    <asp:HiddenField ID="hdfPontosEscala" runat="server" />
                    <asp:HiddenField ID="hdfPontosFigura" runat="server" />
                </td>      
            </tr>
        </table>
    </form>
</body>
    <script type="text/javascript">
        // Inicializa a biblioteca de gráficos
        jg = new jsGraphics(find("divCanvas"));
        jg.setStroke(1); // Grossura da linha
        jg.setColor("#ff0000"); // Cor da linha

        // Ajusta tamanho e posição da tela
        var altura = 740;
        var largura = 800;
        var top = (screen.height - altura) / 2;
        var left = (screen.width - largura) / 2;
        window.outerHeight = altura;
        window.outerWidth = largura;
        window.moveTo(left, top);

        // Carrega pontos e escala da figura, se houver
        var pEscala = FindControl('hdfPontosEscala', 'input').value;
        var pFigura = FindControl('hdfPontosFigura', 'input').value;

        // Carrega escala
        if (pEscala != "") {
            var pontoEscala = pEscala.split(';');
            p1Escala = new point(pontoEscala[0], pontoEscala[1]);
            p2Escala = new point(pontoEscala[2], pontoEscala[3]);
        }

        // Carrega pontos da figura
        if (pFigura != "") {
            var pontos = pFigura.split('|');

            for (var i = 0; i < pontos.length; i++) {
                var x = pontos[i].split(';')[0];
                var y = pontos[i].split(';')[1];

                if (x == "" || y == "" || x == undefined || y == undefined)
                    continue;
                
                vetX.push(parseInt(x));
                vetY.push(parseInt(y));
            }

            desenha();
        }
    </script>
</html>
