<%@ WebHandler Language="C#" Class="XmlMapa" %>

using System;
using System.Web;
using System.Xml;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;

public class XmlMapa : IHttpHandler
{
    public void ProcessRequest (HttpContext context)
    {
        uint idEquipe = Glass.Conversoes.StrParaUint(context.Request["IdEquipe"]);
        string dtInicio = context.Request["dtInicio"];
        string dtFim = context.Request["dtFim"];
        
        context.Response.ContentType = "text/xml";
        
        XmlDocument xml = new XmlDocument();
        XmlElement markers = xml.CreateElement("markers");
        xml.AppendChild(markers);

        // Busca os pontos da rota
        var pontosRota = PontosRotaDAO.Instance.GetByEquipe(idEquipe, dtInicio, dtFim);
        for (int i = 0; i < pontosRota.Count; i++)
        {
            XmlElement marker = xml.CreateElement("marker");

            string vel = pontosRota[i].Velocidade.ToString();
            long minDiff = 0;
            
            // Calcula a diferença em minutos deste ponto para o último ponto 
            if (i>0)
                minDiff = Glass.FuncoesData.DateDiff(Glass.FuncoesData.DateInterval.Minute, 
                    pontosRota[i-1].DataPonto, pontosRota[i].DataPonto);

            if (vel.IndexOf(',') > 0)
                vel = vel.Remove(vel.IndexOf(','));

            // Texto que será mostrado no balão quando o usuário clicar no ícone deste ponto
            string strTexto = "Data: " + pontosRota[i].DataPonto.ToString("dd/MM/yy HH:mm") + "<br />" +
                              "Velocidade: " + vel + " km/h<br />" +
                              "Latitude: " + Coordenada(pontosRota[i].Lat, 'N', 'S') + "<br />" +
                              "Longitude: " + Coordenada(pontosRota[i].Long, 'O', 'L') + "<br />";

            marker.SetAttribute("lat", pontosRota[i].Lat.ToString().Replace(',', '.'));
            marker.SetAttribute("lng", pontosRota[i].Long.ToString().Replace(',', '.'));
            marker.SetAttribute("titulo", "Referência");
            marker.SetAttribute("texto", strTexto);
            marker.SetAttribute("minDiff", minDiff.ToString());

            // A cada 20 pontos, um será de referência
            if (i % 20 != 0 || i < 20)
                marker.SetAttribute("tipo", "N");
            else
                marker.SetAttribute("tipo", "R");

            markers.AppendChild(marker);
        }

        // Busca as coordenadas e o tempo de instalação das instalações que a equipe realizou entre a DtInicio e DtTermino desta rota
        Instalacao[] lstInst = InstalacaoDAO.Instance.GetAllForRota(idEquipe, dtInicio, dtFim, false);
        for (int i = 0; i < lstInst.Length; i++)
        {
            if (lstInst[i].Latitude == null || lstInst[i].Longitude == null)
                continue;

            XmlElement marker = xml.CreateElement("marker");

            // Texto que será mostrado no balão quando o usuário clicar no ícone deste ponto
            string strTexto = "Cliente: " + lstInst[i].NomeCliente + "<br />" +
                "Local: " + lstInst[i].LocalObra + "<br />" +
                "Data Confirmada: " + lstInst[i].DataConfirmada + "<br />" +
                "Tempo de instalação: " + lstInst[i].TempoInst + "<br />" +
                "<a href=\"#\" onclick=\"openWindow(600, 800, '../Relatorios/RelPedido.aspx?idPedido=" + lstInst[0].IdPedido + "');\">Visualizar Pedido</a><br />";
                //"Latitude: " + Coordenada(lstInst[i].Latitude.Value, 'N', 'S') + "<br />" +
                //"Longitude: " + Coordenada(lstInst[i].Longitude.Value, 'O', 'L') + "<br />";

            marker.SetAttribute("lat", lstInst[i].Latitude.ToString().Replace(',', '.'));
            marker.SetAttribute("lng", lstInst[i].Longitude.ToString().Replace(',', '.'));
            marker.SetAttribute("titulo", "Instalação");
            marker.SetAttribute("texto", strTexto);
            marker.SetAttribute("tipo", "I");

            markers.AppendChild(marker);
        }
        
        xml.Save(context.Response.OutputStream);
    }

    public bool IsReusable
    {
        get { return false; }
    }

    private string Coordenada(decimal numero, char positivo, char negativo)
    {
        decimal graus, minutos, segundos;
        char chrPosicao = (numero > 0) ? positivo : negativo;

        graus = Math.Abs(Math.Truncate(numero));
        minutos = Math.Abs(numero) - graus;
        segundos = minutos * 3600;
        minutos = 0;
        while (segundos > 60)
        {
            minutos++;
            segundos -= 60;
        }
        
        return graus.ToString("0") + "°" + minutos.ToString("0") + "\"" + segundos.ToString("0.####") + "' " + chrPosicao;
    }
}