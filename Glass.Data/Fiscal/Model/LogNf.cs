using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(LogNfDAO))]
    [PersistenceClass("log_nf")]
    public class LogNf
    {
        #region Propriedades

        [PersistenceProperty("IDLOGNF", PersistenceParameterType.IdentityKey)]
        public uint IdLogNf { get; set; }

        [PersistenceProperty("IDNF")]
        public uint IdNf { get; set; }

        [PersistenceProperty("EVENTO")]
        public string Evento { get; set; }

        [PersistenceProperty("CODIGO")]
        public int Codigo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("IDFUNCLOG")]
        public int? IdFuncLog { get; set; }

        [PersistenceProperty("DATAHORA")]
        public DateTime DataHora { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFUNCIONARIO", DirectionParameter.InputOptional)]
        public string NomeFuncionario { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NumRecibo { get; set; }

        public string NumProtocolo { get; set; }

        #endregion
    }
}