using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ParcelasCompraDAO))]
    [PersistenceClass("parcelas_compra")]
    public class ParcelasCompra
    {
        #region Propriedades

        [PersistenceProperty("NumParc", PersistenceParameterType.IdentityKey)]
        public uint NumParc { get; set; }

        [PersistenceProperty("IDCOMPRA")]
        public uint IdCompra { get; set; }

        [PersistenceProperty("IDFORMAPAGTO")]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime? Data { get; set; }

        [PersistenceProperty("NUMBOLETO")]
        public string NumBoleto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrValor
        {
            get { return Valor.ToString("F2"); }
        }

        #endregion
    }
}