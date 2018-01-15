using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(EquipeInstalacaoDAO))]
    [PersistenceClass("equipe_instalacao")]
    public class EquipeInstalacao
    {
        #region Propriedades

        [PersistenceProperty("IDEQUIPEINST", PersistenceParameterType.IdentityKey)]
        public uint IdEquipeInstalacao { get; set; }

        [PersistenceProperty("IDORDEMINSTALACAO")]
        public uint IdOrdemInstalacao { get; set; }

        [PersistenceProperty("IDINSTALACAO")]
        public uint IdInstalacao { get; set; }

        [PersistenceProperty("IDEQUIPE")]
        public uint IdEquipe { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEEQUIPE", DirectionParameter.InputOptional)]
        public string NomeEquipe { get; set; }

        #endregion
    }
}