using System;
using System.Collections.Generic;

namespace Glass.Estoque.Negocios.Entidades
{
    public class MovChapa
    {
        public int IdCorVidro { get; set; }

        public int QtdeUtilizado { get; set; }

        public decimal M2Utilizado { get; set; }

        public int QtdeDisponivel { get; set; }

        public decimal M2Disponivel { get; set; }


        /// <summary>
        /// Cor do Vidro
        /// </summary>
        public string CorVidro { get; set; }

        /// <summary>
        /// Espessura
        /// </summary>
        public float Espessura { get; set; }

        /// <summary>
        /// Qtde. e M² utilizado
        /// </summary>
        public string Utilizado
        {
            get
            {
                return QtdeUtilizado + " (" + M2Utilizado + "m²)";
            }
        }

        /// <summary>
        /// Qtde. e M² disponível (Inicial menos o utilizado)
        /// </summary>
        public string Disponivel
        {
            get
            {
                return QtdeDisponivel + " (" + M2Disponivel + "m²)";
            }
        }

        /// <summary>
        /// M² que foi lido nas chapas
        /// </summary>
        public decimal M2Lido { get; set; }

        /// <summary>
        /// Sobra (O m² que sobrou de chapa)
        /// </summary>
        public decimal Sobra
        {
            get
            {
                return Math.Round(M2Utilizado - M2Lido, 2);
            }
        }

        /// <summary>
        /// Chapas do agrupamento
        /// </summary>
        public List<MovChapaDetalhe> Chapas { get; set; }
    }
}
