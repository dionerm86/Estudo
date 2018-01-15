<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CadPosicaoPecaModelo.aspx.cs"
    Inherits="Glass.UI.Web.Cadastros.Projeto.CadPosicaoPecaModelo" Title="Configuração das Informações da Figura do Projeto"
    MasterPageFile="~/Layout.master" %>

<%@ Register Src="../../Controls/ctrlLogPopup.ascx" TagName="ctrlLogPopup" TagPrefix="uc1" %>
<asp:Content ID="menu" runat="server" ContentPlaceHolderID="Menu">
</asp:Content>
<asp:Content ID="pagina" runat="server" ContentPlaceHolderID="Pagina">
    <script type="text/javascript" src="../../Scripts/wz_jsgraphics.js"></script>
    <script type="text/javascript">
        var jg = null; // Variável de gráficos
        var numInfoConfig = 60;

        // Template para trabalhar com pontos
        function point(x, y)
        {
            this.x = x;
            this.y = y;
        }

        var txtCalcSel = null;

        function setExpressao(expressao)
        {
            txtCalcSel.value = expressao;
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

            // Pega o número da peça que deve ser salva a coordenada
            var numInfo = 0;
            for (numInfo = 1; numInfo <= numInfoConfig; numInfo++)
                if (FindControl("radInfo_" + (numInfo < 10 ? "0" : "") + numInfo, "input").checked)
                break;

            FindControl("txtCoordX" + numInfo, "input").value = p.x;
            FindControl("txtCoordY" + numInfo, "input").value = p.y;

            // Redesenha imagens nas suas posições
            redesenhaPosicoes();
        }

        // Função responsável por redesenhar posições na imagem
        function redesenhaPosicoes()
        {
            // Limpa imagens
            jg.clear();

            var numInfo = FindControl("txtQtdInfo", "input").value;

            var medHorPath = '<%= Glass.Data.Helper.Utils.GetFullUrl(HttpContext.Current, "~/Images/info_horizontal_01.png") %>'
            var medVertPath = '<%= Glass.Data.Helper.Utils.GetFullUrl(HttpContext.Current, "~/Images/info_vertical_01.png") %>';

            for (var i = 1; i <= numInfo; i++)
            {
                var coordX = FindControl("txtCoordX" + i, "input").value;
                var coordY = FindControl("txtCoordY" + i, "input").value;

                if (coordX == 0 && coordY == 0)
                    continue;

                var orientacao = FindControl("drpOrientacao" + i, "select").value; // 1-Horizontal 2-Vertical
                var imageHeight = orientacao == 1 ? 15 : 54;
                var imageWidth = orientacao == 1 ? 54 : 15;

                // Centraliza posição das imagens
                coordX -= imageWidth / 2;
                coordY -= imageHeight / 2;

                jg.drawImage(orientacao == 1 ? medHorPath.replace("_01", i < 10 ? "_0" + i : "_" + i) : medVertPath.replace("_01", i < 10 ? "_0" + i : "_" + i),
                coordX, coordY, imageWidth, imageHeight);
            }

            // Desenha posições
            jg.paint();
        }

        // Salva os pontos
        function salvar()
        {
            var numInfo = FindControl("txtQtdInfo", "input").value;
            var vetCoord = "";
            var vetOrientacao = "";
            var vetCalc = "";

            var idProjetoModelo = FindControl("hdfIdProjetoModelo", "input").value;
            var idPecaProjMod = FindControl("hdfIdPecaProjMod", "input").value;
            var item = FindControl("hdfItem", "input").value;
            var tipo = FindControl("hdfTipo", "input").value;

            var i = 0;
            for (i = 1; i <= numInfo; i++)
            {
                coordX = FindControl("txtCoordX" + i, "input").value;
                if (Trim(coordX) == "")
                {
                    alert("Informe a primeira posição da coordenada do Info. " + i);
                    return false;
                }

                coordY = FindControl("txtCoordY" + i, "input").value;
                if (Trim(coordY) == "")
                {
                    alert("Informe a segunda posição da coordenada do Info. " + i);
                    return false;
                }

                calc = FindControl("txtCalc" + i, "input").value;
                if (Trim(calc) == "")
                {
                    alert("Informe a expressão de cálculo do Info. " + i);
                    return false;
                }

                vetCoord += "|" + (coordX != "" ? coordX : 0) + ";" + (coordY != "" ? coordY : 0);
                vetOrientacao += "|" + FindControl("drpOrientacao" + i, "select").value;
                vetCalc += "|" + calc;
            }

            var response = null;

            if (tipo != "pecaIndividual")
                response = CadPosicaoPecaModelo.SalvarModeloCompleto(idProjetoModelo, numInfo, vetCoord, vetOrientacao, vetCalc).value;
            else
                response = CadPosicaoPecaModelo.SalvarModeloIndividual(idPecaProjMod, item, numInfo, vetCoord, vetOrientacao, vetCalc).value;

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

            closeWindow();

            return false;
        }

        function showHideInfo()
        {
            var numInfos = FindControl("txtQtdInfo", "input").value;

            if (numInfos > numInfoConfig)
            {
                FindControl("txtQtdInfo", "input").value = numInfoConfig;
                numInfos = numInfoConfig;
            }

            numInfos = numInfos == "" ? 0 : parseInt(numInfos);

            for (var i = 0; i < numInfoConfig; i++)
                document.getElementById("trInfo" + (i + 1)).style.display = numInfos > i ? "inline" : "none";

            if (numInfos > 0)
                FindControl("radInfo_" + (numInfos < 10 ? "0" : "") + numInfos, "input").checked = true;

            redesenhaPosicoes();
        }

        // Troca os sinais de + das expressões de cálculo para que ao editar a mesma o + não suma
        function trocaSinalMais(descricao)
        {
            while (descricao.indexOf("+") > 0)
                descricao = descricao.replace("+", "@");

            return descricao;
        }

        function limpar()
        {
            jg.clear();
        }

        function openExpressao(nomeControle)
        {
            txtCalcSel = FindControl(nomeControle, "input");

            var idProjetoModelo = GetQueryString("idProjetoModelo");
            var idPecaProjMod = GetQueryString("idPecaProjMod");
            var item = GetQueryString("item");
            var expressao = trocaSinalMais(txtCalcSel.value);
            var numInfo = FindControl("txtQtdInfo", "input").value;

            var url = "../../Utils/SelExpressao.aspx?tipo=posicao";

            url += idProjetoModelo != null ? "&idProjetoModelo=" + idProjetoModelo : "";
            url += idPecaProjMod != null ? "&idPecaProjMod=" + idPecaProjMod : "";
            url += item != null ? "&item=" + item : "";
            url += expressao != null ? "&expr=" + expressao : "";
            url += "&numInfo=" + numInfo;

            openWindow(500, 700, url);

            return false;
        }
         
    </script>

    <style type="text/css">
        .style1
        {
            width: 100%;
        }
    </style>
    <table>
        <tr>
            <td align="center">
                <table id="tbCarregaImagem" runat="server">
                    <tr>
                        <td>
                            <asp:Label ID="Label14" runat="server" ForeColor="#0066FF" Text="Imagem"></asp:Label>
                        </td>
                        <td>
                            <asp:FileUpload ID="fluPecaIndividual" runat="server" />
                        </td>
                        <td>
                            <asp:Button ID="btnInserirImagem" runat="server" Text="Inserir" OnClick="btnInserirImagem_Click" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <div id="divCanvas" onclick="onMouseClick(event);" style="position: relative; border: 1px solid gray;"
                                align="center">
                                <asp:Image ID="imgFigura" runat="server" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" ForeColor="#0066FF" Text="Qtd. Informações"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtQtdInfo" runat="server" Width="50px" onkeypress="return soNumeros(event, true, true)"
                                onkeydown="if (isEnter(event)) showHideInfo();" onblur="showHideInfo();">1</asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table class="style1" id="info" runat="server">
                    <tr>
                        <td width="50%">
                            &nbsp;
                        </td>
                        <td>
                            <table>
                                <tr id="trInfo1">
                                    <td>
                                        <asp:RadioButton ID="radInfo_01" runat="server" Checked="True" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight:\ bold" nowrap="nowrap">
                                        Info. 1
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX1" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY1" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao1" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc1" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc1');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup1" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo2">
                                    <td>
                                        <asp:RadioButton ID="radInfo_02" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 2
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX2" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY2" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao2" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc2" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc2');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup2" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo3">
                                    <td>
                                        <asp:RadioButton ID="radInfo_03" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 3
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX3" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY3" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao3" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc3" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc3');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup3" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo4">
                                    <td>
                                        <asp:RadioButton ID="radInfo_04" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 4
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX4" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY4" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao4" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc4" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc4');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup4" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo5">
                                    <td>
                                        <asp:RadioButton ID="radInfo_05" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 5
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX5" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY5" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao5" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc5" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc5');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup5" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo6">
                                    <td>
                                        <asp:RadioButton ID="radInfo_06" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 6
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX6" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY6" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao6" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc6" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc6');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup6" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo7">
                                    <td>
                                        <asp:RadioButton ID="radInfo_07" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 7
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX7" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY7" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao7" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc7" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc7');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup7" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo8">
                                    <td>
                                        <asp:RadioButton ID="radInfo_08" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 8
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX8" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY8" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao8" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc8" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc8');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup8" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo9">
                                    <td>
                                        <asp:RadioButton ID="radInfo_09" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 9
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX9" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY9" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao9" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc9" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc9');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup9" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo10">
                                    <td>
                                        <asp:RadioButton ID="radInfo_10" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 10
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX10" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY10" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao10" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc10" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc10');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup10" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo11">
                                    <td>
                                        <asp:RadioButton ID="radInfo_11" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 11
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX11" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY11" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao11" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc11" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc11');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup11" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo12">
                                    <td>
                                        <asp:RadioButton ID="radInfo_12" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 12
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX12" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY12" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao12" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc12" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc12');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup12" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo13">
                                    <td>
                                        <asp:RadioButton ID="radInfo_13" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 13
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX13" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY13" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao13" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc13" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc13');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup13" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo14">
                                    <td>
                                        <asp:RadioButton ID="radInfo_14" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 14
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX14" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY14" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao14" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc14" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc14');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup14" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo15">
                                    <td>
                                        <asp:RadioButton ID="radInfo_15" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 15
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX15" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY15" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao15" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc15" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc15');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup15" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo16">
                                    <td>
                                        <asp:RadioButton ID="radInfo_16" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 16
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX16" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY16" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao16" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc16" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc16');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup16" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo17">
                                    <td>
                                        <asp:RadioButton ID="radInfo_17" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 17
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX17" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY17" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao17" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc17" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc17');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup17" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo18">
                                    <td>
                                        <asp:RadioButton ID="radInfo_18" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 18
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX18" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY18" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao18" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc18" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc18');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup18" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo19">
                                    <td>
                                        <asp:RadioButton ID="radInfo_19" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 19
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX19" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY19" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao19" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc19" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc19');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup19" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo20">
                                    <td>
                                        <asp:RadioButton ID="radInfo_20" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 20
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX20" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY20" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao20" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc20" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc20');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup20" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo21">
                                    <td>
                                        <asp:RadioButton ID="radInfo_21" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 21
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX21" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY21" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao21" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc21" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc21');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup21" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo22">
                                    <td>
                                        <asp:RadioButton ID="radInfo_22" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 22
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX22" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY22" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao22" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc22" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc22');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup22" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo23">
                                    <td>
                                        <asp:RadioButton ID="radInfo_23" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 23
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX23" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY23" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao23" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc23" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc23');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup23" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo24">
                                    <td>
                                        <asp:RadioButton ID="radInfo_24" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 24
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX24" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY24" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao24" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc24" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc24');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup24" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo25">
                                    <td>
                                        <asp:RadioButton ID="radInfo_25" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 25
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX25" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY25" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao25" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc25" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc25');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup25" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo26">
                                    <td>
                                        <asp:RadioButton ID="radInfo_26" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 26
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX26" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY26" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao26" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc26" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc26');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup26" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo27">
                                    <td>
                                        <asp:RadioButton ID="radInfo_27" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 27
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX27" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY27" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao27" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc27" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc27');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup27" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo28">
                                    <td>
                                        <asp:RadioButton ID="radInfo_28" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 28
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX28" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY28" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao28" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc28" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc28');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup28" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo29">
                                    <td>
                                        <asp:RadioButton ID="radInfo_29" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 29
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX29" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY29" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao29" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc29" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc29');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup29" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo30">
                                    <td>
                                        <asp:RadioButton ID="radInfo_30" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 30
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX30" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY30" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao30" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc30" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc30');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup30" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo31">
                                    <td>
                                        <asp:RadioButton ID="radInfo_31" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 31
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX31" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY31" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao31" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc31" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc31');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup31" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo32">
                                    <td>
                                        <asp:RadioButton ID="radInfo_32" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 32
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX32" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY32" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao32" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc32" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc32');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup32" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo33">
                                    <td>
                                        <asp:RadioButton ID="radInfo_33" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 33
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX33" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY33" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao33" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc33" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc33');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup33" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo34">
                                    <td>
                                        <asp:RadioButton ID="radInfo_34" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 34
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX34" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY34" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao34" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc34" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc34');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup34" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo35">
                                    <td>
                                        <asp:RadioButton ID="radInfo_35" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 35
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX35" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY35" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao35" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc35" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc35');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup35" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo36">
                                    <td>
                                        <asp:RadioButton ID="radInfo_36" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 36
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX36" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY36" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao36" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc36" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc36');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup36" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo37">
                                    <td>
                                        <asp:RadioButton ID="radInfo_37" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 37
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX37" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY37" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao37" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc37" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc37');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup37" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo38">
                                    <td>
                                        <asp:RadioButton ID="radInfo_38" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 38
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX38" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY38" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao38" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc38" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc38');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup38" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo39">
                                    <td>
                                        <asp:RadioButton ID="radInfo_39" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 39
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX39" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY39" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao39" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc39" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc39');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup39" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo40">
                                    <td>
                                        <asp:RadioButton ID="radInfo_40" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 40
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX40" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY40" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao40" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc40" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc40');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup40" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo41">
                                    <td>
                                        <asp:RadioButton ID="radInfo_41" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 41
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX41" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY41" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao41" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc41" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc41');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup41" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo42">
                                    <td>
                                        <asp:RadioButton ID="radInfo_42" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 42
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX42" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY42" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao42" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc42" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc42');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup42" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo43">
                                    <td>
                                        <asp:RadioButton ID="radInfo_43" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 43
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX43" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY43" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao43" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc43" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc43');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup43" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo44">
                                    <td>
                                        <asp:RadioButton ID="radInfo_44" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 44
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX44" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY44" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao44" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc44" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc44');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup44" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo45">
                                    <td>
                                        <asp:RadioButton ID="radInfo_45" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 45
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX45" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY45" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao45" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc45" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc45');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup45" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo46">
                                    <td>
                                        <asp:RadioButton ID="radInfo_46" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 46
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX46" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY46" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao46" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc46" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc46');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup46" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo47">
                                    <td>
                                        <asp:RadioButton ID="radInfo_47" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 47
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX47" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY47" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao47" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc47" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc47');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup47" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo48">
                                    <td>
                                        <asp:RadioButton ID="radInfo_48" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 48
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX48" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY48" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao48" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc48" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc48');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup48" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo49">
                                    <td>
                                        <asp:RadioButton ID="radInfo_49" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 49
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX49" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY49" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao49" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc49" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc49');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup49" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo50">
                                    <td>
                                        <asp:RadioButton ID="radInfo_50" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 50
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX50" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY50" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao50" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc50" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc50');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup50" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo51">
                                    <td>
                                        <asp:RadioButton ID="radInfo_51" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 51
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX51" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY51" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao51" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc51" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc51');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup51" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo52">
                                    <td>
                                        <asp:RadioButton ID="radInfo_52" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 52
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX52" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY52" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao52" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc52" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc52');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup52" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo53">
                                    <td>
                                        <asp:RadioButton ID="radInfo_53" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 53
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX53" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY53" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao53" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc53" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc53');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup53" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo54">
                                    <td>
                                        <asp:RadioButton ID="radInfo_54" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 54
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX54" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY54" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao54" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc54" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc54');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup54" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo55">
                                    <td>
                                        <asp:RadioButton ID="radInfo_55" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 55
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX55" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY55" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao55" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc55" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc55');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup55" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo56">
                                    <td>
                                        <asp:RadioButton ID="radInfo_56" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 56
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX56" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY56" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao56" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc56" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc56');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup56" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo57">
                                    <td>
                                        <asp:RadioButton ID="radInfo_57" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 57
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX57" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY57" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao57" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc57" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc57');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup57" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo58">
                                    <td>
                                        <asp:RadioButton ID="radInfo_58" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 58
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX58" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY58" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao58" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc58" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc58');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup58" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo59">
                                    <td>
                                        <asp:RadioButton ID="radInfo_59" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 59
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX59" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY59" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao59" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc59" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc59');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup59" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trInfo60">
                                    <td>
                                        <asp:RadioButton ID="radInfo_60" runat="server" GroupName="SelInfo" />
                                    </td>
                                    <td style="font-weight: bold" nowrap="nowrap">
                                        Info. 60
                                    </td>
                                    <td>
                                        Posição
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordX60" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCoordY60" runat="server" Width="50px"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrientacao60" runat="server">
                                            <asp:ListItem Value="1">Horizontal</asp:ListItem>
                                            <asp:ListItem Value="2" Selected="True">Vertical</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td nowrap="nowrap">
                                        <asp:TextBox onpaste="return false;" onkeydown="return false;" onkeyup="return false;" ID="txtCalc60" runat="server" Width="150px" onkeypress="return false;"></asp:TextBox>
                                        <a href="#" onclick="return openExpressao('txtCalc60');">
                                            <img src="../../Images/Pesquisar.gif" border="0" /></a>
                                    </td>
                                    <td>
                                        <uc1:ctrlLogPopup ID="ctrlLogPopup60" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td width="50%">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClientClick="return salvar();" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnSalvar0" runat="server" Text="Fechar" OnClientClick="closeWindow();" />
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdfIdProjetoModelo" runat="server" />
    <asp:HiddenField ID="hdfIdPecaProjMod" runat="server" />
    <asp:HiddenField ID="hdfItem" runat="server" />
    <asp:HiddenField ID="hdfTipo" runat="server" />

    <script type="text/javascript">
        // Inicializa a biblioteca de gráficos
        jg = new jsGraphics(find("divCanvas"));
        showHideInfo();

        // Carrega posições já inseridas nesta imagem de projeto
        redesenhaPosicoes();
    </script>

</asp:Content>
