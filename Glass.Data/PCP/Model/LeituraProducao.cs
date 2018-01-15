using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(LeituraProducaoDAO))]
    [PersistenceClass("leitura_producao")]
    public class LeituraProducao
    {
        #region Propriedades

        [PersistenceProperty("IDLEITURAPROD", PersistenceParameterType.IdentityKey)]
        public uint IdLeituraProd { get; set; }

        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        public uint IdProdPedProducao { get; set; }

        [PersistenceProperty("IDSETOR")]
        public uint IdSetor { get; set; }

        [PersistenceProperty("IDFUNCLEITURA")]
        public uint IdFuncLeitura { get; set; }

        [PersistenceProperty("DATALEITURA")]
        public DateTime? DataLeitura { get; set; }

        [PersistenceProperty("IDCAVALETE")]
        public int? IdCavalete { get; set; }

        [PersistenceProperty("PRONTOROTEIRO")]
        public bool ProntoRoteiro { get; set; }

        #endregion
    }
}
