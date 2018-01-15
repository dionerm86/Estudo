using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ComissaoContasReceberDAO))]
    [PersistenceClass("comissao_contas_receber")]
    public class ComissaoContasReceber
    {
        #region Propiedades

        [PersistenceProperty("IdComissaoContasReceber", PersistenceParameterType.IdentityKey)]
        public int IdComissaoContasReceber { get; set; }

        [PersistenceProperty("IdComissao")]
        public int IdComissao { get; set; }

        [PersistenceProperty("IdContaR")]
        public int IdContaR { get; set; }

        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }

        #endregion
    }
}
