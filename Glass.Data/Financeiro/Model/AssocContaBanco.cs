using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AssocContaBancoDAO))]
    [PersistenceClass("assoc_conta_banco")]
    public class AssocContaBanco
    {
        #region Propriedades

        [PersistenceProperty("IDASSOCCONTABANCO", PersistenceParameterType.IdentityKey)]
        public uint IdAssocContaBanco { get; set; }

        [PersistenceProperty("IDTIPOCARTAO")]
        public uint? IdTipoCartao { get; set; }

        [PersistenceProperty("IDTIPOBOLETO")]
        public uint? IdTipoBoleto { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint IdContaBanco { get; set; }

        [PersistenceProperty("BLOQUEARCONTABANCO")]
        public bool BloquearContaBanco { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        #endregion
    }
}