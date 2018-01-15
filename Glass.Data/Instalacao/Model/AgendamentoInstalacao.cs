using System;
using GDA;

namespace Glass.Data.Model
{
    public class AgendamentoInstalacao
    {
        #region Propriedades

        [PersistenceProperty("IdEquipeInstalacao", DirectionParameter.InputOptional)]
        public uint IdEquipeInstalacao { get; set; }

        [PersistenceProperty("DataInstalacao", DirectionParameter.InputOptional)]
        public DateTime DataInstalacao { get; set; }

        [PersistenceProperty("QuantidadeInstalacao", DirectionParameter.InputOptional)]
        public long QuantidadeInstalacao { get; set; }

        [PersistenceProperty("NomeEquipe", DirectionParameter.InputOptional)]
        public string NomeEquipe { get; set; }

        [PersistenceProperty("Cliente", DirectionParameter.InputOptional)]
        public string Cliente { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion
    }
}