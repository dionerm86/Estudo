using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TipoSugestaoClienteDAO))]
    [PersistenceClass("tipo_sugestao_cliente")]
    public class TipoSugestaoCliente
    {
        [PersistenceProperty("IDTIPOSUGESTAOCLIENTE", PersistenceParameterType.IdentityKey)]
        public int IdTipoSugestaoCliente { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }
    }
}
