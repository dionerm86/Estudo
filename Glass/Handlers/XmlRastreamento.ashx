<%@ WebHandler Language="C#" Class="XmlRastreamento" %>

using System;
using System.Web;
using System.Xml;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System.Collections.Generic;

public class XmlRastreamento : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) 
    {
        context.Response.ContentType = "text/xml";
        
        XmlDocument xml = new XmlDocument();
        XmlElement markers = xml.CreateElement("markers");

        Equipe[] lstEquipe = EquipeDAO.Instance.GetOrdered();
        PontosRota ultPonto;
        string dtUltPonto = null;

        for (int i = 0; i < lstEquipe.Length; i++)
        {
            XmlElement marker = xml.CreateElement("marker");

            ultPonto = PontosRotaDAO.Instance.GetLastByEquipe(lstEquipe[i].IdEquipe);

            if (ultPonto != null)
            {
                dtUltPonto = ultPonto.DataPonto.ToString("dd/MM/yy HH:mm");

                string strTexto = "Data/Hora: " + dtUltPonto + "<br />" +
                    "Latitude: " + Coordenada(ultPonto.Lat, 'L', 'O') + "<br />" +
                    "Longitude: " + Coordenada(ultPonto.Long, 'N', 'S');

                marker.SetAttribute("lat", ultPonto.Lat.ToString().Replace(',', '.'));
                marker.SetAttribute("lng", ultPonto.Long.ToString().Replace(',', '.'));
                marker.SetAttribute("texto", strTexto);
                marker.SetAttribute("dtUltPonto", dtUltPonto);
                
                // Pega os últimos 20 pontos da equipe
                var lstPontos = PontosRotaDAO.Instance.GetLastPoints(lstEquipe[i].IdEquipe, 40);
                string pontosEquipe = String.Empty;
                foreach (PontosRota p in lstPontos)
                    pontosEquipe += p.Lat + ";" + p.Long + "|";

                marker.SetAttribute("pontosEquipe", pontosEquipe.Replace(',', '.').TrimEnd('|'));
            }

            marker.SetAttribute("idEquipe", lstEquipe[i].IdEquipe.ToString());
            marker.SetAttribute("titulo", lstEquipe[i].Nome);
            marker.SetAttribute("tipo", "E");
            
            // Se o último ponto tiver sido atualizado em menos de 1 hora, o nome do fiscal na lista 
            // fica com fundo verde, caso contrário fica vermelho
            marker.SetAttribute("status", !String.IsNullOrEmpty(dtUltPonto) && Glass.FuncoesData.DateDiff(Glass.FuncoesData.DateInterval.Hour, ultPonto.DataPonto, DateTime.Now) < 1 ? "<img src=\"../Images/online.gif\" border=\"0px\"/>" : "<img src=\"../Images/offline.gif\" border=\"0px\"/>");

            markers.AppendChild(marker);

            dtUltPonto = null;
        }

        // Busca as coordenadas e o tempo de instalação das instalações que as equipes realizaram hoje e que estão pendentes
        Instalacao[] lstInst = InstalacaoDAO.Instance.GetAllForRota(0, DateTime.Now.ToString("dd/MM/yyyy 00:00"), DateTime.Now.ToString("dd/MM/yyyy 23:59"), true);
        for (int i = 0; i < lstInst.Length; i++)
        {
            if (lstInst[i].DataConfirmada != null && (lstInst[i].Latitude == null || lstInst[i].Longitude == null))
                continue;

            XmlElement marker = xml.CreateElement("marker");

            // Texto que será mostrado no balão quando o usuário clicar no ícone deste ponto
            string strTexto =
                "Equipe: " + lstInst[i].NomesEquipes + "<br />" +
                "Cliente: " + lstInst[i].NomeCliente + "<br />" +
                "Local: " + lstInst[i].LocalObra + "<br />";

            if (lstInst[i].DataConfirmada != null)
            {
                strTexto += "Data Confirmada: " + lstInst[i].DataConfirmada + "<br />" +
                    "Tempo de instalação: " + lstInst[i].TempoInst + "<br />";
            }
            
            strTexto += "<a href=\"#\" onclick=\"openWindow(600, 800, '../Relatorios/RelPedido.aspx?idPedido=" + lstInst[i].IdPedido + "');\">Visualizar Pedido</a><br />";

            if (lstInst[i].DataConfirmada != null)
            {
                marker.SetAttribute("lat", lstInst[i].Latitude.ToString().Replace(',', '.'));
                marker.SetAttribute("lng", lstInst[i].Longitude.ToString().Replace(',', '.'));
            }
            
            marker.SetAttribute("titulo", "Instalação");
            marker.SetAttribute("texto", strTexto);
            //marker.SetAttribute("idEquipe", lstInst[i].IdEquipe.ToString());
            marker.SetAttribute("idPedido", lstInst[i].IdPedido.ToString());
            marker.SetAttribute("tempo", lstInst[i].TempoInst);
            marker.SetAttribute("tipo", "I");
            marker.SetAttribute("situacao", lstInst[i].DataConfirmada == null ? "P" : "C");

            markers.AppendChild(marker);
        }

        xml.AppendChild(markers);
        xml.Save(context.Response.OutputStream);
    }

    private string Coordenada(decimal? numero, char positivo, char negativo)
    {
        if (numero == null)
            return String.Empty;
        
        decimal graus, minutos, segundos;
        char chrPosicao = (numero > 0) ? positivo : negativo;

        graus = Math.Abs(Math.Truncate(numero.Value));
        minutos = Math.Abs(numero.Value) - graus;
        segundos = minutos * 3600;
        minutos = 0;
        
        while (segundos > 60)
        {
            minutos++;
            segundos -= 60;
        }

        return graus.ToString("0") + "°" + minutos.ToString("0") + "\"" + segundos.ToString("0.####") + "' " + chrPosicao;
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}