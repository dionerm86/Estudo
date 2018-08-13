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

        [XmlAttribute("xDimension")]
        public double Largura { get; set; }

        [XmlAttribute("yDimension")]
        public double Altura { get; set; }

        #endregion
    }
}
