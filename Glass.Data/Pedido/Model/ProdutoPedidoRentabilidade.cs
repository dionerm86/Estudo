using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os dados da rentabilidade associados com o produto do pedido.
    /// </summary>
    [PersistenceBaseDAO(typeof(DAL.ProdutoPedidoRentabilidadeDAO))]
    [PersistenceClass("produto_pedido_rentabilidade")]
    public class ProdutoPedidoRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do produto do pedido.
        /// </summary>
        [PersistenceProperty("IdProdPed", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(ProdutosPedido), "IdProdPed")]
        public int IdProdPed { get; set; }

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
