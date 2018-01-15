using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(IcmsProdutoUfDAO))]
    [PersistenceClass("icms_produto_uf")]
    public class IcmsProdutoUf : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDICMSPRODUF", PersistenceParameterType.IdentityKey)]
        public int IdIcmsProdUf { get; set; }

        [PersistenceProperty("IDPROD")]
        [PersistenceForeignKey(typeof(Model.Produto), "IdProd")]
        public int IdProd { get; set; }

        [PersistenceProperty("UFORIGEM")]
        public string UfOrigem { get; set; }

        [PersistenceProperty("UFDESTINO")]
        public string UfDestino { get; set; }

        [PersistenceProperty("IDTIPOCLIENTE")]
        [PersistenceForeignKey(typeof(TipoCliente), "IdTipoCliente")]
        public int? IdTipoCliente { get; set; }

        [PersistenceProperty("ALIQUOTAINTRA")]
        public float AliquotaIntraestadual { get; set; }

        [PersistenceProperty("ALIQUOTAINTER")]
        public float AliquotaInterestadual { get; set; }
 
        [PersistenceProperty("ALIQUOTAINTERNADESTINATARIO")]
        public float AliquotaInternaDestinatario { get; set; }

        #endregion
    }
}
