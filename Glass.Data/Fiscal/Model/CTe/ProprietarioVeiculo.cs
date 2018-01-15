using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ProprietarioVeiculoDAO))]
    [PersistenceClass("proprietario_veiculo")]
    public class ProprietarioVeiculo
    {
        #region Propriedades

        [PersistenceProperty("IDPROPVEIC", PersistenceParameterType.IdentityKey)]
        public uint IdPropVeic { get; set; }

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [PersistenceProperty("CPF")]
        public string Cpf { get; set; }

        [PersistenceProperty("CNPJ")]
        public string Cnpj { get; set; }

        [PersistenceProperty("RNTRC")]
        public string RNTRC { get; set; }

        [PersistenceProperty("IE")]
        public string IE { get; set; }

        [PersistenceProperty("UF")]
        public string UF { get; set; }

        [PersistenceProperty("TIPOPROP")]
        public int TipoProp { get; set; }

        #endregion
    }
}
