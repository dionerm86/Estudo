using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ParcelaNfDAO))]
    [PersistenceClass("parcela_nf")]
    public class ParcelaNf : Sync.Fiscal.EFD.Entidade.IParcelaNFe
    {
        #region Propriedades

        [PersistenceProperty("NUMPARCELA", PersistenceParameterType.IdentityKey)]
        public uint NumParc { get; set; }

        [PersistenceProperty("IDNF")]
        public uint IdNf { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime? Data { get; set; }

        [PersistenceProperty("NUMBOLETO")]
        public string NumBoleto { get ;set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrValor
        {
            get { return Valor.ToString("F2"); }
        }

        #endregion

        #region IParcelaNFe Members

        DateTime Sync.Fiscal.EFD.Entidade.IParcelaNFe.DataVencimento
        {
            get { return Data ?? new DateTime(); }
        }

        #endregion
    }
}