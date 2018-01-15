using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ExpedicaoChapaDAO))]
    [PersistenceClass("expedicao_chapa")]
    public class ExpedicaoChapa
    {
        [PersistenceProperty("IdExpChapa", PersistenceParameterType.IdentityKey)]
        public int IdExpChapa { get; set; }

        [PersistenceProperty("IdProdImpressaoChapa")]
        public int IdProdImpressaoChapa { get; set; }

        [PersistenceProperty("IdPedido")]
        public int IdPedido { get; set; }

        [PersistenceProperty("SaidaBalcao")]
        public bool SaidaBalcao { get; set; }

        [PersistenceProperty("IdFuncLeitura")]
        public int IdFuncLeitura { get; set; }

        [PersistenceProperty("DataLeitura")]
        public DateTime DataLeitura { get; set; }
    }
}
