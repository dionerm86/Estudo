using System;
using System.Xml;
using Glass.Data.DAL;
using Glass.Data.DAL.CTe;
using Glass.Data.Model.Cte;

namespace Glass.Data.CTeUtils
{
    public class EnviaXML
    {
        #region Cria lote

        /// <summary>
        /// Monta e retorna um Xml do lote para envio com o CT-e a ser enviada
        /// </summary>
        /// <param name="xmlCTe"></param>
        /// <returns></returns>
        private static XmlDocument CriaLote(XmlDocument xmlCTe, uint idCte)
        {
            XmlDocument xmlLote = new XmlDocument();
            XmlNode declarationNode = xmlLote.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlLote.AppendChild(declarationNode);

            string enviCTeString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<enviCTe xmlns=\"http://www.portalfiscal.inf.br/cte\" versao=\"" + ConfigCTe.VersaoLoteCte + "\">" +
                "<idLote>" + ConhecimentoTransporteDAO.Instance.GetNewNumLote(idCte).ToString("000000000000000") + "</idLote>";

            // Insere o XML da CTe no lote
            int nPosI = xmlCTe.InnerXml.IndexOf("<CTe");
            int nPosF = xmlCTe.InnerXml.Length - nPosI;
            enviCTeString += xmlCTe.InnerXml.Substring(nPosI, nPosF) + "</enviCTe>";

            XmlDocument xmlRetorno = new XmlDocument();
            xmlRetorno.LoadXml(enviCTeString);

            #region Valida XML

            try
            {
                ValidaXML.Validar(xmlRetorno, ValidaXML.TipoArquivoXml.EnviCTe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            return xmlRetorno;
        }

        ///// <summary>
        ///// Monta e retorna um Xml do lote para envio de carta de correção
        ///// </summary>
        ///// <param name="xmlCce">Carta de correção</param>
        ///// <param name="idNf">Identificador da nota fiscal</param>
        ///// <returns></returns>
        //public static XmlDocument CriaLoteCce(XmlDocument xmlCce)
        //{
        //    XmlDocument xmlLote = new XmlDocument();
        //    XmlNode declarationNode = xmlLote.CreateXmlDeclaration("1.0", "UTF-8", null);
        //    xmlLote.AppendChild(declarationNode);

        //    string envEventoString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
        //        "<envEvento xmlns=\"http://www.portalfiscal.inf.br/nfe\" versao=\"1.00\">" +
        //        "<idLote>" + ContadorRecepcaoEventoDAO.Instance.GetNext() + "</idLote>";

        //    // Insere o XML da CCe no lote
        //    int nPosI = xmlCce.InnerXml.IndexOf("<evento");
        //    int nPosF = xmlCce.InnerXml.Length - nPosI;
        //    envEventoString += xmlCce.InnerXml.Substring(nPosI, nPosF) + "</envEvento>";

        //    XmlDocument xmlRetorno = new XmlDocument();
        //    xmlRetorno.LoadXml(envEventoString);

        //    #region Valida XML

        //    try
        //    {
        //        ValidaXML.Validar(xmlRetorno, ValidaXML.TipoArquivoXml.EnviCce);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("XML inconsistente." + ex.Message);
        //    }

        //    #endregion

        //    return xmlRetorno;
        //}

        /// <summary>
        /// Monta e retorna um Xml do lote para envio de cancelamento do CT-e
        /// </summary>
        /// <param name="xmlCancelamento"></param>
        /// <returns></returns>
        public static XmlDocument CriaLoteCancelamento(XmlDocument xmlCancelamento)
        {
            XmlDocument xmlLote = new XmlDocument();
            XmlNode declarationNode = xmlLote.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlLote.AppendChild(declarationNode);

            string envEventoString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<envEvento xmlns=\"http://www.portalfiscal.inf.br/cte\" versao=\"1.00\">" +
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
                ValidaXML.Validar(xmlRetorno, ValidaXML.TipoArquivoXml.EnviEvtCancCTe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            return xmlRetorno;
        }

        #endregion

        #region Envia pedido de autorização de CTe

        /// <summary>
        /// Envia a CTe para a SEFAZ via Webservice
        /// </summary>
        /// <param name="xmlNFe"></param>
        /// <param name="idCTe"></param>
        public static void EnviaCTe(XmlDocument xmlCTe, uint idCte)
        {
            try
            {
                // Monta o lote
                XmlDocument xmlLote = CriaLote(xmlCTe, idCte);

                // Busca dados da Note Fiscal
                ConhecimentoTransporte cte = ConhecimentoTransporteDAO.Instance.GetElement(idCte);

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

                        var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);

                        string uf = LojaDAO.Instance.GetElement(participante.IdLoja.Value).Uf.ToUpper();

                        if (cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.Normal)
                        {
                            if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Producao)
                            {
                                switch (uf)
                                {
                                    case "MT":
                                        xmlRetorno = GetWebService.PMTCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "MS":
                                        xmlRetorno = GetWebService.PMSCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "MG":
                                        xmlRetorno = GetWebService.PMGCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "PR":
                                        xmlRetorno = GetWebService.PPRCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "RS":
                                        xmlRetorno = GetWebService.PRSCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "SP":
                                        xmlRetorno = GetWebService.PSPCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "AP":
                                    case "PE":
                                    case "RR":                                    
                                        xmlRetorno = GetWebService.PSVSPCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
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
                                        xmlRetorno = GetWebService.PSVRSCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                }
                            }
                            else
                            {
                                switch (uf)
                                {
                                    //case "MT":
                                        //xmlRetorno = GetWebService.hMTCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "MS":
                                        xmlRetorno = GetWebService.HMSCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "MG":
                                        xmlRetorno = GetWebService.HMGCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "PR":
                                        xmlRetorno = GetWebService.HPRCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "RS":
                                        xmlRetorno = GetWebService.HRSCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "SP":
                                        xmlRetorno = GetWebService.HSPCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                    case "AP":
                                    case "PE":
                                    case "RR":
                                        xmlRetorno = GetWebService.HSVSPCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
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
                                        xmlRetorno = GetWebService.HSVRSCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
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
                                        xmlRetorno = GetWebService.PSVRSCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
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
                                        xmlRetorno = GetWebService.PSVSPCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
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
                                        xmlRetorno = GetWebService.HSVRSCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
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
                                        xmlRetorno = GetWebService.HSVSPCTeRecepcao(cte, null).cteRecepcaoLote(xmlLote); break;
                                }
                            }
                        }

                        #endregion

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 3)
                        {
                            Glass.Data.DAL.CTe.LogCteDAO.Instance.NewLog(idCte, "Emissão", 1, "Falha ao enviar lote. " + ex.Message);
                            ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir);
                            return;
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
                    Glass.Data.DAL.CTe.LogCteDAO.Instance.NewLog(idCte, "Emissão", 2, "Falha ao enviar lote. Retorno de envio do lote inválido.");

                    ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir);

                    return;
                }

                // Lê Xml de retorno do envio do lote
                string status = xmlRetorno["cStat"].InnerXml;

                if (status == "103") // Lote recebido com sucesso
                {
                    Glass.Data.DAL.CTe.LogCteDAO.Instance.NewLog(idCte, "Emissão", 103, "Lote enviado com sucesso. ");

                    // Pega o número do recibo do lote
                    string numReciboLote = xmlRetorno["infRec"]["nRec"].InnerXml;

                    // Salva na nota fiscal o número do recibo do lote
                    ConhecimentoTransporteDAO.Instance.RetornoEnvioLote(idCte, numReciboLote);
                }
                else if (Convert.ToInt32(status) > 200) // Lote foi rejeitado pela SEFAZ
                {
                    Glass.Data.DAL.CTe.LogCteDAO.Instance.NewLog(idCte, "Emissão", Convert.ToInt32(status), xmlRetorno["xMotivo"].InnerXml);

                    ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir);
                }
                else
                {
                    LogCteDAO.Instance.NewLog(idCte, "Emissão", Convert.ToInt32(status), xmlRetorno["xMotivo"].InnerXml);
                }

                #endregion
            }
            catch (Exception ex)
            {
                LogCteDAO.Instance.NewLog(idCte, "Emissão", 1, Glass.MensagemAlerta.FormatErrorMsg("Falha ao enviar lote.", ex));

                ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir);
            }
        }

        #endregion

        #region Envia pedido de autorização da CCe

//        public static string EnviaCCe(XmlDocument xmlCce, uint idCarta)
//        {
//            #region Códigos de status

//            Dictionary<int, string> codigos = new Dictionary<int, string>();
//            codigos.Add(128, "Lote de Evento Processado");
//            codigos.Add(135, "Evento registrado e vinculado a NF-e ");
//            codigos.Add(136, "Evento registrado, mas não vinculado a NF-e");
//            codigos.Add(213, "Rejeição: CNPJ-Base do Emitente difere do CNPJ-Base do Certificado Digital");
//            codigos.Add(280, @"Rejeição: Certificado de Transmissor Inválido: 
//                            - Certificado de Transmissor inexistente na mensagem 
//                            - Versão difere '3'
//                            - Se informado o Basic Constraint deve ser true (não pode ser Certificado 
//                            de AC) 
//                            - KeyUsage não define 'Autenticação Cliente'");
//            codigos.Add(281, "Rejeição: Validade do Certificado (data início e data fim)");
//            codigos.Add(283, @"Rejeição: Verifica a Cadeia de Certificação: 
//                            - Certificado da AC emissora não cadastrado na SEFAZ 
//                            - Certificado de AC revogado 
//                            - Certificado não assinado pela AC emissora do Certificado");
//            codigos.Add(286, @"Rejeição: LCR do Certificado de Transmissor 
//                            - Falta o endereço da LCR (CRL DistributionPoint) 
//                            - LCR indisponível 
//                            - LCR inválida");
//            codigos.Add(284, "Rejeição: Certificado do Transmissor revogado");
//            codigos.Add(285, "Rejeição: Certificado Raiz difere da ICP-Brasil");
//            codigos.Add(282, "Rejeição: Falta a extensão de CNPJ no Certificado (OtherName - OID=2.16.76.1.3.3)");
//            codigos.Add(214, "Rejeição: Tamanho do XML de Dados superior a 500 KB");
//            codigos.Add(108, "Rejeição: Verifica se o Servidor de Processamento está Paralisado Momentaneamente");
//            codigos.Add(109, "Rejeição: Verifica se o Servidor de Processamento está Paralisado sem Previsão");
//            codigos.Add(489, "Rejeição: CNPJ informado inválido (DV ou zeros) ");
//            codigos.Add(490, "Rejeição: CPF informado inválido (DV ou zeros)");
//            codigos.Add(491, "Rejeição: O tpEvento informado inválido ");
//            codigos.Add(492, "Rejeição: O verEvento informado inválido ");
//            codigos.Add(493, "Rejeição: Evento não atende o Schema XML específico");
//            codigos.Add(494, "Rejeição: Chave de Acesso inexistente");
//            codigos.Add(501, "Rejeição: NF-e autorizada há mais de 30 dias (720 horas)");
//            codigos.Add(572, "Rejeição: Erro Atributo ID do evento não corresponde a concatenação dos campos (ID + tpEvento + chNFe + nSeqEvento)");
//            codigos.Add(573, "Rejeição: Duplicidade de Evento ");
//            codigos.Add(574, "Rejeição: O autor do evento diverge do emissor da NF-e");
//            codigos.Add(575, "Rejeição: O autor do evento diverge do destinatário da NF-e");
//            codigos.Add(576, "Rejeição: O autor do evento não é um órgão autorizado a gerar o evento");
//            codigos.Add(577, "Rejeição: A data do evento não pode ser menor que a data de emissão da NF-e");
//            codigos.Add(578, "Rejeição: A data do evento não pode ser maior que a data do processamento");
//            codigos.Add(579, "Rejeição: A data do evento não pode ser menor que a data de autorização para NF-e não emitida em contingência");
//            codigos.Add(580, "Rejeição: O evento exige uma NF-e autorizada");
//            codigos.Add(587, "Rejeição: Usar somente o namespace padrão da NF-e");
//            codigos.Add(588, "Rejeição: Não é permitida a presença de caracteres de edição no início/fim da mensagem ou entre as tags da mensagem");
//            codigos.Add(594, "Rejeição: O número de seqüencia do evento informado é maior que o permitido");
//            codigos.Add(242, "Rejeição: Elemento nfeCabecMsg inexistente no SOAP Header");
//            codigos.Add(409, "Rejeição: Campo cUF inexistente no elemento nfeCabecMsg do SOAP Header");
//            codigos.Add(410, "Rejeição: Verificar se a UF informada no campo cUF é atendida pelo Web Service");
//            codigos.Add(411, "Rejeição: Campo versaoDados inexistente no elemento nfeCabecMsg do SOAP Header ");
//            codigos.Add(238, "Rejeição: Versão dos Dados informada é superior à versão vigente");
//            codigos.Add(239, "Rejeição: Versão dos Dados não suportada");
//            codigos.Add(225, "Rejeição: Verifica Schema XML da Área de Dados");
//            codigos.Add(516, "Rejeição: Em caso de Falha de Schema, verificar se existe a tag raiz esperada para o lote");
//            codigos.Add(517, "Rejeição: Em caso de Falha de Schema, verificar se existe o atributo versao para a tag raiz da mensagem ");
//            codigos.Add(545, "Rejeição: Em caso de Falha de Schema, verificar se o conteúdo do atributo versao difere do conteúdo da versao Dados informado no SOAPHeader ");
//            codigos.Add(404, "Rejeição: Verifica o uso de prefixo no namespace");
//            codigos.Add(402, "Rejeição: XML utiliza codificação diferente de UTF-8");
//            codigos.Add(290, @"Rejeição: Certificado de Assinatura inválido: 
//                            - Certificado de Assinatura inexistente na mensagem (*validado também pelo Schema) 
//                            - Versão difere '3' 
//                            - Se informado o Basic Constraint deve ser true (não pode ser Certificado de AC) 
//                            - KeyUsage não define 'Assinatura Digital' e 'Não Recusa'");
//            codigos.Add(291, "Rejeição: Validade do Certificado (data início e data fim)");
//            codigos.Add(292, "Rejeição: Falta a extensão de CNPJ no Certificado (OtherName - OID=2.16.76.1.3.3)");
//            codigos.Add(293, @"Rejeição: Verifica Cadeia de Certificação: 
//                            - Certificado da AC emissora não cadastrado na SEFAZ 
//                            - Certificado de AC revogado 
//                            - Certificado não assinado pela AC emissora do Certificado");
//            codigos.Add(296, @"Rejeição: LCR do Certificado de Assinatura: 
//                            - Falta o endereço da LCR (CRLDistributionPoint) 
//                            - Erro no acesso a LCR ou LCR inexistente");
//            codigos.Add(294, "Rejeição: Certificado de Assinatura revogado ");
//            codigos.Add(295, "Rejeição: Certificado Raiz difere da 'ICP-Brasil'");
//            codigos.Add(298, @"Rejeição: Assinatura difere do padrão do Projeto: 
//                            - Não assinado o atributo 'ID' (falta 'Reference URI' na assinatura) (*validado também pelo Schema) 
//                            - Faltam os 'Transform Algorithm' previstos na assinatura ('C14N' e 'Enveloped') Estas validações são implementadas pelo Schema XML da Signature ");
//            codigos.Add(297, "Rejeição: Valor da assinatura (SignatureValue) difere do valor calculado");
//            codigos.Add(252, "Rejeição: Tipo do ambiente difere do ambiente do Web Service ");
//            codigos.Add(250, "Rejeição: Código do órgão de recepção do Evento da UF diverge da solicitada");


//            #endregion

//            string retorno = "";

//            CartaCorrecao carta = CartaCorrecaoDAO.Instance.GetElement(idCarta);
//            NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(carta.IdNf);

//            try
//            {
//                // Monta o lote
//                XmlDocument xmlLote = CriaLoteCce(xmlCce);

//                #region Envia XML

//                // Instancia xml de retorno
//                XmlNode xmlRetorno = null;

//                // Salva o callback padrão do WebService
//                System.Net.Security.RemoteCertificateValidationCallback callback = System.Net.ServicePointManager.ServerCertificateValidationCallback;

//                // Realiza 3 tentativas de envio, se não funcionar, gera mensagem de erro
//                for (int i = 0; i < 3; i++)
//                {
//                    try
//                    {
//                        // Altera o callback de validação do WebService
//                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate cert, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
//                        {
//                            // Verifica se a data do certificado é válida
//                            DateTime beginDate = DateTime.Parse(cert.GetEffectiveDateString());
//                            DateTime endDate = DateTime.Parse(cert.GetExpirationDateString());
//                            bool isDateValid = (DateTime.Now >= beginDate) && (DateTime.Now <= endDate);

//                            // Retorna o resultado da função
//                            return isDateValid;
//                        };

//                        #region Envia o arquivo e recebe o retorno

//                        string uf = LojaDAO.Instance.GetElement(nf.IdLoja.Value).Uf.ToUpper();

//                        if (nf.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaComSCAN && ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
//                        {
//                            switch (uf)
//                            {
//                                case "AM":
//                                    xmlRetorno = GetWebService.PAMRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "CE":
//                                    xmlRetorno = GetWebService.PCERecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "MG":
//                                    xmlRetorno = GetWebService.PMGRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "MT":
//                                    xmlRetorno = GetWebService.PMTRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "MS":
//                                    xmlRetorno = GetWebService.PMSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "PE":
//                                    xmlRetorno = GetWebService.PPERecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "GO":
//                                    xmlRetorno = GetWebService.PGORecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "PR":
//                                    xmlRetorno = GetWebService.PPRRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "RS":
//                                    xmlRetorno = GetWebService.PRSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "BA":
//                                    xmlRetorno = GetWebService.PBARecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "AC":
//                                case "AL":
//                                case "AP":
//                                case "DF":
//                                case "PB":
//                                case "RJ":
//                                case "RN":
//                                case "RO":
//                                case "RR":
//                                case "SC":
//                                case "SE":
//                                case "TO":
//                                    xmlRetorno = GetWebService.PSVRSRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                case "SP":
//                                    {
//                                        uint idCidade = LojaDAO.Instance.ObtemValorCampo<uint>("idCidade", "idLoja=" + nf.IdLoja.Value);

//                                        try
//                                        {
//                                            if (nf.IdLoja > 0)
//                                                LojaDAO.Instance.ExecuteScalar<int>("Update loja set idCidade=1630 Where idLoja=" + nf.IdLoja);

//                                            GetWebService.PMGRecepcao(nf, null).nfeRecepcaoLote2(null);
//                                        }
//                                        catch
//                                        {
//                                            try
//                                            {
//                                                if (nf.IdLoja > 0)
//                                                    LojaDAO.Instance.ExecuteScalar<int>("Update loja set idCidade=538 Where idLoja=" + nf.IdLoja);

//                                                GetWebService.PBARecepcao(nf, null).nfeRecepcaoLote2(null);
//                                            }
//                                            catch { }
//                                        }

//                                        if (nf.IdLoja > 0)
//                                            LojaDAO.Instance.ExecuteScalar<int>("Update loja set idCidade=" + idCidade + " Where idLoja=" + nf.IdLoja);

//                                        xmlRetorno = GetWebService.PSPRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                                    }
//                                case "ES":
//                                case "MA":
//                                case "PA":
//                                case "PI":
//                                    xmlRetorno = GetWebService.PSVANRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote); break;
//                            }
//                        }
//                        else
//                            xmlRetorno = GetWebService.HSCANRecepcaoEvento(nf, null).nfeRecepcaoEvento(xmlLote);

//                        #endregion

//                        break;
//                    }
//                    catch (Exception ex)
//                    {
//                        if (i == 3)
//                        {
//                            LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", 1, Glass.MensagemAlerta.FormatErrorMsg("Falha ao enviar carta de correção.", ex));
//                            return "Falha ao enviar carta de correção. " + ex.Message;
//                        }
//                    }
//                    finally
//                    {
//                        // Restaura o callback padrão para o WebService
//                        System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
//                    }
//                }

//                #endregion

//                #region Lê Xml de retorno do envio do lote

//                try
//                {
//                    // Verifica se o Xml de Retorno é válido
//                    if (xmlRetorno == null)
//                    {
//                        LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", 2, "Falha ao enviar carta de correção. Retorno de envio de carta de correção inválido.");

//                        return "Falha ao enviar carta de correção. Retorno de envio de carta de correção inválido.";
//                    }

//                    // Lê Xml de retorno do envio do lote
//                    string status = xmlRetorno["cStat"].InnerText;
//                    string resposta = xmlRetorno["xMotivo"].InnerText;
//                    int statusProcessamento = Glass.Conversoes.StrParaInt(xmlRetorno["retEvento"]["infEvento"]["cStat"].InnerText);
//                    string respostaProcessamento = xmlRetorno["retEvento"]["infEvento"]["xMotivo"].InnerText;

//                    if (codigos.ContainsKey(statusProcessamento))
//                    {
//                        // Salva o retorno apenas se tiver sido aceito
//                        if (statusProcessamento == 135 || statusProcessamento == 136)
//                        {
//                            XmlDocument doc = new XmlDocument();
//                            doc.LoadXml(xmlRetorno.OuterXml);

//                            string fileName = Utils.GetCartaCorrecaoXmlPath + idCarta.ToString().PadLeft(9, '0') + "-cce.xml";

//                            if (System.IO.File.Exists(fileName))
//                                System.IO.File.Delete(fileName);

//                            doc.Save(fileName);
//                        }

//                        switch (statusProcessamento)
//                        {
//                            //Somente os casos 135 e 136 são aceitos
//                            case 135:
//                                LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", statusProcessamento, codigos[statusProcessamento]);
//                                // Salva o protocolo
//                                CartaCorrecaoDAO.Instance.SalvaProtocolo(carta.IdCarta, xmlRetorno["retEvento"]["infEvento"]["nProt"].InnerXml);
//                                CartaCorrecaoDAO.Instance.AtualizaSituacao(carta.IdCarta, (uint)CartaCorrecao.SituacaoEnum.Registrada);
//                                break;
//                            case 136:
//                                LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", statusProcessamento, codigos[statusProcessamento]);
//                                // Salva o protocolo
//                                CartaCorrecaoDAO.Instance.SalvaProtocolo(carta.IdCarta, xmlRetorno["retEvento"]["infEvento"]["nProt"].InnerXml);
//                                CartaCorrecaoDAO.Instance.AtualizaSituacao(carta.IdCarta, (uint)CartaCorrecao.SituacaoEnum.Registrada);
//                                break;
//                            default:
//                                LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", statusProcessamento, codigos[statusProcessamento]);
//                                CartaCorrecaoDAO.Instance.AtualizaSituacao(carta.IdCarta, (uint)CartaCorrecao.SituacaoEnum.Recusada);
//                                break;
//                        }

//                        retorno = codigos[statusProcessamento];
//                    }
//                    else
//                    {
//                        LogNfDAO.Instance.NewLog(carta.IdNf, "Recepção Evento", statusProcessamento, respostaProcessamento);
//                        CartaCorrecaoDAO.Instance.AtualizaSituacao(carta.IdCarta, (uint)CartaCorrecao.SituacaoEnum.Recusada);
//                        retorno = respostaProcessamento;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao salvar xml de retorno.", ex));
//                }

//                #endregion
//            }
//            catch (Exception ex)
//            {
//                LogNfDAO.Instance.NewLog(carta.IdNf, "Emissão", 1, "Falha ao enviar lote. " + ex.Message);

//                return Glass.MensagemAlerta.FormatErrorMsg("Falha ao enviar lote.", ex);
//            }

//            return retorno;
//        }

        #endregion

        #region Envia pedido de cancelamento

        /// <summary>
        /// Envia pedido de cancelamento do CTe para SEFAZ
        /// </summary>
        /// <param name="idNf"></param>
        /// <param name="justificativa"></param>
        /// <returns></returns>
        public static string EnviaCancelamento(uint idCte, string justificativa)
        {
            ConhecimentoTransporte cte = null;

            try
            {
                // Busca dados do CTe
                cte = ConhecimentoTransporteDAO.Instance.GetElement(idCte);

                // Busca XML de cancelamento
                XmlDocument xmlCanc = ConhecimentoTransporteDAO.Instance.CancelarCTeXmlEvt(justificativa, cte);

                // Monta o lote
                //XmlDocument xmlLote = CriaLoteCancelamento(xmlCanc);

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

                        if (cte.TipoEmissao == (int)ConhecimentoTransporte.TipoEmissaoEnum.Normal)
                        {
                            if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Producao)
                            {
                                switch (uf)
                                {
                                    case "MT":
                                        xmlRetorno = GetWebService.PMTCTeRecepcaoEvento(cte, null).cteRecepcaoEvento(xmlCanc); break;
                                    case "MS":
                                        xmlRetorno = GetWebService.PMSCTeRecepcaoEvento(cte, null).cteRecepcaoEvento(xmlCanc); break;
                                    case "MG":
                                        xmlRetorno = GetWebService.PMGCTeRecepcaoEvento(cte, null).cteRecepcaoEvento(xmlCanc); break;
                                    case "PR":
                                        xmlRetorno = GetWebService.PPRCTeRecepcaoEvento(cte, null).cteRecepcaoEvento(xmlCanc); break;
                                    case "RS":
                                        xmlRetorno = GetWebService.PSVRSCTeRecepcaoEvento(cte, null).cteRecepcaoEvento(xmlCanc); break;
                                    case "SP":
                                        xmlRetorno = GetWebService.PSPCTeRecepcaoEvento(cte, null).cteRecepcaoEvento(xmlCanc); break;
                                    case "AP":
                                    case "PE":
                                    case "RR":
                                        xmlRetorno = GetWebService.PSVSPCTeRecepcaoEvento(cte, null).cteRecepcaoEvento(xmlCanc); break;
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
                                        xmlRetorno = GetWebService.PSVRSCTeRecepcaoEvento(cte, null).cteRecepcaoEvento(xmlCanc); break;
                                }
                            }
                            else
                            {
                                switch (uf)
                                {
                                    //case "MT":
                                    //    xmlRetorno = GetWebService.HMTCTeCancelamento(cte, null).cteCancelamentoCT(xmlLote); break;
                                    case "MS":
                                        xmlRetorno = GetWebService.HMSCTeCancelamento(cte, null).cteCancelamentoCT(xmlCanc); break;
                                    case "MG":
                                        xmlRetorno = GetWebService.HMGCTeCancelamento(cte, null).cteCancelamentoCT(xmlCanc); break;
                                    case "PR":
                                        xmlRetorno = GetWebService.HPRCTeCancelamento(cte, null).cteCancelamentoCT(xmlCanc); break;
                                    case "RS":
                                        xmlRetorno = GetWebService.HRSCTeCancelamento(cte, null).cteCancelamentoCT(xmlCanc); break;
                                    case "SP":
                                        xmlRetorno = GetWebService.HSPCTeCancelamento(cte, null).cteCancelamentoCT(xmlCanc); break;
                                    case "AP":
                                    case "PE":
                                    case "RR":
                                        xmlRetorno = GetWebService.HSVSPCTeCancelamento(cte, null).cteCancelamentoCT(xmlCanc); break;
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
                                        xmlRetorno = GetWebService.HSVRSCTeCancelamento(cte, null).cteRecepcaoEvento(xmlCanc); break;
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
                                        xmlRetorno = GetWebService.PSVRSCTeRecepcaoEvento(cte, null).cteRecepcaoEvento(xmlCanc); break;
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
                                        xmlRetorno = GetWebService.PSVSPCTeRecepcaoEvento(cte, null).cteRecepcaoEvento(xmlCanc); break;
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
                                        xmlRetorno = GetWebService.HSVSPCTeCancelamento(cte, null).cteCancelamentoCT(xmlCanc); break;                                        
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
                                        xmlRetorno = GetWebService.HSVRSCTeCancelamento(cte, null).cteRecepcaoEvento(xmlCanc); break;
                                }
                            }
                        }
                        #endregion

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 2)
                        {
                            LogCteDAO.Instance.NewLog(idCte, "Cancelamento", 1, "Falha ao cancelar CTe. " + ex.Message);

                            ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.FalhaCancelar);

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

                #region Lê Xml de retorno do envio do cancelamento

                // Realiza procedimentos de cancelamento de CTe
                return ConhecimentoTransporteDAO.Instance.RetornoEvtCancelamentoCTe(idCte, justificativa, xmlRetorno);

                #endregion
            }
            catch (Exception ex)
            {
                LogCteDAO.Instance.NewLog(idCte, "Cancelamento", 1, "Falha ao cancelar CTe. " + ex.Message);

                if (cte.Situacao != (int)ConhecimentoTransporte.SituacaoEnum.Cancelado)
                    ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.FalhaCancelar);

                throw ex;
            }
        }

        #endregion

        #region Envia pedido de inutilização

        /// <summary>
        /// Envia pedido de inutilização de CTe para SEFAZ
        /// </summary>
        public static string EnviaInutilizacao(uint idCte, string justificativa)
        {
            try
            {
                // Busca dados da Nota Fiscal
                ConhecimentoTransporte cte = ConhecimentoTransporteDAO.Instance.GetElement(idCte);

                // Busca XML de inutilizalção
                XmlDocument xmlInut = ConhecimentoTransporteDAO.Instance.InutilizarCTeXml(idCte, justificativa);

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

                        var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(idCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
                        string uf = LojaDAO.Instance.GetElement(participante.IdLoja.Value).Uf.ToUpper();

                        if (cte.TipoEmissao == (int)ConhecimentoTransporte.TipoEmissaoEnum.Normal)
                        {
                            if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Producao)
                            {
                                switch (uf)
                                {
                                    case "MT":
                                        xmlRetorno = GetWebService.PMTCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "MS":
                                        xmlRetorno = GetWebService.PMSCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "MG":
                                        xmlRetorno = GetWebService.PMGCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "PR":
                                        xmlRetorno = GetWebService.PPRCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "RS":
                                        xmlRetorno = GetWebService.PRSCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "SP":
                                        xmlRetorno = GetWebService.PSPCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "AP":
                                    case "PE":
                                    case "RR":
                                        xmlRetorno = GetWebService.PSVSPCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
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
                                        xmlRetorno = GetWebService.PSVRSCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                }
                            }
                            else
                            {
                                switch (uf)
                                {
                                    //case "MT":
                                        //xmlRetorno = GetWebService.HMTCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "MS":
                                        xmlRetorno = GetWebService.HMSCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "MG":
                                        xmlRetorno = GetWebService.HMGCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "PR":
                                        xmlRetorno = GetWebService.HPRCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "RS":
                                        xmlRetorno = GetWebService.HRSCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "SP":
                                        xmlRetorno = GetWebService.HSPCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    case "AP":
                                    //case "PE":
                                    //case "RR":
                                        //xmlRetorno = GetWebService.HSVSPCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
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
                                        xmlRetorno = GetWebService.HSVRSCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
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
                                        xmlRetorno = GetWebService.PSVRSCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
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
                                        xmlRetorno = GetWebService.PSVSPCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
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
                                        xmlRetorno = GetWebService.HSVRSCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                    //case "AC":    NÃO EXISTE AMBIENTE DE CONTINGÊNCIA DE HOMOLOGAÇÃO SVSP.
                                    //case "AL":
                                    //case "AM":
                                    //case "BA":
                                    //case "CE":
                                    //case "DF":
                                    //case "ES":
                                    //case "GO":
                                    //case "MA":
                                    //case "MG":
                                    //case "PA":
                                    //case "PB":
                                    //case "PI":
                                    //case "PR":
                                    //case "RJ":
                                    //case "RN":
                                    //case "RO":
                                    //case "RS":
                                    //case "SC":
                                    //case "SE":
                                    //case "TO":
                                    //    xmlRetorno = GetWebService.HSVRSCTeInutilizacao(cte, null).cteInutilizacaoCT(xmlInut); break;
                                }
                            }
                        }

                        #endregion

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 3)
                        {
                            LogCteDAO.Instance.NewLog(idCte, "Inutilização", 1, "Falha ao inutilizar numeração do CTe. " + ex.Message);

                            ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.FalhaInutilizar);

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

                // Realiza procedimentos de inutilização do CTe
                ConhecimentoTransporteDAO.Instance.RetornoInutilizacaoCTe(idCte, justificativa, xmlRetorno);

                string codStatus = xmlRetorno["infInut"]["cStat"].InnerXml;

                if (codStatus == "102")
                    return "Inutilização efetuada.";
                else
                    return "Falha ao inutilizar numeração do CTe. " + xmlRetorno["infInut"]["xMotivo"].InnerXml;

                #endregion
            }
            catch (Exception ex)
            {
                if (ex.Message == "A numeração deste cte já foi inutilizada.")
                    LogCteDAO.Instance.NewLog(idCte, "Inutilização", 1, ex.Message);
                else
                {
                    LogCteDAO.Instance.NewLog(idCte, "Inutilização", 1, "Falha ao inutilizar numeração do CTe. " + ex.Message);

                    ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.FalhaInutilizar);
                }

                throw ex;
            }
        }

        #endregion        
    }
}
