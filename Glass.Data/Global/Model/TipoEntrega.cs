using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TipoEntregaDAO))]
    [PersistenceClass("tipo_entrega")]
    public class TipoEntrega
    {
        [PersistenceProperty("IDTIPOENTREGA", PersistenceParameterType.Key)]
        public int IdTipoEntrega { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }
    }
}
