using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Rentabilidade.Negocios.Entidades
{
    /// <summary>
    /// Representa a classe de pesquisa do registro da rentabilidade.
    /// </summary>
    public class ConfigRegistroRentabilidadePesquisa
    {
        #region Propriedades

        /// <summary>
        /// Tipo do registro.
        /// </summary>
        public int Tipo { get; set; }

        /// <summary>
        /// Identificador do registro.
        /// </summary>
        public int IdRegistro { get; set; }

        /// <summary>
        /// Posição.
        /// </summary>
        public int Posicao { get; set; }

        /// <summary>
        /// Identifica se á para exibir no relatório.
        /// </summary>
        public bool ExibirRelatorio { get; set; }

        /// <summary>
        /// Descrição do tipo.
        /// </summary>
        public string DescricaoTipo
        {
            get
            {
                return ((Rentabilidade.TipoRegistroRentabilidade)Tipo).Translate().FormatOrNull();
            }
        }

        /// <summary>
        /// Nome do registro.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Descrição do registro.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Identifica se pode mover para cima.
        /// </summary>
        public bool PodeMoverParaCima { get; set; }

        /// <summary>
        /// Identifica se pode mover para baixo.
        /// </summary>
        public bool PodeMoverParaBaixo { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ConfigRegistroRentabilidadePesquisa()
        {
            PodeMoverParaBaixo = true;
            PodeMoverParaCima = true;
        }

        #endregion
    }
}
