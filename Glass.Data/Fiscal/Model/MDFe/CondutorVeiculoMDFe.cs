using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(CondutorVeiculoMDFeDAO))]
    [PersistenceClass("condutor_veiculo_mdfe")]
    public class CondutorVeiculoMDFe
    {
        #region Propriedades

        [PersistenceProperty("IDRODOVIARIO")]
        [PersistenceForeignKey(typeof(RodoviarioMDFe), "IdRodoviario")]
        public int IdRodoviario { get; set; }

        [PersistenceProperty("IDCONDUTOR")]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int IdCondutor { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMECONDUTOR", DirectionParameter.InputOptional)]
        public string NomeCondutor { get; set; }

        #endregion
    }
}
