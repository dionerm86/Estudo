using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoNfAdicaoDAO))]
    [PersistenceClass("produto_nf_adicao")]
    public class ProdutoNfAdicao
    {
        #region Propriedades

        [PersistenceProperty("IDPRODNFADICAO", PersistenceParameterType.IdentityKey)]
        public uint IdProdNfAdicao { get; set; }

        [PersistenceProperty("IDPRODNF")]
        public uint IdProdNf { get; set; }

        [PersistenceProperty("NUMADICAO")]
        public int NumAdicao { get; set; }

        [PersistenceProperty("NUMSEQADICAO")]
        public int NumSeqAdicao { get; set; }

        [PersistenceProperty("CODFABRICANTE")]
        public string CodFabricante { get; set; }

        [PersistenceProperty("DESCADICAO")]
        public float DescAdicao { get; set; }

        #endregion
    }
}