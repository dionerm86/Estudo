using System;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Security.Cryptography.X509Certificates;
using Glass.Data.Helper;

namespace Glass.Data.NFeUtils
{
    public static class GetWebService
    {
        #region Métodos de Suporte

        /// <summary>
        /// Retorna o certificado usado no WebService.
        /// </summary>
        /// <param name="idLoja">O id da loja.</param>
        /// <param name="certPath">O caminho da pasta do certificado.</param>
        /// <returns></returns>
        private static X509Certificate2 GetCertificado(uint idLoja, string certPath)
        {
            return !String.IsNullOrEmpty(certPath) ? Certificado.GetCertificado(idLoja, certPath) :
                Certificado.GetCertificado(idLoja);
        }

        #endregion

        #region NF-e

        #region Consulta

        #region AM

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMConsulta.NfeConsulta2 PAMConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPAMConsulta.NfeConsulta2 retorno = new wsPAMConsulta.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMConsulta.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region BA

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPBAConsulta.NfeConsulta PBAConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPBAConsulta.NfeConsulta retorno = new wsPBAConsulta.NfeConsulta();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPBAConsulta.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region CE

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPCEConsulta.NfeConsulta2 PCEConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPCEConsulta.NfeConsulta2 retorno = new wsPCEConsulta.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPCEConsulta.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region GO

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPGOConsultaProtocolo.NfeConsulta2 PGOConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPGOConsultaProtocolo.NfeConsulta2 retorno = new wsPGOConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPGOConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGConsulta.NfeConsulta2 PMGConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPMGConsulta.NfeConsulta2 retorno = new wsPMGConsulta.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMGConsulta.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSConsultaProtocolo.NfeConsulta2 PMSConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPMSConsultaProtocolo.NfeConsulta2 retorno = new wsPMSConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMSConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTConsultaProtocolo.NfeConsulta2 PMTConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPMTConsultaProtocolo.NfeConsulta2 retorno = new wsPMTConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMTConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PE

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPEConsultaProtocolo.NfeConsulta2 PPEConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPPEConsultaProtocolo.NfeConsulta2 retorno = new wsPPEConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPPEConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRConsulta.NfeConsulta3 PPRConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPPRConsulta.NfeConsulta3 retorno = new wsPPRConsulta.NfeConsulta3();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPPRConsulta.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSConsulta.NfeConsulta2 PRSConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPRSConsulta.NfeConsulta2 retorno = new wsPRSConsulta.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPRSConsulta.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSConsultaProtocolo.NfeConsulta2 HRSConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsHRSConsultaProtocolo.NfeConsulta2 retorno = new wsHRSConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHRSConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPConsultaProtocolo.NfeConsulta2 PSPConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPSPConsultaProtocolo.NfeConsulta2 retorno = new wsPSPConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSPConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVAN

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVANConsulta.NfeConsulta2 PSVANConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPSVANConsulta.NfeConsulta2 retorno = new wsPSVANConsulta.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVANConsulta.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSConsultaProtocolo.NfeConsulta2 PSVRSConsultaProtocolo(NotaFiscal nota, string caminhoCert)
        {
            wsHSVRSConsultaProtocolo.NfeConsulta2 retorno = new wsHSVRSConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHSVRSConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSConsultaProtocolo.NfeConsulta2 PSVRSConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPSVRSConsultaProtocolo.NfeConsulta2 retorno = new wsPSVRSConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVRSConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVCRS

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVCRSConsultaProtocolo.NfeConsulta2 PSVCRSConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPSVCRSConsultaProtocolo.NfeConsulta2 retorno = new wsPSVCRSConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVCRSConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVCAN

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVCANConsultaProtocolo.NfeConsulta2 PSVCANConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPSVCANConsultaProtocolo.NfeConsulta2 retorno = new wsPSVCANConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVCANConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Consulta Cadastro

        #region AM

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPAMConsultaCadastro.CadConsultaCadastro2 PAMConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPAMConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPAMConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }

        #endregion

        #region BA

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPBAConsultaCadastro.CadConsultaCadastro2 PBAConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPBAConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPBAConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPBAConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }

        #endregion

        #region CE

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPCEConsultaCadastro.CadConsultaCadastro2 PCEConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPCEConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPCEConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPCEConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }

        #endregion

        #region GO

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPGOConsultaCadastro.CadConsultaCadastro2 PGOConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPGOConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPGOConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPGOConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }

        #endregion

        #region MG

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPMGConsultaCadastro.CadConsultaCadastro2 PMGConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPMGConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPMGConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMGConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }

        #endregion

        #region MS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPMSConsultaCadastro.CadConsultaCadastro2 PMSConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPMSConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPMSConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMSConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }


        #endregion

        #region MT

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPMTConsultaCadastro.CadConsultaCadastro2 PMTConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPMTConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPMTConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMTConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }

        #endregion

        #region PE

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPPEConsultaCadastro.CadConsultaCadastro2 PPEConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPPEConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPPEConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPPEConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }

        #endregion

        #region PR

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPPRConsultaCadastro.CadConsultaCadastro2 PPRConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPPRConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPPRConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPPRConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }

        #endregion

        #region RS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPRSConsultaCadastro.CadConsultaCadastro2 PRSConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPRSConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPRSConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPRSConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }

        #endregion

        #region SP

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="caminhoCert"></param>
        /// <returns></returns>
        public static wsPSPConsultaCadastro.CadConsultaCadastro2 PSPConsultaCadastro(string UF, string CpfCnpj)
        {
            wsPSPConsultaCadastro.CadConsultaCadastro2 retorno = new wsPSPConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSPConsultaCadastro.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = CidadeDAO.Instance.GetCodIbgeEstadoByEstado(UF);// Cód UF do Cliente
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsCad; // Versão da mensagem envelopada no SOAP

            return retorno;

        }

        #endregion

        #endregion

        #region Inutilização

        #region AM

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMInutilizacao.NfeInutilizacao2 PAMInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPAMInutilizacao.NfeInutilizacao2 retorno = new wsPAMInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region CE

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPCEInutilizacao.NfeInutilizacao2 PCEInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPCEInutilizacao.NfeInutilizacao2 retorno = new wsPCEInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPCEInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region BA

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPBAInutilizacao.NfeInutilizacao PBAInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPBAInutilizacao.NfeInutilizacao retorno = new wsPBAInutilizacao.NfeInutilizacao();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPBAInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region GO

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPGOInutilizacao.NfeInutilizacao2 PGOInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPGOInutilizacao.NfeInutilizacao2 retorno = new wsPGOInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPGOInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGInutilizacao.NfeInutilizacao2 PMGInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPMGInutilizacao.NfeInutilizacao2 retorno = new wsPMGInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMGInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTInutilizacao.NfeInutilizacao2 PMTInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPMTInutilizacao.NfeInutilizacao2 retorno = new wsPMTInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMTInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSInutilizacao.NfeInutilizacao2 PMSInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPMSInutilizacao.NfeInutilizacao2 retorno = new wsPMSInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMSInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PE

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPEInutilizacao.NfeInutilizacao2 PPEInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPPEInutilizacao.NfeInutilizacao2 retorno = new wsPPEInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPPEInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRInutilizacao.NfeInutilizacao3 PPRInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPPRInutilizacao.NfeInutilizacao3 retorno = new wsPPRInutilizacao.NfeInutilizacao3();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPPRInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSInutilizacao.NfeInutilizacao2 PRSInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPRSInutilizacao.NfeInutilizacao2 retorno = new wsPRSInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPRSInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSInutilizacao.NfeInutilizacao2 HRSInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsHRSInutilizacao.NfeInutilizacao2 retorno = new wsHRSInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHRSInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPInutilizacao.NfeInutilizacao2 PSPInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSPInutilizacao.NfeInutilizacao2 retorno = new wsPSPInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSPInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSInutilizacao.NfeInutilizacao2 PSVRSInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSVRSInutilizacao.NfeInutilizacao2 retorno = new wsPSVRSInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVRSInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVAN

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVANInutilizacao.NfeInutilizacao2 PSVANInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSVANInutilizacao.NfeInutilizacao2 retorno = new wsPSVANInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVANInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SCAN

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (SCAN)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsSCANInutilizacao.NfeInutilizacao2 SCANInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsSCANInutilizacao.NfeInutilizacao2 retorno = new wsSCANInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsSCANInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Recepção

        #region AM

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMAutorizacao.NfeAutorizacao PAMAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPAMAutorizacao.NfeAutorizacao retorno = new wsPAMAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPAMAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region CE

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPCEAutorizacao.NfeAutorizacao PCEAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPCEAutorizacao.NfeAutorizacao retorno = new wsPCEAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPCEAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGAutorizacao.NfeAutorizacao PMGAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPMGAutorizacao.NfeAutorizacao retorno = new wsPMGAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPMGAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTAutorizacao.NfeAutorizacao PMTAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPMTAutorizacao.NfeAutorizacao retorno = new wsPMTAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPMTAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSAutorizacao.NfeAutorizacao PMSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPMSAutorizacao.NfeAutorizacao retorno = new wsPMSAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPMSAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region PE

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPEAutorizacao.NfeAutorizacao PPEAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPPEAutorizacao.NfeAutorizacao retorno = new wsPPEAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPPEAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region BA

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPBAAutorizacao.NfeAutorizacao PBAAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPBAAutorizacao.NfeAutorizacao retorno = new wsPBAAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPBAAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region GO

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPGOAutorizacao.NfeAutorizacao PGOAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPGOAutorizacao.NfeAutorizacao retorno = new wsPGOAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPGOAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRAutorizacao.NfeAutorizacao3 PPRAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPPRAutorizacao.NfeAutorizacao3 retorno = new wsPPRAutorizacao.NfeAutorizacao3();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPPRAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSAutorizacao.NfeAutorizacao PRSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPRSAutorizacao.NfeAutorizacao retorno = new wsPRSAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPRSAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSAutorizacao.NfeAutorizacao HRSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsHRSAutorizacao.NfeAutorizacao retorno = new wsHRSAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHRSAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPAutorizacao.NfeAutorizacao PSPAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPSPAutorizacao.NfeAutorizacao retorno = new wsPSPAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSPAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSAutorizacao.NfeAutorizacao PSVRSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPSVRSAutorizacao.NfeAutorizacao retorno = new wsPSVRSAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSVRSAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region SVCAN

        /// <summary>
        /// Retorna o WebService de autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVCANAutorizacao.NfeAutorizacao SVCANAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPSVCANAutorizacao.NfeAutorizacao retorno = new wsPSVCANAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSVCANAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region SVAN

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVANAutorizacao.NfeAutorizacao PSVANAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPSVANAutorizacao.NfeAutorizacao retorno = new wsPSVANAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSVANAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion        

        #region SVCRS

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (SCAN)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVCRSAutorizacao.NfeAutorizacao SVCRSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPSVCRSAutorizacao.NfeAutorizacao retorno = new wsPSVCRSAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSVCRSAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #endregion

        #region Retorno da recepção

        #region AM

        /// <summary>
        /// Retorna o WebService de retorno da autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMRetornoAutorizacao.NfeRetAutorizacao PAMRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPAMRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPAMRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region CE

        /// <summary>
        /// Retorna o WebService de retorno da autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPCERetornoAutorizacao.NfeRetAutorizacao PCERetornoAutorizao(NotaFiscal nota, string caminhoCert)
        {
            wsPCERetornoAutorizacao.NfeRetAutorizacao retorno = new wsPCERetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPCERetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de retorno da recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGRetornoAutorizacao.NfeRetAutorizacao PMGRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPMGRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPMGRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMGRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de retorno da autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTRetornoAutorizacao.NfeRetAutorizacao PMTRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPMTRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPMTRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMTRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de retorno da recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSRetornoAutorizacao.NfeRetAutorizacao PMSRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPMSRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPMSRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMSRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PE

        /// <summary>
        /// Retorna o WebService de retorno da autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPERetornoAutorizacao.NfeRetAutorizacao PPERetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPPERetornoAutorizacao.NfeRetAutorizacao retorno = new wsPPERetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPPERetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region BA

        /// <summary>
        /// Retorna o WebService de retorno da autorizacao da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPBARetornoAutorizacao.NfeRetAutorizacao PBARetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPBARetornoAutorizacao.NfeRetAutorizacao retorno = new wsPBARetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPBARetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region GO

        /// <summary>
        /// Retorna o WebService de retorno da autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPGORetornoAutorizacao.NfeRetAutorizacao PGORetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPGORetornoAutorizacao.NfeRetAutorizacao retorno = new wsPGORetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPGORetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de retorno da autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRRetornoAutorizacao.NfeRetAutorizacao3 PPRRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPPRRetornoAutorizacao.NfeRetAutorizacao3 retorno = new wsPPRRetornoAutorizacao.NfeRetAutorizacao3();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPPRRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de retorno da autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSRetornoAutorizacao.NfeRetAutorizacao PRSRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPRSRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPRSRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPRSRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de retorno da autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPRetornoAutorizacao.NfeRetAutorizacao PSPRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSPRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPSPRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSPRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de retorno da autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSRetornoAutorizacao.NfeRetAutorizacao PSVRSRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSVRSRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPSVRSRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVRSRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVCAN

        /// <summary>
        /// Retorna o WebService de retorno da recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVCANRetornoAutorizacao.NfeRetAutorizacao PSVCANRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSVCANRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPSVCANRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVCANRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVAN

        /// <summary>
        /// Retorna o WebService de retorno da autorização da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVANRetornoAutorizacao.NfeRetAutorizacao PSVANRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSVANRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPSVANRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVANRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion        

        #region SVCAN

        /// <summary>
        /// Retorna o WebService de retorno da autorizacao da nota fiscal. (SVCAN)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVCANRetornoAutorizacao.NfeRetAutorizacao SVCANRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSVCANRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPSVCANRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVCANRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVCRS

        /// <summary>
        /// Retorna o WebService de retorno da autorizacao da nota fiscal. (SVCAN)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVCRSRetornoAutorizacao.NfeRetAutorizacao SVCRSRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSVCRSRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPSVCRSRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVCRSRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Status

        #region AM

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMStatus.NfeStatusServico2 PAMStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPAMStatus.NfeStatusServico2 retorno = new wsPAMStatus.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region CE

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPCEStatus.NfeStatusServico2 PCEStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPCEStatus.NfeStatusServico2 retorno = new wsPCEStatus.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPCEStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGStatus.NfeStatusServico2 PMGStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPMGStatus.NfeStatusServico2 retorno = new wsPMGStatus.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMGStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTStatus.NfeStatusServico2 PMTStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPMTStatus.NfeStatusServico2 retorno = new wsPMTStatus.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMTStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSStatus.NfeStatusServico2 PMSStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPMSStatus.NfeStatusServico2 retorno = new wsPMSStatus.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPMSStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PE

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPEStatus.NfeStatusServico2 PPEStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPPEStatus.NfeStatusServico2 retorno = new wsPPEStatus.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPPEStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region BA

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPBAStatus.NfeStatusServico PBAStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPBAStatus.NfeStatusServico retorno = new wsPBAStatus.NfeStatusServico();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPBAStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region GO

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPGOStatusServico.NfeStatusServico2 PGOStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPGOStatusServico.NfeStatusServico2 retorno = new wsPGOStatusServico.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPGOStatusServico.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRStatus.NfeStatusServico3 PPRStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPPRStatus.NfeStatusServico3 retorno = new wsPPRStatus.NfeStatusServico3();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPPRStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSStatus.NfeStatusServico2 PRSStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPRSStatus.NfeStatusServico2 retorno = new wsPRSStatus.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPRSStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPStatusServico.NfeStatusServico2 PSPStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPSPStatusServico.NfeStatusServico2 retorno = new wsPSPStatusServico.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSPStatusServico.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSStatusServico.NfeStatusServico2 PSVRSStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPSVRSStatusServico.NfeStatusServico2 retorno = new wsPSVRSStatusServico.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVRSStatusServico.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVAN

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVANStatus.NfeStatusServico2 PSVANStatus(NotaFiscal nota, string caminhoCert)
        {
            wsPSVANStatus.NfeStatusServico2 retorno = new wsPSVANStatus.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVANStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SCAN

        /// <summary>
        /// Retorna o WebService de status da nota fiscal. (SCAN)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsSCANStatus.NfeStatusServico2 SCANStatus(NotaFiscal nota, string caminhoCert)
        {
            wsSCANStatus.NfeStatusServico2 retorno = new wsSCANStatus.NfeStatusServico2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsSCANStatus.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Recepção Evento

        #region AM

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMRecepcaoEvento.RecepcaoEvento PAMRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsPAMRecepcaoEvento.RecepcaoEvento retorno = new wsPAMRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPAMRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }


        #endregion

        #region CE

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPCERecepcaoEvento.RecepcaoEvento PCERecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPCERecepcaoEvento.RecepcaoEvento retorno = new wsPCERecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPCERecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }


        #endregion

        #region MG


        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGRecepcaoEvento.RecepcaoEvento PMGRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPMGRecepcaoEvento.RecepcaoEvento retorno = new wsPMGRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPMGRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }


        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTRecepcaoEvento.RecepcaoEvento PMTRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPMTRecepcaoEvento.RecepcaoEvento retorno = new wsPMTRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPMTRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }


        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSRecepcaoEvento.RecepcaoEvento PMSRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPMSRecepcaoEvento.RecepcaoEvento retorno = new wsPMSRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPMSRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }


        #endregion

        #region PE

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPERecepcaoEvento.RecepcaoEvento PPERecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPPERecepcaoEvento.RecepcaoEvento retorno = new wsPPERecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPPERecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }


        #endregion

        #region BA

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPBARecepcaoEvento.RecepcaoEvento PBARecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPBARecepcaoEvento.RecepcaoEvento retorno = new wsPBARecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPBARecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRRecepcaoEvento.RecepcaoEvento PPRRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsPPRRecepcaoEvento.RecepcaoEvento retorno = new wsPPRRecepcaoEvento.RecepcaoEvento();                

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPPRRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region GO


        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPGORecepcaoEvento.RecepcaoEvento PGORecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPGORecepcaoEvento.RecepcaoEvento retorno = new wsPGORecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPGORecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }


        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSRecepcaoEvento.RecepcaoEvento PRSRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPRSRecepcaoEvento.RecepcaoEvento retorno = new wsPRSRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPRSRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Homologação)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSRecepcaoEvento.RecepcaoEvento HRSRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsHRSRecepcaoEvento.RecepcaoEvento retorno = new wsHRSRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHRSRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Homologação)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPRecepcaoEvento.RecepcaoEvento HSPRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsHSPRecepcaoEvento.RecepcaoEvento retorno = new wsHSPRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHSPRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }


        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPRecepcaoEvento.RecepcaoEvento PSPRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPSPRecepcaoEvento.RecepcaoEvento retorno = new wsPSPRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSPRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }


        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Homologação)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSCANRecepcaoEvento.RecepcaoEvento wsHSCANRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsHSCANRecepcaoEvento.RecepcaoEvento retorno = new wsHSCANRecepcaoEvento.RecepcaoEvento();

                // Define 100 segundos de espera, para evitar timeout
                retorno.Timeout = 100000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHSCANRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSRecepcaoEvento.RecepcaoEvento PSVRSRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPSVRSRecepcaoEvento.RecepcaoEvento retorno = new wsPSVRSRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSVRSRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }
        
        #endregion

        #region SVCAN

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVCANRecepcaoEvento.RecepcaoEvento PSVCANRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPSVCANRecepcaoEvento.RecepcaoEvento retorno = new wsPSVCANRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSVCANRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region SVAN

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVANRecepcaoEvento.RecepcaoEvento PSVANRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {

                wsPSVANRecepcaoEvento.RecepcaoEvento retorno = new wsPSVANRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSVANRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }


        #endregion
        
        #region SVCAN

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsSVCANRecepcaoEvento.RecepcaoEvento SVCANRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsSVCANRecepcaoEvento.RecepcaoEvento retorno = new wsSVCANRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsSVCANRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region SVCRS

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVCRSRecepcaoEvento.RecepcaoEvento SVCRSRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsPSVCRSRecepcaoEvento.RecepcaoEvento retorno = new wsPSVCRSRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSVCRSRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #endregion

        #endregion

        #region NFC-e

        #region Autorização

        #region AM

        /// <summary>
        /// Retorna o WebService de autorização da nfc-e. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCAutorizacao.NfeAutorizacao HAMNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsHAMNFCAutorizacao.NfeAutorizacao retorno = new wsHAMNFCAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHAMNFCAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de autorização da nfc-e. (Producao)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCAutorizacao.NfeAutorizacao PAMNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPAMNFCAutorizacao.NfeAutorizacao retorno = new wsPAMNFCAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPAMNFCAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de autorização da nfc-e. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCAutorizacao.NfeAutorizacao HMTNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsHAMNFCAutorizacao.NfeAutorizacao retorno = new wsHAMNFCAutorizacao.NfeAutorizacao();

                retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeAutorizacao?wsdl";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHAMNFCAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de autorização da nfc-e. (Producao)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCAutorizacao.NfeAutorizacao PMTNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPAMNFCAutorizacao.NfeAutorizacao retorno = new wsPAMNFCAutorizacao.NfeAutorizacao();

                retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeAutorizacao?wsdl";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPAMNFCAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de autorização da nfc-e. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCAutorizacao.NfeAutorizacao HRSNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsHAMNFCAutorizacao.NfeAutorizacao retorno = new wsHAMNFCAutorizacao.NfeAutorizacao();

                retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHAMNFCAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de autorização da nfc-e. (Producao)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCAutorizacao.NfeAutorizacao PRSNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPAMNFCAutorizacao.NfeAutorizacao retorno = new wsPAMNFCAutorizacao.NfeAutorizacao();

                retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPAMNFCAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de autorização da nfc-e. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSNFCAutorizacao.NfeAutorizacao HSVRSNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsHSVRSNFCAutorizacao.NfeAutorizacao retorno = new wsHSVRSNFCAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHSVRSNFCAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de autorização da nfc-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSNFCAutorizacao.NfeAutorizacao PSVRSNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                wsPSVRSNFCAutorizacao.NfeAutorizacao retorno = new wsPSVRSNFCAutorizacao.NfeAutorizacao();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSVRSNFCAutorizacao.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteNFe; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #endregion

        #region Retorno da recepção

        #region AM

        /// <summary>
        /// Retorna o WebService de retorno da autorização da NFC-e. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao HAMNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao retorno = new wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHAMNFCRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de retorno da autorização da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao PAMNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMNFCRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de retorno da autorização da NFC-e. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao HMTNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao retorno = new wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao();

            retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeRetAutorizacao?wsdl";

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHAMNFCRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de retorno da autorização da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao PMTNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao();

            retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeRetAutorizacao?wsdl";

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMNFCRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de retorno da autorização da NFC-e. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao HRSNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao retorno = new wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao();

            retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao.asmx";

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHAMNFCRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de retorno da autorização da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao PRSNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao();

            retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao.asmx";

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMNFCRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de retorno da autorização da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSNFCRetornoAutorizacao.NfeRetAutorizacao PSVRSNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSVRSNFCRetornoAutorizacao.NfeRetAutorizacao retorno = new wsPSVRSNFCRetornoAutorizacao.NfeRetAutorizacao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVRSNFCRetornoAutorizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoRetAutorizacao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Consulta

        #region AM

        /// <summary>
        /// Retorna o WebService de consulta da NFC-e. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCConsultaProtocolo.NfeConsulta2 HAMNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsHAMNFCConsultaProtocolo.NfeConsulta2 retorno = new wsHAMNFCConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHAMNFCConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de consulta da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCConsultaProtocolo.NfeConsulta2 PAMNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPAMNFCConsultaProtocolo.NfeConsulta2 retorno = new wsPAMNFCConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMNFCConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta da NFC-e. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCConsultaProtocolo.NfeConsulta2 HMTNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsHAMNFCConsultaProtocolo.NfeConsulta2 retorno = new wsHAMNFCConsultaProtocolo.NfeConsulta2();

            retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeConsulta2?wsdl";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHAMNFCConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de consulta da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCConsultaProtocolo.NfeConsulta2 PMTNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPAMNFCConsultaProtocolo.NfeConsulta2 retorno = new wsPAMNFCConsultaProtocolo.NfeConsulta2();

            retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeConsulta2?wsdl";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMNFCConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta da NFC-e. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCConsultaProtocolo.NfeConsulta2 HRSNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsHAMNFCConsultaProtocolo.NfeConsulta2 retorno = new wsHAMNFCConsultaProtocolo.NfeConsulta2();

            retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHAMNFCConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de consulta da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCConsultaProtocolo.NfeConsulta2 PRSNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPAMNFCConsultaProtocolo.NfeConsulta2 retorno = new wsPAMNFCConsultaProtocolo.NfeConsulta2();

            retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMNFCConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSNFCConsultaProtocolo.NfeConsulta2 PSVRSNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            wsPSVRSNFCConsultaProtocolo.NfeConsulta2 retorno = new wsPSVRSNFCConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVRSNFCConsultaProtocolo.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region QR Code

        /// <summary>
        /// Obtem a url do QR Code da NFC-e
        /// </summary>
        public static string UrlQrCode(string uf, ConfigNFe.TipoAmbienteNfe tipoAmbiente)
        {
            uf = uf.ToUpper();
            var url = "";

            if (tipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                switch (uf)
                {
                    case "AC":
                        url = @"http://hml.sefaznet.ac.gov.br/nfce/qrcode?";
                        break;
                    case "AL":
                        url = "http://nfce.sefaz.al.gov.br/QRCode/consultarNFCe.jsp?";
                        break;
                    case "AP":
                        url = "https://www.sefaz.ap.gov.br/nfce/nfcep.php?";
                        break;
                    case "AM":
                        url = "http://sistemas.sefaz.am.gov.br/nfceweb/consultarNFCe.jsp?";
                        break;
                    case "BA":
                        url = "http://nfe.sefaz.ba.gov.br/servicos/nfce/modulos/geral/NFCEC_consulta_chave_acesso.aspx?";
                        break;
                    case "CE":
                        throw new Exception("Endereço QR Code não mapeado para o estado CE.");
                    case "DF":
                        url = "http://dec.fazenda.df.gov.br/ConsultarNFCe.aspx?";
                        break;
                    case "ES":
                        throw new Exception("Endereço QR Code não mapeado para o estado ES.");
                    case "GO":
                        throw new Exception("Endereço QR Code não mapeado para o estado GO.");
                    case "MA":
                        throw new Exception("Endereço QR Code não mapeado para o estado MA.");
                    case "MT":
                        url = "http://www.sefaz.mt.gov.br/nfce/consultanfce?";
                        break;
                    case "MS":
                        url = "http://www.dfe.ms.gov.br/nfce/qrcode?";
                        break;
                    case "MG":
                        throw new Exception("Endereço QR Code não mapeado para o estado MG.");
                    case "PA":
                        url = "https://appnfc.sefa.pa.gov.br/portal/view/consultas/nfce/nfceForm.seam?";
                        break;
                    case "PB":
                        url = "http://www.receita.pb.gov.br/nfce?";
                        break;
                    case "PR":
                        url = "http://www.dfeportal.fazenda.pr.gov.br/dfe-portal/rest/servico/consultaNFCe?";
                        break;
                    case "PE":
                        url = "http://nfce.sefaz.pe.gov.br/nfce-web/consultarNFCe?";
                        break;
                    case "PI":
                        url = "http://webas.sefaz.pi.gov.br/nfceweb/consultarNFCe.jsf?";
                        break;
                    case "RJ":
                        url = "http://www4.fazenda.rj.gov.br/consultaNFCe/QRCode?";
                        break;
                    case "RN":
                        url = "http://nfce.set.rn.gov.br/consultarNFCe.aspx?";
                        break;
                    case "RS":
                        url = "https://www.sefaz.rs.gov.br/NFCE/NFCE-COM.aspx?";
                        break;
                    case "RO":
                        url = "http://www.nfce.sefin.ro.gov.br/consultanfce/consulta.jsp?";
                        break;
                    case "RR":
                        url = "https://www.sefaz.rr.gov.br/nfce/servlet/qrcode?";
                        break;
                    case "SC":
                        throw new Exception("Endereço QR Code não mapeado para o estado SC.");
                    case "SP":
                        url = "https://www.nfce.fazenda.sp.gov.br/NFCeConsultaPublica/Paginas/ConsultaQRCode.aspx?";
                        break;
                    case "SE":
                        url = "http://www.nfce.se.gov.br/portal/consultarNFCe.jsp?";
                        break;
                    case "TO":
                        throw new Exception("Endereço QR Code não mapeado para o estado TO.");
                }
            }
            else
            {
                switch (uf)
                {
                    case "AC":
                        url = "http://hml.sefaznet.ac.gov.br/nfce/qrcode?";
                        break;
                    case "AL":
                        url = "http://nfce.sefaz.al.gov.br/QRCode/consultarNFCe.jsp?";
                        break;
                    case "AP":
                        url = "https://www.sefaz.ap.gov.br/nfcehml/nfce.php?";
                        break;
                    case "AM":
                        url = "http://homnfce.sefaz.am.gov.br/nfceweb/consultarNFCe.jsp?";
                        break;
                    case "BA":
                        url = "http://hnfe.sefaz.ba.gov.br/servicos/nfce/modulos/geral/NFCEC_consulta_chave_acesso.aspx?";
                        break;
                    case "CE":
                        url = "http://nfceh.sefaz.ce.gov.br/pages/ShowNFCe.html?";
                        break;
                    case "DF":
                        url = "http://dec.fazenda.df.gov.br/ConsultarNFCe.aspx?";
                        break;
                    case "ES":
                        throw new Exception("Endereço QR Code não mapeado para o estado ES.");
                    case "GO":
                        throw new Exception("Endereço QR Code não mapeado para o estado GO.");
                    case "MA":
                        throw new Exception("Endereço QR Code não mapeado para o estado MA.");
                    case "MT":
                        url = "http://www.sefaz.mt.gov.br/nfce/consultanfce?";
                        break;
                    case "MS":
                        url = "http://www.dfe.ms.gov.br/nfce/qrcode?";
                        break;
                    case "MG":
                        throw new Exception("Endereço QR Code não mapeado para o estado MG.");
                    case "PA":
                        url = "https://appnfc.sefa.pa.gov.br/portal-homologacao/view/consultas/nfce/nfceForm.seam?";
                        break;
                    case "PB":
                        url = "http://www.receita.pb.gov.br/nfcehom?";
                        break;
                    case "PR":
                        url = "http://www.dfeportal.fazenda.pr.gov.br/dfe-portal/rest/servico/consultaNFCe?";
                        break;
                    case "PE":
                        url = "http://nfcehomolog.sefaz.pe.gov.br/nfce-web/consultarNFCe?";
                        break;
                    case "PI":
                        url = "http://webas.sefaz.pi.gov.br/nfceweb-homologacao/consultarNFCe.jsf?";
                        break;
                    case "RJ":
                        url = "http://www4.fazenda.rj.gov.br/consultaNFCe/QRCode?";
                        break;
                    case "RN":
                        url = "http://hom.nfce.set.rn.gov.br/consultarNFCe.aspx?";
                        break;
                    case "RS":
                        url = "https://www.sefaz.rs.gov.br/NFCE/NFCE-COM.aspx?";
                        break;
                    case "RO":
                        url = "http://www.nfce.sefin.ro.gov.br/consultanfce/consulta.jsp?";
                        break;
                    case "RR":
                        url = "http://200.174.88.103:8080/nfce/servlet/qrcode?";
                        break;
                    case "SC":
                        throw new Exception("Endereço QR Code não mapeado para o estado SC.");
                    case "SP":
                        url = "https://www.homologacao.nfce.fazenda.sp.gov.br/NFCeConsultaPublica/Paginas/ConsultaQRCode.aspx?";
                        break;
                    case "SE":
                        url = "http://www.hom.nfe.se.gov.br/portal/consultarNFCe.jsp?";
                        break;
                    case "TO":
                        throw new Exception("Endereço QR Code não mapeado para o estado TO.");
                }
            }

            return url;
        }

        #endregion

        #region Url Consulta por Chave de Acesso

        /// <summary>
        /// Obtem a url do QR Code da NFC-e
        /// </summary>
        public static string UrlConsultaPorChaveAcesso(string uf, ConfigNFe.TipoAmbienteNfe tipoAmbiente)
        {
            uf = uf.ToUpper();
            var url = "";

            if (tipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                switch (uf)
                {
                    case "AC":
                        url = @"http://www.hml.sefaznet.ac.gov.br/nfce/consulta.xhtml?";
                        break;
                    case "AM":
                        url = @"http://sistemas.sefaz.am.gov.br/nfceweb/formConsulta.do";
                        break;
                    case "BA":
                        url = "http://nfe.sefaz.ba.gov.br/servicos/nfce/Modulos/Geral/NFCEC_consulta_chave_acesso.aspx?";
                        break;
                    case "DF":
                        url = "http://dec.fazenda.df.gov.br/NFCE";
                        break;
                    case "MT":
                        url = @"https://www.sefaz.mt.gov.br/nfce/consultanfce?";
                        break;
                    case "PA":
                        url = @"https://appnfc.sefa.pa.gov.br/portal/view/consultas/nfce/consultanfce.seam";
                        break;
                    case "PB":
                        url = @"https://www5.receita.pb.gov.br/atf/seg/SEGf_AcessarFuncao.jsp?cdFuncao=FIS_1410&";
                        break;
                    case "RJ":
                        url = @"http://www4.fazenda.rj.gov.br/consultaDFe/paginas/consultaChaveAcesso.faces?";
                        break;
                    case "RN":
                        url = @"http://nfce.set.rn.gov.br/portalDFE/NFCe/ConsultaNFCe.aspx";
                        break;
                    case "RO":
                        url = @"http://www.nfce.sefin.ro.gov.br/";
                        break;
                    case "RS":
                        url = @"http://www.nfe.se.gov.br/portal/consultarNFCe.jsp?";
                        break;
                }
            }
            else
            {
                switch (uf)
                {
                    case "AM":
                        url = @"http://homnfce.sefaz.am.gov.br/nfceweb/formConsulta.do";
                        break;
                    case "BA":
                        url = "http://hnfe.sefaz.ba.gov.br/servicos/nfce/Modulos/Geral/NFCEC_consulta_chave_acesso.aspx?";
                        break;
                    case "MT":
                        url = @"http://homologacao.sefaz.mt.gov.br/nfce/consultanfce?";
                        break;
                    case "PA":
                        url = @"https://appnfc.sefa.pa.gov.br/portal-homologacao/view/consultas/nfce/consultanfce.seam";
                        break;
                    case "RO":
                        url = @"http://www.nfce.sefin.ro.gov.br/consultaAmbHomologacao.jsp";
                        break;
                }
            }

            return url;
        }

        #endregion

        #region Inutilização

        #region AM

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal de consumidor. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCInitilizacao.NfeInutilizacao2 HAMNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsHAMNFCInitilizacao.NfeInutilizacao2 retorno = new wsHAMNFCInitilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHAMNFCInitilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal de consumidor. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCInutilizacao.NfeInutilizacao2 PAMNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPAMNFCInutilizacao.NfeInutilizacao2 retorno = new wsPAMNFCInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMNFCInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal de consumidor. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCInitilizacao.NfeInutilizacao2 HMTNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsHAMNFCInitilizacao.NfeInutilizacao2 retorno = new wsHAMNFCInitilizacao.NfeInutilizacao2();

            retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeInutilizacao2?wsdl";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHAMNFCInitilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal de consumidor. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCInutilizacao.NfeInutilizacao2 PMTNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPAMNFCInutilizacao.NfeInutilizacao2 retorno = new wsPAMNFCInutilizacao.NfeInutilizacao2();

            retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeInutilizacao2?wsdl";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMNFCInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal de consumidor. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCInitilizacao.NfeInutilizacao2 HRSNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsHAMNFCInitilizacao.NfeInutilizacao2 retorno = new wsHAMNFCInitilizacao.NfeInutilizacao2();

            retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao2.asmx";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsHAMNFCInitilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal de consumidor. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCInutilizacao.NfeInutilizacao2 PRSNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPAMNFCInutilizacao.NfeInutilizacao2 retorno = new wsPAMNFCInutilizacao.NfeInutilizacao2();

            retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao2.asmx";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPAMNFCInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal de consumidor. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSNFCInutilizacao.NfeInutilizacao2 PSVRSNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            wsPSVRSNFCInutilizacao.NfeInutilizacao2 retorno = new wsPSVRSNFCInutilizacao.NfeInutilizacao2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.nfeCabecMsgValue = new wsPSVRSNFCInutilizacao.nfeCabecMsg();
            retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nota.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoInutilizacao; // Versão da mensagem (lote) envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Recepção Evento

        #region AM

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal de consumidor. (Homologação)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCRecpcaoEvento.RecepcaoEvento HAMNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsHAMNFCRecpcaoEvento.RecepcaoEvento retorno = new wsHAMNFCRecpcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHAMNFCRecpcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal de consumidor. (Homologação)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCRecepcaoEvento.RecepcaoEvento PAMNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsPAMNFCRecepcaoEvento.RecepcaoEvento retorno = new wsPAMNFCRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPAMNFCRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region MT

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal de consumidor. (Homologação)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCRecpcaoEvento.RecepcaoEvento HMTNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsHAMNFCRecpcaoEvento.RecepcaoEvento retorno = new wsHAMNFCRecpcaoEvento.RecepcaoEvento();

                retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/RecepcaoEvento?wsdl";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHAMNFCRecpcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal de consumidor. (Homologação)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCRecepcaoEvento.RecepcaoEvento PMTNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsPAMNFCRecepcaoEvento.RecepcaoEvento retorno = new wsPAMNFCRecepcaoEvento.RecepcaoEvento();

                retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/RecepcaoEvento?wsdl";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPAMNFCRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal de consumidor. (Homologação)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHAMNFCRecpcaoEvento.RecepcaoEvento HRSNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsHAMNFCRecpcaoEvento.RecepcaoEvento retorno = new wsHAMNFCRecpcaoEvento.RecepcaoEvento();

                retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsHAMNFCRecpcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal de consumidor. (Homologação)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCRecepcaoEvento.RecepcaoEvento PRSNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsPAMNFCRecepcaoEvento.RecepcaoEvento retorno = new wsPAMNFCRecepcaoEvento.RecepcaoEvento();

                retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPAMNFCRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal de consumidor. (Homologação)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSNFCRecepcaoEvento.RecepcaoEvento PSVRSNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsPSVRSNFCRecepcaoEvento.RecepcaoEvento retorno = new wsPSVRSNFCRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.nfeCabecMsgValue = new wsPSVRSNFCRecepcaoEvento.nfeCabecMsg();
                retorno.nfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement(nf.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.nfeCabecMsgValue.versaoDados = ConfigNFe.VersaoLoteCce; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nf.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}