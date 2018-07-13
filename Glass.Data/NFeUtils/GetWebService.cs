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

        private static X509Certificate2 GetCertificado(uint idLoja, string certPath)
        {
            return !string.IsNullOrEmpty(certPath) ? Certificado.GetCertificado(idLoja, certPath) : Certificado.GetCertificado(idLoja);
        }

        #endregion

        #region Consulta Cadastro

        internal static string ObterServidorConsultaCadastro(string uf)
        {
            switch (uf.ToUpper())
            {
                default: return uf.ToUpper();
            }
        }

        public static wsPConsultaCadastro.CadConsultaCadastro4 ConsultaCadastroProducao(string uf)
        {
            var retorno = new wsPConsultaCadastro.CadConsultaCadastro4();

            switch (ObterServidorConsultaCadastro(uf))
            {
                case "BA": retorno.Url = "https://nfe.sefaz.ba.gov.br/webservices/CadConsultaCadastro4/CadConsultaCadastro4.asmx?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/CadConsultaCadastro4?wsdl"; break;
                case "MS": retorno.Url = "https://nfe.fazenda.ms.gov.br/ws/CadConsultaCadastro4?wsdl"; break;
                case "MT": retorno.Url = "https://nfe.sefaz.mt.gov.br/nfews/v2/services/CadConsultaCadastro4?wsdl"; break;
                case "PR": retorno.Url = "https://nfe.sefa.pr.gov.br/nfe/CadConsultaCadastro4?wsdl"; break;
                case "SP": retorno.Url = "https://nfe.fazenda.sp.gov.br/ws/cadconsultacadastro4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static wsHConsultaCadastro.CadConsultaCadastro4 ConsultaCadastroHomologacao(string uf)
        {
            var retorno = new wsHConsultaCadastro.CadConsultaCadastro4();

            switch (ObterServidorConsultaCadastro(uf))
            {
                case "BA": retorno.Url = "https://hnfe.sefaz.ba.gov.br/webservices/CadConsultaCadastro4/CadConsultaCadastro4.asmx?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/CadConsultaCadastro4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfe.sefaz.ms.gov.br/ws/CadConsultaCadastro4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfews/v2/services/CadConsultaCadastro4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfe.sefa.pr.gov.br/nfe/CadConsultaCadastro4?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfe.fazenda.sp.gov.br/ws/cadconsultacadastro4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(UserInfo.GetUserInfo.IdLoja, null));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region NF-e

        internal static string ObterServidorAutorizadorNFe(NotaFiscal notaFiscal, string uf)
        {
            if (notaFiscal.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaSVCRS && notaFiscal.FormaEmissao != (int)NotaFiscal.TipoEmissao.ContingenciaSVCAN)
            {
                switch (uf.ToUpper())
                {
                    case "MA":
                    case "PA": return "SVAN";
                    case "AC":
                    case "AL":
                    case "AP":
                    case "DF":
                    case "ES":
                    case "PB":
                    case "PI":
                    case "RJ":
                    case "RN":
                    case "RO":
                    case "RR":
                    case "SC":
                    case "SE":
                    case "TO": return "SVRS";
                    default: return uf.ToUpper();
                }
            }
            else
            {
                switch (uf.ToUpper())
                {
                    case "AC":
                    case "AL":
                    case "AP":
                    case "DF":
                    case "ES":
                    case "MG":
                    case "PB":
                    case "RJ":
                    case "RN":
                    case "RO":
                    case "RR":
                    case "RS":
                    case "SC":
                    case "SE":
                    case "SP":
                    case "TO": return "SVCAN";
                    case "AM":
                    case "BA":
                    case "CE":
                    case "GO":
                    case "MA":
                    case "MS":
                    case "MT":
                    case "PA":
                    case "PE":
                    case "PI":
                    case "PR": return "SVCRS";
                    default: return uf.ToUpper();
                }
            }
        }

        #region Autorização

        public static IAutorizacao NFeAutorizacaoProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IAutorizacao retorno = new wsPNFeAutorizacao.NFeAutorizacao4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfe.sefaz.am.gov.br/services2/services/NfeAutorizacao4?wsdl"; break;
                case "BA": retorno.Url = "https://nfe.sefaz.ba.gov.br/webservices/NFeAutorizacao4/NFeAutorizacao4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfe.sefaz.ce.gov.br/nfe4/services/NFeAutorizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeAutorizacao4?wsdl"; break;
                case "MG": retorno.Url = "https://nfe.fazenda.mg.gov.br/nfe2/services/NFeAutorizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://nfe.fazenda.ms.gov.br/ws/NFeAutorizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeAutorizacao4?wsdl"; break;
                case "PE": retorno.Url = "https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeAutorizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://nfe.sefa.pr.gov.br/nfe/NFeAutorizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfe.fazenda.sp.gov.br/ws/nfeautorizacao4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANAutorizacao.NFeAutorizacao4();
                        retorno.Url = "https://www.sefazvirtual.fazenda.gov.br/NFeAutorizacao4/NFeAutorizacao4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?wsdl"; break;
                case "SVCAN": retorno.Url = "https://www.svc.fazenda.gov.br/NFeAutorizacao4/NFeAutorizacao4.asmx?wsdl"; break;
                case "SVCRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static IAutorizacao NFeAutorizacaoHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IAutorizacao retorno = new wsHNFeAutorizacao.NFeAutorizacao4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfe.sefaz.am.gov.br/services2/services/NfeAutorizacao4?wsdl"; break;
                case "BA": retorno.Url = "https://hnfe.sefaz.ba.gov.br/webservices/NFeAutorizacao4/NFeAutorizacao4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeAutorizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeAutorizacao4?wsdl"; break;
                case "MG": retorno.Url = "https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeAutorizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfe.sefaz.ms.gov.br/ws/NFeAutorizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfews/v2/services/NfeAutorizacao4?wsdl"; break;
                case "PE": retorno.Url = "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeAutorizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeAutorizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfe.fazenda.sp.gov.br/ws/nfeautorizacao4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANAutorizacao.NFeAutorizacao4();
                        retorno.Url = "https://hom.sefazvirtual.fazenda.gov.br/NFeAutorizacao4/NFeAutorizacao4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?wsdl"; break;
                case "SVCAN": retorno.Url = "https://hom.svc.fazenda.gov.br/NFeAutorizacao4/NFeAutorizacao4.asmx?wsdl"; break;
                case "SVCRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region Consulta protocolo

        public static IConsultaProtocolo NFeConsultaProtocoloProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IConsultaProtocolo retorno = new wsPNFeConsultaProtocolo.NFeConsultaProtocolo4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfe.sefaz.am.gov.br/services2/services/NfeConsulta4?wsdl"; break;
                case "BA": retorno.Url = "https://nfe.sefaz.ba.gov.br/webservices/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfe.sefaz.ce.gov.br/nfe4/services/NFeConsultaProtocolo4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeConsultaProtocolo4?wsdl"; break;
                case "MG": retorno.Url = "https://nfe.fazenda.mg.gov.br/nfe2/services/NFeConsultaProtocolo4?wsdl"; break;
                case "MS": retorno.Url = "https://nfe.fazenda.ms.gov.br/ws/NFeConsultaProtocolo4?wsdl"; break;
                case "MT": retorno.Url = "https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeConsulta4?wsdl"; break;
                case "PE": retorno.Url = "https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeConsultaProtocolo4?wsdl"; break;
                case "PR": retorno.Url = "https://nfe.sefa.pr.gov.br/nfe/NFeConsultaProtocolo4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfe.fazenda.sp.gov.br/ws/nfeconsultaprotocolo4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANConsultaProtocolo.NFeConsultaProtocolo4();
                        retorno.Url = "https://www.sefazvirtual.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?wsdl"; break;
                case "SVCAN": retorno.Url = "https://www.svc.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx?wsdl"; break;
                case "SVCRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static IConsultaProtocolo NFeConsultaProtocoloHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IConsultaProtocolo retorno = new wsHNFeConsultaProtocolo.NFeConsultaProtocolo4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfe.sefaz.am.gov.br/services2/services/NfeConsulta4?wsdl"; break;
                case "BA": retorno.Url = "https://hnfe.sefaz.ba.gov.br/webservices/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeConsultaProtocolo4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeConsultaProtocolo4?wsdl"; break;
                case "MG": retorno.Url = "https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeConsultaProtocolo4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfe.sefaz.ms.gov.br/ws/NFeConsultaProtocolo4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfews/v2/services/NfeConsulta4?wsdl"; break;
                case "PE": retorno.Url = "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeConsultaProtocolo4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeConsultaProtocolo4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfe.fazenda.sp.gov.br/ws/nfeconsultaprotocolo4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANConsultaProtocolo.NFeConsultaProtocolo4();
                        retorno.Url = "https://hom.sefazvirtual.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?wsdl"; break;
                case "SVCAN": retorno.Url = "https://hom.svc.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx?wsdl"; break;
                case "SVCRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region Inutilização

        public static IInutilizacao NFeInutilizacaoProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IInutilizacao retorno = new wsPNFeInutilizacao.NFeInutilizacao4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfe.sefaz.am.gov.br/services2/services/NfeInutilizacao4?wsdl"; break;
                case "BA": retorno.Url = "https://nfe.sefaz.ba.gov.br/webservices/NFeInutilizacao4/NFeInutilizacao4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfe.sefaz.ce.gov.br/nfe4/services/NFeInutilizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeInutilizacao4?wsdl"; break;
                case "MG": retorno.Url = "https://nfe.fazenda.mg.gov.br/nfe2/services/NFeInutilizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://nfe.fazenda.ms.gov.br/ws/NFeInutilizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeInutilizacao4?wsdl"; break;
                case "PE": retorno.Url = "https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeInutilizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://nfe.sefa.pr.gov.br/nfe/NFeInutilizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfe.fazenda.sp.gov.br/ws/nfeinutilizacao4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANInutilizacao.NFeInutilizacao4();
                        retorno.Url = "https://www.sefazvirtual.fazenda.gov.br/NFeInutilizacao4/NFeInutilizacao4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static IInutilizacao NFeInutilizacaoHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IInutilizacao retorno = new wsHNFeInutilizacao.NFeInutilizacao4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfe.sefaz.am.gov.br/services2/services/NfeInutilizacao4?wsdl"; break;
                case "BA": retorno.Url = "https://hnfe.sefaz.ba.gov.br/webservices/NFeInutilizacao4/NFeInutilizacao4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeInutilizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeInutilizacao4?wsdl"; break;
                case "MG": retorno.Url = "https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeInutilizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfe.sefaz.ms.gov.br/ws/NFeInutilizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfews/v2/services/NfeInutilizacao4?wsdl"; break;
                case "PE": retorno.Url = "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeInutilizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeInutilizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe-homologacao.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfe.fazenda.sp.gov.br/ws/nfeinutilizacao4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANInutilizacao.NFeInutilizacao4();
                        retorno.Url = "https://hom.sefazvirtual.fazenda.gov.br/NFeInutilizacao4/NFeInutilizacao4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region Recepção evento

        public static IRecepcaoEvento NFeRecepcaoEventoProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IRecepcaoEvento retorno = new wsPNFeRecepcaoEvento.NFeRecepcaoEvento4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfe.sefaz.am.gov.br/services2/services/RecepcaoEvento4?wsdl"; break;
                case "BA": retorno.Url = "https://nfe.sefaz.ba.gov.br/webservices/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfe.sefaz.ce.gov.br/nfe4/services/NFeRecepcaoEvento4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeRecepcaoEvento4?wsdl"; break;
                case "MG": retorno.Url = "https://nfe.fazenda.mg.gov.br/nfe2/services/NFeRecepcaoEvento4?wsdl"; break;
                case "MS": retorno.Url = "https://nfe.fazenda.ms.gov.br/ws/NFeRecepcaoEvento4?wsdl"; break;
                case "MT": retorno.Url = "https://nfe.sefaz.mt.gov.br/nfews/v2/services/RecepcaoEvento4?wsdl"; break;
                case "PE": retorno.Url = "https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeRecepcaoEvento4?wsdl"; break;
                case "PR": retorno.Url = "https://nfe.sefa.pr.gov.br/nfe/NFeRecepcaoEvento4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfe.fazenda.sp.gov.br/ws/nferecepcaoevento4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANRecepcaoEvento.NFeRecepcaoEvento4();
                        retorno.Url = "https://www.sefazvirtual.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?wsdl"; break;
                case "SVCAN": retorno.Url = "https://www.svc.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx?wsdl"; break;
                case "SVCRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static IRecepcaoEvento NFeRecepcaoEventoHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IRecepcaoEvento retorno = new wsHNFeRecepcaoEvento.NFeRecepcaoEvento4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfe.sefaz.am.gov.br/services2/services/RecepcaoEvento4?wsdl"; break;
                case "BA": retorno.Url = "https://hnfe.sefaz.ba.gov.br/webservices/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeRecepcaoEvento4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeRecepcaoEvento4?wsdl"; break;
                case "MG": retorno.Url = "https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeRecepcaoEvento4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfe.sefaz.ms.gov.br/ws/NFeRecepcaoEvento4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfews/v2/services/RecepcaoEvento4?wsdl"; break;
                case "PE": retorno.Url = "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeRecepcaoEvento4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeRecepcaoEvento4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe-homologacao.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfe.fazenda.sp.gov.br/ws/nferecepcaoevento4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANRecepcaoEvento.NFeRecepcaoEvento4();
                        retorno.Url = "https://hom.sefazvirtual.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?wsdl"; break;
                case "SVCAN": retorno.Url = "https://hom.svc.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx?wsdl"; break;
                case "SVCRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region Retorno autorização

        public static IRetornoAutorizacao NFeRetornoAutorizacaoProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IRetornoAutorizacao retorno = new wsPNFeRetAutorizacao.NFeRetAutorizacao4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfe.sefaz.am.gov.br/services2/services/NfeRetAutorizacao4?wsdl"; break;
                case "BA": retorno.Url = "https://nfe.sefaz.ba.gov.br/webservices/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfe.sefaz.ce.gov.br/nfe4/services/NFeRetAutorizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeRetAutorizacao4?wsdl"; break;
                case "MG": retorno.Url = "https://nfe.fazenda.mg.gov.br/nfe2/services/NFeRetAutorizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://nfe.fazenda.ms.gov.br/ws/NFeRetAutorizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeRetAutorizacao4?wsdl"; break;
                case "PE": retorno.Url = "https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeRetAutorizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://nfe.sefa.pr.gov.br/nfe/NFeRetAutorizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfe.fazenda.sp.gov.br/ws/nferetautorizacao4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANRetAutorizacao.NFeRetAutorizacao4();
                        retorno.Url = "https://www.sefazvirtual.fazenda.gov.br/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "SVCAN": retorno.Url = "https://www.svc.fazenda.gov.br/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "SVCRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static IRetornoAutorizacao NFeRetornoAutorizacaoHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IRetornoAutorizacao retorno = new wsHNFeRetAutorizacao.NFeRetAutorizacao4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfe.sefaz.am.gov.br/services2/services/NfeRetAutorizacao4?wsdl"; break;
                case "BA": retorno.Url = "https://hnfe.sefaz.ba.gov.br/webservices/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeRetAutorizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeRetAutorizacao4?wsdl"; break;
                case "MG": retorno.Url = "https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeRetAutorizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfe.sefaz.ms.gov.br/ws/NFeRetAutorizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfews/v2/services/NfeRetAutorizacao4?wsdl"; break;
                case "PE": retorno.Url = "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeRetAutorizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeRetAutorizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfe.fazenda.sp.gov.br/ws/nferetautorizacao4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANRetAutorizacao.NFeRetAutorizacao4();
                        retorno.Url = "https://hom.sefazvirtual.fazenda.gov.br/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "SVCAN": retorno.Url = "https://hom.svc.fazenda.gov.br/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "SVCRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region Status serviço

        public static IStatusServico NFeStatusServicoProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IStatusServico retorno = new wsPNFeStatusServico.NFeStatusServico4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfe.sefaz.am.gov.br/services2/services/NfeStatusServico4?wsdl"; break;
                case "BA": retorno.Url = "https://nfe.sefaz.ba.gov.br/webservices/NFeStatusServico4/NFeStatusServico4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfe.sefaz.ce.gov.br/nfe4/services/NFeStatusServico4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeStatusServico4?wsdl"; break;
                case "MG": retorno.Url = "https://nfe.fazenda.mg.gov.br/nfe2/services/NFeStatusServico4?wsdl"; break;
                case "MS": retorno.Url = "https://nfe.fazenda.ms.gov.br/ws/NFeStatusServico4?wsdl"; break;
                case "MT": retorno.Url = "https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeStatusServico4?wsdl"; break;
                case "PE": retorno.Url = "https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeStatusServico4?wsdl"; break;
                case "PR": retorno.Url = "https://nfe.sefa.pr.gov.br/nfe/NFeStatusServico4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe.sefazrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfe.fazenda.sp.gov.br/ws/nfestatusservico4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANStatus.NFeStatusServico4();
                        retorno.Url = "https://www.sefazvirtual.fazenda.gov.br/NFeStatusServico4/NFeStatusServico4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx?wsdl"; break;
                case "SVCAN": retorno.Url = "https://www.svc.fazenda.gov.br/NFeStatusServico4/NFeStatusServico4.asmx?wsdl"; break;
                case "SVCRS": retorno.Url = "https://nfe.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static IStatusServico NFeStatusServicoHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            IStatusServico retorno = new wsHNFeStatusServico.NFeStatusServico4();

            switch (ObterServidorAutorizadorNFe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfe.sefaz.am.gov.br/services2/services/NfeStatusServico4?wsdl"; break;
                case "BA": retorno.Url = "https://hnfe.sefaz.ba.gov.br/webservices/NFeStatusServico4/NFeStatusServico4.asmx?wsdl"; break;
                case "CE": retorno.Url = "https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeStatusServico4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeStatusServico4?wsdl"; break;
                case "MG": retorno.Url = "https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeStatusServico4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfe.sefaz.ms.gov.br/ws/NFeStatusServico4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfews/v2/services/NfeStatusServico4?wsdl"; break;
                case "PE": retorno.Url = "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeStatusServico4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeStatusServico4?wsdl"; break;
                case "RS": retorno.Url = "https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfe.fazenda.sp.gov.br/ws/nfestatusservico4.asmx?wsdl"; break;
                case "SVAN":
                    {
                        retorno = new wsNFeSVANStatus.NFeStatusServico4();
                        retorno.Url = "https://hom.sefazvirtual.fazenda.gov.br/NFeStatusServico4/NFeStatusServico4.asmx?wsdl";
                        break;
                    }
                case "SVRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx?wsdl"; break;
                case "SVCAN": retorno.Url = "https://hom.svc.fazenda.gov.br/NFeStatusServico4/NFeStatusServico4.asmx?wsdl"; break;
                case "SVCRS": retorno.Url = "https://nfe-homologacao.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #endregion

        #region NFC-e

        internal static string ObterServidorAutorizadorNFCe(NotaFiscal notaFiscal, string uf)
        {
            switch (uf.ToUpper())
            {
                case "AC":
                case "AL":
                case "AP":
                case "BA":
                case "CE":
                case "DF":
                case "ES":
                case "MA":
                case "MG":
                case "PA":
                case "PB":
                case "PE":
                case "PI":
                case "RJ":
                case "RN":
                case "RO":
                case "RR":
                case "SC":
                case "SE":
                case "TO": return "SVRS";
                default: return uf.ToUpper();
            }
        }

        #region Autorização

        public static wsPNFCeAutorizacao.NFeAutorizacao4 NFCeAutorizacaoProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsPNFCeAutorizacao.NFeAutorizacao4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfce.sefaz.am.gov.br/nfce-services/services/NfeAutorizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeAutorizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://nfce.fazenda.ms.gov.br/ws/NFeAutorizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeAutorizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://nfce.sefa.pr.gov.br/nfce/NFeAutorizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfce.fazenda.sp.gov.br/ws/NFeAutorizacao4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static wsHNFCeAutorizacao.NFeAutorizacao4 NFCeAutorizacaoHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsHNFCeAutorizacao.NFeAutorizacao4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfce.sefaz.am.gov.br/nfce-services/services/NfeAutorizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeAutorizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfce.sefaz.ms.gov.br/ws/NFeAutorizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeAutorizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeAutorizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeAutorizacao4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region Consulta protocolo

        public static wsPNFCeConsultaProtocolo.NFeConsultaProtocolo4 NFCeConsultaProtocoloProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsPNFCeConsultaProtocolo.NFeConsultaProtocolo4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfce.sefaz.am.gov.br/nfce-services/services/NfeConsulta4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeConsultaProtocolo4?wsdl"; break;
                case "MS": retorno.Url = "https://nfce.fazenda.ms.gov.br/ws/NFeConsultaProtocolo4?wsdl"; break;
                case "MT": retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeConsulta4?wsdl"; break;
                case "PR": retorno.Url = "https://nfce.sefa.pr.gov.br/nfce/NFeConsultaProtocolo4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfce.fazenda.sp.gov.br/ws/NFeConsultaProtocolo4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static wsHNFCeConsultaProtocolo.NFeConsultaProtocolo4 NFCeConsultaProtocoloHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsHNFCeConsultaProtocolo.NFeConsultaProtocolo4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfce.sefaz.am.gov.br/nfce-services/services/NfeConsulta4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeConsultaProtocolo4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfce.sefaz.ms.gov.br/ws/NFeConsultaProtocolo4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeConsulta4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeConsultaProtocolo4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeConsultaProtocolo4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region Inutilização

        public static wsPNFCeInutilizacao.NFeInutilizacao4 NFCeInutilizacaoProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsPNFCeInutilizacao.NFeInutilizacao4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfce.sefaz.am.gov.br/nfce-services/services/NfeInutilizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeInutilizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://nfce.fazenda.ms.gov.br/ws/NFeInutilizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeInutilizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://nfce.sefa.pr.gov.br/nfce/NFeInutilizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfce.fazenda.sp.gov.br/ws/NFeInutilizacao4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static wsHNFCeInutilizacao.NFeInutilizacao4 NFCeInutilizacaoHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsHNFCeInutilizacao.NFeInutilizacao4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfce.sefaz.am.gov.br/nfce-services/services/NfeInutilizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeInutilizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfce.sefaz.ms.gov.br/ws/NFeInutilizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeInutilizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeInutilizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeInutilizacao4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region Recepção evento

        public static wsPNFCeRecepcaoEvento.NFeRecepcaoEvento4 NFCeRecepcaoEventoProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsPNFCeRecepcaoEvento.NFeRecepcaoEvento4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfce.sefaz.am.gov.br/nfce-services/services/RecepcaoEvento4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeRecepcaoEvento4?wsdl"; break;
                case "MS": retorno.Url = "https://nfce.fazenda.ms.gov.br/ws/NFeRecepcaoEvento4?wsdl"; break;
                case "MT": retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/RecepcaoEvento4?wsdl"; break;
                case "PR": retorno.Url = "https://nfce.sefa.pr.gov.br/nfce/NFeRecepcaoEvento4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfce.fazenda.sp.gov.br/ws/NFeRecepcaoEvento4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static wsHNFCeRecepcaoEvento.NFeRecepcaoEvento4 NFCeRecepcaoEventoHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsHNFCeRecepcaoEvento.NFeRecepcaoEvento4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfce.sefaz.am.gov.br/nfce-services/services/RecepcaoEvento4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeRecepcaoEvento4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfce.sefaz.ms.gov.br/ws/NFeRecepcaoEvento4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/RecepcaoEvento4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeRecepcaoEvento4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeRecepcaoEvento4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region Retorno autorização

        public static wsPNFCeRetAutorizacao.NFeRetAutorizacao4 NFCeRetornoAutorizacaoProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsPNFCeRetAutorizacao.NFeRetAutorizacao4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfce.sefaz.am.gov.br/nfce-services/services/NfeRetAutorizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeRetAutorizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://nfce.fazenda.ms.gov.br/ws/NFeRetAutorizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeRetAutorizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://nfce.sefa.pr.gov.br/nfce/NFeRetAutorizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfce.fazenda.sp.gov.br/ws/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static wsHNFCeRetAutorizacao.NFeRetAutorizacao4 NFCeRetornoAutorizacaoHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsHNFCeRetAutorizacao.NFeRetAutorizacao4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfce.sefaz.am.gov.br/nfce-services/services/NfeRetAutorizacao4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeRetAutorizacao4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfce.sefaz.ms.gov.br/ws/NFeRetAutorizacao4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeRetAutorizacao4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeRetAutorizacao4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeRetAutorizacao4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region Status serviço

        public static wsPNFCeStatusServico.NFeStatusServico4 NFCeStatusServicoProducao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsPNFCeStatusServico.NFeStatusServico4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://nfce.sefaz.am.gov.br/nfce-services/services/NfeStatusServico4?wsdl"; break;
                case "GO": retorno.Url = "https://nfe.sefaz.go.gov.br/nfe/services/NFeStatusServico4?wsdl"; break;
                case "MS": retorno.Url = "https://nfce.fazenda.ms.gov.br/ws/NFeStatusServico4?wsdl"; break;
                case "MT": retorno.Url = "https://nfce.sefaz.mt.gov.br/nfcews/services/NfeStatusServico4?wsdl"; break;
                case "PR": retorno.Url = "https://nfce.sefa.pr.gov.br/nfce/NFeStatusServico4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce.sefazrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://nfce.fazenda.sp.gov.br/ws/NFeStatusServico4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        public static wsHNFCeStatusServico.NFeStatusServico4 NFCeStatusServicoHomologacao(NotaFiscal notaFiscal, string caminhoCert, string uf)
        {
            var retorno = new wsHNFCeStatusServico.NFeStatusServico4();

            switch (ObterServidorAutorizadorNFCe(notaFiscal, uf))
            {
                case "AM": retorno.Url = "https://homnfce.sefaz.am.gov.br/nfce-services/services/NfeStatusServico4?wsdl"; break;
                case "GO": retorno.Url = "https://homolog.sefaz.go.gov.br/nfe/services/NFeStatusServico4?wsdl"; break;
                case "MS": retorno.Url = "https://hom.nfce.sefaz.ms.gov.br/ws/NFeStatusServico4?wsdl"; break;
                case "MT": retorno.Url = "https://homologacao.sefaz.mt.gov.br/nfcews/services/NfeStatusServico4?wsdl"; break;
                case "PR": retorno.Url = "https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeStatusServico4?wsdl"; break;
                case "RS": retorno.Url = "https://nfce-homologacao.sefazrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx?wsdl"; break;
                case "SP": retorno.Url = "https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeStatusServico4.asmx?wsdl"; break;
                case "SVRS": retorno.Url = "https://nfce-homologacao.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx?wsdl"; break;
                default: retorno.Url = string.Empty; break;
            }

            retorno.Timeout = 200000;
            retorno.ClientCertificates.Add(GetCertificado(notaFiscal.IdLoja.Value, caminhoCert));
            retorno.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;

            return retorno;
        }

        #endregion

        #region QR Code

        public static string UrlQrCode(string uf, ConfigNFe.TipoAmbienteNfe tipoAmbiente)
        {
            if (tipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                switch (uf.ToUpper())
                {
                    case "AC": return "http://www.sefaznet.ac.gov.br/nfce/qrcode?";
                    case "AL": return "http://nfce.sefaz.al.gov.br/QRCode/consultarNFCe.jsp?";
                    case "AM": return "http://sistemas.sefaz.am.gov.br/nfceweb/consultarNFCe.jsp?";
                    case "AP": return "https://www.sefaz.ap.gov.br/nfce/nfcep.php?";
                    case "BA": return "http://nfe.sefaz.ba.gov.br/servicos/nfce/modulos/geral/NFCEC_consulta_chave_acesso.aspx?";
                    case "DF": return "http://dec.fazenda.df.gov.br/ConsultarNFCe.aspx?";
                    case "ES": return "http://app.sefaz.es.gov.br/ConsultaNFCe?";
                    case "GO": return "http://nfe.sefaz.go.gov.br/nfeweb/sites/nfce/danfeNFCe?";
                    case "MA": return "http://nfce.sefaz.ma.gov.br/portal/consultarNFCe.jsp?";
                    case "MS": return "http://www.dfe.ms.gov.br/nfce/qrcode?";
                    case "MT": return "http://www.sefaz.mt.gov.br/nfce/consultanfce?";
                    case "PA": return "https://appnfc.sefa.pa.gov.br/portal/view/consultas/nfce/nfceForm.seam?";
                    case "PB": return "http://www.receita.pb.gov.br/nfce?";
                    case "PE": return "http://nfce.sefaz.pe.gov.br/nfce-web/consultarNFCe?";
                    case "PI": return "http://webas.sefaz.pi.gov.br/nfceweb/consultarNFCe.jsf?";
                    case "PR": return "http://www.fazenda.pr.gov.br/nfce/qrcode?";
                    case "RJ": return "http://www4.fazenda.rj.gov.br/consultaNFCe/QRCode?";
                    case "RN": return "http://nfce.set.rn.gov.br/consultarNFCe.aspx?";
                    case "RO": return "http://www.nfce.sefin.ro.gov.br/consultanfce/consulta.jsp?";
                    case "RR": return "https://www.sefaz.rr.gov.br/nfce/servlet/qrcode?";
                    case "RS": return "https://www.sefaz.rs.gov.br/NFCE/NFCE-COM.aspx?";
                    case "SE": return "http://www.nfce.se.gov.br/portal/consultarNFCe.jsp?";
                    case "SP": return "https://www.nfce.fazenda.sp.gov.br/NFCeConsultaPublica/Paginas/ConsultaQRCode.aspx?";
                    case "TO": return "http://apps.sefaz.to.gov.br/portal-nfce/qrcodeNFCe?";
                }
            }
            else if (tipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                switch (uf)
                {
                    case "AC": return "http://www.hml.sefaznet.ac.gov.br/nfce/qrcode?";
                    case "AL": return "http://nfce.sefaz.al.gov.br/QRCode/consultarNFCe.jsp?";
                    case "AM": return "http://homnfce.sefaz.am.gov.br/nfceweb/consultarNFCe.jsp?";
                    case "AP": return "https://www.sefaz.ap.gov.br/nfcehml/nfce.php?";
                    case "BA": return "http://hnfe.sefaz.ba.gov.br/servicos/nfce/modulos/geral/NFCEC_consulta_chave_acesso.aspx?";
                    case "CE": return "http://nfceh.sefaz.ce.gov.br/pages/ShowNFCe.html?";
                    case "DF": return "http://dec.fazenda.df.gov.br/ConsultarNFCe.aspx?";
                    case "ES": return "http://homologacao.sefaz.es.gov.br/ConsultaNFCe/qrcode.aspx?";
                    case "GO": return "http://homolog.sefaz.go.gov.br/nfeweb/sites/nfce/danfeNFCe?";
                    case "MA": return "http://homologacao.sefaz.ma.gov.br/portal/consultarNFCe.jsp?";
                    case "MT": return "http://homologacao.sefaz.mt.gov.br/nfce/consultanfce?";
                    case "MS": return "http://www.dfe.ms.gov.br/nfce/qrcode?";
                    case "PA": return "https://appnfc.sefa.pa.gov.br/portal-homologacao/view/consultas/nfce/nfceForm.seam?";
                    case "PB": return "http://www.receita.pb.gov.br/nfcehom?";
                    case "PR": return "http://www.fazenda.pr.gov.br/nfce/qrcode?";
                    case "PE": return "http://nfcehomolog.sefaz.pe.gov.br/nfce-web/consultarNFCe?";
                    case "PI": return "http://webas.sefaz.pi.gov.br/nfceweb-homologacao/consultarNFCe.jsf?";
                    case "RJ": return "http://www4.fazenda.rj.gov.br/consultaNFCe/QRCode?";
                    case "RN": return "http://hom.nfce.set.rn.gov.br/consultarNFCe.aspx?";
                    case "RS": return "https://www.sefaz.rs.gov.br/NFCE/NFCE-COM.aspx?";
                    case "RO": return "http://www.nfce.sefin.ro.gov.br/consultanfce/consulta.jsp?";
                    case "RR": return "http://200.174.88.103:8080/nfce/servlet/qrcode?";
                    case "SP": return "https://www.homologacao.nfce.fazenda.sp.gov.br/NFCeConsultaPublica/Paginas/ConsultaQRCode.aspx?";
                    case "SE": return "http://www.hom.nfe.se.gov.br/portal/consultarNFCe.jsp?";
                    case "TO": return "http://apps.sefaz.to.gov.br/portal-nfce-homologacao/qrcodeNFCe?";
                }
            }

            throw new Exception($"Endereço QR Code não mapeado para o estado { uf.ToUpper() }.");
        }

        #endregion

        #region Url Consulta por Chave de Acesso

        public static string UrlConsultaPorChaveAcesso(string uf, ConfigNFe.TipoAmbienteNfe tipoAmbiente)
        {
            if (tipoAmbiente == ConfigNFe.TipoAmbienteNfe.Producao)
            {
                switch (uf.ToUpper())
                {
                    case "AC": return "http://www.sefaznet.ac.gov.br/nfce/consulta?";
                    case "AL": return "http://www.sefaz.al.gov.br/nfce/consulta?";
                    case "AM": return "http://www.sefaz.am.gov.br/nfce/consulta?";
                    case "BA": return "http://www.sefaz.ba.gov.br/nfce/consulta?";
                    case "CE": return "http://www.sefaz.ce.gov.br/nfce/consulta?";
                    case "DF": return "http://www.fazenda.df.gov.br/nfce/consulta?";
                    case "ES": return "http://www.sefaz.es.gov.br/nfce/consulta?";
                    case "GO": return "http://www.sefaz.go.gov.br/nfce/consulta?";
                    case "MA": return "http://www.sefaz.ma.gov.br/nfce/consulta?";
                    case "MG": return "http://www.fazenda.mg.gov.br/nfce/consulta?";
                    case "MS": return "http://www.dfe.ms.gov.br/nfce/consulta?";
                    case "MT": return "http://www.sefaz.mt.gov.br/nfce/consulta?";
                    case "PA": return "http://www.sefa.pa.gov.br/nfce/consulta?";
                    case "PB": return "http://www.receita.pb.gov.br/nfce/consulta?";
                    case "PE": return "http://www.sefaz.pe.gov.br/nfce/consulta?";
                    case "PI": return "http://www.sefaz.pi.gov.br/nfce/consulta?";
                    case "PR": return "http://www.fazenda.pr.gov.br/nfce/consulta?";
                    case "RJ": return "http://www.fazenda.rj.gov.br/nfce/consulta?";
                    case "RN": return "http://www.set.rn.gov.br/nfce/consulta?";
                    case "RO": return "http://www.sefin.ro.gov.br/nfce/consulta?";
                    case "RR": return "http://www.sefaz.rr.gov.br/nfce/consulta?";
                    case "RS": return "http://www.sefaz.rs.gov.br/nfce/consulta?";
                    case "SE": return "http://www.sefaz.se.gov.br/nfce/consulta?";
                    case "SP": return "https://www.nfce.fazenda.sp.gov.br/consulta?";
                    case "TO": return "http://www.sefaz.to.gov.br/nfce/consulta?";

                }
            }
            else if (tipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao)
            {
                switch (uf)
                {
                    case "AC": return "http://www.sefaznet.ac.gov.br/nfce/consulta?";
                    case "AL": return "http://www.sefaz.al.gov.br/nfce/consulta?";
                    case "AM": return "http://www.sefaz.am.gov.br/nfce/consulta?";
                    case "BA": return "http://hinternet.sefaz.ba.gov.br/nfce/consulta?";
                    case "CE": return "http://www.sefaz.ce.gov.br/nfce/consulta?";
                    case "DF": return "http://www.fazenda.df.gov.br/nfce/consulta?";
                    case "ES": return "http://www.sefaz.es.gov.br/nfce/consulta?";
                    case "GO": return "http://www.sefaz.go.gov.br/nfce/consulta?";
                    case "MA": return "http://www.sefaz.ma.gov.br/nfce/consulta?";
                    case "MG": return "http://www.fazenda.mg.gov.br/nfce/consulta?";
                    case "MS": return "http://www.dfe.ms.gov.br/nfce/consulta?";
                    case "MT": return "http://www.sefaz.mt.gov.br/nfce/consulta?";
                    case "PA": return "http://www.sefa.pa.gov.br/nfce/consulta?";
                    case "PB": return "http://www.receita.pb.gov.br/nfcehom?";
                    case "PE": return "http://www.sefaz.pe.gov.br/nfce/consulta?";
                    case "PI": return "http://www.sefaz.pi.gov.br/nfce/consulta?";
                    case "PR": return "http://www.fazenda.pr.gov.br/nfce/consulta?";
                    case "RJ": return "http://www.fazenda.rj.gov.br/nfce/consulta?";
                    case "RN": return "http://www.set.rn.gov.br/nfce/consulta?";
                    case "RO": return "http://www.sefin.ro.gov.br/nfce/consulta?";
                    case "RR": return "http://www.sefaz.rr.gov.br/nfce/consulta?";
                    case "RS": return "http://www.sefaz.rs.gov.br/nfce/consulta?";
                    case "SE": return "http://www.sefaz.se.gov.br/nfce/consulta?";
                    case "SP": return "https://www.homologacao.nfce.fazenda.sp.gov.br/consulta?";
                    case "TO": return "http://www.sefaz.to.gov.br/nfce/consulta?";
                }
            }

            throw new Exception($"Endereço de consulta por chave de acesso não mapeado para o estado { uf.ToUpper() }.");
        }

        #endregion

        #endregion
    }
}