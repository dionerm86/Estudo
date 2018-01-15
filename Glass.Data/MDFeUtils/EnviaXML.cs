using Glass.Data.DAL;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Glass.Data.MDFeUtils
{
    public class EnviaXML
    {
        #region Cria lote

        /// <summary>
        /// Monta e retorna um Xml do lote para envio com o MDF-e a ser enviada
        /// </summary>
        /// <param name="xmlMDFe"></param>
        /// <returns></returns>
        private static XmlDocument CriaLote(XmlDocument xmlMDFe, int idManifestoEletronico)
        {
            XmlDocument xmlLote = new XmlDocument();
            XmlNode declarationNode = xmlLote.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlLote.AppendChild(declarationNode);

            string enviMDFeString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<enviMDFe xmlns=\"http://www.portalfiscal.inf.br/mdfe\" versao=\"" + ConfigMDFe.VersaoEnvioMDFe + "\">" +
                "<idLote>" + ManifestoEletronicoDAO.Instance.ObterNovoNumLote(idManifestoEletronico).ToString("000000000000000") + "</idLote>";

            // Insere o XML da MDFe no lote
            int nPosI = xmlMDFe.InnerXml.IndexOf("<MDFe");
            int nPosF = xmlMDFe.InnerXml.Length - nPosI;
            enviMDFeString += xmlMDFe.InnerXml.Substring(nPosI, nPosF) + "</enviMDFe>";

            XmlDocument xmlRetorno = new XmlDocument();
            xmlRetorno.LoadXml(enviMDFeString);

            #region Valida XML

            try
            {
                ValidaXML.Validar(xmlRetorno, ValidaXML.TipoArquivoXml.EnviMDFe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            return xmlRetorno;
        }

        #endregion

        #region Envia pedido de autorização de MDFe

        /// <summary>
        /// Envia a MDFe para a SEFAZ via Webservice
        /// </summary>
        /// <param name="xmlMDFe"></param>
        /// <param name="idManifestoEletronico"></param>
        public static string EnviaMDFe(XmlDocument xmlMDFe, int idManifestoEletronico)
        {
            try
            {
                // Monta o lote
                XmlDocument xmlLote = CriaLote(xmlMDFe, idManifestoEletronico);

                // Busca dados do Manifesto Eletronico
                var mdfe = ManifestoEletronicoDAO.Instance.ObterManifestoEletronicoPeloId(idManifestoEletronico);

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

                        #region Envia o arquivo e recebe o retorno

                        if (mdfe.TipoEmissao == Glass.Data.Model.TipoEmissao.Normal)
                        {
                            if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                            {
                                xmlRetorno = GetWebService.PSVRSMDFeRecepcao(mdfe, null).mdfeRecepcaoLote(xmlLote);
                            }
                            // TipoAmbienteMDFe.Homologacao
                            else
                            {
                                xmlRetorno = GetWebService.HSVRSMDFeRecepcao(mdfe, null).mdfeRecepcaoLote(xmlLote);
                            }
                        }
                        // TipoEmissao.Contingencia
                        else
                        {
                            if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                            {
                                xmlRetorno = GetWebService.PSVRSMDFeRecepcao(mdfe, null).mdfeRecepcaoLote(xmlLote);
                            }
                            // TipoAmbienteMDFe.Homologacao
                            else
                            {
                                xmlRetorno = GetWebService.HSVRSMDFeRecepcao(mdfe, null).mdfeRecepcaoLote(xmlLote);
                            }
                        }

                        #endregion

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 3)
                        {
                            LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Emissão", 1, "Falha ao enviar lote. " + ex.Message);
                            if (mdfe.Situacao != SituacaoEnum.ContingenciaOffline)
                                ManifestoEletronicoDAO.Instance.AlteraSituacao(idManifestoEletronico, Glass.Data.Model.SituacaoEnum.FalhaEmitir);
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
                    LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Emissão", 2, "Falha ao enviar lote. Retorno de envio do lote inválido.");

                    if (mdfe.Situacao != SituacaoEnum.ContingenciaOffline)
                        ManifestoEletronicoDAO.Instance.AlteraSituacao(idManifestoEletronico, Glass.Data.Model.SituacaoEnum.FalhaEmitir);

                    return "Falha ao enviar lote. Retorno de envio do lote inválido.";
                }

                // Lê Xml de retorno do envio do lote
                var cStat = Conversoes.StrParaInt(xmlRetorno["cStat"].InnerXml);
                var xMotivo = xmlRetorno["xMotivo"].InnerXml;

                if (cStat == 103) // Lote recebido com sucesso
                {
                    LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Emissão", cStat, xMotivo);

                    // Pega o número do recibo do lote
                    string numReciboLote = xmlRetorno["infRec"]["nRec"].InnerXml;

                    // Salva no MDFe o número do recibo do lote
                    ManifestoEletronicoDAO.Instance.SalvaReciboProtocoloAutorizacao(idManifestoEletronico, numReciboLote);
                    return xMotivo;
                }
                else if (cStat > 200) // Lote foi rejeitado pela SEFAZ
                {
                    LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Emissão", cStat, xMotivo);

                    if (mdfe.Situacao != SituacaoEnum.ContingenciaOffline)
                        ManifestoEletronicoDAO.Instance.AlteraSituacao(idManifestoEletronico, Glass.Data.Model.SituacaoEnum.FalhaEmitir);

                    return xMotivo;
                }
                else
                {
                    LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Emissão", cStat, xMotivo);
                    return xMotivo;
                }

                #endregion
            }
            catch (Exception ex)
            {
                LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Emissão", 1, MensagemAlerta.FormatErrorMsg("Falha ao enviar lote.", ex));

                return MensagemAlerta.FormatErrorMsg("Falha ao enviar lote.", ex);
            }
        }

        #endregion

        #region Envia pedido de cancelamento

        /// <summary>
        /// Envia pedido de cancelamento do MDFe para SEFAZ
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        /// <param name="justificativa"></param>
        /// <returns></returns>
        public static string EnviaCancelamento(int idManifestoEletronico, string justificativa)
        {
            ManifestoEletronico mdfe = null;

            try
            {
                // Busca dados do MDFe
                mdfe = ManifestoEletronicoDAO.Instance.ObterManifestoEletronicoPeloId(idManifestoEletronico);

                // Busca XML de cancelamento
                XmlDocument xmlCanc = ManifestoEletronicoDAO.Instance.GerarXmlMDFeCancelamento(justificativa, mdfe);

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

                        if (mdfe.TipoEmissao == TipoEmissao.Normal)
                        {
                            if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                            {
                                xmlRetorno = GetWebService.PSVRSMDFeRecepcaoEvento(mdfe, null).mdfeRecepcaoEvento(xmlCanc);
                            }
                            // TipoAmbienteMDFe.Homologacao
                            else
                            {
                                xmlRetorno = GetWebService.HSVRSMDFeRecepcaoEvento(mdfe, null).mdfeRecepcaoEvento(xmlCanc);
                            }
                        }
                        // TipoEmissao.Contingencia
                        else
                        {
                            if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                            {
                                xmlRetorno = GetWebService.PSVRSMDFeRecepcaoEvento(mdfe, null).mdfeRecepcaoEvento(xmlCanc);
                            }
                            // TipoAmbienteMDFe.Homologacao
                            else
                            {
                                xmlRetorno = GetWebService.HSVRSMDFeRecepcaoEvento(mdfe, null).mdfeRecepcaoEvento(xmlCanc);
                            }
                        }
                        #endregion

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 2)
                        {
                            LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Cancelamento", 1, "Falha ao cancelar MDFe. " + ex.Message);

                            ManifestoEletronicoDAO.Instance.AlteraSituacao(idManifestoEletronico, SituacaoEnum.FalhaCancelar);

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

                // Realiza procedimentos de cancelamento de MDFe
                return ManifestoEletronicoDAO.Instance.RetornoEventoMDFe(idManifestoEletronico, xmlRetorno, "110111");

                #endregion
            }
            catch (Exception ex)
            {
                LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Cancelamento", 1, "Falha ao cancelar MDFe. " + ex.Message);

                if (mdfe.Situacao != SituacaoEnum.Cancelado)
                    ManifestoEletronicoDAO.Instance.AlteraSituacao(idManifestoEletronico, SituacaoEnum.FalhaCancelar);

                throw ex;
            }
        }

        #endregion

        #region Envia pedido de encerramento

        /// <summary>
        /// Envia pedido de encerramento do MDFe para SEFAZ
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        /// <returns></returns>
        public static string EnviaEncerramento(int idManifestoEletronico)
        {
            ManifestoEletronico mdfe = null;

            try
            {
                // Busca dados do MDFe
                mdfe = ManifestoEletronicoDAO.Instance.ObterManifestoEletronicoPeloId(idManifestoEletronico);

                // Busca XML de encerramento
                XmlDocument xmlEncerramento = ManifestoEletronicoDAO.Instance.GerarXmlMDFeEncerramento(mdfe);

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

                        if (mdfe.TipoEmissao == TipoEmissao.Normal)
                        {
                            if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                            {
                                xmlRetorno = GetWebService.PSVRSMDFeRecepcaoEvento(mdfe, null).mdfeRecepcaoEvento(xmlEncerramento);
                            }
                            // TipoAmbienteMDFe.Homologacao
                            else
                            {
                                xmlRetorno = GetWebService.HSVRSMDFeRecepcaoEvento(mdfe, null).mdfeRecepcaoEvento(xmlEncerramento);
                            }
                        }
                        // TipoEmissao.Contingencia
                        else
                        {
                            if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                            {
                                xmlRetorno = GetWebService.PSVRSMDFeRecepcaoEvento(mdfe, null).mdfeRecepcaoEvento(xmlEncerramento);
                            }
                            // TipoAmbienteMDFe.Homologacao
                            else
                            {
                                xmlRetorno = GetWebService.HSVRSMDFeRecepcaoEvento(mdfe, null).mdfeRecepcaoEvento(xmlEncerramento);
                            }
                        }
                        #endregion

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 2)
                        {
                            LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Encerramento", 1, "Falha ao encerrar MDFe. " + ex.Message);

                            ManifestoEletronicoDAO.Instance.AlteraSituacao(idManifestoEletronico, SituacaoEnum.FalhaEncerrar);

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

                #region Lê Xml de retorno do envio do encerramento

                // Realiza procedimentos de encerramento de MDFe
                return ManifestoEletronicoDAO.Instance.RetornoEventoMDFe(idManifestoEletronico, xmlRetorno, "110112");

                #endregion
            }
            catch (Exception ex)
            {
                LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Encerramento", 1, "Falha ao encerrar MDFe. " + ex.Message);

                if (mdfe.Situacao != SituacaoEnum.Encerrado)
                    ManifestoEletronicoDAO.Instance.AlteraSituacao(idManifestoEletronico, SituacaoEnum.FalhaEncerrar);

                throw ex;
            }
        }

        #endregion

        #region Envia pedido de consulta não encerrados

        /// <summary>
        /// Envia pedido de consulta não encerrados para SEFAZ
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public static string EnviaConsultaNaoEncerrados(uint idLoja)
        {
            try
            {
                var tipoEmissao = Configuracoes.FiscalConfig.ManifestoEletronico.ContingenciaMDFe == Helper.DataSources.TipoContingenciaMDFe.NaoUtilizar ? TipoEmissao.Normal :
                    TipoEmissao.Contingencia;

                var lojaEmitente = LojaDAO.Instance.GetElement(idLoja);

                // Busca XML de encerramento
                XmlDocument xmlConsMDFeNaoEnc = ManifestoEletronicoDAO.Instance.GerarXmlMDFeConsultaNaoEncerrados(lojaEmitente);

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

                        if (tipoEmissao == TipoEmissao.Normal)
                        {
                            if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                            {
                                xmlRetorno = GetWebService.PSVRSMDFeConsultaNaoEncerrado(lojaEmitente, null).mdfeConsNaoEnc(xmlConsMDFeNaoEnc);
                            }
                            // TipoAmbienteMDFe.Homologacao
                            else
                            {
                                xmlRetorno = GetWebService.HSVRSMDFeConsultaNaoEncerrado(lojaEmitente, null).mdfeConsNaoEnc(xmlConsMDFeNaoEnc);
                            }
                        }
                        // TipoEmissao.Contingencia
                        else
                        {
                            if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Producao)
                            {
                                xmlRetorno = GetWebService.PSVRSMDFeConsultaNaoEncerrado(lojaEmitente, null).mdfeConsNaoEnc(xmlConsMDFeNaoEnc);
                            }
                            // TipoAmbienteMDFe.Homologacao
                            else
                            {
                                xmlRetorno = GetWebService.HSVRSMDFeConsultaNaoEncerrado(lojaEmitente, null).mdfeConsNaoEnc(xmlConsMDFeNaoEnc);
                            }
                        }
                        #endregion

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i == 2)
                        {
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

                #region Lê Xml de retorno do consulta não encerrados

                return ManifestoEletronicoDAO.Instance.RetornoConsultaNaoEncerrados(xmlRetorno);

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
