using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ParcImpostoServDAO))]
    [PersistenceClass("parc_imposto_serv")]
    public class ParcImpostoServ
    {
        #region Propriedades

        [PersistenceProperty("IDPARCIMPOSTOSERV", PersistenceParameterType.IdentityKey)]
        public uint IdParcImpostoServ { get; set; }

        [PersistenceProperty("IDIMPOSTOSERV")]
        public uint IdImpostoServ { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        #endregion
    }
}