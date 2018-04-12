using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ProprietarioVeiculo_VeiculoDAO))]
    [PersistenceClass("proprietario_veiculo_veiculo")]
    public class ProprietarioVeiculo_Veiculo
    {
        [PersistenceProperty("IDPROPVEIC", PersistenceParameterType.Key)]
        public uint IdPropVeic { get; set; }

        [PersistenceProperty("PLACA", PersistenceParameterType.Key)]
        public string Placa { get; set; }
    }
}
