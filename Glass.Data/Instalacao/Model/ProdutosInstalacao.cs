using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutosInstalacaoDAO))]
    [PersistenceClass("produtos_instalacao")]
    public class ProdutosInstalacao
    {
        #region Propriedades

        [PersistenceProperty("IDPRODINST", PersistenceParameterType.IdentityKey)]
        public uint IdProdInst { get; set; }

        [PersistenceProperty("IDINSTALACAO")]
        public uint IdInstalacao { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint IdProdPed { get; set; }

        [PersistenceProperty("QTDEINSTALADA")]
        public int QtdeInstalada { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("IDPROD", DirectionParameter.InputOptional)]
        public uint IdProd { get; set; }

        [PersistenceProperty("QTDE", DirectionParameter.InputOptional)]
        public float Qtde { get; set; }

        [PersistenceProperty("VALORVENDIDO", DirectionParameter.InputOptional)]
        public decimal ValorVendido { get; set; }

        [PersistenceProperty("ALTURA", DirectionParameter.InputOptional)]
        public Single Altura { get; set; }

        [PersistenceProperty("LARGURA", DirectionParameter.InputOptional)]
        public int Largura { get; set; }

        [PersistenceProperty("TOTM", DirectionParameter.InputOptional)]
        public Single TotM { get; set; }

        [PersistenceProperty("TOTAL", DirectionParameter.InputOptional)]
        public decimal Total { get; set; }

        [PersistenceProperty("VALORBENEF", DirectionParameter.InputOptional)]
        public decimal ValorBenef { get; set; }

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("IdGrupoProd", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IdSubgrupoProd", DirectionParameter.InputOptional)]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("IdAmbientePedido", DirectionParameter.InputOptional)]
        public uint? IdAmbientePedido { get; set; }

        [PersistenceProperty("Ambiente", DirectionParameter.InputOptional)]
        public string Ambiente { get; set; }

        [PersistenceProperty("DescrAmbiente", DirectionParameter.InputOptional)]
        public string DescrAmbiente { get; set; }

        [PersistenceProperty("ValorProdutos", DirectionParameter.InputOptional)]
        public decimal ValorProdutos { get; set; }

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

        #region Propriedades dos Beneficiamentos do Produto pedido

        private List<ProdutoPedidoBenef> _beneficiamentos = null;

        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                if (!ProdutoDAO.Instance.CalculaBeneficiamento((int)IdProd))
                    _beneficiamentos = new List<ProdutoPedidoBenef>();

                if (_beneficiamentos == null)
                    _beneficiamentos = new List<ProdutoPedidoBenef>(ProdutoPedidoBenefDAO.Instance.GetByProdutoPedido(IdProdPed));

                return _beneficiamentos;
            }
        }

        public string DescrBeneficiamentos
        {
            get { return Beneficiamentos.DescricaoBeneficiamentos; }
        }

        #endregion
    }
}
