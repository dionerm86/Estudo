using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceClass("produto_inventario_estoque")]
    [PersistenceBaseDAO(typeof(ProdutoInventarioEstoqueDAO))]
    [Serializable]
    public class ProdutoInventarioEstoque
    {
        #region Propriedades

        [PersistenceProperty("IDINVENTARIOESTOQUE", PersistenceParameterType.Key)]
        public uint IdInventarioEstoque { get; set; }

        [PersistenceProperty("IDPROD", PersistenceParameterType.Key)]
        public uint IdProd { get; set; }

        [PersistenceProperty("QTDEINI")]
        public float QtdeIni { get; set; }

        [PersistenceProperty("M2INI")]
        public float M2Ini { get; set; }

        [PersistenceProperty("QTDEFIM")]
        public float? QtdeFim { get; set; }

        [PersistenceProperty("M2FIM")]
        public float? M2Fim { get; set; }

        #endregion
    }
}
