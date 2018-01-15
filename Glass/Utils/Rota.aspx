<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Rota.aspx.cs" Inherits="Glass.Utils.Rota" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:v="urn:schemas-microsoft-com:vml">
<head runat="server">
    <title>Rastreamento de Equipes de Instalação</title>
    <link rel="stylesheet" href="../Style/StyleProd.css" />
    <link rel="stylesheet" type="text/css" href="../Style/GridView.css" />
    <link href="../Style/dhtmlgoodies_calendar.css" rel="Stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/dhtmlgoodies_calendar.js"></script>
    <script type="text/javascript" src="http://www.google.com/jsapi?key=<%= keyGoogleMaps %>"></script>
    <script type="text/javascript" src="../Scripts/Utils.js"></script>
    <script type="text/javascript" src="../Scripts/Rota.js"></script>
    <script type="text/javascript">
        window.onresize = resizeMap;
    
        function SelecionaData(nomeCampo, botao) {
            try {
                var campo = FindControl(nomeCampo, "input");

                if (campo != null)
                    displayCalendar(campo, 'dd/mm/yyyy hh:ii', botao, true);
            }
            catch (err) {
                alert(err);
                return false;
            }

            return false;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <table style="height: 100%; width: 100%" border="0">
            <tr>
                <td nowrap="nowrap">
                    <a href="#" id="linkTodaRota" onclick="desenhaRotaCompleta()"><img src="../Images/Globo.gif" border="0" alt="Ver toda rota"></a>&nbsp;
                    <a href="#" id="linkIniciarPausar" onclick="iniciarPausar()">Pausar</a>&nbsp;
                    <a href="#" id="linkParar" onclick="parar()">Parar</a>&nbsp; Velocidade:<input type="radio" name="velocidade" id="baixa" value="2000" onclick="alterarVelocidade(this)" style="margin-bottom: -2px" /><label for="baixa">Baixa</label><input type="radio" name="velocidade" id="media" value="1000" onclick="alterarVelocidade(this)" checked="checked" style="margin-bottom: -2px" /><label for="media">Média</label><input type="radio" name="velocidade" id="alta" value="500" onclick="alterarVelocidade(this)" style="margin-bottom: -2px" /><label for="alta">Alta </label>
                    <input type="hidden" id="valor_velocidade" value="1000" style="width: 30px" />
                    
                    &nbsp; Início:&nbsp;<asp:ImageButton ID="imgIni" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif" OnClientClick="return SelecionaData('DtInicio', this);" />
                    <input type="text" id="DtInicio" class="caixatexto" value="<%= Request["dtInicio"] %>" style="width: 105px" />
                    
                    Fim:&nbsp;<asp:ImageButton ID="imgFim" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/Images/calendario.gif" OnClientClick="return SelecionaData('DtFim', this);" />
                    <input type="text" id="DtFim" class="caixatexto" value="<%= Request["dtFim"] %>" style="width: 105px" />
                    
                    
                    <a href="#" onclick="mudaPeriodo()"><img src="../Images/Pesquisar.gif" border="0" /></a>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="mapa" style="width: 670px; height: 415px; border: solid 1px gray;" onunload="GUnload()"></div>
                </td>
            </tr>
            <tr>
                <td>
                <table width="100%" border="0">
                  <tr>
                    <td align="left" width="30%">
                        <span id="lblDistancia"/>
                    </td>
                    <td align="center" width="30%">
                        <input type="hidden" id="tipo" value="rota" />
                        <input type="hidden" id="IdEquipe" value="<%= Request["IdEquipe"] %>" />
                        <input type="button" class="botao" onclick="GUnload();closeWindow();" value="Fechar" />
                    </td>
                    <td width="30%">
                    </td>
                  </tr>
                </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>