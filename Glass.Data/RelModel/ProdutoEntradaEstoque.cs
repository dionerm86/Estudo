using GDA;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProdutoEntradaEstoqueDAO))]
    [PersistenceClass("ProdutoEntradaEstoque")]
    public class ProdutoEntradaEstoque
    {
        #region Propriedades

        [PersistenceProperty("IDCOMPRA")]
        public uint? IdCompra { get; set; }

        [PersistenceProperty("NUMERONFE")]
        public uint? NumeroNFe { get; set; }

        [PersistenceProperty("IDFORNEC")]
        public uint IdFornec { get; set; }

        [PersistenceProperty("NOMEFORNEC")]
        public string NomeFornec { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("QTDEENTRADA")]
        public float QtdeEntrada { get; set; }

        [PersistenceProperty("ALTURA")]
        public float Altura { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [PersistenceProperty("TOTM")]
        public float TotM { get; set; }

        [PersistenceProperty("VALORUNIT")]
        public decimal ValorUnit { get; set; }

        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [PersistenceProperty("VALORBENEF")]
        public decimal ValorBenef { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODINTERNO")]
        public string CodInterno { get; set; }

        [PersistenceProperty("DESCRPRODUTO")]
        public string DescrProduto { get; set; }

        [PersistenceProperty("IDGRUPOPROD")]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IDSUBGRUPOPROD")]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public int TipoCalc
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(null, (int)IdGrupoProd, (int?)IdSubgrupoProd, false); }
        }

        public string AlturaRpt
        {
            get { return TipoCalc == 4 ? Altura + "ml" : Altura.ToString(); }
        }

        public string TituloAltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Altura" : "Largura"; }
        }

        public string TituloAltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Largura" : "Altura"; }
        }

        public string AltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? AlturaRpt : Largura.ToString(); }
        }

        public string AltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? Largura.ToString() : AlturaRpt; }
        }

        #endregion
    }
}
