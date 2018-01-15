<%@ Page Language="C#" MasterPageFile="~/Painel.master" AutoEventWireup="true" CodeBehind="Rastrear.aspx.cs" Inherits="Glass.UI.Web.Utils.Rastrear" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Conteudo" Runat="Server">

    <script type="text/javascript" src="http://www.google.com/jsapi?key=<%= keyGoogleMaps %>"></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/Scripts/Rastrear.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) %>'></script>

<script type="text/javascript">
    function SelecionaData(nomeCampo, botao)
    {
        try
        {
            var campo = FindControl(nomeCampo, "input");
            if (campo != null)
                displayCalendar(campo, 'dd/mm/yyyy hh:ii', botao, true);
        }
        catch(err)
        {
            return false;
        }
        
        return false;
    }
</script>
<table>
    <tr>
        <td style="height: 480px;">
            <div id="divTituloEquipe" style="padding-bottom: 4px; padding-top: 4px; font-weight: bold; color: #6699CC; text-align: center; background-color: #E3EAEB; font-size: 10pt;">
                Equipes</div>
            <div id="caminho" style="width:200px; height:50%; border: solid 1px gray; overflow: auto;">
                <table cellpadding="1" cellspacing="0" style="width:100%;">
                    <tbody id="MapMarks" style="width:100%;"></tbody>
                </table>
            </div><br />
            <div id="divTituloInst" style="padding-bottom: 4px; padding-top: 4px; font-weight: bold; color: #6699CC; text-align: center; background-color: #E3EAEB; font-size: 10pt">
                Instalações</div>
            <div id="inst" style="height: 37%; border: solid 1px gray; overflow: auto">
                <div id="divInstalacoes" style="width: 100%; text-align: left; overflow: auto">
                  <table cellpadding="0" cellspacing="0" style="width:100%">
                    <tbody id="Instalacoes" style="width:100%;"></tbody>
                  </table>
                </div>
            </div>
            </td>
        <td>
            <div id="mapa" style="width:640px; height:480px; border: solid 1px gray" onunload="GUnload()" />
        </td>
    </tr>
</table>
</asp:Content>

