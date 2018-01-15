using System;

namespace Glass.Estoque.Negocios.Entidades
{
    public class MovChapaDetalhe
    {
        public int IdProdImpressaoChapa { get; set; }

        public int IdLoja { get; set; }

        public int IdCorVidro { get; set; }

        public DateTime DataLeitura { get; set; }

        public int IdProd { get; set; }

        public int QtdeInicial { get; set; }

        public decimal M2Inicial
        {
            get
            {
                return QtdeInicial * M2Utilizado;
            }
        }

        public int QtdeUtilizado { get; set; }

        public decimal M2Utilizado { get; set; }

        public int QtdeDisponivel
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
                return M2Utilizado - M2Lido;
            }
        }

        /// <summary>
        /// Descrição do Produto
        /// </summary>
        public string DescricaoProd { get; set; }

        /// <summary>
        /// Planos de corte associados a chapa
        /// </summary>
        public string PlanosCorte { get; set; }

        /// <summary>
        /// Etiquetas vinculadas a chapa
        /// </summary>
        public string Etiquetas { get; set; }

        /// <summary>
        /// Etiqueta da chapa
        /// </summary>
        public string NumEtiqueta { get; set; }

        /// <summary>
        /// Indica se a chapa tem leitura em dias diferentes
        /// </summary>
        public bool TemOutrasLeituras { get; set; }

        /// <summary>
        /// Indica que a chapa foi revendida e não cortada
        /// </summary>
        public bool SaidaRevenda { get; set; }

    }
}
