using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TipoBoletoDAO))]
    [PersistenceClass("tipo_boleto")]
    public class TipoBoleto
    {
        #region Propriedades

        [PersistenceProperty("IDTIPOBOLETO", PersistenceParameterType.IdentityKey)]
        public uint IdTipoBoleto { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion
    }
}