using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Projeto.Negocios.Entidades
{
    public class FerragemIntegracaoPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da Ferragem
        /// </summary>
        public int IdFerragem { get; set; }

        /// <summary>
        /// Fabricante da Ferragem
        /// </summary>
        public string NomeFabricante { get; set; }

        /// <summary>
        /// Nome da Ferragem
        /// </summary>
        public string NomeFerragem { get; set; }

        /// <summary>
        /// Constante da Ferragem
        /// </summary>
        public string NomeConstante { get; set; }

        /// <summary>
        /// Valor da Constante
        /// </summary>
        public double valor { get; set; }

        #endregion
    }
}
