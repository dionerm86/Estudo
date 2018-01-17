using System;
using System.Xml.Schema;
using System.Xml;
using System.IO;
using Glass.Data.Helper;

namespace Glass.Data.NFeUtils
{
    public class ValidaXML
    {
        private static string _erros;

        public enum TipoArquivoXml
        {
            NFe,
            CancNfe,
            InutNFe,
            RetRecepcao,
            EnviNFe, 
            CCe,
            EnviCce,
            EvtCancNfe,
            EnviEvtCancNfe
        }

        /// <summary>
        /// Valida XMLs com schemas da Receita
        /// </summary>
        /// <param name="xmlNFe"></param>
        public static void Validar(XmlDocument xmlNFe, TipoArquivoXml tipoArq)
        {
            string nomeSchema = String.Empty;

            switch (tipoArq)
            {
                case TipoArquivoXml.NFe:
                    nomeSchema = "nfe_v4.00.xsd";
                    break;
                case TipoArquivoXml.EnviNFe:
                    nomeSchema = "enviNFe_v3.10.xsd";
                    break;
                case TipoArquivoXml.CancNfe:
                    nomeSchema = "cancNFe_v3.10.xsd";
                    break;
                case TipoArquivoXml.InutNFe:
                    nomeSchema = "inutNFe_v3.10.xsd";
                    break;
                case TipoArquivoXml.RetRecepcao:
                    nomeSchema = "consReciNFe_v3.10.xsd";
                    break;
                case TipoArquivoXml.CCe:
                    nomeSchema = "CCe_v1.00.xsd";
                    break;
                case TipoArquivoXml.EnviCce:
                    nomeSchema = "envCCe_v1.00.xsd";
                    break;
                case TipoArquivoXml.EvtCancNfe:
                    nomeSchema = "eventoCancNFe_v1.00.xsd";
                    break;
                case TipoArquivoXml.EnviEvtCancNfe:
                    nomeSchema = "envEventoCancNFe_v1.00.xsd";
                    break;
                default:
                    throw new Exception("Nenhum schema informado.");
            }

            xmlNFe.PreserveWhitespace = true;
            var caminhoDoSchema = Utils.GetSchemasPath + nomeSchema;
            bool lArqXSD = File.Exists(caminhoDoSchema);

            if (lArqXSD)
            {
                MemoryStream stream = new MemoryStream();
                xmlNFe.Save(stream);

                using (stream)
                {
                    stream.Position = 0;

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ValidationType = ValidationType.Schema;
                    settings.Schemas.Add("http://www.portalfiscal.inf.br/nfe", XmlReader.Create(caminhoDoSchema));
                    settings.ValidationEventHandler += new ValidationEventHandler(reader_ValidationEventHandler);

                    using (XmlReader reader = XmlReader.Create(stream, settings))
                    {
                        try
                        {
                            while (reader.Read()) { }
                            reader.Close();
                        }
                        catch (Exception ex)
                        {
                            reader.Close();
                            _erros = String.Empty;
                            throw ex;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(_erros))
                {
                    string err = _erros;
                    _erros = String.Empty;
                    throw new Exception(err);
                }
            }
            else
            {
                throw new Exception("Arquivo XSD (schema) não foi encontrado em " + caminhoDoSchema);
            }
        }

        private static void reader_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            // Informa erro apenas se for Error e /*se não for questão de assinatura*/
            if (e.Severity == XmlSeverityType.Error/* && !e.Exception.Message.Contains("elementos esperados: 'Signature'")*/)
                _erros += "Linha: " + e.Exception.LineNumber + " Erro: " + e.Exception.Message;
        }
    }
}
