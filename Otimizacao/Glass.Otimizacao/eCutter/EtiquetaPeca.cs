using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Glass.Otimizacao.eCutter
{
    /// <summary>
    /// Representa a etiqueta de uma peça do plano de corte.
    /// </summary>
    [XmlRoot("Part")]
    public class EtiquetaPeca : Etiqueta
    {
        #region Propriedades

        /// <summary>
        /// Obtém o tipo de etiqueta.
        /// </summary>
        public override TipoEtiqueta Tipo => TipoEtiqueta.Peca;

        [XmlAttribute("id")]
        public string IdPeca { get; set; }

        [XmlAttribute("partPosition")]
        public int PosicaoPeca { get; set; }

        [XmlAttribute("quantity")]
        public int Quantidade { get; set; }

        [XmlAttribute("xDimension")]
        public double Largura { get; set; }

        [XmlAttribute("yDimension")]
        public double Altura { get; set; }

        [XmlAttribute("realXDimension")]
        public double LarguraReal { get; set; }

        [XmlAttribute("realYDimension")]
        public double AlturaReal { get; set; }

        [XmlAttribute("canRotate")]
        public bool PodeRotacionar { get; set; }

        [XmlAttribute("labelsQuantity")]
        public int QuantidadeEtiquetas { get; set; }

        [XmlAttribute("rack")]
        public string Cavalete { get; set; }

        [XmlAttribute("priority")]
        public int Prioridade { get; set; }

        [XmlAttribute("preference")]
        public int Preferencia { get; set; }

        [XmlAttribute("grinding")]
        public double Aresta { get; set; }

        [XmlAttribute("grindingX1")]
        public double ArestaX1 { get; set; }

        [XmlAttribute("grindingY1")]
        public double ArestaY1 { get; set; }

        [XmlAttribute("grindingX2")]
        public double ArestaX2 { get; set; }

        [XmlAttribute("grindingY2")]
        public double ArestaY2 { get; set; }

        [XmlAttribute("shape")]
        public string Forma { get; set; }

        [XmlAttribute("deliveryDate")]
        public DateTime DataEntrega { get; set; }

        [XmlAttribute("compositeMaterialCode")]
        public string CodigoMaterialComposicao { get; set; }

        [XmlAttribute("compositeMaterialDescription")]
        public string DescricaoMaterialComposicao { get; set; }

        [XmlAttribute("isRotated")]
        public bool Rotacionada { get; set; }

        [XmlElement("Customer")]
        public string Cliente { get; set; }

        [XmlElement("Order")]
        public string Pedido { get; set; }

        [XmlElement("Notes")]
        public string Notas { get; set; }

        [XmlElement("Descricao")]
        public string Descricao { get; set; }

        public List<string> NotasExtendidas { get; } = new List<string>();

        #endregion
    }
}
