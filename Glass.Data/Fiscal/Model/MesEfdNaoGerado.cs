using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MesEfdNaoGeradoDAO))]
    [PersistenceClass("mes_efd_nao_gerado")]
    public class MesEfdNaoGerado
    {
        #region Propriedades

        [PersistenceProperty("IDMESEFDNAOGERADO", PersistenceParameterType.IdentityKey)]
        public uint IdMesEfdNaoGerado { get; set; }

        [PersistenceProperty("MES")]
        public int Mes { get; set; }

        [PersistenceProperty("ANO")]
        public int Ano { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion
    }
}