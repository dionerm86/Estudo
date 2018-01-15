using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ParcelaNaoFiscalOriginalDAO))]
    [PersistenceClass("parcela_naofiscal_original")]
    public class ParcelaNaoFiscalOriginal
    {
        #region Propriedades

        [PersistenceProperty("IDPARCELAORIGINAL", PersistenceParameterType.IdentityKey)]
        public uint IdParcelaOriginal { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDNF")]
        public uint IdNf { get; set; }

        [PersistenceProperty("IDCONTAR")]
        public uint? IdContaR { get; set; }

        [PersistenceProperty("IDCONTAPG")]
        public uint? IdContaPg { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("IDCOMPRA")]
        public uint? IdCompra { get; set; }

        [PersistenceProperty("IDCONTA")]
        public uint IdConta { get; set; }

        [PersistenceProperty("DATAVEC")]
        public DateTime DataVec { get; set; }

        [PersistenceProperty("VALORVEC")]
        public decimal ValorVec { get; set; }

        [PersistenceProperty("NUMPARC")]
        public int NumParc { get; set; }

        [PersistenceProperty("NUMPARCMAX")]
        public int NumParcMax { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("USUCAD")]
        public uint UsuCad { get; set; }

        [PersistenceProperty("TIPOCONTA")]
        public byte? TipoConta { get; set; }

        #endregion
    }
}
