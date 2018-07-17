using System;
using System.Web;
using Ionic.Utils.Zip;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using Glass.Data.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Glass.Data.Handlers
{
    public class ArquivoIntermac : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var nomeArquivo = "Arquivo Intermac " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");

                var lstArqMesa = new List<byte[]>(); // Arquivos para mesa de corte
                var lstCodArq = new List<string>(); // Código dos arquivos para mesa de corte
                var lstErrosArq = new List<KeyValuePair<string, Exception>>(); // Erros ao gerar os arquivos

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
                    throw new Exception("Nenhum pedido filtrado possui arquivo Intermac para ser gerado.");

                ImpressaoEtiquetaDAO.Instance.MontaArquivoMesaOptyway(null, lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, 0, false, (int)TipoArquivoMesaCorte.DXF, false, true, true);

                if (!lstArqMesa.Any() && !lstErrosArq.Any())
                {
                    var mensagem = "O pedido não possui projetos com arquivos para execução na máquina.";
                    context.Response.Write(string.Format("<script>alert(\"{0}\"); window.close();</script>", mensagem));
                    context.Response.Flush();
                    return;
                }

                if (lstErrosArq.Any())
                {
                    var erros = string.Join("</br>", lstErrosArq.Where(f => !string.IsNullOrWhiteSpace(f.Key))
                        .Select(f => string.Format("Etiqueta: {0} Erro: {1}.", f.Key, Glass.MensagemAlerta.FormatErrorMsg(null, f.Value))));

                    context.Response.Write(string.Format("Situações com arquivos de mesa: </br></br>{0}", erros));
                    context.Response.Flush();
                    return;
                }

                // Adiciona o arquivo de otimização ao zip            
                context.Response.ContentType = "application/zip";
                context.Response.AddHeader("content-disposition", "attachment; filename=\"" + nomeArquivo + ".zip\"");

                // Adiciona os arquivos
                using (ZipFile zip = new ZipFile(context.Response.OutputStream))
                {
                    for (var i = 0; i < lstArqMesa.Count; i++)
                    {
                        if (Glass.Data.Helper.Utils.VerificarArquivoZip(lstArqMesa[i]))
                        {
                            using (var zip2 = ZipFile.Read(lstArqMesa[i]))
                            {
                                foreach(var entryFileName in zip2.EntryFilenames)
                                {
                                    var entryStream = new System.IO.MemoryStream();
                                    zip2.Extract(entryFileName, entryStream);
                                    entryStream.Position = 0;

                                    var fileName = System.IO.Path.GetFileName(entryFileName);
                                    var extension = System.IO.Path.GetExtension(fileName)?.ToLower();
                                    if (extension == ".cni" || extension == ".xml")
                                        fileName = System.IO.Path.GetFileNameWithoutExtension(lstCodArq[i].Trim()) + extension;

                                    zip.AddFileStream(fileName, System.IO.Path.GetDirectoryName(entryFileName), entryStream);
                                }
                            }
                        }
                        else
                            zip.AddFileStream(lstCodArq[i].Replace("  ", "").Replace(" ", ""), "", new System.IO.MemoryStream(lstArqMesa[i]));
                    }
                    zip.Save();
                }
            }
            catch (Exception ex)
            {
                // Devolve o erro
                context.Response.ContentType = "text/html";
                context.Response.Write(GetErrorResponse(ex));
                context.Response.Write("<script>window.close();</script>");
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