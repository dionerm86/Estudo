using System;
using System.Xml;
using Glass.Data.Helper;
using System.IO;
using System.Xml.Schema;

namespace Glass.Data.CTeUtils
{
    public class ValidaXML
    {
        private static string _erros;

        public enum TipoArquivoXml
        {
            CTe,
            CancCTe,
            InutCTe,
            RetRecepcao,
            EnviCTe,
            EvtCancCTe,
            EnviEvtCancCTe
        }

        /// <summary>
        /// Valida XMLs com schemas da Receita
        /// </summary>
        /// <param name="xmlCTe"></param>
        public static void Validar(XmlDocument xmlCTe, TipoArquivoXml tipoArq)
        {
            string nomeSchema = String.Empty;

            switch (tipoArq)
            {
                case TipoArquivoXml.CTe:
                    nomeSchema = "cte_v3.00.xsd";
                    break;
                case TipoArquivoXml.EnviCTe:
                    nomeSchema = "enviCte_v3.00.xsd";
                    break;
                case TipoArquivoXml.RetRecepcao:
                    nomeSchema = "consReciCte_v3.00.xsd";
                    break;
                case TipoArquivoXml.InutCTe:
                    nomeSchema = "inutCte_v3.00.xsd";
                    break;
                case TipoArquivoXml.CancCTe:
                    nomeSchema = "eventoCTe_v3.00.xsd";
                    break;
            }

            xmlCTe.PreserveWhitespace = true;
            var caminhoDoSchema = Utils.GetSchemasPath + nomeSchema;
            bool lArqXSD = File.Exists(caminhoDoSchema);

            if (lArqXSD)
            {
                MemoryStream stream = new MemoryStream();
                xmlCTe.Save(stream);

                using (stream)
                {
                    stream.Position = 0;

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ValidationType = ValidationType.Schema;
                    settings.Schemas.Add("http://www.portalfiscal.inf.br/cte", XmlReader.Create(caminhoDoSchema));
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
