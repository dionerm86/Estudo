using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ArquivoFCIDAO))]
    [PersistenceClass("arquivo_fci")]
    public class ArquivoFCI
    {
        #region Propiedades

        [PersistenceProperty("IDARQUIVOFCI", PersistenceParameterType.IdentityKey)]
        public uint IdArquivoFCI { get; set; }

        [PersistenceProperty("USUCAD")]
        public uint UsuCad { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("USUIMPORTACAO")]
        public uint? UsuImportacao { get; set; }

        [PersistenceProperty("DATAIMPORTACAO")]
        public DateTime? DataImportacao { get; set; }

        [PersistenceProperty("PROTOCOLO")]
        public uint? Protocolo { get; set; }

        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("NomeUsuCad", DirectionParameter.InputOptional)]
        public string NomeUsuCad { get; set; }

        [PersistenceProperty("NomeUsuImportacao", DirectionParameter.InputOptional)]
        public string NomeUsuImportacao { get; set; }

        #endregion

        #region Propiedades de Suporte

        public bool EditarVisible
        {
            get { return !Protocolo.HasValue; }
        }

        #endregion
    }
}
