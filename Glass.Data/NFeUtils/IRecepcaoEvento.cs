using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Glass.Data.NFeUtils
{
    public interface IRecepcaoEvento
    {
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
        void CancelAsync(object userState);
        XmlNode nfeRecepcaoEvento([XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4")] XmlNode nfeDadosMsg);
        void nfeRecepcaoEventoAsync(XmlNode nfeDadosMsg);
        void nfeRecepcaoEventoAsync(XmlNode nfeDadosMsg, object userState);
        int Timeout { get; set; }
        X509CertificateCollection ClientCertificates { get; }
        SoapProtocolVersion SoapVersion { get; set; }
    }
}