using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AjusteDocumentoFiscalDAO)),
    PersistenceClass("ajuste_documento_fiscal")]
    public class AjusteDocumentoFiscal : Sync.Fiscal.EFD.Entidade.IAjusteDocumentoFiscal
    {
        #region Propriedades

        [PersistenceProperty("IDAJUSTEDOCUMENTOFISCAL", PersistenceParameterType.IdentityKey)]
        public uint IdAjusteDocumentoFiscal { get; set; }

        [PersistenceProperty("IDNF")]
        public uint? IdNf { get; set; }

        [PersistenceProperty("IDCTE")]
        public uint? IdCte { get; set; }

        [PersistenceProperty("IDOBSLANCFISCAL")]
        public uint IdObsLancFiscal { get; set; }

        [PersistenceProperty("IDAJBENINC")]
        public uint IdAjBenInc { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint? IdProd { get; set; }

        [PersistenceProperty("VALORBASECALCULOIMPOSTO")]
        public decimal? ValorBaseCalculoImposto { get; set; }

        [PersistenceProperty("ALIQUOTAIMPOSTO")]
        public float? AliquotaImposto { get; set; }

        [PersistenceProperty("VALORIMPOSTO")]
        public decimal? ValorImposto { get; set; }

        [PersistenceProperty("OUTROSVALORES")]
        public decimal? OutrosValores { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion

        #region Propriedades extendidas

        [PersistenceProperty("CODIGOAJUSTE", DirectionParameter.InputOptional)]
        public string CodigoAjuste { get; set; }

        [PersistenceProperty("DESCRICAOAJUSTE", DirectionParameter.InputOptional)]
        public string DescricaoAjuste { get; set; }

        [PersistenceProperty("DESCRICAOOBSLANCFISCAL", DirectionParameter.InputOptional)]
        public string DescricaoObsLancFiscal { get; set; }

        [PersistenceProperty("CODINTERNOPROD", DirectionParameter.InputOptional)]
        public string CodInternoProd { get; set; }

        #endregion

        #region Propriedades de suporte

        public string ValorBaseCalculoImpostoString
        {
            get { return ValorBaseCalculoImposto.HasValue ? ValorBaseCalculoImposto.Value.ToString() : null; }
            set { ValorBaseCalculoImposto = Glass.Conversoes.StrParaDecimalNullable(value); }
        }

        public string AliquotaImpostoString
        {
            get { return AliquotaImposto.HasValue ? AliquotaImposto.Value.ToString() : null; }
            set { AliquotaImposto = Glass.Conversoes.StrParaFloatNullable(value); }
        }

        public string ValorImpostoString
        {
            get { return ValorImposto.HasValue ? ValorImposto.Value.ToString() : null; }
            set { ValorImposto = Glass.Conversoes.StrParaDecimalNullable(value); }
        }

        public string OutrosValoresString
        {
            get { return OutrosValores.HasValue ? OutrosValores.Value.ToString() : null; }
            set { OutrosValores = Glass.Conversoes.StrParaDecimalNullable(value); }
        }

        #endregion

        #region IAjusteDocumentoFiscal Members

        int Sync.Fiscal.EFD.Entidade.IAjusteDocumentoFiscal.Codigo
        {
            get { return (int)IdAjusteDocumentoFiscal; }
        }

        int Sync.Fiscal.EFD.Entidade.IAjusteDocumentoFiscal.CodigoAjusteBeneficioIncentivo
        {
            get { return (int)IdAjBenInc; }
        }

        int Sync.Fiscal.EFD.Entidade.IAjusteDocumentoFiscal.CodigoObservacaoLancamentoFiscal
        {
            get { return (int)IdObsLancFiscal; }
        }

        int? Sync.Fiscal.EFD.Entidade.IAjusteDocumentoFiscal.CodigoProduto
        {
            get { return (int?)IdProd; }
        }

        string Sync.Fiscal.EFD.Entidade.IAjusteDocumentoFiscal.Observacao
        {
            get { return Obs; }
        }

        #endregion
    }
}
