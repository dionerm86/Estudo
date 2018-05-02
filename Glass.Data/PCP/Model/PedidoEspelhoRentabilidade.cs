using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os dados da rentabilidade associados com o pedido espelho.
    /// </summary>
    [PersistenceBaseDAO(typeof(DAL.PedidoEspelhoRentabilidadeDAO))]
    [PersistenceClass("pedido_espelho_rentabilidade")]
    public class PedidoEspelhoRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do pedido espelho.
        /// </summary>
        [PersistenceProperty("IdPedido", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(PedidoEspelho), "IdPedido")]
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
