using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TabelaCodigoReceitaDAO))]
    [PersistenceClass("sped_tabela_codigos_receita")]
    public class TabelaCodigoReceita
    {
        #region Propriedades

        [PersistenceProperty("CODIGO", PersistenceParameterType.Key)]
        public uint Codigo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        [PersistenceProperty("UF")]
        public string UF { get; set; }

        #endregion
    }
}