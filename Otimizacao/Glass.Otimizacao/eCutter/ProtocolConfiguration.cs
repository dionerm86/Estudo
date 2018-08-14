using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Configuração do procolo para comunicação com o eCutter.
    /// </summary>
    public class ProtocolConfiguration : IXmlSerializable
    {
        #region Variáveis Locais

        private readonly Uri _enderecoServico;

        #endregion

        #region Propriedades

        /// <summary>
        /// Nome da solução.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Formato do estoque de chapas.
        /// </summary>
        public string SheetStockFormat => "Default";

        /// <summary>
        /// Uri que será usada para recuperar o estoque de chapas.
        /// </summary>
        public string GetSheetStockUri => $"{_enderecoServico}&sheetstock=true";

        /// <summary>
        /// Formato do plano de otimização.
        /// </summary>
        public string OptimizationPlanFormat { get; }

        /// <summary>
        /// Uri que será usado parao plano de otimização.
        /// </summary>
        public string GetOptimizationPlanUri => $"{_enderecoServico}&optimizationplan=true";

        /// <summary>
        /// Uri que será usada para salva os dados da solução.
        /// </summary>
        public string SaveOptimizationUri => $"{_enderecoServico}";

        /// <summary>
        /// Opções da operação de salvar a otimização.
        /// </summary>
        public string[] SaveOptimizationOptions => new[] { "eCutter", "Optyway LabelTemp", "Optimization Package", "eCutter Labels" };

        /// <summary>
        /// Formato das peças padrão.
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("standardPiecesFormat")]
        public string StandardPiecesFormat => "Default";

        /// <summary>
        /// Uri que será usada para recupera
        /// </summary>
        public string GetStandardPiecesUri => $"{_enderecoServico}&standardpieces=true";

        /// <summary>
        /// Identifica se o otimizador deve ser fechado quando a otimização for salva.
        /// </summary>
        public bool CloseOnSave => true;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="enderecoServico">Endereço do serviço.</param>
        /// <param name="nome">Nome da otimização.</param>
        /// <param name="optimizationPlanFormat">Formato do plano de otimização.</param>
        public ProtocolConfiguration(Uri enderecoServico, string nome, string optimizationPlanFormat)
        {
            Name = nome;
            _enderecoServico = enderecoServico;
            OptimizationPlanFormat = optimizationPlanFormat;

        }

        /// <summary>
        /// Construtor geral.
        /// </summary>
        public ProtocolConfiguration()
        {
        }

        #endregion

        #region Membros de IXmlSerializable

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializa os dados da instancia.
        /// </summary>
        /// <param name="writer"></param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("sheetStockFormat", SheetStockFormat);
            writer.WriteAttributeString("getSheetStockUri", GetSheetStockUri);
            writer.WriteAttributeString("optimizationPlanFormat", OptimizationPlanFormat);
            writer.WriteAttributeString("getOptimizationPlanUri", GetOptimizationPlanUri);
            writer.WriteAttributeString("saveOptimizationUri", SaveOptimizationUri);
            writer.WriteAttributeString("standardPiecesFormat", StandardPiecesFormat);
            writer.WriteAttributeString("getStandardPiecesUri", GetStandardPiecesUri);
            writer.WriteStartAttribute("closeOnSave");
            writer.WriteValue(CloseOnSave);
            writer.WriteEndAttribute();

            writer.WriteStartElement("SaveOptimizationOptions");

            if (SaveOptimizationOptions != null)
            {
                foreach (var option in SaveOptimizationOptions)
                {
                    writer.WriteStartElement("Option");
                    writer.WriteValue(option);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
