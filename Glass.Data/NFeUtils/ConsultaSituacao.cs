using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Xml;

namespace Glass.Data.NFeUtils
{
    public class ConsultaSituacao
    {
        #region Consulta situação do Lote (método acionado pelo usuário)

        /// <summary>
        /// Consulta lote, função acionada pelo usuário
        /// </summary>
        /// <param name="idNf"></param>
        public static string ConsultaLote(uint idNf)
        {
            NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(idNf);

            if (nf.Consumidor)
                return ConsultaLoteNFCe(nf);

            return ConsultaLoteNFe(nf);
        }

        private static string ConsultaLoteNFe(NotaFiscal nf)
        {
            #region Monta XML de requisição de situação do lote

            if (String.IsNullOrEmpty(nf.NumRecibo))
                throw new Exception("A NFe não foi emitida. Não há número de recibo.");

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<consReciNFe xmlns=\"http://www.portalfiscal.inf.br/nfe\" " +
                "versao=\"" + ConfigNFe.VersaoRetAutorizacao + "\">" +
                "<tpAmb>" + (int)ConfigNFe.TipoAmbiente + "</tpAmb>" +
                "<nRec>" + nf.NumRecibo.PadLeft(15, '0') + "</nRec></consReciNFe>";

            XmlDocument xmlRetRecep = new XmlDocument();
            xmlRetRecep.LoadXml(strXml);

            try
            {
                ValidaXML.Validar(xmlRetRecep, ValidaXML.TipoArquivoXml.RetRecepcao);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

            // Guarda o status do lote
            string status = String.Empty;

            XmlNode xmlRetorno = null;

            // Salva o callback padrão do WebService
            System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

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

                if (nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaSVCRS && nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaSVCAN)
                {
                    if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                    {
                        switch (uf)
                        {
                            case "AM":
                                xmlRetorno = GetWebService.PAMRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                            case "CE":
                                xmlRetorno = GetWebService.PCERetornoAutorizao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                            case "MG":
                                {
                                    var dadosMsg = new wsPMGNFeRetornoAutorizacao.nfeDadosMsg();

                                    dadosMsg.Any = new XmlNode[] { xmlRetRecep };
                                    dadosMsg.Any[0] = xmlRetRecep.DocumentElement;
                                    var xmlDocument = new XmlDocument();
                                    var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "retConsSitNFe", "");

                                    var retorno = GetWebService.PMGRetornoAutorizacao(nf, null).nfeRetAutorizacao4(dadosMsg);

                                    foreach (var node in retorno[0] as XmlNode[])
                                        xmlNode.InnerXml += node.OuterXml;

                                    xmlRetorno = xmlNode;
                                    break;
                                }
                            case "MT":
                                xmlRetorno = GetWebService.PMTRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                            case "MS":
                                {
                                    var dadosMsg = new wsPMSNFeConsultaProtocolo.nfeResultMsg();

                                    dadosMsg.Any = new XmlNode[] { xmlRetRecep };
                                    dadosMsg.Any[0] = xmlRetRecep.DocumentElement;
                                    var xmlDocument = new XmlDocument();
                                    var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "retConsSitNFe", "");

                                    var retorno = GetWebService.PMSConsulta(nf, null).nfeConsultaNF(dadosMsg);

                                    // Verificar se retorno.Any está funcionando corretamente
                                    foreach (var node in retorno.Any as XmlNode[])
                                        xmlNode.InnerXml += node.OuterXml;

                                    xmlRetorno = xmlNode;
                                    break;
                                }
                                //xmlRetorno = GetWebService.PMSConsulta(nf, null).nfeConsultaNF(xmlRetRecep); break;
                            case "PE":
                                xmlRetorno = GetWebService.PPERetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                            case "BA":
                                xmlRetorno = GetWebService.PBARetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                            case "GO":
                                xmlRetorno = GetWebService.PGORetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                            case "PR":
                                xmlRetorno = GetWebService.PPRRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                            case "RS":
                                xmlRetorno = GetWebService.PRSRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                            case "SP":
                                xmlRetorno = GetWebService.PSPRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                            case "MA":
                            case "PA":
                                xmlRetorno = GetWebService.PSVANRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
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
                                xmlRetorno = GetWebService.PSVRSRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                        }
                    }
                }
                else if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaSVCRS)
                    xmlRetorno = GetWebService.SVCRSRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep);
                else if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaSVCAN)
                    xmlRetorno = GetWebService.SVCANRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep);

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao chamar WebService.", ex));
            }
            finally
            {
                // Restaura o callback padrão para o WebService
                System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
            }

            // Se o lote já tiver sido processado, sai do loop
            if (xmlRetorno != null) // Lote processado
                status = xmlRetorno["cStat"].InnerXml;

            // Verifica o status do lote
            if (status == "104") // Lote processado
            {
                XmlNodeList protNFeList = ((XmlElement)xmlRetorno).GetElementsByTagName("protNFe");

                // Para cada protocolo de autorização de uso (inicialmente será só um, pois cada nota está sendo enviada em um lote distinto)
                foreach (XmlNode protNFeNode in protNFeList)
                {
                    NotaFiscalDAO.Instance.RetornoEmissaoNFe(protNFeNode["infProt"]["chNFe"].InnerXml, protNFeNode);

                    string statusNFe = protNFeNode["infProt"]["cStat"].InnerXml;

                    if (statusNFe == "100" || statusNFe == "150") // Autorizada para uso
                        return "NFe está autorizada para uso.";
                    else
                        return "NFe rejeitada. Motivo: " + protNFeNode["infProt"]["xMotivo"].InnerXml;
                }

                return "Lote processado";
            }
            else if (status == "105") // Lote em processamento
            {
                NotaFiscalDAO.Instance.AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.ProcessoEmissao);

                return "Esta NFe ainda está sendo processada pela SEFAZ, aguarde para realizar uma nova consulta.";
            }
            else if (status == "106") // Lote não encontrado
            {
                NotaFiscalDAO.Instance.AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                LogNfDAO.Instance.NewLog(nf.IdNf, "Consulta", 106, "Falha na emissão da NFe. Não foi encontrado o lote de envio.");

                return "Falha na consulta. Não foi encontrado o lote de envio.";
            }
            else // Lote rejeitado
            {
                NotaFiscalDAO.Instance.AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                LogNfDAO.Instance.NewLog(nf.IdNf, "Consulta", Glass.Conversoes.StrParaInt(xmlRetorno["cStat"].InnerXml), xmlRetorno["xMotivo"].InnerXml);

                string msgErro = "Falha na consulta. ";

                if (status == "215" || status == "516" || status == "517" || status == "545")
                    msgErro += "Mensagem de consulta inválida. ";
                else if (status == "225" || status == "565" || status == "567" || status == "568")
                    msgErro += "Lote da NFe é inválido. ";

                return msgErro + xmlRetorno["cStat"].InnerXml + " - " + CustomizaMensagemRejeicao(nf.IdNf, xmlRetorno["xMotivo"].InnerXml);
            }
        }

        private static string ConsultaLoteNFCe(NotaFiscal nf)
        {
            #region Monta XML de requisição de situação do lote

            if (String.IsNullOrEmpty(nf.NumRecibo))
                throw new Exception("A NFc-e não foi emitida. Não há número de recibo.");

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<consReciNFe xmlns=\"http://www.portalfiscal.inf.br/nfe\" " +
                "versao=\"" + ConfigNFe.VersaoRetAutorizacao + "\">" +
                "<tpAmb>" + (int)ConfigNFe.TipoAmbiente + "</tpAmb>" +
                "<nRec>" + nf.NumRecibo.PadLeft(15, '0') + "</nRec></consReciNFe>";

            XmlDocument xmlRetRecep = new XmlDocument();
            xmlRetRecep.LoadXml(strXml);

            try
            {
                ValidaXML.Validar(xmlRetRecep, ValidaXML.TipoArquivoXml.RetRecepcao);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

            // Guarda o status do lote
            string status = String.Empty;

            XmlNode xmlRetorno = null;

            // Salva o callback padrão do WebService
            System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

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

                if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                {
                    switch (uf)
                    {
                        case "AM":
                            xmlRetorno = GetWebService.PAMNFCeRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                        case "MT":
                            xmlRetorno = GetWebService.PMTNFCeRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                        case "RS":
                            xmlRetorno = GetWebService.PRSNFCeRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                        case "AC":
                        case "BA":
                        case "DF":
                        case "RJ":
                        case "RN":
                        case "RO":
                        case "PA":
                        case "PB":
                            xmlRetorno = GetWebService.PSVRSNFCeRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                    }
                }
                else
                {
                    switch (uf)
                    {
                        case "AM":
                            xmlRetorno = GetWebService.HAMNFCeRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                        case "MT":
                            xmlRetorno = GetWebService.HMTNFCeRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                        case "RS":
                            xmlRetorno = GetWebService.HRSNFCeRetornoAutorizacao(nf, null).nfeRetAutorizacaoLote(xmlRetRecep); break;
                    }
                }
 
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao chamar WebService.", ex));
            }
            finally
            {
                // Restaura o callback padrão para o WebService
                System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
            }

            // Se o lote já tiver sido processado, sai do loop
            if (xmlRetorno != null) // Lote processado
                status = xmlRetorno["cStat"].InnerXml;

            // Verifica o status do lote
            if (status == "104") // Lote processado
            {
                XmlNodeList protNFeList = ((XmlElement)xmlRetorno).GetElementsByTagName("protNFe");

                // Para cada protocolo de autorização de uso (inicialmente será só um, pois cada nota está sendo enviada em um lote distinto)
                foreach (XmlNode protNFeNode in protNFeList)
                {
                    NotaFiscalDAO.Instance.RetornoEmissaoNFe(protNFeNode["infProt"]["chNFe"].InnerXml, protNFeNode);

                    string statusNFe = protNFeNode["infProt"]["cStat"].InnerXml;

                    if (statusNFe == "100" || statusNFe == "150") // Autorizada para uso
                        return "NFe está autorizada para uso.";
                    else
                        return "NFe rejeitada. Motivo: " + protNFeNode["infProt"]["xMotivo"].InnerXml;
                }

                return "Lote processado";
            }
            else if (status == "105") // Lote em processamento
            {
                NotaFiscalDAO.Instance.AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.ProcessoEmissao);

                return "Esta NFe ainda está sendo processada pela SEFAZ, aguarde para realizar uma nova consulta.";
            }
            else if (status == "106") // Lote não encontrado
            {
                NotaFiscalDAO.Instance.AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                LogNfDAO.Instance.NewLog(nf.IdNf, "Consulta", 106, "Falha na emissão da NFe. Não foi encontrado o lote de envio.");

                return "Falha na consulta. Não foi encontrado o lote de envio.";
            }
            else // Lote rejeitado
            {
                NotaFiscalDAO.Instance.AlteraSituacao(nf.IdNf, NotaFiscal.SituacaoEnum.FalhaEmitir);

                LogNfDAO.Instance.NewLog(nf.IdNf, "Consulta", Glass.Conversoes.StrParaInt(xmlRetorno["cStat"].InnerXml), xmlRetorno["xMotivo"].InnerXml);

                string msgErro = "Falha na consulta. ";

                if (status == "215" || status == "516" || status == "517" || status == "545")
                    msgErro += "Mensagem de consulta inválida. ";
                else if (status == "225" || status == "565" || status == "567" || status == "568")
                    msgErro += "Lote da NFe é inválido. ";

                return msgErro + xmlRetorno["cStat"].InnerXml + " - " + CustomizaMensagemRejeicao(nf.IdNf, xmlRetorno["xMotivo"].InnerXml);
            }
        }

        #endregion

        #region Consulta situação da NFe

        /// <summary>
        /// Consulta situação da NFe
        /// </summary>
        /// <param name="idNf"></param>
        public static string ConsultaSitNFe(uint idNf)
        {
            NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(idNf);

            if (nf.Consumidor)
                return ConsultaSitNFCe(nf);

            return ConsultaSitNFe(nf);
        }

        private static string ConsultaSitNFe(NotaFiscal nf)
        {
            #region Monta XML

            XmlDocument xmlConsSitNFe = new XmlDocument();
            XmlNode declarationNode = xmlConsSitNFe.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlConsSitNFe.AppendChild(declarationNode);

            XmlElement consSitNFe = xmlConsSitNFe.CreateElement("consSitNFe");
            consSitNFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
            consSitNFe.SetAttribute("versao", ConfigNFe.VersaoConsulta);
            xmlConsSitNFe.AppendChild(consSitNFe);

            ManipulacaoXml.SetNode(xmlConsSitNFe, consSitNFe, "tpAmb", ((int)ConfigNFe.TipoAmbiente).ToString());
            ManipulacaoXml.SetNode(xmlConsSitNFe, consSitNFe, "xServ", "CONSULTAR");
            ManipulacaoXml.SetNode(xmlConsSitNFe, consSitNFe, "chNFe", nf.ChaveAcesso);

            #endregion

            // Salva o callback padrão do WebService
            System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

            XmlNode xmlRetorno = null;

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

                if (nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaSVCRS && nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaSVCAN)
                {
                    if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                    {
                        switch (uf)
                        {
                            case "AM":
                                xmlRetorno = GetWebService.PAMConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                            case "CE":
                                xmlRetorno = GetWebService.PCEConsulta(nf, null).nfeConsultaNF2(xmlConsSitNFe); break;
                            case "MG":
                                {
                                    var dadosMsg = new wsPMGNFeConsulta.nfeDadosMsg();

                                    dadosMsg.Any = new XmlNode[] { xmlConsSitNFe };
                                    dadosMsg.Any[0] = xmlConsSitNFe.DocumentElement;
                                    var xmlDocument = new XmlDocument();
                                    var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "retConsSitNFe", "");

                                    var retorno = GetWebService.PMGConsulta(nf, null).nfeConsulta4(dadosMsg);

                                    foreach (var node in retorno[0] as XmlNode[])
                                        xmlNode.InnerXml += node.OuterXml;

                                    xmlRetorno = xmlNode;
                                    break;
                                }
                            case "MT":
                                xmlRetorno = GetWebService.PMTConsulta(nf, null).nfeConsultaNF2(xmlConsSitNFe); break;
                            case "MS":
                                {
                                    var dadosMsg = new wsPMSNFeConsultaProtocolo.nfeResultMsg();

                                    dadosMsg.Any = new XmlNode[] { xmlConsSitNFe };
                                    dadosMsg.Any[0] = xmlConsSitNFe.DocumentElement;
                                    var xmlDocument = new XmlDocument();
                                    var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "retConsSitNFe", "");

                                    var retorno = GetWebService.PMSConsulta(nf, null).nfeConsultaNF(dadosMsg);

                                    // Verificar se retorno.Any está funcionando corretamente
                                    foreach (var node in retorno.Any as XmlNode[])
                                        xmlNode.InnerXml += node.OuterXml;

                                    xmlRetorno = xmlNode;
                                    break;
                                }
                                //xmlRetorno = GetWebService.PMSConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                            case "PE":
                                xmlRetorno = GetWebService.PPEConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                            case "BA":
                                xmlRetorno = GetWebService.PBAConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                            case "GO":
                                xmlRetorno = GetWebService.PGOConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                            case "PR":
                                xmlRetorno = GetWebService.PPRConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                            case "RS":
                                xmlRetorno = GetWebService.PRSConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                            case "SP":
                                xmlRetorno = GetWebService.PSPConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                            case "MA":
                            case "PA":
                                xmlRetorno = GetWebService.PSVANConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
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
                                xmlRetorno = GetWebService.PSVRSConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                        }
                    }
                    else
                    {
                        switch (uf)
                        {
                            case "RS":
                                xmlRetorno = GetWebService.HRSConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                        }
                    }
                }
                else if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaSVCRS)
                {
                    if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                        xmlRetorno = GetWebService.PSVCRSConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe);
                    else
                    { }
                }
                else if (nf.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaSVCAN)
                    xmlRetorno = GetWebService.PSVCANConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe);

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao chamar WebService. Favor consultar a disponibilidade da receita", ex));
            }
            finally
            {
                // Restaura o callback padrão para o WebService
                System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
            }

            if (xmlRetorno == null)
                throw new Exception("Falha ao comunicar com webservice da SEFAZ. Favor consultar a disponibilidade da receita");

            string codStatus = xmlRetorno["cStat"].InnerXml;

            // Executa ações de acordo com o retorno dado
            NotaFiscalDAO.Instance.RetornoConsSitNFe(nf.IdNf, xmlRetorno);

            if (codStatus == "100" || codStatus == "150") // NFe Autorizada
                return "NFe está autorizada para uso.";
            else // NFe rejeitada
            {
                string msgErro = "Falha na consulta. ";

                if (codStatus == "215" || codStatus == "516" || codStatus == "517" || codStatus == "545")
                    msgErro += "Mensagem de consulta inválida. ";

                return msgErro + xmlRetorno["cStat"].InnerXml + " - " + CustomizaMensagemRejeicao(nf.IdNf, xmlRetorno["xMotivo"].InnerXml);
            }
        }

        private static string ConsultaSitNFCe(NotaFiscal nf)
        {
            #region Monta XML

            XmlDocument xmlConsSitNFe = new XmlDocument();
            XmlNode declarationNode = xmlConsSitNFe.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlConsSitNFe.AppendChild(declarationNode);

            XmlElement consSitNFe = xmlConsSitNFe.CreateElement("consSitNFe");
            consSitNFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
            consSitNFe.SetAttribute("versao", ConfigNFe.VersaoConsulta);
            xmlConsSitNFe.AppendChild(consSitNFe);

            ManipulacaoXml.SetNode(xmlConsSitNFe, consSitNFe, "tpAmb", ((int)ConfigNFe.TipoAmbiente).ToString());
            ManipulacaoXml.SetNode(xmlConsSitNFe, consSitNFe, "xServ", "CONSULTAR");
            ManipulacaoXml.SetNode(xmlConsSitNFe, consSitNFe, "chNFe", nf.ChaveAcesso);

            #endregion

            // Salva o callback padrão do WebService
            System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

            XmlNode xmlRetorno = null;

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


                if (ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
                {
                    switch (uf)
                    {
                        case "AM":
                            xmlRetorno = GetWebService.PAMNFCeConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                        case "MT":
                            xmlRetorno = GetWebService.PMTNFCeConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                        case "RS":
                            xmlRetorno = GetWebService.PRSNFCeConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                        case "AC":
                        case "BA":
                        case "DF":
                        case "RJ":
                        case "RN":
                        case "RO":
                        case "PA":
                        case "PB":
                            xmlRetorno = GetWebService.PSVRSNFCeConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                    }
                }
                else
                {
                    switch (uf)
                    {
                        case "AM":
                            xmlRetorno = GetWebService.HAMNFCeConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                        case "MT":
                            xmlRetorno = GetWebService.HMTNFCeConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                        case "RS":
                            xmlRetorno = GetWebService.HRSNFCeConsulta(nf, null).nfeConsultaNF(xmlConsSitNFe); break;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao chamar WebService.", ex));
            }
            finally
            {
                // Restaura o callback padrão para o WebService
                System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
            }

            if (xmlRetorno == null)
                throw new Exception("Falha ao comunicar com webservice da SEFAZ.");

            string codStatus = xmlRetorno["cStat"].InnerXml;

            // Executa ações de acordo com o retorno dado
            NotaFiscalDAO.Instance.RetornoConsSitNFe(nf.IdNf, xmlRetorno);

            if (codStatus == "100" || codStatus == "150") // NFe Autorizada
                return "NFe está autorizada para uso.";
            else // NFe rejeitada
            {
                string msgErro = "Falha na consulta. ";

                if (codStatus == "215" || codStatus == "516" || codStatus == "517" || codStatus == "545")
                    msgErro += "Mensagem de consulta inválida. ";

                return msgErro + xmlRetorno["cStat"].InnerXml + " - " + CustomizaMensagemRejeicao(nf.IdNf, xmlRetorno["xMotivo"].InnerXml);
            }
        }

        #endregion

        #region Consulta o cadastro de contribuintes do ICMS da unidade federada.

        /// <summary>
        /// Consulta o cadastro de contribuintes do ICMS da unidade federada.
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public static string ConsultaSitCadastroContribuinte(string UF, string CpfCnpj)
        {
            if (string.IsNullOrEmpty(UF) || string.IsNullOrEmpty(CpfCnpj))
                return "Contribuinte não encontrado.";

            #region Monta XML

            XmlDocument xmlConsCad = new XmlDocument();

            XmlNode declarationNode = xmlConsCad.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlConsCad.AppendChild(declarationNode);

            XmlElement consCad = xmlConsCad.CreateElement("ConsCad");
            consCad.SetAttribute("versao", ConfigNFe.VersaoConsCad);
            consCad.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
            xmlConsCad.AppendChild(consCad);
    
            XmlElement infCons = xmlConsCad.CreateElement("infCons");
            consCad.AppendChild(infCons);

            ManipulacaoXml.SetNode(xmlConsCad, infCons, "xServ", "CONS-CAD");
            ManipulacaoXml.SetNode(xmlConsCad, infCons, "UF", UF);

            CpfCnpj = Formatacoes.LimpaCpfCnpj(CpfCnpj);

            if (CpfCnpj.Length == 11)
                ManipulacaoXml.SetNode(xmlConsCad, infCons, "CPF", CpfCnpj);
            else
                ManipulacaoXml.SetNode(xmlConsCad, infCons, "CNPJ", CpfCnpj);

            #endregion

            // Salva o callback padrão do WebService
            System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

            XmlNode xmlRetorno = null;

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

                switch (UF)
                {
                    case "AM":
                        xmlRetorno = GetWebService.PAMConsultaCadastro().consultaCadastro2(xmlConsCad); break;
                    case "CE":
                        xmlRetorno = GetWebService.PCEConsultaCadastro().consultaCadastro2(xmlConsCad); break;
                    case "MT":
                        xmlRetorno = GetWebService.PMTConsultaCadastro().consultaCadastro(xmlConsCad); break;
                    case "BA":
                        xmlRetorno = GetWebService.PBAConsultaCadastro().consultaCadastro(xmlConsCad); break;
                    case "GO":
                        xmlRetorno = GetWebService.PGOConsultaCadastro().consultaCadastro(xmlConsCad); break;
                    case "PR":
                        xmlRetorno = GetWebService.PPRConsultaCadastro().consultaCadastro(xmlConsCad); break;
                    case "RS":
                        xmlRetorno = GetWebService.PRSConsultaCadastro().consultaCadastro(xmlConsCad); break;
                    case "SP":
                        xmlRetorno = GetWebService.PSPConsultaCadastro().consultaCadastro(xmlConsCad); break;
                    case "MS":
                        {
                            var dadosMsg = new wsPMSNFeConsultaCadastro.consultaCadastroResult();

                            dadosMsg.Any = new XmlNode[] { xmlConsCad };
                            dadosMsg.Any[0] = xmlConsCad.DocumentElement;
                            var xmlDocument = new XmlDocument();
                            var xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "retConsCad", "");

                            var retornoCad = GetWebService.PMSConsultaCadastro().consultaCadastro(dadosMsg);

                            // Verificar se retorno.Any está funcionando corretamente
                            foreach (var node in retornoCad.Any as XmlNode[])
                                xmlNode.InnerXml += node.OuterXml;

                            xmlRetorno = xmlNode;
                            break;
                        }
                        //xmlRetorno = GetWebService.PMSConsultaCadastro().consultaCadastro(xmlConsCad); break;
                    case "PE":
                        xmlRetorno = GetWebService.PPEConsultaCadastro().consultaCadastro(xmlConsCad); break;
                    case "MG":
                        xmlRetorno = GetWebService.PMGConsultaCadastro().consultaCadastro2(xmlConsCad); break;
                    }

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao chamar WebService.", ex));
            }
            finally
            {
                // Restaura o callback padrão para o WebService
                System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
            }

            if (xmlRetorno == null)
                return "Falha ao comunicar com webservice da SEFAZ.";

            XmlDocument xmlDocRetorno = new XmlDocument();
            xmlDocRetorno.ImportNode(xmlRetorno, true);

            XmlNamespaceManager nMgr = new XmlNamespaceManager(xmlDocRetorno.NameTable);
            nMgr.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");

            XmlNode infoCons = xmlRetorno.SelectSingleNode("//nfe:infCons",nMgr);
            string codStatus = infoCons["cStat"].InnerText;

            string retorno = "";
            if (codStatus == "111" || codStatus == "112")
            {
                retorno += "Consulta Situação do Contribuinte no Sintegra\n\n";
                retorno+="Situação: ";
                if (infoCons["infCad"]["cSit"].InnerText == "1")
                    retorno += "Habilitado.";
                else
                    retorno += "Não Habilitado.";
            }
            else
            {
                retorno += "Falha na Consulta Situação do Contribuinte no Sintegra\n\n";
                retorno += "Código: " + codStatus+"\n";
                retorno += infoCons["xMotivo"].InnerText;
            }

            ClienteDAO.Instance.AtualizaUltimaConsultaSintegra(CpfCnpj);

            return retorno;
        }

        /// <summary>
        /// Verifica se a consulta de cadastro esta disponivel
        /// para o cliente em questão
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public static bool HabilitadoConsultaCadastro(string UF)
        {
            if (string.IsNullOrEmpty(UF))
                return false;

            switch (UF)
            {
                case "AM":
                case "CE":
                case "MT":
                case "BA":
                case "GO":
                case "PR":
                case "RS":
                case "SP":
                case "MS":
                case "PE":
                case "MG":
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Customiza mensagem de rejeição

        /// <summary>
        /// Customiza mensagem de rejeição
        /// </summary>
        public static string CustomizaMensagemRejeicao(uint idNf, string motivoRejeicao)
        {
            return CustomizaMensagemRejeicao(null, idNf, motivoRejeicao);
        }

        /// <summary>
        /// Customiza mensagem de rejeição
        /// </summary>
        public static string CustomizaMensagemRejeicao(GDA.GDASession session, uint idNf, string motivoRejeicao)
        {
            bool isSimplesNacional = LojaDAO.Instance.ObtemValorCampo<int?>(session, "crt", "idLoja=" + NotaFiscalDAO.Instance.ObtemIdLoja(session, idNf)) < 3;

            if (motivoRejeicao.Contains("Total da BC ICMS difere do somatório dos itens"))
            {
                if (isSimplesNacional)
                    motivoRejeicao += " (Empresas optantes pelo simples nacional devem destacar ICMS apenas nas informações complementares)";
                else
                    motivoRejeicao += " (Um ou mais produtos desta nota fiscal possuem CST que não calcula ICMS, CST 60 por exemplo, apesar de estar sendo calculado ICMS na mesma)";
            }
            else if (motivoRejeicao.Contains("Total da BC ICMS-ST difere do somatório dos itens"))
            {
                if (isSimplesNacional)
                    motivoRejeicao += " (Um ou mais produtos desta nota fiscal possuem CSOSN que não devem calcular ICMS ST, apesar de estar sendo calculado na mesma)";
                else
                    motivoRejeicao += " (Um ou mais produtos desta nota fiscal possuem CST que não devem calcular ICMS ST, CST 00 por exemplo, apesar de estar sendo calculado na mesma)";
            }

            return motivoRejeicao;
        }

        #endregion
    }
}