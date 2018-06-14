using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa o registro de rentabilidade do custo do produto da nota fiscal.
    /// </summary>
    [PersistenceBaseDAO(typeof(DAL.ProdutoNfCustoRentabilidadeDAO))]
    [PersistenceClass("produto_nf_custo_rentabilidade")]
    public class ProdutoNfCustoRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do custo do produto da nota fiscal.
        /// </summary>
        [PersistenceProperty("IdProdNfCusto", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(ProdutoNfCusto), "IdProdNfCusto")]
        public int IdProdNfCusto { get; set; }

        /// <summary>
        /// Identificador do tipo.
        /// </summary>
        [PersistenceProperty("Tipo", PersistenceParameterType.Key)]
        public int Tipo { get; set; }

        /// <summary>
        /// Identificador do registro vinculado ao tipo.
        /// </summary>
        [PersistenceProperty("IdRegistro", PersistenceParameterType.Key)]
        public int IdRegistro { get; set; }

        /// <summary>
        /// Valor do produto.
        /// </summary>
        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }

        #endregion
    }
}
