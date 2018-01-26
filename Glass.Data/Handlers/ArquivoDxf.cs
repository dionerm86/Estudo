﻿using System;
using System.Web;
using Ionic.Utils.Zip;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using Glass.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.Handlers
{
    public class ArquivoDxf : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var nomeArquivo = "Arquivo DXF " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");

                var lstArqMesa = new List<byte[]>(); // Arquivos para mesa de corte
                var lstCodArq = new List<string>(); // Código dos arquivos para mesa de corte
                var lstErrosArq = new List<KeyValuePair<string, Exception>>(); // Erros ao gerar os arquivos
                var errosGeracaoMarcacao = string.Empty;

                //Busca os produtos do pedido espelho
                var lstProdPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(context.Request["idPedido"]),
                    Glass.Conversoes.StrParaUint(context.Request["idCliente"]), context.Request["NomeCliente"],
                    Glass.Conversoes.StrParaUint(context.Request["idLoja"]), Glass.Conversoes.StrParaUint(context.Request["idFunc"]),
                    Glass.Conversoes.StrParaUint(context.Request["idFuncionarioConferente"]), Glass.Conversoes.StrParaInt(context.Request["situacao"]),
                    context.Request["situacaoPedOri"], context.Request["idsProcesso"], context.Request["dataIniEnt"], context.Request["dataFimEnt"],
                    context.Request["dataIniFab"], context.Request["dataFimFab"], context.Request["dataIniFin"], context.Request["dataFimFin"],
                    context.Request["dataIniConf"], context.Request["dataFimConf"], false, context.Request["pedidosSemAnexos"] == "true",
                    context.Request["pedidosAComprar"] == "true", context.Request["pedidos"], context.Request["situacaoCnc"],
                    context.Request["dataIniSituacaoCnc"], context.Request["dataFimSituacaoCnc"], context.Request["tipoPedido"], context.Request["idsRotas"],
                    Glass.Conversoes.StrParaInt(context.Request["origemPedido"]), Conversoes.StrParaInt(context.Request["pedidosConferidos"]));

                var lstEtiqueta = EtiquetaDAO.Instance.EtiquetasGerarDxf(null, lstProdPedEsp);

                if (lstEtiqueta.Count == 0)
                    throw new Exception("Nenhum pedido filtrado possui arquivo DXF para ser gerado.");

                ImpressaoEtiquetaDAO.Instance.MontaArquivoMesaOptyway(null, lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, 0, false, (int)TipoArquivoMesaCorte.DXF, false, false, false);

                // Verifica se existe algum erro tratado no momento da geração do arquivo.
                if (lstErrosArq != null && lstErrosArq.Any(f => f.Value != null))
                    // Monta um texto com todos os problemas ocorridos ao gerar o arquivo de mesa, ao final do método, o texto é salvo em um arquivo separado e é zipado junto com o ASC.
                    errosGeracaoMarcacao = string.Format("Situações com arquivos de mesa: </br></br>{0}",
                        string.Join("</br>", lstErrosArq.Where(f => f.Value != null).Select(f => string.Format("Etiqueta: {0} Erro: {1}.", f.Key, Glass.MensagemAlerta.FormatErrorMsg(null, f.Value)))));

                // Adiciona o arquivo de otimização ao zip            
                context.Response.ContentType = "application/zip";
                context.Response.AddHeader("content-disposition", "attachment; filename=\"" + nomeArquivo + ".zip\"");

                // Adiciona os arquivos DXF
                using (ZipFile zip = new ZipFile(context.Response.OutputStream))
                {
                    for (var i = 0; i < lstArqMesa.Count; i++)
                        zip.AddFileStream(lstCodArq[i].Replace("  ", string.Empty).Replace(" ", string.Empty), string.Empty, new System.IO.MemoryStream(lstArqMesa[i]));

                    zip.AddStringAsFile(errosGeracaoMarcacao, "Situações com arquivos de mesa.error", string.Empty);

                    zip.Save();
                }
            }
            catch (Exception ex)
            {
                // Devolve o erro
                context.Response.ContentType = "text/html";
                context.Response.Write(GetErrorResponse(ex));
            }
        }

        private string GetErrorResponse(Exception ex)
        {
            bool debug = false;

            string html = debug ? ex.ToString().Replace("\n", "<br>").Replace("\r", "").Replace(" ", "&nbsp;") : @"
            <script type='text/javascript'>
                alert('" + Glass.MensagemAlerta.FormatErrorMsg("", ex) + @"');
                window.history.go(-1);
            </script>";

            return @"
            <html>
                <body>
                    " + html + @"
                </body>
            </html>";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}