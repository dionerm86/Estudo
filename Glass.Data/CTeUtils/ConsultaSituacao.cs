using System;
using System.Xml;
using Glass.Data.DAL;
using Glass.Data.DAL.CTe;
using Glass.Data.Model.Cte;

namespace Glass.Data.CTeUtils
{
    public class ConsultaSituacao
    {
        #region Consulta situação do Lote (método acionado por thread)

        /// <summary>
        /// Consulta situação do lote 25 segundos após enviar o mesmo, função chamada por thread
        /// </summary>
        /// <param name="args">idCte|CertPath|CTeXmlPath</param>
        public static void ConsultaLoteThread(object args)
        {
            var vetParam = args.ToString().Split('|');

            Glass.Data.Model.Cte.ConhecimentoTransporte cte = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.GetElement(Convert.ToUInt32(vetParam[0]));
            Glass.Data.Model.Cte.ProtocoloCte protocolo = Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance.GetElement(Convert.ToUInt32(vetParam[0]), (int)Glass.Data.Model.Cte.ProtocoloCte.TipoProtocoloEnum.Autorizacao);
            try
            {
                #region Monta XML de requisição de situação do lote

                string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    "<consReciCTe xmlns=\"http://www.portalfiscal.inf.br/cte\" " +
                    "versao=\"" + ConfigCTe.VersaoRetRecepcao + "\">" +
                    "<tpAmb>" + (int)ConfigCTe.TipoAmbiente + "</tpAmb>" +
                    "<nRec>" + protocolo.NumRecibo.PadLeft(15, '0') + "</nRec></consReciCTe>";

                XmlDocument xmlRetRecep = new XmlDocument();
                xmlRetRecep.LoadXml(strXml);

                #endregion

                #region Comunica com webservice

                // Instancia xml de retorno
                XmlNode xmlRetorno = null;

                // Salva o status do lote
                string status = String.Empty;

                // Realiza 2 tentativas de envio, se não funcionar, deixa pro usuário consultar mais tarde
                for (int i = 0; i < 2; i++)
                {
                    try
                    {
                        // Espera 25 segundos para realizar a consulta do lote
                        Thread.Sleep(25000);

                        // Salva o callback padrão do WebService
                        System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

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

                            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);

                            string uf = LojaDAO.Instance.GetElement(participante.IdLoja.Value).Uf.ToUpper();

                            if (cte.TipoEmissao != (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.ContingenciaFsda)
                            {
                                if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Producao)
                                {
                                    switch (uf)
                                    {
                                        case "MT":
                                            xmlRetorno = GetWebService.PMTCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "MS":
                                            xmlRetorno = GetWebService.PMSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "MG":
                                            xmlRetorno = GetWebService.PMGCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "PR":
                                            xmlRetorno = GetWebService.PPRCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "RS":
                                            xmlRetorno = GetWebService.PRSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "SP":
                                            xmlRetorno = GetWebService.PSPCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "AP":
                                        case "PE":
                                        case "RR":
                                            xmlRetorno = GetWebService.PSVSPCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "AC":
                                        case "AL":
                                        case "AM":
                                        case "BA":
                                        case "CE":
                                        case "DF":
                                        case "ES":
                                        case "GO":
                                        case "MA":
                                        case "PA":
                                        case "PB":
                                        case "PI":
                                        case "RJ":
                                        case "RN":
                                        case "RO":
                                        case "SC":
                                        case "SE":
                                        case "TO":
                                            xmlRetorno = GetWebService.PSVRSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;                                        
                                    }
                                }
                                else
                                {
                                    switch (uf)
                                    {
                                        //case "MT":
                                        //    xmlRetorno = GetWebService.HMTCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "MS":
                                            xmlRetorno = GetWebService.HMSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "MG":
                                            xmlRetorno = GetWebService.HMGCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "PR":
                                            xmlRetorno = GetWebService.HPRCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "RS":
                                            xmlRetorno = GetWebService.HRSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "SP":
                                            xmlRetorno = GetWebService.HSPCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "AP":
                                        case "PE":
                                        case "RR":
                                            xmlRetorno = GetWebService.HSVSPCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                        case "AC":
                                        case "AL":
                                        case "AM":
                                        case "BA":
                                        case "CE":
                                        case "DF":
                                        case "ES":
                                        case "GO":
                                        case "MA":
                                        case "PA":
                                        case "PB":
                                        case "PI":
                                        case "RJ":
                                        case "RN":
                                        case "RO":
                                        case "SC":
                                        case "SE":
                                        case "TO":
                                            xmlRetorno = GetWebService.HSVRSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                                    }
                                }
                            }
                            //else
                            //    xmlRetorno = GetWebService.SCANRetornoRecepcao(cte, null).nfeRetRecepcao2(xmlRetRecep);

                            #endregion
                        }
                        finally
                        {
                            // Restaura o callback padrão para o WebService
                            System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
                        }

                        // Se o lote já tiver sido processado, sai do loop
                        if (xmlRetorno != null) // Lote processado
                            status = xmlRetorno["cStat"].InnerXml;

                        if (status == "104")
                            break;
                    }
                    catch { }
                }

                #endregion

                // Se o lote já tiver sido processado, lê mensagem de retorno da consulta à situação do lote
                if (status == "104")
                {
                    XmlNodeList protCTeList = ((XmlElement)xmlRetorno).GetElementsByTagName("protCTe");

                    // Para cada protocolo de autorização de uso (inicialmente será só um, pois cada nota está sendo enviada em um lote distinto)
                    foreach (XmlNode protCTeNode in protCTeList)
                        ConhecimentoTransporteDAO.Instance.RetornoEmissaoCte(cte.IdCte, protCTeNode, vetParam[2]);
                }
            }
            catch { }
        }

        #endregion

        #region Consulta situação do Lote (método acionado pelo usuário)

        /// <summary>
        /// Consulta lote, função acionada pelo usuário
        /// </summary>
        /// <param name="idNf"></param>
        public static string ConsultaLote(uint idCte)
        {
            ConhecimentoTransporte cte = ConhecimentoTransporteDAO.Instance.GetElement(idCte);
            ProtocoloCte protocolo = ProtocoloCteDAO.Instance.GetElement(idCte, (int)ProtocoloCte.TipoProtocoloEnum.Autorizacao);

            #region Monta XML de requisição de situação do lote

            if (protocolo == null || String.IsNullOrEmpty(protocolo.NumRecibo))
                throw new Exception("O CTe não foi emitido. Não há número de recibo.");

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<consReciCTe xmlns=\"http://www.portalfiscal.inf.br/cte\" " +
                "versao=\"" + ConfigCTe.VersaoRetRecepcao + "\">" +
                "<tpAmb>" + (int)ConfigCTe.TipoAmbiente + "</tpAmb>" +
                "<nRec>" + protocolo.NumRecibo.PadLeft(15, '0') + "</nRec></consReciCTe>";

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

                var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);

                string uf = LojaDAO.Instance.GetElement(participante.IdLoja.Value).Uf.ToUpper();

                if (cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.Normal)
                {
                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Producao)
                    {
                        switch (uf)
                        {
                            case "MT":
                                xmlRetorno = GetWebService.PMTCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "MS":
                                xmlRetorno = GetWebService.PMSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "MG":
                                xmlRetorno = GetWebService.PMGCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "PR":
                                xmlRetorno = GetWebService.PPRCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "RS":
                                xmlRetorno = GetWebService.PRSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "SP":
                                xmlRetorno = GetWebService.PSPCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "AP":
                            case "PE":
                            case "RR":
                                xmlRetorno = GetWebService.PSVSPCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "AC":
                            case "AL":
                            case "AM":
                            case "BA":
                            case "CE":
                            case "DF":
                            case "ES":
                            case "GO":
                            case "MA":
                            case "PA":
                            case "PB":
                            case "PI":
                            case "RJ":
                            case "RN":
                            case "RO":
                            case "SC":
                            case "SE":
                            case "TO":
                                xmlRetorno = GetWebService.PSVRSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;        
                        }
                    }
                    else
                    {
                        switch (uf)
                        {
                            //case "MT":
                            //    xmlRetorno = GetWebService.HMTCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "MS":
                                xmlRetorno = GetWebService.HMSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "MG":
                                xmlRetorno = GetWebService.HMGCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "PR":
                                xmlRetorno = GetWebService.HPRCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "RS":
                                xmlRetorno = GetWebService.HRSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "SP":
                                xmlRetorno = GetWebService.HSPCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "AP":
                            case "PE":
                            case "RR":
                                xmlRetorno = GetWebService.HSVSPCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "AC":
                            case "AL":
                            case "AM":
                            case "BA":
                            case "CE":
                            case "DF":
                            case "ES":
                            case "GO":
                            case "MA":
                            case "PA":
                            case "PB":
                            case "PI":
                            case "RJ":
                            case "RN":
                            case "RO":
                            case "SC":
                            case "SE":
                            case "TO":
                                xmlRetorno = GetWebService.HSVRSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                        }
                    }
                }
                else if (cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcRs ||
                                    cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcSp)
                {
                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Producao)
                    {
                        switch (uf)
                        {
                            case "AP":
                            case "MT":
                            case "MS":
                            case "PE":
                            case "RR":
                            case "SP":
                                xmlRetorno = GetWebService.PSVRSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "AC":
                            case "AL":
                            case "AM":
                            case "BA":
                            case "CE":
                            case "DF":
                            case "ES":
                            case "GO":
                            case "MA":
                            case "MG":
                            case "PA":
                            case "PB":
                            case "PI":
                            case "PR":
                            case "RJ":
                            case "RN":
                            case "RO":
                            case "RS":
                            case "SC":
                            case "SE":
                            case "TO":
                                xmlRetorno = GetWebService.PSVSPCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                        }
                    }
                    else
                    {
                        switch (uf)
                        {
                            case "AP":
                            case "MT":
                            case "MS":
                            case "PE":
                            case "RR":
                            case "SP":
                                xmlRetorno = GetWebService.HSVRSCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                            case "AC":
                            case "AL":
                            case "AM":
                            case "BA":
                            case "CE":
                            case "DF":
                            case "ES":
                            case "GO":
                            case "MA":
                            case "MG":
                            case "PA":
                            case "PB":
                            case "PI":
                            case "PR":
                            case "RJ":
                            case "RN":
                            case "RO":
                            case "RS":
                            case "SC":
                            case "SE":
                            case "TO":
                                xmlRetorno = GetWebService.HSVSPCTeRetRecepcao(cte, null).cteRetRecepcao(xmlRetRecep); break;
                        }
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
                XmlNodeList protCTeList = ((XmlElement)xmlRetorno).GetElementsByTagName("protCTe");

                // Para cada protocolo de autorização de uso (inicialmente será só um, pois cada nota está sendo enviada em um lote distinto)
                foreach (XmlNode protCTeNode in protCTeList)
                {
                    ConhecimentoTransporteDAO.Instance.RetornoEmissaoCte(idCte, protCTeNode);

                    string statusCTe = protCTeNode["infProt"]["cStat"].InnerXml;

                    if (statusCTe == "100" || statusCTe == "150") // Autorizada para uso
                    {
                        Glass.Data.DAL.CTe.LogCteDAO.Instance.NewLog(idCte, "Consulta", 106, "CTe está autorizada para uso.");
                        return "CTe está autorizado para uso.";                        
                    }
                    else
                    {
                        Glass.Data.DAL.CTe.LogCteDAO.Instance.NewLog(idCte, "Consulta", 106, protCTeNode["infProt"]["xMotivo"].InnerXml);
                        return "CTe rejeitada. Motivo: " + protCTeNode["infProt"]["xMotivo"].InnerXml;
                    }
                }

                return "Lote processado";
            }
            else if (status == "105") // Lote em processamento
            {
                ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoEmissao);

                return "Este CTe ainda está sendo processado pela SEFAZ, aguarde para realizar uma nova consulta.";
            }
            else if (status == "106") // Lote não encontrado
            {
                ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir);

                Glass.Data.DAL.CTe.LogCteDAO.Instance.NewLog(idCte, "Consulta", 106, "Falha na emissão do CTe. Não foi encontrado o lote de envio.");

                return "Falha na consulta. Não foi encontrado o lote de envio.";
            }
            else // Lote rejeitado
            {
                ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir);

                Glass.Data.DAL.CTe.LogCteDAO.Instance.NewLog(idCte, "Consulta", Glass.Conversoes.StrParaInt(xmlRetorno["cStat"].InnerXml), xmlRetorno["xMotivo"].InnerXml);

                string msgErro = "Falha na consulta. ";

                if (status == "215" || status == "516" || status == "517" || status == "545")
                    msgErro += "Mensagem de consulta inválida. ";
                else if (status == "225" || status == "565" || status == "567" || status == "568")
                    msgErro += "Lote do CTe é inválido. ";

                return msgErro + xmlRetorno["cStat"].InnerXml + " - " + CustomizaMensagemRejeicao(idCte, xmlRetorno["xMotivo"].InnerXml);
            }
        }

        #endregion

        #region Consulta situação do CTe

        /// <summary>
        /// Consulta situação do CTe
        /// </summary>
        /// <param name="idNf"></param>
        public static string ConsultaSitCTe(uint idCte)
        {
            ConhecimentoTransporte cte = ConhecimentoTransporteDAO.Instance.GetElement(idCte);

            #region Monta XML

            XmlDocument xmlConsSitCTe = new XmlDocument();
            XmlNode declarationNode = xmlConsSitCTe.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlConsSitCTe.AppendChild(declarationNode);

            XmlElement consSitCTe = xmlConsSitCTe.CreateElement("consSitCTe");
            consSitCTe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/cte");
            consSitCTe.SetAttribute("versao", ConfigCTe.VersaoConsulta);
            xmlConsSitCTe.AppendChild(consSitCTe);

            ManipulacaoXml.SetNode(xmlConsSitCTe, consSitCTe, "tpAmb", ((int)ConfigCTe.TipoAmbiente).ToString());
            ManipulacaoXml.SetNode(xmlConsSitCTe, consSitCTe, "xServ", "CONSULTAR");
            ManipulacaoXml.SetNode(xmlConsSitCTe, consSitCTe, "chCTe", cte.ChaveAcesso);

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

                var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);

                string uf = LojaDAO.Instance.GetElement(participante.IdLoja.Value).Uf.ToUpper();

                if (cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.Normal)
                {
                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Producao)
                    {
                        switch (uf)
                        {
                            case "MT":
                                xmlRetorno = GetWebService.PMTCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "MS":
                                xmlRetorno = GetWebService.PMSCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "MG":
                                xmlRetorno = GetWebService.PMGCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "PR":
                                xmlRetorno = GetWebService.PPRCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "RS":
                                xmlRetorno = GetWebService.PRSCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "SP":
                                xmlRetorno = GetWebService.PSPCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "AP":
                            case "PE":
                            case "RR":
                                xmlRetorno = GetWebService.PSVSPCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "AC":
                            case "AL":
                            case "AM":
                            case "BA":
                            case "CE":
                            case "DF":
                            case "ES":
                            case "GO":
                            case "MA":
                            case "PA":
                            case "PB":
                            case "PI":
                            case "RJ":
                            case "RN":
                            case "RO":
                            case "SC":
                            case "SE":
                            case "TO":
                                xmlRetorno = GetWebService.PSVRSCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                        }
                    }
                    else
                    {
                        switch (uf)
                        {
                            //case "MT":
                            //    xmlRetorno = GetWebService.HMTCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "MS":
                                xmlRetorno = GetWebService.HMSCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "MG":
                                xmlRetorno = GetWebService.HMGCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "PR":
                                xmlRetorno = GetWebService.HPRCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "RS":
                                xmlRetorno = GetWebService.HRSCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "SP":
                                xmlRetorno = GetWebService.HSPCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "AP":
                            case "PE":
                            case "RR":
                                xmlRetorno = GetWebService.HSVSPCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "AC":
                            case "AL":
                            case "AM":
                            case "BA":
                            case "CE":
                            case "DF":
                            case "ES":
                            case "GO":
                            case "MA":
                            case "PA":
                            case "PB":
                            case "PI":
                            case "RJ":
                            case "RN":
                            case "RO":
                            case "SC":
                            case "SE":
                            case "TO":
                                xmlRetorno = GetWebService.HSVRSCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                        }
                    }
                }
                else if (cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcRs ||
                                    cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcSp)
                {
                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Producao)
                    {
                        switch (uf)
                        {
                            case "AP":
                            case "MT":
                            case "MS":
                            case "PE":
                            case "RR":
                            case "SP":
                                xmlRetorno = GetWebService.PSVRSCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "AC":
                            case "AL":
                            case "AM":
                            case "BA":
                            case "CE":
                            case "DF":
                            case "ES":
                            case "GO":
                            case "MA":
                            case "MG":
                            case "PA":
                            case "PB":
                            case "PI":
                            case "PR":
                            case "RJ":
                            case "RN":
                            case "RO":
                            case "RS":
                            case "SC":
                            case "SE":
                            case "TO":
                                xmlRetorno = GetWebService.PSVSPCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                        }
                    }
                    else
                    {
                        switch (uf)
                        {
                            case "AP":
                            case "MT":
                            case "MS":
                            case "PE":
                            case "RR":
                            case "SP":
                                xmlRetorno = GetWebService.HSVRSCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                            case "AC":
                            case "AL":
                            case "AM":
                            case "BA":
                            case "CE":
                            case "DF":
                            case "ES":
                            case "GO":
                            case "MA":
                            case "MG":
                            case "PA":
                            case "PB":
                            case "PI":
                            case "PR":
                            case "RJ":
                            case "RN":
                            case "RO":
                            case "RS":
                            case "SC":
                            case "SE":
                            case "TO":
                                xmlRetorno = GetWebService.HSVSPCTeConsulta(cte, null).cteConsultaCT(xmlConsSitCTe); break;
                        }
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
            ConhecimentoTransporteDAO.Instance.RetornoConsSitCTe(idCte, xmlRetorno);

            if (codStatus == "100" || codStatus == "150") // CTe Autorizado
                return "CTe está autorizada para uso.";
            else // CTe rejeitado
            {
                string msgErro = "Falha na consulta. ";

                if (codStatus == "215" || codStatus == "516" || codStatus == "517" || codStatus == "545")
                    msgErro += "Mensagem de consulta inválida. ";

                return msgErro + xmlRetorno["cStat"].InnerXml + " - " + CustomizaMensagemRejeicao(idCte, xmlRetorno["xMotivo"].InnerXml);
            }
        }

        #endregion        

        #region Customiza mensagem de rejeição

        /// <summary>
        /// Customiza mensagem de rejeição
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="motivoRejeicao"></param>
        /// <returns></returns>
        public static string CustomizaMensagemRejeicao(uint idCte, string motivoRejeicao)
        {
            //bool isSimplesNacional = LojaDAO.Instance.ObtemValorCampo<int?>("crt", "idLoja=" + NotaFiscalDAO.Instance.ObtemIdLoja(idCte)) < 3;

            //if (motivoRejeicao.Contains("Total da BC ICMS difere do somatório dos itens"))
            //{
            //    if (isSimplesNacional)
            //        motivoRejeicao += " (Empresas optantes pelo simples nacional devem destacar ICMS apenas nas informações complementares)";
            //    else
            //        motivoRejeicao += " (Um ou mais produtos desta nota fiscal possuem CST que não calcula ICMS, CST 60 por exemplo, apesar de estar sendo calculado ICMS na mesma)";
            //}
            //else if (motivoRejeicao.Contains("Total da BC ICMS-ST difere do somatório dos itens"))
            //{
            //    if (isSimplesNacional)
            //        motivoRejeicao += " (Um ou mais produtos desta nota fiscal possuem CSOSN que não devem calcular ICMS ST, apesar de estar sendo calculado na mesma)";
            //    else
            //        motivoRejeicao += " (Um ou mais produtos desta nota fiscal possuem CST que não devem calcular ICMS ST, CST 00 por exemplo, apesar de estar sendo calculado na mesma)";
            //}

            return string.Empty;
        }

        #endregion
    }
}
