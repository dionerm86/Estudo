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
        public static string ConsultaLote(uint idNf)
        {
            NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(idNf);

            if (nf.Consumidor)
            {
                return ConsultaLoteNFCe(nf);
            }

            return ConsultaLoteNFe(nf);
        }

        private static string ConsultaLoteNFe(NotaFiscal nf)
        {
            #region Monta XML de requisição de situação do lote

            if (string.IsNullOrEmpty(nf.NumRecibo))
            {
                throw new Exception("A NFe não foi emitida. Não há número de recibo.");
            }

            var strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
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

            var status = string.Empty;
            XmlNode xmlRetorno = null;
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

                // Envia o arquivo e recebe o retorno
                xmlRetorno = EnviaXML.ObterXmlRetornoAutorizacaoNFe(nf, xmlRetRecep);
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
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
                status = xmlRetorno?["cStat"]?.InnerXml;
            }

            if (status == "104") // Lote processado
            {
                XmlNodeList protNFeList = ((XmlElement)xmlRetorno).GetElementsByTagName("protNFe");

                // Para cada protocolo de autorização de uso (inicialmente será só um, pois cada nota está sendo enviada em um lote distinto)
                foreach (XmlNode protNFeNode in protNFeList)
                {
                    NotaFiscalDAO.Instance.RetornoEmissaoNFe(protNFeNode?["infProt"]?["chNFe"]?.InnerXml, protNFeNode);

                    var statusNFe = protNFeNode?["infProt"]?["cStat"]?.InnerXml;

                    if (statusNFe == "100" || statusNFe == "150") // Autorizada para uso
                    {
                        return "NFe está autorizada para uso.";
                    }
                    else
                    {
                        return $"NFe rejeitada. Motivo: { protNFeNode?["infProt"]?["xMotivo"]?.InnerXml }";
                    }
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

                LogNfDAO.Instance.NewLog(nf.IdNf, "Consulta", (xmlRetorno?["cStat"]?.InnerXml?.StrParaInt()).GetValueOrDefault(), xmlRetorno?["xMotivo"]?.InnerXml);

                var msgErro = "Falha na consulta. ";

                if (status == "215" || status == "516" || status == "517" || status == "545")
                {
                    msgErro += "Mensagem de consulta inválida. ";
                }
                else if (status == "225" || status == "565" || status == "567" || status == "568")
                {
                    msgErro += "Lote da NFe é inválido. ";
                }

                return $"{ msgErro }{ xmlRetorno?["cStat"]?.InnerXml } - { CustomizaMensagemRejeicao(nf.IdNf, xmlRetorno?["xMotivo"]?.InnerXml) }";
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

                // Envia o arquivo e recebe o retorno.
                xmlRetorno = EnviaXML.ObterXmlRetornoAutorizacaoNFCe(nf, xmlRetRecep);
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao chamar WebService.", ex));
            }
            finally
            {
                // Restaura o callback padrão para o WebService
                System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
            }

            if (xmlRetorno != null) // Lote processado
            {
                status = xmlRetorno?["cStat"]?.InnerXml;
            }

            // Verifica o status do lote
            if (status == "104") // Lote processado
            {
                XmlNodeList protNFeList = ((XmlElement)xmlRetorno).GetElementsByTagName("protNFe");

                // Para cada protocolo de autorização de uso (inicialmente será só um, pois cada nota está sendo enviada em um lote distinto)
                foreach (XmlNode protNFeNode in protNFeList)
                {
                    NotaFiscalDAO.Instance.RetornoEmissaoNFe(protNFeNode?["infProt"]?["chNFe"]?.InnerXml, protNFeNode);

                    var statusNFe = protNFeNode?["infProt"]?["cStat"]?.InnerXml;

                    if (statusNFe == "100" || statusNFe == "150") // Autorizada para uso
                    {
                        return "NFe está autorizada para uso.";
                    }
                    else
                    {
                        return $"NFe rejeitada. Motivo: { protNFeNode?["infProt"]?["xMotivo"]?.InnerXml }";
                    }
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
                LogNfDAO.Instance.NewLog(nf.IdNf, "Consulta", (xmlRetorno?["cStat"]?.InnerXml?.StrParaInt()).GetValueOrDefault(), xmlRetorno?["xMotivo"]?.InnerXml);

                var msgErro = "Falha na consulta. ";

                if (status == "215" || status == "516" || status == "517" || status == "545")
                {
                    msgErro += "Mensagem de consulta inválida. ";
                }
                else if (status == "225" || status == "565" || status == "567" || status == "568")
                {
                    msgErro += "Lote da NFe é inválido. ";
                }

                return $"{ msgErro }{ xmlRetorno?["cStat"]?.InnerXml } - { CustomizaMensagemRejeicao(nf.IdNf, xmlRetorno?["xMotivo"]?.InnerXml) }";
            }
        }

        #endregion

        #region Consulta situação da NFe

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

                xmlRetorno = EnviaXML.ObterXmlConsultaProtocoloNFe(nf, xmlConsSitNFe);
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao chamar WebService.", ex));
            }
            finally
            {
                // Restaura o callback padrão para o WebService
                System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
            }

            if (xmlRetorno == null)
            {
                throw new Exception("Falha ao comunicar com webservice da SEFAZ.");
            }

            var codStatus = xmlRetorno?["cStat"]?.InnerXml?.StrParaInt() ?? 0;

            // Executa ações de acordo com o retorno dado
            NotaFiscalDAO.Instance.RetornoConsSitNFe(nf.IdNf, xmlRetorno);

            if (codStatus == 100 || codStatus == 150)
            {
                return "NFe está autorizada para uso.";
            }
            else
            {
                var msgErro = "Falha na consulta. ";

                if (codStatus == 215 || codStatus == 516 || codStatus == 517 || codStatus == 545)
                {
                    msgErro += "Mensagem de consulta inválida. ";
                }

                return $"msgErro { xmlRetorno?["cStat"]?.InnerXml } - { CustomizaMensagemRejeicao(nf.IdNf, xmlRetorno?["xMotivo"]?.InnerXml) }";
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

                // Envia o arquivo e recebe o retorno.
                xmlRetorno = EnviaXML.ObterXmlConsultaProtocoloNFCe(nf, xmlConsSitNFe);
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao chamar WebService.", ex));
            }
            finally
            {
                // Restaura o callback padrão para o WebService
                System.Net.ServicePointManager.ServerCertificateValidationCallback = callback;
            }

            if (xmlRetorno == null)
                throw new Exception("Falha ao comunicar com webservice da SEFAZ.");

            var codStatus = xmlRetorno?["cStat"]?.InnerXml;

            // Executa ações de acordo com o retorno dado
            NotaFiscalDAO.Instance.RetornoConsSitNFe(nf.IdNf, xmlRetorno);

            if (codStatus == "100" || codStatus == "150") // NFe Autorizada
            {
                return "NFe está autorizada para uso.";
            }
            else // NFe rejeitada
            {
                var msgErro = "Falha na consulta. ";

                if (codStatus == "215" || codStatus == "516" || codStatus == "517" || codStatus == "545")
                {
                    msgErro += "Mensagem de consulta inválida. ";
                }

                return $"{ msgErro }{ xmlRetorno?["cStat"]?.InnerXml } - { CustomizaMensagemRejeicao(nf.IdNf, xmlRetorno?["xMotivo"]?.InnerXml) }";
            }
        }

        #endregion

        #region Consulta o cadastro de contribuintes do ICMS da unidade federada.

        /// <summary>
        /// Consulta o cadastro de contribuintes do ICMS da unidade federada.
        /// </summary>
        public static string ConsultaSitCadastroContribuinte(string uf, string cpfCnpj)
        {
            if (string.IsNullOrEmpty(uf) || string.IsNullOrEmpty(cpfCnpj))
            {
                return "Contribuinte não encontrado.";
            }

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
            ManipulacaoXml.SetNode(xmlConsCad, infCons, "UF", uf);

            cpfCnpj = Formatacoes.LimpaCpfCnpj(cpfCnpj);

            if (cpfCnpj.Length == 11)
            {
                ManipulacaoXml.SetNode(xmlConsCad, infCons, "CPF", cpfCnpj);
            }
            else
            {
                ManipulacaoXml.SetNode(xmlConsCad, infCons, "CNPJ", cpfCnpj);
            }

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

                // Envia o arquivo e recebe o retorno.
                xmlRetorno = EnviaXML.ObterXmlConsultaCadastroContribuinte(uf, xmlConsCad);
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
            {
                return "Falha ao comunicar com webservice da SEFAZ.";
            }

            XmlDocument xmlDocRetorno = new XmlDocument();
            xmlDocRetorno.ImportNode(xmlRetorno, true);

            XmlNamespaceManager nMgr = new XmlNamespaceManager(xmlDocRetorno.NameTable);
            nMgr.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");

            XmlNode infoCons = xmlRetorno.SelectSingleNode("//nfe:infCons", nMgr);
            var codStatus = infoCons?["cStat"]?.InnerText;

            var retorno = string.Empty;
            if (codStatus == "111" || codStatus == "112")
            {
                retorno += "Consulta Situação do Contribuinte no Sintegra\n\n";
                retorno += "Situação: ";

                if (infoCons["infCad"]["cSit"].InnerText == "1")
                {
                    retorno += "Habilitado.";
                }
                else
                {
                    retorno += "Não Habilitado.";
                }
            }
            else
            {
                retorno += "Falha na Consulta Situação do Contribuinte no Sintegra\n\n";
                retorno += "Código: " + codStatus + "\n";
                retorno += infoCons?["xMotivo"]?.InnerText;
            }

            ClienteDAO.Instance.AtualizaUltimaConsultaSintegra(cpfCnpj);

            return retorno;
        }

        /// <summary>
        /// Verifica se a consulta de cadastro esta disponivel
        /// para o cliente em questão
        /// </summary>
        public static bool HabilitadoConsultaCadastro(string uf)
        {
            if (string.IsNullOrEmpty(uf))
            {
                return false;
            }

            switch (uf)
            {
                case "AC":
                case "MG":
                case "MT":
                case "GO":
                case "PB":
                case "PE":
                case "RN":
                case "SC":
                case "SP": return true;
                default: return false;
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
            var lojaSimplesNacional = LojaDAO.Instance.ObtemValorCampo<int?>(session, "Crt", $"IdLoja={ NotaFiscalDAO.Instance.ObtemIdLoja(session, idNf) }") < 3;

            if (motivoRejeicao.Contains("Total da BC ICMS difere do somatório dos itens"))
            {
                if (lojaSimplesNacional)
                {
                    motivoRejeicao += " (Empresas optantes pelo simples nacional devem destacar ICMS apenas nas informações complementares)";
                }
                else
                {
                    motivoRejeicao += " (Um ou mais produtos desta nota fiscal possuem CST que não calcula ICMS, CST 60 por exemplo, apesar de estar sendo calculado ICMS na mesma)";
                }
            }
            else if (motivoRejeicao.Contains("Total da BC ICMS-ST difere do somatório dos itens"))
            {
                if (lojaSimplesNacional)
                {
                    motivoRejeicao += " (Um ou mais produtos desta nota fiscal possuem CSOSN que não devem calcular ICMS ST, apesar de estar sendo calculado na mesma)";
                }
                else
                {
                    motivoRejeicao += " (Um ou mais produtos desta nota fiscal possuem CST que não devem calcular ICMS ST, CST 00 por exemplo, apesar de estar sendo calculado na mesma)";
                }
            }
            else if (motivoRejeicao.Contains("Falha na solicitação com status HTTP 403: Forbidden."))
            {
                motivoRejeicao += " (Provavelmente não foi exportada a chave privada do certificado digital ou as cadeias do certificado digital não foram instaladas corretamente.)";
            }

            return motivoRejeicao;
        }

        #endregion
    }
}