using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(PrevisaoProducaoDAO)),
    PersistenceClass("previsao_producao")]
    public class PrevisaoProducao
    {
        [PersistenceProperty("IDSETOR", DirectionParameter.InputOptional)]
        public uint IdSetor { get; set; }

        [PersistenceProperty("TOTM")]
        public double TotM { get; set; }

        [PersistenceProperty("DataFabrica", DirectionParameter.InputOptional)]
        public DateTime DataFabrica { get; set; }

        [PersistenceProperty("IdClassificacaoRoteiroProducao", DirectionParameter.InputOptional)]
        public int IdClassificacaoRoteiroProducao { get; set; }
    }
}
