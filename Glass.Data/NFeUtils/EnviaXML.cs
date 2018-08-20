using System;
using System.Xml;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;
using System.IO;
using GDA;
using System.Linq;
using System.Collections.Generic;

namespace Glass.Data.NFeUtils
{
    public class EnviaXML
    {
        #region Cria lote

        /// <summary>
        /// Monta e retorna um Xml do lote para envio com a NF-e a ser enviada
        /// </summary>
        private static XmlDocument CriaLote(XmlDocument xmlNFe, uint idNf)
        {
            XmlDocument xmlLote = new XmlDocument();
            XmlNode declarationNode = xmlLote.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlLote.AppendChild(declarationNode);

            var ufLoja = LojaDAO.Instance.GetUf(NotaFiscalDAO.Instance.ObtemIdLoja(idNf));

            /* Chamado 50403. */
            if (string.IsNullOrEmpty(ufLoja))
                throw new Exception("Não foi possível recuperar a UF da loja associada à nota fiscal.");

            //Servidores da Bahia não aceitam emissão síncrona.
            var indsinc = ufLoja.ToUpper() == "BA" || ufLoja.ToUpper() == "SP" ? "0" : "1";

            string enviNFeString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<enviNFe xmlns=\"http://www.portalfiscal.inf.br/nfe\" versao=\"" + ConfigNFe.VersaoLoteNFe + "\">" +
                "<idLote>" + NotaFiscalDAO.Instance.GetNewNumLote(idNf).ToString("000000000000000") + "</idLote>" +
                "<indSinc>" + indsinc + "</indSinc>";

            // Insere o XML da NFe no lote
            int nPosI = xmlNFe.InnerXml.IndexOf("<NFe");
            int nPosF = xmlNFe.InnerXml.Length - nPosI;
            enviNFeString += xmlNFe.InnerXml.Substring(nPosI, nPosF) + "</enviNFe>";

            XmlDocument xmlValidar = new XmlDocument();
            XmlDocument xmlRetorno = new XmlDocument();

            /* Chamado 51011. */
            if (NotaFiscalDAO.Instance.IsNotaFiscalConsumidor(idNf))
                xmlRetorno.LoadXml(enviNFeString.Replace("<qrCode>", "<qrCode><![CDATA[").Replace("</qrCode>", "]]></qrCode>").Replace("&amp;", "&"));
            else
                xmlRetorno.LoadXml(enviNFeString);

            xmlValidar.LoadXml(enviNFeString);

            #region Valida XML

            try
            {
                ValidaXML.Validar(xmlValidar, ValidaXML.TipoArquivoXml.EnviNFe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            return xmlRetorno;
        }

        /// <summary>
        /// Monta e retorna um Xml do lote para envio de carta de correção
        /// </summary>
        public static XmlDocument CriaLoteCce(XmlDocument xmlCce)
        {
            XmlDocument xmlLote = new XmlDocument();
            XmlNode declarationNode = xmlLote.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlLote.AppendChild(declarationNode);

            string envEventoString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<envEvento xmlns=\"http://www.portalfiscal.inf.br/nfe\" versao=\"1.00\">" +
                "<idLote>" + ContadorRecepcaoEventoDAO.Instance.GetNext() + "</idLote>";

            // Insere o XML da CCe no lote
            int nPosI = xmlCce.InnerXml.IndexOf("<evento");
            int nPosF = xmlCce.InnerXml.Length - nPosI;
            envEventoString += xmlCce.InnerXml.Substring(nPosI, nPosF) + "</envEvento>";

            XmlDocument xmlRetorno = new XmlDocument();
            xmlRetorno.LoadXml(envEventoString);

            #region Valida XML

            try
            {
                ValidaXML.Validar(xmlRetorno, ValidaXML.TipoArquivoXml.EnviCce);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            return xmlRetorno;
        }

        /// <summary>
        /// Monta e retorna um Xml do lote para envio de cancelamento da NF-e
        /// </summary>
        public static XmlDocument CriaLoteCancelamento(XmlDocument xmlCancelamento)
        {
            XmlDocument xmlLote = new XmlDocument();
            XmlNode declarationNode = xmlLote.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlLote.AppendChild(declarationNode);

            string envEventoString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<envEvento xmlns=\"http://www.portalfiscal.inf.br/nfe\" versao=\"1.00\">" +
                "<idLote>" + ContadorRecepcaoEventoDAO.Instance.GetNext() + "</idLote>";

            // Insere o XML do cancelamento no lote
            int nPosI = xmlCancelamento.InnerXml.IndexOf("<evento");
            int nPosF = xmlCancelamento.InnerXml.Length - nPosI;
            envEventoString += xmlCancelamento.InnerXml.Substring(nPosI, nPosF) + "</envEvento>";

            XmlDocument xmlRetorno = new XmlDocument();
            xmlRetorno.LoadXml(envEventoString);

            #region Valida XML

            try
            {
                ValidaXML.Validar(xmlRetorno, ValidaXML.TipoArquivoXml.EnviEvtCancNfe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            return xmlRetorno;
        }

        #endregion

        #region Envia pedido de autorização de NFe

        /// <summary>
        /// Envia a NFe para a SEFAZ via Webservice
        /// </summary>
        public static string EnviaNFe(XmlDocument xmlNFe, uint idNf)
        {
            var numeroNfe = NotaFiscalDAO.Instance.ObtemNumeroNf(null, idNf);

            try
            {
                // Monta o lote
                XmlDocument xmlLote = CriaLote(xmlNFe, idNf);

                /* Chamado 51011. */
                if (NotaFiscalDAO.Instance.IsNotaFiscalConsumidor(idNf))
                    xmlLote.InnerXml = xmlLote.InnerXml.Replace("&amp;", "&");

                // Busca dados da Note Fiscal
                NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(idNf);

                #region Envia XML

                // Instancia xml de retorno
                XmlNode xmlRetorno = null;

                // Salva o callback padrão do WebService
                System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

                // Realiza 3 tentativas de envio, se não funcionar, gera mensagem de erro
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        // Altera o callback de validação do WebService
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
                        {
                            // Verifica se a data do certificado é válida
                            DateTime beginDate = DateTime.Parse(cert.GetEffectiveDateString());
                            DateTime endDate = DateTime.Parse(cert.GetExpirationDateString());
                            bool isDateValid = (DateTime.Now >= beginDate) && (DateTime.Now <= endDate);

                            // Retorna o resultado da função
                            return isDateValid;
                        };

                        // Envia o arquivo e recebe o retorno.
                        if (nf.Consumidor)
                        {
                            xmlRetorno = ObterXmlAutorizacaoNFCe(nf, xmlLote);
                        }
                        else
                        {
                            xmlRetorno = ObterXmlAutorizacaoNFe(nf, xmlLote);
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 2)
                        {
                            var mensagemErro = ex.Message;

                            if(mensagemErro.Contains("The remote name could not be resolved"))
                                mensagemErro = @"Serviço da receita federal indisponível para emissão de NFe.
                                    (é possível acompanhar a disponibilidade dos serviços  pelo site: www.nfe.fazenda.gov.br, clicando na opção “Consultar Disponibilidade“";

                            LogNfDAO.Instance.NewLog(idNf, "Emissão", 1, "Falha ao enviar lote."  + mensagemErro);
                            NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);
                            return "Falha ao enviar lote. " + mensagemErro;
                        }
                    }
                    finally
                    {
                        // Restaura o callback padrão para o WebService
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
                    }
                }

                #endregion

                #region Lê Xml de retorno do envio do lote

                // Verifica se o Xml de Retorno é válido
                if (xmlRetorno == null)
                {
                    LogNfDAO.Instance.NewLog(idNf, "Emissão", 2, "Falha ao enviar lote. Retorno de envio do lote inválido.");
                    NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                    return "Falha ao enviar lote. Retorno de envio do lote inválido.";
                }

                // Lê Xml de retorno do envio do lote
                var status = xmlRetorno?["cStat"]?.InnerXml ?? "0";

                if (status == "103") // Lote recebido com sucesso
                {
                    var numReciboLote = xmlRetorno?["infRec"]?["nRec"]?.InnerXml;

                    LogNfDAO.Instance.NewLog(idNf, "Emissão", 103, "Lote enviado com sucesso. ");
                    NotaFiscalDAO.Instance.RetornoEnvioLote(idNf, numReciboLote);

                    return "Lote enviado com sucesso.";
                }
                else if (status == "104")
                {
                    if (xmlRetorno?["protNFe"] == null)
                    {
                        if (xmlRetorno?["cStat"]?.InnerXml == "104")
                        {
                            var mensagem = "Lote processado.";
                            var numReciboLote = xmlRetorno?["infRec"]?["nRec"]?.InnerXml;

                            LogNfDAO.Instance.NewLog(idNf, "Emissão", 104, mensagem);
                            NotaFiscalDAO.Instance.RetornoEnvioLote(idNf, numReciboLote);

                            return mensagem;
                        }
                        else
                        {
                            var motivo = xmlRetorno?["xMotivo"]?.InnerXml;

                            LogNfDAO.Instance.NewLog(idNf, "Emissão", (xmlRetorno?["cStat"]?.InnerXml.StrParaInt()).GetValueOrDefault(), motivo);
                            NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                            return motivo;
                        }
                    }
                    else if (xmlRetorno?["protNFe"]?["infProt"]?["cStat"]?.InnerXml == "100")
                    {
                        LogNfDAO.Instance.NewLog(idNf, "Emissão", 104, "Lote Processado");
                        NotaFiscalDAO.Instance.RetornoEmissaoNFe(nf.ChaveAcesso, xmlRetorno?["protNFe"]);

                        return xmlRetorno?["protNFe"]?["infProt"]?["xMotivo"]?.InnerXml;
                    }
                    else
                    {
                        var codigo = (xmlRetorno?["protNFe"]?["infProt"]?["cStat"]?.InnerXml?.StrParaInt()).GetValueOrDefault();
                        var motivo = TrataMotivoRejeicaoNFe(codigo, xmlRetorno?["protNFe"]?["infProt"]?["xMotivo"]?.InnerXml ?? string.Empty);

                        LogNfDAO.Instance.NewLog(idNf, "Emissão", codigo, motivo);
                        NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                        return motivo;
                    }
                }
                else if (Convert.ToInt32(status) > 200) // Lote foi rejeitado pela SEFAZ
                {
                    var codigo = status.StrParaInt();
                    var motivo = TrataMotivoRejeicaoNFe(codigo, xmlRetorno?["xMotivo"]?.InnerXml ?? string.Empty);

                    if (string.IsNullOrWhiteSpace(motivo))
                    {
                        motivo = $"Falha ao emitir NFe. Código de Rejeição: { codigo }.";
                    }

                    try
                    {
                        var uf = xmlRetorno?["cUF"]?.InnerXml ?? string.Empty;

                        if (!string.IsNullOrEmpty(uf))
                        {
                            motivo = string.Format("{0} UF: {1}", motivo, CidadeDAO.Instance.GetNomeUf(null, uf.StrParaUintNullable()));
                        }

                        ErroDAO.Instance.InserirFromException(string.Format("RetornoEnvioNFe {0} - {1}", numeroNfe, xmlRetorno?.InnerXml ?? "xmlRetorno nulo"), new Exception());
                    }
                    catch (Exception ex)
                    {
                        ErroDAO.Instance.InserirFromException(string.Format("RetornoEnvioNFe {0}", numeroNfe), ex);
                    }

                    LogNfDAO.Instance.NewLog(idNf, "Emissão", codigo, motivo);
                    NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                    return motivo;
                }
                else
                {
                    LogNfDAO.Instance.NewLog(idNf, "Emissão", status.StrParaInt(), xmlRetorno?["xMotivo"]?.InnerXml ?? string.Empty);

                    try
                    {
                        ErroDAO.Instance.InserirFromException(string.Format("RetornoEnvioNFe {0} - {1}", numeroNfe, xmlRetorno?.InnerXml ?? "xmlRetorno nulo"), new Exception());
                    }
                    catch (Exception ex)
                    {
                        ErroDAO.Instance.InserirFromException(string.Format("RetornoEnvioNFe {0}", numeroNfe), ex);
                    }

                    return xmlRetorno?["xMotivo"]?.InnerXml;
                }

                #endregion
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException(string.Format("Falha no método RetornoEnvioNFe {0}", numeroNfe), ex);

                LogNfDAO.Instance.NewLog(idNf, "Emissão", 1, Glass.MensagemAlerta.FormatErrorMsg("Falha ao enviar lote.", ex));
                NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                return Glass.MensagemAlerta.FormatErrorMsg("Falha ao enviar lote.", ex);
            }
        }

        /// <summary>
        /// Melhora a mensagem de rejeição retornada pela receita
        /// </summary>
        private static string TrataMotivoRejeicaoNFe(int codigo, string motivo)
        {
            if (codigo == 531)
            {
                motivo += ". Possivelmente o CST ou CSOSN utilizado nos produtos da nota não permite o destaque do ICMS ou ICMS ST.";
            }
            else if (codigo == 533)
            {
                motivo += ". Possivelmente o CST ou CSOSN utilizado nos produtos da nota não permite o destaque do ICMS ST.";
            }
            else if (codigo == 321)
            {
                motivo += ". Possivelmente a nota fiscal referenciada não possui chave de acesso ou o CFOP utilizado na nota fiscal não é do tipo Devolução.";
            }

            return motivo;
        }

        #endregion

        #region Envia pedido de autorização da CCe

        public static string EnviaCCe(XmlDocument xmlCce, uint idCarta)
        {
            var retorno = string.Empty;

            CartaCorrecao carta = CartaCorrecaoDAO.Instance.GetElement(idCarta);
            NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(carta.IdNf);

            try
            {
                // Monta o lote
                XmlDocument xmlLote = CriaLoteCce(xmlCce);

                #region Envia XML

                // Instancia xml de retorno
                XmlNode xmlRetorno = null;

                // Salva o callback padrão do WebService
                System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

                // Realiza 3 tentativas de envio, se não funcionar, gera mensagem de erro
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        // Altera o callback de validação do WebService
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
                        {
                            // Verifica se a data do certificado é válida
                            DateTime beginDate = DateTime.Parse(cert.GetEffectiveDateString());
                            DateTime endDate = DateTime.Parse(cert.GetExpirationDateString());
                            bool isDateValid = (DateTime.Now >= beginDate) && (DateTime.Now <= endDate);

                            // Retorna o resultado da função
                            return isDateValid;
                        };

                        if (ConfigNFe.TipoAmbiente != ConfigNFe.TipoAmbienteNfe.Producao)
                        {
                            throw new Exception("A carta de correção está implementada somente para o ambiente de produção.");
                        }

                        // Envia o arquivo e recebe o retorno.
                        xmlRetorno = ObterXmlRecepcaoEventoNFe(nf, xmlLote);

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 2)
                        {
                            LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", 1, Glass.MensagemAlerta.FormatErrorMsg("Falha ao enviar carta de correção.", ex));
                            return "Falha ao enviar carta de correção. " + ex.Message;
                        }
                    }
                    finally
                    {
                        // Restaura o callback padrão para o WebService
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
                    }
                }

                #endregion

                #region Lê Xml de retorno do envio do lote

                try
                {
                    // Verifica se o Xml de Retorno é válido
                    if (xmlRetorno == null)
                    {
                        LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", 2, "Falha ao enviar carta de correção. Retorno de envio de carta de correção inválido.");

                        return "Falha ao enviar carta de correção. Retorno de envio de carta de correção inválido.";
                    }

                    // Lê Xml de retorno do envio do lote
                    var status = xmlRetorno?["cStat"]?.InnerText;
                    var resposta = xmlRetorno?["xMotivo"]?.InnerText;
                    var statusProcessamento = (xmlRetorno?["retEvento"]?["infEvento"]?["cStat"]?.InnerText?.StrParaInt() ?? status?.StrParaInt()).GetValueOrDefault();
                    var respostaProcessamento = xmlRetorno?["retEvento"]?["infEvento"]?["xMotivo"]?.InnerText ?? resposta;

                    // Salva o retorno apenas se tiver sido aceito
                    if (statusProcessamento == 135 || statusProcessamento == 136)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xmlRetorno.OuterXml);

                        var fileName = $"{ Utils.GetCartaCorrecaoXmlPath }{ idCarta.ToString().PadLeft(9, '0') }-cce.xml";

                        if (System.IO.File.Exists(fileName))
                        {
                            System.IO.File.Delete(fileName);
                        }

                        doc.Save(fileName);
                    }

                    switch (statusProcessamento)
                    {
                        //Somente os casos 135 e 136 são aceitos
                        case 135:
                            {
                                LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", statusProcessamento, respostaProcessamento);
                                // Salva o protocolo
                                CartaCorrecaoDAO.Instance.SalvaProtocolo(carta.IdCarta, xmlRetorno?["retEvento"]?["infEvento"]?["nProt"]?.InnerXml);
                                CartaCorrecaoDAO.Instance.AtualizaSituacao(carta.IdCarta, (uint)CartaCorrecao.SituacaoEnum.Registrada);
                                break;
                            }
                        case 136:
                            {
                                LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", statusProcessamento, respostaProcessamento);
                                // Salva o protocolo
                                CartaCorrecaoDAO.Instance.SalvaProtocolo(carta.IdCarta, xmlRetorno?["retEvento"]?["infEvento"]?["nProt"]?.InnerXml);
                                CartaCorrecaoDAO.Instance.AtualizaSituacao(carta.IdCarta, (uint)CartaCorrecao.SituacaoEnum.Registrada);
                                break;
                            }
                        default:
                            {
                                LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", statusProcessamento, respostaProcessamento);

                                // Se a rejeição for por causa do código 594, exclui a carta, uma vez que para este caso é isso que o usuário deveria fazer.
                                if (statusProcessamento != 594)
                                {
                                    CartaCorrecaoDAO.Instance.AtualizaSituacao(carta.IdCarta, (uint)CartaCorrecao.SituacaoEnum.Recusada);
                                }
                                else
                                {
                                    CartaCorrecaoDAO.Instance.DeleteByPrimaryKey(carta.IdCarta);
                                }

                                break;
                            }
                    }

                    retorno = respostaProcessamento;
                }
                catch (Exception ex)
                {
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar xml de retorno.", ex));
                }

                #endregion
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(carta.IdNf, "Emissão", 1, $"Falha ao enviar lote. { ex.Message }");

                return Glass.MensagemAlerta.FormatErrorMsg("Falha ao enviar lote.", ex);
            }

            return retorno;
        }

        #endregion

        #region Envia pedido de cancelamento

        static volatile object _envioCancelamentoEvtLock = new object();

        /// <summary>
        /// Envia pedido de cancelamento da NFe para SEFAZ
        /// </summary>
        public static string EnviaCancelamentoEvt(uint idNf, string justificativa)
        {
            lock (_envioCancelamentoEvtLock)
            {
                NotaFiscal nf = null;
                NotaFiscal.SituacaoEnum? situacaoNota = null;
                Exception exception = null;
                var retorno = string.Empty;

                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var cancelarSeparacaoValores = false;

                        if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
                        {
                            try
                            {
                                // Verifica se o cancelamento de valores pode ser feito.
                                new SeparacaoValoresFiscaisEReaisContasReceber().ValidaCancelamento(transaction, idNf);
                                cancelarSeparacaoValores = true;
                            }
                            catch (Exception ex)
                            {
                                if (!ex.Message.Contains("Não existem parcelas a serem restauradas."))
                                    throw ex;
                            }
                        }

                        // Busca dados da Nota Fiscal
                        nf = NotaFiscalDAO.Instance.GetElement(transaction, idNf);

                        // Busca XML de cancelamento
                        XmlDocument xmlCanc = NotaFiscalDAO.Instance.CancelarNFeXmlEvt(transaction, justificativa, nf);

                        // Monta o lote
                        XmlDocument xmlLote = CriaLoteCancelamento(xmlCanc);

                        #region Envia XML

                        // Salva o callback padrão do WebService
                        System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

                        // Instancia xml de retorno
                        XmlNode xmlRetorno = null;

                        // Realiza 3 tentativas de envio, se não funcionar, gera mensagem de erro
                        for (int i = 0; i < 3; i++)
                        {
                            try
                            {
                                // Altera o callback de validação do WebService
                                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
                                {
                                    // Verifica se a data do certificado é válida
                                    DateTime beginDate = DateTime.Parse(cert.GetEffectiveDateString());
                                    DateTime endDate = DateTime.Parse(cert.GetExpirationDateString());
                                    bool isDateValid = (DateTime.Now >= beginDate) && (DateTime.Now <= endDate);

                                    // Retorna o resultado da função
                                    return isDateValid;
                                };

                                // Envia o arquivo e recebe o retorno
                                if (nf.Consumidor)
                                {
                                    xmlRetorno = ObterXmlRecepcaoEventoNFCe(nf, xmlLote);
                                }
                                else
                                {
                                    xmlRetorno = ObterXmlRecepcaoEventoNFe(nf, xmlLote);
                                }

                                break;
                            }
                            catch (Exception ex)
                            {
                                if (i == 2)
                                {
                                    LogNfDAO.Instance.NewLog(nf.IdNf, "Cancelamento", 1, "Falha ao cancelar NFe. " + ex.Message);

                                    NotaFiscalDAO.Instance.AlteraSituacao(transaction, nf.IdNf, NotaFiscal.SituacaoEnum.FalhaCancelar);

                                    throw new Exception(ex.Message);
                                }
                            }
                            finally
                            {
                                // Restaura o callback padrão para o WebService
                                System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
                            }
                        }

                        #endregion

                        // Realiza procedimentos de cancelamento de NFe
                        retorno = NotaFiscalDAO.Instance.RetornoEvtCancelamentoNFe(transaction, nf.IdNf, justificativa, xmlRetorno, cancelarSeparacaoValores);

                        #region Salva XML de cancelamento com retorno

                        try
                        {
                            if (NotaFiscalDAO.Instance.ObtemSituacao(transaction, nf.IdNf) == (int)NotaFiscal.SituacaoEnum.Cancelada)
                            {
                                var fileName = $"{ Utils.GetNfeXmlPath }110111{ nf.ChaveAcesso }-can.xml";

                                XmlDocument xmlDoc = new XmlDocument();
                                XmlNode declarationNode = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                                xmlDoc.AppendChild(declarationNode);

                                XmlElement procEventoNFe = xmlDoc.CreateElement("procEventoNFe");

                                procEventoNFe.SetAttribute("versao", "1.00");
                                procEventoNFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
                                xmlDoc.AppendChild(procEventoNFe);

                                // Insere o xml de cancelamento no documento xml
                                procEventoNFe.AppendChild(procEventoNFe.OwnerDocument.ImportNode(xmlCanc.DocumentElement, true));

                                // Insere o resultado do cancelamento no documento xml
                                procEventoNFe.AppendChild(procEventoNFe.OwnerDocument.ImportNode(xmlRetorno?["retEvento"] ?? xmlRetorno, true));

                                if (File.Exists(fileName))
                                {
                                    File.Delete(fileName);
                                }

                                xmlDoc.Save(fileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErroDAO.Instance.InserirFromException("CancelamentoNFe", ex);
                        }

                        #endregion

                        situacaoNota = (NotaFiscal.SituacaoEnum)NotaFiscalDAO.Instance.ObtemSituacao(transaction, nf.IdNf);

                        if (situacaoNota != NotaFiscal.SituacaoEnum.Cancelada && situacaoNota != NotaFiscal.SituacaoEnum.ProcessoCancelamento)
                            transaction.Rollback();
                        else
                        {
                            LogNfDAO.Instance.NewLog(idNf, "Cancelamento", 1, string.Format("Funcionário cancelamento: {0}", UserInfo.GetUserInfo.Nome));
                            transaction.Commit();
                            transaction.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        LogNfDAO.Instance.NewLog(idNf, "Cancelamento", 1, $"Falha ao cancelar NFe. { ex.Message } { ex?.InnerException?.Message ?? string.Empty }");

                        exception = ex;
                    }
                }

                if (exception != null)
                {
                    NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaCancelar);
                    throw exception;
                }

                if (situacaoNota != null && situacaoNota != NotaFiscal.SituacaoEnum.Cancelada && situacaoNota != NotaFiscal.SituacaoEnum.ProcessoCancelamento)
                {
                    NotaFiscalDAO.Instance.AlteraSituacao(idNf, situacaoNota.Value);
                }

                var idsPedidoNf = NotaFiscalDAO.Instance.GetIdsPedidoNotaFiscal(null, idNf);
                if (idsPedidoNf.Any())
                {
                    CarregamentoDAO.Instance.AlterarSituacaoFaturamentoCarregamentos(null, idsPedidoNf);
                }

                return retorno;
            }
        }

        #endregion

        #region Envia pedido de inutilização

        /// <summary>
        /// Envia pedido de inutilização de NFe para SEFAZ
        /// </summary>
        public static string EnviaInutilizacao(uint idNf, string justificativa)
        {
            try
            {
                FilaOperacoes.NotaFiscalInutilizar.AguardarVez();

                #region Permite o envio do pedido de inutilização mais de uma vez somente em um intervalo maior que 2 minutos

                if (!NotaFiscalDAO.Instance.PodeEnviarPedidoInutilizacao(null, idNf))
                    return "Inutilização em andamento.";

                #endregion

                // Busca dados da Nota Fiscal
                NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(idNf);

                // Busca XML de inutilizalção
                XmlDocument xmlInut = NotaFiscalDAO.Instance.InutilizarNFeXml(idNf, justificativa);

                #region Comunica com webservice

                // Salva o callback padrão do WebService
                System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

                // Instancia xml de retorno
                XmlNode xmlRetorno = null;

                // Realiza 3 tentativas de envio, se não funcionar, gera mensagem de erro
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        // Altera o callback de validação do WebService
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
                        {
                            // Verifica se a data do certificado é válida
                            DateTime beginDate = DateTime.Parse(cert.GetEffectiveDateString());
                            DateTime endDate = DateTime.Parse(cert.GetExpirationDateString());
                            bool isDateValid = (DateTime.Now >= beginDate) && (DateTime.Now <= endDate);

                            // Retorna o resultado da função
                            return isDateValid;
                        };

                        // Envia o arquivo e recebe o retorno.
                        if (nf.Consumidor)
                        {
                            xmlRetorno = ObterXmlInutilizacaoNFCe(nf, xmlInut);
                        }
                        else
                        {
                            xmlRetorno = ObterXmlInutilizacaoNFe(nf, xmlInut);
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 2)
                        {
                            LogNfDAO.Instance.NewLog(nf.IdNf, "Inutilização", 1, "Falha ao inutilizar numeração da NFe. " + ConsultaSituacao.CustomizaMensagemRejeicao(nf.IdNf, ex.Message));

                            NotaFiscalDAO.Instance.AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.FalhaInutilizar);

                            throw new Exception(ex.Message);
                        }
                    }
                    finally
                    {
                        // Restaura o callback padrão para o WebService
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
                    }
                }

                #endregion

                #region Lê Xml de retorno do envio da inutilização)

                // Realiza procedimentos de inutilização da NFe
                NotaFiscalDAO.Instance.RetornoInutilizacaoNFe(nf.IdNf, justificativa, xmlRetorno);

                var codStatus = xmlRetorno?["infInut"]?["cStat"]?.InnerXml;

                if (codStatus == "102")
                {
                    NotaFiscalDAO.Instance.SalvarRetornoXmlInutilizacao(idNf, xmlInut.ChildNodes[1], xmlRetorno.ChildNodes[0]);
                    return "Inutilização efetuada.";
                }
                else
                {
                    return "Falha ao inutilizar numeração da NFe. " + ConsultaSituacao.CustomizaMensagemRejeicao(nf.IdNf, xmlRetorno?["infInut"]?["xMotivo"]?.InnerXml);
                }

                #endregion
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(idNf, "Inutilização", 1, $"Falha ao inutilizar numeração da NFe. { ConsultaSituacao.CustomizaMensagemRejeicao(idNf, ex.Message) }");

                if (ex.Message != "A numeração desta nota já foi inutilizada.")
                {
                    NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaInutilizar);
                }

                throw ex;
            }
            finally
            {
                FilaOperacoes.NotaFiscalInutilizar.ProximoFila();
            }
        }



        #endregion

        #region Obtém XML de resultado dos eventos da NFe e NFCe

        public static XmlNode ObterXmlConsultaCadastroContribuinte(string uf, XmlDocument xmlConsultaCadastro)
        {
            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                switch (uf)
                {
                    case "AC":
                    case "MG":
                    case "GO":
                    case "PB":
                    case "PE":
                    case "RS":
                    case "SC":
                    case "SP": return GetWebService.ConsultaCadastroProducao(uf).consultaCadastro2(xmlConsultaCadastro);
                    case "BA":
                    case "CE":
                    case "PR":
                    case "MS":
                    case "MT": return GetWebService.ConsultaCadastroProducao4(uf).consultaCadastro(xmlConsultaCadastro);

                    default: return null;
                }
            return GetWebService.ConsultaCadastroHomologacao(uf).consultaCadastro(xmlConsultaCadastro);
        }

        public static XmlNode ObterXmlAutorizacaoNFe(NotaFiscal notaFiscal, XmlDocument xmlAutorizacaoNFe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFeAutorizacaoProducao(notaFiscal, null, uf).nfeAutorizacaoLote(xmlAutorizacaoNFe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFeAutorizacaoHomologacao(notaFiscal, null, uf).nfeAutorizacaoLote(xmlAutorizacaoNFe);
            }

            return null;
        }

        public static XmlNode ObterXmlConsultaProtocoloNFe(NotaFiscal notaFiscal, XmlDocument xmlConsultaProtocoloNFe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFeConsultaProtocoloProducao(notaFiscal, null, uf).nfeConsultaNF(xmlConsultaProtocoloNFe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFeConsultaProtocoloHomologacao(notaFiscal, null, uf).nfeConsultaNF(xmlConsultaProtocoloNFe);
            }

            return null;
        }

        public static XmlNode ObterXmlInutilizacaoNFe(NotaFiscal notaFiscal, XmlDocument xmlInutilizacaoNFe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFeInutilizacaoProducao(notaFiscal, null, uf).nfeInutilizacaoNF(xmlInutilizacaoNFe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFeInutilizacaoHomologacao(notaFiscal, null, uf).nfeInutilizacaoNF(xmlInutilizacaoNFe);
            }

            return null;
        }

        public static XmlNode ObterXmlRecepcaoEventoNFe(NotaFiscal notaFiscal, XmlDocument xmlRecepcaoEventoNFe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFeRecepcaoEventoProducao(notaFiscal, null, uf).nfeRecepcaoEvento(xmlRecepcaoEventoNFe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFeRecepcaoEventoHomologacao(notaFiscal, null, uf).nfeRecepcaoEvento(xmlRecepcaoEventoNFe);
            }

            return null;
        }

        public static XmlNode ObterXmlRetornoAutorizacaoNFe(NotaFiscal notaFiscal, XmlDocument xmlRetornoAutorizacaoNFe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFeRetornoAutorizacaoProducao(notaFiscal, null, uf).nfeRetAutorizacaoLote(xmlRetornoAutorizacaoNFe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFeRetornoAutorizacaoHomologacao(notaFiscal, null, uf).nfeRetAutorizacaoLote(xmlRetornoAutorizacaoNFe);
            }

            return null;
        }

        public static XmlNode ObterXmlStatusServicoNFe(NotaFiscal notaFiscal, XmlDocument xmlStatusServicoNFe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFeStatusServicoProducao(notaFiscal, null, uf).nfeStatusServicoNF(xmlStatusServicoNFe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFeStatusServicoHomologacao(notaFiscal, null, uf).nfeStatusServicoNF(xmlStatusServicoNFe);
            }

            return null;
        }

        public static XmlNode ObterXmlAutorizacaoNFCe(NotaFiscal notaFiscal, XmlDocument xmlAutorizacaoNFCe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFCeAutorizacaoProducao(notaFiscal, null, uf).nfeAutorizacaoLote(xmlAutorizacaoNFCe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFCeAutorizacaoHomologacao(notaFiscal, null, uf).nfeAutorizacaoLote(xmlAutorizacaoNFCe);
            }

            return null;
        }

        public static XmlNode ObterXmlConsultaProtocoloNFCe(NotaFiscal notaFiscal, XmlDocument xmlConsultaProtocoloNFCe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFCeConsultaProtocoloProducao(notaFiscal, null, uf).nfeConsultaNF(xmlConsultaProtocoloNFCe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFCeConsultaProtocoloHomologacao(notaFiscal, null, uf).nfeConsultaNF(xmlConsultaProtocoloNFCe);
            }

            return null;
        }

        public static XmlNode ObterXmlInutilizacaoNFCe(NotaFiscal notaFiscal, XmlDocument xmlInutilizacaoNFCe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFCeInutilizacaoProducao(notaFiscal, null, uf).nfeInutilizacaoNF(xmlInutilizacaoNFCe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFCeInutilizacaoHomologacao(notaFiscal, null, uf).nfeInutilizacaoNF(xmlInutilizacaoNFCe);
            }

            return null;
        }

        public static XmlNode ObterXmlRecepcaoEventoNFCe(NotaFiscal notaFiscal, XmlDocument xmlRecepcaoEventoNFCe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFCeRecepcaoEventoProducao(notaFiscal, null, uf).nfeRecepcaoEvento(xmlRecepcaoEventoNFCe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFCeRecepcaoEventoHomologacao(notaFiscal, null, uf).nfeRecepcaoEvento(xmlRecepcaoEventoNFCe);
            }

            return null;
        }

        public static XmlNode ObterXmlRetornoAutorizacaoNFCe(NotaFiscal notaFiscal, XmlDocument xmlRetornoAutorizacaoNFCe)
        {
            var uf = LojaDAO.Instance.GetUf(notaFiscal.IdLoja.Value)?.ToUpper() ?? string.Empty;

            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                return GetWebService.NFCeRetornoAutorizacaoProducao(notaFiscal, null, uf).nfeRetAutorizacaoLote(xmlRetornoAutorizacaoNFCe);
            }
            else if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                return GetWebService.NFCeRetornoAutorizacaoHomologacao(notaFiscal, null, uf).nfeRetAutorizacaoLote(xmlRetornoAutorizacaoNFCe);
            }

            return null;
        }

        #endregion
    }
}