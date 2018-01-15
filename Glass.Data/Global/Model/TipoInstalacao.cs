using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TipoInstalacaoDAO))]
    [PersistenceClass("tipo_instalacao")]
    public class TipoInstalacao
    {
        [PersistenceProperty("IDTIPOINSTALACAO", PersistenceParameterType.Key)]
        public int IdTipoInstalacao { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }
    }
}
