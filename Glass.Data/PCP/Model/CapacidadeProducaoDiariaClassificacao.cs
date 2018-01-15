using GDA;
using System;

namespace Glass.PCP.Data.Model
{
    [Serializable,
    PersistenceClass("capacidade_producao_diaria_classificacao")]
    public class CapacidadeProducaoDiariaClassificacao : Colosoft.Data.BaseModel
    {
        #region propiedades

        [PersistenceProperty("Data", PersistenceParameterType.Key)]
        public DateTime Data { get; set; }

        [PersistenceProperty("IdClassificacaoRoteiroProducao", PersistenceParameterType.Key)]
        public int IdClassificacaoRoteiroProducao { get; set; }

        [PersistenceProperty("Capacidade")]
        public int Capacidade { get; set; }

        #endregion
    }
}
