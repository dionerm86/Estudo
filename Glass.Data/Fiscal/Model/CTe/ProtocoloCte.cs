using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ProtocoloCteDAO))]
    [PersistenceClass("protocolo_cte")]
    public class ProtocoloCte
    {
        #region enumeradores

        public enum TipoProtocoloEnum
        {
            Autorizacao,
            Denegacao,
            Cancelamento,
            Inutilizacao
        }    

        #endregion

        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("TIPOPROTOCOLO", PersistenceParameterType.Key)]
        public int TipoProtocolo { get; set; }

        [PersistenceProperty("NUMPROTOCOLO")]
        public string NumProtocolo { get; set; }

        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [PersistenceProperty("NUMRECIBO")]
        public string NumRecibo { get; set; }

        #endregion
    }
}
