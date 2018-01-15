using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{

    public enum ViaTransporteInternacional
    {
        /// <summary>
        /// Marítima.
        /// </summary>
        Maritima = 1,
        /// <summary>
        /// Fluvial.
        /// </summary>
        Fluvial,
        /// <summary>
        /// Lacustre.
        /// </summary>
        Lacustre,
        /// <summary>
        /// Aerea.
        /// </summary>
        Aerea,
        /// <summary>
        /// Postal.
        /// </summary>
        Postal,
        /// <summary>
        /// Ferroviária.
        /// </summary>
        Ferroviaria,
        /// <summary>
        /// Rodooviária.
        /// </summary>
        Rodoviaria,
        /// <summary>
        /// Conduto.
        /// </summary>
        Conduto,
        /// <summary>
        /// Meios Próprios.
        /// </summary>
        MeiosProprios,
        /// <summary>
        /// Entrada/Saída Ficta.
        /// </summary>        
        EntradaSaidaFicta
    }

    public enum TpIntermedio
    {
        /// <summary>
        /// Importação Por Conta Própria.
        /// </summary>
        ImportacaoContaPropria = 1,
        /// <summary>
        /// Importação Por Conta e Ordem.
        /// </summary>
        ImportacaoContaOrdem,
        /// <summary>
        /// Importação Por Conta e Ordem.
        /// </summary>        
        ImportacaoEncomenda
    }

    public enum MotivoDesoneracaoEnum
    {
        UsoAgropecuaria = 3,
        SUFRAMA = 7,
        Outros = 9,
        OrgaoFomentoDesenvolvimentoAgropecuario = 12
    }

    [PersistenceBaseDAO(typeof(ProdutosNfDAO))]
	[PersistenceClass("produtos_nf")]
	public class ProdutosNf : Colosoft.Data.BaseModel, Sync.Fiscal.EFD.Entidade.IProdutoNFe, IProdutoIcmsSt
    {
        #region Propriedades

        [PersistenceProperty("IDPRODNF", PersistenceParameterType.IdentityKey)]
        public uint IdProdNf { get; set; }

        [PersistenceProperty("IDNF")]
        public uint IdNf { get; set; }

        [Log("Produto", "Descricao", typeof(Produto))]
        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [Log("Natureza da Operação", "CodCompleto", typeof(NaturezaOperacaoDAO), "IdNaturezaOperacao", "ObtemElemento", true)]
        [PersistenceProperty("IDNATUREZAOPERACAO")]
        public uint? IdNaturezaOperacao { get; set; }

        [Log("Quantidade")]
        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [Log("Qtde. Trib.")]
        [PersistenceProperty("QTDETRIB")]
        public float QtdeTrib { get; set; }

        [Log("Qtde. Entrada")]
        [PersistenceProperty("QTDEENTRADA")]
        public Single QtdeEntrada { get; set; }

        [Log("Valor unitário")]
        [PersistenceProperty("VALORUNITARIO")]
        public decimal ValorUnitario { get; set; }

        [Log("Valor unitário trib.")]
        [PersistenceProperty("VALORUNITARIOTRIB")]
        public decimal ValorUnitarioTrib { get; set; }

        [Log("Altura")]
        [PersistenceProperty("ALTURA")]
        public float Altura { get; set; }

        [Log("Largura")]
        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [Log("Total m²")]
        [PersistenceProperty("TOTM")]
        public float TotM { get; set; }

        [Log("Total")]
        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        [Log("CST")]
        [PersistenceProperty("CST")]
        public string Cst { get; set; }

        /// <summary>
        /// 0 - Nacional
        /// 1 - Estrangeira - Importação direta
        /// 2 - Estrangeira - Adquirida no mercado interno
        /// </summary>
        [Log("CST Orig.")]
        [PersistenceProperty("CSTORIG")]
        public int CstOrig { get; set; }

        [Log("CSOSN")]
        [PersistenceProperty("CSOSN")]
        public string Csosn { get; set; }

        [Log("NCM")]
        [PersistenceProperty("NCM")]
        public string Ncm { get; set; }

        [Log("CST IPI", "Descr", typeof(DataSourcesEFD), "Id", "GetCstIpi", true)]
        [PersistenceProperty("CSTIPI")]
        public int? CstIpi { get; set; }

        [Log("Perc. red. BC ICMS")]
        [PersistenceProperty("PERCREDBCICMS")]
        public float PercRedBcIcms { get; set; }

        [Log("Perc. red. BC ICMS ST")]
        [PersistenceProperty("PERCREDBCICMSST")]
        public float PercRedBcIcmsSt { get; set; }

        [Log("Base cálc. ICMS")]
        [PersistenceProperty("BCICMS")]
        public decimal BcIcms { get; set; }

        [Log("Alíquota ICMS")]
        [PersistenceProperty("ALIQICMS")]
        public Single AliqIcms { get; set; }

        [Log("Valor ICMS")]
        [PersistenceProperty("VALORICMS")]
        public decimal ValorIcms { get; set; }

        [Log("Alíquota IPI")]
        [PersistenceProperty("ALIQIPI")]
        public Single AliqIpi { get; set; }

        [Log("Valor IPI")]
        [PersistenceProperty("VALORIPI")]
        public decimal ValorIpi { get; set; }

        [Log("MVA")]
        [PersistenceProperty("MVA")]
        public float Mva { get; set; }

        [Log("Base cálc. ICMS ST")]
        [PersistenceProperty("BCICMSST")]
        public decimal BcIcmsSt { get; set; }

        [Log("Alíquota ICMS ST")]
        [PersistenceProperty("ALIQICMSST")]
        public Single AliqIcmsSt { get; set; }

        [Log("Valor ICMS ST")]
        [PersistenceProperty("VALORICMSST")]
        public decimal ValorIcmsSt { get; set; }

        [Log("Peso")]
        [PersistenceProperty("PESO")]
        public Single Peso { get; set; }

        [Log("Plano de Conta Contábil", "Descricao", typeof(PlanoContaContabilDAO))]
        [PersistenceProperty("IDCONTACONTABIL")]
        public uint? IdContaContabil { get; set; }

        [Log("Lote")]
        [PersistenceProperty("LOTE")]
        public string Lote { get; set; }

        [Log("Informação Adicional")]
        [PersistenceProperty("INFADIC")]
        public string InfAdic { get; set; }

        [Log("Tipo de Mercadoria")]
        [PersistenceProperty("TIPOMERCADORIA")]
        public int? TipoMercadoria { get; set; }

        [PersistenceProperty("NUMDOCIMP")]
        public string NumDocImp { get; set; }

        [PersistenceProperty("DATAREGDOCIMP")]
        public DateTime? DataRegDocImp { get; set; }

        [PersistenceProperty("LOCALDESEMBARACO")]
        public string LocalDesembaraco { get; set; }

        [PersistenceProperty("UFDESEMBARACO")]
        public string UfDesembaraco { get; set; }

        [PersistenceProperty("DATADESEMBARACO")]
        public DateTime? DataDesembaraco { get; set; }

        [PersistenceProperty("CODEXPORTADOR")]
        public string CodExportador { get; set; }

        [PersistenceProperty("BCII")]
        public decimal BcIi { get; set; }

        [PersistenceProperty("DESPADUANEIRA")]
        public decimal DespAduaneira { get; set; }

        [PersistenceProperty("VALORII")]
        public decimal ValorIi { get; set; }

        [Log("Valor IOF")]
        [PersistenceProperty("VALORIOF")]
        public decimal ValorIof { get; set; }

        [Log("Natureza da Base de Cálculo de Crédito", "Descr", typeof(DataSourcesEFD), "Id", "GetDescrNaturezaBcCredito", true)]
        [PersistenceProperty("NATUREZABCCRED")]
        public int? NaturezaBcCred { get; set; }

        [Log("Indicador da Natureza do Frete", "Descr", typeof(DataSourcesEFD), "Id", "GetDescrIndNaturezaFrete", true)]
        [PersistenceProperty("INDNATUREZAFRETE")]
        public int? IndNaturezaFrete { get; set; }

        [Log("Tipo de Contribuição Social", "Descr", typeof(DataSourcesEFD), "Id", "GetDescrCodCont", true)]
        [PersistenceProperty("CODCONT")]
        public int? CodCont { get; set; }

        [Log("Tipo de Crédito", "Descr", typeof(DataSourcesEFD), "Id", "GetDescrCodCred", true)]
        [PersistenceProperty("CODCRED")]
        public int? CodCred { get; set; }

        [Log("CST PIS", "Descr", typeof(DataSourcesEFD), "Id", "GetDescrCstPisCofins", true)]
        [PersistenceProperty("CSTPIS")]
        public int? CstPis { get; set; }

        [Log("Base de cálculo PIS")]
        [PersistenceProperty("BCPIS")]
        public decimal BcPis { get; set; }

        [Log("Alíquota PIS")]
        [PersistenceProperty("ALIQPIS")]
        public float AliqPis { get; set; }

        [Log("Valor PIS")]
        [PersistenceProperty("VALORPIS")]
        public decimal ValorPis { get; set; }

        [Log("CST Cofins", "Descr", typeof(DataSourcesEFD), "Id", "GetDescrCstPisCofins", true)]
        [PersistenceProperty("CSTCOFINS")]
        public int? CstCofins { get; set; }

        [Log("Base de cálculo Cofins")]
        [PersistenceProperty("BCCOFINS")]
        public decimal BcCofins { get; set; }

        [Log("Alíquota Cofins")]
        [PersistenceProperty("ALIQCOFINS")]
        public float AliqCofins { get; set; }

        [Log("Valor Cofins")]
        [PersistenceProperty("VALORCOFINS")]
        public decimal ValorCofins { get; set; }

        [Log("Número Ato Concessionário Drawback")]
        [PersistenceProperty("NUMACDRAWBACK")]
        public string NumACDrawback { get; set; }

        [Log("Número do Registro de Exportação")]
        [PersistenceProperty("NumRegExportacao")]
        public string NumRegExportacao { get; set; }

        [Log("Chave de Acesso da NF-e recebida para exportação")]
        [PersistenceProperty("ChaveAcessoExportacao")]
        public string ChaveAcessoExportacao { get; set; }

        [Log("Quantidade do item realmente exportado")]
        [PersistenceProperty("QtdeExportada")]
        public decimal QtdeExportada { get; set; }

        [Log("Tipo de Documento Importação")]
        [PersistenceProperty("TIPODOCUMENTOIMP")]
        public int? TipoDocumentoImportacao { get; set; }

        [PersistenceProperty("QTDIMPRESSO", DirectionParameter.Input)]
        public int QtdImpresso { get; set; }

        [Log("Obs.")]
        [PersistenceProperty("OBS", DirectionParameter.Input)]
        public string Obs { get; set; }

        [PersistenceProperty("ISCHAPAIMPORTADA")]
        public bool IsChapaImportada { get; set; }

        [Log("Cód. Valor Fiscal")]
        [PersistenceProperty("CODVALORFISCAL")]
        public uint? CodValorFiscal { get; set; }

        private uint? _codValorFiscalIPI;

        /// <summary>
        /// Código de valores fiscais. Define se haverá crédito ICMS/IPI
        /// 1 - Oper. com crédito do Imposto
        /// 2 - Oper. sem crédito do Imposto - Isentas ou não Tributadas
        /// 3 - Oper. sem crédito do Imposto - Outras
        /// </summary>
        [Log("Cód. Valor Fiscal IPI")]
        [PersistenceProperty("CODVALORFISCALIPI")]
        public uint? CodValorFiscalIPI
        {
            get 
            {
                if(CstIpi == 00 || CstIpi == 50)
                    _codValorFiscalIPI = 1;
                if (CstIpi == 02 || CstIpi == 03 || CstIpi == 52 || CstIpi == 53)
                    _codValorFiscalIPI = 2;
                if (CstIpi == 01 || CstIpi == 04 || CstIpi == 05 || CstIpi == 49 || CstIpi == 51 || CstIpi == 54 || CstIpi == 55 || CstIpi == 99)
                    _codValorFiscalIPI = 3;

                return _codValorFiscalIPI; 
            }
            set { _codValorFiscalIPI = value; }
        }

        [Log("Valor total tributado")]
        [PersistenceProperty("ValorTotalTrib")]
        public decimal ValorTotalTrib { get; set; }

        [Log("Descrição item genérico")]
        [PersistenceProperty("DESCRICAOITEMGENERICO")]
        public string DescricaoItemGenerico { get; set; }

        [Log("Valor da parcela importada")]
        [PersistenceProperty("PARCELAIMPORTADA")]
        public decimal ParcelaImportada { get; set; }

        [Log("Valor da saída interestadual")]
        [PersistenceProperty("SAIDAINTERESTADUAL")]
        public decimal SaidaInterestadual { get; set; }

        [Log("Valor do conteúdo de importação")]
        [PersistenceProperty("CONTEUDOIMPORTACAO")]
        public decimal ConteudoImportacao { get; set; }

        [PersistenceProperty("NUMCONTROLEFCI")]
        public byte[] NumControleFci { get; set; }

        [Log("Via Transporte")]
        [PersistenceProperty("TpViaTransp")]
        public ViaTransporteInternacional TpViaTransp { get; set; }

        [Log("VAFRMM")]
        [PersistenceProperty("VAFRMM")]
        public decimal VAFRMM { get; set; }

        [PersistenceProperty("CnpjAdquirenteEncomendante")]
        public string CnpjAdquirenteEncomendante { get; set; }

        [PersistenceProperty("TpIntermedio")]
        public TpIntermedio TpIntermedio { get; set; }

        [PersistenceProperty("UFTerceiro")]
        public string UfTerceiro { get; set; }

        [PersistenceProperty("ValorIcmsDesonerado")]
        public decimal ValorIcmsDesonerado { get; set; }

        [PersistenceProperty("MotivoDesoneracao")]
        public int MotivoDesoneracao { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _descrProduto;

        [PersistenceProperty("DescrProduto", DirectionParameter.InputOptional)]
        public string DescrProduto
        {
            get { return ProdutoDAO.Instance.GetDescrProduto(_descrProduto, DescricaoItemGenerico); }
            set { _descrProduto = value; }
        }

        [PersistenceProperty("IdGrupoProd", DirectionParameter.InputOptional)]
        public uint IdGrupoProd { get; set; }

        [PersistenceProperty("IdSubgrupoProd", DirectionParameter.InputOptional)]
        public uint IdSubgrupoProd { get; set; }

        [PersistenceProperty("CodInterno", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("CodNaturezaOperacao", DirectionParameter.InputOptional)]
        public string CodNaturezaOperacao { get; set; }

        [PersistenceProperty("CodCfop", DirectionParameter.InputOptional)]
        public string CodCfop { get; set; }

        private string _m2Minimo;

        [PersistenceProperty("AREAMINIMA", DirectionParameter.InputOptional)]
        public string M2Minimo
        {
            get { return !String.IsNullOrEmpty(_m2Minimo) ? _m2Minimo.ToString().Replace(',', '.') : "0"; }
            set { _m2Minimo = value; }
        }

        [PersistenceProperty("UNIDADE", DirectionParameter.InputOptional)]
        public string Unidade { get; set; }

        [PersistenceProperty("UNIDADETRIB", DirectionParameter.InputOptional)]
        public string UnidadeTrib { get; set; }

        [PersistenceProperty("ESPESSURA", DirectionParameter.InputOptional)]
        public float Espessura { get; set; }

        [PersistenceProperty("TEMMOVIMENTACAOBEMATIVOIMOB", DirectionParameter.InputOptional)]
        public bool TemMovimentacaoBemAtivoImob { get; set; }

        [PersistenceProperty("CODINTERNOCONTACONTABIL", DirectionParameter.InputOptional)]
        public string CodInternoContaContabil { get; set; }

        [PersistenceProperty("DESCRPLANOCONTACONTABIL", DirectionParameter.InputOptional)]
        public string DescrPlanoContaContabil { get; set; }

        [PersistenceProperty("VALORFRETE", DirectionParameter.InputOptional)]
        public decimal ValorFrete { get; set; }

        [PersistenceProperty("VALORSEGURO", DirectionParameter.InputOptional)]
        public decimal ValorSeguro { get; set; }

        [PersistenceProperty("VALOROUTRASDESP", DirectionParameter.InputOptional)]
        public decimal ValorOutrasDespesas { get; set; }

        [PersistenceProperty("VALORDESCONTO", DirectionParameter.InputOptional)]
        public decimal ValorDesconto { get; set; }

        [PersistenceProperty("NUMERONFE", DirectionParameter.InputOptional)]
        public uint NumeroNfe { get; set; }

        [PersistenceProperty("EMITENTENFE", DirectionParameter.InputOptional)]
        public string EmitenteNfe { get; set; }

        [PersistenceProperty("DESTINATARIONFE", DirectionParameter.InputOptional)]
        public string DestinatarioNfe { get; set; }

        [PersistenceProperty("DATAEMISSAONFE", DirectionParameter.InputOptional)]
        public DateTime DataEmissaoNfe { get; set; }

        [Log("Ajuste/Benefício/Incentivo")]
        [PersistenceProperty("CODAJBENINC", DirectionParameter.InputOptional)]
        public string CodAjBenInc { get; set; }

        [PersistenceProperty("TipoDocumento", DirectionParameter.InputOptional)]
        public int TipoDocumento { get; set; }

        [PersistenceProperty("TotalEfd", DirectionParameter.InputOptional)]
        public decimal TotalEfd
        {
            get { return Total; }
            set { Total = value; }
        }

        [PersistenceProperty("BcPisEfd", DirectionParameter.InputOptional)]
        public decimal BcPisEfd
        {
            get { return BcPis; }
            set { BcPis = value; }
        }

        [PersistenceProperty("BcCofinsEfd", DirectionParameter.InputOptional)]
        public decimal BcCofinsEfd
        {
            get { return BcCofins; }
            set { BcCofins = value; }
        }

        [PersistenceProperty("GTINPRODUTO", DirectionParameter.InputOptional)]
        public string GTINProduto { get; set; }

        [PersistenceProperty("DescrTipoProduto", DirectionParameter.InputOptional)]
        public string DescrTipoProduto { get; set; }

        [PersistenceProperty("DescrUsuCad", DirectionParameter.InputOptional)]
        public string DescrUsuCad { get; set; }

        #endregion

        #region Propriedades de Suporte

        public float MvaProdutoNf
        {
            get { return Mva; }
        }

        private uint? _idCfop;

        public uint? IdCfop
        {
            get
            {
                if (_idCfop == null && IdNaturezaOperacao > 0)
                    _idCfop = NaturezaOperacaoDAO.Instance.ObtemIdCfop(IdNaturezaOperacao.Value);

                return _idCfop;
            }
        }

        [Log("CST")]
        public string CstCompleto
        {
            get { return CstOrig + Cst; }
        }

        public string CsosnCompleto
        {
            get { return CstOrig + Csosn; }
        }

        public string AlturaRpt
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio((int)IdGrupoProd) ? Altura + "ml" : Altura.ToString(); }
        }

        public string IsVidro
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)IdGrupoProd).ToString().ToLower(); }
        }

        public string IsAluminio
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.IsAluminio((int)IdGrupoProd).ToString().ToLower(); }
        }

        public int TipoCalc
        {
            get { return Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)IdGrupoProd, (int)IdSubgrupoProd, true); }
        }

        public bool AlturaEnabled
        {
            get { return Glass.Calculos.AlturaEnabled(TipoCalc); }
        }

        public bool LarguraEnabled
        {
            get { return Glass.Calculos.LarguraEnabled(TipoCalc); }
        }

        public string DescrQtde
        {
            get { return Qtde + (QtdeTrib > 0 && Qtde != QtdeTrib ? " (Qtd. Trib.: " + QtdeTrib + ")" : ""); }
        }

        public float QtdeEntradaRestante
        {
            get { return Qtde - QtdeEntrada; }
        }

        public string QtdeEntradaEstoque
        {
            get
            {
                return QtdeEntradaRestante.ToString();
            }
        }

        public float QtdMarcadaEntrada { get; set; }

        public string DescrPercRedBcIcms
        {
            get 
            {
                return Cst == "20" || Cst == "70" ? " (Perc. Red. BC ICMS: " + PercRedBcIcms + "%)" : ""; 
            }
        }

        public string DescrPercRedBcIcmsSt
        {
            get { return Cst == "70" ? " (Perc. Red. BC ICMS ST: " + PercRedBcIcmsSt + "%)" : ""; }
        }

        public string DescrCstIpi
        {
            get { return Colosoft.Translator.Translate((ProdutoCstIpi?)CstIpi).Format(); }
        }

        private bool? _isNfImportacao = null;

        public bool IsNfImportacao
        {
            get
            {
                if (_isNfImportacao == null)
                    _isNfImportacao = NotaFiscalDAO.Instance.IsNotaFiscalImportacao(IdNf);

                return _isNfImportacao.GetValueOrDefault();
            }
        }

        public string EmitenteDestinatario
        {
            get { return !String.IsNullOrEmpty(EmitenteNfe) ? EmitenteNfe : DestinatarioNfe; }
        }

        [Log("Natureza BC do Crédito")]
        public string DescrNaturezaBcCred
        {
            get { return DataSourcesEFD.Instance.GetDescrNaturezaBcCredito(NaturezaBcCred); }
        }

        [Log("Indicador Natureza Frete")]
        public string DescrIndNaturezaFrete
        {
            get { return DataSourcesEFD.Instance.GetDescrIndNaturezaFrete(IndNaturezaFrete); }
        }

        [Log("Tipo de Contribuição Social")]
        public string DescrCodCont
        {
            get { return DataSourcesEFD.Instance.GetDescrCodCont(CodCont); }
        }

        [Log("Tipo de Crédito")]
        public string DescrCodCred
        {
            get { return DataSourcesEFD.Instance.GetDescrCodCred(CodCred); }
        }

        [Log(" CST PIS/Cofins")]
        public string DescrCstPis
        {
            get { return DataSourcesEFD.Instance.GetDescrCstPisCofins(CstPis); }
        }

        public string DescrCstCofins
        {
            get { return DataSourcesEFD.Instance.GetDescrCstPisCofins(CstCofins); }
        }

        internal decimal VlBcIcmsEFD
        {
            get { return BcIcmsSt > 0 ? BcIcmsSt : BcIcms; }
        }

        internal decimal VlIcmsEFD
        {
            get { return ValorIcmsSt > 0 ? ValorIcmsSt : ValorIcms; }
        }

        private string _idProdNfEFD = null;

        internal string IdProdNfEFD
        {
            get
            {
                if (String.IsNullOrEmpty(_idProdNfEFD))
                    _idProdNfEFD = IdProdNf.ToString();

                return _idProdNfEFD;
            }
            set { _idProdNfEFD = value; }
        }

        internal int MesEmissao
        {
            get { return DataEmissaoNfe.Month; }
        }

        internal int AnoEmissao
        {
            get { return DataEmissaoNfe.Year; }
        }

        internal string ModeloNf { get; set; }

        internal string CfopNf { get; set; }

        /// <summary>
        /// Código de valores fiscais. Define se haverá crédito ICMS/IPI
        /// 1 - Oper. com crédito do Imposto
        /// 2 - Oper. sem crédito do Imposto - Isentas ou não Tributadas
        /// 3 - Oper. sem crédito do Imposto - Outras
        /// </summary>
        public string CodValorFiscalString
        {
            get
            {
                string ret = "";

                switch (CodValorFiscal)
                {
                    case 1: ret = TipoDocumento != 2 ? "Oper. com crédito do Imposto" : "Imposto Debitado"; break;
                    case 2: ret = TipoDocumento != 2 ? "Oper. sem crédito do Imposto - Isentas ou não Tributadas" : "Isentas ou não Tributadas"; break;
                    case 3: ret = TipoDocumento != 2 ? "Oper. sem crédito do Imposto - Outras" : "Outras"; break;
                }

                return ret;
            }
        }

        [Log("Número de Controle FCI")]
        public string NumControleFciStr
        {
            get { return NumControleFci != null && NumControleFci.Length > 0 ? new Guid(NumControleFci).ToString().ToUpper() : null; }
            set { NumControleFci = value == null ? null : new Guid(value).ToByteArray(); }
        }

        public string VAFRMMString
        {
            get { return VAFRMM.ToString(); }
            set { VAFRMM = Conversoes.StrParaDecimal(value); }
        }

        #endregion

        #region IProdutoNFe Members

        int Sync.Fiscal.EFD.Entidade.IProdutoNFe.Codigo
        {
            get { return (int)IdProdNf; }
        }

        int Sync.Fiscal.EFD.Entidade.IProdutoNFe.CodigoNFe
        {
            get { return (int)IdNf; }
        }

        int? Sync.Fiscal.EFD.Entidade.IProdutoNFe.CodigoNaturezaOperacao
        {
            get { return (int?)IdNaturezaOperacao; }
            set { IdNaturezaOperacao = (uint?)value; }
        }

        int? Sync.Fiscal.EFD.Entidade.IProdutoNFe.CodigoCfop
        {
            get { return null; }
            set { }
        }

        int Sync.Fiscal.EFD.Entidade.IProdutoNFe.CodigoProduto
        {
            get { return (int)IdProd; }
        }

        int? Sync.Fiscal.EFD.Entidade.IProdutoNFe.CodigoContaContabil
        {
            get { return (int?)IdContaContabil; }
            set { IdContaContabil = (uint?)value; }
        }

        Sync.Fiscal.EFD.DataSources.NaturezaBcCredito? Sync.Fiscal.EFD.Entidade.IProdutoNFe.NaturezaBcCredito
        {
            get { return (Sync.Fiscal.EFD.DataSources.NaturezaBcCredito?)NaturezaBcCred; }
        }

        Sync.Fiscal.EFD.DataSources.IndNaturezaFrete? Sync.Fiscal.EFD.Entidade.IProdutoNFe.IndicadorNaturezaFrete
        {
            get { return (Sync.Fiscal.EFD.DataSources.IndNaturezaFrete?)IndNaturezaFrete; }
        }

        Sync.Fiscal.Enumeracao.CodigoTipoCredito? Sync.Fiscal.EFD.Entidade.IProdutoNFe.CodCred
        {
            get { return (Sync.Fiscal.Enumeracao.CodigoTipoCredito?)CodCred; }
        }

        Sync.Fiscal.Enumeracao.CodigoContribuicaoSocial? Sync.Fiscal.EFD.Entidade.IProdutoNFe.CodCont
        {
            get { return (Sync.Fiscal.Enumeracao.CodigoContribuicaoSocial?)CodCont; }
            set { CodCont = (int?)value; }
        }

        private float? _quantidade;

        float Sync.Fiscal.EFD.Entidade.IProdutoNFe.Quantidade
        {
            get
            {
                if (_quantidade == null)
                    _quantidade = ProdutosNfDAO.Instance.ObtemQtdDanfe(this, true);

                return _quantidade.GetValueOrDefault();
            }
        }

        string Sync.Fiscal.EFD.Entidade.IProdutoNFe.InformacoesAdicionais
        {
            get { return InfAdic; }
            set { InfAdic = value; }
        }

        string Sync.Fiscal.EFD.Entidade.IProdutoNFe.NumeroDocumentoImportacao
        {
            get { return NumDocImp; }
        }

        string Sync.Fiscal.EFD.Entidade.IProdutoNFe.NumeroACDrawback
        {
            get { return NumACDrawback; }
        }

        Sync.Fiscal.EFD.DataSources.TipoDocImp? Sync.Fiscal.EFD.Entidade.IProdutoNFe.TipoDocumentoImportacao
        {
            get { return (Sync.Fiscal.EFD.DataSources.TipoDocImp?)TipoDocumentoImportacao; }
        }

        int Sync.Fiscal.EFD.Entidade.IProdutoNFe.CstIcmsOrigem
        {
            get { return CstOrig; }
        }

        Sync.Fiscal.Enumeracao.Cst.CstIcms Sync.Fiscal.EFD.Entidade.IProdutoNFe.CstIcms
        {
            get { return (Sync.Fiscal.Enumeracao.Cst.CstIcms)Glass.Conversoes.StrParaInt(Cst); }
        }

        float Sync.Fiscal.EFD.Entidade.IProdutoNFe.PercentualReducaoBcIcms
        {
            get { return PercRedBcIcms; }
        }

        Sync.Fiscal.Enumeracao.Cst.CstIpi? Sync.Fiscal.EFD.Entidade.IProdutoNFe.CstIpi
        {
            get { return (Sync.Fiscal.Enumeracao.Cst.CstIpi?)CstIpi; }
        }

        Sync.Fiscal.Enumeracao.Cst.CstPisCofins? Sync.Fiscal.EFD.Entidade.IProdutoNFe.CstPis
        {
            get { return (Sync.Fiscal.Enumeracao.Cst.CstPisCofins?)CstPis; }
            set { CstPis = (int?)value; }
        }

        Sync.Fiscal.Enumeracao.Cst.CstPisCofins? Sync.Fiscal.EFD.Entidade.IProdutoNFe.CstCofins
        {
            get { return (Sync.Fiscal.Enumeracao.Cst.CstPisCofins?)CstCofins; }
            set { CstCofins = (int?)value; }
        }

        #endregion

        #region IProdutoIcmsSt Members

        int IProdutoIcmsSt.IdProd
        {
            get { return (int)IdProd; }
        }

        decimal IProdutoIcmsSt.Total
        {
            get { return Total; }
        }

        float IProdutoIcmsSt.AliquotaIcms
        {
            get { return AliqIcms; }
        }

        float IProdutoIcmsSt.AliquotaIpi
        {
            get { return AliqIpi; }
        }

        float IProdutoIcmsSt.AliquotaIcmsSt
        {
            get { return AliqIcmsSt; }
        }

        float IProdutoIcmsSt.PercentualReducaoBaseCalculo
        {
            get { return PercRedBcIcmsSt; }
        }

        #endregion
    }
}