using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Representa um documento de etiquetas.
    /// </summary>
    [XmlRoot("LabelsDocument")]
    public class DocumentoEtiquetas
    {
        #region Propriedades

        /// <summary>
        /// Obtém ou define a versão do documento de etiquetas.
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; }

        /// <summary>
        /// Obtém ou define os planos de otimização.
        /// </summary>
        [XmlArray("OptimizationPlans")]
        [XmlArrayItem("OptimizationPlan")]
        public List<EtiquetaPlanoOtimizacao> PlanosOtimizacao { get; } = new List<EtiquetaPlanoOtimizacao>();

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Abre o documento de etiquetas a partir do conteúdo informado.
        /// </summary>
        /// <param name="conteudo"></param>
        /// <returns></returns>
        public static DocumentoEtiquetas Open(System.IO.Stream conteudo)
        {
            var xmlOverrides = new XmlAttributeOverrides();
            xmlOverrides.Add(typeof(EtiquetaPeca), "Part", new XmlAttributes());
            xmlOverrides.Add(typeof(EtiquetaRetalho), "Waste", new XmlAttributes());
            var serializer = new XmlSerializer(typeof(DocumentoEtiquetas), xmlOverrides);

            return (DocumentoEtiquetas)serializer.Deserialize(conteudo);
        }

        #endregion
    }
}
