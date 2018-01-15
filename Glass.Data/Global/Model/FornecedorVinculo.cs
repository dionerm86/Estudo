using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FornecedorVinculoDAO))]
    [PersistenceClass("Fornecedor_vinculo")]
    public class FornecedorVinculo
    {
        #region Propriedades

        [PersistenceProperty("IdFornec", PersistenceParameterType.Key)]
        public int IdFornec { get; set; }

        [PersistenceProperty("IdFornecVinculo", PersistenceParameterType.Key)]
        public int IdFornecVinculo { get; set; }

        #endregion
    }
}