using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoProjetoConfigDAO))]
	[PersistenceClass("produto_projeto_config")]
	public class ProdutoProjetoConfig
    {
        #region Propriedades

        [PersistenceProperty("IDPRODPROJCONFIG", PersistenceParameterType.IdentityKey)]
        public uint IdProdProjConfig { get; set; }

        [PersistenceProperty("IDPRODPROJ")]
        public uint IdProdProj { get; set; }

        [Log("Cor Alumínio", "Descricao", typeof(CorAluminioDAO))]
        [PersistenceProperty("IDCORALUMINIO")]
        public uint? IdCorAluminio { get; set; }

        [Log("Cor Ferragem", "Descricao", typeof(CorFerragemDAO))]
        [PersistenceProperty("IDCORFERRAGEM")]
        public uint? IdCorFerragem { get; set; }

        [Log("Cor Vidro", "Descricao", typeof(CorVidroDAO))]
        [PersistenceProperty("IDCORVIDRO")]
        public uint? IdCorVidro { get; set; }

        [Log("Produto", "Descricao", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRPROD", DirectionParameter.InputOptional)]
        public string DescrProd { get; set; }

        [PersistenceProperty("CODINTERNOPROD", DirectionParameter.InputOptional)]
        public string CodInternoProd { get; set; }

        [PersistenceProperty("IDCORPRODUTO", DirectionParameter.InputOptional)]
        public uint IdCorProduto { get; set; }

        [PersistenceProperty("DESCRCORPRODUTO", DirectionParameter.InputOptional)]
        public string DescrCorProduto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string CodDescrProd
        {
            get { return CodInternoProd + " - " + DescrProd; }
        }

        #endregion
    }
}