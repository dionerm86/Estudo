using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os dados da rentabilidade associados com o pedido
    /// </summary>
    [PersistenceBaseDAO(typeof(DAL.PedidoRentabilidadeDAO))]
    [PersistenceClass("pedido_rentabilidade")]
    public class PedidoRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do pedido
        /// </summary>
        [PersistenceProperty("IdPedido", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Pedido), "IdPedido")]
        public int IdPedido { get; set; }

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
