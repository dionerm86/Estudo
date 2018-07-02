using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Glass.Data.NFeUtils
{
    public interface IConsultaProtocolo
    {
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
        void CancelAsync(object userState);
        XmlNode nfeConsultaNF([XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe/wsdl/NFeConsultaProtocolo4")] XmlNode nfeDadosMsg);
        void nfeConsultaNFAsync(XmlNode nfeDadosMsg);
        void nfeConsultaNFAsync(XmlNode nfeDadosMsg, object userState);

        int Timeout { get; set; }
        X509CertificateCollection ClientCertificates { get; }
        SoapProtocolVersion SoapVersion { get; set; }
    }
}
