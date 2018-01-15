using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(InfoAdicionalNfDAO))]
    [PersistenceClass("info_adicional_nf")]
    public class InfoAdicionalNf : Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe
    {
        #region Enumeradores

        public enum CodClasseConsumoEnergiaEnum
        {
            Comercial = 1,
            ConsumoProprio,
            IluminacaoPublica,
            Industrial,
            PoderPublico,
            Residencial,
            Rural,
            ServicoPublico
        }

        public enum CodClasseConsumoAguaEnum
        {
            ConsolidadoResAteRs50,
            ConsolidadoResAteRs100,
            ConsolidadoResAteRs200,
            ConsolidadoResAteRs300,
            ConsolidadoResAteRs400,
            ConsolidadoResAteRs500,
            ConsolidadoResAteRs1000,
            ConsolidadoResAcimaRs1000,
            ConsolidadoComIndAteRs50 = 20,
            ConsolidadoComIndAteRs100,
            ConsolidadoComIndAteRs200,
            ConsolidadoComIndAteRs300,
            ConsolidadoComIndAteRs400,
            ConsolidadoComIndAteRs500,
            ConsolidadoComIndAteRs1000,
            ConsolidadoComIndAcimaRs1000,
            ConsolidadoOrgaoPublico = 80,
            ConsolidadoOutrosAteRs50 = 90,
            ConsolidadoOutrosAteRs100,
            ConsolidadoOutrosAteRs200,
            ConsolidadoOutrosAteRs300,
            ConsolidadoOutrosAteRs400,
            ConsolidadoOutrosAteRs500,
            ConsolidadoOutrosAteRs1000,
            ConsolidadoOutrosAcimaRs1000,
            Individual = 99
        }

        public enum CodLigacaoEnum
        {
            Monofasico = 1,
            Bifasico,
            Trifasico
        }

        public enum CodGrupoTensaoEnum
        {
            AltaTensaoA1 = 1,
            AltaTensaoA2,
            AltaTensaoA3,
            AltaTensaoA3a,
            AltaTensaoA4,
            AltaTensaoSubterraneo,
            Residencial,
            ResidencialBaixaRenda,
            Rural,
            CooperativaEletrificacaoRural,
            ServicoPublicoIrrigacao,
            DemaisClasses,
            IluminacaoPublicaRedeDistribuicao,
            IluminacaoPublicaBulboLampada
        }

        public enum TipoCteEnum
        {
            Normal,
            Complementar,
            AnulacaoValores,
            Substituto
        }

        public enum TipoAssinanteEnum
        {
            ComercialIndustrial = 1,
            PoderPublico,
            ResidencialPessoaFisica,
            Publico,
            SemiPublico,
            Outros
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDNF", PersistenceParameterType.Key)]
        public uint IdNf { get; set; }

        [PersistenceProperty("CODCLASSECONSUMO")]
        public int? CodClasseConsumo { get; set; }

        [PersistenceProperty("VALORFORNECIDO")]
        public decimal ValorFornecido { get; set; }

        [PersistenceProperty("VALORNAOTRIBUTADO")]
        public decimal ValorNaoTributado { get; set; }

        [PersistenceProperty("VALORCOBRADOTERCEIROS")]
        public decimal ValorCobradoTerceiros { get; set; }

        [PersistenceProperty("CODLIGACAO")]
        public int? CodLigacao { get; set; }

        [PersistenceProperty("CODGRUPOTENSAO")]
        public int? CodGrupoTensao { get; set; }

        [PersistenceProperty("TIPOCTE")]
        public int? TipoCte { get; set; }

        [PersistenceProperty("IDCONTACONTABIL")]
        public uint? IdContaContabil { get; set; }

        [PersistenceProperty("TIPOASSINANTE")]
        public int? TipoAssinante { get; set; }

        [PersistenceProperty("CST")]
        public string Cst { get; set; }

        [PersistenceProperty("CSTORIG")]
        public int CstOrig { get; set; }

        [PersistenceProperty("CODVALORFISCAL")]
        public int CodValorFiscal { get; set; }

        [PersistenceProperty("ALIQICMS")]
        public float AliqICMS { get; set; }

        [PersistenceProperty("CSTIPI")]
        public int? CstIpi { get; set; }

        [PersistenceProperty("CSTPIS")]
        public int? CstPis { get; set; }

        [PersistenceProperty("CSTCOFINS")]
        public int? CstCofins { get; set; }

        [PersistenceProperty("NATBCCRED")]
        public int? NatBcCred { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("MODELO", DirectionParameter.InputOptional)]
        public string ModeloNf { get; set; }

        [PersistenceProperty("TRANSPORTE", DirectionParameter.InputOptional)]
        public bool IsNfTransporte { get; set; }

        [PersistenceProperty("TIPODOCUMENTO", DirectionParameter.InputOptional)]
        public int TipoDocumento { get; set; }

        [PersistenceProperty("CODINTERNOCONTACONTABIL", DirectionParameter.InputOptional)]
        public string CodInternoContaContabil { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool IsNfEnergiaEletrica
        {
            get 
            {
                NotaFiscal nf = new NotaFiscal();
                nf.Modelo = ModeloNf;
                return nf.IsNfEnergiaEletrica;
            }
        }

        public bool IsNfGas
        {
            get 
            {
                NotaFiscal nf = new NotaFiscal();
                nf.Modelo = ModeloNf;
                return nf.IsNfGas;
            }
        }

        public bool IsNfAgua
        {
            get 
            {
                NotaFiscal nf = new NotaFiscal();
                nf.Modelo = ModeloNf;
                return nf.IsNfAgua;
            }
        }

        public bool IsNfComunicacao
        {
            get 
            {
                NotaFiscal nf = new NotaFiscal();
                nf.Modelo = ModeloNf;
                return nf.IsNfComunicacao;
            }
        }

        public bool IsNfTelecomunicacao
        {
            get
            {
                NotaFiscal nf = new NotaFiscal();
                nf.Modelo = ModeloNf;
                return nf.IsNfTelecomunicacao;
            }
        }

        public string DescrCodClasseConsumo
        {
            get { return DataSourcesEFD.Instance.GetDescrCodClasseConsumoNf(IsNfAgua, CodClasseConsumo); }
        }

        public string DescrCodLigacao
        {
            get { return DataSourcesEFD.Instance.GetDescrCodLigacaoNf(CodLigacao); }
        }

        public string DescrCodGrupoTensao
        {
            get { return DataSourcesEFD.Instance.GetDescrCodGrupoTensaoNf(CodGrupoTensao); }
        }

        public string CstCompleto
        {
            get { return CstOrig + Cst; }
        }

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

        #endregion

        #region IInfoAdicionalNFe Members

        int? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.CodigoContaContabil
        {
            get { return (int?)IdContaContabil; }
        }

        int? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.CstIcmsOrigem
        {
            get { return CstOrig; }
        }

        Sync.Fiscal.Enumeracao.Cst.CstIcms? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.CstIcms
        {
            get { return (Sync.Fiscal.Enumeracao.Cst.CstIcms?)Glass.Conversoes.StrParaIntNullable(Cst); }
        }

        Sync.Fiscal.Enumeracao.Cst.CstIpi? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.CstIpi
        {
            get { return (Sync.Fiscal.Enumeracao.Cst.CstIpi?)CstIpi; }
        }

        Sync.Fiscal.Enumeracao.Cst.CstPisCofins? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.CstPis
        {
            get { return (Sync.Fiscal.Enumeracao.Cst.CstPisCofins?)CstPis; }
        }

        Sync.Fiscal.Enumeracao.Cst.CstPisCofins? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.CstCofins
        {
            get { return (Sync.Fiscal.Enumeracao.Cst.CstPisCofins?)CstCofins; }
        }

        Sync.Fiscal.EFD.DataSources.NaturezaBcCredito? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.NaturezaBcCredito
        {
            get { return (Sync.Fiscal.EFD.DataSources.NaturezaBcCredito?)NatBcCred; }
        }

        int? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.CodigoClasseConsumo
        {
            get { return CodClasseConsumo; }
        }

        Sync.Fiscal.Enumeracao.InfoAdicionalNFe.TipoCTe? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.TipoCTe
        {
            get { return (Sync.Fiscal.Enumeracao.InfoAdicionalNFe.TipoCTe?)TipoCte; }
        }

        Sync.Fiscal.Enumeracao.InfoAdicionalNFe.CodigoLigacao? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.CodigoLigacao
        {
            get { return (Sync.Fiscal.Enumeracao.InfoAdicionalNFe.CodigoLigacao?)CodLigacao; }
        }

        Sync.Fiscal.Enumeracao.InfoAdicionalNFe.CodigoGrupoTensao? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.CodigoGrupoTensao
        {
            get { return (Sync.Fiscal.Enumeracao.InfoAdicionalNFe.CodigoGrupoTensao?)CodGrupoTensao; }
        }

        Sync.Fiscal.Enumeracao.InfoAdicionalNFe.TipoAssinante? Sync.Fiscal.EFD.Entidade.IInfoAdicionalNFe.TipoAssinante
        {
            get { return (Sync.Fiscal.Enumeracao.InfoAdicionalNFe.TipoAssinante?)TipoAssinante; }
        }

        #endregion
    }
}