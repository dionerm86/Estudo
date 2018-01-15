using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PontosRotaDAO))]
    [PersistenceClass("pontos_rota")]
    public class PontosRota
    {
        #region Propriedades

        [PersistenceProperty("IdPonto", PersistenceParameterType.IdentityKey)]
        public uint IdPonto { get; set; }

        [PersistenceProperty("IdEquipe")]
        public uint? IdEquipe { get; set; }

        [PersistenceProperty("IdMedidor")]
        public uint? IdMedidor { get; set; }

        [PersistenceProperty("DataPonto")]
        public DateTime DataPonto { get; set; }

        [PersistenceProperty("Velocidade")]
        public decimal Velocidade { get; set; }

        [PersistenceProperty("Latitude")]
        public decimal Lat { get; set; }

        [PersistenceProperty("Longitude")]
        public decimal Long { get; set; }

        #endregion
    }
}