using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoMoldeDAO))]
    [PersistenceClass("produto_molde")]
    public class ProdutoMolde
    {
        #region Propriedades

        [PersistenceProperty("IDPRODMOLDE", PersistenceParameterType.IdentityKey)]
        public uint IdProdMolde { get; set; }

        [PersistenceProperty("IDMOLDE")]
        public uint IdMolde { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint IdProdPed { get; set; }

        [PersistenceProperty("NUMTABELA")]
        public int NumeroTabela { get; set; }

        [PersistenceProperty("QTDE")]
        public int Qtde { get; set; }

        [PersistenceProperty("ALTURA")]
        public float Altura { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("AMBIENTE", DirectionParameter.InputOptional)]
        public string Ambiente { get; set; }

        [PersistenceProperty("IDPROD", DirectionParameter.InputOptional)]
        public uint IdProduto { get; set; }

        [PersistenceProperty("CODINTERNOPRODUTO", DirectionParameter.InputOptional)]
        public string CodInternoProduto { get; set; }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("QTDEORIGINAL", DirectionParameter.InputOptional)]
        public int QtdeOriginal { get; set; }

        [PersistenceProperty("ALTURAORIGINAL", DirectionParameter.InputOptional)]
        public float AlturaOriginal { get; set; }

        [PersistenceProperty("LARGURAORIGINAL", DirectionParameter.InputOptional)]
        public int LarguraOriginal { get; set; }

        [PersistenceProperty("QTDEJAUTILIZADA", DirectionParameter.InputOptional)]
        public long QtdeJaUtilizada { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescricaoRpt
        {
            get { return CodInternoProduto + " - " + DescrProduto + "  " + Altura + "x" + Largura + "  Qtd. " + Qtde; }
        }

        public float TotM
        {
            get { return Glass.Global.CalculosFluxo.ArredondaM2(Largura, (int)Altura, Qtde, (int)IdProduto, false); }
        }

        public int QtdeMaxima
        {
            get { return QtdeOriginal - (int)QtdeJaUtilizada + Qtde; }
        }

        #endregion
    }
}