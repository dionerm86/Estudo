using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FilaSmsDAO))]
    [PersistenceClass("fila_sms")]
    public class FilaSms
    {
        #region Propriedades

        internal const int MAX_NUMERO_TENTATIVAS = 5;

        [PersistenceProperty("IDSMS", PersistenceParameterType.IdentityKey)]
        public uint IdSms { get; set; }

        [PersistenceProperty("CODMENSAGEM")]
        public string CodMensagem { get; set; }

        [PersistenceProperty("NOMELOJA")]
        public string NomeLoja { get; set; }

        [PersistenceProperty("CELCLIENTE")]
        public string CelCliente { get; set; }

        [PersistenceProperty("MENSAGEM")]
        public string Mensagem { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("DATAENVIO")]
        public DateTime? DataEnvio { get; set; }

        [PersistenceProperty("NUMTENTATIVAS")]
        public int NumeroTentativas { get; set; }

        [PersistenceProperty("CODRESULTADO")]
        public int CodResultado { get; set; }

        [PersistenceProperty("DESCRICAORESULTADO")]
        public string DescricaoResultado { get; set; }

        [PersistenceProperty("SMSADMIN")]
        public bool SmsAdmin { get; set; }

        #endregion
    }
}