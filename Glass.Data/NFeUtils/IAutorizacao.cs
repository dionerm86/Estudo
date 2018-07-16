using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace Glass.Data.NFeUtils
{
    public interface IAutorizacao
    {
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
        void CancelAsync(object userState);
        XmlNode nfeAutorizacaoLote([XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe/wsdl/NFeAutorizacao4")] XmlNode nfeDadosMsg);
        void nfeAutorizacaoLoteAsync(XmlNode nfeDadosMsg);
        void nfeAutorizacaoLoteAsync(XmlNode nfeDadosMsg, object userState);
        XmlNode nfeAutorizacaoLoteZip([XmlElement(Namespace = "http://www.portalfiscal.inf.br/nfe/wsdl/NFeAutorizacao4")] string nfeDadosMsgZip);
        void nfeAutorizacaoLoteZipAsync(string nfeDadosMsgZip);
        void nfeAutorizacaoLoteZipAsync(string nfeDadosMsgZip, object userState);
        int Timeout { get; set; }
        X509CertificateCollection ClientCertificates { get; }
        SoapProtocolVersion SoapVersion { get; set; }
    }
}