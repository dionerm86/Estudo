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

        [XmlAttribute("position")]
        public int Posicao { get; set; }

        [XmlAttribute("sheetId")]
        public string IdChapa { get; set; }

        [XmlAttribute("xDimension")]
        public double Largura { get; set; }

        [XmlAttribute("yDimension")]
        public double Altura { get; set; }

        [XmlArray("Labels")]
        [XmlArrayItem("Part", Type = typeof(EtiquetaPeca))]
        [XmlArrayItem("Waste", Type = typeof(EtiquetaRetalho))]
        public List<Etiqueta> Etiquetas { get; } = new List<Etiqueta>();

        #endregion
    }
}
