using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(VolumeProdutosPedidoDAO))]
    [PersistenceClass("volume_produtos_pedido")]
    public class VolumeProdutosPedido
    {
        #region Propiedades

        [PersistenceProperty("IDPRODPED", PersistenceParameterType.Key)]
        public uint IdProdPed { get; set; }

        [PersistenceProperty("IDVOLUME", PersistenceParameterType.Key)]
        public uint IdVolume { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        #endregion

        #region Propiedades de Estendidas

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodInternoProd { get; set; }

        [PersistenceProperty("DESCRICAO", DirectionParameter.InputOptional)]
        public string DescProd { get; set; }

        [PersistenceProperty("ALTURA", DirectionParameter.InputOptional)]
        public float AlturaProd { get; set; }

        [PersistenceProperty("LARGURA", DirectionParameter.InputOptional)]
        public float LarguraProd { get; set; }

        [PersistenceProperty("QTDEPRODPED", DirectionParameter.InputOptional)]
        public float QtdeProdPed { get; set; }

        [PersistenceProperty("IDPROD", DirectionParameter.InputOptional)]
        public uint IdProd { get; set; }

        [PersistenceProperty("NOMESUBGRUPOPROD", DirectionParameter.InputOptional)]
        public string NomeSubGrupoProd { get; set; }

        #endregion

        #region Prodiedades de Suporte

        public string CodInternoDescProd
        {
            get { return CodInternoProd + " - " + DescProd; }
        }

        public int TipoCalc
        {
            get
            {
                return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdProd);
            }
        }

        #endregion
    }
}
