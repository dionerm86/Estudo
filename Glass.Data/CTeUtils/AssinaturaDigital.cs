using System;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;

namespace Glass.Data.CTeUtils
{
    public class AssinaturaDigital
    {
        private string msgResultado;

        /// <summary>
        /// Mensagem de retorno
        /// </summary>
        public string mensagemResultado
        {
            get { return msgResultado; }
        }

        /// <summary>
        ///     Retornos:
        ///         Assinar : 0 - Assinatura realizada com sucesso
        ///                   1 - Erro: Problema ao acessar o certificado digital
        ///                   2 - Problemas no certificado digital
        ///                   3 - XML mal formado + exceção
        ///                   4 - A tag de assinatura %refUri% inexiste
        ///                   5 - A tag de assinatura %refUri% não é unica
        ///                   6 - Erro Ao assinar o documento - ID deve ser string %RefUri(Atributo)%
        ///                   7 - Erro: Ao assinar o documento - %exceção%
        /// 
        ///         XMLStringAssinado : string XML assinada
        /// 
        ///         XMLDocAssinado    : XMLDocument do XML assinado
        /// </summary>
        /// <param name="xmlNfe">Documento XML a ser assinado</param>
        /// <param name="refUri">Referência da URI a ser assinada (Ex.: infCanc, infCTe, infInut, etc.)</param>
        /// <param name="x509Cert">certificado digital a ser utilizado na assinatura digital</param>
        /// <returns></returns>
        public int Assinar(ref XmlDocument doc, string refUri, X509Certificate2 x509Cert)
        {
            int resultado = 0;
            msgResultado = "Assinatura realizada com sucesso";
            try
            {
                // certificado para ser utilizado na assinatura
                string _xnome = "";
                if (x509Cert != null)
                    _xnome = x509Cert.Subject.ToString();

                string x = x509Cert.GetKeyAlgorithm().ToString();

                // Format the document to ignore white spaces.
                doc.PreserveWhitespace = true;

                // Recarrega XML para fazer o calculo do digest corretamente
                XmlDocument docTemp = new XmlDocument();
                docTemp.LoadXml(doc.InnerXml.ToString());
                doc = docTemp;

                // Load the passed XML file using it's name.
                try
                {
                    // Verifica se a tag a ser assinada existe é única
                    int qtdeRefUri = doc.GetElementsByTagName(refUri).Count;

                    if (qtdeRefUri == 0)
                    {
                        // a URI indicada não existe
                        resultado = 4;
                        msgResultado = "A tag de assinatura " + refUri.Trim() + " inexiste";
                    }
                    // Existe mais de uma tag a ser assinada
                    else
                    {
                        if (qtdeRefUri > 1)
                        {
                            // existe mais de uma URI indicada
                            resultado = 5;
                            msgResultado = "A tag de assinatura " + refUri.Trim() + " não é única.";
                        }
                        else
                        {
                            try
                            {
                                // Create a SignedXml object.
                                SignedXml signedXml = new SignedXml(doc);

                                // Add the key to the SignedXml document 
                                signedXml.SigningKey = x509Cert.PrivateKey;

                                if (x509Cert.PrivateKey == null)
                                    throw new Exception("Chave privada nula.");

                                // Create a reference to be signed
                                Reference reference = new Reference();

                                // pega o uri que deve ser assinada
                                XmlElement e = (XmlElement)doc.GetElementsByTagName(refUri).Item(0);
                                XmlAttributeCollection _Uri = e.Attributes;
                                foreach (XmlAttribute _atributo in _Uri)
                                    if (_atributo.Name == "Id")
                                    {
                                        reference.Uri = "#" + _atributo.InnerText;
                                        break;
                                    }

                                // Add an enveloped transformation to the reference.
                                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                                reference.AddTransform(env);

                                XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
                                reference.AddTransform(c14);

                                // Add the reference to the SignedXml object.
                                signedXml.AddReference(reference);

                                // Create a new KeyInfo object
                                KeyInfo keyInfo = new KeyInfo();

                                // Load the certificate into a KeyInfoX509Data object
                                // and add it to the KeyInfo object.
                                keyInfo.AddClause(new KeyInfoX509Data(x509Cert));

                                // Add the KeyInfo object to the SignedXml object.
                                signedXml.KeyInfo = keyInfo;
                                signedXml.ComputeSignature();

                                // Get the XML representation of the signature and save
                                // it to an XmlElement object.
                                XmlElement xmlDigitalSignature = signedXml.GetXml();

                                // Append the element to the XML document.
                                e.ParentNode.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
                            }
                            catch (Exception caught)
                            {
                                resultado = 7;
                                msgResultado = "Erro: Ao assinar o documento - " + caught.Message;
                            }
                        }
                    }
                }
                catch (Exception caught)
                {
                    resultado = 3;
                    msgResultado = "Erro: XML mal formado - " + caught.Message;
                }
            }
            catch (Exception caught)
            {
                resultado = 1;
                msgResultado = "Erro: Problema ao acessar o certificado digital" + caught.Message;
            }

            return resultado;
        }
    }
}
