using GDA;
using System;

namespace Glass.Data.Model
{
    [PersistenceClass("detalhamento_debitos_piscofins"),
     PersistenceBaseDAO(typeof(DAL.DetalhamentoDebitosPisCofinsDAO))]
    public class DetalhamentoDebitosPisCofins : Colosoft.Data.BaseModel, Sync.Fiscal.EFD.Entidade.IDebitoPisCofins
    {
        [PersistenceProperty("IDDETALHAMENTOPISCOFINS", PersistenceParameterType.IdentityKey)]
        public int IdDetalhamentoPisCofins { get; set; }

        [PersistenceProperty("DATAPAGAMENTO")]
        public DateTime DataPagamento { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        [PersistenceProperty("CODIGORECEITA")]
        public string CodigoReceita { get; set; }

        [PersistenceProperty("CUMULATIVO")]
        public bool Cumulativo { get; set; }

        [PersistenceProperty("VALORPAGAMENTO")]
        public decimal ValorPagamento { get; set; }

        #region IDebitoPisCofins Members

        Sync.Fiscal.Enumeracao.TipoImposto Sync.Fiscal.EFD.Entidade.IDebitoPisCofins.TipoImposto
        {
            get { return (Sync.Fiscal.Enumeracao.TipoImposto)TipoImposto; }
        }

        decimal Sync.Fiscal.EFD.Entidade.IDebitoPisCofins.ValorDebito
        {
            get { return ValorPagamento; }
        }

        #endregion
    }
}
