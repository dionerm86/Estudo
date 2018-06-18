using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ConfiguracaoDAO))]
	[PersistenceClass("config")]
	public class Configuracao
    {
        #region Propriedades

        [PersistenceProperty("IDCONFIG", PersistenceParameterType.Key)]
        public uint IdConfig { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("TIPO")]
        public int Tipo { get; set; }

        [PersistenceProperty("USARLOJA")]
        public bool UsarLoja { get; set; }

        [PersistenceProperty("EXIBIRAPENASADMINSYNC")]
        public bool ExibirApenasAdminSync { get; set; }

        [PersistenceProperty("NOMETIPOENUM")]
        public string NomeTipoEnum { get; set; }

        [PersistenceProperty("NOMETIPOMETODO")]
        public string NomeTipoMetodo { get; set; }

        [PersistenceProperty("GRUPO")]
        public string Grupo { get; set; }

        [PersistenceProperty("TIPOFILHOS")]
        public int TipoFilhos { get; set; }

        [PersistenceProperty("OBSERVACAO")]
        public string Observacao { get; set; }

        [PersistenceProperty("CONFIGINTERNA")]
        public bool ConfigInterna { get; set; }

        [PersistenceProperty("ABA")]
        public string Aba { get; set; }

        [PersistenceProperty("PERMITIRNEGATIVO")]
        public bool PermitirNegativo { get; set; }

        #endregion
    }
}