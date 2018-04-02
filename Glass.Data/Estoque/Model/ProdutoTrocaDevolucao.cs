using System.Collections.Generic;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoTrocaDevolucaoDAO))]
    [PersistenceClass("produto_troca_dev")]
    public class ProdutoTrocaDevolucao : IProdutoDescontoAcrescimo
    {
        #region Propriedades

        [PersistenceProperty("IDPRODTROCADEV", PersistenceParameterType.IdentityKey)]
        public uint IdProdTrocaDev { get; set; }

        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint IdTrocaDevolucao { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint? IdProdPed { get; set; }

        [PersistenceProperty("IDPROCESSO")]
        public uint? IdProcesso { get; set; }

        [PersistenceProperty("IDAPLICACAO")]
        public uint? IdAplicacao { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("VALORVENDIDO")]
        public decimal ValorVendido { get; set; }

        [PersistenceProperty("ALTURA")]
        public float Altura { get; set; }

        [PersistenceProperty("ALTURAREAL")]
        public float AlturaReal { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [PersistenceProperty("TOTM")]
        public float TotM { get; set; }

        [PersistenceProperty("TOTM2CALC")]
        public float TotM2Calc { get; set; }

        [PersistenceProperty("ESPESSURA")]
        public float Espessura { get; set; }

        [PersistenceProperty("REDONDO")]
        public bool Redondo { get; set; }

        [PersistenceProperty("CUSTOPROD")]
        public decimal CustoProd { get; set; }

        [PersistenceProperty("VALORBENEF")]
        public decimal ValorBenef { get; set; }

        [PersistenceProperty("ALTERARESTOQUE")]
        public bool AlterarEstoque { get; set; }

        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [PersistenceProperty("VALORACRESCIMO")]
        public decimal ValorAcrescimo { get; set; }

        [PersistenceProperty("VALORDESCONTO")]
        public decimal ValorDesconto { get; set; }

        [PersistenceProperty("VALORACRESCIMOPROD")]
        public decimal ValorAcrescimoProd { get; set; }

        [PersistenceProperty("VALORDESCONTOPROD")]
        public decimal ValorDescontoProd { get; set; }

        [PersistenceProperty("PEDCLI")]
        public string PedCli { get; set; }

        [PersistenceProperty("PERCDESCONTOQTDE")]
        public float PercDescontoQtde { get; set; }

        [PersistenceProperty("VALORDESCONTOQTDE")]
        public decimal ValorDescontoQtde { get; set; }

        [PersistenceProperty("VALORDESCONTOCLIENTE")]
        public decimal ValorDescontoCliente { get; set; }

        [PersistenceProperty("VALORACRESCIMOCLIENTE")]
        public decimal ValorAcrescimoCliente { get; set; }

        [PersistenceProperty("VALORUNITBRUTO")]
        public decimal ValorUnitarioBruto { get; set; }

        [PersistenceProperty("TOTALBRUTO")]
        public decimal TotalBruto { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("IDPEDIDO", DirectionParameter.InputOptional)]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("IDGRUPOPROD", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IDSUBGRUPOPROD", DirectionParameter.InputOptional)]
        public uint? IdSubgrupoProd { get; set; }

        [PersistenceProperty("CODAPLICACAO", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        [PersistenceProperty("CODPROCESSO", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("ISTROCA", DirectionParameter.InputOptional)]
        public bool IsTroca { get; set; }

        [PersistenceProperty("UNIDADE", DirectionParameter.InputOptional)]
        public string Unidade { get; set; }

        [PersistenceProperty("IDCLIENTE", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        [PersistenceProperty("CustoCompraProduto", DirectionParameter.InputOptional)]
        public decimal CustoCompraProduto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool EditarVisible
        {
            get { return IdProdPed == null || IdProdPed == 0; }
        }

        public string AlturaLista
        {
            get
            {
                if (IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Alumínio)
                    return Altura.ToString();
                else
                    return Altura != AlturaReal ? (AlturaReal > 0 ? AlturaReal.ToString() + " (" + Altura.ToString() + ")" : Altura.ToString()) : Altura.ToString();
            }
        }

        public int TipoCalc
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int?)IdSubgrupoProd); }
        }

        public string IsVidro
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd).ToString().ToLower(); }
        }

        public string IsAluminio
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio((int)IdGrupoProd).ToString().ToLower(); }
        }

        public bool AlturaEnabled
        {
            get { return IsTroca && Glass.Calculos.AlturaEnabled(TipoCalc); }
        }

        public bool LarguraEnabled
        {
            get { return IsTroca && Glass.Calculos.LarguraEnabled(TipoCalc); }
        }

        public float TotalM2CalcSemChapa
        {
            get { return IdProd > 0 ? Glass.Global.CalculosFluxo.CalcM2Calculo(IdCliente, (int)Altura, Largura, Qtde, (int)IdProd, Redondo, Beneficiamentos.CountAreaMinima, ProdutoDAO.Instance.ObtemAreaMinima((int)IdProd), false, 0, true) : 0; }
        }

        public bool BenefVisible
        {
            get { return EditarVisible && IsTroca && (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) || 
                Geral.UsarBeneficiamentosTodosOsGrupos) && !Geral.NaoVendeVidro(); }
        }

        public decimal ValorUnitBruto
        {
            get { return ValorUnitarioBruto; }
        }

        #endregion

        #region Propriedades do Beneficiamento

        private List<ProdutoTrocaDevolucaoBenef> _beneficiamentos = null;

        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                try
                {
                    if (!ProdutoDAO.Instance.CalculaBeneficiamento((int)IdProd))
                        _beneficiamentos = new List<ProdutoTrocaDevolucaoBenef>();

                    if (_beneficiamentos == null)
                        _beneficiamentos = ProdutoTrocaDevolucaoBenefDAO.Instance.GetByProdTrocaDev(IdProdTrocaDev);
                }
                catch
                {
                    _beneficiamentos = new List<ProdutoTrocaDevolucaoBenef>();
                }

                return _beneficiamentos;
            }
            set { _beneficiamentos = value; }
        }

        /// <summary>
        /// Recarrega a lista de beneficiamentos do banco de dados.
        /// </summary>
        public void RefreshBeneficiamentos()
        {
            _beneficiamentos = null;
        }

        public string DescrBeneficiamentos
        {
            get { return Beneficiamentos.DescricaoBeneficiamentos; }
        }

        #endregion

        #region IDescontoAcrescimo Members

        uint IProdutoDescontoAcrescimo.Id
        {
            get { return IdProdTrocaDev; }
        }

        decimal IProdutoDescontoAcrescimo.ValorUnit
        {
            get { return ValorVendido; }
            set { ValorVendido= value; }
        }

        uint IProdutoDescontoAcrescimo.IdProduto
        {
            get { return IdProd; }
        }

        public int QtdeAmbiente
        {
            get { return 1; }
        }

        decimal IProdutoDescontoAcrescimo.ValorComissao
        {
            get { return 0; }
            set { }
        }

        float IProdutoDescontoAcrescimo.AlturaCalc
        {
            get { return Altura; }
        }

        int? IProdutoDescontoAcrescimo.AlturaBenef
        {
            get { return 0; }
        }

        int? IProdutoDescontoAcrescimo.LarguraBenef
        {
            get { return 0; }
        }

        public bool RemoverDescontoQtde { get; set; }

        decimal IProdutoDescontoAcrescimo.ValorTabelaPedido
        {
            get { return 0; }
        }

        uint IProdutoDescontoAcrescimo.IdParent
        {
            get { return IdTrocaDevolucao; }
        }

        uint? IProdutoDescontoAcrescimo.IdObra
        {
            get
            {
                uint? ret = null;
                if (IdPedido > 0)
                    ret = PedidoDAO.Instance.GetIdObra(IdPedido.Value);
                else if (IdProdPed > 0)
                {
                    IdPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(IdProdPed.Value);
                    return (this as IProdutoDescontoAcrescimo).IdObra;
                }

                return ret;
            }
        }

        #endregion
    }
}