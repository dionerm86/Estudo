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
        public static wsPAMNFeConsulta.NfeConsulta4Soap PAMConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFeConsulta.NfeConsulta4Soap();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPBANFeConsulta.NFeConsultaProtocolo4 PBAConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPBANFeConsulta.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
            var retorno = new wsPCEConsulta.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPGONFeConsultaProtocolo.NFeConsultaProtocolo4 PGOConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPGONFeConsultaProtocolo.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMGNFeConsulta.NFeConsulta4 PMGConsulta(NotaFiscal nota, string caminhoCert)
        {
           var retorno = new wsPMGNFeConsulta.NFeConsulta4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMSNFeConsultaProtocolo.NFeConsultaProtocolo4 PMSConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMSNFeConsultaProtocolo.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMTNFeConsultaProtocolo.NfeConsulta2 PMTConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMTNFeConsultaProtocolo.NfeConsulta2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPENFeConsultaProtocolo.NFeConsultaProtocolo4 PPEConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPPENFeConsultaProtocolo.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPRNFeConsulta.NFeConsultaProtocolo4 PPRConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPPRNFeConsulta.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPRSNFeConsulta.NFeConsultaProtocolo4 PRSConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPRSNFeConsulta.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSNFeConsultaProtocolo.NFeConsultaProtocolo4 HRSConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHRSNFeConsultaProtocolo.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSPNFeConsultaProtocolo.NFeConsultaProtocolo4 PSPConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSPNFeConsultaProtocolo.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVANNFeConsulta.NFeConsultaProtocolo4 PSVANConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVANNFeConsulta.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSNFeConsultaProtocolo.NFeConsultaProtocolo4 PSVRSConsultaProtocolo(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHSVRSNFeConsultaProtocolo.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de consulta da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSNFeConsultaProtocolo.NFeConsultaProtocolo4 PSVRSConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVRSNFeConsultaProtocolo.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVCRSNFeConsultaProtocolo.NFeConsultaProtocolo4 PSVCRSConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVCRSNFeConsultaProtocolo.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVCANNFeConsultaProtocolo.NFeConsultaProtocolo4 PSVCANConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVCANNFeConsultaProtocolo.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFeConsultaCadastro.CadConsultaCadastro2 PAMConsultaCadastro()
        {
            var retorno = new wsPAMNFeConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPBANFeConsultaCadastro.CadConsultaCadastro4 PBAConsultaCadastro()
        {
            var retorno = new wsPBANFeConsultaCadastro.CadConsultaCadastro4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPCEConsultaCadastro.CadConsultaCadastro2 PCEConsultaCadastro()
        {
            wsPCEConsultaCadastro.CadConsultaCadastro2 retorno =
                new wsPCEConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPGONFeConsultaCadastro.CadConsultaCadastro4 PGOConsultaCadastro()
        {
            var retorno = new wsPGONFeConsultaCadastro.CadConsultaCadastro4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMGConsultaCadastro.CadConsultaCadastro2 PMGConsultaCadastro()
        {
            var retorno = new wsPMGConsultaCadastro.CadConsultaCadastro2();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMSNFeConsultaCadastro.CadConsultaCadastro4 PMSConsultaCadastro()
        {
            var retorno = new wsPMSNFeConsultaCadastro.CadConsultaCadastro4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMTNFeConsultaCadastro.CadConsultaCadastro4 PMTConsultaCadastro()
        {
            var retorno = new wsPMTNFeConsultaCadastro.CadConsultaCadastro4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPENFeConsultaCadastro.CadConsultaCadastro4 PPEConsultaCadastro()
        {
            var retorno = new wsPPENFeConsultaCadastro.CadConsultaCadastro4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPRNFeConsultaCadastro.CadConsultaCadastro4 PPRConsultaCadastro()
        {
            var retorno = new wsPPRNFeConsultaCadastro.CadConsultaCadastro4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPRSNFeConsultaCadastro.CadConsultaCadastro4 PRSConsultaCadastro()
        {
            var retorno = new wsPRSNFeConsultaCadastro.CadConsultaCadastro4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSPNFeConsultaCadastro.CadConsultaCadastro4 PSPConsultaCadastro()
        {
            var retorno = new wsPSPNFeConsultaCadastro.CadConsultaCadastro4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFeInutilizacao.NfeInutilizacao4Soap PAMInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFeInutilizacao.NfeInutilizacao4Soap();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPBANFeInutilizacao.NFeInutilizacao4 PBAInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPBANFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPGONFeInutilizacao.NFeInutilizacao4 PGOInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPGONFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMGNFeInutilizacao.NFeInutilizacao4 PMGInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMGNFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMTNFeInutilizacao.NfeInutilizacao4 PMTInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMTNFeInutilizacao.NfeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMSNFeInutilizacao.NFeInutilizacao4 PMSInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMSNFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPENFeInutilizacao.NFeInutilizacao4 PPEInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPPENFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPRNFeInutilizacao.NFeInutilizacao4 PPRInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPPRNFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPRSNFeInutilizacao.NFeInutilizacao4 PRSInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPRSNFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSNFeInutilizacao.NFeInutilizacao4 HRSInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHRSNFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSPNFeInutilizacao.NFeInutilizacao4 PSPInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSPNFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVRSNFeInutilizacao.NFeInutilizacao4 PSVRSInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVRSNFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVANNFeInutilizacao.NFeInutilizacao4 PSVANInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVANNFeInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFeAutorizacao.NfeAutorizacao4Soap PAMAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPAMNFeAutorizacao.NfeAutorizacao4Soap();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMGNFeAutorizacao.NFeAutorizacao4 PMGAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPMGNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMTNFeAutorizacao.NfeAutorizacao4 PMTAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPMTNFeAutorizacao.NfeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMSNFeAutorizacao.NFeAutorizacao4 PMSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPMSNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPENFeAutorizacao.NFeAutorizacao4 PPEAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPPENFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPBANFeAutorizacao.NFeAutorizacao4 PBAAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPBANFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPGONFeAutorizacao.NFeAutorizacao4 PGOAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPGONFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPRNFeAutorizacao.NFeAutorizacao4 PPRAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPPRNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPRSNFeAutorizacao.NFeAutorizacao4 PRSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPRSNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHRSNFeAutorizacao.NFeAutorizacao4 HRSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsHRSNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSPNFeAutorizacao.NFeAutorizacao4 PSPAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSPNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVRSNFeAutorizacao.NFeAutorizacao4 PSVRSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSVRSNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

                return retorno;
            }
            catch (Exception ex)
            {
                LogNfDAO.Instance.NewLog(nota.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        /// <summary>
        /// Retorna o WebService de recepção da nota fiscal. (Homologação)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSNFeAutorizacao.NFeAutorizacao4 HSVRSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsHSVRSNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVCANNFeAutorizacao.NFeAutorizacao4 SVCANAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSVCANNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVANNFeAutorizacao.NFeAutorizacao4 PSVANAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSVANNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVCRSNFeAutorizacao.NFeAutorizacao4 SVCRSAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSVCRSNFeAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFeRetornoAutorizacao.NfeRetAutorizacao4Soap PAMRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFeRetornoAutorizacao.NfeRetAutorizacao4Soap();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMGNFeRetornoAutorizacao.NFeRetAutorizacao4 PMGRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMGNFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMTNFeRetornoAutorizacao.NfeRetAutorizacao4 PMTRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMTNFeRetornoAutorizacao.NfeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMSNFeRetornoAutorizacao.NFeRetAutorizacao4 PMSRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMSNFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPENFeRetornoAutorizacao.NFeRetAutorizacao4 PPERetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPPENFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPBANFeRetornoAutorizacao.NFeRetAutorizacao4 PBARetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPBANFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPGONFeRetornoAutorizacao.NFeRetAutorizacao4 PGORetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPGONFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPRNFeRetornoAutorizacao.NFeRetAutorizacao4 PPRRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPPRNFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPRSNFeRetornoAutorizacao.NFeRetAutorizacao4 PRSRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPRSNFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSPNFeRetornoAutorizacao.NFeRetAutorizacao4 PSPRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSPNFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVRSNFeRetornoAutorizacao.NFeRetAutorizacao4 PSVRSRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVRSNFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVCANNFeRetornoAutorizacao.NFeRetAutorizacao4 PSVCANRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVCANNFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVANNFeRetornoAutorizacao.NFeRetAutorizacao4 PSVANRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVANNFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVCANNFeRetornoAutorizacao.NFeRetAutorizacao4 SVCANRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVCANNFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVCRSNFeRetornoAutorizacao.NFeRetAutorizacao4 SVCRSRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVCRSNFeRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFeStatus.NfeStatusServico4Soap PAMStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFeStatus.NfeStatusServico4Soap();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMGNFeStatus.NFeStatusServico4 PMGStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMGNFeStatus.NFeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMTNFeStatus.NfeStatusServico4 PMTStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMTNFeStatus.NfeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMSNFeStatus.NFeStatusServico4 PMSStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPMSNFeStatus.NFeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPENFeStatus.NFeStatusServico4 PPEStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPPENFeStatus.NFeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPBANFeStatus.NFeStatusServico4 PBAStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPBANFeStatus.NFeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPGONFeStatusServico.NFeStatusServico4 PGOStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPGONFeStatusServico.NFeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPRNFeStatus.NFeStatusServico4 PPRStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPPRNFeStatus.NFeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPRSNFeStatus.NfeStatusServico4 PRSStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPRSNFeStatus.NfeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSPNFeStatusServico.NFeStatusServico4 PSPStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSPNFeStatusServico.NFeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVRSNFeStatusServico.NfeStatusServico4 PSVRSStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVRSNFeStatusServico.NfeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVANNFeStatus.NFeStatusServico4 PSVANStatus(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVANNFeStatus.NFeStatusServico4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFeRecepcaoEvento.RecepcaoEvento4Soap PAMRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPAMNFeRecepcaoEvento.RecepcaoEvento4Soap();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMTNFeRecepcaoEvento.RecepcaoEvento4 PMTRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPMTNFeRecepcaoEvento.RecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPMSNFeRecepcaoEvento.NFeRecepcaoEvento4 PMSRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPMSNFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPENFeRecepcaoEvento.RecepcaoEvento PPERecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPPENFeRecepcaoEvento.RecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPBANFeRecepcaoEvento.NFeRecepcaoEvento4 PBARecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPBANFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPPRNFeRecepcaoEvento.NFeRecepcaoEvento4 PPRRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPPRNFeRecepcaoEvento.NFeRecepcaoEvento4();                

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPGONFeRecepcaoEvento.NFeRecepcaoEvento4 PGORecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPGONFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPRSNFeRecepcaoEvento.NFeRecepcaoEvento4 PRSRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPRSNFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHRSNFeRecepcaoEvento.NFeRecepcaoEvento4 HRSRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsHRSNFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPNFeRecepcaoEvento.NFeRecepcaoEvento4 PSPRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSPNFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHSPNFeRecepcaoEvento.NFeRecepcaoEvento4 HSPRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsHSPNFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        /// Retorna o WebService de recepção da nota fiscal. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSNFeRecepcaoEvento.NFeRecepcaoEvento4 PSVRSRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSVRSNFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVCANNFeRecepcaoEvento.NFeRecepcaoEvento4 PSVCANRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSVCANNFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVANNFeRecepcaoEvento.NFeRecepcaoEvento4 PSVANRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSVANNFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVCRSNFeRecepcaoEvento.NFeRecepcaoEvento4 PSVCRSRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSVCRSNFeRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCAutorizacao.NfeAutorizacao4Soap HAMNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsHAMNFCAutorizacao.NfeAutorizacao4Soap();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFCAutorizacao.NfeAutorizacao4Soap PAMNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPAMNFCAutorizacao.NfeAutorizacao4Soap();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCAutorizacao.NfeAutorizacao4Soap HMTNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsHAMNFCAutorizacao.NfeAutorizacao4Soap();

                retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeAutorizacao?wsdl";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFCAutorizacao.NfeAutorizacao4Soap PMTNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPAMNFCAutorizacao.NfeAutorizacao4Soap();

                retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeAutorizacao?wsdl";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCAutorizacao.NfeAutorizacao4Soap HRSNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsHAMNFCAutorizacao.NfeAutorizacao4Soap();

                retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFCAutorizacao.NfeAutorizacao4Soap PRSNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPAMNFCAutorizacao.NfeAutorizacao4Soap();

                retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHSVRSNFCAutorizacao.NFeAutorizacao4 HSVRSNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsHSVRSNFCAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVRSNFCAutorizacao.NFeAutorizacao4 PSVRSNFCeAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSVRSNFCAutorizacao.NFeAutorizacao4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap HAMNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de retorno da autorização da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap PAMNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap HMTNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap();

            retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeRetAutorizacao?wsdl";

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de retorno da autorização da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap PMTNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap();

            retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeRetAutorizacao?wsdl";

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap HRSNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap();

            retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao.asmx";

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de retorno da autorização da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap PRSNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFCRetornoAutorizacao.NfeRetAutorizacao4Soap();

            retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao.asmx";

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVRSNFCRetornoAutorizacao.NFeRetAutorizacao4 PSVRSNFCeRetornoAutorizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVRSNFCRetornoAutorizacao.NFeRetAutorizacao4();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCConsultaProtocolo.NfeConsulta4Soap HAMNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHAMNFCConsultaProtocolo.NfeConsulta4Soap();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de consulta da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCConsultaProtocolo.NfeConsulta4Soap PAMNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFCConsultaProtocolo.NfeConsulta4Soap();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCConsultaProtocolo.NfeConsulta4Soap HMTNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHAMNFCConsultaProtocolo.NfeConsulta4Soap();

            retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeConsulta2?wsdl";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de consulta da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCConsultaProtocolo.NfeConsulta4Soap PMTNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFCConsultaProtocolo.NfeConsulta4Soap();

            retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeConsulta2?wsdl";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCConsultaProtocolo.NfeConsulta4Soap HRSNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHAMNFCConsultaProtocolo.NfeConsulta4Soap();

            retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de consulta da NFC-e. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCConsultaProtocolo.NfeConsulta4Soap PRSNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFCConsultaProtocolo.NfeConsulta4Soap();

            retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVRSNFCConsultaProtocolo.NFeConsultaProtocolo4 PSVRSNFCeConsulta(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVRSNFCConsultaProtocolo.NFeConsultaProtocolo4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCInitilizacao.NfeInutilizacao4Soap HAMNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHAMNFCInitilizacao.NfeInutilizacao4Soap();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal de consumidor. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCInutilizacao.NfeInutilizacao4Soap PAMNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFCInutilizacao.NfeInutilizacao4Soap();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCInitilizacao.NfeInutilizacao4Soap HMTNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHAMNFCInitilizacao.NfeInutilizacao4Soap();

            retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeInutilizacao2?wsdl";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal de consumidor. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCInutilizacao.NfeInutilizacao4Soap PMTNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFCInutilizacao.NfeInutilizacao4Soap();

            retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeInutilizacao2?wsdl";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCInitilizacao.NfeInutilizacao4Soap HRSNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsHAMNFCInitilizacao.NfeInutilizacao4Soap();

            retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao2.asmx";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        /// <summary>
        /// Retorna o WebService de inutilização da nota fiscal de consumidor. (Produção)
        /// </summary>
        /// <param name="nota">A nota fiscal que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPAMNFCInutilizacao.NfeInutilizacao4Soap PRSNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPAMNFCInutilizacao.NfeInutilizacao4Soap();

            retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao2.asmx";

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPSVRSNFCInutilizacao.NFeInutilizacao4 PSVRSNFCInutilizacao(NotaFiscal nota, string caminhoCert)
        {
            var retorno = new wsPSVRSNFCInutilizacao.NFeInutilizacao4();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(nota.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCRecepcaoEvento.RecepcaoEvento4Soap HAMNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsHAMNFCRecepcaoEvento.RecepcaoEvento4Soap();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFCRecepcaoEvento.RecepcaoEvento4Soap PAMNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPAMNFCRecepcaoEvento.RecepcaoEvento4Soap();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCRecepcaoEvento.RecepcaoEvento4Soap HMTNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsHAMNFCRecepcaoEvento.RecepcaoEvento4Soap();

                retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/RecepcaoEvento?wsdl";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFCRecepcaoEvento.RecepcaoEvento4Soap PMTNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPAMNFCRecepcaoEvento.RecepcaoEvento4Soap();

                retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/RecepcaoEvento?wsdl";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHAMNFCRecepcaoEvento.RecepcaoEvento4Soap HRSNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsHAMNFCRecepcaoEvento.RecepcaoEvento4Soap();

                retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsPAMNFCRecepcaoEvento.RecepcaoEvento4Soap PRSNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                wsPAMNFCRecepcaoEvento.RecepcaoEvento4Soap retorno = new wsPAMNFCRecepcaoEvento.RecepcaoEvento4Soap();

                retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        /// Retorna o WebService de recepção da nota fiscal de consumidor. (Produção)
        /// </summary>
        /// <param name="nf">Nota fiscal da carta de correção</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSNFCRecepcaoEvento.NFeRecepcaoEvento4 PSVRSNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsPSVRSNFCRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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
        public static wsHSVRSNFCRecepcaoEvento.NFeRecepcaoEvento4 HSVRSNFCRecepcaoEvento(NotaFiscal nf, string caminhoCert)
        {
            try
            {
                var retorno = new wsHSVRSNFCRecepcaoEvento.NFeRecepcaoEvento4();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado(nf.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

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