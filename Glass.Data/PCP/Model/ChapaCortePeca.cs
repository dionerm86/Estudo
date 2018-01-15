using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ChapaCortePecaDAO))]
    [PersistenceClass("chapa_corte_peca")]
    public class ChapaCortePeca
    {
        #region Propriedades

        [PersistenceProperty("IDCHAPACORTEPECA", PersistenceParameterType.IdentityKey)]
        public uint IdChapaCortePeca { get; set; }

        [PersistenceProperty("IDPRODIMPRESSAOCHAPA")]
        public uint IdProdImpressaoChapa { get; set; }

        [PersistenceProperty("IDPRODIMPRESSAOPECA")]
        public uint IdProdImpressaoPeca { get; set; }

        [PersistenceProperty("PLANOCORTE")]
        public string PlanoCorte { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public int NumSeq { get; set; }

        [PersistenceProperty("SaidaRevenda")]
        public bool SaidaRevenda { get; set; }

        #endregion
    }
}