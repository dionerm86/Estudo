using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Representa o retalho da etiqueta.
    /// </summary>
    [XmlRoot("Waste")]
    public class EtiquetaRetalho : Etiqueta
    {
        #region Propriedades
        
        /// <summary>
        /// Obtém o tipo da etiqueta.
        /// </summary>
        public override TipoEtiqueta Tipo => TipoEtiqueta.Retalho;

        /// <summary>
        /// Obtém ou define a largura do retalho.
        /// </summary>
        [XmlAttribute("xDimension")]
        public double Largura { get; set; }

        /// <summary>
        /// Obtém ou define a altura do retalho.
        /// </summary>
        [XmlAttribute("yDimension")]
        public double Altura { get; set; }

        /// <summary>
        /// Obtém ou define se o retalho é reaproveitável.
        /// </summary>
        [XmlAttribute("isReusable")]
        public bool Reaproveitavel { get; set; }

        #endregion
    }
}
