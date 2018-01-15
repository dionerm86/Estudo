using GDA;

namespace Glass.PCP.Data.Model
{
    [PersistenceClass("Classificacao_Roteiro_Producao")]
    public class ClassificacaoRoteiroProducao : Colosoft.Data.BaseModel
    {
        #region Propiedades

        [PersistenceProperty("IdClassificacaoRoteiroProducao", PersistenceParameterType.IdentityKey)]
        public int IdClassificacaoRoteiroProducao { get; set; }

        [PersistenceProperty("Descricao")]
        public string Descricao { get; set; }

        [PersistenceProperty("CapacidadeDiaria")]
        public int CapacidadeDiaria { get; set; }

        [PersistenceProperty("METADIARIA")]
        public decimal MetaDiaria { get; set; }

        #endregion
    }
}
