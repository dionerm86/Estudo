using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.IO;
using Glass.Configuracoes;
using System.Xml.Serialization;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutosOrcamentoDAO))]
	[PersistenceClass("produtos_orcamento")]
	public class ProdutosOrcamento : Colosoft.Data.BaseModel, IProdutoCalculo, IAmbienteCalculo
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

        [PersistenceProperty("IDPRODORCAMENTOPARENT")]
        public int? IdProdOrcamentoParent { get; set; }

        [PersistenceProperty("IDORCAMENTO")]
        public uint IdOrcamento { get; set; }

        [PersistenceProperty("IDITEMPROJETO")]
        public uint? IdItemProjeto { get; set; }

        [PersistenceProperty("IDMATERITEMPROJ")]
        public int? IdMaterItemProj { get; set; }

        [PersistenceProperty("IDPRODUTO")]
        public uint? IdProduto { get; set; }

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

        [PersistenceProperty("IDPRODBAIXAEST")]
        public int? IdProdBaixaEst { get; set; }

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

        [PersistenceProperty("NEGOCIAR")]
        public bool Negociar { get; set; }

        [PersistenceProperty("IDAMBIENTEPEDIDO", DirectionParameter.Input)]
        public int? IdAmbientePedido { get; set; }

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

        [PersistenceProperty("IDNATUREZAOPERACAO")]
        public uint? IdNaturezaOperacao { get; set; }

        /// <summary>
        /// Base de calculo COFINS
        /// </summary>
        [PersistenceProperty("BCCOFINS")]
        public decimal BcCofins { get; set; }

        /// <summary>
        /// Alíquota CONFINS
        /// </summary>
        [PersistenceProperty("ALIQCOFINS")]
        public float AliqCofins { get; set; }

        /// <summary>
        /// Valor COFINS
        /// </summary>
        [PersistenceProperty("VALORCOFINS")]
        public decimal ValorCofins { get; set; }

        /// <summary>
        /// Base de calculo ICMS
        /// </summary>
        [PersistenceProperty("BCICMS")]
        public decimal BcIcms { get; set; }

        /// <summary>
        /// Valor do ICMS desonerado
        /// </summary>
        [PersistenceProperty("VALORICMSDESONERADO")]
        public decimal ValorIcmsDesonerado { get; set; }

        /// <summary>
        /// Percentual de crédito base calculo ICMS
        /// </summary>
        [PersistenceProperty("PERCREDBCICMS")]
        public float PercRedBcIcms { get; set; }

        /// <summary>
        /// Base de calculo ICMSST
        /// </summary>
        [PersistenceProperty("BCICMSST")]
        public decimal BcIcmsSt { get; set; }

        /// <summary>
        /// Alíquota ICMSST
        /// </summary>
        [PersistenceProperty("ALIQICMSST")]
        public float AliqIcmsSt { get; set; }

        /// <summary>
        /// Valor do ICMSST
        /// </summary>
        [PersistenceProperty("VALORICMSST")]
        public decimal ValorIcmsSt { get; set; }

        /// <summary>
        /// Percentual de crédito base calculo ICMSST
        /// </summary>
        [PersistenceProperty("PERCREDBCICMSST")]
        public decimal PercRedBcIcmsSt { get; set; }

        /// <summary>
        /// Base de calculo PIS
        /// </summary>
        [PersistenceProperty("BCPIS")]
        public decimal BcPis { get; set; }

        /// <summary>
        /// Aliquota PIS
        /// </summary>
        [PersistenceProperty("ALIQPIS")]
        public float AliqPis { get; set; }

        /// <summary>
        /// Valor PIS
        /// </summary>
        [PersistenceProperty("VALORPIS")]
        public decimal ValorPis { get; set; }

        /// <summary>
        /// CST
        /// </summary>
        [PersistenceProperty("CST")]
        public int Cst { get; set; }

        /// <summary>
        /// CSOSN
        /// </summary>
        [PersistenceProperty("CSOSN")]
        public int Csosn { get; set; }

        /// <summary>
        /// CST COFINS
        /// </summary>
        [PersistenceProperty("CSTCOFINS")]
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstCofins { get; set; }

        /// <summary>
        /// CST PIS
        /// </summary>
        [PersistenceProperty("CSTPIS")]
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstPis { get; set; }

        /// <summary>
        /// MVA
        /// </summary>
        [PersistenceProperty("MVA")]
        public float Mva { get; set; }

        /// <summary>
        /// Código do valor fiscal de ICMS do produto do pedido
        /// </summary>
        [PersistenceProperty("CODVALORFISCAL")]
        public int? CodValorFiscal { get; set; }

        /// <summary>
        /// Cód de subistituição tributaria do IPI
        /// </summary>
        [PersistenceProperty("CSTIPI")]
        public Sync.Fiscal.Enumeracao.Cst.CstIpi? CstIpi { get; set; }

        /// <summary>
        /// Base de calc. do FCP ST
        /// </summary>
        [PersistenceProperty("BCFCPST")]
        public decimal BcFcpSt { get; set; }

        /// <summary>
        /// Aliquota do FCP ST
        /// </summary>
        [PersistenceProperty("ALIQFCPST")]
        public float AliqFcpSt { get; set; }

        /// <summary>
        /// Valor do FCP ST
        /// </summary>
        [PersistenceProperty("VALORFCPST")]
        public decimal ValorFcpSt { get; set; }

        /// <summary>
        /// Base de calc. do FCP
        /// </summary>
        [PersistenceProperty("BCFCP")]
        public decimal BcFcp { get; set; }

        /// <summary>
        /// Aliquota do FCP
        /// </summary>
        [PersistenceProperty("ALIQFCP")]
        public float AliqFcp { get; set; }

        /// <summary>
        /// Valor do FCP
        /// </summary>
        [PersistenceProperty("VALORFCP")]
        public decimal ValorFcp { get; set; }

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        [PersistenceProperty("PERCENTUALRENTABILIDADE")]
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Valor da rentabilidade financeira.
        /// </summary>
        [PersistenceProperty("RENTABILIDADEFINANCEIRA")]
        public decimal RentabilidadeFinanceira { get; set; }

        #endregion

        #region Propriedades Estendidas

        private decimal _totalProdutos;

        [PersistenceProperty("TOTALPRODUTOS", DirectionParameter.InputOptional)]
        public decimal TotalProdutos
        {
            get { return _totalProdutos - (!PedidoConfig.RatearDescontoProdutos ? ValorDescontoAtualAmbiente : 0); }
            set { _totalProdutos = value; }
        }

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

        [PersistenceProperty("IDMATERPROJMOD", DirectionParameter.InputOptional)]
        public int? IdMaterProjMod { get; set; }

        [PersistenceProperty("IDPECAITEMPROJ", DirectionParameter.InputOptional)]
        public int? IdPecaItemProj { get; set; }

        private string _m2Minimo;

        [PersistenceProperty("AreaMinima", DirectionParameter.InputOptional)]
        public string M2Minimo
        {
            get { return !string.IsNullOrWhiteSpace(_m2Minimo) ? _m2Minimo.ToString().Replace(',', '.') : "0"; }
            set { _m2Minimo = value; }
        }

        [PersistenceProperty("CODNATUREZAOPERACAO", DirectionParameter.InputOptional)]
        public string CodNaturezaOperacao { get; set; }

        [XmlIgnore]
        [PersistenceProperty("PecaOtimizada", DirectionParameter.InputOptional)]
        public bool? PecaOtimizada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("GrauCorte", DirectionParameter.InputOptional)]
        public GrauCorteEnum? GrauCorte { get; set; }

        [XmlIgnore]
        [PersistenceProperty("ProjetoEsquadria", DirectionParameter.InputOptional)]
        public bool? ProjetoEsquadria { get; set; }

        [PersistenceProperty("NOMSUBGRUPOPROD", DirectionParameter.InputOptional)]
        public string NomeSubGrupoProd { get; set; }

        #endregion

        #region Propriedades de Suporte

        #region Métodos internos estáticos

        #region Produto orçamento

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

        #region Produto ambiente orçamento

        internal static decimal ObterValorDescontoAmbiente(ProdutosOrcamento produtoAmbiente)
        {
            return produtoAmbiente.TipoDesconto == 1 ? (decimal)produtoAmbiente._totalProdutos * (produtoAmbiente.Desconto / 100) : produtoAmbiente.Desconto;
        }

        #endregion

        #endregion

        /// <summary>
        /// Identificador do processo das peças filhas
        /// que é informado na insersão do produto no pedido
        /// </summary>
        public int? IdProcessoFilhas { get; set; }

        /// <summary>
        /// Identificador do processo das peças filhas
        /// que é informado na insersão do produto no pedido
        /// </summary>
        public int? IdAplicacaoFilhas { get; set; }

        public bool AplicarBenefComposicao { get; set; }

        public string AlturaLista
        {
            get
            {
                if (IdGrupoProd != (uint)NomeGrupoProd.Alumínio)
                {
                    return Altura.ToString();
                }
                else
                {
                    return Altura != AlturaCalc ? (AlturaCalc > 0 ? $"{ AlturaCalc.ToString() } ({ Altura.ToString() })" : Altura.ToString()) : Altura.ToString();
                }
            }
        }

        public bool ProjetoVisible
        {
            get { return IdItemProjeto > 0 && IdMaterItemProj.GetValueOrDefault() == 0; }
        }

        public bool PodeEditar
        {
            get
            {
                if (!PedidoConfig.RatearDescontoProdutos)
                {
                    return true;
                }

                var descontoOrcamento = OrcamentoDAO.Instance.ObterDesconto(null, (int)IdOrcamento);
                var descontoAmbiente = ProdutosOrcamentoDAO.Instance.ObterDesconto(null, (int)IdProd);
                return (descontoOrcamento + descontoAmbiente) == 0;
            }
        }

        public bool PodeEditarComposicao
        {
            get
            {
                // Se o produto for composição e tipo subgrupo Vidro Laminado ou a forma de pagamento for Obra.
                if (IdProdOrcamentoParent > 0 && ProdutosOrcamentoDAO.Instance.VerificarProdutoLaminado(null, IdProdOrcamentoParent.Value))
                {
                    return false;
                }

                return true;
            }
        }

        public bool DeleteVisible
        {
            get
            {
                if (IdProdOrcamentoParent > 0 && ProdutosOrcamentoDAO.Instance.VerificarProdutoLaminado(null, IdProdOrcamentoParent.Value))
                {
                    return false;
                }

                if (IdMaterItemProj == null)
                {
                    return true;
                }

                return MaterialItemProjetoDAO.Instance.GetElement((uint)IdMaterItemProj.Value).DeleteVisible;
            }
        }

        private int? _idGrupoProd;

        public int? IdGrupoProd
        {
            get
            {
                if (IdProduto > 0 && _idGrupoProd.GetValueOrDefault() == 0)
                {
                    _idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(null, (int)IdProduto);
                }

                return _idGrupoProd;
            }
        }

        private int? _idSubgrupoProd;

        public int? IdSubgrupoProd
        {
            get
            {
                if (IdProduto > 0 && _idSubgrupoProd.GetValueOrDefault() == 0)
                {
                    _idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(null, (int)IdProduto);
                }

                return _idSubgrupoProd;
            }
        }

        public string IsVidro
        {
            get { return GrupoProdDAO.Instance.IsVidro(IdGrupoProd.GetValueOrDefault()).ToString().ToLower(); }
        }

        public string IsAluminio
        {
            get { return GrupoProdDAO.Instance.IsAluminio(IdGrupoProd.GetValueOrDefault()).ToString().ToLower(); }
        }

        private int _tipoCalc;

        public int TipoCalc
        {
            get
            {
                if (IdProduto > 0 && IdGrupoProd > 0 && _tipoCalc == 0)
                {
                    _tipoCalc = GrupoProdDAO.Instance.TipoCalculo(IdGrupoProd.Value, IdSubgrupoProd.Value);
                }

                return _tipoCalc;
            }
        }

        public bool IsProdLamComposicao
        {
            get
            {
                if (IdProduto > 0)
                {
                    var idsSubgrupoComposicao = new List<int>() { (int)TipoSubgrupoProd.VidroLaminado, (int)TipoSubgrupoProd.VidroDuplo };

                    return OrcamentoDAO.Instance.ObterTipoOrcamento(null, (int)IdOrcamento) == (int)Orcamento.TipoOrcamentoEnum.Venda &&
                        idsSubgrupoComposicao.Contains((int)SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(null, (int)IdProduto));
                }

                return false;
            }
        }

        public bool IsProdLamComposicaoComFilho
        {
            get
            {
                if (IdProduto > 0)
                {
                    var idsSubgrupoComposicao = new List<int>() { (int)TipoSubgrupoProd.VidroLaminado, (int)TipoSubgrupoProd.VidroDuplo };

                    return OrcamentoDAO.Instance.ObterTipoOrcamento(null, (int)IdOrcamento) == (int)Orcamento.TipoOrcamentoEnum.Venda &&
                        idsSubgrupoComposicao.Contains((int)SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(null, (int)IdProduto)) &&
                        ProdutosOrcamentoDAO.Instance.VerificarTemFilhoComposicao(null, (int)IdProd) && IdProdOrcamentoParent > 0;
                }

                return false;
            }
        }

        public bool AlturaEnabled
        {
            get { return Glass.Calculos.AlturaEnabled(TipoCalc); }
        }

        public bool LarguraEnabled
        {
            get { return Glass.Calculos.LarguraEnabled(TipoCalc); }
        }

        public bool BenefVisible
        {
            get
            {
                return (GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) || Geral.UsarBeneficiamentosTodosOsGrupos) && !Geral.NaoVendeVidro();
            }
        }

        public string ImagemProjModPath { get; set; }

        public string NumItem { get; set; }

        public string DadosProdutos { get; set; }

        /// <summary>
        /// Obtém um valor que indica se o produto é um ambiente de orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <returns>True: o produto é um ambiente; False: o produto não é um ambiente;</returns>
        public bool TemItensProdutoSession(GDASession session)
        {
            if (NumChild == null)
            {
                NumChild = ProdutosOrcamentoDAO.Instance.VerificarPossuiProduto(session, (int)IdProd)
                    ? 1
                    : 0;
            }

            return NumChild > 0;
        }

        private decimal? _custoUnit = null;

        public decimal CustoUnit
        {
            get
            {
                if (_custoUnit == null && IdProduto != null)
                {
                    _custoUnit = ProdutoDAO.Instance.ObtemCustoCompra(null, (int)IdProduto);
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

        public decimal ValorDescontoAtualAmbiente
        {
            get { return ObterValorDescontoAmbiente(this); }
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

        public bool IsVidroEstoqueQtde
        {
            get { return TipoCalculoUsado == (int)TipoCalculoGrupoProd.Qtd && GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) && SubgrupoProdDAO.Instance.IsProdutoEstoque((int)IdSubgrupoProd); }
        }

        public string DescricaoProdutoComBenef
        {
            get
            {
                var retorno = DescrProduto;

                if (Redondo && !BenefConfigDAO.Instance.CobrarRedondo() && !retorno.ToLower().Contains("redondo"))
                {
                    retorno += " REDONDO";
                }

                if (Beneficiamentos != null && Beneficiamentos.Count > 0)
                {
                    retorno += string.Format("\n{0}", Beneficiamentos.DescricaoBeneficiamentos);
                }

                return retorno;
            }
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

        #region IProdutoCalculo

        IContainerCalculo IProdutoCalculo.Container { get; set; }
        IAmbienteCalculo IProdutoCalculo.Ambiente { get; set; }
        IDadosProduto IProdutoCalculo.DadosProduto { get; set; }

        uint IProdutoCalculo.Id
        {
            get { return IdProd; }
        }

        uint? IProdutoCalculo.IdAmbiente
        {
            get { return IdProdParent; }
        }

        decimal IProdutoCalculo.ValorUnit
        {
            get { return ValorProd != null ? ValorProd.Value : 0; }
            set { ValorProd = value; }
        }

        decimal IProdutoCalculo.Total
        {
            get { return _total != null ? _total.Value : 0; }
            set { _total = value; }
        }

        float IProdutoCalculo.Qtde
        {
            get { return Qtde != null ? Qtde.Value : 0; }
            set { this.Qtde = value; }
        }

        float IProdutoCalculo.TotM2Calc
        {
            get { return TotMCalc; }
            set { this.TotMCalc = value; }
        }

        uint IProdutoCalculo.IdProduto
        {
            get { return IdProduto != null ? IdProduto.Value : 0; }
        }

        public int QtdeAmbiente
        {
            get { return 1; }
        }

        int? IProdutoCalculo.AlturaBenef
        {
            get { return 0; }
        }

        int? IProdutoCalculo.LarguraBenef
        {
            get { return 0; }
        }

        decimal IProdutoCalculo.ValorTabelaPedido
        {
            get { return ValorTabela; }
        }

        decimal IProdutoCalculo.CustoProd
        {
            get { return Custo; }
            set { Custo = value; }
        }

        int IProdutoCalculo.TipoCalc
        {
            get
            {
                return TipoCalculoUsado;
            }
        }

        #endregion

        #region IAmbienteCalculo

        uint IAmbienteCalculo.Id
        {
            get { return IdProd; }
        }

        int IAmbienteCalculo.TipoDesconto
        {
            get { return TipoDesconto; }
        }

        decimal IAmbienteCalculo.Desconto
        {
            get { return Desconto; }
        }

        int IAmbienteCalculo.TipoAcrescimo
        {
            get { return TipoAcrescimo; }
        }

        decimal IAmbienteCalculo.Acrescimo
        {
            get { return Acrescimo; }
        }

        #endregion
    }
}
