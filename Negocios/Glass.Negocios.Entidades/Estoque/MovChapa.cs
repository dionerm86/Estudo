using System;
using System.Collections.Generic;

namespace Glass.Estoque.Negocios.Entidades
{
    public class MovChapa
    {
        public int IdCorVidro { get; set; }

        public Int64 QtdeInicial { get; set; }

        public decimal M2Inicial { get; set; }

        public int QtdeUtilizado { get; set; }

        public decimal M2Utilizado { get; set; }

        public Int64 QtdeDisponivel
        {
            get
            {
                return QtdeInicial - QtdeUtilizado;
            }
        }

        public decimal M2Disponivel
        {
            get
            {
                return M2Inicial - M2Utilizado;
            }
        }


        /// <summary>
        /// Cor do Vidro
        /// </summary>
        public string CorVidro { get; set; }

        /// <summary>
        /// Espessura
        /// </summary>
        public float Espessura { get; set; }

        /// <summary>
        /// Qtde. e M² inicial (Dia Anterior)
        /// </summary>
        public string Inicial
        {
            get
            {
                return QtdeInicial + " (" + M2Inicial + "m²)";
            }
        }

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
