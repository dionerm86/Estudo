using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ExportacaoDAO))]
    [PersistenceClass("exportacao")]
    public class Exportacao
    {
        #region Propriedades

        [PersistenceProperty("IDEXPORTACAO", PersistenceParameterType.IdentityKey)]
        public uint IdExportacao { get; set; }

        [PersistenceProperty("IDFORNEC")]
        public uint IdFornec { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("DATAEXPORTACAO")]
        public DateTime DataExportacao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion
    }
}