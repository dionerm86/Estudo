using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DescontoQtdeDAO))]
    [PersistenceClass("desconto_qtde")]
    public class DescontoQtde
    {
        #region Propriedades

        [PersistenceProperty("IDDESCONTOQTDE", PersistenceParameterType.IdentityKey)]
        public uint IdDescontoQtde { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("QTDE")]
        public int Qtde { get; set; }

        [PersistenceProperty("PERCDESCONTOMAX")]
        public float PercDescontoMax { get; set; }

        #endregion
    }
}