using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;


namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FolgaPecaClienteDAO))]
    [PersistenceClass("folga_peca_cliente")]
    public class FolgaPecaCliente
    {
        [PersistenceProperty("IDPECAPROJETOMODELO", PersistenceParameterType.Key)]
        public uint IdPecaProjetoModelo { get; set; }

        [PersistenceProperty("IDCLIENTE", PersistenceParameterType.Key)]
        public uint IdCliente { get; set; }
            
        [PersistenceProperty("FOLGAALT06MM")]
        public int FolgaAlt06MM { get; set; }

        [PersistenceProperty("FOLGALARG06MM")]
        public int FolgaLarg06MM { get; set;}

        [PersistenceProperty("FOLGAALT08MM")]
        public int FolgaAlt08MM { get; set; }

        [PersistenceProperty("FOLGALARG08MM")]
        public int FolgaLarg08MM { get; set; }

        [PersistenceProperty("FOLGAALT10MM")]
        public int FolgaAlt10MM { get; set; }

        [PersistenceProperty("FOLGALARG10MM")]
        public int FolgaLarg10MM { get; set; }

        [PersistenceProperty("FOLGAALT12MM")]
        public int FolgaAlt12MM { get; set; }

        [PersistenceProperty("FOLGALARG12MM")]
        public int FolgaLarg12MM { get; set; }
    }
}
