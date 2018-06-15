using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa os valores de custo do produto da nota fiscal.
    /// </summary>
    [PersistenceBaseDAO(typeof(DAL.ProdutoNfCustoDAO))]
    [PersistenceClass("produto_nf_custo")]
    public class ProdutoNfCusto : Colosoft.Data.BaseModel
    {
        #region Propriedades

        /// <summary>
        /// Identificador único do custo.
        /// </summary>
        [PersistenceProperty("IDPRODNFCUSTO", PersistenceParameterType.IdentityKey)]
        public int IdProdNfCusto { get; set; }

        /// <summary>
        /// Identificador do produto da nota fiscal.
        /// </summary>
        [PersistenceProperty("IDPRODNF")]
        public uint IdProdNf { get; set; }

        /// <summary>
        /// Identificador do produto da nota fiscal de entrada associado.
        /// </summary>
        [PersistenceProperty("IDPRODNFENTRADA")]
        public uint? IdProdNfEntrada { get; set; }

        /// <summary>
        /// Quantidade de produtos agrupados.
        /// </summary>
        [PersistenceProperty("QTDE")]
        public int Qtde { get; set; }

        /// <summary>
        /// Valor de custo.
        /// </summary>
        [PersistenceProperty("CUSTOCOMPRA")]
        public decimal CustoCompra { get; set; }

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        [PersistenceProperty("PERCENTUALRENTABILIDADE", Direction = DirectionParameter.Input)]
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Valor da rentabilidade financeira.
        /// </summary>
        [PersistenceProperty("RENTABILIDADEFINANCEIRA", Direction = DirectionParameter.Input)]
        public decimal RentabilidadeFinanceira { get; set; }

        #endregion

        #region Propriedades Extendidas

        /// <summary>
        /// Aliquota do IPI do produto na nota fiscal de entrada.
        /// </summary>
        [PersistenceProperty("ALIQIPICOMPRA", DirectionParameter.InputOptional)]
        public float? AliqIpiCompra { get; set; }

        /// <summary>
        /// Aliquota do ICMS do produto na nota fiscal de entrada.
        /// </summary>
        [PersistenceProperty("ALIQICMSCOMPRA", DirectionParameter.InputOptional)]
        public float? AliqIcmsCompra { get; set; }

        #endregion
    }
}
