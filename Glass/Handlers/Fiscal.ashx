<%@ WebHandler Language="C#" Class="SIntegra" %>

using System;
using System.Web;
using Glass.Data.Helper;
using System.Collections.Generic;

public class SIntegra : Glass.Data.Handlers.HandlerDownloadArquivo
{
    public override string NomeArquivoDownload(HttpRequest request)
    {
        if (request["tipo"] == "SIntegra")
        {
            // Recuperação das informações passadas por GET
            DateTime inicio = DateTime.Parse(request["inicio"]).Date;
            DateTime fim = DateTime.Parse(request["fim"]).Date.AddDays(1).AddMilliseconds(-1);

            return "SIntegra - " + inicio.ToString("dd/MM/yyyy") + " a " + fim.ToString("dd/MM/yyyy") + ".txt";
        }
        else if (request["tipo"] == "EFD")
            return "EFD - " + request["mes"] + "-" + request["ano"] + ".txt";
            
        else if (request["tipo"] == "FCI")
            return "FCI_" + request["idArquivoFci"] + ".txt";

        return String.Empty;
    }

    public override byte[] ObtemBytesArquivoDownload(HttpRequest request)
    {
        string dados = "";
        
        if (request["tipo"] == "SIntegra")
        {
            // Recuperação das informações passadas por GET
            DateTime inicio = DateTime.Parse(request["inicio"]).Date;
            DateTime fim = DateTime.Parse(request["fim"]).Date.AddDays(1).AddMilliseconds(-1);

            // Recupera a listagem de registros
            dados = Glass.Data.SIntegra.SIntegra.Instance.RecuperaArquivoRegistros(Glass.Conversoes.StrParaUint(request["loja"]), inicio, fim,
                request["reg50"].ToLower() == "true", request["reg51"].ToLower() == "true",
                request["reg53"].ToLower() == "true", request["reg54"].ToLower() == "true",
                Glass.Configuracoes.FiscalConfig.SIntegra.SIntegraGerarRegistro61, request["reg70"].ToLower() == "true",
                request["reg74"].ToLower() == "true", request["reg75"].ToLower() == "true");
        }
        else if (request["tipo"] == "EFD")
        {
            List<DateTime> intervalos = new List<DateTime>();
            if (!String.IsNullOrEmpty(request["intervalo"]))
            {
                foreach (string intervalo in request["intervalo"].Split(','))
                    if (!String.IsNullOrEmpty(intervalo))
                        intervalos.Add(DateTime.Parse(intervalo));
            }

            // Recuperação das informações passadas por GET
            DateTime inicio = DateTime.Parse("01/" + request["mes"] + "/" + request["ano"]).Date;
            DateTime fim = DateTime.Parse(DateTime.DaysInMonth(Glass.Conversoes.StrParaInt(request["ano"]),
                Glass.Conversoes.StrParaInt(request["mes"])) + "/" + request["mes"] + "/" + request["ano"]).
                Date.AddDays(1).AddMilliseconds(-1);

            bool arquivoRetificador = request["arquivoRetificador"] == "true";

            // Recupera a listagem de registros
            switch (request["tipoEFD"])
            {
                case "icmsIpi":
                    dados = Glass.Data.EFD.EFD.Instance.RecuperaRegistrosFiscal(arquivoRetificador, request["numReciboOriginal"],
                        Glass.Conversoes.StrParaUint(request["loja"]), Glass.Conversoes.StrParaUint(request["contabilista"]), inicio, fim,
                        intervalos.ToArray(), request["blocoH"].ToLower() == "true", request["aidf"], request["blocoK"].ToLower() == "true");
                    break;

                case "pisCofins":
                    dados = Glass.Data.EFD.EFD.Instance.RecuperaRegistrosContribuicoes(arquivoRetificador, request["numReciboOriginal"],
                        request["loja"], Glass.Conversoes.StrParaUint(request["contabilista"]), inicio, fim,
                        Glass.Conversoes.StrParaInt(request["codIncTrib"]), Glass.Conversoes.StrParaIntNullable(request["indAproCred"]),
                        Glass.Conversoes.StrParaInt(request["codTipoCont"]));
                    break;
            }
        }
        else if (request["tipo"] == "FCI")
        {
            uint idArquivoFCI = 0;

            idArquivoFCI = Glass.Conversoes.StrParaUint(request["idArquivoFci"]);

            // Recupera a listagem de registros
            dados = Glass.Data.EFD.EFD.Instance.RecuperaRegistrosFCI(idArquivoFCI);

            using (System.IO.FileStream f = System.IO.File.Create(Utils.GetArquivoFCIPath + idArquivoFCI + ".txt"))
            {
                using (System.IO.StreamWriter w = new System.IO.StreamWriter(f))
                    w.Write(dados);
            }
        }

        if (request["tipo"] == "FCI")
            return System.Text.Encoding.UTF8.GetBytes(dados);
        else
            return System.Text.Encoding.Default.GetBytes(dados);
    }
}