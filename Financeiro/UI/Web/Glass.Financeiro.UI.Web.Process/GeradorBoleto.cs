using System.Web;
using WebGlass.Business.Boleto.Fluxo;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using System;

namespace Glass.Financeiro.UI.Web.Process
{
    public class GeradorBoleto : IGeradorBoleto
    {
        /// <summary>
        /// Gera o boleto
        /// </summary>
        /// <param name="codigoContaReceber"></param>
        /// <param name="codigoNotaFiscal"></param>
        /// <param name="codigoLiberacao"></param>
        /// <param name="codigoContaBanco"></param>
        /// <param name="carteira"></param>
        /// <param name="especieDocumento"></param>
        /// <param name="instrucoes"></param>
        /// <param name="ignorarNotificacaoBoletoImpresso"></param>
        /// <param name="conteudoBoleto"></param>
        /// <returns></returns>
        public Colosoft.Business.OperationResult<IEnumerable<uint>> GerarBoleto(
            int codigoContaReceber, int codigoNotaFiscal, int codigoLiberacao,
            int codigoContaBanco, string carteira, int especieDocumento, string[] instrucoes,
            System.IO.Stream conteudoBoleto)
        {
            var idsContasR = new List<uint>() { (uint)codigoContaReceber };

            if (codigoNotaFiscal > 0)
                idsContasR = ContasReceberDAO.Instance.ObtemPelaNfe((uint)codigoNotaFiscal).ToList();

            if ((idsContasR.Count == 0 || idsContasR.FirstOrDefault() == 0) && codigoLiberacao > 0)
            {
                var contasReceberLiberacao = ContasReceberDAO.Instance.GetByPedidoLiberacao(0, (uint)codigoLiberacao, null);

                if (contasReceberLiberacao != null && contasReceberLiberacao.Count > 0)
                {
                    if (!Glass.Configuracoes.FinanceiroConfig.EmitirBoletoApenasContaTipoPagtoBoleto)
                    {
                        idsContasR = contasReceberLiberacao.Select(f => f.IdContaR).ToList();
                    }
                    else
                    {
                        var idsContaRBoleto = new List<ContasReceber>();
                        var contasRecebimentoBoleto = UtilsPlanoConta.ContasRecebimentoBoleto().Split(',');

                        foreach (var item in contasReceberLiberacao)
                        {
                            if (contasRecebimentoBoleto.Contains(Conversoes.UintParaStr(item.IdConta)))
                            {
                                idsContaRBoleto.Add(item);
                            }
                        }

                        if (idsContaRBoleto.Count > 0)
                        {
                            idsContasR = idsContaRBoleto.Select(f => f.IdContaR).ToList();
                        }
                        else
                        {
                            throw new Exception("Nenhuma conta encontrada para gerar o boleto");
                        }
                    }
                }
            }

            using (var outPdf = new PdfSharp.Pdf.PdfDocument())
            {
                foreach (var id in idsContasR)
                {
                    var writer = new System.IO.StringWriter();
                    var htmlWriter = new System.Web.UI.HtmlTextWriter(writer);

                    BoletoNet.BoletoBancario.HtmlOfflineHeader(writer.GetStringBuilder());

                   DadosBoleto.Instance.ObtemDadosBoleto(id, (uint)codigoContaBanco, carteira, especieDocumento, instrucoes, htmlWriter);

                    BoletoNet.BoletoBancario.HtmlOfflineFooter(writer.GetStringBuilder());

                    htmlWriter.Flush();

                    var htmlStr = writer.GetStringBuilder().ToString();

                    var converter = new SelectPdf.HtmlToPdf(100);

                    converter.Options.MarginTop = 30;
                    converter.Options.MarginBottom = 10;
                    converter.Options.MarginLeft = 30;
                    converter.Options.MarginRight = 10;

                    var doc = converter.ConvertHtmlString(htmlStr);

                    using (var stream = new System.IO.MemoryStream())
                    {
                        doc.Save(stream);
                        using (var pdfBoleto = PdfSharp.Pdf.IO.PdfReader.Open(stream, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import))
                            CopyPages(pdfBoleto, outPdf);

                        doc.Close();
                    }
                }

                outPdf.Save(conteudoBoleto, false);

                if (Configuracoes.FinanceiroConfig.EnviarEmailEmitirBoleto &&
                    !Impresso.Instance.BoletoFoiImpresso((int)idsContasR.FirstOrDefault()) && codigoNotaFiscal > 0)
                {
                    var idLoja = NotaFiscalDAO.Instance.ObtemIdLoja((uint)codigoNotaFiscal);

                    if (idLoja == 0)
                    {
                        throw new Exception("Não foi possível recuperar a loja da nota fiscal ao salvar o e-mail a ser enviado.");
                    }

                    var idCliente = NotaFiscalDAO.Instance.ObtemIdCliente((uint)codigoNotaFiscal);

                    var email = ClienteDAO.Instance.GetEmail(null, idCliente.GetValueOrDefault(0));
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var numNfe = NotaFiscalDAO.Instance.ObtemNumeroNf(null, (uint)codigoNotaFiscal);

                        var texto = string.Format("Prezado(a) cliente,\nSegue anexo boleto da sua NF-e: {0}.\n\nAtt.\n{1}", numNfe,
                            LojaDAO.Instance.GetElement(null, idLoja).RazaoSocial);

                        var assunto = string.Format("Boleto NF-e: {0}", numNfe);

                        var caminho = string.Format("{0}", "BoletoNFe" + codigoNotaFiscal);

                        var anexo = new Data.Model.AnexoEmail(caminho, string.Format("boletoNFe{0}.pdf", numNfe));                                                      

                        uint idEmail = 0;

                        try
                        {
                            idEmail = Email.EnviaEmailAsyncComTransacao(idLoja, email, assunto, texto, Email.EmailEnvio.Comercial, false, anexo);

                            //Salva o pdf em uma pasta local
                            outPdf.Save(Armazenamento.ArmazenamentoIsolado.DiretorioBoletos + string.Format("\\anexo{0}.pdf", anexo.IdAnexoEmail));
                        }
                        catch (System.Exception ex)
                        {
                            ErroDAO.Instance.InserirFromException("GerarBoletoAnexoEmail", ex);

                            if (idEmail > 0)
                                FilaEmailDAO.Instance.DeleteByPrimaryKey(idEmail);
                        }
                    }
                }

                foreach (var b in idsContasR)
                    Impresso.Instance.IndicarBoletoImpresso((int)b, (int)codigoNotaFiscal, (int)codigoLiberacao, (int)codigoContaBanco, UserInfo.GetUserInfo);
            }

            return new Colosoft.Business.OperationResult<IEnumerable<uint>>(idsContasR);
        }

        private void CopyPages(PdfSharp.Pdf.PdfDocument from, PdfSharp.Pdf.PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
    }
}
