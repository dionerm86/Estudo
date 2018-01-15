using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    [PersistenceClass("relatorio_dinamico")]
    public class RelatorioDinamico : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDRELATORIODINAMICO", PersistenceParameterType.IdentityKey)]
        public int IdRelatorioDinamico { get; set; }

        [PersistenceProperty("NOMERELATORIO")]
        public string NomeRelatorio { get; set; }

        [PersistenceProperty("COMANDOSQL")]
        public string ComandoSql { get; set; }

        [PersistenceProperty("SITUACAO")]
        public Situacao Situacao { get; set; }

        [PersistenceProperty("LinkInsercaoNome")]
        public string LinkInsercaoNome { get; set; }

        [PersistenceProperty("LinkInsersaoUrl")]
        public string LinkInsersaoUrl { get; set; }

        [PersistenceProperty("QuantidadeRegistrosPorPagina")]
        public int QuantidadeRegistrosPorPagina { get; set; }
    }
}
