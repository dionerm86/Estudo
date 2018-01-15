using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [Serializable,
    PersistenceClass("capacidade_producao_diaria_setor"),
    PersistenceBaseDAO(typeof(CapacidadeProducaoDiariaSetorDAO))]
    public class CapacidadeProducaoDiariaSetor
    {
        [PersistenceProperty("DATA", PersistenceParameterType.Key)]
        public DateTime Data { get; set; }

        [PersistenceProperty("IDSETOR", PersistenceParameterType.Key)]
        public uint IdSetor { get; set; }

        [PersistenceProperty("CAPACIDADE")]
        public int Capacidade { get; set; }
    }
}
