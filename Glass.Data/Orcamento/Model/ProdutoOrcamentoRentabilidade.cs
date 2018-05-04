using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os dados da rentabilidade associados com o produto do orçamento.
    /// </summary>
    [PersistenceClass("produto_orcamento_rentabilidade")]
    public class ProdutoOrcamentoRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do produto do orçamento.
        /// </summary>
        [PersistenceProperty("IdProd", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(ProdutosOrcamento), "IdProd")]
        public int IdProdutoOrcamento { get; set; }

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
