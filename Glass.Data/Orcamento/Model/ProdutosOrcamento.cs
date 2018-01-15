using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.IO;
using Glass.Configuracoes;
using System.Xml.Serialization;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutosOrcamentoDAO))]
	[PersistenceClass("produtos_orcamento")]
	public class ProdutosOrcamento : Colosoft.Data.BaseModel, IDescontoAcrescimo
    {
        #region Construtores

        public ProdutosOrcamento()
        {
            TipoAcrescimo = 2;
            TipoDesconto = 2;
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPROD", PersistenceParameterType.IdentityKey)]
        public uint IdProd { get; set; }

        [PersistenceProperty("IDORCAMENTO")]
        public uint IdOrcamento { get; set; }

        [PersistenceProperty("IDITEMPROJETO")]
        public uint? IdItemProjeto { get; set; }

        [PersistenceProperty("IDPRODUTO")]
        public uint? IdProduto { get; set; }

        [PersistenceProperty("IDAMBIENTEORCA")]
        public uint? IdAmbienteOrca { get; set; }

        [PersistenceProperty("QTDE")]
        public float? Qtde { get; set; }

        internal bool usarAmbientePai = true;
        private bool _ambientePai = false;
        private string _ambiente;

        [PersistenceProperty("AMBIENTE")]
        public string Ambiente
        {
            get
            {
                if (IdProdParent != null && (usarAmbientePai && !_ambientePai))
                {
                    try
                    {
                        _ambiente = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<string>("ambiente", "idProd=" + IdProdParent.Value);
                        _ambientePai = true;
                    }
                    catch { }
                }

                return _ambiente;
            }
            set
            {
                _ambientePai = false;
                _ambiente = value;
            }
        }

        [PersistenceProperty("Descricao")]
        public string Descricao { get; set; }

        [PersistenceProperty("VALORPROD")]
        public decimal? ValorProd { get; set; }

        private decimal? _total;

        [PersistenceProperty("Total")]
        public decimal? Total
        {
            get { return _total == 0 ? null : _total; }
            set { _total = value; }
        }

        [PersistenceProperty("ALTURA")]
        public Single Altura { get; set; }

        [PersistenceProperty("ALTURACALC")]
        public float AlturaCalc { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [PersistenceProperty("REDONDO")]
        public bool Redondo { get; set; }

        [PersistenceProperty("TOTM")]
        public float TotM { get; set; }

        [PersistenceProperty("TOTMCALC")]
        public float TotMCalc { get; set; }

        [PersistenceProperty("VALORBENEF")]
        public decimal ValorBenef { get; set; }

        [PersistenceProperty("IDPRODPARENT")]
        public uint? IdProdParent { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public uint NumSeq { get; set; }

        [PersistenceProperty("ESPESSURA")]
        public float Espessura { get; set; }

        [PersistenceProperty("CUSTO")]
        public decimal Custo { get; set; }

        [PersistenceProperty("VALORACRESCIMO")]
        public decimal ValorAcrescimo { get; set; }

        [PersistenceProperty("VALORDESCONTO")]
        public decimal ValorDesconto { get; set; }

        [PersistenceProperty("ALIQUOTAICMS")]
        public float AliquotaIcms { get; set; }

        [PersistenceProperty("VALORICMS")]
        public decimal ValorIcms { get; set; }

        [PersistenceProperty("ALIQUOTAIPI")]
        public float AliquotaIpi { get; set; }

        [PersistenceProperty("VALORIPI")]
        public decimal ValorIpi { get; set; }

        [PersistenceProperty("NEGOCIAR", DirectionParameter.Input)]
        public bool Negociar { get; set; }

        [PersistenceProperty("IDPRODPED", DirectionParameter.Input)]
        public uint? IdProdPed { get; set; }

        [PersistenceProperty("TIPODESCONTO")]
        public int TipoDesconto { get; set; }

        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        [PersistenceProperty("TIPOACRESCIMO")]
        public int TipoAcrescimo { get; set; }

        [PersistenceProperty("ACRESCIMO")]
        public decimal Acrescimo { get; set; }

        [PersistenceProperty("VALORACRESCIMOPROD")]
        public decimal ValorAcrescimoProd { get; set; }

        [PersistenceProperty("VALORDESCONTOPROD")]
        public decimal ValorDescontoProd { get; set; }

        [PersistenceProperty("VALORTABELA")]
        public decimal ValorTabela { get; set; }

        [PersistenceProperty("TIPOCALCULOUSADO")]
        public int TipoCalculoUsado { get; set; }

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

        [PersistenceProperty("VALORCOMISSAO")]
        public decimal ValorComissao { get; set; }

        [PersistenceProperty("Peso", DirectionParameter.Input)]
        public float Peso { get; set; }

        [PersistenceProperty("IDPROCESSO")]
        public uint? IdProcesso { get; set; }

        [PersistenceProperty("IDAPLICACAO")]
        public uint? IdAplicacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NUMCHILD", DirectionParameter.InputOptional)]
        public long? NumChild { get; set; }

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        private string _descrProduto;

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto
        {
            get { return !String.IsNullOrEmpty(_descrProduto) ? _descrProduto : Descricao; }
            set { _descrProduto = value; }
        }

        [PersistenceProperty("VALORPRODUTOTABELA", DirectionParameter.InputOptional)]
        public decimal ValorProdutoTabela { get; set; }

        [PersistenceProperty("UNIDADE", DirectionParameter.InputOptional)]
        public string Unidade { get; set; }

        [PersistenceProperty("OBSPROJ", DirectionParameter.InputOptional)]
        public string ObsProj { get; set; }

        [PersistenceProperty("IDCLIENTE", DirectionParameter.InputOptional)]
        public uint? IdCliente { get; set; }

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("CODPROCESSO", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("CODAPLICACAO", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }
        
        [PersistenceProperty("ISBENEFICIAMENTO", DirectionParameter.InputOptional)]
        public bool IsBeneficiamento { get; set; }

        [XmlIgnore]
        [PersistenceProperty("PecaOtimizada", DirectionParameter.InputOptional)]
        public bool? PecaOtimizada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("GrauCorte", DirectionParameter.InputOptional)]
        public GrauCorteEnum? GrauCorte { get; set; }

        [XmlIgnore]
        [PersistenceProperty("ProjetoEsquadria", DirectionParameter.InputOptional)]
        public bool? ProjetoEsquadria { get; set; }

        #endregion

        #region Propriedades de Suporte

        #region Métodos internos estáticos

        internal static decimal GetTotalSemDesconto(ProdutosOrcamento p)
        {
            if (p._total == null)
                return 0;

            return !PedidoConfig.RatearDescontoProdutos ? p._total.Value :
                p.TipoDesconto == 1 ? p._total.Value * (100 / (100 - p.Desconto)) : p._total.Value + p.Desconto;
        }

        internal static decimal GetTotalSemAcrescimo(ProdutosOrcamento p)
        {
            if (p._total == null)
                return 0;

            return p.TipoAcrescimo == 1 ? p._total.Value * (100 / (100 - p.Acrescimo)) : p._total.Value - p.Acrescimo;
        }

        internal static decimal GetValorDesconto(ProdutosOrcamento p)
        {
            if (p._total == null)
                return 0;

            return Math.Abs(GetTotalSemDesconto(p) - p._total.Value);
        }

        internal static decimal GetValorAcrescimo(ProdutosOrcamento p)
        {
            if (p._total == null)
                return 0;

            return Math.Abs(p._total.Value - GetTotalSemAcrescimo(p));
        }

        #endregion

        public string ImagemProjModPath { get; set; }

        public string NumItem { get; set; }

        public string DadosProdutos { get; set; }

        public bool TemItensProduto
        {
            get
            {
                if (NumChild == null)
                    NumChild = ProdutosOrcamentoDAO.Instance.ContainsChildItems(IdProd) ? 1 : 0;

                return NumChild > 0;
            }
        }

        public bool TemItensProdutoSession(GDASession session)
        {
            if (NumChild == null)
                NumChild = ProdutosOrcamentoDAO.Instance.ContainsChildItems(session, IdProd) ? 1 : 0;

            return NumChild > 0;
        }

        private decimal? _custoUnit = null;

        public decimal CustoUnit
        {
            get
            {
                if (_custoUnit == null && IdProduto != null)
                {
                    Produto prod = ProdutoDAO.Instance.GetElement(IdProduto.Value);
                    _custoUnit = prod.CustoCompra;
                }

                return _custoUnit.GetValueOrDefault();
            }
        }

        private bool _exibirAmbiente = true;

        public bool ExibirAmbiente
        {
            get { return _exibirAmbiente; }
            set { _exibirAmbiente = value; }
        }

        public string TextoDesconto
        {
            get { return TipoDesconto == 1 ? Desconto + "%" : Desconto.ToString("C"); }
        }

        public string TextoAcrescimo
        {
            get { return TipoAcrescimo == 1 ? Acrescimo + "%" : Acrescimo.ToString("C"); }
        }

        public decimal ValorDescontoAtual
        {
            get { return GetValorDesconto(this); }
        }

        public decimal ValorAcrescimoAtual
        {
            get { return GetValorAcrescimo(this); }
        }

        public string DescrObsProj
        {
            get { return !String.IsNullOrEmpty(ObsProj) ? "(Obs. Projeto: " + ObsProj + ")" : String.Empty; }
        }

        public string NomeImagem
        {
            get { return IdProd + ".jpg"; }
        }

        public string ImagemUrl
        {
            get { return Utils.GetProdutosOrcamentoVirtualPath + NomeImagem; }
        }

        public bool TemImagem
        {
            get { return File.Exists(Utils.GetProdutosOrcamentoPath + NomeImagem); }
        }

        public float TotM2SemChapa
        {
            get 
            { 
                return IdProduto.GetValueOrDefault() == 0 || Qtde.GetValueOrDefault() == 0 ? 0 :
                    Glass.Global.CalculosFluxo.CalcM2Calculo(IdCliente.GetValueOrDefault(), (int)Altura, Largura, Qtde.Value, (int)IdProduto.Value, Redondo, Beneficiamentos.CountAreaMinima, ProdutoDAO.Instance.ObtemAreaMinima((int?)IdProduto), false, 0, true); 
            }
        }

        /// <summary>
        /// Indica se o produto pode ser editado no orçamento
        /// (se o pedido não tem desconto, ou se a empresa não rateia o desconto).
        /// </summary>
        public bool PodeEditar
        {
            get
            {
                if (!PedidoConfig.RatearDescontoProdutos)
                    return true;

                decimal descontoOrcamento = OrcamentoDAO.Instance.ObtemValorCampo<decimal>("desconto", "idOrcamento=" + IdOrcamento);
                decimal descontoAmbiente = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<decimal>("desconto", "idProd=" + IdProdParent.GetValueOrDefault());
                return (descontoOrcamento + descontoAmbiente) == 0;
            }
        }

        public bool DescontoAcrescimoPermitido
        {
            get { return PedidoConfig.DadosPedido.AmbientePedido || IdItemProjeto > 0; }
        }

        public decimal TotalAmbiente
        {
            get
            {
                return Total.GetValueOrDefault();
            }
        }

        public decimal ValorProdAmbiente
        {
            get
            {
                return ValorProd.GetValueOrDefault();
            }
        }

        [XmlIgnore]
        public string CodInternoDescProd
        {
            get { return CodInterno + " - " + DescrProduto; }
        }

        #endregion

        #region Propriedades do Beneficiamento

        private List<ProdutoOrcamentoBenef> _beneficiamentos = null;

        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                try
                {
                    if (IdProduto.GetValueOrDefault() == 0 || !ProdutoDAO.Instance.CalculaBeneficiamento((int)IdProduto.Value))
                        _beneficiamentos = new List<ProdutoOrcamentoBenef>();

                    if (_beneficiamentos == null)
                        _beneficiamentos = new List<ProdutoOrcamentoBenef>(ProdutoOrcamentoBenefDAO.Instance.GetByProdutoOrcamento(IdProd));
                }
                catch
                {
                    _beneficiamentos = new List<ProdutoOrcamentoBenef>();
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

        #region IDescontoAcrescimo

        uint IDescontoAcrescimo.Id
        {
            get { return IdProd; }
        }

        uint IDescontoAcrescimo.IdParent
        {
            get { return IdOrcamento; }
        }

        decimal IDescontoAcrescimo.ValorUnit
        {
            get { return ValorProd != null ? ValorProd.Value : 0; }
            set { ValorProd = value; }
        }

        decimal IDescontoAcrescimo.Total
        {
            get { return _total != null ? _total.Value : 0; }
            set { _total = value; }
        }

        float IDescontoAcrescimo.Qtde
        {
            get { return Qtde != null ? Qtde.Value : 0; }
        }

        float IDescontoAcrescimo.TotM2Calc
        {
            get { return TotMCalc; }
        }

        uint IDescontoAcrescimo.IdProduto
        {
            get { return IdProduto != null ? IdProduto.Value : 0; }
        }

        public int QtdeAmbiente
        {
            get { return 1; }
        }

        private bool _removerDescontoQtde = false;

        public bool RemoverDescontoQtde
        {
            get { return _removerDescontoQtde; }
            set { _removerDescontoQtde = value; }
        }

        uint? IDescontoAcrescimo.IdObra
        {
            get { return null; }
        }

        int? IDescontoAcrescimo.AlturaBenef
        {
            get { return 0; }
        }

        int? IDescontoAcrescimo.LarguraBenef
        {
            get { return 0; }
        }

        decimal IDescontoAcrescimo.ValorTabelaPedido
        {
            get { return 0; }
        }

        #endregion
    }
}