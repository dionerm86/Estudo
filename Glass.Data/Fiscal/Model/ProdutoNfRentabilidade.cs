using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os dados da rentabilidade associados com o produto da nota fiscal.
    /// </summary>
    [PersistenceBaseDAO(typeof(DAL.ProdutoNfRentabilidadeDAO))]
    [PersistenceClass("produto_nf_rentabilidade")]
    public class ProdutoNfRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do produto da nota fiscal.
        /// </summary>
        [PersistenceProperty("IdProdNf", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(ProdutosNf), "IdProdNf")]
        public int IdProdNf { get; set; }

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
