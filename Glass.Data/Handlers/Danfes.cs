using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.NFeUtils;
using Glass.Data.RelDAL;
using Glass.Data.RelModel;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.Handlers
{
    public class Danfes : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var isCodigoBarras = context.Request["isCodigoBarras"] == "true";
            var dadosAdicionais = context.Request["dadosAdicionais"] == "true";
            var isQrCode = context.Request["isQrCode"] == "true";
            var idsNf = context.Request["idNf"].Split(',').Select(f => f.StrParaUint()).ToArray();
            context.Response.ContentType = isCodigoBarras || isQrCode ? "image/jpeg" : "application/pdf";         
            
            var bytes = GetBytesRelatorio(context, idsNf, context.Request["previsualizar"] == "true");
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);            
        }

        public static byte[] GetBytesRelatorio(HttpContext context, uint[] idsNf, bool preVisualizar)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            return GetBytesRelatorio(context, idsNf, preVisualizar, out warnings, out streamids, out mimeType, out encoding, out extension);
        }

        public static byte[] GetBytesRelatorio(HttpContext context, uint[] idsNf, bool preVisualizar, out Warning[] warnings, out string[] streamids, 
            out string mimeType, out string encoding, out string extension)
        {
            var report = new LocalReport();
            var lstParam = new List<ReportParameter>();

            foreach (var idNf in idsNf)
            {
                var nf = NotaFiscalDAO.Instance.GetElement(idNf);
                var danfe = NFeDAO.Instance.GetForDanfe(context, nf.ChaveAcesso);
                var produtosNFe = ProdutosNFeDAO.Instance.GetForDanfe(context, nf.ChaveAcesso);
                var idLoja = nf.IdLoja.Value;

                report.DataSources.Add(new ReportDataSource("NFe", new NFe[] { danfe }));
                report.DataSources.Add(new ReportDataSource("ProdutosNFe", produtosNFe));

                #region NF-e

                #region Report Path

                //report.ReportPath = FiscalConfig.NotaFiscalConfig.CaminhoDANFE((int)nf.IdLoja.GetValueOrDefault());

                #endregion

                if (nf.IdLoja > 0)
                    danfe.EnderecoEmit += " " + LojaDAO.Instance.GetElementByPrimaryKey(nf.IdLoja.Value).EmailFiscal;
                    danfe.EmailFiscal += " " + LojaDAO.Instance.GetElementByPrimaryKey(nf.IdLoja.Value).EmailFiscal;

                #region Parametros

                if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaFSDA)
                {
                    report.ReportPath = report.ReportPath.Replace(".", "FS.");

                    lstParam.Add(new ReportParameter("Cabecalho_DadosAdicionaisNfe", danfe.DadosAdicionaisFs));
                    lstParam.Add(new ReportParameter("Cabecalho_CodigoBarrasAdicionais", Utils.GetUrlSite(context) +
                        "/Handlers/Danfe.ashx?idNf=" + idNf + "&isCodigoBarras=true&dadosAdicionais=true"));
                }

                // Verifica apenas se a nota não é cancelada, pois a nota só muda para autorizada depois de enviar o email,
                // fazendo com que os clientes recebesse o DANFE dizendo que a nota "não é um documento fiscal válido"
                lstParam.Add(new ReportParameter("Producao", (danfe.TipoAmbiente == (int)ConfigNFe.TipoAmbienteNfe.Producao && nf.Situacao != (int)NotaFiscal.SituacaoEnum.Cancelada && !preVisualizar).ToString().ToLower()));

                lstParam.Add(new ReportParameter("ExibirNFCancelada",
                    (!preVisualizar && danfe.TipoAmbiente == (int)ConfigNFe.TipoAmbienteNfe.Producao &&
                    nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada).ToString().ToLower()));

                // Parâmetros para o cabeçalho/rodapé do relatório
                lstParam.Add(new ReportParameter("Cabecalho_NumeroNfe", danfe.NumeroNfe));
                lstParam.Add(new ReportParameter("Cabecalho_SerieNfe", danfe.SerieNfe));
                lstParam.Add(new ReportParameter("Cabecalho_RazaoSocialEmit", danfe.RazaoSocialEmit));
                lstParam.Add(new ReportParameter("Cabecalho_EnderecoEmit", danfe.EnderecoEmit));
                lstParam.Add(new ReportParameter("Cabecalho_TipoNfe", danfe.TipoNfe));
                lstParam.Add(new ReportParameter("Cabecalho_ChaveAcesso", danfe.ChaveAcesso));
                lstParam.Add(new ReportParameter("Cabecalho_NatOperacao", danfe.NatOperacao));
                lstParam.Add(new ReportParameter("Cabecalho_ProtAutorizacao", String.IsNullOrEmpty(danfe.ProtAutorizacao) ? "." : danfe.ProtAutorizacao));
                lstParam.Add(new ReportParameter("Cabecalho_InscEstEmit", danfe.InscEstEmit));
                lstParam.Add(new ReportParameter("Cabecalho_InscEstStEmit", danfe.InscEstStEmit));
                lstParam.Add(new ReportParameter("Cabecalho_CnpjEmit", danfe.CnpjEmit));

                if (report.ReportPath != "Relatorios/NFe/rptDanfeSemLogo.rdlc")
                    lstParam.Add(new ReportParameter("Cabecalho_DestacarNFe", "false"));

                #region Código de barras

                /* Chamado 18187. */
                if (FiscalConfig.CorrecaoGeracaoCodigoBarrasDanfe)
                {
                    var url = Utils.GetFullUrl(context, "/Handlers/Danfe.ashx?idNf=" + idNf + "&isCodigoBarras=true");
                    var uri = new Uri(url);
                    lstParam.Add(new ReportParameter("Cabecalho_CodigoBarras",
                        string.Format("{0}://localhost:{1}{2}", uri.Scheme, uri.Port, uri.PathAndQuery)));
                }
                else
                    lstParam.Add(new ReportParameter("Cabecalho_CodigoBarras", Utils.GetUrlSite(context) +
                        "/Handlers/Danfe.ashx?idNf=" + idNf + "&isCodigoBarras=true"));

                #endregion

                lstParam.Add(new ReportParameter("Rodape_InformacoesCompl", danfe.InformacoesCompl));

                #endregion


                #endregion

                #region LogoTipo

                var logotipo = Logotipo.GetReportLogoNF(context.Request, idLoja);

                if (FiscalConfig.NotaFiscalConfig.EsconderLogotipoDANFEComLogo)
                    logotipo = ".";

                report.EnableExternalImages = true;
                lstParam.Add(new ReportParameter("Logotipo", logotipo));

                #endregion

                //Ajusta o caminho do report
                report.ReportPath = context.Server.MapPath("~/" + report.ReportPath);

                // Atribui parâmetros ao relatório
                report.SetParameters(lstParam);                
            }

            return report.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
        }

     
        public bool IsReusable
        {
            get { return false; }
        }
    }
}