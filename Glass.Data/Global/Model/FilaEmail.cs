using System;
using Glass.Data.Helper;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FilaEmailDAO))]
    [PersistenceClass("fila_email")]
    public class FilaEmail
    {
        #region Propriedades

        internal const int MAX_NUMERO_TENTATIVAS = 5;

        [PersistenceProperty("IDEMAIL", PersistenceParameterType.IdentityKey)]
        public uint IdEmail { get; set; }

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("EMAILDESTINATARIO")]
        public string EmailDestinatario { get; set; }

        [PersistenceProperty("ASSUNTO")]
        public string Assunto { get; set; }

        [PersistenceProperty("MENSAGEM")]
        public string Mensagem { get; set; }

        [PersistenceProperty("EMAILENVIO")]
        public Email.EmailEnvio EmailEnvio { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("DATAENVIO")]
        public DateTime? DataEnvio { get; set; }

        [PersistenceProperty("NUMTENTATIVAS")]
        public int NumeroTentativas { get; set; }
        
        [PersistenceProperty("EMAILADMIN")]
        public bool EmailAdmin { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        #endregion
    }
}