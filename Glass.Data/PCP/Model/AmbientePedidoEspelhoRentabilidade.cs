using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os dados da rentabilidade associados com o ambiente do pedido espelho
    /// </summary>
    [PersistenceBaseDAO(typeof(DAL.AmbientePedidoEspelhoRentabilidadeDAO))]
    [PersistenceClass("ambiente_pedido_espelho_rentabilidade")]
    public class AmbientePedidoEspelhoRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do ambiente.
        /// </summary>
        [PersistenceProperty("IdAmbientePedido", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(AmbientePedidoEspelho), "IdAmbientePedido")]
        public int IdAmbientePedido { get; set; }

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
