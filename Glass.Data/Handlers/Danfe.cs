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
using Glass.Configuracoes;

namespace Glass.Data.Handlers
{
    public class Danfe : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var isCodigoBarras = context.Request["isCodigoBarras"] == "true";
            var dadosAdicionais = context.Request["dadosAdicionais"] == "true";
            var isQrCode = context.Request["isQrCode"] == "true";
            var idNf = context.Request["idNf"].StrParaUint();

            var bytes = isCodigoBarras ? GetBytesCodigoBarra(idNf, dadosAdicionais) : isQrCode ? GetBytesQrCode(idNf) :
                GetBytesRelatorio(context, idNf, context.Request["previsualizar"] == "true");

            context.Response.ContentType = isCodigoBarras || isQrCode ? "image/jpeg" : "application/pdf";

            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
        }

        public static byte[] GetBytesRelatorio(HttpContext context, uint idNf, bool preVisualizar)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            return GetBytesRelatorio(context, idNf, preVisualizar, out warnings, out streamids, out mimeType, out encoding, out extension);
        }

        public static byte[] GetBytesRelatorio(HttpContext context, uint idNf, bool preVisualizar, out Warning[] warnings, out string[] streamids, 
            out string mimeType, out string encoding, out string extension)
        {
            var report = new LocalReport();
            var lstParam = new List<ReportParameter>();
            var nf = NotaFiscalDAO.Instance.GetElement(idNf);
            var danfe = NFeDAO.Instance.GetForDanfe(context, nf.ChaveAcesso);
            var produtosNFe = ProdutosNFeDAO.Instance.GetForDanfe(context, nf.ChaveAcesso);
            var idLoja = nf.IdLoja.Value;

            report.DataSources.Add(new ReportDataSource("NFe", new NFe[] { danfe }));
            report.DataSources.Add(new ReportDataSource("ProdutosNFe", produtosNFe));

            #region NFC-e

            if (nf.Consumidor)
            {
                var loja = LojaDAO.Instance.GetElement(NotaFiscalDAO.Instance.ObtemIdLoja(idNf));
                var uf = loja.Uf;

                #region Report Path

                report.ReportPath = Utils.CaminhoRelatorio("Relatorios/NFe/rptDanfeNFCe{0}.rdlc");

                #endregion

                #region Pagamentos

                var pagamentos = PagtoNotaFiscalDAO.Instance.ObtemPagamentos((int)idNf);

                report.DataSources.Add(new ReportDataSource("PagtoNotaFiscal", pagamentos));

                #endregion

                #region Parametros

                // Parâmetros para o relatório                
                lstParam.Add(new ReportParameter("QtdItens", produtosNFe.Length.ToString()));

                // Verifica apenas se a nota não é cancelada, pois a nota só muda para autorizada depois de enviar o email,
                // fazendo com que os clientes recebesse o DANFE dizendo que a nota "não é um documento fiscal válido"
                lstParam.Add(new ReportParameter("Producao", (danfe.TipoAmbiente == (int)ConfigNFe.TipoAmbienteNfe.Producao && nf.Situacao != (int)NotaFiscal.SituacaoEnum.Cancelada && !preVisualizar).ToString().ToLower()));

                #region Total de tributos

                decimal valorTotalTributos = 0;
                var cfop = CfopDAO.Instance.GetElementByPrimaryKey(nf.IdCfop.Value);
                var cliente = ClienteDAO.Instance.GetElement(nf.IdCliente.Value);
                var lstProdNf = ProdutosNfDAO.Instance.GetByNfExtended(idNf);

                // CFOP's de simples faturamento/venda futura não devem destacar os impostos
                var exibirImpostoCfop = cfop.CodInterno != "5922" && cfop.CodInterno != "6922";
                
                try
                {
                    if (nf.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída && exibirImpostoCfop && cliente != null)
                    {
                        var retorno = ImpostoNcmUFDAO.Instance.ObtemDadosImpostos(lstProdNf);

                        valorTotalTributos = retorno.ValorImpostoNacional + retorno.ValorImpostoEstadual;
                    }
                }
                catch (Exception ex)
                {
                    ErroDAO.Instance.InserirFromException("BuscarTributos", ex);

                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao buscar tributos aproximados.", ex));
                }

                if (valorTotalTributos == 0 && nf.ValorTotalTrib > 0)
                    valorTotalTributos = nf.ValorTotalTrib;

                lstParam.Add(new ReportParameter("ValorTotalTributos", valorTotalTributos.ToString()));

                #endregion

                //Url para consultar a NFC-e por chave de acesso
                lstParam.Add(new ReportParameter("UrlConsulta", GetWebService.UrlConsultaPorChaveAcesso(uf, (ConfigNFe.TipoAmbienteNfe)danfe.TipoAmbiente)));

                // Define se a NFC-e foi emitida em contingência
                lstParam.Add(new ReportParameter("Contingencia", (nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.Normal).ToString().ToLower()));

                #endregion

                #region QR Code

                /* Chamado 44291. */
                if (FiscalConfig.CorrecaoGeracaoCodigoBarrasDanfe)
                {
                    var url = Utils.GetFullUrl(context, "/Handlers/Danfe.ashx?idNf=" + idNf + "&isQrCode=true");
                    var uri = new Uri(url);
                    lstParam.Add(new ReportParameter("QrCode",
                        string.Format("{0}://localhost:{1}{2}", uri.Scheme, uri.Port, uri.PathAndQuery)));
                }
                else
                    lstParam.Add(new ReportParameter("QrCode", Utils.GetUrlSite(context) +
                        "/Handlers/Danfe.ashx?idNf=" + idNf + "&isQrCode=true"));

                #endregion
            }

            #endregion

            #region NF-e

            else
            {
                #region Report Path

                report.ReportPath = Utils.CaminhoRelatorio("Relatorios/NFe/rptDanfeRetrato{0}.rdlc");

                #endregion

                #region Email

                if (FiscalConfig.NotaFiscalConfig.ExibirEmailFiscalDANFE && nf.IdLoja > 0)
                    danfe.EnderecoEmit += " " + LojaDAO.Instance.GetElementByPrimaryKey(nf.IdLoja.Value).EmailFiscal;

                #endregion

                #region Parametros

                if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaFSDA)
                {
                    report.ReportPath = Utils.CaminhoRelatorio("Relatorios/NFe/rptDanfeRetratoFS{0}.rdlc");

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

                lstParam.Add(new ReportParameter("TextoDadosRecebimento",
                    string.Format("RECEBEMOS DE {0} CNPJ {1} OS PRODUTOS E/OU SERVIÇOS CONSTANTES DA NOTA FISCAL ELETRÔNICA INDICADA AO LADO." +
                        "\nDESTINATÁRIO: {2} {3} Emissão: {4}. VALOR TOTAL: {5}.", danfe.RazaoSocialEmit, danfe.CnpjEmit,
                        danfe.RazaoSocialRemet, danfe.EnderecoRemet, danfe.DataEmissao, danfe.VlrTotalNota.StrParaDecimal().ToString("C"))));

                if (!report.ReportPath.Contains("rptDanfeSemLogo"))
                    lstParam.Add(new ReportParameter("Cabecalho_DestacarNFe", FiscalConfig.NotaFiscalConfig.DestacarNFe.ToString().ToLower()));

                lstParam.Add(new ReportParameter("Rodape_InformacoesCompl", danfe.InformacoesCompl));

                #endregion

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
            }

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

            return report.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
        }

        private static byte[] GetBytesCodigoBarra(uint idNf, bool dadosAdicionais)
        {
            var chaveAcesso = NotaFiscalDAO.Instance.ObtemChaveAcesso(idNf);
            var nfe = NFeDAO.Instance.GetForDanfe(chaveAcesso);
            return !dadosAdicionais ? nfe.BarCodeImage : nfe.BarCodeImageDadosAdicionais;
        }

        private static byte[] GetBytesQrCode(uint idNf)
        {
            var chaveAcesso = NotaFiscalDAO.Instance.ObtemChaveAcesso(idNf);
            var nfe = NFeDAO.Instance.GetForDanfe(chaveAcesso);
            return Utils.ObterQrCode(nfe.LinkQrCode ?? " ");
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}