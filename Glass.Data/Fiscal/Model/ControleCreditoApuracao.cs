using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ControleCreditoApuracaoDAO))]
    [PersistenceClass("controle_credito_apuracao")]
    public class ControleCreditoApuracao
    {
        #region Propriedades

        [PersistenceProperty("IDCREDITO", PersistenceParameterType.IdentityKey)]
        public uint IdCredito { get; set; }

        [PersistenceProperty("PERIODOGERACAO")]
        public string PeriodoGeracao { get; set; }

        [PersistenceProperty("VALORGERADO")]
        public decimal ValorGerado { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        #endregion
    }
}