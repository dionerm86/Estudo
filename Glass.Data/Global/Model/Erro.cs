using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ErroDAO))]
    [PersistenceClass("erro")]
    public class Erro
    {
        #region Propriedades

        [PersistenceProperty("IDERRO", PersistenceParameterType.IdentityKey)]
        public uint IdErro { get; set; }

        [PersistenceProperty("IDPARENT")]
        public uint? IdParent { get; set; }

        [PersistenceProperty("URLERRO")]
        public string UrlErro { get; set; }

        [PersistenceProperty("TIPOERRO")]
        public string TipoErro { get; set; }

        [PersistenceProperty("MENSAGEM")]
        public string Mensagem { get; set; }

        [PersistenceProperty("TRACE")]
        public string Trace { get; set; }

        [PersistenceProperty("DATAERRO")]
        public DateTime DataErro { get; set; }

        [PersistenceProperty("IDFUNCERRO")]
        public uint IdFuncErro { get; set; }

        [PersistenceProperty("ENVIADO")]
        public bool Enviado { get; set; }

        [PersistenceProperty("NECESSITAENVIO")]
        public bool NecessitaEnvio { get; set; }

        #endregion
    }
}