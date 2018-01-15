using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ComponenteValorCteDAO))]
    [PersistenceClass("componente_valor_cte")]
    public class ComponenteValorCte
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("NOMECOMPONENTE", PersistenceParameterType.Key)]
        public string NomeComponente { get; set; }

        [PersistenceProperty("VALORCOMPONENTE")]
        public decimal ValorComponente { get; set; }
        
        #endregion
    }
}
