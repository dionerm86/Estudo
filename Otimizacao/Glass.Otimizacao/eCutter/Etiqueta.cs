using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Representa uma etiqueta.
    /// </summary>
    public abstract class Etiqueta
    {
        #region Propriedades

        /// <summary>
        /// Obtém o tipo da etiqueta.
        /// </summary>
        [XmlIgnore]
        public abstract TipoEtiqueta Tipo { get; }

        /// <summary>
        /// Obtém a posição da etiqueta.
        /// </summary>
        [XmlAttribute("position")]
        public int Posicao { get; set; }

        #endregion
    }
}
