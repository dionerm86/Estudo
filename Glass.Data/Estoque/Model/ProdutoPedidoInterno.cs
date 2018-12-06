using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoPedidoInternoDAO))]
    [PersistenceClass("produto_pedido_interno")]
    public class ProdutoPedidoInterno
    {
        #region Propriedades

        [PersistenceProperty("IDPRODPEDINTERNO", PersistenceParameterType.IdentityKey)]
        public uint IdProdPedInterno { get; set; }

        [PersistenceProperty("IDPEDIDOINTERNO")]
        public uint IdPedidoInterno { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("ALTURA")]
        public float Altura { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("TOTM")]
        public float TotM { get; set; }

        [PersistenceProperty("QTDECONFIRMADA", DirectionParameter.Input)]
        public float QtdeConfirmada { get; set; }

        [PersistenceProperty("OBS")]
        public string Observacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("IDGRUPOPROD", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IDSUBGRUPOPROD", DirectionParameter.InputOptional)]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("QTDESOMADA", DirectionParameter.InputOptional)]
        public double QtdeSomada { get; set; }

        [PersistenceProperty("TotM2", DirectionParameter.InputOptional)]
        public double TotM2 { get; set; }

        [PersistenceProperty("Custo", DirectionParameter.InputOptional)]
        public double Custo { get; set; }

        #endregion

        #region Propriedades de Suporte

        public int TipoCalc
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, (int)IdGrupoProd, (int?)IdSubgrupoProd, false); }
        }

        public bool AlturaEnabled
        {
            get { return Glass.Calculos.AlturaEnabled(TipoCalc); }
        }

        public bool LarguraEnabled
        {
            get { return Glass.Calculos.LarguraEnabled(TipoCalc); }
        }

        public float QtdeConfirmar
        {
            get { return (ConfirmarQtde ? Qtde : TotM) - QtdeConfirmada; }
        }

        public bool ConfirmarQtde
        {
            get { return TipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 && TipoCalc != (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto; }
        }

        #endregion
    }
}
