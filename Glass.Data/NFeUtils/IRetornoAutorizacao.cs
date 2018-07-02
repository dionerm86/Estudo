using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Glass.Data.NFeUtils
{
    public interface IRetornoAutorizacao
    {
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
        void CancelAsync(object userState);
        XmlNode nfeRetAutorizacaoLote([XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe/wsdl/NFeRetAutorizacao4")] XmlNode nfeDadosMsg);
        void nfeRetAutorizacaoLoteAsync(XmlNode nfeDadosMsg);
        void nfeRetAutorizacaoLoteAsync(XmlNode nfeDadosMsg, object userState);
        int Timeout { get; set; }
        X509CertificateCollection ClientCertificates { get; }
        SoapProtocolVersion SoapVersion { get; set; }
    }
}