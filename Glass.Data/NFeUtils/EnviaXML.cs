using System;
using System.Xml;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;
using System.IO;
using GDA;
using System.Linq;

namespace Glass.Data.NFeUtils
{
    public class EnviaXML
    {
        #region Cria lote

        /// <summary>
        /// Monta e retorna um Xml do lote para envio com a NF-e a ser enviada
        /// </summary>
        /// <param name="xmlNFe"></param>
        /// <returns></returns>
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
        /// <param name="xmlCce">Carta de correção</param>
        /// <param name="idNf">Identificador da nota fiscal</param>
        /// <returns></returns>
        public static XmlDocument CriaLoteCce(XmlDocument xmlCce)
        {
            XmlDocument xmlLote = new XmlDocument();
            XmlNode declarationNode = xmlLote.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlLote.AppendChild(declarationNode);

            string envEventoString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<envEvento xmlns=\"http://www.portalfiscal.inf.br/nfe\" versao=\"1.00\">" +
                "<idLote>" + ContadorRecepcaoEventoDAO.Instance.GetNext() +"</idLote>";

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
        /// <param name="xmlCancelamento"></param>
        /// <returns></returns>
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
        /// <param name="xmlNFe"></param>
        /// <param name="idNf"></param>
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
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
                        {
                            // Verifica se a data do certificado é válida
                            DateTime beginDate = DateTime.Parse(cert.GetEffectiveDateString());
                            DateTime endDate = DateTime.Parse(cert.GetExpirationDateString());
                            bool isDateValid = (DateTime.Now >= beginDate) && (DateTime.Now <= endDate);

                            // Retorna o resultado da função
                            return isDateValid;
                        };

                        #region Envia o arquivo e recebe o retorno

                        string uf = LojaDAO.Instance.GetElement(nf.IdLoja.Value).Uf.ToUpper();

                        #region NFC-e

                        if (nf.Consumidor)
                        {
                            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                            {
                                switch (uf)
                                {
                                    case "AM":
                                        xmlRetorno = GetWebService.PAMNFCeAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                    case "MT":
                                        xmlRetorno = GetWebService.PMTNFCeAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                    case "RS":
                                        xmlRetorno = GetWebService.PRSNFCeAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                    case "AC":
                                    case "BA":
                                    case "DF":
                                    case "PA":
                                    case "PB":
                                    case "RJ":
                                    case "RN":
                                    case "RO":
                                        xmlRetorno = GetWebService.PSVRSNFCeAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                }
                            }
                            else
                            {
                                switch (uf)
                                {
                                    case "AM":
                                        xmlRetorno = GetWebService.HAMNFCeAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                    case "MT":
                                        xmlRetorno = GetWebService.HMTNFCeAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                    case "RS":
                                        xmlRetorno = GetWebService.HRSNFCeAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                    case "AC":
                                    case "BA":
                                    case "DF":
                                    case "PA":
                                    case "PB":
                                    case "RJ":
                                    case "RN":
                                    case "RO":
                                        xmlRetorno = GetWebService.HSVRSNFCeAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                }
                            }
                        }

                        #endregion

                        #region NF-e

                        else
                        {
                            if (nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaSVCRS && nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaSVCAN)
                            {
                                if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                                {
                                    switch (uf)
                                    {
                                        case "AM":
                                            xmlRetorno = GetWebService.PAMAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "CE":
                                            xmlRetorno = GetWebService.PCEAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "MG":
                                            {
                                                var dadosMsg = new wsPMGNFeAutorizacao.nfeDadosMsg();

                                                dadosMsg.Any = new XmlNode[] { xmlLote };
                                                dadosMsg.Any[0] = xmlLote.DocumentElement;
                                                var xmlDocument = new XmlDocument();
                                                var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "retEnviNFe", "");

                                                var retorno = GetWebService.PMGAutorizacao(nf, null).NFeAutorizacao4Lote(dadosMsg);

                                                foreach (var node in retorno[0] as XmlNode[])
                                                    xmlNode.InnerXml += node.OuterXml;

                                                xmlRetorno = xmlNode;
                                                break;
                                            }
                                        case "MS":
                                            {
                                                var dadosMsg = new wsPMSNFeAutorizacao.nfeResultMsg();

                                                dadosMsg.Any = new XmlNode[] { xmlLote };
                                                dadosMsg.Any[0] = xmlLote.DocumentElement;
                                                var xmlDocument = new XmlDocument();
                                                var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "retEnviNFe", "");
                                                var retorno = GetWebService.PMSAutorizacao(nf, null).nfeAutorizacaoLote(dadosMsg).Any;

                                                foreach (var node in retorno)
                                                    xmlNode.InnerXml += node.OuterXml;

                                                xmlRetorno = xmlNode;
                                                break;
                                            }
                                        case "MT":
                                            xmlRetorno = GetWebService.PMTAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "PE":
                                            xmlRetorno = GetWebService.PPEAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "BA":
                                            xmlRetorno = GetWebService.PBAAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "GO":
                                            xmlRetorno = GetWebService.PGOAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "PR":
                                            xmlRetorno = GetWebService.PPRAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "RS":
                                            xmlRetorno = GetWebService.PRSAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "AC":
                                        case "AL":
                                        case "AP":
                                        case "DF":
                                        case "PB":
                                        case "PI":
                                        case "RJ":
                                        case "RN":
                                        case "RO":
                                        case "RR":
                                        case "SC":
                                        case "SE":
                                        case "TO":
                                        case "ES":
                                            xmlRetorno = GetWebService.PSVRSAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "MA":
                                        case "PA":
                                            xmlRetorno = GetWebService.PSVANAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "SP":
                                            {
                                                uint idCidade = LojaDAO.Instance.ObtemValorCampo<uint>("idCidade", "idLoja=" + nf.IdLoja.Value);

                                                try
                                                {
                                                    if (nf.IdLoja > 0)
                                                        LojaDAO.Instance.ExecuteScalar<int>("Update loja set idCidade=1630 Where idLoja=" + nf.IdLoja);

                                                    GetWebService.PMGAutorizacao(nf, null).NFeAutorizacao4Lote(null);
                                                }
                                                catch
                                                {
                                                    try
                                                    {
                                                        if (nf.IdLoja > 0)
                                                            LojaDAO.Instance.ExecuteScalar<int>("Update loja set idCidade=538 Where idLoja=" + nf.IdLoja);

                                                        GetWebService.PBAAutorizacao(nf, null).nfeAutorizacaoLote(null);
                                                    }
                                                    catch { }
                                                }

                                                if (nf.IdLoja > 0)
                                                    LojaDAO.Instance.ExecuteScalar<int>("Update loja set idCidade=" + idCidade + " Where idLoja=" + nf.IdLoja);

                                                xmlRetorno = GetWebService.PSPAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                            }
                                    }
                                }
                                else
                                {
                                    switch (uf)
                                    {
                                        case "RS":
                                            xmlRetorno = GetWebService.HRSAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                        case "AC":
                                        case "AL":
                                        case "AP":
                                        case "DF":
                                        case "PB":
                                        case "RJ":
                                        case "RN":
                                        case "RO":
                                        case "RR":
                                        case "SC":
                                        case "SE":
                                        case "TO":
                                        case "ES":
                                            xmlRetorno = GetWebService.HSVRSAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote); break;
                                    }
                                }
                            }
                            else if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaSVCRS)
                                xmlRetorno = GetWebService.SVCRSAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote);
                            else if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaSVCAN)
                                xmlRetorno = GetWebService.SVCANAutorizacao(nf, null).nfeAutorizacaoLote(xmlLote);
                        }

                        #endregion

                        #endregion

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 2)
                        {
                            LogNfDAO.Instance.NewLog(idNf, "Emissão", 1, "Falha ao enviar lote. " + ex.Message);
                            NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);
                            return "Falha ao enviar lote. " + ex.Message;
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
                var status = xmlRetorno["cStat"] != null && xmlRetorno["cStat"].InnerXml != null ? xmlRetorno["cStat"].InnerXml : "0";

                if (status == "103") // Lote recebido com sucesso
                {
                    LogNfDAO.Instance.NewLog(idNf, "Emissão", 103, "Lote enviado com sucesso. ");

                    // Pega o número do recibo do lote
                    string numReciboLote = xmlRetorno["infRec"]["nRec"].InnerXml;

                    // Salva na nota fiscal o número do recibo do lote
                    NotaFiscalDAO.Instance.RetornoEnvioLote(idNf, numReciboLote);

                    return "Lote enviado com sucesso.";
                }
                else if (status == "104")
                {
                    if (xmlRetorno["protNFe"] == null)
                    {
                        if (xmlRetorno["cStat"].InnerXml == "104")
                        {
                            string mensagem = "Lote processado.";

                            LogNfDAO.Instance.NewLog(idNf, "Emissão", 104, mensagem);

                            // Pega o número do recibo do lote
                            string numReciboLote = xmlRetorno["infRec"]["nRec"].InnerXml;

                            // Salva na nota fiscal o número do recibo do lote
                            NotaFiscalDAO.Instance.RetornoEnvioLote(idNf, numReciboLote);

                            return mensagem;
                        }
                        else
                        {
                            var motivo = xmlRetorno["xMotivo"].InnerXml;

                            LogNfDAO.Instance.NewLog(idNf, "Emissão",
                                Convert.ToInt32(xmlRetorno["cStat"].InnerXml),
                                motivo);

                            NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                            return motivo;
                        }
                    }
                    else if (xmlRetorno["protNFe"]["infProt"]["cStat"].InnerXml == "100")
                    {
                        LogNfDAO.Instance.NewLog(idNf, "Emissão", 104, "Lote Processado");
 
                        NotaFiscalDAO.Instance.RetornoEmissaoNFe(nf.ChaveAcesso, xmlRetorno["protNFe"]);
 
                        return xmlRetorno["protNFe"]["infProt"]["xMotivo"].InnerXml;
                    }
                    else
                    {
                        var codigo = xmlRetorno["protNFe"]["infProt"]["cStat"].InnerXml.StrParaInt();
                        var motivo = TrataMotivoRejeicaoNFe(codigo, xmlRetorno["protNFe"]["infProt"]["xMotivo"].InnerXml);

                        LogNfDAO.Instance.NewLog(idNf, "Emissão", codigo, motivo);

                        NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                        return motivo;
                    }
                }
                else if (Convert.ToInt32(status) > 200) // Lote foi rejeitado pela SEFAZ
                {
                    var codigo = status.StrParaInt();
                    var motivo = TrataMotivoRejeicaoNFe(codigo, xmlRetorno != null && xmlRetorno["xMotivo"] != null && xmlRetorno["xMotivo"].InnerXml != null ?
                        xmlRetorno["xMotivo"].InnerXml : string.Empty);

                    /* Chamado 36067. */
                    try
                    {
                        // Salva na tabela de erro os dados do XML de retorno da NF-e.
                        ErroDAO.Instance.InserirFromException(string.Format("RetornoEnvioNFe {0} - {1}", numeroNfe,
                            xmlRetorno != null && xmlRetorno.InnerXml != null ? xmlRetorno.InnerXml : "xmlRetorno nulo"), new Exception());

                        // Pega a UF retornada no XML.
                        var uf = xmlRetorno != null && xmlRetorno["cUF"] != null && xmlRetorno["cUF"].InnerXml != null ? xmlRetorno["cUF"].InnerXml : string.Empty;

                        if (!string.IsNullOrEmpty(uf))
                            motivo = string.Format("{0} UF: {1}", motivo, CidadeDAO.Instance.GetNomeUf(null, uf.StrParaUintNullable()));
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
                    LogNfDAO.Instance.NewLog(idNf, "Emissão", Convert.ToInt32(status),
                        xmlRetorno != null && xmlRetorno["xMotivo"] != null && xmlRetorno["xMotivo"].InnerXml != null ? xmlRetorno["xMotivo"].InnerXml : string.Empty);

                    /* Chamado 36067. */
                    try
                    {
                        // Salva na tabela de erro os dados do XML de retorno da NF-e.
                        ErroDAO.Instance.InserirFromException(string.Format("RetornoEnvioNFe {0} - {1}", numeroNfe,
                            xmlRetorno != null && xmlRetorno.InnerXml != null ? xmlRetorno.InnerXml : "xmlRetorno nulo"), new Exception());
                    }
                    catch (Exception ex)
                    {
                        ErroDAO.Instance.InserirFromException(string.Format("RetornoEnvioNFe {0}", numeroNfe), ex);
                    }

                    return xmlRetorno["xMotivo"].InnerXml;
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
                motivo += ". Possivelmente o CST ou CSOSN utilizado nos produtos da nota não permite o destaque do ICMS ou ICMS ST.";
            else if (codigo == 533)
                motivo += ". Possivelmente o CST ou CSOSN utilizado nos produtos da nota não permite o destaque do ICMS ST.";
            else if (codigo == 321)
                motivo += ". Possivelmente a nota fiscal referenciada não possui chave de acesso ou o CFOP utilizado na nota fiscal não é do tipo Devolução.";

            return motivo;
        }

        #endregion

        #region Envia pedido de autorização da CCe

        public static string EnviaCCe(XmlDocument xmlCce, uint idCarta)
        {
            string retorno = "";

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
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
                        {
                            // Verifica se a data do certificado é válida
                            DateTime beginDate = DateTime.Parse(cert.GetEffectiveDateString());
                            DateTime endDate = DateTime.Parse(cert.GetExpirationDateString());
                            bool isDateValid = (DateTime.Now >= beginDate) && (DateTime.Now <= endDate);

                            // Retorna o resultado da função
                            return isDateValid;
                        };

                        #region Envia o arquivo e recebe o retorno

                        string uf = LojaDAO.Instance.GetElement(nf.IdLoja.Value).Uf.ToUpper();

                        if (ConfigNFe.TipoAmbiente != ConfigNFe.TipoAmbienteNfe.Producao)
                            throw new Exception("A carta de correção está implementada somente para o ambiente de produção.");

                        if (FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.NaoUtilizar)
                        {
                            switch (uf)
                            {
                                case "AM":
                                    xmlRetorno = GetWebService.PAMRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                case "CE":
                                    xmlRetorno = GetWebService.PCERecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                case "MG":
                                    xmlRetorno = GetWebService.PMGRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                case "MT":
                                    xmlRetorno = GetWebService.PMTRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                //case "MS":
                                //    xmlRetorno = GetWebService.PMSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                case "PE":
                                    xmlRetorno = GetWebService.PPERecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                case "GO":
                                    xmlRetorno = GetWebService.PGORecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                case "PR":
                                    xmlRetorno = GetWebService.PPRRecepcaoEvento(nf, null).nfeRecepcaoEventoNF(xmlLote); break;
                                case "RS":
                                    xmlRetorno = GetWebService.PRSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                case "BA":
                                    xmlRetorno = GetWebService.PBARecepcaoEvento(nf, null).nfeRecepcaoEventoNF(xmlLote); break;
                                case "MA":
                                case "PA":
                                    xmlRetorno = GetWebService.PSVANRecepcaoEvento(nf, null).nfeRecepcaoEventoNF(xmlLote); break;
                                case "AC":
                                case "AL":
                                case "AP":
                                case "DF":
                                case "PB":
                                case "PI":
                                case "RJ":
                                case "RN":
                                case "RO":
                                case "RR":
                                case "SC":
                                case "SE":
                                case "TO":
                                case "ES":
                                    xmlRetorno = GetWebService.PSVRSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                case "SP":
                                    {
                                        uint idCidade = LojaDAO.Instance.ObtemValorCampo<uint>("idCidade", "idLoja=" + nf.IdLoja.Value);

                                        try
                                        {
                                            if (nf.IdLoja > 0)
                                                LojaDAO.Instance.ExecuteScalar<int>("Update loja set idCidade=1630 Where idLoja=" + nf.IdLoja);

                                            GetWebService.PMGAutorizacao(nf, null).NFeAutorizacao4Lote(null);
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                if (nf.IdLoja > 0)
                                                    LojaDAO.Instance.ExecuteScalar<int>("Update loja set idCidade=538 Where idLoja=" + nf.IdLoja);

                                                GetWebService.PBAAutorizacao(nf, null).nfeAutorizacaoLote(null);
                                            }
                                            catch { }
                                        }

                                        if (nf.IdLoja > 0)
                                            LojaDAO.Instance.ExecuteScalar<int>("Update loja set idCidade=" + idCidade + " Where idLoja=" + nf.IdLoja);

                                        xmlRetorno = GetWebService.PSPRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                    }
                            }
                        }
                        else if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaSVCRS)
                            xmlRetorno = GetWebService.PSVRSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                        else if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaSVCAN)
                            xmlRetorno = GetWebService.PSVCANRecepcaoEvento(nf, null).nfeRecepcaoEventoNF(xmlLote);

                        #endregion

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
                    string status = xmlRetorno["cStat"].InnerText;
                    string resposta = xmlRetorno["xMotivo"].InnerText;
                    int statusProcessamento = xmlRetorno["retEvento"] != null ? Glass.Conversoes.StrParaInt(xmlRetorno["retEvento"]["infEvento"]["cStat"].InnerText) : Glass.Conversoes.StrParaInt(status);
                    string respostaProcessamento = xmlRetorno["retEvento"] != null ? xmlRetorno["retEvento"]["infEvento"]["xMotivo"].InnerText : resposta;
                    
                    // Salva o retorno apenas se tiver sido aceito
                    if (statusProcessamento == 135 || statusProcessamento == 136)
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xmlRetorno.OuterXml);

                        string fileName = Utils.GetCartaCorrecaoXmlPath + idCarta.ToString().PadLeft(9, '0') + "-cce.xml";

                        if (System.IO.File.Exists(fileName))
                            System.IO.File.Delete(fileName);

                        doc.Save(fileName);
                    }

                    switch (statusProcessamento)
                    {
                        //Somente os casos 135 e 136 são aceitos
                        case 135:
                            LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", statusProcessamento, respostaProcessamento);
                            // Salva o protocolo
                            CartaCorrecaoDAO.Instance.SalvaProtocolo(carta.IdCarta, xmlRetorno["retEvento"]["infEvento"]["nProt"].InnerXml);
                            CartaCorrecaoDAO.Instance.AtualizaSituacao(carta.IdCarta, (uint)CartaCorrecao.SituacaoEnum.Registrada);
                            break;
                        case 136:
                            LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", statusProcessamento, respostaProcessamento);
                            // Salva o protocolo
                            CartaCorrecaoDAO.Instance.SalvaProtocolo(carta.IdCarta, xmlRetorno["retEvento"]["infEvento"]["nProt"].InnerXml);
                            CartaCorrecaoDAO.Instance.AtualizaSituacao(carta.IdCarta, (uint)CartaCorrecao.SituacaoEnum.Registrada);
                            break;
                        default:
                            LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", statusProcessamento, respostaProcessamento);

                            // Se a rejeição for por causa do código 594, exclui a carta, uma vez que para este caso é isso que o usuário deveria fazer.
                            if (statusProcessamento != 594)
                                CartaCorrecaoDAO.Instance.AtualizaSituacao(carta.IdCarta, (uint)CartaCorrecao.SituacaoEnum.Recusada);
                            else
                                CartaCorrecaoDAO.Instance.DeleteByPrimaryKey(carta.IdCarta);

                            break;
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
                LogNfDAO.Instance.NewLog(carta.IdNf, "Emissão", 1, "Falha ao enviar lote. " + ex.Message);

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
                            /* Chamado 43036. */
                            try
                            {
                                // Verifica se o cancelamento de valores pode ser feito.
                                SeparacaoValoresFiscaisEReaisContasReceber.Instance.ValidaCancelamento(transaction, idNf);
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

                                #region Envia o arquivo e recebe o retorno

                                string uf = LojaDAO.Instance.GetElement(transaction, nf.IdLoja.Value).Uf.ToUpper();

                                #region NFC-e

                                if (nf.Consumidor)
                                {

                                    if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                                    {
                                        switch (uf)
                                        {
                                            case "AM":
                                                xmlRetorno = GetWebService.PAMNFCRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                break;
                                            case "MT":
                                                xmlRetorno = GetWebService.PMTNFCRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                break;
                                            case "RS":
                                                xmlRetorno = GetWebService.PRSNFCRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                break;
                                            case "AC":
                                            case "BA":
                                            case "DF":
                                            case "PA":
                                            case "PB":
                                            case "RJ":
                                            case "RN":
                                            case "RO":
                                                xmlRetorno = GetWebService.PSVRSNFCRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (uf)
                                        {
                                            case "AM":
                                                xmlRetorno = GetWebService.HAMNFCRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                break;
                                            case "MT":
                                                xmlRetorno = GetWebService.HMTNFCRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                break;
                                            case "RS":
                                                xmlRetorno = GetWebService.HRSNFCRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                break;
                                            case "AC":
                                            case "BA":
                                            case "DF":
                                            case "PA":
                                            case "PB":
                                            case "RJ":
                                            case "RN":
                                            case "RO":
                                                xmlRetorno = GetWebService.HSVRSNFCRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                break;
                                        }
                                    }
                                }

                                #endregion

                                #region NF-e

                                //else da verificação se a nota é de consumidor (NFC-e)
                                else
                                {
                                    if (nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaSVCRS && nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaSVCAN)
                                    {
                                        if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                                        {
                                            switch (uf)
                                            {
                                                case "AM":
                                                    xmlRetorno = GetWebService.PAMRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                    break;
                                                case "CE":
                                                    xmlRetorno = GetWebService.PCERecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                    break;
                                                case "MG":
                                                    xmlRetorno = GetWebService.PMGRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                    break;
                                                case "MT":
                                                    xmlRetorno = GetWebService.PMTRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                    break;
                                                //case "MS":
                                                //    xmlRetorno = GetWebService.PMSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
                                                case "PE":
                                                    xmlRetorno = GetWebService.PPERecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                    break;
                                                case "BA":
                                                    xmlRetorno = GetWebService.PBARecepcaoEvento(nf, null).nfeRecepcaoEventoNF(xmlLote);
                                                    break;
                                                case "GO":
                                                    xmlRetorno = GetWebService.PGORecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                    break;
                                                case "PR":
                                                    xmlRetorno = GetWebService.PPRRecepcaoEvento(nf, null).nfeRecepcaoEventoNF(xmlLote);
                                                    break;
                                                case "RS":
                                                    xmlRetorno = GetWebService.PRSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                    break;
                                                case "SP":
                                                    xmlRetorno = GetWebService.PSPRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                    break;
                                                case "MA":
                                                case "PA":
                                                    xmlRetorno = GetWebService.PSVANRecepcaoEvento(nf, null).nfeRecepcaoEventoNF(xmlLote);
                                                    break;
                                                case "AC":
                                                case "AL":
                                                case "AP":
                                                case "DF":
                                                case "PB":
                                                case "PI":
                                                case "RJ":
                                                case "RN":
                                                case "RO":
                                                case "RR":
                                                case "SC":
                                                case "SE":
                                                case "TO":
                                                case "ES":
                                                    xmlRetorno = GetWebService.PSVRSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            switch (uf)
                                            {
                                                case "RS":
                                                    xmlRetorno = GetWebService.HRSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                                    break;
                                            }
                                        }
                                    }
                                    else if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaSVCRS)
                                        xmlRetorno = GetWebService.PSVCRSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);
                                    else if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaSVCAN)
                                        xmlRetorno = GetWebService.PSVCANRecepcaoEvento(nf, null).nfeRecepcaoEventoNF(xmlLote);
                                }

                                #endregion

                                #endregion

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
                                string fileName = Utils.GetNfeXmlPath + "110111" + nf.ChaveAcesso + "-can.xml";

                                XmlDocument xmlDoc = new XmlDocument();
                                XmlNode declarationNode = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                                xmlDoc.AppendChild(declarationNode);

                                XmlElement procEventoNFe = xmlDoc.CreateElement("procEventoNFe");

                                procEventoNFe.SetAttribute("versao", "1.00");
                                procEventoNFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
                                xmlDoc.AppendChild(procEventoNFe);

                                // Insere o xml de cancelamento no documento xml
                                procEventoNFe.AppendChild(
                                    procEventoNFe.OwnerDocument.ImportNode(
                                        xmlCanc.DocumentElement, true));

                                // Insere o resultado do cancelamento no documento xml
                                procEventoNFe.AppendChild(
                                    procEventoNFe.OwnerDocument.ImportNode(
                                    xmlRetorno["retEvento"] != null ? xmlRetorno["retEvento"] : xmlRetorno, true));

                                if (File.Exists(fileName))
                                    File.Delete(fileName);

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

                        LogNfDAO.Instance.NewLog(idNf, "Cancelamento", 1, "Falha ao cancelar NFe. " + ex.Message + " " +
                            (ex.InnerException != null ? ex.InnerException.Message : ""));

                        exception = ex;
                    }
                }

                if (exception != null)
                {
                    NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaCancelar);
                    throw exception;
                }

                if (situacaoNota != null &&
                    situacaoNota != NotaFiscal.SituacaoEnum.Cancelada && situacaoNota != NotaFiscal.SituacaoEnum.ProcessoCancelamento)
                    NotaFiscalDAO.Instance.AlteraSituacao(idNf, situacaoNota.Value);

                var idsPedidoNf = NotaFiscalDAO.Instance.GetIdsPedidoNotaFiscal(null, idNf);
                if (idsPedidoNf.Any())
                    CarregamentoDAO.Instance.AlterarSituacaoFaturamentoCarregamentos(null, idsPedidoNf);

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

                        #region Envia o arquivo e recebe o retorno

                        string uf = LojaDAO.Instance.GetElement(nf.IdLoja.Value).Uf.ToUpper();

                        #region NFC-e

                        if (nf.Consumidor)
                        {

                            if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                            {
                                switch (uf)
                                {
                                    case "AM":
                                        xmlRetorno = GetWebService.PAMNFCInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                    case "MT":
                                        xmlRetorno = GetWebService.PMTNFCInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                    case "RS":
                                        xmlRetorno = GetWebService.PRSNFCInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                    case "AC":
                                    case "BA":
                                    case "DF":
                                    case "PA":
                                    case "PB":
                                    case "RJ":
                                    case "RN":
                                    case "RO":
                                        xmlRetorno = GetWebService.PSVRSNFCInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                }
                            }
                            else
                            {
                                switch (uf)
                                {
                                    case "AM":
                                        xmlRetorno = GetWebService.HAMNFCInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                    case "MT":
                                        xmlRetorno = GetWebService.HMTNFCInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                    case "RS":
                                        xmlRetorno = GetWebService.HRSNFCInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                }
                            }
                        }

                        #endregion

                        #region NF-e

                        else
                        {
                            if (nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaComSCAN)
                            {
                                if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                                {
                                    switch (uf)
                                    {
                                        case "AM":
                                            xmlRetorno = GetWebService.PAMInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                        case "CE":
                                            xmlRetorno = GetWebService.PCEInutilizacao(nf, null).nfeInutilizacaoNF2(xmlInut); break;
                                        case "MG":
                                            {
                                                var dadosMsg = new wsPMGNFeInutilizacao.nfeDadosMsg();

                                                dadosMsg.Any = new XmlNode[] { xmlInut };
                                                dadosMsg.Any[0] = xmlInut.DocumentElement;
                                                var xmlDocument = new XmlDocument();
                                                var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "retConsSitNFe", "");

                                                var retorno = GetWebService.PMGInutilizacao(nf, null).nfeInutilizacao4(dadosMsg);

                                                foreach (var node in retorno[0] as XmlNode[])
                                                    xmlNode.InnerXml += node.OuterXml;

                                                xmlRetorno = xmlNode;
                                                break;
                                            }
                                        case "MT":
                                            xmlRetorno = GetWebService.PMTInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                        case "MS":
                                            {
                                                var dadosMsg = new wsPMSNFeInutilizacao.nfeResultMsg();

                                                dadosMsg.Any = new XmlNode[] { xmlInut };
                                                dadosMsg.Any[0] = xmlInut.DocumentElement;
                                                var xmlDocument = new XmlDocument();
                                                var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "retInutNFe", "");

                                                var retorno = GetWebService.PMSInutilizacao(nf, null).nfeInutilizacaoNF(dadosMsg);

                                                // Verificar se retorno.Any está funcionando corretamente
                                                foreach (var node in retorno.Any as XmlNode[])
                                                    xmlNode.InnerXml += node.OuterXml;

                                                xmlRetorno = xmlNode;
                                                break;
                                            }
                                            //xmlRetorno = GetWebService.PMSInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                        case "PE":
                                            xmlRetorno = GetWebService.PPEInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                        case "BA":
                                            xmlRetorno = GetWebService.PBAInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                        case "GO":
                                            xmlRetorno = GetWebService.PGOInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                        case "PR":
                                            xmlRetorno = GetWebService.PPRInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                        case "RS":
                                            xmlRetorno = GetWebService.PRSInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                        case "SP":
                                            xmlRetorno = GetWebService.PSPInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                        case "MA":
                                        case "PA":
                                            xmlRetorno = GetWebService.PSVANInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                        case "AC":
                                        case "AL":
                                        case "AP":
                                        case "DF":
                                        case "PB":
                                        case "PI":
                                        case "RJ":
                                        case "RN":
                                        case "RO":
                                        case "RR":
                                        case "SC":
                                        case "SE":
                                        case "TO":
                                        case "ES":
                                            xmlRetorno = GetWebService.PSVRSInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                    }
                                }
                                else
                                {
                                    switch (uf)
                                    {
                                        case "RS":
                                            xmlRetorno = GetWebService.HRSInutilizacao(nf, null).nfeInutilizacaoNF(xmlInut); break;
                                    }
                                }
                            }
                            else
                                xmlRetorno = GetWebService.SCANInutilizacao(nf, null).nfeInutilizacaoNF2(xmlInut);
                        }

                        #endregion

                        #endregion

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

                string codStatus = xmlRetorno["infInut"]["cStat"].InnerXml;

                if (codStatus == "102")
                    return "Inutilização efetuada.";
                else
                    return "Falha ao inutilizar numeração da NFe. " + ConsultaSituacao.CustomizaMensagemRejeicao(nf.IdNf, xmlRetorno["infInut"]["xMotivo"].InnerXml);

                #endregion
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(idNf, "Inutilização", 1, "Falha ao inutilizar numeração da NFe. " + ConsultaSituacao.CustomizaMensagemRejeicao(idNf, ex.Message));

                /* Chamado 48738. */
                if (ex.Message != "A numeração desta nota já foi inutilizada.")
                    NotaFiscalDAO.Instance.AlteraSituacao(idNf, NotaFiscal.SituacaoEnum.FalhaInutilizar);

                throw ex;
            }
            finally
            {
                FilaOperacoes.NotaFiscalInutilizar.ProximoFila();
            }
        }

        #endregion
    }
}