using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Glass.Data.NFeUtils
{
    public interface IInutilizacao
    {
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
        void CancelAsync(object userState);
        XmlNode nfeInutilizacaoNF([XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe/wsdl/NFeInutilizacao4")] XmlNode nfeDadosMsg);
        void nfeInutilizacaoNFAsync(XmlNode nfeDadosMsg);
        void nfeInutilizacaoNFAsync(XmlNode nfeDadosMsg, object userState);
        int Timeout { get; set; }
        X509CertificateCollection ClientCertificates { get; }
        SoapProtocolVersion SoapVersion { get; set; }
    }
}