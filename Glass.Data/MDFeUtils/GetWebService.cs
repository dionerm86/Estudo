using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.NFeUtils;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Glass.Data.MDFeUtils
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

        #region Produção

        #region Recepção

        #region SVRS

        /// <summary>
        /// Retorna o WebService de Recepção do MDFe. (Produção)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSMDFeRecepcao.MDFeRecepcao PSVRSMDFeRecepcao(ManifestoEletronico manifestoEletronico, string caminhoCert)
        {
            wsPSVRSMDFeRecepcao.MDFeRecepcao retorno = new wsPSVRSMDFeRecepcao.MDFeRecepcao();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Recupera o participante para buscar o certificado e preenche a UF do emitente
            var participante = manifestoEletronico.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado((uint)participante.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.mdfeCabecMsgValue = new wsPSVRSMDFeRecepcao.mdfeCabecMsg();
            retorno.mdfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement((uint)participante.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoRecepcao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Retorno da Recepção

        #region SVRS

        /// <summary>
        /// Retorna o WebService de retorno da autorização do MDFe. (Produção)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSMDFeRetornoRecepcao.MDFeRetRecepcao PSVRSMDFeRetornoRecepcao(ManifestoEletronico manifestoEletronico, string caminhoCert)
        {
            wsPSVRSMDFeRetornoRecepcao.MDFeRetRecepcao retorno = new wsPSVRSMDFeRetornoRecepcao.MDFeRetRecepcao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Recupera o participante para buscar o certificado e preenche a UF do emitente
            var participante = manifestoEletronico.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado((uint)participante.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.mdfeCabecMsgValue = new wsPSVRSMDFeRetornoRecepcao.mdfeCabecMsg();
            retorno.mdfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement((uint)participante.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoRetornoRecepcao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Recepção Evento

        #region SVRS

        /// <summary>
        /// Retorna o WebService de recepção do MDFe. (Produção)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico do evento</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSMDFeRecepcaoEvento.MDFeRecepcaoEvento PSVRSMDFeRecepcaoEvento(ManifestoEletronico manifestoEletronico, string caminhoCert)
        {
            try
            {
                wsPSVRSMDFeRecepcaoEvento.MDFeRecepcaoEvento retorno = new wsPSVRSMDFeRecepcaoEvento.MDFeRecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Recupera o participante para buscar o certificado e preenche a UF do emitente
                var participante = manifestoEletronico.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado((uint)participante.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.mdfeCabecMsgValue = new wsPSVRSMDFeRecepcaoEvento.mdfeCabecMsg();
                retorno.mdfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement((uint)participante.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoRecepcaoEvento; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                //LogNfDAO.Instance.NewLog(manifestoEletronico.IdNf, "Instaciar Webservice", 3, Glass.MensagemAlerta.FormatErrorMsg("Falha ao instanciar webservice.", ex));
                return null;
            }
        }

        #endregion

        #endregion

        #region Consulta

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do MDFe. (Produção)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSMDFeConsulta.MDFeConsulta PSVRSMDFeConsulta(ManifestoEletronico manifestoEletronico, string caminhoCert)
        {
            wsPSVRSMDFeConsulta.MDFeConsulta retorno = new wsPSVRSMDFeConsulta.MDFeConsulta();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Recupera o participante para buscar o certificado e preenche a UF do emitente
            var participante = manifestoEletronico.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado((uint)participante.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.mdfeCabecMsgValue = new wsPSVRSMDFeConsulta.mdfeCabecMsg();
            retorno.mdfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement((uint)participante.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Status Serviço

        #region SVRS

        /// <summary>
        /// Retorna o WebService de status do MDFe. (Produção)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSMDFeStatusServico.MDFeStatusServico PSVRSMDFeStatusServico(ManifestoEletronico manifestoEletronico, string caminhoCert)
        {
            wsPSVRSMDFeStatusServico.MDFeStatusServico retorno = new wsPSVRSMDFeStatusServico.MDFeStatusServico();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Recupera o participante para buscar o certificado e preenche a UF do emitente
            var participante = manifestoEletronico.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado((uint)participante.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.mdfeCabecMsgValue = new wsPSVRSMDFeStatusServico.mdfeCabecMsg ();
            retorno.mdfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement((uint)participante.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Consulta Não Encerrado

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do MDFe não encerrado. (Produção)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsPSVRSMDFeConsultaNaoEncerrado.MDFeConsNaoEnc PSVRSMDFeConsultaNaoEncerrado(Loja lojaEmitente, string caminhoCert)
        {
            wsPSVRSMDFeConsultaNaoEncerrado.MDFeConsNaoEnc retorno = new wsPSVRSMDFeConsultaNaoEncerrado.MDFeConsNaoEnc();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado((uint)lojaEmitente.IdLoja, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.mdfeCabecMsgValue = new wsPSVRSMDFeConsultaNaoEncerrado.mdfeCabecMsg();
            retorno.mdfeCabecMsgValue.cUF = lojaEmitente.CodUf; // Cód UF do Emissor
            retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoConsultaNaoEncerrado; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #endregion

        #region Homologação

        #region Recepção

        #region SVRS

        /// <summary>
        /// Retorna o WebService de Recepção do MDFe. (Homologação)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSMDFeRecepcao.MDFeRecepcao HSVRSMDFeRecepcao(ManifestoEletronico manifestoEletronico, string caminhoCert)
        {
            wsHSVRSMDFeRecepcao.MDFeRecepcao retorno = new wsHSVRSMDFeRecepcao.MDFeRecepcao();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Recupera o participante para buscar o certificado e preenche a UF do emitente
            var participante = manifestoEletronico.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado((uint)participante.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.mdfeCabecMsgValue = new wsHSVRSMDFeRecepcao.mdfeCabecMsg();
            retorno.mdfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement((uint)participante.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoRecepcao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Retorno da Recepção

        #region SVRS

        /// <summary>
        /// Retorna o WebService de retorno da autorização do MDFe. (Homologação)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico que será enviado.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSMDFeRetornoRecepcao.MDFeRetRecepcao HSVRSMDFeRetornoRecepcao(ManifestoEletronico manifestoEletronico, string caminhoCert)
        {
            wsHSVRSMDFeRetornoRecepcao.MDFeRetRecepcao retorno = new wsHSVRSMDFeRetornoRecepcao.MDFeRetRecepcao();

            // Define 200 segundos de espera, para evitar timeout
            retorno.Timeout = 200000;

            // Recupera o participante para buscar o certificado e preenche a UF do emitente
            var participante = manifestoEletronico.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado((uint)participante.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.mdfeCabecMsgValue = new wsHSVRSMDFeRetornoRecepcao.mdfeCabecMsg();
            retorno.mdfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement((uint)participante.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoRetornoRecepcao; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Recepção Evento

        #region SVRS

        /// <summary>
        /// Retorna o WebService de recepção do MDFe. (Homologação)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico do evento</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSMDFeRecepcaoEvento.MDFeRecepcaoEvento HSVRSMDFeRecepcaoEvento(ManifestoEletronico manifestoEletronico, string caminhoCert)
        {
            try
            {
                wsHSVRSMDFeRecepcaoEvento.MDFeRecepcaoEvento retorno = new wsHSVRSMDFeRecepcaoEvento.MDFeRecepcaoEvento();

                // Define 200 segundos de espera, para evitar timeout
                retorno.Timeout = 200000;

                // Recupera o participante para buscar o certificado e preenche a UF do emitente
                var participante = manifestoEletronico.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();

                // Define o certificado a ser utilizado na comunicação
                retorno.ClientCertificates.Add(GetCertificado((uint)participante.IdLoja.Value, caminhoCert));

                // Monta o cabeçalho do SOAP
                retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                retorno.mdfeCabecMsgValue = new wsHSVRSMDFeRecepcaoEvento.mdfeCabecMsg();
                retorno.mdfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement((uint)participante.IdLoja.Value).CodUf; // Cód UF do Emissor
                retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoRecepcaoEvento; // Versão da mensagem (lote) envelopada no SOAP

                return retorno;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #endregion

        #region Consulta

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do MDFe. (Homologação)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSMDFeConsulta.MDFeConsulta HSVRSMDFeConsulta(ManifestoEletronico manifestoEletronico, string caminhoCert)
        {
            wsHSVRSMDFeConsulta.MDFeConsulta retorno = new wsHSVRSMDFeConsulta.MDFeConsulta();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Recupera o participante para buscar o certificado e preenche a UF do emitente
            var participante = manifestoEletronico.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado((uint)participante.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.mdfeCabecMsgValue = new wsHSVRSMDFeConsulta.mdfeCabecMsg();
            retorno.mdfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement((uint)participante.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoConsulta; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Status Serviço

        #region SVRS

        /// <summary>
        /// Retorna o WebService de status do MDFe. (Homologação)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSMDFeStatusServico.MDFeStatusServico HSVRSMDFeStatusServico(ManifestoEletronico manifestoEletronico, string caminhoCert)
        {
            wsHSVRSMDFeStatusServico.MDFeStatusServico retorno = new wsHSVRSMDFeStatusServico.MDFeStatusServico();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Recupera o participante para buscar o certificado e preenche a UF do emitente
            var participante = manifestoEletronico.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado((uint)participante.IdLoja.Value, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.mdfeCabecMsgValue = new wsHSVRSMDFeStatusServico.mdfeCabecMsg();
            retorno.mdfeCabecMsgValue.cUF = LojaDAO.Instance.GetElement((uint)participante.IdLoja.Value).CodUf; // Cód UF do Emissor
            retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoStatusServico; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #region Consulta Não Encerrado

        #region SVRS

        /// <summary>
        /// Retorna o WebService de consulta do MDFe não encerrado. (Homologação)
        /// </summary>
        /// <param name="manifestoEletronico">O Manifesto Eletronico que será enviada.</param>
        /// <param name="caminhoCert">O caminho da pasta que contém o certificado. Pode ser null ou vazio para usar a pasta padrão.</param>
        /// <returns></returns>
        public static wsHSVRSMDFeConsultaNaoEncerrado.MDFeConsNaoEnc HSVRSMDFeConsultaNaoEncerrado(Loja lojaEmitente, string caminhoCert)
        {
            wsHSVRSMDFeConsultaNaoEncerrado.MDFeConsNaoEnc retorno = new wsHSVRSMDFeConsultaNaoEncerrado.MDFeConsNaoEnc();

            // Define 90 segundos de espera, para evitar timeout
            retorno.Timeout = 90000;

            // Define o certificado a ser utilizado na comunicação
            retorno.ClientCertificates.Add(GetCertificado((uint)lojaEmitente.IdLoja, caminhoCert));

            // Monta o cabeçalho do SOAP
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            retorno.mdfeCabecMsgValue = new wsHSVRSMDFeConsultaNaoEncerrado.mdfeCabecMsg();
            retorno.mdfeCabecMsgValue.cUF = lojaEmitente.CodUf; // Cód UF do Emissor
            retorno.mdfeCabecMsgValue.versaoDados = ConfigMDFe.VersaoConsultaNaoEncerrado; // Versão da mensagem envelopada no SOAP

            return retorno;
        }

        #endregion

        #endregion

        #endregion
    }
}
