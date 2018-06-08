using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os dados da rentabilidade associados com o pedido
    /// </summary>
    [PersistenceBaseDAO(typeof(DAL.NotaFiscalRentabilidadeDAO))]
    [PersistenceClass("nota_fiscal_rentabilidade")]
    public class NotaFiscalRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do pedido
        /// </summary>
        [PersistenceProperty("IdNf", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(NotaFiscal), "IdNf")]
        public int IdNf { get; set; }

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
