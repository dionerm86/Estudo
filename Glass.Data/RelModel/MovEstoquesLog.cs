using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(MovEstoquesLogDAO))]
    public class MovEstoquesLog
    {
        [PersistenceProperty("IDPRODNF")]
        public uint IdProdNf { get; set; }

        [PersistenceProperty("CODPROD")]
        public string CodProd { get; set; }

        [PersistenceProperty("DESCRICAOPROD")]
        public string DescricaoProd { get; set; }

        [PersistenceProperty("MOVESTOQUEFISCAL")]
        public uint? MovEstoqueFiscal { get; set; }

        [PersistenceProperty("TIPOMOVFISCAL")]
        public string TipoMovFiscal { get; set; }

        [PersistenceProperty("QTDEMOVFISCAL")]
        public uint? QtdeMovFiscal { get; set; }

        [PersistenceProperty("DATAMOVFISCAL")]
        public DateTime? DataMovFiscal { get; set; }

        [PersistenceProperty("MOVESTOQUEREAL")]
        public uint? MovEstoqueReal { get; set; }

        [PersistenceProperty("TIPOMOVREAL")]
        public string TipoMovReal { get; set; }

        [PersistenceProperty("QTDEMOVREAL")]
        public uint? QtdeMovReal { get; set; }

        [PersistenceProperty("DATAMOVREAL")]
        public DateTime? DataMovReal { get; set; }
    }
}
