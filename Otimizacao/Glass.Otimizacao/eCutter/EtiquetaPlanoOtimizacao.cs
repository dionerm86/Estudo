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

        /// <summary>
        /// Obtém ou define o nome do plano.
        /// </summary>
        [XmlAttribute("name")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define o código do material.
        /// </summary>
        [XmlAttribute("materialCode")]
        public string CodigoMaterial { get; set; }

        /// <summary>
        /// Obtém os planos de corte.
        /// </summary>
        [XmlArray("CuttingPlans")]
        [XmlArrayItem("CuttingPlan")]
        public List<EtiquetaPlanoCorte> PlanosCorte { get; } = new List<EtiquetaPlanoCorte>();

        #endregion
    }
}
