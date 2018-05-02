using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os dados da rentabilidade associados com o produto do pedido espelho.
    /// </summary>
    [PersistenceBaseDAO(typeof(DAL.ProdutoPedidoEspelhoRentabilidadeDAO))]
    [PersistenceClass("produto_pedido_espelho_rentabilidade")]
    public class ProdutoPedidoEspelhoRentabilidade : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador do produto do pedido espelho
        /// </summary>
        [PersistenceProperty("IdProdPed", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(ProdutosPedidoEspelho), "IdProdPed")]
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
