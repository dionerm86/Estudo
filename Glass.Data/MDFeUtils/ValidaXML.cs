using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Glass.Data.MDFeUtils
{
    public class ValidaXML
    {
        private static string _erros;

        public enum TipoArquivoXml
        {
            /// <summary>
            /// Leiaute do MDF-e
            /// </summary>
            MDFe,

            /// <summary>
            /// Mensagem de envio e solicitação de autorização do MDF-e
            /// </summary>
            EnviMDFe,

            /// <summary>
            /// Mensagem de consulta processamento do MDF-e transmitido.
            /// </summary>
            ConsultaRecibo,

            /// <summary>
            /// Mensagem de consulta da situação atual da MDF-e.
            /// </summary>
            ConsultaSituacaoMDFe,

            /// <summary>
            /// Leiaute do modal Rodoviário
            /// </summary>
            Rodoviario,

            /// <summary>
            /// Mensagem de solicitação de registro de evento do MDF-e
            /// </summary>
            EventoMDFe,

            /// <summary>
            /// Leiaute específico do evento de cancelamento de MDFe
            /// </summary>
            EvCancMDFe,

            /// <summary>
            /// Leiaute específico do evento de encerramento de MDF-e
            /// </summary>
            EvEncMDFe
        }

        /// <summary>
        /// Valida XMLs com schemas da Receita
        /// </summary>
        /// <param name="xmlMDFe"></param>
        public static void Validar(XmlDocument xmlMDFe, TipoArquivoXml tipoArq)
        {
            string nomeSchema = string.Empty;

            switch (tipoArq)
            {
                case TipoArquivoXml.MDFe:
                    nomeSchema = "mdfe_v3.00.xsd";
                    break;
                case TipoArquivoXml.EnviMDFe:
                    nomeSchema = "enviMDFe_v3.00.xsd";
                    break;
                case TipoArquivoXml.ConsultaRecibo:
                    nomeSchema = "consReciMDFe_v3.00.xsd";
                    break;
                case TipoArquivoXml.ConsultaSituacaoMDFe:
                    nomeSchema = "consSitMDFe_v3.00.xsd";
                    break;
                case TipoArquivoXml.Rodoviario:
                    nomeSchema = "mdfeModalRodoviario_v3.00.xsd";
                    break;
                case TipoArquivoXml.EventoMDFe:
                    nomeSchema = "eventoMDFe_v3.00.xsd";
                    break;
                case TipoArquivoXml.EvCancMDFe:
                    nomeSchema = "evCancMDFe_v3.00.xsd";
                    break;
                case TipoArquivoXml.EvEncMDFe:
                    nomeSchema = "evEncMDFe_v3.00.xsd";
                    break;
            }

            xmlMDFe.PreserveWhitespace = true;
            var caminhoDoSchema = $"{ Helper.Utils.ObterCaminhoEsquemaMDFe }{ nomeSchema }";

            if (File.Exists(caminhoDoSchema))
            {
                MemoryStream stream = new MemoryStream();
                xmlMDFe.Save(stream);

                using (stream)
                {
                    stream.Position = 0;

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ValidationType = ValidationType.Schema;
                    settings.Schemas.Add("http://www.portalfiscal.inf.br/mdfe", XmlReader.Create(caminhoDoSchema));
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
