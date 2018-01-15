using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(VeiculoCteDAO))]
    [PersistenceClass("veiculo_cte")]
    public class VeiculoCte
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("PLACA", PersistenceParameterType.Key)]
        public string Placa { get; set; }

        [PersistenceProperty("VALORFRETE")]
        public decimal ValorFrete { get; set; }        

        #endregion
    }
}
