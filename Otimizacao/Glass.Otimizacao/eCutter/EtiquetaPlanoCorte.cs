using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Representa a etiqueta do plano de corte.
    /// </summary>
    [XmlRoot("CuttingPlan")]
    public class EtiquetaPlanoCorte
    {
        #region Propriedades

        /// <summary>
        /// Obtém ou define a posição da etiqueta do plano de corte.
        /// </summary>
        [XmlAttribute("position")]
        public int Posicao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da chapa usada.
        /// </summary>
        [XmlAttribute("sheetId")]
        public string IdChapa { get; set; }

        /// <summary>
        /// Obtém ou define a largura do plano de corte.
        /// </summary>
        [XmlAttribute("xDimension")]
        public double Largura { get; set; }

        /// <summary>
        /// Obtém ou define a altura do plano de corte.
        /// </summary>
        [XmlAttribute("yDimension")]
        public double Altura { get; set; }

        /// <summary>
        /// Obtém as etiquetas associadas.
        /// </summary>
        [XmlArray("Labels")]
        [XmlArrayItem("Part", Type = typeof(EtiquetaPeca))]
        [XmlArrayItem("Waste", Type = typeof(EtiquetaRetalho))]
        public List<Etiqueta> Etiquetas { get; } = new List<Etiqueta>();

        #endregion
    }
}
