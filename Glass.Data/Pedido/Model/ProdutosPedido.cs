using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Drawing;
using System.Xml.Serialization;
using Glass.Configuracoes;
using Glass.Global;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutosPedidoDAO))]
    [PersistenceClass("produtos_pedido")]
	public class ProdutosPedido : Colosoft.Data.BaseModel, IResumoCorte, IProdutoCalculo
    {
        public ProdutosPedido()
        {
            UsarBenefPcp = false;
            BuscarBenefImportacao = true;
        }

        #region Propriedades

        [PersistenceProperty("IdProdPed", PersistenceParameterType.IdentityKey)]
        public uint IdProdPed { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("IdProdPedParent")]
        public uint? IdProdPedParent { get; set; }

        [PersistenceProperty("IDITEMPROJETO")]
        public uint? IdItemProjeto { get; set; }

        [PersistenceProperty("IDMATERITEMPROJ")]
        public uint? IdMaterItemProj { get; set; }

        [PersistenceProperty("IDAMBIENTEPEDIDO")]
        public uint? IdAmbientePedido { get; set; }

        [PersistenceProperty("IDPRODPEDESP")]
        public uint? IdProdPedEsp { get; set; }

        [PersistenceProperty("IDPRODPEDANTERIOR")]
        public uint? IdProdPedAnterior { get; set; }

        [PersistenceProperty("IDPRODPEDPRODREPOS")]
        public uint? IdProdPedProdRepos { get; set; }

        [PersistenceProperty("IDAPLICACAO")]
        public uint? IdAplicacao { get; set; }

        [PersistenceProperty("IDPROCESSO")]
        public uint? IdProcesso { get; set; }

        [PersistenceProperty("IdNaturezaOperacao")]
        public uint? IdNaturezaOperacao { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("VALORVENDIDO")]
        public decimal ValorVendido { get; set; }

        [PersistenceProperty("ALTURA")]
        public Single Altura { get; set; }

        [PersistenceProperty("ALTURAREAL")]
        public float AlturaReal { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [PersistenceProperty("TOTM")]
        public Single TotM { get; set; }

        [PersistenceProperty("ESPESSURA")]
        public float Espessura { get; set; }

        /// <summary>
        /// Identifica se o vidro é redondo
        /// </summary>
        [PersistenceProperty("REDONDO")]
        public bool Redondo { get; set; }

        [PersistenceProperty("QTDSAIDA")]
        public float QtdSaida { get; set; }

        [PersistenceProperty("VALORCOMISSAO")]
        public decimal ValorComissao { get; set; }

        [PersistenceProperty("VALORBENEF")]
        public decimal ValorBenef { get; set; }

        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [PersistenceProperty("CUSTOPROD")]
        public decimal CustoProd { get; set; }

        [PersistenceProperty("ALIQICMS")]
        public Single AliqIcms { get; set; }

        [PersistenceProperty("VALORICMS")]
        public decimal ValorIcms { get; set; }

        [PersistenceProperty("ALIQUOTAIPI")]
        public float AliqIpi { get; set; }

        [PersistenceProperty("VALORIPI")]
        public decimal ValorIpi { get; set; }

        private float _totM2Calc;

        [PersistenceProperty("TOTM2CALC")]
        public float TotM2Calc
        {
            get { return _totM2Calc != 0 ? _totM2Calc : TotM; }
            set { _totM2Calc = value; }
        }

        [PersistenceProperty("ALTURABENEF")]
        public int? AlturaBenef { get; set; }

        [PersistenceProperty("LARGURABENEF")]
        public int? LarguraBenef { get; set; }

        [PersistenceProperty("ESPBENEF")]
        public float? EspessuraBenef { get; set; }

        [PersistenceProperty("VALORACRESCIMO")]
        public decimal ValorAcrescimo { get; set; }

        [PersistenceProperty("VALORDESCONTO")]
        public decimal ValorDesconto { get; set; }

        [PersistenceProperty("VALORACRESCIMOPROD")]
        public decimal ValorAcrescimoProd { get; set; }

        [PersistenceProperty("VALORDESCONTOPROD")]
        public decimal ValorDescontoProd { get; set; }

        [PersistenceProperty("INVISIVELPEDIDO")]
        public bool InvisivelPedido { get; set; }

        [PersistenceProperty("INVISIVELFLUXO")]
        public bool InvisivelFluxo { get; set; }

        [PersistenceProperty("INVISIVELADMIN", DirectionParameter.Input)]
        public bool InvisivelAdmin { get; set; }

        [PersistenceProperty("PEDCLI")]
        public string PedCli { get; set; }

        [PersistenceProperty("NUMETIQUETAREPOS")]
        public string NumEtiquetaRepos { get; set; }

        [PersistenceProperty("VALORTABELAORCAMENTO")]
        public decimal ValorTabelaOrcamento { get; set; }

        [PersistenceProperty("VALORTABELAPEDIDO")]
        public decimal ValorTabelaPedido { get; set; }

        [PersistenceProperty("TIPOCALCULOUSADOORCAMENTO", DirectionParameter.OnlyInsert)]
        public int TipoCalculoUsadoOrcamento { get; set; }

        [PersistenceProperty("TIPOCALCULOUSADOPEDIDO")]
        public int TipoCalculoUsadoPedido { get; set; }

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

        [PersistenceProperty("QTDEINVISIVEL", DirectionParameter.Input)]
        public float QtdeInvisivel { get; set; }

        [PersistenceProperty("Peso", DirectionParameter.Input)]
        public float Peso { get; set; }

        [PersistenceProperty("QtdeBoxImpresso")]
        public int QtdeBoxImpresso { get; set; }
 
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("IdProdBaixaEst")]
        public int? IdProdBaixaEst { get; set; }

        /// <summary>
        /// Base de calculo COFINS
        /// </summary>
        [PersistenceProperty("BcCofins")]
        public decimal BcCofins { get; set; }

        /// <summary>
        /// Alíquota CONFINS
        /// </summary>
        [PersistenceProperty("AliqCofins")]
        public float AliqCofins { get; set; }

        /// <summary>
        /// Valor COFINS
        /// </summary>
        [PersistenceProperty("ValorCofins")]
        public decimal ValorCofins { get; set; }

        /// <summary>
        /// Base de calculo ICMS
        /// </summary>
        [PersistenceProperty("BcIcms")]
        public decimal BcIcms { get; set; }

        /// <summary>
        /// Valor do ICMS desonerado
        /// </summary>
        [PersistenceProperty("ValorIcmsDesonerado")]
        public decimal ValorIcmsDesonerado { get; set; }

        /// <summary>
        /// Percentual de crédito base calculo ICMS
        /// </summary>
        [PersistenceProperty("PercRedBcIcms")]
        public float PercRedBcIcms { get; set; }

        /// <summary>
        /// Base de calculo ICMSST
        /// </summary>
        [PersistenceProperty("BcIcmsSt")]
        public decimal BcIcmsSt { get; set; }

        /// <summary>
        /// Alíquota ICMSST
        /// </summary>
        [PersistenceProperty("AliqIcmsSt")]
        public float AliqIcmsSt { get; set; }

        /// <summary>
        /// Valor do ICMSST
        /// </summary>
        [PersistenceProperty("ValorIcmsSt")]
        public decimal ValorIcmsSt { get; set; }

        /// <summary>
        /// Percentual de crédito base calculo ICMSST
        /// </summary>
        [PersistenceProperty("PercRedBcIcmsSt")]
        public decimal PercRedBcIcmsSt { get; set; }

        /// <summary>
        /// Base de calculo PIS
        /// </summary>
        [PersistenceProperty("BcPis")]
        public decimal BcPis { get; set; }

        /// <summary>
        /// Aliquota PIS
        /// </summary>
        [PersistenceProperty("AliqPis")]
        public float AliqPis { get; set; }

        /// <summary>
        /// Valor PIS
        /// </summary>
        [PersistenceProperty("ValorPis")]
        public decimal ValorPis { get; set; }

        /// <summary>
        /// CST
        /// </summary>
        [PersistenceProperty("Cst")]
        public int Cst { get; set; }

        /// <summary>
        /// CSOSN
        /// </summary>
        [PersistenceProperty("Csosn")]
        public int Csosn { get; set; }

        /// <summary>
        /// CST COFINS
        /// </summary>
        [PersistenceProperty("CstCofins")]
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstCofins { get; set; }

        /// <summary>
        /// CST PIS
        /// </summary>
        [PersistenceProperty("CstPis")]
        public Sync.Fiscal.Enumeracao.Cst.CstPisCofins? CstPis { get; set; }

        /// <summary>
        /// MVA
        /// </summary>
        [PersistenceProperty("Mva")]
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
        [PersistenceProperty("BcFcpSt")]
        public decimal BcFcpSt { get; set; }

        /// <summary>
        /// Aliquota do FCP ST
        /// </summary>
        [PersistenceProperty("AliqFcpSt")]
        public float AliqFcpSt { get; set; }

        /// <summary>
        /// Valor do FCP ST
        /// </summary>
        [PersistenceProperty("ValorFcpSt")]
        public decimal ValorFcpSt { get; set; }

        /// <summary>
        /// Base de calc. do FCP
        /// </summary>
        [PersistenceProperty("BcFcp")]
        public decimal BcFcp { get; set; }

        /// <summary>
        /// Aliquota do FCP
        /// </summary>
        [PersistenceProperty("AliqFcp")]
        public float AliqFcp { get; set; }

        /// <summary>
        /// Valor do FCP
        /// </summary>
        [PersistenceProperty("ValorFcp")]
        public decimal ValorFcp { get; set; }

        [PersistenceProperty("PERCCOMISSAO")]
        public decimal PercComissao { get; set; }

        /// <summary>
        /// Percentual da rentabilidade.
        /// </summary>
        [PersistenceProperty("PERCENTUALRENTABILIDADE", Direction = DirectionParameter.Input)]
        public decimal PercentualRentabilidade { get; set; }

        /// <summary>
        /// Valor da rentabilidade financeira.
        /// </summary>
        [PersistenceProperty("RENTABILIDADEFINANCEIRA", Direction = DirectionParameter.Input)]
        public decimal RentabilidadeFinanceira { get; set; }

        #region Dados para exportação

        [PersistenceProperty("OBSPROJETOEXTERNO")]
        public string ObsProjetoExterno { get; set; }

        #endregion

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("LarguraReal", DirectionParameter.InputOptional)]
        public int LarguraReal { get; set; }

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IdGrupoProd", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DESCRGRUPOPROD", DirectionParameter.InputOptional)]
        public string DescrGrupoProd { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IdSubgrupoProd", DirectionParameter.InputOptional)]
        public uint IdSubgrupoProd { get; set; }

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [XmlIgnore]
        [PersistenceProperty("QtdeInstalada", DirectionParameter.InputOptional)]
        public decimal QtdeInstalada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("QtdeSomada", DirectionParameter.InputOptional)]
        public decimal QtdeSomada { get; set; }

        [PersistenceProperty("CodProcesso", DirectionParameter.InputOptional)]
        public string CodProcesso { get; set; }

        [PersistenceProperty("CodAplicacao", DirectionParameter.InputOptional)]
        public string CodAplicacao { get; set; }

        private string _m2Minimo;

        [XmlIgnore]
        [PersistenceProperty("AreaMinima", DirectionParameter.InputOptional)]
        public string M2Minimo
        {
            get { return !String.IsNullOrEmpty(_m2Minimo) ? _m2Minimo.ToString().Replace(',', '.') : "0"; }
            set { _m2Minimo = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("AliqICMSProd", DirectionParameter.InputOptional)]
        public double AliqICMS { get; set; }

        private string _ambiente;

        [XmlIgnore]
        [PersistenceProperty("AMBIENTE", DirectionParameter.InputOptional)]
        public string Ambiente
        {
            get { return _ambiente == null ? String.Empty : _ambiente; }
            set { _ambiente = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("DESCRAMBIENTE", DirectionParameter.InputOptional)]
        public string DescrAmbiente { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TIPODESCONTOAMBIENTE", DirectionParameter.InputOptional)]
        public int TipoDescontoAmbiente { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DESCONTOAMBIENTE", DirectionParameter.InputOptional)]
        public decimal DescontoAmbiente { get; set; }

        private string _obsProjeto;

        /// <summary>
        /// Campo usado para exportar dados projeto, não colocar a tag [XmlIgnore] de forma alguma
        /// </summary>
        [PersistenceProperty("OBSPROJETO", DirectionParameter.InputOptional)]
        public string ObsProjeto
        {
            get { return !String.IsNullOrEmpty(_obsProjeto) ? _obsProjeto : String.Empty; }
            set { _obsProjeto = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("IDMATERPROJMOD", DirectionParameter.InputOptional)]
        public uint? IdMaterProjMod { get; set; }

        [XmlIgnore]
        [PersistenceProperty("VALORPRODUTOTABELA", DirectionParameter.InputOptional)]
        public decimal ValorProdutoTabela { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TIPOSETORPRODUCAO", DirectionParameter.InputOptional)]
        public long? TipoSetorProducao { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDPRODPEDPRODUCAOCONSULTA", DirectionParameter.InputOptional)]
        public uint? IdProdPedProducaoConsulta { get; set; }

        [XmlIgnore]
        [PersistenceProperty("NUMETIQUETACONSULTA", DirectionParameter.InputOptional)]
        public string NumEtiquetaConsulta { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TEMLIBERACAOETIQUETA", DirectionParameter.InputOptional)]
        public bool TemLiberacaoEtiqueta { get; set; }

        [XmlIgnore]
        [PersistenceProperty("QTDETROCA", DirectionParameter.InputOptional)]
        public decimal QtdeTroca { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDPECAITEMPROJ", DirectionParameter.InputOptional)]
        public uint? IdPecaItemProj { get; set; }

        [XmlIgnore]
        [PersistenceProperty("UNIDADE", DirectionParameter.InputOptional)]
        public string Unidade { get; set; }

        [XmlIgnore]
        [PersistenceProperty("QTDEETIQUETAS", DirectionParameter.InputOptional)]
        public int? QtdeEtiquetas { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDCLIENTE", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        [XmlIgnore]
        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CodPedCliente", DirectionParameter.InputOptional)]
        public string CodPedCliente { get; set; }

        [XmlIgnore]
        [PersistenceProperty("SITUACAOPEDIDO", DirectionParameter.InputOptional)]
        public int SituacaoPedido { get; set; }

        [XmlIgnore]
        [PersistenceProperty("DATAENTREGAPEDIDO", DirectionParameter.InputOptional)]
        public DateTime? DataEntregaPedido { get; set; }

        /// <summary>
        /// Propriedade usada para salvar a qtd do produto para verificar com a qtd liberada quando for gerar a nota.
        /// </summary>
        [XmlIgnore]
        [PersistenceProperty("QTDEORIGINAL", DirectionParameter.InputOptional)]
        public double QtdeOriginal { get; set; }

        [XmlIgnore]
        [PersistenceProperty("QtdeProduzindo", DirectionParameter.InputOptional)]
        public double QtdeProduzindo { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TotMProduzindo", DirectionParameter.InputOptional)]
        public double TotMProduzindo { get; set; }

        [XmlIgnore]
        [PersistenceProperty("LocalArmazenagem", DirectionParameter.InputOptional)]
        public string LocalArmazenagem { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IsClienteRota", DirectionParameter.InputOptional)]
        public bool IsClienteRota { get; set; }

        [PersistenceProperty("IdAmbientePedidoEspelho", DirectionParameter.InputOptional)]
        public uint? IdAmbientePedidoEspelho { get; set; }

        [PersistenceProperty("Ncm", DirectionParameter.InputOptional)]
        public string Ncm { get; set; }

        [PersistenceProperty("CustoCompraProduto", DirectionParameter.InputOptional)]
        public decimal CustoCompraProduto { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IdsItemProjeto", DirectionParameter.InputOptional)]
        public string IdsItemProjeto { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IdProdUsar", DirectionParameter.InputOptional)]
        public ulong? IdProdUsar { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IsChapaImportada", DirectionParameter.InputOptional)]
        public bool IsChapaImportada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("QtdeVolume", DirectionParameter.InputOptional)]
        public float QtdeVolume { get; set; }

        [XmlIgnore]
        [PersistenceProperty("VALORDESCONTOQTDENF", DirectionParameter.InputOptional)]
        public decimal ValorDescontoQtdeNf { get; set; }

        [XmlIgnore]
        [PersistenceProperty("Exportado", DirectionParameter.InputOptional)]
        public bool Exportado { get; set; }

        [XmlIgnore]
        [PersistenceProperty("ChapaVidro", DirectionParameter.InputOptional)]
        public bool ChapaVidro { get; set; }

        [XmlIgnore]
        [PersistenceProperty("IDLOJA", DirectionParameter.InputOptional)]
        public int IdLoja { get; set; }

        [XmlIgnore]
        [PersistenceProperty("PecaOtimizada", DirectionParameter.InputOptional)]
        public bool PecaOtimizada { get; set; }

        [XmlIgnore]
        [PersistenceProperty("GrauCorte", DirectionParameter.InputOptional)]
        public GrauCorteEnum? GrauCorte { get; set; }

        [XmlIgnore]
        [PersistenceProperty("ProjetoEsquadria", DirectionParameter.InputOptional)]
        public bool ProjetoEsquadria { get; set; }

        [XmlIgnore]
        [PersistenceProperty("AMBIENTEPEDIDO", DirectionParameter.InputOptional)]
        public string AmbientePedido { get; set; }
        
        [XmlIgnore]
        [PersistenceProperty("QTDEPECASVIDRO", DirectionParameter.InputOptional)]
        public double QtdePecasVidro { get; set; }

        [XmlIgnore]
        [PersistenceProperty("ORDEMCARGAPARCIAL", DirectionParameter.InputOptional)]
        public bool OrdemCargaParcial { get; set; }
 
        [XmlIgnore]
        [PersistenceProperty("IDORDEMCARGA", DirectionParameter.InputOptional)]
        public int IdOrdemCarga { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CODNATUREZAOPERACAO", DirectionParameter.InputOptional)]
        public string CodNaturezaOperacao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool IsProdutoLaminadoComposicao
        {
            get
            {
                var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)IdProd);

                return tipoSubgrupo == TipoSubgrupoProd.VidroDuplo || tipoSubgrupo == TipoSubgrupoProd.VidroLaminado;
            }
        }

        public bool IsProdFilhoLamComposicao
        {
            get { return IdProdPedParent.GetValueOrDefault(0) > 0; }
        }

        private string _descrPerdaRepos = null;

        [XmlIgnore]
        public string DescrPerdaRepos
        {
            get
            {
                if (IdProdPedProducaoConsulta == null || IdProdPedProducaoConsulta == 0)
                {
                    if (!String.IsNullOrEmpty(NumEtiquetaConsulta))
                    {
                        IdProdPedProducaoConsulta = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<uint>("idProdPedProducao",
                            "numEtiqueta=?num", new GDAParameter("?num", NumEtiquetaConsulta));
                    }
                    else if (!String.IsNullOrEmpty(NumEtiquetaRepos))
                    {
                        IdProdPedProducaoConsulta = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<uint>("idProdPedProducao",
                            "numEtiqueta=?num", new GDAParameter("?num", NumEtiquetaRepos));
                    }
                }

                if (_descrPerdaRepos == null && IdProdPedProducaoConsulta > 0)
                {
                    string where = "idProdPedProducao=" + IdProdPedProducaoConsulta;

                    ProdutoPedidoProducao temp = new ProdutoPedidoProducao();
                    temp.Situacao = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<long>("situacao", where);
                    temp.TipoPerda = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<uint?>("tipoPerda", where);
                    temp.Obs = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<string>("obs", where);

                    _descrPerdaRepos = temp.DescrTipoPerda;
                }

                return _descrPerdaRepos;
            }
        }

        [XmlIgnore]
        public float Produzindo
        {
            get
            {
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int?)IdSubgrupoProd);
                return (float)(tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                    tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto ? TotMProduzindo : QtdeProduzindo);
            }
        }

        [XmlIgnore]
        public string DescricaoProduzindo
        {
            get
            {
                int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int?)IdSubgrupoProd);
                string descrTipoCalculo = Glass.Global.CalculosFluxo.GetDescrTipoCalculo(tipoCalc, true);
                return Produzindo + descrTipoCalculo;
            }
        }

        /// <summary>
        /// Campo utilizado ao marcar saída nos produtos do pedido
        /// </summary>
        [XmlIgnore]
        public float QtdMarcadaSaida { get; set; }

        [XmlIgnore]
        public string TotalM2CalcSemChapaString
        {
            get
            {
                var isPedidoProducaoCorte = PedidoDAO.Instance.IsPedidoProducaoCorte(null, IdPedido);
                return Glass.Global.CalculosFluxo.CalcM2Calculo(IdCliente, (int)Altura, Largura, Qtde, (int)IdProd, Redondo,
                    Beneficiamentos.CountAreaMinima, ProdutoDAO.Instance.ObtemAreaMinima((int)IdProd), false, 0, 
                    TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 && !isPedidoProducaoCorte).ToString();
            }
        }

        [XmlIgnore]
        public string AlturaRpt
        {
            get { return TipoCalc == 4 ? Altura + "ml" : Altura.ToString(); }
        }

        [XmlIgnore]
        public string IsVidro
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd).ToString().ToLower(); }
        }

        [XmlIgnore]
        public bool IsVidroEstoque
        {
            get
            {
                return GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) && SubgrupoProdDAO.Instance.IsProdutoEstoque((int)IdSubgrupoProd);
            }
        }

        [XmlIgnore]
        public int? IdPedidoRevenda
        {
            get { return PedidoDAO.Instance.ObterIdPedidoRevenda(null, (int)IdPedido); }
        }

        [XmlIgnore]
        public string IsAluminio
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio((int)IdGrupoProd).ToString().ToLower(); }
        }

        [XmlIgnore]
        public string IsMaoDeObra
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsMaoDeObra((int)IdGrupoProd).ToString().ToLower(); }
        }

        [XmlIgnore]
        public int TipoCalc
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int)IdSubgrupoProd); }
        }

        [XmlIgnore]
        public bool AlturaEnabled
        {
            get { return Glass.Calculos.AlturaEnabled(TipoCalc); }
        }

        [XmlIgnore]
        public bool LarguraEnabled
        {
            get { return Glass.Calculos.LarguraEnabled(TipoCalc); }
        }

        [XmlIgnore]
        public bool BenefVisible
        {
            get
            {
                return (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) || Geral.UsarBeneficiamentosTodosOsGrupos) && !Geral.NaoVendeVidro();
            }
        }

        [XmlIgnore]
        public bool DeleteVisible
        {
            get
            {
                if (IdProdPedParent.GetValueOrDefault(0) > 0 && ProdutosPedidoDAO.Instance.IsProdLaminado(IdProdPedParent.Value))
                    return false;

                if (IdMaterItemProj == null)
                    return true;

                return MaterialItemProjetoDAO.Instance.GetElement(IdMaterItemProj.Value).DeleteVisible;
            }
        }

        [XmlIgnore]
        public bool PodeEditarComposicao
        {
            get
            {
                // Se o produto for composição e tipo subgrupo Vidro Laminado ou a forma de pagamento for Obra.
                if ((IdProdPedParent.GetValueOrDefault(0) > 0 && ProdutosPedidoDAO.Instance.IsProdLaminado(IdProdPedParent.Value)) ||
                        PedidoDAO.Instance.GetIdObra(null, IdPedido) > 0)
                    return false;
                
                return true;
            }
        }

        private bool? _maoDeObra = null;

        [XmlIgnore]
        public bool PedidoMaoDeObra
        {
            get
            {
                if (_maoDeObra == null)
                    _maoDeObra = PedidoDAO.Instance.IsMaoDeObra(null, IdPedido);

                return _maoDeObra.GetValueOrDefault();
            }
        }

        private float _qtdeLiberados = -1;

        [XmlIgnore]
        public float QtdeDisponivelLiberacao
        {
            get
            {
                #region Variaveis Locais

                int _qtdeAmbiente = QtdeAmbiente;

                #endregion

                var idLoja = PedidoDAO.Instance.ObtemIdLoja(null, IdPedido);
                var naoIgnorar = !LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(null, idLoja);
                bool usarQtdeEtiquetas = (Liberacao.DadosLiberacao.LiberarProdutosProntos && naoIgnorar) &&
                    (TemLiberacaoEtiqueta || !String.IsNullOrEmpty(NumEtiquetaConsulta)) && QtdeEtiquetas > 0;

                float qtde = Qtde * (!usarQtdeEtiquetas ? (float)_qtdeAmbiente : 1);

                if (_qtdeLiberados == -1)
                {
                    _qtdeLiberados = ProdutosLiberarPedidoDAO.Instance.GetQtdeByProdPed(null, IdProdPed, IdProdPedProducaoConsulta);
                    if (((_qtdeLiberados > 1 && _qtdeLiberados < qtde) || (!TemLiberacaoEtiqueta && _qtdeLiberados != (Qtde * _qtdeAmbiente))) && 
                        !String.IsNullOrEmpty(NumEtiquetaConsulta))
                    {
                        string item = NumEtiquetaConsulta.Substring(NumEtiquetaConsulta.IndexOf('.') + 1);
                        item = item.Substring(0, item.IndexOf('/'));

                        _qtdeLiberados = Glass.Conversoes.StrParaInt(item) > _qtdeLiberados ? 
                            usarQtdeEtiquetas ? 0 : _qtdeLiberados :
                            usarQtdeEtiquetas ? (int)QtdeEtiquetas : (int)qtde;
                    }
                }
                
                return (usarQtdeEtiquetas ? (float)QtdeEtiquetas.Value : qtde) - _qtdeLiberados;
            }
        }

        [XmlIgnore]
        public string DescricaoProdutoComBenef
        {
            get
            {
                string retorno = DescrProduto;

                if (AlturaBenef > 0 || LarguraBenef > 0)
                {
                    int altura = AlturaBenef != null ? AlturaBenef.Value : 0;
                    int largura = LarguraBenef != null ? LarguraBenef.Value : 0;
                    retorno += " " + altura + "x" + largura;
                    if (EspessuraBenef > 0)
                        retorno += " Esp. " + EspessuraBenef.Value;
                }

                if (Redondo && !BenefConfigDAO.Instance.CobrarRedondo() && !retorno.ToLower().Contains("redondo"))
                    retorno += " REDONDO";

                if (Beneficiamentos != null && Beneficiamentos.Count > 0)
                    retorno += string.Format("\n{0}", Beneficiamentos.DescricaoBeneficiamentos);

                return retorno;
            }
        }

        [XmlIgnore]
        public string AlturaLista
        {
            get
            {
                // Criado para que ao buscar os produtos na tela de liberar pedido a altura e a largura sejam mostradas conforme o ambiente do pedido caso o mesmo seja MO.
                if (IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra && IdAmbientePedido.GetValueOrDefault() > 0 && Altura == 0)
                    return AmbientePedidoDAO.Instance.ObtemAltura(IdAmbientePedido.GetValueOrDefault()).ToString();
                else if (IdGrupoProd != (uint)Glass.Data.Model.NomeGrupoProd.Alumínio)
                    return Altura.ToString();
                else
                    return Altura != AlturaReal ? (AlturaReal > 0 ? AlturaReal.ToString() + " (" + Altura.ToString() + ")" : Altura.ToString()) : Altura.ToString();
            }
        }

        [XmlIgnore]
        public string LarguraLista
        {
            get
            {
                // Criado para que ao buscar os produtos na tela de liberar pedido a altura e a largura sejam mostradas conforme o ambiente do pedido caso o mesmo seja MO.
                if (IdGrupoProd == (uint)Glass.Data.Model.NomeGrupoProd.MaoDeObra && IdAmbientePedido.GetValueOrDefault() > 0 && Largura == 0)
                    return AmbientePedidoDAO.Instance.ObtemLargura(IdAmbientePedido.GetValueOrDefault()).ToString();
                else
                    return Largura != LarguraReal ? (LarguraReal > 0 ? LarguraReal.ToString() + " (" + Largura.ToString() + ")" : Largura.ToString()) : Largura.ToString();
            }
        }

        [XmlIgnore]
        public string TituloAltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Altura" : "Largura"; }
        }

        [XmlIgnore]
        public string TituloAltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? "Largura" : "Altura"; }
        }

        [XmlIgnore]
        public string AltLarg1
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? AlturaRpt : Largura.ToString(); }
        }

        [XmlIgnore]
        public string AltLarg2
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? Largura.ToString() : AlturaRpt; }
        }

        private bool _exibirAmbiente = true;

        [XmlIgnore]
        public bool ExibirAmbiente
        {
            get { return _exibirAmbiente; }
            set { _exibirAmbiente = value; }
        }

        [XmlIgnore]
        public uint NumItem { get; set; }

        private decimal _custoUnit = 0;

        [XmlIgnore]
        public decimal CustoUnit
        {
            get
            {
                if (_custoUnit == 0)
                    _custoUnit = ProdutoDAO.Instance.ObtemCustoCompra((int)IdProd);

                return _custoUnit;
            }
        }

        [XmlIgnore]
        public decimal TotalCalc
        {
            get 
            {
                #region Variaveis Locais

                bool _pedidoMaoDeObra = PedidoMaoDeObra;
                int _qtdeAmbiente = QtdeAmbiente;

                #endregion

                // Calcula a quantidade de itens para achar o valor unitário do produto
                // Se for mão de obra, considera o número de ambientes (liberação de mão de obra e não de ambiente)
                int qtdeAmbiente = _pedidoMaoDeObra && _qtdeAmbiente > 0 ? _qtdeAmbiente : 1;

                // Se for mão de obra e a quantidade de ambientes for > 0 e se a empresa libera produtos prontos, 
                // a quantidade a usar deve ser a quantidade de ambientes vezes a quantidade de beneficiamentos do mesmo, caso contrário
                // o valor de liberação do beneficiamento ficaria maior do que deveria.
                decimal qtdeUsar = _pedidoMaoDeObra && _qtdeAmbiente > 0 ? qtdeAmbiente : (decimal)Qtde;

                float qtdeDisponivelLiberacao = QtdeDisponivelLiberacao;

                // André 20/02/13: No caso de empresas que não liberam pedidos somente quando estão prontos 
                // (Atualizanddo 12/09/13: Empresas que liberam somente prontos também), os produtos de pedidos mão de obra com 
                // beneficiamentos com quantidade maior que 1 estavam sendo liberados incorretamente, a "QtdeDisponivelLiberacao" está retornando a 
                // quantidade do beneficiamento e utilizando este valor para calcular o total (multiplicando-o) incorretamente, para corrigir esta 
                // situação foi necessário recalcular a "QtdeDisponivelLiberacao" considerando a quantidade de ambiente menos a _qtdeLiberados.
                if (/*!Liberacao.DadosLiberacao.LiberarProdutosProntos &&*/ _pedidoMaoDeObra && _qtdeAmbiente > 0 && Qtde > 1 && qtdeDisponivelLiberacao > 1)
                    qtdeDisponivelLiberacao = qtdeAmbiente - (_qtdeLiberados > 0 ? (int)_qtdeLiberados : 0);

                uint idCliente = IdCliente > 0 ? IdCliente : PedidoDAO.Instance.ObtemIdCliente(null, IdPedido);

                // Calcula o total do produto, considerando beneficiamentos, ICMS e IPI. Soma também o desconto por quantidade, 
                // para que ao aplicar o desconto do pedido na tela de liberação aplique somente uma vez o desconto
                decimal total = Total + ValorBenef + (!PedidoConfig.RatearDescontoProdutos ? ValorDescontoQtde : 0);

                // Calcula a taxa de fast delivery, caso exista
                if (PedidoConfig.Pedido_FastDelivery.FastDelivery && PedidoDAO.Instance.IsFastDelivery(null, IdPedido))
                    total = Math.Round(total * (decimal)(1 + (PedidoDAO.Instance.ObtemTaxaFastDelivery(null, IdPedido) / 100)), 4);

                var idLoja = PedidoDAO.Instance.ObtemIdLoja(null, IdPedido);

                var lojaCalculaIcmsStLiberacao = LojaDAO.Instance.ObtemCalculaIcmsStLiberacao(null, idLoja);
                var clienteCalculaIcmsSt = ClienteDAO.Instance.IsCobrarIcmsSt(idCliente);
                var pedidoCalculouIcmsSt = PedidoDAO.Instance.CobrouICMSST(null, IdPedido);
                var lojaCalculaIpiLiberacao = LojaDAO.Instance.ObtemCalculaIpiLiberacao(null, idLoja);
                var clienteCalculaIpi = ClienteDAO.Instance.IsCobrarIpi(null, idCliente);
                var pedidoCalculouIpi = PedidoDAO.Instance.CobrouIPI(null, IdPedido);

                total += lojaCalculaIcmsStLiberacao && clienteCalculaIcmsSt && pedidoCalculouIcmsSt ? ValorIcms : 0;
                total += lojaCalculaIpiLiberacao && clienteCalculaIpi && pedidoCalculouIpi ? ValorIpi : 0;

                decimal retorno = total;

                // Calcula o total a liberar, considerando a quantidade de itens que podem ser liberados
                // Calcula o valor por peça que será liberada (retorno / qtdeUsar)
                // Depois multiplica pela quantidade que pode ser liberada da peça, considerando a quantidade de ambientes
                if (qtdeDisponivelLiberacao > 0 && qtdeUsar > 0)
                    retorno = retorno / qtdeUsar * (decimal)qtdeDisponivelLiberacao;

                if (String.IsNullOrEmpty(NumEtiquetaConsulta))
                    return Math.Round(retorno, 4);
                else
                {
                    decimal retornoBase = Math.Round(retorno, 4);

                    string item = NumEtiquetaConsulta.Split('.')[1];
                    item = item.Substring(0, item.IndexOf('/'));

                    return decimal.Parse(item) != qtdeUsar ? retornoBase :
                        Math.Round(total - (retornoBase * (qtdeUsar - 1)), 4);
                }
            }
        }

        [XmlIgnore]
        public float TotM2Liberacao
        {
            get
            {
                #region Variaveis Locais

                float _qtdeDisponivelLiberacao = QtdeDisponivelLiberacao;

                #endregion

                if (_qtdeDisponivelLiberacao > 0 && Qtde > 0)
                    return (float)Math.Round(TotM / Qtde * _qtdeDisponivelLiberacao, 2);
                else
                    return TotM;
            }
        }

        [XmlIgnore]
        public Color CorLinha
        {
            get 
            {
                if (IdProdPedEsp == null)
                    return Color.Black;

                return TipoSetorProducao == null ? Color.Black :
                    TipoSetorProducao == (int)SituacaoProdutoProducao.Pronto ? Color.Blue :
                    TipoSetorProducao == (int)SituacaoProdutoProducao.Entregue ? Color.Green : 
                    Color.Red;
            }
        }

        [XmlIgnore]
        public string TotM2Rpt
        {
            get
            {
                switch ((Glass.Data.Model.TipoCalculoGrupoProd)TipoCalc)
                {
                    case Glass.Data.Model.TipoCalculoGrupoProd.ML:
                    case Glass.Data.Model.TipoCalculoGrupoProd.MLAL0:
                    case Glass.Data.Model.TipoCalculoGrupoProd.MLAL05:
                    case Glass.Data.Model.TipoCalculoGrupoProd.MLAL1:
                    case Glass.Data.Model.TipoCalculoGrupoProd.MLAL6:
                        return (Altura * Qtde * QtdeAmbiente).ToString("0.##") + "ml";

                    case Glass.Data.Model.TipoCalculoGrupoProd.Perimetro:
                        int alturaMult = AlturaBenef != null ? AlturaBenef.Value : 2;
                        int larguraMult = LarguraBenef != null ? LarguraBenef.Value : 2;
                        return (((Altura * alturaMult) + (Largura * larguraMult)) * QtdeAmbiente / 1000).ToString() + "ml";

                    case Glass.Data.Model.TipoCalculoGrupoProd.M2:
                    case Glass.Data.Model.TipoCalculoGrupoProd.M2Direto:
                        return TotM.ToString("0.##") + "m²" + (TotM != _totM2Calc ? " (" + _totM2Calc.ToString("0.##") + "m²)" : String.Empty);

                    default:
                        return String.Empty;
                }
            }
        }

        [XmlIgnore]
        public string ValorUnitRpt
        {
            get
            {
                string dividir = TotM2Rpt.Replace("ml", "").Replace("m²", "").Replace("ml", "");
                dividir = dividir.IndexOf("(") == -1 ? dividir : dividir.Substring(0, dividir.IndexOf("(") - 1);

                float d = !String.IsNullOrEmpty(dividir.Trim()) ? float.Parse(dividir.Trim()) : Qtde;
                if (d == float.NaN || d == 0)
                    d = 1;

                return (Total / (decimal)d).ToString("C");
            }
        }

        [XmlIgnore]
        public string QtdePecasVidroMaoDeObra
        {
            get
            {
                var idLoja = PedidoDAO.Instance.ObtemIdLoja(null, IdPedido);
                var ignorar = LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(null, idLoja);
                return PedidoMaoDeObra && (!Liberacao.DadosLiberacao.LiberarProdutosProntos || ignorar) ? " x " + QtdeAmbiente + " p.v." : String.Empty; }
        }

        [XmlIgnore]
        public string DescrSituacaoPedido
        {
            get { return PedidoDAO.Instance.GetSituacaoPedido(SituacaoPedido); }
        }

        [XmlIgnore]
        public decimal ValorVendidoBruto
        {
            get { return ValorUnitarioBruto; }
        }

        /// <summary>
        /// Indica se o produto pode ser editado no pedido
        /// (se o pedido não tem desconto, ou se a empresa não rateia o desconto).
        /// </summary>
        [XmlIgnore]
        public bool PodeEditar
        {
            get 
            {
                if (!PedidoConfig.RatearDescontoProdutos)
                    return true;

                float descontoPedido = PedidoDAO.Instance.ObtemValorCampo<float>("desconto", "idPedido=" + IdPedido);
                float descontoAmbiente = AmbientePedidoDAO.Instance.ObtemValorCampo<float>("desconto", "idAmbientePedido=" + IdAmbientePedido.GetValueOrDefault());
                return (descontoPedido + descontoAmbiente) == 0; 
            }
        }

        [XmlIgnore]
        public decimal TotalProdTelaDesconto
        {
            get { return Total; }// / (decimal)(Qtde + QtdeInvisivel) * (decimal)Qtde; }
        }

        [XmlIgnore]
        public decimal TotalProdRemTelaDesconto
        {
            get
            {
                return Qtde > 0 ? Total / (decimal)Qtde * (decimal)QtdeInvisivel :
                    CalculosFluxo.CalcTotaisItemProdFast(TipoCalc, Altura, Largura, QtdeInvisivel, TotMTelaDesconto, ValorVendido);
            }
        }

        [XmlIgnore]
        public decimal ValorBenefProdTelaDesconto
        {
            get { return Qtde > 0 ? ValorBenef / (decimal)(Qtde + QtdeInvisivel) * (decimal)Qtde : ValorBenef; }
        }

        [XmlIgnore]
        public decimal ValorBenefProdRemTelaDesconto
        {
            get { return Qtde > 0 ? ValorBenef / (decimal)(Qtde + QtdeInvisivel) * (decimal)QtdeInvisivel : ValorBenef; }
        }

        [XmlIgnore]
        public float TotMTelaDesconto
        {
            get
            {
                if (IdGrupoProd == 0)
                    IdGrupoProd = (uint)ProdutoDAO.Instance.ObtemIdGrupoProd((int)IdProd);

                return Qtde > 0 ? TotM / Qtde * QtdeInvisivel : !Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd) ? 0 :
                    Glass.Global.CalculosFluxo.ArredondaM2(Largura, (int)Altura, QtdeInvisivel, (int)IdProd, Redondo);
            }
        }

        [XmlIgnore]
        public string CodInternoDescProd
        {
            get { return CodInterno + " - " + DescrProduto; }
        }

        [XmlIgnore]
        public string SetoresPendentes
        {
            get { return SetorDAO.Instance.ObtemDescricaoSetoresRestantes(NumEtiquetaConsulta, IdProdPedEsp); }
        }

        [XmlIgnore]
        public bool IsProdLamComposicao
        {
            get
            {
                var subGrupos = new List<int>() { (int)TipoSubgrupoProd.VidroLaminado, (int)TipoSubgrupoProd.VidroDuplo };

                return PedidoDAO.Instance.GetTipoPedido(null, IdPedido) == Pedido.TipoPedidoEnum.Venda &&
                    subGrupos.Contains((int)SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)IdProd));
            }
        }

        [XmlIgnore]
        public bool IsProdLamComposicaoComFilho
        {
            get
            {
                var subGrupos = new List<int>() { (int)TipoSubgrupoProd.VidroLaminado, (int)TipoSubgrupoProd.VidroDuplo };

                return PedidoDAO.Instance.GetTipoPedido(null, IdPedido) == Pedido.TipoPedidoEnum.Venda &&
                    subGrupos.Contains((int)SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)IdProd)) &&
                    ProdutosPedidoDAO.Instance.TemFilhoComposicao((int)IdProdPed) && IdProdPedParent.GetValueOrDefault(0) > 0;
            }
        }

        [XmlIgnore]
        public bool ExibirFilhosDescontoPedido
        {
            get
            {
                var subGrupos = new List<int>() { (int)TipoSubgrupoProd.VidroDuplo };

                return PedidoDAO.Instance.GetTipoPedido(null, IdPedido) == Pedido.TipoPedidoEnum.Venda &&
                    subGrupos.Contains((int)SubgrupoProdDAO.Instance.ObtemTipoSubgrupo((int)IdProd)) &&
                    ProdutosPedidoDAO.Instance.TemFilhoComposicao((int)IdProdPed);
            }
        }

        [XmlIgnore]
        public float AlturaProducao
        {
            get { return Altura; }
        }

        [XmlIgnore]
        public int LarguraProducao
        {
            get { return Largura; }
        }

        [XmlIgnore]
        public string EtiquetasLegenda { get; set; }

        [XmlIgnore]
        public string ImagemUrl
        {
            get
            {
                var nomeImagem = Utils.GetPecaComercialVirtualPath + IdProdPed.ToString().PadLeft(10, '0') + "_0.jpg"; 
                if (Utils.ArquivoExiste(nomeImagem))
                    return nomeImagem;

                return null;
            }
        }

        [XmlIgnore]
        public float PesoResumoCorte
        {
            get { return Peso; }
        }

        #endregion

        #region Propriedades do Beneficiamento

        [XmlIgnore]
        public bool UsarBenefPcp { get; set; }

        /// <summary>
        /// Define se os beneficiamentos serão buscados do banco.
        /// Usado apenas durante a importação de pedido.
        /// </summary>
        [XmlIgnore]
        internal bool BuscarBenefImportacao { get; set; }

        private List<ProdutoPedidoBenef> _beneficiamentos = null;

        [XmlIgnore]
        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                try
                {
                    if (!ProdutoDAO.Instance.CalculaBeneficiamento((int)IdProd))
                        _beneficiamentos = new List<ProdutoPedidoBenef>();

                    // Salva a quantidade de beneficiamentos que estão associados ao produto de pedido.
                    int qtdBenef = _beneficiamentos == null ? 0 : _beneficiamentos.Count;

                    // Verifica se o produto de pedido buscou a referência do beneficiamento.
                    if ((qtdBenef == 0 || _beneficiamentos == null) && BuscarBenefImportacao)
                    {
                        // Recupera a referência do beneficiamento direto do banco de dados e incrementa a variável qtdBenef caso algum valor seja buscado.
                        if (!InvisivelPedido && !UsarBenefPcp)
                        {
                            _beneficiamentos = new List<ProdutoPedidoBenef>(ProdutoPedidoBenefDAO.Instance.GetByProdutoPedido(IdProdPed));
                            qtdBenef = _beneficiamentos.Count;
                        }
                        else if (IdProdPedEsp > 0)
                        {
                            _beneficiamentos = new List<ProdutoPedidoBenef>(new GenericBenefCollection(ProdutoPedidoEspelhoBenefDAO.Instance.GetByProdutoPedido(IdProdPedEsp.Value)).ToProdutosPedido(IdProdPed));
                            qtdBenef = _beneficiamentos.Count;
                        }
                    }

                    if (qtdBenef == 0 || _beneficiamentos == null)
                        _beneficiamentos = new List<ProdutoPedidoBenef>();
                }
                catch
                {
                    _beneficiamentos = new List<ProdutoPedidoBenef>();
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

        [XmlIgnore]
        public string DescrBeneficiamentos
        {
            get { return Beneficiamentos.DescricaoBeneficiamentos; }
        }

        /// <summary>
        /// Usado para exportação de pedido.
        /// </summary>
        public string ServicosInfoBenef
        {
            get 
            {
                GenericBenefCollection benef = Beneficiamentos.ToProdutosPedido(IdProdPed);                
                benef.GerarServicosInfo = true;

                return benef.ServicosInfo;
            }
            set
            {
                GenericBenefCollection benef = new GenericBenefCollection();
                benef.ServicosInfo = value;

                _beneficiamentos = benef;
            }
        }

        [XmlIgnore]
        public bool AlterarProcessoAplicacaoVisible
        {
            get
            {                
                return IsVidro == "true";
            }
        }

        #endregion

        #region Propriedades da Nota Fiscal

        [XmlIgnore]
        [PersistenceProperty("QTDNF", DirectionParameter.InputOptional)]
        public double QtdNf { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TOTM2NF", DirectionParameter.InputOptional)]
        public double TotM2Nf { get; set; }

        [XmlIgnore]
        [PersistenceProperty("TOTALNF", DirectionParameter.InputOptional)]
        public decimal TotalNf { get; set; }

        [XmlIgnore]
        [PersistenceProperty("VALORIPINF", DirectionParameter.InputOptional)]
        public decimal ValorIpiNf { get; set; }

        [XmlIgnore]
        [PersistenceProperty("VALORBENEFNF", DirectionParameter.InputOptional)]
        public decimal ValorBenefNf { get; set; }

        [XmlIgnore]
        [PersistenceProperty("VALORACRESCIMONF", DirectionParameter.InputOptional)]
        public decimal ValorAcrescimoNf { get; set; }

        [XmlIgnore]
        public string NumControleFci { get; set; }

        [XmlIgnore]
        public int CstOrig { get; set; }

        #endregion

        #region IProdutoCalculo

        [XmlIgnore]
        uint IResumoCorte.Id
        {
            get { return IdProdPed; }
        }

        [XmlIgnore]
        IContainerCalculo IProdutoCalculo.Container { get; set; }

        [XmlIgnore]
        IAmbienteCalculo IProdutoCalculo.Ambiente { get; set; }

        [XmlIgnore]
        IDadosProduto IProdutoCalculo.DadosProduto { get; set; }

        [XmlIgnore]
        uint IProdutoCalculo.Id
        {
            get { return IdProdPed; }
        }

        [XmlIgnore]
        uint? IProdutoCalculo.IdAmbiente
        {
            get { return IdAmbientePedido; }
        }

        [XmlIgnore]
        uint IProdutoCalculo.IdProduto
        {
            get { return IdProd; }
        }

        [XmlIgnore]
        decimal IProdutoCalculo.ValorUnit
        {
            get { return ValorVendido; }
            set { ValorVendido = value; }
        }

        [XmlIgnore]
        float IProdutoCalculo.AlturaCalc
        {
            get { return Altura; }
        }

        [XmlIgnore]
        int? IProdutoCalculo.AlturaBenef
        {
            get { return AlturaBenef; }
        }

        [XmlIgnore]
        int? IProdutoCalculo.LarguraBenef
        {
            get { return LarguraBenef; }
        }

        [XmlIgnore]
        public int QtdeAmbiente
        {
            get
            {
                if (!PedidoMaoDeObra)
                    return 1;

                /* A variável IdAmbientePedido é setada com o valor do IdAmbientePedidoEspelho no método
                * ProdutosPedido.Instance.GetForRpt caso o IdAmbientePedido seja nulo e a solicitação esteja vindo
                * de um relatório de PCP, por isso, é necessário sempre, que o pedido for mão de obra, buscar
                * o id do ambiente pela variável IdProdPedEsp */

                //else if (!_forLiberacao) // Se não for para liberação, retorna a qtd de ambientes do pedido original
                //  return AmbientePedidoDAO.Instance.GetQtde(IdAmbientePedido);
                else
                {
                    /* Chamado 10235, ao invés de buscar o ambiente pelo IdAmbienteOrig é necessário buscar o ambiente pelo
                     * IdProdPedEsp, pois, caso haja alguma alteração do pedido no PCP o ambiente espelho perde a referência do
                     * ambiente original */

                    // Se for para liberação retorna a quantidade de ambientes do pedido espelho, a menos que não tenha referência deste
                    // ambiente no PCP ou a quantidade do PCP seja 0, neste caso será usada a quantidade no pedido original
                    // uint? idAmbienteEsp = AmbientePedidoEspelhoDAO.Instance.ObtemIdAmbienteByOrig(IdAmbientePedido);
                    // int qtde = AmbientePedidoEspelhoDAO.Instance.GetQtde(idAmbienteEsp);

                    // Caso a variável IdProdPedEsp esteja zerada significa que o pedido não tem espelho, dessa forma a variável
                    // IdAmbientePedido referencia a tabela ambiente_pedido e a quantidade do registro nesta tabela deve ser retornada.
                    if (IdProdPedEsp.GetValueOrDefault() == 0)
                        return AmbientePedidoDAO.Instance.GetQtde(IdAmbientePedido);

                    var ambienteEsp = AmbientePedidoEspelhoDAO.Instance.GetByIdProdPed(IdProdPedEsp.GetValueOrDefault());

                    return ambienteEsp != null && ambienteEsp.Qtde.GetValueOrDefault() > 0 ? ambienteEsp.Qtde.GetValueOrDefault() :
                        AmbientePedidoDAO.Instance.GetQtde(IdAmbientePedido);
                }
            }
        }

        #endregion
    }
}