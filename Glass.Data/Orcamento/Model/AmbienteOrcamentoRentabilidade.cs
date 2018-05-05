using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os dados da rentabilidade associados com o ambiente do orçamento
    /// </summary>
    [PersistenceClass("ambiente_orcamento_rentabilidade")]
    public class AmbienteOrcamentoRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do ambiente.
        /// </summary>
        [PersistenceProperty("IdAmbienteOrca", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(AmbienteOrcamento), "IdAmbienteOrca")]
        public int IdAmbienteOrca { get; set; }

        /// <summary>
        /// Identificador do tipo.
        /// </summary>
        [PersistenceProperty("Tipo", PersistenceParameterType.Key)]
        public byte Tipo { get; set; }

        /// <summary>
        /// Identificador do registro vinculado ao tipo.
        /// </summary>
        [PersistenceProperty("IdRegistro", PersistenceParameterType.Key)]
        public byte IdRegistro { get; set; }

        /// <summary>
        /// Valor do produto.
        /// </summary>
        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }

        #endregion
    }
}
