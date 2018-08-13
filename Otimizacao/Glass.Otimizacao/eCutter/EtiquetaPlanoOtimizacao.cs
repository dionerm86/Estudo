using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Representa a etiqueta de um plano de otimização.
    /// </summary>
    public class EtiquetaPlanoOtimizacao
    {
        #region Propriedades

        [XmlAttribute("name")]
        public string Nome { get; set; }

        [XmlAttribute("materialCode")]
        public string CodigoMaterial { get; set; }

        [XmlArray("CuttingPlans")]
        [XmlArrayItem("CuttingPlan")]
        public List<EtiquetaPlanoCorte> PlanosCorte { get; } = new List<EtiquetaPlanoCorte>();

        #endregion
    }
}
