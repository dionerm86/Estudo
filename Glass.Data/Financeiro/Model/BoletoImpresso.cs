using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceClass("boleto_impresso")]
    [PersistenceBaseDAO(typeof(BoletoImpressoDAO))]
    public class BoletoImpresso : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDCONTAR", PersistenceParameterType.Key)]
        public uint IdContaR { get; set; }

        [PersistenceProperty("IDNF")]
        public int? IdNf { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public int? IdLiberarPedido { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public int IdContaBanco { get; set; }               

        #endregion
    }
}
