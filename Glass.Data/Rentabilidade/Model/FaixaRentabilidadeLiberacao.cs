using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;
using Glass.Log;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa a faixa de rentabilidade para a liberação.
    /// </summary>
    [PersistenceClass("faixa_rentabilidade_liberacao")]
    public class FaixaRentabilidadeLiberacao : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador da faixa de rentabilidade para liberação.
        /// </summary>
        [PersistenceProperty("IDFAIXARENTABILIDADELIBERACAO", PersistenceParameterType.IdentityKey)]
        public int IdFaixaRentabilidadeLiberacao { get; set; }

        /// <summary>
        /// Identificador da loja a qual a faixa está configurada.
        /// </summary>
        [PersistenceProperty("IDLOJA")]
        [PersistenceForeignKey(typeof(Loja), "IdLoja")]
        [Log("Loja", "NomeFantasia", typeof(DAL.LojaDAO))]
        public int IdLoja { get; set; }

        /// <summary>
        /// Percentual da rentabilidade da faixa.
        /// </summary>
        [PersistenceProperty("PERCENTUALRENTABILIDADE")]
        [Log("Percentual Rentabilidade")]
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Identifica se é requirido liberação para a faixa de rentabilidade.
        /// </summary>
        [PersistenceProperty("REQUERLIBERACAO")]
        [Log("Requer Liberação")]
        public bool RequerLiberacao { get; set; }

        #endregion
    }
}
