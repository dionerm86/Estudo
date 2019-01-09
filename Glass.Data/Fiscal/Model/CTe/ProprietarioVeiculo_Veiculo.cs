using System;
using GDA;
using Glass.Data.DAL.CTe;
using System.Xml.Serialization;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ProprietarioVeiculo_VeiculoDAO))]
    [PersistenceClass("proprietario_veiculo_veiculo")]
    public class ProprietarioVeiculo_Veiculo
    {
        #region Propriedades

        [PersistenceProperty("IDPROPVEIC", PersistenceParameterType.Key)]
        public uint IdPropVeic { get; set; }

        [PersistenceProperty("PLACA", PersistenceParameterType.Key)]
        public string Placa { get; set; }

        #endregion

        #region Propriedades Estendidas

        [XmlIgnore]
        [PersistenceProperty("NOME", DirectionParameter.InputOptional)]
        public string Nome { get; set; }

        #endregion
    }
}
