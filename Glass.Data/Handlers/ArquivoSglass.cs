using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelDAL;
using Ionic.Utils.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Glass.Data.Handlers
{
    public class ArquivoSglass : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var nomeArquivo = "Arquivo SGLASS " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");

                var lstArqMesa = new List<byte[]>(); // Arquivos para mesa de corte
                var lstCodArq = new List<string>(); // Código dos arquivos para mesa de corte
                var lstErrosArq = new List<KeyValuePair<string, Exception>>(); // Erros ao gerar os arquivos

                //Busca os produtos do pedido espelho
                var lstProdPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetForRpt(context.Request["idPedido"].StrParaUint(), context.Request["idCliente"].StrParaUint(),
                    context.Request["NomeCliente"], context.Request["idLoja"].StrParaUint(), context.Request["idFunc"].StrParaUint(),
                    context.Request["idFuncionarioConferente"].StrParaUint(), context.Request["situacao"].StrParaInt(), context.Request["situacaoPedOri"],
                    context.Request["idsProcesso"], context.Request["dataIniEnt"], context.Request["dataFimEnt"], context.Request["dataIniFab"],
                    context.Request["dataFimFab"], context.Request["dataIniFin"], context.Request["dataFimFin"], context.Request["dataIniConf"],
                    context.Request["dataFimConf"], false, context.Request["pedidosSemAnexos"] == "true", context.Request["pedidosAComprar"] == "true",
                    context.Request["pedidos"], context.Request["situacaoCnc"], context.Request["dataIniSituacaoCnc"], context.Request["dataFimSituacaoCnc"],
                    context.Request["tipoPedido"], context.Request["idsRotas"], context.Request["origemPedido"].StrParaInt(), Conversoes.StrParaInt(context.Request["pedidosConferidos"]));

                var lstEtiqueta = EtiquetaDAO.Instance.EtiquetasGerarDxf(null, lstProdPedEsp);

                if (lstEtiqueta.Count == 0)
                    throw new Exception("Nenhum pedido filtrado possui arquivo SGLASS para ser gerado.");

                ImpressaoEtiquetaDAO.Instance.MontaArquivoMesaOptyway(null, lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, 0, false, (int)TipoArquivoMesaCorte.DXF, true, false, false);

                if (lstErrosArq.Any())
                {
                    var erros = string.Join("</br>", lstErrosArq.Where(f => !string.IsNullOrWhiteSpace(f.Key))
                        .Select(f => string.Format("Etiqueta: {0} Erro: {1}.", f.Key, Glass.MensagemAlerta.FormatErrorMsg(null, f.Value))));

                    context.Response.Write(string.Format("Situações com arquivos de mesa: </br></br>{0}", erros));
                    context.Response.Flush();
                    return;
                }

                if (lstArqMesa.Count == 0)
                    throw new Exception("Nenhum pedido filtrado possui arquivo SGLASS para ser gerado.");

                var tempPath = Path.GetTempPath();
                var tempDirectory = Directory.CreateDirectory(Path.Combine(tempPath, Guid.NewGuid().ToString()));
                var programsDirectory = tempDirectory.CreateSubdirectory("Programs");
                var hardwaresDirectory = tempDirectory.CreateSubdirectory("hardwares");
                

                for (int i = 0; i < lstArqMesa.Count; i++)
                {
                    var fileName = Path.GetFileNameWithoutExtension(lstCodArq[i]);
                    var zipPath = Path.Combine(tempPath, fileName);
                
                    using(var zip = ZipFile.Read(lstArqMesa[i]))
                    {
                        zip.ExtractAll(zipPath, true);
                    }

                    var programsPath = Path.Combine(zipPath, "Programs");
                    var hardwaresPath = Path.Combine(zipPath, "hardwares");
                    var files = Directory.GetFiles(programsPath);
                    File.Move(files[0], Path.Combine(programsPath, (fileName + Path.GetExtension(files[0]))));

                    if (Directory.Exists(programsPath))
                        foreach (var file in Directory.GetFiles(programsPath))
                            File.Move(file, Path.Combine(programsDirectory.FullName, Path.GetFileName(file)));

                    if (Directory.Exists(hardwaresPath))
                        foreach (var file in Directory.GetFiles(hardwaresPath))
                            File.Move(file, Path.Combine(hardwaresDirectory.FullName, Path.GetFileName(file)));

                    Directory.Delete(zipPath, true);
                }

                // Adiciona o arquivo de otimização ao zip            
                context.Response.ContentType = "application/zip";
                context.Response.AddHeader("content-disposition", "attachment; filename=\"" + nomeArquivo + ".zip\"");

                //Adiciona os arquivos SGLASS
                using (ZipFile zip = new ZipFile(context.Response.OutputStream))
                {
                    zip.AddDirectory(tempDirectory.FullName, "SGLASS");
                    zip.Save();
                }

                Directory.Delete(tempDirectory.FullName, true);
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
    }
}
