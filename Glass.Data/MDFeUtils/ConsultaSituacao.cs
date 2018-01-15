using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Glass.Data.MDFeUtils
{
    public class ConsultaSituacao
    {
        #region Consulta situação do Lote de MDFe (método acionado pelo usuário)

        /// <summary>
        /// Consulta situação do Lote de MDFe (método acionado pelo usuário)
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        /// <returns></returns>
        public static string ConsultaSitLoteMDFe(int idManifestoEletronico)
        {
            var mdfe = ManifestoEletronicoDAO.Instance.ObterManifestoEletronicoPeloId(idManifestoEletronico);
            var protocolo = ProtocoloMDFeDAO.Instance.GetElement(idManifestoEletronico, (int)ProtocoloMDFe.TipoProtocoloEnum.Autorizacao);

            #region Monta XML de requisição de situação do lote

            if (protocolo == null || string.IsNullOrEmpty(protocolo.NumRecibo))
                throw new Exception("O MDFe não foi emitido. Não há número de recibo.");

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<consReciMDFe xmlns=\"http://www.portalfiscal.inf.br/mdfe\" " +
                "versao=\"" + ConfigMDFe.VersaoRecepcao + "\">" +
                "<tpAmb>" + (int)ConfigMDFe.TipoAmbiente + "</tpAmb>" +
                "<nRec>" + protocolo.NumRecibo.PadLeft(15, '0') + "</nRec></consReciMDFe>";

            XmlDocument xmlRetRecep = new XmlDocument();
            xmlRetRecep.LoadXml(strXml);

            #endregion

            #region Valida XML

            try
            {
                ValidaXML.Validar(xmlRetRecep, ValidaXML.TipoArquivoXml.ConsultaRecibo);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #endregion

            // Guarda o status do lote
            var cStat = 0;
            var xMotivo = string.Empty;

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

                if (mdfe.TipoEmissao == Glass.Data.Model.TipoEmissao.Normal)
                {
                    if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                    {
                        xmlRetorno = GetWebService.PSVRSMDFeRetornoRecepcao(mdfe, null).mdfeRetRecepcao(xmlRetRecep);
                    }
                    // TipoAmbienteMDFe.Homologacao
                    else
                    {
                        xmlRetorno = GetWebService.HSVRSMDFeRetornoRecepcao(mdfe, null).mdfeRetRecepcao(xmlRetRecep);
                    }
                }
                // TipoEmissao.Contingencia
                else
                {
                    if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                    {
                        xmlRetorno = GetWebService.PSVRSMDFeRetornoRecepcao(mdfe, null).mdfeRetRecepcao(xmlRetRecep);
                    }
                    // TipoAmbienteMDFe.Homologacao
                    else
                    {
                        xmlRetorno = GetWebService.HSVRSMDFeRetornoRecepcao(mdfe, null).mdfeRetRecepcao(xmlRetRecep);
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
            {
                cStat = Conversoes.StrParaInt(xmlRetorno["cStat"].InnerXml);
                xMotivo = xmlRetorno["xMotivo"].InnerXml;
            }

            // Verifica o status do lote
            if (cStat == 104) // Lote processado
            {
                XmlNodeList protMDFeList = ((XmlElement)xmlRetorno).GetElementsByTagName("protMDFe");

                // Para cada protocolo de autorização de uso (inicialmente será só um, pois cada nota está sendo enviada em um lote distinto)
                foreach (XmlNode protMDFeNode in protMDFeList)
                {
                    return ManifestoEletronicoDAO.Instance.RetornoEmissaoMDFe(idManifestoEletronico, protMDFeNode);
                }

                return "Lote processado";
            }
            else if (cStat == 105) // Lote em processamento
            {
                if (mdfe.Situacao != SituacaoEnum.ContingenciaOffline)
                    ManifestoEletronicoDAO.Instance.AlteraSituacao(idManifestoEletronico, SituacaoEnum.ProcessoEmissao);

                return "Este MDFe ainda está sendo processado pela SEFAZ, aguarde para realizar uma nova consulta.";
            }
            else if (cStat == 106) // Lote não encontrado
            {
                if (mdfe.Situacao != SituacaoEnum.ContingenciaOffline)
                    ManifestoEletronicoDAO.Instance.AlteraSituacao(idManifestoEletronico, SituacaoEnum.FalhaEmitir);

                LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Consulta", cStat, "Falha na emissão do MDFe. " + xMotivo);

                return "Falha na consulta. Não foi encontrado o lote de envio.";
            }
            else // Lote rejeitado
            {
                if (mdfe.Situacao != SituacaoEnum.ContingenciaOffline)
                    ManifestoEletronicoDAO.Instance.AlteraSituacao(idManifestoEletronico, SituacaoEnum.FalhaEmitir);

                LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Consulta", cStat, xMotivo);

                string msgErro = "Falha na consulta. ";

                if (cStat == 215 || cStat == 243 || cStat == 630)
                    msgErro += "Mensagem de consulta inválida. ";

                return msgErro + cStat + " - " + CustomizaMensagemRejeicao(idManifestoEletronico, xMotivo);
            }
        }

        #endregion

        #region Consulta situação do MDFe

        /// <summary>
        /// Consulta situação do MDFe
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        public static string ConsultaSitMDFe(int idManifestoEletronico)
        {
            // Busca dados do Manifesto Eletronico
            var mdfe = ManifestoEletronicoDAO.Instance.ObterManifestoEletronicoPeloId(idManifestoEletronico);

            #region Monta XML

            XmlDocument xmlConsSitMDFe = new XmlDocument();
            XmlNode declarationNode = xmlConsSitMDFe.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlConsSitMDFe.AppendChild(declarationNode);

            XmlElement consSitMDFe = xmlConsSitMDFe.CreateElement("consSitMDFe");
            consSitMDFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/mdfe");
            consSitMDFe.SetAttribute("versao", ConfigMDFe.VersaoConsulta);
            xmlConsSitMDFe.AppendChild(consSitMDFe);

            ManipulacaoXml.SetNode(xmlConsSitMDFe, consSitMDFe, "tpAmb", ((int)ConfigMDFe.TipoAmbiente).ToString());
            ManipulacaoXml.SetNode(xmlConsSitMDFe, consSitMDFe, "xServ", "CONSULTAR");
            ManipulacaoXml.SetNode(xmlConsSitMDFe, consSitMDFe, "chMDFe", mdfe.ChaveAcesso);

            #endregion

            #region Valida XML

            //try
            //{
            //    ValidaXML.Validar(xmlConsSitMDFe, ValidaXML.TipoArquivoXml.ConsultaSituacaoMDFe);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("XML inconsistente." + ex.Message);
            //}

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

                if (mdfe.TipoEmissao == Glass.Data.Model.TipoEmissao.Normal)
                {
                    if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                    {
                        xmlRetorno = GetWebService.PSVRSMDFeConsulta(mdfe, null).mdfeConsultaMDF(xmlConsSitMDFe);
                    }
                    // TipoAmbienteMDFe.Homologacao
                    else
                    {
                        xmlRetorno = GetWebService.HSVRSMDFeConsulta(mdfe, null).mdfeConsultaMDF(xmlConsSitMDFe);
                    }
                }
                // TipoEmissao.Contingencia
                else
                {
                    if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                    {
                        xmlRetorno = GetWebService.PSVRSMDFeConsulta(mdfe, null).mdfeConsultaMDF(xmlConsSitMDFe);
                    }
                    // TipoAmbienteMDFe.Homologacao
                    else
                    {
                        xmlRetorno = GetWebService.HSVRSMDFeConsulta(mdfe, null).mdfeConsultaMDF(xmlConsSitMDFe);
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

            // Executa ações de acordo com o retorno
            return ManifestoEletronicoDAO.Instance.RetornoConsSitMDFe(idManifestoEletronico, xmlRetorno);
        }

        #endregion

        #region Customiza mensagem de rejeição

        /// <summary>
        /// Customiza mensagem de rejeição
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        /// <param name="motivoRejeicao"></param>
        /// <returns></returns>
        public static string CustomizaMensagemRejeicao(int idManifestoEletronico, string motivoRejeicao)
        {
            return motivoRejeicao;
        }

        #endregion
    }
}
