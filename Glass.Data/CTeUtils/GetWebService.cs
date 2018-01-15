using System;
using System.Security.Cryptography.X509Certificates;
using Glass.Data.DAL;
using Glass.Data.Model.Cte;
using Glass.Data.DAL.CTe;
using Glass.Data.Helper;

namespace Glass.Data.CTeUtils
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

        /// <summary>
        /// Utilizado para o A3
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="usaWebService"></param>
        /// <returns></returns>
        //private static X509Certificate2 BuscarNome(string nome, bool usaWebService)
        //{
        //    return Certificado.BuscaNome(nome, usaWebService);
        //}

        #endregion

        #region Produção

        #region Consulta

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTCTeConsulta.CteConsulta PMTCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMTCTeConsulta.CteConsulta retorno = new Glass.Data.wsPMTCTeConsulta.CteConsulta();
            
            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            
            retorno.cteCabecMsgValue = new Glass.Data.wsPMTCTeConsulta.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSCTeConsulta.CteConsulta PMSCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMSCTeConsulta.CteConsulta retorno = new Glass.Data.wsPMSCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
       
            retorno.cteCabecMsg = new Glass.Data.wsPMSCTeConsulta.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGCTeConsulta.CteConsulta PMGCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMGCTeConsulta.CteConsulta retorno = new Glass.Data.wsPMGCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
           
            retorno.cteCabecMsgValue = new Glass.Data.wsPMGCTeConsulta.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRCTeConsulta.CteConsulta PPRCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPPRCTeConsulta.CteConsulta retorno = new Glass.Data.wsPPRCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                     
            retorno.cteCabecMsgValue = new Glass.Data.wsPPRCTeConsulta.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSCTeConsulta.CteConsulta PRSCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPRSCTeConsulta.CteConsulta retorno = new Glass.Data.wsPRSCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
    
            retorno.cteCabecMsgValue = new Glass.Data.wsPRSCTeConsulta.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }        

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeConsulta.CteConsulta PSPCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSPCTeConsulta.CteConsulta retorno = new Glass.Data.wsPSPCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPSPCTeConsulta.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSCTeConsulta.CteConsulta PSVRSCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSVRSCTeConsulta.CteConsulta retorno = new Glass.Data.wsPSVRSCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPSVRSCTeConsulta.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }        

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeConsulta.CteConsulta PSVSPCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            return PSPCTeConsulta(cte, caminhoCert);
        }    

        #endregion

        #endregion

        #region Consulta Cadastro

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSCTeConsultaCadastro.CadConsultaCadastro PMSCTeConsultaCadastro(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMSCTeConsultaCadastro.CadConsultaCadastro retorno = new Glass.Data.wsPMSCTeConsultaCadastro.CadConsultaCadastro();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsg = new Glass.Data.wsPMSCTeConsultaCadastro.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion
     
        #endregion

        #region Inutilização

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTCTeInutilizacao.CteInutilizacao PMTCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMTCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsPMTCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPMTCTeInutilizacao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSCTeInutilizacao.CteInutilizacao PMSCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMSCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsPMSCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            // Monta o cabeçalho do SOAP            
            retorno.cteCabecMsg = new Glass.Data.wsPMSCTeInutilizacao.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGCTeInutilizacao.CteInutilizacao PMGCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMGCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsPMGCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            // Monta o cabeçalho do SOAP            
            retorno.cteCabecMsgValue = new Glass.Data.wsPMGCTeInutilizacao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRCTeInutilizacao.CteInutilizacao PPRCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPPRCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsPPRCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            // Monta o cabeçalho do SOAP            
            retorno.cteCabecMsgValue = new Glass.Data.wsPPRCTeInutilizacao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSCTeInutilizacao.CteInutilizacao PRSCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPRSCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsPRSCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            // Monta o cabeçalho do SOAP            
            retorno.cteCabecMsgValue = new Glass.Data.wsPRSCTeInutilizacao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeInutilizacao.CteInutilizacao PSPCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSPCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsPSPCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            // Monta o cabeçalho do SOAP            
            retorno.cteCabecMsgValue = new Glass.Data.wsPSPCTeInutilizacao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSCTeInutilizacao.CteInutilizacao PSVRSCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSVRSCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsPSVRSCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            // Monta o cabeçalho do SOAP            
            retorno.cteCabecMsgValue = new Glass.Data.wsPSVRSCTeInutilizacao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeInutilizacao.CteInutilizacao PSVSPCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            return PSPCTeInutilizacao(cte, caminhoCert);
        }

        #endregion

        #endregion

        #region Recepção

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTCTeRecepcao.CteRecepcao PMTCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMTCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsPMTCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPMTCTeRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSCTeRecepcao.CteRecepcao PMSCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMSCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsPMSCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            // Monta o cabeçalho do SOAP            
            retorno.cteCabecMsg = new Glass.Data.wsPMSCTeRecepcao.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGCTeRecepcao.CteRecepcao PMGCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMGCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsPMGCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
     
            retorno.cteCabecMsgValue = new Glass.Data.wsPMGCTeRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRCTeRecepcao.CteRecepcao PPRCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPPRCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsPPRCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPPRCTeRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSCTeRecepcao.CteRecepcao PRSCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPRSCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsPRSCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
 
            retorno.cteCabecMsgValue = new Glass.Data.wsPRSCTeRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeRecepcao.CteRecepcao PSPCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSPCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsPSPCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            
            retorno.cteCabecMsgValue = new Glass.Data.wsPSPCTeRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSCTeRecepcao.CteRecepcao PSVRSCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSVRSCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsPSVRSCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPSVRSCTeRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeRecepcao.CteRecepcao PSVSPCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            return PSPCTeRecepcao(cte, caminhoCert);
        }

        #endregion

        #endregion

        #region Retorno da recepção

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTCTeRetRecepcao.CteRetRecepcao PMTCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMTCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsPMTCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPMTCTeRetRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSCTeRetRecepcao.CteRetRecepcao PMSCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMSCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsPMSCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsg = new Glass.Data.wsPMSCTeRetRecepcao.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGCTeRetRecepcao.CteRetRecepcao PMGCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMGCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsPMGCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPMGCTeRetRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRCTeRetRecepcao.CteRetRecepcao PPRCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPPRCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsPPRCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                  
            retorno.cteCabecMsgValue = new Glass.Data.wsPPRCTeRetRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSCTeRetRecepcao.CteRetRecepcao PRSCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPRSCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsPRSCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
      
            retorno.cteCabecMsgValue = new Glass.Data.wsPRSCTeRetRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeRetRecepcao.CteRetRecepcao PSPCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSPCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsPSPCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
      
            retorno.cteCabecMsgValue = new Glass.Data.wsPSPCTeRetRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSCTeRetRecepcao.CteRetRecepcao PSVRSCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSVRSCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsPSVRSCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPSVRSCTeRetRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeRetRecepcao.CteRetRecepcao PSVSPCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            return PSPCTeRetRecepcao(cte, caminhoCert);
        }

        #endregion

        #endregion

        #region Status

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTCTeStatus.CteStatusServico PMTCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMTCTeStatus.CteStatusServico retorno = new Glass.Data.wsPMTCTeStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPMTCTeStatus.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSCTeStatus.CteStatusServico PMSCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMSCTeStatus.CteStatusServico retorno = new Glass.Data.wsPMSCTeStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsg = new Glass.Data.wsPMSCTeStatus.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGCTeStatus.CteStatusServico PMGCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMGCTeStatus.CteStatusServico retorno = new Glass.Data.wsPMGCTeStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
          
            retorno.cteCabecMsgValue = new Glass.Data.wsPMGCTeStatus.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRCTeStatus.CteStatusServico PPRCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPPRCTeStatus.CteStatusServico retorno = new Glass.Data.wsPPRCTeStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
          
            retorno.cteCabecMsgValue = new Glass.Data.wsPPRCTeStatus.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPRSCTeStatus.CteStatusServico PRSCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPRSCTeStatus.CteStatusServico retorno = new Glass.Data.wsPRSCTeStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
      
            retorno.cteCabecMsgValue = new Glass.Data.wsPRSCTeStatus.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeStatus.CteStatusServico PSPCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSPCTeStatus.CteStatusServico retorno = new Glass.Data.wsPSPCTeStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
   
            retorno.cteCabecMsgValue = new Glass.Data.wsPSPCTeStatus.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSCTeStatusServico.CteStatusServico PSVRSCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSVRSCTeStatusServico.CteStatusServico retorno = new Glass.Data.wsPSVRSCTeStatusServico.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPSVRSCTeStatusServico.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeStatus.CteStatusServico PSVSPCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            return PSPCTeStatus(cte, caminhoCert);
        }

        #endregion

        #endregion

        #region Cancelamento

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMTCTeRecepcaoEvento.CteRecepcaoEvento PMTCTeRecepcaoEvento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMTCTeRecepcaoEvento.CteRecepcaoEvento retorno = new Glass.Data.wsPMTCTeRecepcaoEvento.CteRecepcaoEvento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPMTCTeRecepcaoEvento.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMSCTeRecepcaoEvento.CteRecepcaoEvento PMSCTeRecepcaoEvento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMSCTeRecepcaoEvento.CteRecepcaoEvento retorno = new Glass.Data.wsPMSCTeRecepcaoEvento.CteRecepcaoEvento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsg = new Glass.Data.wsPMSCTeRecepcaoEvento.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPMGCTeRecepcaoEvento.RecepcaoEvento PMGCTeRecepcaoEvento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPMGCTeRecepcaoEvento.RecepcaoEvento retorno = new Glass.Data.wsPMGCTeRecepcaoEvento.RecepcaoEvento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPMGCTeRecepcaoEvento.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPPRCTeRecepcaoEvento.CteRecepcaoEvento PPRCTeRecepcaoEvento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPPRCTeRecepcaoEvento.CteRecepcaoEvento retorno = new Glass.Data.wsPPRCTeRecepcaoEvento.CteRecepcaoEvento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPPRCTeRecepcaoEvento.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeRecepcaoEvento.CteRecepcaoEvento PSPCTeRecepcaoEvento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSPCTeRecepcaoEvento.CteRecepcaoEvento retorno = new Glass.Data.wsPSPCTeRecepcaoEvento.CteRecepcaoEvento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPSPCTeRecepcaoEvento.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSCTeRecepcaoEvento.CteRecepcaoEvento PSVRSCTeRecepcaoEvento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsPSVRSCTeRecepcaoEvento.CteRecepcaoEvento retorno = new Glass.Data.wsPSVRSCTeRecepcaoEvento.CteRecepcaoEvento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsPSVRSCTeRecepcaoEvento.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Produção)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSPCTeRecepcaoEvento.CteRecepcaoEvento PSVSPCTeRecepcaoEvento(ConhecimentoTransporte cte, string caminhoCert)
        {
            return PSPCTeRecepcaoEvento(cte, caminhoCert);
        }

        #endregion

        #endregion

        #endregion
        
        #region Homologação

        #region Consulta

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        //public static wsHMTCTeConsulta.CteConsulta HMTCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        //{
        //    Glass.Data.wsPMTCTeConsulta.CteConsulta retorno = new Glass.Data.wsPMTCTeConsulta.CteConsulta();

        //    retorno.cteCabecMsgValue = new Glass.Data.wsPMTCTeConsulta.cteCabecMsg();
        //    var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
        //    retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja).CodUf;
        //    retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

        //    return retorno;
        //}

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMSCTeConsulta.CteConsulta HMSCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMSCTeConsulta.CteConsulta retorno = new Glass.Data.wsHMSCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
         
            retorno.cteCabecMsg = new Glass.Data.wsHMSCTeConsulta.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMGCTeConsulta.CteConsulta HMGCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMGCTeConsulta.CteConsulta retorno = new Glass.Data.wsHMGCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
      
            retorno.cteCabecMsgValue = new Glass.Data.wsHMGCTeConsulta.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHPRCTeConsulta.CteConsulta HPRCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHPRCTeConsulta.CteConsulta retorno = new Glass.Data.wsHPRCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                   
            retorno.cteCabecMsgValue = new Glass.Data.wsHPRCTeConsulta.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCTeConsulta.CteConsulta HRSCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHRSCTeConsulta.CteConsulta retorno = new Glass.Data.wsHRSCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                       
            retorno.cteCabecMsgValue = new Glass.Data.wsHRSCTeConsulta.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeConsulta.CteConsulta HSPCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHSPCTeConsulta.CteConsulta retorno = new Glass.Data.wsHSPCTeConsulta.CteConsulta();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
          
            retorno.cteCabecMsgValue = new Glass.Data.wsHSPCTeConsulta.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCTeConsulta.CteConsulta HSVRSCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            return HRSCTeConsulta(cte, caminhoCert);
        }

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeConsulta.CteConsulta HSVSPCTeConsulta(ConhecimentoTransporte cte, string caminhoCert)
        {
            return HSPCTeConsulta(cte, caminhoCert);
        }

        #endregion

        #endregion

        #region Consulta Cadastro

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMSCTeConsultaCadastro.CadConsultaCadastro HMSCTeConsultaCadastro(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMSCTeConsultaCadastro.CadConsultaCadastro retorno = new Glass.Data.wsHMSCTeConsultaCadastro.CadConsultaCadastro();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
          
            retorno.cteCabecMsg = new Glass.Data.wsHMSCTeConsultaCadastro.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Inutilização

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        //public static wsHMTCTeInutilizacao.CteInutilizacao HMTCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        //{
        //    Glass.Data.wsHMTCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsHMTCTeInutilizacao.CteInutilizacao();

        //    retorno.cteCabecMsgValue = new Glass.Data.wsHMTCTeInutilizacao.cteCabecMsg();
        //    var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
        //    retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja).CodUf;
        //    retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

        //    return retorno;
        //}

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMSCTeInutilizacao.CteInutilizacao HMSCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMSCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsHMSCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
       
            retorno.cteCabecMsg = new Glass.Data.wsHMSCTeInutilizacao.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMGCTeInutilizacao.CteInutilizacao HMGCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMGCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsHMGCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
          
            retorno.cteCabecMsgValue = new Glass.Data.wsHMGCTeInutilizacao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHPRCTeInutilizacao.CteInutilizacao HPRCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHPRCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsHPRCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
           
            retorno.cteCabecMsgValue = new Glass.Data.wsHPRCTeInutilizacao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCTeInutilizacao.CteInutilizacao HRSCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHRSCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsHRSCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
         
            retorno.cteCabecMsgValue = new Glass.Data.wsHRSCTeInutilizacao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeInutilizacao.CteInutilizacao HSPCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHSPCTeInutilizacao.CteInutilizacao retorno = new Glass.Data.wsHSPCTeInutilizacao.CteInutilizacao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
       
            retorno.cteCabecMsgValue = new Glass.Data.wsHSPCTeInutilizacao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCTeInutilizacao.CteInutilizacao HSVRSCTeInutilizacao(ConhecimentoTransporte cte, string caminhoCert)
        {
            return HRSCTeInutilizacao(cte, caminhoCert);
        }

        #endregion        

        #endregion

        #region Recepção

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        //public static wsHMTCTeRecepcao.CteRecepcao HMTCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        //{
        //    Glass.Data.wsHMTCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsHMTCTeRecepcao.CteRecepcao();

        //    retorno.cteCabecMsgValue = new Glass.Data.wsHMTCTeRecepcao.cteCabecMsg();
        //    var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
        //    retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja).CodUf;
        //    retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

        //    return retorno;
        //}

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMSCTeRecepcao.CteRecepcao HMSCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMSCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsHMSCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsg = new Glass.Data.wsHMSCTeRecepcao.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMGCTeRecepcao.CteRecepcao HMGCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMGCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsHMGCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
          
            retorno.cteCabecMsgValue = new Glass.Data.wsHMGCTeRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHPRCTeRecepcao.CteRecepcao HPRCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHPRCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsHPRCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
          
            retorno.cteCabecMsgValue = new Glass.Data.wsHPRCTeRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCTeRecepcao.CteRecepcao HRSCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHRSCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsHRSCTeRecepcao.CteRecepcao();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsHRSCTeRecepcao.cteCabecMsg();            
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP
            
            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeRecepcao.CteRecepcao HSPCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHSPCTeRecepcao.CteRecepcao retorno = new Glass.Data.wsHSPCTeRecepcao.CteRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
     
            retorno.cteCabecMsgValue = new Glass.Data.wsHSPCTeRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCTeRecepcao.CteRecepcao HSVRSCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            return HRSCTeRecepcao(cte, caminhoCert);
        }

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeRecepcao.CteRecepcao HSVSPCTeRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            return HSPCTeRecepcao(cte, caminhoCert);
        }

        #endregion

        #endregion

        #region Retorno da recepção

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        //public static wsHMTCTeRetRecepcao.CteRetRecepcao HMTCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        //{
        //    Glass.Data.wsPMTCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsHMTCTeRetRecepcao.CteRetRecepcao();

        //    retorno.cteCabecMsgValue = new Glass.Data.wsHMTCTeRetRecepcao.cteCabecMsg();
        //    var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
        //    retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja).CodUf;
        //    retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

        //    return retorno;
        //}

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMSCTeRetRecepcao.CteRetRecepcao HMSCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMSCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsHMSCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsg = new Glass.Data.wsHMSCTeRetRecepcao.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMGCTeRetRecepcao.CteRetRecepcao HMGCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMGCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsHMGCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
          
            retorno.cteCabecMsgValue = new Glass.Data.wsHMGCTeRetRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHPRCTeRetRecepcao.CteRetRecepcao HPRCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHPRCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsHPRCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
          
            retorno.cteCabecMsgValue = new Glass.Data.wsHPRCTeRetRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCTeRetRecepcao.CteRetRecepcao HRSCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHRSCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsHRSCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
       
            retorno.cteCabecMsgValue = new Glass.Data.wsHRSCTeRetRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeRetRecepcao.CteRetRecepcao HSPCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHSPCTeRetRecepcao.CteRetRecepcao retorno = new Glass.Data.wsHSPCTeRetRecepcao.CteRetRecepcao();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
 
            retorno.cteCabecMsgValue = new Glass.Data.wsHSPCTeRetRecepcao.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCTeRetRecepcao.CteRetRecepcao HSVRSCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            return HRSCTeRetRecepcao(cte, caminhoCert);
        }

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeRetRecepcao.CteRetRecepcao HSVSPCTeRetRecepcao(ConhecimentoTransporte cte, string caminhoCert)
        {
            return HSPCTeRetRecepcao(cte, caminhoCert);
        }

        #endregion

        #endregion

        #region Status

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        //public static wsHMTCTeStatus.CteStatusServico HMTCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        //{
        //    Glass.Data.wsHMTCTeStatus.CteStatusServico retorno = new Glass.Data.wsHMTCTeStatus.CteStatusServico();

        //    retorno.cteCabecMsgValue = new Glass.Data.wsHMTCTeStatus.cteCabecMsg();
        //    var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
        //    retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja).CodUf;
        //    retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

        //    return retorno;
        //}

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMSCTeStatus.CteStatusServico HMSCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMSCTeStatus.CteStatusServico retorno = new Glass.Data.wsHMSCTeStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
 
            retorno.cteCabecMsg = new Glass.Data.wsHMSCTeStatus.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMGCTeStatus.CteStatusServico HMGCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMGCTeStatus.CteStatusServico retorno = new Glass.Data.wsHMGCTeStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
      
            retorno.cteCabecMsgValue = new Glass.Data.wsHMGCTeStatus.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHPRCTeStatus.CteStatusServico HPRCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHPRCTeStatus.CteStatusServico retorno = new Glass.Data.wsHPRCTeStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
          
            retorno.cteCabecMsgValue = new Glass.Data.wsHPRCTeStatus.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCteStatus.CteStatusServico HRSCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHRSCteStatus.CteStatusServico retorno = new Glass.Data.wsHRSCteStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
        
            retorno.cteCabecMsgValue = new Glass.Data.wsHRSCteStatus.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeStatus.CteStatusServico HSPCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHSPCTeStatus.CteStatusServico retorno = new Glass.Data.wsHSPCTeStatus.CteStatusServico();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
        
            retorno.cteCabecMsgValue = new Glass.Data.wsHSPCTeStatus.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCteStatus.CteStatusServico HSVRSCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            return HRSCTeStatus(cte, caminhoCert);
        }

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeStatus.CteStatusServico HSVSPCTeStatus(ConhecimentoTransporte cte, string caminhoCert)
        {
            return HSPCTeStatus(cte, caminhoCert);
        }

        #endregion

        #endregion

        #region Cancelamento

        #region MT

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        //public static wsHMTCTeCancelamento.CteCancelamento HMTCTeCancelamento(ConhecimentoTransporte cte, string caminhoCert)
        //{
        //    Glass.Data.wsHMTCTeCancelamento.CteCancelamento retorno = new Glass.Data.wsHMTCTeCancelamento.CteCancelamento();

        //    retorno.cteCabecMsgValue = new Glass.Data.wsHMTCTeCancelamento.cteCabecMsg();
        //    var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
        //    retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja).CodUf;
        //    retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

        //    return retorno;
        //}

        #endregion

        #region MS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMSCTeCancelamento.CteCancelamento HMSCTeCancelamento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMSCTeCancelamento.CteCancelamento retorno = new Glass.Data.wsHMSCTeCancelamento.CteCancelamento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
         
            retorno.cteCabecMsg = new Glass.Data.wsHMSCTeCancelamento.CTeCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsg.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsg.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region MG

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHMGCTeCancelamento.CteCancelamento HMGCTeCancelamento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHMGCTeCancelamento.CteCancelamento retorno = new Glass.Data.wsHMGCTeCancelamento.CteCancelamento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
   
            retorno.cteCabecMsgValue = new Glass.Data.wsHMGCTeCancelamento.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region PR

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHPRCTeCancelamento.CteCancelamento HPRCTeCancelamento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHPRCTeCancelamento.CteCancelamento retorno = new Glass.Data.wsHPRCTeCancelamento.CteCancelamento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsHPRCTeCancelamento.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region RS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHRSCTeCancelamento.CteCancelamento HRSCTeCancelamento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHRSCTeCancelamento.CteCancelamento retorno = new Glass.Data.wsHRSCTeCancelamento.CteCancelamento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
     
            retorno.cteCabecMsgValue = new Glass.Data.wsHRSCTeCancelamento.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeCancelamento.CteCancelamento HSPCTeCancelamento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHSPCTeCancelamento.CteCancelamento retorno = new Glass.Data.wsHSPCTeCancelamento.CteCancelamento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
     
            retorno.cteCabecMsgValue = new Glass.Data.wsHSPCTeCancelamento.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSCTeCancelamento.CteRecepcaoEvento HSVRSCTeCancelamento(ConhecimentoTransporte cte, string caminhoCert)
        {
            Glass.Data.wsHSVRSCTeCancelamento.CteRecepcaoEvento retorno = new Glass.Data.wsHSVRSCTeCancelamento.CteRecepcaoEvento();

            // Define 100 segundos de espera, para evitar timeout
            retorno.Timeout = 100000;

            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            // Define o certificado a ser utilizado na comunicação (somente para A3)
            //retorno.ClientCertificates.Add(BuscarNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            retorno.cteCabecMsgValue = new Glass.Data.wsHSVRSCTeCancelamento.cteCabecMsg();
            var participante = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);
            retorno.cteCabecMsgValue.cUF = LojaDAO.Instance.GetElement(participante.IdLoja.Value).CodUf;
            retorno.cteCabecMsgValue.versaoDados = ConfigCTe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #region SVSP

        /// <summary>
        /// Retorna o WebService de consulta do cte. (Homologação)
        /// </summary>
        /// <param name="cte">O cte que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSPCTeCancelamento.CteCancelamento HSVSPCTeCancelamento(ConhecimentoTransporte cte, string caminhoCert)
        {
            return HSPCTeCancelamento(cte, caminhoCert);
        }

        #endregion

        #endregion

        #endregion
    }
}
