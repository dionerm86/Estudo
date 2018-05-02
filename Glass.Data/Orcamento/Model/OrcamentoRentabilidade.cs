using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os dados da rentabilidade associados com o orçamento
    /// </summary>
    [PersistenceClass("orcamento_rentabilidade")]
    public class OrcamentoRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do orçamento
        /// </summary>
        [PersistenceProperty("IdOrcamento", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Orcamento), "IdOrcamento")]
        public int IdOrcamento { get; set; }

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
